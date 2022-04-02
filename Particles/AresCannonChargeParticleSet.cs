using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Particles
{
    public class AresCannonChargeParticleSet : BaseParticleSet
    {
        public override int ParticleLifetime => 30;
        public Color ParticleColor;
        public float SpawnAreaCompactness;
        public float MoveRotationOffset;
        public float chargeProgress = 0f;
        public Particle bloom;
        public List<Particle> Pulses = new List<Particle>();

        public AresCannonChargeParticleSet(int setLifetime, int particleSpawnRate, float spawnAreaCompactness, Color particleColor) :
            base(setLifetime, particleSpawnRate)
        {
            ParticleColor = particleColor;
            MoveRotationOffset = Main.rand.NextFloat(-0.36f, 0.36f);
            SpawnAreaCompactness = spawnAreaCompactness;
        }

        //We'll use more than one here actually, so this doesnt matter that much
        public override Particle SpawnParticle()
        {
            Vector2 originPoint = Main.rand.NextVector2CircularEdge(1f, 1f) * SpawnAreaCompactness;
            return new ChargeUpLineVFX(originPoint, originPoint.ToRotation(), 0.5f, ParticleColor, ParticleLifetime, 0.75f, true, 0.2f, 10f);
        }

        public CurveSegment Rise1 = new CurveSegment(EasingType.SineInOut, 0f, 0f, 0.25f);
        public CurveSegment Fall1 = new CurveSegment(EasingType.SineInOut, 0.13f, 0.25f, -0.05f);

        public CurveSegment Rise2 = new CurveSegment(EasingType.SineInOut, 0.2f, 0.2f, 0.3f);
        public CurveSegment Fall2 = new CurveSegment(EasingType.SineInOut, 0.33f, 0.5f, -0.1f);

        public CurveSegment Rise3 = new CurveSegment(EasingType.SineInOut, 0.4f, 0.4f, 0.3f);
        public CurveSegment Fall3 = new CurveSegment(EasingType.SineInOut, 0.53f, 0.7f, -0.1f);

        public CurveSegment Rise4 = new CurveSegment(EasingType.SineInOut, 0.6f, 0.6f, 0.3f);
        public CurveSegment Fall4 = new CurveSegment(EasingType.SineInOut, 0.73f, 0.9f, -0.1f);

        public CurveSegment Rise5 = new CurveSegment(EasingType.SineInOut, 0.8f, 0.8f, 0.3f);
        public CurveSegment Finale = new CurveSegment(EasingType.SineIn, 0.93f, 1.1f, -1.1f);
        public float ChargeSize() => PiecewiseAnimation(chargeProgress, new CurveSegment[] { Rise1, Fall1, Rise2, Fall2, Rise3, Fall3, Rise4, Fall4, Rise5, Finale });

        public override void Update()
        {
            // Don't perform any operations on the server. Doing so would be a waste of space as these sets are entirely based on drawcode.
            if (Main.netMode == NetmodeID.Server)
                return;

            if (bloom == null)
                bloom = new StrongBloom(Vector2.Zero, Vector2.Zero, ParticleColor, 0.5f, 2);
            else
            {
                bloom.Time = 0;
                bloom.Color = ParticleColor * 0.8f * chargeProgress;
                bloom.Scale = ChargeSize() * 2f;
            }

            //Spawn new particles if time remains
            bool closeToDeath = LocalTimer >= SetLifetime - ParticleLifetime && SetLifetime > 0;
            if (LocalTimer % ParticleSpawnRate == ParticleSpawnRate - 1 && !closeToDeath)
            {
                Particle particle = SpawnParticle();
                Particles.Add(particle);
            }

            // Update and increment the time of all particles, alongside modifying their offset based on their velocity.
            foreach (Particle particle in Particles)
            {
                particle.RelativeOffset += particle.Velocity;
                particle.Time++;
                particle.Update();
            }

            foreach (Particle pulse in Pulses)
            {
                pulse.Time++;
                pulse.Update();
            }

            // Clear all expired particles.
            Particles.RemoveAll(particle => particle.Time >= particle.Lifetime && particle.SetLifetime);
            Pulses.RemoveAll(pulse => pulse.Time >= pulse.Lifetime && pulse.SetLifetime);
            LocalTimer++;
        }

        public void DrawBloom(Vector2 basePosition)
        {
            if (bloom != null)
            {
                bloom.CustomDraw(Main.spriteBatch, basePosition);
            }
        }

        public void AddPulse(float pulseCounter)
        {
            Particle pulse = new PulseRing(Vector2.Zero, Vector2.Zero, pulseCounter == 5 ? Color.Lerp(Color.White, ParticleColor, 0.5f) : ParticleColor, 0.2f, 0.5f * pulseCounter * 0.5f, 20);
            Pulses.Add(pulse);
        }

        public void DrawPulses(Vector2 basePosition)
        {
            foreach (Particle pulse in Pulses)
            {
                pulse.CustomDraw(Main.spriteBatch, basePosition);
            }
        }
    }
}
