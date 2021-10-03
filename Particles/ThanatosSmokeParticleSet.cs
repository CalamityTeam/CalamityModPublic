using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
		public override int ParticleLifetime => 30;
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
			if (particle.Scale < 1.25f)
				particle.Scale += RelativePower * 0.04f;
			particle.RelativeOffset -= (BaseMoveRotation + Main.rand.NextFloat(-0.18f, 0.18f)).ToRotationVector2() * RelativePower * 4.5f;
		}

		public override Color DetermineParticleColor(float lifetimeCompletion)
		{
			Color color = Color.DarkRed;
			color = Color.Lerp(color, new Color(154, 139, 138), (float)Math.Pow(Utils.InverseLerp(0f, 0.57f, lifetimeCompletion, true), 2D) * 0.5f + 0.5f);
			color.A = 92;

			float opacity = Utils.InverseLerp(1f, 0.85f, lifetimeCompletion, true);
			return color * opacity;
		}

		public override Particle SpawnParticle() => new Particle(ParticleLifetime, Main.rand.NextVector2Circular(1f, 1f) * SpawnAreaCompactness, 0.06f, Main.rand.Next(3));
	}
}