using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class Nanotech : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Nanotech");
		}

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 2;
        }

        public override void AI()
        {
			Lighting.AddLight(projectile.Center, new Vector3(0.075f, 0.4f, 0.15f));
			projectile.rotation += projectile.velocity.X * 0.2f;
			if (projectile.velocity.X > 0f)
			{
				projectile.rotation += 0.08f;
			}
			else
			{
				projectile.rotation -= 0.08f;
			}
			projectile.ai[1] += 1f;
			if (projectile.ai[1] > 30f)
			{
				projectile.alpha += 5;
				if (projectile.alpha >= 255)
				{
					projectile.alpha = 255;
					projectile.Kill();
					return;
				}
			}
            float num472 = projectile.Center.X;
            float num473 = projectile.Center.Y;
            float num474 = 600f;
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
                float num483 = 20f;
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
			int num3;
			for (int num191 = 0; num191 < 2; num191 = num3 + 1)
			{
				int num192 = (int)(10f * projectile.scale);
				int num193 = Dust.NewDust(projectile.Center - Vector2.One * (float)num192, num192 * 2, num192 * 2, 107, 0f, 0f, 0, default, 1f);
				Dust dust20 = Main.dust[num193];
				Vector2 value8 = Vector2.Normalize(dust20.position - projectile.Center);
				dust20.position = projectile.Center + value8 * (float)num192 * projectile.scale;
				if (num191 < 30)
				{
					dust20.velocity = value8 * dust20.velocity.Length();
				}
				else
				{
					dust20.velocity = value8 * (float)Main.rand.Next(45, 91) / 10f;
				}
				dust20.color = Main.hslToRgb((float)(0.40000000596046448 + Main.rand.NextDouble() * 0.20000000298023224), 0.9f, 0.5f);
				dust20.color = Color.Lerp(dust20.color, Color.White, 0.3f);
				dust20.noGravity = true;
				dust20.scale = 0.7f;
				num3 = num191;
			}
        }
    }
}
