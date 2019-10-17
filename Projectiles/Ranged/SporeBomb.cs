using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles
{
    public class SporeBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bomb");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.extraUpdates = 1;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.ranged = true;
            projectile.light = 0.2f;
        }

        public override void AI()
        {
            projectile.alpha -= 2;
            if (projectile.localAI[0] == 0f)
            {
                projectile.scale += 0.05f;
                if ((double)projectile.scale > 1.2)
                {
                    projectile.localAI[0] = 1f;
                }
            }
            else
            {
                projectile.scale -= 0.05f;
                if ((double)projectile.scale < 0.8)
                {
                    projectile.localAI[0] = 0f;
                }
            }
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 20f && projectile.ai[0] < 40f)
            {
                projectile.velocity.Y = projectile.velocity.Y + 0.3f;
                projectile.velocity.X = projectile.velocity.X * 0.98f;
            }
            else if (projectile.ai[0] >= 40f && projectile.ai[0] < 60f)
            {
                projectile.velocity.Y = projectile.velocity.Y - 0.3f;
                projectile.velocity.X = projectile.velocity.X * 1.02f;
            }
            else if (projectile.ai[0] >= 60f)
            {
                projectile.ai[0] = 0f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, 203, 103, projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.Center.X, (int)projectile.Center.Y, 14);
            for (int num407 = 0; num407 < 25; num407++)
            {
                int num408 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 157, 0f, 0f, 0, new Color(Main.DiscoR, 203, 103), 1f);
                Main.dust[num408].noGravity = true;
                Main.dust[num408].velocity *= 1.5f;
                Main.dust[num408].scale = 1.5f;
            }
            int num251 = Main.rand.Next(3, 7);
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
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value15.X, value15.Y, 569 + Main.rand.Next(3), (int)((double)projectile.damage * 0.5), 0f, projectile.owner, 0f, 0f);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
    }
}
