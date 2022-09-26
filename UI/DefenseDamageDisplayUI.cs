using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI.Chat;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.GameContent;

namespace CalamityMod.UI
{
    public static class DefenseDamageDisplayUI
    {
        public static void Draw(SpriteBatch spriteBatch)
        {
            int defenseDamage = Main.LocalPlayer.Calamity().CurrentDefenseDamage;

            // Do nothing if no defense damage has been incurred.
            if (defenseDamage <= 0)
                return;

            string defenseDamageText = (-defenseDamage).ToString();
            Texture2D defenseDamageIcon = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/DefenseDamage").Value;
            Vector2 defenseDamageIconCenter = new Vector2(Main.screenWidth - Main.UIScale * 328f, Main.UIScale * 16f) + defenseDamageIcon.Size() * 0.5f;
            Rectangle defenseDamageIconArea = Utils.CenteredRectangle(defenseDamageIconCenter, defenseDamageIcon.Size() * Main.UIScale);
            Vector2 defenseDamageTextArea = FontAssets.MouseText.Value.MeasureString(defenseDamageText);
            Vector2 defenseDamageTextDrawPosition = defenseDamageIconCenter + new Vector2(6f, 16f) - defenseDamageTextArea * 0.5f;
            Rectangle mouseArea = new Rectangle(Main.mouseX, Main.mouseY, 2, 2);
            bool hoveringOverIcon = mouseArea.Intersects(defenseDamageIconArea);
            if (hoveringOverIcon)
                defenseDamageIcon = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/DefenseDamageHover").Value;

            // Draw the icon.
            spriteBatch.Draw(defenseDamageIcon, defenseDamageIconCenter, null, Color.White, 0f, defenseDamageIcon.Size() * 0.5f, Main.UIScale, 0, 0f);

            // Draw the amount of defense damage currently in effect as text. It is negatively signed as an indicator that it is quantity of loss and not gain.
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, defenseDamageText, defenseDamageTextDrawPosition, Color.IndianRed, 0f, Vector2.Zero, Vector2.One * Main.UIScale * 0.75f);

            // Display the amount of defense and defense damage the player has if hovering over the icon.
            if (hoveringOverIcon)
            {
                Main.hoverItemName = $"{Main.LocalPlayer.statDefense} {Language.GetTextValue("LegacyInterface.10")}" +
                    $"\n{defenseDamage} {Language.GetTextValue("Mods.CalamityMod.DefenseDamage")}";
            }
        }
    }
}
