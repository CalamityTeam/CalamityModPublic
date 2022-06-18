using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.Audio;
using Terraria.GameContent;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod
{
    public static partial class CalamityUtils
    {
        /// <summary>
        /// A hitbox that should approximately represent the tip of the mouse cursor. Used for UI stuff.
        /// </summary>
        public static Rectangle MouseHitbox => new((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 2, 2);

        /// <summary>
        /// Finds the Calamity tile entity that is associated with the tile at the given world coordinates (i, j).<br></br>
        /// Can return <b>null</b> if nothing is found.
        /// </summary>
        /// <typeparam name="T">The Type of Tile Entity to look for. Must be a modded tile entity.</typeparam>
        /// <param name="i">The X coordinate of the tile in the world.</param>
        /// <param name="j">The Y coordinate of the tile in the world.</param>
        /// <param name="width">The width of the <b>FrameImportant</b> Tile that hosts the tile entity.</param>
        /// <param name="height">The height of the <b>FrameImportant</b> Tile that hosts the tile entity.</param>
        /// <param name="sheetSquare">The width in pixels of each tile-square on the Tile's sprite sheet. Usually 16. Can be 18 if the sprite sheet has pink lines.</param>
        /// <returns>A <b>ModTileEntity</b> of the proper type, or <b>null</b>.</returns>
        public static T FindTileEntity<T>(int i, int j, int width, int height, int sheetSquare = 16) where T : ModTileEntity
        {
            // Find the top left corner of the FrameImportant tile that the player clicked on in the world.
            Tile t = Main.tile[i, j];
            int left = i - t.TileFrameX % (width * sheetSquare) / sheetSquare;
            int top = j - t.TileFrameY % (height * sheetSquare) / sheetSquare;

            int chargerType = GetInstance<T>().Type;
            bool exists = TileEntity.ByPosition.TryGetValue(new Point16(left, top), out TileEntity te);
            return exists && te.type == chargerType ? (T)te : null;
        }

        /// <summary>
        /// Cancels any currently active sign or chest name edits, and closes any chest the player may have open.
        /// </summary>
        /// <param name="player">The player to act upon. Should basically always be Main.LocalPlayer.</param>
        public static void CancelSignsAndChests(this Player player)
        {
            Main.mouseRightRelease = false;

            // If a sign or chest was in use previously, close those GUIs.
            if (player.sign >= 0)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                player.sign = -1;
                Main.editSign = false;
                Main.npcChatText = "";
            }
            if (Main.editChest)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                Main.editChest = false;
                Main.npcChatText = "";
            }
            if (player.chest >= 0)
                player.chest = -1;
        }

        // Draws the Power Cell item slot in a UI. Used by both the Power Cell Factory and Charging Station.
        internal static void DrawPowercellSlot(SpriteBatch spriteBatch, Item item, Vector2 drawPosition, float iconScale = 0.7f)
        {
            Texture2D slotBackgroundTex = Request<Texture2D>("CalamityMod/ExtraTextures/UI/PowerCellSlot_Empty").Value;

            // This check is done twice because the draw order matters. We want to draw the background icon before any text.
            if (item.stack > 0)
                slotBackgroundTex = Request<Texture2D>("CalamityMod/ExtraTextures/UI/PowerCellSlot_Filled").Value;

            spriteBatch.Draw(slotBackgroundTex, drawPosition, null, Color.White, 0f, slotBackgroundTex.Size() * 0.5f, iconScale, SpriteEffects.None, 0f);
            if (item.stack > 0)
            {
                float inventoryScale = Main.inventoryScale * iconScale;
                Vector2 numberOffset = slotBackgroundTex.Size() * 0.2f;
                numberOffset.X -= 17f;
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch,
                    FontAssets.ItemStack.Value,
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
    }
}
