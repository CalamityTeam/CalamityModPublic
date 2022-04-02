using CalamityMod.Buffs.DamageOverTime;
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
        public override string Texture => "CalamityMod/Projectiles/LightningProj";

		private bool start = true;
		private Vector2 center = Vector2.Zero;
		private Vector2 velocity = Vector2.Zero;
		private double[] distances = new double[4];

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
			writer.Write(projectile.localAI[1]);
			writer.Write(start);
			writer.WriteVector2(center);
			writer.Write(distances[0]);
			writer.Write(distances[1]);
			writer.Write(distances[2]);
			writer.Write(distances[3]);
			writer.WriteVector2(velocity);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			projectile.localAI[0] = reader.ReadSingle();
			projectile.localAI[1] = reader.ReadSingle();
			start = reader.ReadBoolean();
			center = reader.ReadVector2();
			distances[0] = reader.ReadDouble();
			distances[1] = reader.ReadDouble();
			distances[2] = reader.ReadDouble();
			distances[3] = reader.ReadDouble();
			velocity = reader.ReadVector2();
		}

		public override void AI()
        {
			if (projectile.ai[0] != 8f)
			{
				// Pulse in and out
				if (projectile.localAI[1] == 0f)
				{
					projectile.scale -= 0.012f;
					if (projectile.scale <= 0.8f)
					{
						projectile.scale = 0.8f;
						projectile.localAI[1] = 1f;
					}
				}
				else
				{
					projectile.scale += 0.012f;
					if (projectile.scale >= 1.2f)
					{
						projectile.scale = 1.2f;
						projectile.localAI[1] = 0f;
					}
				}
			}

			switch ((int)projectile.ai[0])
			{
				// No AI
				case -1:
					break;

				// Rotate around a point, spread outward and move
				case 0:
				case 1:

					if (start)
					{
						center = projectile.Center;
						velocity = Vector2.Normalize(Main.player[Player.FindClosest(projectile.Center, 1, 1)].Center - projectile.Center) * 2f;
						start = false;
					}

					center += velocity;

					double rad = MathHelper.ToRadians(projectile.ai[1]);

					float amount = 1f - projectile.localAI[0] / 360f;
					if (amount < 0f)
						amount = 0f;

					distances[0] += MathHelper.Lerp(1f, 6f, amount);

					if (projectile.ai[0] == 0f)
					{
						projectile.position.X = center.X - (int)(Math.Sin(rad) * distances[0]) - projectile.width / 2;
						projectile.position.Y = center.Y - (int)(Math.Cos(rad) * distances[0]) - projectile.height / 2;
					}
					else
					{
						projectile.position.X = center.X - (int)(Math.Cos(rad) * distances[0]) - projectile.width / 2;
						projectile.position.Y = center.Y - (int)(Math.Sin(rad) * distances[0]) - projectile.height / 2;
					}

					projectile.ai[1] += 0.25f + amount;
					projectile.localAI[0] += 1f;

					break;

				// Lurch forward
				case 2:

					LurchForward(0.05f, 0.95f, 1.05f);

					break;

				// Wavy motion
				case 3:
				case 4:

					bool useSin = projectile.ai[0] == 3f;

					WavyMotion(0.1f, 0f, useSin, false);

					break;

				// Speed up then slow down on a timer
				case 5:
				case 16:

					float fastGateValue = projectile.ai[0] == 5f ? 30f : 60f;
					float maxVelocity = projectile.ai[0] == 5f ? 12f : 6f;

					LurchForwardOnTimer(90f, fastGateValue, 3f, maxVelocity, 0.95f, 1.2f);

					break;

				// Split into 4 projectiles that can use any of the first 6 AIs
				case 6:

					projectile.ai[1] += 1f;

					if (projectile.ai[1] >= 90f)
					{
						if (projectile.owner == Main.myPlayer)
						{
							int totalProjectiles = 4;
							float radians = MathHelper.TwoPi / totalProjectiles;
							for (int i = 0; i < totalProjectiles; i++)
							{
								Vector2 vector = new Vector2(0f, -8f).RotatedBy(radians * i);
								Projectile.NewProjectile(projectile.Center, vector, projectile.type, projectile.damage, 0f, Main.myPlayer, Main.rand.Next(6), 0f);
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
							int totalProjectiles = 8;
							float radians = MathHelper.TwoPi / totalProjectiles;
							for (int i = 0; i < totalProjectiles; i++)
							{
								Vector2 vector = new Vector2(0f, -8f).RotatedBy(radians * i);
								Projectile.NewProjectile(projectile.Center, vector, projectile.type, projectile.damage, 0f, Main.myPlayer, Main.rand.Next(2), 0f);
							}
						}

						projectile.Kill();
					}

					break;

				// Split into 3 wavy projectiles multiple times
				case 8:

					bool splitOnce = projectile.timeLeft < 1800;
					bool splitTwice = projectile.timeLeft < 900;
					bool splitThrice = projectile.timeLeft < 450;

					if (splitOnce)
						WavyMotion(0.05f, 2f, true, true);

					projectile.localAI[0] += 1f;

					if (projectile.localAI[0] >= 180f)
					{
						if (projectile.owner == Main.myPlayer)
						{
							int totalProjectiles = 3;
							float radians = MathHelper.TwoPi / totalProjectiles;
							int spread = 8;

							if (splitOnce)
							{
								spread *= (splitTwice ? 3 : 0) +
									(splitThrice ? 3 : 0);
							}

							Vector2 vector = Main.player[Player.FindClosest(projectile.Center, 1, 1)].Center - projectile.Center;
							vector.Normalize();
							vector *= 3f;

							for (int i = 0; i < totalProjectiles; i++)
							{
								Vector2 vector2 = splitOnce ? projectile.velocity.RotatedBy(MathHelper.ToRadians(i * spread)) : new Vector2(0f, -3f).RotatedBy(radians * i);

								int proj = Projectile.NewProjectile(projectile.Center, vector + vector2, projectile.type, projectile.damage, 0f, Main.myPlayer, 8f, 0f);

								Main.projectile[proj].timeLeft = projectile.timeLeft / 2;
								if (Main.projectile[proj].timeLeft < 150)
									Main.projectile[proj].timeLeft = 150;

								Main.projectile[proj].scale = projectile.scale * 0.6f;
							}
						}

						projectile.Kill();
					}

					break;

				// idk yet
				case 9:
				case 10:

					bool useSin2 = projectile.ai[0] == 9f;

					OscillationMotion(0.05f, useSin2);

					break;

				// Homing
				case 11:

					projectile.ai[1] += 1f;

					if (projectile.ai[1] >= 180f)
					{
						projectile.localAI[0] += 1f;
						if (projectile.localAI[0] < 180f)
						{
							Vector2 vector = Main.player[Player.FindClosest(projectile.Center, 1, 1)].Center - projectile.Center;
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
						velocity = Vector2.Normalize(Main.player[Player.FindClosest(projectile.Center, 1, 1)].Center - projectile.Center);
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

					double distanceVariable = MathHelper.Lerp(0f, 6f, amount2);

					distances[0] += flyOutward ? distanceVariable : -distanceVariable;

					double rad2 = MathHelper.ToRadians(projectile.ai[1]);

					if (projectile.ai[0] == 12f)
					{
						projectile.position.X = center.X - (int)(Math.Sin(rad2) * distances[0] * 2D) - projectile.width / 2;
						projectile.position.Y = center.Y - (int)(Math.Cos(rad2) * distances[0]) - projectile.height / 2;
					}
					else
					{
						projectile.position.X = center.X - (int)(Math.Cos(rad2) * distances[0]) - projectile.width / 2;
						projectile.position.Y = center.Y - (int)(Math.Sin(rad2) * distances[0] * 2D) - projectile.height / 2;
					}

					projectile.ai[1] += 0.25f + amount2;
					projectile.localAI[0] += 1f;

					break;

				// Spread out then drift inward after some time and then drift outward
				case 14:
				case 15:

					float velocityMult = 12f;

					if (start)
					{
						center = projectile.Center;
						velocity = Vector2.Normalize(Main.player[Player.FindClosest(projectile.Center, 1, 1)].Center - projectile.Center) * velocityMult;
						start = false;
					}

					center += velocity;

					float flyInwardGateValue = 240f;

					float amount3 = 1f - projectile.localAI[0] / flyInwardGateValue;
					if (amount3 < 0f)
						amount3 = 0f;

					bool changeXPos = projectile.ai[0] == 14f;

					float units = 6f;

					distances[0] += MathHelper.Lerp(1f, units, amount3);

					if (projectile.localAI[0] > flyInwardGateValue)
					{
						distances[2] = changeXPos ? Math.Abs(center.X - projectile.Center.X) : Math.Abs(center.Y - projectile.Center.Y);

						if (distances[3] == 0D)
							distances[3] = distances[2] - units * 10f;

						distances[3] += changeXPos ? velocity.X : velocity.Y;

						distances[1] -= distances[2] < (units * 10f) ? distances[2] * 0.1f : MathHelper.Lerp(units, units + 2f, (float)(distances[2] / distances[3]));
					}
					else
					{
						float slowDownGateValue = 30f;

						if (projectile.localAI[0] > flyInwardGateValue - slowDownGateValue)
							velocity *= 1f - velocityMult / slowDownGateValue;

						distances[1] = distances[0];
					}

					double distanceX = changeXPos ? distances[1] : distances[0];
					double distanceY = changeXPos ? distances[0] : distances[1];

					double rad3 = MathHelper.ToRadians(projectile.ai[1]);

					projectile.position.X = center.X - (int)(Math.Sin(rad3) * distanceX) - projectile.width / 2;
					projectile.position.Y = center.Y - (int)(Math.Cos(rad3) * distanceY) - projectile.height / 2;

					projectile.localAI[0] += 1f;

					if (projectile.localAI[0] > 600f)
						projectile.Kill();

					break;
			}
        }

		private void WavyMotion(float frequency, float amplitude, bool useSin, bool waveWithVelocity)
		{
			if (start)
			{
				velocity = projectile.velocity;
				start = false;
			}

			projectile.ai[1] += frequency;

			if (amplitude == 0f)
				amplitude = velocity.Length();

			float wavyVelocity = useSin ? (float)Math.Sin(projectile.ai[1]) : (float)Math.Cos(projectile.ai[1]);

			if (waveWithVelocity)
				projectile.velocity = velocity + new Vector2(wavyVelocity, wavyVelocity) * amplitude;
			else
				projectile.velocity = velocity + new Vector2(wavyVelocity, wavyVelocity).RotatedBy(MathHelper.ToRadians(velocity.ToRotation())) * amplitude;
		}

		private void LurchForward(float frequncy, float deceleration, float acceleration)
		{
			projectile.ai[1] += frequncy;

			projectile.velocity *= MathHelper.Lerp(deceleration, acceleration, (float)Math.Abs(Math.Sin(projectile.ai[1])));
		}

		private void LurchForwardOnTimer(float slowGateValue, float fastGateValue, float minVelocity, float maxVelocity, float deceleration, float acceleration)
		{
			projectile.ai[1] += 1f;

			if (projectile.ai[1] <= slowGateValue)
			{
				if (projectile.velocity.Length() > minVelocity)
					projectile.velocity *= deceleration;
			}
			else if (projectile.ai[1] < slowGateValue + fastGateValue)
			{
				if (projectile.velocity.Length() < maxVelocity)
					projectile.velocity *= acceleration;
			}
			else
				projectile.ai[1] = 0f;
		}

		private void OscillationMotion(float frequncy, bool useSin)
		{
			projectile.ai[1] += frequncy;

			float oscillation = useSin ? (float)Math.Sin(projectile.ai[1]) : (float)Math.Cos(projectile.ai[1]);

			if (start)
			{
				velocity = projectile.velocity;
				start = false;
			}
			else if (oscillation == 0f)
			{
				Player target = Main.player[Player.FindClosest(projectile.Center, 1, 1)];
				Vector2 vector = target.Center + target.velocity * 20f - projectile.Center;
				vector.Normalize();
				vector *= velocity.Length();
				projectile.velocity = vector;
			}
			else
			{
				float amplitude = velocity.Length();

				projectile.velocity.Normalize();
				projectile.velocity *= amplitude * oscillation;
			}
		}

		public override Color? GetAlpha(Color lightColor)
        {
			switch ((int)projectile.ai[0])
			{
				// No AI
				case -1:

					return new Color(0, 100, 255, projectile.alpha);

				// Rotate around a point, spread outward and move
				case 0:
				case 1:

					return new Color(0, 100, 255, projectile.alpha);

				// Lurch forward
				case 2:

					return new Color(255, 200, 0, projectile.alpha);

				// Wavy motion
				case 3:
				case 4:

					return new Color(0, 255, 200, projectile.alpha);

				// Speed up then slow down on a timer
				case 5:
				case 16:

					return new Color(0, 255, 100, projectile.alpha);

				// Split into 4 projectiles that can use any of the first 6 AIs
				case 6:

					return new Color(200, 0, 255, projectile.alpha);

				// Split into 8 projectiles that can use either of the first 2 AIs
				case 7:

					return new Color(255, 0, 150, projectile.alpha);

				// Split into 3 wavy projectiles multiple times
				case 8:

					return new Color(200, 0, 0, projectile.alpha);

				// idk yet
				case 9:
				case 10:

					return new Color(255, 255, 0, projectile.alpha);

				// Homing
				case 11:

					return new Color(150, 0, 200, projectile.alpha);

				// Rotate around a moving point and spread outward then inward in an oval pattern
				case 12:
				case 13:

					return new Color(0, 100, 255, projectile.alpha);

				// Spread out then drift inward after some time and then drift outward
				case 14:
				case 15:

					return new Color(0, 100, 255, projectile.alpha);
			}

			return null;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);
            //target.AddBuff(ModContent.BuffType<Delirium>(), 300, true);
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)	
        {
			target.Calamity().lastProjectileHit = projectile;
		}
    }
}
