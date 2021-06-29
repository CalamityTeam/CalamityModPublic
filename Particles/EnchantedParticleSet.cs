using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
	public class ChargingEnergyParticleSet : BaseParticleSet
	{
		public float InterpolationSpeed;
		public float EdgeOffset;
		public Color EdgeColor;
		public Color CenterColor;
		public override int ParticleLifetime => 50;
		public override Texture2D ParticleTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Particles/Light");
		public ChargingEnergyParticleSet(int setLifetime, int particleSpawnRate, Color edgeColor, Color centerColor, float interpolationSpeed, float edgeOffset) : 
			base(setLifetime, particleSpawnRate)
		{
			EdgeColor = edgeColor;
			CenterColor = centerColor;
			InterpolationSpeed = interpolationSpeed;
			EdgeOffset = edgeOffset;
		}

		public override void UpdateBehavior(Particle particle)
		{
			float distanceToCenter = particle.RelativeOffset.Length();
			particle.Scale = MathHelper.SmoothStep(0.05f, 0.125f, Utils.InverseLerp(EdgeOffset, 6f, distanceToCenter, true));
			particle.Scale *= Utils.InverseLerp(ParticleLifetime, ParticleLifetime - 10f, particle.Time, true);

			if (distanceToCenter > 4.5f)
				particle.RelativeOffset = Vector2.Lerp(particle.RelativeOffset, Vector2.Zero, InterpolationSpeed);
		}

		public override Color DetermineParticleColor(float lifetimeCompletion)
		{
			Color particleColor = Color.Lerp(EdgeColor, CenterColor, Utils.InverseLerp(0f, 0.67f, lifetimeCompletion, true));
			particleColor.A = 50;

			return particleColor;
		}

		public override Particle SpawnParticle() => new Particle(ParticleLifetime, Main.rand.NextVector2CircularEdge(1f, 1f) * EdgeOffset, 0.1f);
	}
}