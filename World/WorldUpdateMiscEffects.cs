using CalamityMod.CalPlayer;
using CalamityMod.DataStructures;
using CalamityMod.Events;
using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs;
using CalamityMod.NPCs.AdultEidolonWyrm;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.ExoMechs;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Tiles;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.SunkenSea;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.World
{
    public partial class CalamityWorld : ModWorld
    {
        public override void PostUpdate()
        {
            // Reset this int because it causes bugs with other mods if you delete Dr. Draedon through abnormal means
            if (!NPC.AnyNPCs(ModContent.NPCType<Draedon>()))
                CalamityGlobalNPC.draedon = -1;

            // Reset the exo mech to summon if Draedon is absent.
            if (DraedonMechToSummon != ExoMech.None && CalamityGlobalNPC.draedon == -1)
                DraedonMechToSummon = ExoMech.None;

            if (Main.netMode != NetmodeID.MultiplayerClient && DraedonSummonCountdown > 0)
            {
                DraedonSummonCountdown--;
                HandleDraedonSummoning();
            }

            // Sunken Sea Location...duh
            SunkenSeaLocation = new Rectangle(WorldGen.UndergroundDesertLocation.Left, WorldGen.UndergroundDesertLocation.Bottom, WorldGen.UndergroundDesertLocation.Width, WorldGen.UndergroundDesertLocation.Height / 2);

            // Player variable, always finds the closest player relative to the center of the map
            int closestPlayer = Player.FindClosest(new Vector2(Main.maxTilesX / 2, (float)Main.worldSurface / 2f) * 16f, 0, 0);
            Player player = Main.player[closestPlayer];
            CalamityPlayer modPlayer = player.Calamity();

            // Force boss rush to off if necessary.
            if (!BossRushEvent.DeactivateStupidFuckingBullshit)
            {
                BossRushEvent.DeactivateStupidFuckingBullshit = true;
                BossRushEvent.BossRushActive = false;
                CalamityNetcode.SyncWorld();
            }

            // Check to see if a natural Acid Rain event should start.
            AcidRainEvent.TryToStartEventNaturally();

            // Handle Acid Rain update logic.
            if (rainingAcid)
                AcidRainEvent.Update();
            else
            {
                if (timeSinceAcidStarted != 0)
                    timeSinceAcidStarted = 0;
                startAcidicDownpour = false;
            }

            // Lumenyl crystal, tenebris spread and sea prism crystal spawn rates
            HandleTileGrowth();

            // Update Boss Rush.
            BossRushEvent.Update();

            // Handle Phase 2 DoG's summon-code, for his sentinels.
            HandleDoGP2Countdown(player);

            // Handle conditional summons.
            TrySpawnArmoredDigger(player, modPlayer);
            TrySpawnDungeonGuardian(player);
            TrySpawnAEoW(player, modPlayer);

            // Very, very, very rarely display a Lorde joke text if the system clock is set to April Fools Day.
            if (Main.rand.NextBool(100000000) && DateTime.Now.Month == 4 && DateTime.Now.Day == 1)
            {
                string key = "Mods.CalamityMod.AprilFools";
                Color messageColor = Color.Crimson;
                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }

            // Disable sandstorms if the Desert Scourge is still alive and Hardmode hasn't begun.
            if (!downedDesertScourge && Main.netMode != NetmodeID.MultiplayerClient && !Main.hardMode)
                CalamityUtils.StopSandstorm();

            // Attempt to summon lab critters manually since they refuse to exist when using vanilla's spawn methods.
            // This needs to check all players since the method only runs server-side.
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (!Main.player[i].active || Main.player[i].dead)
                    continue;

                CalamityGlobalNPC.AttemptToSpawnLabCritters(Main.player[i]);
            }

            // Make the cultist countdown happen much more quickly.
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                CultistRitual.delay -= Main.dayRate * 10;
                CultistRitual.recheck -= Main.dayRate * 10;
                if (CultistRitual.recheck < 0)
                    CultistRitual.recheck = 0;
                if (CultistRitual.delay < 0)
                    CultistRitual.delay = 0;
            }
        }

        #region Handle Draedon Summoning

        public static void HandleDraedonSummoning()
        {
            // Fire a giant laser into the sky.
            if (DraedonSummonCountdown == DraedonSummonCountdownMax - 45)
                Projectile.NewProjectile(DraedonSummonPosition + Vector2.UnitY * 80f, Vector2.Zero, ModContent.ProjectileType<DraedonSummonLaser>(), 0, 0f);

            if (DraedonSummonCountdown == 0)
                NPC.NewNPC((int)DraedonSummonPosition.X, (int)DraedonSummonPosition.Y, ModContent.NPCType<Draedon>());
        }
        #endregion Handle Draedon Summoning

        #region Handle Tile Growing

        public static void HandleTileGrowth()
        {
            int l = 0;
            float mult2 = 1.5E-05f * Main.worldRate;
            while (l < Main.maxTilesX * Main.maxTilesY * mult2)
            {
                int x = WorldGen.genRand.Next(10, Main.maxTilesX - 10);
                int y = WorldGen.genRand.Next((int)Main.worldSurface - 1, Main.maxTilesY - 20);

                int y2 = y - 1;
                if (y2 < 10)
                    y2 = 10;

                if (Main.tile[x, y] != null)
                {
                    if (Main.tile[x, y].nactive())
                    {
                        if (Main.tile[x, y].liquid <= 32)
                        {
                            if (Main.tile[x, y].type == TileID.JungleGrass)
                            {
                                if (Main.tile[x, y2].liquid == 0)
                                {
                                    // Plantera Bulbs pre-mech
                                    if (WorldGen.genRand.Next(1500) == 0)
                                    {
                                        if (Main.hardMode && (!NPC.downedMechBoss1 || !NPC.downedMechBoss2 || !NPC.downedMechBoss3))
                                        {
                                            bool placeBulb = true;
                                            int minDistanceFromOtherBulbs = 150;
                                            for (int i = x - minDistanceFromOtherBulbs; i < x + minDistanceFromOtherBulbs; i += 2)
                                            {
                                                for (int j = y - minDistanceFromOtherBulbs; j < y + minDistanceFromOtherBulbs; j += 2)
                                                {
                                                    if (i > 1 && i < Main.maxTilesX - 2 && j > 1 && j < Main.maxTilesY - 2 && Main.tile[i, j].active() && Main.tile[i, j].type == TileID.PlanteraBulb)
                                                    {
                                                        placeBulb = false;
                                                        break;
                                                    }
                                                }
                                            }

                                            if (placeBulb)
                                            {
                                                WorldGen.PlaceJunglePlant(x, y2, TileID.PlanteraBulb, 0, 0);
                                                WorldGen.SquareTileFrame(x, y2);
                                                WorldGen.SquareTileFrame(x + 2, y2);
                                                WorldGen.SquareTileFrame(x - 1, y2);
                                                if (Main.tile[x, y2].type == TileID.PlanteraBulb && Main.netMode == NetmodeID.Server)
                                                {
                                                    NetMessage.SendTileSquare(-1, x, y2, 5);
                                                }
                                            }
                                        }
                                    }

                                    // Life Fruit pre-mech
                                    int random = Main.expertMode ? 90 : 120;
                                    if (WorldGen.genRand.Next(random) == 0)
                                    {
                                        if (Main.hardMode && !NPC.downedMechBossAny)
                                        {
                                            bool placeFruit = true;
                                            int minDistanceFromOtherFruit = Main.expertMode ? 50 : 60;
                                            for (int i = x - minDistanceFromOtherFruit; i < x + minDistanceFromOtherFruit; i += 2)
                                            {
                                                for (int j = y - minDistanceFromOtherFruit; j < y + minDistanceFromOtherFruit; j += 2)
                                                {
                                                    if (i > 1 && i < Main.maxTilesX - 2 && j > 1 && j < Main.maxTilesY - 2 && Main.tile[i, j].active() && Main.tile[i, j].type == TileID.LifeFruit)
                                                    {
                                                        placeFruit = false;
                                                        break;
                                                    }
                                                }
                                            }

                                            if (placeFruit)
                                            {
                                                WorldGen.PlaceJunglePlant(x, y2, TileID.LifeFruit, WorldGen.genRand.Next(3), 0);
                                                WorldGen.SquareTileFrame(x, y2);
                                                WorldGen.SquareTileFrame(x + 1, y2 + 1);
                                                if (Main.tile[x, y2].type == TileID.LifeFruit && Main.netMode == NetmodeID.Server)
                                                {
                                                    NetMessage.SendTileSquare(-1, x, y2, 4);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        int tileType = Main.tile[x, y].type;
                        bool tenebris = tileType == ModContent.TileType<Tenebris>() && downedCalamitas;

                        if (CalamityGlobalTile.GrowthTiles.Contains(tileType) || tenebris)
                        {
                            int growthChance = tenebris ? 4 : 2;
                            if (tileType == ModContent.TileType<Navystone>())
                                growthChance *= 5;

                            if (Main.rand.NextBool(growthChance))
                            {
                                switch (WorldGen.genRand.Next(4))
                                {
                                    case 0:
                                        x++;
                                        break;
                                    case 1:
                                        x--;
                                        break;
                                    case 2:
                                        y++;
                                        break;
                                    case 3:
                                        y--;
                                        break;
                                    default:
                                        break;
                                }

                                if (Main.tile[x, y] != null)
                                {
                                    Tile tile = Main.tile[x, y];
                                    bool growTile = tenebris ? (tile.active() && tile.type == ModContent.TileType<PlantyMush>()) : (!tile.active() && tile.liquid >= 128);
                                    bool isSunkenSeaTile = tileType == ModContent.TileType<Navystone>() || tileType == ModContent.TileType<EutrophicSand>() || tileType == ModContent.TileType<SeaPrism>();
                                    bool meetsAdditionalGrowConditions = tile.slope() == 0 && !tile.halfBrick() && !tile.lava();

                                    if (growTile && meetsAdditionalGrowConditions)
                                    {
                                        int tileType2 = ModContent.TileType<SeaPrismCrystals>();

                                        if (tileType == ModContent.TileType<Voidstone>())
                                            tileType2 = ModContent.TileType<LumenylCrystals>();

                                        bool canPlaceBasedOnAttached = true;
                                        if (tileType2 == ModContent.TileType<SeaPrismCrystals>() && !isSunkenSeaTile)
                                            canPlaceBasedOnAttached = false;

                                        if (canPlaceBasedOnAttached && (CanPlaceBasedOnProximity(x, y, tileType2) || tenebris))
                                        {
                                            tile.type = tenebris ? (ushort)tileType : (ushort)tileType2;

                                            if (!tenebris)
                                            {
                                                tile.active(true);
                                                if (Main.tile[x, y + 1].active() && Main.tileSolid[Main.tile[x, y + 1].type] && Main.tile[x, y + 1].slope() == 0 && !Main.tile[x, y + 1].halfBrick())
                                                {
                                                    tile.frameY = 0;
                                                }
                                                else if (Main.tile[x, y - 1].active() && Main.tileSolid[Main.tile[x, y - 1].type] && Main.tile[x, y - 1].slope() == 0 && !Main.tile[x, y - 1].halfBrick())
                                                {
                                                    tile.frameY = 18;
                                                }
                                                else if (Main.tile[x + 1, y].active() && Main.tileSolid[Main.tile[x + 1, y].type] && Main.tile[x + 1, y].slope() == 0 && !Main.tile[x + 1, y].halfBrick())
                                                {
                                                    tile.frameY = 36;
                                                }
                                                else if (Main.tile[x - 1, y].active() && Main.tileSolid[Main.tile[x - 1, y].type] && Main.tile[x - 1, y].slope() == 0 && !Main.tile[x - 1, y].halfBrick())
                                                {
                                                    tile.frameY = 54;
                                                }
                                                tile.frameX = (short)(WorldGen.genRand.Next(18) * 18);
                                            }

                                            WorldGen.SquareTileFrame(x, y);

                                            if (Main.netMode == NetmodeID.Server)
                                                NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                l++;
            }

        }

        public static bool CanPlaceBasedOnProximity(int x, int y, int tileType)
        {
            if (tileType == ModContent.TileType<LumenylCrystals>() && !downedCalamitas)
                return false;

            int minDistanceFromOtherTiles = 6;
            int sameTilesNearby = 0;
            for (int i = x - minDistanceFromOtherTiles; i < x + minDistanceFromOtherTiles; i++)
            {
                for (int j = y - minDistanceFromOtherTiles; j < y + minDistanceFromOtherTiles; j++)
                {
                    if (Main.tile[i, j].active() && Main.tile[i, j].type == tileType)
                    {
                        sameTilesNearby++;
                        if (sameTilesNearby > 1)
                            return false;
                    }
                }
            }

            return true;
        }
        #endregion

        #region Handle Phase 2 DoG's Summoning
        public static void HandleDoGP2Countdown(Player player)
        {
            // Reset the DoG P2 transition countdown if DoG is not present.
            if (CalamityGlobalNPC.DoGHead == -1)
                DoGSecondStageCountdown = 0;

            if (DoGSecondStageCountdown <= 0)
                return;

            DoGSecondStageCountdown--;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (DoGSecondStageCountdown == 21540)
                    CalamityUtils.SpawnBossBetter(player.Center, ModContent.NPCType<CeaselessVoid>(), new OffscreenBossSpawnContext());
                if (DoGSecondStageCountdown == 14340)
                    CalamityUtils.SpawnBossBetter(player.Center, ModContent.NPCType<StormWeaverHead>(), new OffscreenBossSpawnContext());
                if (DoGSecondStageCountdown == 7140)
                    CalamityUtils.SpawnBossBetter(player.Center, ModContent.NPCType<Signus>(), new OffscreenBossSpawnContext());
            }
            if (Main.netMode == NetmodeID.Server)
            {
                var netMessage = CalamityMod.Instance.GetPacket();
                netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                netMessage.Write(DoGSecondStageCountdown);
                netMessage.Send();
            }
        }
        #endregion

        #region Handle Armored Digger Random Spawns
        public static void TrySpawnArmoredDigger(Player player, CalamityPlayer modPlayer)
        {
            if (player.ZoneRockLayerHeight && !player.ZoneUnderworldHeight && !player.ZoneDungeon && !player.ZoneJungle && !modPlayer.ZoneSunkenSea && !modPlayer.ZoneAbyss && !CalamityPlayer.areThereAnyDamnBosses)
            {
                if (NPC.downedPlantBoss && player.townNPCs < 3f)
                {
                    double spawnRate = 100000D;

                    if (revenge)
                        spawnRate *= 0.85D;

                    if (death && Main.bloodMoon)
                        spawnRate *= 0.2D;
                    if (modPlayer.zerg)
                        spawnRate *= 0.5D;
                    if (modPlayer.chaosCandle)
                        spawnRate *= 0.6D;
                    if (player.enemySpawns)
                        spawnRate *= 0.7D;
                    if (Main.waterCandles > 0)
                        spawnRate *= 0.8D;

                    if (modPlayer.bossZen || DoGSecondStageCountdown > 0)
                        spawnRate *= 50D;
                    if (modPlayer.zen || (CalamityConfig.Instance.DisableExpertTownSpawns && player.townNPCs > 1f && Main.expertMode))
                        spawnRate *= 2D;
                    if (modPlayer.tranquilityCandle)
                        spawnRate *= 1.67D;
                    if (player.calmed)
                        spawnRate *= 1.43D;
                    if (Main.peaceCandles > 0)
                        spawnRate *= 1.25D;
                    if (player.HasItem(ModContent.ItemType<DraedonsRemote>()))
                        spawnRate *= 5D;

                    int chance = (int)spawnRate;
                    if (Main.rand.NextBool(chance))
                    {
                        if (!NPC.AnyNPCs(ModContent.NPCType<ArmoredDiggerHead>()) && Main.netMode != NetmodeID.MultiplayerClient &&
                        ArmoredDiggerSpawnCooldown <= 0)
                        {
                            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<ArmoredDiggerHead>());
                            ArmoredDiggerSpawnCooldown = 36000;
                        }
                    }
                }
            }
            if (ArmoredDiggerSpawnCooldown > 0)
            {
                ArmoredDiggerSpawnCooldown--;
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = CalamityMod.Instance.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.ArmoredDiggerCountdownSync);
                    netMessage.Write(ArmoredDiggerSpawnCooldown);
                    netMessage.Send();
                }
            }
        }
        #endregion

        #region Handle Dungeon Guardian Spawns
        public static void TrySpawnDungeonGuardian(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || !player.ZoneDungeon || NPC.downedBoss3 || player.dead)
                return;

            if (!NPC.AnyNPCs(NPCID.DungeonGuardian))
                NPC.SpawnOnPlayer(player.whoAmI, NPCID.DungeonGuardian); //your hell is as vast as my bonergrin, pray your life ends quickly
        }
        #endregion

        #region Handle Adult Eidolon Wyrm Spawns
        public static void TrySpawnAEoW(Player player, CalamityPlayer modPlayer)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || !modPlayer.ZoneAbyss || !player.chaosState || player.dead)
                return;

            bool adultWyrmAlive = CalamityGlobalNPC.adultEidolonWyrmHead != -1 && Main.npc[CalamityGlobalNPC.adultEidolonWyrmHead].active;
            if (!adultWyrmAlive)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<EidolonWyrmHeadHuge>());
        }
        #endregion
    }
}
