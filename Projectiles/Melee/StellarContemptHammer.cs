using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Particles;

namespace CalamityMod.Projectiles.Melee
{
    public class StellarContemptHammer : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";

        public override string Texture => "CalamityMod/Items/Weapons/Melee/StellarContempt";
        public static readonly SoundStyle UseSound = new("CalamityMod/Sounds/Item/PwnagehammerSound") { Volume = 0.35f};
        public static readonly SoundStyle RedHamSound = new("CalamityMod/Sounds/Item/StellarContemptClone") { Volume = 0.6f };
        public static readonly SoundStyle UseSoundFunny = new("CalamityMod/Sounds/Item/CalamityBell") { Volume = 1.5f};

        public ref int EmpoweredHammer => ref Main.player[Projectile.owner].Calamity().StellarHammer; 
        public int returnhammer = 0;
        public int DustOnce = 1;
        public float rotatehammer = 8f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 11;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 62;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 120;
        }

        public override void AI()
        {
            // returnhammer determines if the hammer is slowing down after hitting an enemy, or homing in on the player.
            Player player = Main.player[Projectile.owner];

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.rotation += MathHelper.ToRadians(rotatehammer) * Projectile.direction;

            if (EmpoweredHammer >= 5)
                EmpoweredHammer = 0;

            if (returnhammer == 0)
            {
                Projectile.velocity.X *= 0.981f;
                Projectile.velocity.Y += 0.226f;
            }

            if (returnhammer == 1)
            {
                if (EmpoweredHammer == 4)
                {
                    if (DustOnce == 1)
                    {
                        for (int i = 0; i < 20; ++i)
                        {
                            // Pick a random type of dust.
                            int dustID;
                            switch (Main.rand.Next(6))
                            {
                                case 0:
                                    dustID = 229;
                                    break;
                                case 1:
                                case 2:
                                    dustID = 156;
                                    break;
                                default:
                                    dustID = 156;
                                    break;
                            }

                            // Choose a random speed and angle for the dust.
                            float dustSpeed = Main.rand.NextFloat(3.0f, 19.0f);
                            float angleRandom = 0.09f;
                            Vector2 dustVel = new Vector2(0.0f, dustSpeed * -1f);
                            dustVel = dustVel.RotatedBy(-angleRandom);
                            dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                            // Pick a size for the dust particle.
                            float scale = Main.rand.NextFloat(1.7f, 3.8f);

                            // Actually spawn the dust.
                            int idx = Dust.NewDust(Projectile.Center, 1, 1, dustID, dustVel.X, dustVel.Y, 0, default, scale);
                            Main.dust[idx].noGravity = true;
                            Main.dust[idx].position = Projectile.Center;
                        }

                        DustOnce = 0;
                    }

                    Projectile.velocity.X *= 0.281f;
                    Projectile.velocity.Y -= 0.8f;
                    rotatehammer++;

                    if (Projectile.velocity.Y < -18f)
                    {
                        EmpoweredHammer = 0;
                        returnhammer = 3;
                    }
                }
                else
                {
                    Projectile.velocity.Y *= 0.926f;
                    Projectile.velocity.X *= 0.811f;
                    if (Projectile.velocity.X > -1.05f && Projectile.velocity.X < 1.05f & Projectile.velocity.Y > -1.05f && Projectile.velocity.Y < 1.05f)
                        returnhammer = 2;
                }
            }

            if (returnhammer == 2)
            {
                Projectile.extraUpdates = 2;
                float returnSpeed = StellarContempt.Speed;
                float acceleration = 1.1f;
                Player owner = Main.player[Projectile.owner];
                Vector2 playerCenter = owner.Center;
                float xDist = playerCenter.X - Projectile.Center.X;
                float yDist = playerCenter.Y - Projectile.Center.Y;
                float dist = (float)Math.Sqrt(xDist * xDist + yDist * yDist);
                dist = returnSpeed / dist;
                xDist *= dist;
                yDist *= dist;

                if (Projectile.velocity.X < xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X + acceleration;
                    if (Projectile.velocity.X < 0f && xDist > 0f)
                        Projectile.velocity.X += acceleration;
                }
                else if (Projectile.velocity.X > xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X - acceleration;
                    if (Projectile.velocity.X > 0f && xDist < 0f)
                        Projectile.velocity.X -= acceleration;
                }
                if (Projectile.velocity.Y < yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + acceleration;
                    if (Projectile.velocity.Y < 0f && yDist > 0f)
                        Projectile.velocity.Y += acceleration;
                }
                else if (Projectile.velocity.Y > yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - acceleration;
                    if (Projectile.velocity.Y > 0f && yDist < 0f)
                        Projectile.velocity.Y -= acceleration;
                }

                // Delete the projectile if it touches its owner, increase counter to the big hammer, and spawn a dustsplosion on the player that scales with how close they are to getting a big hammer.
                if (Main.myPlayer == Projectile.owner)
                {
                    if (Projectile.Hitbox.Intersects(player.Hitbox))
                    {
                        EmpoweredHammer++;

                        SoundEngine.PlaySound(SoundID.DD2_BetsysWrathShot with { Volume = 0.4f }, Projectile.Center);

                        for (int i = 0; i < 30; i++)
                        {
                            Dust fire = Dust.NewDustPerfect(Projectile.Center, 156);
                            fire.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(0.8f) * new Vector2(4f, 1.25f) * Main.rand.NextFloat(0.9f, 1f);
                            fire.velocity = fire.velocity.RotatedBy(Projectile.rotation - MathHelper.PiOver2);
                            fire.velocity += Projectile.velocity * (EmpoweredHammer * 0.04f);

                            fire.noGravity = true;
                            fire.scale = Main.rand.NextFloat(0.2f, 0.6f) * EmpoweredHammer;

                            fire = Dust.CloneDust(fire);
                            fire.velocity = Main.rand.NextVector2Circular(3f, 3f);
                            fire.velocity += Projectile.velocity * (EmpoweredHammer * 0.04f);
                        }

                        Projectile.Kill();
                    }
                }
            }
            if (returnhammer == 3)
            {
                if (Projectile.velocity.Y < 0f)
                {
                    Particle streak = new ManaDrainStreak(player, Main.rand.NextFloat(0.9f, 1.5f), Main.rand.NextVector2CircularEdge(2f, 2f) * Main.rand.NextFloat(85f, 335f), Main.rand.NextFloat(30f, 44f), Color.PaleTurquoise, Color.Turquoise, Main.rand.Next(15, 30), Projectile.Center);
                    GeneralParticleHandler.SpawnParticle(streak);
                    Projectile.velocity.Y += 0.6f;
                    Projectile.scale += 0.03f;
                }
                else
                {
                    Projectile.velocity *= 0f;
                    SoundEngine.PlaySound(RedHamSound, Projectile.Center);

                    int hammer = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<StellarContemptEcho>(), Projectile.damage * 6, Projectile.knockBack * 1.5f, Projectile.owner, 0f, Projectile.ai[1]);
                    Main.projectile[hammer].localAI[0] = Math.Sign(Projectile.velocity.X);
                    Main.projectile[hammer].netUpdate = true;
                    Projectile.Kill();
                }
            }

            // Spawn dust as the hammer travels.
            if (Main.rand.NextBool())
            {
                Vector2 offset = new Vector2(12, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(4, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.UltraBrightTorch, new Vector2(Projectile.velocity.X * 0.3f + velOffset.X, Projectile.velocity.Y * 0.3f + velOffset.Y), 100);
                dust.noGravity = true;
            }

            if (Main.rand.NextBool(6))
            {
                Vector2 offset = new Vector2(12, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(4, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.UltraBrightTorch, new Vector2(Projectile.velocity.X * 0.2f + velOffset.X, Projectile.velocity.Y * 0.2f + velOffset.Y), 100);
                dust.noGravity = true;
            }
        }

        // On hit play GONG and spawn dust.
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (returnhammer == 0)
            {
                if (Main.zenithWorld)
                    SoundEngine.PlaySound(UseSoundFunny with { Pitch = EmpoweredHammer * 0.1f - 0.1f }, Projectile.Center);
                else
                    SoundEngine.PlaySound(UseSound with { Pitch = EmpoweredHammer * 0.1f - 0.1f }, Projectile.Center);

                if (EmpoweredHammer == 4)
                {
                    Projectile.velocity.Y *= 0f;
                    Projectile.velocity.X *= 0f;
                }
                else
                    SpawnFlares(target.Center, target.width, target.height);

                returnhammer = 1;
            }

            float numberOfDusts = 40f;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 offset = new Vector2(4.8f, 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 4.1f));
                Vector2 velOffset = new Vector2(4f, 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 4.1f));
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, 229, new Vector2(velOffset.X, velOffset.Y));
                dust.noGravity = true;
                dust.velocity = velOffset;
                dust.scale = Main.rand.NextFloat(1.5f, 3.2f);
            }

            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.22f }, Projectile.Center);
            Projectile.ai[1] = target.whoAmI;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.numHits > 0)
                Projectile.damage = (int)(Projectile.damage * 0.9f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }

        private void SpawnFlares(Vector2 targetPos, int width, int height)
        {
            // Play the Lunar Flare sound centered on the user, not the target (consistent with Lunar Flare and Stellar Striker)
            Player user = Main.player[Projectile.owner];
            SoundEngine.PlaySound(SoundID.Item88 with { Volume = 0.4f }, Projectile.Center);
            Projectile.netUpdate = true;

            int numFlares = EmpoweredHammer + 1;
            int flareDamage = (int)(0.1f * Projectile.damage);
            float flareKB = 4f;
            for (int i = 0; i < numFlares; ++i)
            {
                float flareSpeed = Main.rand.NextFloat(9f, 13f);

                // Flares never come from straight up, there is always at least an 80 pixel horizontal offset
                float xDist = Main.rand.NextFloat(80f, 320f) * (Main.rand.NextBool() ? -1f : 1f);
                float yDist = Main.rand.NextFloat(1200f, 1440f);
                Vector2 startPoint = targetPos + new Vector2(xDist, -yDist);

                // The flare is somewhat inaccurate based on the size of the target.
                float xVariance = width / 4f;
                if (xVariance < 8f)
                    xVariance = 8f;
                float yVariance = height / 4f;
                if (yVariance < 8f)
                    yVariance = 8f;
                float xOffset = Main.rand.NextFloat(-xVariance, xVariance);
                float yOffset = Main.rand.NextFloat(-yVariance, yVariance);
                Vector2 offsetTarget = targetPos + new Vector2(xOffset, yOffset);

                // Finalize the velocity vector and make sure it's going at the right speed.
                Vector2 velocity = offsetTarget - startPoint;
                velocity.Normalize();
                velocity *= flareSpeed;

                float AI1 = Main.rand.Next(3);
                if (Projectile.owner == Main.myPlayer)
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), startPoint, velocity, ProjectileID.LunarFlare, flareDamage, flareKB, Main.myPlayer, 0f, AI1);
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].DamageType = DamageClass.MeleeNoSpeed;
                }
            }
        }
      
        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 3);
            return false;
        }
    }
}
        
