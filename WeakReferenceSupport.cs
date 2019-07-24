using System;
using Terraria;
using Terraria.ModLoader;

using CalamityMod.World;

namespace CalamityMod
{
	internal class WeakReferenceSupport
	{
		public static void Setup()
		{
			BossChecklistSupport();
			CensusSupport();
		}

		private static void BossChecklistSupport()
		{
			Mod mod = ModLoader.GetMod("CalamityMod");
			Mod bossChecklist = ModLoader.GetMod("BossChecklist");

			if (bossChecklist != null)
			{
				// 14 is moonlord, 12 is duke fishron
				bossChecklist.Call("AddBossWithInfo", "Desert Scourge", 1.5f, (Func<bool>)(() => CalamityWorld.downedDesertScourge), "Use a [i:" + mod.ItemType("DriedSeafood") + "] in the Desert Biome"); //1
				bossChecklist.Call("AddBossWithInfo", "Crabulon", 2.5f, (Func<bool>)(() => CalamityWorld.downedCrabulon), "Use a [i:" + mod.ItemType("DecapoditaSprout") + "] in the Mushroom Biome"); //1.5
				bossChecklist.Call("AddBossWithInfo", "Hive Mind / Perforator", 3.5f, (Func<bool>)(() => (CalamityWorld.downedPerforator || CalamityWorld.downedHiveMind)), "By killing a Cyst in the World's Evil Biome"); //2
				bossChecklist.Call("AddBossWithInfo", "Slime God", 5.5f, (Func<bool>)(() => CalamityWorld.downedSlimeGod), "Use an [i:" + mod.ItemType("OverloadedSludge") + "]"); //4
				bossChecklist.Call("AddBossWithInfo", "Cryogen", 6.5f, (Func<bool>)(() => CalamityWorld.downedCryogen), "Use a [i:" + mod.ItemType("CryoKey") + "] in the Snow Biome"); //5
				bossChecklist.Call("AddBossWithInfo", "Brimstone Elemental", 7.5f, (Func<bool>)(() => CalamityWorld.downedBrimstoneElemental), "Use a [i:" + mod.ItemType("CharredIdol") + "] in the Hell Crag"); //6
				bossChecklist.Call("AddBossWithInfo", "Aquatic Scourge", 8.5f, (Func<bool>)(() => CalamityWorld.downedAquaticScourge), "Use a [i:" + mod.ItemType("Seafood") + "] in the Sulphuric Sea or wait for it to spawn in the Sulphuric Sea"); //6
				bossChecklist.Call("AddBossWithInfo", "Calamitas", 9.7f, (Func<bool>)(() => CalamityWorld.downedCalamitas), "Use an [i:" + mod.ItemType("BlightedEyeball") + "] at Night"); //7
				bossChecklist.Call("AddBossWithInfo", "Leviathan", 10.5f, (Func<bool>)(() => CalamityWorld.downedLeviathan), "By killing an unknown entity in the Ocean Biome"); //8
				bossChecklist.Call("AddBossWithInfo", "Astrum Aureus", 10.6f, (Func<bool>)(() => CalamityWorld.downedAstrageldon), "Use an [i:" + mod.ItemType("AstralChunk") + "] at Night"); //8.5
				bossChecklist.Call("AddBossWithInfo", "Plaguebringer Goliath", 11.5f, (Func<bool>)(() => CalamityWorld.downedPlaguebringer), "Use an [i:" + mod.ItemType("Abomination") + "] in the Jungle Biome"); //9
				bossChecklist.Call("AddBossWithInfo", "Ravager", 12.5f, (Func<bool>)(() => CalamityWorld.downedScavenger), "Use an [i:" + mod.ItemType("AncientMedallion") + "]"); //9.5
				bossChecklist.Call("AddBossWithInfo", "Astrum Deus", 13.5f, (Func<bool>)(() => CalamityWorld.downedStarGod), "Use a [i:" + mod.ItemType("Starcore") + "] at Night or defeat 3 empowered astral titans"); //9.6
				//bossChecklist.Call("AddBossWithInfo", "The Old Duke", 13.6f, (Func<bool>)(() => CalamityWorld.downedOldDuke), "Fishing with some type of bait in the Sulphuric Sea"); //9.7
				bossChecklist.Call("AddBossWithInfo", "Profaned Guardians", 14.5f, (Func<bool>)(() => CalamityWorld.downedGuardians), "Use a [i:" + mod.ItemType("ProfanedShard") + "] in the Hallow or Underworld Biomes"); //10
				bossChecklist.Call("AddBossWithInfo", "Bumblebirb", 14.6f, (Func<bool>)(() => CalamityWorld.downedBumble), "Use [i:" + mod.ItemType("BirbPheromones") + "] in the Jungle Biome"); //16
				bossChecklist.Call("AddBossWithInfo", "Providence", 15f, (Func<bool>)(() => CalamityWorld.downedProvidence), "Use a [i:" + mod.ItemType("ProfanedCore") + "] in the Hallow or Underworld Biomes"); //11
				bossChecklist.Call("AddBossWithInfo", "Ceaseless Void", 15.1f, (Func<bool>)(() => CalamityWorld.downedSentinel1), "Use a [i:" + mod.ItemType("RuneofCos") + "] in the Dungeon"); //12
				bossChecklist.Call("AddBossWithInfo", "Storm Weaver", 15.2f, (Func<bool>)(() => CalamityWorld.downedSentinel2), "Use a [i:" + mod.ItemType("RuneofCos") + "] in Space"); //13
				bossChecklist.Call("AddBossWithInfo", "Signus", 15.3f, (Func<bool>)(() => CalamityWorld.downedSentinel3), "Use a [i:" + mod.ItemType("RuneofCos") + "] in the Underworld"); //14
				bossChecklist.Call("AddBossWithInfo", "Polterghast", 15.5f, (Func<bool>)(() => CalamityWorld.downedPolterghast), "Use a [i:" + mod.ItemType("NecroplasmicBeacon") + "] in the Dungeon or kill 30 phantom spirits"); //11
				bossChecklist.Call("AddBossWithInfo", "Devourer of Gods", 16f, (Func<bool>)(() => CalamityWorld.downedDoG), "Use a [i:" + mod.ItemType("CosmicWorm") + "]"); //15
				bossChecklist.Call("AddBossWithInfo", "Yharon", 17f, (Func<bool>)(() => CalamityWorld.downedYharon), "Use a [i:" + mod.ItemType("ChickenEgg") + "] in the Jungle Biome"); //17
				bossChecklist.Call("AddBossWithInfo", "Supreme Calamitas", 18f, (Func<bool>)(() => CalamityWorld.downedSCal), "Use an [i:" + mod.ItemType("EyeofExtinction") + "]"); //18
			}
		}

		private static void CensusSupport()
		{
			Mod mod = ModLoader.GetMod("CalamityMod");
			Mod censusMod = ModLoader.GetMod("Census");
			if (censusMod != null)
			{
				censusMod.Call("TownNPCCondition", mod.NPCType("SEAHOE"), "Defeat a Giant Clam");
				censusMod.Call("TownNPCCondition", mod.NPCType("FAP"), "Have [i:" + mod.ItemType("FabsolsVodka") + "] in your inventory in Hardmode");
				censusMod.Call("TownNPCCondition", mod.NPCType("DILF"), "Defeat Cryogen");
			}
		}
	}
}
