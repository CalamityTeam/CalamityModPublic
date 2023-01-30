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

            int heightLimit = (Main.maxTilesY / 2) + (Main.maxTilesY / 7);

            for (int x = 5; x <= Main.maxTilesX - 5; x++)
            {
                for (int y = Main.maxTilesY - 5; y >= heightLimit; y--)
                { 
                    Tile tile = Main.tile[x, y];
                    Tile tileUp = Main.tile[x, y - 1];
                    Tile tileDown = Main.tile[x, y + 1];

                    if ((tile.TileType == 41 || tile.TileType == 43 || tile.TileType == 44 || tile.TileType == 48) && !tileUp.HasTile)
                    {
                        //blue brick walls
                        if (tileUp.WallType == 7 || tileUp.WallType == 94 || tileUp.WallType == 95)
                        {
                            dungeonArchiveColor = 0;
                            archiveX = x;
                            archiveY = y;

                            foundValidPosition = true;
                        }
                        //green brick walls
                        if (tileUp.WallType == 8 || tileUp.WallType == 98 || tileUp.WallType == 99)
                        {
                            dungeonArchiveColor = 1;
                            archiveX = x;
                            archiveY = y;

                            foundValidPosition = true;
                        }
                        //pink brick walls
                        if (tileUp.WallType == 9 || tileUp.WallType == 96 || tileUp.WallType == 97)
                        {
                            dungeonArchiveColor = 2;
                            archiveX = x;
                            archiveY = y;

                            foundValidPosition = true;
                        }
                    }

                    //in order to make sure the archive places at the very bottom of the dungeon:
                    //loop upward a certain amount, and if it doesnt place reset everything and increase the check height before the loop ends
                    //this way it will keep checking upward so it places nicely and doesnt destroy other parts of the dungeon (or at least not as much)
                    if (x >= Main.maxTilesX - 5 && y <= heightLimit + 5 && !foundValidPosition)
                    {
                        x = 5;
                        y = Main.maxTilesY - 5;

                        heightLimit = heightLimit - 2;
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