using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Tiles;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.FurnitureAncient;
using CalamityMod.Tiles.Ores;
using CalamityMod.Walls;
using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;

namespace CalamityMod.World
{
    public class GiantHive
    {
        public static bool GrowLivingJungleTree(Point origin)
        {
            int treeHeight = (int)Main.worldSurface - (Main.maxTilesY / 10); //start here to not touch floating islands
            bool validHeightFound = false;
            int attempts = 0;

            while (!validHeightFound && attempts++ < 100000)
            {
                while (!WorldGen.SolidTile(origin.X, treeHeight))
				{
					treeHeight++;
				}

                if (Main.tile[origin.X, treeHeight].HasTile || Main.tile[origin.X, treeHeight].WallType > 0)
				{
                    validHeightFound = true;
				}
            }

            int extraWidth = 0;
            
            while (validHeightFound)
            {
                //place the roots first so the bottom of the hole doesnt look strange
                for (int k = 0; k < 6; k++)
                {
                    double angle = (double)k / 3.0 * 2.0 + 0.57075;
                    WorldUtils.Gen(new Point(origin.X + 2, origin.Y - 30), new ShapeRoot((int)angle, WorldGen.genRand.Next(80, 120)), 
                    Actions.Chain(new Modifiers.SkipTiles(21, 467, 226, 237), new Modifiers.SkipWalls(87), 
                    new Actions.SetTile(TileID.LivingMahogany, setSelfFrames: true)));
                }

                for (int y = treeHeight - 50; y <= origin.Y - 30; y++)
                {
                    //increase extra width to make the tree wider as it goes down
                    if (y % 45 == 0 && extraWidth < 3)
                    {
                        extraWidth++;
                    }

                    //place branches at the top of the tree
                    if (y <= treeHeight - 45)
                    {
                        //create a list of points for the ends of each branch
                        List<Point> list = new List<Point>();

                        WorldUtils.Gen(new Point(origin.X + WorldGen.genRand.Next(-3, 3), y), new ShapeBranch(-0.6853981852531433, 
                        WorldGen.genRand.Next(5, 25)).OutputEndpoints(list), Actions.Chain(new Modifiers.SkipTiles(21, 467, 226, 237), 
                        new Modifiers.SkipWalls(87), new Actions.SetTile(TileID.LivingMahogany), new Actions.SetFrames(frameNeighbors: true)));

                        WorldUtils.Gen(new Point(origin.X + WorldGen.genRand.Next(-3, 3), y), new ShapeBranch(-2.45619455575943, 
                        WorldGen.genRand.Next(5, 25)).OutputEndpoints(list), Actions.Chain(new Modifiers.SkipTiles(21, 467, 226, 237), 
                        new Modifiers.SkipWalls(87), new Actions.SetTile(TileID.LivingMahogany), new Actions.SetFrames(frameNeighbors: true)));

                        //place living mahogany leaves at the end of each branch
                        foreach (Point item in list)
                        {
                            WorldUtils.Gen(item, new Shapes.Circle(WorldGen.genRand.Next(5, 12)), Actions.Chain(new Modifiers.Blotches(WorldGen.genRand.Next(3, 5), WorldGen.genRand.Next(3, 5)), 
                            new Modifiers.SkipTiles(383, 21, 467, 226, 237), new Modifiers.SkipWalls(78, 87), new Actions.SetTile(TileID.LivingMahoganyLeaves), new Actions.SetFrames(frameNeighbors: true)));
                        }
                    }

                    //place the tree itself
                    WorldUtils.Gen(new Point(origin.X + WorldGen.genRand.Next(-1 - extraWidth, 1), y), new Shapes.Rectangle(6 + extraWidth, 3 + extraWidth), 
                    Actions.Chain(new Modifiers.SkipTiles(21, 467, 226, 237), new Modifiers.SkipWalls(87), new Actions.RemoveWall(),
                    new Actions.SetTile(TileID.LivingMahogany), new Actions.SetFrames()));

                    WorldUtils.Gen(new Point(origin.X + WorldGen.genRand.Next(-1, 1), y), new Shapes.Rectangle(6 + extraWidth, 3 + extraWidth), 
                    Actions.Chain(new Modifiers.SkipTiles(21, 467, 226, 237), new Modifiers.SkipWalls(87), new Actions.RemoveWall(),
                    new Actions.SetTile(TileID.LivingMahogany), new Actions.SetFrames()));

                    //dig hole in tree
                    ShapeData circle = new ShapeData();
                    ShapeData biggerCircle = new ShapeData();
                    GenAction blotchMod = new Modifiers.Blotches(2, 0.4);

                    int radius = extraWidth - 1;

                    WorldUtils.Gen(new Point(origin.X + 2, y), new Shapes.Circle(radius), Actions.Chain(new GenAction[]
                    {
                        blotchMod.Output(circle)
                    }));
                    WorldUtils.Gen(new Point(origin.X + 2, y), new Shapes.Circle(radius + 1), Actions.Chain(new GenAction[]
                    {
                        blotchMod.Output(biggerCircle)
                    }));

                    //main circle of walls, also dig out hole in the tree
                    WorldUtils.Gen(new Point(origin.X + 2, y), new ModShapes.All(circle), Actions.Chain(new GenAction[]
                    {
                        new Actions.ClearTile(),
                        new Actions.PlaceWall(WallID.LivingWood)
                    }));

                    //chance to place blocks inside the tree for randomness
                    if (WorldGen.genRand.Next(20) == 0)
                    {
                        WorldUtils.Gen(new Point(origin.X + WorldGen.genRand.Next(-2, 2), y), new ModShapes.All(circle), Actions.Chain(new GenAction[]
                        {
                            new Actions.ClearTile(),
                            new Actions.PlaceTile(TileID.LivingMahogany)
                        }));
                    }

                    //bigger circle so that walls can place behind blocks (basically makes it not look ugly in game)
                    WorldUtils.Gen(new Point(origin.X + 2, y), new ModShapes.All(biggerCircle), Actions.Chain(new GenAction[]
                    {
                        new Actions.PlaceWall(WallID.LivingWood)
                    }));
                }

                return true;
            }

            return false;
        }
        
        public static bool CanPlaceGiantHive(Point origin, StructureMap structures)
        {
            if (!structures.CanPlace(new Rectangle(origin.X - 50, origin.Y - 50, 100, 100)))
                return false;

            if (TooCloseToImportantLocations(origin))
                return false;

            Ref<int> ref1 = new Ref<int>(0);
            Ref<int> ref2 = new Ref<int>(0);
            Ref<int> ref3 = new Ref<int>(0);
            WorldUtils.Gen(origin, new Shapes.Circle(15), Actions.Chain(new Modifiers.IsSolid(), new Actions.Scanner(ref1), new Modifiers.OnlyTiles(TileID.JungleGrass, TileID.Mud), new Actions.Scanner(ref2), new Modifiers.OnlyTiles(TileID.JungleGrass), new Actions.Scanner(ref3)));
            if (ref2.Value / (float)ref1.Value < 0.75f || ref3.Value < 2)
                return false;

            int arrayInc = 0;
            int[] array = new int[1000];
            int[] array2 = new int[1000];
            Vector2 larvaLocation = origin.ToVector2();
            int numHiveTunnels = WorldGen.genRand.Next(10, 13);

            for (int i = 0; i < numHiveTunnels; i++)
            {
                Vector2 movingLarvaLocation = larvaLocation;
                int hiveTunnelTries = WorldGen.genRand.Next(2, 5);
                for (int j = 0; j < hiveTunnelTries; j++)
                    movingLarvaLocation = CreateHiveTunnel((int)larvaLocation.X, (int)larvaLocation.Y, WorldGen.genRand);

                larvaLocation = movingLarvaLocation;
                array[arrayInc] = (int)larvaLocation.X;
                array2[arrayInc] = (int)larvaLocation.Y;
                arrayInc++;
            }

            FrameOutAllHiveContents(origin, 50);

            for (int k = 0; k < arrayInc; k++)
            {
                int x = array[k];
                int y = array2[k];
                int treeIndentDirection = 1;
                if (WorldGen.genRand.NextBool())
                    treeIndentDirection = -1;

                bool flag = false;
                while (WorldGen.InWorld(x, y, 10) && BadSpotForHoneyFall(x, y))
                {
                    x += treeIndentDirection;
                    if (Math.Abs(x - array[k]) > 50)
                    {
                        flag = true;
                        break;
                    }
                }

                if (!flag)
                {
                    x += treeIndentDirection;
                    if (!SpotActuallyNotInHive(x, y))
                    {
                        CreateBlockedHoneyCube(x, y);
                        CreateDentForHoneyFall(x, y, treeIndentDirection);
                    }
                }
            }

            GrowLivingJungleTree(new Point(origin.X, origin.Y));

            CreateStandForLarva(larvaLocation);

            // Generate a second larva stand
            Vector2 secondLarvaLocation = default;
            int maxAttempts = 1000;
            for (int l = 0; l < maxAttempts; l++)
            {
                Vector2 newStructureLocation = larvaLocation;
                newStructureLocation.X += WorldGen.genRand.Next(-50, 51);
                newStructureLocation.Y += WorldGen.genRand.Next(-50, 51);
                if (WorldGen.InWorld((int)newStructureLocation.X, (int)newStructureLocation.Y) && Vector2.Distance(larvaLocation, newStructureLocation) > 10f && !Main.tile[(int)newStructureLocation.X, (int)newStructureLocation.Y].HasTile && Main.tile[(int)newStructureLocation.X, (int)newStructureLocation.Y].WallType == WallID.HiveUnsafe)
                {
                    secondLarvaLocation = newStructureLocation;
                    CreateStandForLarva(newStructureLocation);
                    break;
                }
            }

            // Honey chests
            Vector2 honeyChestLocation = default;
            for (int l = 0; l < maxAttempts; l++)
            {
                Vector2 newStructureLocation = larvaLocation;
                newStructureLocation.X += WorldGen.genRand.Next(-100, 101);
                newStructureLocation.Y += WorldGen.genRand.Next(-100, 101);
                if (WorldGen.InWorld((int)newStructureLocation.X, (int)newStructureLocation.Y) && Vector2.Distance(larvaLocation, newStructureLocation) > 10f && Vector2.Distance(secondLarvaLocation, newStructureLocation) > 10f && !Main.tile[(int)newStructureLocation.X, (int)newStructureLocation.Y].HasTile && Main.tile[(int)newStructureLocation.X, (int)newStructureLocation.Y].WallType == WallID.HiveUnsafe)
                {
                    honeyChestLocation = newStructureLocation;
                    CreateStandAndPlaceHoneyChest(newStructureLocation);
                    break;
                }
            }
            for (int l = 0; l < maxAttempts; l++)
            {
                Vector2 newStructureLocation = larvaLocation;
                newStructureLocation.X += WorldGen.genRand.Next(-100, 101);
                newStructureLocation.Y += WorldGen.genRand.Next(-100, 101);
                if (WorldGen.InWorld((int)newStructureLocation.X, (int)newStructureLocation.Y) && Vector2.Distance(larvaLocation, newStructureLocation) > 10f && Vector2.Distance(secondLarvaLocation, newStructureLocation) > 10f && Vector2.Distance(honeyChestLocation, newStructureLocation) > 10f && !Main.tile[(int)newStructureLocation.X, (int)newStructureLocation.Y].HasTile && Main.tile[(int)newStructureLocation.X, (int)newStructureLocation.Y].WallType == WallID.HiveUnsafe)
                {
                    CreateStandAndPlaceHoneyChest(newStructureLocation);
                    break;
                }
            }

            CalamityUtils.AddProtectedStructure(new Rectangle(origin.X - 50, origin.Y - 50, 100, 100), 5);
            return true;
        }

        private static void FrameOutAllHiveContents(Point origin, int squareHalfWidth)
        {
            int maxXsize = Math.Max(10, origin.X - squareHalfWidth);
            int minXsize = Math.Min(Main.maxTilesX - 10, origin.X + squareHalfWidth);
            int maxYSize = Math.Max(10, origin.Y - squareHalfWidth);
            int minYSize = Math.Min(Main.maxTilesY - 10, origin.Y + squareHalfWidth);
            for (int i = maxXsize; i < minXsize; i++)
            {
                for (int j = maxYSize; j < minYSize; j++)
                {
                    Tile tile = Main.tile[i, j];
                    if (tile.HasTile && tile.TileType == TileID.Hive)
                        WorldGen.SquareTileFrame(i, j);

                    if (tile.WallType == WallID.HiveUnsafe)
                        WorldGen.SquareWallFrame(i, j);
                }
            }
        }

        private static Vector2 CreateHiveTunnel(int i, int j, UnifiedRandom random)
        {
            double randOffset = random.Next(20, 26);
            float tunnelPlacementAttempts = random.Next(30, 41);
            float miniMaxXTiles = Main.maxTilesX / 4200;
            miniMaxXTiles = (miniMaxXTiles + 1f) / 2f;
            randOffset *= (double)miniMaxXTiles;
            tunnelPlacementAttempts *= miniMaxXTiles;

            double randOffsetCopy = randOffset;
            Vector2 result = default;
            result.X = i;
            result.Y = j;
            Vector2 defaultTunnelPos = default;
            defaultTunnelPos.X = random.Next(-10, 11) * 0.2f;
            defaultTunnelPos.Y = random.Next(-10, 11) * 0.2f;
            while (randOffset > 0.0 && tunnelPlacementAttempts > 0f)
            {
                if (result.Y > (Main.maxTilesY - 250))
                    tunnelPlacementAttempts = 0f;

                randOffset = randOffsetCopy * (double)(1f + random.Next(-20, 20) * 0.01f);
                tunnelPlacementAttempts -= 1f;
                int mainXOffset = (int)(result.X - randOffset);
                int maxXOffset = (int)(result.X + randOffset);
                int minYOffset = (int)(result.Y - randOffset);
                int maxYOffset = (int)(result.Y + randOffset);
                if (mainXOffset < 1)
                    mainXOffset = 1;

                if (maxXOffset > Main.maxTilesX - 1)
                    maxXOffset = Main.maxTilesX - 1;

                if (minYOffset < 1)
                    minYOffset = 1;

                if (maxYOffset > Main.maxTilesY - 1)
                    maxYOffset = Main.maxTilesY - 1;

                for (int k = mainXOffset; k < maxXOffset; k++)
                {
                    for (int l = minYOffset; l < maxYOffset; l++)
                    {
                        if (!WorldGen.InWorld(k, l, 50))
                        {
                            tunnelPlacementAttempts = 0f;
                        }
                        else
                        {
                            if (Main.tile[k - 10, l].WallType == WallID.LihzahrdBrickUnsafe)
                                tunnelPlacementAttempts = 0f;

                            if (Main.tile[k + 10, l].WallType == WallID.LihzahrdBrickUnsafe)
                                tunnelPlacementAttempts = 0f;

                            if (Main.tile[k, l - 10].WallType == WallID.LihzahrdBrickUnsafe)
                                tunnelPlacementAttempts = 0f;

                            if (Main.tile[k, l + 10].WallType == WallID.LihzahrdBrickUnsafe)
                                tunnelPlacementAttempts = 0f;
                        }

                        if (l < Main.worldSurface && Main.tile[k, l - 5].WallType == WallID.None)
                            tunnelPlacementAttempts = 0f;

                        float tileXDist = Math.Abs(k - result.X);
                        float tileYDist = Math.Abs(l - result.Y);
                        double tileDistance = Math.Sqrt(tileXDist * tileXDist + tileYDist * tileYDist);
                        if (tileDistance < randOffsetCopy * 0.4 * (1.0 + random.Next(-10, 11) * 0.005))
                        {
                            Main.tile[k, l].LiquidAmount = byte.MaxValue;
                            Main.tile[k, l].Get<LiquidData>().LiquidType = LiquidID.Honey;
                            Main.tile[k, l].WallType = WallID.HiveUnsafe;
                            Main.tile[k, l].Get<TileWallWireStateData>().HasTile = false;
                            Main.tile[k, l].Get<TileWallWireStateData>().IsHalfBlock = false;
                            Main.tile[k, l].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                        }
                        else if (tileDistance < randOffsetCopy * 0.75 * (1.0 + random.Next(-10, 11) * 0.005))
                        {
                            Main.tile[k, l].LiquidAmount = 0;
                            if (Main.tile[k, l].WallType != WallID.HiveUnsafe)
                            {
                                Main.tile[k, l].Get<TileWallWireStateData>().HasTile = true;
                                Main.tile[k, l].Get<TileWallWireStateData>().IsHalfBlock = false;
                                Main.tile[k, l].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                                Main.tile[k, l].TileType = TileID.Hive;
                            }
                        }

                        if (tileDistance < randOffsetCopy * 0.6 * (1.0 + random.Next(-10, 11) * 0.005))
                        {
                            Main.tile[k, l].WallType = WallID.HiveUnsafe;
                            if (random.NextBool())
                            {
                                Main.tile[k, l].LiquidAmount = byte.MaxValue;
                                Main.tile[k, l].Get<LiquidData>().LiquidType = LiquidID.Honey;
                            }
                        }
                    }
                }

                result += defaultTunnelPos;
                tunnelPlacementAttempts -= 1f;
                defaultTunnelPos.Y += random.Next(-10, 11) * 0.05f;
                defaultTunnelPos.X += random.Next(-10, 11) * 0.05f;
            }

            return result;
        }

        private static bool TooCloseToImportantLocations(Point origin)
        {
            int x = origin.X;
            int y = origin.Y;
            int checkRadius = 150;
            for (int i = x - checkRadius; i < x + checkRadius; i += 10)
            {
                if (i <= 0 || i > Main.maxTilesX - 1)
                    continue;

                for (int j = y - checkRadius; j < y + checkRadius; j += 10)
                {
                    if (j > 0 && j <= Main.maxTilesY - 1)
                    {
                        if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.LihzahrdBrick)
                            return true;

                        if (Main.tile[i, j].WallType == WallID.CrimstoneUnsafe || Main.tile[i, j].WallType == WallID.EbonstoneUnsafe || Main.tile[i, j].WallType == WallID.LihzahrdBrickUnsafe)
                            return true;
                        if (Main.tile[i,j].LiquidAmount != 0 && Main.tile[i, j].LiquidType == LiquidID.Shimmer)
                            return true;
                    }
                }
            }

            return false;
        }

        private static void CreateDentForHoneyFall(int x, int y, int dir)
        {
            dir *= -1;
            y++;
            int honeyDentTries = 0;
            while ((honeyDentTries < 4 || WorldGen.SolidTile(x, y)) && x > 10 && x < Main.maxTilesX - 10)
            {
                honeyDentTries++;
                x += dir;
                if (WorldGen.SolidTile(x, y))
                {
                    WorldGen.PoundTile(x, y);
                    if (!Main.tile[x, y + 1].HasTile)
                    {
                        Main.tile[x, y + 1].Get<TileWallWireStateData>().HasTile = true;
                        Main.tile[x, y + 1].TileType = TileID.Hive;
                    }
                }
            }
        }

        private static void CreateBlockedHoneyCube(int x, int y)
        {
            for (int i = x - 1; i <= x + 2; i++)
            {
                for (int j = y - 1; j <= y + 2; j++)
                {
                    if (i >= x && i <= x + 1 && j >= y && j <= y + 1)
                    {
                        Main.tile[i, j].Get<TileWallWireStateData>().HasTile = false;
                        Main.tile[i, j].LiquidAmount = byte.MaxValue;
                        Main.tile[i, j].Get<LiquidData>().LiquidType = LiquidID.Honey;
                    }
                    else
                    {
                        Main.tile[i, j].Get<TileWallWireStateData>().HasTile = true;
                        Main.tile[i, j].TileType = TileID.Hive;
                    }
                }
            }
        }

        private static bool SpotActuallyNotInHive(int x, int y)
        {
            for (int i = x - 1; i <= x + 2; i++)
            {
                for (int j = y - 1; j <= y + 2; j++)
                {
                    if (i < 10 || i > Main.maxTilesX - 10)
                        return true;

                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType != TileID.Hive)
                        return true;
                }
            }

            return false;
        }

        private static bool BadSpotForHoneyFall(int x, int y)
        {
            if (Main.tile[x, y].HasTile && Main.tile[x, y + 1].HasTile && Main.tile[x + 1, y].HasTile)
                return !Main.tile[x + 1, y + 1].HasTile;

            return true;
        }

        public static void CreateStandForLarva(Vector2 position)
        {
            GenVars.larvaX[GenVars.numLarva] = Utils.Clamp((int)position.X, 5, Main.maxTilesX - 5);
            GenVars.larvaY[GenVars.numLarva] = Utils.Clamp((int)position.Y, 5, Main.maxTilesY - 5);
            GenVars.numLarva++;
            if (GenVars.numLarva >= GenVars.larvaX.Length)
                GenVars.numLarva = GenVars.larvaX.Length - 1;

            int larvaX = (int)position.X;
            int larvaY = (int)position.Y;
            for (int i = larvaX - 1; i <= larvaX + 1 && i > 0 && i < Main.maxTilesX; i++)
            {
                for (int j = larvaY - 2; j <= larvaY + 1 && j > 0 && j < Main.maxTilesY; j++)
                {
                    if (j != larvaY + 1)
                    {
                        Main.tile[i, j].Get<TileWallWireStateData>().HasTile = false;
                        continue;
                    }

                    Main.tile[i, j].Get<TileWallWireStateData>().HasTile = true;
                    Main.tile[i, j].TileType = TileID.Hive;
                    Main.tile[i, j].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                    Main.tile[i, j].Get<TileWallWireStateData>().IsHalfBlock = false;
                }
            }
        }

        public static void CreateStandAndPlaceHoneyChest(Vector2 position)
        {
            int chestPlacementX = Utils.Clamp((int)position.X, 5, Main.maxTilesX - 5);
            int chestPlacementY = Utils.Clamp((int)position.Y, 5, Main.maxTilesY - 5);

            int chestX = (int)position.X;
            int chestY = (int)position.Y;
            for (int i = chestX; i <= chestX + 1 && i > 0 && i < Main.maxTilesX; i++)
            {
                for (int j = chestY - 1; j <= chestY + 1 && j > 0 && j < Main.maxTilesY; j++)
                {
                    if (j != chestY + 1)
                    {
                        Main.tile[i, j].Get<TileWallWireStateData>().HasTile = false;
                        continue;
                    }

                    Main.tile[i, j].Get<TileWallWireStateData>().HasTile = true;
                    Main.tile[i, j].TileType = TileID.Hive;
                    Main.tile[i, j].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                    Main.tile[i, j].Get<TileWallWireStateData>().IsHalfBlock = false;
                }
            }

            int finalChestX = chestPlacementX;
            int finalChestY = chestPlacementY;
            for (int i = finalChestX; i <= finalChestX + 1; i++)
            {
                for (int j = finalChestY - 1; j <= finalChestY + 1; j++)
                {
                    if (j != finalChestY + 1)
                    {
                        Main.tile[i, j].Get<TileWallWireStateData>().HasTile = false;
                    }
                    else
                    {
                        Main.tile[i, j].Get<TileWallWireStateData>().HasTile = true;
                        Main.tile[i, j].TileType = TileID.Hive;
                        Main.tile[i, j].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                        Main.tile[i, j].Get<TileWallWireStateData>().IsHalfBlock = false;
                    }
                }
            }

            int chestID = WorldGen.PlaceChest(finalChestX, finalChestY, 21, false, 29);
            FillHoneyChest(chestID, WorldGen.genRand);
        }

        // Honey chest stuff, this is also used by the Hive Planetoid
        private static int[] FocusLootHoney = new int[]
        {
            ItemID.NaturesGift,
            ItemID.Bezoar,
            ItemID.FlowerBoots,
            ItemID.BeeMinecart
        };

        private static int[] PotionLootHoney = new int[]
        {
            ItemID.LifeforcePotion,
            ItemID.RegenerationPotion,
            ItemID.ManaRegenerationPotion,
            ItemID.HeartreachPotion,
            ModContent.ItemType<PhotosynthesisPotion>()
        };

        private static int[] BarLootHoney = new int[]
        {
            GenVars.silverBar == TileID.Silver ? ItemID.SilverBar : ItemID.TungstenBar,
            GenVars.goldBar == TileID.Gold ? ItemID.GoldBar : ItemID.PlatinumBar
        };

        public static void FillHoneyChest(int id, UnifiedRandom random)
        {
            if (id < 0)
                return;

            Chest chest = Main.chest[id];
            if (chest == null)
                return;

            int index = 0;

            chest.item[index++].SetDefaults(random.Next(FocusLootHoney));

            if (random.Next(3) <= 1)
            {
                chest.item[index].SetDefaults(random.Next(BarLootHoney));
                chest.item[index].SetDefaults(random.Next(7, 15));
            }
            else
            {
                chest.item[index].SetDefaults(ItemID.GoldCoin);
                chest.item[index++].stack = random.Next(3, 5);
            }

            if (random.NextBool())
            {
                chest.item[index].SetDefaults(random.Next(PotionLootHoney));
                chest.item[index++].stack = random.Next(1, 4);
            }
            else
            {
                chest.item[index].SetDefaults(ItemID.BottledHoney);
                chest.item[index++].stack = random.Next(3, 7);
            }

            if (random.NextBool())
            {
                chest.item[index].SetDefaults(ItemID.Stinger);
                chest.item[index++].stack = random.Next(4, 6);
            }
            else
            {
                chest.item[index].SetDefaults(ItemID.JungleSpores);
                chest.item[index++].stack = random.Next(3, 5);
            }

            if (random.NextBool())
            {
                chest.item[index].SetDefaults(ItemID.RecallPotion);
                chest.item[index++].stack = random.Next(1, 4);
            }
            else
            {
                chest.item[index].SetDefaults(ItemID.JungleTorch);
                chest.item[index++].stack = random.Next(18, 36);
            }
        }
    }
}
