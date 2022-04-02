using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

using static CalamityMod.Particles.BaseFusableParticleSet;

namespace CalamityMod.Particles
{
    // NOTE: A lot of the functionalities within this system cannot under really any circumstance be used serverside.
    // This system is entirely visual. However, servers do not have drawing capabilities, nor even a proper GraphicsDevice
    // instance. This system utilizes many things that require both of these things, such as shaders and render targets.
    // As such, it is littered with many netMode checks. While some may be overprotective in their nature it is critical that
    // failsafes exist there, as visual failures tend to result in engine crashes, which will kill a lot of MP servers.
    // If someone other than me for any reason attempts to notably expand upon this system you must keep this in mind. -Dominic
    public static class FusableParticleManager
    {
        internal static List<FusableParticleRenderCollection> ParticleSets = new List<FusableParticleRenderCollection>();
        internal static bool HasBeenFormallyDefined = false;

        /// <summary>
        /// Loads all render sets.
        /// </summary>
        internal static void LoadParticleRenderSets(bool reload = false, int width = -1, int height = -1)
        {
            // Don't attempt to prepare anything serverside.
            if (Main.netMode == NetmodeID.Server)
                return;

            // Use fallbacks for width and height based on the screen.
            if (width == -1 || height == -1)
            {
                width = Main.screenWidth;
                height = Main.screenHeight;
            }

            // Otherwise, if a width and height are defined, but they are the exact same as the screen bounds,
            // do nothing. This indicates that the render targets already have the correct size and re-initailizing would be unnecessary.
            else if (width == Main.screenWidth && height == Main.screenHeight)
                return;

            // Redefine the particle set list in case the mod was reloaded and this field was nullified during that.
            // This does not happen during reloads, as that would delete particles outside of load-time.
            if (!reload)
            {
                ParticleSets = new List<FusableParticleRenderCollection>();
                HasBeenFormallyDefined = true;
            }

            // Look through every type in the mod, and check if it's derived from BaseFusableParticleSet.
            // If it is, create a default instance of said particle, save it, and create a RenderTarget2D for each individual texture/shader.
            foreach (Type type in typeof(CalamityMod).Assembly.GetTypes())
            {
                // Don't load abstract classes; they cannot have instances.
                if (type.IsAbstract)
                    continue;

                if (type.IsSubclassOf(typeof(BaseFusableParticleSet)))
                {
                    BaseFusableParticleSet instance = Activator.CreateInstance(type) as BaseFusableParticleSet;
                    List<RenderTarget2D> backgroundTargets = new List<RenderTarget2D>();

                    // Only generate render targets to use if this isn't called serverside.
                    if (Main.netMode != NetmodeID.Server)
                    {
                        for (int i = 0; i < instance.LayerCount; i++)
                            backgroundTargets.Add(new RenderTarget2D(Main.instance.GraphicsDevice, width, height, false, default, default, 0, RenderTargetUsage.PreserveContents));
                    }

                    FusableParticleRenderCollection particleRenderCollection = new FusableParticleRenderCollection(instance, backgroundTargets);
                    ParticleSets.Add(particleRenderCollection);
                }
            }
        }

        /// <summary>
        /// Unloads all render sets, disposing any created <see cref="RenderTarget2D"/>s in the process.
        /// </summary>
        internal static void UnloadParticleRenderSets()
        {
            // Go through each render collection and manually clear away any render targets.
            foreach (FusableParticleRenderCollection particleRenderSet in ParticleSets)
            {
                particleRenderSet.ParticleSet = null;
                foreach (RenderTarget2D target in particleRenderSet.BackgroundTargets)
                    target?.Dispose();
            }
            HasBeenFormallyDefined = false;
            ParticleSets = null;
        }

        /// <summary>
        /// Prepares the render targets of each cached <see cref="FusableParticleRenderCollection"/> instance in advance.
        /// </summary>
        internal static void PrepareFusableParticleTargets()
        {
            // Don't attempt to prepare anything serverside, if the particle sets are unloaded, or if on the menu.
            if (Main.netMode == NetmodeID.Server || ParticleSets is null || Main.gameMenu)
                return;

            // Prepare the render target for all fusable particles.
            for (int i = 0; i < ParticleSets.Count; i++)
                ParticleSets[i].ParticleSet.PrepareRenderTargetForDrawing();
        }

        /// <summary>
        /// Renders each cached <see cref="FusableParticleRenderCollection"/> instance with its connecting shader.
        /// </summary>
        internal static void RenderAllFusableParticles()
        {
            // Don't attempt to render anything serverside or on the menu.
            if (Main.netMode == NetmodeID.Server || Main.gameMenu)
                return;

            for (int i = 0; i < ParticleSets.Count; i++)
            {
                BaseFusableParticleSet particleSet = ParticleSets[i].ParticleSet;

                // Don't bother rendering if the set has no particles to render at the moment, for the sake of optimization.
                if (particleSet.Particles.Count <= 0)
                    continue;

                List<RenderTarget2D> backgroundTargets = particleSet.GetBackgroundTargets;

                // Restart the sprite batch. This must be done with an immediate sort mode since a shader is going to be applied.
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                for (int j = 0; j < particleSet.LayerCount; j++)
                {
                    Effect shader = particleSet.BackgroundShaders[j];
                    Texture2D backgroundTexture = particleSet.BackgroundTextures[j];

                    // Draw the current target with the specified shader.
                    shader.Parameters["edgeBorderSize"].SetValue(particleSet.BorderSize);
                    shader.Parameters["borderShouldBeSolid"].SetValue(particleSet.BorderShouldBeSolid);
                    shader.Parameters["edgeBorderColor"].SetValue(particleSet.BorderColor.ToVector3());
                    shader.Parameters["screenArea"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight) / Main.GameViewMatrix.Zoom);
                    shader.Parameters["screenMoveOffset"].SetValue(Main.screenPosition - Main.screenLastPosition);
                    shader.Parameters["uWorldPosition"].SetValue(Main.screenPosition);
                    shader.Parameters["renderTargetArea"].SetValue(new Vector2(backgroundTargets[j].Width, backgroundTargets[j].Height));
                    shader.Parameters["invertedScreen"].SetValue(Main.LocalPlayer.gravDir == -1f);
                    shader.Parameters["upscaleFactor"].SetValue(Vector2.One);
                    shader.Parameters["generalBackgroundOffset"].SetValue(Vector2.Zero);

                    // Prepare the background texture for loading.
                    Main.graphics.GraphicsDevice.Textures[1] = backgroundTexture;
                    shader.Parameters["uImageSize1"].SetValue(backgroundTexture.Size());

                    // Do any optional shader operations prior to applying the shader.
                    particleSet.PrepareOptionalShaderData(shader, j);

                    // Apply the shader. This is what is used to connect separated blobs together.
                    shader.CurrentTechnique.Passes[0].Apply();

                    // And draw the particle set's render target with said shader.
                    Main.spriteBatch.Draw(backgroundTargets[j], Vector2.Zero, Color.White);
                }

                Main.spriteBatch.End();
            }
        }

        /// <summary>
        /// Obtains the <see cref="FusableParticleRenderCollection"/> instance from a given <see cref="Type"/> if it is a <see cref="BaseFusableParticleSet"/>.
        /// </summary>
        /// <param name="type">The type to check.</param>
        internal static FusableParticleRenderCollection GetParticleRenderCollectionByType(Type type)
        {
            if (Main.netMode == NetmodeID.Server)
                return null;

            return ParticleSets.First(s => s.ParticleSet.GetType() == type);
        }

        /// <summary>
        /// Obtains the cached singleton instance of a given <see cref="BaseFusableParticleSet"/>.
        /// </summary>
        /// <typeparam name="T">The type to check.</typeparam>
        public static T GetParticleSetByType<T>() where T : BaseFusableParticleSet
        {
            if (Main.netMode == NetmodeID.Server)
                return null;

            return ParticleSets.First(s => s.ParticleSet.GetType() == typeof(T)).ParticleSet as T;
        }
    }
}
