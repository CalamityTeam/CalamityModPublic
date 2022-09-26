using CalamityMod.CalPlayer;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.UI
{
    public class PowerCellFactoryUI
    {
        public const float MaxPlayerDistance = 160f;
        private const int GuiWidth = 36;
        private const int GuiHeight = 36;
        private const float SlotDrawOffsetX = 32f;
        private const float SlotDrawOffsetY = -14f;

        public static void Draw(SpriteBatch spriteBatch)
        {
            Player p = Main.LocalPlayer;
            CalamityPlayer mp = p.Calamity();
            int factoryID = mp.CurrentlyViewedFactoryID;

            // The UI only draws if the player is viewing a factory.
            if (factoryID == -1)
                return;

            // Check if this tile entity ID is actually a factory. If it's not, immediately destroy this UI.
            TEPowerCellFactory factory;
            bool factoryIsValid = TileEntity.ByID.TryGetValue(factoryID, out TileEntity te);
            if (factoryIsValid && te is TEPowerCellFactory cast)
                factory = cast;
            else
            {
                mp.CurrentlyViewedFactoryID = -1;
                return;
            }

            // If the player's inventory isn't open, or they have a chest open, immediately destroy this UI.
            if (!Main.playerInventory || p.chest != -1)
            {
                mp.CurrentlyViewedFactoryID = -1;
                return;
            }

            // If the player is too far away from their viewed factory, immediately destroy this UI and play the menu close sound.
            Vector2 factoryWorldCenter = factory.Center;
            if (p.DistanceSQ(factoryWorldCenter) > MaxPlayerDistance * MaxPlayerDistance)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                mp.CurrentlyViewedFactoryID = -1;
                return;
            }

            // What item is currently in the UI item slot? It's normally nothing, but it could be power cells.
            int powercellID = ModContent.ItemType<DraedonPowerCell>();
            Item powercell = new Item();
            powercell.TurnToAir();
            if (factory.CellStack > 0)
            {
                powercell.SetDefaults(powercellID);
                powercell.stack = factory.CellStack;
            }

            Vector2 uiBasePos = factory.Position.ToWorldCoordinates(0f, 0f);

            // Draw the factory's stored item as an inventory slot.
            Vector2 powercellDrawPos = uiBasePos + new Vector2(SlotDrawOffsetX, SlotDrawOffsetY) - Main.screenPosition;
            CalamityUtils.DrawPowercellSlot(spriteBatch, powercell, powercellDrawPos);

            Rectangle mouseRect = CalamityUtils.MouseHitbox;
            int slotRectX = (int)(uiBasePos.X - 1f);
            int cellSlotRectY = (int)(uiBasePos.Y + SlotDrawOffsetY - GuiHeight / 2);
            Rectangle powercellSlotRect = new Rectangle(slotRectX, cellSlotRectY, GuiWidth, GuiHeight);

            // If the player's cursor is over the slot and there are power cells, then interaction with the UI is possible.
            if (mouseRect.Intersects(powercellSlotRect) && powercell.stack > 0)
            {
                p.mouseInterface = Main.blockMouse = true;

                if (!powercell.IsAir)
                    Main.HoverItem = powercell;

                // If the slot is clicked, try to grab cells from the factory using both "current items" that a player can have.
                int cellsGrabbed = 0;
                bool shiftClicked = false;
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    // If the player is holding shift and has space for the power cells, just spawn all of them on his or her face.
                    if (Main.keyState.PressingShift() && p.ItemSpace(powercell).CanTakeItemToPersonalInventory)
                    {
                        cellsGrabbed = powercell.stack;
                        shiftClicked = true;
                        p.QuickSpawnItem(p.GetSource_TileInteraction(te.Position.X, te.Position.Y), powercellID, cellsGrabbed);
                    }
                    else
                    {
                        cellsGrabbed = TryGrabCell(ref Main.mouseItem, ref powercell);
                        if (cellsGrabbed == 0)
                            cellsGrabbed = TryGrabCell(ref p.inventory[Main.LocalPlayer.selectedItem], ref powercell);
                    }
                }

                // If any cells were actually grabbed, take them from the factory's stockpile.
                // Using the CellStack property setter automatically sends the correct packet to update the entity server side.
                if (cellsGrabbed > 0)
                {
                    factory.CellStack -= (short)cellsGrabbed;

                    // Play a sound, but ONLY if the player didn't shift click.
                    // If they did, they're going to hear the item be picked up in a few frames anyway because it spawned on their face.
                    if (!shiftClicked)
                        SoundEngine.PlaySound(SoundID.Grab);
                }

                // Since HoverItem is active, we don't need to input anything into this method.
                Main.instance.MouseTextHackZoom("");
            }
        }

        // Returns the number of cells grabbed.
        public static int TryGrabCell(ref Item playerHandItem, ref Item cell)
        {
            Main.playerInventory = true;
            Main.recBigList = false;

            // You can only grab cells if you're stacking them with other cells or holding nothing.
            if (playerHandItem.IsAir)
            {
                playerHandItem.SetDefaults(cell.type);
                playerHandItem.stack = cell.stack;
                cell.TurnToAir();
                return playerHandItem.stack;
            }

            // You can grab cells while holding cells, but your held stack can't go over the stack limit.
            if (playerHandItem.type == cell.type)
            {
                int spaceLeft = playerHandItem.maxStack - playerHandItem.stack;

                // If there's more cells than space left, take as many as possible. Otherwise take all the cells.
                int cellsToTake = Math.Min(cell.stack, spaceLeft);
                if (cellsToTake <= 0)
                    return 0;

                playerHandItem.stack += cellsToTake;
                cell.stack -= cellsToTake;

                if (cell.stack == 0)
                    cell.TurnToAir();

                return cellsToTake;
            }
            return 0;
        }
    }
}
