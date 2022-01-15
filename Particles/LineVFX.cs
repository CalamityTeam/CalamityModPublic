using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class LineVFX : Particle
    {
        public override string Texture => "CalamityMod/Particles/ThinEndedLine";
        public override bool UseAdditiveBlend => true;
        public override bool UseCustomDraw => true;

        public override bool SetLifetime => true;
        public float opacity;
        public Vector2 LineVector;
        public bool Concave;

        public LineVFX(Vector2 startPoint, Vector2 lineVector, float thickness, Color color, bool concave = false)
        {
            Position = startPoint;
            LineVector = lineVector;
            Scale = thickness;
            Color = color;
            Concave = concave;
            Velocity = Vector2.Zero;
            Rotation = 0;
            Lifetime = 2;
        }

        public override void CustomDraw(SpriteBatch spriteBatch) 
        {
            Texture2D tex;
            if (Concave)
                tex = ModContent.GetTexture("CalamityMod/Particles/ThickEndedLine"); //THICC :wearty: weary: :weary : oogogg;... ooog... that line be lookin' mighty fine... :hot:
            else
                tex = GeneralParticleHandler.GetTexture(Type);

            float rot = LineVector.ToRotation() + MathHelper.PiOver2;
            Vector2 origin = new Vector2(tex.Width/2f, tex.Height);
            Vector2 scale = new Vector2(Scale, LineVector.Length() / tex.Height);

            spriteBatch.Draw(tex, Position - Main.screenPosition, null, Color, rot, origin, scale, SpriteEffects.None, 0);
        }
    }
}
