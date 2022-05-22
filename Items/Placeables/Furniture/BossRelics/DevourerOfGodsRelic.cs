using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture.BossRelics
{
	public class DevourerOfGodsRelic : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Devourer of Gods Relic");
            Tooltip.SetDefault("A glimpse into what will be..");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			// Vanilla has many useful methods like these, use them! This substitutes setting Item.createTile and Item.placeStyle aswell as setting a few values that are common across all placeable items
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.BossRelics.DevourerOfGodsRelic>(), 0);

			Item.width = 30;
			Item.height = 40;
			Item.maxStack = 99;
			Item.rare = ItemRarityID.Master;
			Item.master = true; // This makes sure that "Master" displays in the tooltip, as the rarity only changes the item name color
			Item.value = Item.buyPrice(0, 5);
		}
	}
}
