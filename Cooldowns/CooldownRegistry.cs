using System.Collections.Generic;

namespace CalamityMod.Cooldowns
{
	public class CooldownRegistry
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

			// TODO -- CooldownHandlers should be ILoadable in 1.4

			// Vanilla cooldowns represented by the interface
			var potionSickness = Register<PotionSickness>(PotionSickness.ID);
			var chaosState = Register<ChaosState>(ChaosState.ID);
			var globalDodge = Register<GlobalDodge>(GlobalDodge.ID);

			// Calamity cooldowns
			var aquaticHeartIceShield = Register<AquaticHeartIceShield>(AquaticHeartIceShield.ID);
			var bloodflareFrenzy = Register<BloodflareFrenzy>(BloodflareFrenzy.ID);
			var bloodflareRanged = Register<BloodflareRangedSet>(BloodflareRangedSet.ID);
			var brimflameFrenzy = Register<BrimflameFrenzy>(BrimflameFrenzy.ID);
			var counterScarf = Register<CounterScarf>(CounterScarf.ID);
			var divineBless = Register<DivineBless>(DivineBless.ID);
			var divingPlatesBreaking = Register<DivingPlatesBreaking>(DivingPlatesBreaking.ID);
			var divingPlatesBroken = Register<DivingPlatesBroken>(DivingPlatesBroken.ID);
			var draconicElixir = Register<DraconicElixir>(DraconicElixir.ID);
			var evasionScarf = Register<EvasionScarf>(EvasionScarf.ID);
			var fleshTotem = Register<FleshTotem>(FleshTotem.ID);
			var godSlayerDash = Register<GodSlayerDash>(GodSlayerDash.ID);
			var inkBomb = Register<InkBomb>(InkBomb.ID);
			var lionHeartShield = Register<LionHeartShield>(LionHeartShield.ID);
			var nebulousCore = Register<NebulousCore>(NebulousCore.ID);
			var omegaBlue = Register<OmegaBlue>(OmegaBlue.ID);
			var permafrostConcoction = Register<PermafrostConcoction>(PermafrostConcoction.ID);
			var plagueBlackout = Register<PlagueBlackout>(PlagueBlackout.ID);
			var prismaticLaser = Register<PrismaticLaser>(PrismaticLaser.ID);
			var profanedSoulArtifact = Register<ProfanedSoulArtifact>(ProfanedSoulArtifact.ID);
			var relicOfResilience = Register<RelicOfResilience>(RelicOfResilience.ID);
			var rogueBooster = Register<RogueBooster>(RogueBooster.ID);
			var sandCloak = Register<SandCloak>(SandCloak.ID);
			var silvaRevive = Register<SilvaRevive>(SilvaRevive.ID);
			var tarragonCloak = Register<TarragonCloak>(TarragonCloak.ID);
			var tarragonImmunity = Register<TarragonImmunity>(TarragonImmunity.ID);
			var universeSplitter = Register<UniverseSplitter>(UniverseSplitter.ID);
		}

		public static void Unload()
		{
			registry = null;
			nameToNetID?.Clear();
			nameToNetID = null;
		}

		public static Cooldown Get(string id)
		{
			bool hasValue = nameToNetID.TryGetValue(id, out ushort netID);
			return hasValue ? registry[netID] : null;
		}

		/// <summary>
		/// Registers a CooldownHandler for use in netcode, assigning it a Cooldown and thus a netID. Cooldowns are useless until this has been done.
		/// </summary>
		/// <returns>The registered Cooldown.</returns>
		public static Cooldown<HandlerT> Register<HandlerT>(string id) where HandlerT : CooldownHandler
		{
			int currentMaxID = registry.Length;

			// This case only happens when you cap out at 65,536 cooldown registrations (which should never occur).
			// It just stops you from registering more cooldowns.
			if (nextCDNetID == currentMaxID)
				return null;

			Cooldown<HandlerT> cd = new Cooldown<HandlerT>(id, nextCDNetID);
			nameToNetID[cd.ID] = cd.netID;
			registry[cd.netID] = cd;
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
	}
}
