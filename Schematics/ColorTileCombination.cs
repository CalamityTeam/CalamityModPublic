using Terraria;
using Microsoft.Xna.Framework;

namespace CalamityMod.Schematics
{
    public struct ColorTileCombination
    {
        internal Color InternalColor;
        internal Tile InternalTile;
        public ColorTileCombination(Color color, Tile tile)
        {
            InternalColor = color;
            InternalTile = tile;
        }
    }
}
