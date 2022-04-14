
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
namespace CalamityMod.Items.Placeables.FurnitureCosmilite
{
    public class CosmiliteClock : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurnitureCosmilite.CosmiliteClock>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.IronBar, 3).AddIngredient(ItemID.Glass, 6).AddIngredient(ModContent.ItemType<CosmiliteBrick>(), 10).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
        }
    }
}
