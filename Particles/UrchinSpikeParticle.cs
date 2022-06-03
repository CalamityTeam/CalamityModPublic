using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class UrchinSpikeParticle : Particle
    {
        public override string Texture => "CalamityMod/Particles/UrchinSpikes";
        public override bool UseCustomDraw => true;
        public override bool SetLifetime => true;
        public override int FrameVariants => 6;

        public float Opacity;

        public UrchinSpikeParticle(Vector2 position, Vector2 speed, float rotation, float scale = 1f, float opacity = 1f, int lifetime = 20)
        {
            Position = position;
            Scale = scale;
            Color = Color.White;
            Opacity = opacity;
            Velocity = speed;
            Rotation = rotation;
            Lifetime = lifetime;
            Variant = Main.rand.Next(6);
        }

        public override void Update()
        {
            Color = Lighting.GetColor((int)Position.X / 16, (int)Position.Y / 16) * Opacity;
            Velocity *= 0.9f;
            if (Velocity.Length() <= 0.01f)
                GeneralParticleHandler.RemoveParticle(this);
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D texture = GeneralParticleHandler.GetTexture(Type);
            Rectangle frame = new Rectangle(8 * Variant, 0, 6, 10);
            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, Color, Rotation, frame.Size() * 0.5f, Scale, SpriteEffects.None, 0f);
        }
    }
}
