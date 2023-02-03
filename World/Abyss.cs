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
        // TODO -- Abyss checks across the mod are littered with hardcoded estimate variables. At some point turn all of that into constants for easy reference and maintainability.
        public static int TotalPlacedIslandsSoFar = 0;
        public static Point[] AbyssIslandPositions = new Point[20];
        public static int[] AbyssItemArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        public static bool AtLeftSideOfWorld = false;
        public static int AbyssChasmBottom = 0;

        public static void PlaceAbyss()
        {
            int x = Main.maxTilesX;
            int y = Main.maxTilesY;
            int genLimit = x / 2;
            int rockLayer = (int)Main.rockLayer;

            int abyssChasmY = y - 250; //Underworld = y - 200
            AbyssChasmBottom = abyssChasmY - 100; //850 small 1450 medium 2050 large
            int abyssChasmX = AtLeftSideOfWorld ? genLimit - (genLimit - 135) : genLimit + (genLimit - 135); //2100 - 1965 = 135 : 2100 + 1965 = 4065

            bool plantyMushSide = true;

            if (abyssChasmX < genLimit)
            {
                plantyMushSide = false;
            }

            if (AtLeftSideOfWorld)
            {
                for (int abyssIndex = 0; abyssIndex < abyssChasmX + 80; abyssIndex++) //235
                {
                    for (int abyssIndex2 = 0; abyssIndex2 < abyssChasmY; abyssIndex2++)
                    {
                        Tile tile = Framing.GetTileSafely(abyssIndex, abyssIndex2);

                        if (tile.LiquidType == LiquidID.Lava && tile.LiquidAmount > 0)
                        {
                            tile.LiquidType = LiquidID.Water;
                            tile.LiquidAmount = 255;
                        }

                        bool canConvert = tile.HasTile && tile.TileType < TileID.Count && tile.TileType != TileID.DyePlants && tile.TileType != TileID.Trees &&
                        tile.TileType != TileID.PalmTree && tile.TileType != TileID.Sand && tile.TileType != TileID.Containers && tile.TileType != TileID.Coral &&
                        tile.TileType != TileID.BeachPiles && tile.TileType != ModContent.TileType<SulphurousSand>() && tile.TileType != ModContent.TileType<SulphurousSandstone>();

                        if (abyssIndex2 > rockLayer - WorldGen.genRand.Next(30))
                        {
                            if (abyssIndex > abyssChasmX + 75)
                            {
                                if (WorldGen.genRand.NextBool(4))
                                {
                                    if (canConvert)
                                    {
                                        tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                        tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                    }
                                    else if (!tile.HasTile)
                                    {
                                        tile.Get<TileWallWireStateData>().HasTile = true;
                                        tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                        tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                    }
                                }
                            }
                            else if (abyssIndex > abyssChasmX + 70)
                            {
                                if (WorldGen.genRand.NextBool(2))
                                {
                                    if (canConvert)
                                    {
                                        tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                        tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                    }
                                    else if (!tile.HasTile)
                                    {
                                        tile.Get<TileWallWireStateData>().HasTile = true;
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
                                    else if (abyssIndex2 > (rockLayer + y * 0.142) && WorldGen.genRand.NextBool(3))
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                        tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                    }
                                    else if (abyssIndex2 > (rockLayer + y * 0.145))
                                    {
                                        if (WorldGen.genRand.NextBool(3))
                                        {
                                            tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                        }
                                        else
                                        {
                                            tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                        }
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
                                    tile.Get<TileWallWireStateData>().HasTile = true;
                                    if (abyssIndex2 > (rockLayer + y * 0.262))
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                        tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                    }
                                    else if (abyssIndex2 > (rockLayer + y * 0.142) && WorldGen.genRand.NextBool(3))
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                        tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                    }
                                    else if (abyssIndex2 > (rockLayer + y * 0.145))
                                    {
                                        if (WorldGen.genRand.NextBool(3))
                                        {
                                            tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                        }
                                        else
                                        {
                                            tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                        }
                                        tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                    }
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
            }
            else
            {
                for (int abyssIndex = abyssChasmX - 80; abyssIndex < x; abyssIndex++) //3965
                {
                    for (int abyssIndex2 = 0; abyssIndex2 < abyssChasmY; abyssIndex2++)
                    {
                        Tile tile = Framing.GetTileSafely(abyssIndex, abyssIndex2);

                        if (tile.LiquidType == LiquidID.Lava && tile.LiquidAmount > 0)
                        {
                            tile.LiquidType = LiquidID.Water;
                            tile.LiquidAmount = 255;
                        }

                        bool canConvert = tile.HasTile && tile.TileType < TileID.Count && tile.TileType != TileID.DyePlants && tile.TileType != TileID.Trees &&
                        tile.TileType != TileID.PalmTree && tile.TileType != TileID.Sand && tile.TileType != TileID.Containers && tile.TileType != TileID.Coral &&
                        tile.TileType != TileID.BeachPiles && tile.TileType != ModContent.TileType<SulphurousSand>() && tile.TileType != ModContent.TileType<SulphurousSandstone>();

                        if (abyssIndex2 > rockLayer - WorldGen.genRand.Next(30))
                        {
                            if (abyssIndex < abyssChasmX - 75)
                            {
                                if (WorldGen.genRand.NextBool(4))
                                {
                                    if (canConvert)
                                    {
                                        tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                        tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                    }
                                    else if (!tile.HasTile)
                                    {
                                        tile.Get<TileWallWireStateData>().HasTile = true;
                                        tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                        tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                    }
                                }
                            }
                            else if (abyssIndex < abyssChasmX - 70)
                            {
                                if (WorldGen.genRand.NextBool(2))
                                {
                                    if (canConvert)
                                    {
                                        tile.WallType = (ushort)ModContent.WallType<AbyssGravelWall>();
                                        tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                    }
                                    else if (!tile.HasTile)
                                    {
                                        tile.Get<TileWallWireStateData>().HasTile = true;
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
                                    else if (abyssIndex2 > (rockLayer + y * 0.140) && WorldGen.genRand.NextBool(3))
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                        tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                    }
                                    else if (abyssIndex2 > (rockLayer + y * 0.145))
                                    {
                                        if (WorldGen.genRand.NextBool(3))
                                        {
                                            tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                        }
                                        else
                                        {
                                            tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                        }
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
                                    tile.Get<TileWallWireStateData>().HasTile = true;
                                    if (abyssIndex2 > (rockLayer + y * 0.262))
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                        tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                    }
                                    else if (abyssIndex2 > (rockLayer + y * 0.140) && WorldGen.genRand.NextBool(3))
                                    {
                                        tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                        tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                    }
                                    else if (abyssIndex2 > (rockLayer + y * 0.145))
                                    {
                                        if (WorldGen.genRand.NextBool(3))
                                        {
                                            tile.TileType = (ushort)ModContent.TileType<Voidstone>();
                                        }
                                        else
                                        {
                                            tile.TileType = (ushort)ModContent.TileType<AbyssGravel>();
                                        }
                                        tile.WallType = (ushort)ModContent.WallType<VoidstoneWallUnsafe>();
                                    }
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
            }

            MiscWorldgenRoutines.ChasmGenerator(abyssChasmX, (int)WorldGen.worldSurfaceLow, AbyssChasmBottom, true);

            int maxAbyssIslands = 11; //Small World

            if (y > 2100)
            {
                maxAbyssIslands = 20; //Large World
            }
            else if (y > 1500)
            {
                maxAbyssIslands = 16; //Medium World
            }

            //place eidolon snail fossil
            int realFossilY = 0;
            bool placedFossil = false;

            //place islands to hide the fossil (funny)
            AbyssIsland(abyssChasmX + 32, AbyssChasmBottom + 25, 55, 75, 35, 45, false, true, true);
            AbyssIsland(abyssChasmX - 32, AbyssChasmBottom + 25, 55, 75, 35, 45, false, true, true);

            while (!placedFossil)
            {
                //make sure it places at the very bottom, below where the terminus shrine is
                for (int fossilCheckY = AbyssChasmBottom + 30; fossilCheckY < AbyssChasmBottom + 75; fossilCheckY++)
                {
                    Tile tile = Main.tile[abyssChasmX, fossilCheckY];
                    Tile tileleft = Main.tile[abyssChasmX - 1, fossilCheckY];
                    Tile tileRight = Main.tile[abyssChasmX + 1, fossilCheckY];

                    if (tile.HasTile || tileleft.HasTile || tileRight.HasTile)
                    {
                        realFossilY = fossilCheckY;

                        //place a box of voidstone for the fossil to spawn on
                        for (int fossilX = abyssChasmX - 12; fossilX <= abyssChasmX + 12; fossilX++)
                        {
                            for (int fossilY = realFossilY - 2; fossilY <= realFossilY + 3; fossilY++)
                            {
                                WorldGen.PlaceTile(fossilX, fossilY, ModContent.TileType<Voidstone>());
                            }
                        }

                        //kill any tiles in a small area above to prevent the fossil from not placing correctly
                        for (int fossilX = abyssChasmX - 2; fossilX <= abyssChasmX + 2; fossilX++)
                        {
                            for (int fossilY = realFossilY - 3; fossilY <= realFossilY - 8; fossilY++)
                            {
                                WorldGen.KillTile(fossilX, fossilY);
                            }
                        }

                        WorldGen.PlaceObject(abyssChasmX, realFossilY - 3, (ushort)ModContent.TileType<AbyssFossilTile>());
                        
                        placedFossil = true;
                    }
                }
            }

            //place a single abyss island under the terminus shrine
            AbyssIsland(abyssChasmX, AbyssChasmBottom + 5, 65, 75, 40, 45, false, false, true);

            //place terminus shrine
            UndergroundShrines.SpecialHut((ushort)ModContent.TileType<SmoothVoidstone>(), (ushort)ModContent.TileType<Voidstone>(),
            (ushort)ModContent.WallType<VoidstoneWallUnsafe>(), UndergroundShrines.UndergroundShrineType.Abyss, abyssChasmX, AbyssChasmBottom);

            int islandLocationOffset = 30;
            int islandLocationY = rockLayer;

            for (int islands = 0; islands < maxAbyssIslands; islands++)
            {
                int islandLocationX = abyssChasmX;
                int randomIsland = WorldGen.genRand.Next(5); //0 1 2 3 4
                bool hasVoidstone = islandLocationY > (rockLayer + y * 0.143);
                AbyssIslandPositions[TotalPlacedIslandsSoFar].Y = islandLocationY;
                switch (randomIsland)
                {
                    case 0:
                        AbyssIsland(islandLocationX, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
                        AbyssIsland(islandLocationX + 40, islandLocationY + 15, 60, 66, 30, 36, false, plantyMushSide, hasVoidstone);
                        AbyssIsland(islandLocationX - 40, islandLocationY + 15, 60, 66, 30, 36, false, !plantyMushSide, hasVoidstone);
                        break;
                    case 1:
                        AbyssIsland(islandLocationX + 30, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
                        AbyssIsland(islandLocationX, islandLocationY + 15, 60, 66, 30, 36, false, plantyMushSide, hasVoidstone);
                        AbyssIsland(islandLocationX - 40, islandLocationY + 10, 60, 66, 30, 36, false, !plantyMushSide, hasVoidstone);
                        islandLocationX += 30;
                        break;
                    case 2:
                        AbyssIsland(islandLocationX - 30, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
                        AbyssIsland(islandLocationX + 30, islandLocationY + 10, 60, 66, 30, 36, false, plantyMushSide, hasVoidstone);
                        AbyssIsland(islandLocationX, islandLocationY + 15, 60, 66, 30, 36, false, !plantyMushSide, hasVoidstone);
                        islandLocationX -= 30;
                        break;
                    case 3:
                        AbyssIsland(islandLocationX + 25, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
                        AbyssIsland(islandLocationX - 5, islandLocationY + 5, 60, 66, 30, 36, false, plantyMushSide, hasVoidstone);
                        AbyssIsland(islandLocationX - 35, islandLocationY + 15, 60, 66, 30, 36, false, !plantyMushSide, hasVoidstone);
                        islandLocationX += 25;
                        break;
                    case 4:
                        AbyssIsland(islandLocationX - 25, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
                        AbyssIsland(islandLocationX + 5, islandLocationY + 15, 60, 66, 30, 36, false, plantyMushSide, hasVoidstone);
                        AbyssIsland(islandLocationX + 35, islandLocationY + 5, 60, 66, 30, 36, false, !plantyMushSide, hasVoidstone);
                        islandLocationX -= 25;
                        break;
                }

                AbyssIslandPositions[TotalPlacedIslandsSoFar].X = islandLocationX;
                TotalPlacedIslandsSoFar++;

                islandLocationY += islandLocationOffset;
                if (islandLocationY >= AbyssChasmBottom - 50)
                    break;
            }

            //place some islands lining the walls in layer 4
            for (int voidIslandY = (int)(rockLayer + y * 0.262); voidIslandY <= AbyssChasmBottom - 20; voidIslandY++)
            {
                if (WorldGen.genRand.NextBool(8))
                {
                    int randomIsland = WorldGen.genRand.Next(3);
                    switch (randomIsland)
                    {
                        case 0:
                            AbyssIsland(abyssChasmX + WorldGen.genRand.Next(42, 48), voidIslandY, 55, 75, 35, 45, false, true, true);
                            break;
                        case 1:
                            AbyssIsland(abyssChasmX - WorldGen.genRand.Next(42, 48), voidIslandY, 55, 75, 35, 45, false, true, true);
                            break;
                        case 2:
                            AbyssIsland(abyssChasmX + WorldGen.genRand.Next(42, 48), voidIslandY, 55, 75, 35, 45, false, true, true);
                            AbyssIsland(abyssChasmX - WorldGen.genRand.Next(42, 48), voidIslandY, 55, 75, 35, 45, false, true, true);
                            break;
                    }

                    //increase y value so islands cant be spammed on top of each other
                    voidIslandY += WorldGen.genRand.Next(10, 20);
                }
            }

            AbyssItemArray = CalamityUtils.ShuffleArray(AbyssItemArray);
            for (int abyssHouse = 0; abyssHouse < TotalPlacedIslandsSoFar; abyssHouse++) //11 15 19
            {
                if (abyssHouse != 20)
                {
                    AbyssIslandHouse(AbyssIslandPositions[abyssHouse].X,
                    AbyssIslandPositions[abyssHouse].Y,
                    AbyssItemArray[abyssHouse > 9 ? (abyssHouse - 10) : abyssHouse], //10 choices 0 to 9
                    AbyssIslandPositions[abyssHouse].Y > (rockLayer + y * 0.143));
                }
            }

            //clean up
            if (AtLeftSideOfWorld)
            {
                for (int abyssIndex = 5; abyssIndex < abyssChasmX + 80; abyssIndex++) //235
                {
                    for (int abyssIndex2 = 5; abyssIndex2 < abyssChasmY; abyssIndex2++)
                    {
                        Tile tile = Main.tile[abyssIndex, abyssIndex2];
                        Tile tileUp = Main.tile[abyssIndex, abyssIndex2 - 1];
                        Tile tileDown = Main.tile[abyssIndex, abyssIndex2 + 1];
                        Tile tileLeft = Main.tile[abyssIndex - 1, abyssIndex2];
                        Tile tileRight = Main.tile[abyssIndex + 1, abyssIndex2];

                        if (tile.TileType == ModContent.TileType<AbyssGravel>() || tile.TileType == ModContent.TileType<Voidstone>() || 
                        tile.TileType == ModContent.TileType<PlantyMush>() || tile.TileType == ModContent.TileType<ChaoticOre>())
                        {
                            //slope tiles
                            Tile.SmoothSlope(abyssIndex, abyssIndex2, true);

                            //kill any individual floating tiles
                            if (!tileUp.HasTile && !tileDown.HasTile && !tileLeft.HasTile && !tileRight.HasTile)
                            {
                                WorldGen.KillTile(abyssIndex, abyssIndex2);
                            }
                        }

                        if (!tile.HasTile && (tile.WallType == ModContent.WallType<AbyssGravelWall>() || tile.WallType == ModContent.WallType<VoidstoneWallUnsafe>()))
                        {
                            //fill up any air pockets with water
                            tile.LiquidType = LiquidID.Water;
                            tile.LiquidAmount = 255;
                        }

                        //kill obsidian
                        if (tile.TileType == TileID.Obsidian)
                        {
                            WorldGen.KillTile(abyssIndex, abyssIndex2);
                        }
                    }
                }
            }
            else
            {
                for (int abyssIndex = abyssChasmX - 80; abyssIndex < x - 5; abyssIndex++) //3965
                {
                    for (int abyssIndex2 = 5; abyssIndex2 < abyssChasmY; abyssIndex2++)
                    {
                        Tile tile = Main.tile[abyssIndex, abyssIndex2];
                        Tile tileUp = Main.tile[abyssIndex, abyssIndex2 - 1];
                        Tile tileDown = Main.tile[abyssIndex, abyssIndex2 + 1];
                        Tile tileLeft = Main.tile[abyssIndex - 1, abyssIndex2];
                        Tile tileRight = Main.tile[abyssIndex + 1, abyssIndex2];

                        if (tile.TileType == ModContent.TileType<AbyssGravel>() || tile.TileType == ModContent.TileType<Voidstone>() || 
                        tile.TileType == ModContent.TileType<PlantyMush>() || tile.TileType == ModContent.TileType<ChaoticOre>())
                        {
                            //slope tiles
                            Tile.SmoothSlope(abyssIndex, abyssIndex2, true);

                            //kill any individual floating tiles
                            if (!tileUp.HasTile && !tileDown.HasTile && !tileLeft.HasTile && !tileRight.HasTile)
                            {
                                WorldGen.KillTile(abyssIndex, abyssIndex2);
                            }
                        }

                        if (!tile.HasTile && (tile.WallType == ModContent.WallType<AbyssGravelWall>() || tile.WallType == ModContent.WallType<VoidstoneWallUnsafe>()))
                        {
                            //fill up any air pockets with water
                            tile.LiquidType = LiquidID.Water;
                            tile.LiquidAmount = 255;
                        }

                        //kill obsidian
                        if (tile.TileType == TileID.Obsidian)
                        {
                            WorldGen.KillTile(abyssIndex, abyssIndex2);
                        }
                    }
                }
            }

            if (AtLeftSideOfWorld)
            {
                for (int abyssIndex = 0; abyssIndex < abyssChasmX + 80; abyssIndex++) //235
                {
                    for (int abyssIndex2 = 0; abyssIndex2 < abyssChasmY; abyssIndex2++)
                    {
                        //pots
                        if (!Main.tile[abyssIndex, abyssIndex2].HasTile)
                        {
                            if (WorldGen.SolidTile(abyssIndex, abyssIndex2 + 1) && abyssIndex2 > rockLayer)
                            {
                                if (WorldGen.genRand.NextBool(5))
                                {
                                    WorldGen.PlacePot(abyssIndex, abyssIndex2, (ushort)ModContent.TileType<AbyssalPots>());
                                    CalamityUtils.SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
                                }
                            }
                            else if (WorldGen.SolidTile(abyssIndex, abyssIndex2 + 1) && abyssIndex2 < (int)Main.worldSurface)
                            {
                                if (WorldGen.genRand.NextBool(3))
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
                        //pots
                        if (!Main.tile[abyssIndex, abyssIndex2].HasTile)
                        {
                            if (WorldGen.SolidTile(abyssIndex, abyssIndex2 + 1) && abyssIndex2 > rockLayer)
                            {
                                if (WorldGen.genRand.NextBool(5))
                                {
                                    WorldGen.PlacePot(abyssIndex, abyssIndex2, (ushort)ModContent.TileType<AbyssalPots>());
                                    CalamityUtils.SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
                                }
                            }
                            else if (WorldGen.SolidTile(abyssIndex, abyssIndex2 + 1) && abyssIndex2 < (int)Main.worldSurface)
                            {
                                if (WorldGen.genRand.NextBool(3))
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
        public static void AbyssIsland(int i, int j, int sizeMin, int sizeMax, int sizeMin2, int sizeMax2, bool hasChest, bool hasPlantyMush, bool isVoid)
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

                double num11 = islandWidth * (double)WorldGen.genRand.Next(sizeMin, sizeMax) * 0.01; //80 120
                float num12 = islandOrigin.Y + 1f;
                for (int k = islandLeftX; k < islandRightX; k++)
                {
                    if (WorldGen.genRand.NextBool(2))
                    {
                        num12 += (float)WorldGen.genRand.Next(-1, 2);
                    }
                    if (num12 < islandOrigin.Y)
                    {
                        num12 = islandOrigin.Y;
                    }
                    if (num12 > islandOrigin.Y + 2f)
                    {
                        num12 = islandOrigin.Y + 2f;
                    }
                    for (int l = islandTopY; l < islandBottomY; l++)
                    {
                        if ((float)l > num12)
                        {
                            float arg_218_0 = Math.Abs((float)k - islandOrigin.X);
                            float num13 = Math.Abs((float)l - islandOrigin.Y) * 3f;
                            if (Math.Sqrt((double)(arg_218_0 * arg_218_0 + num13 * num13)) < num11 * 0.4)
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
                                Main.tile[k, l].TileType = (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>());
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
            int num15;
            for (m += WorldGen.genRand.Next(5); m < islandPositionXAgain; m += WorldGen.genRand.Next(num15, (int)(num15 * 1.5)))
            {
                int num14 = islandPositionY;
                while (!Main.tile[m, num14].HasTile)
                {
                    num14--;
                }
                num14 += WorldGen.genRand.Next(-3, 4);
                num15 = WorldGen.genRand.Next(4, 8);
                int num16 = isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>();
                if (WorldGen.genRand.NextBool(4))
                {
                    num16 = hasChest ? ModContent.TileType<ChaoticOre>() : ModContent.TileType<PlantyMush>();
                }
                for (int n = m - num15; n <= m + num15; n++)
                {
                    for (int num17 = num14 - num15; num17 <= num14 + num15; num17++)
                    {
                        if (num17 > islandPositionYAgain)
                        {
                            float arg_409_0 = (float)Math.Abs(n - m);
                            float num18 = (float)(Math.Abs(num17 - num14) * 2);
                            if (Math.Sqrt((double)(arg_409_0 * arg_409_0 + num18 * num18)) < (double)(num15 + WorldGen.genRand.Next(2)))
                            {
                                Main.tile[n, num17].Get<TileWallWireStateData>().HasTile = true;
                                Main.tile[n, num17].TileType = (ushort)num16;
                                CalamityUtils.SafeSquareTileFrame(n, num17, true);
                            }
                        }
                    }
                }
            }
            if (hasPlantyMush)
            {
                int p = islandPositionX;
                int num150;
                for (p += WorldGen.genRand.Next(5); p < islandPositionXAgain; p += WorldGen.genRand.Next(num150, (int)(num150 * 1.5)))
                {
                    int num14 = islandPositionY;
                    while (!Main.tile[p, num14].HasTile)
                    {
                        num14--;
                    }
                    num14 += WorldGen.genRand.Next(-3, 4); //-3 4
                    num150 = 1; //4 8
                    int num16 = ModContent.TileType<PlantyMush>();
                    for (int n = p - num150; n <= p + num150; n++)
                    {
                        for (int num17 = num14 - num150; num17 <= num14 + num150; num17++)
                        {
                            if (num17 > islandPositionYAgain)
                            {
                                float arg_409_0 = Math.Abs(n - p);
                                float num18 = (Math.Abs(num17 - num14) * 2);
                                if (Math.Sqrt((double)(arg_409_0 * arg_409_0 + num18 * num18)) < (num150 + WorldGen.genRand.Next(2)))
                                {
                                    Main.tile[n, num17].Get<TileWallWireStateData>().HasTile = true;
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
            int num23 = islandPositionX;
            num23 += WorldGen.genRand.Next(5);
            while (num23 < islandPositionXAgain)
            {
                int num24 = islandPositionY;
                while ((!Main.tile[num23, num24].HasTile || Main.tile[num23, num24].TileType != 0) && num23 < islandPositionXAgain)
                {
                    num24--;
                    if (num24 < islandPositionYAgain)
                    {
                        num24 = islandPositionY;
                        num23 += WorldGen.genRand.Next(1, 4);
                    }
                }
                if (num23 < islandPositionXAgain)
                {
                    num24 += WorldGen.genRand.Next(0, 4);
                    int num25 = WorldGen.genRand.Next(2, 5);
                    int num26 = isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>();
                    for (int num27 = num23 - num25; num27 <= num23 + num25; num27++)
                    {
                        for (int num28 = num24 - num25; num28 <= num24 + num25; num28++)
                        {
                            if (num28 > islandPositionYAgain)
                            {
                                float arg_890_0 = Math.Abs(num27 - num23);
                                float num29 = (Math.Abs(num28 - num24) * 2);
                                if (Math.Sqrt((double)(arg_890_0 * arg_890_0 + num29 * num29)) < num25)
                                {
                                    Main.tile[num27, num28].TileType = (ushort)num26;
                                    CalamityUtils.SafeSquareTileFrame(num27, num28, true);
                                }
                            }
                        }
                    }
                    num23 += WorldGen.genRand.Next(num25, (int)(num25 * 1.5));
                }
            }

            //Place backwall
            for (int backwallX = islandPositionX - 20; backwallX <= islandPositionXAgain + 20; backwallX++)
            {
                for (int backwallJ = islandPositionYAgain - 20; backwallJ <= islandPositionY + 20; backwallJ++)
                {
                    //Check to see if the tile is surrounded by other tiles to place a wall
                    bool AmISurroundedByTiles = true;
                    for (int i2 = backwallX - 1; i2 <= backwallX + 1; i2++)
                    {
                        for (int j2 = backwallJ - 1; j2 <= backwallJ + 1; j2++)
                        {
                            if (!Main.tile[i2, j2].HasTile)
                            {
                                AmISurroundedByTiles = false;
                            }
                        }
                    }
                    if (AmISurroundedByTiles)
                    {
                        Main.tile[backwallX, backwallJ].WallType = (ushort)(isVoid ? ModContent.WallType<VoidstoneWallUnsafe>() : ModContent.WallType<AbyssGravelWall>());
                        WorldGen.SquareWallFrame(backwallX, backwallJ, true);
                    }
                }
            }
            for (int num34 = islandPositionX; num34 <= islandPositionXAgain; num34++)
            {
                int num35 = islandPositionYAgain - 10;
                while (!Main.tile[num34, num35 + 1].HasTile)
                {
                    num35++;
                }
                if (num35 < islandPositionY && Main.tile[num34, num35 + 1].TileType == (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>()))
                {
                    if (WorldGen.genRand.NextBool(10))
                    {
                        int num36 = WorldGen.genRand.Next(1, 3);
                        for (int num37 = num34 - num36; num37 <= num34 + num36; num37++)
                        {
                            if (Main.tile[num37, num35].TileType == (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>()))
                            {
                                Main.tile[num37, num35].Get<TileWallWireStateData>().HasTile = false;
                                Main.tile[num37, num35].LiquidAmount = 255;
                                Main.tile[num37, num35].Get<LiquidData>().LiquidType = LiquidID.Water;
                                CalamityUtils.SafeSquareTileFrame(num34, num35, true);
                            }
                            if (Main.tile[num37, num35 + 1].TileType == (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>()))
                            {
                                Main.tile[num37, num35 + 1].Get<TileWallWireStateData>().HasTile = false;
                                Main.tile[num37, num35 + 1].LiquidAmount = 255;
                                Main.tile[num37, num35 + 1].Get<LiquidData>().LiquidType = LiquidID.Water;
                                CalamityUtils.SafeSquareTileFrame(num34, num35 + 1, true);
                            }
                            if (num37 > num34 - num36 && num37 < num34 + 2 && Main.tile[num37, num35 + 2].TileType == (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>()))
                            {
                                Main.tile[num37, num35 + 2].Get<TileWallWireStateData>().HasTile = false;
                                Main.tile[num37, num35 + 2].LiquidAmount = 255;
                                Main.tile[num37, num35 + 2].Get<LiquidData>().LiquidType = LiquidID.Water;
                                CalamityUtils.SafeSquareTileFrame(num34, num35 + 2, true);
                            }
                        }
                    }
                    if (WorldGen.genRand.NextBool(5))
                    {
                        Main.tile[num34, num35].LiquidAmount = 255;
                    }
                    Main.tile[num34, num35].Get<LiquidData>().LiquidType = LiquidID.Water;
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
            if (WorldGen.genRand.NextBool(2))
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
                        Main.tile[l, m].Get<TileWallWireStateData>().HasTile = true;
                        Main.tile[l, m].TileType = type;
                        Main.tile[l, m].WallType = wall;
                        Main.tile[l, m].Get<TileWallWireStateData>().IsHalfBlock = false;
                        Main.tile[l, m].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
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
                        Main.tile[n, num8].Get<TileWallWireStateData>().HasTile = false;
                    }
                }
            }
            int num9 = i + (num2 + 1) * num;
            int num10 = (int)vector.Y;
            for (int num11 = num9 - 2; num11 <= num9 + 2; num11++)
            {
                Main.tile[num11, num10].Get<TileWallWireStateData>().HasTile = false;
                Main.tile[num11, num10 - 1].Get<TileWallWireStateData>().HasTile = false;
                Main.tile[num11, num10 - 2].Get<TileWallWireStateData>().HasTile = false;
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
            WorldGen.AddBuriedChest(i, num10 - 3, itemChoice, false, 4); //chest
        }
        #endregion
    }
}