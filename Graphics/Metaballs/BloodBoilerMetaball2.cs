using System.Collections.Generic;
using System.Linq;
using CalamityMod.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Graphics.Metaballs
{
    public class BloodBoilerMetaball2 : Metaball
    {
        public class BloodBoilerParticle2
        {
            public Vector2 Center;
            public float Size;

            public BloodBoilerParticle2(Vector2 center, float size)
            {
                Center = center;
                Size = size;
            }

            public void Update()
            {
                // Always slowly shrink the particles.
                Size = MathHelper.Clamp(Size - 0.1f, 0f, 200f) * 0.91f;

                // Once sufficiently small, the particles very rapidly shrink.
                if (Size < 20f)
                    Size = Size * 0.8f - 1f;
            }
        }

        public static List<BloodBoilerParticle2> Particles
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

        public override Color EdgeColor => Color.DarkRed;

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

        public static void SpawnParticle(Vector2 position, float size) => Particles.Add(new(position, size));

        public override void DrawInstances()
        {
            float pureRedIntensity = 0.15f;
            float opacity = 0.3f;
            Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/SmallGreyscaleCircle").Value;

            foreach (BloodBoilerParticle2 particle in Particles)
            {
                Vector2 drawPosition = particle.Center - Main.screenPosition;
                var origin = tex.Size() * 0.5f;
                Vector2 scale = Vector2.One * particle.Size / tex.Size();

                float pureRedInterpolant = Utils.GetLerpValue(25f, 60f, particle.Size, true) * pureRedIntensity;
                Color drawColor = Color.Lerp(EdgeColor, Color.DarkRed, pureRedInterpolant).MultiplyRGBA(new Color(1f, 1f, 1f, opacity));

                Main.spriteBatch.Draw(tex, drawPosition, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}
