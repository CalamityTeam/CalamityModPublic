using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables
{
    [LegacyName("ChaoticBrick")]
    public class ScoriaBrick : ModItem
    {
        public override void SetStaticDefaults() => SacrificeTotal = 100;

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
            Item.createTile = ModContent.TileType<Tiles.ScoriaBrick>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(50).
                AddRecipeGroup("AnyStoneBlock", 50).
                AddIngredient<ScoriaOre>().
                AddTile(TileID.Furnaces).
                Register();
            CreateRecipe().
                AddIngredient<ScoriaBrickWall>(4).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
