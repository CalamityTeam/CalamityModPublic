using CalamityMod.CalPlayer;
using CalamityMod.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
    public class ChargeMeterUI
    {
        public static Vector2 DrawPosition = new Vector2(CalamityConfig.Instance.ChargeMeterPosX, CalamityConfig.Instance.ChargeMeterPosY);
        public static Vector2 Offset = DrawPosition;
        public static void Draw(SpriteBatch spriteBatch, Player player)
        {
            if (DrawPosition.X <= 60f && DrawPosition.Y <= 20f)
            {
                DrawPosition.X = Main.screenWidth / 2f;
                DrawPosition.Y = Main.screenHeight / 2 + Main.LocalPlayer.height / 2f + 24f;
            }
            CalamityPlayer modPlayer = player.Calamity();
            Item heldItem = player.ActiveItem();
            if (!CalamityConfig.Instance.ChargeMeter || heldItem is null || heldItem.IsAir)
            {
                Reset();
                return;
            }
            CalamityGlobalItem modItem = heldItem.Calamity();
            if (!(modItem?.UsesCharge ?? false))
            {
                Reset();
                return;
            }
            Texture2D edgeTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/ChargeMeterBorder").Value;
            Texture2D barTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/ChargeMeter").Value;
            float uiScale = Main.UIScale;
            spriteBatch.Draw(edgeTexture, DrawPosition, null, Color.White, 0f, edgeTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);

            Rectangle barRectangle = new Rectangle(0, 0, (int)(barTexture.Width * modItem.ChargeRatio), barTexture.Width);
            spriteBatch.Draw(barTexture, DrawPosition, barRectangle, Color.White, 0f, edgeTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);

            Rectangle mouse = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
            Rectangle chargeBar = Utils.CenteredRectangle(DrawPosition, barTexture.Size() * uiScale);

            if (chargeBar.Intersects(mouse))
            {
                if (modItem.UsesCharge)
                {
                    Main.LocalPlayer.mouseInterface = true;
                    float displayedPercent = modItem.ChargeRatio * 100f;
                    string percentString = displayedPercent.ToString("n2");
                    // Tooltip only goes to one decimal place, but the meter displays two. This doesn't help the player much at all but hey, it's a thing.
                    Main.instance.MouseText("Current Charge: " + percentString + "%", 0, 0, -1, -1, -1, -1);
                }
            }
            if (!CalamityConfig.Instance.MeterPosLock)
            {
                if (chargeBar.Intersects(mouse))
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

                if (CalamityConfig.Instance.ChargeMeterPosX != DrawPosition.X)
                {
                    CalamityConfig.Instance.ChargeMeterPosX = DrawPosition.X;
                    CalamityMod.SaveConfig(CalamityConfig.Instance);
                }
                if (CalamityConfig.Instance.ChargeMeterPosY != DrawPosition.Y)
                {
                    CalamityConfig.Instance.ChargeMeterPosY = DrawPosition.Y;
                    CalamityMod.SaveConfig(CalamityConfig.Instance);
                }
            }
        }

        internal static void Reset()
        {
            if (CalamityConfig.Instance.ChargeMeterPosX != DrawPosition.X)
            {
                CalamityConfig.Instance.ChargeMeterPosX = DrawPosition.X;
                CalamityMod.SaveConfig(CalamityConfig.Instance);
            }
            if (CalamityConfig.Instance.ChargeMeterPosY != DrawPosition.Y)
            {
                CalamityConfig.Instance.ChargeMeterPosY = DrawPosition.Y;
                CalamityMod.SaveConfig(CalamityConfig.Instance);
            }
        }
    }
}
