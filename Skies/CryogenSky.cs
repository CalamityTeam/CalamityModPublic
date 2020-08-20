using CalamityMod.NPCs.Cryogen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityMod.Skies
{
    public class CryogenSky : CustomSky
    {
        public struct CryogenAurora
        {
            public float Depth;
            public float ColorHueOffset;
            public float CenterOffsetRatio;
            public Vector2 Center;
            public SpriteEffects DirectionEffect;
        }
        private int cryogenIndex = -1;
        private float fadeIn = 0f;
        private float fadeOut = 0f;
        private CryogenAurora[] Auroras;

        internal const int TimePerRotation = 75;
        internal static Texture2D AuroraTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/AuroraTexture");

        public override void Update(GameTime gameTime)
        {
            if (fadeIn > 0)
                fadeIn--;

            if (cryogenIndex == -1)
            {
                UpdateCryogenIndex();
                if (fadeOut == 0)
                    fadeOut = 45f;
                fadeOut--;
                if (fadeOut <= 0f)
                    Deactivate();
            }
            for (int i = 0; i < Auroras.Length; i++)
            {
                Vector2 centerRange = new Vector2(Main.screenWidth * 0.5f, Main.screenHeight * 0.1f / Auroras[i].Depth);
                Auroras[i].Center = centerRange * (Auroras[i].CenterOffsetRatio * MathHelper.TwoPi).ToRotationVector2() + Vector2.UnitX * Main.screenWidth * 0.5f;
                Auroras[i].Center.Y -= 180f + (float)Math.Cos(Main.GlobalTime * 1.2f + Auroras[i].CenterOffsetRatio * MathHelper.Pi) * 50f;
                Auroras[i].CenterOffsetRatio += 1f / 1800f * MathHelper.Lerp(0.2f, 1f, 1f - GetCryogenLifeRatio()) * (float)Math.Sin(Main.GlobalTime * 0.9f);
            }
        }

        private float GetCryogenLifeRatio()
        {
            if (UpdateCryogenIndex())
            {
                float localIntensity = 0f;
                if (cryogenIndex != -1)
                {
                    localIntensity = Main.npc[cryogenIndex].life / (float)Main.npc[cryogenIndex].lifeMax;
                }
                return localIntensity;
            }
            return 0f;
        }

        private bool UpdateCryogenIndex()
        {
            int cryogenType = ModContent.NPCType<Cryogen>();
            if (cryogenIndex >= 0 && Main.npc[cryogenIndex].active && Main.npc[cryogenIndex].type == cryogenType)
            {
                return true;
            }
            cryogenIndex = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == cryogenType)
                {
                    cryogenIndex = i;
                    break;
                }
            }
            return cryogenIndex != -1;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= float.MaxValue && minDepth < float.MaxValue)
            {
                float drawPower = 1f - GetCryogenLifeRatio();
                for (int i = 0; i < Auroras.Length; i++)
                {
                    float hue = 0.5f + Auroras[i].ColorHueOffset * 0.5f * MathHelper.Lerp(0.15f, 0.95f, drawPower);
                    hue += (float)Math.Cos(Main.GlobalTime * 1.2f) * 0.25f;
                    hue %= 1f;
                    float scale = 1.4f / Auroras[i].Depth;
                    scale += (float)Math.Cos(Main.GlobalTime * 0.7f + Auroras[i].CenterOffsetRatio * MathHelper.TwoPi) * 0.2f;

                    Color auroraColor = Main.hslToRgb(hue, 1f, 0.825f) * 0.85f;
                    auroraColor *= MathHelper.Lerp(0.3f, 1f, drawPower);

                    if (fadeIn > 0f)
                        auroraColor *= Utils.InverseLerp(45f, 0f, fadeIn);
                    if (fadeOut > 0f)
                        auroraColor *= Utils.InverseLerp(0f, 45f, fadeOut);
                    if (Main.dayTime)
                        auroraColor *= 0.4f;

                    float yBrightness = MathHelper.Lerp(1.5f, 0.5f, 1f - MathHelper.Clamp((Auroras[i].Center.Y + 300f) / 200f, 0f, 1f)) * 1.3f;
                    yBrightness *= MathHelper.Lerp(0.6f, 1.1f, (float)Math.Sin(Main.GlobalTime / 1.8f) * 0.5f + 0.5f);
                    if (yBrightness > 1.3f)
                        yBrightness = 1.3f;
                    auroraColor *= yBrightness;

                    spriteBatch.Draw(AuroraTexture,
                                     Auroras[i].Center,
                                     null,
                                     auroraColor * 1.1f,
                                     MathHelper.PiOver2,
                                     AuroraTexture.Size() * 0.5f,
                                     scale,
                                     Auroras[i].DirectionEffect,
                                     0f);
                }
            }
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            fadeIn = 45f;
            Auroras = new CryogenAurora[150];
            for (int i = 0; i < Auroras.Length; i++)
            {
                Auroras[i].Depth = Main.rand.NextFloat(1f, 2.2f);
                Auroras[i].ColorHueOffset = Main.rand.NextFloat(-1f, 1f);
                Auroras[i].DirectionEffect = Utils.SelectRandom(Main.rand, SpriteEffects.None, SpriteEffects.FlipHorizontally);
                Auroras[i].CenterOffsetRatio = i / (float)Auroras.Length + Main.rand.NextFloat(3f / Auroras.Length);
            }
        }

        public override void Deactivate(params object[] args)
        {
        }

        public override void Reset() { } // a
        public override bool IsActive() => (cryogenIndex != -1 || fadeOut > 0 || fadeIn > 0) && !Main.gameMenu;
    }
}
