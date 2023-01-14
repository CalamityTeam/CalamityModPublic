using CalamityMod.Tiles.Crags;
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
using System.Collections.Generic;

namespace CalamityMod.World
{
    public class BrimstoneCrag
    {
        //clear area for the biome to generate in, do this way before the biome itself places to prevent weird issues or hell terrain shennanigans
        private static void ClearArea(int StartX)
        {
            //set these before genning just to be safe
            int biomeStart = StartX;
            int biomeEdge = biomeStart + (Main.maxTilesX / 6);

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
        }

        //basic terrain (also sorry for the excessive amount of loops lmao)
        private static void BasicCragTerrain(int StartX)
        {
            int biomeStart = StartX;
            int biomeEdge = biomeStart + (Main.maxTilesX / 6);

            float yLevel = 0;

            //place basic rectangle of brimstone slag
            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                for (int y = Main.maxTilesY - 110; y <= Main.maxTilesY - 6; y++)
                {
                    WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<BrimstoneSlag>());
                    Main.tile[x, y + 5].WallType = (ushort)ModContent.WallType<BrimstoneSlagWallUnsafe>();
                }
            }

            //place clusters of slag around the edges of the main square to create a more interesting shape
            //this is done after placing the lava lake so it keeps its shape but still has unique terrain
            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                //place clusters of slag around the edges of the main square to create a more interesting shape
                for (int y = Main.maxTilesY - 130; y <= Main.maxTilesY - 6; y++)
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
                        Circle(x, y, WorldGen.genRand.Next(5, 22), ModContent.TileType<BrimstoneSlag>(), false);
                    }
                }
            }

            //place clumps of scorched remains
            for (int x = biomeStart + 20; x <= biomeEdge - 20; x++)
            {
                if (WorldGen.genRand.Next(50) == 0)
                {
                    TileRunner runner = new TileRunner(new Vector2(x, Main.maxTilesY - 125), new Vector2(0, 5), new Point16(-45, 45), 
                    new Point16(-45, 45), 15f, WorldGen.genRand.Next(100, 250), (ushort)ModContent.TileType<ScorchedRemains>(), true, true);
                    runner.Start();
                }
            }

            //place ceiling across the top of the biome
            for (int x = biomeStart - 20; x <= biomeEdge + 20; x++)
            {
                for (int y = Main.maxTilesY - 200; y <= Main.maxTilesY - 192; y++)
                {
                    if (WorldGen.genRand.Next(25) == 0 || Main.tile[x, y].TileType == TileID.Ash)
                    {
                        Circle(x, y, WorldGen.genRand.Next(5, 15), ModContent.TileType<BrimstoneSlag>(), false);
                    }
                }
            }

            /*
            //place lava lakes on the left side of the biome
            for (int x = biomeStart + 20; x <= biomeStart + (Main.maxTilesX / 25); x++)
            {
                for (int y = Main.maxTilesY - 130; y <= Main.maxTilesY - 120; y++)
                {
                    WorldGen.digTunnel(x, y, 0, 1, 30, 3, false);
                }
            }

            //place lava lakes on the right side of the biome
            for (int x = biomeEdge - (Main.maxTilesX / 25); x <= biomeEdge - 20; x++)
            {
                for (int y = Main.maxTilesY - 130; y <= Main.maxTilesY - 120; y++)
                {
                    //
                    WorldGen.digTunnel(x, y, 0, 1, 30, 3, false);
                }
            }
            */

            /*
            //place big lava pit in the center
            for (int x = biomeStart + (Main.maxTilesX / 25); x <= biomeEdge - (Main.maxTilesX / 25); x++)
            {
                for (int y = Main.maxTilesY - 135; y <= Main.maxTilesY - 5; y++)
                {
                    if (Main.rand.Next(20) == 0 || Main.tile[x, y].TileType == ModContent.TileType<BrimstoneSlag>())
                    {
                        //this needs to be fixed a lot, it looks absolutely ugly due to how small it is
                        //also make an island of brimstone slag in the center to be like a brimstone elemental arena
                        LavaTileRunner runner = new LavaTileRunner(new Vector2(x, y), new Vector2(0, 15), new Point16(-25, 25), 
                        new Point16(-55, 55), 10f, 10, 0, false, true);
                        runner.Start();
                    }
                }
            }
            */

            //clean up any lava above a certain threshold to prevent the biome's surface from being flooded with lava
            for (int x = biomeStart + (Main.maxTilesX / 25); x <= biomeEdge - (Main.maxTilesX / 25); x++)
            {
                for (int y = Main.maxTilesY - 150; y <= Main.maxTilesY - 115; y++)
                {
                    Main.tile[x, y].LiquidAmount = 0;
                }
            }

            //final tile cleanup
            for (int x = biomeStart + (Main.maxTilesX / 25); x <= biomeEdge - (Main.maxTilesX / 25); x++)
            {
                for (int y = Main.maxTilesY - 140; y <= Main.maxTilesY - 5; y++)
                {
                    Tile tile = Main.tile[x, y];
                    Tile tileUp = Main.tile[x, y - 1];
                    Tile tileDown = Main.tile[x, y + 1];
                    Tile tileDown2 = Main.tile[x, y + 2];
                    Tile tileLeft = Main.tile[x - 1, y];
                    Tile tileRight = Main.tile[x + 1, y];

                    //place tiles under random slag tiles that are sticking out and look weird
                    if (tile.TileType == ModContent.TileType<BrimstoneSlag>() && !tileUp.HasTile && !tileDown.HasTile)
                    {
                        WorldGen.PlaceTile(x, y + 1, (ushort)ModContent.TileType<BrimstoneSlag>());
                    }

                    //place more walls behind slag so theres not just a box of walls from the initial rectangle
                    if (tile.TileType == ModContent.TileType<BrimstoneSlag>() && tileDown.TileType == ModContent.TileType<BrimstoneSlag>() && tileDown2.TileType == ModContent.TileType<BrimstoneSlag>())
                    {
                        Main.tile[x, y + 5].WallType = (ushort)ModContent.WallType<BrimstoneSlagWallUnsafe>();
                    }
                }
            }
        }

        //place a circular clump of tiles
        public static void Circle(int i, int j, int size, int tileType, bool killTile)
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

        public static void GenCrags(int StartX)
        {
            ClearArea(StartX);
            BasicCragTerrain(StartX);
        }
    }
}