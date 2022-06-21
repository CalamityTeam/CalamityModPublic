using CalamityMod.DataStructures;
using CalamityMod.Tiles.Ores;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityMod.World.Planets
{
    /// <summary>
    /// Comfy planet, the typical one you'd expect from planetoids.
    /// Has the opposite ore from the world. Has a chance to spawn with things like a campsite on top ( for adventurous campers 0_0 )
    /// Has trees, flowers
    /// </summary>
    public class GrassPlanet : Planetoid
    {
        private ushort[] oreTypes = new ushort[]
        {
            WorldGen.copperBar == TileID.Copper ? (Main.getGoodWorld ? TileID.Palladium : TileID.Tin) : (Main.getGoodWorld ? TileID.Cobalt : TileID.Copper),
            WorldGen.ironBar == TileID.Iron ? (Main.getGoodWorld ? TileID.Palladium : TileID.Lead) : (Main.getGoodWorld ? TileID.Cobalt : TileID.Iron),
            WorldGen.silverBar == TileID.Silver ? (Main.getGoodWorld ? TileID.Orichalcum : TileID.Tungsten) : (Main.getGoodWorld ? TileID.Mythril : TileID.Silver),
            WorldGen.goldBar == TileID.Gold ? (Main.getGoodWorld ? TileID.Titanium : TileID.Platinum) : (Main.getGoodWorld ? TileID.Adamantite : TileID.Gold)
        };

        public override bool Place(Point origin, StructureMap structures)
        {
            int radius = _random.Next(16, 24);

            if (!CheckIfPlaceable(origin, radius, structures))
            {
                return false;
            }

            PlacePlanet(origin, radius, _random.Next(oreTypes));

            return base.Place(origin, structures);
        }

        public void PlacePlanet(Point origin, int radius, ushort oreType)
        {
            Circle planetoid = new Circle(origin.ToVector2() * 16f + new Vector2(8f), radius * 16f);

            ShapeData crust = new ShapeData();
            ShapeData core = new ShapeData();

            int outerRadius = (int)(radius * WorldGen.genRand.NextFloat(0.74f, 0.82f));

            //Create shapes for each layer
            GenAction blotchMod = new Modifiers.Blotches(2, 0.4);
            WorldUtils.Gen(origin, new Shapes.Circle(radius), Actions.Chain(new GenAction[]
            {
                blotchMod.Output(crust)
            }));
            WorldUtils.Gen(origin, new Shapes.Circle(outerRadius), Actions.Chain(new GenAction[]
            {
                blotchMod.Output(core)
            }));

            //Subtract the inner shapes from outer ones
            crust.Subtract(core, origin, origin);

            //Place layers
            WorldUtils.Gen(origin, new ModShapes.All(core), Actions.Chain(new GenAction[]
            {
                new Actions.PlaceTile(Main.getGoodWorld ? TileID.WoodenSpikes : TileID.Stone),
                new Actions.PlaceWall(WallID.Cave2Unsafe)
            }));
            WorldUtils.Gen(origin, new ModShapes.All(crust), Actions.Chain(new GenAction[]
            {
                new Actions.PlaceTile(TileID.Dirt),
                new Actions.PlaceWall(WallID.DirtUnsafe)
            }));

            //Place random dirt/stone
            int randDirt = (int)(radius * 0.4f);
            int randStone = (int)(randDirt * 0.6f);
            for (int i = 0; i < randStone; i++)
            {
                int x = origin.X;
                int y = origin.Y;
                while (Vector2.Distance(origin.ToVector2(), new Vector2(x, y)) < outerRadius)
                {
                    x = _random.Next(origin.X - radius, origin.X + radius + 1);
                    y = _random.Next(origin.Y - radius, origin.Y + radius + 1);
                }
                WorldGen.TileRunner(x, y, _random.NextFloat(4.6f, 7.6f), _random.Next(7, 16), Main.getGoodWorld ? TileID.WoodenSpikes : TileID.Stone);
            }
            for (int i = 0; i < randDirt; i++)
            {
                int x = _random.Next(origin.X - outerRadius, origin.X + outerRadius + 1);
                int y = _random.Next(origin.Y - outerRadius, origin.Y + outerRadius + 1);

                WorldGen.TileRunner(x, y, _random.NextFloat(5f, 8f), _random.Next(8, 18), TileID.Dirt);
            }

            //Bezier Curves for placing ore
            int numStrokes = radius > 20 ? 3 : 2;
            for (int i = 0; i < numStrokes; i++)
            {
                //GET POINTS FOR CURVE
                Vector2 start = planetoid.RandomPointOnCircleEdge();
                //end is opposite side
                Vector2 end = planetoid.Center - (start - planetoid.Center);
                Vector2 control = planetoid.RandomPointOnCircleEdge();
                float min = radius * 0.6f * 16f;
                while (Vector2.Distance(control, start) < min || Vector2.Distance(control, end) < min)
                {
                    control = planetoid.RandomPointOnCircleEdge();
                }

                BezierCurve curve = new BezierCurve(start, control, end);

                int strokeSteps = 50;
                double baseStrength = (double)_random.NextFloat(1.2f, 2.4f);

                List<Vector2> tilePoints = curve.GetPoints(strokeSteps);
                for (int k = 0; k < strokeSteps; k++)
                {
                    float progress = k / (float)strokeSteps * (float)Math.PI;
                    double strengthMultiplier = 1.0 + Math.Sin(progress) * baseStrength * 1.1f;

                    Vector2 nextPoint = tilePoints[k];
                    int x = (int)(nextPoint.X / 16f);
                    int y = (int)(nextPoint.Y / 16f);

                    WorldGen.OreRunner(x, y, baseStrength * strengthMultiplier, 1, oreType);
                }
            }

            //Boffin's funtime 2 -- Small shrine
            #region Boffin's funtime 2
            bool boffsFuntime2 = _random.Next(10) == 0;
            if (boffsFuntime2)
            {
                int topLayer = origin.Y - radius - 4;
                for (; topLayer < origin.Y; topLayer++)
                {
                    for (int x = origin.X - radius; x < origin.X + radius; x++)
                    {
                        if (_tiles[x, topLayer].HasTile && Main.tileSolid[_tiles[x, topLayer].TileType])
                        {
                            //We've found the highest tile
                            goto TopLayerFound;
                        }
                    }
                }
                TopLayerFound:

                ushort brickType = TileID.StoneSlab;
                int shrineDepth = _random.Next(8, 13);
                int startX = origin.X - 3;
                int endX = origin.X + 3;
                for (int y = topLayer; y <= topLayer + shrineDepth; y++)
                {
                    for (int x = startX; x <= endX; x++)
                    {
                        _tiles[x, y].TileType = brickType;
                        _tiles[x, y].Get<TileWallWireStateData>().HasTile = true;
                        if (y == topLayer)
                        {
                            _tiles[x, y].WallType = WallID.None;
                        }
                    }
                    if ((y - topLayer + 1) % 2 == 0)
                    {
                        startX--;
                        endX++;
                    }
                }
                WorldGen.PlaceTile(origin.X, topLayer - 1, 187, true, false, -1, 17);
                WorldGen.PlaceTile(origin.X - 3, topLayer - 1, 93, true, false, -1, 14);
                WorldGen.PlaceTile(origin.X + 3, topLayer - 1, 93, true, false, -1, 14);
            }
            #endregion

            //Place grass
            WorldUtils.Gen(origin, new ModShapes.InnerOutline(crust), Actions.Chain(new GenAction[]
            {
                new Modifiers.OnlyTiles(new ushort[] { TileID.Dirt }),
                new Modifiers.IsTouchingAir(true),
                new Actions.SetTile(TileID.Grass, false, true),
                new Actions.ClearWall(false),
                new Actions.SetFrames(true),
                new Modifiers.Conditions(new CustomConditions.RandomChance(4)),
                new Actions.Smooth(),
                new Actions.SetFrames(true)
            }));
            //Clear dirt walls on outer edge because of stone / ore
            WorldUtils.Gen(origin, new ModShapes.InnerOutline(crust), Actions.Chain(new GenAction[]
            {
                new Modifiers.OnlyTiles(new ushort[] { Main.getGoodWorld ? TileID.WoodenSpikes : TileID.Stone, oreType }),
                new Modifiers.IsTouchingAir(true),
                new Modifiers.OnlyWalls(WallID.DirtUnsafe),
                new Actions.ClearWall(true)
            }));

            //Boffin's funtime 1 -- Campsite if possible
            #region Boffin's funtime 1
            if (!boffsFuntime2)
            {
                int campX = origin.X;
                int campY = origin.Y - radius - 3;
                while (!Main.tile[campX, campY].HasTile || !Main.tileSolid[_tiles[campX, campY].TileType])
                {
                    campY++;
                }
                campY--;

                int startCampX = campX;
                while ((!_tiles[startCampX, campY].HasTile || !Main.tileSolid[_tiles[startCampX, campY].TileType]) &&
                    _tiles[startCampX, campY + 1].HasTile)
                {
                    startCampX--;
                }
                int endCampX = campX;
                while ((!_tiles[endCampX, campY].HasTile || !Main.tileSolid[_tiles[endCampX, campY].TileType]) &&
                    _tiles[endCampX, campY + 1].HasTile)
                {
                    endCampX++;
                }
                //if enough room for tent and campfire
                int distance = (int)MathHelper.Distance(++startCampX, --endCampX);

                if (distance >= 8)
                {
                    int extra = distance - 8;
                    if (extra > 0)
                    {
                        extra = _random.Next(extra + 1);
                    }

                    bool tentOnLeft = _random.NextBool();
                    if (tentOnLeft)
                    {
                        WorldGen.Place3x2(startCampX + 1 + extra, campY, TileID.LargePiles2, 26);
                        WorldGen.Place3x2(startCampX + 5 + extra, campY, TileID.Campfire);
                    }
                    else
                    {
                        WorldGen.Place3x2(endCampX - 1 - extra, campY, TileID.LargePiles2, 26);
                        WorldGen.Place3x2(endCampX - 5 - extra, campY, TileID.Campfire);
                    }
                }
            }
            #endregion

            //Place breakable grass and trees
            WorldUtils.Gen(origin, new ModShapes.All(crust), Actions.Chain(new GenAction[]
            {
                new Modifiers.OnlyTiles(new ushort[] { TileID.Grass }),
                new Modifiers.Offset(0, -1),
                new ActionGrass(),
                new Modifiers.Conditions(new CustomConditions.RandomChance(2.5f)),
                new Modifiers.Offset(0, 1),
                new CustomActions.PlaceTree()
            }));

            //Place vines
            WorldUtils.Gen(origin, new ModShapes.All(crust), Actions.Chain(new GenAction[]
            {
                new Modifiers.OnlyTiles(new ushort[] { TileID.Grass }),
                new Modifiers.Conditions(new CustomConditions.RandomChance(2)),
                new Modifiers.Offset(0, 1),
                new CustomActions.RandomFrom(new GenAction[]
                {
                    new ActionVines(3, 7, TileID.Vines),
                    new ActionVines(7, 12, TileID.VineFlowers)
                },
                new float[] { 0.85f, 0.15f })
            }));
        }
    }
}
