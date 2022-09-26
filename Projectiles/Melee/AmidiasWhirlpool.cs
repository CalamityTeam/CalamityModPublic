using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class AmidiasWhirlpool : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Whirlpool");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 58;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 100;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            for (int num105 = 0; num105 < 2; num105++)
            {
                float num99 = Projectile.velocity.X / 3f * (float)num105;
                float num100 = Projectile.velocity.Y / 3f * (float)num105;
                int num101 = 4;
                int num102 = Dust.NewDust(new Vector2(Projectile.position.X + (float)num101, Projectile.position.Y + (float)num101), Projectile.width - num101 * 2, Projectile.height - num101 * 2, 33, 0f, 0f, 0, new Color(0, 142, 255), 1.5f);
                Dust dust = Main.dust[num102];
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.velocity += Projectile.velocity * 0.1f;
                dust.position.X -= num99;
                dust.position.Y -= num100;
            }
            Projectile.ai[0] += 1f;
            int num1013 = 0;
            if (Projectile.velocity.Length() <= 8f) //4
            {
                num1013 = 1;
            }
            if (num1013 == 0)
            {
                Projectile.rotation -= 0.104719758f;

                if (Projectile.ai[0] >= 30f)
                {
                    Projectile.velocity *= 0.98f;
                    Projectile.rotation -= 0.0174532924f;
                }
                if (Projectile.velocity.Length() < 8.2f) //4.1
                {
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= 4f;
                    Projectile.ai[0] = 0f;
                }
            }
            else if (num1013 == 1)
            {
                int num3;
                Projectile.rotation -= 0.104719758f;
                Vector2 vector145 = Projectile.Center;
                float num1015 = 150f;
                bool flag59 = false;
                int num1016 = 0;
                if (Projectile.ai[1] == 0f)
                {
                    for (int num1017 = 0; num1017 < 200; num1017 = num3 + 1)
                    {
                        if (Main.npc[num1017].CanBeChasedBy(Projectile, false))
                        {
                            Vector2 center13 = Main.npc[num1017].Center;
                            if (Projectile.Distance(center13) < num1015 && Collision.CanHit(new Vector2(Projectile.position.X + (float)(Projectile.width / 2), Projectile.position.Y + (float)(Projectile.height / 2)), 1, 1, Main.npc[num1017].position, Main.npc[num1017].width, Main.npc[num1017].height))
                            {
                                num1015 = Projectile.Distance(center13);
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
                        if (Projectile.ai[1] != (float)(num1016 + 1))
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.ai[1] = (float)(num1016 + 1);
                    }
                    flag59 = false;
                }
                if (Projectile.ai[1] != 0f)
                {
                    int num1018 = (int)(Projectile.ai[1] - 1f);
                    if (Main.npc[num1018].active && Main.npc[num1018].CanBeChasedBy(Projectile, true) && Projectile.Distance(Main.npc[num1018].Center) < 1000f)
                    {
                        flag59 = true;
                        vector145 = Main.npc[num1018].Center;
                    }
                }
                if (!Projectile.friendly)
                {
                    flag59 = false;
                }
                if (flag59)
                {
                    float num1019 = 24f;
                    int num1020 = 10;
                    Vector2 vector146 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float num1021 = vector145.X - vector146.X;
                    float num1022 = vector145.Y - vector146.Y;
                    float num1023 = (float)Math.Sqrt((double)(num1021 * num1021 + num1022 * num1022));
                    num1023 = num1019 / num1023;
                    num1021 *= num1023;
                    num1022 *= num1023;
                    Projectile.velocity.X = (Projectile.velocity.X * (float)(num1020 - 1) + num1021) / (float)num1020;
                    Projectile.velocity.Y = (Projectile.velocity.Y * (float)(num1020 - 1) + num1022) / (float)num1020;
                }
            }
            Lighting.AddLight(Projectile.Center, 0f, 0.1f, 0.9f);
            if (Projectile.ai[0] >= 120f)
            {
                Projectile.Kill();
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(30, 255, 253);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int k = 0; k < 20; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 33, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 0, new Color(0, 142, 255), 1f);
            }
        }
    }
}
