using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace CalamityMod.World.Planets
{
    public class MainPlanet : Planetoid
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            // 1 on Small, 1.52 on Medium, 2 on Large
            float scale = (float)Main.maxTilesX / 4200f;
            // Clamp scale to prevent problems on extra large worlds
            scale = MathHelper.Clamp(scale, 1f, 2f);
            int radius = (int)((float)_random.Next(30, 36) * scale); //50 to 65

            if (!CheckIfPlaceable(origin, radius, structures))
            {
                return false;
            }

            PlacePlanet(origin, radius);

            return base.Place(origin, structures);
        }

        public void PlacePlanet(Point origin, int radius)
        {
            Circle planetoid = new Circle(origin.ToVector2() * 16f + new Vector2(8f), radius * 16f);

            ushort LabTile = TileID.StoneSlab;
            byte LabWall = WallID.StoneSlab;
            bool labLeftSide = _random.NextBool();
            int corridorLength = (int)(radius * _random.NextFloat(0.7f, 0.8f));

            //PLACE DIRT AND WALL
            ShapeData mainShape = new ShapeData();
            WorldUtils.Gen(origin, new Shapes.Circle(radius), Actions.Chain(new GenAction[]
            {
                new Modifiers.Blotches(2, 0.6).Output(mainShape),
                new Actions.SetTile(TileID.Dirt, true),
                new Actions.PlaceWall(WallID.DirtUnsafe)
            }));

            //PLACE STONE AND WALL
            float outerRadiusPercentage = _random.NextFloat(0.75f, 0.85f);
            Circle outerCore = new Circle(planetoid.Center, planetoid.Radius * outerRadiusPercentage);
            ShapeData outerCoreShape = new ShapeData();
            WorldUtils.Gen(origin, new Shapes.Circle((int)(radius * outerRadiusPercentage)), Actions.Chain(new GenAction[]
            {
                new Modifiers.Blotches(2, 0.45).Output(outerCoreShape),
                new Actions.SetTile(TileID.Stone, true),
                new Actions.ClearWall(),
                new Actions.PlaceWall(WallID.CaveUnsafe)
            }));

            //PLACE STONE THEN DIRT PATCHES
            int numStone = _random.Next(30, 40);
            while (numStone > 0)
            {
                Point p = outerCore.RandomPointOnCircleEdge().ToTileCoordinates();
                WorldGen.TileRunner(p.X, p.Y, _random.NextFloat(3f, 6f), _random.Next(5, 15), TileID.Stone);
                numStone--;
            }
            int numDirt = _random.Next(80, 110);
            while (numDirt > 0)
            {
                Point p = outerCore.RandomPointInCircle().ToTileCoordinates();
                WorldGen.TileRunner(p.X, p.Y, _random.NextFloat(3f, 6f), _random.Next(7, 17), TileID.Dirt);
                numDirt--;
            }

            //PLACE CAVES
            int caveCount = _random.Next(8, 14);
            while (caveCount > 0)
            {
                caveCount--;

                Point tileCoords = planetoid.RandomPointInCircle().ToTileCoordinates();

                WorldGen.TileRunner(tileCoords.X, tileCoords.Y, _random.NextFloat(5f, 10f), _random.Next(64, 91), -1);
            }
            WorldGen.TileRunner(origin.X + (labLeftSide ? corridorLength : -corridorLength), origin.Y, _random.NextFloat(11f, 15f), _random.Next(3, 5), -1);

            mainShape.Subtract(outerCoreShape, origin, origin);

            //PLACE GRASS
            Vector2 worldCenter = origin.ToWorldCoordinates();
            WorldUtils.Gen(origin, new ModShapes.OuterOutline(mainShape, true, true), Actions.Chain(new GenAction[]
            {
                new Modifiers.OnlyTiles(new ushort[] { TileID.Dirt }),
                new Modifiers.IsTouchingAir(true),
                new Actions.SetTile(TileID.Grass, false, true),
                new CustomActions.DistanceFromOrigin(true, radius * 16f - 48),
                new Actions.ClearWall(false),
                new Actions.SetFrames(true),
                new Modifiers.Conditions(new CustomConditions.RandomChance(4)),
                new Actions.Smooth(),
                new Actions.SetFrames(true)
            }));

            //PLACE WALL TO SURROUND CINDERPLATE
            ShapeData buildingData = new ShapeData();
            int cinderRadius = (int)(radius * _random.NextFloat(0.15f, 0.17f));
            WorldUtils.Gen(origin, new Shapes.Circle(cinderRadius + 9), Actions.Chain(new GenAction[]
            {
                new Actions.ClearTile(true).Output(buildingData),
                new Actions.ClearWall(),
                new Actions.PlaceWall(LabWall)
            }));

            //PLACE CINDERPLATE
            ShapeData cinderPlateBlob = new ShapeData();
            WorldUtils.Gen(origin, new Shapes.Circle(cinderRadius), Actions.Chain(new GenAction[]
            {
                new Actions.SetTile((ushort)ModContent.GetInstance<CalamityMod>().TileType("Cinderplate"), true).Output(cinderPlateBlob),
                new Actions.ClearWall(),
                new Actions.PlaceWall(WallID.LavaUnsafe2)
            }));

            //PLACE LAVA IN CENTER
            WorldUtils.Gen(origin, new Shapes.Circle((int)(cinderRadius * 0.4f)), Actions.Chain(new GenAction[]
            {
                new Modifiers.Blotches(2, 0.5),
                new Actions.ClearTile(true),
                new Actions.SetLiquid(1, 255),
                new Actions.SetFrames(true)
            }));

            outerCoreShape.Subtract(buildingData, origin, origin);
            buildingData.Subtract(cinderPlateBlob, origin, origin);

            //PLACE TILES FOR BUILDING
            ShapeData bottomTiles = new ShapeData();
            WorldUtils.Gen(origin, new ModShapes.InnerOutline(buildingData, true), Actions.Chain(new GenAction[]
            {
                new Actions.ClearWall(),
                new Actions.PlaceTile(LabTile),
                new CustomActions.DistanceFromOrigin(true, cinderRadius * 16 + 64).Output(bottomTiles)
            }));

            int doPillarEveryXBlocks = 5;
            ushort BeamType = TileID.WoodenBeam;

            //PLACE BEAMS BELOW CIRCLE
            HashSet<Point16> bottomMostTiles = bottomTiles.GetData();
            foreach (Point16 p in bottomMostTiles)
            {
                int tileX = origin.X + p.X;
                int tileY = origin.Y + p.Y;
                if (!_tiles[tileX, tileY + 1].active() && tileX % doPillarEveryXBlocks == 0)
                {
                    PlaceBeams(BeamType, tileX, tileY + 1, worldCenter, radius, LabWall);
                }
            }

            //PLACE HORIZONTAL CORRIDORS
            for (int x = origin.X - corridorLength; x <= origin.X + corridorLength; x++)
            {
                for (int y = origin.Y - 4; y <= origin.Y + 4; y++)
                {
                    int distFromOriginX = Math.Abs(origin.X - x);
                    if (distFromOriginX <= cinderRadius + 1)
                        continue;

                    if (x == origin.X - corridorLength || x == origin.X + corridorLength)
                    {
                        _tiles[x, y].active(true);
                        _tiles[x, y].type = LabTile;
                    }
                    else if (y == origin.Y - 4 || y == origin.Y + 4)
                    {
                        bool bottom = y == origin.Y + 4;
                        if (distFromOriginX < cinderRadius + 9 && !_tiles[x, y].active())
                        {
                            _tiles[x, y].active(false);
                            if (bottom)
                                WorldGen.PlaceTile(x, y, TileID.Platforms, true);
                        }
                        else
                        {
                            _tiles[x, y].active(true);
                            _tiles[x, y].type = LabTile;
                            //IF NECESSARY TO PLACE SUPPORT BEAMS (EVERY X BLOCKS)
                            if (bottom && !_tiles[x, y + 1].active() && x % doPillarEveryXBlocks == 0)
                            {
                                PlaceBeams(BeamType, x, y + 1, worldCenter, radius, LabWall);
                            }
                        }
                    }
                    else
                    {
                        _tiles[x, y].active(false);
                        _tiles[x, y].wall = LabWall;
                    }
                }
            }

            //PLACE LAB ROOM
            int doorX = labLeftSide ? origin.X - corridorLength + 14 : origin.X + corridorLength - 14;
            for (int y3 = origin.Y - 3; y3 <= origin.Y; y3++)
            {
                Main.tile[doorX, y3].active(true);
                Main.tile[doorX, y3].type = TileID.StoneSlab;
            }
            WorldGen.PlaceDoor(doorX, origin.Y + 2, TileID.ClosedDoor, 14);
            int lanternX = labLeftSide ? doorX - 7 : doorX + 7;
            int chest;
            if (labLeftSide)
            {
                //Lantern, chest, heavy work bench
                WorldGen.Place1x2Top(lanternX, origin.Y - 3, TileID.HangingLanterns, 14);
                chest = WorldGen.PlaceChest(doorX - 12, origin.Y + 3);
                WorldGen.Place3x3(doorX - 8, origin.Y + 3, TileID.HeavyWorkBench);
                //switch
                WorldGen.PlaceTile(doorX - 1, origin.Y, TileID.Switches, true);
                //wire
                for (int x = doorX - 1; x >= doorX - 7; x--)
                {
                    Main.tile[x, origin.Y].wire(true);
                    if (x == doorX - 7)
                    {
                        for (int y = origin.Y - 2; y < origin.Y; y++)
                        {
                            Main.tile[x, y].wire(true);
                        }
                    }
                }
            }
            else
            {
                //Lantern, chest, heavy work bench
                WorldGen.Place1x2Top(lanternX, origin.Y - 3, TileID.HangingLanterns, 14);
                WorldGen.Place3x3(doorX + 8, origin.Y + 3, TileID.HeavyWorkBench);
                chest = WorldGen.PlaceChest(doorX + 11, origin.Y + 3);
                //switch
                WorldGen.PlaceTile(doorX + 1, origin.Y, TileID.Switches, true);
                //wire
                for (int x = doorX + 1; x <= doorX + 7; x++)
                {
                    Main.tile[x, origin.Y].wire(true);
                    if (x == doorX + 7)
                    {
                        for (int y = origin.Y - 2; y < origin.Y; y++)
                        {
                            Main.tile[x, y].wire(true);
                        }
                    }
                }
            }
            //Fill new chest with herb bags and planter boxes
            #region Chest shit
            int index = 0;
            Main.chest[chest].item[index].SetDefaults(ItemID.HerbBag);
            Main.chest[chest].item[index++].stack = Main.rand.Next(12, 18);

            Main.chest[chest].item[index].SetDefaults(ItemID.BlinkrootPlanterBox);
            Main.chest[chest].item[index++].stack = Main.rand.Next(5, 10);

            Main.chest[chest].item[index].SetDefaults(ItemID.CorruptPlanterBox);
            Main.chest[chest].item[index++].stack = Main.rand.Next(5, 10);

            Main.chest[chest].item[index].SetDefaults(ItemID.CrimsonPlanterBox);
            Main.chest[chest].item[index++].stack = Main.rand.Next(5, 10);

            Main.chest[chest].item[index].SetDefaults(ItemID.DayBloomPlanterBox);
            Main.chest[chest].item[index++].stack = Main.rand.Next(5, 10);

            Main.chest[chest].item[index].SetDefaults(ItemID.FireBlossomPlanterBox);
            Main.chest[chest].item[index++].stack = Main.rand.Next(5, 10);

            Main.chest[chest].item[index].SetDefaults(ItemID.MoonglowPlanterBox);
            Main.chest[chest].item[index++].stack = Main.rand.Next(5, 10);

            Main.chest[chest].item[index].SetDefaults(ItemID.ShiverthornPlanterBox);
            Main.chest[chest].item[index++].stack = Main.rand.Next(5, 10);

            Main.chest[chest].item[index].SetDefaults(ItemID.WaterleafPlanterBox);
            Main.chest[chest].item[index++].stack = Main.rand.Next(5, 10);

            Mod thorium = ModLoader.GetMod("ThoriumMod");
            if (thorium != null)
			{
				Main.chest[chest].item[index].SetDefaults(thorium.ItemType("MarineKelpPlanterBox"));
				Main.chest[chest].item[index++].stack = Main.rand.Next(5, 10);
			}
            #endregion
            //Turn off the lantern
            for (int y = origin.Y - 3; y <= origin.Y - 2; y++)
            {
                _tiles[lanternX, y].frameX = 18;
            }
            int leftPlats = labLeftSide ? doorX - 5 : doorX + 2;
            int rightPlats = labLeftSide ? doorX - 2 : doorX + 5;
            for (int x = leftPlats; x <= rightPlats; x++)
            {
                for (int y = origin.Y - 1; y <= origin.Y + 1; y += 2)
                {
                    WorldGen.PlaceTile(x, y, TileID.Platforms);
                    if (_random.NextFloat(1f) <= 0.85f)
                    {
                        WorldGen.PlaceTile(x, y - 1, TileID.Bottles, true, false, -1, _random.Next(3));
                    }
                }
            }

            //PLACE ENTRANCE DOOR
            int entranceDoorX = labLeftSide ? origin.X + corridorLength : origin.X - corridorLength;
            for (int i = origin.Y + 1; i <= origin.Y + 3; i++)
            {
                Main.tile[entranceDoorX, i].active(false);
            }
            WorldGen.PlaceDoor(entranceDoorX, origin.Y + 2, TileID.ClosedDoor, 14);

            //PLACE BREAKABLE GRASS AND TREES
            WorldUtils.Gen(origin, new ModShapes.OuterOutline(mainShape, true, true), Actions.Chain(new GenAction[]
            {
                new Modifiers.OnlyTiles(new ushort[] { TileID.Grass }),
                new Modifiers.Offset(0, -1),
                new ActionGrass(),
                new Modifiers.Conditions(new CustomConditions.RandomChance(1.5f)),
                new Modifiers.Offset(0, 1),
                new CustomActions.PlaceTree()
            }));

            //PLACE VINES
            WorldUtils.Gen(origin, new ModShapes.OuterOutline(mainShape, true, true), Actions.Chain(new GenAction[]
            {
                new Modifiers.OnlyTiles(new ushort[] { TileID.Grass }),
                new Modifiers.Conditions(new CustomConditions.RandomChance(2)),
                new Modifiers.Offset(0, 1),
                new CustomActions.RandomFrom(new GenAction[]
                {
                    new ActionVines(7, 12, TileID.Vines),
                    new ActionVines(11, 17, TileID.VineFlowers)
                },
                new float[] { 0.85f, 0.15f })
            }));
        }

        public void PlaceBeams(ushort beamType, int x, int startY, Vector2 planetCenter, int radius, params ushort[] ignoreWalls)
        {
            //lazy quick fix
            List<ushort> walls = ignoreWalls.ToList();
            while (!_tiles[x, startY].active() && !walls.Contains(_tiles[x, startY].wall))
            {
                _tiles[x, startY].active(true);
                _tiles[x, startY].type = beamType;
                startY++;
                if (Vector2.Distance(planetCenter, new Vector2(x * 16 + 8, startY * 16 + 8)) > radius * 16f - 64)
                {
                    _tiles[x, startY].active(true);
                    _tiles[x, startY].type = TileID.Stone;
                }
            }
        }
    }
}
