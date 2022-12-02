using System;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.UI.SulphurousWaterMeter
{
    public class SulphurousWaterMeterUI : ModSystem
    {
        // These values were decided relative to the stealth meter positions, and are by default slightly below them.
        internal const float DefaultPosX = StealthUI.DefaultStealthPosX;
        internal const float DefaultPosY = StealthUI.DefaultStealthPosY + 2.286282f;
        private const float MouseDragEpsilon = 0.05f; // 0.05%

        private static Vector2? dragOffset = null;
        private static Texture2D edgeTexture, barTexture;

        public override void OnModLoad()
        {
            edgeTexture = ModContent.Request<Texture2D>("CalamityMod/UI/SulphurousWaterMeter/SulphWaterMeter", AssetRequestMode.ImmediateLoad).Value;
            barTexture = ModContent.Request<Texture2D>("CalamityMod/UI/SulphurousWaterMeter/SulphWaterBar", AssetRequestMode.ImmediateLoad).Value;
            Reset();
        }

        public override void Unload()
        {
            Reset();
            edgeTexture = barTexture = null;
        }

        private static void Reset() => dragOffset = null;
        
        public static void Draw(SpriteBatch spriteBatch, Player player)
        {
            // Sanity check the planned position before drawing. This is done relative.
            Vector2 screenRatioPosition = new Vector2(CalamityConfig.Instance.SulphuricWaterMeterPosX, CalamityConfig.Instance.SulphuricWaterMeterPosY);
            if (screenRatioPosition.X < 0f || screenRatioPosition.X > 100f)
                screenRatioPosition.X = DefaultPosX;
            if (screenRatioPosition.Y < 0f || screenRatioPosition.Y > 100f)
                screenRatioPosition.Y = DefaultPosY;

            // Convert the screen ratio position to an absolute position in pixels
            // Cast to integer to prevent blurriness which results from decimal pixel positions
            float uiScale = Main.UIScale;
            Vector2 screenPos = screenRatioPosition;
            screenPos.X = (int)(screenPos.X * 0.01f * Main.screenWidth);
            screenPos.Y = (int)(screenPos.Y * 0.01f * Main.screenHeight);

            CalamityPlayer modPlayer = player.Calamity();

            // If not drawing the water meter, save its latest position to config and leave.
            if (modPlayer.SulphWaterUIOpacity > 0f && modPlayer.ZoneSulphur)
                DrawWaterBar(spriteBatch, modPlayer, screenPos);
            else
            {
                bool changed = false;
                if (CalamityConfig.Instance.SulphuricWaterMeterPosX != screenRatioPosition.X)
                {
                    CalamityConfig.Instance.SulphuricWaterMeterPosX = screenRatioPosition.X;
                    changed = true;
                }
                if (CalamityConfig.Instance.SulphuricWaterMeterPosY != screenRatioPosition.Y)
                {
                    CalamityConfig.Instance.SulphuricWaterMeterPosY = screenRatioPosition.Y;
                    changed = true;
                }

                if (changed)
                    CalamityMod.SaveConfig(CalamityConfig.Instance);
            }

            Rectangle mouseHitbox = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
            Rectangle waterBarArea = Utils.CenteredRectangle(screenPos, edgeTexture.Size() * uiScale);

            MouseState ms = Mouse.GetState();
            Vector2 mousePos = Main.MouseScreen;

            // Handle mouse dragging
            if (waterBarArea.Intersects(mouseHitbox))
            {
                if (!CalamityConfig.Instance.MeterPosLock)
                    Main.LocalPlayer.mouseInterface = true;

                // If the mouse is on top of the meter, show the player's accumulated sulphuric poisoning.
                if (modPlayer.SulphWaterPoisoningLevel > 0f && modPlayer.SulphWaterUIOpacity >= 0f)
                {
                    string poisonText = (modPlayer.SulphWaterPoisoningLevel * 100f).ToString("n2");
                    string textToDisplay = $"Sulphuric Poisoning: {poisonText}/100";
                    Main.instance.MouseText(textToDisplay, 0, 0, -1, -1, -1, -1);
                    modPlayer.SulphWaterUIOpacity = MathHelper.Lerp(modPlayer.SulphWaterUIOpacity, 0.25f, 0.035f);
                }

                Vector2 newScreenRatioPosition = screenRatioPosition;

                // As long as the mouse button is held down, drag the meter along with an offset.
                if (!CalamityConfig.Instance.MeterPosLock && ms.LeftButton == ButtonState.Pressed)
                {
                    // If the drag offset doesn't exist yet, create it.
                    if (!dragOffset.HasValue)
                        dragOffset = mousePos - screenPos;

                    // Given the mouse's absolute current position, compute where the corner of the water bar should be based on the original drag offset.
                    Vector2 newCorner = mousePos - dragOffset.GetValueOrDefault(Vector2.Zero);

                    // Convert the new corner position into a screen ratio position.
                    newScreenRatioPosition.X = (100f * newCorner.X) / Main.screenWidth;
                    newScreenRatioPosition.Y = (100f * newCorner.Y) / Main.screenHeight;
                }

                // Compute the change in position. If it is large enough, actually move the meter
                Vector2 delta = newScreenRatioPosition - screenRatioPosition;
                if (Math.Abs(delta.X) >= MouseDragEpsilon || Math.Abs(delta.Y) >= MouseDragEpsilon)
                {
                    CalamityConfig.Instance.SulphuricWaterMeterPosX = newScreenRatioPosition.X;
                    CalamityConfig.Instance.SulphuricWaterMeterPosY = newScreenRatioPosition.Y;
                }

                // When the mouse is released, save the config and destroy the drag offset.
                if (ms.LeftButton == ButtonState.Released)
                {
                    dragOffset = null;
                    CalamityMod.SaveConfig(CalamityConfig.Instance);
                }
            }
            else
                modPlayer.SulphWaterUIOpacity = MathHelper.Clamp(modPlayer.SulphWaterUIOpacity + (modPlayer.SulphWaterPoisoningLevel > 0f).ToDirectionInt() * 0.06f, 0f, 1f);
        }

        private static void DrawWaterBar(SpriteBatch spriteBatch, CalamityPlayer modPlayer, Vector2 screenPos)
        {
            float uiScale = Main.UIScale;
            float offset = (edgeTexture.Width - barTexture.Width) * 0.5f;
            spriteBatch.Draw(edgeTexture, screenPos, null, Color.White * modPlayer.SulphWaterUIOpacity, 0f, edgeTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);
            
            float completionRatio = MathHelper.Clamp(modPlayer.SulphWaterPoisoningLevel, 0f, 1f);
            Rectangle barRectangle = new Rectangle(0, 0, (int)(barTexture.Width * completionRatio), barTexture.Width);
            spriteBatch.Draw(barTexture, screenPos + new Vector2(offset * uiScale, 0), barRectangle, Color.White * modPlayer.SulphWaterUIOpacity, 0f, edgeTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);
        }
    }
}
