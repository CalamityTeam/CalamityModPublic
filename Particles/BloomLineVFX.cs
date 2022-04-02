using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class BloomLineVFX : Particle
    {
        public override string Texture => "CalamityMod/Particles/BloomLine";
        public override bool UseAdditiveBlend => true;
        public override bool UseCustomDraw => true;
        public override bool SetLifetime => true;
        public override bool Important => Telegraph;

        public bool Capped;
        public float opacity;
        public Vector2 LineVector;
        public bool Telegraph; //Denotes if the line is used as an enemy telegraph. In that case, it'll be marked as important

        public BloomLineVFX(Vector2 startPoint, Vector2 lineVector, float thickness, Color color, int lifetime, bool capped = false, bool telegraph = false)
        {
            Position = startPoint;
            LineVector = lineVector;
            Scale = thickness;
            Color = color;
            Lifetime = lifetime;
            Capped = capped;
            Telegraph = telegraph;
            Velocity = Vector2.Zero;
            Rotation = 0;
        }

        public override void CustomDraw(SpriteBatch spriteBatch) 
        {
            Texture2D tex = GeneralParticleHandler.GetTexture(Type);
            float rot = LineVector.ToRotation() + MathHelper.PiOver2;
            Vector2 origin = new Vector2(tex.Width/2f, tex.Height);
            Vector2 scale = new Vector2(Scale, LineVector.Length() / tex.Height);
            spriteBatch.Draw(tex, Position - Main.screenPosition, null, Color, rot, origin, scale, SpriteEffects.None, 0);

            if (Capped)
            {
                Texture2D cap = ModContent.GetTexture("CalamityMod/Particles/BloomLineCap"); ;
                scale = new Vector2(Scale, Scale);
                origin = new Vector2(cap.Width / 2f, cap.Height);

                spriteBatch.Draw(cap, Position - Main.screenPosition, null, Color, rot + MathHelper.Pi, origin, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(cap, Position + LineVector - Main.screenPosition, null, Color, rot, origin, scale, SpriteEffects.None, 0);
            }

            
        }
    }
}
