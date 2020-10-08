using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Schematics;
using CalamityMod.Tiles;
using CalamityMod.Tiles.SunkenSea;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using static CalamityMod.Schematics.SchematicLoader;
using static CalamityMod.Schematics.SchematicPlacementHelpers;

namespace CalamityMod.World
{
    internal struct ChestItem
    {
        internal int Type;
        internal int Stack;
        internal ChestItem(int type, int stack)
        {
            Type = type;
            Stack = stack;
        }
    }
    public static class DraedonStructures
    {
        public static bool ShouldAvoidLocation(Point placementPoint, bool careAboutLava = true)
        {
            Tile tile = CalamityUtils.ParanoidTileRetrieval(placementPoint.X, placementPoint.Y);
            if (tile.lava() && careAboutLava)
                return true;
            if (tile.type == TileID.BlueDungeonBrick ||
                tile.type == TileID.GreenDungeonBrick ||
                tile.type == TileID.PinkDungeonBrick)
            {
                return true;
            }
            if (tile.type == TileID.LihzahrdBrick ||
                tile.wall == WallID.LihzahrdBrickUnsafe)
            {
                return true;
            }
            if (tile.type == TileID.Crimstone ||
                tile.wall == WallID.CrimstoneUnsafe ||
                tile.type == TileID.Ebonstone ||
                tile.wall == WallID.EbonstoneUnsafe)
            {
                return true;
            }
			// Avoid Thorium's Blood Chamber (where you summon Viscount)
            if (tile.type == TileID.StoneSlab ||
                tile.wall == WallID.StoneSlab)
            {
                return true;
            }
            return false;
        }

        #region Workshop
        public static void FillWorkshopChest(Chest chest)
        {
            int potionType = Utils.SelectRandom(WorldGen.genRand, ItemID.EndurancePotion, ItemID.GravitationPotion, ItemID.HeartreachPotion, ItemID.LifeforcePotion);
            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(ModContent.ItemType<DubiousPlating>(), WorldGen.genRand.Next(8, 14 + 1)),
                new ChestItem(ModContent.ItemType<MysteriousCircuitry>(), WorldGen.genRand.Next(7, 12 + 1)),
                new ChestItem(ItemID.Torch, WorldGen.genRand.Next(15, 29 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(5, 11 + 1)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(5, 7 + 1)),
                new ChestItem(ItemID.Bomb, WorldGen.genRand.Next(6, 7 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(3, 5 + 1)),
            };
            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }
        public static void PlaceWorkshop(out Point placementPoint, List<Point> workshopPoints)
        {
            int tries = 0;
            string mapKey = "Workshop";

            do
            {
                int underworldTop = Main.maxTilesY - 200;
                int placementPositionX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f));
                int placementPositionY = WorldGen.genRand.Next(underworldTop - 550, underworldTop - 50);

                placementPoint = new Point(placementPositionX, placementPositionY);
                Vector2 schematicSize = new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1));
                int activeTilesInArea = 0;
                int xCheckArea = 40;
                bool canGenerateInLocation = true;

                // new Vector2 is used here since a lambda expression cannot capture a ref, out, or in parameter.
                bool nearbyOtherWorkshop = workshopPoints.Any(point => Vector2.Distance(point.ToVector2(), new Vector2(placementPositionX, placementPositionY)) < 240f);
                float totalTiles = (schematicSize.X + xCheckArea * 2) * schematicSize.Y;
                for (int x = placementPoint.X - xCheckArea; x < placementPoint.X + schematicSize.X + xCheckArea; x++)
                {
                    for (int y = placementPoint.Y; y < placementPoint.Y + schematicSize.Y; y++)
                    {
                        Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);
                        if (ShouldAvoidLocation(new Point(x, y)))
                            canGenerateInLocation = false;

                        if (tile.active())
                            activeTilesInArea++;
                    }
                }
                if (!canGenerateInLocation || nearbyOtherWorkshop || activeTilesInArea / totalTiles > 0.35f)
                    tries++;
				else
                {
                    bool _ = true;
                    PlaceStructure(mapKey, new Point(placementPoint.X, placementPoint.Y), PlacementAnchorType.TopLeft, ref _, new Action<Chest>(chest => FillWorkshopChest(chest)));
                    break;
                }

            } while (tries <= 25000);
        }
        #endregion

        #region Research Facility
        public static void FillLaboratoryChest(Chest chest)
        {
            int potionType = Utils.SelectRandom(WorldGen.genRand, ItemID.EndurancePotion, ItemID.GravitationPotion, ItemID.HeartreachPotion, ItemID.LifeforcePotion);
            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(ModContent.ItemType<DubiousPlating>(), WorldGen.genRand.Next(10, 17 + 1)),
                new ChestItem(ModContent.ItemType<MysteriousCircuitry>(), WorldGen.genRand.Next(10, 15 + 1)),
                new ChestItem(ItemID.Torch, WorldGen.genRand.Next(20, 40 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(8, 16 + 1)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(7, 10 + 1)),
                new ChestItem(ItemID.Dynamite, WorldGen.genRand.Next(4, 6 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(4, 7 + 1)),
            };
            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }
        public static void PlaceResearchFacility(out Point placementPoint, List<Point> workshopPoints)
        {
            int tries = 0;
            string mapKey = "Research Facility";

            do
            {
                int underworldTop = Main.maxTilesY - 200;
                int placementPositionX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.15f), (int)(Main.maxTilesX * 0.85f));
                int placementPositionY = WorldGen.genRand.Next(underworldTop - 400, underworldTop - 50);

                placementPoint = new Point(placementPositionX, placementPositionY);
                Vector2 schematicSize = new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1));
                int activeTilesInArea = 0;
                int xCheckArea = 30;
                bool canGenerateInLocation = true;

                // new Vector2 is used here since a lambda expression cannot capture a ref, out, or in parameter.
                bool nearbyOtherWorkshop = workshopPoints.Any(point => Vector2.Distance(point.ToVector2(), new Vector2(placementPositionX, placementPositionY)) < 240f);
                float totalTiles = (schematicSize.X + xCheckArea * 2) * schematicSize.Y;
                for (int x = placementPoint.X - xCheckArea; x < placementPoint.X + schematicSize.X + xCheckArea; x++)
                {
                    for (int y = placementPoint.Y; y < placementPoint.Y + schematicSize.Y; y++)
                    {
                        Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);
                        if (ShouldAvoidLocation(new Point(x, y)))
                            canGenerateInLocation = false;

                        if (tile.active())
                            activeTilesInArea++;
                    }
                }
                if (!canGenerateInLocation || nearbyOtherWorkshop || activeTilesInArea / totalTiles > 0.35f)
                    tries++;
                else
                {
                    bool _ = true;
                    PlaceStructure(mapKey, new Point(placementPoint.X, placementPoint.Y), PlacementAnchorType.TopLeft, ref _, new Action<Chest>(chest => FillLaboratoryChest(chest)));
                    break;
                }
            }
            while (tries <= 25000);
        }
        #endregion

        #region Hell Lab
        public static void FillHellLaboratoryChest(Chest chest, int type, bool hasPlacedMurasama)
        {
            int potionType = Utils.SelectRandom(WorldGen.genRand, ItemID.EndurancePotion, ItemID.GravitationPotion, ItemID.HeartreachPotion, ItemID.LifeforcePotion);
            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(ModContent.ItemType<DubiousPlating>(), WorldGen.genRand.Next(8, 14 + 1)),
                new ChestItem(ModContent.ItemType<MysteriousCircuitry>(), WorldGen.genRand.Next(7, 12 + 1)),
                new ChestItem(ItemID.Torch, WorldGen.genRand.Next(15, 29 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(5, 11 + 1)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(5, 7 + 1)),
                new ChestItem(ItemID.Bomb, WorldGen.genRand.Next(6, 7 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(3, 5 + 1)),
            };
            if (!hasPlacedMurasama)
            {
                contents.Insert(0, new ChestItem(ModContent.ItemType<DraedonsLogHell>(), 1));
                contents.Insert(1, new ChestItem(ModContent.ItemType<Murasama>(), 1));
                contents.Insert(2, new ChestItem(ModContent.ItemType<EncryptedSchematic>(), 1));
            }
            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }

        public static void PlaceHellLab(out Point placementPoint, List<Point> workshopPoints)
        {
            int tries = 0;
            string mapKey = "Hell Laboratory";
            PilePlacementMaps.TryGetValue(mapKey, out PilePlacementFunction pilePlacementFunction);
            ColorTileCombination[,] schematic = TileMaps[mapKey];

            do
            {
                int underworldTop = Main.maxTilesY - 200;
                int placementPositionX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.82), (int)(Main.maxTilesX * 0.925));
                int placementPositionY = WorldGen.genRand.Next(Main.maxTilesY - 150, Main.maxTilesY - 125);

                placementPoint = new Point(placementPositionX, placementPositionY);
                Vector2 schematicSize = new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1));
                int xCheckArea = 30;
                bool canGenerateInLocation = true;

                // new Vector2 is used here since a lambda expression cannot capture a ref, out, or in parameter.
                float totalTiles = (schematicSize.X + xCheckArea * 2) * schematicSize.Y;
                for (int x = placementPoint.X - xCheckArea; x < placementPoint.X + schematicSize.X + xCheckArea; x++)
                {
                    for (int y = placementPoint.Y; y < placementPoint.Y + schematicSize.Y; y++)
                    {
                        Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);
                        if (ShouldAvoidLocation(new Point(x, y), false))
                            canGenerateInLocation = false;
                    }
                }
                if (!canGenerateInLocation)
                {
                    tries++;
				}
				else
				{
                    bool hasPlacedMurasama = false;
                    PlaceStructure(mapKey, new Point(placementPoint.X, placementPoint.Y), PlacementAnchorType.TopLeft, ref hasPlacedMurasama, new Action<Chest, int, bool>((chest, type, hasPlaced) => FillHellLaboratoryChest(chest, type, hasPlaced)));
                    break;
                }
            }
            while (tries <= 50000);
        }
        #endregion

        #region Sunken Sea Lab

        public static void FillSunkenSeaLaboratoryChest(Chest chest, int type, bool hasPlacedLogAndSchematic)
        {
            int potionType = Utils.SelectRandom(WorldGen.genRand, ItemID.EndurancePotion, ItemID.GravitationPotion, ItemID.HeartreachPotion, ItemID.LifeforcePotion);
            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(ModContent.ItemType<DubiousPlating>(), WorldGen.genRand.Next(8, 14 + 1)),
                new ChestItem(ModContent.ItemType<MysteriousCircuitry>(), WorldGen.genRand.Next(7, 12 + 1)),
                new ChestItem(ItemID.Torch, WorldGen.genRand.Next(15, 29 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(5, 11 + 1)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(5, 7 + 1)),
                new ChestItem(ItemID.Bomb, WorldGen.genRand.Next(6, 7 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(3, 5 + 1)),
            };
            if (!hasPlacedLogAndSchematic)
            {
                contents.Insert(0, new ChestItem(ModContent.ItemType<DraedonsLogSunkenSea>(), 1));
                contents.Insert(1, new ChestItem(ModContent.ItemType<EncryptedSchematic>(), 1));
            }
            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }

        public static void PlaceSunkenSeaLab(out Point placementPoint, List<Point> workshopPoints)
        {
            int tries = 0;
            string mapKey = "Sunken Sea Laboratory";
            PilePlacementMaps.TryGetValue(mapKey, out PilePlacementFunction pilePlacementFunction);
            ColorTileCombination[,] schematic = TileMaps[mapKey];

            do
            {
                int placementPositionX = WorldGen.genRand.Next(WorldGen.UndergroundDesertLocation.Left - 20, WorldGen.UndergroundDesertLocation.Left + 10);
                if (WorldGen.genRand.NextBool(2))
                    placementPositionX = WorldGen.genRand.Next(WorldGen.UndergroundDesertLocation.Right - 10, WorldGen.UndergroundDesertLocation.Right + 20);
                int sunkenSeaHeight = WorldGen.UndergroundDesertLocation.Height / 2;
                int placementPositionY = WorldGen.genRand.Next(WorldGen.UndergroundDesertLocation.Bottom + sunkenSeaHeight - 25, WorldGen.UndergroundDesertLocation.Bottom + sunkenSeaHeight + 10);

                placementPoint = new Point(placementPositionX, placementPositionY);
                Vector2 schematicSize = new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1));
                int xCheckArea = 25;
                bool shouldAvoidArea = false;

                float totalTiles = (schematicSize.X + xCheckArea * 2) * schematicSize.Y;
                for (int x = placementPoint.X - xCheckArea; x < placementPoint.X + schematicSize.X + xCheckArea; x++)
                {
                    for (int y = placementPoint.Y; y < placementPoint.Y + schematicSize.Y; y++)
                    {
                        Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);
                        if (ShouldAvoidLocation(new Point(x, y), false))
                            shouldAvoidArea = true;
                    }
                }
                tries++;
                if (!shouldAvoidArea)
                    break;
            }
            while (tries < 50000);

            bool hasPlacedLogAndSchematic = false;
            PlaceStructure(mapKey, new Point(placementPoint.X, placementPoint.Y), PlacementAnchorType.TopLeft, ref hasPlacedLogAndSchematic, new Action<Chest, int, bool>((chest, type, hasPlaced) => FillSunkenSeaLaboratoryChest(chest, type, hasPlaced)));
        }
        #endregion

        #region Ice Lab
        public static void FillIceLaboratoryChest(Chest chest, int type, bool hasPlacedLogAndSchematic)
        {
            int potionType = Utils.SelectRandom(WorldGen.genRand, ItemID.EndurancePotion, ItemID.GravitationPotion, ItemID.HeartreachPotion, ItemID.LifeforcePotion);
            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(ModContent.ItemType<DubiousPlating>(), WorldGen.genRand.Next(8, 14 + 1)),
                new ChestItem(ModContent.ItemType<MysteriousCircuitry>(), WorldGen.genRand.Next(7, 12 + 1)),
                new ChestItem(ItemID.Torch, WorldGen.genRand.Next(15, 29 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(5, 11 + 1)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(5, 7 + 1)),
                new ChestItem(ItemID.Bomb, WorldGen.genRand.Next(6, 7 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(3, 5 + 1)),
            };
            if (!hasPlacedLogAndSchematic)
            {
                contents.Insert(0, new ChestItem(ModContent.ItemType<DraedonsLogSnowBiome>(), 1));
                contents.Insert(1, new ChestItem(ModContent.ItemType<EncryptedSchematic>(), 1));
            }
            // If it's a frozen chest.
            if (type == TileID.Containers)
            {
                int specialItem = Utils.SelectRandom(WorldGen.genRand, ItemID.ArcticDivingGear, ItemID.BlizzardinaBalloon, ItemID.FrozenTurtleShell);
                contents.Insert(0, new ChestItem(specialItem, 1));
            }
            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }

        public static void PlaceIceLab(out Point placementPoint, List<Point> workshopPoints)
        {
            int tries = 0;
            string mapKey = "Ice Laboratory";
            PilePlacementMaps.TryGetValue(mapKey, out PilePlacementFunction pilePlacementFunction);
            ColorTileCombination[,] schematic = TileMaps[mapKey];

            do
            {
                int underworldTop = Main.maxTilesY - 200;
                int placementPositionX = WorldGen.genRand.Next(120, Main.maxTilesX - 120);
                int placementPositionY = WorldGen.genRand.Next((int)Main.worldSurface + 160, underworldTop);

                placementPoint = new Point(placementPositionX, placementPositionY);
                Vector2 schematicSize = new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1));
                int activeTilesInArea = 0;
                int iceTilesInArea = 0;
                int xCheckArea = 30;
                bool canGenerateInLocation = true;

                // new Vector2 is used here since a lambda expression cannot capture a ref, out, or in parameter.
                bool nearbyOtherWorkshop = workshopPoints.Any(point => Vector2.Distance(point.ToVector2(), new Vector2(placementPositionX, placementPositionY)) < 180f);
                float totalTiles = (schematicSize.X + xCheckArea * 2) * schematicSize.Y;
                for (int x = placementPoint.X - xCheckArea; x < placementPoint.X + schematicSize.X + xCheckArea; x++)
                {
                    for (int y = placementPoint.Y; y < placementPoint.Y + schematicSize.Y; y++)
                    {
                        Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);
                        if (tile.active())
                        {
                            if (tile.type == TileID.SnowBlock || tile.type == TileID.IceBlock)
                                iceTilesInArea++;
                            activeTilesInArea++;
                        }
                        if (ShouldAvoidLocation(new Point(x, y)))
                            canGenerateInLocation = false;
                    }
                }
                if (!canGenerateInLocation || nearbyOtherWorkshop || iceTilesInArea < totalTiles * 0.35f)
                    tries++;
				else
				{
                    bool hasPlacedLogAndSchematic = false;
                    PlaceStructure(mapKey, new Point(placementPoint.X, placementPoint.Y), PlacementAnchorType.TopLeft, ref hasPlacedLogAndSchematic, new Action<Chest, int, bool>((chest, type, hasPlaced) => FillIceLaboratoryChest(chest, type, hasPlaced)));
                    break;
                }
            }
            while (tries <= 50000);
        }
        #endregion

        #region Plague Lab
        public static void FillPlagueLaboratoryChest(Chest chest, int type, bool hasPlacedLogAndSchematic)
        {
            int potionType = Utils.SelectRandom(WorldGen.genRand, ItemID.EndurancePotion, ItemID.GravitationPotion, ItemID.HeartreachPotion, ItemID.LifeforcePotion);
            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(ModContent.ItemType<DubiousPlating>(), WorldGen.genRand.Next(8, 14 + 1)),
                new ChestItem(ModContent.ItemType<MysteriousCircuitry>(), WorldGen.genRand.Next(7, 12 + 1)),
                new ChestItem(ItemID.Torch, WorldGen.genRand.Next(15, 29 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(5, 11 + 1)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(5, 7 + 1)),
                new ChestItem(ItemID.Bomb, WorldGen.genRand.Next(6, 7 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(3, 5 + 1)),
            };
            if (!hasPlacedLogAndSchematic)
            {
                contents.Insert(0, new ChestItem(ModContent.ItemType<DraedonsLogJungle>(), 1));
                contents.Insert(1, new ChestItem(ModContent.ItemType<EncryptedSchematic>(), 1));
            }
            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }

        public static void PlacePlagueLab(out Point placementPoint, List<Point> workshopPoints)
        {
            int tries = 0;
            string mapKey = "Plague Laboratory";
            PilePlacementMaps.TryGetValue(mapKey, out PilePlacementFunction pilePlacementFunction);
            ColorTileCombination[,] schematic = TileMaps[mapKey];

            do
            {
                int underworldTop = Main.maxTilesY - 200;
                int placementPositionX = WorldGen.genRand.Next(120, Main.maxTilesX - 120);
                int placementPositionY = WorldGen.genRand.Next((int)Main.worldSurface + 160, underworldTop);

                placementPoint = new Point(placementPositionX, placementPositionY);
                Vector2 schematicSize = new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1));
                int activeTilesInArea = 0;
                int jungleTilesInArea = 0;
                int xCheckArea = 30;
                bool canGenerateInLocation = true;

                // new Vector2 is used here since a lambda expression cannot capture a ref, out, or in parameter.
                bool nearbyOtherWorkshop = workshopPoints.Any(point => Vector2.Distance(point.ToVector2(), new Vector2(placementPositionX, placementPositionY)) < 200f);
                float totalTiles = (schematicSize.X + xCheckArea * 2) * schematicSize.Y;
                for (int x = placementPoint.X - xCheckArea; x < placementPoint.X + schematicSize.X + xCheckArea; x++)
                {
                    for (int y = placementPoint.Y; y < placementPoint.Y + schematicSize.Y; y++)
                    {
                        Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);
                        if (ShouldAvoidLocation(new Point(x, y)))
                            canGenerateInLocation = false;
                        if (tile.active())
                        {
                            if (tile.type == TileID.Mud || tile.type == TileID.JungleGrass)
                                jungleTilesInArea++;
                            activeTilesInArea++;
                        }
                    }
                }
                if (!canGenerateInLocation || nearbyOtherWorkshop || jungleTilesInArea < totalTiles * 0.4f)
                {
                    tries++;
                }
				else
				{
                    bool hasPlacedLogAndSchematic = false;
                    PlaceStructure(mapKey, new Point(placementPoint.X, placementPoint.Y), PlacementAnchorType.TopLeft, ref hasPlacedLogAndSchematic, new Action<Chest, int, bool>((chest, type, hasPlaced) => FillPlagueLaboratoryChest(chest, type, hasPlaced)));
                    break;
                }
            }
            while (tries <= 50000);
        }
        #endregion

        #region Planetoid Lab

        public static void FillPlanetoidLaboratoryChest(Chest chest, int type, bool hasPlacedLogAndSchematic)
        {
            int potionType = Utils.SelectRandom(WorldGen.genRand, ItemID.EndurancePotion, ItemID.GravitationPotion, ItemID.HeartreachPotion, ItemID.LifeforcePotion);

            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(ModContent.ItemType<DubiousPlating>(), WorldGen.genRand.Next(8, 14 + 1)),
                new ChestItem(ModContent.ItemType<MysteriousCircuitry>(), WorldGen.genRand.Next(7, 12 + 1)),
                new ChestItem(ItemID.HerbBag, WorldGen.genRand.Next(12, 17 + 1)),
                new ChestItem(ItemID.CorruptPlanterBox, WorldGen.genRand.Next(5, 9 + 1)),
                new ChestItem(ItemID.DayBloomPlanterBox, WorldGen.genRand.Next(5, 9 + 1)),
                new ChestItem(ItemID.FireBlossomPlanterBox, WorldGen.genRand.Next(5, 9 + 1)),
                new ChestItem(ItemID.MoonglowPlanterBox, WorldGen.genRand.Next(5, 9 + 1)),
                new ChestItem(ItemID.ShiverthornPlanterBox, WorldGen.genRand.Next(5, 9 + 1)),
                new ChestItem(ItemID.WaterleafPlanterBox, WorldGen.genRand.Next(5, 9 + 1)),
                new ChestItem(ItemID.Torch, WorldGen.genRand.Next(15, 29 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(5, 11 + 1)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(5, 7 + 1)),
                new ChestItem(ItemID.Bomb, WorldGen.genRand.Next(6, 7 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(3, 5 + 1)),
            };

			Mod thorium = ModLoader.GetMod("ThoriumMod");
            if (thorium != null)
                contents.Add(new ChestItem(thorium.ItemType("MarineKelpPlanterBox"), WorldGen.genRand.Next(5, 9 + 1)));

            if (!hasPlacedLogAndSchematic)
            {
                contents.Insert(0, new ChestItem(ModContent.ItemType<DraedonsLogPlanetoid>(), 1));
                contents.Insert(1, new ChestItem(ModContent.ItemType<EncryptedSchematic>(), 1));
            }
            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }
        #endregion
    }
}
