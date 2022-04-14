using CalamityMod.Tiles.SunkenSea;
using CalamityMod.Walls;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityMod.World
{
    public class SunkenSea
    {
        private struct Hub
        {
            public Vector2 Position;

            public Hub(Vector2 position)
            {
                Position = position;
            }

            public Hub(float x, float y)
            {
                Position = new Vector2(x, y);
            }
        }

        private class Cluster : List<Hub>
        {
        }

        private class ClusterGroup : List<Cluster>
        {
            public int Width;

            public int Height;

            private void SearchForCluster(bool[,] hubMap, List<Point> pointCluster, int x, int y, int level = 2)
            {
                pointCluster.Add(new Point(x, y));
                hubMap[x, y] = false;
                level--;
                if (level == -1)
                {
                    return;
                }
                if (x > 0 && hubMap[x - 1, y])
                {
                    SearchForCluster(hubMap, pointCluster, x - 1, y, level);
                }
                if (x < hubMap.GetLength(0) - 1 && hubMap[x + 1, y])
                {
                    SearchForCluster(hubMap, pointCluster, x + 1, y, level);
                }
                if (y > 0 && hubMap[x, y - 1])
                {
                    SearchForCluster(hubMap, pointCluster, x, y - 1, level);
                }
                if (y < hubMap.GetLength(1) - 1 && hubMap[x, y + 1])
                {
                    SearchForCluster(hubMap, pointCluster, x, y + 1, level);
                }
            }

            private void AttemptClaim(int x, int y, int[,] clusterIndexMap, List<List<Point>> pointClusters, int index) //Attempts to create a cluster
            {
                int num = clusterIndexMap[x, y];
                if (num != -1 && num != index)
                {
                    int num2 = (WorldGen.genRand.Next(2) == 0) ? -1 : index;
                    foreach (Point current in pointClusters[num])
                    {
                        clusterIndexMap[current.X, current.Y] = num2;
                    }
                }
            }

            public void Generate(int width, int height) //Creates clusters for cluster group
            {
                Width = width;
                Height = height;
                Clear();
                bool[,] array = new bool[width, height];
                int num = (width >> 1) - 1;
                int num2 = (height >> 1) - 1;
                int num3 = (num + 1) * (num + 1);
                Point point = new Point(num, num2);
                for (int i = point.Y - num2; i <= point.Y + num2; i++)
                {
                    float num4 = (float)num / (float)num2 * (float)(i - point.Y);
                    int num5 = Math.Min(num, (int)Math.Sqrt((float)num3 - num4 * num4));
                    for (int j = point.X - num5; j <= point.X + num5; j++)
                    {
                        array[j, i] = WorldGen.genRand.Next(2) == 0;
                    }
                }
                List<List<Point>> list = new List<List<Point>>();
                for (int k = 0; k < array.GetLength(0); k++)
                {
                    for (int l = 0; l < array.GetLength(1); l++)
                    {
                        if (array[k, l] && WorldGen.genRand.Next(2) == 0)
                        {
                            List<Point> list2 = new List<Point>();
                            SearchForCluster(array, list2, k, l, 2);
                            if (list2.Count > 2)
                            {
                                list.Add(list2);
                            }
                        }
                    }
                }
                int[,] array2 = new int[array.GetLength(0), array.GetLength(1)];
                for (int m = 0; m < array2.GetLength(0); m++)
                {
                    for (int n = 0; n < array2.GetLength(1); n++)
                    {
                        array2[m, n] = -1;
                    }
                }
                for (int num6 = 0; num6 < list.Count; num6++)
                {
                    foreach (Point current in list[num6])
                    {
                        array2[current.X, current.Y] = num6;
                    }
                }
                for (int num7 = 0; num7 < list.Count; num7++)
                {
                    foreach (Point point2 in list[num7])
                    {
                        int x = point2.X;
                        int y = point2.Y;
                        if (array2[x, y] == -1)
                        {
                            break;
                        }
                        int index = array2[x, y];
                        if (x > 0)
                        {
                            AttemptClaim(x - 1, y, array2, list, index);
                        }
                        if (x < array2.GetLength(0) - 1)
                        {
                            AttemptClaim(x + 1, y, array2, list, index);
                        }
                        if (y > 0)
                        {
                            AttemptClaim(x, y - 1, array2, list, index);
                        }
                        if (y < array2.GetLength(1) - 1)
                        {
                            AttemptClaim(x, y + 1, array2, list, index);
                        }
                    }
                }
                using (List<List<Point>>.Enumerator enumerator2 = list.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        enumerator2.Current.Clear();
                    }
                }
                for (int num8 = 0; num8 < array2.GetLength(0); num8++)
                {
                    for (int num9 = 0; num9 < array2.GetLength(1); num9++)
                    {
                        if (array2[num8, num9] != -1)
                        {
                            list[array2[num8, num9]].Add(new Point(num8, num9));
                        }
                    }
                }
                foreach (List<Point> current2 in list)
                {
                    if (current2.Count < 4)
                    {
                        current2.Clear();
                    }
                }
                foreach (List<Point> current3 in list)
                {
                    Cluster cluster = new Cluster();
                    if (current3.Count > 0)
                    {
                        foreach (Point current4 in current3)
                        {
                            cluster.Add(new Hub((float)current4.X + (WorldGen.genRand.NextFloat() - 0.5f) * 0.5f, (float)current4.Y + (WorldGen.genRand.NextFloat() - 0.5f) * 0.5f));
                        }
                        Add(cluster);
                    }
                }
            }
        }

        // Generates clumps / clusters / chunks of general tiles in the Sunken Sea area
        private static void PlaceClusters(ClusterGroup clusters, Point start, Vector2 terrainApplicationScaleVector)
        {
            int totalWidth = (int)(terrainApplicationScaleVector.X * clusters.Width);
            int totalHeight = (int)(terrainApplicationScaleVector.Y * clusters.Height);
            Vector2 totalScale = new Vector2(totalWidth, totalHeight);
            Vector2 individualScale = new Vector2(clusters.Width, clusters.Height);

            for (int i = -20; i < totalWidth + 20; i++)
            {
                for (int j = -20; j < totalHeight + 20; j++)
                {
                    float num3 = 0f;
                    int num4 = -1;
                    float num5 = 0f;
                    int num6 = i + start.X;
                    int num7 = j + start.Y;
                    Vector2 vector = new Vector2((float)i, (float)j) / totalScale * individualScale;
                    float num8 = (new Vector2((float)i, (float)j) / totalScale * 2f - Vector2.One).Length();
                    for (int k = 0; k < clusters.Count; k++)
                    {
                        Cluster cluster = clusters[k];
                        if (Math.Abs(cluster[0].Position.X - vector.X) <= 10f && Math.Abs(cluster[0].Position.Y - vector.Y) <= 10f)
                        {
                            float num9 = 0f;
                            foreach (Hub current in cluster)
                            {
                                num9 += 1f / Vector2.DistanceSquared(current.Position, vector);
                            }
                            if (num9 > num3)
                            {
                                if (num3 > num5)
                                {
                                    num5 = num3;
                                }
                                num3 = num9;
                                num4 = k;
                            }
                            else if (num9 > num5)
                            {
                                num5 = num9;
                            }
                        }
                    }
                    float num10 = num3 + num5;
                    Tile tile = Main.tile[num6, num7];
                    bool flag = num8 >= 0.8f;
                    if (num10 > 3.5f) //Adjust num10 for all cases if you want different tile frequencies; higher is less frequent, lower is more frequent
                    {
                        tile.ClearEverything();
                        tile.WallType = (ushort)ModContent.WallType<NavystoneWall>();
                        tile.LiquidAmount = 192;
                        if (num4 % 15 == 2)
                        {
                            tile.ResetToType((ushort)ModContent.TileType<Navystone>());
                            tile.Get<TileWallWireStateData>().HasTile = true;
                            tile.LiquidAmount = 0;
                        }
                        Tile.SmoothSlope(num6, num7, true);
                    }
                    else if (num10 > 1.8f)
                    {
                        tile.WallType = (ushort)ModContent.WallType<NavystoneWall>();
                        tile.LiquidAmount = 192;
                        if (!flag || tile.HasTile)
                        {
                            tile.ResetToType((ushort)ModContent.TileType<Navystone>());
                            tile.Get<TileWallWireStateData>().HasTile = true;
                            Tile.SmoothSlope(num6, num7, true);
                            tile.LiquidAmount = 0;
                        }
                    }
                    else if (num10 > 0.7f || !flag)
                    {
                        tile.LiquidAmount = 192;
                        if (!flag || tile.HasTile)
                        {
                            tile.ResetToType((ushort)ModContent.TileType<EutrophicSand>());
                            tile.Get<TileWallWireStateData>().HasTile = true;
                            Tile.SmoothSlope(num6, num7, true);
                            tile.LiquidAmount = 0;
                        }
                        tile.WallType = (ushort)ModContent.WallType<EutrophicSandWall>();
                    }
                    else if (num10 > 0.25f)
                    {
                        float num11 = (num10 - 0.25f) / 0.45f;
                        if (WorldGen.genRand.NextFloat() < num11)
                        {
                            if (tile.HasTile)
                            {
                                tile.ResetToType((ushort)ModContent.TileType<EutrophicSand>());
                                tile.Get<TileWallWireStateData>().HasTile = true;
                                Tile.SmoothSlope(num6, num7, true);
                                tile.WallType = (ushort)ModContent.WallType<EutrophicSandWall>();
                                tile.LiquidAmount = 0;
                            }
                            else
                            {
                                tile.WallType = (ushort)ModContent.WallType<NavystoneWall>();
                                tile.LiquidAmount = 192;
                            }
                        }
                    }
                }
            }
        }

        // Adds tile variation to generated tile clusters and generates open areas with sea prism ore called "Tits"
        // Generates sea prism crystals on prism ore and occasionally on navystone
        private static void AddTileVariance(ClusterGroup clusters, Point start, Vector2 terrainApplicationScaleVector, int biomeAreaX, int biomeAreaY, float overallBiomeScale)
        {
            int num = (int)(terrainApplicationScaleVector.X * clusters.Width);
            int num2 = (int)(terrainApplicationScaleVector.Y * clusters.Height);
            bool genCentalHole = true;
            Rectangle rectangle = default;

            // Radius of the generated hole
            int radius = (int)(WorldGen.genRand.Next(24, 28) * overallBiomeScale);
            int diameter = radius * 2;

            // Where to place the giant hole? "Center of the Sunken Sea", defined as follows:
            // Dead center horizontally, and 1/3 of the way down the area
            Point point = new Point(start.X + biomeAreaX / 2, start.Y + (int)(biomeAreaY * 0.33f));

            ShapeData holeShape = new ShapeData();
            float outerRadiusPercentage = WorldGen.genRand.Next(40, 56) * 0.01f; //Small radius for ore patch to fit inside holes

            // Y coordinate of the "bottom" of the Sunken Sea, 70% of the way down.
            int sunkenSeaBottom = start.Y + (int)(biomeAreaY * 0.7f);
            int smallHoles = 0;

            // Scale amount of holes with world size
            // 4 on Small, 6 on Normal (rounds down), 8 on Large
            int totalHoleCount = (int)(4f * overallBiomeScale);

            for (int i = -20; i < num + 20; i++)
            {
                for (int j = -20; j < num2 + 20; j++)
                {
                    if (genCentalHole)
                    {
                        // Set the rectangle for the central hole
                        genCentalHole = false;
                        rectangle = new Rectangle(start.X + biomeAreaX / 2 - radius, start.Y + (int)(biomeAreaY * 0.33f) - radius, diameter, diameter);

                        WorldUtils.Gen(point, new Shapes.Circle(radius), Actions.Chain(new GenAction[]
                        {
                            new Modifiers.Blotches(2, 0.6).Output(holeShape), //Adds uneven shape to the outer tiles
                            new Actions.ClearTile(true), //Clear all tiles
                            new Actions.SetLiquid(0, 192) //Leave some air pockets
                        }));

                        WorldUtils.Gen(point, new ModShapes.OuterOutline(holeShape, true, true), Actions.Chain(new GenAction[] //Smooth the outer parts
                        {
                            new CustomActions.DistanceFromOrigin(true, radius * 16f - 48),
                            new Modifiers.Conditions(new CustomConditions.RandomChance(4)),
                            new Actions.Smooth(),
                            new Actions.SetFrames(true)
                        }));

                        WorldUtils.Gen(point, new Shapes.Circle((int)((float)radius * outerRadiusPercentage)), Actions.Chain(new GenAction[] //Smallest is 6
                        {
                            new Modifiers.Blotches(2, 0.3).Output(holeShape),
                            new Actions.SetTile((ushort)ModContent.TileType<Navystone>(), true) //Place outer shell
                        }));

                        WorldUtils.Gen(point, new ModShapes.OuterOutline(holeShape, true, true), Actions.Chain(new GenAction[] //Smooth outer shell
                        {
                            new CustomActions.DistanceFromOrigin(true, radius * 16f - 48),
                            new Modifiers.Conditions(new CustomConditions.RandomChance(3)),
                            new Actions.Smooth(),
                            new Actions.SetFrames(true)
                        }));

                        WorldUtils.Gen(point, new Shapes.Circle((int)((float)radius * (outerRadiusPercentage * 0.6f))), Actions.Chain(new GenAction[] //Smallest is 4
                        {
                            new Modifiers.Blotches(2, 0.3),
                            new Actions.SetTile((ushort)ModContent.TileType<SeaPrism>(), true) //Place prism
                        }));

                        WorldUtils.Gen(point, new Shapes.Circle((int)((float)radius * (outerRadiusPercentage * 0.3f))), Actions.Chain(new GenAction[] //Smallest is 2
                        {
                            new Modifiers.Blotches(2, 0.3).Output(holeShape),
                            new Actions.ClearTile(true), //Clear all tiles
                            new Actions.SetLiquid(0, 255)
                        }));

                        WorldUtils.Gen(point, new ModShapes.OuterOutline(holeShape, true, true), Actions.Chain(new GenAction[] //Smooth inner shell
                        {
                            new CustomActions.DistanceFromOrigin(true, radius * 16f - 48),
                            new Modifiers.Conditions(new CustomConditions.RandomChance(2)),
                            new Actions.Smooth(),
                            new Actions.SetFrames(true)
                        }));
                    }

                    // Set the point for non-central holes
                    // X = RAND(Left edge + 30, Right edge - 30)
                    // Y = RAND(Top edge + 20, Bottom calculated earlier)
                    int smallHoleX = WorldGen.genRand.Next(start.X + 30, start.X + biomeAreaX - 30);
                    int smallHoleY = WorldGen.genRand.Next(start.Y + 20, sunkenSeaBottom);
                    point = new Point(smallHoleX, smallHoleY);

                    if (smallHoles < totalHoleCount && WorldGen.genRand.Next(3) == 0 && !rectangle.Contains(point))
                    {
                        smallHoles++;
                        int radiusSmall = (int)(((float)WorldGen.genRand.Next(8, 11)) * overallBiomeScale);
                        WorldUtils.Gen(point, new Shapes.Circle(radiusSmall), Actions.Chain(new GenAction[]
                        {
                            new Modifiers.Blotches(2, 0.45).Output(holeShape),
                            new Actions.ClearTile(true),
                            new Actions.SetLiquid(0, 192)
                        }));

                        WorldUtils.Gen(point, new ModShapes.OuterOutline(holeShape, true, true), Actions.Chain(new GenAction[]
                        {
                            new CustomActions.DistanceFromOrigin(true, radiusSmall * 16f - 48),
                            new Modifiers.Conditions(new CustomConditions.RandomChance(3)),
                            new Actions.Smooth(),
                            new Actions.SetFrames(true)
                        }));

                        outerRadiusPercentage = (float)((double)WorldGen.genRand.Next(65, 81) * 0.01);
                        WorldUtils.Gen(point, new Shapes.Circle((int)((float)radiusSmall * outerRadiusPercentage)), Actions.Chain(new GenAction[] //Smallest is 4
                        {
                            new Modifiers.Blotches(2, 0.3).Output(holeShape),
                            new Actions.SetTile((ushort)ModContent.TileType<Navystone>(), true) //Place outer shell
                        }));

                        WorldUtils.Gen(point, new ModShapes.OuterOutline(holeShape, true, true), Actions.Chain(new GenAction[] //Smooth outer shell
                        {
                            new CustomActions.DistanceFromOrigin(true, radiusSmall * 16f - 48),
                            new Modifiers.Conditions(new CustomConditions.RandomChance(3)),
                            new Actions.Smooth(),
                            new Actions.SetFrames(true)
                        }));

                        WorldUtils.Gen(point, new Shapes.Circle((int)((float)radiusSmall * (outerRadiusPercentage * 0.6f))), Actions.Chain(new GenAction[] //Smallest is 2
                        {
                            new Modifiers.Blotches(2, 0.3),
                            new Actions.SetTile((ushort)ModContent.TileType<SeaPrism>(), true) //Place prism
                        }));

                        WorldUtils.Gen(point, new Shapes.Circle((int)((float)radiusSmall * (outerRadiusPercentage * 0.3f))), Actions.Chain(new GenAction[] //Smallest is 1
                        {
                            new Modifiers.Blotches(2, 0.3).Output(holeShape),
                            new Actions.ClearTile(true), //Clear center
                            new Actions.SetLiquid(0, 255)
                        }));

                        WorldUtils.Gen(point, new ModShapes.OuterOutline(holeShape, true, true), Actions.Chain(new GenAction[] //Smooth inner shell
                        {
                            new CustomActions.DistanceFromOrigin(true, radius * 16f - 48),
                            new Modifiers.Conditions(new CustomConditions.RandomChance(2)),
                            new Actions.Smooth(),
                            new Actions.SetFrames(true)
                        }));
                    }
                    int num3 = i + start.X;
                    int num4 = j + start.Y;
                    Tile tile = Main.tile[num3, num4];
                    Tile testTile = Main.tile[num3, num4 + 1];
                    Tile testTile2 = Main.tile[num3, num4 + 2];
                    if (tile.TileType == ModContent.TileType<EutrophicSand>() && (!WorldGen.SolidTile(testTile) || !WorldGen.SolidTile(testTile2))) //Tile variation
                    {
                        tile.TileType = (ushort)ModContent.TileType<Navystone>();
                    }
                }
            }
            for (int k = -20; k < num + 20; k++)
            {
                for (int l = -20; l < num2 + 20; l++)
                {
                    int num5 = k + start.X;
                    int num6 = l + start.Y;
                    Tile tile2 = Main.tile[num5, num6];
                    if (tile2.HasTile && (tile2.TileType == ModContent.TileType<SeaPrism>() || tile2.TileType == ModContent.TileType<Navystone>()))
                    {
                        bool flag = true;
                        for (int m = -1; m >= -3; m--)
                        {
                            if (Main.tile[num5, num6 + m].HasTile)
                            {
                                flag = false;
                                break;
                            }
                        }
                        bool flag2 = true;
                        for (int n = 1; n <= 3; n++)
                        {
                            if (Main.tile[num5, num6 + n].HasTile)
                            {
                                flag2 = false;
                                break;
                            }
                        }
                        bool flag3 = true;
                        for (int o = -1; o >= -3; o--)
                        {
                            if (Main.tile[num5 + o, num6].HasTile)
                            {
                                flag3 = false;
                                break;
                            }
                        }
                        bool flag4 = true;
                        for (int p = 1; p <= 3; p++)
                        {
                            if (Main.tile[num5 + p, num6].HasTile)
                            {
                                flag4 = false;
                                break;
                            }
                        }
                        if (tile2.TileType == ModContent.TileType<SeaPrism>() || (tile2.TileType == ModContent.TileType<Navystone>() && WorldGen.genRand.Next(8) == 0))
                        {
                            if (flag3 ^ flag4)
                            {
                                if (tile2.Slope == 0 && !tile2.IsHalfBlock)
                                {
                                    Tile tile3 = Main.tile[num5 + (flag3 ? -1 : 1), num6];
                                    tile3.TileType = (ushort)ModContent.TileType<SeaPrismCrystals>();
                                    if (Main.tile[num5 - 1, num6].TileType == ModContent.TileType<SeaPrismCrystals>())
                                    {
                                        Main.tile[num5 - 1, num6].TileFrameY = (short)(2 * 18);
                                    }
                                    else if (Main.tile[num5 + 1, num6].TileType == ModContent.TileType<SeaPrismCrystals>())
                                    {
                                        Main.tile[num5 + 1, num6].TileFrameY = (short)(3 * 18);
                                    }
                                    tile3.TileFrameX = (short)(WorldGen.genRand.Next(18) * 18);
                                    tile3.Get<TileWallWireStateData>().HasTile = true;
                                }
                            }
                            if (flag ^ flag2)
                            {
                                if (tile2.Slope == 0 && !tile2.IsHalfBlock)
                                {
                                    Tile tile3 = Main.tile[num5, num6 + (flag ? -1 : 1)];
                                    tile3.TileType = (ushort)ModContent.TileType<SeaPrismCrystals>();
                                    if (Main.tile[num5, num6 - 1].TileType == ModContent.TileType<SeaPrismCrystals>())
                                    {
                                        Main.tile[num5, num6 - 1].TileFrameY = (short)(0 * 18);
                                    }
                                    else if (Main.tile[num5, num6 + 1].TileType == ModContent.TileType<SeaPrismCrystals>())
                                    {
                                        Main.tile[num5, num6 + 1].TileFrameY = (short)(1 * 18);
                                    }
                                    tile3.TileFrameX = (short)(WorldGen.genRand.Next(18) * 18);
                                    tile3.Get<TileWallWireStateData>().HasTile = true;
                                }
                            }
                        }
                    }
                    if (!tile2.HasTile)
                    {
                        if (tile2.WallType == ModContent.WallType<NavystoneWall>() || tile2.WallType == ModContent.WallType<EutrophicSandWall>())
                        {
                            if (WorldGen.genRand.Next(5) == 0)
                            {
                                PlaceTit(num5, num6, (ushort)ModContent.TileType<SunkenSeaStalactite>());
                            }
                            if (WorldGen.genRand.Next(8) == 0)
                            {
                                WorldGen.PlaceTile(num5, num6, (ushort)ModContent.TileType<BrainCoral>(), true, false, -1, 0);
                            }
                            if (WorldGen.genRand.Next(6) == 0)
                            {
                                WorldGen.PlaceTile(num5, num6, (ushort)ModContent.TileType<SmallBrainCoral>(), true, false, -1, 0);
                            }
                            if (WorldGen.genRand.Next(10) == 0)
                            {
                                WorldGen.PlaceTile(num5, num6, (ushort)ModContent.TileType<FanCoral>(), true, false, -1, 0);
                            }
                            if (WorldGen.genRand.Next(6) == 0)
                            {
                                WorldGen.PlaceTile(num5, num6, (ushort)ModContent.TileType<SeaAnemone>(), true, false, -1, 0);
                            }
                            if (WorldGen.genRand.Next(8) == 0)
                            {
                                WorldGen.PlaceTile(num5, num6, (ushort)ModContent.TileType<TubeCoral>(), true, false, -1, 0);
                            }
                            if (WorldGen.genRand.Next(6) == 0)
                            {
                                WorldGen.PlaceTile(num5, num6, (ushort)ModContent.TileType<SmallTubeCoral>(), true, false, -1, 0);
                            }
                            if (WorldGen.genRand.Next(4) == 0)
                            {
                                WorldGen.PlaceTile(num5, num6, (ushort)ModContent.TileType<TableCoral>(), true, false, -1, 0);
                            }
                        }
                    }
                }
            }
        }

        public static bool Place(Point origin)
        {
            // 1 on Small, 1.52 on Medium, 2 on Large
            float scale = Main.maxTilesX / 4200f;
            // Clamp scale to prevent problems on extra large worlds
            scale = MathHelper.Clamp(scale, 1f, 2f);

            // 80 on Small, 121.6 on Medium, 160 on Large
            int sunkenSeaAreaX = (int)(80f * scale);
            // 84-102 on Small, 127.68-155.04 on Medium, 168-204 on Large
            float baseVerticalSize = 60f * scale;
            float verticalScaleFactor = 1.4f + WorldGen.genRand.NextFloat(0.3f);
            int sunkenSeaAreaY = (int)(verticalScaleFactor * baseVerticalSize);

            // What the fuck is this and why is it used everywhere
            // As far as I can tell, this just scales up the entire Sunken Sea to be 4x wider and 2x taller than what is listed above
            Vector2 arbitrary42GodVector = new Vector2(4f, 2f);

            // Place the majority of the terrain as clusters
            ClusterGroup clusterGroup = new ClusterGroup();
            clusterGroup.Generate(sunkenSeaAreaX, sunkenSeaAreaY);
            PlaceClusters(clusterGroup, origin, arbitrary42GodVector);

            // "Now place the rest of the Sunken Sea" except it's called Tile Variance
            AddTileVariance(clusterGroup, origin, arbitrary42GodVector, sunkenSeaAreaX, sunkenSeaAreaY, scale);

            // Re-frame everything in some arbitrary radius
            int totalWidth = (int)(arbitrary42GodVector.X * clusterGroup.Width);
            int totalHeight = (int)(arbitrary42GodVector.Y * clusterGroup.Height);
            int frameExcessRadius = 40;
            for (int i = -frameExcessRadius; i < totalWidth + frameExcessRadius; i++)
                for (int j = -frameExcessRadius; j < totalHeight + frameExcessRadius; j++)
                {
                    if (i + origin.X > 0 && i + origin.X < Main.maxTilesX - 1 && j + origin.Y > 0 && j + origin.Y < Main.maxTilesY - 1)
                    {
                        WorldGen.SquareWallFrame(i + origin.X, j + origin.Y, true);
                        WorldUtils.TileFrame(i + origin.X, j + origin.Y, true);
                    }
                }

            // Sunken Sea generation always succeeds
            return true;
        }

        public static void PlaceTit(int x, int y, ushort type = 165)
        {
            if (WorldGen.SolidTile(x, y - 1) && !Main.tile[x, y].HasTile && !Main.tile[x, y + 1].HasTile)
            {
                if (Main.tile[x, y - 1].TileType == (ushort)ModContent.TileType<Navystone>())
                {
                    if (WorldGen.genRand.Next(2) == 0 || Main.tile[x, y + 2].HasTile)
                    {
                        int num2 = WorldGen.genRand.Next(3) * 18;
                        Main.tile[x, y].TileType = type;
                        Main.tile[x, y].Get<TileWallWireStateData>().HasTile = true;
                        Main.tile[x, y].TileFrameX = (short)num2;
                        Main.tile[x, y].TileFrameY = 72;
                    }
                    else
                    {
                        int num3 = WorldGen.genRand.Next(3) * 18;
                        Main.tile[x, y].TileType = type;
                        Main.tile[x, y].Get<TileWallWireStateData>().HasTile = true;
                        Main.tile[x, y].TileFrameX = (short)num3;
                        Main.tile[x, y].TileFrameY = 0;
                        Main.tile[x, y + 1].TileType = type;
                        Main.tile[x, y + 1].Get<TileWallWireStateData>().HasTile = true;
                        Main.tile[x, y + 1].TileFrameX = (short)num3;
                        Main.tile[x, y + 1].TileFrameY = 18;
                    }
                }
            }
            else
            {
                if (WorldGen.SolidTile(x, y + 1) && !Main.tile[x, y].HasTile && !Main.tile[x, y - 1].HasTile)
                {
                    if (Main.tile[x, y + 1].TileType == (ushort)ModContent.TileType<Navystone>())
                    {
                        if (WorldGen.genRand.Next(2) == 0 || Main.tile[x, y - 2].HasTile)
                        {
                            int num13 = WorldGen.genRand.Next(3) * 18;
                            Main.tile[x, y].TileType = type;
                            Main.tile[x, y].Get<TileWallWireStateData>().HasTile = true;
                            Main.tile[x, y].TileFrameX = (short)num13;
                            Main.tile[x, y].TileFrameY = 90;
                        }
                        else
                        {
                            int num14 = WorldGen.genRand.Next(3) * 18;
                            Main.tile[x, y - 1].TileType = type;
                            Main.tile[x, y - 1].Get<TileWallWireStateData>().HasTile = true;
                            Main.tile[x, y - 1].TileFrameX = (short)num14;
                            Main.tile[x, y - 1].TileFrameY = 36;
                            Main.tile[x, y].TileType = type;
                            Main.tile[x, y].Get<TileWallWireStateData>().HasTile = true;
                            Main.tile[x, y].TileFrameX = (short)num14;
                            Main.tile[x, y].TileFrameY = 54;
                        }
                    }
                }
            }
        }
    }
}
