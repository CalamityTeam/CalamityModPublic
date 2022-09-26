using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class WulfrumDroidEmote : Particle
    {
        public override string Texture => "CalamityMod/Particles/WulfrumDroidEmotes";
        public override bool UseCustomDraw => true;
        public override bool SetLifetime => true;

        public Rectangle Frame;

        public WulfrumDroidEmote(Vector2 position, Vector2 velocity, int lifeTime, float scale = 1f, int variant = -1)
        {
            Position = position;
            Velocity = velocity;
            Color = Color.White;
            Scale = scale;
            Lifetime = lifeTime;
            Rotation = velocity.ToRotation() + MathHelper.PiOver2;

            if (variant == -1)
                variant = Main.rand.Next(15);

            Frame = new Rectangle(16 * (variant % 8), 16 * (variant / 8), 16, 16);
        }

        public override void Update()
        {
            Velocity *= 0.96f;
            Scale *= 0.97f;

            Color lightColor = Frame.Y > 0 ? new Color(112, 244, 244) : new Color(194, 255, 62);
            Lighting.AddLight(Position, lightColor.ToVector3());
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D emoteTexture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = new Vector2(Frame.Width / 2f, Frame.Height);
            float opacity = 1 - (float)Math.Pow(LifetimeCompletion, 4f);

            spriteBatch.Draw(emoteTexture, Position - Main.screenPosition, Frame, Color * opacity, Rotation, origin, Scale, SpriteEffects.None, 0);
        }
    }
}
