using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace CalamityMod.UI
{
	public class DraedonHologramChatUI
    {
		public const float TextPadding = 170f;
		public const float TextAreaWidth = 800f;
		public const float YOffsetPerLine = 30f;
		public static string SelectDiaglog()
		{
			List<string> potentialDiaglogOptions = new List<string>()
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
				potentialDiaglogOptions.Add("Sensors have detected a significant breach in the spacetime continuum.");
			if (Main.rand.NextBool(5000))
				return "Please help. I'm stuck in this hologram machine.";
			return Main.rand.Next(potentialDiaglogOptions.ToArray());
		}
		public static void Draw(SpriteBatch spriteBatch)
        {
			CalamityPlayer player = Main.LocalPlayer.Calamity();
			if (player.CurrentlyViewedHologramX == -1 || player.CurrentlyViewedHologramY == -1)
				return;

			if (string.IsNullOrEmpty(player.CurrentlyViewedHologramText))
				player.CurrentlyViewedHologramText = SelectDiaglog();

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
