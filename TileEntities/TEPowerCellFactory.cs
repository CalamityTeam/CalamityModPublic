using CalamityMod.CalPlayer;
using CalamityMod.Tiles;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.TileEntities
{
	public class TEPowerCellFactory : ModTileEntity
	{
		public long Time = 0;
		private short _stack = 0;
		public short CellStack
		{
			get => _stack;
			set
			{
				_stack = value;
				// Sends a vanilla Tile Entity sync packet every time the number of cells in this Cell Factory changes.
				// This will be hijacked by vanilla NetSend and NetReceive to send the necessary data.
				NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
			}
		}

		private long CycleFrameCounter
		{
			get
			{
				long totalCycleTime = PowerCellFactory.BetweenCellDowntime + PowerCellFactory.TotalFrames * PowerCellFactory.AnimationFramerate;
				return Time % totalCycleTime;
			}
		}
		
		private bool IsCellFrame
		{
			get
			{
				long magicFrame = PowerCellFactory.BetweenCellDowntime + PowerCellFactory.CellCreateFrame * PowerCellFactory.AnimationFramerate + PowerCellFactory.MagicFrameDelay;
				return CycleFrameCounter == magicFrame;
			}
		}

		// Property which allows anyone to get the current animation frame of this specific factory.
		public int AnimationFrame
		{
			get
			{
				int f = (int)CycleFrameCounter;

				// The animation sticks on the last frame throughout the entire downtime period.
				if (f < PowerCellFactory.BetweenCellDowntime)
					return PowerCellFactory.TotalFrames - 1;

				// Remove the starting downtime period for the framerate divisor calculation.
				return (f - PowerCellFactory.BetweenCellDowntime) / PowerCellFactory.AnimationFramerate;
			}
		}

		// This guarantees that this tile entity will not persist if not placed directly on the top left corner of a Power Cell Factory tile.
		public override bool ValidTile(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			return tile.active() && tile.type == ModContent.TileType<PowerCellFactory>() && tile.frameX == 0 && tile.frameY == 0;
		}

		public override void Update()
		{
			++Time;
			if (IsCellFrame && CellStack < 999)
				// The property setter will automatically send the necessary packet.
				CellStack++;
		}

		// This code is called as a hook when the player places the Power Cell Factory tile so that the tile entity may be placed.
		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
		{
			// If in multiplayer, tell the server to place the tile entity and DO NOT place it yourself. That will mismatch IDs.
			// Also tell the server that you placed the 4x4 tiles that make up the Power Cell Factory.
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				NetMessage.SendTileRange(Main.myPlayer, i, j, PowerCellFactory.Width, PowerCellFactory.Height);
				NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type);
				return -1;
			}

			// If in single player, just place the tile entity, no problems.
			int id = Place(i, j);
			return id;
		}

		// This code is called on dedicated servers only. It is the server-side response to MessageID.TileEntityPlacement.
		// When the server receives such a message from a client, it sends a MessageID.TileEntitySharing to all clients.
		// This will cause them to Place the tile entity locally at that position, all with exactly the same ID.
		public override void OnNetPlace() => NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);

		// If this factory breaks, anyone who's viewing it is no longer viewing it.
		public override void OnKill()
		{
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player p = Main.player[i];
				if (!p.active)
					continue;

				// Use reflection to stop TML from spitting an error here.
				// Try-catching will not stop this error, TML will print it to console anyway. The error is harmless.
				ModPlayer[] mpStorageArray = (ModPlayer[])typeof(Player).GetField("modPlayers", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(p);
				if (mpStorageArray.Length == 0)
					continue;

				CalamityPlayer mp = p.Calamity();
				TEPowerCellFactory factory = mp.CurrentlyViewedFactory;
				if (factory is null)
					continue;
				if (factory.ID == ID)
				{
					mp.CurrentlyViewedFactory = null;
					mp.CurrentlyViewedFactoryX = -1;
					mp.CurrentlyViewedFactoryY = -1;
				}
			}
		}

		public override TagCompound Save() => new TagCompound
		{
			{ "time", Time },
			{ "cells", _stack }
		};

		public override void Load(TagCompound tag)
		{
			Time = tag.GetLong("time");
			_stack = tag.GetShort("cells");
		}

		public override void NetSend(BinaryWriter writer, bool lightSend)
		{
			writer.Write(Time);
			writer.Write(_stack);
		}

		public override void NetReceive(BinaryReader reader, bool lightReceive)
		{
			Time = reader.ReadInt64();
			_stack = reader.ReadInt16();
		}
	}
}
