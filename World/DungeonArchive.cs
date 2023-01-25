using CalamityMod.Schematics;
using static CalamityMod.Schematics.SchematicManager;
using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.WorldBuilding;
using Terraria.Utilities;
using Terraria.GameContent.Generation;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CalamityMod.World
{
    public class DungeonArchive
    {
        public static void PlaceArchive()
        {
            int archiveX = 0;
            int archiveY = 0;

            bool continueLooping = true;
            bool placedArchive = false;
            bool actuallyPlaced = false;
            bool foundValidPosition = false;
            int dungeonArchiveColor = 0; //0 = blue, 1 = green, 2 = pink

            for (int x = 20; x <= Main.maxTilesX - 20; x++)
            {
                for (int y = Main.maxTilesY - 200; y > (Main.maxTilesY / 2) + 100; y--)
                { 
                    Tile tile = Main.tile[x, y];
                    Tile tileUp = Main.tile[x, y - 1];
                    Tile tileDown = Main.tile[x, y + 1];

                    if (!tile.HasTile && !tileUp.HasTile && (tileDown.TileType == 41 || tileDown.TileType == 43 || tileDown.TileType == 44))
                    {
                        //blue brick walls
                        if (tile.WallType == 7 || tile.WallType == 94 || tile.WallType == 95)
                        {
                            dungeonArchiveColor = 0;
                            archiveX = x;
                            archiveY = y;

                            foundValidPosition = true;
                        }
                        //green brick walls
                        if (tile.WallType == 8 || tile.WallType == 98 || tile.WallType == 99)
                        {
                            dungeonArchiveColor = 1;
                            archiveX = x;
                            archiveY = y;

                            foundValidPosition = true;
                        }
                        //pink brick walls
                        if (tile.WallType == 9 || tile.WallType == 96 || tile.WallType == 97)
                        {
                            dungeonArchiveColor = 2;
                            archiveX = x;
                            archiveY = y;

                            foundValidPosition = true;
                        }
                    }
                }
            }

            if (foundValidPosition && !actuallyPlaced)
            {
                bool firstItem = false;

                if (dungeonArchiveColor == 0)
                {
                    SchematicManager.PlaceSchematic(SchematicManager.BlueArchiveKey, new Point(archiveX - 10, archiveY), SchematicAnchor.TopCenter,
                    ref firstItem, new Action<Chest, int, bool>(FillArchiveChests));
                }
                if (dungeonArchiveColor == 1)
                {
                    SchematicManager.PlaceSchematic(SchematicManager.GreenArchiveKey, new Point(archiveX - 10, archiveY), SchematicAnchor.TopCenter, 
                    ref firstItem, new Action<Chest, int, bool>(FillArchiveChests));
                }
                if (dungeonArchiveColor == 2)
                {
                    SchematicManager.PlaceSchematic(SchematicManager.PinkArchiveKey, new Point(archiveX - 10, archiveY), SchematicAnchor.TopCenter, 
                    ref firstItem, new Action<Chest, int, bool>(FillArchiveChests));
                }

                actuallyPlaced = true;
            }
        }

        public static void FillArchiveChests(Chest chest, int Type, bool firstItem)
        {
            int potionType1 = Utils.SelectRandom(WorldGen.genRand, ItemID.HunterPotion, ItemID.IronskinPotion);
            int potionType2 = Utils.SelectRandom(WorldGen.genRand, ItemID.ShinePotion, ItemID.SwiftnessPotion);
            List<ChestItem> contents1 = new List<ChestItem>()
            {
                new ChestItem(ItemID.ShadowKey, 1),
                new ChestItem(ItemID.HealingPotion, WorldGen.genRand.Next(10, 20)),
                new ChestItem(ItemID.ManaPotion, WorldGen.genRand.Next(10, 20)),
                new ChestItem(potionType1, WorldGen.genRand.Next(4, 8)),
                new ChestItem(potionType2, WorldGen.genRand.Next(4, 8)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(5, 10)),
            };

            List<ChestItem> contents2 = new List<ChestItem>()
            {
                new ChestItem(ItemID.SpellTome, WorldGen.genRand.Next(2, 3)),
                new ChestItem(ItemID.Book, WorldGen.genRand.Next(12, 25)),
                new ChestItem(ItemID.TallyCounter, 1),
                new ChestItem(potionType1, WorldGen.genRand.Next(4, 8)),
                new ChestItem(potionType2, WorldGen.genRand.Next(4, 8)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(5, 10)),
            };
            
            for (int i = 0; i < contents1.Count; i++)
            {
                if (!firstItem)
                {
                    chest.item[i].SetDefaults(contents1[i].Type);
                    chest.item[i].stack = contents1[i].Stack;
                }
                else
                {
                    chest.item[i].SetDefaults(contents2[i].Type);
                    chest.item[i].stack = contents2[i].Stack;
                }
            }
        }
    }
}