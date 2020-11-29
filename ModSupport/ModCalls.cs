using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.Projectiles;
using CalamityMod.World;
using System;
using Terraria;

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

				// Because this value will always be true if Acid Rain at any point in the game is beaten.
				case "acid rain":
				case "acidrain":

				case "acid rain 1":
				case "acidrain 1":
				case "acidrain1":
				case "acid rain eoc":
				case "acidrain eoc":
				case "acidraineoc":
					return CalamityWorld.downedEoCAcidRain;

				case "desertscourge":
				case "desert scourge":
					return CalamityWorld.downedDesertScourge;

				case "clam":
				case "giantclam":
				case "giant clam":
					return CalamityWorld.downedCLAM;

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
					
				case "acid rain 2":
				case "acidrain 2":
				case "acidrain2":
				case "acid rain scourge":
				case "acid rain aquatic scourge":
				case "acid rain aquaticscourge":
				case "acidrain scourge":
				case "acidrain aquatic scourge":
				case "acidrain aquaticscourge":
				case "acidrainscourge":
				case "acidrainaquaticscourge":
					return CalamityWorld.downedAquaticScourgeAcidRain;
					
				case "aquaticscourge":
				case "aquatic scourge":
					return CalamityWorld.downedAquaticScourge;

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

				case "gss":
				case "greatsandshark":
				case "great sand shark":
					return CalamityWorld.downedGSS;

				// Don't remove the old references to "Siren" here to avoid breaking other mods
				case "sirenleviathan":
				case "siren leviathan":
				case "sirenandleviathan":
				case "siren and leviathan":
				case "the siren and the leviathan":
				case "siren":
				case "thesiren":
				case "the siren":
				case "anahita":
				case "leviathan":
				case "theleviathan":
				case "the leviathan":
				case "anahitaleviathan":
				case "anahita leviathan":
				case "anahitaandleviathan":
				case "anahita and leviathan":
				case "anahita and the leviathan":
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

				case "scavenger": // backwards compatibility
				case "ravager":
					return CalamityWorld.downedScavenger;

				case "stargod": // backwards compatibility
				case "star god": // backwards compatibility
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

				case "dragonfolly":
				case "the dragonfolly":
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

				// Old Duke is also Acid Rain tier 3, so he gets those names too
				case "oldduke":
				case "old duke":
				case "theoldduke":
				case "the old duke":
				case "boomerduke":
				case "boomer duke":
				case "sulphurduke":
				case "sulphur duke":
				case "sulfurduke":
				case "sulfur duke":
				case "acid rain 3":
				case "acidrain 3":
				case "acidrain3":
				case "acid rain duke":
				case "acidrain duke":
				case "acidrainduke":
					return CalamityWorld.downedBoomerDuke;

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

				case "darksun":
				case "darksuneclipse":
				case "darksun eclipse":
				case "buffedeclipse":
				case "buffed eclipse":
				case "mothron":
				case "darksunmothron":
				case "darksun mothron":
				case "buffedmothron":
				case "buffed mothron":
					return CalamityWorld.downedBuffedMothron;

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

				case "br":
				case "bossrush":
				case "boss rush":
				case "bossrushactive":
				case "boss rush active":
					return BossRushEvent.BossRushActive;

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

		/// <summary>
		/// Either enables or disables the Calamity difficulty modifier corresponding to the given string.<br></br>
		/// Unlike the in-game mode changing items, this has no ancillary effects such as failing if a boss is alive or instantly killing players.
		/// </summary>
		/// <param name="difficulty">The difficulty modifier to edit.</param>
		/// <param name="enabled">Whether to enable or disable the difficulty.</param>
		/// <returns></returns>
		public static bool SetDifficultyActive(string difficulty, bool enabled)
		{
			switch (difficulty.ToLower())
			{
				default:
					return false;

				case "revengeance":
				case "rev":
				case "revengeancemode":
				case "revengeance mode":
					return CalamityWorld.revenge = enabled;

				case "death":
				case "deathmode":
				case "death mode":
					return CalamityWorld.death = enabled;

				case "br":
				case "bossrush":
				case "boss rush":
				case "bossrushactive":
				case "boss rush active":
					return BossRushEvent.BossRushActive = enabled;

				case "defiled":
				case "defiledrune":
				case "defiled rune":
				case "defiledmode":
				case "defiled mode":
					return CalamityWorld.defiled = enabled;

				case "armageddon":
				case "arma":
				case "instakill":
				case "instagib":
				case "armageddonmode":
				case "armageddon mode":
					return CalamityWorld.armageddon = enabled;

				case "ironheart":
				case "iron heart":
				case "steelsoul":
				case "steel soul":
				case "permadeath":
					return CalamityWorld.ironHeart = enabled;
			}
		}
		#endregion

		#region Rogue Class
		/// <summary>
		/// Gets a player's current rogue damage stat.
		/// </summary>
		/// <param name="p">The player whose rogue damage is being queried.</param>
		/// <returns>Current rogue damage boost. 1f is no bonus, 2f is +100%.</returns>
		public static float GetRogueDamage(Player p) => p?.Calamity()?.throwingDamage ?? 1f;

		/// <summary>
		/// Adds a flat amount of rogue damage stat to a player. This amount can be negative.
		/// </summary>
		/// <param name="p">The player whose rogue damage is being modified.</param>
		/// <param name="add">The amount of rogue damage to add or subtract (if negative).</param>
		/// <returns>The player's new rogue damage stat.</returns>
		public static float AddRogueDamage(Player p, float add) => p is null ? 1f : (p.Calamity().throwingDamage += add);

		/// <summary>
		/// Gets a player's current rogue critical strike chance.
		/// </summary>
		/// <param name="p">The player whose rogue crit is being queried.</param>
		/// <returns>Current rogue critical strike chance. 0 is no additional crit, 8 is +8% crit chance.</returns>
		public static int GetRogueCrit(Player p) => p?.Calamity()?.throwingCrit ?? 0;

		/// <summary>
		/// Adds a flat amount of rogue crit to a player. This amount can be negative.
		/// </summary>
		/// <param name="p">The player whose rogue crit is being modified.</param>
		/// <param name="add">The amount of rogue crit to add or subtract (if negative).</param>
		/// <returns>The player's new rogue crit stat.</returns>
		public static int AddRogueCrit(Player p, int add) => p is null ? 0 : (p.Calamity().throwingCrit += add);

		/// <summary>
		/// Gets a player's current rogue projectile velocity multiplier.
		/// </summary>
		/// <param name="p">The player whose rogue velocity is being queried.</param>
		/// <returns>Current rogue projectile velocity multiplier. 1f is no bonus, 2f doubles projectile speed.</returns>
		public static float GetRogueVelocity(Player p) => p?.Calamity()?.throwingVelocity ?? 1f;

		/// <summary>
		/// Adds a flat amount of rogue velocity stat to a player. This amount can be negative.
		/// </summary>
		/// <param name="p">The player whose rogue velocity is being modified.</param>
		/// <param name="add">The amount of rogue velocity to add or subtract (if negative).</param>
		/// <returns>The player's new rogue velocity stat.</returns>
		public static float AddRogueVelocity(Player p, float add) => p is null ? 1f : (p.Calamity().throwingVelocity += add);

		public static float GetCurrentStealth(Player p) => p?.Calamity()?.rogueStealth ?? 0f;
		
		public static float GetMaxStealth(Player p) => p?.Calamity()?.rogueStealthMax ?? 0f;

		public static float AddMaxStealth(Player p, float add) => p is null ? 0f : (p.Calamity().rogueStealthMax += add);

		/// <summary>
		/// Gets whether the given projectile is classified as rogue.
		/// </summary>
		/// <param name="p">The projectile which is being checked.</param>
		/// <returns>Whether the projectile is rogue.</returns>
		public static bool IsRogue(Projectile p)
		{
			if (p is null || p.Calamity() is null)
				return false;
			CalamityGlobalProjectile cgp = p.Calamity();
			return cgp.rogue | cgp.forceRogue;
		}

		/// <summary>
		/// Sets whether the given projectile is classified as rogue. If set to true, also forces the projectile to be rogue every single frame.
		/// </summary>
		/// <param name="p">The projectile whose rogue classification is being toggled.</param>
		/// <param name="isRogue">The value to apply.</param>
		/// <returns>Whether the projectile is now rogue.</returns>
		public static bool SetRogue(Projectile p, bool isRogue)
		{
			if (p is null || p.Calamity() is null)
				return false;
			CalamityGlobalProjectile cgp = p.Calamity();
			cgp.forceRogue = cgp.rogue = isRogue;
			return cgp.rogue;
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

			// LATER -- no summon set bonuses are written well. all use two bools, neither of which actually controls the function

			// Desert Prowler
			if (setBonus == "desertprowler" || setBonus == "desert prowler")
				return mp.desertProwler;

			// Snow Ruffian
			if (setBonus == "snowruffian" || setBonus == "snow ruffian")
				return mp.snowRuffianSet;

			// Victide
			if (setBonus == "victide_summon" || setBonus == "victide summon")
				return mp.urchin; // the bool set directly by VictideHelmet.UpdateArmorSet
			else if (setBonus == "victide" || setBonus.StartsWith("victide_") || setBonus.StartsWith("victide "))
				return mp.victideSet;

			// Aerospec
			if (setBonus == "aerospec_summon" || setBonus == "aerospec summon")
				return mp.valkyrie; // the bool set directly by AerospecHelmet.UpdateArmorSet
			else if (setBonus == "aerospec" || setBonus.StartsWith("aerospec_") || setBonus.StartsWith("aerospec "))
				return mp.aeroSet;

			// Statigel
			if (setBonus == "statigel_summon" || setBonus == "statigel summon")
				return mp.slimeGod; // the bool set directly by StatigelHood.UpdateArmorSet
			if (setBonus == "statigel" || setBonus.StartsWith("statigel_") || setBonus.StartsWith("statigel "))
				return mp.statigelSet;

			// Mollusk
			if (setBonus == "mollusk")
				return mp.molluskSet;

			// Forbidden Circlet
			if (setBonus == "forbidden_circlet" || setBonus == "forbidden circlet")
				return mp.forbiddenCirclet;

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
					return mp.reaverDefense || mp.reaverSpeed || mp.reaverDamage || mp.reaverExplore || mp.reaverHealth;
				case "reaver_defense":
				case "reaver defense":
				case "reaver_helm":
				case "reaver helm":
					return mp.reaverDefense;
				case "reaver_speed":
				case "reaver speed":
				case "reaver_visage":
				case "reaver visage":
					return mp.reaverSpeed;
				case "reaver_damage":
				case "reaver damage":;
				case "reaver_mask":
				case "reaver mask":
					return mp.reaverDamage;
				case "reaver_explore":
				case "reaver explore":
				case "reaver_helmet":
				case "reaver helmet":
					return mp.reaverExplore;
				case "reaver_health":
				case "reaver health":
				case "reaver_regen":
				case "reaver regen":
				case "reaver_headgear":
				case "reaver headgear":
				case "reaver_cap":
				case "reaver cap":
					return mp.reaverHealth;
			}

			// Fathom Swarmer
			if (setBonus == "fathomswarmer" || setBonus == "fathom swarmer")
				return mp.fathomSwarmer;

			// Brimflame
			if (setBonus == "brimflame")
				return mp.brimflameSet;

			// Umbraphile
			if (setBonus == "umbraphile")
				return mp.umbraphileSet;

			// Hydrothermic (Ataxia is legacy name)
			switch (setBonus)
			{
				default:
					break;
				case "ataxia":
				case "hydrothermic":
				case "hydrothermal":
					return mp.ataxiaBlaze;
				case "ataxia_melee":
				case "ataxia melee":
				case "hydrothermic_melee":
				case "hydrothermic melee":
				case "hydrothermal_melee":
				case "hydrothermal melee":
					return mp.ataxiaGeyser;
				case "ataxia_ranged":
				case "ataxia ranged":
				case "hydrothermic_ranged":
				case "hydrothermic ranged":
				case "hydrothermal_ranged":
				case "hydrothermal ranged":
					return mp.ataxiaBolt;
				case "ataxia_magic":
				case "ataxia magic":
				case "hydrothermic_magic":
				case "hydrothermic magic":
				case "hydrothermal_magic":
				case "hydrothermal magic":
					return mp.ataxiaMage;
				case "ataxia_summon":
				case "ataxia summon":
				case "hydrothermic_summon":
				case "hydrothermic summon":
				case "hydrothermal_summon":
				case "hydrothermal summon":
					return mp.chaosSpirit;
				case "ataxia_rogue":
				case "ataxia rogue":
				case "hydrothermic_rogue":
				case "hydrothermic rogue":
				case "hydrothermal_rogue":
				case "hydrothermal rogue":
					return mp.ataxiaVolley;
			}

			// Plague Reaper
			if (setBonus == "plaguereaper" || setBonus == "plague reaper")
				return mp.plagueReaper;

			// Plaguebringer
			if (setBonus == "plaguebringer" || setBonus == "plaguebringerpatron" || setBonus == "plaguebringer patron")
				return mp.plaguebringerPatronSet;

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

			// Prismatic
			if (setBonus == "prismatic" || setBonus == "prism")
				return mp.prismaticSet;

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

			// Fearmonger
			if (setBonus == "fearmonger")
				return mp.fearmongerSet;

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

		/// <summary>
		/// Turns the set bonus corresponding to the given string on or off for the specified player.
		/// </summary>
		/// <param name="p">The player whose set bonuses are being toggled.</param>
		/// <param name="setBonus">The set bonus to check for.</param>
		/// <param name="enabled">Whether the set bonus should be enabled (true) or disabled (false).</param>
		/// <returns>Whether any set bonus was adjusted.</returns>
		public static bool SetSetBonus(Player p, string setBonus, bool enabled)
		{
			CalamityPlayer mp = p.Calamity();
			setBonus = setBonus.ToLower();

			// Desert Prowler
			if (setBonus == "desertprowler" || setBonus == "desert prowler")
			{
				mp.desertProwler = enabled;
				return true;
			}

			// Snow Ruffian
			if (setBonus == "snowruffian" || setBonus == "snow ruffian")
			{
				mp.snowRuffianSet = enabled;
				return true;
			}

			// Victide
			if (setBonus == "victide_summon" || setBonus == "victide summon")
			{
				mp.victideSet = enabled;
				mp.urchin = enabled; // LATER -- remove this when player.urchin actually controls victide summoner
				return true;
			}
			else if (setBonus == "victide" || setBonus.StartsWith("victide_") || setBonus.StartsWith("victide "))
			{
				mp.victideSet = true;
				return true;
			}

			// Aerospec
			if (setBonus == "aerospec_summon" || setBonus == "aerospec summon")
			{
				mp.aeroSet = enabled;
				mp.valkyrie = enabled; // LATER -- remove this when player.valkyrie actually controls aerospec summoner
				return true;
			}
			else if (setBonus == "aerospec" || setBonus.StartsWith("aerospec_") || setBonus.StartsWith("aerospec "))
			{
				mp.aeroSet = enabled;
				return true;
			}

			// Statigel
			if (setBonus == "statigel_summon" || setBonus == "statigel summon")
			{
				mp.statigelSet = enabled;
				mp.slimeGod = enabled; // LATER -- remove this when player.slimeGod actually controls statigel summoner
				return true;
			}
			else if (setBonus == "statigel" || setBonus.StartsWith("statigel_") || setBonus.StartsWith("statigel "))
			{
				mp.statigelSet = enabled;
				return true;
			}

			// Mollusk
			if (setBonus == "mollusk")
			{
				mp.molluskSet = enabled;
				return true;
			}

			// Forbidden Circlet
			if (setBonus == "forbidden_circlet" || setBonus == "forbidden circlet")
			{
				mp.forbiddenCirclet = enabled;
				return true;
			}

			// Daedalus
			switch (setBonus)
			{
				default:
					break;
				case "daedalus_melee":
				case "daedalus melee":
					mp.daedalusReflect = enabled;
					return true;
				case "daedalus_ranged":
				case "daedalus ranged":
					mp.daedalusShard = enabled;
					return true;
				case "daedalus_magic":
				case "daedalus magic":
					mp.daedalusAbsorb = enabled;
					return true;
				case "daedalus_summon":
				case "daedalus summon":
					mp.daedalusCrystal = enabled; // LATER -- remove this when player.daedalusCrystal actually controls daedalus summoner
					return true;
				case "daedalus_rogue":
				case "daedalus rogue":
					mp.daedalusSplit = enabled;
					return true;
			}

			// Reaver
			switch (setBonus)
			{
				default:
					break;
				case "reaver_defense":
				case "reaver defense":
				case "reaver_helm":
				case "reaver helm":
					mp.reaverDefense = enabled;
					return true;
				case "reaver_speed":
				case "reaver speed":
				case "reaver_visage":
				case "reaver visage":
					mp.reaverSpeed = enabled;
					return true;
				case "reaver_damage":
				case "reaver damage":;
				case "reaver_mask":
				case "reaver mask":
					mp.reaverDamage = enabled;
					return true;
				case "reaver_explore":
				case "reaver explore":
				case "reaver_helmet":
				case "reaver helmet":
					mp.reaverExplore = enabled;
					return true;
				case "reaver_health":
				case "reaver health":
				case "reaver_regen":
				case "reaver regen":
				case "reaver_headgear":
				case "reaver headgear":
				case "reaver_cap":
				case "reaver cap":
					mp.reaverHealth = enabled;
					return true;
			}

			// Fathom Swarmer
			if (setBonus == "fathomswarmer" || setBonus == "fathom swarmer")
			{
				mp.fathomSwarmer = enabled;
				return true;
			}

			// Brimflame
			if (setBonus == "brimflame")
			{
				mp.brimflameSet = enabled;
				return true;
			}

			// Umbraphile
			if (setBonus == "umbraphile")
			{
				mp.umbraphileSet = enabled;
				return true;
			}

			// Hydrothermic (Ataxia as legacy name)
			switch (setBonus)
			{
				default:
					break;
				case "ataxia":
				case "hydrothermic":
				case "hydrothermal":
					mp.ataxiaBlaze = enabled;
					return true;
				case "ataxia_melee":
				case "ataxia melee":
				case "hydrothermic_melee":
				case "hydrothermic melee":
				case "hydrothermal_melee":
				case "hydrothermal melee":
					mp.ataxiaBlaze = enabled;
					mp.ataxiaGeyser = enabled;
					return true;
				case "ataxia_ranged":
				case "ataxia ranged":
				case "hydrothermic_ranged":
				case "hydrothermic ranged":
				case "hydrothermal_ranged":
				case "hydrothermal ranged":
					mp.ataxiaBlaze = enabled;
					mp.ataxiaBolt = enabled;
					return true;
				case "ataxia_magic":
				case "ataxia magic":
				case "hydrothermic_magic":
				case "hydrothermic magic":
				case "hydrothermal_magic":
				case "hydrothermal magic":
					mp.ataxiaBlaze = enabled;
					mp.ataxiaMage = enabled;
					return true;
				case "ataxia_summon":
				case "ataxia summon":
				case "hydrothermic_summon":
				case "hydrothermic summon":
				case "hydrothermal_summon":
				case "hydrothermal summon":
					mp.ataxiaBlaze = enabled;
					mp.chaosSpirit = enabled; // LATER -- remove this when player.chaosSpirit actually controls ataxia summoner
					return true;
				case "ataxia_rogue":
				case "ataxia rogue":
				case "hydrothermic_rogue":
				case "hydrothermic rogue":
				case "hydrothermal_rogue":
				case "hydrothermal rogue":
					mp.ataxiaBlaze = enabled;
					mp.ataxiaVolley = enabled;
					return true;
			}

			// Plague Reaper
			if (setBonus == "plaguereaper" || setBonus == "plague reaper")
			{
				mp.plagueReaper = enabled;
				return true;
			}

			// Plaguebringer
			if (setBonus == "plaguebringer" || setBonus == "plaguebringerpatron" || setBonus == "plaguebringer patron")
			{
				mp.plaguebringerPatronSet = enabled;
				return true;
			}

			// Astral
			if (setBonus == "astral")
			{
				mp.astralStarRain = enabled;
				return true;
			}

			// Xeroc
			if (setBonus == "xeroc")
			{
				mp.xerocSet = enabled;
				return true;
			}

			// Tarragon
			switch (setBonus)
			{
				default:
					break;
				case "tarragon":
					mp.tarraSet = enabled;
					return true;
				case "tarragon_melee":
				case "tarragon melee":
					mp.tarraSet = enabled;
					mp.tarraMelee = enabled;
					return true;
				case "tarragon_ranged":
				case "tarragon ranged":
					mp.tarraSet = enabled;
					mp.tarraRanged = enabled;
					return true;
				case "tarragon_magic":
				case "tarragon magic":
					mp.tarraSet = enabled;
					mp.tarraMage = enabled;
					return true;
				case "tarragon_summon":
				case "tarragon summon":
					mp.tarraSet = enabled;
					mp.tarraSummon = enabled; // LATER -- remove this when player.tarraSummon actually controls life aura
					return true;
				case "tarragon_rogue":
				case "tarragon rogue":
					mp.tarraSet = enabled;
					mp.tarraThrowing = enabled;
					return true;
			}

			// Prismatic
			if (setBonus == "prismatic" || setBonus == "prism")
			{
				mp.prismaticSet = enabled;
				return true;
			}

			// Bloodflare
			switch (setBonus)
			{
				default:
					break;
				case "bloodflare":
					mp.bloodflareSet = enabled;
					return true;
				case "bloodflare_melee":
				case "bloodflare melee":
					mp.bloodflareSet = enabled;
					mp.bloodflareMelee = enabled;
					return true;
				case "bloodflare_ranged":
				case "bloodflare ranged":
					mp.bloodflareSet = enabled;
					mp.bloodflareRanged = enabled;
					return true;
				case "bloodflare_magic":
				case "bloodflare magic":
					mp.bloodflareSet = enabled;
					mp.bloodflareMage = enabled;
					return true;
				case "bloodflare_summon":
				case "bloodflare summon":
					mp.bloodflareSet = enabled;
					mp.bloodflareSummon = enabled; // LATER -- remove this when player.bloodflareSummon actually controls bloodflare orbs
					return true;
				case "bloodflare_rogue":
				case "bloodflare rogue":
					mp.bloodflareSet = enabled;
					mp.bloodflareThrowing = enabled;
					return true;
			}

			// Omega Blue
			if (setBonus == "omegablue" || setBonus == "omega blue")
			{
				mp.omegaBlueSet = enabled;
				return true;
			}

			// God Slayer
			switch (setBonus)
			{
				default:
					break;
				case "godslayer":
				case "god slayer":
					mp.godSlayer = enabled;
					return true;
				case "godslayer_melee":
				case "godslayer melee":
				case "god slayer melee":
					mp.godSlayer = enabled;
					mp.godSlayerDamage = enabled; // melee helm's unique damage reducing property
					return true;
				case "godslayer_ranged":
				case "godslayer ranged":
				case "god slayer ranged":
					mp.godSlayer = enabled;
					mp.godSlayerRanged = enabled;
					return true;
				case "godslayer_magic":
				case "godslayer magic":
				case "god slayer magic":
					mp.godSlayer = enabled;
					mp.godSlayerMage = enabled;
					return true;
				case "godslayer_summon":
				case "godslayer summon":
				case "god slayer summon":
					mp.godSlayer = enabled;
					mp.godSlayerSummon = enabled; // LATER -- remove this when player.godSlayerSummon actually controls mechworm
					return true;
				case "godslayer_rogue":
				case "godslayer rogue":
				case "god slayer rogue":
					mp.godSlayer = enabled;
					mp.godSlayerThrowing = enabled;
					return true;
			}

			// Fearmonger
			if (setBonus == "fearmonger")
			{
				mp.fearmongerSet = enabled;
				return true;
			}

			// Silva
			switch (setBonus)
			{
				default:
					break;
				case "silva":
					mp.silvaSet = enabled;
					return true;
				case "silva_melee":
				case "silva melee":
					mp.silvaSet = enabled;
					mp.silvaMelee = enabled;
					return true;
				case "silva_ranged":
				case "silva ranged":
					mp.silvaSet = enabled;
					mp.silvaRanged = enabled;
					return true;
				case "silva_magic":
				case "silva magic":
					mp.silvaSet = enabled;
					mp.silvaMage = enabled;
					return true;
				case "silva_summon":
				case "silva summon":
					mp.silvaSet = enabled;
					mp.silvaSummon = enabled; // LATER -- remove this when player.silvaSummon actually controls silva crystal
					return true;
				case "silva_rogue":
				case "silva rogue":
					mp.silvaSet = enabled;
					mp.silvaThrowing = enabled;
					return true;
			}

			// Auric Tesla (includes all components)
			switch (setBonus)
			{
				default:
					break;
				case "auric":
				case "aurictesla":
				case "auric tesla":
					mp.tarraSet = enabled;
					mp.bloodflareSet = enabled;
					mp.godSlayer = enabled;
					mp.silvaSet = enabled;
					mp.auricSet = enabled;
					return true;
				case "auric_melee":
				case "auric melee":
				case "aurictesla_melee":
				case "aurictesla melee":
				case "auric tesla melee":
					mp.tarraSet = enabled;
					mp.tarraMelee = enabled;
					mp.bloodflareSet = enabled;
					mp.bloodflareMelee = enabled;
					mp.godSlayer = enabled;
					mp.godSlayerDamage = enabled;
					mp.silvaSet = enabled;
					mp.silvaMelee = enabled;
					mp.auricSet = enabled;
					return true;
				case "auric_ranged":
				case "auric ranged":
				case "aurictesla_ranged":
				case "aurictesla ranged":
				case "auric tesla ranged":
					mp.tarraSet = enabled;
					mp.tarraRanged = enabled;
					mp.bloodflareSet = enabled;
					mp.bloodflareRanged = enabled;
					mp.godSlayer = enabled;
					mp.godSlayerRanged = enabled;
					mp.silvaSet = enabled;
					mp.silvaRanged = enabled;
					mp.auricSet = enabled;
					return true;
				case "auric_magic":
				case "auric magic":
				case "aurictesla_magic":
				case "aurictesla magic":
				case "auric tesla magic":
					mp.tarraSet = enabled;
					mp.tarraMage = enabled;
					mp.bloodflareSet = enabled;
					mp.bloodflareMage = enabled;
					mp.godSlayer = enabled;
					mp.godSlayerMage = enabled;
					mp.silvaSet = enabled;
					mp.silvaMage = enabled;
					mp.auricSet = enabled;
					return true;
				case "auric_summon":
				case "auric summon":
				case "aurictesla_summon":
				case "aurictesla summon":
				case "auric tesla summon":
					mp.tarraSet = enabled;
					mp.tarraSummon = enabled;
					mp.bloodflareSet = enabled;
					mp.bloodflareSummon = enabled;
					mp.godSlayer = enabled;
					mp.godSlayerSummon = enabled;
					mp.silvaSet = enabled;
					mp.silvaSummon = enabled;
					mp.auricSet = enabled;
					return true;
				case "auric_rogue":
				case "auric rogue":
				case "aurictesla_rogue":
				case "aurictesla rogue":
				case "auric tesla rogue":
					mp.tarraSet = enabled;
					mp.tarraThrowing = enabled;
					mp.bloodflareSet = enabled;
					mp.bloodflareThrowing = enabled;
					mp.godSlayer = enabled;
					mp.godSlayerThrowing = enabled;
					mp.silvaSet = enabled;
					mp.silvaThrowing = enabled;
					mp.auricSet = enabled;
					return true;
			}

			// Demonshade
			if (setBonus == "demonshade")
			{
				mp.dsSetBonus = enabled;
				mp.rDevil = enabled; // LATER -- remove this when player.rDevil controls demonshade summoned minion
				return true;
			}

			return false;
		}
		#endregion

		#region Other Player Stats
		public static int GetLightStrength(Player p) => p?.Calamity()?.GetTotalLightStrength() ?? 0;

		public static void AddAbyssLightStrength(Player p, int add)
		{
			if (p != null)
				p.Calamity().externalAbyssLight += add;
		}

		public static bool MakeColdImmune(Player p) => p is null ? false : (p.Calamity().externalColdImmunity = true);
		public static bool MakeHeatImmune(Player p) => p is null ? false : (p.Calamity().externalHeatImmunity = true);
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

		#region Boss Health Bars
		public static bool BossHealthBarVisible() => Main.LocalPlayer.Calamity().drawBossHPBar;

		public static bool SetBossHealthBarVisible(bool visible) => Main.LocalPlayer.Calamity().drawBossHPBar = visible;
		#endregion

		#region Item Rarity
		public static int GetCalamityRarity(Item item)
		{
			CalamityGlobalItem cgi = item.Calamity();
			CalamityRarity calrare = cgi.customRarity;
			if (calrare == CalamityRarity.NoEffect)
				return item.rare;
			return (int)calrare;
		}

		public static bool SetCalamityRarity(Item item, int rarityNum)
		{
			CalamityGlobalItem cgi = item.Calamity();
			cgi.customRarity = (CalamityRarity)rarityNum;
			return cgi.customRarity != CalamityRarity.NoEffect;
		}
		#endregion

		#region Call
		public static object Call(params object[] args)
		{
			bool isValidPlayerArg(object o) => o is int || o is Player;
			bool isValidProjectileArg(object o) => o is int || o is Projectile;

			Player castPlayer(object o)
			{
				if (o is int i)
					return Main.player[i];
				else if (o is Player p)
					return p;
				return null;
			}

			Projectile castProjectile(object o)
			{
				if (o is int i)
					return Main.projectile[i];
				else if (o is Projectile p)
					return p;
				return null;
			}

			if (args is null || args.Length <= 0)
				return new ArgumentNullException("ERROR: No function name specified. First argument must be a function name.");
			if (!(args[0] is string))
				return new ArgumentException("ERROR: First argument must be a string function name.");

			string methodName = args[0].ToString();
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
					if (!isValidPlayerArg(args[1]))
						return new ArgumentException("ERROR: The first argument to \"InZone\" must be a Player or an int.");
					return GetInZone(castPlayer(args[1]), args[2].ToString());

				case "Difficulty":
				case "GetDifficulty":
				case "DifficultyActive":
				case "GetDifficultyActive":
					if (args.Length < 2)
						return new ArgumentNullException("ERROR: Must specify a difficulty mode name as a string.");
					if (!(args[1] is string))
						return new ArgumentException("ERROR: The argument to \"Difficulty\" must be a string.");
					return GetDifficultyActive(args[1].ToString());

				case "SetDifficulty":
				case "SetDifficultyActive":
					if (args.Length < 2)
						return new ArgumentNullException("ERROR: Must specify a difficulty mode name as a string and a bool.");
					if (args.Length < 3)
						return new ArgumentNullException("ERROR: Must specify a bool.");
					if (!(args[2] is bool enabled))
						return new ArgumentException("ERROR: The second argument to \"SetDifficulty\" must be a bool.");
					if (!(args[1] is string))
						return new ArgumentException("ERROR: The first argument to \"SetDifficulty\" must be a string.");
					return SetDifficultyActive(args[1].ToString(), enabled);

				case "GetLight":
				case "GetLightLevel":
				case "GetLightStrength":
				case "GetAbyssLight":
				case "GetAbyssLightLevel":
				case "GetAbyssLightStrength":
					if (args.Length < 2)
						return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
					if(!isValidPlayerArg(args[1]))
						return new ArgumentException("ERROR: The argument to \"GetLightStrength\" must be a Player or an int.");
					return GetLightStrength(castPlayer(args[1]));

				case "AddLight":
				case "AddLightLevel":
				case "AddLightStrength":
				case "AddAbyssLight":
				case "AddAbyssLightLevel":
				case "AddAbyssLightStrength":
					if (args.Length < 2)
						return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and light strength change as an int.");
					if (args.Length < 3)
						return new ArgumentNullException("ERROR: Must specify light strength change as an int.");
					if (!(args[2] is int light))
						return new ArgumentException("ERROR: The second argument to \"AddLightStrength\" must be an int.");
					if (!isValidPlayerArg(args[1]))
						return new ArgumentException("ERROR: The first argument to \"AddLightStrength\" must be a Player or an int.");
					AddAbyssLightStrength(castPlayer(args[1]), light);
					return null;

				case "ColdImmune":
				case "ColdImmunity":
				case "GiveColdImmunity":
				case "MakeColdImmune":
				case "DeathModeCold":
					if (args.Length < 2)
						return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
					if (!isValidPlayerArg(args[1]))
						return new ArgumentException("ERROR: The argument to \"DeathModeCold\" must be a Player or an int.");
					return MakeColdImmune(castPlayer(args[1]));

				case "HeatImmune":
				case "HeatImmunity":
				case "GiveHeatImmunity":
				case "MakeHeatImmune":
				case "DeathModeHeat":
					if (args.Length < 2)
						return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
					if (!isValidPlayerArg(args[1]))
						return new ArgumentException("ERROR: The argument to \"DeathModeHeat\" must be a Player or an int.");
					return MakeHeatImmune(castPlayer(args[1]));

				case "GetRogueDamage":
				case "GetRogueDmg":
					if (args.Length < 2)
						return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
					if (!isValidPlayerArg(args[1]))
						return new ArgumentException("ERROR: The argument to \"GetRogueDamage\" must be a Player or an int.");
					return GetRogueDamage(castPlayer(args[1]));

				case "GetRogueCrit":
				case "GetRogueCritChance":
					if (args.Length < 2)
						return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
					if (!isValidPlayerArg(args[1]))
						return new ArgumentException("ERROR: The argument to \"GetRogueCrit\" must be a Player or an int.");
					return GetRogueCrit(castPlayer(args[1]));

				case "GetRogueVelocity":
					if (args.Length < 2)
						return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
					if (!isValidPlayerArg(args[1]))
						return new ArgumentException("ERROR: The argument to \"GetRogueVelocity\" must be a Player or an int.");
					return GetRogueVelocity(castPlayer(args[1]));

				case "AddRogueDamage":
				case "AddRogueDmg":
				case "ModifyRogueDamage":
				case "ModifyRogueDmg":
					if (args.Length < 2)
						return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and rogue damage change as a float.");
					if (args.Length < 3)
						return new ArgumentNullException("ERROR: Must specify rogue damage change as a float.");
					if (!(args[2] is float damage))
						return new ArgumentException("ERROR: The second argument to \"AddRogueDamage\" must be a float.");
					if (!isValidPlayerArg(args[1]))
						return new ArgumentException("ERROR: The first argument to \"AddRogueDamage\" must be a Player or an int.");
					return AddRogueDamage(castPlayer(args[1]), damage);

				case "AddRogueCrit":
				case "AddRogueCritChance":
				case "ModifyRogueCrit":
				case "ModifyRogueCritChance":
					if (args.Length < 2)
						return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and rogue crit change as an int.");
					if (args.Length < 3)
						return new ArgumentNullException("ERROR: Must specify rogue crit change as a float.");
					if (!(args[2] is int crit))
						return new ArgumentException("ERROR: The second argument to \"AddRogueCrit\" must be an int.");
					if (!isValidPlayerArg(args[1]))
						return new ArgumentException("ERROR: The first argument to \"AddRogueCrit\" must be a Player or an int.");
					return AddRogueCrit(castPlayer(args[1]), crit);

				case "AddRogueVelocity":
				case "ModifyRogueVelocity":
					if (args.Length < 2)
						return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and rogue velocity change as a float.");
					if (args.Length < 3)
						return new ArgumentNullException("ERROR: Must specify rogue velocity change as a float.");
					if (!(args[2] is float velocity))
						return new ArgumentException("ERROR: The second argument to \"AddRogueVelocity\" must be a float.");
					if (!isValidPlayerArg(args[1]))
						return new ArgumentException("ERROR: The first argument to \"AddRogueVelocity\" must be a Player or an int.");
					return AddRogueVelocity(castPlayer(args[1]), velocity);

				case "GetStealth":
				case "GetCurrentStealth":
					if (args.Length < 2)
						return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
					if (!isValidPlayerArg(args[1]))
						return new ArgumentException("ERROR: The first argument to \"GetStealth\" must be a Player or an int.");
					return GetCurrentStealth(castPlayer(args[1]));

				case "GetMaxStealth":
				case "GetStealthCap":
					if (args.Length < 2)
						return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
					if (!isValidPlayerArg(args[1]))
						return new ArgumentException("ERROR: The first argument to \"GetMaxStealth\" must be a Player or an int.");
					return GetMaxStealth(castPlayer(args[1]));

				case "AddMaxStealth":
				case "ModifyMaxStealth":
				case "ModifyStealthCap":
					if (args.Length < 2)
						return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and rogue max stealth as a float.");
					if (args.Length < 3)
						return new ArgumentNullException("ERROR: Must specify rogue max stealth as a float.");
					if (!(args[2] is float maxStealth))
						return new ArgumentException("ERROR: The second argument to \"AddMaxStealth\" must be a float.");
					if (!isValidPlayerArg(args[1]))
						return new ArgumentException("ERROR: The first argument to \"AddMaxStealth\" must be a Player or an int.");
					return AddMaxStealth(castPlayer(args[1]), maxStealth);

				case "IsRogue":
				case "IsProjRogue":
				case "IsProjectileRogue":
					if (args.Length < 2)
						return new ArgumentNullException("ERROR: Must specify a Projectile object (or int index of a Projectile).");
					if (!isValidProjectileArg(args[1]))
						return new ArgumentException("ERROR: The first argument to \"IsRogue\" must be a Projectile or an int.");
					return IsRogue(castProjectile(args[1]));

				case "SetRogue":
				case "SetProjRogue":
				case "SetProjectileRogue":
					if (args.Length < 2)
						return new ArgumentNullException("ERROR: Must specify both a Projectile object (or int index of a Projectile) and a bool.");
					if (args.Length < 3)
						return new ArgumentNullException("ERROR: Must specify rogue status as a bool.");
					if (!(args[2] is bool isRogue))
						return new ArgumentException("ERROR: The second argument to \"SetRogue\" must be a bool.");
					if (!isValidProjectileArg(args[1]))
						return new ArgumentException("ERROR: The first argument to \"SetRogue\" must be a Projectile or an int.");
					return SetRogue(castProjectile(args[1]), isRogue);

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
					if (!isValidPlayerArg(args[1]))
						return new ArgumentException("ERROR: The first argument to \"SetBonus\" must be a Player or an int.");
					return GetSetBonus(castPlayer(args[1]), args[2].ToString());

				case "SetSetBonus":
				case "ToggleSetBonus":
				case "SetSetBonusActive":
				case "ToggleSetBonusActive":
					if (args.Length < 2)
						return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player), a set bonus name as a string, and a bool.");
					if (args.Length < 3)
						return new ArgumentNullException("ERROR: Must specify a set bonus name as a string and a bool.");
					if (args.Length < 4)
						return new ArgumentNullException("ERROR: Must specify a bool.");
					if (!(args[3] is bool setBonusEnabled))
						return new ArgumentException("ERROR: The third argument to \"SetSetBonus\" must be a bool.");
					if (!(args[2] is string))
						return new ArgumentException("ERROR: The second argument to \"SetSetBonus\" must be a string.");
					if (!isValidPlayerArg(args[1]))
						return new ArgumentException("ERROR: The first argument to \"SetSetBonus\" must be a Player or an int.");
					return SetSetBonus(castPlayer(args[1]), args[2].ToString(), setBonusEnabled);

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

				case "BossHealthBarVisible":
				case "BossHealthBarsVisible":
				case "GetBossHealthBarVisible":
				case "GetBossHealthBarsVisible":
					return BossHealthBarVisible();

				case "SetBossHealthBarVisible":
				case "SetBossHealthBarsVisible":
					if (args.Length < 2 || !(args[1] is bool bossBarEnabled))
						return new ArgumentNullException("ERROR: Must specify a bool.");
					return SetBossHealthBarVisible(bossBarEnabled);

				case "GetRarity":
				case "GetItemRarity":
				case "GetCalamityRarity":
				case "GetPostMLRarity":
				case "GetPostMoonLordRarity":
					if (args.Length < 2)
						return new ArgumentNullException("ERROR: Must specify an Item.");
					if (!(args[1] is Item))
						return new ArgumentException("ERROR: The first argument to \"GetCalamityRarity\" must be an Item.");
					Item itemToGet = (Item)args[1];
					return GetCalamityRarity(itemToGet);

				case "SetRarity":
				case "SetItemRarity":
				case "SetCalamityRarity":
				case "SetPostMLRarity":
				case "SetPostMoonLordRarity":
					if (args.Length < 2)
						return new ArgumentNullException("ERROR: Must specify both an Item and desired rarity as an int.");
					if (args.Length < 3)
						return new ArgumentNullException("ERROR: Must specify desired rarity as an int.");
					if (!(args[2] is int))
						return new ArgumentException("ERROR: The second argument to \"SetCalamityRarity\" must be an int.");
					if (!(args[1] is Item))
						return new ArgumentException("ERROR: The first argument to \"SetCalamityRarity\" must be an Item.");

					Item itemToSet = (Item)args[1];
					int rarity = (int)args[2];
					return SetCalamityRarity(itemToSet, rarity);

				case "AbominationnClearEvents":
					bool eventActive = CalamityWorld.rainingAcid;
					bool canClear = Convert.ToBoolean(args[1]); //This is to indicate whether abomm is able to clear the event due to a cooldown
					if (eventActive && canClear) //adjust based on other events when added.
					{
						CalamityWorld.acidRainPoints = 0;
						CalamityWorld.triedToSummonOldDuke = false;
						AcidRainEvent.UpdateInvasion(false);
					}
					return eventActive;

				default:
					return new ArgumentException("ERROR: Invalid method name.");
			}
		}
		#endregion
	}
}
