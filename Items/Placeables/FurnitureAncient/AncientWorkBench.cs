using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.FurnitureAncient
{
    [LegacyName("AncientWorkbench")]
    public class AncientWorkBench : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetDefaults()
        {
            Item.SetNameOverride("Ancient Work Bench");
            Item.width = 28;
            Item.height = 14;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = 0;
            Item.createTile = ModContent.TileType<Tiles.FurnitureAncient.AncientWorkBench>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<BrimstoneSlag>(), 10).AddTile(ModContent.TileType<AncientAltar>()).Register();
        }
    }
}
