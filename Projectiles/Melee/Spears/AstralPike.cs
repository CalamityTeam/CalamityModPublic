using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Spears
{
	public class AstralPike : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pike");
		}

		public override void SetDefaults()
		{
			projectile.width = 40;  //The width of the .png file in pixels divided by 2.
			projectile.aiStyle = 19;
			projectile.melee = true;  //Dictates whether projectile is a melee-class weapon.
			projectile.timeLeft = 90;
			projectile.height = 40;  //The height of the .png file in pixels divided by 2.
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.penetrate = -1;
			projectile.ownerHitCheck = true;
			projectile.hide = true;
			projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).trueMelee = true;
		}

		public override void AI()
		{
			Main.player[projectile.owner].direction = projectile.direction;
			Main.player[projectile.owner].heldProj = projectile.whoAmI;
			Main.player[projectile.owner].itemTime = Main.player[projectile.owner].itemAnimation;
			projectile.position.X = Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) - (float)(projectile.width / 2);
			projectile.position.Y = Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2) - (float)(projectile.height / 2);
			projectile.position += projectile.velocity * projectile.ai[0];
			if (Main.rand.Next(5) == 0)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, mod.DustType("AstralOrange"), projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
			}
			if (projectile.ai[0] == 0f)
			{
				projectile.ai[0] = 3f;
				projectile.netUpdate = true;
			}
			if (Main.player[projectile.owner].itemAnimation < Main.player[projectile.owner].itemAnimationMax / 3)
			{
				projectile.ai[0] -= 2.4f;
				if (projectile.localAI[0] == 0f && Main.myPlayer == projectile.owner)
				{
					projectile.localAI[0] = 1f;
				}
			}
			else
			{
				projectile.ai[0] += 0.95f;
			}

			if (Main.player[projectile.owner].itemAnimation == 0)
			{
				projectile.Kill();
			}

			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 2.355f;
			if (projectile.spriteDirection == -1)
			{
				projectile.rotation -= 1.57f;
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.immune[projectile.owner] = 6;
			if (crit)
			{
				for (int i = 0; i < 3; i++)
				{
					float xPos = (Main.rand.Next(2) == 0 ? projectile.position.X + 800 : projectile.position.X - 800);
					Vector2 vector2 = new Vector2(xPos, projectile.position.Y - Main.rand.Next(-800, 801));
					float num80 = xPos;
					float speedX = (float)target.position.X - vector2.X;
					float speedY = (float)target.position.Y - vector2.Y;
					float dir = (float)Math.Sqrt((double)(speedX * speedX + speedY * speedY));
					dir = 10 / num80;
					speedX *= dir * 150;
					speedY *= dir * 150;
					if (speedX > 15f)
					{
						speedX = 15f;
					}
					if (speedX < -15f)
					{
						speedX = -15f;
					}
					if (speedY > 15f)
					{
						speedY = 15f;
					}
					if (speedY < -15f)
					{
						speedY = -15f;
					}
					if (projectile.owner == Main.myPlayer)
					{
						int proj = Projectile.NewProjectile(vector2.X, vector2.Y, speedX, speedY, mod.ProjectileType("AstralStar"), (int)((double)projectile.damage * 0.2), 1f, projectile.owner);
						Main.projectile[proj].GetGlobalProjectile<CalamityGlobalProjectile>(mod).forceMelee = true;
					}
				}
			}
		}
	}
}