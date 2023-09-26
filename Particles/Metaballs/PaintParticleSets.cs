using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Particles.Metaballs
{
    public class CyanPaint : PaintParticleSet
    {
        public override Color BorderColor => Color.Cyan * 0.7f;
    }
    public class BluePaint : PaintParticleSet
    {
        public override Color BorderColor => Color.Blue * 0.7f;
    }
    public class MagentaPaint : PaintParticleSet
    {
        public override Color BorderColor => Color.Magenta * 0.7f;
    }
    public class LimePaint : PaintParticleSet
    {
        public override Color BorderColor => Color.Lime * 0.7f;
    }
    public class YellowPaint : PaintParticleSet
    {
        public override Color BorderColor => Color.Yellow * 0.7f;
    }

    public abstract class PaintParticleSet : BaseFusableParticleSet
    {
        public override float BorderSize => 12f;
        public override bool BorderShouldBeSolid => false;

        public override List<Effect> BackgroundShaders => new()
        {
            GameShaders.Misc["CalamityMod:AdditiveFusableParticleEdge"].Shader,
        };
        public override List<Texture2D> BackgroundTextures => new()
        {
            ModContent.Request<Texture2D>("CalamityMod/Projectiles/InvisibleProj").Value,
        };
        public override FusableParticle SpawnParticle(Vector2 center, float sizeStrength)
        {
            Particles.Add(new FusableParticle(center, sizeStrength));
            return Particles.Last();
        }

        public override void UpdateBehavior(FusableParticle particle)
        {
            particle.Size = MathHelper.Clamp(particle.Size - 0.1f, 0f, 200f) * 0.997f;
            if (particle.Size < 20f)
                particle.Size = particle.Size * 0.8f - 1f;
        }

        public override void DrawParticles()
        {
            Texture2D fusableParticleBase = ModContent.Request<Texture2D>("CalamityMod/Particles/Metaballs/FusableParticleBase").Value;
            foreach (FusableParticle particle in Particles)
            {
                Vector2 drawPosition = particle.Center - Main.screenPosition;
                Vector2 origin = fusableParticleBase.Size() * 0.5f;
                Vector2 scale = Vector2.One * particle.Size / fusableParticleBase.Size();
                Color drawColor = Color.Lerp(BorderColor, new Color(0f, 0f, 1f), Utils.GetLerpValue(50f, 100f, particle.Size, true) * 0.1f) * 1.2f;
                Main.spriteBatch.Draw(fusableParticleBase, drawPosition, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}
