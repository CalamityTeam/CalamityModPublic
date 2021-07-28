using CalamityMod.World.Planets;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.World
{
    public class WorldEvilIsland
    {
        public static void PlaceEvilIsland()
        {
            int x = Main.maxTilesX;
            int xIslandGen;
            int yIslandGen;
            Rectangle potentialArea;

            do
            {
                xIslandGen = WorldGen.crimson ?
                    WorldGen.genRand.Next((int)(x * 0.1), (int)(x * 0.15)) :
                    WorldGen.genRand.Next((int)(x * 0.85), (int)(x * 0.9));
                yIslandGen = WorldGen.genRand.Next(90, 151);
                yIslandGen = Math.Min(yIslandGen, (int)WorldGen.worldSurfaceLow - 50);

                int checkAreaX = 200;
                int checkAreaY = 200;
                potentialArea = new Rectangle(xIslandGen + checkAreaX / 2, yIslandGen + checkAreaY / 2, checkAreaX, checkAreaY);
            }
            while (!Planetoid.InvalidSkyPlacementArea(potentialArea));

            int tileXLookup = xIslandGen;
            if (WorldGen.crimson)
            {
                while (Main.tile[tileXLookup, yIslandGen].active())
                    tileXLookup++;
            }
            else
            {
                while (Main.tile[tileXLookup, yIslandGen].active())
                    tileXLookup--;
            }

            xIslandGen = tileXLookup;
            CalamityWorld.fehX = xIslandGen;
            CalamityWorld.fehY = yIslandGen;
            EvilIsland(xIslandGen, yIslandGen);
            EvilIslandHouse(CalamityWorld.fehX, CalamityWorld.fehY);
        }

        public static void EvilIsland(int i, int j)
        {
            double num = (double)WorldGen.genRand.Next(100, 150);
            float num2 = (float)WorldGen.genRand.Next(20, 30);
            int num3 = i;
            int num4 = i;
            int num5 = i;
            int num6 = j;
            Vector2 vector;
            vector.X = (float)i;
            vector.Y = (float)j;
            Vector2 vector2;
            vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            while (vector2.X > -2f && vector2.X < 2f)
            {
                vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            }
            vector2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.02f;
            while (num > 0.0 && num2 > 0f)
            {
                num -= (double)WorldGen.genRand.Next(4);
                num2 -= 1f;
                int num7 = (int)((double)vector.X - num * 0.5);
                int num8 = (int)((double)vector.X + num * 0.5);
                int num9 = (int)((double)vector.Y - num * 0.5);
                int num10 = (int)((double)vector.Y + num * 0.5);
                if (num7 < 0)
                {
                    num7 = 0;
                }
                if (num8 > Main.maxTilesX)
                {
                    num8 = Main.maxTilesX;
                }
                if (num9 < 0)
                {
                    num9 = 0;
                }
                if (num10 > Main.maxTilesY)
                {
                    num10 = Main.maxTilesY;
                }
                double num11 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01; //80 120
                float num12 = vector.Y + 1f;
                for (int k = num7; k < num8; k++)
                {
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num12 += (float)WorldGen.genRand.Next(-1, 2);
                    }
                    if (num12 < vector.Y)
                    {
                        num12 = vector.Y;
                    }
                    if (num12 > vector.Y + 2f)
                    {
                        num12 = vector.Y + 2f;
                    }
                    for (int l = num9; l < num10; l++)
                    {
                        if ((float)l > num12)
                        {
                            float arg_218_0 = Math.Abs((float)k - vector.X);
                            float num13 = Math.Abs((float)l - vector.Y) * 3f;
                            if (Math.Sqrt((double)(arg_218_0 * arg_218_0 + num13 * num13)) < num11 * 0.4)
                            {
                                if (k < num3)
                                {
                                    num3 = k;
                                }
                                if (k > num4)
                                {
                                    num4 = k;
                                }
                                if (l < num5)
                                {
                                    num5 = l;
                                }
                                if (l > num6)
                                {
                                    num6 = l;
                                }
                                Main.tile[k, l].active(true);
                                Main.tile[k, l].type = (ushort)(WorldGen.crimson ? 400 : 401); //ebonstone or crimstone
                                WorldGen.SquareTileFrame(k, l, true);
                            }
                        }
                    }
                }
                vector += vector2;
                vector2.X += (float)WorldGen.genRand.Next(-20, 21) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if ((double)vector2.Y > 0.2)
                {
                    vector2.Y = -0.2f;
                }
                if ((double)vector2.Y < -0.2)
                {
                    vector2.Y = -0.2f;
                }
            }
            int m = num3;
            int num15;
            for (m += WorldGen.genRand.Next(5); m < num4; m += WorldGen.genRand.Next(num15, (int)((double)num15 * 1.5)))
            {
                int num14 = num6;
                while (!Main.tile[m, num14].active())
                {
                    num14--;
                }
                num14 += WorldGen.genRand.Next(-3, 4);
                num15 = WorldGen.genRand.Next(4, 8);
                int num16 = WorldGen.crimson ? 400 : 401;
                if (WorldGen.genRand.Next(4) == 0)
                {
                    num16 = WorldGen.crimson ? 398 : 399;
                }
                for (int n = m - num15; n <= m + num15; n++)
                {
                    for (int num17 = num14 - num15; num17 <= num14 + num15; num17++)
                    {
                        if (num17 > num5)
                        {
                            float arg_409_0 = (float)Math.Abs(n - m);
                            float num18 = (float)(Math.Abs(num17 - num14) * 2);
                            if (Math.Sqrt((double)(arg_409_0 * arg_409_0 + num18 * num18)) < (double)(num15 + WorldGen.genRand.Next(2)))
                            {
                                Main.tile[n, num17].active(true);
                                Main.tile[n, num17].type = (ushort)num16;
                                WorldGen.SquareTileFrame(n, num17, true);
                            }
                        }
                    }
                }
            }
            num = (double)WorldGen.genRand.Next(80, 95);
            num2 = (float)WorldGen.genRand.Next(10, 15);
            vector.X = (float)i;
            vector.Y = (float)num5;
            vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            while (vector2.X > -2f && vector2.X < 2f)
            {
                vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            }
            vector2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.02f;
            while (num > 0.0 && num2 > 0f)
            {
                num -= (double)WorldGen.genRand.Next(4);
                num2 -= 1f;
                int num7 = (int)((double)vector.X - num * 0.5);
                int num8 = (int)((double)vector.X + num * 0.5);
                int num9 = num5 - 1;
                int num10 = (int)((double)vector.Y + num * 0.5);
                if (num7 < 0)
                {
                    num7 = 0;
                }
                if (num8 > Main.maxTilesX)
                {
                    num8 = Main.maxTilesX;
                }
                if (num9 < 0)
                {
                    num9 = 0;
                }
                if (num10 > Main.maxTilesY)
                {
                    num10 = Main.maxTilesY;
                }
                double num11 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01;
                float num19 = vector.Y + 1f;
                for (int num20 = num7; num20 < num8; num20++)
                {
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num19 += (float)WorldGen.genRand.Next(-1, 2);
                    }
                    if (num19 < vector.Y)
                    {
                        num19 = vector.Y;
                    }
                    if (num19 > vector.Y + 2f)
                    {
                        num19 = vector.Y + 2f;
                    }
                    for (int num21 = num9; num21 < num10; num21++)
                    {
                        if ((float)num21 > num19)
                        {
                            float arg_69E_0 = Math.Abs((float)num20 - vector.X);
                            float num22 = Math.Abs((float)num21 - vector.Y) * 3f;
                            if (Math.Sqrt((double)(arg_69E_0 * arg_69E_0 + num22 * num22)) < num11 * 0.4 &&
                                Main.tile[num20, num21].type == (WorldGen.crimson ? 400 : 401))
                            {
                                Main.tile[num20, num21].type = (ushort)(WorldGen.crimson ? 22 : 204); //ore
                                WorldGen.SquareTileFrame(num20, num21, true);
                            }
                        }
                    }
                }
                vector += vector2;
                vector2.X += (float)WorldGen.genRand.Next(-20, 21) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if ((double)vector2.Y > 0.2)
                {
                    vector2.Y = -0.2f;
                }
                if ((double)vector2.Y < -0.2)
                {
                    vector2.Y = -0.2f;
                }
            }
            int num23 = num3;
            num23 += WorldGen.genRand.Next(5);
            while (num23 < num4)
            {
                int num24 = num6;
                while ((!Main.tile[num23, num24].active() || Main.tile[num23, num24].type != 0) && num23 < num4)
                {
                    num24--;
                    if (num24 < num5)
                    {
                        num24 = num6;
                        num23 += WorldGen.genRand.Next(1, 4);
                    }
                }
                if (num23 < num4)
                {
                    num24 += WorldGen.genRand.Next(0, 4);
                    int num25 = WorldGen.genRand.Next(2, 5);
                    int num26 = WorldGen.crimson ? 400 : 401;
                    for (int num27 = num23 - num25; num27 <= num23 + num25; num27++)
                    {
                        for (int num28 = num24 - num25; num28 <= num24 + num25; num28++)
                        {
                            if (num28 > num5)
                            {
                                float arg_890_0 = (float)Math.Abs(num27 - num23);
                                float num29 = (float)(Math.Abs(num28 - num24) * 2);
                                if (Math.Sqrt((double)(arg_890_0 * arg_890_0 + num29 * num29)) < (double)num25)
                                {
                                    Main.tile[num27, num28].type = (ushort)num26;
                                    WorldGen.SquareTileFrame(num27, num28, true);
                                }
                            }
                        }
                    }
                    num23 += WorldGen.genRand.Next(num25, (int)((double)num25 * 1.5));
                }
            }
            for (int num30 = num3 - 20; num30 <= num4 + 20; num30++)
            {
                for (int num31 = num5 - 20; num31 <= num6 + 20; num31++)
                {
                    bool flag = true;
                    for (int num32 = num30 - 1; num32 <= num30 + 1; num32++)
                    {
                        for (int num33 = num31 - 1; num33 <= num31 + 1; num33++)
                        {
                            if (!Main.tile[num32, num33].active())
                            {
                                flag = false;
                            }
                        }
                    }
                    if (flag)
                    {
                        Main.tile[num30, num31].wall = (ushort)(WorldGen.crimson ? 220 : 221);
                        WorldGen.SquareWallFrame(num30, num31, true);
                    }
                }
            }
            for (int num34 = num3; num34 <= num4; num34++)
            {
                int num35 = num5 - 10;
                while (!Main.tile[num34, num35 + 1].active())
                {
                    num35++;
                }
                if (num35 < num6 && Main.tile[num34, num35 + 1].type == (WorldGen.crimson ? 400 : 401))
                {
                    if (WorldGen.genRand.Next(10) == 0)
                    {
                        int num36 = WorldGen.genRand.Next(1, 3);
                        for (int num37 = num34 - num36; num37 <= num34 + num36; num37++)
                        {
                            if (Main.tile[num37, num35].type == (WorldGen.crimson ? 400 : 401))
                            {
                                Main.tile[num37, num35].active(false);
                                Main.tile[num37, num35].liquid = 255;
                                Main.tile[num37, num35].lava(false);
                                WorldGen.SquareTileFrame(num34, num35, true);
                            }
                            if (Main.tile[num37, num35 + 1].type == (WorldGen.crimson ? 400 : 401))
                            {
                                Main.tile[num37, num35 + 1].active(false);
                                Main.tile[num37, num35 + 1].liquid = 255;
                                Main.tile[num37, num35 + 1].lava(false);
                                WorldGen.SquareTileFrame(num34, num35 + 1, true);
                            }
                            if (num37 > num34 - num36 && num37 < num34 + 2 && Main.tile[num37, num35 + 2].type == (WorldGen.crimson ? 400 : 401))
                            {
                                Main.tile[num37, num35 + 2].active(false);
                                Main.tile[num37, num35 + 2].liquid = 255;
                                Main.tile[num37, num35 + 2].lava(false);
                                WorldGen.SquareTileFrame(num34, num35 + 2, true);
                            }
                        }
                    }
                    if (WorldGen.genRand.Next(5) == 0)
                    {
                        Main.tile[num34, num35].liquid = 255;
                    }
                    Main.tile[num34, num35].lava(false);
                    WorldGen.SquareTileFrame(num34, num35, true);
                }
            }
            int num38 = WorldGen.genRand.Next(4);
            for (int num39 = 0; num39 <= num38; num39++)
            {
                int num40 = WorldGen.genRand.Next(num3 - 5, num4 + 5);
                int num41 = num5 - WorldGen.genRand.Next(20, 40);
                int num42 = WorldGen.genRand.Next(4, 8);
                int num43 = WorldGen.crimson ? 400 : 401;
                if (WorldGen.genRand.Next(2) == 0)
                {
                    num43 = WorldGen.crimson ? 398 : 399;
                }
                for (int num44 = num40 - num42; num44 <= num40 + num42; num44++)
                {
                    for (int num45 = num41 - num42; num45 <= num41 + num42; num45++)
                    {
                        float arg_C74_0 = (float)Math.Abs(num44 - num40);
                        float num46 = (float)(Math.Abs(num45 - num41) * 2);
                        if (Math.Sqrt((double)(arg_C74_0 * arg_C74_0 + num46 * num46)) < (double)(num42 + WorldGen.genRand.Next(-1, 2)))
                        {
                            Main.tile[num44, num45].active(true);
                            Main.tile[num44, num45].type = (ushort)num43;
                            WorldGen.SquareTileFrame(num44, num45, true);
                        }
                    }
                }
                for (int num47 = num40 - num42 + 2; num47 <= num40 + num42 - 2; num47++)
                {
                    int num48 = num41 - num42;
                    while (!Main.tile[num47, num48].active())
                    {
                        num48++;
                    }
                    Main.tile[num47, num48].active(false);
                    Main.tile[num47, num48].liquid = 255;
                    WorldGen.SquareTileFrame(num47, num48, true);
                }
            }
        }

        public static void EvilIslandHouse(int i, int j)
        {
            ushort type = (ushort)(WorldGen.crimson ? 152 : 347); //tile
            byte wall = (byte)(WorldGen.crimson ? 35 : 174); //wall
            Vector2 vector = new Vector2((float)i, (float)j);
            int num = 1;
            if (WorldGen.genRand.Next(2) == 0)
            {
                num = -1;
            }
            int num2 = WorldGen.genRand.Next(7, 12);
            int num3 = WorldGen.genRand.Next(5, 7);
            vector.X = (float)(i + (num2 + 2) * num);
            for (int k = j - 15; k < j + 30; k++)
            {
                if (Main.tile[(int)vector.X, k].active())
                {
                    vector.Y = (float)(k - 1);
                    break;
                }
            }
            vector.X = (float)i;
            int num4 = (int)(vector.X - (float)num2 - 1f);
            int num5 = (int)(vector.X + (float)num2 + 1f);
            int num6 = (int)(vector.Y - (float)num3 - 1f);
            int num7 = (int)(vector.Y + 2f);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int l = num4; l <= num5; l++)
            {
                for (int m = num6 - 1; m < num7 + 1; m++)
                {
                    if (m != num6 - 1 || (l != num4 && l != num5))
                    {
                        Main.tile[l, m].active(true);
                        Main.tile[l, m].liquid = 0;
                        Main.tile[l, m].type = type;
                        Main.tile[l, m].wall = 0;
                        Main.tile[l, m].halfBrick(false);
                        Main.tile[l, m].slope(0);
                    }
                }
            }
            num4 = (int)(vector.X - (float)num2);
            num5 = (int)(vector.X + (float)num2);
            num6 = (int)(vector.Y - (float)num3);
            num7 = (int)(vector.Y + 1f);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int n = num4; n <= num5; n++)
            {
                for (int num8 = num6; num8 < num7; num8++)
                {
                    if ((num8 != num6 || (n != num4 && n != num5)) && Main.tile[n, num8].wall == 0)
                    {
                        Main.tile[n, num8].active(false);
                        Main.tile[n, num8].wall = wall;
                    }
                }
            }
            int num9 = i + (num2 + 1) * num;
            int num10 = (int)vector.Y;
            for (int num11 = num9 - 2; num11 <= num9 + 2; num11++)
            {
                Main.tile[num11, num10].active(false);
                Main.tile[num11, num10 - 1].active(false);
                Main.tile[num11, num10 - 2].active(false);
            }
            WorldGen.PlaceTile(num9, num10, 10, true, false, -1, WorldGen.crimson ? 1 : 10); //door
            num9 = i + (num2 + 1) * -num - num;
            for (int num12 = num6; num12 <= num7 + 1; num12++)
            {
                Main.tile[num9, num12].active(true);
                Main.tile[num9, num12].liquid = 0;
                Main.tile[num9, num12].type = type;
                Main.tile[num9, num12].wall = 0;
                Main.tile[num9, num12].halfBrick(false);
                Main.tile[num9, num12].slope(0);
            }
            int contain;
            if (WorldGen.crimson)
            {
                contain = 1571; //scourge
            }
            else
            {
                contain = 1569; //vampire
            }
            WorldGen.AddBuriedChest(i, num10 - 3, contain, false, WorldGen.crimson ? 19 : 20); //chest
            int num14 = i - num2 / 2 + 1;
            int num15 = i + num2 / 2 - 1;
            int num16 = 1;
            if (num2 > 10)
            {
                num16 = 2;
            }
            int num17 = (num6 + num7) / 2 - 1;
            for (int num18 = num14 - num16; num18 <= num14 + num16; num18++)
            {
                for (int num19 = num17 - 1; num19 <= num17 + 1; num19++)
                {
                    Main.tile[num18, num19].wall = 21; //glass
                }
            }
            for (int num20 = num15 - num16; num20 <= num15 + num16; num20++)
            {
                for (int num21 = num17 - 1; num21 <= num17 + 1; num21++)
                {
                    Main.tile[num20, num21].wall = 21; //glass
                }
            }
            int num22 = i + (num2 / 2 + 1) * -num;
            WorldGen.PlaceTile(num22, num7 - 1, 14, true, false, -1, WorldGen.crimson ? 1 : 8); //table
            WorldGen.PlaceTile(num22 - 2, num7 - 1, 15, true, false, 0, WorldGen.crimson ? 2 : 11); //chair
            Tile tile = Main.tile[num22 - 2, num7 - 1];
            tile.frameX += 18;
            Tile tile2 = Main.tile[num22 - 2, num7 - 2];
            tile2.frameX += 18;
            WorldGen.PlaceTile(num22 + 2, num7 - 1, 15, true, false, 0, WorldGen.crimson ? 2 : 11); //chair
        }
    }
}
