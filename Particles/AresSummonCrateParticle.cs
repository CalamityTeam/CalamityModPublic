using Microsoft.Xna.Framework;
using Terraria;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Particles
{
    public class AresSummonCrateParticle : Particle
    {
        public override bool SetLifetime => true;
        public override string Texture => "CalamityMod/Particles/AresSummonCrate";
        public override bool UseCustomDraw => true;

        public AresSummonCrateParticle(Player owner, Vector2 velocity, int lifetime)
        {
            Position = owner.Center - Vector2.UnitY * 4f;
            Scale = 1f;
            Color = Color.White;
            Velocity = velocity;
            Rotation = 0f;
            Lifetime = lifetime;
        }

        public override void Update()
        {
            Rotation += Math.Sign(Velocity.X) * 0.05f;
            Velocity.X *= 0.95f;
            Velocity.Y += 0.24f;
            if (Collision.SolidCollision(Position - Vector2.One * Scale * 19f, (int)(Scale * 38f), (int)(Scale * 38f), true))
            {
                Velocity.X *= 0.8f;
                Rotation = 0f;
                Velocity.Y = 0f;
            }
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D texture = GeneralParticleHandler.GetTexture(Type);
            Color lightColor = Lighting.GetColor(Position.ToTileCoordinates());

            int frameY = (int)Math.Round(MathHelper.Lerp(0f, 2f, (float)Math.Pow(LifetimeCompletion, 0.16f)));
            Rectangle frame = texture.Frame(1, 3, 0, frameY);
            float opacity = MathHelper.Clamp(1f - (float)Math.Pow(LifetimeCompletion, 7f), 0f, 1f);
            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, lightColor * opacity, Rotation, frame.Size() * 0.5f, Scale, 0, 0);
        }
    }
}




