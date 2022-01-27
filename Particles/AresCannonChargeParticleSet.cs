using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
	public class AresCannonChargeParticleSet : BaseParticleSet
	{
		public override int ParticleLifetime => 30;
		public Color ParticleColor;
		public float SpawnAreaCompactness;
		public float MoveRotationOffset;

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
			return new ChargeUpLineVFX(originPoint, originPoint.ToRotation(), 0.2f, ParticleColor, ParticleLifetime, 0.6f, true);
		}

		public override void Update()
		{
			// Don't perform any operations on the server. Doing so would be a waste of space as these sets are entirely based on drawcode.
			if (Main.netMode == NetmodeID.Server)
				return;

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

			// Clear all expired particles.
			Particles.RemoveAll(particle => particle.Time >= particle.Lifetime && particle.SetLifetime);
			LocalTimer++;
		}

	}
}