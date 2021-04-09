using CalamityMod.Items.Materials;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.Furniture.CraftingStations
{
    public class DraedonsForge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draedon's Forge");
            Tooltip.SetDefault("Used to craft uber-tier items");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.value = 5000000;
            item.rare = ItemRarityID.Red;
            item.createTile = ModContent.TileType<Tiles.Furniture.CraftingStations.DraedonsForge>();
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("HardmodeForge");
            recipe.AddRecipeGroup("HardmodeAnvil");
            recipe.AddIngredient(ItemID.LunarCraftingStation);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 20);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 20);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 20);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
