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
        internal static bool _downedDesertScourge = false;
        internal static bool _downedCrabulon = false;
        internal static bool _downedHiveMind = false;
        internal static bool _downedPerforator = false;
        internal static bool _downedSlimeGod = false;
        internal static bool _downedCryogen = false;
        internal static bool _downedAquaticScourge = false;
        internal static bool _downedBrimstoneElemental = false;
        internal static bool _downedCalamitasClone = false;
        internal static bool _downedLeviathan = false;
        internal static bool _downedAstrumAureus = false;
        internal static bool _downedPlaguebringer = false;
        internal static bool _downedRavager = false;
        internal static bool _downedAstrumDeus = false;
        internal static bool _downedGuardians = false;
        internal static bool _downedDragonfolly = false;
        internal static bool _downedProvidence = false;
        internal static bool _downedCeaselessVoid = false;
        internal static bool _downedStormWeaver = false;
        internal static bool _downedSignus = false;
        internal static bool _downedSecondSentinels = false; // TODO: UNUSED (DoG no longer has sentinels phase)
        internal static bool _downedPolterghast = false;
        internal static bool _downedBoomerDuke = false;
        internal static bool _downedDoG = false;
        internal static bool _downedYharon = false;
        internal static bool _downedAres = false; // only used for loot drops
        internal static bool _downedThanatos = false; // only used for loot drops
        internal static bool _downedArtemisAndApollo = false; // only used for loot drops
        internal static bool _downedExoMechs = false;
        internal static bool _downedCalamitas = false;
        internal static bool _downedPrimordialWyrm = false;

        // Minibosses
        internal static bool _downedGSS = false;
        internal static bool _downedCLAM = false;
        internal static bool _downedCLAMHardMode = false;
        internal static bool _downedCragmawMire = false;
        internal static bool _downedMauler = false;
        internal static bool _downedNuclearTerror = false;

        // Events
        internal static bool _downedEoCAcidRain = false;
        internal static bool _downedAquaticScourgeAcidRain = false;
        internal static bool _downedBossRush = false;

        // Betsy and Dreadnautilus because vanilla doesn't track them
        internal static bool _downedBetsy = false;
        internal static bool _downedDreadnautilus = false;

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
        public static bool downedDreadnautilus
        {
            get => _downedDreadnautilus;
            set
            {
                if (!value)
                    _downedDreadnautilus = false;
                else
                    NPC.SetEventFlagCleared(ref _downedDreadnautilus, -1);
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
        public static bool downedCalamitasClone
        {
            get => _downedCalamitasClone;
            set
            {
                if (!value)
                    _downedCalamitasClone = false;
                else
                    NPC.SetEventFlagCleared(ref _downedCalamitasClone, -1);
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
        public static bool downedPrimordialWyrm
        {
            get => _downedPrimordialWyrm;
            set
            {
                if (!value)
                    _downedPrimordialWyrm = false;
                else
                    NPC.SetEventFlagCleared(ref _downedPrimordialWyrm, -1);
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
        public static bool downedCragmawMire
        {
            get => _downedCragmawMire;
            set
            {
                if (!value)
                    _downedCragmawMire = false;
                else
                    NPC.SetEventFlagCleared(ref _downedCragmawMire, -1);
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
        public static bool downedMauler
        {
            get => _downedMauler;
            set
            {
                if (!value)
                    _downedMauler = false;
                else
                    NPC.SetEventFlagCleared(ref _downedMauler, -1);
            }
        }
        public static bool downedNuclearTerror
        {
            get => _downedNuclearTerror;
            set
            {
                if (!value)
                    _downedNuclearTerror = false;
                else
                    NPC.SetEventFlagCleared(ref _downedNuclearTerror, -1);
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
        public static bool downedBossRush
        {
            get => _downedBossRush;
            set
            {
                if (!value)
                    _downedBossRush = false;
                else
                    NPC.SetEventFlagCleared(ref _downedBossRush, -1);
            }
        }

        internal static void ResetAllFlags()
        {
            downedDesertScourge = false;
            downedCrabulon = false;
            downedHiveMind = false;
            downedPerforator = false;
            downedSlimeGod = false;
            downedDreadnautilus = false;
            downedCryogen = false;
            downedAquaticScourge = false;
            downedBrimstoneElemental = false;
            downedCalamitasClone = false;
            downedLeviathan = false;
            downedAstrumAureus = false;
            downedBetsy = false;
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
            downedCalamitas = false;
            downedPrimordialWyrm = false;

            downedSecondSentinels = false;

            downedCLAM = false;
            downedEoCAcidRain = false;

            downedCLAMHardMode = false;
            downedCragmawMire = false;
            downedAquaticScourgeAcidRain = false;
            downedGSS = false;

            downedMauler = false;
            downedNuclearTerror = false;
            downedBossRush = false;
        }

        public override void OnWorldLoad() => ResetAllFlags();

        public override void OnWorldUnload() => ResetAllFlags();

        public override void SaveWorldData(TagCompound tag)
        {
            List<string> downed = new List<string>();

            // Pre-hardmode bosses (progression sorted)
            if (downedDesertScourge)
                downed.Add("desertScourge");
            if (downedCrabulon)
                downed.Add("crabulon");
            if (downedHiveMind)
                downed.Add("hiveMind");
            if (downedPerforator)
                downed.Add("perforator");
            if (downedSlimeGod)
                downed.Add("slimeGod");

            // Hardmode bosses (progression sorted)
            if (downedDreadnautilus)
                downed.Add("dreadnautilus");
            if (downedCryogen)
                downed.Add("cryogen");
            if (downedAquaticScourge)
                downed.Add("aquaticScourge");
            if (downedBrimstoneElemental)
                downed.Add("brimstoneElemental");
            if (downedCalamitasClone)
                downed.Add("calamitas");
            if (downedLeviathan)
                downed.Add("leviathan");
            if (downedAstrumAureus)
                downed.Add("astrageldon");
            if (downedBetsy)
                downed.Add("betsy");
            if (downedPlaguebringer)
                downed.Add("plaguebringerGoliath");
            if (downedRavager)
                downed.Add("scavenger");
            if (downedAstrumDeus)
                downed.Add("starGod");

            // Post-ML bosses (progression sorted)
            if (downedGuardians)
                downed.Add("guardians");
            if (downedDragonfolly)
                downed.Add("bumblebirb");
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
            if (downedPolterghast)
                downed.Add("polterghast");
            if (downedBoomerDuke)
                downed.Add("oldDuke");
            if (downedDoG)
                downed.Add("devourerOfGods");
            if (downedYharon)
                downed.Add("yharon");
            if (downedThanatos)
                downed.Add("thanatos");
            if (downedArtemisAndApollo)
                downed.Add("artemisAndApollo");
            if (downedAres)
                downed.Add("ares");
            if (downedExoMechs)
                downed.Add("exoMechs");
            if (downedCalamitas)
                downed.Add("supremeCalamitas");
            if (downedPrimordialWyrm)
                downed.Add("adultEidolonWyrm");

            // Minibosses and events
            if (downedCLAM)
                downed.Add("clam");
            if (downedEoCAcidRain)
                downed.Add("eocRain");

            if (downedCLAMHardMode)
                downed.Add("clamHardmode");
            if (downedCragmawMire)
                downed.Add("cragmawMire");
            if (downedAquaticScourgeAcidRain)
                downed.Add("hmRain");
            if (downedGSS)
                downed.Add("greatSandShark");

            if (downedMauler)
                downed.Add("mauler");
            if (downedNuclearTerror)
                downed.Add("nuclearTerror");
            if (downedBossRush)
                downed.Add("bossRush");

            tag["downedFlags"] = downed;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            IList<string> downed = tag.GetList<string>("downedFlags");

            downedDesertScourge = downed.Contains("desertScourge");
            downedAquaticScourge = downed.Contains("aquaticScourge");
            downedCrabulon = downed.Contains("crabulon");
            downedHiveMind = downed.Contains("hiveMind");
            downedPerforator = downed.Contains("perforator");
            downedSlimeGod = downed.Contains("slimeGod");

            downedDreadnautilus = downed.Contains("dreadnautilus");
            downedCryogen = downed.Contains("cryogen");
            downedBrimstoneElemental = downed.Contains("brimstoneElemental");
            downedCalamitasClone = downed.Contains("calamitas");
            downedLeviathan = downed.Contains("leviathan");
            downedAstrumAureus = downed.Contains("astrageldon");
            downedBetsy = downed.Contains("betsy");
            downedPlaguebringer = downed.Contains("plaguebringerGoliath");
            downedRavager = downed.Contains("scavenger");
            downedAstrumDeus = downed.Contains("starGod");

            downedGuardians = downed.Contains("guardians");
            downedDragonfolly = downed.Contains("bumblebirb");
            downedProvidence = downed.Contains("providence");
            downedCeaselessVoid = downed.Contains("ceaselessVoid");
            downedStormWeaver = downed.Contains("stormWeaver");
            downedSignus = downed.Contains("signus");
            downedPolterghast = downed.Contains("polterghast");
            downedBoomerDuke = downed.Contains("oldDuke");
            downedSecondSentinels = downed.Contains("secondSentinels");
            downedDoG = downed.Contains("devourerOfGods");
            downedYharon = downed.Contains("yharon");
            downedThanatos = downed.Contains("thanatos");
            downedArtemisAndApollo = downed.Contains("artemisAndApollo");
            downedAres = downed.Contains("ares");
            downedExoMechs = downed.Contains("exoMechs");
            downedCalamitas = downed.Contains("supremeCalamitas");
            downedPrimordialWyrm = downed.Contains("adultEidolonWyrm");

            downedCLAM = downed.Contains("clam");
            downedEoCAcidRain = downed.Contains("eocRain");

            downedCLAMHardMode = downed.Contains("clamHardmode");
            downedCragmawMire = downed.Contains("cragmawMire");
            downedAquaticScourgeAcidRain = downed.Contains("hmRain");
            downedGSS = downed.Contains("greatSandShark");

            downedMauler = downed.Contains("mauler");
            downedNuclearTerror = downed.Contains("nuclearTerror");
            downedBossRush = downed.Contains("bossRush");
        }
    }
}
