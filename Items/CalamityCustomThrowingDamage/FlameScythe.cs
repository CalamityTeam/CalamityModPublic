using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public class FlameScythe : CalamityDamageItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flame Scythe");
			Tooltip.SetDefault("Throws a scythe that explodes on enemy hits");
		}

		public override void SafeSetDefaults()
		{
			item.width = 50;
			item.damage = 145;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.autoReuse = true;
			item.useAnimation = 19;
			item.useStyle = 1;
			item.useTime = 19;
			item.knockBack = 8.5f;
			item.UseSound = SoundID.Item1;
			item.height = 48;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
			item.shoot = mod.ProjectileType("FlameScytheProjectile");
			item.shootSpeed = 16f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "CruptixBar", 9);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
