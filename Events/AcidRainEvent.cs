using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Events
{
    public enum AcidRainSpawnRequirement
    {
        Water,
        Land,
        Anywhere
    }

    public struct AcidRainSpawnData
    {
        public int InvasionContributionPoints { get; set; }
        public float SpawnRate { get; set; }
        public AcidRainSpawnRequirement SpawnRequirement { get; set; }
        public AcidRainSpawnData(int totalPoints, float spawnRate, AcidRainSpawnRequirement spawnRequirement)
        {
            InvasionContributionPoints = totalPoints;
            SpawnRate = spawnRate;
            SpawnRequirement = spawnRequirement;
        }
    }

    public class AcidRainEvent
    {
        public static int MaxNuclearToadCount = 5; // To prevent the things from spamming. This happens frequently in pre-HM.

        // A partially bright pale-ish cyan with a hint of yellow.

        public static readonly Color TextColor = new(115, 194, 147);

        // How long the invasion persists, in frames, if nothing is killed.
        public const int InvasionNoKillPersistTime = 9000;

        public const float BloodwormSpawnRate = 0.1f;

        // Not a readonly collection so that if anyone else wants to add stuff in here with their own mod, they can.
        public static Dictionary<int, AcidRainSpawnData> PossibleEnemiesPreHM = new()
        {
            { ModContent.NPCType<NuclearToad>(), new AcidRainSpawnData(1, 0.75f, AcidRainSpawnRequirement.Anywhere) },
            { ModContent.NPCType<AcidEel>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<Radiator>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<Skyfin>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<WaterLeech>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) }
        };

        // Note: Irradiated Slimes spawn naturally
        public static Dictionary<int, AcidRainSpawnData> PossibleEnemiesAS = new()
        {
            { ModContent.NPCType<Radiator>(), new AcidRainSpawnData(0, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<AcidEel>(), new AcidRainSpawnData(0, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<NuclearToad>(), new AcidRainSpawnData(0, 0.75f, AcidRainSpawnRequirement.Anywhere) },
            { ModContent.NPCType<FlakCrab>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<Orthocera>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<Skyfin>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<Trilobite>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<WaterLeech>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<SulphurousSkater>(), new AcidRainSpawnData(1, 0.4f, AcidRainSpawnRequirement.Anywhere) }
        };

        public static Dictionary<int, AcidRainSpawnData> PossibleEnemiesPolter = new()
        {
            { ModContent.NPCType<Radiator>(), new AcidRainSpawnData(0, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<AcidEel>(), new AcidRainSpawnData(0, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<NuclearToad>(), new AcidRainSpawnData(0, 0.75f, AcidRainSpawnRequirement.Anywhere) },
            { ModContent.NPCType<FlakCrab>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<Orthocera>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<Skyfin>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<Trilobite>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<WaterLeech>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<SulphurousSkater>(), new AcidRainSpawnData(1, 0.4f, AcidRainSpawnRequirement.Anywhere) },
            { ModContent.NPCType<GammaSlime>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Anywhere) }
        };

        public static Dictionary<int, AcidRainSpawnData> PossibleMinibossesAS = new()
        {
            { ModContent.NPCType<CragmawMire>(), new AcidRainSpawnData(5, 0.08f, AcidRainSpawnRequirement.Water) }
        };

        public static Dictionary<int, AcidRainSpawnData> PossibleMinibossesPolter = new()
        {
            { ModContent.NPCType<CragmawMire>(), new AcidRainSpawnData(4, 0.08f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<Mauler>(), new AcidRainSpawnData(4, 0.07f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<NuclearTerror>(), new AcidRainSpawnData(8, 0.045f, AcidRainSpawnRequirement.Anywhere) }
        };

        public static readonly List<int> AllMinibosses = PossibleMinibossesAS.Select(miniboss => miniboss.Key).ToList().Concat(PossibleMinibossesPolter.Select(miniboss => miniboss.Key)).Distinct().ToList();

        public static bool AnyRainMinibosses
        {
            get
            {
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    if (Main.npc[i].active && (PossibleMinibossesAS.Select(miniboss => miniboss.Key).Contains(Main.npc[i].type) ||
                        PossibleMinibossesPolter.Select(miniboss => miniboss.Key).Contains(Main.npc[i].type)))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Broadcasts some text from a given localization key.
        /// </summary>
        /// <param name="localizationKey">The key to write</param>
        public static void BroadcastEventText(string localizationKey) => CalamityUtils.DisplayLocalizedText(localizationKey, TextColor);
        public static int NeededEnemyKills
        {
            get
            {
                int playerCount = 0;
                for (int i = 0; i < Main.player.Length; i++)
                {
                    if (Main.player[i].active)
                    {
                        playerCount++;
                    }
                }
                if (DownedBossSystem.downedPolterghast)
                    return (int)(Math.Log(playerCount + Math.E - 1) * 170f);
                else if (DownedBossSystem.downedAquaticScourge)
                    return (int)(Math.Log(playerCount + Math.E - 1) * 135f);
                else
                    return (int)(Math.Log(playerCount + Math.E - 1) * 110f);
            }
        }
        /// <summary>
        /// Attempts to start the Acid Rain event. Will fail if there is another invasion or boss rush going on. It will also fail if the EoC, WoF, or AS has not been killed yet.
        /// </summary>
        public static void TryStartEvent(bool forceRain = false)
        {
            if (CalamityWorld.rainingAcid || (!NPC.downedBoss1 && !Main.hardMode && !DownedBossSystem.downedAquaticScourge) || BossRushEvent.BossRushActive)
                return;

            int playerCount = 0;
            for (int i = 0; i < Main.player.Length; i++)
            {
                if (Main.player[i].active)
                    playerCount++;
            }

            if (playerCount > 0)
            {
                CalamityWorld.rainingAcid = true;
                CalamityWorld.acidRainPoints = NeededEnemyKills;

                // Make it rain normally
                if (forceRain)
                {
                    Main.raining = true;
                    Main.cloudBGActive = 1f;
                    Main.numCloudsTemp = Main.maxClouds;
                    Main.numClouds = Main.numCloudsTemp;
                    Main.windSpeedCurrent = 0.72f;
                    Main.windSpeedTarget = Main.windSpeedCurrent;
                    Main.weatherCounter = 60 * 60 * 10; // 10 minutes of rain. Remember, once the rain goes away, so does the invasion.
                    Main.rainTime = Main.weatherCounter;
                    Main.maxRaining = 0.89f;
                    CalamityWorld.forcedDownpourWithTear = true;
                    CalamityNetcode.SyncWorld();
                }
                CalamityWorld.triedToSummonOldDuke = false;
                CalamityWorld.timeSinceAcidRainKill = 0; // Reset the kill cooldown, just in case.
            }

            UpdateInvasion();
            BroadcastEventText("Mods.CalamityMod.AcidRainStart"); // A toxic downpour falls over the wasteland seas!
        }

        /// <summary>
        /// Handles updating the entire invasion, including more misc operations, such as summoning OD's tornado.
        /// </summary>
        public static void Update()
        {
            CalamityWorld.timeSinceAcidRainKill++;
            CalamityWorld.timeSinceAcidStarted++;

            // If enough time has passed, and no enemy has been killed, end the invasion early.
            // The player almost certainly does not want to deal with it.
            if (CalamityWorld.timeSinceAcidRainKill >= AcidRainEvent.InvasionNoKillPersistTime)
            {
                CalamityWorld.acidRainPoints = 0;
                CalamityWorld.triedToSummonOldDuke = false;
                UpdateInvasion(false);
            }
            if (!CalamityWorld.startAcidicDownpour)
            {
                int sulphSeaWidth = SulphurousSea.BiomeWidth;
                for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
                {
                    Player player = Main.player[playerIndex];
                    if (!player.active)
                        continue;

                    // An artificial biome can be made, and therefore, the event could be started by an artificial biome.
                    // While fighting the event in an artificial biome is not bad, having it be started by a patch of Sulphurous Sand
                    // would definitely be strange.
                    // Because of this, this code is executed based on if the player is in the sea (based on tile count) AND position relative to the naturally generated sea.
                    bool inNaturalSeaPosition = (player.Center.X <= (sulphSeaWidth + 60f) * 16f && CalamityWorld.abyssSide) || (player.Center.X >= (Main.maxTilesX - (sulphSeaWidth + 60f)) * 16f && !CalamityWorld.abyssSide);
                    if (inNaturalSeaPosition && player.Calamity().ZoneSulphur)
                    {
                        // Makes rain pour at its maximum intensity (but only after an idiot meanders into the Sulphurous Sea)
                        // You'll never catch me, Fabs, Not when I shift into MAXIMUM OVERDRIVE!!
                        CalamityWorld.startAcidicDownpour = true;
                        CalamityNetcode.SyncWorld();
                        break;
                    }
                }
            }

            // Stop the rain if the Old Duke is present for visibility during the fight.
            if (Main.raining && NPC.AnyNPCs(ModContent.NPCType<OldDuke>()))
            {
                Main.raining = false;
                CalamityNetcode.SyncWorld();
            }

            // If the rain stops for whatever reason, end the invasion.
            // This is primarily done for compatibility, so that if another mod wants to manipulate the weather,
            // they can without having to deal with endless rain.
            if (!Main.raining && !NPC.AnyNPCs(ModContent.NPCType<OldDuke>()) && CalamityWorld.timeSinceAcidStarted > 20)
            {
                CalamityWorld.acidRainPoints = 0;
                CalamityWorld.triedToSummonOldDuke = false;
                UpdateInvasion(false);
            }
            else if (CalamityWorld.timeSinceAcidStarted < 20)
            {
                Main.raining = true;
                Main.cloudBGActive = 1f;
                Main.numCloudsTemp = Main.maxClouds;
                Main.numClouds = Main.numCloudsTemp;
                Main.windSpeedTarget = 0.72f;
                Main.windSpeedCurrent = Main.windSpeedTarget;
                Main.weatherCounter = 60 * 60 * 10; // 10 minutes of rain. Remember, once the rain goes away, so does the invasion.
                Main.rainTime = Main.weatherCounter;
                Main.maxRaining = 0.89f;
            }

            // Summon Old Duke tornado post-Polter as needed
            if (DownedBossSystem.downedPolterghast && CalamityWorld.acidRainPoints == 1)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<OldDuke>()) &&
                CalamityUtils.CountProjectiles(ModContent.ProjectileType<OverlyDramaticDukeSummoner>()) <= 0)
                {
                    if (CalamityWorld.triedToSummonOldDuke)
                    {
                        CalamityWorld.acidRainPoints = 0;
                        CalamityWorld.triedToSummonOldDuke = false;
                        UpdateInvasion(false);
                    }
                    else
                    {
                        var source = new EntitySource_WorldEvent();
                        int playerClosestToAbyss = Player.FindClosest(new Vector2(CalamityWorld.abyssSide ? 0 : Main.maxTilesX * 16, (int)Main.worldSurface), 0, 0);
                        Player closestToAbyss = Main.player[playerClosestToAbyss];
                        if (Main.netMode != NetmodeID.MultiplayerClient && Math.Abs(closestToAbyss.Center.X - (CalamityWorld.abyssSide ? 0 : Main.maxTilesX * 16)) <= 12000f)
                            Projectile.NewProjectile(source, closestToAbyss.Center + Vector2.UnitY * 160f, Vector2.Zero, ModContent.ProjectileType<OverlyDramaticDukeSummoner>(), 120, 8f, Main.myPlayer);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the invasion, checking to see if it has ended.
        /// </summary>
        public static void UpdateInvasion(bool win = true)
        {
            // If the custom invasion is up
            if (CalamityWorld.rainingAcid)
            {
                // End invasion if we've defeated all the enemies, including Old Duke
                if (CalamityWorld.acidRainPoints <= 0)
                {
                    CalamityWorld.rainingAcid = false;
                    BroadcastEventText("Mods.CalamityMod.AcidRainEnd"); // The sulphuric skies begin to clear...

                    // Turn off the rain from the event
                    Main.numCloudsTemp = Main.rand.Next(5, 20 + 1);
                    Main.numClouds = Main.numCloudsTemp;
                    Main.windSpeedCurrent = Main.rand.NextFloat(0.04f, 0.25f);
                    Main.windSpeedTarget = Main.windSpeedCurrent;
                    Main.maxRaining = 0f;
                    if (win)
                    {
                        DownedBossSystem.downedEoCAcidRain = true;
                        DownedBossSystem.downedAquaticScourgeAcidRain = DownedBossSystem.downedAquaticScourge;
                    }
                    CalamityWorld.triedToSummonOldDuke = false;
                    CalamityMod.StopRain();
                }
                CalamityNetcode.SyncWorld();

                // You will be tempted to turn this into a single if conditional.
                // Don't do this. Doing so has caused so much misery, with various things being read instead
                // of the correct thing, like booleans being mixed up in the sending and receiving process.
                // In short, leave this alone.
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = CalamityMod.Instance.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.AcidRainSync);
                    netMessage.Write(CalamityWorld.rainingAcid);
                    netMessage.Write(CalamityWorld.acidRainPoints);
                    netMessage.Write(CalamityWorld.timeSinceAcidRainKill);
                    netMessage.Send();
                }
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = CalamityMod.Instance.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.AcidRainOldDukeSummonSync);
                    netMessage.Write(CalamityWorld.triedToSummonOldDuke);
                    netMessage.Send();
                }
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = CalamityMod.Instance.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.EncounteredOldDukeSync);
                    netMessage.Write(CalamityWorld.encounteredOldDuke);
                    netMessage.Send();
                }
            }
        }

        public static void TryToStartEventNaturally()
        {
            // Attempt to start the acid rain at the 4:29AM.
            // This does not happen if a player has the Broken Water Filter in use.
            bool increasedEventChance = !DownedBossSystem.downedEoCAcidRain || (!DownedBossSystem.downedAquaticScourgeAcidRain && DownedBossSystem.downedAquaticScourge) || (!DownedBossSystem.downedBoomerDuke && DownedBossSystem.downedPolterghast);
            if ((int)Main.time == (int)(Main.nightLength - 1f) && !Main.dayTime && Main.rand.NextBool(increasedEventChance ? 3 : 300))
            {
                bool shouldNotStartEvent = false;
                for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
                {
                    if (!Main.player[playerIndex].active)
                        continue;
                    if (Main.player[playerIndex].Calamity().noStupidNaturalARSpawns)
                    {
                        shouldNotStartEvent = true;
                        break;
                    }
                }
                if (!shouldNotStartEvent)
                {
                    TryStartEvent();
                    CalamityNetcode.SyncWorld();
                }
            }

            // Attempt to force an Acid Rain immediately after the EoC is dead when someone wanders to the sea.
            if (NPC.downedBoss1 && !DownedBossSystem.downedEoCAcidRain && !CalamityWorld.forcedRainAlready)
            {
                for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
                {
                    if (Main.player[playerIndex].active && Main.player[playerIndex].Calamity().ZoneSulphur)
                    {
                        CalamityWorld.forcedRainAlready = true;
                        TryStartEvent();
                        CalamityNetcode.SyncWorld();
                    }
                }
            }

            if (CalamityWorld.forceRainTimer == 1)
            {
                TryStartEvent();
                CalamityNetcode.SyncWorld();
            }

            if (CalamityWorld.forceRainTimer > 0)
                CalamityWorld.forceRainTimer--;
        }

        public static void OnEnemyKill(NPC npc)
        {
            Dictionary<int, AcidRainSpawnData> possibleEnemies = PossibleEnemiesPreHM;

            if (DownedBossSystem.downedAquaticScourge)
                possibleEnemies = PossibleEnemiesAS;
            if (DownedBossSystem.downedPolterghast)
                possibleEnemies = PossibleEnemiesPolter;

            if (CalamityWorld.rainingAcid)
            {
                if (possibleEnemies.Select(enemy => enemy.Key).Contains(npc.type))
                {
                    CalamityWorld.acidRainPoints -= possibleEnemies[npc.type].InvasionContributionPoints;
                    if (DownedBossSystem.downedPolterghast)
                    {
                        CalamityWorld.acidRainPoints = (int)MathHelper.Max(1, CalamityWorld.acidRainPoints); // Cap at 1. The last point is for Old Duke.
                    }

                    // UpdateInvasion incorporates a world sync, so this is indeed synced as a result.
                    Main.rainTime += Main.rand.Next(240, 300 + 1); // Add some time to the rain, so that it doesn't end mid-way.
                }
                Dictionary<int, AcidRainSpawnData> possibleMinibosses = DownedBossSystem.downedPolterghast ? PossibleMinibossesPolter : PossibleMinibossesAS;
                if (possibleMinibosses.Select(miniboss => miniboss.Key).Contains(npc.type))
                {
                    CalamityWorld.acidRainPoints -= possibleMinibosses[npc.type].InvasionContributionPoints;
                    if (DownedBossSystem.downedPolterghast)
                    {
                        CalamityWorld.acidRainPoints = (int)MathHelper.Max(1, CalamityWorld.acidRainPoints); // Cap at 1. The last point is for Old Duke.
                    }

                    // UpdateInvasion incorporates a world sync, so this is indeed synced as a result.
                    Main.rainTime += Main.rand.Next(1800, 2100 + 1); // Add some time to the rain, so that it doesn't end mid-way.
                }
            }

            CalamityWorld.acidRainPoints = (int)MathHelper.Max(0, CalamityWorld.acidRainPoints); // To prevent negative completion ratios

            if (CalamityWorld.rainingAcid && DownedBossSystem.downedPolterghast &&
                npc.type == ModContent.NPCType<OldDuke>() &&
                CalamityWorld.acidRainPoints <= 2f)
            {
                CalamityWorld.triedToSummonOldDuke = false;
                CalamityWorld.acidRainPoints = 0;
            }
            CalamityWorld.timeSinceAcidRainKill = 0;
            UpdateInvasion();
        }
    }
}
