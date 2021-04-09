using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureAncient
{
    public class AncientSink : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Counts as a lava source");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.rare = ItemRarityID.Orange;
            item.consumable = true;
            item.value = 0;
            item.createTile = ModContent.TileType<Tiles.FurnitureAncient.AncientSink>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BrimstoneSlag>(), 6);
            recipe.AddIngredient(ItemID.LavaBucket);
            recipe.SetResult(this, 1);
            recipe.AddTile(ModContent.TileType<AncientAltar>());
            recipe.AddRecipe();
        }
    }
}
