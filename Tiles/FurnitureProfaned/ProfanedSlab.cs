using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureProfaned
{
	public class ProfanedSlab : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
            soundType = 21;
            mineResist = 10f;
            minPick = 225;
            drop = mod.ItemType("ProfanedSlab");
			AddMapEntry(new Color(122, 66, 59));
            animationFrameHeight = 270;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, mod.DustType("ProfanedTileRock"), 0f, 0f, 1, new Color(255, 255, 255), 1f);
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
            //maximum number of variants is 7. A tile sheet can be used multiple times (although this
            //will disrupt the easy to understand arrangement of the tiles in the megasheet.
            //When testing for a tile position in the grid, apply a modulo (%) of the dimension in the axis to
            //either i (x) or j (y). Test if that modulo is equal to the target position (for example:
            //if (i % 4 == 0 && j % 4 == 2) will be for the tile at 0, 2 in a 4x4 grid)
            int uniqueAnimationFrame = Main.tileFrame[Type] + i;
            if (i % 4 == 0 && j % 4 == 0)
            {
                uniqueAnimationFrame = 1;
            }
            else if(i % 4 == 1 && j % 4 == 0)
            {
                uniqueAnimationFrame = 3;
            }
            else if (i % 4 == 2 && j % 4 == 0)
            {
                uniqueAnimationFrame = 1;
            }
            else if (i % 4 == 3 && j % 4 == 0)
            {
                uniqueAnimationFrame = 2;
            }
            else if (i % 4 == 0 && j % 4 == 1)
            {
                uniqueAnimationFrame = 0;
            }
            else if (i % 4 == 1 && j % 4 == 1)
            {
                uniqueAnimationFrame = 2;
            }
            else if (i % 4 == 2 && j % 4 == 1)
            {
                uniqueAnimationFrame = 0;
            }
            else if (i % 4 == 3 && j % 4 == 1)
            {
                uniqueAnimationFrame = 2;
            }
            else if (i % 4 == 0 && j % 4 == 2)
            {
                uniqueAnimationFrame = 1;
            }
            else if (i % 4 == 1 && j % 4 == 2)
            {
                uniqueAnimationFrame = 3;
            }
            else if (i % 4 == 2 && j % 4 == 2)
            {
                uniqueAnimationFrame = 1;
            }
            else if (i % 4 == 3 && j % 4 == 2)
            {
                uniqueAnimationFrame = 3;
            }
            else if (i % 4 == 0 && j % 4 == 3)
            {
                uniqueAnimationFrame = 0;
            }
            else if (i % 4 == 1 && j % 4 == 3)
            {
                uniqueAnimationFrame = 2;
            }
            else if (i % 4 == 2 && j % 4 == 3)
            {
                uniqueAnimationFrame = 0;
            }
            else if (i % 4 == 3 && j % 4 == 3)
            {
                uniqueAnimationFrame = 2;
            }
            //uniqueAnimationFrame = uniqueAnimationFrame % 2;

            frameYOffset = uniqueAnimationFrame * animationFrameHeight;
        }

        //public override void AnimateTile(ref int frame, ref int frameCounter)
        //{
        //    frameCounter++;
        //    if (frameCounter > 10)
        //    {
        //        frameCounter = 0;
        //        frame++;
        //        if (frame > 1)
        //        {
        //            frame = 0;
        //        }
        //    }
        //}
    }
}