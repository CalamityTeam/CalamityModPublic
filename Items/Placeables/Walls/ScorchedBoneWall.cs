using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Walls
{
    public class ScorchedBoneWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scorched Bone Wall");
            SacrificeTotal = 400;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 7;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createWall = ModContent.WallType<WallTiles.ScorchedBoneWall>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(4).
            AddIngredient(ModContent.ItemType<ScorchedBone>()).
            AddTile(TileID.WorkBenches).
            Register();
        }
    }
}
