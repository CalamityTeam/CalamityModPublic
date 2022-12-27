using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.LabFinders;
using CalamityMod.Schematics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

using static CalamityMod.Schematics.SchematicManager;
using CalamityMod.Items.Potions.Alcohol;

namespace CalamityMod.World
{
    // TODO -- This can be made into a ModSystem with simple OnModLoad and Unload hooks.
    public static class DraedonStructures
    {
        public const int HellVerticalAvoidance = 100;

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
            if (tile.TileType == TileID.Crimstone ||
                tile.WallType == WallID.CrimstoneUnsafe ||
                tile.TileType == TileID.Ebonstone ||
                tile.WallType == WallID.EbonstoneUnsafe)
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
            float rng = WorldGen.genRand.NextFloat();
            
            //Adds a Lab Seeking Mechanism at a 50% chance, or any of the 5 Bio Lab Seeking Mechanisms at a 10% chance each
            if(rng < 0.5f)
                contents.Insert(0, new ChestItem(ModContent.ItemType<LabSeekingMechanism>(), 1));
            else if(rng < 0.6f)
                contents.Insert(0, new ChestItem(ModContent.ItemType<CyanSeekingMechanism>(), 1));
            else if(rng < 0.7f)
                contents.Insert(0, new ChestItem(ModContent.ItemType<GreenSeekingMechanism>(), 1));
            else if (rng < 0.8f)
                contents.Insert(0, new ChestItem(ModContent.ItemType<PurpleSeekingMechanism>(), 1));
            else if (rng < 0.9f)
                contents.Insert(0, new ChestItem(ModContent.ItemType<RedSeekingMechanism>(), 1));
            else
                contents.Insert(0, new ChestItem(ModContent.ItemType<YellowSeekingMechanism>(), 1));

            //Add suspicious scrap into the chest rarely. Chance depends on the world size
            int probability = Main.maxTilesX <= 4200f ? 0 : Main.maxTilesX >= 8400f ? 2 : 1;
            if (Main.rand.Next(probability) == 0f)
                contents.Insert(2, new ChestItem(ModContent.ItemType<SuspiciousScrap>(), 1));

            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }
        public static void PlaceWorkshop(out Point placementPoint, List<Point> workshopPoints, StructureMap structures)
        {
            int tries = 0;
            string mapKey = RustedWorkshopKey;

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

                        if (tile.HasTile)
                            activeTilesInArea++;
                    }
                }
                if (!canGenerateInLocation || nearbyOtherWorkshop || activeTilesInArea / totalTiles > 0.35f || !structures.CanPlace(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y)))
                    tries++;
                else
                {
                    bool _ = true;
                    PlaceSchematic(mapKey, new Point(placementPoint.X, placementPoint.Y), SchematicAnchor.TopLeft, ref _, new Action<Chest>(FillWorkshopChest));
                    structures.AddProtectedStructure(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y), 4);
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
                new ChestItem(ModContent.ItemType<SuspiciousScrap>(), 1),
                new ChestItem(ItemID.Torch, WorldGen.genRand.Next(20, 40 + 1)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(8, 16 + 1)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(7, 10 + 1)),
                new ChestItem(ItemID.Dynamite, WorldGen.genRand.Next(4, 6 + 1)),
                new ChestItem(potionType, WorldGen.genRand.Next(4, 7 + 1)),
            };
            float rng = WorldGen.genRand.NextFloat();

            //Adds a Lab Seeking Mechanism at a 50% chance, or any of the 5 Bio Lab Seeking Mechanisms at a 10% chance each
            if (rng < 0.5f)
                contents.Insert(0, new ChestItem(ModContent.ItemType<LabSeekingMechanism>(), 1));
            else if (rng < 0.6f)
                contents.Insert(0, new ChestItem(ModContent.ItemType<CyanSeekingMechanism>(), 1));
            else if (rng < 0.7f)
                contents.Insert(0, new ChestItem(ModContent.ItemType<GreenSeekingMechanism>(), 1));
            else if (rng < 0.8f)
                contents.Insert(0, new ChestItem(ModContent.ItemType<PurpleSeekingMechanism>(), 1));
            else if (rng < 0.9f)
                contents.Insert(0, new ChestItem(ModContent.ItemType<RedSeekingMechanism>(), 1));
            else
                contents.Insert(0, new ChestItem(ModContent.ItemType<YellowSeekingMechanism>(), 1));

            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }
        public static void PlaceResearchFacility(out Point placementPoint, List<Point> workshopPoints, StructureMap structures)
        {
            int tries = 0;
            string mapKey = ResearchOutpostKey;

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

                        if (tile.HasTile)
                            activeTilesInArea++;
                    }
                }
                if (!canGenerateInLocation || nearbyOtherWorkshop || activeTilesInArea / totalTiles > 0.35f || !structures.CanPlace(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y)))
                    tries++;
                else
                {
                    bool _ = true;
                    PlaceSchematic(mapKey, new Point(placementPoint.X, placementPoint.Y), SchematicAnchor.TopLeft, ref _, new Action<Chest>(FillLaboratoryChest));
                    structures.AddProtectedStructure(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y), 4);
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

            //Adds the Ice Seeking Mechanism
            contents.Insert(0, new ChestItem(ModContent.ItemType<YellowSeekingMechanism>(), 1));

            if (!hasPlacedMurasama)
            {
                contents.Insert(0, new ChestItem(ModContent.ItemType<DraedonsLogHell>(), 1));
                contents.Insert(1, new ChestItem(ModContent.ItemType<Murasama>(), 1));
                contents.Insert(2, new ChestItem(ModContent.ItemType<EncryptedSchematicHell>(), 1));
            }
            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }

        public static void PlaceHellLab(out Point placementPoint, List<Point> workshopPoints, StructureMap structures)
        {
            int tries = 0;
            string mapKey = HellLabKey;
            PilePlacementMaps.TryGetValue(mapKey, out PilePlacementFunction pilePlacementFunction);
            SchematicMetaTile[,] schematic = TileMaps[mapKey];

            do
            {
                int underworldTop = Main.maxTilesY - 200;
                int placementPositionX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.82), (int)(Main.maxTilesX * 0.925));
                int placementPositionY = WorldGen.genRand.Next(Main.maxTilesY - 150, Main.maxTilesY - 125);

                placementPoint = new Point(placementPositionX, placementPositionY);
                Vector2 schematicSize = new Vector2(schematic.GetLength(0), schematic.GetLength(1));
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
                if (!canGenerateInLocation || !structures.CanPlace(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y)))
                {
                    tries++;
                }
                else
                {
                    bool hasPlacedMurasama = false;
                    PlaceSchematic(mapKey, new Point(placementPoint.X, placementPoint.Y), SchematicAnchor.TopLeft, ref hasPlacedMurasama, new Action<Chest, int, bool>(FillHellLaboratoryChest));
                    structures.AddProtectedStructure(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y), 4);
                    CalamityWorld.HellLabCenter = placementPoint.ToWorldCoordinates() + new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1)) * 8f;
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

            //Adds the Planetoid Seeking Mechanism
            contents.Insert(0, new ChestItem(ModContent.ItemType<PurpleSeekingMechanism>(), 1));

            if (!hasPlacedLogAndSchematic)
            {
                contents.Insert(0, new ChestItem(ModContent.ItemType<DraedonsLogSunkenSea>(), 1));
                contents.Insert(1, new ChestItem(ModContent.ItemType<EncryptedSchematicSunkenSea>(), 1));
            }
            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }

        public static void PlaceSunkenSeaLab(out Point placementPoint, List<Point> workshopPoints, StructureMap structures)
        {
            int tries = 0;
            string mapKey = SunkenSeaLabKey;
            PilePlacementMaps.TryGetValue(mapKey, out PilePlacementFunction pilePlacementFunction);
            SchematicMetaTile[,] schematic = TileMaps[mapKey];
            int labWidth = schematic.GetLength(0);
            int labHeight = schematic.GetLength(1);

            do
            {
                // Pick a location based on the Underground Desert, because the Sunken Sea is based on the Underground Desert
                Rectangle ugDesert = WorldGen.UndergroundDesertLocation;
                int placementPositionX = -1;

                // 50% chance to be on either the left or the right.
                // If it's on the right then shove it left because all schematics are placed based on their top left corner.
                if (WorldGen.genRand.NextBool())
                    placementPositionX = WorldGen.genRand.Next(ugDesert.Left - 20, ugDesert.Left + 10);
                else
                    placementPositionX = WorldGen.genRand.Next(ugDesert.Right - 10, ugDesert.Right + 20) - labWidth;

                // Somewhere in the middle third of the Sunken Sea, which itself is in the lower half of the Underground Desert
                int sunkenSeaHeight = ugDesert.Height / 4;
                int placementPositionY = (int)(ugDesert.Center.Y + Main.rand.NextFloat(0.33f, 0.67f) * sunkenSeaHeight) - labHeight;

                placementPoint = new Point(placementPositionX, placementPositionY);
                Vector2 schematicSize = new Vector2(schematic.GetLength(0), schematic.GetLength(1));
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
                if (!shouldAvoidArea && structures.CanPlace(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y)))
                    break;
            }
            while (tries < 50000);

            bool hasPlacedLogAndSchematic = false;
            PlaceSchematic(mapKey, new Point(placementPoint.X, placementPoint.Y), SchematicAnchor.TopLeft, ref hasPlacedLogAndSchematic, new Action<Chest, int, bool>(FillSunkenSeaLaboratoryChest));
            structures.AddProtectedStructure(new Rectangle(placementPoint.X, placementPoint.Y, TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1)), 4);
            CalamityWorld.SunkenSeaLabCenter = placementPoint.ToWorldCoordinates() + new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1)) * 8f;
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

            //Adds the Base Seeking Mechanism
            contents.Insert(0, new ChestItem(ModContent.ItemType<LabSeekingMechanism>(), 1));

            if (!hasPlacedLogAndSchematic)
            {
                contents.Insert(0, new ChestItem(ModContent.ItemType<DraedonsLogSnowBiome>(), 1));
                contents.Insert(1, new ChestItem(ModContent.ItemType<EncryptedSchematicIce>(), 1));
            }
            // If it's a frozen chest, add Arctic Diving Gear to it.
            if (type == TileID.Containers)
                contents.Insert(0, new ChestItem(ItemID.ArcticDivingGear, 1));
            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }

        public static void PlaceIceLab(out Point placementPoint, List<Point> workshopPoints, StructureMap structures)
        {
            int tries = 0;
            string mapKey = IceLabKey;
            PilePlacementMaps.TryGetValue(mapKey, out PilePlacementFunction pilePlacementFunction);
            SchematicMetaTile[,] schematic = TileMaps[mapKey];

            do
            {
                int underworldTop = Main.maxTilesY - 200;
                int placementPositionX = WorldGen.genRand.Next(120, Main.maxTilesX - 120);
                int placementPositionY = WorldGen.genRand.Next((int)Main.worldSurface + 160, underworldTop - HellVerticalAvoidance);

                placementPoint = new Point(placementPositionX, placementPositionY);
                Vector2 schematicSize = new Vector2(schematic.GetLength(0), schematic.GetLength(1));
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
                        if (tile.HasTile)
                        {
                            if (tile.TileType == TileID.SnowBlock || tile.TileType == TileID.IceBlock)
                                iceTilesInArea++;
                            activeTilesInArea++;
                        }
                        if (ShouldAvoidLocation(new Point(x, y)))
                            canGenerateInLocation = false;
                    }
                }
                if (!canGenerateInLocation || nearbyOtherWorkshop || iceTilesInArea < totalTiles * 0.35f || !structures.CanPlace(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y)))
                    tries++;
                else
                {
                    bool hasPlacedLogAndSchematic = false;
                    PlaceSchematic(mapKey, new Point(placementPoint.X, placementPoint.Y), SchematicAnchor.TopLeft, ref hasPlacedLogAndSchematic, new Action<Chest, int, bool>(FillIceLaboratoryChest));
                    structures.AddProtectedStructure(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y), 4);
                    CalamityWorld.IceLabCenter = placementPoint.ToWorldCoordinates() + new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1)) * 8f;
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

            //Adds the Hell Seeking Mechanism
            contents.Insert(0, new ChestItem(ModContent.ItemType<RedSeekingMechanism>(), 1));

            if (!hasPlacedLogAndSchematic)
            {
                contents.Insert(0, new ChestItem(ModContent.ItemType<DraedonsLogJungle>(), 1));
                contents.Insert(1, new ChestItem(ModContent.ItemType<EncryptedSchematicJungle>(), 1));
            }
            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }

        public static void PlacePlagueLab(out Point placementPoint, List<Point> workshopPoints, StructureMap structures)
        {
            int tries = 0;
            string mapKey = PlagueLabKey;
            PilePlacementMaps.TryGetValue(mapKey, out PilePlacementFunction pilePlacementFunction);
            SchematicMetaTile[,] schematic = TileMaps[mapKey];

            do
            {
                int underworldTop = Main.maxTilesY - 200;
                int placementPositionX = WorldGen.genRand.Next(120, Main.maxTilesX - 120);
                int placementPositionY = WorldGen.genRand.Next((int)Main.worldSurface + 160, underworldTop - HellVerticalAvoidance);

                placementPoint = new Point(placementPositionX, placementPositionY);
                Vector2 schematicSize = new Vector2(schematic.GetLength(0), schematic.GetLength(1));
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
                        if (tile.HasTile)
                        {
                            if (tile.TileType == TileID.Mud || tile.TileType == TileID.JungleGrass)
                                jungleTilesInArea++;
                            activeTilesInArea++;
                        }
                    }
                }
                if (!canGenerateInLocation || nearbyOtherWorkshop || jungleTilesInArea < totalTiles * 0.4f || !structures.CanPlace(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y)))
                {
                    tries++;
                }
                else
                {
                    bool hasPlacedLogAndSchematic = false;
                    PlaceSchematic(mapKey, new Point(placementPoint.X, placementPoint.Y), SchematicAnchor.TopLeft, ref hasPlacedLogAndSchematic, new Action<Chest, int, bool>(FillPlagueLaboratoryChest));
                    structures.AddProtectedStructure(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y), 4);
                    CalamityWorld.JungleLabCenter = placementPoint.ToWorldCoordinates() + new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1)) * 8f;
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
                new ChestItem(ItemID.DayBloomPlanterBox, WorldGen.genRand.Next(5, 9 + 1)),
                new ChestItem(ItemID.BlinkrootPlanterBox, WorldGen.genRand.Next(5, 9 + 1)),
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
            if (!WorldGen.crimson)
                contents.Insert(8, new ChestItem(ItemID.CorruptPlanterBox, WorldGen.genRand.Next(5, 9 + 1)));
            else
                contents.Insert(8, new ChestItem(ItemID.CrimsonPlanterBox, WorldGen.genRand.Next(5, 9 + 1)));

            Mod thorium = CalamityMod.Instance.thorium;
            if (thorium != null)
			{
				try
				{
					contents.Add(new ChestItem(thorium.Find<ModItem>("MarineKelpPlanterBox").Type, WorldGen.genRand.Next(5, 9 + 1)));
				}
				catch
				{
					CalamityMod.Instance.Logger.Debug("One of the items in this file got renamed internally. Please report this in the #bugs-read-pins channel of the official Calamity discord server.");
				}
			}

            //Adds the Jungle Seeking Mechanism
            contents.Insert(0, new ChestItem(ModContent.ItemType<GreenSeekingMechanism>(), 1));

            if (!hasPlacedLogAndSchematic)
            {
                contents.Insert(0, new ChestItem(ModContent.ItemType<DraedonsLogPlanetoid>(), 1));
                contents.Insert(1, new ChestItem(ModContent.ItemType<EncryptedSchematicPlanetoid>(), 1));
                contents.Insert(2, new ChestItem(ModContent.ItemType<PlasmaDriveCore>(), 1));
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
