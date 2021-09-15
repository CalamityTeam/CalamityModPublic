using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture.CraftingStations
{
    public class DraedonsForge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draedon's Forge");
            Tooltip.SetDefault("A plasma-lattice nanoforge powered by limitless Exo energies\n" +
                "Functions as every major crafting station simultaneously");
        }

        public override void SetDefaults()
        {
            item.width = 58;
            item.height = 32;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.Furniture.CraftingStations.DraedonsForge>();

            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.value = Item.sellPrice(platinum: 27, gold: 50);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CosmicAnvilItem>());
            recipe.AddRecipeGroup("HardmodeForge");
            recipe.AddIngredient(ItemID.TinkerersWorkshop);
            recipe.AddIngredient(ItemID.LunarCraftingStation);
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 15);
            recipe.AddIngredient(ModContent.ItemType<ExoPrism>(), 12);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 25);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
