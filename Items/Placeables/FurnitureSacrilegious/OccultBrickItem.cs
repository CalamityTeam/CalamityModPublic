using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.FurnitureSacrilegious;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureSacrilegious
{
    public class OccultBrickItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Occult Brick");
            SacrificeTotal = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<OccultBrickTile>();
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(400).
				AddRecipeGroup("AnyStoneBlock", 400).
				AddIngredient<AshesofAnnihilation>().
				AddTile<SCalAltar>().
				Register();
            CreateRecipe().
				AddIngredient<OccultPlatformItem>(2).
				Register();
            CreateRecipe().
				AddIngredient<OccultBrickWallItem>(4).
				AddTile(TileID.WorkBenches).
				Register();
        }
    }
}
