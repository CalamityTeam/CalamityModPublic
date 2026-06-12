using System;
using CalamityMod.Dusts;
using CalamityMod.Enums;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class CosmicTentacle : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public ref float time => ref Projectile.ai[0];
        public bool preDamage = true;
        public bool moving = false;
        public float scaling = 1;
        public int scalingTimer = 0;
        public Color InnerColor = Color.LightGreen;
        public int curveDirection = 100;
        public int curves = 3;

        public int scalingTimerMax = 90;
        public float damageMult = 1;
        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 8;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8 * Projectile.extraUpdates;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);
            // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
            Vector2 vel = ((Owner.Calamity().mouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX) * 7).RotatedBy(0.2f * curveDirection);

            if (curveDirection == 100)
            {
                curveDirection = Main.rand.NextBool() ? 1 : -1;
            }
            if (preDamage)
            {
                if (targetDist < 1400f && time > 3)
                {
                    float scaler = Utils.GetLerpValue(-60, 120, time, true);
                    Particle blastRing = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Black, "CalamityMod/Particles/LargeBloom", Vector2.One, Main.rand.NextFloat(-10, 10), 0.6f * scaler, 0f, 4, false);
                    GeneralParticleHandler.SpawnParticle(blastRing);
                    Particle blastRing2 = new CustomPulse(Projectile.Center, Vector2.Zero, InnerColor * 0.6f, "CalamityMod/Particles/LargeBloom", Vector2.One, Main.rand.NextFloat(-10, 10), 0.48f * scaler, 0f, 4, true);
                    GeneralParticleHandler.SpawnParticle(blastRing2, false, GeneralDrawLayer.AfterEverything);
                }
                Projectile.velocity *= 0.96f;
                if (time == 3)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        float variance = Main.rand.NextFloat(-0.4f, 0.4f);
                        int dustStyle = ModContent.DustType<VoidDustInverted>();
                        Dust dust2 = Dust.NewDustPerfect(Projectile.Center, dustStyle);
                        dust2.scale = Main.rand.NextFloat(0.8f, 1.2f) - Math.Abs(variance);
                        dust2.velocity = Projectile.velocity.RotatedBy(variance) * Main.rand.NextFloat(1.2f, 1.5f) * (1 - Math.Abs(variance));
                        dust2.noGravity = true;
                        dust2.color = Color.LightGreen;
                    }
                }
            }
            else
            {
                if (moving)
                {
                    float curveStrength = Utils.GetLerpValue(scalingTimerMax, 0, scalingTimer, true) * 0.04f;
                    Projectile.velocity = Projectile.velocity.RotatedBy(curveStrength * curveDirection);
                    scaling = Utils.GetLerpValue(0, scalingTimerMax, scalingTimer, true);
                    float sharpScaling = Utils.GetLerpValue(scalingTimerMax * 0.7f, scalingTimerMax, scalingTimer, true);

                    if (targetDist < 1400f && Projectile.timeLeft % 2 == 0)
                    {
                        Particle spark = new CustomSpark(Projectile.Center, -Projectile.velocity * 0.05f, "CalamityMod/Particles/GlowSpark2", false, 12, 0.07f * scaling, Color.Black * 0.85f, new Vector2(1.7f - (1 - sharpScaling), 0.9f + (1 - sharpScaling) * 2), false);
                        GeneralParticleHandler.SpawnParticle(spark);
                        Particle spark2 = new CustomSpark(Projectile.Center, -Projectile.velocity * 0.05f, "CalamityMod/Particles/GlowSpark", false, 12, 0.035f * scaling, Color.LightGreen * 0.75f, new Vector2(1.7f - (1 - sharpScaling), 0.9f + (1 - sharpScaling) * 2));
                        GeneralParticleHandler.SpawnParticle(spark2, false, GeneralDrawLayer.AfterEverything);
                    }
                    if (Main.rand.NextBool(6))
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(6) ? 278 : 267, -Projectile.velocity);
                        dust.scale = dust.type == 278 ? Main.rand.NextFloat(0.3f, 0.6f) : Main.rand.NextFloat(0.6f, 1.2f);
                        dust.velocity = -Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.1f, 0.7f);
                        dust.noGravity = true;
                        dust.color = InnerColor;
                    }

                    scalingTimer--;
                    if (scalingTimer <= 0)
                    {
                        if (curves > 1)
                        {
                            Projectile.Center += Main.rand.NextVector2Circular(100, 100);
                            Particle blastRing = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Black, "CalamityMod/Particles/LargeBloom", Vector2.One, Main.rand.NextFloat(-10, 10), 0.6f, 0f, scalingTimerMax / 2, false);
                            GeneralParticleHandler.SpawnParticle(blastRing);
                            for (int i = 0; i < 2; i++)
                            {
                                Particle blastRing2 = new CustomPulse(Projectile.Center, Vector2.Zero, InnerColor, "CalamityMod/Particles/LargeBloom", Vector2.One, Main.rand.NextFloat(-10, 10), 0.36f, 0f, scalingTimerMax / 2, true);
                                GeneralParticleHandler.SpawnParticle(blastRing2, false, GeneralDrawLayer.AfterEverything);
                            }
                            Projectile.velocity = Vector2.Zero;
                        }
                        else
                            Projectile.Kill();
                        moving = false;
                    }
                }
                else
                {
                    scalingTimer += 2;
                    if (scalingTimer >= scalingTimerMax)
                    {
                        curves--;
                        if (curves > 0)
                        {
                            for (int i = 0; i <= 6; i++)
                            {
                                int dustStyle = ModContent.DustType<VoidDustInverted>();
                                Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 191 : dustStyle, Projectile.velocity);
                                dust.scale = Main.rand.NextFloat(0.9f, 1.4f);
                                dust.velocity = new Vector2(5, 5).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1f);
                                dust.noGravity = true;
                                dust.color = dust.type == dustStyle ? InnerColor : default;
                            }
                            Projectile.velocity = vel;
                            curveDirection = (curveDirection == 1 ? -1 : 1);
                            moving = true;
                            Projectile.numHits = 0;
                        }
                        else
                            Projectile.Kill();
                    }
                }
            }
            if (time == 120)
            {
                for (int i = 0; i <= 12; i++)
                {
                    int dustStyle = Main.rand.NextBool() ? 66 : 263;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, dustStyle, Projectile.velocity);
                    dust.scale = Main.rand.NextFloat(0.5f, 0.8f);
                    dust.velocity = new Vector2(7, 7).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1f);
                    dust.noGravity = true;
                    dust.color = Color.LightGreen;
                }
                preDamage = false;
                moving = true;
                scalingTimer = scalingTimerMax;
                Projectile.velocity = vel;
            }
            time++;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits == 0)
            {
                SoundStyle fire = new("CalamityMod/Sounds/Item/MeldShoot");
                SoundEngine.PlaySound(fire with { Volume = 0.3f, Pitch = 0.9f }, Projectile.Center);
                for (int i = 0; i < 6; i++)
                {
                    float variance = Main.rand.NextFloat(-0.6f, 0.6f);
                    int dustStyle = ModContent.DustType<VoidDustInverted>();
                    Dust dust2 = Dust.NewDustPerfect(target.Center, dustStyle);
                    dust2.scale = Main.rand.NextFloat(1.2f, 1.6f) - Math.Abs(variance);
                    dust2.velocity = (Projectile.velocity * 1.5f).RotatedBy(variance) * Main.rand.NextFloat(1.2f, 1.5f) * (1 - Math.Abs(variance));
                    dust2.noGravity = true;
                    dust2.color = Color.LightGreen;
                }
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // it was GOING to have this but deus made it real bad
            // renabled and tested on deus, no noticable effect
            damageMult = MathHelper.Clamp(Utils.GetLerpValue(5, 1, Projectile.numHits), 0.7f, 1);
            modifiers.SourceDamage *= damageMult;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 8; i++)
            {
                int dustStyle = Main.rand.NextBool() ? 66 : 263;
                Dust dust = Dust.NewDustPerfect(Projectile.Center, dustStyle, Projectile.velocity);
                dust.scale = Main.rand.NextFloat(0.5f, 0.8f);
                dust.velocity = Projectile.velocity.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.3f, 3.1f);
                dust.noGravity = true;
                dust.color = InnerColor;
            }
        }
        public override bool? CanDamage() => preDamage ? false : null;
    }
}
