using CalamityMod.CalPlayer;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.Placeables.DraedonStructures;
using CalamityMod.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class PowerCellFactory : ModTile
    {
        public const int Width = 4;
        public const int Height = 4;
        public const int OriginOffsetX = 1;
        public const int OriginOffsetY = 3;
        public const int SheetSquare = 18;

        // The number of animation frames is 45. Cells are created on animation frame 42.
        public const int TotalFrames = 45;
        private const int FramesPerColumn = 15;
        public const int AnimationFramerate = 5;

        // 45 * 5 + 675 = 900 = 15 seconds per complete cell cycle.
        // There are this many frames of downtime between cell creations.
        public const int BetweenCellDowntime = 675;
        // The cell is created on this animation frame.
        public const int CellCreateFrame = 42;
        // With a delay of this many extra frames after that animation frame starts.
        public const int MagicFrameDelay = AnimationFramerate - 1;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileWaterDeath[Type] = false;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Width = Width;
            TileObjectData.newTile.Height = Height;
            TileObjectData.newTile.Origin = new Point16(OriginOffsetX, OriginOffsetY);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
            TileObjectData.newTile.LavaDeath = false;

            // When this tile is placed, it places the power cell factory tile entity.
            ModTileEntity te = ModContent.GetInstance<TEPowerCellFactory>();
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(te.Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);
            AddMapEntry(new Color(67, 72, 81), CalamityUtils.GetItemName<PowerCellFactoryItem>());
            AnimationFrameHeight = 68;
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

            // Drop any cells contained in the factory.
            TEPowerCellFactory factory = CalamityUtils.FindTileEntity<TEPowerCellFactory>(i, j, Width, Height, SheetSquare);
            int numCells = factory?.CellStack ?? 0;
            if (numCells > 0)
                Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16f, ModContent.ItemType<DraedonPowerCell>(), numCells);

            factory?.Kill(left, top);
        }

        public override bool RightClick(int i, int j)
        {
            TEPowerCellFactory thisFactory = CalamityUtils.FindTileEntity<TEPowerCellFactory>(i, j, Width, Height, SheetSquare);
            Player player = Main.LocalPlayer;
            player.CancelSignsAndChests();
            CalamityPlayer mp = player.Calamity();

            // If this is the factory the player is currently looking at OR this factory doesn't really exist, close the GUI.
            if (thisFactory is null || thisFactory.ID == mp.CurrentlyViewedFactoryID)
            {
                mp.CurrentlyViewedFactoryID = -1;
                SoundEngine.PlaySound(SoundID.MenuClose);
            }

            // Otherwise, "switch to" this factory when it exists. This can be either opening the GUI from nothing, or just opening a different factory.
            else if (thisFactory != null)
            {
                // Play a sound depending on whether the player had another factory open previously.
                SoundEngine.PlaySound(mp.CurrentlyViewedFactoryID == -1 ? SoundID.MenuOpen : SoundID.MenuTick);
                mp.CurrentlyViewedFactoryID = thisFactory.ID;
                Main.playerInventory = true;
                Main.recBigList = false;
            }

            Recipe.FindRecipes();
            return true;
        }

        // All tile drawcode is done manually because the tile's animation is controlled by a tile entity.
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            // These offsets start as the tile offsets, i.e. which sub-tile of the FrameImportant structure this specific location is.
            Tile t = Main.tile[i, j];
            int frameXPos = t.TileFrameX;
            int frameYPos = t.TileFrameY;

            // Grab the tile entity because its internal timer controls the animation.
            TEPowerCellFactory factory = CalamityUtils.FindTileEntity<TEPowerCellFactory>(i, j, Width, Height, SheetSquare);
            int frameIndex = factory?.AnimationFrame ?? TotalFrames - 1;
            frameXPos += frameIndex / FramesPerColumn * (Width * SheetSquare);
            frameYPos += frameIndex % FramesPerColumn * (Height * SheetSquare);

            Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Tiles/DraedonStructures/PowerCellFactory").Value;
            Vector2 offset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + offset;
            Color drawColor = Lighting.GetColor(i, j);
            

            if (!t.IsHalfBlock && t.Slope == 0)
                spriteBatch.Draw(tex, drawOffset, new Rectangle(frameXPos, frameYPos, 16, 16), drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            else if (t.IsHalfBlock)
                spriteBatch.Draw(tex, drawOffset + Vector2.UnitY * 8f, new Rectangle(frameXPos, frameYPos, 16, 16), drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);

            // Draws the Smart Cursor Highlight. Not 100% accurate, but its close enough
            bool actuallySelected;
            Color highlightColor;
            Texture2D texhighlight = ModContent.Request<Texture2D>(HighlightTexture).Value;
            if (Main.InSmartCursorHighlightArea(i, j, out actuallySelected))
            {   
                if (actuallySelected)
                {
                    highlightColor = new Color(252, 252, 84);
                }
                else
                {
                    highlightColor = new Color(125, 125, 125);
                }
                spriteBatch.Draw(texhighlight, drawOffset, new Rectangle(frameXPos, frameYPos, 16, 16), highlightColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
            
           

            return false;
        }
    }
}
