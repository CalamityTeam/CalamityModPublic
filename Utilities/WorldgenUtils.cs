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
                    if (convertibleTiles.Length <= 0 || convertibleTiles.Contains(ParanoidTileRetrieval(tilesX, tilesY).TileType))
                        WorldGen.OreRunner(tilesX, tilesY, WorldGen.genRand.Next(strengthMin, strengthMax), WorldGen.genRand.Next(3, 8), (ushort)type);
                }
            }
        }

        /// <summary>
        /// Settles all liquids in the world.
        /// </summary>
        public static void SettleWater()
        {
            Liquid.worldGenTilesIgnoreWater(true);
            Liquid.QuickWater(3);
            WorldGen.WaterCheck();

            Liquid.quickSettle = true;

            for (int i = 0; i < 10; i++)
            {
                int maxLiquid = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                int m = maxLiquid * 5;
                double maxLiquidDifferencePercentage = 0D;
                while (Liquid.numLiquid > 0)
                {
                    m--;
                    if (m < 0)
                        break;

                    double liquidDifferencePercentage = (maxLiquid - Liquid.numLiquid - LiquidBuffer.numLiquidBuffer) / (double)maxLiquid;
                    if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > maxLiquid)
                        maxLiquid = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                    
                    if (liquidDifferencePercentage > maxLiquidDifferencePercentage)
                        maxLiquidDifferencePercentage = liquidDifferencePercentage;

                    Liquid.UpdateLiquid();
                }
                WorldGen.WaterCheck();
            }
            Liquid.quickSettle = false;
            Liquid.worldGenTilesIgnoreWater(false);
        }
    }
}
