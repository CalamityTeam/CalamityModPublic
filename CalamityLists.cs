using CalamityMod.Buffs.Alcohol;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Ammo.FiniteUse;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.AdultEidolonWyrm;
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
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.GreatSandShark;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Tiles.LivingFire;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod
{
    public class CalamityLists
    {
        public static IList<string> donatorList;
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
        public static List<int> alcoholList;
        public static List<int> spearAutoreuseList;
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
        public static List<int> pierceResistExceptionList;

        public static List<int> AstrumDeusIDs;
        public static List<int> DevourerOfGodsIDs;
        public static List<int> CosmicGuardianIDs;
        public static List<int> AquaticScourgeIDs;
        public static List<int> PerforatorIDs;
        public static List<int> DesertScourgeIDs;
        public static List<int> EaterofWorldsIDs;
        public static List<int> SlimeGodIDs;
        public static List<int> DeathModeSplittingWormIDs;
        public static List<int> DestroyerIDs;
        public static List<int> ThanatosIDs;
        public static List<int> AresIDs;
        public static List<int> SkeletronPrimeIDs;
        public static List<int> StormWeaverIDs;
        public static List<int> BoundNPCIDs;

        public static List<int> GrenadeResistIDs;
        public static List<int> ZeroContactDamageNPCList;
        public static List<int> HardmodeNPCNerfList;

        public static SortedDictionary<int, int> BossRushHPChanges;
        public static SortedDictionary<int, int> BossValues;
        public static SortedDictionary<int, int> bossTypes;

        public static List<int> legOverrideList;

        public static List<int> kamiDebuffColorImmuneList;

        public static List<int> MinionsToNotResurrectList;
        public static List<int> ZeroMinionSlotExceptionList;
        public static List<int> DontCopyOriginalMinionAIList;

        public static Dictionary<int, int> EncryptedSchematicIDRelationship;

        public static List<int> DisabledSummonerNerfItems;
        public static List<int> DisabledSummonerNerfMinions;

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
                "Lilith",
                "Ben Shapiro",
                "Frederik",
                "Faye",
                "Gibb50",
                "Braden",
                "Hannes",
                "profoundmango69",
                "Jack",
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
                "Luis",
                "Alexander",
                "BakaQing",
                "Laura",
                "Xaphlactus",
                "MajinBagel",
                "Bendy",
                "Rando Calrissian",
                "Tails the Fox 92",
                "Bread",
                "Minty Candy",
                "Preston",
                "MovingTarget_086",
                "Shiro",
                "Chip",
                "Taylor",
                "ShotgunAngel",
                "Sandblast",
                "ThomasThePencil",
                "Aero (Aero#4599)",
                "Shirosity", // used to be GlitchOut
                "Daawnz",
                "CrabBar",
                "Yatagarasu",
                "Jarod",
                "Zombieh",
                "MingWhy",
                "Random Weeb",
                "Afzofa",
                "Eragon3942",
                "TheBlackHand",
                "william",
                "Samuel",
                "Christopher",
                "DemoN K!ng",
                "Malik",
                "Ryan Baker-Ortiz",
                "Aleksanders",
                "TheSilverGhost",
                "Lucazii",
                "Shay",
                "Prism",
                "BobIsNotMyRealName",
                "Guwahavel",
                "Azura",
                "Joshua",
                "Doveda",
                "William",
                "Arche",
                "DevilSunrise",
                "Yanmei",
                "Chaos",
                "Ryan",
                "Fish Repairs",
                "Melvin",
                "Vroomy Has -3,000 IQ",
                "The Goliath",
                "DaPyRo",
                "Takeru",
                "Circuit-Jay",
                "Commmander Frostbite",
                "cytokat",
                "Cameron",
                "Orudeon",
                "BumbleDoge",
                "John",
                "Naglfar",
                "Helixas",
                "Vetus",
                "High Charity",
                "Devonte",
                "Cerberus",
                "Brendan",
                "Victor",
                "KAT-G307",
                "Tombarry Expresserino",
                "Drip Veezy",
                "Glaid",
                "Apotheosis",
                "Bladesaber",
                "Devon",
                "Ruthoranium",
                "cocodezi_",
                "Mendzey",
                "GameRDheAsianSandwich",
                "Tobias",
                "Streakist",
                "Eisaya",
                "Xenocrona",
                "RKMoon",
                "Eternal Silence",
                "Jeff",
                "Beta165",
                "DanYami",
                "Xenocrona",
                "Ari",
                "cosmickalamity",
                "xd Ow0",
                "Darren",
                "Florian",
                "dawn thunder",
                "asdf935",
                "GentSkeleton",
                "Fizzlpoprock",
                "Pigeon",
                "Aleksh",
                "Just a random guy",
                "Dee",
                "Æthereal",
                "Broken Faucet",
                "Sarcosuchus",
                "Marissa443",
                "Warlok",
                "JackShizz",
                "NebulaMagePlays",
                "Primpy",
                "Thys",
                "Min",
                "Wodernet",
                "Pedro",
                "Depressed Dad Gaming",
                "Snowy",
                "Stormone",
                "Mobian",
                "Rinja",
                "Check pins",
                "Dakota",
                "Neoplasmatic",
                "False",
                "Whitegiraffe",
                "Drakkece",
                "Levi",
                "Izuna",
                "djsnj20",
                "pyobbo",
                "Alec",
                "The Illustrious Sqouinchuousor",
                "Moist Lad",
                "TwanTheGOAT",
                "3x1t_5tyl3",
                "Will",
                "SpookyNinja",
                "Boomdiada",
                "Culex",
                "Rossadon",
                "Ben",
                "hubert thieblot",
                "NepNep",
                "Nanaki",
                "CrimsonCrips",
                "Lagohz",
                "Timon",
                "F00d Demon",
                "Olkothan",
                "Vmar98",
                "Dasdruid",
                "Cinder",
                "Brutzli",
                "Yhashtur",
                "Zekai",
                "Doug",
                "Uberransy",
                "KurlozClown",
                "Nemesis 041",
                "Asheel",
                "Hayden",
                "Lightedflare",
                "Lady Shira",
                "Devin",
                "Qelrin",
                "Thomas",
                "Ne'er Dowell",
                "Potion Man",
                "martyrdomination",
                "Destructoid",
                "Coolguystorm YT",
                "Wolfmaw",
                "yiumik",
                "Destiny Stallcup",
                "GreenBerry",
                "SolsticeUnlimitd",
                "darkhawke",
                "oracle",
                "YumeiSenshi",
                "Cameron",
                "Toxin",
                "Fweepachino",
                "DESPACITO",
                "Altzeus",
                "Ryan",
                "Spider region",
                "WinterTire",
                "Nycro",
                "Bewearium",
                "William",
                "HellGoat2",
                "116taj",
                "CaineSenpai",
                "Suicide Dreams",
                "Roxas",
                "Obsoleek",
                "Jetpat3",
                "GreenTea",
                "Woah",
                "Ryaegos",
                "Popsickle Yoshi",
                "Arcadia",
                "JensB__",
                "Nuclei",
                "Picasso's Bean",
                "Corn M. Cobb",
                "kgh8090",
                "Luke",
                "Barbara",
                "Alexis",
                "Soko",
                "Albino gonkvader",
                "Monic",
                "Slim",
                "ChaosChaos",
                "Deallly",
                "Jeff",
                "vcf55",
                "Kazurgundu",
                "Jheybyrd",
                "Kipluck",
                "SCONICBOOM",
                "Mr.Matter",
                "Billy",
                "jjth0m3",
                "GTW High Cube",
                "Sabrina",
                "Potato - Stego",
                "Perditio Astrum",
                "MaxingOut",
                "SharZz",
                "Allegro",
                "hoosfire",
                "Lauren",
                "Ultra Succ",
                "Ethan",
                "Pacnysam",
                "dummyAzure",
                "Jaykob",
                "Goblin",
                "NoOneElse",
                "Nicholas",
                "Toasty",
                "oli saer",
                "Blobby6799",
                "Domrinth",
                "zombieseatflesh7",
                "Shiny",
                "Whale",
                "The Infinity",
                "Toasty",
                "MrCreamen",
                "TemperedAether",
                "LucasTwocas",
                "JustLonelyPi",
                "Brian",
                "Ashton",
                "Rolandark",
                "Ally2Cute",
                "Dionysos",
                "Plant Waifu",
                "fire",
                "Charles",
                "Kaden",
                "Dr. Pawsworth",
                "Jackson",
                "Freakish",
                "Ashamper",
                "Kinzoku",
                "Elementari",
                "The Wolf Commando",
                "Jordan",
                "Jessire",
                "Ashton",
                "callisto",
                "velneu",
                "Mathuantie",
                "Robert",
                "Matias",
                "T E R M I N A T O R",
                "apotofkoolaid",
                "Matthew",
                "Terrarian Dragon",
                "Pomelo",
                "Thomas",
                "Iconic Parker Gaming",
                "Jaydon",
                "Aidan",
                "Avery",
                "yayoi",
                "Splotchycrib",
                "GIGA MAN",
                "Eric",
                "Merubel",
                "Smug",
                "Lime-Wars l 1",
                "WillyDilly",
                "xAqult",
                "Himakaze",
                "Face",
                "Carboniferous",
                "James"
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

                ProjectileType<BrokenBiomeBladeHoldout>(),
                ProjectileType<AridGrandeur>(),
                ProjectileType<BitingEmbrace>(),
                ProjectileType<DecaysRetort>(),
                ProjectileType<GrovetendersTouch>(),
                ProjectileType<BiomeBladeHoldout>(),
                ProjectileType<TrueAridGrandeur>(),
                ProjectileType<TrueBitingEmbrace>(),
                ProjectileType<TrueDecaysRetort>(),
                ProjectileType<TrueGrovetendersTouch>(),
                ProjectileType<HeavensMight>(),
                ProjectileType<ExtantAbhorrence>(),
                ProjectileType<TrueBiomeBladeHoldout>(),
                ProjectileType<LamentationsOfTheChained>(),
                ProjectileType<ChainedMeatHook>(),
                ProjectileType<SwordsmithsPride>(),
                ProjectileType<SanguineFury>(),
                ProjectileType<MercurialTides>(),
                ProjectileType<GalaxiaHoldout>(),
                ProjectileType<PhoenixsPride>(),
                ProjectileType<PolarisGaze>(),
                ProjectileType<AndromedasStride>(),
                ProjectileType<AriesWrath>(),
                ProjectileType<ArkoftheAncientsSwungBlade>(),
                ProjectileType<ArkoftheAncientsParryHoldout>(),
                ProjectileType<TrueArkoftheAncientsSwungBlade>(),
                ProjectileType<TrueArkoftheAncientsParryHoldout>(),
                ProjectileType<TrueAncientBlast>(),
                ProjectileType<ArkoftheElementsSwungBlade>(),
                ProjectileType<ArkoftheElementsParryHoldout>(),
                ProjectileType<ArkoftheCosmosSwungBlade>(),
                ProjectileType<ArkoftheCosmosParryHoldout>(),

                ProjectileType<PhangasmBow>(),
                ProjectileType<ContagionBow>(),
                ProjectileType<DaemonsFlameBow>(),
                ProjectileType<DrataliornusBow>(),
                ProjectileType<FlakKrakenGun>(),
                ProjectileType<ButcherGun>(),
                ProjectileType<StarfleetMK2Gun>(),
                ProjectileType<NorfleetCannon>(),
                ProjectileType<FlurrystormCannonShooting>(),
                ProjectileType<ChickenCannonHeld>(),
                ProjectileType<PumplerHoldout>(),
                ProjectileType<ClockworkBowHoldout>(),
                ProjectileType<UltimaBowProjectile>(),
                ProjectileType<CondemnationHoldout>(),
                ProjectileType<SurgeDriverHoldout>(),

                ProjectileType<NanoPurgeHoldout>(),
                ProjectileType<AetherfluxCannonHoldout>(),
                ProjectileType<YharimsCrystalPrism>(),
                ProjectileType<DarkSparkPrism>(),
                ProjectileType<YharimsCrystalBeam>(),
                ProjectileType<DarkSparkBeam>(),
                ProjectileType<GhastlyVisageProj>(),
                ProjectileType<ApotheosisWorm>(),
                ProjectileType<SpiritCongregation>(),
                ProjectileType<RancorLaserbeam>(),

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

                // Some hostile boss projectiles
                ProjectileID.SaucerDeathray,
                ProjectileID.PhantasmalDeathray,

                ProjectileType<BrimstoneMonster>(),
                ProjectileType<InfernadoRevenge>(),
                ProjectileType<OverlyDramaticDukeSummoner>(),
                ProjectileType<ProvidenceHolyRay>(),
                ProjectileType<OldDukeVortex>(),
                ProjectileType<BrimstoneRay>(),
                ProjectileType<AresDeathBeamStart>(),
                ProjectileType<AresGaussNukeProjectileBoom>(),
                ProjectileType<AresLaserBeamStart>(),
                ProjectileType<ArtemisSpinLaserbeam>(),
                ProjectileType<BirbAura>(),
                ProjectileType<ThanatosBeamStart>()
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
                NPCID.Deerclops,
                NPCID.WallofFlesh,
                NPCID.WallofFleshEye,
                NPCID.PirateShipCannon,
                NPCID.QueenSlimeBoss,
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
                NPCID.HallowBoss,
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
                NPCType<DesertNuisanceHead>(),
                NPCType<DesertNuisanceBody>(),
                NPCType<DesertNuisanceTail>(),
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
                NPCType<SupremeCataclysm>(),
                NPCType<SupremeCatastrophe>(),
                NPCType<SoulSeekerSupreme>()
            };

            confusionEnemyList = new List<int>()
            {
                NPCType<AeroSlime>(),
                NPCType<Rimehound>(),
                NPCType<AstralachneaGround>(),
                NPCType<AstralachneaWall>(),
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
                NPCType<Rotdog>(),
                NPCType<Scryllar>(),
                NPCType<ScryllarRage>(),
                NPCType<SeaUrchin>(),
                NPCType<StellarCulex>(),
                NPCType<StormlionCharger>(),
                NPCType<SuperDummyNPC>(),
                NPCType<WulfrumGyrator>(),
                NPCType<WulfrumRover>()
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
                NPCType<PlagueBeeLarge>(),
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
                ProjectileID.Stinger,
                ProjectileID.BloodShot,
                ProjectileID.BloodNautilusTears,
                ProjectileID.BloodNautilusShot,
                ProjectileID.RockGolemRock
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

            alcoholList = new List<int>()
            {
                BuffID.Tipsy,
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
                NPCType<Cnidrion>(),
                NPCType<PrismBack>(),
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
                NPCID.QueenSlimeMinionBlue,
                NPCID.QueenSlimeMinionPink,
                NPCID.QueenSlimeMinionPurple,
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
                NPCType<DevourerofGodsBody>(),
                NPCType<DevourerofGodsTail>(),
                NPCType<ThanatosBody1>(),
                NPCType<ThanatosBody2>(),
                NPCType<ThanatosTail>(),
                NPCType<AresLaserCannon>(),
                NPCType<AresTeslaCannon>(),
                NPCType<AresPlasmaFlamethrower>(),
                NPCType<AresGaussNuke>()
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
                ItemType<AntiMaterielRifle>(),
                ItemType<Auralis>(),
                ItemType<HalleysInferno>(),
                ItemType<Shroomer>(),
                ItemType<SpectreRifle>(),
                ItemType<Svantechnical>(),
                ItemType<TyrannysEnd>()
            };

            boomerangList = new List<int>()
            {
                ItemType<Brimblade>(),
                ItemType<BlazingStar>(),
                ItemType<Celestus>(),
                ItemType<ElementalDisk>(),
                ItemType<EnchantedAxe>(),
                ItemType<EpidemicShredder>(),
                ItemType<Equanimity>(),
                ItemType<Eradicator>(),
                ItemType<SubductionSlicer>(),
                ItemType<Glaive>(),
                ItemType<GhoulishGouger>(),
                ItemType<Icebreaker>(),
                ItemType<KelvinCatalyst>(),
                ItemType<Kylie>(),
                ItemType<MangroveChakram>(),
                ItemType<MoltenAmputator>(),
                ItemType<NanoblackReaper>(),
                ItemType<SandDollar>(),
                ItemType<FishboneBoomerang>(),
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
                ProjectileType<FishboneBoomerangProjectile>(),
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
                ItemType<WaveSkipper>(),
                ItemType<EclipsesFall>(),
                ItemType<IchorSpear>(),
                ItemType<ProfanedTrident>(),
                ItemType<LuminousStriker>(),
                ItemType<ScarletDevil>(),
                ItemType<ScourgeoftheDesert>(),
                ItemType<ScourgeoftheSeas>(),
                ItemType<SpearofDestiny>(),
                ItemType<SpearofPaleolith>(),
                ItemType<ShardofAntumbra>(),
                ItemType<PhantasmalRuin>(),
                ItemType<PhantomLance>(),
                ItemType<ProfanedPartisan>(),
                ItemType<Turbulance>(),
                ItemType<NightsGaze>(),
                ItemType<FrequencyManipulator>(),
                ItemType<TheAtomSplitter>()
            };

            javelinProjList = new List<int>()
            {
                ProjectileType<CrystalPiercerProjectile>(),
                ProjectileType<DuneHopperProjectile>(),
                ProjectileType<EclipsesFallMain>(),
                ProjectileType<EclipsesStealth>(),
                ProjectileType<IchorSpearProj>(),
                ProjectileType<WrathwingSpear>(),
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
                ProjectileType<FrequencyManipulatorProjectile>(),
                ProjectileType<TheAtomSplitterProjectile>()
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
                ItemType<DeificThunderbolt>(),
                ItemType<Sacrifice>(),
                ItemType<Seraphim>()
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
                ProjectileType<DeificThunderboltProj>(),
                ProjectileType<SacrificeProjectile>(),
                ProjectileType<SeraphimProjectile>()
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
                ItemType<EssenceofSunlight>(),
                ItemType<EssenceofEleum>(),
                ItemType<CoreofChaos>(),
                ItemType<CoreofSunlight>(),
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
                ItemType<KnowledgeDragonfolly>(),
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
                ItemType<KnowledgeExoMechs>(),
                ItemType<KnowledgeEyeofCthulhu>(),
                ItemType<KnowledgeGolem>(),
                ItemType<KnowledgeHiveMind>(),
                ItemType<KnowledgeKingSlime>(),
                ItemType<KnowledgeLeviathanAnahita>(),
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
                ItemType<RiftReeler>(),
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
                ItemType<OldDukeScales>(),
                ItemType<Greentide>(),
                ItemType<Leviatitan>(),
                ItemType<Atlantis>(),
                ItemType<AnahitasArpeggio>(),
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
                ItemType<KnowledgeLeviathanAnahita>(),
                ItemType<KnowledgeSulphurSea>(),
                ItemType<KnowledgeOcean>(),
                ItemType<KnowledgeOldDuke>(),
                ItemType<VictoryShard>(),
                ItemType<AeroStone>(),
                ItemType<TheCommunity>(),
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
                NPCID.SmallZombie,
                NPCID.MaggotZombie
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
                NPCID.GreekSkeleton,
                NPCID.SporeSkeleton
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
                NPCID.ServantofCthulhu,
                NPCID.TheHungryII,
                NPCID.LeechHead,
                NPCID.LeechBody,
                NPCID.LeechTail,
                NPCID.QueenSlimeMinionBlue,
                NPCID.QueenSlimeMinionPink,
                NPCID.QueenSlimeMinionPurple,
                NPCID.Probe,
                NPCID.Bee,
                NPCID.BeeSmall,
                NPCID.PlanterasTentacle,
                NPCID.Sharkron,
                NPCID.Sharkron2,
                NPCType<DarkEnergy>()
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
                NPCType<RavagerHead>(),
                NPCType<RavagerClawLeft>(),
                NPCType<RavagerClawRight>(),
                NPCType<RavagerLegLeft>(),
                NPCType<RavagerLegRight>(),
                NPCType<AstrumDeusHeadSpectral>(),
                NPCType<AstrumDeusBodySpectral>(),
                NPCType<AstrumDeusTailSpectral>(),
                NPCType<DarkEnergy>(),
                NPCType<StormWeaverHead>(),
                NPCType<StormWeaverBody>(),
                NPCType<StormWeaverTail>(),
                NPCType<DevourerofGodsHead2>(),
                NPCType<DevourerofGodsBody2>(),
                NPCType<DevourerofGodsTail2>(),
                NPCType<ThanatosHead>(),
                NPCType<ThanatosBody1>(),
                NPCType<ThanatosBody2>(),
                NPCType<ThanatosTail>(),
                NPCType<BrimstoneHeart>(),
                NPCType<AresBody>(),
                NPCType<AresLaserCannon>(),
                NPCType<AresTeslaCannon>(),
                NPCType<AresPlasmaFlamethrower>(),
                NPCType<AresGaussNuke>()
            };

            pierceResistExceptionList = new List<int>()
            {
                ProjectileID.FlyingKnife,
                ProjectileID.Arkhalis,
                ProjectileID.Terragrim,
                ProjectileID.MonkStaffT3,
                ProjectileID.LastPrismLaser,
                ProjectileID.ChargedBlasterLaser,
                ProjectileType<FlakKrakenProj>(),
                ProjectileType<MurasamaSlash>(),
                ProjectileType<OmnibladeSwing>(),
                ProjectileType<DragonRageStaff>(),
                ProjectileType<YateveoBloomProj>(),
                ProjectileType<UrchinBall>(),
                ProjectileType<TyphonsGreedStaff>(),
                ProjectileType<DevilsSunriseProj>(),
                ProjectileType<DevilsSunriseCyclone>(),
                ProjectileType<PhaseslayerProjectile>(),
                ProjectileType<TaserHook>(),
                ProjectileType<Snowflake>(),
                ProjectileType<InsidiousHarpoon>(),
                ProjectileType<PhotonRipperProjectile>(),
                ProjectileType<AcidicSaxBubble>(),
                ProjectileType<WaterLeechProj>(),
                ProjectileType<BonebreakerProjectile>(),
                ProjectileType<UrchinBallSpike>(),
                ProjectileType<EmesisGore>(),
                ProjectileType<ExoFlareCluster>(),
                ProjectileType<SulphuricBlast>(),
                ProjectileType<EclipsesStealth>(),
                ProjectileType<EradicatorProjectile>(),
                ProjectileType<FantasyTalismanProj>(),
                ProjectileType<FantasyTalismanStealth>(),
                ProjectileType<GodsParanoiaProj>(),
                ProjectileType<JawsProjectile>(),
                ProjectileType<LeviathanTooth>(),
                ProjectileType<LionfishProj>(),
                ProjectileType<MetalShard>(),
                ProjectileType<NastyChollaBol>(),
                ProjectileType<SacrificeProjectile>(),
                ProjectileType<SnapClamProj>(),
                ProjectileType<SnapClamStealth>(),
                ProjectileType<StickyBol>(),
                ProjectileType<UrchinStingerProj>(),
                ProjectileType<EyeOfNightCell>(),
                ProjectileType<ViolenceThrownProjectile>(),
                ProjectileType<BlushieStaffProj>(),
                ProjectileType<DarkSparkBeam>(),
                ProjectileType<EidolicWailSoundwave>(),
                ProjectileType<HadopelagicEchoSoundwave>(),
                ProjectileType<RancorLaserbeam>(),
                ProjectileType<SparklingBeam>(),
                ProjectileType<YharimsCrystalBeam>(),
                ProjectileType<PrismaticBeam>(),
            };

            // Lists of enemies that resist piercing to some extent (mostly worms).
            // Could prove useful for other things as well.

            AstrumDeusIDs = new List<int>
            {
                NPCType<AstrumDeusHeadSpectral>(),
                NPCType<AstrumDeusBodySpectral>(),
                NPCType<AstrumDeusTailSpectral>()
            };

            DevourerOfGodsIDs = new List<int>
            {
                NPCType<DevourerofGodsHead>(),
                NPCType<DevourerofGodsBody>(),
                NPCType<DevourerofGodsTail>()
            };

            CosmicGuardianIDs = new List<int>
            {
                NPCType<DevourerofGodsHead2>(),
                NPCType<DevourerofGodsBody2>(),
                NPCType<DevourerofGodsTail2>()
            };

            AquaticScourgeIDs = new List<int>
            {
                NPCType<AquaticScourgeHead>(),
                NPCType<AquaticScourgeBody>(),
                NPCType<AquaticScourgeBodyAlt>(),
                NPCType<AquaticScourgeTail>()
            };

            PerforatorIDs = new List<int>
            {
                NPCType<PerforatorHeadLarge>(),
                NPCType<PerforatorBodyLarge>(),
                NPCType<PerforatorTailLarge>(),
                NPCType<PerforatorHeadMedium>(),
                NPCType<PerforatorBodyMedium>(),
                NPCType<PerforatorTailMedium>(),
                NPCType<PerforatorHeadSmall>(),
                NPCType<PerforatorBodySmall>(),
                NPCType<PerforatorTailSmall>()
            };

            DesertScourgeIDs = new List<int>
            {
                NPCType<DesertScourgeHead>(),
                NPCType<DesertScourgeBody>(),
                NPCType<DesertScourgeTail>()
            };

            EaterofWorldsIDs = new List<int>
            {
                NPCID.EaterofWorldsHead,
                NPCID.EaterofWorldsBody,
                NPCID.EaterofWorldsTail
            };

            SlimeGodIDs = new List<int>
            {
                NPCType<SlimeGod>(),
                NPCType<SlimeGodRun>(),
                NPCType<SlimeGodSplit>(),
                NPCType<SlimeGodRunSplit>(),
                NPCType<SlimeGodCore>()
            };

            DeathModeSplittingWormIDs = new List<int>
            {
                NPCID.DuneSplicerHead,
                NPCID.DuneSplicerBody,
                NPCID.DuneSplicerTail,
                NPCID.DiggerHead,
                NPCID.DiggerBody,
                NPCID.DiggerTail,
                NPCID.SeekerHead,
                NPCID.SeekerBody,
                NPCID.SeekerTail
            };

            DestroyerIDs = new List<int>
            {
                NPCID.TheDestroyer,
                NPCID.TheDestroyerBody,
                NPCID.TheDestroyerTail
            };

            ThanatosIDs = new List<int>
            {
                NPCType<ThanatosHead>(),
                NPCType<ThanatosBody1>(),
                NPCType<ThanatosBody2>(),
                NPCType<ThanatosTail>()
            };

            AresIDs = new List<int>
            {
                NPCType<AresBody>(),
                NPCType<AresGaussNuke>(),
                NPCType<AresLaserCannon>(),
                NPCType<AresPlasmaFlamethrower>(),
                NPCType<AresTeslaCannon>()
            };

            SkeletronPrimeIDs = new List<int>
            {
                NPCID.SkeletronPrime,
                NPCID.PrimeCannon,
                NPCID.PrimeLaser,
                NPCID.PrimeSaw,
                NPCID.PrimeVice
            };

            StormWeaverIDs = new List<int>
            {
                NPCType<StormWeaverHead>(),
                NPCType<StormWeaverBody>(),
                NPCType<StormWeaverTail>()
            };

            GrenadeResistIDs = new List<int>
            {
                ProjectileID.Grenade,
                ProjectileID.StickyGrenade,
                ProjectileID.BouncyGrenade,
                ProjectileID.Bomb,
                ProjectileID.StickyBomb,
                ProjectileID.BouncyBomb,
                ProjectileID.Dynamite,
                ProjectileID.StickyDynamite,
                ProjectileID.BouncyDynamite,
                ProjectileID.Explosives,
                ProjectileID.ExplosiveBunny,
                ProjectileID.PartyGirlGrenade,
                ProjectileID.BombFish,
                ProjectileID.Beenade,
                ProjectileID.Bee,
                ProjectileID.GiantBee,
                ProjectileType<AeroExplosive>(),
                ProjectileID.ScarabBomb
            };

            ZeroContactDamageNPCList = new List<int>
            {
                NPCID.DarkCaster,
                NPCID.FireImp,
                NPCID.Tim,
                NPCID.CultistArcherBlue,
                NPCID.DesertDjinn,
                NPCID.DiabolistRed,
                NPCID.DiabolistWhite,
                NPCID.Gastropod,
                NPCID.IceElemental,
                NPCID.IchorSticker,
                NPCID.Necromancer,
                NPCID.NecromancerArmored,
                NPCID.RaggedCaster,
                NPCID.RaggedCasterOpenCoat,
                NPCID.RuneWizard,
                NPCID.SkeletonArcher,
                NPCID.SkeletonCommando,
                NPCID.SkeletonSniper,
                NPCID.TacticalSkeleton,
                NPCID.Clown,
                NPCID.GoblinArcher,
                NPCID.GoblinSorcerer,
                NPCID.GoblinSummoner,
                NPCID.PirateCrossbower,
                NPCID.PirateDeadeye,
                NPCID.PirateCaptain,
                NPCID.SnowmanGangsta,
                NPCID.SnowBalla,
                NPCID.DrManFly,
                NPCID.Eyezor,
                NPCID.Nailhead,
                NPCID.MartianWalker,
                NPCID.MartianTurret,
                NPCID.ElfCopter,
                NPCID.ElfArcher,
                NPCID.NebulaBrain,
                NPCID.StardustJellyfishBig,
                NPCID.PirateShipCannon,
                NPCID.MartianSaucer,
                NPCID.MartianSaucerCannon,
                NPCID.MartianSaucerCore,
                NPCID.MartianSaucerTurret,
                NPCID.Probe,
                NPCID.CultistBoss,
                NPCID.GolemHeadFree,
                NPCID.MoonLordFreeEye,
                NPCID.BloodSquid,
                NPCID.PlanterasHook,
                NPCID.Dandelion
            };

            // Reduce contact damage by 25%
            HardmodeNPCNerfList = new List<int>
            {
                NPCID.AnglerFish,
                NPCID.AngryTrapper,
                NPCID.Arapaima,
                NPCID.BlackRecluse,
                NPCID.BlackRecluseWall,
                NPCID.BloodJelly,
                NPCID.FungoFish,
                NPCID.GreenJellyfish,
                NPCID.Clinger,
                NPCID.ArmoredSkeleton,
                NPCID.ArmoredViking,
                NPCID.Mummy,
                NPCID.DarkMummy,
                NPCID.LightMummy,
                NPCID.BloodFeeder,
                NPCID.DesertBeast,
                NPCID.ChaosElemental,
                NPCID.BloodMummy,
                NPCID.CorruptSlime,
                NPCID.Slimeling,
                NPCID.Corruptor,
                NPCID.Crimslime,
                NPCID.BigCrimslime,
                NPCID.LittleCrimslime,
                NPCID.CrimsonAxe,
                NPCID.CursedHammer,
                NPCID.Derpling,
                NPCID.Herpling,
                NPCID.DiggerHead,
                NPCID.DiggerBody,
                NPCID.DiggerTail,
                NPCID.DesertGhoul,
                NPCID.DesertGhoulCorruption,
                NPCID.DesertGhoulCrimson,
                NPCID.DesertGhoulHallow,
                NPCID.DuneSplicerHead,
                NPCID.DuneSplicerBody,
                NPCID.DuneSplicerTail,
                NPCID.EnchantedSword,
                NPCID.FloatyGross,
                NPCID.GiantBat,
                NPCID.GiantFlyingFox,
                NPCID.GiantFungiBulb,
                NPCID.FungiSpore,
                NPCID.GiantTortoise,
                NPCID.IceTortoise,
                NPCID.HoppinJack,
                NPCID.Mimic,
                NPCID.IchorSticker,
                NPCID.IcyMerman,
                NPCID.IlluminantBat,
                NPCID.IlluminantSlime,
                NPCID.JungleCreeper,
                NPCID.JungleCreeperWall,
                NPCID.DesertLamiaDark,
                NPCID.DesertLamiaLight,
                NPCID.BigMossHornet,
                NPCID.GiantMossHornet,
                NPCID.LittleMossHornet,
                NPCID.MossHornet,
                NPCID.TinyMossHornet,
                NPCID.Moth,
                NPCID.PigronCorruption,
                NPCID.PigronCrimson,
                NPCID.PigronHallow,
                NPCID.Pixie,
                NPCID.PossessedArmor,
                NPCID.RockGolem,
                NPCID.DesertScorpionWalk,
                NPCID.DesertScorpionWall,
                NPCID.Slimer,
                NPCID.Slimer2,
                NPCID.ToxicSludge,
                NPCID.Unicorn,
                NPCID.WanderingEye,
                NPCID.Werewolf,
                NPCID.Wolf,
                NPCID.SeekerHead,
                NPCID.SeekerBody,
                NPCID.SeekerTail,
                NPCID.Wraith,
                NPCID.ChatteringTeethBomb,
                NPCID.Clown,
                NPCID.AngryNimbus,
                NPCID.IceGolem,
                NPCID.RainbowSlime,
                NPCID.SandShark,
                NPCID.SandsharkCorrupt,
                NPCID.SandsharkCrimson,
                NPCID.SandsharkHallow,
                NPCID.ShadowFlameApparition,
                NPCID.Parrot,
                NPCID.PirateCorsair,
                NPCID.PirateDeckhand,
                NPCID.PirateGhost,
                NPCID.BlueArmoredBonesMace,
                NPCID.BlueArmoredBonesSword,
                NPCID.BoneLee,
                NPCID.DungeonSpirit,
                NPCID.FlyingSnake,
                NPCID.HellArmoredBones,
                NPCID.HellArmoredBonesSpikeShield,
                NPCID.HellArmoredBonesSword,
                NPCID.MisterStabby,
                NPCID.SnowBalla,
                NPCID.SnowmanGangsta,
                NPCID.Butcher,
                NPCID.CreatureFromTheDeep,
                NPCID.DeadlySphere,
                NPCID.Frankenstein,
                NPCID.Fritz,
                NPCID.Psycho,
                NPCID.Reaper,
                NPCID.SwampThing,
                NPCID.ThePossessed,
                NPCID.Vampire,
                NPCID.VampireBat,
                NPCID.HeadlessHorseman,
                NPCID.Hellhound,
                NPCID.Poltergeist,
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
                NPCID.Splinterling,
                NPCID.Flocko,
                NPCID.GingerbreadMan,
                NPCID.Krampus,
                NPCID.Nutcracker,
                NPCID.NutcrackerSpinning,
                NPCID.PresentMimic,
                NPCID.Yeti,
                NPCID.ZombieElf,
                NPCID.ZombieElfBeard,
                NPCID.ZombieElfGirl,
                NPCID.BloodEelHead,
                NPCID.BloodEelBody,
                NPCID.BloodEelTail,
                NPCID.GoblinShark,
                NPCID.EyeballFlyingFish,
                NPCID.ZombieMerman
            };

            BoundNPCIDs = new List<int>
            {
                NPCID.BoundGoblin,
                NPCID.BoundWizard,
                NPCID.BoundMechanic,
                NPCID.SleepingAngler,
                NPCID.BartenderUnconscious,
                NPCID.WebbedStylist,
                NPCID.GolferRescue
            };

            // Collections
            // NOTE - Be sure to reference the NeedsFourLifeBytes list in the IL Editing code if changes are made here.
            BossRushHPChanges = new SortedDictionary<int, int>
            {
                // Tier 1
                { NPCID.QueenBee, 315000 }, // 30 seconds

                { NPCID.BrainofCthulhu, 100000 }, // 30 seconds with creepers
                { NPCID.Creeper, 10000 },

                { NPCID.KingSlime, 300000 }, // 30 seconds
                { NPCID.BlueSlime, 3600 },
                { NPCID.SlimeSpiked, 7200 },
                { NPCID.GreenSlime, 2700 },
                { NPCID.RedSlime, 5400 },
                { NPCID.PurpleSlime, 7200 },
                { NPCID.YellowSlime, 6300 },
                { NPCID.IceSlime, 4500 },
                { NPCID.UmbrellaSlime, 5400 },
                { NPCID.RainbowSlime, 30000 },
                { NPCID.Pinky, 15000 },

                { NPCID.EyeofCthulhu, 450000 }, // 30 seconds
                { NPCID.ServantofCthulhu, 6000 },

                { NPCID.SkeletronPrime, 110000 }, // 30 seconds
                { NPCID.PrimeVice, 54000 },
                { NPCID.PrimeCannon, 45000 },
                { NPCID.PrimeSaw, 45000 },
                { NPCID.PrimeLaser, 38000 },

                { NPCID.Golem, 50000 }, // 30 seconds
                { NPCID.GolemHead, 30000 },
                { NPCID.GolemHeadFree, 30000 },
                { NPCID.GolemFistLeft, 25000 },
                { NPCID.GolemFistRight, 25000 },

                { NPCID.EaterofWorldsHead, 10000 }, // 30 seconds + immunity timer at start
                { NPCID.EaterofWorldsBody, 10000 },
                { NPCID.EaterofWorldsTail, 10000 },

                // Tier 2
                { NPCID.TheDestroyer, 250000 }, // 30 seconds + immunity timer at start
                { NPCID.TheDestroyerBody, 250000 },
                { NPCID.TheDestroyerTail, 250000 },
                { NPCID.Probe, 10000 },

                { NPCID.Spazmatism, 150000 }, // 30 seconds
                { NPCID.Retinazer, 125000 },

                { NPCID.WallofFlesh, 450000 }, // 30 seconds
                { NPCID.WallofFleshEye, 450000 },

                { NPCID.SkeletronHead, 160000 }, // 30 seconds
                { NPCID.SkeletronHand, 60000 },

                // Tier 3
                { NPCID.CultistBoss, 220000 }, // 30 seconds
                { NPCID.CultistDragonHead, 60000 },
                { NPCID.CultistDragonBody1, 60000 },
                { NPCID.CultistDragonBody2, 60000 },
                { NPCID.CultistDragonBody3, 60000 },
                { NPCID.CultistDragonBody4, 60000 },
                { NPCID.CultistDragonTail, 60000 },
                { NPCID.AncientCultistSquidhead, 50000 },

                { NPCID.Plantera, 160000 }, // 30 seconds
                { NPCID.PlanterasTentacle, 40000 },

                // Tier 4
                { NPCID.DukeFishron, 290000 }, // 30 seconds

                { NPCID.MoonLordCore, 160000 }, // 1 minute
                { NPCID.MoonLordHand, 45000 },
                { NPCID.MoonLordHead, 60000 },
                { NPCID.MoonLordLeechBlob, 800 }

                // 8 minutes in total for vanilla Boss Rush bosses
            };

            BossValues = new SortedDictionary<int, int>
            {
                { NPCID.KingSlime, Item.buyPrice(0, 5)},
                { NPCID.EyeofCthulhu, Item.buyPrice(0, 10)},
                { NPCID.QueenBee, Item.buyPrice(0, 15)},
                { NPCID.SkeletronHead, Item.buyPrice(0, 20) },
                { NPCID.Deerclops, Item.buyPrice(0, 20) },
                { NPCID.WallofFlesh, Item.buyPrice(0, 25)},
                { NPCID.QueenSlimeBoss, Item.buyPrice(0, 30)},
                { NPCID.Spazmatism, Item.buyPrice(0, 40)},
                { NPCID.Retinazer, Item.buyPrice(0, 40)},
                { NPCID.TheDestroyer, Item.buyPrice(0, 40)},
                { NPCID.SkeletronPrime, Item.buyPrice(0, 40)},
                { NPCID.Plantera, Item.buyPrice(0, 50)},
                { NPCID.HallowBoss, Item.buyPrice(0, 60)},
                { NPCID.Golem, Item.buyPrice(0, 60)},
                { NPCID.DukeFishron, Item.buyPrice(0, 75) },
                { NPCID.CultistBoss, Item.buyPrice(1) },
                { NPCID.MoonLordCore, Item.buyPrice(1, 50) }
            };

            bossTypes = new SortedDictionary<int, int>()
            {
                { NPCID.KingSlime, 1 },
                { NPCType<DesertScourgeHead>(), 2 },
                { NPCID.EyeofCthulhu, 3 },
                { NPCType<CrabulonIdle>(), 4 },
                { NPCID.EaterofWorldsHead, 5 },
                { NPCID.EaterofWorldsBody, 5 },
                { NPCID.EaterofWorldsTail, 5 },
                { NPCID.BrainofCthulhu, 6 },
                { NPCType<HiveMind>(), 7 },
                { NPCType<PerforatorHive>(), 8 },
                { NPCID.QueenBee, 9 },
                { NPCID.SkeletronHead, 10 },
                { NPCType<SlimeGodCore>(), 11 },
                { NPCType<SlimeGodSplit>(), 11 },
                { NPCType<SlimeGodRunSplit>(), 11 },
                { NPCID.WallofFlesh, 12 },
                { NPCType<Cryogen>(), 13 },
                { NPCID.Retinazer, 14 },
                { NPCID.Spazmatism, 14 },
                { NPCType<AquaticScourgeHead>(), 15 },
                { NPCID.TheDestroyer, 16 },
                { NPCType<BrimstoneElemental>(), 17 },
                { NPCID.SkeletronPrime, 18 },
                { NPCType<CalamitasRun3>(), 19 },
                { NPCID.Plantera, 20 },
                { NPCType<Leviathan>(), 21 },
                { NPCType<Siren>(), 21 },
                { NPCType<AstrumAureus>(), 22 },
                { NPCID.Golem, 23 },
                { NPCType<PlaguebringerGoliath>(), 24 },
                { NPCID.DukeFishron, 25 },
                { NPCType<RavagerBody>(), 26 },
                { NPCID.CultistBoss, 27 },
                { NPCType<AstrumDeusHeadSpectral>(), 28 },
                { NPCID.MoonLordCore, 29 },
                { NPCType<ProfanedGuardianBoss>(), 30 },
                { NPCType<Bumblefuck>(), 31 },
                { NPCType<Providence>(), 32 },
                { NPCType<CeaselessVoid>(), 33 },
                { NPCType<StormWeaverHead>(), 34 },
                { NPCType<Signus>(), 35 },
                { NPCType<Polterghast>(), 36 },
                { NPCType<OldDuke>(), 37 },
                { NPCType<DevourerofGodsHead>(), 38 },
                { NPCType<Yharon>(), 39 },
                { NPCType<SupremeCalamitas>(), 40 },
                { NPCType<AresBody>(), 41 },
                { NPCType<ThanatosHead>(), 41 },
                { NPCType<Artemis>(), 41 },
                { NPCType<Apollo>(), 41 },
                { NPCType<EidolonWyrmHeadHuge>(), 42 },
                { NPCID.QueenSlimeBoss, 43 },
                { NPCID.HallowBoss, 44 },
                { NPCID.Deerclops, 45 }
            };

            bossMinionList = new List<int>()
            {
                NPCType<DesertNuisanceHead>(),
                NPCType<DesertNuisanceBody>(),
                NPCType<DesertNuisanceTail>(),
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
                NPCID.QueenSlimeMinionBlue,
                NPCID.QueenSlimeMinionPink,
                NPCID.QueenSlimeMinionPurple,
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
                NPCType<SupremeCataclysm>(),
                NPCType<SupremeCatastrophe>()
            };

            legOverrideList = new List<int>()
            {
                EquipLoader.GetEquipSlot(CalamityMod.Instance, "ProfanedSoulCrystal", EquipType.Legs),
                EquipLoader.GetEquipSlot(CalamityMod.Instance, "AquaticHeart", EquipType.Legs),
                //CalamityMod.Instance.GetEquipSlot("SirenLeg", EquipType.Legs), whate even was SirenLeg vs SirenLegAlt?
                EquipLoader.GetEquipSlot(CalamityMod.Instance, "Popo", EquipType.Legs)
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
                ProjectileID.StardustDragon1,
                ProjectileID.StardustDragon2,
                ProjectileID.StardustDragon3,
                ProjectileID.StardustDragon4,
                ProjectileType<DeathstareEyeball>(),
                ProjectileType<MechwormHead>(),
                ProjectileType<MechwormBody>(),
                ProjectileType<MechwormTail>(),
                ProjectileType<EndoHydraHead>(),
                ProjectileType<EndoHydraBody>(),
                ProjectileType<SeekerSummonProj>(),
                ProjectileType<SepulcherMinion>(),
                ProjectileType<MountedScannerLaser>()
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

            EncryptedSchematicIDRelationship = new Dictionary<int, int>()
            {
                [1] = ItemType<EncryptedSchematicPlanetoid>(),
                [2] = ItemType<EncryptedSchematicJungle>(),
                [3] = ItemType<EncryptedSchematicHell>(),
                [4] = ItemType<EncryptedSchematicIce>(),
            };

            DisabledSummonerNerfItems = new();
            DisabledSummonerNerfMinions = new();
        }

        public static void UnloadLists()
        {
            donatorList = null;
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
            alcoholList = null;
            spearAutoreuseList = null;
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
            pierceResistExceptionList = null;

            AstrumDeusIDs = null;
            DevourerOfGodsIDs = null;
            CosmicGuardianIDs = null;
            AquaticScourgeIDs = null;
            PerforatorIDs = null;
            DesertScourgeIDs = null;
            EaterofWorldsIDs = null;
            SlimeGodIDs = null;
            DeathModeSplittingWormIDs = null;
            DestroyerIDs = null;
            ThanatosIDs = null;
            AresIDs = null;
            SkeletronPrimeIDs = null;
            StormWeaverIDs = null;
            BoundNPCIDs = null;
            GrenadeResistIDs = null;
            ZeroContactDamageNPCList = null;
            HardmodeNPCNerfList = null;

            BossRushHPChanges?.Clear();
            BossRushHPChanges = null;
            BossValues?.Clear();
            BossValues = null;
            bossTypes?.Clear();
            bossTypes = null;

            legOverrideList = null;

            kamiDebuffColorImmuneList = null;

            MinionsToNotResurrectList = null;
            ZeroMinionSlotExceptionList = null;
            DontCopyOriginalMinionAIList = null;

            EncryptedSchematicIDRelationship = null;

            DisabledSummonerNerfItems = null;
            DisabledSummonerNerfMinions = null;
        }
    }
}
