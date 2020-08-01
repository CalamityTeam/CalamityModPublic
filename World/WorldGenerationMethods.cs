using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Potions;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.Tools.ClimateChange;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Schematics;
using CalamityMod.Tiles;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.AstralDesert;
using CalamityMod.Tiles.AstralSnow;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.FurnitureAncient;
using CalamityMod.Tiles.Ores;
using CalamityMod.Tiles.SunkenSea;
using CalamityMod.Walls;
using CalamityMod.World.Planets;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.World.Generation;

namespace CalamityMod.World
{
    public class WorldGenerationMethods : ModWorld
    {
        #region Dungeon Biome Chests
        public static void GenerateBiomeChests(GenerationProgress progress)
        {
            progress.Message = "Calamity Mod: Biome Chests";

            // Get dungeon size field infos. These fields are private for some reason
            int MinX = (int)typeof(WorldGen).GetField("dMinX", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) + 25;
            int MaxX = (int)typeof(WorldGen).GetField("dMaxX", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) - 25;
            int MaxY = (int)typeof(WorldGen).GetField("dMaxY", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) - 25;

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
                    if (Main.wallDungeon[Main.tile[x, y].wall] && !Main.tile[x, y].active())
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
                PutItemInChest(ref chest, ModContent.ItemType<AstralJelly>(), 10, 14);
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
                    ModContent.ItemType<SunkenStew>(), ItemID.WaterWalkingPotion, ItemID.ShinePotion, ItemID.GillsPotion, ItemID.FlipperPotion
                };
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

		#region New Temple
		public static void NewJungleTemple()
		{
			bool flag2 = true;
			while (flag2)
			{
				int num = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 500);
				int num2;
				if (WorldGen.dungeonX < Main.maxTilesX / 2)
				{
					num2 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.6), (int)((double)Main.maxTilesX * 0.85));
				}
				else
				{
					num2 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.15), (int)((double)Main.maxTilesX * 0.4));
				}
				if (Main.tile[num2, num].active() && Main.tile[num2, num].type == 60)
				{
					flag2 = false;
					GenNewTemple(num2, num);
				}
			}
		}

		public static void GenNewTemple(int x, int y)
		{
			Rectangle[] array = new Rectangle[200];
			float num = (float)(Main.maxTilesX / 4200);
			int num2 = WorldGen.genRand.Next((int)(num * 12f), (int)(num * 16f));
			int num3 = 1;
			if (WorldGen.genRand.Next(2) == 0)
			{
				num3 = -1;
			}
			int num4 = num3;
			int num5 = x;
			int num6 = y;
			int num7 = x;
			int num8 = y;
			int num9 = WorldGen.genRand.Next(1, 3);
			int num10 = 0;

			// Temple room sizes
			for (int i = 0; i < num2; i++)
			{
				num10++;
				int num11 = num3;
				int num12 = num7;
				int num13 = num8;
				bool flag = true;
				int num14 = 0;
				int num15 = 0;
				int num16 = -10;
				Rectangle rectangle = new Rectangle(num12 - num14 / 2, num13 - num15 / 2, num14, num15);
				while (flag)
				{
					num12 = num7;
					num13 = num8;
					num14 = WorldGen.genRand.Next(30, 46);
					num15 = WorldGen.genRand.Next(25, 31);

					// Final temple room size
					if (i == num2 - 1)
					{
						num14 = WorldGen.genRand.Next(105, 116);
						num15 = WorldGen.genRand.Next(90, 106);
						num13 += WorldGen.genRand.Next(6, 9);
					}

					if (num10 > num9)
					{
						num13 += WorldGen.genRand.Next(num15 + 1, num15 + 3) + num16;
						num12 += WorldGen.genRand.Next(-2, 3);
						num11 = num3 * -1;
					}
					else
					{
						num12 += (WorldGen.genRand.Next(num14 + 1, num14 + 3) + num16) * num11;
						num13 += WorldGen.genRand.Next(-2, 3);
					}

					flag = false;
					rectangle = new Rectangle(num12 - num14 / 2, num13 - num15 / 2, num14, num15);
					for (int j = 0; j < i; j++)
					{
						if (rectangle.Intersects(array[j]))
						{
							flag = true;
						}
						if (WorldGen.genRand.Next(100) == 0)
						{
							num16++;
						}
					}
				}
				if (num10 > num9)
				{
					num9++;
					num10 = 1;
				}
				array[i] = rectangle;
				num3 = num11;
				num7 = num12;
				num8 = num13;
			}

			for (int k = 0; k < num2; k++)
			{
				for (int l = 0; l < 2; l++)
				{
					for (int m = 0; m < num2; m++)
					{
						for (int n = 0; n < 2; n++)
						{
							int num17 = array[k].X;
							if (l == 1)
							{
								num17 += array[k].Width - 1;
							}
							int num18 = array[k].Y;
							int num19 = num18 + array[k].Height;
							int num20 = array[m].X;
							if (n == 1)
							{
								num20 += array[m].Width - 1;
							}
							int y2 = array[m].Y;
							int num21 = y2 + array[m].Height;
							while (num17 != num20 || num18 != y2 || num19 != num21)
							{
								if (num17 < num20)
								{
									num17++;
								}
								if (num17 > num20)
								{
									num17--;
								}
								if (num18 < y2)
								{
									num18++;
								}
								if (num18 > y2)
								{
									num18--;
								}
								if (num19 < num21)
								{
									num19++;
								}
								if (num19 > num21)
								{
									num19--;
								}
								int num22 = num17;
								for (int num23 = num18; num23 < num19; num23++)
								{
									Main.tile[num22, num23].active(true);
									Main.tile[num22, num23].type = 226;
									Main.tile[num22, num23].liquid = 0;
									Main.tile[num22, num23].slope(0);
									Main.tile[num22, num23].halfBrick(false);
								}
							}
						}
					}
				}
			}

			for (int num24 = 0; num24 < num2; num24++)
			{
				for (int num25 = array[num24].X; num25 < array[num24].X + array[num24].Width; num25++)
				{
					for (int num26 = array[num24].Y; num26 < array[num24].Y + array[num24].Height; num26++)
					{
						Main.tile[num25, num26].active(true);
						Main.tile[num25, num26].type = 226;
						Main.tile[num25, num26].liquid = 0;
						Main.tile[num25, num26].slope(0);
						Main.tile[num25, num26].halfBrick(false);
					}
				}
				int num27 = array[num24].X;
				int num28 = num27 + array[num24].Width;
				int num29 = array[num24].Y;
				int num30 = num29 + array[num24].Height;
				num27 += WorldGen.genRand.Next(4, 7);
				num28 -= WorldGen.genRand.Next(4, 7);
				num29 += WorldGen.genRand.Next(4, 7);
				num30 -= WorldGen.genRand.Next(4, 7);
				int num31 = num27;
				int num32 = num28;
				int num33 = num29;
				int num34 = num30;
				int num35 = (num27 + num28) / 2;
				int num36 = (num29 + num30) / 2;
				for (int num37 = num27; num37 < num28; num37++)
				{
					for (int num38 = num29; num38 < num30; num38++)
					{
						if (WorldGen.genRand.Next(20) == 0)
						{
							num33 += WorldGen.genRand.Next(-1, 2);
						}
						if (WorldGen.genRand.Next(20) == 0)
						{
							num34 += WorldGen.genRand.Next(-1, 2);
						}
						if (WorldGen.genRand.Next(20) == 0)
						{
							num31 += WorldGen.genRand.Next(-1, 2);
						}
						if (WorldGen.genRand.Next(20) == 0)
						{
							num32 += WorldGen.genRand.Next(-1, 2);
						}
						if (num31 < num27)
						{
							num31 = num27;
						}
						if (num32 > num28)
						{
							num32 = num28;
						}
						if (num33 < num29)
						{
							num33 = num29;
						}
						if (num34 > num30)
						{
							num34 = num30;
						}
						if (num31 > num35)
						{
							num31 = num35;
						}
						if (num32 < num35)
						{
							num32 = num35;
						}
						if (num33 > num36)
						{
							num33 = num36;
						}
						if (num34 < num36)
						{
							num34 = num36;
						}
						if (num37 >= num31 && (num37 < num32 & num38 >= num33) && num38 <= num34)
						{
							Main.tile[num37, num38].active(false);
							Main.tile[num37, num38].wall = 87;
						}
					}
				}
				for (int num39 = num30; num39 > num29; num39--)
				{
					for (int num40 = num28; num40 > num27; num40--)
					{
						if (WorldGen.genRand.Next(20) == 0)
						{
							num33 += WorldGen.genRand.Next(-1, 2);
						}
						if (WorldGen.genRand.Next(20) == 0)
						{
							num34 += WorldGen.genRand.Next(-1, 2);
						}
						if (WorldGen.genRand.Next(20) == 0)
						{
							num31 += WorldGen.genRand.Next(-1, 2);
						}
						if (WorldGen.genRand.Next(20) == 0)
						{
							num32 += WorldGen.genRand.Next(-1, 2);
						}
						if (num31 < num27)
						{
							num31 = num27;
						}
						if (num32 > num28)
						{
							num32 = num28;
						}
						if (num33 < num29)
						{
							num33 = num29;
						}
						if (num34 > num30)
						{
							num34 = num30;
						}
						if (num31 > num35)
						{
							num31 = num35;
						}
						if (num32 < num35)
						{
							num32 = num35;
						}
						if (num33 > num36)
						{
							num33 = num36;
						}
						if (num34 < num36)
						{
							num34 = num36;
						}
						if (num40 >= num31 && (num40 < num32 & num39 >= num33) && num39 <= num34)
						{
							Main.tile[num40, num39].active(false);
							Main.tile[num40, num39].wall = 87;
						}
					}
				}
			}

			Vector2 vector = new Vector2((float)num5, (float)num6);
			for (int num41 = 0; num41 < num2; num41++)
			{
				Rectangle rectangle2 = array[num41];
				rectangle2.X += 8;
				rectangle2.Y += 8;
				rectangle2.Width -= 16;
				rectangle2.Height -= 16;
				bool flag2 = true;
				while (flag2)
				{
					int num42 = WorldGen.genRand.Next(rectangle2.X, rectangle2.X + rectangle2.Width);
					int num43 = WorldGen.genRand.Next(rectangle2.Y, rectangle2.Y + rectangle2.Height);
					vector = WorldGen.templePather(vector, num42, num43);
					if (vector.X == (float)num42 && vector.Y == (float)num43)
					{
						flag2 = false;
					}
				}
				if (num41 < num2 - 1)
				{
					if (WorldGen.genRand.Next(3) != 0)
					{
						int num44 = num41 + 1;
						if (array[num44].Y >= array[num41].Y + array[num41].Height)
						{
							rectangle2.X = array[num44].X;
							if (array[num44].X < array[num41].X)
							{
								rectangle2.X += (int)((double)((float)array[num44].Width) * 0.2);
							}
							else
							{
								rectangle2.X += (int)((double)((float)array[num44].Width) * 0.8);
							}
							rectangle2.Y = array[num44].Y;
						}
						else
						{
							rectangle2.X = (array[num41].X + array[num41].Width / 2 + (array[num44].X + array[num44].Width / 2)) / 2;
							rectangle2.Y = (int)((double)array[num44].Y + (double)array[num44].Height * 0.8);
						}
						int x2 = rectangle2.X;
						int y3 = rectangle2.Y;
						flag2 = true;
						while (flag2)
						{
							int num45 = WorldGen.genRand.Next(x2 - 4, x2 + 5);
							int num46 = WorldGen.genRand.Next(y3 - 4, y3 + 5);
							vector = WorldGen.templePather(vector, num45, num46);
							if (vector.X == (float)num45 && vector.Y == (float)num46)
							{
								flag2 = false;
							}
						}
					}
					else
					{
						int num47 = num41 + 1;
						int num48 = (array[num41].X + array[num41].Width / 2 + (array[num47].X + array[num47].Width / 2)) / 2;
						int num49 = (array[num41].Y + array[num41].Height / 2 + (array[num47].Y + array[num47].Height / 2)) / 2;
						flag2 = true;
						while (flag2)
						{
							int num50 = WorldGen.genRand.Next(num48 - 4, num48 + 5);
							int num51 = WorldGen.genRand.Next(num49 - 4, num49 + 5);
							vector = WorldGen.templePather(vector, num50, num51);
							if (vector.X == (float)num50 && vector.Y == (float)num51)
							{
								flag2 = false;
							}
						}
					}
				}
			}
			int num52 = Main.maxTilesX - 20;
			int num53 = 20;
			int num54 = Main.maxTilesY - 20;
			int num55 = 20;
			for (int num56 = 0; num56 < num2; num56++)
			{
				if (array[num56].X < num52)
				{
					num52 = array[num56].X;
				}
				if (array[num56].X + array[num56].Width > num53)
				{
					num53 = array[num56].X + array[num56].Width;
				}
				if (array[num56].Y < num54)
				{
					num54 = array[num56].Y;
				}
				if (array[num56].Y + array[num56].Height > num55)
				{
					num55 = array[num56].Y + array[num56].Height;
				}
			}

			// Create outer temple shape
			num52 -= 10;
			num53 += 10;
			num54 -= 10;
			num55 += 10;
			for (int num57 = num52; num57 < num53; num57++)
			{
				for (int num58 = num54; num58 < num55; num58++)
				{
					WorldGen.outerTempled(num57, num58);
				}
			}
			for (int num59 = num53; num59 >= num52; num59--)
			{
				for (int num60 = num54; num60 < num55 / 2; num60++)
				{
					WorldGen.outerTempled(num59, num60);
				}
			}
			for (int num61 = num54; num61 < num55; num61++)
			{
				for (int num62 = num52; num62 < num53; num62++)
				{
					WorldGen.outerTempled(num62, num61);
				}
			}
			for (int num63 = num55; num63 >= num54; num63--)
			{
				for (int num64 = num52; num64 < num53; num64++)
				{
					WorldGen.outerTempled(num64, num63);
				}
			}

			// Definitely does something
			num3 = -num4;
			Vector2 vector2 = new Vector2((float)num5, (float)num6);
			int num65 = 4;
			bool flag3 = true;
			int num66 = 0;
			int num67 = WorldGen.genRand.Next(12, 14);
			while (flag3)
			{
				num66++;
				if (num66 >= num67)
				{
					num66 = 0;
					vector2.Y -= 1f;
				}
				vector2.X += (float)num3;
				int num68 = (int)vector2.X;
				flag3 = false;
				int num69 = (int)vector2.Y - num65;
				while ((float)num69 < vector2.Y + (float)num65)
				{
					if (Main.tile[num68, num69].wall == 87 || (Main.tile[num68, num69].active() && Main.tile[num68, num69].type == 226))
					{
						flag3 = true;
					}
					if (Main.tile[num68, num69].active() && Main.tile[num68, num69].type == 226)
					{
						Main.tile[num68, num69].active(false);
						Main.tile[num68, num69].wall = 87;
					}
					num69++;
				}
			}

			// Place bricks and walls
			int num70 = num5;
			int num71 = num6;
			while (!Main.tile[num70, num71].active())
			{
				num71++;
			}
			num71 -= 4;
			int num72 = num71;
			while ((Main.tile[num70, num72].active() && Main.tile[num70, num72].type == 226) || Main.tile[num70, num72].wall == 87)
			{
				num72--;
			}
			num72 += 2;
			for (int num73 = num70 - 1; num73 <= num70 + 1; num73++)
			{
				for (int num74 = num72; num74 <= num71; num74++)
				{
					Main.tile[num73, num74].active(true);
					Main.tile[num73, num74].type = 226;
					Main.tile[num73, num74].liquid = 0;
					Main.tile[num73, num74].slope(0);
					Main.tile[num73, num74].halfBrick(false);
				}
			}
			for (int num75 = num70 - 4; num75 <= num70 + 4; num75++)
			{
				for (int num76 = num71 - 1; num76 < num71 + 3; num76++)
				{
					Main.tile[num75, num76].active(false);
					Main.tile[num75, num76].wall = 87;
				}
			}
			for (int num77 = num70 - 1; num77 <= num70 + 1; num77++)
			{
				for (int num78 = num71 - 5; num78 <= num71 + 8; num78++)
				{
					Main.tile[num77, num78].active(true);
					Main.tile[num77, num78].type = 226;
					Main.tile[num77, num78].liquid = 0;
					Main.tile[num77, num78].slope(0);
					Main.tile[num77, num78].halfBrick(false);
				}
			}
			for (int num79 = num70 - 1; num79 <= num70 + 1; num79++)
			{
				for (int num80 = num71; num80 < num71 + 3; num80++)
				{
					Main.tile[num79, num80].active(false);
					Main.tile[num79, num80].wall = 87;
				}
			}

			// Temple Door
			WorldGen.PlaceTile(num70, num71, 10, true, false, -1, 11);

			// Clear space in temple
			for (int num81 = num52; num81 < num53; num81++)
			{
				for (int num82 = num54; num82 < num55; num82++)
				{
					WorldGen.templeCleaner(num81, num82);
				}
			}
			for (int num83 = num55; num83 >= num54; num83--)
			{
				for (int num84 = num53; num84 >= num52; num84--)
				{
					WorldGen.templeCleaner(num84, num83);
				}
			}

			// Place walls
			for (int num85 = num52; num85 < num53; num85++)
			{
				for (int num86 = num54; num86 < num55; num86++)
				{
					bool flag4 = true;
					for (int num87 = num85 - 1; num87 <= num85 + 1; num87++)
					{
						for (int num88 = num86 - 1; num88 <= num86 + 1; num88++)
						{
							if ((!Main.tile[num87, num88].active() || Main.tile[num87, num88].type != 226) && Main.tile[num87, num88].wall != 87)
							{
								flag4 = false;
								break;
							}
						}
					}
					if (flag4)
					{
						Main.tile[num85, num86].wall = 87;
					}
				}
			}

			// Lihzahrd Altar
			int num89 = 0;
			Rectangle rectangle3;
			int num90;
			int num91;
			while (true)
			{
				num89++;
				rectangle3 = array[num2 - 1];
				num90 = rectangle3.X + WorldGen.genRand.Next(rectangle3.Width);
				num91 = rectangle3.Y + WorldGen.genRand.Next(rectangle3.Height);
				WorldGen.PlaceTile(num90, num91, 237, false, false, -1, 0);
				if (Main.tile[num90, num91].type == 237)
				{
					break;
				}
				if (num89 >= 1000)
				{
					goto Block_117;
				}
			}

			// Set Altar location
			CalamityWorld.newAltarX = num90 - (int)(Main.tile[num90, num91].frameX / 18);
			CalamityWorld.newAltarY = num91 - (int)(Main.tile[num90, num91].frameY / 18);

			// If Altar is spawned, continue to temple spike spawn
			goto IL_1548;

			// Force spawn Altar if previous code fails
			Block_117:
			num90 = rectangle3.X + rectangle3.Width / 2;
			num91 = rectangle3.Y + rectangle3.Height / 2;
			num90 += WorldGen.genRand.Next(-10, 11);
			num91 += WorldGen.genRand.Next(-10, 11);
			while (!Main.tile[num90, num91].active())
			{
				num91++;
			}
			Main.tile[num90 - 1, num91].active(true);
			Main.tile[num90 - 1, num91].slope(0);
			Main.tile[num90 - 1, num91].halfBrick(false);
			Main.tile[num90 - 1, num91].type = 226;
			Main.tile[num90, num91].active(true);
			Main.tile[num90, num91].slope(0);
			Main.tile[num90, num91].halfBrick(false);
			Main.tile[num90, num91].type = 226;
			Main.tile[num90 + 1, num91].active(true);
			Main.tile[num90 + 1, num91].slope(0);
			Main.tile[num90 + 1, num91].halfBrick(false);
			Main.tile[num90 + 1, num91].type = 226;
			num91 -= 2;
			num90--;
			for (int num92 = -1; num92 <= 3; num92++)
			{
				for (int num93 = -1; num93 <= 1; num93++)
				{
					x = num90 + num92;
					y = num91 + num93;
					Main.tile[x, y].active(false);
				}
			}
			for (int num94 = 0; num94 <= 2; num94++)
			{
				for (int num95 = 0; num95 <= 1; num95++)
				{
					x = num90 + num94;
					y = num91 + num95;
					Main.tile[x, y].active(true);
					Main.tile[x, y].type = 237;
					Main.tile[x, y].frameX = (short)(num94 * 18);
					Main.tile[x, y].frameY = (short)(num95 * 18);
				}
			}

			// Set Altar location
			CalamityWorld.newAltarX = num90;
			CalamityWorld.newAltarY = num91;

			// Skip previous code if first Altar spawn works
			IL_1548:

			// Create temple spikes
			float num96 = (float)num2 * 1.3f;
			int num97 = 0;
			while (num96 > 0f)
			{
				num97++;
				int num98 = WorldGen.genRand.Next(num2);
				int num99 = WorldGen.genRand.Next(array[num98].X, array[num98].X + array[num98].Width);
				int num100 = WorldGen.genRand.Next(array[num98].Y, array[num98].Y + array[num98].Height);
				if (Main.tile[num99, num100].wall == 87 && !Main.tile[num99, num100].active())
				{
					bool flag5 = false;
					if (WorldGen.genRand.Next(2) == 0)
					{
						int num101 = 1;
						if (WorldGen.genRand.Next(2) == 0)
						{
							num101 = -1;
						}
						while (!Main.tile[num99, num100].active())
						{
							num100 += num101;
						}
						num100 -= num101;
						int num102 = WorldGen.genRand.Next(2);
						int num103 = WorldGen.genRand.Next(8, 10);
						bool flag6 = true;
						for (int num104 = num99 - num103; num104 < num99 + num103; num104++)
						{
							for (int num105 = num100 - num103; num105 < num100 + num103; num105++)
							{
								if (Main.tile[num104, num105].active() && Main.tile[num104, num105].type == 10)
								{
									flag6 = false;
									break;
								}
							}
						}
						if (flag6)
						{
							for (int num106 = num99 - num103; num106 < num99 + num103; num106++)
							{
								for (int num107 = num100 - num103; num107 < num100 + num103; num107++)
								{
									if (WorldGen.SolidTile(num106, num107) && Main.tile[num106, num107].type != 232 && !WorldGen.SolidTile(num106, num107 - num101))
									{
										Main.tile[num106, num107].type = 232;
										flag5 = true;
										if (num102 == 0)
										{
											Main.tile[num106, num107 - 1].type = 232;
											Main.tile[num106, num107 - 1].active(true);
										}
										else
										{
											Main.tile[num106, num107 + 1].type = 232;
											Main.tile[num106, num107 + 1].active(true);
										}
										num102++;
										if (num102 > 1)
										{
											num102 = 0;
										}
									}
								}
							}
						}
						if (flag5)
						{
							num97 = 0;
							num96 -= 1f;
						}
					}
					else
					{
						int num108 = 1;
						if (WorldGen.genRand.Next(2) == 0)
						{
							num108 = -1;
						}
						while (!Main.tile[num99, num100].active())
						{
							num99 += num108;
						}
						num99 -= num108;
						int num109 = WorldGen.genRand.Next(2);
						int num110 = WorldGen.genRand.Next(8, 10);
						bool flag7 = true;
						for (int num111 = num99 - num110; num111 < num99 + num110; num111++)
						{
							for (int num112 = num100 - num110; num112 < num100 + num110; num112++)
							{
								if (Main.tile[num111, num112].active() && Main.tile[num111, num112].type == 10)
								{
									flag7 = false;
									break;
								}
							}
						}
						if (flag7)
						{
							for (int num113 = num99 - num110; num113 < num99 + num110; num113++)
							{
								for (int num114 = num100 - num110; num114 < num100 + num110; num114++)
								{
									if (WorldGen.SolidTile(num113, num114) && Main.tile[num113, num114].type != 232 && !WorldGen.SolidTile(num113 - num108, num114))
									{
										Main.tile[num113, num114].type = 232;
										flag5 = true;
										if (num109 == 0)
										{
											Main.tile[num113 - 1, num114].type = 232;
											Main.tile[num113 - 1, num114].active(true);
										}
										else
										{
											Main.tile[num113 + 1, num114].type = 232;
											Main.tile[num113 + 1, num114].active(true);
										}
										num109++;
										if (num109 > 1)
										{
											num109 = 0;
										}
									}
								}
							}
						}
						if (flag5)
						{
							num97 = 0;
							num96 -= 1f;
						}
					}
				}
				if (num97 > 1000)
				{
					num97 = 0;
					num96 -= 1f;
				}
			}

			// Set temple variables for temple chest, chair, table, workbench, etc. locations
			WorldGen.tLeft = num52;
			WorldGen.tRight = num53;
			WorldGen.tTop = num54;
			WorldGen.tBottom = num55;
			WorldGen.tRooms = num2;
		}

		public static void NewJungleTemplePart2()
		{
			int minValue = WorldGen.tLeft;
			int maxValue = WorldGen.tRight;
			int minValue2 = WorldGen.tTop;
			int num = WorldGen.tBottom;
			int num2 = WorldGen.tRooms;
			float num3 = (float)num2 * 2.1f;
			int num4 = 0;
			while (num3 > 0f)
			{
				int num5 = WorldGen.genRand.Next(minValue, maxValue);
				int num6 = WorldGen.genRand.Next(minValue2, num);
				if (Main.tile[num5, num6].wall == 87 && !Main.tile[num5, num6].active())
				{
					if (WorldGen.mayanTrap(num5, num6))
					{
						num3 -= 1f;
						num4 = 0;
					}
					else
					{
						num4++;
					}
				}
				else
				{
					num4++;
				}
				if (num4 > 100)
				{
					num4 = 0;
					num3 -= 1f;
				}
			}
			Main.tileSolid[232] = false;
			float num7 = (float)num2 * 0.4f;
			int contain = 1293;
			num4 = 0;
			while (num7 > 0f)
			{
				int num8 = WorldGen.genRand.Next(minValue, maxValue);
				int num9 = WorldGen.genRand.Next(minValue2, num);
				if (Main.tile[num8, num9].wall == 87 && !Main.tile[num8, num9].active() && WorldGen.AddBuriedChest(num8, num9, contain, true, 16))
				{
					num7 -= 1f;
					num4 = 0;
				}
				num4++;
				if (num4 > 10000)
				{
					break;
				}
			}
			float num10 = (float)num2 * 1.6f;
			num4 = 0;
			while (num10 > 0f)
			{
				num4++;
				int num11 = WorldGen.genRand.Next(minValue, maxValue);
				int num12 = WorldGen.genRand.Next(minValue2, num);
				if (Main.tile[num11, num12].wall == 87 && !Main.tile[num11, num12].active())
				{
					int num13 = num11;
					int num14 = num12;
					while (!Main.tile[num13, num14].active())
					{
						num14++;
						if (num14 > num)
						{
							break;
						}
					}
					num14--;
					if (num14 <= num)
					{
						WorldGen.PlaceTile(num13, num14, 105, true, false, -1, WorldGen.genRand.Next(43, 46));
						if (Main.tile[num13, num14].type == 105)
						{
							num10 -= 1f;
						}
					}
				}
			}
			float num15 = (float)num2 * 1.6f;
			num4 = 0;
			while (num15 > 0f)
			{
				num4++;
				int num16 = WorldGen.genRand.Next(minValue, maxValue);
				int num17 = WorldGen.genRand.Next(minValue2, num);
				if (Main.tile[num16, num17].wall == 87 && !Main.tile[num16, num17].active())
				{
					int num18 = num16;
					int num19 = num17;
					while (!Main.tile[num18, num19].active())
					{
						num19++;
						if (num19 > num)
						{
							break;
						}
					}
					num19--;
					if (num19 <= num)
					{
						int num20 = WorldGen.genRand.Next(3);
						if (num20 == 0)
						{
							WorldGen.PlaceTile(num18, num19, 18, true, false, -1, 10);
							if (Main.tile[num18, num19].type == 18)
							{
								num15 -= 1f;
							}
						}
						else if (num20 == 1)
						{
							WorldGen.PlaceTile(num18, num19, 14, true, false, -1, 9);
							if (Main.tile[num18, num19].type == 14)
							{
								num15 -= 1f;
							}
						}
						else if (num20 == 2)
						{
							WorldGen.PlaceTile(num18, num19, 15, true, false, -1, 12);
							if (Main.tile[num18, num19].type == 15)
							{
								num15 -= 1f;
							}
						}
					}
				}
				if (num4 > 10000)
				{
					break;
				}
			}
			Main.tileSolid[232] = true;
		}

		public static void NewJungleTempleLihzahrdAltar()
		{
			int lAltarX = CalamityWorld.newAltarX;
			int lAltarY = CalamityWorld.newAltarY;
			for (int i = 0; i <= 2; i++)
			{
				for (int j = 0; j <= 1; j++)
				{
					int num = lAltarX + i;
					int num2 = lAltarY + j;
					Main.tile[num, num2].active(true);
					Main.tile[num, num2].type = 237;
					Main.tile[num, num2].frameX = (short)(i * 18);
					Main.tile[num, num2].frameY = (short)(j * 18);
				}
				Main.tile[i, lAltarY + 2].active(true);
				Main.tile[i, lAltarY + 2].slope(0);
				Main.tile[i, lAltarY + 2].halfBrick(false);
				Main.tile[i, lAltarY + 2].type = 226;
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
                        if (Main.tile[x, y] != null && Main.tile[x, y].type == TileID.LargePiles)
                        {
                            if ((Main.tile[x, y].frameX == 18 && Main.tile[x, y].frameY == 0) || (Main.tile[x, y].frameX == 45 && Main.tile[x, y].frameY == 0))
                            {
                                if (WorldGen.genRand.Next(3) == 0)
                                {
                                    for (int dx = -1; dx < 2; dx++)
                                    {
                                        for (int dy = -1; dy < 2; dy++)
                                            Main.tile[x + dx, y + dy].active(false);
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

        #region OreSpawn
        public static void SpawnOre(int type, double frequency, float depth, float depthLimit)
        {
            int x = Main.maxTilesX;
            int y = Main.maxTilesY;
            if (type == ModContent.TileType<ExodiumOre>())
            {
                depthLimit = 0.14f;
                if (y > 1500)
                { depthLimit = 0.1f; if (y > 2100) { depthLimit = 0.07f; } }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int k = 0; k < (int)((double)(x * y) * frequency); k++)
                {
                    int tilesX = WorldGen.genRand.Next(0, x);
                    int tilesY = WorldGen.genRand.Next((int)(y * depth), (int)(y * depthLimit));
                    if (type == ModContent.TileType<AuricOre>())
                    {
                        WorldGen.OreRunner(tilesX, tilesY, (double)WorldGen.genRand.Next(12, 18), WorldGen.genRand.Next(12, 18), (ushort)type);
                    }
                    else if (type == ModContent.TileType<UelibloomOre>())
                    { //mud
                        if (Main.tile[tilesX, tilesY].type == 59)
                        {
                            WorldGen.OreRunner(tilesX, tilesY, (double)WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), (ushort)type);
                        }
                    }
                    else if (type == ModContent.TileType<PerennialOre>())
                    { //dirt, stone
                        if (Main.tile[tilesX, tilesY].type == 0 || Main.tile[tilesX, tilesY].type == 1)
                        {
                            WorldGen.OreRunner(tilesX, tilesY, (double)WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), (ushort)type);
                        }
                    }
                    else if (type == ModContent.TileType<CryonicOre>())
                    { //snow, ice, purple ice, pink ice, red ice, astral snow, astral ice
                        if (Main.tile[tilesX, tilesY].type == 147 || Main.tile[tilesX, tilesY].type == 161 || Main.tile[tilesX, tilesY].type == 163 || Main.tile[tilesX, tilesY].type == 164 || Main.tile[tilesX, tilesY].type == 200 || Main.tile[tilesX, tilesY].type == ModContent.TileType<AstralSnow>() || Main.tile[tilesX, tilesY].type == ModContent.TileType<AstralIce>())
                        {
                            WorldGen.OreRunner(tilesX, tilesY, (double)WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), (ushort)type);
                        }
                    }
                    else
                    {
                        WorldGen.OreRunner(tilesX, tilesY, (double)WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), (ushort)type);
                    }
                }
            }
        }
        #endregion

        #region AstralMeteor
        public static bool CanAstralMeteorSpawn()
        {
            int astralOreCount = 0;
            float worldSizeFactor = Main.maxTilesX / 4200f; // Small = 4200, Medium = 6400, Large = 8400
            int astralOreAllowed = (int)(200f * worldSizeFactor); // Small = 201 Medium = 305 Large = 401
            for (int x = 5; x < Main.maxTilesX - 5; x++)
            {
                int y = 5;
                while (y < Main.worldSurface)
                {
                    if (Main.tile[x, y].active() && Main.tile[x, y].type == ModContent.TileType<AstralOre>())
                    {
                        astralOreCount++;
                        if (astralOreCount > astralOreAllowed)
                            return false;
                    }
                    y++;
                }
            }
            return true;
        }

        public static bool CanAstralBiomeSpawn()
        {
            int astralTileCount = 0;
            float worldSizeFactor = Main.maxTilesX / 4200f; // Small = 4200, Medium = 6400, Large = 8400
            int astralTilesAllowed = (int)(400f * worldSizeFactor); // Small = 401 Medium = 605 Large = 801
            for (int x = 5; x < Main.maxTilesX - 5; x++)
            {
                int y = 5;
                while (y < Main.worldSurface)
                {
                    if (Main.tile[x, y].active() &&
                        (Main.tile[x, y].type == ModContent.TileType<AstralSand>() || Main.tile[x, y].type == ModContent.TileType<AstralSandstone>() ||
                        Main.tile[x, y].type == ModContent.TileType<HardenedAstralSand>() || Main.tile[x, y].type == ModContent.TileType<AstralIce>() ||
                        Main.tile[x, y].type == ModContent.TileType<AstralDirt>() || Main.tile[x, y].type == ModContent.TileType<AstralStone>() ||
                        Main.tile[x, y].type == ModContent.TileType<AstralGrass>() || Main.tile[x, y].type == ModContent.TileType<AstralSilt>() ||
						Main.tile[x, y].type == ModContent.TileType<AstralFossil>() || Main.tile[x, y].type == ModContent.TileType<AstralSnow>() ||
						Main.tile[x, y].type == ModContent.TileType<AstralClay>() || Main.tile[x, y].type == ModContent.TileType<AstralStone>()))
                    {
                        astralTileCount++;
                        if (astralTileCount > astralTilesAllowed)
                            return false;
                    }
                    y++;
                }
            }
            return true;
        }

        public static void AstralMeteorThreadWrapper(object context)
        {
            PlaceAstralMeteor();
        }

        public static void PlaceAstralMeteor()
        {
            Mod ancientsAwakened = ModLoader.GetMod("AAMod");

            // This flag is also used to determine whether players are nearby.
            bool meteorDropped = true;

            // Clients in multiplayer don't drop meteors.
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            for (int i = 0; i < 255; i++)
            {
                if (Main.player[i].active)
                {
                    meteorDropped = false;
                    break;
                }
            }

            // Check whether there is already too much ore.
            if (!CanAstralMeteorSpawn())
                return;

            UnifiedRandom rand = WorldGen.genRand;
            float solidTileRequirement = 600f;
            bool localAbyssSide = WorldGen.dungeonX < Main.maxTilesX / 2;
            while (!meteorDropped)
            {
                float worldEdgeMargin = (float)Main.maxTilesX * 0.08f;
                int xLimit = Main.maxTilesX / 2;
                int x = CalamityWorld.abyssSide ? rand.Next(400, xLimit) : rand.Next(xLimit, Main.maxTilesX - 400);
                while ((float)x > (float)Main.spawnTileX - worldEdgeMargin && (float)x < (float)Main.spawnTileX + worldEdgeMargin)
                {
                    x = CalamityWorld.abyssSide ? rand.Next(400, xLimit) : rand.Next(xLimit, Main.maxTilesX - 400);
                }
                //world surface = 920 large 740 medium 560 small
                int y = (int)(Main.worldSurface * 0.5); //Large = 522, Medium = 444, Small = 336
                while (y < Main.maxTilesY)
                {
                    if (Main.tile[x, y].active() && Main.tileSolid[(int)Main.tile[x, y].type])
                    {
                        int suitableTiles = 0;
                        int checkRadius = 15;
                        for (int l = x - checkRadius; l < x + checkRadius; l++)
                        {
                            for (int m = y - checkRadius; m < y + checkRadius; m++)
                            {
                                if (WorldGen.SolidTile(l, m))
                                {
                                    suitableTiles++;

                                    // Avoid floating islands: Clouds and Sunplate both harshly punish attempted meteor spawns
                                    if (Main.tile[l, m].type == TileID.Cloud || Main.tile[l, m].type == TileID.Sunplate)
                                    {
                                        suitableTiles -= 100;
                                    }
                                    // Avoid Sulphurous Sea beach: Cannot be converted by astral
                                    else if (Main.tile[l, m].type == ModContent.TileType<SulphurousSand>() || Main.tile[l, m].type == ModContent.TileType<SulphurousSandstone>())
                                    {
                                        suitableTiles -= 100;
                                    }

                                    // Prevent the Astral biome from overriding or interfering with an AA biome
                                    else if (ancientsAwakened != null)
                                    {
                                        if (Main.tile[l, m].type == ancientsAwakened.TileType("InfernoGrass") || Main.tile[l, m].type == ancientsAwakened.TileType("Torchstone") ||
                                            Main.tile[l, m].type == ancientsAwakened.TileType("Torchsand") || Main.tile[l, m].type == ancientsAwakened.TileType("Torchsandstone") ||
                                            Main.tile[l, m].type == ancientsAwakened.TileType("Torchsandhardened") || Main.tile[l, m].type == ancientsAwakened.TileType("Torchice") ||
                                            Main.tile[l, m].type == ancientsAwakened.TileType("Depthstone") || Main.tile[l, m].type == ancientsAwakened.TileType("Depthsand") ||
                                            Main.tile[l, m].type == ancientsAwakened.TileType("Depthsandstone") || Main.tile[l, m].type == ancientsAwakened.TileType("Depthsandhardened") ||
                                            Main.tile[l, m].type == ancientsAwakened.TileType("Depthice"))
                                        {
                                            suitableTiles -= 100;
                                        }
                                    }
                                }

                                // Liquid aversion makes meteors less likely to fall in lakes
                                else if (Main.tile[l, m].liquid > 0)
                                {
                                    suitableTiles--;
                                }
                            }
                        }

                        if ((float)suitableTiles < solidTileRequirement)
                        {
                            solidTileRequirement -= 0.5f;
                            break;
                        }
                        meteorDropped = GenerateAstralMeteor(x, y);

                        // If the meteor actually dropped, post the message stating as such.
                        if (meteorDropped)
                        {
                            string key = "Mods.CalamityMod.AstralText";
                            Color messageColor = Color.Gold;

                            if (Main.netMode == NetmodeID.SinglePlayer)
                                Main.NewText(Language.GetTextValue(key), messageColor);
                            else if (Main.netMode == NetmodeID.Server)
                                NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                            break;
                        }
                        break;
                    }
                    else
                    {
                        y++;
                    }
                }
                if (solidTileRequirement < 100f)
                {
                    return;
                }
            }
        }

        public static bool GenerateAstralMeteor(int i, int j)
        {
            UnifiedRandom rand = WorldGen.genRand;
            if (i < 50 || i > Main.maxTilesX - 50)
            {
                return false;
            }
            // Avoid the dungeon so that the beacon doesn't eat it.
            if (Math.Abs(i - WorldGen.dungeonX) < 65)
            {
                return false;
            }
            if (j < 50 || j > Main.maxTilesY - 50)
            {
                return false;
            }
            int num = 35;
            Rectangle rectangle = new Rectangle((i - num) * 16, (j - num) * 16, num * 2 * 16, num * 2 * 16);
            for (int k = 0; k < 255; k++)
            {
                if (Main.player[k].active)
                {
                    Rectangle value = new Rectangle((int)(Main.player[k].position.X + (float)(Main.player[k].width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(Main.player[k].position.Y + (float)(Main.player[k].height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                    if (rectangle.Intersects(value))
                    {
                        return false;
                    }
                }
            }
            for (int l = 0; l < 200; l++)
            {
                if (Main.npc[l].active)
                {
                    Rectangle value2 = new Rectangle((int)Main.npc[l].position.X, (int)Main.npc[l].position.Y, Main.npc[l].width, Main.npc[l].height);
                    if (rectangle.Intersects(value2))
                    {
                        return false;
                    }
                }
            }
            for (int m = i - num; m < i + num; m++)
            {
                for (int n = j - num; n < j + num; n++)
                {
                    if (Main.tile[m, n].active() && Main.tile[m, n].type == 21)
                    {
                        return false;
                    }
                }
            }
            num = rand.Next(17, 23);
            for (int num2 = i - num; num2 < i + num; num2++)
            {
                for (int num3 = j - num; num3 < j + num; num3++)
                {
                    if (num3 > j + rand.Next(-2, 3) - 5)
                    {
                        float num4 = (float)Math.Abs(i - num2);
                        float num5 = (float)Math.Abs(j - num3);
                        float num6 = (float)Math.Sqrt((double)(num4 * num4 + num5 * num5));
                        if ((double)num6 < (double)num * 0.9 + (double)rand.Next(-4, 5))
                        {
                            if (Main.tile[num2, num3] != null)
                            {
                                if (!Main.tileSolid[(int)Main.tile[num2, num3].type])
                                {
                                    Main.tile[num2, num3].active(false);
                                }
                                Main.tile[num2, num3].type = (ushort)ModContent.TileType<AstralOre>();
                            }
                        }
                    }
                }
            }
            num = WorldGen.genRand.Next(8, 14);
            for (int num7 = i - num; num7 < i + num; num7++)
            {
                for (int num8 = j - num; num8 < j + num; num8++)
                {
                    if (num8 > j + rand.Next(-2, 3) - 4)
                    {
                        float num9 = (float)Math.Abs(i - num7);
                        float num10 = (float)Math.Abs(j - num8);
                        float num11 = (float)Math.Sqrt((double)(num9 * num9 + num10 * num10));
                        if ((double)num11 < (double)num * 0.8 + (double)rand.Next(-3, 4))
                        {
                            if (Main.tile[num7, num8] != null)
                                Main.tile[num7, num8].active(false);
                        }
                    }
                }
            }
            num = WorldGen.genRand.Next(25, 35);
            for (int num12 = i - num; num12 < i + num; num12++)
            {
                for (int num13 = j - num; num13 < j + num; num13++)
                {
                    float num14 = (float)Math.Abs(i - num12);
                    float num15 = (float)Math.Abs(j - num13);
                    float num16 = (float)Math.Sqrt((double)(num14 * num14 + num15 * num15));
                    if (Main.tile[num12, num13] != null)
                    {
                        if ((double)num16 < (double)num * 0.7)
                        {
                            if (Main.tile[num12, num13].type == 5 || Main.tile[num12, num13].type == 32 || Main.tile[num12, num13].type == 352)
                            {
                                try
                                { WorldGen.KillTile(num12, num13, false, false, true); }
                                catch (NullReferenceException)
                                { }
                            }
                            Main.tile[num12, num13].liquid = 0;
                        }
                        if (Main.tile[num12, num13].type == (ushort)ModContent.TileType<AstralOre>())
                        {
                            if (!WorldGen.SolidTile(num12 - 1, num13) && !WorldGen.SolidTile(num12 + 1, num13) && !WorldGen.SolidTile(num12, num13 - 1) && !WorldGen.SolidTile(num12, num13 + 1))
                            {
                                Main.tile[num12, num13].active(false);
                            }
                            else if ((Main.tile[num12, num13].halfBrick() || Main.tile[num12 - 1, num13].topSlope()) && !WorldGen.SolidTile(num12, num13 + 1))
                            {
                                Main.tile[num12, num13].active(false);
                            }
                        }
                        WorldGen.SquareTileFrame(num12, num13, true);
                        WorldGen.SquareWallFrame(num12, num13, true);
                    }
                }
            }
            num = WorldGen.genRand.Next(23, 32);
            for (int num17 = i - num; num17 < i + num; num17++)
            {
                for (int num18 = j - num; num18 < j + num; num18++)
                {
                    if (num18 > j + WorldGen.genRand.Next(-3, 4) - 3 && Main.tile[num17, num18].active() && rand.NextBool(10))
                    {
                        float num19 = (float)Math.Abs(i - num17);
                        float num20 = (float)Math.Abs(j - num18);
                        float num21 = (float)Math.Sqrt((double)(num19 * num19 + num20 * num20));
                        if ((double)num21 < (double)num * 0.8)
                        {
                            if (Main.tile[num17, num18] != null)
                            {
                                if (Main.tile[num17, num18].type == 5 || Main.tile[num17, num18].type == 32 || Main.tile[num17, num18].type == 352)
                                {
                                    WorldGen.KillTile(num17, num18, false, false, false);
                                }
                                Main.tile[num17, num18].type = (ushort)ModContent.TileType<AstralOre>();
                                WorldGen.SquareTileFrame(num17, num18, true);
                            }
                        }
                    }
                }
            }
            num = WorldGen.genRand.Next(30, 38);
            for (int num22 = i - num; num22 < i + num; num22++)
            {
                for (int num23 = j - num; num23 < j + num; num23++)
                {
                    if (num23 > j + WorldGen.genRand.Next(-2, 3) && Main.tile[num22, num23].active() && rand.NextBool(20))
                    {
                        float num24 = (float)Math.Abs(i - num22);
                        float num25 = (float)Math.Abs(j - num23);
                        float num26 = (float)Math.Sqrt((double)(num24 * num24 + num25 * num25));
                        if ((double)num26 < (double)num * 0.85)
                        {
                            if (Main.tile[num22, num23] != null)
                            {
                                if (Main.tile[num22, num23].type == 5 || Main.tile[num22, num23].type == 32 || Main.tile[num22, num23].type == 352)
                                {
                                    WorldGen.KillTile(num22, num23, false, false, false);
                                }
                                Main.tile[num22, num23].type = (ushort)ModContent.TileType<AstralOre>();
                                WorldGen.SquareTileFrame(num22, num23, true);
                            }
                        }
                    }
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(-1, i, j, 40, TileChangeType.None);
                if (CanAstralBiomeSpawn())
                {
                    DoAstralConversion(new Point(i, j));
                    int checkWidth = 180;
                    float averageHeight = 0f;
                    float lowestHeight = 0f;
                    for (int x = i - checkWidth / 2; x < i + checkWidth / 2; x++)
                    {
                        int y = j - 200;
                        while (!Main.tileSolid[CalamityUtils.ParanoidTileRetrieval(x, y).type] || !CalamityUtils.ParanoidTileRetrieval(x, y).active())
                        {
                            y++;
                            if (y > j - 10)
                                break;
                        }
                        lowestHeight = (int)MathHelper.Max(lowestHeight, y);
                        averageHeight += y;
                    }
                    lowestHeight -= 35f;
                    averageHeight /= checkWidth;
                    float height = lowestHeight;

                    // If there's a sudden change between the average and lowest height (which is indicative of holes/chasms), go with the average.
                    if (Math.Abs(lowestHeight - averageHeight) > 50f)
                        height = averageHeight;

                    // WorldGen.gen prevents NewItem from working, and thus prevents a bunch of dumb items from being spawned immediately and deleting the WoF/Aureus loot in the process.
                    WorldGen.gen = true;
                    SchematicPlacementHelpers.PlaceStructure("Astral Beacon", new Point(i, (int)height - 30), SchematicPlacementHelpers.PlacementAnchorType.Center);
                    WorldGen.gen = false;
                }
            }
            return true;
        }

        public static void DoAstralConversion(object obj)
        {
            //Pre-calculate all variables necessary for elliptical area checking
            Point origin = (Point)obj;
            Vector2 center = origin.ToVector2() * 16f + new Vector2(8f);

            float angle = MathHelper.Pi * 0.15f;
            float otherAngle = MathHelper.PiOver2 - angle;

            int distanceInTiles = 150 + (Main.maxTilesX - 4200) / 4200 * 200;
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
                            if (rand.NextFloat(1f) > outerEdgePercent)
                            {
                                ConvertToAstral(x, y);
                            }
                        }
                        else
                        {
                            ConvertToAstral(x, y);
                        }
                    }
                }
            }
        }

        public static void ConvertToAstral(int startX, int endX, int startY, int endY)
        {
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    ConvertToAstral(x, y);
                }
            }
        }

        public static void ConvertToAstral(int x, int y, bool tileframe = true)
        {
            if (WorldGen.InWorld(x, y, 1))
            {
                int type = Main.tile[x, y].type;
                int wallType = Main.tile[x, y].wall;

                if (Main.tile[x, y] != null)
                {
                    if (WallID.Sets.Conversion.Grass[wallType])
                    {
                        Main.tile[x, y].wall = (ushort)ModContent.WallType<AstralGrassWall>();
                    }
                    else if (WallID.Sets.Conversion.HardenedSand[wallType])
                    {
                        Main.tile[x, y].wall = (ushort)ModContent.WallType<HardenedAstralSandWall>();
                    }
                    else if (WallID.Sets.Conversion.Sandstone[wallType])
                    {
                        Main.tile[x, y].wall = (ushort)ModContent.WallType<AstralSandstoneWall>();
                    }
                    else if (WallID.Sets.Conversion.Stone[wallType])
                    {
                        Main.tile[x, y].wall = (ushort)ModContent.WallType<AstralStoneWall>();
                    }
                    else
                    {
                        switch (wallType)
                        {
                            case WallID.DirtUnsafe:
                            case WallID.DirtUnsafe1:
                            case WallID.DirtUnsafe2:
                            case WallID.DirtUnsafe3:
                            case WallID.DirtUnsafe4:
                            case WallID.Cave6Unsafe:
                            case WallID.Dirt:
                                Main.tile[x, y].wall = (ushort)ModContent.WallType<AstralDirtWall>();
                                break;
                            case WallID.SnowWallUnsafe:
                                Main.tile[x, y].wall = (ushort)ModContent.WallType<AstralSnowWall>();
                                break;
                            case WallID.DesertFossil:
                                Main.tile[x, y].wall = (ushort)ModContent.WallType<AstralFossilWall>();
                                break;
                            case WallID.IceUnsafe:
                                Main.tile[x, y].wall = (ushort)ModContent.WallType<AstralIceWall>();
                                break;
                            case WallID.LivingWood:
                                Main.tile[x, y].wall = (ushort)ModContent.WallType<AstralMonolithWall>();
                                break;
                        }
                    }
                    if (TileID.Sets.Conversion.Grass[type] && !TileID.Sets.GrassSpecial[type])
                    {
                        Main.tile[x, y].type = (ushort)ModContent.TileType<AstralGrass>();
                    }
                    else if (TileID.Sets.Conversion.Stone[type] || Main.tileMoss[type])
                    {
                        Main.tile[x, y].type = (ushort)ModContent.TileType<AstralStone>();
                    }
                    else if (TileID.Sets.Conversion.Sand[type])
                    {
                        Main.tile[x, y].type = (ushort)ModContent.TileType<AstralSand>();
                    }
                    else if (TileID.Sets.Conversion.HardenedSand[type])
                    {
                        Main.tile[x, y].type = (ushort)ModContent.TileType<HardenedAstralSand>();
                    }
                    else if (TileID.Sets.Conversion.Sandstone[type])
                    {
                        Main.tile[x, y].type = (ushort)ModContent.TileType<AstralSandstone>();
                    }
                    else if (TileID.Sets.Conversion.Ice[type])
                    {
                        Main.tile[x, y].type = (ushort)ModContent.TileType<AstralIce>();
                    }
                    else
                    {
                        Tile tile = Main.tile[x, y];
                        switch (type)
                        {
                            case TileID.Dirt:
                                Main.tile[x, y].type = (ushort)ModContent.TileType<AstralDirt>();
                                break;
                            case TileID.SnowBlock:
                                Main.tile[x, y].type = (ushort)ModContent.TileType<AstralSnow>();
                                break;
                            case TileID.Silt:
                            case TileID.Slush:
                                Main.tile[x, y].type = (ushort)ModContent.TileType<AstralSilt>();
                                break;
                            case TileID.DesertFossil:
                                Main.tile[x, y].type = (ushort)ModContent.TileType<AstralFossil>();
                                break;
                            case TileID.ClayBlock:
                                Main.tile[x, y].type = (ushort)ModContent.TileType<AstralClay>();
                                break;
                            case TileID.Vines:
                                Main.tile[x, y].type = (ushort)ModContent.TileType<AstralVines>();
                                break;
                            case TileID.LivingWood:
                                Main.tile[x, y].type = (ushort)ModContent.TileType<AstralMonolith>();
                                break;
                            case TileID.LeafBlock:
                                WorldGen.KillTile(x, y);
								if (Main.netMode == NetmodeID.MultiplayerClient)
								{
									NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, x, y);
								}
                                break;
                            case TileID.LargePiles:
                                if (tile.frameX <= 1170)
                                {
                                    RecursiveReplaceToAstral(TileID.LargePiles, (ushort)ModContent.TileType<AstralNormalLargePiles>(), x, y, 324, 0, 1170, 0, 18);
                                }
                                if (tile.frameX >= 1728)
                                {
                                    RecursiveReplaceToAstral(TileID.LargePiles, (ushort)ModContent.TileType<AstralNormalLargePiles>(), x, y, 324, 1728, 1872, 0, 18);
                                }
                                if (tile.frameX >= 1404 && tile.frameX <= 1710)
                                {
                                    RecursiveReplaceToAstral(TileID.LargePiles, (ushort)ModContent.TileType<AstralIceLargePiles>(), x, y, 324, 1404, 1710, 0, 18);
                                }
                                break;
                            case TileID.LargePiles2:
                                if (tile.frameX >= 1566 && tile.frameY < 36)
                                {
                                    RecursiveReplaceToAstral(TileID.LargePiles2, (ushort)ModContent.TileType<AstralDesertLargePiles>(), x, y, 324, 1566, 1872, 0, 18);
                                }
                                if (tile.frameX >= 756 && tile.frameX <= 900)
                                {
                                    RecursiveReplaceToAstral(TileID.LargePiles2, (ushort)ModContent.TileType<AstralNormalLargePiles>(), x, y, 324, 756, 900, 0, 18);
                                }
                                break;
                            case TileID.SmallPiles:
                                if (tile.frameY == 18)
                                {
                                    ushort newType;
                                    if (tile.frameX >= 1476 && tile.frameX <= 1674)
                                    {
                                        newType = (ushort)ModContent.TileType<AstralDesertMediumPiles>();
                                    }
                                    else if (tile.frameX <= 558 || (tile.frameX >= 1368 && tile.frameX <= 1458))
                                    {
                                        newType = (ushort)ModContent.TileType<AstralNormalMediumPiles>();
                                    }
                                    else if (tile.frameX >= 900 && tile.frameX <= 1098)
                                    {
                                        newType = (ushort)ModContent.TileType<AstralIceMediumPiles>();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    int leftMost = x;
                                    if (tile.frameX % 36 != 0) //this means it's the right tile of the two
                                    {
                                        leftMost--;
                                    }
                                    if (Main.tile[leftMost, y] != null)
                                        Main.tile[leftMost, y].type = newType;
                                    if (Main.tile[leftMost + 1, y] != null)
                                        Main.tile[leftMost + 1, y].type = newType;
                                    while (Main.tile[leftMost, y].frameX >= 216)
                                    {
                                        if (Main.tile[leftMost, y] != null)
                                            Main.tile[leftMost, y].frameX -= 216;
                                        if (Main.tile[leftMost + 1, y] != null)
                                            Main.tile[leftMost + 1, y].frameX -= 216;
                                    }
                                }
                                else if (tile.frameY == 0)
                                {
                                    ushort newType3;
                                    if (tile.frameX >= 972 && tile.frameX <= 1062)
                                    {
                                        newType3 = (ushort)ModContent.TileType<AstralDesertSmallPiles>();
                                    }
                                    else if (tile.frameX <= 486)
                                    {
                                        newType3 = (ushort)ModContent.TileType<AstralNormalSmallPiles>();
                                    }
                                    else if (tile.frameX >= 648 && tile.frameX <= 846)
                                    {
                                        newType3 = (ushort)ModContent.TileType<AstralIceSmallPiles>();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    Main.tile[x, y].type = newType3;
                                    while (Main.tile[x, y].frameX >= 108) //REFRAME IT
                                    {
                                        Main.tile[x, y].frameX -= 108;
                                    }
                                }
                                break;
                            case TileID.Stalactite:
                                int topMost = tile.frameY <= 54 ? (tile.frameY % 36 == 0 ? y : y - 1) : y;
                                bool twoTall = tile.frameY <= 54;
                                bool hanging = tile.frameY <= 18 || tile.frameY == 72;
                                ushort newType2;
                                if (tile.frameX >= 378 && tile.frameX <= 414) //DESERT
                                {
                                    newType2 = (ushort)ModContent.TileType<AstralDesertStalactite>();
                                }
                                else if ((tile.frameX >= 54 && tile.frameX <= 90) || (tile.frameX >= 216 && tile.frameX <= 360))
                                {
                                    newType2 = (ushort)ModContent.TileType<AstralNormalStalactite>();
                                }
                                else if (tile.frameX <= 36)
                                {
                                    newType2 = (ushort)ModContent.TileType<AstralIceStalactite>();
                                }
                                else
                                {
                                    break;
                                }

                                //Set types
                                if (Main.tile[x, topMost] != null)
                                    Main.tile[x, topMost].type = newType2;
                                if (twoTall)
                                {
                                    if (Main.tile[x, topMost + 1] != null)
                                        Main.tile[x, topMost + 1].type = newType2;
                                }

                                //Fix frames
                                while (Main.tile[x, topMost].frameX >= 54)
                                {
                                    if (Main.tile[x, topMost] != null)
                                        Main.tile[x, topMost].frameX -= 54;
                                    if (twoTall)
                                    {
                                        if (Main.tile[x, topMost + 1] != null)
                                            Main.tile[x, topMost + 1].frameX -= 54;
                                    }
                                }

                                if (hanging)
                                {
                                    ConvertToAstral(x, topMost - 1);
                                    break;
                                }
                                else
                                {
                                    if (twoTall)
                                    {
                                        ConvertToAstral(x, topMost + 2);
                                        break;
                                    }
                                    ConvertToAstral(x, topMost + 1);
                                    break;
                                }
                        }
                    }
                    if (tileframe)
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            WorldGen.SquareTileFrame(x, y, true);
                        }
                        else if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendTileSquare(-1, x, y, 1);
                        }
                    }
                }
            }
        }

        public static void ConvertFromAstral(int x, int y, ConvertType convert)
        {
            Tile tile = Main.tile[x, y];
            int type = tile.type;
            int wallType = tile.wall;
            Mod mod = ModContent.GetInstance<CalamityMod>();

            if (WorldGen.InWorld(x, y, 1))
            {
                #region WALL
                if (Main.tile[x, y] != null)
                {
                    if (wallType == ModContent.WallType<AstralDirtWall>())
                    {
                        Main.tile[x, y].wall = WallID.DirtUnsafe;
                    }
                    else if (wallType == ModContent.WallType<AstralSnowWall>() || wallType == ModContent.WallType<AstralSnowWallSafe>())
                    {
                        Main.tile[x, y].wall = WallID.SnowWallUnsafe;
                    }
                    else if (wallType == ModContent.WallType<AstralFossilWall>())
                    {
                        Main.tile[x, y].wall = WallID.DesertFossil;
                    }
                    else if (wallType == ModContent.WallType<AstralGrassWall>())
                    {
                        switch (convert)
                        {
                            case ConvertType.Corrupt:
                                Main.tile[x, y].wall = WallID.CorruptGrassUnsafe;
                                break;
                            case ConvertType.Crimson:
                                Main.tile[x, y].wall = WallID.CrimsonGrassUnsafe;
                                break;
                            case ConvertType.Hallow:
                                Main.tile[x, y].wall = WallID.HallowedGrassUnsafe;
                                break;
                            case ConvertType.Pure:
                                Main.tile[x, y].wall = WallID.GrassUnsafe;
                                break;
                        }
                    }
                    else if (wallType == ModContent.WallType<AstralIceWall>())
                    {
                        Main.tile[x, y].wall = WallID.IceUnsafe;
                    }
                    else if (wallType == ModContent.WallType<AstralMonolithWall>())
                    {
                        Main.tile[x, y].wall = WallID.LivingWood;
                    }
                    else if (wallType == ModContent.WallType<AstralStoneWall>())
                    {
                        switch (convert)
                        {
                            case ConvertType.Corrupt:
                                Main.tile[x, y].wall = WallID.EbonstoneUnsafe;
                                break;
                            case ConvertType.Crimson:
                                Main.tile[x, y].wall = WallID.CrimstoneUnsafe;
                                break;
                            case ConvertType.Hallow:
                                Main.tile[x, y].wall = WallID.PearlstoneBrickUnsafe;
                                break;
                            case ConvertType.Pure:
                                Main.tile[x, y].wall = WallID.Stone;
                                break;
                        }
                    }
                }
                #endregion

                #region TILE
                if (Main.tile[x, y] != null)
                {
                    if (type == ModContent.TileType<AstralDirt>())
                    {
                        tile.type = TileID.Dirt;
                    }
                    else if (type == ModContent.TileType<AstralSnow>())
                    {
                        tile.type = TileID.SnowBlock;
                    }
                    else if (type == ModContent.TileType<AstralSilt>())
                    {
                        tile.type = TileID.Silt;
                    }
                    else if (type == ModContent.TileType<AstralFossil>())
                    {
                        tile.type = TileID.DesertFossil;
                    }
                    else if (type == ModContent.TileType<AstralClay>())
                    {
                        tile.type = TileID.ClayBlock;
                    }
                    else if (type == ModContent.TileType<AstralGrass>())
                    {
                        SetTileFromConvert(x, y, convert, TileID.CorruptGrass, TileID.FleshGrass, TileID.HallowedGrass, TileID.Grass);
                    }
                    else if (type == ModContent.TileType<AstralStone>())
                    {
                        SetTileFromConvert(x, y, convert, TileID.Ebonstone, TileID.Crimstone, TileID.Pearlstone, TileID.Stone);
                    }
                    else if (type == ModContent.TileType<AstralMonolith>())
                    {
                        tile.type = TileID.LivingWood;
                    }
                    else if (type == ModContent.TileType<AstralSand>())
                    {
                        SetTileFromConvert(x, y, convert, TileID.Ebonsand, TileID.Crimsand, TileID.Pearlsand, TileID.Sand);
                    }
                    else if (type == ModContent.TileType<AstralSandstone>())
                    {
                        SetTileFromConvert(x, y, convert, TileID.CorruptSandstone, TileID.CrimsonSandstone, TileID.HallowSandstone, TileID.Sandstone);
                    }
                    else if (type == ModContent.TileType<HardenedAstralSand>())
                    {
                        SetTileFromConvert(x, y, convert, TileID.CorruptHardenedSand, TileID.CrimsonHardenedSand, TileID.HallowHardenedSand, TileID.HardenedSand);
                    }
                    else if (type == ModContent.TileType<AstralIce>())
                    {
                        SetTileFromConvert(x, y, convert, TileID.CorruptIce, TileID.FleshIce, TileID.HallowedIce, TileID.IceBlock);
                    }
                    else if (type == ModContent.TileType<AstralVines>())
                    {
                        SetTileFromConvert(x, y, convert, ushort.MaxValue, TileID.CrimsonVines, TileID.HallowedVines, TileID.Vines);
                    }
                    else if (type == ModContent.TileType<AstralShortPlants>())
                    {
                        SetTileFromConvert(x, y, convert, TileID.CorruptPlants, ushort.MaxValue, TileID.HallowedPlants, TileID.Plants);
                    }
                    else if (type == ModContent.TileType<AstralTallPlants>())
                    {
                        SetTileFromConvert(x, y, convert, ushort.MaxValue, ushort.MaxValue, TileID.HallowedPlants2, TileID.Plants2);
                    }
                    else if (type == ModContent.TileType<AstralNormalLargePiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.LargePiles, x, y, 378, 0);
                    }
                    else if (type == ModContent.TileType<AstralNormalMediumPiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 0, 18);
                    }
                    else if (type == ModContent.TileType<AstralNormalSmallPiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 0, 0);
                    }
                    else if (type == ModContent.TileType<AstralDesertLargePiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.LargePiles2, x, y, 1566, 0);
                    }
                    else if (type == ModContent.TileType<AstralDesertMediumPiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 1476, 18);
                    }
                    else if (type == ModContent.TileType<AstralDesertSmallPiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 972, 0);
                    }
                    else if (type == ModContent.TileType<AstralIceLargePiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.LargePiles, x, y, 1404, 0);
                    }
                    else if (type == ModContent.TileType<AstralIceMediumPiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 900, 18);
                    }
                    else if (type == ModContent.TileType<AstralIceSmallPiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 648, 0);
                    }
                    else if (type == ModContent.TileType<AstralNormalStalactite>())
                    {
                        ushort originType = TileID.Stone;
                        int frameXAdd = 54;
                        switch (convert)
                        {
                            case ConvertType.Corrupt:
                                originType = TileID.Ebonstone;
                                frameXAdd = 324;
                                break;
                            case ConvertType.Crimson:
                                originType = TileID.Crimstone;
                                frameXAdd = 270;
                                break;
                            case ConvertType.Hallow:
                                originType = TileID.Pearlstone;
                                frameXAdd = 216;
                                break;
                        }
                        ReplaceAstralStalactite((ushort)type, TileID.Stalactite, originType, x, y, frameXAdd, 0);
                    }
                    else if (type == ModContent.TileType<AstralDesertStalactite>())
                    {
                        ushort originType = TileID.Sandstone;
                        int frameXAdd = 378;
                        switch (convert)
                        {
                            case ConvertType.Corrupt:
                                originType = TileID.CorruptSandstone;
                                frameXAdd = 324;
                                break;
                            case ConvertType.Crimson:
                                originType = TileID.CrimsonSandstone;
                                frameXAdd = 270;
                                break;
                            case ConvertType.Hallow:
                                originType = TileID.HallowSandstone;
                                frameXAdd = 216;
                                break;
                        }
                        ReplaceAstralStalactite((ushort)type, TileID.Stalactite, originType, x, y, frameXAdd, 0);
                    }
                    else if (type == ModContent.TileType<AstralIceStalactite>())
                    {
                        ReplaceAstralStalactite((ushort)type, TileID.Stalactite, TileID.IceBlock, x, y, 0, 0);
                    }
                    if (TileID.Sets.Conversion.Grass[type] || type == TileID.Dirt)
                    {
                        WorldGen.SquareTileFrame(x, y);
                    }
                }
                #endregion
            }
        }

        public static void SetTileFromConvert(int x, int y, ConvertType convert, ushort corrupt, ushort crimson, ushort hallow, ushort pure)
        {
            switch (convert)
            {
                case ConvertType.Corrupt:
                    if (corrupt != ushort.MaxValue)
                    {
                        Main.tile[x, y].type = corrupt;
                        WorldGen.SquareTileFrame(x, y);
                    }
                    break;
                case ConvertType.Crimson:
                    if (crimson != ushort.MaxValue)
                    {
                        Main.tile[x, y].type = crimson;
                        WorldGen.SquareTileFrame(x, y);
                    }
                    break;
                case ConvertType.Hallow:
                    if (hallow != ushort.MaxValue)
                    {
                        Main.tile[x, y].type = hallow;
                        WorldGen.SquareTileFrame(x, y);
                    }
                    break;
                case ConvertType.Pure:
                    if (pure != ushort.MaxValue)
                    {
                        Main.tile[x, y].type = pure;
                        WorldGen.SquareTileFrame(x, y);
                    }
                    break;
            }
        }

        public static void RecursiveReplaceToAstral(ushort checkType, ushort replaceType, int x, int y, int replaceTextureWidth, int minFrameX = 0, int maxFrameX = int.MaxValue, int minFrameY = 0, int maxFrameY = int.MaxValue)
        {
            Tile tile = Main.tile[x, y];
            if (tile == null || !tile.active() || tile.type != checkType || tile.frameX < minFrameX || tile.frameX > maxFrameX || tile.frameY < minFrameY || tile.frameY > maxFrameY)
                return;

            Main.tile[x, y].type = replaceType;
            while (Main.tile[x, y].frameX >= replaceTextureWidth)
            {
                Main.tile[x, y].frameX -= (short)replaceTextureWidth;
            }

            if (Main.tile[x - 1, y] != null)
                RecursiveReplaceToAstral(checkType, replaceType, x - 1, y, replaceTextureWidth, minFrameX, maxFrameX, minFrameY, maxFrameY);
            if (Main.tile[x + 1, y] != null)
                RecursiveReplaceToAstral(checkType, replaceType, x + 1, y, replaceTextureWidth, minFrameX, maxFrameX, minFrameY, maxFrameY);
            if (Main.tile[x, y - 1] != null)
                RecursiveReplaceToAstral(checkType, replaceType, x, y - 1, replaceTextureWidth, minFrameX, maxFrameX, minFrameY, maxFrameY);
            if (Main.tile[x, y + 1] != null)
                RecursiveReplaceToAstral(checkType, replaceType, x, y + 1, replaceTextureWidth, minFrameX, maxFrameX, minFrameY, maxFrameY);
        }

        public static void RecursiveReplaceFromAstral(ushort checkType, ushort replaceType, int x, int y, int addFrameX, int addFrameY)
        {
            Tile tile = Main.tile[x, y];
            if (tile == null || !tile.active() || tile.type != checkType)
                return;

            Main.tile[x, y].type = replaceType;
            Main.tile[x, y].frameX += (short)addFrameX;
            Main.tile[x, y].frameY += (short)addFrameY;

            if (Main.tile[x - 1, y] != null)
                RecursiveReplaceFromAstral(checkType, replaceType, x - 1, y, addFrameX, addFrameY);
            if (Main.tile[x + 1, y] != null)
                RecursiveReplaceFromAstral(checkType, replaceType, x + 1, y, addFrameX, addFrameY);
            if (Main.tile[x, y - 1] != null)
                RecursiveReplaceFromAstral(checkType, replaceType, x, y - 1, addFrameX, addFrameY);
            if (Main.tile[x, y + 1] != null)
                RecursiveReplaceFromAstral(checkType, replaceType, x, y + 1, addFrameX, addFrameY);
        }

        public static void ReplaceAstralStalactite(ushort checkType, ushort replaceType, ushort replaceOriginTile, int x, int y, int addFrameX, int addFrameY)
        {
            Tile tile = Main.tile[x, y];

            int topMost = tile.frameY <= 54 ? (tile.frameY % 36 == 0 ? y : y - 1) : y;
            bool twoTall = tile.frameY <= 54;
            bool hanging = tile.frameY <= 18 || tile.frameY == 72;

            int yOriginTile = hanging ? topMost - 1 : (twoTall ? topMost + 2 : y + 1);

            if (Main.tile[x, topMost++] != null)
                Main.tile[x, topMost++].type = replaceType;
            if (twoTall)
            {
                if (Main.tile[x, topMost] != null)
                    Main.tile[x, topMost].type = replaceType;
            }
            if (Main.tile[x, yOriginTile] != null)
                Main.tile[x, yOriginTile].type = replaceOriginTile;
        }

        public static bool CheckInEllipse(Point tile, Vector2 focus1, Vector2 focus2, float distanceConstant, Vector2 center, out float distance, bool collapse = false)
        {
            Vector2 point = tile.ToVector2() * 16f + new Vector2(8f);
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
        #endregion

        #region EvilIsland
        public static void EvilIsland(int i, int j)
        {
            double num = (double)WorldGen.genRand.Next(100, 150);
            float num2 = (float)WorldGen.genRand.Next(20, 30);
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
                if (num9 < 0)
                {
                    num9 = 0;
                }
                if (num10 > Main.maxTilesY)
                {
                    num10 = Main.maxTilesY;
                }
                double num11 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01; //80 120
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
                                Main.tile[k, l].active(true);
                                Main.tile[k, l].type = (ushort)(WorldGen.crimson ? 400 : 401); //ebonstone or crimstone
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
                while (!Main.tile[m, num14].active())
                {
                    num14--;
                }
                num14 += WorldGen.genRand.Next(-3, 4);
                num15 = WorldGen.genRand.Next(4, 8);
                int num16 = WorldGen.crimson ? 400 : 401;
                if (WorldGen.genRand.Next(4) == 0)
                {
                    num16 = WorldGen.crimson ? 398 : 399;
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
                                Main.tile[n, num17].active(true);
                                Main.tile[n, num17].type = (ushort)num16;
                                WorldGen.SquareTileFrame(n, num17, true);
                            }
                        }
                    }
                }
            }
            num = (double)WorldGen.genRand.Next(80, 95);
            num2 = (float)WorldGen.genRand.Next(10, 15);
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
                int num7 = (int)((double)vector.X - num * 0.5);
                int num8 = (int)((double)vector.X + num * 0.5);
                int num9 = num5 - 1;
                int num10 = (int)((double)vector.Y + num * 0.5);
                if (num7 < 0)
                {
                    num7 = 0;
                }
                if (num8 > Main.maxTilesX)
                {
                    num8 = Main.maxTilesX;
                }
                if (num9 < 0)
                {
                    num9 = 0;
                }
                if (num10 > Main.maxTilesY)
                {
                    num10 = Main.maxTilesY;
                }
                double num11 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01;
                float num19 = vector.Y + 1f;
                for (int num20 = num7; num20 < num8; num20++)
                {
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num19 += (float)WorldGen.genRand.Next(-1, 2);
                    }
                    if (num19 < vector.Y)
                    {
                        num19 = vector.Y;
                    }
                    if (num19 > vector.Y + 2f)
                    {
                        num19 = vector.Y + 2f;
                    }
                    for (int num21 = num9; num21 < num10; num21++)
                    {
                        if ((float)num21 > num19)
                        {
                            float arg_69E_0 = Math.Abs((float)num20 - vector.X);
                            float num22 = Math.Abs((float)num21 - vector.Y) * 3f;
                            if (Math.Sqrt((double)(arg_69E_0 * arg_69E_0 + num22 * num22)) < num11 * 0.4 &&
                                Main.tile[num20, num21].type == (WorldGen.crimson ? 400 : 401))
                            {
                                Main.tile[num20, num21].type = (ushort)(WorldGen.crimson ? 22 : 204); //ore
                                WorldGen.SquareTileFrame(num20, num21, true);
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
            int num23 = num3;
            num23 += WorldGen.genRand.Next(5);
            while (num23 < num4)
            {
                int num24 = num6;
                while ((!Main.tile[num23, num24].active() || Main.tile[num23, num24].type != 0) && num23 < num4)
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
                    int num26 = WorldGen.crimson ? 400 : 401;
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
                                    Main.tile[num27, num28].type = (ushort)num26;
                                    WorldGen.SquareTileFrame(num27, num28, true);
                                }
                            }
                        }
                    }
                    num23 += WorldGen.genRand.Next(num25, (int)((double)num25 * 1.5));
                }
            }
            for (int num30 = num3 - 20; num30 <= num4 + 20; num30++)
            {
                for (int num31 = num5 - 20; num31 <= num6 + 20; num31++)
                {
                    bool flag = true;
                    for (int num32 = num30 - 1; num32 <= num30 + 1; num32++)
                    {
                        for (int num33 = num31 - 1; num33 <= num31 + 1; num33++)
                        {
                            if (!Main.tile[num32, num33].active())
                            {
                                flag = false;
                            }
                        }
                    }
                    if (flag)
                    {
                        Main.tile[num30, num31].wall = (ushort)(WorldGen.crimson ? 220 : 221);
                        WorldGen.SquareWallFrame(num30, num31, true);
                    }
                }
            }
            for (int num34 = num3; num34 <= num4; num34++)
            {
                int num35 = num5 - 10;
                while (!Main.tile[num34, num35 + 1].active())
                {
                    num35++;
                }
                if (num35 < num6 && Main.tile[num34, num35 + 1].type == (WorldGen.crimson ? 400 : 401))
                {
                    if (WorldGen.genRand.Next(10) == 0)
                    {
                        int num36 = WorldGen.genRand.Next(1, 3);
                        for (int num37 = num34 - num36; num37 <= num34 + num36; num37++)
                        {
                            if (Main.tile[num37, num35].type == (WorldGen.crimson ? 400 : 401))
                            {
                                Main.tile[num37, num35].active(false);
                                Main.tile[num37, num35].liquid = 255;
                                Main.tile[num37, num35].lava(false);
                                WorldGen.SquareTileFrame(num34, num35, true);
                            }
                            if (Main.tile[num37, num35 + 1].type == (WorldGen.crimson ? 400 : 401))
                            {
                                Main.tile[num37, num35 + 1].active(false);
                                Main.tile[num37, num35 + 1].liquid = 255;
                                Main.tile[num37, num35 + 1].lava(false);
                                WorldGen.SquareTileFrame(num34, num35 + 1, true);
                            }
                            if (num37 > num34 - num36 && num37 < num34 + 2 && Main.tile[num37, num35 + 2].type == (WorldGen.crimson ? 400 : 401))
                            {
                                Main.tile[num37, num35 + 2].active(false);
                                Main.tile[num37, num35 + 2].liquid = 255;
                                Main.tile[num37, num35 + 2].lava(false);
                                WorldGen.SquareTileFrame(num34, num35 + 2, true);
                            }
                        }
                    }
                    if (WorldGen.genRand.Next(5) == 0)
                    {
                        Main.tile[num34, num35].liquid = 255;
                    }
                    Main.tile[num34, num35].lava(false);
                    WorldGen.SquareTileFrame(num34, num35, true);
                }
            }
            int num38 = WorldGen.genRand.Next(4);
            for (int num39 = 0; num39 <= num38; num39++)
            {
                int num40 = WorldGen.genRand.Next(num3 - 5, num4 + 5);
                int num41 = num5 - WorldGen.genRand.Next(20, 40);
                int num42 = WorldGen.genRand.Next(4, 8);
                int num43 = WorldGen.crimson ? 400 : 401;
                if (WorldGen.genRand.Next(2) == 0)
                {
                    num43 = WorldGen.crimson ? 398 : 399;
                }
                for (int num44 = num40 - num42; num44 <= num40 + num42; num44++)
                {
                    for (int num45 = num41 - num42; num45 <= num41 + num42; num45++)
                    {
                        float arg_C74_0 = (float)Math.Abs(num44 - num40);
                        float num46 = (float)(Math.Abs(num45 - num41) * 2);
                        if (Math.Sqrt((double)(arg_C74_0 * arg_C74_0 + num46 * num46)) < (double)(num42 + WorldGen.genRand.Next(-1, 2)))
                        {
                            Main.tile[num44, num45].active(true);
                            Main.tile[num44, num45].type = (ushort)num43;
                            WorldGen.SquareTileFrame(num44, num45, true);
                        }
                    }
                }
                for (int num47 = num40 - num42 + 2; num47 <= num40 + num42 - 2; num47++)
                {
                    int num48 = num41 - num42;
                    while (!Main.tile[num47, num48].active())
                    {
                        num48++;
                    }
                    Main.tile[num47, num48].active(false);
                    Main.tile[num47, num48].liquid = 255;
                    WorldGen.SquareTileFrame(num47, num48, true);
                }
            }
        }
        #endregion

        #region EvilIslandHouse
        public static void EvilIslandHouse(int i, int j)
        {
            ushort type = (ushort)(WorldGen.crimson ? 152 : 347); //tile
            byte wall = (byte)(WorldGen.crimson ? 35 : 174); //wall
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
                if (Main.tile[(int)vector.X, k].active())
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
                        Main.tile[l, m].active(true);
                        Main.tile[l, m].liquid = 0;
                        Main.tile[l, m].type = type;
                        Main.tile[l, m].wall = 0;
                        Main.tile[l, m].halfBrick(false);
                        Main.tile[l, m].slope(0);
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
                    if ((num8 != num6 || (n != num4 && n != num5)) && Main.tile[n, num8].wall == 0)
                    {
                        Main.tile[n, num8].active(false);
                        Main.tile[n, num8].wall = wall;
                    }
                }
            }
            int num9 = i + (num2 + 1) * num;
            int num10 = (int)vector.Y;
            for (int num11 = num9 - 2; num11 <= num9 + 2; num11++)
            {
                Main.tile[num11, num10].active(false);
                Main.tile[num11, num10 - 1].active(false);
                Main.tile[num11, num10 - 2].active(false);
            }
            WorldGen.PlaceTile(num9, num10, 10, true, false, -1, WorldGen.crimson ? 1 : 10); //door
            num9 = i + (num2 + 1) * -num - num;
            for (int num12 = num6; num12 <= num7 + 1; num12++)
            {
                Main.tile[num9, num12].active(true);
                Main.tile[num9, num12].liquid = 0;
                Main.tile[num9, num12].type = type;
                Main.tile[num9, num12].wall = 0;
                Main.tile[num9, num12].halfBrick(false);
                Main.tile[num9, num12].slope(0);
            }
            int contain;
            if (WorldGen.crimson)
            {
                contain = 1571; //scourge
            }
            else
            {
                contain = 1569; //vampire
            }
            WorldGen.AddBuriedChest(i, num10 - 3, contain, false, WorldGen.crimson ? 19 : 20); //chest
            int num14 = i - num2 / 2 + 1;
            int num15 = i + num2 / 2 - 1;
            int num16 = 1;
            if (num2 > 10)
            {
                num16 = 2;
            }
            int num17 = (num6 + num7) / 2 - 1;
            for (int num18 = num14 - num16; num18 <= num14 + num16; num18++)
            {
                for (int num19 = num17 - 1; num19 <= num17 + 1; num19++)
                {
                    Main.tile[num18, num19].wall = 21; //glass
                }
            }
            for (int num20 = num15 - num16; num20 <= num15 + num16; num20++)
            {
                for (int num21 = num17 - 1; num21 <= num17 + 1; num21++)
                {
                    Main.tile[num20, num21].wall = 21; //glass
                }
            }
            int num22 = i + (num2 / 2 + 1) * -num;
            WorldGen.PlaceTile(num22, num7 - 1, 14, true, false, -1, WorldGen.crimson ? 1 : 8); //table
            WorldGen.PlaceTile(num22 - 2, num7 - 1, 15, true, false, 0, WorldGen.crimson ? 2 : 11); //chair
            Tile tile = Main.tile[num22 - 2, num7 - 1];
            tile.frameX += 18;
            Tile tile2 = Main.tile[num22 - 2, num7 - 2];
            tile2.frameX += 18;
            WorldGen.PlaceTile(num22 + 2, num7 - 1, 15, true, false, 0, WorldGen.crimson ? 2 : 11); //chair
        }
        #endregion

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
                if (num9 < 0)
                {
                    num9 = 0;
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
                                Main.tile[k, l].active(true);
                                Main.tile[k, l].type = (ushort)ModContent.TileType<BrimstoneSlag>();
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
                while (!Main.tile[m, num14].active())
                {
                    num14--;
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
                                Main.tile[n, num17].active(true);
                                Main.tile[n, num17].type = (ushort)num16;
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
                while ((!Main.tile[num23, num24].active() || Main.tile[num23, num24].type != 0) && num23 < num4)
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
                                    Main.tile[num27, num28].type = (ushort)num26;
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
                while (!Main.tile[num34, num35 + 1].active())
                {
                    num35++;
                }
                if (num35 < num6 && Main.tile[num34, num35 + 1].type == (ushort)ModContent.TileType<BrimstoneSlag>())
                {
                    if (WorldGen.genRand.Next(10) == 0)
                    {
                        int num36 = WorldGen.genRand.Next(1, 3);
                        for (int num37 = num34 - num36; num37 <= num34 + num36; num37++)
                        {
                            if (Main.tile[num37, num35].type == (ushort)ModContent.TileType<BrimstoneSlag>())
                            {
                                Main.tile[num37, num35].active(false);
                                Main.tile[num37, num35].liquid = 255;
                                Main.tile[num37, num35].lava(false);
                                WorldGen.SquareTileFrame(num34, num35, true);
                            }
                            if (Main.tile[num37, num35 + 1].type == (ushort)ModContent.TileType<BrimstoneSlag>())
                            {
                                Main.tile[num37, num35 + 1].active(false);
                                Main.tile[num37, num35 + 1].liquid = 255;
                                Main.tile[num37, num35 + 1].lava(false);
                                WorldGen.SquareTileFrame(num34, num35 + 1, true);
                            }
                            if (num37 > num34 - num36 && num37 < num34 + 2 && Main.tile[num37, num35 + 2].type == (ushort)ModContent.TileType<BrimstoneSlag>())
                            {
                                Main.tile[num37, num35 + 2].active(false);
                                Main.tile[num37, num35 + 2].liquid = 255;
                                Main.tile[num37, num35 + 2].lava(false);
                                WorldGen.SquareTileFrame(num34, num35 + 2, true);
                            }
                        }
                    }
                    if (WorldGen.genRand.Next(5) == 0)
                    {
                        Main.tile[num34, num35].liquid = 255;
                    }
                    Main.tile[num34, num35].lava(false);
                    WorldGen.SquareTileFrame(num34, num35, true);
                }
            }
        }
        #endregion

        #region UnderworldIslandHouse
        public static void UnderworldIslandHouse(int i, int j, int item)
        {
            ushort type = (ushort)ModContent.TileType<BrimstoneSlag>(); //tile
            byte wall = (byte)ModContent.WallType<BrimstoneSlagWallUnsafe>(); //wall
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
                if (Main.tile[(int)vector.X, k].active())
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
                        Main.tile[l, m].active(true);
                        Main.tile[l, m].liquid = 0;
                        Main.tile[l, m].type = type;
                        Main.tile[l, m].wall = 0;
                        Main.tile[l, m].halfBrick(false);
                        Main.tile[l, m].slope(0);
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
                    if ((num8 != num6 || (n != num4 && n != num5)) && Main.tile[n, num8].wall == 0)
                    {
                        Main.tile[n, num8].active(false);
                        Main.tile[n, num8].wall = wall;
                    }
                }
            }
            int num9 = i + (num2 + 1) * num;
            int num10 = (int)vector.Y;
            for (int num11 = num9 - 2; num11 <= num9 + 2; num11++)
            {
                Main.tile[num11, num10].active(false);
                Main.tile[num11, num10 - 1].active(false);
                Main.tile[num11, num10 - 2].active(false);
            }
            WorldGen.PlaceTile(num9, num10, ModContent.TileType<AncientDoorClosed>(), true, false, -1); //door
            num9 = i + (num2 + 1) * -num - num;
            for (int num12 = num6; num12 <= num7 + 1; num12++)
            {
                Main.tile[num9, num12].active(true);
                Main.tile[num9, num12].liquid = 0;
                Main.tile[num9, num12].type = type;
                Main.tile[num9, num12].wall = 0;
                Main.tile[num9, num12].halfBrick(false);
                Main.tile[num9, num12].slope(0);
            }
            WorldGen.AddBuriedChest(i, num10 - 3, item, false, 4); //chest
            int num22 = i + (num2 / 2 + 1) * -num;
            WorldGen.PlaceTile(num22, num7 - 1, ModContent.TileType<AncientTable>(), true, false, -1); //table
            WorldGen.PlaceTile(num22 - 2, num7 - 1, ModContent.TileType<AncientChair>(), true, false, 0); //chair
            Tile tile = Main.tile[num22 - 2, num7 - 1];
            tile.frameX += 18;
            Tile tile2 = Main.tile[num22 - 2, num7 - 2];
            tile2.frameX += 18;
            WorldGen.PlaceTile(num22 + 2, num7 - 1, ModContent.TileType<AncientChair>(), true, false, 0); //chair
        }
        #endregion

        #region AbyssIsland
        public static void AbyssIsland(int i, int j, int sizeMin, int sizeMax, int sizeMin2, int sizeMax2, bool hasChest, bool hasTenebris, bool isVoid)
        {
            int sizeMinSmall = sizeMin / 5;
            int sizeMaxSmall = sizeMax / 5;
            double num = (double)WorldGen.genRand.Next(sizeMin, sizeMax); //100 150
            float num2 = (float)WorldGen.genRand.Next(sizeMinSmall, sizeMaxSmall); //20 30
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
                if (num9 < 0)
                {
                    num9 = 0;
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
                                Main.tile[k, l].active(true);
                                Main.tile[k, l].type = (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>());
                                CalamityUtils.SafeSquareTileFrame(k, l, true);
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
                while (!Main.tile[m, num14].active())
                {
                    num14--;
                }
                num14 += WorldGen.genRand.Next(-3, 4);
                num15 = WorldGen.genRand.Next(4, 8);
                int num16 = isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>();
                if (WorldGen.genRand.Next(4) == 0)
                {
                    num16 = hasChest ? ModContent.TileType<ChaoticOre>() : ModContent.TileType<PlantyMush>();
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
                                Main.tile[n, num17].active(true);
                                Main.tile[n, num17].type = (ushort)num16;
                                CalamityUtils.SafeSquareTileFrame(n, num17, true);
                            }
                        }
                    }
                }
            }
            if (hasTenebris)
            {
                int p = num3;
                int num150;
                for (p += WorldGen.genRand.Next(5); p < num4; p += WorldGen.genRand.Next(num150, (int)((double)num150 * 1.5)))
                {
                    int num14 = num6;
                    while (!Main.tile[p, num14].active())
                    {
                        num14--;
                    }
                    num14 += WorldGen.genRand.Next(-3, 4); //-3 4
                    num150 = 1; //4 8
                    int num16 = ModContent.TileType<Tenebris>();
                    for (int n = p - num150; n <= p + num150; n++)
                    {
                        for (int num17 = num14 - num150; num17 <= num14 + num150; num17++)
                        {
                            if (num17 > num5)
                            {
                                float arg_409_0 = (float)Math.Abs(n - p);
                                float num18 = (float)(Math.Abs(num17 - num14) * 2);
                                if (Math.Sqrt((double)(arg_409_0 * arg_409_0 + num18 * num18)) < (double)(num150 + WorldGen.genRand.Next(2)))
                                {
                                    Main.tile[n, num17].active(true);
                                    Main.tile[n, num17].type = (ushort)num16;
                                    CalamityUtils.SafeSquareTileFrame(n, num17, true);
                                }
                            }
                        }
                    }
                }
            }
            int sizeMinSmall2 = sizeMin2 / 8;
            int sizeMaxSmall2 = sizeMax2 / 8;
            num = (double)WorldGen.genRand.Next(sizeMin2, sizeMax2);
            num2 = (float)WorldGen.genRand.Next(sizeMinSmall2, sizeMaxSmall2);
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
                while ((!Main.tile[num23, num24].active() || Main.tile[num23, num24].type != 0) && num23 < num4)
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
                    int num26 = isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>();
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
                                    Main.tile[num27, num28].type = (ushort)num26;
                                    CalamityUtils.SafeSquareTileFrame(num27, num28, true);
                                }
                            }
                        }
                    }
                    num23 += WorldGen.genRand.Next(num25, (int)((double)num25 * 1.5));
                }
            }
            for (int num30 = num3 - 20; num30 <= num4 + 20; num30++)
            {
                for (int num31 = num5 - 20; num31 <= num6 + 20; num31++)
                {
                    bool flag = true;
                    for (int num32 = num30 - 1; num32 <= num30 + 1; num32++)
                    {
                        for (int num33 = num31 - 1; num33 <= num31 + 1; num33++)
                        {
                            if (!Main.tile[num32, num33].active())
                            {
                                flag = false;
                            }
                        }
                    }
                    if (flag)
                    {
                        Main.tile[num30, num31].wall = (ushort)(isVoid ? ModContent.WallType<VoidstoneWallUnsafe>() : ModContent.WallType<AbyssGravelWall>());
                        WorldGen.SquareWallFrame(num30, num31, true);
                    }
                }
            }
            for (int num34 = num3; num34 <= num4; num34++)
            {
                int num35 = num5 - 10;
                while (!Main.tile[num34, num35 + 1].active())
                {
                    num35++;
                }
                if (num35 < num6 && Main.tile[num34, num35 + 1].type == (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>()))
                {
                    if (WorldGen.genRand.Next(10) == 0)
                    {
                        int num36 = WorldGen.genRand.Next(1, 3);
                        for (int num37 = num34 - num36; num37 <= num34 + num36; num37++)
                        {
                            if (Main.tile[num37, num35].type == (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>()))
                            {
                                Main.tile[num37, num35].active(false);
                                Main.tile[num37, num35].liquid = 255;
                                Main.tile[num37, num35].lava(false);
                                CalamityUtils.SafeSquareTileFrame(num34, num35, true);
                            }
                            if (Main.tile[num37, num35 + 1].type == (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>()))
                            {
                                Main.tile[num37, num35 + 1].active(false);
                                Main.tile[num37, num35 + 1].liquid = 255;
                                Main.tile[num37, num35 + 1].lava(false);
                                CalamityUtils.SafeSquareTileFrame(num34, num35 + 1, true);
                            }
                            if (num37 > num34 - num36 && num37 < num34 + 2 && Main.tile[num37, num35 + 2].type == (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>()))
                            {
                                Main.tile[num37, num35 + 2].active(false);
                                Main.tile[num37, num35 + 2].liquid = 255;
                                Main.tile[num37, num35 + 2].lava(false);
                                CalamityUtils.SafeSquareTileFrame(num34, num35 + 2, true);
                            }
                        }
                    }
                    if (WorldGen.genRand.Next(5) == 0)
                    {
                        Main.tile[num34, num35].liquid = 255;
                    }
                    Main.tile[num34, num35].lava(false);
                    CalamityUtils.SafeSquareTileFrame(num34, num35, true);
                }
            }
        }
        #endregion

        #region AbyssIslandHouse
        public static void AbyssIslandHouse(int i, int j, int itemChoice, bool isVoid)
        {
            ushort type = (ushort)(isVoid ? ModContent.TileType<Voidstone>() : ModContent.TileType<AbyssGravel>()); //tile
            ushort wall = (ushort)(isVoid ? ModContent.WallType<VoidstoneWallUnsafe>() : ModContent.WallType<AbyssGravelWall>()); //wall
            Vector2 vector = new Vector2((float)i, (float)j);
            int num = 1;
            if (WorldGen.genRand.Next(2) == 0)
            {
                num = -1;
            }
            int num2 = WorldGen.genRand.Next(5, 9);
            int num3 = 3;
            vector.X = (float)(i + (num2 + 2) * num);
            for (int k = j - 15; k < j + 30; k++)
            {
                if (Main.tile[(int)vector.X, k].active())
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
                        Main.tile[l, m].active(true);
                        Main.tile[l, m].type = type;
                        Main.tile[l, m].wall = wall;
                        Main.tile[l, m].halfBrick(false);
                        Main.tile[l, m].slope(0);
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
                    if ((num8 != num6 || (n != num4 && n != num5)) && Main.tile[n, num8].wall == wall)
                    {
                        Main.tile[n, num8].active(false);
                    }
                }
            }
            int num9 = i + (num2 + 1) * num;
            int num10 = (int)vector.Y;
            for (int num11 = num9 - 2; num11 <= num9 + 2; num11++)
            {
                Main.tile[num11, num10].active(false);
                Main.tile[num11, num10 - 1].active(false);
                Main.tile[num11, num10 - 2].active(false);
            }
            switch (itemChoice)
            {
                case 0:
                    itemChoice = ModContent.ItemType<TorrentialTear>();
                    break; //rain item
                case 1:
                    itemChoice = ModContent.ItemType<IronBoots>();
                    break; //movement acc
                case 2:
                    itemChoice = ModContent.ItemType<DepthCharm>();
                    break; //regen acc
                case 3:
                    itemChoice = ModContent.ItemType<Archerfish>();
                    break; //ranged
                case 4:
                    itemChoice = ModContent.ItemType<AnechoicPlating>();
                    break; //defense acc
                case 5:
                    itemChoice = ModContent.ItemType<BallOFugu>();
                    break; //melee
                case 6:
                    itemChoice = ModContent.ItemType<StrangeOrb>();
                    break; //light pet
                case 7:
                    itemChoice = ModContent.ItemType<HerringStaff>();
                    break; //summon
                case 8:
                    itemChoice = ModContent.ItemType<BlackAnurian>();
                    break; //magic
                case 9:
                    itemChoice = ModContent.ItemType<Lionfish>();
                    break; //throwing
                default:
                    itemChoice = 497;
                    break;
            }
            WorldGen.AddBuriedChest(i, num10 - 3, itemChoice, false, 4); //chest
        }
        #endregion

        #region SpecialHut
        // Special Hut: Takes arguments of tile type 1, tile type 2, wall type, hut type (useful if you use this method to generate different huts), and location of the shrine (x and y)
        public static void SpecialHut(ushort tile, ushort tile2, ushort wall, int hutType, int shrineLocationX, int shrineLocationY)
        {
            // Random variables for shrine size
            int randomX = WorldGen.genRand.Next(2, 4);
            int randomY = WorldGen.genRand.Next(2, 4);

            // Replace tiles in shrine area with shrine tile type 1
            for (int n = shrineLocationX - randomX - 1; n <= shrineLocationX + randomX + 1; n++)
            {
                for (int num8 = shrineLocationY - randomY - 1; num8 <= shrineLocationY + randomY + 1; num8++)
                {
                    Main.tile[n, num8].active(true);
                    Main.tile[n, num8].type = tile;
                    Main.tile[n, num8].slope(0);
                    Main.tile[n, num8].liquid = 0;
                    Main.tile[n, num8].lava(false);
                }
            }

            // Replace walls in shrine area with shrine wall type
            for (int num9 = shrineLocationX - randomX; num9 <= shrineLocationX + randomX; num9++)
            {
                for (int num10 = shrineLocationY - randomY; num10 <= shrineLocationY + randomY; num10++)
                {
                    Main.tile[num9, num10].active(false);
                    Main.tile[num9, num10].wall = wall;
                }
            }

            // Remove tiles from the inner part of the shrine area
            for (int num14 = shrineLocationX - randomX - 1; num14 <= shrineLocationX + randomX + 1; num14++)
            {
                for (int num15 = shrineLocationY + randomY - 2; num15 <= shrineLocationY + randomY; num15++)
                    Main.tile[num14, num15].active(false);
            }
            for (int num16 = shrineLocationX - randomX - 1; num16 <= shrineLocationX + randomX + 1; num16++)
            {
                for (int num17 = shrineLocationY + randomY - 2; num17 <= shrineLocationY + randomY - 1; num17++)
                    Main.tile[num16, num17].active(false);
            }

            // Replace tiles from bottom of shrine area with shrine tile type 2
            for (int num18 = shrineLocationX - randomX - 1; num18 <= shrineLocationX + randomX + 1; num18++)
            {
                int num19 = 4;
                int num20 = shrineLocationY + randomY + 2;
                while (!Main.tile[num18, num20].active() && num20 < Main.maxTilesY && num19 > 0)
                {
                    Main.tile[num18, num20].active(true);
                    Main.tile[num18, num20].type = tile2;
                    Main.tile[num18, num20].slope(0);
                    num20++;
                    num19--;
                }
            }

            // Replace tiles from top of shrine with shrine tile type 1
            randomX -= WorldGen.genRand.Next(1, 3);
            int num21 = shrineLocationY - randomY - 2;
            while (randomX > -1)
            {
                for (int num22 = shrineLocationX - randomX - 1; num22 <= shrineLocationX + randomX + 1; num22++)
                {
                    Main.tile[num22, num21].active(true);
                    Main.tile[num22, num21].type = tile;
                }
                randomX -= WorldGen.genRand.Next(1, 3);
                num21--;
            }

            // Place shrine chest
            CalamityWorld.SChestX[hutType] = shrineLocationX;
            CalamityWorld.SChestY[hutType] = shrineLocationY;
            SpecialChest(hutType);
        }
        #endregion

        #region SpecialChest
        // Special Chest: Used for placing shrine chests, takes argument of item choice which dictates what item will spawn in the first slot of this chest
        public static void SpecialChest(int itemChoice)
        {
            int item = 0;
            int chestType = 0;

            switch (itemChoice) //0 to 9
            {
                case 0:
                    item = ModContent.ItemType<TrinketofChi>();
                    break;
                case 1:
                    item = WorldGen.crimson ? ModContent.ItemType<CrimsonEffigy>() : ModContent.ItemType<CorruptionEffigy>();
                    chestType = WorldGen.crimson ? 43 : 3;
                    break;
                case 2:
                    item = ModContent.ItemType<OnyxExcavatorKey>();
                    chestType = 44;
                    break;
                case 3:
                    item = ModContent.ItemType<TundraLeash>();
                    chestType = 47;
                    break;
                case 4:
                    item = ModContent.ItemType<LuxorsGift>();
                    chestType = 30;
                    break;
                case 5:
                    item = ModContent.ItemType<FungalSymbiote>();
                    chestType = 32;
                    break;
                case 6:
                    item = ModContent.ItemType<UnstablePrism>();
                    chestType = 50;
                    break;
                case 7:
                    item = ModContent.ItemType<GladiatorsLocket>();
                    chestType = 51;
                    break;
                case 8:
                    item = ModContent.ItemType<BossRush>();
                    chestType = 4;
                    break;
            }

            // Destroy tiles in chest spawn location
            for (int j = CalamityWorld.SChestX[itemChoice] - 1; j <= CalamityWorld.SChestX[itemChoice] + 1; j++)
            {
                for (int k = CalamityWorld.SChestY[itemChoice]; k <= CalamityWorld.SChestY[itemChoice] + 2; k++)
                    WorldGen.KillTile(j, k, false, false, false);
            }

            // Attempt to fix sloped tiles under the chest to prevent the chest from killing itself (literally)
            for (int l = CalamityWorld.SChestX[itemChoice] - 1; l <= CalamityWorld.SChestX[itemChoice] + 1; l++)
            {
                for (int m = CalamityWorld.SChestY[itemChoice]; m <= CalamityWorld.SChestY[itemChoice] + 3; m++)
                {
                    if (m < Main.maxTilesY)
                    {
                        Main.tile[l, m].slope(0);
                        Main.tile[l, m].halfBrick(false);
                    }
                }
            }

            // Place the chest, finally
            WorldGen.AddBuriedChest(CalamityWorld.SChestX[itemChoice], CalamityWorld.SChestY[itemChoice], item, false, chestType);
        }
        #endregion

        #region ChasmGenerator
        public static void ChasmGenerator(int i, int j, int steps, bool ocean = false)
        {
            float num = steps; //850 small 1450 medium 2050 large
            if (ocean)
            {
                int tileYLookup = j;
                if (CalamityWorld.abyssSide)
                {
                    while (!Main.tile[i + 125, tileYLookup].active())
                    {
                        tileYLookup++;
                    }
                }
                else
                {
                    while (!Main.tile[i - 125, tileYLookup].active())
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
                    if ((double)vector.Y > CalamityWorld.abyssChasmBottom)
                    {
                        num3 -= WorldGen.genRand.Next(5) + 8;
                    }
                }
                if (Main.maxTilesY > 2100)
                {
                    if (((double)vector.Y > CalamityWorld.abyssChasmBottom && num > 0f && ocean) ||
                        (vector.Y >= Main.maxTilesY && num > 0f && !ocean))
                    {
                        num = 0f;
                    }
                }
                else if (Main.maxTilesY > 1500)
                {
                    if (((double)vector.Y > CalamityWorld.abyssChasmBottom && num > 0f && ocean) ||
                        (vector.Y > Main.maxTilesY && num > 0f && !ocean))
                    {
                        num = 0f;
                    }
                }
                else
                {
                    if (((double)vector.Y > CalamityWorld.abyssChasmBottom && num > 0f && ocean) ||
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
                                    Main.tile[k, l].active(false);
                                    Main.tile[k, l].liquid = 255;
                                    Main.tile[k, l].lava(false);
                                }
                                else
                                {
                                    Main.tile[k, l].active(false);
                                    Main.tile[k, l].liquid = 255;
                                    Main.tile[k, l].lava(true);
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
                                Main.tile[m, n].active(false);
                            }
                            if (steps <= num2)
                            {
                                Main.tile[m, n].active(false);
                            }
                            if (ocean)
                            {
                                Main.tile[m, n].liquid = 255;
                                Main.tile[m, n].lava(false);
                            }
                            else
                            {
                                Main.tile[m, n].liquid = 255;
                                Main.tile[m, n].lava(true);
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
                                Main.tile[num11, num12].liquid = 255;
                                Main.tile[num11, num12].lava(false);
                            }
                            else
                            {
                                Main.tile[num11, num12].liquid = 255;
                                Main.tile[num11, num12].lava(true);
                            }
                            if (steps <= num2)
                            {
                                Main.tile[num11, num12].active(false);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region PlaceTits
        public static void PlaceTit(int x, int y, ushort type = 165)
        {
            if (Main.tile[x, y - 1] == null)
            {
                Main.tile[x, y - 1] = new Tile();
            }
            if (Main.tile[x, y] == null)
            {
                Main.tile[x, y] = new Tile();
            }
            if (Main.tile[x, y + 1] == null)
            {
                Main.tile[x, y + 1] = new Tile();
            }
            if (WorldGen.SolidTile(x, y - 1) && !Main.tile[x, y].active() && !Main.tile[x, y + 1].active())
            {
                if (Main.tile[x, y - 1].type == (ushort)ModContent.TileType<Navystone>())
                {
                    if (WorldGen.genRand.Next(2) == 0 || Main.tile[x, y + 2].active())
                    {
                        int num2 = WorldGen.genRand.Next(3) * 18;
                        Main.tile[x, y].type = type;
                        Main.tile[x, y].active(true);
                        Main.tile[x, y].frameX = (short)num2;
                        Main.tile[x, y].frameY = 72;
                    }
                    else
                    {
                        int num3 = WorldGen.genRand.Next(3) * 18;
                        Main.tile[x, y].type = type;
                        Main.tile[x, y].active(true);
                        Main.tile[x, y].frameX = (short)num3;
                        Main.tile[x, y].frameY = 0;
                        Main.tile[x, y + 1].type = type;
                        Main.tile[x, y + 1].active(true);
                        Main.tile[x, y + 1].frameX = (short)num3;
                        Main.tile[x, y + 1].frameY = 18;
                    }
                }
            }
            else
            {
                if (WorldGen.SolidTile(x, y + 1) && !Main.tile[x, y].active() && !Main.tile[x, y - 1].active())
                {
                    if (Main.tile[x, y + 1].type == (ushort)ModContent.TileType<Navystone>())
                    {
                        if (WorldGen.genRand.Next(2) == 0 || Main.tile[x, y - 2].active())
                        {
                            int num13 = WorldGen.genRand.Next(3) * 18;
                            Main.tile[x, y].type = type;
                            Main.tile[x, y].active(true);
                            Main.tile[x, y].frameX = (short)num13;
                            Main.tile[x, y].frameY = 90;
                        }
                        else
                        {
                            int num14 = WorldGen.genRand.Next(3) * 18;
                            Main.tile[x, y - 1].type = type;
                            Main.tile[x, y - 1].active(true);
                            Main.tile[x, y - 1].frameX = (short)num14;
                            Main.tile[x, y - 1].frameY = 36;
                            Main.tile[x, y].type = type;
                            Main.tile[x, y].active(true);
                            Main.tile[x, y].frameX = (short)num14;
                            Main.tile[x, y].frameY = 54;
                        }
                    }
                }
            }
        }
        #endregion

        #region Planetoids
        public static void Planetoids(GenerationProgress progress)
        {
            progress.Message = "Generating Planetoids...";

            int GrassPlanetoidCount = Main.maxTilesX / 1100;
            int LCPlanetoidCount = Main.maxTilesX / 800;
            int MudPlanetoidCount = Main.maxTilesX / 1100;

            const int MainPlanetoidAttempts = 3000;
            int i = 0;
            while (i < MainPlanetoidAttempts)
            {
                if (Biomes<MainPlanet>.Place(new Point(WorldGen.genRand.Next(Main.maxTilesX / 2 - 300, Main.maxTilesX / 2 + 300), WorldGen.genRand.Next(128, 134)), WorldGen.structures))
                {
                    break;
                }
                i++;
            }

            const int CrystalHeartPlanetoidAttempts = 15000;
            i = 0;
            while (LCPlanetoidCount > 0 && i < CrystalHeartPlanetoidAttempts)
            {
                int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.2), (int)(Main.maxTilesX * 0.8));
                int y = WorldGen.genRand.Next(70, 101);

                bool placed = Biomes<HeartPlanet>.Place(x, y, WorldGen.structures);

                if (placed)
                    LCPlanetoidCount--;
                i++;
            }

            const int GrassPlanetoidAttempts = 12000;
            i = 0;
            while (GrassPlanetoidCount > 0 && i < GrassPlanetoidAttempts)
            {
                int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.333), (int)(Main.maxTilesX * 0.666));
                int y = WorldGen.genRand.Next(100, 131);


                bool placed = Biomes<GrassPlanet>.Place(x, y, WorldGen.structures);

                if (placed)
                    GrassPlanetoidCount--;
                i++;
            }

            const int MudPlanetoidAttempts = 12000;
            i = 0;
            while (MudPlanetoidCount > 0 && i < MudPlanetoidAttempts)
            {
                int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.3f), (int)(Main.maxTilesX * 0.7f));
                int y = WorldGen.genRand.Next(100, 131);

                bool placed = Biomes<MudPlanet>.Place(x, y, WorldGen.structures);

                if (placed)
                    MudPlanetoidCount--;
                i++;
            }
        }
        #endregion
    }
}
