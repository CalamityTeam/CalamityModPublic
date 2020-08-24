using CalamityMod.CalPlayer;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace CalamityMod.UI
{
    public class CellFactoryUI
    {
        public const float IconScale = 0.7f;

        public static void Draw(SpriteBatch spriteBatch)
        {
            Player p = Main.LocalPlayer;
            CalamityPlayer mp = Main.LocalPlayer.Calamity();
            TEPowerCellFactory factory = mp.CurrentlyViewedFactory;
            
            // The UI only draws if the player is viewing a factory.
            if (factory is null)
                return;

            // If the player has a chest open, immediately destroy this UI.
            if (p.chest != -1)
            {
                mp.CurrentlyViewedFactory = null;
                return;
            }

            // What item is currently in the GUI slot? It's normally nothing, but it could be power cells.
            Item powercell = new Item();
            powercell.TurnToAir();
            if (factory.CellStack > 0)
            {
                powercell.SetDefaults(ModContent.ItemType<PowerCell>());
                powercell.stack = factory.CellStack;
            }

            Vector2 position = new Vector2(mp.CurrentlyViewedFactoryX, mp.CurrentlyViewedFactoryY);

            // Draw the factory's stored item as an inventory slot.
            DrawItemSlot(spriteBatch, ref powercell, position + new Vector2(16f, -34f) - Main.screenPosition);

            int width = 39, height = 39;

            Rectangle mouseRectangle = new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 2, 2);
            Rectangle drawnItemRectangle = new Rectangle((int)position.X, (int)position.Y - 60, width, height);

            if (mouseRectangle.Intersects(drawnItemRectangle) && powercell.stack > 0)
            {
                Main.HoverItem.SetDefaults(powercell.type);

                // If the slot is clicked, try to grab cells from the factory using both "current items" that a player can have.
                int cellsGrabbed = 0;
                if (Main.mouseLeft)
                {
                    cellsGrabbed = TryGrabCell(ref Main.mouseItem, ref powercell);
                    if (cellsGrabbed == 0)
                        cellsGrabbed = TryGrabCell(ref p.inventory[Main.LocalPlayer.selectedItem], ref powercell);
                }

                // If any cells were actually grabbed, take them from the factory's stockpile.
                if (cellsGrabbed > 0)
                    factory.CellStack -= (short)cellsGrabbed;

                // Since HoverItem is active, we don't need to input anything into this method.
                Main.instance.MouseTextHackZoom("");

                // Block mouse input if hovering over the item UI and not holding a pickaxe.
                // TODO -- why is this necessary?
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
