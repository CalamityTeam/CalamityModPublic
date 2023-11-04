using CalamityMod.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Graphics.Metaballs
{
    public class CyanPaint : PaintMetaball
    {
        public override Color EdgeColor => Color.Cyan * 0.7f;
    }

    public class BluePaint : PaintMetaball
    {
        public override Color EdgeColor => Color.Blue * 0.7f;
    }

    public class MagentaPaint : PaintMetaball
    {
        public override Color EdgeColor => Color.Magenta * 0.7f;
    }

    public class LimePaint : PaintMetaball
    {
        public override Color EdgeColor => Color.Lime * 0.7f;
    }

    public class YellowPaint : PaintMetaball
    {
        public override Color EdgeColor => Color.Yellow * 0.7f;
    }

    public abstract class PaintMetaball : Metaball
    {
        public class PaintParticle
        {
            public float Size;

            public Vector2 Center;

            public PaintParticle(Vector2 center, float size)
            {
                Center = center;
                Size = size;
            }

            public void Update()
            {
                Size = MathHelper.Clamp(Size - 0.1f, 0f, 200f) * 0.997f;
                if (Size < 20f)
                    Size = Size * 0.8f - 1f;
            }
        }

        // This list is personalized between the various subclasses.
        public List<PaintParticle> Particles
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

        public void SpawnParticle(Vector2 position, float size) =>
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
            foreach (PaintParticle particle in Particles)
            {
                Vector2 drawPosition = particle.Center - Main.screenPosition;
                Vector2 origin = tex.Size() * 0.5f;
                Vector2 scale = Vector2.One * particle.Size / tex.Size();
                Color paintColor = Color.Lerp(EdgeColor, new Color(0f, 0f, 1f), Utils.GetLerpValue(50f, 100f, particle.Size, true) * 0.1f) * 1.2f;

                Main.spriteBatch.Draw(tex, drawPosition, null, paintColor, 0f, origin, scale, 0, 0f);
            }
        }
    }
}
