using CalamityMod.Items.Materials;
using CalamityMod.Schematics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
        public static void FillWorkshopChest(Chest chest)
        {
            int potionType = Utils.SelectRandom(Main.rand, ItemID.EndurancePotion, ItemID.GravitationPotion, ItemID.HeartreachPotion, ItemID.LifeforcePotion);
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
        public static void FillLaboratoryChest(Chest chest)
        {
            int potionType = Utils.SelectRandom(Main.rand, ItemID.EndurancePotion, ItemID.GravitationPotion, ItemID.HeartreachPotion, ItemID.LifeforcePotion);
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
        public static void PlaceWorkshop(out Point placementPoint, List<Point> workshopPoints)
        {
            int tries = 0;

        TryAgain:
            int underworldTop = Main.maxTilesY - 200;
            int placementPositionX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f));
            int placementPositionY = WorldGen.genRand.Next(underworldTop - 550, underworldTop - 50);

            placementPoint = new Point(placementPositionX, placementPositionY);
            Vector2 schematicSize = new Vector2(SchematicLoader.TileMaps["Workshop"].GetLength(0), SchematicLoader.TileMaps["Workshop"].GetLength(1));
            int activeTilesInArea = 0;
            int xCheckArea = 40;
            bool anyDungeonBricks = false;
            bool anyLihzardBricks = false;

            // new Vector2 is used here since a lambda expression cannot capture a ref, out, or in parameter.
            bool nearbyOtherWorkshop = workshopPoints.Any(point => Vector2.Distance(point.ToVector2(), new Vector2(placementPositionX, placementPositionY)) < 240f);
            float totalTiles = (schematicSize.X + xCheckArea * 2) * schematicSize.Y;
            for (int x = placementPoint.X - xCheckArea; x < placementPoint.X + schematicSize.X + xCheckArea; x++)
            {
                for (int y = placementPoint.Y; y < placementPoint.Y + schematicSize.Y; y++)
                {
                    Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);
                    if (tile.active())
                    {
                        if (tile.type == TileID.BlueDungeonBrick ||
                            tile.type == TileID.GreenDungeonBrick ||
                            tile.type == TileID.PinkDungeonBrick)
                        {
                            anyDungeonBricks = true;
                        }
                        if (tile.type == TileID.LihzahrdBrick ||
                            tile.wall == WallID.LihzahrdBrickUnsafe)
                        {
                            anyLihzardBricks = true;
                        }
                        activeTilesInArea++;
                    }
                }
            }
            if (anyDungeonBricks || anyLihzardBricks || nearbyOtherWorkshop || activeTilesInArea / totalTiles > 0.35f)
            {
                tries++;
                if (tries > 2500)
                    return;
                goto TryAgain; // Try again elsewhere if the correct conditions are not met. (Yes, I'm using a goto. Please don't kill me)
            }
            SchematicPlacementHelpers.PlaceDraedonStructure("Workshop", new Point(placementPoint.X, placementPoint.Y), SchematicPlacementHelpers.PlacementAnchorType.TopLeft, FillWorkshopChest);
        }
        public static void PlacePlagueLab(out Point placementPoint, List<Point> workshopPoints)
        {
            int tries = 0;

        TryAgain:
            int underworldTop = Main.maxTilesY - 200;
            int placementPositionX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.15f), (int)(Main.maxTilesX * 0.85f));
            int placementPositionY = WorldGen.genRand.Next(underworldTop - 400, underworldTop - 50);

            placementPoint = new Point(placementPositionX, placementPositionY);
            Vector2 schematicSize = new Vector2(SchematicLoader.TileMaps["Plague Research Facility"].GetLength(0), SchematicLoader.TileMaps["Plague Research Facility"].GetLength(1));
            int activeTilesInArea = 0;
            int xCheckArea = 30;
            bool anyDungeonBricks = false;
            bool anyLihzardBricks = false;

            // new Vector2 is used here since a lambda expression cannot capture a ref, out, or in parameter.
            bool nearbyOtherWorkshop = workshopPoints.Any(point => Vector2.Distance(point.ToVector2(), new Vector2(placementPositionX, placementPositionY)) < 240f);
            float totalTiles = (schematicSize.X + xCheckArea * 2) * schematicSize.Y;
            for (int x = placementPoint.X - xCheckArea; x < placementPoint.X + schematicSize.X + xCheckArea; x++)
            {
                for (int y = placementPoint.Y; y < placementPoint.Y + schematicSize.Y; y++)
                {
                    Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);
                    if (tile.active())
                    {
                        if (tile.type == TileID.BlueDungeonBrick ||
                            tile.type == TileID.GreenDungeonBrick ||
                            tile.type == TileID.PinkDungeonBrick)
                        {
                            anyDungeonBricks = true;
                        }
                        if (tile.type == TileID.LihzahrdBrick ||
                            tile.wall == WallID.LihzahrdBrickUnsafe)
                        {
                            anyLihzardBricks = true;
                        }
                        activeTilesInArea++;
                    }
                }
            }
            if (anyLihzardBricks || anyDungeonBricks || nearbyOtherWorkshop || activeTilesInArea / totalTiles > 0.3f)
            {
                tries++;
                if (tries > 6000)
                    return;
                goto TryAgain; // Try again elsewhere if the correct conditions are not met. (Yes, I'm using a goto. Please don't kill me)
            }
            SchematicPlacementHelpers.PlaceDraedonStructure("Plague Research Facility", new Point(placementPoint.X, placementPoint.Y), SchematicPlacementHelpers.PlacementAnchorType.TopLeft, FillLaboratoryChest);
        }
    }
}
