using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
	public class VividBeam : ModProjectile
	{
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

		private bool initialized = false;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Beam");
		}

		public override void SetDefaults()
		{
			projectile.width = 4;
			projectile.height = 4;
			projectile.friendly = true;
			projectile.magic = true;
			projectile.ignoreWater = true;
			projectile.penetrate = 1;
			projectile.extraUpdates = 100;
			projectile.timeLeft = 240;
		}

		public override void AI()
		{
			if (!initialized)
			{
				initialized = true;
				float dustAmt = 16f;
				int d = 0;
				while ((float)d < dustAmt)
				{
					Vector2 offset = Vector2.UnitX * 0f;
					offset += -Vector2.UnitY.RotatedBy((double)((float)d * (MathHelper.TwoPi / dustAmt)), default) * new Vector2(1f, 4f);
					offset = offset.RotatedBy((double)projectile.velocity.ToRotation(), default);
					int i = Dust.NewDust(projectile.Center, 0, 0, 66, 0f, 0f, 0, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
					Main.dust[i].scale = 1.5f;
					Main.dust[i].noGravity = true;
					Main.dust[i].position = projectile.Center + offset;
					Main.dust[i].velocity = projectile.velocity * 0f + offset.SafeNormalize(Vector2.UnitY) * 1f;
					d++;
				}
			}

			float pi = MathHelper.Pi;
			projectile.ai[0] += 1f;
			if (projectile.ai[0] == 48f)
			{
				projectile.ai[0] = 0f;
			}
			else
			{
				for (int d = 0; d < 2; d++)
				{
					Vector2 offset = Vector2.UnitX * -12f;
					offset = -Vector2.UnitY.RotatedBy((double)(projectile.ai[0] * pi / 24f + (float)d * pi), default) * new Vector2(5f, 10f) - projectile.rotation.ToRotationVector2() * 10f;
					int i = Dust.NewDust(projectile.Center, 0, 0, 66, 0f, 0f, 160, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
					Main.dust[i].scale = 0.75f;
					Main.dust[i].noGravity = true;
					Main.dust[i].position = projectile.Center + offset;
					Main.dust[i].velocity = projectile.velocity;
				}
			}
			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] > 4f)
			{
				for (int d = 0; d < 2; d++)
				{
					Vector2 source = projectile.position;
					source -= projectile.velocity * ((float)d * 0.25f);
					int i = Dust.NewDust(source, 1, 1, 66, 0f, 0f, 0, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
					Main.dust[i].noGravity = true;
					Main.dust[i].position = source;
					Main.dust[i].scale = Main.rand.NextFloat(0.91f, 1.417f);
					Main.dust[i].velocity *= 0.1f;
				}
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.owner == Main.myPlayer)
			{
				SummonLasers();
			}
			return true;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (projectile.owner == Main.myPlayer)
			{
				SummonLasers();
			}
			target.ExoDebuffs();
		}

		public override void OnHitPvp(Player target, int damage, bool crit)
		{
			if (projectile.owner == Main.myPlayer)
			{
				SummonLasers();
			}
			target.ExoDebuffs();
		}

		private void SummonLasers()
		{
			switch (projectile.ai[1])
			{
				case 0f:
					CalamityUtils.ProjectileRain(projectile.Center, 400f, 100f, 500f, 800f, 6f, ModContent.ProjectileType<VividClarityBeam>(), (int)(projectile.damage * 0.7), projectile.knockBack, projectile.owner);
					break;

				case 1f:
					Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<VividExplosion>(), projectile.damage * 2, projectile.knockBack, projectile.owner);
					break;

				case 2f:
					float spread = 30f * 0.01f * MathHelper.PiOver2;
					double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
					double deltaAngle = spread / 8f;
					double offsetAngle;
					for (int i = 0; i < 4; i++)
					{
						offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<VividLaser2>(), projectile.damage, projectile.knockBack, projectile.owner);
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<VividLaser2>(), projectile.damage, projectile.knockBack, projectile.owner);
					}
					break;
			}
		}
	}
}
