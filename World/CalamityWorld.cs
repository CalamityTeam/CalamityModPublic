using CalamityMod.Items.Tools.ClimateChange;
using CalamityMod.NPCs.ExoMechs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.World
{
    // For the sake of ease of access of variables this class remains in its current form (there are likely thousands of references by now).
    // However, all functionalities in the form of hooks have been cleared away in favor of split systems.
    public static class CalamityWorld
    {
        #region Vars
        /// <summary> Tracks the amount of money stolen by the Bandit in this world, in Copper Coins. </summary>
        public static int MoneyStolenByBandit = 0;
        /// <summary>
        /// Tracks the number of reforges which have been performed since the Bandit last refunded the player.<br/>
        /// Used to allow the refund feature to work, and for allowing special Goblin Tinkerer and Bandit dialogue.
        /// </summary>
        public static int Reforges;
        /// <summary>
        /// If false, this world was created before the Draedon Update (1.5.0.001).<br/>
        /// Worlds created before the Draedon Update will not lock specific Draedon's Arsenal recipes behind Schematics.
        /// </summary>
        public static bool IsWorldAfterDraedonUpdate = false;

        // Modes
        /// <summary> If true, the world is in Revengeance Mode. </summary>
        public static bool revenge = false;
        /// <summary> If true, the world is in Death Mode. </summary>
        public static bool death = false;
        /// <summary>
        /// If true, taking damage while a boss is alive will instantly kill the player.<br/>
        /// Unused by Calamity and can only be set externally.
        /// </summary>
        public static bool armageddon = false;

        /// <summary> Evaluates to whether vanilla's "Legendary Mode" is enabled (Master Mode on For the Worthy). </summary>
        public static bool LegendaryMode => Main.getGoodWorld && ReflectMasterMode();

        /// <summary>
        /// Evaluates to whether "Malice Mode" is enabled (Death Mode on For the Worthy).<br/>
        /// Note that the effects of this difficulty are unaffiliated with the removed Malice Mode.
        /// </summary>
        public static bool MaliceMode => Main.getGoodWorld && ReflectMasterMode() && revenge;

        // FTW automatically bumps difficulties up and has no proper check for Master since a world generated in Expert Mode will be classified as Master
        // Therefore gotta reflect!
        public static bool ReflectMasterMode()
        {
            if (Main.GameModeInfo.IsJourneyMode)
            {
                CreativePowers.DifficultySliderPower power = CreativePowerManager.Instance.GetPower<CreativePowers.DifficultySliderPower>();
                float level = power._sliderCurrentValueCache;
                return level == 1f;
            }

            return Main._currentGameModeInfo.IsMasterMode;
        }

        // Sunken Sea
        public static Rectangle SunkenSeaLocation = Rectangle.Empty;

        // Shrines
        public static int[] SChestX = new int[10];
        public static int[] SChestY = new int[10];

        /// <summary> Used to check if the world has generated Luminite planetoids from Moon Lord's defeat. </summary>
        public static bool HasGeneratedLuminitePlanetoids = false;

        /// <summary> If true, the Bandit has lived in this world. Used to ensure their spawn condition is not required on subsequent respawns. </summary>
        public static bool spawnedBandit = false;
        /// <summary> If true, the Archmage has lived in a house in this world. Solely used as a condition for dialogue. </summary>
        public static bool foundHomePermafrost = false;
        /// <summary> If true, the Town Pig is able to move in. </summary>
        public static bool unlockedTownPig = false;

        /// <summary> Used for allowing custom Town Cat patreon names. </summary>
        public static bool catName = false;
        /// <summary> Used for allowing custom Town Dog patreon names. </summary>
        public static bool dogName = false;
        /// <summary> Used for allowing custom Town Bunny patreon names. </summary>
        public static bool bunnyName = false;

        // Draedon Summoning stuff.
        public static int DraedonSummonCountdown = 0;
        public static ExoMech DraedonMechToSummon;
        public static Vector2 DraedonSummonPosition = Vector2.Zero;
        /// <summary> Used to track whether or not Draedon has given their initial monologue when summoned before the Exo Mechs fight for the first time. </summary>
        public static bool TalkedToDraedon = false;
        /// <summary> If true, Draedon will immediately spawn Exo Mechdusa after being summoned. </summary>
        public static bool DraedonMechdusa = false;
        public static bool AbleToSummonDraedon
        {
            get
            {
                if (DraedonSummonCountdown > 0)
                    return false;

                if (NPC.AnyNPCs(ModContent.NPCType<Draedon>()))
                    return false;

                if (NPC.AnyNPCs(ModContent.NPCType<ThanatosHead>()))
                    return false;

                if (NPC.AnyNPCs(ModContent.NPCType<AresBody>()))
                    return false;

                if (NPC.AnyNPCs(ModContent.NPCType<Artemis>()) || NPC.AnyNPCs(ModContent.NPCType<Apollo>()))
                    return false;

                return true;
            }
        }
        public const int DraedonSummonCountdownMax = 260;

        // Draedon Lab Locations.
        public static Vector2 SunkenSeaLabCenter;
        public static Vector2 PlanetoidLabCenter;
        public static Vector2 JungleLabCenter;
        public static Vector2 HellLabCenter;
        public static Vector2 IceLabCenter;
        public static Vector2 CavernLabCenter;

        #endregion

        #region Rain Utils
        public static void StartRain(bool adjustSeverity = false, bool maxSeverity = false, bool worldSync = true)
        {
            int framesInDay = 86400;
            int framesInHour = framesInDay / 24;
            Main.rainTime = Main.rand.Next(framesInHour * 8, framesInDay);
            if (Main.rand.NextBool(3))
            {
                Main.rainTime += Main.rand.Next(0, framesInHour);
            }
            if (Main.rand.NextBool(4))
            {
                Main.rainTime += Main.rand.Next(0, framesInHour * 2);
            }
            if (Main.rand.NextBool(5))
            {
                Main.rainTime += Main.rand.Next(0, framesInHour * 2);
            }
            if (Main.rand.NextBool(6))
            {
                Main.rainTime += Main.rand.Next(0, framesInHour * 3);
            }
            if (Main.rand.NextBool(7))
            {
                Main.rainTime += Main.rand.Next(0, framesInHour * 4);
            }
            if (Main.rand.NextBool(8))
            {
                Main.rainTime += Main.rand.Next(0, framesInHour * 5);
            }
            float randRainExtender = 1f;
            if (Main.rand.NextBool())
            {
                randRainExtender += 0.05f;
            }
            if (Main.rand.NextBool(3))
            {
                randRainExtender += 0.1f;
            }
            if (Main.rand.NextBool(4))
            {
                randRainExtender += 0.15f;
            }
            if (Main.rand.NextBool(5))
            {
                randRainExtender += 0.2f;
            }
            Main.rainTime = (int)(Main.rainTime * randRainExtender);
            Main.raining = true;
            if (adjustSeverity)
                TorrentialTear.AdjustRainSeverity(maxSeverity);

            if (worldSync)
                CalamityNetcode.SyncWorld();
        }

        public static void StopRain(bool clearWeather = false, bool worldSync = true)
        {
            if (clearWeather)
                Main.StopRain();
            else
                Main.raining = false;

            if (worldSync)
                CalamityNetcode.SyncWorld();
        }
        #endregion

        #region Sandstorm Utils
        public static void StartSandstorm()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && !Sandstorm.Happening)
            {
                // If it's not windy enough, make it windy enough for a sandstorm
                // 0.6f is the minimum for vanilla but Calamity changes it to 0.2f
                // Windy days occur when wind speed is at least 0.5f (0.4f in vanilla) so this should never cause a windy day
                float windSpeed = 0f;
                if (Main.windSpeedCurrent == 0f)
                {
                    windSpeed = Main.rand.NextFloat(0.3f, 0.4f) * (Main.rand.Next(0, 2) * 2 - 1);
                }
                else if (Main.windSpeedCurrent < 0.3f && Main.windSpeedCurrent > 0f)
                {
                    windSpeed = Main.rand.NextFloat(0.3f, 0.4f);
                }
                else if (Main.windSpeedCurrent > -0.3f && Main.windSpeedCurrent < 0f)
                {
                    windSpeed = Main.rand.NextFloat(-0.4f, -0.3f);
                }
                if (windSpeed != 0f)
                {
                    Main.windSpeedCurrent = windSpeed < 0f ? -0.3f : 0.3f;
                    Main.windSpeedTarget = windSpeed;
                }
                Sandstorm.StartSandstorm();
            }
        }

        public static void StopSandstorm()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && Sandstorm.Happening)
            {
                Sandstorm.StopSandstorm();
            }
        }
        #endregion

        #region Time Utils
        public static void ResetTime(bool changeToDay)
        {
            Main.time = 0D;
            Main.dayTime = changeToDay;
            CalamityNetcode.SyncWorld();
        }
        #endregion
    }
}
