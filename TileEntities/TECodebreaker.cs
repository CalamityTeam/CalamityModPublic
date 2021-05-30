using CalamityMod.Items.DraedonMisc;
using CalamityMod.Tiles.DraedonStructures;
using CalamityMod.Tiles.DraedonSummoner;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.TileEntities
{
	public class TECodebreaker : ModTileEntity
	{
		public bool ContainsDecryptionComputer;
		public bool ContainsSensorArray;
		public bool ContainsAdvancedDisplay;
		public bool ContainsVoltageRegulationSystem;
		public bool ContainsCoolingCell;
		public Vector2 Center => Position.ToWorldCoordinates(8f * CodebreakerTile.Width, 8f * CodebreakerTile.Height);

		// This guarantees that this tile entity will not persist if not placed directly on the top left corner of a Charging Station tile.
		public override bool ValidTile(int i, int j)
		{
			Tile tile = CalamityUtils.ParanoidTileRetrieval(i, j);
			return tile.active() && tile.type == ModContent.TileType<CodebreakerTile>() && tile.frameX == 0 && tile.frameY == 0;
		}

		// This code is called as a hook when the player places the Charging Station tile so that the tile entity may be placed.
		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
		{
			// If in multiplayer, tell the server to place the tile entity and DO NOT place it yourself. That would mismatch IDs.
			// Also tell the server that you placed the 3x2 tiles that make up the Charging Station.
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				NetMessage.SendTileRange(Main.myPlayer, i, j, CodebreakerTile.Width, CodebreakerTile.Height);
				NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type);
				return -1;
			}

			// If in single player, just place the tile entity, no problems.
			return Place(i, j);
		}

		// This code is called on dedicated servers only. It is the server-side response to MessageID.TileEntityPlacement.
		// When the server receives such a message from a client, it sends a MessageID.TileEntitySharing to all clients.
		// This will cause them to Place the tile entity locally at that position, all with exactly the same ID.
		public override void OnNetPlace() => NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);

		public void DropConstituents(int x, int y)
		{
			if (ContainsDecryptionComputer)
				Item.NewItem(x * 16, y * 16, 32, 32, ModContent.ItemType<DecryptionComputer>());
			if (ContainsSensorArray)
				Item.NewItem(x * 16, y * 16, 32, 32, ModContent.ItemType<LongRangedSensorArray>());
			if (ContainsAdvancedDisplay)
				Item.NewItem(x * 16, y * 16, 32, 32, ModContent.ItemType<AdvancedDisplay>());
			if (ContainsVoltageRegulationSystem)
				Item.NewItem(x * 16, y * 16, 32, 32, ModContent.ItemType<VoltageRegulationSystem>());
			if (ContainsCoolingCell)
				Item.NewItem(x * 16, y * 16, 32, 32, ModContent.ItemType<AuricQuantumCoolingCell>());
        }

		public void SyncConstituents()
		{
			// Don't bother sending packets in singleplayer.
			if (Main.netMode == NetmodeID.SinglePlayer)
				return;

			ModPacket packet = mod.GetPacket();
			packet.Write((byte)CalamityModMessageType.UpdateCodebreakerConstituents);
			packet.Write(ID);
			packet.Write(ContainsDecryptionComputer);
			packet.Write(ContainsSensorArray);
			packet.Write(ContainsAdvancedDisplay);
			packet.Write(ContainsVoltageRegulationSystem);
			packet.Write(ContainsCoolingCell);
		}

		public static void ReadConstituentsUpdateSync(Mod mod, BinaryReader reader)
		{
			int id = reader.ReadInt32();
			bool exists = ByID.TryGetValue(id, out TileEntity tileEntity);

			// Continue reading to the end even if a tile entity with the given ID does not exist.
			// Not doing this will cause errors/bugs.
			bool containsDecryptionComputer = reader.ReadBoolean();
			bool containsSensorArray = reader.ReadBoolean();
			bool containsAdvancedDisplay = reader.ReadBoolean();
			bool containsVoltageRegulationSystem = reader.ReadBoolean();
			bool containsCoolingCell = reader.ReadBoolean();

			// After doing reading, check again to see if the tile entity is actually there.
			// If it isn't don't bother doing anything else.
			if (!exists)
				return;

			// Furthermore, verify to ensure that the tile entity is a valid one.
			if (!(tileEntity is TECodebreaker codebreakerTileEntity))
				return;

			codebreakerTileEntity.ContainsDecryptionComputer = containsDecryptionComputer;
			codebreakerTileEntity.ContainsSensorArray = containsSensorArray;
			codebreakerTileEntity.ContainsAdvancedDisplay = containsAdvancedDisplay;
			codebreakerTileEntity.ContainsVoltageRegulationSystem = containsVoltageRegulationSystem;
			codebreakerTileEntity.ContainsCoolingCell = containsCoolingCell;
		}

		public override TagCompound Save()
		{
			return new TagCompound
			{
				["ContainsDecryptionComputer"] = ContainsDecryptionComputer,
				["ContainsSensorArray"] = ContainsSensorArray,
				["ContainsAdvancedDisplay"] = ContainsAdvancedDisplay,
				["ContainsVoltageRegulationSystem"] = ContainsVoltageRegulationSystem,
				["ContainsCoolingCell"] = ContainsCoolingCell
			};
		}

		public override void Load(TagCompound tag)
		{
			ContainsDecryptionComputer = tag.GetBool("ContainsDecryptionComputer");
			ContainsSensorArray = tag.GetBool("ContainsSensorArray");
			ContainsAdvancedDisplay = tag.GetBool("ContainsAdvancedDisplay");
			ContainsVoltageRegulationSystem = tag.GetBool("ContainsVoltageRegulationSystem");
			ContainsCoolingCell = tag.GetBool("ContainsCoolingCell");
		}

		public override void NetSend(BinaryWriter writer, bool lightSend)
		{
			writer.Write(ContainsDecryptionComputer);
			writer.Write(ContainsSensorArray);
			writer.Write(ContainsAdvancedDisplay);
			writer.Write(ContainsVoltageRegulationSystem);
			writer.Write(ContainsCoolingCell);
		}

		public override void NetReceive(BinaryReader reader, bool lightReceive)
		{
			ContainsDecryptionComputer = reader.ReadBoolean();
			ContainsSensorArray = reader.ReadBoolean();
			ContainsAdvancedDisplay = reader.ReadBoolean();
			ContainsVoltageRegulationSystem = reader.ReadBoolean();
			ContainsCoolingCell = reader.ReadBoolean();
		}
	}
}
