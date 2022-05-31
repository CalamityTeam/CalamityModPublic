using CalamityMod.Items.Placeables.FurnitureOtherworldly;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
using Terraria.ID;
namespace CalamityMod.Items.Placeables.Walls
{
    public class OtherworldlyStoneWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 400;
        }

        public override void SetDefaults()
        {
            Item.SetNameOverride("Otherworldly Stone Wall");
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 7;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createWall = ModContent.WallType<WallTiles.OtherworldlyStoneWall>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(4).AddIngredient(ModContent.ItemType<OtherworldlyStone>()).AddTile(TileID.WorkBenches).Register();
        }
    }
}
