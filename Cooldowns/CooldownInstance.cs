using System;
using System.IO;
using Terraria;
using Terraria.ModLoader.IO;
using CalamityMod.UI;

namespace CalamityMod.Cooldowns
{
    public class CooldownInstance
    {
        private const string NetIDSaveKey = "netID";
        private const string DurationSaveKey = "duration";
        private const string TimeLeftSaveKey = "timeLeft";

        public CooldownInstance(Player p, Cooldown cd, int dur)
        {
            netID = cd.netID;
            player = p;
            duration = dur;
            timeLeft = dur;
            handler = null;
            AssignHandler(cd);
        }

        public CooldownInstance(Player p, Cooldown cd, int dur, params object[] args)
        {
            netID = cd.netID;
            player = p;
            duration = dur;
            timeLeft = dur;
            handler = null;
            AssignHandler(cd, args);
        }

        internal CooldownInstance(Player p, string id, TagCompound tag)
        {
            netID = (ushort)tag.GetAsInt(NetIDSaveKey);
            Cooldown cd = CooldownRegistry.Get(id);

            // It is possible this cooldown does not exist. If this occurs, log it, even though it will be (mostly) harmless.
            if (cd is null)
            {
                CalamityMod.Instance.Logger.Warn($"Cooldown \"{id}\" loaded from NBT, but was not found. This cooldown will not be applied to the player.");
                return;
            }

            // Correct for netID mismatch if for some reason this occurs.
            // The string ID of the cooldown overrides whatever the netID may have been saved as.
            ushort registeredNetID = cd.netID;

            if (netID != registeredNetID)
            {
                // Log when this occurs, even though it should be harmless.
                CalamityMod.Instance.Logger.Warn($"Cooldown \"{id}\" loaded from NBT with discrepant netID {netID}. This cooldown was registered with netID {registeredNetID}");
                netID = registeredNetID;
            }
            player = p;
            duration = tag.GetAsInt(DurationSaveKey);
            timeLeft = tag.GetAsInt(TimeLeftSaveKey);
            AssignHandler(cd);
        }

        /// <summary>
        /// Creates a cooldown instance from serialized binary data, used in netcode.
        /// </summary>
        /// <param name="reader">Reader of the binary stream.</param>
        internal CooldownInstance(BinaryReader reader)
        {
            netID = reader.ReadUInt16();
            byte playerIDByte = reader.ReadByte();
            player = Main.player[playerIDByte];
            duration = reader.ReadInt32();
            timeLeft = reader.ReadInt32();

            string id = CooldownRegistry.registry[netID].ID;
            AssignHandler(CooldownRegistry.Get(id));
        }

        // These two functions make the blind reflection assumption that every Cooldown passed to them will actually be
        // Cooldown<T : CooldownHandler>
        // (which they all will be, of course, because nobody instantiates Cooldown)

        internal void AssignHandler(Cooldown cd)
        {
            Type handlerT = cd.GetType().GenericTypeArguments[0];
            handler = Activator.CreateInstance(handlerT) as CooldownHandler;
            handler.instance = this;
        }

        internal void AssignHandler(Cooldown cd, params object[] args)
        {
            Type handlerT = cd.GetType().GenericTypeArguments[0];
            handler = Activator.CreateInstance(handlerT, args) as CooldownHandler;
            handler.instance = this;
        }

        /// <summary>
        /// The netID of the cooldown represented by this cooldown instance.<br/>
        /// This is used to look up gameplay behavior and rendering behavior as the cooldown instance is used in the game engine.
        /// </summary>
        internal ushort netID;

        /// <summary>
        /// The player that the cooldown is applying to
        /// </summary>
        public Player player;

        /// <summary>
        /// The total, original duration of the cooldown. This value is in frames, 60 per second.
        /// </summary>
        public int duration;

        /// <summary>
        /// The remaining duration of the cooldown. This value is in frames, 60 per second.
        /// </summary>
        public int timeLeft;

        /// <summary>
        /// A ratio of how "completed" an instance of a cooldown is.
        /// </summary>
        public float Completion => CooldownRackUI.DebugFullDisplay ? CooldownRackUI.DebugForceCompletion : (duration != 0 ? timeLeft / (float)duration : 0);

        /// <summary>
        /// The handler which implements the behavior of this cooldown instance.
        /// </summary>
        public CooldownHandler handler;

        /// <summary>
        /// Serializes this cooldown instance into a TagCompound for saving with the world.
        /// </summary>
        /// <returns>An NBT TagCompound which contains the essential variables of this cooldown instance.</returns>
        internal TagCompound Save()
        {
            return new TagCompound
            {
                { NetIDSaveKey, (int)netID },
                { DurationSaveKey, duration },
                { TimeLeftSaveKey, timeLeft }
            };
        }

        /// <summary>
        /// Serializes this cooldown instance into binary data for netcode.
        /// </summary>
        /// <param name="writer">Writer for an unspecified binary stream.</param>
        internal void Write(BinaryWriter writer)
        {
            writer.Write(netID);
            byte playerIDByte = (byte)player.whoAmI;
            writer.Write(playerIDByte);
            writer.Write(duration);
            writer.Write(timeLeft);
        }
    }
}
