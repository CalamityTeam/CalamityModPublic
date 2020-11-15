using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneLaserSplit : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/BrimstoneLaser";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Homing Laser");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.hostile = true;
            projectile.scale = 2f;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 480;
            projectile.alpha = 120;
        }

        public override void AI()
        {
            int num103 = (int)Player.FindClosest(projectile.Center, 1, 1);
            projectile.ai[1] += 1f;
            if (projectile.ai[1] < 150f && projectile.ai[1] > 90f)
            {
                float scaleFactor2 = projectile.velocity.Length();
                Vector2 vector11 = Main.player[num103].Center - projectile.Center;
                vector11.Normalize();
                vector11 *= scaleFactor2;
                projectile.velocity = (projectile.velocity * 24f + vector11) / 25f;
                projectile.velocity.Normalize();
                projectile.velocity *= scaleFactor2;
            }
            else if (projectile.ai[1] >= 150f)
            {
                if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < 16f)
                {
                    projectile.velocity.X *= 1.01f;
                    projectile.velocity.Y *= 1.01f;
                }
            }
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
            Lighting.AddLight(projectile.Center, 0.3f, 0f, 0f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 50, 50, projectile.alpha);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
        }
    }
}
