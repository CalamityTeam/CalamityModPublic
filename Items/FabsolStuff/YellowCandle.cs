using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.FabsolStuff
{
	public class YellowCandle: ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spiteful Candle");
			Tooltip.SetDefault("When placed down nearby enemies take 5% more damage, this bypasses enemy DR");
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