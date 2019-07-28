using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.FabsolStuff
{
    public class YellowCandle : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spiteful Candle");
			Tooltip.SetDefault("When placed, nearby enemies take 5% more damage.\n" +
				"This extra damage bypasses enemy damage reduction\n" +
				"'Its hateful glow flickers with ire'");
		}

		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 40;
			item.maxStack = 99;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
			item.consumable = true;
			item.value = Item.buyPrice(0, 50, 0, 0);
			item.rare = 6;
			item.createTile = mod.TileType("YellowCandle");
		}
    }
}
