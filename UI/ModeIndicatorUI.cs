using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
	public class ModeIndicatorUI
	{
		public static readonly Vector2 OffsetToAreaCenter = new Vector2(0f, 4f);
		public static Vector2 DifficultyIconOffset => new Vector2(0f, -13f);
		public static Vector2 ArmageddonIconOffset => new Vector2(12f, 7f);
		public static Vector2 MaliceIconOffset => new Vector2(-12f, 7f);
		public static void Draw(SpriteBatch spriteBatch)
		{
			// The mode indicator should only be displayed when the inventory is open, to prevent obstruction.
			if (!Main.playerInventory)
				return;

			Texture2D outerAreaTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/ModeIndicatorArea");
			Texture2D revengeanceIconTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/ModeIndicatorRev");
			Texture2D deathIconTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/ModeIndicatorDeath");
			Texture2D armageddonIconTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/ModeIndicatorArma");
			Texture2D maliceIconTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/ModeIndicatorMalice");

			Rectangle mouseRectangle = Utils.CenteredRectangle(Main.MouseScreen, Vector2.One * 2f);

			Vector2 drawCenter = new Vector2(Main.screenWidth - 400f, 72f) + outerAreaTexture.Size() * 0.5f;

			spriteBatch.Draw(outerAreaTexture, drawCenter, null, Color.White, 0f, outerAreaTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);

			Texture2D difficultyIconToUse = CalamityWorld.death ? deathIconTexture : revengeanceIconTexture;
			bool modeIsActive = CalamityWorld.revenge;
			bool renderingText = false;
			if (CalamityWorld.revenge)
				spriteBatch.Draw(difficultyIconToUse, drawCenter + DifficultyIconOffset, null, Color.White, 0f, difficultyIconToUse.Size() * 0.5f, 1f, SpriteEffects.None, 0f);

			// Rev/Death active text.
			if (mouseRectangle.Intersects(Utils.CenteredRectangle(drawCenter + DifficultyIconOffset, difficultyIconToUse.Size())))
			{
				Main.instance.MouseText($"{(CalamityWorld.death ? "Death" : "Revengeance")} Mode is {(modeIsActive ? "active" : "not active")}.");
				renderingText = true;
			}

			if (CalamityWorld.armageddon)
			{
				spriteBatch.Draw(armageddonIconTexture, drawCenter + ArmageddonIconOffset, null, Color.White, 0f, armageddonIconTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
			}

			// Armageddon active text.
			if (!renderingText && mouseRectangle.Intersects(Utils.CenteredRectangle(drawCenter + ArmageddonIconOffset, armageddonIconTexture.Size())))
			{
				Main.instance.MouseText($"Armageddon is {(CalamityWorld.armageddon ? "active" : "not active")}.");
				renderingText = true;
			}

			if (CalamityWorld.malice)
				spriteBatch.Draw(maliceIconTexture, drawCenter + MaliceIconOffset, null, Color.White, 0f, maliceIconTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);

			// Malice active text.
			if (!renderingText && mouseRectangle.Intersects(Utils.CenteredRectangle(drawCenter + MaliceIconOffset, armageddonIconTexture.Size())))
			{
				Main.instance.MouseText($"Malice Mode is {(CalamityWorld.malice ? "active" : "not active")}.");
				renderingText = true;
			}

			if (renderingText)
				Main.instance.MouseTextHackZoom(string.Empty);
		}
	}
}
