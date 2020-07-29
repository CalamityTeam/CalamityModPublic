using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
    public class DraedonLogSunkenSeaGUI : PopupGUI
    {
        public int Page = 0;
        public int ArrowClickCooldown;
        public bool HoveringOverBook = false;
        public int TotalLinesPerPage => 10;
        public const int TotalPages = 2;
        public const int TextStartOffsetX = 40;
        public override void Update()
        {
            if (Active)
            {
                if (FadeTime < FadeTimeMax)
                    FadeTime++;
            }
            else if (FadeTime > 0)
            {
                FadeTime--;
            }

            if (Main.mouseLeft && !HoveringOverBook && FadeTime >= 30)
            {
                Page = 0;
                Active = false;
            }

            if (ArrowClickCooldown > 0)
                ArrowClickCooldown--;
            HoveringOverBook = false;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D pageTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/DraedonsLogPage");
            float xScale = MathHelper.Lerp(0.004f, 1f, FadeTime / (float)FadeTimeMax);
            Vector2 scale = new Vector2(xScale, 1f) * new Vector2(Main.screenWidth, Main.screenHeight) / pageTexture.Size();
            scale.Y *= 1.5f;
            scale *= 0.5f;

            float xResolutionScale = Main.screenWidth / 2560f;
            float yResolutionScale = Main.screenHeight / 1440f;
            float bookScale = 0.75f;
            scale *= bookScale;

            float yPageTop = MathHelper.Lerp(Main.screenHeight * 2, Main.screenHeight * 0.25f, FadeTime / (float)FadeTimeMax);

            Rectangle mouseRectangle = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 2, 2);
            for (int i = 0; i < 2; i++)
            {
                Vector2 drawPosition = new Vector2(Main.screenWidth * 0.5f, yPageTop);
                spriteBatch.Draw(pageTexture,
                                 drawPosition,
                                 null,
                                 Color.White,
                                 0f,
                                 new Vector2(i == 0f ? pageTexture.Width : 0f, 0f),
                                 scale,
                                 i == 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                 0f);
                if (!HoveringOverBook)
                {
                    Rectangle pageRectangle = new Rectangle((int)drawPosition.X - (int)(pageTexture.Width * scale.X), (int)yPageTop, (int)(pageTexture.Width * scale.X) * 2, (int)(pageTexture.Height * scale.Y));
                    HoveringOverBook = mouseRectangle.Intersects(pageRectangle);
                }
            }

            string placeholderText = "Text. ";
            for (int i = 0; i < 11; i++)
            {
                placeholderText += placeholderText;
            }

            // Create text and arrows.
            if (FadeTime >= FadeTimeMax - 4 && Active)
            {
                int textWidth = (int)(xScale * pageTexture.Width) - TextStartOffsetX;
                textWidth = (int)(textWidth * xResolutionScale);
                List<string> dialogLines = Utils.WordwrapString(placeholderText, Main.fontMouseText, textWidth, 250, out _).ToList();
                dialogLines.RemoveAll(text => string.IsNullOrEmpty(text));

                // Ensure the page number doesn't become nonsensical as a result of a resolution change (such as by resizing the game).
                if (Page < 0)
                    Page = 0;
                if (Page > TotalPages)
                    Page = TotalPages;

                int startingLine = Page * (TotalLinesPerPage + 1);
                int endingLine = startingLine + TotalLinesPerPage + 1;
                if (endingLine > dialogLines.Count)
                    endingLine = dialogLines.Count;

                DrawArrows(spriteBatch, xResolutionScale, yResolutionScale, yPageTop + 586f * yResolutionScale, mouseRectangle);
                for (int i = startingLine; i < endingLine; i++)
                {
                    if (dialogLines[i] != null)
                    {
                        int textDrawPositionX = (int)(Main.screenWidth / 2 + 100 * xResolutionScale) - (int)(pageTexture.Width * xResolutionScale);
                        int textDrawPositionY = (int)(250 * yResolutionScale) + (i - startingLine) * (int)(28 * yResolutionScale) + (int)yPageTop;

                        Color drawColor = Color.Lerp(Color.Purple, Color.DarkCyan, (float)Math.Cos(i * 0.1) * 0.5f + 0.5f);
                        Utils.DrawBorderStringFourWay(spriteBatch,
                                                      Main.fontMouseText,
                                                      dialogLines[i],
                                                      textDrawPositionX,
                                                      textDrawPositionY,
                                                      drawColor,
                                                      Color.Black,
                                                      Vector2.Zero);
                    }
                }
                DrawSpecialImage(spriteBatch, xResolutionScale, yResolutionScale, yPageTop);
            }
        }
        public void DrawArrows(SpriteBatch spriteBatch, float xResolutionScale, float yResolutionScale, float yPageBottom, Rectangle mouseRectangle)
        {
            Texture2D arrowTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/DraedonsLogArrow");
            if (Page > 0)
            {
                Vector2 drawPosition = new Vector2(Main.screenWidth / 2 - 70f, yPageBottom);
                Rectangle arrowRectangle = new Rectangle((int)drawPosition.X, (int)drawPosition.Y, arrowTexture.Width, arrowTexture.Height);
                arrowRectangle.Width = (int)(arrowRectangle.Width * xResolutionScale);
                arrowRectangle.Height = (int)(arrowRectangle.Height * yResolutionScale);

                if (mouseRectangle.Intersects(arrowRectangle))
                {
                    arrowTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/DraedonsLogArrowHover");
                    if (ArrowClickCooldown <= 0 && Main.mouseLeft)
                    {
                        Page--;
                        ArrowClickCooldown = 8;
                    }
                    Main.blockMouse = true;
                }

                spriteBatch.Draw(arrowTexture, drawPosition, null, Color.White, 0f, Vector2.Zero, new Vector2(xResolutionScale, yResolutionScale), SpriteEffects.FlipHorizontally, 0f);
            }

            if (Page < TotalPages - 1)
            {
                Vector2 drawPosition = new Vector2(Main.screenWidth / 2 + 70f, yPageBottom);
                Rectangle arrowRectangle = new Rectangle((int)drawPosition.X, (int)drawPosition.Y, arrowTexture.Width, arrowTexture.Height);
                arrowRectangle.Width = (int)(arrowRectangle.Width * xResolutionScale);
                arrowRectangle.Height = (int)(arrowRectangle.Height * yResolutionScale);

                if (mouseRectangle.Intersects(arrowRectangle))
                {
                    arrowTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/DraedonsLogArrowHover");
                    if (ArrowClickCooldown <= 0 && Main.mouseLeft)
                    {
                        Page++;
                        ArrowClickCooldown = 8;
                    }
                    Main.blockMouse = true;
                }

                spriteBatch.Draw(arrowTexture, drawPosition, null, Color.White, 0f, Vector2.Zero, new Vector2(xResolutionScale, yResolutionScale), SpriteEffects.None, 0f);
            }
        }
        public void DrawSpecialImage(SpriteBatch spriteBatch, float xResolutionScale, float yResolutionScale, float yPageTop)
        {
            Vector2 drawPosition;
            drawPosition.X = Main.screenWidth / 2 + (int)(320f * xResolutionScale);
            drawPosition.Y = yPageTop;
            Vector2 scale = new Vector2(540f * xResolutionScale, 450f * yResolutionScale);
            switch (Page)
            {
                case 0:
                    Texture2D texture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/DraedonsLogPrismBack");
                    scale /= texture.Size();
                    drawPosition.Y += texture.Height * 0.4f * scale.Y;
                    spriteBatch.Draw(texture, drawPosition, null, Color.White, 0f, new Vector2(texture.Width * 0.5f, 0f), scale, SpriteEffects.None, 0f);
                    break;
                case 1:
                    texture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/DraedonsLogGiantClam");
                    scale /= texture.Size();
                    drawPosition.Y += texture.Height * 0.4f * scale.Y;
                    spriteBatch.Draw(texture, drawPosition, null, Color.White, 0f, new Vector2(texture.Width * 0.5f, 0f), scale, SpriteEffects.None, 0f);
                    break;
            }
        }
    }
}
