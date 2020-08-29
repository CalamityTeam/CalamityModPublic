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
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 4;
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

        // Finds the Tile Entity associated with the Power Cell Factory tile that the player clicked on.
        // This can return null, so code using this function needs to be prepared for that.
        public TEPowerCellFactory FindTileEntity(int i, int j)
        {
            // Find the top left corner of the FrameImportant tile that the player clicked on in the world.
            int left = i - Main.tile[i, j].frameX % (Width * 18) / 18;
            int top = j - Main.tile[i, j].frameY % (Height * 18) / 18;

            byte factoryType = ModContent.GetInstance<TEPowerCellFactory>().type;
            bool exists = TileEntity.ByPosition.TryGetValue(new Point16(left, top), out TileEntity te);
            return exists && te.type == factoryType ? (TEPowerCellFactory)te : null;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 226);
            return false;
        }

        public override bool HasSmartInteract() => true;

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            // Drop the factory itself.
            Item.NewItem(i * 16, j * 16, 32, 32, ModContent.ItemType<PowerCellFactoryItem>());

            int left = i - Main.tile[i, j].frameX % (Width * 18) / 18;
            int top = j - Main.tile[i, j].frameY % (Height * 18) / 18;

            // Drop any cells contained in the factory.
            TEPowerCellFactory factory = FindTileEntity(i, j);
            int numCells = factory?.CellStack ?? 0;
            if (numCells > 0)
                Item.NewItem(new Vector2(i, j) * 16f, ModContent.ItemType<PowerCell>(), numCells);

            factory?.Kill(left, top);
        }

        public override bool NewRightClick(int i, int j)
        {
            int left = i - Main.tile[i, j].frameX % (Width * 18) / 18;
            int top = j - Main.tile[i, j].frameY % (Height * 18) / 18;
            TEPowerCellFactory thisFactory = FindTileEntity(i, j);

            Player player = Main.LocalPlayer;
            Main.mouseRightRelease = false;

            // If a sign or chest was in use previously, close those GUIs.
            if (player.sign >= 0)
            {
                Main.PlaySound(SoundID.MenuClose);
                player.sign = -1;
                Main.editSign = false;
                Main.npcChatText = "";
            }
            if (Main.editChest)
            {
                Main.PlaySound(SoundID.MenuTick);
                Main.editChest = false;
                Main.npcChatText = "";
            }

            CalamityPlayer mp = player.Calamity();
            TEPowerCellFactory viewedFactory = mp.CurrentlyViewedFactory;

            // If this is the factory the player is currently looking at OR this factory is doesn't really exist, close the GUI.
            if (viewedFactory != null && (thisFactory is null || thisFactory.ID == viewedFactory.ID))
            {
                mp.CurrentlyViewedFactory = null;
                mp.CurrentlyViewedFactoryX = mp.CurrentlyViewedFactoryY = -1;
                Main.PlaySound(SoundID.MenuClose);
            }

            // Otherwise, "switch to" this factory when it exists. This can be either opening the GUI from nothing, or just opening a different factory.
            else if(thisFactory != null)
            {
                // Play a sound depending on whether the player had another factory open previously.
                Main.PlaySound(mp.CurrentlyViewedFactory is null ? SoundID.MenuOpen : SoundID.MenuTick);

                // Ensure that the UI position is always centered and above the tile.
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
            Tile theTile = Main.tile[i, j];

            // These offsets start as the tile offsets, i.e. which sub-tile of the FrameImportant structure this specific location is.
            int frameXPos = Main.tile[i, j].frameX;
            int frameYPos = Main.tile[i, j].frameY;

            // Grab the tile entity because its internal timer controls the animation.
            TEPowerCellFactory factory = FindTileEntity(i, j);
            int frameIndex = factory?.AnimationFrame ?? TotalFrames - 1;
            frameXPos += frameIndex / FramesPerColumn * 72;
            frameYPos += frameIndex % FramesPerColumn * 72;

            Texture2D tex = ModContent.GetTexture("CalamityMod/Tiles/PowerCellFactory");
            Vector2 offset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + offset;
            Color drawColor = Lighting.GetColor(i, j);

            if (!theTile.halfBrick() && theTile.slope() == 0)
                spriteBatch.Draw(tex, drawOffset, new Rectangle(frameXPos, frameYPos, 16, 16), drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            else if (theTile.halfBrick())
                spriteBatch.Draw(tex, drawOffset + Vector2.UnitY * 8f, new Rectangle(frameXPos, frameYPos, 16, 16), drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            return false;
        }
    }
}
