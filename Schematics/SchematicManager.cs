using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CalamityMod.Schematics
{
    public static class SchematicManager
    {
        internal static Dictionary<string, SchematicMetaTile[,]> TileMaps;
        internal static Dictionary<string, PilePlacementFunction> PilePlacementMaps;
        public delegate void PilePlacementFunction(int x, int y, Rectangle placeInArea);

        internal static void Load()
        {
            PilePlacementMaps = new Dictionary<string, PilePlacementFunction>();
            TileMaps = new Dictionary<string, SchematicMetaTile[,]>
            {
                // Draedon's Arsenal world gen structures
                ["Workshop"] = CalamitySchematicIO.LoadSchematic("Schematics/Workshop.csch"),
                ["Research Facility"] = CalamitySchematicIO.LoadSchematic("Schematics/ResearchFacility.csch"),
                ["Hell Laboratory"] = CalamitySchematicIO.LoadSchematic("Schematics/HellLaboratory.csch"),
                ["Sunken Sea Laboratory"] = CalamitySchematicIO.LoadSchematic("Schematics/SunkenSeaLaboratory.csch"),
                ["Ice Laboratory"] = CalamitySchematicIO.LoadSchematic("Schematics/IceLaboratory.csch"),
                ["Plague Laboratory"] = CalamitySchematicIO.LoadSchematic("Schematics/PlagueLaboratory.csch"),
                ["Planetoid Laboratory"] = CalamitySchematicIO.LoadSchematic("Schematics/PlanetoidLaboratory.csch"),

                // Astral world gen structures
                ["Astral Beacon"] = CalamitySchematicIO.LoadSchematic("Schematics/AstralBeacon.csch"),
            };
        }
        internal static void Unload()
        {
            TileMaps = null;
            PilePlacementMaps = null;
        }
    }
}
