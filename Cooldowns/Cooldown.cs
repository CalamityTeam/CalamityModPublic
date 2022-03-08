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

			// TODO -- CooldownHandlers should be ILoadable in 1.4

			// Vanilla cooldowns represented by the interface
			Cooldown potionSickness = Register("PotionSickness", new PotionSickness(null));
			Cooldown chaosState = Register("ChaosState", new ChaosState(null));
			Cooldown globalDodge = Register("GlobalDodge", new GlobalDodge(null));

			// Calamity cooldowns
			Cooldown aquaticHeartIceShield = Register("AquaticHeartIceShield", new AquaticHeartIceShield(null));
			Cooldown bloodflareFrenzy = Register("BloodflareFrenzy", new BloodflareFrenzy(null));
			Cooldown bloodflareRanged = Register("BloodflareRangedSet", new BloodflareRangedSet(null));
			Cooldown brimflameFrenzy = Register("BrimflameFrenzy", new BrimflameFrenzy(null));
			Cooldown counterScarf = Register("CounterScarf", new CounterScarf(null));
			Cooldown divineBless = Register("DivineBless", new DivineBless(null));
			Cooldown divingPlatesBreaking = Register("DivingPlatesBreaking", new DivingPlatesBreaking(null));
			Cooldown divingPlatesBroken = Register("DivingPlatesBroken", new DivingPlatesBroken(null));
			Cooldown draconicElixir = Register("DraconicElixir", new DraconicElixir(null));
			Cooldown evasionScarf = Register("EvasionScarf", new EvasionScarf(null));
			Cooldown fleshTotem = Register("FleshTotem", new FleshTotem(null));
			Cooldown godSlayerDash = Register("GodSlayerDash", new GodSlayerDash(null));
			Cooldown inkBomb = Register("InkBomb", new InkBomb(null));
			Cooldown lionHeartShield = Register("LionHeartShield", new LionHeartShield(null));
			Cooldown nebulousCore = Register("NebulousCore", new NebulousCore(null));
			Cooldown omegaBlue = Register("OmegaBlue", new OmegaBlue(null));
			Cooldown permafrostConcoction = Register("PermafrostConcoction", new PermafrostConcoction(null));
			Cooldown plagueBlackout = Register("PlagueBlackout", new PlagueBlackout(null));
			Cooldown prismaticLaser = Register("PrismaticLaser", new PrismaticLaser(null));
			Cooldown profanedSoulArtifact = Register("ProfanedSoulArtifact", new ProfanedSoulArtifact(null));
			Cooldown relicOfResilience = Register("RelicOfResilience", new RelicOfResilience(null));
			Cooldown rogueBooster = Register("RogueBooster", new RogueBooster(null));
			Cooldown sandCloak = Register("SandCloak", new SandCloak(null));
			Cooldown silvaRevive = Register("SilvaRevive", new SilvaRevive(null));
			Cooldown tarragonCloak = Register("TarragonCloak", new TarragonCloak(null));
			Cooldown universeSplitter = Register("UniverseSplitter", new UniverseSplitter(null));
		}

		public static void Unload()
		{
			registry = null;
			nameToNetID?.Clear();
			nameToNetID = null;
		}

		/// <summary>
		/// Registers a CooldownHandler for use in netcode, assigning it a Cooldown and thus a netID. Cooldowns are useless until this has been done.
		/// </summary>
		/// <returns>The registered Cooldown.</returns>
		public static Cooldown Register(string id, CooldownHandler h)
		{
			int currentMaxID = registry.Length;

			// This case only happens when you cap out at 65,536 cooldown registrations (which should never occur).
			// It just stops you from registering more cooldowns.
			if (nextCDNetID == currentMaxID)
				return null;

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
			return cd;
		}
		#endregion
	}
}
