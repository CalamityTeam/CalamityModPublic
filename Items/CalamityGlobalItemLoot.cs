using CalamityMod.Items.Accessories;
using CalamityMod.Items.DifficultyItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
	public class CalamityGlobalItemLoot : GlobalItem
	{
		public override bool CloneNewInstances => false;
		public override bool InstancePerEntity => false;

		// NOTE: this function applies to all right-click-to-open items, even modded ones (despite the name).
		// This means it applies to boss bags, crates, lockboxes, herb bags, goodie bags, and presents.
		// The internal names of these cases are as follows (TML 0.11 API):
		// bossBag, crate, lockBox, herbBag, goodieBag, present
		public override void OpenVanillaBag(string context, Player player, int itemID)
		{
			if (context == "crate")
				CrateLoot(player, itemID);

			else if (context == "bossBag")
				BossBagLoot(player, itemID);
		}

		#region Boss Bags
		private static void BossBagLoot(Player player, int itemID)
		{
			// Give a chance for Laudanum, Stress Pills and Heart of Darkness from every boss bag
			DropHelper.DropRevBagAccessories(player);

			switch (itemID)
			{
				case ItemID.KingSlimeBossBag:
					DropHelper.DropItemCondition(player, ModContent.ItemType<CrownJewel>(), CalamityWorld.revenge);
					break;

				case ItemID.EyeOfCthulhuBossBag:
					DropHelper.DropItem(player, ModContent.ItemType<VictoryShard>(), 3, 5);
					DropHelper.DropItemChance(player, ModContent.ItemType<TeardropCleaver>(), 3);
					DropHelper.DropItemCondition(player, ModContent.ItemType<CounterScarf>(), CalamityWorld.revenge);
					break;

				case ItemID.QueenBeeBossBag:
					// Drop weapons Calamity style instead of mutually exclusive
					int[] queenBeeWeapons = new int[]
					{
						ItemID.BeeKeeper,
						ItemID.BeesKnees,
						ItemID.BeeGun,
					};
					DropHelper.DropEntireSet(player, 0.33f, queenBeeWeapons);
					DropHelper.BlockDrops(queenBeeWeapons);

					DropHelper.DropItem(player, ItemID.Stinger, 8, 12); // Extra stingers
					DropHelper.DropItem(player, ModContent.ItemType<HardenedHoneycomb>(), 50, 75);
					break;

				case ItemID.SkeletronBossBag:
					DropHelper.DropItemChance(player, ModContent.ItemType<ClothiersWrath>(), DropHelper.RareVariantDropRateInt);
					break;

				case ItemID.WallOfFleshBossBag:
					DropHelper.DropItemChance(player, ModContent.ItemType<Meowthrower>(), 3);
					DropHelper.DropItemChance(player, ModContent.ItemType<BlackHawkRemote>(), 3);
					DropHelper.DropItemChance(player, ModContent.ItemType<BlastBarrel>(), 3);
					DropHelper.DropItemChance(player, ModContent.ItemType<RogueEmblem>(), 4);
					DropHelper.DropItemFromSetChance(player, 0.2f, ItemID.CorruptionKey, ItemID.CrimsonKey);
					DropHelper.DropItemCondition(player, ModContent.ItemType<MLGRune>(), !CalamityWorld.demonMode); // Demon Trophy
					break;

				case ItemID.DestroyerBossBag:
					float shpcChance = DropHelper.LegendaryDropRateFloat;
					DropHelper.DropItemCondition(player, ModContent.ItemType<SHPC>(), CalamityWorld.revenge, shpcChance);
					break;

				case ItemID.PlanteraBossBag:
					DropHelper.DropItem(player, ModContent.ItemType<LivingShard>(), 16, 22);
					float bFluxChance = DropHelper.LegendaryDropRateFloat;
					DropHelper.DropItemCondition(player, ModContent.ItemType<BlossomFlux>(), CalamityWorld.revenge, bFluxChance);
					DropHelper.DropItemChance(player, ItemID.JungleKey, 5);
					break;

				case ItemID.GolemBossBag:
					float aegisChance = DropHelper.LegendaryDropRateFloat;
					DropHelper.DropItemCondition(player, ModContent.ItemType<AegisBlade>(), CalamityWorld.revenge, aegisChance);
					DropHelper.DropItem(player, ModContent.ItemType<EssenceofCinder>(), 8, 13);
					DropHelper.DropItemChance(player, ModContent.ItemType<LeadWizard>(), DropHelper.RareVariantDropRateInt);
					break;

				case ItemID.FishronBossBag:
					float baronChance = DropHelper.LegendaryDropRateFloat;
					DropHelper.DropItemCondition(player, ModContent.ItemType<BrinyBaron>(), CalamityWorld.revenge, baronChance);
					DropHelper.DropItemChance(player, ModContent.ItemType<DukesDecapitator>(), 4);
					break;

				case ItemID.BossBagBetsy:
					float vesuviusChance = DropHelper.LegendaryDropRateFloat;
					DropHelper.DropItemCondition(player, ModContent.ItemType<Vesuvius>(), CalamityWorld.revenge, vesuviusChance);
					break;

				case ItemID.MoonLordBossBag:
					DropHelper.DropItem(player, ItemID.LunarOre, 50);
					DropHelper.DropItem(player, ModContent.ItemType<MLGRune2>()); // Celestial Onion
					DropHelper.DropItemChance(player, ModContent.ItemType<UtensilPoker>(), 8);
					DropHelper.DropItemChance(player, ModContent.ItemType<GrandDad>(), DropHelper.RareVariantDropRateInt);
					DropHelper.DropItemChance(player, ModContent.ItemType<Infinity>(), DropHelper.RareVariantDropRateInt);
					break;
			}
		}
		#endregion

		#region Fishing Crates
		private static void CrateLoot(Player player, int itemID)
		{
			switch (itemID)
			{
				case ItemID.WoodenCrate:
					DropHelper.DropItemChance(player, ModContent.ItemType<WulfrumShard>(), 0.25f, 3, 5);
					break;

				case ItemID.IronCrate:
					DropHelper.DropItemChance(player, ModContent.ItemType<WulfrumShard>(), 0.25f, 5, 8);
					DropHelper.DropItemChance(player, ModContent.ItemType<AncientBoneDust>(), 0.25f, 5, 8);
					break;

				case ItemID.CorruptFishingCrate:
				case ItemID.CrimsonFishingCrate:
					DropHelper.DropItemChance(player, ModContent.ItemType<EbonianGel>(), 0.15f, 5, 8);
					DropHelper.DropItemChance(player, ModContent.ItemType<MurkySludge>(), 0.15f, 1, 3);
					break;

				case ItemID.HallowedFishingCrate:
					int potion = WorldGen.crimson ? ModContent.ItemType<ProfanedRagePotion>() : ModContent.ItemType<HolyWrathPotion>();
					DropHelper.DropItemCondition(player, ModContent.ItemType<UnholyEssence>(), CalamityWorld.downedProvidence, 0.15f, 5, 10);
					DropHelper.DropItemCondition(player, potion, CalamityWorld.downedProvidence, 0.1f, 1, 2);
					break;

				case ItemID.DungeonFishingCrate:
					DropHelper.DropItemCondition(player, ItemID.Ectoplasm, NPC.downedPlantBoss, 0.1f, 1, 5);
					DropHelper.DropItemCondition(player, ModContent.ItemType<Phantoplasm>(), CalamityWorld.downedPolterghast, 0.1f, 1, 5);
					break;

				case ItemID.JungleFishingCrate:
					DropHelper.DropItemChance(player, ModContent.ItemType<MurkyPaste>(), 0.2f, 1, 3);
					DropHelper.DropItemCondition(player, ModContent.ItemType<BeetleJuice>(), Main.hardMode, 0.2f, 1, 3);
					DropHelper.DropItemCondition(player, ModContent.ItemType<TrapperBulb>(), Main.hardMode, 0.2f, 1, 3);
					DropHelper.DropItemCondition(player, ItemID.ChlorophyteBar, CalamityWorld.downedCalamitas || NPC.downedPlantBoss, 0.1f, 1, 3);
					DropHelper.DropItemCondition(player, ModContent.ItemType<DraedonBar>(), NPC.downedPlantBoss, 0.1f, 1, 3);
					DropHelper.DropItemCondition(player, ModContent.ItemType<PlagueCellCluster>(), NPC.downedGolemBoss, 0.2f, 3, 6);
					DropHelper.DropItemCondition(player, ModContent.ItemType<UeliaceBar>(), CalamityWorld.downedProvidence, 0.1f, 1, 3);
					break;

				case ItemID.FloatingIslandFishingCrate:
					DropHelper.DropItemCondition(player, ModContent.ItemType<AerialiteBar>(), CalamityWorld.downedHiveMind || CalamityWorld.downedPerforator, 0.1f, 1, 3);
					DropHelper.DropItemCondition(player, ModContent.ItemType<EssenceofCinder>(), Main.hardMode, 0.2f, 2, 4);
					DropHelper.DropItemCondition(player, ModContent.ItemType<GalacticaSingularity>(), NPC.downedMoonlord, 0.1f, 1, 3);
					break;
			}
		}
		#endregion
	}
}
