using CalamityMod.Items.Placeables.Walls.DraedonStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.DraedonStructures
{
    public class LaboratoryPlating : ModItem
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
                AddIngredient(ModContent.ItemType<RustedPlating>()).
                AddTile(TileID.Anvils).
                Register();

            CreateRecipe().
                AddIngredient(ModContent.ItemType<LaboratoryShelf>(), 2).
                Register();

            CreateRecipe().
                AddIngredient(ModContent.ItemType<LaboratoryPlatingWall>(), 4).
                AddTile(TileID.WorkBenches).
                Register();

            CreateRecipe().
                AddIngredient(ModContent.ItemType<LaboratoryPlateBeam>(), 4).
                AddTile(TileID.WorkBenches).
                Register();

            CreateRecipe().
                AddIngredient(ModContent.ItemType<LaboratoryPlatePillar>(), 4).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
