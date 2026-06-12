using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class FeatherParticle : Particle
    {
        public float RotationSpeed;

        public float Opacity;

        public float SwayAngleStrength;

        public bool AffectedByWind;

        public bool NoGravity;

        public bool ShouldCollideWithTiles;

        public bool DieWithTileCollision;

        private int DeathTimer;

        public static float WaveAngle => MathHelper.ToRadians(25f);

        public float WaveFrameState { get; private set; }

        public int SwayInterval { get; private set; }

        public bool CurrentlyCollidingWithTiles { get; private set; }

        public FeatherParticle(Vector2 position, Vector2 velocity, Color color, float scale, int lifetime, float? rotationSpeed = null, float swayAngleStrength = 1f, bool affectedByWind = false, bool noGravity = false, bool shouldCollideWithTiles = false, bool dieWithTileCollision = false)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Scale = scale;
            Lifetime = lifetime;
            RotationSpeed = rotationSpeed ?? Main.rand.NextFloat(120f, 150f) * Main.rand.NextBool().ToDirectionInt();
            SwayAngleStrength = swayAngleStrength;
            AffectedByWind = affectedByWind;
            NoGravity = noGravity;
            ShouldCollideWithTiles = shouldCollideWithTiles;
            DieWithTileCollision = dieWithTileCollision;

            Variant = Main.rand.Next(3);
            Opacity = 0f;
            SwayInterval = Main.rand.Next(75, 150);
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override string Texture => "CalamityMod/Particles/FeatherParticle";

        public override bool SetLifetime => false;

        public override bool UseCustomDraw => true;

        public override int FrameVariants => 3;

        public override void Update()
        {
            if (!CurrentlyCollidingWithTiles)
                Rotation += (MathHelper.Pi / RotationSpeed) + (Velocity.X * 0.01f);

            // Original math from AstralGodRay.cs.
            int waveSign = (WaveFrameState > 0f).ToDirectionInt();
            if (Math.Abs(WaveFrameState) < 1f)
            {
                int dirToUse = WaveFrameState == 0 ? (Main.rand.NextBool() ? -1 : 1) : waveSign;
                waveSign = -dirToUse;
                WaveFrameState = dirToUse * SwayInterval * 0.5f;
            }

            // Switch directions whenever necessary.
            else if (Math.Abs(WaveFrameState) > SwayInterval)
                WaveFrameState = -waveSign;
            else
                WaveFrameState += waveSign;

            // Rotate velocity towards the wave angle to appear as if its swaying side to side.
            float swayAngle = WaveAngle / (SwayInterval * 0.5f);
            Velocity = Velocity.RotatedBy(waveSign * swayAngle * SwayAngleStrength);

            Velocity.X *= 0.98f;
            if (Velocity.Y < 0.6f && !NoGravity)
                Velocity.Y += 0.005f;
            else
                Velocity.Y *= 0.98f;

            // Move with wind speed.
            if (AffectedByWind)
                Velocity.X += Main.windSpeedCurrent / 3f;

            // Tile collision.
            if (ShouldCollideWithTiles)
            {
                CurrentlyCollidingWithTiles = false;

                Vector2 hitboxDimensions = new(12f, 12f);
                if (Collision.SolidCollision(Position, (int)hitboxDimensions.X, (int)hitboxDimensions.Y))
                {
                    Velocity = Collision.TileCollision(Position, Velocity, (int)hitboxDimensions.X, (int)hitboxDimensions.Y);
                    CurrentlyCollidingWithTiles = true;
                }

                if (Collision.WetCollision(Position, (int)hitboxDimensions.X, (int)hitboxDimensions.Y))
                {
                    Velocity.Y *= 0.8f;
                    CurrentlyCollidingWithTiles = true;
                }
            }

            if (DieWithTileCollision && ShouldCollideWithTiles)
            {
                if (CurrentlyCollidingWithTiles)
                {
                    Opacity = MathHelper.Lerp(1f, 0f, (float)(DeathTimer / 45));
                    if (DeathTimer >= 45)
                    {
                        Kill();
                        return;
                    }

                    DeathTimer++;
                }
                else
                {
                    Opacity = MathHelper.Clamp(Opacity + 0.075f, 0f, 1f);
                }
            }
            else
            {
                int fadeInTime = 45 - (int)(Lifetime * 0.2f);
                if (fadeInTime < 20)
                    fadeInTime = 20;
                int fadeOutTime = (int)(Lifetime * 0.8f);
                Opacity = Utils.GetLerpValue(0, fadeInTime, Time, true) * Utils.GetLerpValue(Lifetime, fadeOutTime, Time);

                if (Time >= Lifetime)
                {
                    Kill();
                    return;
                }
            }
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D texture = GeneralParticleHandler.GetTexture(Type);
            Color lightColor = Color.MultiplyRGB(Lighting.GetColor((Position / 16).ToPoint()));
            Rectangle frame = texture.Frame(1, FrameVariants, 0, Variant);
            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, lightColor * Opacity, Rotation, frame.Size() * 0.5f, Scale, SpriteEffects.None, 0f);
        }
    }
}
