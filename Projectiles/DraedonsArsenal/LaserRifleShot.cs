using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
	public class LaserRifleShot : ModProjectile
	{
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

		private int dust = 127;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Laser Rifle");
		}

		public override void SetDefaults()
		{
			projectile.width = 2;
			projectile.height = 2;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.penetrate = 1;
			projectile.extraUpdates = 100;
			projectile.timeLeft = 600;
		}

		public override void AI()
		{
			Lighting.AddLight(projectile.Center, 0.2f, 0.1f, 0f);

			float createDustVar = 12f;
			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] > createDustVar)
			{
				Vector2 value7 = new Vector2(5f, 10f);
				Vector2 value8 = Vector2.UnitX * -12f;

				for (int i = 0; i < 2; i++)
				{
					int num41 = Dust.NewDust(projectile.Center, 0, 0, dust, 0f, 0f, 160, projectile.ai[0] == 0f ? default : new Color(255, 255, 0), 2f);
					Main.dust[num41].noGravity = true;
					Main.dust[num41].position = projectile.Center;
					Main.dust[num41].velocity = projectile.velocity;
				}
			}

			if (projectile.localAI[0] == createDustVar)
				LaserBurst(1.8f, 3f);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.AddBuff(BuffID.OnFire, 180);
        }

		public override void Kill(int timeLeft)
		{
			int height = 60;
			projectile.position = projectile.Center;
			projectile.width = projectile.height = height;
			projectile.Center = projectile.position;
			projectile.maxPenetrate = -1;
			projectile.penetrate = -1;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 10;
			projectile.Damage();

			LaserBurst(2.4f, 4.2f); // 60 dusts

			for (int num640 = 0; num640 < 100; num640++)
			{
				float num641 = 4f;

				int num643 = Dust.NewDust(projectile.Center, 6, 6, dust, 0f, 0f, 100, default, 2f);
				float num644 = Main.dust[num643].velocity.X;
				float num645 = Main.dust[num643].velocity.Y;

				if (num644 == 0f && num645 == 0f)
					num644 = 1f;

				float num646 = (float)Math.Sqrt(num644 * num644 + num645 * num645);
				num646 = num641 / num646;
				num644 *= num646;
				num645 *= num646;

				Dust dust2 = Main.dust[num643];
				dust2.velocity *= 0.5f;
				dust2.velocity.X = dust2.velocity.X + num644;
				dust2.velocity.Y = dust2.velocity.Y + num645;
				dust2.noGravity = true;
			}
		}

		private void LaserBurst(float speed1, float speed2)
		{
			float angleRandom = 0.05f;

			for (int num53 = 0; num53 < 20; num53++)
			{
				float dustSpeed = Main.rand.NextFloat(speed1, speed2);
				Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(projectile.velocity.ToRotation());
				dustVel = dustVel.RotatedBy(-angleRandom);
				dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

				int num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dust, dustVel.X, dustVel.Y, 200, default, 2.5f);
				Main.dust[num54].position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * projectile.width / 2f;
				Main.dust[num54].noGravity = true;

				Dust dust2 = Main.dust[num54];
				dust2.velocity *= 3f;
				dust2 = Main.dust[num54];

				num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dust, dustVel.X, dustVel.Y, 100, default, 1.9f);
				Main.dust[num54].position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * projectile.width / 2f;

				dust2 = Main.dust[num54];
				dust2.velocity *= 2f;

				Main.dust[num54].noGravity = true;
				Main.dust[num54].fadeIn = 1f;
				Main.dust[num54].color = Color.Orange * 0.5f;

				dust2 = Main.dust[num54];
			}
			for (int num55 = 0; num55 < 10; num55++)
			{
				float dustSpeed = Main.rand.NextFloat(speed1, speed2);
				Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(projectile.velocity.ToRotation());
				dustVel = dustVel.RotatedBy(-angleRandom);
				dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

				int num56 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dust, dustVel.X, dustVel.Y, 0, default, 3.2f);
				Main.dust[num56].position = projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(projectile.velocity.ToRotation()) * projectile.width / 3f;
				Main.dust[num56].noGravity = true;

				Dust dust2 = Main.dust[num56];
				dust2.velocity *= 0.5f;
				dust2 = Main.dust[num56];
			}
		}
	}
}
