using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
	public class CadencePotion : ModItem
	{
		public override void SetStaticDefaults()
	 	{
	 		DisplayName.SetDefault("Cadance Potion");
	 		Tooltip.SetDefault("Gives the cadence buff which reduces shop prices and enemy aggro\n" +
	 		                   "Increases life regen and increases max life by 10%\n" +
	 		                   "Increases heart pickup range");
	 	}
	
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 18;
			item.useTurn = true;
			item.maxStack = 30;
			item.rare = 3;
			item.useAnimation = 17;
			item.useTime = 17;
			item.useStyle = 2;
			item.UseSound = SoundID.Item3;
			item.consumable = true;
			item.buffType = mod.BuffType("Cadence");
			item.buffTime = 18000;
			item.value = 10000;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LovePotion);
            recipe.AddIngredient(ItemID.HeartreachPotion);
            recipe.AddIngredient(ItemID.LifeforcePotion);
            recipe.AddIngredient(ItemID.RegenerationPotion);
            recipe.AddIngredient(ItemID.CalmingPotion);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BloodOrb", 40);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
	}
}