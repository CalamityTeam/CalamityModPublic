using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Enemy
{
    public class PearlBurst : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Enemy";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 64;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0) // saving pcs on the zenith seed
            {
                Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0.5f / 255f);
                if (Main.rand.NextBool(5))
                {
                    Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 92, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 0, new Color(255, 255, 255), 0.7f);
                }
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) - 1.57f;
            Projectile.alpha -= 6;
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] <= 20f)
            {
                Projectile.velocity.X *= 0.975f;
                Projectile.velocity.Y *= 0.975f;
            }
            else if (Projectile.ai[1] > 20f && Projectile.ai[1] <= 39f)
            {
                Projectile.velocity.X *= 1.05f;
                Projectile.velocity.Y *= 1.05f;
            }
            else if (Projectile.ai[1] == 40f)
            {
                Projectile.ai[1] = 0f;
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] == 30f)
            {
                Projectile.localAI[0] = 0f;
                for (int l = 0; l < 12; l++)
                {
                    Vector2 dustRotation = Vector2.UnitX * (float)-(float)Projectile.width / 2f;
                    dustRotation += -Vector2.UnitY.RotatedBy((double)((float)l * 3.14159274f / 6f), default) * new Vector2(8f, 16f);
                    dustRotation = dustRotation.RotatedBy((double)(Projectile.rotation - 1.57079637f), default);
                    int pearlDust = Dust.NewDust(Projectile.Center, 0, 0, 92, 0f, 0f, 160, default, 1f);
                    Main.dust[pearlDust].scale = 1.1f;
                    Main.dust[pearlDust].noGravity = true;
                    Main.dust[pearlDust].position = Projectile.Center + dustRotation;
                    Main.dust[pearlDust].velocity = Projectile.velocity * 0.1f;
                    Main.dust[pearlDust].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[pearlDust].position) * 1.25f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255);
        }
    }
}
