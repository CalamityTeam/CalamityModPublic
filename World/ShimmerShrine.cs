using System;
using System.Collections.Generic;
using CalamityMod.Schematics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;
using static CalamityMod.Schematics.SchematicManager;

namespace CalamityMod.World
{
    public class ShimmerShrine
    {
        public static void PlaceShimmerShrine(StructureMap structures)
        {
            string mapKey = ShimmerShrineKey;
            var schematic = TileMaps[mapKey];

            int placementPositionX = (int)GenVars.shimmerPosition.X;
            int placementPositionY = (int)Main.worldSurface - 300;
            int offset = 28;

            int width = schematic.GetLength(0);
            int height = schematic.GetLength(1);
            // If the point directly above the shimmer is blocked by something, try to move to the side a bit
            if (!structures.CanPlace(new Rectangle(placementPositionX - width, placementPositionY - height, width, height)))
            {
                int attempts = 0;
                while (attempts < 1000)
                {
                    attempts++;
                    placementPositionX = Main.rand.Next((int)GenVars.shimmerPosition.X - 100, (int)GenVars.shimmerPosition.X + 100);
                    if (structures.CanPlace(new Rectangle(placementPositionX - width, placementPositionY - height, width, height)))
                        break;
                }
                // If still not good, just give up and place it directly above the shimmer regardless
                placementPositionX = (int)GenVars.shimmerPosition.X;
            }

            while (!Main.tile[placementPositionX, placementPositionY].HasTile)
                placementPositionY++;

            Point placementPoint = new Point(placementPositionX, placementPositionY + offset);
            SchematicAnchor anchorType = SchematicAnchor.Center;

            bool place = true;
            PlaceSchematic(mapKey, placementPoint, anchorType, ref place, new Action<Chest, int, bool>(FillShimmerShrineChest));

            Rectangle protectionArea = CalamityUtils.GetSchematicProtectionArea(schematic, placementPoint, anchorType);
            CalamityUtils.AddProtectedStructure(protectionArea, 30);
        }

        public static void FillShimmerShrineChest(Chest chest, int Type, bool place)
        {
            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(ItemID.AngelStatue, 1),
                new ChestItem(WorldGen.genRand.NextBool() ? ItemID.LifeCrystal : ItemID.ManaCrystal, 1),
                new ChestItem(GenVars.gold == TileID.Gold ? ItemID.GoldBar : ItemID.PlatinumBar, WorldGen.genRand.Next(5, 16)),
                new ChestItem(ItemID.CanOfWorms, WorldGen.genRand.Next(3, 5)),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(5, 11)),
                new ChestItem(ItemID.LuckPotionGreater, WorldGen.genRand.Next(1, 3)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(2, 5)),
            };

            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }
    }
}
