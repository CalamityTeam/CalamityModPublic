using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureProfaned
{
    public class ProfanedSlab : ModItem
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
            Item.createTile = ModContent.TileType<Tiles.FurnitureProfaned.ProfanedSlab>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(5).AddIngredient(ModContent.ItemType<ProfanedRock>(), 5).AddTile(TileID.LunarCraftingStation).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<ProfanedSlabWall>(), 4).AddTile(TileID.WorkBenches).Register();
        }
    }
}
