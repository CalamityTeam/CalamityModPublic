using System;
using System.Collections.Generic;

namespace CalamityMod.Cooldowns
{
	public class Cooldown
	{
		// Do not ever change this value. It is set when a cooldown is registered.
		internal ushort netID;

		/// <summary>
		/// Unique string ID of the cooldown. You can set this to whatever you want.
		/// </summary>
		public string ID = "";

		// Cooldowns are nothing other than identifiers. They serve as a minimal identification interface that is netcode compatible.
		// Everything else is handled by one of the following two types:
		// - CooldownInstance (if a player has a cooldown, this is what they have)
		// - CooldownHandler (every CooldownInstance has one: it handles the behavior of that cooldown instance)
		//
		// Individual "types" of cooldowns from a content sense are registered with a Handler.
		// Every cooldown MUST define a Handler to be registered and functional.
		// The differences between all the cooldowns are implemented as various subclasses of CooldownHandler.
		public CooldownHandler handler;

		internal Cooldown(string id, CooldownHandler h)
		{
			ID = id;
			handler = h;
		}

		#region Cooldown Registry
		// Indexed by ushort netID. Contains every registered cooldown.
		// Cooldowns are given netIDs when they are registered.
		// Cooldowns are useless until they are registered.
		public static Cooldown[] registry;
		private const ushort defaultSize = 256;
		private static ushort nextCDNetID = 0;

		private static Dictionary<string, ushort> nameToNetID = null;

		public static void Load()
		{
			registry = new Cooldown[defaultSize];
			nameToNetID = new Dictionary<string, ushort>(defaultSize);

			Register("AbyssalDivingSuitBreakingPlates", new DivingPlatesBreaking(null));
			Register("AbyssalDivingSuitBrokenPlates", new DivingPlatesBroken(null));
		}

		public static void Unload()
		{
			registry = null;
			nameToNetID?.Clear();
			nameToNetID = null;
		}

		/// <summary>
		/// Registers a cooldown for use in netcode. Cooldowns are useless until this has been done.
		/// </summary>
		/// <returns></returns>
		public static bool Register(string id, CooldownHandler h)
		{
			int currentMaxID = registry.Length;

			// This case only happens when you cap out at 65,536 cooldown registrations (which should never occur).
			// It just stops you from registering more cooldowns.
			if (nextCDNetID == currentMaxID)
				return false;

			Cooldown cd = new Cooldown(id, h);
			cd.netID = nextCDNetID;
			nameToNetID[cd.ID] = cd.netID;
			++nextCDNetID;

			// If the end of the array is reached, double its size.
			if (nextCDNetID == currentMaxID && currentMaxID < ushort.MaxValue)
			{
				Cooldown[] largerArray = new Cooldown[currentMaxID * 2];
				for (int i = 0; i < currentMaxID; ++i)
					largerArray[i] = registry[i];

				registry = largerArray;
			}
			return true;
		}
		#endregion
	}
}
