using CalamityMod.CalPlayer;
using CalamityMod.World;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod
{
    public class ModCalls
    {
        #region Boss / Event Downed
        /// <summary>
        /// Returns whether the Calamity boss or event corresponding to the given string has been defeated.
        /// </summary>
        /// <param name="boss">The boss or event name to check. Many aliases are accepted.</param>
        /// <returns>Whether the boss or event has been defeated.</returns>
        public static bool GetBossDowned(string boss)
        {
            switch (boss.ToLower())
            {
                default:
                    return false;

                case "desertscourge":
                case "desert scourge":
                    return CalamityWorld.downedDesertScourge;

                case "crabulon":
                    return CalamityWorld.downedCrabulon;

                case "hivemind":
                case "hive mind":
                case "thehivemind":
                case "the hive mind":
                    return CalamityWorld.downedHiveMind;

                case "perforator":
                case "perforators":
                case "theperforators":
                case "the perforators":
                case "perforatorhive":
                case "perforator hive":
                case "theperforatorhive":
                case "the perforator hive":
                    return CalamityWorld.downedPerforator;

                case "slimegod":
                case "slime god":
                case "theslimegod":
                case "the slime god":
                    return CalamityWorld.downedSlimeGod;

                case "cryogen":
                    return CalamityWorld.downedCryogen;

                case "brimstoneelemental":
                case "brimstone elemental":
                    return CalamityWorld.downedBrimstoneElemental;

                case "calamitas":
                case "clone":
                case "calamitasclone":
                case "calamitas clone":
                case "clonelamitas":
                case "calamitasdoppelganger":
                case "calamitas doppelganger":
                    return CalamityWorld.downedCalamitas;

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
                case "the siren and the leviathan":
                    return CalamityWorld.downedLeviathan;

                case "aureus":
                case "astrumaureus":
                case "astrum aureus":
                    return CalamityWorld.downedAstrageldon;

                case "pbg":
                case "plaguebringer":
                case "plaguebringergoliath":
                case "plaguebringer goliath":
                case "theplaguebringergoliath":
                case "the plaguebringer goliath":
                    return CalamityWorld.downedPlaguebringer;

                case "ravager":
                    return CalamityWorld.downedScavenger;

                case "astrumdeus":
                case "astrum deus":
                    return CalamityWorld.downedStarGod;

                case "guardians":
                case "donuts":
                case "profanedguardians":
                case "profaned guardians":
                case "theprofanedguardians":
                case "the profaned guardians":
                    return CalamityWorld.downedGuardians;

                case "bumblebirb":
                case "bumblefuck":
                    return CalamityWorld.downedBumble;

                case "providence":
                case "providencetheprofanedgoddess":
                case "providence the profaned goddess":
                case "providence, the profaned goddess":
                    return CalamityWorld.downedProvidence;

                case "polterghast":
                case "necroghast":
                case "necroplasm":
                    return CalamityWorld.downedPolterghast;

                case "sentinel1": // backwards compatibility
                case "void":
                case "ceaselessvoid":
                case "ceaseless void":
                    return CalamityWorld.downedSentinel1;

                case "sentinel2": // backwards compatibility
                case "stormweaver":
                case "storm weaver":
                    return CalamityWorld.downedSentinel2;

                case "sentinel3": // backwards compatibility
                case "cosmicwraith":
                case "cosmic wraith":
                case "signus":
                case "signusenvoyofthedevourer":
                case "signus envoy of the devourer":
                case "signus, envoy of the devourer":
                    return CalamityWorld.downedSentinel3;

                case "sentinelany": // backwards compatibility
                case "anysentinel":
                case "any sentinel":
                case "onesentinel":
                case "one sentinel":
                case "sentinel":
                    return CalamityWorld.downedSentinel1 || CalamityWorld.downedSentinel2 || CalamityWorld.downedSentinel3;

                case "sentinelall": // backwards compatibility
                case "sentinels":
                case "allsentinel":
                case "allsentinels":
                case "all sentinels":
                    return CalamityWorld.downedSentinel1 && CalamityWorld.downedSentinel2 && CalamityWorld.downedSentinel3;

                case "dog":
                case "devourerofgods":
                case "devourer of gods":
                case "thedevourerofgods":
                case "the devourer of gods":
                    return CalamityWorld.downedDoG;

                case "yharon":
                case "jungledragonyharon":
                case "jungle dragon yharon":
                case "jungle dragon, yharon":
                    return CalamityWorld.downedYharon;

                case "scal":
                case "supremecalamitas":
                case "supreme calamitas":
                    return CalamityWorld.downedSCal;
            }
        }
        #endregion

        #region Player in Zone / Area
        /// <summary>
        /// Returns whether the specified player is in the Calamity biome or area corresponding to the given string.
        /// </summary>
        /// <param name="p">The player whose locale is being questioned.</param>
        /// <param name="zone">The zone or area name to check. Many aliases are accepted.</param>
        /// <returns>Whether the player is currently in the zone.</returns>
        public static bool GetInZone(Player p, string zone)
        {
            CalamityPlayer mp = p.Calamity();
            switch (zone.ToLower())
            {
                default:
                    return false;

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
                case "brimstone crags":
                    return mp.ZoneCalamity;

                case "astral":
                case "astralbiome":
                case "astral biome":
                case "astralinfection":
                case "astral infection":
                    return mp.ZoneAstral;

                case "sunkensea":
                case "sunken sea":
                case "thesunkensea":
                case "the sunken sea":
                    return mp.ZoneSunkenSea;

                case "sulfur":
                case "sulphur":
                case "sulfursea":
                case "sulfur sea":
                case "sulphursea":
                case "sulphur sea":
                case "sulfuroussea":
                case "sulfurous sea":
                case "sulphuroussea":
                case "sulphurous sea":
                    return mp.ZoneSulphur;

                case "abyss":
                case "theabyss":
                case "the abyss":
                case "anyabyss":
                case "any abyss":
                case "abyssany":
                case "any abyss layer":
                    return mp.ZoneAbyss;

                case "abyss1":
                case "abyss 1":
                case "abyss_1":
                case "layer1":
                case "layer 1":
                case "layer_1":
                case "abysslayer1":
                case "abyss layer 1":
                    return mp.ZoneAbyssLayer1;

                case "abyss2":
                case "abyss 2":
                case "abyss_2":
                case "layer2":
                case "layer 2":
                case "layer_2":
                case "abysslayer2":
                case "abyss layer 2":
                    return mp.ZoneAbyssLayer2;

                case "abyss3":
                case "abyss 3":
                case "abyss_3":
                case "layer3":
                case "layer 3":
                case "layer_3":
                case "abysslayer3":
                case "abyss layer 3":
                    return mp.ZoneAbyssLayer3;

                case "abyss4":
                case "abyss 4":
                case "abyss_4":
                case "layer4":
                case "layer 4":
                case "layer_4":
                case "abysslayer4":
                case "abyss layer 4":
                    return mp.ZoneAbyssLayer4;
            }
        }
        #endregion

        #region Difficulty Modes
        /// <summary>
        /// Returns whether the Calamity difficulty modifier corresponding to the given string is currently active.
        /// </summary>
        /// <param name="difficulty">The difficulty modifier to check for.</param>
        /// <returns>Whether the difficulty is currently active.</returns>
        public static bool GetDifficultyActive(string difficulty)
        {
            switch (difficulty.ToLower())
            {
                default:
                    return false;

                case "revengeance":
                case "rev":
                case "revengeancemode":
                case "revengeance mode":
                    return CalamityWorld.revenge;

                case "death":
                case "deathmode":
                case "death mode":
                    return CalamityWorld.death;

                case "defiled":
                case "defiledrune":
                case "defiled rune":
                case "defiledmode":
                case "defiled mode":
                    return CalamityWorld.defiled;

                case "armageddon":
                case "arma":
                case "instakill":
                case "instagib":
                case "armageddonmode":
                case "armageddon mode":
                    return CalamityWorld.armageddon;

                case "ironheart":
                case "iron heart":
                case "steelsoul":
                case "steel soul":
                case "permadeath":
                    return CalamityWorld.ironHeart;
            }
        }
        #endregion

        #region Player Armor Set Bonuses
        /// <summary>
        /// Returns whether the specified player has the set bonus corresponding to the given string.
        /// </summary>
        /// <param name="p">The player whose set bonuses are being questioned.</param>
        /// <param name="setBonus">The set bonus to check for.</param>
        /// <returns>Whether the player currently has the set bonus.</returns>
        public static bool GetSetBonus(Player p, string setBonus)
        {
            CalamityPlayer mp = p.Calamity();

            setBonus = setBonus.ToLower();

            // Victide
            if (setBonus == "victide" || setBonus.StartsWith("victide_") || setBonus.StartsWith("victide "))
                return mp.victideSet;

            // Aerospec
            if (setBonus == "aerospec" || setBonus.StartsWith("aerospec_") || setBonus.StartsWith("aerospec "))
                return mp.aeroSet;

            // Statigel
            if (setBonus == "statigel" || setBonus.StartsWith("statigel_") || setBonus.StartsWith("statigel "))
                return mp.statigelSet;

            // Mollusk
            if (setBonus == "mollusk")
                return mp.molluskSet;

            // Daedalus
            switch (setBonus)
            {
                default:
                    break;
                case "daedalus":
                    return mp.daedalusReflect || mp.daedalusShard || mp.daedalusAbsorb || mp.daedalusCrystal || mp.daedalusSplit;
                case "daedalus_melee":
                case "daedalus melee":
                    return mp.daedalusReflect;
                case "daedalus_ranged":
                case "daedalus ranged":
                    return mp.daedalusShard;
                case "daedalus_magic":
                case "daedalus magic":
                    return mp.daedalusAbsorb;
                case "daedalus_summon":
                case "daedalus summon":
                    return mp.daedalusCrystal;
                case "daedalus_rogue":
                case "daedalus rogue":
                    return mp.daedalusSplit;
            }

            // Reaver
            switch (setBonus)
            {
                default:
                    break;
                case "reaver":
                    return mp.reaverBlast || mp.reaverDoubleTap || mp.reaverBurst || mp.reaverOrb || mp.reaverSpore;
                case "reaver_melee":
                case "reaver melee":
                    return mp.reaverBlast;
                case "reaver_ranged":
                case "reaver ranged":
                    return mp.reaverDoubleTap;
                case "reaver_magic":
                case "reaver magic":
                    return mp.reaverBurst;
                case "reaver_summon":
                case "reaver summon":
                    return mp.reaverOrb;
                case "reaver_rogue":
                case "reaver rogue":
                    return mp.reaverSpore;
            }

            // Ataxia
            switch (setBonus)
            {
                default:
                    break;
                case "ataxia":
                    return mp.ataxiaBlaze;
                case "ataxia_melee":
                case "ataxia melee":
                    return mp.ataxiaGeyser;
                case "ataxia_ranged":
                case "ataxia ranged":
                    return mp.ataxiaBolt;
                case "ataxia_magic":
                case "ataxia magic":
                    return mp.ataxiaMage;
                case "ataxia_summon":
                case "ataxia summon":
                    return mp.chaosSpirit;
                case "ataxia_rogue":
                case "ataxia rogue":
                    return mp.ataxiaVolley;
            }

            // Astral
            if (setBonus == "astral")
                return mp.astralStarRain;

            // Xeroc
            if (setBonus == "xeroc")
                return mp.xerocSet;

            // Tarragon
            switch (setBonus)
            {
                default:
                    break;
                case "tarragon":
                    return mp.tarraSet;
                case "tarragon_melee":
                case "tarragon melee":
                    return mp.tarraMelee;
                case "tarragon_ranged":
                case "tarragon ranged":
                    return mp.tarraRanged;
                case "tarragon_magic":
                case "tarragon magic":
                    return mp.tarraMage;
                case "tarragon_summon":
                case "tarragon summon":
                    return mp.tarraSummon;
                case "tarragon_rogue":
                case "tarragon rogue":
                    return mp.tarraThrowing;
            }

            // Bloodflare
            switch (setBonus)
            {
                default:
                    break;
                case "bloodflare":
                    return mp.bloodflareSet;
                case "bloodflare_melee":
                case "bloodflare melee":
                    return mp.bloodflareMelee;
                case "bloodflare_ranged":
                case "bloodflare ranged":
                    return mp.bloodflareRanged;
                case "bloodflare_magic":
                case "bloodflare magic":
                    return mp.bloodflareMage;
                case "bloodflare_summon":
                case "bloodflare summon":
                    return mp.bloodflareSummon;
                case "bloodflare_rogue":
                case "bloodflare rogue":
                    return mp.bloodflareThrowing;
            }

            // Omega Blue
            if (setBonus == "omegablue" || setBonus == "omega blue")
                return mp.omegaBlueSet;

            // God Slayer
            switch (setBonus)
            {
                default:
                    break;
                case "godslayer":
                case "god slayer":
                    return mp.godSlayer;
                case "godslayer_melee":
                case "godslayer melee":
                case "god slayer melee":
                    return mp.godSlayerDamage; // melee helm's unique damage reducing property
                case "godslayer_ranged":
                case "godslayer ranged":
                case "god slayer ranged":
                    return mp.godSlayerRanged;
                case "godslayer_magic":
                case "godslayer magic":
                case "god slayer magic":
                    return mp.godSlayerMage;
                case "godslayer_summon":
                case "godslayer summon":
                case "god slayer summon":
                    return mp.godSlayerSummon;
                case "godslayer_rogue":
                case "godslayer rogue":
                case "god slayer rogue":
                    return mp.godSlayerThrowing;
            }

            // Silva
            switch (setBonus)
            {
                default:
                    break;
                case "silva":
                    return mp.silvaSet;
                case "silva_melee":
                case "silva melee":
                    return mp.silvaMelee;
                case "silva_ranged":
                case "silva ranged":
                    return mp.silvaRanged;
                case "silva_magic":
                case "silva magic":
                    return mp.silvaMage;
                case "silva_summon":
                case "silva summon":
                    return mp.silvaSummon;
                case "silva_rogue":
                case "silva rogue":
                    return mp.silvaThrowing;
            }

            // Auric Tesla
            if (setBonus == "auric" || setBonus == "aurictesla" || setBonus == "auric tesla")
                return mp.auricSet;

            // Demonshade
            if (setBonus == "demonshade")
                return mp.dsSetBonus;

            return false;
        }
        #endregion

        #region Set Damage Reduction

        public static float SetDamageReduction(int npcID, float dr)
        {
            CalamityMod.DRValues.TryGetValue(npcID, out float oldDR);
            CalamityMod.DRValues.Remove(npcID);
            CalamityMod.DRValues.Add(npcID, dr);
            return oldDR;
        }
        #endregion

        #region Call
        public static object Call(params object[] args)
        {
            if (args is null || args.Length <= 0)
                return new ArgumentNullException("ERROR: No function name specified. First argument must be a function name.");
            if (!(args[0] is string))
                return new ArgumentException("ERROR: First argument must be a string function name.");

            string methodName = args[0].ToString();
            Player p = null;
            switch (methodName)
            {
                case "Downed":
                case "GetDowned":
                case "BossDowned":
                case "GetBossDowned":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a boss or event name as a string.");
                    if (!(args[1] is string))
                        return new ArgumentException("ERROR: The argument to \"Downed\" must be a string.");
                    return GetBossDowned(args[1].ToString());

                case "Zone":
                case "GetZone":
                case "InZone":
                case "GetInZone":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and a zone name as a string.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify a zone name as a string.");
                    if (!(args[2] is string))
                        return new ArgumentException("ERROR: The second argument to \"InZone\" must be a string.");
                    if (!(args[1] is int) || !(args[1] is Player))
                        return new ArgumentException("ERROR: The first argument to \"InZone\" must be a Player or an int.");

                    // If the argument is an int, get the corresponding player
                    if (args[1] is int)
                        p = Main.player[(int)args[1]];
                    else if (args[1] is Player)
                        p = (Player)args[1];
                    return GetInZone(p, args[2].ToString());

                case "Difficulty":
                case "GetDifficulty":
                case "DifficultyActive":
                case "GetDifficultyActive":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a difficulty modifier name as a string.");
                    if (!(args[1] is string))
                        return new ArgumentException("ERROR: The argument to \"Difficulty\" must be a string.");
                    return GetDifficultyActive(args[1].ToString());

                case "SetBonus":
                case "SetBonusActive":
                case "HasSetBonus":
                case "GetSetBonus":
                case "GetSetBonusActive":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and a set bonus name as a string.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify a set bonus name as a string.");
                    if (!(args[2] is string))
                        return new ArgumentException("ERROR: The second argument to \"SetBonus\" must be a string.");
                    if (!(args[1] is int) || !(args[1] is Player))
                        return new ArgumentException("ERROR: The first argument to \"SetBonus\" must be a Player or an int.");

                    // If the argument is an int, get the corresponding player
                    if (args[1] is int)
                        p = Main.player[(int)args[1]];
                    else if (args[1] is Player)
                        p = (Player)args[1];
                    return GetSetBonus(p, args[2].ToString());

                case "DR":
                case "DamageReduction":
                case "SetDR":
                case "SetDamageReduction":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify both NPC ID as an int and damage reduction as a float or double.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify damage reduction as a float or double.");
                    if (!(args[2] is float) && !(args[2] is double))
                        return new ArgumentException("ERROR: The second argument to \"SetDamageReduction\" must be a float or a double.");
                    if (!(args[1] is string))
                        return new ArgumentException("ERROR: The first argument to \"SetDamageReduction\" must be an int.");

                    int npcID = (int)args[1];
                    float DR = (float)args[2];
                    return SetDamageReduction(npcID, DR);

                default:
                    return new ArgumentException("ERROR: Invalid method name.");
            }
        }
        #endregion
    }
}
