using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls.DraedonStructures;
using TileItems = CalamityMod.Items.Placeables.DraedonStructures;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Walls.DraedonStructures
{
    public class RustedPlateBeam : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 7;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createWall = ModContent.WallType<WallTiles.RustedPlateBeam>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(4).AddIngredient(ModContent.ItemType<TileItems.RustedPlating>()).AddTile(TileID.WorkBenches).Register();
        }
    }
}
