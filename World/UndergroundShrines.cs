using CalamityMod.Items.Accessories;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.SummonItems;
using CalamityMod.Tiles.DraedonStructures;
using CalamityMod.Tiles.SunkenSea;
using CalamityMod.Walls;
using CalamityMod.Schematics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

using static CalamityMod.Schematics.SchematicManager;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.Potions;

namespace CalamityMod.World
{
    public class UndergroundShrines
    {
        public static bool ShouldAvoidLocation(Point placementPoint, bool careAboutLiquids = true)
        {
            Tile tile = CalamityUtils.ParanoidTileRetrieval(placementPoint.X, placementPoint.Y);
            if (tile.LiquidAmount > 0 && careAboutLiquids)
                return true;
            if (tile.TileType == TileID.BlueDungeonBrick ||
            tile.TileType == TileID.GreenDungeonBrick ||
            tile.TileType == TileID.PinkDungeonBrick)
            {
                return true;
            }
            if (tile.TileType == TileID.LihzahrdBrick ||
            tile.WallType == WallID.LihzahrdBrickUnsafe)
            {
                return true;
            }
            if (tile.TileType == ModContent.TileType<Navystone>() ||
            tile.TileType == ModContent.TileType<EutrophicSand>() ||
            tile.WallType == ModContent.WallType<NavystoneWall>() ||
            tile.WallType == ModContent.WallType<EutrophicSandWall>())
            {
                return true;
            }

            return false;
        }

        #region Corruption Shrine
        public static void FillCorruptionShrineChest(Chest chest)
        {
            int potionType = Utils.SelectRandom(WorldGen.genRand, ItemID.BattlePotion, ItemID.HunterPotion, ItemID.TrapsightPotion);
            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(ModContent.ItemType<CorruptionEffigy>(), 1),
                new ChestItem(ItemID.RottenChunk, WorldGen.genRand.Next(24, 28 + 1)),
                new ChestItem(ItemID.CorruptionKey, 1),
                new ChestItem(ItemID.CorruptTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(20, 24 + 1)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(10, 12 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(10, 12 + 1)),
            };

            if (Main.zenithWorld)
            {
                int evil = Utils.SelectRandom(WorldGen.genRand, ModContent.ItemType<StressPills>(), ModContent.ItemType<Laudanum>(), ModContent.ItemType<HeartofDarkness>());
                contents = new List<ChestItem>()
            {
                new ChestItem(ModContent.ItemType<CorruptionEffigy>(), 1),
                new ChestItem(ItemID.RottenChunk, WorldGen.genRand.Next(24, 28 + 1)),
                new ChestItem(ItemID.CorruptionKey, 1),
                new ChestItem(ItemID.CorruptTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(20, 24 + 1)),
                new ChestItem(evil, 1),
                new ChestItem(ItemID.RedPotion, WorldGen.genRand.Next(1, 2 + 1)),
                new ChestItem(ItemID.GasTrap,1),
            };
            }

            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }
        public static void PlaceCorruptionShrine(StructureMap structures)
        {
            int tries = 0;
            string mapKey = CorruptionShrineKey;
            
            do
            {
                int placementPositionX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.05f), (int)(Main.maxTilesX * 0.95f));
                int placementPositionY = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.15f), (int)(Main.maxTilesY * 0.5f));
                Point placementPoint = new Point(placementPositionX, placementPositionY);

                Vector2 schematicSize = new Vector2(TileMaps[mapKey].GetLength(0)/2, TileMaps[mapKey].GetLength(1)); //Fooling the system into thinking the shrine is smaller than it actually is so it fits into chasms
                int corruptStuffInArea = 0;
                bool canGenerateInLocation = true;
                bool inYourWalls = false;

                float totalTiles = schematicSize.X * schematicSize.Y;
                for (int x = placementPoint.X; x < placementPoint.X + schematicSize.X; x++)
                {
                    for (int y = placementPoint.Y; y < placementPoint.Y + schematicSize.Y; y++)
                    {
                        Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);
                        if (ShouldAvoidLocation(new Point(x, y)))
                            canGenerateInLocation = false;

                        //Should generate within the bounds of the walls.
                        if (tile.TileType == TileID.Ebonstone || tile.WallType == WallID.EbonstoneUnsafe)
                            corruptStuffInArea++;

                        if (tile.WallType == WallID.EbonstoneUnsafe)
                            inYourWalls = true;
                        
                        //Do not cut into the altars
                        if (tile.TileType == TileID.DemonAltar)
                            canGenerateInLocation = false;
                    }
                }
                if (!canGenerateInLocation || corruptStuffInArea < totalTiles*0.9f || !structures.CanPlace(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y)) || !inYourWalls)
                {
                    tries++;
                }
                else
                {
                    bool _ = true;
                    PlaceSchematic(mapKey, new Point(placementPoint.X, placementPoint.Y), SchematicAnchor.TopLeft, ref _, new Action<Chest>(FillCorruptionShrineChest));
                    CalamityUtils.AddProtectedStructure(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X * 2, (int)schematicSize.Y), 4);
                    break;
                }
                //FUCK YOU FUCK YOU FUCK YOU
            } while (tries <= 60000);
        }
        #endregion

        #region Crimson Shrine
        public static void FillCrimsonShrineChest(Chest chest)
        {
            int potionType = Utils.SelectRandom(WorldGen.genRand, ItemID.BattlePotion, ItemID.HunterPotion, ItemID.TrapsightPotion);
            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(ModContent.ItemType<CrimsonEffigy>(), 1),
                new ChestItem(ItemID.Vertebrae, WorldGen.genRand.Next(24, 28 + 1)),
                new ChestItem(ItemID.CrimsonKey, 1),
                new ChestItem(ItemID.CrimsonTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(20, 24 + 1)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(10, 12 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(10, 12 + 1)),
            };

            //Gfb loot change
            if (Main.zenithWorld)
            {
                //Cannot modify the return value of List<ChestItem>.this[int] because its not a variable so gotta do this instead
                contents = new List<ChestItem>()
                {
                new ChestItem(ModContent.ItemType<CrimsonEffigy>(), 1),
                new ChestItem(ItemID.Vertebrae, WorldGen.genRand.Next(24, 28 + 1)),
                new ChestItem(ItemID.CrimsonKey, 1),
                new ChestItem(ItemID.CrimsonTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(20, 24 + 1)),
                new ChestItem(ModContent.ItemType<BloodyMary>(), WorldGen.genRand.Next(2, 2 + 1)),
                new ChestItem(ItemID.RedPotion, WorldGen.genRand.Next(1, 2 + 1)),
                new ChestItem(ItemID.GasTrap, 1),
                };
            }

            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }
        public static void PlaceCrimsonShrine(StructureMap structures)
        {
            int tries = 0;
            string mapKey = CrimsonShrineKey;
            
            do
            {
                int placementPositionX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.05f), (int)(Main.maxTilesX * 0.95f));
                int placementPositionY = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.15f), (int)(Main.maxTilesY * 0.5f));
                Point placementPoint = new Point(placementPositionX, placementPositionY);

                Vector2 schematicSize = new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1));
                int crimsonStuffInArea = 0;
                bool canGenerateInLocation = true;
                bool inYourWalls = false;

                float groundThreshold = schematicSize.Y * 0.4f;
                float groundTiles = schematicSize.X * groundThreshold;
                float totalTiles = schematicSize.X * schematicSize.Y;
                for (int x = placementPoint.X; x < placementPoint.X + schematicSize.X; x++)
                {
                    for (int y = placementPoint.Y; y < placementPoint.Y + schematicSize.Y; y++)
                    {
                        Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);
                        if (ShouldAvoidLocation(new Point(x, y)))
                            canGenerateInLocation = false;
                        
                        //Crimson does not generate walls in blocks very much, so both walls and tiles are grouped
                        if (tile.TileType == TileID.Crimstone || tile.WallType == WallID.CrimstoneUnsafe)
                            crimsonStuffInArea++;

                        if (tile.WallType == WallID.CrimstoneUnsafe)
                            inYourWalls = true;
                        
                        //Do not cut into the altars
                        if (tile.TileType == TileID.DemonAltar)
                            canGenerateInLocation = false;
                    }
                }
                if (!canGenerateInLocation || crimsonStuffInArea < totalTiles * 0.4f || !structures.CanPlace(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y)) || !inYourWalls)
                {
                    tries++;
                }
                else
                {
                    bool _ = true;
                    PlaceSchematic(mapKey, new Point(placementPoint.X, placementPoint.Y), SchematicAnchor.TopLeft, ref _, new Action<Chest>(FillCrimsonShrineChest));
                    CalamityUtils.AddProtectedStructure(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y), 4);
                    break;
                }
                //FUCK YOU TOO
            } while (tries <= 60000);
        }
        #endregion
        
        #region Desert Shrine
        public static void FillDesertShrineChest(Chest chest)
        {
            int potionType = Utils.SelectRandom(WorldGen.genRand, ItemID.ShinePotion, ItemID.MiningPotion, ItemID.BuilderPotion);
            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(ModContent.ItemType<LuxorsGift>(), 1),
                new ChestItem(ModContent.ItemType<Items.Placeables.PrismShard>(), WorldGen.genRand.Next(6, 8 + 1)),
                new ChestItem(ItemID.DungeonDesertKey, 1),
                new ChestItem(ItemID.DesertTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(20, 24 + 1)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(10, 12 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(10, 12 + 1)),
            };
            if (Main.zenithWorld)
            {
                int golfClub = Utils.SelectRandom(WorldGen.genRand, ItemID.GolfClubBronzeWedge, ItemID.GolfClubWedge, ItemID.GasTrap);
                contents = new List<ChestItem>()
                {
                new ChestItem(ModContent.ItemType<LuxorsGift>(), 1),
                new ChestItem(ModContent.ItemType<Items.Placeables.PrismShard>(), WorldGen.genRand.Next(6, 8 + 1)),
                new ChestItem(ItemID.DungeonDesertKey, 1),
                new ChestItem(ItemID.DesertTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(20, 24 + 1)),
                new ChestItem(ModContent.ItemType<SpelunkersAmulet>(), 1),
                new ChestItem(ItemID.RedPotion, WorldGen.genRand.Next(1, 2 + 1)),
                new ChestItem(golfClub, 1), //Implying that the golfer messed with the loot but forgot this, OR its trapped
                };
            }

            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }
        public static void PlaceDesertShrine(StructureMap structures)
        {
            int tries = 0;
            string mapKey = DesertShrineKey;
            
            do
            {
                int placementPositionX = WorldGen.genRand.Next(GenVars.UndergroundDesertLocation.Left, GenVars.UndergroundDesertLocation.Right);
                int placementPositionY = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.3f), (int)(Main.maxTilesY * 0.55f));
                Point placementPoint = new Point(placementPositionX, placementPositionY);

                Vector2 schematicSize = new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1));
                int desertTilesInArea = 0;
                int xCheckArea = 50;
                bool canGenerateInLocation = true;

                float totalTiles = (schematicSize.X + xCheckArea * 2) * schematicSize.Y;
                for (int x = placementPoint.X - xCheckArea; x < placementPoint.X + schematicSize.X + xCheckArea; x++)
                {
                    for (int y = placementPoint.Y; y < placementPoint.Y + schematicSize.Y; y++)
                    {
                        Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);
                        if (ShouldAvoidLocation(new Point(x, y)))
                            canGenerateInLocation = false;

                        if (tile.TileType == TileID.DesertFossil || tile.TileType == TileID.Sand || tile.TileType == TileID.HardenedSand || tile.TileType == TileID.Sandstone)
                                desertTilesInArea++;
                    }
                }
                if (!canGenerateInLocation || desertTilesInArea < totalTiles * 0.3f || !structures.CanPlace(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y)))
                {
                    tries++;
                }
                else
                {
                    bool _ = true;
                    PlaceSchematic(mapKey, new Point(placementPoint.X, placementPoint.Y), SchematicAnchor.TopLeft, ref _, new Action<Chest>(FillDesertShrineChest));
                    CalamityUtils.AddProtectedStructure(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y), 4);
                    break;
                }

            } while (tries <= 20000);
        }
        #endregion

        #region Granite Shrine
        public static void FillGraniteShrineChest(Chest chest)
        {
            int potionType = Utils.SelectRandom(WorldGen.genRand, ItemID.EndurancePotion, ItemID.HeartreachPotion, ItemID.LifeforcePotion);
            if (Main.zenithWorld)
                potionType = ItemID.RedPotion;

            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(ModContent.ItemType<UnstableGraniteCore>(), 1),
                new ChestItem(ItemID.Geode, WorldGen.genRand.Next(6, 8 + 1)),
                new ChestItem(ItemID.BlueTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(20, 24 + 1)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(10, 12 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(Main.zenithWorld ? 1 : 10, (Main.zenithWorld ? 2 : 12) + 1)),
                new ChestItem((Main.rand.NextBool() && Main.zenithWorld) ? ItemID.GasTrap : ItemID.Granite, Main.zenithWorld ? 1 : WorldGen.genRand.Next(7,15+1)),
            };

            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }
        public static void PlaceGraniteShrine(StructureMap structures)
        {
            int tries = 0;
            string mapKey = GraniteShrineKey;
            
            do
            {
                int placementPositionX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f));
                int placementPositionY = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.2f), (int)(Main.maxTilesY * 0.85f));
                Point placementPoint = new Point(placementPositionX, placementPositionY);

                Vector2 schematicSize = new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1));
                int graniteWallsInArea = 0;
                bool canGenerateInLocation = true;

                float totalTiles = schematicSize.X * schematicSize.Y;
                for (int x = placementPoint.X; x < placementPoint.X + schematicSize.X; x++)
                {
                    for (int y = placementPoint.Y; y < placementPoint.Y + schematicSize.Y; y++)
                    {
                        Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);
                        if (ShouldAvoidLocation(new Point(x, y)))
                            canGenerateInLocation = false;

                        //The granite geode is supposed to fully float in free air. No tile replacements
                        if (tile.WallType == WallID.GraniteUnsafe && !tile.HasTile && !Main.remixWorld)
                            graniteWallsInArea++;
                        else if ((tile.WallType == WallID.MarbleUnsafe || tile.TileType == TileID.Marble) && Main.remixWorld) //Get fixed boi makes it generate in marble biomes
                            graniteWallsInArea++;
                    }
                }
                if (!canGenerateInLocation || graniteWallsInArea < totalTiles * 0.95f || !structures.CanPlace(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y)))
                {
                    tries++;
                }
                else
                {
                    bool _ = true;
                    PlaceSchematic(mapKey, new Point(placementPoint.X, placementPoint.Y), SchematicAnchor.TopLeft, ref _, new Action<Chest>(FillGraniteShrineChest));
                    CalamityUtils.AddProtectedStructure(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y), 4);
                    break;
                }

            } while (tries <= 30000);
        }
        #endregion

        #region Ice Shrine
        public static void FillIceShrineChest(Chest chest)
        {
            int foodType = Utils.SelectRandom(WorldGen.genRand, ItemID.ChristmasPudding, ItemID.SugarCookie, ItemID.GingerbreadCookie);
            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(ModContent.ItemType<TundraLeash>(), 1),
                new ChestItem(ItemID.FlinxFur, WorldGen.genRand.Next(6, 8 + 1)),
                new ChestItem(ItemID.FrozenKey, 1),
                new ChestItem(ItemID.IceTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(20, 24 + 1)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(10, 12 + 1)),
                new ChestItem(foodType, WorldGen.genRand.Next(10, 12 + 1)),
            };

            if (Main.zenithWorld)
            {
                contents = new List<ChestItem>()
            {
                new ChestItem(ModContent.ItemType<TundraLeash>(), 1),
                new ChestItem(ItemID.FlinxFur, WorldGen.genRand.Next(6, 8 + 1)),
                new ChestItem(ItemID.FrozenKey, 1),
                new ChestItem(ItemID.IceTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(20, 24 + 1)),
                new ChestItem(ItemID.Eggnog, WorldGen.genRand.Next(10, 12 + 1)),
                new ChestItem(ModContent.ItemType<DeliciousMeat>(), WorldGen.genRand.Next(200, 349 + 1)),
                new ChestItem(Main.rand.NextBool() ? ItemID.GasTrap : ItemID.Marshmallow, 1)
            };
            }

            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }
        public static void PlaceIceShrine(StructureMap structures)
        {
            int tries = 0;
            string mapKey = IceShrineKey;
            
            do
            {
                int placementPositionX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.25f), (int)(Main.maxTilesX * 0.75f));
                int placementPositionY = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.35f), (int)(Main.maxTilesY * 0.7f));
                Point placementPoint = new Point(placementPositionX, placementPositionY);

                Vector2 schematicSize = new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1));
                int iceTilesInArea = 0;
                int xCheckArea = 80;
                int yCheckArea = 20;
                bool canGenerateInLocation = true;

                float totalTiles = (schematicSize.X + xCheckArea * 2) * (schematicSize.Y + yCheckArea * 2);
                for (int x = placementPoint.X - xCheckArea; x < placementPoint.X + schematicSize.X + xCheckArea; x++)
                {
                    for (int y = placementPoint.Y - yCheckArea; y < placementPoint.Y + schematicSize.Y + yCheckArea; y++)
                    {
                        Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);
                        //Ice biomes obviously have a lot of water
                        if (ShouldAvoidLocation(new Point(x, y), false))
                            canGenerateInLocation = false;

                        if (tile.TileType == TileID.SnowBlock || tile.TileType == TileID.IceBlock)
                                iceTilesInArea++;
                    }
                }
                if (!canGenerateInLocation || iceTilesInArea < totalTiles * 0.35f || !structures.CanPlace(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y)))
                {
                    tries++;
                }
                else
                {
                    bool _ = true;
                    PlaceSchematic(mapKey, new Point(placementPoint.X, placementPoint.Y), SchematicAnchor.TopLeft, ref _, new Action<Chest>(FillIceShrineChest));
                    CalamityUtils.AddProtectedStructure(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y), 4);
                    break;
                }

            } while (tries <= 20000);
        }
        #endregion

        #region Marble Shrine
        public static void FillMarbleShrineChest(Chest chest)
        {
            int potionType = Utils.SelectRandom(WorldGen.genRand, ItemID.EndurancePotion, ItemID.HeartreachPotion, ItemID.LifeforcePotion);
            if (Main.zenithWorld)
                potionType = ItemID.RedPotion;
            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(ModContent.ItemType<GladiatorsLocket>(), 1),
                new ChestItem(GenVars.goldBar == TileID.Gold ? ItemID.GoldBar : ItemID.PlatinumBar, WorldGen.genRand.Next(12, 15 + 1)),
                new ChestItem(ItemID.WhiteTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(20, 24 + 1)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(10, 12 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(Main.zenithWorld ? 1 : 10, (Main.zenithWorld ? 2 : 12) + 1)),
                new ChestItem((Main.rand.NextBool() && Main.zenithWorld) ? ItemID.GasTrap : ItemID.Marble, Main.zenithWorld ? 1 : WorldGen.genRand.Next(7,15+1)),
            };

            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }
        public static void PlaceMarbleShrine(StructureMap structures)
        {
            int tries = 0;
            string mapKey = MarbleShrineKey;
            
            do
            {
                int placementPositionX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f));
                int placementPositionY = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.2f), (int)(Main.maxTilesY * 0.85f));
                Point placementPoint = new Point(placementPositionX, placementPositionY);

                Vector2 schematicSize = new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1));
                int marbleStuffInArea = 0;
                int airTilesBetweenPillar = 0;
                bool canGenerateInLocation = true;

                float totalTiles = schematicSize.X * schematicSize.Y;
                for (int x = placementPoint.X; x < placementPoint.X + schematicSize.X; x++)
                {
                    for (int y = placementPoint.Y; y < placementPoint.Y + schematicSize.Y; y++)
                    {
                        Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);
                        if (ShouldAvoidLocation(new Point(x, y)))
                            canGenerateInLocation = false;

                        //Marble biomes either have blocks or walls, occasionally both
                        //This should be near maximum to prevent the structure from overextending
                        if ((tile.TileType == TileID.Marble || tile.WallType == WallID.MarbleUnsafe) && !Main.zenithWorld)
                            marbleStuffInArea++;
                        else if ((tile.WallType == WallID.GraniteUnsafe || tile.TileType == TileID.Granite) && Main.zenithWorld) //Generates in granite in gfb
                            marbleStuffInArea++;
                        
                        //There should be some space between the pillars so it doesn't make pillars in the middle of nowhere zone
                        float pillarFoundationBound = schematicSize.Y * 0.2f;
                        bool pillarSpace = y <= placementPoint.Y + schematicSize.Y - pillarFoundationBound && y >= placementPoint.Y + pillarFoundationBound;
                        if (pillarSpace && !tile.HasTile)
                            airTilesBetweenPillar++;
                    }
                }

                if (!canGenerateInLocation || marbleStuffInArea < totalTiles * 0.9f || airTilesBetweenPillar < totalTiles * 0.3f || !structures.CanPlace(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y)))
                {
                    tries++;
                }
                else
                {
                    bool _ = true;
                    PlaceSchematic(mapKey, new Point(placementPoint.X, placementPoint.Y), SchematicAnchor.TopLeft, ref _, new Action<Chest>(FillMarbleShrineChest));
                    CalamityUtils.AddProtectedStructure(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y), 4);
                    break;
                }

            } while (tries <= 30000);
        }
        #endregion

        #region Mushroom Shrine
        public static void FillMushroomShrineChest(Chest chest)
        {
            int potionType = Utils.SelectRandom(WorldGen.genRand, ItemID.ShinePotion, ItemID.MiningPotion, ItemID.BuilderPotion);
            List<ChestItem> contents = new List<ChestItem>()
                {
                new ChestItem(ModContent.ItemType<FungalSymbiote>(), 1),
                new ChestItem(ItemID.TruffleWorm, 3),
                new ChestItem(ItemID.MushroomTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(20, 24 + 1)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(10, 12 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(10, 12 + 1)),
                };

            // Gfb loot change
            if (Main.zenithWorld)
            {
                //"Cannot modify the return value of List<ChestItem>.this[int] because its not a variable" so gotta do this instead, I could add a bunch of bools but I feel this is better for how much is changed
                contents = new List<ChestItem>()
                {
                new ChestItem(ModContent.ItemType<FungalSymbiote>(), 1),
                new ChestItem(ItemID.TruffleWorm, 3),
                new ChestItem(ItemID.MushroomTorch, WorldGen.genRand.Next(50, 60 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(4, 6 + 1)),
                new ChestItem(ModContent.ItemType<OddMushroom>(), WorldGen.genRand.Next(2, 3 + 1)),
                new ChestItem(ItemID.RedPotion, WorldGen.genRand.Next(1, 2 + 1)),
                new ChestItem(ItemID.GasTrap, 1)
                };
            }


            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }
        public static void PlaceMushroomShrine(StructureMap structures)
        {
            int tries = 0;
            string mapKey = MushroomShrineKey;

            do
            {
                int placementPositionX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.2f), (int)(Main.maxTilesX * 0.8f));
                int placementPositionY = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.2f), (int)(Main.maxTilesY * 0.85f));

                // Gfb and remix
                if (Main.remixWorld)
                {
                    // Ensure that the shrine doesn't generate too close to the center of the world
                    do
                    {
                        placementPositionX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.2f), (int)(Main.maxTilesX * 0.8f));
                    }
                    while (placementPositionX > (int)(Main.maxTilesX * 0.4f) && placementPositionX < (int)(Main.maxTilesX * 0.6f));
                    placementPositionY = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.85f), (int)(Main.maxTilesY * 0.9f)); //Mushroom layer
                }

                Point placementPoint = new Point(placementPositionX, placementPositionY);

                Vector2 schematicSize = new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1));
                int realMushroomsInArea = 0;
                int extraArea = 20;
                int yExtraArea = 40;
                bool canGenerateInLocation = true;

                int requiredShrooms = 20; //for now lower this, will look through the gen later
                for (int x = placementPoint.X - extraArea; x < placementPoint.X + schematicSize.X + extraArea; x++)
                {
                    for (int y = placementPoint.Y; y < placementPoint.Y + schematicSize.Y + yExtraArea; y++)
                    {
                        Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);

                        //For some reason, mushroom biomes are very wet
                        //It gets way too difficult to generate if it doesn't ignore water

                        if (ShouldAvoidLocation(new Point(x, y), false))
                            canGenerateInLocation = false;

                        //Only generated within the area of mushroom plants
                        if (tile.TileType == TileID.MushroomPlants || tile.TileType == TileID.MushroomVines || tile.TileType == TileID.MushroomTrees || tile.TileType == TileID.MushroomGrass)
                            realMushroomsInArea++;

                    }
                }
                if ((!canGenerateInLocation || realMushroomsInArea < requiredShrooms || !structures.CanPlace(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y))) && !Main.remixWorld)
                {
                    tries++;
                }
                else if (!canGenerateInLocation && Main.remixWorld) //GFB and remix will not give a shit about mushrooms or the rectangle
                {
                    tries++;
                }
                else
                {
                    bool _ = true;
                    PlaceSchematic(mapKey, new Point(placementPoint.X, placementPoint.Y), SchematicAnchor.TopLeft, ref _, new Action<Chest>(FillMushroomShrineChest));
                    CalamityUtils.AddProtectedStructure(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y), 4);
                    break;
                }
            } while (tries <= 20000);
        }
        #endregion

        #region Surface Shrine
        public static void FillSurfaceShrineChest(Chest chest)
        {
            int potionType = Utils.SelectRandom(WorldGen.genRand, ItemID.RecallPotion, ItemID.CalmingPotion, ItemID.SwiftnessPotion);
            if (Main.zenithWorld)
                potionType = ItemID.Sake;
            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(ModContent.ItemType<TrinketofChi>(), 1),
                new ChestItem(ItemID.PinkGel, WorldGen.genRand.Next(12, 15 + 1)),
                new ChestItem(ItemID.Torch, WorldGen.genRand.Next(50, 60 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(4, 6 + 1)),
                new ChestItem(Main.zenithWorld ? ItemID.RestorationPotion : ItemID.LesserHealingPotion, WorldGen.genRand.Next(10, 12 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(10, 12 + 1)),
                new ChestItem(Main.zenithWorld ? ItemID.GasTrap : ItemID.Mushroom, Main.zenithWorld ? 1 : WorldGen.genRand.Next(5,9+1)),
            };

            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }
        public static void PlaceSurfaceShrine(StructureMap structures)
        {
            int tries = 0;
            string mapKey = SurfaceShrineKey;
            
            do
            {
                int placementPositionX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.2f), (int)(Main.maxTilesX * 0.8f));

                // Ensure that the shrine doesn't generate too close to the center of the world
                do
                {
                    placementPositionX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.2f), (int)(Main.maxTilesX * 0.8f));
                }
                while (placementPositionX > (int)(Main.maxTilesX * 0.4f) && placementPositionX < (int)(Main.maxTilesX * 0.6f));

                int numTilesBelowSurface = WorldGen.genRand.Next(25, 50);
                
                //use Main.worldSurface and not WorldGen.WorldSurface, i believe that is why it was genning on the surface so much
                int placementPositionY = (int)Main.worldSurface + numTilesBelowSurface;
                
                if (Main.remixWorld)
                    placementPositionY = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.65f), (int)(Main.maxTilesY * 0.7f)); //above mushroom layer
                Point placementPoint = new Point(placementPositionX, placementPositionY);

                Vector2 schematicSize = new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1));
                int normalTilesInArea = 0;
                int activeTilesInArea = 0;
                bool canGenerateInLocation = true;

                for (int x = placementPoint.X; x < placementPoint.X + schematicSize.X; x++)
                {
                    for (int y = placementPoint.Y; y < placementPoint.Y + schematicSize.Y; y++)
                    {
                        Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);
                        // Liquids are fine, the structure is sealed.
                        if (ShouldAvoidLocation(new Point(x, y), false))
                            canGenerateInLocation = false;

                        if (tile.TileType == TileID.Dirt || tile.TileType == TileID.Stone || tile.TileType == TileID.ClayBlock || tile.TileType == TileID.Sand)
                            normalTilesInArea++;
                            
                        if (tile.HasTile)
                            activeTilesInArea++;
                        
                        // Avoid the desert due to sand checks.
                        if (tile.WallType == WallID.HardenedSand || tile.WallType == WallID.Sandstone)
                            canGenerateInLocation = false;
                    }
                }

                if (!canGenerateInLocation || normalTilesInArea < activeTilesInArea * 0.8f || !structures.CanPlace(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y)))
                {
                    tries++;
                }
                else
                {
                    if (!Main.remixWorld) // Make tunnel if its not remix or Gfb
                    {
                        Point result;
                        Point shrineTunnelPlacementPoint = new Point(placementPoint.X + (int)(schematicSize.X * 0.5f), placementPoint.Y);
                        bool flag = WorldUtils.Find(shrineTunnelPlacementPoint, Searches.Chain(new Searches.Up(1000), new Conditions.IsSolid().AreaOr(1, 50).Not()), out result);
                        if (WorldUtils.Find(shrineTunnelPlacementPoint, Searches.Chain(new Searches.Up(shrineTunnelPlacementPoint.Y - result.Y), new Conditions.IsTile(TileID.Sand)), out Point _))
                        {
                            tries++;
                        }
                        else if (!flag)
                        {
                            tries++;
                        }
                        else
                        {
                            result.Y += numTilesBelowSurface;

                            bool[] array = new bool[TileID.Sets.GeneralPlacementTiles.Length];
                            for (int i = 0; i < array.Length; i++)
                                array[i] = TileID.Sets.GeneralPlacementTiles[i];

                            array[TileID.Containers] = false;
                            array[TileID.Containers2] = false;

                            if (!structures.CanPlace(new Rectangle(shrineTunnelPlacementPoint.X, result.Y + 10, 1, shrineTunnelPlacementPoint.Y - result.Y - 9), array, 2))
                            {
                                tries++;
                            }
                            else
                            {
                                bool _ = true;
                                PlaceSchematic(mapKey, new Point(placementPoint.X, placementPoint.Y), SchematicAnchor.TopLeft, ref _, new Action<Chest>(FillSurfaceShrineChest));
                                CalamityUtils.AddProtectedStructure(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y), 4);

                                // Generate entrance tunnel
                                ShapeData data = new ShapeData();
                                WorldUtils.Gen(new Point(shrineTunnelPlacementPoint.X, result.Y + 10), new Shapes.Rectangle(1, shrineTunnelPlacementPoint.Y - result.Y - 9), Actions.Chain(new Modifiers.Blotches(2, 0.2), new Modifiers.SkipTiles(TileID.LivingWood, TileID.LeafBlock), new Actions.ClearTile().Output(data), new Modifiers.Expand(1), new Modifiers.OnlyTiles(TileID.Sand), new Actions.SetTile(TileID.HardenedSand).Output(data)));
                                WorldUtils.Gen(new Point(shrineTunnelPlacementPoint.X, result.Y + 10), new ModShapes.All(data), new Actions.SetFrames(frameNeighbors: true));

                                break;
                            }
                        }
                    } else
                    {
                        bool _ = true;
                        PlaceSchematic(mapKey, new Point(placementPoint.X, placementPoint.Y), SchematicAnchor.TopLeft, ref _, new Action<Chest>(FillSurfaceShrineChest));
                        CalamityUtils.AddProtectedStructure(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y), 4);
                        break;
                    }

                }

            } while (tries <= 30000);
        }
        #endregion

        //Enums and generation methods for old shrines
        //Currently still used for Abyss (Terminus) shrine
        #region Enumeration
        public enum UndergroundShrineType
        {
            Surface,
            Cavern,
            WorldEvil,
            Ice,
            Desert,
            Mushroom,
            Granite,
            Marble,
            Abyss
        }
        #endregion

        #region Hut Outline Creation

        // Special Hut: Takes arguments of tile type 1, tile type 2, wall type, hut type (useful if you use this method to generate different huts), and location of the shrine (x and y)
        public static void SpecialHut(ushort tile, ushort tile2, ushort wall, UndergroundShrineType hutType, int shrineLocationX, int shrineLocationY)
        {
            // Random variables for shrine size
            int randomX = WorldGen.genRand.Next(2, 4);
            int randomY = WorldGen.genRand.Next(2, 4);

            // Replace tiles in shrine area with shrine tile type 1
            for (int x = shrineLocationX - randomX - 1; x <= shrineLocationX + randomX + 1; x++)
            {
                for (int y = shrineLocationY - randomY - 1; y <= shrineLocationY + randomY + 1; y++)
                {
                    Main.tile[x, y].Get<TileWallWireStateData>().HasTile = true;
                    Main.tile[x, y].TileType = tile;
                    Main.tile[x, y].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                    Main.tile[x, y].LiquidAmount = 0;
                    Main.tile[x, y].Get<LiquidData>().LiquidType = LiquidID.Water;
                }
            }

            // Replace walls in shrine area with shrine wall type
            for (int x = shrineLocationX - randomX; x <= shrineLocationX + randomX; x++)
            {
                for (int y = shrineLocationY - randomY; y <= shrineLocationY + randomY; y++)
                {
                    Main.tile[x, y].Get<TileWallWireStateData>().HasTile = false;
                    Main.tile[x, y].WallType = wall;
                }
            }

            // Remove tiles from the inner part of the shrine area
            for (int x = shrineLocationX - randomX - 1; x <= shrineLocationX + randomX + 1; x++)
            {
                for (int y = shrineLocationY + randomY - 2; y <= shrineLocationY + randomY; y++)
                    Main.tile[x, y].Get<TileWallWireStateData>().HasTile = false;
            }
            for (int x = shrineLocationX - randomX - 1; x <= shrineLocationX + randomX + 1; x++)
            {
                for (int y = shrineLocationY + randomY - 2; y <= shrineLocationY + randomY - 1; y++)
                    Main.tile[x, y].Get<TileWallWireStateData>().HasTile = false;
            }

            // Replace tiles from bottom of shrine area with shrine tile type 2
            for (int x = shrineLocationX - randomX - 1; x <= shrineLocationX + randomX + 1; x++)
            {
                int verticalOffset = 4;
                int y = shrineLocationY + randomY + 2;
                while (!Main.tile[x, y].HasTile && y < Main.maxTilesY && verticalOffset > 0)
                {
                    Main.tile[x, y].Get<TileWallWireStateData>().HasTile = true;
                    Main.tile[x, y].TileType = tile2;
                    Main.tile[x, y].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                    y++;
                    verticalOffset--;
                }
            }

            // Replace tiles from top of shrine with shrine tile type 1
            randomX -= WorldGen.genRand.Next(1, 3);
            int num21 = shrineLocationY - randomY - 2;
            while (randomX > -1)
            {
                for (int x = shrineLocationX - randomX - 1; x <= shrineLocationX + randomX + 1; x++)
                {
                    Main.tile[x, num21].Get<TileWallWireStateData>().HasTile = true;
                    Main.tile[x, num21].TileType = tile;
                }
                randomX -= WorldGen.genRand.Next(1, 3);
                num21--;
            }

            // Place shrine chest
            CalamityWorld.SChestX[(int)hutType] = shrineLocationX;
            CalamityWorld.SChestY[(int)hutType] = shrineLocationY;
            SpecialChest(hutType);
        }
        #endregion

        #region Chest Creation
        // Special Chest: Used for placing shrine chests, takes argument of the shrine type which dictates what item will spawn in the first slot of this chest
        public static void SpecialChest(UndergroundShrineType shrineType)
        {
            int item = 0;
            //1.4 chests are dubious and use another ID type, so there's 2 now
            int chestType = 21;
            int chestSubType = 0;

            switch (shrineType)
            {
                case UndergroundShrineType.Surface:
                    item = ModContent.ItemType<TrinketofChi>(); //Default chest
                    break;
                case UndergroundShrineType.Cavern:
                    item = ModContent.ItemType<OnyxExcavatorKey>();
                    chestSubType = 44; //Obsidian
                    break;
                case UndergroundShrineType.WorldEvil:
                    item = WorldGen.crimson ? ModContent.ItemType<CrimsonEffigy>() : ModContent.ItemType<CorruptionEffigy>();
                    chestType = WorldGen.crimson ? 21 : 467; //Flesh and Lesion
                    chestSubType = WorldGen.crimson ? 43 : 3;
                    break;
                case UndergroundShrineType.Ice:
                    item = ModContent.ItemType<TundraLeash>();
                    chestSubType = 47; //Glass
                    break;
                case UndergroundShrineType.Desert:
                    item = ModContent.ItemType<LuxorsGift>();
                    chestType = 467;
                    chestSubType = 10; //Sandstone
                    break;
                case UndergroundShrineType.Mushroom:
                    item = ModContent.ItemType<FungalSymbiote>();
                    chestSubType = 32; //Mushroom
                    break;
                case UndergroundShrineType.Granite:
                    item = ModContent.ItemType<UnstableGraniteCore>();
                    chestSubType = 50; //Granite
                    break;
                case UndergroundShrineType.Marble:
                    item = ModContent.ItemType<GladiatorsLocket>();
                    chestSubType = 51; //Marble
                    break;
                case UndergroundShrineType.Abyss:
                    item = ModContent.ItemType<Terminus>();
                    chestType = ModContent.TileType<Tiles.FurnitureVoid.VoidChest>();
                    break;
            }

            // Destroy tiles in chest spawn location
            for (int j = CalamityWorld.SChestX[(int)shrineType] - 1; j <= CalamityWorld.SChestX[(int)shrineType] + 1; j++)
            {
                for (int k = CalamityWorld.SChestY[(int)shrineType]; k <= CalamityWorld.SChestY[(int)shrineType] + 2; k++)
                    WorldGen.KillTile(j, k, false, false, false);
            }

            // Attempt to fix sloped tiles under the chest to prevent the chest from killing itself (literally)
            for (int l = CalamityWorld.SChestX[(int)shrineType] - 1; l <= CalamityWorld.SChestX[(int)shrineType] + 1; l++)
            {
                for (int m = CalamityWorld.SChestY[(int)shrineType]; m <= CalamityWorld.SChestY[(int)shrineType] + 3; m++)
                {
                    if (m < Main.maxTilesY)
                    {
                        Main.tile[l, m].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                        Main.tile[l, m].Get<TileWallWireStateData>().IsHalfBlock = false;
                    }
                }
            }

            // Place the chest, finally
            WorldGen.AddBuriedChest(CalamityWorld.SChestX[(int)shrineType], CalamityWorld.SChestY[(int)shrineType], item, false, chestSubType, false, (ushort)(chestType));
        }
        #endregion

        #region Direct Gen
        /*public static void PlaceShrines()
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

                if (Main.tile[tilesX, tilesY].TileType == TileID.Dirt || Main.tile[tilesX, tilesY].TileType == TileID.Stone)
                {
                    SpecialHut(TileID.RedBrick, TileID.Dirt, WallID.RedBrick, 0, tilesX, tilesY);
                    break;
                }
                if (Main.tile[tilesX2, tilesY].TileType == TileID.Dirt || Main.tile[tilesX2, tilesY].TileType == TileID.Stone)
                {
                    SpecialHut(TileID.RedBrick, TileID.Dirt, WallID.RedBrick, 0, tilesX2, tilesY);
                    break;
                }
            }

            for (int k = 0; k < (int)(x * y * shrineChance); k++) //Evil Shrine
            {
                int tilesX = WorldGen.genRand.Next(0, x);
                int tilesY = WorldGen.genRand.Next((int)(y * 0.3f), (int)(y * 0.35f));

                if (Main.tile[tilesX, tilesY].TileType == (WorldGen.crimson ? TileID.Crimstone : TileID.Ebonstone))
                {
                    SpecialHut(WorldGen.crimson ? TileID.CrimtaneBrick : TileID.DemoniteBrick,
                        WorldGen.crimson ? TileID.Crimstone : TileID.Ebonstone,
                        WorldGen.crimson ? WallID.CrimtaneBrick : WallID.DemoniteBrick, UndergroundShrineType.WorldEvil, tilesX, tilesY);
                    break;
                }
            }

            for (int k = 0; k < (int)(x * y * shrineChance); k++) //Cavern
            {
                int tilesX = WorldGen.genRand.Next((int)(x * 0.3), generateBack);
                int tilesX2 = WorldGen.genRand.Next(generateForward, (int)(x * 0.7));
                int tilesY = WorldGen.genRand.Next((int)(y * 0.55f), (int)(y * 0.8f));

                if (Main.tile[tilesX, tilesY].TileType == TileID.Stone)
                {
                    SpecialHut(TileID.ObsidianBrick, TileID.Obsidian, WallID.ObsidianBrick, UndergroundShrineType.Cavern, tilesX, tilesY);
                    break;
                }
                if (Main.tile[tilesX2, tilesY].TileType == TileID.Stone)
                {
                    SpecialHut(TileID.ObsidianBrick, TileID.Obsidian, WallID.ObsidianBrick, UndergroundShrineType.Cavern, tilesX2, tilesY);
                    break;
                }
            }

            for (int k = 0; k < (int)(x * y * shrineChance); k++) //Ice Shrine
            {
                int tilesX = WorldGen.genRand.Next(0, x);
                int tilesY = WorldGen.genRand.Next((int)(y * 0.35f), (int)(y * 0.5f));

                if (Main.tile[tilesX, tilesY].TileType == TileID.IceBlock)
                {
                    SpecialHut(TileID.IceBrick, TileID.IceBlock, WallID.IceBrick, UndergroundShrineType.Ice, tilesX, tilesY);
                    break;
                }
            }

            for (int k = 0; k < (int)(x * y * shrineChance); k++) //Desert Shrine
            {
                int tilesX = WorldGen.genRand.Next(0, x);
                int tilesY = WorldGen.genRand.Next((int)(y * 0.3f), (int)(y * 0.5f));

                if (Main.tile[tilesX, tilesY].TileType == TileID.DesertFossil)
                {
                    SpecialHut(TileID.DesertFossil, TileID.Sandstone, WallID.DesertFossil, UndergroundShrineType.Desert, tilesX, tilesY);
                    break;
                }
            }

            for (int k = 0; k < (int)(x * y * shrineChance); k++) //Mushroom Shrine
            {
                int tilesX = WorldGen.genRand.Next(0, x);
                int tilesY = WorldGen.genRand.Next((int)(y * 0.35f), (int)(y * 0.5f));

                if (Main.tile[tilesX, tilesY].TileType == TileID.MushroomGrass)
                {
                    SpecialHut(TileID.MushroomBlock, TileID.Mud, WallID.MushroomUnsafe, UndergroundShrineType.Mushroom, tilesX, tilesY);
                    break;
                }
            }

            for (int k = 0; k < (int)(x * y * shrineChance); k++) //Granite Shrine
            {
                int tilesX = WorldGen.genRand.Next(0, x);
                int tilesY = WorldGen.genRand.Next((int)(y * 0.35f), (int)(y * 0.5f));

                if (Main.tile[tilesX, tilesY].TileType == TileID.Granite)
                {
                    SpecialHut(TileID.GraniteBlock, TileID.Granite, WallID.GraniteUnsafe, UndergroundShrineType.Granite, tilesX, tilesY);
                    break;
                }
            }

            for (int k = 0; k < (int)(x * y * shrineChance); k++) //Marble Shrine
            {
                int tilesX = WorldGen.genRand.Next(0, x);
                int tilesY = WorldGen.genRand.Next((int)(y * 0.35f), (int)(y * 0.5f));

                if (Main.tile[tilesX, tilesY].TileType == TileID.Marble)
                {
                    SpecialHut(TileID.MarbleBlock, TileID.Marble, WallID.MarbleUnsafe, UndergroundShrineType.Marble, tilesX, tilesY);
                    break;
                }
            }
        }*/
        #endregion
    }
}
