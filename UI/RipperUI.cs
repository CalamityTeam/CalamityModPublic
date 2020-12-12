using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.World;
using Microsoft.Xna.Framework.Input;

namespace CalamityMod.UI
{
	public class RipperUI
    {
        public const float DefaultRagePosX = 500f;
        public const float DefaultRagePosY = 30f;
        public const float DefaultAdrenPosX = 650f;
        public const float DefaultAdrenPosY = 30f;

        private const int RageAnimFrameDelay = 6;
        private const int RageAnimFrames = 9;
        private const int AdrenAnimFrameDelay = 5;
        private const int AdrenAnimFrames = 9;
        
        public static Vector2 rageDrawPos = new Vector2(DefaultRagePosX, DefaultRagePosY);
        public static Vector2 adrenDrawPos = new Vector2(DefaultAdrenPosX, DefaultAdrenPosY);
        private static Vector2? rageDragOffset = null;
        private static Vector2? adrenDragOffset = null;

        private static int rageAnimFrame = -1;
        private static int rageAnimTimer = 0;
        private static int adrenAnimFrame = -1;
        private static int adrenAnimTimer = 0;

        public static void Reset()
        {
            bool configExists = CalamityConfig.Instance != null;

            rageDrawPos = configExists ? new Vector2(CalamityConfig.Instance.RageMeterPosX, CalamityConfig.Instance.RageMeterPosY) : Vector2.Zero;
            adrenDrawPos = configExists ? new Vector2(CalamityConfig.Instance.AdrenalineMeterPosX, CalamityConfig.Instance.AdrenalineMeterPosY) : Vector2.Zero;
            rageDragOffset = null;
            adrenDragOffset = null;
            rageAnimFrame = -1;
            rageAnimTimer = 0;
            adrenAnimFrame = -1;
            adrenAnimTimer = 0;
        }

        public static void Draw(SpriteBatch spriteBatch, Player player)
        {
            // If Revengeance isn't on, or Rage and Adrenaline are turned off, don't draw anything.
            if (!CalamityWorld.revenge || !CalamityConfig.Instance.Rippers)
                return;

            // If for some reason either of the bars has been thrown into the corner (likely to 0,0 by default), put them at their default positions
            CheckGarbageCornerPos();

            // Prevent blurriness which results from decimals in the position variables.
            rageDrawPos.X = (int)rageDrawPos.X;
            rageDrawPos.Y = (int)rageDrawPos.Y;
            adrenDrawPos.X = (int)adrenDrawPos.X;
            adrenDrawPos.Y = (int)adrenDrawPos.Y;

            // Grab all the textures and the player object and then start drawing
            Texture2D rageBarTex = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/RageBar");
            Texture2D rageBorderTex = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/RageBarBorder");
            Texture2D rageAnimTex = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/RageFullAnimation");
            Texture2D adrenBarTex = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/AdrenalineBar");
            Texture2D adrenBorderTex = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/AdrenalineBarBorder");
            Texture2D adrenAnimTex = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/AdrenalineFullAnimation");
            CalamityPlayer modPlayer = player.Calamity();
            DrawRage(spriteBatch, modPlayer, rageBarTex, rageBorderTex, rageAnimTex);
            DrawAdrenaline(spriteBatch, modPlayer, adrenBarTex, adrenBorderTex, adrenAnimTex);

            HandleMouseInteraction(modPlayer, rageBorderTex.Size(), adrenBorderTex.Size());
        }

        private static void DrawRage(SpriteBatch spriteBatch, CalamityPlayer modPlayer, Texture2D barTex, Texture2D borderTex, Texture2D animTex)
        {
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

			float uiScale = Main.UIScale;

            // Draw the border of the Rage Bar first
            spriteBatch.Draw(borderTex, rageDrawPos + shakeOffset, null, Color.White, 0f, borderTex.Size() * 0.5f, uiScale, SpriteEffects.None, 0);

            // The amount of the bar to draw depends on the player's current Rage level
            // 7 pixels of dead space, 90 pixels of bar, 7 pixels of dead space. Bar is 24 pixels tall
            int deadSpace = 7;
            int barWidth = barTex.Width - 2 * deadSpace;
            Rectangle cropRect = new Rectangle(0, 0, deadSpace + (int)(barWidth * rageRatio), barTex.Height);
            spriteBatch.Draw(barTex, rageDrawPos + shakeOffset, cropRect, Color.White, 0f, borderTex.Size() * 0.5f, uiScale, SpriteEffects.None, 0);

            // If the animation is active, draw the animation on top of both the border and the bar.
            if (animationActive)
            {
                float xOffset = (borderTex.Width - animTex.Width) / 2f;
                int frameHeight = (animTex.Height / RageAnimFrames) - 1;
                float yOffset = (borderTex.Height - frameHeight) / 2f;
                Vector2 sizeDiffOffset = new Vector2(xOffset, yOffset);
                Rectangle animCropRect = new Rectangle(0, (frameHeight + 1) * rageAnimFrame, animTex.Width, frameHeight);
                spriteBatch.Draw(animTex, rageDrawPos + shakeOffset + sizeDiffOffset, animCropRect, Color.White, 0f, borderTex.Size() * 0.5f, uiScale, SpriteEffects.None, 0);
            }
        }

        private static void DrawAdrenaline(SpriteBatch spriteBatch, CalamityPlayer modPlayer, Texture2D barTex, Texture2D borderTex, Texture2D animTex)
        {
            Vector2 shakeOffset = modPlayer.adrenalineModeActive ? GetShakeOffset() : Vector2.Zero;

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

			float uiScale = Main.UIScale;

            // Draw the border of the Adrenaline Bar first
            spriteBatch.Draw(borderTex, adrenDrawPos + shakeOffset, null, Color.White, 0f, borderTex.Size() * 0.5f, uiScale, SpriteEffects.None, 0);

            // The amount of the bar to draw depends on the player's current Adrenaline level
            // 7 pixels of dead space, 90 pixels of bar, 7 pixels of dead space.
            int deadSpace = 7;
            int barWidth = barTex.Width - 2 * deadSpace;
            Rectangle cropRect = new Rectangle(0, 0, deadSpace + (int)(barWidth * adrenRatio), barTex.Height);
            spriteBatch.Draw(barTex, adrenDrawPos + shakeOffset, cropRect, Color.White, 0f, borderTex.Size() * 0.5f, uiScale, SpriteEffects.None, 0);

            // If the animation is active, draw the animation on top of both the border and the bar.
            if (animationActive)
            {
                float xOffset = (borderTex.Width - animTex.Width) / 2f;
                int frameHeight = (animTex.Height / AdrenAnimFrames) - 1;
                float yOffset = (borderTex.Height - frameHeight) / 2f;
                Vector2 sizeDiffOffset = new Vector2(xOffset, yOffset);
                Rectangle animCropRect = new Rectangle(0, (frameHeight + 1) * adrenAnimFrame, animTex.Width, frameHeight);
                spriteBatch.Draw(animTex, adrenDrawPos + shakeOffset + sizeDiffOffset, animCropRect, Color.White, 0f, borderTex.Size() * 0.5f, uiScale, SpriteEffects.None, 0);
            }
        }

        private static void HandleMouseInteraction(CalamityPlayer modPlayer, Vector2 rageBarSize, Vector2 adrenBarSize)
        {
			float uiScale = Main.UIScale;
            Rectangle mouse = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
			Rectangle rageBar = Utils.CenteredRectangle(rageDrawPos, rageBarSize * uiScale);
			Rectangle adrenBar = Utils.CenteredRectangle(adrenDrawPos, adrenBarSize * uiScale);

            bool rageHover = mouse.Intersects(rageBar);
            bool adrenHover = mouse.Intersects(adrenBar);

            MouseState mouseInput = Mouse.GetState();
            Vector2 mousePos = Main.MouseScreen;

            // Rage bar takes priority, and only one can be hovered at a time
            if (rageHover && !adrenHover)
            {
                // Add hover text if the mouse is over Rage bar
                Main.LocalPlayer.mouseInterface = true;
                Main.instance.MouseText($"Rage: {(int)modPlayer.rage} / {(int)modPlayer.rageMax}");

                // The bar is draggable if enabled in config.
                if (!CalamityConfig.Instance.MeterPosLock && mouseInput.LeftButton == ButtonState.Pressed)
                {
                    // On the first frame new mouse input comes in, set the offset. Otherwise do nothing.
                    if (rageDragOffset is null)
                        rageDragOffset = rageDrawPos - mousePos;
                }
            }
            else if (adrenHover)
            {
                // Add hover text if the mouse is over the bar
                Main.LocalPlayer.mouseInterface = true;
                Main.instance.MouseText($"Adrenaline: {(int)modPlayer.adrenaline} / {(int)modPlayer.adrenalineMax}");

                // The bar is draggable if enabled in config.
                if (!CalamityConfig.Instance.MeterPosLock && mouseInput.LeftButton == ButtonState.Pressed)
                {
                    // On the first frame new mouse input comes in, set the offset. Otherwise do nothing.
                    if (adrenDragOffset is null)
                        adrenDragOffset = adrenDrawPos - mousePos;
                }
            }

            // If the rage bar is currently being dragged, move it to follow the mouse.
            if (rageDragOffset.HasValue)
                rageDrawPos = mousePos + rageDragOffset.GetValueOrDefault();

            // If the adrenaline bar is currently being dragged, move it to follow the mouse.
            if (adrenDragOffset.HasValue)
                adrenDrawPos = mousePos + adrenDragOffset.GetValueOrDefault();

            // Regardless of what is hovered, when the mouse is released, destroy the offset, and save the config.
            if (mouseInput.LeftButton == ButtonState.Released)
            {
                bool updateCfg = false;

                if (rageDragOffset.HasValue)
                {
                    rageDragOffset = null;
                    if (CalamityConfig.Instance.RageMeterPosX != rageDrawPos.X)
                    {
                        CalamityConfig.Instance.RageMeterPosX = rageDrawPos.X;
                        updateCfg = true;
                    }
                    if (CalamityConfig.Instance.RageMeterPosY != rageDrawPos.Y)
                    {
                        CalamityConfig.Instance.RageMeterPosY = rageDrawPos.Y;
                        updateCfg = true;
                    }
                }
                if (adrenDragOffset.HasValue)
                {
                    adrenDragOffset = null;
                    if (CalamityConfig.Instance.AdrenalineMeterPosX != adrenDrawPos.X)
                    {
                        CalamityConfig.Instance.AdrenalineMeterPosX = adrenDrawPos.X;
                        updateCfg = true;
                    }
                    if (CalamityConfig.Instance.AdrenalineMeterPosY != adrenDrawPos.Y)
                    {
                        CalamityConfig.Instance.AdrenalineMeterPosY = adrenDrawPos.Y;
                        updateCfg = true;
                    }
                }
                if (updateCfg)
                    CalamityMod.SaveConfig(CalamityConfig.Instance);
            }
        }

        private static void CheckGarbageCornerPos()
        {
            // This should only happen if something happens to the config file.
            if (rageDrawPos == Vector2.Zero)
                rageDrawPos = new Vector2(DefaultRagePosX, DefaultRagePosY);
            rageDrawPos = Vector2.Clamp(rageDrawPos, Vector2.Zero, new Vector2(Main.screenWidth - 104f, Main.screenHeight - 24f));

            if (adrenDrawPos == Vector2.Zero)
                adrenDrawPos = new Vector2(DefaultAdrenPosX, DefaultAdrenPosY);
            adrenDrawPos = Vector2.Clamp(adrenDrawPos, Vector2.Zero, new Vector2(Main.screenWidth - 104f, Main.screenHeight - 24f));
        }

        private static Vector2 GetShakeOffset()
        {
            float shake = CalamityConfig.Instance.MeterShake;
            float shakeX = Main.rand.NextFloat(-shake, shake);
            float shakeY = Main.rand.NextFloat(-shake, shake);
            return new Vector2(shakeX, shakeY);
        } 
    }
}
