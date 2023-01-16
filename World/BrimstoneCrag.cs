using CalamityMod.Schematics;
using static CalamityMod.Schematics.SchematicManager;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.Ores;
using CalamityMod.Walls;
using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.WorldBuilding;
using Terraria.GameContent.Generation;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CalamityMod.World
{
    public class BrimstoneCrag
    {
        //the 25 is there to offset it from the edge of the world so no "out of bounds" crashing occurs
        static int StartX = WorldGen.dungeonX < Main.maxTilesX / 2 ? 25 : (Main.maxTilesX - (Main.maxTilesX / 6)) - 25;

        static int lavaLakePlaceDelay = 0;
        static int lavaLakeBigPlaceDelay = 0;

        //basic terrain (sorry for the excessive amount of loops lmao)
        private static void CragTerrain()
        {
            StartX = WorldGen.dungeonX < Main.maxTilesX / 2 ? 25 : (Main.maxTilesX - (Main.maxTilesX / 6)) - 25;

            int biomeStart = StartX;
            int biomeEdge = biomeStart + (Main.maxTilesX / 6);
            int biomeMiddle = (biomeStart + biomeEdge) / 2;

            //clear all blocks and lava in the area
            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                for (int y = Main.maxTilesY - 200; y < Main.maxTilesY - 2; y++)
                {
                    Tile tile = Main.tile[x, y];
                    tile.ClearEverything();
                    WorldGen.KillWall(x, y);
                }
            }

            //place basic rectangle of brimstone slag
            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                for (int y = Main.maxTilesY - 90; y <= Main.maxTilesY - 5; y++)
                {
                    WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<BrimstoneSlag>());
                    Main.tile[x, y + 5].WallType = (ushort)ModContent.WallType<BrimstoneSlagWallUnsafe>();
                }
            }

            //place clusters of slag around the edges of the main square to create a more interesting shape
            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                for (int y = Main.maxTilesY - 110; y <= Main.maxTilesY - 5; y++)
                {
                    Tile tile = Main.tile[x, y];
                    Tile tileUp = Main.tile[x, y - 1];
                    Tile tileDown = Main.tile[x, y + 1];
                    Tile tileLeft = Main.tile[x - 1, y];
                    Tile tileRight = Main.tile[x + 1, y];

                    if (tile.TileType == ModContent.TileType<BrimstoneSlag>() && (tileUp.TileType != ModContent.TileType<BrimstoneSlag>() || 
                    tileDown.TileType != ModContent.TileType<BrimstoneSlag>() || tileLeft.TileType != ModContent.TileType<BrimstoneSlag>() || 
                    tileRight.TileType != ModContent.TileType<BrimstoneSlag>()))
                    {
                        NaturalCircle(x, y, WorldGen.genRand.Next(5, 22), ModContent.TileType<BrimstoneSlag>());
                    }
                }
            }

            /*
            //TODO: make a better way of generating patches of grass 
            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                if (WorldGen.genRand.Next(150) == 0)
                {
                    WorldGen.TileRunner(x - 20, Main.maxTilesY - 120, WorldGen.genRand.Next(8, 22), 
                    WorldGen.genRand.Next(8, 22), ModContent.TileType<ScorchedRemains>(), false, 0f, 0f, false, true);
                }
            }
            */

            //place ceiling across the top of the biome
            for (int x = biomeStart - 20; x <= biomeEdge + 20; x++)
            {
                for (int y = Main.maxTilesY - 200; y <= Main.maxTilesY - 192; y++)
                {
                    if (WorldGen.genRand.Next(25) == 0 || Main.tile[x, y].TileType == TileID.Ash)
                    {
                        NaturalCircle(x, y, WorldGen.genRand.Next(5, 15), ModContent.TileType<BrimstoneSlag>());
                    }
                }
            }

            //tile cleanup
            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                for (int y = Main.maxTilesY - 140; y <= Main.maxTilesY - 5; y++)
                {
                    Tile tile = Main.tile[x, y];
                    Tile tileUp = Main.tile[x, y - 1];
                    Tile tileDown = Main.tile[x, y + 1];
                    Tile tileDown2 = Main.tile[x, y + 2];
                    Tile tileLeft = Main.tile[x - 1, y];
                    Tile tileRight = Main.tile[x + 1, y];

                    //place tiles under slag tiles that are sticking out and look weird
                    if (tile.TileType == ModContent.TileType<BrimstoneSlag>() && !tileDown.HasTile)
                    {
                        WorldGen.PlaceTile(x, y + 1, (ushort)ModContent.TileType<BrimstoneSlag>());
                    }

                    //same as above, but for scorched remains
                    if (tile.TileType == ModContent.TileType<ScorchedRemains>() && !tileDown.HasTile)
                    {
                        WorldGen.PlaceTile(x, y + 1, (ushort)ModContent.TileType<ScorchedRemains>());
                    }

                    //place more walls behind slag so theres not just a box of walls from the initial rectangle
                    if (tile.HasTile && tileDown.HasTile && tileDown2.HasTile && Main.tile[x, y + 5].HasTile)
                    {
                        Main.tile[x, y + 5].WallType = (ushort)ModContent.WallType<BrimstoneSlagWallUnsafe>();
                    }

                    if (tile.LiquidType == LiquidID.Water && tile.LiquidAmount > 0)
                    {
                        tile.ClearEverything();
                        tile.LiquidType = LiquidID.Lava;
                        tile.LiquidAmount = 255;
                    }
                }
            }

            //place lava throughout the biome
            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                //cooldown before another lava pit can be placed
                if (lavaLakeBigPlaceDelay > 0)
                {
                    lavaLakeBigPlaceDelay--;
                }

                //surface lava lakes and pits
                //the reason theres three different runners being used is so that a shallow lava pit is placed above each lava pit
                if (lavaLakeBigPlaceDelay == 0)
                {
                    LavaTileRunner runner1 = new LavaTileRunner(new Vector2(x - 10, Main.maxTilesY - 110), new Vector2(0, 5), new Point16(-150, 150),
                    new Point16(250, 500), 15f, WorldGen.genRand.Next(200, 300), 0, true, true);
                    runner1.Start();

                    LavaTileRunner runner2 = new LavaTileRunner(new Vector2(x + 10, Main.maxTilesY - 110), new Vector2(0, 5), new Point16(-150, 150),
                    new Point16(250, 500), 15f, WorldGen.genRand.Next(200, 300), 0, true, true);
                    runner2.Start();

                    LavaTileRunner runner3 = new LavaTileRunner(new Vector2(x, Main.maxTilesY - 165), new Vector2(0, 5), new Point16(-20, 20), 
                    new Point16(250, 1000), 15f, WorldGen.genRand.Next(300, 400), 0, true, true);
                    runner3.Start();

                    lavaLakeBigPlaceDelay = 240; //set lava lake delay so it cant just spam lava lakes everywhere
                }
            }

            //place random blotches of lava underground
            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                for (int y = Main.maxTilesY - 100; y <= Main.maxTilesY - 20; y++)
                {
                    if (WorldGen.genRand.Next(1200) == 0)
                    {
                        LavaTileRunner runner = new LavaTileRunner(new Vector2(x, y), new Vector2(0, 5), new Point16(-20000, 20000),
                        new Point16(-12, 12), 15f, WorldGen.genRand.Next(12, 25), 0, true, true);
                        runner.Start();
                    }
                }
            }

            //clean up any lava above a certain threshold to prevent the biome's surface from being flooded
            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                for (int y = Main.maxTilesY - 150; y <= Main.maxTilesY - 125; y++)
                {
                    Main.tile[x, y].LiquidAmount = 0;
                }
            }

            //spread grass on all scorched remains
            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                for (int y = Main.maxTilesY - 200; y <= Main.maxTilesY - 5; y++)
                {
                    Tile tile = Main.tile[x, y];
                    Tile tileUp = Main.tile[x, y - 1];

                    if (tile.TileType == ModContent.TileType<ScorchedRemains>() && !tileUp.HasTile && tileUp.LiquidAmount == 0)
                    {
                        tile.TileType = (ushort)ModContent.TileType<ScorchedRemainsGrass>();
                    }

                    Tile.SmoothSlope(x, y, true);
                }
            }

            //charred ore
            for (int i = 0; i < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 10E-05); i++)
            {
                int x = WorldGen.genRand.Next(5, Main.maxTilesX - 5);
                int y = WorldGen.genRand.Next(5, Main.maxTilesY - 5);

                Tile tile = Main.tile[x, y];
                Tile tileUp = Main.tile[x, y - 1];
                Tile tileDown = Main.tile[x, y + 1];
                Tile tileLeft = Main.tile[x - 1, y];
                Tile tileRight = Main.tile[x + 1, y];

                if (tile.TileType == ModContent.TileType<BrimstoneSlag>() && (tileUp.LiquidAmount > 0 || 
                tileDown.LiquidAmount > 0 || tileLeft.LiquidAmount > 0 || tileRight.LiquidAmount > 0)) 
                {
                    WorldGen.TileRunner(x + WorldGen.genRand.Next(-22, 22), y + WorldGen.genRand.Next(-22, 22), 
                    WorldGen.genRand.Next(8, 22), WorldGen.genRand.Next(8, 22), ModContent.TileType<CharredOre>(), false, 0f, 0f, false, true);
                }
            }

            bool place = true;
            SchematicManager.PlaceSchematic<Action<Chest>>(SchematicManager.CragBridgeKey, new Point(biomeMiddle, Main.maxTilesY - 100), SchematicAnchor.Center, ref place);

            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                for (int y = Main.maxTilesY - 200; y <= Main.maxTilesY - 5; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.LiquidAmount > 0)
                    {
                        tile.LiquidType = LiquidID.Lava;
                        tile.LiquidAmount = 255;
                    }
                }
            }
        }

        //place a circular clump of tiles
        public static void NaturalCircle(int i, int j, int size, int tileType)
		{
			int BaseRadius = size;
			int radius = BaseRadius;

			for (int y = j - radius; y <= j + radius; y++)
			{
				for (int x = i - radius; x <= i + radius + 1; x++)
				{
					if ((int)Vector2.Distance(new Vector2(x, y), new Vector2(i, j)) <= radius && WorldGen.InWorld(x, y))
                    {
						Tile tile = Framing.GetTileSafely(x, y);

                        WorldGen.KillTile(x, y);
                        WorldGen.PlaceTile(x, y, tileType);
                        tile.Slope = 0;
                    }
				}

				radius = BaseRadius - WorldGen.genRand.Next(-1, 2);
			}
		}

        public static void GenCrags()
        {
            CragTerrain();
        }
    }
}