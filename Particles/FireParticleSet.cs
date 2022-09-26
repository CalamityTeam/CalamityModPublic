using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Particles
{
    public class FireParticleSet : BaseParticleSet
    {
        public float SpawnAreaCompactness;
        public float RelativePower;
        public Color BrightColor;
        public Color DarkColor;
        public override int ParticleLifetime => 50;

        public FireParticleSet(int setLifetime, int particleSpawnRate, Color brightColor, Color darkColor, float spawnAreaCompactness, float relativePower) :
            base(setLifetime, particleSpawnRate)
        {
            BrightColor = brightColor;
            DarkColor = darkColor;
            SpawnAreaCompactness = spawnAreaCompactness;
            RelativePower = relativePower;
        }

        public override Particle SpawnParticle() => new FireParticle(Main.rand.NextVector2Circular(1f, 1f) * SpawnAreaCompactness, ParticleLifetime, 0.06f, RelativePower, BrightColor, DarkColor);
    }
}
