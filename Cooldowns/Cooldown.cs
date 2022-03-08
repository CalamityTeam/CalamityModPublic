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
			Cooldown potionSickness = Register(PotionSickness.ID, new PotionSickness(null));
			Cooldown chaosState = Register(ChaosState.ID, new ChaosState(null));
			Cooldown globalDodge = Register(GlobalDodge.ID, new GlobalDodge(null));

			// Calamity cooldowns
			Cooldown aquaticHeartIceShield = Register(AquaticHeartIceShield.ID, new AquaticHeartIceShield(null));
			Cooldown bloodflareFrenzy = Register(BloodflareFrenzy.ID, new BloodflareFrenzy(null));
			Cooldown bloodflareRanged = Register(BloodflareRangedSet.ID, new BloodflareRangedSet(null));
			Cooldown brimflameFrenzy = Register(BrimflameFrenzy.ID, new BrimflameFrenzy(null));
			Cooldown counterScarf = Register(CounterScarf.ID, new CounterScarf(null));
			Cooldown divineBless = Register(DivineBless.ID, new DivineBless(null));
			Cooldown divingPlatesBreaking = Register(DivingPlatesBreaking.ID, new DivingPlatesBreaking(null));
			Cooldown divingPlatesBroken = Register(DivingPlatesBroken.ID, new DivingPlatesBroken(null));
			Cooldown draconicElixir = Register(DraconicElixir.ID, new DraconicElixir(null));
			Cooldown evasionScarf = Register(EvasionScarf.ID, new EvasionScarf(null));
			Cooldown fleshTotem = Register(FleshTotem.ID, new FleshTotem(null));
			Cooldown godSlayerDash = Register(GodSlayerDash.ID, new GodSlayerDash(null));
			Cooldown inkBomb = Register(InkBomb.ID, new InkBomb(null));
			Cooldown lionHeartShield = Register(LionHeartShield.ID, new LionHeartShield(null));
			Cooldown nebulousCore = Register(NebulousCore.ID, new NebulousCore(null));
			Cooldown omegaBlue = Register(OmegaBlue.ID, new OmegaBlue(null));
			Cooldown permafrostConcoction = Register(PermafrostConcoction.ID, new PermafrostConcoction(null));
			Cooldown plagueBlackout = Register(PlagueBlackout.ID, new PlagueBlackout(null));
			Cooldown prismaticLaser = Register(PrismaticLaser.ID, new PrismaticLaser(null));
			Cooldown profanedSoulArtifact = Register(ProfanedSoulArtifact.ID, new ProfanedSoulArtifact(null));
			Cooldown relicOfResilience = Register(RelicOfResilience.ID, new RelicOfResilience(null));
			Cooldown rogueBooster = Register(RogueBooster.ID, new RogueBooster(null));
			Cooldown sandCloak = Register(SandCloak.ID, new SandCloak(null));
			Cooldown silvaRevive = Register(SilvaRevive.ID, new SilvaRevive(null));
			Cooldown tarragonCloak = Register(TarragonCloak.ID, new TarragonCloak(null));
			Cooldown tarragonImmunity = Register(TarragonImmunity.ID, new TarragonImmunity(null));
			Cooldown universeSplitter = Register(UniverseSplitter.ID, new UniverseSplitter(null));
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
