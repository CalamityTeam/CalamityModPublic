using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class ShadowspecBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowspec Bar");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(8, 10));
        }

        public override void SetDefaults()
        {
            item.width = 15;
            item.height = 12;
            item.maxStack = 999;
			item.rare = 10;
			item.value = Item.buyPrice(0, 10, 0, 0);
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 15;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BarofLife", 3);
            recipe.AddIngredient(null, "Phantoplasm", 3);
            recipe.AddIngredient(null, "NightmareFuel", 3);
            recipe.AddIngredient(null, "EndothermicEnergy", 3);
            recipe.AddIngredient(null, "CalamitousEssence");
            recipe.AddIngredient(null, "DarksunFragment");
            recipe.AddIngredient(null, "HellcasterFragment");
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this, 3);
            recipe.AddRecipe();
        }
    }
}
