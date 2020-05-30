using CalamityMod.Items.Placeables.Walls;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables
{
    public class AstralIce : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Ice");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<Tiles.AstralSnow.AstralIce>();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 16;
            item.height = 16;
            item.maxStack = 999;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddTile(TileID.WorkBenches);
            recipe.AddIngredient(ModContent.ItemType<AstralIceWall>(), 4);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
            base.AddRecipes();
        }
    }
}
