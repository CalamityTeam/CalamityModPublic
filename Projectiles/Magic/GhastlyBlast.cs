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
        private const float DriftVelocity = 10f;
        private const float FramesBeforeSlowing = 8f;
        private const float MaximumWaitFrames = 360f;
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ghast Blast");
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
            projectile.extraUpdates = 2;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.magic = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 12;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
            bool goingSlow = projectile.velocity.Length() <= DriftVelocity;
            bool currentlyHoming = projectile.ai[1] > 0f;

            // Rapidly fade in when the projectile starts existing
            projectile.alpha -= 15;
            if (projectile.alpha < 0)
                projectile.alpha = 0;

            // Moving faster than drift velocity, but not homing.
            // This way, when the projectile speeds back up upon homing, it doesn't suddenly lose its homing again.
            if (!goingSlow && !currentlyHoming)
            {
                // Spin at a certain rate
                projectile.rotation -= 0.104719758f;

                // Dust, randomly
                if (Main.rand.NextBool(3))
                {
                    if (Main.rand.NextBool(2))
                    {
                        Vector2 vector140 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                        Dust dust28 = Main.dust[Dust.NewDust(projectile.Center - vector140 * 30f, 0, 0, 60, 0f, 0f, 0, default, 1f)];
                        dust28.noGravity = true;
                        dust28.position = projectile.Center - vector140 * Main.rand.Next(10, 21);
                        dust28.velocity = vector140.RotatedBy(MathHelper.PiOver2) * 6f;
                        dust28.scale = 0.5f + Main.rand.NextFloat();
                        dust28.fadeIn = 0.5f;
                        dust28.customData = projectile;
                    }
                    else
                    {
                        Vector2 vector141 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                        Dust dust29 = Main.dust[Dust.NewDust(projectile.Center - vector141 * 30f, 0, 0, 60, 0f, 0f, 0, default, 1f)];
                        dust29.noGravity = true;
                        dust29.position = projectile.Center - vector141 * 30f;
                        dust29.velocity = vector141.RotatedBy(-MathHelper.PiOver2) * 3f;
                        dust29.scale = 0.5f + Main.rand.NextFloat();
                        dust29.fadeIn = 0.5f;
                        dust29.customData = projectile;
                    }
                }

                // Grow and slow down 
                if (projectile.ai[0] >= FramesBeforeSlowing * projectile.MaxUpdates)
                {
                    projectile.velocity *= 0.977f;
                    projectile.scale += 0.00744680827f;
                    if (projectile.scale > 1.2f)
                    {
                        projectile.scale = 1.2f;
                    }
                    projectile.rotation -= 0.0174532924f;
                }

                // Set velocity to the drift velocity exactly after slowing down enough and reset the time counter.
                float speed = projectile.velocity.Length();
                if (speed < 1.02f * DriftVelocity)
                {
                    projectile.velocity *= DriftVelocity / speed;
                    projectile.ai[0] = 0f;
                }
            }

            // Either moving at or below drift velocity, or currently homing. Either case runs this AI path.
            else
            {
                // Spin at the exact same rate anyway
                projectile.rotation -= 0.104719758f;

                // Excessively complicated dust
                int num3;
                for (int num1014 = 0; num1014 < 1; num1014 = num3 + 1)
                {
                    if (Main.rand.NextBool(2))
                    {
                        Vector2 vector142 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                        Dust dust30 = Main.dust[Dust.NewDust(projectile.Center - vector142 * 30f, 0, 0, 60, 0f, 0f, 0, default, 1f)];
                        dust30.noGravity = true;
                        dust30.position = projectile.Center - vector142 * Main.rand.Next(10, 21);
                        dust30.velocity = vector142.RotatedBy(MathHelper.PiOver2) * 6f;
                        dust30.scale = 0.9f + Main.rand.NextFloat();
                        dust30.fadeIn = 0.5f;
                        dust30.customData = projectile;
                        vector142 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                        dust30.noGravity = true;
                        dust30.position = projectile.Center - vector142 * Main.rand.Next(10, 21);
                        dust30.velocity = vector142.RotatedBy(MathHelper.PiOver2) * 6f;
                        dust30.scale = 0.9f + Main.rand.NextFloat();
                        dust30.fadeIn = 0.5f;
                        dust30.customData = projectile;
                        dust30.color = Color.Crimson;
                    }
                    else
                    {
                        Vector2 vector143 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                        Dust dust31 = Main.dust[Dust.NewDust(projectile.Center - vector143 * 30f, 0, 0, 60, 0f, 0f, 0, default, 1f)];
                        dust31.noGravity = true;
                        dust31.position = projectile.Center - vector143 * Main.rand.Next(20, 31);
                        dust31.velocity = vector143.RotatedBy(-MathHelper.PiOver2) * 5f;
                        dust31.scale = 0.9f + Main.rand.NextFloat();
                        dust31.fadeIn = 0.5f;
                        dust31.customData = projectile;
                    }
                    num3 = num1014;
                }

                // Every so many frames, spawn a sub blast.
                if (projectile.ai[0] % 30f == 0f && projectile.ai[0] < 241f && Main.myPlayer == projectile.owner)
                {
                    Vector2 vector144 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * 12f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, vector144.X, vector144.Y, ModContent.ProjectileType<GhastlySubBlast>(), projectile.damage, 0f, projectile.owner, 0f, projectile.whoAmI);
                }

                // Undocumented, unrefactored homing. Will not home through walls.
                // Apparently has two different homing distances.
                Vector2 vector145 = projectile.Center;
                float num1015 = 500f;
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
                    float HomingVelocity = 24f;
                    int HomingN = 8;
                    Vector2 vector146 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float dx = vector145.X - vector146.X;
                    float dy = vector145.Y - vector146.Y;
                    float dist = (float)Math.Sqrt((double)(dx * dx + dy * dy));
                    dist = HomingVelocity / dist;
                    dx *= dist;
                    dy *= dist;
                    projectile.velocity.X = (projectile.velocity.X * (float)(HomingN - 1) + dx) / (float)HomingN;
                    projectile.velocity.Y = (projectile.velocity.Y * (float)(HomingN - 1) + dy) / (float)HomingN;
                }
            }

            // If the projectile is solid enough, add light.
            if (projectile.alpha < 150)
            {
                Lighting.AddLight(projectile.Center, 0.9f, 0f, 0.1f);
            }

            if (projectile.ai[0] >= MaximumWaitFrames * projectile.MaxUpdates)
            {
                projectile.Kill();
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
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
                Main.dust[num96].position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * projectile.width / 2f;
            }
            for (int num97 = 0; num97 < 30; num97++)
            {
                int num98 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 200, default, 3.7f);
                Dust dust = Main.dust[num98];
                dust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 3f;
                num98 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 100, default, 1.5f);
                dust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * projectile.width / 2f;
                dust.velocity *= 2f;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.color = Color.Crimson * 0.5f;
            }
            for (int num99 = 0; num99 < 10; num99++)
            {
                int num100 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 0, default, 2.7f);
                Dust dust = Main.dust[num100];
                dust.position = projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(projectile.velocity.ToRotation()) * projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 3f;
            }
            for (int num101 = 0; num101 < 10; num101++)
            {
                int num102 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 0, default, 1.5f);
                Dust dust = Main.dust[num102];
                dust.position = projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(projectile.velocity.ToRotation()) * projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 3f;
            }
            if (Main.myPlayer == projectile.owner)
            {
                for (int num105 = 0; num105 < Main.maxProjectiles; num105++)
                {
                    if (Main.projectile[num105].active && Main.projectile[num105].type == ModContent.ProjectileType<GhastlySubBlast>() && Main.projectile[num105].ai[1] == projectile.whoAmI)
                    {
                        Main.projectile[num105].Kill();
                    }
                }
                int num106 = 5;
                int num107 = 5;
                int num108 = Utils.SelectRandom(Main.rand, new int[]
                {
                    60,
                    180
                });
                int num109 = (num108 == 60) ? 180 : 60;
                for (int num110 = 0; num110 < num106; num110++)
                {
                    Vector2 vector4 = projectile.Center + Utils.RandomVector2(Main.rand, -30f, 30f);
                    Vector2 vector5 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    while (vector5.X == 0f && vector5.Y == 0f)
                    {
                        vector5 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    }
                    vector5.Normalize();
                    if (vector5.Y > 0.2f)
                    {
                        vector5.Y *= -1f;
                    }
                    vector5 *= Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(vector4.X, vector4.Y, vector5.X, vector5.Y, ModContent.ProjectileType<GhastlyExplosionShard>(), (int)(projectile.damage * 0.8), projectile.knockBack * 0.8f, projectile.owner, num108, 0f);
                }
                for (int num111 = 0; num111 < num107; num111++)
                {
                    Vector2 vector6 = projectile.Center + Utils.RandomVector2(Main.rand, -30f, 30f);
                    Vector2 vector7 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    while (vector7.X == 0f && vector7.Y == 0f)
                    {
                        vector7 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    }
                    vector7.Normalize();
                    if (vector7.Y > 0.4f)
                    {
                        vector7.Y *= -1f;
                    }
                    vector7 *= Main.rand.Next(40, 81) * 0.1f;
                    Projectile.NewProjectile(vector6.X, vector6.Y, vector7.X, vector7.Y, ModContent.ProjectileType<GhastlyExplosion>(), (int)(projectile.damage * 0.8), projectile.knockBack * 0.8f, projectile.owner, num109, 0f);
                }
            }
        }
    }
}
