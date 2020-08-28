using CalamityMod.NPCs;
using CalamityMod.NPCs.AquaticScourge;
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
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Projectiles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod
{
	public static class NPCStats
	{
		// I want to die
		public static void GetNPCDamage(this NPC npc, bool expert, bool revenge, bool death, bool? master = null)
		{
			/*if (master)
			{
				BossStats.MasterContactDamage.TryGetValue(npc.type, out int contactDamage);
				npc.damage = contactDamage;
			}*/
			if (death)
			{
				BossStats.DeathContactDamage.TryGetValue(npc.type, out int contactDamage);
				npc.damage = contactDamage;
			}
			else if (revenge)
			{
				BossStats.RevengeanceContactDamage.TryGetValue(npc.type, out int contactDamage);
				npc.damage = contactDamage;
			}
			else if (expert)
			{
				BossStats.ExpertContactDamage.TryGetValue(npc.type, out int contactDamage);
				npc.damage = contactDamage;
			}
			else
			{
				BossStats.NormalContactDamage.TryGetValue(npc.type, out int contactDamage);
				npc.damage = contactDamage;
			}
		}

		public struct BossStats
		{
			public static SortedDictionary<int, int> NormalContactDamage = new SortedDictionary<int, int>
			{
				{ NPCID.KingSlime, 40 },

				{ ModContent.NPCType<DesertScourgeHead>(), 30 },
				{ ModContent.NPCType<DesertScourgeBody>(), 16 },
				{ ModContent.NPCType<DesertScourgeTail>(), 12 },
				{ ModContent.NPCType<DriedSeekerHead>(), 14 },
				{ ModContent.NPCType<DriedSeekerBody>(), 8 },
				{ ModContent.NPCType<DriedSeekerTail>(), 6 },

				{ NPCID.EyeofCthulhu, 15 }, // 23 in phase 2
				{ NPCID.ServantofCthulhu, 12 },

				{ ModContent.NPCType<CrabulonIdle>(), 40 },
				{ ModContent.NPCType<CrabShroom>(), 25 },

				{ NPCID.EaterofWorldsHead, 22 },
				{ NPCID.EaterofWorldsBody, 13 },
				{ NPCID.EaterofWorldsTail, 11 },

				{ NPCID.BrainofCthulhu, 30 },
				{ NPCID.Creeper, 20 },

				{ ModContent.NPCType<HiveMind>(), 20 },
				{ ModContent.NPCType<HiveMindP2>(), 35 },
				{ ModContent.NPCType<DankCreeper>(), 25 },

				{ ModContent.NPCType<PerforatorHive>(), 30 },
				{ ModContent.NPCType<PerforatorHeadLarge>(), 45 },
				{ ModContent.NPCType<PerforatorBodyLarge>(), 24 },
				{ ModContent.NPCType<PerforatorTailLarge>(), 18 },
				{ ModContent.NPCType<PerforatorHeadMedium>(), 35 },
				{ ModContent.NPCType<PerforatorBodyMedium>(), 21 },
				{ ModContent.NPCType<PerforatorTailMedium>(), 14 },
				{ ModContent.NPCType<PerforatorHeadSmall>(), 30 },
				{ ModContent.NPCType<PerforatorBodySmall>(), 18 },
				{ ModContent.NPCType<PerforatorTailSmall>(), 10 },

				{ NPCID.QueenBee, 30 },
				{ NPCID.Bee, 20 },
				{ NPCID.BeeSmall, 15 },

				{ NPCID.SkeletronHead, 32 },
				{ NPCID.SkeletronHand, 20 },

				{ ModContent.NPCType<SlimeGodCore>(), 40 },
				{ ModContent.NPCType<SlimeGod>(), 45 },
				{ ModContent.NPCType<SlimeGodSplit>(), 40 },
				{ ModContent.NPCType<SlimeGodRun>(), 50 },
				{ ModContent.NPCType<SlimeGodRunSplit>(), 45 },
				{ ModContent.NPCType<SlimeSpawnCorrupt>(), 40 },
				{ ModContent.NPCType<SlimeSpawnCorrupt2>(), 30 },
				{ ModContent.NPCType<SlimeSpawnCrimson>(), 45 },
				{ ModContent.NPCType<SlimeSpawnCrimson2>(), 35 },

				{ NPCID.WallofFlesh, 50 },
				{ NPCID.TheHungry, 30 }, // Ranges from 30 to 75 depending on WoF life
				{ NPCID.TheHungryII, 30 },
				{ NPCID.LeechHead, 26 },
				{ NPCID.LeechBody, 22 },
				{ NPCID.LeechTail, 18 },

				{ ModContent.NPCType<Cryogen>(), 50 },
				{ ModContent.NPCType<CryogenIce>(), 50 },
				{ ModContent.NPCType<Cryocore>(), 35 },
				{ ModContent.NPCType<Cryocore2>(), 40 },
				{ ModContent.NPCType<IceMass>(), 40 },

				{ NPCID.Spazmatism, 50 }, // 75 in phase 2
				{ NPCID.Retinazer, 45 }, // 67 in phase 2

				{ ModContent.NPCType<AquaticScourgeHead>(), 80 },
				{ ModContent.NPCType<AquaticScourgeBody>(), 65 },
				{ ModContent.NPCType<AquaticScourgeBodyAlt>(), 55 },
				{ ModContent.NPCType<AquaticScourgeTail>(), 45 },

				{ NPCID.TheDestroyer, 70 },
				{ NPCID.TheDestroyerBody, 55 },
				{ NPCID.TheDestroyerTail, 40 },

				{ ModContent.NPCType<BrimstoneElemental>(), 60 },

				{ NPCID.SkeletronPrime, 47 }, // 94 while spinning
				{ NPCID.PrimeVice, 52 },
				{ NPCID.PrimeSaw, 56 },
				{ NPCID.PrimeCannon, 30 },
				{ NPCID.PrimeLaser, 29 },

				{ ModContent.NPCType<Calamitas>(), 55 },
				{ ModContent.NPCType<CalamitasRun3>(), 70 },
				{ ModContent.NPCType<CalamitasRun>(), 60 },
				{ ModContent.NPCType<CalamitasRun2>(), 65 },

				{ NPCID.Plantera, 50 }, // 70 in phase 2
				{ NPCID.PlanterasHook, 60 },
				{ NPCID.PlanterasTentacle, 60 },
				{ NPCID.Spore, 70 },

				{ ModContent.NPCType<Leviathan>(), 90 },
				{ ModContent.NPCType<Siren>(), 70 },
				{ ModContent.NPCType<SirenIce>(), 55 },
				{ NPCID.DetonatingBubble, 100 },
				{ ModContent.NPCType<AquaticAberration>(), 55 },
				{ ModContent.NPCType<Parasea>(), 50 },

				{ ModContent.NPCType<AstrumAureus>(), 80 },

				{ NPCID.Golem, 72 },
				{ NPCID.GolemHead, 64 },
				{ NPCID.GolemHeadFree, 80 },
				{ NPCID.GolemFistLeft, 59 },
				{ NPCID.GolemFistRight, 59 },

				{ ModContent.NPCType<PlaguebringerGoliath>(), 100 },
				{ ModContent.NPCType<PlaguebringerShade>(), 70 },
				{ ModContent.NPCType<PlagueHomingMissile>(), 90 },
				{ ModContent.NPCType<PlagueBeeG>(), 60 },
				{ ModContent.NPCType<PlagueBeeLargeG>(), 65 },

				{ NPCID.DukeFishron, 100 }, // 120 in phase 2
				{ NPCID.Sharkron, 100 },
				{ NPCID.Sharkron2, 120 },

				{ ModContent.NPCType<RavagerBody>(), 120 },
				{ ModContent.NPCType<RavagerClawLeft>(), 80 },
				{ ModContent.NPCType<RavagerClawRight>(), 80 },
				{ ModContent.NPCType<RockPillar>(), 120 },
				{ ModContent.NPCType<FlamePillar>(), 100 },

				{ NPCID.CultistDragonHead, 80 },
				{ NPCID.CultistDragonBody1, 40 },
				{ NPCID.CultistDragonBody2, 40 },
				{ NPCID.CultistDragonBody3, 40 },
				{ NPCID.CultistDragonBody4, 40 },
				{ NPCID.CultistDragonTail, 40 },
				{ NPCID.AncientCultistSquidhead, 90 },
				{ NPCID.AncientDoom, 30 },
				{ NPCID.AncientLight, 120 },

				{ ModContent.NPCType<AstrumDeusHeadSpectral>(), 120 },
				{ ModContent.NPCType<AstrumDeusBodySpectral>(), 100 },
				{ ModContent.NPCType<AstrumDeusTailSpectral>(), 80 },

				{ NPCID.MoonLordHand, 80 },
			};

			public static SortedDictionary<int, int> ExpertContactDamage = new SortedDictionary<int, int>
			{
				{ NPCID.KingSlime, 67 },

				{ ModContent.NPCType<DesertScourgeHead>(), 66 },
				{ ModContent.NPCType<DesertScourgeBody>(), 27 },
				{ ModContent.NPCType<DesertScourgeTail>(), 20 },
				{ ModContent.NPCType<DriedSeekerHead>(), 28 },
				{ ModContent.NPCType<DriedSeekerBody>(), 16 },
				{ ModContent.NPCType<DriedSeekerTail>(), 12 },

				{ NPCID.EyeofCthulhu, 30 }, // 36 in phase 2, 40 in phase 3
				{ NPCID.ServantofCthulhu, 24 },

				{ ModContent.NPCType<CrabulonIdle>(), 64 },
				{ ModContent.NPCType<CrabShroom>(), 50 },

				{ NPCID.EaterofWorldsHead, 48 },
				{ NPCID.EaterofWorldsBody, 20 },
				{ NPCID.EaterofWorldsTail, 17 },
				{ NPCID.VileSpit, 64 },

				{ NPCID.BrainofCthulhu, 54 },
				{ NPCID.Creeper, 40 },

				{ ModContent.NPCType<HiveMind>(), 32 },
				{ ModContent.NPCType<HiveMindP2>(), 63 },
				{ ModContent.NPCType<DankCreeper>(), 50 },

				{ ModContent.NPCType<PerforatorHive>(), 48 },
				{ ModContent.NPCType<PerforatorHeadLarge>(), 90 },
				{ ModContent.NPCType<PerforatorBodyLarge>(), 38 },
				{ ModContent.NPCType<PerforatorTailLarge>(), 28 },
				{ ModContent.NPCType<PerforatorHeadMedium>(), 70 },
				{ ModContent.NPCType<PerforatorBodyMedium>(), 33 },
				{ ModContent.NPCType<PerforatorTailMedium>(), 22 },
				{ ModContent.NPCType<PerforatorHeadSmall>(), 60 },
				{ ModContent.NPCType<PerforatorBodySmall>(), 28 },
				{ ModContent.NPCType<PerforatorTailSmall>(), 16 },

				{ NPCID.QueenBee, 54 },
				{ NPCID.Bee, 24 },
				{ NPCID.BeeSmall, 18 },

				{ NPCID.SkeletronHead, 70 }, // 91 while spinning
				{ NPCID.SkeletronHand, 44 },

				{ ModContent.NPCType<SlimeGodCore>(), 80 },
				{ ModContent.NPCType<SlimeGod>(), 76 },
				{ ModContent.NPCType<SlimeGodSplit>(), 68 },
				{ ModContent.NPCType<SlimeGodRun>(), 85 },
				{ ModContent.NPCType<SlimeGodRunSplit>(), 76 },
				{ ModContent.NPCType<SlimeSpawnCorrupt>(), 80 },
				{ ModContent.NPCType<SlimeSpawnCorrupt2>(), 60 },
				{ ModContent.NPCType<SlimeSpawnCrimson>(), 90 },
				{ ModContent.NPCType<SlimeSpawnCrimson2>(), 70 },

				{ NPCID.WallofFlesh, 150 },
				{ NPCID.TheHungry, 60 }, // Ranges from 60 to 150 depending on WoF life
				{ NPCID.TheHungryII, 60 },
				{ NPCID.LeechHead, 52 },
				{ NPCID.LeechBody, 44 },
				{ NPCID.LeechTail, 36 },

				{ ModContent.NPCType<Cryogen>(), 90 },
				{ ModContent.NPCType<CryogenIce>(), 100 },
				{ ModContent.NPCType<Cryocore>(), 70 },
				{ ModContent.NPCType<Cryocore2>(), 80 },
				{ ModContent.NPCType<IceMass>(), 80 },

				{ NPCID.Spazmatism, 85 }, // 127 in phase 2
				{ NPCID.Retinazer, 76 }, // 114 in phase 2

				{ ModContent.NPCType<AquaticScourgeHead>(), 160 },
				{ ModContent.NPCType<AquaticScourgeBody>(), 110 },
				{ ModContent.NPCType<AquaticScourgeBodyAlt>(), 93 },
				{ ModContent.NPCType<AquaticScourgeTail>(), 76 },

				{ NPCID.TheDestroyer, 280 },
				{ NPCID.TheDestroyerBody, 93 },
				{ NPCID.TheDestroyerTail, 68 },

				{ ModContent.NPCType<BrimstoneElemental>(), 96 },

				{ NPCID.SkeletronPrime, 79 }, // 158 while spinning
				{ NPCID.PrimeVice, 88 },
				{ NPCID.PrimeSaw, 95 },
				{ NPCID.PrimeCannon, 51 },
				{ NPCID.PrimeLaser, 49 },

				{ ModContent.NPCType<Calamitas>(), 88 },
				{ ModContent.NPCType<CalamitasRun3>(), 112 },
				{ ModContent.NPCType<CalamitasRun>(), 120 },
				{ ModContent.NPCType<CalamitasRun2>(), 130 },

				{ NPCID.Plantera, 100 }, // 140 in phase 2
				{ NPCID.PlanterasHook, 120 },
				{ NPCID.PlanterasTentacle, 120 },
				{ NPCID.Spore, 126 },

				{ ModContent.NPCType<Leviathan>(), 153 },
				{ ModContent.NPCType<Siren>(), 119 },
				{ ModContent.NPCType<SirenIce>(), 110 },
				{ NPCID.DetonatingBubble, 140 },
				{ ModContent.NPCType<AquaticAberration>(), 110 },
				{ ModContent.NPCType<Parasea>(), 100 },

				{ ModContent.NPCType<AstrumAureus>(), 136 },

				{ NPCID.Golem, 115 },
				{ NPCID.GolemHead, 102 },
				{ NPCID.GolemHeadFree, 128 },
				{ NPCID.GolemFistLeft, 94 },
				{ NPCID.GolemFistRight, 94 },

				{ ModContent.NPCType<PlaguebringerGoliath>(), 180 },
				{ ModContent.NPCType<PlaguebringerShade>(), 140 },
				{ ModContent.NPCType<PlagueHomingMissile>(), 180 },
				{ ModContent.NPCType<PlagueMine>(), 200 },
				{ ModContent.NPCType<PlagueBeeG>(), 120 },
				{ ModContent.NPCType<PlagueBeeLargeG>(), 130 },

				{ NPCID.DukeFishron, 140 }, // 201 in phase 2, 184 in phase 3
				{ NPCID.Sharkron, 150 },
				{ NPCID.Sharkron2, 180 },

				{ ModContent.NPCType<RavagerBody>(), 192 },
				{ ModContent.NPCType<RavagerClawLeft>(), 160 },
				{ ModContent.NPCType<RavagerClawRight>(), 160 },
				{ ModContent.NPCType<RockPillar>(), 200 },
				{ ModContent.NPCType<FlamePillar>(), 180 },

				{ NPCID.CultistDragonHead, 120 },
				{ NPCID.CultistDragonBody1, 60 },
				{ NPCID.CultistDragonBody2, 60 },
				{ NPCID.CultistDragonBody3, 60 },
				{ NPCID.CultistDragonBody4, 60 },
				{ NPCID.CultistDragonTail, 60 },
				{ NPCID.AncientCultistSquidhead, 180 },
				{ NPCID.AncientDoom, 45 },
				{ NPCID.AncientLight, 180 },

				{ ModContent.NPCType<AstrumDeusHeadSpectral>(), 240 },
				{ ModContent.NPCType<AstrumDeusBodySpectral>(), 170 },
				{ ModContent.NPCType<AstrumDeusTailSpectral>(), 136 },

				{ NPCID.MoonLordHand, 80 },
			};

			public static SortedDictionary<int, int> RevengeanceContactDamage = new SortedDictionary<int, int>
			{
				{ NPCID.KingSlime, 80 },

				{ ModContent.NPCType<DesertScourgeHead>(), 79 },
				{ ModContent.NPCType<DesertScourgeBody>(), 33 },
				{ ModContent.NPCType<DesertScourgeTail>(), 25 },
				{ ModContent.NPCType<DesertScourgeHeadSmall>(), 45 },
				{ ModContent.NPCType<DesertScourgeBodySmall>(), 25 },
				{ ModContent.NPCType<DesertScourgeTailSmall>(), 20 },
				{ ModContent.NPCType<DriedSeekerHead>(), 35 },
				{ ModContent.NPCType<DriedSeekerBody>(), 20 },
				{ ModContent.NPCType<DriedSeekerTail>(), 15 },

				{ NPCID.EyeofCthulhu, 37 }, // 45 in phase 2, 52 in phase 3
				{ NPCID.ServantofCthulhu, 30 },

				{ ModContent.NPCType<CrabulonIdle>(), 76 },
				{ ModContent.NPCType<CrabShroom>(), 62 },

				{ NPCID.EaterofWorldsHead, 70 },
				{ NPCID.EaterofWorldsBody, 25 },
				{ NPCID.EaterofWorldsTail, 21 },
				{ NPCID.VileSpit, 64 },

				{ NPCID.BrainofCthulhu, 76 },
				{ NPCID.Creeper, 50 },

				{ ModContent.NPCType<HiveMind>(), 40 },
				{ ModContent.NPCType<HiveMindP2>(), 75 },
				{ ModContent.NPCType<DankCreeper>(), 62 },

				{ ModContent.NPCType<PerforatorHive>(), 60 },
				{ ModContent.NPCType<PerforatorHeadLarge>(), 108 },
				{ ModContent.NPCType<PerforatorBodyLarge>(), 44 },
				{ ModContent.NPCType<PerforatorTailLarge>(), 32 },
				{ ModContent.NPCType<PerforatorHeadMedium>(), 84 },
				{ ModContent.NPCType<PerforatorBodyMedium>(), 38 },
				{ ModContent.NPCType<PerforatorTailMedium>(), 26 },
				{ ModContent.NPCType<PerforatorHeadSmall>(), 72 },
				{ ModContent.NPCType<PerforatorBodySmall>(), 32 },
				{ ModContent.NPCType<PerforatorTailSmall>(), 20 },

				{ NPCID.QueenBee, 74 },
				{ NPCID.Bee, 30 },
				{ NPCID.BeeSmall, 22 },

				{ NPCID.SkeletronHead, 84 }, // 109 while spinning
				{ NPCID.SkeletronHand, 55 },

				{ ModContent.NPCType<SlimeGodCore>(), 96 },
				{ ModContent.NPCType<SlimeGod>(), 91 },
				{ ModContent.NPCType<SlimeGodSplit>(), 81 },
				{ ModContent.NPCType<SlimeGodRun>(), 102 },
				{ ModContent.NPCType<SlimeGodRunSplit>(), 91 },
				{ ModContent.NPCType<SlimeSpawnCorrupt>(), 96 },
				{ ModContent.NPCType<SlimeSpawnCorrupt2>(), 72 },
				{ ModContent.NPCType<SlimeSpawnCrimson>(), 108 },
				{ ModContent.NPCType<SlimeSpawnCrimson2>(), 84 },

				{ NPCID.WallofFlesh, 172 },
				{ NPCID.TheHungry, 60 }, // Ranges from 60 to 150 depending on WoF life
				{ NPCID.TheHungryII, 72 },
				{ NPCID.LeechHead, 62 },
				{ NPCID.LeechBody, 52 },
				{ NPCID.LeechTail, 42 },

				{ ModContent.NPCType<Cryogen>(), 103 },
				{ ModContent.NPCType<CryogenIce>(), 115 },
				{ ModContent.NPCType<Cryocore>(), 84 },
				{ ModContent.NPCType<Cryocore2>(), 96 },
				{ ModContent.NPCType<IceMass>(), 96 },

				{ NPCID.Spazmatism, 97 }, // 146 in phase 2
				{ NPCID.Retinazer, 87 }, // 131 in phase 2

				{ ModContent.NPCType<AquaticScourgeHead>(), 184 },
				{ ModContent.NPCType<AquaticScourgeBody>(), 126 },
				{ ModContent.NPCType<AquaticScourgeBodyAlt>(), 106 },
				{ ModContent.NPCType<AquaticScourgeTail>(), 91 },

				{ NPCID.TheDestroyer, 308 },
				{ NPCID.TheDestroyerBody, 111 },
				{ NPCID.TheDestroyerTail, 81 },
				{ NPCID.Probe, 0 },

				{ ModContent.NPCType<BrimstoneElemental>(), 115 },

				{ NPCID.SkeletronPrime, 90 }, // 181 while spinning
				{ NPCID.PrimeVice, 105 },
				{ NPCID.PrimeSaw, 114 },
				{ NPCID.PrimeCannon, 61 },
				{ NPCID.PrimeLaser, 58 },

				{ ModContent.NPCType<Calamitas>(), 101 },
				{ ModContent.NPCType<CalamitasRun3>(), 128 },
				{ ModContent.NPCType<CalamitasRun>(), 138 },
				{ ModContent.NPCType<CalamitasRun2>(), 149 },

				{ NPCID.Plantera, 132 }, // 185 in phase 2
				{ NPCID.PlanterasTentacle, 138 },
				{ NPCID.Spore, 144 },

				{ ModContent.NPCType<Leviathan>(), 175 },
				{ ModContent.NPCType<Siren>(), 136 },
				{ ModContent.NPCType<SirenIce>(), 126 },
				{ NPCID.DetonatingBubble, 161 },
				{ ModContent.NPCType<AquaticAberration>(), 126 },
				{ ModContent.NPCType<Parasea>(), 115 },

				{ ModContent.NPCType<AstrumAureus>(), 156 },
				{ ModContent.NPCType<AureusSpawn>(), 150 },

				{ NPCID.Golem, 165 },
				{ NPCID.GolemHead, 117 },
				{ NPCID.GolemFistLeft, 108 },
				{ NPCID.GolemFistRight, 108 },

				{ ModContent.NPCType<PlaguebringerGoliath>(), 207 },
				{ ModContent.NPCType<PlaguebringerShade>(), 161 },
				{ ModContent.NPCType<PlagueHomingMissile>(), 207 },
				{ ModContent.NPCType<PlagueMine>(), 225 },
				{ ModContent.NPCType<PlagueBeeG>(), 138 },
				{ ModContent.NPCType<PlagueBeeLargeG>(), 149 },

				{ NPCID.DukeFishron, 161 }, // 231 in phase 2, 211 in phase 3
				{ NPCID.Sharkron, 172 },
				{ NPCID.Sharkron2, 207 },

				{ ModContent.NPCType<RavagerBody>(), 220 },
				{ ModContent.NPCType<RavagerClawLeft>(), 184 },
				{ ModContent.NPCType<RavagerClawRight>(), 184 },
				{ ModContent.NPCType<RockPillar>(), 200 },
				{ ModContent.NPCType<FlamePillar>(), 180 },

				{ NPCID.CultistDragonHead, 138 },
				{ NPCID.CultistDragonBody1, 69 },
				{ NPCID.CultistDragonBody2, 69 },
				{ NPCID.CultistDragonBody3, 69 },
				{ NPCID.CultistDragonBody4, 69 },
				{ NPCID.CultistDragonTail, 69 },
				{ NPCID.AncientCultistSquidhead, 207 },
				{ NPCID.AncientLight, 207 },

				{ ModContent.NPCType<AstrumDeusHeadSpectral>(), 264 },
				{ ModContent.NPCType<AstrumDeusBodySpectral>(), 182 },
				{ ModContent.NPCType<AstrumDeusTailSpectral>(), 146 },
			};

			public static SortedDictionary<int, int> DeathContactDamage = new SortedDictionary<int, int>
			{
				{ NPCID.KingSlime, 88 },

				{ ModContent.NPCType<DesertScourgeHead>(), 87 },
				{ ModContent.NPCType<DesertScourgeBody>(), 37 },
				{ ModContent.NPCType<DesertScourgeTail>(), 28 },
				{ ModContent.NPCType<DesertScourgeHeadSmall>(), 51 },
				{ ModContent.NPCType<DesertScourgeBodySmall>(), 28 },
				{ ModContent.NPCType<DesertScourgeTailSmall>(), 23 },
				{ ModContent.NPCType<DriedSeekerHead>(), 40 },
				{ ModContent.NPCType<DriedSeekerBody>(), 23 },
				{ ModContent.NPCType<DriedSeekerTail>(), 17 },

				{ NPCID.EyeofCthulhu, 58 },
				{ NPCID.ServantofCthulhu, 33 },

				{ ModContent.NPCType<CrabulonIdle>(), 84 },
				{ ModContent.NPCType<CrabShroom>(), 70 },

				{ NPCID.EaterofWorldsHead, 79 },
				{ NPCID.EaterofWorldsBody, 25 },
				{ NPCID.EaterofWorldsTail, 23 },
				{ NPCID.VileSpit, 64 },

				{ NPCID.BrainofCthulhu, 84 },
				{ NPCID.Creeper, 56 },

				{ ModContent.NPCType<HiveMind>(), 44 },
				{ ModContent.NPCType<HiveMindP2>(), 83 },
				{ ModContent.NPCType<DankCreeper>(), 70 },

				{ ModContent.NPCType<PerforatorHive>(), 67 },
				{ ModContent.NPCType<PerforatorHeadLarge>(), 118 },
				{ ModContent.NPCType<PerforatorBodyLarge>(), 48 },
				{ ModContent.NPCType<PerforatorTailLarge>(), 36 },
				{ ModContent.NPCType<PerforatorHeadMedium>(), 92 },
				{ ModContent.NPCType<PerforatorBodyMedium>(), 41 },
				{ ModContent.NPCType<PerforatorTailMedium>(), 29 },
				{ ModContent.NPCType<PerforatorHeadSmall>(), 79 },
				{ ModContent.NPCType<PerforatorBodySmall>(), 35 },
				{ ModContent.NPCType<PerforatorTailSmall>(), 23 },

				{ NPCID.QueenBee, 97 },
				{ NPCID.Bee, 33 },
				{ NPCID.BeeSmall, 25 },

				{ NPCID.SkeletronHead, 92 }, // 120 while spinning
				{ NPCID.SkeletronHand, 61 },

				{ ModContent.NPCType<SlimeGodCore>(), 105 },
				{ ModContent.NPCType<SlimeGod>(), 100 },
				{ ModContent.NPCType<SlimeGodSplit>(), 89 },
				{ ModContent.NPCType<SlimeGodRun>(), 112 },
				{ ModContent.NPCType<SlimeGodRunSplit>(), 100 },
				{ ModContent.NPCType<SlimeSpawnCorrupt>(), 105 },
				{ ModContent.NPCType<SlimeSpawnCorrupt2>(), 84 },
				{ ModContent.NPCType<SlimeSpawnCrimson>(), 118 },
				{ ModContent.NPCType<SlimeSpawnCrimson2>(), 98 },

				{ NPCID.WallofFlesh, 186 },
				{ NPCID.TheHungry, 60 }, // Ranges from 60 to 150 depending on WoF life
				{ NPCID.TheHungryII, 79 },
				{ NPCID.LeechHead, 68 },
				{ NPCID.LeechBody, 56 },
				{ NPCID.LeechTail, 44 },

				{ ModContent.NPCType<Cryogen>(), 111 },
				{ ModContent.NPCType<CryogenIce>(), 124 },
				{ ModContent.NPCType<Cryocore>(), 92 },
				{ ModContent.NPCType<Cryocore2>(), 105 },
				{ ModContent.NPCType<IceMass>(), 105 },

				{ NPCID.Spazmatism, 105 }, // 157 in phase 2
				{ NPCID.Retinazer, 94 }, // 141 in phase 2

				{ ModContent.NPCType<AquaticScourgeHead>(), 198 },
				{ ModContent.NPCType<AquaticScourgeBody>(), 136 },
				{ ModContent.NPCType<AquaticScourgeBodyAlt>(), 115 },
				{ ModContent.NPCType<AquaticScourgeTail>(), 100 },

				{ NPCID.TheDestroyer, 324 },
				{ NPCID.TheDestroyerBody, 122 },
				{ NPCID.TheDestroyerTail, 89 },
				{ NPCID.Probe, 0 },

				{ ModContent.NPCType<BrimstoneElemental>(), 126 },

				{ NPCID.SkeletronPrime, 97 }, // 195 while spinning
				{ NPCID.PrimeVice, 116 },
				{ NPCID.PrimeSaw, 125 },
				{ NPCID.PrimeCannon, 67 },
				{ NPCID.PrimeLaser, 64 },

				{ ModContent.NPCType<Calamitas>(), 109 },
				{ ModContent.NPCType<CalamitasRun3>(), 138 },
				{ ModContent.NPCType<CalamitasRun>(), 148 },
				{ ModContent.NPCType<CalamitasRun2>(), 161 },

				{ NPCID.Plantera, 142 }, // 199 in phase 2
				{ NPCID.PlanterasTentacle, 148 },
				{ NPCID.Spore, 156 },

				{ ModContent.NPCType<Leviathan>(), 189 },
				{ ModContent.NPCType<Siren>(), 147 },
				{ ModContent.NPCType<SirenIce>(), 136 },
				{ NPCID.DetonatingBubble, 173 },
				{ ModContent.NPCType<AquaticAberration>(), 136 },
				{ ModContent.NPCType<Parasea>(), 124 },

				{ ModContent.NPCType<AstrumAureus>(), 168 },
				{ ModContent.NPCType<AureusSpawn>(), 165 },

				{ NPCID.Golem, 178 },
				{ NPCID.GolemHead, 126 },
				{ NPCID.GolemFistLeft, 116 },
				{ NPCID.GolemFistRight, 116 },

				{ ModContent.NPCType<PlaguebringerGoliath>(), 223 },
				{ ModContent.NPCType<PlaguebringerShade>(), 173 },
				{ ModContent.NPCType<PlagueHomingMissile>(), 223 },
				{ ModContent.NPCType<PlagueMine>(), 240 },
				{ ModContent.NPCType<PlagueBeeG>(), 148 },
				{ ModContent.NPCType<PlagueBeeLargeG>(), 161 },

				{ NPCID.DukeFishron, 173 }, // 249 in phase 2, 228 in phase 3
				{ NPCID.Sharkron, 186 },
				{ NPCID.Sharkron2, 223 },

				{ ModContent.NPCType<RavagerBody>(), 238 },
				{ ModContent.NPCType<RavagerClawLeft>(), 198 },
				{ ModContent.NPCType<RavagerClawRight>(), 198 },
				{ ModContent.NPCType<RockPillar>(), 200 },
				{ ModContent.NPCType<FlamePillar>(), 180 },

				{ NPCID.CultistDragonHead, 148 },
				{ NPCID.CultistDragonBody1, 74 },
				{ NPCID.CultistDragonBody2, 74 },
				{ NPCID.CultistDragonBody3, 74 },
				{ NPCID.CultistDragonBody4, 74 },
				{ NPCID.CultistDragonTail, 74 },
				{ NPCID.AncientCultistSquidhead, 223 },
				{ NPCID.AncientLight, 223 },

				{ ModContent.NPCType<AstrumDeusHeadSpectral>(), 278 },
				{ ModContent.NPCType<AstrumDeusBodySpectral>(), 190 },
				{ ModContent.NPCType<AstrumDeusTailSpectral>(), 154 },
			};

			public static SortedDictionary<int, int> MasterContactDamage = new SortedDictionary<int, int>
			{
				{ NPCID.KingSlime, 96 },

				{ ModContent.NPCType<DesertScourgeHead>(), 120 },
				{ ModContent.NPCType<DesertScourgeBody>(), 56 },
				{ ModContent.NPCType<DesertScourgeTail>(), 42 },
				{ ModContent.NPCType<DesertScourgeHeadSmall>(), 85 },
				{ ModContent.NPCType<DesertScourgeBodySmall>(), 38 },
				{ ModContent.NPCType<DesertScourgeTailSmall>(), 30 },
				{ ModContent.NPCType<DriedSeekerHead>(), 50 },
				{ ModContent.NPCType<DriedSeekerBody>(), 24 },
				{ ModContent.NPCType<DriedSeekerTail>(), 18 },

				{ NPCID.EyeofCthulhu, 45 }, // 54 in phase 2, 60 in phase 3; NOTE: In Death Mode, it is always in phase 3
				{ NPCID.ServantofCthulhu, 38 },

				{ ModContent.NPCType<CrabulonIdle>(), 120 },
				{ ModContent.NPCType<CrabShroom>(), 75 },

				{ NPCID.EaterofWorldsHead, 100 },
				{ NPCID.EaterofWorldsBody, 42 },
				{ NPCID.EaterofWorldsTail, 36 },
				{ NPCID.VileSpit, 96 },

				{ NPCID.BrainofCthulhu, 110 },
				{ NPCID.Creeper, 70 },

				{ ModContent.NPCType<HiveMind>(), 60 },
				{ ModContent.NPCType<HiveMindP2>(), 110 },
				{ ModContent.NPCType<DankCreeper>(), 85 },

				{ ModContent.NPCType<PerforatorHive>(), 90 },
				{ ModContent.NPCType<PerforatorHeadLarge>(), 150 },
				{ ModContent.NPCType<PerforatorBodyLarge>(), 60 },
				{ ModContent.NPCType<PerforatorTailLarge>(), 45 },
				{ ModContent.NPCType<PerforatorHeadMedium>(), 125 },
				{ ModContent.NPCType<PerforatorBodyMedium>(), 52 },
				{ ModContent.NPCType<PerforatorTailMedium>(), 37 },
				{ ModContent.NPCType<PerforatorHeadSmall>(), 100 },
				{ ModContent.NPCType<PerforatorBodySmall>(), 46 },
				{ ModContent.NPCType<PerforatorTailSmall>(), 41 },

				{ NPCID.QueenBee, 130 },
				{ NPCID.Bee, 50 },
				{ NPCID.BeeSmall, 40 },

				{ NPCID.SkeletronHead, 125 }, // 162 while spinning
				{ NPCID.SkeletronHand, 80 },

				{ ModContent.NPCType<SlimeGodCore>(), 140 },
				{ ModContent.NPCType<SlimeGod>(), 130 },
				{ ModContent.NPCType<SlimeGodSplit>(), 115 },
				{ ModContent.NPCType<SlimeGodRun>(), 145 },
				{ ModContent.NPCType<SlimeGodRunSplit>(), 130 },
				{ ModContent.NPCType<SlimeSpawnCorrupt>(), 120 },
				{ ModContent.NPCType<SlimeSpawnCorrupt2>(), 100 },
				{ ModContent.NPCType<SlimeSpawnCrimson>(), 130 },
				{ ModContent.NPCType<SlimeSpawnCrimson2>(), 110 },

				{ NPCID.WallofFlesh, 225 },
				{ NPCID.TheHungry, 90 }, // Ranges from 90 to 225 depending on WoF life
				{ NPCID.TheHungryII, 90 },
				{ NPCID.LeechHead, 78 },
				{ NPCID.LeechBody, 66 },
				{ NPCID.LeechTail, 54 },

				{ ModContent.NPCType<Cryogen>(), 145 },
				{ ModContent.NPCType<CryogenIce>(), 165 },
				{ ModContent.NPCType<Cryocore>(), 105 },
				{ ModContent.NPCType<Cryocore2>(), 120 },
				{ ModContent.NPCType<IceMass>(), 120 },

				{ NPCID.Spazmatism, 127 }, // 190 in phase 2
				{ NPCID.Retinazer, 114 }, // 171 in phase 2

				{ ModContent.NPCType<AquaticScourgeHead>(), 250 },
				{ ModContent.NPCType<AquaticScourgeBody>(), 180 },
				{ ModContent.NPCType<AquaticScourgeBodyAlt>(), 150 },
				{ ModContent.NPCType<AquaticScourgeTail>(), 110 },

				{ NPCID.TheDestroyer, 420 },
				{ NPCID.TheDestroyerBody, 140 },
				{ NPCID.TheDestroyerTail, 102 },

				{ ModContent.NPCType<BrimstoneElemental>(), 170 },

				{ NPCID.SkeletronPrime, 119 }, // 238 while spinning
				{ NPCID.PrimeVice, 132 },
				{ NPCID.PrimeSaw, 142 },
				{ NPCID.PrimeCannon, 76 },
				{ NPCID.PrimeLaser, 73 },

				{ ModContent.NPCType<Calamitas>(), 120 },
				{ ModContent.NPCType<CalamitasRun3>(), 165 },
				{ ModContent.NPCType<CalamitasRun>(), 190 },
				{ ModContent.NPCType<CalamitasRun2>(), 210 },

				{ NPCID.Plantera, 213 }, // 298 in phase 2
				{ NPCID.PlanterasTentacle, 206 },
				{ NPCID.Spore, 189 },

				{ ModContent.NPCType<Leviathan>(), 280 },
				{ ModContent.NPCType<Siren>(), 190 },
				{ ModContent.NPCType<SirenIce>(), 160 },
				{ NPCID.DetonatingBubble, 225 },
				{ ModContent.NPCType<AquaticAberration>(), 160 },
				{ ModContent.NPCType<Parasea>(), 140 },

				{ ModContent.NPCType<AstrumAureus>(), 250 },
				{ ModContent.NPCType<AureusSpawn>(), 240 },

				{ NPCID.Golem, 265 },
				{ NPCID.GolemHead, 153 },
				{ NPCID.GolemFistLeft, 141 },
				{ NPCID.GolemFistRight, 141 },

				{ ModContent.NPCType<PlaguebringerGoliath>(), 310 },
				{ ModContent.NPCType<PlaguebringerShade>(), 255 },
				{ ModContent.NPCType<PlagueHomingMissile>(), 310 },
				{ ModContent.NPCType<PlagueMine>(), 340 },
				{ ModContent.NPCType<PlagueBeeG>(), 195 },
				{ ModContent.NPCType<PlagueBeeLargeG>(), 210 },

				{ NPCID.DukeFishron, 210 }, // 302 in phase 2, 277 in phase 3
				{ NPCID.Sharkron, 225 },
				{ NPCID.Sharkron2, 270 },

				{ ModContent.NPCType<RavagerBody>(), 330 },
				{ ModContent.NPCType<RavagerClawLeft>(), 280 },
				{ ModContent.NPCType<RavagerClawRight>(), 280 },
				{ ModContent.NPCType<RockPillar>(), 280 },
				{ ModContent.NPCType<FlamePillar>(), 250 },

				{ NPCID.CultistDragonHead, 300 },
				{ NPCID.CultistDragonBody1, 150 },
				{ NPCID.CultistDragonBody2, 150 },
				{ NPCID.CultistDragonBody3, 150 },
				{ NPCID.CultistDragonBody4, 150 },
				{ NPCID.CultistDragonTail, 150 },
				{ NPCID.AncientCultistSquidhead, 300 },
				{ NPCID.AncientLight, 300 },

				{ ModContent.NPCType<AstrumDeusHeadSpectral>(), 400 },
				{ ModContent.NPCType<AstrumDeusBodySpectral>(), 260 },
				{ ModContent.NPCType<AstrumDeusTailSpectral>(), 220 },
			};
		};
    }
}
