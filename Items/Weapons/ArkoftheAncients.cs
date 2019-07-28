using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.Items.Weapons
{
    public class ArkoftheAncients : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ark of the Ancients");
			Tooltip.SetDefault("A heavenly blade forged to vanquish all evil");
		}

		public override void SetDefaults()
		{
			item.width = 50;
			item.damage = 58;
			item.melee = true;
			item.useAnimation = 22;
			item.useTime = 22;
			item.useTurn = true;
			item.useStyle = 1;
			item.knockBack = 6.25f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 50;
			item.value = Item.buyPrice(0, 48, 0, 0);
			item.rare = 6;
			item.shoot = mod.ProjectileType("EonBeam");
			item.shootSpeed = 12f;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			switch (Main.rand.Next(2))
			{
				case 0: type = mod.ProjectileType("EonBeam"); break;
				case 1: type = 173; break;
			}
			Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, Main.myPlayer);
			float num72 = Main.rand.Next(18, 25);
			damage = Main.rand.Next(40, 60);
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
			int num107 = 2;
			for (int num108 = 0; num108 < num107; num108++)
			{
				vector2 = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
				vector2.X = (vector2.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
				vector2.Y -= (float)(100 * num108);
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
				float speedX4 = num78 + (float)Main.rand.Next(-120, 121) * 0.02f;
				float speedY5 = num79 + (float)Main.rand.Next(-120, 121) * 0.02f;
				int proj = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, 92, damage, knockBack, player.whoAmI, 0f, (float)Main.rand.Next(5));
				Main.projectile[proj].GetGlobalProjectile<CalamityGlobalProjectile>(mod).forceMelee = true;
			}
			return false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "EssenceofCinder", 3);
			recipe.AddIngredient(null, "EssenceofEleum", 3);
			recipe.AddIngredient(ItemID.Starfury);
			recipe.AddIngredient(ItemID.EnchantedSword);
			recipe.AddIngredient(ItemID.Excalibur);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "EssenceofCinder", 3);
			recipe.AddIngredient(null, "EssenceofEleum", 3);
			recipe.AddIngredient(ItemID.Starfury);
			recipe.AddIngredient(ItemID.Arkhalis);
			recipe.AddIngredient(ItemID.Excalibur);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.Next(5) == 0)
			{
				int num249 = Main.rand.Next(3);
				if (num249 == 0)
				{
					num249 = 15;
				}
				else if (num249 == 1)
				{
					num249 = 57;
				}
				else
				{
					num249 = 58;
				}
				int num250 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, num249, (float)(player.direction * 2), 0f, 150, default(Color), 1.3f);
				Main.dust[num250].velocity *= 0.2f;
			}
		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
		{
			if (Main.rand.Next(2) == 0)
			{
				target.AddBuff(mod.BuffType("HolyLight"), 300);
			}
		}
	}
}
