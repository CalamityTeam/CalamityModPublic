using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class SquishyLightParticle : Particle
    {
        public override string Texture => "CalamityMod/Particles/Light";
        public override bool UseAdditiveBlend => true;
        public override bool UseCustomDraw => true;
        public override bool SetLifetime => true;


        public float Opacity;
        public float SquishStrenght;
        public float MaxSquish;
        public float HueShift;

        public SquishyLightParticle(Vector2 position, Vector2 velocity, float scale, Color color, int lifetime, float opacity = 1f, float squishStrenght = 1f, float maxSquish = 3f, float hueShift = 0f)
        {
            Position = position;
            Velocity = velocity;
            Scale = scale;
            Color = color;
            Opacity = opacity;
            Rotation = 0;
            Lifetime = lifetime;
            SquishStrenght = squishStrenght;
            MaxSquish = maxSquish;
            HueShift = hueShift;
        }

        public override void Update()
        {
            Velocity *= (LifetimeCompletion >= 0.34f) ? 0.93f : 1.02f;

            Opacity = LifetimeCompletion > 0.5f ? (float)Math.Sin(LifetimeCompletion * MathHelper.Pi) * 0.2f + 0.8f : (float)Math.Sin(LifetimeCompletion * MathHelper.Pi);
            Scale *= 0.95f;

            Color = Main.hslToRgb(Main.rgbToHsl(Color).X + HueShift, Main.rgbToHsl(Color).Y, Main.rgbToHsl(Color).Z);
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Particles/Light").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;

            float squish = MathHelper.Clamp(Velocity.Length() / 10f * SquishStrenght, 1f, MaxSquish);

            float rot = Velocity.ToRotation() + MathHelper.PiOver2;
            Vector2 origin = tex.Size() / 2f;
            Vector2 scale = new Vector2(Scale - Scale * squish * 0.3f, Scale * squish);
            float properBloomSize = (float)tex.Height / (float)bloomTex.Height;

            Vector2 drawPosition = Position - Main.screenPosition;

            Main.spriteBatch.Draw(bloomTex, drawPosition, null, Color * Opacity * 0.8f, rot, bloomTex.Size() / 2f, scale * 2 * properBloomSize, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(tex, drawPosition, null, Color * Opacity * 0.8f, rot, origin, scale * 1.1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(tex, drawPosition, null, Color.White * Opacity * 0.9f, rot, origin, scale, SpriteEffects.None, 0f);

        }
    }
}
