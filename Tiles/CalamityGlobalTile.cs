using System;
using System.Collections.Generic;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Tools;
using CalamityMod.Items.VanillaArmorChanges;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.SunkenSea;
using CalamityMod.Tiles.SunkenSea.Ambient;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Tiles
{
    public class CalamityGlobalTile : GlobalTile
    {
        public static List<int> GrowthTiles = new List<int>()
        {
            TileType<SeaPrism>(),
            TileType<AbyssGravel>(),
            TileType<PyreMantle>(),
            TileType<Navystone>(),
            TileType<Shellstone>(),
            TileType<EutrophicSand>(),
            TileType<PolypSand>(),
            TileType<VolcanicSand>(),
            TileType<HardenedEutrophicSand>(),
            TileType<Dunesand>(),
            TileType<Limestone>(),
            TileType<LimestoneCobble>(),
            TileType<Voidstone>()
        };

        public override void SetStaticDefaults()
        {
            Main.tileSpelunker[TileID.LunarOre] = true;
            Main.tileOreFinderPriority[TileID.LunarOre] = 900;

            // Allow Queen Bee larvae to be protected by Guide to Environmental Protection
            // Yes this naming is backwards. Do not blame me!
            TileID.Sets.TileCutIgnore.IgnoreDontHurtNature[TileID.Larva] = true;
        }

        public override void PreShakeTree(int x, int y, TreeTypes treeType)
        {
            // All trees have a 33% chance to drop extra fruit when using Feller of Evergreens
            Vector2 worldPosition = new Vector2(x, y).ToWorldCoordinates();
            Player nearestPlayer = Main.player[Player.FindClosest(worldPosition, 16, 16)];
            if (nearestPlayer.active && nearestPlayer.HeldItem.type == ItemType<FellerofEvergreens>() && WorldGen.genRand.NextBool(3))
            {
                int treeDropItemType = 0;
                switch (treeType)
                {
                    case TreeTypes.Forest:

                        switch (WorldGen.genRand.Next(5))
                        {
                            case 0:
                                treeDropItemType = ItemID.Apple;
                                break;
                            case 1:
                                treeDropItemType = ItemID.Apricot;
                                break;
                            case 2:
                                treeDropItemType = ItemID.Peach;
                                break;
                            case 3:
                                treeDropItemType = ItemID.Grapefruit;
                                break;
                            default:
                                treeDropItemType = ItemID.Lemon;
                                break;
                        }
                        break;

                    case TreeTypes.Snow:
                        treeDropItemType = WorldGen.genRand.NextBool() ? ItemID.Cherry : ItemID.Plum;
                        break;

                    case TreeTypes.Jungle:
                        treeDropItemType = WorldGen.genRand.NextBool() ? ItemID.Mango : ItemID.Pineapple;
                        break;

                    case TreeTypes.Palm:
                        if (WorldGen.IsPalmOasisTree(x))
                            treeDropItemType = WorldGen.genRand.NextBool() ? ItemID.Banana : ItemID.Coconut;
                        break;

                    case TreeTypes.PalmCorrupt:
                        if (WorldGen.genRand.NextBool())
                            treeDropItemType = WorldGen.genRand.NextBool() ? ItemID.BlackCurrant : ItemID.Elderberry;
                        else if (WorldGen.IsPalmOasisTree(x))
                            treeDropItemType = WorldGen.genRand.NextBool() ? ItemID.Banana : ItemID.Coconut;
                        else
                            treeDropItemType = WorldGen.genRand.NextBool() ? ItemID.BlackCurrant : ItemID.Elderberry;
                        break;

                    case TreeTypes.Corrupt:
                        treeDropItemType = WorldGen.genRand.NextBool() ? ItemID.BlackCurrant : ItemID.Elderberry;
                        break;

                    case TreeTypes.PalmHallowed:
                        if (WorldGen.genRand.NextBool())
                            treeDropItemType = WorldGen.genRand.NextBool() ? ItemID.Dragonfruit : ItemID.Starfruit;
                        else if (WorldGen.IsPalmOasisTree(x))
                            treeDropItemType = WorldGen.genRand.NextBool() ? ItemID.Banana : ItemID.Coconut;
                        else
                            treeDropItemType = WorldGen.genRand.NextBool() ? ItemID.Dragonfruit : ItemID.Starfruit;
                        break;

                    case TreeTypes.Hallowed:
                        treeDropItemType = WorldGen.genRand.NextBool() ? ItemID.Dragonfruit : ItemID.Starfruit;
                        break;

                    case TreeTypes.PalmCrimson:
                        if (WorldGen.genRand.NextBool())
                            treeDropItemType = WorldGen.genRand.NextBool() ? ItemID.BloodOrange : ItemID.Rambutan;
                        else if (WorldGen.IsPalmOasisTree(x))
                            treeDropItemType = WorldGen.genRand.NextBool() ? ItemID.Banana : ItemID.Coconut;
                        else
                            treeDropItemType = WorldGen.genRand.NextBool() ? ItemID.BloodOrange : ItemID.Rambutan;
                        break;

                    case TreeTypes.Crimson:
                        treeDropItemType = WorldGen.genRand.NextBool() ? ItemID.BloodOrange : ItemID.Rambutan;
                        break;

                    case TreeTypes.Ash:
                        treeDropItemType = WorldGen.genRand.NextBool() ? ItemID.Pomegranate : ItemID.SpicyPepper;
                        break;

                    default:
                        break;
                }

                if (treeDropItemType != 0)
                    Item.NewItem(WorldGen.GetItemSource_FromTreeShake(x, y), x * 16, y * 16, 16, 16, treeDropItemType);
            }
        }

        public override bool ShakeTree(int x, int y, TreeTypes treeType)
        {
            // Ha-pu Fruit @ 1% (6.67% during Valentines)
            // This *will* work on modded trees as well
            if (WorldGen.genRand.NextBool(100) || (DateTime.Now.Month == 2 && DateTime.Now.Day == 14 && WorldGen.genRand.NextBool(15)))
            {
                Item.NewItem(WorldGen.GetItemSource_FromTreeShake(x, y), x * 16, y * 16, 16, 16, ItemType<HapuFruit>());
                return true;
            }

            return base.ShakeTree(x, y, treeType);
        }

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Tile tile = Main.tile[i, j];

            // Helper function to shatter crystals attached to neighboring solid tiles.
            void CheckShatterCrystal(int xPos, int yPos, bool dontShatter)
            {
                if (xPos < 0 || xPos >= Main.maxTilesX || yPos < 0 || yPos >= Main.maxTilesY || dontShatter)
                    return;

                Tile t = Main.tile[xPos, yPos];
                if (t.HasTile && (t.TileType == TileType<LumenylCrystals>() || t.TileType == TileType<SeaPrismCrystals>() || t.TileType == TileType<SmallCorals>()))
                {
                    WorldGen.KillTile(xPos, yPos, false, false, false);
                    if (!Main.tile[xPos, yPos].HasTile && Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, xPos, yPos, 0f, 0, 0, 0);
                }
            }

            // Check if crystals should be shattered, do not shatter crystals next to other crystals if a crystal is shattered.
            if (Main.tileSolid[tile.TileType] && tile.TileType != TileType<LumenylCrystals>() && tile.TileType != TileType<SeaPrismCrystals>() && tile.TileType != TileType<SmallCorals>())
            {
                bool dontShatter = fail || effectOnly;
                CheckShatterCrystal(i + 1, j, dontShatter);
                CheckShatterCrystal(i - 1, j, dontShatter);
                CheckShatterCrystal(i, j + 1, dontShatter);
                CheckShatterCrystal(i, j - 1, dontShatter);
            }

            // Cumbling Dungeon Bricks have a 100% chance to crumble. This causes an effect similar to the Vein Miner mod.
            if (Main.netMode != NetmodeID.MultiplayerClient && tile.TileType >= TileID.CrackedBlueDungeonBrick && tile.TileType <= TileID.CrackedPinkDungeonBrick)
            {
                for (int m = 0; m < 8; m++)
                {
                    int x = i;
                    int y = j;
                    switch (m)
                    {
                        case 0:
                            x--;
                            break;
                        case 1:
                            x++;
                            break;
                        case 2:
                            y--;
                            break;
                        case 3:
                            y++;
                            break;
                        case 4:
                            x--;
                            y--;
                            break;
                        case 5:
                            x++;
                            y--;
                            break;
                        case 6:
                            x--;
                            y++;
                            break;
                        case 7:
                            x++;
                            y++;
                            break;
                    }

                    Tile tile3 = Main.tile[x, y];
                    if (tile3.HasTile && tile3.TileType >= TileID.CrackedBlueDungeonBrick && tile3.TileType <= TileID.CrackedPinkDungeonBrick)
                    {
                        tile.Get<TileWallWireStateData>().HasTile = false;
                        WorldGen.KillTile(x, y, fail: false, effectOnly: false, noItem: true);
                        if (Main.dedServ)
                            NetMessage.TrySendData(17, -1, -1, null, 20, x, y);
                    }
                }

                int projectileType = tile.TileType - TileID.CrackedBlueDungeonBrick + ProjectileID.BlueDungeonDebris;
                int damage = 20;
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Projectile.NewProjectile(new EntitySource_TileBreak(i, j), i * 16 + 8, j * 16 + 8, 0f, 0.41f, projectileType, damage, 0f, Main.myPlayer);
                }
                else if (Main.dedServ)
                {
                    int proj = Projectile.NewProjectile(new EntitySource_TileBreak(i, j), i * 16 + 8, j * 16 + 8, 0f, 0.41f, projectileType, damage, 0f, Main.myPlayer);
                    Main.projectile[proj].netUpdate = true;
                }
            }

            Player player = Main.LocalPlayer;
            if (player is null || !player.active)
                return;

            // Mining set gives a chance for additional ore. This can be abused for infinite ore but it has a cooldown to prevent too much abuse
            if (player.Calamity().miningSet && player.Calamity().miningSetCooldown <= 0 && !fail && TileID.Sets.Ore[tile.TileType])
            {
                // 25% chance for additional ore
                if (!Main.rand.NextBool(MiningArmorSetChange.BonusOreChance))
                    return;

                var source = new EntitySource_TileBreak(i, j);
                Vector2 pos = new Vector2(i, j) * 16;
                ModTile moddedTile = TileLoader.GetTile(tile.TileType);
                if (moddedTile != null) // Fetch the modded tile's drop logic
                {
                    IEnumerable<Item> itemDrops = moddedTile.GetItemDrops(i, j);
                    if (itemDrops == null)
                        return;

                    foreach (Item item in itemDrops)
                    {
                        item.Prefix(-1); // You're twisted if you have a prefixable item inside ores but fuck it
                        int moddedOre = Item.NewItem(source, pos, item);
                        Main.item[moddedOre].TryCombiningIntoNearbyItems(moddedOre);
                    }
                }
                else // Fetch normal tile-item relationships (all vanilla ores are normal thankfully)
                {
                    int itemType = TileLoader.GetItemDropFromTypeAndStyle(tile.TileType);
                    Item.NewItem(source, pos, itemType);
                }

                // Cooldown varies between 3 and 6 seconds
                player.Calamity().miningSetCooldown = Main.rand.Next(MiningArmorSetChange.CooldownMin, MiningArmorSetChange.CooldownMax + 1);
            }
        }

        public override void Drop(int i, int j, int type)/* tModPorter Suggestion: Use CanDrop to decide if items can drop, use this method to drop additional items. See documentation. */
        {
            // Handle for Demon Altar Drops
            // Drop:
            // - Soul of Night (x4) (Only if Early Hardmode Progression Rework is on)
            if (type == TileID.DemonAltar && Main.hardMode)
            {
                Vector2 spreadMinMax = new Vector2(-32.0f, 32.0f);

                // Drop 4 Soul of Night
                if (CalamityServerConfig.Instance.EarlyHardmodeProgressionRework)
                {
                    DropItem(i, j, ItemID.SoulofNight, quantity: 4, asStack: false, spreadMinMax);
                    WorldGen.altarCount++; // altarCount does not update automatically if ProgressionRework is enabled!
                    AchievementsHelper.NotifyProgressionEvent(6); // Gives the Begone, Evil! achievement
                }
            }
        }

        private static void DropItem(int i, int j, int itemType, int quantity, bool asStack, Vector2 spreadMinMax = default, bool prefix = false)
        {
            // Multiplayer Client should not spawn item themselves
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Vector2 worldPos = new Vector2(i, j) * 16.0f;
            if (asStack)
            {
                Vector2 spawnOffset = Main.rand.NextVector2Unit(spreadMinMax.X, spreadMinMax.Y);
                Item.NewItem(new EntitySource_TileBreak(i, j), worldPos + spawnOffset, itemType, Stack: quantity, prefixGiven: prefix ? -1 : 0);
            }
            else
            {
                for (int k = 0; k < quantity; k += 1)
                {
                    Vector2 spawnOffset = Main.rand.NextVector2Unit(spreadMinMax.X, spreadMinMax.Y);
                    Item.NewItem(new EntitySource_TileBreak(i, j), worldPos + spawnOffset, itemType, Stack: 1, prefixGiven: prefix ? -1 : 0);
                }
            }
        }
    }
}
