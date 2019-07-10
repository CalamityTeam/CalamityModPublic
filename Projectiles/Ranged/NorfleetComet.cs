using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
	public class NorfleetComet : ModProjectile
	{
		public int noTileHitCounter = 120;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Comet");
		}

		public override void SetDefaults()
		{
			projectile.width = 34;
			projectile.height = 34;
			projectile.alpha = 255;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.tileCollide = false;
			projectile.penetrate = 1;
			projectile.timeLeft = 600;
			projectile.ignoreWater = true;
		}

		public override void AI()
		{
			int randomToSubtract = Main.rand.Next(1, 4);
			noTileHitCounter -= randomToSubtract;
			if (noTileHitCounter == 0)
			{
				projectile.tileCollide = true;
			}
			if (projectile.soundDelay == 0)
			{
				projectile.soundDelay = 20 + Main.rand.Next(40);
				if (Main.rand.Next(5) == 0)
				{
					Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 9);
				}
			}
			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] == 18f)
			{
				projectile.localAI[0] = 0f;
				for (int l = 0; l < 12; l++)
				{
					Vector2 vector3 = Vector2.UnitX * (float)(-(float)projectile.width) / 2f;
					vector3 += -Vector2.UnitY.RotatedBy((double)((float)l * 3.14159274f / 6f), default(Vector2)) * new Vector2(8f, 16f);
					vector3 = vector3.RotatedBy((double)(projectile.rotation - 1.57079637f), default(Vector2));
					int num9 = Dust.NewDust(projectile.Center, 0, 0, (Main.rand.Next(2) == 0 ? 221 : 244), 0f, 0f, 160, default(Color), 1f);
					Main.dust[num9].scale = 1.1f;
					Main.dust[num9].noGravity = true;
					Main.dust[num9].position = projectile.Center + vector3;
					Main.dust[num9].velocity = projectile.velocity * 0.1f;
					Main.dust[num9].velocity = Vector2.Normalize(projectile.Center - projectile.velocity * 3f - Main.dust[num9].position) * 1.25f;
				}
			}
			projectile.alpha -= 15;
			int num58 = 150;
			if (projectile.Center.Y >= projectile.ai[1])
			{
				num58 = 0;
			}
			if (projectile.alpha < num58)
			{
				projectile.alpha = num58;
			}
			projectile.rotation = projectile.velocity.ToRotation() - 1.57079637f;
			if (Main.rand.Next(16) == 0)
			{
				Vector2 value3 = Vector2.UnitX.RotatedByRandom(1.5707963705062866).RotatedBy((double)projectile.velocity.ToRotation(), default(Vector2));
				int num59 = Dust.NewDust(projectile.position, projectile.width, projectile.height, (Main.rand.Next(2) == 0 ? 221 : 244), projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, default(Color), 1.2f);
				Main.dust[num59].velocity = value3 * 0.66f;
				Main.dust[num59].position = projectile.Center + value3 * 12f;
			}
			if (projectile.ai[1] == 1f)
			{
				projectile.light = 0.5f;
				if (Main.rand.Next(10) == 0)
				{
					Dust.NewDust(projectile.position, projectile.width, projectile.height, (Main.rand.Next(2) == 0 ? 221 : 244), projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, default(Color), 1.2f);
				}
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(Main.DiscoR, 100, 255, projectile.alpha);
		}

		public override void Kill(int timeLeft)
		{
			if (projectile.owner == Main.myPlayer)
			{
				Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("NorfleetExplosion"), (int)((double)projectile.damage * 0.3), projectile.knockBack * 0.1f, projectile.owner, 0f, 0f);
			}
			Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 10);
			projectile.position = projectile.Center;
			projectile.width = (projectile.height = 144);
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
			for (int num193 = 0; num193 < 4; num193++)
			{
				Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (Main.rand.Next(2) == 0 ? 221 : 244), 0f, 0f, 50, default(Color), 1.5f);
			}
			for (int num194 = 0; num194 < 20; num194++)
			{
				int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (Main.rand.Next(2) == 0 ? 221 : 244), 0f, 0f, 0, default(Color), 2.5f);
				Main.dust[num195].noGravity = true;
				Main.dust[num195].velocity *= 3f;
				num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (Main.rand.Next(2) == 0 ? 221 : 244), 0f, 0f, 50, default(Color), 1.5f);
				Main.dust[num195].velocity *= 2f;
				Main.dust[num195].noGravity = true;
			}
		}
	}
}
