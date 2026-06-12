using System;
using System.Collections.Generic;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.Potions.Food;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Schematics;
using CalamityMod.Tiles;
using CalamityMod.Tiles.FurnitureVoid;
using CalamityMod.Walls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static CalamityMod.Schematics.SchematicManager;
using static Terraria.ModLoader.ModContent;

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
            if (tile.TileType == TileType<Tiles.SunkenSea.Navystone>() ||
            tile.TileType == TileType<Tiles.SunkenSea.EutrophicSand>() ||
            tile.WallType == WallType<NavystoneWall>())
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
                new ChestItem(ItemType<CorruptionEffigy>(), 1),
                new ChestItem(ItemID.RottenChunk, WorldGen.genRand.Next(24, 28 + 1)),
                new ChestItem(ItemID.CorruptionKey, 1),
                new ChestItem(ItemID.CorruptTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(8, 10 + 1)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(10, 12 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(10, 12 + 1)),
            };

            if (Main.zenithWorld)
            {
                int evil = Utils.SelectRandom(WorldGen.genRand, ItemType<StressPills>(), ItemType<Laudanum>(), ItemType<HeartofDarkness>());
                contents = new List<ChestItem>()
            {
                new ChestItem(ItemType<CorruptionEffigy>(), 1),
                new ChestItem(ItemID.RottenChunk, WorldGen.genRand.Next(24, 28 + 1)),
                new ChestItem(ItemID.CorruptionKey, 1),
                new ChestItem(ItemID.CorruptTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(8, 10 + 1)),
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
                int placementPositionY = WorldGen.genRand.Next((int)Main.worldSurface, (int)(Main.maxTilesY * 0.5f));
                Point placementPoint = new Point(placementPositionX, placementPositionY);

                Vector2 schematicSize = new Vector2(TileMaps[mapKey].GetLength(0) / 2, TileMaps[mapKey].GetLength(1)); //Fooling the system into thinking the shrine is smaller than it actually is so it fits into chasms
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
                if (!canGenerateInLocation || corruptStuffInArea < totalTiles * 0.9f || !structures.CanPlace(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y)) || !inYourWalls)
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
                new ChestItem(ItemType<CrimsonEffigy>(), 1),
                new ChestItem(ItemID.Vertebrae, WorldGen.genRand.Next(24, 28 + 1)),
                new ChestItem(ItemID.CrimsonKey, 1),
                new ChestItem(ItemID.CrimsonTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(8, 10 + 1)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(10, 12 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(10, 12 + 1)),
            };

            //Gfb loot change
            if (Main.zenithWorld)
            {
                //Cannot modify the return value of List<ChestItem>.this[int] because its not a variable so gotta do this instead
                contents = new List<ChestItem>()
                {
                new ChestItem(ItemType<CrimsonEffigy>(), 1),
                new ChestItem(ItemID.Vertebrae, WorldGen.genRand.Next(24, 28 + 1)),
                new ChestItem(ItemID.CrimsonKey, 1),
                new ChestItem(ItemID.CrimsonTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(8, 10 + 1)),
                new ChestItem(ItemType<BloodyMary>(), WorldGen.genRand.Next(2, 2 + 1)),
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
                int placementPositionY = WorldGen.genRand.Next((int)Main.worldSurface, (int)(Main.maxTilesY * 0.5f));
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
                new ChestItem(ItemType<LuxorsGift>(), 1),
                new ChestItem(ItemType<Items.Placeables.SunkenSea.PrismShard>(), WorldGen.genRand.Next(6, 8 + 1)),
                new ChestItem(ItemID.DungeonDesertKey, 1),
                new ChestItem(ItemID.DesertTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(8, 10 + 1)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(10, 12 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(10, 12 + 1)),
            };
            if (Main.zenithWorld)
            {
                int golfClub = Utils.SelectRandom(WorldGen.genRand, ItemID.GolfClubBronzeWedge, ItemID.GolfClubWedge, ItemID.GasTrap);
                contents = new List<ChestItem>()
                {
                new ChestItem(ItemType<LuxorsGift>(), 1),
                new ChestItem(ItemType<Items.Placeables.SunkenSea.PrismShard>(), WorldGen.genRand.Next(6, 8 + 1)),
                new ChestItem(ItemID.DungeonDesertKey, 1),
                new ChestItem(ItemID.DesertTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(8, 10 + 1)),
                new ChestItem(ItemType<SpelunkersAmulet>(), 1),
                new ChestItem(ItemID.RedPotion, WorldGen.genRand.Next(1, 2 + 1)),
                new ChestItem(golfClub, 1), //Implying that the golfer messed with the loot but forgot this, OR its trapped
                };
            }

            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].Prefix(-1);
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
                new ChestItem(ItemType<UnstableGraniteCore>(), 1),
                new ChestItem(ItemID.Geode, WorldGen.genRand.Next(6, 8 + 1)),
                new ChestItem(ItemID.BlueTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(8, 10 + 1)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(10, 12 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(Main.zenithWorld ? 1 : 10, (Main.zenithWorld ? 2 : 12) + 1)),
                new ChestItem((Main.rand.NextBool() && Main.zenithWorld) ? ItemID.GasTrap : ItemID.Granite, Main.zenithWorld ? 1 : WorldGen.genRand.Next(7,15+1)),
            };

            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].Prefix(-1);
                chest.item[i].stack = contents[i].Stack;
            }
        }
        public static void PlaceGraniteShrine(StructureMap structures)
        {
            int tries = 0;
            string mapKey = GraniteShrineKey;

            do
            {
                int placementPositionX = Main.rand.NextBool() ? WorldGen.genRand.Next((int)(Main.maxTilesX * 0.55f), Main.maxTilesX - WorldGen.beachDistance) : WorldGen.genRand.Next(WorldGen.beachDistance, (int)(Main.maxTilesX * 0.45f));
                int placementPositionY = WorldGen.genRand.Next((int)GenVars.rockLayer + 20, Main.maxTilesY - 220);
                if (Main.remixWorld)
                    placementPositionY = WorldGen.genRand.Next((int)GenVars.worldSurface + 100, (int)GenVars.rockLayer);
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
                        if (ShouldAvoidLocation(new Point(x, y), false))
                            canGenerateInLocation = false;

                        //The granite geode is supposed to fully float in free air. No tile replacements
                        if (tile.WallType == WallID.GraniteUnsafe && !tile.HasTile && !Main.drunkWorld)
                            graniteWallsInArea++;
                        // Drunk world variant
                        else if ((tile.WallType == WallID.MarbleUnsafe || tile.TileType == TileID.Marble) && Main.drunkWorld)
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
                    PlaceSchematic(mapKey, new Point(placementPoint.X, placementPoint.Y), SchematicAnchor.TopLeft, ref _, Main.drunkWorld ? new Action<Chest>(FillMarbleShrineChest) : new Action<Chest>(FillGraniteShrineChest));
                    CalamityUtils.AddProtectedStructure(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y), 4);

                    // Drunk world: turns into a Marble Geode
                    if (Main.drunkWorld)
                    {
                        for (int x = placementPoint.X; x < placementPoint.X + schematicSize.X; x++)
                        {
                            for (int y = placementPoint.Y; y < placementPoint.Y + schematicSize.Y; y++)
                            {
                                Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);
                                switch (tile.TileType)
                                {
                                    case TileID.Granite: // Granite Block --> Marble Block
                                        tile.TileType = TileID.Marble;
                                        break;
                                    case TileID.GraniteBlock: // Smooth Granite Block --> Smooth Marble Block
                                        tile.TileType = TileID.MarbleBlock;
                                        break;
                                    case TileID.Containers: // Granite Chest --> Marble Chest
                                        tile.TileFrameX += 36;
                                        break;
                                    case TileID.ExposedGems: // Sapphire --> Diamond
                                        tile.TileFrameX += 54;
                                        break;
                                }
                                switch (tile.WallType)
                                {
                                    case WallID.Granite: // Granite Wall --> Marble Wall
                                        tile.WallType = WallID.Marble;
                                        break;
                                    case WallID.SapphireGemspark: // Sapphire Gemspark Wall --> Diamond Gemspark Wall
                                        tile.WallType = WallID.DiamondGemspark;
                                        tile.WallColor = PaintID.None;
                                        break;
                                }
                            }
                        }
                    }
                    break;
                }

            } while (tries <= 30000);
        }
        #endregion

        #region Ice Shrine
        public static void FillIceShrineChest(Chest chest)
        {
            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(ItemType<FrozenCube>(), 1),
                new ChestItem(ItemID.FlinxFur, WorldGen.genRand.Next(6, 8 + 1)),
                new ChestItem(ItemID.FrozenKey, 1),
                new ChestItem(ItemID.IceTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(8, 10 + 1)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(10, 12 + 1)),
                new ChestItem(ItemID.IceCream, WorldGen.genRand.Next(10, 12 + 1)),
            };

            if (Main.zenithWorld)
            {
                contents = new List<ChestItem>()
            {
                new ChestItem(ItemType<FrozenCube>(), 1),
                new ChestItem(ItemID.FlinxFur, WorldGen.genRand.Next(6, 8 + 1)),
                new ChestItem(ItemID.FrozenKey, 1),
                new ChestItem(ItemID.IceTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(8, 10 + 1)),
                new ChestItem(ItemID.Eggnog, WorldGen.genRand.Next(10, 12 + 1)),
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
                new ChestItem(ItemType<GladiatorsLocket>(), 1),
                new ChestItem(GenVars.goldBar == TileID.Gold ? ItemID.GoldBar : ItemID.PlatinumBar, WorldGen.genRand.Next(12, 15 + 1)),
                new ChestItem(ItemID.WhiteTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(8, 10 + 1)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(10, 12 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(Main.zenithWorld ? 1 : 10, (Main.zenithWorld ? 2 : 12) + 1)),
                new ChestItem((Main.rand.NextBool() && Main.zenithWorld) ? ItemID.GasTrap : ItemID.Marble, Main.zenithWorld ? 1 : WorldGen.genRand.Next(7,15+1)),
            };

            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].Prefix(-1);
                chest.item[i].stack = contents[i].Stack;
            }
        }
        public static void PlaceMarbleShrine(StructureMap structures)
        {
            int tries = 0;
            string mapKey = MarbleShrineKey;

            do
            {
                int placementPositionX = Main.rand.NextBool() ? WorldGen.genRand.Next((int)(Main.maxTilesX * 0.55f), Main.maxTilesX - WorldGen.beachDistance) : WorldGen.genRand.Next(WorldGen.beachDistance, (int)(Main.maxTilesX * 0.45f));
                int placementPositionY = WorldGen.genRand.Next((int)GenVars.rockLayer + 20, Main.maxTilesY - 220);
                if (Main.remixWorld)
                    placementPositionY = WorldGen.genRand.Next((int)GenVars.worldSurface + 100, (int)GenVars.rockLayer);
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
                        if ((tile.TileType == TileID.Marble || tile.WallType == WallID.MarbleUnsafe) && !Main.drunkWorld)
                            marbleStuffInArea++;
                        // Drunk world variant
                        else if ((tile.TileType == TileID.Granite || tile.WallType == WallID.GraniteUnsafe) && Main.drunkWorld)
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
                    PlaceSchematic(mapKey, new Point(placementPoint.X, placementPoint.Y), SchematicAnchor.TopLeft, ref _, Main.drunkWorld ? new Action<Chest>(FillGraniteShrineChest) : new Action<Chest>(FillMarbleShrineChest));
                    CalamityUtils.AddProtectedStructure(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y), 4);

                    // Drunk world: turns into a Granite Column
                    if (Main.drunkWorld)
                    {
                        for (int x = placementPoint.X; x < placementPoint.X + schematicSize.X; x++)
                        {
                            for (int y = placementPoint.Y; y < placementPoint.Y + schematicSize.Y; y++)
                            {
                                Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);
                                switch (tile.TileType)
                                {
                                    case TileID.Marble: // Marble Block --> Granite Block
                                        tile.TileType = TileID.Granite;
                                        break;
                                    case TileID.MarbleBlock: // Smooth Marble Block --> Smooth Granite Block
                                        tile.TileType = TileID.GraniteBlock;
                                        break;
                                    case TileID.MarbleColumn: // Marble Column --> Granite Column
                                        tile.TileType = TileID.GraniteColumn;
                                        break;
                                    case TileID.Containers: // Marble Chest --> Granite Chest
                                        tile.TileFrameX -= 36;
                                        break;
                                    case TileID.Platforms: // Marble Platform --> Granite Platform
                                        tile.TileFrameY -= 18;
                                        break;
                                }
                                switch (tile.WallType)
                                {
                                    case WallID.Marble: // Marble Wall --> Granite Wall
                                        tile.WallType = WallID.Granite;
                                        break;
                                    case WallID.MarbleBlock: // Smooth Marble Wall --> Smooth Granite Wall
                                        tile.WallType = WallID.GraniteBlock;
                                        tile.WallColor = PaintID.None;
                                        break;
                                }
                            }
                        }
                    }
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
                new ChestItem(ItemType<FungalSymbiote>(), 1),
                new ChestItem(ItemID.TruffleWorm, 3),
                new ChestItem(ItemID.MushroomTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(8, 10 + 1)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(10, 12 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(10, 12 + 1)),
                };

            // Gfb loot change
            if (Main.zenithWorld)
            {
                //"Cannot modify the return value of List<ChestItem>.this[int] because its not a variable" so gotta do this instead, I could add a bunch of bools but I feel this is better for how much is changed
                contents = new List<ChestItem>()
                {
                new ChestItem(ItemType<FungalSymbiote>(), 1),
                new ChestItem(ItemID.TruffleWorm, 3),
                new ChestItem(ItemID.MushroomTorch, WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(8, 10 + 1)),
                new ChestItem(ItemType<OddMushroom>(), WorldGen.genRand.Next(2, 3 + 1)),
                new ChestItem(ItemID.RedPotion, WorldGen.genRand.Next(1, 2 + 1)),
                new ChestItem(ItemID.GasTrap, 1)
                };
            }


            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].Prefix(-1);
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
                new ChestItem(ItemType<TrinketofChi>(), 1),
                new ChestItem(ItemID.PinkGel, WorldGen.genRand.Next(12, 15 + 1)),
                new ChestItem(ItemID.Torch, WorldGen.genRand.Next(50, 60 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(2, 4 + 1)),
                new ChestItem(Main.zenithWorld ? ItemID.RestorationPotion : ItemID.LesserHealingPotion, WorldGen.genRand.Next(10, 12 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(10, 12 + 1)),
                new ChestItem(Main.zenithWorld ? ItemID.GasTrap : ItemID.Mushroom, Main.zenithWorld ? 1 : WorldGen.genRand.Next(5,9+1)),
            };

            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].Prefix(-1);
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
                    }
                    else
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

        #region Roxcalibur Shrine
        public static void PlaceRoxShrine(StructureMap structures)
        {
            int tries = 0;
            string mapKey = Main.rand.NextBool() ?  RoxcaliburShrineKey1 : RoxcaliburShrineKey2;
            Vector2 schematicSize = new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1));

            do
            {
                int placementPositionX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.15f), (int)(Main.maxTilesX * 0.85f));
                int placementPositionY = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.75f), Main.UnderworldLayer-50); //Lava layer
                
                Point placementPoint = new Point(placementPositionX, placementPositionY);
                int yExtraArea = 10;
                bool canGenerateInLocation = true;

                for (int x = placementPoint.X; x < placementPoint.X + schematicSize.X; x++)
                {
                    for (int y = placementPoint.Y; y < placementPoint.Y + schematicSize.Y + yExtraArea; y++)
                    {
                        Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);

                        //Avoid shacks, jungle and mushroom biomes
                        if (tile.TileType == TileID.WoodBlock || tile.TileType == TileID.Mud || tile.TileType == TileType<VernalSoil>())
                            canGenerateInLocation = false;
                            
                        //Try to not be in a place with lava on the top half or above the shrine
                        if (ShouldAvoidLocation(new Point(x, y-20), true))
                            canGenerateInLocation = false;

                        //Check for the rest of the structure
                        if (ShouldAvoidLocation(new Point(x, y), false))
                            canGenerateInLocation = false;
                    }
                }
                if ((!canGenerateInLocation || !structures.CanPlace(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y))) && !Main.remixWorld)
                {
                    tries++;
                }
                else
                {
                    bool _ = false;
                    //added those first things so it stops complaining, there's no easter bunny, there's no tooth fairy and there is no chest
                    PlaceSchematic<Action<Chest>>(mapKey, new Point(placementPoint.X, placementPoint.Y), SchematicAnchor.TopLeft, ref _);
                    //Do not get eaten by other structures or Fargo's instabridge
                    CalamityUtils.AddProtectedStructure(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y), 4);
                    return;
                }
            } while (tries <= 100000);
            CalamityMod.Log.Debug("Rox Shrine failed to generate");
        }
        #endregion

        #region Abyss Shrine
        public static void PlaceAbyssShrine(int chestLeftX, int chestTopY)
        {
            // Shrine position
            int shrineLeftX = chestLeftX - 5;
            int shrineRightX = chestLeftX + 4;
            int shrineTopY = chestTopY - 5;
            int shrineBottomY = chestTopY + 3;

            // Creates a solid box of Smooth Voidstone
            for (int x = shrineLeftX; x <= shrineRightX; x++)
            {
                for (int y = shrineTopY; y <= shrineBottomY; y++)
                {
                    Main.tile[x, y].Get<TileWallWireStateData>().HasTile = true;
                    Main.tile[x, y].TileType = (ushort)TileType<SmoothVoidstone>();
                    Main.tile[x, y].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                    Main.tile[x, y].Get<LiquidData>().LiquidType = LiquidID.Water;
                }
            }

            // Carve out the inner room to make the above box hollow
            for (int x = shrineLeftX + 1; x <= shrineRightX - 1; x++)
            {
                for (int y = shrineTopY + 1; y <= shrineBottomY - 1; y++)
                    Main.tile[x, y].Get<TileWallWireStateData>().HasTile = false;
            }

            // Carve out the entrance
            for (int x = shrineLeftX; x <= shrineRightX; x++)
            {
                for (int y = shrineBottomY - 3; y <= shrineBottomY - 1; y++)
                    Main.tile[x, y].Get<TileWallWireStateData>().HasTile = false;
            }

            // Add in the pyramid-like hat
            int yTop = shrineTopY - 1;
            int halfWidth = 4;
            while (halfWidth > -1)
            {
                halfWidth -= WorldGen.genRand.Next(1, 3); // Random step distance
                for (int x = chestLeftX - halfWidth - 1; x <= chestLeftX + halfWidth; x++)
                {
                    Main.tile[x, yTop].Get<TileWallWireStateData>().HasTile = true;
                    Main.tile[x, yTop].TileType = (ushort)TileType<SmoothVoidstone>();
                }
                yTop--;
            }

            // Place the chest and fill it
            Chest chest = MiscWorldgenRoutines.AddChestWithLoot(chestLeftX, chestTopY, (ushort)TileType<VoidChest>());
            if (chest != null)
                FillAbyssShrineChest(chest);
        }

        public static void FillAbyssShrineChest(Chest chest)
        {
            int dropType = Utils.SelectRandom(WorldGen.genRand, ItemType<AbyssShocker>(), ItemType<DepthCrusher>(), ItemType<InkBomb>());
            int potionType = Utils.SelectRandom(WorldGen.genRand, ItemID.PotionOfReturn, ItemID.LuckPotionGreater);
            if (Main.zenithWorld)
            {
                dropType = ItemID.OldShoe;
                potionType = ItemID.RedPotion;
            }
            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(ItemType<Terminus>(), 1),
                new ChestItem(dropType, 1),
                new ChestItem(ItemType<VoidTorch>(), WorldGen.genRand.Next(100, 110 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(8, 10 + 1)),
                new ChestItem(ItemType<HadalStew>(), WorldGen.genRand.Next(10, 12 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(10, 12 + 1)),
            };

            if (Main.zenithWorld)
                contents.Add(new ChestItem(ItemID.GasTrap, 1));

            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].Prefix(-1);
                chest.item[i].stack = contents[i].Stack;
            }
        }
        #endregion
    }
}
