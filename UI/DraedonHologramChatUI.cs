using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace CalamityMod.UI
{
	public class DraedonHologramChatUI
    {
		public static List<string> DialogOptions = new List<string>()
		{
			"Please help. I'm stuck in this hologram machine."
		};
		public const float TextPadding = 170f;
		public const float TextAreaWidth = 800f;
		public const float YOffsetPerLine = 30f;
		public static void Draw(SpriteBatch spriteBatch)
        {
			CalamityPlayer player = Main.LocalPlayer.Calamity();
			if (player.CurrentlyViewedHologramX == -1 || player.CurrentlyViewedHologramY == -1)
				return;

			if (string.IsNullOrEmpty(player.CurrentlyViewedHologramText))
				player.CurrentlyViewedHologramText = DialogOptions[Main.rand.Next(DialogOptions.Count)];

			Color backgroundColor = new Color(200, 200, 200, 200);
			string[] dialogLines = Utils.WordwrapString(player.CurrentlyViewedHologramText, Main.fontMouseText, (int)(TextAreaWidth - TextPadding * 2), 10, out int lineCount);
			spriteBatch.Draw(Main.chatBackTexture,
							 new Vector2(Main.screenWidth / 2 - Main.chatBackTexture.Width / 2, 100f),
							 new Rectangle(0, 0, Main.chatBackTexture.Width, (lineCount + 2) * 30),
							 backgroundColor, 
							 0f,
							 Vector2.Zero, 
							 1f,
							 SpriteEffects.None,
							 0f);
			spriteBatch.Draw(Main.chatBackTexture, 
							 new Vector2(Main.screenWidth / 2 - Main.chatBackTexture.Width / 2, 100 + (lineCount + 2) * 30),
							 new Rectangle(0, Main.chatBackTexture.Height - 30, Main.chatBackTexture.Width, 30),
							 backgroundColor,
							 0f,
							 Vector2.Zero, 
							 1f,
							 SpriteEffects.None,
							 0f);

			for (int i = 0; i < lineCount + 1; i++)
			{
				if (dialogLines[i] != null)
				{
					Utils.DrawBorderStringFourWay(spriteBatch, 
												  Main.fontMouseText,
												  dialogLines[i], 
												  (int)TextPadding + (int)(Main.screenWidth - TextAreaWidth) / 2, 
												  120 + i * YOffsetPerLine,
												  Color.Cyan,
												  Color.Black, 
												  Vector2.Zero);
				}
			}
		}
    }
}
