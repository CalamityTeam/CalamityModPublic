using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.PlagueEnemies;
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
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod
{
    public static partial class NPCStats
    {
        #region Boss Stats Container Struct
        internal partial struct EnemyStats
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
            bool exists = EnemyStats.DebuffImmunities.TryGetValue(npc.type, out var stupidTupleThing);
            if (npc.ModNPC is null || !exists)
                return;

            // If the npc is immune to everything, make it immune to everything
            if (stupidTupleThing.Item1)
            {
                for (int k = 0; k < npc.buffImmune.Length; k++)
                    npc.buffImmune[k] = true;
            }

            // Then set debuff vulnerabilities, or immunities if not immune to everything
            for (int i = 0; i < stupidTupleThing.Item2.Length; ++i)
                npc.buffImmune[stupidTupleThing.Item2[i]] = !stupidTupleThing.Item1;

            // Declare the immunity sets in 1.4's new registry.
            if (!NPCID.Sets.DebuffImmunitySets.ContainsKey(npc.type) && !stupidTupleThing.Item1)
            {
                NPCID.Sets.DebuffImmunitySets[npc.type] = new NPCDebuffImmunityData()
                {
                    SpecificallyImmuneTo = stupidTupleThing.Item2
                };
            }
        }
        #endregion

        #region Load/Unload
        // A static function, called exactly once, which initializes the EnemyStats struct at a predictable time.
        // This is necessary to ensure this dictionary is populated as early as possible.
        // Any boss not listed in here is only immune to Confusion and Pearl Aura.
        internal static void LoadDebuffs()
        {
            EnemyStats.DebuffImmunities = new SortedDictionary<int, Tuple<bool, int[]>>
            {
                { ModContent.NPCType<KingSlimeJewel>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },

                { NPCID.Deerclops, new Tuple<bool, int[]>(false, CalamityMod.iceEnemyImmunities) },

                { ModContent.NPCType<SlimeGodCore>(), new Tuple<bool, int[]>(false, CalamityMod.slimeEnemyImmunities) },
                { ModContent.NPCType<EbonianPaladin>(), new Tuple<bool, int[]>(false, CalamityMod.slimeEnemyImmunities) },
                { ModContent.NPCType<SplitEbonianPaladin>(), new Tuple<bool, int[]>(false, CalamityMod.slimeEnemyImmunities) },
                { ModContent.NPCType<CrimulanPaladin>(), new Tuple<bool, int[]>(false, CalamityMod.slimeEnemyImmunities) },
                { ModContent.NPCType<SplitCrimulanPaladin>(), new Tuple<bool, int[]>(false, CalamityMod.slimeEnemyImmunities) },
                { ModContent.NPCType<CorruptSlimeSpawn>(), new Tuple<bool, int[]>(false, CalamityMod.slimeEnemyImmunities) },
                { ModContent.NPCType<CorruptSlimeSpawn2>(), new Tuple<bool, int[]>(false, CalamityMod.slimeEnemyImmunities) },
                { ModContent.NPCType<CrimsonSlimeSpawn>(), new Tuple<bool, int[]>(false, CalamityMod.slimeEnemyImmunities) },
                { ModContent.NPCType<CrimsonSlimeSpawn2>(), new Tuple<bool, int[]>(false, CalamityMod.slimeEnemyImmunities) },

                { ModContent.NPCType<Cryogen>(), new Tuple<bool, int[]>(false, CalamityMod.iceEnemyImmunities) },
                { ModContent.NPCType<CryogenShield>(), new Tuple<bool, int[]>(false, CalamityMod.iceEnemyImmunities) },

                { ModContent.NPCType<AquaticScourgeHead>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<AquaticScourgeBody>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<AquaticScourgeBodyAlt>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<AquaticScourgeTail>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },

                { ModContent.NPCType<BrimstoneElemental>(), new Tuple<bool, int[]>(false, CalamityMod.cragEnemyImmunities) },
                { ModContent.NPCType<Brimling>(), new Tuple<bool, int[]>(false, CalamityMod.cragEnemyImmunities) },

                { ModContent.NPCType<CalamitasClone>(), new Tuple<bool, int[]>(false, CalamityMod.cragEnemyImmunities) },
                { ModContent.NPCType<Cataclysm>(), new Tuple<bool, int[]>(false, CalamityMod.cragEnemyImmunities) },
                { ModContent.NPCType<Catastrophe>(), new Tuple<bool, int[]>(false, CalamityMod.cragEnemyImmunities) },
                { ModContent.NPCType<SoulSeeker>(), new Tuple<bool, int[]>(false, CalamityMod.cragEnemyImmunities) },

                { NPCID.Plantera, new Tuple<bool, int[]>(false, new int[] { BuffID.Venom }) },
                { NPCID.PlanterasTentacle, new Tuple<bool, int[]>(false, new int[] { BuffID.Venom }) },
                { ModContent.NPCType<PlanterasFreeTentacle>(), new Tuple<bool, int[]>(false, new int[] { BuffID.Venom }) },

                { NPCID.HallowBoss, new Tuple<bool, int[]>(false, CalamityMod.holyEnemyImmunities) },

                // She resists the cold because of her ice-related abilities.
                { ModContent.NPCType<Anahita>(), new Tuple<bool, int[]>(false, CalamityMod.iceEnemyImmunities) },
                { ModContent.NPCType<AnahitasIceShield>(), new Tuple<bool, int[]>(false, CalamityMod.iceEnemyImmunities) },

                { ModContent.NPCType<AstrumAureus>(), new Tuple<bool, int[]>(false, CalamityMod.astralEnemyImmunities) },
                { ModContent.NPCType<AureusSpawn>(), new Tuple<bool, int[]>(false, CalamityMod.astralEnemyImmunities) },

                { ModContent.NPCType<PlaguebringerGoliath>(), new Tuple<bool, int[]>(false, CalamityMod.plagueEnemyImmunities) },
                { ModContent.NPCType<PlagueMine>(), new Tuple<bool, int[]>(false, CalamityMod.plagueEnemyImmunities) },
                { ModContent.NPCType<PlagueHomingMissile>(), new Tuple<bool, int[]>(false, CalamityMod.plagueEnemyImmunities) },

                { ModContent.NPCType<RavagerHead2>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<FlamePillar>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },

                { ModContent.NPCType<AstrumDeusHead>(), new Tuple<bool, int[]>(false, CalamityMod.astralEnemyImmunities) },
                { ModContent.NPCType<AstrumDeusBody>(), new Tuple<bool, int[]>(false, CalamityMod.astralEnemyImmunities) },
                { ModContent.NPCType<AstrumDeusTail>(), new Tuple<bool, int[]>(false, CalamityMod.astralEnemyImmunities) },

                { NPCID.MoonLordCore, new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<Nightwither>() }) },
                { NPCID.MoonLordHand, new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<Nightwither>() }) },
                { NPCID.MoonLordHead, new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<Nightwither>() }) },
                { NPCID.MoonLordLeechBlob, new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<Nightwither>() }) },

                { ModContent.NPCType<ProfanedGuardianCommander>(), new Tuple<bool, int[]>(false, CalamityMod.holyEnemyImmunities) },
                { ModContent.NPCType<ProfanedGuardianDefender>(), new Tuple<bool, int[]>(false, CalamityMod.holyEnemyImmunities) },
                { ModContent.NPCType<ProfanedGuardianHealer>(), new Tuple<bool, int[]>(false, CalamityMod.holyEnemyImmunities) },
                { ModContent.NPCType<ProfanedRocks>(), new Tuple<bool, int[]>(false, CalamityMod.holyEnemyImmunities) },

                { ModContent.NPCType<Providence>(), new Tuple<bool, int[]>(false, CalamityMod.holyEnemyImmunities) },
                { ModContent.NPCType<ProvSpawnOffense>(), new Tuple<bool, int[]>(false, CalamityMod.holyEnemyImmunities) },
                { ModContent.NPCType<ProvSpawnDefense>(), new Tuple<bool, int[]>(false, CalamityMod.holyEnemyImmunities) },
                { ModContent.NPCType<ProvSpawnHealer>(), new Tuple<bool, int[]>(false, CalamityMod.holyEnemyImmunities) },

                { ModContent.NPCType<CeaselessVoid>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<DarkEnergy>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },

                { ModContent.NPCType<StormWeaverHead>(), new Tuple<bool, int[]>(false, new int[] { BuffID.Electrified }) },
                { ModContent.NPCType<StormWeaverBody>(), new Tuple<bool, int[]>(false, new int[] { BuffID.Electrified }) },
                { ModContent.NPCType<StormWeaverTail>(), new Tuple<bool, int[]>(false, new int[] { BuffID.Electrified }) },

                { ModContent.NPCType<Signus>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<WhisperingDeath>() }) },
                { ModContent.NPCType<CosmicLantern>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<CosmicMine>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },

                { ModContent.NPCType<Polterghast>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<Nightwither>(), ModContent.BuffType<WhisperingDeath>() }) },
                { ModContent.NPCType<PolterPhantom>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<PhantomFuckYou>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<PolterghastHook>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },

                { ModContent.NPCType<OldDuke>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<OldDukeToothBall>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<SulphurousSharkron>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },

                { ModContent.NPCType<DevourerofGodsHead>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<DevourerofGodsBody>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<DevourerofGodsTail>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<CosmicGuardianHead>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<CosmicGuardianBody>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<CosmicGuardianTail>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },

                { ModContent.NPCType<Yharon>(), new Tuple<bool, int[]>(false, new int[] { BuffID.OnFire }) },

                { ModContent.NPCType<ThanatosHead>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<ThanatosBody1>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<ThanatosBody2>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<ThanatosTail>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },

                // 20FEB2023: Ozzatron: made Ares immune to Miracle Blight, because otherwise he vanishes when affected by it
                // The "correct" way to fix this involves a colossal amount of work and will be handled at a later date
                { ModContent.NPCType<AresBody>(), new Tuple<bool, int[]>(false, new int[] { ModContent.BuffType<MiracleBlight>() }) },

                { ModContent.NPCType<SupremeCalamitas>(), new Tuple<bool, int[]>(false, CalamityMod.cragEnemyImmunities) },
                { ModContent.NPCType<SupremeCatastrophe>(), new Tuple<bool, int[]>(false, CalamityMod.cragEnemyImmunities) },
                { ModContent.NPCType<SupremeCataclysm>(), new Tuple<bool, int[]>(false, CalamityMod.cragEnemyImmunities) },
                { ModContent.NPCType<SoulSeekerSupreme>(), new Tuple<bool, int[]>(false, CalamityMod.cragEnemyImmunities) },
                { ModContent.NPCType<BrimstoneHeart>(), new Tuple<bool, int[]>(false, CalamityMod.cragEnemyImmunities) },
                { ModContent.NPCType<SepulcherHead>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<SepulcherBody>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<SepulcherBodyEnergyBall>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<SepulcherTail>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },

                { ModContent.NPCType<PrimordialWyrmHead>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<PrimordialWyrmBody>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<PrimordialWyrmBodyAlt>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<PrimordialWyrmTail>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },

                { ModContent.NPCType<AcidEel>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<BloodwormFleeing>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<BloodwormNormal>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<CragmawMire>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<BabyFlakCrab>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<FlakCrab>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<GammaSlime>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<IrradiatedSlime>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<NuclearTerror>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<NuclearToad>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<Orthocera>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<Radiator>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<Skyfin>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<SulphurousSkater>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<Trilobite>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<AquaticUrchin>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<AnthozoanCrab>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<BelchingCoral>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<Toxicatfish>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<Sulflounder>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<Gnasher>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<Mauler>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<MicrobialCluster>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },
                { ModContent.NPCType<Trasher>(), new Tuple<bool, int[]>(false, CalamityMod.sulphurEnemyImmunities) },

                { ModContent.NPCType<BlindedAngler>(), new Tuple<bool, int[]>(false, CalamityMod.sunkenSeaEnemyImmunities) },
                { ModContent.NPCType<Clam>(), new Tuple<bool, int[]>(false, CalamityMod.sunkenSeaEnemyImmunities) },
                { ModContent.NPCType<EutrophicRay>(), new Tuple<bool, int[]>(false, CalamityMod.sunkenSeaEnemyImmunities) },
                { ModContent.NPCType<GhostBell>(), new Tuple<bool, int[]>(false, CalamityMod.sunkenSeaEnemyImmunities) },
                { ModContent.NPCType<GiantClam>(), new Tuple<bool, int[]>(false, CalamityMod.sunkenSeaEnemyImmunities) },
                { ModContent.NPCType<PrismBack>(), new Tuple<bool, int[]>(false, CalamityMod.sunkenSeaEnemyImmunities) },
                { ModContent.NPCType<SeaSerpent1>(), new Tuple<bool, int[]>(false, CalamityMod.sunkenSeaEnemyImmunities) },
                { ModContent.NPCType<SeaSerpent2>(), new Tuple<bool, int[]>(false, CalamityMod.sunkenSeaEnemyImmunities) },
                { ModContent.NPCType<SeaSerpent3>(), new Tuple<bool, int[]>(false, CalamityMod.sunkenSeaEnemyImmunities) },
                { ModContent.NPCType<SeaSerpent4>(), new Tuple<bool, int[]>(false, CalamityMod.sunkenSeaEnemyImmunities) },
                { ModContent.NPCType<SeaSerpent5>(), new Tuple<bool, int[]>(false, CalamityMod.sunkenSeaEnemyImmunities) },

                { ModContent.NPCType<Bloatfish>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<BobbitWormHead>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<BoxJellyfish>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<ChaoticPuffer>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<Cuttlefish>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<DevilFish>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<DevilFishAlt>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<GiantSquid>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<GulperEelHead>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<GulperEelBody>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<GulperEelBodyAlt>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<GulperEelTail>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<Laserfish>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<LuminousCorvina>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<MirageJelly>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<MorayEel>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<OarfishHead>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<OarfishBody>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<OarfishTail>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<ToxicMinnow>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<Viperfish>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<EidolonWyrmHead>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<EidolonWyrmBody>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<EidolonWyrmBodyAlt>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<EidolonWyrmTail>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<ColossalSquid>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },
                { ModContent.NPCType<ReaperShark>(), new Tuple<bool, int[]>(false, CalamityMod.abyssEnemyImmunities) },

                { ModContent.NPCType<HeatSpirit>(), new Tuple<bool, int[]>(false, CalamityMod.cragEnemyImmunities) },
                { ModContent.NPCType<Scryllar>(), new Tuple<bool, int[]>(false, CalamityMod.cragEnemyImmunities) },
                { ModContent.NPCType<ScryllarRage>(), new Tuple<bool, int[]>(false, CalamityMod.cragEnemyImmunities) },
                { ModContent.NPCType<DespairStone>(), new Tuple<bool, int[]>(false, CalamityMod.cragEnemyImmunities) },
                { ModContent.NPCType<InfernalCongealment>(), new Tuple<bool, int[]>(false, CalamityMod.cragEnemyImmunities) },
                { ModContent.NPCType<RenegadeWarlock>(), new Tuple<bool, int[]>(false, CalamityMod.cragEnemyImmunities) },
                { ModContent.NPCType<CalamityEye>(), new Tuple<bool, int[]>(false, CalamityMod.cragEnemyImmunities) },
                { ModContent.NPCType<SoulSlurper>(), new Tuple<bool, int[]>(false, CalamityMod.cragEnemyImmunities) },

                { ModContent.NPCType<Aries>(), new Tuple<bool, int[]>(false, CalamityMod.astralEnemyImmunities) },
                { ModContent.NPCType<AstralachneaGround>(), new Tuple<bool, int[]>(false, CalamityMod.astralEnemyImmunities) },
                { ModContent.NPCType<AstralachneaWall>(), new Tuple<bool, int[]>(false, CalamityMod.astralEnemyImmunities) },
                { ModContent.NPCType<AstralProbe>(), new Tuple<bool, int[]>(false, CalamityMod.astralEnemyImmunities) },
                { ModContent.NPCType<AstralSlime>(), new Tuple<bool, int[]>(false, CalamityMod.astralEnemyImmunities) },
                { ModContent.NPCType<Atlas>(), new Tuple<bool, int[]>(false, CalamityMod.astralEnemyImmunities) },
                { ModContent.NPCType<SightseerSpitter>(), new Tuple<bool, int[]>(false, CalamityMod.astralEnemyImmunities) },
                { ModContent.NPCType<FusionFeeder>(), new Tuple<bool, int[]>(false, CalamityMod.astralEnemyImmunities) },
                { ModContent.NPCType<Hadarian>(), new Tuple<bool, int[]>(false, CalamityMod.astralEnemyImmunities) },
                { ModContent.NPCType<HiveEnemy>(), new Tuple<bool, int[]>(false, CalamityMod.astralEnemyImmunities) },
                { ModContent.NPCType<Hiveling>(), new Tuple<bool, int[]>(false, CalamityMod.astralEnemyImmunities) },
                { ModContent.NPCType<Mantis>(), new Tuple<bool, int[]>(false, CalamityMod.astralEnemyImmunities) },
                { ModContent.NPCType<Nova>(), new Tuple<bool, int[]>(false, CalamityMod.astralEnemyImmunities) },
                { ModContent.NPCType<SightseerCollider>(), new Tuple<bool, int[]>(false, CalamityMod.astralEnemyImmunities) },
                { ModContent.NPCType<StellarCulex>(), new Tuple<bool, int[]>(false, CalamityMod.astralEnemyImmunities) },

                { ModContent.NPCType<Plagueshell>(), new Tuple<bool, int[]>(false, CalamityMod.plagueEnemyImmunities) },
                { ModContent.NPCType<Viruling>(), new Tuple<bool, int[]>(false, CalamityMod.plagueEnemyImmunities) },
                { ModContent.NPCType<Melter>(), new Tuple<bool, int[]>(false, CalamityMod.plagueEnemyImmunities) },
                { ModContent.NPCType<PestilentSlime>(), new Tuple<bool, int[]>(false, CalamityMod.plagueEnemyImmunities) },
                { ModContent.NPCType<PlagueChargerLarge>(), new Tuple<bool, int[]>(false, CalamityMod.plagueEnemyImmunities) },
                { ModContent.NPCType<PlagueCharger>(), new Tuple<bool, int[]>(false, CalamityMod.plagueEnemyImmunities) },
                { ModContent.NPCType<PlaguebringerMiniboss>(), new Tuple<bool, int[]>(false, CalamityMod.plagueEnemyImmunities) },

                { ModContent.NPCType<ScornEater>(), new Tuple<bool, int[]>(false, CalamityMod.holyEnemyImmunities) },
                { ModContent.NPCType<ImpiousImmolator>(), new Tuple<bool, int[]>(false, CalamityMod.holyEnemyImmunities) },
                { ModContent.NPCType<ProfanedEnergyBody>(), new Tuple<bool, int[]>(false, CalamityMod.holyEnemyImmunities) },
                { ModContent.NPCType<Sunskater>(), new Tuple<bool, int[]>(false, CalamityMod.holyEnemyImmunities) },

                { ModContent.NPCType<ArmoredDiggerHead>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<ArmoredDiggerBody>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<ArmoredDiggerTail>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },
                { ModContent.NPCType<Eidolist>(), new Tuple<bool, int[]>(true, Array.Empty<int>()) },

                { ModContent.NPCType<SeaUrchin>(), new Tuple<bool, int[]>(false, new int[] { BuffID.Poisoned, BuffID.Venom }) },
                { ModContent.NPCType<Frogfish>(), new Tuple<bool, int[]>(false, new int[] { BuffID.Poisoned, BuffID.Venom }) },

                { ModContent.NPCType<ThiccWaifu>(), new Tuple<bool, int[]>(false, new int[] { BuffID.Electrified }) },

                { ModContent.NPCType<CrimulanBlightSlime>(), new Tuple<bool, int[]>(false, CalamityMod.slimeEnemyImmunities) },
                { ModContent.NPCType<EbonianBlightSlime>(), new Tuple<bool, int[]>(false, CalamityMod.slimeEnemyImmunities) },

                { ModContent.NPCType<Rimehound>(), new Tuple<bool, int[]>(false, CalamityMod.iceEnemyImmunities) },
                { ModContent.NPCType<Cryon>(), new Tuple<bool, int[]>(false, CalamityMod.iceEnemyImmunities) },
                { ModContent.NPCType<CryoSlime>(), new Tuple<bool, int[]>(false, CalamityMod.iceEnemyImmunities) },
                { ModContent.NPCType<IceClasper>(), new Tuple<bool, int[]>(false, CalamityMod.iceEnemyImmunities) },
                { ModContent.NPCType<AuroraSpirit>(), new Tuple<bool, int[]>(false, CalamityMod.iceEnemyImmunities) }
            };
        }

        // Destroys the EnemyStats struct to save memory because mod assemblies will not be fully unloaded until TML 1.4.
        internal static void UnloadDebuffs()
        {
            EnemyStats.DebuffImmunities = null;
        }
        #endregion
    }
}
