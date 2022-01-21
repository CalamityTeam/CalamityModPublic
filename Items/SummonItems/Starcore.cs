using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Potions;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class Starcore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starcore");
            Tooltip.SetDefault("May the stars guide your way\n" +
                "Summons Astrum Deus at the Astral Beacon, but is not consumed");
        }

        public override void SetDefaults()
        {
            item.width = 34;
            item.height = 40;
            item.rare = ItemRarityID.Cyan;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Stardust>(), 25);
            recipe.AddIngredient(ModContent.ItemType<AstralJelly>(), 8);
            recipe.AddIngredient(ModContent.ItemType<AstralBar>(), 4);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
