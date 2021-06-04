using CalamityMod.CustomRecipes;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Tiles.DraedonSummoner;
using Microsoft.Xna.Framework;
using System;
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
		public int InputtedCellCount;
		public int InitialCellCountBeforeDecrypting;
		public int HeldSchematicID;

		public int DecryptionCountdown;
		public int DecryptionTotalTime
		{
			get
			{
				// Decryption takes 35 minutes typically.
				int decryptTime = 126000;

				// However, if this codebreaker has a quantum cell, it only takes 1 minute.
				if (ContainsCoolingCell)
					decryptTime = 3600;
				return decryptTime;
			}
		}

		public int DecryptionCellCost
		{
			get
			{
				// You can't decrypt nothing.
				if (HeldSchematicID == 0 || !CalamityLists.EncryptedSchematicIDRelationship.ContainsKey(HeldSchematicID))
					return 0;

				int schematicType = CalamityLists.EncryptedSchematicIDRelationship[HeldSchematicID];
				if (schematicType == ModContent.ItemType<EncryptedSchematicPlanetoid>())
					return 500;
				if (schematicType == ModContent.ItemType<EncryptedSchematicJungle>())
					return 950;
				if (schematicType == ModContent.ItemType<EncryptedSchematicHell>())
					return 1750;
				if (schematicType == ModContent.ItemType<EncryptedSchematicIce>())
					return 5000;

				return 0;
			}
		}

		public float DecryptionCompletion => 1f - DecryptionCountdown / (float)DecryptionTotalTime;
		public bool ReadyToSummonDreadon => ContainsCoolingCell;

		public bool ContainsDecryptionComputer;
		public bool ContainsSensorArray;
		public bool ContainsAdvancedDisplay;
		public bool ContainsVoltageRegulationSystem;
		public bool ContainsCoolingCell;

		public bool CanDecryptHeldSchematic
		{
			get
			{
				// You can't decrypt nothing.
				if (HeldSchematicID == 0 || !CalamityLists.EncryptedSchematicIDRelationship.ContainsKey(HeldSchematicID))
					return false;

				int schematicType = CalamityLists.EncryptedSchematicIDRelationship[HeldSchematicID];
				if (schematicType == ModContent.ItemType<EncryptedSchematicPlanetoid>())
					return ContainsDecryptionComputer;
				if (schematicType == ModContent.ItemType<EncryptedSchematicJungle>())
					return ContainsDecryptionComputer && ContainsSensorArray;
				if (schematicType == ModContent.ItemType<EncryptedSchematicHell>())
					return ContainsDecryptionComputer && ContainsSensorArray && ContainsAdvancedDisplay;
				if (schematicType == ModContent.ItemType<EncryptedSchematicIce>())
					return ContainsDecryptionComputer && ContainsSensorArray && ContainsAdvancedDisplay && ContainsVoltageRegulationSystem;

				return false;
			}
		}

		public string UnderlyingSchematicText
		{
			get
			{
				// You can't decrypt nothing.
				if (HeldSchematicID == 0 || !CalamityLists.EncryptedSchematicIDRelationship.ContainsKey(HeldSchematicID))
					return string.Empty;

				int schematicType = CalamityLists.EncryptedSchematicIDRelationship[HeldSchematicID];
				if (schematicType == ModContent.ItemType<EncryptedSchematicPlanetoid>())
					return "test1";
				if (schematicType == ModContent.ItemType<EncryptedSchematicJungle>())
					return "test2";
				if (schematicType == ModContent.ItemType<EncryptedSchematicHell>())
					return "test3";
				if (schematicType == ModContent.ItemType<EncryptedSchematicIce>())
					return "test4";

				return string.Empty;
			}
		}

		public Vector2 Center => Position.ToWorldCoordinates(8f * CodebreakerTile.Width, 8f * CodebreakerTile.Height);
		public const int MaxCellCapacity = 9999;

		// This guarantees that this tile entity will not persist if not placed directly on the top left corner of a Codebreaker tile.
		public override bool ValidTile(int i, int j)
		{
			Tile tile = CalamityUtils.ParanoidTileRetrieval(i, j);
			return tile.active() && tile.type == ModContent.TileType<CodebreakerTile>() && tile.frameX == 0 && tile.frameY == 0;
		}

		// This code is called as a hook when the player places the Codebreaker tile so that the tile entity may be placed.
		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
		{
			// If in multiplayer, tell the server to place the tile entity and DO NOT place it yourself. That would mismatch IDs.
			// Also tell the server that you placed the 5x8 tiles that make up the Codebreaker.
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

		public override void Update() => UpdateTime();

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

			if (CalamityLists.EncryptedSchematicIDRelationship.ContainsKey(HeldSchematicID))
				Item.NewItem(x * 16, y * 16, 32, 32, CalamityLists.EncryptedSchematicIDRelationship[HeldSchematicID]);

			while (InputtedCellCount > 0)
			{
				int totalCellsToDrop = InputtedCellCount;
				if (totalCellsToDrop > 999)
					totalCellsToDrop = 999;
				InputtedCellCount -= totalCellsToDrop;
				Item.NewItem(x * 16, y * 16, 32, 32, ModContent.ItemType<PowerCell>(), totalCellsToDrop);
			}
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

		public void SyncContainedStuff()
		{
			// Don't bother sending packets in singleplayer.
			if (Main.netMode == NetmodeID.SinglePlayer)
				return;

			ModPacket packet = mod.GetPacket();
			packet.Write((byte)CalamityModMessageType.UpdateCodebreakerContainedStuff);
			packet.Write(ID);
			packet.Write(InputtedCellCount);
			packet.Write(InitialCellCountBeforeDecrypting);
			packet.Write(HeldSchematicID);
		}

		public static void ReadContainmentSync(Mod mod, BinaryReader reader)
		{
			int id = reader.ReadInt32();
			bool exists = ByID.TryGetValue(id, out TileEntity tileEntity);

			// Continue reading to the end even if a tile entity with the given ID does not exist.
			// Not doing this will cause errors/bugs.
			int cellCount = reader.ReadInt32();
			int cellCountBeforeDecrypting = reader.ReadInt32();
			int schematicID = reader.ReadInt32();

			// After doing reading, check again to see if the tile entity is actually there.
			// If it isn't don't bother doing anything else.
			if (!exists)
				return;

			// Furthermore, verify to ensure that the tile entity is a valid one.
			if (!(tileEntity is TECodebreaker codebreakerTileEntity))
				return;

			codebreakerTileEntity.InputtedCellCount = cellCount;
			codebreakerTileEntity.InitialCellCountBeforeDecrypting = cellCountBeforeDecrypting;
			codebreakerTileEntity.HeldSchematicID = schematicID;
		}

		public void SyncDecryptCountdown()
		{
			// Don't bother sending packets in singleplayer.
			if (Main.netMode == NetmodeID.SinglePlayer)
				return;

			ModPacket packet = mod.GetPacket();
			packet.Write((byte)CalamityModMessageType.UpdateCodebreakerDecryptCountdown);
			packet.Write(ID);
			packet.Write(DecryptionCountdown);
		}

		public static void ReadDecryptCountdownSync(Mod mod, BinaryReader reader)
		{
			int id = reader.ReadInt32();
			bool exists = ByID.TryGetValue(id, out TileEntity tileEntity);

			// Continue reading to the end even if a tile entity with the given ID does not exist.
			// Not doing this will cause errors/bugs.
			int countdown = reader.ReadInt32();

			// After doing reading, check again to see if the tile entity is actually there.
			// If it isn't don't bother doing anything else.
			if (!exists)
				return;

			// Furthermore, verify to ensure that the tile entity is a valid one.
			if (!(tileEntity is TECodebreaker codebreakerTileEntity))
				return;

			codebreakerTileEntity.DecryptionCountdown = countdown;
		}

		public void UpdateTime()
		{
			if (DecryptionCountdown > 0)
			{
				DecryptionCountdown--;

				// Gradually consume cells.
				if (DecryptionCountdown % 5 == 4)
				{
					InputtedCellCount = InitialCellCountBeforeDecrypting - (int)(DecryptionCellCost * DecryptionCompletion);
					if (Main.netMode != NetmodeID.MultiplayerClient)
						SyncContainedStuff();
				}

				if (DecryptionCountdown == 0)
				{
					// Reset the cell count prior to decrypting.
					InitialCellCountBeforeDecrypting = 0;

					LearnFromHeldSchematic(out bool anythingChanged);
					if (Main.netMode == NetmodeID.Server)
					{
						SyncDecryptCountdown();
						SyncContainedStuff();
					}
					else if (anythingChanged)
						CombatText.NewText(Main.LocalPlayer.Hitbox, Color.Cyan, "You learned how to create new things!", true);
				}
			}
		}

		public void LearnFromHeldSchematic(out bool anythingChanged)
		{
			anythingChanged = false;

			// Do nothing if no valid schematic is inputted at the moment.
			if (!CalamityLists.EncryptedSchematicIDRelationship.ContainsKey(HeldSchematicID))
				return;

			int schematicType = CalamityLists.EncryptedSchematicIDRelationship[HeldSchematicID];

			if (!RecipeUnlockHandler.HasUnlockedT2ArsenalRecipes && schematicType == ModContent.ItemType<EncryptedSchematicPlanetoid>())
			{
				RecipeUnlockHandler.HasUnlockedT2ArsenalRecipes = true;
				anythingChanged = true;
			}
			if (!RecipeUnlockHandler.HasUnlockedT3ArsenalRecipes && schematicType == ModContent.ItemType<EncryptedSchematicJungle>())
			{
				RecipeUnlockHandler.HasUnlockedT3ArsenalRecipes = true;
				anythingChanged = true;
			}
			if (!RecipeUnlockHandler.HasUnlockedT4ArsenalRecipes && schematicType == ModContent.ItemType<EncryptedSchematicHell>())
			{
				RecipeUnlockHandler.HasUnlockedT4ArsenalRecipes = true;
				anythingChanged = true;
			}
			if (!RecipeUnlockHandler.HasUnlockedT5ArsenalRecipes && schematicType == ModContent.ItemType<EncryptedSchematicIce>())
			{
				RecipeUnlockHandler.HasUnlockedT5ArsenalRecipes = true;
				anythingChanged = true;
			}

			if (Main.netMode == NetmodeID.Server && anythingChanged)
				CalamityNetcode.SyncWorld();
		}

		public override TagCompound Save()
		{
			return new TagCompound
			{
				["ContainsDecryptionComputer"] = ContainsDecryptionComputer,
				["ContainsSensorArray"] = ContainsSensorArray,
				["ContainsAdvancedDisplay"] = ContainsAdvancedDisplay,
				["ContainsVoltageRegulationSystem"] = ContainsVoltageRegulationSystem,
				["ContainsCoolingCell"] = ContainsCoolingCell,
				["InputtedCellCount"] = InputtedCellCount,
				["HeldSchematicID"] = HeldSchematicID,
				["DecryptionCountdown"] = DecryptionCountdown,
				["InitialCellCountBeforeDecrypting"] = InitialCellCountBeforeDecrypting
			};
		}

		public override void Load(TagCompound tag)
		{
			ContainsDecryptionComputer = tag.GetBool("ContainsDecryptionComputer");
			ContainsSensorArray = tag.GetBool("ContainsSensorArray");
			ContainsAdvancedDisplay = tag.GetBool("ContainsAdvancedDisplay");
			ContainsVoltageRegulationSystem = tag.GetBool("ContainsVoltageRegulationSystem");
			ContainsCoolingCell = tag.GetBool("ContainsCoolingCell");
			InputtedCellCount = tag.GetInt("InputtedCellCount");
			HeldSchematicID = tag.GetInt("HeldSchematicID");
			DecryptionCountdown = tag.GetInt("DecryptionCountdown");
			InitialCellCountBeforeDecrypting = tag.GetInt("InitialCellCountBeforeDecrypting");
		}

		public override void NetSend(BinaryWriter writer, bool lightSend)
		{
			writer.Write(ContainsDecryptionComputer);
			writer.Write(ContainsSensorArray);
			writer.Write(ContainsAdvancedDisplay);
			writer.Write(ContainsVoltageRegulationSystem);
			writer.Write(ContainsCoolingCell);
			writer.Write(InputtedCellCount);
			writer.Write(HeldSchematicID);
			writer.Write(DecryptionCountdown);
			writer.Write(InitialCellCountBeforeDecrypting);
		}

		public override void NetReceive(BinaryReader reader, bool lightReceive)
		{
			ContainsDecryptionComputer = reader.ReadBoolean();
			ContainsSensorArray = reader.ReadBoolean();
			ContainsAdvancedDisplay = reader.ReadBoolean();
			ContainsVoltageRegulationSystem = reader.ReadBoolean();
			ContainsCoolingCell = reader.ReadBoolean();
			InputtedCellCount = reader.ReadInt32();
			HeldSchematicID = reader.ReadInt32();
			DecryptionCountdown = reader.ReadInt32();
			InitialCellCountBeforeDecrypting = reader.ReadInt32();
		}
	}
}
