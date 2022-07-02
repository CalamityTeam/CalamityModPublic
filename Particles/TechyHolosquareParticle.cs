using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Particles
{
    public class TechyHoloysquareParticle : Particle
    {
        public override bool SetLifetime => true;
        public override string Texture => "CalamityMod/Particles/TechyHolosquare";
        public override bool UseAdditiveBlend => true;
        public override bool UseCustomDraw => true;

        internal Rectangle Frame;
        public float Opacity;

        public TechyHoloysquareParticle(Vector2 position, Vector2 speed, float scale, Color color, float opacity = 1f)
        {
            Position = position;
            Scale = scale;
            Color = color;
            Velocity = speed;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Opacity = opacity;
            Variant = Main.rand.Next(6);
            Lifetime = 25;

            switch (Variant)
            {
                case 0:
                    Frame = new Rectangle(8, 0, 6, 6);
                    break;
                case 1:
                    Frame = new Rectangle(6, 8, 10, 6);
                    break;
                case 2:
                    Frame = new Rectangle(4, 16, 14, 8);
                    break;
                case 3:
                    Frame = new Rectangle(2, 26, 18, 10);
                    break;
                case 4:
                    Frame = new Rectangle(2, 38, 18, 8);
                    break;
                case 5:
                    Frame = new Rectangle(6, 48, 12, 12);
                    break;
            }
        }

        public override void Update()
        {
            Opacity = (float)Math.Pow(LifetimeCompletion, 0.5f);
            Lighting.AddLight(Position, Color.ToVector3() * Opacity);
            Rotation = Velocity.ToRotation();

            Velocity *= 0.875f;
            Scale *= 0.96f;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D baseTex = GeneralParticleHandler.GetTexture(Type);

            CalamityUtils.DrawChromaticAberration(Vector2.UnitX.RotatedBy(Rotation), 1.5f, delegate (Vector2 offset, Color colorMod)
            {
                spriteBatch.Draw(baseTex, Position + offset - Main.screenPosition, Frame, Color.MultiplyRGB(colorMod) * Opacity, Rotation, Frame.Size() / 2, Scale / 2f, SpriteEffects.None, 0);
            });
        }
    }
}
