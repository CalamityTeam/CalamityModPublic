using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace CalamityMod.Projectiles.Rogue
{
    public class TotalityFlask : ModProjectile
    {
        private int tarTimer = 10;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Totality Flask");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.aiStyle = 68;
            projectile.timeLeft = 180;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.Calamity().stealthStrike == true)
            {
				tarTimer--;
				if (tarTimer == 0)
				{
					if (projectile.owner == Main.myPlayer)
					{
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0f, projectile.velocity.Y * 0f, ModContent.ProjectileType<TotalityTar>(), (int)((double)projectile.damage * 0.6), projectile.knockBack, projectile.owner, 0f, 0f);
					}
					tarTimer = 10;
				}
            }
            Vector2 spinningpoint = new Vector2(4f, -8f);
            float rotation = projectile.rotation;
            if (projectile.direction == -1)
              spinningpoint.X = -4f;
            Vector2 vector2 = spinningpoint.RotatedBy((double) rotation, new Vector2());
            for (int index1 = 0; index1 < 1; ++index1)
            {
              int index2 = Dust.NewDust(projectile.Center + vector2 - Vector2.One * 5f, 4, 4, 6, 0.0f, 0.0f, 0, new Color(), 1f);
              Main.dust[index2].scale = 1.5f;
              Main.dust[index2].noGravity = true;
              Main.dust[index2].velocity = Main.dust[index2].velocity * 0.25f + Vector2.Normalize(vector2) * 1f;
              Main.dust[index2].velocity = Main.dust[index2].velocity.RotatedBy(-1.57079637050629 * (double) projectile.direction, new Vector2());
            }
		}

        public override void Kill(int timeLeft)
        {
			Main.PlaySound(13, (int) projectile.position.X, (int) projectile.position.Y, 1, 1f, 0.0f);
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
			Vector2 vector2 = new Vector2(20f, 20f);
			for (int index = 0; index < 5; ++index)
				Dust.NewDust(projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 191, 0.0f, 0.0f, 0, Color.Red, 1f);
			for (int index1 = 0; index1 < 10; ++index1)
			{
				int index2 = Dust.NewDust(projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 31, 0.0f, 0.0f, 100, new Color(), 1.5f);
				Dust dust = Main.dust[index2];
				dust.velocity = dust.velocity * 1.4f;
			}
			for (int index1 = 0; index1 < 20; ++index1)
			{
				int index2 = Dust.NewDust(projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 6, 0.0f, 0.0f, 100, new Color(), 2.5f);
				Main.dust[index2].noGravity = true;
				Dust dust1 = Main.dust[index2];
				dust1.velocity = dust1.velocity * 5f;
				int index3 = Dust.NewDust(projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 6, 0.0f, 0.0f, 100, new Color(), 1.5f);
				Dust dust2 = Main.dust[index3];
				dust2.velocity = dust2.velocity * 3f;
			}
            int num251 = Main.rand.Next(2, 4);
            if (projectile.owner == Main.myPlayer)
            {
                for (int num252 = 0; num252 < num251; num252++)
                {
                    Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    while (value15.X == 0f && value15.Y == 0f)
                    {
                        value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    }
                    value15.Normalize();
                    value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value15.X, value15.Y, ModContent.ProjectileType<TotalityTar>(), (int)((double)projectile.damage * 0.6), 0f, Main.myPlayer, 0f, 0f);
                }
			}
        }
    }
}
