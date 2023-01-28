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
using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using Terraria.GameContent.Generation;

namespace CalamityMod.World
{
    public class GiantHive
    {
        public static bool GrowLivingJungleTree(Point origin)
        {
            if (!WorldGen.drunkWorldGen || WorldGen.genRand.Next(50) > 0)
            {
                Dictionary<ushort, int> dictionary = new Dictionary<ushort, int>();
                WorldUtils.Gen(new Point(origin.X - 25, origin.Y - 25), new Shapes.Rectangle(50, 50), new Actions.TileScanner(0, 59, 60, 147, 1).Output(dictionary));
                int num = dictionary[0] + dictionary[1];
                int num2 = dictionary[59] + dictionary[60];
                if (dictionary[147] > num2 || num > num2 || num2 < 50)
                {
                    return false;
                }
            }

            int treeHeight = (int)Main.worldSurface - (Main.maxTilesY / 8); //start here to not touch floating islands
            bool validheightFound = false;
            int attempts = 0;
            while (!validheightFound && attempts++ < 100000)
            {

                while (!WorldGen.SolidTile(origin.X, treeHeight) && treeHeight <= Main.worldSurface)
				{
					treeHeight++;
				}

                if (Main.tile[origin.X, treeHeight].HasTile || Main.tile[origin.X, treeHeight].WallType > 0)
				{
                    validheightFound = true;
				}
            }

            while (validheightFound)
            {
                int num3 = (origin.Y + treeHeight) / 23; //the height of the tree
                int num4 = num3 * 5;
                int num5 = 0;
                double num6 = WorldGen.genRand.NextDouble() + 1.0;
                double num7 = WorldGen.genRand.NextDouble() + 2.0;

                if (WorldGen.genRand.Next(2) == 0)
                {
                    num7 = 0.0 - num7;
                }

                for (int i = 0; i < num3; i++)
                {
                    int num8 = (int)(Math.Sin((double)(i + 1) / 12.0 * num6 * 3.1415927410125732) * num7);
                    int num9 = ((num8 < num5) ? (num8 - num5) : 0);

                    //places the actual log of the tree
                    //i genuinely cannot figure out how to make this get thicker as it goes down, im giving up on that for now
                    WorldUtils.Gen(new Point(origin.X + num5 + num9, origin.Y - (i + 1) * 5), new Shapes.Rectangle(6 + Math.Abs(num8 - num5), 7), 
                    Actions.Chain(new Modifiers.SkipTiles(21, 467, 226, 237), new Modifiers.SkipWalls(87), new Actions.RemoveWall(), 
                    new Actions.SetTile(383), new Actions.SetFrames()));

                    //digs out the hole on the middle of the tree
                    WorldUtils.Gen(new Point(origin.X + num5 + num9 + 2, origin.Y - (i + 1) * 5), new Shapes.Rectangle(2 + Math.Abs(num8 - num5), 5), 
                    Actions.Chain(new Modifiers.SkipTiles(21, 467, 226, 237), new Modifiers.SkipWalls(87), 
                    new Actions.ClearTile(frameNeighbors: true), new Actions.PlaceWall(78)));

                    //place wood walls on the inside of the tree
                    WorldUtils.Gen(new Point(origin.X + num5 + 2, origin.Y - i * 5), new Shapes.Rectangle(2, 2), 
                    Actions.Chain(new Modifiers.SkipTiles(21, 467, 226, 237), new Modifiers.SkipWalls(87),
                    new Actions.ClearTile(frameNeighbors: true), new Actions.PlaceWall(78)));

                    num5 = num8;
                }

                int num10 = 6;
                if (num7 < 0.0)
                {
                    num10 = 0;
                }

                List<Point> list = new List<Point>();

                //loop to place extra branches on the top of the tree
                for (int j = 0; j < 6; j++)
                {
                    double num11 = ((double)j + 1.0) / 3.0;
                    int num12 = num10 + (int)(Math.Sin((double)num3 * num11 / 12.0 * num6 * 3.1415927410125732) * num7);
                    double num13 = WorldGen.genRand.NextDouble() * 0.7853981852531433 - 0.7853981852531433 - 0.2;

                    if (num10 == 0)
                    {
                        num13 -= 1.5707963705062866;
                    }

                    //branches
                    WorldUtils.Gen(new Point(origin.X + num12, origin.Y - (int)((double)(num3 * 5))),
                    new ShapeBranch(num13, WorldGen.genRand.Next(12, 22)).OutputEndpoints(list), Actions.Chain(new Modifiers.SkipTiles(21, 467, 226, 237), 
                    new Modifiers.SkipWalls(87), new Actions.SetTile(383), new Actions.SetFrames(frameNeighbors: true)));

                    num10 = 6 - num10;
                }

                int num14 = (int)(Math.Sin((double)num3 / 12.0 * num6 * 3.1415927410125732) * num7);

                //places branches for the leaves to grow on
                WorldUtils.Gen(new Point(origin.X + 6 + num14, origin.Y - num4), new ShapeBranch(-0.6853981852531433, 
                WorldGen.genRand.Next(16, 22)).OutputEndpoints(list), Actions.Chain(new Modifiers.SkipTiles(21, 467, 226, 237), 
                new Modifiers.SkipWalls(87), new Actions.SetTile(383), new Actions.SetFrames(frameNeighbors: true)));
                
                WorldUtils.Gen(new Point(origin.X + num14, origin.Y - num4), new ShapeBranch(-2.45619455575943, 
                WorldGen.genRand.Next(16, 22)).OutputEndpoints(list), Actions.Chain(new Modifiers.SkipTiles(21, 467, 226, 237), 
                new Modifiers.SkipWalls(87), new Actions.SetTile(383), new Actions.SetFrames(frameNeighbors: true)));
                
                foreach (Point item in list)
                {
                    //place living mahogany leaves
                    WorldUtils.Gen(item, new Shapes.Circle(4), Actions.Chain(new Modifiers.Blotches(WorldGen.genRand.Next(5, 8), WorldGen.genRand.Next(3, 6)), 
                    new Modifiers.SkipTiles(383, 21, 467, 226, 237), new Modifiers.SkipWalls(78, 87), new Actions.SetTile(384), new Actions.SetFrames(frameNeighbors: true)));
                }

                for (int k = 0; k < 5; k++)
                {
                    double angle = (double)k / 3.0 * 2.0 + 0.57075;
                    WorldUtils.Gen(origin, new ShapeRoot((float)angle, WorldGen.genRand.Next(40, 60)), Actions.Chain(new Modifiers.SkipTiles(21, 467, 226, 237), 
                    new Modifiers.SkipWalls(87), new Actions.SetTile(383, setSelfFrames: true)));
                }

                WorldGen.AddBuriedChest(origin.X + 3, origin.Y - 1, (WorldGen.genRand.Next(4) != 0) ? WorldGen.GetNextJungleChestItem() : 0, notNearOtherChests: false, 10, trySlope: false, 0);
                
                //i dunno how to use this properly, so disabled it stays
                //though i guess theres not really any other structures in the surface jungle anyways
                //structures.AddProtectedStructure(new Rectangle(origin.X - 30, origin.Y - 30, 60, 60));

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

            int num = 0;
            int[] array = new int[1000];
            int[] array2 = new int[1000];
            Vector2 larvaLocation = origin.ToVector2();
            int numHiveTunnels = WorldGen.genRand.Next(10, 13);

            GrowLivingJungleTree(new Point(origin.X, origin.Y));

            for (int i = 0; i < numHiveTunnels; i++)
            {
                Vector2 vector2 = larvaLocation;
                int num3 = WorldGen.genRand.Next(2, 5);
                for (int j = 0; j < num3; j++)
                    vector2 = CreateHiveTunnel((int)larvaLocation.X, (int)larvaLocation.Y, WorldGen.genRand);

                larvaLocation = vector2;
                array[num] = (int)larvaLocation.X;
                array2[num] = (int)larvaLocation.Y;
                num++;
            }

            FrameOutAllHiveContents(origin, 50);

            for (int k = 0; k < num; k++)
            {
                int num4 = array[k];
                int y = array2[k];
                int num5 = 1;
                if (WorldGen.genRand.NextBool())
                    num5 = -1;

                bool flag = false;
                while (WorldGen.InWorld(num4, y, 10) && BadSpotForHoneyFall(num4, y))
                {
                    num4 += num5;
                    if (Math.Abs(num4 - array[k]) > 50)
                    {
                        flag = true;
                        break;
                    }
                }

                if (!flag)
                {
                    num4 += num5;
                    if (!SpotActuallyNotInHive(num4, y))
                    {
                        CreateBlockedHoneyCube(num4, y);
                        CreateDentForHoneyFall(num4, y, num5);
                    }
                }
            }

            CreateStandForLarva(larvaLocation);

            // Generate a second larva stand
            Vector2 secondLarvaLocation = default;
            int maxAttempts = 1000;
            for (int l = 0; l < maxAttempts; l++)
            {
                Vector2 vector3 = larvaLocation;
                vector3.X += WorldGen.genRand.Next(-50, 51);
                vector3.Y += WorldGen.genRand.Next(-50, 51);
                if (WorldGen.InWorld((int)vector3.X, (int)vector3.Y) && Vector2.Distance(larvaLocation, vector3) > 10f && !Main.tile[(int)vector3.X, (int)vector3.Y].HasTile && Main.tile[(int)vector3.X, (int)vector3.Y].WallType == WallID.HiveUnsafe)
                {
                    secondLarvaLocation = vector3;
                    CreateStandForLarva(vector3);
                    break;
                }
            }

            // Honey chests
            Vector2 honeyChestLocation = default;
            for (int l = 0; l < maxAttempts; l++)
            {
                Vector2 vector3 = larvaLocation;
                vector3.X += WorldGen.genRand.Next(-100, 101);
                vector3.Y += WorldGen.genRand.Next(-100, 101);
                if (WorldGen.InWorld((int)vector3.X, (int)vector3.Y) && Vector2.Distance(larvaLocation, vector3) > 10f && Vector2.Distance(secondLarvaLocation, vector3) > 10f && !Main.tile[(int)vector3.X, (int)vector3.Y].HasTile && Main.tile[(int)vector3.X, (int)vector3.Y].WallType == WallID.HiveUnsafe)
                {
                    honeyChestLocation = vector3;
                    CreateStandAndPlaceHoneyChest(vector3);
                    break;
                }
            }
            for (int l = 0; l < maxAttempts; l++)
            {
                Vector2 vector3 = larvaLocation;
                vector3.X += WorldGen.genRand.Next(-100, 101);
                vector3.Y += WorldGen.genRand.Next(-100, 101);
                if (WorldGen.InWorld((int)vector3.X, (int)vector3.Y) && Vector2.Distance(larvaLocation, vector3) > 10f && Vector2.Distance(secondLarvaLocation, vector3) > 10f && Vector2.Distance(honeyChestLocation, vector3) > 10f && !Main.tile[(int)vector3.X, (int)vector3.Y].HasTile && Main.tile[(int)vector3.X, (int)vector3.Y].WallType == WallID.HiveUnsafe)
                {
                    CreateStandAndPlaceHoneyChest(vector3);
                    break;
                }
            }

            structures.AddProtectedStructure(new Rectangle(origin.X - 50, origin.Y - 50, 100, 100), 5);
            return true;
        }

        private static void FrameOutAllHiveContents(Point origin, int squareHalfWidth)
        {
            int num = Math.Max(10, origin.X - squareHalfWidth);
            int num2 = Math.Min(Main.maxTilesX - 10, origin.X + squareHalfWidth);
            int num3 = Math.Max(10, origin.Y - squareHalfWidth);
            int num4 = Math.Min(Main.maxTilesY - 10, origin.Y + squareHalfWidth);
            for (int i = num; i < num2; i++)
            {
                for (int j = num3; j < num4; j++)
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
            double num = random.Next(20, 26);
            float num2 = random.Next(30, 41);
            float num3 = Main.maxTilesX / 4200;
            num3 = (num3 + 1f) / 2f;
            num *= (double)num3;
            num2 *= num3;

            double num4 = num;
            Vector2 result = default;
            result.X = i;
            result.Y = j;
            Vector2 vector = default;
            vector.X = random.Next(-10, 11) * 0.2f;
            vector.Y = random.Next(-10, 11) * 0.2f;
            while (num > 0.0 && num2 > 0f)
            {
                if (result.Y > (Main.maxTilesY - 250))
                    num2 = 0f;

                num = num4 * (double)(1f + random.Next(-20, 20) * 0.01f);
                num2 -= 1f;
                int num5 = (int)(result.X - num);
                int num6 = (int)(result.X + num);
                int num7 = (int)(result.Y - num);
                int num8 = (int)(result.Y + num);
                if (num5 < 1)
                    num5 = 1;

                if (num6 > Main.maxTilesX - 1)
                    num6 = Main.maxTilesX - 1;

                if (num7 < 1)
                    num7 = 1;

                if (num8 > Main.maxTilesY - 1)
                    num8 = Main.maxTilesY - 1;

                for (int k = num5; k < num6; k++)
                {
                    for (int l = num7; l < num8; l++)
                    {
                        if (!WorldGen.InWorld(k, l, 50))
                        {
                            num2 = 0f;
                        }
                        else
                        {
                            if (Main.tile[k - 10, l].WallType == WallID.LihzahrdBrickUnsafe)
                                num2 = 0f;

                            if (Main.tile[k + 10, l].WallType == WallID.LihzahrdBrickUnsafe)
                                num2 = 0f;

                            if (Main.tile[k, l - 10].WallType == WallID.LihzahrdBrickUnsafe)
                                num2 = 0f;

                            if (Main.tile[k, l + 10].WallType == WallID.LihzahrdBrickUnsafe)
                                num2 = 0f;
                        }

                        if (l < Main.worldSurface && Main.tile[k, l - 5].WallType == WallID.None)
                            num2 = 0f;

                        float num9 = Math.Abs(k - result.X);
                        float num10 = Math.Abs(l - result.Y);
                        double num11 = Math.Sqrt(num9 * num9 + num10 * num10);
                        if (num11 < num4 * 0.4 * (1.0 + random.Next(-10, 11) * 0.005))
                        {
                            Main.tile[k, l].LiquidAmount = byte.MaxValue;
                            Main.tile[k, l].Get<LiquidData>().LiquidType = LiquidID.Honey;
                            Main.tile[k, l].WallType = WallID.HiveUnsafe;
                            Main.tile[k, l].Get<TileWallWireStateData>().HasTile = false;
                            Main.tile[k, l].Get<TileWallWireStateData>().IsHalfBlock = false;
                            Main.tile[k, l].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                        }
                        else if (num11 < num4 * 0.75 * (1.0 + random.Next(-10, 11) * 0.005))
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

                        if (num11 < num4 * 0.6 * (1.0 + random.Next(-10, 11) * 0.005))
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

                result += vector;
                num2 -= 1f;
                vector.Y += random.Next(-10, 11) * 0.05f;
                vector.X += random.Next(-10, 11) * 0.05f;
            }

            return result;
        }

        private static bool TooCloseToImportantLocations(Point origin)
        {
            int x = origin.X;
            int y = origin.Y;
            int num = 150;
            for (int i = x - num; i < x + num; i += 10)
            {
                if (i <= 0 || i > Main.maxTilesX - 1)
                    continue;

                for (int j = y - num; j < y + num; j += 10)
                {
                    if (j > 0 && j <= Main.maxTilesY - 1)
                    {
                        if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.LihzahrdBrick)
                            return true;

                        if (Main.tile[i, j].WallType == WallID.CrimstoneUnsafe || Main.tile[i, j].WallType == WallID.EbonstoneUnsafe || Main.tile[i, j].WallType == WallID.LihzahrdBrickUnsafe)
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
            int num = 0;
            while ((num < 4 || WorldGen.SolidTile(x, y)) && x > 10 && x < Main.maxTilesX - 10)
            {
                num++;
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
            WorldGen.larvaX[WorldGen.numLarva] = Utils.Clamp((int)position.X, 5, Main.maxTilesX - 5);
            WorldGen.larvaY[WorldGen.numLarva] = Utils.Clamp((int)position.Y, 5, Main.maxTilesY - 5);
            WorldGen.numLarva++;
            if (WorldGen.numLarva >= WorldGen.larvaX.Length)
                WorldGen.numLarva = WorldGen.larvaX.Length - 1;

            int num = (int)position.X;
            int num2 = (int)position.Y;
            for (int i = num - 1; i <= num + 1 && i > 0 && i < Main.maxTilesX; i++)
            {
                for (int j = num2 - 2; j <= num2 + 1 && j > 0 && j < Main.maxTilesY; j++)
                {
                    if (j != num2 + 1)
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

            int num = (int)position.X;
            int num2 = (int)position.Y;
            for (int i = num; i <= num + 1 && i > 0 && i < Main.maxTilesX; i++)
            {
                for (int j = num2 - 1; j <= num2 + 1 && j > 0 && j < Main.maxTilesY; j++)
                {
                    if (j != num2 + 1)
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

            int num144 = chestPlacementX;
            int num145 = chestPlacementY;
            for (int num146 = num144; num146 <= num144 + 1; num146++)
            {
                for (int num147 = num145 - 1; num147 <= num145 + 1; num147++)
                {
                    if (num147 != num145 + 1)
                    {
                        Main.tile[num146, num147].Get<TileWallWireStateData>().HasTile = false;
                    }
                    else
                    {
                        Main.tile[num146, num147].Get<TileWallWireStateData>().HasTile = true;
                        Main.tile[num146, num147].TileType = TileID.Hive;
                        Main.tile[num146, num147].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                        Main.tile[num146, num147].Get<TileWallWireStateData>().IsHalfBlock = false;
                    }
                }
            }

            int chestID = WorldGen.PlaceChest(num144, num145, 21, false, 29);
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
            WorldGen.silverBar == TileID.Silver ? ItemID.SilverBar : ItemID.TungstenBar,
            WorldGen.goldBar == TileID.Gold ? ItemID.GoldBar : ItemID.PlatinumBar
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
