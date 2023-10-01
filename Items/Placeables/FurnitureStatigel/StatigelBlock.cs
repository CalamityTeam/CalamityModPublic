using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureStatigel
{
    public class StatigelBlock : ModItem, ILocalizedModType
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
            Item.createTile = ModContent.TileType<Tiles.FurnitureStatigel.StatigelBlock>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(25).
                AddIngredient<PurifiedGel>().
                AddTile<StaticRefiner>().
                Register();
            CreateRecipe().
                AddIngredient<StatigelPlatform>(2).
                Register();
            CreateRecipe().
                AddIngredient<StatigelWall>(4).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
