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
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod
{
	public static class NPCStats
	{
		// I want to die
		public static void GetNPCDamage(this NPC npc)
		{
			double damageAdjustment = GetExpertDamageMultiplier(npc) * 2D;
			BossStats.ContactDamageValues.TryGetValue(npc.type, out int[] contactDamage);
			int normalDamage = contactDamage[0];
			int expertDamage = (int)(contactDamage[1] / damageAdjustment);
			int revengeanceDamage = (int)(contactDamage[2] / damageAdjustment);
			int deathDamage = (int)(contactDamage[3] / damageAdjustment);
			int masterDamage = (int)(contactDamage[4] / damageAdjustment);
			npc.damage = CalamityWorld.death ? deathDamage : CalamityWorld.revenge ? revengeanceDamage : Main.expertMode ? expertDamage : normalDamage;
		}

		/// <summary>
		/// Gets the Expert/Master Mode damage multiplier for the specified boss NPC.
		/// Useful for determining the base damage a boss NPC should have prior to being run through the Expert/Master scaling code.
		/// </summary>
		/// <param name="npc">The NPC you want to get the damage multiplier for</param>
		/// <param name="master">Whether Master Mode is enabled or not</param>
		/// <returns></returns>
		public static double GetExpertDamageMultiplier(this NPC npc, bool? master = null)
		{
			if (!BossStats.ExpertDamageMultiplier.ContainsKey(npc.type))
				return 1D;

			BossStats.ExpertDamageMultiplier.TryGetValue(npc.type, out double damageMult);
			return damageMult;
		}

		public struct BossStats
		{
			public static SortedDictionary<int, double> ExpertDamageMultiplier = new SortedDictionary<int, double>
			{
				{ NPCID.KingSlime, 0.8 },

				{ ModContent.NPCType<DesertScourgeHead>(), 1.1 },

				{ ModContent.NPCType<CrabulonIdle>(), 0.8 },

				{ NPCID.EaterofWorldsHead, 1.1 },
				{ NPCID.EaterofWorldsBody, 0.8 },
				{ NPCID.EaterofWorldsTail, 0.8 },

				{ NPCID.BrainofCthulhu, 0.9 },
				{ NPCID.Creeper, 0.9 }, // Off in all expert+ modes by some amount

				{ ModContent.NPCType<HiveMind>(), 0.9 },
				{ ModContent.NPCType<HiveMindP2>(), 0.9 },

				{ ModContent.NPCType<PerforatorHive>(), 0.9 },

				{ NPCID.QueenBee, 0.9 },
				{ NPCID.Bee, 0.6 },
				{ NPCID.BeeSmall, 0.6 },

				{ NPCID.SkeletronHead, 1.1 }, // Off in death mode by -3
				{ NPCID.SkeletronHand, 1.1 }, // Off in death mode by -3

				{ NPCID.WallofFlesh, 1.5 },
				{ NPCID.WallofFleshEye, 1.5 },

				{ ModContent.NPCType<Cryogen>(), 1.15 },

				{ NPCID.Spazmatism, 0.85 },
				{ NPCID.Retinazer, 0.85 },

				{ ModContent.NPCType<AquaticScourgeHead>(), 1.1 },
				{ ModContent.NPCType<AquaticScourgeBody>(), 0.8 },
				{ ModContent.NPCType<AquaticScourgeBodyAlt>(), 0.8 },
				{ ModContent.NPCType<AquaticScourgeTail>(), 0.8 },

				{ NPCID.TheDestroyer, 2 },
				{ NPCID.TheDestroyerBody, 0.85 },
				{ NPCID.TheDestroyerTail, 0.85 },

				{ ModContent.NPCType<BrimstoneElemental>(), 0.8 },

				{ NPCID.SkeletronPrime, 0.85 },
				{ NPCID.PrimeCannon, 0.85 },
				{ NPCID.PrimeLaser, 0.85 },
				{ NPCID.PrimeSaw, 0.85 },
				{ NPCID.PrimeVice, 0.85 },

				{ ModContent.NPCType<Calamitas>(), 0.8 },
				{ ModContent.NPCType<CalamitasRun3>(), 0.8 },

				{ NPCID.Plantera, 1.15 },
				{ NPCID.PlanterasTentacle, 1.15 },

				{ ModContent.NPCType<Leviathan>(), 1.2 },
				{ ModContent.NPCType<Siren>(), 0.8 },
				{ NPCID.DetonatingBubble, 0.75 },

				{ ModContent.NPCType<AstrumAureus>(), 1.1 },

				{ NPCID.Golem, 0.8 },
				{ NPCID.GolemHead, 0.8 },
				{ NPCID.GolemFistLeft, 0.8 },
				{ NPCID.GolemFistRight, 0.8 },

				{ ModContent.NPCType<PlaguebringerGoliath>(), 0.9 },

				{ NPCID.DukeFishron, 0.7 },
				{ NPCID.Sharkron, 0.75 },
				{ NPCID.Sharkron2, 0.75 },

				{ ModContent.NPCType<RavagerBody>(), 0.8 },

				{ NPCID.CultistDragonHead, 0.75 },
				{ NPCID.CultistDragonBody1, 0.75 },
				{ NPCID.CultistDragonBody2, 0.75 },
				{ NPCID.CultistDragonBody3, 0.75 },
				{ NPCID.CultistDragonBody4, 0.75 },
				{ NPCID.CultistDragonTail, 0.75 },
				{ NPCID.AncientDoom, 0.75 },
				{ NPCID.AncientLight, 0.75 },

				{ ModContent.NPCType<AstrumDeusBodySpectral>(), 0.8 },
				{ ModContent.NPCType<AstrumDeusTailSpectral>(), 0.8 },

				{ ModContent.NPCType<ProfanedGuardianBoss>(), 0.8 },
				{ ModContent.NPCType<ProfanedGuardianBoss2>(), 0.8 },
				{ ModContent.NPCType<ProfanedGuardianBoss3>(), 0.8 },

				{ ModContent.NPCType<Bumblefuck>(), 0.8 },

				{ ModContent.NPCType<StormWeaverBody>(), 0.8 },
				{ ModContent.NPCType<StormWeaverTail>(), 0.8 },
				{ ModContent.NPCType<StormWeaverBodyNaked>(), 0.8 },
				{ ModContent.NPCType<StormWeaverTailNaked>(), 0.8 },

				{ ModContent.NPCType<Signus>(), 0.9 },

				{ ModContent.NPCType<Polterghast>(), 0.8 },
				{ ModContent.NPCType<PolterPhantom>(), 0.8 },

				{ ModContent.NPCType<OldDuke>(), 0.7 },
				{ ModContent.NPCType<OldDukeToothBall>(), 0.75 },
				{ ModContent.NPCType<OldDukeSharkron>(), 0.75 },

				{ ModContent.NPCType<DevourerofGodsBody>(), 0.85 },
				{ ModContent.NPCType<DevourerofGodsTail>(), 0.85 },
				{ ModContent.NPCType<DevourerofGodsBodyS>(), 0.85 },
				{ ModContent.NPCType<DevourerofGodsTailS>(), 0.85 },

				{ ModContent.NPCType<Yharon>(), 0.8 },

				{ ModContent.NPCType<SupremeCalamitas>(), 0.8 }
			};

			public static SortedDictionary<int, int[]> ContactDamageValues = new SortedDictionary<int, int[]>
			{
				{ NPCID.KingSlime, new int[] { 40, 64, 80, 88, 96 } },

				{ ModContent.NPCType<DesertScourgeHead>(), new int[] { 30, 66, 88, 99, 132 } },
				{ ModContent.NPCType<DesertScourgeBody>(), new int[] { 16, 32, 40, 44, 60 } },
				{ ModContent.NPCType<DesertScourgeTail>(), new int[] { 12, 24, 30, 32, 42 } },
				{ ModContent.NPCType<DesertScourgeHeadSmall>(), new int[] { 0, 0, 60, 70, 90 } },
				{ ModContent.NPCType<DesertScourgeBodySmall>(), new int[] { 0, 0, 32, 36, 45 } },
				{ ModContent.NPCType<DesertScourgeTailSmall>(), new int[] { 0, 0, 20, 24, 30 } },
				{ ModContent.NPCType<DriedSeekerHead>(), new int[] { 14, 28, 36, 40, 51 } },
				{ ModContent.NPCType<DriedSeekerBody>(), new int[] { 8, 16, 20, 24, 30 } },
				{ ModContent.NPCType<DriedSeekerTail>(), new int[] { 6, 12, 16, 18, 24 } },

				{ NPCID.EyeofCthulhu, new int[] {
					15, // 23 in phase 2
					30, // 36 in phase 2, 40 in phase 3
					40, // 48 in phase 2, 56 in phase 3
					60, // Same for all phases
					51 } }, // Vanilla: 54 in phase 2, 60 in phase 3; Rev: 61 in phase 2, 71 in phase 3; Death: 71 at all times
				{ NPCID.ServantofCthulhu, new int[] { 12, 24, 30, 34, 42 } },

				{ ModContent.NPCType<CrabulonIdle>(), new int[] { 40, 64, 80, 88, 120 } },
				{ ModContent.NPCType<CrabShroom>(), new int[] { 25, 50, 62, 70, 75 } },

				{ NPCID.EaterofWorldsHead, new int[] { 25, 55, 77, 88, 132 } },
				{ NPCID.EaterofWorldsBody, new int[] { 15, 24, 32, 40, 60 } },
				{ NPCID.EaterofWorldsTail, new int[] { 10, 16, 24, 32, 42 } },
				{ NPCID.VileSpit, new int[] { 0, 64, 64, 64, 96 } },

				{ NPCID.BrainofCthulhu, new int[] { 30, 54, 81, 99, 135 } },
				{ NPCID.Creeper, new int[] { 20, 36, 54, 72, 90 } },

				{ ModContent.NPCType<HiveMind>(), new int[] { 20, 36, 45, 54, 81 } },
				{ ModContent.NPCType<HiveMindP2>(), new int[] { 35, 63, 81, 90, 135 } },
				{ ModContent.NPCType<DankCreeper>(), new int[] { 25, 50, 62, 68, 90 } },

				{ ModContent.NPCType<PerforatorHive>(), new int[] { 30, 54, 63, 72, 108 } },
				{ ModContent.NPCType<PerforatorHeadLarge>(), new int[] { 45, 90, 108, 118, 150 } },
				{ ModContent.NPCType<PerforatorBodyLarge>(), new int[] { 24, 48, 56, 60, 75 } },
				{ ModContent.NPCType<PerforatorTailLarge>(), new int[] { 18, 36, 42, 46, 60 } },
				{ ModContent.NPCType<PerforatorHeadMedium>(), new int[] { 35, 70, 84, 92, 126 } },
				{ ModContent.NPCType<PerforatorBodyMedium>(), new int[] { 21, 42, 50, 54, 69 } },
				{ ModContent.NPCType<PerforatorTailMedium>(), new int[] { 14, 28, 34, 38, 54 } },
				{ ModContent.NPCType<PerforatorHeadSmall>(), new int[] { 30, 60, 72, 80, 102 } },
				{ ModContent.NPCType<PerforatorBodySmall>(), new int[] { 18, 36, 42, 46, 60 } },
				{ ModContent.NPCType<PerforatorTailSmall>(), new int[] { 10, 20, 26, 30, 45 } },

				{ NPCID.QueenBee, new int[] { 30, 54, 81, 99, 135 } },
				{ NPCID.Bee, new int[] { 20, 24, 30, 36, 54 } },
				{ NPCID.BeeSmall, new int[] { 15, 18, 24, 30, 45 } },

				{ NPCID.SkeletronHead, new int[] {
					35, // Same for all phases
					77, // 100 while spinning
					88, // 114 while spinning
					99, // 128 while spinning
					132 } }, // 171 while spinning
				{ NPCID.SkeletronHand, new int[] { 20, 44, 55, 66, 99 } },

				{ ModContent.NPCType<SlimeGodCore>(), new int[] { 40, 80, 96, 104, 135 } },
				{ ModContent.NPCType<SlimeGod>(), new int[] { 45, 90, 108, 118, 150 } },
				{ ModContent.NPCType<SlimeGodSplit>(), new int[] { 40, 80, 96, 104, 135 } },
				{ ModContent.NPCType<SlimeGodRun>(), new int[] { 50, 100, 120, 130, 171 } },
				{ ModContent.NPCType<SlimeGodRunSplit>(), new int[] { 45, 90, 108, 118, 150 } },
				{ ModContent.NPCType<SlimeSpawnCorrupt>(), new int[] { 30, 60, 72, 78, 99 } },
				{ ModContent.NPCType<SlimeSpawnCorrupt2>(), new int[] { 20, 40, 48, 52, 66 } },
				{ ModContent.NPCType<SlimeSpawnCrimson>(), new int[] { 35, 70, 84, 92, 120 } },
				{ ModContent.NPCType<SlimeSpawnCrimson2>(), new int[] { 25, 50, 60, 66, 84 } },

				{ NPCID.WallofFlesh, new int[] { 50, 150, 180, 195, 225 } },
				{ NPCID.WallofFleshEye, new int[] { 50, 150, 180, 195, 225 } },
				{ NPCID.TheHungry, new int[] {
					30, // Ranges from 30 to 75 depending on WoF life
					60, // Ranges from 60 to 150 depending on WoF life
					60, // Ranges from 60 to 150 depending on WoF life
					60, // Ranges from 60 to 150 depending on WoF life
					90 } }, // Ranges from 90 to 225 depending on WoF life
				{ NPCID.TheHungryII, new int[] { 30, 60, 74, 80, 90 } },
				{ NPCID.LeechHead, new int[] { 26, 52, 62, 68, 78 } },
				{ NPCID.LeechBody, new int[] { 22, 44, 52, 56, 66 } },
				{ NPCID.LeechTail, new int[] { 18, 36, 42, 44, 54 } },

				{ ModContent.NPCType<Cryogen>(), new int[] { 50, 115, 138, 161, 207 } },
				{ ModContent.NPCType<CryogenIce>(), new int[] { 50, 100, 120, 134, 150 } },
				{ ModContent.NPCType<Cryocore>(), new int[] { 35, 70, 84, 92, 105 } },
				{ ModContent.NPCType<Cryocore2>(), new int[] { 40, 80, 96, 106, 120 } },
				{ ModContent.NPCType<IceMass>(), new int[] { 40, 80, 96, 106, 120 } },

				{ NPCID.Spazmatism, new int[] {
					60, // 90 in phase 2
					102, // 153 in phase 2
					119, // 178 in phase 2
					136, // 204 in phase 2
					204 } }, // 306 in phase 2
				{ NPCID.Retinazer, new int[] {
					50, // 75 in phase 2
					85, // 127 in phase 2
					102, // 153 in phase 2
					119, // 178 in phase 2
					153 } }, // 229 in phase 2

				{ ModContent.NPCType<AquaticScourgeHead>(), new int[] { 80, 176, 187, 198, 264 } },
				{ ModContent.NPCType<AquaticScourgeBody>(), new int[] { 65, 104, 128, 136, 168 } },
				{ ModContent.NPCType<AquaticScourgeBodyAlt>(), new int[] { 55, 88, 112, 120, 144 } },
				{ ModContent.NPCType<AquaticScourgeTail>(), new int[] { 45, 72, 96, 104, 120 } },

				{ NPCID.TheDestroyer, new int[] { 70, 280, 320, 340, 420 } },
				{ NPCID.TheDestroyerBody, new int[] { 60, 102, 119, 136, 204 } },
				{ NPCID.TheDestroyerTail, new int[] { 40, 68, 85, 102, 153 } },

				{ ModContent.NPCType<BrimstoneElemental>(), new int[] { 60, 96, 120, 136, 180 } },

				{ NPCID.SkeletronPrime, new int[] {
					50, // 100 while spinning
					85, // 170 while spinning
					102, // 204 while spinning
					119, // 238 while spinning
					153 } }, // 306 while spinning
				{ NPCID.PrimeVice, new int[] { 60, 102, 119, 136, 204 } },
				{ NPCID.PrimeSaw, new int[] { 60, 102, 119, 136, 204 } },
				{ NPCID.PrimeCannon, new int[] { 30, 51, 68, 85, 102 } },
				{ NPCID.PrimeLaser, new int[] { 30, 51, 68, 85, 102 } },

				{ ModContent.NPCType<Calamitas>(), new int[] { 55, 88, 104, 112, 144 } },
				{ ModContent.NPCType<CalamitasRun3>(), new int[] { 70, 112, 128, 136, 168 } },
				{ ModContent.NPCType<CalamitasRun>(), new int[] { 60, 120, 138, 148, 198 } },
				{ ModContent.NPCType<CalamitasRun2>(), new int[] { 65, 130, 150, 162, 216 } },

				{ NPCID.Plantera, new int[] {
					50, // 70 in phase 2, vanilla is retarded and doesn't use the expert multiplier for plantera's damage
					100, // 140 in phase 2, vanilla is retarded and doesn't use the expert multiplier for plantera's damage
					138, // 193 in phase 2
					161, // 225 in phase 2
					207 } }, // Vanilla: Is retarded, so plantera does 150 in phase 1 and 210 in phase 2; Rev and Death: 289 in phase 2
				{ NPCID.PlanterasTentacle, new int[] { 60, 138, 161, 207, 276 } },
				{ NPCID.Spore, new int[] { 70, 140, 160, 170, 210 } },

				{ ModContent.NPCType<Leviathan>(), new int[] { 90, 216, 240, 252, 324 } },
				{ ModContent.NPCType<Siren>(), new int[] {
					70, // 105 during charge
					112, // 168 during charge
					136, // 204 during charge
					144, // 216 during charge
					192 } }, // 288 during charge
				{ ModContent.NPCType<SirenIce>(), new int[] { 55, 110, 126, 136, 165 } },
				{ NPCID.DetonatingBubble, new int[] { 100, 150, 180, 195, 225 } },
				{ ModContent.NPCType<AquaticAberration>(), new int[] { 55, 110, 126, 136, 165 } },
				{ ModContent.NPCType<Parasea>(), new int[] { 50, 100, 116, 128, 150 } },

				{ ModContent.NPCType<AstrumAureus>(), new int[] { 80, 176, 198, 209, 264 } },
				{ ModContent.NPCType<AureusSpawn>(), new int[] { 0, 0, 150, 180, 240 } },

				{ NPCID.Golem, new int[] { 90, 144, 176, 192, 240 } },
				{ NPCID.GolemHead, new int[] { 80, 128, 144, 160, 192 } },
				{ NPCID.GolemFistLeft, new int[] { 70, 112, 144, 160, 180 } },
				{ NPCID.GolemFistRight, new int[] { 70, 112, 144, 160, 180 } },

				{ ModContent.NPCType<PlaguebringerGoliath>(), new int[] { 100, 180, 216, 234, 297 } },
				{ ModContent.NPCType<PlaguebringerShade>(), new int[] { 70, 140, 160, 170, 240 } },
				{ ModContent.NPCType<PlagueHomingMissile>(), new int[] { 90, 180, 210, 224, 270 } },
				{ ModContent.NPCType<PlagueMine>(), new int[] { 0, 200, 240, 260, 330 } },
				{ ModContent.NPCType<PlagueBeeG>(), new int[] { 60, 120, 140, 150, 195 } },
				{ ModContent.NPCType<PlagueBeeLargeG>(), new int[] { 65, 130, 150, 160, 210 } },

				{ NPCID.DukeFishron, new int[] {
					100, // 120 in phase 2
					140, // 201 in phase 2, 184 in phase 3
					168, // 241 in phase 2, 221 in phase 3
					182, // 262 in phase 2, 240 in phase 3
					210 } }, // 302 in phase 2, 277 in phase 3
				{ NPCID.Sharkron, new int[] { 100, 150, 180, 195, 225 } },
				{ NPCID.Sharkron2, new int[] { 120, 180, 210, 225, 270 } },

				{ ModContent.NPCType<RavagerBody>(), new int[] { 120, 192, 224, 232, 288 } },
				{ ModContent.NPCType<RavagerClawLeft>(), new int[] { 80, 160, 180, 200, 240 } },
				{ ModContent.NPCType<RavagerClawRight>(), new int[] { 80, 160, 180, 200, 240 } },
				{ ModContent.NPCType<RockPillar>(), new int[] { 120, 192, 224, 232, 288 } },
				{ ModContent.NPCType<FlamePillar>(), new int[] { 100, 160, 192, 200, 216 } },

				{ NPCID.CultistDragonHead, new int[] { 120, 180, 210, 225, 270 } },
				{ NPCID.CultistDragonBody1, new int[] { 60, 90, 105, 120, 135 } },
				{ NPCID.CultistDragonBody2, new int[] { 60, 90, 105, 120, 135 } },
				{ NPCID.CultistDragonBody3, new int[] { 60, 90, 105, 120, 135 } },
				{ NPCID.CultistDragonBody4, new int[] { 60, 90, 105, 120, 135 } },
				{ NPCID.CultistDragonTail, new int[] { 60, 90, 105, 120, 135 } },
				{ NPCID.AncientCultistSquidhead, new int[] { 90, 180, 210, 225, 270 } },
				{ NPCID.AncientDoom, new int[] { 30, 45, 0, 0, 90 } }, // Vanilla: 90 in Master Mode; Rev and Death: 0 in Master Mode
				{ NPCID.AncientLight, new int[] { 120, 180, 210, 225, 270 } },

				{ ModContent.NPCType<AstrumDeusHeadSpectral>(), new int[] { 120, 240, 268, 280, 360 } },
				{ ModContent.NPCType<AstrumDeusBodySpectral>(), new int[] { 100, 160, 192, 200, 240 } },
				{ ModContent.NPCType<AstrumDeusTailSpectral>(), new int[] { 80, 128, 160, 168, 192 } },

				{ ModContent.NPCType<ProfanedGuardianBoss>(), new int[] { 140, 224, 256, 280, 336 } },
				{ ModContent.NPCType<ProfanedGuardianBoss2>(), new int[] { 110, 176, 200, 216, 264 } },
				{ ModContent.NPCType<ProfanedGuardianBoss3>(), new int[] { 90, 144, 168, 184, 216 } },

				{ ModContent.NPCType<Bumblefuck>(), new int[] { 160, 256, 288, 304, 384 } },
				{ ModContent.NPCType<Bumblefuck2>(), new int[] { 110, 220, 242, 256, 330 } },

				{ ModContent.NPCType<ProvSpawnOffense>(), new int[] { 120, 240, 264, 278, 336 } },
				{ ModContent.NPCType<ProvSpawnDefense>(), new int[] { 100, 200, 220, 232, 270 } },

				{ ModContent.NPCType<CeaselessVoid>(), new int[] { 150, 300, 330, 348, 450 } },
				{ ModContent.NPCType<DarkEnergy>(), new int[] { 120, 240, 264, 278, 360 } },
				{ ModContent.NPCType<DarkEnergy2>(), new int[] { 120, 240, 264, 278, 360 } },
				{ ModContent.NPCType<DarkEnergy3>(), new int[] { 120, 240, 264, 278, 360 } },

				{ ModContent.NPCType<StormWeaverHead>(), new int[] { 140, 280, 308, 324, 420 } },
				{ ModContent.NPCType<StormWeaverBody>(), new int[] { 100, 160, 192, 208, 300 } },
				{ ModContent.NPCType<StormWeaverTail>(), new int[] { 80, 128, 160, 176, 240 } },
				{ ModContent.NPCType<StormWeaverHeadNaked>(), new int[] { 180, 360, 396, 418, 540 } },
				{ ModContent.NPCType<StormWeaverBodyNaked>(), new int[] { 120, 192, 224, 250, 330 } },
				{ ModContent.NPCType<StormWeaverTailNaked>(), new int[] { 100, 160, 192, 210, 270 } },

				{ ModContent.NPCType<Signus>(), new int[] { 175, 315, 351, 369, 459 } },
				{ ModContent.NPCType<CosmicLantern>(), new int[] { 110, 220, 242, 256, 336 } },
				{ ModContent.NPCType<SignusBomb>(), new int[] { 0, 0, 300, 320, 390 } },

				{ ModContent.NPCType<Polterghast>(), new int[] {
					150, // 180 in phase 2, 210 in phase 3
					240, // 288 in phase 2, 336 in phase 3
					264, // 316 in phase 2, 369 in phase 3
					280, // 336 in phase 2, 392 in phase 3
					384 } }, // 460 in phase 2, 537 in phase 3
				{ ModContent.NPCType<PolterPhantom>(), new int[] { 210, 336, 360, 392, 528 } },

				{ ModContent.NPCType<OldDuke>(), new int[] {
					160, // 192 in phase 2, 208 in phase 3
					224, // 268 in phase 2, 291 in phase 3
					280, // 336 in phase 2, 364 in phase 3
					294, // 352 in phase 2, 382 in phase 3
					378 } }, // 453 in phase 2, 491 in phase 3
				{ ModContent.NPCType<OldDukeToothBall>(), new int[] { 180, 270, 300, 315, 405 } },
				{ ModContent.NPCType<OldDukeSharkron>(), new int[] { 180, 270, 300, 315, 405 } },

				{ ModContent.NPCType<DevourerofGodsHead>(), new int[] { 250, 500, 550, 580, 660 } },
				{ ModContent.NPCType<DevourerofGodsBody>(), new int[] { 180, 306, 340, 357, 510 } },
				{ ModContent.NPCType<DevourerofGodsTail>(), new int[] { 150, 255, 289, 306, 408 } },
				{ ModContent.NPCType<DevourerofGodsHeadS>(), new int[] { 300, 600, 650, 680, 780 } }, // Death Mode damage is an instant kill
				{ ModContent.NPCType<DevourerofGodsBodyS>(), new int[] { 220, 374, 425, 442, 561 } },
				{ ModContent.NPCType<DevourerofGodsTailS>(), new int[] { 180, 306, 340, 357, 459 } },
				{ ModContent.NPCType<DevourerofGodsHead2>(), new int[] { 180, 360, 396, 420, 510 } },
				{ ModContent.NPCType<DevourerofGodsBody2>(), new int[] { 130, 260, 290, 320, 420 } },
				{ ModContent.NPCType<DevourerofGodsTail2>(), new int[] { 100, 200, 230, 260, 330 } },

				{ ModContent.NPCType<Yharon>(), new int[] { 330, 528, 560, 576, 690 } },
				{ ModContent.NPCType<DetonatingFlare>(), new int[] { 100, 200, 220, 232, 300 } },
				{ ModContent.NPCType<DetonatingFlare2>(), new int[] { 220, 440, 462, 476, 540 } },

				{ ModContent.NPCType<SupremeCalamitas>(), new int[] { 350, 560, 592, 608, 768 } }
			};
		};
    }
}
