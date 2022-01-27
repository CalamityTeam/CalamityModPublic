using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class ChargeUpLineVFX : Particle
    {
        public override string Texture => "CalamityMod/Particles/HalfLine";
        public override bool UseAdditiveBlend => true;
        public override bool UseCustomDraw => true;
        public override bool SetLifetime => true;
        public override bool Important => Telegraph;

        public float BaseOpacity;
        public float Opacity;
        public float LineDirection;
        public float LineLenght = 0f;
        private Vector2 PrevOffset = Vector2.Zero;
        public bool Telegraph; //Denotes if the line is used as an enemy telegraph. In that case, it'll be marked as important
        public float FullFadeInPoint;
        public float MinDistanceFromOrigin;

        public ChargeUpLineVFX(Vector2 startPoint, float lineDirection, float thickness, Color color, int lifetime, float opacity = 1f, bool telegraph = true, float fullFadeInPoint = 0.5f, float minDistanceFromOrigin = 8f)
        {
            RelativeOffset = startPoint;
            LineDirection = lineDirection + MathHelper.Pi;
            Scale = thickness;
            Color = color;
            BaseOpacity = opacity;
            Telegraph = telegraph;
            FullFadeInPoint = fullFadeInPoint;
            MinDistanceFromOrigin = minDistanceFromOrigin;
            Velocity = Vector2.Zero;
            Rotation = 0;
            Lifetime = lifetime;
        }

        public override void Update()
        {
            Opacity = Time / (float)Lifetime > FullFadeInPoint ? BaseOpacity : (float)Math.Sin(Time / (FullFadeInPoint * Lifetime) * MathHelper.PiOver2) * BaseOpacity;

            PrevOffset = Vector2.Lerp(PrevOffset, RelativeOffset, 0.1f);
            RelativeOffset *= 0.8f;
            LineLenght = (PrevOffset - RelativeOffset).Length();

            if (RelativeOffset.Length() < MinDistanceFromOrigin)
                RelativeOffset = LineDirection.ToRotationVector2() * MinDistanceFromOrigin;
        }

        public override void CustomDraw(SpriteBatch spriteBatch, Vector2 basePosition) 
        {
            Texture2D tex = GeneralParticleHandler.GetTexture(Type);
            Texture2D bloomTex = ModContent.GetTexture("CalamityMod/Particles/Light");

            float rot = LineDirection + MathHelper.PiOver2;
            Vector2 origin = new Vector2(tex.Width / 2f, tex.Height);
            Vector2 scale = new Vector2(Scale, LineLenght / tex.Height);

            Vector2 drawPosition = basePosition - Main.screenPosition + RelativeOffset;

            Main.spriteBatch.Draw(tex, drawPosition, null, Color * Opacity * 0.8f, rot, origin, scale * 1.1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(tex, drawPosition, null, Color.White * Opacity, rot, origin, scale, SpriteEffects.None, 0f);


            Main.spriteBatch.Draw(bloomTex, drawPosition, null, Color * Opacity * 0.8f, 0f, bloomTex.Size() / 2f, Scale * 1.1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(bloomTex, drawPosition, null, Color.White * Opacity, 0f, bloomTex.Size() / 2f, Scale, SpriteEffects.None, 0f);

        }
    }
}
