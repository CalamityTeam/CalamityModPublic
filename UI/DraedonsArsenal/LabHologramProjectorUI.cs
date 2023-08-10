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

namespace CalamityMod.UI.DraedonsArsenal
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
                "Text1", "Text2", "Text3", "Text4", "Text5", "Text6", "Text7", "Text8", "Text9", "Text10", "Text11"
            };
            if (NPC.downedAncientCultist)
                dialogueOptions.Add("PostCultistText");

            return Main.rand.NextBool(5000) ? "EasterEgg" : Main.rand.Next(dialogueOptions.ToArray());
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
                mp.CurrentlyViewedHologramText = CalamityUtils.GetText("UI.Hologram." + ChooseDialogue()).ToString();

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
            float x = (int)TextPadding + (int)(Main.screenWidth - TextAreaWidth) / 2;
            for (int i = 0; i < lineCount + 1; i++)
            {
                string text = dialogLines[i];
                if (text is null)
                    continue;

                Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, dialogLines[i], x, 120 + i * YOffsetPerLine, Color.Cyan, Color.Black, Vector2.Zero);
            }
        }
    }
}
