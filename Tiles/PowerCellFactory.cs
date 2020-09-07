using CalamityMod.CalPlayer;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.Placeables;
using CalamityMod.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles
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

        public override void SetDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileWaterDeath[Type] = false;
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
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Power Cell Factory");
            AddMapEntry(new Color(67, 72, 81), name);
            animationFrameHeight = 68;
        }

        public override bool CanExplode(int i, int j) => false;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 226);
            return false;
        }

        public override bool HasSmartInteract() => true;

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            // Drop the factory itself.
            Item.NewItem(i * 16, j * 16, 32, 32, ModContent.ItemType<PowerCellFactoryItem>());

            Tile t = Main.tile[i, j];
            int left = i - t.frameX % (Width * SheetSquare) / SheetSquare;
            int top = j - t.frameY % (Height * SheetSquare) / SheetSquare;

            // Drop any cells contained in the factory.
            TEPowerCellFactory factory = CalamityUtils.FindTileEntity<TEPowerCellFactory>(i, j, Width, Height, 18);
            int numCells = factory?.CellStack ?? 0;
            if (numCells > 0)
                Item.NewItem(new Vector2(i, j) * 16f, ModContent.ItemType<PowerCell>(), numCells);

            factory?.Kill(left, top);
        }

        public override bool NewRightClick(int i, int j)
        {
            Tile t = Main.tile[i, j];
            TEPowerCellFactory thisFactory = CalamityUtils.FindTileEntity<TEPowerCellFactory>(i, j, Width, Height, SheetSquare);

            Player player = Main.LocalPlayer;
            player.CancelSignsAndChests();

            CalamityPlayer mp = player.Calamity();
            TEPowerCellFactory viewedFactory = mp.CurrentlyViewedFactory;

            // If this is the factory the player is currently looking at OR this factory doesn't really exist, close the GUI.
            if (viewedFactory != null && (thisFactory is null || thisFactory.ID == viewedFactory.ID))
            {
                mp.CurrentlyViewedFactory = null;
                mp.CurrentlyViewedFactoryX = mp.CurrentlyViewedFactoryY = -1;
                Main.PlaySound(SoundID.MenuClose);
            }

            // Otherwise, "switch to" this factory when it exists. This can be either opening the GUI from nothing, or just opening a different factory.
            else if (thisFactory != null)
            {
                // Play a sound depending on whether the player had another factory open previously.
                Main.PlaySound(mp.CurrentlyViewedFactory is null ? SoundID.MenuOpen : SoundID.MenuTick);

                // Ensure that the UI position is always centered and above the tile.
                int left = i - t.frameX % (Width * SheetSquare) / SheetSquare;
                int top = j - t.frameY % (Height * SheetSquare) / SheetSquare;
                mp.CurrentlyViewedFactoryX = left * 16 + 16;
                mp.CurrentlyViewedFactoryY = top * 16;
                mp.CurrentlyViewedFactory = thisFactory;

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
            int frameXPos = t.frameX;
            int frameYPos = t.frameY;

            // Grab the tile entity because its internal timer controls the animation.
            TEPowerCellFactory factory = CalamityUtils.FindTileEntity<TEPowerCellFactory>(i, j, Width, Height, SheetSquare);
            int frameIndex = factory?.AnimationFrame ?? TotalFrames - 1;
            frameXPos += frameIndex / FramesPerColumn * (Width * SheetSquare);
            frameYPos += frameIndex % FramesPerColumn * (Height * SheetSquare);

            Texture2D tex = ModContent.GetTexture("CalamityMod/Tiles/PowerCellFactory");
            Vector2 offset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + offset;
            Color drawColor = Lighting.GetColor(i, j);

            if (!t.halfBrick() && t.slope() == 0)
                spriteBatch.Draw(tex, drawOffset, new Rectangle(frameXPos, frameYPos, 16, 16), drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            else if (t.halfBrick())
                spriteBatch.Draw(tex, drawOffset + Vector2.UnitY * 8f, new Rectangle(frameXPos, frameYPos, 16, 16), drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            return false;
        }
    }
}
