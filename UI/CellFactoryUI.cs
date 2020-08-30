using CalamityMod.CalPlayer;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace CalamityMod.UI
{
    public class CellFactoryUI
    {
        public const float MaxPlayerDistance = 160f;
        public const float IconScale = 0.7f;
        private const int GuiWidth = 39;
        private const int GuiHeight = 39;

        public static void Draw(SpriteBatch spriteBatch)
        {
            Player p = Main.LocalPlayer;
            CalamityPlayer mp = Main.LocalPlayer.Calamity();
            TEPowerCellFactory factory = mp.CurrentlyViewedFactory;
            
            // The UI only draws if the player is viewing a factory.
            if (factory is null)
                return;

            // If the player's inventory isn't open, or they have a chest open, immediately destroy this UI.
            if (!Main.playerInventory || p.chest != -1)
            {
                mp.CurrentlyViewedFactory = null;
                return;
            }

            // If the player is too far away from their viewed factory, immediately destroy this UI and play the menu close sound.
            Vector2 factoryWorldCenter = factory.Position.ToWorldCoordinates(32f, 32f);
            if (Vector2.DistanceSquared(p.Center, factoryWorldCenter) > MaxPlayerDistance * MaxPlayerDistance)
            {
                Main.PlaySound(SoundID.MenuClose);
                mp.CurrentlyViewedFactory = null;
                return;
            }

            // What item is currently in the UI item slot? It's normally nothing, but it could be power cells.
            int powercellID = ModContent.ItemType<PowerCell>();
            Item powercell = new Item();
            powercell.TurnToAir();
            if (factory.CellStack > 0)
            {
                powercell.SetDefaults(powercellID);
                powercell.stack = factory.CellStack;
            }

            Vector2 position = new Vector2(mp.CurrentlyViewedFactoryX, mp.CurrentlyViewedFactoryY);

            // Draw the factory's stored item as an inventory slot.
            DrawItemSlot(spriteBatch, ref powercell, position + new Vector2(16f, -34f) - Main.screenPosition);

            int width = GuiWidth, height = GuiHeight;

            Rectangle mouseRectangle = new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 2, 2);
            Rectangle drawnItemRectangle = new Rectangle((int)position.X, (int)position.Y - 60, width, height);

            // If the player's cursor is over the slot and there are power cells, then interaction with the UI is possible.
            if (mouseRectangle.Intersects(drawnItemRectangle) && powercell.stack > 0)
            {
                Main.HoverItem.SetDefaults(powercell.type);

                // If the slot is clicked, try to grab cells from the factory using both "current items" that a player can have.
                int cellsGrabbed = 0;
                bool shiftClicked = false;
                if (Main.mouseLeft)
                {
                    // If the player is holding shift and has space for the power cells, just spawn all of them on his or her face.
                    if (Main.keyState.PressingShift() && p.ItemSpace(powercell))
                    {
                        cellsGrabbed = powercell.stack;
                        shiftClicked = true;
                        DropHelper.DropItem(p, powercellID, cellsGrabbed);
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
                        Main.PlaySound(SoundID.Grab);
                }

                // Since HoverItem is active, we don't need to input anything into this method.
                Main.instance.MouseTextHackZoom("");

                // Specifically do not block mouse input if holding a pickaxe, so that you can mine blocks behind the UI.
                Main.blockMouse = Main.LocalPlayer.ActiveItem().pick <= 0;
            }
        }

        public static void DrawItemSlot(SpriteBatch spriteBatch, ref Item item, Vector2 drawPosition)
        {
            Texture2D slotBackgroundTex = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/CellFactoryItemSlot_Empty");

            // This check is done twice because the draw order matters. We want to draw the background icon before any text.
            if (item.stack > 0)
                slotBackgroundTex = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/CellFactoryItemSlot_Filled");

            spriteBatch.Draw(slotBackgroundTex, drawPosition, null, Color.White, 0f, slotBackgroundTex.Size() * 0.5f, IconScale, SpriteEffects.None, 0f);
            if (item.stack > 0)
            {
                float inventoryScale = Main.inventoryScale;
                Vector2 numberOffset = slotBackgroundTex.Size() * 0.2f;
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch,
                    Main.fontItemStack,
                    item.stack.ToString(),
                    drawPosition + numberOffset * inventoryScale,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    new Vector2(inventoryScale),
                    -1f,
                    inventoryScale);
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

            // You can grab cells while holding cells, but your held stack can't go over 999.
            if (playerHandItem.type == cell.type)
            {
                int spaceLeft = 999 - playerHandItem.stack;

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
