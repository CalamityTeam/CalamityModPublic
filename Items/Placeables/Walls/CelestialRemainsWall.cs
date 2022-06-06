using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Walls
{
    [LegacyName("AstralFossilWall")]
    public class CelestialRemainsWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 400;
            DisplayName.SetDefault("Celestial Remains Wall");
        }

        public override void SetDefaults()
        {
            Item.createWall = ModContent.WallType<WallTiles.CelestialRemainsWall>();
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
            CreateRecipe(4).AddTile(TileID.WorkBenches).AddIngredient(ModContent.ItemType<CelestialRemains>()).Register();
        }
    }
}
