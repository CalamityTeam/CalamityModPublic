using CalamityMod.DataStructures;
using CalamityMod.Schematics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.WorldBuilding;

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

            // TODO -- This should probably use PlacementAnchor.Center and just provide the center of the planetoid...
            bool hasPlacedLogAndSchematic = false;
            SchematicManager.PlaceSchematic("Planetoid Laboratory", new Point(origin.X - 33, origin.Y - 17), SchematicAnchor.TopLeft, ref hasPlacedLogAndSchematic, new Action<Chest, int, bool>(DraedonStructures.FillPlanetoidLaboratoryChest));
            CalamityWorld.PlanetoidLabCenter = origin.ToWorldCoordinates();

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
