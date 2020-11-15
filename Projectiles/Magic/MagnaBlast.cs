using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class MagnaBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blast");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if (projectile.scale <= 1.6f)
            {
                projectile.scale *= 1.01f;
                projectile.width = (int)(16f * projectile.scale);
                projectile.height = (int)(32f * projectile.scale);
            }
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.1f / 255f, (255 - projectile.alpha) * 0.1f / 255f, (255 - projectile.alpha) * 1f / 255f);
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                for (int num468 = 0; num468 < 3; num468++)
                {
                    int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 59, 0f, 0f, 100, default, projectile.scale);
                    Main.dust[num469].noGravity = true;
                    Main.dust[num469].velocity *= 0f;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 59, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }
    }
}
