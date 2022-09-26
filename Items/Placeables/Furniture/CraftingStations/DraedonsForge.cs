using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture.CraftingStations
{
    public class DraedonsForge : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Draedon's Forge");
            Tooltip.SetDefault("A plasma-lattice nanoforge powered by limitless Exo energies\n" +
                "Functions as every major crafting station simultaneously");
        }

        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 32;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Furniture.CraftingStations.DraedonsForge>();

            Item.rare = ModContent.RarityType<Violet>();
            Item.value = Item.sellPrice(platinum: 27, gold: 50);
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.CraftingObjects;
		}

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<CosmicAnvilItem>()).AddRecipeGroup("HardmodeForge").AddIngredient(ItemID.TinkerersWorkshop).AddIngredient(ItemID.LunarCraftingStation).AddIngredient(ModContent.ItemType<AuricBar>(), 15).AddIngredient(ModContent.ItemType<ExoPrism>(), 12).AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 25).Register();
        }
    }
}
