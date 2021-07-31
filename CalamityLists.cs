using CalamityMod.Buffs.Alcohol;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Ammo.FiniteUse;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.Fishing.SunkenSeaCatches;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Tools;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
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
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.GreatSandShark;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.Projectiles.Hybrid;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Melee.Spears;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Tiles.LivingFire;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod
{
    public class CalamityLists
    {
        public static IList<string> donatorList;
        public static List<int> trueMeleeProjectileList; // DO NOT, EVER, DELETE THIS LIST, OR I WILL COME FOR YOU :D
        public static List<int> rangedProjectileExceptionList;
        public static List<int> projectileDestroyExceptionList;
        public static List<int> projectileMinionList;
        public static List<int> enemyImmunityList;
        public static List<int> confusionEnemyList;
        public static List<int> dungeonEnemyBuffList;
        public static List<int> dungeonProjectileBuffList;
        public static List<int> bossHPScaleList;
        public static List<int> beeEnemyList;
        public static List<int> beeProjectileList;
        public static List<int> friendlyBeeList;
        public static List<int> hardModeNerfList;
        public static List<int> debuffList;
        public static List<int> fireWeaponList;
        public static List<int> iceWeaponList;
        public static List<int> natureWeaponList;
        public static List<int> alcoholList;
        public static List<int> useTurnList;
        public static List<int> twentyUseTimeBuffList; //20% use time buff
        public static List<int> fiftySizeBuffList; //50% size buff
        public static List<int> quadrupleDamageBuffList; //300% buff
        public static List<int> tripleDamageBuffList; //200% buff
        public static List<int> doubleDamageBuffList; //100% buff
        public static List<int> sixtySixDamageBuffList; //66% buff
        public static List<int> fiftyDamageBuffList; //50% buff
        public static List<int> thirtyThreeDamageBuffList; //33% buff
        public static List<int> twentyFiveDamageBuffList; //25% buff
        public static List<int> twentyDamageBuffList; //20% buff
        public static List<int> tenDamageBuffList; //10% buff
        public static List<int> weaponAutoreuseList;
        public static List<int> spearAutoreuseList;
        public static List<int> tenDamageNerfList; //10% nerf
        public static List<int> quarterDamageNerfList; //25% nerf
        public static List<int> pumpkinMoonBuffList;
        public static List<int> frostMoonBuffList;
        public static List<int> eclipseBuffList;
        public static List<int> eventProjectileBuffList;
        public static List<int> revengeanceEnemyBuffList25Percent;
        public static List<int> revengeanceEnemyBuffList20Percent;
        public static List<int> revengeanceEnemyBuffList15Percent;
        public static List<int> revengeanceEnemyBuffList10Percent;
        public static List<int> revengeanceProjectileBuffList25Percent;
        public static List<int> revengeanceProjectileBuffList20Percent;
        public static List<int> revengeanceProjectileBuffList15Percent;
        public static List<int> revengeanceLifeStealExceptionList;
        public static List<int> noRageWormSegmentList;
        public static List<int> needsDebuffIconDisplayList;
        public static List<int> trapProjectileList;
        public static List<int> scopedWeaponList;
        public static List<int> boomerangList;
        public static List<int> javelinList;
        public static List<int> daggerList;
        public static List<int> flaskBombList;
        public static List<int> spikyBallList;
        public static List<int> boomerangProjList;
        public static List<int> javelinProjList;
        public static List<int> daggerProjList;
        public static List<int> flaskBombProjList;
        public static List<int> spikyBallProjList;
        public static List<int> noGravityList;
        public static List<int> lavaFishList;
        public static List<int> highTestFishList;
        public static List<int> flamethrowerList;
        public static List<int> forceItemList;
        public static List<int> livingFireBlockList;

        public static List<int> zombieList;
        public static List<int> demonEyeList;
        public static List<int> skeletonList;
        public static List<int> angryBonesList;
        public static List<int> hornetList;
        public static List<int> mossHornetList;
        public static List<int> bossMinionList;
        public static List<int> minibossList;
		public static List<int> heartDropBlockList;
		public static List<int> pierceResistList;

        public static List<int> legOverrideList;

        public static List<int> kamiDebuffColorImmuneList;

        public static List<int> MinionsToNotResurrectList;
        public static List<int> ZeroMinionSlotExceptionList;
        public static List<int> DontCopyOriginalMinionAIList;

        public static void LoadLists()
        {
            donatorList = new List<string>()
            {
                "Vorbis",
                "SoloMael",
                "Chaotic Reks",
                "The Buildmonger",
                "Yuh",
                "Littlepiggy",
                "LompL",
                "Lilith Saintclaire",
                "Ben Shapiro",
                "Frederik Henschel",
                "Faye",
                "Gibb50",
                "Braden Hajer",
                "Hannes Holmlund",
                "profoundmango69",
                "Jack M Sargent",
                "Hans Volter",
                "Krankwagon",
                "MishiroUsui",
                "pixlgray",
                "Arkhine",
                "Lodude",
                "DevAesthetic",
                "Mister Winchester",
                "Zacky",
                "Veine",
                "Javyz",
                "Shifter",
                "Crysthamyr",
                "Elfinlocks",
                "Ein",
                "2Larry2",
                "Jenonen",
                "Dodu",
                "Arti",
                "Tervastator",
                "Luis Arguello",
                "Alexander Davis",
                "BakaQing",
                "Laura Coonrod",
                "Xaphlactus",
                "MajinBagel",
                "Bendy",
                "Rando Calrissian",
                "Tails the Fox 92",
                "Bread",
                "Minty Candy",
                "Preston Card",
                "MovingTarget_086",
                "Shiro",
                "Chip",
                "Taylor Riverpaw",
                "ShotgunAngel",
                "Sandblast",
                "ThomasThePencil",
                "Aero (Aero#4599)",
                "GlitchOut",
                "Daawnz",
                "CrabBar",
                "Yatagarasu",
                "Jarod Isaac Gordon",
                "Zombieh",
                "MingWhy",
                "Random Weeb",
                "Ahmed Fahad Zamel Al Sharif",
                "Eragon3942",
                "TheBlackHand",
                "william",
                "Samuel Foreman",
                "Christopher Pham",
                "DemoN K!ng",
                "Malik Ciaramella",
                "Ryan Baker-Ortiz",
                "Aleksanders Denisovs",
                "TheSilverGhost",
                "Lucazii",
                "Shay",
                "Prism",
                "BobIsNotMyRealName",
                "Guwahavel",
                "Azura",
                "Joshua Miranda",
                "Doveda",
                "William Chang",
                "Arche",
                "DevilSunrise",
                "Yanmei",
                "Chaos",
                "Ryan Tucker",
                "Fish Repairs",
                "Melvin Brouwers",
                "Vroomy Has -3,000 IQ",
                "The Goliath",
                "DaPyRo",
                "Takeru",
                "Circuit-Jay",
                "Commmander Frostbite",
                "cytokat",
                "Cameron Fowlks",
                "Orudeon",
                "BumbleDoge",
                "John Ballard",
                "Naglfar",
                "Helixas",
                "Vetus Dea",
                "High Charity",
                "Devonte Plati",
                "Cerberus",
                "Brendan Kendall",
                "Victor Pittman",
                "KAT-G307",
                "Tombarry Expresserino",
                "Drip Veezy",
                "Glaid",
                "Apotheosis",
                "Bladesaber",
                "Devon Leigh",
                "Ruthoranium",
                "cocodezi_",
                "Mendzey",
                "GameRDheAsianSandwich",
                "Tobias",
                "Streakist .",
                "Eisaya A Cook",
                "Xenocrona",
                "RKMoon"
            };

            trueMeleeProjectileList = new List<int>()
            {
                // Vanilla shit
                ProjectileID.Spear,
                ProjectileID.Trident,
                ProjectileID.TheRottedFork,
                ProjectileID.Swordfish,
                ProjectileID.Arkhalis,
                ProjectileID.DarkLance,
                ProjectileID.CobaltNaginata,
                ProjectileID.PalladiumPike,
                ProjectileID.MythrilHalberd,
                ProjectileID.OrichalcumHalberd,
                ProjectileID.AdamantiteGlaive,
                ProjectileID.TitaniumTrident,
                ProjectileID.MushroomSpear,
                ProjectileID.Gungnir,
                ProjectileID.ObsidianSwordfish,
                ProjectileID.MonkStaffT1,
                ProjectileID.MonkStaffT2,

                // Tools
                ProjectileID.CobaltDrill,
                ProjectileID.MythrilDrill,
                ProjectileID.AdamantiteDrill,
                ProjectileID.PalladiumDrill,
                ProjectileID.OrichalcumDrill,
                ProjectileID.TitaniumDrill,
                ProjectileID.ChlorophyteDrill,
                ProjectileID.CobaltChainsaw,
                ProjectileID.MythrilChainsaw,
                ProjectileID.AdamantiteChainsaw,
                ProjectileID.PalladiumChainsaw,
                ProjectileID.OrichalcumChainsaw,
                ProjectileID.TitaniumChainsaw,
                ProjectileID.ChlorophyteChainsaw,
                ProjectileID.VortexDrill,
                ProjectileID.VortexChainsaw,
                ProjectileID.NebulaDrill,
                ProjectileID.NebulaChainsaw,
                ProjectileID.SolarFlareDrill,
                ProjectileID.SolarFlareChainsaw,
                ProjectileID.StardustDrill,
                ProjectileID.StardustChainsaw,
                ProjectileID.Hamdrax,
                ProjectileID.ChlorophyteJackhammer,
                ProjectileID.SawtoothShark,
                ProjectileID.ButchersChainsaw,

                // Calamity shit
                ProjectileType<DevilsSunriseProj>(),
                ProjectileType<MarniteObliteratorProj>(),
                ProjectileType<MurasamaSlash>(),
                ProjectileType<AstralPikeProj>(),
                ProjectileType<BrimlanceProj>(),
                ProjectileType<DiseasedPikeSpear>(),
                ProjectileType<ExsanguinationLanceProjectile>(),
                ProjectileType<FulgurationHalberdProj>(),
                ProjectileType<GildedProboscisProj>(),
                ProjectileType<MarniteSpearProjectile>(),
                ProjectileType<SausageMakerSpear>(),
                ProjectileType<YateveoBloomSpear>(),
                ProjectileType<HydraulicVoltCrasherProjectile>(),
                ProjectileType<DragonRageStaff>(),
                ProjectileType<TaintedBladeSlasher>()
            };

            rangedProjectileExceptionList = new List<int>()
            {
                ProjectileID.Phantasm,
                ProjectileID.VortexBeater,
                ProjectileID.DD2PhoenixBow,
                ProjectileID.IchorDart,
                ProjectileID.PhantasmArrow,
                ProjectileID.RainbowBack,
                ProjectileType<PhangasmBow>(),
                ProjectileType<ContagionBow>(),
                ProjectileType<DaemonsFlameBow>(),
                ProjectileType<ExoTornado>(),
                ProjectileType<DrataliornusBow>(),
                ProjectileType<FlakKrakenGun>(),
                ProjectileType<ButcherGun>(),
                ProjectileType<StarfleetMK2Gun>(),
                ProjectileType<TerraBulletSplit>(),
                ProjectileType<TerraArrowSplit>(),
                ProjectileType<HyperiusSplit>(),
                ProjectileType<NorfleetCannon>(),
                ProjectileType<NorfleetComet>(),
                ProjectileType<NorfleetExplosion>(),
                ProjectileType<AetherBeam>(),
                ProjectileType<FlurrystormCannonShooting>(),
                ProjectileType<MagnomalyBeam>(),
                ProjectileType<MagnomalyAura>(),
                ProjectileType<RainbowTrail>(),
                ProjectileType<PrismaticBeam>(),
                ProjectileType<ExoLight>(),
                ProjectileType<ExoLightBomb>(),
                ProjectileType<UltimaBowProjectile>(),
                ProjectileType<UltimaSpark>(), // Because of potential dust lag.
                ProjectileType<UltimaRay>()
            };

            projectileDestroyExceptionList = new List<int>()
            {
                //holdout projectiles
                ProjectileID.Phantasm,
                ProjectileID.VortexBeater,
                ProjectileID.DD2PhoenixBow,
                ProjectileID.LastPrism,
                ProjectileID.LastPrismLaser,
                ProjectileID.LaserMachinegun,
                ProjectileID.ChargedBlasterCannon,
                ProjectileID.MedusaHead,

                ProjectileType<PhangasmBow>(),
                ProjectileType<ContagionBow>(),
                ProjectileType<DaemonsFlameBow>(),
                ProjectileType<DrataliornusBow>(),
                ProjectileType<FlakKrakenGun>(),
                ProjectileType<ButcherGun>(),
                ProjectileType<StarfleetMK2Gun>(),
                ProjectileType<NorfleetCannon>(),
                ProjectileType<FlurrystormCannonShooting>(),

                ProjectileType<PurgeProj>(),
                ProjectileType<T1000Proj>(),
                ProjectileType<YharimsCrystalPrism>(),
                ProjectileType<DarkSparkPrism>(),
                ProjectileType<YharimsCrystalBeam>(),
                ProjectileType<DarkSparkBeam>(),
                ProjectileType<GhastlyVisageProj>(),
                ProjectileType<ApotheosisWorm>(),

                ProjectileType<FlakKrakenProj>(),
                ProjectileType<InfernadoFriendly>(),
                ProjectileType<DragonRageStaff>(),
                ProjectileType<MurasamaSlash>(),
                ProjectileType<PhaseslayerProjectile>(),
                ProjectileType<TaintedBladeSlasher>(),
                ProjectileType<PhotonRipperProjectile>(),
                ProjectileType<SpineOfThanatosProjectile>(),

                ProjectileType<SylvanSlashAttack>(),
                ProjectileType<FinalDawnProjectile>(),
                ProjectileType<FinalDawnThrow>(),
                ProjectileType<FinalDawnHorizontalSlash>(),
                ProjectileType<FinalDawnFireSlash>(),

                //Some hostile boss projectiles
                ProjectileType<BrimstoneMonster>(),
                ProjectileType<InfernadoRevenge>(),
                ProjectileType<OverlyDramaticDukeSummoner>(),
                ProjectileType<ProvidenceHolyRay>(),
                ProjectileType<OldDukeVortex>(),
                ProjectileType<BrimstoneRay>(),
                ProjectileType<BrimstoneTargetRay>()
            };

            projectileMinionList = new List<int>()
            {
                ProjectileID.PygmySpear,
                ProjectileID.UFOMinion,
                ProjectileID.UFOLaser,
                ProjectileID.StardustCellMinionShot,
                ProjectileID.MiniSharkron,
                ProjectileID.MiniRetinaLaser,
                ProjectileID.ImpFireball,
                ProjectileID.HornetStinger,
                ProjectileID.DD2FlameBurstTowerT1Shot,
                ProjectileID.DD2FlameBurstTowerT2Shot,
                ProjectileID.DD2FlameBurstTowerT3Shot,
                ProjectileID.DD2BallistraProj,
                ProjectileID.DD2ExplosiveTrapT1Explosion,
                ProjectileID.DD2ExplosiveTrapT2Explosion,
                ProjectileID.DD2ExplosiveTrapT3Explosion,
                ProjectileID.SpiderEgg,
                ProjectileID.BabySpider,
                ProjectileID.FrostBlastFriendly,
                ProjectileID.MoonlordTurretLaser,
                ProjectileID.RainbowCrystalExplosion
            };

            enemyImmunityList = new List<int>()
            {
                NPCID.KingSlime,
                NPCID.EaterofWorldsHead,
                NPCID.EaterofWorldsBody,
                NPCID.EaterofWorldsTail,
                NPCID.BrainofCthulhu,
                NPCID.Creeper,
                NPCID.EyeofCthulhu,
                NPCID.QueenBee,
                NPCID.SkeletronHead,
                NPCID.SkeletronHand,
                NPCID.WallofFlesh,
                NPCID.WallofFleshEye,
				NPCID.PirateShipCannon,
				NPCID.Probe,
                NPCID.Retinazer,
                NPCID.Spazmatism,
                NPCID.SkeletronPrime,
                NPCID.PrimeCannon,
                NPCID.PrimeSaw,
                NPCID.PrimeLaser,
                NPCID.PrimeVice,
                NPCID.Plantera,
				NPCID.PlanterasTentacle,
				NPCID.Everscream,
				NPCID.SantaNK1,
                NPCID.IceQueen,
				NPCID.MourningWood,
                NPCID.Pumpking,
                NPCID.Mothron,
                NPCID.Golem,
                NPCID.GolemHead,
                NPCID.GolemHeadFree,
                NPCID.GolemFistRight,
                NPCID.GolemFistLeft,
				NPCID.MartianSaucerCore,
				NPCID.MartianSaucerCannon,
				NPCID.MartianSaucerTurret,
                NPCID.DukeFishron,
                NPCID.Sharkron,
                NPCID.Sharkron2,
                NPCID.CultistBoss,
				NPCID.CultistDragonHead,
				NPCID.CultistDragonBody1,
				NPCID.CultistDragonBody2,
				NPCID.CultistDragonBody3,
				NPCID.CultistDragonBody4,
				NPCID.CultistDragonTail,
				NPCID.AncientCultistSquidhead,
				NPCID.MoonLordHead,
                NPCID.MoonLordHand,
                NPCID.MoonLordCore,
                NPCID.MoonLordFreeEye,
                NPCID.DD2WyvernT1,
                NPCID.DD2WyvernT2,
                NPCID.DD2WyvernT3,
                NPCID.DD2DarkMageT1,
                NPCID.DD2DarkMageT3,
                NPCID.DD2SkeletonT1,
                NPCID.DD2SkeletonT3,
                NPCID.DD2WitherBeastT2,
                NPCID.DD2WitherBeastT3,
                NPCID.DD2DrakinT2,
                NPCID.DD2DrakinT3,
                NPCID.DD2KoboldWalkerT2,
                NPCID.DD2KoboldWalkerT3,
                NPCID.DD2KoboldFlyerT2,
                NPCID.DD2KoboldFlyerT3,
                NPCID.DD2OgreT2,
                NPCID.DD2OgreT3,
                NPCID.DD2Betsy,
                NPCType<DesertScourgeHeadSmall>(),
                NPCType<DesertScourgeBodySmall>(),
                NPCType<DesertScourgeTailSmall>(),
                NPCType<GiantClam>(),
                NPCType<PerforatorHeadLarge>(),
                NPCType<PerforatorHeadMedium>(),
                NPCType<PerforatorHeadSmall>(),
                NPCType<PerforatorBodyLarge>(),
                NPCType<PerforatorBodyMedium>(),
                NPCType<PerforatorBodySmall>(),
                NPCType<PerforatorTailLarge>(),
                NPCType<PerforatorTailMedium>(),
                NPCType<PerforatorTailSmall>(),
                NPCType<SlimeGod>(),
                NPCType<SlimeGodRun>(),
                NPCType<SlimeGodSplit>(),
                NPCType<SlimeGodRunSplit>(),
				NPCType<Horse>(),
				NPCType<ThiccWaifu>(),
				NPCType<CryogenIce>(),
				NPCType<AquaticScourgeHead>(),
				NPCType<AquaticScourgeBody>(),
				NPCType<AquaticScourgeBodyAlt>(),
				NPCType<AquaticScourgeTail>(),
				NPCType<CragmawMire>(),
				NPCType<CalamitasRun>(),
				NPCType<CalamitasRun2>(),
				NPCType<SoulSeeker>(),
				NPCType<GreatSandShark>(),
				NPCType<SirenIce>(),
				NPCType<AureusSpawn>(),
				NPCType<PlaguebringerShade>(),
				NPCType<PlagueHomingMissile>(),
				NPCType<PlagueMine>(),
				NPCType<RavagerClawLeft>(),
				NPCType<RavagerClawRight>(),
				NPCType<RavagerLegLeft>(),
				NPCType<RavagerLegRight>(),
				NPCType<RockPillar>(),
				NPCType<RavagerHead>(),
				NPCType<Bumblefuck2>(),
				NPCType<ProvSpawnDefense>(),
				NPCType<ProvSpawnHealer>(),
				NPCType<ProvSpawnOffense>(),
				NPCType<BobbitWormHead>(),
				NPCType<Mauler>(),
				NPCType<ColossalSquid>(),
				NPCType<Reaper>(),
				NPCType<EidolonWyrmHead>(),
				NPCType<NuclearTerror>(),
				NPCType<OldDukeToothBall>(),
				NPCType<OldDukeSharkron>(),
				NPCType<DetonatingFlare>(),
				NPCType<DetonatingFlare2>(),
				NPCType<SupremeCataclysm>(),
				NPCType<SupremeCatastrophe>(),
				NPCType<SoulSeekerSupreme>()
			};

            confusionEnemyList = new List<int>()
            {
                NPCType<AeroSlime>(),
                NPCType<AngryDog>(),
                NPCType<AstralachneaGround>(),
                NPCType<AstralachneaWall>(),
                NPCType<BlightedEye>(),
                NPCType<BloomSlime>(),
                NPCType<Bohldohr>(),
                NPCType<CalamityEye>(),
                NPCType<CosmicElemental>(),
                NPCType<CrimulanBlightSlime>(),
                NPCType<Cryon>(),
                NPCType<CryoSlime>(),
                NPCType<CultistAssassin>(),
                NPCType<DespairStone>(),
                NPCType<EbonianBlightSlime>(),
                NPCType<FearlessGoldfishWarrior>(),
                NPCType<HeatSpirit>(),
                NPCType<MantisShrimp>(),
                NPCType<OverloadedSoldier>(),
                NPCType<PerennialSlime>(),
                NPCType<PhantomDebris>(),
                NPCType<Pitbull>(),
                NPCType<SandTortoise>(),
                NPCType<Scryllar>(),
                NPCType<ScryllarRage>(),
                NPCType<SeaUrchin>(),
                NPCType<StellarCulex>(),
                NPCType<StormlionCharger>(),
                NPCType<SuperDummyNPC>(),
                NPCType<SunBat>(),
                NPCType<WulfrumGyrator>(),
                NPCType<WulfrumRover>(),
                NPCType<WulfrumSlime>()
            };

            dungeonEnemyBuffList = new List<int>()
            {
                NPCID.SkeletonSniper,
                NPCID.TacticalSkeleton,
                NPCID.SkeletonCommando,
                NPCID.Paladin,
                NPCID.GiantCursedSkull,
                NPCID.BoneLee,
                NPCID.DiabolistWhite,
                NPCID.DiabolistRed,
                NPCID.NecromancerArmored,
                NPCID.Necromancer,
                NPCID.RaggedCasterOpenCoat,
                NPCID.RaggedCaster,
                NPCID.HellArmoredBonesSword,
                NPCID.HellArmoredBonesMace,
                NPCID.HellArmoredBonesSpikeShield,
                NPCID.HellArmoredBones,
                NPCID.BlueArmoredBonesSword,
                NPCID.BlueArmoredBonesNoPants,
                NPCID.BlueArmoredBonesMace,
                NPCID.BlueArmoredBones,
                NPCID.RustyArmoredBonesSwordNoArmor,
                NPCID.RustyArmoredBonesSword,
                NPCID.RustyArmoredBonesFlail,
                NPCID.RustyArmoredBonesAxe
            };

            dungeonProjectileBuffList = new List<int>()
            {
                ProjectileID.PaladinsHammerHostile,
                ProjectileID.ShadowBeamHostile,
                ProjectileID.InfernoHostileBolt,
                ProjectileID.InfernoHostileBlast,
                ProjectileID.LostSoulHostile,
                ProjectileID.SniperBullet,
                ProjectileID.RocketSkeleton,
                ProjectileID.BulletDeadeye,
                ProjectileID.Shadowflames
            };

            bossHPScaleList = new List<int>()
            {
                NPCID.EaterofWorldsHead,
                NPCID.EaterofWorldsBody,
                NPCID.EaterofWorldsTail,
                NPCID.SkeletronHand,
                NPCID.WallofFleshEye,
                NPCID.TheDestroyerBody,
                NPCID.TheDestroyerTail,
                NPCID.PrimeCannon,
                NPCID.PrimeLaser,
                NPCID.PrimeVice,
                NPCID.PrimeSaw,
                NPCID.GolemHead,
                NPCID.GolemHeadFree,
                NPCID.GolemFistRight,
                NPCID.GolemFistLeft,
                NPCID.MoonLordHead,
                NPCID.MoonLordHand
            };

            beeEnemyList = new List<int>()
            {
                NPCID.GiantMossHornet,
                NPCID.BigMossHornet,
                NPCID.LittleMossHornet,
                NPCID.TinyMossHornet,
                NPCID.MossHornet,
                NPCID.VortexHornetQueen,
                NPCID.VortexHornet,
                NPCID.Bee,
                NPCID.BeeSmall,
                NPCID.QueenBee,
                NPCType<PlaguebringerGoliath>(),
                NPCType<PlaguebringerShade>(),
                NPCType<PlagueBeeLargeG>(),
                NPCType<PlagueBeeLarge>(),
                NPCType<PlagueBeeG>(),
                NPCType<PlagueBee>()
            };

            beeProjectileList = new List<int>()
            {
                ProjectileID.Stinger,
                ProjectileID.HornetStinger,
                ProjectileType<PlagueStingerGoliath>(),
                ProjectileType<PlagueStingerGoliathV2>(),
                ProjectileType<PlagueExplosion>()
            };

            friendlyBeeList = new List<int>()
            {
                ProjectileID.GiantBee,
                ProjectileID.Bee,
                ProjectileID.Wasp,
                ProjectileType<PlaguenadeBee>(),
                ProjectileType<PlaguePrincess>(),
                ProjectileType<BabyPlaguebringer>(),
                ProjectileType<PlagueBeeSmall>()
            };

            hardModeNerfList = new List<int>()
            {
                ProjectileID.PinkLaser,
                ProjectileID.FrostBlastHostile,
                ProjectileID.GoldenShowerHostile,
                ProjectileID.RainNimbus,
                ProjectileID.FlamingArrow,
                ProjectileID.BulletDeadeye,
                ProjectileID.CannonballHostile,
				ProjectileID.UnholyTridentHostile,
				ProjectileID.FrostBeam,
				ProjectileID.CursedFlameHostile,
				ProjectileID.Stinger
            };

            debuffList = new List<int>()
            {
                BuffID.Poisoned,
                BuffID.Darkness,
                BuffID.Cursed,
                BuffID.OnFire,
                BuffID.Bleeding,
                BuffID.Confused,
                BuffID.Slow,
                BuffID.Weak,
                BuffID.Silenced,
                BuffID.BrokenArmor,
                BuffID.CursedInferno,
                BuffID.Frostburn,
                BuffID.Chilled,
                BuffID.Frozen,
                BuffID.Burning,
                BuffID.Suffocation,
                BuffID.Ichor,
                BuffID.Venom,
                BuffID.Blackout,
                BuffID.Electrified,
                BuffID.Rabies,
                BuffID.Webbed,
                BuffID.Stoned,
                BuffID.Dazed,
                BuffID.VortexDebuff,
                BuffID.WitheredArmor,
                BuffID.WitheredWeapon,
                BuffID.OgreSpit,
                BuffID.BetsysCurse,
                BuffType<SulphuricPoisoning>(),
                BuffType<Shadowflame>(),
                BuffType<BrimstoneFlames>(),
                BuffType<BurningBlood>(),
                BuffType<GlacialState>(),
                BuffType<GodSlayerInferno>(),
                BuffType<AstralInfectionDebuff>(),
                BuffType<HolyFlames>(),
                BuffType<Irradiated>(),
                BuffType<Plague>(),
                BuffType<AbyssalFlames>(),
                BuffType<CrushDepth>(),
                BuffType<MarkedforDeath>(),
                BuffType<WarCleave>(),
                BuffType<ArmorCrunch>(),
                BuffType<Vaporfied>(),
                BuffType<Eutrophication>(),
                BuffType<LethalLavaBurn>(),
                BuffType<Nightwither>(),
				BuffType<VulnerabilityHex>()
			};

            fireWeaponList = new List<int>()
            {
                ItemID.FieryGreatsword,
                ItemID.DD2SquireDemonSword,
                ItemID.TheHorsemansBlade,
                ItemID.Cascade,
                ItemID.HelFire,
                ItemID.Flamarang,
                ItemID.MoltenFury,
                ItemID.Sunfury,
                ItemID.PhoenixBlaster,
                ItemID.Flamelash,
                ItemID.SolarEruption,
                ItemID.DayBreak,
                ItemID.HellwingBow,
                ItemID.DD2PhoenixBow,
                ItemID.DD2BetsyBow,
                ItemID.FlareGun,
                ItemID.Flamethrower,
                ItemID.EldMelter,
                ItemID.FlowerofFire,
                ItemID.MeteorStaff,
                ItemID.ApprenticeStaffT3,
                ItemID.InfernoFork,
                ItemID.HeatRay,
                ItemID.BookofSkulls,
                ItemID.ImpStaff,
                ItemID.DD2FlameburstTowerT1Popper,
                ItemID.DD2FlameburstTowerT2Popper,
                ItemID.DD2FlameburstTowerT3Popper,
                ItemID.MolotovCocktail,
                ItemID.WandofSparking,
                ItemType<AegisBlade>(),
                ItemType<BalefulHarvester>(),
                ItemType<Chaotrix>(),
                ItemType<CometQuasher>(),
                ItemType<DivineRetribution>(),
                ItemType<DraconicDestruction>(),
                ItemType<Drataliornus>(),
                ItemType<EnergyStaff>(),
                ItemType<ExsanguinationLance>(),
                ItemType<FirestormCannon>(),
                ItemType<FlameburstShortsword>(),
                ItemType<FlameScythe>(),
                ItemType<FlareBolt>(),
                ItemType<FlarewingBow>(),
                ItemType<ForbiddenSun>(),
                ItemType<GreatbowofTurmoil>(),
                ItemType<HarvestStaff>(),
                ItemType<Hellborn>(),
                ItemType<HellBurst>(),
                ItemType<HellfireFlamberge>(),
                ItemType<Hellkite>(),
                ItemType<HellwingStaff>(),
                ItemType<Helstorm>(),
                ItemType<HellsSun>(),
                ItemType<InfernaCutter>(),
                ItemType<Lazhar>(),
                ItemType<MeteorFist>(),
                ItemType<Mourningstar>(),
                ItemType<PhoenixBlade>(),
                ItemType<RedSun>(),
                ItemType<SparkSpreader>(),
                ItemType<SpectralstormCannon>(),
                ItemType<SunGodStaff>(),
                ItemType<SunSpiritStaff>(),
                ItemType<TerraFlameburster>(),
                ItemType<TheLastMourning>(),
                ItemType<TheWand>(),
                ItemType<VenusianTrident>(),
                ItemType<Vesuvius>(),
                ItemType<BlissfulBombardier>(),
                ItemType<HolyCollider>(),
                ItemType<MoltenAmputator>(),
                ItemType<PurgeGuzzler>(),
                ItemType<SolarFlare>(),
                ItemType<TelluricGlare>(),
                ItemType<AngryChickenStaff>(),
                ItemType<ChickenCannon>(),
                ItemType<DragonRage>(),
                ItemType<DragonsBreath>(),
                ItemType<PhoenixFlameBarrage>(),
                ItemType<ProfanedTrident>(),
                ItemType<TheBurningSky>(),
                ItemType<TotalityBreakers>(),
                ItemType<ProfanedPartisan>(),
                ItemType<BlastBarrel>(),
                ItemType<LatcherMine>(),
                ItemType<BouncingBetty>(),
                ItemType<HeliumFlash>(),
                ItemType<ShatteredSun>(),
                ItemType<DivineHatchet>(),
                ItemType<DazzlingStabberStaff>(),
                ItemType<PristineFury>(),
                ItemType<SarosPossession>(),
                ItemType<CinderBlossomStaff>(),
                ItemType<FinalDawn>(),
                ItemType<DragonPow>()
            };

            iceWeaponList = new List<int>()
            {
                ItemID.IceBlade,
                ItemID.IceSickle,
                ItemID.Frostbrand,
                ItemID.Amarok,
                ItemID.NorthPole,
                ItemID.IceBoomerang,
                ItemID.IceBow,
                ItemID.SnowmanCannon,
                ItemID.SnowballCannon,
                ItemID.IceRod,
                ItemID.FlowerofFrost,
                ItemID.FrostStaff,
                ItemID.BlizzardStaff,
                ItemID.StaffoftheFrostHydra,
                ItemID.Snowball,
                ItemType<AbsoluteZero>(),
                ItemType<Avalanche>(),
                ItemType<GlacialCrusher>(),
                ItemType<TemporalFloeSword>(),
                ItemType<ColdheartIcicle>(),
                ItemType<CosmicDischarge>(),
                ItemType<EffluviumBow>(),
                ItemType<EternalBlizzard>(),
                ItemType<FrostbiteBlaster>(),
                ItemType<IcicleStaff>(),
                ItemType<BittercoldStaff>(),
                ItemType<CrystalFlareStaff>(),
                ItemType<IcicleTrident>(),
                ItemType<SnowstormStaff>(),
                ItemType<Cryophobia>(),
                ItemType<FrostBolt>(),
                ItemType<WintersFury>(),
                ItemType<ArcticBearPaw>(),
                ItemType<AncientIceChunk>(),
                ItemType<CryogenicStaff>(),
                ItemType<FrostyFlare>(),
                ItemType<IceStar>(),
                ItemType<Icebreaker>(),
                ItemType<KelvinCatalyst>(),
                ItemType<FrostcrushValari>(),
                ItemType<Endogenesis>(),
                ItemType<FlurrystormCannon>(),
                ItemType<Hypothermia>(),
                ItemType<IceBarrage>(),
                ItemType<FrostBlossomStaff>(),
                ItemType<EndoHydraStaff>(),
                //Cryonic Bar set stuff, could potentially be removed
                ItemType<Trinity>(),
                ItemType<Shimmerspark>(),
                ItemType<StarnightLance>(),
                ItemType<DarkechoGreatbow>(),
                ItemType<ShadecrystalTome>(),
                ItemType<CrystalPiercer>()
            };

            natureWeaponList = new List<int>()
            {
                ItemID.BladeofGrass,
                ItemID.ChlorophyteClaymore,
                ItemID.ChlorophyteSaber,
                ItemID.ChlorophytePartisan,
                ItemID.ChlorophyteShotbow,
                ItemID.Seedler,
                ItemID.ChristmasTreeSword,
                ItemID.TerraBlade,
                ItemID.JungleYoyo,
                ItemID.Yelets,
                ItemID.MushroomSpear,
                ItemID.ThornChakram,
                ItemID.Bananarang,
                ItemID.FlowerPow,
                ItemID.BeesKnees,
                ItemID.Toxikarp,
                ItemID.Bladetongue,
                ItemID.PoisonStaff,
                ItemID.VenomStaff,
                ItemID.StaffofEarth,
                ItemID.BeeGun,
                ItemID.LeafBlower,
                ItemID.WaspGun,
                ItemID.CrystalSerpent,
                ItemID.Razorpine,
                ItemID.HornetStaff,
                ItemID.QueenSpiderStaff,
                ItemID.SlimeStaff,
                ItemID.PygmyStaff,
                ItemID.RavenStaff,
                ItemID.BatScepter,
                ItemID.SpiderStaff,
                ItemID.Beenade,
                ItemID.FrostDaggerfish,
                ItemType<DepthBlade>(),
                ItemType<AbyssBlade>(),
                ItemType<NeptunesBounty>(),
                ItemType<AquaticDissolution>(),
                ItemType<ArchAmaryllis>(),
                ItemType<ThornBlossom>(),
                ItemType<BiomeBlade>(),
                ItemType<TrueBiomeBlade>(),
                ItemType<OmegaBiomeBlade>(),
                ItemType<BladedgeGreatbow>(),
                ItemType<BlossomFlux>(),
                ItemType<EvergladeSpray>(),
                ItemType<FeralthornClaymore>(),
                ItemType<Floodtide>(),
                ItemType<FourSeasonsGalaxia>(),
                ItemType<GammaFusillade>(),
                ItemType<GleamingMagnolia>(),
                ItemType<HarvestStaff>(),
                ItemType<HellionFlowerSpear>(),
                ItemType<Lazhar>(),
                ItemType<LifefruitScythe>(),
                ItemType<ManaRose>(),
                ItemType<MangroveChakram>(),
                ItemType<MantisClaws>(),
                ItemType<Mariana>(),
                ItemType<Mistlestorm>(),
                ItemType<Monsoon>(),
                ItemType<Alluvion>(),
                ItemType<Needler>(),
                ItemType<NettlelineGreatbow>(),
                ItemType<Quagmire>(),
                ItemType<Shroomer>(),
                ItemType<SolsticeClaymore>(),
                ItemType<SporeKnife>(),
                ItemType<Spyker>(),
                ItemType<StormSaber>(),
                ItemType<StormRuler>(),
                ItemType<StormSurge>(),
                ItemType<TarragonThrowingDart>(),
                ItemType<TerraEdge>(),
                ItemType<TerraLance>(),
                ItemType<TerraRay>(),
                ItemType<TerraShiv>(),
                ItemType<Terratomere>(),
                ItemType<TerraFlameburster>(),
                ItemType<TheSwarmer>(),
                ItemType<Verdant>(),
                ItemType<Barinautical>(),
                ItemType<DeepseaStaff>(),
                ItemType<Downpour>(),
                ItemType<SubmarineShocker>(),
                ItemType<ScourgeoftheSeas>(),
                ItemType<Archerfish>(),
                ItemType<BallOFugu>(),
                ItemType<BlackAnurian>(),
                ItemType<CalamarisLament>(),
                ItemType<HerringStaff>(),
                ItemType<Lionfish>(),
                ItemType<ShellfishStaff>(),
                ItemType<ClamCrusher>(),
                ItemType<ClamorRifle>(),
                ItemType<Serpentine>(),
                ItemType<UrchinFlail>(),
                ItemType<CoralCannon>(),
                ItemType<Shellshooter>(),
                ItemType<SandDollar>(),
                ItemType<MagicalConch>(),
                ItemType<SnapClam>(),
                ItemType<GacruxianMollusk>(),
                ItemType<PolarisParrotfish>(),
                ItemType<SparklingEmpress>(),
                ItemType<NastyCholla>(),
                ItemType<PoisonPack>(),
                ItemType<PlantationStaff>(),
                ItemType<SeasSearing>(),
                ItemType<YateveoBloom>(),
                ItemType<TerraDisk>(),
                ItemType<BelladonnaSpiritStaff>(),
                ItemType<TenebreusTides>(),
                ItemType<Greentide>(),
                ItemType<Leviatitan>(),
                ItemType<BrackishFlask>(),
                ItemType<LeviathanTeeth>(),
                ItemType<GastricBelcherStaff>()
            };

            alcoholList = new List<int>()
            {
                BuffType<BloodyMaryBuff>(),
                BuffType<CaribbeanRumBuff>(),
                BuffType<CinnamonRollBuff>(),
                BuffType<EverclearBuff>(),
                BuffType<EvergreenGinBuff>(),
                BuffType<FireballBuff>(),
                BuffType<GrapeBeerBuff>(),
                BuffType<MargaritaBuff>(),
                BuffType<MoonshineBuff>(),
                BuffType<MoscowMuleBuff>(),
                BuffType<RedWineBuff>(),
                BuffType<RumBuff>(),
                BuffType<ScrewdriverBuff>(),
                BuffType<StarBeamRyeBuff>(),
                BuffType<TequilaBuff>(),
                BuffType<TequilaSunriseBuff>(),
                BuffType<VodkaBuff>(),
                BuffType<WhiskeyBuff>(),
                BuffType<WhiteWineBuff>()
            };

            useTurnList = new List<int>()
            {
				ItemID.WoodenSword,
				ItemID.RichMahoganySword,
				ItemID.BorealWoodSword,
				ItemID.EbonwoodSword,
				ItemID.ShadewoodSword,
				ItemID.CopperBroadsword,
				ItemID.IronBroadsword,
                ItemID.LeadBroadsword,
                ItemID.SilverBroadsword,
                ItemID.TungstenBroadsword,
                ItemID.PlatinumBroadsword,
                ItemID.GoldBroadsword,
                ItemID.CactusSword,
                ItemID.ZombieArm,
                ItemID.StylistKilLaKillScissorsIWish,
                ItemID.DyeTradersScimitar,
                ItemID.PurpleClubberfish,
                ItemID.LightsBane,
                ItemID.BloodButcherer,
                ItemID.BluePhaseblade,
                ItemID.RedPhaseblade,
                ItemID.GreenPhaseblade,
                ItemID.WhitePhaseblade,
                ItemID.YellowPhaseblade,
                ItemID.PurplePhaseblade,
                ItemID.BladeofGrass,
                ItemID.FieryGreatsword,
                ItemID.NightsEdge,
                ItemID.FalconBlade,
                ItemID.BreakerBlade,
                ItemID.SlapHand,
                ItemID.TaxCollectorsStickOfDoom,
                ItemID.Bladetongue,
                ItemID.CobaltSword,
                ItemID.PalladiumSword,
                ItemID.MythrilSword,
                ItemID.OrichalcumSword,
                ItemID.AdamantiteSword,
                ItemID.TitaniumSword,
                ItemID.Excalibur,
                ItemID.TrueExcalibur,
                ItemID.TrueNightsEdge,
                ItemID.TheHorsemansBlade,
                ItemID.Keybrand,
				ItemID.PsychoKnife,
				ItemID.BeeKeeper,
				ItemID.ChristmasTreeSword,
				ItemID.BoneSword
            };

            twentyUseTimeBuffList = new List<int>()
            {
                ItemID.CobaltSword,
                ItemID.PalladiumSword,
                ItemID.OrichalcumSword,
                ItemID.AdamantiteSword,
                ItemID.TitaniumSword,
                ItemID.Excalibur,
                ItemID.Bladetongue,
                ItemID.Cutlass,
                ItemID.TheHorsemansBlade,
                ItemID.AdamantiteGlaive,
                ItemID.ChlorophytePartisan,
                ItemID.CobaltNaginata,
                ItemID.Gungnir,
                ItemID.MythrilHalberd,
                ItemID.OrichalcumHalberd,
                ItemID.PalladiumPike,
                ItemID.TitaniumTrident,
                ItemID.MushroomSpear,
				ItemID.TaxCollectorsStickOfDoom
            };

            fiftySizeBuffList = new List<int>()
            {
                ItemID.BreakerBlade,
                ItemID.StylistKilLaKillScissorsIWish,
                ItemID.NightsEdge,
                ItemID.CobaltSword,
                ItemID.MythrilSword,
                ItemID.AdamantiteSword,
                ItemID.PalladiumSword,
                ItemID.OrichalcumSword,
                ItemID.TitaniumSword,
                ItemID.Excalibur,
                ItemID.TheHorsemansBlade,
                ItemID.Keybrand,
                ItemID.SlapHand,
                ItemID.PlatinumBroadsword,
                ItemID.GoldBroadsword,
                ItemID.LightsBane,
                ItemID.FalconBlade,
                ItemID.BeeKeeper,
                ItemID.ZombieArm,
                ItemID.BladeofGrass,
                ItemID.Muramasa,
                ItemID.FieryGreatsword,
                ItemID.BluePhasesaber,
                ItemID.RedPhasesaber,
                ItemID.GreenPhasesaber,
                ItemID.WhitePhasesaber,
                ItemID.YellowPhasesaber,
                ItemID.PurplePhasesaber,
                ItemID.BluePhaseblade,
                ItemID.RedPhaseblade,
                ItemID.GreenPhaseblade,
                ItemID.WhitePhaseblade,
                ItemID.YellowPhaseblade,
                ItemID.PurplePhaseblade,
                ItemID.AntlionClaw,
                ItemID.DyeTradersScimitar,
                ItemID.BoneSword,
				ItemID.TaxCollectorsStickOfDoom,
				ItemID.PurpleClubberfish
            };

            quadrupleDamageBuffList = new List<int>()
            {
                ItemID.PsychoKnife
            };

            tripleDamageBuffList = new List<int>()
            {
				ItemID.SpectreStaff,
                ItemID.KOCannon,
                ItemID.NightsEdge,
                ItemID.PalladiumPike
            };

            doubleDamageBuffList = new List<int>()
            {
                ItemID.BallOHurt,
                ItemID.TheMeatball,
                ItemID.BlueMoon,
                ItemID.Sunfury,
                ItemID.FlowerPow,
                ItemID.MonkStaffT2,
                ItemID.ProximityMineLauncher,
                ItemID.FireworksLauncher,
                ItemID.ShadowbeamStaff,
                ItemID.PlatinumShortsword,
                ItemID.PlatinumBroadsword,
                ItemID.GoldShortsword,
                ItemID.GoldBroadsword,
                ItemID.LightsBane,
                ItemID.BeeKeeper,
                ItemID.ZombieArm,
                ItemID.Muramasa,
                ItemID.Spear,
                ItemID.Trident,
                ItemID.WoodenBoomerang,
                ItemID.EnchantedBoomerang,
                ItemID.IceBoomerang,
                ItemID.FruitcakeChakram,
                ItemID.ThornChakram,
                ItemID.Flamarang,
                ItemID.Cutlass,
                ItemID.BluePhaseblade,
                ItemID.RedPhaseblade,
                ItemID.GreenPhaseblade,
                ItemID.WhitePhaseblade,
                ItemID.YellowPhaseblade,
                ItemID.PurplePhaseblade,
				ItemID.RocketLauncher,
				ItemID.GrenadeLauncher
			};

            sixtySixDamageBuffList = new List<int>()
            {
				ItemID.PulseBow,
				ItemID.TrueNightsEdge,
                ItemID.MedusaHead,
                ItemID.StaffofEarth,
                ItemID.InfernoFork,
                ItemID.Frostbrand,
                ItemID.BloodButcherer
            };

            fiftyDamageBuffList = new List<int>()
            {
				ItemID.IceBow,
				ItemID.Marrow,
				ItemID.EldMelter,
                ItemID.Flamethrower,
                ItemID.MoonlordTurretStaff,
                ItemID.WaspGun,
                ItemID.SolarEruption,
                ItemID.DayBreak,
                ItemID.LunarFlareBook,
                ItemID.SilverShortsword,
                ItemID.SilverBroadsword,
                ItemID.TungstenShortsword,
                ItemID.TungstenBroadsword,
                ItemID.AntlionClaw,
                ItemID.Katana,
                ItemID.Seedler
            };

            thirtyThreeDamageBuffList = new List<int>()
            {
                ItemID.WandofSparking,
                ItemID.CrystalVileShard,
                ItemID.SoulDrain,
                ItemID.ClingerStaff,
                ItemID.ChargedBlasterCannon,
                ItemID.NettleBurst,
				ItemID.VenomStaff,
				ItemID.AmberStaff,
                ItemID.VampireKnives,
                ItemID.Cascade,
                ItemID.TrueExcalibur,
                ItemID.Meowmere,
                ItemID.DyeTradersScimitar
            };

            twentyFiveDamageBuffList = new List<int>()
            {
                ItemID.StakeLauncher,
                ItemID.BookStaff,
                ItemID.IronShortsword,
                ItemID.IronBroadsword,
                ItemID.LeadShortsword,
                ItemID.LeadBroadsword,
                ItemID.CandyCaneSword,
                ItemID.BoneSword
            };

            twentyDamageBuffList = new List<int>()
            {
                ItemID.ChainGuillotines,
                ItemID.FlowerofFrost,
                ItemID.PoisonStaff,
                ItemID.TacticalShotgun
            };

            tenDamageBuffList = new List<int>()
            {
				ItemID.WoodenArrow,
				ItemID.FlamingArrow,
				ItemID.UnholyArrow,
				ItemID.JestersArrow,
				ItemID.HellfireArrow,
				ItemID.HolyArrow,
				ItemID.CursedArrow,
				ItemID.FrostburnArrow,
				ItemID.ChlorophyteArrow,
				ItemID.IchorArrow,
				ItemID.VenomArrow,
				ItemID.BoneArrow,
				ItemID.EndlessQuiver,
				ItemID.MoonlordArrow,
				ItemID.DD2BetsyBow,
				ItemID.BorealWoodBow,
				ItemID.CopperBow,
				ItemID.DemonBow,
				ItemID.EbonwoodBow,
				ItemID.GoldBow,
				ItemID.HellwingBow,
				ItemID.IronBow,
				ItemID.LeadBow,
				ItemID.MoltenFury,
				ItemID.PalmWoodBow,
				ItemID.PearlwoodBow,
				ItemID.DD2PhoenixBow,
				ItemID.PlatinumBow,
				ItemID.RichMahoganyBow,
				ItemID.ShadewoodBow,
				ItemID.ShadowFlameBow,
				ItemID.SilverBow,
				ItemID.TendonBow,
				ItemID.BeesKnees,
				ItemID.TinBow,
				ItemID.Tsunami,
				ItemID.TungstenBow,
				ItemID.WoodenBow,
				ItemID.AdamantiteRepeater,
				ItemID.ChlorophyteShotbow,
				ItemID.CobaltRepeater,
				ItemID.HallowedRepeater,
				ItemID.MythrilRepeater,
				ItemID.OrichalcumRepeater,
				ItemID.PalladiumRepeater,
				ItemID.TitaniumRepeater,
                ItemID.MagnetSphere,
                ItemID.BatScepter,
				ItemID.ElectrosphereLauncher
			};

            weaponAutoreuseList = new List<int>()
            {
				ItemID.WoodenSword,
				ItemID.RichMahoganySword,
				ItemID.BorealWoodSword,
				ItemID.EbonwoodSword,
				ItemID.ShadewoodSword,
				ItemID.CopperShortsword,
				ItemID.CopperBroadsword,
                ItemID.IronShortsword,
                ItemID.IronBroadsword,
                ItemID.LeadShortsword,
                ItemID.LeadBroadsword,
                ItemID.SilverShortsword,
                ItemID.SilverBroadsword,
                ItemID.TungstenShortsword,
                ItemID.TungstenBroadsword,
                ItemID.PlatinumShortsword,
                ItemID.PlatinumBroadsword,
                ItemID.GoldShortsword,
                ItemID.GoldBroadsword,
				ItemID.CactusSword,
                ItemID.ZombieArm,
                ItemID.CandyCaneSword,
                ItemID.BoneSword,
                ItemID.LightsBane,
                ItemID.BloodButcherer,
                ItemID.DyeTradersScimitar,
				ItemID.FieryGreatsword,
                ItemID.NightsEdge,
                ItemID.TrueNightsEdge,
                ItemID.TrueExcalibur,
				ItemID.BreakerBlade,
                ItemID.PhoenixBlaster,
                ItemID.VenusMagnum,
                ItemID.MagicDagger,
                ItemID.BeamSword,
                ItemID.PaladinsHammer,
                ItemID.PearlwoodSword,
                ItemID.PearlwoodBow,
                ItemID.TaxCollectorsStickOfDoom,
                ItemID.StylistKilLaKillScissorsIWish,
                ItemID.AdamantiteGlaive,
                ItemID.ChlorophytePartisan,
                ItemID.CobaltNaginata,
                ItemID.DarkLance,
                ItemID.MonkStaffT2,
                ItemID.Gungnir,
                ItemID.MushroomSpear,
                ItemID.MythrilHalberd,
                ItemID.NorthPole,
                ItemID.ObsidianSwordfish,
                ItemID.OrichalcumHalberd,
                ItemID.PalladiumPike,
                ItemID.Spear,
                ItemID.Swordfish,
                ItemID.TheRottedFork,
                ItemID.TitaniumTrident,
                ItemID.Trident,
                ItemID.BluePhaseblade,
                ItemID.RedPhaseblade,
                ItemID.GreenPhaseblade,
                ItemID.WhitePhaseblade,
                ItemID.YellowPhaseblade,
                ItemID.PurplePhaseblade,
				ItemID.BladeofGrass,
				ItemID.ChristmasTreeSword,
				ItemID.ChainKnife
                //ItemID.StormSpear
            };

            spearAutoreuseList = new List<int>()
            {
                ItemID.AdamantiteGlaive,
                ItemID.ChlorophytePartisan,
                ItemID.CobaltNaginata,
                ItemID.DarkLance,
                ItemID.MonkStaffT2,
                ItemID.Gungnir,
                ItemID.MushroomSpear,
                ItemID.MythrilHalberd,
                ItemID.NorthPole,
                ItemID.ObsidianSwordfish,
                ItemID.OrichalcumHalberd,
                ItemID.PalladiumPike,
                ItemID.Spear,
                ItemID.Swordfish,
                ItemID.TheRottedFork,
                ItemID.TitaniumTrident,
                ItemID.Trident
                //ItemID.StormSpear
            };

            tenDamageNerfList = new List<int>()
            {
				ItemID.DaedalusStormbow,
				ItemID.StarWrath
            };

            quarterDamageNerfList = new List<int>()
            {
				ItemID.LastPrism,
				ItemID.DemonScythe,
                ItemID.Razorpine,
                ItemID.PhoenixBlaster,
                ItemID.InfluxWaver,
                ItemID.Xenopopper,
                ItemID.OpticStaff, //Note: got local i frames so it should be better
				ItemID.RocketIII,
				ItemID.RocketIV
			};

            pumpkinMoonBuffList = new List<int>()
            {
                NPCID.Scarecrow1,
                NPCID.Scarecrow2,
                NPCID.Scarecrow3,
                NPCID.Scarecrow4,
                NPCID.Scarecrow5,
                NPCID.Scarecrow6,
                NPCID.Scarecrow7,
                NPCID.Scarecrow8,
                NPCID.Scarecrow9,
                NPCID.Scarecrow10,
                NPCID.HeadlessHorseman,
                NPCID.MourningWood,
                NPCID.Splinterling,
                NPCID.Pumpking,
                NPCID.PumpkingBlade,
                NPCID.Hellhound,
                NPCID.Poltergeist
            };

            frostMoonBuffList = new List<int>()
            {
                NPCID.ZombieElf,
                NPCID.ZombieElfBeard,
                NPCID.ZombieElfGirl,
                NPCID.PresentMimic,
                NPCID.GingerbreadMan,
                NPCID.Yeti,
                NPCID.Everscream,
                NPCID.IceQueen,
                NPCID.SantaNK1,
                NPCID.ElfCopter,
                NPCID.Nutcracker,
                NPCID.NutcrackerSpinning,
                NPCID.ElfArcher,
                NPCID.Krampus,
                NPCID.Flocko
            };

            eclipseBuffList = new List<int>()
            {
                NPCID.Eyezor,
                NPCID.Reaper,
                NPCID.Frankenstein,
                NPCID.SwampThing,
                NPCID.Vampire,
                NPCID.VampireBat,
                NPCID.Butcher,
                NPCID.CreatureFromTheDeep,
                NPCID.Fritz,
                NPCID.Nailhead,
                NPCID.Psycho,
                NPCID.DeadlySphere,
                NPCID.DrManFly,
                NPCID.ThePossessed,
                NPCID.Mothron,
                NPCID.MothronEgg,
                NPCID.MothronSpawn
            };

            eventProjectileBuffList = new List<int>()
            {
                ProjectileID.FlamingWood,
                ProjectileID.GreekFire1,
                ProjectileID.GreekFire2,
                ProjectileID.GreekFire3,
                ProjectileID.FlamingScythe,
                ProjectileID.FlamingArrow,
                ProjectileID.PineNeedleHostile,
                ProjectileID.OrnamentHostile,
                ProjectileID.OrnamentHostileShrapnel,
                ProjectileID.FrostWave,
                ProjectileID.FrostShard,
                ProjectileID.Missile,
                ProjectileID.Present,
                ProjectileID.Spike,
                ProjectileID.BulletDeadeye,
                ProjectileID.EyeLaser,
                ProjectileID.Nail,
                ProjectileID.DrManFlyFlask
            };

            // Enemies that inflict an average of 1 to 50 damage in Expert Mode
            revengeanceEnemyBuffList25Percent = new List<int>()
            {
                NPCID.GiantWormHead,
                NPCID.BlazingWheel,
                NPCType<AquaticSeekerHead>(),
                NPCType<Cnidrion>(),
                NPCType<PrismTurtle>(),
                NPCType<GhostBell>()
            };

            // Enemies that inflict an average of 51 to 100 damage in Expert Mode
            revengeanceEnemyBuffList20Percent = new List<int>()
            {
                NPCID.DevourerHead,
                NPCID.MeteorHead,
                NPCID.BoneSerpentHead,
                NPCID.ManEater,
                NPCID.Snatcher,
                NPCID.Piranha,
                NPCID.Shark,
                NPCID.SpikeBall,
                NPCID.DiggerHead,
                NPCID.WallCreeper,
                NPCID.WallCreeperWall,
                NPCID.Lihzahrd,
                NPCID.Pumpking,
                NPCID.SlimeSpiked,
                NPCType<EutrophicRay>(),
                NPCType<Clam>(),
                NPCType<SeaSerpent1>(),
                NPCType<GiantClam>(),
                NPCType<FearlessGoldfishWarrior>()
            };

            // Enemies that inflict an average of 101 to 200 damage in Expert Mode
            revengeanceEnemyBuffList15Percent = new List<int>()
            {
                NPCID.DD2Betsy,
                NPCID.Mimic,
                NPCID.WyvernHead,
                NPCID.SeekerHead,
                NPCID.AnglerFish,
                NPCID.Werewolf,
                NPCID.Wraith,
                NPCID.Arapaima,
                NPCID.BlackRecluse,
                NPCID.BlackRecluseWall,
                NPCID.AngryTrapper,
                NPCID.LihzahrdCrawler,
                NPCID.PirateCaptain,
                NPCID.FlyingSnake,
                NPCID.Reaper,
                NPCID.Paladin,
                NPCID.BoneLee,
                NPCID.MourningWood,
                NPCID.PumpkingBlade,
                NPCID.PresentMimic,
                NPCID.Everscream,
                NPCID.IceQueen,
                NPCID.SantaNK1,
                NPCID.StardustWormHead,
                NPCID.Butcher,
                NPCID.Psycho,
                NPCID.DeadlySphere,
                NPCID.BigMimicCorruption,
                NPCID.BigMimicCrimson,
                NPCID.BigMimicHallow,
                NPCID.Mothron,
                NPCID.DuneSplicerHead,
                NPCID.SandShark,
                NPCID.SandsharkCorrupt,
                NPCID.SandsharkCrimson,
                NPCID.SandsharkHallow,
                NPCType<Atlas>(),
                NPCType<ArmoredDiggerHead>(),
                NPCType<GreatSandShark>(),
                NPCType<Horse>(),
                NPCType<ScornEater>(),
                NPCType<BlindedAngler>()
            };

            // Enemies that inflict an average of 201 to 400 damage in Expert Mode
            revengeanceEnemyBuffList10Percent = new List<int>()
            {
                NPCID.SolarCrawltipedeHead,
                NPCType<BobbitWormHead>(),
                NPCType<ColossalSquid>(),
                NPCType<EidolonWyrmHead>(),
                NPCType<GulperEelHead>(),
                NPCType<Mauler>(),
                NPCType<Reaper>()
            };

            revengeanceProjectileBuffList25Percent = new List<int>()
            {
                ProjectileID.SandBallFalling,
                ProjectileID.AshBallFalling,
                ProjectileID.EbonsandBallFalling,
                ProjectileID.PearlSandBallFalling,
                ProjectileID.CrimsandBallFalling,
                ProjectileID.GeyserTrap
            };

            revengeanceProjectileBuffList20Percent = new List<int>()
            {
                ProjectileID.PoisonDartTrap,
                ProjectileID.DemonSickle,
                ProjectileID.SandnadoHostile,
                ProjectileID.DD2BetsyFireball,
                ProjectileID.DD2BetsyFlameBreath
            };

            revengeanceProjectileBuffList15Percent = new List<int>()
            {
                ProjectileID.SpikyBallTrap,
                ProjectileID.SpearTrap,
                ProjectileID.FlamethrowerTrap,
                ProjectileID.FlamesTrap,
                ProjectileID.PaladinsHammerHostile,
                ProjectileID.FlamingWood,
                ProjectileID.FlamingScythe,
                ProjectileID.FrostWave,
                ProjectileID.Present,
                ProjectileID.Spike,
                ProjectileID.SaucerDeathray,
                ProjectileID.NebulaBolt,
                ProjectileID.NebulaSphere,
                ProjectileID.NebulaLaser,
                ProjectileID.StardustSoldierLaser,
                ProjectileID.VortexLaser,
                ProjectileID.VortexVortexLightning,
                ProjectileID.VortexLightning,
                ProjectileID.VortexAcid,
                ProjectileType<GreatSandBlast>(),
                ProjectileType<PearlBurst>(),
                ProjectileType<PearlRain>()
            };

            revengeanceLifeStealExceptionList = new List<int>()
            {
                NPCID.Probe,
                NPCID.CultistDragonHead,
                NPCID.CultistDragonBody1,
                NPCID.CultistDragonBody2,
                NPCID.CultistDragonBody3,
                NPCID.CultistDragonBody4,
                NPCID.CultistDragonTail,
                NPCID.AncientCultistSquidhead,
                NPCID.AncientLight,
                NPCID.Sharkron,
                NPCID.Sharkron2,
                NPCID.PlanterasTentacle,
                NPCID.Spore,
                NPCID.TheHungryII,
                NPCID.LeechHead,
                NPCID.LeechBody,
                NPCID.LeechTail,
                NPCID.TheDestroyerBody,
                NPCID.TheDestroyerTail,
                NPCID.EaterofWorldsBody,
                NPCID.EaterofWorldsTail,
                NPCID.MoonLordCore
            };

            noRageWormSegmentList = new List<int>()
            {
                NPCType<DesertScourgeBody>(),
                NPCType<DesertScourgeTail>(),
                NPCType<AquaticScourgeBody>(),
                NPCType<AquaticScourgeBodyAlt>(),
                NPCType<AquaticScourgeTail>(),
                NPCType<AstrumDeusBodySpectral>(),
                NPCType<AstrumDeusTailSpectral>(),
                NPCType<StormWeaverBody>(),
                NPCType<StormWeaverTail>(),
                NPCType<StormWeaverBodyNaked>(),
                NPCType<StormWeaverTailNaked>(),
                NPCType<DevourerofGodsBody>(),
                NPCType<DevourerofGodsTail>(),
                NPCType<DevourerofGodsBodyS>(),
                NPCType<DevourerofGodsTailS>(),
				NPCType<ThanatosBody1>(),
				NPCType<ThanatosBody2>(),
				NPCType<ThanatosTail>()
			};

            needsDebuffIconDisplayList = new List<int>()
            {
                NPCID.WallofFleshEye
            };

            trapProjectileList = new List<int>()
            {
                ProjectileID.PoisonDartTrap,
                ProjectileID.SpikyBallTrap,
                ProjectileID.SpearTrap,
                ProjectileID.FlamethrowerTrap,
                ProjectileID.FlamesTrap,
                ProjectileID.PoisonDart,
                ProjectileID.GeyserTrap
            };

            scopedWeaponList = new List<int>()
            {
                ItemType<AMR>(),
                ItemType<Auralis>(),
                ItemType<HalleysInferno>(),
                ItemType<Shroomer>(),
                ItemType<SpectreRifle>(),
                ItemType<Svantechnical>(),
                ItemType<Skullmasher>(),
                ItemType<TyrannysEnd>()
            };

            boomerangList = new List<int>()
            {
                ItemType<Brimblade>(),
                ItemType<BlazingStar>(),
                ItemType<Celestus>(),
                ItemType<AccretionDisk>(),
                ItemType<EnchantedAxe>(),
                ItemType<EpidemicShredder>(),
                ItemType<Equanimity>(),
                ItemType<Eradicator>(),
                ItemType<FlameScythe>(),
                ItemType<Glaive>(),
                ItemType<GhoulishGouger>(),
                ItemType<Icebreaker>(),
                ItemType<KelvinCatalyst>(),
                ItemType<Kylie>(),
                ItemType<MangroveChakram>(),
                ItemType<MoltenAmputator>(),
                ItemType<NanoblackReaperRogue>(),
                ItemType<SandDollar>(),
                ItemType<SeashellBoomerang>(),
                ItemType<Shroomerang>(),
                ItemType<Valediction>(),
                ItemType<FrostcrushValari>(),
                ItemType<DefectiveSphere>(),
                ItemType<TerraDisk>(),
                ItemType<ToxicantTwister>(),
                ItemType<TrackingDisk>()
            };

            boomerangProjList = new List<int>()
            {
                ProjectileType<AccretionDiskProj>(),
                ProjectileType<AccretionDisk2>(),
                ProjectileType<BlazingStarProj>(),
                ProjectileType<CelestusBoomerang>(),
                ProjectileType<BrimbladeProj>(),
                ProjectileType<Brimblade2>(),
                ProjectileType<EnchantedAxeProj>(),
                ProjectileType<EpidemicShredderProjectile>(),
                ProjectileType<EquanimityProj>(),
                ProjectileType<EradicatorProjectile>(),
                ProjectileType<FlameScytheProjectile>(),
                ProjectileType<GhoulishGougerBoomerang>(),
                ProjectileType<GlaiveProj>(),
                ProjectileType<KylieBoomerang>(),
                ProjectileType<MangroveChakramProjectile>(),
                ProjectileType<MoltenAmputatorProj>(),
                ProjectileType<FallenPaladinsHammerProj>(),
                ProjectileType<SandDollarProj>(),
                ProjectileType<SandDollarStealth>(),
                ProjectileType<SeashellBoomerangProjectile>(),
                ProjectileType<ShroomerangProj>(),
                ProjectileType<TriactisHammerProj>(),
                ProjectileType<ValedictionBoomerang>(),
                ProjectileType<GalaxySmasherHammer>(),
                ProjectileType<KelvinCatalystBoomerang>(),
                ProjectileType<NanoblackMain>(),
                ProjectileType<StellarContemptHammer>(),
                ProjectileType<IcebreakerHammer>(),
                ProjectileType<PwnagehammerProj>(),
                ProjectileType<ValariBoomerang>(),
                ProjectileType<SphereSpiked>(),
                ProjectileType<SphereBladed>(),
                ProjectileType<SphereYellow>(),
                ProjectileType<ButcherKnife>(),
                ProjectileType<TerraDiskProjectile>(),
                ProjectileType<TerraDiskProjectile2>(),
                ProjectileType<ToxicantTwisterTwoPointZero>(),
                ProjectileType<TrackingDiskProjectile>()
            };

            javelinList = new List<int>()
            {
                ItemType<CrystalPiercer>(),
                ItemType<PalladiumJavelin>(),
                ItemType<DuneHopper>(),
                ItemType<EclipsesFall>(),
                ItemType<IchorSpear>(),
                ItemType<ProfanedTrident>(),
                ItemType<LuminousStriker>(),
                ItemType<ScarletDevil>(),
                ItemType<ScourgeoftheDesert>(),
                ItemType<ScourgeoftheSeas>(),
                ItemType<SpearofDestiny>(),
                ItemType<SpearofPaleolith>(),
                ItemType<XerocPitchfork>(),
                ItemType<PhantasmalRuin>(),
                ItemType<PhantomLance>(),
                ItemType<ProfanedPartisan>(),
                ItemType<Turbulance>(),
                ItemType<NightsGaze>(),
                ItemType<FrequencyManipulator>()
            };

            javelinProjList = new List<int>()
            {
                ProjectileType<CrystalPiercerProjectile>(),
                ProjectileType<DuneHopperProjectile>(),
                ProjectileType<EclipsesFallMain>(),
                ProjectileType<EclipsesStealth>(),
                ProjectileType<IchorSpearProj>(),
                ProjectileType<InfernalSpearProjectile>(),
                ProjectileType<LuminousStrikerProj>(),
                ProjectileType<PalladiumJavelinProjectile>(),
                ProjectileType<PhantasmalRuinProj>(),
                ProjectileType<PhantomLanceProj>(),
                ProjectileType<ProfanedPartisanProj>(),
                ProjectileType<ScarletDevilProjectile>(),
                ProjectileType<ScourgeoftheDesertProj>(),
                ProjectileType<ScourgeoftheSeasProjectile>(),
                ProjectileType<ScourgeoftheCosmosProj>(),
                ProjectileType<SpearofDestinyProjectile>(),
                ProjectileType<SpearofPaleolithProj>(),
                ProjectileType<AntumbraShardProjectile>(),
                ProjectileType<TurbulanceProjectile>(),
                ProjectileType<NightsGazeProjectile>(),
                ProjectileType<FrequencyManipulatorProjectile>()
            };

            daggerList = new List<int>()
            {
                ItemType<AshenStalactite>(),
                ItemType<CobaltKunai>(),
                ItemType<FeatherKnife>(),
                ItemType<GelDart>(),
                ItemType<MonkeyDarts>(),
                ItemType<MythrilKnife>(),
                ItemType<OrichalcumSpikedGemstone>(),
                ItemType<TarragonThrowingDart>(),
                ItemType<WulfrumKnife>(),
                ItemType<Cinquedea>(),
                ItemType<CosmicKunai>(),
                ItemType<CorpusAvertor>(),
                ItemType<Crystalline>(),
                ItemType<CursedDagger>(),
                ItemType<Malachite>(),
                ItemType<Mycoroot>(),
                ItemType<Prismalline>(),
                ItemType<Quasar>(),
                ItemType<RadiantStar>(),
                ItemType<ShatteredSun>(),
                ItemType<StellarKnife>(),
                ItemType<StormfrontRazor>(),
                ItemType<TimeBolt>(),
                ItemType<LunarKunai>(),
                ItemType<GildedDagger>(),
                ItemType<GleamingDagger>(),
                ItemType<InfernalKris>(),
                ItemType<UtensilPoker>(),
                ItemType<ShinobiBlade>(),
                ItemType<JawsOfOblivion>(),
                ItemType<LeviathanTeeth>(),
                ItemType<DeificThunderbolt>()
            };

            daggerProjList = new List<int>()
            {
                ProjectileType<AshenStalactiteProj>(),
                ProjectileType<AshenStalagmiteProj>(),
                ProjectileType<CinquedeaProj>(),
                ProjectileType<CobaltKunaiProjectile>(),
                ProjectileType<CosmicKunaiProj>(),
                ProjectileType<CrystallineProj>(),
                ProjectileType<Crystalline2>(),
                ProjectileType<CursedDaggerProj>(),
                ProjectileType<EmpyreanKnife>(),
                ProjectileType<FeatherKnifeProjectile>(),
                ProjectileType<GelDartProjectile>(),
                ProjectileType<GildedDaggerProj>(),
                ProjectileType<GleamingDaggerProj>(),
                ProjectileType<IllustriousKnife>(),
                ProjectileType<LunarKunaiProj>(),
                ProjectileType<MalachiteProj>(),
                ProjectileType<MalachiteBolt>(),
                ProjectileType<MalachiteStealth>(),
                ProjectileType<MonkeyDart>(),
                ProjectileType<MycorootProj>(),
                ProjectileType<MythrilKnifeProjectile>(),
                ProjectileType<OrichalcumSpikedGemstoneProjectile>(),
                ProjectileType<PrismallineProj>(),
                ProjectileType<Prismalline2>(),
                ProjectileType<Prismalline3>(),
                ProjectileType<QuasarKnife>(),
                ProjectileType<Quasar2>(),
                ProjectileType<RadiantStarKnife>(),
                ProjectileType<RadiantStar2>(),
                ProjectileType<ShatteredSunKnife>(),
                ProjectileType<StellarKnifeProj>(),
                ProjectileType<StormfrontRazorProjectile>(),
                ProjectileType<TarragonThrowingDartProjectile>(),
                ProjectileType<TimeBoltKnife>(),
                ProjectileType<WulfrumKnifeProj>(),
                ProjectileType<Fork>(),
                ProjectileType<Knife>(),
                ProjectileType<CarvingFork>(),
                ProjectileType<InfernalKrisProjectile>(),
                ProjectileType<ShinobiBladeProjectile>(),
                ProjectileType<JawsProjectile>(),
                ProjectileType<LeviathanTooth>(),
                ProjectileType<DeificThunderboltProj>()
            };

            flaskBombList = new List<int>()
            {
                ItemType<Plaguenade>(),
                ItemType<BallisticPoisonBomb>(),
                ItemType<BrackishFlask>(),
                ItemType<DuststormInABottle>(),
                ItemType<SeafoamBomb>(),
                ItemType<ConsecratedWater>(),
                ItemType<DesecratedWater>(),
                ItemType<BouncingBetty>(),
                ItemType<TotalityBreakers>(),
                ItemType<BlastBarrel>(),
                ItemType<Penumbra>(),
                ItemType<LatcherMine>(),
                ItemType<Supernova>(),
                ItemType<ShockGrenade>(),
                ItemType<Exorcism>(),
                ItemType<MeteorFist>(),
                ItemType<StarofDestruction>(),
                ItemType<CraniumSmasher>(),
                ItemType<ContaminatedBile>(),
                ItemType<AcidicRainBarrel>(),
                ItemType<SkyfinBombers>(),
                ItemType<SpentFuelContainer>(),
                ItemType<SealedSingularity>(),
                ItemType<PlasmaGrenade>(),
                ItemType<WavePounder>()
            };

            flaskBombProjList = new List<int>()
            {
                ProjectileType<BallisticPoisonBombProj>(),
                ProjectileType<BlastBarrelProjectile>(),
                ProjectileType<BouncingBettyProjectile>(),
                ProjectileType<BrackishFlaskProj>(),
                ProjectileType<DuststormInABottleProj>(),
                ProjectileType<PlaguenadeProj>(),
                ProjectileType<SeafoamBombProj>(),
                ProjectileType<TotalityFlask>(),
                ProjectileType<ConsecratedWaterProjectile>(),
                ProjectileType<DesecratedWaterProj>(),
                ProjectileType<PenumbraBomb>(),
                ProjectileType<LatcherMineProjectile>(),
                ProjectileType<SupernovaBomb>(),
                ProjectileType<ShockGrenadeProjectile>(),
                ProjectileType<ExorcismProj>(),
                ProjectileType<MeteorFistProj>(),
                ProjectileType<CraniumSmasherProj>(),
                ProjectileType<CraniumSmasherExplosive>(),
                ProjectileType<CraniumSmasherStealth>(),
                ProjectileType<DestructionStar>(),
                ProjectileType<DestructionBolt>(),
                ProjectileType<ContaminatedBileFlask>(),
                ProjectileType<GreenDonkeyKongReference>(),
                ProjectileType<SkyfinNuke>(),
                ProjectileType<SpentFuelContainerProjectile>(),
                ProjectileType<SealedSingularityProj>(),
                ProjectileType<PlasmaGrenadeProjectile>(),
                ProjectileType<WavePounderProjectile>()
            };

            spikyBallList = new List<int>()
            {
                ItemType<BouncySpikyBall>(),
                ItemType<GodsParanoia>(),
                ItemType<NastyCholla>(),
                ItemType<HellsSun>(),
                ItemType<SkyStabber>(),
                ItemType<StickySpikyBall>(),
                ItemType<WebBall>(),
                ItemType<PoisonPack>(),
                ItemType<Nychthemeron>(),
                ItemType<MetalMonstrosity>(),
                ItemType<BurningStrife>(),
                ItemType<SystemBane>()
            };

            spikyBallProjList = new List<int>()
            {
                ProjectileType<BouncyBol>(),
                ProjectileType<GodsParanoiaProj>(),
                ProjectileType<HellsSunProj>(),
                ProjectileType<NastyChollaBol>(),
                ProjectileType<StickyBol>(),
                ProjectileType<SkyStabberProj>(),
                ProjectileType<WebBallBol>(),
                ProjectileType<PoisonBol>(),
                ProjectileType<NychthemeronProjectile>(),
                ProjectileType<MetalChunk>(),
                ProjectileType<BurningStrifeProj>(),
                ProjectileType<SystemBaneProjectile>()
            };

            noGravityList = new List<int>()
            {
                ItemType<AuricBar>(),
                ItemType<EssenceofChaos>(),
                ItemType<EssenceofCinder>(),
                ItemType<EssenceofEleum>(),
                ItemType<CoreofChaos>(),
                ItemType<CoreofCinder>(),
                ItemType<CoreofEleum>(),
                ItemType<CoreofCalamity>(),
                ItemType<HellcasterFragment>(),
                ItemType<TwistingNether>(),
                ItemType<DarkPlasma>(),
                ItemType<DarksunFragment>(),
                ItemType<UnholyEssence>(),
                ItemType<GalacticaSingularity>(),
                ItemType<NightmareFuel>(),
                ItemType<EndothermicEnergy>(),
                ItemType<SoulofCryogen>(),
                ItemType<AscendantSpiritEssence>(),

                ItemType<KnowledgeAquaticScourge>(),
                ItemType<KnowledgeAstralInfection>(),
                ItemType<KnowledgeAstrumAureus>(),
                ItemType<KnowledgeAstrumDeus>(),
                ItemType<KnowledgeBloodMoon>(),
                ItemType<KnowledgeBrainofCthulhu>(),
                ItemType<KnowledgeBrimstoneCrag>(),
                ItemType<KnowledgeBrimstoneElemental>(),
                ItemType<KnowledgeBumblebirb>(),
                ItemType<KnowledgeCalamitas>(),
                ItemType<KnowledgeCalamitasClone>(),
                ItemType<KnowledgeCorruption>(),
                ItemType<KnowledgeCrabulon>(),
                ItemType<KnowledgeCrimson>(),
                ItemType<KnowledgeCryogen>(),
                ItemType<KnowledgeDesertScourge>(),
                ItemType<KnowledgeDestroyer>(),
                ItemType<KnowledgeDevourerofGods>(),
                ItemType<KnowledgeDukeFishron>(),
                ItemType<KnowledgeEaterofWorlds>(),
                ItemType<KnowledgeEyeofCthulhu>(),
                ItemType<KnowledgeGolem>(),
                ItemType<KnowledgeHiveMind>(),
                ItemType<KnowledgeKingSlime>(),
                ItemType<KnowledgeLeviathanandSiren>(),
                ItemType<KnowledgeLunaticCultist>(),
                ItemType<KnowledgeMechs>(),
                ItemType<KnowledgeMoonLord>(),
                ItemType<KnowledgeOcean>(),
                ItemType<KnowledgeOldDuke>(),
                ItemType<KnowledgePerforators>(),
                ItemType<KnowledgePlaguebringerGoliath>(),
                ItemType<KnowledgePlantera>(),
                ItemType<KnowledgePolterghast>(),
                ItemType<KnowledgeProfanedGuardians>(),
                ItemType<KnowledgeProvidence>(),
                ItemType<KnowledgeQueenBee>(),
                ItemType<KnowledgeRavager>(),
                ItemType<KnowledgeSentinels>(),
                ItemType<KnowledgeSkeletron>(),
                ItemType<KnowledgeSkeletronPrime>(),
                ItemType<KnowledgeSlimeGod>(),
                ItemType<KnowledgeSulphurSea>(),
                ItemType<KnowledgeTwins>(),
                ItemType<KnowledgeUnderworld>(),
                ItemType<KnowledgeWallofFlesh>(),
                ItemType<KnowledgeYharon>(),
            };

            lavaFishList = new List<int>()
            {
                ItemType<SlurperPole>(),
                ItemType<ChaoticSpreadRod>(),
                ItemType<TheDevourerofCods>()
            };

            highTestFishList = new List<int>()
            {
                ItemID.GoldenFishingRod,
                ItemType<EarlyBloomRod>(),
                ItemType<TheDevourerofCods>()
            };

            flamethrowerList = new List<int>()
            {
                ItemType<DragoonDrizzlefish>(),
                ItemType<BloodBoiler>()
            };

            forceItemList = new List<int>()
            {
                ItemType<SubmarineShocker>(),
                ItemType<Barinautical>(),
                ItemType<Downpour>(),
                ItemType<DeepseaStaff>(),
                ItemType<ScourgeoftheSeas>(),
                ItemType<InsidiousImpaler>(),
                ItemType<SepticSkewer>(),
                ItemType<FetidEmesis>(),
                ItemType<VitriolicViper>(),
                ItemType<CadaverousCarrion>(),
                ItemType<ToxicantTwister>(),
                ItemType<DukeScales>(),
                ItemType<Greentide>(),
                ItemType<Leviatitan>(),
                ItemType<Atlantis>(),
                ItemType<SirensSong>(),
                ItemType<BrackishFlask>(),
                ItemType<LeviathanTeeth>(),
                ItemType<GastricBelcherStaff>(),
                ItemType<LureofEnthrallment>(),
                ItemType<AquaticScourgeBag>(),
                ItemType<OldDukeBag>(),
                ItemType<LeviathanBag>(),
                ItemType<OldDukeMask>(),
                ItemType<LeviathanMask>(),
                ItemType<AquaticScourgeMask>(),
                ItemType<OldDukeTrophy>(),
                ItemType<LeviathanTrophy>(),
                ItemType<AquaticScourgeTrophy>(),
                ItemType<KnowledgeAquaticScourge>(),
                ItemType<KnowledgeLeviathanandSiren>(),
                ItemType<KnowledgeSulphurSea>(),
                ItemType<KnowledgeOcean>(),
                ItemType<KnowledgeOldDuke>(),
                ItemType<VictoryShard>(),
                ItemType<AeroStone>(),
                ItemType<DukesDecapitator>(),
                ItemType<SulphurousSand>(),
                ItemType<MagnumRounds>(),
                ItemType<GrenadeRounds>(),
                ItemType<ExplosiveShells>(),
                ItemID.HotlineFishingHook,
                ItemID.BottomlessBucket,
                ItemID.SuperAbsorbantSponge,
                ItemID.FishingPotion,
                ItemID.SonarPotion,
                ItemID.CratePotion,
                ItemID.AnglerTackleBag,
                ItemID.HighTestFishingLine,
                ItemID.TackleBox,
                ItemID.AnglerEarring,
                ItemID.FishermansGuide,
                ItemID.WeatherRadio,
                ItemID.Sextant,
                ItemID.AnglerHat,
                ItemID.AnglerVest,
                ItemID.AnglerPants,
                ItemID.GoldenBugNet,
                ItemID.FishronWings,
                ItemID.Flairon,
                ItemID.Tsunami,
                ItemID.BubbleGun,
                ItemID.RazorbladeTyphoon,
                ItemID.TempestStaff,
                ItemID.FishronBossBag,
                ItemID.Coral,
                ItemID.Seashell,
                ItemID.Starfish,
                ItemID.SoulofSight,
                ItemID.GreaterHealingPotion,
                ItemID.SuperHealingPotion
            };

            livingFireBlockList = new List<int>()
            {
                TileType<LivingGodSlayerFireBlockTile>(),
                TileType<LivingHolyFireBlockTile>(),
                TileType<LivingBrimstoneFireBlockTile>(),
                TileID.LivingFire,
                TileID.LivingCursedFire,
                TileID.LivingDemonFire,
                TileID.LivingFrostFire,
                TileID.LivingIchor,
                TileID.LivingUltrabrightFire
            };

            zombieList = new List<int>()
            {
                NPCID.Zombie,
                NPCID.ArmedZombie,
                NPCID.BaldZombie,
                NPCID.PincushionZombie,
                NPCID.ArmedZombiePincussion, // what is this spelling
                NPCID.SlimedZombie,
                NPCID.ArmedZombieSlimed,
                NPCID.SwampZombie,
                NPCID.ArmedZombieSwamp,
                NPCID.TwiggyZombie,
                NPCID.ArmedZombieTwiggy,
                NPCID.FemaleZombie,
                NPCID.ArmedZombieCenx,
                NPCID.ZombieRaincoat,
                NPCID.ZombieEskimo,
                NPCID.ArmedZombieEskimo,
                NPCID.BigRainZombie,
                NPCID.SmallRainZombie,
                NPCID.BigFemaleZombie,
                NPCID.SmallFemaleZombie,
                NPCID.BigTwiggyZombie,
                NPCID.SmallTwiggyZombie,
                NPCID.BigSwampZombie,
                NPCID.SmallSwampZombie,
                NPCID.BigSlimedZombie,
                NPCID.SmallSlimedZombie,
                NPCID.BigPincushionZombie,
                NPCID.SmallPincushionZombie,
                NPCID.BigBaldZombie,
                NPCID.SmallBaldZombie,
                NPCID.BigZombie,
                NPCID.SmallZombie
                // halloween zombies not included because they don't drop shackles or zombie arms
            };

            demonEyeList = new List<int>()
            {
                NPCID.DemonEye,
                NPCID.CataractEye,
                NPCID.SleepyEye,
                NPCID.DialatedEye, // it is spelled "dilated"
                NPCID.GreenEye,
                NPCID.PurpleEye,
                NPCID.DemonEyeOwl,
                NPCID.DemonEyeSpaceship,
                NPCID.DemonEye2,
                NPCID.PurpleEye2,
                NPCID.GreenEye2,
                NPCID.DialatedEye2,
                NPCID.SleepyEye2,
                NPCID.CataractEye2
            };

            skeletonList = new List<int>()
            {
                NPCID.Skeleton,
                NPCID.HeadacheSkeleton,
                NPCID.MisassembledSkeleton,
                NPCID.PantlessSkeleton,
                NPCID.BoneThrowingSkeleton,
                NPCID.BoneThrowingSkeleton2,
                NPCID.BoneThrowingSkeleton3,
                NPCID.BoneThrowingSkeleton4,
                NPCID.BigPantlessSkeleton,
                NPCID.SmallPantlessSkeleton,
                NPCID.BigMisassembledSkeleton,
                NPCID.SmallMisassembledSkeleton,
                NPCID.BigHeadacheSkeleton,
                NPCID.SmallHeadacheSkeleton,
                NPCID.BigSkeleton,
                NPCID.SmallSkeleton,

                //Note: These skeletons don't count for Skeleton Banner for some god forsaken reason
                NPCID.SkeletonTopHat,
                NPCID.SkeletonAstonaut,
                NPCID.SkeletonAlien,

                //Other skeleton types
                NPCID.ArmoredSkeleton,
                NPCID.HeavySkeleton,
                NPCID.SkeletonArcher,
                NPCID.GreekSkeleton
            };

            angryBonesList = new List<int>()
            {
                NPCID.AngryBones,
                NPCID.AngryBonesBig,
                NPCID.AngryBonesBigMuscle,
                NPCID.AngryBonesBigHelmet,
                NPCID.BigBoned,
                NPCID.ShortBones
            };

            hornetList = new List<int>()
            {
                NPCID.BigHornetStingy,
                NPCID.LittleHornetStingy,
                NPCID.BigHornetSpikey,
                NPCID.LittleHornetSpikey,
                NPCID.BigHornetLeafy,
                NPCID.LittleHornetLeafy,
                NPCID.BigHornetHoney,
                NPCID.LittleHornetHoney,
                NPCID.BigHornetFatty,
                NPCID.LittleHornetFatty,
                NPCID.BigStinger,
                NPCID.LittleStinger,
                NPCID.Hornet,
                NPCID.HornetFatty,
                NPCID.HornetHoney,
                NPCID.HornetLeafy,
                NPCID.HornetSpikey,
                NPCID.HornetStingy
            };

            mossHornetList = new List<int>()
            {
                NPCID.MossHornet,
                NPCID.TinyMossHornet,
                NPCID.LittleMossHornet,
                NPCID.BigMossHornet,
                NPCID.GiantMossHornet
            };

            minibossList = new List<int>()
            {
                NPCType<EidolonWyrmHead>(),
                NPCType<Mauler>(),
                NPCType<Reaper>(),
                NPCType<ColossalSquid>(),
                NPCType<GreatSandShark>(),
                NPCType<GiantClam>(),
                NPCType<ArmoredDiggerHead>(),
                NPCType<ArmoredDiggerBody>(),
                NPCType<ArmoredDiggerTail>(),
                NPCType<ThiccWaifu>(),
                NPCType<Horse>(),
                NPCType<PlaguebringerShade>(),
                NPCID.Pumpking,
                NPCID.MourningWood,
                NPCID.IceQueen,
                NPCID.SantaNK1,
                NPCID.Everscream,
                NPCID.DD2Betsy,
                NPCID.Mothron
            };

			heartDropBlockList = new List<int>()
			{
				NPCID.BlueSlime,
				NPCID.YellowSlime,
				NPCID.RedSlime,
				NPCID.PurpleSlime,
				NPCID.GreenSlime,
				NPCID.IceSlime,
				NPCID.SlimeSpiked,
				NPCID.UmbrellaSlime,
				NPCID.RainbowSlime,
				NPCID.Pinky,
				NPCID.ServantofCthulhu,
				NPCID.EaterofWorldsHead,
				NPCID.EaterofWorldsBody,
				NPCID.EaterofWorldsTail,
				NPCID.Creeper,
				NPCID.TheHungryII,
				NPCID.LeechHead,
				NPCID.LeechBody,
				NPCID.LeechTail,
				NPCID.Probe,
				NPCID.Bee,
				NPCID.BeeSmall,
				NPCID.PlanterasTentacle,
				NPCID.Sharkron,
				NPCID.Sharkron2,
				NPCType<DesertScourgeHeadSmall>(),
				NPCType<DriedSeekerHead>(),
				NPCType<CrabShroom>(),
				NPCType<DankCreeper>(),
				NPCID.DevourerHead,
				NPCID.EaterofSouls,
				NPCType<PerforatorHeadLarge>(),
				NPCType<PerforatorHeadMedium>(),
				NPCType<PerforatorHeadSmall>(),
				NPCType<SlimeSpawnCorrupt2>(),
				NPCType<SlimeSpawnCrimson>(),
				NPCType<SlimeSpawnCrimson2>(),
				NPCType<SlimeGod>(),
				NPCType<SlimeGodRun>(),
				NPCType<SlimeGodSplit>(),
				NPCType<SlimeGodRunSplit>(),
				NPCType<IceMass>(),
				NPCType<Cryocore>(),
				NPCType<Cryocore2>(),
				NPCType<Parasea>(),
				NPCType<AquaticAberration>(),
				NPCType<PlagueBeeG>(),
				NPCType<PlagueBeeLargeG>(),
				NPCType<PlaguebringerShade>(),
				NPCType<RavagerClawLeft>(),
				NPCType<RavagerClawRight>(),
				NPCType<Bumblefuck2>(),
				NPCType<DarkEnergy>(),
				NPCType<CosmicLantern>(),
				NPCType<OldDukeSharkron>(),
				NPCType<OldDukeToothBall>(),
				NPCType<DetonatingFlare>(),
				NPCType<DetonatingFlare2>()
			};

			pierceResistList = new List<int>()
			{
				NPCID.EaterofWorldsHead,
				NPCID.EaterofWorldsBody,
				NPCID.EaterofWorldsTail,
				NPCID.Creeper,
				NPCID.TheDestroyer,
				NPCID.TheDestroyerBody,
				NPCID.TheDestroyerTail,
				NPCType<DesertScourgeHead>(),
				NPCType<DesertScourgeBody>(),
				NPCType<DesertScourgeTail>(),
				NPCType<PerforatorHeadLarge>(),
				NPCType<PerforatorBodyLarge>(),
				NPCType<PerforatorTailLarge>(),
				NPCType<PerforatorHeadMedium>(),
				NPCType<PerforatorBodyMedium>(),
				NPCType<PerforatorTailMedium>(),
				NPCType<PerforatorHeadSmall>(),
				NPCType<PerforatorBodySmall>(),
				NPCType<PerforatorTailSmall>(),
				NPCType<AquaticScourgeHead>(),
				NPCType<AquaticScourgeBody>(),
				NPCType<AquaticScourgeBodyAlt>(),
				NPCType<AquaticScourgeTail>(),
				NPCType<AstrumDeusHeadSpectral>(),
				NPCType<AstrumDeusBodySpectral>(),
				NPCType<AstrumDeusTailSpectral>(),
				NPCType<DarkEnergy>(),
				NPCType<StormWeaverHeadNaked>(),
				NPCType<StormWeaverBodyNaked>(),
				NPCType<StormWeaverTailNaked>(),
				NPCType<DevourerofGodsHead>(),
				NPCType<DevourerofGodsBody>(),
				NPCType<DevourerofGodsTail>(),
				NPCType<DevourerofGodsHead2>(),
				NPCType<DevourerofGodsBody2>(),
				NPCType<DevourerofGodsTail2>(),
				NPCType<DevourerofGodsHeadS>(),
				NPCType<DevourerofGodsBodyS>(),
				NPCType<DevourerofGodsTailS>(),
				NPCType<ThanatosHead>(),
				NPCType<ThanatosBody1>(),
				NPCType<ThanatosBody2>(),
				NPCType<ThanatosTail>(),
				NPCType<BrimstoneHeart>()
			};

			bossMinionList = new List<int>()
            {
                NPCType<DesertScourgeHeadSmall>(),
                NPCType<DesertScourgeBodySmall>(),
                NPCType<DesertScourgeTailSmall>(),
                NPCID.SlimeSpiked,
                NPCID.ServantofCthulhu,
                NPCType<CrabShroom>(),
                NPCID.EaterofWorldsHead,
                NPCID.EaterofWorldsBody,
                NPCID.EaterofWorldsTail,
                NPCID.Creeper,
                NPCType<PerforatorHeadSmall>(),
                NPCType<PerforatorBodySmall>(),
                NPCType<PerforatorTailSmall>(),
                NPCType<PerforatorHeadMedium>(),
                NPCType<PerforatorBodyMedium>(),
                NPCType<PerforatorTailMedium>(),
                NPCType<PerforatorHeadLarge>(),
                NPCType<PerforatorBodyLarge>(),
                NPCType<PerforatorTailLarge>(),
                NPCType<HiveBlob>(),
                NPCType<HiveBlob2>(),
                NPCType<DankCreeper>(),
                NPCID.SkeletronHand,
                NPCType<SlimeGod>(),
                NPCType<SlimeGodSplit>(),
                NPCType<SlimeGodRun>(),
                NPCType<SlimeGodRunSplit>(),
                NPCType<SlimeSpawnCorrupt>(),
                NPCType<SlimeSpawnCorrupt2>(),
                NPCType<SlimeSpawnCrimson>(),
                NPCType<SlimeSpawnCrimson2>(),
                NPCID.LeechHead,
                NPCID.LeechBody,
                NPCID.LeechTail,
                NPCID.WallofFleshEye,
                NPCID.TheHungry,
                NPCID.TheHungryII,
                NPCType<Cryocore>(),
                NPCType<Cryocore2>(),
                NPCType<IceMass>(),
                NPCID.PrimeCannon,
                NPCID.PrimeLaser,
                NPCID.PrimeSaw,
                NPCID.PrimeVice,
                NPCType<Brimling>(),
                NPCID.TheDestroyer,
                NPCID.TheDestroyerBody,
                NPCID.TheDestroyerTail,
                NPCType<AquaticScourgeHead>(),
                NPCType<AquaticScourgeBody>(),
                NPCType<AquaticScourgeBodyAlt>(),
                NPCType<AquaticScourgeTail>(),
                NPCType<CalamitasRun>(),
                NPCType<CalamitasRun2>(),
                NPCType<LifeSeeker>(),
                NPCType<SoulSeeker>(),
                NPCID.PlanterasTentacle,
                NPCType<AureusSpawn>(),
                NPCID.Spore,
                NPCID.GolemHead,
                NPCID.GolemHeadFree,
                NPCID.GolemFistLeft,
                NPCID.GolemFistRight,
                NPCType<PlagueMine>(),
                NPCType<PlagueHomingMissile>(),
                NPCType<PlagueBeeG>(),
                NPCType<PlagueBeeLargeG>(),
                NPCType<RavagerClawLeft>(),
                NPCType<RavagerClawRight>(),
                NPCType<RavagerLegLeft>(),
                NPCType<RavagerLegRight>(),
                NPCType<RavagerHead>(),
                NPCID.CultistDragonHead,
                NPCID.CultistDragonBody1,
                NPCID.CultistDragonBody2,
                NPCID.CultistDragonBody3,
                NPCID.CultistDragonBody4,
                NPCID.CultistDragonTail,
                NPCID.AncientCultistSquidhead,
                NPCID.MoonLordFreeEye,
                NPCID.MoonLordHand,
                NPCID.MoonLordHead,
                NPCType<Bumblefuck2>(),
                NPCType<ProvSpawnOffense>(),
                NPCType<ProvSpawnDefense>(),
                NPCType<ProvSpawnHealer>(),
                NPCType<DarkEnergy>(),
                NPCType<CosmicLantern>(),
                NPCType<DevourerofGodsHead2>(),
                NPCType<DevourerofGodsBody2>(),
                NPCType<DevourerofGodsTail2>(),
                NPCType<DetonatingFlare>(),
                NPCType<DetonatingFlare2>(),
                NPCType<SupremeCataclysm>(),
                NPCType<SupremeCatastrophe>()
            };

            legOverrideList = new List<int>()
            {
                CalamityMod.Instance.GetEquipSlot("ProviLegs", EquipType.Legs),
                CalamityMod.Instance.GetEquipSlot("SirenLegAlt", EquipType.Legs),
                CalamityMod.Instance.GetEquipSlot("SirenLeg", EquipType.Legs),
                CalamityMod.Instance.GetEquipSlot("PopoLeg", EquipType.Legs)
            };

            // Duke Fishron and Old Duke phase 3 becomes way too easy if you can make him stop being invisible with Yanmei's Knife.
            // This is a list so that other NPCs can be added as necessary.
            // IT DOES NOT make them immune to the debuff, just stops them from being recolored.
            kamiDebuffColorImmuneList = new List<int>()
            {
                NPCID.DukeFishron,
                NPCType<OldDuke>()
            };

            MinionsToNotResurrectList = new List<int>()
            {
                ProjectileID.StardustDragon2,
                ProjectileID.StardustDragon3,
                ProjectileID.StardustDragon4,
                ProjectileType<MechwormBody>(),
                ProjectileType<MechwormTail>(),
                ProjectileType<EndoHydraHead>()
            };

            ZeroMinionSlotExceptionList = new List<int>()
            {
                ProjectileID.StardustDragon1,
                ProjectileType<MechwormHead>(),
                ProjectileType<EndoHydraBody>()
            };

            DontCopyOriginalMinionAIList = new List<int>()
            {
                ProjectileType<GammaHead>()
            };
        }

        public static void UnloadLists()
        {
            donatorList = null;
            trueMeleeProjectileList = null;
            rangedProjectileExceptionList = null;
            projectileDestroyExceptionList = null;
            projectileMinionList = null;
            enemyImmunityList = null;
            confusionEnemyList = null;
            dungeonEnemyBuffList = null;
            dungeonProjectileBuffList = null;
            bossHPScaleList = null;
            beeEnemyList = null;
            friendlyBeeList = null;
            beeProjectileList = null;
            hardModeNerfList = null;
            debuffList = null;
            fireWeaponList = null;
            iceWeaponList = null;
            natureWeaponList = null;
            alcoholList = null;
			useTurnList = null;
			twentyUseTimeBuffList = null;
			fiftySizeBuffList = null;
			quadrupleDamageBuffList = null;
			tripleDamageBuffList = null;
            doubleDamageBuffList = null;
            sixtySixDamageBuffList = null;
            fiftyDamageBuffList = null;
            thirtyThreeDamageBuffList = null;
            twentyFiveDamageBuffList = null;
			twentyDamageBuffList = null;
            tenDamageBuffList = null;
            weaponAutoreuseList = null;
			spearAutoreuseList = null;
            tenDamageNerfList = null;
            quarterDamageNerfList = null;
            pumpkinMoonBuffList = null;
            frostMoonBuffList = null;
            eclipseBuffList = null;
            eventProjectileBuffList = null;
            revengeanceEnemyBuffList25Percent = null;
            revengeanceEnemyBuffList20Percent = null;
            revengeanceEnemyBuffList15Percent = null;
            revengeanceEnemyBuffList10Percent = null;
            revengeanceProjectileBuffList25Percent = null;
            revengeanceProjectileBuffList20Percent = null;
            revengeanceProjectileBuffList15Percent = null;
            revengeanceLifeStealExceptionList = null;
            noRageWormSegmentList = null;
            needsDebuffIconDisplayList = null;
            trapProjectileList = null;
            scopedWeaponList = null;
            boomerangList = null;
            javelinList = null;
            daggerList = null;
            flaskBombList = null;
            spikyBallList = null;
            boomerangProjList = null;
            javelinProjList = null;
            daggerProjList = null;
            flaskBombProjList = null;
            spikyBallProjList = null;
            noGravityList = null;
            lavaFishList = null;
            highTestFishList = null;
            flamethrowerList = null;
            forceItemList = null;
            livingFireBlockList = null;

            zombieList = null;
            demonEyeList = null;
            skeletonList = null;
            angryBonesList = null;
            hornetList = null;
            mossHornetList = null;
            bossMinionList = null;
            minibossList = null;
			heartDropBlockList = null;
			pierceResistList = null;

            legOverrideList = null;

            kamiDebuffColorImmuneList = null;

            MinionsToNotResurrectList = null;
            ZeroMinionSlotExceptionList = null;
            DontCopyOriginalMinionAIList = null;
        }
    }
}
