using CalamityMod.Items.Materials;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items
{
    public class RustyLockpick : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Opens locked rusty chests");
        }

        public override void SetDefaults()
        {
            item.width = 14;
            item.height = 28;
            item.maxStack = 99;
            item.rare = 1;
            item.value = CalamityGlobalItem.Rarity1BuyPrice;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SulfuricScale>(), 15);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
