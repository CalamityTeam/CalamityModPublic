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

        public static void PlaceShrines()
        {
            int x = Main.maxTilesX;
            int y = Main.maxTilesY;
            int genLimit = x / 2;
            int generateBack = genLimit - 80; //Small = 2020
            int generateForward = genLimit + 80; //Small = 2180
			double shrineChance = 100E-05;

			for (int k = 0; k < (int)(x * y * shrineChance); k++) //Surface Shrine
            {
                int tilesX = WorldGen.genRand.Next((int)(x * 0.35), generateBack);
                int tilesX2 = WorldGen.genRand.Next(generateForward, (int)(x * 0.65));
                int tilesY = WorldGen.genRand.Next((int)(y * 0.3f), (int)(y * 0.35f));

                if (Main.tile[tilesX, tilesY].type == TileID.Dirt || Main.tile[tilesX, tilesY].type == TileID.Stone)
                {
                    WorldGenerationMethods.SpecialHut(TileID.RedBrick, TileID.Dirt, WallID.RedBrick, 0, tilesX, tilesY);
                    break;
                }
                if (Main.tile[tilesX2, tilesY].type == TileID.Dirt || Main.tile[tilesX2, tilesY].type == TileID.Stone)
                {
                    WorldGenerationMethods.SpecialHut(TileID.RedBrick, TileID.Dirt, WallID.RedBrick, 0, tilesX2, tilesY);
                    break;
                }
            }

            for (int k = 0; k < (int)(x * y * shrineChance); k++) //Evil Shrine
            {
                int tilesX = WorldGen.genRand.Next(0, x);
                int tilesY = WorldGen.genRand.Next((int)(y * 0.3f), (int)(y * 0.35f));

                if (Main.tile[tilesX, tilesY].type == (WorldGen.crimson ? TileID.Crimstone : TileID.Ebonstone))
                {
                    WorldGenerationMethods.SpecialHut(WorldGen.crimson ? TileID.CrimtaneBrick : TileID.DemoniteBrick,
                        WorldGen.crimson ? TileID.Crimstone : TileID.Ebonstone,
                        WorldGen.crimson ? WallID.CrimtaneBrick : WallID.DemoniteBrick, 1, tilesX, tilesY);
                    break;
                }
            }

            for (int k = 0; k < (int)(x * y * shrineChance); k++) //Cavern Shrine
            {
                int tilesX = WorldGen.genRand.Next((int)(x * 0.3), generateBack);
                int tilesX2 = WorldGen.genRand.Next(generateForward, (int)(x * 0.7));
                int tilesY = WorldGen.genRand.Next((int)(y * 0.55f), (int)(y * 0.8f));

                if (Main.tile[tilesX, tilesY].type == TileID.Stone)
                {
                    WorldGenerationMethods.SpecialHut(TileID.ObsidianBrick, TileID.Obsidian, WallID.ObsidianBrick, 2, tilesX, tilesY);
                    break;
                }
                if (Main.tile[tilesX2, tilesY].type == TileID.Stone)
                {
                    WorldGenerationMethods.SpecialHut(TileID.ObsidianBrick, TileID.Obsidian, WallID.ObsidianBrick, 2, tilesX2, tilesY);
                    break;
                }
            }

            for (int k = 0; k < (int)(x * y * shrineChance); k++) //Ice Shrine
            {
                int tilesX = WorldGen.genRand.Next(0, x);
                int tilesY = WorldGen.genRand.Next((int)(y * 0.35f), (int)(y * 0.5f));

                if (Main.tile[tilesX, tilesY].type == TileID.IceBlock)
                {
                    WorldGenerationMethods.SpecialHut(TileID.IceBrick, TileID.IceBlock, WallID.IceBrick, 3, tilesX, tilesY);
                    break;
                }
            }

            for (int k = 0; k < (int)(x * y * shrineChance); k++) //Desert Shrine
            {
                int tilesX = WorldGen.genRand.Next(0, x);
                int tilesY = WorldGen.genRand.Next((int)(y * 0.3f), (int)(y * 0.5f));

                if (Main.tile[tilesX, tilesY].type == TileID.DesertFossil)
                {
                    WorldGenerationMethods.SpecialHut(TileID.DesertFossil, TileID.Sandstone, WallID.DesertFossil, 4, tilesX, tilesY);
                    break;
                }
            }

            for (int k = 0; k < (int)(x * y * shrineChance); k++) //Mushroom Shrine
            {
                int tilesX = WorldGen.genRand.Next(0, x);
                int tilesY = WorldGen.genRand.Next((int)(y * 0.35f), (int)(y * 0.5f));

                if (Main.tile[tilesX, tilesY].type == TileID.MushroomGrass)
                {
                    WorldGenerationMethods.SpecialHut(TileID.MushroomBlock, TileID.Mud, WallID.MushroomUnsafe, 5, tilesX, tilesY);
                    break;
                }
            }

            for (int k = 0; k < (int)(x * y * shrineChance); k++) //Granite Shrine
            {
                int tilesX = WorldGen.genRand.Next(0, x);
                int tilesY = WorldGen.genRand.Next((int)(y * 0.35f), (int)(y * 0.5f));

                if (Main.tile[tilesX, tilesY].type == TileID.Granite)
                {
                    WorldGenerationMethods.SpecialHut(TileID.GraniteBlock, TileID.Granite, WallID.GraniteUnsafe, 6, tilesX, tilesY);
                    break;
                }
            }

            for (int k = 0; k < (int)(x * y * shrineChance); k++) //Marble Shrine
            {
                int tilesX = WorldGen.genRand.Next(0, x);
                int tilesY = WorldGen.genRand.Next((int)(y * 0.35f), (int)(y * 0.5f));

                if (Main.tile[tilesX, tilesY].type == TileID.Marble)
                {
                    WorldGenerationMethods.SpecialHut(TileID.MarbleBlock, TileID.Marble, WallID.MarbleUnsafe, 7, tilesX, tilesY);
                    break;
                }
            }
        }
    }
}
