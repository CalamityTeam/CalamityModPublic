using System;
using System.Collections.Generic;
using CalamityMod.Schematics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static CalamityMod.Schematics.SchematicManager;

namespace CalamityMod.World
{
    public class MechanicShed
    {
        public static void PlaceMechanicShed(StructureMap structures)
        {
            string mapKey = MechanicShedKey;
            var schematic = TileMaps[mapKey];

            int placementPositionX = WorldGen.genRand.Next(GenVars.snowOriginLeft + 100, GenVars.snowOriginRight - 100);
            int placementPositionY = (int)Main.worldSurface - (Main.maxTilesY / 6);

            bool foundValidGround = false;
            int attempts = 0;
            while (!foundValidGround && attempts++ < 100000)
            {
                while (!WorldGen.SolidTile(placementPositionX, placementPositionY) && placementPositionY <= Main.worldSurface)
				{
                    placementPositionY++;
				}

                if (Main.tile[placementPositionX, placementPositionY].HasTile || Main.tile[placementPositionX, placementPositionY].WallType > 0)
				{
                    foundValidGround = true;
				}
            }

            Point placementPoint = new Point(placementPositionX, placementPositionY + 5);

            Vector2 schematicSize = new Vector2(schematic.GetLength(0), schematic.GetLength(1));
            SchematicAnchor anchorType = SchematicAnchor.BottomCenter;

            bool place = true;
            PlaceSchematic(mapKey, placementPoint, anchorType, ref place, new Action<Chest, int, bool>(FillMechanicChest));

            Rectangle protectionArea = CalamityUtils.GetSchematicProtectionArea(schematic, placementPoint, anchorType);
            CalamityUtils.AddProtectedStructure(protectionArea, 30);
        }

        public static void FillMechanicChest(Chest chest, int Type, bool place)
        {
            int gizmoGoobabGadgets = Utils.SelectRandom(WorldGen.genRand, ItemID.BrickLayer, ItemID.ExtendoGrip, ItemID.PaintSprayer, ItemID.PortableCementMixer);
            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(ItemID.Toolbox, 1),
                new ChestItem(ItemID.ActuationAccessory, 1),
                new ChestItem(gizmoGoobabGadgets, 1),
                new ChestItem(ItemID.BuilderPotion, WorldGen.genRand.Next(1, 3)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(1, 3)),
            };
            
            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }
    }
}
