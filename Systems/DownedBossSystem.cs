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
        // Bosses
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
        private static bool _downedAstrumAureus = false;
        private static bool _downedPlaguebringer = false;
        private static bool _downedRavager = false;
        private static bool _downedAstrumDeus = false;
        private static bool _downedGuardians = false;
        private static bool _downedDragonfolly = false;
        private static bool _downedProvidence = false;
        private static bool _downedCeaselessVoid = false;
        private static bool _downedStormWeaver = false;
        private static bool _downedSignus = false;
        private static bool _downedSecondSentinels = false;
        private static bool _downedPolterghast = false;
        private static bool _downedBoomerDuke = false;
        private static bool _downedDoG = false;
        private static bool _downedYharon = false;
        private static bool _downedAres = false; // only used for loot drops
        private static bool _downedThanatos = false; // only used for loot drops
        private static bool _downedArtemisAndApollo = false; // only used for loot drops
        private static bool _downedExoMechs = false;
        private static bool _downedSCal = false;
        private static bool _downedAdultEidolonWyrm = false;

        // Minibosses
        private static bool _downedGSS = false;
        private static bool _downedCLAM = false;
        private static bool _downedCLAMHardMode = false;

        // Events
        private static bool _downedEoCAcidRain = false;
        private static bool _downedAquaticScourgeAcidRain = false;

        // Betsy, because vanilla doesn't track her
        private static bool _downedBetsy = false;

        #region Wrapper Properties for Lantern Nights
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
        public static bool downedAstrumAureus
        {
            get => _downedAstrumAureus;
            set
            {
                if (!value)
                    _downedAstrumAureus = false;
                else
                    NPC.SetEventFlagCleared(ref _downedAstrumAureus, -1);
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
        public static bool downedRavager
        {
            get => _downedRavager;
            set
            {
                if (!value)
                    _downedRavager = false;
                else
                    NPC.SetEventFlagCleared(ref _downedRavager, -1);
            }
        }
        public static bool downedAstrumDeus
        {
            get => _downedAstrumDeus;
            set
            {
                if (!value)
                    _downedAstrumDeus = false;
                else
                    NPC.SetEventFlagCleared(ref _downedAstrumDeus, -1);
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
        public static bool downedDragonfolly
        {
            get => _downedDragonfolly;
            set
            {
                if (!value)
                    _downedDragonfolly = false;
                else
                    NPC.SetEventFlagCleared(ref _downedDragonfolly, -1);
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
        public static bool downedCeaselessVoid
        {
            get => _downedCeaselessVoid;
            set
            {
                if (!value)
                    _downedCeaselessVoid = false;
                else
                    NPC.SetEventFlagCleared(ref _downedCeaselessVoid, -1);
            }
        }
        public static bool downedStormWeaver
        {
            get => _downedStormWeaver;
            set
            {
                if (!value)
                    _downedStormWeaver = false;
                else
                    NPC.SetEventFlagCleared(ref _downedStormWeaver, -1);
            }
        }
        public static bool downedSignus
        {
            get => _downedSignus;
            set
            {
                if (!value)
                    _downedSignus = false;
                else
                    NPC.SetEventFlagCleared(ref _downedSignus, -1);
            }
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
        #endregion

        // These are purely used for loot drops, nothing else
        public static bool downedSecondSentinels
        {
            get => _downedSecondSentinels;
            set => _downedSecondSentinels = value;
        }

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
            downedCrabulon = false;
            downedHiveMind = false;
            downedPerforator = false;
            downedSlimeGod = false;
            downedCryogen = false;
            downedAquaticScourge = false;
            downedBrimstoneElemental = false;
            downedCalamitas = false;
            downedLeviathan = false;
            downedAstrumAureus = false;
            downedPlaguebringer = false;
            downedRavager = false;
            downedAstrumDeus = false;
            downedGuardians = false;
            downedDragonfolly = false;
            downedProvidence = false;
            downedCeaselessVoid = false;
            downedStormWeaver = false;
            downedSignus = false;
            downedPolterghast = false;
            downedBoomerDuke = false;
            downedDoG = false;
            downedYharon = false;
            downedAres = false;
            downedThanatos = false;
            downedArtemisAndApollo = false;
            downedExoMechs = false;
            downedSCal = false;
            downedAdultEidolonWyrm = false;

            downedSecondSentinels = false;

            downedCLAM = false;
            downedCLAMHardMode = false;
            downedGSS = false;

            downedEoCAcidRain = false;
            downedAquaticScourgeAcidRain = false;

            downedBetsy = false;
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
            if (downedCeaselessVoid)
                downed.Add("ceaselessVoid");
            if (downedStormWeaver)
                downed.Add("stormWeaver");
            if (downedSignus)
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
            if (downedDragonfolly)
                downed.Add("bumblebirb");
            if (downedCrabulon)
                downed.Add("crabulon");
            if (downedBetsy)
                downed.Add("betsy");
            if (downedRavager)
                downed.Add("scavenger");
            if (downedAstrumDeus)
                downed.Add("starGod");
            if (downedAstrumAureus)
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
            downedCeaselessVoid = downed.Contains("ceaselessVoid");
            downedStormWeaver = downed.Contains("stormWeaver");
            downedSignus = downed.Contains("signus");
            downedSecondSentinels = downed.Contains("secondSentinels");
            downedYharon = downed.Contains("yharon");
            downedExoMechs = downed.Contains("exoMechs");
            downedAres = downed.Contains("ares");
            downedThanatos = downed.Contains("thanatos");
            downedArtemisAndApollo = downed.Contains("artemisAndApollo");
            downedSCal = downed.Contains("supremeCalamitas");
            downedAdultEidolonWyrm = downed.Contains("adultEidolonWyrm");
            downedDragonfolly = downed.Contains("bumblebirb");
            downedCrabulon = downed.Contains("crabulon");
            downedBetsy = downed.Contains("betsy");
            downedRavager = downed.Contains("scavenger");
            downedAstrumDeus = downed.Contains("starGod");
            downedAstrumAureus = downed.Contains("astrageldon");
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
