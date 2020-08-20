using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CalamityMod.Schematics
{
    public class ColorTileMap
    {
        internal readonly Dictionary<Color, TileDefinitionStructure> Map = new Dictionary<Color, TileDefinitionStructure>();
        public ColorTileMap(Dictionary<Color, TileDefinitionStructure> colorTileMap)
        {
            Map = colorTileMap;
        }
        // For general convenience and as a means of mitigating the key check every time.
        public TileDefinitionStructure this[Color key] => Map.ContainsKey(key) ? Map[key] : TileDefinitionStructure.AirDefinition;
    }
}
