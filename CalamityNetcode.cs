using CalamityMod.Events;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Providence;
using CalamityMod.TileEntities;
using CalamityMod.World;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod
{
    public class CalamityNetcode
    {
        public static void HandlePacket(BinaryReader reader, int whoAmI)
        {
            try
            {
                CalamityModMessageType msgType = (CalamityModMessageType)reader.ReadByte();
                switch (msgType)
                {
                    case CalamityModMessageType.MeleeLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleLevels(reader, 0);
                        break;
                    case CalamityModMessageType.RangedLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleLevels(reader, 1);
                        break;
                    case CalamityModMessageType.MagicLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleLevels(reader, 2);
                        break;
                    case CalamityModMessageType.SummonLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleLevels(reader, 3);
                        break;
                    case CalamityModMessageType.RogueLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleLevels(reader, 4);
                        break;
                    case CalamityModMessageType.ExactMeleeLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleExactLevels(reader, 0);
                        break;
                    case CalamityModMessageType.ExactRangedLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleExactLevels(reader, 1);
                        break;
                    case CalamityModMessageType.ExactMagicLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleExactLevels(reader, 2);
                        break;
                    case CalamityModMessageType.ExactSummonLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleExactLevels(reader, 3);
                        break;
                    case CalamityModMessageType.ExactRogueLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleExactLevels(reader, 4);
                        break;
                    case CalamityModMessageType.StressSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleStress(reader);
                        break;
                    case CalamityModMessageType.BossRushStage:
                        int stage = reader.ReadInt32();
                        BossRushEvent.BossRushStage = stage;
                        break;
                    case CalamityModMessageType.AdrenalineSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleAdrenaline(reader);
                        break;
                    case CalamityModMessageType.TeleportPlayer:
                        Main.player[reader.ReadInt32()].Calamity().HandleTeleport(reader.ReadInt32(), true, whoAmI);
                        break;
                    case CalamityModMessageType.DoGCountdownSync:
                        int countdown = reader.ReadInt32();
                        CalamityWorld.DoGSecondStageCountdown = countdown;
                        break;
                    case CalamityModMessageType.BossSpawnCountdownSync:
                        int countdown2 = reader.ReadInt32();
                        CalamityWorld.bossSpawnCountdown = countdown2;
                        break;
                    case CalamityModMessageType.BRHostileProjKillSync:
                        int countdown3 = reader.ReadInt32();
                        CalamityWorld.bossRushHostileProjKillCounter = countdown3;
                        break;
                    case CalamityModMessageType.DeathBossSpawnCountdownSync:
                        int countdown4 = reader.ReadInt32();
                        CalamityWorld.deathBossSpawnCooldown = countdown4;
                        break;
                    case CalamityModMessageType.ArmoredDiggerCountdownSync:
                        int countdown5 = reader.ReadInt32();
                        CalamityWorld.ArmoredDiggerSpawnCooldown = countdown5;
                        break;
                    case CalamityModMessageType.BossTypeSync:
                        int type = reader.ReadInt32();
                        CalamityWorld.bossType = type;
                        break;
                    case CalamityModMessageType.DeathCountSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleDeathCount(reader);
                        break;
                    case CalamityModMessageType.DeathModeUnderworldTimeSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleDeathModeUnderworldTime(reader);
                        break;
                    case CalamityModMessageType.DeathModeBlizzardTimeSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleDeathModeBlizzardTime(reader);
                        break;
                    case CalamityModMessageType.RevengeanceBoolSync:
                        bool revActive = reader.ReadBoolean();
                        CalamityWorld.revenge = revActive;
                        break;
                    case CalamityModMessageType.DeathBoolSync:
                        bool deathActive = reader.ReadBoolean();
                        CalamityWorld.death = deathActive;
                        break;
                    case CalamityModMessageType.DefiledBoolSync:
                        bool defiledActive = reader.ReadBoolean();
                        CalamityWorld.defiled = defiledActive;
                        break;
                    case CalamityModMessageType.IronHeartBoolSync:
                        bool ironHeartActive = reader.ReadBoolean();
                        CalamityWorld.ironHeart = ironHeartActive;
                        break;
                    case CalamityModMessageType.ArmageddonBoolSync:
                        bool armaActive = reader.ReadBoolean();
                        CalamityWorld.armageddon = armaActive;
                        break;
                    case CalamityModMessageType.DemonTrophyBoolSync:
                        bool demonModeBoost = reader.ReadBoolean();
                        CalamityWorld.demonMode = demonModeBoost;
                        break;
                    case CalamityModMessageType.NPCRegenerationSync:
                        byte npcIndex = reader.ReadByte();
                        Main.npc[npcIndex].lifeRegen = reader.ReadInt32();
                        break;
                    case CalamityModMessageType.AcidRainSync:
                        CalamityWorld.rainingAcid = reader.ReadBoolean();
                        CalamityWorld.acidRainPoints = reader.ReadInt32();
                        CalamityWorld.timeSinceAcidRainKill = reader.ReadInt32();
                        break;
                    case CalamityModMessageType.AcidRainUIDrawFadeSync:
                        CalamityWorld.acidRainExtraDrawTime = reader.ReadInt32();
                        break;
                    case CalamityModMessageType.AcidRainOldDukeSummonSync:
                        CalamityWorld.triedToSummonOldDuke = reader.ReadBoolean();
                        break;
                    case CalamityModMessageType.GaelsGreatswordSwingSync:
                        byte playerIndex = reader.ReadByte();
                        Main.player[playerIndex].Calamity().gaelSwipes = reader.ReadInt32();
                        break;
                    case CalamityModMessageType.SpawnSuperDummy:
                        int x = reader.ReadInt32();
                        int y = reader.ReadInt32();
                        // Not strictly necessary, but helps prevent unnecessary packetstorm in MP
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.NewNPC(x, y, ModContent.NPCType<SuperDummyNPC>());
                        break;
                    case CalamityModMessageType.PowerCellFactory:
                        TEPowerCellFactory.ReadSyncPacket(reader);
                        break;
                    case CalamityModMessageType.ChargingStationStandard:
                        TEChargingStation.ReadSyncPacket(reader);
                        break;
                    case CalamityModMessageType.ChargingStationItemChange:
                        TEChargingStation.ReadItemSyncPacket(reader);
                        break;
                    case CalamityModMessageType.DraedonFieldGeneratorSync:
                        int entityID2 = reader.ReadInt32();
                        (TileEntity.ByID[entityID2] as TEDraedonFieldGenerator).Time = reader.ReadInt32();
                        (TileEntity.ByID[entityID2] as TEDraedonFieldGenerator).ActiveTimer = reader.ReadInt32();
                        break;
                    case CalamityModMessageType.SyncCalamityNPCAIArray:
                        byte npcIndex2 = reader.ReadByte();
                        Main.npc[npcIndex2].Calamity().newAI[0] = reader.ReadSingle();
                        Main.npc[npcIndex2].Calamity().newAI[1] = reader.ReadSingle();
                        Main.npc[npcIndex2].Calamity().newAI[2] = reader.ReadSingle();
                        Main.npc[npcIndex2].Calamity().newAI[3] = reader.ReadSingle();
                        break;
                    case CalamityModMessageType.ProvidenceDyeConditionSync:
                        byte npcIndex3 = reader.ReadByte();
                        (Main.npc[npcIndex3].modNPC as Providence).hasTakenDaytimeDamage = reader.ReadBoolean();
                        break;
                    case CalamityModMessageType.PSCChallengeSync:
                        byte npcIndex4 = reader.ReadByte();
                        (Main.npc[npcIndex4].modNPC as Providence).challenge = reader.ReadBoolean();
                        break;
                    default:
                        CalamityMod.Instance.Logger.Error($"Failed to parse Calamity packet: No Calamity packet exists with ID {msgType}.");
                        break;
                }
            }
            catch (Exception e)
            {
                if (e is EndOfStreamException eose)
                    CalamityMod.Instance.Logger.Error("Failed to parse Calamity packet: Packet was too short, missing data, or otherwise corrupt.", eose);
                else if (e is ObjectDisposedException ode)
                    CalamityMod.Instance.Logger.Error("Failed to parse Calamity packet: Packet reader disposed or destroyed.", ode);
                else if (e is IOException ioe)
                    CalamityMod.Instance.Logger.Error("Failed to parse Calamity packet: An unknown I/O error occurred.", ioe);
                else
                    throw e; // this either will crash the game or be caught by TML's packet policing
            }
        }

        public static void SyncWorld()
        {
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }
    }

    public enum CalamityModMessageType : byte
    {
        MeleeLevelSync,
        RangedLevelSync,
        MagicLevelSync,
        SummonLevelSync,
        RogueLevelSync,
        ExactMeleeLevelSync,
        ExactRangedLevelSync,
        ExactMagicLevelSync,
        ExactSummonLevelSync,
        ExactRogueLevelSync,
        StressSync,
        AdrenalineSync,
        TeleportPlayer,
        BossRushStage,
        DoGCountdownSync,
        BossSpawnCountdownSync,
        BRHostileProjKillSync,
        ArmoredDiggerCountdownSync,
        BossTypeSync,
        DeathCountSync,
        RevengeanceBoolSync,
        DeathBoolSync,
        DefiledBoolSync,
        IronHeartBoolSync,
        ArmageddonBoolSync,
        DemonTrophyBoolSync,
        NPCRegenerationSync,
        DeathModeUnderworldTimeSync,
        DeathModeBlizzardTimeSync,
        DeathBossSpawnCountdownSync,
        AcidRainSync,
        AcidRainUIDrawFadeSync,
        AcidRainOldDukeSummonSync,
        GaelsGreatswordSwingSync,
        SpawnSuperDummy,
        SyncCalamityNPCAIArray,
        ProvidenceDyeConditionSync, // We shouldn't fucking need this. Die in a hole, Multiplayer.
        PSCChallengeSync, // See above
        DraedonFieldGeneratorSync,

        // These message types were written by Ozz. They are Ozz's working tile entity netcode. Do not touch them.
        PowerCellFactory,
        ChargingStationStandard,
        ChargingStationItemChange
    }
}
