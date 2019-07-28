using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class PlanetaryAnnihilation : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Planetary Annihilation");
			Tooltip.SetDefault("Fires a storm of arrows from the sky\n" +
				"Wooden arrows are converted to homing energy bolts");
		}

		public override void SetDefaults()
		{
			item.damage = 75;
			item.ranged = true;
			item.width = 58;
			item.height = 102;
			item.useTime = 22;
			item.useAnimation = 22;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 5.5f;
			item.value = Item.buyPrice(1, 20, 0, 0);
			item.rare = 10;
			item.UseSound = SoundID.Item75;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("TerraBall");
			item.shootSpeed = 16f;
			item.useAmmo = 40;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			float num72 = Main.rand.Next(19, 35);
			Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
			float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
			float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
			if (player.gravDir == -1f)
			{
				num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
			}
			float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
			float num81 = num80;
			if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
			{
				num78 = (float)player.direction;
				num79 = 0f;
				num80 = num72;
			}
			else
			{
				num80 = num72 / num80;
			}
			num78 *= num80;
			num79 *= num80;
			vector2 = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
			vector2.X = (vector2.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
			vector2.Y -= 100f;
			num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
			num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
			if (num79 < 0f)
			{
				num79 *= -1f;
			}
			if (num79 < 20f)
			{
				num79 = 20f;
			}
			num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
			num80 = num72 / num80;
			num78 *= num80;
			num79 *= num80;
			float speedX4 = num78 + (float)Main.rand.Next(-240, 241) * 0.02f;
			float speedY5 = num79 + (float)Main.rand.Next(-240, 241) * 0.02f;
			if (type == ProjectileID.WoodenArrowFriendly)
			{
				Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, (speedY5 * 1.3f), mod.ProjectileType("TerraBallR"), damage, knockBack, player.whoAmI, 0f, (float)Main.rand.Next(2));
				Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, (speedY5 * 1.2f), mod.ProjectileType("TerraBallO"), damage, knockBack, player.whoAmI, 0f, (float)Main.rand.Next(3));
				Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, (speedY5 * 1.1f), mod.ProjectileType("TerraBallY"), damage, knockBack, player.whoAmI, 0f, (float)Main.rand.Next(4));
				Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, (speedY5 * 1f), mod.ProjectileType("TerraBallG"), damage, knockBack, player.whoAmI, 0f, (float)Main.rand.Next(5));
				Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, (speedY5 * 0.9f), mod.ProjectileType("TerraBallB"), damage, knockBack, player.whoAmI, 0f, (float)Main.rand.Next(6));
				Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, (speedY5 * 0.8f), mod.ProjectileType("TerraBallI"), damage, knockBack, player.whoAmI, 0f, (float)Main.rand.Next(7));
				Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, (speedY5 * 0.7f), mod.ProjectileType("TerraBallV"), damage, knockBack, player.whoAmI, 0f, (float)Main.rand.Next(8));
			}
			else
			{
				int num121 = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, (speedY5 * 1.3f), type, damage, knockBack, player.whoAmI, 0f, 0f);
				Main.projectile[num121].noDropItem = true;
				int num122 = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, (speedY5 * 1.2f), type, damage, knockBack, player.whoAmI, 0f, 0f);
				Main.projectile[num122].noDropItem = true;
				int num123 = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, (speedY5 * 1.1f), type, damage, knockBack, player.whoAmI, 0f, 0f);
				Main.projectile[num123].noDropItem = true;
				int num124 = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, (speedY5 * 1f), type, damage, knockBack, player.whoAmI, 0f, 0f);
				Main.projectile[num124].noDropItem = true;
				int num125 = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, (speedY5 * 0.9f), type, damage, knockBack, player.whoAmI, 0f, 0f);
				Main.projectile[num125].noDropItem = true;
				int num126 = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, (speedY5 * 0.8f), type, damage, knockBack, player.whoAmI, 0f, 0f);
				Main.projectile[num126].noDropItem = true;
				int num127 = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, (speedY5 * 0.7f), type, damage, knockBack, player.whoAmI, 0f, 0f);
				Main.projectile[num127].noDropItem = true;
			}
			return false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "GalacticaSingularity", 5);
			recipe.AddIngredient(ItemID.LunarBar, 5);
			recipe.AddIngredient(null, "CosmicBolter");
			recipe.AddIngredient(ItemID.DaedalusStormbow);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
