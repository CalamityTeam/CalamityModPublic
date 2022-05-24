
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureAstral
{
    public class MonolithPlatform : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 200;
        }

        public override void SetDefaults()
        {
            Item.SetNameOverride("Monolith Platform");
            Item.width = 8;
            Item.height = 10;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurnitureAstral.MonolithPlatform>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).AddIngredient(ModContent.ItemType<AstralMonolith>()).Register();
        }
    }
}
