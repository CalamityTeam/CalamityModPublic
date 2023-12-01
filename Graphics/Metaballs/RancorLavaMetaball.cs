using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Effects;
using System;

namespace CalamityMod.Graphics.Metaballs
{
    // CONSIDER -- Dominic 01NOV2023 This really should not be a metaball. Some sort of separately drawn fire effect would be far more suitable, and overall less awkward.
    // However, for consistency with the previous effect, I'll leave it as is.
    public class RancorLavaMetaball : Metaball
    {
        public class LavaParticle
        {
            public int Time;

            public float Size;

            public Vector2 Center;

            public LavaParticle(Vector2 center, float size)
            {
                Center = center;
                Size = size;
            }

            public void Update()
            {
                Time++;
                Size = MathHelper.Clamp(Size - 0.24f, 0f, 200f) * 0.9956f;
                if (Size < 15f)
                    Size = Size * 0.95f - 0.9f;
            }
        }

        public static List<LavaParticle> Particles
        {
            get;
            private set;
        } = new();

        public override bool AnythingToDraw => Particles.Any();

        public override MetaballDrawLayer DrawContext => MetaballDrawLayer.AfterProjectiles;

        public override IEnumerable<Texture2D> Layers
        {
            get
            {
                yield return ModContent.Request<Texture2D>("CalamityMod/Projectiles/InvisibleProj").Value;
            }
        }

        public override Color EdgeColor => new(255, 56, 3);

        public override void ClearInstances() => Particles.Clear();

        public override void PrepareShaderForTarget(int layerIndex)
        {
            // Store the shader in an easy to use local variable.
            var metaballShader = CalamityShaders.AdditiveMetaballEdgeShader;

            // Calculate the layer scroll offset. This is used to ensure that the texture contents of the given metaball have parallax, rather than being static over the screen
            // regardless of world position.
            Vector2 screenSize = new(Main.screenWidth, Main.screenHeight);

            // Supply shader parameter values.
            metaballShader.Parameters["screenArea"]?.SetValue(screenSize);
            metaballShader.Parameters["layerOffset"]?.SetValue(Vector2.Zero);
            metaballShader.Parameters["singleFrameScreenOffset"]?.SetValue(Vector2.Zero);

            // Apply the metaball shader.
            metaballShader.CurrentTechnique.Passes[0].Apply();
        }

        public static void SpawnParticle(Vector2 position, float size) =>
            Particles.Add(new(position, size));

        public override void Update()
        {
            // Update all particle instances.
            // Once sufficiently small, they vanish.
            for (int i = 0; i < Particles.Count; i++)
                Particles[i].Update();
            Particles.RemoveAll(p => p.Size <= 3f);
        }

        public override void PrepareSpriteBatch(SpriteBatch spriteBatch)
        {
            // Draw with additive blending.
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);
        }

        public override void DrawInstances()
        {
            Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/SmallGreyscaleCircle").Value;

            // Draw all particles.
            foreach (LavaParticle particle in Particles)
            {
                Vector2 drawPosition = particle.Center - Main.screenPosition;
                Vector2 origin = tex.Size() * 0.5f;
                Vector2 scale = Vector2.One * particle.Size / tex.Size();
                Color lavaColor = EdgeColor * 1.2f;

                // Make the lava color vary slightly based on position, to make the effect less monotonous.
                lavaColor.B = (byte)(lavaColor.B + (int)(MathF.Cos(particle.Center.Y * 0.015f + Main.GlobalTimeWrappedHourly * 0.1f) * 3f));

                // Make the lava color bright for the first few frames of its existence.
                float brightnessInterpolant = Utils.GetLerpValue(10f, 2f, particle.Time, true) * 0.67f;
                lavaColor = Color.Lerp(lavaColor, Color.Wheat, brightnessInterpolant);

                Main.spriteBatch.Draw(tex, drawPosition, null, lavaColor, 0f, origin, scale, 0, 0f);
            }
        }
    }
}
