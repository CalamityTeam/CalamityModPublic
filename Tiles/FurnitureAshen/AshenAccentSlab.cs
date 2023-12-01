using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.FurnitureAshen
{
    public class AshenAccentSlab : ModTile
    {
        public override string Texture => "CalamityMod/Tiles/FurnitureAshen/AshenSlab";

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;

            HitSound = SoundID.Tink;
            MineResist = 5f;
            MinPick = 180;
            AddMapEntry(new Color(40, 24, 48));
            AnimationFrameHeight = 90;
        }
        int animationFrameWidth = 234;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 60, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(100, 100, 100), 1f);
            return false;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            int uniqueAnimationFrameX = Main.tileFrame[Type] + i;
            int uniqueAnimationFrameY = Main.tileFrame[Type] + j;
            int xPos = i % 2;
            int yPos = j % 3;
            int xPattern = i % 20 / 2;
            int yPattern = j % 30 / 3;
            int xOffset;
            switch (xPattern)
            {
                case 0:
                    switch (yPattern)
                    {
                        case 1:
                            xOffset = 1;
                            break;
                        case 5:
                            xOffset = 0;
                            break;
                        case 8:
                            xOffset = 2;
                            break;
                        case 9:
                            xOffset = 1;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 1:
                    switch (yPattern)
                    {
                        case 0:
                            xOffset = 2;
                            break;
                        case 3:
                            xOffset = 0;
                            break;
                        case 8:
                            xOffset = 1;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 2:
                    switch (yPattern)
                    {
                        case 2:
                            xOffset = 0;
                            break;
                        case 7:
                            xOffset = 1;
                            break;
                        case 8:
                            xOffset = 0;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 3:
                    switch (yPattern)
                    {
                        case 2:
                            xOffset = 2;
                            break;
                        case 5:
                            xOffset = 2;
                            break;
                        case 7:
                            xOffset = 1;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 4:
                    switch (yPattern)
                    {
                        case 1:
                            xOffset = 0;
                            break;
                        case 4:
                            xOffset = 0;
                            break;
                        case 9:
                            xOffset = 2;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 5:
                    switch (yPattern)
                    {
                        case 0:
                            xOffset = 1;
                            break;
                        case 7:
                            xOffset = 0;
                            break;
                        case 8:
                            xOffset = 1;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 6:
                    switch (yPattern)
                    {
                        case 5:
                            xOffset = 2;
                            break;
                        case 8:
                            xOffset = 1;
                            break;
                        case 12:
                            xOffset = 0;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 7:
                    switch (yPattern)
                    {
                        case 2:
                            xOffset = 0;
                            break;
                        case 6:
                            xOffset = 0;
                            break;
                        case 7:
                            xOffset = 1;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 8:
                    switch (yPattern)
                    {
                        case 3:
                            xOffset = 0;
                            break;
                        case 4:
                            xOffset = 0;
                            break;
                        case 9:
                            xOffset = 0;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 9:
                    switch (yPattern)
                    {
                        case 1:
                            xOffset = 0;
                            break;
                        case 7:
                            xOffset = 0;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                default:
                    xOffset = -1;
                    break;
            }
            if (yPos < 2)
            {
                if (xOffset != -1)
                {
                    if (j % 3 < 2)
                    {
                        uniqueAnimationFrameX = Main.tile[i - (i % 2), j - (j % 3)].TileFrameNumber;
                    }
                    if (uniqueAnimationFrameX != 0)
                    {
                        uniqueAnimationFrameX += xOffset;
                    }
                }
                else
                {
                    uniqueAnimationFrameX = 0;
                }
            }
            switch (yPos)
            {
                case 0:
                    switch (xPos)
                    {
                        case 0:
                            uniqueAnimationFrameY = 0;
                            break;
                        case 1:
                            uniqueAnimationFrameY = 2;
                            break;
                    }
                    break;
                case 1:
                    switch (xPos)
                    {
                        case 0:
                            uniqueAnimationFrameY = 1;
                            break;
                        case 1:
                            uniqueAnimationFrameY = 3;
                            break;
                    }
                    break;
                case 2:
                    uniqueAnimationFrameY = 4 + xPos;
                    uniqueAnimationFrameX = 0;
                    break;
                default:
                    uniqueAnimationFrameY = 0;
                    break;
            }
            frameXOffset = uniqueAnimationFrameX * animationFrameWidth;
            frameYOffset = uniqueAnimationFrameY * AnimationFrameHeight;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int uniqueAnimationFrameX = 0;
            int uniqueAnimationFrameY = 0;
            int xPos = i % 2;
            int yPos = j % 3;
            int xPattern = i % 20 / 2;
            int yPattern = j % 30 / 3;
            int xOffset;
            switch (xPattern)
            {
                case 0:
                    switch (yPattern)
                    {
                        case 1:
                            xOffset = 1;
                            break;
                        case 5:
                            xOffset = 0;
                            break;
                        case 8:
                            xOffset = 2;
                            break;
                        case 9:
                            xOffset = 1;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 1:
                    switch (yPattern)
                    {
                        case 0:
                            xOffset = 2;
                            break;
                        case 3:
                            xOffset = 0;
                            break;
                        case 8:
                            xOffset = 1;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 2:
                    switch (yPattern)
                    {
                        case 2:
                            xOffset = 0;
                            break;
                        case 7:
                            xOffset = 1;
                            break;
                        case 8:
                            xOffset = 0;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 3:
                    switch (yPattern)
                    {
                        case 2:
                            xOffset = 2;
                            break;
                        case 5:
                            xOffset = 2;
                            break;
                        case 7:
                            xOffset = 1;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 4:
                    switch (yPattern)
                    {
                        case 1:
                            xOffset = 0;
                            break;
                        case 4:
                            xOffset = 0;
                            break;
                        case 9:
                            xOffset = 2;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 5:
                    switch (yPattern)
                    {
                        case 0:
                            xOffset = 1;
                            break;
                        case 7:
                            xOffset = 0;
                            break;
                        case 8:
                            xOffset = 1;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 6:
                    switch (yPattern)
                    {
                        case 5:
                            xOffset = 2;
                            break;
                        case 8:
                            xOffset = 1;
                            break;
                        case 12:
                            xOffset = 0;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 7:
                    switch (yPattern)
                    {
                        case 2:
                            xOffset = 0;
                            break;
                        case 6:
                            xOffset = 0;
                            break;
                        case 7:
                            xOffset = 1;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 8:
                    switch (yPattern)
                    {
                        case 3:
                            xOffset = 0;
                            break;
                        case 4:
                            xOffset = 0;
                            break;
                        case 9:
                            xOffset = 0;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                case 9:
                    switch (yPattern)
                    {
                        case 1:
                            xOffset = 0;
                            break;
                        case 7:
                            xOffset = 0;
                            break;
                        default:
                            xOffset = -1;
                            break;
                    }
                    break;
                default:
                    xOffset = -1;
                    break;
            }
            if (yPos < 2)
            {
                if (xOffset != -1)
                {
                    if (j % 3 < 2)
                    {
                        uniqueAnimationFrameX = Main.tile[i - (i % 2), j - (j % 3)].TileFrameNumber;
                    }
                    if (uniqueAnimationFrameX != 0)
                    {
                        uniqueAnimationFrameX += xOffset;
                    }
                }
                else
                {
                    uniqueAnimationFrameX = 0;
                }
            }
            switch (yPos)
            {
                case 0:
                    switch (xPos)
                    {
                        case 0:
                            uniqueAnimationFrameY = 0;
                            break;
                        case 1:
                            uniqueAnimationFrameY = 2;
                            break;
                    }
                    break;
                case 1:
                    switch (xPos)
                    {
                        case 0:
                            uniqueAnimationFrameY = 1;
                            break;
                        case 1:
                            uniqueAnimationFrameY = 3;
                            break;
                    }
                    break;
                case 2:
                    uniqueAnimationFrameY = 4 + xPos;
                    uniqueAnimationFrameX = 0;
                    break;
                default:
                    uniqueAnimationFrameY = 0;
                    break;
            }
            int AnimationFrameHeight = 90;
            int animationFrameWidth = 234;
            int xDrawPos = Main.tile[i, j].TileFrameX + (uniqueAnimationFrameX * animationFrameWidth);
            int yDrawPos = Main.tile[i, j].TileFrameY + (uniqueAnimationFrameY * AnimationFrameHeight);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + zero;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Tiles/FurnitureAshen/AshenSlabGlow").Value;
            Color drawColour = GetDrawColour(i, j, new Color(64, 64, 64, 64));
            Tile trackTile = Main.tile[i, j];
            if (!trackTile.IsHalfBlock && trackTile.Slope == 0)
            {
                Main.spriteBatch.Draw(glowmask, drawOffset, new Rectangle?(new Rectangle(xDrawPos, yDrawPos, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
            else if (trackTile.IsHalfBlock)
            {
                Main.spriteBatch.Draw(glowmask, drawOffset + new Vector2(0f, 8f), new Rectangle?(new Rectangle(xDrawPos, yDrawPos, 18, 8)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
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

        /*
        /// <summary>
        /// Gets the tile variant to use. Returns -1 by default
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public int GetTileVariant(int i, int j)
        {
            int variant = 0; //Default to using variant 1
            if (j % 3 < 2)
            {
                //We need to get the particular tile to refer to. This is the top left tile in each iteration of the pattern (large slab above the small rune bricks
                Tile sourceTile = Main.tile[i - (i % 2), j - (j % 3)]; //Gets the tile for this iteration by taking the current position of the tile minus this tile's position in the pattern
                                                                       //Now to get the particular 'variant group' to use, which is used to take the frameX/frameY of the tile and convert it to the variant the tile is using
                int frameX = sourceTile.frameX / 18;
                int frameY = sourceTile.frameY / 18;
                if (frameY < 3 && !(frameX >= 6 && frameX < 9))
                {
                    int group = 0;
                    int[] group1XPos = new int[] { 1, 2, 3 };
                    int[] group2XPos = new int[] { 0, 4, 5, 9, 10, 11, 12 };
                    foreach (int k in group1XPos)
                    {
                        if (frameX < k + 1 && frameX >= k)
                        {
                            group = 1;
                        }
                    }
                    foreach (int k in group2XPos)
                    {
                        if (frameX < k + 1 && frameX >= k)
                        {
                            group = 2;
                        }
                    }
                    if (group == 1)
                    {
                        if (frameX < 2)
                        {
                            variant = 0;
                        }
                        else if (frameX < 3)
                        {
                            variant = 1;
                        }
                        else
                        {
                            variant = 2;
                        }
                    }
                    else if (group == 2)
                    {
                        if (frameY < 1)
                        {
                            variant = 0;
                        }
                        else if (frameY < 2)
                        {
                            variant = 1;
                        }
                        else
                        {
                            variant = 2;
                        }
                    }
                }
                else if (frameX < 6 && frameY >= 3)
                {
                    if (frameX < 2)
                    {
                        variant = 0;
                    }
                    else if (frameX < 4)
                    {
                        variant = 1;
                    }
                    else
                    {
                        variant = 2;
                    }
                }
                else if (frameX >= 6 && frameX < 9)
                {
                    if (frameX < 7)
                    {
                        variant = 0;
                    }
                    else if (frameX < 8)
                    {
                        variant = 1;
                    }
                    else
                    {
                        variant = 2;
                    }
                }
                else if (frameX >= 9 && frameY >= 3)
                {
                    if (frameX < 10)
                    {
                        variant = 0;
                    }
                    else if (frameX < 11)
                    {
                        variant = 1;
                    }
                    else
                    {
                        variant = 2;
                    }
                }
            }
            return (variant);
        }
        */
    }
}
