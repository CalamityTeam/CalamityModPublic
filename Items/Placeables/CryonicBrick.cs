using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables
{
    public class CryonicBrick : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
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
            Item.createTile = ModContent.TileType<Tiles.CryonicBrick>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient<CryonicOre>().
                AddRecipeGroup("AnyStoneBlock").
                AddTile(TileID.Furnaces).
                Register();

            CreateRecipe().
                AddIngredient<CryonicBrickWall>(4).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
