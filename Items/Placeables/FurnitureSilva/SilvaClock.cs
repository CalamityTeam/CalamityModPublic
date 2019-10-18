using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureSilva
{
    public class SilvaClock : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 20;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.SilvaClock>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SilvaCrystal", 10);
            recipe.AddIngredient(ItemID.GoldBar, 3);
            recipe.anyIronBar = true;
            recipe.AddIngredient(ItemID.Glass, 6);
            recipe.SetResult(this);
            recipe.AddTile(null, "SilvaBasin");
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SilvaCrystal", 10);
            recipe.AddIngredient(ItemID.PlatinumBar, 3);
            recipe.anyIronBar = true;
            recipe.AddIngredient(ItemID.Glass, 6);
            recipe.SetResult(this);
            recipe.AddTile(null, "SilvaBasin");
            recipe.AddRecipe();
        }
    }
}
