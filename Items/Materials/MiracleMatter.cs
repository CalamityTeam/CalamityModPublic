using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class MiracleMatter : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Miracle Matter");
            Tooltip.SetDefault("Its amorphous form contains untold potential\n" + "One is required for every Exo Weapon");
        }

        public override void SetDefaults()
        {
            item.width = 46;
            item.height = 64;
            item.maxStack = 999;
            item.rare = ItemRarityID.Purple;
            item.value = Item.sellPrice(platinum: 6, gold: 50);
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        // TODO -- Miracle Matter should probably look cooler than it does. Right now it just glows in the dark.
        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 0);

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ExoPrism>(), 5);
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>(), 1);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 1);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 3);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
