using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.FabsolStuff
{
	public class PinkCandle: ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Vigorous Candle");
			Tooltip.SetDefault("When placed down nearby players regenerate 0.4% of their maximum health per second\n" +
							   "This is separate from normal life regeneration, bypassing caps and not being affected by damage taken or current movement");
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
			item.createTile = mod.TileType("PinkCandle");
		}
    }
}