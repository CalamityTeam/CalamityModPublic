using Terraria;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.Items.Scavenger
{
    public class RavagerBag : ModItem
	{
		public override int BossBagNPC => mod.NPCType("ScavengerBody");

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Treasure Bag");
			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
		}

		public override void SetDefaults()
		{
			item.maxStack = 999;
			item.consumable = true;
			item.width = 24;
			item.height = 24;
			item.expert = true;
			item.rare = 9;
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
			player.TryGettingDevArmor();

			// Materials
			int barMin = CalamityWorld.downedProvidence ? 7 : 2;
			int barMax = CalamityWorld.downedProvidence ? 12 : 3;
			int coreMin = CalamityWorld.downedProvidence ? 2 : 1;
			int coreMax = CalamityWorld.downedProvidence ? 4 : 3;
			DropHelper.DropItemCondition(player, mod.ItemType("Bloodstone"), CalamityWorld.downedProvidence, 60, 70);
			DropHelper.DropItem(player, mod.ItemType("VerstaltiteBar"), barMin, barMax);
			DropHelper.DropItem(player, mod.ItemType("DraedonBar"), barMin, barMax);
			DropHelper.DropItem(player, mod.ItemType("CruptixBar"), barMin, barMax);
			DropHelper.DropItem(player, mod.ItemType("CoreofCinder"), coreMin, coreMax);
			DropHelper.DropItem(player, mod.ItemType("CoreofEleum"), coreMin, coreMax);
			DropHelper.DropItem(player, mod.ItemType("CoreofChaos"), coreMin, coreMax);
			DropHelper.DropItemCondition(player, mod.ItemType("BarofLife"), CalamityWorld.downedProvidence);
			DropHelper.DropItemCondition(player, mod.ItemType("CoreofCalamity"), CalamityWorld.downedProvidence, 0.5f);

			// Weapons
			DropHelper.DropItemFromSet(player,
				mod.ItemType("UltimusCleaver"),
				mod.ItemType("RealmRavager"),
				mod.ItemType("Hematemesis"),
				mod.ItemType("SpikecragStaff"),
				mod.ItemType("CraniumSmasher"));

            // Equipment
            DropHelper.DropItemFromSetChance(player, 0.05f, mod.ItemType("CorpusAvertorMelee"), mod.ItemType("CorpusAvertor"));
			DropHelper.DropItemChance(player, mod.ItemType("BloodPact"), 0.5f);
			DropHelper.DropItemChance(player, mod.ItemType("FleshTotem"), 0.5f);
			DropHelper.DropItemCondition(player, mod.ItemType("BloodflareCore"), CalamityWorld.downedProvidence);
			DropHelper.DropItemCondition(player, mod.ItemType("InfernalBlood"), CalamityWorld.revenge);

			// Vanity
			// there is no Ravager mask yet
		}
	}
}
