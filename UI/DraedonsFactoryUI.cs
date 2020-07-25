using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace CalamityMod.UI
{
	public class DraedonsFactoryUI
    {
		public static void Draw(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer.Calamity().CurrentlyViewedFactory != null)
            {
                Item fuel = Main.LocalPlayer.Calamity().CurrentlyViewedFactory.HeldItem;
                Vector2 position = new Vector2(Main.LocalPlayer.Calamity().CurrentlyViewedFactoryX, Main.LocalPlayer.Calamity().CurrentlyViewedFactoryY);

                // Draw the fuel as an inventory slot.
                ItemSlot.Draw(spriteBatch, ref fuel, 0, position + new Vector2(0f, -54f) - Main.screenPosition);

                int width = 36, height = 40;
                Rectangle mouseRectangle = new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 2, 2);
                Rectangle drawnItemRectangle = new Rectangle((int)position.X, (int)position.Y - 60, width, height);
                if (mouseRectangle.Intersects(drawnItemRectangle) && fuel.stack > 0)
                {
                    Main.HoverItem = fuel.Clone();
                    // Take items from the chest if the item is clicked.
                    bool alreadyUsedItem = UseFuel(ref Main.mouseItem, ref fuel);
                    if (!alreadyUsedItem)
                        UseFuel(ref Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem], ref fuel);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        var netMessage = CalamityMod.Instance.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.DraedonGeneratorStackSync);
                        netMessage.Write(Main.LocalPlayer.Calamity().CurrentlyViewedFactory.ID);
                        netMessage.Write(fuel.stack);
                        netMessage.Send();
                    }
                    Main.blockMouse = Main.LocalPlayer.ActiveItem().pick <= 0; // Block mouse input if hovering over the item UI and not holding a pickaxe.
                }
            }
        }
        public static bool UseFuel(ref Item itemToUse, ref Item fuel)
        {
            if (Main.mouseLeft)
            {
                Main.playerInventory = true;
                Main.recBigList = false;
                if (itemToUse.type == fuel.type || itemToUse.type == ItemID.None)
                {
                    itemToUse = fuel.Clone();
                    int stack = itemToUse.stack;
                    itemToUse.SetDefaults(itemToUse.type);
                    itemToUse.stack = stack;
                    fuel.stack = 0;
                    return true;
                }
            }
            return false;
        }
    }
}
