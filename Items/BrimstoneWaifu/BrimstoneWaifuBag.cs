using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.World;

namespace CalamityMod.Items.BrimstoneWaifu
{
	public class BrimstoneWaifuBag : ModItem
	{
		public override int BossBagNPC => mod.NPCType("BrimstoneElemental");

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
			DropHelper.DropItem(player, ItemID.SoulofFright, 25, 40);
			DropHelper.DropItem(player, mod.ItemType("EssenceofChaos"), 5, 9);
			DropHelper.DropItemCondition(player, mod.ItemType("Bloodstone"), CalamityWorld.downedProvidence, 25, 35);

			// Weapons
			DropHelper.DropItemChance(player, mod.ItemType("Brimlance"), 3);
			DropHelper.DropItemChance(player, mod.ItemType("SeethingDischarge"), 3);

			// Equipment
			DropHelper.DropItem(player, mod.ItemType("Abaddon"));
			DropHelper.DropItem(player, mod.ItemType("Gehenna"));
			DropHelper.DropItemChance(player, mod.ItemType("RoseStone"), 10);
			DropHelper.DropItemCondition(player, mod.ItemType("Brimrose"), CalamityWorld.revenge && CalamityWorld.downedProvidence);

			// Vanity
			DropHelper.DropItemCondition(player, mod.ItemType("CharredRelic"), CalamityWorld.revenge);
		}
	}
}