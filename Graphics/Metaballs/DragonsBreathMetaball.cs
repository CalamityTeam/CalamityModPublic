using System.Collections.Generic;
using System.Linq;
using CalamityMod.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Graphics.Metaballs
{
    public class DragonsBreathFlameMetaball : DragonsBreathMetaball
    {
        // OrangeRed is FF4500
        // new Color(0xFF, 0x3A, 0x0A);
        public override Color EdgeColor => Color.Orange;

        // Flame metaballs never turn white
        public override void DrawInstances() => DrawInstancesInternal(0.03f, -1f);
    }
    public class DragonsBreathBeamMetaball : DragonsBreathMetaball
    {
        private static float whiteSizeThreshold = 72f;

        public override Color EdgeColor => Color.Orange;

        // Laser metaballs very suddenly turn white at a specific size
        public override void DrawInstances() => DrawInstancesInternal(0.4f, whiteSizeThreshold);
    }

    public abstract class DragonsBreathMetaball : Metaball
    {
        // Identical behavior to Blood Boiler particles
        public class DragonsBreathParticle
        {
            public Vector2 Center;
            public float Size;

            public DragonsBreathParticle(Vector2 center, float size)
            {
                Center = center;
                Size = size;
            }

            public void Update()
            {
                // Always slowly shrink the particles.
                Size = MathHelper.Clamp(Size - 0.1f, 0f, 200f) * 0.91f;

                // Once sufficiently small, the particles very rapidly shrink.
                if (Size < 5f)
                    Size = Size * 0.8f - 1f;
            }
        }

        public List<DragonsBreathParticle> Particles
        {
            get;
            private set;
        } = new();

        public override bool AnythingToDraw => Particles.Any();

        public override IEnumerable<Texture2D> Layers
        {
            get
            {
                yield return ModContent.Request<Texture2D>("CalamityMod/Projectiles/InvisibleProj").Value;
            }
        }

        public override MetaballDrawLayer DrawContext => MetaballDrawLayer.AfterProjectiles;

        public override void Update()
        {
            // Update all particle instances.
            // Once sufficiently small, they vanish.
            for (int i = 0; i < Particles.Count; i++)
                Particles[i].Update();
            Particles.RemoveAll(p => p.Size <= 2f);
        }

        // Copied from Rancor Lava metaballs, since these need to be additive metaballs.
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

        public void SpawnParticle(Vector2 position, float size) => Particles.Add(new(position, size));

        internal void DrawInstancesInternal(float opacity, float whiteSizeThreshold)
        {
            float pureWhiteIntensity = 0.16f;
            Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Graphics/Metaballs/MetaballMessy").Value;

            foreach (DragonsBreathParticle particle in Particles)
            {
                Vector2 drawPosition = particle.Center - Main.screenPosition;
                var origin = tex.Size() * 0.5f;
                Vector2 scale = Vector2.One * particle.Size / tex.Size();

                Color drawColor;
                if (whiteSizeThreshold >= 0f)
                {
                    float pureWhiteInterpolant = Utils.GetLerpValue(0.8f * whiteSizeThreshold, whiteSizeThreshold, particle.Size, true) * pureWhiteIntensity;
                    drawColor = Color.Lerp(EdgeColor, Color.DarkOrange, pureWhiteInterpolant) * opacity;
                }
                else
                    drawColor = Color.Lerp(EdgeColor, new Color(0xE0, 0x70, 0x10), Utils.GetLerpValue(60f, 100f, particle.Size, true) * 0.75f);

                Main.spriteBatch.Draw(tex, drawPosition, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}
