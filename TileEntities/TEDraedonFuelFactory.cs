using CalamityMod.Items;
using CalamityMod.Tiles;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.TileEntities
{
    public class TEDraedonFuelFactory : ModTileEntity
    {
        public int Time;
        public Item HeldItem = new Item();
        public void SyncTile()
        {
            NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
        }
        public override bool ValidTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.active() && tile.type == ModContent.TileType<DraedonFuelFactory>();
        }
        public override void Update()
        {
            if (HeldItem.type != ModContent.ItemType<PowerCell>())
            {
                HeldItem.SetDefaults(ModContent.ItemType<PowerCell>());
                HeldItem.stack = 0;
            }

            Time++;
            if (Time % 3 == 2 && Main.tileFrame[ModContent.TileType<DraedonFuelFactory>()] == 43)
            {
                HeldItem.stack++;
            }
            SyncTile();
        }
        public override void OnKill()
        {
            if (Main.LocalPlayer.Calamity().CurrentlyViewedFactory == this)
                Main.LocalPlayer.Calamity().CurrentlyViewedFactory = null;
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
        public override TagCompound Save()
        {
            return new TagCompound
            {
                ["Type"] = HeldItem.type,
                ["Stack"] = HeldItem.stack,
                ["Prefix"] = HeldItem.prefix,
                ["NetID"] = HeldItem.active && HeldItem.stack > 0 ? HeldItem.netID : 0,
            };
        }
        public override void Load(TagCompound tag)
        {
            HeldItem.type = tag.GetInt("Type");
            HeldItem.stack = tag.GetInt("Stack");
            HeldItem.prefix = tag.GetByte("Prefix");
            HeldItem.netID = tag.GetInt("NetID");
        }
        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            writer.Write(Time);
            writer.Write(HeldItem.type);
            writer.Write(HeldItem.stack);
            writer.Write(HeldItem.active && HeldItem.stack > 0 ? HeldItem.netID : 0);
            writer.Write(HeldItem.prefix);
        }
        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            Time = reader.ReadInt32();
            HeldItem.type = reader.ReadInt32();
            HeldItem.stack = reader.ReadInt32();
            HeldItem.netID = reader.ReadInt32();
            HeldItem.prefix = reader.ReadByte();
        }
    }
}
