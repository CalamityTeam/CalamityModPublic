using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
namespace CalamityMod.Items.Placeables.Walls
{
    public class SulphurousShaleWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 400;
            DisplayName.SetDefault("Sulphurous Shale Wall");
        }

        public override void SetDefaults()
        {
            Item.createWall = ModContent.WallType<WallTiles.SulphurousShaleWallSafe>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
        }

        public override void AddRecipes()
        {
            CreateRecipe(4).AddTile(
                TileID.WorkBenches).AddIngredient(ModContent.ItemType<SulphurousShale>()).Register();
        }
    }
}
