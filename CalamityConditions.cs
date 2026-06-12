using System;
using CalamityMod.World;
using Terraria;
using Terraria.Localization;

namespace CalamityMod
{
    public static class CalamityConditions
    {
        private static Condition Create(string key, Func<bool> predicate)
        {
            return new Condition(
                Language.GetText($"Mods.CalamityMod.Condition.{key}"),
                predicate
            );
        }

        //
        // Player conditions
        //

        public static readonly Condition PlayerHasRogueArmor            = Create("HasRogueArmor",        () => Main.LocalPlayer.Calamity().rogueStealthMax > 0f && Main.LocalPlayer.Calamity().wearingRogueArmor);
        public static readonly Condition PlayerHasWings                 = Create("HasWings",             () => Main.LocalPlayer.wingTimeMax > 0);

        //
        // Calamity Event Flag conditions
        //

        public static readonly Condition DownedAcidRainT1               = Create("Drops.DownedT1AR",     () => DownedBossSystem.downedEoCAcidRain);
        public static readonly Condition DownedAcidRainT2               = Create("Drops.DownedT2AR",     () => DownedBossSystem.downedAquaticScourgeAcidRain);

        //
        // Calamity Boss Flag conditions
        //

        public static readonly Condition DownedDesertScourge         = Create("Drops.DownedDS",          () => DownedBossSystem.downedDesertScourge);
        public static readonly Condition DownedCrabulon              = Create("Drops.DownedCrabulon",    () => DownedBossSystem.downedCrabulon);
        public static readonly Condition DownedHiveMind              = Create("Drops.DownedHM",          () => DownedBossSystem.downedHiveMind);
        public static readonly Condition DownedPerforator            = Create("Drops.DownedPerfs",       () => DownedBossSystem.downedPerforator);
        public static readonly Condition DownedSlimeGod              = Create("Drops.DownedSG",          () => DownedBossSystem.downedSlimeGod);
        public static readonly Condition DownedCryogen               = Create("Drops.DownedCryo",        () => DownedBossSystem.downedCryogen);
        public static readonly Condition DownedAquaticScourge        = Create("Drops.DownedAS",          () => DownedBossSystem.downedAquaticScourge);
        public static readonly Condition DownedBrimstoneElemental    = Create("Drops.DownedBrim",        () => DownedBossSystem.downedBrimstoneElemental);
        public static readonly Condition DownedCalamitasClone        = Create("Drops.DownedCal",         () => DownedBossSystem.downedCalamitasClone);
        public static readonly Condition DownedLeviathan             = Create("Drops.DownedLevi",        () => DownedBossSystem.downedLeviathan);
        public static readonly Condition DownedAstrumAureus          = Create("Drops.DownedAureus",      () => DownedBossSystem.downedAstrumAureus);
        public static readonly Condition DownedPlaguebringer         = Create("Drops.DownedPBG",         () => DownedBossSystem.downedPlaguebringer);
        public static readonly Condition DownedRavager               = Create("Drops.DownedRav",         () => DownedBossSystem.downedRavager);
        public static readonly Condition DownedAstrumDeus            = Create("Drops.DownedAD",          () => DownedBossSystem.downedAstrumDeus);
        public static readonly Condition DownedGuardians             = Create("Drops.DownedGuard",       () => DownedBossSystem.downedGuardians);
        public static readonly Condition DownedBumblebird            = Create("Drops.DownedBirb",        () => DownedBossSystem.downedDragonfolly);
        public static readonly Condition DownedProvidence            = Create("Drops.DownedProv",        () => DownedBossSystem.downedProvidence);
        public static readonly Condition DownedSignus                = Create("Drops.DownedSig",         () => DownedBossSystem.downedSignus);
        public static readonly Condition DownedStormWeaver           = Create("Drops.DownedSW",          () => DownedBossSystem.downedStormWeaver);
        public static readonly Condition DownedCeaselessVoid         = Create("Drops.DownedCV",          () => DownedBossSystem.downedCeaselessVoid);
        public static readonly Condition DownedPolterghast           = Create("Drops.DownedPolter",      () => DownedBossSystem.downedPolterghast);
        public static readonly Condition DownedOldDuke               = Create("Drops.DownedOD",          () => DownedBossSystem.downedBoomerDuke);
        public static readonly Condition DownedDevourerOfGods        = Create("Drops.DownedDoG",         () => DownedBossSystem.downedDoG);
        public static readonly Condition DownedYharon                = Create("Drops.DownedYharon",      () => DownedBossSystem.downedYharon);
        public static readonly Condition DownedExoMechs              = Create("Drops.DownedExos",        () => DownedBossSystem.downedExoMechs);
        public static readonly Condition DownedSupremeCalamitas      = Create("Drops.DownedSCal",        () => DownedBossSystem.downedCalamitas);
        public static readonly Condition DownedPrimordialWyrm        = Create("Drops.DownedAEW",         () => DownedBossSystem.downedPrimordialWyrm);
        public static readonly Condition DownedHorribleHog           = Create("Drops.DownedHorribleHog", () => DownedBossSystem.downedHorribleHog);
        public static readonly Condition DownedClam                  = Create("Drops.DownedClam",        () => DownedBossSystem.downedCLAM);
        public static readonly Condition DownedBuffedClam            = Create("Drops.DownedClamHM",      () => DownedBossSystem.downedCLAMHardMode);
        public static readonly Condition DownedGreatSandShark        = Create("Drops.DownedGSS",         () => DownedBossSystem.downedGSS);

        // Mixed
        public static readonly Condition DownedHiveMindOrPerforator     = Create("Drops.DownedHMOrPerfs",() => DownedBossSystem.downedHiveMind || DownedBossSystem.downedPerforator);
        public static readonly Condition DownedCalamitasCloneOrPlantera = Create("Drops.DownedCalPlant", () => DownedBossSystem.downedCalamitasClone || Condition.DownedPlantera.IsMet());

        // Vanilla
        public static readonly Condition DownedBetsy                    = Create("Drops.DownedBetsy",    () => DownedBossSystem.downedBetsy);

        //
        // Calamity Biome conditions
        //

        public static readonly Condition InAstral                       = Create("InAstral",             () => Main.LocalPlayer.Calamity().ZoneAstral);
        public static readonly Condition InCrag                         = Create("InCrag",               () => Main.LocalPlayer.Calamity().ZoneCalamity);
        public static readonly Condition InSulph                        = Create("InSulph",              () => Main.LocalPlayer.Calamity().ZoneSulphur);
        public static readonly Condition InSunken                       = Create("InSunken",             () => Main.LocalPlayer.Calamity().ZoneSunkenSea);

        //
        // World based conditions
        //

        public static readonly Condition InRevengeanceMode              = Create("InRev",                () => CalamityWorld.revenge);
        public static readonly Condition InRevengeanceModeNotMasterMode = Create("InRev",                () => !Main.masterMode && CalamityWorld.revenge);
        public static readonly Condition InRevengeanceModeOrMasterMode  = Create("InRevOrMM",            () => Main.masterMode || CalamityWorld.revenge);
    }
}
