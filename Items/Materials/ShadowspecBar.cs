using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class ShadowspecBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowspec Bar");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(8, 8));
        }

        public override void SetDefaults()
        {
            item.width = 15;
            item.height = 12;
            item.maxStack = 999;
            item.rare = 10;
            item.value = Item.sellPrice(gold: 70);
            item.Calamity().postMoonLordRarity = 15;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AuricBar>());
            recipe.AddIngredient(ModContent.ItemType<CalamitousEssence>());
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
