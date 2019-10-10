using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public class Prismalline : CalamityDamageItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Prismalline");
			Tooltip.SetDefault("Throws daggers that split after a while");
		}

		public override void SafeSetDefaults()
		{
			item.width = 46;
			item.damage = 22;
			item.crit += 4;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 14;
			item.useStyle = 1;
			item.useTime = 14;
			item.knockBack = 5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 46;
			item.value = Item.buyPrice(0, 36, 0, 0);
			item.rare = 5;
			item.shoot = mod.ProjectileType("Prismalline");
			item.shootSpeed = 16f;
			item.Calamity().rogue = true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "Crystalline");
	        recipe.AddIngredient(null, "EssenceofEleum", 5);
	        recipe.AddIngredient(null, "SeaPrism", 5);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
