using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
namespace CalamityMod.Items.Placeables
{
    public class CryonicBrick : ModItem
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
            Item.createTile = ModContent.TileType<Tiles.CryonicBrick>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).AddIngredient(ModContent.ItemType<CryonicOre>()).AddRecipeGroup("AnyStoneBlock").AddTile(TileID.Furnaces).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<CryonicBrickWall>(), 4).AddTile(TileID.WorkBenches).Register();
        }
    }
}
