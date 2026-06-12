using System.IO;
using CalamityMod.Items.SummonItems.TownPets;
using CalamityMod.NPCs.Abyss;
using Terraria;

namespace CalamityMod.Packets
{
    internal sealed class SyncTownPigLicensePacket : CalamityPacket
    {
        public static SyncTownPigLicensePacket Instance { get; private set; }

        public static void Send(int toClient = -1, int ignoreClient = -1)
        {
            var packet = Instance.CreateBasePacket();
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender) => TheHousingContract.SpawnPiggy();
    }
}
