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
        public static bool ShouldAvoidLocation(Point placementPoint)
        {
            Tile tile = CalamityUtils.ParanoidTileRetrieval(placementPoint.X, placementPoint.Y);

            //avoid hive and jungle temple blocks
            if (tile.TileType == TileID.LihzahrdBrick || tile.WallType == WallID.LihzahrdBrickUnsafe)
            {
                return true;
            }

            return false;
        }

        public static void PlaceVernalPass(StructureMap structures)
        {
            int tries = 0;
            string mapKey = VernalKey;
            
            do
            {
                int placementPositionX = WorldGen.genRand.Next(WorldGen.jungleOriginX - 300, WorldGen.jungleOriginX + 300);
                int placementPositionY = WorldGen.genRand.Next((int)(Main.maxTilesY / 2), (int)((Main.maxTilesY - 500)));
                Point placementPoint = new Point(placementPositionX, placementPositionY);

                Vector2 schematicSize = new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1));
                int jungleStuffInArea = 0;
                bool canGenerateInLocation = true;

                float totalTiles = schematicSize.X * schematicSize.Y;
                for (int x = placementPoint.X - 50; x < placementPoint.X + schematicSize.X + 50; x++)
                {
                    for (int y = placementPoint.Y - 50; y < placementPoint.Y + schematicSize.Y + 50; y++)
                    {
                        Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);

                        if (ShouldAvoidLocation(new Point(x, y)))
                        {
                            canGenerateInLocation = false;
                        }

                        //check for jungle grasses
                        if (tile.TileType == TileID.JungleGrass || tile.TileType == TileID.JunglePlants || tile.TileType == TileID.JungleVines ||
                        tile.WallType == WallID.MudUnsafe || tile.WallType == WallID.JungleUnsafe || tile.TileType == TileID.Hive || tile.WallType == WallID.HiveUnsafe)
                        {
                            jungleStuffInArea++;
                        }

                        //immediately reset the counter if any invalid tiles are found
                        if (tile.TileType == TileID.LihzahrdBrick || tile.WallType == WallID.LihzahrdBrickUnsafe)
                        {
                            canGenerateInLocation = false;
                        }
                    }
                }

                if (!canGenerateInLocation || jungleStuffInArea < totalTiles * 0.1f || !structures.CanPlace(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y)))
                {
                    tries++;
                }
                else
                {
                    bool place = true;

                    //TODO: this needs to have items placed in chests, will do that later
                    SchematicManager.PlaceSchematic<Action<Chest>>(mapKey, new Point(placementPoint.X, placementPoint.Y), SchematicAnchor.Center, ref place);
                    structures.AddProtectedStructure(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y), 4);
                    break;
                }

            } while (tries <= 10000);
        }
    }
}
