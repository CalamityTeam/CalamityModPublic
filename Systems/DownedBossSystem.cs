using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

// This does not use the Systems directory in its namespace for the sake of allowing ease of access.
// It can be changed at some point in the future if really necessary but there are hundreds of references to downed bools and it just
// sounds like more trouble than it's worth.
namespace CalamityMod
{
    public class DownedBossSystem : ModSystem
    {
        private static bool _downedDesertScourge = false;
        private static bool _downedCrabulon = false;
        private static bool _downedHiveMind = false;
        private static bool _downedPerforator = false;
        private static bool _downedSlimeGod = false;
        private static bool _downedCryogen = false;
        private static bool _downedAquaticScourge = false;
        private static bool _downedBrimstoneElemental = false;
        private static bool _downedCalamitas = false;
        private static bool _downedLeviathan = false;
        private static bool _downedAstrageldon = false;
        private static bool _downedStarGod = false;
        private static bool _downedPlaguebringer = false;
        private static bool _downedScavenger = false;
        private static bool _downedBoomerDuke = false;
        private static bool _downedGuardians = false;
        private static bool _downedProvidence = false;
        private static bool _downedSentinel1 = false; // Ceaseless Void
        private static bool _downedSentinel2 = false; // Storm Weaver
        private static bool _downedSentinel3 = false; // Signus, Envoy of the Devourer
        private static bool _downedSecondSentinels = false;
        private static bool _downedPolterghast = false;
        private static bool _downedDoG = false;
        private static bool _downedBumble = false;
        private static bool _downedYharon = false;
        private static bool _downedExoMechs = false;
        private static bool _downedSCal = false;
        private static bool _downedAdultEidolonWyrm = false;
        private static bool _downedGSS = false;
        private static bool _downedCLAM = false;
        private static bool _downedCLAMHardMode = false;
        private static bool _downedBetsy = false; // Betsy

        // These are purely used for loot drops, nothing else
        private static bool _downedAres = false;
        private static bool _downedThanatos = false;
        private static bool _downedArtemisAndApollo = false;

        private static bool _downedEoCAcidRain = false;
        private static bool _downedAquaticScourgeAcidRain = false;

        public static bool downedDesertScourge
        {
            get => _downedDesertScourge;
            set
            {
                if (!value)
                    _downedDesertScourge = false;
                else
                    NPC.SetEventFlagCleared(ref _downedDesertScourge, -1);
            }
        }
        public static bool downedCrabulon
        {
            get => _downedCrabulon;
            set
            {
                if (!value)
                    _downedCrabulon = false;
                else
                    NPC.SetEventFlagCleared(ref _downedCrabulon, -1);
            }
        }
        public static bool downedHiveMind
        {
            get => _downedHiveMind;
            set
            {
                if (!value)
                    _downedHiveMind = false;
                else
                    NPC.SetEventFlagCleared(ref _downedHiveMind, -1);
            }
        }
        public static bool downedPerforator
        {
            get => _downedPerforator;
            set
            {
                if (!value)
                    _downedPerforator = false;
                else
                    NPC.SetEventFlagCleared(ref _downedPerforator, -1);
            }
        }
        public static bool downedSlimeGod
        {
            get => _downedSlimeGod;
            set
            {
                if (!value)
                    _downedSlimeGod = false;
                else
                    NPC.SetEventFlagCleared(ref _downedSlimeGod, -1);
            }
        }
        public static bool downedCryogen
        {
            get => _downedCryogen;
            set
            {
                if (!value)
                    _downedCryogen = false;
                else
                    NPC.SetEventFlagCleared(ref _downedCryogen, -1);
            }
        }
        public static bool downedAquaticScourge
        {
            get => _downedAquaticScourge;
            set
            {
                if (!value)
                    _downedAquaticScourge = false;
                else
                    NPC.SetEventFlagCleared(ref _downedAquaticScourge, -1);
            }
        }
        public static bool downedBrimstoneElemental
        {
            get => _downedBrimstoneElemental;
            set
            {
                if (!value)
                    _downedBrimstoneElemental = false;
                else
                    NPC.SetEventFlagCleared(ref _downedBrimstoneElemental, -1);
            }
        }
        public static bool downedCalamitas
        {
            get => _downedCalamitas;
            set
            {
                if (!value)
                    _downedCalamitas = false;
                else
                    NPC.SetEventFlagCleared(ref _downedCalamitas, -1);
            }
        }
        public static bool downedLeviathan
        {
            get => _downedLeviathan;
            set
            {
                if (!value)
                    _downedLeviathan = false;
                else
                    NPC.SetEventFlagCleared(ref _downedLeviathan, -1);
            }
        }
        public static bool downedAstrageldon
        {
            get => _downedAstrageldon;
            set
            {
                if (!value)
                    _downedAstrageldon = false;
                else
                    NPC.SetEventFlagCleared(ref _downedAstrageldon, -1);
            }
        }
        public static bool downedStarGod
        {
            get => _downedStarGod;
            set
            {
                if (!value)
                    _downedStarGod = false;
                else
                    NPC.SetEventFlagCleared(ref _downedStarGod, -1);
            }
        }
        public static bool downedPlaguebringer
        {
            get => _downedPlaguebringer;
            set
            {
                if (!value)
                    _downedPlaguebringer = false;
                else
                    NPC.SetEventFlagCleared(ref _downedPlaguebringer, -1);
            }
        }
        public static bool downedScavenger
        {
            get => _downedScavenger;
            set
            {
                if (!value)
                    _downedScavenger = false;
                else
                    NPC.SetEventFlagCleared(ref _downedScavenger, -1);
            }
        }
        public static bool downedBoomerDuke
        {
            get => _downedBoomerDuke;
            set
            {
                if (!value)
                    _downedBoomerDuke = false;
                else
                    NPC.SetEventFlagCleared(ref _downedBoomerDuke, -1);
            }
        }
        public static bool downedGuardians
        {
            get => _downedGuardians;
            set
            {
                if (!value)
                    _downedGuardians = false;
                else
                    NPC.SetEventFlagCleared(ref _downedGuardians, -1);
            }
        }
        public static bool downedProvidence
        {
            get => _downedProvidence;
            set
            {
                if (!value)
                    _downedProvidence = false;
                else
                    NPC.SetEventFlagCleared(ref _downedProvidence, -1);
            }
        }
        // Ceaseless Void
        public static bool downedSentinel1
        {
            get => _downedSentinel1;
            set
            {
                if (!value)
                    _downedSentinel1 = false;
                else
                    NPC.SetEventFlagCleared(ref _downedSentinel1, -1);
            }
        }
        // Storm Weaver
        public static bool downedSentinel2
        {
            get => _downedSentinel2;
            set
            {
                if (!value)
                    _downedSentinel2 = false;
                else
                    NPC.SetEventFlagCleared(ref _downedSentinel2, -1);
            }
        }
        // Signus, Envoy of the Devourer
        public static bool downedSentinel3
        {
            get => _downedSentinel3;
            set
            {
                if (!value)
                    _downedSentinel3 = false;
                else
                    NPC.SetEventFlagCleared(ref _downedSentinel3, -1);
            }
        }
        public static bool downedSecondSentinels
        {
            get => _downedSecondSentinels;
            set => _downedSecondSentinels = value;
        }
        public static bool downedPolterghast
        {
            get => _downedPolterghast;
            set
            {
                if (!value)
                    _downedPolterghast = false;
                else
                    NPC.SetEventFlagCleared(ref _downedPolterghast, -1);
            }
        }
        public static bool downedDoG
        {
            get => _downedDoG;
            set
            {
                if (!value)
                    _downedDoG = false;
                else
                    NPC.SetEventFlagCleared(ref _downedDoG, -1);
            }
        }
        public static bool downedBumble
        {
            get => _downedBumble;
            set
            {
                if (!value)
                    _downedBumble = false;
                else
                    NPC.SetEventFlagCleared(ref _downedBumble, -1);
            }
        }
        public static bool downedYharon
        {
            get => _downedYharon;
            set
            {
                if (!value)
                    _downedYharon = false;
                else
                    NPC.SetEventFlagCleared(ref _downedYharon, -1);
            }
        }
        public static bool downedExoMechs
        {
            get => _downedExoMechs;
            set
            {
                if (!value)
                    _downedExoMechs = false;
                else
                    NPC.SetEventFlagCleared(ref _downedExoMechs, -1);
            }
        }
        public static bool downedSCal
        {
            get => _downedSCal;
            set
            {
                if (!value)
                    _downedSCal = false;
                else
                    NPC.SetEventFlagCleared(ref _downedSCal, -1);
            }
        }
        public static bool downedAdultEidolonWyrm
        {
            get => _downedAdultEidolonWyrm;
            set
            {
                if (!value)
                    _downedAdultEidolonWyrm = false;
                else
                    NPC.SetEventFlagCleared(ref _downedAdultEidolonWyrm, -1);
            }
        }
        public static bool downedGSS
        {
            get => _downedGSS;
            set
            {
                if (!value)
                    _downedGSS = false;
                else
                    NPC.SetEventFlagCleared(ref _downedGSS, -1);
            }
        }
        public static bool downedCLAM
        {
            get => _downedCLAM;
            set
            {
                if (!value)
                    _downedCLAM = false;
                else
                    NPC.SetEventFlagCleared(ref _downedCLAM, -1);
            }
        }
        public static bool downedCLAMHardMode
        {
            get => _downedCLAMHardMode;
            set
            {
                if (!value)
                    _downedCLAMHardMode = false;
                else
                    NPC.SetEventFlagCleared(ref _downedCLAMHardMode, -1);
            }
        }
        public static bool downedBetsy
        {
            get => _downedBetsy;
            set
            {
                if (!value)
                    _downedBetsy = false;
                else
                    NPC.SetEventFlagCleared(ref _downedBetsy, -1);
            }
        }

        // These are purely used for loot drops, nothing else
        public static bool downedAres
        {
            get => _downedAres;
            set => _downedAres = value;
        }
        public static bool downedThanatos
        {
            get => _downedThanatos;
            set => _downedThanatos = value;
        }
        public static bool downedArtemisAndApollo
        {
            get => _downedArtemisAndApollo;
            set => _downedArtemisAndApollo = value;
        }

        public static bool downedEoCAcidRain
        {
            get => _downedEoCAcidRain;
            set
            {
                if (!value)
                    _downedEoCAcidRain = false;
                else
                    NPC.SetEventFlagCleared(ref _downedEoCAcidRain, -1);
            }
        }
        public static bool downedAquaticScourgeAcidRain
        {
            get => _downedAquaticScourgeAcidRain;
            set
            {
                if (!value)
                    _downedAquaticScourgeAcidRain = false;
                else
                    NPC.SetEventFlagCleared(ref _downedAquaticScourgeAcidRain, -1);
            }
        }

        internal static void ResetAllFlags()
        {
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
            downedSCal = false;
            downedAdultEidolonWyrm = false;
            downedCLAM = false;
            downedCLAMHardMode = false;
            downedBumble = false;
            downedCrabulon = false;
            downedBetsy = false;
            downedStarGod = false;
            downedAstrageldon = false;
            downedPolterghast = false;
            downedGSS = false;
            downedBoomerDuke = false;
            downedSecondSentinels = false;
            downedEoCAcidRain = false;
            downedAquaticScourgeAcidRain = false;
        }

        public override void OnWorldLoad() => ResetAllFlags();

        public override void OnWorldUnload() => ResetAllFlags();

        public override void SaveWorldData(TagCompound tag)
        {
            List<string> downed = new List<string>();

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
            if (downedCLAM)
                downed.Add("clam");
            if (downedCLAMHardMode)
                downed.Add("clamHardmode");
            if (downedEoCAcidRain)
                downed.Add("eocRain");
            if (downedAquaticScourgeAcidRain)
                downed.Add("hmRain");

            tag["downedFlags"] = downed;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            IList<string> downed = tag.GetList<string>("downedFlags");
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
            downedSCal = downed.Contains("supremeCalamitas");
            downedAdultEidolonWyrm = downed.Contains("adultEidolonWyrm");
            downedBumble = downed.Contains("bumblebirb");
            downedCrabulon = downed.Contains("crabulon");
            downedBetsy = downed.Contains("betsy");
            downedScavenger = downed.Contains("scavenger");
            downedStarGod = downed.Contains("starGod");
            downedAstrageldon = downed.Contains("astrageldon");
            downedPolterghast = downed.Contains("polterghast");
            downedGSS = downed.Contains("greatSandShark");
            downedBoomerDuke = downed.Contains("oldDuke");
            downedCLAM = downed.Contains("clam");
            downedCLAMHardMode = downed.Contains("clamHardmode");
            downedEoCAcidRain = downed.Contains("eocRain");
            downedAquaticScourgeAcidRain = downed.Contains("hmRain");
        }
    }
}
