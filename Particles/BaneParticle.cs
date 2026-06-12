using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class BaneParticle : Particle
    {
        public override string Texture => "CalamityMod/Particles/BaneParticle";
        public Color InitialColor;
        public Color EndColor;
        public Color glowColor = Color.White;
        public bool AffectedByGravity;
        public bool FadeIn = false;
        public float FadeInScale = 0f;
        public float SineRate = 0;
        public float SineIntensity = 0;
        public float Sine = 0;
        public int sineTime = 0;

        public string NewTexture;
        public float ActiveRotation;
        public float AddedRotation = 0;
        public float SpeedReduction = 0.95f;
        public float ExtraRotation = 0;

        public Vector2 Stretch = Vector2.One;
        public float ShrinkSpeed = 0;
        public bool AltVisual = false;
        public override bool UseAdditiveBlend => AltVisual;
        public override bool SetLifetime => true;
        public override bool UseCustomDraw => true;

        public override int FrameVariants => 5;

        public BaneParticle(Vector2 relativePosition, Vector2 velocity, bool affectedByGravity, int lifetime, float scale, Color color, Color colorEnd, Vector2 stretch, bool useAddativeBlend = false, float activeRotation = 0, float speedReduction = 0.95f, float extraRotation = 0, float sineRate = 1, float sineIntensity = 0, bool fadeIn = false, bool affectedByLight = false, float shrinkSpeed = 0)
        {
            Position = relativePosition;
            Velocity = velocity;
            ActiveRotation = activeRotation;
            AffectedByGravity = affectedByGravity;
            AffectedByLight = affectedByLight;
            Scale = scale;
            Stretch = stretch;
            FadeInScale = scale;
            Lifetime = lifetime;
            Color = InitialColor = color;
            EndColor = colorEnd;
            ShrinkSpeed = shrinkSpeed;
            SpeedReduction = speedReduction;
            ExtraRotation = extraRotation;
            SineRate = sineRate;
            SineIntensity = sineIntensity;
            sineTime = Main.rand.Next(0, 400 + 1);


            AltVisual = useAddativeBlend;

            FadeIn = fadeIn;

            if (FadeIn)
                Scale = 0f;

            Variant = Main.rand.Next(FrameVariants);
        }

        public override void Update()
        {
            if (!FadeIn)
            {
                Scale *= 0.95f;
                Color = Color.Lerp(InitialColor, EndColor, LifetimeCompletion);
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
            Velocity *= SpeedReduction;
            if (Velocity.Length() < 12f && AffectedByGravity)
            {
                Velocity.X *= 0.94f;
                Velocity.Y += 0.25f;
            }

            float velRot = ActiveRotation == 0 ? (Velocity.ToRotation() + MathHelper.PiOver2) : ActiveRotation;
            Rotation = velRot + AddedRotation;
            AddedRotation += ActiveRotation;
            ActiveRotation *= 0.975f;

            Stretch.X *= (1 - 0.2f * ShrinkSpeed);
            Stretch.Y *= (1 + 0.2f * ShrinkSpeed);

            Sine = (float)Math.Sin((sineTime * SineRate) / MathHelper.Pi) * SineIntensity;
            sineTime++;
        }
        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Particles/BaneParticleGlow").Value;
            int frameWidth = 18;
            int frameHeight = 18;
            int frameSpacing = frameHeight + 2;
            Rectangle frame = new Rectangle(0, frameSpacing * Variant, frameWidth, frameHeight);

            Texture2D texture2 = ModContent.Request<Texture2D>("CalamityMod/Particles/BaneParticle").Value;
            int frameWidth2 = 14;
            int frameHeight2 = 10;
            int frameSpacing2 = frameHeight2 + 2;
            Rectangle frame2 = new Rectangle(0, frameSpacing2 * Variant, frameWidth2, frameHeight2);

            Vector2 scale = Stretch * Scale;

            Color col = Color;

            if (AffectedByLight)
            {
                col = Lighting.GetColor((Position / 16).ToPoint()).MultiplyRGB(Color);
            }
            float fadeOut = (1 - (float)Math.Pow(LifetimeCompletion, 3D));
            spriteBatch.Draw(texture, Position - Main.screenPosition + Vector2.UnitX * Sine, frame, col * fadeOut, Rotation + ExtraRotation, frame.Size() * 0.5f, scale, 0, 0f);
            spriteBatch.Draw(texture2, Position - Main.screenPosition + Vector2.UnitX * Sine, frame2, Color.Lerp(glowColor, col, 0.2f) * 0.75f * fadeOut, Rotation + ExtraRotation, frame2.Size() * 0.5f, scale * 0.95f, 0, 0f);
        }
    }
}
