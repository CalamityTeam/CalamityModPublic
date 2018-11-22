using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
	public class ZenPotion : ModItem
	{
		public override void SetStaticDefaults()
	 	{
	 		DisplayName.SetDefault("Zen Potion");
	 		Tooltip.SetDefault("Reduces spawn rates...a lot...");
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
			item.buffType = mod.BuffType("Zen");
			item.buffTime = 36000;
			item.value = 10000;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.CalmingPotion);
			recipe.AddIngredient(ItemID.PinkGel, 3);
			recipe.AddIngredient(null, "EbonianGel", 3);
			recipe.AddTile(TileID.AlchemyTable);
			recipe.SetResult(this);
			recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BloodOrb", 20);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
	}
}