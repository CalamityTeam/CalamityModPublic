using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class BloodBall : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blood Ball");
		}

		public override void SetDefaults()
		{
			projectile.width = 20;
			projectile.height = 20;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.timeLeft = 200;
			projectile.melee = true;
			projectile.tileCollide = false;
		}

		public override void AI()
		{
			try
			{
				int num223 = (int)(projectile.position.X / 16f) - 1;
				int num224 = (int)((projectile.position.X + (float)projectile.width) / 16f) + 2;
				int num225 = (int)(projectile.position.Y / 16f) - 1;
				int num226 = (int)((projectile.position.Y + (float)projectile.height) / 16f) + 2;
				if (num223 < 0)
				{
					num223 = 0;
				}
				if (num224 > Main.maxTilesX)
				{
					num224 = Main.maxTilesX;
				}
				if (num225 < 0)
				{
					num225 = 0;
				}
				if (num226 > Main.maxTilesY)
				{
					num226 = Main.maxTilesY;
				}
				for (int num227 = num223; num227 < num224; num227++)
				{
					for (int num228 = num225; num228 < num226; num228++)
					{
						if (Main.tile[num227, num228] != null && Main.tile[num227, num228].nactive() && (Main.tileSolid[(int)Main.tile[num227, num228].type] || (Main.tileSolidTop[(int)Main.tile[num227, num228].type] && Main.tile[num227, num228].frameY == 0)))
						{
							Vector2 vector19;
							vector19.X = (float)(num227 * 16);
							vector19.Y = (float)(num228 * 16);
							if (projectile.position.X + (float)projectile.width - 4f > vector19.X && projectile.position.X + 4f < vector19.X + 16f && projectile.position.Y + (float)projectile.height - 4f > vector19.Y && projectile.position.Y + 4f < vector19.Y + 16f)
							{
								projectile.velocity.X = 0f;
								projectile.velocity.Y = -0.2f;
							}
						}
					}
				}
			}
			catch
			{
			}
			if (projectile.owner == Main.myPlayer && projectile.timeLeft <= 3)
			{
				projectile.tileCollide = false;
				projectile.ai[1] = 0f;
				projectile.alpha = 255;
				projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
				projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
				projectile.width = 128;
				projectile.height = 128;
				projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
				projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
				projectile.knockBack = 8f;
			}
			projectile.ai[0] += 1f;
			if (projectile.ai[0] > 10f)
			{
				projectile.ai[0] = 10f;
				if (projectile.velocity.Y == 0f && projectile.velocity.X != 0f)
				{
					projectile.velocity.X = projectile.velocity.X * 0.97f;
					if ((double)projectile.velocity.X > -0.01 && (double)projectile.velocity.X < 0.01)
					{
						projectile.velocity.X = 0f;
						projectile.netUpdate = true;
					}
				}
				projectile.velocity.Y = projectile.velocity.Y + 0.2f;
			}
			projectile.rotation += projectile.velocity.X * 0.1f;
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(3, (int)projectile.position.X, (int)projectile.position.Y, 20);
			projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
			projectile.width = 22;
			projectile.height = 22;
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
			for (int num648 = 0; num648 < 20; num648++)
			{
				int num649 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 5, 0f, 0f, 100, default, 1.5f);
				Main.dust[num649].velocity *= 1.4f;
			}
			for (int num650 = 0; num650 < 10; num650++)
			{
				int num651 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 5, 0f, 0f, 100, default, 2.5f);
				Main.dust[num651].noGravity = true;
				Main.dust[num651].velocity *= 5f;
				num651 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 5, 0f, 0f, 100, default, 1.5f);
				Main.dust[num651].velocity *= 3f;
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			projectile.Kill();
		}
	}
}
