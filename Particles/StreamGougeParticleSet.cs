using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class StreamGougeParticleSet : BaseFusableParticleSet
    {
        public override float BorderSize => 3f;
        public override bool BorderShouldBeSolid => false;
        public override Color BorderColor => Color.Lerp(Color.DarkBlue, Color.Black, 0.75f) * 0.85f;
        public override FusableParticleRenderLayer RenderLayer => FusableParticleRenderLayer.OverNPCsBeforeProjectiles;

        public override List<Effect> BackgroundShaders => new List<Effect>()
        {
            GameShaders.Misc["CalamityMod:BaseFusableParticleEdge"].Shader,
        };
        public override List<Texture2D> BackgroundTextures => new List<Texture2D>()
        {
            ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/ParticleBackgrounds/StreamGougeLayer"),
        };
        public override FusableParticle SpawnParticle(Vector2 center, float sizeStrength)
        {
            Particles.Add(new FusableParticle(center, sizeStrength));
            return Particles.Last();
        }

        public override void UpdateBehavior(FusableParticle particle)
        {
            particle.Size = MathHelper.Clamp(particle.Size - 1f, 0f, 200f) * 0.98f;
        }

        public override void PrepareOptionalShaderData(Effect effect, int index)
        {
            switch (index)
            {
                // Background.
                case 0:
                    Vector2 offset = Vector2.UnitX * Main.GlobalTime * 0.045f;
                    effect.Parameters["generalBackgroundOffset"].SetValue(offset);
                    effect.Parameters["upscaleFactor"].SetValue(Vector2.One * -0.3f);
                    break;
            }
        }

        public override void DrawParticles()
        {
            Texture2D fusableParticleBase = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/FusableParticleBase");
            foreach (FusableParticle particle in Particles)
            {
                Vector2 drawPosition = particle.Center - Main.screenPosition;
                Vector2 origin = fusableParticleBase.Size() * 0.5f;
                Vector2 scale = Vector2.One * particle.Size / fusableParticleBase.Size();
                Main.spriteBatch.Draw(fusableParticleBase, drawPosition, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}
