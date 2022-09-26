using CalamityMod.Tiles.Furniture.DevPaintings;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture.DevPaintings
{
	public class NincityPainting : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Developer Painting");
			Tooltip.SetDefault("~ Nincity ~\n" +
			"Thanks to the entire team, everyone who supported, and those who all play the mod and keep it alive!");
            SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.width = Item.height = 96;
			Item.maxStack = 9999;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.value = Item.buyPrice(0, 5, 0, 0);
			Item.rare = ItemRarityID.Blue;
			Item.createTile = ModContent.TileType<NincityPaintingTile>();
		}
	}
}
