using CalamityMod.Tiles.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Ores
{
    public class ExodiumClusterOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exodium Cluster");
            Tooltip.SetDefault("A cold cluster from the great unknown.");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<ExodiumOre>();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 13;
            item.height = 10;
            item.maxStack = 999;
            item.value = Item.sellPrice(gold: 1, silver: 60);
            item.rare = ItemRarityID.Red;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LunarOre, 3);
            recipe.AddIngredient(ItemID.FragmentStardust);
            recipe.AddIngredient(ItemID.FragmentSolar);
            recipe.AddIngredient(ItemID.FragmentVortex);
            recipe.AddIngredient(ItemID.FragmentNebula);
            recipe.SetResult(this);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.AddRecipe();
        }
    }
}
