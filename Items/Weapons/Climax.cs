using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
	public class Climax : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Voltaic Climax");
			Tooltip.SetDefault("Fires a circular spread of magnetic orbs around the mouse cursor");
			Item.staff[item.type] = true;
		}

		public override void SetDefaults()
		{
			item.damage = 90;
			item.magic = true;
			item.mana = 30;
			item.width = 78;
			item.height = 78;
			item.useTime = 30;
			item.useAnimation = 30;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 5f;
			item.value = Item.buyPrice(1, 80, 0, 0);
			item.rare = 10;
			item.UseSound = SoundID.Item20;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("Climax");
			item.shootSpeed = 12f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 14;
		}

		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			float num72 = item.shootSpeed;
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
			vector2 += new Vector2(num78, num79);
			float spread = 45f * 0.0174f;
			double startAngle = Math.Atan2(speedX, speedY) - spread / 2;
			double deltaAngle = spread / 8f;
			double offsetAngle;
			int i;
			for (i = 0; i < 4; i++)
			{
				offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
				Projectile.NewProjectile(vector2.X, vector2.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), type, damage, knockBack, player.whoAmI, 0f, 0f);
				Projectile.NewProjectile(vector2.X, vector2.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), type, damage, knockBack, player.whoAmI, 0f, 0f);
			}
			return false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "MagneticMeltdown");
			recipe.AddIngredient(null, "DarksunFragment", 15);
			recipe.AddTile(null, "DraedonsForge");
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}