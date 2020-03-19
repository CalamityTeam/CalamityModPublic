using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.FurnitureVoid;
using CalamityMod.Walls;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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

            int yStart = DetermineYStart();
            // How wide the biome is.
            // Note: Change this to suit all world sizes once the biome layout code is complete.
            const int biomeWidth = 270;
            // How deep the biome goes.
            const int yDescent = 80;

            PlaceShallowSea(biomeWidth, yStart, yDescent);
            PlaceCavernBlock(biomeWidth, yStart);
            PlaceWaterCaverns(biomeWidth, yStart - 20);
            PlaceScenery(yStart, yDescent, biomeWidth);
            // Empty cavern pockets were becoming a problem.
            ReplaceAirWithWater(yStart, biomeWidth);
            RemoveWeirdSlopes(yStart, biomeWidth);

            // A copy of old sulf sea code that replaced all sand with sulph sea/abyss gravel. 
            // Made by Fabsol.
            ReplaceRemainingSand();
        }
        public static int DetermineYStart()
        {
            int xCheck = CalamityWorld.abyssSide ?
                WorldGen.genRand.Next(95, 120) :
                Main.maxTilesX - WorldGen.genRand.Next(95, 120);
            int yStart = 1;
            while (Main.tile[xCheck, yStart].liquid == 0)
            {
                yStart++;
                if (yStart > Main.worldSurface)
                    break;
            }
            return yStart;
        }
        public static void PlaceShallowSea(int biomeWidth, int yStart, int yDescent)
        {
            // Create a block
            if (CalamityWorld.abyssSide)
            {
                for (int x = 0; x < biomeWidth; x++)
                {
                    float xRatio = MathHelper.Clamp(x / (float)biomeWidth, 0f, 0.7f);
                    for (int y = yStart; y < yStart + yDescent; y++)
                    {
                        if (1f - ((y - yStart) / (float)yDescent) < xRatio)
                        {
                            Main.tile[x, y].type = (ushort)ModContent.TileType<SulphurousSand>();
                            Main.tile[x, y].slope(0);
                            Main.tile[x, y].active(true);
                        }
                        else
                        {
                            Main.tile[x, y] = new Tile()
                            {
                                liquid = 255
                            };
                        }
                    }
                }
            }
            else
            {
                int xCounter = 0;
                for (int x = Main.maxTilesX - 1; x >= Main.maxTilesX - biomeWidth; x--)
                {
                    float xRatio = MathHelper.Clamp(xCounter / (float)biomeWidth, 0f, 0.7f);
                    for (int y = yStart; y < yStart + yDescent; y++)
                    {
                        if (1f - ((y - yStart) / (float)yDescent) < xRatio)
                        {
                            Main.tile[x, y].type = (ushort)ModContent.TileType<SulphurousSand>();
                            Main.tile[x, y].slope(0);
                            Main.tile[x, y].active(true);
                        }
                        else
                        {
                            Main.tile[x, y] = new Tile()
                            {
                                liquid = 255
                            };
                        }
                    }
                    xCounter++;
                }
            }
        }
        public static void PlaceCavernBlock(int biomeWidth, int yStart)
        {
            if (CalamityWorld.abyssSide)
            {
                for (int x = 0; x < biomeWidth; x++)
                {
                    for (int y = yStart + 18; y < yStart + 150; y++)
                    {
                        if (Main.tile[x, y].liquid == 0)
                        {
                            Main.tile[x, y].type = (ushort)ModContent.TileType<SulphurousSand>();
                            Main.tile[x, y].slope(0);
                            Main.tile[x, y].active(true);
                        }
                    }
                }
            }
            else
            {
                for (int x = Main.maxTilesX - biomeWidth; x < Main.maxTilesX; x++)
                {
                    for (int y = yStart + 18; y < yStart + 150; y++)
                    {
                        if (Main.tile[x, y].liquid == 0)
                        {
                            Main.tile[x, y].type = (ushort)ModContent.TileType<SulphurousSand>();
                            Main.tile[x, y].slope(0);
                            Main.tile[x, y].active(true);
                        }
                    }
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
                float xDirection = WorldGen.genRand.NextFloat();
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
                    Vector2 tunnelVector = WorldGen.digTunnel(startingPosition.X, startingPosition.Y, xdirection2, ydirection2, WorldGen.genRand.Next(30, 50), WorldGen.genRand.Next(3, 6), false);

                    // The -2 parameter causes the tile-runner to generate liquids. Water above the lavaLine and lava below.
                    WorldGen.TileRunner((int)tunnelVector.X, (int)tunnelVector.Y, WorldGen.genRand.Next(20, 28), WorldGen.genRand.Next(15, 18 + 1), -2, false, 0f, 0f, false, true);
                }
            }
        }
        public static void PlaceScenery(int yStart, int yDescent, int biomeWidth)
        {
            if (CalamityWorld.abyssSide)
            {
                for (int x = 1; x < biomeWidth; x++)
                {
                    for (int y = yStart; y < yStart + yDescent; y++)
                    {
                        if (WorldGen.genRand.NextBool(6))
                        {
                            if (!WorldGen.SolidTile(Framing.GetTileSafely(x, y + 1)) ||
                                !WorldGen.SolidTile(Framing.GetTileSafely(x + 1, y + 1)) ||
                                WorldGen.SolidTile(Framing.GetTileSafely(x, y)) ||
                                WorldGen.SolidTile(Framing.GetTileSafely(x, y - 1)) ||
                                WorldGen.SolidTile(Framing.GetTileSafely(x + 1, y)) ||
                                WorldGen.SolidTile(Framing.GetTileSafely(x + 1, y - 1)))
                                break;
                            WorldGen.PlaceTile(x, y, ModContent.TileType<SteamGeyser>());
                        }
                    }
                }
            }
            else
            {
                for (int x = Main.maxTilesX - biomeWidth; x < Main.maxTilesX; x++)
                {
                    for (int y = yStart; y < yStart + yDescent; y++)
                    {
                        if (WorldGen.genRand.NextBool(13))
                        {
                            if (!WorldGen.SolidTile(Framing.GetTileSafely(x, y + 1)) ||
                                !WorldGen.SolidTile(Framing.GetTileSafely(x + 1, y + 1)) ||
                                WorldGen.SolidTile(Framing.GetTileSafely(x, y)) ||
                                WorldGen.SolidTile(Framing.GetTileSafely(x, y - 1)) ||
                                WorldGen.SolidTile(Framing.GetTileSafely(x + 1, y)) ||
                                WorldGen.SolidTile(Framing.GetTileSafely(x + 1, y - 1)))
                                break;
                            WorldGen.PlaceTile(x, y, ModContent.TileType<SteamGeyser>());
                        }
                    }
                }
            }

            // Rock pillars in the water
            for (int c = 1; c <= 2; c++)
            {
                int xStart;
                if (CalamityWorld.abyssSide)
                    xStart = (WorldGen.genRand.Next(-5, 5 + 1) + biomeWidth / 2 + (int)(biomeWidth / 2 / 3f * c * 0.7f) - 32);
                else
                    xStart = Main.maxTilesX - (WorldGen.genRand.Next(-5, 5 + 1) + biomeWidth / 2 + (int)(biomeWidth / 2 / 3f * c * 0.7f) - 32);
                int dy = -20;
                while (!Main.tile[xStart, yStart + dy].active() ||
                        Main.tile[xStart, yStart + dy].type != (ushort)ModContent.TileType<SulphurousSand>())
                {
                    dy++;
                }
                // Adjust to make sure we're a bit above water
                int ascent = WorldGen.genRand.Next(18, 25 + 1);
                switch (c)
                {
                    case 2:
                        ascent += 24;
                        break;
                    case 1:
                        ascent += 8;
                        break;
                }

                int widthTop = WorldGen.genRand.Next(3, 6 + 1);
                int widthBottomAdditive = WorldGen.genRand.Next(13, 20 + 1);
                float root = WorldGen.genRand.NextFloat(1.6f, 3.2f);
                for (int y = yStart - ascent; y <= yStart; y++)
                {
                    // The root is used to give a less linear scale
                    float yRatio = (float)Math.Pow(1f - (-(y - yStart) / (float)ascent), 1f / root);
                    int width = widthTop + (int)(widthBottomAdditive * yRatio);
                    for (int x = xStart - width / 2; x <= xStart + width / 2; x++)
                    {
                        Main.tile[x, y + dy].type = (ushort)ModContent.TileType<SulphurousSand>();
                        Main.tile[x, y + dy].slope(0);
                        Main.tile[x, y + dy].active(true);
                    }
                }
            }
        }
        public static void ReplaceAirWithWater(int yStart, int biomeWidth)
        {
            if (CalamityWorld.abyssSide)
            {
                for (int x = 0; x < biomeWidth; x++)
                {
                    for (int y = yStart + 10; y < yStart + 180; y++)
                    {
                        if (!Main.tile[x, y].active())
                        {
                            Main.tile[x, y] = new Tile()
                            {
                                liquid = 255
                            };
                        }
                    }
                }
            }
            else
            {
                for (int x = Main.maxTilesX - biomeWidth; x < Main.maxTilesX; x++)
                {
                    for (int y = yStart + 10; y < yStart + 180; y++)
                    {
                        if (!Main.tile[x, y].active())
                        {
                            Main.tile[x, y] = new Tile()
                            {
                                liquid = 255
                            };
                        }
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
