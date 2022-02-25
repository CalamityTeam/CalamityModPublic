using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
	public class ApolloFireball : ModProjectile
	{
		private const int timeLeft = 60;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Volatile Plasma Blast");
			Main.projFrames[projectile.type] = 6;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			projectile.Calamity().canBreakPlayerDefense = true;
			projectile.width = 48;
			projectile.height = 48;
			projectile.hostile = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.penetrate = -1;
			projectile.Opacity = 0f;
			cooldownSlot = 1;
			projectile.timeLeft = (CalamityWorld.malice || BossRushEvent.BossRushActive) ? 48 : timeLeft;
			projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(projectile.localAI[0]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			projectile.localAI[0] = reader.ReadSingle();
		}

		public override void AI()
		{
			Vector2 targetLocation = new Vector2(projectile.ai[0], projectile.ai[1]);
			if (Vector2.Distance(targetLocation, projectile.Center) < 80f)
				projectile.tileCollide = true;

			int fadeInTime = 3;
			projectile.Opacity = MathHelper.Clamp(1f - ((projectile.timeLeft - (((CalamityWorld.malice || BossRushEvent.BossRushActive) ? 48 : timeLeft) - fadeInTime)) / (float)fadeInTime), 0f, 1f);

			Lighting.AddLight(projectile.Center, 0f, 0.6f * projectile.Opacity, 0f);

			projectile.frameCounter++;
			if (projectile.frameCounter > 4)
			{
				projectile.frame++;
				projectile.frameCounter = 0;
			}
			if (projectile.frame > 5)
				projectile.frame = 0;

			projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) - MathHelper.PiOver2;

			if (projectile.localAI[0] == 0f)
			{
				projectile.localAI[0] = 1f;

				float speed1 = 1.8f;
				float speed2 = 2.8f;
				float angleRandom = 0.35f;

				for (int num53 = 0; num53 < 40; num53++)
				{
					float dustSpeed = Main.rand.NextFloat(speed1, speed2);
					Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(projectile.velocity.ToRotation());
					dustVel = dustVel.RotatedBy(-angleRandom);
					dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);
					int randomDustType = Main.rand.NextBool(2) ? 107 : 110;

					int num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDustType, dustVel.X, dustVel.Y, 200, default, 1.7f);
					Main.dust[num54].position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * projectile.width / 2f;
					Main.dust[num54].noGravity = true;

					Dust dust = Main.dust[num54];
					dust.velocity *= 3f;
					dust = Main.dust[num54];

					num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDustType, dustVel.X, dustVel.Y, 100, default, 0.8f);
					Main.dust[num54].position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * projectile.width / 2f;

					dust = Main.dust[num54];
					dust.velocity *= 2f;

					Main.dust[num54].noGravity = true;
					Main.dust[num54].fadeIn = 1f;
					Main.dust[num54].color = Color.Green * 0.5f;

					dust = Main.dust[num54];
				}
				for (int num55 = 0; num55 < 20; num55++)
				{
					float dustSpeed = Main.rand.NextFloat(speed1, speed2);
					Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(projectile.velocity.ToRotation());
					dustVel = dustVel.RotatedBy(-angleRandom);
					dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);
					int randomDustType = Main.rand.NextBool(2) ? 107 : 110;

					int num56 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDustType, dustVel.X, dustVel.Y, 0, default, 2f);
					Main.dust[num56].position = projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(projectile.velocity.ToRotation()) * projectile.width / 3f;
					Main.dust[num56].noGravity = true;

					Dust dust = Main.dust[num56];
					dust.velocity *= 0.5f;
					dust = Main.dust[num56];
				}
			}
		}

		public override bool CanHitPlayer(Player target) => projectile.Opacity == 1f;

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			if (projectile.Opacity != 1f)
				return;

			target.AddBuff(BuffID.OnFire, 360);
			target.AddBuff(BuffID.CursedInferno, 180);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			lightColor.R = (byte)(255 * projectile.Opacity);
			lightColor.G = (byte)(255 * projectile.Opacity);
			lightColor.B = (byte)(255 * projectile.Opacity);
			CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
			return false;
		}

		public override void Kill(int timeLeft)
		{
			int height = 90;
			projectile.position = projectile.Center;
			projectile.width = projectile.height = height;
			projectile.Center = projectile.position;
			projectile.Damage();

			Main.PlaySound(SoundID.Item93, projectile.Center);

			if (Main.myPlayer == projectile.owner)
			{
				bool splitNormal = true;
				if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1)
				{
					if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].active)
						splitNormal = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].ai[2] % 2f == 0f;
				}

				// Plasma bolts
				int totalProjectiles = (CalamityWorld.malice || BossRushEvent.BossRushActive) ? 6 : 4;
				float radians = MathHelper.TwoPi / totalProjectiles;
				int type = ModContent.ProjectileType<AresPlasmaBolt>();
				float velocity = 0.5f;
				double angleA = radians * 0.5;
				double angleB = MathHelper.ToRadians(90f) - angleA;
				float velocityX2 = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
				Vector2 spinningPoint = splitNormal ? new Vector2(0f, -velocity) : new Vector2(-velocityX2, -velocity);
				for (int k = 0; k < totalProjectiles; k++)
				{
					Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
					Projectile.NewProjectile(projectile.Center, velocity2, type, (int)Math.Round(projectile.damage * 0.8), 0f, Main.myPlayer);
				}
			}

			for (int num640 = 0; num640 < 200; num640++)
			{
				float num641 = 16f;
				if (num640 < 150)
					num641 = 12f;
				if (num640 < 100)
					num641 = 8f;
				if (num640 < 50)
					num641 = 4f;

				int num643 = Dust.NewDust(projectile.Center, 6, 6, Main.rand.NextBool(2) ? 107 : 110, 0f, 0f, 100, default, 1f);
				float num644 = Main.dust[num643].velocity.X;
				float num645 = Main.dust[num643].velocity.Y;

				if (num644 == 0f && num645 == 0f)
					num644 = 1f;

				float num646 = (float)Math.Sqrt(num644 * num644 + num645 * num645);
				num646 = num641 / num646;
				num644 *= num646;
				num645 *= num646;

				float scale = 1f;
				switch ((int)num641)
				{
					case 4:
						scale = 1.2f;
						break;
					case 8:
						scale = 1.1f;
						break;
					case 12:
						scale = 1f;
						break;
					case 16:
						scale = 0.9f;
						break;
					default:
						break;
				}

				Dust dust = Main.dust[num643];
				dust.velocity *= 0.5f;
				dust.velocity.X = dust.velocity.X + num644;
				dust.velocity.Y = dust.velocity.Y + num645;
				dust.scale = scale;
				dust.noGravity = true;
			}
		}

		public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
		{
			target.Calamity().lastProjectileHit = projectile;
		}
	}
}
