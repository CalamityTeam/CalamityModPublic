using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureAncient
{
    public class AncientBed : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 20;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.rare = ItemRarityID.Orange;
            item.consumable = true;
            item.value = 0;
            item.createTile = ModContent.TileType<Tiles.FurnitureAncient.AncientBed>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BrimstoneSlag>(), 15);
            recipe.AddIngredient(ItemID.Silk, 5);
            recipe.SetResult(this, 1);
            recipe.AddTile(ModContent.TileType<AncientAltar>());
            recipe.AddRecipe();
        }
    }
}
