using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Patreon
{
    public class DevilsSunrise : ModItem
	{
		public static int BaseDamage = 500;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Devil's Sunrise");
			Tooltip.SetDefault("Balls? Smalls");
		}

		public override void SetDefaults()
		{
			item.width = 66;
			item.height = 66;
			item.melee = true;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.channel = true;
			item.damage = BaseDamage;
			item.crit += 10;
			item.knockBack = 4f;
			item.useAnimation = 25;
			item.useTime = 5;
			item.autoReuse = false;
			item.useStyle = 5;

			item.rare = 10;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 21;
			item.value = Item.buyPrice(1, 40, 0, 0);

			item.shoot = mod.ProjectileType("DevilsSunrise");
			item.shootSpeed = 15f;
		}

		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("DevilsSunrise"), damage, knockBack, player.whoAmI, 0f, 0f);
			Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("DevilsSunriseCyclone"), damage, knockBack, player.whoAmI, 0f, 0f);
			return false;
		}

		public override void AddRecipes()
		{
			ModRecipe r = new ModRecipe(mod);
			r.AddIngredient(ItemID.Arkhalis);
			r.AddIngredient(null, "DemonicBoneAsh", 10);
			r.AddIngredient(null, "BloodstoneCore", 25);
			r.AddTile(TileID.LunarCraftingStation);
			r.SetResult(this);
			r.AddRecipe();
		}
	}
}
