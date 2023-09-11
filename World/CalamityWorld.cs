using CalamityMod.NPCs.ExoMechs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.World
{
    // For the sake of ease of access of variables this class remains in its current form (there are likely thousands of references by now).
    // However, all functionalities in the form of hooks have been cleared away in favor of split systems.
    public static class CalamityWorld
    {
        #region Vars
        public static int ArmoredDiggerSpawnCooldown = 0;
        public static int MoneyStolenByBandit = 0;
        public static int Reforges;
        public static bool IsWorldAfterDraedonUpdate = false;
        public static ushort[] OreTypes = new ushort[4];

        // Modes
        public static bool onionMode = false; // Extra accessory from Moon Lord
        public static bool revenge = false; // Revengeance Mode
        public static bool death = false; // Death Mode
        public static bool armageddon = false; // Armageddon Mode
        
        // Evaluates to whether vanilla's "Legendary Mode" is enabled (Master Mode on For the Worthy)
        public static bool LegendaryMode => Main.getGoodWorld && ReflectMasterMode();

        // FTW automatically bumps difficulties up and has no proper check for Master since a world generated in Expert Mode will be classified as Master
        // Therefore gotta reflect!
        public static bool ReflectMasterMode()
        {
            FieldInfo findInfo = typeof(Main).GetField("_currentGameModeInfo", BindingFlags.Static | BindingFlags.NonPublic);            
            GameModeData data = (GameModeData)findInfo.GetValue(null);
            return data.IsMasterMode;
        }

        // Sunken Sea
        public static Rectangle SunkenSeaLocation = Rectangle.Empty;

        // Shrines
        public static int[] SChestX = new int[10];
        public static int[] SChestY = new int[10];
        public static bool roxShrinePlaced = false;

        // Planetoids
        public static bool HasGeneratedLuminitePlanetoids = false;

        // Town NPC spawn/home bools
        public static bool spawnedBandit = false;
        public static bool spawnedCirrus = false;
        public static bool foundHomePermafrost = false;

        // Town Pet name chosen bools
        public static bool catName = false;
        public static bool dogName = false;
        public static bool bunnyName = false;

        // Draedon Summoning stuff.
        public static int DraedonSummonCountdown = 0;
        public static ExoMech DraedonMechToSummon;
        public static Vector2 DraedonSummonPosition = Vector2.Zero;
        public static bool TalkedToDraedon = false;
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
    }
}
