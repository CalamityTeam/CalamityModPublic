using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables
{
    public class HardenedSulphurousSandstone : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hardened Sulphurous Sandstone");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<Tiles.Abyss.HardenedSulphurousSandstone>();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 16;
            item.height = 16;
            item.maxStack = 999;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddTile(18);
            recipe.AddIngredient(ModContent.ItemType<Walls.HardenedSulphurousSandstoneWall>(), 4);
            recipe.SetResult(this);
            recipe.AddRecipe();
            base.AddRecipes();
        }
    }
}
