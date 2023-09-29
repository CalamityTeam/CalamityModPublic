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
    public class VernalPass
    {
        public static void PlaceVernalPass(StructureMap structures)
        {
            string mapKey = VernalKey;
            var schematic = TileMaps[mapKey];

            int placementPositionX = WorldGen.genRand.Next(GenVars.tLeft, GenVars.tRight);
            int placementPositionY = GenVars.tTop < Main.rockLayer - 10 ? GenVars.tBottom + 120 : GenVars.tTop - 120;
            Point placementPoint = new Point(placementPositionX, placementPositionY);

            Vector2 schematicSize = new Vector2(schematic.GetLength(0), schematic.GetLength(1));
            SchematicAnchor anchorType = SchematicAnchor.Center;

            bool firstItem = false;
            PlaceSchematic(mapKey, placementPoint, anchorType, ref firstItem, new Action<Chest, int, bool>(FillVernalPassChests));

            // Add the Vernal Pass as a protected structure.
            Rectangle protectionArea = CalamityUtils.GetSchematicProtectionArea(schematic, placementPoint, anchorType);
            CalamityUtils.AddProtectedStructure(protectionArea, 30);
        }

        public static void FillVernalPassChests(Chest chest, int Type, bool firstItem)
        {
            int mainItem = Utils.SelectRandom(WorldGen.genRand, ItemID.StaffofRegrowth, ItemID.AnkletoftheWind, ItemID.FeralClaws);

            int bars = Utils.SelectRandom(WorldGen.genRand, ItemID.GoldBar, ItemID.PlatinumBar);
            int potionType = Utils.SelectRandom(WorldGen.genRand, ItemID.ThornsPotion, ItemID.BattlePotion, ItemID.ShinePotion, ItemID.HunterPotion);
            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(bars, WorldGen.genRand.Next(4, 7)),
                new ChestItem(ItemID.JungleSpores, WorldGen.genRand.Next(4, 8)),
                new ChestItem(ItemID.Stinger, WorldGen.genRand.Next(2, 5)),
                new ChestItem(ItemID.JungleTorch, WorldGen.genRand.Next(2, 5)),
                new ChestItem(potionType, WorldGen.genRand.Next(1, 4)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(1, 3)),
            };

            if (!firstItem)
            {
                contents.Insert(0, new ChestItem(ModContent.ItemType<Items.Tools.FellerofEvergreens>(), 1));
            }
            else
            {
                contents.RemoveAt(0);
                contents.Insert(0, new ChestItem(mainItem, 1));
                contents.Insert(1, new ChestItem(bars, WorldGen.genRand.Next(4, 7)));
            }
            
            for (int i = 0; i < contents.Count; i++)
            {
                chest.item[i].SetDefaults(contents[i].Type);
                chest.item[i].stack = contents[i].Stack;
            }
        }
    }
}
