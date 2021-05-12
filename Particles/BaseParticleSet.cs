using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Particles
{
	public abstract class BaseParticleSet
	{
		public int LocalTimer { get; internal set; }
		public int SetLifetime { get; internal set; }
		public int ParticleSpawnRate;
		public List<Particle> Particles = new List<Particle>();
		public abstract int ParticleLifetime { get; }
		public abstract void UpdateBehavior(Particle particle);
		public abstract Particle SpawnParticle();
		public abstract Color DetermineParticleColor(float lifetimeCompletion);
		public abstract Texture2D ParticleTexture { get; }
		public virtual int ParticleFrameVariants => 1;
		public virtual Func<Particle, int> OrderFunction { get; } = null;

		public BaseParticleSet(int setLifetime, int particleSpawnRate)
		{
			SetLifetime = setLifetime;
			ParticleSpawnRate = particleSpawnRate;
		}

		public virtual void Update()
		{
			// Don't perform any operations on the server. Doing so would be a waste of space as these sets are entirely based on drawcode.
			if (Main.netMode == NetmodeID.Server)
				return;

			bool closeToDeath = LocalTimer >= SetLifetime - ParticleLifetime && SetLifetime > 0;
			if (LocalTimer % ParticleSpawnRate == 0 && !closeToDeath)
			{
				Particle particle = SpawnParticle();
				particle.Lifetime = ParticleLifetime;
				particle.Variant = Main.rand.Next(ParticleFrameVariants);
				Particles.Add(particle);
			}

			// Update and increment the time of all particles.
			for (int i = 0; i < Particles.Count; i++)
			{
				Particles[i].Time++;
				UpdateBehavior(Particles[i]);
			}

			// Clear all expired particles.
			Particles.RemoveAll(particle => particle.Time >= particle.Lifetime);
			LocalTimer++;
		}

		public virtual void DrawSet(Vector2 basePosition)
		{
			if (Main.netMode == NetmodeID.Server)
				return;

			IEnumerable<Particle> orderedParticles = Particles;
			if (OrderFunction != null)
				orderedParticles = orderedParticles.OrderBy(OrderFunction);

			foreach (Particle particle in Particles.OrderBy(p => p.Time))
			{
				Vector2 drawPosition = basePosition - Main.screenPosition + particle.RelativeOffset;
				Color particleColor = DetermineParticleColor(particle.Time / (float)particle.Lifetime);
				Rectangle frame = ParticleTexture.Frame(1, ParticleFrameVariants, 0, particle.Variant);

				Main.spriteBatch.Draw(ParticleTexture, drawPosition, frame, particleColor, 0f, frame.Size() * 0.5f, particle.Scale, SpriteEffects.None, 0f);
			}
		}
	}
}