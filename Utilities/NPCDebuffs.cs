using System;
using System.Collections.Generic;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AcidRain;
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
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.PrimordialWyrm;
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
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod
{
    public static partial class NPCStats
    {
        internal partial struct EnemyStats
        {
            public static SortedDictionary<int, Tuple<GeneralImmunityStatus, int[]>> DebuffImmunities;
        };

        internal enum GeneralImmunityStatus : int
        {
            None = 0,
            ImmuneToRegularBuffs = 1,
            ImmuneToAllBuffs = 2, // This case is rather extreme and it is unlikely Calamity will ever use it.
        };

        #region Stat Retrieval Methods
        // PORTING APOCRYPHA: 29SEP2023: Ozzatron: yeah okay this WHOLE SYSTEM has to go. NOW.
        public static void SetDebuffImmunities(this NPC npc)
        {
            // Safety check: If for some reason the debuff array is not initialized yet, return and do nothing.
            if (npc is null || EnemyStats.DebuffImmunities is null)
                return;

            //
            // PART 1: General Immunity Status and applying all data contained in EnemyStats.DebuffImmunities
            //
            
            // Also, can I just say that I hate Sorted Dictionaries and Tuples and want to make something explode? -Ben
            // I mean you can but sorted dicts and dicts in general are pre great -Amber
            // Neither of you are free of sin. In porting, we are all brothers in damnation. -Ozzatron
            bool hasEntry = EnemyStats.DebuffImmunities.TryGetValue(npc.type, out var buffSetTuple);

            if (hasEntry)
            {
                // Apply the NPC's General Immunity Status first.
                // General Immunity Status is used to make an NPC "immune to everything" by default.
                GeneralImmunityStatus gis = buffSetTuple.Item1;
                if (gis == GeneralImmunityStatus.ImmuneToRegularBuffs)
                    NPCID.Sets.ImmuneToRegularBuffs[npc.type] = true;
                else if (gis == GeneralImmunityStatus.ImmuneToAllBuffs)
                    NPCID.Sets.ImmuneToAllBuffs[npc.type] = true;

                // From here on out, all listed buff IDs are treated differently depending on what the General Immunity Status was.
                // If it was None, then the listed buffs are buffs that the NPC should be immune to.
                // If it was anything else, then the listed buffs are buffs that the NPC should be STILL VULNERABLE to (despite their General Immunity Status).
                bool providingExceptions = gis != GeneralImmunityStatus.None;
                for (int i = 0; i < buffSetTuple.Item2.Length; ++i)
                {
                    int buffID = buffSetTuple.Item2[i];
                    NPCID.Sets.SpecificDebuffImmunity[npc.type][buffID] = !providingExceptions;
                }
            }


            //
            // PART 2: Specific other cases that can't be neatly fit into the database
            //

            // All bosses and several enemies are automatically immune to Pearl Aura.
            if (CalamityLists.enemyImmunityList.Contains(npc.type) || npc.boss)
                NPCID.Sets.SpecificDebuffImmunity[npc.type][ModContent.BuffType<PearlAura>()] = true;

            // Make all Cal NPCs immune to confused unless otherwise specified
            // Extra note: Clams are not in this list as they initially immune to Confused, but are no longer immune once aggro'd. This is set in their AI().
            bool cal = npc.ModNPC != null && npc.ModNPC.Mod.Name.Equals(ModContent.GetInstance<CalamityMod>().Name);
            if (!CalamityLists.confusionEnemyList.Contains(npc.type) && cal)
                NPCID.Sets.SpecificDebuffImmunity[npc.type][BuffID.Confused] = true;

            // Sets certain vanilla NPCs and all town NPCs to be immune to most debuffs.
            bool isConsideredSpecialTarget = npc.type == NPCID.SkeletronHead || npc.type == NPCID.DD2EterniaCrystal;
            bool isTownNPC = npc.townNPC || NPCID.Sets.ActsLikeTownNPC[npc.type];
            bool applyRegularBuffImmunitySpecialCases = isConsideredSpecialTarget || isTownNPC;
            if (applyRegularBuffImmunitySpecialCases)
            {
                NPCID.Sets.ImmuneToRegularBuffs[npc.type] = true;

                // Town NPCs are kept vulnerable to cosmetic debuffs that are meant to affect them.
                if (isTownNPC)
                {
                    NPCID.Sets.SpecificDebuffImmunity[npc.type][BuffID.Wet] = false;
                    NPCID.Sets.SpecificDebuffImmunity[npc.type][BuffID.Slimed] = false;
                    NPCID.Sets.SpecificDebuffImmunity[npc.type][BuffID.Lovestruck] = false;
                    NPCID.Sets.SpecificDebuffImmunity[npc.type][BuffID.Stinky] = false;
                    NPCID.Sets.SpecificDebuffImmunity[npc.type][BuffID.GelBalloonBuff] = false;

                    // Additionally, "real" Town NPCs which transform in Shimmer are kept vulnerable to Shimmering.
                    if (npc.townNPC && NPCID.Sets.ShimmerTownTransform[npc.type])
                        NPCID.Sets.SpecificDebuffImmunity[npc.type][BuffID.Shimmer] = false;
                }
            }

            // Extra Notes:
            // Shellfish minions set debuff immunity to Shellfish Claps on enemy hits, so most things are technically not immune.
            // The Spiteful Candle sets the debuff immunity of Spite to all nearby enemies in the tile file for an enemy with less than 99% DR.
        }
        #endregion

        #region Standard Enemy Immunity Sets
        private static readonly int[] slimeEnemyImmunities = new int[1] { BuffID.Poisoned };
        private static readonly int[] iceEnemyImmunities = new int[3] { BuffID.Frostburn, BuffID.Frostburn2, ModContent.BuffType<GlacialState>() };
        private static readonly int[] sulphurEnemyImmunities = new int[4] { BuffID.Poisoned, BuffID.Venom, ModContent.BuffType<SulphuricPoisoning>(), ModContent.BuffType<Irradiated>() };
        private static readonly int[] sunkenSeaEnemyImmunities = new int[2] { ModContent.BuffType<Eutrophication>(), ModContent.BuffType<PearlAura>() };
        private static readonly int[] abyssEnemyImmunities = new int[2] { ModContent.BuffType<CrushDepth>(), ModContent.BuffType<RiptideDebuff>() };
        private static readonly int[] cragEnemyImmunities = new int[3] { BuffID.OnFire, BuffID.OnFire3, ModContent.BuffType<BrimstoneFlames>() };
        private static readonly int[] astralEnemyImmunities = new int[2] { BuffID.Poisoned, ModContent.BuffType<AstralInfectionDebuff>() };
        private static readonly int[] plagueEnemyImmunities = new int[3] { BuffID.Poisoned, BuffID.Venom, ModContent.BuffType<Plague>() };
        private static readonly int[] holyEnemyImmunities = new int[4] { BuffID.OnFire, BuffID.OnFire3, ModContent.BuffType<HolyFlames>(), ModContent.BuffType<Nightwither>() };
        #endregion

        #region Load/Unload
        // A static function, called exactly once, which initializes the EnemyStats struct at a predictable time.
        // This is necessary to ensure this dictionary is populated as early as possible.
        // Any boss not listed in here is only immune to Confusion and Pearl Aura.
        internal static void LoadDebuffs()
        {
            // Various shorthands for NPCs which have very simple and common buff immunity sets.
            Tuple<GeneralImmunityStatus, int[]> immuneToEverything = new(GeneralImmunityStatus.ImmuneToRegularBuffs, Array.Empty<int>());
            Tuple<GeneralImmunityStatus, int[]> immuneToEverythingIncludingTags = new(GeneralImmunityStatus.ImmuneToAllBuffs, Array.Empty<int>());
            Tuple<GeneralImmunityStatus, int[]> slime = new(GeneralImmunityStatus.None, slimeEnemyImmunities);
            Tuple<GeneralImmunityStatus, int[]> ice = new(GeneralImmunityStatus.None, iceEnemyImmunities);
            Tuple<GeneralImmunityStatus, int[]> sulphur = new(GeneralImmunityStatus.None, sulphurEnemyImmunities);
            Tuple<GeneralImmunityStatus, int[]> sunkenSea = new(GeneralImmunityStatus.None, sunkenSeaEnemyImmunities);
            Tuple<GeneralImmunityStatus, int[]> abyss = new(GeneralImmunityStatus.None, abyssEnemyImmunities);
            Tuple<GeneralImmunityStatus, int[]> crags = new(GeneralImmunityStatus.None, cragEnemyImmunities);
            Tuple<GeneralImmunityStatus, int[]> astral = new(GeneralImmunityStatus.None, astralEnemyImmunities);
            Tuple<GeneralImmunityStatus, int[]> plague = new(GeneralImmunityStatus.None, plagueEnemyImmunities);
            Tuple<GeneralImmunityStatus, int[]> holy = new(GeneralImmunityStatus.None, holyEnemyImmunities);

            // Is this sorted... like... at all??
            EnemyStats.DebuffImmunities = new SortedDictionary<int, Tuple<GeneralImmunityStatus, int[]>>
            {
                { ModContent.NPCType<KingSlimeJewel>(), immuneToEverything },

                { NPCID.Deerclops, ice },

                { ModContent.NPCType<SlimeGodCore>(), slime },
                { ModContent.NPCType<EbonianPaladin>(), slime },
                { ModContent.NPCType<SplitEbonianPaladin>(), slime },
                { ModContent.NPCType<CrimulanPaladin>(), slime },
                { ModContent.NPCType<SplitCrimulanPaladin>(), slime },
                { ModContent.NPCType<CorruptSlimeSpawn>(), slime },
                { ModContent.NPCType<CorruptSlimeSpawn2>(), slime },
                { ModContent.NPCType<CrimsonSlimeSpawn>(), slime },
                { ModContent.NPCType<CrimsonSlimeSpawn2>(), slime },

                { ModContent.NPCType<Cryogen>(), ice },
                { ModContent.NPCType<CryogenShield>(), ice },

                { ModContent.NPCType<AquaticScourgeHead>(), sulphur },
                { ModContent.NPCType<AquaticScourgeBody>(), sulphur },
                { ModContent.NPCType<AquaticScourgeBodyAlt>(), sulphur },
                { ModContent.NPCType<AquaticScourgeTail>(), sulphur },

                { ModContent.NPCType<BrimstoneElemental>(), crags },
                { ModContent.NPCType<Brimling>(), crags },

                { ModContent.NPCType<CalamitasClone>(), crags },
                { ModContent.NPCType<Cataclysm>(), crags },
                { ModContent.NPCType<Catastrophe>(), crags },
                { ModContent.NPCType<SoulSeeker>(), crags },

                { NPCID.Plantera, new(GeneralImmunityStatus.None, new int[] { BuffID.Venom }) },
                { NPCID.PlanterasTentacle, new(GeneralImmunityStatus.None, new int[] { BuffID.Venom }) },
                { ModContent.NPCType<PlanterasFreeTentacle>(), new(GeneralImmunityStatus.None, new int[] { BuffID.Venom }) },

                { NPCID.HallowBoss, holy },

                // She resists the cold because of her ice-related abilities.
                { ModContent.NPCType<Anahita>(), ice },
                { ModContent.NPCType<AnahitasIceShield>(), ice },

                { ModContent.NPCType<AstrumAureus>(), astral },
                { ModContent.NPCType<AureusSpawn>(), astral },

                { ModContent.NPCType<PlaguebringerGoliath>(), plague },
                { ModContent.NPCType<PlagueMine>(), plague },
                { ModContent.NPCType<PlagueHomingMissile>(), plague },

                { ModContent.NPCType<RavagerHead2>(), immuneToEverything },
                { ModContent.NPCType<FlamePillar>(), immuneToEverything },

                { ModContent.NPCType<AstrumDeusHead>(), astral },
                { ModContent.NPCType<AstrumDeusBody>(), astral },
                { ModContent.NPCType<AstrumDeusTail>(), astral },

                { NPCID.MoonLordCore, new(GeneralImmunityStatus.None, new int[] { ModContent.BuffType<Nightwither>() }) },
                { NPCID.MoonLordHand, new(GeneralImmunityStatus.None, new int[] { ModContent.BuffType<Nightwither>() }) },
                { NPCID.MoonLordHead, new(GeneralImmunityStatus.None, new int[] { ModContent.BuffType<Nightwither>() }) },
                { NPCID.MoonLordLeechBlob, new(GeneralImmunityStatus.None, new int[] { ModContent.BuffType<Nightwither>() }) },

                { ModContent.NPCType<ProfanedGuardianCommander>(), holy },
                { ModContent.NPCType<ProfanedGuardianDefender>(), holy },
                { ModContent.NPCType<ProfanedGuardianHealer>(), holy },
                { ModContent.NPCType<ProfanedRocks>(), holy },

                { ModContent.NPCType<Providence>(), holy },
                { ModContent.NPCType<ProvSpawnOffense>(), holy },
                { ModContent.NPCType<ProvSpawnDefense>(), holy },
                { ModContent.NPCType<ProvSpawnHealer>(), holy },

                { ModContent.NPCType<CeaselessVoid>(), immuneToEverything },
                { ModContent.NPCType<DarkEnergy>(), immuneToEverything },

                { ModContent.NPCType<StormWeaverHead>(), new(GeneralImmunityStatus.None, new int[] { BuffID.Electrified }) },
                { ModContent.NPCType<StormWeaverBody>(), new(GeneralImmunityStatus.None, new int[] { BuffID.Electrified }) },
                { ModContent.NPCType<StormWeaverTail>(), new(GeneralImmunityStatus.None, new int[] { BuffID.Electrified }) },

                { ModContent.NPCType<Signus>(), new(GeneralImmunityStatus.None, new int[] { ModContent.BuffType<WhisperingDeath>() }) },
                { ModContent.NPCType<CosmicLantern>(), immuneToEverything },
                { ModContent.NPCType<CosmicMine>(), immuneToEverything },

                { ModContent.NPCType<Polterghast>(), new(GeneralImmunityStatus.None, new int[] { ModContent.BuffType<Nightwither>(), ModContent.BuffType<WhisperingDeath>() }) },
                { ModContent.NPCType<PolterPhantom>(), immuneToEverything },
                { ModContent.NPCType<PhantomFuckYou>(), immuneToEverything },
                { ModContent.NPCType<PolterghastHook>(), immuneToEverything },

                { ModContent.NPCType<OldDuke>(), sulphur },
                { ModContent.NPCType<OldDukeToothBall>(), sulphur },
                { ModContent.NPCType<SulphurousSharkron>(), sulphur },

                { ModContent.NPCType<DevourerofGodsHead>(), immuneToEverything },
                { ModContent.NPCType<DevourerofGodsBody>(), immuneToEverything },
                { ModContent.NPCType<DevourerofGodsTail>(), immuneToEverything },
                { ModContent.NPCType<CosmicGuardianHead>(), immuneToEverything },
                { ModContent.NPCType<CosmicGuardianBody>(), immuneToEverything },
                { ModContent.NPCType<CosmicGuardianTail>(), immuneToEverything },

                { ModContent.NPCType<Yharon>(), new(GeneralImmunityStatus.None, new int[] { BuffID.OnFire, ModContent.BuffType<Dragonfire>() }) },

                { ModContent.NPCType<ThanatosHead>(), immuneToEverything },
                { ModContent.NPCType<ThanatosBody1>(), immuneToEverything },
                { ModContent.NPCType<ThanatosBody2>(), immuneToEverything },
                { ModContent.NPCType<ThanatosTail>(), immuneToEverything },

                { ModContent.NPCType<SupremeCalamitas>(), crags },
                { ModContent.NPCType<SupremeCatastrophe>(), crags },
                { ModContent.NPCType<SupremeCataclysm>(), crags },
                { ModContent.NPCType<SoulSeekerSupreme>(), crags },
                { ModContent.NPCType<BrimstoneHeart>(), crags },
                { ModContent.NPCType<SepulcherHead>(), immuneToEverything },
                { ModContent.NPCType<SepulcherBody>(), immuneToEverything },
                { ModContent.NPCType<SepulcherBodyEnergyBall>(), immuneToEverything },
                { ModContent.NPCType<SepulcherTail>(), immuneToEverything },

                { ModContent.NPCType<PrimordialWyrmHead>(), abyss },
                { ModContent.NPCType<PrimordialWyrmBody>(), immuneToEverything },
                { ModContent.NPCType<PrimordialWyrmBodyAlt>(), immuneToEverything },
                { ModContent.NPCType<PrimordialWyrmTail>(), immuneToEverything },

                { ModContent.NPCType<AcidEel>(), sulphur },
                { ModContent.NPCType<BloodwormFleeing>(), sulphur },
                { ModContent.NPCType<BloodwormNormal>(), sulphur },
                { ModContent.NPCType<CragmawMire>(), sulphur },
                { ModContent.NPCType<BabyFlakCrab>(), sulphur },
                { ModContent.NPCType<FlakCrab>(), sulphur },
                { ModContent.NPCType<GammaSlime>(), sulphur },
                { ModContent.NPCType<IrradiatedSlime>(), sulphur },
                { ModContent.NPCType<NuclearTerror>(), sulphur },
                { ModContent.NPCType<NuclearToad>(), sulphur },
                { ModContent.NPCType<Orthocera>(), sulphur },
                { ModContent.NPCType<Radiator>(), sulphur },
                { ModContent.NPCType<Skyfin>(), sulphur },
                { ModContent.NPCType<SulphurousSkater>(), sulphur },
                { ModContent.NPCType<Trilobite>(), sulphur },
                { ModContent.NPCType<AquaticUrchin>(), sulphur },
                { ModContent.NPCType<AnthozoanCrab>(), sulphur },
                { ModContent.NPCType<BelchingCoral>(), sulphur },
                { ModContent.NPCType<Toxicatfish>(), sulphur },
                { ModContent.NPCType<Sulflounder>(), sulphur },
                { ModContent.NPCType<Gnasher>(), sulphur },
                { ModContent.NPCType<Mauler>(), sulphur },
                { ModContent.NPCType<MicrobialCluster>(), sulphur },
                { ModContent.NPCType<Trasher>(), sulphur },

                { ModContent.NPCType<BlindedAngler>(), sunkenSea },
                { ModContent.NPCType<Clam>(), sunkenSea },
                { ModContent.NPCType<EutrophicRay>(), sunkenSea },
                { ModContent.NPCType<GhostBell>(), sunkenSea },
                { ModContent.NPCType<GiantClam>(), sunkenSea },
                { ModContent.NPCType<PrismBack>(), sunkenSea },
                { ModContent.NPCType<SeaSerpent1>(), sunkenSea },
                { ModContent.NPCType<SeaSerpent2>(), sunkenSea },
                { ModContent.NPCType<SeaSerpent3>(), sunkenSea },
                { ModContent.NPCType<SeaSerpent4>(), sunkenSea },
                { ModContent.NPCType<SeaSerpent5>(), sunkenSea },

                { ModContent.NPCType<BabyCannonballJellyfish>(), abyss },
                { ModContent.NPCType<CannonballJellyfish>(), abyss },
                { ModContent.NPCType<Bloatfish>(), abyss },
                { ModContent.NPCType<BobbitWormHead>(), abyss },
                { ModContent.NPCType<BoxJellyfish>(), abyss },
                { ModContent.NPCType<ChaoticPuffer>(), abyss },
                { ModContent.NPCType<Cuttlefish>(), abyss },
                { ModContent.NPCType<DevilFish>(), abyss },
                { ModContent.NPCType<DevilFishAlt>(), abyss },
                { ModContent.NPCType<GiantSquid>(), abyss },
                { ModContent.NPCType<GulperEelHead>(), abyss },
                { ModContent.NPCType<GulperEelBody>(), abyss },
                { ModContent.NPCType<GulperEelBodyAlt>(), abyss },
                { ModContent.NPCType<GulperEelTail>(), abyss },
                { ModContent.NPCType<Laserfish>(), abyss },
                { ModContent.NPCType<LuminousCorvina>(), abyss },
                { ModContent.NPCType<MirageJelly>(), abyss },
                { ModContent.NPCType<MorayEel>(), abyss },
                { ModContent.NPCType<OarfishHead>(), abyss },
                { ModContent.NPCType<OarfishBody>(), abyss },
                { ModContent.NPCType<OarfishTail>(), abyss },
                { ModContent.NPCType<ToxicMinnow>(), abyss },
                { ModContent.NPCType<Viperfish>(), abyss },
                { ModContent.NPCType<EidolonWyrmHead>(), abyss },
                { ModContent.NPCType<EidolonWyrmBody>(), immuneToEverything },
                { ModContent.NPCType<EidolonWyrmBodyAlt>(), immuneToEverything },
                { ModContent.NPCType<EidolonWyrmTail>(), immuneToEverything },
                { ModContent.NPCType<ColossalSquid>(), abyss },
                { ModContent.NPCType<ReaperShark>(), abyss },

                { ModContent.NPCType<HeatSpirit>(), crags },
                { ModContent.NPCType<Scryllar>(), crags },
                { ModContent.NPCType<ScryllarRage>(), crags },
                { ModContent.NPCType<DespairStone>(), crags },
                { ModContent.NPCType<InfernalCongealment>(), crags },
                { ModContent.NPCType<RenegadeWarlock>(), crags },
                { ModContent.NPCType<CalamityEye>(), crags },
                { ModContent.NPCType<SoulSlurper>(), crags },

                { ModContent.NPCType<Aries>(), astral },
                { ModContent.NPCType<AstralachneaGround>(), astral },
                { ModContent.NPCType<AstralachneaWall>(), astral },
                { ModContent.NPCType<AstralProbe>(), astral },
                { ModContent.NPCType<AstralSlime>(), astral },
                { ModContent.NPCType<Atlas>(), astral },
                { ModContent.NPCType<SightseerSpitter>(), astral },
                { ModContent.NPCType<FusionFeeder>(), astral },
                { ModContent.NPCType<Hadarian>(), astral },
                { ModContent.NPCType<HiveEnemy>(), astral },
                { ModContent.NPCType<Hiveling>(), astral },
                { ModContent.NPCType<Mantis>(), astral },
                { ModContent.NPCType<Nova>(), astral },
                { ModContent.NPCType<SightseerCollider>(), astral },
                { ModContent.NPCType<StellarCulex>(), astral },

                { ModContent.NPCType<Plagueshell>(), plague },
                { ModContent.NPCType<Viruling>(), plague },
                { ModContent.NPCType<Melter>(), plague },
                { ModContent.NPCType<PestilentSlime>(), plague },
                { ModContent.NPCType<PlagueChargerLarge>(), plague },
                { ModContent.NPCType<PlagueCharger>(), plague },
                { ModContent.NPCType<PlaguebringerMiniboss>(), plague },

                { ModContent.NPCType<ScornEater>(), holy },
                { ModContent.NPCType<ImpiousImmolator>(), holy },
                { ModContent.NPCType<ProfanedEnergyBody>(), holy },
                { ModContent.NPCType<Sunskater>(), holy },

                { ModContent.NPCType<ArmoredDiggerHead>(), immuneToEverything },
                { ModContent.NPCType<ArmoredDiggerBody>(), immuneToEverything },
                { ModContent.NPCType<ArmoredDiggerTail>(), immuneToEverything },
                { ModContent.NPCType<Eidolist>(), immuneToEverything },

                { ModContent.NPCType<SeaUrchin>(), new(GeneralImmunityStatus.None, new int[] { BuffID.Poisoned, BuffID.Venom }) },
                { ModContent.NPCType<Frogfish>(), new(GeneralImmunityStatus.None, new int[] { BuffID.Poisoned, BuffID.Venom }) },

                { ModContent.NPCType<ThiccWaifu>(), new(GeneralImmunityStatus.None, new int[] { BuffID.Electrified }) },

                { ModContent.NPCType<CrimulanBlightSlime>(), slime },
                { ModContent.NPCType<EbonianBlightSlime>(), slime },

                { ModContent.NPCType<Rimehound>(), ice },
                { ModContent.NPCType<Cryon>(), ice },
                { ModContent.NPCType<CryoSlime>(), ice },
                { ModContent.NPCType<IceClasper>(), ice },
                { ModContent.NPCType<AuroraSpirit>(), ice }
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
