using System.IO;
using CalamityMod.Items.Tools;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Packets.Entities
{
    internal sealed class RequestApplyTheGiftPacket : CalamityPacket
    {
        public static RequestApplyTheGiftPacket Instance { get; private set; }

        public static void Send(Item item, NPC npc, int toClient = -1, int ignoreClient = -1)
        {
            if (item is null || npc is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.Write((short)item.whoAmI);
            packet.WriteWhoAmI(npc);
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            int itemWhoAmI = packet.ReadInt16();
            NPC npc = packet.ReadNPC();

            if (!Main.dedServ || npc is null || itemWhoAmI < 0 || itemWhoAmI >= Main.maxItems)
                return;

            Item item = Main.item[itemWhoAmI];
            if (!item.active || item.type != ModContent.ItemType<TheGift>())
                return;

            TheGift.TryApplyGift(item, npc);
        }
    }
}
