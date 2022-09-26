using Terraria.ModLoader;
using Terraria.ID;
namespace CalamityMod.Items.Placeables.FurnitureOtherworldly
{
    [LegacyName("OccultPlatform")]
    public class OtherworldlyPlatform : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 200;
        }

        public override void SetDefaults()
        {
            Item.SetNameOverride("Otherworldly Platform");
            Item.width = 8;
            Item.height = 10;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurnitureOtherworldly.OtherworldlyPlatform>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).AddIngredient(ModContent.ItemType<OtherworldlyStone>()).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
