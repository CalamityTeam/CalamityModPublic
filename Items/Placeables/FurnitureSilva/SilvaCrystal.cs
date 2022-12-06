using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureSilva
{
    public class SilvaCrystal : ModItem
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
            Item.createTile = ModContent.TileType<Tiles.FurnitureSilva.SilvaCrystal>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(400).
                AddIngredient(ItemID.Glass, 400).
                AddIngredient<PlantyMush>(5).
                AddIngredient<EffulgentFeather>().
                AddIngredient<AscendantSpiritEssence>().
                AddTile<CosmicAnvil>().
                Register();
            CreateRecipe().
                AddIngredient<SilvaWall>(4).
                AddTile<SilvaBasin>().
                Register();
            CreateRecipe().
                AddIngredient<SilvaPlatform>(2).
                AddTile<SilvaBasin>().
                Register();
        }
    }
}
