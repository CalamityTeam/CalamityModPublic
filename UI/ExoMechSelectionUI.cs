using CalamityMod.Items.DraedonMisc;
using CalamityMod.NPCs.ExoMechs;
using CalamityMod.TileEntities;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace CalamityMod.UI
{
    public static class ExoMechSelectionUI
    {
        public static float DestroyerIconScale = 1f;
        public static float PrimeIconScale = 1f;
        public static float TwinsIconScale = 1f;
        public static readonly Color HoverTextColor = Draedon.TextColor;
        public static Rectangle MouseScreenArea => Utils.CenteredRectangle(Main.MouseScreen, Vector2.One * 2f);
        public static void Draw(SpriteBatch spriteBatch)
        {
            Vector2 drawAreaVerticalOffset = Vector2.UnitY * 105f;
            Vector2 baseDrawPosition = Main.LocalPlayer.Top + drawAreaVerticalOffset - Main.screenPosition;
            Vector2 destroyerIconDrawOffset = new Vector2(-78f, -124f);
            Vector2 primeIconDrawOffset = new Vector2(0f, -140f);
            Vector2 twinsIconDrawOffset = new Vector2(78f, -124f);

            HandleInteractionWithButton(baseDrawPosition + destroyerIconDrawOffset, Draedon.ExoMech.Destroyer);
            HandleInteractionWithButton(baseDrawPosition + primeIconDrawOffset, Draedon.ExoMech.Prime);
            HandleInteractionWithButton(baseDrawPosition + twinsIconDrawOffset, Draedon.ExoMech.Twins);
        }

        public static bool HandleInteractionWithButton(Vector2 drawPosition, Draedon.ExoMech exoMech)
        {
            float iconScale;
            string description;
            Texture2D iconMechTexture;

            switch (exoMech)
            {
                case Draedon.ExoMech.Destroyer:
                    iconScale = DestroyerIconScale;
                    iconMechTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/HeadIcon_THanos");
                    description = "Thanatos, a serpentine terror with impervious armor and innumerable laser turrets.";
                    break;
                case Draedon.ExoMech.Prime:
                    iconScale = PrimeIconScale;
                    iconMechTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/HeadIcon_Ares");
                    description = "Ares, a heavyweight, diabolical monstrosity with four Exo superweapons.";
                    break;
                default:
                case Draedon.ExoMech.Twins:
                    iconScale = TwinsIconScale;
                    iconMechTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/HeadIcon_ArtemisApollo");
                    description = "Artemis and Apollo, a pair of extremely agile destroyers with pulse cannons.";
                    break;
            }

            // Check for mouse collision/clicks.
            Rectangle clickArea = Utils.CenteredRectangle(drawPosition, iconMechTexture.Size() * iconScale);

            // Check if the mouse is hovering over the contact button area.
            bool hoveringOverIcon = MouseScreenArea.Intersects(clickArea);
            if (hoveringOverIcon)
            {
                // If so, cause the button to inflate a little bit.
                iconScale = MathHelper.Clamp(iconScale + 0.035f, 1f, 1.35f);

                // Make the selection known if a click is done.
                if (Main.mouseLeft && Main.mouseLeftRelease)
				{
                    CalamityWorld.DraedonMechToSummon = exoMech;
                    CalamityNetcode.SyncWorld();
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
                drawPosition.X -= Main.fontMouseText.MeasureString(description).X * 0.5f;
                drawPosition.Y += 36f;
                Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, description, drawPosition.X, drawPosition.Y, HoverTextColor, Color.Black, Vector2.Zero, 1f);
            }

            // And update to reflect the new scale.
            switch (exoMech)
            {
                case Draedon.ExoMech.Destroyer:
                    DestroyerIconScale = iconScale;
                    break;
                case Draedon.ExoMech.Prime:
                    PrimeIconScale = iconScale;
                    break;
                default:
                case Draedon.ExoMech.Twins:
                    TwinsIconScale = iconScale;
                    break;
            }
            return false;
        }
    }
}
