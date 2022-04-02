using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class RainbowFront : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rainbow");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.penetrate = -1;
            projectile.ranged = true;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.scale = 1.25f;
        }

        public override void AI()
        {
            int num1 = 1200;
            if (projectile.owner == Main.myPlayer)
            {
                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] > 4f)
                {
                    projectile.localAI[0] = 3f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * (1f / 1000f), projectile.velocity.Y * (1f / 1000f), ModContent.ProjectileType<RainbowTrail>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                }
                if (projectile.timeLeft > num1)
                    projectile.timeLeft = num1;
            }
            float num2 = 1f;
            if (projectile.velocity.Y < 0f)
                num2 -= projectile.velocity.Y / 3f;
            projectile.ai[0] += num2;
            if (projectile.ai[0] > 30f)
            {
                projectile.velocity.Y += 0.5f;
                if (projectile.velocity.Y > 0f)
                {
                    projectile.velocity.X *= 0.95f;
                }
                else
                {
                    projectile.velocity.X *= 1.05f;
                }
            }
            float x = projectile.velocity.X;
            float y = projectile.velocity.Y;
            float num3 = 15.95f * projectile.scale / (float)Math.Sqrt((double)x * (double)x + (double)y * (double)y);
            float num4 = x * num3;
            float num5 = y * num3;
            projectile.velocity.X = num4;
            projectile.velocity.Y = num5;
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) - MathHelper.PiOver2;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Transparent;
    }
}
