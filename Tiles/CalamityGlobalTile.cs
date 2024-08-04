using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalamityMod.Events;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.VanillaArmorChanges;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Abyss.AbyssAmbient;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.AstralDesert;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.DraedonStructures;
using CalamityMod.Tiles.DraedonSummoner;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.SunkenSea;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
    public class CalamityGlobalTile : GlobalTile
    {
        public static ushort[] PlantTypes = new ushort[]
        {
            TileID.Plants,
            TileID.CorruptPlants,
            TileID.JunglePlants,
            TileID.MushroomPlants,
            TileID.Plants2,
            TileID.JunglePlants2,
            TileID.HallowedPlants,
            TileID.HallowedPlants2,
            TileID.CrimsonPlants,
            (ushort)ModContent.TileType<AstralShortPlants>(),
            (ushort)ModContent.TileType<AstralTallPlants>(),
            (ushort)ModContent.TileType<LavaPistil>(),
            (ushort)ModContent.TileType<CinderBlossomTallPlants>(),
            (ushort)ModContent.TileType<SulphurTentacleCorals>(),
            (ushort)ModContent.TileType<AbyssKelp>(),
            (ushort)ModContent.TileType<TenebrisRemnant>(),
            (ushort)ModContent.TileType<PhoviamareHalm>(),
            (ushort)ModContent.TileType<SmallCorals>(),

        };

        public static List<int> GrowthTiles = new List<int>()
        {
            ModContent.TileType<SeaPrism>(),
            ModContent.TileType<Navystone>(),
            ModContent.TileType<Voidstone>()
        };

        public override void SetStaticDefaults()
        {
            Main.tileSpelunker[TileID.LunarOre] = true;
            Main.tileOreFinderPriority[TileID.LunarOre] = 900;
        }

        public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak)
        {
            // Custom plant framing
            for (int k = 0; k < PlantTypes.Length; k++)
            {
                if (type == PlantTypes[k])
                {
                    TileFraming.PlantFrame(i, j);
                    return false;
                }
            }

            // Custom vine framing
            if (type == TileID.Vines || type == TileID.CrimsonVines || type == TileID.HallowedVines || type == ModContent.TileType<AstralVines>())
                TileFraming.VineFrame(i, j);

            return base.TileFrame(i, j, type, ref resetFrame, ref noBreak);
        }

        public override void PostDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            // Guaranteed not null at this point
            Tile tile = Main.tile[i, j];

            // This function is only for Astral Cactus. If the tile isn't even cactus, forget about it.
            if (type != TileID.Cactus)
                return;

            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;
            int frameX = tile.TileFrameX;
            int frameY = tile.TileFrameY;

            // Search down the cactus to find out whether the block it is planted in is Astral Sand.
            int xTile = i;
            if (frameX == 36) // Cactus segment which splits left
                xTile--;
            if (frameX == 54) // Cactus segment which splits right
                xTile++;
            if (frameX == 108) // Cactus segment which splits both directions
                xTile += (frameY == 18) ? -1 : 1;

            int yTile = j;
            bool slidingDownCactus = Main.tile[xTile, yTile] != null && Main.tile[xTile, yTile].TileType == TileID.Cactus && Main.tile[xTile, yTile].HasTile;
            while (!Main.tile[xTile, yTile].HasTile || !Main.tileSolid[Main.tile[xTile, yTile].TileType] || !slidingDownCactus)
            {
                if (Main.tile[xTile, yTile].TileType == TileID.Cactus && Main.tile[xTile, yTile].HasTile)
                {
                    slidingDownCactus = true;
                }
                yTile++;
                // Cacti are assumed to be no more than 20 blocks tall.
                if (yTile > i + 20)
                    break;
            }
            bool astralCactus = Main.tile[xTile, yTile].TileType == (ushort)ModContent.TileType<AstralSand>();

            // If it is actually astral cactus, then draw its glowmask.
            if (astralCactus)
            {
                spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/Tiles/AstralDesert/AstralCactusGlow").Value, new Vector2((float)(i * 16 - (int)Main.screenPosition.X), (float)(j * 16 - (int)Main.screenPosition.Y)) + zero, new Rectangle((int)frameX, (int)frameY, 16, 18), Color.White * 0.75f, 0f, default, 1f, SpriteEffects.None, 0f);
            }
        }

        // This function has two purposes:
        // 1 - Shatters adjacent Lumenyl or Sea Prism crystals when a neighboring solid tile is destroyed
        // 2 - Gives the player breath back when breaking blocks with Reaver set bonus
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Tile tile = Main.tile[i, j];

            // Fruit from trees upon tree destruction
            // 25% chance to drop 1 to 2 fruit
            if (!effectOnly && !fail && Main.netMode != NetmodeID.MultiplayerClient && TileID.Sets.IsShakeable[type] && WorldGen.genRand.NextBool(4))
            {
                GetTreeBottom(i, j, out int treeX, out int treeY);
                TreeTypes treeType = WorldGen.GetTreeType(Main.tile[treeX, treeY].TileType);
                if (treeType != TreeTypes.None)
                {
                    treeY--;
                    while (treeY > 10 && Main.tile[treeX, treeY].HasTile && TileID.Sets.IsShakeable[Main.tile[treeX, treeY].TileType])
                        treeY--;

                    treeY++;

                    if (WorldGen.IsTileALeafyTreeTop(treeX, treeY) && !Collision.SolidTiles(treeX - 2, treeX + 2, treeY - 2, treeY + 2))
                    {
                        int randomAmt = WorldGen.genRand.Next(1, 3);
                        for (int z = 0; z < randomAmt; z++)
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

                                    if (WorldGen.IsPalmOasisTree(treeX))
                                        treeDropItemType = WorldGen.genRand.NextBool() ? ItemID.Banana : ItemID.Coconut;

                                    break;

                                case TreeTypes.PalmCorrupt:

                                    if (WorldGen.genRand.NextBool())
                                        treeDropItemType = WorldGen.genRand.NextBool() ? ItemID.BlackCurrant : ItemID.Elderberry;
                                    else if (WorldGen.IsPalmOasisTree(treeX))
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
                                    else if (WorldGen.IsPalmOasisTree(treeX))
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
                                    else if (WorldGen.IsPalmOasisTree(treeX))
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
                            {
                                if (Main.rand.NextBool(100) || (DateTime.Now.Month == 2 && DateTime.Now.Day == 14 && Main.rand.NextBool(15)))
                                {
                                    treeDropItemType = ModContent.ItemType<HapuFruit>();
                                }
                                Item.NewItem(new EntitySource_TileBreak(treeX, treeY), treeX * 16, treeY * 16, 16, 16, treeDropItemType);
                            }
                        }
                    }
                }
            }

            // Helper function to shatter crystals attached to neighboring solid tiles.
            void CheckShatterCrystal(int xPos, int yPos, bool dontShatter)
            {
                if (xPos < 0 || xPos >= Main.maxTilesX || yPos < 0 || yPos >= Main.maxTilesY || dontShatter)
                    return;

                Tile t = Main.tile[xPos, yPos];
                if (t.HasTile && (t.TileType == ModContent.TileType<LumenylCrystals>() || t.TileType == ModContent.TileType<SeaPrismCrystals>()))
                {
                    WorldGen.KillTile(xPos, yPos, false, false, false);
                    if (!Main.tile[xPos, yPos].HasTile && Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, xPos, yPos, 0f, 0, 0, 0);
                }
            }

            // Check if crystals should be shattered, do not shatter crystals next to other crystals if a crystal is shattered.
            if (Main.tileSolid[tile.TileType] && tile.TileType != ModContent.TileType<LumenylCrystals>() && tile.TileType != ModContent.TileType<SeaPrismCrystals>())
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
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.TrySendData(17, -1, -1, null, 20, x, y);
                    }
                }

                int projectileType = tile.TileType - TileID.CrackedBlueDungeonBrick + ProjectileID.BlueDungeonDebris;
                int damage = 20;
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Projectile.NewProjectile(new EntitySource_TileBreak(i, j), i * 16 + 8, j * 16 + 8, 0f, 0.41f, projectileType, damage, 0f, Main.myPlayer);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    int proj = Projectile.NewProjectile(new EntitySource_TileBreak(i, j), i * 16 + 8, j * 16 + 8, 0f, 0.41f, projectileType, damage, 0f, Main.myPlayer);
                    Main.projectile[proj].netUpdate = true;
                }
            }

            Player player = Main.LocalPlayer;
            if (player is null || !player.active)
                return;

            if (player.Calamity().reaverExplore && !fail)
            {
                player.breath += 20;
                if (player.breath > player.breathMax)
                    player.breath = player.breathMax;
            }

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
            // - Evil Smasher (x1) (Every 12th altar)
            if (type == TileID.DemonAltar && Main.hardMode)
            {
                Vector2 spreadMinMax = new Vector2(-32.0f, 32.0f);

                // Drop 4 Soul of Night
                if (CalamityConfig.Instance.EarlyHardmodeProgressionRework)
                {
                    DropItem(i, j, ItemID.SoulofNight, quantity: 4, asStack: false, spreadMinMax);
                    WorldGen.altarCount++; // altarCount does not update automatically if ProgressionRework is enabled!
                }

                // Drop Evil Smasher on every 12 alter smashed
                if (WorldGen.altarCount > 1 && WorldGen.altarCount % 12 == 0)
                {
                    DropItem(i, j, ModContent.ItemType<EvilSmasher>(), quantity: 1, asStack: true);
                }
            }
        }

        private static void DropItem(int i, int j, int itemType, int quantity, bool asStack, Vector2 spreadMinMax = default)
        {
            // Multiplayer Client should not spawn item themselves
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Vector2 worldPos = new Vector2(i, j) * 16.0f;
            if (asStack)
            {
                Vector2 spawnOffset = Main.rand.NextVector2Unit(spreadMinMax.X, spreadMinMax.Y);
                Item.NewItem(new EntitySource_TileBreak(i, j), worldPos + spawnOffset, itemType, Stack: quantity);
            }
            else
            {
                for (int k = 0; k < quantity; k += 1)
                {
                    Vector2 spawnOffset = Main.rand.NextVector2Unit(spreadMinMax.X, spreadMinMax.Y);
                    Item.NewItem(new EntitySource_TileBreak(i, j), worldPos + spawnOffset, itemType, Stack: 1);
                }
            }
        }

        // TODO: Make this a data set or smth?
        // Plausible name: PreventsAnchorTileChanges  ///  Tile prevents its "anchors" from being hammered, killed, actuated, or edited in any way which may cause it to unintentionally break.
        public static bool ShouldNotBreakDueToAboveTile(int x, int y)
        {
            int[] invincibleTiles = new int[]
            {
                ModContent.TileType<DraedonLabTurret>(),
                ModContent.TileType<AstralBeacon>(),
                ModContent.TileType<CodebreakerTile>(),
                ModContent.TileType<SCalAltar>(),
                ModContent.TileType<GiantPlanteraBulb>()
            };

            Tile checkTile = CalamityUtils.ParanoidTileRetrieval(x, y);
            Tile aboveTile = CalamityUtils.ParanoidTileRetrieval(x, y - 1);

            // Prevent tiles below invincible tiles from being destroyed. This is like chests in vanilla.
            return aboveTile.HasTile && checkTile.TileType != aboveTile.TileType && invincibleTiles.Contains(aboveTile.TileType);
        }

        public override bool CanExplode(int i, int j, int type)
        {
            if (ShouldNotBreakDueToAboveTile(i, j))
                return false;

            return base.CanExplode(i, j, type);
        }

        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            if (ShouldNotBreakDueToAboveTile(i, j))
                return false;

            return base.CanKillTile(i, j, type, ref blockDamaged);
        }

        // "Private" my ass, fuck off
        public static void GetTreeBottom(int i, int j, out int x, out int y)
        {
            x = i;
            y = j;
            Tile tileSafely = Framing.GetTileSafely(x, y);
            if (tileSafely.TileType == TileID.PalmTree)
            {
                while (y < Main.maxTilesY - 50 && (!tileSafely.HasTile || tileSafely.TileType == TileID.PalmTree))
                {
                    y++;
                    tileSafely = Framing.GetTileSafely(x, y);
                }

                return;
            }

            int treeTileX = tileSafely.TileFrameX / 22;
            int treeTileY = tileSafely.TileFrameY / 22;
            if (treeTileX == 3 && treeTileY <= 2)
                x++;
            else if (treeTileX == 4 && treeTileY >= 3 && treeTileY <= 5)
                x--;
            else if (treeTileX == 1 && treeTileY >= 6 && treeTileY <= 8)
                x--;
            else if (treeTileX == 2 && treeTileY >= 6 && treeTileY <= 8)
                x++;
            else if (treeTileX == 2 && treeTileY >= 9)
                x++;
            else if (treeTileX == 3 && treeTileY >= 9)
                x--;

            tileSafely = Framing.GetTileSafely(x, y);
            while (y < Main.maxTilesY - 50 && (!tileSafely.HasTile || TileID.Sets.IsATreeTrunk[tileSafely.TileType] || tileSafely.TileType == TileID.MushroomTrees))
            {
                y++;
                tileSafely = Framing.GetTileSafely(x, y);
            }
        }
    }
}
