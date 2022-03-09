using System;
using System.IO;
using Terraria;

namespace CalamityMod.Cooldowns
{
	public struct CooldownInstance
	{
		public CooldownInstance(Player p, Cooldown<CooldownHandler> cd, int dur)
		{
			netID = cd.netID;
			player = p;
			duration = dur;
			timeLeft = dur;
			handler = null;
			AssignHandler(cd);
		}

		public CooldownInstance(Player p, Cooldown<CooldownHandler> cd, int dur, params object[] args)
		{
			netID = cd.netID;
			player = p;
			duration = dur;
			timeLeft = dur;
			handler = null;
			AssignHandler(cd, args);
		}

		internal void AssignHandler(Cooldown<CooldownHandler> cd)
		{
			handler = Activator.CreateInstance(cd.GetType().GenericTypeArguments[0]) as CooldownHandler;
			handler.instance = this;
		}

		internal void AssignHandler(Cooldown<CooldownHandler> cd, params object[] args)
		{
			handler = Activator.CreateInstance(cd.GetType().GenericTypeArguments[0], args) as CooldownHandler;
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
		public float Completion => duration != 0 ? timeLeft / (float)duration : 0;

		/// <summary>
		/// The handler which implements the behavior of this cooldown instance.
		/// </summary>
		public CooldownHandler handler;

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

		/// <summary>
		/// Defines this cooldown instance from serialized binary data, used in netcode.
		/// </summary>
		/// <param name="reader">Reader of the binary stream.</param>
		internal void Read(BinaryReader reader)
		{
			netID = reader.ReadUInt16();
			byte playerIDByte = reader.ReadByte();
			player = Main.player[playerIDByte];
			duration = reader.ReadInt32();
			timeLeft = reader.ReadInt32();
			AssignHandler(CooldownRegistry.registry[netID]);
		}
	}
}
