using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Abyss;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.Furniture
{
    public class RustyChest : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = 500;
            Item.createTile = ModContent.TileType<RustyChestTile>();
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Chest).AddIngredient(ModContent.ItemType<SulphuricScale>(), 5).AddTile(TileID.Anvils).Register();
        }
    }
}
