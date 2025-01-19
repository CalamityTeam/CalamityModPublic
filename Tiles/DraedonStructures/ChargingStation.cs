using CalamityMod.CalPlayer;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.Placeables.DraedonStructures;
using CalamityMod.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class ChargingStation : ModTile
    {
        // The origin offsets are not needed in this file but may be needed by other code.
        public const int Width = 3;
        public const int Height = 2;
        public const int OriginOffsetX = 1;
        public const int OriginOffsetY = 1;
        public const int SheetSquare = 18;

        // One cell of charge is placed into the weapon on a timer of this many frames.
        public const int FramesPerChargeAction = 8;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileWaterDeath[Type] = false;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            // No need to set width, height, origin, etc. here, Style3x2 is exactly what we want.
            TileObjectData.newTile.LavaDeath = false;

            // When this tile is placed, it places the charging station tile entity.
            ModTileEntity te = ModContent.GetInstance<TEChargingStation>();
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(te.Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);
            AddMapEntry(new Color(67, 72, 81), CalamityUtils.GetItemName<ChargingStationItem>());
        }

        public override bool CanExplode(int i, int j) => false;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.Electric);
            return false;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Tile t = Main.tile[i, j];
            int left = i - t.TileFrameX % (Width * SheetSquare) / SheetSquare;
            int top = j - t.TileFrameY % (Height * SheetSquare) / SheetSquare;

            // Drop any cells contained in the charging station, as well as its plugged item.
            Vector2 dropPos = new Vector2(i, j) * 16f;
            TEChargingStation charger = CalamityUtils.FindTileEntity<TEChargingStation>(i, j, Width, Height, SheetSquare);
            int numCells = charger?.CellStack ?? 0;
            if (numCells > 0)
                Item.NewItem(new EntitySource_TileBreak(i, j), dropPos, ModContent.ItemType<DraedonPowerCell>(), numCells);

            // Netcode check is required because otherwise this will spawn two items.
            // Force cloning items into the Main item array is weird.
            Item pluggedItem = charger?.PluggedItem ?? null;
            if (pluggedItem != null && !pluggedItem.IsAir && Main.netMode != NetmodeID.MultiplayerClient)
                DropHelper.DropItemClone(new EntitySource_TileBreak(i, j), pluggedItem, dropPos, pluggedItem.stack);

            charger?.Kill(left, top);
        }

        public override bool RightClick(int i, int j)
        {
            TEChargingStation thisCharger = CalamityUtils.FindTileEntity<TEChargingStation>(i, j, Width, Height, SheetSquare);
            Player player = Main.LocalPlayer;
            player.CancelSignsAndChests();
            CalamityPlayer mp = player.Calamity();

            // If this is the charger the player is currently looking at OR this charger doesn't really exist, close the GUI.
            if (thisCharger is null || thisCharger.ID == mp.CurrentlyViewedChargerID)
            {
                mp.CurrentlyViewedChargerID = -1;
                SoundEngine.PlaySound(SoundID.MenuClose);
            }

            // Otherwise, "switch to" this charger when it exists. This can be either opening the GUI from nothing, or just opening a different charger.
            else if (thisCharger != null)
            {
                // Play a sound depending on whether the player had another charger open previously.
                SoundEngine.PlaySound(mp.CurrentlyViewedChargerID == -1 ? SoundID.MenuOpen : SoundID.MenuTick);
                mp.CurrentlyViewedChargerID = thisCharger.ID;
                Main.playerInventory = true;
                Main.recBigList = false;
            }

            Recipe.FindRecipes();
            return true;
        }

        // Draws the charger's glowing light on top of it.
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile t = Main.tile[i, j];
            int xFrame = t.TileFrameX;
            int yFrame = t.TileFrameY;

            // Grab the tile entity because its glowmask depends on whether it's currently charging.
            TEChargingStation charger = CalamityUtils.FindTileEntity<TEChargingStation>(i, j, Width, Height, SheetSquare);

            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Tiles/DraedonStructures/ChargingStation_Glow").Value;
            Vector2 screenOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + screenOffset;
            Color drawColor = charger?.LightColor ?? Color.Red;

            if (!t.IsHalfBlock && t.Slope == 0)
                Main.spriteBatch.Draw(glowmask, drawOffset, new Rectangle?(new Rectangle(xFrame, yFrame, 18, 18)), drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            else if (t.IsHalfBlock)
                Main.spriteBatch.Draw(glowmask, drawOffset + new Vector2(0f, 8f), new Rectangle?(new Rectangle(xFrame, yFrame, 18, 8)), drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
        }
    }
}
