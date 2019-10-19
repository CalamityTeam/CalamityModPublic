using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureSilva
{
    public class SilvaCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.SilvaCrystal>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CrystalBlock, 200);
            recipe.AddIngredient(ItemID.GoldBar, 25);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>());
            recipe.AddIngredient(ModContent.ItemType<EffulgentFeather>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>());
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>());
            recipe.SetResult(this, 400);
            recipe.AddTile(null, "DraedonsForge");
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CrystalBlock, 200);
            recipe.AddIngredient(ItemID.PlatinumBar, 25);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>());
            recipe.AddIngredient(ModContent.ItemType<EffulgentFeather>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>());
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>());
            recipe.SetResult(this, 400);
            recipe.AddTile(null, "DraedonsForge");
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SilvaWall>(), 4);
            recipe.SetResult(this);
            recipe.AddTile(18);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SilvaPlatform>(), 2);
            recipe.SetResult(this);
            recipe.AddTile(null, "SilvaBasin");
            recipe.AddRecipe();
        }
    }
}
