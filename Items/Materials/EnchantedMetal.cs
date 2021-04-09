using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class EnchantedMetal : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enchanted Metal");
        }
        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 20;
            item.value = Item.buyPrice(0, 15, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.maxStack = 99;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(item.type, 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(ItemID.MechanicalEye);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(item.type, 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(ItemID.MechanicalSkull);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(item.type, 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(ItemID.MechanicalWorm);
            recipe.AddRecipe();
        }
    }
}
