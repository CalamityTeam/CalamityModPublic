using CalamityMod.Items.Placeables.Walls.DraedonStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.DraedonStructures
{
    public class HazardChevronPanels : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.DraedonStructures.HazardChevronPanels>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient<LaboratoryPanels>(10).
                AddIngredient(ItemID.YellowPaint).
                AddIngredient(ItemID.BlackPaint).
                AddTile(TileID.WorkBenches).
                Register();
            CreateRecipe().
                AddIngredient<HazardChevronWall>(4).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
