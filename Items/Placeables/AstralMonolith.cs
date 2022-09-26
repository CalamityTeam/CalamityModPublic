using CalamityMod.Items.Placeables.FurnitureMonolith;
using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class AstralMonolith : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Astral.AstralMonolith>();
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			// It's not really wood... but it comes from trees!
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.Wood;
		}

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstralMonolithWall>(4).
                AddTile<MonolithAmalgam>().
                Register();

            CreateRecipe().
                AddIngredient<MonolithPlatform>(2).
                Register();
        }
    }
}
