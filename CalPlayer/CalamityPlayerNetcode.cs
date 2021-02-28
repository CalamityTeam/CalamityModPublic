using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer
{
    public partial class CalamityPlayer
    {
        #region Standard Syncs
        internal const int GlobalSyncPacketTimer = 15;

        internal void StandardSync()
        {
            SyncRage(false);
            SyncAdrenaline(false);
            SyncDeathModeUnderworldTime(false);
            SyncDeathModeBlizzardTime(false);
            SyncMoveSpeed(false);
            SyncDefenseDamage(false);
            SyncDodgeCooldown(false);
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
            SyncRage(false);
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
            packet.Write(defenseDamage);
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

        internal void SyncDodgeCooldown(bool server)
        {
            ModPacket packet = mod.GetPacket();
            packet.Write((byte)CalamityModMessageType.DodgeCooldown);
            packet.Write(player.whoAmI);
            packet.Write(dodgeCooldownTimer);
            player.SendPacket(packet, server);
        }

        private void SyncDeathCount(bool server)
        {
            ModPacket packet = mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.DeathCountSync);
            packet.Write(player.whoAmI);
            packet.Write(deathCount);
            player.SendPacket(packet, server);
        }

        public void SyncDeathModeUnderworldTime(bool server)
        {
            ModPacket packet = mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.DeathModeUnderworldTimeSync);
            packet.Write(player.whoAmI);
            packet.Write(deathModeUnderworldTime);
            player.SendPacket(packet, server);
        }

        public void SyncDeathModeBlizzardTime(bool server)
        {
            ModPacket packet = mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.DeathModeBlizzardTimeSync);
            packet.Write(player.whoAmI);
            packet.Write(deathModeBlizzardTime);
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
            rage = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
                SyncRage(true);
        }

        internal void HandleAdrenaline(BinaryReader reader)
        {
            adrenaline = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
                SyncAdrenaline(true);
        }

        internal void HandleDeathCount(BinaryReader reader)
        {
            deathCount = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
                SyncDeathCount(true);
        }

        internal void HandleDeathModeUnderworldTime(BinaryReader reader)
        {
            deathModeUnderworldTime = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
                SyncDeathModeUnderworldTime(true);
        }

        internal void HandleDeathModeBlizzardTime(BinaryReader reader)
        {
            deathModeBlizzardTime = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
                SyncDeathModeBlizzardTime(true);
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
            defenseDamage = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
                SyncDefenseDamage(true);
        }

        internal void HandleDodgeCooldown(BinaryReader reader)
        {
            dodgeCooldownTimer = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
                SyncDodgeCooldown(true);
        }
        #endregion
    }
}
