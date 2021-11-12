using CalamityMod.CalPlayer;
using CalamityMod.DataStructures;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Tiles;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.SunkenSea;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

using static CalamityMod.World.CalamityWorld;

namespace CalamityMod.World
{
    public class WorldUpdateMiscEffects
    {
        public static void PerformWorldUpdates()
        {
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
            TrySpawnRandomDeathmodeBosses(player, modPlayer);

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

                    if (demonMode)
                        spawnRate *= 0.75D;

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

                    int chance = (int)spawnRate;
                    if (Main.rand.NextBool(chance))
                    {
                        if (!NPC.AnyNPCs(ModContent.NPCType<ArmoredDiggerHead>()) && Main.netMode != NetmodeID.MultiplayerClient &&
                        ArmoredDiggerSpawnCooldown <= 0)
                        {
                            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<ArmoredDiggerHead>());
                            ArmoredDiggerSpawnCooldown = 3600;
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

        #region Handle Deathmode Random Boss Spawns

        public static void TrySpawnRandomDeathmodeBosses(Player player, CalamityPlayer modPlayer)
        {
            void BossText()
            {
                string key = "Mods.CalamityMod.BossSpawnText";
                Color messageColor = Color.Crimson;
                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }

            //does not occur while a boss is alive or during certain events)
            if (!death || CalamityPlayer.areThereAnyDamnBosses || Main.snowMoon || Main.pumpkinMoon || DD2Event.Ongoing || player.statLifeMax2 < 300 || WorldGen.spawnEye || WorldGen.spawnHardBoss > 0)
                return;

            if (bossSpawnCountdown <= 0 && deathBossSpawnCooldown <= 0) // Check for countdown and cooldown being 0
            {
                if (Main.rand.NextBool(50000))
                {
                    // Only set countdown and boss type if conditions are met
                    // Night time bosses set message only before 11pm. Day time bosses only before 2pm.
                    if (!NPC.downedBoss1 && bossType == 0)
                        if (!Main.dayTime && (player.ZoneOverworldHeight || player.ZoneSkyHeight) && Main.time < 12600)
                        {
                            BossText();
                            bossType = NPCID.EyeofCthulhu;
                            bossSpawnCountdown = 3600; // 1 minute
                        }

                    if (!NPC.downedBoss2 && bossType == 0)
                        if (player.ZoneCorrupt)
                        {
                            BossText();
                            bossType = NPCID.EaterofWorldsHead;
                            bossSpawnCountdown = 3600;
                        }

                    if (!NPC.downedBoss2 && bossType == 0)
                        if (player.ZoneCrimson)
                        {
                            BossText();
                            bossType = NPCID.BrainofCthulhu;
                            bossSpawnCountdown = 3600;
                        }

                    /*if (!NPC.downedQueenBee && bossType == 0)
                        if (player.ZoneJungle && !player.ZoneOverworldHeight && !player.ZoneSkyHeight)
                        {
                            BossText();
                            bossType = NPCID.QueenBee;
                            bossSpawnCountdown = 3600;
                        }*/

                    if (!downedDesertScourge && bossType == 0)
                        if (player.ZoneDesert)
                        {
                            BossText();
                            bossType = ModContent.NPCType<DesertScourgeHead>();
                            bossSpawnCountdown = 3600;
                        }

                    if (!downedPerforator && bossType == 0)
                        if (player.ZoneCrimson)
                        {
                            BossText();
                            bossType = ModContent.NPCType<PerforatorHive>();
                            bossSpawnCountdown = 3600;
                        }

                    if (!downedHiveMind && bossType == 0)
                        if (player.ZoneCorrupt)
                        {
                            BossText();
                            bossType = ModContent.NPCType<HiveMind>();
                            bossSpawnCountdown = 3600;
                        }

                    if (!downedCrabulon && bossType == 0)
                        if (player.ZoneGlowshroom)
                        {
                            BossText();
                            bossType = ModContent.NPCType<CrabulonIdle>();
                            bossSpawnCountdown = 3600;
                        }

                    if (Main.hardMode)
                    {
                        if (!NPC.downedMechBoss1 && bossType == 0)
                            if (!Main.dayTime && (player.ZoneOverworldHeight || player.ZoneSkyHeight) && Main.time < 12600)
                            {
                                BossText();
                                bossType = NPCID.TheDestroyer;
                                bossSpawnCountdown = 3600;
                            }

                        if (!NPC.downedMechBoss2 && bossType == 0)
                            if (!Main.dayTime && (player.ZoneOverworldHeight || player.ZoneSkyHeight) && Main.time < 12600)
                            {
                                BossText();
                                bossType = NPCID.Spazmatism;
                                bossSpawnCountdown = 3600;
                            }

                        if (!NPC.downedMechBoss3 && bossType == 0)
                            if (!Main.dayTime && (player.ZoneOverworldHeight || player.ZoneSkyHeight) && Main.time < 12600)
                            {
                                BossText();
                                bossType = NPCID.SkeletronPrime;
                                bossSpawnCountdown = 3600;
                            }

                        if (!NPC.downedPlantBoss && bossType == 0)
                            if (player.ZoneJungle && !player.ZoneOverworldHeight && !player.ZoneSkyHeight)
                            {
                                BossText();
                                bossType = NPCID.Plantera;
                                bossSpawnCountdown = 3600;
                            }

                        if (!NPC.downedFishron && bossType == 0)
                            if (player.ZoneBeach && !modPlayer.ZoneSulphur)
                            {
                                BossText();
                                bossType = NPCID.DukeFishron;
                                bossSpawnCountdown = 3600;
                            }

                        if (!downedCryogen && bossType == 0)
                            if (player.ZoneSnow && player.ZoneOverworldHeight)
                            {
                                BossText();
                                bossType = ModContent.NPCType<Cryogen>();
                                bossSpawnCountdown = 3600;
                            }

                        if (!downedCalamitas && bossType == 0)
                            if (!Main.dayTime && player.ZoneOverworldHeight && Main.time < 12600)
                            {
                                BossText();
                                bossType = ModContent.NPCType<CalamitasRun3>();
                                bossSpawnCountdown = 3600;
                            }

                        if (!downedAstrageldon && bossType == 0)
                            if (modPlayer.ZoneAstral &&
                                !Main.dayTime && player.ZoneOverworldHeight)
                            {
                                BossText();
                                bossType = ModContent.NPCType<AstrumAureus>();
                                bossSpawnCountdown = 3600;
                            }

                        /*if (!downedPlaguebringer && bossType == 0)
                            if (player.ZoneJungle && NPC.downedGolemBoss && !player.ZoneOverworldHeight && !player.ZoneSkyHeight)
                            {
                                BossText();
                                bossType = ModContent.NPCType<PlaguebringerGoliath>();
                                bossSpawnCountdown = 3600;
                            }*/

                        if (NPC.downedMoonlord)
                        {
                            if (!downedGuardians && bossType == 0)
                                if (Main.dayTime && (player.ZoneUnderworldHeight ||
                                    (player.ZoneHoly && player.ZoneOverworldHeight)) && Main.time < 34200) //only before 2pm
                                {
                                    BossText();
                                    bossType = ModContent.NPCType<ProfanedGuardianBoss>();
                                    bossSpawnCountdown = 3600;
                                }

                            if (!downedBumble && bossType == 0)
                                if (player.ZoneJungle && player.ZoneOverworldHeight)
                                {
                                    BossText();
                                    bossType = ModContent.NPCType<Bumblefuck>();
                                    bossSpawnCountdown = 3600;
                                }
                        }
                    }
                    if (Main.netMode == NetmodeID.Server)
                    {
                        var netMessage = CalamityMod.Instance.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.BossSpawnCountdownSync);
                        netMessage.Write(bossSpawnCountdown);
                        netMessage.Send();
                        var netMessage2 = CalamityMod.Instance.GetPacket();
                        netMessage2.Write((byte)CalamityModMessageType.BossTypeSync);
                        netMessage2.Write(bossType);
                        netMessage2.Send();
                    }
                }
            }
            else
            {
                if (bossSpawnCountdown > 0)
                {
                    bossSpawnCountdown--;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        var netMessage = CalamityMod.Instance.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.BossSpawnCountdownSync);
                        netMessage.Write(bossSpawnCountdown);
                        netMessage.Send();
                    }
                }

                if (bossSpawnCountdown <= 0 && deathBossSpawnCooldown <= 0) // Check both cooldowns again here to avoid infinite message possibilities
                {
                    bool canSpawn = true;
                    switch (bossType)
                    {
                        case NPCID.EyeofCthulhu:
                            if (Main.dayTime || (!player.ZoneOverworldHeight && !player.ZoneSkyHeight) || Main.time > 16200)
                                canSpawn = false;
                            break;
                        case NPCID.EaterofWorldsHead:
                            if (!player.ZoneCorrupt)
                                canSpawn = false;
                            break;
                        case NPCID.BrainofCthulhu:
                            if (!player.ZoneCrimson)
                                canSpawn = false;
                            break;
                        /*case NPCID.QueenBee:
                            if (!player.ZoneJungle || player.ZoneOverworldHeight || player.ZoneSkyHeight)
                                canSpawn = false;
                            break;*/
                        case NPCID.TheDestroyer:
                            if (Main.dayTime || (!player.ZoneOverworldHeight && !player.ZoneSkyHeight) || Main.time > 16200)
                                canSpawn = false;
                            break;
                        case NPCID.Spazmatism:
                            if (Main.dayTime || (!player.ZoneOverworldHeight && !player.ZoneSkyHeight) || Main.time > 16200)
                                canSpawn = false;
                            break;
                        case NPCID.SkeletronPrime:
                            if (Main.dayTime || (!player.ZoneOverworldHeight && !player.ZoneSkyHeight) || Main.time > 16200)
                                canSpawn = false;
                            break;
                        case NPCID.Plantera:
                            if (!player.ZoneJungle || player.ZoneOverworldHeight || player.ZoneSkyHeight)
                                canSpawn = false;
                            break;
                        case NPCID.DukeFishron:
                            if (!player.ZoneBeach)
                                canSpawn = false;
                            break;
                    }

                    if (bossType == ModContent.NPCType<DesertScourgeHead>())
                    {
                        if (!player.ZoneDesert)
                            canSpawn = false;
                    }
                    else if (bossType == ModContent.NPCType<PerforatorHive>())
                    {
                        if (!player.ZoneCrimson)
                            canSpawn = false;
                    }
                    else if (bossType == ModContent.NPCType<HiveMind>())
                    {
                        if (!player.ZoneCorrupt)
                            canSpawn = false;
                    }
                    else if (bossType == ModContent.NPCType<CrabulonIdle>())
                    {
                        if (!player.ZoneGlowshroom)
                            canSpawn = false;
                    }
                    else if (bossType == ModContent.NPCType<Cryogen>())
                    {
                        if (!player.ZoneSnow || !player.ZoneOverworldHeight)
                            canSpawn = false;
                    }
                    else if (bossType == ModContent.NPCType<CalamitasRun3>())
                    {
                        if (Main.dayTime || !player.ZoneOverworldHeight || Main.time > 16200)
                            canSpawn = false;
                    }
                    else if (bossType == ModContent.NPCType<AstrumAureus>())
                    {
                        if (!modPlayer.ZoneAstral ||
                                Main.dayTime || !player.ZoneOverworldHeight)
                            canSpawn = false;
                    }
                    /*else if (bossType == ModContent.NPCType<PlaguebringerGoliath>())
                    {
                        if (!player.ZoneJungle || player.ZoneOverworldHeight || player.ZoneSkyHeight)
                            canSpawn = false;
                    }*/
                    else if (bossType == ModContent.NPCType<ProfanedGuardianBoss>())
                    {
                        if (!Main.dayTime || (!player.ZoneUnderworldHeight &&
                                    (!player.ZoneHoly || !player.ZoneOverworldHeight)) || Main.time > 37800)
                            canSpawn = false;
                    }
                    else if (bossType == ModContent.NPCType<Bumblefuck>())
                    {
                        if (!player.ZoneJungle || !player.ZoneOverworldHeight)
                            canSpawn = false;
                    }

                    if (canSpawn)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (bossType == NPCID.Spazmatism)
                                NPC.SpawnOnPlayer(player.whoAmI, NPCID.Retinazer);
                            else if (bossType == ModContent.NPCType<ProfanedGuardianBoss>())
                            {
                                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<ProfanedGuardianBoss2>());
                                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<ProfanedGuardianBoss3>());
                            }
                            else if (bossType == ModContent.NPCType<DesertScourgeHead>())
                            {
                                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<DesertNuisanceHead>());
                                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<DesertNuisanceHead>());
                            }
                            if (bossType == NPCID.DukeFishron)
                            {
                                NPC.NewNPC((int)player.Center.X - 300, (int)player.Center.Y - 300, bossType);
                            }
                            else
                            {
                                NPC.SpawnOnPlayer(player.whoAmI, bossType);
                            }
                        }

                        deathBossSpawnCooldown = 86400; // 24 minutes (1 full Terraria day)
                        if (Main.netMode == NetmodeID.Server)
                        {
                            var netMessage = CalamityMod.Instance.GetPacket();
                            netMessage.Write((byte)CalamityModMessageType.DeathBossSpawnCountdownSync);
                            netMessage.Write(deathBossSpawnCooldown);
                            netMessage.Send();
                        }
                    }

                    bossType = 0;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        var netMessage = CalamityMod.Instance.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.BossTypeSync);
                        netMessage.Write(bossType);
                        netMessage.Send();
                    }
                }

                // IMPORTANT! Decrement this cooldown AFTER everything else to avoid infinite possibilities
                if (deathBossSpawnCooldown > 0)
                {
                    deathBossSpawnCooldown--;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        var netMessage = CalamityMod.Instance.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.DeathBossSpawnCountdownSync);
                        netMessage.Write(deathBossSpawnCooldown);
                        netMessage.Send();
                    }
                }
            }
        }
        #endregion
    }
}
