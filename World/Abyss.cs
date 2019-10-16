using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.World
{
    public class Abyss : ModWorld
    {
        public static void PlaceSulphurSea()
        {
            Mod mod = ModLoader.GetMod("CalamityMod");

            int x = Main.maxTilesX;
            int y = Main.maxTilesY;
            int genLimit = x / 2;

            if (WorldGen.dungeonX < genLimit)
                CalamityWorld.abyssSide = true;

            int abyssChasmX = CalamityWorld.abyssSide ? genLimit - (genLimit - 135) : genLimit + (genLimit - 135); //2100 - 1965 = 135 : 2100 + 1965 = 4065

            if (CalamityWorld.abyssSide)
            {
                for (int abyssIndexSand = 0; abyssIndexSand < abyssChasmX + 240; abyssIndexSand++)
                {
                    for (int abyssIndexSand2 = 0; abyssIndexSand2 < y - 200; abyssIndexSand2++)
                    {
                        Tile tile = Framing.GetTileSafely(abyssIndexSand, abyssIndexSand2);
                        if (abyssIndexSand > abyssChasmX + 225)
                        {
                            if (tile.active() &&
                                tile.type == TileID.Sand &&
                                WorldGen.genRand.Next(4) == 0)
                            {
                                tile.type = (ushort)ModContent.TileType<SulphurousSand>();
                            }
                            else if (tile.active() &&
                                     tile.type == TileID.PalmTree)
                            {
                                tile.active(false);
                            }
                        }
                        else if (abyssIndexSand > abyssChasmX + 210)
                        {
                            if (tile.active() &&
                                tile.type == TileID.Sand &&
                                WorldGen.genRand.Next(2) == 0)
                            {
                                tile.type = (ushort)ModContent.TileType<SulphurousSand>();
                            }
                            else if (tile.active() &&
                                     tile.type == TileID.PalmTree)
                            {
                                tile.active(false);
                            }
                        }
                        else
                        {
                            if (tile.active() &&
                                tile.type == TileID.Sand)
                            {
                                tile.type = (ushort)ModContent.TileType<SulphurousSand>();
                            }
                            else if (tile.active() &&
                                     tile.type == TileID.PalmTree)
                            {
                                tile.active(false);
                            }
                        }
                    }
                }
            }
            else
            {
                for (int abyssIndexSand = abyssChasmX - 240; abyssIndexSand < x; abyssIndexSand++)
                {
                    for (int abyssIndexSand2 = 0; abyssIndexSand2 < y - 200; abyssIndexSand2++)
                    {
                        Tile tile = Framing.GetTileSafely(abyssIndexSand, abyssIndexSand2);
                        if (abyssIndexSand < abyssChasmX - 225)
                        {
                            if (tile.active() &&
                                tile.type == TileID.Sand &&
                                WorldGen.genRand.Next(4) == 0)
                            {
                                tile.type = (ushort)ModContent.TileType<SulphurousSand>();
                            }
                            else if (tile.active() &&
                                     tile.type == TileID.PalmTree)
                            {
                                tile.active(false);
                            }
                        }
                        else if (abyssIndexSand < abyssChasmX - 210)
                        {
                            if (tile.active() &&
                                tile.type == TileID.Sand &&
                                WorldGen.genRand.Next(2) == 0)
                            {
                                tile.type = (ushort)ModContent.TileType<SulphurousSand>();
                            }
                            else if (tile.active() &&
                                     tile.type == TileID.PalmTree)
                            {
                                tile.active(false);
                            }
                        }
                        else
                        {
                            if (tile.active() &&
                                tile.type == TileID.Sand)
                            {
                                tile.type = (ushort)ModContent.TileType<SulphurousSand>();
                            }
                            else if (tile.active() &&
                                     tile.type == TileID.PalmTree)
                            {
                                tile.active(false);
                            }
                        }
                    }
                }
            }
        }

        public static void PlaceAbyss()
        {
            Mod mod = ModLoader.GetMod("CalamityMod");

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
                        bool canConvert = tile.active() &&
                            tile.type < TileID.Count &&
                            tile.type != TileID.DyePlants &&
                            tile.type != TileID.Trees &&
                            tile.type != TileID.PalmTree &&
                            tile.type != TileID.Sand &&
                            tile.type != TileID.Containers &&
                            tile.type != TileID.Coral &&
                            tile.type != TileID.BeachPiles;
                        if (abyssIndex > abyssChasmX + 75)
                        {
                            if (WorldGen.genRand.Next(4) == 0)
                            {
                                if (canConvert)
                                {
                                    tile.wall = (ushort)ModContent.WallType<AbyssGravelWall>();
                                    tile.type = (ushort)ModContent.TileType<AbyssGravel>();
                                }
                                else if (!tile.active() &&
                                          abyssIndex2 > rockLayer)
                                {
                                    tile.active(true);
                                    tile.wall = (ushort)ModContent.WallType<AbyssGravelWall>();
                                    tile.type = (ushort)ModContent.TileType<AbyssGravel>();
                                }
                            }
                        }
                        else if (abyssIndex > abyssChasmX + 70)
                        {
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                if (canConvert)
                                {
                                    tile.wall = (ushort)ModContent.WallType<AbyssGravelWall>();
                                    tile.type = (ushort)ModContent.TileType<AbyssGravel>();
                                }
                                else if (!tile.active() &&
                                          abyssIndex2 > rockLayer)
                                {
                                    tile.active(true);
                                    tile.wall = (ushort)ModContent.WallType<AbyssGravelWall>();
                                    tile.type = (ushort)ModContent.TileType<AbyssGravel>();
                                }
                            }
                        }
                        else
                        {
                            if (canConvert)
                            {
                                if (abyssIndex2 > (rockLayer + y * 0.262))
                                {
                                    tile.type = (ushort)ModContent.TileType<Voidstone>();
                                    tile.wall = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                }
                                else if (abyssIndex2 > (rockLayer + y * 0.143) && WorldGen.genRand.Next(3) == 0)
                                {
                                    tile.type = (ushort)ModContent.TileType<Voidstone>();
                                    tile.wall = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                }
                                else
                                {
                                    tile.type = (ushort)ModContent.TileType<AbyssGravel>();
                                    tile.wall = (ushort)ModContent.WallType<AbyssGravelWall>();
                                }
                            }
                            else if (!tile.active() &&
                                      abyssIndex2 > rockLayer)
                            {
                                tile.active(true);
                                if (abyssIndex2 > (rockLayer + y * 0.262))
                                {
                                    tile.type = (ushort)ModContent.TileType<Voidstone>();
                                    tile.wall = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                }
                                else if (abyssIndex2 > (rockLayer + y * 0.143) && WorldGen.genRand.Next(3) == 0)
                                {
                                    tile.type = (ushort)ModContent.TileType<Voidstone>();
                                    tile.wall = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                }
                                else
                                {
                                    tile.wall = (ushort)ModContent.WallType<AbyssGravelWall>();
                                    tile.type = (ushort)ModContent.TileType<AbyssGravel>();
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
                        bool canConvert = tile.active() &&
                            tile.type < TileID.Count &&
                            tile.type != TileID.DyePlants &&
                            tile.type != TileID.Trees &&
                            tile.type != TileID.PalmTree &&
                            tile.type != TileID.Sand &&
                            tile.type != TileID.Containers &&
                            tile.type != TileID.Coral &&
                            tile.type != TileID.BeachPiles;
                        if (abyssIndex < abyssChasmX - 75)
                        {
                            if (WorldGen.genRand.Next(4) == 0)
                            {
                                if (canConvert)
                                {
                                    tile.wall = (ushort)ModContent.WallType<AbyssGravelWall>();
                                    tile.type = (ushort)ModContent.TileType<AbyssGravel>();
                                }
                                else if (!tile.active() &&
                                          abyssIndex2 > rockLayer)
                                {
                                    tile.active(true);
                                    tile.wall = (ushort)ModContent.WallType<AbyssGravelWall>();
                                    tile.type = (ushort)ModContent.TileType<AbyssGravel>();
                                }
                            }
                        }
                        else if (abyssIndex < abyssChasmX - 70)
                        {
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                if (canConvert)
                                {
                                    tile.wall = (ushort)ModContent.WallType<AbyssGravelWall>();
                                    tile.type = (ushort)ModContent.TileType<AbyssGravel>();
                                }
                                else if (!tile.active() &&
                                          abyssIndex2 > rockLayer)
                                {
                                    tile.active(true);
                                    tile.wall = (ushort)ModContent.WallType<AbyssGravelWall>();
                                    tile.type = (ushort)ModContent.TileType<AbyssGravel>();
                                }
                            }
                        }
                        else
                        {
                            if (canConvert)
                            {
                                if (abyssIndex2 > (rockLayer + y * 0.262))
                                {
                                    tile.type = (ushort)ModContent.TileType<Voidstone>();
                                    tile.wall = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                }
                                else if (abyssIndex2 > (rockLayer + y * 0.143) && WorldGen.genRand.Next(3) == 0)
                                {
                                    tile.type = (ushort)ModContent.TileType<Voidstone>();
                                    tile.wall = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                }
                                else
                                {
                                    tile.type = (ushort)ModContent.TileType<AbyssGravel>();
                                    tile.wall = (ushort)ModContent.WallType<AbyssGravelWall>();
                                }
                            }
                            else if (!tile.active() &&
                                      abyssIndex2 > rockLayer)
                            {
                                tile.active(true);
                                if (abyssIndex2 > (rockLayer + y * 0.262))
                                {
                                    tile.type = (ushort)ModContent.TileType<Voidstone>();
                                    tile.wall = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                }
                                else if (abyssIndex2 > (rockLayer + y * 0.143) && WorldGen.genRand.Next(3) == 0)
                                {
                                    tile.type = (ushort)ModContent.TileType<Voidstone>();
                                    tile.wall = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                }
                                else
                                {
                                    tile.wall = (ushort)ModContent.WallType<AbyssGravelWall>();
                                    tile.type = (ushort)ModContent.TileType<AbyssGravel>();
                                }
                            }
                        }
                    }
                }
            }

            WorldGenerationMethods.ChasmGenerator(abyssChasmX, (int)WorldGen.worldSurfaceLow, CalamityWorld.abyssChasmBottom, true);

            int maxAbyssIslands = 11; //Small World
            if (y > 2100)
                maxAbyssIslands = 20; //Large World
            else if (y > 1500)
                maxAbyssIslands = 16; //Medium World

            WorldGenerationMethods.SpecialHut((ushort)ModContent.TileType<SmoothVoidstone>(), (ushort)ModContent.TileType<Voidstone>(),
                (ushort)ModContent.WallType<VoidstoneWallUnsafe>(), 9, abyssChasmX, CalamityWorld.abyssChasmBottom);

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
                        WorldGenerationMethods.AbyssIsland(islandLocationX, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
                        WorldGenerationMethods.AbyssIsland(islandLocationX + 40, islandLocationY + 15, 60, 66, 30, 36, false, tenebrisSide, hasVoidstone);
                        WorldGenerationMethods.AbyssIsland(islandLocationX - 40, islandLocationY + 15, 60, 66, 30, 36, false, !tenebrisSide, hasVoidstone);
                        break;
                    case 1:
                        WorldGenerationMethods.AbyssIsland(islandLocationX + 30, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
                        WorldGenerationMethods.AbyssIsland(islandLocationX, islandLocationY + 15, 60, 66, 30, 36, false, tenebrisSide, hasVoidstone);
                        WorldGenerationMethods.AbyssIsland(islandLocationX - 40, islandLocationY + 10, 60, 66, 30, 36, false, !tenebrisSide, hasVoidstone);
                        islandLocationX += 30;
                        break;
                    case 2:
                        WorldGenerationMethods.AbyssIsland(islandLocationX - 30, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
                        WorldGenerationMethods.AbyssIsland(islandLocationX + 30, islandLocationY + 10, 60, 66, 30, 36, false, tenebrisSide, hasVoidstone);
                        WorldGenerationMethods.AbyssIsland(islandLocationX, islandLocationY + 15, 60, 66, 30, 36, false, !tenebrisSide, hasVoidstone);
                        islandLocationX -= 30;
                        break;
                    case 3:
                        WorldGenerationMethods.AbyssIsland(islandLocationX + 25, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
                        WorldGenerationMethods.AbyssIsland(islandLocationX - 5, islandLocationY + 5, 60, 66, 30, 36, false, tenebrisSide, hasVoidstone);
                        WorldGenerationMethods.AbyssIsland(islandLocationX - 35, islandLocationY + 15, 60, 66, 30, 36, false, !tenebrisSide, hasVoidstone);
                        islandLocationX += 25;
                        break;
                    case 4:
                        WorldGenerationMethods.AbyssIsland(islandLocationX - 25, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
                        WorldGenerationMethods.AbyssIsland(islandLocationX + 5, islandLocationY + 15, 60, 66, 30, 36, false, tenebrisSide, hasVoidstone);
                        WorldGenerationMethods.AbyssIsland(islandLocationX + 35, islandLocationY + 5, 60, 66, 30, 36, false, !tenebrisSide, hasVoidstone);
                        islandLocationX -= 25;
                        break;
                }
                CalamityWorld.AbyssIslandX[CalamityWorld.numAbyssIslands] = islandLocationX;
                CalamityWorld.numAbyssIslands++;
                islandLocationY += islandLocationOffset;
            }

            CalamityWorld.AbyssItemArray = WorldGenerationMethods.ShuffleArray(CalamityWorld.AbyssItemArray);
            for (int abyssHouse = 0; abyssHouse < CalamityWorld.numAbyssIslands; abyssHouse++) //11 15 19
            {
                if (abyssHouse != 20)
                    WorldGenerationMethods.AbyssIslandHouse(CalamityWorld.AbyssIslandX[abyssHouse],
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
                        if (!Main.tile[abyssIndex, abyssIndex2].active())
                        {
                            if (WorldGen.SolidTile(abyssIndex, abyssIndex2 + 1) &&
                                abyssIndex2 > rockLayer)
                            {
                                if (WorldGen.genRand.Next(5) == 0)
                                {
                                    int style = WorldGen.genRand.Next(13, 16);
                                    WorldGen.PlacePot(abyssIndex, abyssIndex2, 28, style);
                                    WorldGenerationMethods.SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
                                }
                            }
                            else if (WorldGen.SolidTile(abyssIndex, abyssIndex2 + 1) &&
                                     abyssIndex2 < (int)Main.worldSurface)
                            {
                                if (WorldGen.genRand.Next(3) == 0)
                                {
                                    int style = WorldGen.genRand.Next(25, 28);
                                    WorldGen.PlacePot(abyssIndex, abyssIndex2, 28, style);
                                    WorldGenerationMethods.SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
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
                        if (!Main.tile[abyssIndex, abyssIndex2].active())
                        {
                            if (WorldGen.SolidTile(abyssIndex, abyssIndex2 + 1) &&
                                abyssIndex2 > rockLayer)
                            {
                                if (WorldGen.genRand.Next(5) == 0)
                                {
                                    int style = WorldGen.genRand.Next(13, 16);
                                    WorldGen.PlacePot(abyssIndex, abyssIndex2, 28, style);
                                    WorldGenerationMethods.SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
                                }
                            }
                            else if (WorldGen.SolidTile(abyssIndex, abyssIndex2 + 1) &&
                                     abyssIndex2 < (int)Main.worldSurface)
                            {
                                if (WorldGen.genRand.Next(3) == 0)
                                {
                                    int style = WorldGen.genRand.Next(25, 28);
                                    WorldGen.PlacePot(abyssIndex, abyssIndex2, 28, style);
                                    WorldGenerationMethods.SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
