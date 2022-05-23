using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureOccult
{
    public class OccultSofa : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.SetNameOverride("Otherworldly Sofa");
            Item.width = 28;
            Item.height = 20;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurnitureOccult.OccultSofa>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<OccultStone>(), 5).AddIngredient(ItemID.Silk, 2).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
