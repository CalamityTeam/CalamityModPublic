using System;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Tiles;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.SunkenSea;
using CalamityMod.Tiles.SunkenSea.Ambient;
using CalamityMod.Walls;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static CalamityMod.World.CalamityWorld;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Systems
{
    public class WorldMiscUpdateSystem : ModSystem
    {
        public override void PostUpdateWorld()
        {
            // Reset this int because it causes bugs with other mods if you delete Dr. Draedon through abnormal means.
            if (CalamityGlobalNPC.draedon != -1)
            {
                if (!NPC.AnyNPCs(NPCType<Draedon>()))
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
            {
                AcidRainEvent.Update();
            }
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
                TrySpawnDungeonGuardian(player);
                TrySpawnAEoW(player, modPlayer);
            }

            // Very, very, very rarely display a Lorde joke text if the system clock is set to April Fools Day.
            if (Main.rand.NextBool(100000000) && DateTime.Now.Month == 4 && DateTime.Now.Day == 1)
            {
                string key = Main.zenithWorld ? "Mods.CalamityMod.Status.Boss.AprilFoolsGFB" : "Mods.CalamityMod.Status.Boss.AprilFools";
                Color messageColor = Color.Crimson;
                CalamityUtils.BroadcastLocalizedText(key, messageColor);
            }

            // Disable sandstorms if the Desert Scourge is still alive and Hardmode hasn't begun.
            if (!DownedBossSystem.downedDesertScourge && Main.netMode != NetmodeID.MultiplayerClient && !Main.hardMode)
                CalamityWorld.StopSandstorm();

            // Attempt to summon lab critters and lava-based NPCs manually since they refuse to exist when using vanilla's spawn methods.
            // This needs to check all players since the method only runs server-side.
            foreach (Player p in Main.ActivePlayers)
            {
                if (p.dead)
                    continue;

                CalamityGlobalNPC.AttemptToSpawnLabCritters(p);
                //CalamityGlobalNPC.AttemptToSpawnLavaNPCs(p);
            }

            // Spawn the Old Man if Skeletron hasn't been defeated and there is no Old Man, it takes too fucking long otherwise.
            TrySpawnOldMan();

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
                Projectile.NewProjectile(source, DraedonSummonPosition + Vector2.UnitY * 80f, Vector2.Zero, ProjectileType<DraedonSummonLaser>(), 0, 0f);
            }

            if (DraedonSummonCountdown == 0)
            {
                IEntitySource source = new EntitySource_WorldEvent();
                NPC.NewNPC(source, (int)DraedonSummonPosition.X, (int)DraedonSummonPosition.Y, NPCType<Draedon>());
            }
        }
        #endregion Handle Draedon Summoning

        #region Handle Tile Growing

        public static void HandleTileGrowth()
        {
            double worldUpdateRate = WorldGen.GetWorldUpdateRate();
            if (worldUpdateRate == 0)
                return;

            // Used for growing herbs at an accelerated rate
            double herbGrowthRateSurface = 3E-05f * (float)worldUpdateRate;
            double herbGrowthRateUnderground = 1.5E-05f * (float)worldUpdateRate;
            double remixWorldHerbGrowthRate = 2.5E-05f * (float)worldUpdateRate;

            // 5% chance to boost non-planter box herb growth rate
            // 50% chance to boost correct planter box herb growth rate
            int oneInXChanceToBoostHerbGrowthRate = 20;
            int oneInXChanceToBoostHerbGrowthRate_PlanterBox = 2;

            int oneInXMaximumChanceToPlaceHerb = 15100;
            int oneInXMinimumChanceToPlaceHerb = (int)(oneInXMaximumChanceToPlaceHerb * 2.8);
            double chanceAdjustmentBasedOnWorldSize = Utils.Clamp((double)Main.maxTilesX / 4200D - 1D, 0D, 1D);
            int herbPlacementChance = (int)Utils.Lerp(oneInXMaximumChanceToPlaceHerb, oneInXMinimumChanceToPlaceHerb, chanceAdjustmentBasedOnWorldSize);

            for (int herbPlacementIndex = 0; (double)herbPlacementIndex < (double)(Main.maxTilesX * Main.maxTilesY) * herbGrowthRateSurface; herbPlacementIndex++)
            {
                // Call this again to give effectively double the chance for new herbs to be placed in the world
                if (Main.rand.NextBool(herbPlacementChance))
                    WorldGen.PlantAlch();

                // Surface-only herb growth
                // Increase the growth rate of herbs in planter boxes
                // Slightly increase the growth rate of herbs not in planter boxes
                int herbGrowthRangeX = WorldGen.genRand.Next(10, Main.maxTilesX - 10);
                int herbGrowthRangeY = WorldGen.genRand.Next(10, (int)Main.worldSurface - 1);
                if (Main.tileAlch[Main.tile[herbGrowthRangeX, herbGrowthRangeY].TileType])
                {
                    // If you use the wrong planter box you get fuck all
                    if (Main.tile[herbGrowthRangeX, herbGrowthRangeY + 1].TileType == TileID.PlanterBox)
                    {
                        int herbType = Main.tile[herbGrowthRangeX, herbGrowthRangeY].TileFrameX / 18;
                        int planterBoxType = Main.tile[herbGrowthRangeX, herbGrowthRangeY + 1].TileFrameY / 18;
                        if (IsCorrectPlanterBox(herbType: herbType, planterBoxType: planterBoxType) && WorldGen.genRand.NextBool(oneInXChanceToBoostHerbGrowthRate_PlanterBox))
                            WorldGen.GrowAlch(herbGrowthRangeX, herbGrowthRangeY);
                    }
                    else if (WorldGen.genRand.NextBool(oneInXChanceToBoostHerbGrowthRate))
                        WorldGen.GrowAlch(herbGrowthRangeX, herbGrowthRangeY);
                }
            }

            if (Main.remixWorld)
            {
                // Remix seed checks the entire world at once for herb growth
                for (int herbGrowthIndex = 0; (double)herbGrowthIndex < (double)(Main.maxTilesX * Main.maxTilesY) * remixWorldHerbGrowthRate; herbGrowthIndex++)
                {
                    // Increase the growth rate of herbs in planter boxes
                    // Slightly increase the growth rate of herbs not in planter boxes
                    int herbGrowthRangeX = WorldGen.genRand.Next(10, Main.maxTilesX - 10);
                    int herbGrowthRangeY = WorldGen.genRand.Next((int)Main.worldSurface - 1, Main.maxTilesY - 20);
                    if (Main.tileAlch[Main.tile[herbGrowthRangeX, herbGrowthRangeY].TileType])
                    {
                        // If you use the wrong planter box you get fuck all
                        if (Main.tile[herbGrowthRangeX, herbGrowthRangeY + 1].TileType == TileID.PlanterBox)
                        {
                            int herbType = Main.tile[herbGrowthRangeX, herbGrowthRangeY].TileFrameX / 18;
                            int planterBoxType = Main.tile[herbGrowthRangeX, herbGrowthRangeY + 1].TileFrameY / 18;
                            if (IsCorrectPlanterBox(herbType: herbType, planterBoxType: planterBoxType) && WorldGen.genRand.NextBool(oneInXChanceToBoostHerbGrowthRate_PlanterBox))
                                WorldGen.GrowAlch(herbGrowthRangeX, herbGrowthRangeY);
                        }
                        else if (WorldGen.genRand.NextBool(oneInXChanceToBoostHerbGrowthRate))
                            WorldGen.GrowAlch(herbGrowthRangeX, herbGrowthRangeY);
                    }
                }
            }
            else
            {
                // Underground-only herb growth
                for (int herbGrowthIndex = 0; (double)herbGrowthIndex < (double)(Main.maxTilesX * Main.maxTilesY) * herbGrowthRateUnderground; herbGrowthIndex++)
                {
                    // Increase the growth rate of herbs in planter boxes
                    // Slightly increase the growth rate of herbs not in planter boxes
                    int herbGrowthRangeX = WorldGen.genRand.Next(10, Main.maxTilesX - 10);
                    int herbGrowthRangeY = WorldGen.genRand.Next((int)Main.worldSurface - 1, Main.maxTilesY - 20);
                    if (Main.tileAlch[Main.tile[herbGrowthRangeX, herbGrowthRangeY].TileType])
                    {
                        // If you use the wrong planter box you get fuck all
                        if (Main.tile[herbGrowthRangeX, herbGrowthRangeY + 1].TileType == TileID.PlanterBox)
                        {
                            int herbType = Main.tile[herbGrowthRangeX, herbGrowthRangeY].TileFrameX / 18;
                            int planterBoxType = Main.tile[herbGrowthRangeX, herbGrowthRangeY + 1].TileFrameY / 18;
                            if (IsCorrectPlanterBox(herbType: herbType, planterBoxType: planterBoxType) && WorldGen.genRand.NextBool(oneInXChanceToBoostHerbGrowthRate_PlanterBox))
                                WorldGen.GrowAlch(herbGrowthRangeX, herbGrowthRangeY);
                        }
                        else if (WorldGen.genRand.NextBool(oneInXChanceToBoostHerbGrowthRate))
                            WorldGen.GrowAlch(herbGrowthRangeX, herbGrowthRangeY);
                    }
                }
            }

            // 2MAY2025: Lucille: By default, the assumption that these values are within a natural min/max relationship with searchBottom being greater than searchTop is sensible.
            // However, in subworlds where these lines may be placed arbitrarily (such as shoving the surface line near the bottom of the world), this no longer holds true, and results
            // in the logic below causing a storm of minValue >= maxValue exceptions.
            int searchTop = (int)Main.worldSurface - 1;
            int searchBottom = Main.maxTilesY - 20;
            if (searchBottom <= searchTop)
                return;

            int l = 0;
            float mult2 = (float)(1.5E-05f * worldUpdateRate);
            while (l < Main.maxTilesX * Main.maxTilesY * mult2)
            {
                int x = WorldGen.genRand.Next(10, Main.maxTilesX - 10);
                int y = WorldGen.genRand.Next(searchTop, searchBottom);

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
                                                if (Main.tile[x, y2].TileType == TileID.PlanteraBulb && Main.dedServ)
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
                                                if (Main.tile[x, y2].TileType == TileID.LifeFruit && Main.dedServ)
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
                            if (tileType == TileType<Navystone>())
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
                                    bool isSunkenSeaTile = tileType == TileType<Navystone>() || tileType == TileType<SeaPrism>();
                                    bool meetsAdditionalGrowConditions = tile.Slope == SlopeType.Solid && !tile.IsHalfBlock && tile.LiquidType != LiquidID.Lava;

                                    if (growTile && meetsAdditionalGrowConditions)
                                    {
                                        int tileType2 = TileType<SeaPrismCrystals>();

                                        if (tileType == TileType<Voidstone>())
                                            tileType2 = TileType<LumenylCrystals>();

                                        if (tileType == TileType<Shellstone>())
                                            tileType2 = TileType<SmallCorals>();

                                        bool canPlaceBasedOnAttached = true;
                                        if (tileType2 == TileType<SeaPrismCrystals>() && !isSunkenSeaTile)
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

                                            if (Main.dedServ)
                                                NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                                        }
                                    }
                                }
                            }
                        }

                        if (growthTile.LiquidAmount == 0 && y > Main.UnderworldLayer)
                        {
                            bool isCragsTile = tileType == TileType<BrimstoneSlag>() ||
                                tileType == TileType<BrimstoneSlab>() ||
                                tileType == TileType<ScorchedRemains>() ||
                                tileType == TileType<ScorchedRemainsGrass>() ||
                                tileType == TileType<ScorchedBone>();

                            int wallType = Main.tile[x, y2].WallType;
                            bool isCragHouseWall = wallType == WallType<BrimstoneSlagWall>() ||
                                wallType == WallType<BrimstoneSlabWall>() ||
                                wallType == WallType<ScorchedBoneWall>() ||
                                wallType == WallType<SmoothBrimstoneSlagWall>();

                            if (isCragsTile && isCragHouseWall && Main.tile[x, y2].LiquidAmount == 0)
                            {
                                // Lilies of Finality post-Yharon.
                                if (WorldGen.genRand.NextBool(20) && DownedBossSystem.downedYharon)
                                {
                                    ushort tileTypeToPlace = (ushort)TileType<LiliesOfFinalityTile>();
                                    int tileTypeToPlaceThickness = 3;
                                    bool placeLilies = true;

                                    // Apparently this is a reference!
                                    int minDistanceFromOtherLilies = 66;

                                    for (int k = x - minDistanceFromOtherLilies; k < x + minDistanceFromOtherLilies; k += 2)
                                    {
                                        for (int m = y - minDistanceFromOtherLilies; m < y + minDistanceFromOtherLilies; m += 2)
                                        {
                                            if (k > tileTypeToPlaceThickness && k < Main.maxTilesX - tileTypeToPlaceThickness && m > tileTypeToPlaceThickness && m < Main.maxTilesY - tileTypeToPlaceThickness && Main.tile[k, m].HasTile && Main.tile[k, m].TileType == tileTypeToPlace)
                                            {
                                                placeLilies = false;
                                                break;
                                            }
                                        }
                                    }

                                    if (placeLilies)
                                    {
                                        if (x < tileTypeToPlaceThickness || x > Main.maxTilesX - tileTypeToPlaceThickness || y2 < tileTypeToPlaceThickness || y2 > Main.maxTilesY - tileTypeToPlaceThickness)
                                            return;

                                        bool placeTile = true;
                                        for (int i2 = x - 1; i2 < x + 2; i2++)
                                        {
                                            for (int j3 = y2 - 2; j3 < y2 + 1; j3++)
                                            {
                                                if (Main.tile[i2, j3] == null)
                                                    return;

                                                if (Main.tile[i2, j3].HasTile)
                                                    placeTile = false;
                                            }

                                            if (Main.tile[i2, y2 + 1] == null)
                                                return;

                                            if (!WorldGen.SolidTile2(i2, y2 + 1))
                                                placeTile = false;
                                        }

                                        if (placeTile)
                                        {
                                            WorldGen.PlaceObject(x, y2, tileTypeToPlace, true);
                                            NetMessage.SendObjectPlacement(-1, x, y2, tileTypeToPlace, 0, 0, -1, -1);
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
            if (tileType == TileType<LumenylCrystals>() && !DownedBossSystem.downedLeviathan)
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

        public static bool IsCorrectPlanterBox(int herbType, int planterBoxType)
        {
            bool usingCorrectPlanterBox = false;
            switch (herbType)
            {
                default:
                    break;

                case (int)HerbType.Daybloom:
                    if (planterBoxType == (int)PlanterBoxType.Daybloom)
                        usingCorrectPlanterBox = true;
                    break;

                case (int)HerbType.Moonglow:
                    if (planterBoxType == (int)PlanterBoxType.Moonglow)
                        usingCorrectPlanterBox = true;
                    break;

                case (int)HerbType.Blinkroot:
                    if (planterBoxType == (int)PlanterBoxType.Blinkroot)
                        usingCorrectPlanterBox = true;
                    break;

                case (int)HerbType.Deathweed:
                    if (planterBoxType == (int)PlanterBoxType.Deathweed || planterBoxType == (int)PlanterBoxType.DeathweedCrimson)
                        usingCorrectPlanterBox = true;
                    break;

                case (int)HerbType.Waterleaf:
                    if (planterBoxType == (int)PlanterBoxType.Waterleaf)
                        usingCorrectPlanterBox = true;
                    break;

                case (int)HerbType.Fireblossom:
                    if (planterBoxType == (int)PlanterBoxType.Fireblossom)
                        usingCorrectPlanterBox = true;
                    break;

                case (int)HerbType.Shiverthorn:
                    if (planterBoxType == (int)PlanterBoxType.Shiverthorn)
                        usingCorrectPlanterBox = true;
                    break;
            }

            return usingCorrectPlanterBox;
        }
        #endregion

        #region Handle Old Man Spawn
        public static void TrySpawnOldMan()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || NPC.downedBoss3 || Main.dayTime)
                return;

            if (NPC.AnyNPCs(NPCID.OldMan))
                return;

            if (NPC.AnyNPCs(NPCID.SkeletronHead))
                return;

            int oldMan = NPC.NewNPC(NPC.GetSource_TownSpawn(), Main.dungeonX * 16 + 8, Main.dungeonY * 16, NPCID.OldMan);
            Main.npc[oldMan].homeless = false;
            Main.npc[oldMan].homeTileX = Main.dungeonX;
            Main.npc[oldMan].homeTileY = Main.dungeonY;
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
            if (Main.netMode == NetmodeID.MultiplayerClient || !(modPlayer.ZoneAbyss || Main.zenithWorld) || !player.chaosState || player.dead || BossRushEvent.BossRushActive)
                return;

            bool adultWyrmAlive = CalamityGlobalNPC.adultEidolonWyrmHead != -1 && Main.npc[CalamityGlobalNPC.adultEidolonWyrmHead].active;
            if (!adultWyrmAlive)
            {
                CalamityUtils.BroadcastLocalizedText("Mods.CalamityMod.Status.Boss.PrimordialWyrmSpawn", Color.Cyan);
                NPC.SpawnOnPlayer(player.whoAmI, NPCType<PrimordialWyrmHead>());
            }
        }
        #endregion
    }
}
