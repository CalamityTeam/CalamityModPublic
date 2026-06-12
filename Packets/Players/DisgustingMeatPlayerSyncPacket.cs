using System;
using System.IO;
using CalamityMod.Items.Tools;
using MonoMod.Core.Platforms;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityMod.Packets
{
    internal class DisgustingMeatPlayerSyncPacket : CalamityPacket
    {
        public static DisgustingMeatPlayerSyncPacket Instance { get; private set; }

        public static void Send(DisgustingMeatPlayer playerToSync, int toClient = -1, int ignoreClient = -1)
        {
            if (playerToSync is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteWhoAmI(playerToSync);
            packet.Write(playerToSync.DoingVomitAnimation);
            packet.Write(playerToSync.EjectMiscUpgrades);
            packet.Write(playerToSync.VomitTime);
            packet.Write((int)playerToSync.Player.eyeHelper.CurrentEyeFrame);
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            var player = packet.ReadPlayer();
            var doingVomitAnimation = packet.ReadBoolean();
            var ejectMiscUpgrades = packet.ReadBoolean();
            var vomitTime = packet.ReadInt32();
            var currentEyeFrame = packet.ReadInt32();

            if (player is null)
                return;

            var modPlayer = player.GetModPlayer<DisgustingMeatPlayer>();
            modPlayer.DoingVomitAnimation = doingVomitAnimation;
            modPlayer.EjectMiscUpgrades = ejectMiscUpgrades;
            modPlayer.VomitTime = vomitTime;
            player.eyeHelper.CurrentEyeFrame = (PlayerEyeHelper.EyeFrame)currentEyeFrame;

            if (Main.dedServ)
                Send(modPlayer, ignoreClient: sender);
        }
    }
}
