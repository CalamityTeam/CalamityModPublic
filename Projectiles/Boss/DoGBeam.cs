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
            Main.projFrames[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.hostile = true;
            projectile.scale = 2f;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 960;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 1)
                projectile.frame = 0;

            Lighting.AddLight(projectile.Center, 0f, 0.2f, 0.3f);

            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + MathHelper.PiOver2;

            int num103 = Player.FindClosest(projectile.Center, 1, 1);
            projectile.ai[1] += 1f;
            if (projectile.ai[1] < 90f && projectile.ai[1] > 30f)
            {
                float scaleFactor2 = projectile.velocity.Length();
                Vector2 vector11 = Main.player[num103].Center - projectile.Center;
                vector11.Normalize();
                vector11 *= scaleFactor2;
                projectile.velocity = (projectile.velocity * 20f + vector11) / 21f;
                projectile.velocity.Normalize();
                projectile.velocity *= scaleFactor2;
            }

            if (projectile.timeLeft == 950)
                projectile.damage = (int)projectile.ai[0];
            if (projectile.timeLeft < 30)
                projectile.damage = 0;
        }

        public override bool CanHitPlayer(Player target)
        {
            if (projectile.timeLeft > 950 || projectile.timeLeft < 30)
            {
                return false;
            }
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft > 950)
            {
                return new Color(0, 0, 0, 0);
            }
            if (projectile.timeLeft < 30)
            {
                byte b2 = (byte)(projectile.timeLeft * 8.5);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, 100);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
