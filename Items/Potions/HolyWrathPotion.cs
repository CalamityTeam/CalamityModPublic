using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
	public class HolyWrathPotion : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Holy Wrath Potion");
			Tooltip.SetDefault("Increases damage by 12% and increases movement and horizontal flight speed by 5%\n" +
				"Attacks inflict holy fire\n" +
				"While this potion's buff is active the Wrath Potion's buff is disabled");
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
			item.buffType = mod.BuffType("HolyWrathBuff");
			item.buffTime = 10800;
			item.value = Item.buyPrice(0, 2, 0, 0);
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.WrathPotion);
			recipe.AddIngredient(null, "UnholyEssence");
			recipe.AddIngredient(null, "GalacticaSingularity");
			recipe.AddTile(TileID.AlchemyTable);
			recipe.SetResult(this);
			recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.BottledWater);
			recipe.AddIngredient(null, "BloodOrb", 40);
			recipe.AddIngredient(null, "UnholyEssence");
			recipe.AddTile(TileID.AlchemyTable);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
