using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Particles
{
    public class ChargingEnergyParticleSet : BaseParticleSet
    {
        public float InterpolationSpeed;
        public float EdgeOffset;
        public Color EdgeColor;
        public Color CenterColor;
        public override int ParticleLifetime => 50;
        public ChargingEnergyParticleSet(int setLifetime, int particleSpawnRate, Color edgeColor, Color centerColor, float interpolationSpeed, float edgeOffset) : 
            base(setLifetime, particleSpawnRate)
        {
            EdgeColor = edgeColor;
            CenterColor = centerColor;
            InterpolationSpeed = interpolationSpeed;
            EdgeOffset = edgeOffset;
        }

        public override Particle SpawnParticle() => new EnchantedParticle(Main.rand.NextVector2CircularEdge(1f, 1f) * EdgeOffset, ParticleLifetime, 0.1f, EdgeColor, CenterColor, InterpolationSpeed, EdgeOffset);
    }
}