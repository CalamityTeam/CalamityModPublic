using System.IO;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Packets.Entities
{
    internal sealed class WulfrumLureSpawnSparksPacket : CalamityPacket
    {
        public static WulfrumLureSpawnSparksPacket Instance { get; private set; }

        // Only the server spawns lure waves, so it tells clients where to draw the arrival sparks.
        public static void Send(Vector2 position)
        {
            if (!Main.dedServ)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteVector2(position);
            packet.Send();
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            Vector2 position = packet.ReadVector2();

            if (!Main.dedServ)
                WulfrumLureSignal.SpawnArrivalSparks(position);
        }
    }
}
