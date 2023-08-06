using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.Projectiles.Boss;
using CalamityMod.UI;
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

    public class AcidRainEvent : ModSystem
    {
        public static int MaxNuclearToadCount = 5; // To prevent the things from spamming. This happens frequently in pre-HM.

        // A partially bright pale-ish cyan with a hint of yellow.

        public static readonly Color TextColor = new(115, 194, 147);

        // How long the invasion persists, in frames, if nothing is killed.
        public const int InvasionNoKillPersistTime = 9000;

        public const float BloodwormSpawnRate = 0.1f;

        // Collections are not readonly so that if anyone else wants to add stuff to them with their own mod, they can.
        public static Dictionary<int, AcidRainSpawnData> PossibleEnemiesPreHM = new();

        // Note: Irradiated Slimes spawn naturally
        public static Dictionary<int, AcidRainSpawnData> PossibleEnemiesAS = new();

        public static Dictionary<int, AcidRainSpawnData> PossibleEnemiesPolter = new();

        public static Dictionary<int, AcidRainSpawnData> PossibleMinibossesAS = new();

        public static Dictionary<int, AcidRainSpawnData> PossibleMinibossesPolter = new();

        public static List<int> AllMinibosses => PossibleMinibossesAS.Select(miniboss => miniboss.Key).ToList().Concat(PossibleMinibossesPolter.Select(miniboss => miniboss.Key)).Distinct().ToList();

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

        public static bool AcidRainEventIsOngoing;

        public static int AccumulatedKillPoints;

        public static bool HasTriedToSummonOldDuke;

        public static bool HasStartedAcidicDownpour;

        public static bool HasBeenForceStartedByEoCDefeat;

        public static bool OldDukeHasBeenEncountered;

        public static int CountdownUntilForcedAcidRain;

        public static int TimeSinceLastAcidRainKill;

        public static int TimeSinceEventStarted;

        public static float AcidRainCompletionRatio => MathHelper.Clamp(AccumulatedKillPoints / (float)NeededEnemyKills, 0f, 1f);

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

        // Populate the enemy caches safely, after the mod's core content has been fully loaded.
        public override void OnModLoad()
        {
            PossibleEnemiesPreHM = new()
            {
                { ModContent.NPCType<NuclearToad>(), new AcidRainSpawnData(1, 0.75f, AcidRainSpawnRequirement.Anywhere) },
                { ModContent.NPCType<AcidEel>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
                { ModContent.NPCType<Radiator>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
                { ModContent.NPCType<Skyfin>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) }
            };
            PossibleEnemiesAS = new()
            {
                { ModContent.NPCType<Radiator>(), new AcidRainSpawnData(0, 1f, AcidRainSpawnRequirement.Water) },
                { ModContent.NPCType<AcidEel>(), new AcidRainSpawnData(0, 1f, AcidRainSpawnRequirement.Water) },
                { ModContent.NPCType<NuclearToad>(), new AcidRainSpawnData(0, 0.75f, AcidRainSpawnRequirement.Anywhere) },
                { ModContent.NPCType<FlakCrab>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
                { ModContent.NPCType<Orthocera>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
                { ModContent.NPCType<Skyfin>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
                { ModContent.NPCType<Trilobite>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
                { ModContent.NPCType<SulphurousSkater>(), new AcidRainSpawnData(1, 0.4f, AcidRainSpawnRequirement.Anywhere) }
            };
            PossibleEnemiesPolter = new()
            {
                { ModContent.NPCType<Radiator>(), new AcidRainSpawnData(0, 1f, AcidRainSpawnRequirement.Water) },
                { ModContent.NPCType<AcidEel>(), new AcidRainSpawnData(0, 1f, AcidRainSpawnRequirement.Water) },
                { ModContent.NPCType<NuclearToad>(), new AcidRainSpawnData(0, 0.75f, AcidRainSpawnRequirement.Anywhere) },
                { ModContent.NPCType<FlakCrab>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
                { ModContent.NPCType<Orthocera>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
                { ModContent.NPCType<Skyfin>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
                { ModContent.NPCType<Trilobite>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
                { ModContent.NPCType<SulphurousSkater>(), new AcidRainSpawnData(1, 0.4f, AcidRainSpawnRequirement.Anywhere) },
                { ModContent.NPCType<GammaSlime>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Anywhere) }
            };

            PossibleMinibossesAS = new()
            {
                { ModContent.NPCType<CragmawMire>(), new AcidRainSpawnData(5, 0.08f, AcidRainSpawnRequirement.Water) }
            };
            PossibleMinibossesPolter = new()
            {
                { ModContent.NPCType<CragmawMire>(), new AcidRainSpawnData(4, 0.08f, AcidRainSpawnRequirement.Water) },
                { ModContent.NPCType<Mauler>(), new AcidRainSpawnData(4, 0.07f, AcidRainSpawnRequirement.Water) },
                { ModContent.NPCType<NuclearTerror>(), new AcidRainSpawnData(8, 0.045f, AcidRainSpawnRequirement.Anywhere) }
            };
            BossHealthBarManager.MinibossHPBarList.AddRange(AllMinibosses);
        }

        // Clear enemy cache lists.
        public override void Unload()
        {
            PossibleEnemiesPreHM = new();
            PossibleEnemiesAS = new();
            PossibleEnemiesPolter = new();
            PossibleMinibossesAS = new();
            PossibleMinibossesPolter = new();
        }

        /// <summary>
        /// Attempts to start the Acid Rain event. Will fail if there is another invasion or boss rush going on. It will also fail if the EoC, WoF, or AS has not been killed yet.
        /// </summary>
        public static void TryStartEvent(bool forceRain = false)
        {
            if (AcidRainEventIsOngoing || (!NPC.downedBoss1 && !Main.hardMode && !DownedBossSystem.downedAquaticScourge) || BossRushEvent.BossRushActive)
                return;

            int playerCount = 0;
            for (int i = 0; i < Main.player.Length; i++)
            {
                if (Main.player[i].active)
                    playerCount++;
            }

            if (playerCount > 0)
            {
                AcidRainEventIsOngoing = true;
                AccumulatedKillPoints = NeededEnemyKills;

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
                    CalamityNetcode.SyncWorld();
                }
                HasTriedToSummonOldDuke = false;
                TimeSinceLastAcidRainKill = 0; // Reset the kill cooldown, just in case.
            }

            UpdateInvasion();
            BroadcastEventText("Mods.CalamityMod.Events.AcidRainStart"); // A toxic downpour falls over the wasteland seas!
        }

        /// <summary>
        /// Handles updating the entire invasion, including more misc operations, such as summoning OD's tornado.
        /// </summary>
        public static void Update()
        {
            TimeSinceLastAcidRainKill++;
            TimeSinceEventStarted++;

            // If enough time has passed, and no enemy has been killed, end the invasion early.
            // The player almost certainly does not want to deal with it.
            if (TimeSinceLastAcidRainKill >= InvasionNoKillPersistTime)
            {
                AccumulatedKillPoints = 0;
                HasTriedToSummonOldDuke = false;
                UpdateInvasion(false);
            }

            if (!HasStartedAcidicDownpour)
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
                    bool inNaturalSeaPosition = (player.Center.X <= (sulphSeaWidth + 60f) * 16f && Abyss.AtLeftSideOfWorld) || (player.Center.X >= (Main.maxTilesX - (sulphSeaWidth + 60f)) * 16f && !Abyss.AtLeftSideOfWorld);
                    if (inNaturalSeaPosition && player.Calamity().ZoneSulphur)
                    {
                        // Makes rain pour at its maximum intensity (but only after an idiot meanders into the Sulphurous Sea)
                        // You'll never catch me, Fabs, Not when I shift into MAXIMUM OVERDRIVE!!
                        HasStartedAcidicDownpour = true;
                        CalamityNetcode.SyncWorld();
                        break;
                    }
                }
            }

            // If the rain stops for whatever reason, end the invasion.
            // This is primarily done for compatibility, so that if another mod wants to manipulate the weather,
            // they can without having to deal with endless rain.
            if (!Main.raining && TimeSinceEventStarted > 20)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<OldDuke>()))
                {
                    AccumulatedKillPoints = 0;
                    HasTriedToSummonOldDuke = false;
                    UpdateInvasion(false);
                }
            }
            else if (TimeSinceEventStarted < 20)
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
            if (DownedBossSystem.downedPolterghast && AccumulatedKillPoints == 1)
            {
                if (CalamityUtils.CountProjectiles(ModContent.ProjectileType<OverlyDramaticDukeSummoner>()) <= 0)
                {
                    if (!NPC.AnyNPCs(ModContent.NPCType<OldDuke>()))
                    {
                        // If the Old Duke has been summoned already, that means that he was successfully killed and it's time to end the event.
                        if (HasTriedToSummonOldDuke)
                        {
                            AccumulatedKillPoints = 0;
                            HasTriedToSummonOldDuke = false;
                            UpdateInvasion(false);
                        }

                        // If not, that means he's ready to be summoned.
                        else
                        {
                            var source = new EntitySource_WorldEvent();
                            int playerClosestToAbyss = Player.FindClosest(new Vector2(Abyss.AtLeftSideOfWorld ? 0 : Main.maxTilesX * 16, (int)Main.worldSurface), 0, 0);
                            Player closestToAbyss = Main.player[playerClosestToAbyss];
                            if (Main.netMode != NetmodeID.MultiplayerClient && Math.Abs(closestToAbyss.Center.X - (Abyss.AtLeftSideOfWorld ? 0 : Main.maxTilesX * 16)) <= 12000f)
                                Projectile.NewProjectile(source, closestToAbyss.Center + Vector2.UnitY * 160f, Vector2.Zero, ModContent.ProjectileType<OverlyDramaticDukeSummoner>(), 120, 8f, Main.myPlayer);
                        }
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
            if (AcidRainEventIsOngoing)
            {
                // End invasion if we've defeated all the enemies, including Old Duke
                if (AccumulatedKillPoints <= 0)
                {
                    AcidRainEventIsOngoing = false;
                    BroadcastEventText("Mods.CalamityMod.Events.AcidRainEnd"); // The sulphuric skies begin to clear...

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
                    HasTriedToSummonOldDuke = false;
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
                    netMessage.Write(AcidRainEventIsOngoing);
                    netMessage.Write(AccumulatedKillPoints);
                    netMessage.Write(TimeSinceLastAcidRainKill);
                    netMessage.Send();
                }
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = CalamityMod.Instance.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.AcidRainOldDukeSummonSync);
                    netMessage.Write(HasTriedToSummonOldDuke);
                    netMessage.Send();
                }
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = CalamityMod.Instance.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.EncounteredOldDukeSync);
                    netMessage.Write(OldDukeHasBeenEncountered);
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
            if (NPC.downedBoss1 && !DownedBossSystem.downedEoCAcidRain && !HasBeenForceStartedByEoCDefeat)
            {
                for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
                {
                    if (Main.player[playerIndex].active && Main.player[playerIndex].Calamity().ZoneSulphur)
                    {
                        HasBeenForceStartedByEoCDefeat = true;
                        TryStartEvent();
                        CalamityNetcode.SyncWorld();
                    }
                }
            }

            if (CountdownUntilForcedAcidRain == 1)
            {
                TryStartEvent();
                CalamityNetcode.SyncWorld();
            }

            if (CountdownUntilForcedAcidRain > 0)
                CountdownUntilForcedAcidRain--;
        }

        public static void OnEnemyKill(NPC npc)
        {
            Dictionary<int, AcidRainSpawnData> possibleEnemies = PossibleEnemiesPreHM;

            if (DownedBossSystem.downedAquaticScourge)
                possibleEnemies = PossibleEnemiesAS;
            if (DownedBossSystem.downedPolterghast)
                possibleEnemies = PossibleEnemiesPolter;

            if (AcidRainEventIsOngoing)
            {
                if (possibleEnemies.Select(enemy => enemy.Key).Contains(npc.type))
                {
                    AccumulatedKillPoints -= possibleEnemies[npc.type].InvasionContributionPoints;
                    if (DownedBossSystem.downedPolterghast)
                    {
                        AccumulatedKillPoints = (int)MathHelper.Max(1, AccumulatedKillPoints); // Cap at 1. The last point is for Old Duke.
                    }

                    // UpdateInvasion incorporates a world sync, so this is indeed synced as a result.
                    Main.rainTime += Main.rand.Next(240, 300 + 1); // Add some time to the rain, so that it doesn't end mid-way.
                }
                Dictionary<int, AcidRainSpawnData> possibleMinibosses = DownedBossSystem.downedPolterghast ? PossibleMinibossesPolter : PossibleMinibossesAS;
                if (possibleMinibosses.Select(miniboss => miniboss.Key).Contains(npc.type))
                {
                    AccumulatedKillPoints -= possibleMinibosses[npc.type].InvasionContributionPoints;
                    if (DownedBossSystem.downedPolterghast)
                    {
                        AccumulatedKillPoints = (int)MathHelper.Max(1, AccumulatedKillPoints); // Cap at 1. The last point is for Old Duke.
                    }

                    // UpdateInvasion incorporates a world sync, so this is indeed synced as a result.
                    Main.rainTime += Main.rand.Next(1800, 2100 + 1); // Add some time to the rain, so that it doesn't end mid-way.
                }
            }

            AccumulatedKillPoints = (int)MathHelper.Max(0, AccumulatedKillPoints); // To prevent negative completion ratios

            if (AcidRainEventIsOngoing && DownedBossSystem.downedPolterghast && npc.type == ModContent.NPCType<OldDuke>() && AccumulatedKillPoints <= 2f)
            {
                HasTriedToSummonOldDuke = false;
                AccumulatedKillPoints = 0;
            }
            TimeSinceLastAcidRainKill = 0;
            UpdateInvasion();
        }
    }
}
