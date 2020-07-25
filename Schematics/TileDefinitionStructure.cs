using System;
using Terraria;

namespace CalamityMod.Schematics
{
    public enum LiquidState
    {
        Water,
        Lava,
        Honey
    }
    // This class exists to make tile creation easier when creating schematic maps.
    public class TileDefinitionStructure
    {
        public Tile SpecifiedTile { get; private set; } = new Tile();
        public Action<int, int> SpecialAction { get; private set; } = null;
        public static readonly TileDefinitionStructure AirDefinition = new TileDefinitionStructure(0, false);

        // Don't remove this just because another constructor with a default argument exists. It seems the CodeDOM isn't smart enough to work with the other constructor.
        public TileDefinitionStructure(int type)
        {
            SpecifiedTile.type = (ushort)type;
            SpecifiedTile.liquid = 0;
            SpecifiedTile.active(true);
        }
        public TileDefinitionStructure(ushort type, bool active = true)
        {
            SpecifiedTile.type = type;
            SpecifiedTile.liquid = 0;
            SpecifiedTile.active(active);
        }
        public TileDefinitionStructure UseWall(ushort wallType, int xFrame, int yFrame)
        {
            SpecifiedTile.wall = wallType;
            SpecifiedTile.wallFrameX(xFrame);
            SpecifiedTile.wallFrameY(yFrame);
            return this;
        }
        public TileDefinitionStructure UseSlope(byte slope)
        {
            SpecifiedTile.slope(slope);
            return this;
        }
        public TileDefinitionStructure UseFrame(short frameX, short frameY)
        {
            SpecifiedTile.frameX = frameX;
            SpecifiedTile.frameY = frameY;
            return this;
        }
        public TileDefinitionStructure UseHeaders(byte header1, byte header2, byte header3, ushort header4)
        {
            SpecifiedTile.bTileHeader = header1;
            SpecifiedTile.bTileHeader2 = header2;
            SpecifiedTile.bTileHeader3 = header3;
            SpecifiedTile.sTileHeader = header4;
            return this;
        }
        public TileDefinitionStructure UseLiquid(LiquidState liquidType, byte liquidAmount)
        {
            SpecifiedTile.liquid = liquidAmount;
            SpecifiedTile.lava(liquidType == LiquidState.Lava);
            SpecifiedTile.honey(liquidType == LiquidState.Honey);
            return this;
        }
        public TileDefinitionStructure MakeActuated()
        {
            SpecifiedTile.inActive(true);
            return this;
        }
    }
}
