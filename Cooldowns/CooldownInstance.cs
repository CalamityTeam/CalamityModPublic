using System.IO;
using Terraria;

namespace CalamityMod.Cooldowns
{
	public struct CooldownInstance
	{
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

		internal void Write(BinaryWriter writer)
		{
			writer.Write(netID);
			byte playerIDByte = (byte)player.whoAmI;
			writer.Write(playerIDByte);
			writer.Write(duration);
			writer.Write(timeLeft);
		}

		internal void Read(BinaryReader reader)
		{
			netID = reader.ReadUInt16();
			byte playerIDByte = reader.ReadByte();
			player = Main.player[playerIDByte];
			duration = reader.ReadInt32();
			timeLeft = reader.ReadInt32();
		}
	}
}
