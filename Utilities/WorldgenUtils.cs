using System.Linq;
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
                    if (convertibleTiles.Length <= 0 || convertibleTiles.Contains(ParanoidTileRetrieval(tilesX, tilesY).type))
                        WorldGen.OreRunner(tilesX, tilesY, WorldGen.genRand.Next(strengthMin, strengthMax), WorldGen.genRand.Next(3, 8), (ushort)type);
                }
            }
        }
    }
}
