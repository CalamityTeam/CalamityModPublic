using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureStatigel
{
    public class StatigelClock : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 46;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurnitureStatigel.StatigelClock>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.IronBar, 3).AddIngredient(ItemID.Glass, 6).AddIngredient(ModContent.ItemType<StatigelBlock>(), 10).AddTile(ModContent.TileType<StaticRefiner>()).Register();
        }
    }
}
