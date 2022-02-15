using CalamityMod.UI.CooldownIndicators;
using System;
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
            SyncMoveSpeed(false);
            SyncDefenseDamage(false);
            SyncCooldown(false);
        }

        private void EnterWorldSync()
        {
            StandardSync();
            SyncExactLevel(false, 0);
            SyncExactLevel(false, 1);
            SyncExactLevel(false, 2);
            SyncExactLevel(false, 3);
            SyncExactLevel(false, 4);
            SyncLevel(false, 0);
            SyncLevel(false, 1);
            SyncLevel(false, 2);
            SyncLevel(false, 3);
            SyncLevel(false, 4);
        }

        internal void MouseControlsSync()
        {
            SyncRightClick(false);
            SyncMousePosition(false);
        }
        #endregion

        #region Creating and Sending Packets
        private void SyncExactLevel(bool server, int levelType)
        {
            ModPacket packet = mod.GetPacket();
            switch (levelType)
            {
                case 0:
                    packet.Write((byte)CalamityModMessageType.ExactMeleeLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(exactMeleeLevel);
                    break;
                case 1:
                    packet.Write((byte)CalamityModMessageType.ExactRangedLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(exactRangedLevel);
                    break;
                case 2:
                    packet.Write((byte)CalamityModMessageType.ExactMagicLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(exactMagicLevel);
                    break;
                case 3:
                    packet.Write((byte)CalamityModMessageType.ExactSummonLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(exactSummonLevel);
                    break;
                case 4:
                    packet.Write((byte)CalamityModMessageType.ExactRogueLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(exactRogueLevel);
                    break;
            }
            player.SendPacket(packet, server);
        }

        private void SyncLevel(bool server, int levelType)
        {
            ModPacket packet = mod.GetPacket(256);
            switch (levelType)
            {
                case 0:
                    packet.Write((byte)CalamityModMessageType.MeleeLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(meleeLevel);
                    break;
                case 1:
                    packet.Write((byte)CalamityModMessageType.RangedLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(rangedLevel);
                    break;
                case 2:
                    packet.Write((byte)CalamityModMessageType.MagicLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(magicLevel);
                    break;
                case 3:
                    packet.Write((byte)CalamityModMessageType.SummonLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(summonLevel);
                    break;
                case 4:
                    packet.Write((byte)CalamityModMessageType.RogueLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(rogueLevel);
                    break;
            }
            player.SendPacket(packet, server);
        }

        public void SyncMoveSpeed(bool server)
        {
            ModPacket packet = mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.MoveSpeedStatSync);
            packet.Write(player.whoAmI);
            packet.Write(moveSpeedStat);
            player.SendPacket(packet, server);
        }

        public void SyncDefenseDamage(bool server)
        {
            ModPacket packet = mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.DefenseDamageSync);
            packet.Write(player.whoAmI);
            packet.Write(totalDefenseDamage);
            packet.Write(defenseDamageRecoveryFrames);
            packet.Write(totalDefenseDamageRecoveryFrames);
            packet.Write(defenseDamageDelayFrames);
            player.SendPacket(packet, server);
        }

        public void SyncRage(bool server)
        {
            ModPacket packet = mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.RageSync);
            packet.Write(player.whoAmI);
            packet.Write(rage);
            player.SendPacket(packet, server);
        }

        public void SyncAdrenaline(bool server)
        {
            ModPacket packet = mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.AdrenalineSync);
            packet.Write(player.whoAmI);
            packet.Write(adrenaline);
            player.SendPacket(packet, server);
        }

        internal void SyncCooldown(bool server, string cooldownID = "")
        {
            //If no specific sync id is provided, sync all of the player's cooldowns. Ideally this doesn't happen though. Ideally.
            if (cooldownID == "")
            {
                foreach (string key in CooldownIndicator.IDtoType.Keys)
                {
                    ModPacket packet = mod.GetPacket();
                    packet.Write((byte)CalamityModMessageType.CooldownSync);
                    packet.Write(player.whoAmI);
                    packet.Write(key);
                    int timer = 0;
                    if (Cooldowns.ContainsType(CooldownIndicator.IDtoType[key]))
                        timer = Cooldowns.Find(cooldown => cooldown.GetType() == CooldownIndicator.IDtoType[key]).TimeLeft;
                    packet.Write(timer);
                    player.SendPacket(packet, server);
                }
            }

            else
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)CalamityModMessageType.CooldownSync);
                packet.Write(player.whoAmI);
                packet.Write(cooldownID);
                int timer = 0;
                if (Cooldowns.ContainsType(CooldownIndicator.IDtoType[cooldownID]))
                    timer = Cooldowns.Find(cooldown => cooldown.GetType() == CooldownIndicator.IDtoType[cooldownID]).TimeLeft;
                packet.Write(timer);
                player.SendPacket(packet, server);
            }
        }

        private void SyncDeathCount(bool server)
        {
            ModPacket packet = mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.DeathCountSync);
            packet.Write(player.whoAmI);
            packet.Write(deathCount);
            player.SendPacket(packet, server);
        }

        public void SyncItemTypeLastReforged(bool server)
        {
            ModPacket packet = mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.ItemTypeLastReforgedSync);
            packet.Write(player.whoAmI);
            packet.Write(itemTypeLastReforged);
            player.SendPacket(packet, server);
        }

        public void SyncReforgeTierSafety(bool server)
        {
            ModPacket packet = mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.ReforgeTierSafetySync);
            packet.Write(player.whoAmI);
            packet.Write(reforgeTierSafety);
            player.SendPacket(packet, server);
        }

        public void SyncRightClick(bool server)
        {
            ModPacket packet = mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.RightClickSync);
            packet.Write(player.whoAmI);
            packet.Write(mouseRight);
            player.SendPacket(packet, server);
        }
        public void SyncMousePosition(bool server)
        {
            ModPacket packet = mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.MousePositionSync);
            packet.Write(player.whoAmI);
            packet.WriteVector2(mouseWorld);
            player.SendPacket(packet, server);
        }

        #endregion

        #region Reading and Handling Packets
        internal void HandleExactLevels(BinaryReader reader, int levelType)
        {
            switch (levelType)
            {
                case 0:
                    exactMeleeLevel = reader.ReadInt32();
                    break;
                case 1:
                    exactRangedLevel = reader.ReadInt32();
                    break;
                case 2:
                    exactMagicLevel = reader.ReadInt32();
                    break;
                case 3:
                    exactSummonLevel = reader.ReadInt32();
                    break;
                case 4:
                    exactRogueLevel = reader.ReadInt32();
                    break;
            }

            if (Main.netMode == NetmodeID.Server)
                SyncExactLevel(true, levelType);
        }

        internal void HandleLevels(BinaryReader reader, int levelType)
        {
            switch (levelType)
            {
                case 0:
                    meleeLevel = reader.ReadInt32();
                    break;
                case 1:
                    rangedLevel = reader.ReadInt32();
                    break;
                case 2:
                    magicLevel = reader.ReadInt32();
                    break;
                case 3:
                    summonLevel = reader.ReadInt32();
                    break;
                case 4:
                    rogueLevel = reader.ReadInt32();
                    break;
            }

            if (Main.netMode == NetmodeID.Server)
                SyncLevel(true, levelType);
        }

        internal void HandleRage(BinaryReader reader)
        {
            rage = reader.ReadSingle();
            if (Main.netMode == NetmodeID.Server)
                SyncRage(true);
        }

        internal void HandleAdrenaline(BinaryReader reader)
        {
            adrenaline = reader.ReadSingle();
            if (Main.netMode == NetmodeID.Server)
                SyncAdrenaline(true);
        }

        internal void HandleDeathCount(BinaryReader reader)
        {
            deathCount = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
                SyncDeathCount(true);
        }

        internal void HandleItemTypeLastReforged(BinaryReader reader)
        {
            itemTypeLastReforged = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
                SyncItemTypeLastReforged(true);
        }

        internal void HandleReforgeTierSafety(BinaryReader reader)
        {
            reforgeTierSafety = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
                SyncReforgeTierSafety(true);
        }

        internal void HandleMoveSpeedStat(BinaryReader reader)
        {
            moveSpeedStat = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
                SyncMoveSpeed(true);
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

        internal void HandleCooldowns(BinaryReader reader)
        {
            string syncID = reader.ReadString();
            int timer = reader.ReadInt32();
            Type cooldownType = CooldownIndicator.IDtoType[syncID];

            if (Cooldowns.ContainsType(cooldownType))
                Cooldowns.Find(cooldown => cooldown.GetType() == cooldownType).TimeLeft = timer;

            else if (timer > 0)
                Cooldowns.Add((CooldownIndicator)Activator.CreateInstance(cooldownType, timer, player));

            if (Main.netMode == NetmodeID.Server)
                SyncCooldown(true);
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
