using CalamityMod.Items.Accessories;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Tools.ClimateChange;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Abyss.AbyssAmbient;
using CalamityMod.Tiles.FurnitureVoid;
using CalamityMod.Tiles.Ores;
using CalamityMod.Walls;
using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.GameContent.Generation;
using Terraria.DataStructures;
using Terraria.WorldBuilding;
using System.Diagnostics;

namespace CalamityMod.World
{
    public class Abyss
    {
        // TODO -- Abyss checks across the mod are littered with hardcoded estimate variables. At some point turn all of that into constants for easy reference and maintainability.
        public static int TotalPlacedIslandsSoFar = 0;
        public static Point[] AbyssIslandPositions = new Point[20];
        public static int[] AbyssItemArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        public static bool AtLeftSideOfWorld = false;
        public static int AbyssChasmBottom = 0;

        /// <summary>
        /// Forces abyss chests to unlock even if <see cref="NPC.downedBoss3"/> is false. Used in <see cref="UnlockAllAbyssChests"/> as it is ran during Skeletron's death, not after.
        /// </summary>
        public static bool UnlockChests { get; set; }

        public static void PlaceAbyss()
        {
            int x = Main.maxTilesX;
            int y = Main.maxTilesY;
            int genLimit = x / 2;
            int rockLayer = Main.remixWorld ? SulphurousSea.YStart : (int)Main.rockLayer;

            int abyssChasmY = Main.remixWorld ? SulphurousSea.YStart : y - 250; //Underworld = y - 200
            AbyssChasmBottom = abyssChasmY - 100; //850 small 1450 medium 2050 large
            int abyssChasmX = AtLeftSideOfWorld ? genLimit - (genLimit - 135) + 35 : genLimit + (genLimit - 135) - 35; //2100 - 1965 = 135 : 2100 + 1965 = 4065

            int abyssMinX = AtLeftSideOfWorld ? 0 : abyssChasmX - 160;
            int abyssMaxX = AtLeftSideOfWorld ? abyssChasmX + 160 : x;

            for (int abyssIndex = abyssMinX; abyssIndex < abyssMaxX; abyssIndex++)
            {
                for (int abyssIndex2 = 0; abyssIndex2 < abyssChasmY + 50; abyssIndex2++)
                {
                    Tile tile = Framing.GetTileSafely(abyssIndex, abyssIndex2);

                    if (tile.LiquidType == LiquidID.Lava && tile.LiquidAmount > 0)
                    {
                        tile.Get<LiquidData>().LiquidType = LiquidID.Water;
                        tile.LiquidAmount = byte.MaxValue;
                    }

                    bool canConvert = tile.HasTile && tile.TileType < TileID.Count && tile.TileType != ModContent.TileType<SulphurousSandstone>();

                    //normally i would organize each layer of blocks by the order of how they are placed in the abyss
                    //but i cannot be bothered, and when i do it, it keeps messing up or making certain parts like transitions not gen right
                    //i have at least left comments so people reading will know what does what
                    if (Main.remixWorld)
                    {
                        if (abyssIndex2 <= rockLayer)
                        {
                            //replaces blocks wand walls that can be converted
                            if (canConvert)
                            {
                                //layer 4
                                if (abyssIndex2 <= rockLayer - (int)((y - 200) * 0.6f))
                                {
                                    tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                    tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                }
                                //layer 3-4 dithering transition
                                else if (abyssIndex2 <= rockLayer - (int)((y - 200) * 0.59f) && WorldGen.genRand.NextBool(2))
                                {
                                    tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                    tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                }
                                //layer 3
                                else if (abyssIndex2 <= rockLayer - (int)((y - 200) * 0.4f))
                                {
                                    tile.TileType = (ushort)ModContent.TileType<PyreMantle>();
                                    tile.WallType = (ushort)ModContent.WallType<PyreMantleWall>();
                                }
                                //layer 2-3 dithering transition
                                else if (abyssIndex2 <= rockLayer - (int)((y - 200) * 0.39f) && WorldGen.genRand.NextBool(2))
                                {
                                    tile.TileType = (ushort)ModContent.TileType<PyreMantle>();
                                    tile.WallType = (ushort)ModContent.WallType<PyreMantleWall>();
                                }
                                //layer 2
                                else if (abyssIndex2 <= rockLayer - (int)((y - 200) * 0.2f))
                                {
                                    tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                    tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                }
                                //layer 1-2 dithering transition
                                else if (abyssIndex2 <= rockLayer - (int)((y - 200) * 0.19f))
                                {
                                    if (WorldGen.genRand.NextBool(2))
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                        tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                    }
                                    else
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<SulphurousShale>();
                                        tile.WallType = (ushort)ModContent.WallType<SulphurousShaleWall>();
                                    }
                                }
                                //layer 1
                                else
                                {
                                    tile.TileType = (ushort)ModContent.TileType<SulphurousShale>();
                                    tile.WallType = (ushort)ModContent.WallType<SulphurousShaleWall>();
                                }
                            }
                            //basically places smaller clusters everywhere else?
                            else if (!tile.HasTile)
                            {
                                tile.Get<TileWallWireStateData>().HasTile = true;

                                //layer 4
                                if (abyssIndex2 <= rockLayer - (int)((y - 200) * 0.6f))
                                {
                                    tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                    tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                }
                                //layer 3-4 dithering transition
                                else if (abyssIndex2 <= rockLayer - (int)((y - 200) * 0.59f) && WorldGen.genRand.NextBool(2))
                                {
                                    tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                    tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                }
                                //layer 3
                                else if (abyssIndex2 <= rockLayer - (int)((y - 200) * 0.4f))
                                {
                                    tile.TileType = (ushort)ModContent.TileType<PyreMantle>();
                                    tile.WallType = (ushort)ModContent.WallType<PyreMantleWall>();
                                }
                                //layer 2-3 dithering transition
                                else if (abyssIndex2 <= rockLayer - (int)((y - 200) * 0.39f) && WorldGen.genRand.NextBool(2))
                                {
                                    tile.TileType = (ushort)ModContent.TileType<PyreMantle>();
                                    tile.WallType = (ushort)ModContent.WallType<PyreMantleWall>();
                                }
                                //layer 2
                                else if (abyssIndex2 <= rockLayer - (int)((y - 200) * 0.2f))
                                {
                                    tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                    tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                }
                                //layer 1-2 dithering transition
                                else if (abyssIndex2 <= rockLayer - (int)((y - 200) * 0.19f))
                                {
                                    if (WorldGen.genRand.NextBool(2))
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                        tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                    }
                                    else
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<SulphurousShale>();
                                        tile.WallType = (ushort)ModContent.WallType<SulphurousShaleWall>();
                                    }
                                }
                                //layer 1
                                else
                                {
                                    tile.TileType = (ushort)ModContent.TileType<SulphurousShale>();
                                    tile.WallType = (ushort)ModContent.WallType<SulphurousShaleWall>();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (abyssIndex2 > (rockLayer - Main.maxTilesY / 15) + 35)
                        {
                            //replaces blocks wand walls that can be converted
                            if (canConvert)
                            {
                                //layer 4
                                if (abyssIndex2 > (rockLayer + y * 0.270))
                                {
                                    tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                    tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                }
                                //layer 3-4 dithering transition
                                else if (abyssIndex2 > (rockLayer + y * 0.268) && WorldGen.genRand.NextBool(2))
                                {
                                    tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                    tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                }
                                //layer 3
                                else if (abyssIndex2 > (rockLayer + y * 0.145))
                                {
                                    tile.TileType = (ushort)ModContent.TileType<PyreMantle>();
                                    tile.WallType = (ushort)ModContent.WallType<PyreMantleWall>();
                                }
                                //layer 2-3 dithering transition
                                else if (abyssIndex2 > (rockLayer + y * 0.143) && WorldGen.genRand.NextBool(2))
                                {
                                    tile.TileType = (ushort)ModContent.TileType<PyreMantle>();
                                    tile.WallType = (ushort)ModContent.WallType<PyreMantleWall>();
                                }
                                //layer 1-2 dithering transition
                                else if (abyssIndex2 >= rockLayer - 10 && abyssIndex2 <= rockLayer)
                                {
                                    if (WorldGen.genRand.NextBool(2))
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                        tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                    }
                                    else
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<SulphurousShale>();
                                        tile.WallType = (ushort)ModContent.WallType<SulphurousShaleWall>();
                                    }
                                }
                                //layer 1
                                else if (abyssIndex2 <= rockLayer - 10)
                                {
                                    tile.TileType = (ushort)ModContent.TileType<SulphurousShale>();
                                    tile.WallType = (ushort)ModContent.WallType<SulphurousShaleWall>();
                                }
                                //layer 2 (default block for the abyss)
                                else
                                {
                                    tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                    tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                }
                            }
                            //basically places smaller clusters everywhere else?
                            else if (!tile.HasTile)
                            {
                                tile.Get<TileWallWireStateData>().HasTile = true;

                                //layer 4
                                if (abyssIndex2 > (rockLayer + y * 0.270))
                                {
                                    tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                    tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                }
                                //layer 3-4 dithering transition
                                else if (abyssIndex2 > (rockLayer + y * 0.268) && WorldGen.genRand.NextBool(2))
                                {
                                    tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                    tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                }
                                //layer 3
                                else if (abyssIndex2 > (rockLayer + y * 0.145))
                                {
                                    tile.TileType = (ushort)ModContent.TileType<PyreMantle>();
                                    tile.WallType = (ushort)ModContent.WallType<PyreMantleWall>();
                                }
                                //layer 2-3 dithering transition
                                else if (abyssIndex2 > (rockLayer + y * 0.143) && WorldGen.genRand.NextBool(2))
                                {
                                    tile.TileType = (ushort)ModContent.TileType<PyreMantle>();
                                    tile.WallType = (ushort)ModContent.WallType<PyreMantleWall>();
                                }
                                //layer 1-2 dithering transition
                                else if (abyssIndex2 >= rockLayer - 10 && abyssIndex2 <= rockLayer)
                                {
                                    if (WorldGen.genRand.NextBool(2))
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                        tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                    }
                                    else
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<SulphurousShale>();
                                        tile.WallType = (ushort)ModContent.WallType<SulphurousShaleWall>();
                                    }
                                }
                                //layer 1
                                else if (abyssIndex2 <= rockLayer - 10)
                                {
                                    tile.TileType = (ushort)ModContent.TileType<SulphurousShale>();
                                    tile.WallType = (ushort)ModContent.WallType<SulphurousShaleWall>();
                                }
                                //layer 2 (default block for the abyss)
                                else
                                {
                                    tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                    tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                }
                            }
                        }
                    }
                }
            }

            //initial hole
            MiscWorldgenRoutines.ChasmGenerator(abyssChasmX, Main.remixWorld ? 100 : (int)GenVars.worldSurfaceLow + 65, Main.remixWorld ? AbyssChasmBottom + 110 : AbyssChasmBottom, true);
            MiscWorldgenRoutines.ChasmGenerator(abyssChasmX - 22, Main.remixWorld ? 35 : (int)GenVars.worldSurfaceLow, AbyssChasmBottom, true);
            MiscWorldgenRoutines.ChasmGenerator(abyssChasmX + 22, Main.remixWorld ? 35 : (int)GenVars.worldSurfaceLow, AbyssChasmBottom, true);

            int maxAbyssIslands = 11; //Small World
            if (y > 2100)
                maxAbyssIslands = 20; //Large World
            else if (y > 1500)
                maxAbyssIslands = 16; //Medium World

            PlaceSnailFossil(abyssChasmX, Main.remixWorld ? 145 : AbyssChasmBottom + 45);

            //place a single abyss island under the terminus shrine
            AbyssIsland(abyssChasmX, Main.remixWorld ? 105 : AbyssChasmBottom + 5, 65, 75, 40, 45, ModContent.TileType<Voidstone>(), false, false, false);

            //place terminus shrine
            UndergroundShrines.SpecialHut((ushort)ModContent.TileType<SmoothVoidstone>(), (ushort)ModContent.TileType<Voidstone>(),
            (ushort)ModContent.WallType<VoidstoneWallUnsafe>(), UndergroundShrines.UndergroundShrineType.Abyss, abyssChasmX, Main.remixWorld ? 100 : AbyssChasmBottom);

            //place islands in the sulphuric depths layer
            //start placing islands at the world surface so the abyss hole isnt completely empty
            int abyssStartHeight = Main.remixWorld ? rockLayer - (int)((y - 200) * 0.2f) : (SulphurousSea.YStart + (int)Main.worldSurface) / 2 + 90;
            for (int sulphurIslandY = abyssStartHeight; sulphurIslandY <= rockLayer - 25; sulphurIslandY++)
            {
                int islandLocationX = abyssChasmX;
                int islandLocationOffset = WorldGen.genRand.Next(15, 22);
                int randomPositon = WorldGen.genRand.Next(35, 80);
                int randomIsland = WorldGen.genRand.Next(5);
                switch (randomIsland)
                {
                    case 0:
                        //3 islands, middle one with chest
                        AbyssIsland(islandLocationX - randomPositon - 10, sulphurIslandY + 15, 60, 65, 45, 55, ModContent.TileType<SulphurousShale>(), false, false, false);
                        AbyssIsland(islandLocationX, sulphurIslandY, 60, 65, 45, 55, ModContent.TileType<SulphurousShale>(), false, false, false);
                        AbyssIsland(islandLocationX + randomPositon + 10, sulphurIslandY + 15, 60, 65, 45, 55, ModContent.TileType<SulphurousShale>(), false, false, false);
                        break;
                    case 1:
                        //3 islands, left, middle one with chest, and right
                        AbyssIsland(islandLocationX - randomPositon, sulphurIslandY + 10, 60, 85, 30, 35, ModContent.TileType<SulphurousShale>(), false, false, false);
                        AbyssIsland(islandLocationX, sulphurIslandY + 15, 60, 85, 30, 35, ModContent.TileType<SulphurousShale>(), false, false, false);
                        AbyssIsland(islandLocationX + randomPositon, sulphurIslandY, 60, 85, 30, 35, ModContent.TileType<SulphurousShale>(), false, false, false);
                        islandLocationX += 15;
                        break;
                    case 2:
                        //2 islands, left, and slightly right
                        AbyssIsland(islandLocationX - randomPositon, sulphurIslandY + 15, 55, 65, 30, 35, ModContent.TileType<SulphurousShale>(), false, false, false);
                        AbyssIsland(islandLocationX + WorldGen.genRand.Next(15, 30), sulphurIslandY + 15, 60, 85, 30, 35, ModContent.TileType<SulphurousShale>(), false, false, false);
                        islandLocationX -= 15;
                        break;
                    case 3:
                        //2 islands, slightly left, and right
                        AbyssIsland(islandLocationX - WorldGen.genRand.Next(15, 30), sulphurIslandY + 10, 55, 65, 30, 35, ModContent.TileType<SulphurousShale>(), false, false, false);
                        AbyssIsland(islandLocationX + randomPositon, sulphurIslandY + 15, 60, 85, 30, 35, ModContent.TileType<SulphurousShale>(), false, false, false);
                        islandLocationX += 25;
                        break;
                    case 4:
                        //2 islands, left and right
                        AbyssIsland(islandLocationX - randomPositon, sulphurIslandY, 60, 75, 30, 40, ModContent.TileType<SulphurousShale>(), false, false, false);
                        AbyssIsland(islandLocationX + randomPositon, sulphurIslandY + 5, 60, 75, 30, 40, ModContent.TileType<SulphurousShale>(), false, false, false);
                        islandLocationX -= 25;
                        break;
                }

                //increase y value so islands cant be spammed on top of each other
                sulphurIslandY += islandLocationOffset;
            }

            //islands for layer 2
            int islandLocationY = Main.remixWorld ? rockLayer - (int)((y - 200) * 0.4f) : rockLayer;

            for (int islands = 0; islands < maxAbyssIslands; islands++)
            {
                int islandLocationX = abyssChasmX;
                int islandLocationOffset = WorldGen.genRand.Next(18, 25);
                int randomPositon = WorldGen.genRand.Next(45, 80);
                AbyssIslandPositions[TotalPlacedIslandsSoFar].Y = islandLocationY;

                int randomIsland = WorldGen.genRand.Next(5);
                switch (randomIsland)
                {
                    case 0:
                        //3 islands, middle one with chest
                        AbyssIsland(islandLocationX - randomPositon - 10, islandLocationY + 15, 60, 65, 30, 35, ModContent.TileType<AbyssGravel>(), false, true, false);
                        AbyssIsland(islandLocationX, islandLocationY, 60, 65, 30, 35, ModContent.TileType<AbyssGravel>(), true, true, true);
                        AbyssIsland(islandLocationX + randomPositon + 10, islandLocationY + 15, 60, 65, 30, 35, ModContent.TileType<AbyssGravel>(), false, true, false);
                        break;
                    case 1:
                        //3 islands, left, middle one with chest, and right
                        AbyssIsland(islandLocationX - randomPositon, islandLocationY + 10, 60, 65, 30, 35, ModContent.TileType<AbyssGravel>(), false, true, true);
                        AbyssIsland(islandLocationX, islandLocationY + 15, 60, 65, 30, 35, ModContent.TileType<AbyssGravel>(), true, true, false);
                        AbyssIsland(islandLocationX + randomPositon, islandLocationY, 60, 65, 30, 35, ModContent.TileType<AbyssGravel>(), false, true, false);
                        islandLocationX += 30;
                        break;
                    case 2:
                        //3 islands, none with chest
                        AbyssIsland(islandLocationX - randomPositon - 20, islandLocationY, 55, 65, 30, 35, ModContent.TileType<AbyssGravel>(), false, true, true);
                        AbyssIsland(islandLocationX + 15, islandLocationY + 15, 55, 65, 30, 35, ModContent.TileType<AbyssGravel>(), false, true, false);
                        AbyssIsland(islandLocationX + randomPositon + 20, islandLocationY + 10, 55, 65, 30, 35, ModContent.TileType<AbyssGravel>(), false, true, false);
                        islandLocationX -= 30;
                        break;
                    case 3:
                        //3 islands, left, middle, and right one with chest
                        AbyssIsland(islandLocationX - randomPositon, islandLocationY + 15, 60, 65, 30, 45, ModContent.TileType<AbyssGravel>(), false, true, false);
                        AbyssIsland(islandLocationX - 15, islandLocationY + 5, 60, 65, 30, 45, ModContent.TileType<AbyssGravel>(), false, true, false);
                        AbyssIsland(islandLocationX + randomPositon, islandLocationY, 60, 65, 30, 45, ModContent.TileType<AbyssGravel>(), true, true, true);
                        islandLocationX += 25;
                        break;
                    case 4:
                        //3 islands, left one with chest, middle, and right
                        AbyssIsland(islandLocationX - randomPositon, islandLocationY, 60, 75, 30, 40, ModContent.TileType<AbyssGravel>(), true, true, false);
                        AbyssIsland(islandLocationX, islandLocationY + 15, 60, 75, 30, 40, ModContent.TileType<AbyssGravel>(), false, true, false);
                        AbyssIsland(islandLocationX + randomPositon, islandLocationY + 5, 60, 75, 30, 40, ModContent.TileType<AbyssGravel>(), false, true, false);
                        islandLocationX -= 25;
                        break;
                }

                AbyssIslandPositions[TotalPlacedIslandsSoFar].X = islandLocationX;
                TotalPlacedIslandsSoFar++;

                islandLocationY += islandLocationOffset;

                //do not place anymore islands when the third layer is reached
                if (islandLocationY >= (Main.remixWorld ? rockLayer - (int)((y - 200) * 0.2f) : (rockLayer + y * 0.145) - 10))
                {
                    break;
                }
            }

            //islands for layer 3
            for (int thermalIslandY = Main.remixWorld ? rockLayer - (int)((y - 200) * 0.6f) : (int)(rockLayer + y * 0.145); thermalIslandY <= (Main.remixWorld ? rockLayer - (int)((y - 200) * 0.4f) : (int)(rockLayer + y * 0.270)); thermalIslandY++)
            {
                int islandLocationX = abyssChasmX;
                int islandLocationOffset = WorldGen.genRand.Next(18, 30);
                int randomPositon = WorldGen.genRand.Next(40, 75);
                bool hasScoria = WorldGen.genRand.NextBool(2); //50% chance to place scoria in each island instead of planty mush

                int randomIsland = WorldGen.genRand.Next(4);
                switch (randomIsland)
                {
                    case 0:
                        //3 islands, left, middle, and right
                        AbyssIsland(islandLocationX - randomPositon - 10, thermalIslandY + 15, 55, 65, 45, 55, ModContent.TileType<PyreMantle>(), false, true, false);
                        AbyssIsland(islandLocationX, thermalIslandY, 60, 65, 45, 55, ModContent.TileType<PyreMantle>(), false, true, true);
                        AbyssIsland(islandLocationX + randomPositon + 10, thermalIslandY + 15, 60, 65, 45, 55, ModContent.TileType<PyreMantle>(), false, true, true);
                        islandLocationX -= 30;
                        break;
                    case 1:
                        //3 islands, left, middle (slightly lower), and right
                        AbyssIsland(islandLocationX - randomPositon, thermalIslandY + 10, 60, 75, 30, 35, ModContent.TileType<PyreMantle>(), false, true, true);
                        AbyssIsland(islandLocationX, thermalIslandY + 15, 75, 85, 30, 35, ModContent.TileType<PyreMantle>(), false, true, false);
                        AbyssIsland(islandLocationX + randomPositon, thermalIslandY, 55, 85, 30, 35, ModContent.TileType<PyreMantle>(), false, true, false);
                        islandLocationX += 30;
                        break;
                    case 2:
                        //3 islands, left, middle (slightly left), and right
                        AbyssIsland(islandLocationX - randomPositon - 20, thermalIslandY, 55, 65, 30, 35, ModContent.TileType<PyreMantle>(), false, true, true);
                        AbyssIsland(islandLocationX - 20, thermalIslandY + 15, 60, 70, 30, 35, ModContent.TileType<PyreMantle>(), false, true, false);
                        AbyssIsland(islandLocationX + randomPositon + 20, thermalIslandY + 10, 65, 70, 30, 35, ModContent.TileType<PyreMantle>(), false, true, true);
                        islandLocationX -= 25;
                        break;
                    case 3:
                        //3 islands, left, middle (slightly right), and right
                        AbyssIsland(islandLocationX - randomPositon, thermalIslandY + 15, 60, 75, 30, 55, ModContent.TileType<PyreMantle>(), false, true, true);
                        AbyssIsland(islandLocationX + 20, thermalIslandY + 5, 60, 75, 30, 55, ModContent.TileType<PyreMantle>(), false, true, false);
                        AbyssIsland(islandLocationX + randomPositon, thermalIslandY, 60, 75, 30, 55, ModContent.TileType<PyreMantle>(), false, true, true);
                        islandLocationX += 25;
                        break;
                }

                thermalIslandY += islandLocationOffset;

                //do not place anymore islands when the fourth layer is reached
                if (thermalIslandY >= (Main.remixWorld ? rockLayer - (int)((y - 200) * 0.4f) : (rockLayer + y * 0.270) - 10))
                {
                    break;
                }
            }

            //place some islands lining the walls in layer 4
            for (int voidIslandY = Main.remixWorld ? rockLayer - (int)((y - 200) * 0.8f) : (int)(rockLayer + y * 0.275); voidIslandY <= (Main.remixWorld ? rockLayer - (int)((y - 200) * 0.6f) : AbyssChasmBottom - 20); voidIslandY++)
            {
                if (WorldGen.genRand.NextBool(8))
                {
                    int randomIsland = WorldGen.genRand.Next(3);
                    switch (randomIsland)
                    {
                        case 0:
                            //1 island, to the left
                            AbyssIsland(abyssChasmX - WorldGen.genRand.Next(70, 78), voidIslandY, 65, 85, 35, 45, ModContent.TileType<Voidstone>(), false, false, false);
                            break;
                        case 1:
                            //1 island, to the right
                            AbyssIsland(abyssChasmX + WorldGen.genRand.Next(70, 78), voidIslandY, 65, 85, 35, 45, ModContent.TileType<Voidstone>(), false, false, false);
                            break;
                        case 2:
                            //2 islands, left and right
                            AbyssIsland(abyssChasmX - WorldGen.genRand.Next(70, 78), voidIslandY, 65, 85, 35, 45, ModContent.TileType<Voidstone>(), false, false, false);
                            AbyssIsland(abyssChasmX + WorldGen.genRand.Next(70, 78), voidIslandY, 65, 85, 35, 45, ModContent.TileType<Voidstone>(), false, false, false);
                            break;
                    }

                    //increase y value so islands cant be spammed on top of each other
                    voidIslandY += WorldGen.genRand.Next(20, 32);
                }
            }

            //place chests in each location saved from before
            AbyssItemArray = CalamityUtils.ShuffleArray(AbyssItemArray);
            for (int abyssHouse = 0; abyssHouse < TotalPlacedIslandsSoFar; abyssHouse++) //11 15 19
            {
                bool isInLayer2 = AbyssIslandPositions[abyssHouse].Y < (rockLayer + y * 0.145);

                if (abyssHouse != 20)
                {
                    AbyssChest(AbyssIslandPositions[abyssHouse].X, AbyssIslandPositions[abyssHouse].Y, AbyssItemArray[abyssHouse > 9 ? (abyssHouse - 10) : abyssHouse]);
                }
            }

            //clean up
            for (int abyssIndex = abyssMinX + 5; abyssIndex < abyssMaxX - 5; abyssIndex++)
            {
                for (int abyssIndex2 = 5; abyssIndex2 < abyssChasmY; abyssIndex2++)
                {
                    Tile tile = Main.tile[abyssIndex, abyssIndex2];
                    Tile tileUp = Main.tile[abyssIndex, abyssIndex2 - 1];
                    Tile tileDown = Main.tile[abyssIndex, abyssIndex2 + 1];
                    Tile tileLeft = Main.tile[abyssIndex - 1, abyssIndex2];
                    Tile tileRight = Main.tile[abyssIndex + 1, abyssIndex2];

                    if (tile.TileType == ModContent.TileType<AbyssGravel>() || tile.TileType == ModContent.TileType<PyreMantle>() || 
                    tile.TileType == ModContent.TileType<Voidstone>() || tile.TileType == ModContent.TileType<PlantyMush>() || 
                    tile.TileType == ModContent.TileType<ScoriaOre>() || tile.TileType == ModContent.TileType<SulphurousShale>())
                    {
                        //slope tiles
                        Tile.SmoothSlope(abyssIndex, abyssIndex2, true);

                        //kill any individual floating tiles
                        if (!tileUp.HasTile && !tileDown.HasTile && !tileLeft.HasTile && !tileRight.HasTile)
                        {
                            WorldGen.KillTile(abyssIndex, abyssIndex2);
                        }
                    }

                    if (!tile.HasTile && tile.WallType > 0)
                    {
                        //fill up any air pockets with water
                        tile.Get<LiquidData>().LiquidType = LiquidID.Water;
                        tile.LiquidAmount = byte.MaxValue;
                    }

                    //kill obsidian
                    if (tile.TileType == TileID.Obsidian)
                    {
                        WorldGen.KillTile(abyssIndex, abyssIndex2);
                    }
                }
            }

            //ambient tiles and pots
            for (int abyssIndex = abyssMinX + 5; abyssIndex < abyssMaxX - 5; abyssIndex++)
            {
                for (int abyssIndex2 = 0; abyssIndex2 < (Main.remixWorld ? rockLayer : Main.maxTilesY - 200); abyssIndex2++)
                {
                    Tile tileToGrowVineOn = Main.tile[abyssIndex, abyssIndex2];

                    if (!Main.tile[abyssIndex, abyssIndex2].HasTile)
                    {
                        Tile tile = Main.tile[abyssIndex, abyssIndex2 + 1];

                        //above the 4th layer
                        if (abyssIndex2 < (Main.remixWorld ? rockLayer : Main.maxTilesY - 200) && WorldGen.SolidTile(abyssIndex, abyssIndex2 + 1))
                        {
                            //sulphur shale stuff 
                            if (tile.TileType == ModContent.TileType<SulphurousShale>())
                            {
                                //tube coral
                                if (WorldGen.genRand.NextBool(85))
                                {
                                    WorldGen.PlaceObject(abyssIndex, abyssIndex2, (ushort)ModContent.TileType<SulphurTubeCoral>());
                                }

                                //shale rock piles
                                if (WorldGen.genRand.NextBool(18))
                                {
                                    ushort[] ShalePiles = new ushort[] { (ushort)ModContent.TileType<ShalePile1>(),
                                    (ushort)ModContent.TileType<ShalePile2>(), (ushort)ModContent.TileType<ShalePile3>() };

                                    WorldGen.PlaceObject(abyssIndex, abyssIndex2, WorldGen.genRand.Next(ShalePiles));
                                }

                                //pire corals
                                if (WorldGen.genRand.NextBool(15))
                                {
                                    ushort[] PireCorals = new ushort[] { (ushort)ModContent.TileType<SulphurPireCoral1>(),
                                    (ushort)ModContent.TileType<SulphurPireCoral2>(), (ushort)ModContent.TileType<SulphurPireCoral3>() };

                                    WorldGen.PlaceObject(abyssIndex, abyssIndex2, WorldGen.genRand.Next(PireCorals));
                                }

                                //sulphur fossils
                                if (WorldGen.genRand.NextBool(12))
                                {
                                    ushort[] SulphuricFossils = new ushort[] { (ushort)ModContent.TileType<SulphuricFossil1>(),
                                    (ushort)ModContent.TileType<SulphuricFossil2>(), (ushort)ModContent.TileType<SulphuricFossil3>() };

                                    WorldGen.PlaceObject(abyssIndex, abyssIndex2, WorldGen.genRand.Next(SulphuricFossils));
                                }

                                //ribs
                                if (WorldGen.genRand.NextBool(12))
                                {
                                    ushort[] Ribs = new ushort[] { (ushort)ModContent.TileType<SulphurousRib1>(),
                                    (ushort)ModContent.TileType<SulphurousRib2>(), (ushort)ModContent.TileType<SulphurousRib3>(), 
                                    (ushort)ModContent.TileType<SulphurousRib4>(), (ushort)ModContent.TileType<SulphurousRib5>() };

                                    WorldGen.PlaceObject(abyssIndex, abyssIndex2, WorldGen.genRand.Next(Ribs));
                                }
                            }

                            //planty mush stuff
                            if (tile.TileType == ModContent.TileType<PlantyMush>())
                            {
                                if (WorldGen.genRand.NextBool(8))
                                {
                                    ushort[] PlantPiles = new ushort[] { (ushort)ModContent.TileType<PlantyMushPile1>(),
                                    (ushort)ModContent.TileType<PlantyMushPile2>(), (ushort)ModContent.TileType<PlantyMushPile3>() };

                                    WorldGen.PlaceObject(abyssIndex, abyssIndex2, WorldGen.genRand.Next(PlantPiles));
                                }
                            }

                            //abyss gravel stuff
                            if (tile.TileType == ModContent.TileType<AbyssGravel>())
                            {
                                //rare pearls
                                if (WorldGen.genRand.NextBool(50))
                                {
                                    WorldGen.PlaceObject(abyssIndex, abyssIndex2, (ushort)ModContent.TileType<MassiveRarePearl>());
                                }

                                //giant kelp on abyss gravel
                                if (WorldGen.genRand.NextBool(15))
                                {
                                    ushort[] Kelps = new ushort[] { (ushort)ModContent.TileType<AbyssGiantKelp1>(), (ushort)ModContent.TileType<AbyssGiantKelp2>(),
                                    (ushort)ModContent.TileType<AbyssGiantKelp3>(), (ushort)ModContent.TileType<AbyssGiantKelp4>() };

                                    WorldGen.PlaceObject(abyssIndex, abyssIndex2, WorldGen.genRand.Next(Kelps));
                                }

                                //plany mush piles
                                if (WorldGen.genRand.NextBool(15))
                                {
                                    ushort[] PlantPiles = new ushort[] { (ushort)ModContent.TileType<PlantyMushPile1>(),
                                    (ushort)ModContent.TileType<PlantyMushPile2>(), (ushort)ModContent.TileType<PlantyMushPile3>() };

                                    WorldGen.PlaceObject(abyssIndex, abyssIndex2, WorldGen.genRand.Next(PlantPiles));
                                }

                                //gravel rock piles
                                if (WorldGen.genRand.NextBool(15))
                                {
                                    ushort[] GravelPiles = new ushort[] { (ushort)ModContent.TileType<GravelPile1>(),
                                    (ushort)ModContent.TileType<GravelPile2>(), (ushort)ModContent.TileType<GravelPile3>() };

                                    WorldGen.PlaceObject(abyssIndex, abyssIndex2, WorldGen.genRand.Next(GravelPiles));
                                }

                                //abyss gravel vents
                                if (WorldGen.genRand.NextBool(45))
                                {
                                    ushort[] Vents = new ushort[] { (ushort)ModContent.TileType<AbyssVent1>(),
                                    (ushort)ModContent.TileType<AbyssVent2>(), (ushort)ModContent.TileType<AbyssVent3>() };

                                    WorldGen.PlaceObject(abyssIndex, abyssIndex2, WorldGen.genRand.Next(Vents));
                                }

                                //pirate crates
                                if (WorldGen.genRand.NextBool(17))
                                {
                                    ushort[] PirateCrate = new ushort[] { (ushort)ModContent.TileType<PirateCrate1>(),
                                    (ushort)ModContent.TileType<PirateCrate2>(), (ushort)ModContent.TileType<PirateCrate3>(), (ushort)ModContent.TileType<PirateCrate4>(), (ushort)ModContent.TileType<PirateCrate5>(), (ushort)ModContent.TileType<PirateCrate6>() };

                                    WorldGen.PlaceObject(abyssIndex, abyssIndex2, WorldGen.genRand.Next(PirateCrate));
                                }
                            }

                            //prye mantle stuff
                            if (tile.TileType == ModContent.TileType<PyreMantle>())
                            {
                                //spider coral
                                if (WorldGen.genRand.NextBool(12))
                                {
                                    ushort[] SpiderCorals = new ushort[] { (ushort)ModContent.TileType<SpiderCoral1>(),
                                    (ushort)ModContent.TileType<SpiderCoral2>(), (ushort)ModContent.TileType<SpiderCoral3>(),
                                    (ushort)ModContent.TileType<SpiderCoral4>(), (ushort)ModContent.TileType<SpiderCoral5>() };

                                    WorldGen.PlaceObject(abyssIndex, abyssIndex2, WorldGen.genRand.Next(SpiderCorals));
                                }

                                //thermal vent
                                if (WorldGen.genRand.NextBool(15))
                                {
                                    ushort[] Vents = new ushort[] { (ushort)ModContent.TileType<ThermalVent1>(),
                                    (ushort)ModContent.TileType<ThermalVent2>(), (ushort)ModContent.TileType<ThermalVent3>() };

                                    WorldGen.PlaceObject(abyssIndex, abyssIndex2, WorldGen.genRand.Next(Vents));
                                }
                            }

                            //voidstone stuff
                            if (tile.TileType == ModContent.TileType<Voidstone>())
                            {
                                //bulb trees
                                if (WorldGen.genRand.NextBool(25))
                                {
                                    ushort[] BulbTrees = new ushort[] { (ushort)ModContent.TileType<BulbTree1>(),
                                    (ushort)ModContent.TileType<BulbTree2>(), (ushort)ModContent.TileType<BulbTree3>() };

                                    WorldGen.PlaceObject(abyssIndex, abyssIndex2, WorldGen.genRand.Next(BulbTrees));
                                }
                            }
                        }

                        //pots
                        if ((tile.TileType == ModContent.TileType<AbyssGravel>() || tile.TileType == ModContent.TileType<PyreMantle>() || 
                        tile.TileType == ModContent.TileType<Voidstone>()) && abyssIndex2 > (Main.remixWorld ? rockLayer - (int)((y - 200) * 0.8f) : rockLayer))
                        {
                            if (WorldGen.genRand.NextBool(5))
                            {
                                WorldGen.PlacePot(abyssIndex, abyssIndex2, (ushort)ModContent.TileType<AbyssalPots>());
                                CalamityUtils.SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
                            }
                        }

                        //sulphur pots
                        else if (tile.TileType == ModContent.TileType<SulphurousShale>() && abyssIndex2 < (Main.remixWorld ? Main.maxTilesY - 200 : (int)Main.worldSurface))
                        {
                            if (WorldGen.genRand.NextBool(3))
                            {
                                WorldGen.PlacePot(abyssIndex, abyssIndex2, (ushort)ModContent.TileType<SulphurousPots>());
                                CalamityUtils.SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
                            }
                        }
                    }

                    //esentially what this does is grow one vine on the bottom of a tile, then use the util to keep placing the vine until it decides to stop
                    if (tileToGrowVineOn.TileType == ModContent.TileType<PlantyMush>() && !Main.tile[abyssIndex, abyssIndex2 + 1].HasTile)
                    {
                        if (WorldGen.genRand.Next(2) == 0)
                        {
                            WorldGen.PlaceTile(abyssIndex, abyssIndex2 + 1, (ushort)ModContent.TileType<ViperVines>());
                        }
                    }
                    if (tileToGrowVineOn.TileType == ModContent.TileType<ViperVines>())
                    {
                        CalamityUtils.GrowVines(abyssIndex, abyssIndex2, WorldGen.genRand.Next(1, 4), (ushort)ModContent.TileType<ViperVines>());
                    }

                    //same as above but for sulphur vines
                    if (tileToGrowVineOn.TileType == ModContent.TileType<SulphurousShale>() && !Main.tile[abyssIndex, abyssIndex2 + 1].HasTile)
                    {
                        if (WorldGen.genRand.Next(5) == 0)
                        {
                            WorldGen.PlaceTile(abyssIndex, abyssIndex2 + 1, (ushort)ModContent.TileType<SulphurousVines>());
                        }
                    }
                    if (tileToGrowVineOn.TileType == ModContent.TileType<SulphurousVines>())
                    {
                        CalamityUtils.GrowVines(abyssIndex, abyssIndex2, WorldGen.genRand.Next(1, 4), (ushort)ModContent.TileType<SulphurousVines>());
                    }
                }
            }

            //ugh ill edit this later so i can customize the check positions and tiles for columns
            //SulphurousSea.GenerateColumnsInCaverns();
        }

        public static void AbyssIsland(int i, int j, int sizeMin, int sizeMax, int sizeMin2, int sizeMax2, int tileType, bool hasChest, bool hasClumps, bool hasScoria)
        {
            float islandWidth = WorldGen.genRand.Next(sizeMin, sizeMax); //100 150
            float smallIslandWidth = (float)WorldGen.genRand.Next(sizeMin, sizeMax) / (float)5; //20 30
            int islandPositionX = i;
            int islandPositionXAgain = i;
            int islandPositionY = j;
            int islandPositionYAgain = j;
            Vector2 islandOrigin;
            islandOrigin.X = i;
            islandOrigin.Y = j;
            Vector2 vector2;
            vector2.X = WorldGen.genRand.Next(-20, 21) * 0.2f;
            while (vector2.X > -2f && vector2.X < 2f)
            {
                vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f; //What the fuck is this loop for? Why not just use a nextFloat??? What
            }
            vector2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.02f;

            while (islandWidth > 0f && smallIslandWidth > 0f)
            {
                islandWidth -= WorldGen.genRand.Next(4);
                smallIslandWidth -= 1f;

                int islandLeftX = Math.Clamp((int)(islandOrigin.X - islandWidth * 0.5), 0, Main.maxTilesX);
                int islandRightX = Math.Clamp((int)(islandOrigin.X + islandWidth * 0.5), 0, Main.maxTilesX);
                int islandTopY = Math.Clamp((int)(islandOrigin.Y - islandWidth * 0.5), 0, Main.maxTilesY);
                int islandBottomY = Math.Clamp((int)(islandOrigin.Y + islandWidth * 0.5), 0, Main.maxTilesY);

                double extraIslandWidth = islandWidth * (double)WorldGen.genRand.Next(sizeMin, sizeMax) * 0.01; //80 120
                float islandYOffset = islandOrigin.Y + 1f;
                for (int k = islandLeftX; k < islandRightX; k++)
                {
                    if (WorldGen.genRand.NextBool(2))
                    {
                        islandYOffset += (float)WorldGen.genRand.Next(-1, 2);
                    }
                    if (islandYOffset < islandOrigin.Y)
                    {
                        islandYOffset = islandOrigin.Y;
                    }
                    if (islandYOffset > islandOrigin.Y + 2f)
                    {
                        islandYOffset = islandOrigin.Y + 2f;
                    }
                    for (int l = islandTopY; l < islandBottomY; l++)
                    {
                        if ((float)l > islandYOffset)
                        {
                            float tileCheckXDist = Math.Abs((float)k - islandOrigin.X);
                            float tileCheckYDist = Math.Abs((float)l - islandOrigin.Y) * 3f;
                            if (Math.Sqrt((double)(tileCheckXDist * tileCheckXDist + tileCheckYDist * tileCheckYDist)) < extraIslandWidth * 0.4)
                            {
                                if (k < islandPositionX)
                                {
                                    islandPositionX = k;
                                }
                                if (k > islandPositionXAgain)
                                {
                                    islandPositionXAgain = k;
                                }
                                if (l < islandPositionYAgain)
                                {
                                    islandPositionYAgain = l;
                                }
                                if (l > islandPositionY)
                                {
                                    islandPositionY = l;
                                }
                                Main.tile[k, l].Get<TileWallWireStateData>().HasTile = true;

                                Main.tile[k, l].TileType = (ushort)tileType;

                                CalamityUtils.SafeSquareTileFrame(k, l, true);
                            }
                        }
                    }
                }
                islandOrigin += vector2;
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
            int m = islandPositionX;
            int randMinMaxValues;
            for (m += WorldGen.genRand.Next(5); m < islandPositionXAgain; m += WorldGen.genRand.Next(randMinMaxValues, (int)(randMinMaxValues * 1.5)))
            {
                int islandTileY = islandPositionY;
                while (!Main.tile[m, islandTileY].HasTile)
                {
                    islandTileY--;
                }
                islandTileY += WorldGen.genRand.Next(-3, 4);
                randMinMaxValues = WorldGen.genRand.Next(4, 8);
                int placedTile = tileType;

                //chance to place clumps of whatever defined block
                if (hasClumps && WorldGen.genRand.NextBool(3))
                {
                    //if the tile isnt pyre mantle place scoria or planty mush
                    if (tileType != ModContent.TileType<PyreMantle>())
                    {
                        //if scoria is enabled, then place it, otherwise place planty mush clumps
                        placedTile = hasScoria ? ModContent.TileType<ScoriaOre>() : ModContent.TileType<PlantyMush>();
                    }
                    //if the tile is pyre mantle then always place scoria
                    else
                    {
                        //place molten pyre mantle and scorcia ore
                        placedTile = hasScoria ? ModContent.TileType<ScoriaOre>() : ModContent.TileType<PyreMantleMolten>();
                    }
                }

                for (int n = m - randMinMaxValues; n <= m + randMinMaxValues; n++)
                {
                    for (int p = islandTileY - randMinMaxValues; p <= islandTileY + randMinMaxValues; p++)
                    {
                        if (p > islandPositionYAgain)
                        {
                            float islandTileXDist = (float)Math.Abs(n - m);
                            float islandTileYDist = (float)(Math.Abs(p - islandTileY) * 2);
                            if (Math.Sqrt((double)(islandTileXDist * islandTileXDist + islandTileYDist * islandTileYDist)) < (double)(randMinMaxValues + WorldGen.genRand.Next(2)))
                            {
                                Main.tile[n, p].Get<TileWallWireStateData>().HasTile = true;

                                Main.tile[n, p].TileType = (ushort)placedTile;
                                CalamityUtils.SafeSquareTileFrame(n, p, true);
                            }
                        }
                    }
                }
            }
            
            int sizeMinSmall2 = sizeMin2 / 8;
            int sizeMaxSmall2 = sizeMax2 / 8;
            islandWidth = WorldGen.genRand.Next(sizeMin2, sizeMax2);
            smallIslandWidth = WorldGen.genRand.Next(sizeMinSmall2, sizeMaxSmall2);
            islandOrigin.X = i;
            islandOrigin.Y = islandPositionYAgain;
            vector2.X = WorldGen.genRand.Next(-20, 21) * 0.2f;
            while (vector2.X > -2f && vector2.X < 2f)
            {
                vector2.X = WorldGen.genRand.Next(-20, 21) * 0.2f;
            }
            vector2.Y = WorldGen.genRand.Next(-20, -10) * 0.02f;
            while (islandWidth > 0.0 && smallIslandWidth > 0f)
            {
                islandWidth -= WorldGen.genRand.Next(4);
                smallIslandWidth -= 1f;
                islandOrigin += vector2;
                vector2.X += WorldGen.genRand.Next(-20, 21) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if (vector2.Y > 0.2)
                {
                    vector2.Y = -0.2f;
                }
                if (vector2.Y < -0.2)
                {
                    vector2.Y = -0.2f;
                }
            }
            int islandXOffsetPos = islandPositionX;
            islandXOffsetPos += WorldGen.genRand.Next(5);
            while (islandXOffsetPos < islandPositionXAgain)
            {
                int islandYOffsetPos = islandPositionY;
                while ((!Main.tile[islandXOffsetPos, islandYOffsetPos].HasTile || Main.tile[islandXOffsetPos, islandYOffsetPos].TileType != 0) && islandXOffsetPos < islandPositionXAgain)
                {
                    islandYOffsetPos--;
                    if (islandYOffsetPos < islandPositionYAgain)
                    {
                        islandYOffsetPos = islandPositionY;
                        islandXOffsetPos += WorldGen.genRand.Next(1, 4);
                    }
                }
                if (islandXOffsetPos < islandPositionXAgain)
                {
                    islandYOffsetPos += WorldGen.genRand.Next(0, 4);
                    int islandOffsetRandValues = WorldGen.genRand.Next(2, 5);
                    int placeTile = tileType;
                    for (int r = islandXOffsetPos - islandOffsetRandValues; r <= islandXOffsetPos + islandOffsetRandValues; r++)
                    {
                        for (int s = islandYOffsetPos - islandOffsetRandValues; s <= islandYOffsetPos + islandOffsetRandValues; s++)
                        {
                            if (s > islandPositionYAgain)
                            {
                                float islandOffsetXDist = Math.Abs(r - islandXOffsetPos);
                                float islandOffsetYDist = (Math.Abs(s - islandYOffsetPos) * 2);
                                if (Math.Sqrt((double)(islandOffsetXDist * islandOffsetXDist + islandOffsetYDist * islandOffsetYDist)) < islandOffsetRandValues)
                                {
                                    Main.tile[r, s].TileType = (ushort)placeTile;
                                    CalamityUtils.SafeSquareTileFrame(r, s, true);
                                }
                            }
                        }
                    }

                    islandXOffsetPos += WorldGen.genRand.Next(islandOffsetRandValues, (int)(islandOffsetRandValues * 1.5));
                }
            }
        }

        public static void AbyssChest(int i, int j, int itemChoice)
        {
            //place an island
            //always place abyss gravel because chests will never place outside of layer 2 for now
            AbyssIsland(i, j + 2, 55, 75, 35, 45, ModContent.TileType<AbyssGravel>(), false, true, false);

            //clear decently big circular area where the chest will be
            for (int clearX = i - 2; clearX <= i + 2; clearX++)
            {
                for (int clearY = j - 8; clearY <= j - 1; clearY++)
                {
                    //clear area for the chest to place
                    ShapeData circle = new ShapeData();
                    GenAction blotchMod = new Modifiers.Blotches(2, 0.4);

                    int radius = (int)(WorldGen.genRand.Next(3, 5) * WorldGen.genRand.NextFloat(0.74f, 0.82f));
                    
                    WorldUtils.Gen(new Point(clearX, clearY), new Shapes.Circle(radius), Actions.Chain(new GenAction[]
                    {
                        blotchMod.Output(circle)
                    }));

                    WorldUtils.Gen(new Point(clearX, clearY), new ModShapes.All(circle), Actions.Chain(new GenAction[]
                    {
                        new Actions.ClearTile()
                    }));
                }
            }

            //place cluster where chest will spawn
            WorldGen.TileRunner(i, j + 4, 8, 5, ModContent.TileType<AbyssGravel>(), true, 0, 0, false, true);

            //clear another small square exactly around where the chest will be so it has a flat area to place on
            for (int clearX = i - 7; clearX <= i + 7; clearX++)
            {
                for (int clearY = j - 1; clearY <= j + 5; clearY++)
                {
                    WorldGen.PlaceTile(clearX, clearY, ModContent.TileType<AbyssGravel>());
                }
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
                    itemChoice = ItemID.NeptunesShell;
                    break; //fallback case. Should never happen naturally
            }

            //place the chest itself
            //TODO: will create a proper loot pool to go in abyss chests along with the main item
            int ChestIndex = WorldGen.PlaceChest(i, j - 2, (ushort)ModContent.TileType<Tiles.Abyss.AbyssTreasureChest>(), false, 1);

            int[] Potions1 = new int[] { ItemID.ShinePotion, ItemID.GillsPotion, ItemID.TeleportationPotion, ItemID.LuckPotionLesser };
            int[] Potions2 = new int[] { ItemID.ThornsPotion, ItemID.FlipperPotion, ItemID.BattlePotion, ItemID.IronskinPotion };

            if (ChestIndex != -1)
            {
                Main.chest[ChestIndex].item[0].SetDefaults(itemChoice);

                Main.chest[ChestIndex].item[1].SetDefaults(WorldGen.genRand.Next(Potions1));
                Main.chest[ChestIndex].item[1].stack = WorldGen.genRand.Next(1, 3);

                Main.chest[ChestIndex].item[2].SetDefaults(WorldGen.genRand.Next(Potions2));
                Main.chest[ChestIndex].item[2].stack = WorldGen.genRand.Next(1, 3);

                Main.chest[ChestIndex].item[3].SetDefaults(ItemID.HealingPotion);
                Main.chest[ChestIndex].item[3].stack = WorldGen.genRand.Next(1, 3);

                Main.chest[ChestIndex].item[4].SetDefaults(ItemID.ManaPotion);
                Main.chest[ChestIndex].item[4].stack = WorldGen.genRand.Next(2, 5);

                Main.chest[ChestIndex].item[5].SetDefaults(ModContent.ItemType<Items.Placeables.FurnitureAbyss.AbyssTorch>());
                Main.chest[ChestIndex].item[5].stack = WorldGen.genRand.Next(3, 12);

                Main.chest[ChestIndex].item[6].SetDefaults(ItemID.GoldCoin);
                Main.chest[ChestIndex].item[6].stack = WorldGen.genRand.Next(2, 5);
            }
        }

        public static void PlaceSnailFossil(int i, int j)
        {
            //place an island
            AbyssIsland(i, j + 2, 55, 75, 35, 45, ModContent.TileType<Voidstone>(), false, false, false);

            //clear decently big circular area where the chest will be
            for (int clearX = i - 2; clearX <= i + 2; clearX++)
            {
                for (int clearY = j - 8; clearY <= j - 1; clearY++)
                {
                    //clear area for the chest to place
                    ShapeData circle = new ShapeData();
                    GenAction blotchMod = new Modifiers.Blotches(2, 0.4);

                    int radius = (int)(WorldGen.genRand.Next(3, 5) * WorldGen.genRand.NextFloat(0.74f, 0.82f));
                    
                    WorldUtils.Gen(new Point(clearX, clearY), new Shapes.Circle(radius), Actions.Chain(new GenAction[]
                    {
                        blotchMod.Output(circle)
                    }));

                    WorldUtils.Gen(new Point(clearX, clearY), new ModShapes.All(circle), Actions.Chain(new GenAction[]
                    {
                        new Actions.ClearTile()
                    }));
                }
            }

            //place cluster where chest will spawn
            WorldGen.TileRunner(i, j + 4, 8, 5, ModContent.TileType<Voidstone>(), true, 0, 0, false, true);

            //clear another small square exactly around where the chest will be so it has a flat area to place on
            for (int clearX = i - 7; clearX <= i + 7; clearX++)
            {
                for (int clearY = j - 1; clearY <= j + 5; clearY++)
                {
                    WorldGen.PlaceTile(clearX, clearY, ModContent.TileType<Voidstone>());
                }
            }

            WorldGen.PlaceObject(i, j - 2, (ushort)ModContent.TileType<AbyssFossilTile>());
        }

        /// <summary>
        /// Unlocks all abyss chests, automatically synced across the server.
        /// Only run initally on the server.
        /// </summary>
        public static void UnlockAllAbyssChests()
        {
            UnlockChests = true;

            if (Main.netMode == NetmodeID.Server)
            {
                var netMessage = CalamityMod.Instance.GetPacket();
                netMessage.Write((byte)CalamityModMessageType.UnlockAbyssChests);
                netMessage.Send();
            }

            for (int c = 0; c < Main.maxChests; c++)
            {
                var chest = Main.chest[c];
                if (chest == null)
                {
                    continue;
                }

                var chestTile = Framing.GetTileSafely(chest.x, chest.y);
                if (chestTile.HasTile && chestTile.TileType == ModContent.TileType<AbyssTreasureChest>() && Chest.IsLocked(chest.x, chest.y)) 
                {
                    Chest.Unlock(chest.x, chest.y);
                }
            }

            UnlockChests = false;
        }
    }
}
