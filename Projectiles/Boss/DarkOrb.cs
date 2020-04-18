using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class DarkOrb : ModProjectile
    {
		private bool start = true;
		private Vector2 center = Vector2.Zero;
		private double distance = 0D;
		private double distance2 = 0D;
		private Vector2 velocity = Vector2.Zero;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Orb");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 3600;
            cooldownSlot = 1;
        }

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(projectile.localAI[0]);
			writer.Write(start);
			writer.WriteVector2(center);
			writer.Write(distance);
			writer.Write(distance2);
			writer.WriteVector2(velocity);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			projectile.localAI[0] = reader.ReadSingle();
			start = reader.ReadBoolean();
			center = reader.ReadVector2();
			distance = reader.ReadDouble();
			distance2 = reader.ReadDouble();
			velocity = reader.ReadVector2();
		}

		public override void AI()
        {
			// Pulse in and out
			if (projectile.localAI[1] == 0f)
			{
				projectile.scale -= 0.006f;
				if (projectile.scale <= 0.9f)
				{
					projectile.scale = 0.9f;
					projectile.localAI[1] = 1f;
				}
			}
			else
			{
				projectile.scale += 0.006f;
				if (projectile.scale >= 1.1f)
				{
					projectile.scale = 1.1f;
					projectile.localAI[1] = 0f;
				}
			}

			switch ((int)projectile.ai[0])
			{
				// No AI
				case -1:
					break;

				// Rotate around a point and spread outward
				case 0:
				case 1:

					if (start)
					{
						center = projectile.Center;
						velocity = Vector2.Normalize(Main.player[(int)Player.FindClosest(projectile.Center, 1, 1)].Center - projectile.Center) * 2f;
						start = false;
					}

					center += velocity;

					double rad = MathHelper.ToRadians(projectile.ai[1]);

					float amount = 1f - projectile.localAI[0] / 360f;
					if (amount < 0f)
						amount = 0f;

					distance += (double)MathHelper.Lerp(1f, 6f, amount);

					if (projectile.ai[0] == 0f)
					{
						projectile.position.X = center.X - (int)(Math.Sin(rad) * distance) - projectile.width / 2;
						projectile.position.Y = center.Y - (int)(Math.Cos(rad) * distance) - projectile.height / 2;
					}
					else
					{
						projectile.position.X = center.X - (int)(Math.Cos(rad) * distance) - projectile.width / 2;
						projectile.position.Y = center.Y - (int)(Math.Sin(rad) * distance) - projectile.height / 2;
					}

					projectile.ai[1] += 0.25f + amount;
					projectile.localAI[0] += 1f;

					break;

				// Lurch forward
				case 2:

					projectile.ai[1] += 0.05f;

					projectile.velocity *= MathHelper.Lerp(0.95f, 1.05f, (float)Math.Abs(Math.Sin(projectile.ai[1])));

					break;

				// Wavy motion
				case 3:
				case 4:

					if (start)
					{
						velocity = projectile.velocity;
						start = false;
					}

					projectile.ai[1] += 0.1f;

					float wavyVelocity = projectile.ai[0] == 3f ? (float)Math.Sin(projectile.ai[1]) : (float)Math.Cos(projectile.ai[1]);

					projectile.velocity = velocity + new Vector2(wavyVelocity, wavyVelocity) * velocity.Length();

					break;

				// Speed up then slow down on a timer
				case 5:

					projectile.ai[1] += 1f;

					float velocityGateValue = 90f;
					float velocityGateValue2 = 30f;
					if (projectile.ai[1] <= velocityGateValue)
					{
						if (projectile.velocity.Length() > 3f)
							projectile.velocity *= 0.95f;
					}
					else if (projectile.ai[1] <= velocityGateValue + velocityGateValue2)
					{
						if (projectile.velocity.Length() < 18f)
							projectile.velocity *= 1.1f;
					}
					else if (projectile.ai[1] == velocityGateValue + velocityGateValue2)
					{
						projectile.ai[1] = 0f;
					}

					break;

				// Split into 4 projectiles that can use any of the first 6 AIs
				case 6:

					projectile.ai[1] += 1f;

					if (projectile.ai[1] >= 90f)
					{
						if (projectile.owner == Main.myPlayer)
						{
							for (int i = 0; i < 4; i++)
							{
								Vector2 vector = new Vector2(0f, -8f).RotatedBy((double)(MathHelper.PiOver2 * (float)i), default);
								Projectile.NewProjectile(projectile.Center, vector, projectile.type, projectile.damage, 0f, Main.myPlayer, (float)Main.rand.Next(6), 0f);
							}
						}

						projectile.Kill();
					}

					break;

				// Split into 8 projectiles that can use either of the first 2 AIs
				case 7:

					projectile.ai[1] += 1f;

					if (projectile.ai[1] >= 120f)
					{
						if (projectile.owner == Main.myPlayer)
						{
							for (int i = 0; i < 8; i++)
							{
								Vector2 vector = new Vector2(0f, -8f).RotatedBy((double)(MathHelper.PiOver4 * (float)i), default);
								Projectile.NewProjectile(projectile.Center, vector, projectile.type, projectile.damage, 0f, Main.myPlayer, (float)Main.rand.Next(2), 0f);
							}
						}

						projectile.Kill();
					}

					break;

				// Split into 3 projectiles multiple times
				case 8:

					projectile.ai[1] += 1f;

					if (projectile.ai[1] >= 120f)
					{
						if (projectile.owner == Main.myPlayer)
						{
							for (int i = 0; i < 3; i++)
							{
								Vector2 vector = new Vector2(0f, -8f).RotatedBy((double)(MathHelper.Pi * 0.66666666667f * (float)i), default);
								int proj = Projectile.NewProjectile(projectile.Center, vector, projectile.type, projectile.damage, 0f, Main.myPlayer, 8f, 0f);
								Main.projectile[proj].timeLeft = projectile.timeLeft / 4;
							}
						}

						projectile.Kill();
					}

					break;

				// Walls
				case 9:
				case 10:

					projectile.ai[1] += 1f;

					if (projectile.ai[1] >= 90f)
					{
						if (projectile.ai[0] == 9f)
							projectile.velocity.X = 0f;
						else
							projectile.velocity.Y = 0f;
					}

					break;

				// Homing arc
				case 11:

					projectile.ai[1] += 1f;

					if (projectile.ai[1] >= 180f)
					{
						projectile.localAI[0] += 1f;
						if (projectile.localAI[0] < 180f)
						{
							Vector2 vector = Main.player[(int)Player.FindClosest(projectile.Center, 1, 1)].Center - projectile.Center;
							float scaleFactor = projectile.velocity.Length();
							vector.Normalize();
							vector *= scaleFactor;
							projectile.velocity = (projectile.velocity * 15f + vector) / 16f;
							projectile.velocity.Normalize();
							projectile.velocity *= scaleFactor;
						}
						else if (projectile.velocity.Length() < 18f)
							projectile.velocity *= 1.01f;
					}

					break;

				// Rotate around a moving point and spread outward then inward in an oval pattern
				case 12:
				case 13:

					if (start)
					{
						center = projectile.Center;
						velocity = Vector2.Normalize(Main.player[(int)Player.FindClosest(projectile.Center, 1, 1)].Center - projectile.Center);
						start = false;
					}

					center += velocity;

					float velocityGateValue3 = 240f;
					float amount2 = 1f;

					bool flyOutward = projectile.localAI[0] < velocityGateValue3;

					if (flyOutward)
						amount2 -= projectile.localAI[0] / velocityGateValue3;
					else
						amount2 = (projectile.localAI[0] - velocityGateValue3) / velocityGateValue3;

					amount2 = MathHelper.Clamp(0f, 1f, amount2);

					double distanceVariable = (double)MathHelper.Lerp(0f, 6f, amount2);

					distance += (flyOutward ? distanceVariable : -distanceVariable);

					double rad2 = MathHelper.ToRadians(projectile.ai[1]);

					if (projectile.ai[0] == 12f)
					{
						projectile.position.X = center.X - (int)(Math.Sin(rad2) * distance * 2D) - projectile.width / 2;
						projectile.position.Y = center.Y - (int)(Math.Cos(rad2) * distance) - projectile.height / 2;
					}
					else
					{
						projectile.position.X = center.X - (int)(Math.Cos(rad2) * distance) - projectile.width / 2;
						projectile.position.Y = center.Y - (int)(Math.Sin(rad2) * distance * 2D) - projectile.height / 2;
					}

					projectile.ai[1] += 0.25f + amount2;
					projectile.localAI[0] += 1f;

					break;
			}
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, 100, 255, projectile.alpha);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);
            //target.AddBuff(ModContent.BuffType<Delirium>(), 300, true);
			target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 300);
			target.AddBuff(ModContent.BuffType<Horror>(), 300, true);
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)	
        {
			target.Calamity().lastProjectileHit = projectile;
		}
    }
}
