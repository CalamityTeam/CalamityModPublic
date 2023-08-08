using System.Collections.Generic;
using System.Linq;
using CalamityMod.Balancing;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static Terraria.Player;

namespace CalamityMod
{
    public static partial class CalamityUtils
    {
        #region Stat Retrieval
        public static int GetCurrentDefense(this Player player, bool accountForDefenseDamage = false)
        {
            CalamityPlayer mp = player.Calamity();
            return player.statDefense + (accountForDefenseDamage ? 0 : mp.CurrentDefenseDamage);
        }

        public static float CalcDamage<T>(this Player player, float baseDamage) where T : DamageClass => player.GetTotalDamage<T>().ApplyTo(baseDamage);
        public static int CalcIntDamage<T>(this Player player, float baseDamage) where T : DamageClass => (int)player.CalcDamage<T>(baseDamage);

        // Naively determines the player's chosen (aka "best") class by whichever has the highest damage boost.
        public static DamageClass GetBestClass(this Player player)
        {
            // Check the five Calamity classes to see what the strongest one is, and use that for the typical damage stat.
            float bestDamage = 1f;
            DamageClass bestClass = DamageClass.Generic;

            float melee = player.GetTotalDamage<MeleeDamageClass>().Additive;
            if (melee > bestDamage)
            {
                bestDamage = melee;
                bestClass = DamageClass.Melee;
            }
            float ranged = player.GetTotalDamage<RangedDamageClass>().Additive;
            if (ranged > bestDamage)
            {
                bestDamage = ranged;
                bestClass = DamageClass.Ranged;
            }
            float magic = player.GetTotalDamage<MagicDamageClass>().Additive;
            if (magic > bestDamage)
            {
                bestDamage = magic;
                bestClass = DamageClass.Magic;
            }

            // Summoner intentionally has a reduction. As the only class with no crit, it tends to have higher raw damage than other classes.
            float summon = player.GetTotalDamage<SummonDamageClass>().Additive * BalancingConstants.SummonAllClassScalingFactor;
            if (summon > bestDamage)
            {
                bestDamage = summon;
                bestClass = DamageClass.Summon;
            }
            // We intentionally don't check whip class, because it inherits 100% from Summon

            float rogue = player.GetTotalDamage<RogueDamageClass>().Additive;
            if (rogue > bestDamage)
            {
                bestClass = RogueDamageClass.Instance;
            }

            return bestClass;
        }

        public static StatModifier GetBestClassDamage(this Player player)
        {
            StatModifier ret = StatModifier.Default;
            StatModifier classless = player.GetTotalDamage<GenericDamageClass>();

            // Atypical damage stats are copied from "classless", like Avenger Emblem. This prevents stacking flat damage effects repeatedly.
            ret.Base = classless.Base;
            ret *= classless.Multiplicative;
            ret.Flat = classless.Flat;

            // Check the five Calamity classes to see what the strongest one is, and use that for the typical damage stat.
            float best = 1f;

            float melee = player.GetTotalDamage<MeleeDamageClass>().Additive;
            if (melee > best) best = melee;
            float ranged = player.GetTotalDamage<RangedDamageClass>().Additive;
            if (ranged > best) best = ranged;
            float magic = player.GetTotalDamage<MagicDamageClass>().Additive;
            if (magic > best) best = magic;

            // Summoner intentionally has a reduction. As the only class with no crit, it tends to have higher raw damage than other classes.
            float summon = player.GetTotalDamage<SummonDamageClass>().Additive * BalancingConstants.SummonAllClassScalingFactor;
            if (summon > best) best = summon;
            // We intentionally don't check whip class, because it inherits 100% from Summon

            float rogue = player.GetTotalDamage<RogueDamageClass>().Additive;
            if (rogue > best) best = rogue;

            // Add the best typical damage stat, then return the full modifier.
            ret += best - 1f;
            return ret;
        }

        public static float GetRangedAmmoCostReduction(this Player player)
        {
            // Tally up all possible vanilla effects.
            float vanillaCost = player.ammoBox ? 0.8f : 1f;
            if (player.ammoPotion)
                vanillaCost *= 0.8f;
            if (player.ammoCost80)
                vanillaCost *= 0.8f;
            if (player.ammoCost75)
                vanillaCost *= 0.75f;

            // Account for Calamity effects.
            return vanillaCost * player.Calamity().rangedAmmoCost;
        }

        public static float GetStandingStealthRegen(this Player player)
        {
            CalamityPlayer mp = player.Calamity();
            return (mp.rogueStealthMax / BalancingConstants.BaseStealthGenTime) * mp.stealthGenStandstill;
        }

        public static float GetMovingStealthRegen(this Player player)
        {
            CalamityPlayer mp = player.Calamity();
            return (mp.rogueStealthMax / BalancingConstants.BaseStealthGenTime) * BalancingConstants.MovingStealthGenRatio * mp.stealthGenMoving * mp.stealthAcceleration;
        }

        public static float GetJumpBoost(this Player player) => player.jumpSpeedBoost + (player.wereWolf ? 0.2f : 0f) + (player.jumpBoost ? BalancingConstants.BalloonJumpSpeedBoost : 0f);

        /// <summary>
        /// Calculates and returns the player's total light strength. This is used for Abyss darkness, among other things.<br/>
        /// The Stat Meter also reports this stat.
        /// </summary>
        /// <returns>The player's total light strength.</returns>
        public static int GetCurrentAbyssLightLevel(this Player player)
        {
            CalamityPlayer mp = player.Calamity();
            int light = mp.externalAbyssLight;
            bool underwater = player.IsUnderwater();
            bool miningHelmet = player.head == ArmorIDs.Head.MiningHelmet || player.head == ArmorIDs.Head.UltraBrightHelmet;

            // The campfire bonus does not apply while in the Abyss.
            if (!mp.ZoneAbyss && (player.HasBuff(BuffID.Campfire) || Main.SceneMetrics.HasCampfire))
                light += 1;
            if (mp.camper) // inherits Campfire so it is +2 in practice
                light += 1;
            if (miningHelmet)
                light += 1;
            if (player.hasMagiluminescence)
                light += 1;
            if (player.lightOrb)
                light += 1;
            if (player.crimsonHeart)
                light += 1;
            if (player.magicLantern)
                light += 1;
            if (mp.giantPearl)
                light += 1;
            if (mp.radiator)
                light += 1;
            if (mp.bendyPet)
                light += 1;
            if (mp.sparks)
                light += 1;
            if (mp.thiefsDime)
                light += 1;
            if (mp.fathomSwarmerVisage)
                light += 1;
            if (mp.aquaticHeart)
                light += 1;
            if (mp.aAmpoule)
                light += 1;
            else if (mp.rOoze && !Main.dayTime) // radiant ooze and ampoule/higher don't stack
                light += 1;
            if (mp.aquaticEmblem && underwater)
                light += 1;
            if (player.arcticDivingGear && underwater) // inherited by abyssal diving gear/suit. jellyfish necklace is inherited so arctic diving gear is really +2
                light += 1;
            if (mp.jellyfishNecklace && underwater) // inherited by jellyfish diving gear and higher
                light += 1;
            if (mp.lumenousAmulet && underwater)
                light += 2;
            if (mp.shine)
                light += 2;
            if (mp.blazingCore)
                light += 2;
            if (player.redFairy || player.greenFairy || player.blueFairy)
                light += 2;
            if (mp.babyGhostBell)
                light += underwater ? 2 : 1;
            if (player.petFlagDD2Ghost)
                light += 2;
            if (mp.sirenPet)
                light += underwater ? 3 : 1;
            if (player.petFlagPumpkingPet)
                light += 3;
            if (player.petFlagGolemPet)
                light += 3;
            if (player.petFlagFairyQueenPet)
                light += 3;
            if (player.wisp)
                light += 3;
            if (player.suspiciouslookingTentacle)
                light += 3;
            if (mp.littleLightPet)
                light += 3;
            if (mp.profanedCrystalBuffs && !mp.ZoneAbyss)
                light += (Main.dayTime || player.lavaWet) ? 2 : 1; // not sure how you'd be in lava in the abyss but go ham I guess
            return light;
        }

        /// <summary>
        /// Directly retrieves the best pickaxe power of the player.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static int GetBestPickPower(this Player player)
        {
            int highestPickPower = 35; //35% if you have no pickaxes.
            for (int item = 0; item < Main.InventorySlotsTotal; item++)
            {
                if (player.inventory[item].pick <= 0)
                    continue;

                if (player.inventory[item].pick > highestPickPower)
                    highestPickPower = player.inventory[item].pick;
            }

            return highestPickPower;
        }
        #endregion

        #region Movement and Controls
        public static bool ControlsEnabled(this Player player, bool allowWoFTongue = false)
        {
            if (player.CCed) // Covers frozen (player.frozen), webs (player.webbed), and Medusa (player.stoned)
                return false;
            if (player.tongued && !allowWoFTongue)
                return false;
            return true;
        }
        
        public static bool StandingStill(this Player player, float velocity = 0.05f) => player.velocity.Length() < velocity;

        /// <summary>
        /// Checks if the player is ontop of solid ground. May also check for solid ground for X tiles in front of them
        /// </summary>
        /// <param name="player">The Player whose position is being checked</param>
        /// <param name="solidGroundAhead">How many tiles in front of the player to check</param>
        /// <param name="airExposureNeeded">How many tiles above every checked tile are checked for non-solid ground</param>
        public static bool CheckSolidGround(this Player player, int solidGroundAhead = 0, int airExposureNeeded = 0)
        {
            if (player.velocity.Y != 0) // Player gotta be standing still in any case.
                return false;

            Tile checkedTile;
            bool ConditionMet = true;

            int playerCenterX = (int)player.Center.X / 16;
            int playerCenterY = (int)(player.position.Y + (float)player.height - 1f) / 16 + 1;
            for (int i = 0; i <= solidGroundAhead; i++) // Check i tiles in front of the player.
            {
                ConditionMet = Main.tile[playerCenterX + player.direction * i, playerCenterY].IsTileSolidGround();
                if (!ConditionMet)
                    return ConditionMet;

                for (int j = 1; j <= airExposureNeeded; j++) // Check j tiles ontop of each checked tiles for non-solid ground.
                {
                    checkedTile = Main.tile[playerCenterX + player.direction * i, playerCenterY - j];

                    ConditionMet = !(checkedTile != null && checkedTile.HasUnactuatedTile && Main.tileSolid[checkedTile.TileType]); // IsTileSolidGround minus the ground part, to avoid platforms and other half solid tiles messing it up.
                    if (!ConditionMet)
                        return ConditionMet;
                }
            }
            return ConditionMet;
        }
        #endregion

        #region Location and Biomes
        public static bool IsUnderwater(this Player player) => Collision.DrownCollision(player.position, player.width, player.height, player.gravDir);

        public static bool InSpace(this Player player)
        {
            float x = Main.maxTilesX / 4200f;
            x *= x;
            float spaceGravityMult = (float)((player.position.Y / 16f - (60f + 10f * x)) / (Main.worldSurface / 6.0));
            return spaceGravityMult < 1f;
        }

        public static bool PillarZone(this Player player) => player.ZoneTowerStardust || player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula;

        public static bool InCalamity(this Player player) => player.Calamity().ZoneCalamity;

        public static bool InSunkenSea(this Player player) => player.Calamity().ZoneSunkenSea;

        public static bool InSulphur(this Player player) => player.Calamity().ZoneSulphur;

        public static bool InAstral(this Player player, int biome = 0) //1 is above ground, 2 is underground, 3 is desert
        {
            switch (biome)
            {
                case 1:
                    return player.Calamity().ZoneAstral && (player.ZoneOverworldHeight || player.ZoneSkyHeight);

                case 2:
                    return player.Calamity().ZoneAstral && (player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight || player.ZoneUnderworldHeight);

                case 3:
                    return player.Calamity().ZoneAstral && player.ZoneDesert;

                default:
                    return player.Calamity().ZoneAstral;
            }
        }

        public static bool InAbyss(this Player player, int layer = 0)
        {
            switch (layer)
            {
                case 1:
                    return player.Calamity().ZoneAbyssLayer1;

                case 2:
                    return player.Calamity().ZoneAbyssLayer2;

                case 3:
                    return player.Calamity().ZoneAbyssLayer3;

                case 4:
                    return player.Calamity().ZoneAbyssLayer4;

                default:
                    return player.Calamity().ZoneAbyss;
            }
        }
        #endregion

        #region Inventory Checks
        // TODO -- Wrong. This should return false for weapons which emit true melee projectiles e.g. Arkhalis
        public static bool HoldingProjectileMeleeWeapon(this Player player)
        {
            Item item = player.ActiveItem();
            return item.CountsAsClass<MeleeDamageClass>() && item.shoot != ProjectileID.None;
        }

        public static bool HoldingTrueMeleeWeapon(this Player player) => player.ActiveItem().IsTrueMelee();

        public static bool InventoryHas(this Player player, params int[] items)
        {
            return player.inventory.Any(item => items.Contains(item.type));
        }

        public static bool PortableStorageHas(this Player player, params int[] items)
        {
            bool hasItem = false;
            if (player.bank.item.Any(item => items.Contains(item.type)))
                hasItem = true;
            if (player.bank2.item.Any(item => items.Contains(item.type)))
                hasItem = true;
            if (player.bank3.item.Any(item => items.Contains(item.type)))
                hasItem = true;
            if (player.bank4.item.Any(item => items.Contains(item.type)))
                hasItem = true;
            return hasItem;
        }
        #endregion

        #region Immunity Frames
        /// <summary>
        /// Gives the player the specified number of immunity frames (or "iframes" for short).<br />If the player already has more iframes than you want to give them, this function does nothing.
        /// </summary>
        /// <param name="player">The player who should be given immunity frames.</param>
        /// <param name="frames">The number of immunity frames to give.</param>
        /// <param name="blink">Whether or not the player should be blinking during this time.</param>
        /// <returns>Whether or not any immunity frames were given.</returns>
        public static bool GiveIFrames(this Player player, int frames, bool blink = false)
        {
            // Check to see if there is any way for the player to get iframes from this operation.
            bool anyIFramesWouldBeGiven = false;
            for (int i = 0; i < player.hurtCooldowns.Length; ++i)
                if (player.hurtCooldowns[i] < frames)
                    anyIFramesWouldBeGiven = true;

            // If they would get nothing, don't do it.
            if (!anyIFramesWouldBeGiven)
                return false;

            // Apply iframes thoroughly.
            player.immune = true;
            player.immuneNoBlink = !blink;
            player.immuneTime = frames;
            for (int i = 0; i < player.hurtCooldowns.Length; ++i)
                if (player.hurtCooldowns[i] < frames)
                    player.hurtCooldowns[i] = frames;

            return true;
        }

        /// <summary>
        /// Removes all immunity frames (or "iframes" for short) from the specified player immediately.
        /// </summary>
        /// <param name="player">The player whose iframes should be removed.</param>
        public static void RemoveAllIFrames(this Player player)
        {
            player.immune = false;
            player.immuneNoBlink = false;
            player.immuneTime = 0;
            for (int i = 0; i < player.hurtCooldowns.Length; ++i)
                player.hurtCooldowns[i] = 0;
        }
        #endregion

        #region Rage and Adrenaline
        /// <summary>
        /// Returns the damage multiplier Adrenaline Mode provides for the given player.
        /// </summary>
        /// <param name="mp">The player whose Adrenaline damage should be calculated.</param>
        /// <returns>Adrenaline damage multiplier. 1.0 would be no change.</returns>
        public static float GetAdrenalineDamage(this CalamityPlayer mp)
        {
            float adrenalineBoost = BalancingConstants.AdrenalineDamageBoost;
            if (mp.adrenalineBoostOne)
                adrenalineBoost += BalancingConstants.AdrenalineDamagePerBooster;
            if (mp.adrenalineBoostTwo)
                adrenalineBoost += BalancingConstants.AdrenalineDamagePerBooster;
            if (mp.adrenalineBoostThree)
                adrenalineBoost += BalancingConstants.AdrenalineDamagePerBooster;

            return adrenalineBoost;
        }

        /// <summary>
        /// Returns the damage reduction that holding full Adrenaline provides for the given player.
        /// </summary>
        /// <param name="mp">The player whose Adrenaline DR should be calculated.</param>
        /// <returns>Adrenaline DR. 0f is no DR.</returns>
        public static float GetAdrenalineDR(this CalamityPlayer mp)
        {
            float dr = BalancingConstants.FullAdrenalineDR;
            if (mp.adrenalineBoostOne)
                dr += BalancingConstants.AdrenalineDRPerBooster;
            if (mp.adrenalineBoostTwo)
                dr += BalancingConstants.AdrenalineDRPerBooster;
            if (mp.adrenalineBoostThree)
                dr += BalancingConstants.AdrenalineDRPerBooster;

            return dr;
        }

        /// <summary>
        /// Applies Rage and Adrenaline to the given damage multiplier. The values controlling the so-called "Rippers" can be found in CalamityPlayer.
        /// </summary>
        /// <param name="mp">The CalamityPlayer who may or may not be using Rage or Adrenaline.</param>
        /// <param name="damageMult">A reference to the current in-use damage multiplier. This will be increased in-place.</param>
        public static void ApplyRippersToDamage(CalamityPlayer mp, bool trueMelee, ref float damageMult)
        {
            // Reduce how much true melee benefits from Rage and Adrenaline.
            float rageAndAdrenalineTrueMeleeDamageMult = 0.5f;

            // Rage and Adrenaline now stack additively with no special cases.
            if (mp.rageModeActive)
                damageMult += trueMelee ? mp.RageDamageBoost * rageAndAdrenalineTrueMeleeDamageMult : mp.RageDamageBoost;
            // Draedon's Heart disables Adrenaline damage.
            if (mp.adrenalineModeActive && !mp.draedonsHeart)
                damageMult += trueMelee ? mp.GetAdrenalineDamage() * rageAndAdrenalineTrueMeleeDamageMult : mp.GetAdrenalineDamage();
        }
        #endregion

        #region Cooldowns
        public static bool HasCooldown(this Player p, string id)
        {
            if (p is null)
                return false;
            CalamityPlayer modPlayer = p.Calamity();
            return !(modPlayer is null) && modPlayer.cooldowns.ContainsKey(id);
        }

        /// <summary>
        /// Applies the specified cooldown to the player, creating a new instance automatically.<br/>
        /// By default, overwrites existing instances of this cooldown, but this behavior can be disabled.
        /// </summary>
        /// <param name="p">The player to whom the cooldown should be applied.</param>
        /// <param name="id">The string ID of the cooldown to apply. This is referenced against the Cooldown Registry.</param>
        /// <param name="duration">The duration, in frames, of this instance of the cooldown.</param>
        /// <param name="overwrite">Whether or not to overwrite any existing instances of this cooldown. Defaults to true.</param>
        /// <returns>The cooldown instance which was created. <b>Note the cooldown is always created, but may not be necessarily applied to the player.</b></returns>
        public static CooldownInstance AddCooldown(this Player p, string id, int duration, bool overwrite = true)
        {
            var cd = CooldownRegistry.Get(id);
            CooldownInstance instance = new CooldownInstance(p, cd, duration);

            bool alreadyHasCooldown = p.HasCooldown(id);
            if (!alreadyHasCooldown || overwrite)
            {
                CalamityPlayer mp = p.Calamity();
                mp.cooldowns[id] = instance;
                mp.SyncCooldownAddition(Main.netMode == NetmodeID.Server, instance);
            }

            return instance;
        }

        /// <summary>
        /// Applies the specified cooldown to the player, creating a new instance automatically.<br/>
        /// By default, overwrites existing instances of this cooldown, but this behavior can be disabled.
        /// </summary>
        /// <param name="p">The player to whom the cooldown should be applied.</param>
        /// <param name="id">The string ID of the cooldown to apply. This is referenced against the Cooldown Registry.</param>
        /// <param name="duration">The duration, in frames, of this instance of the cooldown.</param>
        /// <param name="overwrite">Whether or not to overwrite any existing instances of this cooldown. Defaults to true.</param>
        /// <param name="handlerArgs">Arbitrary extra arguments to pass to the CooldownHandler constructor via reflection.</param>
        /// <returns>The cooldown instance which was created. <b>Note the cooldown is always created, but may not be necessarily applied to the player.</b></returns>
        public static CooldownInstance AddCooldown(this Player p, string id, int duration, bool overwrite = true, params object[] handlerArgs)
        {
            var cd = CooldownRegistry.Get(id);
            CooldownInstance instance = new CooldownInstance(p, cd, duration, handlerArgs);

            bool alreadyHasCooldown = p.HasCooldown(id);
            if (!alreadyHasCooldown || overwrite)
                p.Calamity().cooldowns[id] = instance;

            return instance;
        }

        public static IList<CooldownInstance> GetDisplayedCooldowns(this Player p)
        {
            List<CooldownInstance> ret = new List<CooldownInstance>(16);
            if (p is null || p.Calamity() is null)
                return ret;

            foreach (CooldownInstance instance in p.Calamity().cooldowns.Values)
                if (instance.handler.ShouldDisplay)
                    ret.Add(instance);
            return ret;
        }
        #endregion

        #region Arms Control

        /// <summary>
        /// Gets an arm stretch amount from a number ranging from 0 to 1
        /// </summary>
        public static CompositeArmStretchAmount ToStretchAmount(this float percent)
        {
            if (percent < 0.25f)
                return CompositeArmStretchAmount.None;
            if (percent < 0.5f)
                return CompositeArmStretchAmount.Quarter;
            if (percent < 0.75f)
                return CompositeArmStretchAmount.ThreeQuarters;

            return CompositeArmStretchAmount.Full;
        }

        /// <summary>
        /// The exact same thing as Player.GetFrontHandPosition() except it properly accounts for gravity swaps instead of requiring the coders to do it manually afterwards.
        /// Additionally, it simply takes in the arm data instead of asking for the rotation and stretch separately.
        /// </summary>
        public static Vector2 GetFrontHandPositionImproved(this Player player, CompositeArmData arm)
        {
            Vector2 position = player.GetFrontHandPosition(arm.stretch, arm.rotation * player.gravDir).Floor();

            if (player.gravDir == -1f)
            {
                position.Y = player.position.Y + (float)player.height + (player.position.Y - position.Y);
            }

            return position;
        }

        /// <summary>
        /// The exact same thing as Player.GetBackHandPosition() except it properly accounts for gravity swaps instead of requiring the coders to do it manually afterwards.
        /// Additionally, it simply takes in the arm data instead of asking for the rotation and stretch separately.
        /// </summary>
        public static Vector2 GetBackHandPositionImproved(this Player player, CompositeArmData arm)
        {
            Vector2 position = player.GetBackHandPosition(arm.stretch, arm.rotation * player.gravDir).Floor();

            if (player.gravDir == -1f)
            {
                position.Y = player.position.Y + (float)player.height + (player.position.Y - position.Y);
            }

            return position;
        }

        /// <summary>
        /// Properly sets the player's held item rotation and position by doing the annoying math for you, since vanilla decided to be wholly inconsistent about it!
        /// This all assumes the player is facing right. All the flip stuff is automatically handled in here
        /// </summary>
        /// <param name="player">The player for which we set the hold style</param>
        /// <param name="desiredRotation">The desired rotation of the item</param>
        /// <param name="desiredPosition">The desired position of the item</param>
        /// <param name="spriteSize">The size of the item sprite (used in calculations)</param>
        /// <param name="rotationOriginFromCenter">The offset from the center of the sprite of the rotation origin</param>
        /// <param name="noSandstorm">Should the swirly effect from the sandstorm jump be disabled</param>
        /// <param name="flipAngle">Should the angle get flipped with the player, or should it be rotated by 180 degrees</param>
        /// <param name="stepDisplace">Should the item get displaced with the player's height during the walk anim? </param>
        public static void CleanHoldStyle(Player player, float desiredRotation, Vector2 desiredPosition, Vector2 spriteSize, Vector2? rotationOriginFromCenter = null, bool noSandstorm = false, bool flipAngle = false, bool stepDisplace = true)
        {
            if (noSandstorm)
                player.sandStorm = false;

            //Since Vector2.Zero isn't a compile-time constant, we can't use it directly as the default parameter
            if (rotationOriginFromCenter == null)
                rotationOriginFromCenter = Vector2.Zero;

            Vector2 origin = rotationOriginFromCenter.Value;
            //Flip the origin's X position, since the sprite will be flipped if the player faces left.
            origin.X *= player.direction;
            //Additionally, flip the origin's Y position in case the player is in reverse gravity.
            origin.Y *= player.gravDir;

            player.itemRotation = desiredRotation;

            if (flipAngle)
                player.itemRotation *= player.direction;
            else if (player.direction < 0)
                player.itemRotation += MathHelper.Pi;

            //This can anchors the item to rotate around the middle left of its sprite
            //Vector2 consistentLeftAnchor = (player.itemRotation).ToRotationVector2() * -10f * player.direction;

            //This anchors the item to rotate around the center of its sprite.
            Vector2 consistentCenterAnchor = player.itemRotation.ToRotationVector2() * (spriteSize.X / -2f - 10f) * player.direction;

            //This shifts the item so it rotates around the set origin instead
            Vector2 consistentAnchor = consistentCenterAnchor - origin.RotatedBy(player.itemRotation);

            //The sprite needs to be offset by half its sprite size.
            Vector2 offsetAgain = spriteSize * -0.5f;

            Vector2 finalPosition = desiredPosition + offsetAgain + consistentAnchor;

            //Account for the players extra height when stepping
            if (stepDisplace)
            {
                int frame = player.bodyFrame.Y / player.bodyFrame.Height;
                if ((frame > 6 && frame < 10) || (frame > 13 && frame < 17))
                {
                    finalPosition -= Vector2.UnitY * 2f;
                }
            }

            player.itemLocation = finalPosition;
        }
        #endregion

        #region visual layers
        public static void HideAccessories(this Player player, bool hideHeadAccs = true, bool hideBodyAccs = true, bool hideLegAccs = true,  bool hideShield = true)
        {
            if (hideHeadAccs)
                player.face = -1;

            if (hideBodyAccs)
            {
                player.handon = -1;
                player.handoff = -1;
                
                player.back = -1;
                player.front = -1;
                player.neck = -1;
            }

            if (hideLegAccs)
            {
                player.shoe = -1;
                player.waist = -1;
            }

            if (hideShield)
                player.shield = -1;
        }
        #endregion

        /// <summary>
        /// Makes the given player send the given packet to all appropriate receivers.<br />
        /// If server is false, the packet is sent only to the multiplayer host.<br />
        /// If server is true, the packet is sent to all clients except the player it pertains to.
        /// </summary>
        /// <param name="player">The player to whom the packet's data pertains.</param>
        /// <param name="packet">The packet to send with certain parameters.</param>
        /// <param name="server">True if a dedicated server is broadcasting information to all players.</param>
        public static void SendPacket(this Player player, ModPacket packet, bool server)
        {
            // Client: Send the packet only to the host.
            if (!server)
                packet.Send();

            // Server: Send the packet to every OTHER client.
            else
                packet.Send(-1, player.whoAmI);
        }
    }
}
