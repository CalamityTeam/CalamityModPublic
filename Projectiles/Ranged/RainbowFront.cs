using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class RainbowFront : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
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
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.localAI[0] += 1f;
                if (Projectile.localAI[0] > 4f)
                {
                    Projectile.localAI[0] = 3f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X * (1f / 1000f), Projectile.velocity.Y * (1f / 1000f), ModContent.ProjectileType<RainbowTrail>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                }
                if (Projectile.timeLeft > 1200)
                    Projectile.timeLeft = 1200;
            }
            float gravityControl = 1f;
            if (Projectile.velocity.Y < 0f)
                gravityControl -= Projectile.velocity.Y / 3f;
            Projectile.ai[0] += gravityControl;
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
            float velocityMult = 15.95f * Projectile.scale / (float)Math.Sqrt((double)x * (double)x + (double)y * (double)y);
            float xVel = x * velocityMult;
            float yVel = y * velocityMult;
            Projectile.velocity.X = xVel;
            Projectile.velocity.Y = yVel;
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) - MathHelper.PiOver2;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Transparent;
    }
}
