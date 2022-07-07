using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class WulfrumDroidSweatEmote : Particle
    {
        public override string Texture => "CalamityMod/Particles/WulfrumDroidSweatEmote";
        public override bool UseCustomDraw => true;
        public override bool SetLifetime => true;

        public WulfrumDroidSweatEmote(Vector2 position, Vector2 velocity, int lifeTime, float scale = 1f)
        {
            Position = position;
            Velocity = velocity;
            Color = Color.White;
            Scale = scale;
            Lifetime = lifeTime;
            Rotation = velocity.ToRotation() + MathHelper.PiOver2;

        }

        public override void Update()
        {
            Velocity *= 0.96f;
            Scale *= 0.97f;
            Velocity.Y += 0.06f;

            Lighting.AddLight(Position, new Color(194, 255, 62).ToVector3());
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D emoteTexture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = new Vector2(emoteTexture.Width / 2f, emoteTexture.Height / 2f);
            float opacity = 1 - (float)Math.Pow(LifetimeCompletion, 4f);

            SpriteEffects effect = Velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(emoteTexture, Position - Main.screenPosition, null, Color * opacity, Rotation, origin, Scale, effect, 0);
        }
    }
}
