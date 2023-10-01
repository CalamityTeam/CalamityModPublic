using CalamityMod.Items.Placeables.FurnitureAshen;
using Terraria.ModLoader;
using Terraria.ID;
using WallTiles = CalamityMod.Walls;
namespace CalamityMod.Items.Placeables.Walls
{
    public class SmoothBrimstoneSlagWall : ModItem, ILocalizedModType
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
            Item.rare = ItemRarityID.Orange;
            Item.consumable = true;
            Item.createWall = ModContent.WallType<WallTiles.SmoothBrimstoneSlagWall>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<SmoothBrimstoneSlag>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
