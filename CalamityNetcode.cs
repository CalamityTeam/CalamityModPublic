using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.NPCs;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Providence;
using CalamityMod.TileEntities;
using CalamityMod.World;
using Microsoft.Xna.Framework;
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
        public static void HandlePacket(Mod mod, BinaryReader reader, int whoAmI)
        {
            try
            {
                CalamityModMessageType msgType = (CalamityModMessageType)reader.ReadByte();
                switch (msgType)
                {
                    //
                    // Proficiency levels
                    //

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

                    //
                    // Player mechanic syncs
                    //

                    case CalamityModMessageType.MoveSpeedStatSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleMoveSpeedStat(reader);
                        break;
                    case CalamityModMessageType.DefenseDamageSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleDefenseDamage(reader);
                        break;
                    case CalamityModMessageType.RageSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleRage(reader);
                        break;
                    case CalamityModMessageType.AdrenalineSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleAdrenaline(reader);
                        break;
                    case CalamityModMessageType.CooldownAddition:
                        Main.player[reader.ReadInt32()].Calamity().HandleCooldownAddition(reader);
                        break;
                    case CalamityModMessageType.CooldownRemoval:
                        Main.player[reader.ReadInt32()].Calamity().HandleCooldownRemoval(reader);
                        break;
                    case CalamityModMessageType.SyncCooldownDictionary:
                        Main.player[reader.ReadInt32()].Calamity().HandleCooldownDictionary(reader);
                        break;
                    case CalamityModMessageType.DeathCountSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleDeathCount(reader);
                        break;

                    //
                    // Syncs for specific bosses or entities
                    //

                    // This code has been edited to fail gracefully when trying to provide data for an invalid NPC.
                    case CalamityModMessageType.SyncCalamityNPCAIArray:
                        // Read the entire packet regardless of anything
                        byte npcIdx = reader.ReadByte();
                        float ai0 = reader.ReadSingle();
                        float ai1 = reader.ReadSingle();
                        float ai2 = reader.ReadSingle();
                        float ai3 = reader.ReadSingle();

                        // If the NPC in question isn't valid, don't do anything.
                        NPC npc = Main.npc[npcIdx];
                        if (!npc.active)
                            break;

                        CalamityGlobalNPC cgn = npc.Calamity();
                        cgn.newAI[0] = ai0;
                        cgn.newAI[1] = ai1;
                        cgn.newAI[2] = ai2;
                        cgn.newAI[3] = ai3;
                        break;
                    case CalamityModMessageType.SpawnSuperDummy:
                        int x = reader.ReadInt32();
                        int y = reader.ReadInt32();
                        // Not strictly necessary, but helps prevent unnecessary packetstorm in MP
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.NewNPC(new EntitySource_WorldEvent(), x, y, ModContent.NPCType<SuperDummyNPC>());
                        break;
                    case CalamityModMessageType.DeleteAllSuperDummies:
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            SuperDummy.DeleteDummies();
                        break;
                    case CalamityModMessageType.ServersideSpawnOldDuke:
                        byte playerIndex2 = reader.ReadByte();
                        CalamityUtils.SpawnOldDuke(playerIndex2);
                        break;
                    case CalamityModMessageType.DoGCountdownSync:
                        int countdown = reader.ReadInt32();
                        CalamityWorld.DoGSecondStageCountdown = countdown;
                        break;
                    case CalamityModMessageType.ArmoredDiggerCountdownSync:
                        int countdown5 = reader.ReadInt32();
                        CalamityWorld.ArmoredDiggerSpawnCooldown = countdown5;
                        break;
                    case CalamityModMessageType.ProvidenceDyeConditionSync:
                        byte npcIndex3 = reader.ReadByte();
                        (Main.npc[npcIndex3].ModNPC as Providence).hasTakenDaytimeDamage = reader.ReadBoolean();
                        break;
                    case CalamityModMessageType.PSCChallengeSync:
                        byte npcIndex4 = reader.ReadByte();
                        (Main.npc[npcIndex4].ModNPC as Providence).challenge = reader.ReadBoolean();
                        break;

                    //
                    // General syncs for entities
                    //

                    case CalamityModMessageType.SpawnNPCOnPlayer:
                        x = reader.ReadInt32();
                        y = reader.ReadInt32();
                        int npcType = reader.ReadInt32();
                        int player = reader.ReadInt32();
                        Vector2 spawnPosition = reader.ReadVector2();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int spawnedNPC = NPC.NewNPC(new EntitySource_WorldEvent(), x, y, npcType, Target: player);
                            NetMessage.SendData(MessageID.SyncNPC, -1, player, null, spawnedNPC);
                        }
                        break;
                    case CalamityModMessageType.SyncNPCMotionDataToServer:
                        int npcIndex = reader.ReadInt32();
                        Vector2 center = reader.ReadVector2();
                        Vector2 velocity = reader.ReadVector2();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Main.npc[npcIndex].Center = center;
                            Main.npc[npcIndex].velocity = velocity;
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npcIndex);
                        }
                        break;

                    //
                    // Tile Entities
                    //

                    case CalamityModMessageType.PowerCellFactory:
                        TEPowerCellFactory.ReadSyncPacket(mod, reader);
                        break;
                    case CalamityModMessageType.ChargingStationStandard:
                        TEChargingStation.ReadSyncPacket(mod, reader);
                        break;
                    case CalamityModMessageType.ChargingStationItemChange:
                        TEChargingStation.ReadItemSyncPacket(mod, reader);
                        break;
                    case CalamityModMessageType.Turret:
                        TEBaseTurret.ReadSyncPacket(mod, reader);
                        break;
                    case CalamityModMessageType.LabHologramProjector:
                        TELabHologramProjector.ReadSyncPacket(mod, reader);
                        break;
                    case CalamityModMessageType.UpdateCodebreakerConstituents:
                        TECodebreaker.ReadConstituentsUpdateSync(mod, reader);
                        break;
                    case CalamityModMessageType.UpdateCodebreakerContainedStuff:
                        TECodebreaker.ReadContainmentSync(mod, reader);
                        break;
                    case CalamityModMessageType.UpdateCodebreakerDecryptCountdown:
                        TECodebreaker.ReadDecryptCountdownSync(mod, reader);
                        break;

                    //
                    // Boss Rush
                    //

                    case CalamityModMessageType.BossRushStage:
                        int stage = reader.ReadInt32();
                        BossRushEvent.BossRushStage = stage;
                        break;
                    case CalamityModMessageType.BossRushStartTimer:
                        BossRushEvent.StartTimer = reader.ReadInt32();
                        break;
                    case CalamityModMessageType.BossRushEndTimer:
                        BossRushEvent.EndTimer = reader.ReadInt32();
                        break;
                    case CalamityModMessageType.EndBossRush:
                        BossRushEvent.EndEffects();
                        break;
                    case CalamityModMessageType.BRHostileProjKillSync:
                        int countdown3 = reader.ReadInt32();
                        BossRushEvent.HostileProjectileKillCounter = countdown3;
                        break;
                    case CalamityModMessageType.TeleportPlayer:
                        Main.player[reader.ReadInt32()].Calamity().HandleTeleport(reader.ReadInt32(), true, whoAmI);
                        break;

                    //
                    // Acid Rain
                    //

                    case CalamityModMessageType.AcidRainSync:
                        AcidRainEvent.AcidRainEventIsOngoing = reader.ReadBoolean();
                        AcidRainEvent.AccumulatedKillPoints = reader.ReadInt32();
                        AcidRainEvent.TimeSinceLastAcidRainKill = reader.ReadInt32();
                        break;
                    case CalamityModMessageType.AcidRainOldDukeSummonSync:
                        AcidRainEvent.HasTriedToSummonOldDuke = reader.ReadBoolean();
                        break;
                    case CalamityModMessageType.EncounteredOldDukeSync:
                        AcidRainEvent.OldDukeHasBeenEncountered = reader.ReadBoolean();
                        break;

                    //
                    // Draedon Summoner stuff
                    //
                    case CalamityModMessageType.CodebreakerSummonStuff:
                        CalamityWorld.DraedonSummonCountdown = reader.ReadInt32();
                        CalamityWorld.DraedonSummonPosition = reader.ReadVector2();
                        break;
                    case CalamityModMessageType.ExoMechSelection:
                        CalamityWorld.DraedonMechToSummon = (ExoMech)reader.ReadInt32();
                        break;

                    //
                    // Reforge syncs
                    //

                    case CalamityModMessageType.ItemTypeLastReforgedSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleItemTypeLastReforged(reader);
                        break;
                    case CalamityModMessageType.ReforgeTierSafetySync:
                        Main.player[reader.ReadInt32()].Calamity().HandleReforgeTierSafety(reader);
                        break;

                    //
                    // Mouse control syncs
                    //

                    case CalamityModMessageType.RightClickSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleRightClick(reader);
                        break;
                    case CalamityModMessageType.MousePositionSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleMousePosition(reader);
                        break;



                    //
                    // Default case: with no idea how long the packet is, we can't safely read data.
                    // Throw an exception now instead of allowing the network stream to corrupt.
                    //
                    default:
                        CalamityMod.Instance.Logger.Error($"Failed to parse Calamity packet: No Calamity packet exists with ID {msgType}.");
                        throw new Exception("Failed to parse Calamity packet: Invalid Calamity packet ID.");
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

        public static void NewNPC_ClientSide(Vector2 spawnPosition, int npcType, Player player)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                NPC.NewNPC(new EntitySource_WorldEvent(), (int)spawnPosition.X, (int)spawnPosition.Y, npcType, Target: player.whoAmI);
                return;
            }

            var netMessage = CalamityMod.Instance.GetPacket();
            netMessage.Write((byte)CalamityModMessageType.SpawnNPCOnPlayer);
            netMessage.Write((int)spawnPosition.X);
            netMessage.Write((int)spawnPosition.Y);
            netMessage.Write(npcType);
            netMessage.Write(player.whoAmI);
            netMessage.Send();
        }
    }

    public enum CalamityModMessageType : byte
    {
        // Proficiency levels
        // TODO -- simplify proficiency netcode, there do not need to be this many separate packet types
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

        // Player mechanic syncs
        MoveSpeedStatSync, // TODO -- this can't be synced every 60 frames, it needs to be synced every time the player is
        DefenseDamageSync, // TODO -- this can't be synced every 60 frames, it needs to be synced when the player gets hit, or every time it heals up
        RageSync, // TODO -- this can't be synced every 60 frames, it needs to be synced every time the player is
        AdrenalineSync, // TODO -- this can't be synced every 60 frames, it needs to be synced every time the player is
        CooldownAddition,
        CooldownRemoval,
        SyncCooldownDictionary,
        DeathCountSync, // TODO -- this is synced in numerous incorrect places, Armageddon deaths count twice, and it supposedly counts every time you log in

        // Syncs for specific bosses or entities
        SyncCalamityNPCAIArray,
        SpawnSuperDummy,
        DeleteAllSuperDummies,
        ServersideSpawnOldDuke,
        DoGCountdownSync, // TODO -- this gets written in about six thousand places which all need to be individually evaluated
        ArmoredDiggerCountdownSync, // TODO -- remove this mechanic entirely
        ProvidenceDyeConditionSync, // TODO -- this packetstorms if you hit Provi with spam weapons. It should ONLY send a packet if the status changes.
        PSCChallengeSync, // TODO -- once you've failed the PSC challenge this packetstorms

        // General things for entities
        SpawnNPCOnPlayer,
        SyncNPCMotionDataToServer,

        // Tile Entities
        PowerCellFactory,
        ChargingStationStandard,
        ChargingStationItemChange,
        Turret,
        LabHologramProjector,
        UpdateCodebreakerConstituents,
        UpdateCodebreakerContainedStuff,
        UpdateCodebreakerDecryptCountdown,

        // Draedon Summoner
        CodebreakerSummonStuff,
        ExoMechSelection,

        // Boss Rush
        BossRushStage,
        BossRushStartTimer,
        BossRushEndTimer,
        EndBossRush,
        BRHostileProjKillSync, // TODO -- Simplify this. Only one packet needs be sent: "kill all hostile projectiles for N frames".
        TeleportPlayer, // also used by Astral Arcanum.

        // Acid Rain
        AcidRainSync,
        AcidRainOldDukeSummonSync,
        EncounteredOldDukeSync,

        // Reforge syncs
        ItemTypeLastReforgedSync, // TODO -- there has to be a better way to do this, but I don't know what it is
        ReforgeTierSafetySync,

        // Mouse Controls syncs
        RightClickSync,
        MousePositionSync
    }
}
