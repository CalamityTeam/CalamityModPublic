using CalamityMod.Cooldowns;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer
{
    public partial class CalamityPlayer : ModPlayer
    {
        #region Standard Syncs
        internal const int GlobalSyncPacketTimer = 15;

        internal void StandardSync()
        {
            SyncRage(false);
            SyncAdrenaline(false);
            SyncDefenseDamage(false);
        }

        private void EnterWorldSync()
        {
            StandardSync();
        }

        internal void MouseControlsSync()
        {
            SyncRightClick(false);
            SyncMousePosition(false);
        }
        #endregion

        #region Creating and Sending Packets
        public void SyncDefenseDamage(bool server)
        {
            ModPacket packet = Mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.DefenseDamageSync);
            packet.Write(Player.whoAmI);
            packet.Write(totalDefenseDamage);
            packet.Write(defenseDamageRecoveryFrames);
            packet.Write(totalDefenseDamageRecoveryFrames);
            packet.Write(defenseDamageDelayFrames);
            Player.SendPacket(packet, server);
        }

        public void SyncRage(bool server)
        {
            ModPacket packet = Mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.RageSync);
            packet.Write(Player.whoAmI);
            packet.Write(rage);
            packet.Write(rageCombatFrames);
            Player.SendPacket(packet, server);
        }

        public void SyncAdrenaline(bool server)
        {
            ModPacket packet = Mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.AdrenalineSync);
            packet.Write(Player.whoAmI);
            packet.Write(adrenaline);
            Player.SendPacket(packet, server);
        }

        public void SyncCooldownAddition(bool server, CooldownInstance cd)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = Mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.CooldownAddition);
            packet.Write(Player.whoAmI);
            cd.Write(packet);
            Player.SendPacket(packet, server);
        }

        public void SyncCooldownRemoval(bool server, IList<string> cooldownIDs)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = Mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.CooldownRemoval);
            packet.Write(Player.whoAmI);
            packet.Write(cooldownIDs.Count);
            foreach (string id in cooldownIDs)
                packet.Write(CooldownRegistry.Get(id).netID);
            Player.SendPacket(packet, server);
        }

        public void SyncCooldownDictionary(bool server)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = Mod.GetPacket(1024);
            packet.Write((byte)CalamityModMessageType.SyncCooldownDictionary);
            packet.Write(Player.whoAmI);
            packet.Write(cooldowns.Count);
            foreach (CooldownInstance cd in cooldowns.Values)
                cd.Write(packet);
            Player.SendPacket(packet, server);
        }

        private void SyncDeathCount(bool server)
        {
            ModPacket packet = Mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.DeathCountSync);
            packet.Write(Player.whoAmI);
            packet.Write(deathCount);
            Player.SendPacket(packet, server);
        }

        public void SyncRightClick(bool server)
        {
            ModPacket packet = Mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.RightClickSync);
            packet.Write(Player.whoAmI);
            packet.Write(mouseRight);
            Player.SendPacket(packet, server);
        }
        public void SyncMousePosition(bool server)
        {
            ModPacket packet = Mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.MousePositionSync);
            packet.Write(Player.whoAmI);
            packet.WriteVector2(mouseWorld);
            Player.SendPacket(packet, server);
        }

        #endregion

        #region Reading and Handling Packets
        internal void HandleRage(BinaryReader reader)
        {
            rage = reader.ReadSingle();
            rageCombatFrames = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
                SyncRage(true);
        }

        internal void HandleAdrenaline(BinaryReader reader)
        {
            adrenaline = reader.ReadSingle();
            if (Main.netMode == NetmodeID.Server)
                SyncAdrenaline(true);
        }

        internal void HandleCooldownAddition(BinaryReader reader)
        {
            // The player ID and message ID are already read. The only remaining data is the serialization of the cooldown instance.
            CooldownInstance instance = new CooldownInstance(reader);

            // Actually assign this freshly synced cooldown to the appropriate player.
            string id = CooldownRegistry.registry[instance.netID].ID;
            cooldowns[id] = instance;
        }

        internal void HandleCooldownRemoval(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                ushort netID = reader.ReadUInt16();
                cooldowns.Remove(CooldownRegistry.registry[netID].ID);
            }
        }

        internal void HandleCooldownDictionary(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            if (count <= 0)
                return;

            // Cooldown dictionary packets are just a span of serialized cooldown instances. So each one can be read exactly as with a single cooldown.
            Dictionary<ushort, CooldownInstance> syncedCooldowns = new Dictionary<ushort, CooldownInstance>(count);
            for (int i = 0; i < count; ++i)
            {
                CooldownInstance instance = new CooldownInstance(reader);
                syncedCooldowns[instance.netID] = instance;
            }

            HashSet<ushort> localIDs = new HashSet<ushort>();
            foreach (CooldownInstance localInstance in cooldowns.Values)
                localIDs.Add(localInstance.netID);

            HashSet<ushort> syncedIDs = new HashSet<ushort>();
            foreach (ushort syncedID in syncedCooldowns.Keys)
                syncedIDs.Add(syncedID);

            HashSet<ushort> combinedIDSet = new HashSet<ushort>();
            combinedIDSet.UnionWith(localIDs);
            combinedIDSet.UnionWith(syncedIDs);

            foreach (ushort netID in combinedIDSet)
            {
                bool existsLocally = localIDs.Contains(netID);
                bool existsRemotely = syncedIDs.Contains(netID);
                string id = CooldownRegistry.registry[netID].ID;

                // Exists locally but not remotely = cull -- destroy the local copy.
                if (existsLocally && !existsRemotely)
                    cooldowns.Remove(id);
                // Exists remotely but not locally = add -- insert into the dictionary.
                else if (existsRemotely && !existsLocally)
                    cooldowns[id] = syncedCooldowns[netID];
                // Exists in both places = update -- update timing fields but don't replace the instance.
                else if (existsLocally && existsRemotely)
                {
                    CooldownInstance localInstance = cooldowns[id];
                    localInstance.duration = syncedCooldowns[netID].duration;
                    localInstance.timeLeft = syncedCooldowns[netID].timeLeft;
                }
            }
        }

        internal void HandleDeathCount(BinaryReader reader)
        {
            deathCount = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
                SyncDeathCount(true);
        }

        internal void HandleDefenseDamage(BinaryReader reader)
        {
            totalDefenseDamage = reader.ReadInt32();
            defenseDamageRecoveryFrames = reader.ReadInt32();
            totalDefenseDamageRecoveryFrames = reader.ReadInt32();
            defenseDamageDelayFrames = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
                SyncDefenseDamage(true);
        }

        internal void HandleRightClick(BinaryReader reader)
        {
            mouseRight = reader.ReadBoolean();
            if (Main.netMode == NetmodeID.Server)
                SyncRightClick(true);
        }
        internal void HandleMousePosition(BinaryReader reader)
        {
            mouseWorld = reader.ReadVector2();
            if (Main.netMode == NetmodeID.Server)
                SyncMousePosition(true);
        }
        #endregion
    }
}
