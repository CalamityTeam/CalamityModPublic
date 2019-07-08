using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Spears
{
	public class Brimlance : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimlance");
		}

		public override void SetDefaults()
		{
			projectile.width = 40;  //The width of the .png file in pixels divided by 2.
			projectile.aiStyle = 19;
			projectile.melee = true;  //Dictates whether this is a melee-class weapon.
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
			if (Main.rand.Next(4) == 0)
			{
				int num = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 235, (float)(projectile.direction * 2), 0f, 150, default(Color), 1f);
				Main.dust[num].noGravity = true;
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
			target.immune[projectile.owner] = 7;
			if (target.life <= 0)
			{
				int num251 = Main.rand.Next(2, 5);
				if (projectile.owner == Main.myPlayer)
				{
					Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, mod.ProjectileType("BrimlanceHellfireExplosion"), (int)((double)projectile.damage * 0.75), knockback, Main.myPlayer);
					for (int num252 = 0; num252 < num251; num252++)
					{
						Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
						while (value15.X == 0f && value15.Y == 0f)
						{
							value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
						}
						value15.Normalize();
						value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
						Projectile.NewProjectile(projectile.oldPosition.X + (float)(projectile.width / 2), projectile.oldPosition.Y + (float)(projectile.height / 2), value15.X, value15.Y, mod.ProjectileType("BrimlanceStandingFire"), (int)((double)projectile.damage * 0.5), 0f, projectile.owner, 0f, 0f);
					}
				}
			}
		}
	}
}