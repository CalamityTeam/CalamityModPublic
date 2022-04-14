using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
namespace CalamityMod.Items.Placeables
{
    public class AerialiteBrick : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.AerialiteBrick>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).AddIngredient(ModContent.ItemType<AerialiteOre>(), 1).AddRecipeGroup("AnyStoneBlock").AddTile(TileID.Furnaces).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<AerialiteBrickWall>(), 4).AddTile(TileID.WorkBenches).Register();
        }
    }
}
