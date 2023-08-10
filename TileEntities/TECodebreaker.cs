using CalamityMod.CustomRecipes;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.Materials;
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
        public int InputtedCellCount;
        public int InitialCellCountBeforeDecrypting;
        public int HeldSchematicID;

        public int DecryptionCountdown;
        public int DecryptionTotalTime
        {
            get
            {
                // Decryption takes 2 minutes typically.
                int decryptTime = 7200;

                // However, if this codebreaker has a quantum cell, it only takes 15 seconds.
                if (ContainsCoolingCell)
                    decryptTime = 900;
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
        public bool ReadyToSummonDraedon => ContainsCoolingCell && DecryptionCountdown <= 0;

        public bool ContainsDecryptionComputer;
        public bool ContainsSensorArray;
        public bool ContainsAdvancedDisplay;
        public bool ContainsVoltageRegulationSystem;
        public bool ContainsCoolingCell;

        public bool ContainsBloodSample;

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
                if (schematicType == ModContent.ItemType<EncryptedSchematicPlanetoid>()
                || schematicType == ModContent.ItemType<EncryptedSchematicJungle>()
                || schematicType == ModContent.ItemType<EncryptedSchematicHell>()
                || schematicType == ModContent.ItemType<EncryptedSchematicIce>())
                    return CalamityUtils.GetTextValueFromModItem(schematicType, "Content");

                return string.Empty;
            }
        }

        public Vector2 Center => Position.ToWorldCoordinates(8f * CodebreakerTile.Width, 8f * CodebreakerTile.Height);
        public const int MaxCellCapacity = 9999;

        // This guarantees that this tile entity will not persist if not placed directly on the top left corner of a Codebreaker tile.
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);
            return tile.HasTile && tile.TileType == ModContent.TileType<CodebreakerTile>() && tile.TileFrameX == 0 && tile.TileFrameY == 0;
        }

        // This code is called as a hook when the player places the Codebreaker tile so that the tile entity may be placed.
        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            // If in multiplayer, tell the server to place the tile entity and DO NOT place it yourself. That would mismatch IDs.
            // Also tell the server that you placed the 5x8 tiles that make up the Codebreaker.
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, CodebreakerTile.Width, CodebreakerTile.Height);
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
            var source = new EntitySource_TileEntity(this);
            if (ContainsDecryptionComputer)
                Item.NewItem(source, x * 16, y * 16, 32, 32, ModContent.ItemType<DecryptionComputer>());
            if (ContainsSensorArray)
                Item.NewItem(source, x * 16, y * 16, 32, 32, ModContent.ItemType<LongRangedSensorArray>());
            if (ContainsAdvancedDisplay)
                Item.NewItem(source, x * 16, y * 16, 32, 32, ModContent.ItemType<AdvancedDisplay>());
            if (ContainsVoltageRegulationSystem)
                Item.NewItem(source, x * 16, y * 16, 32, 32, ModContent.ItemType<VoltageRegulationSystem>());
            if (ContainsCoolingCell)
                Item.NewItem(source, x * 16, y * 16, 32, 32, ModContent.ItemType<AuricQuantumCoolingCell>());

            if (CalamityLists.EncryptedSchematicIDRelationship.ContainsKey(HeldSchematicID))
                Item.NewItem(source, x * 16, y * 16, 32, 32, CalamityLists.EncryptedSchematicIDRelationship[HeldSchematicID]);

            while (InputtedCellCount > 0)
            {
                int totalCellsToDrop = InputtedCellCount;
                if (totalCellsToDrop > 999)
                    totalCellsToDrop = 999;

                InputtedCellCount -= totalCellsToDrop;
                int itemType = ContainsBloodSample ? ModContent.ItemType<BloodSample>() : ModContent.ItemType<DraedonPowerCell>();
                Item.NewItem(new EntitySource_TileEntity(this), x * 16, y * 16, 32, 32, itemType, totalCellsToDrop);
            }
        }

        public void SyncConstituents(short sender)
        {
            // Don't bother sending packets in singleplayer.
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            ModPacket packet = Mod.GetPacket();
            BitsByte containmentFlagWrapper = new BitsByte();
            containmentFlagWrapper[0] = ContainsDecryptionComputer;
            containmentFlagWrapper[1] = ContainsSensorArray;
            containmentFlagWrapper[2] = ContainsAdvancedDisplay;
            containmentFlagWrapper[3] = ContainsVoltageRegulationSystem;
            containmentFlagWrapper[4] = ContainsCoolingCell;

            packet.Write((byte)CalamityModMessageType.UpdateCodebreakerConstituents);
            packet.Write(ID);
            packet.Write(sender);
            packet.Write(containmentFlagWrapper);
            packet.Send(-1, sender);
        }

        public static void ReadConstituentsUpdateSync(Mod mod, BinaryReader reader)
        {
            int id = reader.ReadInt32();
            short sender = reader.ReadInt16();
            bool exists = ByID.TryGetValue(id, out TileEntity tileEntity);

            // Continue reading to the end even if a tile entity with the given ID does not exist.
            // Not doing this will cause errors/bugs.
            BitsByte containmentFlagWrapper = reader.ReadByte();
            bool containsDecryptionComputer = containmentFlagWrapper[0];
            bool containsSensorArray = containmentFlagWrapper[1];
            bool containsAdvancedDisplay = containmentFlagWrapper[2];
            bool containsVoltageRegulationSystem = containmentFlagWrapper[3];
            bool containsCoolingCell = containmentFlagWrapper[4];

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

            // Send the packet again to the other clients if this packet was received on the server.
            // Since ModPackets go solely to the server when sent by a client this is necesssary
            // to ensure that all clients are informed of what happened.
            if (Main.netMode == NetmodeID.Server)
                codebreakerTileEntity.SyncConstituents(sender);
        }

        public void SyncContainedStuff()
        {
            // Don't bother sending packets in singleplayer.
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)CalamityModMessageType.UpdateCodebreakerContainedStuff);
            packet.Write(ID);
            packet.Write(InputtedCellCount);
            packet.Write(InitialCellCountBeforeDecrypting);
            packet.Write(HeldSchematicID);
            packet.Write(ContainsBloodSample);
            packet.Send();
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

            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)CalamityModMessageType.UpdateCodebreakerDecryptCountdown);
            packet.Write(ID);
            packet.Write(DecryptionCountdown);
            packet.Send();
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
                if (DecryptionCountdown % 5 == 0)
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
                        CombatText.NewText(Main.LocalPlayer.Hitbox, Color.Cyan, CalamityUtils.GetTextValue("Misc.LearnedSchematic"), true);
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

        public override void SaveData(TagCompound tag)
        {
            tag["ContainsDecryptionComputer"] = ContainsDecryptionComputer;
            tag["ContainsSensorArray"] = ContainsSensorArray;
            tag["ContainsAdvancedDisplay"] = ContainsAdvancedDisplay;
            tag["ContainsVoltageRegulationSystem"] = ContainsVoltageRegulationSystem;
            tag["ContainsCoolingCell"] = ContainsCoolingCell;
            tag["InputtedCellCount"] = InputtedCellCount;
            tag["HeldSchematicID"] = HeldSchematicID;
            tag["DecryptionCountdown"] = DecryptionCountdown;
            tag["InitialCellCountBeforeDecrypting"] = InitialCellCountBeforeDecrypting;
            tag["ContainsBloodSample"] = ContainsBloodSample;
        }

        public override void LoadData(TagCompound tag)
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
            ContainsBloodSample = tag.GetBool("ContainsBloodSample");
        }

        public override void NetSend(BinaryWriter writer)
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
            writer.Write(ContainsBloodSample);
        }

        public override void NetReceive(BinaryReader reader)
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
            ContainsBloodSample= reader.ReadBoolean();
        }
    }
}
