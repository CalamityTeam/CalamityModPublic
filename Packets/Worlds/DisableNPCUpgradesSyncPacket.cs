using System.IO;
using Terraria;
using Terraria.ID;  

namespace CalamityMod.Packets.Worlds
{
    internal class DisableNPCUpgradesSyncPacket : CalamityPacket
    {
        public static DisableNPCUpgradesSyncPacket Instance { get; private set; }

        public static void Send(int upgradeType = -1, int toClient = -1, int ignoreClient = -1)
        {
            if (upgradeType < 0 || upgradeType > 2)
                return;

            var packet = Instance.CreateBasePacket();
            packet.Write(upgradeType);
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            var upgradeType = packet.ReadInt32();

            if (upgradeType == 0)
                NPC.peddlersSatchelWasUsed = false;
            else if (upgradeType == 1)
                NPC.combatBookWasUsed = false;
            else if (upgradeType == 2)
                NPC.combatBookVolumeTwoWasUsed = false;

            // Apply change to everyone
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.WorldData);
            }
        }
    }
}
