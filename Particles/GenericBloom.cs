using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace CalamityMod.Particles
{
    public class GenericBloom : Particle
    {
        public override string Texture => "CalamityMod/Particles/Light";
        public override bool UseAdditiveBlend => true;
        public override bool SetLifetime => true;

        private float opacity;
        private Color BaseColor;

        public GenericBloom(Vector2 position, Vector2 velocity, Color color, float scale, int lifeTime)
        {
            Position = position;
            Velocity = velocity;
            BaseColor = color;
            Scale = scale;
            Lifetime = lifeTime;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override void Update()
        {
            opacity = (float)Math.Sin(LifetimeCompletion * MathHelper.Pi);
            Color = BaseColor * opacity;
            Lighting.AddLight(Position, Color.R / 255f, Color.G / 255f, Color.B / 255f);
            Velocity *= 0.95f;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = GeneralParticleHandler.GetTexture(Type);

            spriteBatch.Draw(tex, Position - Main.screenPosition, null, Color * opacity, Rotation, tex.Size() / 2f, Scale, SpriteEffects.None, 0);
        }
    }
}
