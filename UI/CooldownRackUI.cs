using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace CalamityMod.UI
{
    public class CooldownRackUI
    {
        /// <summary>
        /// The maximum number of cooldowns which can be drawn in expanded mode before the rack auto-switches to compact mode.
        /// </summary>
        public static int MaxLargeIcons = 10;

        public static bool CompactIcons
        {
            get
            {
                // Option 1: Always use compact icons if configured to do so.
                if (CalamityConfig.Instance.CooldownDisplay == 1)
                    return true;

                // Option 2: If there are too many cooldowns, auto switch to compact mode.
                return Main.LocalPlayer.GetDisplayedCooldowns().Count > MaxLargeIcons;
            }
        }

        // TODO -- more of these display positioning constants
        public const float CompactXSpacing = 28f;
        public const float ExpandedXScaling = 46f;
        public static Vector2 Spacing => CompactIcons ? Vector2.UnitX * CompactXSpacing : Vector2.UnitX * ExpandedXScaling;

        public static Vector2 BaseDrawPosition => new Vector2(32, 100) + Spacing / 2f + (Main.LocalPlayer.CountBuffs() > 0 ? Vector2.UnitY * 50 : Vector2.Zero) + (Main.LocalPlayer.CountBuffs() > 11 ? Vector2.UnitY * 50  : Vector2.Zero);

        public static bool DebugFullDisplay = false;
        public static float DebugForceCompletion = 0f;

        public static void Draw(SpriteBatch spriteBatch)
        {
            // Don't draw cooldowns under the following conditions:
            // 1 - The game isn't even on the game screen yet.
            // 2 - The player's inventory is open.
            // 3 - Cooldown display is completely disabled.
            if (Main.gameMenu || Main.playerInventory || CalamityConfig.Instance.CooldownDisplay < 1)
                return;

            IList<CooldownInstance> cooldownsToDraw = Main.LocalPlayer.GetDisplayedCooldowns();
            if (cooldownsToDraw.Count == 0)
                return;

            float uiScale = 1f; //The actual UI scale is automatically applied. Using Main.UIScale here will make everyting way bigger due to the scale being applied twice over
            Vector2 displayPosition = BaseDrawPosition;
            int rectangleSide = (int)Math.Floor(CompactIcons ? 24 * uiScale : 52 * uiScale);
            Rectangle iconRectangle = new Rectangle((int)displayPosition.X - rectangleSide / 2, (int)displayPosition.Y - rectangleSide / 2, rectangleSide, rectangleSide);
            Rectangle mouse = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);

            string mouseHover = "";
            float iconOpacityScale = (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.1f + 0.6f;
            Vector2 mouseCenter = mouse.Center.ToVector2();
            float opacity = MathHelper.Clamp((float)Math.Sin(Main.GlobalTimeWrappedHourly % MathHelper.Pi) * 2f, 0, 1) * 0.1f + 0.9f;

            foreach (CooldownInstance instance in cooldownsToDraw)
            {
                CooldownHandler handler = instance.handler;
                float iconOpacity = iconOpacityScale;

                // Icons get brighter if the mouse gets closer
                iconOpacity += 0.3f * (1 - MathHelper.Clamp(Vector2.Distance(mouseCenter, iconRectangle.Center.ToVector2()), 0f, 80f) / 80f);

                if (iconRectangle.Intersects(mouse))
                {
                    mouseHover = handler.DisplayName.ToString();
                    iconOpacity = opacity;
                }

                if (DebugFullDisplay)
                    iconOpacity = 1f;

                if (CompactIcons)
                    handler.DrawCompact(spriteBatch, displayPosition, iconOpacity, uiScale);
                else
                    handler.DrawExpanded(spriteBatch, displayPosition, iconOpacity, uiScale);

                displayPosition += Spacing;
                iconRectangle.X += (int)Spacing.X;
            }

            if (mouseHover != "")
            {
                Main.LocalPlayer.mouseInterface = true;
                Main.instance.MouseText(mouseHover);
            }

        }
    }
}
