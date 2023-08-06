using CalamityMod.Items.Placeables.FurnitureSacrilegious;
using CalamityMod.Walls;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Walls
{
    public class OccultBrickWallItem : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 7;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createWall = ModContent.WallType<OccultBrickWall>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(4).
            AddIngredient(ModContent.ItemType<OccultBrickItem>()).
            AddTile(TileID.WorkBenches).
            Register();
        }
    }
}
