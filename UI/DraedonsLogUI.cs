using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
    public class DraedonsLogUI : PopupGUI
    {
        public const int PageTextPadding = 15;
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

            if (Main.mouseLeft && Main.mouseLeftRelease && !Main.blockMouse && FadeTime >= 30)
            {
                Main.mouseLeftRelease = false;
                Main.mouseLeft = false;
                Active = false;
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D pageTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/DraedonsLogPage");
            float xScale = MathHelper.Lerp(0.004f, 1f, FadeTime / (float)FadeTimeMax);
            Vector2 scale = new Vector2(xScale, 1f) * new Vector2(Main.screenWidth, Main.screenHeight) / pageTexture.Size();
            scale *= 0.5f;

            float bookScale = 0.75f;

            scale *= bookScale;
            float yPageTop = MathHelper.Lerp(Main.screenHeight * 2, Main.screenHeight * 0.25f, FadeTime / (float)FadeTimeMax);

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
            }

            // Create text.
            if (FadeTime >= FadeTimeMax - 4 && Active)
            {
                Vector2 topLeft = new Vector2(Main.screenWidth * 0.5f - pageTexture.Width * scale.X * 0.5f, yPageTop + pageTexture.Height * scale.Y * 0.25f);
                Vector2 topRight = new Vector2(Main.screenWidth * 0.5f + pageTexture.Width * scale.X * 0.5f, yPageTop + pageTexture.Height * scale.Y * 0.25f);

                topLeft.X -= PageTextPadding;
                topRight.X -= PageTextPadding;

                string retardedText = @"This text is a placeholder.";

                retardedText = string.Concat(Utils.WordwrapString(retardedText, Main.fontMouseText, (int)(pageTexture.Width * scale.X) - PageTextPadding * 2, 9, out _).Select(text => text + "\n"));

                int splitPoint = retardedText.Length / 2;
                while (retardedText[splitPoint] != '.')
                {
                    splitPoint++;
                }
                string firstHalf = string.Concat(retardedText.Take(splitPoint + 1).ToArray());
                string secondHalf = string.Concat(retardedText.Skip(splitPoint).ToArray());
                Utils.DrawBorderString(spriteBatch, firstHalf, topLeft, Color.White);
                Utils.DrawBorderString(spriteBatch, secondHalf, topRight, Color.White);
            }
        }
    }
}
