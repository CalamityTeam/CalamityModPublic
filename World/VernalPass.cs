using CalamityMod.Items.Accessories;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.SummonItems;
using CalamityMod.Tiles.DraedonStructures;
using CalamityMod.Tiles.SunkenSea;
using CalamityMod.Walls;
using CalamityMod.Schematics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
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
            if (tile.TileType == TileID.LihzahrdBrick || tile.WallType == WallID.LihzahrdBrickUnsafe || 
            tile.TileType == TileID.Hive || tile.WallType == WallID.HiveUnsafe)
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
                int placementPositionX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f));
                int placementPositionY = WorldGen.genRand.Next((int)(Main.maxTilesY / 2), (int)((Main.maxTilesY - 300)));
                Point placementPoint = new Point(placementPositionX, placementPositionY);

                Vector2 schematicSize = new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1));
                int jungleGrassInArea = 0;
                bool canGenerateInLocation = true;

                float totalTiles = schematicSize.X * schematicSize.Y;
                for (int x = placementPoint.X; x < placementPoint.X + schematicSize.X; x++)
                {
                    for (int y = placementPoint.Y; y < placementPoint.Y + schematicSize.Y; y++)
                    {
                        Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);

                        if (ShouldAvoidLocation(new Point(x, y)))
                        {
                            canGenerateInLocation = false;
                        }

                        if (tile.TileType == TileID.JungleGrass || tile.TileType == TileID.JunglePlants || tile.TileType == TileID.JungleVines)
                        {
                            jungleGrassInArea++;
                        }
                    }
                }
                if (!canGenerateInLocation || jungleGrassInArea < totalTiles * 0.1f ||  !structures.CanPlace(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y)))
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

            } while (tries <= 20000);
        }
    }
}
