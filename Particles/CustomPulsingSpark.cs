using System;
using CalamityMod.Systems.Graphic.PixelationSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class CustomPulsingSpark : Particle
    {
        public Color InitialColor;
        public Color BackColor;
        public bool AffectedByGravity;
        public bool FadeIn = false;
        public float FadeInScale = 0f;
        public bool GlowCenter = false;
        public float GlowCenterScale = 1;
        public float GlowOpacity = 1;
        public Texture2D NewTexture;
        public Texture2D NewTextureBack;
        public float ExtraRotation;
        public Vector2 Stretch = new Vector2(0.5f, 1.6f);
        public float ShrinkSpeed = 0;
        public bool FlipHorizontal = false;
        public bool NoShrink = false;
        public float Spin = 0;
        public float ColorFadeSpeed = 0;
        public int PulseRate;
        public int randTimeAdd = 0;

        public float SineRate = 0;
        public float SineIntensity = 0;
        public float Sine = 0;
        public float SineRotation = 0;
        public float TurnRate = 0;
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public bool AltVisual = true;
        public override bool UseAdditiveBlend => AltVisual;
        public override bool SetLifetime => true;
        public override bool UseCustomDraw => true;

        public CustomPulsingSpark(Vector2 relativePosition, Vector2 velocity, string frontTexture, string backTexture, bool affectedByGravity, int lifetime, float scale, Color color, Color backColor, Vector2 stretch, bool useAddativeBlend = true, bool glowCenter = false, int pulseRate = 10, float turnRate = 0, float sineRate = 1, float sineIntensity = 0, float sineRotation = 0, float extraRotation = 0, bool fadeIn = false, bool affectedByLight = false, float shrinkSpeed = 0, float glowCenterScale = 1, float glowOpacity = 1, bool flipHorizontal = false, bool noShrink = false, float spin = 0, float colorFadeSpeed = 3)
        {
            Position = relativePosition;
            Velocity = velocity;
            NewTexture ??= ModContent.Request<Texture2D>(frontTexture).Value;
            NewTextureBack ??= ModContent.Request<Texture2D>(backTexture).Value;
            ExtraRotation = extraRotation;
            AffectedByGravity = affectedByGravity;
            AffectedByLight = affectedByLight;
            Scale = scale;
            Stretch = stretch;
            FadeInScale = scale;
            Lifetime = lifetime;
            Color = InitialColor = color;
            BackColor = backColor;
            ShrinkSpeed = shrinkSpeed;
            PulseRate = pulseRate;

            AltVisual = useAddativeBlend;
            GlowCenter = glowCenter;
            GlowCenterScale = glowCenterScale;
            GlowOpacity = glowOpacity;
            FlipHorizontal = flipHorizontal;
            NoShrink = noShrink;

            FadeIn = fadeIn;
            randTimeAdd = Main.rand.Next(0, 200 + 1);
            SineRate = sineRate;
            SineIntensity = sineIntensity;
            SineRotation = sineRotation;
            TurnRate = turnRate;

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

            Sine = (float)Math.Sin(((Time + randTimeAdd) * SineRate) / MathHelper.Pi) * SineIntensity;
            if (TurnRate != 0)
                Velocity = Velocity.RotatedBy(TurnRate);

            Stretch.X *= (1 - 0.2f * ShrinkSpeed);
            Stretch.Y *= (1 + 0.2f * ShrinkSpeed);
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            float pulseScale = 1 + Utils.Remap(Time % PulseRate, 0, PulseRate, 0, 0.85f);
            Vector2 adjustedSquash = new Vector2(Math.Max(Stretch.X - 0.4f, 0.1f), Math.Max(Stretch.Y + 0.4f, 0.1f));
            Vector2 scale = adjustedSquash * Scale * pulseScale;

            float scaleMult = 1;
            Color col = Color;
            Color backCol = BackColor;

            Vector2 drawPos = Position + Vector2.UnitX.RotatedBy(SineRotation) * Sine;
            if (AffectedByLight)
            {
                col = Lighting.GetColor((drawPos / 16).ToPoint()).MultiplyRGB(Color);
                backCol = Lighting.GetColor((drawPos / 16).ToPoint()).MultiplyRGB(BackColor);
            }

            if (Pixelate)
            {
                PixelationManager.AddPixelatedDrawer((_) =>
                {
                    float fadeOut = (1 - (float)Math.Pow(LifetimeCompletion, ColorFadeSpeed));
                    spriteBatch.Draw(NewTexture, drawPos - Main.screenPosition, null, col * fadeOut, Rotation, NewTexture.Size() * 0.5f, scale * scaleMult, FlipHorizontal ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                    if (GlowCenter)
                        spriteBatch.Draw(NewTexture, drawPos - Main.screenPosition, null, Color.Lerp(col, Color.White, 0.8f) * fadeOut * GlowOpacity, Rotation, NewTexture.Size() * 0.5f, scale * 0.8f * GlowCenterScale * scaleMult, FlipHorizontal ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                    scaleMult = (MathHelper.Lerp(NewTexture.Size().X / NewTextureBack.Size().X, NewTexture.Size().Y / NewTextureBack.Size().Y, 0.5f));
                    spriteBatch.Draw(NewTextureBack, drawPos - Main.screenPosition, null, backCol * fadeOut * 0.7f, Rotation, NewTextureBack.Size() * 0.5f, scale * scaleMult * 1.45f, FlipHorizontal ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                }, Enums.GeneralDrawLayer.AfterProjectiles, UseAdditiveBlend ? BlendState.Additive : null);
            }
            else
            {
                float fadeOut = (1 - (float)Math.Pow(LifetimeCompletion, ColorFadeSpeed));
                spriteBatch.Draw(NewTexture, drawPos - Main.screenPosition, null, col * fadeOut, Rotation, NewTexture.Size() * 0.5f, scale * scaleMult, FlipHorizontal ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                if (GlowCenter)
                    spriteBatch.Draw(NewTexture, drawPos - Main.screenPosition, null, Color.Lerp(col, Color.White, 0.8f) * fadeOut * GlowOpacity, Rotation, NewTexture.Size() * 0.5f, scale * 0.8f * GlowCenterScale * scaleMult, FlipHorizontal ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                scaleMult = (MathHelper.Lerp(NewTexture.Size().X / NewTextureBack.Size().X, NewTexture.Size().Y / NewTextureBack.Size().Y, 0.5f));
                spriteBatch.Draw(NewTextureBack, drawPos - Main.screenPosition, null, backCol * fadeOut * 0.7f, Rotation, NewTextureBack.Size() * 0.5f, scale * scaleMult * 1.45f, FlipHorizontal ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
        }
    }
}
