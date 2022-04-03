using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables
{
    public class ChaoticBrick : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 25;
            Item.maxStack = 999;
            Item.value = 0;
            Item.rare = ItemRarityID.Blue;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.ChaoticBrick>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).AddRecipeGroup("AnyStoneBlock").AddIngredient(ModContent.ItemType<ChaoticOre>()).AddTile(TileID.Furnaces).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<ChaoticBrickWall>(), 4).AddTile(TileID.WorkBenches).Register();
        }
    }
}
