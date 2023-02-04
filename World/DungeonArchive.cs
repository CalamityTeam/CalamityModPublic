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
            int worldThird = Main.maxTilesX / 3;

            int dungeonArchiveColor = 0; //0 = blue, 1 = green, 2 = pink

            //start much higher above the top of hell so it doesnt get nuked by the crags generation, and so it wont generate too low
            for (int j = Main.maxTilesY - 380; j > 0; j--)
            {
                int i = 100;
                if (WorldGen.dungeonSide == 1)
                {
                    i = Main.maxTilesX - 100;
                }

                bool shouldContinue = true;
                bool placedArchive = false;
                int color = 0;

                while (shouldContinue)
                {
                    if (WorldGen.dungeonSide == 1)
                    {
                        i--;
                        if (i < Main.maxTilesX - worldThird)
                        {
                            shouldContinue = false;
                        }
                    }
                    else
                    {
                        i++;
                        if (i > worldThird)
                        {
                            shouldContinue = false;
                        }
                    }

                    Tile tile = Main.tile[i, j];
                    Tile tileUp1 = Main.tile[i, j - 1];
                    Tile tileUp2 = Main.tile[i, j - 2];
                    Tile tileUp3 = Main.tile[i, j - 3];
                    Tile tileUp4 = Main.tile[i, j - 4];
                    Tile tileUp5 = Main.tile[i, j - 5];

                    int[] DungeonWalls = { 7, 94, 95, 8, 98, 99, 9, 96, 97 };
                    //if (Main.tileDungeon[tile.TileType] && DungeonWalls.Contains(tileUp.WallType) && !tileUp.HasTile)

                    if (Main.tileDungeon[tile.TileType] && !tileUp1.HasTile && !tileUp2.HasTile && !tileUp3.HasTile && !tileUp4.HasTile && !tileUp5.HasTile)
                    {
                        //i += WorldGen.dungeonSide * -16;

                        //determine the archive brick color
                        if (tile.TileType == TileID.BlueDungeonBrick)
                            dungeonArchiveColor = 0;
                        else if (tile.TileType == TileID.GreenDungeonBrick)
                            dungeonArchiveColor = 1;
                        else if (tile.TileType == TileID.PinkDungeonBrick)
                            dungeonArchiveColor = 2;
                        
                        placedArchive = true;

                        break;
                    }
                }

                if (placedArchive)
                {
                    bool firstItem = false;

                    if (dungeonArchiveColor == 0)
                    {
                        SchematicManager.PlaceSchematic(SchematicManager.BlueArchiveKey, new Point(i, j), SchematicAnchor.TopCenter,
                        ref firstItem, new Action<Chest, int, bool>(FillArchiveChests));
                    }
                    if (dungeonArchiveColor == 1)
                    {
                        SchematicManager.PlaceSchematic(SchematicManager.GreenArchiveKey, new Point(i, j), SchematicAnchor.TopCenter, 
                        ref firstItem, new Action<Chest, int, bool>(FillArchiveChests));
                    }
                    if (dungeonArchiveColor == 2)
                    {
                        SchematicManager.PlaceSchematic(SchematicManager.PinkArchiveKey, new Point(i, j), SchematicAnchor.TopCenter, 
                        ref firstItem, new Action<Chest, int, bool>(FillArchiveChests));
                    }

                    break;
                }
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
            
            //this is normally not a good idea with separate items lists, but both lists are the same size so it is fine here
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