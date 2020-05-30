using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
    public static class AcidRainUI
    {
        public static void Draw(SpriteBatch spriteBatch)
        {
            bool draw = CalamityWorld.rainingAcid || CalamityWorld.acidRainExtraDrawTime > 0;
            if ((draw && !Main.LocalPlayer.Calamity().ZoneSulphur) ||
                (!draw && Main.LocalPlayer.Calamity().ZoneSulphur))
                return;

            float progressAlpha = 1f;
            Vector2 screenCoords = new Vector2(Main.screenWidth - 120, Main.screenHeight - 40);
            if (Main.invasionProgressNearInvasion || Main.invasionProgressAlpha > 0f)
            {
                screenCoords.Y -= 85;
            }
            int sizeX = (int)(200f * progressAlpha);
            int sizeY = (int)(45f * progressAlpha);
            Rectangle screenCoordsRectangle =
                new Rectangle((int)screenCoords.X - sizeX / 2,
                (int)screenCoords.Y - sizeY / 2, sizeX, sizeY);
            Texture2D barTexture = Main.colorBarTexture;

            // If you touch the netcode behind this without a good reason your life will come to an abrupt end -Dominic
            if (CalamityWorld.rainingAcid || CalamityWorld.acidRainExtraDrawTime > 0)
            {
                Utils.DrawInvBG(spriteBatch, screenCoordsRectangle, new Color(63, 65, 151, 255) * 0.785f);

                float progressRatio = 1f - CalamityWorld.AcidRainCompletionRatio;
                string progressText = $"{((int)(100 * progressRatio)).ToString()}%";
                progressText = Language.GetTextValue("Game.WaveCleared", progressText);
                spriteBatch.Draw(barTexture, screenCoords, null, Color.White, 0f, new Vector2(barTexture.Width / 2, 0f), progressAlpha, SpriteEffects.None, 0f);
                Vector2 textMeasurement = Main.fontMouseText.MeasureString(progressText);
                float adjustedAlpha = progressAlpha;
                if (textMeasurement.Y > 22f)
                {
                    adjustedAlpha *= 22f / textMeasurement.Y;
                }
                float barDrawOffsetX = 169f * progressAlpha;
                float scaleY = 8f * progressAlpha;
                Vector2 barDrawPosition = screenCoords + Vector2.UnitY * scaleY + Vector2.UnitX * 1f;
                Utils.DrawBorderString(spriteBatch, progressText, barDrawPosition + new Vector2(0f, -4f), Color.White, adjustedAlpha, 0.5f, 1f, -1);
                barDrawPosition += Vector2.UnitX * (progressRatio - 0.5f) * barDrawOffsetX;
                spriteBatch.Draw(Main.magicPixel, barDrawPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), new Color(255, 241, 51), 0f, new Vector2(1f, 0.5f), new Vector2(barDrawOffsetX * progressRatio, scaleY), SpriteEffects.None, 0f);
                spriteBatch.Draw(Main.magicPixel, barDrawPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), new Color(255, 165, 0, 127), 0f, new Vector2(1f, 0.5f), new Vector2(2f, scaleY), SpriteEffects.None, 0f);
                spriteBatch.Draw(Main.magicPixel, barDrawPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.Black, 0f, new Vector2(0f, 0.5f), new Vector2(barDrawOffsetX * (1f - progressRatio), scaleY), SpriteEffects.None, 0f);

                string invasionNameText = "Acid Rain";
                Vector2 textMeasurement2 = Main.fontMouseText.MeasureString(invasionNameText);
                float x = 120f;
                if (textMeasurement2.X > 200f)
                {
                    x += textMeasurement2.X - 200f;
                }
                Texture2D iconTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/AcidRainIcon");
                Rectangle iconRectangle = Utils.CenteredRectangle(new Vector2(Main.screenWidth - x, Main.screenHeight - 80), (textMeasurement2 + new Vector2(iconTexture.Width + 12, 6f)) * progressAlpha);
                if (Main.invasionProgressNearInvasion || Main.invasionProgressAlpha > 0f)
                {
                    iconRectangle.Y -= 85;
                }
                Utils.DrawInvBG(spriteBatch, iconRectangle, AcidRainEvent.TextColor * 0.5f);
                spriteBatch.Draw(iconTexture, iconRectangle.Left() + Vector2.UnitX * progressAlpha * 8f, null, Color.White, 0f, new Vector2(0f, iconTexture.Height / 2), progressAlpha * 0.8f, SpriteEffects.None, 0f);
                Utils.DrawBorderString(spriteBatch, invasionNameText, iconRectangle.Right() + Vector2.UnitX * progressAlpha * -22f, Color.White, progressAlpha * 0.9f, 1f, 0.4f, -1);
            }
        }
    }
}
