using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.FurnitureSacrilegious;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureSacrilegious
{
    public class SacrilegiousBookcase : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sacrilegious Bookcase");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 36;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<SacrilegiousBookcaseTile>();
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
				AddIngredient(ModContent.ItemType<OccultBrickItem>(), 20).
				AddIngredient(ItemID.Book, 10).
				AddTile(ModContent.TileType<SCalAltar>()).
				Register();
        }
    }
}
