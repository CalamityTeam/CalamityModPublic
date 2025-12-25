using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityMod.Backgrounds
{
    public class SulphurSeaSurfaceBGStyle : ModSurfaceBackgroundStyle
    {
        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            //This just fades in the background and fades out other backgrounds.
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

        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
        {
            return BackgroundTextureLoader.GetBackgroundSlot("CalamityMod/Backgrounds/SulphurSeaSurfaceClose");
        }

        public override bool PreDrawCloseBackground(SpriteBatch spriteBatch) //Taken from astral bg so the sulphur sea bg doesn't overlap other backgrounds
        {
            float screenOff = (float)AstralSurfaceBGStyle.screenOffField.GetValue(Main.instance);
            float scAdj = (float)AstralSurfaceBGStyle.scAdjField.GetValue(Main.instance);
            Color ColorOfSurfaceBackgroundsModified = (Color)AstralSurfaceBGStyle.COSBMAplhaField.GetValue(null);
            bool canBGDraw = false;
            if ((!Main.remixWorld || (Main.gameMenu && !WorldGen.remixWorldGen)) && (!WorldGen.remixWorldGen || !WorldGen.drunkWorldGen))
                canBGDraw = true;
            if (Main.mapFullscreen)
                canBGDraw = false;
            int offset = 30;
            if (Main.gameMenu)
                offset = 0;
            if (WorldGen.drunkWorldGen)
                offset = -180;
            float surfacePosition = (float)Main.worldSurface;
            if (surfacePosition == 0f)
                surfacePosition = 1f;
            float screenPosition = Main.screenPosition.Y + (float)(Main.screenHeight / 2) - 600f;
            double backgroundTopMagicNumber = (0f - screenPosition + screenOff / 2f) / (surfacePosition * 16f);
            float bgGlobalScaleMultiplier = 2f;
            int pushBGTopHack;
            int offset2 = -180;
            int menuOffset = 0;
            if (Main.gameMenu)
            {
                menuOffset -= offset2;
            }
            pushBGTopHack = menuOffset;
            pushBGTopHack += offset;
            pushBGTopHack += offset2; //Offsets to the background placement
            if (canBGDraw) //If the background can draw (player is not in a remix world or is not in a map).  This can probably be removed since backgrounds already go through these checks
            {
                var bgScale = 1.25f; //Scale of the furthest of the closest background layers
                var bgParallax = 0.4; //The parallax of the background layer
                var bgTopY = (int)(backgroundTopMagicNumber * 1800.0 + 1500.0) + (int)scAdj + pushBGTopHack; //the Y position of the background
                bgScale *= bgGlobalScaleMultiplier; //Scale of the background
                var bgWidthScaled = (int)((float)CalamityMod.SulphurSeaSurface.Width * bgScale); //The Width of the bg texture scaled to the correct size
                SkyManager.Instance.DrawToDepth(Main.spriteBatch, 1.2f / (float)bgParallax);
                var bgStartX = (int)(0.0 - Math.IEEERemainder((double)Main.screenPosition.X * bgParallax, bgWidthScaled) - (double)(bgWidthScaled / 2)); //The starting position of the background layer
                if (Main.gameMenu)
                    bgTopY = 320 + pushBGTopHack; //increases the height in the main menu

                var bgLoops = Main.screenWidth / bgWidthScaled + 2;
                if ((double)Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0)
                {
                    for (int i = 0; i < bgLoops; i++)
                    {
                        //Draw the texture and its glowmask to each loop of the texture
                        Main.spriteBatch.Draw(CalamityMod.SulphurSeaSurface, new Vector2(bgStartX + bgWidthScaled * i, bgTopY + 400), new Rectangle(0, 0, CalamityMod.SulphurSeaSurface.Width, CalamityMod.SulphurSeaSurface.Height), ColorOfSurfaceBackgroundsModified, 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(CalamityMod.SulphurSeaSkyFront, new Vector2(bgStartX + bgWidthScaled * i, bgTopY + 400), new Rectangle(0, 0, CalamityMod.SulphurSeaSkyFront.Width, CalamityMod.SulphurSeaSkyFront.Height), new Color(Color.LightSeaGreen.R, Color.LightSeaGreen.G, Color.LightSeaGreen.B, ColorOfSurfaceBackgroundsModified.A) * 0.5f, 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
                    }
                }
            }
            return false; //Stop the drawing of the base close background texture (note: does NOT stop the drawing of the Middle and Far textures because tmodloader is slight stupid :blushing:)
        }
    }
}
