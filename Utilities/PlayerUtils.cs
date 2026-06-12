using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Balancing;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using CalamityMod.Enums;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor;
using CalamityMod.Items.Armor.GodSlayer;
using CalamityMod.Systems.Collections;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
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

        public static int GetDefenseDamageFloor()
        {
            if (BossRushEvent.BossRushActive)
                return BalancingConstants.DefenseDamageFloor_BossRush;
            else if (NPC.downedMoonlord)
            {
                return CalamityWorld.death ? BalancingConstants.DefenseDamageFloor_DeathPML
                    : CalamityWorld.revenge ? BalancingConstants.DefenseDamageFloor_RevPML
                    : BalancingConstants.DefenseDamageFloor_NormalPML;
            }
            else if (Main.hardMode)
            {
                return CalamityWorld.death ? BalancingConstants.DefenseDamageFloor_DeathHM
                    : CalamityWorld.revenge ? BalancingConstants.DefenseDamageFloor_RevHM
                    : BalancingConstants.DefenseDamageFloor_NormalHM;
            }
            else
            {
                return CalamityWorld.death ? BalancingConstants.DefenseDamageFloor_DeathPHM
                    : CalamityWorld.revenge ? BalancingConstants.DefenseDamageFloor_RevPHM
                    : BalancingConstants.DefenseDamageFloor_NormalPHM;
            }
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

            // CIT 28FEB2025: Damage boosts from stealth are now subtracted from total damage when considering best class, to prevent all-class damage from being skewed by it.
            float rogue = player.GetTotalDamage<RogueDamageClass>().Additive - player.Calamity().stealthDamage;
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

            // CIT 28FEB2025: Damage boosts from stealth are now subtracted from total damage when considering best class, to prevent all-class damage from being skewed by it.
            float rogue = player.GetTotalDamage<RogueDamageClass>().Additive - player.Calamity().stealthDamage;
            if (rogue > best) best = rogue;

            // Add the best typical damage stat, then return the full modifier.
            ret += best - 1f;
            return ret;
        }
        /// <summary>
        /// Calculates and returns the player's total melee scale boosts. This is used mostly for melee holdouts.
        /// </summary>
        /// <param name="addHeldItemScale">If the item scale of the players held item should be added to the calculation.<br/>
        /// Should be disabled for anything that doesn't lock you into holding one item.
        /// </param>
        public static float GetMeleeScale(this Player player, bool addHeldItemScale = true)
        {
            float baseScale = 1;
            player.ApplyMeleeScale(ref baseScale); // Gets vanilla's glove scale boosts
            // Xyk 3MARCH2026: Will be implemented better by Doze eventually
            if (addHeldItemScale)
                baseScale += (player.HeldItem.scale - 1);
            if (player.HasBuff(BuffID.Tipsy))
                baseScale += 0.25f;
            if (player.GetModPlayer<IVDripPlayer>().HasAlcohol(AlcoholType.Ale) || player.GetModPlayer<IVDripPlayer>().HasAlcohol(AlcoholType.Sake))
                baseScale += 0.25f;

            return baseScale;
        }
        public static float GetAmmoCostReduction(this Player player)
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
            return vanillaCost * player.Calamity().ammoCost;
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
        public static void SetAbyssLightLevels(this Player player)
        {
            CalamityPlayer mp = player.Calamity();
            bool underwater = player.IsUnderwater();
            bool miningHelmet = player.head == ArmorIDs.Head.MiningHelmet || player.head == ArmorIDs.Head.UltraBrightHelmet;
            //Things that include darkness multipliers
            if (mp.camper)
                mp.abyssPlayerGlowMultiplier += Utils.Remap(player.velocity.Length(), 0, 5, 1, 0);
            if (miningHelmet)
                mp.abyssPlayerGlowMultiplier += 0.2f;
            if (player.nightVision)
                mp.abyssDarkness -= 0.4f;
            if (mp.giantPearl)
                mp.abyssPlayerGlowMultiplier += 0.2f;
            if (mp.fathomSwarmerVisage)
            {
                mp.abyssDarkness -= 0.2f;
                mp.abyssPlayerGlowMultiplier += 0.2f;
                mp.abyssFlashlightWidthMultiplier += 0.5f;
            }
            if (mp.aquaticHeart)
                mp.abyssDarkness -= 0.1f;
            if (mp.jellyfishNecklace && underwater) // inherited by jellyfish diving gear and higher
                mp.abyssPlayerGlowMultiplier += 0.2f;
            if (mp.reaverExplore)
            {
                mp.abyssDarkness -= 0.2f;
                mp.abyssPlayerGlowMultiplier += 0.2f;
                mp.abyssFlashlightWidthMultiplier += 0.5f;
            }
            if (mp.shine)
                mp.abyssPlayerGlowMultiplier += 0.2f;
            if (mp.babyGhostBell && underwater)
                mp.abyssDarkness -= 0.1f;
            if (mp.sirenPet && underwater)
                mp.abyssDarkness -= 0.2f;
            if (mp.littleLightPet)
                mp.abyssDarkness -= 0.4f;
        }

        /// <summary>
        /// Retrieves the best pickaxe item in the player's inventory.<br /></br>
        /// May return <see langword="null"> if the player has no pickaxes.
        /// </summary>
        /// <param name="player">The player whose best pickaxe is being queried.</param>
        /// <returns>An item with a nonzero pickaxe power, or <see langword="null">.</returns>
        internal static Item GetBestPick(this Player player)
        {
            int bestPickPower = 0;
            Item bestPick = null;
            for (int item = 0; item < Main.InventorySlotsTotal; item++)
            {
                if (player.inventory[item].pick <= 0)
                    continue;

                if (player.inventory[item].pick > bestPickPower)
                {
                    bestPick = player.inventory[item];
                    bestPickPower = bestPick.pick;
                }
            }

            return bestPick;
        }

        // Pickaxe power minimum is decided by the Copper Pickaxe.
        private static int _minimumPickPower => ContentSamples.ItemsByType[ItemID.CopperPickaxe].pick;

        /// <summary>
        /// Retrieves the best pickaxe power of the player.<br /></br>
        /// Always returns a minimum pickaxe power equivalent to the Copper Pickaxe.
        /// </summary>
        /// <param name="player">The player whose pickaxe strength is being queried.</param>
        /// <returns>A pickaxe power value, minimum 35.</returns>
        public static int GetBestPickPower(this Player player) => player.GetBestPick()?.pick ?? _minimumPickPower;
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

        // See also: Player.IsStandingStillForSpecialEffects (Vanilla shiny stone + standing still mana regen)
        // That is more or less equivalent to this with the default value of 0.05
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

        /// <summary>
        /// Disables the default wing flap sound from vanilla; this is to stop custom wings playing this sound when they shouldnt
        /// This must be called each update (eg, in the wings item UpdateAccessory method)
        /// </summary>
        /// <param name="player">The Player to disable the sound on</param>
        public static void DisableWingFlapSound(this Player player)
        {
            // vanilla plays a flap sound for all wings barring a few hardcoded exceptions
            // the flapSound flag is used to see if the sound *has been* played, so we set it to true here to prevent it from playing 
            player.flapSound = true;
        }

        #endregion

        #region Location and Biomes
        public static bool IsUnderwater(this Player player) => Collision.DrownCollision(player.position, player.width, player.height, player.gravDir);

        public static bool ReducedSpaceGravity(this Player player)
        {
            float x = Main.maxTilesX / 4200f;
            x *= x;
            float spaceGravityMult = (float)((player.position.Y / 16f - (60f + 10f * x)) / (Main.worldSurface / (Main.remixWorld ? 1.0 : 6.0)));
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
            Item item = player.HeldItem;
            return item.CountsAsClass<MeleeDamageClass>() && item.shoot != ProjectileID.None;
        }

        public static bool HoldingTrueMeleeWeapon(this Player player) => player.HeldItem.IsTrueMelee();

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
        /// Computes the appropriate amount of immunity frames to grant a player when they are struck by an attack.<br />
        /// Accounts for all Calamity effects, but not effects from other mods.
        /// </summary>
        /// <param name="player">The player whose immunity frames are being computed.</param>
        /// <returns>The amount of immunity frames the player should receive if struck.</returns>
        public static int ComputeHitIFrames(this Player player, HurtInfo hurtInfo)
        {
            // Start with vanilla immunity frames.
            int iframes = BalancingConstants.VanillaDefaultIFrames + (player.longInvince ? BalancingConstants.CrossNecklaceIFrameBoost : 0);

            // Add on all Calamity effects.
            int calBonusIFrames = player.GetExtraHitIFrames(hurtInfo);

            return iframes + calBonusIFrames;
        }

        /// <summary>
        /// Gets the total amount of extra immunity frames from a hit granted by various Calamity effects.
        /// </summary>
        /// <param name="player">The player whose extra immunity frames are being computed.</param>
        /// <returns>The amount of extra immunity frames to grant.</returns>
        public static int GetExtraHitIFrames(this Player player, HurtInfo hurtInfo)
        {
            CalamityPlayer modPlayer = player.Calamity();

            int extraIFrames = 0;
            if (modPlayer.godSlayerThrowing && hurtInfo.Damage > GodSlayerHeadRogue.SetBonusHurtDamageThreshold)
                extraIFrames += GodSlayerHeadRogue.ExtraIFrames;

            // Deific Amulet provides 10 to 40 bonus immunity frames when you get hit which scale with your missing health.
            // If you only take 1 damage, you get 5 iframes.
            // This effect is inherited by Rampart of Deities.
            if (modPlayer.dAmulet)
            {
                if (hurtInfo.Damage > 1)
                {
                    float lifeRatio = (float)player.statLife / player.statLifeMax2;
                    float iframeEffectivenessRatio = Utils.GetLerpValue(1.0f, 0.25f, lifeRatio, true);

                    extraIFrames += (int)(iframeEffectivenessRatio * DeificAmulet.MaxBonusIFrames);
                }
                else
                    extraIFrames += 5;
            }

            // Ozzatron 20FEB2024: Moved extra iframes from Seraph Tracers to Rampart of Deities to counteract its loss of Charm of Myths
            // This stacks with the above Deific Amulet effect
            if (modPlayer.rampartOfDeities && hurtInfo.Damage > 200)
                extraIFrames += 30;
            return extraIFrames;
        }

        /// <summary>
        /// Computes the appropriate amount of immunity frames to grant a player when they activate a dodge.<br />
        /// Accounts for all Calamity effects, but not effects from other mods.
        /// </summary>
        /// <param name="player">The player whose immunity frames are being computed.</param>
        /// <returns>The amount of immunity frames the player should receive upon dodging.</returns>
        public static int ComputeDodgeIFrames(this Player player)
        {
            int iframes = BalancingConstants.VanillaDodgeIFrames + (player.longInvince ? BalancingConstants.CrossNecklaceIFrameBoost : 0);
            return iframes;
        }

        /// <summary>
        /// Computes the appropriate amount of immunity frames to grant a player when they activate a parry.<br />
        /// Accounts for all Calamity effects, but not effects from other mods.
        /// </summary>
        /// <param name="player">The player whose immunity frames are being computed.</param>
        /// <returns>The amount of immunity frames the player should receive upon parrying.</returns>
        public static int ComputeParryIFrames(this Player player)
        {
            int iframes = BalancingConstants.VanillaParryIFrames + (player.longInvince ? BalancingConstants.CrossNecklaceIFrameBoost_Parry : 0);
            return iframes;
        }

        // Currently, reflects are functionally equivalent to dodges.
        /// <summary>
        /// Computes the appropriate amount of immunity frames to grant a player when they activate a reflect.<br />
        /// Accounts for all Calamity effects, but not effects from other mods.
        /// </summary>
        /// <param name="player">The player whose immunity frames are being computed.</param>
        /// <returns>The amount of immunity frames the player should receive upon reflecting an attack.</returns>
        public static int ComputeReflectIFrames(this Player player) => player.ComputeDodgeIFrames();

        /// <summary>
        /// Checks whether the player has any kind of immunity frames (or "iframes" for short) available.
        /// </summary>
        /// <param name="player">The player whose immunity frames should be checked.</param>
        /// <returns>Whether or not they are currently in any immunity frames.</returns>
        public static bool HasIFrames(this Player player)
        {
            // Check old school iframes first (aka "cooldown timer -1". Regular hits, falling damage, etc.)
            if (player.immune || player.immuneTime > 0)
                return true;

            // Check more particular iframes. This primarily comes from traps, lava, and bosses.
            for (int i = 0; i < player.hurtCooldowns.Length; i++)
                if (player.hurtCooldowns[i] > 0)
                    return true;

            return false;
        }

        /// <summary>
        /// Gives the player the specified number of immunity frames (or "iframes" for short) to a specific cooldown slot.<br />
        /// If the player already has more iframes than you want to give them, this function does nothing.<br />
        /// <br />
        /// <b>This should be used for effects that need to mock or mimic the iframes that would be granted by getting hit.</b>
        /// </summary>
        /// <param name="player">The player who should be given immunity frames.</param>
        /// <param name="cooldownSlot">The immunity cooldown slot to use. See TML documentation for which is which.</param>
        /// <param name="frames">The number of immunity frames to give.</param>
        /// <param name="blink">Whether or not the player should be blinking during this time.</param>
        /// <returns>Whether or not any immunity frames were given.</returns>
        public static bool GiveIFrames(this Player player, int cooldownSlot, int frames, bool blink = false)
        {
            // Check to see if there is any way for the player to get iframes from this operation.
            bool anyIFramesWouldBeGiven = (cooldownSlot < 0) ? player.immuneTime < frames : player.hurtCooldowns[cooldownSlot] < frames;

            // If they would get nothing, don't do it.
            if (!anyIFramesWouldBeGiven)
                return false;

            // Apply iframes thoroughly. Player.AddImmuneTime is not used because iframes should not exceed the intended amount.
            player.immune = true;
            player.immuneNoBlink = !blink;
            if (cooldownSlot < 0)
                player.immuneTime = frames;
            else
                player.hurtCooldowns[cooldownSlot] = frames;

            return true;
        }

        /// <summary>
        /// Gives the player the specified number of immunity frames (or "iframes" for short) to all cooldown slots.<br />
        /// If the player already has more iframes than you want to give them, this function does nothing.<br />
        /// <br />
        /// <b>This should be used for effects like dodges or true invulnerability that should prevent the player from being hit for a predetermined time.</b>
        /// </summary>
        /// <param name="player">The player who should be given immunity frames.</param>
        /// <param name="frames">The number of immunity frames to give.</param>
        /// <param name="blink">Whether or not the player should be blinking during this time.</param>
        /// <returns>Whether or not any immunity frames were given.</returns>
        public static bool GiveUniversalIFrames(this Player player, int frames, bool blink = false)
        {
            // Check to see if there is any way for the player to get iframes from this operation.
            bool anyIFramesWouldBeGiven = false;
            for (int i = 0; i < player.hurtCooldowns.Length; ++i)
                if (player.hurtCooldowns[i] < frames)
                    anyIFramesWouldBeGiven = true;

            // If they would get nothing, don't do it.
            if (!anyIFramesWouldBeGiven)
                return false;

            // Apply iframes thoroughly. Player.AddImmuneTime is not used because iframes should not exceed the intended amount.
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

        /// <summary>
        /// Lifted from Fargo's. Sets the damage and knockback of an incoming hit to zero, making it not affect the player.
        /// </summary>
        /// <param name="hurtInfo">The HurtInfo instance to nullify.</param>
        public static void NullifyHit(ref this HurtInfo hurtInfo)
        {
            hurtInfo._damage = 0;
            hurtInfo.Knockback = 0;
        }
        #endregion

        #region Player Healing and Lifesteal
        /// <summary>
        /// Directly provides lifesteal to the player.
        /// This obeys the spawn behaviour of lifesteal and incurs cooldown.
        /// </summary>
        /// <param name="player">The player being healed.</param>
        /// <param name="target">The target being hit. Input null if there is none.</param>
        /// <param name="amount">The amount of life being healed.</param>
        /// <param name="cooldownMultiplier">The multiplier to the rate of lifesteal. Increase above 1 to make it weaker and below 1 to make it stronger.</param>
        public static void DoLifestealDirect(this Player player, NPC target, int amount, float cooldownMultiplier = 1f)
        {
            // NPC limitations: disallow if target is not an enemy or deliberately disallowed from lifestealing
            if (target is not null && (!target.IsAnEnemy(false) || !target.canGhostHeal))
                return;

            // Limit the amount of heal to the player's max health
            amount = Math.Min(amount, player.statLifeMax2 - player.statLife);

            // As well as the physical cap to how much HP can be healed
            amount = Math.Min(amount, BalancingConstants.LifeStealCap);

            // Player limitations: disallow if lifesteal variable reaches 0 or lower or Moon Bite is active
            if (amount <= 0 || player.lifeSteal <= 0f || player.moonLeech)
                return;

            // Grant lifesteal and subtract from the variable accordingly
            player.lifeSteal -= amount * cooldownMultiplier;
            player.HealPlayer(amount);
        }

        /// <summary>
        /// Spawns a projectile which grants healing to the player on contact.
        /// This obeys the spawn behaviour of lifesteal and incurs cooldown.
        /// This applies as a projectile on-hit effect. Use DoLifestealDirect for direct lifestealing.
        /// </summary>
        /// <param name="player">The player being healed.</param>
        /// <param name="target">The target being hit. Input null if there is none.</param>
        /// <param name="projSource">The source projectile performing this heal.</param>
        /// <param name="projType">The type of healing projectile spawned.</param>
        /// <param name="amount">The amount of life to heal.</param>
        /// <param name="cooldownMultiplier">The multiplier to the rate of lifesteal. Increase above 1 to make it weaker and below 1 to make it stronger.</param>
        /// <param name="shared">Whether or not the heal is given to the player on the team with lowest HP. False by default.</param>
        /// <param name="distanceRequired">The distance the projectile has to be from the player to direct the heal to when sharing. Default of 3000f (187.5 tiles)</param>
        public static void SpawnLifeStealProjectile(this Player player, NPC target, Projectile projSource, int projType, int amount, float cooldownMultiplier = 1f, bool shared = false, float distanceRequired = 3000f)
        {
            // NPC limitations: disallow if target is not an enemy or deliberately disallowed from lifestealing
            if (target is not null && (!target.IsAnEnemy(false) || !target.canGhostHeal))
                return;

            int lowestHealthCheck = player.statLifeMax2 - player.statLife;
            int targetPlayer = player.whoAmI;
            if (shared)
            {
                foreach (Player otherPlayer in Main.ActivePlayers)
                {
                    if (!otherPlayer.dead && ((!player.hostile && !otherPlayer.hostile) || player.team == otherPlayer.team))
                    {
                        float playerDist = Vector2.Distance(projSource.Center, otherPlayer.Center);
                        if (playerDist < distanceRequired && (otherPlayer.statLifeMax2 - otherPlayer.statLife) > lowestHealthCheck)
                        {
                            lowestHealthCheck = otherPlayer.statLifeMax2 - otherPlayer.statLife;
                            targetPlayer = otherPlayer.whoAmI;
                        }
                    }
                }
            }

            // Limit the amount of heal to the target player's max health
            amount = Math.Min(amount, lowestHealthCheck);

            // As well as the physical cap to how much HP can be healed
            amount = Math.Min(amount, BalancingConstants.LifeStealCap);

            // Player limitations: disallow if lifesteal variable reaches 0 or lower or Moon Bite is active
            // This is performed on the player who DOES the lifesteal, and not the recipient
            if (amount <= 0 || player.lifeSteal <= 0f || player.moonLeech)
                return;

            // Grant lifesteal and subtract from the variable accordingly
            player.lifeSteal -= amount * cooldownMultiplier;
            if (projSource.owner == Main.myPlayer)
                Projectile.NewProjectile(projSource.GetSource_FromThis(), projSource.Center, Vector2.Zero, projType, 0, 0f, projSource.owner, targetPlayer, amount);
        }

        /// <summary>
        /// A short method that heals the player.
        /// All direct heals in Calamity should use this.
        /// </summary>
        /// <param name="player">The player being healed.</param>
        /// <param name="amount">The amount of life being healed.</param>
        /// <param name="healTextType">Whether the heal CombatText should be displayed, and whether it should be synced. Displays and syncs by default.</param>
        public static void HealPlayer(this Player player, int amount, HealTextType healTextType = HealTextType.Broadcast)
        {
            player.statLife += amount;
            if (player.statLife > player.statLifeMax2)
                player.statLife = player.statLifeMax2;
            if (healTextType != HealTextType.None)
                player.HealEffect(amount, healTextType == HealTextType.Broadcast);
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
            // Rage and Adrenaline now stack additively with no special cases.
            if (mp.rageModeActive)
                damageMult += trueMelee ? mp.RageDamageBoost * BalancingConstants.TrueMeleeRipperReductionFactor : mp.RageDamageBoost;
            // Draedon's Heart disables Adrenaline damage.
            if (mp.adrenalineModeActive && !mp.draedonsHeart)
                damageMult += trueMelee ? mp.GetAdrenalineDamage() * BalancingConstants.TrueMeleeRipperReductionFactor : mp.GetAdrenalineDamage();
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
                mp.SyncCooldownAddition(Main.dedServ, instance);
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

            player.itemLocation = finalPosition + new Vector2(spriteSize.X * 0.5f, 0);
        }
        #endregion

        #region Visual Layers
        public static void HideAccessories(this Player player, bool hideHeadAccs = true, bool hideBodyAccs = true, bool hideLegAccs = true, bool hideShield = true)
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
        /// Used to limit the cursor up to a 1080p monitor. A similar method is used for items such as Zenith in vanilla.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <returns>The current position of the player's mouse, clamped to a 1920x1080 screen.</returns>
        public static Vector2 ClampedMouseWorld(this Player player)
        {
            Vector2 mouseWorld = player.Calamity().mouseWorld;

            // Clamp each axis
            mouseWorld.X = mouseWorld.X >= player.MountedCenter.X ? MathF.Min(mouseWorld.X, player.MountedCenter.X + 960f) : MathF.Max(mouseWorld.X, player.MountedCenter.X - 960f);
            mouseWorld.Y = mouseWorld.Y >= player.MountedCenter.Y ? MathF.Min(mouseWorld.Y, player.MountedCenter.Y + 540f) : MathF.Max(mouseWorld.Y, player.MountedCenter.Y - 540f);
            return mouseWorld;
        }

        /// <summary>
        /// A shorthand bool to check if the player can continue using the holdout or not.
        /// </summary>
        /// <param name="player">The player using the holdout.</param>
        /// <returns>Returns <see langword="true"/> if the player CAN'T use the item.</returns>
        public static bool CantUseHoldout(this Player player, bool needsToHold = true) => player == null || !player.active || player.dead || (!player.channel && needsToHold) || player.CCed || player.noItems;

        /// <summary>
        /// A shorthand bool to check if the held item should trigger Calamity's summon damage penalty.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <param name="item">The current held item.</param>
        /// <returns>Returns <see langword="true"/> if the held item should trigger the summon damage penalty.</returns>
        public static bool ShouldTriggerSummonPenalty(Player player, Item item)
        {
            var modPlayer = player.Calamity();

            bool forbiddenWithMagicWeapon = player.armor[0].type == ItemID.AncientBattleArmorHat && player.armor[1].type == ItemID.AncientBattleArmorShirt && player.armor[2].type == ItemID.AncientBattleArmorPants && item.CountsAsClass<MagicDamageClass>();
            bool circletWithRogueWeapon = player.armor[0].type == ModContent.ItemType<ForbiddenCirclet>() && player.armor[1].type == ItemID.AncientBattleArmorShirt && player.armor[2].type == ItemID.AncientBattleArmorPants && item.CountsAsClass<RogueDamageClass>();
            bool gemTechBlueGem = modPlayer.GemTechSet && modPlayer.GemTechState.IsBlueGemActive;

            bool crossClassNerfDisabled = forbiddenWithMagicWeapon || circletWithRogueWeapon || modPlayer.fearmongerSet || gemTechBlueGem || modPlayer.profanedCrystalBuffs || DD2Event.Ongoing;

            if (item.type > ItemID.None && !crossClassNerfDisabled)
            {
                bool heldItemIsClassedWeapon = !item.CountsAsClass<SummonDamageClass>() && (
                    item.CountsAsClass<MeleeDamageClass>() ||
                    item.CountsAsClass<RangedDamageClass>() ||
                    item.CountsAsClass<MagicDamageClass>() ||
                    item.CountsAsClass<ThrowingDamageClass>()
                );

                bool heldItemIsTool = (item.pick > 0 || item.axe > 0 || item.hammer > 0) && !CalamityItemSets.WeaponWithToolPowerAffectedBySummonPenalty[item.type];
                bool heldItemCanBeUsed = item.useStyle != ItemUseStyleID.None;
                bool heldItemIsAccessoryOrAmmo = item.accessory || item.ammo != AmmoID.None;
                bool heldItemIsExcludedByModCall = CalamityItemSets.ItemWhichDisablesSummonerNerf[item.type];

                if (heldItemIsClassedWeapon && heldItemCanBeUsed && !heldItemIsTool && !heldItemIsAccessoryOrAmmo && !heldItemIsExcludedByModCall)
                    return true;
            }
            return false;
        }

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
