using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.FurnitureVoid;
using CalamityMod.Walls;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.World
{
    public class Abyss : ModWorld
    {
        public static void PlaceSulphurSea()
        {
            CalamityWorld.abyssSide = Main.dungeonX < Main.maxTilesX / 2;

            // How wide the biome is.

            int biomeWidth;
            int blockDepth;

            // Small world
            if (Main.maxTilesX == 4200)
            {
                biomeWidth = 270;
                blockDepth = 160;
            }
            // Medium world
            else if (Main.maxTilesX == 6400)
            {
                biomeWidth = 365;
                blockDepth = 240;
            }
            // Large world
            else
            {
                biomeWidth = 430;
                blockDepth = 310;
            }
            // How deep the biome goes.
            const int yDescent = 80;
            int yStart = DetermineYStart(biomeWidth, yDescent);

            PlaceShallowSea(biomeWidth, yStart, yDescent);
            PlaceCavernBlock(biomeWidth, yStart, blockDepth);
            PlaceWaterCaverns(biomeWidth, yStart - 35);
            PlaceScenery(yStart, 135, biomeWidth);
            RemoveWeirdSlopes(yStart, biomeWidth);

            // A copy of old sulf sea code that replaced all sand with sulph sea/abyss gravel. 
            // Made by Fabsol.
            ReplaceRemainingSand();
            PlaceMiscTiles(biomeWidth, yStart + 20, 90);
            SmoothenSea();

            // Empty cavern pockets were becoming a problem.
            ReplaceAirWithWater(yStart, biomeWidth);
            PlaceTrees(biomeWidth);
        }
        public static int DetermineYStart(int biomeWidth, int yDescent)
        {
            int maxHeight = int.MaxValue;
            for (int i = 0; i < 15; i++)
            {
                int xCheck = CalamityWorld.abyssSide ?
                    biomeWidth - i :
                    Main.maxTilesX - biomeWidth + i;
                int yStart = 1;
                while (!Main.tile[xCheck, yStart].active())
                {
                    yStart++;
                    if (yStart > (int)Main.rockLayer - yDescent - 5)
                        break;
                }
                if (maxHeight > yStart)
                    maxHeight = yStart;
            }
            return maxHeight;
        }
        public static void PlaceShallowSea(int biomeWidth, int yStart, int yDescent)
        {
            for (int x = 0; x < biomeWidth; x++)
            {
                int trueX = x;
                if (!CalamityWorld.abyssSide)
                {
                    trueX = Main.maxTilesX - x;
                }
                float xRatio = MathHelper.Clamp(trueX / (float)biomeWidth, 0f, 0.95f);
                for (int y = yStart; y < yStart + yDescent; y++)
                {
                    if (1f - ((y - yStart) / (float)yDescent) < xRatio)
                    {
                        if (Main.tile[trueX, y] == null)
                            Main.tile[trueX, y] = new Tile();
                        Main.tile[trueX, y].type = (ushort)ModContent.TileType<SulphurousSand>();
                        Main.tile[trueX, y].slope(0);
                        Main.tile[trueX, y].active(true);
                    }
                    else
                    {
                        Main.tile[trueX, y] = new Tile()
                        {
                            liquid = 255
                        };
                    }
                }
            }
        }
        public static void PlaceCavernBlock(int biomeWidth, int yStart, int blockDepth)
        {
            int trueYStart = yStart + 18;
            for (int x = 0; x < biomeWidth; x++)
            {
                for (int y = 0; y < blockDepth; y++)
                {
                    int trueX = x;
                    if (!CalamityWorld.abyssSide)
                    {
                        trueX = Main.maxTilesX - x;
                    }
                    if (Framing.GetTileSafely(trueX, y + trueYStart).liquid == 0 &&
                        y < blockDepth - 36 + Math.Sin(MathHelper.Pi * x / biomeWidth) * 36)
                    {
                        Main.tile[trueX, y + trueYStart].type = (ushort)ModContent.TileType<SulphurousSand>();
                    }
                    Main.tile[trueX, y + trueYStart].slope(0);
                    Main.tile[trueX, y + trueYStart].active(true);
                }
            }
        }
        public static void PlaceWaterCaverns(int biomeWidth, int yStart)
        {
            // This is used in other vanilla worldgen. However, it does not affect any worldgen after this.
            // This is basically the point where TileRunner stops spawning water.
            WorldGen.waterLine = 1;
            int xStart = CalamityWorld.abyssSide ? biomeWidth / 2 :
                Main.maxTilesX - biomeWidth / 2;
            for (int c = 0; c < 6; c++)
            {
                Vector2 startingPosition = new Vector2(xStart, yStart);
                while (!Main.tile[(int)startingPosition.X, (int)startingPosition.Y].active())
                {
                    startingPosition.Y++;
                }
                float xDirection = WorldGen.genRand.NextFloat() * 0.2f - 0.2f;
                float yDirection = 1f - xDirection;
                if (c == 0)
                {
                    xDirection = WorldGen.genRand.NextFloat(1f, 2f);
                    yDirection = 2f;
                }
                if (WorldGen.genRand.NextBool(2))
                {
                    xDirection *= -1;
                }
                if (WorldGen.genRand.NextBool(2))
                {
                    yDirection *= -1;
                }
                for (int i = 0; i < 11; i++)
                {
                    startingPosition = WorldGen.digTunnel(startingPosition.X, startingPosition.Y, xDirection, yDirection, WorldGen.genRand.Next(6, 20), WorldGen.genRand.Next(4, 9), false);
                    xDirection += WorldGen.genRand.NextFloat(-0.5f, 0.5f);
                    yDirection += WorldGen.genRand.NextFloat(-0.2f, 0.2f);
                    xDirection = MathHelper.Clamp(xDirection, -4f, 4f);
                    yDirection = MathHelper.Clamp(yDirection, -1f, 3f);
                    float xdirection2 = WorldGen.genRand.NextFloat(-0.4f, 0.4f);
                    float ydirection2 = WorldGen.genRand.NextFloat(-0.2f, 0.2f);
                    if (WorldGen.genRand.NextBool(2))
                    {
                        xdirection2 *= -1;
                    }
                    if (WorldGen.genRand.NextBool(2))
                    {
                        ydirection2 *= -1;
                    }
                    Vector2 tunnelVector = WorldGen.digTunnel(startingPosition.X, startingPosition.Y, xdirection2, ydirection2, WorldGen.genRand.Next(30, 50), WorldGen.genRand.Next(5, 10 + 1), false);

                    // The -2 parameter causes the tile-runner to generate liquids. Water above the lavaLine and lava below.
                    WorldGen.TileRunner((int)tunnelVector.X, (int)tunnelVector.Y, WorldGen.genRand.Next(24, 32 + 1), WorldGen.genRand.Next(19, 24 + 1), -2, false, 0f, 0f, false, true);
                }
            }
        }
        public static void PlaceScenery(int yStart, int yDescent, int biomeWidth)
        {
            // Vents
            for (int x = 0; x < biomeWidth; x++)
            {
                int trueX = x;
                if (!CalamityWorld.abyssSide)
                {
                    trueX = Main.maxTilesX - x;
                }
                for (int y = yStart - 30; y < yStart + yDescent; y++)
                {
                    if (WorldGen.genRand.NextBool(9))
                    {
                        if (CalamityUtils.TileSelectionSolid(trueX, y, 2, -2))
                            break;
                        WorldGen.PlaceTile(trueX, y, ModContent.TileType<SteamGeyser>());
                    }
                }
            }

            // Rock pillars in the water
            for (int c = 1; c <= 5; c++)
            {
                int xStart = 0;
                switch (c)
                {
                    case 1:
                        xStart = 40;
                        break;
                    case 2:
                        xStart = 60;
                        break;
                    case 3:
                        xStart = 90;
                        break;
                    case 4:
                        xStart = 140;
                        break;
                    case 5:
                        xStart = 200;
                        break;
                }
                if (!CalamityWorld.abyssSide)
                {
                    xStart = Main.maxTilesX - xStart;
                }
                // Adjust Y positioning a bit
                int dy = -40;
                while (!CalamityUtils.ParanoidTileRetrieval(xStart, yStart + dy).active() ||
                        CalamityUtils.ParanoidTileRetrieval(xStart, yStart + dy).type != (ushort)ModContent.TileType<SulphurousSand>())
                {
                    if (WorldGen.InWorld(xStart, yStart + dy + 1))
                    {
                        if (Main.tile[xStart, yStart + dy + 1] == null)
                            Main.tile[xStart, yStart + dy + 1] = new Tile();
                    }
                    else
                    {
                        dy++;
                        continue;
                    }
                    dy++;
                    if (yStart + dy > Main.rockLayer - 30)
                        break;
                }
                int ascent = WorldGen.genRand.Next(4 * c, 8 * c + 3) + 7;

                int widthTop = WorldGen.genRand.Next(2, 4 + 1);
                int widthBottomAdditive = WorldGen.genRand.Next(13, 20 + 1);
                float root = WorldGen.genRand.NextFloat(1.6f, 3.2f);
                for (int y = yStart - ascent; y <= yStart + 6; y++)
                {
                    // The root is used to give a less linear scale
                    float yRatio = (float)Math.Pow(1f - (-(y - yStart) / (float)ascent), 1f / root);
                    int width = widthTop + (int)(widthBottomAdditive * yRatio);
                    for (int x = xStart - width / 2; x <= xStart + width / 2; x++)
                    {
                        Main.tile[x, y + dy] = new Tile
                        {
                            type = (ushort)ModContent.TileType<SulphurousSand>()
                        };
                        Main.tile[x, y + dy].slope(0);
                        Main.tile[x, y + dy].active(true);
                    }
                }
            }
        }
        public static void ReplaceAirWithWater(int yStart, int biomeWidth)
        {
            for (int x = 0; x < biomeWidth; x++)
            {
                int trueX = x;
                if (!CalamityWorld.abyssSide)
                {
                    trueX = Main.maxTilesX - x;
                }
                for (int y = yStart + 35; y < yStart + 180; y++)
                {
                    if (CalamityUtils.ParanoidTileRetrieval(trueX, y).type != (ushort)ModContent.TileType<SulphurousSand>() ||
                        !CalamityUtils.ParanoidTileRetrieval(trueX, y).active())
                    {
                        Main.tile[trueX, y] = new Tile()
                        {
                            liquid = 255,
                            wall = (ushort)ModContent.WallType<SulphurousSandstoneWall>()
                        };
                    }
                }
            }
        }
        public static void RemoveWeirdSlopes(int yStart, int biomeWidth)
        {
            if (CalamityWorld.abyssSide)
            {
                for (int x = 0; x < biomeWidth; x++)
                {
                    for (int y = yStart - 1; y >= yStart - 60; y--)
                    {
                        if (Main.tile[x, y].type == ModContent.TileType<SulphurousSand>())
                        {
                            Main.tile[x, y] = new Tile();
                        }
                    }
                }
            }
            else
            {
                for (int x = Main.maxTilesX - biomeWidth; x < Main.maxTilesX; x++)
                {
                    for (int y = yStart - 1; y >= yStart - 60; y--)
                    {
                        if (Main.tile[x, y].type == ModContent.TileType<SulphurousSand>())
                        {
                            Main.tile[x, y] = new Tile();
                        }
                    }
                }
            }
        }
        public static void ReplaceRemainingSand()
        {
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
                            else if (tile.active() &&
                                     tile.type == TileID.Trees)
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
                            else if (tile.active() &&
                                     tile.type == TileID.Trees)
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
                            else if (tile.active() &&
                                     tile.type == TileID.Trees)
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
                            else if (tile.active() &&
                                     tile.type == TileID.Trees)
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
                            else if (tile.active() &&
                                     tile.type == TileID.Trees)
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
                            else if (tile.active() &&
                                     tile.type == TileID.Trees)
                            {
                                tile.active(false);
                            }
                        }
                    }
                }
            }
        }
        public static void PlaceMiscTiles(int biomeWidth, int yStart, int maximumDepth)
        {
            // Kelp
            for (int x = 0; x < biomeWidth; x++)
            {
                int trueX = x;
                if (!CalamityWorld.abyssSide)
                {
                    trueX = Main.maxTilesX - x;
                }
                for (int y = yStart; y < yStart + maximumDepth; y++)
                {
                    if (WorldGen.genRand.NextBool(24))
                    {
                        if (CalamityUtils.TileSelectionSolid(trueX, y, 1, 2) && CalamityUtils.ParanoidTileRetrieval(trueX, y).type == ModContent.TileType<SulphurousSand>())
                            break;
                        WorldGen.PlaceTile(trueX, y, TileID.DyePlants, style:5);
                    }
                }
            }
        }
        public static void SmoothenSea()
        {
            for (int x = 1; x < Main.maxTilesX - 1; x++)
            {
                int trueX = x;
                if (!CalamityWorld.abyssSide)
                {
                    trueX = Main.maxTilesX - x;
                }
                for (int y = 1; y < Main.maxTilesY * 0.5; y++)
                {
                    if (Framing.GetTileSafely(trueX, y).type == (ushort)ModContent.TileType<SulphurousSand>())
                    {
                        Main.tile[trueX, y].slope(0);
                        Main.tile[trueX, y].halfBrick(false);
                        try
                        {
                            Tile.SmoothSlope(trueX, y);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }
        }
        public static void PlaceTrees(int biomeWidth)
        {
            for (int x = 1; x < biomeWidth + 50; x++)
            {
                int trueX = x;
                if (!CalamityWorld.abyssSide)
                {
                    trueX = Main.maxTilesX - x;
                }
                for (int y = 30; y < (int)Main.rockLayer; y++)
                {
                    if (!CalamityUtils.TileSelectionSolid(trueX, y, 1, -30) && Main.rand.NextBool(15) &&
                        CalamityUtils.ParanoidTileRetrieval(trueX, y + 1).active() &&
                        CalamityUtils.ParanoidTileRetrieval(trueX, y + 1).type == ModContent.TileType<SulphurousSand>())
                    {
                        WorldGen.PlaceTile(trueX, y - 1, ModContent.TileType<AcidWoodTreeSapling>());
                        bool success = GrowSaplingImmediately(trueX, y - 1);
                        if (!success && 
                            Main.tile[trueX, y - 1].type == ModContent.TileType<AcidWoodTreeSapling>() &&
                            Main.tile[trueX, y - 2].type == ModContent.TileType<AcidWoodTreeSapling>())
                        {
                            Main.tile[trueX, y - 1] = new Tile();
                            Main.tile[trueX, y - 2] = new Tile();
                        }
                    }
                }
            }
        }
        public static bool GrowSaplingImmediately(int i, int j)
        {
            int trueStartingPositionY = j;
            while (TileLoader.IsSapling((int)Main.tile[i, trueStartingPositionY].type))
            {
                trueStartingPositionY++;
            }
            Tile tileAtPosition = Main.tile[i, trueStartingPositionY];
            Tile tileAbovePosition = Main.tile[i, trueStartingPositionY - 1];
            if (!tileAtPosition.active() || tileAtPosition.halfBrick() || tileAtPosition.slope() != 0)
            {
                return false;
            }
            if (tileAbovePosition.wall != 0)
            {
                return false;
            }
            if (!WorldGen.EmptyTileCheck(i - 1, i + 1, trueStartingPositionY - 30, trueStartingPositionY - 1, 20))
            {
                return false;
            }
            int treeHeight = WorldGen.genRand.Next(10, 21);
            int frameYIdeal = WorldGen.genRand.Next(-8, 9);
            frameYIdeal *= 2;
            short frameY = 0;
            for (int k = 0; k < treeHeight; k++)
            {
                tileAtPosition = Main.tile[i, trueStartingPositionY - 1 - k];
                if (k == 0)
                {
                    tileAtPosition.active(true);
                    tileAtPosition.type = TileID.PalmTree;
                    tileAtPosition.frameX = 66;
                    tileAtPosition.frameY = 0;
                }
                else if (k == treeHeight - 1)
                {
                    tileAtPosition.active(true);
                    tileAtPosition.type = TileID.PalmTree;
                    tileAtPosition.frameX = (short)(22 * WorldGen.genRand.Next(4, 7));
                    tileAtPosition.frameY = frameY;
                }
                else
                {
                    if (frameY != frameYIdeal)
                    {
                        float heightRatio = k / (float)treeHeight;
                        bool increaseFrameY = heightRatio >= 0.25f && ((heightRatio < 0.5f && WorldGen.genRand.Next(13) == 0) || (heightRatio < 0.7f && WorldGen.genRand.Next(9) == 0) || heightRatio >= 0.95f || WorldGen.genRand.Next(5) != 0 || true);
                        if (increaseFrameY)
                        {
                            frameY += (short)(Math.Sign(frameYIdeal) * 2);
                        }
                    }
                    tileAtPosition.active(true);
                    tileAtPosition.type = TileID.PalmTree;
                    tileAtPosition.frameX = (short)(22 * WorldGen.genRand.Next(0, 3));
                    tileAtPosition.frameY = frameY;
                }
            }
            WorldGen.RangeFrame(i - 2, trueStartingPositionY - treeHeight - 1, i + 2, trueStartingPositionY + 1);
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendTileSquare(-1, i, (int)((double)trueStartingPositionY - (double)treeHeight * 0.5), treeHeight + 1, TileChangeType.None);
            }
            return true;
        }
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

            CalamityWorld.AbyssItemArray = CalamityUtils.ShuffleArray(CalamityWorld.AbyssItemArray);
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
                                    CalamityUtils.SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
                                }
                            }
                            else if (WorldGen.SolidTile(abyssIndex, abyssIndex2 + 1) &&
                                     abyssIndex2 < (int)Main.worldSurface)
                            {
                                if (WorldGen.genRand.Next(3) == 0)
                                {
                                    int style = WorldGen.genRand.Next(25, 28);
                                    WorldGen.PlacePot(abyssIndex, abyssIndex2, 28, style);
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
                        if (!Main.tile[abyssIndex, abyssIndex2].active())
                        {
                            if (WorldGen.SolidTile(abyssIndex, abyssIndex2 + 1) &&
                                abyssIndex2 > rockLayer)
                            {
                                if (WorldGen.genRand.Next(5) == 0)
                                {
                                    int style = WorldGen.genRand.Next(13, 16);
                                    WorldGen.PlacePot(abyssIndex, abyssIndex2, 28, style);
                                    CalamityUtils.SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
                                }
                            }
                            else if (WorldGen.SolidTile(abyssIndex, abyssIndex2 + 1) &&
                                     abyssIndex2 < (int)Main.worldSurface)
                            {
                                if (WorldGen.genRand.Next(3) == 0)
                                {
                                    int style = WorldGen.genRand.Next(25, 28);
                                    WorldGen.PlacePot(abyssIndex, abyssIndex2, 28, style);
                                    CalamityUtils.SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
