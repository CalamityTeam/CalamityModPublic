using CalamityMod.CalPlayer;
using CalamityMod.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;

namespace CalamityMod.UI
{
    public abstract class InvasionProgressUI
    {
        public virtual int SecondaryDigitPrecision { get; } = 0;
        public abstract bool IsActive { get; }
        public abstract float CompletionRatio { get; }
        public abstract string InvasionName { get; }
        public abstract Color InvasionBarColor { get; }
        public abstract Texture2D IconTexture { get; }
        public virtual void DrawBlueBar(SpriteBatch spriteBatch, Vector2 barDrawPosition, int barOffsetY)
        {
            int barWidth = 200;
            int barHeight = 45;

            barDrawPosition.Y += barOffsetY;

            Rectangle screenCoordsRectangle = new Rectangle((int)barDrawPosition.X - barWidth / 2, (int)barDrawPosition.Y - barHeight / 2, barWidth, barHeight);
            Texture2D barTexture = TextureAssets.ColorBar.Value;

            Utils.DrawInvBG(spriteBatch, screenCoordsRectangle, new Color(63, 65, 151, 255) * 0.785f);
            spriteBatch.Draw(barTexture, barDrawPosition, null, Color.White, 0f, new Vector2(barTexture.Width / 2, 0f), 1f, SpriteEffects.None, 0f);
        }
        public virtual void DrawProgressText(SpriteBatch spriteBatch, float yScale, Vector2 baseBarDrawPosition, int barOffsetY, out Vector2 newBarPosition)
        {
            string progressText = (100 * CompletionRatio).ToString($"N{SecondaryDigitPrecision}") + "%";
            progressText = Language.GetTextValue("Game.WaveCleared", progressText);

            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(progressText);
            float progressTextScale = 1f;
            if (textSize.Y > 22f)
                progressTextScale *= 22f / textSize.Y;

            newBarPosition = baseBarDrawPosition + Vector2.UnitY * (yScale + barOffsetY);
            Utils.DrawBorderString(spriteBatch, progressText, newBarPosition - Vector2.UnitY * 4f, Color.White, progressTextScale, 0.5f, 1f, -1);
        }
        public virtual void DrawBackground(SpriteBatch spriteBatch, float yScale, Vector2 baseBarDrawPosition, int barOffsetY)
        {
            float barDrawOffsetX = 169f;
            Vector2 barDrawPosition = baseBarDrawPosition + Vector2.UnitX * (CompletionRatio - 0.5f) * barDrawOffsetX;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, barDrawPosition, new Rectangle(0, 0, 1, 1), new Color(255, 241, 51), 0f, new Vector2(1f, 0.5f), new Vector2(barDrawOffsetX * CompletionRatio, yScale), SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, barDrawPosition, new Rectangle(0, 0, 1, 1), new Color(255, 165, 0, 127), 0f, new Vector2(1f, 0.5f), new Vector2(2f, yScale), SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, barDrawPosition, new Rectangle(0, 0, 1, 1), Color.Black, 0f, Vector2.UnitY * 0.5f, new Vector2(barDrawOffsetX * (1f - CompletionRatio), yScale), SpriteEffects.None, 0f);
        }
        public virtual void DrawProgressTextAndIcons(SpriteBatch spriteBatch, int barOffsetY)
        {
            Vector2 textMeasurement = FontAssets.MouseText.Value.MeasureString(InvasionName);
            float x = 120f;
            if (textMeasurement.X > 200f)
            {
                x += textMeasurement.X - 200f;
            }
            Rectangle iconRectangle = Utils.CenteredRectangle(new Vector2(Main.screenWidth - x, Main.screenHeight - 80 + barOffsetY), textMeasurement + new Vector2(IconTexture.Width + 12, 6f));
            Utils.DrawInvBG(spriteBatch, iconRectangle, InvasionBarColor * 0.5f);
            spriteBatch.Draw(IconTexture, iconRectangle.Left() + Vector2.UnitX * 8f, null, Color.White, 0f, Vector2.UnitY * IconTexture.Height / 2, 0.8f, SpriteEffects.None, 0f);
            Utils.DrawBorderString(spriteBatch, InvasionName, iconRectangle.Right() + Vector2.UnitX * -16f, Color.White, 0.9f, 1f, 0.4f, -1);
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!IsActive || Main.invasionProgressMode == 0)
                return;

            int barOffsetY = 0;
            int totalBars = 0;
            if (Main.invasionProgressNearInvasion || Main.invasionProgressAlpha > 0f)
                totalBars++;

            // Incorporate the MGR boss health bar so that the two do not overlap.
            // Also, keep the offset active during boss rush, so that it doesn't flip positions during cool-down periods.
            if (CalamityPlayer.areThereAnyDamnBosses || BossRushEvent.BossRushActive)
                totalBars++;
            totalBars += InvasionProgressUIManager.TotalGUIsActive;
            // Rise somewhat if there's other invasions going on.
            barOffsetY -= 85 * (totalBars - 1);

            Vector2 barDrawPosition = new Vector2(Main.screenWidth - 120, Main.screenHeight - 40);
            DrawBlueBar(spriteBatch, barDrawPosition, barOffsetY);

            float yScale = 8f;
            DrawProgressText(spriteBatch, yScale, barDrawPosition, barOffsetY, out Vector2 newBarPosition);
            DrawBackground(spriteBatch, yScale, newBarPosition, barOffsetY);
            DrawProgressTextAndIcons(spriteBatch, barOffsetY);
        }
    }
}
