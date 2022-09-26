using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureStatigel
{
    public class StatigelBlock : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
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
            Item.createTile = ModContent.TileType<Tiles.FurnitureStatigel.StatigelBlock>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).AddIngredient(ModContent.ItemType<PurifiedGel>()).AddTile(ModContent.TileType<StaticRefiner>()).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<StatigelPlatform>(), 2).AddTile(ModContent.TileType<StaticRefiner>()).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<StatigelWall>(), 4).AddTile(ModContent.TileType<StaticRefiner>()).Register();
        }
    }
}
