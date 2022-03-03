using System.Collections.Generic;

namespace CalamityMod.Cooldowns
{
	public static class CooldownRegistry
	{
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
		/// <param name="cd"></param>
		/// <returns></returns>
		public static bool Register(Cooldown cd)
		{
			int currentMaxID = registry.Length;

			// This case only happens when you cap out at 65,536 cooldown registrations (which should never occur).
			// It just stops you from registering more cooldowns.
			if (nextCDNetID == currentMaxID)
				return false;

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
	}
}
