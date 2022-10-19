using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Schematics;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Walls;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.RGB;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityMod.World
{
    public class SulphurousSea
    {
        #region Fields and Properties

        public const float SandBlockEdgeDescentSmoothness = 0.24f;

        // What percentage it takes for dither effects to start appearing at the edges.
        // As an example, if this value is 0.7 that would mean that tiles that are below 70% of the way down from the top of the ocean would
        // start randomly dithering.
        public const float DitherStartFactor = 0.9f;

        public const int DepthForWater = 12;

        public const float TopWaterDepthPercentage = 0.125f;

        public const float TopWaterDescentSmoothnessMin = 0.26f;

        public const float TopWaterDescentSmoothnessMax = 0.39f;

        public const int TotalSandTilesBeforeWaterMin = 32;

        public const int TotalSandTilesBeforeWaterMax = 45;

        public const float OpenSeaWidthPercentage = 0.53f;

        public const float IslandWidthPercentage = 0.36f;

        public const float IslandCurvatureSharpness = 0.74f;

        // 0-1 value of how jagged the small caves should be. The higher this value is, the more variance you can expect for each step when carving out caves.
        public const float SmallCavesJaggedness = 0.51f;

        // How much of a tendency the small caves have to be cramped instead of large and open, with values between 0-1 emphasizing larger caves while values greater than 1
        // emphasizing more cramped caves.
        public const float SmallCavesBiasTowardsTightness = 2.21f;

        // How much of a magnification is performed when calculating perlin noise for spaghetti caves. The closer to 0 this value is, the more same-y the caves will seem in
        // terms of direction, size, etc.
        public const float SpaghettiCaveMagnification = 0.00193f;

        // 0-1 value that determines the threshold for spaghetti caves being carved out. At 0, no tiles are carved out, at 1, all tiles are carved out.
        // This is used in the formula 'abs(noise(x, y)) < r' to determine whether the cave should remove tiles.
        public static readonly float[] SpaghettiCaveCarveOutThresholds = new float[]
        {
            0.033f,
            0.089f
        };

        public const float CheeseCaveMagnification = 0.00237f;
        
        public static readonly float[] CheeseCaveCarveOutThresholds = new float[]
        {
            0.32f
        };

        // Percentage of how far down a tile has to be for open caverns to appear.
        public const float OpenCavernStartDepthPercentage = 0.42f;

        // The percentage of tiles on average that should be transformed into water.
        // A value of 1 indicates that every tile should have water.
        // This value should be close to 1, but not exactly, so that when water settles the top of caverns will be open.
        public const float WaterSpreadPercentage = 0.91f;

        public const float HardenedSandstoneLineMagnification = 0.004f;

        public const int MaxIslandHeight = 16;

        public const int MaxIslandDepth = 9;

        public const float IslandLineMagnification = 0.0079f;

        public const int TreeGrowChance = 5;

        public const int MinColumnHeight = 5;

        public const int MaxColumnHeight = 50;

        public const int BeachMaxDepth = 50;

        public const int ScrapPileAnticlumpDistance = 80;

        public const float SandstoneEdgeNoiseMagnification = 0.00115f;

        public const int StalactitePairMinDistance = 6;
        
        public const int StalactitePairMaxDistance = 44;

        // Loop variables that are accessed via getter methods should be stored externally in local variables for performance reasons.
        public static int BiomeWidth
        {
            get
            {
                return Main.maxTilesX switch
                {
                    // Small worlds.
                    4200 => 370,

                    // Medium worlds.
                    6400 => 445,

                    // Large worlds. This also accounts for worlds of an unknown size, such as extra large worlds.
                    _ => (int)(Main.maxTilesX / 16.8f),
                };
            }
        }

        public static int BlockDepth
        {
            get
            {
                float depthFactor = Main.maxTilesX switch
                {
                    // Small worlds.
                    4200 => 0.8f,

                    // Medium worlds.
                    6400 => 0.85f,

                    // Large worlds.
                    _ => 0.925f
                };
                return (int)((Main.rockLayer + 112 - YStart) * depthFactor);
            }
        }

        public static int TotalCavesInShallowWater => (int)Math.Ceiling(Main.maxTilesX / 2000f);

        public static int MaxTopWaterDepth => (int)(BlockDepth * TopWaterDepthPercentage);

        public static int MinCaveWidth => Main.maxTilesX / 2500;

        public static int MaxCaveWidth => (int)Math.Ceiling(Main.maxTilesX / 566f);

        public static int MinCaveMovementSteps => (int)Math.Ceiling(Main.maxTilesX / 70f);

        public static int MaxCaveMovementSteps => (int)Math.Ceiling(Main.maxTilesX / 40f);

        public static int ColumnCount => Main.maxTilesX / 96;

        public static int GeyserCount => Main.maxTilesX / 137;

        public static int YStart
        {
            get;
            set;
        }

        public static readonly List<int> SulphSeaTiles = new()
        {
            ModContent.TileType<SulphurousSand>(),
            ModContent.TileType<SulphurousSandstone>(),
            ModContent.TileType<HardenedSulphurousSandstone>()
        };

        // Vines cannot grow any higher than this. This is done to prevent vines from growing very close to the sea surface.
        public static int VineGrowTopLimit => YStart + 100;
        #endregion

        #region Placement Methods
        public static void PlaceSulphurSea()
        {
            // Determine which side the abyss is such that it's at the same side as the dungeon.
            Abyss.AtLeftSideOfWorld = Main.dungeonX < Main.maxTilesX / 2;

            // Settle the foundation for the sea. This involves creating the base sulphurous sea block, old tile cleanup, and creating water at the surface.
            DetermineYStart();
            GenerateSandBlock();
            RemoveStupidTilesAboveSea();
            GenerateShallowTopWater();
            GenerateIsland();

            // Cave generation. Some of these borrow concepts and tricks used by Minecraft's new generation.
            GenerateSmallWaterCaverns();
            GenerateSpaghettiWaterCaves();
            GenerateCheeseWaterCaves();

            // Lay down decorations and post-processing effects after the caves are generated.
            DecideHardSandstoneLine();
            MakeSurfaceLessRigid();
            LayTreesOnSurface();
        }
        
        public static void SulphurSeaGenerationAfterAbyss()
        {
            CreateBeachNearSea();
            ClearOutStrayTiles();
            ClearAloneTiles();
            var scrapPilePositions = PlaceScrapPiles();
            GenerateColumnsInCaverns();
            GenerateSteamGeysersInCaverns();
            GenerateHardenedSandstone();
            PlaceStalactites();
            GenerateChests(scrapPilePositions);
        }
        #endregion

        #region Generation Functions
        public static void DetermineYStart()
        {
            int xCheckPosition = GetActualX(BiomeWidth + 1);
            var searchCondition = Searches.Chain(new Searches.Down(3000), new Conditions.IsSolid());
            Point determinedPoint;

            do
            {
                WorldUtils.Find(new Point(xCheckPosition, (int)WorldGen.worldSurfaceLow - 20), searchCondition, out determinedPoint);
                xCheckPosition += Abyss.AtLeftSideOfWorld.ToDirectionInt();
            }
            while (CalamityUtils.ParanoidTileRetrieval(determinedPoint.X, determinedPoint.Y).TileType == TileID.Ebonstone);
            YStart = determinedPoint.Y;
        }

        public static void GenerateSandBlock()
        {
            int width = BiomeWidth + 1;
            int maxDepth = BlockDepth;
            ushort blockTileType = (ushort)ModContent.TileType<SulphurousSand>();
            ushort wallID = (ushort)ModContent.WallType<SulphurousSandWall>();

            for (int i = 1; i < width; i++)
            {
                int x = GetActualX(i);

                // Calculate the 0-1 factor that determines how far down a vertical strip of the sea should descend.
                float depthFactor = (float)Math.Pow(Math.Sin((1f - i / (float)width) * MathHelper.PiOver2), SandBlockEdgeDescentSmoothness);

                // Determine the top and botton of the strip.
                int top = YStart;
                int bottom = top + (int)(maxDepth * depthFactor);
                for (int y = top; y < bottom; y++)
                {
                    float ditherChance = CalculateDitherChance(width, top, bottom, i, y);
                    if (WorldGen.genRand.NextFloat() >= ditherChance)
                    {
                        Main.tile[x, y].TileType = blockTileType;
                        if (y >= top + 45)
                            Main.tile[x, y].WallType = wallID;
                    }

                    // Ensure that the sand pops into existence if there is no chance that dithering will occur.
                    // This doesn't happen if there is dithering to ensure that there aren't stray sand tiles in the middle of open
                    // caves that have nothing to connect to.
                    if (ditherChance <= 0f)
                    {
                        Main.tile[x, y].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                        Main.tile[x, y].Get<TileWallWireStateData>().IsHalfBlock = false;
                        Main.tile[x, y].Get<TileWallWireStateData>().HasTile = true;
                    }
                }

                // Obliterate old palm trees.
                for (int y = top - 75; y < top + 50; y++)
                {
                    if (Main.tile[x, y].TileType == TileID.PalmTree)
                        WorldGen.KillTile(x, y);
                }
            }
        }

        public static void RemoveStupidTilesAboveSea()
        {
            for (int i = 0; i < BiomeWidth; i++)
            {
                int x = GetActualX(i);
                for (int y = YStart - 140; y < YStart + 80; y++)
                {
                    int type = CalamityUtils.ParanoidTileRetrieval(x, y).TileType;
                    if (YStartWhitelist.Contains(type) ||
                        OtherTilesForSulphSeaToDestroy.Contains(type))
                        CalamityUtils.ParanoidTileRetrieval(x, y).Get<TileWallWireStateData>().HasTile = false;
                    if (WallsForSulphSeaToDestroy.Contains(CalamityUtils.ParanoidTileRetrieval(x, y).WallType))
                        CalamityUtils.ParanoidTileRetrieval(x, y).WallType = 0;
                }
            }
        }

        public static void GenerateShallowTopWater()
        {
            int maxDepth = MaxTopWaterDepth;
            int totalSandTilesBeforeWater = WorldGen.genRand.Next(TotalSandTilesBeforeWaterMin, TotalSandTilesBeforeWaterMax);
            int width = (int)((BiomeWidth - totalSandTilesBeforeWater) * OpenSeaWidthPercentage);
            float descentSmoothness = WorldGen.genRand.NextFloat(TopWaterDescentSmoothnessMin, TopWaterDescentSmoothnessMax);
            
            for (int i = 1; i < width; i++)
            {
                int x = GetActualX(i);

                // Calculate the 0-1 factor that determines how far down a vertical strip of water should descend.
                float depthFactor = (float)Math.Pow(Math.Sin((1f - i / (float)width) * MathHelper.PiOver2), descentSmoothness);

                // Determine the top and botton of the water strip.
                int top = YStart;
                int bottom = top + (int)(maxDepth * depthFactor);
                for (int y = top; y < bottom; y++)
                {
                    if (y >= top + DepthForWater)
                        Main.tile[x, y].LiquidAmount = byte.MaxValue;
                    Main.tile[x, y].Get<TileWallWireStateData>().HasTile = false;
                }

                // Clear water that's above the level for some reason.
                for (int y = top - 150; y < top + DepthForWater; y++)
                    Main.tile[x, y].LiquidAmount = 0;
            }
        }

        public static void GenerateIsland()
        {
            int left = -32;
            int right = (int)(BiomeWidth * IslandWidthPercentage);
            int maxDepth = MaxTopWaterDepth;
            ushort blockTileType = (ushort)ModContent.TileType<SulphurousSand>();
            ushort wallID = (ushort)ModContent.WallType<SulphurousSandWall>();

            for (int i = left; i < right; i++)
            {
                if (i < 0)
                    continue;

                // Generate a bump at the edge of the ocean as an island.
                // The inside of this is by default solid but cave generation can sometimes create a really cool "hollow" look.
                int x = GetActualX(i);
                float xCompletionRatio = Utils.GetLerpValue(left, right, i, true);
                int islandHeight = (int)Math.Round(Math.Pow(CalamityUtils.Convert01To010(xCompletionRatio), IslandCurvatureSharpness) * (maxDepth + 4f));
                for (int dy = -30; dy <= islandHeight; dy++)
                {
                    int y = YStart + maxDepth - dy;
                    Main.tile[x, y].TileType = blockTileType;

                    if (dy < islandHeight)
                        Main.tile[x, y].WallType = wallID;
                    Main.tile[x, y].LiquidAmount = byte.MaxValue;
                    Main.tile[x, y].Get<TileWallWireStateData>().HasTile = true;
                }
            }
        }

        public static void GenerateSmallWaterCaverns()
        {
            int width = BiomeWidth;
            int depth = BlockDepth;
            int shallowWaterCaveCount = TotalCavesInShallowWater;
            int minCaveWidth = MinCaveWidth;
            int maxCaveWidth = MaxCaveWidth;
            ushort wallID = (ushort)ModContent.WallType<SulphurousSandWall>();

            for (int i = 0; i < shallowWaterCaveCount; i++)
            {
                // Initialize variables for the cave.
                int caveHorizontalOffset = WorldGen.genRand.Next((int)(width * 0.1f));
                int x = GetActualX((int)MathHelper.Lerp(width * 0.8f, width * 0.24f, i / (float)(shallowWaterCaveCount - 1f)) + caveHorizontalOffset);
                int caveWidth = (minCaveWidth + maxCaveWidth) / 3 + WorldGen.genRand.Next(8);
                int caveSteps = WorldGen.genRand.Next(MinCaveMovementSteps, MaxCaveMovementSteps);
                int caveSeed = WorldGen.genRand.Next();
                Vector2 baseCaveDirection = Vector2.UnitY.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);
                Vector2 cavePosition = new(x, YStart + MaxTopWaterDepth + DepthForWater / 2);

                for (int j = 0; j < caveSteps; j++)
                {
                    // Make the cave direction turn based on perlin noise.
                    float caveOffsetAngleAtStep = CalamityUtils.PerlinNoise2D(i / 50f, j / 50f, 4, caveSeed) * MathHelper.Pi * 1.9f;
                    Vector2 caveDirection = baseCaveDirection.RotatedBy(caveOffsetAngleAtStep);

                    // Carve out at the current position.
                    if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                    {
                        WorldGen.digTunnel(cavePosition.X, cavePosition.Y, caveDirection.X, caveDirection.Y, 1, (int)(caveWidth * 1.18f), true);
                        WorldUtils.Gen(cavePosition.ToPoint(), new Shapes.Circle(caveWidth), Actions.Chain(new GenAction[]
                        {
                            new Actions.ClearTile(true),
                            new Actions.PlaceWall(wallID, true),
                            new Actions.SetLiquid(LiquidID.Water, (byte)(WorldGen.genRand.NextFloat() > WaterSpreadPercentage ? 0 : byte.MaxValue))
                        }));
                    }

                    // Update the cave position.
                    cavePosition += caveDirection * caveWidth;

                    // Turn back if the cave position is close to leaving the sea.
                    if (GetActualX((int)cavePosition.X) > width * 0.8f || cavePosition.Y > YStart + depth * 0.7f)
                    {
                        cavePosition.X -= Abyss.AtLeftSideOfWorld.ToDirectionInt() * caveWidth;
                        cavePosition.Y -= caveWidth;
                    }

                    // Update the cave width for the next update step.
                    float caveWidthFactorInterpolant = (float)Math.Pow(WorldGen.genRand.NextFloat(), SmallCavesBiasTowardsTightness);
                    caveWidth = (int)Math.Round(caveWidth * MathHelper.Lerp(1f - SmallCavesJaggedness, 1f + SmallCavesJaggedness, caveWidthFactorInterpolant));
                    if (WorldGen.genRand.NextBool(12))
                        caveWidth = (int)(caveWidth * 1.4f);

                    caveWidth = Utils.Clamp(caveWidth, minCaveWidth, maxCaveWidth);
                }
            }
        }

        public static void GenerateSpaghettiWaterCaves()
        {
            int width = BiomeWidth;
            int depth = (int)(BlockDepth * 0.96f);
            ushort wallID = (ushort)ModContent.WallType<SulphurousSandWall>();

            for (int c = 0; c < SpaghettiCaveCarveOutThresholds.Length; c++)
            {
                int caveSeed = WorldGen.genRand.Next();
                for (int i = 1; i < width; i++)
                {
                    // Initialize variables for the cave.
                    int x = GetActualX(i);
                    for (int y = YStart; y < YStart + depth; y++)
                    {
                        float noise = FractalBrownianMotion(i * SpaghettiCaveMagnification, y * SpaghettiCaveMagnification, caveSeed, 5);

                        // Bias noise away from 0, effectively making caves less likely to appear, based on how close it is to the bottom/horizontal edge.
                        // This causes caves to naturally stop as the reach ends instead of abruptly stopping like in the old Sulphurous Sea worldgen.
                        float distanceFromEdge = new Vector2(i / (float)width, (y - YStart) / (float)depth).Length();
                        float biasAwayFrom0Interpolant = Utils.GetLerpValue(0.82f, 0.96f, distanceFromEdge * 0.8f, true);
                        biasAwayFrom0Interpolant += Utils.GetLerpValue(YStart + 12f, YStart, y, true) * 0.2f;
                        biasAwayFrom0Interpolant += Utils.GetLerpValue(width - 19f, width - 4f, i, true) * 0.6f;

                        // If the noise is less than 0, bias to -1, if it's greater than 0, bias away to 1.
                        // This is done instead of biasing to -1 or 1 without exception to ensure that in doing so the noise does not cross into the
                        // cutout threshold near 0 as it interpolates.
                        noise = MathHelper.Lerp(noise, Math.Sign(noise), biasAwayFrom0Interpolant);

                        if (Math.Abs(noise) < SpaghettiCaveCarveOutThresholds[c])
                        {
                            WorldUtils.Gen(new(x, y), new Shapes.Rectangle(1, 1), Actions.Chain(new GenAction[]
                            {
                                new Actions.ClearTile(true),
                                new Actions.PlaceWall(wallID, true),
                                new Actions.SetLiquid(LiquidID.Water, (byte)(WorldGen.genRand.NextFloat() > WaterSpreadPercentage ? 0 : byte.MaxValue))
                            }));
                        }
                    }
                }
            }
        }

        public static void GenerateCheeseWaterCaves()
        {
            int width = BiomeWidth;
            int depth = (int)(BlockDepth * 0.96f);
            ushort wallID = (ushort)ModContent.WallType<SulphurousSandWall>();

            for (int c = 0; c < CheeseCaveCarveOutThresholds.Length; c++)
            {
                int caveSeed = WorldGen.genRand.Next();
                for (int i = 1; i < width; i++)
                {
                    // Initialize variables for the cave.
                    int x = GetActualX(i);
                    for (int y = YStart; y < YStart + depth; y++)
                    {
                        float noise = FractalBrownianMotion(i * CheeseCaveMagnification, y * CheeseCaveMagnification, caveSeed, 6);
                        float distanceFromEdge = new Vector2(i / (float)width, (y - YStart) / (float)depth).Length();

                        float biasToNegativeOneInterpolant = Utils.GetLerpValue(0.82f, 0.96f, distanceFromEdge * 0.8f, true);
                        biasToNegativeOneInterpolant += Utils.GetLerpValue(YStart + OpenCavernStartDepthPercentage * depth, YStart + OpenCavernStartDepthPercentage * depth - 25f, y, true);
                        biasToNegativeOneInterpolant += Utils.GetLerpValue(width - 19f, width - 4f, i, true);
                        if (noise - biasToNegativeOneInterpolant > CheeseCaveCarveOutThresholds[c])
                        {
                            WorldUtils.Gen(new(x, y), new Shapes.Rectangle(1, 1), Actions.Chain(new GenAction[]
                            {
                                new Actions.ClearTile(true),
                                new Actions.PlaceWall(wallID, true),
                                new Actions.SetLiquid(LiquidID.Water, (byte)(WorldGen.genRand.NextFloat() > WaterSpreadPercentage ? 0 : byte.MaxValue))
                            }));
                        }
                    }
                }
            }
        }

        public static void ClearOutStrayTiles()
        {
            int width = BiomeWidth;
            int depth = BlockDepth;
            List<ushort> blockTileTypes = new()
            {
                (ushort)ModContent.TileType<SulphurousSand>(),
                (ushort)ModContent.TileType<SulphurousSandstone>(),
                (ushort)ModContent.TileType<HardenedSulphurousSandstone>(),
            };
            ushort wallID = (ushort)ModContent.WallType<SulphurousSandWall>();
            
            void getAttachedPoints(int x, int y, List<Point> points)
            {
                Tile t = CalamityUtils.ParanoidTileRetrieval(x, y);
                Point p = new(x, y);
                if (!blockTileTypes.Contains(t.TileType) || !t.HasTile || points.Count > 432 || points.Contains(p) || CalculateDitherChance(width, YStart, YStart + depth, x, y) > 0f)
                    return;

                points.Add(p);

                getAttachedPoints(x + 1, y, points);
                getAttachedPoints(x - 1, y, points);
                getAttachedPoints(x, y + 1, points);
                getAttachedPoints(x, y - 1, points);
            }

            // Clear out stray chunks created by caverns.
            for (int i = 1; i < width; i++)
            {
                int x = GetActualX(i);
                for (int y = YStart; y < YStart + depth; y++)
                {
                    List<Point> chunkPoints = new();
                    getAttachedPoints(x, y, chunkPoints);

                    int cutoffLimit = y >= YStart + depth * OpenCavernStartDepthPercentage ? 432 : 50;
                    if (chunkPoints.Count >= 2 && chunkPoints.Count < cutoffLimit)
                    {
                        foreach (Point p in chunkPoints)
                        {
                            WorldUtils.Gen(p, new Shapes.Rectangle(1, 1), Actions.Chain(new GenAction[]
                            {
                                new Actions.ClearTile(true),
                                new Actions.PlaceWall(wallID, true),
                                new Actions.SetLiquid()
                            }));
                        }
                    }
                }
            }
        }

        public static void DecideHardSandstoneLine()
        {
            int width = BiomeWidth;
            int depth = BlockDepth;

            int sandstoneSeed = WorldGen.genRand.Next();
            ushort blockTypeToReplace = (ushort)ModContent.TileType<SulphurousSand>();
            ushort blockTypeToPlace = (ushort)ModContent.TileType<HardenedSulphurousSandstone>();
            ushort wallID = (ushort)ModContent.WallType<HardenedSulphurousSandstoneWall>();

            for (int i = 0; i < width; i++)
            {
                for (int y = YStart; y < YStart + depth; y++)
                {
                    int sandstoneLineOffset = (int)(FractalBrownianMotion(i * HardenedSandstoneLineMagnification, y * HardenedSandstoneLineMagnification, sandstoneSeed, 7) * 30) + (int)(depth * OpenCavernStartDepthPercentage);

                    // Make the sandstone line descent a little bit the closer it is to the world edge, to make it look like it "warps" towards the abyss.
                    sandstoneLineOffset -= (int)(Math.Pow(Utils.GetLerpValue(width * 0.1f, width * 0.8f, i, true), 1.72f) * 67f);

                    Point p = new(GetActualX(i), y);
                    Tile t = CalamityUtils.ParanoidTileRetrieval(p.X, p.Y);
                    if (y >= YStart + sandstoneLineOffset && t.HasTile && t.TileType == blockTypeToReplace)
                    {
                        WorldUtils.Gen(p, new Shapes.Rectangle(1, 1), Actions.Chain(new GenAction[]
                        {
                            new Actions.SetTile(blockTypeToPlace, true),
                            new Actions.PlaceWall(wallID, true),
                            new Actions.SetLiquid()
                        }));
                    }
                }
            }
        }

        public static void MakeSurfaceLessRigid()
        {
            int y = YStart;
            int width = BiomeWidth;
            int heightSeed = WorldGen.genRand.Next();
            ushort blockTileType = (ushort)ModContent.TileType<SulphurousSand>();
            ushort wallID = (ushort)ModContent.WallType<HardenedSulphurousSandstoneWall>();

            for (int i = 0; i < width; i++)
            {
                int x = GetActualX(i);
                Tile t = CalamityUtils.ParanoidTileRetrieval(x, y);

                // If the tile below is solid, then determine how high it should rise upward.
                // This is done to make the surface less unnaturally flat.
                if (t.HasTile)
                {
                    float noise = FractalBrownianMotion(i * IslandLineMagnification, y * IslandLineMagnification, heightSeed, 5) * 0.5f + 0.5f;
                    noise = MathHelper.Lerp(noise, 0.5f, Utils.GetLerpValue(width - 13f, width - 1f, i, true));

                    int heightOffset = -(int)Math.Round(MathHelper.Lerp(-MaxIslandDepth, MaxIslandHeight, noise));
                    for (int dy = 0; dy != heightOffset; dy += Math.Sign(heightOffset))
                    {
                        WorldUtils.Gen(new(x, y + dy), new Shapes.Rectangle(1, 1), Actions.Chain(new GenAction[]
                        {
                            heightOffset > 0 ? new Actions.ClearTile() : new Actions.SetTile(blockTileType, true),
                            new Actions.PlaceWall(MathHelper.Distance(dy, heightOffset) >= 2f && heightOffset < 0f ? wallID : WallID.None, true),
                            new Actions.SetLiquid()
                        }));
                    }
                }
            }
        }

        public static void LayTreesOnSurface()
        {
            int width = BiomeWidth;
            
            for (int i = 0; i < width - 8; i++)
            {
                // Only sometimes generate trees.
                if (!WorldGen.genRand.NextBool(TreeGrowChance))
                    continue;

                int x = GetActualX(i);
                int y = YStart - 30;

                // Search downward in hopes of finding a position to generate and grow an acorn.
                // If no such downward tile exists, skip this tile.
                if (!WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Down(MaxIslandDepth + 31), new Conditions.IsSolid()), out Point growPoint))
                    continue;

                x = growPoint.X;
                y = growPoint.Y - 1;

                // Ignore tiles if there's water above.
                if (CalamityUtils.ParanoidTileRetrieval(x, y).LiquidAmount > 0)
                    continue;

                Main.tile[x, y].TileType = TileID.Saplings;
                Main.tile[x, y].Get<TileWallWireStateData>().HasTile = true;
                if (!WorldGen.GrowPalmTree(x, y))
                    WorldGen.KillTile(x, y);
            }
        }

        public static void CreateBeachNearSea()
        {
            int beachWidth = WorldGen.genRand.Next(150, 190 + 1);
            var searchCondition = Searches.Chain(new Searches.Down(3000), new Conditions.IsSolid());
            ushort sandID = (ushort)ModContent.TileType<SulphurousSand>();

            // Stop immediately if for some strange reason a valid tile could not be located for the beach starting point.
            if (!WorldUtils.Find(new Point(BiomeWidth + 4, (int)WorldGen.worldSurfaceLow - 20), searchCondition, out Point determinedPoint))
                return;

            Tile tileAtEdge = CalamityUtils.ParanoidTileRetrieval(determinedPoint.X, determinedPoint.Y);
            
            // Extend outward to encompass some of the desert, if there is one.
            if (tileAtEdge.TileType is TileID.Sand or TileID.Ebonsand or TileID.Crimsand)
                beachWidth += 85;

            // Transform the landscape.
            for (int i = BiomeWidth - 10; i <= BiomeWidth + beachWidth; i++)
            {
                int x = GetActualX(i);
                float xRatio = Utils.GetLerpValue(BiomeWidth - 10, BiomeWidth + beachWidth, i, true);
                float ditherChance = Utils.GetLerpValue(0.92f, 0.99f, xRatio, true);
                int depth = (int)(Math.Sin((1f - xRatio) * MathHelper.PiOver2) * BeachMaxDepth + 1f);
                for (int y = YStart - 50; y < YStart + depth; y++)
                {
                    Tile tileAtPosition = CalamityUtils.ParanoidTileRetrieval(x, y);
                    if (tileAtPosition.HasTile && ValidBeachDestroyTiles.Contains(tileAtPosition.TileType))
                    {
                        // Kill trees manually so that no leftover tiles are present.
                        if (Main.tile[x, y].TileType == TileID.Trees)
                            WorldGen.KillTile(x, y);
                        else
                            Main.tile[x, y].Get<TileWallWireStateData>().HasTile = false;
                    }

                    else if (tileAtPosition.HasTile && ValidBeachCovertTiles.Contains(tileAtPosition.TileType) && WorldGen.genRand.NextFloat() >= ditherChance)
                        Main.tile[x, y].TileType = sandID;
                }
            }

            // Plant new trees.
            for (int x = BiomeWidth - 10; x <= BiomeWidth + beachWidth; x++)
            {
                int trueX = Abyss.AtLeftSideOfWorld ? x : Main.maxTilesX - x;
                if (!WorldGen.genRand.NextBool(10))
                    continue;

                int y = YStart - 30;
                if (!WorldUtils.Find(new Point(trueX, y), Searches.Chain(new Searches.Down(100), new Conditions.IsTile(sandID)), out Point treePlantPosition))
                    continue;

                treePlantPosition.Y--;

                // Place the saplings and try to grow them.
                WorldGen.PlaceTile(treePlantPosition.X, treePlantPosition.Y, ModContent.TileType<AcidWoodTreeSapling>());
                Main.tile[treePlantPosition].TileType = TileID.Saplings;
                Main.tile[treePlantPosition].Get<TileWallWireStateData>().HasTile = true;
                if (!WorldGen.GrowPalmTree(treePlantPosition.X, treePlantPosition.Y))
                    WorldGen.KillTile(treePlantPosition.X, treePlantPosition.Y);
            }
        }

        public static void ClearAloneTiles()
        {
            int width = BiomeWidth;
            int depth = BlockDepth;
            List<ushort> blockTileTypes = new()
            {
                (ushort)ModContent.TileType<SulphurousSand>(),
                (ushort)ModContent.TileType<SulphurousSandstone>(),
                (ushort)ModContent.TileType<HardenedSulphurousSandstone>(),
            };

            for (int i = 0; i < width; i++)
            {
                int x = GetActualX(i);
                for (int y = YStart; y < YStart + depth; y++)
                {
                    Tile t = CalamityUtils.ParanoidTileRetrieval(x, y);
                    if (!t.HasTile || !blockTileTypes.Contains(t.TileType))
                        continue;

                    // Check to see if the tile has any cardinal neighbors. If it doesn't, destroy it.
                    if (!CalamityUtils.ParanoidTileRetrieval(x - 1, y).HasTile &&
                        !CalamityUtils.ParanoidTileRetrieval(x + 1, y).HasTile &&
                        !CalamityUtils.ParanoidTileRetrieval(x, y - 1).HasTile &&
                        !CalamityUtils.ParanoidTileRetrieval(x, y + 1).HasTile)
                    {
                        WorldUtils.Gen(new(x, y), new Shapes.Rectangle(1, 1), Actions.Chain(new GenAction[]
                        {
                            new Actions.ClearTile(true),
                            new Actions.ClearWall(true),
                            new Actions.SetLiquid()
                        }));
                    }
                }
            }
        }

        public static List<Vector2> PlaceScrapPiles()
        {
            int tries = 0;
            List<Vector2> pastPlacementPositions = new List<Vector2>();
            for (int i = 0; i < 3; i++)
            {
                tries++;
                if (tries > 20000)
                    continue;

                int x = GetActualX(WorldGen.genRand.Next(75, BiomeWidth - 85));
                int y = WorldGen.genRand.Next(YStart + (int)(BlockDepth * 0.3f), YStart + (int)(BlockDepth * 0.8f));

                Point pilePlacementPosition = new Point(x, y);

                // If the selected position is sitting inside of a tile, try again.
                if (WorldGen.SolidTile(pilePlacementPosition.X, pilePlacementPosition.Y))
                {
                    i--;
                    continue;
                }

                // If the selected position is close to other piles, try again.
                if (pastPlacementPositions.Any(p => Vector2.Distance(p, pilePlacementPosition.ToVector2()) < ScrapPileAnticlumpDistance))
                {
                    i--;
                    continue;
                }

                // Otherwise, decide which pile should be created.
                int pileVariant = WorldGen.genRand.Next(7);
                string schematicName = $"Sulphurous Scrap {pileVariant + 1}";
                Vector2? wrappedSchematicArea = SchematicManager.GetSchematicArea(schematicName);

                // Create a log message if for some reason the schematic in question doesn't exist.
                if (!wrappedSchematicArea.HasValue)
                {
                    CalamityMod.Instance.Logger.Warn($"Tried to place a schematic with name \"{schematicName}\". No matching schematic file found.");
                    continue;
                }

                Vector2 schematicArea = wrappedSchematicArea.Value;

                // Decide the placement position by searching downward and looking for the lowest point.
                // If the position is quite steep, try again.
                Vector2 left = pilePlacementPosition.ToVector2() - Vector2.UnitX * schematicArea.X * 0.5f;
                Vector2 right = pilePlacementPosition.ToVector2() + Vector2.UnitX * schematicArea.X * 0.5f;
                while (!WorldGen.SolidTile(CalamityUtils.ParanoidTileRetrieval((int)left.X, (int)left.Y)))
                    left.Y++;
                while (!WorldGen.SolidTile(CalamityUtils.ParanoidTileRetrieval((int)right.X, (int)right.Y)))
                    right.Y++;

                if (Math.Abs(left.Y - right.Y) >= 20f)
                {
                    i--;
                    continue;
                }

                // If the placement position ended up in the abyss, try again.
                if (left.Y >= YStart + BlockDepth - 50 || right.Y >= YStart + BlockDepth - 50)
                {
                    i--;
                    continue;
                }

                // Pick the lowest point vertically.
                Point bottomCenter = new Point(pilePlacementPosition.X, (int)Math.Max(left.Y, right.Y) + 6);
                bool _ = false;
                SchematicManager.PlaceSchematic<Action<Chest>>(schematicName, bottomCenter, SchematicAnchor.BottomCenter, ref _);

                pastPlacementPositions.Add(bottomCenter.ToVector2());
                tries = 0;
            }
            return pastPlacementPositions;
        }

        public static void GenerateColumnsInCaverns()
        {
            int columnCount = ColumnCount;
            int width = BiomeWidth;
            int depth = BlockDepth;
            var searchCondition = Searches.Chain(new Searches.Up(MaxColumnHeight), new Conditions.IsSolid());

            for (int c = 0; c < columnCount; c++)
            {
                int x = GetActualX(WorldGen.genRand.Next(20, width - 32));
                int y = WorldGen.genRand.Next(YStart, YStart + depth - 30);

                bool tryAgain = false;

                // Try again if inside a tile.
                Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);
                Tile right = CalamityUtils.ParanoidTileRetrieval(x + 1, y);
                Tile bottom = CalamityUtils.ParanoidTileRetrieval(x, y + 1);
                Tile bottomRight = CalamityUtils.ParanoidTileRetrieval(x + 1, y + 1);
                if (tile.HasTile || right.HasTile)
                    tryAgain = true;

                // Try again if there is no bottom tile.
                if (!WorldGen.SolidTile(bottom) || !WorldGen.SolidTile(bottomRight))
                    tryAgain = true;

                // Try again if there is no tile above or the ceiling is not level.
                if (!WorldUtils.Find(new(x, y), searchCondition, out Point top) ||
                    !WorldUtils.Find(new(x + 1, y), searchCondition, out Point topRight) || top.Y != topRight.Y)
                {
                    tryAgain = true;
                }

                // Try again if the distance between the top and bottom is too short.
                if (MathHelper.Distance(y, top.Y) < MinColumnHeight)
                    tryAgain = true;

                if (tryAgain)
                {
                    c--;
                    continue;
                }

                GenerateColumn(x, top.Y, y);
            }
        }

        public static void GenerateSteamGeysersInCaverns()
        {
            int geyserCount = GeyserCount;
            int width = BiomeWidth;
            int depth = BlockDepth;
            ushort geyserID = (ushort)ModContent.TileType<SteamGeyser>();

            for (int g = 0; g < geyserCount; g++)
            {
                int x = GetActualX(WorldGen.genRand.Next(20, width - 32));
                int y = WorldGen.genRand.Next(YStart + depth / 2, YStart + depth - 42);

                bool tryAgain = false;

                // Try again if inside a tile.
                for (int dx = 0; dx < 2; dx++)
                {
                    for (int dy = 0; dy < 2; dy++)
                    {
                        Tile tile = CalamityUtils.ParanoidTileRetrieval(x + dx, y - dy);
                        if (tile.HasTile)
                            tryAgain = true;
                    }
                }

                // Try again if there is no ground.
                for (int dx = 0; dx < 2; dx++)
                {
                    Tile tile = CalamityUtils.ParanoidTileRetrieval(x + dx, y + 1);
                    if (!WorldGen.SolidTile(tile))
                        tryAgain = true;
                }

                if (tryAgain)
                {
                    g--;
                    continue;
                }

                WorldGen.PlacePot(x, y, geyserID);
            }
        }

        public static void GenerateHardenedSandstone()
        {
            int sandstoneSeed = WorldGen.genRand.Next();
            ushort sandstoneID = (ushort)ModContent.TileType<SulphurousSandstone>();
            ushort sandstoneWallID = (ushort)ModContent.WallType<SulphurousSandstoneWall>();

            // Edge score evaluation function that determines the propensity a tile has to become sandstone.
            // This is based on how much nearby empty areas there are, allowing for "edges" to appear.
            static int getEdgeScore(int x, int y)
            {
                int edgeScore = 0;
                for (int dx = x - 6; dx <= x + 6; dx++)
                {
                    if (dx == x)
                        continue;

                    if (!CalamityUtils.ParanoidTileRetrieval(dx, y).HasTile)
                        edgeScore++;
                }
                for (int dy = y - 6; dy <= y + 6; dy++)
                {
                    if (dy == y)
                        continue;

                    if (!CalamityUtils.ParanoidTileRetrieval(x, dy).HasTile)
                        edgeScore++;
                }
                return edgeScore;
            }

            for (int i = 1; i < BiomeWidth; i++)
            {
                for (int y = YStart; y <= YStart + BlockDepth; y++)
                {
                    int x = GetActualX(i);
                    float sandstoneConvertChance = FractalBrownianMotion(i * SandstoneEdgeNoiseMagnification, y * SandstoneEdgeNoiseMagnification, sandstoneSeed, 7) * 0.5f + 0.5f;

                    // Make the sandstone appearance chance dependant on the edge score.
                    sandstoneConvertChance *= Utils.GetLerpValue(4f, 11f, getEdgeScore(x, y), true);

                    // Make sandstone less likely to appear on the surface.
                    sandstoneConvertChance *= Utils.GetLerpValue(YStart + 30f, YStart + 54f, y, true);

                    if (WorldGen.genRand.NextFloat() > sandstoneConvertChance || sandstoneConvertChance < 0.5f)
                        continue;

                    // Convert to sandstone as necessary.
                    for (int dx = -2; dx <= 2; dx++)
                    {
                        for (int dy = -2; dy <= 2; dy++)
                        {
                            if (WorldGen.InWorld(x + dx, y + dy))
                            {
                                if (CalamityUtils.ParanoidTileRetrieval(x + dx, y + dy).TileType != sandstoneID &&
                                    SulphSeaTiles.Contains(CalamityUtils.ParanoidTileRetrieval(x + dx, y + dy).TileType))
                                {
                                    Main.tile[x + dx, y + dy].WallType = sandstoneWallID;
                                    Main.tile[x + dx, y + dy].TileType = sandstoneID;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void PlaceStalactites()
        {
            static int heightFromType(int type)
            {
                if (type <= 2)
                    return 2;
                else if (type <= 4)
                    return 3;
                else
                    return 4;
            };
            
            for (int i = 1; i < BiomeWidth; i++)
            {
                int x = GetActualX(i);
                for (int y = YStart; y <= YStart + BlockDepth; y++)
                {
                    if (y - YStart > BlockDepth * 0.25f)
                    {
                        if (WorldGen.SolidTile(x, y - 1) && WorldGen.genRand.NextBool(10))
                        {
                            int dy = 1;
                            while (!CalamityUtils.ParanoidTileRetrieval(x, y + dy).HasTile)
                            {
                                dy++;
                                if (dy > StalactitePairMaxDistance)
                                    break;
                            }
                            if (dy <= StalactitePairMaxDistance && dy >= StalactitePairMinDistance)
                            {
                                int type = WorldGen.genRand.Next(6);
                                type++;
                                int height = heightFromType(type);

                                PlaceStalactite(x, y, height, CalamityMod.Instance.Find<ModTile>($"SulphurousStalactite{type}").Type);
                                if (WorldGen.SolidTile(x, y + dy + 1))
                                    PlaceStalacmite(x, y + dy, height, CalamityMod.Instance.Find<ModTile>($"SulphurousStalacmite{type}").Type);
                            }
                        }
                    }
                }
            }
        }

        public static void GenerateChests(List<Vector2> scrapPilePositions)
        {
            GenerateTreasureChest();
            CalamityUtils.SettleWater();
            GenerateOpenAirChestChest();
            GenerateScrapPileChest(scrapPilePositions);
            GenerateDeepWaterChest();
        }

        public static void GenerateTreasureChest()
        {
            // Generate on chest below the island to the edge as buried treasure.
            static bool tryToGenerateTreasureChest(Point chestPoint)
            {
                WorldUtils.Find(chestPoint, Searches.Chain(new Searches.Down(300), new Conditions.IsSolid()), out Point p);
                chestPoint = p;

                // Determine how far down the island chest should generate.
                int minDepth = 32;
                int digDepth = 0;
                Point startingIslandChestPoint = chestPoint;
                while (true)
                {
                    Tile down = CalamityUtils.ParanoidTileRetrieval(chestPoint.X, chestPoint.Y + digDepth);
                    Tile downRight = CalamityUtils.ParanoidTileRetrieval(chestPoint.X + 1, chestPoint.Y + digDepth);

                    // Stop if far down enough and either the tile to the left or right is open water.
                    if (((!down.HasTile && down.LiquidAmount >= 127) || (!downRight.HasTile && downRight.LiquidAmount >= 127)) && digDepth >= minDepth)
                        break;

                    digDepth++;
                    if (digDepth >= 80)
                        return false;
                }
                chestPoint.Y += digDepth - 12;

                // Check the nearby area and ensure that it's not exposed to air. The treasure should be buried.
                bool nearbyAreaIsClosed = false;
                while (!nearbyAreaIsClosed)
                {
                    nearbyAreaIsClosed = true;
                    for (int dx = -2; dx < 4; dx++)
                    {
                        for (int dy = -1; dy < 3; dy++)
                        {
                            if (!Main.tile[chestPoint.X + dx, chestPoint.Y - dy].HasTile)
                                nearbyAreaIsClosed = false;
                        }
                    }

                    if (!nearbyAreaIsClosed)
                        chestPoint.Y++;
                }

                // Dig up a bit and place the chest.
                for (int dx = 0; dx < 2; dx++)
                {
                    for (int dy = 0; dy < 2; dy++)
                    {
                        Main.tile[chestPoint.X + dx, chestPoint.Y - dy].LiquidAmount = 0;
                        Main.tile[chestPoint.X + dx, chestPoint.Y - dy].WallType = (ushort)ModContent.WallType<SulphurousSandWallSafe>();
                        Main.tile[chestPoint.X + dx, chestPoint.Y - dy].Get<TileWallWireStateData>().HasTile = false;
                    }
                }

                // If a buried chest was placed, force its first item to be the effigy of decay.
                Chest chest = MiscWorldgenRoutines.AddChestWithLoot(chestPoint.X + 1, chestPoint.Y + 1, (ushort)ModContent.TileType<RustyChestTile>());
                if (chest != null)
                {
                    chest.item[0].SetDefaults(ModContent.ItemType<EffigyOfDecay>());
                    chest.item[0].Prefix(-1);
                }
                else
                    return false;

                // Go back to the surface and leave a little bit of sulphurous sandstone instead of sand as a small marker of the treasure.
                for (int dx = 0; dx < 2; dx++)
                {
                    for (int dy = -1; dy < 3; dy++)
                    {
                        // Ensure that palm trees and pots are not transformed.
                        int oldTileType = Main.tile[startingIslandChestPoint.X + dx, startingIslandChestPoint.Y + dy].TileType;
                        if (oldTileType == TileID.PalmTree || !Main.tileSolid[oldTileType])
                        {
                            WorldGen.KillTile(startingIslandChestPoint.X + dx, startingIslandChestPoint.Y + dy);
                            continue;
                        }

                        Main.tile[startingIslandChestPoint.X + dx, startingIslandChestPoint.Y + dy].LiquidAmount = 0;
                        Main.tile[startingIslandChestPoint.X + dx, startingIslandChestPoint.Y + dy].TileType = (ushort)ModContent.TileType<SulphurousSandstone>();
                    }
                }
                return true;
            }

            Point islandChestPoint = new(GetActualX((int)(BiomeWidth * IslandWidthPercentage * 0.5f) + WorldGen.genRand.Next(-8, 9)), YStart - 100);
            while (!tryToGenerateTreasureChest(islandChestPoint))
                islandChestPoint.X += Abyss.AtLeftSideOfWorld.ToDirectionInt();
        }

        public static void GenerateOpenAirChestChest()
        {
            int width = BiomeWidth;
            Dictionary<int, int> depthMap = new();

            for (int i = 60; i < width - 50; i++)
            {
                int x = GetActualX(i);
                int y = YStart + MaxIslandDepth + 2;
                int dy = 0;

                while (CalamityUtils.ParanoidTileRetrieval(x, y + dy).HasTile || CalamityUtils.ParanoidTileRetrieval(x, y + dy).LiquidAmount <= 0)
                    dy++;

                depthMap[x] = CalamityUtils.ParanoidTileRetrieval(x, y).HasTile ? y + dy : 0;
            }

            // Pick a smooth place on the depth map to place the chest. This should happen close to an open air point in the caves.
            for (int i = 0; i < 400; i++)
            {
                int x = depthMap.Keys.ElementAt(WorldGen.genRand.Next(10, depthMap.Count - 10));
                int leftY = depthMap[x - 1];
                int currentY = depthMap[x];
                int rightY = depthMap[x + 1];
                int averageY = (leftY + currentY + rightY) / 3;

                if (Math.Abs(averageY - currentY) < 3f && currentY > 0)
                {
                    currentY += 3;

                    // Ignore the current position if the chest cannot be placed due to tiles in the way.
                    if (CalamityUtils.AnySolidTileInSelection(x, currentY - 1, 4, -4))
                        continue;

                    // Place the chest and ground.
                    for (int dx = -1; dx < 3; dx++)
                    {
                        Main.tile[x + dx, currentY + 1].LiquidAmount = 0;
                        Main.tile[x + dx, currentY + 1].TileType = (ushort)ModContent.TileType<SulphurousSand>();
                        Main.tile[x + dx, currentY + 1].Get<TileWallWireStateData>().HasTile = true;
                    }

                    // If a buried chest was placed, force its first item to be the broken water filter.
                    Chest chest = MiscWorldgenRoutines.AddChestWithLoot(x, currentY - 2, (ushort)ModContent.TileType<RustyChestTile>());
                    if (chest != null)
                    {
                        chest.item[0].SetDefaults(ModContent.ItemType<BrokenWaterFilter>());
                        chest.item[0].Prefix(-1);
                        break;
                    }
                    else
                        continue;
                }
            }
        }

        public static void GenerateScrapPileChest(List<Vector2> scrapPilePositions)
        {
            // Pick a random scrap pile to generate near.
            for (int i = 0; i < 400; i++)
            {
                Point placeToGenerateNear = WorldGen.genRand.Next(scrapPilePositions).ToPoint();
                int x = placeToGenerateNear.X + WorldGen.genRand.Next(-25 - i / 15, 25 + i / 15);
                int y = placeToGenerateNear.Y + WorldGen.genRand.Next(-16, 4);
                if (WorldGen.SolidTile(x, y))
                    continue;

                // If a buried chest was successfully placed, force its first item to be the rusty beacon prototype.
                Chest chest = MiscWorldgenRoutines.AddChestWithLoot(x, y, (ushort)ModContent.TileType<RustyChestTile>());
                if (chest != null)
                {
                    chest.item[0].SetDefaults(ModContent.ItemType<RustyBeaconPrototype>());
                    chest.item[0].Prefix(-1);
                    break;
                }
            }
        }

        public static void GenerateDeepWaterChest()
        {
            // Pick a random scrap pile to generate near.
            for (int i = 0; i < 400; i++)
            {
                int x = GetActualX(WorldGen.genRand.Next(60, BiomeWidth - 60));
                int y = YStart + WorldGen.genRand.Next(BlockDepth - 150, BlockDepth - 45);
                if (WorldGen.SolidTile(x, y))
                    continue;

                // Try again if too far down.
                while (y < Main.maxTilesY - 210)
                {
                    if (!WorldGen.SolidTile(x, y))
                        y++;
                    else
                    {
                        y -= 3;
                        break;
                    }
                }
                if (y >= YStart + BlockDepth - 45)
                    continue;

                // If a buried chest was successfully placed, force its first item to be the rusty medallion.
                Chest chest = MiscWorldgenRoutines.AddChestWithLoot(x, y, (ushort)ModContent.TileType<RustyChestTile>());
                if (chest != null)
                {
                    chest.item[0].SetDefaults(ModContent.ItemType<RustyMedallion>());
                    chest.item[0].Prefix(-1);
                    break;
                }
            }
        }
        #endregion Generation Functions

        #region Misc Functions
        public static readonly List<int> YStartWhitelist = new()
        {
            TileID.Stone,
            TileID.Dirt,
            TileID.Sand,
            TileID.Ebonsand,
            TileID.Crimsand,
            TileID.Grass,
            TileID.CorruptGrass,
            TileID.CrimsonGrass,
            TileID.ClayBlock,
            TileID.Mud,
            TileID.Copper,
            TileID.Tin,
            TileID.Iron,
            TileID.Lead,
            TileID.Silver,
            TileID.Tungsten,
            TileID.Crimstone,
            TileID.Ebonstone,
            TileID.HardenedSand,
            TileID.CorruptHardenedSand,
            TileID.CrimsonHardenedSand,
            TileID.Coral,
            TileID.BeachPiles,
            TileID.Plants,
            TileID.Plants2,
            TileID.SmallPiles,
            TileID.LargePiles,
            TileID.LargePiles2,
            TileID.Trees,
            TileID.Vines,
            TileID.CorruptThorns,
            TileID.CrimsonThorns,
            TileID.CrimsonVines,
            TileID.Containers,
            TileID.DyePlants,
            TileID.JungleGrass, // Yes, this can happen on rare occasion.
            TileID.SeaOats
        };

        public static readonly List<int> OtherTilesForSulphSeaToDestroy = new()
        {
            TileID.PalmTree,
            TileID.Sunflower,
            TileID.CorruptThorns,
            TileID.CrimsonThorns,
            TileID.CorruptGrass,
            TileID.CorruptPlants,
            TileID.Stalactite,
            TileID.ImmatureHerbs,
            TileID.MatureHerbs,
            TileID.Pots,
            TileID.Pumpkins, // Happens during Halloween
            TileID.FallenLog,
            TileID.LilyPad,
            TileID.VanityTreeSakura,
            TileID.VanityTreeYellowWillow,
            TileID.ShellPile
        };

        public static readonly List<int> WallsForSulphSeaToDestroy = new()
        {
            WallID.Dirt,
            WallID.DirtUnsafe,
            WallID.DirtUnsafe1,
            WallID.DirtUnsafe2,
            WallID.DirtUnsafe3,
            WallID.DirtUnsafe4,
            WallID.Cave6Unsafe, // Rocky dirt wall
            WallID.Grass,
            WallID.GrassUnsafe,
            WallID.Flower,
            WallID.FlowerUnsafe,
            WallID.CorruptGrassUnsafe,
            WallID.EbonstoneUnsafe,
            WallID.CrimstoneUnsafe,
        };

        public static readonly List<int> ValidBeachCovertTiles = new()
        {
            TileID.Dirt,
            TileID.Stone,
            TileID.Crimstone,
            TileID.Ebonstone,
            TileID.Sand,
            TileID.Ebonsand,
            TileID.Crimsand,
            TileID.Grass,
            TileID.CorruptGrass,
            TileID.CrimsonGrass,
            TileID.ClayBlock,
            TileID.Mud,
        };

        public static readonly List<int> ValidBeachDestroyTiles = new()
        {
            TileID.Coral,
            TileID.BeachPiles,
            TileID.Plants,
            TileID.Plants2,
            TileID.SmallPiles,
            TileID.LargePiles,
            TileID.LargePiles2,
            TileID.CorruptThorns,
            TileID.CrimsonThorns,
            TileID.DyePlants,
            TileID.Trees,
            TileID.Sunflower,
            TileID.LilyPad,
            TileID.SeaOats,
            TileID.ImmatureHerbs,
            TileID.MatureHerbs,
            TileID.BloomingHerbs,
            TileID.VanityTreeSakura,
            TileID.VanityTreeYellowWillow,
        };

        // This method is an involutory function, meaning that applying it to the same number twice always yields the original number.
        public static int GetActualX(int x)
        {
            if (Abyss.AtLeftSideOfWorld)
                return x;

            return Main.maxTilesX - x;
        }

        public static float CalculateDitherChance(int width, int top, int bottom, int x, int y)
        {
            float verticalCompletion = Utils.GetLerpValue(top, bottom, y, true);
            float horizontalDitherChance = Utils.GetLerpValue(DitherStartFactor, 1f, x / (float)width, true);
            float verticalDitherChance = Utils.GetLerpValue(DitherStartFactor, 1f, verticalCompletion, true);
            float ditherChance = horizontalDitherChance + verticalDitherChance;
            if (ditherChance > 1f)
                ditherChance = 1f;

            // Make the dither chance fizzle out at low vertical completion values.
            // This is done so that there isn't dithering on the surface of the sea.
            ditherChance -= Utils.GetLerpValue(0.56f, 0.5f, verticalCompletion, true);
            if (ditherChance < 0f)
                ditherChance = 0f;
            return ditherChance;
        }

        public static float FractalBrownianMotion(float x, float y, int seed, int octaves, float gain = 0.5f, float lacunarity = 2f)
        {
            float result = 0f;
            float frequency = 1f;
            float amplitude = 0.5f;
            x += seed * 0.00489937f % 10f;

            for (int i = 0; i < octaves; i++)
            {
                float noise = NoiseHelper.GetStaticNoise(new Vector2(x, y) * frequency) * 2f - 1f;
                result += noise * amplitude;
                amplitude *= gain;
                frequency *= lacunarity;
            }

            return result;
        }

        public static void GenerateColumn(int left, int top, int bottom)
        {
            int depth = BlockDepth;
            ushort columnID = (ushort)ModContent.TileType<SulphurousColumn>();
            ushort hardenedSandstoneWallID = (ushort)ModContent.WallType<HardenedSulphurousSandstoneWall>();
            ushort sandWallID = (ushort)ModContent.WallType<SulphurousSandWall>();
            short variantFrameOffset = (short)(WorldGen.genRand.Next(3) * 36);

            for (int x = left; x < left + 2; x++)
            {
                for (int y = top; y <= bottom; y++)
                {
                    short frameX = (short)((x - left) * 18 + variantFrameOffset);

                    // Use the top frame if at the top, bottom frame if at the bottom, and the middle frame otherwise.
                    short frameY = 18;
                    if (y == top)
                        frameY = 0;
                    else if (y == bottom)
                        frameY = 36;

                    Main.tile[x, y].TileType = columnID;
                    Main.tile[x, y].TileFrameX = frameX;
                    Main.tile[x, y].TileFrameY = frameY;
                    Main.tile[x, y].WallType = y >= YStart + depth * OpenCavernStartDepthPercentage ? hardenedSandstoneWallID : sandWallID;
                    Main.tile[x, y].Get<TileWallWireStateData>().HasTile = true;
                }
            }
        }

        public static void PlaceStalactite(int x, int y, int height, ushort type)
        {
            for (int dy = 0; dy < height; dy++)
            {
                ushort oldWall = Main.tile[x, y + dy].WallType;
                Main.tile[x, y + dy].ClearEverything();
                Main.tile[x, y + dy].WallType = oldWall;
                Main.tile[x, y + dy].TileType = type;
                Main.tile[x, y + dy].TileFrameY = (short)(dy * 18);
                Main.tile[x, y + dy].Get<TileWallWireStateData>().HasTile = true;
            }
        }
        
        public static void PlaceStalacmite(int x, int y, int height, ushort type)
        {
            for (int dy = height - 1; dy > 0; dy--)
            {
                ushort oldWall = Main.tile[x, y + dy].WallType;
                Main.tile[x, y - dy].ClearEverything();
                Main.tile[x, y - dy].WallType = oldWall;
                Main.tile[x, y - dy].TileType = type;
                Main.tile[x, y - dy].TileFrameY = (short)(height * 18 - dy * 18);
                Main.tile[x, y - dy].Get<TileWallWireStateData>().HasTile = true;
            }
        }
        #endregion
    }
}
