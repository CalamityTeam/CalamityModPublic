using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public static class GeneralParticleHandler
    {
        /// <summary>
        /// This is the maximum of particles allowed. Particles marked as important may replace nonimportant particles when the cap is reached
        /// </summary>
        // May be a good idea to make this into a config option
        private static readonly int particleCap = 500;

        private static Particle[] particles;
        private static int nextVacantIndex;
        private static int activeParticles;
        //Static list for details concerning every particle type
        private static Dictionary<Type, int> particleTypes;
        private static Dictionary<int, Texture2D> particleTextures;
        private static List<Particle> particleInstances;
        //Lists used when drawing particles batched
        private static List<Particle> batchedAlphaBlendParticles;
        private static List<Particle> batchedAdditiveBlendParticles;

        internal static void Load()
        {
            particles = new Particle[particleCap];
            particleTypes = new Dictionary<Type, int>();
            particleTextures = new Dictionary<int, Texture2D>();
            particleInstances = new List<Particle>();

            batchedAlphaBlendParticles = new List<Particle>(particleCap);
            batchedAdditiveBlendParticles = new List<Particle>(particleCap);

            Type baseParticleType = typeof(Particle);
            CalamityMod calamity = ModContent.GetInstance<CalamityMod>();

            foreach (Type type in calamity.Code.GetTypes())
            {
                if (type.IsSubclassOf(baseParticleType) && !type.IsAbstract && type != baseParticleType)
                {
                    int ID = particleTypes.Count; //Get the ID of the particle
                    particleTypes[type] = ID;

                    Particle instance = (Particle)FormatterServices.GetUninitializedObject(type);
                    particleInstances.Add(instance);

                    string texturePath = type.Namespace.Replace('.', '/') + "/" + type.Name;
                    if (instance.Texture != "")
                        texturePath = instance.Texture;
                    particleTextures[ID] = ModContent.GetTexture(texturePath);
                }
            }
        }

        internal static void Unload()
        {
            particles = null;
            particleTypes = null;
            particleTextures = null;
            particleInstances = null;
            batchedAlphaBlendParticles = null;
            batchedAdditiveBlendParticles = null;
        }

        /// <summary>
		/// Spawns the particle instance provided into the world. If the particle limit is reached but the particle is marked as important, it will try to replace a non important particle.
		/// </summary>
		public static void SpawnParticle(Particle particle)
        {
            if (activeParticles == particleCap)
                return;

            particles[nextVacantIndex] = particle;
            particle.ID = nextVacantIndex;
            particle.Type = particleTypes[particle.GetType()];

            //If the next index is also vacant
            if (nextVacantIndex + 1 < particles.Length && particles[nextVacantIndex + 1] == null)
                nextVacantIndex++;
            //If it isnt, go check for the first empty spot.
            else
                for (int i = 0; i < particles.Length; i++)
                    if (particles[i] == null)
                    {
                        nextVacantIndex = i;
                        break;
                    }

            activeParticles++;
        }

        public static void Update()
        {
            foreach (Particle particle in particles)
            {
                if (particle == null)
                    continue;
                particle.Position += particle.Velocity;
                particle.Time++;
                particle.Update();

                //Clear out particles whose time is up
                if (particle.Time >= particle.Lifetime && particle.SetLifetime)
                    particle.Kill();
            }
        }

        public static void RemoveParticle(int position)
        {
            particles[position] = null;
            nextVacantIndex = position;
            activeParticles--;
        }

        public static void DrawAllParticles(SpriteBatch sb)
        {
            //Batch the particles to avoid constant restarting of the spritebatch
            foreach (Particle particle in particles)
            {
                if (particle == null)
                    continue;

                if (particle.UseAdditiveBlend)
                    batchedAdditiveBlendParticles.Add(particle);
                else
                    batchedAlphaBlendParticles.Add(particle);
            }

            foreach (Particle particle in batchedAlphaBlendParticles)
            {
                if (particle.UseCustomDraw)
                    particle.CustomDraw(sb);
                else
                {
                    Rectangle frame = particleTextures[particle.Type].Frame(1, particle.FrameVariants, 0, particle.Variant);
                    sb.Draw(particleTextures[particle.Type], particle.Position - Main.screenPosition, frame, particle.Color, particle.Rotation, frame.Size() * 0.5f, particle.Scale, SpriteEffects.None, 0f);
                }
            }

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, null, null, Main.GameViewMatrix.ZoomMatrix);

            foreach (Particle particle in batchedAdditiveBlendParticles)
            {
                if (particle.UseCustomDraw)
                    particle.CustomDraw(sb);
                else
                {
                    Rectangle frame = particleTextures[particle.Type].Frame(1, particle.FrameVariants, 0, particle.Variant);
                    sb.Draw(particleTextures[particle.Type], particle.Position - Main.screenPosition, frame, particle.Color, particle.Rotation, frame.Size() * 0.5f, particle.Scale, SpriteEffects.None, 0f);
                }
            }

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            batchedAlphaBlendParticles.Clear();
            batchedAdditiveBlendParticles.Clear();
        }
        /// <summary>
        /// Gives you the texture of the particle type. Useful for custom drawing
        /// </summary>
        public static Texture2D GetTexture(int type) => particleTextures[type];
    }
}
