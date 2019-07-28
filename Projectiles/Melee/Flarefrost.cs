using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class Flarefrost : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flarefrost");
		}

		public override void SetDefaults()
		{
			projectile.width = 10;
			projectile.height = 10;
			projectile.friendly = true;
			projectile.alpha = 255;
			projectile.penetrate = 1;
			projectile.timeLeft = 180;
			projectile.melee = true;
		}

		public override void AI()
		{
			Vector2 value7 = new Vector2(5f, 10f);
			Lighting.AddLight(projectile.Center, 0.25f, 0f, 0.25f);
			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] == 48f)
			{
				projectile.localAI[0] = 0f;
			}
			else
			{
				for (int num41 = 0; num41 < 2; num41++)
				{
					int dustType = (num41 == 0 ? 67 : 174);
					Vector2 value8 = Vector2.UnitX * -12f;
					value8 = -Vector2.UnitY.RotatedBy((double)(projectile.localAI[0] * 0.1308997f + (float)num41 * 3.14159274f), default(Vector2)) * value7 - projectile.rotation.ToRotationVector2() * 10f;
					int num42 = Dust.NewDust(projectile.Center, 0, 0, dustType, 0f, 0f, 160, default(Color), 1f);
					Main.dust[num42].scale = (dustType == 67 ? 1.5f : 1f);
					Main.dust[num42].noGravity = true;
					Main.dust[num42].position = projectile.Center + value8;
					Main.dust[num42].velocity = projectile.velocity;
					int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default(Color), 0.8f);
					Main.dust[num458].noGravity = true;
					Main.dust[num458].velocity *= 0f;
				}
			}
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
				float num483 = 11f;
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
			Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 27);
			for (int k = 0; k < 2; k++)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 67, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 174, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.OnFire, 300);
			target.AddBuff(BuffID.Frostburn, 300);
		}
	}
}
