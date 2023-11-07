using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class GhastlyBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        private const float DriftVelocity = 10f;
        private const float FramesBeforeSlowing = 8f;
        private const float MaximumWaitFrames = 360f;

        public override void SetStaticDefaults()
        {
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
            Projectile.MaxUpdates = 3;
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
                    if (Main.rand.NextBool())
                    {
                        Vector2 vector140 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                        Dust ghostlyRed = Main.dust[Dust.NewDust(Projectile.Center - vector140 * 30f, 0, 0, 60, 0f, 0f, 0, default, 1f)];
                        ghostlyRed.noGravity = true;
                        ghostlyRed.position = Projectile.Center - vector140 * Main.rand.Next(10, 21);
                        ghostlyRed.velocity = vector140.RotatedBy(MathHelper.PiOver2) * 6f;
                        ghostlyRed.scale = 0.5f + Main.rand.NextFloat();
                        ghostlyRed.fadeIn = 0.5f;
                        ghostlyRed.customData = Projectile;
                    }
                    else
                    {
                        Vector2 vector141 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                        Dust ghostlyRedder = Main.dust[Dust.NewDust(Projectile.Center - vector141 * 30f, 0, 0, 60, 0f, 0f, 0, default, 1f)];
                        ghostlyRedder.noGravity = true;
                        ghostlyRedder.position = Projectile.Center - vector141 * 30f;
                        ghostlyRedder.velocity = vector141.RotatedBy(-MathHelper.PiOver2) * 3f;
                        ghostlyRedder.scale = 0.5f + Main.rand.NextFloat();
                        ghostlyRedder.fadeIn = 0.5f;
                        ghostlyRedder.customData = Projectile;
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
                int inc;
                for (int i = 0; i < 1; i = inc + 1)
                {
                    if (Main.rand.NextBool())
                    {
                        Vector2 dustRotate = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                        Dust complexDust = Main.dust[Dust.NewDust(Projectile.Center - dustRotate * 30f, 0, 0, 60, 0f, 0f, 0, default, 1f)];
                        complexDust.noGravity = true;
                        complexDust.position = Projectile.Center - dustRotate * Main.rand.Next(10, 21);
                        complexDust.velocity = dustRotate.RotatedBy(MathHelper.PiOver2) * 6f;
                        complexDust.scale = 0.9f + Main.rand.NextFloat();
                        complexDust.fadeIn = 0.5f;
                        complexDust.customData = Projectile;
                        dustRotate = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                        complexDust.noGravity = true;
                        complexDust.position = Projectile.Center - dustRotate * Main.rand.Next(10, 21);
                        complexDust.velocity = dustRotate.RotatedBy(MathHelper.PiOver2) * 6f;
                        complexDust.scale = 0.9f + Main.rand.NextFloat();
                        complexDust.fadeIn = 0.5f;
                        complexDust.customData = Projectile;
                        complexDust.color = Color.Crimson;
                    }
                    else
                    {
                        Vector2 moreDustRotate = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                        Dust complicatedDust = Main.dust[Dust.NewDust(Projectile.Center - moreDustRotate * 30f, 0, 0, 60, 0f, 0f, 0, default, 1f)];
                        complicatedDust.noGravity = true;
                        complicatedDust.position = Projectile.Center - moreDustRotate * Main.rand.Next(20, 31);
                        complicatedDust.velocity = moreDustRotate.RotatedBy(-MathHelper.PiOver2) * 5f;
                        complicatedDust.scale = 0.9f + Main.rand.NextFloat();
                        complicatedDust.fadeIn = 0.5f;
                        complicatedDust.customData = Projectile;
                    }
                    inc = i;
                }

                // Every so many frames, spawn a sub blast.
                if (Projectile.ai[0] % 30f == 0f && Projectile.ai[0] < 241f && Main.myPlayer == Projectile.owner)
                {
                    Vector2 randomSubBlastOffset = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * 12f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, randomSubBlastOffset.X, randomSubBlastOffset.Y, ModContent.ProjectileType<GhastlySubBlast>(), Projectile.damage, 0f, Projectile.owner, 0f, Projectile.whoAmI);
                }

                // Undocumented, unrefactored homing. Will not home through walls.
                // Apparently has two different homing distances.
		// ^ See, this is why num variables are EVIL.... -CIT
                Vector2 projCenter = Projectile.Center;
                float homingRange = 500f;
                bool isHoming = false;
                int npcTracker = 0;
                if (Projectile.ai[1] == 0f)
                {
                    for (int i = 0; i < Main.maxNPCs; i = inc + 1)
                    {
                        if (Main.npc[i].CanBeChasedBy(Projectile, false))
                        {
                            Vector2 npcCenter = Main.npc[i].Center;
                            if (Projectile.Distance(npcCenter) < homingRange && Collision.CanHit(new Vector2(Projectile.position.X + (float)(Projectile.width / 2), Projectile.position.Y + (float)(Projectile.height / 2)), 1, 1, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
                            {
                                homingRange = Projectile.Distance(npcCenter);
                                projCenter = npcCenter;
                                isHoming = true;
                                npcTracker = i;
                            }
                        }
                        inc = i;
                    }
                    if (isHoming)
                    {
                        if (Projectile.ai[1] != (float)(npcTracker + 1))
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.ai[1] = (float)(npcTracker + 1);
                    }
                    isHoming = false;
                }
                if (Projectile.ai[1] != 0f)
                {
                    int target = (int)(Projectile.ai[1] - 1f);
                    if (Main.npc[target].active && Main.npc[target].CanBeChasedBy(Projectile, true) && Projectile.Distance(Main.npc[target].Center) < 1000f)
                    {
                        isHoming = true;
                        projCenter = Main.npc[target].Center;
                    }
                }
                if (!Projectile.friendly)
                {
                    isHoming = false;
                }
                if (Projectile.localAI[0] < 60f)
                {
                    Projectile.localAI[0] += 1f;
                }
                if (isHoming && Projectile.localAI[0] >= 60f)
                {
                    float HomingVelocity = 24f;
                    int HomingN = 8;
                    Vector2 projDistance = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float dx = projCenter.X - projDistance.X;
                    float dy = projCenter.Y - projDistance.Y;
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

        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 238;
            Projectile.Center = Projectile.position;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            for (int i = 0; i < 4; i++)
            {
                int killRed = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 100, default, 1.5f);
                Main.dust[killRed].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
            }
            for (int j = 0; j < 30; j++)
            {
                int killRed2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 200, default, 3.7f);
                Dust dust = Main.dust[killRed2];
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 3f;
                killRed2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 100, default, 1.5f);
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                dust.velocity *= 2f;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.color = Color.Crimson * 0.5f;
            }
            for (int k = 0; k < 10; k++)
            {
                int killRed3 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 0, default, 2.7f);
                Dust dust = Main.dust[killRed3];
                dust.position = Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 3f;
            }
            for (int l = 0; l < 10; l++)
            {
                int killRed4 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 0, default, 1.5f);
                Dust dust = Main.dust[killRed4];
                dust.position = Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 3f;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                for (int numSubBlasts = 0; numSubBlasts < Main.maxProjectiles; numSubBlasts++)
                {
                    if (Main.projectile[numSubBlasts].active && Main.projectile[numSubBlasts].type == ModContent.ProjectileType<GhastlySubBlast>() && Main.projectile[numSubBlasts].ai[1] == Projectile.whoAmI)
                    {
                        Main.projectile[numSubBlasts].Kill();
                    }
                }
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    60,
                    180
                });
                int subBlastAI = (dustType == 60) ? 180 : 60;
                for (int r = 0; r < 5; r++)
                {
                    Vector2 randShardRotate = Projectile.Center + Utils.RandomVector2(Main.rand, -30f, 30f);
                    Vector2 randShardVel = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    while (randShardVel.X == 0f && randShardVel.Y == 0f)
                    {
                        randShardVel = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    }
                    randShardVel.Normalize();
                    if (randShardVel.Y > 0.2f)
                    {
                        randShardVel.Y *= -1f;
                    }
                    randShardVel *= Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), randShardRotate.X, randShardRotate.Y, randShardVel.X, randShardVel.Y, ModContent.ProjectileType<GhastlyExplosionShard>(), (int)(Projectile.damage * 0.8), Projectile.knockBack * 0.8f, Projectile.owner, dustType, 0f);
                }
                for (int s = 0; s < 5; s++)
                {
                    Vector2 randExplodeRotate = Projectile.Center + Utils.RandomVector2(Main.rand, -30f, 30f);
                    Vector2 randExplodeVel = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    while (randExplodeVel.X == 0f && randExplodeVel.Y == 0f)
                    {
                        randExplodeVel = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    }
                    randExplodeVel.Normalize();
                    if (randExplodeVel.Y > 0.4f)
                    {
                        randExplodeVel.Y *= -1f;
                    }
                    randExplodeVel *= Main.rand.Next(40, 81) * 0.1f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), randExplodeRotate.X, randExplodeRotate.Y, randExplodeVel.X, randExplodeVel.Y, ModContent.ProjectileType<GhastlyExplosion>(), (int)(Projectile.damage * 0.8), Projectile.knockBack * 0.8f, Projectile.owner, subBlastAI, 0f);
                }
            }
        }
    }
}
