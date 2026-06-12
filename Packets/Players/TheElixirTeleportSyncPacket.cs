using System.IO;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Potions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Packets
{
    internal class TheElixirTeleportSyncPacket : CalamityPacket
    {
        public static TheElixirTeleportSyncPacket Instance { get; private set; }

        public static void Send(Player player, int toClient = -1, int ignoreClient = -1)
        {
            if (player is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteWhoAmI(player);
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            var player = packet.ReadPlayer();
            if (player is null)
                return;

            player.GetModPlayer<TheElixirPlayer>().RunDangerousLocationTeleportation();
        }
    }
}
