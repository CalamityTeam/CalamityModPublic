using System.Collections.Generic;
using System.Linq;
using CalamityMod.Effects;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Graphics.Metaballs
{
    public class PhotoMetaball4 : Metaball
    {
        public class PhotoParticle4
        {
            public Vector2 Center;
            public float Size;
            public int Time = 0;

            public PhotoParticle4(Vector2 center, float size)
            {
                Center = center;
                Size = size;
            }

            public void Update()
            {
                Time++;
                if (Time >= 6)
                    Size = 2;
            }
        }

        public static List<PhotoParticle4> Particles
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
        
        public Color sparkColor;
        public int Time;
        public override Color EdgeColor => sparkColor * 5;

        public override void Update()
        {
            Time++;
            // Update all particle instances.
            // Once sufficiently small, they vanish.
            for (int i = 0; i < Particles.Count; i++)
                Particles[i].Update();
            Particles.RemoveAll(p => p.Size <= 2f);

            if (Time % 8 == 0)
            {
                sparkColor = Main.rand.Next(4) switch
                {
                    0 => Color.Red,
                    1 => Color.MediumTurquoise,
                    2 => Color.Orange,
                    _ => Color.LawnGreen,
                };
            }

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
            float opacity = 1f;
            Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Graphics/Metaballs/MetaballMessy").Value;

            foreach (PhotoParticle4 particle in Particles)
            {
                Vector2 drawPosition = particle.Center - Main.screenPosition;
                var origin = tex.Size() * 0.5f;
                Vector2 scale = Vector2.One * particle.Size / tex.Size();

                float Interpolant = Utils.GetLerpValue(25f, 60f, particle.Size * 0.6f, true);
                Color drawColor = Color.Lerp(Color.White, EdgeColor, Interpolant).MultiplyRGBA(new Color(1f, 1f, 1f, opacity));

                Main.spriteBatch.Draw(tex, drawPosition, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}
