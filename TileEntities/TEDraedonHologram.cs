using CalamityMod.Tiles;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.TileEntities
{
    public class TEDraedonHologram : ModTileEntity
    {
        public bool CloseToPlayer;
        public override bool ValidTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.active() && tile.type == ModContent.TileType<DraedonHologram>();
        }
        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            writer.Write(CloseToPlayer);
        }
        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            CloseToPlayer = reader.ReadBoolean();
        }
        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 7);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }
            return Place(i, j);
        }
    }
}
