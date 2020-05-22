using CalamityMod.World;
using System;

namespace CalamityMod
{
	internal class Downed
	{
		public static readonly Func<bool> DownedDesertScourge = () => CalamityWorld.downedDesertScourge;
		public static readonly Func<bool> DownedGiantClam = () => CalamityWorld.downedCLAM;
		public static readonly Func<bool> DownedCrabulon = () => CalamityWorld.downedCrabulon;
		public static readonly Func<bool> DownedHiveMind = () => CalamityWorld.downedHiveMind;
		public static readonly Func<bool> DownedPerfs = () => CalamityWorld.downedPerforator;
		public static readonly Func<bool> DownedSlimeGod = () => CalamityWorld.downedSlimeGod;
		public static readonly Func<bool> DownedCryogen = () => CalamityWorld.downedCryogen;
		public static readonly Func<bool> DownedBrimstoneElemental = () => CalamityWorld.downedBrimstoneElemental;
		public static readonly Func<bool> DownedAquaticScourge = () => CalamityWorld.downedAquaticScourge;
		public static readonly Func<bool> DownedCalamitas = () => CalamityWorld.downedCalamitas;
		public static readonly Func<bool> DownedGSS = () => CalamityWorld.downedGSS;
		public static readonly Func<bool> DownedLeviathan = () => CalamityWorld.downedLeviathan;
		public static readonly Func<bool> DownedAureus = () => CalamityWorld.downedAstrageldon;
		public static readonly Func<bool> DownedPBG = () => CalamityWorld.downedPlaguebringer;
		public static readonly Func<bool> DownedRavager = () => CalamityWorld.downedScavenger;
		public static readonly Func<bool> DownedDeus = () => CalamityWorld.downedStarGod;
		public static readonly Func<bool> DownedGuardians = () => CalamityWorld.downedGuardians;
		public static readonly Func<bool> DownedBirb = () => CalamityWorld.downedBumble;
		public static readonly Func<bool> DownedProvidence = () => CalamityWorld.downedProvidence;
		public static readonly Func<bool> DownedCeaselessVoid = () => CalamityWorld.downedSentinel1;
		public static readonly Func<bool> DownedStormWeaver = () => CalamityWorld.downedSentinel2;
		public static readonly Func<bool> DownedSignus = () => CalamityWorld.downedSentinel3;
		public static readonly Func<bool> DownedPolterghast = () => CalamityWorld.downedPolterghast;
		public static readonly Func<bool> DownedBoomerDuke = () => CalamityWorld.downedBoomerDuke;
		public static readonly Func<bool> DownedDoG = () => CalamityWorld.downedDoG;
		public static readonly Func<bool> DownedYharon = () => CalamityWorld.downedYharon;
		public static readonly Func<bool> DownedSCal = () => CalamityWorld.downedSCal;

		public static readonly Func<bool> DownedAcidRainInitial = () => CalamityWorld.downedEoCAcidRain;
		public static readonly Func<bool> DownedAcidRainHardmode = () => CalamityWorld.downedAquaticScourgeAcidRain;
	}
}
