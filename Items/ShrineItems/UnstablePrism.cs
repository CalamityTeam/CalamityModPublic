using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.ShrineItems
{
    public class UnstablePrism : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Unstable Prism");
			Tooltip.SetDefault("Three sparks are released on critical hits");
		}

		public override void SetDefaults()
		{
			item.width = 22;
			item.height = 38;
			item.value = Item.buyPrice(0, 9, 0, 0);
			item.rare = 3;
			item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.unstablePrism = true;
		}
	}
}
