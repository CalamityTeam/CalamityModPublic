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
using CalamityMod.NPCs.GreatSandShark;
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
        public static List<int> doubleDamageBuffList; //100% buff
        public static List<int> sixtySixDamageBuffList; //66% buff
        public static List<int> fiftyDamageBuffList; //50% buff
        public static List<int> thirtyThreeDamageBuffList; //33% buff
        public static List<int> twentyFiveDamageBuffList; //25% buff
        public static List<int> twentyDamageBuffList; //20% buff
        public static List<int> tenDamageBuffList; //10% buff
        public static List<int> weaponAutoreuseList;
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
        public static List<int> movementImpairImmuneList;
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

        public static List<int> legOverrideList;

        public static List<int> kamiDebuffColorImmuneList;

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
                "Termi",
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
                ProjectileID.ChlorophytePartisan,
                ProjectileID.MonkStaffT1,
                ProjectileID.MonkStaffT2,
                ProjectileID.MonkStaffT3,
                ProjectileID.NorthPoleWeapon,

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
                ModContent.ProjectileType<DevilsSunriseProj>(),
                ModContent.ProjectileType<MarniteObliteratorProj>(),
                ModContent.ProjectileType<MurasamaSlash>(),
                ModContent.ProjectileType<AmidiasTridentProj>(),
                ModContent.ProjectileType<AstralPikeProj>(),
                ModContent.ProjectileType<BansheeHookProj>(),
                ModContent.ProjectileType<BrimlanceProj>(),
                ModContent.ProjectileType<DiseasedPikeSpear>(),
                ModContent.ProjectileType<EarthenPikeSpear>(),
                ModContent.ProjectileType<ExsanguinationLanceProjectile>(),
                ModContent.ProjectileType<FulgurationHalberdProj>(),
                ModContent.ProjectileType<GildedProboscisProj>(),
                ModContent.ProjectileType<GoldplumeSpearProjectile>(),
                ModContent.ProjectileType<HellionFlowerSpearProjectile>(),
                ModContent.ProjectileType<InsidiousImpalerProj>(),
                ModContent.ProjectileType<MarniteSpearProjectile>(),
                ModContent.ProjectileType<NadirSpear>(),
                ModContent.ProjectileType<SausageMakerSpear>(),
                ModContent.ProjectileType<SpatialLanceProjectile>(),
                ModContent.ProjectileType<StarnightLanceProjectile>(),
                ModContent.ProjectileType<StreamGougeProj>(),
                ModContent.ProjectileType<TenebreusTidesProjectile>(),
                ModContent.ProjectileType<TerraLanceProjectile>(),
                ModContent.ProjectileType<TyphonsGreedStaff>(),
                ModContent.ProjectileType<UrchinSpearProjectile>(),
                ModContent.ProjectileType<YateveoBloomSpear>(),
                ModContent.ProjectileType<HydraulicVoltCrasherProjectile>()
            };

            rangedProjectileExceptionList = new List<int>()
            {
                ProjectileID.Phantasm,
                ProjectileID.VortexBeater,
                ProjectileID.DD2PhoenixBow,
                ProjectileID.IchorDart,
                ProjectileID.PhantasmArrow,
                ProjectileID.RainbowBack,
                ModContent.ProjectileType<PhangasmBow>(),
                ModContent.ProjectileType<ContagionBow>(),
                ModContent.ProjectileType<DaemonsFlameBow>(),
                ModContent.ProjectileType<ExoTornado>(),
                ModContent.ProjectileType<DrataliornusBow>(),
                ModContent.ProjectileType<FlakKrakenGun>(),
                ModContent.ProjectileType<ButcherGun>(),
                ModContent.ProjectileType<StarfleetMK2Gun>(),
                ModContent.ProjectileType<TerraBulletSplit>(),
                ModContent.ProjectileType<TerraArrowSplit>(),
                ModContent.ProjectileType<HyperiusSplit>(),
                ModContent.ProjectileType<NorfleetCannon>(),
                ModContent.ProjectileType<NorfleetComet>(),
                ModContent.ProjectileType<NorfleetExplosion>(),
                ModContent.ProjectileType<AetherBeam>(),
                ModContent.ProjectileType<FlurrystormCannonShooting>(),
                ModContent.ProjectileType<MagnomalyBeam>(),
                ModContent.ProjectileType<MagnomalyAura>(),
                ModContent.ProjectileType<RainbowTrail>(),
                ModContent.ProjectileType<PrismaticBeam>(),
                ModContent.ProjectileType<ExoLight>(),
                ModContent.ProjectileType<ExoLightBomb>(),
                ModContent.ProjectileType<UltimaBowProjectile>(),
                ModContent.ProjectileType<UltimaSpark>(), // Because of potential dust lag.
                ModContent.ProjectileType<UltimaRay>()
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
                ModContent.ProjectileType<PhangasmBow>(),
                ModContent.ProjectileType<ContagionBow>(),
                ModContent.ProjectileType<DaemonsFlameBow>(),
                ModContent.ProjectileType<DrataliornusBow>(),
                ModContent.ProjectileType<FlakKrakenGun>(),
                ModContent.ProjectileType<ButcherGun>(),
                ModContent.ProjectileType<StarfleetMK2Gun>(),
                ModContent.ProjectileType<NorfleetCannon>(),
                ModContent.ProjectileType<FlurrystormCannonShooting>(),
                ModContent.ProjectileType<PurgeProj>(),
                ModContent.ProjectileType<T1000Proj>(),
                ModContent.ProjectileType<YharimsCrystalPrism>(),
                ModContent.ProjectileType<DarkSparkPrism>(),
                ModContent.ProjectileType<YharimsCrystalBeam>(),
                ModContent.ProjectileType<DarkSparkBeam>(),
                ModContent.ProjectileType<GhastlyVisageProj>(),
                // ModContent.ProjectileType<ApotheosisWorm>(), // TODO: ADD THE APOTHEOSIS WORM HERE LATER. IT IS IN A DIFFERENT BRANCH AS OF WRITING THIS.

                ModContent.ProjectileType<FlakKrakenProj>(),
                ModContent.ProjectileType<SylvanSlashAttack>(),
                ModContent.ProjectileType<InfernadoFriendly>(),
                ModContent.ProjectileType<MurasamaSlash>(),
                ModContent.ProjectileType<PhaseslayerProjectile>(),

                //Some hostile boss projectiles
                ModContent.ProjectileType<BrimstoneMonster>(),
                ModContent.ProjectileType<InfernadoRevenge>(),
                ModContent.ProjectileType<OverlyDramaticDukeSummoner>(),
                ModContent.ProjectileType<ProvidenceHolyRay>(),
                ModContent.ProjectileType<OldDukeVortex>(),
                ModContent.ProjectileType<BrimstoneRay>(),
                ModContent.ProjectileType<BrimstoneTargetRay>()
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
                NPCID.Retinazer,
                NPCID.Spazmatism,
                NPCID.SkeletronPrime,
                NPCID.PrimeCannon,
                NPCID.PrimeSaw,
                NPCID.PrimeLaser,
                NPCID.PrimeVice,
                NPCID.Plantera,
                NPCID.IceQueen,
                NPCID.Pumpking,
                NPCID.Mothron,
                NPCID.Golem,
                NPCID.GolemHead,
                NPCID.GolemHeadFree,
                NPCID.GolemFistRight,
                NPCID.GolemFistLeft,
                NPCID.GolemHeadFree,
                NPCID.DukeFishron,
                NPCID.Sharkron,
                NPCID.Sharkron2,
                NPCID.CultistBoss,
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
                ModContent.NPCType<DesertScourgeHeadSmall>(),
                ModContent.NPCType<DesertScourgeBodySmall>(),
                ModContent.NPCType<DesertScourgeTailSmall>(),
                ModContent.NPCType<GiantClam>(),
                ModContent.NPCType<PerforatorHeadLarge>(),
                ModContent.NPCType<PerforatorHeadMedium>(),
                ModContent.NPCType<PerforatorHeadSmall>(),
                ModContent.NPCType<PerforatorBodyLarge>(),
                ModContent.NPCType<PerforatorBodyMedium>(),
                ModContent.NPCType<PerforatorBodySmall>(),
                ModContent.NPCType<PerforatorTailLarge>(),
                ModContent.NPCType<PerforatorTailMedium>(),
                ModContent.NPCType<PerforatorTailSmall>(),
                ModContent.NPCType<SlimeGod>(),
                ModContent.NPCType<SlimeGodRun>(),
                ModContent.NPCType<SlimeGodSplit>(),
                ModContent.NPCType<SlimeGodRunSplit>(),
                ModContent.NPCType<CalamitasRun>(),
                ModContent.NPCType<CalamitasRun2>() //brothers
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
                ModContent.NPCType<PlaguebringerGoliath>(),
                ModContent.NPCType<PlaguebringerShade>(),
                ModContent.NPCType<PlagueBeeLargeG>(),
                ModContent.NPCType<PlagueBeeLarge>(),
                ModContent.NPCType<PlagueBeeG>(),
                ModContent.NPCType<PlagueBee>()
            };

            beeProjectileList = new List<int>()
            {
                ProjectileID.Stinger,
                ProjectileID.HornetStinger,
                ModContent.ProjectileType<PlagueStingerGoliath>(),
                ModContent.ProjectileType<PlagueStingerGoliathV2>(),
                ModContent.ProjectileType<PlagueExplosion>()
            };

            friendlyBeeList = new List<int>()
            {
                ProjectileID.GiantBee,
                ProjectileID.Bee,
                ProjectileID.Wasp,
                ModContent.ProjectileType<PlaguenadeBee>(),
                ModContent.ProjectileType<PlaguePrincess>(),
                ModContent.ProjectileType<BabyPlaguebringer>(),
                ModContent.ProjectileType<PlagueBeeSmall>()
            };

            hardModeNerfList = new List<int>()
            {
                ProjectileID.WebSpit,
                ProjectileID.PinkLaser,
                ProjectileID.FrostBlastHostile,
                ProjectileID.RuneBlast,
                ProjectileID.GoldenShowerHostile,
                ProjectileID.RainNimbus,
                ProjectileID.Stinger,
                ProjectileID.FlamingArrow,
                ProjectileID.BulletDeadeye,
                ProjectileID.CannonballHostile
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
                ModContent.BuffType<SulphuricPoisoning>(),
                ModContent.BuffType<Shadowflame>(),
                ModContent.BuffType<BrimstoneFlames>(),
                ModContent.BuffType<BurningBlood>(),
                ModContent.BuffType<GlacialState>(),
                ModContent.BuffType<GodSlayerInferno>(),
                ModContent.BuffType<AstralInfectionDebuff>(),
                ModContent.BuffType<HolyFlames>(),
                ModContent.BuffType<Irradiated>(),
                ModContent.BuffType<Plague>(),
                ModContent.BuffType<AbyssalFlames>(),
                ModContent.BuffType<CrushDepth>(),
                ModContent.BuffType<MarkedforDeath>(),
                ModContent.BuffType<WarCleave>(),
                ModContent.BuffType<ArmorCrunch>(),
                ModContent.BuffType<Vaporfied>(),
                ModContent.BuffType<Eutrophication>(),
                ModContent.BuffType<LethalLavaBurn>(),
                ModContent.BuffType<Nightwither>()
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
                ModContent.ItemType<AegisBlade>(),
                ModContent.ItemType<BalefulHarvester>(),
                ModContent.ItemType<Chaotrix>(),
                ModContent.ItemType<CometQuasher>(),
                ModContent.ItemType<DivineRetribution>(),
                ModContent.ItemType<DraconicDestruction>(),
                ModContent.ItemType<Drataliornus>(),
                ModContent.ItemType<EnergyStaff>(),
                ModContent.ItemType<ExsanguinationLance>(),
                ModContent.ItemType<FirestormCannon>(),
                ModContent.ItemType<FlameburstShortsword>(),
                ModContent.ItemType<FlameScythe>(),
                ModContent.ItemType<FlameScytheMelee>(),
                ModContent.ItemType<FlareBolt>(),
                ModContent.ItemType<FlarewingBow>(),
                ModContent.ItemType<ForbiddenSun>(),
                ModContent.ItemType<GreatbowofTurmoil>(),
                ModContent.ItemType<HarvestStaff>(),
                ModContent.ItemType<Hellborn>(),
                ModContent.ItemType<HellBurst>(),
                ModContent.ItemType<HellfireFlamberge>(),
                ModContent.ItemType<Hellkite>(),
                ModContent.ItemType<HellwingStaff>(),
                ModContent.ItemType<Helstorm>(),
                ModContent.ItemType<HellsSun>(),
                ModContent.ItemType<InfernaCutter>(),
                ModContent.ItemType<Lazhar>(),
                ModContent.ItemType<MeteorFist>(),
                ModContent.ItemType<Mourningstar>(),
                ModContent.ItemType<PhoenixBlade>(),
                ModContent.ItemType<RedSun>(),
                ModContent.ItemType<SparkSpreader>(),
                ModContent.ItemType<SpectralstormCannon>(),
                ModContent.ItemType<SunGodStaff>(),
                ModContent.ItemType<SunSpiritStaff>(),
                ModContent.ItemType<TerraFlameburster>(),
                ModContent.ItemType<TheLastMourning>(),
                ModContent.ItemType<TheWand>(),
                ModContent.ItemType<VenusianTrident>(),
                ModContent.ItemType<Vesuvius>(),
                ModContent.ItemType<BlissfulBombardier>(),
                ModContent.ItemType<HolyCollider>(),
                ModContent.ItemType<MoltenAmputator>(),
                ModContent.ItemType<PurgeGuzzler>(),
                ModContent.ItemType<SolarFlare>(),
                ModContent.ItemType<TelluricGlare>(),
                ModContent.ItemType<AngryChickenStaff>(),
                ModContent.ItemType<ChickenCannon>(),
                ModContent.ItemType<DragonRage>(),
                ModContent.ItemType<DragonsBreath>(),
                ModContent.ItemType<PhoenixFlameBarrage>(),
                ModContent.ItemType<ProfanedTrident>(),
                ModContent.ItemType<TheBurningSky>(),
                ModContent.ItemType<TotalityBreakers>(),
                ModContent.ItemType<ProfanedPartisan>(),
                ModContent.ItemType<BlastBarrel>(),
                ModContent.ItemType<LatcherMine>(),
                ModContent.ItemType<BouncingBetty>(),
                ModContent.ItemType<HeliumFlash>(),
                ModContent.ItemType<ShatteredSun>(),
                ModContent.ItemType<DivineHatchet>(),
                ModContent.ItemType<DazzlingStabberStaff>(),
                ModContent.ItemType<PristineFury>(),
                ModContent.ItemType<SarosPossession>(),
                ModContent.ItemType<CinderBlossomStaff>(),
                ModContent.ItemType<FinalDawn>()
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
                ModContent.ItemType<AbsoluteZero>(),
                ModContent.ItemType<Avalanche>(),
                ModContent.ItemType<GlacialCrusher>(),
                ModContent.ItemType<TemporalFloeSword>(),
                ModContent.ItemType<ColdheartIcicle>(),
                ModContent.ItemType<KelvinCatalystMelee>(),
                ModContent.ItemType<CosmicDischarge>(),
                ModContent.ItemType<EffluviumBow>(),
                ModContent.ItemType<EternalBlizzard>(),
                ModContent.ItemType<FrostbiteBlaster>(),
                ModContent.ItemType<IcicleStaff>(),
                ModContent.ItemType<BittercoldStaff>(),
                ModContent.ItemType<CrystalFlareStaff>(),
                ModContent.ItemType<IcicleTrident>(),
                ModContent.ItemType<SnowstormStaff>(),
                ModContent.ItemType<Cryophobia>(),
                ModContent.ItemType<FrostBolt>(),
                ModContent.ItemType<WintersFury>(),
                ModContent.ItemType<ArcticBearPaw>(),
                ModContent.ItemType<AncientIceChunk>(),
                ModContent.ItemType<CryogenicStaff>(),
                ModContent.ItemType<FrostyFlare>(),
                ModContent.ItemType<IceStar>(),
                ModContent.ItemType<Icebreaker>(),
                ModContent.ItemType<KelvinCatalyst>(),
                ModContent.ItemType<FrostcrushValari>(),
                ModContent.ItemType<Endogenesis>(),
                ModContent.ItemType<FlurrystormCannon>(),
                ModContent.ItemType<Hypothermia>(),
                ModContent.ItemType<IceBarrage>(),
                ModContent.ItemType<FrostBlossomStaff>(),
                ModContent.ItemType<EndoHydraStaff>(),
                //Cryonic Bar set stuff, could potentially be removed
                ModContent.ItemType<Trinity>(),
                ModContent.ItemType<Shimmerspark>(),
                ModContent.ItemType<StarnightLance>(),
                ModContent.ItemType<DarkechoGreatbow>(),
                ModContent.ItemType<ShadecrystalTome>(),
                ModContent.ItemType<CrystalPiercer>()
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
                ModContent.ItemType<DepthBlade>(),
                ModContent.ItemType<AbyssBlade>(),
                ModContent.ItemType<NeptunesBounty>(),
                ModContent.ItemType<AquaticDissolution>(),
                ModContent.ItemType<ArchAmaryllis>(),
                ModContent.ItemType<ThornBlossom>(),
                ModContent.ItemType<BiomeBlade>(),
                ModContent.ItemType<TrueBiomeBlade>(),
                ModContent.ItemType<OmegaBiomeBlade>(),
                ModContent.ItemType<BladedgeGreatbow>(),
                ModContent.ItemType<BlossomFlux>(),
                ModContent.ItemType<EvergladeSpray>(),
                ModContent.ItemType<FeralthornClaymore>(),
                ModContent.ItemType<Floodtide>(),
                ModContent.ItemType<FourSeasonsGalaxia>(),
                ModContent.ItemType<GammaFusillade>(),
                ModContent.ItemType<GleamingMagnolia>(),
                ModContent.ItemType<HarvestStaff>(),
                ModContent.ItemType<HellionFlowerSpear>(),
                ModContent.ItemType<Lazhar>(),
                ModContent.ItemType<LifefruitScythe>(),
                ModContent.ItemType<ManaRose>(),
                ModContent.ItemType<MangroveChakram>(),
                ModContent.ItemType<MangroveChakramMelee>(),
                ModContent.ItemType<MantisClaws>(),
                ModContent.ItemType<Mariana>(),
                ModContent.ItemType<Mistlestorm>(),
                ModContent.ItemType<Monsoon>(),
                ModContent.ItemType<Alluvion>(),
                ModContent.ItemType<Needler>(),
                ModContent.ItemType<NettlelineGreatbow>(),
                ModContent.ItemType<Quagmire>(),
                ModContent.ItemType<Shroomer>(),
                ModContent.ItemType<SolsticeClaymore>(),
                ModContent.ItemType<SporeKnife>(),
                ModContent.ItemType<Spyker>(),
                ModContent.ItemType<StormSaber>(),
                ModContent.ItemType<StormRuler>(),
                ModContent.ItemType<StormSurge>(),
                ModContent.ItemType<TarragonThrowingDart>(),
                ModContent.ItemType<TerraEdge>(),
                ModContent.ItemType<TerraLance>(),
                ModContent.ItemType<TerraRay>(),
                ModContent.ItemType<TerraShiv>(),
                ModContent.ItemType<Terratomere>(),
                ModContent.ItemType<TerraFlameburster>(),
                ModContent.ItemType<TheSwarmer>(),
                ModContent.ItemType<Verdant>(),
                ModContent.ItemType<Barinautical>(),
                ModContent.ItemType<DeepseaStaff>(),
                ModContent.ItemType<Downpour>(),
                ModContent.ItemType<SubmarineShocker>(),
                ModContent.ItemType<ScourgeoftheSeas>(),
                ModContent.ItemType<Archerfish>(),
                ModContent.ItemType<BallOFugu>(),
                ModContent.ItemType<BlackAnurian>(),
                ModContent.ItemType<CalamarisLament>(),
                ModContent.ItemType<HerringStaff>(),
                ModContent.ItemType<Lionfish>(),
                ModContent.ItemType<ShellfishStaff>(),
                ModContent.ItemType<ClamCrusher>(),
                ModContent.ItemType<ClamorRifle>(),
                ModContent.ItemType<Serpentine>(),
                ModContent.ItemType<UrchinFlail>(),
                ModContent.ItemType<CoralCannon>(),
                ModContent.ItemType<Shellshooter>(),
                ModContent.ItemType<SandDollar>(),
                ModContent.ItemType<MagicalConch>(),
                ModContent.ItemType<SnapClam>(),
                ModContent.ItemType<GacruxianMollusk>(),
                ModContent.ItemType<PolarisParrotfish>(),
                ModContent.ItemType<SparklingEmpress>(),
                ModContent.ItemType<NastyCholla>(),
                ModContent.ItemType<PoisonPack>(),
                ModContent.ItemType<PlantationStaff>(),
                ModContent.ItemType<SeasSearing>(),
                ModContent.ItemType<YateveoBloom>(),
                ModContent.ItemType<TerraDisk>(),
                ModContent.ItemType<TerraDiskMelee>(),
                ModContent.ItemType<BelladonnaSpiritStaff>(),
                ModContent.ItemType<TenebreusTides>(),
                ModContent.ItemType<Greentide>(),
                ModContent.ItemType<Leviatitan>(),
                ModContent.ItemType<BrackishFlask>(),
                ModContent.ItemType<LeviathanTeeth>(),
                ModContent.ItemType<GastricBelcherStaff>()
            };

            alcoholList = new List<int>()
            {
                ModContent.BuffType<BloodyMaryBuff>(),
                ModContent.BuffType<CaribbeanRumBuff>(),
                ModContent.BuffType<CinnamonRollBuff>(),
                ModContent.BuffType<EverclearBuff>(),
                ModContent.BuffType<EvergreenGinBuff>(),
                ModContent.BuffType<FireballBuff>(),
                ModContent.BuffType<GrapeBeerBuff>(),
                ModContent.BuffType<MargaritaBuff>(),
                ModContent.BuffType<MoonshineBuff>(),
                ModContent.BuffType<MoscowMuleBuff>(),
                ModContent.BuffType<RedWineBuff>(),
                ModContent.BuffType<RumBuff>(),
                ModContent.BuffType<ScrewdriverBuff>(),
                ModContent.BuffType<StarBeamRyeBuff>(),
                ModContent.BuffType<TequilaBuff>(),
                ModContent.BuffType<TequilaSunriseBuff>(),
                ModContent.BuffType<VodkaBuff>(),
                ModContent.BuffType<WhiskeyBuff>(),
                ModContent.BuffType<WhiteWineBuff>()
            };

			doubleDamageBuffList = new List<int>()
			{
				ItemID.BallOHurt,
				ItemID.TheMeatball,
				ItemID.BlueMoon,
				ItemID.Sunfury,
				ItemID.DaoofPow,
				ItemID.FlowerPow,
				ItemID.Anchor,
				ItemID.KOCannon,
				ItemID.GolemFist,
				ItemID.BreakerBlade,
				ItemID.MonkStaffT2,
				ItemID.ProximityMineLauncher,
				ItemID.FireworksLauncher,
				ItemID.ShadowbeamStaff,
				ItemID.Terrarian
            };

            sixtySixDamageBuffList = new List<int>()
            {
                ItemID.TrueNightsEdge,
                ItemID.MedusaHead,
                ItemID.StaffofEarth,
                ItemID.ChristmasTreeSword,
                ItemID.MonkStaffT1,
                ItemID.InfernoFork,
                ItemID.VenomStaff,
                ItemID.Frostbrand
            };

            fiftyDamageBuffList = new List<int>()
            {
                ItemID.NightsEdge,
                ItemID.EldMelter,
                ItemID.Flamethrower,
                ItemID.MoonlordTurretStaff,
                ItemID.WaspGun,
                ItemID.Keybrand,
                ItemID.PulseBow,
                ItemID.PaladinsHammer,
                ItemID.SolarEruption,
                ItemID.DayBreak
            };

            thirtyThreeDamageBuffList = new List<int>()
            {
                ItemID.WandofSparking,
                ItemID.IceBow,
                ItemID.Marrow,
                ItemID.CrystalVileShard,
                ItemID.SoulDrain,
                ItemID.ClingerStaff,
                ItemID.ChargedBlasterCannon,
                ItemID.NettleBurst,
                ItemID.Excalibur,
                ItemID.AmberStaff,
                ItemID.BluePhasesaber,
                ItemID.RedPhasesaber,
                ItemID.GreenPhasesaber,
                ItemID.WhitePhasesaber,
                ItemID.YellowPhasesaber,
                ItemID.PurplePhasesaber,
                ItemID.TheRottedFork,
                ItemID.VampireKnives,
                ItemID.Cascade,
                ItemID.TrueExcalibur
            };

            twentyFiveDamageBuffList = new List<int>()
            {
                ItemID.Muramasa,
                ItemID.StakeLauncher,
                ItemID.BookStaff
            };

            twentyDamageBuffList = new List<int>()
            {
                ItemID.ChainGuillotines,
                ItemID.FlowerofFrost,
                ItemID.PoisonStaff,
                ItemID.Gungnir,
                ItemID.TacticalShotgun
            };

            tenDamageBuffList = new List<int>()
            {
                ItemID.MagnetSphere,
                ItemID.BatScepter
            };

            weaponAutoreuseList = new List<int>()
            {
                ItemID.NightsEdge,
                ItemID.TrueNightsEdge,
                ItemID.TrueExcalibur,
                ItemID.PhoenixBlaster,
                ItemID.VenusMagnum,
                ItemID.MagicDagger,
                ItemID.BeamSword,
                ItemID.MonkStaffT2,
                ItemID.PaladinsHammer,
                ItemID.PearlwoodSword,
                ItemID.PearlwoodBow,
                ItemID.TaxCollectorsStickOfDoom,
                ItemID.StylistKilLaKillScissorsIWish
            };

            tenDamageNerfList = new List<int>()
            {
                ItemID.Phantasm
            };

            quarterDamageNerfList = new List<int>()
            {
                ItemID.Razorpine,
                ItemID.DaedalusStormbow,
                ItemID.PhoenixBlaster,
                ItemID.DD2BetsyBow,
                ItemID.InfluxWaver,
                ItemID.Xenopopper,
                ItemID.ElectrosphereLauncher,
                ItemID.OpticStaff //Note: got local i frames so it should be better
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
                ModContent.NPCType<AquaticSeekerHead>(),
                ModContent.NPCType<Cnidrion>(),
                ModContent.NPCType<PrismTurtle>(),
                ModContent.NPCType<GhostBell>()
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
                ModContent.NPCType<EutrophicRay>(),
                ModContent.NPCType<Clam>(),
                ModContent.NPCType<SeaSerpent1>(),
                ModContent.NPCType<GiantClam>(),
                ModContent.NPCType<FearlessGoldfishWarrior>()
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
                ModContent.NPCType<Atlas>(),
                ModContent.NPCType<ArmoredDiggerHead>(),
                ModContent.NPCType<GreatSandShark>(),
                ModContent.NPCType<Horse>(),
                ModContent.NPCType<ScornEater>(),
                ModContent.NPCType<BlindedAngler>()
            };

            // Enemies that inflict an average of 201 to 400 damage in Expert Mode
            revengeanceEnemyBuffList10Percent = new List<int>()
            {
                NPCID.SolarCrawltipedeHead,
                ModContent.NPCType<BobbitWormHead>(),
                ModContent.NPCType<ColossalSquid>(),
                ModContent.NPCType<EidolonWyrmHead>(),
                ModContent.NPCType<GulperEelHead>(),
                ModContent.NPCType<Mauler>(),
                ModContent.NPCType<Reaper>()
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
                ModContent.ProjectileType<GreatSandBlast>(),
                ModContent.ProjectileType<PearlBurst>(),
                ModContent.ProjectileType<PearlRain>()
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
                NPCID.GolemHead,
                NPCID.GolemHeadFree,
                NPCID.GolemFistRight,
                NPCID.GolemFistLeft,
                NPCID.MoonLordCore
            };

            movementImpairImmuneList = new List<int>()
            {
                NPCID.QueenBee,
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
                ModContent.ItemType<AMR>(),
                ModContent.ItemType<Auralis>(),
                ModContent.ItemType<HalleysInferno>(),
                ModContent.ItemType<Shroomer>(),
                ModContent.ItemType<SpectreRifle>(),
                ModContent.ItemType<Svantechnical>(),
                ModContent.ItemType<Skullmasher>(),
                ModContent.ItemType<TyrannysEnd>()
            };

            boomerangList = new List<int>()
            {
                ModContent.ItemType<Brimblade>(),
                ModContent.ItemType<BlazingStar>(),
                ModContent.ItemType<Celestus>(),
                ModContent.ItemType<AccretionDisk>(),
                ModContent.ItemType<EnchantedAxe>(),
                ModContent.ItemType<EpidemicShredder>(),
                ModContent.ItemType<Equanimity>(),
                ModContent.ItemType<Eradicator>(),
                ModContent.ItemType<TruePaladinsHammer>(),
                ModContent.ItemType<FlameScythe>(),
                ModContent.ItemType<GalaxySmasherRogue>(),
                ModContent.ItemType<Glaive>(),
                ModContent.ItemType<GhoulishGouger>(),
                ModContent.ItemType<Icebreaker>(),
                ModContent.ItemType<KelvinCatalyst>(),
                ModContent.ItemType<Kylie>(),
                ModContent.ItemType<MangroveChakram>(),
                ModContent.ItemType<MoltenAmputator>(),
                ModContent.ItemType<NanoblackReaperRogue>(),
                ModContent.ItemType<Pwnagehammer>(),
                ModContent.ItemType<SandDollar>(),
                ModContent.ItemType<SeashellBoomerang>(),
                ModContent.ItemType<Shroomerang>(),
                ModContent.ItemType<StellarContemptRogue>(),
                ModContent.ItemType<TriactisTruePaladinianMageHammerofMight>(),
                ModContent.ItemType<Valediction>(),
                ModContent.ItemType<FrostcrushValari>(),
                ModContent.ItemType<DefectiveSphere>(),
                ModContent.ItemType<TerraDisk>(),
                ModContent.ItemType<ToxicantTwister>(),
                ModContent.ItemType<TrackingDisk>()
            };

            boomerangProjList = new List<int>()
            {
                ModContent.ProjectileType<AccretionDiskProj>(),
                ModContent.ProjectileType<AccretionDisk2>(),
                ModContent.ProjectileType<BlazingStarProj>(),
                ModContent.ProjectileType<CelestusBoomerang>(),
                ModContent.ProjectileType<BrimbladeProj>(),
                ModContent.ProjectileType<Brimblade2>(),
                ModContent.ProjectileType<EnchantedAxeProj>(),
                ModContent.ProjectileType<EpidemicShredderProjectile>(),
                ModContent.ProjectileType<EquanimityProj>(),
                ModContent.ProjectileType<EradicatorProjectile>(),
                ModContent.ProjectileType<FlameScytheProjectile>(),
                ModContent.ProjectileType<GhoulishGougerBoomerang>(),
                ModContent.ProjectileType<GlaiveProj>(),
                ModContent.ProjectileType<KylieBoomerang>(),
                ModContent.ProjectileType<MangroveChakramProjectile>(),
                ModContent.ProjectileType<MoltenAmputatorProj>(),
                ModContent.ProjectileType<OPHammer>(),
                ModContent.ProjectileType<SandDollarProj>(),
                ModContent.ProjectileType<SandDollarStealth>(),
                ModContent.ProjectileType<SeashellBoomerangProjectile>(),
                ModContent.ProjectileType<ShroomerangProj>(),
                ModContent.ProjectileType<TriactisOPHammer>(),
                ModContent.ProjectileType<ValedictionBoomerang>(),
                ModContent.ProjectileType<GalaxySmasherHammer>(),
                ModContent.ProjectileType<KelvinCatalystBoomerang>(),
                ModContent.ProjectileType<NanoblackMain>(),
                ModContent.ProjectileType<StellarContemptHammer>(),
                ModContent.ProjectileType<IcebreakerHammer>(),
                ModContent.ProjectileType<PwnagehammerProj>(),
                ModContent.ProjectileType<ValariBoomerang>(),
                ModContent.ProjectileType<SphereSpiked>(),
                ModContent.ProjectileType<SphereBladed>(),
                ModContent.ProjectileType<SphereYellow>(),
                ModContent.ProjectileType<ButcherKnife>(),
                ModContent.ProjectileType<TerraDiskProjectile>(),
                ModContent.ProjectileType<TerraDiskProjectile2>(),
                ModContent.ProjectileType<ToxicantTwisterTwoPointZero>(),
                ModContent.ProjectileType<TrackingDiskProjectile>()
            };

            javelinList = new List<int>()
            {
                ModContent.ItemType<CrystalPiercer>(),
                ModContent.ItemType<PalladiumJavelin>(),
                ModContent.ItemType<DuneHopper>(),
                ModContent.ItemType<EclipsesFall>(),
                ModContent.ItemType<IchorSpear>(),
                ModContent.ItemType<ProfanedTrident>(),
                ModContent.ItemType<LuminousStriker>(),
                ModContent.ItemType<ScarletDevil>(),
                ModContent.ItemType<ScourgeoftheCosmosThrown>(),
                ModContent.ItemType<ScourgeoftheDesert>(),
                ModContent.ItemType<ScourgeoftheSeas>(),
                ModContent.ItemType<SpearofDestiny>(),
                ModContent.ItemType<SpearofPaleolith>(),
                ModContent.ItemType<XerocPitchfork>(),
                ModContent.ItemType<PhantasmalRuin>(),
                ModContent.ItemType<PhantomLance>(),
                ModContent.ItemType<ProfanedPartisan>(),
                ModContent.ItemType<Turbulance>(),
                ModContent.ItemType<NightsGaze>(),
                ModContent.ItemType<FrequencyManipulator>()
            };

            javelinProjList = new List<int>()
            {
                ModContent.ProjectileType<CrystalPiercerProjectile>(),
                ModContent.ProjectileType<DuneHopperProjectile>(),
                ModContent.ProjectileType<EclipsesFallMain>(),
                ModContent.ProjectileType<EclipsesStealth>(),
                ModContent.ProjectileType<IchorSpearProj>(),
                ModContent.ProjectileType<InfernalSpearProjectile>(),
                ModContent.ProjectileType<LuminousStrikerProj>(),
                ModContent.ProjectileType<PalladiumJavelinProjectile>(),
                ModContent.ProjectileType<PhantasmalRuinProj>(),
                ModContent.ProjectileType<PhantomLanceProj>(),
                ModContent.ProjectileType<ProfanedPartisanproj>(),
                ModContent.ProjectileType<ScarletDevilProjectile>(),
                ModContent.ProjectileType<ScourgeoftheDesertProj>(),
                ModContent.ProjectileType<ScourgeoftheSeasProjectile>(),
                ModContent.ProjectileType<ScourgeoftheCosmosProj>(),
                ModContent.ProjectileType<SpearofDestinyProjectile>(),
                ModContent.ProjectileType<SpearofPaleolithProj>(),
                ModContent.ProjectileType<AntumbraShardProjectile>(),
                ModContent.ProjectileType<TurbulanceProjectile>(),
                ModContent.ProjectileType<NightsGazeProjectile>(),
                ModContent.ProjectileType<FrequencyManipulatorProjectile>()
            };

            daggerList = new List<int>()
            {
                ModContent.ItemType<AshenStalactite>(),
                ModContent.ItemType<CobaltKunai>(),
                ModContent.ItemType<FeatherKnife>(),
                ModContent.ItemType<GelDart>(),
                ModContent.ItemType<MonkeyDarts>(),
                ModContent.ItemType<MythrilKnife>(),
                ModContent.ItemType<OrichalcumSpikedGemstone>(),
                ModContent.ItemType<TarragonThrowingDart>(),
                ModContent.ItemType<WulfrumKnife>(),
                ModContent.ItemType<Cinquedea>(),
                ModContent.ItemType<CosmicKunai>(),
                ModContent.ItemType<CorpusAvertor>(),
                ModContent.ItemType<Crystalline>(),
                ModContent.ItemType<CursedDagger>(),
                ModContent.ItemType<Malachite>(),
                ModContent.ItemType<Mycoroot>(),
                ModContent.ItemType<Prismalline>(),
                ModContent.ItemType<Quasar>(),
                ModContent.ItemType<RadiantStar>(),
                ModContent.ItemType<ShatteredSun>(),
                ModContent.ItemType<StellarKnife>(),
                ModContent.ItemType<StormfrontRazor>(),
                ModContent.ItemType<TimeBolt>(),
                ModContent.ItemType<LunarKunai>(),
                ModContent.ItemType<GildedDagger>(),
                ModContent.ItemType<GleamingDagger>(),
                ModContent.ItemType<EmpyreanKnives>(),
                ModContent.ItemType<RoyalKnives>(),
                ModContent.ItemType<InfernalKris>(),
                ModContent.ItemType<UtensilPoker>(),
                ModContent.ItemType<ShinobiBlade>(),
                ModContent.ItemType<JawsOfOblivion>(),
                ModContent.ItemType<LeviathanTeeth>(),
                ModContent.ItemType<DeificThunderbolt>()
            };

            daggerProjList = new List<int>()
            {
                ModContent.ProjectileType<AshenStalactiteProj>(),
                ModContent.ProjectileType<AshenStalagmiteProj>(),
                ModContent.ProjectileType<CinquedeaProj>(),
                ModContent.ProjectileType<CobaltKunaiProjectile>(),
                ModContent.ProjectileType<CosmicKunaiProj>(),
                ModContent.ProjectileType<CrystallineProj>(),
                ModContent.ProjectileType<Crystalline2>(),
                ModContent.ProjectileType<CursedDaggerProj>(),
                ModContent.ProjectileType<EmpyreanKnife>(),
                ModContent.ProjectileType<FeatherKnifeProjectile>(),
                ModContent.ProjectileType<GelDartProjectile>(),
                ModContent.ProjectileType<GildedDaggerProj>(),
                ModContent.ProjectileType<GleamingDaggerProj>(),
                ModContent.ProjectileType<IllustriousKnife>(),
                ModContent.ProjectileType<LunarKunaiProj>(),
                ModContent.ProjectileType<MalachiteProj>(),
                ModContent.ProjectileType<MalachiteBolt>(),
                ModContent.ProjectileType<MalachiteStealth>(),
                ModContent.ProjectileType<MonkeyDart>(),
                ModContent.ProjectileType<MycorootProj>(),
                ModContent.ProjectileType<MythrilKnifeProjectile>(),
                ModContent.ProjectileType<OrichalcumSpikedGemstoneProjectile>(),
                ModContent.ProjectileType<PrismallineProj>(),
                ModContent.ProjectileType<Prismalline2>(),
                ModContent.ProjectileType<Prismalline3>(),
                ModContent.ProjectileType<QuasarKnife>(),
                ModContent.ProjectileType<Quasar2>(),
                ModContent.ProjectileType<RadiantStarKnife>(),
                ModContent.ProjectileType<RadiantStar2>(),
                ModContent.ProjectileType<ShatteredSunKnife>(),
                ModContent.ProjectileType<StellarKnifeProj>(),
                ModContent.ProjectileType<StormfrontRazorProjectile>(),
                ModContent.ProjectileType<TarragonThrowingDartProjectile>(),
                ModContent.ProjectileType<TimeBoltKnife>(),
                ModContent.ProjectileType<WulfrumKnifeProj>(),
                ModContent.ProjectileType<Fork>(),
                ModContent.ProjectileType<Knife>(),
                ModContent.ProjectileType<CarvingFork>(),
                ModContent.ProjectileType<InfernalKrisProjectile>(),
                ModContent.ProjectileType<ShinobiBladeProjectile>(),
                ModContent.ProjectileType<JawsProjectile>(),
                ModContent.ProjectileType<LeviathanTooth>(),
                ModContent.ProjectileType<DeificThunderboltProj>()
            };

            flaskBombList = new List<int>()
            {
                ModContent.ItemType<Plaguenade>(),
                ModContent.ItemType<BallisticPoisonBomb>(),
                ModContent.ItemType<BrackishFlask>(),
                ModContent.ItemType<DuststormInABottle>(),
                ModContent.ItemType<SeafoamBomb>(),
                ModContent.ItemType<ConsecratedWater>(),
                ModContent.ItemType<DesecratedWater>(),
                ModContent.ItemType<BouncingBetty>(),
                ModContent.ItemType<TotalityBreakers>(),
                ModContent.ItemType<BlastBarrel>(),
                ModContent.ItemType<Penumbra>(),
                ModContent.ItemType<LatcherMine>(),
                ModContent.ItemType<Supernova>(),
                ModContent.ItemType<ShockGrenade>(),
                ModContent.ItemType<Exorcism>(),
                ModContent.ItemType<MeteorFist>(),
                ModContent.ItemType<StarofDestruction>(),
                ModContent.ItemType<CraniumSmasher>(),
                ModContent.ItemType<ContaminatedBile>(),
                ModContent.ItemType<AcidicRainBarrel>(),
                ModContent.ItemType<SkyfinBombers>(),
                ModContent.ItemType<SpentFuelContainer>(),
                ModContent.ItemType<SealedSingularity>(),
                ModContent.ItemType<PlasmaGrenade>(),
                ModContent.ItemType<WavePounder>()
            };

            flaskBombProjList = new List<int>()
            {
                ModContent.ProjectileType<BallisticPoisonBombProj>(),
                ModContent.ProjectileType<BlastBarrelProjectile>(),
                ModContent.ProjectileType<BouncingBettyProjectile>(),
                ModContent.ProjectileType<BrackishFlaskProj>(),
                ModContent.ProjectileType<DuststormInABottleProj>(),
                ModContent.ProjectileType<PlaguenadeProj>(),
                ModContent.ProjectileType<SeafoamBombProj>(),
                ModContent.ProjectileType<TotalityFlask>(),
                ModContent.ProjectileType<ConsecratedWaterProjectile>(),
                ModContent.ProjectileType<DesecratedWaterProj>(),
                ModContent.ProjectileType<PenumbraBomb>(),
                ModContent.ProjectileType<LatcherMineProjectile>(),
                ModContent.ProjectileType<SupernovaBomb>(),
                ModContent.ProjectileType<ShockGrenadeProjectile>(),
                ModContent.ProjectileType<ExorcismProj>(),
                ModContent.ProjectileType<MeteorFistProj>(),
                ModContent.ProjectileType<CraniumSmasherProj>(),
                ModContent.ProjectileType<CraniumSmasherExplosive>(),
                ModContent.ProjectileType<CraniumSmasherStealth>(),
                ModContent.ProjectileType<DestructionStar>(),
                ModContent.ProjectileType<DestructionBolt>(),
                ModContent.ProjectileType<ContaminatedBileFlask>(),
                ModContent.ProjectileType<GreenDonkeyKongReference>(),
                ModContent.ProjectileType<SkyfinNuke>(),
                ModContent.ProjectileType<SpentFuelContainerProjectile>(),
                ModContent.ProjectileType<SealedSingularityProj>(),
                ModContent.ProjectileType<PlasmaGrenadeProjectile>(),
                ModContent.ProjectileType<WavePounderProjectile>()
            };

            spikyBallList = new List<int>()
            {
                ModContent.ItemType<BouncySpikyBall>(),
                ModContent.ItemType<GodsParanoia>(),
                ModContent.ItemType<NastyCholla>(),
                ModContent.ItemType<HellsSun>(),
                ModContent.ItemType<SkyStabber>(),
                ModContent.ItemType<StickySpikyBall>(),
                ModContent.ItemType<WebBall>(),
                ModContent.ItemType<PoisonPack>(),
                ModContent.ItemType<Nychthemeron>(),
                ModContent.ItemType<MetalMonstrosity>(),
                ModContent.ItemType<BurningStrife>(),
                ModContent.ItemType<SystemBane>()
            };

            spikyBallProjList = new List<int>()
            {
                ModContent.ProjectileType<BouncyBol>(),
                ModContent.ProjectileType<GodsParanoiaProj>(),
                ModContent.ProjectileType<HellsSunProj>(),
                ModContent.ProjectileType<NastyChollaBol>(),
                ModContent.ProjectileType<StickyBol>(),
                ModContent.ProjectileType<SkyStabberProj>(),
                ModContent.ProjectileType<WebBallBol>(),
                ModContent.ProjectileType<PoisonBol>(),
                ModContent.ProjectileType<NychthemeronProjectile>(),
                ModContent.ProjectileType<MetalChunk>(),
                ModContent.ProjectileType<BurningStrifeProj>(),
                ModContent.ProjectileType<SystemBaneProjectile>()
            };

            noGravityList = new List<int>()
            {
                ModContent.ItemType<AuricBar>(),
                ModContent.ItemType<EssenceofChaos>(),
                ModContent.ItemType<EssenceofCinder>(),
                ModContent.ItemType<EssenceofEleum>(),
                ModContent.ItemType<CoreofChaos>(),
                ModContent.ItemType<CoreofCinder>(),
                ModContent.ItemType<CoreofEleum>(),
                ModContent.ItemType<CoreofCalamity>(),
                ModContent.ItemType<CalamitousEssence>(),
                ModContent.ItemType<HellcasterFragment>(),
                ModContent.ItemType<TwistingNether>(),
                ModContent.ItemType<DarkPlasma>(),
                ModContent.ItemType<DarksunFragment>(),
                ModContent.ItemType<UnholyEssence>(),
                ModContent.ItemType<GalacticaSingularity>(),
                ModContent.ItemType<NightmareFuel>(),
                ModContent.ItemType<EndothermicEnergy>(),
                ModContent.ItemType<SoulofCryogen>(),
                ModContent.ItemType<AscendantSpiritEssence>(),

                ModContent.ItemType<KnowledgeAquaticScourge>(),
                ModContent.ItemType<KnowledgeAstralInfection>(),
                ModContent.ItemType<KnowledgeAstrumAureus>(),
                ModContent.ItemType<KnowledgeAstrumDeus>(),
                ModContent.ItemType<KnowledgeBloodMoon>(),
                ModContent.ItemType<KnowledgeBrainofCthulhu>(),
                ModContent.ItemType<KnowledgeBrimstoneCrag>(),
                ModContent.ItemType<KnowledgeBrimstoneElemental>(),
                ModContent.ItemType<KnowledgeBumblebirb>(),
                ModContent.ItemType<KnowledgeCalamitas>(),
                ModContent.ItemType<KnowledgeCalamitasClone>(),
                ModContent.ItemType<KnowledgeCorruption>(),
                ModContent.ItemType<KnowledgeCrabulon>(),
                ModContent.ItemType<KnowledgeCrimson>(),
                ModContent.ItemType<KnowledgeCryogen>(),
                ModContent.ItemType<KnowledgeDesertScourge>(),
                ModContent.ItemType<KnowledgeDestroyer>(),
                ModContent.ItemType<KnowledgeDevourerofGods>(),
                ModContent.ItemType<KnowledgeDukeFishron>(),
                ModContent.ItemType<KnowledgeEaterofWorlds>(),
                ModContent.ItemType<KnowledgeEyeofCthulhu>(),
                ModContent.ItemType<KnowledgeGolem>(),
                ModContent.ItemType<KnowledgeHiveMind>(),
                ModContent.ItemType<KnowledgeKingSlime>(),
                ModContent.ItemType<KnowledgeLeviathanandSiren>(),
                ModContent.ItemType<KnowledgeLunaticCultist>(),
                ModContent.ItemType<KnowledgeMechs>(),
                ModContent.ItemType<KnowledgeMoonLord>(),
                ModContent.ItemType<KnowledgeOcean>(),
                ModContent.ItemType<KnowledgeOldDuke>(),
                ModContent.ItemType<KnowledgePerforators>(),
                ModContent.ItemType<KnowledgePlaguebringerGoliath>(),
                ModContent.ItemType<KnowledgePlantera>(),
                ModContent.ItemType<KnowledgePolterghast>(),
                ModContent.ItemType<KnowledgeProfanedGuardians>(),
                ModContent.ItemType<KnowledgeProvidence>(),
                ModContent.ItemType<KnowledgeQueenBee>(),
                ModContent.ItemType<KnowledgeRavager>(),
                ModContent.ItemType<KnowledgeSentinels>(),
                ModContent.ItemType<KnowledgeSkeletron>(),
                ModContent.ItemType<KnowledgeSkeletronPrime>(),
                ModContent.ItemType<KnowledgeSlimeGod>(),
                ModContent.ItemType<KnowledgeSulphurSea>(),
                ModContent.ItemType<KnowledgeTwins>(),
                ModContent.ItemType<KnowledgeUnderworld>(),
                ModContent.ItemType<KnowledgeWallofFlesh>(),
                ModContent.ItemType<KnowledgeYharon>(),
            };

            lavaFishList = new List<int>()
            {
                ModContent.ItemType<SlurperPole>(),
                ModContent.ItemType<ChaoticSpreadRod>(),
                ModContent.ItemType<TheDevourerofCods>()
            };

            highTestFishList = new List<int>()
            {
                ItemID.GoldenFishingRod,
                ModContent.ItemType<EarlyBloomRod>(),
                ModContent.ItemType<TheDevourerofCods>()
            };

            flamethrowerList = new List<int>()
            {
                ModContent.ItemType<DragoonDrizzlefish>(),
                ModContent.ItemType<BloodBoiler>()
            };

            forceItemList = new List<int>()
            {
                ModContent.ItemType<SubmarineShocker>(),
                ModContent.ItemType<Barinautical>(),
                ModContent.ItemType<Downpour>(),
                ModContent.ItemType<DeepseaStaff>(),
                ModContent.ItemType<ScourgeoftheSeas>(),
                ModContent.ItemType<InsidiousImpaler>(),
                ModContent.ItemType<SepticSkewer>(),
                ModContent.ItemType<FetidEmesis>(),
                ModContent.ItemType<VitriolicViper>(),
                ModContent.ItemType<CadaverousCarrion>(),
                ModContent.ItemType<ToxicantTwister>(),
                ModContent.ItemType<DukeScales>(),
                ModContent.ItemType<Greentide>(),
                ModContent.ItemType<Leviatitan>(),
                ModContent.ItemType<Atlantis>(),
                ModContent.ItemType<SirensSong>(),
                ModContent.ItemType<BrackishFlask>(),
                ModContent.ItemType<LeviathanTeeth>(),
                ModContent.ItemType<GastricBelcherStaff>(),
                ModContent.ItemType<LureofEnthrallment>(),
                ModContent.ItemType<AquaticScourgeBag>(),
                ModContent.ItemType<OldDukeBag>(),
                ModContent.ItemType<LeviathanBag>(),
                ModContent.ItemType<OldDukeMask>(),
                ModContent.ItemType<LeviathanMask>(),
                ModContent.ItemType<AquaticScourgeMask>(),
                ModContent.ItemType<OldDukeTrophy>(),
                ModContent.ItemType<LeviathanTrophy>(),
                ModContent.ItemType<AquaticScourgeTrophy>(),
                ModContent.ItemType<KnowledgeAquaticScourge>(),
                ModContent.ItemType<KnowledgeLeviathanandSiren>(),
                ModContent.ItemType<KnowledgeSulphurSea>(),
                ModContent.ItemType<KnowledgeOcean>(),
                ModContent.ItemType<KnowledgeOldDuke>(),
                ModContent.ItemType<VictoryShard>(),
                ModContent.ItemType<AeroStone>(),
                ModContent.ItemType<DukesDecapitator>(),
                ModContent.ItemType<SulphurousSand>(),
                ModContent.ItemType<MagnumRounds>(),
                ModContent.ItemType<GrenadeRounds>(),
                ModContent.ItemType<ExplosiveShells>(),
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
                ModContent.TileType<LivingGodSlayerFireBlockTile>(),
                ModContent.TileType<LivingHolyFireBlockTile>(),
                ModContent.TileType<LivingBrimstoneFireBlockTile>(),
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
                ModContent.NPCType<EidolonWyrmHead>(),
                ModContent.NPCType<Mauler>(),
                ModContent.NPCType<Reaper>(),
                ModContent.NPCType<ColossalSquid>(),
                ModContent.NPCType<GreatSandShark>(),
                ModContent.NPCType<GiantClam>(),
                ModContent.NPCType<ArmoredDiggerHead>(),
                ModContent.NPCType<ArmoredDiggerBody>(),
                ModContent.NPCType<ArmoredDiggerTail>(),
                ModContent.NPCType<ThiccWaifu>(),
                ModContent.NPCType<Horse>(),
                ModContent.NPCType<PlaguebringerShade>(),
                NPCID.Pumpking,
                NPCID.MourningWood,
                NPCID.IceQueen,
                NPCID.SantaNK1,
                NPCID.Everscream,
                NPCID.DD2Betsy,
                NPCID.Mothron
            };

            bossMinionList = new List<int>()
            {
                ModContent.NPCType<DesertScourgeHeadSmall>(),
                ModContent.NPCType<DesertScourgeBodySmall>(),
                ModContent.NPCType<DesertScourgeTailSmall>(),
                NPCID.SlimeSpiked,
                NPCID.ServantofCthulhu,
                ModContent.NPCType<CrabShroom>(),
                NPCID.EaterofWorldsHead,
                NPCID.EaterofWorldsBody,
                NPCID.EaterofWorldsTail,
                NPCID.Creeper,
                ModContent.NPCType<PerforatorHeadSmall>(),
                ModContent.NPCType<PerforatorBodySmall>(),
                ModContent.NPCType<PerforatorTailSmall>(),
                ModContent.NPCType<PerforatorHeadMedium>(),
                ModContent.NPCType<PerforatorBodyMedium>(),
                ModContent.NPCType<PerforatorTailMedium>(),
                ModContent.NPCType<PerforatorHeadLarge>(),
                ModContent.NPCType<PerforatorBodyLarge>(),
                ModContent.NPCType<PerforatorTailLarge>(),
                ModContent.NPCType<HiveBlob>(),
                ModContent.NPCType<HiveBlob2>(),
                ModContent.NPCType<DankCreeper>(),
                NPCID.SkeletronHand,
                ModContent.NPCType<SlimeGod>(),
                ModContent.NPCType<SlimeGodSplit>(),
                ModContent.NPCType<SlimeGodRun>(),
                ModContent.NPCType<SlimeGodRunSplit>(),
                ModContent.NPCType<SlimeSpawnCorrupt>(),
                ModContent.NPCType<SlimeSpawnCorrupt2>(),
                ModContent.NPCType<SlimeSpawnCrimson>(),
                ModContent.NPCType<SlimeSpawnCrimson2>(),
                NPCID.LeechHead,
                NPCID.LeechBody,
                NPCID.LeechTail,
                NPCID.WallofFleshEye,
                NPCID.TheHungry,
                NPCID.TheHungryII,
                ModContent.NPCType<Cryocore>(),
                ModContent.NPCType<Cryocore2>(),
                ModContent.NPCType<IceMass>(),
                NPCID.PrimeCannon,
                NPCID.PrimeLaser,
                NPCID.PrimeSaw,
                NPCID.PrimeVice,
                ModContent.NPCType<Brimling>(),
                NPCID.TheDestroyer,
                NPCID.TheDestroyerBody,
                NPCID.TheDestroyerTail,
                ModContent.NPCType<AquaticScourgeHead>(),
                ModContent.NPCType<AquaticScourgeBody>(),
                ModContent.NPCType<AquaticScourgeBodyAlt>(),
                ModContent.NPCType<AquaticScourgeTail>(),
                ModContent.NPCType<CalamitasRun>(),
                ModContent.NPCType<CalamitasRun2>(),
                ModContent.NPCType<LifeSeeker>(),
                ModContent.NPCType<SoulSeeker>(),
                NPCID.PlanterasTentacle,
                ModContent.NPCType<AureusSpawn>(),
                NPCID.Spore,
                NPCID.GolemHead,
                NPCID.GolemHeadFree,
                NPCID.GolemFistLeft,
                NPCID.GolemFistRight,
                ModContent.NPCType<PlagueMine>(),
                ModContent.NPCType<PlagueHomingMissile>(),
                ModContent.NPCType<PlagueBeeG>(),
                ModContent.NPCType<PlagueBeeLargeG>(),
                ModContent.NPCType<RavagerClawLeft>(),
                ModContent.NPCType<RavagerClawRight>(),
                ModContent.NPCType<RavagerLegLeft>(),
                ModContent.NPCType<RavagerLegRight>(),
                ModContent.NPCType<RavagerHead>(),
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
                ModContent.NPCType<Bumblefuck2>(),
                ModContent.NPCType<ProvSpawnOffense>(),
                ModContent.NPCType<ProvSpawnDefense>(),
                ModContent.NPCType<ProvSpawnHealer>(),
                ModContent.NPCType<DarkEnergy>(),
                ModContent.NPCType<DarkEnergy2>(),
                ModContent.NPCType<DarkEnergy3>(),
                ModContent.NPCType<CosmicLantern>(),
                ModContent.NPCType<DevourerofGodsHead2>(),
                ModContent.NPCType<DevourerofGodsBody2>(),
                ModContent.NPCType<DevourerofGodsTail2>(),
                ModContent.NPCType<DetonatingFlare>(),
                ModContent.NPCType<DetonatingFlare2>(),
                ModContent.NPCType<SupremeCataclysm>(),
                ModContent.NPCType<SupremeCatastrophe>()
            };

            legOverrideList = new List<int>()
            {
                CalamityMod.Instance.GetEquipSlot("ProviLegs", EquipType.Legs),
                CalamityMod.Instance.GetEquipSlot("SirenLegAlt", EquipType.Legs),
                CalamityMod.Instance.GetEquipSlot("SirenLeg", EquipType.Legs),
                CalamityMod.Instance.GetEquipSlot("PopoLeg", EquipType.Legs)
            };

            // Duke Fishron phase 3 becomes way too easy if you can make him stop being invisible with Yanmei's Knife.
            // This is a list so that other NPCs can be added as necessary.
            // IT DOES NOT make them immune to the debuff, just stops them from being recolored.
            kamiDebuffColorImmuneList = new List<int>()
            {
                NPCID.DukeFishron,
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
            doubleDamageBuffList = null;
            sixtySixDamageBuffList = null;
            fiftyDamageBuffList = null;
            thirtyThreeDamageBuffList = null;
            twentyFiveDamageBuffList = null;
            tenDamageBuffList = null;
            weaponAutoreuseList = null;
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
            movementImpairImmuneList = null;
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

            legOverrideList = null;

            kamiDebuffColorImmuneList = null;
        }
    }
}
