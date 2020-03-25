using CalamityMod.Buffs.Summon;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.DifficultyItems;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Furniture.CraftingStations;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Placeables.FurnitureCosmilite;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.SummonItems.Invasion;
using CalamityMod.Items.Tools;
using CalamityMod.Items.Tools.ClimateChange;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.AcidRain;
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
using CalamityMod.Projectiles.Summon;
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
			{ "BrimstoneElemental", 7.5f },
			{ "AquaticScourge", 8.5f },
			{ "Calamitas", 9.7f },
			{ "GreatSandShark", 10.09f },
			{ "Leviathan", 10.5f },
			{ "AstrumAureus", 10.6f },
			{ "PlaguebringerGoliath", 11.5f },
			{ "Ravager", 12.5f },
			{ "AstrumDeus", 13.5f },
			{ "ProfanedGuardians", 14.5f },
			{ "Bumblebirb", 14.6f },
			{ "Providence", 15.01f }, // Thorium's Ragnarok is 15f
			{ "CeaselessVoid", 15.5f },
			{ "StormWeaver", 15.51f },
			{ "Signus", 15.52f },
			{ "Polterghast", 16f },
			{ "OldDuke", 16.5f },
			{ "DevourerOfGods", 17f },
			{ "Yharon", 18f },
			// { "Draedon", 18.5f },
			{ "SupremeCalamitas", 19f },
			// { "Yharim", 20f },
			// { "Noxus", 120f },
			// { "Xeroc", 121f },
		};

		private static readonly Dictionary<string, float> InvasionDifficulty = new Dictionary<string, float>
		{
			{ "Acid Rain Initial", 2.4f },
			{ "Acid Rain Aquatic Scourge", 8.51f },
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
			Mod bossChecklist = ModLoader.GetMod("BossChecklist");
			Mod calamity = GetInstance<CalamityMod>();

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
				List<int> loot = new List<int>() { ItemType<DesertScourgeBag>(), ItemID.SandBlock, ItemType<VictoryShard>(), ItemID.Coral, ItemID.Seashell, ItemID.Starfish, ItemType<AquaticDischarge>(), ItemType<Barinade>(), ItemType<StormSpray>(), ItemType<SeaboundStaff>(), ItemType<ScourgeoftheDesert>(), ItemType<DuneHopper>(), ItemType<AeroStone>(), ItemType<SandCloak>(), ItemType<DeepDiver>(), ItemType<OceanCrest>(), ItemID.AnglerTackleBag, ItemID.HighTestFishingLine, ItemID.TackleBox, ItemID.AnglerEarring, ItemID.FishermansGuide, ItemID.WeatherRadio, ItemID.Sextant, ItemID.AnglerHat, ItemID.AnglerVest, ItemID.AnglerPants, ItemID.FishingPotion, ItemID.SonarPotion, ItemID.CratePotion, ItemID.GoldenBugNet, ItemID.LesserHealingPotion };
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
				List<int> phases = new List<int>() { NPCType<HiveMind>(), NPCType<HiveMindP2>() };
				int summon = ItemType<Teratoma>();
				List<int> loot = new List<int>() { ItemType<HiveMindBag>(), ItemType<TrueShadowScale>(), ItemID.DemoniteBar, ItemID.RottenChunk, ItemID.CursedFlame, ItemType<PerfectDark>(), ItemType<LeechingDagger>(), ItemType<Shadethrower>(), ItemType<ShadowdropStaff>(), ItemType<ShaderainStaff>(), ItemType<DankStaff>(), ItemType<RotBall>(), ItemType<FilthyGlove>(), ItemType<RottenBrain>(), ItemID.LesserHealingPotion };
				List<int> collection = new List<int>() { ItemType<HiveMindTrophy>(), ItemType<HiveMindMask>(), ItemType<KnowledgeHiveMind>() };
				string instructions = $"Kill a Cyst in the Corruption or use a [i:{summon}] in the Corruption";
				string despawn = CalamityUtils.ColorMessage("The corrupted colony began searching for a new breeding ground.", new Color(0x94, 0x00, 0xD3));
				AddBoss(bossChecklist, calamity, "Hive Mind", order, phases, DownedHiveMind, summon, loot, collection, instructions, despawn);
			}

			// Perforators
			{
				BossDifficulty.TryGetValue("Perforators", out float order);
				int type = NPCType<PerforatorHive>();
				int summon = ItemType<BloodyWormFood>();
				List<int> loot = new List<int>() { ItemType<PerforatorBag>(), ItemType<BloodSample>(), ItemID.CrimtaneBar, ItemID.Vertebrae, ItemID.Ichor, ItemType<VeinBurster>(), ItemType<BloodyRupture>(), ItemType<SausageMaker>(), ItemType<Aorta>(), ItemType<Eviscerator>(), ItemType<BloodBath>(), ItemType<BloodClotStaff>(), ItemType<ToothBall>(), ItemType<BloodstainedGlove>(), ItemType<BloodyWormTooth>(), ItemID.LesserHealingPotion };
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
				List<int> loot = new List<int>() { ItemType<SlimeGodBag>(), ItemID.Gel, ItemType<PurifiedGel>(), ItemType<OverloadedBlaster>(), ItemType<AbyssalTome>(), ItemType<EldritchTome>(), ItemType<CorroslimeStaff>(), ItemType<CrimslimeStaff>(), ItemType<GelDart>(), ItemType<ManaOverloader>(), ItemType<ElectrolyteGelPack>(), ItemType<PurifiedJam>(), ItemID.HealingPotion };
				List<int> collection = new List<int>() { ItemType<SlimeGodTrophy>(), ItemType<SlimeGodMask>(), ItemType<SlimeGodMask2>(), ItemType<KnowledgeSlimeGod>(), ItemType<StaticRefiner>() };
				string instructions = $"Use an [i:{summon}]";
				string despawn = CalamityUtils.ColorMessage("The gelatinous monstrosity achieved vengeance for its breathren.", new Color(0xBA, 0x55, 0x33));
				AddBoss(bossChecklist, calamity, "Slime God", order, bosses, DownedSlimeGod, summon, loot, collection, instructions, despawn);
			}

			// Cryogen
			{
				BossDifficulty.TryGetValue("Cryogen", out float order);
				int type = NPCType<Cryogen>();
				int summon = ItemType<CryoKey>();
				List<int> loot = new List<int>() { ItemType<CryogenBag>(), ItemID.SoulofMight, ItemType<CryoBar>(), ItemType<EssenceofEleum>(), ItemID.FrostCore, ItemType<Avalanche>(), ItemType<GlacialCrusher>(), ItemType<EffluviumBow>(), ItemType<BittercoldStaff>(), ItemType<SnowstormStaff>(), ItemType<Icebreaker>(), ItemType<IceStar>(), ItemType<CryoStone>(), ItemType<Regenator>(), ItemType<SoulofCryogen>(), ItemType<FrostFlare>(), ItemID.FrozenKey, ItemID.GreaterHealingPotion };
				List<int> collection = new List<int>() { ItemType<CryogenTrophy>(), ItemType<CryogenMask>(), ItemType<KnowledgeCryogen>() };
				string instructions = $"Use a [i:{summon}] in the Snow Biome";
				string despawn = CalamityUtils.ColorMessage("Cryogen drifts away, carried on a freezing wind.", new Color(0x00, 0xFF, 0xFF));
				string bossLogTex = "CalamityMod/NPCs/Cryogen/Cryogen_BossChecklist";
				AddBoss(bossChecklist, calamity, "Cryogen", order, type, DownedCryogen, summon, loot, collection, instructions, despawn, bossLogTex);
			}

			// Brimstone Elemental
			{
				BossDifficulty.TryGetValue("BrimstoneElemental", out float order);
				int type = NPCType<BrimstoneElemental>();
				int summon = ItemType<CharredIdol>();
				List<int> loot = new List<int>() { ItemType<BrimstoneWaifuBag>(), ItemID.SoulofFright, ItemType<EssenceofChaos>(), ItemType<Bloodstone>(), ItemType<Brimlance>(), ItemType<DormantBrimseeker>(), ItemType<SeethingDischarge>(), ItemType<Abaddon>(), ItemType<RoseStone>(), ItemType<Gehenna>(), ItemType<Brimrose>(), ItemID.GreaterHealingPotion };
				List<int> collection = new List<int>() { ItemType<BrimstoneElementalTrophy>(), ItemType<BrimstoneWaifuMask>(), ItemType<KnowledgeBrimstoneCrag>(), ItemType<KnowledgeBrimstoneElemental>(), ItemType<CharredRelic>() };
				string instructions = $"Use a [i:{summon}] in the Brimstone Crag";
				string despawn = CalamityUtils.ColorMessage("Brimstone Elemental withdraws to the ruins of her shrine.", new Color(0xDC, 0x14, 0x3C));
				AddBoss(bossChecklist, calamity, "Brimstone Elemental", order, type, DownedBrimstoneElemental, summon, loot, collection, instructions, despawn);
			}

			// Aquatic Scourge
			{
				BossDifficulty.TryGetValue("AquaticScourge", out float order);
				List<int> segments = new List<int>() { NPCType<AquaticScourgeHead>(), NPCType<AquaticScourgeBody>(), NPCType<AquaticScourgeBodyAlt>(), NPCType<AquaticScourgeTail>() };
				int summon = ItemType<Seafood>();
				List<int> loot = new List<int>() { ItemType<AquaticScourgeBag>(), ItemType<SulphurousSand>(), ItemID.SoulofSight, ItemType<VictoryShard>(), ItemID.Coral, ItemID.Seashell, ItemID.Starfish, ItemType<SubmarineShocker>(), ItemType<Barinautical>(), ItemType<Downpour>(), ItemType<DeepseaStaff>(), ItemType<ScourgeoftheSeas>(), ItemType<SeasSearing>(), ItemType<AeroStone>(), ItemType<AquaticEmblem>(), ItemType<CorrosiveSpine>(), ItemID.AnglerTackleBag, ItemID.HighTestFishingLine, ItemID.TackleBox, ItemID.AnglerEarring, ItemID.FishermansGuide, ItemID.WeatherRadio, ItemID.Sextant, ItemID.AnglerHat, ItemID.AnglerVest, ItemID.AnglerPants, ItemID.FishingPotion, ItemID.SonarPotion, ItemID.CratePotion, ItemID.GoldenBugNet, ItemID.GreaterHealingPotion };
				List<int> collection = new List<int>() { ItemType<AquaticScourgeTrophy>(), ItemType<AquaticScourgeMask>(), ItemType<KnowledgeAquaticScourge>(), ItemType<KnowledgeSulphurSea>() };
				string instructions = $"Use a [i:{summon}] in the Sulphuric Sea or wait for it to spawn in the Sulphuric Sea";
				string despawn = CalamityUtils.ColorMessage("The Aquatic Scourge swam back into the open ocean.", new Color(0xF0, 0xE6, 0x8C));
				string bossLogTex = "CalamityMod/NPCs/AquaticScourge/AquaticScourge_BossChecklist";
				AddBoss(bossChecklist, calamity, "Aquatic Scourge", order, segments, DownedAquaticScourge, summon, loot, collection, instructions, despawn, bossLogTex);
			}

			// Calamitas
			{
				BossDifficulty.TryGetValue("Calamitas", out float order);
				int type = NPCType<CalamitasRun3>();
				int summon = ItemType<BlightedEyeball>();
				List<int> loot = new List<int>() { ItemType<CalamitasBag>(), ItemType<EssenceofChaos>(), ItemType<CalamityDust>(), ItemType<BlightedLens>(), ItemType<Bloodstone>(), ItemType<CalamitasInferno>(), ItemType<TheEyeofCalamitas>(), ItemType<BlightedEyeStaff>(), ItemType<Animosity>(), ItemType<BrimstoneFlamesprayer>(), ItemType<BrimstoneFlameblaster>(), ItemType<CrushsawCrasher>(), ItemType<ChaosStone>(), ItemType<CalamityRing>(), ItemID.BrokenHeroSword, ItemID.GreaterHealingPotion };
				List<int> collection = new List<int>() { ItemType<CalamitasTrophy>(), ItemType<CataclysmTrophy>(), ItemType<CatastropheTrophy>(), ItemType<KnowledgeCalamitasClone>() };
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
				List<int> loot = new List<int>() { ItemType<LeviathanBag>(), ItemType<Greentide>(), ItemType<Leviatitan>(), ItemType<SirensSong>(), ItemType<Atlantis>(), ItemType<BrackishFlask>(), ItemType<LeviathanTeeth>(), ItemType<LureofEnthrallment>(), ItemType<LeviathanAmbergris>(), ItemType<TheCommunity>(), ItemID.HotlineFishingHook, ItemID.BottomlessBucket, ItemID.SuperAbsorbantSponge, ItemID.FishingPotion, ItemID.SonarPotion, ItemID.CratePotion, ItemID.GreaterHealingPotion };
				List<int> collection = new List<int>() { ItemType<LeviathanTrophy>(), ItemType<LeviathanMask>(), ItemType<KnowledgeOcean>(), ItemType<KnowledgeLeviathanandSiren>() };
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
				List<int> loot = new List<int>() { ItemType<AstrageldonBag>(), ItemType<Stardust>(), ItemID.FallenStar, ItemType<Nebulash>(), ItemType<BorealisBomber>(), ItemType<GravistarSabaton>(), ItemType<AstralJelly>(), ItemID.HallowedKey, ItemID.FragmentSolar, ItemID.FragmentVortex, ItemID.FragmentNebula, ItemID.FragmentStardust, ItemType<StarlightFuelCell>(), ItemID.GreaterHealingPotion };
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
				List<int> loot = new List<int>() { ItemType<PlaguebringerGoliathBag>(), ItemType<PlagueCellCluster>(), ItemType<VirulentKatana>(), ItemType<DiseasedPike>(), ItemType<ThePlaguebringer>(), ItemType<Malevolence>(), ItemType<PestilentDefiler>(), ItemType<TheHive>(), ItemType<MepheticSprayer>(), ItemType<PlagueStaff>(), ItemType<TheSyringe>(), ItemType<FuelCellBundle>(), ItemType<InfectedRemote>(), ItemType<Malachite>(), ItemType<BloomStone>(), ItemType<ToxicHeart>(), ItemID.GreaterHealingPotion };
				List<int> collection = new List<int>() { ItemType<PlaguebringerGoliathTrophy>(), ItemType<PlaguebringerGoliathMask>(), ItemType<KnowledgePlaguebringerGoliath>() };
				string instructions = $"Use an [i:{summon}] in the Jungle Biome";
				string despawn = CalamityUtils.ColorMessage("HOSTILE SPECIMENS TERMINATED. INITIATE RECALL TO HOME BASE.", new Color(0x00, 0xFF, 0x00));
				AddBoss(bossChecklist, calamity, "Plaguebringer Goliath", order, type, DownedPBG, summon, loot, collection, instructions, despawn);
			}

			// Ravager
			{
				BossDifficulty.TryGetValue("Ravager", out float order);
				List<int> segments = new List<int>() { NPCType<RavagerBody>(), NPCType<RavagerClawLeft>(), NPCType<RavagerClawRight>(), NPCType<RavagerHead>(), NPCType<RavagerLegLeft>(), NPCType<RavagerLegRight>() };
				int summon = ItemType<AncientMedallion>();
				List<int> loot = new List<int>() { ItemType<RavagerBag>(), ItemType<Bloodstone>(), ItemType<VerstaltiteBar>(), ItemType<DraedonBar>(), ItemType<CruptixBar>(), ItemType<CoreofCinder>(), ItemType<CoreofEleum>(), ItemType<CoreofChaos>(), ItemType<BarofLife>(), ItemType<CoreofCalamity>(), ItemType<UltimusCleaver>(), ItemType<RealmRavager>(), ItemType<Hematemesis>(), ItemType<SpikecragStaff>(), ItemType<CraniumSmasher>(), ItemType<BloodPact>(), ItemType<FleshTotem>(), ItemType<BloodflareCore>(), ItemType<InfernalBlood>(), ItemID.GreaterHealingPotion };
				List<int> collection = new List<int>() { ItemType<RavagerTrophy>(), ItemType<RavagerMask>(), ItemType<KnowledgeRavager>() };
				string instructions = $"Use an [i:{summon}]";
				string despawn = CalamityUtils.ColorMessage("The automaton of misshapen victims went looking for the true perpetrator.", new Color(0xB2, 0x22, 0x22));
				string bossLogTex = "CalamityMod/NPCs/Ravager/Ravager_BossChecklist";
				AddBoss(bossChecklist, calamity, "Ravager", order, segments, DownedRavager, summon, loot, collection, instructions, despawn, bossLogTex);
			}

			// Astrum Deus
			{
				BossDifficulty.TryGetValue("AstrumDeus", out float order);
				List<int> segments = new List<int>() { NPCType<AstrumDeusHeadSpectral>(), NPCType<AstrumDeusBodySpectral>(), NPCType<AstrumDeusTailSpectral>() };
				int summon = ItemType<Starcore>();
				List<int> loot = new List<int>() { ItemType<AstrumDeusBag>(), ItemType<Stardust>(), ItemType<TheMicrowave>(), ItemType<StarSputter>(), ItemType<Starfall>(), ItemType<GodspawnHelixStaff>(), ItemType<RegulusRiot>(), ItemType<Quasar>(), ItemType<AstralBulwark>(), ItemType<HideofAstrumDeus>(), ItemID.FragmentSolar, ItemID.FragmentVortex, ItemID.FragmentNebula, ItemID.FragmentStardust, ItemID.GreaterHealingPotion };
				List<int> collection = new List<int>() { ItemType<AstrumDeusTrophy>(), ItemType<AstrumDeusMask>(), ItemType<KnowledgeAstrumDeus>(), ItemType<KnowledgeAstralInfection>() };
				string instructions = $"Defeat 3 empowered astral titans or use a [i:{summon}] at Night";
				string despawn = CalamityUtils.ColorMessage("The infected deity retreats to the heavens.", new Color(0xFF, 0xD7, 0x00));
				string bossLogTex = "CalamityMod/NPCs/AstrumDeus/AstrumDeus_BossChecklist";
				AddBoss(bossChecklist, calamity, "Astrum Deus", order, segments, DownedDeus, summon, loot, collection, instructions, despawn, bossLogTex);
			}

			// Profaned Guardians
			{
				BossDifficulty.TryGetValue("ProfanedGuardians", out float order);
				int type = NPCType<ProfanedGuardianBoss>();
				int summon = ItemType<ProfanedShard>();
				List<int> loot = new List<int>() { ItemType<ProfanedCore>(), ItemID.GreaterHealingPotion };
				List<int> collection = new List<int>() { ItemType<ProfanedGuardianMask>(), ItemType<KnowledgeProfanedGuardians>() };
				string instructions = $"Use a [i:{summon}] in the Hallow or Underworld Biomes";
				string despawn = CalamityUtils.ColorMessage("The guardians must protect their goddess at all costs.", new Color(0xFF, 0xA5, 0x00));
				string bossLogTex = "CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardians_BossChecklist";
				AddBoss(bossChecklist, calamity, "Profaned Guardians", order, type, DownedGuardians, summon, loot, collection, instructions, despawn, bossLogTex);
			}

			// Bumblebirb
			{
				BossDifficulty.TryGetValue("Bumblebirb", out float order);
				int type = NPCType<Bumblefuck>();
				int summon = ItemType<BirbPheromones>();
				List<int> loot = new List<int>() { ItemType<BumblebirbBag>(), ItemType<EffulgentFeather>(), ItemType<GildedProboscis>(), ItemType<GoldenEagle>(), ItemType<RougeSlash>(), ItemType<Swordsplosion>(), ItemType<DynamoStemCells>(), ItemType<RedLightningContainer>(), ItemID.SuperHealingPotion };
				List<int> collection = new List<int>() { ItemType<BumblebirbTrophy>(), ItemType<BumblefuckMask>(), ItemType<KnowledgeBumblebirb>() };
				string instructions = $"Use [i:{summon}] in the Jungle Biome";
				string despawn = CalamityUtils.ColorMessage("The failed experiment returns into its reproductive routine.", new Color(0xFF, 0xD7, 0x00));
				AddBoss(bossChecklist, calamity, "Bumblebirb", order, type, DownedBirb, summon, loot, collection, instructions, despawn);
			}

			// Providence
			{
				BossDifficulty.TryGetValue("Providence", out float order);
				List<int> bosses = new List<int>() { NPCType<Providence>(), NPCType<ProvSpawnOffense>(), NPCType<ProvSpawnDefense>(), NPCType<ProvSpawnHealer>() };
				List<int> summons = new List<int>() { ItemType<ProfanedCore>(), ItemType<ProfanedCoreUnlimited>() };
				List<int> loot = new List<int>() { ItemType<ProvidenceBag>(), ItemType<UnholyEssence>(), ItemType<DivineGeode>(), ItemType<HolyCollider>(), ItemType<SolarFlare>(), ItemType<TelluricGlare>(), ItemType<BlissfulBombardier>(), ItemType<PurgeGuzzler>(), ItemType<MoltenAmputator>(), ItemType<DazzlingStabberStaff>(), ItemType<PristineFury>(), ItemType<ElysianWings>(), ItemType<ElysianAegis>(), ItemType<SamuraiBadge>(), ItemType<BlazingCore>(), ItemType<RuneofCos>(), ItemID.SuperHealingPotion };
				List<int> collection = new List<int>() { ItemType<ProvidenceTrophy>(), ItemType<ProvidenceMask>(), ItemType<KnowledgeProvidence>() };
				string instructions = $"Use either [i:{ItemType<ProfanedCore>()}] or [i:{ItemType<ProfanedCoreUnlimited>()}] in the Hallow or Underworld Biomes";
				string despawn = CalamityUtils.ColorMessage("The Profaned Goddess vanishes in a burning blaze.", new Color(0xFF, 0xA5, 0x00));
				string bossLogTex = "CalamityMod/NPCs/Providence/Providence_BossChecklist";
				AddBoss(bossChecklist, calamity, "Providence", order, bosses, DownedProvidence, summons, loot, collection, instructions, despawn, bossLogTex);
			}

			// Ceaseless Void
			{
				BossDifficulty.TryGetValue("CeaselessVoid", out float order);
				List<int> bosses = new List<int>() { NPCType<CeaselessVoid>(), NPCType<DarkEnergy>(), NPCType<DarkEnergy2>(), NPCType<DarkEnergy3>() };
				int summon = ItemType<RuneofCos>();
				List<int> loot = new List<int>() { ItemType<DarkPlasma>(), ItemType<MirrorBlade>(), ItemType<ArcanumoftheVoid>(), ItemType<TheEvolution>(), ItemID.SuperHealingPotion };
				List<int> collection = new List<int>() { ItemType<CeaselessVoidTrophy>(), ItemType<CeaselessVoidMask>(), ItemType<KnowledgeSentinels>() };
				string instructions = $"Use a [i:{summon}] in the Dungeon";
				string despawn = CalamityUtils.ColorMessage("The rift in time and space has moved away from your reach.", new Color(0x4B, 0x00, 0x82));
				AddBoss(bossChecklist, calamity, "Ceaseless Void", order, bosses, DownedCeaselessVoid, summon, loot, collection, instructions, despawn);
			}

			// Storm Weaver
			{
				BossDifficulty.TryGetValue("StormWeaver", out float order);
				List<int> segments = new List<int>() { NPCType<StormWeaverHead>(), NPCType<StormWeaverBody>(), NPCType<StormWeaverTail>(), NPCType<StormWeaverHeadNaked>(), NPCType<StormWeaverBodyNaked>(), NPCType<StormWeaverTailNaked>() };
				int summon = ItemType<RuneofCos>();
				List<int> loot = new List<int>() { ItemType<ArmoredShell>(), ItemType<TheStorm>(), ItemType<StormDragoon>(), ItemID.SuperHealingPotion };
				List<int> collection = new List<int>() { ItemType<WeaverTrophy>(), ItemType<StormWeaverMask>(), ItemType<KnowledgeSentinels>() };
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
				List<int> loot = new List<int>() { ItemType<TwistingNether>(), ItemType<Cosmilamp>(), ItemType<CosmicKunai>(), ItemType<LanternoftheSoul>(), ItemType<SpectralVeil>(), ItemID.SuperHealingPotion };
				List<int> collection = new List<int>() { ItemType<SignusTrophy>(), ItemType<SignusMask>(), ItemType<KnowledgeSentinels>() };
				string instructions = $"Use a [i:{summon}] in the Underworld";
				string despawn = CalamityUtils.ColorMessage("The Devourer's assassin has finished its easy task.", new Color(0xBA, 0x55, 0xD3));
				AddBoss(bossChecklist, calamity, "Signus", order, type, DownedSignus, summon, loot, collection, instructions, despawn);
			}

			// Polterghast
			{
				BossDifficulty.TryGetValue("Polterghast", out float order);
				List<int> bosses = new List<int>() { NPCType<Polterghast>(), NPCType<PolterPhantom>() };
				int summon = ItemType<NecroplasmicBeacon>();
				List<int> loot = new List<int>() { ItemType<PolterghastBag>(), ItemType<RuinousSoul>(), ItemType<Phantoplasm>(), ItemType<TerrorBlade>(), ItemType<BansheeHook>(), ItemType<DaemonsFlame>(), ItemType<FatesReveal>(), ItemType<GhastlyVisage>(), ItemType<EtherealSubjugator>(), ItemType<GhoulishGouger>(), ItemType<Affliction>(), ItemType<Ectoheart>(), ItemID.SuperHealingPotion };
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
				List<int> loot = new List<int>() { ItemType<OldDukeBag>(), ItemType<InsidiousImpaler>(), ItemType<SepticSkewer>(), ItemType<FetidEmesis>(), ItemType<VitriolicViper>(), ItemType<CadaverousCarrion>(), ItemType<ToxicantTwister>(), ItemType<DukeScales>(), ItemType<MutatedTruffle>(), ItemID.SuperHealingPotion };
				List<int> collection = new List<int>() { ItemType<OldDukeTrophy>(), ItemType<OldDukeMask>(), ItemType<KnowledgeOldDuke>() };
				string instructions = $"Defeat the Acid Rain event post-Polterghast or fish using a [i:{summon}] in the Sulphurous Sea";
				string despawn = CalamityUtils.ColorMessage("The old duke disappears amidst the acidic downpour.", new Color(0xF0, 0xE6, 0x8C));
				AddBoss(bossChecklist, calamity, "Old Duke", order, bosses, DownedBoomerDuke, summon, loot, collection, instructions, despawn);
			}

			// Devourer of Gods
			{
				BossDifficulty.TryGetValue("DevourerOfGods", out float order);
				int type = NPCType<DevourerofGodsHeadS>();
				int summon = ItemType<CosmicWorm>();
				List<int> loot = new List<int>() { ItemType<DevourerofGodsBag>(), ItemType<CosmiliteBar>(), ItemType<CosmiliteBrick>(), ItemType<Excelsus>(), ItemType<EradicatorMelee>(), ItemType<TheObliterator>(), ItemType<Deathwind>(), ItemType<DeathhailStaff>(), ItemType<StaffoftheMechworm>(), ItemType<Eradicator>(), ItemType<Skullmasher>(), ItemType<Norfleet>(), ItemType<CosmicDischarge>(), ItemType<NebulousCore>(), ItemType<Fabsol>(), ItemType<SupremeHealingPotion>() };
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
				List<int> loot = new List<int>() { ItemType<YharonBag>(), ItemType<HellcasterFragment>(), ItemType<DragonRage>(), ItemType<TheBurningSky>(), ItemType<DragonsBreath>(), ItemType<ChickenCannon>(), ItemType<PhoenixFlameBarrage>(), ItemType<AngryChickenStaff>(), ItemType<ProfanedTrident>(), ItemType<VoidVortex>(), ItemType<YharimsCrystal>(), ItemType<YharimsGift>(), ItemType<DrewsWings>(), ItemType<BossRush>(), ItemType<OmegaHealingPotion>() };
				List<int> collection = new List<int>() { ItemType<YharonTrophy>(), ItemType<YharonMask>(), ItemType<KnowledgeYharon>(), ItemType<ForgottenDragonEgg>(), ItemType<FoxDrive>() };
				string instructions = $"Use a [i:{summon}] in the Jungle Biome";
				// TODO -- this setup code is only run once, so the despawn message can't be changed post-eclipse. Find a way around this.
				string despawn = CalamityUtils.ColorMessage("Yharon found you too weak to stay near your gravestone.", new Color(0xFF, 0xA5, 0x00));
				string bossLogTex = "CalamityMod/NPCs/Yharon/Yharon_BossChecklist";
				AddBoss(bossChecklist, calamity, "Yharon", order, type, DownedYharon, summon, loot, collection, instructions, despawn, bossLogTex);
			}

			// Supreme Calamitas
			{
				BossDifficulty.TryGetValue("SupremeCalamitas", out float order);
				int type = NPCType<SupremeCalamitas>();
				int summon = ItemType<EyeofExtinction>();
				List<int> loot = new List<int>() { ItemType<CalamitousEssence>(), ItemType<Animus>(), ItemType<Azathoth>(), ItemType<Contagion>(), ItemType<CrystylCrusher>(), ItemType<DraconicDestruction>(), ItemType<Earth>(), ItemType<Fabstaff>(), ItemType<RoyalKnivesMelee>(), ItemType<RoyalKnives>(), ItemType<NanoblackReaperMelee>(), ItemType<NanoblackReaperRogue>(), ItemType<RedSun>(), ItemType<ScarletDevil>(), ItemType<SomaPrime>(), ItemType<BlushieStaff>(), ItemType<Svantechnical>(), ItemType<Judgement>(), ItemType<TriactisTruePaladinianMageHammerofMightMelee>(), ItemType<TriactisTruePaladinianMageHammerofMight>(), ItemType<Megafleet>(), ItemType<Endogenesis>(), ItemType<Vehemenc>(), ItemType<OmegaHealingPotion>() };
				List<int> collection = new List<int>() { ItemType<KnowledgeCalamitas>(), ItemType<BrimstoneJewel>(), ItemType<Levi>() };
				string instructions = $"Use an [i:{summon}]";
				string despawn = CalamityUtils.ColorMessage("Please don't waste my time.", new Color(0xFF, 0xA5, 0x00));
				AddBoss(bossChecklist, calamity, "Supreme Calamitas", order, type, DownedSCal, summon, loot, collection, instructions, despawn);
			}
		}
		
		private static void AddCalamityInvasions(Mod bossChecklist, Mod calamity)
		{
			// Initial Acid Rain
			{
				InvasionDifficulty.TryGetValue("Acid Rain Initial", out float order);
				List<int> enemies = AcidRainEvent.PossibleEnemiesPreHM.Select(enemy => enemy.Item1).ToList();
				int summon = ItemType<CausticTear>();
				List<int> loot = new List<int>() { ItemType<SulfuricScale>() };
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
				List<int> enemies = AcidRainEvent.PossibleEnemiesAS.Select(enemy => enemy.Item1).ToList();
				enemies.Add(ModContent.NPCType<CragmawMire>());
				int summon = ItemType<CausticTear>();
				List<int> loot = new List<int>() { ItemType<SulfuricScale>(), ItemType<CorrodedFossil>(), ItemType<LeadCore>(), ItemType<NuclearRod>(), ItemType<FlakToxicannon>() };
				List<int> collection = new List<int>() { ItemType<RadiatingCrystal>() };
				string instructions = $"Use a [i:{summon}] or wait for the invasion to occur naturally after the Aquatic Scourge is defeated";
				string despawn = CalamityUtils.ColorMessage("The mysterious creatures of the sulphuric sea descended back into the deep ocean.", new Color(146, 183, 116));
				string bossLogTex = "CalamityMod/Events/AcidRainT2_BossChecklist";
				string iconTexture = "CalamityMod/ExtraTextures/UI/AcidRainIcon";
				AddInvasion(bossChecklist, calamity, "Acid Rain (Post-AS)", order, enemies, DownedAcidRainHardmode, summon, loot, collection, instructions, despawn, bossLogTex, iconTexture);
			}
			// Post-Polterghast Acid Rain
			{
				InvasionDifficulty.TryGetValue("Acid Rain Polterghast", out float order);
				List<int> enemies = AcidRainEvent.PossibleEnemiesPolter.Select(enemy => enemy.Item1).ToList();
				enemies.Add(ModContent.NPCType<CragmawMire>());
				int summon = ItemType<CausticTear>();
				List<int> loot = new List<int>() { ItemType<SulfuricScale>(), ItemType<CorrodedFossil>(), ItemType<LeadCore>(), ItemType<NuclearRod>(), ItemType<FlakToxicannon>() };
				List<int> collection = new List<int>() { ItemType<RadiatingCrystal>() };
				string instructions = $"Use a [i:{summon}] or wait for the invasion to occur naturally after the Polterghast is defeated";
				string despawn = CalamityUtils.ColorMessage("The mysterious creatures of the sulphuric sea descended back into the deep ocean.", new Color(146, 183, 116));
				string bossLogTex = "CalamityMod/Events/AcidRainT2_BossChecklist";
				string iconTexture = "CalamityMod/ExtraTextures/UI/AcidRainIcon";
				AddInvasion(bossChecklist, calamity, "Acid Rain (Post-Polter)", order, enemies, DownedBoomerDuke, summon, loot, collection, instructions, despawn, bossLogTex, iconTexture);
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
				new List<int>() { ItemType<VictoryShard>(), ItemType<TeardropCleaver>(), ItemType<CounterScarf>() },
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
				new List<int>() { ItemType<HardenedHoneycomb>() },
				new List<int>() { ItemType<KnowledgeQueenBee>() }
			);

			// Skeletron
			AddLoot(bossChecklist, "SkeletronHead",
				new List<int>() { ItemType<ClothiersWrath>() },
				new List<int>() { ItemType<KnowledgeSkeletron>() }
			);

			// Wall of Flesh
			AddLoot(bossChecklist, "WallofFlesh",
				new List<int>() { ItemType<Meowthrower>(), ItemType<BlackHawkRemote>(), ItemType<BlastBarrel>(), ItemType<RogueEmblem>(), ItemType<MLGRune>(), ItemID.CorruptionKey, ItemID.CrimsonKey },
				new List<int>() { ItemType<KnowledgeWallofFlesh>(), ItemType<KnowledgeUnderworld>(), ItemType<IbarakiBox>() }
			);

			// The Twins
			AddLoot(bossChecklist, "TheTwins",
				null,
				new List<int>() { ItemType<KnowledgeTwins>(), ItemType<KnowledgeMechs>() }
			);

			// The Destroyer
			AddLoot(bossChecklist, "TheDestroyer",
				new List<int>() { ItemType<SHPC>() },
				new List<int>() { ItemType<KnowledgeDestroyer>(), ItemType<KnowledgeMechs>() }
			);

			// Skeletron Prime
			AddLoot(bossChecklist, "SkeletronPrime",
				null,
				new List<int>() { ItemType<KnowledgeSkeletronPrime>(), ItemType<KnowledgeMechs>() }
			);

			// Plantera
			AddLoot(bossChecklist, "Plantera",
				new List<int>() { ItemType<LivingShard>(), ItemType<BlossomFlux>(), ItemID.JungleKey },
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
				new List<int>() { ItemType<Vesuvius>() }
			);

			// Lunatic Cultist
			AddLoot(bossChecklist, "CultistBoss",
				new List<int>() { ItemType<StardustStaff>(), ItemType<ThornBlossom>() },
				new List<int>() { ItemType<KnowledgeLunaticCultist>(), ItemType<KnowledgeBloodMoon>() }
			);
			AddSummons(bossChecklist, "CultistBoss", new List<int>() { ItemType<EidolonTablet>() });

			// Moon Lord
			AddLoot(bossChecklist, "MoonLord",
				new List<int>() { ItemType<UtensilPoker>(), ItemType<GrandDad>(), ItemType<Infinity>(), ItemType<MLGRune2>() },
				new List<int>() { ItemType<KnowledgeMoonLord>() }
			);
		}

		private static void AddCalamityEventLoot(Mod bossChecklist)
		{
			// Blood Moon
			AddLoot(bossChecklist, "Blood Moon",
				new List<int>() { ItemType<BloodOrb>(), ItemType<BouncingEyeball>(), ItemType<Carnage>() },
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
				new List<int>() { ItemType<RaidersGlory>(), ItemType<Arbalest>(), ItemType<ProporsePistol>() },
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
				new List<int>() { ItemType<HolidayHalberd>(), ItemType<EndothermicEnergy>() },
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
				new List<int>() { ItemType<MeldBlob>(), ItemType<TrueConferenceCall>() },
				null
			);
		}

		private static void FargosSupport()
		{
			Mod fargos = ModLoader.GetMod("Fargowiltas");
			if (fargos is null)
				return;

			// Mark Fargo's Mutant Mod as loaded so that Calamity doesn't add ANY boss summons to vanilla NPCs, even for its own bosses
			GetInstance<CalamityMod>().fargosMutant = true;

			void AddToMutantShop(string bossName, string summonItemName, Func<bool> downed, int price)
			{
				BossDifficulty.TryGetValue(bossName, out float order);
				fargos.Call("AddSummon", order, "CalamityMod", summonItemName, downed, price);
			}

			AddToMutantShop("Crabulon", "DecapoditaSprout", DownedCrabulon, Item.buyPrice(gold: 4));
			AddToMutantShop("HiveMind", "Teratoma", DownedHiveMind, Item.buyPrice(gold: 10));
			AddToMutantShop("Perforators", "BloodyWormFood", DownedPerfs, Item.buyPrice(gold: 10));
			AddToMutantShop("SlimeGod", "OverloadedSludge", DownedSlimeGod, Item.buyPrice(gold: 15));
			AddToMutantShop("BrimstoneElemental", "CharredIdol", DownedBrimstoneElemental, Item.buyPrice(gold: 20));
			AddToMutantShop("AstrumAureus", "AstralChunk", DownedAureus, Item.buyPrice(gold: 25));
			AddToMutantShop("PlaguebringerGoliath", "Abombination", DownedPBG, Item.buyPrice(gold: 50));
			AddToMutantShop("Ravager", "AncientMedallion", DownedRavager, Item.buyPrice(gold: 50));
			AddToMutantShop("ProfanedGuardians", "ProfanedShard", DownedGuardians, Item.buyPrice(platinum: 5));
			AddToMutantShop("Bumblebirb", "BirbPheromones", DownedBirb, Item.buyPrice(platinum: 5));
		}

		private static void CensusSupport()
		{
			Mod censusMod = ModLoader.GetMod("Census");
			if (censusMod != null)
			{
				censusMod.Call("TownNPCCondition", NPCType<SEAHOE>(), "Defeat a Giant Clam after defeating the Desert Scourge");
				censusMod.Call("TownNPCCondition", NPCType<THIEF>(), "Have a [i:" + ItemID.PlatinumCoin + "] in your inventory after defeating Skeletron");
				censusMod.Call("TownNPCCondition", NPCType<FAP>(), "Have [i:" + ItemType<FabsolsVodka>() + "] in your inventory in Hardmode");
				censusMod.Call("TownNPCCondition", NPCType<DILF>(), "Defeat Cryogen");
			}
		}

		private static void SummonersAssociationSupport()
		{
			Mod sAssociation = ModLoader.GetMod("SummonersAssociation");
			if(sAssociation != null)
			{
				sAssociation.Call("AddMinionInfo", ItemType<SquirrelSquireStaff>(), BuffType<SquirrelSquireBuff>(), ProjectileType<SquirrelSquireMinion>());
				sAssociation.Call("AddMinionInfo", ItemType<WulfrumController>(), BuffType<WulfrumDroidBuff>(), ProjectileType<WulfrumDroid>());
				sAssociation.Call("AddMinionInfo", ItemType<SunSpiritStaff>(), BuffType<SolarSpirit>(), ProjectileType<SolarPixie>());
				sAssociation.Call("AddMinionInfo", ItemType<FrostBlossomStaff>(), BuffType<FrostBlossomBuff>(), ProjectileType<FrostBlossom>());
				sAssociation.Call("AddMinionInfo", ItemType<BelladonnaSpiritStaff>(), BuffType<BelladonnaSpiritBuff>(), ProjectileType<BelladonnaSpirit>());
				sAssociation.Call("AddMinionInfo", ItemType<StormjawStaff>(), BuffType<StormjawBuff>(), ProjectileType<StormjawBaby>());
				sAssociation.Call("AddMinionInfo", ItemType<SeaboundStaff>(), BuffType<BrittleStar>(), ProjectileType<BrittleStarMinion>());
				sAssociation.Call("AddMinionInfo", ItemType<MagicalConch>(), BuffType<HermitCrab>(), ProjectileType<HermitCrabMinion>());
				sAssociation.Call("AddMinionInfo", ItemType<VileFeeder>(), BuffType<VileFeederBuff>(), ProjectileType<VileFeederSummon>());
				sAssociation.Call("AddMinionInfo", ItemType<ScabRipper>(), BuffType<ScabRipperBuff>(), ProjectileType<BabyBloodCrawler>());
				sAssociation.Call("AddMinionInfo", ItemType<CinderBlossomStaff>(), BuffType<CinderBlossomBuff>(), ProjectileType<CinderBlossom>());
				sAssociation.Call("AddMinionInfo", ItemType<BloodClotStaff>(), BuffType<BloodClot>(), ProjectileType<BloodClotMinion>());
				sAssociation.Call("AddMinionInfo", ItemType<DankStaff>(), BuffType<DankCreeperBuff>(), ProjectileType<DankCreeperMinion>());
				sAssociation.Call("AddMinionInfo", ItemType<HerringStaff>(), BuffType<Herring>(), ProjectileType<HerringMinion>());
				sAssociation.Call("AddMinionInfo", ItemType<CorroslimeStaff>(), BuffType<Corroslime>(), ProjectileType<CorroslimeMinion>());
				sAssociation.Call("AddMinionInfo", ItemType<CrimslimeStaff>(), BuffType<Crimslime>(), ProjectileType<CrimslimeMinion>());
				sAssociation.Call("AddMinionInfo", ItemType<BlackHawkRemote>(), BuffType<BlackHawkBuff>(), ProjectileType<BlackHawkSummon>());
				sAssociation.Call("AddMinionInfo", ItemType<AncientIceChunk>(), BuffType<IceClasper>(), ProjectileType<IceClasperMinion>());
				sAssociation.Call("AddMinionInfo", ItemType<ShellfishStaff>(), BuffType<ShellfishBuff>(), ProjectileType<Shellfish>());
				sAssociation.Call("AddMinionInfo", ItemType<HauntedScroll>(), BuffType<HauntedDishesBuff>(), ProjectileType<HauntedDishes>());
				sAssociation.Call("AddMinionInfo", ItemType<ForgottenApexWand>(), BuffType<ApexSharkBuff>(), ProjectileType<ApexShark>());
				sAssociation.Call("AddMinionInfo", ItemType<DeepseaStaff>(), BuffType<AquaticStar>(), ProjectileType<AquaticStarMinion>());
				sAssociation.Call("AddMinionInfo", ItemType<SunGodStaff>(), BuffType<SolarSpiritGod>(), ProjectileType<SolarGod>());
				sAssociation.Call("AddMinionInfo", ItemType<DormantBrimseeker>(), BuffType<DormantBrimseekerBuff>(), ProjectileType<DormantBrimseekerBab>());
				sAssociation.Call("AddMinionInfo", ItemType<IgneousExaltation>(), BuffType<IgneousExaltationBuff>(), ProjectileType<IgneousBlade>());
				sAssociation.Call("AddMinionInfo", ItemType<BlightedEyeStaff>(), BuffType<CalamitasEyes>(), new List<int>() { ProjectileType<Calamitamini>(), ProjectileType<Cataclymini>(), ProjectileType<Catastromini>()}, new List<float>() {1f-0.6666666f, 0.3333333f, 0.3333333f});
				//Entropy's Vigil is a bruh moment
				sAssociation.Call("AddMinionInfo", ItemType<PlantationStaff>(), BuffType<PlantationBuff>(), ProjectileType<PlantSummon>());
				sAssociation.Call("AddMinionInfo", ItemType<SandSharknadoStaff>(), BuffType<Sandnado>(), ProjectileType<SandnadoMinion>());
				sAssociation.Call("AddMinionInfo", ItemType<ResurrectionButterfly>(), BuffType<ResurrectionButterflyBuff>(), new List<int>() { ProjectileType<PinkButterfly>(), ProjectileType<PurpleButterfly>()});
				sAssociation.Call("AddMinionInfo", ItemType<FuelCellBundle>(), BuffType<FuelCellBundleBuff>(), ProjectileType<PlaguebringerMK2>());
				sAssociation.Call("AddMinionInfo", ItemType<GodspawnHelixStaff>(), BuffType<AstralProbeBuff>(), ProjectileType<AstralProbeSummon>());
				sAssociation.Call("AddMinionInfo", ItemType<TacticalPlagueEngine>(), BuffType<TacticalPlagueEngineBuff>(), ProjectileType<TacticalPlagueEngineSummon>());
				sAssociation.Call("AddMinionInfo", ItemType<ElementalAxe>(), BuffType<ElementalAxeBuff>(), ProjectileType<ElementalAxeMinion>());
				sAssociation.Call("AddMinionInfo", ItemType<DazzlingStabberStaff>(), BuffType<DazzlingStabberBuff>(), ProjectileType<DazzlingStabber>());
				sAssociation.Call("AddMinionInfo", ItemType<DragonbloodDisgorger>(), BuffType<BloodDragonsBuff>(), ProjectileType<SkeletalDragonMother>());
				sAssociation.Call("AddMinionInfo", ItemType<Cosmilamp>(), BuffType<CosmilampBuff>(), ProjectileType<CosmilampMinion>());
				sAssociation.Call("AddMinionInfo", ItemType<EtherealSubjugator>(), BuffType<Phantom>(), ProjectileType<PhantomGuy>());
				sAssociation.Call("AddMinionInfo", ItemType<CalamarisLament>(), BuffType<Calamari>(), ProjectileType<CalamariMinion>());
				sAssociation.Call("AddMinionInfo", ItemType<StaffoftheMechworm>(), BuffType<Mechworm>(), ProjectileType<MechwormBody>(), 1f);
				sAssociation.Call("AddMinionInfo", ItemType<CorvidHarbringerStaff>(), BuffType<CorvidHarbringerBuff>(), ProjectileType<PowerfulRaven>());
				sAssociation.Call("AddMinionInfo", ItemType<EndoHydraStaff>(), BuffType<EndoHydraBuff>(), ProjectileType<EndoHydraHead>());
				sAssociation.Call("AddMinionInfo", ItemType<CosmicViperEngine>(), BuffType<CosmicViperEngineBuff>(), ProjectileType<CosmicViperSummon>());
				sAssociation.Call("AddMinionInfo", ItemType<AngryChickenStaff>(), BuffType<YharonKindleBuff>(), ProjectileType<SonOfYharon>());
				sAssociation.Call("AddMinionInfo", ItemType<MidnightSunBeacon>(), BuffType<MidnightSunBuff>(), ProjectileType<MidnightSunUFO>());
				sAssociation.Call("AddMinionInfo", ItemType<CosmicImmaterializer>(), BuffType<CosmicEnergy>(), ProjectileType<CosmicEnergySpiral>());
				sAssociation.Call("AddMinionInfo", ItemType<BensUmbrella>(), BuffType<MagicHatBuff>(), ProjectileType<MagicHat>());
				sAssociation.Call("AddMinionInfo", ItemType<Endogenesis>(), BuffType<EndoCooperBuff>(), ProjectileType<EndoCooperBody>());
			}
		}
	}
}
