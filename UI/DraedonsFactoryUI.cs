using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace CalamityMod.UI
{
	public class DraedonsFactoryUI
    {
        public const float IconScale = 0.7f;
		public static void Draw(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer.Calamity().CurrentlyViewedFactory != null)
            {
                var time = Terraria.DataStructures.TileEntity.ByID;
                var stack = Main.LocalPlayer.Calamity().CurrentlyViewedFactory;
                if (Main.LocalPlayer.chest != -1)
                {
                    Main.LocalPlayer.Calamity().CurrentlyViewedFactory = null;
                    return;
                }
                Item fuel = Main.LocalPlayer.Calamity().CurrentlyViewedFactory.HeldItem;
                Vector2 position = new Vector2(Main.LocalPlayer.Calamity().CurrentlyViewedFactoryX, Main.LocalPlayer.Calamity().CurrentlyViewedFactoryY);

                // Draw the fuel as an inventory slot.
                DrawItemSlot(spriteBatch, ref fuel, position + new Vector2(16f, -34f) - Main.screenPosition);

                int width = 39, height = 39;
                Rectangle mouseRectangle = new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 2, 2);
                Rectangle drawnItemRectangle = new Rectangle((int)position.X, (int)position.Y - 60, width, height);
                if (mouseRectangle.Intersects(drawnItemRectangle) && fuel.stack > 0)
                {
                    Main.HoverItem.SetDefaults(fuel.type);
                    // Take items from the chest if the item is clicked.
                    bool alreadyUsedItem = UseFuel(ref Main.mouseItem, ref fuel);
                    if (!alreadyUsedItem)
                        UseFuel(ref Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem], ref fuel);
                    Main.instance.MouseTextHackZoom(""); // Since HoverItem is active, we don't need to input anything into this method.
                    Main.blockMouse = Main.LocalPlayer.ActiveItem().pick <= 0; // Block mouse input if hovering over the item UI and not holding a pickaxe.
                }
            }
        }

        public static void DrawItemSlot(SpriteBatch spriteBatch, ref Item item, Vector2 drawPosition)
		{
            Texture2D iconTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/DraedonFuelCellUISlot");

            // This check is done twice because the draw order matters. We want to draw the background icon before any text.
            if (item.stack > 0)
                iconTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/DraedonFuelCellUISlotFilled");
            spriteBatch.Draw(iconTexture, drawPosition, null, Color.White, 0f, iconTexture.Size() * 0.5f, IconScale, SpriteEffects.None, 0f);
            if (item.stack > 0)
            {
                float inventoryScale = Main.inventoryScale;
                Vector2 numberOffset = iconTexture.Size() * 0.2f;
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
