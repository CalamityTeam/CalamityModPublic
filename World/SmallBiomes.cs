using CalamityMod.World.Planets;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.World
{
    public class SmallBiomes : ModWorld
    {
        public static void PlaceEvilIsland()
        {
            int x = Main.maxTilesX;
            int xIslandGen;
            int yIslandGen;
            Rectangle potentialArea;

            do
            {
                xIslandGen = WorldGen.crimson ?
                    WorldGen.genRand.Next((int)(x * 0.1), (int)(x * 0.15)) :
                    WorldGen.genRand.Next((int)(x * 0.85), (int)(x * 0.9));
                yIslandGen = WorldGen.genRand.Next(90, 151);
                yIslandGen = Math.Min(yIslandGen, (int)WorldGen.worldSurfaceLow - 50);

                int checkAreaX = 200;
                int checkAreaY = 200;
                potentialArea = new Rectangle(xIslandGen + checkAreaX / 2, yIslandGen + checkAreaY / 2, checkAreaX, checkAreaY);
            }
            while (!Planetoid.InvalidSkyPlacementArea(potentialArea));

            int tileXLookup = xIslandGen;
            if (WorldGen.crimson)
            {
                while (Main.tile[tileXLookup, yIslandGen].active())
                    tileXLookup++;
            }
            else
            {
                while (Main.tile[tileXLookup, yIslandGen].active())
                    tileXLookup--;
            }

            xIslandGen = tileXLookup;
            CalamityWorld.fehX = xIslandGen;
            CalamityWorld.fehY = yIslandGen;
            WorldGenerationMethods.EvilIsland(xIslandGen, yIslandGen);
            WorldGenerationMethods.EvilIslandHouse(CalamityWorld.fehX, CalamityWorld.fehY);
        }
    }
}
