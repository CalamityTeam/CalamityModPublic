using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    /// <summary>
    /// A set of particles that aren't attached to any particular position in the world, but instead an arbitrary center point defined in the draw function
    /// The particles in particle sets don't count towards the general particle handler's particle cap since it doesn't process them at all.
    /// You'll have to manually update and draw these sets.
    /// TLDR: Groups of particles that work with relative positions instead of world positions and have to be manually updated and drawn
    /// </summary>
    public abstract class BaseParticleSet
    {
        public int LocalTimer { get; internal set; }
        public int SetLifetime { get; internal set; }
        /// <summary>
        /// The amount of frames it takes for a new particle to be spawned in the set.
        /// </summary>
        public int ParticleSpawnRate;
        /// <summary>
        /// The particles in the set
        /// </summary>
        public List<Particle> Particles = new List<Particle>();
        /// <summary>
        /// The lifetime of the particles in the set.
        /// If the particles spawned don't have this same lifetime set they may get cut off when the set dies.
        /// </summary>
        public abstract int ParticleLifetime { get; }
        /// <summary>
        /// The particle spawned by the set
        /// </summary>
        /// <returns></returns>
        public abstract Particle SpawnParticle();
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

            //Spawn new particles if time remains
            bool closeToDeath = LocalTimer >= SetLifetime - ParticleLifetime && SetLifetime > 0;
            if (LocalTimer % ParticleSpawnRate == ParticleSpawnRate - 1 && !closeToDeath)
            {
                Particle particle = SpawnParticle();
                Particles.Add(particle);
            }

            // Update and increment the time of all particles, alongside modifying their offset based on their velocity.
            foreach(Particle particle in Particles)
            {
                particle.RelativeOffset += particle.Velocity;
                particle.Time++;
                particle.Update();
            }

            // Clear all expired particles.
            Particles.RemoveAll(particle => particle.Time >= particle.Lifetime && particle.SetLifetime);
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
                if (particle.UseCustomDraw)
                {
                    particle.CustomDraw(Main.spriteBatch, basePosition);
                }
                else
                {
                    var tex = ModContent.Request<Texture2D>(particle.Texture).Value;

                    Vector2 drawPosition = basePosition - Main.screenPosition + particle.RelativeOffset;
                    Color particleColor = particle.Color;
                    Rectangle frame = tex.Frame(1, particle.FrameVariants, 0, particle.Variant);

                    Main.spriteBatch.Draw(tex, drawPosition, frame, particle.Color, 0f, frame.Size() * 0.5f, particle.Scale, SpriteEffects.None, 0f);
                }
            }
        }
    }
}
