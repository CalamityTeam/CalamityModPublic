
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class AstralBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Bar");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<Tiles.AstralBar>();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 16;
            item.height = 16;
            item.maxStack = 99;
            item.rare = ItemRarityID.Cyan;
            item.value = Item.sellPrice(gold: 1, silver: 20);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Stardust>(), 3);
            recipe.AddIngredient(ModContent.ItemType<AstralOre>(), 2);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
