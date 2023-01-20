using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.Crags.Spike;
using CalamityMod.Tiles.Crags.Tree;
using CalamityMod.Tiles.Ores;
using CalamityMod.Walls;
using CalamityMod.Schematics;
using static CalamityMod.Schematics.SchematicManager;
using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.WorldBuilding;
using Terraria.Utilities;
using Terraria.GameContent.Generation;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CalamityMod.World
{
    public class BrimstoneCrag
    {
        static int StartX = 0;

        static int lavaLakePlaceDelay = 0;
        static int lavaLakeBigPlaceDelay = 0;

        static int TreeDelay = 0;

        //basic terrain (sorry for the excessive amount of loops lmao)
        private static void GenCrags()
        {
            //set this here so it can properly scale to the worldsize and whatever
            //the 25's are there to offset it from the exact edge of the world so no "out of bounds" crashing occurs
            StartX = WorldGen.dungeonX < Main.maxTilesX / 2 ? 25 : (Main.maxTilesX - (Main.maxTilesX / 5)) - 25;

            //set these to be able to easily place things in certain locations, like structures
            int biomeStart = StartX;
            int biomeEdge = biomeStart + (Main.maxTilesX / 5);
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

            //scorched remains patches
            for (int x = biomeStart + 30; x <= biomeEdge - 30; x++)
            {
                if (WorldGen.genRand.Next(150) == 0)
                {
                    ScorchedGrassPatches(new Point(x, Main.maxTilesY - 135));
                }
            }

            //place ceiling across the top of the biome
            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                for (int y = Main.maxTilesY - 200; y <= Main.maxTilesY - 192; y++)
                {
                    if (WorldGen.genRand.Next(25) == 0)
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

            //ceiling cleanup
            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                for (int y = Main.maxTilesY - 195; y <= Main.maxTilesY - 180; y++)
                {
                    Tile tile = Main.tile[x, y];
                    Tile tileUp = Main.tile[x, y - 1];

                    if (tile.TileType == ModContent.TileType<BrimstoneSlag>() && !tileUp.HasTile)
                    {
                        WorldGen.PlaceTile(x, y - 1, (ushort)ModContent.TileType<BrimstoneSlag>());
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

                    LavaTileRunner runner3 = new LavaTileRunner(new Vector2(x, Main.maxTilesY - 165), new Vector2(0, 5), new Point16(-500, 500), 
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
            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                for (int y = Main.maxTilesY - 150; y <= Main.maxTilesY - 45; y++)
                {
                    Tile tile = Main.tile[x, y];
                    Tile tileUp = Main.tile[x, y - 1];
                    Tile tileDown = Main.tile[x, y + 1];
                    Tile tileLeft = Main.tile[x - 1, y];
                    Tile tileRight = Main.tile[x + 1, y];

                    //only place ore nearby lava
                    if (WorldGen.genRand.Next(200) == 0 && tile.TileType == ModContent.TileType<BrimstoneSlag>() && (tileUp.LiquidAmount > 0 || 
                    tileDown.LiquidAmount > 0 || tileLeft.LiquidAmount > 0 || tileRight.LiquidAmount > 0))
                    {
                        WorldGen.TileRunner(x + WorldGen.genRand.Next(-15, 15), y + WorldGen.genRand.Next(-15, 15), 
                        WorldGen.genRand.Next(8, 12), WorldGen.genRand.Next(8, 12), ModContent.TileType<CharredOre>(), false, 0f, 0f, false, true);
                    }
                }
            }

            bool firstItem = false;
            SchematicManager.PlaceSchematic(SchematicManager.CragBridgeKey, new Point(biomeMiddle, Main.maxTilesY - 100),
            SchematicAnchor.Center, ref firstItem, new Action<Chest, int, bool>(FillBrimstoneChests));

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

        //place ambient objects and other stuff
        private static void GenCragsAmbience()
        {
            int biomeStart = StartX;
            int biomeEdge = biomeStart + (Main.maxTilesX / 5);
            int biomeMiddle = (biomeStart + biomeEdge) / 2;

            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                for (int y = Main.maxTilesY - 200; y <= Main.maxTilesY - 5; y++)
                {
                    Tile tile = Main.tile[x, y];
                    Tile tileUp = Main.tile[x, y - 1];

                    //stalactites and stalagmites
                    if (tile.TileType == ModContent.TileType<BrimstoneSlag>())
                    {
                        if (WorldGen.genRand.Next(32) == 0)
                        {
                            ushort[] Stalactites = new ushort[] { (ushort)ModContent.TileType<CragStalactiteGiant1>(),
                            (ushort)ModContent.TileType<CragStalactiteGiant2>(), (ushort)ModContent.TileType<CragStalactiteGiant3>() };

                            WorldGen.PlaceObject(x, y + 2, WorldGen.genRand.Next(Stalactites));
                        }

                        if (WorldGen.genRand.Next(10) == 0)
                        {
                            ushort[] Stalactites = new ushort[] { (ushort)ModContent.TileType<CragStalactiteLarge1>(), 
                            (ushort)ModContent.TileType<CragStalactiteLarge2>(), (ushort)ModContent.TileType<CragStalactiteLarge3>(), 
                            (ushort)ModContent.TileType<CragStalactiteSmall1>(), (ushort)ModContent.TileType<CragStalactiteSmall2>(), 
                            (ushort)ModContent.TileType<CragStalactiteSmall3>() };

                            WorldGen.PlaceObject(x, y + 2, WorldGen.genRand.Next(Stalactites));
                        }

                        if (WorldGen.genRand.Next(25) == 0)
                        {
                            ushort[] Stalagmites = new ushort[] { (ushort)ModContent.TileType<CragStalagmiteGiant1>(),
                            (ushort)ModContent.TileType<CragStalagmiteGiant2>(), (ushort)ModContent.TileType<CragStalagmiteGiant3>() };

                            WorldGen.PlaceObject(x, y - 1, WorldGen.genRand.Next(Stalagmites));
                        }

                        if (WorldGen.genRand.Next(8) == 0)
                        {
                            ushort[] Stalagmites = new ushort[] { (ushort)ModContent.TileType<CragStalagmiteLarge1>(), 
                            (ushort)ModContent.TileType<CragStalagmiteLarge2>(), (ushort)ModContent.TileType<CragStalagmiteLarge3>(), 
                            (ushort)ModContent.TileType<CragStalagmiteSmall1>(), (ushort)ModContent.TileType<CragStalagmiteSmall2>(), 
                            (ushort)ModContent.TileType<CragStalagmiteSmall3>() };

                            WorldGen.PlaceObject(x, y - 1, WorldGen.genRand.Next(Stalagmites));
                        }

                        //decrease tree delay
                        if (TreeDelay > 0)
                        {
                            TreeDelay--;
                        }

                        //grow spine tree
                        //TODO: why are they still spawning in lava i literally checked for liquid please terraria why
                        if (WorldGen.genRand.Next(12) == 0 && tileUp.LiquidAmount == 0 && !tile.IsHalfBlock && TreeDelay == 0)
                        {
                            SpineTree.Spawn(x, y - 1, -1, 22, 28, false, -1, false);
                            TreeDelay = 12;
                        }
                    }
                }
            }
        }

        public static void FillBrimstoneChests(Chest chest, int Type, bool firstItem)
        {
            /*
            Potential other item ideas:
            Charred Idol
            Obsidian Rose
            Demon Conch
            Lava Charm
            */

            int potionType = Utils.SelectRandom(WorldGen.genRand, ItemID.ObsidianSkinPotion, ItemID.BattlePotion, ItemID.InfernoPotion, ItemID.PotionOfReturn);
            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(ItemID.HellstoneBar, WorldGen.genRand.Next(2, 5)),
                new ChestItem(ModContent.ItemType<Items.Materials.DemonicBoneAsh>(), WorldGen.genRand.Next(4, 15)),
                new ChestItem(ModContent.ItemType<Items.Fishing.BrimstoneCragCatches.CoastalDemonfish>(), WorldGen.genRand.Next(2, 5)),
                new ChestItem(ItemID.HellfireArrow, WorldGen.genRand.Next(25, 50)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(2, 12)),
            };

            if (!firstItem)
            {
                contents.Insert(0, new ChestItem(ModContent.ItemType<Items.Weapons.Rogue.AshenStalactite>(), 1));
            }
            else
            {
                contents.RemoveAt(0);
                contents.Insert(0, new ChestItem(ModContent.ItemType<Items.Weapons.Melee.BladecrestOathsword>(), 1));
                contents.Insert(1, new ChestItem(ItemID.HellstoneBar, WorldGen.genRand.Next(2, 5))); //temporary fix since removing the first item also removes hellstone bars
            }
            
            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }

        //shamelessly copy pasted from the astral biome generation
        public static void ScorchedGrassPatches(object obj)
        {
            //Pre-calculate all variables necessary for elliptical area checking
            Point origin = (Point)obj;
            Vector2 center = origin.ToVector2() * 16f + new Vector2(8f);

            float angle = MathHelper.Pi * 0.15f;
            float otherAngle = MathHelper.PiOver2 - angle;

            int distanceInTiles = WorldGen.genRand.Next(20, 50);
            float distance = distanceInTiles * 16f;
            float constant = distance * 2f / (float)Math.Sin(angle);

            float fociSpacing = distance * (float)Math.Sin(otherAngle) / (float)Math.Sin(angle);
            int verticalRadius = (int)(constant / 16f);

            Vector2 fociOffset = Vector2.UnitY * fociSpacing;
            Vector2 topFoci = center - fociOffset;
            Vector2 bottomFoci = center + fociOffset;

            UnifiedRandom rand = WorldGen.genRand;
            for (int x = origin.X - distanceInTiles - 2; x <= origin.X + distanceInTiles + 2; x++)
            {
                for (int y = (int)(origin.Y - verticalRadius * 0.4f) - 3; y <= origin.Y + verticalRadius + 3; y++)
                {
                    if (CheckInEllipse(new Point(x, y), topFoci, bottomFoci, constant, center, out float dist, y < origin.Y))
                    {
                        //If we're in the outer blurPercent% of the ellipse
                        float percent = dist / constant;
                        float blurPercent = 0.98f;
                        if (percent > blurPercent)
                        {
                            float outerEdgePercent = (percent - blurPercent) / (1f - blurPercent);
                            if (rand.NextFloat(1f) > outerEdgePercent && Main.tile[x, y].HasTile && WorldGen.InWorld(x, y))
                            {
                                Main.tile[x, y].TileType = (ushort)ModContent.TileType<ScorchedRemains>();
                            }
                        }
                        else if (Main.tile[x, y].HasTile && WorldGen.InWorld(x, y))
                        {
                            Main.tile[x, y].TileType = (ushort)ModContent.TileType<ScorchedRemains>();
                        }
                    }
                }
            }
        }

        //shamelessly copy pasted from the astral biome generation
        public static bool CheckInEllipse(Point tile, Vector2 focus1, Vector2 focus2, float distanceConstant, Vector2 center, out float distance, bool collapse = false)
        {
            Vector2 point = tile.ToWorldCoordinates();
            if (collapse) //Collapse ensures the ellipse is shrunk down a lot in terms of distance.
            {
                float distY = center.Y - point.Y;
                point.Y -= distY * 8f;
            }
            float distance1 = Vector2.Distance(point, focus1);
            float distance2 = Vector2.Distance(point, focus2);
            distance = distance1 + distance2;
            return distance <= distanceConstant;
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

        public static void GenAllCragsStuff()
        {
            GenCrags();
            GenCragsAmbience();
        }
    }
}