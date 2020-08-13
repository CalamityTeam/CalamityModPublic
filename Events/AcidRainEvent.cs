using CalamityMod.NPCs.AcidRain;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;

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

        public static readonly Color TextColor = new Color(115, 194, 147);

        public const int InvasionNoKillPersistTime = 14400; // How long the invasion persists, in frames, if nothing is killed.

        public const float BloodwormSpawnRate = 0.1f;

        // Not a readonly collection so that if anyone else wants to add stuff in here with their own mod, they can.
        public static Dictionary<int, AcidRainSpawnData> PossibleEnemiesPreHM = new Dictionary<int, AcidRainSpawnData>()
        {
            { ModContent.NPCType<NuclearToad>(), new AcidRainSpawnData(1, 0.75f, AcidRainSpawnRequirement.Anywhere) },
            { ModContent.NPCType<AcidEel>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<Radiator>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<Skyfin>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<WaterLeech>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) }
        };

        // Note: Irradiated Slimes spawn naturally
        public static Dictionary<int, AcidRainSpawnData> PossibleEnemiesAS = new Dictionary<int, AcidRainSpawnData>()
        {
            { ModContent.NPCType<Radiator>(), new AcidRainSpawnData(0, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<AcidEel>(), new AcidRainSpawnData(0, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<NuclearToad>(), new AcidRainSpawnData(0, 0.75f, AcidRainSpawnRequirement.Anywhere) },
            { ModContent.NPCType<FlakCrab>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<Orthocera>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<Skyfin>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<Trilobite>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<WaterLeech>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<SulfurousSkater>(), new AcidRainSpawnData(1, 0.4f, AcidRainSpawnRequirement.Anywhere) }
        };

        public static Dictionary<int, AcidRainSpawnData> PossibleEnemiesPolter = new Dictionary<int, AcidRainSpawnData>()
        {
            { ModContent.NPCType<Radiator>(), new AcidRainSpawnData(0, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<AcidEel>(), new AcidRainSpawnData(0, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<NuclearToad>(), new AcidRainSpawnData(0, 0.75f, AcidRainSpawnRequirement.Anywhere) },
            { ModContent.NPCType<FlakCrab>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<Orthocera>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<Skyfin>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<Trilobite>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<WaterLeech>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<SulfurousSkater>(), new AcidRainSpawnData(1, 0.4f, AcidRainSpawnRequirement.Anywhere) },
            { ModContent.NPCType<GammaSlime>(), new AcidRainSpawnData(1, 1f, AcidRainSpawnRequirement.Anywhere) }
        };

        public static Dictionary<int, AcidRainSpawnData> PossibleMinibossesAS = new Dictionary<int, AcidRainSpawnData>()
        {
            { ModContent.NPCType<CragmawMire>(), new AcidRainSpawnData(5, 0.08f, AcidRainSpawnRequirement.Water) }
        };

        public static Dictionary<int, AcidRainSpawnData> PossibleMinibossesPolter = new Dictionary<int, AcidRainSpawnData>()
        {
            { ModContent.NPCType<CragmawMire>(), new AcidRainSpawnData(4, 0.08f, AcidRainSpawnRequirement.Water) },
            { ModContent.NPCType<NuclearTerror>(), new AcidRainSpawnData(8, 0.05f, AcidRainSpawnRequirement.Anywhere) }
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
        public static void BroadcastEventText(string localizationKey)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue(localizationKey), TextColor);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromKey(localizationKey), TextColor);
            }
        }
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
                if (CalamityWorld.downedPolterghast)
                    return (int)(140 * Math.Log(playerCount + Math.E - 1));
                else if (CalamityWorld.downedAquaticScourge)
                    return (int)(115 * Math.Log(playerCount + Math.E - 1));
                else
                    return (int)(90 * Math.Log(playerCount + Math.E - 1));
            }
        }
        /// <summary>
        /// Attempts to start the Acid Rain event. Will fail if there is another invasion or boss rush going on. It will also fail if the EoC, WoF, or AS has not been killed yet.
        /// </summary>
        public static void TryStartEvent(bool forceRain = false)
        {
            if (CalamityWorld.rainingAcid || (!NPC.downedBoss1 && !Main.hardMode && !CalamityWorld.downedAquaticScourge) || BossRushEvent.BossRushActive)
                return;
            int playerCount = 0;
            for (int i = 0; i < Main.player.Length; i++)
            {
                if (Main.player[i].active)
                {
                    playerCount++;
                }
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
                    Main.numCloudsTemp = Main.cloudLimit;
                    Main.numClouds = Main.numCloudsTemp;
                    Main.windSpeedTemp = 0.72f;
                    Main.windSpeedSet = Main.windSpeedTemp;
                    Main.weatherCounter = 60 * 60 * 10; // 10 minutes of rain. Remember, once the rain goes away, so does the invasion.
                    Main.rainTime = Main.weatherCounter;
                    Main.maxRaining = 0.89f;
                    CalamityWorld.forcedDownpourWithTear = true;
                    CalamityMod.UpdateServerBoolean();
                }
                CalamityWorld.triedToSummonOldDuke = false;
                CalamityWorld.timeSinceAcidRainKill = 0; // Reset the kill cooldown, just in case.
            }

            UpdateInvasion();
            BroadcastEventText("Mods.CalamityMod.AcidRainStart"); // A toxic downpour falls over the wasteland seas!
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
                    CalamityWorld.acidRainExtraDrawTime = 40;

                    // Turn off the rain from the event
                    Main.numCloudsTemp = Main.rand.Next(5, 20 + 1);
                    Main.numClouds = Main.numCloudsTemp;
                    Main.windSpeedTemp = Main.rand.NextFloat(0.04f, 0.25f);
                    Main.windSpeedSet = Main.windSpeedTemp;
                    Main.maxRaining = 0f;
                    if (win)
                    {
                        CalamityWorld.downedEoCAcidRain = true;
                        CalamityWorld.downedAquaticScourgeAcidRain = CalamityWorld.downedAquaticScourge;
                    }
                    CalamityWorld.triedToSummonOldDuke = false;
                    CalamityMod.StopRain();
                }
                CalamityMod.UpdateServerBoolean();

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
                    netMessage.Write((byte)CalamityModMessageType.AcidRainUIDrawFadeSync);
                    netMessage.Write(CalamityWorld.acidRainExtraDrawTime);
                    netMessage.Send();
                }
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = CalamityMod.Instance.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.AcidRainOldDukeSummonSync);
                    netMessage.Write(CalamityWorld.triedToSummonOldDuke);
                    netMessage.Send();
                }
            }
        }
    }
}
