using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.SunkenSea
{
	public class Prismalline : ModProjectile
	{
		public bool hitEnemy = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Prismalline");
		}

		public override void SetDefaults()
		{
			projectile.width = 20;
			projectile.height = 20;
			projectile.friendly = true;
			projectile.penetrate = 2;
			projectile.aiStyle = 113;
			projectile.timeLeft = 180;
			aiType = 598;
			projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).rogue = true;
		}

		public override void AI()
		{
			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 2.355f;
			if (projectile.spriteDirection == -1)
			{
				projectile.rotation -= 1.57f;
			}
			projectile.ai[1] += 1f;
			if (projectile.ai[1] == 40f)
			{
				int numProj = 4;
				int numSpecProj = 0;
				float rotation = MathHelper.ToRadians(50);
				if (projectile.owner == Main.myPlayer)
				{
					for (int i = 0; i < numProj + 1; i++)
					{
						Vector2 speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
						while (speed.X == 0f && speed.Y == 0f)
						{
							speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
						}
						speed.Normalize();
						speed *= ((float)Main.rand.Next(30, 61) * 0.1f) * 2f;
						if (numSpecProj < 2 && !hitEnemy)
						{
							Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, speed.X, speed.Y, mod.ProjectileType("Prismalline3"), (int)((double)projectile.damage * 1.25), projectile.knockBack, projectile.owner, 0f, 0f);
							++numSpecProj;
						}
						else
						{
							Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, speed.X, speed.Y, mod.ProjectileType("Prismalline2"), (int)((double)projectile.damage * 0.75), projectile.knockBack, projectile.owner, 0f, 0f);
						}
					}
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D tex = Main.projectileTexture[projectile.type];
			spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
			return false;
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 27);
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 154, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			hitEnemy = true;
		}
	}
}