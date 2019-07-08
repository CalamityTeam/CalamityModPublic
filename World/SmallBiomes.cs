using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;
using Terraria.Graphics.Effects;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.Localization;
using Terraria.GameContent.Events;
using Terraria.ModLoader.IO;
using CalamityMod.Tiles;

namespace CalamityMod.World
{
	public class SmallBiomes : ModWorld
	{
		public static void PlaceIceTomb()
		{
			int x = Main.maxTilesX;
			int y = Main.maxTilesY;

			for (int k = 0; k < (int)((double)(x * y) * 50E-05); k++)
			{
				int tilesX = WorldGen.genRand.Next(0, x);
				int tilesY = WorldGen.genRand.Next((int)(y * .4f), (int)(y * .5f));

				if (Main.tile[tilesX, tilesY].type == TileID.SnowBlock || Main.tile[tilesX, tilesY].type == TileID.IceBlock)
				{
					WorldGenerationMethods.IceTomb(tilesX, tilesY);
					break;
				}
			}
		}

		public static void PlaceEvilIsland()
		{
			int x = Main.maxTilesX;
			int xIslandGen = WorldGen.crimson ?
				WorldGen.genRand.Next((int)((double)x * 0.15), (int)((double)x * 0.2)) :
				WorldGen.genRand.Next((int)((double)x * 0.8), (int)((double)x * 0.85));
			int yIslandGen = WorldGen.genRand.Next(90, 151);
			yIslandGen = Math.Min(yIslandGen, (int)WorldGen.worldSurfaceLow - 50);

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

			for (int k = 0; k < (int)((double)(x * y) * 50E-05); k++) //Surface Shrine
			{
				int tilesX = WorldGen.genRand.Next((int)((double)x * 0.3), generateBack);
				int tilesX2 = WorldGen.genRand.Next(generateForward, (int)((double)x * 0.7));
				int tilesY = WorldGen.genRand.Next((int)(y * .3f), (int)(y * .35f));

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

			for (int k = 0; k < (int)((double)(x * y) * 50E-05); k++) //Evil Shrine
			{
				int tilesX = WorldGen.genRand.Next(0, x);
				int tilesY = WorldGen.genRand.Next((int)(y * .3f), (int)(y * .35f));

				if (Main.tile[tilesX, tilesY].type == (WorldGen.crimson ? TileID.Crimstone : TileID.Ebonstone))
				{
					WorldGenerationMethods.SpecialHut(WorldGen.crimson ? TileID.CrimtaneBrick : TileID.DemoniteBrick,
						WorldGen.crimson ? TileID.Crimstone : TileID.Ebonstone,
						WorldGen.crimson ? WallID.CrimtaneBrick : WallID.DemoniteBrick, 1, tilesX, tilesY);
					break;
				}
			}

			for (int k = 0; k < (int)((double)(x * y) * 50E-05); k++) //Cavern Shrine
			{
				int tilesX = WorldGen.genRand.Next((int)((double)x * 0.3), generateBack);
				int tilesX2 = WorldGen.genRand.Next(generateForward, (int)((double)x * 0.7));
				int tilesY = WorldGen.genRand.Next((int)(y * .55f), (int)(y * .8f));

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

			for (int k = 0; k < (int)((double)(x * y) * 50E-05); k++) //Ice Shrine
			{
				int tilesX = WorldGen.genRand.Next(0, x);
				int tilesY = WorldGen.genRand.Next((int)(y * .35f), (int)(y * .5f));

				if (Main.tile[tilesX, tilesY].type == TileID.IceBlock)
				{
					WorldGenerationMethods.SpecialHut(TileID.IceBrick, TileID.IceBlock, WallID.IceBrick, 3, tilesX, tilesY);
					break;
				}
			}

			for (int k = 0; k < (int)((double)(x * y) * 50E-05); k++) //Desert Shrine
			{
				int tilesX = WorldGen.genRand.Next(0, x);
				int tilesY = WorldGen.genRand.Next((int)(y * .35f), (int)(y * .5f));

				if (Main.tile[tilesX, tilesY].type == TileID.DesertFossil)
				{
					WorldGenerationMethods.SpecialHut(TileID.DesertFossil, TileID.Sandstone, WallID.DesertFossil, 4, tilesX, tilesY);
					break;
				}
			}

			for (int k = 0; k < (int)((double)(x * y) * 50E-05); k++) //Mushroom Shrine
			{
				int tilesX = WorldGen.genRand.Next(0, x);
				int tilesY = WorldGen.genRand.Next((int)(y * .35f), (int)(y * .5f));

				if (Main.tile[tilesX, tilesY].type == TileID.MushroomGrass)
				{
					WorldGenerationMethods.SpecialHut(TileID.MushroomBlock, TileID.Mud, WallID.MushroomUnsafe, 5, tilesX, tilesY);
					break;
				}
			}

			for (int k = 0; k < (int)((double)(x * y) * 50E-05); k++) //Granite Shrine
			{
				int tilesX = WorldGen.genRand.Next(0, x);
				int tilesY = WorldGen.genRand.Next((int)(y * .35f), (int)(y * .5f));

				if (Main.tile[tilesX, tilesY].type == TileID.Granite)
				{
					WorldGenerationMethods.SpecialHut(TileID.GraniteBlock, TileID.Granite, WallID.GraniteUnsafe, 6, tilesX, tilesY);
					break;
				}
			}

			for (int k = 0; k < (int)((double)(x * y) * 50E-05); k++) //Marble Shrine
			{
				int tilesX = WorldGen.genRand.Next(0, x);
				int tilesY = WorldGen.genRand.Next((int)(y * .35f), (int)(y * .5f));

				if (Main.tile[tilesX, tilesY].type == TileID.Marble)
				{
					WorldGenerationMethods.SpecialHut(TileID.MarbleBlock, TileID.Marble, WallID.MarbleUnsafe, 7, tilesX, tilesY);
					break;
				}
			}

			//Murasama Shrine
			WorldGenerationMethods.SpecialHut(TileID.HellstoneBrick, TileID.Hellstone, WallID.HellstoneBrick, 8, WorldGen.genRand.Next((int)((double)x * 0.97), (int)((double)x * 0.98)), y - 60);
		}
	}
}
