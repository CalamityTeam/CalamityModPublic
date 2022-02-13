using CalamityMod;
using CalamityMod.CustomRecipes;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.AstralDesert;
using CalamityMod.Tiles.AstralSnow;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.Ores;
using CalamityMod.Tiles.SunkenSea;
using CalamityMod.World.Planets;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;

namespace CalamityMod.World
{
    public partial class CalamityWorld : ModWorld
    {
        #region Vars
        public static int DoGSecondStageCountdown = 0;
        private const int saveVersion = 0;
        public static int ArmoredDiggerSpawnCooldown = 0;
        public static int MoneyStolenByBandit = 0;
        public static int Reforges;
        public static bool IsWorldAfterDraedonUpdate = false;
        public static ushort[] OreTypes = new ushort[4];

        // Boss Rush
        public static int bossRushHostileProjKillCounter = 0;

        // Death Mode natural boss spawns
        public static int bossSpawnCountdown = 0; // Death Mode natural boss spawn countdown
        public static int bossType = 0; // Death Mode natural boss spawn type
        public static int deathBossSpawnCooldown = 0; // Cooldown between Death Mode natural boss spawns

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
        public static int calamityTiles = 0;

        // Abyss & Sulphur
        public static int numAbyssIslands = 0;
        public static int[] AbyssIslandX = new int[20];
        public static int[] AbyssIslandY = new int[20];
        public static int[] AbyssItemArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        public static int sulphurTiles = 0;
        public static int abyssTiles = 0;
        public static bool abyssSide = false;
        public static int abyssChasmBottom = 0;
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

        // Astral
        public static int astralTiles = 0;

        // Sunken Sea
        public static int sunkenSeaTiles = 0;
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

        #region Downed Bools
        public static bool downedDesertScourge = false;
        public static bool downedCrabulon = false;
        public static bool downedHiveMind = false;
        public static bool downedPerforator = false;
        public static bool downedSlimeGod = false;
        public static bool downedCryogen = false;
        public static bool downedAquaticScourge = false;
        public static bool downedBrimstoneElemental = false;
        public static bool downedCalamitas = false;
        public static bool downedLeviathan = false;
        public static bool downedAstrageldon = false;
        public static bool downedStarGod = false;
        public static bool downedPlaguebringer = false;
        public static bool downedScavenger = false;
        public static bool downedBoomerDuke = false;
        public static bool downedGuardians = false;
        public static bool downedProvidence = false;
        public static bool downedSentinel1 = false; // Ceaseless Void
        public static bool downedSentinel2 = false; // Storm Weaver
        public static bool downedSentinel3 = false; // Signus, Envoy of the Devourer
        public static bool downedSecondSentinels = false;
        public static bool downedPolterghast = false;
        public static bool downedDoG = false;
        public static bool downedBumble = false;
        public static bool downedYharon = false;
        public static bool downedExoMechs = false;
        public static bool downedSCal = false;
        public static bool downedAdultEidolonWyrm = false;
        public static bool downedGSS = false;
        public static bool downedCLAM = false;
        public static bool downedCLAMHardMode = false;
        public static bool downedBetsy = false; // Betsy

        // These are purely used for loot drops, nothing else
        public static bool downedAres = false;
        public static bool downedThanatos = false;
        public static bool downedArtemisAndApollo = false;

        public static bool downedEoCAcidRain = false;
        public static bool downedAquaticScourgeAcidRain = false;
        #endregion

        #endregion

        #region Initialize
        public override void Initialize()
        {
            NPC.LunarShieldPowerExpert = 100;

            CalamityGlobalNPC.holyBoss = -1;
            CalamityGlobalNPC.doughnutBoss = -1;
            CalamityGlobalNPC.voidBoss = -1;
            CalamityGlobalNPC.energyFlame = -1;
            CalamityGlobalNPC.hiveMind = -1;
            CalamityGlobalNPC.scavenger = -1;

            for (int i = 0; i < CalamityGlobalNPC.bobbitWormBottom.Length; i++)
                CalamityGlobalNPC.bobbitWormBottom[i] = -1;

            CalamityGlobalNPC.DoGHead = -1;
            CalamityGlobalNPC.SCal = -1;
            CalamityGlobalNPC.ghostBoss = -1;
            CalamityGlobalNPC.laserEye = -1;
            CalamityGlobalNPC.fireEye = -1;
            CalamityGlobalNPC.brimstoneElemental = -1;
            CalamityGlobalNPC.signus = -1;
			CalamityGlobalNPC.draedonExoMechPrimePlasmaCannon = -1;
			CalamityGlobalNPC.draedonExoMechPrime = -1;
            CalamityGlobalNPC.draedonExoMechTwinGreen = -1;
            CalamityGlobalNPC.draedonExoMechTwinRed = -1;
            CalamityGlobalNPC.draedonExoMechWorm = -1;
            CalamityGlobalNPC.adultEidolonWyrmHead = -1;
            BossRushEvent.BossRushStage = 0;
            DoGSecondStageCountdown = 0;
            ArmoredDiggerSpawnCooldown = 0;
            BossRushEvent.BossRushActive = false;
            BossRushEvent.BossRushSpawnCountdown = 180;
            bossSpawnCountdown = 0;
            bossRushHostileProjKillCounter = 0;
            deathBossSpawnCooldown = 0;
            bossType = 0;
            newAltarX = 0;
            newAltarY = 0;
            abyssChasmBottom = 0;
            abyssSide = false;

            spawnedBandit = false;
            spawnedCirrus = false;
            foundHomePermafrost = false;

            anglerName = false;
            armsDealerName = false;
            clothierName = false;
            cyborgName = false;
            demolitionistName = false;
            dryadName = false;
            dyeTraderName = false;
            goblinTinkererName = false;
            guideName = false;
            mechanicName = false;
            merchantName = false;
            nurseName = false;
            painterName = false;
            partyGirlName = false;
            pirateName = false;
            skeletonMerchantName = false;
            steampunkerName = false;
            stylistName = false;
            tavernkeepName = false;
            taxCollectorName = false;
            travelingMerchantName = false;
            truffleName = false;
            witchDoctorName = false;
            wizardName = false;

            downedDesertScourge = false;
            downedAquaticScourge = false;
            downedHiveMind = false;
            downedPerforator = false;
            downedSlimeGod = false;
            downedCryogen = false;
            downedBrimstoneElemental = false;
            downedCalamitas = false;
            downedLeviathan = false;
            downedDoG = false;
            downedPlaguebringer = false;
            downedScavenger = false;
            downedGuardians = false;
            downedProvidence = false;
            downedSentinel1 = false;
            downedSentinel2 = false;
            downedSentinel3 = false;
            downedYharon = false;
            downedExoMechs = false;
            downedAres = false;
            downedThanatos = false;
            downedArtemisAndApollo = false;
            TalkedToDraedon = false;
            downedSCal = false;
            downedAdultEidolonWyrm = false;
            downedCLAM = false;
            downedCLAMHardMode = false;
            downedBumble = false;
            downedCrabulon = false;
            downedBetsy = false;
            onionMode = false;
            revenge = false;
            downedStarGod = false;
            downedAstrageldon = false;
            downedPolterghast = false;
            downedGSS = false;
            downedBoomerDuke = false;
            downedSecondSentinels = false;
            death = false;
            armageddon = false;
            malice = false;
            rainingAcid = false;
            downedEoCAcidRain = false;
            downedAquaticScourgeAcidRain = false;
            forceRainTimer = 0;
        }
        #endregion

        #region Save
        public override TagCompound Save()
        {
            var downed = new List<string>();
            if (downedDesertScourge)
                downed.Add("desertScourge");
            if (downedAquaticScourge)
                downed.Add("aquaticScourge");
            if (downedHiveMind)
                downed.Add("hiveMind");
            if (downedPerforator)
                downed.Add("perforator");
            if (downedSlimeGod)
                downed.Add("slimeGod");
            if (downedCryogen)
                downed.Add("cryogen");
            if (downedBrimstoneElemental)
                downed.Add("brimstoneElemental");
            if (downedCalamitas)
                downed.Add("calamitas");
            if (downedLeviathan)
                downed.Add("leviathan");
            if (downedDoG)
                downed.Add("devourerOfGods");
            if (downedPlaguebringer)
                downed.Add("plaguebringerGoliath");
            if (downedGuardians)
                downed.Add("guardians");
            if (downedProvidence)
                downed.Add("providence");
            if (downedSentinel1)
                downed.Add("ceaselessVoid");
            if (downedSentinel2)
                downed.Add("stormWeaver");
            if (downedSentinel3)
                downed.Add("signus");
            if (downedSecondSentinels)
                downed.Add("secondSentinels");
            if (downedYharon)
                downed.Add("yharon");
            if (downedExoMechs)
                downed.Add("exoMechs");
            if (downedAres)
                downed.Add("ares");
            if (downedThanatos)
                downed.Add("thanatos");
            if (downedArtemisAndApollo)
                downed.Add("artemisAndApollo");
            if (TalkedToDraedon)
                downed.Add("TalkedToDraedon");
            if (downedSCal)
                downed.Add("supremeCalamitas");
            if (downedAdultEidolonWyrm)
                downed.Add("adultEidolonWyrm");
            if (downedBumble)
                downed.Add("bumblebirb");
            if (downedCrabulon)
                downed.Add("crabulon");
            if (downedBetsy)
                downed.Add("betsy");
            if (downedScavenger)
                downed.Add("scavenger");
            if (onionMode)
                downed.Add("onionMode");
            if (revenge)
                downed.Add("revenge");
            if (downedStarGod)
                downed.Add("starGod");
            if (downedAstrageldon)
                downed.Add("astrageldon");
            if (downedPolterghast)
                downed.Add("polterghast");
            if (downedGSS)
                downed.Add("greatSandShark");
            if (downedBoomerDuke)
                downed.Add("oldDuke");
            if (death)
                downed.Add("death");
            if (armageddon)
                downed.Add("armageddon");
            if (malice)
                downed.Add("malice");
            if (abyssSide)
                downed.Add("abyssSide");
            if (BossRushEvent.BossRushActive)
                downed.Add("bossRushActive");
            if (downedCLAM)
                downed.Add("clam");
            if (downedCLAMHardMode)
                downed.Add("clamHardmode");
            if (rainingAcid)
                downed.Add("acidRain");
            if (spawnedBandit)
                downed.Add("bandit");
            if (spawnedCirrus)
                downed.Add("drunkPrincess");
            if (foundHomePermafrost)
                downed.Add("archmageHome");

            #region Save NPC Names
            if (anglerName)
                downed.Add("anglerName");
            if (armsDealerName)
                downed.Add("armsDealerName");
            if (clothierName)
                downed.Add("clothierName");
            if (cyborgName)
                downed.Add("cyborgName");
            if (demolitionistName)
                downed.Add("demolitionistName");
            if (dryadName)
                downed.Add("dryadName");
            if (dyeTraderName)
                downed.Add("dyeTraderName");
            if (goblinTinkererName)
                downed.Add("goblinTinkererName");
            if (guideName)
                downed.Add("guideName");
            if (mechanicName)
                downed.Add("mechanicName");
            if (merchantName)
                downed.Add("merchantName");
            if (nurseName)
                downed.Add("nurseName");
            if (painterName)
                downed.Add("painterName");
            if (partyGirlName)
                downed.Add("partyGirlName");
            if (pirateName)
                downed.Add("pirateName");
            if (skeletonMerchantName)
                downed.Add("skeletonMerchantName");
            if (steampunkerName)
                downed.Add("steampunkerName");
            if (stylistName)
                downed.Add("stylistName");
            if (tavernkeepName)
                downed.Add("tavernkeepName");
            if (taxCollectorName)
                downed.Add("taxCollectorName");
            if (travelingMerchantName)
                downed.Add("travelingMerchantName");
            if (truffleName)
                downed.Add("truffleName");
            if (witchDoctorName)
                downed.Add("witchDoctorName");
            if (wizardName)
                downed.Add("wizardName");
            #endregion

            if (downedEoCAcidRain)
                downed.Add("eocRain");
            if (downedAquaticScourgeAcidRain)
                downed.Add("hmRain");
            if (triedToSummonOldDuke)
                downed.Add("spawnedBoomer");
            if (startAcidicDownpour)
                downed.Add("startDownpour");
            if (forcedRainAlready)
                downed.Add("forcedRain");
            if (forcedDownpourWithTear)
                downed.Add("forcedTear");
            if (encounteredOldDuke)
                downed.Add("encounteredOldDuke");
            if (HasGeneratedLuminitePlanetoids)
                downed.Add("HasGeneratedLuminitePlanetoids");
            downed.AddWithCondition("IsWorldAfterDraedonUpdate", IsWorldAfterDraedonUpdate);

            downed.AddWithCondition("TinOreWorld", OreTypes[0] == TileID.Tin);
            downed.AddWithCondition("LeadOreWorld", OreTypes[1] == TileID.Lead);
            downed.AddWithCondition("TungstenOreWorld", OreTypes[2] == TileID.Tungsten);
            downed.AddWithCondition("PlatinumOreWorld", OreTypes[3] == TileID.Platinum);

            RecipeUnlockHandler.Save(downed);

            return new TagCompound
            {
                ["downed"] = downed,
                ["abyssChasmBottom"] = abyssChasmBottom,
                ["acidRainPoints"] = acidRainPoints,
                ["Reforges"] = Reforges,
                ["MoneyStolenByBandit"] = MoneyStolenByBandit,

                ["SunkenSeaLabCenter"] = SunkenSeaLabCenter,
                ["PlanetoidLabCenter"] = PlanetoidLabCenter,
                ["JungleLabCenter"] = JungleLabCenter,
                ["HellLabCenter"] = HellLabCenter,
                ["IceLabCenter"] = IceLabCenter
            };
        }
        #endregion

        #region Load
        public override void Load(TagCompound tag)
        {
            var downed = tag.GetList<string>("downed");
            downedDesertScourge = downed.Contains("desertScourge");
            downedAquaticScourge = downed.Contains("aquaticScourge");
            downedHiveMind = downed.Contains("hiveMind");
            downedPerforator = downed.Contains("perforator");
            downedSlimeGod = downed.Contains("slimeGod");
            downedCryogen = downed.Contains("cryogen");
            downedBrimstoneElemental = downed.Contains("brimstoneElemental");
            downedCalamitas = downed.Contains("calamitas");
            downedLeviathan = downed.Contains("leviathan");
            downedDoG = downed.Contains("devourerOfGods");
            downedPlaguebringer = downed.Contains("plaguebringerGoliath");
            downedGuardians = downed.Contains("guardians");
            downedProvidence = downed.Contains("providence");
            downedSentinel1 = downed.Contains("ceaselessVoid");
            downedSentinel2 = downed.Contains("stormWeaver");
            downedSentinel3 = downed.Contains("signus");
            downedSecondSentinels = downed.Contains("secondSentinels");
            downedYharon = downed.Contains("yharon");
            downedExoMechs = downed.Contains("exoMechs");
            downedAres = downed.Contains("ares");
            downedThanatos = downed.Contains("thanatos");
            downedArtemisAndApollo = downed.Contains("artemisAndApollo");
            TalkedToDraedon = downed.Contains("TalkedToDraedon");
            downedSCal = downed.Contains("supremeCalamitas");
            downedAdultEidolonWyrm = downed.Contains("adultEidolonWyrm");
            downedBumble = downed.Contains("bumblebirb");
            downedCrabulon = downed.Contains("crabulon");
            downedBetsy = downed.Contains("betsy");
            downedScavenger = downed.Contains("scavenger");
            onionMode = downed.Contains("onionMode");
            revenge = downed.Contains("revenge");
            downedStarGod = downed.Contains("starGod");
            downedAstrageldon = downed.Contains("astrageldon");
            downedPolterghast = downed.Contains("polterghast");
            downedGSS = downed.Contains("greatSandShark");
            downedBoomerDuke = downed.Contains("oldDuke");
            death = downed.Contains("death");
            armageddon = downed.Contains("armageddon");
            malice = downed.Contains("malice");
            abyssSide = downed.Contains("abyssSide");
            BossRushEvent.BossRushActive = downed.Contains("bossRushActive");
            downedCLAM = downed.Contains("clam");
            downedCLAMHardMode = downed.Contains("clamHardmode");
            rainingAcid = downed.Contains("acidRain");

            spawnedBandit = downed.Contains("bandit");
            spawnedCirrus = downed.Contains("drunkPrincess");
            foundHomePermafrost = downed.Contains("archmageHome");

            #region Load NPC Names
            anglerName = downed.Contains("anglerName");
            armsDealerName = downed.Contains("armsDealerName");
            clothierName = downed.Contains("clothierName");
            cyborgName = downed.Contains("cyborgName");
            demolitionistName = downed.Contains("demolitionistName");
            dryadName = downed.Contains("dryadName");
            dyeTraderName = downed.Contains("dyeTraderName");
            goblinTinkererName = downed.Contains("goblinTinkererName");
            guideName = downed.Contains("guideName");
            mechanicName = downed.Contains("mechanicName");
            merchantName = downed.Contains("merchantName");
            nurseName = downed.Contains("nurseName");
            painterName = downed.Contains("painterName");
            partyGirlName = downed.Contains("partyGirlName");
            pirateName = downed.Contains("pirateName");
            skeletonMerchantName = downed.Contains("skeletonMerchantName");
            steampunkerName = downed.Contains("steampunkerName");
            stylistName = downed.Contains("stylistName");
            tavernkeepName = downed.Contains("tavernkeepName");
            taxCollectorName = downed.Contains("taxCollectorName");
            travelingMerchantName = downed.Contains("travelingMerchantName");
            truffleName = downed.Contains("truffleName");
            witchDoctorName = downed.Contains("witchDoctorName");
            wizardName = downed.Contains("wizardName");
            #endregion

            downedEoCAcidRain = downed.Contains("eocRain");
            downedAquaticScourgeAcidRain = downed.Contains("hmRain");
            triedToSummonOldDuke = downed.Contains("spawnedBoomer");
            startAcidicDownpour = downed.Contains("startDownpour");
            forcedRainAlready = downed.Contains("forcedRain");
            forcedDownpourWithTear = downed.Contains("forcedTear");
            encounteredOldDuke = downed.Contains("encounteredOldDuke");
            HasGeneratedLuminitePlanetoids = downed.Contains("HasGeneratedLuminitePlanetoids");
            IsWorldAfterDraedonUpdate = downed.Contains("IsWorldAfterDraedonUpdate");

            OreTypes[0] = downed.Contains("TinOreWorld") ? TileID.Tin : TileID.Copper;
            OreTypes[1] = downed.Contains("LeadOreWorld") ? TileID.Lead : TileID.Iron;
            OreTypes[2] = downed.Contains("TungstenOreWorld") ? TileID.Tungsten : TileID.Silver;
            OreTypes[3] = downed.Contains("PlatinumOreWorld") ? TileID.Platinum : TileID.Gold;

            RecipeUnlockHandler.Load(downed);

            abyssChasmBottom = tag.GetInt("abyssChasmBottom");
            acidRainPoints = tag.GetInt("acidRainPoints");
            Reforges = tag.GetInt("Reforges");
            MoneyStolenByBandit = tag.GetInt("MoneyStolenByBandit");

            SunkenSeaLabCenter = tag.Get<Vector2>("SunkenSeaLabCenter");
            PlanetoidLabCenter = tag.Get<Vector2>("PlanetoidLabCenter");
            JungleLabCenter = tag.Get<Vector2>("JungleLabCenter");
            HellLabCenter = tag.Get<Vector2>("HellLabCenter");
            IceLabCenter = tag.Get<Vector2>("IceLabCenter");
        }
        #endregion

        #region LoadLegacy
        public override void LoadLegacy(BinaryReader reader)
        {
            int loadVersion = reader.ReadInt32();
            abyssChasmBottom = reader.ReadInt32();
            acidRainPoints = reader.ReadInt32();
            Reforges = reader.ReadInt32();
            MoneyStolenByBandit = reader.ReadInt32();

            if (loadVersion == 0)
            {
                BitsByte flags = reader.ReadByte();
                downedDesertScourge = flags[0];
                downedHiveMind = flags[1];
                downedPerforator = flags[2];
                downedSlimeGod = flags[3];
                downedCryogen = flags[4];
                downedBrimstoneElemental = flags[5];
                downedCalamitas = flags[6];
                downedLeviathan = flags[7];

                BitsByte flags2 = reader.ReadByte();
                downedDoG = flags2[0];
                downedPlaguebringer = flags2[1];
                downedGuardians = flags2[2];
                downedProvidence = flags2[3];
                downedSentinel1 = flags2[4];
                downedSentinel2 = flags2[5];
                downedSentinel3 = flags2[6];
                downedYharon = flags2[7];

                // Explicitly discard the now-unused vanilla boss booleans
                BitsByte flags3 = reader.ReadByte();
                downedSCal = flags3[0];
                downedBumble = flags3[1];
                downedCrabulon = flags3[2];
                downedBetsy = flags3[3];
                downedScavenger = flags3[4];
                _ = flags3[5];
                _ = flags3[6];
                _ = flags3[7];

                BitsByte flags4 = reader.ReadByte();
                _ = flags4[0];
                _ = flags4[1];
                _ = flags4[2];
                _ = flags4[3];
                _ = flags4[4];
                _ = flags4[5];
                onionMode = flags4[6];
                revenge = flags4[7];

                BitsByte flags5 = reader.ReadByte();
                downedStarGod = flags5[0];
                spawnedBandit = flags5[1];
                spawnedCirrus = flags5[2];
                startAcidicDownpour = flags5[3];
                _ = flags5[4];
                downedPolterghast = flags5[5];
                death = flags5[6];
                downedGSS = flags5[7];

                BitsByte flags6 = reader.ReadByte();
                abyssSide = flags6[0];
                downedAquaticScourge = flags6[1];
                downedAstrageldon = flags6[2];
                _ = flags6[3];
                armageddon = flags6[4];
                _ = flags6[5];
                _ = flags6[6];
                _ = flags6[7];

                BitsByte flags7 = reader.ReadByte();
                BossRushEvent.BossRushActive = flags7[0];
                downedBoomerDuke = flags7[1];
                downedCLAM = flags7[2];
                _ = flags7[3];
                rainingAcid = flags7[4];
                downedEoCAcidRain = flags7[5];
                downedAquaticScourgeAcidRain = flags7[6];
                triedToSummonOldDuke = flags7[7];

                BitsByte flags8 = reader.ReadByte();
                forcedRainAlready = flags8[0];
                forcedDownpourWithTear = flags8[1];
                downedSecondSentinels = flags8[2];
                foundHomePermafrost = flags8[3];
                downedCLAMHardMode = flags8[4];
                guideName = flags8[5];
                wizardName = flags8[6];
                steampunkerName = flags8[7];

                BitsByte flags9 = reader.ReadByte();
                stylistName = flags9[0];
                witchDoctorName = flags9[1];
                taxCollectorName = flags9[2];
                pirateName = flags9[3];
                mechanicName = flags9[4];
                armsDealerName = flags9[5];
                dryadName = flags9[6];
                nurseName = flags9[7];

                BitsByte flags10 = reader.ReadByte();
                anglerName = flags10[0];
                clothierName = flags10[1];
                encounteredOldDuke = flags10[2];
                travelingMerchantName = flags10[3];
                _ = flags10[4];
                _ = flags10[5];
                _ = flags10[6];
                _ = flags10[7];

                BitsByte flags11 = reader.ReadByte();
                malice = flags11[0];
                HasGeneratedLuminitePlanetoids = flags11[1];
                downedAdultEidolonWyrm = flags11[2];
                downedExoMechs = flags11[3];
                downedAres = flags11[4];
                downedThanatos = flags11[5];
                downedArtemisAndApollo = flags11[6];
				TalkedToDraedon = flags11[7];
            }
            else
            {
                ModContent.GetInstance<CalamityMod>().Logger.Error("Unknown loadVersion: " + loadVersion);
            }
        }
        #endregion

        #region NetSend
        public override void NetSend(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = downedDesertScourge;
            flags[1] = downedHiveMind;
            flags[2] = downedPerforator;
            flags[3] = downedSlimeGod;
            flags[4] = downedCryogen;
            flags[5] = downedBrimstoneElemental;
            flags[6] = downedCalamitas;
            flags[7] = downedLeviathan;

            BitsByte flags2 = new BitsByte();
            flags2[0] = downedDoG;
            flags2[1] = downedPlaguebringer;
            flags2[2] = downedGuardians;
            flags2[3] = downedProvidence;
            flags2[4] = downedSentinel1;
            flags2[5] = downedSentinel2;
            flags2[6] = downedSentinel3;
            flags2[7] = downedYharon;

            // Don't write meaningful values for the now-unused vanilla boss booleans
            BitsByte flags3 = new BitsByte();
            flags3[0] = downedSCal;
            flags3[1] = downedBumble;
            flags3[2] = downedCrabulon;
            flags3[3] = downedBetsy;
            flags3[4] = downedScavenger;
            flags3[5] = false;
            flags3[6] = false;
            flags3[7] = false;

            BitsByte flags4 = new BitsByte();
            flags4[0] = false;
            flags4[1] = false;
            flags4[2] = false;
            flags4[3] = false;
            flags4[4] = false;
            flags4[5] = false;
            flags4[6] = onionMode;
            flags4[7] = revenge;

            BitsByte flags5 = new BitsByte();
            flags5[0] = downedStarGod;
            flags5[1] = spawnedBandit;
            flags5[2] = spawnedCirrus;
            flags5[3] = startAcidicDownpour;
            flags5[4] = false;
            flags5[5] = downedPolterghast;
            flags5[6] = death;
            flags5[7] = downedGSS;

            BitsByte flags6 = new BitsByte();
            flags6[0] = abyssSide;
            flags6[1] = downedAquaticScourge;
            flags6[2] = downedAstrageldon;
            flags6[3] = false;
            flags6[4] = armageddon;
            flags6[5] = false;
            flags6[6] = false;
            flags6[7] = false;

            BitsByte flags7 = new BitsByte();
            flags7[0] = BossRushEvent.BossRushActive;
            flags7[1] = downedBoomerDuke;
            flags7[2] = downedCLAM;
            flags7[3] = false;
            flags7[4] = rainingAcid;
            flags7[5] = downedEoCAcidRain;
            flags7[6] = downedAquaticScourgeAcidRain;
            flags7[7] = triedToSummonOldDuke;

            BitsByte flags8 = new BitsByte();
            flags8[0] = forcedRainAlready;
            flags8[1] = forcedDownpourWithTear;
            flags8[2] = downedSecondSentinels;
            flags8[3] = foundHomePermafrost;
            flags8[4] = downedCLAMHardMode;
            flags8[5] = guideName;
            flags8[6] = wizardName;
            flags8[7] = steampunkerName;

            BitsByte flags9 = new BitsByte();
            flags9[0] = stylistName;
            flags9[1] = witchDoctorName;
            flags9[2] = taxCollectorName;
            flags9[3] = pirateName;
            flags9[4] = mechanicName;
            flags9[5] = armsDealerName;
            flags9[6] = dryadName;
            flags9[7] = nurseName;

            BitsByte flags10 = new BitsByte();
            flags10[0] = anglerName;
            flags10[1] = clothierName;
            flags10[2] = encounteredOldDuke;
            flags10[3] = travelingMerchantName;
            flags10[4] = false;
            flags10[5] = false;
            flags10[6] = false;
            flags10[7] = false;

            BitsByte flags11 = new BitsByte();
            flags11[0] = malice;
            flags11[1] = HasGeneratedLuminitePlanetoids;
            flags11[2] = downedAdultEidolonWyrm;
            flags11[3] = downedExoMechs;
            flags11[4] = downedAres;
            flags11[5] = downedThanatos;
            flags11[6] = downedArtemisAndApollo;
            flags11[7] = TalkedToDraedon;

            writer.Write(flags);
            writer.Write(flags2);
            writer.Write(flags3);
            writer.Write(flags4);
            writer.Write(flags5);
            writer.Write(flags6);
            writer.Write(flags7);
            writer.Write(flags8);
            writer.Write(flags9);
            writer.Write(flags10);
            writer.Write(flags11);

            RecipeUnlockHandler.SendData(writer);

            writer.Write(abyssChasmBottom);
            writer.Write(acidRainPoints);
            writer.Write(Reforges);
            writer.Write(MoneyStolenByBandit);
            writer.Write(DraedonSummonCountdown);
            writer.Write((int)DraedonMechToSummon);
            writer.WriteVector2(DraedonSummonPosition);
            writer.WriteVector2(SunkenSeaLabCenter);
            writer.WriteVector2(PlanetoidLabCenter);
            writer.WriteVector2(JungleLabCenter);
            writer.WriteVector2(HellLabCenter);
            writer.WriteVector2(IceLabCenter);
        }
        #endregion

        #region NetReceive
        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedDesertScourge = flags[0];
            downedHiveMind = flags[1];
            downedPerforator = flags[2];
            downedSlimeGod = flags[3];
            downedCryogen = flags[4];
            downedBrimstoneElemental = flags[5];
            downedCalamitas = flags[6];
            downedLeviathan = flags[7];

            BitsByte flags2 = reader.ReadByte();
            downedDoG = flags2[0];
            downedPlaguebringer = flags2[1];
            downedGuardians = flags2[2];
            downedProvidence = flags2[3];
            downedSentinel1 = flags2[4];
            downedSentinel2 = flags2[5];
            downedSentinel3 = flags2[6];
            downedYharon = flags2[7];

            // Explicitly discard the now-unused vanilla boss booleans
            BitsByte flags3 = reader.ReadByte();
            downedSCal = flags3[0];
            downedBumble = flags3[1];
            downedCrabulon = flags3[2];
            downedBetsy = flags3[3];
            downedScavenger = flags3[4];
            _ = flags3[5];
            _ = flags3[6];
            _ = flags3[7];

            BitsByte flags4 = reader.ReadByte();
            _ = flags4[0];
            _ = flags4[1];
            _ = flags4[2];
            _ = flags4[3];
            _ = flags4[4];
            _ = flags4[5];
            onionMode = flags4[6];
            revenge = flags4[7];

            BitsByte flags5 = reader.ReadByte();
            downedStarGod = flags5[0];
            spawnedBandit = flags5[1];
            spawnedCirrus = flags5[2];
            startAcidicDownpour = flags5[3];
            _ = flags5[4];
            downedPolterghast = flags5[5];
            death = flags5[6];
            downedGSS = flags5[7];

            BitsByte flags6 = reader.ReadByte();
            abyssSide = flags6[0];
            downedAquaticScourge = flags6[1];
            downedAstrageldon = flags6[2];
            _ = flags6[3];
            armageddon = flags6[4];
            _ = flags6[5];
            _ = flags6[6];
            _ = flags6[7];

            BitsByte flags7 = reader.ReadByte();
            BossRushEvent.BossRushActive = flags7[0];
            downedBoomerDuke = flags7[1];
            downedCLAM = flags7[2];
            _ = flags7[3];
            rainingAcid = flags7[4];
            downedEoCAcidRain = flags7[5];
            downedAquaticScourgeAcidRain = flags7[6];
            triedToSummonOldDuke = flags7[7];

            BitsByte flags8 = reader.ReadByte();
            forcedRainAlready = flags8[0];
            forcedDownpourWithTear = flags8[1];
            downedSecondSentinels = flags8[2];
            foundHomePermafrost = flags8[3];
            downedCLAMHardMode = flags8[4];
            guideName = flags8[5];
            wizardName = flags8[6];
            steampunkerName = flags8[7];

            BitsByte flags9 = reader.ReadByte();
            stylistName = flags9[0];
            witchDoctorName = flags9[1];
            taxCollectorName = flags9[2];
            pirateName = flags9[3];
            mechanicName = flags9[4];
            armsDealerName = flags9[5];
            dryadName = flags9[6];
            nurseName = flags9[7];

            BitsByte flags10 = reader.ReadByte();
            anglerName = flags10[0];
            clothierName = flags10[1];
            encounteredOldDuke = flags10[2];
            travelingMerchantName = flags10[3];
            _ = flags10[4];
            _ = flags10[5];
            _ = flags10[6];
            _ = flags10[7];

            BitsByte flags11 = reader.ReadByte();
            malice = flags11[0];
            HasGeneratedLuminitePlanetoids = flags11[1];
            downedAdultEidolonWyrm = flags11[2];
            downedExoMechs = flags11[3];
            downedAres = flags11[4];
            downedThanatos = flags11[5];
            downedArtemisAndApollo = flags11[6];
            TalkedToDraedon = flags11[7];

			RecipeUnlockHandler.ReceiveData(reader);

			abyssChasmBottom = reader.ReadInt32();
            acidRainPoints = reader.ReadInt32();
            Reforges = reader.ReadInt32();
            MoneyStolenByBandit = reader.ReadInt32();
            DraedonSummonCountdown = reader.ReadInt32();
            DraedonMechToSummon = (ExoMech)reader.ReadInt32();
            DraedonSummonPosition = reader.ReadVector2();
            SunkenSeaLabCenter = reader.ReadVector2();
            PlanetoidLabCenter = reader.ReadVector2();
            JungleLabCenter = reader.ReadVector2();
            HellLabCenter = reader.ReadVector2();
            IceLabCenter = reader.ReadVector2();
        }
        #endregion

        #region Tiles
        public override void ResetNearbyTileEffects()
        {
            calamityTiles = 0;
            astralTiles = 0;
            sunkenSeaTiles = 0;
            sulphurTiles = 0;
            abyssTiles = 0;
        }

        public override void TileCountsAvailable(int[] tileCounts)
        {
            calamityTiles = tileCounts[ModContent.TileType<CharredOre>()] + tileCounts[ModContent.TileType<BrimstoneSlag>()];
            sunkenSeaTiles = tileCounts[ModContent.TileType<EutrophicSand>()] + tileCounts[ModContent.TileType<Navystone>()] + tileCounts[ModContent.TileType<SeaPrism>()];
            abyssTiles = tileCounts[ModContent.TileType<AbyssGravel>()] + tileCounts[ModContent.TileType<Voidstone>()];
            sulphurTiles = tileCounts[ModContent.TileType<SulphurousSand>()] + tileCounts[ModContent.TileType<SulphurousSandNoWater>()] + tileCounts[ModContent.TileType<SulphurousSandstone>()] + tileCounts[ModContent.TileType<HardenedSulphurousSandstone>()];

            #region Astral Stuff
            int astralDesertTiles = tileCounts[ModContent.TileType<AstralSand>()] + tileCounts[ModContent.TileType<AstralSandstone>()] + tileCounts[ModContent.TileType<HardenedAstralSand>()] + tileCounts[ModContent.TileType<AstralFossil>()];
            int astralSnowTiles = tileCounts[ModContent.TileType<AstralIce>()] + tileCounts[ModContent.TileType<AstralSnow>()];

            Main.sandTiles += astralDesertTiles;
            Main.snowTiles += astralSnowTiles;

            astralTiles = astralDesertTiles + astralSnowTiles + tileCounts[ModContent.TileType<AstralDirt>()] + tileCounts[ModContent.TileType<AstralStone>()] + tileCounts[ModContent.TileType<AstralGrass>()] + tileCounts[ModContent.TileType<AstralOre>()] + tileCounts[ModContent.TileType<AstralSilt>()] + tileCounts[ModContent.TileType<AstralClay>()];
            #endregion
        }
        #endregion

        #region PreWorldGen
        public override void PreWorldGen()
        {
            numAbyssIslands = 0;
            roxShrinePlaced = false;

            // This will only be applied at world-gen time to new worlds.
            // Old worlds will never receive this marker naturally.
            IsWorldAfterDraedonUpdate = true;
        }
        #endregion

        #region ModifyWorldGenTasks
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int islandIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Floating Island Houses"));
            if (islandIndex != -1)
            {
                tasks.Insert(islandIndex + 2, new PassLegacy("EvilIsland", delegate (GenerationProgress progress)
                {
                    progress.Message = "Corrupting a floating island";
                    WorldEvilIsland.PlaceEvilIsland();
                }));
            }

            int DungeonChestIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));
            if (DungeonChestIndex != -1)
            {
                tasks.Insert(DungeonChestIndex + 1, new PassLegacy("Calamity Mod: Biome Chests", MiscWorldgenRoutines.GenerateBiomeChests));
            }

            int WaterFromSandIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Remove Water From Sand"));
            if (WaterFromSandIndex != -1)
            {
                tasks.Insert(WaterFromSandIndex + 1, new PassLegacy("SunkenSea", delegate (GenerationProgress progress)
                {
                    progress.Message = "Making the world more wet";
                    SunkenSea.Place(new Point(WorldGen.UndergroundDesertLocation.Left, WorldGen.UndergroundDesertLocation.Bottom));
                }));
            }

            int JungleTempleIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle Temple"));
            tasks[JungleTempleIndex] = new PassLegacy("Jungle Temple", delegate (GenerationProgress progress)
            {
                progress.Message = "Building a bigger jungle temple";
                CustomTemple.NewJungleTemple();
            });

            int JungleTempleIndex2 = tasks.FindIndex(genpass => genpass.Name.Equals("Temple"));
            tasks[JungleTempleIndex2] = new PassLegacy("Temple", delegate (GenerationProgress progress)
            {
                progress.Message = "Building a bigger jungle temple";
                Main.tileSolid[162] = false;
                Main.tileSolid[226] = true;
                CustomTemple.NewJungleTemplePart2();
                Main.tileSolid[232] = false;
            });

            int LihzahrdAltarIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Lihzahrd Altars"));
            tasks[LihzahrdAltarIndex] = new PassLegacy("Lihzahrd Altars", delegate (GenerationProgress progress)
            {
                progress.Message = "Placing the Lihzahrd altar";
                CustomTemple.NewJungleTempleLihzahrdAltar();
            });

            int FinalIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
            if (FinalIndex != -1)
            {
                //Not touching this yet because the Crags will be reworked in the future
                #region BrimstoneCrag
                tasks.Insert(FinalIndex + 1, new PassLegacy("BrimstoneCrag", delegate (GenerationProgress progress)
                {
                    progress.Message = "Uncovering the ruins of a fallen empire";

                    int x = Main.maxTilesX;

                    int xUnderworldGen = WorldGen.genRand.Next((int)((double)x * 0.1), (int)((double)x * 0.15));
                    int yUnderworldGen = Main.maxTilesY - 100;

                    fuhX = xUnderworldGen;
                    fuhY = yUnderworldGen;

                    MiscWorldgenRoutines.UnderworldIsland(xUnderworldGen, yUnderworldGen, 180, 201, 120, 136);
                    MiscWorldgenRoutines.UnderworldIsland(xUnderworldGen - 50, yUnderworldGen - 30, 100, 111, 60, 71);
                    MiscWorldgenRoutines.UnderworldIsland(xUnderworldGen + 50, yUnderworldGen - 30, 100, 111, 60, 71);

                    MiscWorldgenRoutines.ChasmGenerator(fuhX - 110, fuhY - 10, WorldGen.genRand.Next(150) + 150);
                    MiscWorldgenRoutines.ChasmGenerator(fuhX + 110, fuhY - 10, WorldGen.genRand.Next(150) + 150);

                    MiscWorldgenRoutines.UnderworldIsland(xUnderworldGen - 150, yUnderworldGen - 30, 60, 66, 35, 41);
                    MiscWorldgenRoutines.UnderworldIsland(xUnderworldGen + 150, yUnderworldGen - 30, 60, 66, 35, 41);
                    MiscWorldgenRoutines.UnderworldIsland(xUnderworldGen - 180, yUnderworldGen - 20, 60, 66, 35, 41);
                    MiscWorldgenRoutines.UnderworldIsland(xUnderworldGen + 180, yUnderworldGen - 20, 60, 66, 35, 41);

                    MiscWorldgenRoutines.UnderworldIslandHouse(fuhX, fuhY + 30, 1323);
                    MiscWorldgenRoutines.UnderworldIslandHouse(fuhX - 22, fuhY + 15, 1322);
                    MiscWorldgenRoutines.UnderworldIslandHouse(fuhX + 22, fuhY + 15, 535);
                    MiscWorldgenRoutines.UnderworldIslandHouse(fuhX - 50, fuhY - 30, 112);
                    MiscWorldgenRoutines.UnderworldIslandHouse(fuhX + 50, fuhY - 30, 906);
                    MiscWorldgenRoutines.UnderworldIslandHouse(fuhX - 150, fuhY - 30, 218);
                    MiscWorldgenRoutines.UnderworldIslandHouse(fuhX + 150, fuhY - 30, 3019);
                    MiscWorldgenRoutines.UnderworldIslandHouse(fuhX - 180, fuhY - 20, 274);
                    MiscWorldgenRoutines.UnderworldIslandHouse(fuhX + 180, fuhY - 20, 220);
                }));
                #endregion

                int SulphurIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
                if (SulphurIndex != -1)
                {
                    tasks.Insert(SulphurIndex + 1, new PassLegacy("Sulphur", delegate (GenerationProgress progress)
                    {
                        progress.Message = "Polluting the ocean";
                        SulphurousSea.PlaceSulphurSea();
                    }));
                }

                tasks.Insert(FinalIndex + 2, new PassLegacy("SpecialShrines", delegate (GenerationProgress progress)
                {
                    progress.Message = "Placing Special Shrines";
                    UndergroundShrines.PlaceShrines();
                }));


                tasks.Insert(FinalIndex + 3, new PassLegacy("Rust and Dust", (GenerationProgress progress) =>
                {
                    List<Point> workshopPositions = new List<Point>();
                    int workshopCount = Main.maxTilesX / 900;
                    int labCount = Main.maxTilesX / 1500;

                    DraedonStructures.PlaceHellLab(out Point hellPlacementPosition, workshopPositions);
                    workshopPositions.Add(hellPlacementPosition);

                    DraedonStructures.PlaceSunkenSeaLab(out Point sunkenSeaPlacementPosition, workshopPositions);
                    workshopPositions.Add(sunkenSeaPlacementPosition);

                    DraedonStructures.PlaceIceLab(out Point icePlacementPosition, workshopPositions);
                    workshopPositions.Add(icePlacementPosition);

                    DraedonStructures.PlacePlagueLab(out Point plaguePlacementPosition, workshopPositions);
                    workshopPositions.Add(plaguePlacementPosition);

                    for (int i = 0; i < workshopCount; i++)
                    {
                        DraedonStructures.PlaceWorkshop(out Point placementPosition, workshopPositions);
                        workshopPositions.Add(placementPosition);
                    }
                    for (int i = 0; i < labCount; i++)
                    {
                        DraedonStructures.PlaceResearchFacility(out Point placementPosition, workshopPositions);
                        workshopPositions.Add(placementPosition);
                    }
                }));

                tasks.Insert(FinalIndex + 4, new PassLegacy("Abyss", delegate (GenerationProgress progress)
                {
                    progress.Message = "Discovering the new Challenger Deep"; //Putting the Mariana Trench to shame
                    Abyss.PlaceAbyss();
                }));

                tasks.Insert(FinalIndex + 5, new PassLegacy("Sulphur2", delegate (GenerationProgress progress)
                {
                    progress.Message = "Polluting the ocean more";
                    SulphurousSea.FinishGeneratingSulphurSea();
                }));

                tasks.Insert(FinalIndex + 6, new PassLegacy("IWannaRock", delegate (GenerationProgress progress)
                {
                    progress.Message = "I Wanna Rock";
                    MiscWorldgenRoutines.PlaceRoxShrine();
                }));

                tasks.Insert(FinalIndex + 7, new PassLegacy("GoodGameDesignGemGen", delegate (GenerationProgress progress)
                {
                    progress.Message = "Good Game Design Gem Gen";
                    MiscWorldgenRoutines.SmartGemGen();
                }));
            }

            tasks.Add(new PassLegacy("Planetoid Test", Planetoid.GenerateAllBasePlanetoids));
        }

        // An Astral Meteor always falls at the beginning of Hardmode.
        public override void ModifyHardmodeTasks(List<GenPass> tasks)
        {
            // Yes, this internal identifier is misspelled in vanilla.
            int announceIndex = tasks.FindIndex(match => match.Name == "Hardmode Announcment");

            // Insert the Astral biome generation right before the final hardmode announcement.
            tasks.Insert(announceIndex, new PassLegacy("AstralMeteor", delegate (GenerationProgress progress)
            {
                AstralBiome.PlaceAstralMeteor();
            }));
        }
        #endregion

        #region PostWorldGen
        public override void PostWorldGen()
        {
            // 60% chance of 3-5 Mining Potions
            // 20% chance of 2-3 Builder's Potions
            // 20% chance of 5-9 Shine Potions
            WeightedItemStack[] replacementPotions = new WeightedItemStack[]
            {
                DropHelper.WeightStack(ItemID.MiningPotion, 0.6f, 3, 5),
                DropHelper.WeightStack(ItemID.BuilderPotion, 0.2f, 2, 3),
                DropHelper.WeightStack(ItemID.ShinePotion, 0.2f, 5, 9),
            };

            // Replace Suspicious Looking Eyes in Chests with random useful early game potions.
            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest != null && Main.tile[chest.x, chest.y].type == TileID.Containers)
                {
                    bool isGoldChest = Main.tile[chest.x, chest.y].frameX == 36;
                    bool isMahoganyChest = Main.tile[chest.x, chest.y].frameX == 8 * 36;
                    bool isIvyChest = Main.tile[chest.x, chest.y].frameX == 10 * 36;
                    bool isIceChest = Main.tile[chest.x, chest.y].frameX == 11 * 36;
                    if (isGoldChest || isMahoganyChest || isIvyChest || isIceChest)
                    {
                        for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                        {
                            if (chest.item[inventoryIndex].type == ItemID.SuspiciousLookingEye)
                            {
                                WeightedItemStack replacement = DropHelper.RollWeightedRandom(replacementPotions);
                                chest.item[inventoryIndex].SetDefaults(replacement.itemID);
                                chest.item[inventoryIndex].stack = replacement.ChooseQuantity();
                                break;
                            }
                        }
                    }
                }
            }

            // Save the set of ores that got generated
            OreTypes[0] = WorldGen.CopperTierOre;
            OreTypes[1] = WorldGen.IronTierOre;
            OreTypes[2] = WorldGen.SilverTierOre;
            OreTypes[3] = WorldGen.GoldTierOre;
        }
        #endregion
    }
}
