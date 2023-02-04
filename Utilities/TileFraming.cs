using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Abyss.AbyssAmbient;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.AstralDesert;
using CalamityMod.Tiles.AstralSnow;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.FurnitureAshen;
using CalamityMod.Tiles.Ores;
using CalamityMod.Tiles.SunkenSea;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod
{
    // TODO -- This can be made into a ModSystem with simple OnModLoad and Unload hooks.
    // OnModLoad is guaranteed to be run after all content has autoloaded.
    public static class TileFraming
    {
        private static int[][] PlantCheckAgainst;
        private static Dictionary<ushort, ushort> VineToGrass;

        // CONSIDER -- This is a triangle array, but does it need to be? Main.tileMerge is a triangle array as well
        public static bool[][] tileMergeTypes;

        #region Similarity Enum
        private enum Similarity
        {
            Same,
            MergeLink,
            None
        }

        private static Similarity GetSimilarity(Tile check, int myType, int mergeType)
        {
            if (!check.HasTile)
                return Similarity.None;

            if (check.TileType == myType || Main.tileMerge[myType][check.TileType])
                return Similarity.Same;
            else if (check.TileType == mergeType)
                return Similarity.MergeLink;

            return Similarity.None;
        }
        #endregion

        #region Load/Unload
        internal static void Load()
        {
            PlantCheckAgainst = new int[TileLoader.TileCount][];
            PlantCheckAgainst[TileID.Plants] = new int[3] { TileID.Grass, TileID.PlanterBox, TileID.ClayPot };
            PlantCheckAgainst[TileID.CorruptPlants] = new int[1] { TileID.CorruptGrass };
            PlantCheckAgainst[TileID.JunglePlants] = new int[1] { TileID.JungleGrass };
            PlantCheckAgainst[TileID.MushroomPlants] = new int[1] { TileID.MushroomGrass };
            PlantCheckAgainst[TileID.Plants2] = new int[3] { TileID.Grass, TileID.PlanterBox, TileID.ClayPot };
            PlantCheckAgainst[TileID.JunglePlants2] = new int[1] { TileID.JungleGrass };
            PlantCheckAgainst[TileID.HallowedPlants] = new int[1] { TileID.HallowedGrass };
            PlantCheckAgainst[TileID.HallowedPlants2] = new int[1] { TileID.HallowedGrass };
            PlantCheckAgainst[TileID.CrimsonPlants] = new int[1] { TileID.CrimsonPlants };
            PlantCheckAgainst[ModContent.TileType<AstralShortPlants>()] = new int[1] { ModContent.TileType<AstralGrass>() };
            PlantCheckAgainst[ModContent.TileType<AstralTallPlants>()] = new int[1] { ModContent.TileType<AstralGrass>() };
            PlantCheckAgainst[ModContent.TileType<CinderBlossomTallPlants>()] = new int[1] { ModContent.TileType<ScorchedRemainsGrass>() };
            PlantCheckAgainst[ModContent.TileType<AbyssKelp>()] = new int[2] { ModContent.TileType<AbyssGravel>(), ModContent.TileType<Voidstone>() };

            VineToGrass = new Dictionary<ushort, ushort>
            {
                [TileID.Vines] = TileID.Grass,
                [TileID.CrimsonVines] = TileID.CrimsonGrass,
                [TileID.HallowedVines] = TileID.HallowedGrass,
                [(ushort)ModContent.TileType<AstralVines>()] = (ushort)ModContent.TileType<AstralGrass>()
            };

            tileMergeTypes = new bool[TileLoader.TileCount][];
            for (int i = 0; i < tileMergeTypes.Length; ++i)
                tileMergeTypes[i] = new bool[TileLoader.TileCount];
            tileMergeTypes[ModContent.TileType<AstralDirt>()][ModContent.TileType<AstralOre>()] = true;
            tileMergeTypes[ModContent.TileType<AstralDirt>()][ModContent.TileType<AstralStone>()] = true;
            tileMergeTypes[ModContent.TileType<AstralDirt>()][ModContent.TileType<AstralSand>()] = true;
            tileMergeTypes[ModContent.TileType<AstralDirt>()][ModContent.TileType<AstralSnow>()] = true;
            tileMergeTypes[ModContent.TileType<AstralDirt>()][ModContent.TileType<AstralClay>()] = true;
            tileMergeTypes[ModContent.TileType<AstralDirt>()][ModContent.TileType<NovaeSlag>()] = true;
            tileMergeTypes[ModContent.TileType<AstralSnow>()][ModContent.TileType<AstralIce>()] = true;
            tileMergeTypes[ModContent.TileType<AstralSand>()][ModContent.TileType<HardenedAstralSand>()] = true;
            tileMergeTypes[ModContent.TileType<HardenedAstralSand>()][ModContent.TileType<AstralSandstone>()] = true;
            tileMergeTypes[ModContent.TileType<AstralSandstone>()][ModContent.TileType<CelestialRemains>()] = true;

            tileMergeTypes[ModContent.TileType<BrimstoneSlag>()][ModContent.TileType<CharredOre>()] = true;

            tileMergeTypes[TileID.Sandstone][ModContent.TileType<EutrophicSand>()] = true;
            tileMergeTypes[ModContent.TileType<EutrophicSand>()][ModContent.TileType<Navystone>()] = true;
            tileMergeTypes[ModContent.TileType<Navystone>()][ModContent.TileType<SeaPrism>()] = true;

            tileMergeTypes[ModContent.TileType<AbyssGravel>()][ModContent.TileType<ChaoticOre>()] = true;
            tileMergeTypes[ModContent.TileType<AbyssGravel>()][ModContent.TileType<PlantyMush>()] = true;
            tileMergeTypes[ModContent.TileType<AbyssGravel>()][ModContent.TileType<Voidstone>()] = true;
            tileMergeTypes[ModContent.TileType<AbyssGravel>()][ModContent.TileType<SulphurousSandstone>()] = true;
            tileMergeTypes[ModContent.TileType<SulphurousSandstone>()][ModContent.TileType<SulphurousSand>()] = true;
        }

        internal static void Unload()
        {
            PlantCheckAgainst = null;
            VineToGrass?.Clear();
            VineToGrass = null;
            tileMergeTypes = null;
        }
        #endregion

        #region Framing Helpers
        private static bool GetMerge(Tile myTile, Tile mergeTile)
        {
            return mergeTile.HasTile && (mergeTile.TileType == myTile.TileType || Main.tileMerge[myTile.TileType][mergeTile.TileType]);
        }

        private static void GetAdjacentTiles(int x, int y, out bool up, out bool down, out bool left, out bool right, out bool upLeft, out bool upRight, out bool downLeft, out bool downRight)
        {
            // These all get null checked in the GetMerge function
            Tile tile = Main.tile[x, y];
            Tile north = Main.tile[x, y - 1];
            Tile south = Main.tile[x, y + 1];
            Tile west = Main.tile[x - 1, y];
            Tile east = Main.tile[x + 1, y];
            Tile southwest = Main.tile[x - 1, y + 1];
            Tile southeast = Main.tile[x + 1, y + 1];
            Tile northwest = Main.tile[x - 1, y - 1];
            Tile northeast = Main.tile[x + 1, y - 1];

            left = false;
            right = false;
            up = false;
            down = false;
            upLeft = false;
            upRight = false;
            downLeft = false;
            downRight = false;

            if (GetMerge(tile, north) && (north.Slope == 0 || north.Slope == SlopeType.SlopeDownLeft || north.Slope == SlopeType.SlopeDownRight))
                up = true;
            if (GetMerge(tile, south) && (south.Slope == 0 || south.Slope == SlopeType.SlopeUpLeft || south.Slope == SlopeType.SlopeUpRight))
                down = true;
            if (GetMerge(tile, west) && (west.Slope == 0 || west.Slope == SlopeType.SlopeDownRight || west.Slope == SlopeType.SlopeUpRight))
                left = true;
            if (GetMerge(tile, east) && (east.Slope == 0 || east.Slope == SlopeType.SlopeDownLeft || east.Slope == SlopeType.SlopeUpLeft))
                right = true;
            if (GetMerge(tile, north) && GetMerge(tile, west) && GetMerge(tile, northwest) && (northwest.Slope == 0 || northwest.Slope == SlopeType.SlopeDownRight) && (north.Slope == 0 || north.Slope == SlopeType.SlopeDownLeft || north.Slope == SlopeType.SlopeUpLeft) && (west.Slope == 0 || west.Slope == SlopeType.SlopeUpLeft || west.Slope == SlopeType.SlopeUpRight))
                upLeft = true;
            if (GetMerge(tile, north) && GetMerge(tile, east) && GetMerge(tile, northeast) && (northeast.Slope == 0 || northeast.Slope == SlopeType.SlopeDownLeft) && (north.Slope == 0 || north.Slope == SlopeType.SlopeDownRight || north.Slope == SlopeType.SlopeUpRight) && (east.Slope == 0 || east.Slope == SlopeType.SlopeUpLeft || east.Slope == SlopeType.SlopeUpRight))
                upRight = true;
            if (GetMerge(tile, south) && GetMerge(tile, west) && GetMerge(tile, southwest) && !southwest.IsHalfBlock && (southwest.Slope == 0 || southwest.Slope == SlopeType.SlopeUpRight) && (south.Slope == 0 || south.Slope == SlopeType.SlopeDownLeft || south.Slope == SlopeType.SlopeUpLeft) && (west.Slope == 0 || west.Slope == SlopeType.SlopeDownLeft || west.Slope == SlopeType.SlopeDownRight))
                downLeft = true;
            if (GetMerge(tile, south) && GetMerge(tile, east) && GetMerge(tile, southeast) && !southeast.IsHalfBlock && (southeast.Slope == 0 || southeast.Slope == SlopeType.SlopeUpLeft) && (south.Slope == 0 || south.Slope == SlopeType.SlopeDownRight || south.Slope == SlopeType.SlopeUpRight) && (east.Slope == 0 || east.Slope == SlopeType.SlopeDownLeft || east.Slope == SlopeType.SlopeDownRight))
                downRight = true;
        }

        private static void SetFrameAt(int x, int y, int frameX, int frameY)
        {
            Tile tile = Main.tile[x, y];
            if (tile != null)
            {
                tile.TileFrameX = (short)frameX;
                tile.TileFrameY = (short)frameY;
            }
        }
        #endregion

        #region Specific Framing Code
        internal static void PlantFrame(int x, int y)
        {
            if (x < 0 || x >= Main.maxTilesX)
                return;
            if (y < 0 || y >= Main.maxTilesY)
                return;
            Tile tile = Main.tile[x, y];
            int checkType = -1;
            int plantType = tile.TileType;

            // If the tile below is off the bottom of the map, then assume it's the same tile type.
            if (y + 1 >= Main.maxTilesY)
                checkType = plantType;
            else
            {
                Tile below = Main.tile[x, y + 1];
                if (below != null && below.HasUnactuatedTile && !below.IsHalfBlock && below.Slope == 0)
                    checkType = below.TileType;
            }

            // Sub function to determine whether the plant needs an update
            static bool PlantNeedsUpdate(int plant, int check)
            {
                if (PlantCheckAgainst[plant] is null)
                    return false;

                for (int i = 0; i < PlantCheckAgainst[plant].Length; ++i)
                    if (PlantCheckAgainst[plant][i] == check)
                        return false;

                return true;
            }

            // If no valid below tile type could be determined, then don't do anything.
            // Additionally, don't do anything if the plant doesn't need a framing update.
            if (checkType == -1 || !PlantNeedsUpdate(plantType, checkType))
                return;

            if ((plantType == TileID.Plants || plantType == TileID.Plants2) && checkType != TileID.Grass && tile.TileFrameX >= 162)
            {
                Main.tile[x, y].TileFrameX = 126;
            }
            if (plantType == TileID.JunglePlants2 && checkType != TileID.JungleGrass && tile.TileFrameX >= 162)
            {
                Main.tile[x, y].TileFrameX = 126;
            }

            if (checkType == TileID.CorruptGrass)
            {
                plantType = TileID.CorruptPlants;
                if (tile.TileFrameX >= 162)
                {
                    Main.tile[x, y].TileFrameX = 126;
                }
            }
            else if (checkType == TileID.Grass)
            {
                plantType = plantType == TileID.HallowedPlants2 ? TileID.Plants2 : TileID.Plants;
            }
            else if (checkType == TileID.HallowedGrass)
            {
                plantType = plantType == TileID.Plants2 ? TileID.HallowedPlants2 : TileID.HallowedPlants;
            }
            else if (checkType == TileID.CrimsonGrass)
            {
                plantType = TileID.CrimsonPlants;
            }
            else if (checkType == TileID.MushroomGrass)
            {
                plantType = TileID.MushroomPlants;
                while (Main.tile[x, y].TileFrameX > 72)
                {
                    Main.tile[x, y].TileFrameX -= 72;
                }
            }

            // Astral grass and plant behavior
            else if (checkType == ModContent.TileType<AstralGrass>())
            {
                bool isShortPlant = plantType == TileID.Plants ||
                    plantType == TileID.CorruptPlants ||
                    plantType == TileID.CrimsonPlants ||
                    plantType == TileID.HallowedPlants ||
                    plantType == TileID.MushroomPlants ||
                    plantType == TileID.JunglePlants;
                plantType = isShortPlant ? ModContent.TileType<AstralShortPlants>() : ModContent.TileType<AstralTallPlants>();
            }

            // If the tile type is not the same as the plant type, then set it equal. Otherwise, destroy it.
            if (Main.tile[x, y].TileType != plantType)
                Main.tile[x, y].TileType = (ushort)plantType;
            else
                WorldGen.KillTile(x, y, false, false, false);
        }

        internal static void VineFrame(int x, int y)
        {
            if (x < 0 || x >= Main.maxTilesX)
                return;
            if (y < 0 || y >= Main.maxTilesY)
                return;

            Tile tile = Main.tile[x, y];
            int myType = tile.TileType;

            // Get the type of the tile above this vine. If that tile doesn't exist, just assume it's another vine.
            Tile north = y <= 0 ? default : Main.tile[x, y - 1];
            int northType = north == default(Tile) ? myType : (!north.HasTile || north.BottomSlope) ? -1 : north.TileType;

            // Make this vine match the tile above it if that's another vine or a grass tile.
            ushort[] vines = VineToGrass.Keys.ToArray();
            for (int i = 0; i < vines.Length; ++i)
            {
                ushort correspondingGrass = VineToGrass[vines[i]];
                if (myType != vines[i] && (northType == correspondingGrass || northType == vines[i]))
                {
                    Main.tile[x, y].TileType = vines[i];
                    WorldGen.SquareTileFrame(x, y, true);
                    return;
                }
            }
            
            // If the tile above is an identical vine, nothing else needs to be done.
            if (northType == myType)
                return;

            // If the tile above isn't sloped correctly or otherwise isn't a valid anchor for this vine, check whether the vine must die.
            bool tileMustDie = northType == -1;
            if (northType != -1)
            {
                // Vanilla vines can hang from vanilla grass and vanilla leaf blocks.
                if (myType == TileID.Vines && northType != TileID.Grass && northType != TileID.LeafBlock)
                {
                    tileMustDie = true;
                }
                else
                {
                    for (int i = 0; i < vines.Length; ++i)
                    {
                        // Not matching grass? Die.
                        if (myType == vines[i] && northType != VineToGrass[vines[i]])
                        {
                            tileMustDie = true;
                            break;
                        }
                    }
                }
            }

            if (tileMustDie)
                WorldGen.KillTile(x, y, false, false, false);
        }

        internal static bool BetterGemsparkFraming(int x, int y, bool resetFrame)
        {
            if (x < 0 || x >= Main.maxTilesX)
                return false;
            if (y < 0 || y >= Main.maxTilesY)
                return false;

            Tile tile = Main.tile[x, y];
            if (tile.Slope > 0 && TileID.Sets.HasSlopeFrames[tile.TileType])
            {
                return true;
            }

            GetAdjacentTiles(x, y, out bool up, out bool down, out bool left, out bool right, out bool upLeft, out bool upRight, out bool downLeft, out bool downRight);

            // Reset the tile's random frame style if the frame is being reset.
            int randomFrame;
            if (resetFrame)
            {
                randomFrame = WorldGen.genRand.Next(3);
                Main.tile[x, y].Get<TileWallWireStateData>().TileFrameNumber = randomFrame;
            }
            else
            {
                randomFrame = Main.tile[x, y].TileFrameNumber;
            }

            /*
                8 2 9
                4 - 5
                6 3 7
            */

            #region L States
            if (!up && down && !left && right && !downRight)
            {
                tile.TileFrameX = 13 * 18;
                tile.TileFrameY = 0;
                return false;
            }
            if (!up && down && left && !right && !downLeft)
            {
                tile.TileFrameX = 15 * 18;
                tile.TileFrameY = 0;
                return false;
            }
            if (up && !down && !left && right && !upRight)
            {
                tile.TileFrameX = 13 * 18;
                tile.TileFrameY = 2 * 18;
                return false;
            }
            if (up && !down && left && !right && !upLeft)
            {
                tile.TileFrameX = 15 * 18;
                tile.TileFrameY = 2 * 18;
                return false;
            }
            #endregion

            #region T States
            if (!up && down && left && right && !downLeft && !downRight)
            {
                tile.TileFrameX = 14 * 18;
                tile.TileFrameY = 0;
                return false;
            }
            if (up && !down && left && right && !upLeft && !upRight)
            {
                tile.TileFrameX = 14 * 18;
                tile.TileFrameY = 2 * 18;
                return false;
            }
            if (up && down && !left && right && !downRight && !upRight)
            {
                tile.TileFrameX = 13 * 18;
                tile.TileFrameY = 18;
                return false;
            }
            if (up && down && left && !right && !downLeft && !upLeft)
            {
                tile.TileFrameX = 15 * 18;
                tile.TileFrameY = 18;
                return false;
            }
            #endregion

            #region X State
            if (up && down && left && right && !downLeft && !downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 14 * 18;
                tile.TileFrameY = 18;
                return false;
            }
            #endregion

            #region Inner Corner x1
            if (up && down && left && right && !downLeft && downRight && upLeft && upRight)
            {
                tile.TileFrameX = 15 * 18;
                tile.TileFrameY = 3 * 18;
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && upLeft && upRight)
            {
                tile.TileFrameX = 14 * 18;
                tile.TileFrameY = 3 * 18;
                return false;
            }
            if (up && down && left && right && downLeft && downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 15 * 18;
                tile.TileFrameY = 4 * 18;
                return false;
            }
            if (up && down && left && right && downLeft && downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 14 * 18;
                tile.TileFrameY = 4 * 18;
                return false;
            }
            #endregion

            #region Inner Corner x2 (same side)
            if (up && down && left && right && !downLeft && !downRight && upLeft && upRight)
            {
                tile.TileFrameX = (short)((6 * 18) + (randomFrame * 18));
                tile.TileFrameY = 2 * 18;
                return false;
            }
            if (up && down && left && right && downLeft && downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = (short)((6 * 18) + (randomFrame * 18));
                tile.TileFrameY = 1 * 18;
                return false;
            }
            if (up && down && left && right && !downLeft && downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 10 * 18;
                tile.TileFrameY = (short)(randomFrame * 18);
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 11 * 18;
                tile.TileFrameY = (short)(randomFrame * 18);
                return false;
            }
            #endregion

            #region Inner Corner x2 (opposite corners)
            if (up && down && left && right && !downLeft && downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 16 * 18;
                tile.TileFrameY = 4 * 18;
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 17 * 18;
                tile.TileFrameY = 4 * 18;
                return false;
            }
            #endregion

            #region Inner Corner x3
            if (up && down && left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 12 * 18;
                tile.TileFrameY = 4 * 18;
                return false;
            }
            if (up && down && left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 12 * 18;
                tile.TileFrameY = 3 * 18;
                return false;
            }
            if (up && down && left && right && !downLeft && !downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 13 * 18;
                tile.TileFrameY = 4 * 18;
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 13 * 18;
                tile.TileFrameY = 3 * 18;
                return false;
            }
            #endregion

            #region Corner and Side
            if (!up && down && left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 17 * 18;
                tile.TileFrameY = 2 * 18;
                return false;
            }
            if (!up && down && left && right && downLeft && !downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 16 * 18;
                tile.TileFrameY = 2 * 18;
                return false;
            }
            if (up && !down && left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 17 * 18;
                tile.TileFrameY = 3 * 18;
                return false;
            }
            if (up && !down && left && right && !downLeft && !downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 16 * 18;
                tile.TileFrameY = 3 * 18;
                return false;
            }
            if (up && down && !left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 16 * 18;
                tile.TileFrameY = 0;
                return false;
            }
            if (up && down && !left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 16 * 18;
                tile.TileFrameY = 18;
                return false;
            }
            if (up && down && left && !right && !downLeft && !downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 17 * 18;
                tile.TileFrameY = 0;
                return false;
            }
            if (up && down && left && !right && downLeft && !downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 17 * 18;
                tile.TileFrameY = 18;
                return false;
            }
            #endregion

            return true;
        }

        internal static bool BrimstoneFraming(int x, int y, bool resetFrame)
        {
            if (x < 0 || x >= Main.maxTilesX)
                return false;
            if (y < 0 || y >= Main.maxTilesY)
                return false;

            Tile tile = Main.tile[x, y];
            if (tile.Slope > 0 && TileID.Sets.HasSlopeFrames[tile.TileType])
            {
                return true;
            }

            GetAdjacentTiles(x, y, out bool up, out bool down, out bool left, out bool right, out bool upLeft, out bool upRight, out bool downLeft, out bool downRight);

            // Reset the tile's random frame style if the frame is being reset.
            int randomFrame;
            if (resetFrame)
            {
                randomFrame = WorldGen.genRand.Next(3);
                Main.tile[x, y].Get<TileWallWireStateData>().TileFrameNumber = randomFrame;
            }
            else
            {
                randomFrame = Main.tile[x, y].TileFrameNumber;
            }

            /*
                8 2 9
                4 - 5
                6 3 7
            */

            #region L States
            if (!up && down && !left && right && !downRight)
            {
                tile.TileFrameX = (short)((16 * 18) + (randomFrame * 54));
                tile.TileFrameY = 0;
                return false;
            }
            if (!up && down && left && !right && !downLeft)
            {
                tile.TileFrameX = (short)((18 * 18) + (randomFrame * 54));
                tile.TileFrameY = 0;
                return false;
            }
            if (up && !down && !left && right && !upRight)
            {
                tile.TileFrameX = (short)((16 * 18) + (randomFrame * 54));
                tile.TileFrameY = 2 * 18;
                return false;
            }
            if (up && !down && left && !right && !upLeft)
            {
                tile.TileFrameX = (short)((18 * 18) + (randomFrame * 54));
                tile.TileFrameY = 2 * 18;
                return false;
            }
            #endregion

            #region T States
            if (!up && down && left && right && !downLeft && !downRight)
            {
                tile.TileFrameX = (short)((17 * 18) + (randomFrame * 54));
                tile.TileFrameY = 0;
                return false;
            }
            if (up && !down && left && right && !upLeft && !upRight)
            {
                tile.TileFrameX = (short)((17 * 18) + (randomFrame * 54));
                tile.TileFrameY = 2 * 18;
                return false;
            }
            if (up && down && !left && right && !downRight && !upRight)
            {
                tile.TileFrameX = (short)((16 * 18) + (randomFrame * 54));
                tile.TileFrameY = 18;
                return false;
            }
            if (up && down && left && !right && !downLeft && !upLeft)
            {
                tile.TileFrameX = (short)((18 * 18) + (randomFrame * 54));
                tile.TileFrameY = 18;
                return false;
            }
            #endregion

            #region X State
            if (up && down && left && right && !downLeft && !downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = (short)((17 * 18) + (randomFrame * 54));
                tile.TileFrameY = 18;
                return false;
            }
            #endregion

            #region Inner Corner x1
            if (up && down && left && right && !downLeft && downRight && upLeft && upRight)
            {
                tile.TileFrameX = 14 * 18;
                tile.TileFrameY = (short)((5 * 18) + (randomFrame * 36));
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && upLeft && upRight)
            {
                tile.TileFrameX = 13 * 18;
                tile.TileFrameY = (short)((5 * 18) + (randomFrame * 36));
                return false;
            }
            if (up && down && left && right && downLeft && downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 14 * 18;
                tile.TileFrameY = (short)((6 * 18) + (randomFrame * 36));
                return false;
            }
            if (up && down && left && right && downLeft && downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 13 * 18;
                tile.TileFrameY = (short)((6 * 18) + (randomFrame * 36));
                return false;
            }
            #endregion

            #region Inner Corner x2 (same side)
            if (up && down && left && right && !downLeft && !downRight && upLeft && upRight)
            {
                tile.TileFrameX = (short)((6 * 18) + (randomFrame * 18));
                tile.TileFrameY = 2 * 18;
                return false;
            }
            if (up && down && left && right && downLeft && downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = (short)((6 * 18) + (randomFrame * 18));
                tile.TileFrameY = 1 * 18;
                return false;
            }
            if (up && down && left && right && !downLeft && downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 10 * 18;
                tile.TileFrameY = (short)(randomFrame * 18);
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 11 * 18;
                tile.TileFrameY = (short)(randomFrame * 18);
                return false;
            }
            #endregion

            #region Inner Corner x2 (opposite corners)
            if (up && down && left && right && !downLeft && downRight && upLeft && !upRight)
            {
                tile.TileFrameX = (short)((10 * 18) + (randomFrame * 18));
                tile.TileFrameY = 4 * 18;
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && !upLeft && upRight)
            {
                tile.TileFrameX = (short)((13 * 18) + (randomFrame * 18));
                tile.TileFrameY = 4 * 18;
                return false;
            }
            #endregion

            #region Inner Corner x3
            if (up && down && left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 15 * 18;
                tile.TileFrameY = (short)((6 * 18) + (randomFrame * 36));
                return false;
            }
            if (up && down && left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 15 * 18;
                tile.TileFrameY = (short)((5 * 18) + (randomFrame * 36));
                return false;
            }
            if (up && down && left && right && !downLeft && !downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 16 * 18;
                tile.TileFrameY = (short)((6 * 18) + (randomFrame * 36));
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 16 * 18;
                tile.TileFrameY = (short)((5 * 18) + (randomFrame * 36));
                return false;
            }
            #endregion

            #region Corner and Side
            if (!up && down && left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = (short)((17 * 18) + (randomFrame * 36));
                tile.TileFrameY = 3 * 18;
                return false;
            }
            if (!up && down && left && right && downLeft && !downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = (short)((16 * 18) + (randomFrame * 36));
                tile.TileFrameY = 3 * 18;
                return false;
            }
            if (up && !down && left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                tile.TileFrameX = (short)((17 * 18) + (randomFrame * 36));
                tile.TileFrameY = 4 * 18;
                return false;
            }
            if (up && !down && left && right && !downLeft && !downRight && upLeft && !upRight)
            {
                tile.TileFrameX = (short)((16 * 18) + (randomFrame * 36));
                tile.TileFrameY = 4 * 18;
                return false;
            }
            if (up && down && !left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 17 * 18;
                tile.TileFrameY = (short)((5 * 18) + (randomFrame * 36));
                return false;
            }
            if (up && down && !left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 17 * 18;
                tile.TileFrameY = (short)((6 * 18) + (randomFrame * 36));
                return false;
            }
            if (up && down && left && !right && !downLeft && !downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 18 * 18;
                tile.TileFrameY = (short)((5 * 18) + (randomFrame * 36));
                return false;
            }
            if (up && down && left && !right && downLeft && !downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 18 * 18;
                tile.TileFrameY = (short)((6 * 18) + (randomFrame * 36));
                return false;
            }
            #endregion

            return true;
        }

        internal static void CompactFraming(int x, int y, bool resetFrame = true)
        {
            if (x < 0 || x >= Main.maxTilesX)
                return;
            if (y < 0 || y >= Main.maxTilesY)
                return;

            Tile tile = Main.tile[x, y];
            if (tile.Slope > 0 && TileID.Sets.HasSlopeFrames[tile.TileType])
            {
                return;
            }

            // Reset the tile's random frame style if the frame is being reset.
            int randomFrame;
            if (resetFrame)
            {
                randomFrame = WorldGen.genRand.Next(3);
                Main.tile[x, y].Get<TileWallWireStateData>().TileFrameNumber = (byte)randomFrame;
            }
            else
            {
                randomFrame = Main.tile[x, y].TileFrameNumber;
            }

            GetAdjacentTiles(x, y, out bool up, out bool down, out bool left, out bool right, out bool upLeft, out bool upRight, out bool downLeft, out bool downRight);

            #region Middle State
            if (up && down && left && right && upLeft && upRight && downLeft && downRight)
            {
                tile.TileFrameX = 18;
                tile.TileFrameY = 18;
                return;
            }
            #endregion

            #region Single State
            if (!up && !down && !left && !right)
            {
                tile.TileFrameX = 54;
                tile.TileFrameY = 54;
                return;
            }
            #endregion

            #region Edges
            if (!up && down && left && right && downLeft && downRight)
            {
                tile.TileFrameX = 18;
                tile.TileFrameY = 0;
                return;
            }
            if (up && down && !left && right && upRight && downRight)
            {
                tile.TileFrameX = 0;
                tile.TileFrameY = 18;
                return;
            }
            if (up && !down && left && right && upLeft && upRight)
            {
                tile.TileFrameX = 18;
                tile.TileFrameY = 36;
                return;
            }
            if (up && down && left && !right && upLeft && downLeft)
            {
                tile.TileFrameX = 36;
                tile.TileFrameY = 18;
                return;
            }
            #endregion

            #region Edge Corners
            if (!up && down && !left && right && downRight)
            {
                tile.TileFrameX = 0;
                tile.TileFrameY = 0;
                return;
            }
            if (!up && down && left && !right && downLeft)
            {
                tile.TileFrameX = 36;
                tile.TileFrameY = 0;
                return;
            }
            if (up && !down && !left && right && upRight)
            {
                tile.TileFrameX = 0;
                tile.TileFrameY = 36;
                return;
            }
            if (up && !down && left && !right && upLeft)
            {
                tile.TileFrameX = 36;
                tile.TileFrameY = 36;
                return;
            }
            #endregion

            #region I States
            if (up && down && !left && !right)
            {
                tile.TileFrameX = 54;
                tile.TileFrameY = 18;
                return;
            }
            if (!up && !down && left && right)
            {
                tile.TileFrameX = 18;
                tile.TileFrameY = 54;
                return;
            }
            #endregion

            #region I End States
            if (!up && down && !left && !right)
            {
                tile.TileFrameX = 54;
                tile.TileFrameY = 0;
                return;
            }
            if (up && !down && !left && !right)
            {
                tile.TileFrameX = 54;
                tile.TileFrameY = 36;
                return;
            }
            if (!up && !down && !left && right)
            {
                tile.TileFrameX = 0;
                tile.TileFrameY = 54;
                return;
            }
            if (!up && !down && left && !right)
            {
                tile.TileFrameX = 36;
                tile.TileFrameY = 54;
                return;
            }
            #endregion

            #region L States
            if (!up && down && !left && right && !downRight)
            {
                tile.TileFrameX = 72;
                tile.TileFrameY = 0;
                return;
            }
            if (!up && down && left && !right && !downLeft)
            {
                tile.TileFrameX = 108;
                tile.TileFrameY = 0;
                return;
            }
            if (up && !down && !left && right && !upRight)
            {
                tile.TileFrameX = 72;
                tile.TileFrameY = 36;
                return;
            }
            if (up && !down && left && !right && !upLeft)
            {
                tile.TileFrameX = 108;
                tile.TileFrameY = 36;
                return;
            }
            #endregion

            #region T States
            if (!up && down && left && right && !downLeft && !downRight)
            {
                tile.TileFrameX = 90;
                tile.TileFrameY = 0;
                return;
            }
            if (up && !down && left && right && !upLeft && !upRight)
            {
                tile.TileFrameX = 90;
                tile.TileFrameY = 36;
                return;
            }
            if (up && down && !left && right && !downRight && !upRight)
            {
                tile.TileFrameX = 72;
                tile.TileFrameY = 18;
                return;
            }
            if (up && down && left && !right && !downLeft && !upLeft)
            {
                tile.TileFrameX = 108;
                tile.TileFrameY = 18;
                return;
            }
            #endregion

            #region X State
            if (up && down && left && right && !downLeft && !downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 90;
                tile.TileFrameY = 18;
                return;
            }
            #endregion

            #region Inner Corner x1
            if (up && down && left && right && !downLeft && downRight && upLeft && upRight)
            {
                tile.TileFrameX = 144;
                tile.TileFrameY = 36;
                return;
            }
            if (up && down && left && right && downLeft && !downRight && upLeft && upRight)
            {
                tile.TileFrameX = 126;
                tile.TileFrameY = 36;
                return;
            }
            if (up && down && left && right && downLeft && downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 144;
                tile.TileFrameY = 54;
                return;
            }
            if (up && down && left && right && downLeft && downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 126;
                tile.TileFrameY = 54;
                return;
            }
            #endregion

            #region Inner Corner x2 (same side)
            if (up && down && left && right && !downLeft && !downRight && upLeft && upRight)
            {
                tile.TileFrameX = 198;
                tile.TileFrameY = 0;
                return;
            }
            if (up && down && left && right && downLeft && downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 198;
                tile.TileFrameY = 18;
                return;
            }
            if (up && down && left && right && !downLeft && downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 198;
                tile.TileFrameY = 36;
                return;
            }
            if (up && down && left && right && downLeft && !downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 198;
                tile.TileFrameY = 54;
                return;
            }
            #endregion

            #region Inner Corner x2 (opposite corners)
            if (up && down && left && right && !downLeft && downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 108;
                tile.TileFrameY = 54;
                return;
            }
            if (up && down && left && right && downLeft && !downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 90;
                tile.TileFrameY = 54;
                return;
            }
            #endregion

            #region Inner Corner x3
            if (up && down && left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 126;
                tile.TileFrameY = 18;
                return;
            }
            if (up && down && left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 126;
                tile.TileFrameY = 0;
                return;
            }
            if (up && down && left && right && !downLeft && !downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 144;
                tile.TileFrameY = 18;
                return;
            }
            if (up && down && left && right && downLeft && !downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 144;
                tile.TileFrameY = 0;
                return;
            }
            #endregion

            #region Corner and Side
            if (!up && down && left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 180;
                tile.TileFrameY = 0;
                return;
            }
            if (!up && down && left && right && downLeft && !downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 162;
                tile.TileFrameY = 0;
                return;
            }
            if (up && !down && left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 180;
                tile.TileFrameY = 18;
                return;
            }
            if (up && !down && left && right && !downLeft && !downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 162;
                tile.TileFrameY = 18;
                return;
            }
            if (up && down && !left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 162;
                tile.TileFrameY = 36;
                return;
            }
            if (up && down && !left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 162;
                tile.TileFrameY = 54;
                return;
            }
            if (up && down && left && !right && !downLeft && !downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 180;
                tile.TileFrameY = 36;
                return;
            }
            if (up && down && left && !right && downLeft && !downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 180;
                tile.TileFrameY = 54;
                return;
            }
            #endregion
        }
        #endregion

        #region Generic Custom Framing Code
        internal static void CustomMergeFrameExplicit(int x, int y, int myType, int mergeType, out bool mergedUp,
            out bool mergedLeft, out bool mergedRight, out bool mergedDown, bool forceSameDown = false,
            bool forceSameUp = false, bool forceSameLeft = false, bool forceSameRight = false, bool resetFrame = true)
        {
            if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY)
            {
                mergedUp = mergedLeft = mergedRight = mergedDown = false;
                return;
            }

            // Disable vanilla trying to merge these tiles automtaically.
            Main.tileMerge[myType][mergeType] = false;

            // These all get null checked in the GetSimilarity and GetMerge functions
            Tile tileLeft = Main.tile[x - 1, y];
            Tile tileRight = Main.tile[x + 1, y];
            Tile tileUp = Main.tile[x, y - 1];
            Tile tileDown = Main.tile[x, y + 1];
            Tile tileTopLeft = Main.tile[x - 1, y - 1];
            Tile tileTopRight = Main.tile[x + 1, y - 1];
            Tile tileBottomLeft = Main.tile[x - 1, y + 1];
            Tile tileBottomRight = Main.tile[x + 1, y + 1];

            // Cardinal directions
            Similarity leftSim = forceSameLeft ? Similarity.Same : GetSimilarity(tileLeft, myType, mergeType);
            Similarity rightSim = forceSameRight ? Similarity.Same : GetSimilarity(tileRight, myType, mergeType);
            Similarity upSim = forceSameUp ? Similarity.Same : GetSimilarity(tileUp, myType, mergeType);
            Similarity downSim = forceSameDown ? Similarity.Same : GetSimilarity(tileDown, myType, mergeType);

            // Diagonal directions
            Similarity topLeftSim = GetSimilarity(tileTopLeft, myType, mergeType);
            Similarity topRightSim = GetSimilarity(tileTopRight, myType, mergeType);
            Similarity bottomLeftSim = GetSimilarity(tileBottomLeft, myType, mergeType);
            Similarity bottomRightSim = GetSimilarity(tileBottomRight, myType, mergeType);

            // Reset the tile's random frame style if the frame is being reset.
            int randomFrame;
            if (resetFrame)
            {
                randomFrame = WorldGen.genRand.Next(3);
                Main.tile[x, y].Get<TileWallWireStateData>().TileFrameNumber = (byte)randomFrame;
            }
            else
            {
                randomFrame = Main.tile[x, y].TileFrameNumber;
            }

            // Initialize all merged variables to false.
            mergedDown = mergedLeft = mergedRight = mergedUp = false;

            #region Custom Merge Conditional Tree
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
            #endregion
        }

        internal static void CustomMergeFrame(int x, int y, int myType, int mergeType, bool forceSameDown = false,
            bool forceSameUp = false, bool forceSameLeft = false, bool forceSameRight = false, bool resetFrame = true)
            => CustomMergeFrameExplicit(x, y, myType, mergeType, out _, out _, out _, out _, forceSameDown, forceSameUp, forceSameLeft, forceSameRight, resetFrame);

        internal static void CustomMergeFrame(int x, int y, int myType, int mergeType)
        {
            if (x < 0 || x >= Main.maxTilesX)
                return;
            if (y < 0 || y >= Main.maxTilesY)
                return;

            bool forceSameUp = false;
            bool forceSameDown = false;
            bool forceSameLeft = false;
            bool forceSameRight = false;

            Tile north = Main.tile[x, y - 1];
            Tile south = Main.tile[x, y + 1];
            Tile west = Main.tile[x - 1, y];
            Tile east = Main.tile[x + 1, y];

            if (north != null && north.HasTile && tileMergeTypes[myType][north.TileType])
            {
                // Register this tile as not automatically merging with the tile above it.
                CalamityUtils.SetMerge(myType, north.TileType, false);
                TileID.Sets.ChecksForMerge[myType] = true;

                // Properly frame the tile given this constraint.
                CustomMergeFrameExplicit(x, y - 1, north.TileType, myType, out _, out _, out _, out forceSameUp, false, false, false, false, false);
            }
            if (west != null && west.HasTile && tileMergeTypes[myType][west.TileType])
            {
                // Register this tile as not automatically merging with the tile to the left of it.
                CalamityUtils.SetMerge(myType, west.TileType, false);
                TileID.Sets.ChecksForMerge[myType] = true;

                // Properly frame the tile given this constraint.
                CustomMergeFrameExplicit(x - 1, y, west.TileType, myType, out _, out _, out forceSameLeft, out _, false, false, false, false, false);
            }
            if (east != null && east.HasTile && tileMergeTypes[myType][east.TileType])
            {
                // Register this tile as not automatically merging with the tile to the right of it.
                CalamityUtils.SetMerge(myType, east.TileType, false);
                TileID.Sets.ChecksForMerge[myType] = true;

                // Properly frame the tile given this constraint.
                CustomMergeFrameExplicit(x + 1, y, east.TileType, myType, out _, out forceSameRight , out _, out _, false, false, false, false, false);
            }
            if (south != null && south.HasTile && tileMergeTypes[myType][south.TileType])
            {
                // Register this tile as not automatically merging with the tile below it.
                CalamityUtils.SetMerge(myType, south.TileType, false);
                TileID.Sets.ChecksForMerge[myType] = true;

                // Properly frame the tile given this constraint.
                CustomMergeFrameExplicit(x, y + 1, south.TileType, myType, out forceSameDown, out _, out _, out _, false, false, false, false, false);
            }

            // With all constraints determined, properly frame the tile a final time.
            CustomMergeFrameExplicit(x, y, myType, mergeType, out _, out _, out _, out _, forceSameDown, forceSameUp, forceSameLeft, forceSameRight, true);
        }
        #endregion
    }
}
