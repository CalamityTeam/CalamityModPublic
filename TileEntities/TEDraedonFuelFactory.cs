using CalamityMod.Items.DraedonMisc;
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
		public int Time = 0;
		public Item HeldItem = new Item();

		public override bool ValidTile(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			return tile.active() && tile.type == ModContent.TileType<DraedonFuelFactory>();
		}

		public override void Update()
		{
			HeldItem.favorited = false;
			if (HeldItem.type != ModContent.ItemType<PowerCell>())
			{
				HeldItem.SetDefaults(ModContent.ItemType<PowerCell>());
				HeldItem.stack = 0;
			}

			Time++;
			// Sometimes the item gets fucked up and it gets a maxStack of 0. Using it as a max can be unreliable as a result.
			bool rightTimeToMakeCell = Time % 5 == 4 && Main.tileFrame[Main.tile[Position.X, Position.Y].type] == 43;
			if (HeldItem.stack < 999 && rightTimeToMakeCell)
			{
				HeldItem.stack++;

				// Forcibly sync the tile entity whenever a cell is made.
				NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
			}
		}

		// If this factory breaks, anyone who's viewing it is no longer viewing it.
		public override void OnKill()
		{
			if (Main.LocalPlayer.Calamity().CurrentlyViewedFactory == this)
				Main.LocalPlayer.Calamity().CurrentlyViewedFactory = null;
		}

		public override void OnNetPlace()
		{
			HeldItem.SetDefaults(ModContent.ItemType<PowerCell>());
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
			TagCompound tag = new TagCompound
			{
				["Type"] = HeldItem.type,
				["Stack"] = HeldItem.stack,
				["Prefix"] = HeldItem.prefix,
				["NetID"] = HeldItem.active && HeldItem.stack > 0 ? HeldItem.netID : 0,
			};
			CalamityUtils.SaveModItem(tag, HeldItem);
			return tag;
		}

		public override void Load(TagCompound tag)
		{
			HeldItem = CalamityUtils.LoadModItem(tag);
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
