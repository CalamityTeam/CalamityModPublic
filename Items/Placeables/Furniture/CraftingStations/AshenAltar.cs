using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.FurnitureAshen;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.Furniture.CraftingStations
{
    public class AshenAltar : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Orange;
            Item.value = 0;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Furniture.CraftingStations.AshenAltar>();
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.CraftingObjects;
		}

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SmoothBrimstoneSlag>(10).
                AddIngredient<UnholyCore>(10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
