using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.FurnitureAncient
{
    public class AncientTable : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;

        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Orange;
            Item.consumable = true;
            Item.value = 0;
            Item.createTile = ModContent.TileType<Tiles.FurnitureAncient.AncientTable>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<BrimstoneSlag>(), 8).AddTile(ModContent.TileType<AncientAltar>()).Register();
        }
    }
}
