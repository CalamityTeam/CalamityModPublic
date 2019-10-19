using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureVoid
{
    public class VoidChandelier : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.VoidChandelier>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SmoothVoidstone>(), 4);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 4);
            recipe.AddIngredient(ItemID.Chain);
            recipe.SetResult(this, 1);
            recipe.AddTile(ModContent.TileType<VoidCondenser>());
            recipe.AddRecipe();
        }
    }
}
