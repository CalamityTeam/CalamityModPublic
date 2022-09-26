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
		}

		public override void SetDefaults()
		{
			item.width = item.height = 96;
			item.maxStack = 9999;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.consumable = true;
			item.value = Item.buyPrice(0, 5, 0, 0);;
			item.Calamity().customRarity = CalamityRarity.Dedicated;
			item.createTile = ModContent.TileType<NincityPaintingTile>();
		}
	}
}
