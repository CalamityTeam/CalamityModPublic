using CalamityMod.Schematics;
using static CalamityMod.Schematics.SchematicManager;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CalamityMod.World
{
    public class VernalPass
    {
        public static void PlaceVernalPass(StructureMap structures)
        {
            int tries = 0;
            string mapKey = VernalKey;

            int placementPositionX = WorldGen.genRand.Next(WorldGen.tLeft, WorldGen.tRight);
            int placementPositionY = WorldGen.tTop - 120;
            Point placementPoint = new Point(placementPositionX, placementPositionY);

            Vector2 schematicSize = new Vector2(TileMaps[mapKey].GetLength(0), TileMaps[mapKey].GetLength(1));

            bool firstItem = false;
            SchematicManager.PlaceSchematic(mapKey, new Point(placementPoint.X, placementPoint.Y), 
            SchematicAnchor.Center, ref firstItem, new Action<Chest, int, bool>(FillVernalPassChests));

            //add it as a protected structure
            structures.AddProtectedStructure(new Rectangle(placementPoint.X, placementPoint.Y, (int)schematicSize.X, (int)schematicSize.Y), 30);
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
