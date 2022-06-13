using System;
using CalamityMod.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
    // TODO -- This can be made into a ModSystem with simple OnModLoad and Unload hooks.
    public class ChargeMeterUI
    {
        // These values were handpicked on a 1080p screen by Ozzatron. Please disregard the bizarre precision.
        internal const float DefaultChargePosX = 50.104603f;
        internal const float DefaultChargePosY = 58.05169f;
        private const float MouseDragEpsilon = 0.05f; // 0.05%

        private static Vector2? dragOffset = null;
        private static Texture2D edgeTexture, barTexture;

        internal static void Load()
        {
            edgeTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/ChargeMeterBorder", AssetRequestMode.ImmediateLoad).Value;
            barTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/ChargeMeter", AssetRequestMode.ImmediateLoad).Value;
            Reset();
        }

        internal static void Unload()
        {
            Reset();
            edgeTexture = barTexture = null;
        }

        private static void Reset() => dragOffset = null;

        public static void Draw(SpriteBatch spriteBatch, Player player)
        {
            // Sanity check the planned position before drawing
            Vector2 screenRatioPosition = new Vector2(CalamityConfig.Instance.ChargeMeterPosX, CalamityConfig.Instance.ChargeMeterPosY);
            if (screenRatioPosition.X < 0f || screenRatioPosition.X > 100f)
                screenRatioPosition.X = DefaultChargePosX;
            if (screenRatioPosition.Y < 0f || screenRatioPosition.Y > 100f)
                screenRatioPosition.Y = DefaultChargePosY;

            // Convert the screen ratio position to an absolute position in pixels
            // Cast to integer to prevent blurriness which results from decimal pixel positions
            float uiScale = Main.UIScale;
            Vector2 screenPos = screenRatioPosition;
            screenPos.X = (int)(screenPos.X * 0.01f * Main.screenWidth);
            screenPos.Y = (int)(screenPos.Y * 0.01f * Main.screenHeight);

            void SavePosition()
            {
                bool changed = false;
                if (CalamityConfig.Instance.ChargeMeterPosX != screenRatioPosition.X)
                {
                    CalamityConfig.Instance.ChargeMeterPosX = screenRatioPosition.X;
                    changed = true;
                }
                if (CalamityConfig.Instance.ChargeMeterPosY != screenRatioPosition.Y)
                {
                    CalamityConfig.Instance.ChargeMeterPosY = screenRatioPosition.Y;
                    changed = true;
                }
                if (changed)
                    CalamityMod.SaveConfig(CalamityConfig.Instance);
            }

            // If the Charge Meter is turned off or the player is not holding an item, stop.
            Item heldItem = player.ActiveItem();
            if (!CalamityConfig.Instance.ChargeMeter || heldItem is null || heldItem.IsAir)
            {
                Reset();
                SavePosition();
                return;
            }

            // If the player is not holding an item which uses charge, stop.
            CalamityGlobalItem modItem = heldItem.Calamity();
            if (!(modItem?.UsesCharge ?? false))
            {
                Reset();
                SavePosition();
                return;
            }

            float offset = (edgeTexture.Width - barTexture.Width) * 0.5f;
            spriteBatch.Draw(edgeTexture, screenPos, null, Color.White, 0f, edgeTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);

            Rectangle barRectangle = new Rectangle(0, 0, (int)(barTexture.Width * modItem.ChargeRatio), barTexture.Width);
            spriteBatch.Draw(barTexture, screenPos + new Vector2(offset * uiScale, 0), barRectangle, Color.White, 0f, edgeTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);

            Rectangle mouseHitbox = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
            Rectangle chargeBar = Utils.CenteredRectangle(screenPos, edgeTexture.Size() * uiScale);

            // If the mouse is on top of the meter, show the exact numeric charge.
            if (chargeBar.Intersects(mouseHitbox))
            {
                Main.LocalPlayer.mouseInterface = true;
                string percentString = (100f * modItem.ChargeRatio).ToString("n2");
                // Tooltip only goes to one decimal place, but the meter displays two. This doesn't help the player much at all but hey, it's a thing.
                Main.instance.MouseText($"Current Charge: {percentString}%", 0, 0, -1, -1, -1, -1);
            }

            // Handle mouse dragging
            if (!CalamityConfig.Instance.MeterPosLock)
            {
                Vector2 newScreenRatioPosition = screenRatioPosition;
                if (chargeBar.Intersects(mouseHitbox))
                {
                    MouseState ms = Mouse.GetState();
                    Vector2 mousePos = new Vector2(ms.X, ms.Y);

                    // As long as the mouse button is held down, drag the meter along with an offset.
                    if (ms.LeftButton == ButtonState.Pressed)
                    {
                        // If the drag offset doesn't exist yet, create it.
                        if (!dragOffset.HasValue)
                            dragOffset = mousePos - screenPos;

                        // Given the mouse's absolute current position, compute where the corner of the stealth bar should be based on the original drag offset.
                        Vector2 newCorner = mousePos - dragOffset.GetValueOrDefault(Vector2.Zero);

                        // Convert the new corner position into a screen ratio position.
                        newScreenRatioPosition.X = (100f * newCorner.X) / Main.screenWidth;
                        newScreenRatioPosition.Y = (100f * newCorner.Y) / Main.screenHeight;
                    }

                    // Compute the change in position. If it is large enough, actually move the meter
                    Vector2 delta = newScreenRatioPosition - screenRatioPosition;
                    if (Math.Abs(delta.X) >= MouseDragEpsilon || Math.Abs(delta.Y) >= MouseDragEpsilon)
                    {
                        CalamityConfig.Instance.ChargeMeterPosX = newScreenRatioPosition.X;
                        CalamityConfig.Instance.ChargeMeterPosY = newScreenRatioPosition.Y;
                    }

                    // When the mouse is released, save the config and destroy the drag offset.
                    if (ms.LeftButton == ButtonState.Released)
                    {
                        dragOffset = null;
                        CalamityMod.SaveConfig(CalamityConfig.Instance);
                    }
                }
            }
        }
    }
}
