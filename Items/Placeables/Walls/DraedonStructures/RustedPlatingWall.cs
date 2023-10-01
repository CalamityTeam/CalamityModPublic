using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls.DraedonStructures;
using TileItems = CalamityMod.Items.Placeables.DraedonStructures;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Walls.DraedonStructures
{
    public class RustedPlatingWall : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 7;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createWall = ModContent.WallType<WallTiles.RustedPlatingWall>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<TileItems.RustedPlating>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
