using System;
using System.Collections.Generic;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod
{
    // TODO -- This can be made into a ModSystem with simple OnModLoad and Unload hooks.
    public static partial class NPCStats
    {
        private const double ExpertContactVanillaMultiplier = 2D;
        private const double MasterContactVanillaMultiplier = 3D;
        private const double NormalProjectileVanillaMultiplier = 2D;
        private const double ExpertProjectileVanillaMultiplier = 4D;
        private const double MasterProjectileVanillaMultiplier = 6D;

        #region Enemy Stats Container Struct
        internal partial struct EnemyStats
        {
            public static SortedDictionary<int, double> ExpertDamageMultiplier;
            public static SortedDictionary<int, int[]> ContactDamageValues;
            public static SortedDictionary<Tuple<int, int>, int[]> ProjectileDamageValues;
        };
        #endregion

        #region Stat Retrieval Methods
        public static void GetNPCDamage(this NPC npc)
        {
            double damageAdjustment = GetExpertDamageMultiplier(npc) * (Main.masterMode ? MasterContactVanillaMultiplier : ExpertContactVanillaMultiplier);

            // Safety check: If for some reason the contact damage array is not initialized yet, set the NPC's damage to 1.
            bool exists = EnemyStats.ContactDamageValues.TryGetValue(npc.type, out int[] contactDamage);
            if (!exists)
                npc.damage = 1;

            int normalDamage = contactDamage[0];
            int expertDamage = contactDamage[1] == -1 ? -1 : (int)Math.Round(contactDamage[1] / damageAdjustment);
            int revengeanceDamage = contactDamage[2] == -1 ? -1 : (int)Math.Round(contactDamage[2] / damageAdjustment);
            int deathDamage = contactDamage[3] == -1 ? -1 : (int)Math.Round(contactDamage[3] / damageAdjustment);
            int masterDamage = contactDamage[4] == -1 ? -1 : (int)Math.Round(contactDamage[4] / damageAdjustment);

            // If the assigned value would be -1, don't actually assign it. This allows for conditionally disabling the system.
            int damageToUse = Main.masterMode ? masterDamage : CalamityWorld.death ? deathDamage : CalamityWorld.revenge ? revengeanceDamage : Main.expertMode ? expertDamage : normalDamage;
            if (damageToUse != -1)
                npc.damage = damageToUse;
        }

        // Gets the amount of damage a given projectile should do from this NPC.
        // Automatically compensates for Terraria's internal spaghetti scaling.
        public static int GetProjectileDamage(this NPC npc, int projType)
        {
            double damageAdjustment = Main.masterMode ? MasterProjectileVanillaMultiplier : Main.expertMode ? ExpertProjectileVanillaMultiplier : NormalProjectileVanillaMultiplier;

            // Safety check: If for some reason the projectile damage array is not initialized yet, return 1.
            bool exists = EnemyStats.ProjectileDamageValues.TryGetValue(new Tuple<int, int>(npc.type, projType), out int[] projectileDamage);
            if (!exists)
                return 1;

            int normalDamage = (int)Math.Round(projectileDamage[0] / damageAdjustment);
            int expertDamage = (int)Math.Round(projectileDamage[1] / damageAdjustment);
            int revengeanceDamage = (int)Math.Round(projectileDamage[2] / damageAdjustment);
            int deathDamage = (int)Math.Round(projectileDamage[3] / damageAdjustment);
            int masterDamage = (int)Math.Round(projectileDamage[4] / damageAdjustment);

            int damageToUse = Main.masterMode ? masterDamage : CalamityWorld.death ? deathDamage : CalamityWorld.revenge ? revengeanceDamage : Main.expertMode ? expertDamage : normalDamage;

            return damageToUse;
        }

        // Gets the amount of damage this projectile should do from a given NPC.
        // Automatically compensates for Terraria's internal spaghetti scaling.
        public static int GetProjectileDamage(this Projectile projectile, int npcType)
        {
            double damageAdjustment = Main.masterMode ? MasterProjectileVanillaMultiplier : Main.expertMode ? ExpertProjectileVanillaMultiplier : NormalProjectileVanillaMultiplier;

            // Safety check: If for some reason the projectile damage array is not initialized yet, return 1.
            bool exists = EnemyStats.ProjectileDamageValues.TryGetValue(new Tuple<int, int>(npcType, projectile.type), out int[] projectileDamage);
            if (!exists)
                return 1;

            int normalDamage = (int)Math.Round(projectileDamage[0] / damageAdjustment);
            int expertDamage = (int)Math.Round(projectileDamage[1] / damageAdjustment);
            int revengeanceDamage = (int)Math.Round(projectileDamage[2] / damageAdjustment);
            int deathDamage = (int)Math.Round(projectileDamage[3] / damageAdjustment);
            int masterDamage = (int)Math.Round(projectileDamage[4] / damageAdjustment);

            int damageToUse = Main.masterMode ? masterDamage : CalamityWorld.death ? deathDamage : CalamityWorld.revenge ? revengeanceDamage : Main.expertMode ? expertDamage : normalDamage;

            return damageToUse;
        }

        // Gets the raw amount of damage a projectile should do from this NPC.
        // That is, this doesn't adjust the value to compensate for Terraria's internal spaghetti scaling.
        public static int GetProjectileDamageNoScaling(this NPC npc, int projType)
        {
            bool exists = EnemyStats.ProjectileDamageValues.TryGetValue(new Tuple<int, int>(npc.type, projType), out int[] projectileDamage);
            return !exists ? 1 // Base case for safety, in case the array is not initialized yet.
                : Main.masterMode ? projectileDamage[4]
                : CalamityWorld.death ? projectileDamage[3]
                : CalamityWorld.revenge ? projectileDamage[2]
                : Main.expertMode ? projectileDamage[1]
                : projectileDamage[0];
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
            bool exists = EnemyStats.ExpertDamageMultiplier.TryGetValue(npc.type, out double damageMult);
            return exists ? damageMult : 1D;
        }
        #endregion

        #region Load/Unload
        internal static void Load()
        {
            LoadEnemyStats();
            LoadDebuffs();
        }
        internal static void Unload()
        {
            UnloadEnemyStats();
            UnloadDebuffs();
        }

        // A static function, called exactly once, which initializes the EnemyStats struct at a predictable time.
        // This is necessary to ensure this dictionary is populated as early as possible.
        internal static void LoadEnemyStats()
        {
            EnemyStats.ExpertDamageMultiplier = new SortedDictionary<int, double>
            {
                { NPCID.KingSlime, 0.8 },

                { ModContent.NPCType<DesertScourgeHead>(), 1.1 },

                { ModContent.NPCType<Crabulon>(), 0.8 },

                { NPCID.EaterofWorldsHead, 1.1 },
                { NPCID.EaterofWorldsBody, 0.8 },
                { NPCID.EaterofWorldsTail, 0.8 },

                { NPCID.BrainofCthulhu, 0.9 },

                { ModContent.NPCType<HiveMind>(), 0.9 },

                { ModContent.NPCType<PerforatorHive>(), 0.9 },

                { NPCID.QueenBee, 0.9 },
                { NPCID.Bee, 0.6 },
                { NPCID.BeeSmall, 0.6 },

                { NPCID.SkeletronHead, 1.1 },
                { NPCID.SkeletronHand, 1.1 },

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

                { ModContent.NPCType<CalamitasClone>(), 0.8 },

                { NPCID.Plantera, 1.15 },
                { NPCID.PlanterasTentacle, 1.15 },
                { ModContent.NPCType<PlanterasFreeTentacle>(), 1.15 },

                { NPCID.HallowBoss, 0.6875 },

                { ModContent.NPCType<Leviathan>(), 1.2 },
                { ModContent.NPCType<Anahita>(), 0.8 },
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

                { ModContent.NPCType<AstrumDeusBody>(), 0.8 },
                { ModContent.NPCType<AstrumDeusTail>(), 0.8 },

                { ModContent.NPCType<ProfanedGuardianCommander>(), 0.8 },

                { ModContent.NPCType<Bumblefuck>(), 0.8 },

                { ModContent.NPCType<StormWeaverBody>(), 0.8 },
                { ModContent.NPCType<StormWeaverTail>(), 0.8 },

                { ModContent.NPCType<Signus>(), 0.9 },

                { ModContent.NPCType<Polterghast>(), 0.8 },
                { ModContent.NPCType<PolterPhantom>(), 0.8 },

                { ModContent.NPCType<OldDuke>(), 0.9 },
                { ModContent.NPCType<OldDukeToothBall>(), 0.8 },
                { ModContent.NPCType<SulphurousSharkron>(), 0.8 },

                { ModContent.NPCType<DevourerofGodsBody>(), 0.85 },
                { ModContent.NPCType<DevourerofGodsTail>(), 0.85 },

                { ModContent.NPCType<Yharon>(), 0.8 },

                { ModContent.NPCType<SupremeCalamitas>(), 0.8 },

                { ModContent.NPCType<Apollo>(), 0.8 },
                { ModContent.NPCType<Artemis>(), 0.8 },

                { ModContent.NPCType<ThanatosHead>(), 0.8 },
                { ModContent.NPCType<ThanatosBody1>(), 0.8 },
                { ModContent.NPCType<ThanatosBody2>(), 0.8 },
                { ModContent.NPCType<ThanatosTail>(), 0.8 },

                { ModContent.NPCType<PrimordialWyrmHead>(), 0.8 }
            };

            EnemyStats.ContactDamageValues = new SortedDictionary<int, int[]>
            {
                { NPCID.KingSlime, new int[] { 40, 64, 80, 96, 144 } },
                { ModContent.NPCType<KingSlimeJewelEmerald>(), new int[] { 30, 44, 60, 76, 114 } },

                { ModContent.NPCType<DesertScourgeHead>(), new int[] { 45, 66, 88, 110, 165 } },
                { ModContent.NPCType<DesertScourgeBody>(), new int[] { 24, 32, 40, 48, 72 } },
                { ModContent.NPCType<DesertScourgeTail>(), new int[] { 18, 24, 30, 36, 54 } },
                { ModContent.NPCType<DesertNuisanceHead>(), new int[] { 30, 44, 60, 76, 114 } },
                { ModContent.NPCType<DesertNuisanceBody>(), new int[] { 21, 28, 32, 36, 63 } },
                { ModContent.NPCType<DesertNuisanceTail>(), new int[] { 12, 16, 20, 24, 36 } },
                { ModContent.NPCType<DesertNuisanceHeadYoung>(), new int[] { 30, 44, 60, 76, 114 } },
                { ModContent.NPCType<DesertNuisanceBodyYoung>(), new int[] { 21, 28, 32, 36, 63 } },
                { ModContent.NPCType<DesertNuisanceTailYoung>(), new int[] { 12, 16, 20, 24, 36 } },

                { NPCID.EyeofCthulhu, new int[] {
                    24, // 36 in phase 2
                    30, // 45 in phase 2
                    40, // 48 in phase 2, 56 in phase 3
                    50, // 60 in phase 2, 70 in phase 3
                    75 } }, // Vanilla: 113 in phase 2; Rev+: 90 in phase 2, 105 in phase 3
                { NPCID.ServantofCthulhu, new int[] { 18, 24, 30, 36, 54 } },
                { ModContent.NPCType<BloodlettingServant>(), new int[] { 20, 26, 34, 42, 63 } }, // In phase 2: 26, 34, 44, 55, 82

                { ModContent.NPCType<Crabulon>(), new int[] { 40, 64, 80, 96, 144 } },
                { ModContent.NPCType<CrabShroom>(), new int[] { 25, 50, 62, 74, 111 } },

                { NPCID.EaterofWorldsHead, new int[] { 38, 55, 77, 99, 165 } },
                { NPCID.EaterofWorldsBody, new int[] { 18, 24, 32, 40, 60 } },
                { NPCID.EaterofWorldsTail, new int[] { 12, 16, 24, 32, 48 } },
                { NPCID.VileSpitEaterOfWorlds, new int[] { -1, 64, 64, 64, 96 } },

                { NPCID.BrainofCthulhu, new int[] { 36, 54, 81, 108, 162 } },
                { NPCID.Creeper, new int[] { 24, 36, 66, 96, 144 } },

                { ModContent.NPCType<HiveMind>(), new int[] { 42, 63, 81, 99, 162 } },
                { ModContent.NPCType<DankCreeper>(), new int[] { 30, 50, 62, 74, 111 } },

                { ModContent.NPCType<PerforatorHive>(), new int[] { 36, 54, 63, 72, 108 } },
                { ModContent.NPCType<PerforatorHeadLarge>(), new int[] { 54, 90, 108, 126, 189 } },
                { ModContent.NPCType<PerforatorBodyLarge>(), new int[] { 28, 48, 56, 64, 96 } },
                { ModContent.NPCType<PerforatorTailLarge>(), new int[] { 22, 36, 42, 48, 72 } },
                { ModContent.NPCType<PerforatorHeadMedium>(), new int[] { 42, 70, 84, 98, 147 } },
                { ModContent.NPCType<PerforatorBodyMedium>(), new int[] { 24, 42, 50, 58, 87 } },
                { ModContent.NPCType<PerforatorTailMedium>(), new int[] { 18, 28, 34, 40, 60 } },
                { ModContent.NPCType<PerforatorHeadSmall>(), new int[] { 36, 60, 72, 84, 126 } },
                { ModContent.NPCType<PerforatorBodySmall>(), new int[] { 20, 36, 42, 48, 72 } },
                { ModContent.NPCType<PerforatorTailSmall>(), new int[] { 14, 20, 26, 32, 48 } },

                { NPCID.QueenBee, new int[] { 36, 54, 81, 108, 162 } },
                { NPCID.Bee, new int[] { 20, 24, 48, 72, 108 } },
                { NPCID.BeeSmall, new int[] { 15, 18, 36, 54, 81 } },

                { NPCID.SkeletronHead, new int[] {
                    42, // 55 while spinning
                    77, // 100 while spinning
                    88, // 114 while spinning
                    99, // 128 while spinning
                    165 } }, // 215 while spinning
                { NPCID.SkeletronHand, new int[] { 32, 44, 77, 110, 165 } },

                { NPCID.Deerclops, new int[] { 20, 40, 50, 60, 90 } },

                { ModContent.NPCType<SlimeGodCore>(), new int[] { 48, 80, 96, 112, 168 } },
                { ModContent.NPCType<EbonianPaladin>(), new int[] { 54, 90, 108, 126, 189 } },
                { ModContent.NPCType<SplitEbonianPaladin>(), new int[] { 48, 80, 96, 112, 168 } },
                { ModContent.NPCType<CrimulanPaladin>(), new int[] { 60, 100, 120, 140, 210 } },
                { ModContent.NPCType<SplitCrimulanPaladin>(), new int[] { 54, 90, 108, 126, 189 } },
                { ModContent.NPCType<CorruptSlimeSpawn>(), new int[] { 36, 60, 72, 84, 126 } },
                { ModContent.NPCType<CorruptSlimeSpawn2>(), new int[] { 24, 40, 48, 56, 84 } },
                { ModContent.NPCType<CrimsonSlimeSpawn>(), new int[] { 42, 70, 84, 98, 147 } },
                { ModContent.NPCType<CrimsonSlimeSpawn2>(), new int[] { 30, 50, 60, 70, 105 } },

                { NPCID.WallofFlesh, new int[] { 100, 150, 180, 210, 315 } },
                { NPCID.WallofFleshEye, new int[] { 100, 150, 180, 210, 315 } },
                { NPCID.TheHungry, new int[] {
                    30, // Ranges from 30 to 75 depending on WoF life
                    60, // Ranges from 60 to 150 depending on WoF life
                    60, // Ranges from 60 to 150 depending on WoF life
                    60, // Ranges from 60 to 150 depending on WoF life
                    90 } }, // Ranges from 90 to 225 depending on WoF life
                { NPCID.TheHungryII, new int[] { 30, 60, 74, 88, 132 } },
                { NPCID.LeechHead, new int[] { 26, 52, 62, 72, 108 } },
                { NPCID.LeechBody, new int[] { 22, 44, 52, 60, 90 } },
                { NPCID.LeechTail, new int[] { 18, 36, 42, 48, 72 } },

                { NPCID.QueenSlimeBoss, new int[] { 80, 120, 150, 180, 270 } },
                { NPCID.QueenSlimeMinionBlue, new int[] { 60, 80, 100, 120, 180 } },
                { NPCID.QueenSlimeMinionPink, new int[] { 60, 80, 100, 120, 180 } },
                { NPCID.QueenSlimeMinionPurple, new int[] { 70, 100, 120, 140, 210 } },

                { ModContent.NPCType<Cryogen>(), new int[] { 90, 138, 161, 184, 276 } },
                { ModContent.NPCType<CryogenShield>(), new int[] { 80, 120, 138, 156, 240 } },

                { NPCID.Spazmatism, new int[] {
                    70, // 105 in phase 2
                    102, // 153 in phase 2
                    119, // 178 in phase 2
                    136, // 204 in phase 2
                    204 } }, // 306 in phase 2
                { NPCID.Retinazer, new int[] {
                    55, // 83 in phase 2
                    85, // 127 in phase 2
                    102, // 153 in phase 2
                    119, // 178 in phase 2
                    153 } }, // 229 in phase 2

                { ModContent.NPCType<AquaticScourgeHead>(), new int[] { 110, 176, 187, 198, 330 } },
                { ModContent.NPCType<AquaticScourgeBody>(), new int[] { 75, 112, 136, 160, 240 } },
                { ModContent.NPCType<AquaticScourgeBodyAlt>(), new int[] { 70, 104, 112, 120, 192 } },
                { ModContent.NPCType<AquaticScourgeTail>(), new int[] { 65, 96, 104, 112, 168 } },

                { NPCID.TheDestroyer, new int[] { 140, 280, 300, 320, 480 } },
                { NPCID.TheDestroyerBody, new int[] { 70, 102, 136, 170, 255 } },
                { NPCID.TheDestroyerTail, new int[] { 45, 68, 102, 136, 204 } },

                { ModContent.NPCType<BrimstoneElemental>(), new int[] { 75, 112, 136, 160, 240 } },

                { NPCID.SkeletronPrime, new int[] {
                    50, // 100 while spinning
                    85, // 170 while spinning
                    102, // 204 while spinning
                    119, // 238 while spinning
                    153 } }, // 306 while spinning
                { NPCID.PrimeVice, new int[] { 70, 102, 136, 170, 255 } },
                { NPCID.PrimeSaw, new int[] { 70, 102, 136, 170, 255 } },
                { NPCID.PrimeCannon, new int[] { 30, 51, 68, 85, 102 } },
                { NPCID.PrimeLaser, new int[] { 30, 51, 68, 85, 102 } },

                { ModContent.NPCType<CalamitasClone>(), new int[] { 95, 144, 168, 192, 288 } },
                { ModContent.NPCType<Cataclysm>(), new int[] { 80, 120, 138, 156, 240 } },
                { ModContent.NPCType<Catastrophe>(), new int[] { 85, 130, 150, 170, 255 } },

                { NPCID.Plantera, new int[] {
                    70, // 98 in phase 2
                    100, // 140 in phase 2
                    138, // 193 in phase 2
                    176, // 246 in phase 2
                    276 } }, // 386 in phase 2
                { NPCID.PlanterasTentacle, new int[] { 90, 138, 161, 184, 276 } },
                { ModContent.NPCType<PlanterasFreeTentacle>(), new int[] { 90, 138, 161, 184, 276 } },
                { NPCID.Spore, new int[] { 90, 140, 160, 180, 270 } },

                { NPCID.HallowBoss, new int[] { 80, // 120 during charge
                    110, // 165 during charge
                    143, // 215 during charge
                    176, // 264 during charge
                    264 } }, // 396 during charge

                { ModContent.NPCType<Leviathan>(), new int[] { 140, 216, 240, 264, 414 } },
                { ModContent.NPCType<Anahita>(), new int[] {
                    75, // 113 during charge
                    112, // 168 during charge
                    136, // 204 during charge
                    160, // 240 during charge
                    240 } }, // 360 during charge
                { ModContent.NPCType<AnahitasIceShield>(), new int[] { 70, 110, 126, 142, 210 } },
                { NPCID.DetonatingBubble, new int[] { 100, 150, 180, 210, 315 } },
                { ModContent.NPCType<AquaticAberration>(), new int[] { 90, 140, 160, 180, 270 } },

                { ModContent.NPCType<AstrumAureus>(), new int[] { 145, 220, 242, 264, 429 } },
                { ModContent.NPCType<AureusSpawn>(), new int[] { 75, 112, 128, 144, 225 } },

                { NPCID.Golem, new int[] { 95, 144, 176, 208, 312 } },
                { NPCID.GolemHead, new int[] { 85, 128, 144, 160, 240 } },
                { NPCID.GolemFistLeft, new int[] { 75, 112, 144, 176, 264 } },
                { NPCID.GolemFistRight, new int[] { 75, 112, 144, 176, 264 } },

                { ModContent.NPCType<PlaguebringerGoliath>(), new int[] { 120, 180, 216, 252, 378 } },
                { ModContent.NPCType<PlagueHomingMissile>(), new int[] { 120, 180, 210, 240, 360 } },
                { ModContent.NPCType<PlagueMine>(), new int[] { 130, 200, 240, 280, 420 } },

                { NPCID.DukeFishron, new int[] {
                    100, // 120 in phase 2
                    140, // 202 in phase 2, 185 in phase 3
                    168, // 242 in phase 2, 222 in phase 3
                    196, // 282 in phase 2, 259 in phase 3
                    294 } }, // 423 in phase 2, 388 in phase 3
                { NPCID.Sharkron, new int[] { 100, 150, 180, 210, 315 } },
                { NPCID.Sharkron2, new int[] { 120, 180, 210, 240, 360 } },

                { ModContent.NPCType<RavagerBody>(), new int[] { 130, 192, 224, 256, 384 } },
                { ModContent.NPCType<RavagerClawLeft>(), new int[] { 105, 160, 180, 200, 315 } },
                { ModContent.NPCType<RavagerClawRight>(), new int[] { 105, 160, 180, 200, 315 } },
                { ModContent.NPCType<RockPillar>(), new int[] { 130, 192, 224, 256, 390 } },
                { ModContent.NPCType<FlamePillar>(), new int[] { 105, 160, 192, 224, 336 } },

                { NPCID.CultistDragonHead, new int[] { 120, 180, 210, 240, 360 } },
                { NPCID.CultistDragonBody1, new int[] { 60, 90, 105, 120, 180 } },
                { NPCID.CultistDragonBody2, new int[] { 60, 90, 105, 120, 180 } },
                { NPCID.CultistDragonBody3, new int[] { 60, 90, 105, 120, 180 } },
                { NPCID.CultistDragonBody4, new int[] { 60, 90, 105, 120, 180 } },
                { NPCID.CultistDragonTail, new int[] { 60, 90, 105, 120, 180 } },
                { NPCID.AncientCultistSquidhead, new int[] { 120, 180, 210, 240, 360 } },
                { NPCID.AncientLight, new int[] { 120, 180, 210, 240, 360 } },

                { ModContent.NPCType<AstrumDeusHead>(), new int[] { 160, 240, 268, 296, 480 } },
                { ModContent.NPCType<AstrumDeusBody>(), new int[] { 105, 160, 192, 224, 336 } },
                { ModContent.NPCType<AstrumDeusTail>(), new int[] { 85, 128, 160, 192, 288 } },

                { ModContent.NPCType<ProfanedGuardianCommander>(), new int[] { 150, 224, 256, 288, 444 } },
                { ModContent.NPCType<ProfanedGuardianDefender>(), new int[] { 160, 240, 264, 288, 405 } },
                { ModContent.NPCType<ProfanedGuardianHealer>(), new int[] { 100, 200, 220, 240, 300 } },

                { ModContent.NPCType<Bumblefuck>(), new int[] { 170, 256, 288, 320, 504 } },
                { ModContent.NPCType<Bumblefuck2>(), new int[] { 145, 220, 242, 264, 435 } },

                { ModContent.NPCType<ProvSpawnOffense>(), new int[] { 120, 240, 264, 278, 360 } },
                { ModContent.NPCType<ProvSpawnDefense>(), new int[] { 100, 200, 220, 232, 300 } },
                { ModContent.NPCType<ProfanedRocks>(), new int[] { 100, 200, 220, 232, 300 } },

                { ModContent.NPCType<CeaselessVoid>(), new int[] { 240, 360, 396, 432, 720 } },
                { ModContent.NPCType<DarkEnergy>(), new int[] { 175, 260, 288, 316, 525 } },

                { ModContent.NPCType<StormWeaverHead>(), new int[] { 240, 360, 396, 432, 720 } },
                { ModContent.NPCType<StormWeaverBody>(), new int[] { 125, 192, 224, 256, 372 } },
                { ModContent.NPCType<StormWeaverTail>(), new int[] { 105, 160, 192, 224, 312 } },

                { ModContent.NPCType<Signus>(), new int[] { 210, 315, 351, 387, 621 } },
                { ModContent.NPCType<CosmicLantern>(), new int[] { 175, 260, 288, 316, 525 } },
                { ModContent.NPCType<CosmicMine>(), new int[] { 190, 280, 300, 320, 570 } },

                { ModContent.NPCType<Polterghast>(), new int[] {
                    160, // 192 in phase 2, 224 in phase 3
                    240, // 288 in phase 2, 336 in phase 3
                    264, // 317 in phase 2, 370 in phase 3
                    288, // 346 in phase 2, 403 in phase 3
                    480 } }, // 576 in phase 2, 672 in phase 3
                { ModContent.NPCType<PolterPhantom>(), new int[] { 224, 336, 370, 403, 672 } },

                { ModContent.NPCType<OldDuke>(), new int[] {
                    192, // 211 in phase 2, 230 in phase 3
                    288, // 317 in phase 2, 346 in phase 3
                    324, // 356 in phase 2, 389 in phase 3
                    360, // 396 in phase 2, 432 in phase 3
                    567 } }, // 624 in phase 2, 680 in phase 3
                { ModContent.NPCType<OldDukeToothBall>(), new int[] { 192, 288, 328, 368, 576 } },
                { ModContent.NPCType<SulphurousSharkron>(), new int[] { 192, 288, 328, 368, 576 } },

                { ModContent.NPCType<DevourerofGodsHead>(), new int[] { 470, 700, 750, 800, 1410 } },
                { ModContent.NPCType<DevourerofGodsBody>(), new int[] { 250, 374, 425, 476, 714 } },
                { ModContent.NPCType<DevourerofGodsTail>(), new int[] { 204, 306, 340, 374, 612 } },
                { ModContent.NPCType<CosmicGuardianHead>(), new int[] { 240, 360, 396, 432, 720 } },
                { ModContent.NPCType<CosmicGuardianBody>(), new int[] { 175, 260, 290, 320, 525 } },
                { ModContent.NPCType<CosmicGuardianTail>(), new int[] { 135, 200, 230, 260, 405 } },

                { ModContent.NPCType<Yharon>(), new int[] { 300, 448, 480, 512, 900 } },

                { ModContent.NPCType<SupremeCalamitas>(), new int[] { 340, 512, 544, 576, 1020 } },

                { ModContent.NPCType<Apollo>(), new int[] { 340, 512, 544, 576, 1020 } },
                { ModContent.NPCType<Artemis>(), new int[] { 320, 480, 512, 544, 960 } },

                { ModContent.NPCType<ThanatosHead>(), new int[] { 375, 560, 592, 624, 1116 } },
                { ModContent.NPCType<ThanatosBody1>(), new int[] { 320, 480, 512, 544, 960 } },
                { ModContent.NPCType<ThanatosBody2>(), new int[] { 320, 480, 512, 544, 960 } },
                { ModContent.NPCType<ThanatosTail>(), new int[] { 270, 400, 424, 448, 804 } },

                { ModContent.NPCType<PrimordialWyrmHead>(), new int[] { 535, 800, 850, 900, 1596 } }
            };

            EnemyStats.ProjectileDamageValues = new SortedDictionary<Tuple<int, int>, int[]>
            {
                { new Tuple<int, int>(ModContent.NPCType<KingSlimeJewelRuby>(), ModContent.ProjectileType<JewelProjectile>()), new int[] { 26, 36, 44, 52, 84 } },

                { new Tuple<int, int>(ModContent.NPCType<DesertScourgeHead>(), ModContent.ProjectileType<DesertScourgeSpit>()), new int[] { 26, 36, 44, 52, 84 } },
                { new Tuple<int, int>(ModContent.NPCType<DesertNuisanceHeadYoung>(), ModContent.ProjectileType<DesertScourgeSpit>()), new int[] { 26, 36, 44, 52, 84 } },

                { new Tuple<int, int>(NPCID.EyeofCthulhu, ProjectileID.BloodNautilusShot), new int[] { 26, 44, 60, 76, 114 } },
                { new Tuple<int, int>(ModContent.NPCType<BloodlettingServant>(), ProjectileID.BloodShot), new int[] { 26, 36, 44, 52, 84 } },

                { new Tuple<int, int>(ModContent.NPCType<Crabulon>(), ModContent.ProjectileType<MushBomb>()), new int[] { 32, 48, 60, 72, 108 } },
                { new Tuple<int, int>(ModContent.NPCType<Crabulon>(), ModContent.ProjectileType<MushBombFall>()), new int[] { 32, 48, 60, 72, 108 } },

                { new Tuple<int, int>(NPCID.EaterofWorldsHead, ProjectileID.CursedFlameHostile), new int[] { 26, 44, 60, 76, 114 } },
                { new Tuple<int, int>(NPCID.EaterofWorldsHead, ModContent.ProjectileType<ShadowflameFireball>()), new int[] { 26, 36, 44, 52, 84 } },

                { new Tuple<int, int>(NPCID.BrainofCthulhu, ProjectileID.BloodNautilusShot), new int[] { 36, 56, 68, 80, 120 } },
                { new Tuple<int, int>(NPCID.BrainofCthulhu, ProjectileID.BloodShot), new int[] { 26, 44, 60, 76, 114 } },
                { new Tuple<int, int>(NPCID.Creeper, ProjectileID.BloodShot), new int[] { 26, 44, 60, 76, 114 } },

                { new Tuple<int, int>(ModContent.NPCType<HiveMind>(), ModContent.ProjectileType<ShadeNimbusHostile>()), new int[] { 36, 56, 68, 80, 120 } },
                { new Tuple<int, int>(ModContent.NPCType<DankCreeper>(), ModContent.ProjectileType<ShadeNimbusHostile>()), new int[] { 36, 56, 68, 80, 120 } },
                { new Tuple<int, int>(ModContent.NPCType<DarkHeart>(), ModContent.ProjectileType<ShaderainHostile>()), new int[] { 36, 56, 68, 80, 120 } },
                { new Tuple<int, int>(ModContent.NPCType<HiveBlob>(), ModContent.ProjectileType<VileClot>()), new int[] { 30, 48, 60, 72, 108 } },
                { new Tuple<int, int>(ModContent.NPCType<HiveBlob2>(), ModContent.ProjectileType<VileClot>()), new int[] { 30, 48, 60, 72, 108 } },

                { new Tuple<int, int>(ModContent.NPCType<PerforatorHive>(), ModContent.ProjectileType<BloodGeyser>()), new int[] { 36, 56, 68, 80, 120 } },
                { new Tuple<int, int>(ModContent.NPCType<PerforatorHive>(), ModContent.ProjectileType<IchorShot>()), new int[] { 36, 56, 68, 80, 120 } },
                { new Tuple<int, int>(ModContent.NPCType<PerforatorHive>(), ModContent.ProjectileType<IchorBlob>()), new int[] { 36, 56, 68, 80, 120 } },
                { new Tuple<int, int>(ModContent.NPCType<PerforatorHeadMedium>(), ModContent.ProjectileType<IchorBlob>()), new int[] { 36, 56, 68, 80, 120 } },
                { new Tuple<int, int>(ModContent.NPCType<PerforatorBodyMedium>(), ModContent.ProjectileType<IchorBlob>()), new int[] { 36, 56, 68, 80, 120 } },
                { new Tuple<int, int>(ModContent.NPCType<PerforatorTailMedium>(), ModContent.ProjectileType<IchorBlob>()), new int[] { 36, 56, 68, 80, 120 } },
                { new Tuple<int, int>(ModContent.NPCType<PerforatorHeadLarge>(), ModContent.ProjectileType<DoGDeath>()), new int[] { 22, 36, 44, 52, 84 } },

                { new Tuple<int, int>(NPCID.QueenBee, ProjectileID.QueenBeeStinger), new int[] { 22, 44, 64, 84, 126 } },

                { new Tuple<int, int>(NPCID.SkeletronHead, ProjectileID.Skull), new int[] { 46, 72, 84, 96, 144 } },
                { new Tuple<int, int>(NPCID.SkeletronHead, ProjectileID.Shadowflames), new int[] { 36, 72, 88, 104, 156 } },
                { new Tuple<int, int>(NPCID.SkeletronHand, ProjectileID.Skull), new int[] { 36, 56, 68, 80, 120 } },

                { new Tuple<int, int>(NPCID.Deerclops, ProjectileID.DeerclopsIceSpike), new int[] { 26, 52, 76, 100, 150 } },
                { new Tuple<int, int>(NPCID.Deerclops, ProjectileID.DeerclopsRangedProjectile), new int[] { 36, 72, 88, 104, 156 } },
                { new Tuple<int, int>(NPCID.Deerclops, ProjectileID.InsanityShadowHostile), new int[] { 20, 40, 60, 80, 120 } },

                { new Tuple<int, int>(ModContent.NPCType<SlimeGodCore>(), ModContent.ProjectileType<UnstableEbonianGlob>()), new int[] { 42, 68, 84, 100, 150 } },
                { new Tuple<int, int>(ModContent.NPCType<SlimeGodCore>(), ModContent.ProjectileType<UnstableCrimulanGlob>()), new int[] { 38, 60, 76, 92, 138 } },
                { new Tuple<int, int>(ModContent.NPCType<EbonianPaladin>(), ModContent.ProjectileType<UnstableEbonianGlob>()), new int[] { 42, 68, 84, 100, 150 } },
                { new Tuple<int, int>(ModContent.NPCType<CrimulanPaladin>(), ModContent.ProjectileType<UnstableCrimulanGlob>()), new int[] { 38, 60, 76, 92, 138 } },
                { new Tuple<int, int>(ModContent.NPCType<SplitEbonianPaladin>(), ModContent.ProjectileType<UnstableEbonianGlob>()), new int[] { 38, 60, 76, 92, 138 } },
                { new Tuple<int, int>(ModContent.NPCType<SplitCrimulanPaladin>(), ModContent.ProjectileType<UnstableCrimulanGlob>()), new int[] { 34, 52, 68, 80, 120 } },
                { new Tuple<int, int>(ModContent.NPCType<CorruptSlimeSpawn>(), ModContent.ProjectileType<ShadeNimbusHostile>()), new int[] { 36, 56, 68, 80, 120 } },
                { new Tuple<int, int>(ModContent.NPCType<CrimsonSlimeSpawn2>(), ModContent.ProjectileType<CrimsonSpike>()), new int[] { 24, 48, 60, 72, 108 } },

                { new Tuple<int, int>(NPCID.WallofFleshEye, ProjectileID.EyeLaser), new int[] { 36, 72, 88, 104, 156 } },
                { new Tuple<int, int>(NPCID.WallofFlesh, ProjectileID.DemonSickle), new int[] { 52, 92, 112, 132, 198 } },
                { new Tuple<int, int>(NPCID.WallofFlesh, ProjectileID.Fireball), new int[] { 36, 72, 88, 104, 156 } },

                { new Tuple<int, int>(NPCID.QueenSlimeBoss, ProjectileID.QueenSlimeGelAttack), new int[] { 60, 120, 136, 152, 228 } },
                { new Tuple<int, int>(NPCID.QueenSlimeBoss, ProjectileID.QueenSlimeSmash), new int[] { 80, 160, 188, 216, 324 } },
                { new Tuple<int, int>(NPCID.QueenSlimeBoss, ProjectileID.QueenSlimeMinionBlueSpike), new int[] { 52, 92, 112, 132, 198 } },
                { new Tuple<int, int>(NPCID.QueenSlimeMinionBlue, ProjectileID.QueenSlimeMinionBlueSpike), new int[] { 52, 92, 112, 132, 198 } },
                { new Tuple<int, int>(NPCID.QueenSlimeMinionPink, ProjectileID.QueenSlimeMinionPinkBall), new int[] { 52, 92, 112, 132, 198 } },

                { new Tuple<int, int>(ModContent.NPCType<Cryogen>(), ModContent.ProjectileType<IceBlast>()), new int[] { 52, 92, 112, 132, 198 } },
                { new Tuple<int, int>(ModContent.NPCType<Cryogen>(), ModContent.ProjectileType<IceBomb>()), new int[] { 70, 120, 136, 152, 228 } },
                { new Tuple<int, int>(ModContent.NPCType<Cryogen>(), ModContent.ProjectileType<IceRain>()), new int[] { 60, 100, 120, 140, 210 } },
                { new Tuple<int, int>(ModContent.NPCType<CryogenShield>(), ModContent.ProjectileType<IceBlast>()), new int[] { 52, 92, 112, 132, 198 } },

                { new Tuple<int, int>(NPCID.Retinazer, ProjectileID.EyeLaser), new int[] { 40, 92, 116, 140, 210 } },
                { new Tuple<int, int>(NPCID.Retinazer, ProjectileID.DeathLaser), new int[] {
                    50, // 38 in rapid fire
                    108, // 81 in rapid fire
                    132, // 99 in rapid fire
                    156, // 117 in rapid fire
                    234 } }, // 176 in rapid fire
                { new Tuple<int, int>(NPCID.Retinazer, ModContent.ProjectileType<ScavengerLaser>()), new int[] { 70, 120, 136, 152, 228 } },
                { new Tuple<int, int>(NPCID.Spazmatism, ProjectileID.CursedFlameHostile), new int[] { 50, 100, 120, 140, 210 } },
                { new Tuple<int, int>(NPCID.Spazmatism, ProjectileID.EyeFire), new int[] { 60, 120, 148, 176, 264 } },
                { new Tuple<int, int>(NPCID.Spazmatism, ModContent.ProjectileType<Shadowflamethrower>()), new int[] { 70, 120, 148, 176, 264 } },
                { new Tuple<int, int>(NPCID.Spazmatism, ModContent.ProjectileType<ShadowflameFireball>()), new int[] { 60, 100, 128, 156, 234 } },

                { new Tuple<int, int>(ModContent.NPCType<AquaticScourgeHead>(), ModContent.ProjectileType<SulphuricAcidMist>()), new int[] { 60, 100, 120, 140, 210 } },
                { new Tuple<int, int>(ModContent.NPCType<AquaticScourgeHead>(), ModContent.ProjectileType<SandPoisonCloud>()), new int[] { 70, 120, 136, 152, 228 } },
                { new Tuple<int, int>(ModContent.NPCType<AquaticScourgeHead>(), ModContent.ProjectileType<ToxicCloud>()), new int[] { 80, 140, 160, 180, 270 } },
                { new Tuple<int, int>(ModContent.NPCType<AquaticScourgeBody>(), ModContent.ProjectileType<SandTooth>()), new int[] { 66, 112, 128, 144, 216 } },

                { new Tuple<int, int>(NPCID.TheDestroyer, ProjectileID.DeathLaser), new int[] { 44, 96, 116, 136, 204 } },
                { new Tuple<int, int>(NPCID.TheDestroyer, ModContent.ProjectileType<DestroyerCursedLaser>()), new int[] { 46, 108, 128, 148, 222 } },
                { new Tuple<int, int>(NPCID.TheDestroyer, ModContent.ProjectileType<DestroyerElectricLaser>()), new int[] { 48, 116, 136, 156, 234 } },
                { new Tuple<int, int>(NPCID.TheDestroyerBody, ProjectileID.DeathLaser), new int[] { 44, 96, 116, 136, 204 } },
                { new Tuple<int, int>(NPCID.TheDestroyerBody, ModContent.ProjectileType<DestroyerCursedLaser>()), new int[] { 46, 108, 128, 148, 222 } },
                { new Tuple<int, int>(NPCID.TheDestroyerBody, ModContent.ProjectileType<DestroyerElectricLaser>()), new int[] { 48, 116, 136, 156, 234 } },
                { new Tuple<int, int>(NPCID.Probe, ProjectileID.PinkLaser), new int[] { 50, 88, 100, 112, 168 } },

                { new Tuple<int, int>(ModContent.NPCType<BrimstoneElemental>(), ModContent.ProjectileType<BrimstoneHellfireball>()), new int[] { 80, 140, 160, 180, 270 } },
                { new Tuple<int, int>(ModContent.NPCType<BrimstoneElemental>(), ModContent.ProjectileType<BrimstoneBarrage>()), new int[] { 70, 112, 128, 144, 216 } },
                { new Tuple<int, int>(ModContent.NPCType<BrimstoneElemental>(), ModContent.ProjectileType<BrimstoneHellblast>()), new int[] { 80, 140, 160, 180, 270 } },
                { new Tuple<int, int>(ModContent.NPCType<BrimstoneElemental>(), ModContent.ProjectileType<BrimstoneRay>()), new int[] { 120, 200, 240, 280, 420 } }, // Split shots: 72, 120, 144, 168, 252
                { new Tuple<int, int>(ModContent.NPCType<Brimling>(), ModContent.ProjectileType<BrimstoneHellfireball>()), new int[] { 80, 140, 160, 180, 270 } },
                { new Tuple<int, int>(ModContent.NPCType<Brimling>(), ModContent.ProjectileType<BrimstoneBarrage>()), new int[] { 70, 112, 128, 144, 216 } },

                { new Tuple<int, int>(NPCID.SkeletronPrime, ProjectileID.Skull), new int[] { 50, 108, 124, 140, 210 } },
                { new Tuple<int, int>(NPCID.SkeletronPrime, ProjectileID.DeathLaser), new int[] { 50, 108, 124, 140, 210 } },
                { new Tuple<int, int>(NPCID.SkeletronPrime, ProjectileID.RocketSkeleton), new int[] { 60, 120, 148, 176, 264 } },
                { new Tuple<int, int>(NPCID.PrimeCannon, ProjectileID.RocketSkeleton), new int[] { 60, 120, 148, 176, 264 } },
                { new Tuple<int, int>(NPCID.PrimeCannon, ProjectileID.BombSkeletronPrime), new int[] { 80, 160, 0, 0, 300 } },
                { new Tuple<int, int>(NPCID.PrimeLaser, ProjectileID.DeathLaser), new int[] { 50, 108, 124, 140, 210 } },

                { new Tuple<int, int>(ModContent.NPCType<CalamitasClone>(), ModContent.ProjectileType<BrimstoneHellblast>()), new int[] { 80, 140, 160, 180, 270 } },
                { new Tuple<int, int>(ModContent.NPCType<CalamitasClone>(), ModContent.ProjectileType<BrimstoneHellfireball>()), new int[] { 80, 140, 160, 180, 270 } },
                { new Tuple<int, int>(ModContent.NPCType<CalamitasClone>(), ModContent.ProjectileType<BrimstoneHellblast2>()), new int[] { 80, 140, 160, 180, 270 } },
                { new Tuple<int, int>(ModContent.NPCType<CalamitasClone>(), ModContent.ProjectileType<SCalBrimstoneFireblast>()), new int[] { 80, 140, 160, 180, 270 } },
                { new Tuple<int, int>(ModContent.NPCType<CalamitasClone>(), ModContent.ProjectileType<BrimstoneBarrage>()), new int[] { 70, 112, 128, 144, 216 } },
                { new Tuple<int, int>(ModContent.NPCType<Cataclysm>(), ModContent.ProjectileType<BrimstoneFire>()), new int[] { 80, 140, 160, 180, 270 } },
                { new Tuple<int, int>(ModContent.NPCType<Catastrophe>(), ModContent.ProjectileType<BrimstoneBall>()), new int[] { 70, 112, 128, 144, 216 } },
                { new Tuple<int, int>(ModContent.NPCType<SoulSeeker>(), ModContent.ProjectileType<BrimstoneBarrage>()), new int[] { 70, 112, 128, 144, 216 } },

                { new Tuple<int, int>(NPCID.Plantera, ProjectileID.SeedPlantera), new int[] { 52, 96, 128, 160, 240 } },
                { new Tuple<int, int>(NPCID.Plantera, ProjectileID.PoisonSeedPlantera), new int[] { 62, 116, 144, 172, 258 } },
                { new Tuple<int, int>(NPCID.Plantera, ProjectileID.ThornBall), new int[] { 72, 132, 160, 188, 282 } },
                { new Tuple<int, int>(NPCID.Plantera, ModContent.ProjectileType<SporeGasPlantera>()), new int[] { 80, 140, 160, 180, 270 } },
                { new Tuple<int, int>(NPCID.Plantera, ModContent.ProjectileType<HomingGasBulb>()), new int[] { 80, 140, 160, 180, 270 } },

                { new Tuple<int, int>(NPCID.HallowBoss, ProjectileID.HallowBossRainbowStreak), new int[] { 100, 152, 172, 192, 300 } },
                { new Tuple<int, int>(NPCID.HallowBoss, ProjectileID.FairyQueenSunDance), new int[] { 120, 180, 204, 228, 360 } },
                { new Tuple<int, int>(NPCID.HallowBoss, ProjectileID.HallowBossLastingRainbow), new int[] { 100, 152, 172, 192, 300 } },
                { new Tuple<int, int>(NPCID.HallowBoss, ProjectileID.FairyQueenLance), new int[] { 130, 196, 216, 236, 390 } },

                { new Tuple<int, int>(ModContent.NPCType<Leviathan>(), ModContent.ProjectileType<LeviathanBomb>()), new int[] { 100, 172, 208, 244, 366 } },
                { new Tuple<int, int>(ModContent.NPCType<Anahita>(), ModContent.ProjectileType<WaterSpear>()), new int[] { 80, 140, 160, 180, 270 } },
                { new Tuple<int, int>(ModContent.NPCType<Anahita>(), ModContent.ProjectileType<FrostMist>()), new int[] { 84, 148, 172, 196, 294 } },
                { new Tuple<int, int>(ModContent.NPCType<Anahita>(), ModContent.ProjectileType<SirenSong>()), new int[] { 88, 156, 184, 212, 318 } },

                { new Tuple<int, int>(ModContent.NPCType<AstrumAureus>(), ModContent.ProjectileType<AstralLaser>()), new int[] { 88, 156, 184, 212, 318 } },
                { new Tuple<int, int>(ModContent.NPCType<AstrumAureus>(), ModContent.ProjectileType<AstralFlame>()), new int[] { 100, 172, 208, 244, 366 } },
                { new Tuple<int, int>(ModContent.NPCType<AureusSpawn>(), ModContent.ProjectileType<AstralLaser>()), new int[] { 88, 156, 184, 212, 318 } },

                { new Tuple<int, int>(NPCID.Golem, ProjectileID.Fireball), new int[] { 72, 132, 160, 188, 282 } },
                { new Tuple<int, int>(NPCID.Golem, ProjectileID.EyeBeam), new int[] { 80, 140, 172, 204, 306 } },
                { new Tuple<int, int>(NPCID.GolemHead, ProjectileID.Fireball), new int[] { 60, 120, 160, 200, 300 } },
                { new Tuple<int, int>(NPCID.GolemHead, ProjectileID.EyeBeam), new int[] { 80, 140, 172, 204, 306 } },
                { new Tuple<int, int>(NPCID.GolemFistLeft, ProjectileID.InfernoHostileBolt), new int[] { 88, 156, 184, 212, 318 } },
                { new Tuple<int, int>(NPCID.GolemFistRight, ProjectileID.InfernoHostileBolt), new int[] { 88, 156, 184, 212, 318 } },
                { new Tuple<int, int>(NPCID.GolemHeadFree, ProjectileID.Fireball), new int[] { 60, 120, 160, 200, 300 } },
                { new Tuple<int, int>(NPCID.GolemHeadFree, ProjectileID.EyeBeam), new int[] { 80, 140, 172, 204, 306 } },
                { new Tuple<int, int>(NPCID.GolemHeadFree, ProjectileID.InfernoHostileBolt), new int[] { 88, 156, 184, 212, 318 } },

                { new Tuple<int, int>(ModContent.NPCType<PlaguebringerGoliath>(), ModContent.ProjectileType<PlagueStingerGoliath>()), new int[] { 88, 156, 184, 212, 318 } },
                { new Tuple<int, int>(ModContent.NPCType<PlaguebringerGoliath>(), ModContent.ProjectileType<PlagueStingerGoliathV2>()), new int[] { 88, 156, 184, 212, 318 } },
                { new Tuple<int, int>(ModContent.NPCType<PlaguebringerGoliath>(), ModContent.ProjectileType<HiveBombGoliath>()), new int[] { 120, 192, 220, 248, 372 } },

                { new Tuple<int, int>(NPCID.DukeFishron, ProjectileID.Sharknado), new int[] { 80, 100, 150, 200, 300 } },
                { new Tuple<int, int>(NPCID.DukeFishron, ProjectileID.Cthulunado), new int[] { 160, 200, 232, 264, 480 } },

                { new Tuple<int, int>(ModContent.NPCType<RavagerBody>(), ModContent.ProjectileType<RavagerBlaster>()), new int[] { 120, 180, 208, 236, 360 } },
                { new Tuple<int, int>(ModContent.NPCType<RavagerHead>(), ModContent.ProjectileType<ScavengerNuke>()), new int[] { 120, 180, 208, 236, 360 } },
                { new Tuple<int, int>(ModContent.NPCType<RavagerHead2>(), ModContent.ProjectileType<ScavengerLaser>()), new int[] { 90, 144, 172, 200, 300 } },
                { new Tuple<int, int>(ModContent.NPCType<RavagerHead2>(), ModContent.ProjectileType<ScavengerNuke>()), new int[] { 120, 180, 208, 236, 360 } },
                { new Tuple<int, int>(ModContent.NPCType<FlamePillar>(), ModContent.ProjectileType<RavagerFlame>()), new int[] { 90, 144, 172, 200, 300 } },

                { new Tuple<int, int>(NPCID.CultistBoss, ProjectileID.CultistBossFireBall), new int[] { 88, 156, 184, 212, 318 } },
                { new Tuple<int, int>(NPCID.CultistBoss, ProjectileID.CultistBossIceMist), new int[] { 120, 180, 208, 236, 360 } },
                { new Tuple<int, int>(NPCID.CultistBoss, ProjectileID.CultistBossLightningOrb), new int[] { 120, 192, 220, 248, 372 } },
                { new Tuple<int, int>(NPCID.CultistBossClone, ProjectileID.CultistBossFireBallClone), new int[] { 90, 144, 172, 200, 300 } },
                { new Tuple<int, int>(NPCID.AncientDoom, ProjectileID.AncientDoomProjectile), new int[] { 88, 156, 184, 212, 318 } },

                { new Tuple<int, int>(ModContent.NPCType<AstrumDeusBody>(), ModContent.ProjectileType<AstralShot2>()), new int[] { 90, 152, 176, 200, 300 } },
                { new Tuple<int, int>(ModContent.NPCType<AstrumDeusBody>(), ModContent.ProjectileType<DeusMine>()), new int[] { 120, 180, 208, 236, 360 } },
                { new Tuple<int, int>(ModContent.NPCType<AstrumDeusBody>(), ModContent.ProjectileType<AstralGodRay>()), new int[] { 100, 172, 192, 212, 318 } },

                { new Tuple<int, int>(NPCID.MoonLordHead, ProjectileID.PhantasmalDeathray), new int[] { 200, 300, 380, 460, 690 } },
                { new Tuple<int, int>(NPCID.MoonLordHead, ProjectileID.PhantasmalBolt), new int[] { 90, 120, 160, 200, 300 } },
                { new Tuple<int, int>(NPCID.MoonLordHand, ProjectileID.PhantasmalEye), new int[] { 90, 120, 160, 200, 300 } },
                { new Tuple<int, int>(NPCID.MoonLordHand, ProjectileID.PhantasmalSphere), new int[] { 120, 160, 220, 280, 420 } },
                { new Tuple<int, int>(NPCID.MoonLordHand, ProjectileID.PhantasmalBolt), new int[] { 90, 120, 160, 200, 300 } },
                { new Tuple<int, int>(NPCID.MoonLordFreeEye, ProjectileID.PhantasmalBolt), new int[] { 100, 140, 180, 220, 330 } },
                { new Tuple<int, int>(NPCID.MoonLordFreeEye, ProjectileID.PhantasmalEye), new int[] { 100, 140, 180, 220, 330 } },
                { new Tuple<int, int>(NPCID.MoonLordFreeEye, ProjectileID.PhantasmalSphere), new int[] { 132, 176, 220, 264, 396 } },
                { new Tuple<int, int>(NPCID.MoonLordFreeEye, ProjectileID.PhantasmalDeathray), new int[] { 150, 200, 260, 320, 480 } },

                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianCommander>(), ModContent.ProjectileType<ProfanedSpear>()), new int[] { 140, 220, 244, 268, 420 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianCommander>(), ModContent.ProjectileType<HolySpear>()), new int[] { 140, 220, 244, 268, 420 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianCommander>(), ModContent.ProjectileType<HolyBlast>()), new int[] { 140, 220, 244, 268, 420 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianCommander>(), ModContent.ProjectileType<HolyFire>()), new int[] { 120, 192, 220, 248, 360 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianCommander>(), ModContent.ProjectileType<HolyFire2>()), new int[] { 120, 192, 220, 248, 360 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianCommander>(), ModContent.ProjectileType<ProvidenceHolyRay>()), new int[] { 160, 320, 352, 384, 480 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianDefender>(), ModContent.ProjectileType<HolyBomb>()), new int[] { 140, 220, 244, 268, 420 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianDefender>(), ModContent.ProjectileType<HolyFlare>()), new int[] { 105, 171, 189, 207, 315 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianDefender>(), ModContent.ProjectileType<MoltenBlast>()), new int[] { 140, 228, 252, 276, 420 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianDefender>(), ModContent.ProjectileType<MoltenBlob>()), new int[] { 105, 171, 189, 207, 315 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianHealer>(), ModContent.ProjectileType<ProvidenceCrystalShard>()), new int[] { 140, 220, 244, 268, 420 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianHealer>(), ModContent.ProjectileType<HolyBurnOrb>()), new int[] { 120, 192, 220, 248, 360 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianHealer>(), ModContent.ProjectileType<HolyLight>()), new int[] { 35, 50, 50, 0, 0 } },

                { new Tuple<int, int>(ModContent.NPCType<Bumblefuck>(), ModContent.ProjectileType<RedLightningFeather>()), new int[] { 140, 220, 244, 268, 420 } },
                { new Tuple<int, int>(ModContent.NPCType<Bumblefuck>(), ModContent.ProjectileType<BirbAuraFlare>()), new int[] { 200, 300, 332, 364, 600 } },

                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<HolyBlast>()), new int[] { 150, 264, 288, 312, 450 } }, // Split holy fire does: 113, 198, 216, 234, 338
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<HolyFire>()), new int[] { 120, 192, 220, 248, 360 } },
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<HolyFire2>()), new int[] { 120, 192, 220, 248, 360 } },
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<HolyBurnOrb>()), new int[] { 140, 220, 244, 268, 420 } },
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<HolyLight>()), new int[] { 35, 50, 50, 0, 0 } },
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<MoltenBlast>()), new int[] { 140, 228, 252, 276, 420 } },
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<MoltenBlob>()), new int[] { 105, 171, 189, 207, 315 } },
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<HolyBomb>()), new int[] { 140, 228, 252, 276, 420 } },
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<HolyFlare>()), new int[] { 105, 171, 189, 207, 315 } },
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<HolySpear>()), new int[] { 140, 220, 244, 268, 420 } },
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<ProvidenceCrystal>()), new int[] { 140, 228, 252, 276, 420 } },
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<ProvidenceCrystalShard>()), new int[] { 140, 228, 252, 276, 420 } },
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<ProvidenceHolyRay>()), new int[] { 200, 400, 440, 480, 600 } },

                { new Tuple<int, int>(ModContent.NPCType<CeaselessVoid>(), ModContent.ProjectileType<DoGBeamPortal>()), new int[] { 140, 240, 264, 288, 420 } },
                { new Tuple<int, int>(ModContent.NPCType<CeaselessVoid>(), ModContent.ProjectileType<DarkEnergyBall>()), new int[] { 140, 240, 264, 288, 420 } },
                { new Tuple<int, int>(ModContent.NPCType<CeaselessVoid>(), ModContent.ProjectileType<DarkEnergyBall2>()), new int[] { 140, 240, 264, 288, 420 } },

                { new Tuple<int, int>(ModContent.NPCType<StormWeaverHead>(), ProjectileID.CultistBossLightningOrbArc), new int[] { 150, 264, 288, 312, 450 } },
                { new Tuple<int, int>(ModContent.NPCType<StormWeaverHead>(), ProjectileID.FrostWave), new int[] { 150, 264, 288, 312, 450 } },
                { new Tuple<int, int>(ModContent.NPCType<StormWeaverHead>(), ModContent.ProjectileType<StormMarkHostile>()), new int[] { 160, 276, 304, 332, 480 } },
                { new Tuple<int, int>(ModContent.NPCType<StormWeaverBody>(), ModContent.ProjectileType<DestroyerElectricLaser>()), new int[] { 140, 240, 264, 288, 420 } },
                { new Tuple<int, int>(ModContent.NPCType<StormWeaverTail>(), ProjectileID.CultistBossLightningOrb), new int[] { 150, 264, 288, 312, 450 } },

                { new Tuple<int, int>(ModContent.NPCType<Signus>(), ModContent.ProjectileType<SignusScythe>()), new int[] { 140, 240, 264, 288, 420 } },
                { new Tuple<int, int>(ModContent.NPCType<Signus>(), ModContent.ProjectileType<EssenceDust>()), new int[] { 140, 240, 264, 288, 420 } },

                { new Tuple<int, int>(ModContent.NPCType<Polterghast>(), ModContent.ProjectileType<PhantomShot>()), new int[] { 140, 240, 264, 288, 420 } },
                { new Tuple<int, int>(ModContent.NPCType<Polterghast>(), ModContent.ProjectileType<PhantomShot2>()), new int[] { 150, 264, 288, 312, 450 } },
                { new Tuple<int, int>(ModContent.NPCType<Polterghast>(), ModContent.ProjectileType<PhantomBlast>()), new int[] { 150, 264, 288, 312, 450 } },
                { new Tuple<int, int>(ModContent.NPCType<Polterghast>(), ModContent.ProjectileType<PhantomBlast2>()), new int[] { 160, 276, 304, 332, 480 } },
                { new Tuple<int, int>(ModContent.NPCType<PolterghastHook>(), ModContent.ProjectileType<PhantomHookShot>()), new int[] { 140, 240, 264, 288, 420 } },
                { new Tuple<int, int>(ModContent.NPCType<PhantomFuckYou>(), ModContent.ProjectileType<PhantomMine>()), new int[] { 170, 296, 324, 352, 510 } },
                { new Tuple<int, int>(ModContent.NPCType<PhantomSpiritL>(), ModContent.ProjectileType<PhantomGhostShot>()), new int[] { 150, 264, 288, 312, 450 } },

                { new Tuple<int, int>(ModContent.NPCType<Mauler>(), ModContent.ProjectileType<MaulerAcidBubble>()), new int[] { 140, 220, 244, 264, 420 } },
                { new Tuple<int, int>(ModContent.NPCType<Mauler>(), ModContent.ProjectileType<MaulerAcidDrop>()), new int[] { 140, 220, 244, 264, 420 } },

                { new Tuple<int, int>(ModContent.NPCType<OldDuke>(), ModContent.ProjectileType<OldDukeGore>()), new int[] { 170, 296, 324, 352, 510 } },
                { new Tuple<int, int>(ModContent.NPCType<OldDuke>(), ModContent.ProjectileType<OldDukeVortex>()), new int[] { 280, 420, 460, 500, 840 } },
                { new Tuple<int, int>(ModContent.NPCType<OldDukeToothBall>(), ModContent.ProjectileType<OldDukeToothBallSpike>()), new int[] { 170, 296, 324, 352, 510 } },
                { new Tuple<int, int>(ModContent.NPCType<OldDukeToothBall>(), ModContent.ProjectileType<SandPoisonCloudOldDuke>()), new int[] { 180, 316, 348, 380, 540 } },
                { new Tuple<int, int>(ModContent.NPCType<SulphurousSharkron>(), ModContent.ProjectileType<OldDukeGore>()), new int[] { 170, 296, 324, 352, 510 } },

                { new Tuple<int, int>(ModContent.NPCType<DevourerofGodsHead>(), ModContent.ProjectileType<DoGDeath>()), new int[] { 180, 316, 348, 380, 540 } },
                { new Tuple<int, int>(ModContent.NPCType<DevourerofGodsHead>(), ModContent.ProjectileType<DoGFire>()), new int[] { 200, 340, 376, 412, 600 } },
                { new Tuple<int, int>(ModContent.NPCType<DevourerofGodsBody>(), ModContent.ProjectileType<DoGDeath>()), new int[] { 180, 316, 348, 380, 540 } },

                { new Tuple<int, int>(ModContent.NPCType<Yharon>(), ModContent.ProjectileType<SkyFlareRevenge>()), new int[] { 300, 520, 548, 576, 900 } },
                { new Tuple<int, int>(ModContent.NPCType<Yharon>(), ModContent.ProjectileType<FlareBomb>()), new int[] { 220, 384, 424, 464, 660 } },
                { new Tuple<int, int>(ModContent.NPCType<Yharon>(), ModContent.ProjectileType<Flarenado>()), new int[] { 250, 440, 464, 488, 750 } },
                { new Tuple<int, int>(ModContent.NPCType<Yharon>(), ModContent.ProjectileType<Infernado>()), new int[] { 250, 440, 464, 488, 750 } },
                { new Tuple<int, int>(ModContent.NPCType<Yharon>(), ModContent.ProjectileType<Infernado2>()), new int[] { 250, 440, 464, 488, 750 } },
                { new Tuple<int, int>(ModContent.NPCType<Yharon>(), ModContent.ProjectileType<FlareDust>()), new int[] { 220, 384, 424, 464, 660 } },
                { new Tuple<int, int>(ModContent.NPCType<Yharon>(), ModContent.ProjectileType<FlareDust2>()), new int[] { 220, 384, 424, 464, 660 } },
                { new Tuple<int, int>(ModContent.NPCType<Yharon>(), ModContent.ProjectileType<YharonFireball>()), new int[] { 220, 384, 424, 464, 660 } },
                { new Tuple<int, int>(ModContent.NPCType<Yharon>(), ModContent.ProjectileType<YharonBulletHellVortex>()), new int[] { 250, 440, 464, 488, 750 } },

                { new Tuple<int, int>(ModContent.NPCType<SupremeCalamitas>(), ModContent.ProjectileType<BrimstoneHellblast2>()), new int[] { 280, 468, 484, 500, 840 } },
                { new Tuple<int, int>(ModContent.NPCType<SupremeCalamitas>(), ModContent.ProjectileType<SCalBrimstoneFireblast>()), new int[] { 280, 468, 484, 500, 840 } },
                { new Tuple<int, int>(ModContent.NPCType<SupremeCalamitas>(), ModContent.ProjectileType<SCalBrimstoneGigablast>()), new int[] { 300, 508, 528, 548, 900 } },
                { new Tuple<int, int>(ModContent.NPCType<SupremeCalamitas>(), ModContent.ProjectileType<BrimstoneMonster>()), new int[] { 350, 592, 624, 656, 1050 } },
                { new Tuple<int, int>(ModContent.NPCType<SupremeCalamitas>(), ModContent.ProjectileType<BrimstoneWave>()), new int[] { 280, 468, 484, 500, 840 } },
                { new Tuple<int, int>(ModContent.NPCType<SupremeCalamitas>(), ModContent.ProjectileType<BrimstoneBarrage>()), new int[] { 250, 440, 464, 488, 750 } },
                { new Tuple<int, int>(ModContent.NPCType<SupremeCalamitas>(), ModContent.ProjectileType<BrimstoneHellblast>()), new int[] { 280, 468, 484, 500, 840 } },
                { new Tuple<int, int>(ModContent.NPCType<SepulcherBodyEnergyBall>(), ModContent.ProjectileType<BrimstoneBarrage>()), new int[] { 250, 440, 464, 488, 750 } },
                { new Tuple<int, int>(ModContent.NPCType<SoulSeekerSupreme>(), ModContent.ProjectileType<BrimstoneBarrage>()), new int[] { 250, 440, 464, 488, 750 } },
                { new Tuple<int, int>(ModContent.NPCType<SupremeCataclysm>(), ModContent.ProjectileType<SupremeCataclysmFist>()), new int[] { 280, 468, 484, 500, 840 } },
                { new Tuple<int, int>(ModContent.NPCType<SupremeCataclysm>(), ModContent.ProjectileType<BrimstoneBarrage>()), new int[] { 250, 440, 464, 488, 750 } },
                { new Tuple<int, int>(ModContent.NPCType<SupremeCatastrophe>(), ModContent.ProjectileType<SupremeCatastropheSlash>()), new int[] { 280, 468, 484, 500, 840 } },
                { new Tuple<int, int>(ModContent.NPCType<SupremeCatastrophe>(), ModContent.ProjectileType<BrimstoneBarrage>()), new int[] { 250, 440, 464, 488, 750 } },

                { new Tuple<int, int>(ModContent.NPCType<Artemis>(), ModContent.ProjectileType<ArtemisSpinLaserbeam>()), new int[] { 300, 508, 528, 548, 900 } },
                { new Tuple<int, int>(ModContent.NPCType<Artemis>(), ModContent.ProjectileType<ArtemisLaser>()), new int[] { 240, 408, 432, 456, 720 } },
                { new Tuple<int, int>(ModContent.NPCType<Apollo>(), ModContent.ProjectileType<ApolloFireball>()), new int[] { 240, 408, 432, 456, 720 } },
                { new Tuple<int, int>(ModContent.NPCType<Apollo>(), ModContent.ProjectileType<ApolloRocket>()), new int[] { 280, 468, 484, 500, 840 } },

                { new Tuple<int, int>(ModContent.NPCType<ThanatosHead>(), ModContent.ProjectileType<ThanatosBeamStart>()), new int[] { 350, 592, 624, 656, 1050 } },
                { new Tuple<int, int>(ModContent.NPCType<ThanatosHead>(), ModContent.ProjectileType<ThanatosLaser>()), new int[] { 240, 408, 432, 456, 720 } },
                { new Tuple<int, int>(ModContent.NPCType<ThanatosBody1>(), ModContent.ProjectileType<ThanatosLaser>()), new int[] { 240, 408, 432, 456, 720 } },
                { new Tuple<int, int>(ModContent.NPCType<ThanatosBody2>(), ModContent.ProjectileType<ThanatosLaser>()), new int[] { 240, 408, 432, 456, 720 } },
                { new Tuple<int, int>(ModContent.NPCType<ThanatosTail>(), ModContent.ProjectileType<ThanatosLaser>()), new int[] { 240, 408, 432, 456, 720 } },

                { new Tuple<int, int>(ModContent.NPCType<AresBody>(), ModContent.ProjectileType<AresDeathBeamStart>()), new int[] { 300, 508, 528, 548, 900 } },
                { new Tuple<int, int>(ModContent.NPCType<AresLaserCannon>(), ModContent.ProjectileType<AresLaserBeamStart>()), new int[] { 300, 508, 528, 548, 900 } },
                { new Tuple<int, int>(ModContent.NPCType<AresLaserCannon>(), ModContent.ProjectileType<ThanatosLaser>()), new int[] { 240, 408, 432, 456, 720 } },
                { new Tuple<int, int>(ModContent.NPCType<AresPlasmaFlamethrower>(), ModContent.ProjectileType<AresPlasmaFireball>()), new int[] { 240, 408, 432, 456, 720 } },
                { new Tuple<int, int>(ModContent.NPCType<AresTeslaCannon>(), ModContent.ProjectileType<AresTeslaOrb>()), new int[] { 240, 408, 432, 456, 720 } },
                { new Tuple<int, int>(ModContent.NPCType<AresGaussNuke>(), ModContent.ProjectileType<AresGaussNukeProjectile>()), new int[] { 400, 608, 640, 672, 1200 } },

                { new Tuple<int, int>(ModContent.NPCType<PrimordialWyrmHead>(), ProjectileID.CultistBossIceMist), new int[] { 400, 600, 632, 664, 1200 } },
                { new Tuple<int, int>(ModContent.NPCType<PrimordialWyrmHead>(), ProjectileID.CultistBossLightningOrbArc), new int[] { 500, 752, 788, 824, 1500 } },
                { new Tuple<int, int>(ModContent.NPCType<PrimordialWyrmHead>(), ProjectileID.AncientDoomProjectile), new int[] { 400, 600, 632, 664, 1200 } },
                { new Tuple<int, int>(ModContent.NPCType<PrimordialWyrmBodyAlt>(), ProjectileID.CultistBossFireBallClone), new int[] { 400, 600, 632, 664, 1200 } }
            };
        }

        // Destroys the EnemyStats struct to save memory because mod assemblies will not be fully unloaded until TML 1.4.
        internal static void UnloadEnemyStats()
        {
            EnemyStats.ExpertDamageMultiplier = null;
            EnemyStats.ContactDamageValues = null;
            EnemyStats.ProjectileDamageValues = null;
        }
        #endregion
    }
}
