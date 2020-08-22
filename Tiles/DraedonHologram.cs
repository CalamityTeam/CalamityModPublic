using CalamityMod.Items.Placeables.Furniture;
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
    public class DraedonHologram : ModTile
    {
        public const int Width = 6;
        public const int Height = 7;
        public const int IdleFrames = 8;
        public const int TalkingFrames = 8;
        public const int FrameCount = IdleFrames + TalkingFrames;
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
            TileObjectData.newTile.Height = 7;
            TileObjectData.newTile.Origin = new Point16(3, 6);
            TileObjectData.newTile.CoordinateHeights = new int[TileObjectData.newTile.Height];
            TileObjectData.newTile.CoordinatePadding = 0;
            for (int i = 0; i < TileObjectData.newTile.CoordinateHeights.Length; i++)
            {
                TileObjectData.newTile.CoordinateHeights[i] = 16;
            }
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 6, 0);
            //TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TEDraedonHologram>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
            animationFrameHeight = 112;

            soundType = SoundID.Tink;
            dustType = 229;
            minPick = 100;
            AddMapEntry(new Color(99, 131, 199));
        }

        public override bool CanExplode(int i, int j) => false;

        public TEDraedonHologram RetrieveTileEntity(int i, int j)
        {
            int left = i - Main.tile[i, j].frameX % (Width * 18) / 18;
            int top = j - Main.tile[i, j].frameY % (Height * 18) / 18;
            if (!TileEntity.ByPosition.ContainsKey(new Point16(left, top)))
            {
                TileEntity.ByPosition[new Point16(left, top)] = ModTileEntity.ConstructFromType(ModContent.TileEntityType<TEDraedonHologram>());
                TileEntity.ByPosition[new Point16(left, top)].Position = new Point16(left, top);
            }
            return (TEDraedonHologram)TileEntity.ByPosition[new Point16(left, top)];
        }


        public override bool NewRightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            if (player.Calamity().CurrentlyViewedHologramX == -1 ||
                player.Calamity().CurrentlyViewedHologramY == -1)
            {
                Main.PlaySound(SoundID.Chat, -1, -1, 1, 1f, 0f);
                player.Calamity().CurrentlyViewedHologramX = i;
                player.Calamity().CurrentlyViewedHologramY = j;
                player.talkNPC = -1;
            }
            else
            {
                player.Calamity().CurrentlyViewedHologramX = player.Calamity().CurrentlyViewedHologramY = -1;
            }
            return true;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int left = i - Main.tile[i, j].frameX % (Width * 18) / 18;
            int top = j - Main.tile[i, j].frameY % (Height * 18) / 18;
            Tile trackTile = Main.tile[i, j];
            int time = (int)(Main.GlobalTime * 60);
            TEDraedonHologram tileEntity = RetrieveTileEntity(i, j);
            tileEntity.CloseToPlayer = false;
            foreach (var player in Main.player)
            {
                if (!player.active)
                    continue;
                if (player.Distance(new Point(left, top).ToWorldCoordinates()) < 560f)
                {
                    tileEntity.CloseToPlayer = true;
                    break;
                }
            }
            bool closeToPlayer = tileEntity.CloseToPlayer;
            int frame = time / 5 % FrameCount;
            if (closeToPlayer)
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

            int xPos = Main.tile[i, j].frameX;
            int yPos = Main.tile[i, j].frameY;
            xPos += frame / 8 * 96;
            yPos += frame % 8 * 112;
            Texture2D glowmask = ModContent.GetTexture("CalamityMod/Tiles/DraedonHologram");
            Vector2 offset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + offset;
            Color drawColor = Lighting.GetColor(i, j);

            if (!trackTile.halfBrick() && trackTile.slope() == 0)
                spriteBatch.Draw(glowmask, drawOffset, new Rectangle(xPos, yPos, 16, 16), drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            else if (trackTile.halfBrick())
                spriteBatch.Draw(glowmask, drawOffset + Vector2.UnitY * 8f, new Rectangle(xPos, yPos, 16, 16), drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            return false;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 16, 32, ModContent.ItemType<HolographicDisplayBox>());
        }
    }
}
