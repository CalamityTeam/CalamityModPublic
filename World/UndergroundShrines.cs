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

namespace CalamityMod.World
{
    public class UndergroundShrines
    {
        #region Schematics
        public static bool ShouldAvoidLocation(Point placementPoint, bool careAboutLava = true)
        {
            Tile tile = CalamityUtils.ParanoidTileRetrieval(placementPoint.X, placementPoint.Y);
            if (tile.LiquidType == LiquidID.Lava && careAboutLava)
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
            if (tile.TileType == ModContent.TileType<HazardChevronPanels>() ||
            tile.TileType == ModContent.TileType<LaboratoryPanels>() ||
            tile.TileType == ModContent.TileType<LaboratoryPipePlating>() ||
            tile.TileType == ModContent.TileType<LaboratoryPlating>() ||
            tile.TileType == ModContent.TileType<RustedPipes>() ||
            tile.TileType == ModContent.TileType<RustedPlating>())
            {
                return true;
            }

            return false;
        }

        public static void FillDesertShrineChest(Chest chest)
        {
            int potionType = Utils.SelectRandom(WorldGen.genRand, ItemID.SpelunkerPotion, ItemID.MiningPotion, ItemID.BuilderPotion);
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
                int placementPositionX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.2f), (int)(Main.maxTilesX * 0.8f));
                int placementPositionY = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.3f), (int)(Main.maxTilesY * 0.6f));
                Point placementPoint = new Point(placementPositionX, placementPositionY);

                Vector2 schematicSize = new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1));
                int desertTilesInArea = 0;
                int desertWallsInArea = 0;
                int xCheckArea = 40;
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

                        //The desert is absolutely covered with walls. This prevents it from generating in random sand patches.
                        if (tile.WallType == WallID.HardenedSand || tile.WallType == WallID.Sandstone)
                                desertWallsInArea++;
                    }
                }
                if (!canGenerateInLocation || desertTilesInArea < totalTiles * 0.5f || desertWallsInArea < totalTiles * 0.9f || !structures.CanPlace(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y)))
                    tries++;
                else
                {
                    bool _ = true;
                    PlaceSchematic(mapKey, new Point(placementPoint.X, placementPoint.Y), SchematicAnchor.TopLeft, ref _, new Action<Chest>(FillDesertShrineChest));
                    structures.AddProtectedStructure(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y), 4);
                    break;
                }

            } while (tries <= 50000);
        }
        #endregion

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
                    chestSubType = 4; //Locked Shadow
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
        public static void PlaceShrines()
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

            /*for (int k = 0; k < (int)(x * y * shrineChance); k++) //Cavern: Done.
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
            }*/

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

            /*for (int k = 0; k < (int)(x * y * shrineChance); k++) //Desert Shrine: Done.
            {
                int tilesX = WorldGen.genRand.Next(0, x);
                int tilesY = WorldGen.genRand.Next((int)(y * 0.3f), (int)(y * 0.5f));

                if (Main.tile[tilesX, tilesY].TileType == TileID.DesertFossil)
                {
                    SpecialHut(TileID.DesertFossil, TileID.Sandstone, WallID.DesertFossil, UndergroundShrineType.Desert, tilesX, tilesY);
                    break;
                }
            }*/

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
        }
        #endregion
    }
}
