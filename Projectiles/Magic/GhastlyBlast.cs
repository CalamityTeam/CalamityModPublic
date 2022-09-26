using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 6;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1f;
            bool goingSlow = Projectile.velocity.Length() <= DriftVelocity;
            bool currentlyHoming = Projectile.ai[1] > 0f;

            // Rapidly fade in when the projectile starts existing
            Projectile.alpha -= 15;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            // Moving faster than drift velocity, but not homing.
            // This way, when the projectile speeds back up upon homing, it doesn't suddenly lose its homing again.
            if (!goingSlow && !currentlyHoming)
            {
                // Spin at a certain rate
                Projectile.rotation -= 0.104719758f;

                // Dust, randomly
                if (Main.rand.NextBool(3))
                {
                    if (Main.rand.NextBool(2))
                    {
                        Vector2 vector140 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                        Dust dust28 = Main.dust[Dust.NewDust(Projectile.Center - vector140 * 30f, 0, 0, 60, 0f, 0f, 0, default, 1f)];
                        dust28.noGravity = true;
                        dust28.position = Projectile.Center - vector140 * Main.rand.Next(10, 21);
                        dust28.velocity = vector140.RotatedBy(MathHelper.PiOver2) * 6f;
                        dust28.scale = 0.5f + Main.rand.NextFloat();
                        dust28.fadeIn = 0.5f;
                        dust28.customData = Projectile;
                    }
                    else
                    {
                        Vector2 vector141 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                        Dust dust29 = Main.dust[Dust.NewDust(Projectile.Center - vector141 * 30f, 0, 0, 60, 0f, 0f, 0, default, 1f)];
                        dust29.noGravity = true;
                        dust29.position = Projectile.Center - vector141 * 30f;
                        dust29.velocity = vector141.RotatedBy(-MathHelper.PiOver2) * 3f;
                        dust29.scale = 0.5f + Main.rand.NextFloat();
                        dust29.fadeIn = 0.5f;
                        dust29.customData = Projectile;
                    }
                }

                // Grow and slow down
                if (Projectile.ai[0] >= FramesBeforeSlowing * Projectile.MaxUpdates)
                {
                    Projectile.velocity *= 0.977f;
                    Projectile.scale += 0.00744680827f;
                    if (Projectile.scale > 1.2f)
                    {
                        Projectile.scale = 1.2f;
                    }
                    Projectile.rotation -= 0.0174532924f;
                }

                // Set velocity to the drift velocity exactly after slowing down enough and reset the time counter.
                float speed = Projectile.velocity.Length();
                if (speed < 1.02f * DriftVelocity)
                {
                    Projectile.velocity *= DriftVelocity / speed;
                    Projectile.ai[0] = 0f;
                }
            }

            // Either moving at or below drift velocity, or currently homing. Either case runs this AI path.
            else
            {
                // Spin at the exact same rate anyway
                Projectile.rotation -= 0.104719758f;

                // Excessively complicated dust
                int num3;
                for (int num1014 = 0; num1014 < 1; num1014 = num3 + 1)
                {
                    if (Main.rand.NextBool(2))
                    {
                        Vector2 vector142 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                        Dust dust30 = Main.dust[Dust.NewDust(Projectile.Center - vector142 * 30f, 0, 0, 60, 0f, 0f, 0, default, 1f)];
                        dust30.noGravity = true;
                        dust30.position = Projectile.Center - vector142 * Main.rand.Next(10, 21);
                        dust30.velocity = vector142.RotatedBy(MathHelper.PiOver2) * 6f;
                        dust30.scale = 0.9f + Main.rand.NextFloat();
                        dust30.fadeIn = 0.5f;
                        dust30.customData = Projectile;
                        vector142 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                        dust30.noGravity = true;
                        dust30.position = Projectile.Center - vector142 * Main.rand.Next(10, 21);
                        dust30.velocity = vector142.RotatedBy(MathHelper.PiOver2) * 6f;
                        dust30.scale = 0.9f + Main.rand.NextFloat();
                        dust30.fadeIn = 0.5f;
                        dust30.customData = Projectile;
                        dust30.color = Color.Crimson;
                    }
                    else
                    {
                        Vector2 vector143 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                        Dust dust31 = Main.dust[Dust.NewDust(Projectile.Center - vector143 * 30f, 0, 0, 60, 0f, 0f, 0, default, 1f)];
                        dust31.noGravity = true;
                        dust31.position = Projectile.Center - vector143 * Main.rand.Next(20, 31);
                        dust31.velocity = vector143.RotatedBy(-MathHelper.PiOver2) * 5f;
                        dust31.scale = 0.9f + Main.rand.NextFloat();
                        dust31.fadeIn = 0.5f;
                        dust31.customData = Projectile;
                    }
                    num3 = num1014;
                }

                // Every so many frames, spawn a sub blast.
                if (Projectile.ai[0] % 30f == 0f && Projectile.ai[0] < 241f && Main.myPlayer == Projectile.owner)
                {
                    Vector2 vector144 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * 12f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, vector144.X, vector144.Y, ModContent.ProjectileType<GhastlySubBlast>(), Projectile.damage, 0f, Projectile.owner, 0f, Projectile.whoAmI);
                }

                // Undocumented, unrefactored homing. Will not home through walls.
                // Apparently has two different homing distances.
                Vector2 vector145 = Projectile.Center;
                float num1015 = 500f;
                bool flag59 = false;
                int num1016 = 0;
                if (Projectile.ai[1] == 0f)
                {
                    for (int num1017 = 0; num1017 < Main.maxNPCs; num1017 = num3 + 1)
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
                if (Projectile.localAI[0] < 60f)
                {
                    Projectile.localAI[0] += 1f;
                }
                if (flag59 && Projectile.localAI[0] >= 60f)
                {
                    float HomingVelocity = 24f;
                    int HomingN = 8;
                    Vector2 vector146 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float dx = vector145.X - vector146.X;
                    float dy = vector145.Y - vector146.Y;
                    float dist = (float)Math.Sqrt((double)(dx * dx + dy * dy));
                    dist = HomingVelocity / dist;
                    dx *= dist;
                    dy *= dist;
                    Projectile.velocity.X = (Projectile.velocity.X * (float)(HomingN - 1) + dx) / (float)HomingN;
                    Projectile.velocity.Y = (Projectile.velocity.Y * (float)(HomingN - 1) + dy) / (float)HomingN;
                }
            }

            // If the projectile is solid enough, add light.
            if (Projectile.alpha < 150)
            {
                Lighting.AddLight(Projectile.Center, 0.9f, 0f, 0.1f);
            }

            if (Projectile.ai[0] >= MaximumWaitFrames * Projectile.MaxUpdates)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 238;
            Projectile.Center = Projectile.position;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            for (int num95 = 0; num95 < 4; num95++)
            {
                int num96 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 100, default, 1.5f);
                Main.dust[num96].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
            }
            for (int num97 = 0; num97 < 30; num97++)
            {
                int num98 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 200, default, 3.7f);
                Dust dust = Main.dust[num98];
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 3f;
                num98 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 100, default, 1.5f);
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                dust.velocity *= 2f;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.color = Color.Crimson * 0.5f;
            }
            for (int num99 = 0; num99 < 10; num99++)
            {
                int num100 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 0, default, 2.7f);
                Dust dust = Main.dust[num100];
                dust.position = Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 3f;
            }
            for (int num101 = 0; num101 < 10; num101++)
            {
                int num102 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 0, default, 1.5f);
                Dust dust = Main.dust[num102];
                dust.position = Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 3f;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                for (int num105 = 0; num105 < Main.maxProjectiles; num105++)
                {
                    if (Main.projectile[num105].active && Main.projectile[num105].type == ModContent.ProjectileType<GhastlySubBlast>() && Main.projectile[num105].ai[1] == Projectile.whoAmI)
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
                    Vector2 vector4 = Projectile.Center + Utils.RandomVector2(Main.rand, -30f, 30f);
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
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), vector4.X, vector4.Y, vector5.X, vector5.Y, ModContent.ProjectileType<GhastlyExplosionShard>(), (int)(Projectile.damage * 0.8), Projectile.knockBack * 0.8f, Projectile.owner, num108, 0f);
                }
                for (int num111 = 0; num111 < num107; num111++)
                {
                    Vector2 vector6 = Projectile.Center + Utils.RandomVector2(Main.rand, -30f, 30f);
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
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), vector6.X, vector6.Y, vector7.X, vector7.Y, ModContent.ProjectileType<GhastlyExplosion>(), (int)(Projectile.damage * 0.8), Projectile.knockBack * 0.8f, Projectile.owner, num109, 0f);
                }
            }
        }
    }
}
