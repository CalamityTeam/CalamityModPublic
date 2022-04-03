using CalamityMod.CalPlayer;
using CalamityMod.TileEntities;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.GameContent;

namespace CalamityMod.UI
{
    public class LabHologramProjectorUI
    {
        public const float MaxPlayerDistance = 120f;
        public const float TextPadding = 170f;
        public const float TextAreaWidth = 800f;
        public const float YOffsetPerLine = 30f;
        public static string ChooseDialogue()
        {
            List<string> dialogueOptions = new List<string>()
            {
                "To any personnel engaged in the laboratories. Please wear your steel engraved ID badge at all times. It is the easiest method to discern your body if any accidents do occur.",
                "To experiment is to fail. To fail is to learn. To learn is to advance.",
                "Apparent danger while researching serves only to enhance the research experience.",

                "Laser-type weapon prototypes are incredibly lethal and are not to be used within presentation halls.",
                "High-energy plasma emissions have adverse effects on both flesh and metal. Do not attempt to handle vented plasma.",
                "Electric shocks from military equipment are intended to be fatal. If you survive such a shock, that is a clear indicator that the device is not functioning properly. Please report any such cases.",
                "All employees are hereby notified that they will be held accountable for any collateral damage caused by Gauss weapon fire, even during sanctioned testing exercises.",
                "Security Field Emitters will vaporize all unauthorized equipment and personnel. Please leave personal effects in the designated lockers off-site. This also means: Do not bring any family members who are not enlisted as personnel.",

                "If one does manage to breach restricted testing facilities, do at least record any unexpected burns, lacerations, bruising, fractur... ...trauma, shocks and otherwise. Thank you.",
                "The power grid has been... ...eavily compromised. Abort research and proceed to the emergency exits located at... ...and egress with haste.",
                "Notify the Security Department of any aggressive local fauna immediately."
            };
            if (NPC.downedAncientCultist)
                dialogueOptions.Add("Sensors have detected a significant breach in the spacetime continuum.");

            return Main.rand.NextBool(5000) ? "Please help. I'm stuck in this hologram machine." : Main.rand.Next(dialogueOptions.ToArray());
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            Player p = Main.LocalPlayer;
            CalamityPlayer mp = p.Calamity();
            int projectorID = mp.CurrentlyViewedHologramID;

            // The UI only draws if the player is viewing a projector.
            if (projectorID == -1)
                return;

            // The UI cannot draw if the player is already occupied with an NPC.
            if (p.talkNPC > 0 || Main.npcShop > 0)
                return;

            // Check if this tile entity ID is actually a projector. If it's not, immediately destroy this UI.
            TELabHologramProjector projector;
            bool projectorIsValid = TileEntity.ByID.TryGetValue(projectorID, out TileEntity te);
            if (projectorIsValid && te is TELabHologramProjector cast)
                projector = cast;
            else
            {
                mp.CurrentlyViewedHologramID = -1;
                mp.CurrentlyViewedHologramText = string.Empty;
                return;
            }

            // If the player is too far away from their viewed projector, immediately destroy this UI and play the menu close sound.
            Vector2 projectorWorldCenter = projector.Center;
            if (p.DistanceSQ(projectorWorldCenter) > MaxPlayerDistance * MaxPlayerDistance)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                mp.CurrentlyViewedHologramID = -1;
                mp.CurrentlyViewedHologramText = string.Empty;
                return;
            }

            // Pick text to display, if it has not already been picked.
            if (string.IsNullOrEmpty(mp.CurrentlyViewedHologramText))
                mp.CurrentlyViewedHologramText = ChooseDialogue();

            Color backgroundColor = new Color(200, 200, 200, 200);

            // Wrap the text so that it neatly fits in the dialogue box.
            string[] dialogLines = Utils.WordwrapString(mp.CurrentlyViewedHologramText, FontAssets.MouseText.Value, (int)(TextAreaWidth - TextPadding * 2), 10, out int lineCount);

            // Draw the background of the text box.
            spriteBatch.Draw(TextureAssets.ChatBack.Value, new Vector2(Main.screenWidth / 2 - TextureAssets.ChatBack.Value.Width / 2, 100f),
                new Rectangle(0, 0, TextureAssets.ChatBack.Value.Width, (lineCount + 2) * 30), backgroundColor, 0f, Vector2.Zero, 1f,
                SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureAssets.ChatBack.Value,
                new Vector2(Main.screenWidth / 2 - TextureAssets.ChatBack.Value.Width / 2, 100 + (lineCount + 2) * 30),
                new Rectangle(0, TextureAssets.ChatBack.Value.Height - 30, TextureAssets.ChatBack.Value.Width, 30), backgroundColor, 0f,
                Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Draw the dialogue itself.
            for (int i = 0; i < lineCount + 1; i++)
            {
                string text = dialogLines[i];
                if (text is null)
                    continue;
                Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, dialogLines[i],
                    (int)TextPadding + (int)(Main.screenWidth - TextAreaWidth) / 2, 120 + i * YOffsetPerLine, Color.Cyan, Color.Black,
                    Vector2.Zero);
            }
        }
    }
}
