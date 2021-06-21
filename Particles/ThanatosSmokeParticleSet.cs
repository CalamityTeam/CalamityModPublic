using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
	public class ThanatosSmokeParticleSet : BaseParticleSet
	{
		public float SpawnAreaCompactness;
		public float RelativePower;
		public float BaseMoveRotation;
		public float MoveRotationOffset;
		public override int ParticleLifetime => 50;
		public override int ParticleFrameVariants => 3;
		public override Texture2D ParticleTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/ThanatosVentParticle");
		public ThanatosSmokeParticleSet(int setLifetime, int particleSpawnRate, float baseMoveRotation, float spawnAreaCompactness, float relativePower) : 
			base(setLifetime, particleSpawnRate)
		{
			MoveRotationOffset = Main.rand.NextFloat(-0.36f, 0.36f);
			BaseMoveRotation = baseMoveRotation;
			SpawnAreaCompactness = spawnAreaCompactness;
			RelativePower = relativePower;
		}

		public override void UpdateBehavior(Particle particle)
		{
			particle.Scale += RelativePower * 0.02f;
			particle.RelativeOffset -= (BaseMoveRotation + Main.rand.NextFloat(-0.38f, 0.38f)).ToRotationVector2() * RelativePower * 4.5f;
		}

		public override Color DetermineParticleColor(float lifetimeCompletion) => Color.White * Utils.InverseLerp(1f, 0.85f, lifetimeCompletion, true);

		public override Particle SpawnParticle() => new Particle(ParticleLifetime, Main.rand.NextVector2Circular(1f, 1f) * SpawnAreaCompactness, 0.06f, Main.rand.Next(3));
	}
}