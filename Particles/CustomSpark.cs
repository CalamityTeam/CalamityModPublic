using System;
using CalamityMod.Systems.Graphic.PixelationSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class CustomSpark : Particle
    {
        public Color InitialColor;
        public bool AffectedByGravity;
        public bool FadeIn = false;
        public float FadeInScale = 0f;
        public bool GlowCenter = false;
        public float GlowCenterScale = 1;
        public float GlowOpacity = 1;
        public string NewTexture;
        public float ExtraRotation;
        public Vector2 Stretch = new Vector2(0.5f, 1.6f);
        public float ShrinkSpeed = 0;
        public bool FlipHorizontal = false;
        public bool NoShrink = false;
        public float Spin = 0;
        public float ColorFadeSpeed = 0;
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public bool AltVisual = true;
        public override bool UseAdditiveBlend => AltVisual;
        public override bool SetLifetime => true;
        public override bool UseCustomDraw => true;

        public CustomSpark(Vector2 relativePosition, Vector2 velocity, string texture, bool affectedByGravity, int lifetime, float scale, Color color, Vector2 stretch, bool useAddativeBlend = true, bool glowCenter = false, float extraRotation = 0, bool fadeIn = false, bool affectedByLight = false, float shrinkSpeed = 0, float glowCenterScale = 1, float glowOpacity = 1, bool flipHorizontal = false, bool noShrink = false, float spin = 0, float colorFadeSpeed = 3)
        {
            Position = relativePosition;
            Velocity = velocity;
            NewTexture = texture;
            ExtraRotation = extraRotation;
            AffectedByGravity = affectedByGravity;
            AffectedByLight = affectedByLight;
            Scale = scale;
            Stretch = stretch;
            FadeInScale = scale;
            Lifetime = lifetime;
            Color = InitialColor = color;
            ShrinkSpeed = shrinkSpeed;

            AltVisual = useAddativeBlend;
            GlowCenter = glowCenter;
            GlowCenterScale = glowCenterScale;
            GlowOpacity = glowOpacity;
            FlipHorizontal = flipHorizontal;
            NoShrink = noShrink;

            FadeIn = fadeIn;

            if (FadeIn)
                Scale = 0f;
            Spin = spin;
            ColorFadeSpeed = colorFadeSpeed;
        }

        public override void Update()
        {
            if (!FadeIn)
            {
                if (!NoShrink)
                    Scale *= 0.95f;
                Color = Color.Lerp(InitialColor, Color.Transparent, (float)Math.Pow(LifetimeCompletion, ColorFadeSpeed));
            }
            else
            {
                if ((float)Time / (float)Lifetime < 0.5f)
                {
                    Scale = MathHelper.Lerp(Scale, FadeInScale, 0.2f);
                }
                else
                {
                    Scale = MathHelper.Lerp(Scale, FadeInScale, -0.21f);
                }
            }
            Velocity *= 0.95f;
            if (Velocity.Length() < 12f && AffectedByGravity)
            {
                Velocity.X *= 0.94f;
                Velocity.Y += 0.25f;
            }
            ExtraRotation += Spin;
            Rotation = Velocity.ToRotation() + MathHelper.PiOver2 + ExtraRotation;

            Stretch.X *= (1 - 0.2f * ShrinkSpeed);
            Stretch.Y *= (1 + 0.2f * ShrinkSpeed);
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Vector2 scale = Stretch * Scale;
            Texture2D texture = ModContent.Request<Texture2D>(NewTexture).Value;

            float scaleMult = 1;
            if (Main.zenithWorld)
            {
                DateTime day = DateTime.Now;
                if (day.DayOfWeek == DayOfWeek.Tuesday)
                {
                    Texture2D joke = ModContent.Request<Texture2D>("CalamityMod/Particles/MammothParticle").Value;
                    scaleMult = (MathHelper.Lerp(texture.Size().X / joke.Size().X, texture.Size().Y / joke.Size().Y, 0.5f));
                    texture = joke;
                    AltVisual = true;
                }
            }

            Color col = Color;

            if (AffectedByLight)
            {
                col = Lighting.GetColor((Position / 16).ToPoint()).MultiplyRGB(Color);
            }

            if (Pixelate)
            {
                PixelationManager.AddPixelatedDrawer((_) =>
                {
                    spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color.Lerp(col, Color.Transparent, (float)Math.Pow(LifetimeCompletion, 3D)), Rotation, texture.Size() * 0.5f, scale * scaleMult, FlipHorizontal ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                    if (GlowCenter)
                        spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color.Lerp(Color.Lerp(col, Color.White, 0.8f), Color.Transparent, (float)Math.Pow(LifetimeCompletion, 3D)) * GlowOpacity, Rotation, texture.Size() * 0.5f, scale * 0.8f * GlowCenterScale * scaleMult, FlipHorizontal ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                }, Enums.GeneralDrawLayer.AfterProjectiles, UseAdditiveBlend ? BlendState.Additive : null);
            }
            else
            {
                spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color.Lerp(col, Color.Transparent, (float)Math.Pow(LifetimeCompletion, 3D)), Rotation, texture.Size() * 0.5f, scale * scaleMult, FlipHorizontal ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                if (GlowCenter)
                    spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color.Lerp(Color.Lerp(col, Color.White, 0.8f), Color.Transparent, (float)Math.Pow(LifetimeCompletion, 3D)) * GlowOpacity, Rotation, texture.Size() * 0.5f, scale * 0.8f * GlowCenterScale * scaleMult, FlipHorizontal ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
        }
    }
}
