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
        public int CryogenIndex = -1;
        public float FadeInCountdown = 0f;
        public float FadeoutTimer = 0f;
        public CryogenAurora[] Auroras;

        public static bool ShouldDrawRegularly;

        internal static Texture2D AuroraTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/AuroraTexture");

        public static void UpdateDrawEligibility()
        {
            bool useEffect = NPC.AnyNPCs(ModContent.NPCType<Cryogen>()) || ShouldDrawRegularly;

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
            for (int i = 0; i < Auroras.Length; i++)
            {
                Vector2 centerRange = new Vector2(Main.screenWidth * 0.5f, Main.screenHeight * 0.1f / Auroras[i].Depth);
                Auroras[i].Center = centerRange * (Auroras[i].CenterOffsetRatio * MathHelper.TwoPi).ToRotationVector2() + Vector2.UnitX * Main.screenWidth * 0.5f;
                Auroras[i].Center.Y -= 180f + (float)Math.Cos(Main.GlobalTime * 1.2f + Auroras[i].CenterOffsetRatio * MathHelper.Pi) * 50f;
                Auroras[i].CenterOffsetRatio += 1f / 1800f * MathHelper.Lerp(0.2f, 1f, auroraStrength) * (float)Math.Sin(Main.GlobalTime * 0.9f);
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
                float auroraStrength = CalculateAuroraStrength();
                for (int i = 0; i < Auroras.Length; i++)
                {
                    float hue = 0.5f + Auroras[i].ColorHueOffset * 0.5f * MathHelper.Lerp(0.15f, 0.95f, auroraStrength);
                    hue += (float)Math.Cos(Main.GlobalTime * 1.2f) * 0.25f;
                    hue %= 1f;
                    float scale = 1.4f / Auroras[i].Depth;
                    scale += (float)Math.Cos(Main.GlobalTime * 0.7f + Auroras[i].CenterOffsetRatio * MathHelper.TwoPi) * 0.2f;

                    Color auroraColor = Main.hslToRgb(hue, 1f, 0.825f) * 0.85f;
                    auroraColor *= MathHelper.Lerp(0.3f, 1f, auroraStrength);

                    if (FadeInCountdown > 0f)
                        auroraColor *= Utils.InverseLerp(45f, 0f, FadeInCountdown);
                    if (FadeoutTimer > 0f)
                        auroraColor *= Utils.InverseLerp(0f, 45f, FadeoutTimer);
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
            FadeInCountdown = 45f;
            Auroras = new CryogenAurora[150];
            for (int i = 0; i < Auroras.Length; i++)
            {
                Auroras[i].Depth = Main.rand.NextFloat(1f, 2.2f);
                Auroras[i].ColorHueOffset = Main.rand.NextFloat(-1f, 1f);
                Auroras[i].DirectionEffect = Utils.SelectRandom(Main.rand, SpriteEffects.None, SpriteEffects.FlipHorizontally);
                Auroras[i].CenterOffsetRatio = i / (float)Auroras.Length + Main.rand.NextFloat(3f / Auroras.Length);
            }
        }

        public override void Reset() { }

        public override void Deactivate(params object[] args) { }

        public override bool IsActive() => (CryogenIndex != -1 || FadeoutTimer > 0 || FadeInCountdown > 0) && !Main.gameMenu;
    }
}
