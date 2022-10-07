using CalamityMod.Items;
using CalamityMod.Items.Accessories;
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

        // Loop variables that are accessed via getter methods should be stored externally in local variables for performance reasons.
        public static readonly List<int> SulphSeaTiles = new()
        {
            ModContent.TileType<SulphurousSand>(),
            ModContent.TileType<SulphurousSandstone>(),
            ModContent.TileType<HardenedSulphurousSandstone>()
        };

        public const float SandBlockEdgeDescentSmoothness = 0.24f;

        // What percentage it takes for dither effects to start appearing at the edges.
        // As an example, if this value is 0.7 that would mean that tiles that are below 70% of the way down from the top of the ocean would
        // start randomly dithering.
        public const float DitherStartFactor = 0.9f;

        public const int DepthForWater = 18;

        public const float TopWaterDepthPercentage = 0.125f;

        public const float TopWaterDescentSmoothnessMin = 0.26f;

        public const float TopWaterDescentSmoothnessMax = 0.39f;

        public const int TotalSandTilesBeforeWaterMin = 32;

        public const int TotalSandTilesBeforeWaterMax = 45;

        // 0-1 value of how jagged the small caves should be. The higher this value is, the more variance you can expect for each step when carving out caves.
        public const float SmallCavesJaggedness = 0.51f;

        // How much of a tendency the small caves have to be cramped instead of large and open, with values between 0-1 emphasizing larger caves while values greater than 1
        // emphasizing more cramped caves.
        public const float SmallCavesBiasTowardsTightness = 2.21f;

        // How much of a magnification is performed when calculating perlin noise for spaghetti caves. The closer to 0 this value is, the more same-y the caves will seem in
        // terms of direction, size, etc.
        public const float SpaghettiCaveMagnification = 0.00193f;

        // 0-1 value that determines the threshold for spaghetti caves being carved out. At 0, no caves are carved out, at 1, all tiles are carved out.
        // This is used in the formula 'abs(noise(x, y)) < r' to determine whether the cave should remove tiles.
        public static readonly float[] SpaghettiCaveCarveOutThresholds = new float[]
        {
            0.033f,
            0.089f
        };

        public const float CheeseCaveMagnification = 0.00237f;
        
        public static readonly float[] CheeseCaveCarveOutThresholds = new float[]
        {
            0.4f
        };

        // Percentage of how far down a tile has to be for open caverns to appear.
        public const float OpenCavernStartDepthPercentage = 0.42f;

        public const float HardenedSandstoneLineMagnification = 0.004f;

        public const int MaxIslandHeight = 16;

        public const int MaxIslandDepth = 9;

        public const float IslandLineMagnification = 0.0079f;

        public const int TreeGrowChance = 5;

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

        public static int MinCaveWidth => (int)(Main.maxTilesX / 2500f);

        public static int MaxCaveWidth => (int)Math.Ceiling(Main.maxTilesX / 566f);

        public static int MinCaveMovementSteps => (int)Math.Ceiling(Main.maxTilesX / 70f);

        public static int MaxCaveMovementSteps => (int)Math.Ceiling(Main.maxTilesX / 40f);

        public static int YStart
        {
            get;
            set;
        }

        // Vines cannot grow any higher than this. This is done to prevent vines from growing very close to the sea surface.
        public static int VineGrowTopLimit => YStart + 100;
        #endregion

        #region Placement Methods
        public static void PlaceSulphurSea()
        {
            Abyss.AtLeftSideOfWorld = Main.dungeonX < Main.maxTilesX / 2;
            DetermineYStart();
            GenerateSandBlock();
            RemoveStupidTilesAboveSea();
            GenerateShallowTopWater();

            // Cave generation. Some of these borrow concepts and tricks used by Minecraft's new generation.
            GenerateSmallWaterCaverns();
            GenerateSpaghettiWaterCaves();
            GenerateCheeseWaterCaves();
            ClearOutStrayTiles();

            // Lay down decorations after the caves are generated.
            ClearAloneTiles();
            DecideHardSandstoneLine();
            MakeSurfaceLessRigid();
            LayTreesOnSurface();
        }

        public static void FinishGeneratingSulphurSea()
        {
            CreateBeachNearSea();
        }
        #endregion

        #region Generation Functions
        public static void GenerateSandBlock()
        {
            int width = BiomeWidth + 1;
            int maxDepth = BlockDepth;
            ushort blockTileType = (ushort)ModContent.TileType<SulphurousSand>();
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
                        Main.tile[x, y].TileType = blockTileType;

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
            int width = (int)((BiomeWidth - totalSandTilesBeforeWater) * 0.42f);
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
                            new Actions.SetLiquid()
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
                                new Actions.SetLiquid()
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
                        if (noise - biasToNegativeOneInterpolant > CheeseCaveCarveOutThresholds[c])
                        {
                            WorldUtils.Gen(new(x, y), new Shapes.Rectangle(1, 1), Actions.Chain(new GenAction[]
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

        public static void ClearOutStrayTiles()
        {
            int width = BiomeWidth;
            int depth = BlockDepth;
            ushort blockTileType = (ushort)ModContent.TileType<SulphurousSand>();
            ushort wallID = (ushort)ModContent.WallType<SulphurousSandWall>();

            void recursivelyGetAttachedPoints(int x, int y, List<Point> points)
            {
                Tile t = CalamityUtils.ParanoidTileRetrieval(x, y);
                Point p = new(x, y);
                if (t.TileType != blockTileType || !t.HasTile || points.Count > 300 || points.Contains(p))
                    return;

                points.Add(p);

                recursivelyGetAttachedPoints(x + 1, y, points);
                recursivelyGetAttachedPoints(x - 1, y, points);
                recursivelyGetAttachedPoints(x, y + 1, points);
                recursivelyGetAttachedPoints(x, y - 1, points);
            }


            // Clear out stray chunks created by caverns.
            for (int i = 1; i < width; i++)
            {
                int x = GetActualX(i);
                for (int y = YStart; y < YStart + depth; y++)
                {
                    List<Point> chunkPoints = new();
                    recursivelyGetAttachedPoints(x, y, chunkPoints);

                    int cutoffLimit = y >= YStart + depth * OpenCavernStartDepthPercentage ? 300 : 50;
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

        public static void ClearAloneTiles()
        {
            int width = BiomeWidth;
            int depth = BlockDepth;
            ushort blockTileType = (ushort)ModContent.TileType<SulphurousSand>();

            for (int i = 0; i < width; i++)
            {
                int x = GetActualX(i);
                for (int y = YStart; y < YStart + depth; y++)
                {
                    Tile t = CalamityUtils.ParanoidTileRetrieval(x, y);
                    if (!t.HasTile || t.TileType != blockTileType)
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
                            new Actions.PlaceWall(MathHelper.Distance(dy, heightOffset) >= 2f ? wallID : WallID.None, true),
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
                if (!WorldGen.genRand.NextBool(TreeGrowChance))
                    continue;

                int x = GetActualX(i);
                int y = YStart - 30;
                if (!WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Down(MaxIslandDepth + 31), new Conditions.IsSolid()), out Point growPoint))
                    continue;

                x = growPoint.X;
                y = growPoint.Y - 1;

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
            WorldUtils.Find(new Point(BiomeWidth + 4, (int)WorldGen.worldSurfaceLow - 20), searchCondition, out Point determinedPoint);
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
                int depth = (int)(Math.Sin((1f - xRatio) * MathHelper.PiOver2) * 45f + 1f);
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
                        Main.tile[x, y].TileType = (ushort)ModContent.TileType<SulphurousSand>();
                }
            }

            // Plant new trees.
            for (int x = BiomeWidth - 10; x <= BiomeWidth + beachWidth; x++)
            {
                int trueX = Abyss.AtLeftSideOfWorld ? x : Main.maxTilesX - x;
                if (!WorldGen.genRand.NextBool(10))
                    continue;

                int y = YStart - 30;
                if (!WorldUtils.Find(new Point(trueX, y), Searches.Chain(new Searches.Down(100), new Conditions.IsTile((ushort)ModContent.TileType<SulphurousSand>())), out Point treePlantPosition))
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
        #endregion
    }
}
