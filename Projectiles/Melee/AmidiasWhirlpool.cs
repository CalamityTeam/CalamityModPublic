using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class AmidiasWhirlpool : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Whirlpool");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 58;
            projectile.height = 58;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.alpha = 100;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.melee = true;
        }

        public override void AI()
        {
            for (int num105 = 0; num105 < 2; num105++)
            {
                float num99 = projectile.velocity.X / 3f * (float)num105;
                float num100 = projectile.velocity.Y / 3f * (float)num105;
                int num101 = 4;
                int num102 = Dust.NewDust(new Vector2(projectile.position.X + (float)num101, projectile.position.Y + (float)num101), projectile.width - num101 * 2, projectile.height - num101 * 2, 33, 0f, 0f, 0, new Color(0, 142, 255), 1.5f);
                Dust dust = Main.dust[num102];
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.velocity += projectile.velocity * 0.1f;
                dust.position.X -= num99;
                dust.position.Y -= num100;
            }
            projectile.ai[0] += 1f;
            int num1013 = 0;
            if (projectile.velocity.Length() <= 8f) //4
            {
                num1013 = 1;
            }
            if (num1013 == 0)
            {
                projectile.rotation -= 0.104719758f;

                if (projectile.ai[0] >= 30f)
                {
                    projectile.velocity *= 0.98f;
                    projectile.rotation -= 0.0174532924f;
                }
                if (projectile.velocity.Length() < 8.2f) //4.1
                {
                    projectile.velocity.Normalize();
                    projectile.velocity *= 4f;
                    projectile.ai[0] = 0f;
                }
            }
            else if (num1013 == 1)
            {
                int num3;
                projectile.rotation -= 0.104719758f;
                Vector2 vector145 = projectile.Center;
                float num1015 = 150f;
                bool flag59 = false;
                int num1016 = 0;
                if (projectile.ai[1] == 0f)
                {
                    for (int num1017 = 0; num1017 < 200; num1017 = num3 + 1)
                    {
                        if (Main.npc[num1017].CanBeChasedBy(projectile, false))
                        {
                            Vector2 center13 = Main.npc[num1017].Center;
                            if (projectile.Distance(center13) < num1015 && Collision.CanHit(new Vector2(projectile.position.X + (float)(projectile.width / 2), projectile.position.Y + (float)(projectile.height / 2)), 1, 1, Main.npc[num1017].position, Main.npc[num1017].width, Main.npc[num1017].height))
                            {
                                num1015 = projectile.Distance(center13);
                                vector145 = center13;
                                flag59 = true;
                                num1016 = num1017;
                                break;
                            }
                        }
                        num3 = num1017;
                    }
                    if (flag59)
                    {
                        if (projectile.ai[1] != (float)(num1016 + 1))
                        {
                            projectile.netUpdate = true;
                        }
                        projectile.ai[1] = (float)(num1016 + 1);
                    }
                    flag59 = false;
                }
                if (projectile.ai[1] != 0f)
                {
                    int num1018 = (int)(projectile.ai[1] - 1f);
                    if (Main.npc[num1018].active && Main.npc[num1018].CanBeChasedBy(projectile, true) && projectile.Distance(Main.npc[num1018].Center) < 1000f)
                    {
                        flag59 = true;
                        vector145 = Main.npc[num1018].Center;
                    }
                }
                if (!projectile.friendly)
                {
                    flag59 = false;
                }
                if (flag59)
                {
                    float num1019 = 24f;
                    int num1020 = 10;
                    Vector2 vector146 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float num1021 = vector145.X - vector146.X;
                    float num1022 = vector145.Y - vector146.Y;
                    float num1023 = (float)Math.Sqrt((double)(num1021 * num1021 + num1022 * num1022));
                    num1023 = num1019 / num1023;
                    num1021 *= num1023;
                    num1022 *= num1023;
                    projectile.velocity.X = (projectile.velocity.X * (float)(num1020 - 1) + num1021) / (float)num1020;
                    projectile.velocity.Y = (projectile.velocity.Y * (float)(num1020 - 1) + num1022) / (float)num1020;
                }
            }
            Lighting.AddLight(projectile.Center, 0f, 0.1f, 0.9f);
            if (projectile.ai[0] >= 120f)
            {
                projectile.Kill();
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(30, 255, 253);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 10);
            for (int k = 0; k < 20; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 33, projectile.oldVelocity.X, projectile.oldVelocity.Y, 0, new Color(0, 142, 255), 1f);
            }
        }
    }
}
