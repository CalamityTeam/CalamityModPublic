using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace CalamityMod.Items.Materials
{
    public class ShadowspecBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowspec Bar");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(8, 10));
        }

        public override void SetDefaults()
        {
            item.width = 15;
            item.height = 12;
            item.maxStack = 999;
            item.rare = 10;
            item.value = Item.sellPrice(gold: 27, silver: 50);
            item.Calamity().postMoonLordRarity = 15;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 3);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 3);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 3);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 3);
            recipe.AddIngredient(ModContent.ItemType<CalamitousEssence>());
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>());
            recipe.AddIngredient(ModContent.ItemType<HellcasterFragment>());
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this, 3);
            recipe.AddRecipe();
        }
    }
}
