using System;
using System.Collections.Generic;
using System.Text;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.UI.Rippers
{
    // TODO -- This can be made into a ModSystem with simple OnModLoad and Unload hooks.
    public static class RipperUI
    {
        // These values were handpicked on a 1080p screen by Ozzatron. Please disregard the bizarre precision.
        public const float DefaultRagePosX = 35.77406f;
        public const float DefaultRagePosY = 4.5761431f;
        public const float DefaultAdrenPosX = 35.77406f;
        public const float DefaultAdrenPosY = 8.846918f;
        private const float MouseDragEpsilon = 0.05f; // 0.05%

        private const int RageAnimFrameDelay = 6;
        private const int RageAnimFrames = 10;
        private const int AdrenAnimFrameDelay = 5;
        private const int AdrenAnimFrames = 10;

        private static Vector2? rageDragOffset = null;
        private static Vector2? adrenDragOffset = null;
        private static Vector2 pearlOffsetLeft = Vector2.Zero;
        private static Vector2 pearlOffsetCenter = Vector2.Zero;
        private static Vector2 pearlOffsetRight = Vector2.Zero;
        private static Vector2 PearlOffsetCenterLeft => 0.5f * (pearlOffsetLeft + pearlOffsetCenter);
        private static Vector2 PearlOffsetCenterRight => 0.5f * (pearlOffsetRight + pearlOffsetCenter);

        private static int rageAnimFrame = -1;
        private static int rageAnimTimer = 0;
        private static int adrenAnimFrame = -1;
        private static int adrenAnimTimer = 0;

        private const int AdrenBarFrames = 12;
        private const int AdrenBarFullFrames = 6;
        private const int AdrenBarFrameDelay = 5;
        private static int adrenBarFrame = -1;
        private static int adrenBarFullFrame = -1;
        private static int adrenBarTimer = 0;

        private static Texture2D rageBarTex, rageBorderTex, rageAnimTex;
        private static Texture2D mushroomPlasmaTex, infernalBloodTex, redLightningTex;
        private static Texture2D adrenBarTex, adrenBorderTex, adrenBorderTexFull, adrenAnimTex, draedonBarTex, draedonAnimTex;
        private static Texture2D electrolyteGelTex, starlightFuelTex, ectoheartTex;

        internal static void Load()
        {
            rageBarTex = ModContent.Request<Texture2D>("CalamityMod/UI/Rippers/RageBar", AssetRequestMode.ImmediateLoad).Value;
            rageBorderTex = ModContent.Request<Texture2D>("CalamityMod/UI/Rippers/RageBarBorder", AssetRequestMode.ImmediateLoad).Value;
            rageAnimTex = ModContent.Request<Texture2D>("CalamityMod/UI/Rippers/RageFullAnimation", AssetRequestMode.ImmediateLoad).Value;

            adrenBarTex = ModContent.Request<Texture2D>("CalamityMod/UI/Rippers/AdrenalineBar", AssetRequestMode.ImmediateLoad).Value;
            adrenBorderTex = ModContent.Request<Texture2D>("CalamityMod/UI/Rippers/AdrenalineBarBorder", AssetRequestMode.ImmediateLoad).Value;
            adrenBorderTexFull = ModContent.Request<Texture2D>("CalamityMod/UI/Rippers/AdrenalineBarBorderFull", AssetRequestMode.ImmediateLoad).Value;
            adrenAnimTex = ModContent.Request<Texture2D>("CalamityMod/UI/Rippers/AdrenalineFullAnimation", AssetRequestMode.ImmediateLoad).Value;
            draedonBarTex = ModContent.Request<Texture2D>("CalamityMod/UI/Rippers/DraedonAdrenalineBar", AssetRequestMode.ImmediateLoad).Value;
            draedonAnimTex = ModContent.Request<Texture2D>("CalamityMod/UI/Rippers/DraedonAdrenalineFullAnimation", AssetRequestMode.ImmediateLoad).Value;

            mushroomPlasmaTex = ModContent.Request<Texture2D>("CalamityMod/UI/Rippers/RageDisplay_MushroomPlasmaRoot", AssetRequestMode.ImmediateLoad).Value;
            infernalBloodTex = ModContent.Request<Texture2D>("CalamityMod/UI/Rippers/RageDisplay_InfernalBlood", AssetRequestMode.ImmediateLoad).Value;
            redLightningTex = ModContent.Request<Texture2D>("CalamityMod/UI/Rippers/RageDisplay_RedLightningContainer", AssetRequestMode.ImmediateLoad).Value;

            electrolyteGelTex = ModContent.Request<Texture2D>("CalamityMod/UI/Rippers/AdrenalineDisplay_ElectrolyteGelPack", AssetRequestMode.ImmediateLoad).Value;
            starlightFuelTex = ModContent.Request<Texture2D>("CalamityMod/UI/Rippers/AdrenalineDisplay_StarlightFuelCell", AssetRequestMode.ImmediateLoad).Value;
            ectoheartTex = ModContent.Request<Texture2D>("CalamityMod/UI/Rippers/AdrenalineDisplay_Ectoheart", AssetRequestMode.ImmediateLoad).Value;

            pearlOffsetLeft = new Vector2(rageBorderTex.Width * 0.3333f - 6f, rageBorderTex.Height - 9f);
            pearlOffsetCenter = new Vector2(rageBorderTex.Width * 0.5f - 6f, rageBorderTex.Height - 9f);
            pearlOffsetRight = new Vector2(rageBorderTex.Width * 0.6667f - 6f, rageBorderTex.Height - 9f);

            Reset();
        }

        internal static void Unload()
        {
            Reset();
            rageBarTex = rageBorderTex = rageAnimTex = null;
            adrenBarTex = adrenBorderTex = adrenBorderTexFull = adrenAnimTex = draedonBarTex = draedonAnimTex = null;
            mushroomPlasmaTex = infernalBloodTex = redLightningTex = null;
            electrolyteGelTex = starlightFuelTex = ectoheartTex = null;
            pearlOffsetLeft = pearlOffsetCenter = pearlOffsetRight = Vector2.Zero;
        }

        internal static void Reset()
        {
            rageDragOffset = null;
            adrenDragOffset = null;
            rageAnimFrame = -1;
            rageAnimTimer = 0;
            adrenAnimFrame = -1;
            adrenAnimTimer = 0;
            adrenBarFrame = -1;
            adrenBarFullFrame = -1;
            adrenBarTimer = 0;
        }

        public static void Draw(SpriteBatch spriteBatch, Player player)
        {
            // Sanity check the planned Rage Meter position
            Vector2 rageScreenRatioPos = new Vector2(CalamityConfig.Instance.RageMeterPosX, CalamityConfig.Instance.RageMeterPosY);
            if (rageScreenRatioPos.X < 0f || rageScreenRatioPos.X > 100f)
                rageScreenRatioPos.X = DefaultRagePosX;
            if (rageScreenRatioPos.Y < 0f || rageScreenRatioPos.Y > 100f)
                rageScreenRatioPos.Y = DefaultRagePosY;

            // Sanity check the planned Adrenaline Meter position
            Vector2 adrenScreenRatioPos = new Vector2(CalamityConfig.Instance.AdrenalineMeterPosX, CalamityConfig.Instance.AdrenalineMeterPosY);
            if (adrenScreenRatioPos.X < 0f || adrenScreenRatioPos.X > 100f)
                adrenScreenRatioPos.X = DefaultAdrenPosX;
            if (adrenScreenRatioPos.Y < 0f || adrenScreenRatioPos.Y > 100f)
                adrenScreenRatioPos.Y = DefaultAdrenPosY;

            // Convert the screen ratio position to an absolute position in pixels
            // Cast to integer to prevent blurriness which results from decimal pixel positions
            Vector2 rageScreenPos = rageScreenRatioPos;
            rageScreenPos.X = (int)(rageScreenPos.X * 0.01f * Main.screenWidth);
            rageScreenPos.Y = (int)(rageScreenPos.Y * 0.01f * Main.screenHeight);
            Vector2 adrenScreenPos = adrenScreenRatioPos;
            adrenScreenPos.X = (int)(adrenScreenPos.X * 0.01f * Main.screenWidth);
            adrenScreenPos.Y = (int)(adrenScreenPos.Y * 0.01f * Main.screenHeight);

            // Grab the ModPlayer object and draw if applicable. If not applicable, save positions to config.
            CalamityPlayer modPlayer = player.Calamity();

            if (modPlayer.RageEnabled)
                DrawRageBar(spriteBatch, modPlayer, rageScreenPos);
            else
            {
                bool changed = false;
                if (CalamityConfig.Instance.RageMeterPosX != rageScreenRatioPos.X)
                {
                    CalamityConfig.Instance.RageMeterPosX = rageScreenRatioPos.X;
                    changed = true;
                }
                if (CalamityConfig.Instance.RageMeterPosY != rageScreenRatioPos.Y)
                {
                    CalamityConfig.Instance.RageMeterPosY = rageScreenRatioPos.Y;
                    changed = true;
                }
                if (changed)
                    CalamityMod.SaveConfig(CalamityConfig.Instance);
            }

            if (modPlayer.AdrenalineEnabled)
                DrawAdrenalineBar(spriteBatch, modPlayer, adrenScreenPos);
            else
            {
                bool changed = false;
                if (CalamityConfig.Instance.AdrenalineMeterPosX != adrenScreenRatioPos.X)
                {
                    CalamityConfig.Instance.AdrenalineMeterPosX = adrenScreenRatioPos.X;
                    changed = true;
                }
                if (CalamityConfig.Instance.AdrenalineMeterPosY != adrenScreenRatioPos.Y)
                {
                    CalamityConfig.Instance.AdrenalineMeterPosY = adrenScreenRatioPos.Y;
                    changed = true;
                }
                if (changed)
                    CalamityMod.SaveConfig(CalamityConfig.Instance);
            }

            #region Mouse Interaction
            float uiScale = Main.UIScale;
            Rectangle mouseHitbox = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);

            bool useFullTexture = modPlayer.adrenaline >= modPlayer.adrenalineMax || modPlayer.adrenalineModeActive;
            Vector2 adrenSize = new Vector2(adrenBorderTex.Width, adrenBorderTex.Height / AdrenBarFrames);
            if (useFullTexture)
                adrenSize = new Vector2(adrenBorderTexFull.Width, adrenBorderTexFull.Height / AdrenBarFullFrames);

            Rectangle rageBar = Utils.CenteredRectangle(rageScreenPos, rageBorderTex.Size() * uiScale);
            Rectangle adrenBar = Utils.CenteredRectangle(adrenScreenPos, adrenSize * uiScale);

            bool rageHover = mouseHitbox.Intersects(rageBar) && modPlayer.RageEnabled;
            bool adrenHover = mouseHitbox.Intersects(adrenBar) && modPlayer.AdrenalineEnabled;

            MouseState ms = Mouse.GetState();
            Vector2 mousePos = Main.MouseScreen;

            // Rage bar takes priority, and only one can be hovered at a time
            if (rageHover && !adrenHover)
            {
                // If the meter isn't locked, then the player's mouse counts as being over interface
                if (!CalamityConfig.Instance.MeterPosLock)
                    Main.LocalPlayer.mouseInterface = true;

                // Add hover text if the mouse is over Rage bar
                string rageStr = MakeRipperPercentString(modPlayer.rage, modPlayer.rageMax);
                Main.instance.MouseText($"{CalamityUtils.GetTextValue("UI.Rage")}: {rageStr}");

                // The bar is draggable if enabled in config.
                Vector2 newScreenRatioPosition = rageScreenRatioPos;
                if (!CalamityConfig.Instance.MeterPosLock && ms.LeftButton == ButtonState.Pressed)
                {
                    // If the drag offset doesn't exist yet, create it.
                    if (!rageDragOffset.HasValue)
                        rageDragOffset = mousePos - rageScreenPos;

                    // Given the mouse's absolute current position, compute where the corner of the stealth bar should be based on the original drag offset.
                    Vector2 newCorner = mousePos - rageDragOffset.GetValueOrDefault(Vector2.Zero);

                    // Convert the new corner position into a screen ratio position.
                    newScreenRatioPosition.X = (100f * newCorner.X) / Main.screenWidth;
                    newScreenRatioPosition.Y = (100f * newCorner.Y) / Main.screenHeight;
                }

                // Compute the change in position. If it is large enough, actually move the meter
                Vector2 delta = newScreenRatioPosition - rageScreenRatioPos;
                if (Math.Abs(delta.X) >= MouseDragEpsilon || Math.Abs(delta.Y) >= MouseDragEpsilon)
                {
                    CalamityConfig.Instance.RageMeterPosX = newScreenRatioPosition.X;
                    CalamityConfig.Instance.RageMeterPosY = newScreenRatioPosition.Y;
                }

                // When the mouse is released, save the config and destroy the drag offset.
                if (ms.LeftButton == ButtonState.Released)
                {
                    rageDragOffset = null;
                    CalamityMod.SaveConfig(CalamityConfig.Instance);
                }
            }
            else if (adrenHover)
            {
                // If the meter isn't locked, then the player's mouse counts as being over interface
                if (!CalamityConfig.Instance.MeterPosLock)
                    Main.LocalPlayer.mouseInterface = true;

                // Add hover text if the mouse is over the bar
                string adrenNameStr = CalamityUtils.GetTextValue("UI." + (modPlayer.draedonsHeart ? "Nanomachines" : "Adrenaline"));
                string adrenAmountStr = MakeRipperPercentString(modPlayer.adrenaline, modPlayer.adrenalineMax);
                Main.instance.MouseText($"{adrenNameStr}: {adrenAmountStr}");

                // The bar is draggable if enabled in config.
                Vector2 newScreenRatioPosition = adrenScreenRatioPos;
                if (!CalamityConfig.Instance.MeterPosLock && ms.LeftButton == ButtonState.Pressed)
                {
                    // If the drag offset doesn't exist yet, create it.
                    if (!adrenDragOffset.HasValue)
                        adrenDragOffset = mousePos - adrenScreenPos;

                    // Given the mouse's absolute current position, compute where the corner of the stealth bar should be based on the original drag offset.
                    Vector2 newCorner = mousePos - adrenDragOffset.GetValueOrDefault(Vector2.Zero);

                    // Convert the new corner position into a screen ratio position.
                    newScreenRatioPosition.X = (100f * newCorner.X) / Main.screenWidth;
                    newScreenRatioPosition.Y = (100f * newCorner.Y) / Main.screenHeight;
                }

                // Compute the change in position. If it is large enough, actually move the meter
                Vector2 delta = newScreenRatioPosition - adrenScreenRatioPos;
                if (Math.Abs(delta.X) >= MouseDragEpsilon || Math.Abs(delta.Y) >= MouseDragEpsilon)
                {
                    CalamityConfig.Instance.AdrenalineMeterPosX = newScreenRatioPosition.X;
                    CalamityConfig.Instance.AdrenalineMeterPosY = newScreenRatioPosition.Y;
                }

                // When the mouse is released, save the config and destroy the drag offset.
                if (ms.LeftButton == ButtonState.Released)
                {
                    adrenDragOffset = null;
                    CalamityMod.SaveConfig(CalamityConfig.Instance);
                }
            }
            #endregion
        }

        #region Draw Rage Bar
        private static void DrawRageBar(SpriteBatch spriteBatch, CalamityPlayer modPlayer, Vector2 screenPos)
        {
            float uiScale = Main.UIScale;
            Vector2 shakeOffset = modPlayer.rageModeActive ? GetShakeOffset() : Vector2.Zero;

            // If rage is full this frame and the animation hasn't started yet, start it.
            float rageRatio = modPlayer.rage / modPlayer.rageMax;
            if (rageRatio >= 1f && rageAnimFrame == -1)
                rageAnimFrame = 0;

            // If the animation has already finished and rage isn't full anymore, reset it.
            else if (rageRatio < 1f && rageAnimFrame == RageAnimFrames)
                rageAnimFrame = -1;

            // Otherwise, the animation runs to completion even if the user activates rage in the middle of it.
            bool animationActive = rageAnimFrame >= 0 && rageAnimFrame < RageAnimFrames;
            if (animationActive)
            {
                rageAnimTimer++;
                if(rageAnimTimer >= RageAnimFrameDelay)
                {
                    rageAnimTimer = 0;
                    rageAnimFrame++; // This will eventually increment it to RageAnimFrames, thus stopping the animation.
                }
            }

            // Draw the border of the Rage Bar first
            spriteBatch.Draw(rageBorderTex, screenPos + shakeOffset, null, Color.White, 0f, rageBorderTex.Size() * 0.5f, uiScale, SpriteEffects.None, 0);

            // The amount of the bar to draw depends on the player's current Rage level
            // offset calculates the deadspace that is the border and not the bar. Bar is 24 pixels tall
            int barWidth = rageBarTex.Width;
            float offset = (rageBorderTex.Width - rageBarTex.Width) * 0.5f;
            Rectangle cropRect = new Rectangle(0, 0, (int)(barWidth * rageRatio), rageBarTex.Height);
            spriteBatch.Draw(rageBarTex, screenPos + shakeOffset + new Vector2(offset * uiScale, 0), cropRect, Color.White, 0f, rageBorderTex.Size() * 0.5f, uiScale, SpriteEffects.None, 0);

            // Determine which pearls to draw (and their positions) based off of which Rage upgrades the player has.
            IList<Texture2D> pearls = new List<Texture2D>(3);
            if (modPlayer.rageBoostOne) // Mushroom Plasma Root
                pearls.Add(mushroomPlasmaTex);
            if (modPlayer.rageBoostTwo) // Infernal Blood
                pearls.Add(infernalBloodTex);
            if (modPlayer.rageBoostThree) // Red Lightning Container
                pearls.Add(redLightningTex);
            IList<Vector2> offsets = GetPearlOffsets(pearls.Count);

            // Draw pearls at appropriate positions.
            for (int i = 0; i < pearls.Count; ++i)
                spriteBatch.Draw(pearls[i], screenPos + shakeOffset + offsets[i] * uiScale, null, Color.White, 0f, rageBorderTex.Size() * 0.5f, uiScale, SpriteEffects.None, 0);

            // If the animation is active, draw the animation on top of both the border and the bar.
            if (animationActive)
            {
                float xOffset = (rageBorderTex.Width - rageAnimTex.Width) / 2f;
                int frameHeight = (rageAnimTex.Height / RageAnimFrames) - 1;
                float yOffset = (rageBorderTex.Height - frameHeight) / 2f;
                Vector2 sizeDiffOffset = new Vector2(xOffset, yOffset);
                Rectangle animCropRect = new Rectangle(0, (frameHeight + 1) * rageAnimFrame, rageAnimTex.Width, frameHeight);
                spriteBatch.Draw(rageAnimTex, screenPos + shakeOffset + sizeDiffOffset, animCropRect, Color.White, 0f, rageBorderTex.Size() * 0.5f, uiScale, SpriteEffects.None, 0);
            }
        }
        #endregion

        #region Draw Adrenaline Bar
        private static void DrawAdrenalineBar(SpriteBatch spriteBatch, CalamityPlayer modPlayer, Vector2 screenPos)
        {
			bool draedonHeart = modPlayer.draedonsHeart;
            bool useFullTexture = modPlayer.adrenaline >= modPlayer.adrenalineMax || modPlayer.adrenalineModeActive;

            float uiScale = Main.UIScale;
            Vector2 shakeOffset = modPlayer.adrenalineModeActive ? GetShakeOffset() : Vector2.Zero;

            Vector2 origin = new Vector2(adrenBorderTex.Width * 0.5f, (adrenBorderTex.Height / AdrenBarFrames) * 0.5f);
            if (useFullTexture)
                origin = new Vector2(adrenBorderTexFull.Width * 0.5f, (adrenBorderTexFull.Height / AdrenBarFullFrames) * 0.5f);

            if (draedonHeart)
            {
                adrenBarTimer++;
                if (adrenBarTimer >= AdrenBarFrameDelay)
                {
                    adrenBarTimer = 0;
                    adrenBarFrame++;
                    adrenBarFullFrame++;
					if (adrenBarFrame == AdrenBarFrames)
						adrenBarFrame = 1;
					if (adrenBarFullFrame == AdrenBarFullFrames)
						adrenBarFullFrame = 1;
                }
            }
            else
			{
				adrenBarTimer = 0;
				adrenBarFrame = 0;
				adrenBarFullFrame = 0;
			}

            // If adrenaline is full this frame and the animation hasn't started yet, start it.
            float adrenRatio = modPlayer.adrenaline / modPlayer.adrenalineMax;
            if (adrenRatio >= 1f && adrenAnimFrame == -1)
                adrenAnimFrame = 0;

            // If the animation has already finished and adrenaline isn't full anymore, reset it.
            else if (adrenRatio < 1f && adrenAnimFrame == AdrenAnimFrames)
                adrenAnimFrame = -1;

            // Otherwise, the animation runs to completion even if the user activates adrenaline in the middle of it.
            bool animationActive = adrenAnimFrame >= 0 && adrenAnimFrame < AdrenAnimFrames;
            if (animationActive)
            {
                adrenAnimTimer++;
                if (adrenAnimTimer >= AdrenAnimFrameDelay)
                {
                    adrenAnimTimer = 0;
                    adrenAnimFrame++; // This will eventually increment it to AdrenAnimFrames, thus stopping the animation.
                }
            }

            if (!useFullTexture)
			{
				int frameHeight = (adrenBorderTex.Height / AdrenBarFrames) - 1;
				Rectangle borderRect = new Rectangle(0, (frameHeight + 1) * adrenBarFrame, adrenBorderTex.Width, frameHeight);
				// Draw the border of the Adrenaline Bar first
				spriteBatch.Draw(adrenBorderTex, screenPos + shakeOffset, borderRect, Color.White, 0f, origin, uiScale, SpriteEffects.None, 0);
			}
            else
			{
                // Use a slightly different texture if Adrenaline is full or active
				int frameHeight = (adrenBorderTexFull.Height / AdrenBarFullFrames) - 1;
				Rectangle borderRect = new Rectangle(0, (frameHeight + 1) * adrenBarFullFrame, adrenBorderTexFull.Width, frameHeight);

                spriteBatch.Draw(adrenBorderTexFull, screenPos + shakeOffset, borderRect, Color.White, 0f, origin, uiScale, SpriteEffects.None, 0);
			}

            // The amount of the bar to draw depends on the player's current Adrenaline level
            // offset calculates the deadspace that is the border and not the bar. Bar is 24 pixels tall
            int barWidth = adrenBarTex.Width;
            float offset = (adrenBorderTex.Width - adrenBarTex.Width) * 0.5f;
            Rectangle cropRect = new Rectangle(0, 0, (int)(barWidth * adrenRatio), adrenBarTex.Height);
            spriteBatch.Draw(draedonHeart ? draedonBarTex : adrenBarTex, screenPos + shakeOffset + new Vector2(offset * uiScale, 2f), cropRect, Color.White, 0f, origin, uiScale, SpriteEffects.None, 0);

            // Determine which pearls to draw (and their positions) based off of which Adrenaline upgrades the player has.
            IList<Texture2D> pearls = new List<Texture2D>(3);
            if (modPlayer.adrenalineBoostOne) // Electrolyte Gel Pack
                pearls.Add(electrolyteGelTex);
            if (modPlayer.adrenalineBoostTwo) // Starlight Fuel Cell
                pearls.Add(starlightFuelTex);
            if (modPlayer.adrenalineBoostThree) // Ectoheart
                pearls.Add(ectoheartTex);
            IList<Vector2> offsets = GetPearlOffsets(pearls.Count);

            // Draw pearls at appropriate positions.
            for (int i = 0; i < pearls.Count; ++i)
                spriteBatch.Draw(pearls[i], screenPos + shakeOffset + offsets[i] * uiScale + new Vector2(0, 5f), null, Color.White, 0f, origin, uiScale, SpriteEffects.None, 0);

            // If the animation is active, draw the animation on top of both the border and the bar.
            if (animationActive)
            {
                float animOffset = 5f;
                float xOffset = (adrenBorderTex.Width - adrenAnimTex.Width) / 2f;
                int frameHeight = (adrenAnimTex.Height / AdrenAnimFrames) - 1;
                float yOffset = ((adrenBorderTex.Height / AdrenBarFrames) - frameHeight) / 2f + animOffset;
                if (useFullTexture)
                    yOffset = ((adrenBorderTexFull.Height / AdrenBarFullFrames) - frameHeight) / 2f + animOffset;
                Vector2 sizeDiffOffset = new Vector2(xOffset, yOffset);
                Rectangle animCropRect = new Rectangle(0, (frameHeight + 1) * adrenAnimFrame, adrenAnimTex.Width, frameHeight);
                spriteBatch.Draw(draedonHeart ? draedonAnimTex : adrenAnimTex, screenPos + shakeOffset + sizeDiffOffset, animCropRect, Color.White, 0f, origin, uiScale, SpriteEffects.None, 0);
            }
        }
        #endregion
        private static Vector2 GetShakeOffset()
        {
            float shake = CalamityConfig.Instance.RipperMeterShake;
            float shakeX = Main.rand.NextFloat(-shake, shake);
            float shakeY = Main.rand.NextFloat(-shake, shake);
            return new Vector2(shakeX, shakeY);
        }

        private static IList<Vector2> GetPearlOffsets(int count) => count switch
        {
            1 => new List<Vector2>() { pearlOffsetCenter },
            2 => new List<Vector2>() { PearlOffsetCenterLeft, PearlOffsetCenterRight },
            3 => new List<Vector2>() { pearlOffsetLeft, pearlOffsetCenter, pearlOffsetRight },
            _ => null,
        };

        private static string MakeRipperPercentString(float value, float maxValue)
        {
            string topTwoDecimalPlaces = value.ToString("n2");
            string bottomTwoDecimalPlaces = maxValue.ToString("n2");
            string percent = (100f * value / maxValue).ToString("0.00");
            StringBuilder sb = new StringBuilder(32);
            sb.Append(percent);
            sb.Append("% (");
            sb.Append(topTwoDecimalPlaces);
            sb.Append(" / ");
            sb.Append(bottomTwoDecimalPlaces);
            sb.Append(')');
            return sb.ToString();
        }
    }
}
