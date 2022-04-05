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
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.scale = 1.25f;
        }

        public override void AI()
        {
            int num1 = 1200;
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.localAI[0] += 1f;
                if (Projectile.localAI[0] > 4f)
                {
                    Projectile.localAI[0] = 3f;
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X * (1f / 1000f), Projectile.velocity.Y * (1f / 1000f), ModContent.ProjectileType<RainbowTrail>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                }
                if (Projectile.timeLeft > num1)
                    Projectile.timeLeft = num1;
            }
            float num2 = 1f;
            if (Projectile.velocity.Y < 0f)
                num2 -= Projectile.velocity.Y / 3f;
            Projectile.ai[0] += num2;
            if (Projectile.ai[0] > 30f)
            {
                Projectile.velocity.Y += 0.5f;
                if (Projectile.velocity.Y > 0f)
                {
                    Projectile.velocity.X *= 0.95f;
                }
                else
                {
                    Projectile.velocity.X *= 1.05f;
                }
            }
            float x = Projectile.velocity.X;
            float y = Projectile.velocity.Y;
            float num3 = 15.95f * Projectile.scale / (float)Math.Sqrt((double)x * (double)x + (double)y * (double)y);
            float num4 = x * num3;
            float num5 = y * num3;
            Projectile.velocity.X = num4;
            Projectile.velocity.Y = num5;
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) - MathHelper.PiOver2;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Transparent;
    }
}
