using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureProfaned
{
    public class ProfanedBasin : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Used for special crafting");
        }

        public override void SetDefaults()
        {
            item.SetNameOverride("Profaned Crucible");
            item.width = 8;
            item.height = 10;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.ProfanedBasin>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ProfanedRock>(), 10);
            recipe.AddIngredient(ModContent.ItemType<UnholyEssence>(), 5);
            recipe.SetResult(this, 1);
            recipe.AddTile(412);
            recipe.AddRecipe();
        }
    }
}
