using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace CalamityMod.Particles
{
    public class HealingPlus : Particle
    {
        public override string Texture => "CalamityMod/Particles/HealingPlus";
        public override bool UseAdditiveBlend => true;
        public override bool UseCustomDraw => true;
        public override bool SetLifetime => true;

        public Player Owner;
        public Color StartColor;
        public Color EndColor;
        public Vector2 OverridePosition;
        public float Opacity;

        public HealingPlus(Vector2 position, float scale, Color colorStart, Color colorEnd, int lifetime)
        {
            Position = position;
            Scale = scale;
            Velocity.X = 0;
            Velocity.Y = Main.rand.NextFloat(-2f, -5f);
            Rotation = 0;
            StartColor = colorStart;
            EndColor = colorEnd;
            Color = colorStart;
            Lifetime = lifetime;
        }

        public override void Update()
        {
            Color = Color.Lerp(StartColor, EndColor, LifetimeCompletion);
            Lighting.AddLight(Position, Color.ToVector3() * 0.2f);
            Opacity -= 0.05f;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 Size = new Vector2 (Scale);

            Vector2 origin = new Vector2(tex.Width, tex.Height);

            Vector2 PositionAdjust = new Vector2(-16, -40);

            spriteBatch.Draw(tex, Position - Main.screenPosition - PositionAdjust, null, Color, Rotation, origin, Size, SpriteEffects.None, 0);

        }
    }
}
