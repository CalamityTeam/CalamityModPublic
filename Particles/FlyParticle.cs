using System;
using CalamityMod.Systems.Graphic.TempParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityMod.Particles
{
    public class FlyParticle : Particle
    {
        public float Opacity;

        private bool WasSpawnedWithAnchor;

        public Entity AnchorEntity { get; private set; }

        public Vector2 StoredPosition { get; private set; }

        public float HeightToNeverGoBelow { get; private set; }

        public FlyParticle(Vector2 position, float scale, int lifetime, Entity anchorEntity = null)
        {
            Position = position;
            Scale = scale;
            Lifetime = lifetime;
            AnchorEntity = anchorEntity;

            if (anchorEntity == null)
            {
                StoredPosition = position;
                HeightToNeverGoBelow = position.Y + 8f;
                WasSpawnedWithAnchor = false;
            }
            else
            {
                WasSpawnedWithAnchor = true;
            }

            Variant = Main.rand.Next(2);
            Color = Color.White;
            AffectedByLight = true;
        }

        public override string Texture => "Terraria/Images/Extra_262";

        public override bool SetLifetime => true;

        public override bool UseCustomDraw => true;

        public override int FrameVariants => 2;

        public override void Update()
        {
            if (WasSpawnedWithAnchor)
            {
                StoredPosition = AnchorEntity.Top;
                HeightToNeverGoBelow = AnchorEntity.Top.Y + 8f;
                if (!AnchorEntity.active || AnchorEntity == null)
                {
                    Kill();
                    return;
                }
            }

            Velocity += new Vector2(MathF.Sign(StoredPosition.X - Position.X) * 0.02f, MathF.Sign(StoredPosition.Y - Position.Y) * 0.02f);
            if (Time % 30 == 0 && Main.rand.NextBool(2))
            {
                Velocity = Main.rand.NextVector2Circular(1f, 1f);
                if (Main.rand.NextBool(2))
                    Velocity /= 2;
            }

            if (Position.Y > HeightToNeverGoBelow)
            {
                Position.Y = HeightToNeverGoBelow;
                if (Velocity.Y > 0f)
                    Velocity *= -1f;
            }

            Variant = Time % 6 / 3;
            int fadeInTime = (int)(Lifetime * 0.13f);
            int fadeOutTime = (int)(Lifetime * 0.93f);
            Opacity = Utils.GetLerpValue(0f, fadeInTime, Time, true) * Utils.GetLerpValue(Lifetime, Lifetime - fadeOutTime, Time, true);
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Color lightColor = Color.MultiplyRGB(Lighting.GetColor((Position / 16).ToPoint()));
            Rectangle frame = GeneralParticleHandler.GetTexture(Type).Frame(1, FrameVariants, 0, Variant);
            SpriteEffects effects = (Velocity.X > 0f) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.Draw(GeneralParticleHandler.GetTexture(Type), Position - Main.screenPosition, frame, lightColor * Opacity, Rotation, frame.Size() * 0.5f, Scale, effects, 0f);
        }
    }
}
