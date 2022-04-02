using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class BloodstoneCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodstone Core");
        }

        public override void SetDefaults()
        {
            item.width = 15;
            item.height = 12;
            item.maxStack = 999;
            item.value = Item.sellPrice(gold: 4);
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Bloodstone>(), 5);
            recipe.AddIngredient(ModContent.ItemType<BloodOrb>());
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>());
            recipe.AddTile(TileID.AdamantiteForge);
            recipe.SetResult(this, 2);
            recipe.AddRecipe();
        }
    }
}
