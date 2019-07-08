using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
	public class YharonFireball2 : ModProjectile
	{
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
			projectile.timeLeft = 3600;
			cooldownSlot = 1;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(projectile.localAI[0]);
			writer.Write(projectile.localAI[1]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			projectile.localAI[0] = reader.ReadSingle();
			projectile.localAI[1] = reader.ReadSingle();
		}

		public override void AI()
		{
			if (projectile.velocity.Y < 0f)
			{
				projectile.velocity.Y *= 0.97f;
			}
			else
			{
				projectile.velocity.Y *= 1.03f;
				if (projectile.velocity.Y > 16f)
				{
					projectile.velocity.Y = 16f;
				}
			}
			if (projectile.velocity.Y > -1f && projectile.localAI[1] == 0f)
			{
				projectile.localAI[1] = 1f;
				projectile.velocity.Y = 1f;
			}
			projectile.velocity.X *= 0.995f;
			projectile.rotation = projectile.velocity.ToRotation() + 1.57079637f;
			if (projectile.localAI[0] == 0f)
			{
				projectile.localAI[0] = 1f;
				Main.PlayTrackedSound(SoundID.DD2_BetsyFireballShot, projectile.Center);
			}
			if (projectile.ai[0] >= 2f)
			{
				projectile.alpha -= 25;
				if (projectile.alpha < 0)
				{
					projectile.alpha = 0;
				}
			}
			if (Main.rand.Next(16) == 0)
			{
				Dust expr_733B = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 55, 0f, 0f, 200, default(Color), 1f);
				expr_733B.scale *= 0.7f;
				expr_733B.velocity += projectile.velocity * 0.25f;
			}
			if (Main.rand.Next(12) == 0 && projectile.oldPos[9] != Vector2.Zero)
			{
				Dust expr_73D4 = Dust.NewDustDirect(projectile.oldPos[9], projectile.width, projectile.height, 55, 0f, 0f, 50, default(Color), 1f);
				expr_73D4.scale *= 0.85f;
				expr_73D4.velocity += projectile.velocity * 0.15f;
				expr_73D4.color = Color.Purple;
			}
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
		}

		public override bool CanDamage()
		{
			if (projectile.velocity.Y < -16f)
			{
				return false;
			}
			return true;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, Main.DiscoG, 53, projectile.alpha);
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14, 0.5f, 0f);
			projectile.position = projectile.Center;
			projectile.width = (projectile.height = 144);
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
			for (int num193 = 0; num193 < 2; num193++)
			{
				Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 55, 0f, 0f, 100, default(Color), 1.5f);
			}
			for (int num194 = 0; num194 < 20; num194++)
			{
				int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 55, 0f, 0f, 0, default(Color), 2.5f);
				Main.dust[num195].noGravity = true;
				Main.dust[num195].velocity *= 3f;
				num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 55, 0f, 0f, 100, default(Color), 1.5f);
				Main.dust[num195].velocity *= 2f;
				Main.dust[num195].noGravity = true;
			}
			projectile.Damage();
		}
	}
}