using CalamityMod.NPCs.PrimordialWyrm;
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
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Thanatos;
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
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.NPCs.AcidRain;

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

                { NPCID.Deerclops, 1 },

                { ModContent.NPCType<HiveMind>(), 0.9 },

                { ModContent.NPCType<PerforatorHive>(), 0.9 },

                { NPCID.QueenBee, 0.9 },
                { NPCID.Bee, 0.6 },
                { NPCID.BeeSmall, 0.6 },

                { NPCID.SkeletronHead, 1.1 },
                { NPCID.SkeletronHand, 1.1 },

                { NPCID.WallofFlesh, 1.5 },
                { NPCID.WallofFleshEye, 1.5 },

                { NPCID.QueenSlimeBoss, 1 },

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
                { NPCID.KingSlime, new int[] { 40, 64, 80, 88, 96 } },

                { ModContent.NPCType<DesertScourgeHead>(), new int[] { 30, 66, 88, 99, 132 } },
                { ModContent.NPCType<DesertScourgeBody>(), new int[] { 16, 32, 40, 44, 60 } },
                { ModContent.NPCType<DesertScourgeTail>(), new int[] { 12, 24, 30, 32, 42 } },
                { ModContent.NPCType<DesertNuisanceHead>(), new int[] { 20, 44, 60, 70, 90 } },
                { ModContent.NPCType<DesertNuisanceBody>(), new int[] { 14, 28, 32, 36, 45 } },
                { ModContent.NPCType<DesertNuisanceTail>(), new int[] { 8, 16, 20, 24, 30 } },

                { NPCID.EyeofCthulhu, new int[] {
                    15, // 23 in phase 2
                    30, // 36 in phase 2, 40 in phase 3
                    40, // 48 in phase 2, 56 in phase 3
                    60, // Same for all phases
                    51 } }, // Vanilla: 54 in phase 2, 60 in phase 3; Rev: 61 in phase 2, 71 in phase 3; Death: 71 at all times
                { NPCID.ServantofCthulhu, new int[] { 12, 24, 30, 34, 42 } },

                { ModContent.NPCType<Crabulon>(), new int[] { 40, 64, 80, 88, 120 } },
                { ModContent.NPCType<CrabShroom>(), new int[] { 25, 50, 62, 70, 75 } },

                { NPCID.EaterofWorldsHead, new int[] { 25, 55, 77, 88, 132 } },
                { NPCID.EaterofWorldsBody, new int[] { 15, 24, 32, 40, 60 } },
                { NPCID.EaterofWorldsTail, new int[] { 10, 16, 24, 32, 42 } },
                { NPCID.VileSpitEaterOfWorlds, new int[] { -1, 64, 64, 64, 96 } },

                { NPCID.BrainofCthulhu, new int[] { 30, 54, 81, 99, 135 } },
                { NPCID.Creeper, new int[] { 20, 36, 54, 72, 90 } },

                { ModContent.NPCType<HiveMind>(), new int[] { 35, 63, 81, 90, 135 } },
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
                { NPCID.ChaosBall, new int[] { -1, -1, -1, 40, 60 } },

                { NPCID.Deerclops, new int[] { 20, 40, 50, 60, 80 } },

                { ModContent.NPCType<SlimeGodCore>(), new int[] { 40, 80, 96, 104, 135 } },
                { ModContent.NPCType<EbonianPaladin>(), new int[] { 45, 90, 108, 118, 150 } },
                { ModContent.NPCType<SplitEbonianPaladin>(), new int[] { 40, 80, 96, 104, 135 } },
                { ModContent.NPCType<CrimulanPaladin>(), new int[] { 50, 100, 120, 130, 171 } },
                { ModContent.NPCType<SplitCrimulanPaladin>(), new int[] { 45, 90, 108, 118, 150 } },
                { ModContent.NPCType<CorruptSlimeSpawn>(), new int[] { 30, 60, 72, 78, 99 } },
                { ModContent.NPCType<CorruptSlimeSpawn2>(), new int[] { 20, 40, 48, 52, 66 } },
                { ModContent.NPCType<CrimsonSlimeSpawn>(), new int[] { 35, 70, 84, 92, 120 } },
                { ModContent.NPCType<CrimsonSlimeSpawn2>(), new int[] { 25, 50, 60, 66, 84 } },

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

                { NPCID.QueenSlimeBoss, new int[] { 60, 120, 150, 170, 240 } },

                { ModContent.NPCType<Cryogen>(), new int[] { 60, 138, 161, 184, 276 } },
                { ModContent.NPCType<CryogenShield>(), new int[] { 60, 120, 138, 158, 216 } },

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
                { ModContent.NPCType<AquaticScourgeBody>(), new int[] { 70, 112, 136, 144, 180 } },
                { ModContent.NPCType<AquaticScourgeBodyAlt>(), new int[] { 65, 104, 112, 136, 168 } },
                { ModContent.NPCType<AquaticScourgeTail>(), new int[] { 60, 96, 104, 112, 156 } },

                { NPCID.TheDestroyer, new int[] { 70, 280, 300, 320, 420 } },
                { NPCID.TheDestroyerBody, new int[] { 60, 102, 136, 153, 204 } },
                { NPCID.TheDestroyerTail, new int[] { 40, 68, 102, 136, 153 } },

                { ModContent.NPCType<BrimstoneElemental>(), new int[] { 70, 112, 136, 144, 180 } },

                { NPCID.SkeletronPrime, new int[] {
                    50, // 100 while spinning
                    85, // 170 while spinning
                    102, // 204 while spinning
                    119, // 238 while spinning
                    153 } }, // 306 while spinning
                { NPCID.PrimeVice, new int[] { 60, 102, 136, 153, 204 } },
                { NPCID.PrimeSaw, new int[] { 60, 102, 136, 153, 204 } },
                { NPCID.PrimeCannon, new int[] { 30, 51, 68, 85, 102 } },
                { NPCID.PrimeLaser, new int[] { 30, 51, 68, 85, 102 } },

                { ModContent.NPCType<CalamitasClone>(), new int[] { 90, 144, 168, 176, 240 } },
                { ModContent.NPCType<Cataclysm>(), new int[] { 60, 120, 138, 148, 198 } },
                { ModContent.NPCType<Catastrophe>(), new int[] { 65, 130, 150, 162, 216 } },

                { NPCID.Plantera, new int[] {
                    50, // 70 in phase 2, vanilla is retarded and doesn't use the expert multiplier for plantera's damage
                    100, // 140 in phase 2, vanilla is retarded and doesn't use the expert multiplier for plantera's damage
                    138, // 193 in phase 2
                    161, // 225 in phase 2
                    207 } }, // Vanilla: Is retarded, so plantera does 150 in phase 1 and 210 in phase 2; Rev and Death: 289 in phase 2
                { NPCID.PlanterasTentacle, new int[] { 60, 138, 161, 207, 276 } },
                { ModContent.NPCType<PlanterasFreeTentacle>(), new int[] { 60, 138, 161, 207, 276 } },
                { NPCID.Spore, new int[] { 70, 140, 160, 170, 210 } },

                { NPCID.HallowBoss, new int[] { 80, // 120 during charge
                    110, // 165 during charge
                    143, // 214 during charge
                    154, // 231 during charge
                    198 } }, // 297 during charge

                { ModContent.NPCType<Leviathan>(), new int[] { 90, 216, 240, 252, 324 } },
                { ModContent.NPCType<Anahita>(), new int[] {
                    70, // 105 during charge
                    112, // 168 during charge
                    136, // 204 during charge
                    144, // 216 during charge
                    192 } }, // 288 during charge
                { ModContent.NPCType<AnahitasIceShield>(), new int[] { 55, 110, 126, 136, 165 } },
                { NPCID.DetonatingBubble, new int[] { 100, 150, 180, 195, 225 } },
                { ModContent.NPCType<AquaticAberration>(), new int[] { 70, 140, 160, 170, 210 } },

                { ModContent.NPCType<AstrumAureus>(), new int[] { 100, 220, 242, 253, 330 } },
                { ModContent.NPCType<AureusSpawn>(), new int[] { 66, 112, 128, 138, 180 } },

                { NPCID.Golem, new int[] { 90, 144, 176, 192, 240 } },
                { NPCID.GolemHead, new int[] { 80, 128, 144, 160, 192 } },
                { NPCID.GolemFistLeft, new int[] { 70, 112, 144, 160, 180 } },
                { NPCID.GolemFistRight, new int[] { 70, 112, 144, 160, 180 } },

                { ModContent.NPCType<PlaguebringerGoliath>(), new int[] { 100, 180, 216, 234, 297 } },
                { ModContent.NPCType<PlagueHomingMissile>(), new int[] { 90, 180, 210, 224, 270 } },
                { ModContent.NPCType<PlagueMine>(), new int[] { 100, 200, 240, 260, 330 } },

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

                { ModContent.NPCType<AstrumDeusHead>(), new int[] { 120, 240, 268, 280, 360 } },
                { ModContent.NPCType<AstrumDeusBody>(), new int[] { 100, 160, 192, 200, 240 } },
                { ModContent.NPCType<AstrumDeusTail>(), new int[] { 80, 128, 160, 168, 192 } },

                { ModContent.NPCType<ProfanedGuardianCommander>(), new int[] { 140, 224, 256, 280, 336 } },
                { ModContent.NPCType<ProfanedGuardianDefender>(), new int[] { 120, 240, 264, 278, 336 } },
                { ModContent.NPCType<ProfanedGuardianHealer>(), new int[] { 100, 200, 220, 232, 270 } },

                { ModContent.NPCType<Bumblefuck>(), new int[] { 160, 256, 288, 304, 384 } },
                { ModContent.NPCType<Bumblefuck2>(), new int[] { 110, 220, 242, 256, 330 } },

                { ModContent.NPCType<ProvSpawnOffense>(), new int[] { 120, 240, 264, 278, 336 } },
                { ModContent.NPCType<ProvSpawnDefense>(), new int[] { 100, 200, 220, 232, 270 } },
                { ModContent.NPCType<ProfanedRocks>(), new int[] { 100, 200, 220, 232, 270 } },

                { ModContent.NPCType<CeaselessVoid>(), new int[] { 150, 300, 330, 348, 450 } },
                { ModContent.NPCType<DarkEnergy>(), new int[] { 130, 260, 288, 304, 390 } },

                { ModContent.NPCType<StormWeaverHead>(), new int[] { 180, 360, 396, 418, 540 } },
                { ModContent.NPCType<StormWeaverBody>(), new int[] { 120, 192, 224, 250, 330 } },
                { ModContent.NPCType<StormWeaverTail>(), new int[] { 100, 160, 192, 210, 270 } },

                { ModContent.NPCType<Signus>(), new int[] { 175, 315, 351, 369, 459 } },
                { ModContent.NPCType<CosmicLantern>(), new int[] { 130, 260, 288, 304, 390 } },
                { ModContent.NPCType<CosmicMine>(), new int[] { 140, 280, 300, 320, 390 } },

                { ModContent.NPCType<Polterghast>(), new int[] {
                    150, // 180 in phase 2, 210 in phase 3
                    240, // 288 in phase 2, 336 in phase 3
                    264, // 316 in phase 2, 369 in phase 3
                    280, // 336 in phase 2, 392 in phase 3
                    384 } }, // 460 in phase 2, 537 in phase 3
                { ModContent.NPCType<PolterPhantom>(), new int[] { 210, 336, 360, 392, 528 } },

                { ModContent.NPCType<OldDuke>(), new int[] {
                    160, // 176 in phase 2, 192 in phase 3
                    288, // 316 in phase 2, 345 in phase 3
                    324, // 356 in phase 2, 388 in phase 3
                    342, // 376 in phase 2, 410 in phase 3
                    432 } }, // 475 in phase 2, 518 in phase 3
                { ModContent.NPCType<OldDukeToothBall>(), new int[] { 180, 288, 328, 344, 432 } },
                { ModContent.NPCType<SulphurousSharkron>(), new int[] { 180, 288, 328, 344, 432 } },

                { ModContent.NPCType<DevourerofGodsHead>(), new int[] { 350, 700, 750, 780, 880 } },
                { ModContent.NPCType<DevourerofGodsBody>(), new int[] { 220, 374, 425, 442, 561 } },
                { ModContent.NPCType<DevourerofGodsTail>(), new int[] { 180, 306, 340, 357, 459 } },
                { ModContent.NPCType<CosmicGuardianHead>(), new int[] { 180, 360, 396, 420, 510 } },
                { ModContent.NPCType<CosmicGuardianBody>(), new int[] { 130, 260, 290, 320, 420 } },
                { ModContent.NPCType<CosmicGuardianTail>(), new int[] { 100, 200, 230, 260, 330 } },

                { ModContent.NPCType<Yharon>(), new int[] { 280, 448, 480, 512, 624 } },

                { ModContent.NPCType<SupremeCalamitas>(), new int[] { 320, 512, 544, 560, 696 } },

                { ModContent.NPCType<Apollo>(), new int[] { 320, 512, 544, 560, 696 } },
                { ModContent.NPCType<Artemis>(), new int[] { 300, 480, 512, 528, 660 } },

                { ModContent.NPCType<ThanatosHead>(), new int[] { 350, 560, 592, 608, 756 } },
                { ModContent.NPCType<ThanatosBody1>(), new int[] { 300, 480, 512, 528, 660 } },
                { ModContent.NPCType<ThanatosBody2>(), new int[] { 300, 480, 512, 528, 660 } },
                { ModContent.NPCType<ThanatosTail>(), new int[] { 250, 400, 424, 440, 552 } },

                { ModContent.NPCType<PrimordialWyrmHead>(), new int[] { 400, 800, 850, 880, 1000 } }
            };

            EnemyStats.ProjectileDamageValues = new SortedDictionary<Tuple<int, int>, int[]>
            {
                { new Tuple<int, int>(ModContent.NPCType<KingSlimeJewel>(), ModContent.ProjectileType<JewelProjectile>()), new int[] { 22, 36, 44, 48, 66 } },

                { new Tuple<int, int>(ModContent.NPCType<DesertScourgeHead>(), ModContent.ProjectileType<SandBlast>()), new int[] { 26, 44, 60, 68, 90 } },
                { new Tuple<int, int>(ModContent.NPCType<DesertScourgeHead>(), ModContent.ProjectileType<GreatSandBlast>()), new int[] { 26, 44, 60, 68, 90 } },

                { new Tuple<int, int>(ModContent.NPCType<Crabulon>(), ModContent.ProjectileType<MushBomb>()), new int[] { 32, 48, 60, 68, 90 } },
                { new Tuple<int, int>(ModContent.NPCType<Crabulon>(), ModContent.ProjectileType<MushBombFall>()), new int[] { 32, 48, 60, 68, 90 } },

                { new Tuple<int, int>(NPCID.EaterofWorldsHead, ProjectileID.CursedFlameHostile), new int[] { 32, 48, 60, 68, 90 } },

                { new Tuple<int, int>(ModContent.NPCType<HiveMind>(), ModContent.ProjectileType<ShadeNimbusHostile>()), new int[] { 36, 56, 68, 76, 102 } },
                { new Tuple<int, int>(ModContent.NPCType<DankCreeper>(), ModContent.ProjectileType<ShadeNimbusHostile>()), new int[] { 36, 56, 68, 76, 102 } },
                { new Tuple<int, int>(ModContent.NPCType<DarkHeart>(), ModContent.ProjectileType<ShaderainHostile>()), new int[] { 36, 56, 68, 76, 102 } },
                { new Tuple<int, int>(ModContent.NPCType<HiveBlob>(), ModContent.ProjectileType<VileClot>()), new int[] { 30, 48, 60, 68, 90 } },
                { new Tuple<int, int>(ModContent.NPCType<HiveBlob2>(), ModContent.ProjectileType<VileClot>()), new int[] { 30, 48, 60, 68, 90 } },

                { new Tuple<int, int>(ModContent.NPCType<PerforatorHive>(), ModContent.ProjectileType<BloodGeyser>()), new int[] { 36, 56, 68, 76, 102 } },
                { new Tuple<int, int>(ModContent.NPCType<PerforatorHive>(), ModContent.ProjectileType<IchorShot>()), new int[] { 36, 56, 68, 76, 102 } },
                { new Tuple<int, int>(ModContent.NPCType<PerforatorHive>(), ModContent.ProjectileType<IchorBlob>()), new int[] { 36, 56, 68, 76, 102 } },
                { new Tuple<int, int>(ModContent.NPCType<PerforatorHeadMedium>(), ModContent.ProjectileType<IchorBlob>()), new int[] { 36, 56, 68, 76, 102 } },
                { new Tuple<int, int>(ModContent.NPCType<PerforatorBodyMedium>(), ModContent.ProjectileType<IchorBlob>()), new int[] { 36, 56, 68, 76, 102 } },
                { new Tuple<int, int>(ModContent.NPCType<PerforatorTailMedium>(), ModContent.ProjectileType<IchorBlob>()), new int[] { 36, 56, 68, 76, 102 } },
                { new Tuple<int, int>(ModContent.NPCType<PerforatorHeadLarge>(), ModContent.ProjectileType<DoGDeath>()), new int[] { 22, 36, 44, 48, 66 } },

                { new Tuple<int, int>(NPCID.QueenBee, ProjectileID.QueenBeeStinger), new int[] { 22, 44, 64, 72, 96 } }, // 66 damage in non-rev master mode

                { new Tuple<int, int>(NPCID.SkeletronHead, ProjectileID.Skull), new int[] { 46, 68, 84, 92, 126 } }, // 102 damage in non-rev master mode

                { new Tuple<int, int>(NPCID.Deerclops, ProjectileID.DeerclopsIceSpike), new int[] { 26, 52, 76, 84, 114 } }, // 78 damage in non-rev master mode
                { new Tuple<int, int>(NPCID.Deerclops, ProjectileID.DeerclopsRangedProjectile), new int[] { 36, 72, 88, 96, 132 } }, // 108 damage in non-rev master mode
                { new Tuple<int, int>(NPCID.Deerclops, ProjectileID.InsanityShadowHostile), new int[] { 20, 40, 60, 68, 90 } }, // 60 damage in non-rev master mode

                { new Tuple<int, int>(ModContent.NPCType<SlimeGodCore>(), ModContent.ProjectileType<UnstableEbonianGlob>()), new int[] { 42, 68, 84, 92, 126 } },
                { new Tuple<int, int>(ModContent.NPCType<SlimeGodCore>(), ModContent.ProjectileType<UnstableCrimulanGlob>()), new int[] { 38, 60, 76, 84, 114 } },
                { new Tuple<int, int>(ModContent.NPCType<EbonianPaladin>(), ModContent.ProjectileType<UnstableEbonianGlob>()), new int[] { 42, 68, 84, 92, 126 } },
                { new Tuple<int, int>(ModContent.NPCType<CrimulanPaladin>(), ModContent.ProjectileType<UnstableCrimulanGlob>()), new int[] { 38, 60, 76, 84, 114 } },
                { new Tuple<int, int>(ModContent.NPCType<SplitEbonianPaladin>(), ModContent.ProjectileType<UnstableEbonianGlob>()), new int[] { 38, 60, 76, 84, 114 } },
                { new Tuple<int, int>(ModContent.NPCType<SplitCrimulanPaladin>(), ModContent.ProjectileType<UnstableCrimulanGlob>()), new int[] { 34, 52, 68, 74, 102 } },
                { new Tuple<int, int>(ModContent.NPCType<CorruptSlimeSpawn>(), ModContent.ProjectileType<ShadeNimbusHostile>()), new int[] { 36, 56, 68, 76, 102 } },
                { new Tuple<int, int>(ModContent.NPCType<CrimsonSlimeSpawn2>(), ModContent.ProjectileType<CrimsonSpike>()), new int[] { 24, 48, 60, 68, 90 } },

                { new Tuple<int, int>(NPCID.WallofFleshEye, ProjectileID.EyeLaser), new int[] {
                    22, // 22 to 30, depending on life
                    44, // 44 to 60, depending on life
                    68,
                    76,
                    102 } }, // 66 to 90, depending on life
                { new Tuple<int, int>(NPCID.WallofFleshEye, ProjectileID.DeathLaser), new int[] { 40, 72, 88, 96, 132 } },
                { new Tuple<int, int>(NPCID.WallofFlesh, ProjectileID.DemonSickle), new int[] { 52, 92, 112, 124, 168 } },

                { new Tuple<int, int>(NPCID.QueenSlimeBoss, ProjectileID.QueenSlimeGelAttack), new int[] { 60, 120, 136, 152, 210 } },
                { new Tuple<int, int>(NPCID.QueenSlimeBoss, ProjectileID.QueenSlimeSmash), new int[] { 80, 160, 188, 200, 270 } },
                { new Tuple<int, int>(NPCID.QueenSlimeBoss, ProjectileID.QueenSlimeMinionBlueSpike), new int[] { 0, 0, 112, 124, 168 } },

                { new Tuple<int, int>(ModContent.NPCType<Cryogen>(), ModContent.ProjectileType<IceBlast>()), new int[] { 52, 92, 112, 124, 168 } },
                { new Tuple<int, int>(ModContent.NPCType<Cryogen>(), ModContent.ProjectileType<IceBomb>()), new int[] { 70, 120, 136, 152, 210 } },
                { new Tuple<int, int>(ModContent.NPCType<Cryogen>(), ModContent.ProjectileType<IceRain>()), new int[] { 60, 100, 120, 132, 180 } },
                { new Tuple<int, int>(ModContent.NPCType<CryogenShield>(), ModContent.ProjectileType<IceBlast>()), new int[] { 52, 92, 112, 124, 168 } },

                { new Tuple<int, int>(NPCID.Retinazer, ProjectileID.EyeLaser), new int[] { 40, 76, 116, 128, 174 } }, // 114 damage in non-rev master mode
                { new Tuple<int, int>(NPCID.Retinazer, ProjectileID.DeathLaser), new int[] {
                    50, // 36 in rapid fire
                    92, // 68 in rapid fire
                    124, // 93 in rapid fire
                    136, // 102 in rapid fire
                    186 } }, // 139 in rapid fire; 138 in non-rev master mode, 102 in rapid fire in non-rev master mode
                { new Tuple<int, int>(NPCID.Retinazer, ModContent.ProjectileType<ScavengerLaser>()), new int[] { 70, 120, 136, 152, 210 } },
                { new Tuple<int, int>(NPCID.Spazmatism, ProjectileID.CursedFlameHostile), new int[] { 50, 88, 120, 132, 180 } }, // 132 in non-rev master mode
                { new Tuple<int, int>(NPCID.Spazmatism, ProjectileID.EyeFire), new int[] { 60, 108, 0, 0, 162 } }, // Only used in non-rev modes
                { new Tuple<int, int>(NPCID.Spazmatism, ModContent.ProjectileType<Shadowflamethrower>()), new int[] { 70, 120, 148, 160, 222 } },
                { new Tuple<int, int>(NPCID.Spazmatism, ModContent.ProjectileType<ShadowflameFireball>()), new int[] { 60, 100, 128, 140, 192 } },

                { new Tuple<int, int>(ModContent.NPCType<AquaticScourgeHead>(), ModContent.ProjectileType<SulphuricAcidMist>()), new int[] { 60, 100, 120, 132, 180 } },
                { new Tuple<int, int>(ModContent.NPCType<AquaticScourgeHead>(), ModContent.ProjectileType<SandPoisonCloud>()), new int[] { 70, 120, 136, 152, 210 } },
                { new Tuple<int, int>(ModContent.NPCType<AquaticScourgeHead>(), ModContent.ProjectileType<ToxicCloud>()), new int[] { 80, 140, 160, 176, 234 } },
                { new Tuple<int, int>(ModContent.NPCType<AquaticScourgeBody>(), ModContent.ProjectileType<SandTooth>()), new int[] { 66, 112, 128, 140, 192 } },

                { new Tuple<int, int>(NPCID.TheDestroyer, ProjectileID.DeathLaser), new int[] { 0, 0, 0, 152, 210 } },
                { new Tuple<int, int>(NPCID.TheDestroyerBody, ProjectileID.DeathLaser), new int[] { 44, 72, 116, 128, 174 } }, // 108 in non-rev master mode
                { new Tuple<int, int>(NPCID.TheDestroyerBody, ModContent.ProjectileType<DestroyerCursedLaser>()), new int[] { 46, 76, 128, 140, 186 } },
                { new Tuple<int, int>(NPCID.TheDestroyerBody, ModContent.ProjectileType<DestroyerElectricLaser>()), new int[] { 48, 80, 136, 152, 210 } },
                { new Tuple<int, int>(NPCID.TheDestroyerBody, ProjectileID.EyeLaser), new int[] { 40, 68, 100, 112, 144 } },
                { new Tuple<int, int>(NPCID.Probe, ProjectileID.PinkLaser), new int[] { 50, 88, 100, 112, 144 } }, // 132 in non-rev master mode

                { new Tuple<int, int>(ModContent.NPCType<BrimstoneElemental>(), ModContent.ProjectileType<BrimstoneHellfireball>()), new int[] { 80, 140, 160, 176, 234 } },
                { new Tuple<int, int>(ModContent.NPCType<BrimstoneElemental>(), ModContent.ProjectileType<BrimstoneBarrage>()), new int[] { 70, 112, 128, 140, 192 } },
                { new Tuple<int, int>(ModContent.NPCType<BrimstoneElemental>(), ModContent.ProjectileType<BrimstoneHellblast>()), new int[] { 80, 140, 160, 176, 234 } },
                { new Tuple<int, int>(ModContent.NPCType<BrimstoneElemental>(), ModContent.ProjectileType<BrimstoneRay>()), new int[] { 120, 200, 240, 252, 342 } },
                { new Tuple<int, int>(ModContent.NPCType<Brimling>(), ModContent.ProjectileType<BrimstoneHellfireball>()), new int[] { 80, 140, 160, 176, 234 } },
                { new Tuple<int, int>(ModContent.NPCType<Brimling>(), ModContent.ProjectileType<BrimstoneBarrage>()), new int[] { 70, 112, 128, 140, 192 } },

                { new Tuple<int, int>(NPCID.SkeletronPrime, ProjectileID.Skull), new int[] { 50, 100, 124, 136, 186 } },
                { new Tuple<int, int>(NPCID.SkeletronPrime, ProjectileID.DeathLaser), new int[] { 50, 100, 124, 136, 186 } },
                { new Tuple<int, int>(NPCID.SkeletronPrime, ProjectileID.RocketSkeleton), new int[] { 60, 120, 148, 160, 222 } },
                { new Tuple<int, int>(NPCID.PrimeCannon, ProjectileID.RocketSkeleton), new int[] { 60, 120, 148, 160, 222 } },
                { new Tuple<int, int>(NPCID.PrimeCannon, ProjectileID.BombSkeletronPrime), new int[] { 80, 160, 0, 0, 240 } },
                { new Tuple<int, int>(NPCID.PrimeLaser, ProjectileID.DeathLaser), new int[] { 50, 100, 124, 136, 186 } }, // 150 in non-rev master mode

                { new Tuple<int, int>(ModContent.NPCType<CalamitasClone>(), ModContent.ProjectileType<BrimstoneHellblast>()), new int[] { 80, 140, 160, 176, 234 } },
                { new Tuple<int, int>(ModContent.NPCType<CalamitasClone>(), ModContent.ProjectileType<BrimstoneHellfireball>()), new int[] { 80, 140, 160, 176, 234 } },
                { new Tuple<int, int>(ModContent.NPCType<CalamitasClone>(), ModContent.ProjectileType<BrimstoneHellblast2>()), new int[] { 80, 140, 160, 176, 234 } },
                { new Tuple<int, int>(ModContent.NPCType<CalamitasClone>(), ModContent.ProjectileType<SCalBrimstoneFireblast>()), new int[] { 80, 140, 160, 176, 234 } },
                { new Tuple<int, int>(ModContent.NPCType<CalamitasClone>(), ModContent.ProjectileType<BrimstoneBarrage>()), new int[] { 70, 112, 128, 140, 192 } },
                { new Tuple<int, int>(ModContent.NPCType<Cataclysm>(), ModContent.ProjectileType<BrimstoneFire>()), new int[] { 80, 140, 160, 176, 234 } },
                { new Tuple<int, int>(ModContent.NPCType<Catastrophe>(), ModContent.ProjectileType<BrimstoneBall>()), new int[] { 70, 112, 128, 140, 192 } },
                { new Tuple<int, int>(ModContent.NPCType<SoulSeeker>(), ModContent.ProjectileType<BrimstoneBarrage>()), new int[] { 70, 112, 128, 140, 192 } },

                { new Tuple<int, int>(NPCID.Plantera, ProjectileID.SeedPlantera), new int[] { 44, 76, 128, 140, 186 } }, // 114 in non-rev master mode
                { new Tuple<int, int>(NPCID.Plantera, ProjectileID.PoisonSeedPlantera), new int[] { 54, 96, 136, 152, 210 } }, // 144 in non-rev master mode
                { new Tuple<int, int>(NPCID.Plantera, ProjectileID.ThornBall), new int[] { 62, 108, 160, 176, 234 } }, // 162 in non-rev master mode
                { new Tuple<int, int>(NPCID.Plantera, ModContent.ProjectileType<SporeGasPlantera>()), new int[] { 80, 140, 160, 176, 234 } },
                { new Tuple<int, int>(NPCID.Plantera, ModContent.ProjectileType<HomingGasBulb>()), new int[] { 80, 140, 160, 176, 234 } },

                { new Tuple<int, int>(NPCID.HallowBoss, ProjectileID.HallowBossRainbowStreak), new int[] { 90, 120, 160, 176, 234 } },
                { new Tuple<int, int>(NPCID.HallowBoss, ProjectileID.FairyQueenSunDance), new int[] { 100, 140, 184, 200, 270 } },
                { new Tuple<int, int>(NPCID.HallowBoss, ProjectileID.HallowBossLastingRainbow), new int[] { 90, 120, 160, 176, 234 } },
                { new Tuple<int, int>(NPCID.HallowBoss, ProjectileID.FairyQueenLance), new int[] { 100, 120, 160, 176, 234 } },

                { new Tuple<int, int>(ModContent.NPCType<Leviathan>(), ModContent.ProjectileType<LeviathanBomb>()), new int[] { 100, 172, 208, 228, 300 } },
                { new Tuple<int, int>(ModContent.NPCType<Anahita>(), ModContent.ProjectileType<WaterSpear>()), new int[] { 80, 140, 160, 176, 234 } },
                { new Tuple<int, int>(ModContent.NPCType<Anahita>(), ModContent.ProjectileType<FrostMist>()), new int[] { 84, 148, 172, 188, 252 } },
                { new Tuple<int, int>(ModContent.NPCType<Anahita>(), ModContent.ProjectileType<SirenSong>()), new int[] { 88, 156, 184, 200, 270 } },

                { new Tuple<int, int>(ModContent.NPCType<AstrumAureus>(), ModContent.ProjectileType<AstralLaser>()), new int[] { 88, 156, 184, 200, 270 } },
                { new Tuple<int, int>(ModContent.NPCType<AstrumAureus>(), ModContent.ProjectileType<AstralFlame>()), new int[] { 100, 172, 208, 228, 300 } },
                { new Tuple<int, int>(ModContent.NPCType<AureusSpawn>(), ModContent.ProjectileType<AstralLaser>()), new int[] { 88, 156, 184, 200, 270 } },

                { new Tuple<int, int>(NPCID.Golem, ProjectileID.Fireball), new int[] { 58, 116, 160, 176, 234 } },
                { new Tuple<int, int>(NPCID.Golem, ProjectileID.EyeBeam), new int[] { 56, 112, 172, 188, 252 } },
                { new Tuple<int, int>(NPCID.GolemHead, ProjectileID.Fireball), new int[] {
                    36, // 36 to 58 depending on life
                    72, // 72 to 116 depending on life
                    160,
                    176,
                    234 } }, // 108 to 174 depending on life in non-rev master mode
                { new Tuple<int, int>(NPCID.GolemHead, ProjectileID.EyeBeam), new int[] { 56, 112, 172, 188, 252 } }, // 168 in non-rev master mode
                { new Tuple<int, int>(NPCID.GolemHeadFree, ProjectileID.Fireball), new int[] {
                    36, // 36 to 58 depending on life
                    72, // 72 to 116 depending on life
                    160,
                    176,
                    234 } }, // 108 to 174 depending on life in non-rev master mode
                { new Tuple<int, int>(NPCID.GolemHeadFree, ProjectileID.EyeBeam), new int[] { 56, 112, 172, 188, 252 } }, // 168 in non-rev master mode
                { new Tuple<int, int>(NPCID.GolemHeadFree, ProjectileID.InfernoHostileBolt), new int[] { 64, 128, 184, 200, 270 } },

                { new Tuple<int, int>(ModContent.NPCType<PlaguebringerGoliath>(), ModContent.ProjectileType<PlagueStingerGoliath>()), new int[] { 88, 156, 184, 200, 270 } },
                { new Tuple<int, int>(ModContent.NPCType<PlaguebringerGoliath>(), ModContent.ProjectileType<PlagueStingerGoliathV2>()), new int[] { 88, 156, 184, 200, 270 } },
                { new Tuple<int, int>(ModContent.NPCType<PlaguebringerGoliath>(), ModContent.ProjectileType<HiveBombGoliath>()), new int[] { 120, 192, 220, 240, 330 } },

                { new Tuple<int, int>(NPCID.DukeFishron, ProjectileID.Sharknado), new int[] { 80, 100, 184, 200, 270 } }, // 150 in non-rev master mode
                { new Tuple<int, int>(NPCID.DukeFishron, ProjectileID.Cthulunado), new int[] { 160, 200, 232, 248, 348 } }, // 300 in non-rev master mode

                { new Tuple<int, int>(ModContent.NPCType<RavagerBody>(), ModContent.ProjectileType<RavagerBlaster>()), new int[] { 120, 180, 208, 224, 312 } },
                { new Tuple<int, int>(ModContent.NPCType<RavagerHead>(), ModContent.ProjectileType<ScavengerNuke>()), new int[] { 120, 180, 208, 224, 312 } },
                { new Tuple<int, int>(ModContent.NPCType<RavagerHead2>(), ModContent.ProjectileType<ScavengerLaser>()), new int[] { 90, 144, 172, 188, 258 } },
                { new Tuple<int, int>(ModContent.NPCType<RavagerHead2>(), ModContent.ProjectileType<ScavengerNuke>()), new int[] { 120, 180, 208, 224, 312 } },
                { new Tuple<int, int>(ModContent.NPCType<FlamePillar>(), ModContent.ProjectileType<RavagerFlame>()), new int[] { 90, 144, 172, 188, 258 } },

                { new Tuple<int, int>(NPCID.CultistBoss, ProjectileID.CultistBossFireBall), new int[] { 60, 80, 184, 200, 270 } }, // 80 in non-rev master mode
                { new Tuple<int, int>(NPCID.CultistBoss, ProjectileID.CultistBossIceMist), new int[] { 70, 100, 208, 224, 312 } }, // 100 in non-rev master mode
                { new Tuple<int, int>(NPCID.CultistBoss, ProjectileID.CultistBossLightningOrb), new int[] { 90, 120, 220, 240, 330 } }, // 120 in non-rev master mode
                { new Tuple<int, int>(NPCID.CultistBossClone, ProjectileID.CultistBossFireBallClone), new int[] { 36, 72, 172, 188, 258 } }, // 72 in non-rev master mode
                { new Tuple<int, int>(NPCID.AncientDoom, ProjectileID.AncientDoomProjectile), new int[] { 60, 180, 184, 200, 270 } }, // 402 in non-rev master mode

                { new Tuple<int, int>(ModContent.NPCType<AstrumDeusBody>(), ModContent.ProjectileType<AstralShot2>()), new int[] { 90, 152, 176, 188, 264 } },
                { new Tuple<int, int>(ModContent.NPCType<AstrumDeusBody>(), ModContent.ProjectileType<DeusMine>()), new int[] { 120, 180, 208, 224, 312 } },
                { new Tuple<int, int>(ModContent.NPCType<AstrumDeusBody>(), ModContent.ProjectileType<AstralGodRay>()), new int[] { 100, 172, 192, 204, 288 } },

                { new Tuple<int, int>(NPCID.MoonLordHead, ProjectileID.PhantasmalDeathray), new int[] { 150, 300, 380, 420, 570 } }, // 450 in non-rev master mode
                { new Tuple<int, int>(NPCID.MoonLordHead, ProjectileID.PhantasmalBolt), new int[] { 60, 120, 160, 176, 240 } }, // 180 in non-rev master mode
                { new Tuple<int, int>(NPCID.MoonLordHand, ProjectileID.PhantasmalEye), new int[] { 60, 120, 160, 176, 240 } }, // 180 in non-rev master mode
                { new Tuple<int, int>(NPCID.MoonLordHand, ProjectileID.PhantasmalSphere), new int[] { 80, 160, 260, 284, 390 } }, // 240 in non-rev master mode
                { new Tuple<int, int>(NPCID.MoonLordHand, ProjectileID.PhantasmalBolt), new int[] { 60, 120, 160, 176, 240 } }, // 180 in non-rev master mode
                { new Tuple<int, int>(NPCID.MoonLordFreeEye, ProjectileID.PhantasmalBolt), new int[] { 70, 140, 180, 200, 270 } }, // 210 in non-rev master mode
                { new Tuple<int, int>(NPCID.MoonLordFreeEye, ProjectileID.PhantasmalEye), new int[] { 70, 140, 180, 200, 270 } }, // 210 in non-rev master mode
                { new Tuple<int, int>(NPCID.MoonLordFreeEye, ProjectileID.PhantasmalSphere), new int[] { 88, 176, 260, 284, 390 } }, // 264 in non-rev master mode
                { new Tuple<int, int>(NPCID.MoonLordFreeEye, ProjectileID.PhantasmalDeathray), new int[] { 100, 200, 260, 284, 390 } }, // 300 in non-rev master mode

                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianCommander>(), ModContent.ProjectileType<ProfanedSpear>()), new int[] { 140, 220, 244, 256, 366 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianCommander>(), ModContent.ProjectileType<HolySpear>()), new int[] { 140, 220, 244, 256, 366 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianCommander>(), ModContent.ProjectileType<HolyBlast>()), new int[] { 140, 220, 244, 256, 366 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianCommander>(), ModContent.ProjectileType<HolyFire>()), new int[] { 120, 192, 220, 236, 330 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianCommander>(), ModContent.ProjectileType<HolyFire2>()), new int[] { 120, 192, 220, 236, 330 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianCommander>(), ModContent.ProjectileType<ProvidenceHolyRay>()), new int[] { 160, 320, 352, 370, 528 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianDefender>(), ModContent.ProjectileType<HolyBomb>()), new int[] { 140, 220, 244, 256, 366 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianDefender>(), ModContent.ProjectileType<HolyFlare>()), new int[] { 105, 171, 189, 198, 284 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianDefender>(), ModContent.ProjectileType<MoltenBlast>()), new int[] { 140, 228, 252, 264, 378 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianDefender>(), ModContent.ProjectileType<MoltenBlob>()), new int[] { 105, 171, 189, 198, 284 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianHealer>(), ModContent.ProjectileType<ProvidenceCrystalShard>()), new int[] { 140, 220, 244, 256, 366 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianHealer>(), ModContent.ProjectileType<HolyBurnOrb>()), new int[] { 120, 192, 220, 236, 330 } },
                { new Tuple<int, int>(ModContent.NPCType<ProfanedGuardianHealer>(), ModContent.ProjectileType<HolyLight>()), new int[] { 35, 50, 50, 0, 0 } },

                { new Tuple<int, int>(ModContent.NPCType<Bumblefuck>(), ModContent.ProjectileType<RedLightningFeather>()), new int[] { 140, 220, 244, 256, 366 } },
                { new Tuple<int, int>(ModContent.NPCType<Bumblefuck>(), ModContent.ProjectileType<BirbAuraFlare>()), new int[] { 200, 300, 332, 356, 498 } },

                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<HolyBlast>()), new int[] { 150, 264, 288, 300, 432 } }, // Split holy fire does: 113, 198, 216, 225, 324
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<HolyFire>()), new int[] { 120, 192, 220, 236, 330 } },
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<HolyFire2>()), new int[] { 120, 192, 220, 236, 330 } },
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<HolyBurnOrb>()), new int[] { 140, 220, 244, 256, 366 } },
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<HolyLight>()), new int[] { 35, 50, 50, 0, 0 } },
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<MoltenBlast>()), new int[] { 140, 228, 252, 264, 378 } },
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<MoltenBlob>()), new int[] { 105, 171, 189, 198, 284 } },
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<HolyBomb>()), new int[] { 140, 228, 252, 264, 378 } },
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<HolyFlare>()), new int[] { 105, 171, 189, 198, 284 } },
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<HolySpear>()), new int[] { 140, 220, 244, 256, 366 } },
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<ProvidenceCrystal>()), new int[] { 140, 228, 252, 264, 378 } },
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<ProvidenceCrystalShard>()), new int[] { 140, 228, 252, 264, 378 } },
                { new Tuple<int, int>(ModContent.NPCType<Providence>(), ModContent.ProjectileType<ProvidenceHolyRay>()), new int[] { 200, 400, 440, 464, 660 } },

                { new Tuple<int, int>(ModContent.NPCType<CeaselessVoid>(), ModContent.ProjectileType<DoGBeamPortal>()), new int[] { 140, 240, 264, 280, 396 } },
                { new Tuple<int, int>(ModContent.NPCType<CeaselessVoid>(), ModContent.ProjectileType<DarkEnergyBall>()), new int[] { 140, 240, 264, 280, 396 } },
                { new Tuple<int, int>(ModContent.NPCType<CeaselessVoid>(), ModContent.ProjectileType<DarkEnergyBall2>()), new int[] { 140, 240, 264, 280, 396 } },

                { new Tuple<int, int>(ModContent.NPCType<StormWeaverHead>(), ProjectileID.CultistBossLightningOrbArc), new int[] { 150, 264, 288, 300, 432 } },
                { new Tuple<int, int>(ModContent.NPCType<StormWeaverHead>(), ProjectileID.FrostWave), new int[] { 150, 264, 288, 300, 432 } },
                { new Tuple<int, int>(ModContent.NPCType<StormWeaverHead>(), ModContent.ProjectileType<StormMarkHostile>()), new int[] { 160, 276, 304, 320, 456 } },
                { new Tuple<int, int>(ModContent.NPCType<StormWeaverBody>(), ModContent.ProjectileType<DestroyerElectricLaser>()), new int[] { 140, 240, 264, 280, 396 } },
                { new Tuple<int, int>(ModContent.NPCType<StormWeaverTail>(), ProjectileID.CultistBossLightningOrb), new int[] { 150, 264, 288, 300, 432 } },

                { new Tuple<int, int>(ModContent.NPCType<Signus>(), ModContent.ProjectileType<SignusScythe>()), new int[] { 140, 240, 264, 280, 396 } },
                { new Tuple<int, int>(ModContent.NPCType<Signus>(), ModContent.ProjectileType<EssenceDust>()), new int[] { 140, 240, 264, 280, 396 } },

                { new Tuple<int, int>(ModContent.NPCType<Polterghast>(), ModContent.ProjectileType<PhantomShot>()), new int[] { 140, 240, 264, 280, 396 } },
                { new Tuple<int, int>(ModContent.NPCType<Polterghast>(), ModContent.ProjectileType<PhantomShot2>()), new int[] { 150, 264, 288, 300, 432 } },
                { new Tuple<int, int>(ModContent.NPCType<Polterghast>(), ModContent.ProjectileType<PhantomBlast>()), new int[] { 150, 264, 288, 300, 432 } },
                { new Tuple<int, int>(ModContent.NPCType<Polterghast>(), ModContent.ProjectileType<PhantomBlast2>()), new int[] { 160, 276, 304, 320, 456 } },
                { new Tuple<int, int>(ModContent.NPCType<PolterghastHook>(), ModContent.ProjectileType<PhantomHookShot>()), new int[] { 140, 240, 264, 280, 396 } },
                { new Tuple<int, int>(ModContent.NPCType<PhantomFuckYou>(), ModContent.ProjectileType<PhantomMine>()), new int[] { 170, 296, 324, 340, 462 } },
                { new Tuple<int, int>(ModContent.NPCType<PhantomSpiritL>(), ModContent.ProjectileType<PhantomGhostShot>()), new int[] { 150, 264, 288, 300, 432 } },

                { new Tuple<int, int>(ModContent.NPCType<Mauler>(), ModContent.ProjectileType<MaulerAcidBubble>()), new int[] { 140, 220, 220, 220, 220 } },
                { new Tuple<int, int>(ModContent.NPCType<Mauler>(), ModContent.ProjectileType<MaulerAcidDrop>()), new int[] { 140, 220, 220, 220, 220 } },

                { new Tuple<int, int>(ModContent.NPCType<OldDuke>(), ModContent.ProjectileType<OldDukeGore>()), new int[] { 170, 296, 324, 340, 462 } },
                { new Tuple<int, int>(ModContent.NPCType<OldDuke>(), ModContent.ProjectileType<OldDukeVortex>()), new int[] { 280, 400, 440, 464, 660 } },
                { new Tuple<int, int>(ModContent.NPCType<OldDukeToothBall>(), ModContent.ProjectileType<OldDukeToothBallSpike>()), new int[] { 170, 296, 324, 340, 462 } },
                { new Tuple<int, int>(ModContent.NPCType<OldDukeToothBall>(), ModContent.ProjectileType<SandPoisonCloudOldDuke>()), new int[] { 180, 316, 348, 364, 492 } },
                { new Tuple<int, int>(ModContent.NPCType<SulphurousSharkron>(), ModContent.ProjectileType<OldDukeGore>()), new int[] { 170, 296, 324, 340, 462 } },

                { new Tuple<int, int>(ModContent.NPCType<DevourerofGodsHead>(), ModContent.ProjectileType<DoGDeath>()), new int[] { 180, 316, 348, 364, 492 } },
                { new Tuple<int, int>(ModContent.NPCType<DevourerofGodsHead>(), ModContent.ProjectileType<DoGFire>()), new int[] { 200, 340, 376, 396, 564 } },
                { new Tuple<int, int>(ModContent.NPCType<DevourerofGodsBody>(), ModContent.ProjectileType<DoGDeath>()), new int[] { 180, 316, 348, 364, 492 } },

                { new Tuple<int, int>(ModContent.NPCType<Yharon>(), ModContent.ProjectileType<SkyFlareRevenge>()), new int[] { 300, 520, 548, 564, 822 } },
                { new Tuple<int, int>(ModContent.NPCType<Yharon>(), ModContent.ProjectileType<FlareBomb>()), new int[] { 220, 384, 424, 444, 600 } },
                { new Tuple<int, int>(ModContent.NPCType<Yharon>(), ModContent.ProjectileType<Flarenado>()), new int[] { 250, 440, 464, 476, 696 } },
                { new Tuple<int, int>(ModContent.NPCType<Yharon>(), ModContent.ProjectileType<Infernado>()), new int[] { 250, 440, 464, 476, 696 } },
                { new Tuple<int, int>(ModContent.NPCType<Yharon>(), ModContent.ProjectileType<Infernado2>()), new int[] { 250, 440, 464, 476, 696 } },
                { new Tuple<int, int>(ModContent.NPCType<Yharon>(), ModContent.ProjectileType<FlareDust>()), new int[] { 220, 384, 424, 444, 600 } },
                { new Tuple<int, int>(ModContent.NPCType<Yharon>(), ModContent.ProjectileType<FlareDust2>()), new int[] { 220, 384, 424, 444, 600 } },
                { new Tuple<int, int>(ModContent.NPCType<Yharon>(), ModContent.ProjectileType<YharonFireball>()), new int[] { 220, 384, 424, 444, 600 } },
                { new Tuple<int, int>(ModContent.NPCType<Yharon>(), ModContent.ProjectileType<YharonBulletHellVortex>()), new int[] { 250, 440, 464, 476, 696 } },

                { new Tuple<int, int>(ModContent.NPCType<SupremeCalamitas>(), ModContent.ProjectileType<BrimstoneHellblast2>()), new int[] { 280, 476, 484, 500, 726 } },
                { new Tuple<int, int>(ModContent.NPCType<SupremeCalamitas>(), ModContent.ProjectileType<SCalBrimstoneFireblast>()), new int[] { 280, 476, 484, 500, 726 } },
                { new Tuple<int, int>(ModContent.NPCType<SupremeCalamitas>(), ModContent.ProjectileType<SCalBrimstoneGigablast>()), new int[] { 300, 508, 528, 544, 780 } },
                { new Tuple<int, int>(ModContent.NPCType<SupremeCalamitas>(), ModContent.ProjectileType<BrimstoneMonster>()), new int[] { 350, 592, 624, 656, 918 } },
                { new Tuple<int, int>(ModContent.NPCType<SupremeCalamitas>(), ModContent.ProjectileType<BrimstoneWave>()), new int[] { 280, 476, 484, 500, 726 } },
                { new Tuple<int, int>(ModContent.NPCType<SupremeCalamitas>(), ModContent.ProjectileType<BrimstoneBarrage>()), new int[] { 250, 440, 464, 476, 696 } },
                { new Tuple<int, int>(ModContent.NPCType<SupremeCalamitas>(), ModContent.ProjectileType<BrimstoneHellblast>()), new int[] { 280, 476, 484, 500, 726 } },
                { new Tuple<int, int>(ModContent.NPCType<SepulcherBodyEnergyBall>(), ModContent.ProjectileType<BrimstoneBarrage>()), new int[] { 250, 440, 464, 476, 696 } },
                { new Tuple<int, int>(ModContent.NPCType<SoulSeekerSupreme>(), ModContent.ProjectileType<BrimstoneBarrage>()), new int[] { 250, 440, 464, 476, 696 } },
                { new Tuple<int, int>(ModContent.NPCType<SupremeCataclysm>(), ModContent.ProjectileType<SupremeCataclysmFist>()), new int[] { 280, 476, 484, 500, 726 } },
                { new Tuple<int, int>(ModContent.NPCType<SupremeCataclysm>(), ModContent.ProjectileType<BrimstoneBarrage>()), new int[] { 250, 440, 464, 476, 696 } },
                { new Tuple<int, int>(ModContent.NPCType<SupremeCatastrophe>(), ModContent.ProjectileType<SupremeCatastropheSlash>()), new int[] { 280, 476, 484, 500, 726 } },
                { new Tuple<int, int>(ModContent.NPCType<SupremeCatastrophe>(), ModContent.ProjectileType<BrimstoneBarrage>()), new int[] { 250, 440, 464, 476, 696 } },

                { new Tuple<int, int>(ModContent.NPCType<Artemis>(), ModContent.ProjectileType<ArtemisSpinLaserbeam>()), new int[] { 300, 508, 528, 544, 780 } },
                { new Tuple<int, int>(ModContent.NPCType<Artemis>(), ModContent.ProjectileType<ArtemisLaser>()), new int[] { 240, 408, 432, 456, 630 } },
                { new Tuple<int, int>(ModContent.NPCType<Apollo>(), ModContent.ProjectileType<ApolloFireball>()), new int[] { 240, 408, 432, 456, 630 } },
                { new Tuple<int, int>(ModContent.NPCType<Apollo>(), ModContent.ProjectileType<ApolloRocket>()), new int[] { 280, 476, 484, 500, 726 } },

                { new Tuple<int, int>(ModContent.NPCType<ThanatosHead>(), ModContent.ProjectileType<ThanatosBeamStart>()), new int[] { 350, 592, 624, 656, 918 } },
                { new Tuple<int, int>(ModContent.NPCType<ThanatosHead>(), ModContent.ProjectileType<ThanatosLaser>()), new int[] { 240, 408, 432, 456, 630 } },
                { new Tuple<int, int>(ModContent.NPCType<ThanatosBody1>(), ModContent.ProjectileType<ThanatosLaser>()), new int[] { 240, 408, 432, 456, 630 } },
                { new Tuple<int, int>(ModContent.NPCType<ThanatosBody2>(), ModContent.ProjectileType<ThanatosLaser>()), new int[] { 240, 408, 432, 456, 630 } },
                { new Tuple<int, int>(ModContent.NPCType<ThanatosTail>(), ModContent.ProjectileType<ThanatosLaser>()), new int[] { 240, 408, 432, 456, 630 } },

                { new Tuple<int, int>(ModContent.NPCType<AresBody>(), ModContent.ProjectileType<AresDeathBeamStart>()), new int[] { 300, 508, 528, 544, 780 } },
                { new Tuple<int, int>(ModContent.NPCType<AresLaserCannon>(), ModContent.ProjectileType<AresLaserBeamStart>()), new int[] { 300, 508, 528, 544, 780 } },
                { new Tuple<int, int>(ModContent.NPCType<AresLaserCannon>(), ModContent.ProjectileType<ThanatosLaser>()), new int[] { 240, 408, 432, 456, 630 } },
                { new Tuple<int, int>(ModContent.NPCType<AresPlasmaFlamethrower>(), ModContent.ProjectileType<AresPlasmaFireball>()), new int[] { 240, 408, 432, 456, 630 } },
                { new Tuple<int, int>(ModContent.NPCType<AresTeslaCannon>(), ModContent.ProjectileType<AresTeslaOrb>()), new int[] { 240, 408, 432, 456, 630 } },
                { new Tuple<int, int>(ModContent.NPCType<AresGaussNuke>(), ModContent.ProjectileType<AresGaussNukeProjectile>()), new int[] { 400, 608, 640, 658, 960 } },

                { new Tuple<int, int>(ModContent.NPCType<PrimordialWyrmHead>(), ProjectileID.CultistBossIceMist), new int[] { 400, 600, 632, 648, 948 } },
                { new Tuple<int, int>(ModContent.NPCType<PrimordialWyrmHead>(), ProjectileID.CultistBossLightningOrbArc), new int[] { 500, 752, 788, 808, 1182 } },
                { new Tuple<int, int>(ModContent.NPCType<PrimordialWyrmHead>(), ProjectileID.AncientDoomProjectile), new int[] { 400, 600, 632, 648, 948 } },
                { new Tuple<int, int>(ModContent.NPCType<PrimordialWyrmBodyAlt>(), ProjectileID.CultistBossFireBallClone), new int[] { 400, 600, 632, 648, 948 } }
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
