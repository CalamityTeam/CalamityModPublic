using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.BaseTiles
{
    public abstract class DoubleDirectionalTile : ModTile
    {
        public enum TileDirection : byte
        {
            Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight, Center
        }

        // There is a chungus among us
        // This is based largely on characteristics exhibited by vanilla's tile sheets.
        public static readonly TileDirection?[,] WhereDoIPoint = new TileDirection?[,] 
        { 
            { TileDirection.Left, TileDirection.Up, TileDirection.Up, TileDirection.Up, TileDirection.Right, null, null, null, null, null, TileDirection.Left, TileDirection.Right, null },          
            { TileDirection.Left, TileDirection.Center, TileDirection.Center, TileDirection.Center, TileDirection.Right, null, TileDirection.Up, TileDirection.Up, TileDirection.Up, null, TileDirection.Left, TileDirection.Right, null },
            { TileDirection.Left, TileDirection.Down, TileDirection.Down, TileDirection.Down, TileDirection.Right, null, TileDirection.Down, TileDirection.Down, TileDirection.Down, null, TileDirection.Left, TileDirection.Right, null },
            { TileDirection.UpLeft, TileDirection.UpRight, TileDirection.UpLeft, TileDirection.UpRight, TileDirection.UpLeft, TileDirection.UpRight, null, null, null, null, null, null, null },
            { TileDirection.DownLeft, TileDirection.DownRight, TileDirection.DownLeft, TileDirection.DownRight, TileDirection.DownLeft, TileDirection.DownRight, null, null, null, null, null, null, null }
        };
        
        public static readonly int?[,] WhatVariantAmI = new int?[,]
        {
            { 1, 1, 2, 3, 1, 1, 1, 2, 3, 1, 1, 1, 1 },
            { 2, 1, 2, 3, 2, 2, 1, 2, 3, 2, 2, 2, 2 },
            { 3, 1, 2, 3, 3, 3, 1, 2, 3, 3, 3, 3, 3 },
            { 1, 1, 2, 2, 3, 3, 1, 2, 3, 1, 2, 3, null },
            { 1, 1, 2, 2, 3, 3, 1, 2, 3, null, null, null, null }
        };

        public TileDirection? GiveDirection(int typeToConnectTo, int i, int j)
        {
            Tile tile = CalamityUtils.ParanoidTileRetrieval(i, j);

            // We enforce racism here.
            if (tile.type != typeToConnectTo)
                return null;

            int slotY = tile.frameX / 18;
            int slotX = tile.frameY / 18;
            return WhereDoIPoint[slotX, slotY];
        }

        public int GiveVariant(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int slotY = tile.frameX / 18;
            int slotX = tile.frameY / 18;
            return (int)WhatVariantAmI[slotX, slotY];
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            if (GiveDirection(type, i, j) != TileDirection.Center)
                return;

            TileDirection? TopLeft = GiveDirection(type, i - 1, j - 1);
            TileDirection? Top = GiveDirection(type, i, j - 1);
            TileDirection? TopRight = GiveDirection(type, i + 1, j - 1);
            TileDirection? Left = GiveDirection(type, i - 1, j);
            TileDirection? Right = GiveDirection(type, i + 1, j);
            TileDirection? BottomLeft = GiveDirection(type, i - 1, j + 1);
            TileDirection? Bottom = GiveDirection(type, i, j + 1);
            TileDirection? BottomRight = GiveDirection(type, i + 1, j + 1);

            //Shitty if chain incoming :| Wonder if thats doable with a switch statement, i just dont know how.
            if (Top == TileDirection.Up && Bottom == TileDirection.Center)
            {
                frameXOffset = -18 + -18 * (GiveVariant(i, j) - 1);
                frameYOffset = -18 + 90 + 18 * (GiveVariant(i, j) - 1);
            }
            if (Top == TileDirection.Center && Bottom == TileDirection.Down)
            {
                frameXOffset = -18 * (GiveVariant(i, j) - 1);
                frameYOffset = -18 + 90 + 18 * (GiveVariant(i, j) - 1);
            }
            if (Left == TileDirection.Left && Right == TileDirection.Center)
            {
                frameXOffset = 18 + -18 * (GiveVariant(i, j) - 1);
                frameYOffset = -18 + 90 + 18 * (GiveVariant(i, j) - 1);
            }
            if (Left == TileDirection.Center && Right == TileDirection.Right)
            {
                frameXOffset = 36 + -18 * (GiveVariant(i, j) - 1);
                frameYOffset = -18 + 90 + 18 * (GiveVariant(i, j) - 1);
            }
            if ((Top == TileDirection.Up && Left == TileDirection.Left && Bottom == TileDirection.Center && Right == TileDirection.Center) ||
                (Top == TileDirection.Left && Left == TileDirection.Up && (BottomRight == TileDirection.Center || BottomRight == TileDirection.DownRight)) ||
                (Top == TileDirection.UpLeft && Left == TileDirection.UpLeft))
            {
                frameXOffset = 54 + -18 * (GiveVariant(i, j) - 1);
                frameYOffset = -18 + 90 + 18 * (GiveVariant(i, j) - 1);
            }
            if ((Top == TileDirection.Up && Right == TileDirection.Right && Bottom == TileDirection.Center && Left == TileDirection.Center) ||
               (Top == TileDirection.Right && Right == TileDirection.Up && (BottomLeft == TileDirection.Center || BottomLeft == TileDirection.DownLeft)) ||
               (Top == TileDirection.UpRight && Right == TileDirection.UpRight))
            {
                frameXOffset = 72 + -18 * (GiveVariant(i, j) - 1);
                frameYOffset = -18 + 90 + 18 * (GiveVariant(i, j) - 1);
            }
            if ((Bottom == TileDirection.Down && Left == TileDirection.Left && Top == TileDirection.Center && Right == TileDirection.Center) ||
               (Bottom == TileDirection.Left && Left == TileDirection.Down && (TopRight == TileDirection.Center || TopRight == TileDirection.UpRight)) ||
               (Bottom == TileDirection.DownLeft && Left == TileDirection.DownLeft))
            {
                frameXOffset = 90 + -18 * (GiveVariant(i, j) - 1);
                frameYOffset = -18 + 90 + 18 * (GiveVariant(i, j) - 1);
            }
            if ((Bottom == TileDirection.Down && Right == TileDirection.Right && Top == TileDirection.Center && Left == TileDirection.Center) ||
               (Bottom == TileDirection.Right && Right == TileDirection.Down && (TopLeft == TileDirection.Center || TopLeft == TileDirection.UpLeft)) ||
               (Bottom == TileDirection.DownRight && Right == TileDirection.DownRight))
            {
                frameXOffset = 108 + -18 * (GiveVariant(i, j) - 1);
                frameYOffset = -18 + 90 + 18 * (GiveVariant(i, j) - 1);
            }
        }
    }
}