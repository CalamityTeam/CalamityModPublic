using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
	public class YharonFireball : ModProjectile
	{
		private float speedX = -3f;
		private float speedX2 = -5f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dragon Fireball");
		}

		public override void SetDefaults()
		{
			projectile.width = 30;
			projectile.height = 30;
			projectile.hostile = true;
			projectile.alpha = 255;
			projectile.penetrate = -1;
			projectile.timeLeft = 120;
			projectile.aiStyle = 1;
			aiType = 686;
			cooldownSlot = 1;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(speedX);
			writer.Write(projectile.localAI[0]);
			writer.Write(speedX2);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			speedX = reader.ReadSingle();
			projectile.localAI[0] = reader.ReadSingle();
			speedX2 = reader.ReadSingle();
		}

		public override bool PreAI()
		{
			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] == 36f)
			{
				projectile.localAI[0] = 0f;
				for (int l = 0; l < 12; l++)
				{
					Vector2 vector3 = Vector2.UnitX * (float)(-(float)projectile.width) / 2f;
					vector3 += -Vector2.UnitY.RotatedBy((double)((float)l * 3.14159274f / 6f), default(Vector2)) * new Vector2(8f, 16f);
					vector3 = vector3.RotatedBy((double)(projectile.rotation - 1.57079637f), default(Vector2));
					int num9 = Dust.NewDust(projectile.Center, 0, 0, 55, 0f, 0f, 160, default(Color), 1f);
					Main.dust[num9].scale = 1.1f;
					Main.dust[num9].noGravity = true;
					Main.dust[num9].position = projectile.Center + vector3;
					Main.dust[num9].velocity = projectile.velocity * 0.1f;
					Main.dust[num9].velocity = Vector2.Normalize(projectile.Center - projectile.velocity * 3f - Main.dust[num9].position) * 1.25f;
				}
			}
			return true;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, Main.DiscoG, 53, projectile.alpha);
		}

		public override void Kill(int timeLeft)
		{
			if (projectile.owner == Main.myPlayer)
			{
				for (int x = 0; x < 3; x++)
				{
					Projectile.NewProjectile((int)projectile.Center.X, (int)projectile.Center.Y, speedX, -50f, mod.ProjectileType("YharonFireball2"), projectile.damage, 0f, Main.myPlayer, 0f, 0f);
					speedX += 3f;
				}
				for (int x = 0; x < 2; x++)
				{
					Projectile.NewProjectile((int)projectile.Center.X, (int)projectile.Center.Y, speedX2, -75f, mod.ProjectileType("YharonFireball2"), projectile.damage, 0f, Main.myPlayer, 0f, 0f);
					speedX2 += 10f;
				}
			}
			projectile.position = projectile.Center;
			projectile.width = (projectile.height = 144);
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
			for (int num193 = 0; num193 < 2; num193++)
			{
				Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 55, 0f, 0f, 50, default(Color), 1.5f);
			}
			for (int num194 = 0; num194 < 20; num194++)
			{
				int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 55, 0f, 0f, 0, default(Color), 2.5f);
				Main.dust[num195].noGravity = true;
				Main.dust[num195].velocity *= 3f;
				num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 55, 0f, 0f, 50, default(Color), 1.5f);
				Main.dust[num195].velocity *= 2f;
				Main.dust[num195].noGravity = true;
			}
		}
	}
}