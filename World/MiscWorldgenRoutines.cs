using System;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Potions.Food;
using CalamityMod.Tiles;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.FurnitureAuric;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityMod.World
{
    public class MiscWorldgenRoutines
    {
        #region Place Chests
        internal static Chest AddChestWithLoot(int i, int j, ushort type = TileID.Containers, uint startingSlot = 1, int tileStyle = 0)
        {
            int chestIndex = -1;

            // Slide downwards on the Y axis trying to find the floor beneath the empty position initially picked
            while (j < Main.maxTilesY - 210)
            {
                if (!WorldGen.SolidTile(i, j))
                {
                    j++;
                    continue;
                }

                // If there are already 1,000 chests in the world and this one fails to place, just give up.
                chestIndex = WorldGen.PlaceChest(i - 1, j - 1, type, false, tileStyle);
                break;
            }

            if (chestIndex < 0)
                return null;

            Chest chest = Main.chest[chestIndex];
            PlaceLootInChest(ref chest, type, startingSlot);
            return chest;
        }

        internal static void PlaceLootInChest(ref Chest chest, ushort type, uint startingSlot)
        {
            uint itemIndex = startingSlot;

            void PutItemInChest(ref Chest c, int id, int minQuantity = 0, int maxQuantity = 0, bool condition = true)
            {
                if (!condition)
                    return;

                c.item[itemIndex].SetDefaults(id, false);
                c.item[itemIndex].Prefix(-1);

                // Don't set quantity unless quantity is specified
                if (minQuantity > 0)
                {
                    // Max quantity cannot be less than min quantity. It's zero if not specified, meaning you get exactly minQuantity.
                    if (maxQuantity < minQuantity)
                        maxQuantity = minQuantity;
                    c.item[itemIndex].stack = WorldGen.genRand.Next(minQuantity, maxQuantity + 1);
                }
                itemIndex++;
            }

            // Astral Chest has completely different loot in it
            if (type == ModContent.TileType<AstralChestLocked>())
            {
                PutItemInChest(ref chest, ModContent.ItemType<StarblightSoot>(), 30, 80);
                PutItemInChest(ref chest, ModContent.ItemType<AureusCell>(), 10, 14);
                PutItemInChest(ref chest, ModContent.ItemType<ZergPotion>(), 8);
                PutItemInChest(ref chest, ModContent.ItemType<ZenPotion>(), 3, 5);
                PutItemInChest(ref chest, ItemID.FallenStar, 12, 30);

                // Gold Coins don't stack above 100, so this efficiently lets you stuff over a platinum into a chest
                int goldCoins = WorldGen.genRand.Next(30, 120);
                if (goldCoins > 100)
                {
                    PutItemInChest(ref chest, ItemID.PlatinumCoin);
                    goldCoins -= 100;
                }
                PutItemInChest(ref chest, ItemID.GoldCoin, goldCoins);
            }
            else if (type == ModContent.TileType<RustyChestTile>())
            {
                // 15-29 torches (in accordence with vanilla)
                PutItemInChest(ref chest, ItemID.Torch, 15, 29);

                // 50% chance of 1 or 2 of the following potions
                int[] potions = new int[]
                {
                    ModContent.ItemType<HadalStew>(), ItemID.WaterWalkingPotion, ItemID.ShinePotion, ItemID.GillsPotion, ItemID.FlipperPotion
                };
                PutItemInChest(ref chest, ModContent.ItemType<SulphurskinPotion>(), 4, 7, WorldGen.genRand.NextBool());
                PutItemInChest(ref chest, WorldGen.genRand.Next(potions), 1, 2, WorldGen.genRand.NextBool());
                PutItemInChest(ref chest, WorldGen.genRand.Next(potions), 1, 2, WorldGen.genRand.NextBool());

                // 33% chance of flippers
                PutItemInChest(ref chest, ItemID.Flipper, condition: WorldGen.genRand.NextBool(3));

                // Typical coins
                PutItemInChest(ref chest, ItemID.GoldCoin, 2, 4);
            }
            // Default loot
            else
            {
                // Silver, Tungsten, Gold or Platinum bars (following worldgen choice)
                int barID = WorldGen.genRand.NextBool() ? GenVars.goldBar : GenVars.silverBar;
                PutItemInChest(ref chest, barID, 3, 10);

                // 50% chance of 25-50 Holy Arrows
                PutItemInChest(ref chest, ItemID.HolyArrow, 25, 50, WorldGen.genRand.NextBool());

                // 50% chance of 1 or 2 of the following potions
                int[] potions = new int[] {
                    ItemID.SpelunkerPotion, ItemID.FeatherfallPotion, ItemID.NightOwlPotion,
                    ItemID.WaterWalkingPotion, ItemID.ArcheryPotion, ItemID.GravitationPotion
                };
                PutItemInChest(ref chest, WorldGen.genRand.Next(potions), 1, 2, WorldGen.genRand.NextBool());

                // 50% chance of 1 or 2 of the following potions
                // Yes, in vanilla, Dangersense Potions have double the chance to appear.
                potions = new int[] {
                    ItemID.ThornsPotion, ItemID.WaterWalkingPotion, ItemID.InvisibilityPotion,
                    ItemID.ManaRegenerationPotion, ItemID.TeleportationPotion, ItemID.TrapsightPotion, ItemID.TrapsightPotion
                };

                PutItemInChest(ref chest, WorldGen.genRand.Next(potions), 1, 2, WorldGen.genRand.NextBool());
                PutItemInChest(ref chest, ItemID.RecallPotion, 1, 2, WorldGen.genRand.NextBool());
                PutItemInChest(ref chest, ItemID.GoldCoin, 1, 2);
            }
        }
        #endregion

        #region Tunnel Tools
        public static void CreateTunnel(int startX, int startY, int endX, int endY, int width = 6, int tileType = TileID.Dirt)
        {
            int dx = endX - startX;
            int dy = endY - startY;
            int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

            for (int i = 0; i <= steps; i++)
            {
                float t = (float)i / steps;
                int x = (int)(startX + dx * t);
                int y = (int)(startY + dy * t);

                // Draw a small circular path
                for (int xi = -width; xi <= width; xi++)
                {
                    for (int yi = -width; yi <= width; yi++)
                    {
                        if (xi * xi + yi * yi <= width * width)
                        {
                            int tileX = x + xi;
                            int tileY = y + yi;

                            if (WorldGen.InWorld(tileX, tileY))
                            {
                                Tile tTile = Main.tile[tileX, tileY];
                                tTile.HasTile = true;
                                tTile.TileType = (ushort)tileType;
                                WorldGen.SquareTileFrame(tileX, tileY);
                            }
                        }
                    }
                }
            }
        }
        public static void ClearTunnel(int startX, int startY, int endX, int endY, int width = 3, int wallType = WallID.Dirt, int liquidType = LiquidID.Water, bool liquidOn = true, bool wallOn = true)
        {
            int dx = endX - startX;
            int dy = endY - startY;
            int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

            for (int i = 0; i <= steps; i++)
            {
                float t = (float)i / steps;
                int x = (int)(startX + dx * t);
                int y = (int)(startY + dy * t);

                for (int xi = -width; xi <= width; xi++)
                {
                    for (int yi = -width; yi <= width; yi++)
                    {
                        if (xi * xi + yi * yi <= width * width)
                        {
                            int tileX = x + xi;
                            int tileY = y + yi;

                            if (WorldGen.InWorld(tileX, tileY))
                            {
                                Tile tTile = Main.tile[tileX, tileY];
                                tTile.HasTile = false;
                                if (wallOn)
                                {
                                    tTile.WallType = (ushort)wallType;
                                }

                                tTile.LiquidAmount = 0;

                                WorldGen.SquareWallFrame(tileX, tileY);

                                // Optional: Add liquid with low chance
                                if (liquidOn)
                                {
                                    if (WorldGen.genRand.NextBool(10))
                                    {
                                        tTile.LiquidAmount = 100;
                                        tTile.LiquidType = liquidType;
                                        WorldGen.SquareTileFrame(tileX, tileY);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Chasm Generator
        public static void ChasmGenerator(int i, int j, int steps, bool ocean = false)
        {
            float maxChasmSize = steps; //850 small 1450 medium 2050 large
            int limitIncrease = Main.remixWorld ? 110 : 0;
            if (ocean)
            {
                int tileYLookup = j;
                if (Abyss.AtLeftSideOfWorld)
                {
                    while (!Main.tile[i + 125, tileYLookup].HasTile)
                    {
                        tileYLookup++;
                    }
                }
                else
                {
                    while (!Main.tile[i - 125, tileYLookup].HasTile)
                    {
                        tileYLookup++;
                    }
                }
                j = tileYLookup;
            }
            Vector2 chasmGenPosition;
            chasmGenPosition.X = i;
            chasmGenPosition.Y = j;
            Vector2 randChasmGenOffset;
            randChasmGenOffset.X = WorldGen.genRand.Next(-1, 2) * 0.1f;
            randChasmGenOffset.Y = WorldGen.genRand.Next(3, 8) * 0.2f + 0.5f;
            int five = 5;
            double chasmWidth = WorldGen.genRand.Next(5, 7) + 20; //start width
            while (chasmWidth > 0.0)
            {
                if (maxChasmSize > 0f)
                {
                    chasmWidth += WorldGen.genRand.Next(10);
                    chasmWidth -= WorldGen.genRand.Next(10);
                    float smallHoleLimit = 790f; //small

                    if (Main.maxTilesY > 1500)
                    {
                        smallHoleLimit = 1360f;

                        if (Main.maxTilesY > 2100)
                        {
                            smallHoleLimit = 1950f;
                        }
                    }

                    if (ocean && maxChasmSize > smallHoleLimit)
                    {
                        if (chasmWidth < 7.0) //min width
                        {
                            chasmWidth = 7.0; //min width
                        }
                        if (chasmWidth > 45.0) //max width
                        {
                            chasmWidth = 45.0; //max width
                        }
                    }
                    else //dig large hole
                    {
                        if (chasmWidth < (ocean ? 30.0 : 8.0)) //min width
                        {
                            chasmWidth = ocean ? 30.0 : 8.0; //min width
                        }
                        if (chasmWidth > (ocean ? 70.0 : 20.0)) //max width
                        {
                            chasmWidth = ocean ? 70.0 : 20.0; //max width
                        }
                        if (maxChasmSize == 1f && chasmWidth < (ocean ? 50.0 : 15.0))
                        {
                            chasmWidth = ocean ? 50.0 : 15.0;
                        }
                    }
                }
                else
                {
                    if ((double)chasmGenPosition.Y > (Abyss.AbyssChasmBottom + limitIncrease))
                    {
                        chasmWidth -= WorldGen.genRand.Next(5) + 8;
                    }
                }
                if (Main.maxTilesY > 2100)
                {
                    if (((double)chasmGenPosition.Y > (Abyss.AbyssChasmBottom + limitIncrease) && maxChasmSize > 0f && ocean) ||
                    (chasmGenPosition.Y >= Main.maxTilesY && maxChasmSize > 0f && !ocean))
                    {
                        maxChasmSize = 0f;
                    }
                }
                else if (Main.maxTilesY > 1500)
                {
                    if (((double)chasmGenPosition.Y > (Abyss.AbyssChasmBottom + limitIncrease) && maxChasmSize > 0f && ocean) ||
                    (chasmGenPosition.Y > Main.maxTilesY && maxChasmSize > 0f && !ocean))
                    {
                        maxChasmSize = 0f;
                    }
                }
                else
                {
                    if (((double)chasmGenPosition.Y > (Abyss.AbyssChasmBottom + limitIncrease) && maxChasmSize > 0f && ocean) ||
                    (chasmGenPosition.Y > Main.maxTilesY && maxChasmSize > 0f && !ocean))
                    {
                        maxChasmSize = 0f;
                    }
                }

                maxChasmSize -= 1f;
                int chasmWidthMin;
                int chasmWidthMax;
                int chasmHeightMin;
                int chasmHeightMax;
                if (maxChasmSize > five)
                {
                    chasmWidthMin = (int)(chasmGenPosition.X - chasmWidth * 0.5);
                    chasmWidthMax = (int)(chasmGenPosition.X + chasmWidth * 0.5);
                    chasmHeightMin = (int)(chasmGenPosition.Y - chasmWidth * 0.5);
                    chasmHeightMax = (int)(chasmGenPosition.Y + chasmWidth * 0.5);
                    if (chasmWidthMin < 0)
                    {
                        chasmWidthMin = 0;
                    }
                    if (chasmWidthMax > Main.maxTilesX - 1)
                    {
                        chasmWidthMax = Main.maxTilesX - 1;
                    }
                    if (chasmHeightMin < 0)
                    {
                        chasmHeightMin = 0;
                    }
                    if (chasmHeightMax > Main.maxTilesY)
                    {
                        chasmHeightMax = Main.maxTilesY;
                    }
                    for (int k = chasmWidthMin; k < chasmWidthMax; k++)
                    {
                        for (int l = chasmHeightMin; l < chasmHeightMax; l++)
                        {
                            if ((Math.Abs(k - chasmGenPosition.X) + Math.Abs(l - chasmGenPosition.Y)) < chasmWidth * 0.5 * (1.0 + WorldGen.genRand.Next(-5, 6) * 0.015))
                            {
                                if (ocean)
                                {
                                    Main.tile[k, l].Get<TileWallWireStateData>().HasTile = false;
                                    Main.tile[k, l].LiquidAmount = 255;
                                    Main.tile[k, l].Get<LiquidData>().LiquidType = LiquidID.Water;
                                }
                                else
                                {
                                    Main.tile[k, l].Get<TileWallWireStateData>().HasTile = false;
                                    Main.tile[k, l].LiquidAmount = 255;
                                    Main.tile[k, l].Get<LiquidData>().LiquidType = LiquidID.Lava;
                                }
                            }
                        }
                    }
                }
                /*if (maxChasmSize <= 2f && chasmGenPosition.Y < (Main.rockLayer + Main.maxTilesY * 0.3))
                {
                    maxChasmSize = 2f;
                }*/
                chasmGenPosition += randChasmGenOffset;
                randChasmGenOffset.X += WorldGen.genRand.Next(-1, 2) * 0.01f;
                if (randChasmGenOffset.X > 0.02)
                {
                    randChasmGenOffset.X = 0.02f;
                }
                if (randChasmGenOffset.X < -0.02)
                {
                    randChasmGenOffset.X = -0.02f;
                }
                chasmWidthMin = (int)(chasmGenPosition.X - chasmWidth * 1.1);
                chasmWidthMax = (int)(chasmGenPosition.X + chasmWidth * 1.1);
                chasmHeightMin = (int)(chasmGenPosition.Y - chasmWidth * 1.1);
                chasmHeightMax = (int)(chasmGenPosition.Y + chasmWidth * 1.1);
                if (chasmWidthMin < 1)
                {
                    chasmWidthMin = 1;
                }
                if (chasmWidthMax > Main.maxTilesX - 1)
                {
                    chasmWidthMax = Main.maxTilesX - 1;
                }
                if (chasmHeightMin < 0)
                {
                    chasmHeightMin = 0;
                }
                if (chasmHeightMax > Main.maxTilesY)
                {
                    chasmHeightMax = Main.maxTilesY;
                }
                for (int m = chasmWidthMin; m < chasmWidthMax; m++)
                {
                    for (int n = chasmHeightMin; n < chasmHeightMax; n++)
                    {
                        if ((Math.Abs(m - chasmGenPosition.X) + Math.Abs(n - chasmGenPosition.Y)) < chasmWidth * 1.1 * (1.0 + WorldGen.genRand.Next(-5, 6) * 0.015))
                        {
                            if (n > j + WorldGen.genRand.Next(7, 16))
                            {
                                Main.tile[m, n].Get<TileWallWireStateData>().HasTile = false;
                            }
                            if (steps <= five)
                            {
                                Main.tile[m, n].Get<TileWallWireStateData>().HasTile = false;
                            }
                            if (ocean)
                            {
                                Main.tile[m, n].LiquidAmount = 255;
                                Main.tile[m, n].Get<LiquidData>().LiquidType = LiquidID.Water;
                            }
                            else
                            {
                                Main.tile[m, n].LiquidAmount = 255;
                                Main.tile[m, n].Get<LiquidData>().LiquidType = LiquidID.Lava;
                            }
                        }
                    }
                }
                for (int r = chasmWidthMin; r < chasmWidthMax; r++)
                {
                    for (int s = chasmHeightMin; s < chasmHeightMax; s++)
                    {
                        if ((Math.Abs(r - chasmGenPosition.X) + Math.Abs(s - chasmGenPosition.Y)) < chasmWidth * 1.1 * (1.0 + WorldGen.genRand.Next(-5, 6) * 0.015))
                        {
                            if (ocean)
                            {
                                Main.tile[r, s].LiquidAmount = 255;
                                Main.tile[r, s].Get<LiquidData>().LiquidType = LiquidID.Water;
                            }
                            else
                            {
                                Main.tile[r, s].LiquidAmount = 255;
                                Main.tile[r, s].Get<LiquidData>().LiquidType = LiquidID.Lava;
                            }
                            if (steps <= five)
                            {
                                Main.tile[r, s].Get<TileWallWireStateData>().HasTile = false;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Smart Gem Gen
        public static void SmartGemGen()
        {
            double oneThirdOfUnderground = (Main.UnderworldLayer - Main.worldSurface) / 3D;
            double verticalStartFactor_Layer1 = Main.worldSurface;
            double verticalStartFactor_Layer2 = verticalStartFactor_Layer1 + oneThirdOfUnderground;
            double verticalStartFactor_Layer3 = verticalStartFactor_Layer1 + oneThirdOfUnderground * 2D;
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    if (y > verticalStartFactor_Layer3)
                    {
                        if (Main.tile[x, y] != null)
                        {
                            if (Main.remixWorld)
                            {
                                if (Main.tile[x, y].TileType == TileID.Diamond || Main.tile[x, y].TileType == TileID.Ruby)
                                    Main.tile[x, y].TileType = TileID.Topaz;
                                else if (Main.tile[x, y].TileType == TileID.Emerald || Main.tile[x, y].TileType == TileID.Sapphire)
                                    Main.tile[x, y].TileType = TileID.Amethyst;
                            }
                            else
                            {
                                if (Main.tile[x, y].TileType == TileID.Emerald || Main.tile[x, y].TileType == TileID.Sapphire)
                                    Main.tile[x, y].TileType = TileID.Diamond;
                                else if (Main.tile[x, y].TileType == TileID.Topaz || Main.tile[x, y].TileType == TileID.Amethyst)
                                    Main.tile[x, y].TileType = TileID.Ruby;
                            }
                        }
                    }
                    else if (y > verticalStartFactor_Layer2)
                    {
                        if (Main.tile[x, y] != null)
                        {
                            if (Main.tile[x, y].TileType == TileID.Diamond || Main.tile[x, y].TileType == TileID.Ruby)
                                Main.tile[x, y].TileType = TileID.Emerald;
                            else if (Main.tile[x, y].TileType == TileID.Topaz || Main.tile[x, y].TileType == TileID.Amethyst)
                                Main.tile[x, y].TileType = TileID.Sapphire;
                        }
                    }
                    else if (y > verticalStartFactor_Layer1)
                    {
                        if (Main.tile[x, y] != null)
                        {
                            if (Main.remixWorld)
                            {
                                if (Main.tile[x, y].TileType == TileID.Emerald || Main.tile[x, y].TileType == TileID.Sapphire)
                                    Main.tile[x, y].TileType = TileID.Diamond;
                                else if (Main.tile[x, y].TileType == TileID.Topaz || Main.tile[x, y].TileType == TileID.Amethyst)
                                    Main.tile[x, y].TileType = TileID.Ruby;
                            }
                            else
                            {
                                if (Main.tile[x, y].TileType == TileID.Diamond || Main.tile[x, y].TileType == TileID.Ruby)
                                    Main.tile[x, y].TileType = TileID.Topaz;
                                else if (Main.tile[x, y].TileType == TileID.Emerald || Main.tile[x, y].TileType == TileID.Sapphire)
                                    Main.tile[x, y].TileType = TileID.Amethyst;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Auric Land Mines
        public static void GenerateAuricLandMines()
        {
            int landMineID = ModContent.TileType<AuricLandMineTile>();
            int landMineChance = Main.zenithWorld ? 125 : 300;
            float maxDepth = Main.maxTilesY * (Main.zenithWorld ? 0.75f : 0.5f); // depth increased in gfb due to the evil columns that extend further downward
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < maxDepth; y++)
                {
                    Tile t = CalamityUtils.ParanoidTileRetrieval(x, y);
                    Tile above = CalamityUtils.ParanoidTileRetrieval(x, y - 1);
                    if (t != null)
                    {
                        if (above != null)
                        {
                            // Yharim killed the gods with auric land mines obviously
                            if ((t.TileType == TileID.Ebonstone || t.TileType == TileID.Crimstone) && !above.HasTile)
                            {
                                if (WorldGen.genRand.NextBool(landMineChance))
                                {
                                    WorldGen.SlopeTile(x, y);
                                    WorldGen.PlaceTile(x, y - 1, landMineID);
                                }
                            }
                        }
                    }                    
                }
            }
        }
        #endregion

        #region Iron Ball
        public static void GenerateIronBall()
        {
            int ballID = ModContent.TileType<IronBallPlaced>();
            int attempts = 0;
            int ballsPlaced = 0;
            int worldSize = WorldGen.GetWorldSize();
            // Same amount as Chillet
            int maxBalls = worldSize switch
            {
                1 => 9,
                2 => 12,
                _ => 6,
            };
            while (attempts < 10000)
            {
                Point location = new Point(WorldGen.genRand.Next(100, Main.maxTilesX - 100), WorldGen.genRand.Next((int)Main.rockLayer, Main.UnderworldLayer));
                Tile t = CalamityUtils.ParanoidTileRetrieval(location.X, location.Y);
                Tile above = CalamityUtils.ParanoidTileRetrieval(location.X, location.Y - 1);
                if (t.TileType == TileID.Stone && t.HasTile && !above.HasTile)
                {
                    WorldGen.SlopeTile(location.X, location.Y);
                    WorldGen.PlaceTile(location.X, location.Y - 1, ballID);
                    ballsPlaced++;
                }
                if (ballsPlaced >= maxBalls)
                    break;
            }
        }
        #endregion
    }
}
