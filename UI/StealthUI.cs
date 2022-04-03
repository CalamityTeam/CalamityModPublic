using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
    public class StealthUI
    {
        public static Vector2 DrawPosition = new Vector2(CalamityConfig.Instance.StealthMeterPosX, CalamityConfig.Instance.StealthMeterPosY);
        public static Vector2 Offset = DrawPosition;
        public static void Draw(SpriteBatch spriteBatch, Player player)
        {
            if (DrawPosition.X <= 60f && DrawPosition.Y <= 20f)
            {
                DrawPosition.X = Main.screenWidth / 2f;
                DrawPosition.Y = Main.screenHeight / 2 + Main.LocalPlayer.height / 2f + 24f;
            }
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.stealthUIAlpha <= 0f || !CalamityConfig.Instance.StealthBar || modPlayer.rogueStealthMax <= 0f || !modPlayer.wearingRogueArmor)
            {
                if (CalamityConfig.Instance.StealthMeterPosX != DrawPosition.X)
                {
                    CalamityConfig.Instance.StealthMeterPosX = DrawPosition.X;
                    CalamityMod.SaveConfig(CalamityConfig.Instance);
                }
                if (CalamityConfig.Instance.StealthMeterPosY != DrawPosition.Y)
                {
                    CalamityConfig.Instance.StealthMeterPosY = DrawPosition.Y;
                    CalamityMod.SaveConfig(CalamityConfig.Instance);
                }
                return;
            }
            Texture2D edgeTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/StealthMeter");
            Texture2D indicatorTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/StealthMeterStrikeIndicator");
            Texture2D barTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/StealthMeterBar");
            Texture2D fullBarTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/StealthMeterBarFull");
            float uiScale = Main.UIScale;
            spriteBatch.Draw(edgeTexture, DrawPosition, null, Color.White * modPlayer.stealthUIAlpha, 0f, edgeTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);
            if (modPlayer.StealthStrikeAvailable())
            {
                spriteBatch.Draw(indicatorTexture, DrawPosition, null, Color.White * modPlayer.stealthUIAlpha, 0f, indicatorTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);
            }

            float completionRatio = modPlayer.rogueStealth / modPlayer.rogueStealthMax;
            Rectangle barRectangle = new Rectangle(0, 0, (int)(barTexture.Width * completionRatio), barTexture.Width);
            bool full = modPlayer.rogueStealth >= modPlayer.rogueStealthMax;
            spriteBatch.Draw(full ? fullBarTexture : barTexture, DrawPosition, barRectangle, Color.White * modPlayer.stealthUIAlpha, 0f, indicatorTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);

            Rectangle mouse = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
            Rectangle stealthBar = Utils.CenteredRectangle(DrawPosition, barTexture.Size() * uiScale);

            if (stealthBar.Intersects(mouse))
            {
                if (modPlayer.rogueStealthMax > 0f && modPlayer.stealthUIAlpha >= 0.5f)
                {
                    Main.LocalPlayer.mouseInterface = true;
                    int stealthInt = (int)Math.Round(modPlayer.rogueStealth * 100f);
                    int maxStealthInt = (int)Math.Round(modPlayer.rogueStealthMax * 100f);
                    //only way I got this to show up consistently, otherwise it fucked up and showed up anywhere onscreen lol.
                    Main.instance.MouseText("Stealth: " + stealthInt + "/" + maxStealthInt + "", 0, 0, -1, -1, -1, -1);
                    modPlayer.stealthUIAlpha = MathHelper.Lerp(modPlayer.stealthUIAlpha, 0.25f, 0.035f);
                }
            }
            if (!CalamityConfig.Instance.MeterPosLock)
            {
                if (stealthBar.Intersects(mouse))
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

                if (CalamityConfig.Instance.StealthMeterPosX != DrawPosition.X)
                {
                    CalamityConfig.Instance.StealthMeterPosX = DrawPosition.X;
                    CalamityMod.SaveConfig(CalamityConfig.Instance);
                }
                if (CalamityConfig.Instance.StealthMeterPosY != DrawPosition.Y)
                {
                    CalamityConfig.Instance.StealthMeterPosY = DrawPosition.Y;
                    CalamityMod.SaveConfig(CalamityConfig.Instance);
                }
            }
        }
    }
}
