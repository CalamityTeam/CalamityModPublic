using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class MeldiateBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meld Construct");
        }

        public override void SetDefaults()
        {
            item.width = 15;
            item.height = 12;
            item.maxStack = 999;
            item.value = Item.sellPrice(gold: 1, silver: 20);
            item.rare = ItemRarityID.Cyan;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MeldBlob>(), 6);
            recipe.AddIngredient(ModContent.ItemType<Stardust>(), 3);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 3);
            recipe.AddRecipe();
        }
    }
}
