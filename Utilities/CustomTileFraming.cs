using CalamityMod.Tiles;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.AstralDesert;
using CalamityMod.Tiles.AstralSnow;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.Ores;
using CalamityMod.Tiles.SunkenSea;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod
{
    public static class CustomTileFraming
    {
        private static int[][] PlantCheckAgainst;
        private static Dictionary<ushort, ushort> VineToGrass;
        public static bool[][] tileMergeTypes;

        static CustomTileFraming()
        {
            Setup();
        }

        private static void Setup()
        {
            int size = CalamityGlobalTile.PlantTypes.Length;
            PlantCheckAgainst = new int[TileLoader.TileCount][];

            PlantCheckAgainst[TileID.Plants] = new int[3] { TileID.Grass, TileID.PlanterBox, TileID.ClayPot };
            PlantCheckAgainst[TileID.CorruptPlants] = new int[1] { TileID.CorruptGrass };
            PlantCheckAgainst[TileID.JunglePlants] = new int[1] { TileID.JungleGrass };
            PlantCheckAgainst[TileID.MushroomPlants] = new int[1] { TileID.MushroomGrass };
            PlantCheckAgainst[TileID.Plants2] = new int[3] { TileID.Grass, TileID.PlanterBox, TileID.ClayPot };
            PlantCheckAgainst[TileID.JunglePlants2] = new int[1] { TileID.JungleGrass };
            PlantCheckAgainst[TileID.HallowedPlants] = new int[1] { TileID.HallowedGrass };
            PlantCheckAgainst[TileID.HallowedPlants2] = new int[1] { TileID.HallowedGrass };
            PlantCheckAgainst[TileID.FleshWeeds] = new int[1] { TileID.FleshWeeds };
            PlantCheckAgainst[ModContent.TileType<AstralShortPlants>()] = new int[1] { ModContent.TileType<AstralGrass>() };
            PlantCheckAgainst[ModContent.TileType<AstralTallPlants>()] = new int[1] { ModContent.TileType<AstralGrass>() };

            VineToGrass = new Dictionary<ushort, ushort>();
            VineToGrass[TileID.Vines] = TileID.Grass;
            VineToGrass[TileID.CrimsonVines] = TileID.FleshGrass;
            VineToGrass[TileID.HallowedVines] = TileID.HallowedGrass;
            VineToGrass[(ushort)ModContent.TileType<AstralVines>()] = (ushort)ModContent.TileType<AstralGrass>();

            tileMergeTypes = new bool[TileLoader.TileCount][];
            for (int i = 0; i < tileMergeTypes.Length; i++)
            {
                tileMergeTypes[i] = new bool[TileLoader.TileCount];
            }

            tileMergeTypes[ModContent.TileType<AstralDirt>()][ModContent.TileType<AstralOre>()] = true;
            tileMergeTypes[ModContent.TileType<AstralDirt>()][ModContent.TileType<AstralStone>()] = true;
            tileMergeTypes[ModContent.TileType<AstralDirt>()][ModContent.TileType<AstralSand>()] = true;
            tileMergeTypes[ModContent.TileType<AstralDirt>()][ModContent.TileType<AstralMonolith>()] = true;
            tileMergeTypes[ModContent.TileType<AstralDirt>()][ModContent.TileType<AstralIce>()] = true;
            tileMergeTypes[ModContent.TileType<AstralSand>()][ModContent.TileType<HardenedAstralSand>()] = true;
            tileMergeTypes[ModContent.TileType<HardenedAstralSand>()][ModContent.TileType<AstralSandstone>()] = true;

            tileMergeTypes[ModContent.TileType<BrimstoneSlag>()][ModContent.TileType<CharredOre>()] = true;

            tileMergeTypes[TileID.Sandstone][ModContent.TileType<EutrophicSand>()] = true;
            tileMergeTypes[ModContent.TileType<EutrophicSand>()][ModContent.TileType<Navystone>()] = true;
            tileMergeTypes[ModContent.TileType<Navystone>()][ModContent.TileType<SeaPrism>()] = true;

            tileMergeTypes[ModContent.TileType<AbyssGravel>()][ModContent.TileType<ChaoticOre>()] = true;
            tileMergeTypes[ModContent.TileType<AbyssGravel>()][ModContent.TileType<PlantyMush>()] = true;
            tileMergeTypes[ModContent.TileType<AbyssGravel>()][ModContent.TileType<Voidstone>()] = true;
            tileMergeTypes[ModContent.TileType<AbyssGravel>()][ModContent.TileType<SulphurousSand>()] = true;
            tileMergeTypes[ModContent.TileType<PlantyMush>()][ModContent.TileType<Tenebris>()] = true;

        }

        private static bool PlantNeedsUpdate(int plantType, int checkType)
        {
            if (PlantCheckAgainst[plantType] == null)
                return false;
            int size = PlantCheckAgainst[plantType].Length;
            for (int i = 0; i < size; i++)
            {
                if (PlantCheckAgainst[plantType][i] == checkType)
                {
                    return false;
                }
            }
            return true;
        }

        public static void CheckPlants(int x, int y)
        {
            int checkType = -1;
            int plantType = Main.tile[x, y].type;

            if (y + 1 >= Main.maxTilesY)
                checkType = plantType;

            Tile tile = Main.tile[x, y];
            Tile below = Main.tile[x, y + 1];
            if (y + 1 < Main.maxTilesY && below != null && below.nactive() && !below.halfBrick() && below.slope() == 0)
                checkType = below.type;

            if (checkType == -1)
                return;

            //Check if this plant needs an update
            if (PlantNeedsUpdate(plantType, checkType))
            {
                if ((plantType == TileID.Plants || plantType == TileID.Plants2) && checkType != TileID.Grass && tile.frameX >= 162)
                {
                    Main.tile[x, y].frameX = 126;
                }
                if (plantType == TileID.JunglePlants2 && checkType != TileID.JungleGrass && tile.frameX >= 162)
                {
                    Main.tile[x, y].frameX = 126;
                }

                if (checkType == TileID.CorruptGrass)
                {
                    plantType = TileID.CorruptPlants;
                    if (tile.frameX >= 162)
                    {
                        Main.tile[x, y].frameX = 126;
                    }
                }
                else if (checkType == TileID.Grass)
                {
                    if (plantType == TileID.HallowedPlants2)
                    {
                        plantType = TileID.Plants2;
                    }
                    else
                    {
                        plantType = TileID.Plants;
                    }
                }
                else if (checkType == TileID.HallowedGrass)
                {
                    if (plantType == TileID.Plants2)
                    {
                        plantType = TileID.HallowedPlants2;
                    }
                    else
                    {
                        plantType = TileID.HallowedPlants;
                    }
                }
                else if (checkType == TileID.FleshGrass)
                {
                    plantType = TileID.FleshWeeds;
                }
                else if (checkType == TileID.MushroomGrass)
                {
                    plantType = TileID.MushroomPlants;
                    while (Main.tile[x, y].frameX > 72)
                    {
                        Main.tile[x, y].frameX -= 72;
                    }
                }
                else if (checkType == ModContent.GetInstance<CalamityMod>().TileType("AstralGrass")) //ASTRAL
                {
                    if (plantType == TileID.Plants || plantType == TileID.CorruptPlants ||
                        plantType == TileID.FleshWeeds || plantType == TileID.HallowedPlants ||
                        plantType == TileID.MushroomPlants || plantType == TileID.JunglePlants)
                    {
                        plantType = ModContent.GetInstance<CalamityMod>().TileType("AstralShortPlants");
                    }
                    else
                    {
                        plantType = ModContent.GetInstance<CalamityMod>().TileType("AstralTallPlants");
                    }
                }
                if (plantType != Main.tile[x, y].type)
                {
                    Main.tile[x, y].type = (ushort)plantType;
                    return;
                }
                WorldGen.KillTile(x, y, false, false, false);
            }
        }

        public static void VineFrame(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            int myType = tile.type;
            Tile tile2 = Main.tile[x, y - 1];
            Tile tile3 = Main.tile[x, y + 1];
            Tile tile4 = Main.tile[x - 1, y];
            Tile tile5 = Main.tile[x + 1, y];
            Tile tile6 = Main.tile[x - 1, y + 1];
            Tile tile7 = Main.tile[x + 1, y + 1];
            Tile tile8 = Main.tile[x - 1, y - 1];
            Tile tile9 = Main.tile[x + 1, y - 1];
            if (tile4 != null && tile4.active())
            {
                if (Main.tileStone[(int)tile4.type])
                {
                }
                else
                {
                    int num55 = (int)tile4.type;
                }
                if (tile4.slope() == 1 || tile4.slope() == 3)
                {
                }
            }
            if (tile5 != null && tile5.active())
            {
                if (Main.tileStone[(int)tile5.type])
                {
                }
                else
                {
                    int num56 = (int)tile5.type;
                }
                if (tile5.slope() == 2 || tile5.slope() == 4)
                {
                }
            }

            int num53;
            if (tile2 != null && tile2.active())
            {
                if (Main.tileStone[(int)tile2.type])
                {
                }
                else
                {
                    num53 = (int)tile2.type;
                }
                if (tile2.slope() == 3 || tile2.slope() == 4)
                {
                }
            }
            if (tile3 != null && tile3.active())
            {
                if (Main.tileStone[(int)tile3.type])
                {
                }
                else
                {
                    int num58 = (int)tile3.type;
                }
                if (tile3.slope() == 1 || tile3.slope() == 2)
                {
                }
            }
            if (tile8 != null && tile8.active())
            {
                if (Main.tileStone[(int)tile8.type])
                {
                }
                else
                {
                    int num52 = (int)tile8.type;
                }
            }
            if (tile9 != null && tile9.active())
            {
                if (Main.tileStone[(int)tile9.type])
                {
                }
                else
                {
                    int num54 = (int)tile9.type;
                }
            }
            if (tile6 != null && tile6.active())
            {
                if (Main.tileStone[(int)tile6.type])
                {
                }
                else
                {
                    int num57 = (int)tile6.type;
                }
            }
            if (tile7 != null && tile7.active())
            {
                if (Main.tileStone[(int)tile7.type])
                {
                }
                else
                {
                    int num59 = (int)tile7.type;
                }
            }
            if (tile.slope() == 2)
            {
            }
            if (tile.slope() == 1)
            {
            }
            if (tile.slope() == 4)
            {
            }
            if (tile.slope() == 3)
            {
            }

            if (tile2 != null)
            {
                if (!tile2.active())
                {
                    num53 = -1;
                }
                else if (tile2.bottomSlope())
                {
                    num53 = -1;
                }
                else
                {
                    num53 = (int)tile2.type;
                }
            }
            else
            {
                num53 = myType;
            }

            ushort[] vines = VineToGrass.Keys.ToArray();

            for (int i = 0; i < vines.Length; i++)
            {
                ushort myGrassType = VineToGrass[(ushort)vines[i]];
                if (myType != vines[i] && (num53 == myGrassType || num53 == vines[i]))
                {
                    Main.tile[x, y].type = vines[i];
                    WorldGen.SquareTileFrame(x, y, true);
                    return;
                }
            }

            if (num53 != myType)
            {
                bool flag17 = false;
                if (num53 == -1)
                {
                    flag17 = true;
                }
                else
                {
                    for (int i = 0; i < vines.Length; i++)
                    {
                        if (myType != TileID.Vines)
                        {
                            if (myType == vines[i] && num53 != VineToGrass[vines[i]])
                            {
                                flag17 = true;
                            }
                        }
                        else if (num53 != TileID.Grass && num53 != TileID.LeafBlock)
                        {
                            flag17 = true;
                        }
                    }
                }
                if (flag17)
                {
                    WorldGen.KillTile(x, y, false, false, false);
                }
            }
            return;
        }

        public static bool BetterGemsparkFraming(int x, int y, bool resetFrame)
        {
            Tile tile = Main.tile[x, y];

            if (tile.slope() > 0 && TileID.Sets.HasSlopeFrames[tile.type])
            {
                return true;
            }

            Tile tile2 = Main.tile[x, y - 1];
            Tile tile3 = Main.tile[x, y + 1];
            Tile tile4 = Main.tile[x - 1, y];
            Tile tile5 = Main.tile[x + 1, y];
            Tile tile6 = Main.tile[x - 1, y + 1];
            Tile tile7 = Main.tile[x + 1, y + 1];
            Tile tile8 = Main.tile[x - 1, y - 1];
            Tile tile9 = Main.tile[x + 1, y - 1];

            int myType = tile.type;

            bool left = false;
            bool right = false;
            bool up = false;
            bool down = false;
            bool upLeft = false;
            bool upRight = false;
            bool downLeft = false;
            bool downRight = false;

            if (GetMerge(tile, tile2) && (tile2.slope() == 0 || tile2.slope() == 1 || tile2.slope() == 2))
                up = true;
            if (GetMerge(tile, tile3) && (tile3.slope() == 0 || tile3.slope() == 3 || tile3.slope() == 4))
                down = true;
            if (GetMerge(tile, tile4) && (tile4.slope() == 0 || tile4.slope() == 2 || tile4.slope() == 4))
                left = true;
            if (GetMerge(tile, tile5) && (tile5.slope() == 0 || tile5.slope() == 1 || tile5.slope() == 3))
                right = true;
            if (GetMerge(tile, tile2) && GetMerge(tile, tile4) && GetMerge(tile, tile8) && (tile8.slope() == 0 || tile8.slope() == 2) && (tile2.slope() == 0 || tile2.slope() == 1 || tile2.slope() == 3) && (tile4.slope() == 0 || tile4.slope() == 3 || tile4.slope() == 4))
                upLeft = true;
            if (GetMerge(tile, tile2) && GetMerge(tile, tile5) && GetMerge(tile, tile9) && (tile9.slope() == 0 || tile9.slope() == 1) && (tile2.slope() == 0 || tile2.slope() == 2 || tile2.slope() == 4) && (tile5.slope() == 0 || tile5.slope() == 3 || tile5.slope() == 4))
                upRight = true;
            if (GetMerge(tile, tile3) && GetMerge(tile, tile4) && GetMerge(tile, tile6) && !tile6.halfBrick() && (tile6.slope() == 0 || tile6.slope() == 4) && (tile3.slope() == 0 || tile3.slope() == 1 || tile3.slope() == 3) && (tile4.slope() == 0 || tile4.slope() == 1 || tile4.slope() == 2))
                downLeft = true;
            if (GetMerge(tile, tile3) && GetMerge(tile, tile5) && GetMerge(tile, tile7) && !tile7.halfBrick() && (tile7.slope() == 0 || tile7.slope() == 3) && (tile3.slope() == 0 || tile3.slope() == 2 || tile3.slope() == 4) && (tile5.slope() == 0 || tile5.slope() == 1 || tile5.slope() == 2))
                downRight = true;

            int randomFrame;
            if (resetFrame)
            {
                randomFrame = WorldGen.genRand.Next(3);
                Main.tile[x, y].frameNumber((byte)randomFrame);
            }
            else
            {
                randomFrame = Main.tile[x, y].frameNumber();
            }

            /*
                8 2 9
                4 - 5
                6 3 7
            */

            #region L States
            if (!up && down && !left && right && !downRight)
            {
                tile.frameX = 13 * 18;
                tile.frameY = 0;
                return false;
            }
            if (!up && down && left && !right && !downLeft)
            {
                tile.frameX = 15 * 18;
                tile.frameY = 0;
                return false;
            }
            if (up && !down && !left && right && !upRight)
            {
                tile.frameX = 13 * 18;
                tile.frameY = 2 * 18;
                return false;
            }
            if (up && !down && left && !right && !upLeft)
            {
                tile.frameX = 15 * 18;
                tile.frameY = 2 * 18;
                return false;
            }
            #endregion

            #region T States
            if (!up && down && left && right && !downLeft && !downRight)
            {
                tile.frameX = 14 * 18;
                tile.frameY = 0;
                return false;
            }
            if (up && !down && left && right && !upLeft && !upRight)
            {
                tile.frameX = 14 * 18;
                tile.frameY = 2 * 18;
                return false;
            }
            if (up && down && !left && right && !downRight && !upRight)
            {
                tile.frameX = 13 * 18;
                tile.frameY = 18;
                return false;
            }
            if (up && down && left && !right && !downLeft && !upLeft)
            {
                tile.frameX = 15 * 18;
                tile.frameY = 18;
                return false;
            }
            #endregion

            #region X State
            if (up && down && left && right && !downLeft && !downRight && !upLeft && !upRight)
            {
                tile.frameX = 14 * 18;
                tile.frameY = 18;
                return false;
            }
            #endregion

            #region Inner Corner x1
            if (up && down && left && right && !downLeft && downRight && upLeft && upRight)
            {
                tile.frameX = 15 * 18;
                tile.frameY = 3 * 18;
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && upLeft && upRight)
            {
                tile.frameX = 14 * 18;
                tile.frameY = 3 * 18;
                return false;
            }
            if (up && down && left && right && downLeft && downRight && !upLeft && upRight)
            {
                tile.frameX = 15 * 18;
                tile.frameY = 4 * 18;
                return false;
            }
            if (up && down && left && right && downLeft && downRight && upLeft && !upRight)
            {
                tile.frameX = 14 * 18;
                tile.frameY = 4 * 18;
                return false;
            }
            #endregion

            #region Inner Corner x2 (same side)
            if (up && down && left && right && !downLeft && !downRight && upLeft && upRight)
            {
                tile.frameX = (short)((6 * 18) + (randomFrame * 18));
                tile.frameY = 2 * 18;
                return false;
            }
            if (up && down && left && right && downLeft && downRight && !upLeft && !upRight)
            {
                tile.frameX = (short)((6 * 18) + (randomFrame * 18));
                tile.frameY = 1 * 18;
                return false;
            }
            if (up && down && left && right && !downLeft && downRight && !upLeft && upRight)
            {
                tile.frameX = 10 * 18;
                tile.frameY = (short)(randomFrame * 18);
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && upLeft && !upRight)
            {
                tile.frameX = 11 * 18;
                tile.frameY = (short)(randomFrame * 18);
                return false;
            }
            #endregion

            #region Inner Corner x2 (opposite corners)
            if (up && down && left && right && !downLeft && downRight && upLeft && !upRight)
            {
                tile.frameX = 16 * 18;
                tile.frameY = 4 * 18;
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && !upLeft && upRight)
            {
                tile.frameX = 17 * 18;
                tile.frameY = 4 * 18;
                return false;
            }
            #endregion

            #region Inner Corner x3
            if (up && down && left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                tile.frameX = 12 * 18;
                tile.frameY = 4 * 18;
                return false;
            }
            if (up && down && left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                tile.frameX = 12 * 18;
                tile.frameY = 3 * 18;
                return false;
            }
            if (up && down && left && right && !downLeft && !downRight && upLeft && !upRight)
            {
                tile.frameX = 13 * 18;
                tile.frameY = 4 * 18;
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && !upLeft && !upRight)
            {
                tile.frameX = 13 * 18;
                tile.frameY = 3 * 18;
                return false;
            }
            #endregion

            #region Corner and Side
            if (!up && down && left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                tile.frameX = 17 * 18;
                tile.frameY = 2 * 18;
                return false;
            }
            if (!up && down && left && right && downLeft && !downRight && !upLeft && !upRight)
            {
                tile.frameX = 16 * 18;
                tile.frameY = 2 * 18;
                return false;
            }
            if (up && !down && left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                tile.frameX = 17 * 18;
                tile.frameY = 3 * 18;
                return false;
            }
            if (up && !down && left && right && !downLeft && !downRight && upLeft && !upRight)
            {
                tile.frameX = 16 * 18;
                tile.frameY = 3 * 18;
                return false;
            }
            if (up && down && !left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                tile.frameX = 16 * 18;
                tile.frameY = 0;
                return false;
            }
            if (up && down && !left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                tile.frameX = 16 * 18;
                tile.frameY = 18;
                return false;
            }
            if (up && down && left && !right && !downLeft && !downRight && upLeft && !upRight)
            {
                tile.frameX = 17 * 18;
                tile.frameY = 0;
                return false;
            }
            if (up && down && left && !right && downLeft && !downRight && !upLeft && !upRight)
            {
                tile.frameX = 17 * 18;
                tile.frameY = 18;
                return false;
            }
            #endregion

            return true;
        }

        public static bool BrimstoneFraming(int x, int y, bool resetFrame)
        {
            Tile tile = Main.tile[x, y];

            if (tile.slope() > 0 && TileID.Sets.HasSlopeFrames[tile.type])
            {
                return true;
            }

            Tile tile2 = Main.tile[x, y - 1];
            Tile tile3 = Main.tile[x, y + 1];
            Tile tile4 = Main.tile[x - 1, y];
            Tile tile5 = Main.tile[x + 1, y];
            Tile tile6 = Main.tile[x - 1, y + 1];
            Tile tile7 = Main.tile[x + 1, y + 1];
            Tile tile8 = Main.tile[x - 1, y - 1];
            Tile tile9 = Main.tile[x + 1, y - 1];

            int myType = tile.type;

            bool left = false;
            bool right = false;
            bool up = false;
            bool down = false;
            bool upLeft = false;
            bool upRight = false;
            bool downLeft = false;
            bool downRight = false;

            if (GetMerge(tile, tile2) && (tile2.slope() == 0 || tile2.slope() == 1 || tile2.slope() == 2))
                up = true;
            if (GetMerge(tile, tile3) && (tile3.slope() == 0 || tile3.slope() == 3 || tile3.slope() == 4))
                down = true;
            if (GetMerge(tile, tile4) && (tile4.slope() == 0 || tile4.slope() == 2 || tile4.slope() == 4))
                left = true;
            if (GetMerge(tile, tile5) && (tile5.slope() == 0 || tile5.slope() == 1 || tile5.slope() == 3))
                right = true;
            if (GetMerge(tile, tile2) && GetMerge(tile, tile4) && GetMerge(tile, tile8) && (tile8.slope() == 0 || tile8.slope() == 2) && (tile2.slope() == 0 || tile2.slope() == 1 || tile2.slope() == 3) && (tile4.slope() == 0 || tile4.slope() == 3 || tile4.slope() == 4))
                upLeft = true;
            if (GetMerge(tile, tile2) && GetMerge(tile, tile5) && GetMerge(tile, tile9) && (tile9.slope() == 0 || tile9.slope() == 1) && (tile2.slope() == 0 || tile2.slope() == 2 || tile2.slope() == 4) && (tile5.slope() == 0 || tile5.slope() == 3 || tile5.slope() == 4))
                upRight = true;
            if (GetMerge(tile, tile3) && GetMerge(tile, tile4) && GetMerge(tile, tile6) && !tile6.halfBrick() && (tile6.slope() == 0 || tile6.slope() == 4) && (tile3.slope() == 0 || tile3.slope() == 1 || tile3.slope() == 3) && (tile4.slope() == 0 || tile4.slope() == 1 || tile4.slope() == 2))
                downLeft = true;
            if (GetMerge(tile, tile3) && GetMerge(tile, tile5) && GetMerge(tile, tile7) && !tile7.halfBrick() && (tile7.slope() == 0 || tile7.slope() == 3) && (tile3.slope() == 0 || tile3.slope() == 2 || tile3.slope() == 4) && (tile5.slope() == 0 || tile5.slope() == 1 || tile5.slope() == 2))
                downRight = true;

            int randomFrame;
            if (resetFrame)
            {
                randomFrame = WorldGen.genRand.Next(3);
                Main.tile[x, y].frameNumber((byte)randomFrame);
            }
            else
            {
                randomFrame = Main.tile[x, y].frameNumber();
            }

            /*
                8 2 9
                4 - 5
                6 3 7
            */

            #region L States
            if (!up && down && !left && right && !downRight)
            {
                tile.frameX = (short)((16 * 18) + (randomFrame * 54));
                tile.frameY = 0;
                return false;
            }
            if (!up && down && left && !right && !downLeft)
            {
                tile.frameX = (short)((18 * 18) + (randomFrame * 54));
                tile.frameY = 0;
                return false;
            }
            if (up && !down && !left && right && !upRight)
            {
                tile.frameX = (short)((16 * 18) + (randomFrame * 54));
                tile.frameY = 2 * 18;
                return false;
            }
            if (up && !down && left && !right && !upLeft)
            {
                tile.frameX = (short)((18 * 18) + (randomFrame * 54));
                tile.frameY = 2 * 18;
                return false;
            }
            #endregion

            #region T States
            if (!up && down && left && right && !downLeft && !downRight)
            {
                tile.frameX = (short)((17 * 18) + (randomFrame * 54));
                tile.frameY = 0;
                return false;
            }
            if (up && !down && left && right && !upLeft && !upRight)
            {
                tile.frameX = (short)((17 * 18) + (randomFrame * 54));
                tile.frameY = 2 * 18;
                return false;
            }
            if (up && down && !left && right && !downRight && !upRight)
            {
                tile.frameX = (short)((16 * 18) + (randomFrame * 54));
                tile.frameY = 18;
                return false;
            }
            if (up && down && left && !right && !downLeft && !upLeft)
            {
                tile.frameX = (short)((18 * 18) + (randomFrame * 54));
                tile.frameY = 18;
                return false;
            }
            #endregion

            #region X State
            if (up && down && left && right && !downLeft && !downRight && !upLeft && !upRight)
            {
                tile.frameX = (short)((17 * 18) + (randomFrame * 54));
                tile.frameY = 18;
                return false;
            }
            #endregion

            #region Inner Corner x1
            if (up && down && left && right && !downLeft && downRight && upLeft && upRight)
            {
                tile.frameX = 14 * 18;
                tile.frameY = (short)((5 * 18) + (randomFrame * 36));
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && upLeft && upRight)
            {
                tile.frameX = 13 * 18;
                tile.frameY = (short)((5 * 18) + (randomFrame * 36));
                return false;
            }
            if (up && down && left && right && downLeft && downRight && !upLeft && upRight)
            {
                tile.frameX = 14 * 18;
                tile.frameY = (short)((6 * 18) + (randomFrame * 36));
                return false;
            }
            if (up && down && left && right && downLeft && downRight && upLeft && !upRight)
            {
                tile.frameX = 13 * 18;
                tile.frameY = (short)((6 * 18) + (randomFrame * 36));
                return false;
            }
            #endregion

            #region Inner Corner x2 (same side)
            if (up && down && left && right && !downLeft && !downRight && upLeft && upRight)
            {
                tile.frameX = (short)((6 * 18) + (randomFrame * 18));
                tile.frameY = 2 * 18;
                return false;
            }
            if (up && down && left && right && downLeft && downRight && !upLeft && !upRight)
            {
                tile.frameX = (short)((6 * 18) + (randomFrame * 18));
                tile.frameY = 1 * 18;
                return false;
            }
            if (up && down && left && right && !downLeft && downRight && !upLeft && upRight)
            {
                tile.frameX = 10 * 18;
                tile.frameY = (short)(randomFrame * 18);
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && upLeft && !upRight)
            {
                tile.frameX = 11 * 18;
                tile.frameY = (short)(randomFrame * 18);
                return false;
            }
            #endregion

            #region Inner Corner x2 (opposite corners)
            if (up && down && left && right && !downLeft && downRight && upLeft && !upRight)
            {
                tile.frameX = (short)((10 * 18) + (randomFrame * 18));
                tile.frameY = 4 * 18;
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && !upLeft && upRight)
            {
                tile.frameX = (short)((13 * 18) + (randomFrame * 18));
                tile.frameY = 4 * 18;
                return false;
            }
            #endregion

            #region Inner Corner x3
            if (up && down && left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                tile.frameX = 15 * 18;
                tile.frameY = (short)((6 * 18) + (randomFrame * 36));
                return false;
            }
            if (up && down && left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                tile.frameX = 15 * 18;
                tile.frameY = (short)((5 * 18) + (randomFrame * 36));
                return false;
            }
            if (up && down && left && right && !downLeft && !downRight && upLeft && !upRight)
            {
                tile.frameX = 16 * 18;
                tile.frameY = (short)((6 * 18) + (randomFrame * 36));
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && !upLeft && !upRight)
            {
                tile.frameX = 16 * 18;
                tile.frameY = (short)((5 * 18) + (randomFrame * 36));
                return false;
            }
            #endregion

            #region Corner and Side
            if (!up && down && left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                tile.frameX = (short)((17 * 18) + (randomFrame * 36));
                tile.frameY = 3 * 18;
                return false;
            }
            if (!up && down && left && right && downLeft && !downRight && !upLeft && !upRight)
            {
                tile.frameX = (short)((16 * 18) + (randomFrame * 36));
                tile.frameY = 3 * 18;
                return false;
            }
            if (up && !down && left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                tile.frameX = (short)((17 * 18) + (randomFrame * 36));
                tile.frameY = 4 * 18;
                return false;
            }
            if (up && !down && left && right && !downLeft && !downRight && upLeft && !upRight)
            {
                tile.frameX = (short)((16 * 18) + (randomFrame * 36));
                tile.frameY = 4 * 18;
                return false;
            }
            if (up && down && !left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                tile.frameX = 17 * 18;
                tile.frameY = (short)((5 * 18) + (randomFrame * 36));
                return false;
            }
            if (up && down && !left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                tile.frameX = 17 * 18;
                tile.frameY = (short)((6 * 18) + (randomFrame * 36));
                return false;
            }
            if (up && down && left && !right && !downLeft && !downRight && upLeft && !upRight)
            {
                tile.frameX = 18 * 18;
                tile.frameY = (short)((5 * 18) + (randomFrame * 36));
                return false;
            }
            if (up && down && left && !right && downLeft && !downRight && !upLeft && !upRight)
            {
                tile.frameX = 18 * 18;
                tile.frameY = (short)((6 * 18) + (randomFrame * 36));
                return false;
            }
            #endregion

            return true;
        }

        enum Similarity
        {
            Same,
            MergeLink,
            None
        }
        public static void FrameTileForCustomMerge(int x, int y, int myType, int mergeType, bool forceSameDown = false, bool forceSameUp = false, bool forceSameLeft = false, bool forceSameRight = false, bool resetFrame = true)
        {
            bool tmp;
            FrameTileForCustomMerge(x, y, myType, mergeType, out tmp, out tmp, out tmp, out tmp, forceSameDown, forceSameUp, forceSameLeft, forceSameRight, resetFrame);
        }

        public static void FrameTileForCustomMerge(int x, int y, int myType, int mergeType, out bool mergedUp, out bool mergedLeft, out bool mergedRight, out bool mergedDown, bool forceSameDown = false, bool forceSameUp = false, bool forceSameLeft = false, bool forceSameRight = false, bool resetFrame = true)
        {
            Tile tileLeft = Main.tile[x - 1, y];
            Tile tileRight = Main.tile[x + 1, y];
            Tile tileUp = Main.tile[x, y - 1];
            Tile tileDown = Main.tile[x, y + 1];
            Tile tileTopLeft = Main.tile[x - 1, y - 1];
            Tile tileTopRight = Main.tile[x + 1, y - 1];
            Tile tileBottomLeft = Main.tile[x - 1, y + 1];
            Tile tileBottomRight = Main.tile[x + 1, y + 1];

            Main.tileMerge[myType][mergeType] = false;

            //CARDINAL
            Similarity leftSim = GetSimilarity(tileLeft, myType, mergeType);
            if (forceSameLeft)
                leftSim = Similarity.Same;
            Similarity rightSim = GetSimilarity(tileRight, myType, mergeType);
            if (forceSameRight)
                rightSim = Similarity.Same;
            Similarity upSim = GetSimilarity(tileUp, myType, mergeType);
            if (forceSameUp)
                upSim = Similarity.Same;
            Similarity downSim = GetSimilarity(tileDown, myType, mergeType);
            if (forceSameDown)
                downSim = Similarity.Same;

            //DIAGONAL
            Similarity topLeftSim = GetSimilarity(tileTopLeft, myType, mergeType);
            Similarity topRightSim = GetSimilarity(tileTopRight, myType, mergeType);
            Similarity bottomLeftSim = GetSimilarity(tileBottomLeft, myType, mergeType);
            Similarity bottomRightSim = GetSimilarity(tileBottomRight, myType, mergeType);

            int randomFrame;
            if (resetFrame)
            {
                randomFrame = WorldGen.genRand.Next(3);
                Main.tile[x, y].frameNumber((byte)randomFrame);
            }
            else
            {
                randomFrame = Main.tile[x, y].frameNumber();
            }

            /* DEBUGGING
                if (Player.tileTargetX == x && Player.tileTargetY == y)
                {
                    Main.NewText(StringWriter(leftSim, rightSim, upSim, downSim, topLeftSim, topRightSim, bottomLeftSim, bottomRightSim, forceSameDown, forceSameUp, forceSameLeft, forceSameRight));
                }*/

            mergedDown = mergedLeft = mergedRight = mergedUp = false;

            if (leftSim == Similarity.None)
            {
                if (upSim == Similarity.Same)
                {
                    if (downSim == Similarity.Same)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            SetFrameAt(x, y, 0, 18 * randomFrame);
                            return;
                        }
                        else if (rightSim == Similarity.MergeLink)
                        {
                            mergedRight = true;
                            SetFrameAt(x, y, 234 + 18 * randomFrame, 36);
                            return;
                        }
                        SetFrameAt(x, y, 90, 18 * randomFrame);
                        return;
                    }
                    else if (downSim == Similarity.MergeLink)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            mergedDown = true;
                            SetFrameAt(x, y, 72, 90 + 18 * randomFrame);
                            return;
                        }
                        else if (rightSim == Similarity.MergeLink)
                        {
                            SetFrameAt(x, y, 108 + 18 * randomFrame, 54);
                            return;
                        }
                        mergedDown = true;
                        SetFrameAt(x, y, 126, 90 + 18 * randomFrame);
                        return;
                    }
                    if (rightSim == Similarity.Same)
                    {
                        SetFrameAt(x, y, 36 * randomFrame, 72);
                        return;
                    }
                    SetFrameAt(x, y, 108 + 18 * randomFrame, 54);
                    return;
                }
                else if (upSim == Similarity.MergeLink)
                {
                    if (downSim == Similarity.Same)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            mergedUp = true;
                            SetFrameAt(x, y, 72, 144 + 18 * randomFrame);
                            return;
                        }
                        else if (rightSim == Similarity.MergeLink)
                        {
                            SetFrameAt(x, y, 108 + 18 * randomFrame, 0);
                            return;
                        }
                        mergedUp = true;
                        SetFrameAt(x, y, 126, 144 + 18 * randomFrame);
                        return;
                    }
                    else if (downSim == Similarity.MergeLink)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            SetFrameAt(x, y, 162, 18 * randomFrame);
                            return;
                        }
                        else if (rightSim == Similarity.MergeLink)
                        {
                            SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                            return;
                        }
                        mergedUp = true;
                        mergedDown = true;
                        SetFrameAt(x, y, 108, 216 + 18 * randomFrame);
                        return;
                    }
                    if (rightSim == Similarity.Same)
                    {
                        SetFrameAt(x, y, 162, 18 * randomFrame);
                        return;
                    }
                    else if (rightSim == Similarity.MergeLink)
                    {
                        SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                        return;
                    }
                    mergedUp = true;
                    SetFrameAt(x, y, 108, 144 + 18 * randomFrame);
                    return;
                }
                if (downSim == Similarity.Same)
                {
                    if (rightSim == Similarity.Same)
                    {
                        SetFrameAt(x, y, 36 * randomFrame, 54);
                        return;
                    }
                    else if (rightSim == Similarity.MergeLink)
                    {
                        SetFrameAt(x, y, 108 + 18 * randomFrame, 0);
                        return;
                    }
                    SetFrameAt(x, y, 108 + 18 * randomFrame, 0);
                    return;
                }
                else if (downSim == Similarity.MergeLink)
                {
                    if (rightSim == Similarity.Same)
                    {
                        SetFrameAt(x, y, 162, 18 * randomFrame);
                        return;
                    }
                    else if (rightSim == Similarity.MergeLink)
                    {
                        SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                        return;
                    }
                    mergedDown = true;
                    SetFrameAt(x, y, 108, 90 + 18 * randomFrame);
                    return;
                }
                if (rightSim == Similarity.Same)
                {
                    SetFrameAt(x, y, 162, 18 * randomFrame);
                    return;
                }
                else if (rightSim == Similarity.MergeLink)
                {
                    mergedRight = true;
                    SetFrameAt(x, y, 54 + 18 * randomFrame, 234);
                    return;
                }
                SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                return;
            }
            else if (leftSim == Similarity.MergeLink)
            {
                if (upSim == Similarity.Same)
                {
                    if (downSim == Similarity.Same)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            mergedLeft = true;
                            SetFrameAt(x, y, 162, 126 + 18 * randomFrame);
                            return;
                        }
                        else if (rightSim == Similarity.MergeLink)
                        {
                            mergedLeft = true;
                            mergedRight = true;
                            SetFrameAt(x, y, 180, 126 + 18 * randomFrame);
                            return;
                        }
                        mergedLeft = true;
                        SetFrameAt(x, y, 234 + 18 * randomFrame, 54);
                        return;
                    }
                    else if (downSim == Similarity.MergeLink)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            mergedLeft = mergedDown = true;
                            SetFrameAt(x, y, 36, 108 + 36 * randomFrame);
                            return;
                        }
                        else if (rightSim == Similarity.MergeLink)
                        {
                            mergedLeft = mergedRight = mergedDown = true;
                            SetFrameAt(x, y, 198, 144 + 18 * randomFrame);
                            return;
                        }
                        SetFrameAt(x, y, 108 + 18 * randomFrame, 54);
                        return;
                    }
                    if (rightSim == Similarity.Same)
                    {
                        mergedLeft = true;
                        SetFrameAt(x, y, 18 * randomFrame, 216);
                        return;
                    }
                    SetFrameAt(x, y, 108 + 18 * randomFrame, 54);
                    return;
                }
                else if (upSim == Similarity.MergeLink)
                {
                    if (downSim == Similarity.Same)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            mergedUp = mergedLeft = true;
                            SetFrameAt(x, y, 36, 90 + 36 * randomFrame);
                            return;
                        }
                        else if (rightSim == Similarity.MergeLink)
                        {
                            mergedLeft = mergedRight = mergedUp = true;
                            SetFrameAt(x, y, 198, 90 + 18 * randomFrame);
                            return;
                        }
                        SetFrameAt(x, y, 108 + 18 * randomFrame, 0);
                        return;
                    }
                    else if (downSim == Similarity.MergeLink)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            mergedUp = mergedLeft = mergedDown = true;
                            SetFrameAt(x, y, 216, 90 + 18 * randomFrame);
                            return;
                        }
                        else if (rightSim == Similarity.MergeLink)
                        {
                            mergedDown = mergedLeft = mergedRight = mergedUp = true;
                            SetFrameAt(x, y, 108 + 18 * randomFrame, 198);
                            return;
                        }
                        SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                        return;
                    }
                    if (rightSim == Similarity.Same)
                    {
                        SetFrameAt(x, y, 162, 18 * randomFrame);
                        return;
                    }
                    SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                    return;
                }
                if (downSim == Similarity.Same)
                {
                    if (rightSim == Similarity.Same)
                    {
                        mergedLeft = true;
                        SetFrameAt(x, y, 18 * randomFrame, 198);
                        return;
                    }
                    else if (rightSim == Similarity.MergeLink)
                    {
                        SetFrameAt(x, y, 108 + 18 * randomFrame, 0);
                        return;
                    }
                    SetFrameAt(x, y, 108 + 18 * randomFrame, 0);
                    return;
                }
                else if (downSim == Similarity.MergeLink)
                {
                    if (rightSim == Similarity.Same)
                    {
                        SetFrameAt(x, y, 162, 18 * randomFrame);
                        return;
                    }
                    else if (rightSim == Similarity.MergeLink)
                    {
                        SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                        return;
                    }
                    SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                    return;
                }
                if (rightSim == Similarity.Same)
                {
                    mergedLeft = true;
                    SetFrameAt(x, y, 18 * randomFrame, 252);
                    return;
                }
                else if (rightSim == Similarity.MergeLink)
                {
                    mergedRight = mergedLeft = true;
                    SetFrameAt(x, y, 162 + 18 * randomFrame, 198);
                    return;
                }
                mergedLeft = true;
                SetFrameAt(x, y, 18 * randomFrame, 234);
                return;
            }
            if (upSim == Similarity.Same)
            {
                if (downSim == Similarity.Same)
                {
                    if (rightSim == Similarity.Same)
                    {
                        #region FULL TILE STUFF
                        if (topLeftSim == Similarity.MergeLink || topRightSim == Similarity.MergeLink || bottomLeftSim == Similarity.MergeLink || bottomRightSim == Similarity.MergeLink)
                        {
                            if (bottomRightSim == Similarity.MergeLink)
                            {
                                SetFrameAt(x, y, 0, 90 + 36 * randomFrame);
                                return;
                            }
                            else if (bottomLeftSim == Similarity.MergeLink)
                            {
                                SetFrameAt(x, y, 18, 90 + 36 * randomFrame);
                                return;
                            }
                            else if (topRightSim == Similarity.MergeLink)
                            {
                                SetFrameAt(x, y, 0, 108 + 36 * randomFrame);
                                return;
                            }
                            SetFrameAt(x, y, 18, 108 + 36 * randomFrame);
                            return;
                        }
                        if (topLeftSim == Similarity.Same)
                        {
                            if (topRightSim == Similarity.Same)
                            {
                                if (bottomLeftSim == Similarity.Same)
                                {
                                    SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                    return;
                                }
                                if (bottomRightSim == Similarity.Same)
                                {
                                    SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                    return;
                                }
                                SetFrameAt(x, y, 108 + 18 * randomFrame, 36);
                                return;
                            }
                            if (bottomLeftSim == Similarity.Same)
                            {
                                if (bottomRightSim == Similarity.Same)
                                {
                                    if (topRightSim == Similarity.MergeLink)
                                    {
                                        SetFrameAt(x, y, 0, 108 + 36 * randomFrame);
                                        return;
                                    }
                                    SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                    return;
                                }
                                SetFrameAt(x, y, 198, 18 * randomFrame);
                                return;
                            }
                        }
                        else if (topLeftSim == Similarity.None)
                        {
                            if (topRightSim == Similarity.Same)
                            {
                                if (bottomRightSim == Similarity.Same)
                                {
                                    if (bottomLeftSim == Similarity.Same)
                                    {
                                        SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                        return;
                                    }
                                    SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                    return;
                                }
                                if (bottomLeftSim == Similarity.Same)
                                {
                                    SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                    return;
                                }
                            }
                            SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                            return;
                        }
                        SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                        return;
                        #endregion
                    }
                    else if (rightSim == Similarity.MergeLink)
                    {
                        mergedRight = true;
                        SetFrameAt(x, y, 144, 126 + 18 * randomFrame);
                        return;
                    }
                    SetFrameAt(x, y, 72, 18 * randomFrame);
                    return;
                }
                else if (downSim == Similarity.MergeLink)
                {
                    if (rightSim == Similarity.Same)
                    {
                        mergedDown = true;
                        SetFrameAt(x, y, 144 + 18 * randomFrame, 90);
                        return;
                    }
                    else if (rightSim == Similarity.MergeLink)
                    {
                        mergedDown = mergedRight = true;
                        SetFrameAt(x, y, 54, 108 + 36 * randomFrame);
                        return;
                    }
                    mergedDown = true;
                    SetFrameAt(x, y, 90, 90 + 18 * randomFrame);
                    return;
                }
                if (rightSim == Similarity.Same)
                {
                    SetFrameAt(x, y, 18 + 18 * randomFrame, 36);
                    return;
                }
                else if (rightSim == Similarity.MergeLink)
                {
                    mergedRight = true;
                    SetFrameAt(x, y, 54 + 18 * randomFrame, 216);
                    return;
                }
                SetFrameAt(x, y, 18 + 36 * randomFrame, 72);
                return;
            }
            else if (upSim == Similarity.MergeLink)
            {
                if (downSim == Similarity.Same)
                {
                    if (rightSim == Similarity.Same)
                    {
                        mergedUp = true;
                        SetFrameAt(x, y, 144 + 18 * randomFrame, 108);
                        return;
                    }
                    else if (rightSim == Similarity.MergeLink)
                    {
                        mergedRight = mergedUp = true;
                        SetFrameAt(x, y, 54, 90 + 36 * randomFrame);
                        return;
                    }
                    mergedUp = true;
                    SetFrameAt(x, y, 90, 144 + 18 * randomFrame);
                    return;
                }
                else if (downSim == Similarity.MergeLink)
                {
                    if (rightSim == Similarity.Same)
                    {
                        mergedUp = mergedDown = true;
                        SetFrameAt(x, y, 144 + 18 * randomFrame, 180);
                        return;
                    }
                    else if (rightSim == Similarity.MergeLink)
                    {
                        mergedUp = mergedRight = mergedDown = true;
                        SetFrameAt(x, y, 216, 144 + 18 * randomFrame);
                        return;
                    }
                    SetFrameAt(x, y, 216, 18 * randomFrame);
                    return;
                }
                if (rightSim == Similarity.Same)
                {
                    mergedUp = true;
                    SetFrameAt(x, y, 234 + 18 * randomFrame, 18);
                    return;
                }
                SetFrameAt(x, y, 216, 18 * randomFrame);
                return;
            }
            if (downSim == Similarity.Same)
            {
                if (rightSim == Similarity.Same)
                {
                    SetFrameAt(x, y, 18 + 18 * randomFrame, 0);
                    return;
                }
                else if (rightSim == Similarity.MergeLink)
                {
                    mergedRight = true;
                    SetFrameAt(x, y, 54 + 18 * randomFrame, 198);
                    return;
                }
                SetFrameAt(x, y, 18 + 36 * randomFrame, 54);
                return;
            }
            else if (downSim == Similarity.MergeLink)
            {
                if (rightSim == Similarity.Same)
                {
                    mergedDown = true;
                    SetFrameAt(x, y, 234 + 18 * randomFrame, 0);
                    return;
                }
                SetFrameAt(x, y, 216, 18 * randomFrame);
                return;
            }
            if (rightSim == Similarity.Same)
            {
                SetFrameAt(x, y, 108 + 18 * randomFrame, 72);
                return;
            }
            else if (rightSim == Similarity.MergeLink)
            {
                mergedRight = true;
                SetFrameAt(x, y, 54 + 18 * randomFrame, 252);
                return;
            }
            SetFrameAt(x, y, 216, 18 * randomFrame);
            return;
        }

        public static void FrameTileForCustomMergeFrom(int x, int y, int myType)
        {
            bool forceSameUp = false;
            bool forceSameDown = false;
            bool forceSameLeft = false;
            bool forceSameRight = false;

            Tile up = Main.tile[x, y - 1];
            Tile down = Main.tile[x, y + 1];
            Tile left = Main.tile[x - 1, y];
            Tile right = Main.tile[x + 1, y];

            bool tmp;

            if (up.active() && tileMergeTypes[myType][up.type])
            {
                bool mergedDown;
                FrameTileForCustomMerge(x, y - 1, up.type, myType, out tmp, out tmp, out tmp, out mergedDown, false, false, false, false, false);
                if (mergedDown)
                {
                    forceSameUp = true;
                }
            }
            if (left.active() && tileMergeTypes[myType][left.type])
            {
                bool mergedRight;
                FrameTileForCustomMerge(x - 1, y, left.type, myType, out tmp, out tmp, out mergedRight, out tmp, false, false, false, false, false);
                if (mergedRight)
                {
                    forceSameLeft = true;
                }
            }
            if (right.active() && tileMergeTypes[myType][right.type])
            {
                bool mergedLeft;
                FrameTileForCustomMerge(x + 1, y, right.type, myType, out tmp, out mergedLeft, out tmp, out tmp, false, false, false, false, false);
                if (mergedLeft)
                {
                    forceSameRight = true;
                }
            }
            if (down.active() && tileMergeTypes[myType][down.type])
            {
                bool mergedUp;
                FrameTileForCustomMerge(x, y + 1, down.type, myType, out mergedUp, out tmp, out tmp, out tmp, false, false, false, false, false);
                if (mergedUp)
                {
                    forceSameDown = true;
                }
            }

            FrameTileForCustomMerge(x, y, myType, TileID.Dirt, forceSameDown, forceSameUp, forceSameLeft, forceSameRight);
        }

        private static void SetFrameAt(int x, int y, int frameX, int frameY)
        {
            Main.tile[x, y].frameX = (short)frameX;
            Main.tile[x, y].frameY = (short)frameY;
        }

        private static Similarity GetSimilarity(Tile check, int myType, int mergeType)
        {
            if (!check.active())
                return Similarity.None;

            if (check.type == myType || Main.tileMerge[myType][check.type])
                return Similarity.Same;
            else if (check.type == mergeType)
                return Similarity.MergeLink;

            return Similarity.None;
        }

        private static bool GetMerge(Tile myTile, Tile mergeTile)
        {
            if (!mergeTile.active())
                return false;
            return (mergeTile.type == myTile.type || Main.tileMerge[myTile.type][mergeTile.type]);
        }
        private static string StringWriter(params object[] strings)
        {
            string s = "";
            for (int i = 0; i < strings.Length; i++)
            {
                s += strings[i].ToString();
                if (i != strings.Length - 1)
                {
                    s += ", ";
                }
            }
            return s;
        }
    }
}
