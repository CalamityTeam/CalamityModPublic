using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
	public class ScarletDevil : CalamityDamageItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scarlet Devil");
			Tooltip.SetDefault("Throws an ultra high velocity spear, which creates more projectiles that home in\n"+
			"The spear creates a Scarlet Blast upon hitting an enemy\n"+
			"Stealth strikes grant you lifesteal\n"+
			"'Divine Spear \"Spear the Gungnir\"'");
		}

		public override void SafeSetDefaults()
		{
			item.width = 94;
			item.height = 94;
			item.damage = 40000;
			item.crit += 20;
			item.noMelee = true;
            item.noUseGraphic = true;
			item.useAnimation = 60;
			item.useStyle = 1;
			item.useTime = 60;
			item.knockBack = 8f;
			item.UseSound = SoundID.Item60;
			item.autoReuse = true;
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.rare = 8;
			item.shoot = mod.ProjectileType("ScarletDevilProjectile");
			item.shootSpeed = 30f;
			item.Calamity().rogue = true;
			item.Calamity().postMoonLordRarity = 16;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "ProfanedTrident");
            recipe.AddIngredient(null, "BloodstoneCore", 15);
            recipe.AddIngredient(ItemID.SoulofNight, 15);
            recipe.AddIngredient(null, "ShadowspecBar", 5);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
		}
	}
}
