using CalamityMod.Items.Materials;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.Furniture.CraftingStations
{
    public class StaticRefiner : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            Tooltip.SetDefault("Used for special crafting");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Furniture.CraftingStations.StaticRefiner>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<PurifiedGel>(), 5).AddIngredient(ItemID.Solidifier).AddTile(TileID.Anvils).Register();
        }
    }
}
