using System.IO;
using CalamityMod.NPCs.NormalNPCs;
using Terraria;

namespace CalamityMod.Packets
{
    internal class DivineSwineOfferingPacket : CalamityPacket
    {
        public static DivineSwineOfferingPacket Instance { get; private set; }

        public static void Send(DivineSwine divineSwine, bool? offeringState = null, int toClient = -1, int ignoreClient = -1)
        {
            if (divineSwine is null || !offeringState.HasValue)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteWhoAmI(divineSwine);
            packet.Write(offeringState.Value);
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            var divineSwine = packet.ReadModNPC<DivineSwine>();
            var offeringState = packet.ReadBoolean();

            if (divineSwine is null)
                return;

            if (Main.dedServ)
                divineSwine.AcceptOffering(offeringState);
        }
    }
}
