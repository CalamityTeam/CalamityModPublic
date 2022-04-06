using CalamityMod.CalPlayer;
using CalamityMod.Items;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Tiles.DraedonStructures;
using Microsoft.Xna.Framework;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.TileEntities
{
    public class TEChargingStation : ModTileEntity
    {
        public Vector2 Center => Position.ToWorldCoordinates(8f * ChargingStation.Width, 8f * ChargingStation.Height);
        private short ChargingTimer = 0;
        private short _stack = 0;
        public short CellStack
        {
            get => _stack;
            set
            {
                _stack = value;
                SendSyncPacket();
            }
        }
        public Item PluggedItem = new Item();
        private bool syncItemCharge = false;
        public bool ClientChargingDust = false;

        // This returns whether the charging station's inserted ("plugged") item can actually be charged.
        private bool PluggedItemCanCharge
        {
            get
            {
                if (PluggedItem is null || PluggedItem.IsAir)
                    return false;
                CalamityGlobalItem modItem = PluggedItem.Calamity();
                return modItem.UsesCharge && modItem.Charge < modItem.MaxCharge;
            }
        }
        public bool CanDoWork => _stack > 0 && PluggedItemCanCharge;

        // Red when not actively charging an item, green when charging
        public Color LightColor => CanDoWork ? Color.MediumSpringGreen : Color.Red;

        // This guarantees that this tile entity will not persist if not placed directly on the top left corner of a Charging Station tile.
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<ChargingStation>() && tile.TileFrameX == 0 && tile.TileFrameY == 0;
        }

        public override void Update()
        {
            bool canWork = CanDoWork;
            if (!canWork)
            {
                ChargingTimer = 0;
                return;
            }

            ++ChargingTimer;

            if (ChargingTimer >= ChargingStation.FramesPerChargeAction)
            {
                // Apply charge to the plugged item first, as this doesn't send a sync packet.
                CalamityGlobalItem modItem = PluggedItem.Calamity();
                modItem.Charge += PowerCell.ChargeValue;
                if (modItem.Charge >= modItem.MaxCharge)
                    modItem.Charge = modItem.MaxCharge;

                SpawnChargingDust();

                // Set the flag for syncing the plugged item's charge as well.
                syncItemCharge = true;

                // Then decrement the cell stack. With the sync flag set, this will sync both the cell stack and the plugged item.
                CellStack--;
                ChargingTimer = 0;
            }
        }

        public void SpawnChargingDust()
        {
            bool chargeComplete = false;
            if (!PluggedItem.IsAir)
            {
                CalamityGlobalItem modItem = PluggedItem.Calamity();
                chargeComplete = modItem.Charge == modItem.MaxCharge;
            }

            int dustID = 182; // Mechanical Cart laser dust. Looks epic.
            int numDust = 18;
            if (chargeComplete)
                numDust *= 3;
            Vector2 dustPos = Position.ToWorldCoordinates(ChargingStation.Width * 8f - 4f, 1f);
            for (int i = 0; i < numDust; i += 2)
            {
                float pairSpeed = Main.rand.NextFloat(0.5f, 7f);
                float pairScale = chargeComplete ? 2.4f : 1f;

                Dust d = Dust.NewDustDirect(dustPos, 0, 0, dustID);
                d.velocity = Vector2.UnitX * pairSpeed;
                d.scale = pairScale;
                d.noGravity = true;

                d = Dust.NewDustDirect(dustPos, 0, 0, dustID);
                d.velocity = Vector2.UnitX * -pairSpeed;
                d.scale = pairScale;
                d.noGravity = true;
            }
        }

        // This code is called as a hook when the player places the Charging Station tile so that the tile entity may be placed.
        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            // If in multiplayer, tell the server to place the tile entity and DO NOT place it yourself. That would mismatch IDs.
            // Also tell the server that you placed the 3x2 tiles that make up the Charging Station.
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, ChargingStation.Width, ChargingStation.Height);
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

        // If this charging station breaks, anyone's who's viewing it is no longer viewing it.
        public override void OnKill()
        {
            for (int i = 0; i < Main.maxPlayers; ++i)
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
                if (mp.CurrentlyViewedChargerID == ID)
                    mp.CurrentlyViewedChargerID = -1;
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("time", ChargingTimer);
            tag.Add("cells", _stack);
            Item forSaving;
            if (PluggedItem is null)
            {
                forSaving = new Item();
                forSaving.TurnToAir();
            }
            else
                forSaving = PluggedItem;
            tag.Add("item", forSaving);
        }

        public override void LoadData(TagCompound tag)
        {
            ChargingTimer = tag.GetShort("time");
            _stack = tag.GetShort("cells");
            PluggedItem = ItemIO.Load(tag.GetCompound("item"));
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(ChargingTimer);
            writer.Write(_stack);
            ItemIO.Send(PluggedItem, writer, true);
        }

        public override void NetReceive(BinaryReader reader)
        {
            ChargingTimer = reader.ReadInt16();
            _stack = reader.ReadInt16();
            PluggedItem = ItemIO.Receive(reader, true);
        }

        // This packet may or may not contain the plugged item's charge value. If it doesn't, it contains a dummy value (NaN) instead.
        // The plugged item's charge is synced when syncItemCharge is true.
        private void SendSyncPacket()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)CalamityModMessageType.ChargingStationStandard);
            packet.Write(ID);
            packet.Write(ChargingTimer);
            packet.Write(_stack);

            // Either write the real charge or garbage data. If garbage data is written it will be ignored on the other end.
            CalamityGlobalItem modItem = PluggedItem.IsAir ? null : PluggedItem.Calamity();
            packet.Write(syncItemCharge && modItem != null ? modItem.Charge : float.NaN);
            packet.Send(-1, -1);

            // If this flag was set to true, set it to false for any further updates (until it gets set to true again).
            syncItemCharge = false;
        }

        internal static bool ReadSyncPacket(Mod mod, BinaryReader reader)
        {
            int teID = reader.ReadInt32();
            bool exists = ByID.TryGetValue(teID, out TileEntity te);

            // The rest of the packet must be read even if it turns out the charging station doesn't exist for whatever reason.
            short timer = reader.ReadInt16();
            short cellStack = reader.ReadInt16();
            float chargeOrNaN = reader.ReadSingle();

            // When a server gets this packet, it immediately sends an equivalent packet to all clients.
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)CalamityModMessageType.ChargingStationStandard);
                packet.Write(teID);
                packet.Write(timer);
                packet.Write(cellStack);
                packet.Write(chargeOrNaN);
                packet.Send(-1, -1);
            }

            if (exists && te is TEChargingStation charger)
            {
                // Only clients update their timer from this packet. When a server receives this packet it ignores the time variable.
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    charger.ChargingTimer = timer;
                charger._stack = cellStack;

                // If the charge value sent is not garbage, then try to apply the new charge to the plugged item.
                if (!float.IsNaN(chargeOrNaN))
                {
                    bool itemExists = charger.PluggedItem != null && !charger.PluggedItem.IsAir;
                    CalamityGlobalItem modItem = itemExists ? charger.PluggedItem.Calamity() : null;
                    if (modItem != null && modItem.UsesCharge)
                    {
                        if (modItem.Charge != chargeOrNaN && Main.netMode == NetmodeID.MultiplayerClient)
                            charger.ClientChargingDust = true;
                        modItem.Charge = chargeOrNaN;
                    }
                }
                return true;
            }
            return false;
        }

        internal void SendItemSyncPacket()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = Mod.GetPacket(1024);
            packet.Write((byte)CalamityModMessageType.ChargingStationItemChange);
            packet.Write(ID);
            ItemIO.Send(PluggedItem, packet, true);
            packet.Send(-1, -1);
        }

        internal static bool ReadItemSyncPacket(Mod mod, BinaryReader reader)
        {
            int teID = reader.ReadInt32();
            bool exists = ByID.TryGetValue(teID, out TileEntity te);

            // The rest of the packet must be read even if it turns out the charging station doesn't exist for whatever reason.
            Item thePlug = ItemIO.Receive(reader, true);

            // When a server gets this packet, it immediately sends an equivalent packet to all clients.
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)CalamityModMessageType.ChargingStationItemChange);
                packet.Write(teID);
                ItemIO.Send(thePlug, packet, true);
                packet.Send(-1, -1);
            }

            if (exists && te is TEChargingStation charger)
            {
                charger.PluggedItem = thePlug;
                return true;
            }
            return false;
        }
    }
}
