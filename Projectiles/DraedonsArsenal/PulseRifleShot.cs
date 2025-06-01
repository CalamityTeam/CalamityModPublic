using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Ranged;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PulseRifleShot : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private Color mainColor = Color.Orchid;
        private bool notSplit;
        private bool doDamage = false;
        private NPC closestTarget = null;
        private NPC lastTarget = null;
        private float distance;
        private int timesItCanHit = 3;
        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            // If it's hit targeted enemies enough, kill it
            if (timesItCanHit <= 0)
                Projectile.Kill();

            notSplit = Projectile.ai[1] == 0f;

            Lighting.AddLight(Projectile.Center, 0.3f, 0f, 0.5f);
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);
            Projectile.rotation = Projectile.velocity.ToRotation();

            float createDustVar = 10f;

            // Set split projectile stats
            if (Projectile.localAI[0] == 0 && !notSplit)
            {
                Projectile.extraUpdates = 1;
                Projectile.timeLeft = 1240;
                if (Projectile.ai[1] == 1)
                    SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal with { Volume = 1.7f, Pitch = 0.3f }, Projectile.Center);
            }
            if (notSplit)
            {
                doDamage = true;
                Projectile.extraUpdates = 100;
            }

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > createDustVar && notSplit)
            {
                // Main projectile visual
                float sizeBonus = MathHelper.Clamp(Projectile.localAI[0] * 0.009f, 0, 3.5f);
                if (targetDist < 1400f)
                {
                    GlowOrbParticle spark = new GlowOrbParticle(Projectile.Center, -Projectile.velocity * Main.rand.NextFloat(-0.01f, 0.01f), false, 30, 1.1f - Projectile.ai[1] + sizeBonus, Main.rand.NextBool(3) ? Color.DarkViolet : mainColor);
                    GeneralParticleHandler.SpawnParticle(spark);
                }

                if (targetDist < 1400f && Main.rand.NextBool())
                {
                    GlowOrbParticle spark = new GlowOrbParticle(Projectile.Center + Projectile.velocity * Main.rand.NextFloat(-2f, 2f), -Projectile.velocity * Main.rand.NextFloat(-0.01f, 0.01f), false, 30, 1.1f - Projectile.ai[1] + sizeBonus, Main.rand.NextBool(3) ? Color.DarkViolet : mainColor);
                    GeneralParticleHandler.SpawnParticle(spark);
                }

                // A lingering trail of dust you can see after the particle trail disipates
                if (Projectile.localAI[0] > 20 && Main.rand.NextBool())
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5), 66);
                    dust.scale = Main.rand.NextFloat(0.7f, 1.5f);
                    dust.velocity = Projectile.velocity * Main.rand.NextFloat(-3, 3);
                    dust.noGravity = true;
                    dust.color = Main.rand.NextBool(3) ? Color.DarkViolet : mainColor;
                    dust.noLight = true;
                }
            }

            #region Split Projectile AI
            if (!notSplit)
            {
                // Split projectile visuals
                if (targetDist < 1400f)
                {
                    GlowOrbParticle spark = new GlowOrbParticle(Projectile.Center, -Projectile.velocity * Main.rand.NextFloat(-0.01f, 0.01f), false, 5, 1.7f - Projectile.ai[1] * 0.18f, Main.rand.NextBool(3) ? Color.DarkViolet : mainColor);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                if (targetDist < 1400f && Main.rand.NextBool())
                {
                    GlowOrbParticle spark = new GlowOrbParticle(Projectile.Center + Projectile.velocity * Main.rand.NextFloat(-2f, 2f), -Projectile.velocity * Main.rand.NextFloat(-0.01f, 0.01f), false, 5, 1.7f - Projectile.ai[1] * 0.18f, Main.rand.NextBool(3) ? Color.DarkViolet : mainColor);
                    GeneralParticleHandler.SpawnParticle(spark);
                }

                if (Projectile.localAI[0] > 90)
                {
                    // Velocity must look like it has stoped, but can't actually be zero otherwise homing code doesn't work
                    if (Projectile.localAI[0] == 91)
                        Projectile.velocity *= 0.001f;

                    // When they begin the homing after a hit of spawning, do a few visuals
                    if (Projectile.localAI[0] == 120)
                    {
                        distance = 3000;

                        SoundStyle fire = new("CalamityMod/Sounds/Item/PulseSound");
                        SoundEngine.PlaySound(fire with { Volume = 0.35f, Pitch = 0.3f, MaxInstances = -1 }, Projectile.Center);
                            

                        Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, mainColor, new Vector2(1f, 1f), Main.rand.NextFloat(12f, 25f), 0f, 0.5f, 15);
                        GeneralParticleHandler.SpawnParticle(pulse);

                        for (int k = 0; k < 6; k++)
                        {
                            Dust dust = Dust.NewDustPerfect(Projectile.Center, 66);
                            dust.scale = Main.rand.NextFloat(0.6f, 1.1f);
                            dust.velocity = new Vector2(6, 6).RotatedByRandom(100) * Main.rand.NextFloat(0.05f, 0.8f);
                            dust.noGravity = true;
                            dust.color = Main.rand.NextBool(3) ? Color.DarkViolet : mainColor;
                            dust.noLight = true;
                        }
                    }
                    // Homing code
                    if (Projectile.localAI[0] > 120)
                    {
                        // Do damage if it's near its target otherwise don't, this is to prevent excessive hits as orbs rail through worm bosses and such
                        if (closestTarget is not null)
                            doDamage = Vector2.Distance(Projectile.Center, closestTarget.Center) < 10;
                        else
                            doDamage = false;

                        // Tracking code, originally was going to try only tracking the closest target not including the last target you hit, but I couldn't make it work
                        // Eventually I settled on how it works now and it seems to home consistently so I'm happy enough there
                        float projectileSpeed = 9.5f;
                        if (closestTarget is not null && closestTarget.active)
                        {
                            float targetDirectionRotation = Projectile.SafeDirectionTo(closestTarget.Center).ToRotation();
                            float turningRate = 10f + Projectile.localAI[0] * 0.00008f;
                            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetDirectionRotation, turningRate).ToRotationVector2() * projectileSpeed;
                        }
                        else
                        {
                            Projectile.velocity = Projectile.rotation.ToRotationVector2() * projectileSpeed;
                            Projectile.velocity *= 0.999f;
                        }
                        if (closestTarget is not null && Vector2.Distance(Projectile.Center, closestTarget.Center) < 10)
                        {
                            closestTarget = null;
                            distance = 3000;
                        }

                        // Add extra updates as it hits more times, this smoothy increases the speed without destroying velocity based visual effects
                        Projectile.extraUpdates = 5 + (int)(Projectile.numHits * 0.3f);
                        {
                            // Actual homing movement
                            for (int index = 0; index < Main.npc.Length; index++)
                            {
                                if (Main.npc[index].CanBeChasedBy(null, false) || Main.npc[index] == lastTarget)
                                {
                                    float extraDistance = (Main.npc[index].width / 2) + (Main.npc[index].height / 2);

                                    if (Vector2.Distance(Projectile.Center, Main.npc[index].Center) < (distance + extraDistance))
                                    {
                                        closestTarget = Main.npc[index];
                                        distance = Vector2.Distance(Projectile.Center, Main.npc[index].Center);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Minor dust trail for when they first spawn
                    if (Main.rand.NextBool(5))
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5), 66);
                        dust.scale = Main.rand.NextFloat(0.8f, 1.4f);
                        dust.velocity = -Projectile.velocity * Main.rand.NextFloat(0.2f, 0.5f);
                        dust.noGravity = true;
                        dust.color = Main.rand.NextBool(3) ? Color.DarkViolet : mainColor;
                        dust.noLight = true;
                    }
                    Projectile.velocity *= 0.98f;
                }
            }
            #endregion

            // Visuals for the shot as it exits the tip of the rifle
            if (Projectile.localAI[0] == createDustVar && notSplit)
                PulseBurst(4f, 5f);
        }

        public override bool? CanHitNPC(NPC target) => doDamage ? null : false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            bool onKill = target.life <= 0;
            if (notSplit)
            {
                Projectile.Kill();

                for (int i = 0; i <= 9; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 66);
                    dust.scale = Main.rand.NextFloat(0.4f, 1.1f);
                    dust.velocity = (Projectile.velocity * 4).RotateRandom(0.6f) * Main.rand.NextFloat(0.2f, 0.9f);
                    dust.noGravity = true;
                    dust.color = Main.rand.NextBool(3) ? Color.DarkViolet : mainColor;
                    dust.noLight = true;
                }

                // Since the beam looks wider as it travels, these hit particles scale a bit to fit that
                float sizeBonus = MathHelper.Clamp(Projectile.localAI[0] * 0.01f, 0, 3.5f);
                for (int k = 0; k < 9; k++)
                {
                    SparkParticle spark = new SparkParticle(Projectile.Center + Main.rand.NextVector2Circular(8, 8) - Projectile.velocity * 7, Projectile.velocity * Main.rand.NextFloat(0.7f, 3.1f), false, 30, 0.9f * Main.rand.NextFloat(0.9f, 1.1f) + sizeBonus, mainColor);
                    GeneralParticleHandler.SpawnParticle(spark);
                }

                // Spawn the 4 energy orbs
                // These should do a large fraction of the beam's damage so they will easily kill even some decently bulky enemies regular enemies in one hit
                // This is so it can better proc its on kill effect
                int numProj = 4;
                int projectileDamage = (int)(Projectile.damage * 0.5f);
                for (int i = 0; i < numProj; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (Projectile.velocity.SafeNormalize(Vector2.Zero) * (14f - i * 0.7f)) * ((i + 1) * 0.25f), ModContent.ProjectileType<PulseRifleShot>(), projectileDamage, Projectile.knockBack, Projectile.owner, 0f, 1f + i);
                }
            }
            // Split projectile on hit effects
            else
            {
                // Set some values to get ready foir it to home again for its next hit
                lastTarget = target;
                distance = 3000;
                Projectile.localAI[0] = 60;
                Projectile.velocity *= MathHelper.Clamp(1.5f - Projectile.numHits * 0.5f, 1f, 1.5f);

                for (int i = 0; i <= 2; i++)
                {
                    SquishyLightParticle energy = new SquishyLightParticle(Projectile.Center, (Projectile.velocity * 2).RotatedByRandom(0.7f) * Main.rand.NextFloat(0.1f, 0.4f), Main.rand.NextFloat(0.1f, 0.25f), Main.rand.NextBool(3) ? Color.DarkViolet : mainColor, Main.rand.Next(20, 30 + 1), 0.25f, 2f);
                    GeneralParticleHandler.SpawnParticle(energy);
                }

                if (hit.Damage > 2 && target == closestTarget)
                    timesItCanHit--;

                if (onKill)
                {
                    timesItCanHit += 1;
                    Projectile.timeLeft += 90;
                }
            }
        }
        private void PulseBurst(float speed1, float speed2)
        {
            for (int i = 0; i <= 15; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 66);
                dust.scale = Main.rand.NextFloat(0.4f, 1.4f);
                dust.velocity = (Projectile.velocity * 4).RotateRandom(0.6f) * Main.rand.NextFloat(0.2f, 0.9f);
                dust.noGravity = true;
                dust.color = Main.rand.NextBool(3) ? Color.DarkViolet : mainColor;
                dust.noLight = true;
            }
            for (int i = 0; i <= 15; i++)
            {
                SquishyLightParticle energy = new SquishyLightParticle(Projectile.Center, (Projectile.velocity * 4).RotatedByRandom(0.6f) * Main.rand.NextFloat(0.1f, 0.4f), Main.rand.NextFloat(0.2f, 0.6f), Main.rand.NextBool(3) ? Color.DarkViolet : mainColor, Main.rand.Next(30, 40 + 1), 0.25f, 2f);
                GeneralParticleHandler.SpawnParticle(energy);
            }
        }
    }
}
