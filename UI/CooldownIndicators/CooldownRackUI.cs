using CalamityMod.CalPlayer;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.UI.CooldownIndicators
{
	public class CooldownRackUI
	{
		public static int MaxLargeIcons = 10;

		public static bool CompactIcons => Main.LocalPlayer.Calamity().Cooldowns.FindAll(cooldown => cooldown.DisplayMe).Count > MaxLargeIcons || CalamityConfig.Instance.CooldownDisplay == 1;

		public static Vector2 Spacing => CompactIcons ? Vector2.UnitX * Main.UIScale * 28f : Vector2.UnitX * Main.UIScale * 46f;

		public static Vector2 BaseDrawPosition =>  new Vector2(32, 100) * Main.UIScale + Spacing / 2f + (Main.LocalPlayer.CountBuffs() > 0 ? Vector2.UnitY * 50 * Main.UIScale : Vector2.Zero) + (Main.LocalPlayer.CountBuffs() > 11 ? Vector2.UnitY * 50 * Main.UIScale : Vector2.Zero);


		public static void Draw(SpriteBatch spriteBatch)
		{
			//Don't draw the cooldowns if the player's inventory is open
			if (Main.playerInventory || CalamityConfig.Instance.CooldownDisplay < 1)
				return;

			List<CooldownIndicator> cooldownsToDraw = CooldownIndicator.DeepCopy(Main.LocalPlayer.Calamity().Cooldowns);
			cooldownsToDraw.RemoveAll(cooldown => !cooldown.DisplayMe);

			//DEbug
			//cooldownsToDraw = new List<CooldownIndicator>();
			//CooldownIndicator testCD = new GlobalDodgeCooldown(130)
			//{
			//	TimeLeft = (int)(130 * (1 - (Main.GlobalTime * 0.1f % 1)))
			//};
			//cooldownsToDraw.Add(testCD);
			
			if (cooldownsToDraw.Count == 0)
				return;

			float uiScale = Main.UIScale;
			Vector2 displayPosition = BaseDrawPosition;
			int rectangleSide = (int)Math.Floor(CompactIcons ? 24 * uiScale : 52 * uiScale);
			Rectangle iconRectangle = new Rectangle((int)displayPosition.X - rectangleSide / 2, (int)displayPosition.Y - rectangleSide / 2, rectangleSide, rectangleSide);
			Rectangle mouse = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);

			string mouseHover = "";

			foreach (CooldownIndicator cd in cooldownsToDraw)
            {
				float iconOpacity = (float)Math.Sin(Main.GlobalTime) * 0.1f + 0.6f;

				//Icons get brighter if the mouse gets closer
				iconOpacity += 0.3f * (1 - MathHelper.Clamp(Vector2.Distance(mouse.Center.ToVector2(), iconRectangle.Center.ToVector2()), 0f, 80f) / 80f);

				if (iconRectangle.Intersects(mouse))
				{
					mouseHover = cd.Name;
					iconOpacity = MathHelper.Clamp((float)Math.Sin(Main.GlobalTime % MathHelper.Pi) * 2f, 0, 1) * 0.1f + 0.9f;
				}

				if ((cd.UseCustomDraw && !CompactIcons) || (cd.UseCustomDrawCompact && CompactIcons))
				{
					if (CompactIcons)
						cd.CustomDrawCompact(spriteBatch, displayPosition, iconOpacity, uiScale);
					else
						cd.CustomDraw(spriteBatch, displayPosition, iconOpacity, uiScale);
				}

				else
					DrawIndividualIcon(cd, spriteBatch, displayPosition, iconOpacity, uiScale);

				displayPosition += Spacing;
				iconRectangle.X += (int)Spacing.X;
			}

			if (mouseHover != "")
            {
				Main.LocalPlayer.mouseInterface = true;
				Main.instance.MouseText(mouseHover);
			}

		}

		public static void DrawIndividualIcon(CooldownIndicator cooldown, SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
		{
			Texture2D sprite = ModContent.GetTexture(cooldown.Texture);
			Texture2D outline = ModContent.GetTexture(cooldown.TextureOutline);
			Texture2D barBase = ModContent.GetTexture(cooldown.ChargeBarTexture);

			//Draw the ring
			if (!CompactIcons)
			{
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);
				cooldown.ApplyBarShaders(opacity);

				spriteBatch.Draw(barBase, position, null, Color.White * opacity, 0, barBase.Size() * 0.5f, scale, SpriteEffects.None, 0f);

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
			}

			//Draw the outline
			spriteBatch.Draw(outline, position, null, cooldown.OutlineColor * opacity, 0, outline.Size() * 0.5f, scale, SpriteEffects.None, 0f);

			//Draw the icon
			spriteBatch.Draw(sprite, position, null, Color.White * opacity, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);

			//Draw the small overlay
			if (CompactIcons)
			{
				Texture2D overlay = ModContent.GetTexture(cooldown.TextureOverlay);

				int lostHeight = (int)Math.Ceiling(overlay.Height * (1 - cooldown.Completion));
				Rectangle crop = new Rectangle(0, lostHeight, overlay.Width, overlay.Height - lostHeight);
				spriteBatch.Draw(overlay, position + Vector2.UnitY * lostHeight * scale, crop, cooldown.OutlineColor * opacity * 0.9f, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);
			}
		}
	}
}
