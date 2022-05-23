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
            CreateRecipe(400).AddIngredient(ItemID.CrystalBlock, 200).AddRecipeGroup("AnyGoldBar", 25).AddIngredient(ModContent.ItemType<EffulgentFeather>(), 5).AddIngredient(ModContent.ItemType<DarksunFragment>()).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<SilvaWall>(), 4).AddTile(ModContent.TileType<SilvaBasin>()).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<SilvaPlatform>(), 2).AddTile(ModContent.TileType<SilvaBasin>()).Register();
        }
    }
}
