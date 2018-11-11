using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
	public class AstralMonolith : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            drop = mod.ItemType("AstralMonolith");
            AddMapEntry(new Color(100, 100, 150));
            animationFrameHeight = 270;
        }
        int animationFrameWidth = 288;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(149, 96, 255), 1f);
            return false;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            //So the basic summary of how this works is that for each possible state there is a unique tile sheet
            //for every single unique position. This, as opposed to animating, is used to set which tile sheet is
            //used for each tile position. Think of it as a grid, only converted into a single axis.
            //Say you have a 3 x 3 grid. The 3 tile sheetes are 1 row, the next three are the next row and the 
            //final 3 are the last row. Simple. So for determining the y offset, 1 is added to
            //uniqueAnimationFrame, and the x dimension of the gid is added to determine the x offset of where
            //the tile will appear. Set the relevant uniqueAnimationFrame to equal this value.
            //There needs to be a tile sheet for each tile in the grid though, assuming that none are rused, so
            //the grid can only be so big due to sheet size limitations. Unless tile sheets are recycled, the
            //maximum number of variants is 42, 7 sheets vertically, 6 sheets horizontally. A tile sheet can be 
            //used multiple times (although this will disrupt the easy to understand arrangement of the tiles 
            //in the megasheet. When testing for a tile position in the grid, apply a modulo (%) of the 
            //dimension in the axis to either i (x) or j (y). Test if that modulo is equal to the target position
            // (for example: if (i % 4 == 0 && j % 4 == 2) will be for the tile at 0, 2 in a 4x4 grid)
            int uniqueAnimationFrameX = Main.tileFrame[Type] + i;
            int uniqueAnimationFrameY = Main.tileFrame[Type] + j;
            int xPos = i % 4;
            int yPos = j % 4;
            switch (xPos)
            {
                case 0:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 0;
                            uniqueAnimationFrameY = 0;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 0;
                            uniqueAnimationFrameY = 1;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 0;
                            uniqueAnimationFrameY = 2;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 0;
                            uniqueAnimationFrameY = 3;
                            break;
                        default:
                            uniqueAnimationFrameX = 0;
                            uniqueAnimationFrameY = 0;
                            break;
                    }
                    break;
                case 1:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 1;
                            uniqueAnimationFrameY = 0;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 1;
                            uniqueAnimationFrameY = 1;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 1;
                            uniqueAnimationFrameY = 2;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 1;
                            uniqueAnimationFrameY = 3;
                            break;
                        default:
                            uniqueAnimationFrameX = 1;
                            uniqueAnimationFrameY = 0;
                            break;
                    }
                    break;
                case 2:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 2;
                            uniqueAnimationFrameY = 0;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 2;
                            uniqueAnimationFrameY = 1;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 2;
                            uniqueAnimationFrameY = 2;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 2;
                            uniqueAnimationFrameY = 3;
                            break;
                        default:
                            uniqueAnimationFrameX = 2;
                            uniqueAnimationFrameY = 0;
                            break;
                    }
                    break;
                case 3:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 3;
                            uniqueAnimationFrameY = 0;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 3;
                            uniqueAnimationFrameY = 1;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 3;
                            uniqueAnimationFrameY = 2;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 3;
                            uniqueAnimationFrameY = 3;
                            break;
                        default:
                            uniqueAnimationFrameX = 3;
                            uniqueAnimationFrameY = 0;
                            break;
                    }
                    break;
            }
            frameXOffset = uniqueAnimationFrameX * animationFrameWidth;
            frameYOffset = uniqueAnimationFrameY * animationFrameHeight;
        }
    }
}