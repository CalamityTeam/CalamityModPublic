using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class GenericBloom : Particle
    {
        public override string Texture => "CalamityMod/Particles/Light";
        public bool UseAltVisual = true;
        public override bool UseAdditiveBlend => UseAltVisual;
        public override bool SetLifetime => true;

        private float opacity;
        private Color BaseColor;
        private bool ProduceLight;

        public GenericBloom(Vector2 position, Vector2 velocity, Color color, float scale, int lifeTime, bool produceLight = true, bool AddativeBlend = true)
        {
            Position = position;
            Velocity = velocity;
            BaseColor = color;
            Scale = scale;
            Lifetime = lifeTime;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            ProduceLight = produceLight;
            UseAltVisual = AddativeBlend;
        }

        public override void Update()
        {
            opacity = (float)Math.Sin(LifetimeCompletion * MathHelper.Pi);
            Color = BaseColor * opacity;
            if (ProduceLight)
            {
                Lighting.AddLight(Position, Color.R / 255f, Color.G / 255f, Color.B / 255f);
            }
            Velocity *= 0.95f;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(tex, Position - Main.screenPosition, null, Color * opacity, Rotation, tex.Size() / 2f, Scale, SpriteEffects.None, 0);
        }
    }
}
