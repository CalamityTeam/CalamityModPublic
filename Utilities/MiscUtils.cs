using CalamityMod.Items.Tools.ClimateChange;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace CalamityMod
{
	public static partial class CalamityUtils
	{
		public static void DisplayLocalizedText(string key, Color? textColor = null)
		{
			// An attempt to bypass the need for a separate method and runtime/compile-time parameter
			// constraints by using nulls for defaults.
			if (!textColor.HasValue)
				textColor = Color.White;

			if (Main.netMode == NetmodeID.SinglePlayer)
				Main.NewText(Language.GetTextValue(key), textColor.Value);
			else if (Main.netMode == NetmodeID.Server)
				NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), textColor.Value);
		}

		public static T[] ShuffleArray<T>(T[] array, Random rand = null)
		{
			if (rand is null)
				rand = new Random();

			for (int i = array.Length; i > 0; --i)
			{
				int j = rand.Next(i);
				T tempElement = array[j];
				array[j] = array[i - 1];
				array[i - 1] = tempElement;
			}
			return array;
		}

		/// <summary>
		/// Retrieves all the colors from a <see cref="Texture2D"/> and returns them as a 2D <see cref="Color"/> array.
		/// </summary>
		/// <param name="texture">The texture to load.</param>
		/// <returns></returns>
		public static Color[,] GetColorsFromTexture(this Texture2D texture)
		{
			Color[] alignedColors = new Color[texture.Width * texture.Height];
			texture.GetData(alignedColors); // Fills the color array with all the colors in the texture

			Color[,] colors2D = new Color[texture.Width, texture.Height];
			for (int x = 0; x < texture.Width; x++)
			{
				for (int y = 0; y < texture.Height; y++)
				{
					colors2D[x, y] = alignedColors[x + y * texture.Width];
				}
			}
			return colors2D;
		}

		public static string CombineStrings(params object[] args)
		{
			StringBuilder result = new StringBuilder(1024);
			for (int i = 0; i < args.Length; ++i)
			{
				object o = args[i];
				result.Append(o.ToString());
				result.Append(' ');
			}
			return result.ToString();
		}

		/// <summary>
		/// Determines if a list contains an entry of a specific type. Specifically intended to account for derived types.
		/// </summary>
		/// <typeparam name="T">The base type of the collection.</typeparam>
		/// <param name="collection">The collection.</param>
		/// <param name="type">The type to search for.</param>
		public static bool ContainsType<T>(this IEnumerable<T> collection, Type type) => collection.Any(entry => entry.GetType() == type.GetType());

		/// <summary>
		/// Calculates the sound volume and panning for a sound which is played at the specified location in the game world.<br/>
		/// Note that sound does not play on dedicated servers or during world generation.
		/// </summary>
		/// <param name="soundPos">The position the sound is emitting from. If either X or Y is -1, the sound does not fade with distance.</param>
		/// <param name="ambient">Whether the sound is considered ambient, which makes it use the ambient sound slider in the options. Defaults to false.</param>
		/// <returns>Volume and pan, in that order. Volume is always between 0 and 1. Pan is always between -1 and 1.</returns>
		public static (float, float) CalculateSoundStats(Vector2 soundPos, bool ambient = false)
		{
			float volume = 0f;
			float pan = 0f;

			if (soundPos.X == -1f || soundPos.Y == -1f)
				volume = 1f;
			else if (WorldGen.gen || Main.dedServ || Main.netMode == NetmodeID.Server)
				volume = 0f;
			else
			{
				float topLeftX = Main.screenPosition.X - Main.screenWidth * 2f;
				float topLeftY = Main.screenPosition.Y - Main.screenHeight * 2f;

				// Sounds cannot be heard from more than ~2.5 screens away.
				// This rectangle is 5x5 screens centered on the current screen center position.
				Rectangle audibleArea = new Rectangle((int)topLeftX, (int)topLeftY, Main.screenWidth * 5, Main.screenHeight * 5);
				Rectangle soundHitbox = new Rectangle((int)soundPos.X, (int)soundPos.Y, 1, 1);
				Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth * 0.5f, Main.screenHeight * 0.5f);
				if (audibleArea.Intersects(soundHitbox))
				{
					pan = (soundPos.X - screenCenter.X) / (Main.screenWidth * 0.5f);
					float dist = Vector2.Distance(soundPos, screenCenter);
					volume = 1f - (dist / (Main.screenWidth * 1.5f));
				}
			}

			pan = MathHelper.Clamp(pan, -1f, 1f);
			volume = MathHelper.Clamp(volume, 0f, 1f);
			if (ambient)
				volume = Main.gameInactive ? 0f : volume * Main.ambientVolume;
			else
				volume *= Main.soundVolume;

			// This is actually done by vanilla. I guess if the sound volume gets corrupted during gameplay, you can't blast your eardrums out.
			volume = MathHelper.Clamp(volume, 0f, 1f);
			return (volume, pan);
		}

		/// <summary>
		/// Convenience function to utilize CalculateSoundStats immediately on an existing sound effect.<br/>
		/// This allows updating a looping sound every single frame to have the correct volume and pan, even if the player drags the audio sliders around.
		/// </summary>
		/// <param name="sfx">The SoundEffectInstance which is having its values updated.</param>
		/// <param name="soundPos">The position the sound is emitting from. If either X or Y is -1, the sound does not fade with distance.</param>
		/// <param name="ambient">Whether the sound is considered ambient, which makes it use the ambient sound slider in the options. Defaults to false.</param>
		public static void ApplySoundStats(ref SoundEffectInstance sfx, Vector2 soundPos, bool ambient = false)
		{
			if (sfx is null || sfx.IsDisposed)
				return;
			(sfx.Volume, sfx.Pan) = CalculateSoundStats(soundPos, ambient);
		}

		public static void StartRain(bool torrentialTear = false)
		{
			int num = 86400;
			int num2 = num / 24;
			Main.rainTime = Main.rand.Next(num2 * 8, num);
			if (Main.rand.NextBool(3))
			{
				Main.rainTime += Main.rand.Next(0, num2);
			}
			if (Main.rand.NextBool(4))
			{
				Main.rainTime += Main.rand.Next(0, num2 * 2);
			}
			if (Main.rand.NextBool(5))
			{
				Main.rainTime += Main.rand.Next(0, num2 * 2);
			}
			if (Main.rand.NextBool(6))
			{
				Main.rainTime += Main.rand.Next(0, num2 * 3);
			}
			if (Main.rand.NextBool(7))
			{
				Main.rainTime += Main.rand.Next(0, num2 * 4);
			}
			if (Main.rand.NextBool(8))
			{
				Main.rainTime += Main.rand.Next(0, num2 * 5);
			}
			float num3 = 1f;
			if (Main.rand.NextBool(2))
			{
				num3 += 0.05f;
			}
			if (Main.rand.NextBool(3))
			{
				num3 += 0.1f;
			}
			if (Main.rand.NextBool(4))
			{
				num3 += 0.15f;
			}
			if (Main.rand.NextBool(5))
			{
				num3 += 0.2f;
			}
			Main.rainTime = (int)(Main.rainTime * num3);
			Main.raining = true;
			if (torrentialTear)
				TorrentialTear.AdjustRainSeverity(false);
			CalamityNetcode.SyncWorld();
		}

		public static void StartSandstorm()
		{
			typeof(Terraria.GameContent.Events.Sandstorm).GetMethod("StartSandstorm", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
		}

		public static void StopSandstorm()
		{
			Terraria.GameContent.Events.Sandstorm.Happening = false;
		}

		public static void AddWithCondition<T>(this List<T> list, T type, bool condition)
		{
			if (condition)
				list.Add(type);
		}

		public static void Inflict246DebuffsNPC(NPC target, int buff, float timeBase = 2f)
		{
			if (Main.rand.NextBool(4))
			{
				target.AddBuff(buff, SecondsToFrames(timeBase * 3f), false);
			}
			else if (Main.rand.NextBool(2))
			{
				target.AddBuff(buff, SecondsToFrames(timeBase * 2f), false);
			}
			else
			{
				target.AddBuff(buff, SecondsToFrames(timeBase), false);
			}
		}

		public static void Inflict246DebuffsPvp(Player target, int buff, float timeBase = 2f)
		{
			if (Main.rand.NextBool(4))
			{
				target.AddBuff(buff, SecondsToFrames(timeBase * 3f), false);
			}
			else if (Main.rand.NextBool(2))
			{
				target.AddBuff(buff, SecondsToFrames(timeBase * 2f), false);
			}
			else
			{
				target.AddBuff(buff, SecondsToFrames(timeBase), false);
			}
		}

		public static int SecondsToFrames(int seconds) => seconds * 60;
		public static int SecondsToFrames(float seconds) => (int)(seconds * 60);

		/// <summary>
		/// Call this function to spawn explosion clouds at the specified location. Good for when NPCs or projectiles die and need to explode.
		/// </summary>
		/// <param name="goreSource">The spot to spawn the explosion clouds</param>
		/// <param name="goreAmt">Number of times it loops to spawn gores</param>
		public static void ExplosionGores(Vector2 goreSource, int goreAmt)
		{
			Vector2 source = new Vector2(goreSource.X - 24f, goreSource.Y - 24f);
			for (int goreIndex = 0; goreIndex < goreAmt; goreIndex++)
			{
				float velocityMult = 0.33f;
				if (goreIndex < (int)(goreAmt / 3))
				{
					velocityMult = 0.66f;
				}
				if (goreIndex >= (int)((2 * goreAmt) / 3))
				{
					velocityMult = 1f;
				}
				int smoke = Gore.NewGore(source, default, Main.rand.Next(61, 64), 1f);
				Gore gore = Main.gore[smoke];
				gore.velocity *= velocityMult;
				gore.velocity.X += 1f;
				gore.velocity.Y += 1f;
				smoke = Gore.NewGore(source, default, Main.rand.Next(61, 64), 1f);
				gore.velocity *= velocityMult;
				gore.velocity.X -= 1f;
				gore.velocity.Y += 1f;
				smoke = Gore.NewGore(source, default, Main.rand.Next(61, 64), 1f);
				gore.velocity *= velocityMult;
				gore.velocity.X += 1f;
				gore.velocity.Y -= 1f;
				smoke = Gore.NewGore(source, default, Main.rand.Next(61, 64), 1f);
				gore.velocity *= velocityMult;
				gore.velocity.X -= 1f;
				gore.velocity.Y -= 1f;
			}
		}

		public static bool WithinBounds(this int index, int cap) => index >= 0 && index < cap;

		// REMOVE THIS IN CALAMITY 1.4, it's a 1.4 Main.cs function
		public static float GetLerpValue(float from, float to, float t, bool clamped = false)
		{
			if (clamped)
			{
				if (from < to)
				{
					if (t < from)
					{
						return 0f;
					}
					if (t > to)
					{
						return 1f;
					}
				}
				else
				{
					if (t < to)
					{
						return 1f;
					}
					if (t > from)
					{
						return 0f;
					}
				}
			}
			return (t - from) / (to - from);
		}

		/// <summary>
		/// Clamps the distance between vectors via normalization.
		/// </summary>
		/// <param name="start">The starting point.</param>
		/// <param name="end">The ending point.</param>
		/// <param name="maxDistance">The maximum possible distance between the two vectors before they get clamped.</param>
		public static void DistanceClamp(ref Vector2 start, ref Vector2 end, float maxDistance)
		{
			if (Vector2.Distance(end, start) > maxDistance)
			{
				end = start + Vector2.Normalize(end - start) * maxDistance;
			}
		}

		// REMOVE THIS IN CALAMITY 1.4, it's a 1.4 World.cs function
		public static Rectangle ClampToWorld(Rectangle tileRectangle)
		{
			int num = Math.Max(0, Math.Min(tileRectangle.Left, Main.maxTilesX));
			int num2 = Math.Max(0, Math.Min(tileRectangle.Top, Main.maxTilesY));
			int num3 = Math.Max(0, Math.Min(tileRectangle.Right, Main.maxTilesX));
			int num4 = Math.Max(0, Math.Min(tileRectangle.Bottom, Main.maxTilesY));
			return new Rectangle(num, num2, num3 - num, num4 - num2);
		}
	}
}
