using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class Knowledge16 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Queen Bee");
			Tooltip.SetDefault("As crude as the giant insects are they can prove useful in certain situations...given the ability to control them.\n" +
				"Place in your inventory to make small bees and weak hornets friendly.");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 3;
			item.consumable = false;
		}

		public override bool CanUseItem(Player player)
		{
			return false;
		}

		public override void UpdateInventory(Player player)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.queenBeeLore = true;
		}
	}
}
