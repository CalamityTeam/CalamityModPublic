using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Schematics;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.Crags.Lily;
using CalamityMod.Tiles.Crags.Spike;
using CalamityMod.Tiles.Crags.Tree;
using CalamityMod.Tiles.Ores;
using CalamityMod.Walls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using static CalamityMod.Schematics.SchematicManager;

namespace CalamityMod.World
{
    public class BrimstoneCrag
    {
        static int StartX = 0;

        static int lavaLakeBigPlaceDelay = 0;
        static int numLavaLakes = 0;

        //all the main crags terrain generation
        private static void GenCrags()
        {
            //set StartX here so it can properly scale with worldsize (just to be safe)
            //the 25's are there to offset it from the exact edge of the world so that no "out of bounds" crashing occurs
            StartX = GenVars.dungeonX < Main.maxTilesX / 2 ? 25 : (Main.maxTilesX - (Main.maxTilesX / 5)) - 25;

            //set these to be able to easily place things in certain locations, like structures
            int biomeStart = StartX;
            int biomeEdge = biomeStart + (Main.maxTilesX / 5);
            int biomeMiddle = (biomeStart + biomeEdge) / 2;

            //clear literally everything in the area the biome will generate in
            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                for (int y = Main.UnderworldLayer; y < Main.maxTilesY - 2; y++)
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
                    Main.tile[x, y].WallType = (ushort)ModContent.WallType<BrimstoneSlagWallUnsafe>();
                }
            }

            //place clusters of slag around the edges of the main square to create a more interesting shape
            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                for (int y = Main.maxTilesY - 110; y <= Main.maxTilesY - 20; y++)
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
                        ShapeData circle = new ShapeData();
                        GenAction blotchMod = new Modifiers.Blotches(2, 0.4);
                        int radius = WorldGen.genRand.Next(5, 20);
                        WorldUtils.Gen(new Point(x, y), new Shapes.Circle(radius), Actions.Chain(new GenAction[]
                        {
                            blotchMod.Output(circle)
                        }));

                        WorldUtils.Gen(new Point(x, y), new ModShapes.All(circle), Actions.Chain(new GenAction[]
                        {
                            new Actions.ClearTile(),
                            new Actions.PlaceTile((ushort)ModContent.TileType<BrimstoneSlag>())
                        }));
                    }
                }
            }

            //place ceiling of slag across the top of the biome
            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                for (int y = Main.UnderworldLayer; y <= Main.maxTilesY - 192; y++)
                {
                    if (WorldGen.genRand.NextBool(25))
                    {
                        ShapeData circle = new ShapeData();
                        GenAction blotchMod = new Modifiers.Blotches(2, 0.4);
                        int radius = WorldGen.genRand.Next(3, 7);
                        WorldUtils.Gen(new Point(x, y), new Shapes.Circle(radius), Actions.Chain(new GenAction[]
                        {
                            blotchMod.Output(circle)
                        }));

                        WorldUtils.Gen(new Point(x, y), new ModShapes.All(circle), Actions.Chain(new GenAction[]
                        {
                            new Actions.ClearTile(),
                            new Actions.PlaceTile((ushort)ModContent.TileType<BrimstoneSlag>())
                        }));
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

                //surface lava pits
                if (lavaLakeBigPlaceDelay == 0)
                {
                    //do not place lava in the general middle area of the biome because it keeps flooding the crag bridge
                    if (numLavaLakes != 2 && numLavaLakes != 3)
                    {
                        LavaTileRunner runner = new LavaTileRunner(new Vector2(x, Main.maxTilesY - 165), new Vector2(0, 5), new Point16(-500, 500),
                        new Point16(250, 1000), 15f, WorldGen.genRand.Next(300, 1000), 0, true, true);
                        runner.Start();
                    }

                    numLavaLakes++;
                    lavaLakeBigPlaceDelay = 200; //set lava lake delay so it cant just spam lava lakes everywhere
                }
            }

            //place random blotches of lava underground
            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                for (int y = Main.maxTilesY - 100; y <= Main.maxTilesY - 20; y++)
                {
                    if (WorldGen.genRand.NextBool(1200))
                    {
                        LavaTileRunner runner = new LavaTileRunner(new Vector2(x, y), new Vector2(0, 5), new Point16(0, 5),
                        new Point16(-12, 12), 15f, WorldGen.genRand.Next(-12, 25), 0, true, true);
                        runner.Start();
                    }
                }
            }

            //scorched remains patches
            for (int x = biomeStart + 30; x <= biomeEdge - 30; x++)
            {
                if (WorldGen.genRand.NextBool(145))
                {
                    ScorchedGrassPatches(new Point(x, Main.maxTilesY - 135));
                }
            }

            //place clumps of Infernal Suevite
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
                    if (WorldGen.genRand.NextBool(180)&& tile.TileType == ModContent.TileType<BrimstoneSlag>() && (tileUp.LiquidAmount > 0 ||
                    tileDown.LiquidAmount > 0 || tileLeft.LiquidAmount > 0 || tileRight.LiquidAmount > 0))
                    {
                        WorldGen.TileRunner(x + WorldGen.genRand.Next(-15, 15), y + WorldGen.genRand.Next(-15, 15),
                        WorldGen.genRand.Next(10, 12), WorldGen.genRand.Next(10, 12), ModContent.TileType<InfernalSuevite>(), false, 0f, 0f, false, true);
                    }
                }
            }

            //place bridge in the center of the biome
            bool firstItem = false;
            SchematicManager.PlaceSchematic(SchematicManager.CragBridgeKey, new Point(biomeMiddle, Main.maxTilesY - 100),
            SchematicAnchor.Center, ref firstItem, new Action<Chest, int, bool>(FillBrimstoneChests));

            //place crag ruins
            bool place = true;

            int house1Offset = WorldGen.genRand.Next(0, 55);
            PlaceSquareForCragHouses(biomeStart + 150 + house1Offset, Main.maxTilesY - 125);
            SchematicManager.PlaceSchematic<Action<Chest>>(SchematicManager.CragRuinKey3,
            new Point(biomeStart + 150 + house1Offset, Main.maxTilesY - 125), SchematicAnchor.BottomCenter, ref place);

            int house2Offset = WorldGen.genRand.Next(-55, 0);
            PlaceSquareForCragHouses(biomeMiddle - 235 + house2Offset, Main.maxTilesY - 125);
            SchematicManager.PlaceSchematic<Action<Chest>>(SchematicManager.CragRuinKey1,
            new Point(biomeMiddle - 235 + house2Offset, Main.maxTilesY - 125), SchematicAnchor.BottomCenter, ref place);

            int house3Offset = WorldGen.genRand.Next(0, 55);
            PlaceSquareForCragHouses(biomeMiddle + 235 + house3Offset, Main.maxTilesY - 125);
            SchematicManager.PlaceSchematic<Action<Chest>>(SchematicManager.CragRuinKey4,
            new Point(biomeMiddle + 235 + house3Offset, Main.maxTilesY - 125), SchematicAnchor.BottomCenter, ref place);

            int house4Offset = WorldGen.genRand.Next(-55, 0);
            PlaceSquareForCragHouses(biomeEdge - 150 + house4Offset, Main.maxTilesY - 125);
            SchematicManager.PlaceSchematic<Action<Chest>>(SchematicManager.CragRuinKey2,
            new Point(biomeEdge - 150 + house4Offset, Main.maxTilesY - 125), SchematicAnchor.BottomCenter, ref place);

            //lava clean up again
            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                for (int y = Main.UnderworldLayer; y <= Main.maxTilesY - 5; y++)
                {
                    Tile tile = Main.tile[x, y];
                    Tile tileAbove = Main.tile[x, y - 1];

                    if (tile.LiquidAmount > 0)
                    {
                        tile.LiquidType = LiquidID.Lava;
                        tile.LiquidAmount = 255;
                    }

                    //get rid of lava above scorched remains as much as possible
                    if (tile.TileType == ModContent.TileType<ScorchedRemains>() && !tileAbove.HasTile)
                    {
                        for (int i = x - 1; i <= x + 1; i++)
                        {
                            for (int j = y - 5; j <= y; j++)
                            {
                                Tile lavaTile = Main.tile[i, j];
                                Tile lavaTileDown = Main.tile[i, j + 1];

                                if (lavaTile.WallType == 0 && lavaTileDown.WallType == 0)
                                {
                                    lavaTile.LiquidAmount = 0;
                                }
                            }
                        }
                    }
                }
            }

            //clean up any lava in the biomes surface to prevent it from being obnoxiously flooded
            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                for (int y = Main.maxTilesY - 150; y <= Main.maxTilesY - 122; y++)
                {
                    Main.tile[x, y].LiquidAmount = 0;
                }
            }

            //settle all liquids
            CalamityUtils.SettleWater();

            //spread grass on all scorched remains with no lava above them
            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                for (int y = Main.UnderworldLayer; y <= Main.maxTilesY - 110; y++)
                {
                    Tile tile = Main.tile[x, y];
                    Tile tileUp = Main.tile[x, y - 1];

                    if (tile.TileType == ModContent.TileType<ScorchedRemains>() && !tileUp.HasTile && tileUp.LiquidAmount == 0)
                    {
                        tile.TileType = (ushort)ModContent.TileType<ScorchedRemainsGrass>();
                    }
                }
            }

            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                for (int y = Main.UnderworldLayer; y <= Main.maxTilesY - 5; y++)
                {
                    Tile tile = Main.tile[x, y];
                    Tile tileUp = Main.tile[x, y - 1];
                    Tile tileDown = Main.tile[x, y + 1];
                    Tile tileLeft = Main.tile[x - 1, y];
                    Tile tileRight = Main.tile[x + 1, y];

                    if (tile.TileType == ModContent.TileType<BrimstoneSlag>() || tile.TileType == ModContent.TileType<ScorchedRemains>() ||
                    tile.TileType == ModContent.TileType<ScorchedRemainsGrass>())
                    {
                        //slope tiles
                        Tile.SmoothSlope(x, y, true);

                        //kill any individual floating tiles
                        if (!tileUp.HasTile && !tileDown.HasTile && !tileLeft.HasTile && !tileRight.HasTile)
                        {
                            WorldGen.KillTile(x, y);
                        }
                    }
                }
            }
        }

        //place ambient objects
        private static void GenCragsAmbience()
        {
            int biomeStart = StartX;
            int biomeEdge = biomeStart + (Main.maxTilesX / 5);
            int biomeMiddle = (biomeStart + biomeEdge) / 2;

            for (int x = biomeStart; x <= biomeEdge; x++)
            {
                for (int y = Main.UnderworldLayer; y <= Main.maxTilesY - 5; y++)
                {
                    Tile tile = Main.tile[x, y];

                    //stalactites and stalagmites
                    if (tile.TileType == ModContent.TileType<BrimstoneSlag>())
                    {
                        if (WorldGen.genRand.NextBool(20))
                        {
                            ushort[] Stalactites = new ushort[] { (ushort)ModContent.TileType<CragStalactiteGiant1>(),
                            (ushort)ModContent.TileType<CragStalactiteGiant2>(), (ushort)ModContent.TileType<CragStalactiteGiant3>() };

                            WorldGen.PlaceObject(x, y + 2, WorldGen.genRand.Next(Stalactites));
                        }

                        if (WorldGen.genRand.NextBool(8))
                        {
                            ushort[] Stalactites = new ushort[] { (ushort)ModContent.TileType<CragStalactiteLarge1>(),
                            (ushort)ModContent.TileType<CragStalactiteLarge2>(), (ushort)ModContent.TileType<CragStalactiteLarge3>(),
                            (ushort)ModContent.TileType<CragStalactiteSmall1>(), (ushort)ModContent.TileType<CragStalactiteSmall2>(),
                            (ushort)ModContent.TileType<CragStalactiteSmall3>() };

                            WorldGen.PlaceObject(x, y + 2, WorldGen.genRand.Next(Stalactites));
                        }

                        if (WorldGen.genRand.NextBool(25))
                        {
                            ushort[] Stalagmites = new ushort[] { (ushort)ModContent.TileType<CragStalagmiteGiant1>(),
                            (ushort)ModContent.TileType<CragStalagmiteGiant2>(), (ushort)ModContent.TileType<CragStalagmiteGiant3>() };

                            WorldGen.PlaceObject(x, y - 1, WorldGen.genRand.Next(Stalagmites));
                        }

                        if (WorldGen.genRand.NextBool(8))
                        {
                            ushort[] Stalagmites = new ushort[] { (ushort)ModContent.TileType<CragStalagmiteLarge1>(),
                            (ushort)ModContent.TileType<CragStalagmiteLarge2>(), (ushort)ModContent.TileType<CragStalagmiteLarge3>(),
                            (ushort)ModContent.TileType<CragStalagmiteSmall1>(), (ushort)ModContent.TileType<CragStalagmiteSmall2>(),
                            (ushort)ModContent.TileType<CragStalagmiteSmall3>() };

                            WorldGen.PlaceObject(x, y - 1, WorldGen.genRand.Next(Stalagmites));
                        }
                    }

                    //place lillies on scorched remains grass
                    if (tile.TileType == ModContent.TileType<ScorchedRemainsGrass>())
                    {
                        //place them often since they are pretty big tiles, also dont place them in lava
                        ushort[] Lillies = new ushort[] { (ushort)ModContent.TileType<LavaLily1>(),
                        (ushort)ModContent.TileType<LavaLily2>(), (ushort)ModContent.TileType<LavaLily3>(),
                        (ushort)ModContent.TileType<LavaLily4>(), (ushort)ModContent.TileType<LavaLily5>(),
                        (ushort)ModContent.TileType<LavaLily6>() };

                        PlaceCragLily(x, y - 1, WorldGen.genRand.Next(Lillies));
                    }
                }

                //separate y loop so trees dont grow too deep
                for (int y = Main.maxTilesY - 150; y <= Main.maxTilesY - 122; y++)
                {
                    Tile tile = Main.tile[x, y];

                    Tile.SmoothSlope(x, y, true);

                    //stalactites and stalagmites
                    if (tile.TileType == ModContent.TileType<BrimstoneSlag>())
                    {
                        //grow spine tree
                        if (WorldGen.genRand.NextBool(8)&& !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
                        {
                            PlaceTree(x, y - 1, ModContent.TileType<SpineTree>());
                        }
                    }
                }
            }
        }

        public static void PlaceSquareForCragHouses(int x, int y)
        {
            //loop to place tiles under the house so it doesnt look weird
            for (int i = x - 25; i <= x + 25; i++)
            {
                for (int j = y; j <= y + 15; j++)
                {
                    WorldGen.PlaceTile(i, j, ModContent.TileType<BrimstoneSlag>());
                }
            }
        }

        public static bool PlaceTree(int x, int y, int tileType)
        {
            int minDistance = 5;
            int treeNearby = 0;

            for (int i = x - minDistance; i < x + minDistance; i++)
            {
                for (int j = y - minDistance; j < y + minDistance; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == tileType)
                    {
                        treeNearby++;
                        if (treeNearby > 0)
                        {
                            return false;
                        }
                    }
                }
            }

            SpineTree.Spawn(x, y, 22, 28, false);

            return true;
        }

        public static bool PlaceCragLily(int x, int y, int tileType)
        {
            int minDistance = 15;
            int lilyNearby = 0;

            for (int i = x - minDistance; i < x + minDistance; i++)
            {
                for (int j = y - minDistance; j < y + minDistance; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == tileType)
                    {
                        lilyNearby++;
                        if (lilyNearby > 0)
                        {
                            return false;
                        }
                    }
                }
            }

            for (int upChechY = y - 10; upChechY < y; upChechY++)
            {
                if (Main.tile[x, upChechY].HasTile)
                {
                    return false;
                }
            }

            WorldGen.PlaceObject(x, y, tileType);

            return true;
        }

        public static void FillBrimstoneChests(Chest chest, int Type, bool firstItem)
        {
            int potionType = Utils.SelectRandom(WorldGen.genRand, ItemID.ObsidianSkinPotion, ItemID.BattlePotion, ItemID.InfernoPotion, ItemID.PotionOfReturn);
            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(ItemID.HellstoneBar, WorldGen.genRand.Next(4, 6)),
                new ChestItem(ModContent.ItemType<Items.Materials.DemonicBoneAsh>(), WorldGen.genRand.Next(4, 15)),
                new ChestItem(ModContent.ItemType<Items.Fishing.BrimstoneCragCatches.CoastalDemonfish>(), WorldGen.genRand.Next(2, 5)),
                new ChestItem(ItemID.HellfireArrow, WorldGen.genRand.Next(25, 50)),
                new ChestItem(potionType, WorldGen.genRand.Next(1, 3)),
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
                //re-add hellstone bars to the list since removing the first item also removes hellstone bars for some reason
                contents.Insert(1, new ChestItem(ItemID.HellstoneBar, WorldGen.genRand.Next(2, 5)));
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

            int distanceInTiles = WorldGen.genRand.Next(30, 45);
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

        public static void GenAllCragsStuff()
        {
            GenCrags();
            GenCragsAmbience();
        }
    }
}
