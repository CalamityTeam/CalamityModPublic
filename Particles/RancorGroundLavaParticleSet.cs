using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class RancorGroundLavaParticleSet : BaseFusableParticleSet
    {
        public override float BorderSize => 18f;
        public override bool BorderShouldBeSolid => false;
        public override Color BorderColor => Color.Lerp(Color.Yellow, Color.Red, 0.85f) * 0.85f;
        public override FusableParticleRenderLayer RenderLayer => FusableParticleRenderLayer.OverWater;

        public override List<Effect> BackgroundShaders => new List<Effect>()
        {
            GameShaders.Misc["CalamityMod:AdditiveFusableParticleEdge"].Shader,
        };
        public override List<Texture2D> BackgroundTextures => new List<Texture2D>()
        {
            ModContent.Request<Texture2D>("CalamityMod/Projectiles/InvisibleProj"),
        };
        public override FusableParticle SpawnParticle(Vector2 center, float sizeStrength)
        {
            Particles.Add(new FusableParticle(center, sizeStrength));
            return Particles.Last();
        }

        public override void UpdateBehavior(FusableParticle particle)
        {
            particle.Size = MathHelper.Clamp(particle.Size - 0.15f, 0f, 200f) * 0.997f;
            if (particle.Size < 20f)
                particle.Size = particle.Size * 0.95f - 0.9f;
        }

        public override void DrawParticles()
        {
            Texture2D fusableParticleBase = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/FusableParticleBase");
            foreach (FusableParticle particle in Particles)
            {
                Vector2 drawPosition = particle.Center - Main.screenPosition;
                Vector2 origin = fusableParticleBase.Size() * 0.5f;
                Vector2 scale = Vector2.One * particle.Size / fusableParticleBase.Size() * new Vector2(1f, 0.5f);
                Color drawColor = Color.Lerp(BorderColor, new Color(0f, 0f, 1f), Utils.InverseLerp(120f, 135f, particle.Size, true) * 0.1f) * 1.4f;
                Main.spriteBatch.Draw(fusableParticleBase, drawPosition, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}
