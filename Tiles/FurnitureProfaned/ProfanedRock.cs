
using CalamityMod.Dusts.Furniture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureProfaned
{
    public class ProfanedRock : ModTile
    {
        internal static FramedGlowMask GlowMask;

        public override void SetStaticDefaults()
        {
            GlowMask = new("CalamityMod/Tiles/FurnitureProfaned/ProfanedRockGlow", 18, 18);

            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeDecorativeTiles(Type);
            CalamityUtils.MergeSmoothTiles(Type);

            HitSound = SoundID.Tink;
            MineResist = 4f;
            MinPick = 225;
            AddMapEntry(new Color(84, 38, 33));
        }
        int animationFrameWidth = 288;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.GoldCoin, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, ModContent.DustType<ProfanedTileRock>(), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            int uniqueAnimationFrameX = 0;
            int xPos = i % 4;
            int yPos = j % 4;
            switch (xPos)
            {
                case 0:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 1;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 2;
                            break;
                        default:
                            uniqueAnimationFrameX = 2;
                            break;
                    }
                    break;
                case 1:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 2;
                            break;
                        default:
                            uniqueAnimationFrameX = 2;
                            break;
                    }
                    break;
                case 2:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 1;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 2;
                            break;
                        default:
                            uniqueAnimationFrameX = 2;
                            break;
                    }
                    break;
                case 3:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 1;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 2;
                            break;
                        default:
                            uniqueAnimationFrameX = 2;
                            break;
                    }
                    break;
            }
            frameXOffset = uniqueAnimationFrameX * animationFrameWidth;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (GlowMask.Texture is null)
                return;

            int xPos = Main.tile[i, j].TileFrameX;
            int yPos = Main.tile[i, j].TileFrameY;
            int xOffset = 0;
            int relativeXPos = i % 4;
            int relativeYPos = j % 4;
            switch (relativeXPos)
            {
                case 0:
                    switch (relativeYPos)
                    {
                        case 0:
                            xOffset = 0;
                            break;
                        case 1:
                            xOffset = 2;
                            break;
                        case 2:
                            xOffset = 1;
                            break;
                        case 3:
                            xOffset = 2;
                            break;
                        default:
                            xOffset = 2;
                            break;
                    }
                    break;
                case 1:
                    switch (relativeYPos)
                    {
                        case 0:
                            xOffset = 2;
                            break;
                        case 1:
                            xOffset = 0;
                            break;
                        case 2:
                            xOffset = 2;
                            break;
                        case 3:
                            xOffset = 2;
                            break;
                        default:
                            xOffset = 2;
                            break;
                    }
                    break;
                case 2:
                    switch (relativeYPos)
                    {
                        case 0:
                            xOffset = 2;
                            break;
                        case 1:
                            xOffset = 0;
                            break;
                        case 2:
                            xOffset = 1;
                            break;
                        case 3:
                            xOffset = 2;
                            break;
                        default:
                            xOffset = 2;
                            break;
                    }
                    break;
                case 3:
                    switch (relativeYPos)
                    {
                        case 0:
                            xOffset = 1;
                            break;
                        case 1:
                            xOffset = 2;
                            break;
                        case 2:
                            xOffset = 0;
                            break;
                        case 3:
                            xOffset = 2;
                            break;
                        default:
                            xOffset = 2;
                            break;
                    }
                    break;
            }
            xOffset *= 288;
            xPos += xOffset;

            if (GlowMask.HasContentInFramePos(xPos, yPos))
            {
                Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
                Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + zero;
                Color drawColour = GetDrawColour(i, j, new Color(25, 25, 25, 25));
                Tile trackTile = Main.tile[i, j];
                TileFraming.SlopedGlowmask(i, j, 0, GlowMask.Texture, drawOffset, null, GetDrawColour(i, j, drawColour), default);
            }
        }

        private Color GetDrawColour(int i, int j, Color colour)
        {
            int colType = Main.tile[i, j].TileColor;
            Color paintCol = WorldGen.paintColor(colType);
            if (colType >= 13 && colType <= 24)
            {
                colour.R = (byte)(paintCol.R / 255f * colour.R);
                colour.G = (byte)(paintCol.G / 255f * colour.G);
                colour.B = (byte)(paintCol.B / 255f * colour.B);
            }
            return colour;
        }
    }
}
