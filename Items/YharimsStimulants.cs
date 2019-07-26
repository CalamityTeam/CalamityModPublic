using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
	public class YharimsStimulants : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Yharim's Stimulants");
			Tooltip.SetDefault("Gives decent buffs to ALL offensive and defensive stats");
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
			item.buffType = mod.BuffType("YharimPower");
			item.buffTime = 108000;
			item.value = Item.buyPrice(0, 2, 0, 0);
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.EndurancePotion);
			recipe.AddIngredient(ItemID.IronskinPotion);
			recipe.AddIngredient(ItemID.SwiftnessPotion);
			recipe.AddIngredient(ItemID.ArcheryPotion);
			recipe.AddIngredient(ItemID.MagicPowerPotion);
			recipe.AddIngredient(ItemID.TitanPotion);
			recipe.AddTile(TileID.AlchemyTable);
			recipe.SetResult(this);
			recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BloodOrb", 50);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
	}
}
