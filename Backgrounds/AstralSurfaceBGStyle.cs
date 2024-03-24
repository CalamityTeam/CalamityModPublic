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
        internal static readonly FieldInfo screenOffField = typeof(Main).GetField("screenOff", BindingFlags.Instance | BindingFlags.NonPublic);
        internal static readonly FieldInfo scAdjField = typeof(Main).GetField("scAdj", BindingFlags.Instance | BindingFlags.NonPublic);
        internal static readonly FieldInfo COSBMAplhaField = typeof(Main).GetField("ColorOfSurfaceBackgroundsModified", BindingFlags.Static | BindingFlags.NonPublic);
        readonly int FrontBGYOffset = 275; //Offsets for the height positioning. Make these 0 for backgrounds that want to be the same as treeline backgrounds (backgrounds that have the front 3 layers as trees)
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
                        fades[i] = 1f;
                }
                else
                {
                    fades[i] -= transitionSpeed;
                    if (fades[i] < 0f)
                        fades[i] = 0f;
                }
            }
        }

        public override int ChooseFarTexture() => BackgroundTextureLoader.GetBackgroundSlot("CalamityMod/Backgrounds/AstralSurfaceHorizon"); //5th layer (Horizon)

        public override int ChooseMiddleTexture() => BackgroundTextureLoader.GetBackgroundSlot("CalamityMod/Backgrounds/AstralSurfaceFar"); //4th layer (Far)

        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b) => BackgroundTextureLoader.GetBackgroundSlot("CalamityMod/Skies/AstralSurfaceMiddle"); //Fallback texture, not nessesarily needed since predraw returns false

        public override bool PreDrawCloseBackground(SpriteBatch spriteBatch) //Taken from vanilla to draw the front 3 layers, edited to fit tmod and astral background needs
        {
            float screenOff = (float)screenOffField.GetValue(Main.instance);
            float scAdj = (float)scAdjField.GetValue(Main.instance);
            Color COSBMAplha = (Color)COSBMAplhaField.GetValue(null);
            Color ColorOfSurfaceBackgroundsModified = new Color(63, 51, 90, COSBMAplha.A); //Astral Biome Coloration with the alpha as the background fade
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
                //3rd layer (Middle)
                var bgScale = 1.25f; //Scale of the furthest of the closest background layers (3rd layer)
                var bgParallax = 0.4; //The parallax of the background layer
                var bgTopY = (int)(backgroundTopMagicNumber * 1800.0 + 1500.0) + (int)scAdj + pushBGTopHack; //the Y position of the background
                bgScale *= bgGlobalScaleMultiplier; //Scale of the background
                var bgWidthScaled = (int)((float)CalamityMod.AstralSurfaceMiddle.Width * bgScale); //The Width of the bg texture scaled to the correct size
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
                        Main.spriteBatch.Draw(CalamityMod.AstralSurfaceMiddle, new Vector2(bgStartX + bgWidthScaled * i, bgTopY + MiddleBGYOffset), new Rectangle(0, 0, CalamityMod.AstralSurfaceMiddle.Width, CalamityMod.AstralSurfaceMiddle.Height), ColorOfSurfaceBackgroundsModified, 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(CalamityMod.AstralSurfaceMiddleGlow, new Vector2(bgStartX + bgWidthScaled * i, bgTopY + MiddleBGYOffset), new Rectangle(0, 0, CalamityMod.AstralSurfaceMiddleGlow.Width, CalamityMod.AstralSurfaceMiddleGlow.Height), new Color(Color.White.R * 0.5f, Color.White.G * 0.5f, Color.White.B * 0.5f, COSBMAplha.A), 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
                    }
                }

                //2nd layer (Close)
                //Repeat of above with slight changes to variables
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
                    bgStartX -= 80; //the initial postion of where the background starts in the main menu is pushed back to not be directly ontop, mostly for tree bgs that use the same treeline texture but faded
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

                //1st layer (Front)
                //Repeat of above with slight changes to variables
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
            return false; //Stop the drawing of the base close background texture (note: does NOT stop the drawing of the Middle and Far textures because tmodloader is slight stupid :blushing:)
        }
    }
}
