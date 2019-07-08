using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
	public class RedLightningFeather : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lightning Feather");
			Main.projFrames[projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			projectile.width = 20;
			projectile.height = 20;
			projectile.hostile = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.penetrate = 1;
			projectile.timeLeft = 1200;
			cooldownSlot = 1;
		}

		public override void AI()
		{
			projectile.frameCounter++;
			if (projectile.frameCounter > 4)
			{
				projectile.frame++;
				projectile.frameCounter = 0;
			}
			if (projectile.frame > 3)
			{
				projectile.frame = 0;
			}
			if ((double)Math.Abs(projectile.velocity.X) > 0.2)
			{
				projectile.spriteDirection = -projectile.direction;
			}
			if (projectile.velocity.X < 0f)
			{
				projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
			}
			else
			{
				projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
			}
			Lighting.AddLight(projectile.Center, 0.7f, 0f, 0f);
			projectile.ai[0] += 1f;
			if (projectile.ai[0] >= 150f)
			{
				projectile.extraUpdates = 1;
				int num103 = (int)Player.FindClosest(projectile.Center, 1, 1);
				projectile.ai[1] += 1f;
				if (projectile.ai[1] < 180f)
				{
					float scaleFactor2 = projectile.velocity.Length();
					Vector2 vector11 = Main.player[num103].Center - projectile.Center;
					vector11.Normalize();
					vector11 *= scaleFactor2;
					projectile.velocity = (projectile.velocity * 15f + vector11) / 16f;
					projectile.velocity.Normalize();
					projectile.velocity *= scaleFactor2;
				}
				else if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < 18f)
				{
					projectile.velocity.X *= 1.01f;
					projectile.velocity.Y *= 1.01f;
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture2D13 = Main.projectileTexture[projectile.type];
			int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
			int y6 = num214 * projectile.frame;
			Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
			return false;
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 21, 1f, 0f);
			projectile.position = projectile.Center;
			projectile.width = (projectile.height = 64);
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
			for (int num193 = 0; num193 < 6; num193++)
			{
				Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 100, default(Color), 1.5f);
			}
			for (int num194 = 0; num194 < 10; num194++)
			{
				int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 0, default(Color), 2.5f);
				Main.dust[num195].noGravity = true;
				Main.dust[num195].velocity *= 3f;
				num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 100, default(Color), 1.5f);
				Main.dust[num195].velocity *= 2f;
				Main.dust[num195].noGravity = true;
			}
			projectile.Damage();
		}
	}
}