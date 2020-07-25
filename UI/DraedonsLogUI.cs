using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
    public class DraedonsLogUI : PopupGUI
    {
        public int Page = 0;
        public int ArrowClickCooldown;
        public bool HoveringOverBook = false;
        public int TotalLinesPerPage => (int)Math.Ceiling(16 * Main.screenHeight / 1440f - 1);
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

            string placeholderText = "A GOD DOES NOT FEAR DEATH ";
            for (int i = 0; i < 7; i++)
            {
                placeholderText += placeholderText;
            }

            // Create text and arrows.
            if (FadeTime >= FadeTimeMax - 4 && Active)
            {
                int textWidth = (int)(xScale * 2 * pageTexture.Width) - TextStartOffsetX;
                textWidth = (int)(textWidth * xResolutionScale);
                List<string> dialogLines = Utils.WordwrapString(placeholderText, Main.fontMouseText, textWidth, 250, out _).ToList();
                dialogLines.RemoveAll(text => string.IsNullOrEmpty(text));

                int totalPages = dialogLines.Count / (TotalLinesPerPage * 2 + 2);

                // Ensure the page number doesn't become nonsensical as a result of a resolution change (such as by resizing the game).
                if (Page < 0)
                    Page = 0;
                if (Page > totalPages)
                    Page = totalPages;

                // Draw arrows.
                if (Page > 0)
                {
                    Texture2D pageArrowTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/DraedonsLogArrow");
                    Vector2 offset = new Vector2(-(int)(pageTexture.Width * xResolutionScale) * 2 - (int)(36 * xResolutionScale), (int)(pageTexture.Height * yResolutionScale) + (int)(52 * yResolutionScale));
                    Vector2 bottomLeft = new Vector2(Main.screenWidth / 2, yPageTop);
                    bottomLeft += offset;
                    Rectangle arrowRectangle = new Rectangle((int)bottomLeft.X, (int)bottomLeft.Y, pageArrowTexture.Width, pageArrowTexture.Height);
                    arrowRectangle.Width = (int)(arrowRectangle.Width * xResolutionScale);
                    arrowRectangle.Height = (int)(arrowRectangle.Height * yResolutionScale);
                    if (mouseRectangle.Intersects(arrowRectangle))
                    {
                        pageArrowTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/DraedonsLogArrowHover");
                        if (ArrowClickCooldown <= 0 && Main.mouseLeft)
                        {
                            Page--;
                            ArrowClickCooldown = 8;
                        }
                        Main.blockMouse = true;
                    }

                    spriteBatch.Draw(pageArrowTexture,
                                     bottomLeft,
                                     null,
                                     Color.White,
                                     0f,
                                     Vector2.Zero,
                                     new Vector2(xResolutionScale, yResolutionScale),
                                     SpriteEffects.FlipHorizontally,
                                     0f);
                }
                if (Page < totalPages)
                {
                    Texture2D pageArrowTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/DraedonsLogArrow");
                    Vector2 offset = new Vector2((int)(pageTexture.Width * xResolutionScale) * 2 + (int)(10 * xResolutionScale), (int)(pageTexture.Height * yResolutionScale) + (int)(52 * yResolutionScale));
                    Vector2 bottomRight = new Vector2(Main.screenWidth / 2, yPageTop);
                    bottomRight += offset;
                    Rectangle arrowRectangle = new Rectangle((int)bottomRight.X, (int)bottomRight.Y, pageArrowTexture.Width, pageArrowTexture.Height); ;
                    arrowRectangle.Width = (int)(arrowRectangle.Width * xResolutionScale);
                    arrowRectangle.Height = (int)(arrowRectangle.Height * yResolutionScale);
                    if (mouseRectangle.Intersects(arrowRectangle))
                    {
                        pageArrowTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/DraedonsLogArrowHover");
                        if (ArrowClickCooldown <= 0 && Main.mouseLeft)
                        {
                            Page++;
                            ArrowClickCooldown = 8;
                        }
                        Main.blockMouse = true;
                    }

                    spriteBatch.Draw(pageArrowTexture,
                                     bottomRight,
                                     null,
                                     Color.White,
                                     0f,
                                     Vector2.Zero,
                                     new Vector2(xResolutionScale, yResolutionScale),
                                     SpriteEffects.None,
                                     0f);
                }
                int startingLine = Page * (TotalLinesPerPage * 2 + 2);
                int endingLine = startingLine + TotalLinesPerPage * 2 + 2;
                if (endingLine > dialogLines.Count)
                    endingLine = dialogLines.Count;

                for (int i = startingLine; i < endingLine; i++)
                {
                    bool onNextPage = i - startingLine > TotalLinesPerPage;
                    if (dialogLines[i] != null)
                    {
                        int textDrawPositionX = (int)(Main.screenWidth / 2 - 50 * xResolutionScale) - (int)(pageTexture.Width * 2 * xResolutionScale);
                        int textDrawPositionY = (int)(50 * yResolutionScale) + (i - startingLine) * 24 + (int)yPageTop;
                        if (onNextPage)
                        {
                            textDrawPositionX = (int)(Main.screenWidth / 2 + 16 * xResolutionScale) + (int)(TextStartOffsetX * 1.5 * xResolutionScale);
                            textDrawPositionY = (int)(50 * yResolutionScale) + (i - startingLine - (TotalLinesPerPage + 1)) * 24 + (int)yPageTop;
                        }

                        Color drawColor = Color.Lerp(Color.Cyan, Color.DarkCyan, (float)Math.Cos(i * 0.4) * 0.5f + 0.5f);
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
            }
        }
    }
}
