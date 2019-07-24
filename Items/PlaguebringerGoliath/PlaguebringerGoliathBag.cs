using Terraria;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.Items.PlaguebringerGoliath
{
	public class PlaguebringerGoliathBag : ModItem
	{
		public override int BossBagNPC => mod.NPCType("PlaguebringerGoliath");

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
			item.rare = 9;
			item.expert = true;
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
			player.TryGettingDevArmor();

			// Materials
			DropHelper.DropItem(player, mod.ItemType("PlagueCellCluster"), 13, 17);

			// Weapons
			DropHelper.DropItemChance(player, mod.ItemType("VirulentKatana"), 3); // Virulence
			DropHelper.DropItemChance(player, mod.ItemType("DiseasedPike"), 3);
			DropHelper.DropItemChance(player, mod.ItemType("ThePlaguebringer"), 3); // Pandemic
			DropHelper.DropItemChance(player, mod.ItemType("Malevolence"), 3);
			DropHelper.DropItemChance(player, mod.ItemType("PestilentDefiler"), 3);
			DropHelper.DropItemChance(player, mod.ItemType("TheHive"), 3);
			DropHelper.DropItemChance(player, mod.ItemType("MepheticSprayer"), 3); // Blight Spewer
			DropHelper.DropItemChance(player, mod.ItemType("PlagueStaff"), 3);
			float malachiteChance = CalamityWorld.defiled ? DropHelper.DefiledDropRateFloat : DropHelper.LegendaryDropRateFloat;
			DropHelper.DropItemCondition(player, mod.ItemType("Malachite"), CalamityWorld.revenge, malachiteChance);

			// Equipment
			DropHelper.DropItem(player, mod.ItemType("ToxicHeart"));
			DropHelper.DropItemChance(player, mod.ItemType("BloomStone"), 10);

			// Vanity
			DropHelper.DropItemChance(player, mod.ItemType("PlaguebringerGoliathMask"), 7);
		}
	}
}