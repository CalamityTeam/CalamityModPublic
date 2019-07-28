using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
	public class SignusScythe : ModProjectile
	{
		private int counter = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scythe");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			projectile.width = 26;
			projectile.height = 26;
			projectile.hostile = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.timeLeft = 600;
			projectile.alpha = 255;
			projectile.penetrate = -1;
			cooldownSlot = 1;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(counter);
			writer.Write(projectile.localAI[0]);
			writer.Write(projectile.localAI[1]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			counter = reader.ReadInt32();
			projectile.localAI[0] = reader.ReadSingle();
			projectile.localAI[1] = reader.ReadSingle();
		}

		public override void AI()
		{
			projectile.rotation += 0.5f * (float)projectile.direction;
			if (projectile.localAI[0] == 0f)
			{
				projectile.localAI[0] = 1f;
				Main.PlaySound(SoundID.Item73, projectile.position);
			}
			int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default(Color), 1f);
			Main.dust[num469].noGravity = true;
			Main.dust[num469].velocity *= 0f;
			projectile.ai[0] += 1f;
			if (projectile.ai[0] > 180f)
			{
				if (projectile.ai[1] > 0f)
				{
					int num625 = (int)projectile.ai[1] - 1;
					if (num625 < 255)
					{
						Vector2 value16 = Main.player[num625].Center - projectile.Center;
						if (value16.Length() < 200f || counter > 0)
						{
							counter--;
							if (counter <= 0 && projectile.localAI[0] == 2f)
							{
								projectile.Kill();
								return;
							}
							if (projectile.localAI[0] < 2f)
							{
								projectile.localAI[0] = 2f;
								float speed = 30f;
								Vector2 vector167 = new Vector2(projectile.Center.X + (float)(projectile.direction * 20), projectile.Center.Y + 6f);
								float num1373 = Main.player[num625].position.X + (float)Main.player[num625].width * 0.5f - vector167.X;
								float num1374 = Main.player[num625].Center.Y - vector167.Y;
								float num1375 = (float)Math.Sqrt((double)(num1373 * num1373 + num1374 * num1374));
								float num1376 = speed / num1375;
								num1373 *= num1376;
								num1374 *= num1376;
								projectile.velocity.X = (projectile.velocity.X * 50f + num1373) / 51f;
								projectile.velocity.Y = (projectile.velocity.Y * 50f + num1374) / 51f;
								counter = 90;
							}
						}
						else
						{
							projectile.velocity = Vector2.Normalize(value16) * 15f;
						}
					}
				}
			}
			if (projectile.alpha > 0)
			{
				projectile.alpha -= 25;
				if (projectile.alpha < 0)
				{
					projectile.alpha = 0;
				}
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			if (projectile.timeLeft > 515)
			{
				projectile.localAI[1] += 1f;
				byte b2 = (byte)(((int)projectile.localAI[1]) * 3);
				byte a2 = (byte)(projectile.alpha * ((float)b2 / 255f));
				return new Color((int)b2, (int)b2, (int)b2, (int)a2);
			}
			return new Color(255, 255, 255, projectile.alpha);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
			return false;
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(mod.BuffType("WhisperingDeath"), 300);
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 74);
			for (int num621 = 0; num621 < 5; num621++)
			{
				int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default(Color), 1f);
				Main.dust[num622].velocity *= 3f;
				if (Main.rand.Next(2) == 0)
				{
					Main.dust[num622].scale = 0.5f;
					Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
				}
			}
			for (int num623 = 0; num623 < 10; num623++)
			{
				int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default(Color), 1.5f);
				Main.dust[num624].noGravity = true;
				Main.dust[num624].velocity *= 5f;
				num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default(Color), 1f);
				Main.dust[num624].velocity *= 2f;
			}
		}
	}
}
