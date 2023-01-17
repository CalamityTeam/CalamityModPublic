using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Tiles;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.FurnitureAncient;
using CalamityMod.Tiles.Ores;
using CalamityMod.Walls;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace CalamityMod.World
{
    public class MiscWorldgenRoutines
    {
        #region Dungeon Biome Chests
        public static void GenerateBiomeChests(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Adding a new Biome Chest";

            // Get dungeon size field infos.
            int MinX = (int)typeof(WorldGen).GetField("dMinX", BindingFlags.Static | BindingFlags.Public).GetValue(null) + 25;
            int MaxX = (int)typeof(WorldGen).GetField("dMaxX", BindingFlags.Static | BindingFlags.Public).GetValue(null) - 25;
            int MaxY = (int)typeof(WorldGen).GetField("dMaxY", BindingFlags.Static | BindingFlags.Public).GetValue(null) - 25;

            int[] ChestTypes = { ModContent.TileType<AstralChestLocked>() };
            int[] ItemTypes = { ModContent.ItemType<HeavenfallenStardisk>() };
            int[] ChestStyles = { 1 }; // Astral Chest generates in style 1, which is locked

            for (int i = 0; i < ChestTypes.Length; ++i)
            {
                Chest chest = null;
                int attempts = 0;

                // Try 1000 times to place the chest somewhere in the dungeon.
                // The placement algorithm ensures that if it tries to appear in midair, it is moved down to the floor.
                while (chest == null && attempts < 1000)
                {
                    attempts++;
                    int x = WorldGen.genRand.Next(MinX, MaxX);
                    int y = WorldGen.genRand.Next((int)Main.worldSurface, MaxY);
                    if (Main.wallDungeon[Main.tile[x, y].WallType] && !Main.tile[x, y].HasTile)
                        chest = AddChestWithLoot(x, y, (ushort)ChestTypes[i], tileStyle: ChestStyles[i]);
                }

                // If a chest was placed, force its first item to be the unique Biome Chest weapon.
                if (chest != null)
                {
                    chest.item[0].SetDefaults(ItemTypes[i]);
                    chest.item[0].Prefix(-1);
                }
            }
        }

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

                // Don't set quantity unless quantity is specified
                if(minQuantity > 0)
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
                PutItemInChest(ref chest, ModContent.ItemType<Stardust>(), 30, 80);
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
                int barID = WorldGen.genRand.NextBool() ? WorldGen.goldBar : WorldGen.silverBar;
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

        #region Giant Hive
        public static bool CanPlaceGiantHive(Point origin, StructureMap structures)
        {
            if (!structures.CanPlace(new Rectangle(origin.X - 50, origin.Y - 50, 100, 100)))
                return false;

            if (TooCloseToImportantLocations(origin))
                return false;

            Ref<int> ref1 = new Ref<int>(0);
            Ref<int> ref2 = new Ref<int>(0);
            Ref<int> ref3 = new Ref<int>(0);
            WorldUtils.Gen(origin, new Shapes.Circle(15), Actions.Chain(new Modifiers.IsSolid(), new Actions.Scanner(ref1), new Modifiers.OnlyTiles(60, 59), new Actions.Scanner(ref2), new Modifiers.OnlyTiles(60), new Actions.Scanner(ref3)));
            if (ref2.Value / (float)ref1.Value < 0.75f || ref3.Value < 2)
                return false;

            int num = 0;
            int[] array = new int[1000];
            int[] array2 = new int[1000];
            Vector2 larvaLocation = origin.ToVector2();
            int numHiveTunnels = WorldGen.genRand.Next(10, 13);

            for (int i = 0; i < numHiveTunnels; i++)
            {
                Vector2 vector2 = larvaLocation;
                int num3 = WorldGen.genRand.Next(2, 5);
                for (int j = 0; j < num3; j++)
                    vector2 = CreateHiveTunnel((int)larvaLocation.X, (int)larvaLocation.Y, WorldGen.genRand);

                larvaLocation = vector2;
                array[num] = (int)larvaLocation.X;
                array2[num] = (int)larvaLocation.Y;
                num++;
            }

            FrameOutAllHiveContents(origin, 50);

            for (int k = 0; k < num; k++)
            {
                int num4 = array[k];
                int y = array2[k];
                int num5 = 1;
                if (WorldGen.genRand.NextBool())
                    num5 = -1;

                bool flag = false;
                while (WorldGen.InWorld(num4, y, 10) && BadSpotForHoneyFall(num4, y))
                {
                    num4 += num5;
                    if (Math.Abs(num4 - array[k]) > 50)
                    {
                        flag = true;
                        break;
                    }
                }

                if (!flag)
                {
                    num4 += num5;
                    if (!SpotActuallyNotInHive(num4, y))
                    {
                        CreateBlockedHoneyCube(num4, y);
                        CreateDentForHoneyFall(num4, y, num5);
                    }
                }
            }

            CreateStandForLarva(larvaLocation);

            // Generate a second larva stand
            Vector2 secondLarvaLocation = default;
            int maxAttempts = 1000;
            for (int l = 0; l < maxAttempts; l++)
            {
                Vector2 vector3 = larvaLocation;
                vector3.X += WorldGen.genRand.Next(-50, 51);
                vector3.Y += WorldGen.genRand.Next(-50, 51);
                if (WorldGen.InWorld((int)vector3.X, (int)vector3.Y) && Vector2.Distance(larvaLocation, vector3) > 10f && !Main.tile[(int)vector3.X, (int)vector3.Y].HasTile && Main.tile[(int)vector3.X, (int)vector3.Y].WallType == WallID.HiveUnsafe)
                {
                    secondLarvaLocation = vector3;
                    CreateStandForLarva(vector3);
                    break;
                }
            }

            // Honey chests
            Vector2 honeyChestLocation = default;
            for (int l = 0; l < maxAttempts; l++)
            {
                Vector2 vector3 = larvaLocation;
                vector3.X += WorldGen.genRand.Next(-100, 101);
                vector3.Y += WorldGen.genRand.Next(-100, 101);
                if (WorldGen.InWorld((int)vector3.X, (int)vector3.Y) && Vector2.Distance(larvaLocation, vector3) > 10f && Vector2.Distance(secondLarvaLocation, vector3) > 10f && !Main.tile[(int)vector3.X, (int)vector3.Y].HasTile && Main.tile[(int)vector3.X, (int)vector3.Y].WallType == WallID.HiveUnsafe)
                {
                    honeyChestLocation = vector3;
                    CreateStandAndPlaceHoneyChest(vector3);
                    break;
                }
            }
            for (int l = 0; l < maxAttempts; l++)
            {
                Vector2 vector3 = larvaLocation;
                vector3.X += WorldGen.genRand.Next(-100, 101);
                vector3.Y += WorldGen.genRand.Next(-100, 101);
                if (WorldGen.InWorld((int)vector3.X, (int)vector3.Y) && Vector2.Distance(larvaLocation, vector3) > 10f && Vector2.Distance(secondLarvaLocation, vector3) > 10f && Vector2.Distance(honeyChestLocation, vector3) > 10f && !Main.tile[(int)vector3.X, (int)vector3.Y].HasTile && Main.tile[(int)vector3.X, (int)vector3.Y].WallType == WallID.HiveUnsafe)
                {
                    CreateStandAndPlaceHoneyChest(vector3);
                    break;
                }
            }

            structures.AddProtectedStructure(new Rectangle(origin.X - 50, origin.Y - 50, 100, 100), 5);
            return true;
        }

        private static void FrameOutAllHiveContents(Point origin, int squareHalfWidth)
        {
            int num = Math.Max(10, origin.X - squareHalfWidth);
            int num2 = Math.Min(Main.maxTilesX - 10, origin.X + squareHalfWidth);
            int num3 = Math.Max(10, origin.Y - squareHalfWidth);
            int num4 = Math.Min(Main.maxTilesY - 10, origin.Y + squareHalfWidth);
            for (int i = num; i < num2; i++)
            {
                for (int j = num3; j < num4; j++)
                {
                    Tile tile = Main.tile[i, j];
                    if (tile.HasTile && tile.TileType == TileID.Hive)
                        WorldGen.SquareTileFrame(i, j);

                    if (tile.WallType == WallID.HiveUnsafe)
                        WorldGen.SquareWallFrame(i, j);
                }
            }
        }

        private static Vector2 CreateHiveTunnel(int i, int j, UnifiedRandom random)
        {
            double num = random.Next(20, 26);
            float num2 = random.Next(30, 41);
            float num3 = Main.maxTilesX / 4200;
            num3 = (num3 + 1f) / 2f;
            num *= (double)num3;
            num2 *= num3;

            double num4 = num;
            Vector2 result = default;
            result.X = i;
            result.Y = j;
            Vector2 vector = default;
            vector.X = random.Next(-10, 11) * 0.2f;
            vector.Y = random.Next(-10, 11) * 0.2f;
            while (num > 0.0 && num2 > 0f)
            {
                if (result.Y > (Main.maxTilesY - 250))
                    num2 = 0f;

                num = num4 * (double)(1f + random.Next(-20, 20) * 0.01f);
                num2 -= 1f;
                int num5 = (int)(result.X - num);
                int num6 = (int)(result.X + num);
                int num7 = (int)(result.Y - num);
                int num8 = (int)(result.Y + num);
                if (num5 < 1)
                    num5 = 1;

                if (num6 > Main.maxTilesX - 1)
                    num6 = Main.maxTilesX - 1;

                if (num7 < 1)
                    num7 = 1;

                if (num8 > Main.maxTilesY - 1)
                    num8 = Main.maxTilesY - 1;

                for (int k = num5; k < num6; k++)
                {
                    for (int l = num7; l < num8; l++)
                    {
                        if (!WorldGen.InWorld(k, l, 50))
                        {
                            num2 = 0f;
                        }
                        else
                        {
                            if (Main.tile[k - 10, l].WallType == WallID.LihzahrdBrickUnsafe)
                                num2 = 0f;

                            if (Main.tile[k + 10, l].WallType == WallID.LihzahrdBrickUnsafe)
                                num2 = 0f;

                            if (Main.tile[k, l - 10].WallType == WallID.LihzahrdBrickUnsafe)
                                num2 = 0f;

                            if (Main.tile[k, l + 10].WallType == WallID.LihzahrdBrickUnsafe)
                                num2 = 0f;
                        }

                        if (l < Main.worldSurface && Main.tile[k, l - 5].WallType == WallID.None)
                            num2 = 0f;

                        float num9 = Math.Abs(k - result.X);
                        float num10 = Math.Abs(l - result.Y);
                        double num11 = Math.Sqrt(num9 * num9 + num10 * num10);
                        if (num11 < num4 * 0.4 * (1.0 + random.Next(-10, 11) * 0.005))
                        {
                            Main.tile[k, l].LiquidAmount = byte.MaxValue;
                            Main.tile[k, l].Get<LiquidData>().LiquidType = LiquidID.Honey;
                            Main.tile[k, l].WallType = WallID.HiveUnsafe;
                            Main.tile[k, l].Get<TileWallWireStateData>().HasTile = false;
                            Main.tile[k, l].Get<TileWallWireStateData>().IsHalfBlock = false;
                            Main.tile[k, l].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                        }
                        else if (num11 < num4 * 0.75 * (1.0 + random.Next(-10, 11) * 0.005))
                        {
                            Main.tile[k, l].LiquidAmount = 0;
                            if (Main.tile[k, l].WallType != WallID.HiveUnsafe)
                            {
                                Main.tile[k, l].Get<TileWallWireStateData>().HasTile = true;
                                Main.tile[k, l].Get<TileWallWireStateData>().IsHalfBlock = false;
                                Main.tile[k, l].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                                Main.tile[k, l].TileType = TileID.Hive;
                            }
                        }

                        if (num11 < num4 * 0.6 * (1.0 + random.Next(-10, 11) * 0.005))
                        {
                            Main.tile[k, l].WallType = WallID.HiveUnsafe;
                            if (random.NextBool())
                            {
                                Main.tile[k, l].LiquidAmount = byte.MaxValue;
                                Main.tile[k, l].Get<LiquidData>().LiquidType = LiquidID.Honey;
                            }
                        }
                    }
                }

                result += vector;
                num2 -= 1f;
                vector.Y += random.Next(-10, 11) * 0.05f;
                vector.X += random.Next(-10, 11) * 0.05f;
            }

            return result;
        }

        private static bool TooCloseToImportantLocations(Point origin)
        {
            int x = origin.X;
            int y = origin.Y;
            int num = 150;
            for (int i = x - num; i < x + num; i += 10)
            {
                if (i <= 0 || i > Main.maxTilesX - 1)
                    continue;

                for (int j = y - num; j < y + num; j += 10)
                {
                    if (j > 0 && j <= Main.maxTilesY - 1)
                    {
                        if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.LihzahrdBrick)
                            return true;

                        if (Main.tile[i, j].WallType == WallID.CrimstoneUnsafe || Main.tile[i, j].WallType == WallID.EbonstoneUnsafe || Main.tile[i, j].WallType == WallID.LihzahrdBrickUnsafe)
                            return true;
                    }
                }
            }

            return false;
        }

        private static void CreateDentForHoneyFall(int x, int y, int dir)
        {
            dir *= -1;
            y++;
            int num = 0;
            while ((num < 4 || WorldGen.SolidTile(x, y)) && x > 10 && x < Main.maxTilesX - 10)
            {
                num++;
                x += dir;
                if (WorldGen.SolidTile(x, y))
                {
                    WorldGen.PoundTile(x, y);
                    if (!Main.tile[x, y + 1].HasTile)
                    {
                        Main.tile[x, y + 1].Get<TileWallWireStateData>().HasTile = true;
                        Main.tile[x, y + 1].TileType = TileID.Hive;
                    }
                }
            }
        }

        private static void CreateBlockedHoneyCube(int x, int y)
        {
            for (int i = x - 1; i <= x + 2; i++)
            {
                for (int j = y - 1; j <= y + 2; j++)
                {
                    if (i >= x && i <= x + 1 && j >= y && j <= y + 1)
                    {
                        Main.tile[i, j].Get<TileWallWireStateData>().HasTile = false;
                        Main.tile[i, j].LiquidAmount = byte.MaxValue;
                        Main.tile[i, j].Get<LiquidData>().LiquidType = LiquidID.Honey;
                    }
                    else
                    {
                        Main.tile[i, j].Get<TileWallWireStateData>().HasTile = true;
                        Main.tile[i, j].TileType = TileID.Hive;
                    }
                }
            }
        }

        private static bool SpotActuallyNotInHive(int x, int y)
        {
            for (int i = x - 1; i <= x + 2; i++)
            {
                for (int j = y - 1; j <= y + 2; j++)
                {
                    if (i < 10 || i > Main.maxTilesX - 10)
                        return true;

                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType != TileID.Hive)
                        return true;
                }
            }

            return false;
        }

        private static bool BadSpotForHoneyFall(int x, int y)
        {
            if (Main.tile[x, y].HasTile && Main.tile[x, y + 1].HasTile && Main.tile[x + 1, y].HasTile)
                return !Main.tile[x + 1, y + 1].HasTile;

            return true;
        }

        public static void CreateStandForLarva(Vector2 position)
        {
            WorldGen.larvaX[WorldGen.numLarva] = Utils.Clamp((int)position.X, 5, Main.maxTilesX - 5);
            WorldGen.larvaY[WorldGen.numLarva] = Utils.Clamp((int)position.Y, 5, Main.maxTilesY - 5);
            WorldGen.numLarva++;
            if (WorldGen.numLarva >= WorldGen.larvaX.Length)
                WorldGen.numLarva = WorldGen.larvaX.Length - 1;

            int num = (int)position.X;
            int num2 = (int)position.Y;
            for (int i = num - 1; i <= num + 1 && i > 0 && i < Main.maxTilesX; i++)
            {
                for (int j = num2 - 2; j <= num2 + 1 && j > 0 && j < Main.maxTilesY; j++)
                {
                    if (j != num2 + 1)
                    {
                        Main.tile[i, j].Get<TileWallWireStateData>().HasTile = false;
                        continue;
                    }

                    Main.tile[i, j].Get<TileWallWireStateData>().HasTile = true;
                    Main.tile[i, j].TileType = TileID.Hive;
                    Main.tile[i, j].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                    Main.tile[i, j].Get<TileWallWireStateData>().IsHalfBlock = false;
                }
            }
        }

        public static void CreateStandAndPlaceHoneyChest(Vector2 position)
        {
            int chestPlacementX = Utils.Clamp((int)position.X, 5, Main.maxTilesX - 5);
            int chestPlacementY = Utils.Clamp((int)position.Y, 5, Main.maxTilesY - 5);

            int num = (int)position.X;
            int num2 = (int)position.Y;
            for (int i = num; i <= num + 1 && i > 0 && i < Main.maxTilesX; i++)
            {
                for (int j = num2 - 1; j <= num2 + 1 && j > 0 && j < Main.maxTilesY; j++)
                {
                    if (j != num2 + 1)
                    {
                        Main.tile[i, j].Get<TileWallWireStateData>().HasTile = false;
                        continue;
                    }

                    Main.tile[i, j].Get<TileWallWireStateData>().HasTile = true;
                    Main.tile[i, j].TileType = TileID.Hive;
                    Main.tile[i, j].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                    Main.tile[i, j].Get<TileWallWireStateData>().IsHalfBlock = false;
                }
            }

            int num144 = chestPlacementX;
            int num145 = chestPlacementY;
            for (int num146 = num144; num146 <= num144 + 1; num146++)
            {
                for (int num147 = num145 - 1; num147 <= num145 + 1; num147++)
                {
                    if (num147 != num145 + 1)
                    {
                        Main.tile[num146, num147].Get<TileWallWireStateData>().HasTile = false;
                    }
                    else
                    {
                        Main.tile[num146, num147].Get<TileWallWireStateData>().HasTile = true;
                        Main.tile[num146, num147].TileType = TileID.Hive;
                        Main.tile[num146, num147].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                        Main.tile[num146, num147].Get<TileWallWireStateData>().IsHalfBlock = false;
                    }
                }
            }

            int chestID = WorldGen.PlaceChest(num144, num145, 21, false, 29);
            FillHoneyChest(chestID, WorldGen.genRand);
        }

        // Honey chest stuff, this is also used by the Hive Planetoid
        private static int[] FocusLootHoney = new int[]
        {
            ItemID.NaturesGift,
            ItemID.Bezoar,
            ItemID.FlowerBoots,
            ItemID.BeeMinecart
        };

        private static int[] PotionLootHoney = new int[]
        {
            ItemID.LifeforcePotion,
            ItemID.RegenerationPotion,
            ItemID.ManaRegenerationPotion,
            ItemID.HeartreachPotion,
            ModContent.ItemType<PhotosynthesisPotion>()
        };

        private static int[] BarLootHoney = new int[]
        {
            WorldGen.silverBar == TileID.Silver ? ItemID.SilverBar : ItemID.TungstenBar,
            WorldGen.goldBar == TileID.Gold ? ItemID.GoldBar : ItemID.PlatinumBar
        };

        public static void FillHoneyChest(int id, UnifiedRandom random)
        {
            if (id < 0)
                return;

            Chest chest = Main.chest[id];
            if (chest == null)
                return;

            int index = 0;

            chest.item[index++].SetDefaults(random.Next(FocusLootHoney));

            if (random.Next(3) <= 1)
            {
                chest.item[index].SetDefaults(random.Next(BarLootHoney));
                chest.item[index].SetDefaults(random.Next(7, 15));
            }
            else
            {
                chest.item[index].SetDefaults(ItemID.GoldCoin);
                chest.item[index++].stack = random.Next(3, 5);
            }

            if (random.NextBool())
            {
                chest.item[index].SetDefaults(random.Next(PotionLootHoney));
                chest.item[index++].stack = random.Next(1, 4);
            }
            else
            {
                chest.item[index].SetDefaults(ItemID.BottledHoney);
                chest.item[index++].stack = random.Next(3, 7);
            }

            if (random.NextBool())
            {
                chest.item[index].SetDefaults(ItemID.Stinger);
                chest.item[index++].stack = random.Next(4, 6);
            }
            else
            {
                chest.item[index].SetDefaults(ItemID.JungleSpores);
                chest.item[index++].stack = random.Next(3, 5);
            }

            if (random.NextBool())
            {
                chest.item[index].SetDefaults(ItemID.RecallPotion);
                chest.item[index++].stack = random.Next(1, 4);
            }
            else
            {
                chest.item[index].SetDefaults(ItemID.JungleTorch);
                chest.item[index++].stack = random.Next(18, 36);
            }

        }
        #endregion

        #region Place Rox Shrine
        public static void PlaceRoxShrine()
        {
            while (!CalamityWorld.roxShrinePlaced)
            {
                CalamityWorld.roxShrinePlaced = true;
                for (int x = 0; x < Main.maxTilesX; x++)
                {
                    for (int y = 0; y < Main.maxTilesY; y++)
                    {
                        if (Main.tile[x, y] != null && Main.tile[x, y].TileType == TileID.LargePiles)
                        {
                            if ((Main.tile[x, y].TileFrameX == 18 && Main.tile[x, y].TileFrameY == 0) || (Main.tile[x, y].TileFrameX == 45 && Main.tile[x, y].TileFrameY == 0))
                            {
                                if (WorldGen.genRand.NextBool(3))
                                {
                                    for (int dx = -1; dx < 2; dx++)
                                    {
                                        for (int dy = -1; dy < 2; dy++)
                                            Main.tile[x + dx, y + dy].Get<TileWallWireStateData>().HasTile = false;
                                    }

                                    WorldGen.PlaceTile(x, y + 1, ModContent.TileType<RoxTile>());
                                    return;
                                }
                                else
                                    CalamityWorld.roxShrinePlaced = false;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        // NOTE - The crag gen is scheduled for a rework soon. As such, its code will not be moved.

        #region UnderworldIsland
        public static void UnderworldIsland(int i, int j, int sizeMin, int sizeMax, int sizeMin2, int sizeMax2)
        {
            double num = (double)WorldGen.genRand.Next(sizeMin, sizeMax); //100 150
            float num2 = (float)WorldGen.genRand.Next(sizeMin / 5, sizeMax / 5); //20 30
            int num3 = i;
            int num4 = i;
            int num5 = i;
            int num6 = j;
            Vector2 vector;
            vector.X = (float)i;
            vector.Y = (float)j;
            Vector2 vector2;
            vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            while (vector2.X > -2f && vector2.X < 2f)
            {
                vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            }
            vector2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.02f;
            while (num > 0.0 && num2 > 0f)
            {
                num -= (double)WorldGen.genRand.Next(4);
                num2 -= 1f;
                int num7 = (int)((double)vector.X - num * 0.5);
                int num8 = (int)((double)vector.X + num * 0.5);
                int num9 = (int)((double)vector.Y - num * 0.5);
                int num10 = (int)((double)vector.Y + num * 0.5);
                if (num7 < 0)
                {
                    num7 = 0;
                }
                if (num8 > Main.maxTilesX)
                {
                    num8 = Main.maxTilesX;
                }
                if (num9 < Main.maxTilesY - 270)
                {
                    num9 = Main.maxTilesY - 270;
                }
                if (num10 > Main.maxTilesY)
                {
                    num10 = Main.maxTilesY;
                }
                double num11 = num * (double)WorldGen.genRand.Next(sizeMin, sizeMax) * 0.01; //80 120
                float num12 = vector.Y + 1f;
                for (int k = num7; k < num8; k++)
                {
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num12 += (float)WorldGen.genRand.Next(-1, 2);
                    }
                    if (num12 < vector.Y)
                    {
                        num12 = vector.Y;
                    }
                    if (num12 > vector.Y + 2f)
                    {
                        num12 = vector.Y + 2f;
                    }
                    for (int l = num9; l < num10; l++)
                    {
                        if ((float)l > num12)
                        {
                            float arg_218_0 = Math.Abs((float)k - vector.X);
                            float num13 = Math.Abs((float)l - vector.Y) * 3f;
                            if (Math.Sqrt((double)(arg_218_0 * arg_218_0 + num13 * num13)) < num11 * 0.4)
                            {
                                if (k < num3)
                                {
                                    num3 = k;
                                }
                                if (k > num4)
                                {
                                    num4 = k;
                                }
                                if (l < num5)
                                {
                                    num5 = l;
                                }
                                if (l > num6)
                                {
                                    num6 = l;
                                }
                                Main.tile[k, l].Get<TileWallWireStateData>().HasTile = true;
                                Main.tile[k, l].TileType = (ushort)ModContent.TileType<BrimstoneSlag>();
                                WorldGen.SquareTileFrame(k, l, true);
                            }
                        }
                    }
                }
                vector += vector2;
                vector2.X += (float)WorldGen.genRand.Next(-20, 21) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if ((double)vector2.Y > 0.2)
                {
                    vector2.Y = -0.2f;
                }
                if ((double)vector2.Y < -0.2)
                {
                    vector2.Y = -0.2f;
                }
            }
            int m = num3;
            int num15;
            for (m += WorldGen.genRand.Next(5); m < num4; m += WorldGen.genRand.Next(num15, (int)((double)num15 * 1.5)))
            {
                int num14 = num6;
                while (!Main.tile[m, num14].HasTile)
                {
                    num14--;
                    if (num14 < Main.maxTilesX - WorldGen.genRand.Next(265, 275 + 1))
                        break;
                }
                num14 += WorldGen.genRand.Next(-3, 4);
                num15 = WorldGen.genRand.Next(4, 8);
                int num16 = ModContent.TileType<BrimstoneSlag>();
                if (WorldGen.genRand.Next(4) == 0)
                {
                    num16 = ModContent.TileType<CharredOre>();
                }
                for (int n = m - num15; n <= m + num15; n++)
                {
                    for (int num17 = num14 - num15; num17 <= num14 + num15; num17++)
                    {
                        if (num17 > num5)
                        {
                            float arg_409_0 = (float)Math.Abs(n - m);
                            float num18 = (float)(Math.Abs(num17 - num14) * 2);
                            if (Math.Sqrt((double)(arg_409_0 * arg_409_0 + num18 * num18)) < (double)(num15 + WorldGen.genRand.Next(2)))
                            {
                                Main.tile[n, num17].Get<TileWallWireStateData>().HasTile = true;
                                Main.tile[n, num17].TileType = (ushort)num16;
                                WorldGen.SquareTileFrame(n, num17, true);
                            }
                        }
                    }
                }
            }
            num = (double)WorldGen.genRand.Next(sizeMin2, sizeMax2);
            num2 = (float)WorldGen.genRand.Next(sizeMin2 / 8, sizeMax2 / 8);
            vector.X = (float)i;
            vector.Y = (float)num5;
            vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            while (vector2.X > -2f && vector2.X < 2f)
            {
                vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            }
            vector2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.02f;
            while (num > 0.0 && num2 > 0f)
            {
                num -= (double)WorldGen.genRand.Next(4);
                num2 -= 1f;
                vector += vector2;
                vector2.X += (float)WorldGen.genRand.Next(-20, 21) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if ((double)vector2.Y > 0.2)
                {
                    vector2.Y = -0.2f;
                }
                if ((double)vector2.Y < -0.2)
                {
                    vector2.Y = -0.2f;
                }
            }
            int num23 = num3;
            num23 += WorldGen.genRand.Next(5);
            while (num23 < num4)
            {
                int num24 = num6;
                while ((!Main.tile[num23, num24].HasTile || Main.tile[num23, num24].TileType != 0) && num23 < num4)
                {
                    num24--;
                    if (num24 < num5)
                    {
                        num24 = num6;
                        num23 += WorldGen.genRand.Next(1, 4);
                    }
                }
                if (num23 < num4)
                {
                    num24 += WorldGen.genRand.Next(0, 4);
                    int num25 = WorldGen.genRand.Next(2, 5);
                    int num26 = ModContent.TileType<BrimstoneSlag>();
                    for (int num27 = num23 - num25; num27 <= num23 + num25; num27++)
                    {
                        for (int num28 = num24 - num25; num28 <= num24 + num25; num28++)
                        {
                            if (num28 > num5)
                            {
                                float arg_890_0 = (float)Math.Abs(num27 - num23);
                                float num29 = (float)(Math.Abs(num28 - num24) * 2);
                                if (Math.Sqrt((double)(arg_890_0 * arg_890_0 + num29 * num29)) < (double)num25)
                                {
                                    Main.tile[num27, num28].TileType = (ushort)num26;
                                    WorldGen.SquareTileFrame(num27, num28, true);
                                }
                            }
                        }
                    }
                    num23 += WorldGen.genRand.Next(num25, (int)((double)num25 * 1.5));
                }
            }
            for (int num34 = num3; num34 <= num4; num34++)
            {
                int num35 = num5 - 10;
                while (!Main.tile[num34, num35 + 1].HasTile)
                {
                    num35++;
                }
                if (num35 < num6 && Main.tile[num34, num35 + 1].TileType == (ushort)ModContent.TileType<BrimstoneSlag>())
                {
                    if (WorldGen.genRand.Next(10) == 0)
                    {
                        int num36 = WorldGen.genRand.Next(1, 3);
                        for (int num37 = num34 - num36; num37 <= num34 + num36; num37++)
                        {
                            if (Main.tile[num37, num35].TileType == (ushort)ModContent.TileType<BrimstoneSlag>())
                            {
                                Main.tile[num37, num35].Get<TileWallWireStateData>().HasTile = false;
                                Main.tile[num37, num35].LiquidAmount = 255;
                                Main.tile[num37, num35].Get<LiquidData>().LiquidType = LiquidID.Water;
                                WorldGen.SquareTileFrame(num34, num35, true);
                            }
                            if (Main.tile[num37, num35 + 1].TileType == (ushort)ModContent.TileType<BrimstoneSlag>())
                            {
                                Main.tile[num37, num35 + 1].Get<TileWallWireStateData>().HasTile = false;
                                Main.tile[num37, num35 + 1].LiquidAmount = 255;
                                Main.tile[num37, num35 + 1].Get<LiquidData>().LiquidType = LiquidID.Water;
                                WorldGen.SquareTileFrame(num34, num35 + 1, true);
                            }
                            if (num37 > num34 - num36 && num37 < num34 + 2 && Main.tile[num37, num35 + 2].TileType == (ushort)ModContent.TileType<BrimstoneSlag>())
                            {
                                Main.tile[num37, num35 + 2].Get<TileWallWireStateData>().HasTile = false;
                                Main.tile[num37, num35 + 2].LiquidAmount = 255;
                                Main.tile[num37, num35 + 2].Get<LiquidData>().LiquidType = LiquidID.Water;
                                WorldGen.SquareTileFrame(num34, num35 + 2, true);
                            }
                        }
                    }
                    if (WorldGen.genRand.Next(5) == 0)
                    {
                        Main.tile[num34, num35].LiquidAmount = 255;
                    }
                    Main.tile[num34, num35].Get<LiquidData>().LiquidType = LiquidID.Water;
                    WorldGen.SquareTileFrame(num34, num35, true);
                }
            }
        }
        #endregion

        #region UnderworldIslandHouse
        public static void UnderworldIslandHouse(int i, int j, int item)
        {
            ushort type = (ushort)ModContent.TileType<BrimstoneSlag>(); //tile
            int wall = ModContent.WallType<BrimstoneSlagWallUnsafe>(); //wall
            Vector2 vector = new Vector2((float)i, (float)j);
            int num = 1;
            if (WorldGen.genRand.Next(2) == 0)
            {
                num = -1;
            }
            int num2 = WorldGen.genRand.Next(7, 12);
            int num3 = WorldGen.genRand.Next(5, 7);
            vector.X = (float)(i + (num2 + 2) * num);
            for (int k = j - 15; k < j + 30; k++)
            {
                if (Main.tile[(int)vector.X, k].HasTile)
                {
                    vector.Y = (float)(k - 1);
                    break;
                }
            }
            vector.X = (float)i;
            int num4 = (int)(vector.X - (float)num2 - 1f);
            int num5 = (int)(vector.X + (float)num2 + 1f);
            int num6 = (int)(vector.Y - (float)num3 - 1f);
            int num7 = (int)(vector.Y + 2f);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int l = num4; l <= num5; l++)
            {
                for (int m = num6 - 1; m < num7 + 1; m++)
                {
                    if (m != num6 - 1 || (l != num4 && l != num5))
                    {
                        Main.tile[l, m].Get<TileWallWireStateData>().HasTile = true;
                        Main.tile[l, m].LiquidAmount = 0;
                        Main.tile[l, m].TileType = type;
                        Main.tile[l, m].WallType = 0;
                        Main.tile[l, m].Get<TileWallWireStateData>().IsHalfBlock = false;
                        Main.tile[l, m].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                    }
                }
            }
            num4 = (int)(vector.X - (float)num2);
            num5 = (int)(vector.X + (float)num2);
            num6 = (int)(vector.Y - (float)num3);
            num7 = (int)(vector.Y + 1f);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int n = num4; n <= num5; n++)
            {
                for (int num8 = num6; num8 < num7; num8++)
                {
                    if ((num8 != num6 || (n != num4 && n != num5)) && Main.tile[n, num8].WallType == 0)
                    {
                        Main.tile[n, num8].Get<TileWallWireStateData>().HasTile = false;
                        Main.tile[n, num8].WallType = (ushort)wall;
                    }
                }
            }
            int num9 = i + (num2 + 1) * num;
            int num10 = (int)vector.Y;
            for (int num11 = num9 - 2; num11 <= num9 + 2; num11++)
            {
                Main.tile[num11, num10].Get<TileWallWireStateData>().HasTile = false;
                Main.tile[num11, num10 - 1].Get<TileWallWireStateData>().HasTile = false;
                Main.tile[num11, num10 - 2].Get<TileWallWireStateData>().HasTile = false;
            }
            WorldGen.PlaceTile(num9, num10, ModContent.TileType<AncientDoorClosed>(), true, false, -1); //door
            num9 = i + (num2 + 1) * -num - num;
            for (int num12 = num6; num12 <= num7 + 1; num12++)
            {
                Main.tile[num9, num12].Get<TileWallWireStateData>().HasTile = true;
                Main.tile[num9, num12].LiquidAmount = 0;
                Main.tile[num9, num12].TileType = type;
                Main.tile[num9, num12].WallType = 0;
                Main.tile[num9, num12].Get<TileWallWireStateData>().IsHalfBlock = false;
                Main.tile[num9, num12].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
            }
            WorldGen.AddBuriedChest(i, num10 - 3, item, false, 4); //chest
            int num22 = i + (num2 / 2 + 1) * -num;
            WorldGen.PlaceTile(num22, num7 - 1, ModContent.TileType<AncientTable>(), true, false, -1); //table
            WorldGen.PlaceTile(num22 - 2, num7 - 1, ModContent.TileType<AncientChair>(), true, false, 0); //chair
            Tile tile = Main.tile[num22 - 2, num7 - 1];
            tile.TileFrameX += 18;
            Tile tile2 = Main.tile[num22 - 2, num7 - 2];
            tile2.TileFrameX += 18;
            WorldGen.PlaceTile(num22 + 2, num7 - 1, ModContent.TileType<AncientChair>(), true, false, 0); //chair
        }
        #endregion

        #region ChasmGenerator
        public static void ChasmGenerator(int i, int j, int steps, bool ocean = false)
        {
            float num = steps; //850 small 1450 medium 2050 large
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
            Vector2 vector;
            vector.X = i;
            vector.Y = j;
            Vector2 vector2;
            vector2.X = WorldGen.genRand.Next(-1, 2) * 0.1f;
            vector2.Y = WorldGen.genRand.Next(3, 8) * 0.2f + 0.5f;
            int num2 = 5;
            double num3 = WorldGen.genRand.Next(5, 7) + 20; //start width
            while (num3 > 0.0)
            {
                if (num > 0f)
                {
                    num3 += WorldGen.genRand.Next(2);
                    num3 -= WorldGen.genRand.Next(2);
                    float smallHoleLimit = 790f; //small
                    if (Main.maxTilesY > 1500)
                    { smallHoleLimit = 1360f; if (Main.maxTilesY > 2100) { smallHoleLimit = 1950f; } }
                    if (ocean && num > smallHoleLimit)
                    {
                        if (num3 < 7.0) //min width
                        {
                            num3 = 7.0; //min width
                        }
                        if (num3 > 11.0) //max width
                        {
                            num3 = 11.0; //max width
                        }
                    }
                    else //dig large hole
                    {
                        if (num3 < (ocean ? 45.0 : 8.0)) //min width
                        {
                            num3 = ocean ? 45.0 : 8.0; //min width
                        }
                        if (num3 > (ocean ? 50.0 : 20.0)) //max width
                        {
                            num3 = ocean ? 50.0 : 20.0; //max width
                        }
                        if (num == 1f && num3 < (ocean ? 50.0 : 15.0))
                        {
                            num3 = ocean ? 50.0 : 15.0;
                        }
                    }
                }
                else
                {
                    if ((double)vector.Y > Abyss.AbyssChasmBottom)
                    {
                        num3 -= WorldGen.genRand.Next(5) + 8;
                    }
                }
                if (Main.maxTilesY > 2100)
                {
                    if (((double)vector.Y > Abyss.AbyssChasmBottom && num > 0f && ocean) ||
                        (vector.Y >= Main.maxTilesY && num > 0f && !ocean))
                    {
                        num = 0f;
                    }
                }
                else if (Main.maxTilesY > 1500)
                {
                    if (((double)vector.Y > Abyss.AbyssChasmBottom && num > 0f && ocean) ||
                        (vector.Y > Main.maxTilesY && num > 0f && !ocean))
                    {
                        num = 0f;
                    }
                }
                else
                {
                    if (((double)vector.Y > Abyss.AbyssChasmBottom && num > 0f && ocean) ||
                        (vector.Y > Main.maxTilesY && num > 0f && !ocean))
                    {
                        num = 0f;
                    }
                }
                num -= 1f;
                int num4;
                int num5;
                int num6;
                int num7;
                if (num > num2)
                {
                    num4 = (int)(vector.X - num3 * 0.5);
                    num5 = (int)(vector.X + num3 * 0.5);
                    num6 = (int)(vector.Y - num3 * 0.5);
                    num7 = (int)(vector.Y + num3 * 0.5);
                    if (num4 < 0)
                    {
                        num4 = 0;
                    }
                    if (num5 > Main.maxTilesX - 1)
                    {
                        num5 = Main.maxTilesX - 1;
                    }
                    if (num6 < 0)
                    {
                        num6 = 0;
                    }
                    if (num7 > Main.maxTilesY)
                    {
                        num7 = Main.maxTilesY;
                    }
                    for (int k = num4; k < num5; k++)
                    {
                        for (int l = num6; l < num7; l++)
                        {
                            if ((Math.Abs(k - vector.X) + Math.Abs(l - vector.Y)) < num3 * 0.5 * (1.0 + WorldGen.genRand.Next(-5, 6) * 0.015))
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
                /*if (num <= 2f && vector.Y < (Main.rockLayer + Main.maxTilesY * 0.3))
                {
                    num = 2f;
                }*/
                vector += vector2;
                vector2.X += WorldGen.genRand.Next(-1, 2) * 0.01f;
                if (vector2.X > 0.02)
                {
                    vector2.X = 0.02f;
                }
                if (vector2.X < -0.02)
                {
                    vector2.X = -0.02f;
                }
                num4 = (int)(vector.X - num3 * 1.1);
                num5 = (int)(vector.X + num3 * 1.1);
                num6 = (int)(vector.Y - num3 * 1.1);
                num7 = (int)(vector.Y + num3 * 1.1);
                if (num4 < 1)
                {
                    num4 = 1;
                }
                if (num5 > Main.maxTilesX - 1)
                {
                    num5 = Main.maxTilesX - 1;
                }
                if (num6 < 0)
                {
                    num6 = 0;
                }
                if (num7 > Main.maxTilesY)
                {
                    num7 = Main.maxTilesY;
                }
                for (int m = num4; m < num5; m++)
                {
                    for (int n = num6; n < num7; n++)
                    {
                        if ((Math.Abs(m - vector.X) + Math.Abs(n - vector.Y)) < num3 * 1.1 * (1.0 + WorldGen.genRand.Next(-5, 6) * 0.015))
                        {
                            if (n > j + WorldGen.genRand.Next(7, 16))
                            {
                                Main.tile[m, n].Get<TileWallWireStateData>().HasTile = false;
                            }
                            if (steps <= num2)
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
                for (int num11 = num4; num11 < num5; num11++)
                {
                    for (int num12 = num6; num12 < num7; num12++)
                    {
                        if ((Math.Abs(num11 - vector.X) + Math.Abs(num12 - vector.Y)) < num3 * 1.1 * (1.0 + WorldGen.genRand.Next(-5, 6) * 0.015))
                        {
                            if (ocean)
                            {
                                Main.tile[num11, num12].LiquidAmount = 255;
                                Main.tile[num11, num12].Get<LiquidData>().LiquidType = LiquidID.Water;
                            }
                            else
                            {
                                Main.tile[num11, num12].LiquidAmount = 255;
                                Main.tile[num11, num12].Get<LiquidData>().LiquidType = LiquidID.Lava;
                            }
                            if (steps <= num2)
                            {
                                Main.tile[num11, num12].Get<TileWallWireStateData>().HasTile = false;
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
            double oneThirdOfUnderground = (Main.maxTilesY - 200 - Main.worldSurface) / 3D;
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
                            if (Main.tile[x, y].TileType == TileID.Emerald || Main.tile[x, y].TileType == TileID.Sapphire)
                                Main.tile[x, y].TileType = TileID.Diamond;
                            else if (Main.tile[x, y].TileType == TileID.Topaz || Main.tile[x, y].TileType == TileID.Amethyst)
                                Main.tile[x, y].TileType = TileID.Ruby;
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
                            if (Main.tile[x, y].TileType == TileID.Diamond || Main.tile[x, y].TileType == TileID.Ruby)
                                Main.tile[x, y].TileType = TileID.Topaz;
                            else if (Main.tile[x, y].TileType == TileID.Emerald || Main.tile[x, y].TileType == TileID.Sapphire)
                                Main.tile[x, y].TileType = TileID.Amethyst;
                        }
                    }
                }
            }
        }
        #endregion
    }
}
