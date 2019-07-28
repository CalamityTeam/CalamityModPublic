using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class AmidiasSpark : ModItem
	{
		public override void SetStaticDefaults()
		{
				DisplayName.SetDefault("Amidias' Spark");
				Tooltip.SetDefault("Taking damage releases a blast of sparks\n" +
								   "Sparks do extra damage in Hardmode");
		}

		public override void SetDefaults()
		{
			item.width = 26;
			item.height = 26;
			item.value = Item.buyPrice(0, 3, 0, 0);
			item.rare = 1;
			item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.aSpark = true;
		}
	}
}
