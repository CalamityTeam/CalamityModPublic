using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.GreatSandShark;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod
{
	public static partial class NPCStats
	{
		#region Boss Stats Container Struct
		internal partial struct BossStats
		{
			public static SortedDictionary<int, Tuple<bool, int[]>> DebuffImmunities;
		};
		#endregion

		#region Stat Retrieval Methods
		public static void SetNPCDebuffImmunities(this NPC npc)
		{
			// Safety check: If for some reason the debuff array is not initialized yet, return and do nothing.
			// If the npc is not in the dictionary, return and do nothing.
			// Also, can I just say that I hate Sorted Dictionaries and Tuples and want to make something explode? -Ben
			var stupidTupleThing = new Tuple<bool, int[]>(true, new int[] {});
			bool exists = BossStats.DebuffImmunities.TryGetValue(npc.type, out stupidTupleThing);
			if (npc.modNPC is null || !exists)
				return;

			// If the npc is immune to everything, make it immune to everything
			if (stupidTupleThing.Item1)
			{
				for (int k = 0; k < npc.buffImmune.Length; k++)
				{
					npc.buffImmune[k] = true;
				}
			}
			// Then set debuff vulnerabilities, or immunities if not immune to everything
			for (int i = 0; i < stupidTupleThing.Item2.Length; ++i)
			{
				npc.buffImmune[stupidTupleThing.Item2[i]] = !stupidTupleThing.Item1;
			}
		}
		#endregion

		#region Load/Unload
		// A static function, called exactly once, which initializes the BossStats struct at a predictable time.
		// This is necessary to ensure this dictionary is populated as early as possible.
		internal static void LoadDebuffs()
		{
			BossStats.DebuffImmunities = new SortedDictionary<int, Tuple<bool, int[]>>
			{
				{ ModContent.NPCType<KingSlimeJewel>(), new Tuple<bool, int[]>(true, new int[] { }) },

				{ ModContent.NPCType<DesertScourgeHead>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<DesertScourgeBody>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<DesertScourgeTail>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<DesertScourgeHeadSmall>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<DesertScourgeBodySmall>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<DesertScourgeTailSmall>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<DriedSeekerHead>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<DriedSeekerBody>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<DriedSeekerTail>(), new Tuple<bool, int[]>(false, new int[] { }) },

				{ ModContent.NPCType<CrabulonIdle>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<CrabShroom>(), new Tuple<bool, int[]>(false, new int[] { }) },

				{ ModContent.NPCType<HiveMind>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<HiveMindP2>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<DankCreeper>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<DarkHeart>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<HiveBlob>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<HiveBlob2>(), new Tuple<bool, int[]>(false, new int[] { }) },

				{ ModContent.NPCType<PerforatorHive>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<PerforatorHeadLarge>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<PerforatorBodyLarge>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<PerforatorTailLarge>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<PerforatorHeadMedium>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<PerforatorBodyMedium>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<PerforatorTailMedium>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<PerforatorHeadSmall>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<PerforatorBodySmall>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<PerforatorTailSmall>(), new Tuple<bool, int[]>(false, new int[] { }) },

				{ ModContent.NPCType<SlimeGodCore>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<SlimeGod>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<SlimeGodSplit>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<SlimeGodRun>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<SlimeGodRunSplit>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<SlimeSpawnCorrupt>(), new Tuple<bool, int[]>(false, new int[] { BuffID.OnFire }) },
				{ ModContent.NPCType<SlimeSpawnCorrupt2>(), new Tuple<bool, int[]>(false, new int[] { BuffID.OnFire }) },
				{ ModContent.NPCType<SlimeSpawnCrimson>(), new Tuple<bool, int[]>(false, new int[] { BuffID.OnFire }) },
				{ ModContent.NPCType<SlimeSpawnCrimson2>(), new Tuple<bool, int[]>(false, new int[] { BuffID.OnFire }) },

				{ ModContent.NPCType<Cryogen>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.BoneJavelin,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.OnFire,
					BuffID.ShadowFlame,
					BuffID.SoulDrain,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<BrimstoneFlames>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<MarkedforDeath>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Plague>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<SulphuricPoisoning>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<CryogenIce>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<Cryocore>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<Cryocore2>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<IceMass>(), new Tuple<bool, int[]>(false, new int[] { }) },

				{ ModContent.NPCType<AquaticScourgeHead>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<AquaticScourgeBody>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<AquaticScourgeBodyAlt>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<AquaticScourgeTail>(), new Tuple<bool, int[]>(true, new int[] { }) },

				{ ModContent.NPCType<BrimstoneElemental>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.BoneJavelin,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Frostburn,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.SoulDrain,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<MarkedforDeath>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Plague>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<SulphuricPoisoning>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<Brimling>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.BoneJavelin,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Frostburn,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.SoulDrain,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<MarkedforDeath>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Plague>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<SulphuricPoisoning>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },

				{ ModContent.NPCType<Calamitas>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.BoneJavelin,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Frostburn,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.SoulDrain,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<MarkedforDeath>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Plague>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<SulphuricPoisoning>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<CalamitasRun3>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.BoneJavelin,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Frostburn,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.SoulDrain,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<MarkedforDeath>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Plague>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<SulphuricPoisoning>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<CalamitasRun>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.BoneJavelin,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Frostburn,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.SoulDrain,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<MarkedforDeath>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Plague>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<SulphuricPoisoning>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<CalamitasRun2>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.BoneJavelin,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Frostburn,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.SoulDrain,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<MarkedforDeath>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Plague>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<SulphuricPoisoning>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<SoulSeeker>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.BoneJavelin,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Frostburn,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.SoulDrain,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<MarkedforDeath>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Plague>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<SulphuricPoisoning>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<LifeSeeker>(), new Tuple<bool, int[]>(false, new int[] { BuffID.OnFire }) },

				{ ModContent.NPCType<Leviathan>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.BoneJavelin,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Frostburn,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.SoulDrain,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Plague>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<SulphuricPoisoning>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<Siren>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.BoneJavelin,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Frostburn,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.SoulDrain,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<MarkedforDeath>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Plague>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<SulphuricPoisoning>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<SirenIce>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<AquaticAberration>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<Parasea>(), new Tuple<bool, int[]>(false, new int[] { }) },

				{ ModContent.NPCType<AstrumAureus>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.BoneJavelin,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Frostburn,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.Poisoned,
					BuffID.SoulDrain,
					BuffID.StardustMinionBleed,
					BuffID.Venom,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<MarkedforDeath>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Plague>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<SulphuricPoisoning>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<AureusSpawn>(), new Tuple<bool, int[]>(true, new int[] { }) },

				{ ModContent.NPCType<PlaguebringerGoliath>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.Frostburn,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<PlaguebringerShade>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.Frostburn,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<ExoFreeze>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<MarkedforDeath>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<SilvaStun>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<PlagueMine>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<PlagueHomingMissile>(), new Tuple<bool, int[]>(false, new int[] {
					BuffID.OnFire,
					BuffID.Poisoned,
					BuffID.ShadowFlame,
					BuffID.Venom,
					ModContent.BuffType<BrimstoneFlames>(),
					ModContent.BuffType<Plague>()
				}) },
				{ ModContent.NPCType<PlagueBeeG>(), new Tuple<bool, int[]>(false, new int[] {
					BuffID.OnFire,
					BuffID.Poisoned,
					BuffID.ShadowFlame,
					BuffID.Venom,
					ModContent.BuffType<BrimstoneFlames>(),
					ModContent.BuffType<Plague>()
				}) },
				{ ModContent.NPCType<PlagueBeeLargeG>(), new Tuple<bool, int[]>(false, new int[] {
					BuffID.OnFire,
					BuffID.Poisoned,
					BuffID.ShadowFlame,
					BuffID.Venom,
					ModContent.BuffType<BrimstoneFlames>(),
					ModContent.BuffType<Plague>()
				}) },

				{ ModContent.NPCType<RavagerBody>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Frostburn,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<RavagerClawLeft>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Frostburn,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<RavagerClawRight>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Frostburn,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<RavagerHead>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Frostburn,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<RavagerLegLeft>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Frostburn,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<RavagerLegRight>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Frostburn,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<RavagerHead2>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<RockPillar>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<FlamePillar>(), new Tuple<bool, int[]>(true, new int[] { }) },

				{ ModContent.NPCType<AstrumDeusHeadSpectral>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<AstrumDeusBodySpectral>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<AstrumDeusTailSpectral>(), new Tuple<bool, int[]>(true, new int[] { }) },

				{ ModContent.NPCType<ProfanedGuardianBoss>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<ProfanedGuardianBoss2>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<ProfanedGuardianBoss3>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },

				{ ModContent.NPCType<Bumblefuck>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<Bumblefuck2>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<ExoFreeze>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<SilvaStun>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },

				{ ModContent.NPCType<Providence>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Ichor,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<ProvSpawnOffense>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<ExoFreeze>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<SilvaStun>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<ProvSpawnDefense>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<ExoFreeze>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<SilvaStun>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<ProvSpawnHealer>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<ExoFreeze>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<SilvaStun>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },

				{ ModContent.NPCType<CeaselessVoid>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<DarkEnergy>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<DarkEnergy2>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<DarkEnergy3>(), new Tuple<bool, int[]>(true, new int[] { }) },

				{ ModContent.NPCType<StormWeaverHead>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<StormWeaverBody>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<StormWeaverTail>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<StormWeaverHeadNaked>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<StormWeaverBodyNaked>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<StormWeaverTailNaked>(), new Tuple<bool, int[]>(true, new int[] { }) },

				{ ModContent.NPCType<Signus>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<CosmicLantern>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<SignusBomb>(), new Tuple<bool, int[]>(false, new int[] { }) },

				{ ModContent.NPCType<Polterghast>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Shred>()
				}) },
				{ ModContent.NPCType<PolterPhantom>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<PhantomFuckYou>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<PolterghastHook>(), new Tuple<bool, int[]>(true, new int[] { }) },

				{ ModContent.NPCType<OldDuke>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<OldDukeToothBall>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<OldDukeSharkron>(), new Tuple<bool, int[]>(true, new int[] { }) },

				{ ModContent.NPCType<DevourerofGodsHead>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<DevourerofGodsBody>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<DevourerofGodsTail>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<DevourerofGodsHeadS>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<DevourerofGodsBodyS>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<DevourerofGodsTailS>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<DevourerofGodsHead2>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<DevourerofGodsBody2>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<DevourerofGodsTail2>(), new Tuple<bool, int[]>(true, new int[] { }) },

				{ ModContent.NPCType<Yharon>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.CursedInferno,
					BuffID.Ichor,
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<Shred>()
				}) },
				{ ModContent.NPCType<DetonatingFlare>(), new Tuple<bool, int[]>(false, new int[] { }) },
				{ ModContent.NPCType<DetonatingFlare2>(), new Tuple<bool, int[]>(false, new int[] { }) },

				{ ModContent.NPCType<SupremeCalamitas>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.CursedInferno,
					BuffID.Ichor
				}) },
				{ ModContent.NPCType<SupremeCatastrophe>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.CursedInferno,
					BuffID.Ichor
				}) },
				{ ModContent.NPCType<SupremeCataclysm>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.CursedInferno,
					BuffID.Ichor
				}) },
				{ ModContent.NPCType<SoulSeekerSupreme>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.CursedInferno,
					BuffID.Ichor
				}) },
				{ ModContent.NPCType<SCalWormHeart>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<SCalWormHead>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<SCalWormBody>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<SCalWormBodyWeak>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<SCalWormTail>(), new Tuple<bool, int[]>(true, new int[] { }) },

				// The struct is called BossStats but we have all the enemies... just something to consider...
				{ ModContent.NPCType<Bloatfish>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<CrushDepth>() }) },
				{ ModContent.NPCType<BobbitWormHead>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<CrushDepth>() }) },
				{ ModContent.NPCType<BoxJellyfish>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<CrushDepth>() }) },
				{ ModContent.NPCType<ChaoticPuffer>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<CrushDepth>() }) },
				{ ModContent.NPCType<Cuttlefish>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<CrushDepth>() }) },
				{ ModContent.NPCType<DevilFish>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<CrushDepth>() }) },
				{ ModContent.NPCType<DevilFishAlt>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<CrushDepth>() }) },
				{ ModContent.NPCType<GiantSquid>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<CrushDepth>() }) },
				{ ModContent.NPCType<GulperEelHead>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<GulperEelBody>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<GulperEelBodyAlt>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<GulperEelTail>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<Laserfish>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<CrushDepth>() }) },
				{ ModContent.NPCType<LuminousCorvina>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<CrushDepth>() }) },
				{ ModContent.NPCType<MirageJelly>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<CrushDepth>() }) },
				{ ModContent.NPCType<MorayEel>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<CrushDepth>() }) },
				{ ModContent.NPCType<OarfishHead>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<OarfishBody>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<OarfishTail>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<ToxicMinnow>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<CrushDepth>() }) },
				{ ModContent.NPCType<Viperfish>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<CrushDepth>() }) },

				{ ModContent.NPCType<EidolonWyrmHead>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<EidolonWyrmBody>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<EidolonWyrmBodyAlt>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<EidolonWyrmTail>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<EidolonWyrmHeadHuge>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<EidolonWyrmBodyHuge>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<EidolonWyrmBodyAltHuge>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<EidolonWyrmTailHuge>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<ColossalSquid>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Frostburn,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Plague>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<Reaper>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Frostburn,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Plague>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },

				{ ModContent.NPCType<AcidEel>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<BloodwormFleeing>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<BloodwormNormal>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<CragmawMire>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<FlakBaby>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<FlakCrab>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<GammaSlime>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<IrradiatedSlime>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<NuclearTerror>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<NuclearToad>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<Orthocera>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<Radiator>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<Skyfin>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<SulfurousSkater>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<Trilobite>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<WaterLeech>(), new Tuple<bool, int[]>(true, new int[] { }) },

				{ ModContent.NPCType<AquaticParasite>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<AquaticSeekerHead>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<AquaticSeekerBody>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<AquaticSeekerTail>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<AquaticUrchin>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<AnthozoanCrab>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<BelchingCoral>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<Catfish>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<Flounder>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<Gnasher>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<Mauler>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<MicrobialCluster>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<Trasher>(), new Tuple<bool, int[]>(true, new int[] { }) },

				{ ModContent.NPCType<Aries>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<AstralInfectionDebuff>() }) },
				{ ModContent.NPCType<AstralachneaGround>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<AstralInfectionDebuff>(), BuffID.Poisoned }) },
				{ ModContent.NPCType<AstralachneaWall>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<AstralInfectionDebuff>(), BuffID.Poisoned }) },
				{ ModContent.NPCType<AstralProbe>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<AstralInfectionDebuff>() }) },
				{ ModContent.NPCType<AstralSlime>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<AstralInfectionDebuff>() }) },
				{ ModContent.NPCType<Atlas>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<AstralInfectionDebuff>() }) },
				{ ModContent.NPCType<BigSightseer>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<AstralInfectionDebuff>() }) },
				{ ModContent.NPCType<FusionFeeder>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<AstralInfectionDebuff>() }) },
				{ ModContent.NPCType<Hadarian>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<AstralInfectionDebuff>() }) },
				{ ModContent.NPCType<Hive>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<AstralInfectionDebuff>() }) },
				{ ModContent.NPCType<Hiveling>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<AstralInfectionDebuff>() }) },
				{ ModContent.NPCType<Mantis>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<AstralInfectionDebuff>() }) },
				{ ModContent.NPCType<Nova>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<AstralInfectionDebuff>() }) },
				{ ModContent.NPCType<SmallSightseer>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<AstralInfectionDebuff>() }) },
				{ ModContent.NPCType<StellarCulex>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<AstralInfectionDebuff>() }) },

				{ ModContent.NPCType<GreatSandShark>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Frostburn,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<ExoFreeze>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<MarkedforDeath>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Plague>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<SilvaStun>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },

				{ ModContent.NPCType<SeaSerpent1>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<SeaSerpent2>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<SeaSerpent3>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<SeaSerpent4>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<SeaSerpent5>(), new Tuple<bool, int[]>(true, new int[] { }) },

				{ ModContent.NPCType<ScornEater>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<ExoFreeze>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<SilvaStun>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<ImpiousImmolator>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<ExoFreeze>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<SilvaStun>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<Horse>(), new Tuple<bool, int[]>(true, new int[] {
					BuffID.BetsysCurse,
					BuffID.CursedInferno,
					BuffID.Daybreak,
					BuffID.DryadsWardDebuff,
					BuffID.Frostburn,
					BuffID.Ichor,
					BuffID.Oiled,
					BuffID.StardustMinionBleed,
					ModContent.BuffType<AbyssalFlames>(),
					ModContent.BuffType<ArmorCrunch>(),
					ModContent.BuffType<AstralInfectionDebuff>(),
					ModContent.BuffType<DemonFlames>(),
					ModContent.BuffType<ExoFreeze>(),
					ModContent.BuffType<GodSlayerInferno>(),
					ModContent.BuffType<HolyFlames>(),
					ModContent.BuffType<MarkedforDeath>(),
					ModContent.BuffType<Nightwither>(),
					ModContent.BuffType<Plague>(),
					ModContent.BuffType<SilvaStun>(),
					ModContent.BuffType<Shred>(),
					ModContent.BuffType<WarCleave>(),
					ModContent.BuffType<WhisperingDeath>()
				}) },
				{ ModContent.NPCType<PlaguedTortoise>(), new Tuple<bool, int[]>(false, new int[] {
					BuffID.OnFire,
					BuffID.Poisoned,
					BuffID.ShadowFlame,
					BuffID.Venom,
					ModContent.BuffType<BrimstoneFlames>(),
					ModContent.BuffType<Plague>()
				}) },
				{ ModContent.NPCType<PlaguedDerpling>(), new Tuple<bool, int[]>(false, new int[] {
					BuffID.OnFire,
					BuffID.Poisoned,
					BuffID.ShadowFlame,
					BuffID.Venom,
					ModContent.BuffType<BrimstoneFlames>(),
					ModContent.BuffType<Plague>()
				}) },
				{ ModContent.NPCType<PlaguedFlyingFox>(), new Tuple<bool, int[]>(false, new int[] {
					BuffID.OnFire,
					BuffID.Poisoned,
					BuffID.ShadowFlame,
					BuffID.Venom,
					ModContent.BuffType<BrimstoneFlames>(),
					ModContent.BuffType<Plague>()
				}) },
				{ ModContent.NPCType<PlaguedJungleSlime>(), new Tuple<bool, int[]>(false, new int[] {
					BuffID.OnFire,
					BuffID.Poisoned,
					BuffID.ShadowFlame,
					BuffID.Venom,
					ModContent.BuffType<BrimstoneFlames>(),
					ModContent.BuffType<Plague>()
				}) },
				{ ModContent.NPCType<PlagueBeeLarge>(), new Tuple<bool, int[]>(false, new int[] {
					BuffID.OnFire,
					BuffID.Poisoned,
					BuffID.ShadowFlame,
					BuffID.Venom,
					ModContent.BuffType<BrimstoneFlames>(),
					ModContent.BuffType<Plague>()
				}) },
				{ ModContent.NPCType<PlagueBee>(), new Tuple<bool, int[]>(false, new int[] {
					BuffID.OnFire,
					BuffID.Poisoned,
					BuffID.ShadowFlame,
					BuffID.Venom,
					ModContent.BuffType<BrimstoneFlames>(),
					ModContent.BuffType<Plague>()
				}) },
				{ ModContent.NPCType<ArmoredDiggerHead>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<ArmoredDiggerBody>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<ArmoredDiggerTail>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<Eidolist>(), new Tuple<bool, int[]>(true, new int[] { }) },
				{ ModContent.NPCType<ThiccWaifu>(), new Tuple<bool, int[]>(false, new int[] { BuffID.Poisoned }) },
				{ ModContent.NPCType<CrimulanBlightSlime>(), new Tuple<bool, int[]>(false, new int[] { BuffID.OnFire }) },
				{ ModContent.NPCType<EbonianBlightSlime>(), new Tuple<bool, int[]>(false, new int[] { BuffID.OnFire }) },
				{ ModContent.NPCType<Frogfish>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<CrushDepth>() }) },
				{ ModContent.NPCType<MantisShrimp>(), new Tuple<bool, int[]>(false, new int[] { BuffID.Daybreak }) }
			};
		}

		// Destroys the BossStats struct to save memory because mod assemblies will not be fully unloaded until TML 1.4.
		internal static void UnloadDebuffs()
		{
			BossStats.DebuffImmunities = null;
		}
		#endregion
	}
}
