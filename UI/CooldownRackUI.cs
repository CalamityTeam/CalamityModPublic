using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
	public class CooldownRackUI
	{
		/// <summary>
		/// The maximum number of cooldowns which can be drawn in expanded mode before the rack auto-switches to compact mode.
		/// </summary>
		public static int MaxLargeIcons = 10;

		public static bool CompactIcons
		{
			get
			{
				// Option 1: Always use compact icons if configured to do so.
				if (CalamityConfig.Instance.CooldownDisplay == 1)
					return true;

				// Option 2: If there are too many cooldowns, auto switch to compact mode.
				return Main.LocalPlayer.GetDisplayedCooldowns().Count > MaxLargeIcons;
			}
		}

		// TODO -- more of these display positioning constants
		public const float CompactXSpacing = 28f;
		public const float ExpandedXScaling = 46f;
		public static Vector2 Spacing => CompactIcons ? Vector2.UnitX * Main.UIScale * CompactXSpacing : Vector2.UnitX * Main.UIScale * ExpandedXScaling;

		public static Vector2 BaseDrawPosition => new Vector2(32, 100) * Main.UIScale + Spacing / 2f + (Main.LocalPlayer.CountBuffs() > 0 ? Vector2.UnitY * 50 * Main.UIScale : Vector2.Zero) + (Main.LocalPlayer.CountBuffs() > 11 ? Vector2.UnitY * 50 * Main.UIScale : Vector2.Zero);

		public static void Draw(SpriteBatch spriteBatch)
		{
			// Don't draw the cooldowns if the player's inventory is open, or if cooldown display is completely disabled.
			if (Main.playerInventory || CalamityConfig.Instance.CooldownDisplay < 1)
				return;

			IList<CooldownInstance> cooldownsToDraw = Main.LocalPlayer.GetDisplayedCooldowns();
			if (cooldownsToDraw.Count == 0)
				return;

			float uiScale = Main.UIScale;
			Vector2 displayPosition = BaseDrawPosition;
			int rectangleSide = (int)Math.Floor(CompactIcons ? 24 * uiScale : 52 * uiScale);
			Rectangle iconRectangle = new Rectangle((int)displayPosition.X - rectangleSide / 2, (int)displayPosition.Y - rectangleSide / 2, rectangleSide, rectangleSide);
			Rectangle mouse = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);

			string mouseHover = "";

			foreach (CooldownInstance instance in cooldownsToDraw)
			{
				CooldownHandler handler = instance.handler;
				float iconOpacity = (float)Math.Sin(Main.GlobalTime) * 0.1f + 0.6f;

				// Icons get brighter if the mouse gets closer
				iconOpacity += 0.3f * (1 - MathHelper.Clamp(Vector2.Distance(mouse.Center.ToVector2(), iconRectangle.Center.ToVector2()), 0f, 80f) / 80f);

				if (iconRectangle.Intersects(mouse))
				{
					mouseHover = handler.DisplayName;
					iconOpacity = MathHelper.Clamp((float)Math.Sin(Main.GlobalTime % MathHelper.Pi) * 2f, 0, 1) * 0.1f + 0.9f;
				}

				if (handler.UseCustomExpandedDraw && !CompactIcons || handler.UseCustomCompactDraw && CompactIcons)
				{
					if (CompactIcons)
						handler.DrawCompact(spriteBatch, displayPosition, iconOpacity, uiScale);
					else
						handler.DrawExpanded(spriteBatch, displayPosition, iconOpacity, uiScale);
				}

				else
					DrawIndividualIcon(instance, spriteBatch, displayPosition, iconOpacity, uiScale);

				displayPosition += Spacing;
				iconRectangle.X += (int)Spacing.X;
			}

			if (mouseHover != "")
			{
				Main.LocalPlayer.mouseInterface = true;
				Main.instance.MouseText(mouseHover);
			}

		}

		public static void DrawIndividualIcon(CooldownInstance instance, SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
		{
			CooldownHandler handler = instance.handler;
			Texture2D sprite = ModContent.GetTexture(handler.Texture);
			Texture2D outline = ModContent.GetTexture(handler.OutlineTexture);
			Texture2D barBase = ModContent.GetTexture(handler.ChargeBarTexture);

			//Draw the ring
			if (!CompactIcons)
			{
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);
				handler.ApplyBarShaders(opacity);

				spriteBatch.Draw(barBase, position, null, Color.White * opacity, 0, barBase.Size() * 0.5f, scale, SpriteEffects.None, 0f);

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
			}

			//Draw the outline
			spriteBatch.Draw(outline, position, null, handler.OutlineColor * opacity, 0, outline.Size() * 0.5f, scale, SpriteEffects.None, 0f);

			//Draw the icon
			spriteBatch.Draw(sprite, position, null, Color.White * opacity, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);

			//Draw the small overlay
			if (CompactIcons)
			{
				Texture2D overlay = ModContent.GetTexture(handler.OverlayTexture);

				int lostHeight = (int)Math.Ceiling(overlay.Height * (1 - instance.Completion));
				Rectangle crop = new Rectangle(0, lostHeight, overlay.Width, overlay.Height - lostHeight);
				spriteBatch.Draw(overlay, position + Vector2.UnitY * lostHeight * scale, crop, handler.OutlineColor * opacity * 0.9f, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);
			}
		}
	}
}
