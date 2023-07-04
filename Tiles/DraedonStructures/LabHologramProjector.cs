using CalamityMod.CalPlayer;
using CalamityMod.Items.Placeables.DraedonStructures;
using CalamityMod.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Audio;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class LabHologramProjector : ModTile
    {
        public const int Width = 6;
        public const int Height = 7;
        public const int SheetSquare = 16;
        public const int IdleFrames = 8;
        public const int TalkingFrames = 8;
        public const int FrameCount = IdleFrames + TalkingFrames;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileWaterDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);

            // Manually reset the tile's coordinates to suit this tile, using the 6x3 style as a base.
            TileObjectData.newTile.Height = Height;
            TileObjectData.newTile.Origin = new Point16(3, 6);
            TileObjectData.newTile.CoordinateHeights = new int[TileObjectData.newTile.Height];
            TileObjectData.newTile.CoordinatePadding = 0;
            for (int i = 0; i < TileObjectData.newTile.CoordinateHeights.Length; i++)
            {
                TileObjectData.newTile.CoordinateHeights[i] = 16;
            }
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 6, 0);
            TileObjectData.newTile.LavaDeath = false;

            // Set the respective hologram tile entity as a secondary element to incorporate when placing this tile.
            ModTileEntity te = ModContent.GetInstance<TELabHologramProjector>();
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(te.Hook_AfterPlacement, -1, 0, true);

            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(99, 131, 199), CalamityUtils.GetItemName<LabHologramProjectorItem>());
            AnimationFrameHeight = 112;

            // Spawn electric sparks when this tile is hit with a pickaxe.
            DustType = 229;
            HitSound = SoundID.Tink;
        }

        public override bool CanExplode(int i, int j) => false;

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Tile tile = Main.tile[i, j];
            int left = i - tile.TileFrameX % (Width * SheetSquare) / SheetSquare;
            int top = j - tile.TileFrameY % (Height * SheetSquare) / SheetSquare;

            // Kill the hosted tile entity directly and immediately.
            TELabHologramProjector hologramTileEntity = CalamityUtils.FindTileEntity<TELabHologramProjector>(i, j, Width, Height, SheetSquare);
            hologramTileEntity?.Kill(left, top);
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            CalamityPlayer mp = player.Calamity();

            if (mp.CurrentlyViewedHologramID == -1)
            {
                TELabHologramProjector projector = CalamityUtils.FindTileEntity<TELabHologramProjector>(i, j, Width, Height, SheetSquare);
                mp.CurrentlyViewedHologramID = projector?.ID ?? -1;
                if (mp.CurrentlyViewedHologramID != -1)
                {
                    SoundEngine.PlaySound(SoundID.Chat);
                    player.SetTalkNPC(-1);
                }
            }
            else
                mp.CurrentlyViewedHologramID = -1;

            return true;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile trackTile = Main.tile[i, j];
            int time = (int)(Main.GlobalTimeWrappedHourly * 60);

            TELabHologramProjector hologramTileEntity = CalamityUtils.FindTileEntity<TELabHologramProjector>(i, j, Width, Height, SheetSquare);
            bool popup = false;
            if (hologramTileEntity != null && hologramTileEntity.PoppingUp)
                popup = true;

            // Frame cycling. Idly moves when the hologram is visible.
            int frame = time / 5 % FrameCount;
            if (popup)
            {
                if (frame <= IdleFrames)
                    frame += IdleFrames;
                if (frame >= FrameCount)
                    frame = IdleFrames + frame % TalkingFrames;
            }
            else
            {
                frame %= IdleFrames;
                if (frame >= IdleFrames - 2)
                    frame -= IdleFrames - 2;
            }

            int xPos = trackTile.TileFrameX;
            int yPos = trackTile.TileFrameY;
            yPos += frame % 16 * 112;

            Texture2D tileTexture = ModContent.Request<Texture2D>("CalamityMod/Tiles/DraedonStructures/LabHologramProjector").Value;
            Vector2 offset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + offset;
            Color drawColor = Lighting.GetColor(i, j);

            if (!trackTile.IsHalfBlock && trackTile.Slope == 0)
                spriteBatch.Draw(tileTexture, drawOffset, new Rectangle(xPos, yPos, 16, 16), drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            else if (trackTile.IsHalfBlock)
                spriteBatch.Draw(tileTexture, drawOffset + Vector2.UnitY * 8f, new Rectangle(xPos, yPos, 16, 16), drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            return false;
        }
    }
}
