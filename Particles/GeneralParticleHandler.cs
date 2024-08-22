using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace CalamityMod.Particles
{
    // TODO -- This can be made into a ModSystem with simple OnModLoad and Unload hooks.
    public static class GeneralParticleHandler
    {
        private static List<Particle> particles;
        //List containing the particles to delete
        private static List<Particle> particlesToKill;
        //Static list for details concerning every particle type
        internal static Dictionary<Type, int> particleTypes;
        internal static Dictionary<int, Texture2D> particleTextures;
        private static List<Particle> particleInstances;
        //Lists used when drawing particles batched
        private static List<Particle> batchedAlphaBlendParticles;
        private static List<Particle> batchedNonPremultipliedParticles;
        private static List<Particle> batchedAdditiveBlendParticles;

        public static void LoadModParticleInstances()
        {
            Type baseParticleType = typeof(Particle);
            foreach (Mod mod in ModLoader.Mods)
            {
                foreach (Type type in AssemblyManager.GetLoadableTypes(mod.Code))
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
                        particleTextures[ID] = ModContent.Request<Texture2D>(texturePath, AssetRequestMode.ImmediateLoad).Value;
                    }
                }
            }
        }

        internal static void Load()
        {
            particles = new List<Particle>();
            particlesToKill = new List<Particle>();
            particleTypes = new Dictionary<Type, int>();
            particleTextures = new Dictionary<int, Texture2D>();
            particleInstances = new List<Particle>();

            batchedAlphaBlendParticles = new List<Particle>();
            batchedNonPremultipliedParticles = new List<Particle>();
            batchedAdditiveBlendParticles = new List<Particle>();
        }

        internal static void Unload()
        {
            particles = null;
            particlesToKill = null;
            particleTypes = null;
            particleTextures = null;
            particleInstances = null;

            batchedAlphaBlendParticles = null;
            batchedNonPremultipliedParticles = null;
            batchedAdditiveBlendParticles = null;
        }

        /// <summary>
        /// Spawns the particle instance provided into the world. If the particle limit is reached but the particle is marked as important, it will try to replace a non important particle.
        /// </summary>
        public static void SpawnParticle(Particle particle)
        {
            // Don't queue particles if the game is paused.
            // This precedent is established with how Dust instances are created.
            //Don't spawn particles if on the server side either, or if the particles dict is somehow null
            if (Main.gamePaused || Main.dedServ || particles == null)
                return;

            if (particles.Count >= CalamityConfig.Instance.ParticleLimit && !particle.Important)
                return;

            particles.Add(particle);
            particle.Type = particleTypes[particle.GetType()];
        }

        public static void Update()
        {
            if (Main.dedServ)
                return;

            foreach (Particle particle in particles)
            {
                if (particle == null)
                    continue;
                particle.Position += particle.Velocity;
                particle.Time++;
                particle.Update();
            }
            //Clear out particles whose time is up
            particles.RemoveAll(particle => (particle.Time >= particle.Lifetime && particle.SetLifetime) || particlesToKill.Contains(particle));
            particlesToKill.Clear();
        }

        public static void RemoveParticle(Particle particle)
        {
            if (Main.dedServ)
                return;

            particlesToKill.Add(particle);
        }

        public static void DrawAllParticles(SpriteBatch sb)
        {
            if (Main.dedServ)
                return;

            if (particles.Count == 0)
                return;

            sb.End();
            var rasterizer = Main.Rasterizer;
            rasterizer.ScissorTestEnable = true;
            Main.instance.GraphicsDevice.RasterizerState.ScissorTestEnable = true;
            Main.instance.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

            //Batch the particles to avoid constant restarting of the spritebatch
            foreach (Particle particle in particles)
            {
                if (particle == null)
                    continue;

                if (particle.UseAdditiveBlend)
                    batchedAdditiveBlendParticles.Add(particle);
                else if (particle.UseHalfTransparency)
                    batchedNonPremultipliedParticles.Add(particle);
                else
                    batchedAlphaBlendParticles.Add(particle);
            }
            if (batchedAlphaBlendParticles.Count > 0)
            {
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                foreach (Particle particle in batchedAlphaBlendParticles)
                {
                    if (particle.UseCustomDraw)
                        particle.CustomDraw(sb);
                    else
                    {
                        Rectangle frame = particleTextures[particle.Type].Frame(1, particle.FrameVariants, 0, particle.Variant);
                        sb.Draw(particleTextures[particle.Type], particle.Position - Main.screenPosition, frame, particle.Color, particle.Rotation, frame.Size() * 0.5f,
                            particle.Scale, SpriteEffects.None, 0f);
                    }
                }
                sb.End();
            }


            if (batchedNonPremultipliedParticles.Count > 0)
            {
                rasterizer = Main.Rasterizer;
                rasterizer.ScissorTestEnable = true;
                Main.instance.GraphicsDevice.RasterizerState.ScissorTestEnable = true;
                Main.instance.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
                sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                foreach (Particle particle in batchedNonPremultipliedParticles)
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
            }

            if (batchedAdditiveBlendParticles.Count > 0)
            {
                rasterizer = RasterizerState.CullNone;
                rasterizer.ScissorTestEnable = true;
                Main.instance.GraphicsDevice.RasterizerState.ScissorTestEnable = true;
                Main.instance.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
                sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

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
            }

            batchedAlphaBlendParticles.Clear();
            batchedNonPremultipliedParticles.Clear();
            batchedAdditiveBlendParticles.Clear();

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        }

        /// <summary>
        /// Gives you the amount of particle slots that are available. Useful when you need multiple particles at once to make an effect and dont want it to be only halfway drawn due to a lack of particle slots
        /// </summary>
        /// <returns></returns>
        public static int FreeSpacesAvailable()
        {
            //Safety check
            if (Main.dedServ || particles == null)
                return 0;

            return CalamityConfig.Instance.ParticleLimit - particles.Count();
        }

        /// <summary>
        /// Gives you the texture of the particle type. Useful for custom drawing
        /// </summary>
        public static Texture2D GetTexture(int type)
        {
            if (Main.dedServ)
                return null;

            return particleTextures[type];
        }

#pragma warning disable CS0414
        private static string noteToEveryone = "This particle system was inspired by spirit mod's own particle system, with permission granted by Yuyutsu. Love you spirit mod! -Iban";
#pragma warning restore CS0414
    }
}
