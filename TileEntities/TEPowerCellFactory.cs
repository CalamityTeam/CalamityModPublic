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
				NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID);
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
				TEPowerCellFactory fact = mp.CurrentlyViewedFactory;
				if (fact is null)
					continue;
				if (fact.ID == ID)
				{
					mp.CurrentlyViewedFactory = null;
					mp.CurrentlyViewedFactoryX = -1;
					mp.CurrentlyViewedFactoryY = -1;
				}
			}
		}

		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
		{
			// If a client places this tile it must notify the server that it was placed at these coordinates.
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				NetMessage.SendTileSquare(Main.myPlayer, i, j, 5);
				NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type);
				return -1;
			}
			return Place(i, j);
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
