using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityMod.Particles
{
    public class GraveyardMistParticle : Particle
    {
        public float StretchFactor;

        public float BaseOpacity;

        public int Direction;

        public float Opacity { get; private set; }

        public int TextureIndex { get; private set; }

        public GraveyardMistParticle(Vector2 spawnPosition, Vector2 velocity, Color color, float scale, float baseOpacity, int lifetime, int textureIndex)
        {
            Position = spawnPosition;
            Velocity = velocity;
            Color = color;
            Scale = scale;
            BaseOpacity = baseOpacity;
            Lifetime = lifetime;
            TextureIndex = textureIndex;
            StretchFactor = Main.rand.NextFloat(1f, 2f);
        }

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override bool SetLifetime => true;

        public override bool UseCustomDraw => true;

        public override void Update()
        {
            float halfLifetime = Lifetime / 2;
            Opacity = BaseOpacity * MathHelper.Lerp(0f, 1f, MathF.Sin(MathHelper.TwoPi * (Time / halfLifetime)));
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D mistTexture = Main.Assets.Request<Texture2D>("Images/Gore_" + TextureIndex).Value;
            SpriteEffects spriteEffects = (Velocity.X > 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 scale = new Vector2(StretchFactor, 1f) * Scale;
            spriteBatch.Draw(mistTexture, Position - Main.screenPosition, mistTexture.Frame(), Color * Opacity * 0.375f, Rotation, mistTexture.Frame().Size() * 0.5f, scale, spriteEffects, 0f);
        }
    }
}
