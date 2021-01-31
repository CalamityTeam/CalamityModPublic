using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class VictideBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Victide Bar");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 24;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 6);
            item.rare = ItemRarityID.Green;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VictoryShard>());
            recipe.AddIngredient(ItemID.Coral);
            recipe.AddIngredient(ItemID.Starfish);
            recipe.AddIngredient(ItemID.Seashell);
            recipe.AddTile(TileID.Furnaces);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
