using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class BloodParticle : Particle
    {
        public Color InitialColor;
        public override bool SetLifetime => true;
        public override bool UseCustomDraw => true;
        public override bool UseAdditiveBlend => true;

        public override string Texture => "CalamityMod/Particles/Blood";

        public BloodParticle(Vector2 relativePosition, Vector2 velocity, int lifetime, float scale, Color color)
        {
            Position = relativePosition;
            Velocity = velocity;
            Scale = scale;
            Lifetime = lifetime;
            Color = InitialColor = color;
        }

        public override void Update()
        {
            Scale *= 0.98f;
            Velocity.X *= 0.97f;
            Velocity.Y = MathHelper.Clamp(Velocity.Y + 0.9f, -22f, 22f);
            Color = Color.Lerp(InitialColor, Color.Transparent, (float)Math.Pow(LifetimeCompletion, 3D));
            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            float verticalStretch = Utils.GetLerpValue(0f, 24f, Math.Abs(Velocity.Y), true) * 0.84f;
            float brightness = (float)Math.Pow(Lighting.Brightness((int)(Position.X / 16f), (int)(Position.Y / 16f)), 0.15);
            Vector2 scale = new Vector2(1f, verticalStretch + 1f) * Scale * 0.1f;
            Texture2D texture = ModContent.Request<Texture2D>(Texture);

            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * brightness, Rotation, texture.Size() * 0.5f, scale, 0, 0f);
        }
    }
}
