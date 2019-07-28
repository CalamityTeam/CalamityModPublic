using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public class ToothBall : CalamityDamageItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Tooth Ball");
		}

		public override void SafeSetDefaults()
		{
			item.width = 30;
			item.damage = 20;
			item.noMelee = true;
			item.consumable = true;
			item.noUseGraphic = true;
			item.useAnimation = 13;
			item.crit = 8;
			item.useStyle = 1;
			item.useTime = 13;
			item.knockBack = 2.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 30;
			item.maxStack = 999;
			item.value = 1000;
			item.rare = 3;
			item.shoot = mod.ProjectileType("ToothBallProjectile");
			item.shootSpeed = 16f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "BloodSample");
	        recipe.AddIngredient(ItemID.Vertebrae);
	        recipe.AddIngredient(ItemID.CrimtaneBar);
	        recipe.AddTile(TileID.DemonAltar);
	        recipe.SetResult(this, 100);
	        recipe.AddRecipe();
		}
	}
}
