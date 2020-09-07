using CalamityMod.CalPlayer;
using CalamityMod.Items;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.TileEntities;
using CalamityMod.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityMod.UI
{
    public class ChargingStationUI
    {
        public const float MaxPlayerDistance = 160f;
        private const float IconScale = 0.7f;
        private const int GuiWidth = 36;
        private const int GuiHeight = 36;
        private const int SlotSpacing = 8;
        private const float SlotDrawOffsetX = 24f;
        private const float CellDrawOffsetY = -20f;
        private const float PluggedDrawOffsetY = CellDrawOffsetY - GuiHeight - SlotSpacing;

        public static void Draw(SpriteBatch spriteBatch)
        {
            Player p = Main.LocalPlayer;
            CalamityPlayer mp = p.Calamity();
            TEChargingStation charger = mp.CurrentlyViewedCharger;

            // The UI only draws if the player is viewing a charger.
            if (charger is null)
                return;

            // If the player's inventory isn't open, or they have a chest open, immediately destroy this UI.
            if (!Main.playerInventory || p.chest != -1)
            {
                mp.CurrentlyViewedCharger = null;
                return;
            }

            // If the player is too far away from their viewed charger, immediately destroy this UI and play the menu close sound.
            Vector2 chargerWorldCenter = charger.Position.ToWorldCoordinates(8f * ChargingStation.Width, 8f * ChargingStation.Height);
            if (Vector2.DistanceSquared(p.Center, chargerWorldCenter) > MaxPlayerDistance * MaxPlayerDistance)
            {
                Main.PlaySound(SoundID.MenuClose);
                mp.CurrentlyViewedCharger = null;
                return;
            }

            // What items are currently in the UI item slots?
            // Normally they are empty, but the plugged item could be anything and the bottom slot may be a stack of power cells.
            int powercellID = ModContent.ItemType<PowerCell>();
            ref Item pluggedItem = ref charger.PluggedItem;
            Item powercell = new Item();
            powercell.TurnToAir();
            if (charger.CellStack > 0)
            {
                powercell.SetDefaults(powercellID);
                powercell.stack = charger.CellStack;
            }

            Vector2 uiBasePos = new Vector2(mp.CurrentlyViewedChargerX, mp.CurrentlyViewedChargerY);

            // Draw the charger's plugged item and the power cells (if any) as inventory slots.
            Vector2 pluggedDrawPos = uiBasePos + new Vector2(SlotDrawOffsetX, PluggedDrawOffsetY) - Main.screenPosition;
            DrawWeaponSlot(spriteBatch, pluggedItem, pluggedDrawPos);
            Vector2 powercellDrawPos = uiBasePos + new Vector2(SlotDrawOffsetX, CellDrawOffsetY) - Main.screenPosition;
            CalamityUtils.DrawPowercellSlot(spriteBatch, powercell, powercellDrawPos);

            Rectangle mouseRect = CalamityUtils.MouseHitbox;
            int slotRectX = (int)(uiBasePos.X - 1f);
            int pluggedSlotRectY = (int)(uiBasePos.Y + PluggedDrawOffsetY - GuiHeight / 2);
            Rectangle pluggedSlotRect = new Rectangle(slotRectX, pluggedSlotRectY, GuiWidth, GuiHeight);
            int cellSlotRectY = (int)(uiBasePos.Y + CellDrawOffsetY - GuiHeight / 2);
            Rectangle powercellSlotRect = new Rectangle(slotRectX, cellSlotRectY, GuiWidth, GuiHeight);

            // If the player's cursor is over the plugged slot, then interaction with that UI element is possible.
            ref Item playerHandItem = ref Main.mouseItem;
            if (mouseRect.Intersects(pluggedSlotRect))
            {
                if (!pluggedItem.IsAir)
                    Main.HoverItem = pluggedItem;

                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    bool syncRequired = false;

                    // If the player is holding shift and has space for the item, just spawn it on his or her face.
                    if (Main.keyState.PressingShift() && p.ItemSpace(pluggedItem))
                    {
                        // This does not use DropHelper because it has to clone an existing item, not create one from nothing.
                        p.QuickSpawnClonedItem(pluggedItem, pluggedItem.stack);

                        // Destroy the original plugged item because a clone of it was just spawned.
                        pluggedItem.TurnToAir();

                        // Immediately swap the now-empty slot with the player's held item as well, if said held item can be charged.
                        // Do not play a sound in this situation. The player is going to pick up the cloned item in a few frames, which will make sound.
                        if (!playerHandItem.IsAir && playerHandItem.Calamity().UsesCharge)
                            Utils.Swap(ref playerHandItem, ref pluggedItem);

                        syncRequired = true;
                    }

                    // Otherwise, if the slot is clicked, try to swap it with the player's held item. There are a few cases this succeeds:
                    // 1) The player's held item is air and the plugged item is NOT air.
                    // 2) The player's held item is chargeable.
                    // If neither case meets, then nothing happens 
                    else if((playerHandItem.IsAir && !pluggedItem.IsAir) || (!playerHandItem.IsAir && playerHandItem.Calamity().UsesCharge))
                    {
                        Utils.Swap(ref playerHandItem, ref pluggedItem);
                        Main.PlaySound(SoundID.Grab);
                        syncRequired = true;
                    }

                    // If either type of click action succeeded, the charger must now sync its item slot.
                    if (syncRequired)
                        charger.SendItemSyncPacket();
                }

                // Since HoverItem is active, we don't need to input anything into this method.
                Main.instance.MouseTextHackZoom("");
                // Specifically do not block mouse input if holding a pickaxe, so that you can mine blocks behind the UI.
                Main.blockMouse = Main.LocalPlayer.ActiveItem().pick <= 0;
            }

            // Otherwise if the player's cursor is over the power cell slot, they can interact with that UI element instead.
            else if (mouseRect.Intersects(powercellSlotRect))
            {
                if (!powercell.IsAir)
                    Main.HoverItem = powercell;

                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    short chargerStackDiff = 0;
                    bool shiftClicked = false;
                    
                    // If the player is holding shift and has space for the power cells, just spawn all of them on his or her face.
                    if (Main.keyState.PressingShift() && p.ItemSpace(powercell))
                    {
                        DropHelper.DropItem(p, powercellID, powercell.stack);
                        chargerStackDiff = (short)-powercell.stack;

                        // Do not play a sound in this situation. The player is going to pick up the dropped cells in a few frames, which will make sound.
                        shiftClicked = true;
                    }

                    // If the slot is normally clicked, behavior depends on whether the player is holding power cells.
                    else
                    {
                        bool holdingPowercell = playerHandItem.type == powercellID;

                        // If the player's held power cells can be stacked on top of what's already in the charger, then stack them.
                        if (holdingPowercell && powercell.stack < 999)
                        {
                            int spaceLeft = 999 - powercell.stack;

                            // If the player has more cells than there is space left, insert as many as possible. Otherwise insert all the cells.
                            int cellsToInsert = Math.Min(playerHandItem.stack, spaceLeft);
                            chargerStackDiff = (short)cellsToInsert;
                            playerHandItem.stack -= cellsToInsert;
                            if (playerHandItem.stack == 0)
                                playerHandItem.TurnToAir();
                        }
                        // If the player is holding nothing, then pick up all the power cells (if any exist).
                        else if (playerHandItem.IsAir && powercell.stack > 0)
                        {
                            chargerStackDiff = (short)-powercell.stack;
                            playerHandItem.SetDefaults(powercell.type);
                            playerHandItem.stack = powercell.stack;
                            powercell.TurnToAir();
                        }
                    }

                    // This assignment will automatically send the necessary sync packet.
                    if (chargerStackDiff != 0)
                    {
                        if (!shiftClicked)
                            Main.PlaySound(SoundID.Grab);
                        charger.CellStack += chargerStackDiff;
                    }
                }

                // Since HoverItem is active, we don't need to input anything into this method.
                Main.instance.MouseTextHackZoom("");
                // Specifically do not block mouse input if holding a pickaxe, so that you can mine blocks behind the UI.
                Main.blockMouse = Main.LocalPlayer.ActiveItem().pick <= 0;
            }
        }

        public static void DrawWeaponSlot(SpriteBatch spriteBatch, Item item, Vector2 drawPosition)
        {
            Texture2D slotBackgroundTex = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/ChargerWeaponSlot");
            spriteBatch.Draw(slotBackgroundTex, drawPosition, null, Color.White, 0f, slotBackgroundTex.Size() * 0.5f, IconScale, SpriteEffects.None, 0f);

            if (!item.IsAir)
            {
                float inventoryScale = Main.inventoryScale;
                Texture2D itemTexture = Main.itemTexture[item.type];
                Rectangle itemFrame = Main.itemAnimations[item.type] == null ? itemTexture.Frame(1, 1, 0, 0) : Main.itemAnimations[item.type].GetFrame(itemTexture);

                float baseScale = 1f;
                Color _ = Color.White;
                ItemSlot.GetItemLight(ref _, ref baseScale, item, false);
                float scaleRestrictor = 1f;
                if (itemFrame.Width > 46 || itemFrame.Height > 46)
                {
                    int restrictingDim = Math.Max(itemFrame.Width, itemFrame.Height);
                    scaleRestrictor = 46f / restrictingDim;
                }
                scaleRestrictor *= inventoryScale;

                if (ItemLoader.PreDrawInInventory(item, spriteBatch, drawPosition, itemFrame, item.GetAlpha(Color.White), item.GetColor(Color.White), itemTexture.Size() * 0.5f, scaleRestrictor * baseScale))
                {
                    spriteBatch.Draw(itemTexture, drawPosition, itemFrame, item.GetAlpha(Color.White), 0f, itemTexture.Size() * 0.5f, scaleRestrictor * baseScale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(itemTexture, drawPosition, itemFrame, item.GetColor(Color.White), 0f, itemTexture.Size() * 0.5f, scaleRestrictor * baseScale, SpriteEffects.None, 0f);
                }
            }
        }
    }
}
