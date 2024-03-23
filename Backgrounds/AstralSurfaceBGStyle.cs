using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Reflection;
using System;
using Terraria.Graphics.Effects;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Backgrounds
{
    public class AstralSurfaceBGStyle : ModSurfaceBackgroundStyle
    {
        readonly int FrontBGYOffset = 275;
        readonly int CloseBGYOffset = 175;
        readonly int MiddleBGYOffset = 475;

        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            for (int i = 0; i < fades.Length; i++)
            {
                if (i == Slot)
                {
                    fades[i] += transitionSpeed;
                    if (fades[i] > 1f)
                    {
                        fades[i] = 1f;
                    }
                }
                else
                {
                    fades[i] -= transitionSpeed;
                    if (fades[i] < 0f)
                    {
                        fades[i] = 0f;
                    }
                }
            }
        }

        public override int ChooseFarTexture()
        {
            return BackgroundTextureLoader.GetBackgroundSlot("CalamityMod/Backgrounds/AstralSurfaceHorizon");
        }

        public override int ChooseMiddleTexture()
        {
            return BackgroundTextureLoader.GetBackgroundSlot("CalamityMod/Backgrounds/AstralSurfaceFar");
        }

        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
        {
            return BackgroundTextureLoader.GetBackgroundSlot("CalamityMod/Skies/AstralSurfaceMiddle");
        }

        public override bool PreDrawCloseBackground(SpriteBatch spriteBatch)
        {
            float screenOff = (float)typeof(Main).GetField("screenOff", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Main.instance);
            float scAdj = (float)typeof(Main).GetField("scAdj", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Main.instance);
            Color COSBMAplha = (Color)typeof(Main).GetField("ColorOfSurfaceBackgroundsModified", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            Color ColorOfSurfaceBackgroundsModified = new Color(63, 51, 90, COSBMAplha.A);

            bool flag = false;
            if ((!Main.remixWorld || (Main.gameMenu && !WorldGen.remixWorldGen)) && (!WorldGen.remixWorldGen || !WorldGen.drunkWorldGen))
            {
                flag = true;
            }
            if (Main.mapFullscreen)
            {
                flag = false;
            }
            int num = 30;
            if (Main.gameMenu)
            {
                num = 0;
            }
            if (WorldGen.drunkWorldGen)
            {
                num = -180;
            }
            float num12 = (float)Main.worldSurface;
            if (num12 == 0f)
            {
                num12 = 1f;
            }
            float num17 = Main.screenPosition.Y + (float)(Main.screenHeight / 2) - 600f;
            double backgroundTopMagicNumber = (0f - num17 + screenOff / 2f) / (num12 * 16f);
            float bgGlobalScaleMultiplier = 2f;
            int pushBGTopHack;
            int num3 = -180;
            bool flag2 = true;
            int num4 = 0;
            if (Main.gameMenu)
            {
                num4 -= num3;
            }
            pushBGTopHack = num4;
            pushBGTopHack += num;
            if (flag2)
            {
                pushBGTopHack += num3;
            }
            if (flag)
            {
                var bgScale = 1.25f;
                var bgParallax = 0.4;
                var bgTopY = (int)(backgroundTopMagicNumber * 1800.0 + 1500.0) + (int)scAdj + pushBGTopHack;
                bgScale *= bgGlobalScaleMultiplier;
                var bgWidthScaled = (int)((float)CalamityMod.AstralSurfaceMiddle.Width * bgScale);
                SkyManager.Instance.DrawToDepth(Main.spriteBatch, 1.2f / (float)bgParallax);
                var bgStartX = (int)(0.0 - Math.IEEERemainder((double)Main.screenPosition.X * bgParallax, bgWidthScaled) - (double)(bgWidthScaled / 2));
                if (Main.gameMenu)
                    bgTopY = 320 + pushBGTopHack;

                var bgLoops = Main.screenWidth / bgWidthScaled + 2;
                if ((double)Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0)
                {
                    for (int i = 0; i < bgLoops; i++)
                    {
                        Main.spriteBatch.Draw(CalamityMod.AstralSurfaceMiddle, new Vector2(bgStartX + bgWidthScaled * i, bgTopY + MiddleBGYOffset), new Rectangle(0, 0, CalamityMod.AstralSurfaceMiddle.Width, CalamityMod.AstralSurfaceMiddle.Height), ColorOfSurfaceBackgroundsModified, 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(CalamityMod.AstralSurfaceMiddleGlow, new Vector2(bgStartX + bgWidthScaled * i, bgTopY + MiddleBGYOffset), new Rectangle(0, 0, CalamityMod.AstralSurfaceMiddleGlow.Width, CalamityMod.AstralSurfaceMiddleGlow.Height), new Color(Color.White.R * 0.5f, Color.White.G * 0.5f, Color.White.B * 0.5f, COSBMAplha.A), 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
                    }
                }

                bgScale = 1.31f;
                bgParallax = 0.43;
                bgTopY = (int)(backgroundTopMagicNumber * 1950.0 + 1750.0) + (int)scAdj + pushBGTopHack;
                bgScale *= bgGlobalScaleMultiplier;
                bgWidthScaled = (int)((float)CalamityMod.AstralSurfaceClose.Width * bgScale);
                SkyManager.Instance.DrawToDepth(Main.spriteBatch, 1f / (float)bgParallax);
                bgStartX = (int)(0.0 - Math.IEEERemainder((double)Main.screenPosition.X * bgParallax, bgWidthScaled) - (double)(bgWidthScaled / 2));
                if (Main.gameMenu)
                {
                    bgTopY = 400 + pushBGTopHack;
                    bgStartX -= 80;
                }

                bgLoops = Main.screenWidth / bgWidthScaled + 2;
                if ((double)Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0)
                {
                    for (int i = 0; i < bgLoops; i++)
                    {
                        Main.spriteBatch.Draw(CalamityMod.AstralSurfaceClose, new Vector2(bgStartX + bgWidthScaled * i, bgTopY + CloseBGYOffset), new Rectangle(0, 0, CalamityMod.AstralSurfaceClose.Width, CalamityMod.AstralSurfaceClose.Height), ColorOfSurfaceBackgroundsModified, 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(CalamityMod.AstralSurfaceCloseGlow, new Vector2(bgStartX + bgWidthScaled * i, bgTopY + CloseBGYOffset), new Rectangle(0, 0, CalamityMod.AstralSurfaceCloseGlow.Width, CalamityMod.AstralSurfaceCloseGlow.Height), new Color(Color.White.R * 0.7f, Color.White.G * 0.7f, Color.White.B * 0.7f, COSBMAplha.A), 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
                    }
                }
                bgScale = 1.34f;
                bgParallax = 0.49;
                bgTopY = (int)(backgroundTopMagicNumber * 2100.0 + 2000.0) + (int)scAdj + pushBGTopHack;
                bgScale *= bgGlobalScaleMultiplier;
                bgWidthScaled = (int)(CalamityMod.AstralSurfaceFront.Width * bgScale);
                SkyManager.Instance.DrawToDepth(Main.spriteBatch, 1f / (float)bgParallax);
                bgStartX = (int)(0.0 - Math.IEEERemainder((double)Main.screenPosition.X * bgParallax, bgWidthScaled) - (double)(bgWidthScaled / 2));
                if (Main.gameMenu)
                {
                    bgTopY = 480 + pushBGTopHack;
                    bgStartX -= 120;
                }

                bgLoops = Main.screenWidth / bgWidthScaled + 2;
                if ((double)Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0)
                {
                    for (int i = 0; i < bgLoops; i++)
                    {
                        Main.spriteBatch.Draw(CalamityMod.AstralSurfaceFront, new Vector2(bgStartX + bgWidthScaled * i, bgTopY + FrontBGYOffset), new Rectangle(0, 0, CalamityMod.AstralSurfaceFront.Width, CalamityMod.AstralSurfaceFront.Height), ColorOfSurfaceBackgroundsModified, 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(CalamityMod.AstralSurfaceFrontGlow, new Vector2(bgStartX + bgWidthScaled * i, bgTopY + FrontBGYOffset), new Rectangle(0, 0, CalamityMod.AstralSurfaceFrontGlow.Width, CalamityMod.AstralSurfaceFrontGlow.Height), new Color(Color.White.R * 0.9f, Color.White.G * 0.9f, Color.White.B * 0.9f, COSBMAplha.A), 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
                    }
                }
            }
            return false;
        }
    }
}
