using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;

namespace CalamityMod.Items.Placeables.Furniture.CraftingStations
{
    [LegacyName("EutrophicCrafting")]
    public class EutrophicShelf : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            Tooltip.SetDefault("Used for special crafting");
        }

        public override void SetDefaults()
        {
            Item.SetNameOverride("Eutrophic Shelf");
            Item.width = 28;
            Item.height = 20;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Furniture.CraftingStations.EutrophicShelf>();
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.CraftingObjects;
		}

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Navystone>(10).
                AddIngredient<SeaPrism>(5).
                AddIngredient<PrismShard>(5).
                AddIngredient<PearlShard>(3).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
