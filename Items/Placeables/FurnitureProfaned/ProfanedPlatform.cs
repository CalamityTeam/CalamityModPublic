using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureProfaned
{
    public class ProfanedPlatform : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 200;
        }

        public override void SetDefaults()
        {
            Item.width = 8;
            Item.height = 10;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurnitureProfaned.ProfanedPlatform>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).AddIngredient(ModContent.ItemType<ProfanedRock>()).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
