using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
    public class StealthStrikeUI
    {
        public static Vector2 DrawPosition = new Vector2(CalamityMod.CalamityConfig.StealthMeterPosX, CalamityMod.CalamityConfig.StealthMeterPosY);
        public static Vector2 Offset = DrawPosition;
        public static void Draw(SpriteBatch spriteBatch, Player player)
        {
            if (DrawPosition.X <= 60f && DrawPosition.Y <= 20f)
            {
                DrawPosition.X = Main.screenWidth / 2f;
                DrawPosition.Y = Main.screenHeight / 2 + Main.LocalPlayer.height / 2f + 24f;
            }
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.stealthUIAlpha <= 0f || !CalamityMod.CalamityConfig.StealthBar)
            {
                if (CalamityMod.CalamityConfig.StealthMeterPosX != DrawPosition.X)
                {
                    CalamityMod.CalamityConfig.StealthMeterPosX = DrawPosition.X;
                    CalamityMod.SaveConfig(CalamityMod.CalamityConfig);
                }
                if (CalamityMod.CalamityConfig.StealthMeterPosY != DrawPosition.Y)
                {
                    CalamityMod.CalamityConfig.StealthMeterPosY = DrawPosition.Y;
                    CalamityMod.SaveConfig(CalamityMod.CalamityConfig);
                }
                return;
            }
            Texture2D edgeTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/StealthMeter");
            Texture2D indicatorTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/StealthMeterStrikeIndicator");
            Texture2D barTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/StealthMeterBar");
            Texture2D fullBarTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/StealthMeterBarFull");
            spriteBatch.Draw(edgeTexture, DrawPosition, null, Color.White * modPlayer.stealthUIAlpha, 0f, edgeTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            if (modPlayer.rogueStealth >= modPlayer.rogueStealthMax * (modPlayer.stealthStrikeHalfCost ? 0.5f : 1f))
            {
                spriteBatch.Draw(indicatorTexture, DrawPosition, null, Color.White * modPlayer.stealthUIAlpha, 0f, indicatorTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }

            float completionRatio = modPlayer.rogueStealth / modPlayer.rogueStealthMax;
            Rectangle barRectangle = new Rectangle(0, 0, (int)(barTexture.Width * completionRatio), barTexture.Width);
            bool full = modPlayer.rogueStealth >= modPlayer.rogueStealthMax;
            spriteBatch.Draw(full ? fullBarTexture : barTexture, DrawPosition, barRectangle, Color.White * modPlayer.stealthUIAlpha, 0f, indicatorTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            if (new Rectangle((int)(DrawPosition.X + Main.screenPosition.X - player.width / 2), (int)(DrawPosition.Y + Main.screenPosition.Y), barTexture.Width, barTexture.Height).Intersects(
                new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 8, 8)))
            {
                if (modPlayer.rogueStealthMax > 0f && modPlayer.stealthUIAlpha >= 0.5f)
                {
                    Main.LocalPlayer.mouseInterface = true;
                    Main.instance.MouseText("Stealth: " + (int)(modPlayer.rogueStealth * 100) + "/" + (int)(modPlayer.rogueStealthMax * 100) + "", 0, 0, -1, -1, -1, -1); //only way I got this to show up consistently, otherwise it fucked up and showed up anywhere onscreen lol.
                    modPlayer.stealthUIAlpha = MathHelper.Lerp(modPlayer.stealthUIAlpha, 0.25f, 0.035f);
                }
            }
            if (!CalamityMod.CalamityConfig.MeterPosLock)
            {
                if (new Rectangle((int)(DrawPosition.X + Main.screenPosition.X - 26), (int)(DrawPosition.Y + Main.screenPosition.Y - 9), 52, 18).Intersects(
                   new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 8, 8)))
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        Offset = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                        DrawPosition.X = Offset.X;
                        DrawPosition.Y = Offset.Y;
                    }
                    if (Mouse.GetState().LeftButton == ButtonState.Released)
                    {
                        DrawPosition.X = Offset.X;
                        DrawPosition.Y = Offset.Y;
                    }
                }

                if (CalamityMod.CalamityConfig.StealthMeterPosX != DrawPosition.X)
                {
                    CalamityMod.CalamityConfig.StealthMeterPosX = DrawPosition.X;
                    CalamityMod.SaveConfig(CalamityMod.CalamityConfig);
                }
                if (CalamityMod.CalamityConfig.StealthMeterPosY != DrawPosition.Y)
                {
                    CalamityMod.CalamityConfig.StealthMeterPosY = DrawPosition.Y;
                    CalamityMod.SaveConfig(CalamityMod.CalamityConfig);
                }
            }
        }
    }
}
