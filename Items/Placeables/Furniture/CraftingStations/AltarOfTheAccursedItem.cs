using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture.CraftingStations
{
    [LegacyName("SCalAltarItem")]
    public class AltarOfTheAccursedItem : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Placeables";
        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<SCalAltarLarge>();
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 38;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ModContent.RarityType<Violet>();
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.CraftingObjects;
		}

        public override void AddRecipes()
        {
            CreateRecipe().
				AddIngredient<BrimstoneSlag>(30).
				AddIngredient<AuricBar>(5).
				AddIngredient<CoreofCalamity>().
				AddTile<CosmicAnvil>().
				Register();
        }
    }
}
