using CalamityMod.Items;
using CalamityMod.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityMod.UI
{
	public class DraedonsItemChargerUI
    {
        public static int RectangleWidth => (int)(40 / Main.GameViewMatrix.Zoom.X);
        public static int RectangleHeight => (int)(40 / Main.GameViewMatrix.Zoom.Y);
        public static Vector2 ChargerPosition => new Vector2(Main.LocalPlayer.Calamity().CurrentlyViewedChargerX, Main.LocalPlayer.Calamity().CurrentlyViewedChargerY);
        public static Rectangle FuelIconBounds => new Rectangle((int)ChargerPosition.X - RectangleWidth / 2 + 8, (int)ChargerPosition.Y - 54, RectangleWidth, RectangleHeight);
        public static Rectangle ChargingItemIconBounds => new Rectangle((int)ChargerPosition.X - RectangleWidth / 2 + 8, (int)ChargerPosition.Y - 94, RectangleWidth, RectangleHeight);
        public static void Draw(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer.Calamity().CurrentlyViewedCharger != null)
            {
                ref TEDraedonItemCharger charger = ref Main.LocalPlayer.Calamity().CurrentlyViewedCharger;
                ref int depositWithdrawCooldown = ref charger.DepositWithdrawCooldown;
                ref Item itemBeingCharged = ref charger.ItemBeingCharged;
                ref Item fuel = ref charger.FuelItem;

                // Draw the fuel as an inventory slot.
                ItemSlot.Draw(spriteBatch, ref fuel, 0, ChargerPosition + new Vector2(8f, -54f) - Main.screenPosition);

                // Draw the item being charged as an inventory slot.
                ItemSlot.Draw(spriteBatch, ref itemBeingCharged, 0, ChargerPosition + new Vector2(8f, -94f) - Main.screenPosition);

                Rectangle mouseRectangle = new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 2, 2);
                bool cooldownComplete = depositWithdrawCooldown == 0;
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    cooldownComplete = Main.time % 5 == 0;
                }
                if (mouseRectangle.Intersects(FuelIconBounds) && fuel.stack > 0)
                    Main.HoverItem = fuel.Clone();
                if (mouseRectangle.Intersects(ChargingItemIconBounds) && itemBeingCharged.stack > 0)
                {
                    Main.HoverItem = itemBeingCharged.Clone();

                    // This effect is indeed purely aesthetic.
                    // Why not just use CurrentCharge instead of some wacky field in the tile entity, you might be asking?
                    // Well, as it turns out, multiplayer (and NBT tags) don't like taking data from the item. I was attempting to fix the issue for
                    // nearly 4 hours, to no avail. I'd consider this weird little piece of code here the lesser of two evils.
                    itemBeingCharged.Calamity().CurrentCharge = charger.Charge;
                }
                if (Main.mouseLeft && Main.LocalPlayer.itemAnimation == 0)
                {
                    if (mouseRectangle.Intersects(FuelIconBounds))
                    {
                        Main.playerInventory = true;
                        Main.recBigList = false;

                        if (cooldownComplete)
                        {
                            if (!UseFuel(ref Main.mouseItem, ref fuel))
                                UseFuel(ref Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem], ref fuel);
                        }
                        fuel.position = ChargerPosition + new Vector2(RectangleWidth, RectangleHeight) * 0.5f - Vector2.UnitY * 54f;

                        depositWithdrawCooldown = 9;
                    }
                    if (mouseRectangle.Intersects(ChargingItemIconBounds))
                    {
                        Main.playerInventory = true;
                        Main.recBigList = false;

                        if (cooldownComplete)
                        {
                            if (!UseChargeItem(ref Main.mouseItem, ref itemBeingCharged, ref charger))
                                UseChargeItem(ref Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem], ref itemBeingCharged, ref charger);
                        }
                        itemBeingCharged.position = ChargerPosition + new Vector2(RectangleWidth, RectangleHeight) * 0.5f - Vector2.UnitY * 94f;

                        depositWithdrawCooldown = 9;
                    }
                    charger.ClientToServerSync();
                }

                if (mouseRectangle.Intersects(FuelIconBounds) || mouseRectangle.Intersects(ChargingItemIconBounds))
                {
                    Main.instance.MouseTextHackZoom(""); // Since HoverItem is active, we don't need to input anything into this method.
                }
                Main.blockMouse = mouseRectangle.Intersects(FuelIconBounds) || mouseRectangle.Intersects(ChargingItemIconBounds); // Block mouse input if hovering over the item UI but not holding a pickaxe.
                Main.blockMouse &= Main.LocalPlayer.ActiveItem().pick <= 0;

                charger.DrawAllSparks(spriteBatch);
            }
        }
        public static bool UseFuel(ref Item itemToUse, ref Item fuel)
        {
            // Withdraw the stored fuel.
            if ((itemToUse.type == ModContent.ItemType<PowerCell>() || itemToUse.type == ItemID.None) && fuel.stack > 0)
            {
                int oldMouseStack = itemToUse.stack;
                itemToUse = fuel.Clone();
                itemToUse.SetDefaults(itemToUse.type);
                itemToUse.stack = fuel.stack + oldMouseStack;
                fuel.stack = 0;
                return true;
            }
            // Deposit fuel.
            else if (itemToUse.type == ModContent.ItemType<PowerCell>() && itemToUse.stack > 0 && fuel.stack == 0)
            {
                int oldStack = itemToUse.stack;
                fuel = itemToUse.Clone();
                fuel.SetDefaults(fuel.type);
                fuel.stack = oldStack;
                itemToUse.stack = 0;
                return true;
            }
            return false;
        }
        public static bool UseChargeItem(ref Item itemToUse, ref Item chargeItem, ref TEDraedonItemCharger charger)
        {
            // Withdraw the placed item.
            if (itemToUse.type == ItemID.None && chargeItem.stack > 0)
            {
                int oldCharge = charger.Charge;
                itemToUse = chargeItem.Clone();
                itemToUse.SetDefaults(itemToUse.type);
                itemToUse.Calamity().CurrentCharge = oldCharge;
                chargeItem.stack = 0;
                return false;
            }
            // Insert an item.
            else if (itemToUse.stack == 1 && chargeItem.stack == 0)
            {
                if (itemToUse is null || itemToUse.type < ItemID.Count)
                    return false;
                if (!itemToUse.Calamity().Chargeable)
                    return false;
                charger.Charge = itemToUse.Calamity().CurrentCharge;
                chargeItem = itemToUse.Clone();
                chargeItem.SetDefaults(chargeItem.type);
                itemToUse = new Item();
                return true;
            }
            return false;
        }
    }
}
