using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class CraniumSmasherExplosive : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Explosive Cranium Smasher");
		}

		public override void SetDefaults()
		{
			projectile.width = 50;
			projectile.height = 50;
			projectile.friendly = true;
			projectile.penetrate = 1;
			projectile.timeLeft = 300;
			projectile.localNPCHitCooldown = 15;
			projectile.tileCollide = false;
			projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).rogue = true;
		}

		public override void AI()
		{
			projectile.ai[0] += 1f;
			if (projectile.ai[0] >= 5f)
			{
				projectile.tileCollide = true;
			}
			projectile.rotation += projectile.velocity.X * 0.02f;
			projectile.velocity.Y = projectile.velocity.Y + 0.085f;
			projectile.velocity.X = projectile.velocity.X * 0.99f;
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
			for (int num625 = 0; num625 < 3; num625++)
			{
				float scaleFactor10 = 0.33f;
				if (num625 == 1)
				{
					scaleFactor10 = 0.66f;
				}
				if (num625 == 2)
				{
					scaleFactor10 = 1f;
				}
				int num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
				Main.gore[num626].velocity *= scaleFactor10;
				Gore expr_13AB6_cp_0 = Main.gore[num626];
				expr_13AB6_cp_0.velocity.X = expr_13AB6_cp_0.velocity.X + 1f;
				Gore expr_13AD6_cp_0 = Main.gore[num626];
				expr_13AD6_cp_0.velocity.Y = expr_13AD6_cp_0.velocity.Y + 1f;
				num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
				Main.gore[num626].velocity *= scaleFactor10;
				Gore expr_13B79_cp_0 = Main.gore[num626];
				expr_13B79_cp_0.velocity.X = expr_13B79_cp_0.velocity.X - 1f;
				Gore expr_13B99_cp_0 = Main.gore[num626];
				expr_13B99_cp_0.velocity.Y = expr_13B99_cp_0.velocity.Y + 1f;
				num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
				Main.gore[num626].velocity *= scaleFactor10;
				Gore expr_13C3C_cp_0 = Main.gore[num626];
				expr_13C3C_cp_0.velocity.X = expr_13C3C_cp_0.velocity.X + 1f;
				Gore expr_13C5C_cp_0 = Main.gore[num626];
				expr_13C5C_cp_0.velocity.Y = expr_13C5C_cp_0.velocity.Y - 1f;
			}
			projectile.width = 200;
			projectile.height = 200;
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
			for (int num194 = 0; num194 < 25; num194++)
			{
				int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 135, 0f, 0f, 100, default(Color), 2f);
				Main.dust[num195].noGravity = true;
				Main.dust[num195].velocity *= 0f;
			}
			projectile.Damage();
		}
	}
}
