using System.Collections.Generic;
using System.Linq;
using CalamityMod.Items.Accessories;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod
{
    public struct RicoshotTarget
    {
        public Vector2 pos;
        public RicoshotTargetType type;
        public int entityID;

        public RicoshotTarget()
        {
            pos = -Vector2.One; // -1, -1
            type = RicoshotTargetType.None;
            entityID = -1;
        }

        public bool IsValid => type != RicoshotTargetType.None;
    }
    
    public static partial class CalamityUtils
    {
        public static int GetActiveRicoshotCoinCount(this Player player)
        {
            int count = 0;
            int id = ModContent.ProjectileType<RicoshotCoin>();

            for (int i = 0; i < Main.maxProjectiles; ++i)
            {
                Projectile p = Main.projectile[i];
                if (p is null || !p.active || p.owner != player.whoAmI)
                    continue;
                if (p.type == id)
                    ++count;
            }
            return count;
        }

        // Intentionally not an extension so it doesn't clog the Visual Studio recommendation lists. This is not generally useful.
        public static bool CanRicoshotCoinForceCrit(Projectile p)
        {
            int id = ModContent.ProjectileType<RicoshotCoin>();
            if (!p.active || p.type != id)
                return false;

            int coinUpdateCount = RicoshotCoin.CoinLifetime - p.timeLeft;
            return coinUpdateCount >= RicoshotCoin.CritDelayTime;
        }

        /// <summary>
        /// Gets the velocity that the player will toss a Ricoshot Coin at based on their current mouse position and velocity.
        /// </summary>
        /// <param name="p">The player who would (or will) toss a coin.</param>
        /// <returns>The appropriate velocity to toss the coin at.</returns>
        public static Vector2 GetCoinTossVelocity(this Player player)
        {
            // As 0,0 in world coordinates is the top left of the world (+Y = down instead of up),
            // we have to flip the Y here to do valid trigonometry.
            Vector2 vectorToMouse = player.Calamity().mouseWorld - player.MountedCenter;
            vectorToMouse.Y *= -1f;

            // All coin tosses are guaranteed to have a baseline gentle upwards velocity.
            // This can be canceled out by pointing your mouse cursor straight down.
            // Again, normally in Terraria +Y is straight down. However we are inverting it here for the math.
            Vector2 tossVelocity = new Vector2(0f, RicoshotCoin.CoinTossForce);

            // Normalize the direction to the mouse.
            Vector2 mouseDirNormalized = vectorToMouse.SafeNormalize(Vector2.UnitY);
            // Add extra coin toss velocity directly towards the mouse.
            tossVelocity += RicoshotCoin.CoinTossForce * mouseDirNormalized;

            // Re-invert the Y coordinates for the game world.
            tossVelocity.Y *= -1f;

            // All coin tosses inherit the player's full velocity.
            Vector2 finalVelocity = player.velocity + tossVelocity;

            // Coins have extra updates, so account for those.
            return finalVelocity / RicoshotCoin.UpdateCount;
        }

        /// <summary>
        /// Gets a list of all coin projectiles that are within valid range of this projectile to set up a multi-ricoshot.
        /// </summary>
        /// <param name="searchingShot">The projectile to search around.</param>
        /// <returns>An array of Projectiles. This array may be empty, but it will never be null.</returns>
        public static Projectile[] GetAvailableCoins(this Projectile searchingShot)
        {
            IList<Projectile> coins = new List<Projectile>();

            // Search for all nearby coins.
            // Coins do not necessarily have to be owned by the same player as the searching projectile!
            int coinID = ModContent.ProjectileType<RicoshotCoin>();
            for (int i = 0; i < Main.maxProjectiles; ++i)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.active || proj.ModProjectile is null || proj.type != coinID)
                    continue;

                // Coins that have already been struck are ignored.
                if (proj.localAI[0] > 0f)
                    continue;

                // Finally, do a distance check
                float dist = searchingShot.DistanceSQ(proj.Center);
                if (dist <= RicoshotCoin.MaxIntraCoinRicoshotDistance * RicoshotCoin.MaxIntraCoinRicoshotDistance)
                    coins.Add(proj);
            }

            return coins.ToArray();
        }

        /// <summary>
        /// Given an array of available Ricoshot Coins, finds the next target of the given projectile's ricoshot.<br />
        /// <br />
        /// Ricoshots prioritize the following targets, in order:<br />
        /// 1) Other coins which are not already part of a ricoshot, including those owned by other players<br />
        /// 2) Daawnlight Spirit Origin bullseyes owned by the shot's owner<br />
        /// 3) Bosses, determined by <see cref="IsABoss(NPC)"/><br />
        /// 4) Any other hostile NPC
        /// </summary>
        /// <param name="theShot">The projectile which is starting (or continuing) a ricoshot.</param>
        /// <param name="startPos">The position to consider for this ricochet. This may not necessarily be the shot's current position.</param>
        /// <param name="availableCoins">An array of available coins for the current stage of the ricoshot.</param>
        /// <param name="considerFrozenCoins">Whether or not already-frozen coins are considered valid targets.<br />
        /// This is <b>true</b> when actually redirecting a shot, as it is expected to strike frozen coins.<br />
        /// This is <b>false</b> when recursively freezing coins (setting up ricoshots) to prevent infinite recursion.</param>
        /// <returns>The next ricochet target. This may not be a valid target.</returns>
        public static RicoshotTarget FindRicochetTarget(this Projectile theShot, Vector2 startPos, Projectile[] availableCoins, bool considerFrozenCoins = false)
        {
            RicoshotTarget ret = default;

            // The first thing coins attempt to ricochet to is other coins.
            // If any coins are available, they will ricochet to the closest possible coin that has line of sight.
            bool foundAnyCoin = false;
            float closestCoinDistance = RicoshotCoin.MaxIntraCoinRicoshotDistance;
            foreach (Projectile coin in availableCoins)
            {
                // Skip coins which are already frozen (part of an ongoing ricoshot), unless considering them
                if (!considerFrozenCoins && coin.ai[1] > 0f)
                    continue;

                // Check line of sight.
                Vector2 wouldBeShotCorner = startPos - theShot.Size / 2f;
                bool los = Collision.CanHitLine(wouldBeShotCorner, theShot.width, theShot.height, coin.position, coin.width, coin.height);
                if (los)
                {
                    // If this coin is closer than the current closest coin, choose it as the target.
                    float coinDistance = coin.Distance(startPos);
                    if (coinDistance < closestCoinDistance)
                    {
                        foundAnyCoin = true;
                        closestCoinDistance = coinDistance;
                        ret.type = RicoshotTargetType.Coin;
                        ret.pos = coin.Center;
                        ret.entityID = coin.whoAmI;
                    }
                }
            }

            // If a coin was found, that is the target. Return it. 
            if (foundAnyCoin)
                return ret;

            // The second thing coins attempt to ricochet to is Daawnlight Spirit Origin bullseyes.
            bool foundAnyBullseye = false;
            float closestBullseyeDistance = DaawnlightSpiritOrigin.RicoshotSearchDistance;
            int bullseyeID = ModContent.ProjectileType<SpiritOriginBullseye>();
            for (int i = 0; i < Main.maxProjectiles; ++i)
            {
                Projectile proj = Main.projectile[i];
                if (proj is null || !proj.active || proj.ModProjectile is null || proj.type != bullseyeID || proj.owner != theShot.owner)
                    continue;

                // Do a distance check.
                float bullseyeDistance = proj.Distance(startPos);
                if (bullseyeDistance < closestBullseyeDistance)
                {
                    foundAnyBullseye = true;
                    closestBullseyeDistance = bullseyeDistance;
                    ret.type = RicoshotTargetType.Bullseye;
                    ret.entityID = proj.whoAmI;

                    NPC bullseyesOwner = Main.npc[(int)proj.ai[0]];
                    float currentSpeed = theShot.velocity.Length();
                    Vector2 superpredictiveVelocity = CalculatePredictiveAimToTarget(startPos, bullseyesOwner, currentSpeed);
                    Vector2 superpredictiveDirection = superpredictiveVelocity.SafeNormalize(Vector2.Zero);
                    Vector2 superprediction = startPos + superpredictiveDirection * bullseyeDistance;
                    ret.pos = Vector2.Lerp(proj.Center, superprediction, RicoshotCoin.SuperpredictionRatio);
                }
            }

            // If a Daawnlight Spirit Origin bullseye was found, that is the target. Return it.
            if (foundAnyBullseye)
                return ret;

            // The third thing coins attempt to ricochet to is hostile NPCs.
            // Bosses are preferred over other possible targets.
            NPC targetNPC = startPos.ClosestNPCAt(RicoshotCoin.RicoshotSearchDistance, false, true);
            if (targetNPC != null)
            {
                ret.type = RicoshotTargetType.NPC;
                ret.entityID = targetNPC.whoAmI;

                float currentSpeed = theShot.velocity.Length();
                Vector2 superpredictiveVelocity = CalculatePredictiveAimToTarget(startPos, targetNPC, currentSpeed);
                Vector2 superpredictiveDirection = superpredictiveVelocity.SafeNormalize(Vector2.Zero);
                Vector2 superprediction = startPos + superpredictiveDirection * targetNPC.Distance(startPos);
                ret.pos = Vector2.Lerp(targetNPC.Center, superprediction, RicoshotCoin.SuperpredictionRatio);
                return ret;
            }

            // If absolutely nothing was found, leave the type at None but give a random destination.
            ret.type = RicoshotTargetType.None;
            ret.pos = startPos + Main.rand.NextVector2Unit() * 32f;
            return ret;
        }
    }
}
