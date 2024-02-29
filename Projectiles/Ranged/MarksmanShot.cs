using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class MarksmanShot : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        private static readonly int trailLength = 35;
        internal List<Vector2> trailPositions;

        // Lifetime of a bullet. Defined in terms of updates because Terraria's engine is trash.
        internal static readonly int UpdateCount = 8;
        internal static readonly int Lifetime = UpdateCount * CalamityUtils.SecondsToFrames(2);

        // Number of ricochets this coin has performed. This is tracked to enforce ricochet caps (if enabled).
        internal ref float NumRicochets => ref Projectile.ai[0];
        public static readonly int RicochetCap = 999; // 4

        // A counter for how long the shot remains frozen in place when ricocheting off a coin.
        internal ref float RicochetFreezeTimer => ref Projectile.ai[1];
        // The default length of the above freeze timer.
        internal static int RicochetPause = UpdateCount * 3;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 4000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime;
            Projectile.MaxUpdates = UpdateCount;
            Projectile.alpha = 255;
        }

        // Marksman Shots are not immediately deleted on collision (due to their trail streak) and are not allowed to deal damage more than once.
        public override bool? CanDamage() => Projectile.numHits == 0 ? null : false;

        // Bullets stop moving when ricocheting for a certain number of frames.
        public override bool ShouldUpdatePosition() => RicochetFreezeTimer <= 0;

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (Color.Gold * 0.8f).ToVector3() * 0.5f);

            // Do not update the custom old position array if the shot is not currently moving.
            if (ShouldUpdatePosition())
            {
                // If the trail positions array isn't initialized yet, set everything equal to the projectile's current center
                if (trailPositions == null)
                {
                    trailPositions = new List<Vector2>(trailLength);
                    for (int i = 0; i < trailLength; ++i)
                        trailPositions.Add(Projectile.Center);
                }

                // Insert the projectile's current center
                trailPositions.Insert(0, Projectile.Center);

                // Trim excess old position data
                while (trailPositions.Count > trailLength)
                    trailPositions.RemoveAt(trailPositions.Count - 1);
            }

            // If the shot is currently not moving due to being frozen from a ricochet, decrement that timer.
            if (RicochetFreezeTimer > 0f)
                --RicochetFreezeTimer;

            // Otherwise, if the ricochet cap has not been reached, check for collisions with any coins that have not been struck.
            else if (NumRicochets < RicochetCap)
            {
                Projectile[] validCoins = Projectile.GetAvailableCoins();
                Projectile struckCoin = null;

                // Check for collisions with valid coins
                for (int i = 0; i < validCoins.Length; ++i)
                {
                    Projectile coin = validCoins[i];
                    bool hitThisCoin = Collision.CheckAABBvAABBCollision(
                        coin.Hitbox.TopLeft(), coin.Hitbox.Size(), Projectile.Hitbox.TopLeft(), Projectile.Hitbox.Size()
                    );
                    if (hitThisCoin)
                    {
                        struckCoin = coin;
                        break;
                    }
                }

                // If a coin was struck, set up the potential multi-ricoshot.
                if (struckCoin is not null)
                {
                    // Assemble an array of the remaining coins.
                    Projectile[] otherCoins = validCoins.Where((proj) => proj.whoAmI != struckCoin.whoAmI).ToArray();

                    // Ricochet off of this coin to the next target.
                    // This coin (and potential further coins) may have already been recursively frozen.
                    // As long as those coins have not yet been struck, they are valid targets.
                    RicoshotTarget nextTarget = Projectile.FindRicochetTarget(Projectile.Center, otherCoins, true);
                    RicochetOffCoin(nextTarget, struckCoin);

                    // If the next target is another coin, freeze it to (almost) guarantee it will be hit.
                    if (nextTarget.type == RicoshotTargetType.Coin)
                    {
                        Projectile nextCoin = Main.projectile[nextTarget.entityID];
                        nextCoin.ai[1] = RicoshotCoin.RicochetPause;
                    }
                }
            }
        }

        public void RicochetOffCoin(RicoshotTarget target, Projectile struckCoin)
        {
            // Mechanically perform the ricochet (redirect towards the given target)
            float speed = Projectile.velocity.Length();
            Projectile.velocity = Projectile.DirectionTo(target.pos) * speed;

            // If the coin is old enough, and this is the first coin struck, force the projectile as a crit.
            // This is done without editing underlying crit chance.
            //
            // The limitations here are intended to stop the dominant strategy of tossing coins 6 inches straight up and immediately shooting them.
            // Because only the first coin hit can enable the forced crit, you can't get it "for free" by chaining coins. You have to aim at a ripe coin initially.
            CalamityGlobalProjectile cgp = Projectile.Calamity();
            if (NumRicochets == 0 && CalamityUtils.CanRicoshotCoinForceCrit(struckCoin))
                cgp.forcedCrit = true;

            // Apply bonus damage to the projectile. This is an additive multiplier between multiple coins.
            float bonusDamageRatio = struckCoin.ai[0] switch
            {
                1f => NumRicochets > 0 ? RicoshotCoin.SilverMulticoinBonus : RicoshotCoin.SilverBonus,
                2f => NumRicochets > 0 ? RicoshotCoin.GoldMulticoinBonus : RicoshotCoin.GoldBonus,
                _ => NumRicochets > 0 ? RicoshotCoin.CopperMulticoinBonus : RicoshotCoin.CopperBonus,
            };
            cgp.totalRicoshotDamageBonus += bonusDamageRatio;

            // Increment the ricochet count, relegating further coins to only give the multicoin bonus damage.
            NumRicochets++;

            // If the target was valid, freeze the shot in place for a brief moment.
            if (target.IsValid)
                RicochetFreezeTimer = RicochetPause;

            // Play coin ricochet sound ("Ultrabling")
            SoundEngine.PlaySound(RicoshotCoin.BlingHitSound, struckCoin.Center);

            // Apply a little screenshake
            Main.player[Projectile.owner].Calamity().GeneralScreenShakePower = 5;

            // Kill the struck coin immediately after marking it as having been struck.
            struckCoin.localAI[0] = NumRicochets;
            struckCoin.Kill();

            // ORDER easter egg fades in every time a Ricoshot is performed
            if (struckCoin.owner == Main.myPlayer)
            {
                Player coinOwner = Main.player[struckCoin.owner];
                if (coinOwner.name.ToLower() == "v1" || coinOwner.name.ToLower() == "v2" || coinOwner.name.ToLower() == "mirage")
                    ORDERSystem.JUDGMENT();
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

            // Instead of immediately killing the projectile, simply set the time left so the trail can fade out.
            Projectile.timeLeft = Math.Min(Lifetime - Projectile.timeLeft, trailLength);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Midas, 60);

            // Instead of immediately killing the projectile, simply set the time left so the trail can fade out.
            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = Math.Min((Lifetime - Projectile.timeLeft), trailLength);
        }

        internal Color ColorFunction(float completionRatio)
        {
            float fadeOpacity = Math.Min(Projectile.timeLeft / (float)trailLength, 1f);
            return Color.PaleGoldenrod * fadeOpacity;
        }

        internal float WidthFunction(float completionRatio)
        {
            float width = Math.Min(Projectile.timeLeft / (float)trailLength, 1f);
            return (1 - completionRatio) * 6.4f * width;
        }

        // Marksman Shots render entirely as a shader based on previous positions and do not draw a sprite.
        public override bool PreDraw(ref Color lightColor)
        {
            if (trailPositions is null)
                return false;

            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/BasicTrail"));
            PrimitiveSet.Prepare(trailPositions, new(WidthFunction, ColorFunction, (_) => Projectile.Size * 0.5f, false, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), trailLength);

            return false;
        }
    }
}
