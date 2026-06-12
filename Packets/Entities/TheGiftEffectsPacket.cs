using System.IO;
using CalamityMod.Items.Tools;
using Terraria;

namespace CalamityMod.Packets.Entities
{
    internal sealed class TheGiftEffectsPacket : CalamityPacket
    {
        public static TheGiftEffectsPacket Instance { get; private set; }

        public static void Send(NPC npc, bool positive, int toClient = -1, int ignoreClient = -1)
        {
            if (npc is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteWhoAmI(npc);
            packet.Write(positive);
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            NPC npc = packet.ReadNPC();
            bool positive = packet.ReadBoolean();

            if (npc is null)
                return;

            TheGift.ApplyGiftEffects(npc, positive);

            if (Main.dedServ)
                Send(npc, positive, ignoreClient: sender);
        }
    }
}
