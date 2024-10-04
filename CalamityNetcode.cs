using System;
using System.IO;
using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.NPCs;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Systems;
using CalamityMod.TileEntities;
using CalamityMod.World;
using Microsoft.Xna.Framework;
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
                    // Player mechanic syncs
                    //

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

                    //
                    // Syncs for specific bosses or entities
                    //

                    case CalamityModMessageType.SyncDestroyerLaserColor:
                        byte npcIdx3 = reader.ReadByte();
                        int laserColor = reader.ReadInt32();

                        // If the NPC in question isn't valid, don't do anything.
                        NPC npc3 = Main.npc[npcIdx3];
                        if (!npc3.active)
                            break;

                        CalamityGlobalNPC cgn3 = npc3.Calamity();
                        cgn3.destroyerLaserColor = laserColor;
                        break;

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

                    case CalamityModMessageType.SyncVanillaNPCLocalAIArray:
                        // Read the entire packet regardless of anything
                        byte npcIdx2 = reader.ReadByte();
                        float localAI0 = reader.ReadSingle();
                        float localAI1 = reader.ReadSingle();
                        float localAI2 = reader.ReadSingle();
                        float localAI3 = reader.ReadSingle();

                        // If the NPC in question isn't valid, don't do anything.
                        NPC npc2 = Main.npc[npcIdx2];
                        if (!npc2.active)
                            break;

                        npc2.localAI[0] = localAI0;
                        npc2.localAI[1] = localAI1;
                        npc2.localAI[2] = localAI2;
                        npc2.localAI[3] = localAI3;
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

                    case CalamityModMessageType.SyncAndroombaSolution:
                        int index = reader.ReadInt32();
                        int solType = reader.ReadInt32();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            AndroombaFriendly.SwapSolution(index, solType);
                        break;

                    case CalamityModMessageType.SyncAndroombaAI:
                        {
                            int idx = reader.ReadInt32();
                            int phase = reader.ReadInt32();
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                AndroombaFriendly.ChangeAI(idx, phase);
                        }
                        break;

                    case CalamityModMessageType.SyncSlabCrabAI:
                        {
                            int idx = reader.ReadInt32();
                            int phase = reader.ReadInt32();
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                AndroombaFriendly.ChangeAI(idx, phase);
                        }
                        break;

                    case CalamityModMessageType.ServersideSpawnOldDuke:
                        byte playerIndex2 = reader.ReadByte();
                        CalamityUtils.SpawnOldDuke(playerIndex2);
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

                    case CalamityModMessageType.SyncNPCPosAndRotOnly:
                        npcIndex = reader.ReadByte();
                        Vector2 position = reader.ReadVector2();
                        float rotation = (float)reader.ReadHalf(); //rotation unit is radian (-π/2 ≤ rotation ≤ π/2) so Half precision should works

                        if (npcIndex >= Main.maxNPCs)
                            break;

                        npc = Main.npc[npcIndex];
                        npc.position = position;
                        npc.rotation = rotation;

                        if (Main.dedServ)
                        {
                            ModPacket packet = CalamityMod.Instance.GetPacket();
                            packet.Write((byte)CalamityModMessageType.SyncNPCPosAndRotOnly);
                            packet.Write((byte)npcIndex);
                            packet.WriteVector2(position);
                            packet.Write((Half)rotation);
                            packet.Send(ignoreClient: whoAmI);
                        }
                        break;

                    //
                    // Tile Entities
                    //

                    case CalamityModMessageType.UnlockAbyssChests:
                        Abyss.UnlockAllAbyssChests();
                        break;
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
                        CalamityWorld.DraedonMechdusa = reader.ReadBoolean();
                        break;
                    case CalamityModMessageType.ExoMechSelection:
                        CalamityWorld.DraedonMechToSummon = (ExoMech)reader.ReadInt32();
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
                    // Difficulty syncs
                    //

                    case CalamityModMessageType.SyncDifficulties:
                        int sender = reader.ReadInt32();
                        CalamityWorld.revenge = reader.ReadBoolean();
                        CalamityWorld.death = reader.ReadBoolean();
                        //TODO - Something so that other mods that hijack the difficulty ui can also use the remainder of the reader to have their own shit

                        if (Main.netMode == NetmodeID.Server)
                            SyncCalamityWorldDifficulties(sender);
                        break;

                    //
                    // Music event syncs
                    //
                    case CalamityModMessageType.MusicEventSyncRequest:
                        MusicEventSystem.FulfillSyncRequest(whoAmI);
                        break;

                    case CalamityModMessageType.MusicEventSyncResponse:
                        MusicEventSystem.ReceiveSyncResponse(reader);
                        break;

                    //
                    // Bandit refund syncs
                    //
                    case CalamityModMessageType.SomeoneGotScammedByTinkerer:
                        int scammedOne = reader.ReadByte();
                        int stolen = reader.Read7BitEncodedInt();

                        CalamityWorld.MoneyStolenByBandit += stolen;
                        CalamityWorld.Reforges++;

                        // Broadcast back for tragic event
                        // WorldSync DO sync the MoneyStolenByBandit and Refores variable, But spamming SyncWorld is not a ideal action
                        if (Main.dedServ)
                        {
                            ModPacket packet = CalamityMod.Instance.GetPacket();
                            packet.Write((byte)CalamityModMessageType.SomeoneGotScammedByTinkerer);
                            packet.Write((byte)scammedOne);
                            packet.Write7BitEncodedInt(stolen);
                            packet.Send(ignoreClient: scammedOne);
                        }

                        break;

                    case CalamityModMessageType.WantToRefundReforges:
                        int requester = reader.ReadByte();

                        // Only Server should handle this action!
                        if (!Main.dedServ)
                            break;

                        int banditIdx = NPC.FindFirstNPC(ModContent.NPCType<THIEF>());
                        if (banditIdx == -1)
                            break;

                        NPC bandit = Main.npc[banditIdx];
                        if (bandit == null || !bandit.active)
                            break;

                        THIEF.DoRefund(bandit);
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
                    throw; // this either will crash the game or be caught by TML's packet policing
            }
        }

        public static void SyncWorld()
        {
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }

        public static void SyncCalamityWorldDifficulties(int sender)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            var netMessage = CalamityMod.Instance.GetPacket();
            netMessage.Write((byte)CalamityModMessageType.SyncDifficulties);
            netMessage.Write(sender);
            netMessage.Write(CalamityWorld.revenge);
            netMessage.Write(CalamityWorld.death);

            //TODO - Let other mods also add their own bits in that sync. Ideally would be done through the difficultystem itself
            netMessage.Send(-1, sender);
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
        // Player mechanic syncs
        DefenseDamageSync, // TODO -- this can't be synced every 60 frames, it needs to be synced when the player gets hit, or every time it heals up
        RageSync, // TODO -- this can't be synced every 60 frames, it needs to be synced every time the player is
        AdrenalineSync, // TODO -- this can't be synced every 60 frames, it needs to be synced every time the player is
        CooldownAddition,
        CooldownRemoval,
        SyncCooldownDictionary,

        // Syncs for specific bosses or entities
        SyncDestroyerLaserColor,
        SyncCalamityNPCAIArray,
        SyncVanillaNPCLocalAIArray,
        SpawnSuperDummy,
        DeleteAllSuperDummies,
        SyncAndroombaSolution,
        SyncAndroombaAI,
        SyncSlabCrabAI,
        ServersideSpawnOldDuke,
        ArmoredDiggerCountdownSync, // TODO -- remove this mechanic entirely
        ProvidenceDyeConditionSync, // TODO -- this packetstorms if you hit Provi with spam weapons. It should ONLY send a packet if the status changes.
        PSCChallengeSync, // TODO -- once you've failed the PSC challenge this packetstorms

        // General things for entities
        SpawnNPCOnPlayer,
        SyncNPCMotionDataToServer,
        SyncNPCPosAndRotOnly,

        // Tile Entities
        PowerCellFactory,
        ChargingStationStandard,
        ChargingStationItemChange,
        Turret,
        LabHologramProjector,
        UpdateCodebreakerConstituents,
        UpdateCodebreakerContainedStuff,
        UpdateCodebreakerDecryptCountdown,
        UnlockAbyssChests,

        // Draedon Summoner
        CodebreakerSummonStuff,
        ExoMechSelection,

        // Boss Rush
        BossRushStage,
        BossRushStartTimer,
        BossRushEndTimer,
        EndBossRush,
        BRHostileProjKillSync, // TODO -- Simplify this. Only one packet needs be sent: "kill all hostile projectiles for N frames".

        // Acid Rain
        AcidRainSync,
        AcidRainOldDukeSummonSync,
        EncounteredOldDukeSync,

        // Mouse Controls syncs
        RightClickSync,
        MousePositionSync,

        // World state sync
        SyncDifficulties,

        // Music events
        MusicEventSyncRequest,
        MusicEventSyncResponse,
        
        // Bandit Reforge Refund
        SomeoneGotScammedByTinkerer,
        WantToRefundReforges
    }
}
