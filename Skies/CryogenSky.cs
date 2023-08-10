using CalamityMod.NPCs.Cryogen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
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

        public int CryogenIndex = -1;
        public float FadeInCountdown = 0f;
        public float FadeoutTimer = 0f;
        public CryogenAurora[] Auroras;

        public static bool ShouldDrawRegularly;

        public static void UpdateDrawEligibility()
        {
            bool useEffect = (NPC.AnyNPCs(ModContent.NPCType<Cryogen>()) || ShouldDrawRegularly) && !Main.gameMenu;

            if (SkyManager.Instance["CalamityMod:Cryogen"] != null && useEffect != SkyManager.Instance["CalamityMod:Cryogen"].IsActive())
            {
                if (useEffect)
                    SkyManager.Instance.Activate("CalamityMod:Cryogen");
                else
                    SkyManager.Instance.Deactivate("CalamityMod:Cryogen");
            }

            if (ShouldDrawRegularly)
                ShouldDrawRegularly = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (FadeInCountdown > 0)
                FadeInCountdown--;

            if (CryogenIndex == -1 && !ShouldDrawRegularly)
            {
                UpdateCryogenIndex();
                if (FadeoutTimer == 0)
                    FadeoutTimer = 45f;
                FadeoutTimer--;
                if (FadeoutTimer <= 0f)
                    Deactivate();
            }

            float auroraStrength = CalculateAuroraStrength();
            float width = Main.screenWidth * 0.5f;
            float height = Main.screenHeight * 0.1f;
            Vector2 centerOffset = Vector2.UnitX * Main.screenWidth * 0.5f;
            float time = Main.GlobalTimeWrappedHourly * 1.2f;
            float centerOffsetRatioIncrement = 1f / 1800f * MathHelper.Lerp(0.2f, 1f, auroraStrength) * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.9f);
            for (int i = 0; i < Auroras.Length; i++)
            {
                Vector2 centerRange = new Vector2(width, height / Auroras[i].Depth);
                Auroras[i].Center = centerRange * (Auroras[i].CenterOffsetRatio * MathHelper.TwoPi).ToRotationVector2() + centerOffset;
                Auroras[i].Center.Y -= 180f + (float)Math.Cos(time + Auroras[i].CenterOffsetRatio * MathHelper.Pi) * 50f;
                Auroras[i].CenterOffsetRatio += centerOffsetRatioIncrement;
            }
        }

        public float GetCryogenLifeRatio()
        {
            if (UpdateCryogenIndex())
            {
                float lifeRatio = 0f;
                if (CryogenIndex != -1)
                    lifeRatio = Main.npc[CryogenIndex].life / (float)Main.npc[CryogenIndex].lifeMax;

                return lifeRatio;
            }
            return 0f;
        }

        public float CalculateAuroraStrength()
        {
            if (ShouldDrawRegularly)
                return 1f;

            return 1f - GetCryogenLifeRatio();
        }

        public bool UpdateCryogenIndex()
        {
            int cryogenType = ModContent.NPCType<Cryogen>();
            if (CryogenIndex >= 0 && Main.npc[CryogenIndex].active && Main.npc[CryogenIndex].type == cryogenType)
                return true;

            CryogenIndex = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == cryogenType)
                {
                    CryogenIndex = i;
                    break;
                }
            }

            return CryogenIndex != -1;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= float.MaxValue && minDepth < float.MaxValue)
            {
                var auroraTexture = CalamityUtils.AuroraTexture;
                float auroraStrength = CalculateAuroraStrength();
                float hueOffset = 0.5f * MathHelper.Lerp(0.15f, 0.95f, auroraStrength);
                float time = (float)Math.Cos(Main.GlobalTimeWrappedHourly * 1.2f) * 0.25f;
                float time2 = Main.GlobalTimeWrappedHourly * 0.7f;
                float auroraColorLerp = MathHelper.Lerp(0.3f, 1f, auroraStrength);
                float fadeInLerp = Utils.GetLerpValue(45f, 0f, FadeInCountdown);
                float fadeOutLerp = Utils.GetLerpValue(0f, 45f, FadeoutTimer);
                float brightnessLerp = MathHelper.Lerp(0.6f, 1.1f, (float)Math.Sin(Main.GlobalTimeWrappedHourly / 1.8f) * 0.5f + 0.5f);
                Vector2 origin = auroraTexture.Size() * 0.5f;
                for (int i = 0; i < Auroras.Length; i++)
                {
                    float hue = 0.5f + Auroras[i].ColorHueOffset * hueOffset;
                    hue += time;
                    hue %= 1f;
                    float scale = 1.4f / Auroras[i].Depth;
                    scale += (float)Math.Cos(time2 + Auroras[i].CenterOffsetRatio * MathHelper.TwoPi) * 0.2f;

                    Color auroraColor = Main.hslToRgb(hue, 1f, 0.825f) * 0.85f;
                    auroraColor *= auroraColorLerp;

                    if (FadeInCountdown > 0f)
                        auroraColor *= fadeInLerp;
                    if (FadeoutTimer > 0f)
                        auroraColor *= fadeOutLerp;
                    if (Main.dayTime)
                        auroraColor *= 0.4f;

                    float yBrightness = MathHelper.Lerp(1.5f, 0.5f, 1f - MathHelper.Clamp((Auroras[i].Center.Y + 300f) / 200f, 0f, 1f)) * 1.3f;
                    yBrightness *= brightnessLerp;
                    if (yBrightness > 1.3f)
                        yBrightness = 1.3f;
                    auroraColor *= yBrightness;

                    spriteBatch.Draw(auroraTexture, Auroras[i].Center, null, auroraColor * 1.1f, MathHelper.PiOver2, origin, scale, Auroras[i].DirectionEffect, 0f);
                }
            }
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            FadeInCountdown = 45f;
            Auroras = new CryogenAurora[150];
            float randomOffsetMax = 3f / Auroras.Length;
            for (int i = 0; i < Auroras.Length; i++)
            {
                Auroras[i].Depth = Main.rand.NextFloat(1f, 2.2f);
                Auroras[i].ColorHueOffset = Main.rand.NextFloat(-1f, 1f);
                Auroras[i].DirectionEffect = Utils.SelectRandom(Main.rand, SpriteEffects.None, SpriteEffects.FlipHorizontally);
                Auroras[i].CenterOffsetRatio = i / (float)Auroras.Length + Main.rand.NextFloat(randomOffsetMax);
            }
        }

        public override void Reset() { }

        public override void Deactivate(params object[] args) { }

        public override bool IsActive() => (CryogenIndex != -1 || FadeoutTimer > 0 || FadeInCountdown > 0) && !Main.gameMenu;
    }
}
