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
            // TODO -- re-enable this code after the schematics exist again.
            /*
            TileMaps = new Dictionary<string, SchematicMetaTile[,]>
            {
                // Draedon's Arsenal world gen structures
                ["Workshop"] = CalamitySchematicIO.LoadSchematic("Schematics/WorkshopSchematic.csch"),
                ["Research Facility"] = CalamitySchematicIO.LoadSchematic("Schematics/ResearchFacilitySchematic.csch"),
                ["Hell Laboratory"] = CalamitySchematicIO.LoadSchematic("Schematics/HellLaboratorySchematic.csch"),
                ["Sunken Sea Laboratory"] = CalamitySchematicIO.LoadSchematic("Schematics/SunkenSeaLaboratorySchematic.csch"),
                ["Ice Laboratory"] = CalamitySchematicIO.LoadSchematic("Schematics/IceLaboratorySchematic.csch"),
                ["Plague Laboratory"] = CalamitySchematicIO.LoadSchematic("Schematics/PlagueLaboratorySchematic.csch"),
                ["Planetoid Laboratory"] = CalamitySchematicIO.LoadSchematic("Schematics/PlanetoidLaboratorySchematic.csch"),

                // Astral world gen structures
                ["Astral Beacon"] = CalamitySchematicIO.LoadSchematic("Schematics/AstralBeaconSchematic.csch"),
            };
            */
        }
        internal static void Unload()
        {
            TileMaps = null;
            PilePlacementMaps = null;
        }
    }
}
