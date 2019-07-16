using System;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod
{
	public class ModSupport
	{
        // Returns Calamity's boss downed booleans based on a string provided.
        public static readonly Func<string, bool> Downed = (name) =>
        {
            name = name.ToLower();
            switch (name)
            {
                default: return false;

                case "desertscourge":
                case "desert scourge": return CalamityWorld.downedDesertScourge;

                case "crabulon": return CalamityWorld.downedCrabulon;

                case "hivemind":
                case "hive mind":
                case "thehivemind":
                case "the hive mind": return CalamityWorld.downedHiveMind;

                case "perforator":
                case "perforators":
                case "theperforators":
                case "the perforators":
                case "perforatorhive":
                case "perforator hive":
                case "theperforatorhive":
                case "the perforator hive": return CalamityWorld.downedPerforator;

                case "slimegod":
                case "slime god":
                case "theslimegod":
                case "the slime god": return CalamityWorld.downedSlimeGod;

                case "cryogen": return CalamityWorld.downedCryogen;

                case "brimstoneelemental":
                case "brimstone elemental": return CalamityWorld.downedBrimstoneElemental;

                case "calamitas":
                case "clone":
                case "calamitasclone":
                case "calamitas clone":
                case "clonelamitas":
                case "calamitasdoppelganger":
                case "calamitas doppelganger": return CalamityWorld.downedCalamitas;

                case "siren":
                case "thesiren":
                case "the siren":
                case "leviathan":
                case "theleviathan":
                case "the leviathan":
                case "sirenleviathan":
                case "siren leviathan":
                case "sirenandleviathan":
                case "siren and leviathan":
                case "the siren and the leviathan": return CalamityWorld.downedLeviathan;

                case "aureus":
                case "astrumaureus":
                case "astrum aureus": return CalamityWorld.downedAstrageldon;

                case "pbg":
                case "plaguebringer":
                case "plaguebringergoliath":
                case "plaguebringer goliath":
                case "theplaguebringergoliath":
                case "the plaguebringer goliath": return CalamityWorld.downedPlaguebringer;

                case "ravager": return CalamityWorld.downedScavenger;

                case "astrumdeus":
                case "astrum deus": return CalamityWorld.downedStarGod;

                case "guardians":
                case "donuts":
                case "profanedguardians":
                case "profaned guardians":
                case "theprofanedguardians":
                case "the profaned guardians": return CalamityWorld.downedGuardians;

                case "bumblebirb": return CalamityWorld.downedBumble;

                case "providence":
                case "providencetheprofanedgoddess":
                case "providence the profaned goddess":
                case "providence, the profaned goddess": return CalamityWorld.downedProvidence;

                case "polterghast":
                case "necroghast":
                case "necroplasm": return CalamityWorld.downedPolterghast;

                case "sentinel1": // backwards compatibility
                case "void":
                case "ceaselessvoid":
                case "ceaseless void": return CalamityWorld.downedSentinel1;

                case "sentinel2": // backwards compatibility
                case "stormweaver":
                case "storm weaver": return CalamityWorld.downedSentinel2;

                case "sentinel3": // backwards compatibility
                case "cosmicwraith":
                case "cosmic wraith":
                case "signus":
                case "signusenvoyofthedevourer":
                case "signus envoy of the devourer":
                case "signus, envoy of the devourer": return CalamityWorld.downedSentinel3;

                case "sentinelany": // backwards compatibility
                case "anysentinel":
                case "any sentinel":
                case "onesentinel":
                case "one sentinel":
                case "sentinel": return (CalamityWorld.downedSentinel1 || CalamityWorld.downedSentinel2 || CalamityWorld.downedSentinel3);

                case "sentinelall": // backwards compatibility
                case "sentinels":
                case "allsentinel":
                case "allsentinels":
                case "all sentinels": return (CalamityWorld.downedSentinel1 && CalamityWorld.downedSentinel2 && CalamityWorld.downedSentinel3);

                case "dog":
                case "devourerofgods":
                case "devourer of gods":
                case "thedevourerofgods":
                case "the devourer of gods": return CalamityWorld.downedDoG;

                case "yharon":
                case "jungledragonyharon":
                case "jungle dragon yharon":
                case "jungle dragon, yharon": return CalamityWorld.downedYharon;

                case "scal":
                case "supremecalamitas":
                case "supreme calamitas": return CalamityWorld.downedSCal;
            }
        };

        // Returns Calamity's "in biome" booleans based on a string provided.
        public static readonly Func<Player, string, bool> InZone = (p, name) =>
        {
            Mod calamity = ModLoader.GetMod("CalamityMod");
            CalamityPlayer mp = p.GetModPlayer<CalamityPlayer>(calamity);
            name = name.ToLower();
            switch (name)
            {
                default: return false;

                case "calamity": // backwards compatibility
                case "calamitybiome":
                case "calamity biome":
                case "crag":
                case "crags":
                case "profanedcrag": // remove these four when the actual profaned biome is added
                case "profaned crag":
                case "profanedcrags":
                case "profaned crags":
                case "brimstone":
                case "brimstonecrag":
                case "brimstone crag":
                case "brimstonecrags":
                case "brimstone crags": return mp.ZoneCalamity;

                case "astral":
                case "astralbiome":
                case "astral biome":
                case "astralinfection":
                case "astral infection": return mp.ZoneAstral;


                case "sunkensea":
                case "sunken sea":
                case "thesunkensea":
                case "the sunken sea": return mp.ZoneSunkenSea;

                case "sulfur":
                case "sulphur":
                case "sulfursea":
                case "sulfur sea":
                case "sulphursea":
                case "sulphur sea":
                case "sulfuroussea":
                case "sulfurous sea":
                case "sulphuroussea":
                case "sulphurous sea": return mp.ZoneSulphur;

                case "abyss":
                case "theabyss":
                case "the abyss":
                case "anyabyss":
                case "any abyss":
                case "abyssany":
                case "any abyss layer": return mp.ZoneAbyss;

                case "abyss1":
                case "abyss 1":
                case "abyss_1":
                case "layer1":
                case "layer 1":
                case "layer_1":
                case "abysslayer1":
                case "abyss layer 1": return mp.ZoneAbyssLayer1;

                case "abyss2":
                case "abyss 2":
                case "abyss_2":
                case "layer2":
                case "layer 2":
                case "layer_2":
                case "abysslayer2":
                case "abyss layer 2": return mp.ZoneAbyssLayer2;

                case "abyss3":
                case "abyss 3":
                case "abyss_3":
                case "layer3":
                case "layer 3":
                case "layer_3":
                case "abysslayer3":
                case "abyss layer 3": return mp.ZoneAbyssLayer3;

                case "abyss4":
                case "abyss 4":
                case "abyss_4":
                case "layer4":
                case "layer 4":
                case "layer_4":
                case "abysslayer4":
                case "abyss layer 4": return mp.ZoneAbyssLayer4;
            }
        };

        // Returns Calamity's various difficulty modes/modifiers based on a string provided.
        public static readonly Func<string, bool> Difficulty = (name) =>
        {
            name = name.ToLower();
            switch (name)
            {
                default: return false;

                case "revengeance":
                case "rev":
                case "revengeancemode":
                case "revengeance mode": return CalamityWorld.revenge;

                case "death":
                case "deathmode":
                case "death mode": return CalamityWorld.death;

                case "defiled":
                case "defiledrune":
                case "defiled rune":
                case "defiledmode":
                case "defiled mode": return CalamityWorld.defiled;

                case "armageddon":
                case "arma":
                case "instakill":
                case "instagib":
                case "armageddonmode":
                case "armageddon mode": return CalamityWorld.armageddon;

                case "ironheart":
                case "iron heart":
                case "steelsoul":
                case "steel soul":
                case "permadeath": return CalamityWorld.ironHeart;
            }
        };

        public static object Call(params object[] args)
		{
			if (args.Length <= 0 || !(args[0] is string)) return new Exception("ERROR: No function name specified. First argument must be a function name.");

            string methodName = (string)args[0];
            switch (methodName)
            {
                case "Downed":
                    return Downed;
                case "InZone":
                    return InZone;
                case "Difficulty":
                    return Difficulty;
            }

			return new Exception("ERROR: Invalid function name provided as first argument.");
		}
	}
}
