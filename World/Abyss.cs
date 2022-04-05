using CalamityMod.Items.Accessories;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Tools.ClimateChange;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.FurnitureVoid;
using CalamityMod.Tiles.Ores;
using CalamityMod.Walls;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.World
{
    public class Abyss
    {
        public static void PlaceAbyss()
        {
            int x = Main.maxTilesX;
            int y = Main.maxTilesY;
            int genLimit = x / 2;
            int rockLayer = (int)Main.rockLayer;

            int abyssChasmY = y - 250; //Underworld = y - 200
            CalamityWorld.abyssChasmBottom = abyssChasmY - 100; //850 small 1450 medium 2050 large
            int abyssChasmX = CalamityWorld.abyssSide ? genLimit - (genLimit - 135) : genLimit + (genLimit - 135); //2100 - 1965 = 135 : 2100 + 1965 = 4065

            bool tenebrisSide = true;
            if (abyssChasmX < genLimit)
                tenebrisSide = false;

            if (CalamityWorld.abyssSide)
            {
                for (int abyssIndex = 0; abyssIndex < abyssChasmX + 80; abyssIndex++) //235
                {
                    for (int abyssIndex2 = 0; abyssIndex2 < abyssChasmY; abyssIndex2++)
                    {
                        Tile tile = Framing.GetTileSafely(abyssIndex, abyssIndex2);
                        bool canConvert = tile.HasTile &&
                            tile.TileType < TileID.Count &&
                            tile.TileType != TileID.DyePlants &&
                            tile.TileType != TileID.Trees &&
                            tile.TileType != TileID.PalmTree &&
                            tile.TileType != TileID.Sand &&
                            tile.TileType != TileID.Containers &&
                            tile.TileType != TileID.Coral &&
                            tile.TileType != TileID.BeachPiles &&
                            tile.TileType != ModContent.TileType<SulphurousSand>() &&
                            tile.TileType != ModContent.TileType<SulphurousSandstone>();
                        if (abyssIndex2 > rockLayer - WorldGen.genRand.Next(30))
                        {
                            if (abyssIndex > abyssChasmX + 75 - WorldGen.genRand.Next(30))
                            {
                                if (WorldGen.genRand.Next(4) == 0)
                                {
                                    if (canConvert)
                                    {
                                        tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                        tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                    }
                                    else if (!tile.HasTile)
                                    {
                                        tile.active(true);
                                        tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                        tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                    }
                                }
                            }
                            else if (abyssIndex > abyssChasmX + 70)
                            {
                                if (WorldGen.genRand.Next(2) == 0)
                                {
                                    if (canConvert)
                                    {
                                        tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                        tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                    }
                                    else if (!tile.HasTile)
                                    {
                                        tile.active(true);
                                        tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                        tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                    }
                                }
                            }
                            else
                            {
                                if (canConvert)
                                {
                                    if (abyssIndex2 > (rockLayer + y * 0.262))
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                        tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                    }
                                    else if (abyssIndex2 > (rockLayer + y * 0.143) && WorldGen.genRand.Next(3) == 0)
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                        tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                    }
                                    else
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                        tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                    }
                                }
                                else if (!tile.HasTile)
                                {
                                    tile.active(true);
                                    if (abyssIndex2 > (rockLayer + y * 0.262))
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                        tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                    }
                                    else if (abyssIndex2 > (rockLayer + y * 0.143) && WorldGen.genRand.Next(3) == 0)
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                        tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                    }
                                    else
                                    {
                                        tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                        tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int abyssIndex = abyssChasmX - 80; abyssIndex < x; abyssIndex++) //3965
                {
                    for (int abyssIndex2 = 0; abyssIndex2 < abyssChasmY; abyssIndex2++)
                    {
                        Tile tile = Framing.GetTileSafely(abyssIndex, abyssIndex2);
                        bool canConvert = tile.HasTile &&
                            tile.TileType < TileID.Count &&
                            tile.TileType != TileID.DyePlants &&
                            tile.TileType != TileID.Trees &&
                            tile.TileType != TileID.PalmTree &&
                            tile.TileType != TileID.Sand &&
                            tile.TileType != TileID.Containers &&
                            tile.TileType != TileID.Coral &&
                            tile.TileType != TileID.BeachPiles;
                        if (abyssIndex2 > rockLayer - WorldGen.genRand.Next(30))
                        {
                            if (abyssIndex < abyssChasmX - 75)
                            {
                                if (WorldGen.genRand.Next(4) == 0)
                                {
                                    if (canConvert)
                                    {
                                        tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                        tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                    }
                                    else if (!tile.HasTile)
                                    {
                                        tile.active(true);
                                        tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                        tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                    }
                                }
                            }
                            else if (abyssIndex < abyssChasmX - 70)
                            {
                                if (WorldGen.genRand.Next(2) == 0)
                                {
                                    if (canConvert)
                                    {
                                        tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                        tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                    }
                                    else if (!tile.HasTile)
                                    {
                                        tile.active(true);
                                        tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                        tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                    }
                                }
                            }
                            else
                            {
                                if (canConvert)
                                {
                                    if (abyssIndex2 > (rockLayer + y * 0.262))
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                        tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                    }
                                    else if (abyssIndex2 > (rockLayer + y * 0.143) && WorldGen.genRand.Next(3) == 0)
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                        tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                    }
                                    else
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                        tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                    }
                                }
                                else if (!tile.HasTile)
                                {
                                    tile.active(true);
                                    if (abyssIndex2 > (rockLayer + y * 0.262))
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                        tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                    }
                                    else if (abyssIndex2 > (rockLayer + y * 0.143) && WorldGen.genRand.Next(3) == 0)
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                        tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                    }
                                    else
                                    {
                                        tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                        tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            MiscWorldgenRoutines.ChasmGenerator(abyssChasmX, (int)WorldGen.worldSurfaceLow, CalamityWorld.abyssChasmBottom, true);

            int maxAbyssIslands = 11; //Small World
            if (y > 2100)
                maxAbyssIslands = 20; //Large World
            else if (y > 1500)
                maxAbyssIslands = 16; //Medium World

            // Place the Terminus shrine.
            UndergroundShrines.SpecialHut((ushort)ModContent.TileType<SmoothVoidstone>(), (ushort)ModContent.TileType<Voidstone>(),
                (ushort)ModContent.WallType<VoidstoneWallUnsafe>(), UndergroundShrines.UndergroundShrineType.Abyss, abyssChasmX, CalamityWorld.abyssChasmBottom);

            int islandLocationOffset = 30;
            int islandLocationY = rockLayer;
            for (int islands = 0; islands < maxAbyssIslands; islands++)
            {
                int islandLocationX = abyssChasmX;
                int randomIsland = WorldGen.genRand.Next(5); //0 1 2 3 4
                bool hasVoidstone = islandLocationY > (rockLayer + y * 0.143);
                CalamityWorld.AbyssIslandY[CalamityWorld.numAbyssIslands] = islandLocationY;
                switch (randomIsland)
                {
                    case 0:
                        AbyssIsland(islandLocationX, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
                        AbyssIsland(islandLocationX + 40, islandLocationY + 15, 60, 66, 30, 36, false, tenebrisSide, hasVoidstone);
                        AbyssIsland(islandLocationX - 40, islandLocationY + 15, 60, 66, 30, 36, false, !tenebrisSide, hasVoidstone);
                        break;
                    case 1:
                        AbyssIsland(islandLocationX + 30, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
                        AbyssIsland(islandLocationX, islandLocationY + 15, 60, 66, 30, 36, false, tenebrisSide, hasVoidstone);
                        AbyssIsland(islandLocationX - 40, islandLocationY + 10, 60, 66, 30, 36, false, !tenebrisSide, hasVoidstone);
                        islandLocationX += 30;
                        break;
                    case 2:
                        AbyssIsland(islandLocationX - 30, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
                        AbyssIsland(islandLocationX + 30, islandLocationY + 10, 60, 66, 30, 36, false, tenebrisSide, hasVoidstone);
                        AbyssIsland(islandLocationX, islandLocationY + 15, 60, 66, 30, 36, false, !tenebrisSide, hasVoidstone);
                        islandLocationX -= 30;
                        break;
                    case 3:
                        AbyssIsland(islandLocationX + 25, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
                        AbyssIsland(islandLocationX - 5, islandLocationY + 5, 60, 66, 30, 36, false, tenebrisSide, hasVoidstone);
                        AbyssIsland(islandLocationX - 35, islandLocationY + 15, 60, 66, 30, 36, false, !tenebrisSide, hasVoidstone);
                        islandLocationX += 25;
                        break;
                    case 4:
                        AbyssIsland(islandLocationX - 25, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
                        AbyssIsland(islandLocationX + 5, islandLocationY + 15, 60, 66, 30, 36, false, tenebrisSide, hasVoidstone);
                        AbyssIsland(islandLocationX + 35, islandLocationY + 5, 60, 66, 30, 36, false, !tenebrisSide, hasVoidstone);
                        islandLocationX -= 25;
                        break;
                }

                CalamityWorld.AbyssIslandX[CalamityWorld.numAbyssIslands] = islandLocationX;
                CalamityWorld.numAbyssIslands++;

                islandLocationY += islandLocationOffset;
                if (islandLocationY >= CalamityWorld.abyssChasmBottom - 50)
                    break;
            }

            CalamityWorld.AbyssItemArray = CalamityUtils.ShuffleArray(CalamityWorld.AbyssItemArray);
            for (int abyssHouse = 0; abyssHouse < CalamityWorld.numAbyssIslands; abyssHouse++) //11 15 19
            {
                if (abyssHouse != 20)
                    AbyssIslandHouse(CalamityWorld.AbyssIslandX[abyssHouse],
                        CalamityWorld.AbyssIslandY[abyssHouse],
                        CalamityWorld.AbyssItemArray[abyssHouse > 9 ? (abyssHouse - 10) : abyssHouse], //10 choices 0 to 9
                        CalamityWorld.AbyssIslandY[abyssHouse] > (rockLayer + y * 0.143));
            }

            if (CalamityWorld.abyssSide)
            {
                for (int abyssIndex = 0; abyssIndex < abyssChasmX + 80; abyssIndex++) //235
                {
                    for (int abyssIndex2 = 0; abyssIndex2 < abyssChasmY; abyssIndex2++)
                    {
                        if (!Main.tile[abyssIndex, abyssIndex2].HasTile)
                        {
                            if (WorldGen.SolidTile(abyssIndex, abyssIndex2 + 1) &&
                                abyssIndex2 > rockLayer)
                            {
                                if (WorldGen.genRand.Next(5) == 0)
                                {
                                    WorldGen.PlacePot(abyssIndex, abyssIndex2, (ushort)ModContent.TileType<AbyssalPots>());
                                    CalamityUtils.SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
                                }
                            }
                            else if (WorldGen.SolidTile(abyssIndex, abyssIndex2 + 1) &&
                                     abyssIndex2 < (int)Main.worldSurface)
                            {
                                if (WorldGen.genRand.Next(3) == 0)
                                {
                                    WorldGen.PlacePot(abyssIndex, abyssIndex2, (ushort)ModContent.TileType<SulphurousPots>());
                                    CalamityUtils.SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int abyssIndex = abyssChasmX - 80; abyssIndex < x; abyssIndex++) //3965
                {
                    for (int abyssIndex2 = 0; abyssIndex2 < abyssChasmY; abyssIndex2++)
                    {
                        if (!Main.tile[abyssIndex, abyssIndex2].HasTile)
                        {
                            if (WorldGen.SolidTile(abyssIndex, abyssIndex2 + 1) &&
                                abyssIndex2 > rockLayer)
                            {
                                if (WorldGen.genRand.Next(5) == 0)
                                {
                                    WorldGen.PlacePot(abyssIndex, abyssIndex2, (ushort)ModContent.TileType<AbyssalPots>());
                                    CalamityUtils.SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
                                }
                            }
                            else if (WorldGen.SolidTile(abyssIndex, abyssIndex2 + 1) &&
                                     abyssIndex2 < (int)Main.worldSurface)
                            {
                                if (WorldGen.genRand.Next(3) == 0)
                                {
                                    WorldGen.PlacePot(abyssIndex, abyssIndex2, (ushort)ModContent.TileType<SulphurousPots>());
                                    CalamityUtils.SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
                                }
                            }
                        }
                    }
                }
            }
        }

        #region Houses
        public static void AbyssIsland(int i, int j, int sizeMin, int sizeMax, int sizeMin2, int sizeMax2, bool hasChest, bool hasTenebris, bool isVoid)
        {
            int sizeMinSmall = sizeMin / 5;
            int sizeMaxSmall = sizeMax / 5;
            double num = (double)WorldGen.genRand.Next(sizeMin, sizeMax); //100 150
            float num2 = (float)WorldGen.genRand.Next(sizeMinSmall, sizeMaxSmall); //20 30
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
                double num11 = num * (double)WorldGen.genRand.Next(sizeMin, sizeMax) * 0.01; //80 120
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
                                Main.tile[k, l].TileType = (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>());
                                CalamityUtils.SafeSquareTileFrame(k, l, true);
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
                while (!Main.tile[m, num14].HasTile)
                {
                    num14--;
                }
                num14 += WorldGen.genRand.Next(-3, 4);
                num15 = WorldGen.genRand.Next(4, 8);
                int num16 = isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>();
                if (WorldGen.genRand.Next(4) == 0)
                {
                    num16 = hasChest ? ModContent.TileType<ChaoticOre>() : ModContent.TileType<PlantyMush>();
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
                                Main.tile[n, num17].TileType = (ushort)num16;
                                CalamityUtils.SafeSquareTileFrame(n, num17, true);
                            }
                        }
                    }
                }
            }
            if (hasTenebris)
            {
                int p = num3;
                int num150;
                for (p += WorldGen.genRand.Next(5); p < num4; p += WorldGen.genRand.Next(num150, (int)((double)num150 * 1.5)))
                {
                    int num14 = num6;
                    while (!Main.tile[p, num14].HasTile)
                    {
                        num14--;
                    }
                    num14 += WorldGen.genRand.Next(-3, 4); //-3 4
                    num150 = 1; //4 8
                    int num16 = ModContent.TileType<Tenebris>();
                    for (int n = p - num150; n <= p + num150; n++)
                    {
                        for (int num17 = num14 - num150; num17 <= num14 + num150; num17++)
                        {
                            if (num17 > num5)
                            {
                                float arg_409_0 = (float)Math.Abs(n - p);
                                float num18 = (float)(Math.Abs(num17 - num14) * 2);
                                if (Math.Sqrt((double)(arg_409_0 * arg_409_0 + num18 * num18)) < (double)(num150 + WorldGen.genRand.Next(2)))
                                {
                                    Main.tile[n, num17].active(true);
                                    Main.tile[n, num17].TileType = (ushort)num16;
                                    CalamityUtils.SafeSquareTileFrame(n, num17, true);
                                }
                            }
                        }
                    }
                }
            }
            int sizeMinSmall2 = sizeMin2 / 8;
            int sizeMaxSmall2 = sizeMax2 / 8;
            num = (double)WorldGen.genRand.Next(sizeMin2, sizeMax2);
            num2 = (float)WorldGen.genRand.Next(sizeMinSmall2, sizeMaxSmall2);
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
                while ((!Main.tile[num23, num24].HasTile || Main.tile[num23, num24].TileType != 0) && num23 < num4)
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
                    int num26 = isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>();
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
                                    Main.tile[num27, num28].TileType = (ushort)num26;
                                    CalamityUtils.SafeSquareTileFrame(num27, num28, true);
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
                            if (!Main.tile[num32, num33].HasTile)
                            {
                                flag = false;
                            }
                        }
                    }
                    if (flag)
                    {
                        Main.tile[num30, num31].WallType = (ushort)(isVoid ? ModContent.WallType<VoidstoneWallUnsafe>() : ModContent.WallType<AbyssGravelWall>());
                        WorldGen.SquareWallFrame(num30, num31, true);
                    }
                }
            }
            for (int num34 = num3; num34 <= num4; num34++)
            {
                int num35 = num5 - 10;
                while (!Main.tile[num34, num35 + 1].HasTile)
                {
                    num35++;
                }
                if (num35 < num6 && Main.tile[num34, num35 + 1].TileType == (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>()))
                {
                    if (WorldGen.genRand.Next(10) == 0)
                    {
                        int num36 = WorldGen.genRand.Next(1, 3);
                        for (int num37 = num34 - num36; num37 <= num34 + num36; num37++)
                        {
                            if (Main.tile[num37, num35].TileType == (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>()))
                            {
                                Main.tile[num37, num35].active(false);
                                Main.tile[num37, num35].LiquidAmount = 255;
                                Main.tile[num37, num35].LiquidType = LiquidID.Water;
                                CalamityUtils.SafeSquareTileFrame(num34, num35, true);
                            }
                            if (Main.tile[num37, num35 + 1].TileType == (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>()))
                            {
                                Main.tile[num37, num35 + 1].active(false);
                                Main.tile[num37, num35 + 1].LiquidAmount = 255;
                                Main.tile[num37, num35 + 1].LiquidType = LiquidID.Water;
                                CalamityUtils.SafeSquareTileFrame(num34, num35 + 1, true);
                            }
                            if (num37 > num34 - num36 && num37 < num34 + 2 && Main.tile[num37, num35 + 2].TileType == (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>()))
                            {
                                Main.tile[num37, num35 + 2].active(false);
                                Main.tile[num37, num35 + 2].LiquidAmount = 255;
                                Main.tile[num37, num35 + 2].LiquidType = LiquidID.Water;
                                CalamityUtils.SafeSquareTileFrame(num34, num35 + 2, true);
                            }
                        }
                    }
                    if (WorldGen.genRand.Next(5) == 0)
                    {
                        Main.tile[num34, num35].LiquidAmount = 255;
                    }
                    Main.tile[num34, num35].LiquidType = LiquidID.Water;
                    CalamityUtils.SafeSquareTileFrame(num34, num35, true);
                }
            }
        }

        public static void AbyssIslandHouse(int i, int j, int itemChoice, bool isVoid)
        {
            ushort type = (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>()); //tile
            ushort wall = (ushort)(isVoid ? ModContent.WallType<VoidstoneWallUnsafe>() : ModContent.WallType<AbyssGravelWall>()); //wall
            Vector2 vector = new Vector2((float)i, (float)j);
            int num = 1;
            if (WorldGen.genRand.Next(2) == 0)
            {
                num = -1;
            }
            int num2 = WorldGen.genRand.Next(5, 9);
            int num3 = 3;
            vector.X = (float)(i + (num2 + 2) * num);
            for (int k = j - 15; k < j + 30; k++)
            {
                if (Main.tile[(int)vector.X, k].HasTile)
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
                        Main.tile[l, m].TileType = type;
                        Main.tile[l, m].WallType = wall;
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
                    if ((num8 != num6 || (n != num4 && n != num5)) && Main.tile[n, num8].WallType == wall)
                    {
                        Main.tile[n, num8].active(false);
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
            switch (itemChoice)
            {
                case 0:
                    itemChoice = ModContent.ItemType<TorrentialTear>();
                    break; //rain item
                case 1:
                    itemChoice = ModContent.ItemType<IronBoots>();
                    break; //movement acc
                case 2:
                    itemChoice = ModContent.ItemType<DepthCharm>();
                    break; //regen acc
                case 3:
                    itemChoice = ModContent.ItemType<Archerfish>();
                    break; //ranged
                case 4:
                    itemChoice = ModContent.ItemType<AnechoicPlating>();
                    break; //defense acc
                case 5:
                    itemChoice = ModContent.ItemType<BallOFugu>();
                    break; //melee
                case 6:
                    itemChoice = ModContent.ItemType<StrangeOrb>();
                    break; //light pet
                case 7:
                    itemChoice = ModContent.ItemType<HerringStaff>();
                    break; //summon
                case 8:
                    itemChoice = ModContent.ItemType<BlackAnurian>();
                    break; //magic
                case 9:
                    itemChoice = ModContent.ItemType<Lionfish>();
                    break; //throwing
                default:
                    itemChoice = 497;
                    break;
            }
            WorldGen.AddBuriedChest(i, num10 - 3, itemChoice, false, 4); //chest
        }
        #endregion

    }
}
