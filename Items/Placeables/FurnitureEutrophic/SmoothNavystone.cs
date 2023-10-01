using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader; // If you are using c# 6, you can use: "using static Terraria.Localization.GameCulture;" which would mean you could just write "DisplayName.AddTranslation(German, "");"

namespace CalamityMod.Items.Placeables.FurnitureEutrophic
{
    public class SmoothNavystone : ModItem, ILocalizedModType
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
            Item.createTile = ModContent.TileType<Tiles.FurnitureEutrophic.SmoothNavystone>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Navystone>().
                AddTile(TileID.WorkBenches).
                Register();
            CreateRecipe().
                AddIngredient<SmoothNavystoneWall>(4).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
