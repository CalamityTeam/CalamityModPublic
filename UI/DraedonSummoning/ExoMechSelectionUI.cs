﻿using CalamityMod.NPCs.ExoMechs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.UI.DraedonSummoning
{
    public static class ExoMechSelectionUI
    {
        public static ExoMech? HoverSoundMechType
        {
            get;
            set;
        } = null;

        public static float DestroyerIconScale = 1f;

        public static float PrimeIconScale = 1f;

        public static float TwinsIconScale = 1f;

        public static readonly Color HoverTextColor = Draedon.TextColor;

        public static Rectangle MouseScreenArea => Utils.CenteredRectangle(Main.MouseScreen, Vector2.One * 2f);

        public static readonly SoundStyle ThanatosHoverSound = new("CalamityMod/Sounds/Custom/Codebreaker/ThanatosIconHover");

        public static readonly SoundStyle AresHoverSound = new("CalamityMod/Sounds/Custom/Codebreaker/AresIconHover");

        public static readonly SoundStyle TwinsHoverSound = new("CalamityMod/Sounds/Custom/Codebreaker/ArtemisApolloIconHover");

        public static void Draw()
        {
            Vector2 drawAreaVerticalOffset = Vector2.UnitY * 105f;
            Vector2 baseDrawPosition = Main.LocalPlayer.Top + drawAreaVerticalOffset - Main.screenPosition;
            Vector2 destroyerIconDrawOffset = new Vector2(-78f, -124f);
            Vector2 primeIconDrawOffset = new Vector2(0f, -140f);
            Vector2 twinsIconDrawOffset = new Vector2(78f, -124f);

            bool hoveringOverAnyIcon = HandleInteractionWithButton(baseDrawPosition + destroyerIconDrawOffset, ExoMech.Destroyer);
            hoveringOverAnyIcon |= HandleInteractionWithButton(baseDrawPosition + primeIconDrawOffset, ExoMech.Prime);
            hoveringOverAnyIcon |= HandleInteractionWithButton(baseDrawPosition + twinsIconDrawOffset, ExoMech.Twins);
            if (!hoveringOverAnyIcon)
                HoverSoundMechType = null;
        }

        public static bool HandleInteractionWithButton(Vector2 drawPosition, ExoMech exoMech)
        {
            float iconScale;
            string description;
            Texture2D iconMechTexture;
            SoundStyle hoverSound;

            switch (exoMech)
            {
                case ExoMech.Destroyer:
                    iconScale = DestroyerIconScale;
                    iconMechTexture = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/HeadIcon_THanos").Value;
                    description = CalamityUtils.GetTextValue("UI.ThanatosIcon");
                    hoverSound = ThanatosHoverSound;
                    break;
                case ExoMech.Prime:
                    iconScale = PrimeIconScale;
                    iconMechTexture = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/HeadIcon_Ares").Value;
                    description = CalamityUtils.GetTextValue("UI.AresIcon");
                    hoverSound = AresHoverSound;
                    break;
                default:
                case ExoMech.Twins:
                    iconScale = TwinsIconScale;
                    iconMechTexture = ModContent.Request<Texture2D>("CalamityMod/UI/DraedonSummoning/HeadIcon_ArtemisApollo").Value;
                    description = CalamityUtils.GetTextValue("UI.ArtemisApolloIcon");
                    hoverSound = TwinsHoverSound;
                    break;
            }

            // Check for mouse collision/clicks.
            Rectangle clickArea = Utils.CenteredRectangle(drawPosition, iconMechTexture.Size() * iconScale * 0.9f);

            // Check if the mouse is hovering over the contact button area.
            bool hoveringOverIcon = MouseScreenArea.Intersects(clickArea);
            if (hoveringOverIcon)
            {
                // If so, cause the button to inflate a little bit.
                iconScale = MathHelper.Clamp(iconScale + 0.035f, 1f, 1.35f);

                // Play the hover sound.
                if (HoverSoundMechType != exoMech)
                {
                    HoverSoundMechType = exoMech;
                    SoundEngine.PlaySound(hoverSound with { Volume = 1.5f });
                }

                // Make the selection known if a click is done.
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    CalamityWorld.DraedonMechToSummon = exoMech;

                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        var netMessage = CalamityMod.Instance.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.ExoMechSelection);
                        netMessage.Write((int)CalamityWorld.DraedonMechToSummon);
                        netMessage.Send();
                    }
                }
                Main.blockMouse = Main.LocalPlayer.mouseInterface = true;
            }

            // Otherwise, if not hovering, cause the button to deflate back to its normal size.
            else
                iconScale = MathHelper.Clamp(iconScale - 0.05f, 1f, 1.2f);

            // Draw the icon with the new scale.
            Main.spriteBatch.Draw(iconMechTexture, drawPosition, null, Color.White, 0f, iconMechTexture.Size() * 0.5f, iconScale, SpriteEffects.None, 0f);

            // Draw the descrption if hovering over the icon.
            if (hoveringOverIcon)
            {
                drawPosition.X -= FontAssets.MouseText.Value.MeasureString(description).X * 0.5f;
                drawPosition.Y += 36f;
                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, description, drawPosition.X, drawPosition.Y, HoverTextColor, Color.Black, Vector2.Zero, 1f);
            }

            // And update to reflect the new scale.
            switch (exoMech)
            {
                case ExoMech.Destroyer:
                    DestroyerIconScale = iconScale;
                    break;
                case ExoMech.Prime:
                    PrimeIconScale = iconScale;
                    break;
                default:
                case ExoMech.Twins:
                    TwinsIconScale = iconScale;
                    break;
            }
            return hoveringOverIcon;
        }
    }
}
