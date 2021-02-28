using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;
using Terraria;

namespace CalamityMod
{
	public static partial class CalamityUtils
	{
		public static void DrawItemGlowmaskSingleFrame(this Item item, SpriteBatch spriteBatch, float rotation, Texture2D glowmaskTexture)
		{
			Vector2 origin = new Vector2(glowmaskTexture.Width / 2f, glowmaskTexture.Height / 2f - 2f);
			spriteBatch.Draw(glowmaskTexture, item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
		}

		public static Rectangle GetCurrentFrame(this Item item, ref int frame, ref int frameCounter, int frameDelay, int frameAmt, bool frameCounterUp = true)
		{
			if (frameCounter >= frameDelay)
			{
				frameCounter = -1;
				frame = frame == frameAmt - 1 ? 0 : frame + 1;
			}
			if (frameCounterUp)
				frameCounter++;
			return new Rectangle(0, item.height * frame, item.width, item.height);
		}

		public static bool DrawFishingLine(this Projectile projectile, int fishingRodType, Color poleColor, int xPositionAdditive = 45, float yPositionAdditive = 35f)
		{
			Player player = Main.player[projectile.owner];
			Item item = player.HeldItem;
			if (!projectile.bobber || item.holdStyle <= 0)
				return false;

			float originX = player.MountedCenter.X;
			float originY = player.MountedCenter.Y;
			originY += player.gfxOffY;
			//This variable is used to account for Gravitation Potions
			float gravity = player.gravDir;

			if (item.type == fishingRodType)
			{
				originX += (float)(xPositionAdditive * player.direction);
				if (player.direction < 0)
				{
					originX -= 13f;
				}
				originY -= yPositionAdditive * gravity;
			}

			if (gravity == -1f)
			{
				originY -= 12f;
			}
			Vector2 mountedCenter = new Vector2(originX, originY);
			mountedCenter = player.RotatedRelativePoint(mountedCenter + new Vector2(8f), true) - new Vector2(8f);
			Vector2 lineOrigin = projectile.Center - mountedCenter;
			bool canDraw = true;
			if (lineOrigin.X == 0f && lineOrigin.Y == 0f)
				return false;

			float projPosMagnitude = lineOrigin.Length();
			projPosMagnitude = 12f / projPosMagnitude;
			lineOrigin.X *= projPosMagnitude;
			lineOrigin.Y *= projPosMagnitude;
			mountedCenter -= lineOrigin;
			lineOrigin = projectile.Center - mountedCenter;

			while (canDraw)
			{
				float height = 12f;
				float positionMagnitude = lineOrigin.Length();
				if (float.IsNaN(positionMagnitude) || float.IsNaN(positionMagnitude))
					break;

				if (positionMagnitude < 20f)
				{
					height = positionMagnitude - 8f;
					canDraw = false;
				}
				positionMagnitude = 12f / positionMagnitude;
				lineOrigin.X *= positionMagnitude;
				lineOrigin.Y *= positionMagnitude;
				mountedCenter += lineOrigin;
				lineOrigin = projectile.Center - mountedCenter;
				if (positionMagnitude > 12f)
				{
					float positionInverseMultiplier = 0.3f;
					float absVelocitySum = Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y);
					if (absVelocitySum > 16f)
					{
						absVelocitySum = 16f;
					}
					absVelocitySum = 1f - absVelocitySum / 16f;
					positionInverseMultiplier *= absVelocitySum;
					absVelocitySum = positionMagnitude / 80f;
					if (absVelocitySum > 1f)
					{
						absVelocitySum = 1f;
					}
					positionInverseMultiplier *= absVelocitySum;
					if (positionInverseMultiplier < 0f)
					{
						positionInverseMultiplier = 0f;
					}
					absVelocitySum = 1f - projectile.localAI[0] / 100f;
					positionInverseMultiplier *= absVelocitySum;
					if (lineOrigin.Y > 0f)
					{
						lineOrigin.Y *= 1f + positionInverseMultiplier;
						lineOrigin.X *= 1f - positionInverseMultiplier;
					}
					else
					{
						absVelocitySum = Math.Abs(projectile.velocity.X) / 3f;
						if (absVelocitySum > 1f)
						{
							absVelocitySum = 1f;
						}
						absVelocitySum -= 0.5f;
						positionInverseMultiplier *= absVelocitySum;
						if (positionInverseMultiplier > 0f)
						{
							positionInverseMultiplier *= 2f;
						}
						lineOrigin.Y *= 1f + positionInverseMultiplier;
						lineOrigin.X *= 1f - positionInverseMultiplier;
					}
				}
				//This color decides the color of the fishing line.
				Color lineColor = Lighting.GetColor((int)mountedCenter.X / 16, (int)mountedCenter.Y / 16, poleColor);
				float rotation = lineOrigin.ToRotation() - MathHelper.PiOver2;

				Main.spriteBatch.Draw(Main.fishingLineTexture, new Vector2(mountedCenter.X - Main.screenPosition.X + Main.fishingLineTexture.Width * 0.5f, mountedCenter.Y - Main.screenPosition.Y + Main.fishingLineTexture.Height * 0.5f), new Rectangle?(new Rectangle(0, 0, Main.fishingLineTexture.Width, (int)height)), lineColor, rotation, new Vector2(Main.fishingLineTexture.Width * 0.5f, 0f), 1f, SpriteEffects.None, 0f);
			}
			return false;
		}

		public static void DrawHook(this Projectile projectile, Texture2D hookTexture, float angleAdditive = 0f)
		{
			Player player = Main.player[projectile.owner];
			Vector2 center = projectile.Center;
			float angleToMountedCenter = projectile.AngleTo(player.MountedCenter) - MathHelper.PiOver2;
			bool canShowHook = true;
			while (canShowHook)
			{
				float distanceMagnitude = (player.MountedCenter - center).Length(); //Exact same as using a Sqrt
				if (distanceMagnitude < hookTexture.Height + 1f)
				{
					canShowHook = false;
				}
				else if (float.IsNaN(distanceMagnitude))
				{
					canShowHook = false;
				}
				else
				{
					center += projectile.DirectionTo(player.MountedCenter) * hookTexture.Height;
					Color tileAtCenterColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16f));
					Main.spriteBatch.Draw(hookTexture, center - Main.screenPosition,
						new Rectangle?(new Rectangle(0, 0, hookTexture.Width, hookTexture.Height)),
						tileAtCenterColor, angleToMountedCenter + angleAdditive,
						hookTexture.Size() / 2, 1f, SpriteEffects.None, 0f);
				}
			}
		}

		internal static void IterateDisco(ref Color c, ref float aiParam, in byte discoIter = 7)
		{
			switch (aiParam)
			{
				case 0f:
					c.G += discoIter;
					if (c.G >= 255)
					{
						c.G = 255;
						aiParam = 1f;
					}
					break;
				case 1f:
					c.R -= discoIter;
					if (c.R <= 0)
					{
						c.R = 0;
						aiParam = 2f;
					}
					break;
				case 2f:
					c.B += discoIter;
					if (c.B >= 255)
					{
						c.B = 255;
						aiParam = 3f;
					}
					break;
				case 3f:
					c.G -= discoIter;
					if (c.G <= 0)
					{
						c.G = 0;
						aiParam = 4f;
					}
					break;
				case 4f:
					c.R += discoIter;
					if (c.R >= 255)
					{
						c.R = 255;
						aiParam = 5f;
					}
					break;
				case 5f:
					c.B -= discoIter;
					if (c.B <= 0)
					{
						c.B = 0;
						aiParam = 0f;
					}
					break;
				default:
					aiParam = 0f;
					c = Color.Red;
					break;
			}
		}

		public static string ColorMessage(string msg, Color color)
		{
			StringBuilder sb = new StringBuilder(msg.Length + 12);
			sb.Append("[c/").Append(color.Hex3()).Append(':').Append(msg).Append(']');
			return sb.ToString();
		}

		/// <summary>
		/// Returns a color lerp that allows for smooth transitioning between two given colors
		/// </summary>
		/// <param name="firstColor">The first color you want it to switch between</param>
		/// <param name="secondColor">The second color you want it to switch between</param>
		/// <param name="seconds">How long you want it to take to swap between colors</param>
		public static Color ColorSwap(Color firstColor, Color secondColor, float seconds)
		{
			double timeMult = (double)(MathHelper.TwoPi / seconds);
			float colorMePurple = (float)((Math.Sin(timeMult * Main.GlobalTime) + 1) * 0.5f);
			return Color.Lerp(firstColor, secondColor, colorMePurple);
		}
		/// <summary>
		/// Returns a color lerp that supports multiple colors.
		/// </summary>
		/// <param name="increment">The 0-1 incremental value used when interpolating.</param>
		/// <param name="colors">The various colors to interpolate across.</param>
		/// <returns></returns>
		public static Color MulticolorLerp(float increment, params Color[] colors)
		{
			increment %= 0.999f;
			int currentColorIndex = (int)(increment * colors.Length);
			Color currentColor = colors[currentColorIndex];
			Color nextColor = colors[(currentColorIndex + 1) % colors.Length];
			return Color.Lerp(currentColor, nextColor, increment * colors.Length % 1f);
		}

		public static void EnterShaderRegion(this SpriteBatch spriteBatch)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
		}

		public static void ExitShaderRegion(this SpriteBatch spriteBatch)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
		}
	}
}
