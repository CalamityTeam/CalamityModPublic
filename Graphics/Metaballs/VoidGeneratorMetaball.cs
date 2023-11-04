using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using static CalamityMod.Graphics.Metaballs.StreamGougeMetaball;

namespace CalamityMod.Graphics.Metaballs
{
    public class VoidGeneratorMetaball : Metaball
    {
        public static List<CosmicParticle> Particles
        {
            get;
            private set;
        } = new();

        public override bool AnythingToDraw => Particles.Any();

        public override IEnumerable<Texture2D> Layers
        {
            get
            {
                yield return LayerAsset.Value;
            }
        }

        public override MetaballDrawLayer DrawContext => MetaballDrawLayer.BeforeProjectiles;

        public override Color EdgeColor => Color.Lerp(Color.Purple, Color.Black, 0.75f);

        public override void Update()
        {
            // Update all particle instances.
            // Once sufficiently small, they vanish.
            for (int i = 0; i < Particles.Count; i++)
                Particles[i].Update();
            Particles.RemoveAll(p => p.Size <= 2f);
        }

        public static CosmicParticle SpawnParticle(Vector2 position, Vector2 velocity, float size)
        {
            CosmicParticle particle = new(position, velocity, size);
            Particles.Add(particle);

            return particle;
        }

        // Make the texture scroll.
        public override Vector2 CalculateManualOffsetForLayer(int layerIndex)
        {
            return Vector2.UnitX * Main.GlobalTimeWrappedHourly * 0.037f;
        }

        public override void DrawInstances()
        {
            Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/BasicCircle").Value;

            foreach (CosmicParticle particle in Particles)
            {
                Vector2 drawPosition = particle.Center - Main.screenPosition;
                Vector2 origin = tex.Size() * 0.5f;
                Vector2 scale = Vector2.One * particle.Size / tex.Size();
                Main.spriteBatch.Draw(tex, drawPosition, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}
