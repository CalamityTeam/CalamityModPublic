using CalamityMod.Tiles.DraedonStructures;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.TileEntities
{
    public class TEDraedonHologram : ModTileEntity
    {
        public bool PoppingUp;
        public const float PopupDistance = 560f;
        public override bool ValidTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.active() && tile.type == ModContent.TileType<DraedonHologram>();
        }

        // Sync the tile entity the moment it is place on the server.
        // This is done to cause it to register among all clients.
        public override void OnNetPlace() => NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            writer.Write(PoppingUp);
        }
        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            PoppingUp = reader.ReadBoolean();
        }

        // Check if the hologram should become visible.
        public override void Update()
        {
            bool wasPoppingUp = PoppingUp;

            // Stop popping up by default.
            PoppingUp = false;

            // But check if a player is nearby.
            // If one is, pop up.
            foreach (var player in Main.player)
            {
                if (!player.active)
                    continue;
                if (player.DistanceSQ(Position.ToWorldCoordinates()) < PopupDistance * PopupDistance)
                {
                    if (!PoppingUp)
                    {
                        PoppingUp = true;
                    }
                    break;
                }
            }

            // If the popup state changes, sync the tile entity across all clients.
            if (PoppingUp != wasPoppingUp)
                NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
        }
        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
        {
            // If in multiplayer, tell the server to place the tile entity and DO NOT place it yourself. That would mismatch IDs.
            // Also tell the server that you placed the 6x7 tiles that make up the Charging Station.
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileRange(Main.myPlayer, i, j, DraedonHologram.Width, DraedonHologram.Height);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type);
                return -1;
            }
            return Place(i, j);
        }
    }
}
