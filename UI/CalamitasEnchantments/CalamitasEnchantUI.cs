using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace CalamityMod.UI.CalamitasEnchants
{
	public class CalamitasEnchantUI
	{
		public static int NPCIndex = -1;
		public static Enchantment? SelectedEnchantment = null;
		public static float SelectedEnchantmentScaleFactor = 1f;
		public static Item CurrentlyHeldItem = new Item();

		public static bool CurrentlyViewing = false;

		public static readonly Vector2 ReforgeUITopLeft = new Vector2(28f, 270f);
		public static readonly float ResolutionRatio = Main.screenHeight / 1440f;

		public static Rectangle MouseScreenArea => Utils.CenteredRectangle(Main.MouseScreen, Vector2.One * 2f);

		public static bool InRangeOfNPC()
		{
			// Don't bother trying if no valid NPC has been selected yet.
			if (!Main.npc.IndexInRange(NPCIndex) || !Main.npc[NPCIndex].active)
				return false;

			Rectangle validTalkArea = Utils.CenteredRectangle(Main.LocalPlayer.Center, new Vector2(Player.tileRangeX * 2f, Player.tileRangeY) * 16f);
			return validTalkArea.Intersects(Main.npc[NPCIndex].Hitbox);
		}

		public static void Draw(SpriteBatch spriteBatch)
		{
			// Don't bother doing anything except resetting if not looking at the UI.
			if (!CurrentlyViewing)
			{
				// If an item was stored, release it back into the world.
				if (!CurrentlyHeldItem.IsAir)
				{
					Main.LocalPlayer.QuickSpawnClonedItem(CurrentlyHeldItem, CurrentlyHeldItem.stack);
					CurrentlyHeldItem.TurnToAir();
				}

				NPCIndex = -1;
				return;
			}

			// Check if the player can still be in the UI.
			if (Main.LocalPlayer.chest != -1 || Main.LocalPlayer.sign != -1 || Main.LocalPlayer.talkNPC == -1 || !Main.playerInventory || !InRangeOfNPC() || Main.InGuideCraftMenu)
			{
				CurrentlyViewing = false;

				// Check if the player has any items being held.
				// If they do, drop it onto the ground.
				Main.LocalPlayer.dropItemCheck();

				// Reload visible recipes in case the dropped item was an ingredient.
				Recipe.FindRecipes();
				return;
			}

			// Open the inventory and stop talking to any NPCs by default while the UI is open, similar to the goblin tinkerer.
			Main.playerInventory = true;
			Main.npcChatText = string.Empty;

			Texture2D backgroundTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/CalamitasReforgeBackground");
			Texture2D pillarTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/CalamitasReforgePillar");
			Vector2 backgroundScale = MathHelper.Clamp(ResolutionRatio * 1.25f, 0.67f, 1f) * new Vector2(1.3f, 1f);

			// Draw the background.
			spriteBatch.Draw(backgroundTexture, ReforgeUITopLeft, null, Color.White, 0f, Vector2.Zero, backgroundScale, SpriteEffects.None, 0f);

			// As well as a cool pillar to the side.
			Vector2 pillarDrawPosition = ReforgeUITopLeft + Vector2.UnitX * (backgroundScale.X * backgroundTexture.Width + 25f);
			pillarDrawPosition.Y += 20f;
			spriteBatch.Draw(pillarTexture, pillarDrawPosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			DrawIcons(spriteBatch, out bool isHoveringOverItemIcon, out bool isHoveringOverReforgeIcon);
			if (isHoveringOverItemIcon)
				InteractWithItemSlot();

			DisplayEnchantmentOptions(spriteBatch, ReforgeUITopLeft + new Vector2(112f, 55f), 
				out List<Rectangle> textAreas,
				out IEnumerable<Enchantment> possibleEnchantments,
				out Enchantment? enchantmentToUse);

			InteractWithTextAreas(textAreas, possibleEnchantments);

			int cost = 0;
			if (SelectedEnchantment.HasValue)
			{
				cost = DisplayEnchantmentCost(spriteBatch, out Point costDrawPositionTopLeft);
				Point descriptionDrawPositionTopLeft = costDrawPositionTopLeft;
				descriptionDrawPositionTopLeft.Y += 90;

				DisplayEnchantmentDescription(spriteBatch, descriptionDrawPositionTopLeft);
			}

			if (isHoveringOverReforgeIcon && Main.mouseLeft && Main.mouseLeftRelease)
				InteractWithReforgeIcon(enchantmentToUse, cost);
		}

		public static void DrawIcons(SpriteBatch spriteBatch, out bool isHoveringOverItemIcon, out bool isHoveringOverReforgeIcon)
		{
			Texture2D itemSlotTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/CalamitasReforgeSlot");
			Vector2 itemSlotDrawPosition = ReforgeUITopLeft + new Vector2(40f, 65f);
			Vector2 reforgeIconDrawPosition = itemSlotDrawPosition + Vector2.UnitX * (itemSlotTexture.Width * 0.5f + 24f);

			isHoveringOverReforgeIcon = false;
			Rectangle reforgeIconArea = Utils.CenteredRectangle(reforgeIconDrawPosition, Main.reforgeTexture[0].Size());

			Texture2D reforgeIconTexture = Main.reforgeTexture[0];

			// Have the reforge icon light up if the mouse is hovering over it.
			if (MouseScreenArea.Intersects(reforgeIconArea))
			{
				reforgeIconTexture = Main.reforgeTexture[1];
				isHoveringOverReforgeIcon = true;
			}

			// This will be used for item deposit/withdrawal logic.
			isHoveringOverItemIcon = MouseScreenArea.Intersects(Utils.CenteredRectangle(itemSlotDrawPosition, itemSlotTexture.Size()));

			spriteBatch.Draw(itemSlotTexture, itemSlotDrawPosition, null, Color.White, 0f, itemSlotTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);

			// Draw the draw the item within the slot, if it exists.
			if (!CurrentlyHeldItem.IsAir)
				AttemptToDrawItemInIcon(spriteBatch, itemSlotDrawPosition);

			spriteBatch.Draw(reforgeIconTexture, reforgeIconDrawPosition, null, Color.White, 0f, reforgeIconTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
		}

		public static void AttemptToDrawItemInIcon(SpriteBatch spriteBatch, Vector2 drawPosition)
		{
			float inventoryScale = Main.inventoryScale;
			Texture2D itemTexture = Main.itemTexture[CurrentlyHeldItem.type];
			Rectangle itemFrame = itemTexture.Frame(1, 1, 0, 0);
			if (Main.itemAnimations[CurrentlyHeldItem.type] != null)
				itemFrame = Main.itemAnimations[CurrentlyHeldItem.type].GetFrame(itemTexture);

			float baseScale = 1f;
			Color _ = Color.White;
			ItemSlot.GetItemLight(ref _, ref baseScale, CurrentlyHeldItem, false);

			float itemScale = 1f;

			// Ensure that the item being drawn does not exceed a certain size.
			// If it does, constrict its scale to prevent it from going beyond the maximum.
			if (itemFrame.Width > 36 || itemFrame.Height > 36)
				itemScale = 36f / MathHelper.Max(itemFrame.Width, itemFrame.Height);

			itemScale *= inventoryScale * baseScale;

			// Draw the item.
			if (ItemLoader.PreDrawInInventory(CurrentlyHeldItem, spriteBatch, drawPosition, itemFrame, CurrentlyHeldItem.GetAlpha(Color.White), CurrentlyHeldItem.GetColor(Color.White), itemTexture.Size() * 0.5f, itemScale))
			{
				spriteBatch.Draw(itemTexture, drawPosition, itemFrame, CurrentlyHeldItem.GetAlpha(Color.White), 0f, itemTexture.Size() * 0.5f, itemScale, SpriteEffects.None, 0f);
				spriteBatch.Draw(itemTexture, drawPosition, itemFrame, CurrentlyHeldItem.GetColor(Color.White), 0f, itemTexture.Size() * 0.5f, itemScale, SpriteEffects.None, 0f);
			}
		}

		public static void InteractWithItemSlot()
		{
			if (!CurrentlyHeldItem.IsAir)
			{
				// Display item stats.
				Main.HoverItem = CurrentlyHeldItem.Clone();

				// Force the HoverItem to be displayed.
				Main.instance.MouseTextHackZoom(string.Empty);
			}

			// Prevent the player from say, firing a weapon while the mouse is hovering over the slot.
			Main.LocalPlayer.mouseInterface = false;
			Main.blockMouse = true;

			bool isHeldItemEnchantable = Main.mouseItem.damage > 0;

			// Attempt to exchange if the slot is clicked.
			if (Main.mouseLeftRelease && Main.mouseLeft && (isHeldItemEnchantable || Main.mouseItem.IsAir))
			{
				// Reset the enchantment variables.
				SelectedEnchantmentScaleFactor = 1f;
				SelectedEnchantment = null;

				Utils.Swap(ref Main.mouseItem, ref CurrentlyHeldItem);
				Main.PlaySound(SoundID.Grab);
			}
		}

		public static void DisplayEnchantmentOptions(SpriteBatch spriteBatch, Vector2 drawPosition, out List<Rectangle> textAreas, out IEnumerable<Enchantment> possibleEnchantments, out Enchantment? enchantmentToUse)
		{
			enchantmentToUse = null;
			possibleEnchantments = EnchantmentManager.GetValidEnchantmentsForItem(CurrentlyHeldItem);

			// Initialize the areas.
			textAreas = new List<Rectangle>();

			if (SelectedEnchantment is null)
				SelectedEnchantmentScaleFactor = MathHelper.Lerp(SelectedEnchantmentScaleFactor, 1f, 0.15f);
			else
				SelectedEnchantmentScaleFactor = MathHelper.Lerp(SelectedEnchantmentScaleFactor, 1.35f, 0.15f);

			// Don't attempt to draw anything if no valid enchantments exist for an item, for whatever reason.
			if (possibleEnchantments.Count() <= 0)
				return;

			int totalEnchantmentsToDisplay = Math.Min(possibleEnchantments.Count(), 6);
			for (int i = 0; i < totalEnchantmentsToDisplay; i++)
			{
				Color textColor = Color.Orange;
				Vector2 scale = Vector2.One;
				Enchantment enchantment = possibleEnchantments.ElementAt(i);

				// Make the text for the disenchantment option white to differentiate it from
				// everything else.
				if (enchantment.Equals(EnchantmentManager.ClearEnchantment))
					textColor = Color.White;

				// Save this enchantment specifically if it's the one that's going to be selected.
				if (enchantment.Equals(SelectedEnchantment))
				{
					scale *= SelectedEnchantmentScaleFactor;
					enchantmentToUse = enchantment;
				}

				// Draw all options.
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontMouseText, enchantment.Name, drawPosition, textColor, 0f, Vector2.Zero, scale);
				textAreas.Add(new Rectangle((int)drawPosition.X, (int)drawPosition.Y, 180, 30));
				drawPosition.Y += 32f;
			}
		}

		public static void InteractWithTextAreas(List<Rectangle> textAreas, IEnumerable<Enchantment> possibleEnchantments)
		{
			for (int i = 0; i < textAreas.Count; i++)
			{
				Rectangle area = textAreas[i];
				Enchantment enchantmentAtIndex = possibleEnchantments.ElementAt(i);
				if (Main.mouseLeft && Main.mouseLeftRelease && MouseScreenArea.Intersects(area))
				{
					if (SelectedEnchantment.Equals(enchantmentAtIndex))
						SelectedEnchantment = null;
					else
						SelectedEnchantment = enchantmentAtIndex;
					break;
				}
			}
		}

		public static int DisplayEnchantmentCost(SpriteBatch spriteBatch, out Point costDrawPositionTopLeft)
		{
			costDrawPositionTopLeft = (ReforgeUITopLeft + new Vector2(18f, 34f)).ToPoint();
			if (CurrentlyHeldItem.IsAir)
				return 0;

			int cost = CurrentlyHeldItem.value * 4;
			ItemSlot.DrawMoney(spriteBatch, "Cost: ", costDrawPositionTopLeft.X, costDrawPositionTopLeft.Y, Utils.CoinsSplit(cost));

			return cost;
		}

		public static void DisplayEnchantmentDescription(SpriteBatch spriteBatch, Point descriptionDrawPositionTopLeft)
		{
			Vector2 vectorDrawPosition = descriptionDrawPositionTopLeft.ToVector2();
			Vector2 scale = new Vector2(0.55f, 0.6f);
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontMouseText, SelectedEnchantment.Value.Description, vectorDrawPosition, Color.Orange, 0f, Vector2.Zero, scale);
		}

		public static void InteractWithReforgeIcon(Enchantment? enchantmentToUse, int cost)
		{
			// If there's no valid item in the slot, do nothing.
			if (CurrentlyHeldItem.IsAir)
				return;

			// If there is no cost or the player cannot afford it, do nothing.
			if (cost <= 0L || !Main.LocalPlayer.CanBuyItem(cost))
				return;

			// If no enchantment has been selected, do nothing.
			if (!enchantmentToUse.HasValue)
				return;

			Item originalItem = CurrentlyHeldItem.Clone();
			byte oldPrefix = CurrentlyHeldItem.prefix;
			CurrentlyHeldItem.SetDefaults(CurrentlyHeldItem.type);
			CurrentlyHeldItem.Prefix(oldPrefix);
			CurrentlyHeldItem = CurrentlyHeldItem.CloneWithModdedDataFrom(originalItem);

			CurrentlyHeldItem.Calamity().AppliedEnchantment = enchantmentToUse.Value;
			enchantmentToUse.Value.CreationEffect?.Invoke(CurrentlyHeldItem);

			// Update the compare item. This is used check comparisons when showing reforge tooltip bonuses.
			// Updating it with the same bonuses as what was applied to the real item will negate the incorrect numbers,
			// such as absurd damage boosts.
			if (Main.cpItem is null)
				Main.cpItem = new Item();
			Main.cpItem.SetDefaults(Main.cpItem.type);
			Main.cpItem.Calamity().AppliedEnchantment = enchantmentToUse.Value;
			enchantmentToUse.Value.CreationEffect?.Invoke(Main.cpItem);

			// Take away the money for the cost.
			Main.LocalPlayer.BuyItem(cost);

			// Reset the enchantment variables.
			SelectedEnchantmentScaleFactor = 1f;
			SelectedEnchantment = null;

			Main.PlaySound(SoundID.DD2_BetsyFlameBreath, Main.LocalPlayer.Center);
		}
	}
}
