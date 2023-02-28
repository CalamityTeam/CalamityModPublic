using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Items.Materials;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureWulfrum
{
    public class WulfrumPlating : ModItem
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
            Item.createTile = ModContent.TileType<Tiles.FurnitureWulfrum.WulfrumPlating>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(25).
                AddRecipeGroup("AnyStoneBlock", 25).
                AddIngredient<WulfrumMetalScrap>().
                AddTile(TileID.HeavyWorkBench).
                Register();
            //CreateRecipe().
                //AddIngredient<WulfrumPlatingWall>(4).
                //AddTile(TileID.WorkBenches).
                //Register();
        }
    }
}
