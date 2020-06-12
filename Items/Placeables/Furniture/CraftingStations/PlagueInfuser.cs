using CalamityMod.Items.Materials;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.Furniture.CraftingStations
{
    public class PlagueInfuser : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Used for special crafting");
        }

        public override void SetDefaults()
        {
            item.width = 8;
            item.height = 10;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.Furniture.CraftingStations.PlagueInfuser>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.IronBar, 10);
            recipe.anyIronBar = true;
            recipe.AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 20);
            recipe.SetResult(this, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.AddRecipe();
        }
    }
}
