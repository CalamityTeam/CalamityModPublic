using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public class CobaltKunai : CalamityDamageItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cobalt Kunai");
		}

		public override void SafeSetDefaults()
		{
			item.width = 18;
			item.damage = 28;
			item.noMelee = true;
			item.consumable = true;
			item.noUseGraphic = true;
			item.useAnimation = 12;
			item.scale = 0.75f;
			item.useStyle = 1;
			item.useTime = 12;
			item.knockBack = 2.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 40;
			item.maxStack = 999;
			item.value = 900;
			item.rare = 4;
			item.shoot = mod.ProjectileType("CobaltKunaiProjectile");
			item.shootSpeed = 12f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.CobaltBar);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this, 30);
	        recipe.AddRecipe();
		}
	}
}
