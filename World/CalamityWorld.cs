using CalamityMod.Events;
using CalamityMod.NPCs.ExoMechs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.World
{
    // For the sake of ease of access of variables this class remains in its current form (there are likely thousands of references by now).
    // However, all functionalities in the form of hooks have been cleared away in favor of split systems.
    public static class CalamityWorld
    {
        #region Vars
        public static int DoGSecondStageCountdown = 0;
        private const int saveVersion = 0;
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
        public static bool malice = false; // Malice Mode

        // New Temple Altar
        public static int newAltarX = 0;
        public static int newAltarY = 0;

        // Evil Islands
        public static int fehX = 0;
        public static int fehY = 0;

        // Brimstone Crag
        public static int fuhX = 0;
        public static int fuhY = 0;

        // Abyss & Sulphur
        public static bool rainingAcid;
        public static int acidRainPoints = 0;
        public static bool triedToSummonOldDuke = false;
        public static bool startAcidicDownpour = false;
        public static bool forcedRainAlready = false;
        public static bool forcedDownpourWithTear = false;
        public static bool encounteredOldDuke = false;
        public static int forceRainTimer = 0;
        public static int timeSinceAcidRainKill = 0;
        public static int timeSinceAcidStarted = 0;
        public static float AcidRainCompletionRatio
        {
            get
            {
                return MathHelper.Clamp(acidRainPoints / (float)AcidRainEvent.NeededEnemyKills, 0f, 1f);
            }
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

        // Town NPC name chosen bools
        public static bool anglerName = false;
        public static bool armsDealerName = false;
        public static bool clothierName = false;
        public static bool cyborgName = false;
        public static bool demolitionistName = false;
        public static bool dryadName = false;
        public static bool dyeTraderName = false;
        public static bool goblinTinkererName = false;
        public static bool guideName = false;
        public static bool mechanicName = false;
        public static bool merchantName = false;
        public static bool nurseName = false;
        public static bool painterName = false;
        public static bool partyGirlName = false;
        public static bool pirateName = false;
        public static bool skeletonMerchantName = false;
        public static bool steampunkerName = false;
        public static bool stylistName = false;
        public static bool tavernkeepName = false;
        public static bool taxCollectorName = false;
        public static bool travelingMerchantName = false;
        public static bool truffleName = false;
        public static bool witchDoctorName = false;
        public static bool wizardName = false;

        // Draedon Summoning stuff.
        public static int DraedonSummonCountdown = 0;
        public static ExoMech DraedonMechToSummon;
        public static Vector2 DraedonSummonPosition = Vector2.Zero;
        public static bool TalkedToDraedon = false;
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

        #endregion
    }
}
