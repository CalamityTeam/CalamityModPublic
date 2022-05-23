using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureVoid
{
    public class VoidClock : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.SetNameOverride("Void Obelisk");
            Item.width = 26;
            Item.height = 22;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurnitureVoid.VoidClock>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.IronBar, 3).AddIngredient(ItemID.Glass, 6).AddIngredient(ModContent.ItemType<SmoothVoidstone>(), 10).AddTile(ModContent.TileType<VoidCondenser>()).Register();
        }
    }
}
