using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class PhasedGodRay : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        private const float LaserLength = 80f;
        private const float LaserLengthChangeRate = 2f;

        // Do not change these unless you are absolutely sure you know how to fix the wave math.
        // They are extremely carefully chosen and barely work as is!
        private const float WaveTheta = 0.09f;
        private const int WaveTwistFrames = 9;
        private ref float WaveFrameState => ref Projectile.ai[0];

        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.MaxUpdates = 3;
            Projectile.timeLeft = 280;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            // Very rapidly fade into existence.
            if (Projectile.alpha > 0)
                Projectile.alpha -= 25;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            float waveSign = WaveFrameState < 0f ? -1f : 1f;

            // Initialize waving. Setting ai[0] to a number between -1 and 1 tells it which way to wave.
            // Exactly 0 is a coinflip.
            if (Math.Abs(WaveFrameState) < 1f)
            {
                float dirToUse = WaveFrameState == 0f ? (Main.rand.NextBool() ? -1f : 1f) : waveSign;
                waveSign = -dirToUse;
                WaveFrameState = dirToUse * WaveTwistFrames * 0.5f;

                // Backfill old rotations to prevent visual glitches.
                float iterRotation = Projectile.velocity.ToRotation();
                for (int i = 0; i < Projectile.oldRot.Length; ++i)
                {
                    Projectile.oldRot[i] = iterRotation;
                    iterRotation += waveSign * WaveTheta;
                }
            }
            // Switch waving directions as necessary.
            else if (Math.Abs(WaveFrameState) > WaveTwistFrames)
                WaveFrameState = -waveSign;
            else
                WaveFrameState += waveSign;

            // Apply a constant, rapid wave to the laser's motion.
            Projectile.velocity = Projectile.velocity.RotatedBy(waveSign * WaveTheta);
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Emit light.
            Lighting.AddLight(Projectile.Center, 0.87f, 0.65f, 0.1725f);

            // Laser length shenanigans. If the laser is still growing, increase localAI 0 to indicate it is getting longer.
            if (Projectile.ai[1] == 0f)
            {
                Projectile.localAI[0] += 10f; // LaserLengthChangeRate;

                // Cap it at max length.
                if (Projectile.localAI[0] > LaserLength)
                    Projectile.localAI[0] = LaserLength;
            }

            // Otherwise it's shrinking. Once it reaches zero length it dies for good.
            else
            {
                Projectile.localAI[0] -= LaserLengthChangeRate;
                if (Projectile.localAI[0] <= 0f)
                    Projectile.Kill();
            }

            float fadeInLerp = Utils.GetLerpValue(280, 265, Projectile.timeLeft, true);
            Color beamColor = Color.Lerp(AetherfluxCannon.accentColor, AetherfluxCannon.mainColor, fadeInLerp);
            Color subColor = Color.Lerp(AetherfluxCannon.accentColor, AetherfluxCannon.mainColor, Main.rand.NextFloat());

            Particle beamBody = new CustomSpark(Projectile.Center, -Projectile.velocity * 0.05f, "CalamityMod/Particles/BloomCircle", false, 7, 0.65f - 0.3f * fadeInLerp, beamColor * 0.8f, new Vector2(1.1f - 0.4f * fadeInLerp, 0.8f + 0.6f * fadeInLerp), true, false, shrinkSpeed: 0.6f);
            GeneralParticleHandler.SpawnParticle(beamBody);

            Particle beamCore = new CustomSpark(Projectile.Center, -Projectile.velocity * 0.05f, "CalamityMod/Particles/BloomCircle", false, 5, 0.35f - 0.15f * fadeInLerp, Color.Lerp(beamColor, Color.White, 0.6f), new Vector2(0.7f, 1.4f), true, false, shrinkSpeed: 0.6f);
            GeneralParticleHandler.SpawnParticle(beamCore);

            if (Projectile.timeLeft == 280)
            {
                for (int i = 0; i < 3; i++)
                {
                    subColor = Color.Lerp(AetherfluxCannon.accentColor, AetherfluxCannon.mainColor, Main.rand.NextFloat());
                    Particle spark = new CustomSpark(Projectile.Center - Projectile.velocity * 2, (Projectile.velocity).RotatedByRandom(0.8f) * Main.rand.NextFloat(0.5f, 0.7f), "CalamityMod/Particles/SemiCircularSmearVerticalBlank", false, 10, 0.45f, subColor, new Vector2(0.8f, 1.2f), true, false, shrinkSpeed: 0.5f);
                    GeneralParticleHandler.SpawnParticle(spark);

                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>());
                    dust2.scale = Main.rand.NextFloat(0.6f, 1.4f);
                    dust2.noGravity = true;
                    dust2.velocity = (Projectile.velocity).RotatedByRandom(0.8f) * Main.rand.NextFloat(0.3f, 0.8f);
                    dust2.color = subColor;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i< 2; i++)
            {
                Color color = AetherfluxCannon.accentColor;
                Particle orb2 = new CustomSpark(Projectile.Center, Vector2.Zero, "CalamityMod/Particles/BloomCircle", false, 20, Main.rand.NextFloat(0.15f, 0.25f) * 3, color, new Vector2(1f, 1f), true, false);
                GeneralParticleHandler.SpawnParticle(orb2);
            }
            Projectile.timeLeft = 280;
            SoundStyle hitSound = new("CalamityMod/Sounds/Item/MeldShoot");
            SoundEngine.PlaySound(hitSound with { Volume = 0.4f, Pitch = Main.rand.NextFloat(0.5f, 0.65f) }, Projectile.Center);
            if (Projectile.damage > 1)
                Projectile.damage = (int)(Projectile.damage * 0.865f);
        }


        public override Color? GetAlpha(Color lightColor) => new Color(222, 166, 44, 0);

        public override bool PreDraw(ref Color lightColor)
        {
            //Projectile.DrawBeam(LaserLength, 2f, lightColor, curve: true);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Color subColor = Color.Lerp(AetherfluxCannon.accentColor, AetherfluxCannon.mainColor, Main.rand.NextFloat());
                float rot = Main.rand.NextFloat(-0.1f, 0.1f);
                Vector2 startVel = (Projectile.velocity * 0.1f).RotatedBy(rot);
                Vector2 endVel = startVel.RotatedBy(rot * 5) * 20;
                Particle orb2 = new VelChangingSpark(Projectile.Center, startVel, endVel, "CalamityMod/Particles/BloomCircle", Main.rand.Next(15, 20 + 1), Main.rand.NextFloat(0.015f, 0.025f) * 15, subColor, new Vector2(1.1f, 1f), true, false, lerpRate: 0.015f, shrinkSpeed: 0.4f);
                GeneralParticleHandler.SpawnParticle(orb2);
            }
        }
    }
}
