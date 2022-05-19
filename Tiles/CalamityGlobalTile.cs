using CalamityMod.Events;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.AstralDesert;
using CalamityMod.Tiles.DraedonStructures;
using CalamityMod.Tiles.DraedonSummoner;
using CalamityMod.Tiles.Furniture;
using CalamityMod.Tiles.FurnitureExo;
using CalamityMod.Tiles.SunkenSea;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
    public class CalamityGlobalTile : GlobalTile
    {
        internal static readonly MethodInfo ActiveFountainColorMethod = typeof(SceneMetrics).GetMethod("set_ActiveFountainColor", BindingFlags.NonPublic | BindingFlags.Instance);

        public static void SetActiveFountainColor(int fountainID)
        {
            ActiveFountainColorMethod.Invoke(Main.SceneMetrics, new object[]
            {
                fountainID
            });
        }

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
            (ushort)ModContent.TileType<AstralTallPlants>()
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

        public override bool PreHitWire(int i, int j, int type)
        {
            return !BossRushEvent.BossRushActive;
        }

        public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak)
        {
            // Custom plant framing
            for (int k = 0; k < PlantTypes.Length; k++)
                if (type == PlantTypes[k])
                {
                    TileFraming.PlantFrame(i, j);
                    return false;
                }

            // Custom vine framing
            if (type == TileID.Vines || type == TileID.CrimsonVines || type == TileID.HallowedVines || type == ModContent.TileType<AstralVines>())
            {
                TileFraming.VineFrame(i, j);
                return false;
            }
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

            // Helper function to shatter crystals attached to neighboring solid tiles.
            static void CheckShatterCrystal(int xPos, int yPos)
            {
                if (xPos < 0 || xPos >= Main.maxTilesX || yPos < 0 || yPos >= Main.maxTilesY)
                    return;
                Tile t = Main.tile[xPos, yPos];
                if (t.HasTile && (t.TileType == ModContent.TileType<LumenylCrystals>() || (t.TileType == ModContent.TileType<SeaPrismCrystals>() && DownedBossSystem.downedDesertScourge)))
                {
                    WorldGen.KillTile(xPos, yPos, false, false, false);
                    if (!Main.tile[xPos, yPos].HasTile && Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, xPos, yPos, 0f, 0, 0, 0);
                }
            }

            // CONSIDER -- Lumenyl Crystals and Sea Prism Crystals aren't solid. They shouldn't need to be checked here.
            if (Main.tileSolid[tile.TileType] && tile.TileType != ModContent.TileType<LumenylCrystals>() && tile.TileType != ModContent.TileType<SeaPrismCrystals>())
            {
                CheckShatterCrystal(i + 1, j);
                CheckShatterCrystal(i - 1, j);
                CheckShatterCrystal(i, j + 1);
                CheckShatterCrystal(i, j - 1);
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
        }

        // LATER -- clean up copied decompiled pot code here
        public override bool Drop(int i, int j, int type)
        {
            Tile tileAtPosition = CalamityUtils.ParanoidTileRetrieval(i, j);
            if (tileAtPosition.TileFrameX % 36 == 0 && tileAtPosition.TileFrameY % 36 == 0)
            {
                if (type == ModContent.TileType<AbyssalPots>())
                {
                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ModContent.ItemType<AbyssalTreasure>());

                    if (Main.netMode != NetmodeID.Server)
                    {
                        for (int k = 0; k < Main.rand.Next(1, 2 + 1); k++)
                        {
                            Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16, Main.rand.NextVector2CircularEdge(3f, 3f), Mod.Find<ModGore>("AbyssPotGore1").Type);
                            Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16, Main.rand.NextVector2CircularEdge(3f, 3f), Mod.Find<ModGore>("AbyssPotGore2").Type);
                        }
                    }
                }
                else if (type == ModContent.TileType<SulphurousPots>())
                {
                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ModContent.ItemType<SulphuricTreasure>());

                    if (Main.netMode != NetmodeID.Server)
                    {
                        for (int k = 0; k < Main.rand.Next(1, 2 + 1); k++)
                        {
                            Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16, Main.rand.NextVector2CircularEdge(3f, 3f), Mod.Find<ModGore>("SulphPotGore1").Type);
                            Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16, Main.rand.NextVector2CircularEdge(3f, 3f), Mod.Find<ModGore>("SulphPotGore2").Type);
                        }
                    }
                }
                else if (type == TileID.DemonAltar && Main.hardMode)
                {
                    Vector2 pos = new Vector2(i, j) * 16;
                    if (CalamityConfig.Instance.EarlyHardmodeProgressionRework)
                    {
                        WorldGen.altarCount++;
                        int quantity = 4;
                        for (int k = 0; k < quantity; k += 1)
                        {
                            pos.X += Main.rand.NextFloat(-32f, 32f);
                            pos.Y += Main.rand.NextFloat(-32f, 32f);
                            Item.NewItem(new EntitySource_TileBreak(i, j), pos, ItemID.SoulofNight);
                        }
                    }
                    if (WorldGen.altarCount % 3 == 0 && WorldGen.altarCount > 1)
                        Item.NewItem(new EntitySource_TileBreak(i, j), pos, ModContent.ItemType<EvilSmasher>());
                }
            }

            // This is old pot code, kept here for legacy reasons with old worlds. Should be removed in a future update after a sufficient amount of time.
            if (type == TileID.Pots)
            {
                int x = Main.maxTilesX;
                int y = Main.maxTilesY;
                int genLimit = x / 2;
                int abyssChasmSteps = y / 4;
                int abyssChasmY = y - abyssChasmSteps + (int)(y * 0.055); //132 = 1932 large
                if (y < 1500)
                {
                    abyssChasmY = y - abyssChasmSteps + (int)(y * 0.095); //114 = 1014 small
                }
                else if (y < 2100)
                {
                    abyssChasmY = y - abyssChasmSteps + (int)(y * 0.0735); //132 = 1482 medium
                }
                int abyssChasmX = CalamityWorld.abyssSide ? genLimit - (genLimit - 135) : genLimit + (genLimit - 135);

                bool abyssPosX = false;
                bool sulphurPosX = false;
                bool abyssPosY = j <= abyssChasmY;
                if (CalamityWorld.abyssSide)
                {
                    if (i < 380)
                    {
                        sulphurPosX = true;
                    }
                    if (i < abyssChasmX + 80)
                    {
                        abyssPosX = true;
                    }
                }
                else
                {
                    if (i > Main.maxTilesX - 380)
                    {
                        sulphurPosX = true;
                    }
                    if (i > abyssChasmX - 80)
                    {
                        abyssPosX = true;
                    }
                }
                if (abyssPosX && abyssPosY)
                {
                    if (Main.rand.NextBool(10))
                    {
                        int potionType = Utils.SelectRandom(WorldGen.genRand, new int[]
                        {
                            ItemID.SpelunkerPotion,
                            ItemID.MagicPowerPotion,
                            ItemID.ShinePotion,
                            ItemID.WaterWalkingPotion,
                            ItemID.ObsidianSkinPotion,
                            ItemID.WaterWalkingPotion,
                            ItemID.GravitationPotion,
                            ItemID.RegenerationPotion,
                            ModContent.ItemType<TriumphPotion>(),
                            ModContent.ItemType<AnechoicCoating>(),
                            ItemID.GillsPotion,
                            ItemID.EndurancePotion,
                            ItemID.HeartreachPotion,
                            ItemID.FlipperPotion,
                            ItemID.LifeforcePotion,
                            ItemID.InfernoPotion
                        });
                        Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, potionType, 1, false, 0, false, false);
                    }
                    else
                    {
                        int lootType = Main.rand.Next(10); //0 to 9
                        if (lootType == 0) //spelunker glowsticks
                        {
                            int sglowstickAmt = Main.rand.Next(2, 6);
                            if (Main.expertMode)
                            {
                                sglowstickAmt += Main.rand.Next(1, 7);
                            }
                            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ItemID.SpelunkerGlowstick, sglowstickAmt, false, 0, false, false);
                        }
                        else if (lootType == 1) //hellfire arrows
                        {
                            int arrowAmt = Main.rand.Next(10, 21);
                            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ItemID.HellfireArrow, arrowAmt, false, 0, false, false);
                        }
                        else if (lootType == 2) //stew
                        {
                            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ModContent.ItemType<SunkenStew>(), 1, false, 0, false, false);
                        }
                        else if (lootType == 3) //sticky dynamite
                        {
                            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ItemID.StickyDynamite, 1, false, 0, false, false);
                        }
                        else //money
                        {
                            float num13 = (float)(5000 + WorldGen.genRand.Next(-100, 101));
                            while ((int)num13 > 0)
                            {
                                if (num13 > 1000000f)
                                {
                                    int ptCoinAmt = (int)(num13 / 1000000f);
                                    if (ptCoinAmt > 50 && Main.rand.NextBool(2))
                                    {
                                        ptCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    if (Main.rand.NextBool(2))
                                    {
                                        ptCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    num13 -= (float)(1000000 * ptCoinAmt);
                                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ItemID.PlatinumCoin, ptCoinAmt, false, 0, false, false);
                                }
                                else if (num13 > 10000f)
                                {
                                    int auCoinAmt = (int)(num13 / 10000f);
                                    if (auCoinAmt > 50 && Main.rand.NextBool(2))
                                    {
                                        auCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    if (Main.rand.NextBool(2))
                                    {
                                        auCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    num13 -= (float)(10000 * auCoinAmt);
                                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ItemID.GoldCoin, auCoinAmt, false, 0, false, false);
                                }
                                else if (num13 > 100f)
                                {
                                    int agCoinAmt = (int)(num13 / 100f);
                                    if (agCoinAmt > 50 && Main.rand.NextBool(2))
                                    {
                                        agCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    if (Main.rand.NextBool(2))
                                    {
                                        agCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    num13 -= (float)(100 * agCoinAmt);
                                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ItemID.SilverCoin, agCoinAmt, false, 0, false, false);
                                }
                                else
                                {
                                    int cuCoinAmt = (int)num13;
                                    if (cuCoinAmt > 50 && Main.rand.NextBool(2))
                                    {
                                        cuCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    if (Main.rand.NextBool(2))
                                    {
                                        cuCoinAmt /= Main.rand.Next(4) + 1;
                                    }
                                    if (cuCoinAmt < 1)
                                    {
                                        cuCoinAmt = 1;
                                    }
                                    num13 -= (float)cuCoinAmt;
                                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ItemID.CopperCoin, cuCoinAmt, false, 0, false, false);
                                }
                            }
                        }
                    }
                }
                else if (sulphurPosX)
                {
                    if (Main.rand.NextBool(15))
                    {
                        int potionType = Utils.SelectRandom(WorldGen.genRand, new int[]
                        {
                            ItemID.SpelunkerPotion,
                            ItemID.MagicPowerPotion,
                            ItemID.ShinePotion,
                            ItemID.WaterWalkingPotion,
                            ItemID.ObsidianSkinPotion,
                            ItemID.WaterWalkingPotion,
                            ItemID.GravitationPotion,
                            ItemID.RegenerationPotion,
                            ModContent.ItemType<TriumphPotion>(),
                            ModContent.ItemType<AnechoicCoating>(),
                            ItemID.GillsPotion,
                            ItemID.EndurancePotion,
                            ItemID.HeartreachPotion,
                            ItemID.FlipperPotion,
                            ItemID.LifeforcePotion,
                            ItemID.InfernoPotion
                        });
                        Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, potionType, 1, false, 0, false, false);
                    }
                    else
                    {
                        int lootType = Main.rand.Next(10); //0 to 9
                        if (lootType == 0) //glowsticks
                        {
                            int glowstickAmt = Main.rand.Next(2, 6);
                            if (Main.expertMode)
                            {
                                glowstickAmt += Main.rand.Next(1, 7);
                            }
                            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ItemID.Glowstick, glowstickAmt, false, 0, false, false);
                        }
                        else if (lootType == 1) //jesters arrows
                        {
                            int jArrowAmt = Main.rand.Next(10, 21);
                            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ItemID.JestersArrow, jArrowAmt, false, 0, false, false);
                        }
                        else if (lootType == 2) //potion
                        {
                            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ItemID.HealingPotion, 1, false, 0, false, false);
                        }
                        else if (lootType == 3) //bomb
                        {
                            int bombAmt = Main.rand.Next(5, 9);
                            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ItemID.Bomb, bombAmt, false, 0, false, false);
                        }
                        else //money
                        {
                            float num13 = (float)(5000 + WorldGen.genRand.Next(-100, 101));
                            while ((int)num13 > 0)
                            {
                                if (num13 > 1000000f)
                                {
                                    int ptCoinAmt = (int)(num13 / 1000000f);
                                    if (ptCoinAmt > 50 && Main.rand.NextBool(2))
                                    {
                                        ptCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    if (Main.rand.NextBool(2))
                                    {
                                        ptCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    num13 -= (float)(1000000 * ptCoinAmt);
                                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ItemID.PlatinumCoin, ptCoinAmt, false, 0, false, false);
                                }
                                else if (num13 > 10000f)
                                {
                                    int auCoinAmt = (int)(num13 / 10000f);
                                    if (auCoinAmt > 50 && Main.rand.NextBool(2))
                                    {
                                        auCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    if (Main.rand.NextBool(2))
                                    {
                                        auCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    num13 -= (float)(10000 * auCoinAmt);
                                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ItemID.GoldCoin, auCoinAmt, false, 0, false, false);
                                }
                                else if (num13 > 100f)
                                {
                                    int agCoinAmt = (int)(num13 / 100f);
                                    if (agCoinAmt > 50 && Main.rand.NextBool(2))
                                    {
                                        agCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    if (Main.rand.NextBool(2))
                                    {
                                        agCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    num13 -= (float)(100 * agCoinAmt);
                                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ItemID.SilverCoin, agCoinAmt, false, 0, false, false);
                                }
                                else
                                {
                                    int cuCoinAmt = (int)num13;
                                    if (cuCoinAmt > 50 && Main.rand.NextBool(2))
                                    {
                                        cuCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    if (Main.rand.NextBool(2))
                                    {
                                        cuCoinAmt /= Main.rand.Next(4) + 1;
                                    }
                                    if (cuCoinAmt < 1)
                                    {
                                        cuCoinAmt = 1;
                                    }
                                    num13 -= (float)cuCoinAmt;
                                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ItemID.CopperCoin, cuCoinAmt, false, 0, false, false);
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        public override int[] AdjTiles (int type)
        {
            // Ashen, Ancient and Profaned Sinks all count as a lava source instead of a water source
            // Exo Sinks count as a water, lava, and honey source
            if (type == ModContent.TileType<FurnitureAncient.AncientSink>() ||
                type == ModContent.TileType<FurnitureAshen.AshenSink>() ||
                type == ModContent.TileType<FurnitureProfaned.ProfanedSink>() ||
                type == ModContent.TileType<ExoSinkTile>())
            {
                Main.LocalPlayer.adjLava = true;
            }
            // Botanic Sink counts as a honey source instead of a water source
            if (type == ModContent.TileType<FurnitureBotanic.BotanicSink>() ||
                type == ModContent.TileType<ExoSinkTile>())
            {
                Main.LocalPlayer.adjHoney = true;
            }

            return new int[0];
        }

        public static bool ShouldNotBreakDueToAboveTile(int x, int y)
        {
            int[] invincibleTiles = new int[]
            {
                ModContent.TileType<DraedonLabTurret>(),
                ModContent.TileType<AstralBeacon>(),
                ModContent.TileType<CodebreakerTile>(),
                ModContent.TileType<SCalAltar>()
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
    }
}
