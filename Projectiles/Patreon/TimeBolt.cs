using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Patreon
{
    public class TimeBolt : ModProjectile
	{
		private int penetrationAmt = 6;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Time Bolt");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			projectile.width = 28;
			projectile.height = 28;
			projectile.friendly = true;
			projectile.penetrate = penetrationAmt;
			projectile.timeLeft = 600;
			projectile.Calamity().rogue = true;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 5;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(penetrationAmt);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			penetrationAmt = reader.ReadInt32();
		}

		public override void AI()
		{
			projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.03f;

			// If projectile hasn't hit anything yet
			if (projectile.ai[0] == 0f)
			{
				projectile.tileCollide = true;
				projectile.localAI[0] += 1f;
				if (projectile.localAI[0] > 7f)
				{
					int dustType = Utils.SelectRandom<int>(Main.rand, new int[]
					{
						226,
						229
					});
					Vector2 center = projectile.Center;
					Vector2 vector74 = new Vector2(-4f, 4f);
					vector74 += new Vector2(-4f, 4f);
					vector74 = vector74.RotatedBy((double)projectile.rotation, default);
					int dust = Dust.NewDust(center + vector74 + Vector2.One * -4f, 8, 8, dustType, 0f, 0f, 100, default, 1f);
					Dust dust2 = Main.dust[dust];
					dust2.velocity *= 0.1f;
					if (Main.rand.Next(6) != 0)
						dust2.noGravity = true;
				}
				float num773 = 0.01f;
				int num774 = 5;
				int num775 = num774 * 15;
				int num776 = 0;
				if (projectile.localAI[0] > 7f)
				{
					if (projectile.localAI[1] == 0f)
					{
						projectile.scale -= num773;

						projectile.alpha += num774;
						if (projectile.alpha > num775)
						{
							projectile.alpha = num775;
							projectile.localAI[1] = 1f;
						}
					}
					else if (projectile.localAI[1] == 1f)
					{
						projectile.scale += num773;

						projectile.alpha -= num774;
						if (projectile.alpha <= num776)
						{
							projectile.alpha = num776;
							projectile.localAI[1] = 0f;
						}
					}
				}
			}

			// If projectile has hit an enemy and has 'split'
			else if (projectile.ai[0] >= 1f && projectile.ai[0] < (float)(1 + penetrationAmt))
			{
				projectile.tileCollide = false;
				projectile.alpha += 15;
				projectile.velocity *= 0.98f;
				projectile.localAI[0] = 0f;

				if (projectile.alpha >= 255)
				{
					if (projectile.ai[0] == 1f)
					{
						projectile.Kill();
						return;
					}

					int whoAmI = -1;
					Vector2 value19 = projectile.Center;
					float num778 = 300f;
					int j;
					for (int i = 0; i < 200; i = j + 1)
					{
						NPC nPC = Main.npc[i];
						if (nPC.CanBeChasedBy(projectile, false))
						{
							Vector2 center = nPC.Center;
							float num780 = Vector2.Distance(center, projectile.Center);
							if (num780 < num778)
							{
								num778 = num780;
								value19 = center;
								whoAmI = i;
							}
						}
						j = i;
					}

					if (whoAmI >= 0)
					{
						projectile.netUpdate = true;
						projectile.ai[0] += (float)penetrationAmt;
						projectile.position = value19 + ((float)Main.rand.NextDouble() * 6.28318548f).ToRotationVector2() * 100f - new Vector2((float)projectile.width, (float)projectile.height) / 2f;
						projectile.velocity = Vector2.Normalize(value19 - projectile.Center) * 18f;
					}
					else
						projectile.Kill();
				}

				if (Main.rand.NextBool(3))
				{
					int dustType = Utils.SelectRandom<int>(Main.rand, new int[]
					{
						226,
						229
					});
					Vector2 center = projectile.Center;
					Vector2 vector75 = new Vector2(-4f, 4f);
					vector75 += new Vector2(-4f, 4f);
					vector75 = vector75.RotatedBy((double)projectile.rotation, default);
					int dust = Dust.NewDust(center + vector75 + Vector2.One * -4f, 8, 8, dustType, 0f, 0f, 100, default, 0.6f);
					Dust dust2 = Main.dust[dust];
					dust2.velocity *= 0.1f;
					dust2.noGravity = true;
				}
			}

			// If 'split' projectile has a target
			else if (projectile.ai[0] >= (float)(1 + penetrationAmt) && projectile.ai[0] < (float)(1 + penetrationAmt * 2))
			{
				projectile.scale = 0.9f;
				projectile.tileCollide = false;

				projectile.ai[1] += 1f;
				if (projectile.ai[1] >= 15f)
				{
					projectile.alpha += 51;
					projectile.velocity *= 0.8f;

					if (projectile.alpha >= 255)
						projectile.Kill();
				}
				else
				{
					projectile.alpha -= 125;
					if (projectile.alpha < 0)
						projectile.alpha = 0;

					projectile.velocity *= 0.98f;
				}

				projectile.localAI[0] += 1f;

				int dustType = Utils.SelectRandom<int>(Main.rand, new int[]
				{
					226,
					229
				});
				Vector2 center = projectile.Center;
				Vector2 vector76 = new Vector2(-4f, 4f);
				vector76 += new Vector2(-4f, 4f);
				vector76 = vector76.RotatedBy((double)projectile.rotation, default);
				int dust = Dust.NewDust(center + vector76 + Vector2.One * -4f, 8, 8, dustType, 0f, 0f, 100, default, 0.6f);
				Dust dust2 = Main.dust[dust];
				dust2.velocity *= 0.1f;
				dust2.noGravity = true;
			}

			float colorScale = (float)projectile.alpha / 255f;
			Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 0.3f * colorScale, 0.4f * colorScale, 1f * colorScale);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 255, 255, 200) * ((255f - (float)projectile.alpha) / 255f);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
			return false;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			projectile.ai[0] = 1f;
			projectile.ai[1] = 0f;
			projectile.netUpdate = true;
			projectile.velocity = oldVelocity / 2f;

			if (penetrationAmt == 6)
				SlowTime();

			penetrationAmt = 2;

			return false;
		}

		public override bool CanDamage()
		{
			// Do not do damage if a tile is hit OR if projectile has 'split' and hasn't been live for more than 5 frames
			if ((((int)(projectile.ai[0] - 1f) / penetrationAmt == 0 && penetrationAmt < 3) || projectile.ai[1] < 5f) && projectile.ai[0] != 0f)
				return false;
			return true;
		}

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (penetrationAmt == 6)
				SlowTime();

			// If 'split' projectile hits an enemy
			if (projectile.ai[0] >= (float)(1 + penetrationAmt) && projectile.ai[0] < (float)(1 + penetrationAmt * 2))
				projectile.ai[0] = 0f;

			// Becomes 5 on first hit, then 4, and so on
			penetrationAmt--;

			// Hits enemy, ai[0] = 0f + 4f = 4f on first hit
			// ai[0] = 4f - 1f = 3f on second hit
			// ai[0] = 3f - 1f = 2f on third hit
			if (projectile.ai[0] == 0f)
				projectile.ai[0] += (float)penetrationAmt;
			else
				projectile.ai[0] -= (float)(penetrationAmt + 1);

			projectile.ai[1] = 0f;
			projectile.netUpdate = true;
		}

		private void SlowTime()
		{
			Main.PlaySound(SoundID.Item114, projectile.Center);

			int radius = 300;
			int numDust = (int)(0.2f * MathHelper.TwoPi * radius);
			float angleIncrement = MathHelper.TwoPi / (float)numDust;
			Vector2 dustOffset = new Vector2(radius, 0f);
			dustOffset = dustOffset.RotatedByRandom(MathHelper.TwoPi);
			int dustType = 0;
			int dust = 0;
			for (int i = 0; i < numDust; i++)
			{
				dustOffset = dustOffset.RotatedBy(angleIncrement);
				dustType = Utils.SelectRandom<int>(Main.rand, new int[]
				{
					226,
					229
				});
				dust = Dust.NewDust(projectile.Center, 1, 1, dustType);
				Main.dust[dust].position = projectile.Center + dustOffset;
				if (Main.rand.Next(6) != 0)
					Main.dust[dust].noGravity = true;
				Main.dust[dust].fadeIn = 1f;
				Main.dust[dust].velocity *= 0f;
				Main.dust[dust].scale = 0.3f;
			}

			int buffType = mod.BuffType("TimeSlow");
			int damage = projectile.damage / 2;

			for (int i = 0; i < 200; i++)
			{
				NPC nPC = Main.npc[i];
				if (nPC.active && !nPC.dontTakeDamage && !nPC.buffImmune[buffType] && Vector2.Distance(projectile.Center, nPC.Center) <= (float)radius)
				{
					if (nPC.FindBuffIndex(buffType) == -1)
						nPC.AddBuff(buffType, 180, false);
				}
			}
		}
	}
}
