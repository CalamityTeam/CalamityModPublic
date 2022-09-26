using Microsoft.Xna.Framework;
using Terraria;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Particles
{
    public class WulfrumHatParticle : Particle
    {
        public override bool SetLifetime => true;
        public override string Texture => "CalamityMod/Particles/WulfrumHat";
        public override bool UseCustomDraw => true;

        public int Direction;

        public WulfrumHatParticle(Player owner, Vector2 velocity, int lifetime)
        {
            Position = owner.Center - Vector2.UnitY * 20f;
            Direction = owner.direction;
            Scale = 1f;
            Color = Color.White;
            Velocity = velocity;
            Rotation = 0f;
            Lifetime = lifetime;
        }

        public override void Update()
        {
            Rotation += 0.05f * Math.Sign(Velocity.X);

            Velocity *= 0.95f;
            Velocity.Y += 0.22f;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D baseTex = GeneralParticleHandler.GetTexture(Type);
            Color lightColor = Lighting.GetColor(Position.ToTileCoordinates());

            SpriteEffects spriteEffect = SpriteEffects.None;
            if (Direction < 0)
            {
                spriteEffect = SpriteEffects.FlipHorizontally;
            }
                
            float opacity = Math.Clamp(1 - (float)Math.Pow(LifetimeCompletion , 3f), 0f, 1f);
            spriteBatch.Draw(baseTex, Position - Main.screenPosition, null, lightColor * opacity, Rotation, baseTex.Size()/2f, Scale, spriteEffect, 0);
        }
    }
}




