using CalamityMod.Buffs.Summon;
using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Dyes;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Placeables.FurnitureCosmilite;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.SummonItems.Invasion;
using CalamityMod.Items.Tools.ClimateChange;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Weapons.Typeless;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.AdultEidolonWyrm;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.GreatSandShark;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Projectiles.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using static CalamityMod.Downed;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod
{
	internal class Downed
	{
		public static readonly Func<bool> DownedDesertScourge = () => CalamityWorld.downedDesertScourge;
		public static readonly Func<bool> DownedGiantClam = () => CalamityWorld.downedCLAM;
		public static readonly Func<bool> DownedCrabulon = () => CalamityWorld.downedCrabulon;
		public static readonly Func<bool> DownedHiveMind = () => CalamityWorld.downedHiveMind;
		public static readonly Func<bool> DownedPerfs = () => CalamityWorld.downedPerforator;
		public static readonly Func<bool> DownedSlimeGod = () => CalamityWorld.downedSlimeGod;
		public static readonly Func<bool> DownedCryogen = () => CalamityWorld.downedCryogen;
		public static readonly Func<bool> DownedBrimstoneElemental = () => CalamityWorld.downedBrimstoneElemental;
		public static readonly Func<bool> DownedAquaticScourge = () => CalamityWorld.downedAquaticScourge;
		public static readonly Func<bool> DownedCalamitas = () => CalamityWorld.downedCalamitas;
		public static readonly Func<bool> DownedGSS = () => CalamityWorld.downedGSS;
		public static readonly Func<bool> DownedLeviathan = () => CalamityWorld.downedLeviathan;
		public static readonly Func<bool> DownedAureus = () => CalamityWorld.downedAstrageldon;
		public static readonly Func<bool> DownedPBG = () => CalamityWorld.downedPlaguebringer;
		public static readonly Func<bool> DownedRavager = () => CalamityWorld.downedScavenger;
		public static readonly Func<bool> DownedDeus = () => CalamityWorld.downedStarGod;
		public static readonly Func<bool> DownedGuardians = () => CalamityWorld.downedGuardians;
		public static readonly Func<bool> DownedBirb = () => CalamityWorld.downedBumble;
		public static readonly Func<bool> DownedProvidence = () => CalamityWorld.downedProvidence;
		public static readonly Func<bool> DownedCeaselessVoid = () => CalamityWorld.downedSentinel1;
		public static readonly Func<bool> DownedStormWeaver = () => CalamityWorld.downedSentinel2;
		public static readonly Func<bool> DownedSignus = () => CalamityWorld.downedSentinel3;
		public static readonly Func<bool> DownedPolterghast = () => CalamityWorld.downedPolterghast;
		public static readonly Func<bool> DownedBoomerDuke = () => CalamityWorld.downedBoomerDuke;
		public static readonly Func<bool> DownedDoG = () => CalamityWorld.downedDoG;
		public static readonly Func<bool> DownedYharon = () => CalamityWorld.downedYharon;
		public static readonly Func<bool> DownedExoMechs = () => CalamityWorld.downedExoMechs;
		public static readonly Func<bool> DownedSCal = () => CalamityWorld.downedSCal;
		public static readonly Func<bool> DownedAdultEidolonWyrm = () => CalamityWorld.downedAdultEidolonWyrm;

		public static readonly Func<bool> DownedAcidRainInitial = () => CalamityWorld.downedEoCAcidRain;
		public static readonly Func<bool> DownedAcidRainHardmode = () => CalamityWorld.downedAquaticScourgeAcidRain;
	}

	internal class WeakReferenceSupport
	{
		private static readonly Dictionary<string, float> BossDifficulty = new Dictionary<string, float>
		{
			{ "DesertScourge", 1.5f },
			{ "GiantClam", 1.6f },
			{ "Crabulon", 2.5f },
			{ "HiveMind", 3.5f },
			{ "Perforators", 3.51f },
			{ "SlimeGod", 5.5f },
			{ "Cryogen", 6.5f },
			{ "AquaticScourge", 7.5f },
			{ "BrimstoneElemental", 8.5f },
			{ "Calamitas", 9.7f },
			{ "GreatSandShark", 10.09f },
			{ "Leviathan", 10.5f },
			{ "AstrumAureus", 10.6f },
			{ "PlaguebringerGoliath", 11.5f },
			{ "Ravager", 12.5f },
			{ "AstrumDeus", 13.5f },
			{ "ProfanedGuardians", 14.5f },
			{ "Dragonfolly", 14.6f },
			{ "Providence", 15.01f }, // Thorium's Ragnarok is 15f
			{ "CeaselessVoid", 15.5f },
			{ "StormWeaver", 15.51f },
			{ "Signus", 15.52f },
			{ "Polterghast", 16f },
			{ "OldDuke", 16.5f },
			{ "DevourerOfGods", 17f },
			{ "Yharon", 18f },
			{ "ExoMechs", 18.5f },
			{ "SupremeCalamitas", 19f },
			{ "AdultEidolonWyrm", 19.5f },
			// { "Yharim", 20f },
			// { "Noxus", 120f },
			// { "Xeroc", 121f },
		};

		private static readonly Dictionary<string, float> InvasionDifficulty = new Dictionary<string, float>
		{
			{ "Acid Rain Initial", 2.4f },
			{ "Acid Rain Aquatic Scourge", 7.51f },
			{ "Acid Rain Polterghast", 16.49f }
		};

		public static void Setup()
		{
			BossChecklistSupport();
			FargosSupport();
			CensusSupport();
			SummonersAssociationSupport();
		}

		// Wrapper function to add bosses to Boss Checklist.
		private static void AddBoss(Mod bossChecklist, Mod hostMod, string name, float difficulty, int npcType, Func<bool> downed, object summon,
			List<int> loot, List<int> collection, string instructions, string despawn, string bossLogTex = null, string bossHeadTex = null)
		{
			bossChecklist.Call("AddBoss", difficulty, npcType, hostMod, name, downed, summon ?? null, collection, loot, instructions, despawn, bossLogTex, bossHeadTex);
		}

		// Wrapper function to add bosses with multiple segments or phases to Boss Checklist.
		private static void AddBoss(Mod bossChecklist, Mod hostMod, string name, float difficulty, List<int> npcTypes, Func<bool> downed, object summon,
			List<int> loot, List<int> collection, string instructions, string despawn, string bossLogTex = null, string bossHeadTex = null)
		{
			bossChecklist.Call("AddBoss", difficulty, npcTypes, hostMod, name, downed, summon ?? null, collection, loot, instructions, despawn, bossLogTex, bossHeadTex);
		}

		// Wrapper function to add bosses with multiple segments or phases to Boss Checklist.
		private static void AddInvasion(Mod bossChecklist, Mod hostMod, string name, float difficulty, List<int> npcTypes, Func<bool> downed, object summon,
			List<int> loot, List<int> collection, string instructions, string despawn, string invasionTexture = null, string iconTexture = null)
		{
			bossChecklist.Call("AddEventWithInfo", difficulty, npcTypes, hostMod, name, downed, summon ?? null, collection, loot, instructions, despawn, invasionTexture, iconTexture);
		}

		// Wrapper function to add minibosses to Boss Checklist.
		private static void AddMiniBoss(Mod bossChecklist, Mod hostMod, string name, float difficulty, int npcType, Func<bool> downed, object summon,
			List<int> loot, List<int> collection, string instructions, string despawn, string bossLogTex = null, string bossHeadTex = null)
		{
			bossChecklist.Call("AddMiniBoss", difficulty, npcType, hostMod, name, downed, summon ?? null, collection, loot, instructions, despawn, bossLogTex, bossHeadTex);
		}

		// Wrapper function to add loot and collection items to vanilla bosses for Boss Checklist.
		private static void AddLoot(Mod bossChecklist, string bossName, List<int> loot = null, List<int> collection = null)
			=> AddLoot(bossChecklist, "Terraria", bossName, loot, collection);

		// Wrapper function to add loot and collection items to other mods' bosses for Boss Checklist.
		private static void AddLoot(Mod bossChecklist, string mod, string bossName, List<int> loot = null, List<int> collection = null)
		{
			if (loot != null)
				bossChecklist.Call("AddToBossLoot", mod, bossName, loot);
			if (collection != null)
				bossChecklist.Call("AddToBossCollection", mod, bossName, collection);
		}

		// Wrapper function to add summon items to vanilla bosses for Boss Checklist.
		private static void AddSummons(Mod bossChecklist, string bossName, List<int> summons) => AddSummons(bossChecklist, "Terraria", bossName, summons);

		// Wrapper function to add summon items to other mods' bosses for Boss Checklist.
		private static void AddSummons(Mod bossChecklist, string mod, string bossName, List<int> summons) => bossChecklist.Call("AddToBossSpawnItems", mod, bossName, summons);

		/// <summary>
		/// 1.0 = King Slime<br />
		/// 2.0 = Eye of Cthulhu<br />
		/// 3.0 = Eater of Worlds / Brain of Cthulhu<br />
		/// 4.0 = Queen Bee<br />
		/// 5.0 = Skeletron<br />
		/// 6.0 = Wall of Flesh<br />
		/// 7.0 = The Twins<br />
		/// 8.0 = The Destroyer<br />
		/// 9.0 = Skeletron Prime<br />
		/// 10.0 = Plantera<br />
		/// 11.0 = Golem<br />
		/// 12.0 = Duke Fishron<br />
		/// 13.0 = Lunatic Cultist<br />
		/// 14.0 = Moon Lord
		/// </summary>
		private static void BossChecklistSupport()
		{
			CalamityMod calamity = GetInstance<CalamityMod>();
			Mod bossChecklist = calamity.bossChecklist;
			if (bossChecklist is null)
				return;

			// Adds every single Calamity boss and miniboss to Boss Checklist's Boss Log.
			AddCalamityBosses(bossChecklist, calamity);

			// Adds every single Calamity invasion to the Boss Checklist's Invasion Log.
			AddCalamityInvasions(bossChecklist, calamity);

			// Loot which Calamity adds to vanilla bosses and events is also added to Boss Checklist's Boss Log.
			AddCalamityBossLoot(bossChecklist);
			AddCalamityEventLoot(bossChecklist);
		}

		private static void AddCalamityBosses(Mod bossChecklist, Mod calamity)
		{
			// Desert Scourge
			{
				BossDifficulty.TryGetValue("DesertScourge", out float order);
				List<int> segments = new List<int>() { NPCType<DesertScourgeHead>(), NPCType<DesertScourgeBody>(), NPCType<DesertScourgeTail>() };
				int summon = ItemType<DriedSeafood>();
				List<int> loot = new List<int>() { ItemType<DesertScourgeBag>(), ItemID.SandBlock, ItemType<VictoryShard>(), ItemID.Coral, ItemID.Seashell, ItemID.Starfish, ItemType<AquaticDischarge>(), ItemType<Barinade>(), ItemType<StormSpray>(), ItemType<SeaboundStaff>(), ItemType<ScourgeoftheDesert>(), ItemType<AeroStone>(), ItemType<SandCloak>(), ItemType<OceanCrest>(),  ItemType<SandyAnglingKit>(), ItemID.LesserHealingPotion };
				List<int> collection = new List<int>() { ItemType<DesertScourgeTrophy>(), ItemType<DesertScourgeMask>(), ItemType<KnowledgeDesertScourge>() };
				string instructions = $"Use a [i:{summon}] in the Desert Biome";
				string despawn = CalamityUtils.ColorMessage("The scourge of the desert delved back into the sand.", new Color(0xEE, 0xE8, 0xAA));
				string bossLogTex = "CalamityMod/NPCs/DesertScourge/DesertScourge_BossChecklist";
				AddBoss(bossChecklist, calamity, "Desert Scourge", order, segments, DownedDesertScourge, summon, loot, collection, instructions, despawn, bossLogTex);
			}

			// Giant Clam
			{
				BossDifficulty.TryGetValue("GiantClam", out float order);
				int type = NPCType<GiantClam>();
				List<int> loot = new List<int>() { ItemType<Navystone>(), ItemType<MolluskHusk>(), ItemType<ClamCrusher>(), ItemType<ClamorRifle>(), ItemType<Poseidon>(), ItemType<ShellfishStaff>(), ItemType<GiantPearl>(), ItemType<AmidiasPendant>() };
				string instructions = "Can spawn naturally in the Sunken Sea";
				string despawn = CalamityUtils.ColorMessage("The Giant Clam returns into hiding in its grotto.", new Color(0x7F, 0xFF, 0xD4));
				AddMiniBoss(bossChecklist, calamity, "Giant Clam", order, type, DownedGiantClam, null, loot, null, instructions, despawn);
			}

			// Crabulon
			{
				BossDifficulty.TryGetValue("Crabulon", out float order);
				int type = NPCType<CrabulonIdle>();
				int summon = ItemType<DecapoditaSprout>();
				List<int> loot = new List<int>() { ItemType<CrabulonBag>(), ItemID.GlowingMushroom, ItemID.MushroomGrassSeeds, ItemType<MycelialClaws>(), ItemType<Fungicide>(), ItemType<HyphaeRod>(), ItemType<Mycoroot>(), ItemType<Shroomerang>(), ItemType<FungalClump>(), ItemType<MushroomPlasmaRoot>(), ItemID.LesserHealingPotion };
				List<int> collection = new List<int>() { ItemType<CrabulonTrophy>(), ItemType<CrabulonMask>(), ItemType<KnowledgeCrabulon>() };
				string instructions = $"Use a [i:{summon}] in the Mushroom Biome";
				string despawn = CalamityUtils.ColorMessage("The mycleium crab has lost interest.", new Color(0x64, 0x95, 0xED));
				AddBoss(bossChecklist, calamity, "Crabulon", order, type, DownedCrabulon, summon, loot, collection, instructions, despawn);
			}

			// Hive Mind
			{
				BossDifficulty.TryGetValue("HiveMind", out float order);
				int type = NPCType<HiveMind>();
				int summon = ItemType<Teratoma>();
				List<int> loot = new List<int>() { ItemType<HiveMindBag>(), ItemType<TrueShadowScale>(), ItemID.DemoniteBar, ItemID.RottenChunk, ItemID.CorruptSeeds, ItemID.CursedFlame, ItemType<PerfectDark>(), ItemType<LeechingDagger>(), ItemType<Shadethrower>(), ItemType<ShadowdropStaff>(), ItemType<ShaderainStaff>(), ItemType<DankStaff>(), ItemType<RotBall>(), ItemType<FilthyGlove>(), ItemType<RottenBrain>(), ItemID.LesserHealingPotion };
				List<int> collection = new List<int>() { ItemType<HiveMindTrophy>(), ItemType<HiveMindMask>(), ItemType<KnowledgeHiveMind>(), ItemType<RottingEyeball>() };
				string instructions = $"Kill a Cyst in the Corruption or use a [i:{summon}] in the Corruption";
				string despawn = CalamityUtils.ColorMessage("The corrupted colony began searching for a new breeding ground.", new Color(0x94, 0x00, 0xD3));
				AddBoss(bossChecklist, calamity, "Hive Mind", order, type, DownedHiveMind, summon, loot, collection, instructions, despawn);
			}

			// Perforators
			{
				BossDifficulty.TryGetValue("Perforators", out float order);
				int type = NPCType<PerforatorHive>();
				int summon = ItemType<BloodyWormFood>();
				List<int> loot = new List<int>() { ItemType<PerforatorBag>(), ItemType<BloodSample>(), ItemID.CrimtaneBar, ItemID.Vertebrae, ItemID.CrimsonSeeds, ItemID.Ichor, ItemType<VeinBurster>(), ItemType<BloodyRupture>(), ItemType<SausageMaker>(), ItemType<Aorta>(), ItemType<Eviscerator>(), ItemType<BloodBath>(), ItemType<BloodClotStaff>(), ItemType<ToothBall>(), ItemType<BloodstainedGlove>(), ItemType<BloodyWormTooth>(), ItemID.LesserHealingPotion };
				List<int> collection = new List<int>() { ItemType<PerforatorTrophy>(), ItemType<PerforatorMask>(), ItemType<KnowledgePerforators>(), ItemType<BloodyVein>() };
				string instructions = $"Kill a Cyst in the Crimson or use a [i:{summon}] in the Crimson";
				string despawn = CalamityUtils.ColorMessage("The parasitic hive began searching for a new host.", new Color(0xDC, 0x14, 0x3C));
				AddBoss(bossChecklist, calamity, "The Perforators", order, type, DownedPerfs, summon, loot, collection, instructions, despawn);
			}

			// Slime God
			{
				BossDifficulty.TryGetValue("SlimeGod", out float order);
				List<int> bosses = new List<int>() { NPCType<SlimeGodCore>(), NPCType<SlimeGod>(), NPCType<SlimeGodRun>() };
				int summon = ItemType<OverloadedSludge>();
				List<int> loot = new List<int>() { ItemType<SlimeGodBag>(), ItemID.Gel, ItemType<PurifiedGel>(), ItemType<OverloadedBlaster>(), ItemType<AbyssalTome>(), ItemType<EldritchTome>(), ItemType<CorroslimeStaff>(), ItemType<CrimslimeStaff>(), ItemType<SlimePuppetStaff>(), ItemType<ManaOverloader>(), ItemType<ElectrolyteGelPack>(), ItemType<PurifiedJam>(), ItemID.HealingPotion };
				List<int> collection = new List<int>() { ItemType<SlimeGodTrophy>(), ItemType<SlimeGodMask>(), ItemType<SlimeGodMask2>(), ItemType<KnowledgeSlimeGod>() };
				string instructions = $"Use an [i:{summon}]";
				string despawn = CalamityUtils.ColorMessage("The gelatinous monstrosity achieved vengeance for its brethren.", new Color(0xBA, 0x55, 0x33));
				AddBoss(bossChecklist, calamity, "Slime God", order, bosses, DownedSlimeGod, summon, loot, collection, instructions, despawn);
			}

			// Cryogen
			{
				BossDifficulty.TryGetValue("Cryogen", out float order);
				int type = NPCType<Cryogen>();
				int summon = ItemType<CryoKey>();
				List<int> loot = new List<int>() { ItemType<CryogenBag>(), ItemType<EssenceofEleum>(), ItemType<Avalanche>(), ItemType<GlacialCrusher>(), ItemType<EffluviumBow>(), ItemType<SnowstormStaff>(), ItemType<Icebreaker>(), ItemType<ColdDivinity>(), ItemType<CryoStone>(), ItemType<SoulofCryogen>(), ItemType<FrostFlare>(), ItemID.FrozenKey, ItemID.GreaterHealingPotion };
				List<int> collection = new List<int>() { ItemType<CryogenTrophy>(), ItemType<CryogenMask>(), ItemType<KnowledgeCryogen>() };
				string instructions = $"Use a [i:{summon}] in the Snow Biome";
				string despawn = CalamityUtils.ColorMessage("Cryogen drifts away, carried on a freezing wind.", new Color(0x00, 0xFF, 0xFF));
				string bossLogTex = "CalamityMod/NPCs/Cryogen/Cryogen_BossChecklist";
				AddBoss(bossChecklist, calamity, "Cryogen", order, type, DownedCryogen, summon, loot, collection, instructions, despawn, bossLogTex);
			}

			// Aquatic Scourge
			{
				BossDifficulty.TryGetValue("AquaticScourge", out float order);
				List<int> segments = new List<int>() { NPCType<AquaticScourgeHead>(), NPCType<AquaticScourgeBody>(), NPCType<AquaticScourgeBodyAlt>(), NPCType<AquaticScourgeTail>() };
				int summon = ItemType<Seafood>();
				List<int> loot = new List<int>() { ItemType<AquaticScourgeBag>(), ItemType<SulphurousSand>(), ItemType<SubmarineShocker>(), ItemType<Barinautical>(), ItemType<Downpour>(), ItemType<DeepseaStaff>(), ItemType<ScourgeoftheSeas>(), ItemType<SeasSearing>(), ItemType<AquaticEmblem>(), ItemType<CorrosiveSpine>(), ItemType<DeepDiver>(), ItemType<BleachedAnglingKit>(), ItemID.GreaterHealingPotion };
				List<int> collection = new List<int>() { ItemType<AquaticScourgeTrophy>(), ItemType<AquaticScourgeMask>(), ItemType<KnowledgeAquaticScourge>(), ItemType<KnowledgeSulphurSea>() };
				string instructions = $"Use a [i:{summon}] in the Sulphuric Sea or wait for it to spawn in the Sulphuric Sea";
				string despawn = CalamityUtils.ColorMessage("The Aquatic Scourge swam back into the open ocean.", new Color(0xF0, 0xE6, 0x8C));
				string bossLogTex = "CalamityMod/NPCs/AquaticScourge/AquaticScourge_BossChecklist";
				AddBoss(bossChecklist, calamity, "Aquatic Scourge", order, segments, DownedAquaticScourge, summon, loot, collection, instructions, despawn, bossLogTex);
			}

			// Brimstone Elemental
			{
				BossDifficulty.TryGetValue("BrimstoneElemental", out float order);
				int type = NPCType<BrimstoneElemental>();
				int summon = ItemType<CharredIdol>();
				List<int> loot = new List<int>() { ItemType<BrimstoneWaifuBag>(), ItemType<EssenceofChaos>(), ItemType<Bloodstone>(), ItemType<Brimlance>(), ItemType<DormantBrimseeker>(), ItemType<SeethingDischarge>(), ItemType<Abaddon>(), ItemType<RoseStone>(), ItemType<Gehenna>(), ItemType<Brimrose>(), ItemType<Hellborn>(), ItemType<FabledTortoiseShell>(), ItemID.GreaterHealingPotion };
				List<int> collection = new List<int>() { ItemType<BrimstoneElementalTrophy>(), ItemType<BrimstoneWaifuMask>(), ItemType<KnowledgeBrimstoneCrag>(), ItemType<KnowledgeBrimstoneElemental>(), ItemType<CharredRelic>() };
				string instructions = $"Use a [i:{summon}] in the Brimstone Crag";
				string despawn = CalamityUtils.ColorMessage("Brimstone Elemental withdraws to the ruins of her shrine.", new Color(0xDC, 0x14, 0x3C));
				AddBoss(bossChecklist, calamity, "Brimstone Elemental", order, type, DownedBrimstoneElemental, summon, loot, collection, instructions, despawn);
			}

			// Calamitas
			{
				BossDifficulty.TryGetValue("Calamitas", out float order);
				int type = NPCType<CalamitasRun3>();
				int summon = ItemType<BlightedEyeball>();
				List<int> loot = new List<int>() { ItemType<CalamitasBag>(), ItemType<EssenceofChaos>(), ItemType<CalamityDust>(), ItemType<BlightedLens>(), ItemType<Bloodstone>(), ItemType<CalamitasInferno>(), ItemType<TheEyeofCalamitas>(), ItemType<BlightedEyeStaff>(), ItemType<Animosity>(), ItemType<BrimstoneFlamesprayer>(), ItemType<BrimstoneFlameblaster>(), ItemType<CrushsawCrasher>(), ItemType<ChaosStone>(), ItemType<CalamityRing>(), ItemType<Regenator>(), ItemID.BrokenHeroSword, ItemID.GreaterHealingPotion };
				List<int> collection = new List<int>() { ItemType<CalamitasTrophy>(), ItemType<CataclysmTrophy>(), ItemType<CatastropheTrophy>(), ItemType<CalamitasMask>(), ItemType<CalamityHood>(), ItemType<CalamityRobes>(), ItemType<KnowledgeCalamitasClone>() };
				string instructions = $"Use an [i:{summon}] at Night";
				string despawn = CalamityUtils.ColorMessage("If you wanted a fight, you should've came more prepared.", new Color(0xFF, 0xA5, 0x00));
				AddBoss(bossChecklist, calamity, "Calamitas", order, type, DownedCalamitas, summon, loot, collection, instructions, despawn);
			}

			// Great Sand Shark
			{
				BossDifficulty.TryGetValue("GreatSandShark", out float order);
				int type = NPCType<GreatSandShark>();
				int summon = ItemType<SandstormsCore>();
				List<int> loot = new List<int>() { ItemType<GrandScale>(), ItemID.AncientBattleArmorMaterial };
				List<int> collection = new List<int>() { ItemID.MusicBoxSandstorm };
				string instructions = $"Kill 10 sand sharks after defeating Plantera or use a [i:{summon}] in the Desert Biome";
				string despawn = CalamityUtils.ColorMessage("The apex predator of the sands disappears into the dunes...", new Color(0xDA, 0xA5, 0x20));
				AddMiniBoss(bossChecklist, calamity, "Great Sand Shark", order, type, DownedGSS, summon, loot, collection, instructions, despawn);
			}

			// Siren and Leviathan
			{
				BossDifficulty.TryGetValue("Leviathan", out float order);
				List<int> bosses = new List<int>() { NPCType<Leviathan>(), NPCType<Siren>() };
				List<int> loot = new List<int>() { ItemType<LeviathanBag>(), ItemType<Greentide>(), ItemType<Leviatitan>(), ItemType<SirensSong>(), ItemType<Atlantis>(), ItemType<GastricBelcherStaff>(), ItemType<BrackishFlask>(), ItemType<LeviathanTeeth>(), ItemType<LureofEnthrallment>(), ItemType<LeviathanAmbergris>(), ItemType<TheCommunity>(), ItemID.HotlineFishingHook, ItemID.BottomlessBucket, ItemID.SuperAbsorbantSponge, ItemID.FishingPotion, ItemID.SonarPotion, ItemID.CratePotion, ItemID.GreaterHealingPotion };
				List<int> collection = new List<int>() { ItemType<LeviathanTrophy>(), ItemType<AnahitaTrophy>(), ItemType<LeviathanMask>(), ItemType<AnahitaMask>(), ItemType<KnowledgeOcean>(), ItemType<KnowledgeLeviathanandSiren>() };
				string instructions = "By killing an unknown entity in the Ocean Biome";
				string despawn = CalamityUtils.ColorMessage("The aquatic entities sink back beneath the ocean depths.", new Color(0x7F, 0xFF, 0xD4));
				string bossLogTex = "CalamityMod/NPCs/Leviathan/SirenandLevi_BossChecklist";
				AddBoss(bossChecklist, calamity, "Leviathan", order, bosses, DownedLeviathan, null, loot, collection, instructions, despawn, bossLogTex);
			}

			// Astrum Aureus
			{
				BossDifficulty.TryGetValue("AstrumAureus", out float order);
				int type = NPCType<AstrumAureus>();
				int summon = ItemType<AstralChunk>();
				List<int> loot = new List<int>() { ItemType<AstrageldonBag>(), ItemType<Stardust>(), ItemID.FallenStar, ItemType<Nebulash>(), ItemType<AuroraBlazer>(), ItemType<AlulaAustralis>(), ItemType<BorealisBomber>(), ItemType<AuroradicalThrow>(), ItemType<LeonidProgenitor>(), ItemType<GravistarSabaton>(), ItemType<AstralJelly>(), ItemID.HallowedKey, ItemType<StarlightFuelCell>(), ItemID.GreaterHealingPotion };
				List<int> collection = new List<int>() { ItemType<AstrageldonTrophy>(), ItemType<AureusMask>(), ItemType<KnowledgeAstrumAureus>() };
				string instructions = $"Use an [i:{summon}] at Night in the Astral Biome";
				string despawn = CalamityUtils.ColorMessage("Astrum Aureus’ program has been executed. Initiate recall.", new Color(0xFF, 0xD7, 0x00));
				string bossLogTex = "CalamityMod/NPCs/AstrumAureus/AstrumAureus_BossChecklist";
				AddBoss(bossChecklist, calamity, "Astrum Aureus", order, type, DownedAureus, summon, loot, collection, instructions, despawn, bossLogTex);
			}

			// Plaguebringer Goliath
			{
				BossDifficulty.TryGetValue("PlaguebringerGoliath", out float order);
				int type = NPCType<PlaguebringerGoliath>();
				int summon = ItemType<Abomination>();
				List<int> loot = new List<int>() { ItemType<PlaguebringerGoliathBag>(), ItemType<PlagueCellCluster>(), ItemType<InfectedArmorPlating>(), ItemID.Stinger, ItemType<VirulentKatana>(), ItemType<DiseasedPike>(), ItemType<ThePlaguebringer>(), ItemType<Malevolence>(), ItemType<PestilentDefiler>(), ItemType<TheHive>(), ItemType<MepheticSprayer>(), ItemType<PlagueStaff>(), ItemType<TheSyringe>(), ItemType<FuelCellBundle>(), ItemType<InfectedRemote>(), ItemType<Malachite>(), ItemType<ToxicHeart>(), ItemID.GreaterHealingPotion };
				List<int> collection = new List<int>() { ItemType<PlaguebringerGoliathTrophy>(), ItemType<PlaguebringerGoliathMask>(), ItemType<KnowledgePlaguebringerGoliath>(), ItemType<PlagueCaller>() };
				string instructions = $"Use an [i:{summon}] in the Jungle Biome";
				string despawn = CalamityUtils.ColorMessage("HOSTILE SPECIMENS TERMINATED. INITIATE RECALL TO HOME BASE.", new Color(0x00, 0xFF, 0x00));
				string bossLogTex = "CalamityMod/NPCs/PlaguebringerGoliath/PlaguebringerGoliath_BossChecklist";
				AddBoss(bossChecklist, calamity, "Plaguebringer Goliath", order, type, DownedPBG, summon, loot, collection, instructions, despawn, bossLogTex);
			}

			// Ravager
			{
				BossDifficulty.TryGetValue("Ravager", out float order);
				List<int> segments = new List<int>() { NPCType<RavagerBody>(), NPCType<RavagerClawLeft>(), NPCType<RavagerClawRight>(), NPCType<RavagerHead>(), NPCType<RavagerLegLeft>(), NPCType<RavagerLegRight>() };
				int summon = ItemType<AncientMedallion>();
				List<int> loot = new List<int>() { ItemType<RavagerBag>(), ItemType<FleshyGeodeT1>(), ItemType<FleshyGeodeT2>(), ItemType<UltimusCleaver>(), ItemType<RealmRavager>(), ItemType<Hematemesis>(), ItemType<SpikecragStaff>(), ItemType<CraniumSmasher>(), ItemType<CorpusAvertor>(), ItemType<BloodPact>(), ItemType<FleshTotem>(), ItemType<BloodflareCore>(), ItemType<Vesuvius>(), ItemType<InfernalBlood>(), ItemID.GreaterHealingPotion };
				List<int> collection = new List<int>() { ItemType<RavagerTrophy>(), ItemType<RavagerMask>(), ItemType<KnowledgeRavager>() };
				string instructions = $"Use a [i:{summon}]";
				string despawn = CalamityUtils.ColorMessage("The automaton of misshapen victims went looking for the true perpetrator.", new Color(0xB2, 0x22, 0x22));
				string bossLogTex = "CalamityMod/NPCs/Ravager/Ravager_BossChecklist";
				AddBoss(bossChecklist, calamity, "Ravager", order, segments, DownedRavager, summon, loot, collection, instructions, despawn, bossLogTex);
			}

			// Astrum Deus
			{
				BossDifficulty.TryGetValue("AstrumDeus", out float order);
				List<int> segments = new List<int>() { NPCType<AstrumDeusHeadSpectral>(), NPCType<AstrumDeusBodySpectral>(), NPCType<AstrumDeusTailSpectral>() };
				int summon1 = ItemType<TitanHeart>();
				int summon2 = ItemType<Starcore>();
				int altar = ItemType<AstralBeaconItem>();
				List<int> summons = new List<int>() { summon1, summon2 };
				List<int> loot = new List<int>() { ItemType<AstrumDeusBag>(), ItemType<Stardust>(), ItemID.FallenStar, ItemType<TheMicrowave>(), ItemType<StarSputter>(), ItemType<Starfall>(), ItemType<GodspawnHelixStaff>(), ItemType<RegulusRiot>(), ItemType<HideofAstrumDeus>(), ItemID.FragmentSolar, ItemID.FragmentVortex, ItemID.FragmentNebula, ItemID.FragmentStardust, ItemID.GreaterHealingPotion };
				List<int> collection = new List<int>() { ItemType<AstrumDeusTrophy>(), ItemType<AstrumDeusMask>(), ItemType<KnowledgeAstrumDeus>(), ItemType<KnowledgeAstralInfection>(), ItemType<ChromaticOrb>() };
				string instructions = $"Use a [i:{summon1}] or [i:{summon2}] as offering at an [i:{altar}]";
				string despawn = CalamityUtils.ColorMessage("The infected deity retreats to the heavens.", new Color(0xFF, 0xD7, 0x00));
				string bossLogTex = "CalamityMod/NPCs/AstrumDeus/AstrumDeus_BossChecklist";
				AddBoss(bossChecklist, calamity, "Astrum Deus", order, segments, DownedDeus, summons, loot, collection, instructions, despawn, bossLogTex);
			}

			// Profaned Guardians
			{
				BossDifficulty.TryGetValue("ProfanedGuardians", out float order);
				int type = NPCType<ProfanedGuardianBoss>();
				int summon = ItemType<ProfanedShard>();
				List<int> loot = new List<int>() { ItemType<RelicOfResilience>(), ItemType<RelicOfConvergence>(), ItemType<RelicOfDeliverance>(), ItemType<SamuraiBadge>(), ItemType<ProfanedCoreUnlimited>(), ItemID.SuperHealingPotion };
				List<int> collection = new List<int>() { ItemType<ProfanedGuardianTrophy>(), ItemType<ProfanedGuardianMask>(), ItemType<KnowledgeProfanedGuardians>() };
				string instructions = $"Use a [i:{summon}] in the Hallow or Underworld Biomes";
				string despawn = CalamityUtils.ColorMessage("The guardians must protect their goddess at all costs.", new Color(0xFF, 0xA5, 0x00));
				string bossLogTex = "CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardians_BossChecklist";
				AddBoss(bossChecklist, calamity, "Profaned Guardians", order, type, DownedGuardians, summon, loot, collection, instructions, despawn, bossLogTex);
			}

			// Dragonfolly
			{
				BossDifficulty.TryGetValue("Dragonfolly", out float order);
				int type = NPCType<Bumblefuck>();
				int summon = ItemType<BirbPheromones>();
				List<int> loot = new List<int>() { ItemType<BumblebirbBag>(), ItemType<EffulgentFeather>(), ItemType<GildedProboscis>(), ItemType<GoldenEagle>(), ItemType<RougeSlash>(), ItemType<Swordsplosion>(), ItemType<BirdSeed>(), ItemType<DynamoStemCells>(), ItemType<RedLightningContainer>(), ItemType<SupremeHealingPotion>() };
				List<int> collection = new List<int>() { ItemType<BumblebirbTrophy>(), ItemType<BumblefuckMask>(), ItemType<KnowledgeBumblebirb>() };
				string instructions = $"Use [i:{summon}] in the Jungle Biome";
				string despawn = CalamityUtils.ColorMessage("The failed experiment returns to its reproductive routine.", new Color(0xFF, 0xD7, 0x00));
				AddBoss(bossChecklist, calamity, "Dragonfolly", order, type, DownedBirb, summon, loot, collection, instructions, despawn);
			}

			// Providence
			{
				BossDifficulty.TryGetValue("Providence", out float order);
				int type = NPCType<Providence>();
				int summon = ItemType<ProfanedCoreUnlimited>();
				List<int> loot = new List<int>() { ItemType<ProvidenceBag>(), ItemType<UnholyEssence>(), ItemType<DivineGeode>(), ItemType<HolyCollider>(), ItemType<SolarFlare>(), ItemType<TelluricGlare>(), ItemType<BlissfulBombardier>(), ItemType<PurgeGuzzler>(), ItemType<MoltenAmputator>(), ItemType<DazzlingStabberStaff>(), ItemType<PristineFury>(), ItemType<ElysianWings>(), ItemType<ElysianAegis>(), ItemType<BlazingCore>(), ItemType<RuneofCos>(), ItemType<ProfanedMoonlightDye>(), ItemType<SupremeHealingPotion>() };
				List<int> collection = new List<int>() { ItemType<ProvidenceTrophy>(), ItemType<ProvidenceMask>(), ItemType<KnowledgeProvidence>() };
				string instructions = $"Use [i:{summon}] in the Hallow or Underworld Biomes";
				string despawn = CalamityUtils.ColorMessage("The Profaned Goddess vanishes in a burning blaze.", new Color(0xFF, 0xA5, 0x00));
				string bossLogTex = "CalamityMod/NPCs/Providence/Providence_BossChecklist";
				AddBoss(bossChecklist, calamity, "Providence", order, type, DownedProvidence, summon, loot, collection, instructions, despawn, bossLogTex);
			}

			// Ceaseless Void
			{
				BossDifficulty.TryGetValue("CeaselessVoid", out float order);
				List<int> bosses = new List<int>() { NPCType<CeaselessVoid>(), NPCType<DarkEnergy>() };
				int summon = ItemType<RuneofCos>();
				List<int> loot = new List<int>() { ItemType<CeaselessVoidBag>(), ItemType<DarkPlasma>(), ItemType<MirrorBlade>(), ItemType<VoidConcentrationStaff>(), ItemType<TheEvolution>(), ItemType<SupremeHealingPotion>() };
				List<int> collection = new List<int>() { ItemType<CeaselessVoidTrophy>(), ItemType<CeaselessVoidMask>(), ItemType<AncientGodSlayerHelm>(), ItemType<AncientGodSlayerChestplate>(), ItemType<AncientGodSlayerLeggings>(), ItemType<KnowledgeSentinels>() };
				string instructions = $"Use a [i:{summon}] in the Dungeon";
				string despawn = CalamityUtils.ColorMessage("The rift in time and space has moved away from your reach.", new Color(0x4B, 0x00, 0x82));
				AddBoss(bossChecklist, calamity, "Ceaseless Void", order, bosses, DownedCeaselessVoid, summon, loot, collection, instructions, despawn);
			}

			// Storm Weaver
			{
				BossDifficulty.TryGetValue("StormWeaver", out float order);
				List<int> segments = new List<int>() { NPCType<StormWeaverHead>(), NPCType<StormWeaverBody>(), NPCType<StormWeaverTail>() };
				int summon = ItemType<RuneofCos>();
				List<int> loot = new List<int>() { ItemType<StormWeaverBag>(), ItemType<ArmoredShell>(), ItemType<TheStorm>(), ItemType<StormDragoon>(), ItemType<Thunderstorm>(), ItemType<SupremeHealingPotion>() };
				List<int> collection = new List<int>() { ItemType<WeaverTrophy>(), ItemType<StormWeaverMask>(), ItemType<AncientGodSlayerHelm>(), ItemType<AncientGodSlayerChestplate>(), ItemType<AncientGodSlayerLeggings>(), ItemType<KnowledgeSentinels>(), ItemType<LittleLight>() };
				string instructions = $"Use a [i:{summon}] in Space";
				string despawn = CalamityUtils.ColorMessage("Storm Weaver hid itself once again within the stormfront.", new Color(0xEE, 0x82, 0xEE));
				string bossLogTex = "CalamityMod/NPCs/StormWeaver/StormWeaver_BossChecklist";
				AddBoss(bossChecklist, calamity, "Storm Weaver", order, segments, DownedStormWeaver, summon, loot, collection, instructions, despawn, bossLogTex);
			}

			// Signus
			{
				BossDifficulty.TryGetValue("Signus", out float order);
				int type = NPCType<Signus>();
				int summon = ItemType<RuneofCos>();
				List<int> loot = new List<int>() { ItemType<SignusBag>(), ItemType<TwistingNether>(), ItemType<Cosmilamp>(), ItemType<CosmicKunai>(), ItemType<SpectralVeil>(), ItemType<SupremeHealingPotion>() };
				List<int> collection = new List<int>() { ItemType<SignusTrophy>(), ItemType<SignusMask>(), ItemType<AncientGodSlayerHelm>(), ItemType<AncientGodSlayerChestplate>(), ItemType<AncientGodSlayerLeggings>(), ItemType<KnowledgeSentinels>() };
				string instructions = $"Use a [i:{summon}] in the Underworld";
				string despawn = CalamityUtils.ColorMessage("The Devourer's assassin has finished its easy task.", new Color(0xBA, 0x55, 0xD3));
				AddBoss(bossChecklist, calamity, "Signus", order, type, DownedSignus, summon, loot, collection, instructions, despawn);
			}

			// Polterghast
			{
				BossDifficulty.TryGetValue("Polterghast", out float order);
				List<int> bosses = new List<int>() { NPCType<Polterghast>(), NPCType<PolterPhantom>() };
				int summon = ItemType<NecroplasmicBeacon>();
				List<int> loot = new List<int>() { ItemType<PolterghastBag>(), ItemType<RuinousSoul>(), ItemType<Phantoplasm>(), ItemType<TerrorBlade>(), ItemType<BansheeHook>(), ItemType<DaemonsFlame>(), ItemType<FatesReveal>(), ItemType<GhastlyVisage>(), ItemType<EtherealSubjugator>(), ItemType<GhoulishGouger>(), ItemType<Affliction>(), ItemType<Ectoheart>(), ItemType<SupremeHealingPotion>() };
				List<int> collection = new List<int>() { ItemType<PolterghastTrophy>(), ItemType<PolterghastMask>(), ItemType<KnowledgePolterghast>() };
				string instructions = $"Kill 30 phantom spirits or use a [i:{summon}] in the Dungeon";
				string despawn = CalamityUtils.ColorMessage("The volatile spirits disperse throughout the depths of the dungeon.", new Color(0xB0, 0xE0, 0xE6));
				AddBoss(bossChecklist, calamity, "Polterghast", order, bosses, DownedPolterghast, summon, loot, collection, instructions, despawn);
			}

			// Old Duke
			{
				BossDifficulty.TryGetValue("OldDuke", out float order);
				List<int> bosses = new List<int>() { NPCType<OldDuke>() };
				int summon = ItemType<BloodwormItem>();
				List<int> loot = new List<int>() { ItemType<OldDukeBag>(), ItemType<InsidiousImpaler>(), ItemType<SepticSkewer>(), ItemType<FetidEmesis>(), ItemType<VitriolicViper>(), ItemType<CadaverousCarrion>(), ItemType<ToxicantTwister>(), ItemType<DukeScales>(), ItemType<MutatedTruffle>(), ItemType<TheReaper>(), ItemType<SupremeHealingPotion>() };
				List<int> collection = new List<int>() { ItemType<OldDukeTrophy>(), ItemType<OldDukeMask>(), ItemType<KnowledgeOldDuke>() };
				string instructions = $"Defeat the Acid Rain event post-Polterghast or fish using a [i:{summon}] in the Sulphurous Sea";
				string despawn = CalamityUtils.ColorMessage("The old duke disappears amidst the acidic downpour.", new Color(0xF0, 0xE6, 0x8C));
				AddBoss(bossChecklist, calamity, "Old Duke", order, bosses, DownedBoomerDuke, summon, loot, collection, instructions, despawn);
			}

			// Devourer of Gods
			{
				BossDifficulty.TryGetValue("DevourerOfGods", out float order);
				int type = NPCType<DevourerofGodsHead>();
				int summon = ItemType<CosmicWorm>();
				List<int> loot = new List<int>() { ItemType<DevourerofGodsBag>(), ItemType<CosmiliteBar>(), ItemType<CosmiliteBrick>(), ItemType<Excelsus>(), ItemType<TheObliterator>(), ItemType<Deathwind>(), ItemType<DeathhailStaff>(), ItemType<StaffoftheMechworm>(), ItemType<Eradicator>(), ItemType<Norfleet>(), ItemType<CosmicDischarge>(), ItemType<NebulousCore>(), ItemType<Fabsol>(), ItemType<OmegaHealingPotion>() };
				List<int> collection = new List<int>() { ItemType<DevourerofGodsTrophy>(), ItemType<DevourerofGodsMask>(), ItemType<KnowledgeDevourerofGods>() };
				string instructions = $"Use a [i:{summon}]";
				string despawn = CalamityUtils.ColorMessage("The Devourer of Gods has slain everyone and feasted on their essence.", new Color(0x00, 0xFF, 0xFF));
				string bossLogTex = "CalamityMod/NPCs/DevourerofGods/DevourerofGods_BossChecklist";
				string bossHeadTex = "CalamityMod/NPCs/DevourerofGods/DevourerofGodsHead_Head_Boss";
				AddBoss(bossChecklist, calamity, "Devourer of Gods", order, type, DownedDoG, summon, loot, collection, instructions, despawn, bossLogTex, bossHeadTex);
			}

			// Yharon
			{
				BossDifficulty.TryGetValue("Yharon", out float order);
				int type = NPCType<Yharon>();
				int summon = ItemType<ChickenEgg>();
				List<int> loot = new List<int>() { ItemType<YharonBag>(), ItemType<HellcasterFragment>(), ItemType<DragonRage>(), ItemType<TheBurningSky>(), ItemType<DragonsBreath>(), ItemType<ChickenCannon>(), ItemType<PhoenixFlameBarrage>(), ItemType<AngryChickenStaff>(), ItemType<ProfanedTrident>(), ItemType<FinalDawn>(), ItemType<YharimsCrystal>(), ItemType<YharimsGift>(), ItemType<DrewsWings>(), /*ItemType<BossRush>(), */ItemType<OmegaHealingPotion>() };
				List<int> collection = new List<int>() { ItemType<YharonTrophy>(), ItemType<YharonMask>(), ItemType<KnowledgeYharon>(), ItemType<ForgottenDragonEgg>(), ItemType<McNuggets>(), ItemType<FoxDrive>() };
				string instructions = $"Use a [i:{summon}] in the Jungle Biome";
				string despawn = CalamityUtils.ColorMessage("Yharon found you too weak to stay near your gravestone.", new Color(0xFF, 0xA5, 0x00));
				string bossLogTex = "CalamityMod/NPCs/Yharon/Yharon_BossChecklist";
				AddBoss(bossChecklist, calamity, "Yharon", order, type, DownedYharon, summon, loot, collection, instructions, despawn, bossLogTex);
			}

			// Exo Mechs
			// Collection requires edits
			// Instructions require edits
			// Despawn requires edits
			{
				BossDifficulty.TryGetValue("ExoMechs", out float order);
				List<int> bosses = new List<int>() { NPCType<Apollo>(), NPCType<AresBody>(), NPCType<Artemis>(), NPCType<ThanatosHead>() };
				List<int> loot = new List<int>() { ItemType<DraedonTreasureBag>(), ItemType<ExoPrism>(), ItemType<SpineOfThanatos>(), ItemType<PhotonRipper>(), ItemType<SurgeDriver>(), ItemType<TheJailor>(), ItemType<RefractionRotor>(), ItemType<TheAtomSplitter>(), ItemType<DraedonsHeart>(), ItemType<ExoThrone>(), ItemType<OmegaHealingPotion>() };
				List<int> collection = new List<int>() { ItemType<AresTrophy>(), ItemType<ThanatosTrophy>(), ItemType<ArtemisTrophy>(), ItemType<ApolloTrophy>(), ItemType<DraedonMask>(), ItemType<AresMask>(), ItemType<ThanatosMask>(), ItemType<ArtemisMask>(), ItemType<ApolloMask>(), ItemType<KnowledgeExoMechs>() };
				string instructions = "By using a high-tech computer";
				string despawn = CalamityUtils.ColorMessage("An imperfection after all... what a shame.", new Color(0x7F, 0xFF, 0xD4));
				string bossLogTex = "CalamityMod/NPCs/ExoMechs/ExoMechs_BossChecklist";
				AddBoss(bossChecklist, calamity, "Exo Mechs", order, bosses, DownedExoMechs, null, loot, collection, instructions, despawn, bossLogTex);
			}

			// Supreme Calamitas
			{
				BossDifficulty.TryGetValue("SupremeCalamitas", out float order);
				int type = NPCType<SupremeCalamitas>();
				int summon1 = ItemType<CalamityDust>();
				int summon2 = ItemType<EyeofExtinction>();
				int altar = ItemType<SCalAltarItem>();
				List<int> summons = new List<int>() { summon1, summon2 };
				List<int> loot = new List<int>() { ItemType<SCalBag>(), ItemType<CalamitousEssence>(), ItemType<Vehemenc>(), ItemType<Heresy>(), ItemType<Perdition>(), ItemType<Vigilance>(), ItemType<Sacrifice>(), ItemType<Violence>(), ItemType<Condemnation>(), ItemType<GaelsGreatsword>(), ItemType<OmegaHealingPotion>() };
				List<int> collection = new List<int>() { ItemType<SupremeCalamitasTrophy>(), ItemType<SupremeCataclysmTrophy>(), ItemType<SupremeCatastropheTrophy>(), ItemType<AshenHorns>(), ItemType<SCalMask>(), ItemType<SCalRobes>(), ItemType<SCalBoots>(), ItemType<KnowledgeCalamitas>(), ItemType<BrimstoneJewel>(), ItemType<Levi>() };
				string instructions = $"Use [i:{summon1}] or a [i:{summon2}] as offering at an [i:{altar}]";
				string despawn = CalamityUtils.ColorMessage("Please don't waste my time.", new Color(0xFF, 0xA5, 0x00));
				string bossHeadTex = "CalamityMod/NPCs/SupremeCalamitas/HoodedHeadIcon";
				AddBoss(bossChecklist, calamity, "Supreme Calamitas", order, type, DownedSCal, summons, loot, collection, instructions, despawn, null, bossHeadTex);
			}

			// Adult Eidolon Wyrm
			{
				BossDifficulty.TryGetValue("AdultEidolonWyrm", out float order);
				int type = NPCType<EidolonWyrmHeadHuge>();
				int summon = ItemID.RodofDiscord;
				List<int> loot = new List<int>() { ItemType<Voidstone>(), ItemType<Lumenite>(), ItemID.Ectoplasm, ItemType<EidolicWail>(), ItemType<SoulEdge>(), ItemType<HalibutCannon>(), ItemType<OmegaHealingPotion>() };
				List<int> collection = new List<int>() { };
				string instructions = $"While in the Abyss, use an item that inflicts Chaos State";
				string despawn = CalamityUtils.ColorMessage("...", new Color(0x7F, 0xFF, 0xD4));
				string bossLogTex = "CalamityMod/NPCs/AdultEidolonWyrm/AdultEidolonWyrm_BossChecklist";
				AddBoss(bossChecklist, calamity, "Adult Eidolon Wyrm", order, type, DownedAdultEidolonWyrm, summon, loot, collection, instructions, despawn, bossLogTex);
			}
		}
		
		private static void AddCalamityInvasions(Mod bossChecklist, Mod calamity)
		{
			// Initial Acid Rain
			{
				InvasionDifficulty.TryGetValue("Acid Rain Initial", out float order);
				List<int> enemies = AcidRainEvent.PossibleEnemiesPreHM.Select(enemy => enemy.Key).ToList();
				int summon = ItemType<CausticTear>();
				List<int> loot = new List<int>() { ItemType<SulfuricScale>(), ItemType<ParasiticSceptor>(), ItemType<CausticCroakerStaff>() };
				List<int> collection = new List<int>() { ItemType<RadiatingCrystal>() };
				string instructions = $"Use a [i:{summon}] or wait for the invasion to occur naturally after the Eye of Cthulhu is defeated.";
				string despawn = CalamityUtils.ColorMessage("The mysterious creatures of the sulphuric sea descended back into the ocean.", new Color(146, 183, 116));
				string bossLogTex = "CalamityMod/Events/AcidRainT1_BossChecklist";
				string iconTexture = "CalamityMod/ExtraTextures/UI/AcidRainIcon";
				AddInvasion(bossChecklist, calamity, "Acid Rain", order, enemies, DownedAcidRainInitial, summon, loot, collection, instructions, despawn, bossLogTex, iconTexture);
			}
			// Post-Aquatic Scourge Acid Rain
			{
				InvasionDifficulty.TryGetValue("Acid Rain Aquatic Scourge", out float order);
				List<int> enemies = AcidRainEvent.PossibleEnemiesAS.Select(enemy => enemy.Key).ToList();
				enemies.Add(ModContent.NPCType<IrradiatedSlime>());
				enemies.AddRange(AcidRainEvent.PossibleMinibossesAS.Select(miniboss => miniboss.Key));
				List<int> summons = new List<int>() { ItemType<CausticTear>() };
				List<int> loot = new List<int>() { ItemType<SulfuricScale>(), ItemType<CorrodedFossil>(), ItemType<LeadCore>(), ItemType<NuclearRod>(), ItemType<ParasiticSceptor>(), ItemType<CausticCroakerStaff>(), ItemType<FlakToxicannon>(), ItemType<OrthoceraShell>(), ItemType<SkyfinBombers>(), ItemType<SlitheringEels>(), ItemType<SpentFuelContainer>(), ItemType<SulphurousGrabber>() };
				List<int> collection = new List<int>() { ItemType<RadiatingCrystal>() };
				string instructions = $"Use a [i:{ItemType<CausticTear>()}] or wait for the invasion to occur naturally after the Aquatic Scourge is defeated";
				string despawn = CalamityUtils.ColorMessage("The mysterious creatures of the sulphuric sea descended back into the deep ocean.", new Color(146, 183, 116));
				string bossLogTex = "CalamityMod/Events/AcidRainT2_BossChecklist";
				string iconTexture = "CalamityMod/ExtraTextures/UI/AcidRainIcon";
				AddInvasion(bossChecklist, calamity, "Acid Rain (Post-AS)", order, enemies, DownedAcidRainHardmode, summons, loot, collection, instructions, despawn, bossLogTex, iconTexture);
			}
			// Post-Polterghast Acid Rain
			{
				InvasionDifficulty.TryGetValue("Acid Rain Polterghast", out float order);
				List<int> enemies = AcidRainEvent.PossibleEnemiesPolter.Select(enemy => enemy.Key).ToList();
				enemies.AddRange(AcidRainEvent.PossibleMinibossesPolter.Select(miniboss => miniboss.Key));
				List<int> summons = new List<int>() { ItemType<CausticTear>() };
				List<int> loot = new List<int>() { ItemID.SharkFin, ItemType<SulfuricScale>(), ItemType<CorrodedFossil>(), ItemType<LeadCore>(), ItemType<NuclearRod>(), ItemType<ParasiticSceptor>(), ItemType<CausticCroakerStaff>(), ItemType<FlakToxicannon>(), ItemType<OrthoceraShell>(), ItemType<SkyfinBombers>(), ItemType<SlitheringEels>(), ItemType<SpentFuelContainer>(), ItemType<SulphurousGrabber>(), ItemType<GammaHeart>(), ItemType<PhosphorescentGauntlet>(), ItemType<SulphuricAcidCannon>() };
				List<int> collection = new List<int>() { ItemType<RadiatingCrystal>() };
				string instructions = $"Use a [i:{ItemType<CausticTear>()}] or wait for the invasion to occur naturally after the Polterghast is defeated";
				string despawn = CalamityUtils.ColorMessage("The mysterious creatures of the sulphuric sea descended back into the deep ocean.", new Color(146, 183, 116));
				string bossLogTex = "CalamityMod/Events/AcidRainT3_BossChecklist";
				string iconTexture = "CalamityMod/ExtraTextures/UI/AcidRainIcon";
				AddInvasion(bossChecklist, calamity, "Acid Rain (Post-Polter)", order, enemies, DownedBoomerDuke, summons, loot, collection, instructions, despawn, bossLogTex, iconTexture);
			}
		}

		private static void AddCalamityBossLoot(Mod bossChecklist)
		{
			// King Slime
			AddLoot(bossChecklist, "KingSlime",
				new List<int>() { ItemType<CrownJewel>() },
				new List<int>() { ItemType<KnowledgeKingSlime>() }
			);

			// Eye of Cthulhu
			AddLoot(bossChecklist, "EyeofCthulhu",
				new List<int>() { ItemType<DeathstareRod>(), ItemType<TeardropCleaver>() },
				new List<int>() { ItemType<KnowledgeEyeofCthulhu>() }
			);

			// Eater of Worlds
			AddLoot(bossChecklist, "EaterofWorldsHead",
				null,
				new List<int>() { ItemType<KnowledgeEaterofWorlds>(), ItemType<KnowledgeCorruption>() }
			);

			// Brain of Cthulhu
			AddLoot(bossChecklist, "BrainofCthulhu",
				null,
				new List<int>() { ItemType<KnowledgeBrainofCthulhu>(), ItemType<KnowledgeCrimson>() }
			);

			// Queen Bee
			AddLoot(bossChecklist, "QueenBee",
				new List<int>() { ItemType<HardenedHoneycomb>(), ItemID.Stinger, ItemType<TheBee>() },
				new List<int>() { ItemType<KnowledgeQueenBee>() }
			);

			// Skeletron
			AddLoot(bossChecklist, "SkeletronHead",
				null,
				new List<int>() { ItemType<KnowledgeSkeletron>() }
			);

			// Wall of Flesh
			AddLoot(bossChecklist, "WallofFlesh",
				new List<int>() { ItemType<Meowthrower>(), ItemType<BlackHawkRemote>(), ItemType<BlastBarrel>(), ItemType<RogueEmblem>(), ItemType<Carnage>(), ItemID.CorruptionKey, ItemID.CrimsonKey },
				new List<int>() { ItemType<KnowledgeWallofFlesh>(), ItemType<KnowledgeUnderworld>(), ItemType<IbarakiBox>() }
			);

			// The Twins
			AddLoot(bossChecklist, "TheTwins",
				new List<int>() { ItemType<Arbalest>() },
				new List<int>() { ItemType<KnowledgeTwins>(), ItemType<KnowledgeMechs>() }
			);

			// The Destroyer
			AddLoot(bossChecklist, "TheDestroyer",
				new List<int>() { ItemType<KnowledgeDestroyer>(), ItemType<KnowledgeMechs>() }
			);

			// Skeletron Prime
			AddLoot(bossChecklist, "SkeletronPrime",
				null,
				new List<int>() { ItemType<KnowledgeSkeletronPrime>(), ItemType<KnowledgeMechs>() }
			);

			// Plantera
			AddLoot(bossChecklist, "Plantera",
				new List<int>() { ItemType<LivingShard>(), ItemType<BlossomFlux>(), ItemType<BloomStone>(), ItemID.JungleKey },
				new List<int>() { ItemType<KnowledgePlantera>() }
			);
			AddSummons(bossChecklist, "Plantera", new List<int>() { ItemType<BulbofDoom>() });

			// Golem
			AddLoot(bossChecklist, "Golem",
				new List<int>() { ItemType<EssenceofCinder>(), ItemType<AegisBlade>() },
				new List<int>() { ItemType<KnowledgeGolem>() }
			);
			AddSummons(bossChecklist, "Golem", new List<int>() { ItemType<OldPowerCell>() });

			// Duke Fishron
			AddLoot(bossChecklist, "DukeFishron",
				new List<int>() { ItemType<DukesDecapitator>(), ItemType<BrinyBaron>() },
				new List<int>() { ItemType<KnowledgeDukeFishron>() }
			);

			// Betsy
			AddLoot(bossChecklist, "DD2Betsy",
				null,
				null
			);

			// Lunatic Cultist
			AddLoot(bossChecklist, "CultistBoss",
				null,
				new List<int>() { ItemType<KnowledgeLunaticCultist>(), ItemType<KnowledgeBloodMoon>() }
			);
			AddSummons(bossChecklist, "CultistBoss", new List<int>() { ItemType<EidolonTablet>() });

			// Moon Lord
			AddLoot(bossChecklist, "MoonLord",
				new List<int>() { ItemType<UtensilPoker>(), ItemType<MLGRune2>() },
				new List<int>() { ItemType<KnowledgeMoonLord>() }
			);
		}

		private static void AddCalamityEventLoot(Mod bossChecklist)
		{
			// Blood Moon
			AddLoot(bossChecklist, "Blood Moon",
				new List<int>() { ItemType<BloodOrb>(), ItemType<BouncingEyeball>() },
				null
			);
			AddSummons(bossChecklist, "Blood Moon", new List<int>() { ItemType<BloodIdol>() });

			// Goblin Army
			AddLoot(bossChecklist, "Goblin Army",
				new List<int>() { ItemType<PlasmaRod>(), ItemType<Warblade>(), ItemType<TheFirstShadowflame>(), ItemType<BurningStrife>() },
				null
			);

			// Pirates
			AddLoot(bossChecklist, "Pirate Invasion",
				new List<int>() { ItemType<RaidersGlory>(), ItemType<ProporsePistol>() },
				null
			);

			// Solar Eclipse
			AddLoot(bossChecklist, "Solar Eclipse",
				new List<int>() { ItemType<SolarVeil>(), ItemType<DefectiveSphere>(), ItemType<DarksunFragment>() },
				null
			);

			// Pumpkin Moon
			AddLoot(bossChecklist, "Pumpkin Moon",
				new List<int>() { ItemType<NightmareFuel>() },
				null
			);
			AddLoot(bossChecklist, "Pumpking",
				new List<int>() { ItemType<NightmareFuel>() },
				null
			);

			// Frost Moon
			AddLoot(bossChecklist, "Frost Moon",
				new List<int>() { ItemType<EndothermicEnergy>() },
				null
			);
			AddLoot(bossChecklist, "Ice Queen",
				new List<int>() { ItemType<EndothermicEnergy>() },
				null
			);

			// Martian Madness
			AddLoot(bossChecklist, "Martian Madness",
				new List<int>() { ItemType<Wingman>(), ItemType<ShockGrenade>(), ItemType<NullificationRifle>() },
				null
			);
			AddLoot(bossChecklist, "Martian Saucer",
				new List<int>() { ItemType<NullificationRifle>() },
				null
			);

			// Lunar Events
			AddLoot(bossChecklist, "Lunar Event",
				new List<int>() { ItemType<MeldBlob>() },
				null
			);
		}

		private static void FargosSupport()
		{
			Mod fargos = GetInstance<CalamityMod>().fargos;
			if (fargos is null)
				return;

			void AddToMutantShop(string bossName, string summonItemName, Func<bool> downed, int price)
			{
				BossDifficulty.TryGetValue(bossName, out float order);
				fargos.Call("AddSummon", order, "CalamityMod", summonItemName, downed, price);
			}

			fargos.Call("AbominationnClearEvents", "CalamityMod", CalamityWorld.rainingAcid, true);

			AddToMutantShop("OldDuke", "BloodwormItem", DownedBoomerDuke, Item.buyPrice(platinum: 2));
		}

		private static void CensusSupport()
		{
			Mod censusMod = GetInstance<CalamityMod>().census;
			if (censusMod != null)
			{
				censusMod.Call("TownNPCCondition", NPCType<SEAHOE>(), "Defeat a Giant Clam after defeating the Desert Scourge");
				censusMod.Call("TownNPCCondition", NPCType<THIEF>(), "Have a [i:" + ItemID.PlatinumCoin + "] in your inventory after defeating Skeletron");
				censusMod.Call("TownNPCCondition", NPCType<FAP>(), "Have [i:" + ItemType<FabsolsVodka>() + "] in your inventory in Hardmode");
				censusMod.Call("TownNPCCondition", NPCType<DILF>(), "Defeat Cryogen");
				censusMod.Call("TownNPCCondition", NPCType<WITCH>(), "Defeat Supreme Calamitas");
			}
		}

		private static void SummonersAssociationSupport()
		{
			Mod sAssociation = GetInstance<CalamityMod>().summonersAssociation;
			if (sAssociation is null)
				return;

			void RegisterSummon(int summonItem, int summonBuff, int summonProjectile)
			{
				sAssociation.Call("AddMinionInfo", summonItem, summonBuff, summonProjectile);
			}
			RegisterSummon(ItemType<WulfrumController>(), BuffType<WulfrumDroidBuff>(), ProjectileType<WulfrumDroid>());
			RegisterSummon(ItemType<SunSpiritStaff>(), BuffType<SolarSpirit>(), ProjectileType<SolarPixie>());
			RegisterSummon(ItemType<FrostBlossomStaff>(), BuffType<FrostBlossomBuff>(), ProjectileType<FrostBlossom>());
			RegisterSummon(ItemType<BelladonnaSpiritStaff>(), BuffType<BelladonnaSpiritBuff>(), ProjectileType<BelladonnaSpirit>());
			RegisterSummon(ItemType<StormjawStaff>(), BuffType<StormjawBuff>(), ProjectileType<StormjawBaby>());
			RegisterSummon(ItemType<RustyBeaconPrototype>(), BuffType<RustyDroneBuff>(), ProjectileType<RustyDrone>());
			RegisterSummon(ItemType<SeaboundStaff>(), BuffType<BrittleStar>(), ProjectileType<BrittleStarMinion>());
			RegisterSummon(ItemType<MagicalConch>(), BuffType<HermitCrab>(), ProjectileType<HermitCrabMinion>());
			RegisterSummon(ItemType<DeathstareRod>(), BuffType<DeathstareBuff>(), ProjectileType<DeathstareEyeball>());
			RegisterSummon(ItemType<VileFeeder>(), BuffType<VileFeederBuff>(), ProjectileType<VileFeederSummon>());
			RegisterSummon(ItemType<ScabRipper>(), BuffType<ScabRipperBuff>(), ProjectileType<BabyBloodCrawler>());
			RegisterSummon(ItemType<CinderBlossomStaff>(), BuffType<CinderBlossomBuff>(), ProjectileType<CinderBlossom>());
			RegisterSummon(ItemType<BloodClotStaff>(), BuffType<BloodClot>(), ProjectileType<BloodClotMinion>());
			RegisterSummon(ItemType<DankStaff>(), BuffType<DankCreeperBuff>(), ProjectileType<DankCreeperMinion>());
			RegisterSummon(ItemType<StarSwallowerContainmentUnit>(), BuffType<StarSwallowerBuff>(), ProjectileType<StarSwallowerSummon>());
			RegisterSummon(ItemType<HerringStaff>(), BuffType<Herring>(), ProjectileType<HerringMinion>());
			RegisterSummon(ItemType<EyeOfNight>(), BuffType<EyeOfNightBuff>(), ProjectileType<EyeOfNightSummon>());
			RegisterSummon(ItemType<FleshOfInfidelity>(), BuffType<FleshOfInfidelityBuff>(), ProjectileType<FleshBallMinion>());
			RegisterSummon(ItemType<CorroslimeStaff>(), BuffType<Corroslime>(), ProjectileType<CorroslimeMinion>());
			RegisterSummon(ItemType<CrimslimeStaff>(), BuffType<Crimslime>(), ProjectileType<CrimslimeMinion>());
			RegisterSummon(ItemType<BlackHawkRemote>(), BuffType<BlackHawkBuff>(), ProjectileType<BlackHawkSummon>());
			RegisterSummon(ItemType<CausticStaff>(), BuffType<CausticStaffBuff>(), ProjectileType<CausticStaffSummon>());
			RegisterSummon(ItemType<AncientIceChunk>(), BuffType<IceClasperBuff>(), ProjectileType<IceClasperMinion>());
			RegisterSummon(ItemType<ShellfishStaff>(), BuffType<ShellfishBuff>(), ProjectileType<Shellfish>());
			RegisterSummon(ItemType<HauntedScroll>(), BuffType<HauntedDishesBuff>(), ProjectileType<HauntedDishes>());
			RegisterSummon(ItemType<ForgottenApexWand>(), BuffType<ApexSharkBuff>(), ProjectileType<ApexShark>());
			RegisterSummon(ItemType<DaedalusGolemStaff>(), BuffType<DaedalusGolemBuff>(), ProjectileType<DaedalusGolem>());
			RegisterSummon(ItemType<ColdDivinity>(), BuffType<ColdDivinityBuff>(), ProjectileType<ColdDivinityPointyThing>());
			RegisterSummon(ItemType<MountedScanner>(), BuffType<MountedScannerBuff>(), ProjectileType<MountedScannerSummon>());
			RegisterSummon(ItemType<DeepseaStaff>(), BuffType<AquaticStar>(), ProjectileType<AquaticStarMinion>());
			RegisterSummon(ItemType<SunGodStaff>(), BuffType<SolarSpiritGod>(), ProjectileType<SolarGod>());
			RegisterSummon(ItemType<TundraFlameBlossomsStaff>(), BuffType<TundraFlameBlossomsBuff>(), ProjectileType<TundraFlameBlossom>());
			RegisterSummon(ItemType<DormantBrimseeker>(), BuffType<DormantBrimseekerBuff>(), ProjectileType<DormantBrimseekerBab>());
			RegisterSummon(ItemType<IgneousExaltation>(), BuffType<IgneousExaltationBuff>(), ProjectileType<IgneousBlade>());
			RegisterSummon(ItemType<PlantationStaff>(), BuffType<PlantationBuff>(), ProjectileType<PlantSummon>());
			RegisterSummon(ItemType<ViralSprout>(), BuffType<SageSpiritBuff>(), ProjectileType<SageSpirit>());
			RegisterSummon(ItemType<SandSharknadoStaff>(), BuffType<Sandnado>(), ProjectileType<SandnadoMinion>());
			RegisterSummon(ItemType<GastricBelcherStaff>(), BuffType<GastricBelcherBuff>(), ProjectileType<GastricBelcher>());
			RegisterSummon(ItemType<FuelCellBundle>(), BuffType<FuelCellBundleBuff>(), ProjectileType<PlaguebringerMK2>());
			RegisterSummon(ItemType<WitherBlossomsStaff>(), BuffType<WitherBlossomsBuff>(), ProjectileType<WitherBlossom>());
			RegisterSummon(ItemType<GodspawnHelixStaff>(), BuffType<AstralProbeBuff>(), ProjectileType<AstralProbeSummon>());
			RegisterSummon(ItemType<TacticalPlagueEngine>(), BuffType<TacticalPlagueEngineBuff>(), ProjectileType<TacticalPlagueJet>());
			RegisterSummon(ItemType<ElementalAxe>(), BuffType<ElementalAxeBuff>(), ProjectileType<ElementalAxeMinion>());
			RegisterSummon(ItemType<FlowersOfMortality>(), BuffType<FlowersOfMortalityBuff>(), ProjectileType<FlowersOfMortalityPetal>());
			RegisterSummon(ItemType<SnakeEyes>(), BuffType<SnakeEyesBuff>(), ProjectileType<SnakeEyesSummon>());
			RegisterSummon(ItemType<DazzlingStabberStaff>(), BuffType<DazzlingStabberBuff>(), ProjectileType<DazzlingStabber>());
			RegisterSummon(ItemType<ViridVanguard>(), BuffType<ViridVanguardBuff>(), ProjectileType<ViridVanguardBlade>());
			RegisterSummon(ItemType<DragonbloodDisgorger>(), BuffType<BloodDragonsBuff>(), ProjectileType<SkeletalDragonMother>());
			RegisterSummon(ItemType<Cosmilamp>(), BuffType<CosmilampBuff>(), ProjectileType<CosmilampMinion>());
			RegisterSummon(ItemType<VoidConcentrationStaff>(), BuffType<VoidConcentrationBuff>(), ProjectileType<VoidConcentrationAura>());
			RegisterSummon(ItemType<EtherealSubjugator>(), BuffType<Phantom>(), ProjectileType<PhantomGuy>());
			RegisterSummon(ItemType<CalamarisLament>(), BuffType<Calamari>(), ProjectileType<CalamariMinion>());
			RegisterSummon(ItemType<GammaHeart>(), BuffType<GammaHeadBuff>(), ProjectileType<GammaHead>());
			RegisterSummon(ItemType<StaffoftheMechworm>(), BuffType<Mechworm>(), ProjectileType<MechwormBody>());
			RegisterSummon(ItemType<CorvidHarbringerStaff>(), BuffType<CorvidHarbringerBuff>(), ProjectileType<PowerfulRaven>());
			RegisterSummon(ItemType<EndoHydraStaff>(), BuffType<EndoHydraBuff>(), ProjectileType<EndoHydraHead>());
			RegisterSummon(ItemType<CosmicViperEngine>(), BuffType<CosmicViperEngineBuff>(), ProjectileType<CosmicViperSummon>());
			RegisterSummon(ItemType<AngryChickenStaff>(), BuffType<YharonKindleBuff>(), ProjectileType<SonOfYharon>());
			RegisterSummon(ItemType<MidnightSunBeacon>(), BuffType<MidnightSunBuff>(), ProjectileType<MidnightSunUFO>());
			RegisterSummon(ItemType<PoleWarper>(), BuffType<PoleWarperBuff>(), ProjectileType<PoleWarperSummon>());
			RegisterSummon(ItemType<Vigilance>(), BuffType<SoulSeekerBuff>(), ProjectileType<SeekerSummonProj>());
			RegisterSummon(ItemType<Metastasis>(), BuffType<SepulcherMinionBuff>(), ProjectileType<SepulcherMinion>());
			RegisterSummon(ItemType<CosmicImmaterializer>(), BuffType<CosmicEnergy>(), ProjectileType<CosmicEnergySpiral>());
			RegisterSummon(ItemType<BensUmbrella>(), BuffType<MagicHatBuff>(), ProjectileType<MagicHat>());
			RegisterSummon(ItemType<Endogenesis>(), BuffType<EndoCooperBuff>(), ProjectileType<EndoCooperBody>());

			sAssociation.Call("AddMinionInfo", ItemType<BlightedEyeStaff>(), BuffType<CalamitasEyes>(), new List<int>() { ProjectileType<Calamitamini>(), ProjectileType<Cataclymini>(), ProjectileType<Catastromini>()}, new List<float>() {1-(1f/3f), 2f/3f, 2f/3f});
			//Entropy's Vigil is a bruh moment
			sAssociation.Call("AddMinionInfo", ItemType<ResurrectionButterfly>(), BuffType<ResurrectionButterflyBuff>(), new List<int>() { ProjectileType<PinkButterfly>(), ProjectileType<PurpleButterfly>()});
		}
	}
}
