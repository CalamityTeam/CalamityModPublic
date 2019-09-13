using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class DoGFire : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Death Fire");
		}

		public override void SetDefaults()
		{
			projectile.width = 16;
			projectile.height = 16;
			projectile.hostile = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.alpha = 255;
			projectile.penetrate = -1;
			projectile.timeLeft = 480;
			projectile.scale = 1.25f;
			cooldownSlot = 1;
		}

		public override void AI()
		{
			if (projectile.localAI[0] == 0f)
			{
				projectile.localAI[0] = 1f;
				Main.PlaySound(SoundID.Item20, projectile.position);
			}

			int dust = Dust.NewDust(new Vector2(projectile.position.X + projectile.velocity.X, projectile.position.Y + projectile.velocity.Y), projectile.width, projectile.height, 173, projectile.velocity.X, projectile.velocity.Y, 100, default, 3f);
			Main.dust[dust].noGravity = true;

			projectile.rotation += 0.3f * (float)projectile.direction;

			if (projectile.ai[1] == 1f)
			{
				int num103 = (int)Player.FindClosest(projectile.Center, 1, 1);
				Vector2 vector11 = Main.player[num103].Center - projectile.Center;
				projectile.ai[0] += 1f;
				if (projectile.ai[0] >= 60f)
				{
					if (projectile.ai[0] < 300f)
					{
						float scaleFactor2 = projectile.velocity.Length();
						vector11.Normalize();
						vector11 *= scaleFactor2;
						projectile.velocity = (projectile.velocity * 24f + vector11) / 25f;
						projectile.velocity.Normalize();
						projectile.velocity *= scaleFactor2;
					}
					else if (projectile.velocity.Length() < 24f)
					{
						projectile.tileCollide = true;
						projectile.velocity *= 1.02f;
					}
				}
			}
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(mod.BuffType("GodSlayerInferno"), 300);
			target.AddBuff(BuffID.Frostburn, 300, true);
			target.AddBuff(BuffID.Darkness, 300, true);
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(SoundID.Item10, projectile.position);
			int num3;
			for (int num584 = 0; num584 < 20; num584 = num3 + 1)
			{
				int num585 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, -projectile.velocity.X * 0.2f, -projectile.velocity.Y * 0.2f, 100, default, 2.5f);
				Main.dust[num585].noGravity = true;
				Dust dust = Main.dust[num585];
				dust.velocity *= 2f;
				num585 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, -projectile.velocity.X * 0.2f, -projectile.velocity.Y * 0.2f, 100, default, 1.2f);
				dust = Main.dust[num585];
				dust.velocity *= 2f;
				num3 = num584;
			}
		}
	}
}
