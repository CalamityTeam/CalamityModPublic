using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class StellarContemptHammer : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";

        public override string Texture => "CalamityMod/Items/Weapons/Melee/StellarContempt";
        public static readonly SoundStyle UseSound = new("CalamityMod/Sounds/Item/PwnagehammerSound") { Volume = 0.35f };
        public static readonly SoundStyle RedHamSound = new("CalamityMod/Sounds/Item/StellarContemptClone") { Volume = 0.6f };
        public static readonly SoundStyle UseSoundFunny = new("CalamityMod/Sounds/Item/CalamityBell") { Volume = 1.5f };
        public ref int EmpoweredHammer => ref Main.player[Projectile.owner].Calamity().StellarHammer;
        public int returnhammer = 0;
        public int DustOnce = 1;
        public float rotatehammer = 15f;
        public int time = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 11;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 74;
            Projectile.height = 74;
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
                int falloffTime = 15;
                if (time > falloffTime)
                    Projectile.velocity.X *= 0.967f;
                if (Projectile.velocity.Y < 15 && time > falloffTime)
                    Projectile.velocity.Y += 0.426f;
                if (Projectile.velocity.Y < 5)
                    Projectile.velocity.Y *= 0.98f;
            }

            if (returnhammer == 1)
            {
                if (EmpoweredHammer == 4)
                {
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
                float returnSpeed = StellarContempt.Speed * 0.7f;
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
                            Dust fire = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>());
                            fire.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(0.8f) * new Vector2(4f, 1.25f) * Main.rand.NextFloat(0.9f, 1f);
                            fire.velocity = fire.velocity.RotatedBy(Projectile.rotation - MathHelper.PiOver2);
                            fire.velocity += Projectile.velocity * (EmpoweredHammer * 0.04f);
                            fire.color = Main.rand.NextBool(3) ? Color.PaleTurquoise : Color.Turquoise;

                            fire.noGravity = true;
                            fire.scale = Main.rand.NextFloat(0.2f, 0.6f) * EmpoweredHammer;

                            fire = Dust.BetterCloneDust(fire);
                            fire.velocity = Main.rand.NextVector2Circular(3f, 3f);
                            fire.velocity += Projectile.velocity * (EmpoweredHammer * 0.04f);
                            fire.color = Main.rand.NextBool(3) ? Color.PaleTurquoise : Color.Turquoise;
                        }

                        Projectile.Kill();
                    }
                }
            }
            if (returnhammer == 3)
            {
                if (Projectile.velocity.Y < 0f)
                {
                    float fade = Utils.GetLerpValue(3, -10, Projectile.velocity.Y, true);
                    float numberOfDusts = 2f;
                    float rotFactor = 360f / numberOfDusts;
                    for (int i = 0; i < numberOfDusts; i++)
                    {
                        float rot = MathHelper.ToRadians(i * rotFactor);
                        Vector2 velOffset = CalamityUtils.RandomVelocity(100f, 70f, 250f, 0.04f);
                        velOffset *= Main.rand.NextFloat(25, 45) * fade;
                        Particle energy = new SparkParticle(Projectile.Center + velOffset * 2.5f, -velOffset * Main.rand.NextFloat(0.08f, 0.12f) * 1.5f, false, 14, Main.rand.NextFloat(1.1f, 1.25f) - 0.5f * fade, Color.Turquoise);
                        GeneralParticleHandler.SpawnParticle(energy);
                        Dust dust = Dust.NewDustPerfect(Projectile.Center + velOffset * 2.5f, DustID.FireworksRGB, -velOffset * Main.rand.NextFloat(0.08f, 0.12f) * 1.5f, 0, default, Main.rand.NextFloat(0.4f, 0.6f));
                        dust.noGravity = true;
                        dust.color = Color.Turquoise;
                        dust.velocity += Projectile.velocity;
                    }

                    Projectile.velocity.Y += 0.6f;
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
            if (Main.rand.NextBool(3))
            {
                Vector2 offset = new Vector2(12, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(4, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, ModContent.DustType<LightDust>(), new Vector2(Projectile.velocity.X * 0.3f + velOffset.X, Projectile.velocity.Y * 0.3f + velOffset.Y));
                dust.noGravity = true;
                dust.color = Main.rand.NextBool(3) ? Color.PaleTurquoise : Color.Turquoise;
            }
            time++;
        }

        // On hit play GONG and spawn dust.
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (returnhammer == 0)
            {
                Projectile.ai[1] = target.whoAmI;

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

            float numberOfDusts = MathHelper.Clamp(40 - Projectile.numHits * 5, 6, 40);
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 offset = new Vector2(4.8f, 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 4.1f));
                Vector2 velOffset = new Vector2(4f, 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 4.1f));

                if (i % 3 == 0)
                {
                    Particle orb = new CustomSpark(Projectile.Center + offset, velOffset * Main.rand.NextFloat(1f, 1.5f), "CalamityMod/Particles/Sparkle", false, 25, Main.rand.NextFloat(0.55f, 0.75f), Main.rand.NextBool(3) ? Color.PaleTurquoise : Color.Turquoise, new Vector2(1f, 2f), true, true);
                    GeneralParticleHandler.SpawnParticle(orb);
                }
                else
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, ModContent.DustType<LightDust>(), new Vector2(velOffset.X, velOffset.Y));
                    dust.noGravity = true;
                    dust.velocity = velOffset * Main.rand.NextFloat(0.75f, 1);
                    dust.scale = Main.rand.NextFloat(0.9f, 1.6f);
                    dust.color = Main.rand.NextBool(3) ? Color.PaleTurquoise : Color.Turquoise;
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 90);
            float minMult = 0.7f;
            int hitsToMinMult = 10;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;
        }

        private void SpawnFlares(Vector2 targetPos, int width, int height)
        {
            // Play the Lunar Flare sound centered on the user, not the target (consistent with Lunar Flare and Stellar Striker)
            Player user = Main.player[Projectile.owner];
            SoundEngine.PlaySound(SoundID.Item88 with { Volume = 0.4f }, Projectile.Center);
            Projectile.netUpdate = true;

            int numFlares = EmpoweredHammer + 1;
            int flareDamage = (int)(0.05f * Projectile.damage);
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
                    {
                        Main.projectile[proj].DamageType = DamageClass.MeleeNoSpeed;
                        Main.projectile[proj].tileCollide = false;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 3);
            return false;
        }
    }
}

