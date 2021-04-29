using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class Starblast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 600;
            projectile.magic = true;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 5f)
            {
                projectile.velocity.Y = projectile.velocity.Y + 0.1f;
                projectile.velocity.X = projectile.velocity.X * 1.025f;
                projectile.alpha -= 23;
                projectile.scale = 0.8f * (255f - (float)projectile.alpha) / 255f;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
            }
            if (projectile.alpha >= 255 && projectile.ai[0] > 5f)
            {
                projectile.Kill();
            }
            if (Main.rand.NextBool(4))
            {
                int num193 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 180, 0f, 0f, 100, default, 1f);
                Main.dust[num193].position = projectile.Center;
                Main.dust[num193].scale += (float)Main.rand.Next(50) * 0.01f;
                Main.dust[num193].noGravity = true;
                Dust expr_835F_cp_0 = Main.dust[num193];
                expr_835F_cp_0.velocity.Y -= 2f;
            }
            if (Main.rand.NextBool(6))
            {
                int num194 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 176, 0f, 0f, 100, default, 1f);
                Main.dust[num194].position = projectile.Center;
                Main.dust[num194].scale += 0.3f + (float)Main.rand.Next(50) * 0.01f;
                Main.dust[num194].noGravity = true;
                Main.dust[num194].velocity *= 0.1f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 176, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 180, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
