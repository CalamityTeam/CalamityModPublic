using System;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.NPCs.ExoMechs;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Tiles;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.SunkenSea;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.World.CalamityWorld;
using Terraria.WorldBuilding;

namespace CalamityMod.Systems
{
    public class WorldMiscUpdateSystem : ModSystem
    {
        public override void PostUpdateWorld()
        {
            // Reset this int because it causes bugs with other mods if you delete Dr. Draedon through abnormal means.
            if (CalamityGlobalNPC.draedon != -1)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<Draedon>()))
                    CalamityGlobalNPC.draedon = -1;
            }

            // Reset the exo mech to summon if Draedon is absent.
            if (DraedonMechToSummon != ExoMech.None && CalamityGlobalNPC.draedon == -1)
                DraedonMechToSummon = ExoMech.None;

            if (Main.netMode != NetmodeID.MultiplayerClient && DraedonSummonCountdown > 0)
            {
                DraedonSummonCountdown--;
                HandleDraedonSummoning();
            }

            // Sunken Sea Location.
            // This moved in 1.4, it's now officially the "lower half of the Underground Desert" until its worldgen gets fixed.
            Rectangle ugDesert = GenVars.UndergroundDesertLocation;
            SunkenSeaLocation = new Rectangle(ugDesert.Left, ugDesert.Center.Y, ugDesert.Width, ugDesert.Height / 2);

            // Player variable, always finds the closest player relative to the center of the map.
            int closestPlayer = Player.FindClosest(new Vector2(Main.maxTilesX / 2, (float)Main.worldSurface / 2f) * 16f, 0, 0);
            Player player = Main.player[closestPlayer];

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
            if (AcidRainEvent.AcidRainEventIsOngoing)
                AcidRainEvent.Update();
            else
            {
                if (AcidRainEvent.TimeSinceEventStarted != 0)
                    AcidRainEvent.TimeSinceEventStarted = 0;
                AcidRainEvent.HasStartedAcidicDownpour = false;
            }

            // Lumenyl crystal and sea prism crystal spawn rates.
            HandleTileGrowth();

            // Update Boss Rush.
            BossRushEvent.Update();

            // Handle conditional summons.
            if (player is not null && player.active)
            {
                CalamityPlayer modPlayer = player.Calamity();
                TrySpawnArmoredDigger(player, modPlayer);
                TrySpawnDungeonGuardian(player);
                TrySpawnAEoW(player, modPlayer);
            }

            // Very, very, very rarely display a Lorde joke text if the system clock is set to April Fools Day.
            if (Main.rand.NextBool(100000000) && DateTime.Now.Month == 4 && DateTime.Now.Day == 1)
            {
                string key = Main.zenithWorld ? "Mods.CalamityMod.Status.Boss.AprilFoolsGFB" : "Mods.CalamityMod.Status.Boss.AprilFools";
                Color messageColor = Color.Crimson;
                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }

            // Disable sandstorms if the Desert Scourge is still alive and Hardmode hasn't begun.
            if (!DownedBossSystem.downedDesertScourge && Main.netMode != NetmodeID.MultiplayerClient && !Main.hardMode)
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
            {
                IEntitySource source = new EntitySource_WorldEvent();
                Projectile.NewProjectile(source, DraedonSummonPosition + Vector2.UnitY * 80f, Vector2.Zero, ModContent.ProjectileType<DraedonSummonLaser>(), 0, 0f);
            }

            if (DraedonSummonCountdown == 0)
            {
                IEntitySource source = new EntitySource_WorldEvent();
                NPC.NewNPC(source, (int)DraedonSummonPosition.X, (int)DraedonSummonPosition.Y, ModContent.NPCType<Draedon>());
            }
        }
        #endregion Handle Draedon Summoning

        #region Handle Tile Growing

        public static void HandleTileGrowth()
        {
            int l = 0;
            float mult2 = (float)(1.5E-05f * WorldGen.GetWorldUpdateRate());
            while (l < Main.maxTilesX * Main.maxTilesY * mult2)
            {
                int x = WorldGen.genRand.Next(10, Main.maxTilesX - 10);
                int y = WorldGen.genRand.Next((int)Main.worldSurface - 1, Main.maxTilesY - 20);

                int y2 = y - 1;
                if (y2 < 10)
                    y2 = 10;

                if (WorldGen.InWorld(x, y, 1) && Main.tile[x, y].HasTile)
                {
                    if (Main.tile[x, y].HasUnactuatedTile)
                    {
                        if (Main.tile[x, y].LiquidAmount <= 32)
                        {
                            if (Main.tile[x, y].TileType == TileID.JungleGrass)
                            {
                                if (Main.tile[x, y2].LiquidAmount == 0)
                                {
                                    // Plantera Bulbs pre-mech
                                    if (WorldGen.genRand.NextBool(1500))
                                    {
                                        if (Main.hardMode && (!NPC.downedMechBoss1 || !NPC.downedMechBoss2 || !NPC.downedMechBoss3))
                                        {
                                            bool placeBulb = true;
                                            int minDistanceFromOtherBulbs = 150;
                                            for (int i = x - minDistanceFromOtherBulbs; i < x + minDistanceFromOtherBulbs; i += 2)
                                            {
                                                for (int j = y - minDistanceFromOtherBulbs; j < y + minDistanceFromOtherBulbs; j += 2)
                                                {
                                                    if (i > 1 && i < Main.maxTilesX - 2 && j > 1 && j < Main.maxTilesY - 2 && Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.PlanteraBulb)
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
                                                if (Main.tile[x, y2].TileType == TileID.PlanteraBulb && Main.netMode == NetmodeID.Server)
                                                {
                                                    NetMessage.SendTileSquare(-1, x, y2, 5);
                                                }
                                            }
                                        }
                                    }

                                    // Life Fruit pre-mech
                                    int random = Main.expertMode ? 90 : 120;
                                    if (WorldGen.genRand.NextBool(random))
                                    {
                                        if (Main.hardMode && !NPC.downedMechBossAny)
                                        {
                                            bool placeFruit = true;
                                            int minDistanceFromOtherFruit = Main.expertMode ? 50 : 60;
                                            for (int i = x - minDistanceFromOtherFruit; i < x + minDistanceFromOtherFruit; i += 2)
                                            {
                                                for (int j = y - minDistanceFromOtherFruit; j < y + minDistanceFromOtherFruit; j += 2)
                                                {
                                                    if (i > 1 && i < Main.maxTilesX - 2 && j > 1 && j < Main.maxTilesY - 2 && Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.LifeFruit)
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
                                                if (Main.tile[x, y2].TileType == TileID.LifeFruit && Main.netMode == NetmodeID.Server)
                                                {
                                                    NetMessage.SendTileSquare(-1, x, y2, 4);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        Tile growthTile = Main.tile[x, y];
                        int tileType = growthTile.TileType;
                        if (CalamityGlobalTile.GrowthTiles.Contains(tileType) && growthTile.Slope == SlopeType.Solid && !growthTile.IsHalfBlock)
                        {
                            int growthChance = 2;
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
                                    bool growTile = !tile.HasTile && tile.LiquidAmount >= 128;
                                    bool isSunkenSeaTile = tileType == ModContent.TileType<Navystone>() || tileType == ModContent.TileType<EutrophicSand>() || tileType == ModContent.TileType<SeaPrism>();
                                    bool meetsAdditionalGrowConditions = tile.Slope == SlopeType.Solid && !tile.IsHalfBlock && tile.LiquidType != LiquidID.Lava;

                                    if (growTile && meetsAdditionalGrowConditions)
                                    {
                                        int tileType2 = ModContent.TileType<SeaPrismCrystals>();

                                        if (tileType == ModContent.TileType<Voidstone>())
                                            tileType2 = ModContent.TileType<LumenylCrystals>();

                                        bool canPlaceBasedOnAttached = true;
                                        if (tileType2 == ModContent.TileType<SeaPrismCrystals>() && !isSunkenSeaTile)
                                            canPlaceBasedOnAttached = false;

                                        if (canPlaceBasedOnAttached && CanPlaceBasedOnProximity(x, y, tileType2))
                                        {
                                            tile.TileType = (ushort)tileType2;

                                            tile.HasTile = true;
                                            if (Main.tile[x, y + 1].HasTile && Main.tileSolid[Main.tile[x, y + 1].TileType] && Main.tile[x, y + 1].Slope == 0 && !Main.tile[x, y + 1].IsHalfBlock)
                                            {
                                                tile.TileFrameY = 0;
                                            }
                                            else if (Main.tile[x, y - 1].HasTile && Main.tileSolid[Main.tile[x, y - 1].TileType] && Main.tile[x, y - 1].Slope == 0 && !Main.tile[x, y - 1].IsHalfBlock)
                                            {
                                                tile.TileFrameY = 18;
                                            }
                                            else if (Main.tile[x + 1, y].HasTile && Main.tileSolid[Main.tile[x + 1, y].TileType] && Main.tile[x + 1, y].Slope == 0 && !Main.tile[x + 1, y].IsHalfBlock)
                                            {
                                                tile.TileFrameY = 36;
                                            }
                                            else if (Main.tile[x - 1, y].HasTile && Main.tileSolid[Main.tile[x - 1, y].TileType] && Main.tile[x - 1, y].Slope == 0 && !Main.tile[x - 1, y].IsHalfBlock)
                                            {
                                                tile.TileFrameY = 54;
                                            }
                                            tile.TileFrameX = (short)(WorldGen.genRand.Next(18) * 18);

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
            if (tileType == ModContent.TileType<LumenylCrystals>() && !DownedBossSystem.downedCalamitasClone)
                return false;

            int minDistanceFromOtherTiles = 10;
            int sameTilesNearby = 0;
            for (int i = x - minDistanceFromOtherTiles; i < x + minDistanceFromOtherTiles; i++)
            {
                for (int j = y - minDistanceFromOtherTiles; j < y + minDistanceFromOtherTiles; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == tileType)
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

        #region Handle Armored Digger Random Spawns
        public static void TrySpawnArmoredDigger(Player player, CalamityPlayer modPlayer)
        {
            bool gfbCondition = Main.zenithWorld && (player.ZoneHallow || player.ZoneUnderworldHeight) && NPC.downedMoonlord;
            if ((gfbCondition || (player.ZoneRockLayerHeight && !player.ZoneUnderworldHeight && !player.ZoneJungle)) && !player.ZoneDungeon && !modPlayer.ZoneSunkenSea && !modPlayer.ZoneAbyss && !CalamityPlayer.areThereAnyDamnBosses)
            {
                if (NPC.downedPlantBoss && player.townNPCs < 3f)
                {
                    double spawnRate = 100000D;

                    if (CalamityWorld.LegendaryMode && revenge)
                        spawnRate *= 0.25D;

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
                    if (Main.SceneMetrics.WaterCandleCount > 0)
                        spawnRate *= 0.8D;

                    if (modPlayer.isNearbyBoss && CalamityConfig.Instance.BossZen)
                        spawnRate *= 50D;
                    if (modPlayer.zen || (CalamityConfig.Instance.ForceTownSafety && player.townNPCs > 1f && Main.expertMode))
                        spawnRate *= 2D;
                    if (modPlayer.tranquilityCandle)
                        spawnRate *= 1.67D;
                    if (player.calmed)
                        spawnRate *= 1.43D;
                    if (Main.SceneMetrics.PeaceCandleCount > 0)
                        spawnRate *= 1.25D;

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
            if (Main.netMode == NetmodeID.MultiplayerClient || !player.ZoneDungeon || player.dead)
                return;

            bool spawn = !NPC.downedBoss3;
            if (Main.drunkWorld && player.position.Y / 16f < (float)(Main.dungeonY + 40))
                spawn = false;

            if (spawn)
            {
                if (!NPC.AnyNPCs(NPCID.DungeonGuardian))
                    NPC.SpawnOnPlayer(player.whoAmI, NPCID.DungeonGuardian); //your hell is as vast as my bonergrin, pray your life ends quickly
            }
        }
        #endregion

        #region Handle Primordial Wyrm Spawns
        public static void TrySpawnAEoW(Player player, CalamityPlayer modPlayer)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || !(modPlayer.ZoneAbyss || Main.zenithWorld) || !player.chaosState || player.dead)
                return;

            bool adultWyrmAlive = CalamityGlobalNPC.adultEidolonWyrmHead != -1 && Main.npc[CalamityGlobalNPC.adultEidolonWyrmHead].active;
            if (!adultWyrmAlive)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<PrimordialWyrmHead>());
        }
        #endregion
    }
}
