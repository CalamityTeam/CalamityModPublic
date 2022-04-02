using Terraria;

namespace CalamityMod.Particles
{
	public class ThanatosSmokeParticleSet : BaseParticleSet
	{
		public override int ParticleLifetime => 30;
        public float SpawnAreaCompactness;
		public float RelativePower;
		public float BaseMoveRotation;
		public float MoveRotationOffset;

		public ThanatosSmokeParticleSet(int setLifetime, int particleSpawnRate, float baseMoveRotation, float spawnAreaCompactness, float relativePower) : 
			base(setLifetime, particleSpawnRate)
		{
			MoveRotationOffset = Main.rand.NextFloat(-0.36f, 0.36f);
			BaseMoveRotation = baseMoveRotation;
			SpawnAreaCompactness = spawnAreaCompactness;
			RelativePower = relativePower;
		}

		public override Particle SpawnParticle() => new ThanatosSmokeParticle(Main.rand.NextVector2Circular(1f, 1f) * SpawnAreaCompactness, ParticleLifetime, 0.06f, RelativePower, BaseMoveRotation);
	}
}