using CalamityMod.Items.Placeables.Walls;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables
{
    public class AstralDirt : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Dirt");
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.Astral.AstralDirt>();
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
            CreateRecipe(1).AddTile(TileID.WorkBenches).AddIngredient(ModContent.ItemType<AstralDirtWall>(), 4).Register();
        }
    }
}
