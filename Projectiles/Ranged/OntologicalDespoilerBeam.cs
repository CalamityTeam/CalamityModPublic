using System;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using CalamityMod.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class OntologicalDespoilerBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public ref float time => ref Projectile.ai[0];
        public Color baseColor = Color.White;
        public bool fading = false;
        public int sineDir = 0;
        public float fadeMultiplier = 1;
        public override void SetDefaults()
        {
            Projectile.width = 45;
            Projectile.height = 45;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 80;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);
            bool inRange = targetDist < 1400;
            if (sineDir == 0)
                sineDir = Main.rand.NextBool() ? 1 : -1;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (time <= 5)
                Projectile.scale = 0.1f;
            else if (Projectile.scale < 1 && !fading)
                Projectile.scale += 0.07f;
            else if (fading)
            {
                Projectile.velocity *= 0.992f;
                Projectile.scale *= 0.992f;
            }
            if (fading)
                fadeMultiplier -= 0.007f;
            if (time > 11 && inRange)
            {
                if (fadeMultiplier > 0.01f)
                {
                    for (int i = -1 * sineDir; (sineDir == 1 ? (i <= 1) : (i >= -1)); i += 2 * sineDir)
                    {
                        float sine = (float)Math.Sin(Projectile.timeLeft * 0.25f / MathHelper.Pi);

                        Particle beam = new CustomSpark(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * sine * 35 * i * fadeMultiplier, Projectile.velocity.SafeNormalize(Vector2.UnitX) * 4 * Main.rand.NextFloat(0.6f, 0.8f), "CalamityMod/Particles/Light", false, 45, 0.4f * fadeMultiplier * MathHelper.Lerp(Math.Abs(sine), 0.55f, 0.6f), i == 1 ? Color.White : Color.Black, new Vector2(1f, 1f), i == 1 ? true : false, false, 0, false, false, 0, noShrink: true);
                        GeneralParticleHandler.SpawnParticle(beam);
                    }
                }

                if (Main.rand.NextBool(4) && !fading)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(20, 20), (Main.rand.NextBool() ? ModContent.DustType<VoidDustInverted>() : ModContent.DustType<VoidDust>()), Projectile.velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(2.3f, 5.8f));
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(1.3f, 2.15f);
                    dust.color = baseColor;
                }
                if (Main.rand.NextBool(12) && !fading)
                {
                    Particle beam = new CustomSpark(Projectile.Center - Projectile.velocity * 8, Projectile.velocity * Main.rand.NextFloat(0.5f, 4f), "CalamityMod/Particles/DrainLine", false, 80, 4.5f * Projectile.scale, Color.Black, new Vector2(0.4f, 3f), false, false, 0, false, false, 0);
                    GeneralParticleHandler.SpawnParticle(beam);
                }
                if (time % (fading ? 2 : 1) == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        bool glow = i == 0;
                        Particle beam = new CustomSpark(Projectile.Center - Projectile.velocity * 8, -Projectile.velocity * 0.1f, glow ? "CalamityMod/Particles/VoidBeamGlow" : "CalamityMod/Particles/VoidBeam", false, 23, 2.3f * Projectile.scale, glow ? baseColor : Color.Black, Vector2.One, glow ? true : false, false, 0, false, false, 0.5f);
                        GeneralParticleHandler.SpawnParticle(beam);
                    }
                }
            }
            time++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 30; i++)
            {
                float dustPower = Main.rand.NextFloat(0.2f, 1f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, (Main.rand.NextBool() ? ModContent.DustType<VoidDustInverted>() : ModContent.DustType<VoidDust>()), (Projectile.velocity * 15 * (dustPower * dustPower)).RotatedByRandom(1f - dustPower * dustPower) * Main.rand.NextFloat(0.9f, 1f));
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(2.15f, 3.45f) * dustPower;
                dust.color = baseColor;
                Dust dust3 = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, (Projectile.velocity * 25 * (dustPower * dustPower)).RotatedByRandom(1f - dustPower * dustPower) * Main.rand.NextFloat(0.9f, 1f));
                dust3.noGravity = true;
                dust3.scale = Main.rand.NextFloat(1.35f, 1.75f) * dustPower;
                dust3.color = baseColor;
                Particle orb2 = new AltLineParticle(Projectile.Center, (Projectile.velocity * 15 * (dustPower * dustPower)).RotatedByRandom(1f - dustPower * dustPower) * Main.rand.NextFloat(0.9f, 1f), false, Main.rand.Next(30, 38 + 1), Main.rand.NextFloat(2.6f, 3.3f) * dustPower, Color.Black);
                GeneralParticleHandler.SpawnParticle(orb2);
            }
            Particle orb = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomRing", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0.5f, 1.35f, 18);
            GeneralParticleHandler.SpawnParticle(orb);
            for (int i = 0; i < 6; i++)
            {
                Particle orb3 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Black, "CalamityMod/Particles/SmallBloom", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 1.05f - i * 0.1f, 0.4f,  25 - i * 2, false);
                GeneralParticleHandler.SpawnParticle(orb3);
            }

            fading = true;
            Projectile.velocity *= 0.5f;
            Projectile.scale = 1.3f;

            for (int i = 0; i < 3; i++)
            {
                SoundStyle fire = new("CalamityMod/Sounds/Item/OntologicalDespoilerLargeImpact");
                SoundEngine.PlaySound(fire with { Volume = 0.65f, MaxInstances = -1}, Projectile.Center);
            }
            SoundStyle fire2 = new("CalamityMod/Sounds/Item/MeldExplosion");
            SoundEngine.PlaySound(fire2 with { Volume = 1, Pitch = Main.rand.NextFloat(-0.5f, -0.6f), MaxInstances = -1 }, Projectile.Center);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ApplyScalingForcedCrit(Projectile);
            Player Owner = Main.player[Projectile.owner];
            Vector2 launchVel = Projectile.velocity;
            float launchPower = 60;
            target.MoveNPC(launchVel, launchPower, true, Owner);
        }
        public override bool? CanDamage() => Projectile.numHits < 1 ? null : false;
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
