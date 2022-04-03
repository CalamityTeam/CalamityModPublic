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
        public override bool Important => Telegraph;

        public float opacity;
        public Vector2 LineVector;
        public bool Concave;
        public bool Telegraph; //Denotes if the line is used as an enemy telegraph. In that case, it'll be marked as important
        public float Expansion;

        public LineVFX(Vector2 startPoint, Vector2 lineVector, float thickness, Color color, bool concave = false, bool telegraph = false, float expansion = 0f)
        {
            Position = startPoint;
            LineVector = lineVector;
            Scale = thickness;
            Color = color;
            Concave = concave;
            Telegraph = telegraph;
            Expansion = expansion;
            Velocity = Vector2.Zero;
            Rotation = 0;
            Lifetime = 2;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex;
            if (Concave)
                tex = ModContent.Request<Texture2D>("CalamityMod/Particles/ThickEndedLine"); //THICC :wearty: weary: :weary : oogogg;... ooog... that line be lookin' mighty fine... :hot:
            else
                tex = GeneralParticleHandler.Assets.Request<Texture2D>(Type).Value;

            Vector2 drawPosition = Position - Utils.SafeNormalize(LineVector, Vector2.Zero) * (float)Math.Sqrt(1f - (float)Math.Pow(LifetimeCompletion - 1f, 2)) * Expansion / 2f;
            Vector2 expandedLine = LineVector + Utils.SafeNormalize(LineVector, Vector2.Zero) * (float)Math.Sqrt(1f - (float)Math.Pow(LifetimeCompletion - 1f, 2)) * Expansion;

            float rot = LineVector.ToRotation() + MathHelper.PiOver2;
            Vector2 origin = new Vector2(tex.Width/2f, tex.Height);
            Vector2 scale = new Vector2(Scale, expandedLine.Length() / tex.Height);

            spriteBatch.Draw(tex, drawPosition - Main.screenPosition, null, Color, rot, origin, scale, SpriteEffects.None, 0);
        }
    }
}
