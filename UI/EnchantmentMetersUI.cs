using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
    public class EnchantmentMetersUI
    {
        public static Vector2 DrawPosition => Main.LocalPlayer.Center;
        public static void Draw(SpriteBatch spriteBatch, Player player)
        {
            Item heldItem = player.ActiveItem();

            // Do not draw if the player has an empty item. Precise item checks for modded content will fail.
            if (heldItem is null || heldItem.IsAir)
                return;

            // Draw a bar above the player that displays the discharge of an item if the enchant is used.
            if (player.Calamity().dischargingItemEnchant)
            {
                float dischargeFactor = heldItem.Calamity().DischargeExhaustionRatio;
                Texture2D barBorderTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/EphemeralBarBorder");
                Texture2D barTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/EphemeralBar");

                int barCutoff = (int)(barTexture.Height * (1f - dischargeFactor));
                Rectangle barFrame = new Rectangle(0, barCutoff, barTexture.Width, barTexture.Height - barCutoff);
                Vector2 barDrawPosition = player.Top - Vector2.UnitY * (barTexture.Height * 0.5f + 40f) + Vector2.UnitY * player.gfxOffY - Main.screenPosition;
                Color barColor = Color.White * 0.6f;

                spriteBatch.Draw(barBorderTexture, barDrawPosition, null, barColor, 0f, barBorderTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(barTexture, barDrawPosition, barFrame, barColor, 0f, barTexture.Size() * 0.5f, 1f, SpriteEffects.FlipVertically, 0f);
            }
        }
    }
}
