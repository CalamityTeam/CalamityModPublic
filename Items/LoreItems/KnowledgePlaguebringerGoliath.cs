using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgePlaguebringerGoliath : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Plaguebringer Goliath");
			Tooltip.SetDefault("A horrific amalgam of steel, flesh, and infection, capable of destroying an entire civilization in just one onslaught.\n" +
				"Its plague nuke barrage can leave an entire area uninhabitable for months. A shame that it came to this but the plague must be contained.\n" +
				"Place in your inventory to gain increased wing flight time but at the cost of reduced life regen.");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 8;
			item.consumable = false;
		}

		public override bool CanUseItem(Player player)
		{
			return false;
		}

		public override void UpdateInventory(Player player)
		{
			CalamityPlayer modPlayer = player.GetCalamityPlayer();
			modPlayer.plaguebringerGoliathLore = true;
		}
	}
}
