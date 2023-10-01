using CalamityMod.Items.Placeables.Walls.DraedonStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.DraedonStructures
{
    public class LaboratoryPlating : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.DraedonStructures.LaboratoryPlating>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(25).
                AddRecipeGroup("AnyStoneBlock", 25).
                AddRecipeGroup("IronBar").
                AddTile(TileID.HeavyWorkBench).
                Register();

            CreateRecipe().
                AddIngredient<RustedPlating>().
                AddTile(TileID.Anvils).
                Register();

            CreateRecipe().
                AddIngredient<LaboratoryShelf>(2).
                Register();

            CreateRecipe().
                AddIngredient<LaboratoryPlatingWall>(4).
                AddTile(TileID.WorkBenches).
                Register();

            CreateRecipe().
                AddIngredient<LaboratoryPlateBeam>(4).
                AddTile(TileID.WorkBenches).
                Register();

            CreateRecipe().
                AddIngredient<LaboratoryPlatePillar>(4).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
