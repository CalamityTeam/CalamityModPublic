using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.FurnitureEutrophic
{
    public class EutrophicBench : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 20;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurnitureEutrophic.EutrophicBench>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<Navystone>(), 10).AddTile(ModContent.TileType<EutrophicCrafting>()).Register();
        }
    }
}
