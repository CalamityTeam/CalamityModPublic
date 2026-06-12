using CalamityMod.TileEntities;
using CalamityMod.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Furniture.Paintings
{
    public abstract class BaseCanvasPainting : ModTile
    {
        // TODO: make this not a hardcoded number with seemingly no meaning
        public virtual float Scale => 0.4f;

        public static Asset<Texture2D> border;
        public static Asset<Texture2D> corner;

        public override void Load()
        {
            border = ModContent.Request<Texture2D>("CalamityMod/Tiles/Furniture/Paintings/CalamityCanvasBorder");
            corner = ModContent.Request<Texture2D>("CalamityMod/Tiles/Furniture/Paintings/CalamityCanvasCorner");
        }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileWaterDeath[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.Width = 5;
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.CoordinateHeights = new int[] { 18, 18, 18, 18, 18 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TECanvasPainting>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.addTile(Type);

            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            AddMapEntry(new Color(99, 50, 30), Language.GetText("MapObject.Painting"));
        }
        public override bool HasSmartInteract(int i, int j, Terraria.GameContent.ObjectInteractions.SmartInteractScanSettings settings) => true;

        public override bool RightClick(int i, int j)
        {
            Main.LocalPlayer.CancelSignsAndChests();
            var cube = CalamityUtils.FindTileEntity<TECanvasPainting>(i, j, 5, 5);
            if (cube != null)
            {
                CanvasPaintingUIState.ResetVars();
                Main.LocalPlayer.Calamity().CurrentlyViewedCanvasID = cube.ID;
                Main.LocalPlayer.Calamity().CurrentlyViewedCanvasType = Type;
                SoundEngine.PlaySound(SoundID.MenuOpen);
                Main.playerInventory = true;
                Main.recBigList = false;
            }
            Recipe.FindRecipes();
            return false;
        }

        public override void MouseOver(int i, int j)
        {
            // The sprites for all Canvas Paintings are the same
            Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<Items.Placeables.Furniture.Paintings.CalamityCanvas2023>();
            Main.LocalPlayer.noThrow = 2;
            Main.LocalPlayer.cursorItemIconEnabled = true;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            var t = Main.tile[i, j];
            var left = i - t.TileFrameX % (5 * 18) / 18;
            var top = j - t.TileFrameY % (5 * 18) / 18;

            var canvas = CalamityUtils.FindTileEntity<TECanvasPainting>(i, j, 5, 5, 18);

            canvas?.Kill(left, top);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            var t = Main.tile[i, j];
            var texture = TextureAssets.Tile[Type].Value;
            var cube = CalamityUtils.FindTileEntity<TECanvasPainting>(i, j, 1, 1);

            var pos = new Vector2(i * 16, j * 16) + CalamityUtils.TileDrawOffset;
            if (cube != null && t.TileFrameX == 0)
            {
                var fPX = (int)cube.framePosition.X;
                var fPY = (int)cube.framePosition.Y;
                var scale = (int)(texture.Width * 0.1f * cube.scale);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null);
                spriteBatch.Draw(texture, pos - Main.screenPosition, new Rectangle(fPX, fPY, scale, scale), Lighting.GetColor(i, j), 0, new Vector2(0, 0), 1 / cube.scale * Scale, 0, 0);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, null);
            }
            // Calculate and draw the borders
            if (t.TileFrameX == 0 && t.TileFrameY == 0)
                DrawBorders(spriteBatch, pos - Main.screenPosition, new Point(i, j));
            return false;
        }
        public void DrawBorders(SpriteBatch spriteBatch, Vector2 pos, Point cords)
        {
            var texture = border.Value;
            var cornerTex = corner.Value;
            var canvasID = Type;
            var commonDim = 8;
            var finalCord = 72;
            var size = 80;
            var light = Lighting.GetColor(cords.X, cords.Y);

            // Check for nearby Canvas paintings
            // If they are cleanly lined up with this Canvas painting, don't draw borders between them
            var drawTop = true;
            var drawLeft = true;
            var drawRight = true;
            var drawBottom = true;
            // Corners
            var drawTopLeft = false;
            var drawTopRight = false;
            var drawBottomLeft = false;
            var drawBottomRight = false;

            // The bottom right tile of the canvas painting
            var bottomRight = new Point(cords.X + 4, cords.Y + 4);

            // Adjacent tiles that may or may not have other canvas paintings
            var left = CalamityUtils.ParanoidTileRetrieval(cords.X - 1, cords.Y);
            var top = CalamityUtils.ParanoidTileRetrieval(cords.X, cords.Y - 1);
            var right = CalamityUtils.ParanoidTileRetrieval(bottomRight.X + 1, bottomRight.Y);
            var bottom = CalamityUtils.ParanoidTileRetrieval(bottomRight.X, bottomRight.Y + 1);

            // Check if adjacent tiles are canvas paintings that are lined up with this one
            var validTop = ValidCanvasFrame(top, 0, size, canvasID);
            var validRight = ValidCanvasFrame(right, 0, size, canvasID);
            var validLeft = ValidCanvasFrame(left, finalCord, 0, canvasID);
            var validBottom = ValidCanvasFrame(bottom, finalCord, 0, canvasID);

            if (validTop)
            {
                drawTop = false;
            }
            if (validBottom)
            {
                drawBottom = false;
            }
            if (validRight)
            {
                drawRight = false;
                // Check for junction corners
                var topright = CalamityUtils.ParanoidTileRetrieval(bottomRight.X + 1, cords.Y - 1);
                var bottomright = CalamityUtils.ParanoidTileRetrieval(bottomRight.X + 1, bottomRight.Y + 1);
                if (!drawTop && !ValidCanvasFrame(topright, 0, size, canvasID))
                {
                    drawTopRight = true;
                }
                if (!drawBottom && !ValidCanvasFrame(bottomright, 0, 0, canvasID))
                {
                    drawBottomRight = true;
                }
            }
            if (validLeft)
            {
                drawLeft = false;
                // Check for junction corners
                var topleft = CalamityUtils.ParanoidTileRetrieval(cords.X - 1, cords.Y - 1);
                var bottomleft = CalamityUtils.ParanoidTileRetrieval(cords.X - 1, bottomRight.Y + 1);
                if (!drawTop && !ValidCanvasFrame(topleft, finalCord, size, canvasID))
                {
                    drawTopLeft = true;
                }
                if (!drawBottom && !ValidCanvasFrame(bottomleft, finalCord, 0, canvasID))
                {
                    drawBottomLeft = true;
                }
            }

            // Draw the sides
            if (drawBottom)
                spriteBatch.Draw(texture, pos + Vector2.UnitY * finalCord, new Rectangle(0, texture.Height - commonDim, texture.Width, commonDim), light, 0, new Vector2(0, 0), 1, 0, 0);
            if (drawTop)
                spriteBatch.Draw(texture, pos, new Rectangle(0, 0, texture.Width, commonDim), light, 0, new Vector2(0, 0), 1, 0, 0);
            if (drawRight)
                spriteBatch.Draw(texture, pos + Vector2.UnitX * finalCord, new Rectangle(texture.Width - commonDim, commonDim, commonDim, texture.Height - 2 * commonDim), light, 0, new Vector2(0, 0), 1, 0, 0);
            if (drawLeft)
                spriteBatch.Draw(texture, pos, new Rectangle(0, commonDim, commonDim, texture.Height - 2 * commonDim), light, 0, new Vector2(0, 0), 1, 0, 0);

            // Draw the corners 
            // All corner drawing is done with a single corner sprite that is rotated and flipped about depending on the situation
            if (drawTop && drawLeft)
                spriteBatch.Draw(cornerTex, pos, null, light, 0, new Vector2(0, 0), 1, 0, 0);
            if (drawTop && drawRight)
                spriteBatch.Draw(cornerTex, pos + Vector2.UnitX * finalCord, null, light, 0, new Vector2(0, 0), 1, SpriteEffects.FlipHorizontally, 0);
            if (drawBottom && drawLeft)
                spriteBatch.Draw(cornerTex, pos + Vector2.UnitY * finalCord, null, light, 0, new Vector2(0, 0), 1, SpriteEffects.FlipVertically, 0);
            if (drawBottom && drawRight)
                spriteBatch.Draw(cornerTex, pos + Vector2.One * size, null, light, MathHelper.Pi, new Vector2(0, 0), 1, 0, 0);

            // Draw junction corners
            if (drawTopLeft)
                spriteBatch.Draw(cornerTex, pos + corner.Size(), null, light, MathHelper.Pi, new Vector2(0, 0), 1, 0, 0);
            if (drawTopRight)
                spriteBatch.Draw(cornerTex, pos + Vector2.UnitX * finalCord, null, light, 0, new Vector2(0, 0), 1, SpriteEffects.FlipVertically, 0);
            if (drawBottomLeft)
                spriteBatch.Draw(cornerTex, pos + Vector2.UnitY * finalCord, null, light, 0, new Vector2(0, 0), 1, SpriteEffects.FlipHorizontally, 0);
            if (drawBottomRight)
                spriteBatch.Draw(cornerTex, pos - corner.Size() + Vector2.One * size, null, light, 0, new Vector2(0, 0), 1, 0, 0);
        }

        // Check if the tile is a canvas tile that has the correct tile frames
        public static bool ValidCanvasFrame(Tile t, int frameX, int frameY, ushort canvasID)
        {
            return t.HasTile && t.TileType == canvasID && t.TileFrameX == frameX && t.TileFrameY == frameY;
        }
    }
}
