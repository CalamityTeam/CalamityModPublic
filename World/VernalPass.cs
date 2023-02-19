using System;
using CalamityMod.Schematics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;
using static CalamityMod.Schematics.SchematicManager;

namespace CalamityMod.World
{
    public class VernalPass
    {
        public static void PlaceVernalPass(StructureMap structures)
        {
            int tries = 0;
            string mapKey = VernalKey;

            int placementPositionX = WorldGen.genRand.Next(WorldGen.tLeft, WorldGen.tRight);
            int placementPositionY = WorldGen.tTop - 120;
            Point placementPoint = new Point(placementPositionX, placementPositionY);

            Vector2 schematicSize = new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1));
            bool place = true;

            //TODO: this needs to have items placed in chests, will do that later
            SchematicManager.PlaceSchematic<Action<Chest>>(mapKey, new Point(placementPoint.X, placementPoint.Y), SchematicAnchor.Center, ref place);
            structures.AddProtectedStructure(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y), 30);
        }
    }
}
