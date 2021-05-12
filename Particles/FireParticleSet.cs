using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
	public class FireParticleSet : BaseParticleSet
	{
		public float SpawnAreaCompactness;
		public float RelativePower;
		public Color BrightColor;
		public Color DarkColor;
		public override int ParticleLifetime => 50;
		public override Texture2D ParticleTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Particles/Fire");
		public FireParticleSet(int setLifetime, int particleSpawnRate, Color brightColor, Color darkColor, float spawnAreaCompactness, float relativePower) : 
			base(setLifetime, particleSpawnRate)
		{
			BrightColor = brightColor;
			DarkColor = darkColor;
			SpawnAreaCompactness = spawnAreaCompactness;
			RelativePower = relativePower;
		}

		public override void UpdateBehavior(Particle particle)
		{
			particle.Scale += RelativePower * 0.01f;
			particle.RelativeOffset.Y -= RelativePower * 3f;
		}

		public override Color DetermineParticleColor(float lifetimeCompletion)
		{
			Color particleColor = Color.Lerp(BrightColor, DarkColor, lifetimeCompletion);
			particleColor = Color.Lerp(particleColor, Color.SaddleBrown, Utils.InverseLerp(0.95f, 0.7f, lifetimeCompletion, true));
			particleColor = Color.Lerp(particleColor, Color.White, Utils.InverseLerp(0.1f, 0.25f, lifetimeCompletion, true) * Utils.InverseLerp(0.4f, 0.25f, lifetimeCompletion, true) * 0.7f);
			particleColor *= Utils.InverseLerp(0f, 0.15f, lifetimeCompletion, true) * Utils.InverseLerp(1f, 0.8f, lifetimeCompletion, true) * 0.6f;
			particleColor.A = 50;

			return particleColor;
		}

		public override Particle SpawnParticle() => new Particle(ParticleLifetime, Main.rand.NextVector2Circular(1f, 1f) * SpawnAreaCompactness, 0.06f);
	}
}