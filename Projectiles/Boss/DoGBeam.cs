using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class DoGBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Portal Laser");
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.hostile = true;
            Projectile.scale = 2f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 960;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 1)
                Projectile.frame = 0;

            Lighting.AddLight(Projectile.Center, 0f, 0.2f, 0.3f);

            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + MathHelper.PiOver2;

            int num103 = Player.FindClosest(Projectile.Center, 1, 1);
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] < 90f && Projectile.ai[1] > 30f)
            {
                float scaleFactor2 = Projectile.velocity.Length();
                Vector2 vector11 = Main.player[num103].Center - Projectile.Center;
                vector11.Normalize();
                vector11 *= scaleFactor2;
                Projectile.velocity = (Projectile.velocity * 20f + vector11) / 21f;
                Projectile.velocity.Normalize();
                Projectile.velocity *= scaleFactor2;
            }

            if (Projectile.timeLeft == 950)
                Projectile.damage = (int)Projectile.ai[0];
            if (Projectile.timeLeft < 30)
                Projectile.damage = 0;
        }

        public override bool CanHitPlayer(Player target)
        {
            if (Projectile.timeLeft > 950 || Projectile.timeLeft < 30)
            {
                return false;
            }
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft > 950)
            {
                return new Color(0, 0, 0, 0);
            }
            if (Projectile.timeLeft < 30)
            {
                byte b2 = (byte)(Projectile.timeLeft * 8.5);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, 100);
        }
    }
}
