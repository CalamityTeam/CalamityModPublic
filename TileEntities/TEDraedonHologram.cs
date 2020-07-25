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
        public override void Update()
        {
            if ((int)(Main.GlobalTime * 60) % 20f == 19f)
            {
                CloseToPlayer = false;
                foreach (var player in Main.player)
                {
                    if (!player.active)
                        continue;
                    if (player.Distance(Position.ToWorldCoordinates()) < 560f)
                    {
                        CloseToPlayer = true;
                        break;
                    }
                }
            }
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
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 3);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }
            return Place(i, j);
        }
    }
}
