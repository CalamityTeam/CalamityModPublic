using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureStratus
{
    public class StratusBricks : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.StratusBricks>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.StoneBlock, 40);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 3);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 1);
            recipe.SetResult(this, 40);
            recipe.AddTile(412);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<StratusWall>(), 4);
            recipe.SetResult(this, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<StratusPlatform>(), 2);
            recipe.SetResult(this);
            recipe.AddTile(412);
            recipe.AddRecipe();
        }
    }
}
