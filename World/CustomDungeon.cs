using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Astral;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityMod.World
{
    public class CustomDungeon
    {
        public static void NewDungeon()
        {
            int dungeonLocation = GenVars.dungeonLocation;
            int randomY = (int)((Main.worldSurface + Main.rockLayer) / 2.0) + WorldGen.genRand.Next(-200, 200);
            int limit = (int)((Main.worldSurface + Main.rockLayer) / 2.0) + 200;
            int y = randomY;
            bool success = false;
            for (int i = 0; i < 10; i++)
            {
                if (WorldGen.SolidTile(dungeonLocation, y + i))
                {
                    success = true;
                    break;
                }
            }

            if (!success)
            {
                for (; y < limit && !WorldGen.SolidTile(dungeonLocation, y + 10); y++)
                {
                }
            }

            if (WorldGen.drunkWorldGen)
                y = (int)Main.worldSurface + 70;

            GenNewDungeon(dungeonLocation, y);
        }

        public static void GenNewDungeon(int x, int y)
        {
            GenVars.dEnteranceX = 0;
            GenVars.numDRooms = 0;
            GenVars.numDDoors = 0;
            GenVars.numDungeonPlatforms = 0;
            int num = WorldGen.genRand.Next(3);
            WorldGen.genRand.Next(3);
            if (WorldGen.remixWorldGen)
                num = (WorldGen.crimson ? 2 : 0);

            ushort num2;
            int num3;
            switch (num)
            {
                case 0:
                    num2 = 41;
                    num3 = 7;
                    GenVars.crackedType = 481;
                    break;
                case 1:
                    num2 = 43;
                    num3 = 8;
                    GenVars.crackedType = 482;
                    break;
                default:
                    num2 = 44;
                    num3 = 9;
                    GenVars.crackedType = 483;
                    break;
            }

            Main.tileSolid[GenVars.crackedType] = false;
            GenVars.dungeonLake = true;
            GenVars.numDDoors = 0;
            GenVars.numDungeonPlatforms = 0;
            GenVars.numDRooms = 0;
            GenVars.dungeonX = x;
            GenVars.dungeonY = y;

            // The horizontal adjustment Calamity makes.
            GenVars.dungeonX = Utils.Clamp(GenVars.dungeonX, SulphurousSea.BiomeWidth + 167, Main.maxTilesX - SulphurousSea.BiomeWidth - 167);

            // Adjust the Y position of the dungeon to accomodate for the X shift.
            WorldUtils.Find(new Point(GenVars.dungeonX, GenVars.dungeonY), Searches.Chain(new Searches.Down(9001), new Conditions.IsSolid()), out Point result);
            GenVars.dungeonY = result.Y - 10;

            GenVars.dMinX = x;
            GenVars.dMaxX = x;
            GenVars.dMinY = y;
            GenVars.dMaxY = y;
            GenVars.dxStrength1 = WorldGen.genRand.Next(25, 30);
            GenVars.dyStrength1 = WorldGen.genRand.Next(20, 25);
            GenVars.dxStrength2 = WorldGen.genRand.Next(35, 50);
            GenVars.dyStrength2 = WorldGen.genRand.Next(10, 15);
            double num4 = Main.maxTilesX / 60;
            num4 += (double)WorldGen.genRand.Next(0, (int)(num4 / 3.0));
            double num5 = num4;
            int num6 = 5;
            WorldGen.DungeonRoom(GenVars.dungeonX, GenVars.dungeonY, num2, num3);
            while (num4 > 0.0)
            {
                if (GenVars.dungeonX < GenVars.dMinX)
                    GenVars.dMinX = GenVars.dungeonX;

                if (GenVars.dungeonX > GenVars.dMaxX)
                    GenVars.dMaxX = GenVars.dungeonX;

                if (GenVars.dungeonY > GenVars.dMaxY)
                    GenVars.dMaxY = GenVars.dungeonY;

                num4 -= 1.0;
                Main.statusText = Lang.gen[58].Value + " " + (int)((num5 - num4) / num5 * 60.0) + "%";
                if (num6 > 0)
                    num6--;

                if ((num6 == 0) & (WorldGen.genRand.NextBool(3)))
                {
                    num6 = 5;
                    if (WorldGen.genRand.NextBool())
                    {
                        int dungeonX = GenVars.dungeonX;
                        int dungeonY = GenVars.dungeonY;
                        GenNewDungeonHalls(GenVars.dungeonX, GenVars.dungeonY, num2, num3);
                        if (WorldGen.genRand.NextBool())
                            GenNewDungeonHalls(GenVars.dungeonX, GenVars.dungeonY, num2, num3);

                        WorldGen.DungeonRoom(GenVars.dungeonX, GenVars.dungeonY, num2, num3);
                        GenVars.dungeonX = dungeonX;
                        GenVars.dungeonY = dungeonY;
                    }
                    else
                    {
                        WorldGen.DungeonRoom(GenVars.dungeonX, GenVars.dungeonY, num2, num3);
                    }
                }
                else
                {
                    WorldGen.DungeonHalls(GenVars.dungeonX, GenVars.dungeonY, num2, num3);
                }
            }

            WorldGen.DungeonRoom(GenVars.dungeonX, GenVars.dungeonY, num2, num3);
            int num7 = GenVars.dRoomX[0];
            int num8 = GenVars.dRoomY[0];
            for (int i = 0; i < GenVars.numDRooms; i++)
            {
                if (GenVars.dRoomY[i] < num8)
                {
                    num7 = GenVars.dRoomX[i];
                    num8 = GenVars.dRoomY[i];
                }
            }

            GenVars.dungeonX = num7;
            GenVars.dungeonY = num8;
            GenVars.dEnteranceX = num7;
            GenVars.dSurface = false;
            num6 = 5;
            if (WorldGen.drunkWorldGen)
                GenVars.dSurface = true;

            while (!GenVars.dSurface)
            {
                if (num6 > 0)
                    num6--;

                if (num6 == 0 && WorldGen.genRand.NextBool(5) && (double)GenVars.dungeonY > Main.worldSurface + 100.0)
                {
                    num6 = 10;
                    int dungeonX2 = GenVars.dungeonX;
                    int dungeonY2 = GenVars.dungeonY;
                    GenNewDungeonHalls(GenVars.dungeonX, GenVars.dungeonY, num2, num3, forceX: true);
                    WorldGen.DungeonRoom(GenVars.dungeonX, GenVars.dungeonY, num2, num3);
                    GenVars.dungeonX = dungeonX2;
                    GenVars.dungeonY = dungeonY2;
                }

                WorldGen.DungeonStairs(GenVars.dungeonX, GenVars.dungeonY, num2, num3);
            }

            WorldGen.DungeonEnt(GenVars.dungeonX, GenVars.dungeonY, num2, num3);
            Main.statusText = Lang.gen[58].Value + " 65%";
            int num9 = Main.maxTilesX * 2;
            int num10;
            for (num10 = 0; num10 < num9; num10++)
            {
                int i2 = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                int num11 = GenVars.dMinY;
                if ((double)num11 < Main.worldSurface)
                    num11 = (int)Main.worldSurface;

                int j = WorldGen.genRand.Next(num11, GenVars.dMaxY);
                num10 = ((!WorldGen.DungeonPitTrap(i2, j, num2, num3)) ? (num10 + 1) : (num10 + 1500));
            }

            for (int k = 0; k < GenVars.numDRooms; k++)
            {
                for (int l = GenVars.dRoomL[k]; l <= GenVars.dRoomR[k]; l++)
                {
                    if (!Main.tile[l, GenVars.dRoomT[k] - 1].Get<TileWallWireStateData>().HasTile)
                    {
                        GenVars.dungeonPlatformX[GenVars.numDungeonPlatforms] = l;
                        GenVars.dungeonPlatformY[GenVars.numDungeonPlatforms] = GenVars.dRoomT[k] - 1;
                        GenVars.numDungeonPlatforms++;
                        break;
                    }
                }

                for (int m = GenVars.dRoomL[k]; m <= GenVars.dRoomR[k]; m++)
                {
                    if (!Main.tile[m, GenVars.dRoomB[k] + 1].Get<TileWallWireStateData>().HasTile)
                    {
                        GenVars.dungeonPlatformX[GenVars.numDungeonPlatforms] = m;
                        GenVars.dungeonPlatformY[GenVars.numDungeonPlatforms] = GenVars.dRoomB[k] + 1;
                        GenVars.numDungeonPlatforms++;
                        break;
                    }
                }

                for (int n = GenVars.dRoomT[k]; n <= GenVars.dRoomB[k]; n++)
                {
                    if (!Main.tile[GenVars.dRoomL[k] - 1, n].Get<TileWallWireStateData>().HasTile)
                    {
                        GenVars.DDoorX[GenVars.numDDoors] = GenVars.dRoomL[k] - 1;
                        GenVars.DDoorY[GenVars.numDDoors] = n;
                        GenVars.DDoorPos[GenVars.numDDoors] = -1;
                        GenVars.numDDoors++;
                        break;
                    }
                }

                for (int num12 = GenVars.dRoomT[k]; num12 <= GenVars.dRoomB[k]; num12++)
                {
                    if (!Main.tile[GenVars.dRoomR[k] + 1, num12].Get<TileWallWireStateData>().HasTile)
                    {
                        GenVars.DDoorX[GenVars.numDDoors] = GenVars.dRoomR[k] + 1;
                        GenVars.DDoorY[GenVars.numDDoors] = num12;
                        GenVars.DDoorPos[GenVars.numDDoors] = 1;
                        GenVars.numDDoors++;
                        break;
                    }
                }
            }

            Main.statusText = Lang.gen[58].Value + " 70%";
            int num13 = 0;
            int num14 = 1000;
            int num15 = 0;
            int num16 = Main.maxTilesX / 200;
            if (WorldGen.getGoodWorldGen)
                num16 *= 6;
            if (Main.zenithWorld)
                num16 *= 4;

            while (num15 < num16)
            {
                num13++;
                int num17 = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                int num18 = WorldGen.genRand.Next((int)Main.worldSurface + 25, GenVars.dMaxY);
                if (WorldGen.drunkWorldGen)
                    num18 = WorldGen.genRand.Next(GenVars.dungeonY + 25, GenVars.dMaxY);

                int num19 = num17;
                if (Main.tile[num17, num18].WallType == num3 && !Main.tile[num17, num18].Get<TileWallWireStateData>().HasTile)
                {
                    int num20 = 1;
                    if (WorldGen.genRand.NextBool())
                        num20 = -1;

                    for (; !Main.tile[num17, num18].Get<TileWallWireStateData>().HasTile; num18 += num20)
                    {
                    }

                    if (Main.tile[num17 - 1, num18].Get<TileWallWireStateData>().HasTile && Main.tile[num17 + 1, num18].Get<TileWallWireStateData>().HasTile && Main.tile[num17 - 1, num18].TileType != GenVars.crackedType && !Main.tile[num17 - 1, num18 - num20].Get<TileWallWireStateData>().HasTile && !Main.tile[num17 + 1, num18 - num20].Get<TileWallWireStateData>().HasTile)
                    {
                        num15++;
                        int num21 = WorldGen.genRand.Next(5, 13);
                        while (Main.tile[num17 - 1, num18].Get<TileWallWireStateData>().HasTile && Main.tile[num17 - 1, num18].TileType != GenVars.crackedType && Main.tile[num17, num18 + num20].Get<TileWallWireStateData>().HasTile && Main.tile[num17, num18].Get<TileWallWireStateData>().HasTile && !Main.tile[num17, num18 - num20].Get<TileWallWireStateData>().HasTile && num21 > 0)
                        {
                            Main.tile[num17, num18].TileType = 48;
                            if (!Main.tile[num17 - 1, num18 - num20].Get<TileWallWireStateData>().HasTile && !Main.tile[num17 + 1, num18 - num20].Get<TileWallWireStateData>().HasTile)
                            {
                                Main.tile[num17, num18 - num20].Clear(TileDataType.Slope);
                                Main.tile[num17, num18 - num20].TileType = 48;
                                Main.tile[num17, num18 - num20].Get<TileWallWireStateData>().HasTile = true;
                                Main.tile[num17, num18 - num20 * 2].Clear(TileDataType.Slope);
                                Main.tile[num17, num18 - num20 * 2].TileType = 48;
                                Main.tile[num17, num18 - num20 * 2].Get<TileWallWireStateData>().HasTile = true;
                            }

                            num17--;
                            num21--;
                        }

                        num21 = WorldGen.genRand.Next(5, 13);
                        num17 = num19 + 1;
                        while (Main.tile[num17 + 1, num18].Get<TileWallWireStateData>().HasTile && Main.tile[num17 + 1, num18].TileType != GenVars.crackedType && Main.tile[num17, num18 + num20].Get<TileWallWireStateData>().HasTile && Main.tile[num17, num18].Get<TileWallWireStateData>().HasTile && !Main.tile[num17, num18 - num20].Get<TileWallWireStateData>().HasTile && num21 > 0)
                        {
                            Main.tile[num17, num18].TileType = 48;
                            if (!Main.tile[num17 - 1, num18 - num20].Get<TileWallWireStateData>().HasTile && !Main.tile[num17 + 1, num18 - num20].Get<TileWallWireStateData>().HasTile)
                            {
                                Main.tile[num17, num18 - num20].Clear(TileDataType.Slope);
                                Main.tile[num17, num18 - num20].TileType = 48;
                                Main.tile[num17, num18 - num20].Get<TileWallWireStateData>().HasTile = true;
                                Main.tile[num17, num18 - num20 * 2].Clear(TileDataType.Slope);
                                Main.tile[num17, num18 - num20 * 2].TileType = 48;
                                Main.tile[num17, num18 - num20 * 2].Get<TileWallWireStateData>().HasTile = true;
                            }

                            num17++;
                            num21--;
                        }
                    }
                }

                if (num13 > num14)
                {
                    num13 = 0;
                    num15++;
                }
            }

            num13 = 0;
            num14 = 1000;
            num15 = 0;
            Main.statusText = Lang.gen[58].Value + " 75%";
            while (num15 < num16)
            {
                num13++;
                int num22 = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                int num23 = WorldGen.genRand.Next((int)Main.worldSurface + 25, GenVars.dMaxY);
                int num24 = num23;
                if (Main.tile[num22, num23].WallType == num3 && !Main.tile[num22, num23].Get<TileWallWireStateData>().HasTile)
                {
                    int num25 = 1;
                    if (WorldGen.genRand.NextBool())
                        num25 = -1;

                    for (; num22 > 5 && num22 < Main.maxTilesX - 5 && !Main.tile[num22, num23].Get<TileWallWireStateData>().HasTile; num22 += num25)
                    {
                    }

                    if (Main.tile[num22, num23 - 1].Get<TileWallWireStateData>().HasTile && Main.tile[num22, num23 + 1].Get<TileWallWireStateData>().HasTile && Main.tile[num22, num23 - 1].TileType != GenVars.crackedType && !Main.tile[num22 - num25, num23 - 1].Get<TileWallWireStateData>().HasTile && !Main.tile[num22 - num25, num23 + 1].Get<TileWallWireStateData>().HasTile)
                    {
                        num15++;
                        int num26 = WorldGen.genRand.Next(5, 13);
                        while (Main.tile[num22, num23 - 1].Get<TileWallWireStateData>().HasTile && Main.tile[num22, num23 - 1].TileType != GenVars.crackedType && Main.tile[num22 + num25, num23].Get<TileWallWireStateData>().HasTile && Main.tile[num22, num23].Get<TileWallWireStateData>().HasTile && !Main.tile[num22 - num25, num23].Get<TileWallWireStateData>().HasTile && num26 > 0)
                        {
                            Main.tile[num22, num23].TileType = 48;
                            if (!Main.tile[num22 - num25, num23 - 1].Get<TileWallWireStateData>().HasTile && !Main.tile[num22 - num25, num23 + 1].Get<TileWallWireStateData>().HasTile)
                            {
                                Main.tile[num22 - num25, num23].TileType = 48;
                                Main.tile[num22 - num25, num23].Get<TileWallWireStateData>().HasTile = true;
                                Main.tile[num22 - num25, num23].Clear(TileDataType.Slope);
                                Main.tile[num22 - num25 * 2, num23].TileType = 48;
                                Main.tile[num22 - num25 * 2, num23].Get<TileWallWireStateData>().HasTile = true;
                                Main.tile[num22 - num25 * 2, num23].Clear(TileDataType.Slope);
                            }

                            num23--;
                            num26--;
                        }

                        num26 = WorldGen.genRand.Next(5, 13);
                        num23 = num24 + 1;
                        while (Main.tile[num22, num23 + 1].Get<TileWallWireStateData>().HasTile && Main.tile[num22, num23 + 1].TileType != GenVars.crackedType && Main.tile[num22 + num25, num23].Get<TileWallWireStateData>().HasTile && Main.tile[num22, num23].Get<TileWallWireStateData>().HasTile && !Main.tile[num22 - num25, num23].Get<TileWallWireStateData>().HasTile && num26 > 0)
                        {
                            Main.tile[num22, num23].TileType = 48;
                            if (!Main.tile[num22 - num25, num23 - 1].Get<TileWallWireStateData>().HasTile && !Main.tile[num22 - num25, num23 + 1].Get<TileWallWireStateData>().HasTile)
                            {
                                Main.tile[num22 - num25, num23].TileType = 48;
                                Main.tile[num22 - num25, num23].Get<TileWallWireStateData>().HasTile = true;
                                Main.tile[num22 - num25, num23].Clear(TileDataType.Slope);
                                Main.tile[num22 - num25 * 2, num23].TileType = 48;
                                Main.tile[num22 - num25 * 2, num23].Get<TileWallWireStateData>().HasTile = true;
                                Main.tile[num22 - num25 * 2, num23].Clear(TileDataType.Slope);
                            }

                            num23++;
                            num26--;
                        }
                    }
                }

                if (num13 > num14)
                {
                    num13 = 0;
                    num15++;
                }
            }

            Main.statusText = Lang.gen[58].Value + " 80%";
            for (int num27 = 0; num27 < GenVars.numDDoors; num27++)
            {
                int num28 = GenVars.DDoorX[num27] - 10;
                int num29 = GenVars.DDoorX[num27] + 10;
                int num30 = 100;
                int num31 = 0;
                int num32 = 0;
                int num33 = 0;
                for (int num34 = num28; num34 < num29; num34++)
                {
                    bool flag = true;
                    int num35 = GenVars.DDoorY[num27];
                    while (num35 > 10 && !Main.tile[num34, num35].Get<TileWallWireStateData>().HasTile)
                    {
                        num35--;
                    }

                    if (!Main.tileDungeon[Main.tile[num34, num35].TileType])
                        flag = false;

                    num32 = num35;
                    for (num35 = GenVars.DDoorY[num27]; !Main.tile[num34, num35].Get<TileWallWireStateData>().HasTile; num35++)
                    {
                    }

                    if (!Main.tileDungeon[Main.tile[num34, num35].TileType])
                        flag = false;

                    num33 = num35;
                    if (num33 - num32 < 3)
                        continue;

                    int num36 = num34 - 20;
                    int num37 = num34 + 20;
                    int num38 = num33 - 10;
                    int num39 = num33 + 10;
                    for (int num40 = num36; num40 < num37; num40++)
                    {
                        for (int num41 = num38; num41 < num39; num41++)
                        {
                            if (Main.tile[num40, num41].Get<TileWallWireStateData>().HasTile && Main.tile[num40, num41].TileType == 10)
                            {
                                flag = false;
                                break;
                            }
                        }
                    }

                    if (flag)
                    {
                        for (int num42 = num33 - 3; num42 < num33; num42++)
                        {
                            for (int num43 = num34 - 3; num43 <= num34 + 3; num43++)
                            {
                                if (Main.tile[num43, num42].Get<TileWallWireStateData>().HasTile)
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }
                    }

                    if (flag && num33 - num32 < 20)
                    {
                        bool flag2 = false;
                        if (GenVars.DDoorPos[num27] == 0 && num33 - num32 < num30)
                            flag2 = true;

                        if (GenVars.DDoorPos[num27] == -1 && num34 > num31)
                            flag2 = true;

                        if (GenVars.DDoorPos[num27] == 1 && (num34 < num31 || num31 == 0))
                            flag2 = true;

                        if (flag2)
                        {
                            num31 = num34;
                            num30 = num33 - num32;
                        }
                    }
                }

                if (num30 >= 20)
                    continue;

                int num44 = num31;
                int num45 = GenVars.DDoorY[num27];
                int num46 = num45;
                for (; !Main.tile[num44, num45].Get<TileWallWireStateData>().HasTile; num45++)
                {
                    Main.tile[num44, num45].Get<TileWallWireStateData>().HasTile = false;
                }

                while (!Main.tile[num44, num46].Get<TileWallWireStateData>().HasTile)
                {
                    num46--;
                }

                num45--;
                num46++;
                for (int num47 = num46; num47 < num45 - 2; num47++)
                {
                    Main.tile[num44, num47].Clear(TileDataType.Slope);
                    Main.tile[num44, num47].Get<TileWallWireStateData>().HasTile = true;
                    Main.tile[num44, num47].TileType = num2;
                    if (Main.tile[num44 - 1, num47].TileType == num2)
                    {
                        Main.tile[num44 - 1, num47].Get<TileWallWireStateData>().HasTile = false;
                        Main.tile[num44 - 1, num47].ClearEverything();
                        Main.tile[num44 - 1, num47].WallType = (ushort)num3;
                    }

                    if (Main.tile[num44 - 2, num47].TileType == num2)
                    {
                        Main.tile[num44 - 2, num47].Get<TileWallWireStateData>().HasTile = false;
                        Main.tile[num44 - 2, num47].ClearEverything();
                        Main.tile[num44 - 2, num47].WallType = (ushort)num3;
                    }

                    if (Main.tile[num44 + 1, num47].TileType == num2)
                    {
                        Main.tile[num44 + 1, num47].Get<TileWallWireStateData>().HasTile = false;
                        Main.tile[num44 + 1, num47].ClearEverything();
                        Main.tile[num44 + 1, num47].WallType = (ushort)num3;
                    }

                    if (Main.tile[num44 + 2, num47].TileType == num2)
                    {
                        Main.tile[num44 + 2, num47].Get<TileWallWireStateData>().HasTile = false;
                        Main.tile[num44 + 2, num47].ClearEverything();
                        Main.tile[num44 + 2, num47].WallType = (ushort)num3;
                    }
                }

                int style = 13;
                if (WorldGen.genRand.NextBool(3))
                {
                    switch (num3)
                    {
                        case 7:
                            style = 16;
                            break;
                        case 8:
                            style = 17;
                            break;
                        case 9:
                            style = 18;
                            break;
                    }
                }

                WorldGen.PlaceTile(num44, num45, 10, mute: true, forced: false, -1, style);
                num44--;
                int num48 = num45 - 3;
                while (!Main.tile[num44, num48].Get<TileWallWireStateData>().HasTile)
                {
                    num48--;
                }

                if (num45 - num48 < num45 - num46 + 5 && Main.tileDungeon[Main.tile[num44, num48].TileType])
                {
                    for (int num49 = num45 - 4 - WorldGen.genRand.Next(3); num49 > num48; num49--)
                    {
                        Main.tile[num44, num49].Clear(TileDataType.Slope);
                        Main.tile[num44, num49].Get<TileWallWireStateData>().HasTile = true;
                        Main.tile[num44, num49].TileType = num2;
                        if (Main.tile[num44 - 1, num49].TileType == num2)
                        {
                            Main.tile[num44 - 1, num49].Get<TileWallWireStateData>().HasTile = false;
                            Main.tile[num44 - 1, num49].ClearEverything();
                            Main.tile[num44 - 1, num49].WallType = (ushort)num3;
                        }

                        if (Main.tile[num44 - 2, num49].TileType == num2)
                        {
                            Main.tile[num44 - 2, num49].Get<TileWallWireStateData>().HasTile = false;
                            Main.tile[num44 - 2, num49].ClearEverything();
                            Main.tile[num44 - 2, num49].WallType = (ushort)num3;
                        }
                    }
                }

                num44 += 2;
                num48 = num45 - 3;
                while (!Main.tile[num44, num48].Get<TileWallWireStateData>().HasTile)
                {
                    num48--;
                }

                if (num45 - num48 < num45 - num46 + 5 && Main.tileDungeon[Main.tile[num44, num48].TileType])
                {
                    for (int num50 = num45 - 4 - WorldGen.genRand.Next(3); num50 > num48; num50--)
                    {
                        Main.tile[num44, num50].Get<TileWallWireStateData>().HasTile = true;
                        Main.tile[num44, num50].Clear(TileDataType.Slope);
                        Main.tile[num44, num50].TileType = num2;
                        if (Main.tile[num44 + 1, num50].TileType == num2)
                        {
                            Main.tile[num44 + 1, num50].Get<TileWallWireStateData>().HasTile = false;
                            Main.tile[num44 + 1, num50].ClearEverything();
                            Main.tile[num44 + 1, num50].WallType = (ushort)num3;
                        }

                        if (Main.tile[num44 + 2, num50].TileType == num2)
                        {
                            Main.tile[num44 + 2, num50].Get<TileWallWireStateData>().HasTile = false;
                            Main.tile[num44 + 2, num50].ClearEverything();
                            Main.tile[num44 + 2, num50].WallType = (ushort)num3;
                        }
                    }
                }

                num45++;
                num44--;
                for (int num51 = num45 - 8; num51 < num45; num51++)
                {
                    if (Main.tile[num44 + 2, num51].TileType == num2)
                    {
                        Main.tile[num44 + 2, num51].Get<TileWallWireStateData>().HasTile = false;
                        Main.tile[num44 + 2, num51].ClearEverything();
                        Main.tile[num44 + 2, num51].WallType = (ushort)num3;
                    }

                    if (Main.tile[num44 + 3, num51].TileType == num2)
                    {
                        Main.tile[num44 + 3, num51].Get<TileWallWireStateData>().HasTile = false;
                        Main.tile[num44 + 3, num51].ClearEverything();
                        Main.tile[num44 + 3, num51].WallType = (ushort)num3;
                    }

                    if (Main.tile[num44 - 2, num51].TileType == num2)
                    {
                        Main.tile[num44 - 2, num51].Get<TileWallWireStateData>().HasTile = false;
                        Main.tile[num44 - 2, num51].ClearEverything();
                        Main.tile[num44 - 2, num51].WallType = (ushort)num3;
                    }

                    if (Main.tile[num44 - 3, num51].TileType == num2)
                    {
                        Main.tile[num44 - 3, num51].Get<TileWallWireStateData>().HasTile = false;
                        Main.tile[num44 - 3, num51].ClearEverything();
                        Main.tile[num44 - 3, num51].WallType = (ushort)num3;
                    }
                }

                Main.tile[num44 - 1, num45].Get<TileWallWireStateData>().HasTile = true;
                Main.tile[num44 - 1, num45].TileType = num2;
                Main.tile[num44 - 1, num45].Clear(TileDataType.Slope);
                Main.tile[num44 + 1, num45].Get<TileWallWireStateData>().HasTile = true;
                Main.tile[num44 + 1, num45].TileType = num2;
                Main.tile[num44 + 1, num45].Clear(TileDataType.Slope);
            }

            int[] array = new int[3];
            switch (num3)
            {
                case 7:
                    array[0] = 7;
                    array[1] = 94;
                    array[2] = 95;
                    break;
                case 9:
                    array[0] = 9;
                    array[1] = 96;
                    array[2] = 97;
                    break;
                default:
                    array[0] = 8;
                    array[1] = 98;
                    array[2] = 99;
                    break;
            }

            for (int num52 = 0; num52 < 5; num52++)
            {
                for (int num53 = 0; num53 < 3; num53++)
                {
                    int num54 = WorldGen.genRand.Next(40, 240);
                    int num55 = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                    int num56 = WorldGen.genRand.Next(GenVars.dMinY, GenVars.dMaxY);
                    for (int num57 = num55 - num54; num57 < num55 + num54; num57++)
                    {
                        for (int num58 = num56 - num54; num58 < num56 + num54; num58++)
                        {
                            if ((double)num58 > Main.worldSurface)
                            {
                                double num59 = Math.Abs(num55 - num57);
                                double num60 = Math.Abs(num56 - num58);
                                if (Math.Sqrt(num59 * num59 + num60 * num60) < (double)num54 * 0.4 && Main.wallDungeon[Main.tile[num57, num58].WallType])
                                    WorldGen.Spread.WallDungeon(num57, num58, array[num53]);
                            }
                        }
                    }
                }
            }

            Main.statusText = Lang.gen[58].Value + " 85%";
            for (int num61 = 0; num61 < GenVars.numDungeonPlatforms; num61++)
            {
                int num62 = GenVars.dungeonPlatformX[num61];
                int num63 = GenVars.dungeonPlatformY[num61];
                int num64 = Main.maxTilesX;
                int num65 = 10;
                if ((double)num63 < Main.worldSurface + 50.0)
                    num65 = 20;

                for (int num66 = num63 - 5; num66 <= num63 + 5; num66++)
                {
                    int num67 = num62;
                    int num68 = num62;
                    bool flag3 = false;
                    if (Main.tile[num67, num66].Get<TileWallWireStateData>().HasTile)
                    {
                        flag3 = true;
                    }
                    else
                    {
                        while (!Main.tile[num67, num66].Get<TileWallWireStateData>().HasTile)
                        {
                            num67--;
                            if (!Main.tileDungeon[Main.tile[num67, num66].TileType] || num67 == 0)
                            {
                                flag3 = true;
                                break;
                            }
                        }

                        while (!Main.tile[num68, num66].Get<TileWallWireStateData>().HasTile)
                        {
                            num68++;
                            if (!Main.tileDungeon[Main.tile[num68, num66].TileType] || num68 == Main.maxTilesX - 1)
                            {
                                flag3 = true;
                                break;
                            }
                        }
                    }

                    if (flag3 || num68 - num67 > num65)
                        continue;

                    bool flag4 = true;
                    int num69 = num62 - num65 / 2 - 2;
                    int num70 = num62 + num65 / 2 + 2;
                    int num71 = num66 - 5;
                    int num72 = num66 + 5;
                    for (int num73 = num69; num73 <= num70; num73++)
                    {
                        for (int num74 = num71; num74 <= num72; num74++)
                        {
                            if (Main.tile[num73, num74].Get<TileWallWireStateData>().HasTile && Main.tile[num73, num74].TileType == 19)
                            {
                                flag4 = false;
                                break;
                            }
                        }
                    }

                    for (int num75 = num66 + 3; num75 >= num66 - 5; num75--)
                    {
                        if (Main.tile[num62, num75].Get<TileWallWireStateData>().HasTile)
                        {
                            flag4 = false;
                            break;
                        }
                    }

                    if (flag4)
                    {
                        num64 = num66;
                        break;
                    }
                }

                if (num64 <= num63 - 10 || num64 >= num63 + 10)
                    continue;

                int num76 = num62;
                int num77 = num64;
                int num78 = num62 + 1;
                while (!Main.tile[num76, num77].Get<TileWallWireStateData>().HasTile)
                {
                    Main.tile[num76, num77].Get<TileWallWireStateData>().HasTile = true;
                    Main.tile[num76, num77].TileType = 19;
                    Main.tile[num76, num77].Clear(TileDataType.Slope);
                    switch (num3)
                    {
                        case 7:
                            Main.tile[num76, num77].TileFrameY = 108;
                            break;
                        case 8:
                            Main.tile[num76, num77].TileFrameY = 144;
                            break;
                        default:
                            Main.tile[num76, num77].TileFrameY = 126;
                            break;
                    }

                    WorldGen.TileFrame(num76, num77);
                    num76--;
                }

                for (; !Main.tile[num78, num77].Get<TileWallWireStateData>().HasTile; num78++)
                {
                    Main.tile[num78, num77].Get<TileWallWireStateData>().HasTile = true;
                    Main.tile[num78, num77].TileType = 19;
                    Main.tile[num78, num77].Clear(TileDataType.Slope);
                    switch (num3)
                    {
                        case 7:
                            Main.tile[num78, num77].TileFrameY = 108;
                            break;
                        case 8:
                            Main.tile[num78, num77].TileFrameY = 144;
                            break;
                        default:
                            Main.tile[num78, num77].TileFrameY = 126;
                            break;
                    }

                    WorldGen.TileFrame(num78, num77);
                }
            }

            int num79 = 5;
            if (WorldGen.drunkWorldGen)
                num79 = 6;

            for (int num80 = 0; num80 < num79; num80++)
            {
                bool flag5 = false;
                while (!flag5)
                {
                    int num81 = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                    int num82 = WorldGen.genRand.Next((int)Main.worldSurface, GenVars.dMaxY);
                    if (!Main.wallDungeon[Main.tile[num81, num82].WallType] || Main.tile[num81, num82].Get<TileWallWireStateData>().HasTile)
                        continue;

                    ushort chestTileType = 21;
                    int contain = 0;
                    int style2 = 0;
                    switch (num80)
                    {
                        case 0:
                            style2 = 23;
                            contain = 1156;
                            break;
                        case 1:
                            if (!WorldGen.crimson)
                            {
                                style2 = 24;
                                contain = 1571;
                            }
                            else
                            {
                                style2 = 25;
                                contain = 1569;
                            }
                            break;
                        case 5:
                            if (WorldGen.crimson)
                            {
                                style2 = 24;
                                contain = 1571;
                            }
                            else
                            {
                                style2 = 25;
                                contain = 1569;
                            }
                            break;
                        case 2:
                            style2 = 26;
                            contain = 1260;
                            break;
                        case 3:
                            style2 = 27;
                            contain = 1572;
                            break;
                        case 4:
                            chestTileType = 467;
                            style2 = 13;
                            contain = 4607;
                            break;
                    }

                    flag5 = WorldGen.AddBuriedChest(num81, num82, contain, notNearOtherChests: false, style2, trySlope: false, chestTileType);
                }
            }

            int[] array2 = new int[3]
            {
                WorldGen.genRand.Next(9, 13),
                WorldGen.genRand.Next(9, 13),
                0
            };

            while (array2[1] == array2[0])
            {
                array2[1] = WorldGen.genRand.Next(9, 13);
            }

            array2[2] = WorldGen.genRand.Next(9, 13);
            while (array2[2] == array2[0] || array2[2] == array2[1])
            {
                array2[2] = WorldGen.genRand.Next(9, 13);
            }

            Main.statusText = Lang.gen[58].Value + " 90%";
            num13 = 0;
            num14 = 1000;
            num15 = 0;
            while (num15 < Main.maxTilesX / 20)
            {
                num13++;
                int num83 = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                int num84 = WorldGen.genRand.Next(GenVars.dMinY, GenVars.dMaxY);
                bool flag6 = true;
                if (Main.wallDungeon[Main.tile[num83, num84].WallType] && !Main.tile[num83, num84].Get<TileWallWireStateData>().HasTile)
                {
                    int num85 = 1;
                    if (WorldGen.genRand.NextBool())
                        num85 = -1;

                    while (flag6 && !Main.tile[num83, num84].Get<TileWallWireStateData>().HasTile)
                    {
                        num83 -= num85;
                        if (num83 < 5 || num83 > Main.maxTilesX - 5)
                            flag6 = false;
                        else if (Main.tile[num83, num84].Get<TileWallWireStateData>().HasTile && !Main.tileDungeon[Main.tile[num83, num84].TileType])
                            flag6 = false;
                    }

                    if (flag6 && Main.tile[num83, num84].Get<TileWallWireStateData>().HasTile && Main.tileDungeon[Main.tile[num83, num84].TileType] && Main.tile[num83, num84 - 1].Get<TileWallWireStateData>().HasTile && Main.tileDungeon[Main.tile[num83, num84 - 1].TileType] && Main.tile[num83, num84 + 1].Get<TileWallWireStateData>().HasTile && Main.tileDungeon[Main.tile[num83, num84 + 1].TileType])
                    {
                        num83 += num85;
                        for (int num86 = num83 - 3; num86 <= num83 + 3; num86++)
                        {
                            for (int num87 = num84 - 3; num87 <= num84 + 3; num87++)
                            {
                                if (Main.tile[num86, num87].Get<TileWallWireStateData>().HasTile && Main.tile[num86, num87].TileType == 19)
                                {
                                    flag6 = false;
                                    break;
                                }
                            }
                        }

                        if (flag6 && (!Main.tile[num83, num84 - 1].Get<TileWallWireStateData>().HasTile & !Main.tile[num83, num84 - 2].Get<TileWallWireStateData>().HasTile & !Main.tile[num83, num84 - 3].Get<TileWallWireStateData>().HasTile))
                        {
                            int num88 = num83;
                            int num89 = num83;
                            for (; num88 > GenVars.dMinX && num88 < GenVars.dMaxX && !Main.tile[num88, num84].Get<TileWallWireStateData>().HasTile && !Main.tile[num88, num84 - 1].Get<TileWallWireStateData>().HasTile && !Main.tile[num88, num84 + 1].Get<TileWallWireStateData>().HasTile; num88 += num85)
                            {
                            }

                            num88 = Math.Abs(num83 - num88);
                            bool flag7 = false;
                            if (WorldGen.genRand.NextBool())
                                flag7 = true;

                            if (num88 > 5)
                            {
                                for (int num90 = WorldGen.genRand.Next(1, 4); num90 > 0; num90--)
                                {
                                    Main.tile[num83, num84].Get<TileWallWireStateData>().HasTile = true;
                                    Main.tile[num83, num84].Clear(TileDataType.Slope);
                                    Main.tile[num83, num84].TileType = 19;
                                    if (Main.tile[num83, num84].WallType == array[0])
                                        Main.tile[num83, num84].TileFrameY = (short)(18 * array2[0]);
                                    else if (Main.tile[num83, num84].WallType == array[1])
                                        Main.tile[num83, num84].TileFrameY = (short)(18 * array2[1]);
                                    else
                                        Main.tile[num83, num84].TileFrameY = (short)(18 * array2[2]);

                                    WorldGen.TileFrame(num83, num84);
                                    if (flag7)
                                    {
                                        WorldGen.PlaceTile(num83, num84 - 1, 50, mute: true);
                                        if (WorldGen.genRand.NextBool(50) && (double)num84 > (Main.worldSurface + Main.rockLayer) / 2.0 && Main.tile[num83, num84 - 1].TileType == 50)
                                            Main.tile[num83, num84 - 1].TileFrameX = 90;
                                    }

                                    num83 += num85;
                                }

                                num13 = 0;
                                num15++;
                                if (!flag7 && WorldGen.genRand.NextBool())
                                {
                                    num83 = num89;
                                    num84--;
                                    int num91 = 0;
                                    if (WorldGen.genRand.NextBool(4))
                                        num91 = 1;

                                    switch (num91)
                                    {
                                        case 0:
                                            num91 = 13;
                                            break;
                                        case 1:
                                            num91 = 49;
                                            break;
                                    }

                                    WorldGen.PlaceTile(num83, num84, num91, mute: true);
                                    if (Main.tile[num83, num84].TileType == 13)
                                    {
                                        if (WorldGen.genRand.NextBool())
                                            Main.tile[num83, num84].TileFrameX = 18;
                                        else
                                            Main.tile[num83, num84].TileFrameX = 36;
                                    }
                                }
                            }
                        }
                    }
                }

                if (num13 > num14)
                {
                    num13 = 0;
                    num15++;
                }
            }

            Main.statusText = Lang.gen[58].Value + " 95%";
            int num92 = 1;
            for (int num93 = 0; num93 < GenVars.numDRooms; num93++)
            {
                int num94 = 0;
                while (num94 < 1000)
                {
                    int num95 = (int)((double)GenVars.dRoomSize[num93] * 0.4);
                    int i3 = GenVars.dRoomX[num93] + WorldGen.genRand.Next(-num95, num95 + 1);
                    int num96 = GenVars.dRoomY[num93] + WorldGen.genRand.Next(-num95, num95 + 1);
                    int num97 = 0;
                    int style3 = 2;
                    if (num92 == 1)
                        num92++;

                    switch (num92)
                    {
                        case 2:
                            num97 = 155;
                            break;
                        case 3:
                            num97 = 156;
                            break;
                        case 4:
                            num97 = ((!WorldGen.remixWorldGen) ? 157 : 2623);
                            break;
                        case 5:
                            num97 = 163;
                            break;
                        case 6:
                            num97 = 113;
                            break;
                        case 7:
                            num97 = 3317;
                            break;
                        case 8:
                            num97 = 327;
                            style3 = 0;
                            break;
                        default:
                            num97 = 164;
                            num92 = 0;
                            break;
                    }

                    if ((double)num96 < Main.worldSurface + 50.0)
                    {
                        num97 = 327;
                        style3 = 0;
                    }

                    if (num97 == 0 && WorldGen.genRand.NextBool())
                    {
                        num94 = 1000;
                        continue;
                    }

                    if (WorldGen.AddBuriedChest(i3, num96, num97, notNearOtherChests: false, style3, trySlope: false, 0))
                    {
                        num94 += 1000;
                        num92++;
                    }

                    num94++;
                }
            }

            GenVars.dMinX -= 25;
            GenVars.dMaxX += 25;
            GenVars.dMinY -= 25;
            GenVars.dMaxY += 25;
            if (GenVars.dMinX < 0)
                GenVars.dMinX = 0;

            if (GenVars.dMaxX > Main.maxTilesX)
                GenVars.dMaxX = Main.maxTilesX;

            if (GenVars.dMinY < 0)
                GenVars.dMinY = 0;

            if (GenVars.dMaxY > Main.maxTilesY)
                GenVars.dMaxY = Main.maxTilesY;

            num13 = 0;
            num14 = 1000;
            num15 = 0;
            MakeDungeon_Lights(num2, ref num13, num14, ref num15, array);
            num13 = 0;
            num14 = 1000;
            num15 = 0;
            MakeDungeon_Traps(ref num13, num14, ref num15);
            double count = MakeDungeon_GroundFurniture(num3);
            count = MakeDungeon_Pictures(array, count);
            count = MakeDungeon_Banners(array, count);

            // Get dungeon size field infos.
            int MinX = GenVars.dMinX + 25;
            int MaxX = GenVars.dMaxX - 25;
            int MaxY = GenVars.dMaxY - 25;

            int[] ChestTypes = { ModContent.TileType<AstralChestLocked>() };
            int[] ItemTypes = { ModContent.ItemType<HeavenfallenStardisk>() };
            int[] ChestStyles = { 1 }; // Astral Chest generates in style 1, which is locked

            for (int i = 0; i < ChestTypes.Length; ++i)
            {
                Chest chest = null;
                int attempts = 0;

                // Try 1000 times to place the chest somewhere in the dungeon.
                // The placement algorithm ensures that if it tries to appear in midair, it is moved down to the floor.
                while (chest == null && attempts < 1000)
                {
                    attempts++;
                    int x2 = WorldGen.genRand.Next(MinX, MaxX);
                    int y2 = WorldGen.genRand.Next((int)Main.worldSurface, MaxY);
                    if (Main.wallDungeon[Main.tile[x2, y2].WallType] && !Main.tile[x2, y2].HasTile)
                        chest = MiscWorldgenRoutines.AddChestWithLoot(x2, y2, (ushort)ChestTypes[i], tileStyle: ChestStyles[i]);
                }

                // If a chest was placed, force its first item to be the unique Biome Chest weapon.
                if (chest != null)
                {
                    chest.item[0].SetDefaults(ItemTypes[i]);
                    chest.item[0].Prefix(-1);
                }
            }
        }

        public static void MakeDungeon_Traps(ref int failCount, int failMax, ref int numAdd)
        {
            while (numAdd < Main.maxTilesX / 500)
            {
                failCount++;
                int num = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                int num2 = WorldGen.genRand.Next(GenVars.dMinY, GenVars.dMaxY);
                while ((double)num2 < Main.worldSurface)
                {
                    num2 = WorldGen.genRand.Next(GenVars.dMinY, GenVars.dMaxY);
                }

                if (Main.wallDungeon[Main.tile[num, num2].WallType] && WorldGen.placeTrap(num, num2, 0))
                    failCount = failMax;

                if (failCount > failMax)
                {
                    numAdd++;
                    failCount = 0;
                }
            }
        }

        public static void MakeDungeon_Lights(ushort tileType, ref int failCount, int failMax, ref int numAdd, int[] roomWall)
        {
            int[] array = new int[3]
            {
                WorldGen.genRand.Next(7),
                WorldGen.genRand.Next(7),
                0
            };

            while (array[1] == array[0])
            {
                array[1] = WorldGen.genRand.Next(7);
            }

            array[2] = WorldGen.genRand.Next(7);
            while (array[2] == array[0] || array[2] == array[1])
            {
                array[2] = WorldGen.genRand.Next(7);
            }

            while (numAdd < Main.maxTilesX / 150)
            {
                failCount++;
                int num = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                int num2 = WorldGen.genRand.Next(GenVars.dMinY, GenVars.dMaxY);
                if (Main.wallDungeon[Main.tile[num, num2].WallType])
                {
                    for (int num3 = num2; num3 > GenVars.dMinY; num3--)
                    {
                        if (Main.tile[num, num3 - 1].HasTile && Main.tile[num, num3 - 1].TileType == tileType)
                        {
                            bool flag = false;
                            for (int i = num - 15; i < num + 15; i++)
                            {
                                for (int j = num3 - 15; j < num3 + 15; j++)
                                {
                                    if (i > 0 && i < Main.maxTilesX && j > 0 && j < Main.maxTilesY && (Main.tile[i, j].TileType == 42 || Main.tile[i, j].TileType == 34))
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                            }

                            if (Main.tile[num - 1, num3].HasTile || Main.tile[num + 1, num3].HasTile || Main.tile[num - 1, num3 + 1].HasTile || Main.tile[num + 1, num3 + 1].HasTile || Main.tile[num, num3 + 2].HasTile)
                                flag = true;

                            if (flag)
                                break;

                            bool flag2 = false;
                            if (!flag2 && WorldGen.genRand.NextBool(7))
                            {
                                int style = 27;
                                switch (roomWall[0])
                                {
                                    case 7:
                                        style = 27;
                                        break;
                                    case 8:
                                        style = 28;
                                        break;
                                    case 9:
                                        style = 29;
                                        break;
                                }

                                bool flag3 = false;
                                for (int k = 0; k < 15; k++)
                                {
                                    if (WorldGen.SolidTile(num, num3 + k))
                                    {
                                        flag3 = true;
                                        break;
                                    }
                                }

                                if (!flag3)
                                    WorldGen.PlaceChand(num, num3, 34, style);

                                if (Main.tile[num, num3].TileType == 34)
                                {
                                    flag2 = true;
                                    failCount = 0;
                                    numAdd++;
                                    for (int l = 0; l < 1000; l++)
                                    {
                                        int num4 = num + WorldGen.genRand.Next(-12, 13);
                                        int num5 = num3 + WorldGen.genRand.Next(3, 21);
                                        if (Main.tile[num4, num5].HasTile || Main.tile[num4, num5 + 1].HasTile || !Main.tileDungeon[Main.tile[num4 - 1, num5].TileType] || !Main.tileDungeon[Main.tile[num4 + 1, num5].TileType] || !Collision.CanHit(new Point(num4 * 16, num5 * 16), 16, 16, new Point(num * 16, num3 * 16 + 1), 16, 16))
                                            continue;

                                        if (((WorldGen.SolidTile(num4 - 1, num5) && Main.tile[num4 - 1, num5].TileType != 10) || (WorldGen.SolidTile(num4 + 1, num5) && Main.tile[num4 + 1, num5].TileType != 10) || WorldGen.SolidTile(num4, num5 + 1)) && Main.wallDungeon[Main.tile[num4, num5].WallType] && (Main.tileDungeon[Main.tile[num4 - 1, num5].TileType] || Main.tileDungeon[Main.tile[num4 + 1, num5].TileType]))
                                            WorldGen.PlaceTile(num4, num5, 136, mute: true);

                                        if (!Main.tile[num4, num5].HasTile)
                                            continue;

                                        while (num4 != num || num5 != num3)
                                        {
                                            Main.tile[num4, num5].Get<TileWallWireStateData>().RedWire = true;
                                            if (num4 > num)
                                                num4--;

                                            if (num4 < num)
                                                num4++;

                                            Main.tile[num4, num5].Get<TileWallWireStateData>().RedWire = true;
                                            if (num5 > num3)
                                                num5--;

                                            if (num5 < num3)
                                                num5++;

                                            Main.tile[num4, num5].Get<TileWallWireStateData>().RedWire = true;
                                        }

                                        if (WorldGen.genRand.Next(3) > 0)
                                        {
                                            Main.tile[num, num3].TileFrameX = 18;
                                            Main.tile[num, num3 + 1].TileFrameX = 18;
                                        }

                                        break;
                                    }
                                }
                            }

                            if (flag2)
                                break;

                            int style2 = array[0];
                            if (Main.tile[num, num3].WallType == roomWall[1])
                                style2 = array[1];

                            if (Main.tile[num, num3].WallType == roomWall[2])
                                style2 = array[2];

                            WorldGen.Place1x2Top(num, num3, 42, style2);
                            if (Main.tile[num, num3].TileType != 42)
                                break;

                            flag2 = true;
                            failCount = 0;
                            numAdd++;
                            for (int m = 0; m < 1000; m++)
                            {
                                int num6 = num + WorldGen.genRand.Next(-12, 13);
                                int num7 = num3 + WorldGen.genRand.Next(3, 21);
                                if (Main.tile[num6, num7].HasTile || Main.tile[num6, num7 + 1].HasTile || Main.tile[num6 - 1, num7].TileType == 48 || Main.tile[num6 + 1, num7].TileType == 48 || !Collision.CanHit(new Point(num6 * 16, num7 * 16), 16, 16, new Point(num * 16, num3 * 16 + 1), 16, 16))
                                    continue;

                                if ((WorldGen.SolidTile(num6 - 1, num7) && Main.tile[num6 - 1, num7].TileType != 10) || (WorldGen.SolidTile(num6 + 1, num7) && Main.tile[num6 + 1, num7].TileType != 10) || WorldGen.SolidTile(num6, num7 + 1))
                                    WorldGen.PlaceTile(num6, num7, 136, mute: true);

                                if (!Main.tile[num6, num7].HasTile)
                                    continue;

                                while (num6 != num || num7 != num3)
                                {
                                    Main.tile[num6, num7].Get<TileWallWireStateData>().RedWire = true;
                                    if (num6 > num)
                                        num6--;

                                    if (num6 < num)
                                        num6++;

                                    Main.tile[num6, num7].Get<TileWallWireStateData>().RedWire = true;
                                    if (num7 > num3)
                                        num7--;

                                    if (num7 < num3)
                                        num7++;

                                    Main.tile[num6, num7].Get<TileWallWireStateData>().RedWire = true;
                                }

                                if (WorldGen.genRand.Next(3) > 0)
                                {
                                    Main.tile[num, num3].TileFrameX = 18;
                                    Main.tile[num, num3 + 1].TileFrameX = 18;
                                }

                                break;
                            }

                            break;
                        }
                    }
                }

                if (failCount > failMax)
                {
                    numAdd++;
                    failCount = 0;
                }
            }
        }

        public static double MakeDungeon_Banners(int[] roomWall, double count)
        {
            count = 840000.0 / (double)Main.maxTilesX;
            for (int i = 0; (double)i < count; i++)
            {
                int num = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                int num2 = WorldGen.genRand.Next(GenVars.dMinY, GenVars.dMaxY);
                while (!Main.wallDungeon[Main.tile[num, num2].WallType] || Main.tile[num, num2].HasTile)
                {
                    num = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                    num2 = WorldGen.genRand.Next(GenVars.dMinY, GenVars.dMaxY);
                }

                while (!WorldGen.SolidTile(num, num2) && num2 > 10)
                {
                    num2--;
                }

                num2++;
                if (!Main.wallDungeon[Main.tile[num, num2].WallType] || Main.tile[num, num2 - 1].TileType == 48 || Main.tile[num, num2].HasTile || Main.tile[num, num2 + 1].HasTile || Main.tile[num, num2 + 2].HasTile || Main.tile[num, num2 + 3].HasTile)
                    continue;

                bool flag = true;
                for (int j = num - 1; j <= num + 1; j++)
                {
                    for (int k = num2; k <= num2 + 3; k++)
                    {
                        if (Main.tile[j, k].HasTile && (Main.tile[j, k].TileType == 10 || Main.tile[j, k].TileType == 11 || Main.tile[j, k].TileType == 91))
                            flag = false;
                    }
                }

                if (flag)
                {
                    int num3 = 10;
                    if (Main.tile[num, num2].WallType == roomWall[1])
                        num3 = 12;

                    if (Main.tile[num, num2].WallType == roomWall[2])
                        num3 = 14;

                    num3 += WorldGen.genRand.Next(2);
                    WorldGen.PlaceTile(num, num2, 91, mute: true, forced: false, -1, num3);
                }
            }

            return count;
        }

        public static double MakeDungeon_Pictures(int[] roomWall, double count)
        {
            count = 420000.0 / (double)Main.maxTilesX;
            for (int i = 0; (double)i < count; i++)
            {
                int num = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                int num2 = WorldGen.genRand.Next((int)Main.worldSurface, GenVars.dMaxY);
                while (!Main.wallDungeon[Main.tile[num, num2].WallType] || Main.tile[num, num2].HasTile)
                {
                    num = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                    num2 = WorldGen.genRand.Next((int)Main.worldSurface, GenVars.dMaxY);
                }

                int num3 = num;
                int num4 = num;
                int num5 = num2;
                int num6 = num2;
                int num7 = 0;
                int num8 = 0;
                for (int j = 0; j < 2; j++)
                {
                    num3 = num;
                    num4 = num;
                    while (!Main.tile[num3, num2].HasTile && Main.wallDungeon[Main.tile[num3, num2].WallType])
                    {
                        num3--;
                    }

                    num3++;
                    for (; !Main.tile[num4, num2].HasTile && Main.wallDungeon[Main.tile[num4, num2].WallType]; num4++)
                    {
                    }

                    num4--;
                    num = (num3 + num4) / 2;
                    num5 = num2;
                    num6 = num2;
                    while (!Main.tile[num, num5].HasTile && Main.wallDungeon[Main.tile[num, num5].WallType])
                    {
                        num5--;
                    }

                    num5++;
                    for (; !Main.tile[num, num6].HasTile && Main.wallDungeon[Main.tile[num, num6].WallType]; num6++)
                    {
                    }

                    num6--;
                    num2 = (num5 + num6) / 2;
                }

                num3 = num;
                num4 = num;
                while (!Main.tile[num3, num2].HasTile && !Main.tile[num3, num2 - 1].HasTile && !Main.tile[num3, num2 + 1].HasTile)
                {
                    num3--;
                }

                num3++;
                for (; !Main.tile[num4, num2].HasTile && !Main.tile[num4, num2 - 1].HasTile && !Main.tile[num4, num2 + 1].HasTile; num4++)
                {
                }

                num4--;
                num5 = num2;
                num6 = num2;
                while (!Main.tile[num, num5].HasTile && !Main.tile[num - 1, num5].HasTile && !Main.tile[num + 1, num5].HasTile)
                {
                    num5--;
                }

                num5++;
                for (; !Main.tile[num, num6].HasTile && !Main.tile[num - 1, num6].HasTile && !Main.tile[num + 1, num6].HasTile; num6++)
                {
                }

                num6--;
                num = (num3 + num4) / 2;
                num2 = (num5 + num6) / 2;
                num7 = num4 - num3;
                num8 = num6 - num5;
                if (num7 <= 7 || num8 <= 5)
                    continue;

                bool[] array = new bool[3]
                {
                    true,
                    false,
                    false
                };

                if (num7 > num8 * 3 && num7 > 21)
                    array[1] = true;

                if (num8 > num7 * 3 && num8 > 21)
                    array[2] = true;

                int num9 = WorldGen.genRand.Next(3);
                if (Main.tile[num, num2].WallType == roomWall[0])
                    num9 = 0;

                while (!array[num9])
                {
                    num9 = WorldGen.genRand.Next(3);
                }

                if (WorldGen.nearPicture2(num, num2))
                    num9 = -1;

                switch (num9)
                {
                    case 0:
                        {
                            PaintingEntry paintingEntry2 = WorldGen.RandPictureTile();
                            if (Main.tile[num, num2].WallType != roomWall[0])
                                paintingEntry2 = WorldGen.RandBonePicture();

                            if (!WorldGen.nearPicture(num, num2))
                                WorldGen.PlaceTile(num, num2, paintingEntry2.tileType, mute: true, forced: false, -1, paintingEntry2.style);

                            break;
                        }
                    case 1:
                        {
                            PaintingEntry paintingEntry3 = WorldGen.RandPictureTile();
                            if (Main.tile[num, num2].WallType != roomWall[0])
                                paintingEntry3 = WorldGen.RandBonePicture();

                            if (!Main.tile[num, num2].HasTile)
                                WorldGen.PlaceTile(num, num2, paintingEntry3.tileType, mute: true, forced: false, -1, paintingEntry3.style);

                            int num13 = num;
                            int num14 = num2;
                            int num15 = num2;
                            for (int m = 0; m < 2; m++)
                            {
                                num += 7;
                                num5 = num15;
                                num6 = num15;
                                while (!Main.tile[num, num5].HasTile && !Main.tile[num - 1, num5].HasTile && !Main.tile[num + 1, num5].HasTile)
                                {
                                    num5--;
                                }

                                num5++;
                                for (; !Main.tile[num, num6].HasTile && !Main.tile[num - 1, num6].HasTile && !Main.tile[num + 1, num6].HasTile; num6++)
                                {
                                }

                                num6--;
                                num15 = (num5 + num6) / 2;
                                paintingEntry3 = WorldGen.RandPictureTile();
                                if (Main.tile[num, num15].WallType != roomWall[0])
                                    paintingEntry3 = WorldGen.RandBonePicture();

                                if (Math.Abs(num14 - num15) >= 4 || WorldGen.nearPicture(num, num15))
                                    break;

                                WorldGen.PlaceTile(num, num15, paintingEntry3.tileType, mute: true, forced: false, -1, paintingEntry3.style);
                            }

                            num15 = num2;
                            num = num13;
                            for (int n = 0; n < 2; n++)
                            {
                                num -= 7;
                                num5 = num15;
                                num6 = num15;
                                while (!Main.tile[num, num5].HasTile && !Main.tile[num - 1, num5].HasTile && !Main.tile[num + 1, num5].HasTile)
                                {
                                    num5--;
                                }

                                num5++;
                                for (; !Main.tile[num, num6].HasTile && !Main.tile[num - 1, num6].HasTile && !Main.tile[num + 1, num6].HasTile; num6++)
                                {
                                }

                                num6--;
                                num15 = (num5 + num6) / 2;
                                paintingEntry3 = WorldGen.RandPictureTile();
                                if (Main.tile[num, num15].WallType != roomWall[0])
                                    paintingEntry3 = WorldGen.RandBonePicture();

                                if (Math.Abs(num14 - num15) >= 4 || WorldGen.nearPicture(num, num15))
                                    break;

                                WorldGen.PlaceTile(num, num15, paintingEntry3.tileType, mute: true, forced: false, -1, paintingEntry3.style);
                            }

                            break;
                        }
                    case 2:
                        {
                            PaintingEntry paintingEntry = WorldGen.RandPictureTile();
                            if (Main.tile[num, num2].WallType != roomWall[0])
                                paintingEntry = WorldGen.RandBonePicture();

                            if (!Main.tile[num, num2].HasTile)
                                WorldGen.PlaceTile(num, num2, paintingEntry.tileType, mute: true, forced: false, -1, paintingEntry.style);

                            int num10 = num2;
                            int num11 = num;
                            int num12 = num;
                            for (int k = 0; k < 3; k++)
                            {
                                num2 += 7;
                                num3 = num12;
                                num4 = num12;
                                while (!Main.tile[num3, num2].HasTile && !Main.tile[num3, num2 - 1].HasTile && !Main.tile[num3, num2 + 1].HasTile)
                                {
                                    num3--;
                                }

                                num3++;
                                for (; !Main.tile[num4, num2].HasTile && !Main.tile[num4, num2 - 1].HasTile && !Main.tile[num4, num2 + 1].HasTile; num4++)
                                {
                                }

                                num4--;
                                num12 = (num3 + num4) / 2;
                                paintingEntry = WorldGen.RandPictureTile();
                                if (Main.tile[num12, num2].WallType != roomWall[0])
                                    paintingEntry = WorldGen.RandBonePicture();

                                if (Math.Abs(num11 - num12) >= 4 || WorldGen.nearPicture(num12, num2))
                                    break;

                                WorldGen.PlaceTile(num12, num2, paintingEntry.tileType, mute: true, forced: false, -1, paintingEntry.style);
                            }

                            num12 = num;
                            num2 = num10;
                            for (int l = 0; l < 3; l++)
                            {
                                num2 -= 7;
                                num3 = num12;
                                num4 = num12;
                                while (!Main.tile[num3, num2].HasTile && !Main.tile[num3, num2 - 1].HasTile && !Main.tile[num3, num2 + 1].HasTile)
                                {
                                    num3--;
                                }

                                num3++;
                                for (; !Main.tile[num4, num2].HasTile && !Main.tile[num4, num2 - 1].HasTile && !Main.tile[num4, num2 + 1].HasTile; num4++)
                                {
                                }

                                num4--;
                                num12 = (num3 + num4) / 2;
                                paintingEntry = WorldGen.RandPictureTile();
                                if (Main.tile[num12, num2].WallType != roomWall[0])
                                    paintingEntry = WorldGen.RandBonePicture();

                                if (Math.Abs(num11 - num12) >= 4 || WorldGen.nearPicture(num12, num2))
                                    break;

                                WorldGen.PlaceTile(num12, num2, paintingEntry.tileType, mute: true, forced: false, -1, paintingEntry.style);
                            }

                            break;
                        }
                }
            }

            return count;
        }

        public static double MakeDungeon_GroundFurniture(int wallType)
        {
            double num = (double)(2000 * Main.maxTilesX) / 4200.0;
            int num2 = 1 + (int)((double)Main.maxTilesX / 4200.0);
            int num3 = 1 + (int)((double)Main.maxTilesX / 4200.0);
            for (int i = 0; (double)i < num; i++)
            {
                if (num2 > 0 || num3 > 0)
                    i--;

                int num4 = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                int j = WorldGen.genRand.Next((int)Main.worldSurface + 10, GenVars.dMaxY);
                while (!Main.wallDungeon[Main.tile[num4, j].WallType] || Main.tile[num4, j].HasTile)
                {
                    num4 = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                    j = WorldGen.genRand.Next((int)Main.worldSurface + 10, GenVars.dMaxY);
                }

                if (!Main.wallDungeon[Main.tile[num4, j].WallType] || Main.tile[num4, j].HasTile)
                    continue;

                for (; !WorldGen.SolidTile(num4, j) && j < Main.UnderworldLayer; j++)
                {
                }

                j--;
                int num5 = num4;
                int k = num4;
                while (!Main.tile[num5, j].HasTile && WorldGen.SolidTile(num5, j + 1))
                {
                    num5--;
                }

                num5++;
                for (; !Main.tile[k, j].HasTile && WorldGen.SolidTile(k, j + 1); k++)
                {
                }

                k--;
                int num6 = k - num5;
                int num7 = (k + num5) / 2;
                if (Main.tile[num7, j].HasTile || !Main.wallDungeon[Main.tile[num7, j].WallType] || !WorldGen.SolidTile(num7, j + 1) || Main.tile[num7, j + 1].TileType == 48)
                    continue;

                int style = 13;
                int style2 = 10;
                int style3 = 11;
                int num8 = 1;
                int num9 = 46;
                int style4 = 1;
                int num10 = 5;
                int num11 = 11;
                int num12 = 5;
                int num13 = 6;
                int num14 = 21;
                int num15 = 22;
                int num16 = 24;
                int num17 = 30;
                switch (wallType)
                {
                    case 8:
                        style = 14;
                        style2 = 11;
                        style3 = 12;
                        num8 = 2;
                        num9 = 47;
                        style4 = 2;
                        num10 = 6;
                        num11 = 12;
                        num12 = 6;
                        num13 = 7;
                        num14 = 22;
                        num15 = 23;
                        num16 = 25;
                        num17 = 31;
                        break;
                    case 9:
                        style = 15;
                        style2 = 12;
                        style3 = 13;
                        num8 = 3;
                        num9 = 48;
                        style4 = 3;
                        num10 = 7;
                        num11 = 13;
                        num12 = 7;
                        num13 = 8;
                        num14 = 23;
                        num15 = 24;
                        num16 = 26;
                        num17 = 32;
                        break;
                }

                if (Main.tile[num7, j].WallType >= 94 && Main.tile[num7, j].WallType <= 105)
                {
                    style = 17;
                    style2 = 14;
                    style3 = 15;
                    num8 = -1;
                    num9 = -1;
                    style4 = 5;
                    num10 = -1;
                    num11 = -1;
                    num12 = -1;
                    num13 = -1;
                    num14 = -1;
                    num15 = -1;
                    num16 = -1;
                    num17 = -1;
                }

                int num18 = WorldGen.genRand.Next(13);
                if ((num18 == 10 || num18 == 11 || num18 == 12) && WorldGen.genRand.Next(4) != 0)
                    num18 = WorldGen.genRand.Next(13);

                while ((num18 == 2 && num9 == -1) || (num18 == 5 && num10 == -1) || (num18 == 6 && num11 == -1) || (num18 == 7 && num12 == -1) || (num18 == 8 && num13 == -1) || (num18 == 9 && num14 == -1) || (num18 == 10 && num15 == -1) || (num18 == 11 && num16 == -1) || (num18 == 12 && num17 == -1))
                {
                    num18 = WorldGen.genRand.Next(13);
                }

                int num19 = 0;
                int num20 = 0;
                if (num18 == 0)
                {
                    num19 = 5;
                    num20 = 4;
                }

                if (num18 == 1)
                {
                    num19 = 4;
                    num20 = 3;
                }

                if (num18 == 2)
                {
                    num19 = 3;
                    num20 = 5;
                }

                if (num18 == 3)
                {
                    num19 = 4;
                    num20 = 6;
                }

                if (num18 == 4)
                {
                    num19 = 3;
                    num20 = 3;
                }

                if (num18 == 5)
                {
                    num19 = 5;
                    num20 = 3;
                }

                if (num18 == 6)
                {
                    num19 = 5;
                    num20 = 4;
                }

                if (num18 == 7)
                {
                    num19 = 5;
                    num20 = 4;
                }

                if (num18 == 8)
                {
                    num19 = 5;
                    num20 = 4;
                }

                if (num18 == 9)
                {
                    num19 = 5;
                    num20 = 3;
                }

                if (num18 == 10)
                {
                    num19 = 2;
                    num20 = 4;
                }

                if (num18 == 11)
                {
                    num19 = 3;
                    num20 = 3;
                }

                if (num18 == 12)
                {
                    num19 = 2;
                    num20 = 5;
                }

                for (int l = num7 - num19; l <= num7 + num19; l++)
                {
                    for (int m = j - num20; m <= j; m++)
                    {
                        if (Main.tile[l, m].HasTile)
                        {
                            num18 = -1;
                            break;
                        }
                    }
                }

                if ((double)num6 < (double)num19 * 1.75)
                    num18 = -1;

                if (num2 > 0 || num3 > 0)
                {
                    if (num2 > 0)
                    {
                        WorldGen.PlaceTile(num7, j, 355, mute: true);
                        if (Main.tile[num7, j].TileType == 355)
                            num2--;
                    }
                    else if (num3 > 0)
                    {
                        WorldGen.PlaceTile(num7, j, 354, mute: true);
                        if (Main.tile[num7, j].TileType == 354)
                            num3--;
                    }

                    continue;
                }

                switch (num18)
                {
                    case 0:
                        {
                            WorldGen.PlaceTile(num7, j, 14, mute: true, forced: false, -1, style2);
                            if (Main.tile[num7, j].HasTile)
                            {
                                if (!Main.tile[num7 - 2, j].HasTile)
                                {
                                    WorldGen.PlaceTile(num7 - 2, j, 15, mute: true, forced: false, -1, style);
                                    if (Main.tile[num7 - 2, j].HasTile)
                                    {
                                        Main.tile[num7 - 2, j].TileFrameX += 18;
                                        Main.tile[num7 - 2, j - 1].TileFrameX += 18;
                                    }
                                }

                                if (!Main.tile[num7 + 2, j].HasTile)
                                    WorldGen.PlaceTile(num7 + 2, j, 15, mute: true, forced: false, -1, style);
                            }

                            for (int num22 = num7 - 1; num22 <= num7 + 1; num22++)
                            {
                                if (WorldGen.genRand.NextBool() && !Main.tile[num22, j - 2].HasTile)
                                {
                                    int num23 = WorldGen.genRand.Next(5);
                                    if (num8 != -1 && num23 <= 1 && !Main.tileLighted[Main.tile[num22 - 1, j - 2].TileType])
                                        WorldGen.PlaceTile(num22, j - 2, 33, mute: true, forced: false, -1, num8);

                                    if (num23 == 2 && !Main.tileLighted[Main.tile[num22 - 1, j - 2].TileType])
                                        WorldGen.PlaceTile(num22, j - 2, 49, mute: true);

                                    if (num23 == 3)
                                        WorldGen.PlaceTile(num22, j - 2, 50, mute: true);

                                    if (num23 == 4)
                                        WorldGen.PlaceTile(num22, j - 2, 103, mute: true);
                                }
                            }

                            break;
                        }
                    case 1:
                        {
                            WorldGen.PlaceTile(num7, j, 18, mute: true, forced: false, -1, style3);
                            if (!Main.tile[num7, j].HasTile)
                                break;

                            if (WorldGen.genRand.NextBool())
                            {
                                if (!Main.tile[num7 - 1, j].HasTile)
                                {
                                    WorldGen.PlaceTile(num7 - 1, j, 15, mute: true, forced: false, -1, style);
                                    if (Main.tile[num7 - 1, j].HasTile)
                                    {
                                        Main.tile[num7 - 1, j].TileFrameX += 18;
                                        Main.tile[num7 - 1, j - 1].TileFrameX += 18;
                                    }
                                }
                            }
                            else if (!Main.tile[num7 + 2, j].HasTile)
                            {
                                WorldGen.PlaceTile(num7 + 2, j, 15, mute: true, forced: false, -1, style);
                            }

                            for (int n = num7; n <= num7 + 1; n++)
                            {
                                if (WorldGen.genRand.NextBool() && !Main.tile[n, j - 1].HasTile)
                                {
                                    int num21 = WorldGen.genRand.Next(5);
                                    if (num8 != -1 && num21 <= 1 && !Main.tileLighted[Main.tile[n - 1, j - 1].TileType])
                                        WorldGen.PlaceTile(n, j - 1, 33, mute: true, forced: false, -1, num8);

                                    if (num21 == 2 && !Main.tileLighted[Main.tile[n - 1, j - 1].TileType])
                                        WorldGen.PlaceTile(n, j - 1, 49, mute: true);

                                    if (num21 == 3)
                                        WorldGen.PlaceTile(n, j - 1, 50, mute: true);

                                    if (num21 == 4)
                                        WorldGen.PlaceTile(n, j - 1, 103, mute: true);
                                }
                            }

                            break;
                        }
                    case 2:
                        WorldGen.PlaceTile(num7, j, 105, mute: true, forced: false, -1, num9);
                        break;
                    case 3:
                        WorldGen.PlaceTile(num7, j, 101, mute: true, forced: false, -1, style4);
                        break;
                    case 4:
                        if (WorldGen.genRand.NextBool())
                        {
                            WorldGen.PlaceTile(num7, j, 15, mute: true, forced: false, -1, style);
                            Main.tile[num7, j].TileFrameX += 18;
                            Main.tile[num7, j - 1].TileFrameX += 18;
                        }
                        else
                        {
                            WorldGen.PlaceTile(num7, j, 15, mute: true, forced: false, -1, style);
                        }
                        break;
                    case 5:
                        if (WorldGen.genRand.NextBool())
                            WorldGen.Place4x2(num7, j, 79, 1, num10);
                        else
                            WorldGen.Place4x2(num7, j, 79, -1, num10);
                        break;
                    case 6:
                        WorldGen.PlaceTile(num7, j, 87, mute: true, forced: false, -1, num11);
                        break;
                    case 7:
                        WorldGen.PlaceTile(num7, j, 88, mute: true, forced: false, -1, num12);
                        break;
                    case 8:
                        WorldGen.PlaceTile(num7, j, 89, mute: true, forced: false, -1, num13);
                        break;
                    case 9:
                        if (WorldGen.genRand.NextBool())
                            WorldGen.Place4x2(num7, j, 90, 1, num14);
                        else
                            WorldGen.Place4x2(num7, j, 90, -1, num14);
                        break;
                    case 10:
                        WorldGen.PlaceTile(num7, j, 93, mute: true, forced: false, -1, num16);
                        break;
                    case 11:
                        WorldGen.PlaceTile(num7, j, 100, mute: true, forced: false, -1, num15);
                        break;
                    case 12:
                        WorldGen.PlaceTile(num7, j, 104, mute: true, forced: false, -1, num17);
                        break;
                }
            }

            return num;
        }

        public static void GenNewDungeonHalls(int i, int j, ushort tileType, int wallType, bool forceX = false)
        {
            Vector2D zero = Vector2D.Zero;
            double num = WorldGen.genRand.Next(4, 6);
            double num2 = num;
            Vector2D zero2 = Vector2D.Zero;
            Vector2D zero3 = Vector2D.Zero;
            int num3 = 1;
            Vector2D vector2D = default(Vector2D);
            vector2D.X = i;

            // This is what the Calamity IL edit used to do, in order to move the dungeon halls away from the Sulphur Sea.
            vector2D.X = MathHelper.Clamp((float)vector2D.X, SulphurousSea.BiomeWidth + 25, Main.maxTilesX - SulphurousSea.BiomeWidth - 25);

            vector2D.Y = j;
            int num4 = WorldGen.genRand.Next(35, 80);
            bool flag = false;
            if (WorldGen.genRand.Next(6) == 0)
                flag = true;

            if (forceX)
            {
                num4 += 20;
                GenVars.lastDungeonHall = Vector2D.Zero;
            }
            else if (WorldGen.genRand.Next(5) == 0)
            {
                num *= 2.0;
                num4 /= 2;
            }

            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = true;
            bool flag5 = false;
            while (!flag2)
            {
                flag5 = false;
                if (flag4 && !forceX)
                {
                    bool flag6 = true;
                    bool flag7 = true;
                    bool flag8 = true;
                    bool flag9 = true;
                    int num5 = num4;
                    bool flag10 = false;
                    for (int num6 = j; num6 > j - num5; num6--)
                    {
                        if (Main.tile[i, num6].WallType == wallType)
                        {
                            if (flag10)
                            {
                                flag6 = false;
                                break;
                            }
                        }
                        else
                        {
                            flag10 = true;
                        }
                    }

                    flag10 = false;
                    for (int k = j; k < j + num5; k++)
                    {
                        if (Main.tile[i, k].WallType == wallType)
                        {
                            if (flag10)
                            {
                                flag7 = false;
                                break;
                            }
                        }
                        else
                        {
                            flag10 = true;
                        }
                    }

                    flag10 = false;
                    for (int num7 = i; num7 > i - num5; num7--)
                    {
                        if (Main.tile[num7, j].WallType == wallType)
                        {
                            if (flag10)
                            {
                                flag8 = false;
                                break;
                            }
                        }
                        else
                        {
                            flag10 = true;
                        }
                    }

                    flag10 = false;
                    for (int l = i; l < i + num5; l++)
                    {
                        if (Main.tile[l, j].WallType == wallType)
                        {
                            if (flag10)
                            {
                                flag9 = false;
                                break;
                            }
                        }
                        else
                        {
                            flag10 = true;
                        }
                    }

                    if (!flag8 && !flag9 && !flag6 && !flag7)
                    {
                        num3 = (WorldGen.genRand.NextBool() ? 1 : (-1));
                        if (WorldGen.genRand.NextBool())
                            flag5 = true;
                    }
                    else
                    {
                        int num8 = WorldGen.genRand.Next(4);
                        do
                        {
                            num8 = WorldGen.genRand.Next(4);
                        } while (!(num8 == 0 && flag6) && !(num8 == 1 && flag7) && !(num8 == 2 && flag8) && !(num8 == 3 && flag9));

                        switch (num8)
                        {
                            case 0:
                                num3 = -1;
                                break;
                            case 1:
                                num3 = 1;
                                break;
                            default:
                                flag5 = true;
                                num3 = ((num8 != 2) ? 1 : (-1));
                                break;
                        }
                    }
                }
                else
                {
                    num3 = (WorldGen.genRand.NextBool() ? 1 : (-1));
                    if (WorldGen.genRand.NextBool())
                        flag5 = true;
                }

                flag4 = false;
                if (forceX)
                    flag5 = true;

                if (flag5)
                {
                    zero2.Y = 0.0;
                    zero2.X = num3;
                    zero3.Y = 0.0;
                    zero3.X = -num3;
                    zero.Y = 0.0;
                    zero.X = num3;
                    if (WorldGen.genRand.Next(3) == 0)
                    {
                        if (WorldGen.genRand.NextBool())
                            zero.Y = -0.2;
                        else
                            zero.Y = 0.2;
                    }
                }
                else
                {
                    num += 1.0;
                    zero.Y = num3;
                    zero.X = 0.0;
                    zero2.X = 0.0;
                    zero2.Y = num3;
                    zero3.X = 0.0;
                    zero3.Y = -num3;
                    if (WorldGen.genRand.Next(3) != 0)
                    {
                        flag3 = true;
                        if (WorldGen.genRand.NextBool())
                            zero.X = (double)WorldGen.genRand.Next(10, 20) * 0.1;
                        else
                            zero.X = (double)(-WorldGen.genRand.Next(10, 20)) * 0.1;
                    }
                    else if (WorldGen.genRand.NextBool())
                    {
                        if (WorldGen.genRand.NextBool())
                            zero.X = (double)WorldGen.genRand.Next(20, 40) * 0.01;
                        else
                            zero.X = (double)(-WorldGen.genRand.Next(20, 40)) * 0.01;
                    }
                    else
                    {
                        num4 /= 2;
                    }
                }

                if (GenVars.lastDungeonHall != zero3)
                    flag2 = true;
            }

            int num9 = 0;
            bool flag11 = vector2D.Y < Main.rockLayer + 100.0;
            if (WorldGen.remixWorldGen)
                flag11 = vector2D.Y < Main.worldSurface + 100.0;

            // Fuck you
            int LastMaxTilesX = (int)typeof(WorldGen).GetField("lastMaxTilesX", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            int LastMaxTilesY = (int)typeof(WorldGen).GetField("lastMaxTilesY", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

            if (!forceX)
            {
                if (vector2D.X > (double)(LastMaxTilesX - 200))
                {
                    num3 = -1;
                    zero2.Y = 0.0;
                    zero2.X = num3;
                    zero.Y = 0.0;
                    zero.X = num3;
                    if (WorldGen.genRand.Next(3) == 0)
                    {
                        if (WorldGen.genRand.NextBool())
                            zero.Y = -0.2;
                        else
                            zero.Y = 0.2;
                    }
                }
                else if (vector2D.X < 200.0)
                {
                    num3 = 1;
                    zero2.Y = 0.0;
                    zero2.X = num3;
                    zero.Y = 0.0;
                    zero.X = num3;
                    if (WorldGen.genRand.Next(3) == 0)
                    {
                        if (WorldGen.genRand.NextBool())
                            zero.Y = -0.2;
                        else
                            zero.Y = 0.2;
                    }
                }
                else if (vector2D.Y > (double)(LastMaxTilesY - 300))
                {
                    num3 = -1;
                    num += 1.0;
                    zero.Y = num3;
                    zero.X = 0.0;
                    zero2.X = 0.0;
                    zero2.Y = num3;
                    if (WorldGen.genRand.NextBool())
                    {
                        if (WorldGen.genRand.NextBool())
                            zero.X = (double)WorldGen.genRand.Next(20, 50) * 0.01;
                        else
                            zero.X = (double)(-WorldGen.genRand.Next(20, 50)) * 0.01;
                    }
                }
                else if (flag11)
                {
                    num3 = 1;
                    num += 1.0;
                    zero.Y = num3;
                    zero.X = 0.0;
                    zero2.X = 0.0;
                    zero2.Y = num3;
                    if (WorldGen.genRand.Next(3) != 0)
                    {
                        flag3 = true;
                        if (WorldGen.genRand.NextBool())
                            zero.X = (double)WorldGen.genRand.Next(10, 20) * 0.1;
                        else
                            zero.X = (double)(-WorldGen.genRand.Next(10, 20)) * 0.1;
                    }
                    else if (WorldGen.genRand.NextBool())
                    {
                        if (WorldGen.genRand.NextBool())
                            zero.X = (double)WorldGen.genRand.Next(20, 50) * 0.01;
                        else
                            zero.X = (double)WorldGen.genRand.Next(20, 50) * 0.01;
                    }
                }
                else if (vector2D.X < (double)(Main.maxTilesX / 2) && vector2D.X > (double)Main.maxTilesX * 0.25)
                {
                    num3 = -1;
                    zero2.Y = 0.0;
                    zero2.X = num3;
                    zero.Y = 0.0;
                    zero.X = num3;
                    if (WorldGen.genRand.Next(3) == 0)
                    {
                        if (WorldGen.genRand.NextBool())
                            zero.Y = -0.2;
                        else
                            zero.Y = 0.2;
                    }
                }
                else if (vector2D.X > (double)(Main.maxTilesX / 2) && vector2D.X < (double)Main.maxTilesX * 0.75)
                {
                    num3 = 1;
                    zero2.Y = 0.0;
                    zero2.X = num3;
                    zero.Y = 0.0;
                    zero.X = num3;
                    if (WorldGen.genRand.Next(3) == 0)
                    {
                        if (WorldGen.genRand.NextBool())
                            zero.Y = -0.2;
                        else
                            zero.Y = 0.2;
                    }
                }
            }

            if (zero2.Y == 0.0)
            {
                GenVars.DDoorX[GenVars.numDDoors] = (int)vector2D.X;
                GenVars.DDoorY[GenVars.numDDoors] = (int)vector2D.Y;
                GenVars.DDoorPos[GenVars.numDDoors] = 0;
                GenVars.numDDoors++;
            }
            else
            {
                GenVars.dungeonPlatformX[GenVars.numDungeonPlatforms] = (int)vector2D.X;
                GenVars.dungeonPlatformY[GenVars.numDungeonPlatforms] = (int)vector2D.Y;
                GenVars.numDungeonPlatforms++;
            }

            GenVars.lastDungeonHall = zero2;
            if (Math.Abs(zero.X) > Math.Abs(zero.Y) && WorldGen.genRand.Next(3) != 0)
                num = (int)(num2 * ((double)WorldGen.genRand.Next(110, 150) * 0.01));

            while (num4 > 0)
            {
                num9++;
                if (zero2.X > 0.0 && vector2D.X > (double)(Main.maxTilesX - 100))
                    num4 = 0;
                else if (zero2.X < 0.0 && vector2D.X < 100.0)
                    num4 = 0;
                else if (zero2.Y > 0.0 && vector2D.Y > (double)(Main.maxTilesY - 100))
                    num4 = 0;
                else if (WorldGen.remixWorldGen && zero2.Y < 0.0 && vector2D.Y < (Main.rockLayer + Main.worldSurface) / 2.0)
                    num4 = 0;
                else if (!WorldGen.remixWorldGen && zero2.Y < 0.0 && vector2D.Y < Main.rockLayer + 50.0)
                    num4 = 0;

                num4--;
                int num10 = (int)(vector2D.X - num - 4.0 - (double)WorldGen.genRand.Next(6));
                int num11 = (int)(vector2D.X + num + 4.0 + (double)WorldGen.genRand.Next(6));
                int num12 = (int)(vector2D.Y - num - 4.0 - (double)WorldGen.genRand.Next(6));
                int num13 = (int)(vector2D.Y + num + 4.0 + (double)WorldGen.genRand.Next(6));
                if (num10 < 0)
                    num10 = 0;

                if (num11 > Main.maxTilesX)
                    num11 = Main.maxTilesX;

                if (num12 < 0)
                    num12 = 0;

                if (num13 > Main.maxTilesY)
                    num13 = Main.maxTilesY;

                for (int m = num10; m < num11; m++)
                {
                    for (int n = num12; n < num13; n++)
                    {
                        if (m < GenVars.dMinX)
                            GenVars.dMinX = m;

                        if (m > GenVars.dMaxX)
                            GenVars.dMaxX = m;

                        if (n > GenVars.dMaxY)
                            GenVars.dMaxY = n;

                        Main.tile[m, n].LiquidAmount = 0;
                        if (!Main.wallDungeon[Main.tile[m, n].WallType])
                        {
                            Main.tile[m, n].Get<TileWallWireStateData>().HasTile = true;
                            Main.tile[m, n].TileType = tileType;
                            Main.tile[m, n].Clear(TileDataType.Slope);
                        }
                    }
                }

                for (int num14 = num10 + 1; num14 < num11 - 1; num14++)
                {
                    for (int num15 = num12 + 1; num15 < num13 - 1; num15++)
                    {
                        Main.tile[num14, num15].WallType = (ushort)wallType;
                    }
                }

                int num16 = 0;
                if (zero.Y == 0.0 && WorldGen.genRand.Next((int)num + 1) == 0)
                    num16 = WorldGen.genRand.Next(1, 3);
                else if (zero.X == 0.0 && WorldGen.genRand.Next((int)num - 1) == 0)
                    num16 = WorldGen.genRand.Next(1, 3);
                else if (WorldGen.genRand.Next((int)num * 3) == 0)
                    num16 = WorldGen.genRand.Next(1, 3);

                num10 = (int)(vector2D.X - num * 0.5 - (double)num16);
                num11 = (int)(vector2D.X + num * 0.5 + (double)num16);
                num12 = (int)(vector2D.Y - num * 0.5 - (double)num16);
                num13 = (int)(vector2D.Y + num * 0.5 + (double)num16);
                if (num10 < 0)
                    num10 = 0;

                if (num11 > Main.maxTilesX)
                    num11 = Main.maxTilesX;

                if (num12 < 0)
                    num12 = 0;

                if (num13 > Main.maxTilesY)
                    num13 = Main.maxTilesY;

                for (int num17 = num10; num17 < num11; num17++)
                {
                    for (int num18 = num12; num18 < num13; num18++)
                    {
                        Main.tile[num17, num18].Clear(TileDataType.Slope);
                        if (flag)
                        {
                            if (Main.tile[num17, num18].HasTile || Main.tile[num17, num18].WallType != wallType)
                            {
                                Main.tile[num17, num18].Get<TileWallWireStateData>().HasTile = true;
                                Main.tile[num17, num18].TileType = GenVars.crackedType;
                            }
                        }
                        else
                        {
                            Main.tile[num17, num18].Get<TileWallWireStateData>().HasTile = false;
                        }

                        Main.tile[num17, num18].Clear(TileDataType.Slope);
                        Main.tile[num17, num18].WallType = (ushort)wallType;
                    }
                }

                vector2D += zero;
                if (flag3 && num9 > WorldGen.genRand.Next(10, 20))
                {
                    num9 = 0;
                    zero.X *= -1.0;
                }
            }

            GenVars.dungeonX = (int)vector2D.X;
            GenVars.dungeonY = (int)vector2D.Y;
            if (zero2.Y == 0.0)
            {
                GenVars.DDoorX[GenVars.numDDoors] = (int)vector2D.X;
                GenVars.DDoorY[GenVars.numDDoors] = (int)vector2D.Y;
                GenVars.DDoorPos[GenVars.numDDoors] = 0;
                GenVars.numDDoors++;
            }
            else
            {
                GenVars.dungeonPlatformX[GenVars.numDungeonPlatforms] = (int)vector2D.X;
                GenVars.dungeonPlatformY[GenVars.numDungeonPlatforms] = (int)vector2D.Y;
                GenVars.numDungeonPlatforms++;
            }
        }
    }
}
