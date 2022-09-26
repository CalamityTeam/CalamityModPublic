using CalamityMod.Items.Placeables.Walls;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables
{
    public class AstralSnow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Snow");
            SacrificeTotal = 100;
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.AstralSnow.AstralSnow>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstralSnowWall>(4).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
