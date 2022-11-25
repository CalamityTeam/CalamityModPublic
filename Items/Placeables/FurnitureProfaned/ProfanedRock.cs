using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureProfaned
{
    public class ProfanedRock : ModItem
    {
        public override void SetStaticDefaults()
        {
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
            Item.createTile = ModContent.TileType<Tiles.FurnitureProfaned.ProfanedRock>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(100).
                AddIngredient<UnholyEssence>().
                AddRecipeGroup("AnyStoneBlock", 100).
                AddTile(TileID.LunarCraftingStation).
                Register();
            CreateRecipe().
                AddIngredient<ProfanedRockWall>(4).
                AddTile(TileID.WorkBenches).
                Register();
            CreateRecipe().
                AddIngredient<ProfanedPlatform>(2).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
