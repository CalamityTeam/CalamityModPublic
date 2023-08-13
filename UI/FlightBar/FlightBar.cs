using System;
using System.Text;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
    // TODO -- This can be made into a ModSystem with simple OnModLoad and Unload hooks.
    public class FlightBar
    {
        // These values were handpicked on a 1080p screen by Amber. Please disregard the bizarre precision.
        internal const float DefaultFlightPosX = 40.9375f;
        internal const float DefaultFlightPosY = 7.2222223f; //yes really, that many twos were handpicked, how? your guess is as good as mine lmao
        private const float MouseDragEpsilon = 0.05f; // 0.05%
        private const int FlightAnimFrameDelay = 5;
        private const int FlightAnimFrames = 17;
        
        private static int FlightAnimFrame = -1;
        private static int FlightAnimTimer = 0;

        private static Vector2? dragOffset = null;
        private static Texture2D borderTexture, flightBarAnimTexture, barTexture, disabledBarTexture, infiniteBarTexture, limitedBarTexture;
        private static bool completedAnimation = false;

        private static Texture2D GetApplicableBorder(CalamityPlayer modPlayer)
        {
            if (modPlayer.Player.equippedWings != null && modPlayer.Player.wingTimeMax == 0)
                return disabledBarTexture;
            if (modPlayer.infiniteFlight && completedAnimation)
                return infiniteBarTexture;
            if (modPlayer.weakPetrification || modPlayer.vHex || modPlayer.icarusFolly || modPlayer.DoGExtremeGravity)
                return limitedBarTexture;
            return borderTexture;
        }
        
        private static object GetFlightTime(CalamityPlayer modPlayer)
        {
            Player player = modPlayer.Player;
            object result;
            if (player.equippedWings != null && player.wingTimeMax == 0)
                result = 0;
            if (modPlayer.infiniteFlight && completedAnimation)
            {
                result = "∞"; //infinite flight
            }
            else
            {
                int currentFlight = (int)player.wingTime;
                int maxFlight = player.wingTimeMax;
                return (Math.Min(100f * currentFlight / maxFlight, 100f)).ToString("0.00"); // why the FUCK can wingtime be higher than max wingtime?????????
            }

            return result;
        }

        internal static void Load()
        {
            borderTexture = ModContent.Request<Texture2D>("CalamityMod/UI/FlightBar/FlightBarBorder", AssetRequestMode.ImmediateLoad).Value;
            flightBarAnimTexture = ModContent.Request<Texture2D>("CalamityMod/UI/FlightBar/FlightBarAnim", AssetRequestMode.ImmediateLoad).Value;
            barTexture = ModContent.Request<Texture2D>("CalamityMod/UI/FlightBar/FlightBar", AssetRequestMode.ImmediateLoad).Value;
            disabledBarTexture = ModContent.Request<Texture2D>("CalamityMod/UI/FlightBar/FlightBarBorderDisabled", AssetRequestMode.ImmediateLoad).Value;
            infiniteBarTexture = ModContent.Request<Texture2D>("CalamityMod/UI/FlightBar/FlightBarBorderInfinite", AssetRequestMode.ImmediateLoad).Value;
            limitedBarTexture = ModContent.Request<Texture2D>("CalamityMod/UI/FlightBar/FlightBarBorderReduced", AssetRequestMode.ImmediateLoad).Value;
            Reset();
        }

        internal static void Unload()
        {
            Reset();
            borderTexture = flightBarAnimTexture = barTexture = disabledBarTexture = infiniteBarTexture = limitedBarTexture = null;
            FlightAnimFrame = -1;
            FlightAnimTimer = 0;
            completedAnimation = false;
        }

        private static void Reset() => dragOffset = null;


        
        public static void Draw(SpriteBatch spriteBatch, Player player)
        {
            // Sanity check the planned position before drawing
            Vector2 screenRatioPosition = new Vector2(CalamityConfig.Instance.FlightBarPosX, CalamityConfig.Instance.FlightBarPosY);
            if (screenRatioPosition.X < 0f || screenRatioPosition.X > 100f)
                screenRatioPosition.X = DefaultFlightPosX;
            if (screenRatioPosition.Y < 0f || screenRatioPosition.Y > 100f)
                screenRatioPosition.Y = DefaultFlightPosY;

            // Convert the screen ratio position to an absolute position in pixels
            // Cast to integer to prevent blurriness which results from decimal pixel positions
            float uiScale = Main.UIScale;
            Vector2 screenPos = screenRatioPosition;
            screenPos.X = (int)(screenPos.X * 0.01f * Main.screenWidth);
            screenPos.Y = (int)(screenPos.Y * 0.01f * Main.screenHeight);

            CalamityPlayer modPlayer = player.Calamity();

            // If not drawing the flight bar, save its latest position to config and leave.
            if (CalamityConfig.Instance.FlightBar && player.wingsLogic > 0)
			{
				DrawFlightBar(spriteBatch, modPlayer, screenPos);
			}
			else
            {
                bool changed = false;
                if (CalamityConfig.Instance.FlightBarPosX != screenRatioPosition.X)
                {
                    CalamityConfig.Instance.FlightBarPosX = screenRatioPosition.X;
                    changed = true;
                }
                if (CalamityConfig.Instance.FlightBarPosY != screenRatioPosition.Y)
                {
                    CalamityConfig.Instance.FlightBarPosY = screenRatioPosition.Y;
                    changed = true;
                }

                if (changed)
                    CalamityMod.SaveConfig(CalamityConfig.Instance);
                return;
            }

            Rectangle mouseHitbox = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
            Rectangle flightBar = Utils.CenteredRectangle(screenPos,  borderTexture.Size() * uiScale);

			MouseState ms = Mouse.GetState();
			Vector2 mousePos = Main.MouseScreen;

            // Handle mouse dragging
			if (flightBar.Intersects(mouseHitbox))
			{
				if (!CalamityConfig.Instance.MeterPosLock)
					Main.LocalPlayer.mouseInterface = true;
                
                if (modPlayer.Player.equippedWings != null && modPlayer.Player.wingTimeMax > 0) //equipped wings and max wingtime above 0 (so not disabled bar)
                {
                    string textToDisplay = CalamityUtils.GetText("UI.Flight").Format((GetFlightTime(modPlayer).ToString() + (modPlayer.infiniteFlight ? "" : "%"))); //the percent is here and not in localisation otherwise it looks like a dick when it's infinite flight
                    Main.instance.MouseText(textToDisplay, 0, 0, -1, -1, -1, -1);
                }

                Vector2 newScreenRatioPosition = screenRatioPosition;
				// As long as the mouse button is held down, drag the meter along with an offset.
                if (!CalamityConfig.Instance.MeterPosLock && ms.LeftButton == ButtonState.Pressed)
                {
					// If the drag offset doesn't exist yet, create it.
					if (!dragOffset.HasValue)
						dragOffset = mousePos - screenPos;

					// Given the mouse's absolute current position, compute where the corner of the flight bar should be based on the original drag offset.
					Vector2 newCorner = mousePos - dragOffset.GetValueOrDefault(Vector2.Zero);

					// Convert the new corner position into a screen ratio position.
					newScreenRatioPosition.X = (100f * newCorner.X) / Main.screenWidth;
					newScreenRatioPosition.Y = (100f * newCorner.Y) / Main.screenHeight;
				}

				// Compute the change in position. If it is large enough, actually move the meter
				Vector2 delta = newScreenRatioPosition - screenRatioPosition;
				if (Math.Abs(delta.X) >= MouseDragEpsilon || Math.Abs(delta.Y) >= MouseDragEpsilon)
				{
					CalamityConfig.Instance.FlightBarPosX = newScreenRatioPosition.X;
					CalamityConfig.Instance.FlightBarPosY = newScreenRatioPosition.Y;
				}

				// When the mouse is released, save the config and destroy the drag offset.
				if (ms.LeftButton == ButtonState.Released)
				{
					dragOffset = null;
					CalamityMod.SaveConfig(CalamityConfig.Instance);
				}
            }
		}

        /**
        private const int FlightAnimFrameDelay = 5;
        private const int FlightAnimFrames = 17;
        
        private static int FlightAnimFrame = -1;
        private static int FlightAnimTimer = 0;
         */
        private static void DrawFlightBar(SpriteBatch spriteBatch, CalamityPlayer modPlayer, Vector2 screenPos)
        {
            float uiScale = Main.UIScale;
            Player player = modPlayer.Player;
            float flightRatio = Math.Min(player.wingTime / player.wingTimeMax, 1f); // why the FUCK can wingtime be higher than max wingtime?????????
            if (!completedAnimation && FlightAnimFrame == -1 && modPlayer.infiniteFlight) 
                FlightAnimFrame++;
            if (FlightAnimFrame > -1) //animation started, complete it.
            {
                FlightAnimTimer++;
                if (FlightAnimTimer >= FlightAnimFrameDelay)
                {
                    if (FlightAnimFrame >= FlightAnimFrames)
                    {
                        FlightAnimFrame = -1;
                        FlightAnimTimer = 0;
                        completedAnimation = modPlayer.infiniteFlight; //completed animation sets to true if infinite flight still exists
                    }
                    else
                    {
                        FlightAnimTimer = 0;
                        FlightAnimFrame++;   
                    }
                }
            }
            Texture2D correctBorder = GetApplicableBorder(modPlayer); //Fetch texture after animation calculations in case update to infinite flight
            
            if (completedAnimation && !modPlayer.infiniteFlight && correctBorder != infiniteBarTexture)
                completedAnimation = false; //reset flight anim once infinite flight expires.
            
            
            float offset = (correctBorder.Width - barTexture.Width) * 0.5f;
            spriteBatch.Draw(correctBorder, screenPos, null, Color.White, 0f, correctBorder.Size() * 0.5f, uiScale, SpriteEffects.None, 0);
            if (correctBorder != disabledBarTexture && correctBorder != infiniteBarTexture) //neither requires an internal bar to be drawn
            {
                int correctHeight = (correctBorder == limitedBarTexture ? barTexture.Height / 2 : barTexture.Height);
                Rectangle barRectangle = barTexture.Bounds;
                barRectangle.Height = (int)(correctHeight * flightRatio);
                Vector2 origin = correctBorder.Size() * 0.5f;

                spriteBatch.Draw(barTexture, screenPos - new Vector2(offset * uiScale, 11), barRectangle, Color.White, MathHelper.ToRadians(180), origin, uiScale, SpriteEffects.None, 0);
            }
            if (!completedAnimation && FlightAnimFrame >= 0)
            {
                Vector2 origin = new Vector2(correctBorder.Width * 0.5f, (correctBorder.Height / FlightAnimFrames) * 0.5f);
                float animOffset = 5f;
                float xOffset = (correctBorder.Width - flightBarAnimTexture.Width) / 2f;
                int frameHeight = (flightBarAnimTexture.Height / FlightAnimFrames) - 1;
                float yOffset = FlightAnimFrame == 0 ? 0 : ((correctBorder.Height / FlightAnimFrame) - frameHeight) / 2f ;
                Vector2 sizeDiffOffset = new Vector2(xOffset, yOffset);
                Rectangle animCropRect = new Rectangle(0, (frameHeight + 1) * FlightAnimFrame, flightBarAnimTexture.Width, frameHeight);
                spriteBatch.Draw(flightBarAnimTexture, screenPos + sizeDiffOffset, animCropRect, Color.White, 0f, origin, uiScale, SpriteEffects.None, 0);
            }
        }
    }
}
