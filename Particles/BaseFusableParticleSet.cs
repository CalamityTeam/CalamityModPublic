using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Particles
{
    public abstract class BaseFusableParticleSet
    {
        public class FusableParticleRenderCollection
        {
            public BaseFusableParticleSet ParticleSet;
            public List<RenderTarget2D> BackgroundTargets;
            public FusableParticleRenderCollection(BaseFusableParticleSet set, List<RenderTarget2D> backgroundTargets)
            {
                ParticleSet = set;
                BackgroundTargets = backgroundTargets;
            }
        }

        public class FusableParticle
        {
            public Vector2 Center;
            public float Size;
            public Vector2 TopLeft => Center + new Vector2(-1f, -1f) * Size * 0.5f;
            public Vector2 TopRight => Center + new Vector2(1f, -1f) * Size * 0.5f;
            public Vector2 BottomLeft => Center + new Vector2(-1f, 1f) * Size * 0.5f;
            public Vector2 BottomRight => Center + new Vector2(1f, 1f) * Size * 0.5f;

            public FusableParticle(Vector2 center, float size)
            {
                Center = center;
                Size = size;
            }
        }

        public int LayerCount
        {
            get
            {
                // Not doing this will result in inevitable crashes down the line as the graphics engine fails.
                if (BackgroundShaders.Count != BackgroundTextures.Count)
                    throw new InvalidOperationException("The number of texture maps and shaders are not equivalent.");

                return BackgroundShaders.Count;
            }
        }

        public List<FusableParticle> Particles = new List<FusableParticle>();

        public FusableParticleRenderCollection RenderCollection => FusableParticleManager.GetParticleRenderCollectionByType(GetType());

        public List<RenderTarget2D> GetBackgroundTargets => RenderCollection.BackgroundTargets;

        public virtual float BorderSize => 0f;
        public virtual bool BorderShouldBeSolid => false;
        public virtual Color BorderColor => Color.Transparent;
        public virtual void PrepareOptionalShaderData(Effect effect, int index) { }
        public abstract FusableParticleRenderLayer RenderLayer { get; }
        public abstract List<Effect> BackgroundShaders { get; }
        public abstract List<Texture2D> BackgroundTextures { get; }
        public abstract FusableParticle SpawnParticle(Vector2 center, float sizeStrength);
        public abstract void UpdateBehavior(FusableParticle particle);
        public abstract void DrawParticles();

        /// <summary>
        /// Temporarily moves from the backbuffer to a specialized render target, draws all particles, and caches the resulting render target for drawing later.
        /// </summary>
        internal void PrepareRenderTargetForDrawing()
        {
            // Don't bother doing anything if this method is called serverside.
            if (Main.netMode == NetmodeID.Server)
                return;

            // Don't bother doing anything if the set has no particles to render at the moment, for the sake of optimization.
            if (Particles.Count <= 0)
                return;

            // Go through each background render target in the set and clear the entire thing to use a base of transparent pixels.
            foreach (RenderTarget2D backgroundTarget in GetBackgroundTargets)
            {
                Main.instance.GraphicsDevice.SetRenderTarget(backgroundTarget);
                Main.instance.GraphicsDevice.Clear(Color.Transparent);

                // Clear away any particles that shouldn't exist anymore.
                Particles.RemoveAll(p => p.Size <= 5f);

                // Prepare the sprite batch for specialized drawing in prepration that the graphics device will draw to new render targets.
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);

                // Draw the surviving particles.
                DrawParticles();

                Main.spriteBatch.End();
            }

            // Return to using the backbuffer after done drawing everything to the background targets.
            Main.instance.GraphicsDevice.SetRenderTarget(null);
        }
    }
}
