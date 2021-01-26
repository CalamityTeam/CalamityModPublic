using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class GhastlyBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blast");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 6;
            projectile.extraUpdates = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.magic = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 8;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
            int num1013 = 0;
            if (projectile.velocity.Length() <= 8f) //4
            {
                num1013 = 1;
            }
            projectile.alpha -= 15;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            if (num1013 == 0)
            {
                projectile.rotation -= 0.104719758f;
                if (Main.rand.NextBool(3))
                {
                    if (Main.rand.NextBool(2))
                    {
                        Vector2 vector140 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
                        Dust dust28 = Main.dust[Dust.NewDust(projectile.Center - vector140 * 30f, 0, 0, 60, 0f, 0f, 0, default, 1f)];
                        dust28.noGravity = true;
                        dust28.position = projectile.Center - vector140 * (float)Main.rand.Next(10, 21);
                        dust28.velocity = vector140.RotatedBy(1.5707963705062866, default) * 6f;
                        dust28.scale = 0.5f + Main.rand.NextFloat();
                        dust28.fadeIn = 0.5f;
                        dust28.customData = projectile;
                    }
                    else
                    {
                        Vector2 vector141 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
                        Dust dust29 = Main.dust[Dust.NewDust(projectile.Center - vector141 * 30f, 0, 0, 60, 0f, 0f, 0, default, 1f)];
                        dust29.noGravity = true;
                        dust29.position = projectile.Center - vector141 * 30f;
                        dust29.velocity = vector141.RotatedBy(-1.5707963705062866, default) * 3f;
                        dust29.scale = 0.5f + Main.rand.NextFloat();
                        dust29.fadeIn = 0.5f;
                        dust29.customData = projectile;
                    }
                }
                if (projectile.ai[0] >= 30f)
                {
                    projectile.velocity *= 0.98f;
                    projectile.scale += 0.00744680827f;
                    if (projectile.scale > 1.2f)
                    {
                        projectile.scale = 1.2f;
                    }
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
                projectile.rotation -= 0.104719758f;
                int num3;
                for (int num1014 = 0; num1014 < 1; num1014 = num3 + 1)
                {
                    if (Main.rand.NextBool(2))
                    {
                        Vector2 vector142 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
                        Dust dust30 = Main.dust[Dust.NewDust(projectile.Center - vector142 * 30f, 0, 0, 60, 0f, 0f, 0, default, 1f)];
                        dust30.noGravity = true;
                        dust30.position = projectile.Center - vector142 * (float)Main.rand.Next(10, 21);
                        dust30.velocity = vector142.RotatedBy(1.5707963705062866, default) * 6f;
                        dust30.scale = 0.9f + Main.rand.NextFloat();
                        dust30.fadeIn = 0.5f;
                        dust30.customData = projectile;
                        vector142 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
                        dust30.noGravity = true;
                        dust30.position = projectile.Center - vector142 * (float)Main.rand.Next(10, 21);
                        dust30.velocity = vector142.RotatedBy(1.5707963705062866, default) * 6f;
                        dust30.scale = 0.9f + Main.rand.NextFloat();
                        dust30.fadeIn = 0.5f;
                        dust30.customData = projectile;
                        dust30.color = Color.Crimson;
                    }
                    else
                    {
                        Vector2 vector143 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
                        Dust dust31 = Main.dust[Dust.NewDust(projectile.Center - vector143 * 30f, 0, 0, 60, 0f, 0f, 0, default, 1f)];
                        dust31.noGravity = true;
                        dust31.position = projectile.Center - vector143 * (float)Main.rand.Next(20, 31);
                        dust31.velocity = vector143.RotatedBy(-1.5707963705062866, default) * 5f;
                        dust31.scale = 0.9f + Main.rand.NextFloat();
                        dust31.fadeIn = 0.5f;
                        dust31.customData = projectile;
                    }
                    num3 = num1014;
                }
                if (projectile.ai[0] % 30f == 0f && projectile.ai[0] < 241f && Main.myPlayer == projectile.owner)
                {
                    Vector2 vector144 = Vector2.UnitY.RotatedByRandom(6.2831854820251465) * 12f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, vector144.X, vector144.Y, ModContent.ProjectileType<GhastlySubBlast>(), projectile.damage, 0f, projectile.owner, 0f, (float)projectile.whoAmI);
                }
                Vector2 vector145 = projectile.Center;
                float num1015 = 800f;
                bool flag59 = false;
                int num1016 = 0;
                if (projectile.ai[1] == 0f)
                {
                    for (int num1017 = 0; num1017 < Main.maxNPCs; num1017 = num3 + 1)
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
                if (projectile.localAI[0] < 60f)
                {
                    projectile.localAI[0] += 1f;
                }
                if (flag59 && projectile.localAI[0] >= 60f)
                {
                    float num1019 = 24f; //14
                    int num1020 = 8;
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
            if (projectile.alpha < 150)
            {
                Lighting.AddLight(projectile.Center, 0.9f, 0f, 0.1f);
            }
            if (projectile.ai[0] >= 600f)
            {
                projectile.Kill();
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 238;
            projectile.Center = projectile.position;
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.Damage();
            Main.PlaySound(SoundID.Item14, projectile.position);

            for (int num95 = 0; num95 < 4; num95++)
            {
                int num96 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 100, default, 1.5f);
                Main.dust[num96].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
            }
            for (int num97 = 0; num97 < 30; num97++)
            {
                int num98 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 200, default, 3.7f);
                Dust dust = Main.dust[num98];
				dust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 3f;
                num98 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 100, default, 1.5f);
                dust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                dust.velocity *= 2f;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.color = Color.Crimson * 0.5f;
            }
            for (int num99 = 0; num99 < 10; num99++)
            {
                int num100 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 0, default, 2.7f);
                Dust dust = Main.dust[num100];
                dust.position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)projectile.velocity.ToRotation(), default) * (float)projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 3f;
            }
            for (int num101 = 0; num101 < 10; num101++)
            {
                int num102 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 0, default, 1.5f);
                Dust dust = Main.dust[num102];
                dust.position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)projectile.velocity.ToRotation(), default) * (float)projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 3f;
            }
            if (Main.myPlayer == projectile.owner)
            {
                for (int num105 = 0; num105 < Main.maxProjectiles; num105++)
                {
                    if (Main.projectile[num105].active && Main.projectile[num105].type == ModContent.ProjectileType<GhastlySubBlast>() && Main.projectile[num105].ai[1] == (float)projectile.whoAmI)
                    {
                        Main.projectile[num105].Kill();
                    }
                }
                int num106 = Main.rand.Next(5, 9);
                int num107 = Main.rand.Next(5, 9);
                int num108 = Utils.SelectRandom(Main.rand, new int[]
                {
                    60,
                    180
                });
                int num109 = (num108 == 60) ? 180 : 60;
                for (int num110 = 0; num110 < num106; num110++)
                {
                    Vector2 vector4 = projectile.Center + Utils.RandomVector2(Main.rand, -30f, 30f);
                    Vector2 vector5 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    while (vector5.X == 0f && vector5.Y == 0f)
                    {
                        vector5 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    }
                    vector5.Normalize();
                    if (vector5.Y > 0.2f)
                    {
                        vector5.Y *= -1f;
                    }
                    vector5 *= (float)Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(vector4.X, vector4.Y, vector5.X, vector5.Y, ModContent.ProjectileType<GhastlyExplosionShard>(), (int)((double)projectile.damage * 0.8), projectile.knockBack * 0.8f, projectile.owner, (float)num108, 0f);
                }
                for (int num111 = 0; num111 < num107; num111++)
                {
                    Vector2 vector6 = projectile.Center + Utils.RandomVector2(Main.rand, -30f, 30f);
                    Vector2 vector7 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    while (vector7.X == 0f && vector7.Y == 0f)
                    {
                        vector7 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    }
                    vector7.Normalize();
                    if (vector7.Y > 0.4f)
                    {
                        vector7.Y *= -1f;
                    }
                    vector7 *= (float)Main.rand.Next(40, 81) * 0.1f;
                    Projectile.NewProjectile(vector6.X, vector6.Y, vector7.X, vector7.Y, ModContent.ProjectileType<GhastlyExplosion>(), (int)((double)projectile.damage * 0.8), projectile.knockBack * 0.8f, projectile.owner, (float)num109, 0f);
                }
            }
        }
    }
}
