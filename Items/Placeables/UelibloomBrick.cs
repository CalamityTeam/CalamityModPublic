using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Items.Placeables.FurnitureBotanic;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
namespace CalamityMod.Items.Placeables
{
    public class UelibloomBrick : ModItem
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
            Item.createTile = ModContent.TileType<Tiles.UelibloomBrick>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).AddIngredient(ModContent.ItemType<UelibloomOre>()).AddRecipeGroup("AnyStoneBlock").AddTile(TileID.AdamantiteForge).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<UelibloomBrickWall>(), 4).AddTile(ModContent.TileType<BotanicPlanter>()).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<BotanicPlatform>(), 2).AddTile(ModContent.TileType<BotanicPlanter>()).Register();
        }
    }
}
