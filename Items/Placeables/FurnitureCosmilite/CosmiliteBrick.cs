using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
namespace CalamityMod.Items.Placeables.FurnitureCosmilite
{
    public class CosmiliteBrick : ModItem
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
            Item.createTile = ModContent.TileType<Tiles.FurnitureCosmilite.CosmiliteBrick>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(20).AddIngredient(ModContent.ItemType<CosmiliteBar>(), 1).AddRecipeGroup("AnyStoneBlock", 20).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<CosmiliteBrickWall>(), 4).AddTile(TileID.WorkBenches).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<CosmilitePlatform>(), 2).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
        }
    }
}
