using System.IO;
using CalamityMod.Systems;
using Terraria;
using Terraria.ModLoader.IO;

namespace CalamityMod.Packets.Entities
{
    internal sealed class TrustyOldRodEnemyPacket : CalamityPacket
    {
        public static TrustyOldRodEnemyPacket Instance { get; private set; }

        public static void Send(Player player, int bobberWhoAmI, int rarity = 1, bool lava = false, bool honey = false, int toClient = -1, int ignoreClient = -1)
        {
            if (player is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteWhoAmI(player);
            packet.Write(bobberWhoAmI);
            packet.Write(rarity);
            packet.WriteFlags(lava, honey);
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            var player = packet.ReadPlayer();
            var bobber = packet.ReadInt32();
            var rarity = packet.ReadInt32();
            packet.ReadFlags(out bool lava, out bool honey);

            if (player is null)
                return;

            if (Main.dedServ)
                TrustyOldRodEnemySystem.SpawnTrustyOldRodNPC(player, bobber, rarity, lava, honey);
        }
    }
}
