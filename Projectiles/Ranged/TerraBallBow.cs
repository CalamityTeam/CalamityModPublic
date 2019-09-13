using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class TerraBallBow : ModProjectile
    {
		private int dustType = 0;

    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ball");
		}

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
			projectile.extraUpdates = 1;
            projectile.tileCollide = true;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
			switch ((int)projectile.ai[1])
			{
				case 0:
					dustType = 15;
					break;
				case 1:
					dustType = 74;
					break;
				case 2:
					dustType = 73;
					break;
				case 3:
					dustType = 162;
					break;
				case 4:
					dustType = 90;
					break;
				case 5:
					dustType = 173;
					break;
				case 6:
					dustType = 57;
					break;
			}
			int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 2.2f);
			Main.dust[num469].noGravity = true;
			Main.dust[num469].velocity *= 0f;

			float num472 = projectile.Center.X;
			float num473 = projectile.Center.Y;
			float num474 = 400f;
			bool flag17 = false;
			for (int num475 = 0; num475 < 200; num475++)
			{
				if (Main.npc[num475].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num475].Center, 1, 1))
				{
					float num476 = Main.npc[num475].position.X + (float)(Main.npc[num475].width / 2);
					float num477 = Main.npc[num475].position.Y + (float)(Main.npc[num475].height / 2);
					float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
					if (num478 < num474)
					{
						num474 = num478;
						num472 = num476;
						num473 = num477;
						flag17 = true;
					}
				}
			}
			if (flag17)
			{
				float num483 = 18f;
				Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
				float num484 = num472 - vector35.X;
				float num485 = num473 - vector35.Y;
				float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
				num486 = num483 / num486;
				num484 *= num486;
				num485 *= num486;
				projectile.velocity.X = (projectile.velocity.X * 20f + num484) / 21f;
				projectile.velocity.Y = (projectile.velocity.Y * 20f + num485) / 21f;
			}
        }

		public override void Kill(int timeLeft)
		{
			int height = 40;
			float num50 = 2.1f;
			float num51 = 1.1f;
			float num52 = 2.5f;
			Vector2 value3 = (projectile.rotation - 1.57079637f).ToRotationVector2();
			Vector2 value4 = value3 * projectile.velocity.Length() * (float)projectile.MaxUpdates;
			Main.PlaySound(SoundID.Item14, projectile.position);
			projectile.position = projectile.Center;
			projectile.width = (projectile.height = height);
			projectile.Center = projectile.position;
			projectile.maxPenetrate = -1;
			projectile.penetrate = -1;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 10;
			projectile.Damage();
			int num3;
			for (int num53 = 0; num53 < 20; num53 = num3 + 1)
			{
				int num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 200, default, num50);
				Main.dust[num54].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
				Main.dust[num54].noGravity = true;
				Dust dust = Main.dust[num54];
				dust.velocity *= 3f;
				dust = Main.dust[num54];
				dust.velocity += value4 * Main.rand.NextFloat();
				num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, num51);
				Main.dust[num54].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
				dust = Main.dust[num54];
				dust.velocity *= 2f;
				Main.dust[num54].noGravity = true;
				Main.dust[num54].fadeIn = 1f;
				dust = Main.dust[num54];
				dust.velocity += value4 * Main.rand.NextFloat();
				num3 = num53;
			}
			for (int num55 = 0; num55 < 10; num55 = num3 + 1)
			{
				int num56 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 0, default, num52);
				Main.dust[num56].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)projectile.velocity.ToRotation(), default) * (float)projectile.width / 3f;
				Main.dust[num56].noGravity = true;
				Dust dust = Main.dust[num56];
				dust.velocity *= 0.5f;
				dust = Main.dust[num56];
				dust.velocity += value4 * (0.6f + 0.6f * Main.rand.NextFloat());
				num3 = num55;
			}
		}
	}
}
