using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod
{
    public static partial class CalamityUtils
    {
        /// <summary>
        /// Generates clusters of ore across the world based on various requirements and with various strengths/frequencies.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="frequency"></param>
        /// <param name="verticalStartFactor"></param>
        /// <param name="verticalEndFactor"></param>
        /// <param name="strengthMin"></param>
        /// <param name="strengthMax"></param>
        /// <param name="convertibleTiles"></param>
        public static void SpawnOre(int type, double frequency, float verticalStartFactor, float verticalEndFactor, int strengthMin, int strengthMax, params int[] convertibleTiles)
        {
            int x = Main.maxTilesX;
            int y = Main.maxTilesY;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int k = 0; k < (int)(x * y * frequency); k++)
                {
                    int tilesX = WorldGen.genRand.Next(0, x);
                    int tilesY = WorldGen.genRand.Next((int)(y * verticalStartFactor), (int)(y * verticalEndFactor));
                    if (convertibleTiles.Length <= 0 || convertibleTiles.Contains(ParanoidTileRetrieval(tilesX, tilesY).TileType))
                        WorldGen.OreRunner(tilesX, tilesY, WorldGen.genRand.Next(strengthMin, strengthMax), WorldGen.genRand.Next(3, 8), (ushort)type);
                }
            }
        }

        /// <summary>
        /// Generates caves via celluar automata in terms of active/inactive states. May also be used to smoothen out areas.
        /// </summary>
        /// <param name="area">The area to perform the algorithm on.</param>
        /// <param name="gameIterations">The number of times to iterate. The higher this value is, the more smooth the result is.</param>
        public static void GenerateCellularAutomataCave(Rectangle area, int gameIterations)
        {
            bool[,] cellStates = new bool[area.Width, area.Height];
            int width = cellStates.GetLength(0);
            int height = cellStates.GetLength(1);

            // Initialize the cell states.
            for (int i = area.Left; i < area.Right; i++)
            {
                for (int j = area.Top; j < area.Bottom; j++)
                    cellStates[i - area.Left, j - area.Top] = ParanoidTileRetrieval(i, j).HasTile;
            }

            for (int i = 0; i < gameIterations; i++)
            {
                bool[,] newState = cellStates.Clone() as bool[,];
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int nearbyActiveCells = 0;
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                if (x + dx < 0 || x + dx >= width || y + dy < 0 || y + dy >= height)
                                {
                                    nearbyActiveCells++;
                                    continue;
                                }

                                nearbyActiveCells += cellStates[x + dx, y + dy].ToInt();
                            }
                        }

                        newState[x, y] = nearbyActiveCells >= 5;
                    }
                }
                cellStates = newState;
            }

            for (int i = area.Left; i < area.Right; i++)
            {
                for (int j = area.Top; j < area.Bottom; j++)
                    Main.tile[i, j].Get<TileWallWireStateData>().HasTile = cellStates[i - area.Left, j - area.Top];
            }
        }
    }
}
