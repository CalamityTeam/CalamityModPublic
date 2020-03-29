using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
    public static class StealthStrikeUI
    {
        public static void Draw(SpriteBatch spriteBatch, Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.stealthUIAlpha <= 0f || !CalamityMod.CalamityConfig.StealthBar)
                return;
            Texture2D edgeTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/StealthMeter");
            Texture2D indicatorTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/StealthMeterStrikeIndicator");
            Texture2D barTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/StealthMeterBar");
            Texture2D fullBarTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/StealthMeterBarFull");
            Vector2 drawPosition = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f + new Vector2(0f, player.height / 2f + 24f) - Main.screenPosition;
            spriteBatch.Draw(edgeTexture, drawPosition, null, Color.White * modPlayer.stealthUIAlpha, 0f, edgeTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            if (modPlayer.rogueStealth >= modPlayer.rogueStealthMax * (modPlayer.stealthStrikeHalfCost ? 0.5f : 1f))
            {
                spriteBatch.Draw(indicatorTexture, drawPosition, null, Color.White * modPlayer.stealthUIAlpha, 0f, indicatorTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }

            float completionRatio = modPlayer.rogueStealth / modPlayer.rogueStealthMax;
            Rectangle barRectangle = new Rectangle(0, 0, (int)(barTexture.Width * completionRatio), barTexture.Width);
            bool full = modPlayer.rogueStealth >= modPlayer.rogueStealthMax;
            spriteBatch.Draw(full ? fullBarTexture : barTexture, drawPosition, barRectangle, Color.White * modPlayer.stealthUIAlpha, 0f, indicatorTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            if (new Rectangle((int)(drawPosition.X + Main.screenPosition.X - player.width / 2), (int)(drawPosition.Y + Main.screenPosition.Y), barTexture.Width, barTexture.Height).Intersects(
                new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 8, 8)))
            {
                if (modPlayer.rogueStealthMax > 0f)
                {
                    Main.LocalPlayer.mouseInterface = true;
                    Main.instance.MouseText("Stealth: " + (int)(modPlayer.rogueStealth * 100) + "/" + (int)(modPlayer.rogueStealthMax * 100) + "", 0, 0, -1, -1, -1, -1); //only way I got this to show up consistently, otherwise it fucked up and showed up anywhere onscreen lol.
                }
            }
        }
    }
}
