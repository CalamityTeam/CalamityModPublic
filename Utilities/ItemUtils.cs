using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod
{
	public static partial class CalamityUtils
	{
		public static readonly CalamityRarity[] postMLRarities =
		{
			CalamityRarity.Turquoise,
			CalamityRarity.PureGreen,
			CalamityRarity.DarkBlue,
			CalamityRarity.Violet,
			CalamityRarity.HotPink,
			CalamityRarity.Rainbow
		};
		public static bool IsPostML(this CalamityRarity calrare)
		{
			return calrare != CalamityRarity.NoEffect;
			// TODO -- separate out whether an item is post-ML from its custom rarity.
			// This is necessary because there are pre-ML rare variants, legendary weapons and dedicated items.
			/*
			for(int i = 0; i < postMLRarities.Length; ++i)
				if (postMLRarities[i] == calrare)
					return true;
			return false;
			*/
		}

		/// <summary>
		/// Converts the given ModHotKey into a string for insertion into item tooltips.<br></br>
		/// This allows the user's actual keybind choices to be shown to them in tooltips.
		/// </summary>
		/// <param name="mhk">The ModHotKey to convert to a string.</param>
		/// <returns></returns>
		public static string TooltipHotkeyString(this ModHotKey mhk)
		{
			if (Main.dedServ || mhk is null)
				return "";

			List<string> keys = mhk.GetAssignedKeys();
			if (keys.Count == 0)
				return "[NONE]";
			else
			{
				StringBuilder sb = new StringBuilder(16);
				sb.Append(keys[0]);

				// In almost all cases, this code won't run, because there won't be multiple bindings for the hotkey. But just in case...
				for (int i = 1; i < keys.Count; ++i)
					sb.Append(" / ").Append(keys[i]);
				return sb.ToString();
			}
		}

		private const float WorldInsertionOffset = 15f;
		/// <summary>
		/// If the given item is outside the world, force it to be within the world boundaries.
		/// </summary>
		/// <param name="item">The item to possibly relocate.</param>
		/// <param name="dist">The minimum distance in pixels the item can be from the world boundary.</param>
		/// <returns>Whether the item was relocated.</returns>
		public static bool ForceItemIntoWorld(Item item, float desiredDist = WorldInsertionOffset)
		{
			if (item is null || !item.active)
				return false;

			// The world edge needs to be accounted for regardless of the distance chosen as an argument.
			float worldEdge = Main.offLimitBorderTiles * 16f;
			float dist = worldEdge + desiredDist;

			float maxPosX = Main.maxTilesX * 16f;
			float maxPosY = Main.maxTilesY * 16f;
			bool moved = false;
			if (item.position.X < worldEdge)
			{
				item.position.X = dist;
				moved = true;
			}
			else if (item.position.X + item.width > maxPosX - worldEdge)
			{
				item.position.X = maxPosX - item.width - dist;
				moved = true;
			}
			if (item.position.Y < worldEdge)
			{
				item.position.Y = dist;
				moved = true;
			}
			else if (item.position.Y + item.height > maxPosY - worldEdge)
			{
				item.position.Y = maxPosY - item.height - dist;
				moved = true;
			}
			return moved;
		}

		public static Rectangle FixSwingHitbox(float hitboxWidth, float hitboxHeight)
		{
			Player player = Main.player[Main.myPlayer];
			Item item = player.ActiveItem();
			float hitbox_X, hitbox_Y;
			float mountOffsetY = player.mount.PlayerOffsetHitbox;

			// Third hitbox shifting values
			if (player.itemAnimation < player.itemAnimationMax * 0.333)
			{
				float shiftX = 10f;
				if (hitboxWidth >= 92)
					shiftX = 38f;
				else if (hitboxWidth >= 64)
					shiftX = 28f;
				else if (hitboxWidth >= 52)
					shiftX = 24f;
				else if (hitboxWidth > 32)
					shiftX = 14f;
				hitbox_X = player.position.X + player.width * 0.5f + (hitboxWidth * 0.5f - shiftX) * player.direction;
				hitbox_Y = player.position.Y + 24f + mountOffsetY;
			}

			// Second hitbox shifting values
			else if (player.itemAnimation < player.itemAnimationMax * 0.666)
			{
				float shift = 10f;
				if (hitboxWidth >= 92)
					shift = 38f;
				else if (hitboxWidth >= 64)
					shift = 28f;
				else if (hitboxWidth >= 52)
					shift = 24f;
				else if (hitboxWidth > 32)
					shift = 18f;
				hitbox_X = player.position.X + (player.width * 0.5f + (hitboxWidth * 0.5f - shift) * player.direction);

				shift = 10f;
				if (hitboxHeight > 64)
					shift = 14f;
				else if (hitboxHeight > 52)
					shift = 12f;
				else if (hitboxHeight > 32)
					shift = 8f;

				hitbox_Y = player.position.Y + shift + mountOffsetY;
			}

			// First hitbox shifting values
			else
			{
				float shift = 6f;
				if (hitboxWidth >= 92)
					shift = 38f;
				else if (hitboxWidth >= 64)
					shift = 28f;
				else if (hitboxWidth >= 52)
					shift = 24f;
				else if (hitboxWidth >= 48)
					shift = 18f;
				else if (hitboxWidth > 32)
					shift = 14f;
				hitbox_X = player.position.X + player.width * 0.5f - (hitboxWidth * 0.5f - shift) * player.direction;

				shift = 10f;
				if (hitboxHeight > 64)
					shift = 14f;
				else if (hitboxHeight > 52)
					shift = 12f;
				else if (hitboxHeight > 32)
					shift = 10f;
				hitbox_Y = player.position.Y + shift + mountOffsetY;
			}

			// Inversion due to grav potion
			if (player.gravDir == -1f)
			{
				hitbox_Y = player.position.Y + player.height + (player.position.Y - hitbox_Y);
			}

			// Hitbox size adjustments
			Rectangle hitbox = new Rectangle((int)hitbox_X, (int)hitbox_Y, 32, 32);
			if (item.damage >= 0 && item.type > ItemID.None && !item.noMelee && player.itemAnimation > 0)
			{
				if (!Main.dedServ)
				{
					hitbox = new Rectangle((int)hitbox_X, (int)hitbox_Y, (int)hitboxWidth, (int)hitboxHeight);
				}
				hitbox.Width = (int)(hitbox.Width * item.scale);
				hitbox.Height = (int)(hitbox.Height * item.scale);
				if (player.direction == -1)
				{
					hitbox.X -= hitbox.Width;
				}
				if (player.gravDir == 1f)
				{
					hitbox.Y -= hitbox.Height;
				}

				// Broadsword use style
				if (item.useStyle == ItemUseStyleID.SwingThrow)
				{
					// Third hitbox size adjustments
					if (player.itemAnimation < player.itemAnimationMax * 0.333)
					{
						if (player.direction == -1)
						{
							hitbox.X -= (int)(hitbox.Width * 1.4 - hitbox.Width);
						}
						hitbox.Width = (int)(hitbox.Width * 1.4);
						hitbox.Y += (int)(hitbox.Height * 0.5 * player.gravDir);
						hitbox.Height = (int)(hitbox.Height * 1.1);
					}

					// First hitbox size adjustments
					else if (player.itemAnimation >= player.itemAnimationMax * 0.666)
					{
						if (player.direction == 1)
						{
							hitbox.X -= (int)(hitbox.Width * 1.2);
						}
						hitbox.Width *= 2;
						hitbox.Y -= (int)((hitbox.Height * 1.4 - hitbox.Height) * player.gravDir);
						hitbox.Height = (int)(hitbox.Height * 1.4);
					}
				}
			}
			return hitbox;
		}

		public static void ConsumeItemViaQuickBuff(Player player, Item item, int buffType, int buffTime, bool reducedPotionSickness)
		{
			bool showsOver = false;
			//Fail if you have the buff
			for (int l = 0; l < Player.MaxBuffs; l++)
			{
				int hasBuff = player.buffType[l];
				if (player.buffTime[l] > 0 && hasBuff == buffType)
					showsOver = true;
			}
			//Fail if you have potion sickness
			if (player.potionDelay > 0 || player.Calamity().potionTimer > 0)
				showsOver = true;

			if (!showsOver)
			{
				Main.PlaySound(item.UseSound, player.Center);

				double healMult = 1D +
						(player.Calamity().coreOfTheBloodGod ? 0.15 : 0) +
						(player.Calamity().bloodPactBoost ? 0.5 : 0);
				int healAmt = (int)(item.healLife * healMult);
				if (CalamityWorld.ironHeart)
					healAmt = 0;
				if (healAmt > 0 && player.QuickHeal_GetItemToUse() != null)
				{
					if (player.QuickHeal_GetItemToUse().type != item.type)
						healAmt = 0;
				}

				player.statLife += healAmt;
				player.statMana += item.healMana;
				if (player.statMana > player.statManaMax2)
				{
					player.statMana = player.statManaMax2;
				}
				if (player.statLife > player.statLifeMax2)
				{
					player.statLife = player.statLifeMax2;
				}
				if (item.healMana > 0)
					player.AddBuff(BuffID.ManaSickness, Player.manaSickTime, true);
				if (Main.myPlayer == player.whoAmI)
				{
					if (healAmt > 0)
						player.HealEffect(healAmt, true);
					if (item.healMana > 0)
						player.ManaEffect(item.healMana);
				}
				if (item.potion && healAmt > 0) //Don't inflict Potion Sickness if you don't actually heal
				{
					int duration = reducedPotionSickness ? 3000 : 3600;
					if (player.pStone)
						duration = (int)(duration * 0.75);
					player.AddBuff(BuffID.PotionSickness, duration);
				}

				player.AddBuff(buffType, buffTime);

				--item.stack;
				if (item.stack <= 0)
					item.TurnToAir();
				Recipe.FindRecipes();
			}
		}
	}
}
