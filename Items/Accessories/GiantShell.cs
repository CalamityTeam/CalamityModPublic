using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class GiantShell : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Giant Shell");
			Tooltip.SetDefault("15% reduced movement speed\n" +
				"Taking a hit will make you move very fast for a short time");
		}

		public override void SetDefaults()
		{
			item.defense = 6;
			item.width = 20;
			item.height = 24;
            item.value = Item.buyPrice(0, 3, 0, 0);
            item.rare = 1;
			item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.moveSpeed -= 0.15f;
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.gShell = true;
		}
	}
}
