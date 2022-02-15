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
		public static bool CompactIcons
		{
			get
			{
				return Main.LocalPlayer.Calamity().Cooldowns.Count > MaxLargeIcons || CalamityConfig.Instance.CooldownDisplay == 1;
			}
		}

		/// <summary>
		/// How many segments are displayed in a ring around the icon when compact mode is is off
		/// </summary>
		static float RingSmoothness = MathHelper.TwoPi / 60f;

		public static Vector2 Spacing
		{
			get
			{ 
				return CompactIcons ? Vector2.UnitX * Main.UIScale * 28f : Vector2.UnitX * Main.UIScale * 54f;
			}
		}

		public static Vector2 BaseDrawPosition
        {
			get
			{
				return new Vector2(32, 100) * Main.UIScale + Spacing / 2f + (Main.LocalPlayer.CountBuffs() > 0 ? Vector2.UnitY * 50 * Main.UIScale : Vector2.Zero) + (Main.LocalPlayer.CountBuffs() > 11 ? Vector2.UnitY * 50 * Main.UIScale : Vector2.Zero);
			}
        }

		public static void Draw(SpriteBatch spriteBatch)
		{
			//Don't draw the cooldowns if the player's inventory is open
			if (Main.playerInventory || CalamityConfig.Instance.CooldownDisplay < 1)
				return;

			List<CooldownIndicator> cooldownsToDraw = Main.LocalPlayer.Calamity().Cooldowns;

			cooldownsToDraw = new List<CooldownIndicator>();

			CooldownIndicator testCD = new NebulousCoreCooldown(30)
			{
				TimeLeft = 5
			};
			cooldownsToDraw.Add(testCD);
			


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

				if (cd.UseCustomDraw)
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

			//Draw the ring
			if (!CompactIcons)
			{
				Texture2D bar = ModContent.GetTexture("CalamityMod/UI/CooldownIndicators/MiniBar");
				Rectangle emptyCrop = new Rectangle(0, 0, 2, 22);
				Rectangle fullCrop = new Rectangle(6, 0, 2, 22);
				Rectangle capCrop = new Rectangle(12, 0, 2, 22);
				Vector2 origin = new Vector2(1, 22);
				bool cappedBar = false;

				for (float i = 0; i < MathHelper.TwoPi; i += RingSmoothness)
				{
					Rectangle crop = i / MathHelper.TwoPi > cooldown.Completion ? (cappedBar ? fullCrop : capCrop) : emptyCrop;
					spriteBatch.Draw(bar, position, crop, cooldown.CooldownColor(i / MathHelper.TwoPi) * opacity, i, origin, scale, SpriteEffects.None, 0f);
					cappedBar = true;
				}
			}

			//Draw the outline
			spriteBatch.Draw(outline, position, null, cooldown.OutlineColor() * opacity, 0, outline.Size() * 0.5f, scale, SpriteEffects.None, 0f);

			//Draw the icon
			spriteBatch.Draw(sprite, position, null, Color.White * opacity, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);

			//Draw the small overlay
			if (CompactIcons)
			{
				Texture2D overlay = ModContent.GetTexture(cooldown.TextureOverlay);

				int lostHeight = (int)Math.Ceiling(overlay.Height * (1 - cooldown.Completion));
				Rectangle crop = new Rectangle(0, lostHeight, overlay.Width, overlay.Height - lostHeight);
				spriteBatch.Draw(overlay, position + Vector2.UnitY * lostHeight * scale, crop, cooldown.OutlineColor() * opacity * 0.7f, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);
			}
		}
	}
}
