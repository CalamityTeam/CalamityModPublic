using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class ScavengerNuke : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Homing Nuke");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 44;
            projectile.height = 44;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.alpha = 255;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;

            if (projectile.timeLeft < 180)
                projectile.tileCollide = true;

            projectile.frameCounter++;
            if (projectile.frameCounter > 18)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 4)
                projectile.frame = 0;

            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

            float inertia = malice ? 70f : revenge ? 90f : 110f;
            float scaleFactor12 = malice ? 20f : revenge ? 16f : 12f;

            if (projectile.alpha > 0)
                projectile.alpha -= 10;
            if (projectile.alpha < 0)
                projectile.alpha = 0;

            Lighting.AddLight(projectile.Center, 1f, 0.7f, 0f);

            int num959 = (int)projectile.ai[0];
            if (num959 >= 0 && Main.player[num959].active && !Main.player[num959].dead)
            {
                if (projectile.Distance(Main.player[num959].Center) > 320f)
                {
                    Vector2 moveDirection = projectile.SafeDirectionTo(Main.player[num959].Center, Vector2.UnitY);
                    projectile.velocity = (projectile.velocity * (inertia - 1f) + moveDirection * scaleFactor12) / inertia;
                }
            }
            else
            {
                if (projectile.timeLeft > 30)
                    projectile.timeLeft = 30;

                if (projectile.ai[0] != -1f)
                {
                    projectile.ai[0] = -1f;
                    projectile.netUpdate = true;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.position);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 160;
            projectile.position.X = projectile.position.X - (projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (projectile.height / 2);
            projectile.Damage();
            for (int num621 = 0; num621 < 30; num621++)
            {
                int num622 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 40; num623++)
            {
                int num624 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
            CalamityUtils.ExplosionGores(projectile.Center, 3);
        }
    }
}
