using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class CalamityMusicbox : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Music Box (Calamity)");
        }

        public override void SetDefaults()
        {
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.createTile = mod.TileType("CalamityMusicbox");
            item.width = 24;
            item.height = 24;
            item.rare = 4;
            item.value = 100000;
            item.accessory = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BrimstoneSlag", 12);
            recipe.AddIngredient(null, "EssenceofChaos", 3);
            recipe.SetResult(this);
            recipe.AddTile(null, "AshenAltar");
            recipe.AddRecipe();
        }
    }
}