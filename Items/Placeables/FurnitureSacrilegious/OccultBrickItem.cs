using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.FurnitureSacrilegious;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureSacrilegious
{
    public class OccultBrickItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Occult Brick");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<OccultBrickTile>();
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void AddRecipes()
        {
            CreateRecipe(200).
				AddRecipeGroup("AnyStoneBlock", 200).
				AddIngredient(ModContent.ItemType<AshesofAnnihilation>()).
				AddTile(ModContent.TileType<CosmicAnvil>()).
				Register();

            CreateRecipe(1).
				AddIngredient(ModContent.ItemType<OccultPlatformItem>(), 2).
				Register();

            CreateRecipe(1).
				AddIngredient(ModContent.ItemType<OccultBrickWallItem>(), 4).
				AddTile(TileID.WorkBenches).
				Register();
        }
    }
}
