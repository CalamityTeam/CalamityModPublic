using System;
using CalamityMod.CalPlayer;
using CalamityMod.Systems.Graphic.PixelationSystem;
using CalamityMod.Utilities.Daybreak.Buffers;
using Microsoft.Build.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.Utilities.Terraria.Utilities;

namespace CalamityMod.UI
{
    public class FishStocksUI : ModSystem
    {
        private static Asset<Texture2D> lineTex, overlayGraphTex, overlayTex, overlayBorderTex, screenTex, fishyPoint1Tex, fishyPoint2Tex, fishyHappy1Tex, fishyHappy2Tex, fishyPanic1Tex, fishyPanic2Tex, fishyBye1Tex, fishyBye2Tex, fishyWhatTex;
        private static int time = 0;

        private static RenderTargetLease PixelationLease;

        private static Color normalColor => Color.Cyan;
        private static Color goodColor => Color.Lime;
        private static Color badColor => Color.Red;
        private static Color shiftColor = goodColor;
        private static float jumpMult = 0;
        private static float shakeMult = 0;
        private static float screenFlicker = 1;
        private static bool waving = true;

        public override void OnModLoad()
        {
            if (Main.dedServ)
                return;

            string folder = "CalamityMod/UI/FishStocks/";
            lineTex = ModContent.Request<Texture2D>("CalamityMod/Particles/LineThick");
            overlayGraphTex = ModContent.Request<Texture2D>(folder + "FishStocksGraphOverlay");
            overlayTex = ModContent.Request<Texture2D>(folder + "FishStocksOverlay");
            overlayBorderTex = ModContent.Request<Texture2D>(folder + "FishStocksOverlayBorder");
            screenTex = ModContent.Request<Texture2D>(folder + "FishStocksScreenOverlay");
            fishyPoint1Tex = ModContent.Request<Texture2D>(folder + "FishyPoint1");
            fishyPoint2Tex = ModContent.Request<Texture2D>(folder + "FishyPoint2");
            fishyHappy1Tex = ModContent.Request<Texture2D>(folder + "FishyHappy1");
            fishyHappy2Tex = ModContent.Request<Texture2D>(folder + "FishyHappy2");
            fishyPanic1Tex = ModContent.Request<Texture2D>(folder + "FishyPanic1");
            fishyPanic2Tex = ModContent.Request<Texture2D>(folder + "FishyPanic2");
            fishyBye1Tex = ModContent.Request<Texture2D>(folder + "FishyBye1");
            fishyBye2Tex = ModContent.Request<Texture2D>(folder + "FishyBye2");
            fishyWhatTex = ModContent.Request<Texture2D>(folder + "FishyWhat");

            Main.QueueMainThreadAction(() => PixelationLease = ScreenspaceTargetPool.Shared.Rent(Main.graphics.GraphicsDevice, (width, height) => (width / 2, height / 2)));
        }

        public override void UpdateUI(GameTime gameTime)
        {
            Player player = Main.LocalPlayer;
            time++;
            float power = Math.Clamp(MathF.Pow((Math.Abs(player.Calamity().fishStockSlidingPower) / 2), 1.8f), 0, 1);

            bool dead = player.dead;

            Color attemptColor = dead ? Color.Gray : (player.Calamity().fishStockSlidingPower >= 0 ? Color.Lerp(normalColor, goodColor, power) : Color.Lerp(normalColor, badColor, power));
            shiftColor = Color.Lerp(shiftColor, attemptColor, 0.03f);

            bool doingBad = player.Calamity().fishStockPower <= -1;
            if (time % 60 < 10 && !doingBad && !waving && !dead)
                jumpMult = MathHelper.Lerp(jumpMult, player.Calamity().fishStockPower >= 1 ? 2.5f : 1, 0.17f);
            if (jumpMult > 0)
                jumpMult = MathHelper.Lerp(jumpMult, 0, 0.09f);
            
            shakeMult = MathHelper.Lerp(shakeMult, (doingBad && !dead) ? 1 : 0, 0.08f);

            screenFlicker = MathHelper.Lerp(0.6f, 1f, MathF.Abs(MathF.Sin(time * 0.0125f)));
        }

        public static void Draw(SpriteBatch spriteBatch, Player player)
        {
            if (Main.hideUI || Main.gameMenu || Main.playerInventory || player.Calamity().fishStockVisual <= 0.001f)
                return;

            // Fishy waves at the screen if the UI is entering or leaving.
            float fishStocksSlideValue = MathF.Pow(1 - player.Calamity().fishStockVisual, player.Calamity().fishStocks ? 1.2f : 20);
            waving = (!player.Calamity().fishStocks && player.Calamity().fishStockPower >= 0) || fishStocksSlideValue > 0.002f;

            float baseScale = 0.65f;
            float fishStocksUIScale = baseScale * Main.UIScale;

            Vector2 screenPos = new Vector2(Main.screenWidth / 1920, Main.screenHeight / 1080) * fishStocksUIScale;
            Vector2 baseUIPosition = (screenPos + new Vector2(overlayBorderTex.Width() * 0.342f * Main.UIScale - (1000 * fishStocksSlideValue), overlayBorderTex.Height() * 0.566f * MathHelper.Lerp(Main.UIScale, 1, 0.5f) + 54 * MathF.Ceiling(player.CountBuffs() / 11f)));
            Vector2 fishyDrawPos = baseUIPosition + new Vector2(112, 40) * fishStocksUIScale;  
            Vector2 leftEdgePos = baseUIPosition - Vector2.UnitX * overlayTex.Width();
            Vector2 rightEdgePos = baseUIPosition - Vector2.UnitX * overlayTex.Width() / 2.7f;
            float fullDist = leftEdgePos.Distance(rightEdgePos) * 0.748f * fishStocksUIScale;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, default, SamplerState.PointClamp, default, default, default, Main.UIScaleMatrix);

            // Overlay
            spriteBatch.Draw(overlayTex.Value, baseUIPosition, null, shiftColor, 0f, overlayTex.Size() / 2, fishStocksUIScale, SpriteEffects.None, 0f);

            // The stock graph.
            Vector2 xAdjust = (-Vector2.UnitX * overlayTex.Width() / 2.565f) * fishStocksUIScale;
            float yAdjust = (overlayTex.Height() / 4.4f) * fishStocksUIScale;
            int backgroundLines = 12;
            float scroll = (time * 0.23f) % ((fullDist / backgroundLines));
            for (int i = 1; i <= backgroundLines; i++) // background grid
            {
                Vector2 start = new Vector2(15 * fishStocksUIScale + (fullDist / backgroundLines * (i - 1)) - scroll, -yAdjust);
                Vector2 end = new Vector2(15 * fishStocksUIScale + (fullDist / backgroundLines * i) - scroll, yAdjust);
                Vector2 scale = new Vector2(0.1f * fishStocksUIScale, 0.0103f * start.Distance(end)) * 0.05f;
                spriteBatch.Draw(lineTex.Value, baseUIPosition + xAdjust + start, null, shiftColor * 0.2f, 0, new Vector2(lineTex.Width() / 2, 0), scale, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(lineTex.Value, baseUIPosition + xAdjust, null, shiftColor * 0.3f, -MathHelper.PiOver2, new Vector2(lineTex.Width() / 2, 0), new Vector2(0.15f * fishStocksUIScale, 0.0103f * fullDist * 0.99f) * 0.05f, SpriteEffects.None, 0f);

            // Graph overlay
            spriteBatch.Draw(overlayGraphTex.Value, baseUIPosition, null, shiftColor, 0f, overlayGraphTex.Size() / 2, fishStocksUIScale, SpriteEffects.None, 0f);

            // Draw Fishy and the stcok graph line to a downscaled render target so that it can be redrawn at 2x for a pixelated effect.
            spriteBatch.End();
            using (PixelationLease.Scope(clearColor: Color.Transparent))
            {
                Matrix pixelationMatrix = Matrix.CreateScale(0.5f, 0.5f, 1f);
                spriteBatch.Begin(SpriteSortMode.Deferred, default, SamplerState.PointClamp, default, default, default, pixelationMatrix);

                int lines = 5;
                for (int i = 1; i <= lines; i++) // The stock graph
                {
                    float maxHeight = 36f * fishStocksUIScale;
                    Vector2 start = new Vector2((fullDist / lines) * (i - 1), maxHeight * GetHeight(i - 1, player));
                    Vector2 end = new Vector2((fullDist / lines) * i, maxHeight * GetHeight(i, player));
                    Vector2 scale = new Vector2(0.2f * fishStocksUIScale, 0.0103f * start.Distance(end)) * 0.05f;
                    spriteBatch.Draw(lineTex.Value, baseUIPosition + xAdjust + start - Vector2.UnitY * 1f, null, shiftColor, start.DirectionTo(end).ToRotation() - MathHelper.PiOver2, new Vector2(lineTex.Width() / 2, 0), scale, SpriteEffects.None, 0f);
                }

                // Fishy :D
                Vector2 positionOffset = new Vector2(4 * MathF.Sin(time * 0.65f) * shakeMult, -25 * jumpMult) * fishStocksUIScale;
                float fishyScale = 1.45f * fishStocksUIScale;
                Texture2D fishyTexture = GetFishyTexture(player);
                spriteBatch.Draw(fishyTexture, fishyDrawPos + positionOffset, null, Color.Lerp(shiftColor, Color.White, 0.35f), 0f, fishyTexture.Size() / 2, fishyScale, SpriteEffects.None, 0f);

                spriteBatch.End();
            }

            // Redraw the now-pixelated target.
            spriteBatch.Begin(SpriteSortMode.Deferred, default, SamplerState.PointClamp, default, default, default, Main.UIScaleMatrix);
            spriteBatch.Draw(PixelationLease.Target, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, 0, 0f);

            string fishStocksPower = (player.Calamity().fishStockPower > -0 ? "+" : "") + Math.Round(player.Calamity().fishStockPower, 2).ToString() + "x";
            Vector2 textPos = baseUIPosition + new Vector2(-148 - (3.5f * fishStocksPower.Length), -118.5f) * fishStocksUIScale;

            // Text
            CalamityUtils.DrawBorderStringEightWay(spriteBatch, FontAssets.MouseText.Value, fishStocksPower, textPos, Color.Lerp(shiftColor, Color.White, 0.35f), Color.Black, 1.4f * fishStocksUIScale);

            // Screen overlay
            if (!CalamityClientConfig.Instance.Photosensitivity)
                spriteBatch.Draw(screenTex.Value, baseUIPosition, null, shiftColor * 0.166f * screenFlicker, 0f, screenTex.Size() / 2, fishStocksUIScale, SpriteEffects.None, 0f);
            
            // Overlay Border
            spriteBatch.Draw(overlayBorderTex.Value, baseUIPosition, null, Color.White, 0f, overlayBorderTex.Size() / 2, fishStocksUIScale, SpriteEffects.None, 0f);
        }

        private static Texture2D GetFishyTexture(Player player)
        {
            float stockPower = player.Calamity().fishStockPower;
            bool panic = stockPower <= -1f;
            bool happy = stockPower >= 1f;
            bool frame2 = time % 60 < 30;
            Texture2D fishyTexture = player.dead ? fishyWhatTex.Value : (waving ? frame2 ? fishyBye2Tex.Value : fishyBye1Tex.Value : happy ? frame2 ? fishyHappy2Tex.Value : fishyHappy1Tex.Value :
                panic ? frame2 ? fishyPanic2Tex.Value : fishyPanic1Tex.Value : frame2 ? fishyPoint2Tex.Value : fishyPoint1Tex.Value);

            return fishyTexture;
        }

        public static float GetHeight(int point, Player player)
        {
            (float, float, float, float, float) oldHeights = player.Calamity().fishStockOldPower;
            float height = point == 0 ? oldHeights.Item5 :
                    point == 1 ? oldHeights.Item4 : point == 2 ? oldHeights.Item3 :
                    point == 3 ? oldHeights.Item2 : point == 4 ? oldHeights.Item1 : player.Calamity().fishStockSlidingPower;
            return -height;
        }
    }
}
