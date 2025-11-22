using System.Collections.Generic;
using CalamityMod.Buffs;
using CalamityMod.Buffs.Alcohol;
using CalamityMod.Buffs.Cooldowns;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Potions;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.DraedonMisc;
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
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CalClone;
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
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod
{
    // TODO -- This can be made into a ModSystem with simple OnModLoad and Unload hooks.
    public class CalamityLists
    {
        public static IList<string> donatorList;
        public static List<int> projectileDestroyExceptionList;
        public static List<int> enemyImmunityList;
        public static List<int> confusionEnemyList;
        public static List<int> dungeonEnemyBuffList;
        public static List<int> dungeonProjectileBuffList;
        public static List<int> bossHPScaleList;
        public static List<int> friendlyBeeList;
        public static List<int> debuffList;
        public static List<int> fireDebuffList;
        public static List<int> sicknessDebuffList;
        public static List<int> alcoholList;
        public static List<int> spearAutoreuseList;
        public static List<int> pumpkinMoonBuffList;
        public static List<int> frostMoonBuffList;
        public static List<int> eclipseBuffList;
        public static List<int> eventProjectileBuffList;
        public static List<int> noRageWormSegmentList;
        public static List<int> needsDebuffIconDisplayList;
        public static List<int> scopedWeaponList;
        public static List<int> highTestFishList;
        public static List<int> forceItemList;
        public static List<int> livingFireBlockList;
        public static List<int> amalgamBuffList;
        public static List<int> persistentBuffList;
        public static List<int> MagicGunIDs;
        public static List<int> MushroomWeaponIDs;
        public static List<int> MushroomProjectileIDs;

        // Some of these lists of enemies are unused, but were difficult to create. Many of these common enemy types have a ton of variants.
        public static List<int> zombieList;
        public static List<int> demonEyeList;
        public static List<int> skeletonList;
        public static List<int> angryBonesList;
        public static List<int> hornetList;
        public static List<int> mossHornetList;
        public static List<int> minibossList;

        public static List<int> pierceResistList;
        public static List<int> pierceResistExceptionLeviAureusList;
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
        public static List<int> RavagerIDs;
        public static List<int> GolemIDs;
        public static List<int> BoundNPCIDs;

        public static List<int> GrenadeResistIDs;
        public static List<int> ZeroContactDamageNPCList;
        public static List<int> HardmodeNPCNerfList;

        public static SortedDictionary<int, int> BossRushHPChanges;
        public static SortedDictionary<int, int> BossValues;
        public static SortedDictionary<int, int> bossTypes;

        public static List<int> legOverrideList;

        public static Dictionary<int, int> EncryptedSchematicIDRelationship;

        public static List<int> DisabledSummonerNerfItems;
        public static List<int> DisabledSummonerNerfMinions;

        public static List<int> VeneratedLocketBanlist; //To ban projectiles from locket, mainly spikeballs altho Toasty asked me to add mod calls for adding stuff like Dreamtastic

        public static void LoadLists()
        {
            var newDonatorList = new List<string>()
            {
                "Aerosyn",
                "always-tired",
                "Amandalias",
                "andrew rodriguez",
                "Ant Slime",
                "ArcTwik",
                "Arcxus",
                "Arialen",
                "Ariallis",
                "awesomechapro",
                "Axy Usagi",
                "Azariah",
                "azazel",
                "Bepzi25",
                "BlueRay_256",
                "Boomdiada",
                "Borb9834",
                "botbot94",
                "Brio_Scarlet",
                "Brodiero-Solar",
                "Bruggs",
                "c0d3_404",
                "Careless_imp",
                "Catkey",
                "Charge Maniac",
                "Chin",
                "Chloe",
                "Chow Chow",
                "Cosmore",
                "CrazyGamer69",
                "Creeper Hunter 2K0",
                "Darthlego",
                "Diamond Nife",
                "djsnj20",
                "DogVTF",
                "Dragev",
                "Easy Perrin",
                "Entrian",
                "Equinoxux",
                "EternalGrayson",
                "FishBread",
                "Grant Curtiss",
                "Halleyvetica",
                "Hamsting",
                "helptree",
                "Herr Feuer",
                "Iptktp9",
                "Jalapeno9",
                "Jankle",
                "JFL",
                "Johnny",
                "Juneark",
                "J.U.N.E.S",
                "Justin",
                "Kaitlyn-Kerbulon",
                "KapryÅny",
                "Kat Manklow",
                "Kelt",
                "Kiddorox",
                "Kiwi_lol",
                "Levi Sharpe",
                "Liam Rafle",
                "Lightedflare",
                "Lolmoeven",
                "Luke Wakumoto",
                "Lumenos",
                "Magic Love",
                "Magicoal",
                "MalachiteWisp",
                "MarioMan84",
                "mARvOEoUS",
                "MaxingOut",
                "Medi",
                "Mint_loll",
                "Misterbottle8",
                "MizzUltraViolet DamienTK",
                "Nature",
                "niceguysrage",
                "nill bye",
                "Nova Solarius",
                "NyxxyNightstar",
                "OakhamSam",
                "PantySlack",
                "Patch357",
                "Phil Broome",
                "Pomelo",
                "Pseulamitas",
                "Red",
                "Red X",
                "Renavo",
                "Roadie Roadster",
                "Robert Yaron",
                "roryoftheabyss",
                "Rowan Shane",
                "Sable",
                "Salted Warlock",
                "Sharktank6",
                //"Shayy", // Removed from circulation as the user in question committed acts undeserving of recognition
                "shredalert",
                "SirChaos189",
                "SomeRandomPerson",
                "Taylor Olligoci", // also an ex-dev. Listed as "Lilac Olligoci" on Patreon. There were two "Taylor"s on the old list
                "Tethox",
                "thalass",
                "thanat.oshi",
                "The Deer Who Sold The World",
                "The Roborex",
                "Thessyll",
                "Trinity Series",
                "Tsukara",
                "Tweee",
                "twist",
                "Umbara",
                "Unreal Parrot",
                "V00194",
                "Vanillin",
                "Voltron284",
                "WatWouldJesusDo",
                "weisslerren",
                "Xtra Trinity 3678",
                "Yaggitarius",
                "YashimaYamanata",
                "Zachtoplasm",
                "Zackstar02",
                "ZoeyPlague",
            };

            var oldDonatorList = new List<string>
            {
                "116taj",
                "26-4-1",
                "2Larry2",
                "3en",
                "3x1t_5tyl3",
                "adain",
                "Adamko",
                "Aden C.N.",
                "Aero",
                "Aero (Aero#4599)",
                "Æthereal",
                "Afzofa",
                "Aidan",
                "Aidan Spears",
                "Akkolite",
                "Albino gonkvader",
                "Alec",
                "Aleksanders",
                "Aleksh",
                "Alex",
                "Alexander",
                "Alexis",
                "Alex N",
                "Alfragiste",
                "Allegro",
                "allosar",
                "Ally2Cute",
                "Altzeus",
                "anglerraptor",
                "Anish",
                "A_Normal_Person",
                "Anton",
                "Antonie",
                "anything5000",
                "Apotheosis",
                "apotofkoolaid",
                "Arcadia",
                "Arche",
                "Archie",
                "Ari",
                "Ariscottle",
                "Arkhine",
                "Arthur",
                "Arti",
                "asdf935",
                "Ash",
                "Ashamper",
                "Asheel",
                "Ashton",
                "Audrey Lynn Beemus",
                "Avery",
                "Azura",
                "Azure",
                "Azzilan",
                "BakaQing",
                "Ballin",
                "Barbara",
                "Barrett Turner",
                "Ben",
                "Bendy",
                "Beta165",
                "bikmin",
                "Bill",
                "Billy",
                "Blackbluue",
                "Bladesaber",
                "Blobby6799",
                "BobIsNotMyRealName",
                "Bossy Punch",
                "Braden",
                "BreachNClear747",
                "Bread",
                "Brendan",
                "Brian",
                "Briny_Coffee",
                "Broken Faucet",
                "Bruh.PNG",
                "Brutus",
                "Brutzli",
                "BumbleDoge",
                "Buppercups - Roblox",
                "Bwlstorm",
                "CaineSenpai",
                "Calcium Comrade",
                "callisto",
                "Cameron",
                "Carboniferous",
                "Carduelis",
                "CasualNoLifer",
                "Cerberus",
                "Chaos",
                "ChaosChaos",
                "ChAoS DiScOrD",
                "Chaotic Reks",
                "Chaozhi",
                "Charles",
                "Check pins",
                "Cheddar",
                "Chigbungus",
                "Chip",
                "ChrigTopher",
                "Christopher",
                "Cinder",
                "Circuit-Jay",
                "cocodezi_",
                "Cody",
                "Cole",
                "Colin",
                "Colin V",
                "Commander Frostbite",
                "Conner",
                "Coolguystorm YT",
                "Corn M. Cobb",
                "cosmickalamity",
                "CosmicStarIight",
                "CrabBar",
                "Creamy",
                "CrimsonCrips",
                "Crippling-Ambition",
                "Cristian(Mihaii)",
                "Crysthamyr",
                "Culex",
                "Curtis",
                "cytokat",
                "Daawnz",
                "Dakota",
                "Dakota C",
                "DanYami",
                "DaPyRo",
                "darkhawke",
                "Darkus",
                "Darkweb",
                "Darren",
                "Dasdruid",
                "dawnboi",
                "dawn thunder",
                "Dayne",
                "ddoogg88 tdog",
                "Deallly",
                "Dee",
                "DeLordeyeee",
                "DemoN K!ng",
                "dennis",
                "Depressed Dad Gaming",
                "Derdjin",
                "DESPACITO",
                "Destiny Stallcup",
                "Destructoid",
                "DevAesthetic",
                "DevilSunrise",
                "Devin",
                "Devon",
                "Devonte",
                "dexxer65",
                "Dionysos",
                "discokittie",
                "DjackV",
                "DOGMA",
                "Domrinth",
                "Done",
                "Doodled_lynx",
                "Doug",
                "Doveda",
                "drake093104",
                "Drakkece",
                "drenmus!",
                "Drip Veezy",
                "Driser",
                "Dr. Pawsworth",
                "Duck Satan",
                "Dull",
                "dummyAzure",
                "Dylan",
                "Eddie Spaghetti",
                "EdelVollMilch",
                "edm vibes",
                "Ein",
                "Eisaya",
                "Ekun",
                "Elementari",
                "Elfinlocks",
                "Elijah",
                "Empress Vega",
                "Eragon3942",
                "Eric",
                "Eternal Silence",
                "Ethan",
                "ethan",
                "everquartz",
                "F00d Demon",
                "Face",
                "FaeLoPondering",
                "False",
                "Fartein",
                "Faye",
                "Feels Fishy",
                "Ferret4T",
                "Finnrua",
                "fire",
                "Fish Repairs",
                "Fizzlpoprock",
                "Florian",
                "Forge",
                "Freakish",
                "Frederik",
                "Fweepachino",
                "Gameology",
                "GameRDheAsianSandwich",
                "Gamma Freya",
                "garfu",
                "GentSkeleton",
                "Georgios",
                "Gibb50",
                "GIGA MAN",
                "Gilded Gryphon",
                "Glaid",
                "gluten tag",
                "Goblin",
                "goo",
                "Goober",
                "Goomfrontlut",
                "GP",
                "GreenBerry",
                "GreenTea",
                "GregTheSpinarak",
                "Gretchen",
                "Griffin",
                "Grylken",
                "GTW High Cube",
                "Guwahavel",
                "Habb",
                "haefer.goat.oats",
                "Hana",
                "Handburger",
                "Hannes",
                "Hans Volter",
                "happy thoughts",
                "Hargestar",
                "Hayden",
                "Helixas",
                "HellGoat2",
                "High Charity",
                "Him",
                "Himakaze",
                "Hokojin",
                "Homunculus Derelictus",
                "hoosfire",
                "hubert thieblot",
                "Hunter",
                "Iconic Parker Gaming",
                "Iguy",
                "Indeciiissive",
                "IsaacInsomnia",
                "Izuna",
                "Jace Ufret",
                "Jack",
                "JackShizz",
                "Jackson",
                "Jakob",
                "James",
                "Jarod",
                "Javyz",
                "Jaydon",
                "Jaykob",
                "jc.",
                "Jeff",
                "Jenonen",
                "JensB__",
                "Jersey",
                "jes",
                "Jessire",
                "Jetpat3",
                "Jheybyrd",
                "jjth0m3",
                "Joe",
                "Joep",
                "Jordan",
                "Jose",
                "Joshua",
                "julius",
                "just akkolite",
                "Just a random guy",
                "JustLonelyPi",
                "Kaden",
                "Kaimonick",
                "Kaledoulas",
                "KAT-G307",
                "Katherine",
                "Kazurgundu",
                "KeL",
                "Kevin",
                "kgh8090",
                "Kinzoku",
                "Kipluck",
                "Kiyotu",
                "KJ",
                "Konorango",
                "Korb Orb",
                "Krankwagon",
                "KugelBlitz",
                "KurlozClown",
                "Lacuna",
                "Lady Shira",
                "Lagohz",
                "Lance",
                "Landon",
                "Larry That Barry",
                "Lauren",
                "Leonidas",
                "Levi",
                "Lilith",
                "Lime-Wars l 1",
                "Littlepiggy",
                "Lodude",
                "LompL",
                "Lord_Lucerne",
                "Loser",
                "Lucas",
                "LucasHM",
                "LucasTwocas",
                "Lucazii",
                "Luis",
                "Luke",
                "LumiEvi",
                "LvL-94",
                "M001NG",
                "Madd Cat",
                "MajinBagel",
                "Malik",
                "Marco",
                "Marissa443",
                "Mark",
                "Marko",
                "Mars",
                "martyrdomination",
                "Maskedmilo",
                "Mathuantie",
                "Matias",
                "Matthew",
                "Max Kim",
                "Mayhem",
                "MeanieG",
                "Melvin",
                "Mendzey",
                "Meow",
                "Merubel",
                "Met Vox",
                "Mikabul",
                "Min",
                "MingWhy",
                "Minty Candy",
                "MishiroUsui",
                "Misinput",
                "MissMudflaps",
                "Mister Winchester",
                "MittoMan",
                "MiyoshiEira",
                "Mobian",
                "Mohammad",
                "Moist Lad",
                "Monic",
                "Monti",
                "Moonicento",
                "MovingTarget_086",
                "Mr. Bones",
                "MrCreamen",
                "Mr.Matter",
                "MrNobody",
                "n0tacat",
                "Naglfar",
                "Nanaki",
                "Natalie",
                "Nathan",
                "Navigator",
                "Ne'er Dowell",
                "NEBULA",
                "NebulaMagePlays",
                "Nemesis 041",
                "Neoplasmatic",
                "NepNep",
                "Nexus",
                "Nicholas",
                "Nick H",
                "Nightinglade",
                "Nightskyflyer",
                "Nitronium Productions",
                "Noah",
                "NoOneElse",
                "Nothin Purrsonal",
                "Nuclear Chaos",
                "Nuclei",
                "Number1piratepan",
                "NuT NiTe",
                "NyanLegacy215",
                "Nyapano",
                "Nycro",
                "Oblivionisbruh",
                "Obsoleek",
                "Oceanman232",
                "OctolingGrimm",
                "oli saer",
                "Olkothan",
                "OnTheAirPogs",
                "oracle",
                "OriginForme487",
                "OrrangProto",
                "Orudeon",
                "Pacnysam",
                "Paltham",
                "Patrera",
                "Patrick",
                "Pedro",
                "Perditio Astrum",
                "Picasso's Bean",
                "Pigeon",
                "Plant Waifu",
                "Pneuma",
                "pomp neigh",
                "Ponynator",
                "Poopifier Poopifly",
                "Popsickle Yoshi",
                "porglesupreme",
                "Potato - Stego",
                "Potion Man",
                "Preston",
                "Primpy",
                "Prism",
                "Professor Pissington",
                "ProfessorWinston",
                "Proffesor prostate",
                "profoundmango69",
                "PsychoGrizzly",
                "Pusheen_",
                "pyobbo",
                "Pyromaniac146",
                "Qelrin",
                "Qwertz",
                "Rando Calrissian",
                "Random Weeb",
                "rawpie2",
                "Real mystlc",
                "Reanamet",
                "Reece",
                "Reiter splash",
                "RetroRed",
                "Rinja",
                "Rixu",
                "RKMoon",
                "Robert",
                "RockRecker39",
                "Rolandark",
                "Rooki",
                "Rossadon",
                "Rottingwood",
                "Roxas",
                "Ruben",
                "RuskieThe3rd",
                "Ruthoranium",
                "Ryaegos",
                "Ryan",
                "Ryan Baker-Ortiz",
                "Sabrina",
                "Sadouken",
                "Sailor Jolt",
                "SakuraWinterz",
                "Saladify",
                "Samuel",
                "Sanctuary",
                "Sandblast",
                "Sarcosuchus",
                "Schlarfblrfsch",
                "schmoovi",
                "SCONICBOOM",
                "Scribbles",
                "Scrumlet",
                "Seanツ",
                "Sevenfold",
                "Shadoku",
                "SharZz",
                "Shaun",
                "sherk",
                "Shifter",
                "Shiny",
                "Shiro",
                "Shirosity", // used to be GlitchOut
                "ShotgunAngel",
                "Shpee",
                "Sigil",
                "sk",
                "_Skeggy_",
                "SkeletonHunter96",
                "Skeli_G",
                "Slim",
                "Smart2004",
                "Smug",
                "Snowy",
                "Soko",
                "Solaire",
                "SoloMael",
                "SolsticeUnlimitd",
                "SomeRando",
                "SoyScoutSmasher",
                "Spider region",
                "Spirit Shield",
                "Splotchycrib",
                "SpookyNinja",
                "Squishy",
                "srxe",
                "Starmitzy",
                "StarryFox",
                "Steven",
                "Stormone",
                "Streakist",
                "Suicide Dreams",
                "Superbeepig",
                "sweatingdishes",
                "Taelishe",
                "Tails the Fox 92",
                "Taitou1",
                "Takeru",
                "Talmadge",
                "Taylor",
                "Team",
                "TemperedAether",
                "T E R M I N A T O R",
                "Terrarian Dragon",
                "Tervastator",
                "Tezguin",
                "That Katsafaros",
                "thebettercat",
                "TheBlackHand",
                "The Buildmonger",
                "The Davester",
                "The Evolution",
                "The Goliath",
                "TheGreatSako",
                "theHoopty",
                "The Illustrious Sqouinchuousor",
                "The Infinity",
                "The Pyro",
                "TheSilverGhost",
                "The Wither",
                "The Wolf Commando",
                "Thomas",
                "ThomasThePencil",
                "Thys",
                "Timon",
                "TitaniumLlama",
                "Tomat (Stevie)",
                "Tombarry Expresserino",
                "topnormal",
                "Toxin",
                "TwanTheGOAT",
                "Tyler",
                "Uberransy",
                "Ulmod",
                "Ultra Succ",
                "Umberto",
                "Underlost",
                "Valkyrie",
                "vcf55",
                "Veine",
                "velneu",
                "Vertigo",
                "VeryMasterNinja",
                "Vetus",
                "Victor",
                "Vimek Xol",
                "Vmar98",
                "Vorbis",
                "Vroomy Has -3,000 IQ",
                "Vyster",
                "Warlok",
                "Whale",
                "Whitegiraffe",
                "Will",
                "William",
                "william",
                "WillyDilly",
                "WinterTire",
                "Wodernet",
                "Wolfmaw",
                "WrathOfOlympus",
                "Xaphlactus",
                "xAqult",
                "XDkilljoy65XX",
                "xd Ow0",
                "xElectrix_",
                "Xsiana",
                "Xtra",
                "XusTingo9",
                "Xzier_Tengal",
                "Yatagarasu",
                "yayoi",
                "Yhashtur",
                "yiumik",
                "YuH",
                "YumeiSenshi",
                "Yumi",
                "Zacky",
                "Zekai",
                "Zerafir",
                "Zerimore",
                "Zombieh",
                "zombieseatflesh7",
                "zombie wolf",
                "Zombified _G",
                "ZyferDex_",
                "병현 송"
            };

            donatorList = [.. oldDonatorList, .. newDonatorList];

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

                ProjectileType<UrchinMaceProjectile>(),
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
                ProjectileType<FlakKrakenHoldout>(),
                ProjectileType<BuzzkillHoldout>(),
                ProjectileType<StarfleetMK2Gun>(),
                ProjectileType<NorfleetCannon>(),
                ProjectileType<FlurrystormCannonShooting>(),
                ProjectileType<ChickenCannonHeld>(),
                ProjectileType<PumplerHoldout>(),
                ProjectileType<ClockworkBowHoldout>(),
                ProjectileType<UltimaBowProjectile>(),
                ProjectileType<CondemnationHoldout>(),
                ProjectileType<SurgeDriverHoldout>(),
                ProjectileType<StarmageddonHeld>(),

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
                ProjectileType<NebulousCataclysm_Held>(),

                ProjectileType<FlakKrakenProjectile>(),
                ProjectileType<InfernadoFriendly>(),
                ProjectileType<DragonRageStaff>(),
                ProjectileType<MurasamaSlash>(),
                ProjectileType<PhaseslayerProjectile>(),
                ProjectileType<TaintedBladeSlasher>(),
                ProjectileType<PhotonRipperProjectile>(),
                ProjectileType<SpineOfThanatosProjectile>(),

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

            enemyImmunityList = new List<int>()
            {
                NPCID.KingSlime,
                NPCType<KingSlimeJewelRuby>(),
                NPCType<KingSlimeJewelSapphire>(),
                NPCType<KingSlimeJewelEmerald>(),
                NPCID.EaterofWorldsHead,
                NPCID.EaterofWorldsBody,
                NPCID.EaterofWorldsTail,
                NPCID.BrainofCthulhu,
                NPCID.Creeper,
                NPCID.EyeofCthulhu,
                NPCType<BloodlettingServant>(),
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
                NPCType<SkeletronPrime2>(),
                NPCID.PrimeCannon,
                NPCID.PrimeSaw,
                NPCID.PrimeLaser,
                NPCID.PrimeVice,
                NPCID.Plantera,
                NPCID.PlanterasTentacle,
                NPCType<PlanterasFreeTentacle>(),
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
                NPCType<DesertNuisanceHeadYoung>(),
                NPCType<DesertNuisanceBodyYoung>(),
                NPCType<DesertNuisanceTailYoung>(),
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
                NPCType<EbonianPaladin>(),
                NPCType<CrimulanPaladin>(),
                NPCType<SplitEbonianPaladin>(),
                NPCType<SplitCrimulanPaladin>(),
                NPCType<Horse>(),
                NPCType<ThiccWaifu>(),
                NPCType<CryogenShield>(),
                NPCType<AquaticScourgeHead>(),
                NPCType<AquaticScourgeBody>(),
                NPCType<AquaticScourgeBodyAlt>(),
                NPCType<AquaticScourgeTail>(),
                NPCType<CragmawMire>(),
                NPCType<Cataclysm>(),
                NPCType<Catastrophe>(),
                NPCType<SoulSeeker>(),
                NPCType<GreatSandShark>(),
                NPCType<AnahitasIceShield>(),
                NPCType<AureusSpawn>(),
                NPCType<PlaguebringerMiniboss>(),
                NPCType<PlagueHomingMissile>(),
                NPCType<PlagueMine>(),
                NPCType<RavagerClawLeft>(),
                NPCType<RavagerClawRight>(),
                NPCType<RavagerLegLeft>(),
                NPCType<RavagerLegRight>(),
                NPCType<RockPillar>(),
                NPCType<RavagerHead>(),
                NPCType<ProfanedGuardianDefender>(),
                NPCType<ProfanedGuardianHealer>(),
                NPCType<Bumblefuck2>(),
                NPCType<ProvSpawnDefense>(),
                NPCType<ProvSpawnHealer>(),
                NPCType<ProvSpawnOffense>(),
                NPCType<BobbitWormHead>(),
                NPCType<Mauler>(),
                NPCType<ColossalSquid>(),
                NPCType<ReaperShark>(),
                NPCType<EidolonWyrmHead>(),
                NPCType<NuclearTerror>(),
                NPCType<OldDukeToothBall>(),
                NPCType<SulphurousSharkron>(),
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
                NPCType<CrimulanBlightSlime>(),
                NPCType<Cryon>(),
                NPCType<CryoSlime>(),
                NPCType<RenegadeWarlock>(),
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
                NPCType<Stormlion>(),
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
                BuffType<BrainRot>(),
                BuffType<ElementalMix>(),
                BuffType<GlacialState>(),
                BuffType<GodSlayerInferno>(),
                BuffType<AstralInfectionDebuff>(),
                BuffType<HolyFlames>(),
                BuffType<Irradiated>(),
                BuffType<Plague>(),
                BuffType<CrushDepth>(),
                BuffType<RiptideDebuff>(),
                BuffType<MarkedforDeath>(),
                BuffType<AbsorberAffliction>(),
                BuffType<ArmorCrunch>(),
                BuffType<Crumbling>(),
                BuffType<Vaporfied>(),
                BuffType<Eutrophication>(),
                BuffType<Dragonfire>(),
                BuffType<Nightwither>(),
                BuffType<VulnerabilityHex>(),
                BuffType<MiracleBlight>(),
                BuffType<WhisperingDeath>(),
                BuffType<FrozenLungs>(),
                BuffType<FishAlert>(),
                BuffType<HolyInferno>(),
                BuffType<IcarusFolly>(),
                BuffType<DoGExtremeGravity>(),
                // BuffType<NOU>(),
                BuffType<PopoNoselessBuff>(),
                BuffType<SearingLava>(),
                BuffType<WeakBrimstoneFlames>(),
                BuffType<Withered>()
            };

            fireDebuffList = new List<int>()
            {
                BuffID.OnFire,
                BuffID.OnFire3, // Hellfire
                BuffID.Burning, // Touching meteorite ore or hellstone without obsidian skull
                BuffID.CursedInferno,
                BuffID.ShadowFlame, // Vanilla Shadowflame, can normally never be applied to players
                BuffType<Shadowflame>(), // Calamity Shadowflame copy for players
                BuffType<SearingLava>(), // Crags lava
                BuffType<BrimstoneFlames>(),
                BuffType<HolyFlames>(),
                BuffType<GodSlayerInferno>(),
                BuffType<Dragonfire>(),
                BuffType<WeakBrimstoneFlames>(), // Aflame enchant self damage
                BuffType<BanishingFire>(),
            };

            sicknessDebuffList = new List<int>()
            {
                BuffID.Poisoned,
                BuffID.Venom,
                BuffType<SulphuricPoisoning>(),
                BuffType<AstralInfectionDebuff>(),
                BuffType<Plague>(),
                BuffType<AbsorberAffliction>(),
                BuffType<WhisperingDeath>(),
                BuffType<Irradiated>()
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
                BuffType<OldFashionedBuff>(),
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
                ItemID.Trident,
                ItemID.ThunderSpear
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

            noRageWormSegmentList = new List<int>()
            {
                NPCType<DesertScourgeBody>(),
                NPCType<DesertScourgeTail>(),
                NPCType<AquaticScourgeBody>(),
                NPCType<AquaticScourgeBodyAlt>(),
                NPCType<AquaticScourgeTail>(),
                NPCType<AstrumDeusBody>(),
                NPCType<AstrumDeusTail>(),
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
                NPCID.TargetDummy,
                NPCID.WallofFleshEye,
                NPCType<SuperDummyNPC>()
            };

            // TODO -- override HoldItem => Player.accFishingLine = true; on these items, just like the scope fix...
            highTestFishList = new List<int>()
            {
                ItemID.GoldenFishingRod,
                ItemType<EarlyBloomRod>(),
                ItemType<TheDevourerofCods>()
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
                ItemType<PearlofEnthrallment>(),
                ItemType<AquaticScourgeBag>(),
                ItemType<OldDukeBag>(),
                ItemType<LeviathanBag>(),
                ItemType<OldDukeMask>(),
                ItemType<LeviathanMask>(),
                ItemType<AquaticScourgeMask>(),
                ItemType<OldDukeTrophy>(),
                ItemType<LeviathanTrophy>(),
                ItemType<AquaticScourgeTrophy>(),
                ItemType<LoreAquaticScourge>(),
                ItemType<LoreLeviathanAnahita>(),
                ItemType<LoreSulphurSea>(),
                ItemType<LoreAbyss>(),
                ItemType<LoreOldDuke>(),
                ItemType<PearlShard>(),
                ItemType<AeroStone>(),
                ItemType<TheCommunity>(),
                ItemType<DukesDecapitator>(),
                ItemType<SulphurousSand>(),
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

            amalgamBuffList = new List<int>()
            {
                BuffType<AnechoicCoatingBuff>(),
                BuffType<AstralInjectionBuff>(),
                BuffType<BaguetteBuff>(),
                BuffType<BloodfinBoost>(),
                BuffType<BoundingBuff>(),
                BuffType<CalciumBuff>(),
                BuffType<CeaselessHunger>(),
                BuffType<GravityNormalizerBuff>(),
                BuffType<Omniscience>(),
                BuffType<PhotosynthesisBuff>(),
                BuffType<ShadowBuff>(),
                BuffType<Soaring>(),
                BuffType<SulphurskinBuff>(),
                BuffType<TeslaBuff>(),
                BuffType<WeaponImbueBrimstone>(),
                BuffType<WeaponImbueCrumbling>(),
                BuffType<WeaponImbueHolyFlames>(),
                BuffType<Zen>(),
                BuffType<Zerg>(),
                BuffType<BloodyMaryBuff>(),
                BuffType<CaribbeanRumBuff>(),
                BuffType<CinnamonRollBuff>(),
                BuffType<EverclearBuff>(),
                BuffType<EvergreenGinBuff>(),
                BuffType<PurpleHazeBuff>(),
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
                BuffType<Trippy>(),
                BuffType<VodkaBuff>(),
                BuffType<WhiskeyBuff>(),
                BuffType<WhiteWineBuff>(),
                BuffID.ObsidianSkin,
                BuffID.Regeneration,
                BuffID.Swiftness,
                BuffID.Gills,
                BuffID.Ironskin,
                BuffID.ManaRegeneration,
                BuffID.MagicPower,
                BuffID.Featherfall,
                BuffID.Spelunker,
                BuffID.Invisibility,
                BuffID.Shine,
                BuffID.NightOwl,
                BuffID.Battle,
                BuffID.Thorns,
                BuffID.WaterWalking,
                BuffID.Archery,
                BuffID.Hunter,
                BuffID.Gravitation,
                BuffID.Tipsy,
                BuffID.WellFed,
                BuffID.WellFed2,
                BuffID.WellFed3,
                BuffID.Honey,
                BuffID.WeaponImbueVenom,
                BuffID.WeaponImbueCursedFlames,
                BuffID.WeaponImbueFire,
                BuffID.WeaponImbueGold,
                BuffID.WeaponImbueIchor,
                BuffID.WeaponImbueNanites,
                BuffID.WeaponImbueConfetti,
                BuffID.WeaponImbuePoison,
                BuffID.Lucky,
                BuffID.Mining,
                BuffID.Heartreach,
                BuffID.Calm,
                BuffID.Builder,
                BuffID.Titan,
                BuffID.Flipper,
                BuffID.Summoning,
                BuffID.Dangersense,
                BuffID.AmmoReservation,
                BuffID.Lifeforce,
                BuffID.Endurance,
                BuffID.Rage,
                BuffID.Inferno,
                BuffID.Wrath,
                BuffID.Lovestruck,
                BuffID.Stinky,
                BuffID.Fishing,
                BuffID.Sonar,
                BuffID.Crate,
                BuffID.Warmth,
                BuffID.SugarRush
            };

            persistentBuffList = new List<int>()
            {
                BuffType<WeaponImbueBrimstone>(),
                BuffType<WeaponImbueCrumbling>(),
                BuffType<WeaponImbueHolyFlames>(),
                BuffID.WeaponImbueVenom,
                BuffID.WeaponImbueCursedFlames,
                BuffID.WeaponImbueFire,
                BuffID.WeaponImbueGold,
                BuffID.WeaponImbueIchor,
                BuffID.WeaponImbueNanites,
                BuffID.WeaponImbueConfetti,
                BuffID.WeaponImbuePoison
            };

            MagicGunIDs = new List<int>()
            {
                ItemType<AbyssShocker>(),
                ItemType<AcidGun>(),
                ItemType<AethersWhisper>(),
                ItemType<AetherfluxCannon>(),
                ItemType<Omicron>(),
                ItemType<ApoctosisArray>(),
                ItemType<Cryophobia>(),
                ItemType<Effervescence>(),
                ItemType<EidolicWail>(),
                ItemType<GatlingLaser>(),
                ItemType<GaussPistol>(),
                ItemType<Genesis>(),
                ItemType<IonBlaster>(),
                ItemType<Lazhar>(),
                ItemType<NanoPurge>(),
                ItemType<PlasmaCaster>(),
                ItemType<PlasmaRifle>(),
                ItemType<PulsePistol>(),
                ItemType<PurgeGuzzler>(),
                ItemType<RainbowPartyCannon>(),
                ItemType<SHPC>(),
                ItemType<TeslaCannon>(),
                ItemType<TheSwarmer>(),
                ItemType<Thunderstorm>(),
                ItemType<Wingman>(),
                ItemID.BeeGun,
                ItemID.BubbleGun,
                ItemID.ChargedBlasterCannon,
                ItemID.HeatRay,
                ItemID.LaserMachinegun,
                ItemID.LaserRifle,
                ItemID.LeafBlower,
                ItemID.RainbowGun,
                ItemID.SpaceGun,
                ItemID.WaspGun,
                ItemID.ZapinatorGray,
                ItemID.ZapinatorOrange
            };

            MushroomWeaponIDs = new List<int>()
            {
                ItemType<Mycoroot>(),
                ItemType<InfestedClawmerang>(),
                ItemType<PuffShroom>(),
                ItemType<HyphaeRod>(),
                ItemType<Fungicide>(),
                ItemType<MycelialClaws>(),
                ItemType<Shroomer>(),
                ItemID.Hammush,
                ItemID.MushroomSpear,
                ItemID.Shroomerang
            };

            MushroomProjectileIDs = new List<int>()
            {
                ProjectileType<MycorootProj>(),
                ProjectileType<ShroomerangSpore>(),
                ProjectileType<InfestedClawmerangProj>(),
                ProjectileType<PuffCloud>(),
                ProjectileType<FungiOrb2>(),
                ProjectileType<FungiOrb>(),
                ProjectileType<Shroom>(),
                ProjectileID.TruffleSpore,
                ProjectileID.MushroomSpear,
                ProjectileID.Shroomerang
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
                NPCType<ReaperShark>(),
                NPCType<ColossalSquid>(),
                NPCType<GreatSandShark>(),
                NPCType<GiantClam>(),
                NPCType<ArmoredDiggerHead>(),
                NPCType<ArmoredDiggerBody>(),
                NPCType<ArmoredDiggerTail>(),
                NPCType<ThiccWaifu>(),
                NPCType<Horse>(),
                NPCType<PlaguebringerMiniboss>(),
                NPCID.Pumpking,
                NPCID.MourningWood,
                NPCID.IceQueen,
                NPCID.SantaNK1,
                NPCID.Everscream,
                NPCID.DD2Betsy,
                NPCID.Mothron,
                NPCID.MartianSaucer,
                NPCID.MartianSaucerCannon,
                NPCID.MartianSaucerCore,
                NPCID.MartianSaucerTurret,
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
                NPCType<AstrumAureus>(),
                NPCType<Leviathan>(),
                NPCType<RavagerHead>(),
                NPCType<RavagerClawLeft>(),
                NPCType<RavagerClawRight>(),
                NPCType<RavagerLegLeft>(),
                NPCType<RavagerLegRight>(),
                NPCType<AstrumDeusHead>(),
                NPCType<AstrumDeusBody>(),
                NPCType<AstrumDeusTail>(),
                NPCType<ProfanedRocks>(),
                NPCType<DarkEnergy>(),
                NPCType<StormWeaverHead>(),
                NPCType<StormWeaverBody>(),
                NPCType<StormWeaverTail>(),
                NPCType<CosmicGuardianHead>(),
                NPCType<CosmicGuardianBody>(),
                NPCType<CosmicGuardianTail>(),
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

            pierceResistExceptionLeviAureusList = new List<int>()
            {
                ProjectileID.NettleBurstEnd,
                ProjectileID.NettleBurstLeft,
                ProjectileID.NettleBurstRight,
                ProjectileType<AtlantisSpear>(),
                ProjectileType<AuroraFire>(),
                ProjectileType<BallisticPoisonCloud>(),
                ProjectileType<DuststormCloudHitbox>()
            };

            pierceResistExceptionList = new List<int>()
            {
                ProjectileID.Arkhalis,
                ProjectileID.ChargedBlasterLaser,
                ProjectileID.ClingerStaff,
                ProjectileID.FinalFractal,
                ProjectileID.FlyingKnife,
                ProjectileID.LastPrismLaser,
                ProjectileID.MechanicalPiranha,
                ProjectileID.MonkStaffT3,
                ProjectileID.PiercingStarlight,
                ProjectileID.Terragrim,
                ProjectileType<AcidicSaxBubble>(),
                ProjectileType<BlushieStaffProj>(),
                ProjectileType<BonebreakerProjectile>(),
                ProjectileType<DarkSparkBeam>(),
                ProjectileType<DevilsSunriseCyclone>(),
                ProjectileType<DevilsSunriseProj>(),
                ProjectileType<DragonRageStaff>(),
                ProjectileType<EclipsesStealth>(),
                ProjectileType<EidolicWailSoundwave>(),
                ProjectileType<EmesisGore>(),
                ProjectileType<EradicatorProjectile>(),
                ProjectileType<ExoFlareCluster>(),
                ProjectileType<EyeOfNightCell>(),
                ProjectileType<FantasyTalismanProj>(),
                ProjectileType<FantasyTalismanStealth>(),
                ProjectileType<GodsParanoiaProj>(),
                ProjectileType<InsidiousHarpoon>(),
                ProjectileType<JawsProjectile>(),
                ProjectileType<LeviathanTooth>(),
                ProjectileType<LiliesOfFinalityAoE>(),
                ProjectileType<LionfishProj>(),
                ProjectileType<MechanicalBarracuda>(),
                ProjectileType<MetalShard>(),
                ProjectileType<MurasamaSlash>(),
                ProjectileType<NastyChollaBol>(),
                ProjectileType<OmnibladeSwing>(),
                ProjectileType<PhaseslayerProjectile>(),
                ProjectileType<PhotonRipperProjectile>(),
                ProjectileType<PlaguedFuelPackCloud>(),
                ProjectileType<PlantationStaffSporeCloud>(),
                ProjectileType<PrismaticBeam>(),
                ProjectileType<RancorLaserbeam>(),
                ProjectileType<ReaperProjectile>(),
                ProjectileType<RespiteblockHoldout>(),
                ProjectileType<SacrificeProjectile>(),
                ProjectileType<SnapClamProj>(),
                ProjectileType<SnapClamStealth>(),
                ProjectileType<Snowflake>(),
                ProjectileType<SparklingLaser>(),
                ProjectileType<SpiritCongregation>(),
                ProjectileType<StarmageddonBinaryStarCenter>(),
                ProjectileType<StickyBol>(),
                ProjectileType<SulphuricBlast>(),
                ProjectileType<TaserHook>(),
                ProjectileType<Teslabeam>(),
                ProjectileType<TyphonsGreedStaff>(),
                ProjectileType<UrchinBall>(),
                ProjectileType<UrchinBallSpike>(),
                ProjectileType<UrchinMaceProjectile>(),
                ProjectileType<UrchinStingerProj>(),
                ProjectileType<ViolenceThrownProjectile>(),
                ProjectileType<WaterLeechProj>(),
                ProjectileType<YateveoBloomProj>(),
                ProjectileType<YharimsCrystalBeam>(),
            };

            // Lists of enemies that resist piercing to some extent (mostly worms).
            // Could prove useful for other things as well.

            AstrumDeusIDs = new List<int>
            {
                NPCType<AstrumDeusHead>(),
                NPCType<AstrumDeusBody>(),
                NPCType<AstrumDeusTail>()
            };

            DevourerOfGodsIDs = new List<int>
            {
                NPCType<DevourerofGodsHead>(),
                NPCType<DevourerofGodsBody>(),
                NPCType<DevourerofGodsTail>()
            };

            CosmicGuardianIDs = new List<int>
            {
                NPCType<CosmicGuardianHead>(),
                NPCType<CosmicGuardianBody>(),
                NPCType<CosmicGuardianTail>()
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
                NPCType<EbonianPaladin>(),
                NPCType<CrimulanPaladin>(),
                NPCType<SplitEbonianPaladin>(),
                NPCType<SplitCrimulanPaladin>(),
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
                NPCType<SkeletronPrime2>(),
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

            // Purposefully does not include the freed head
            RavagerIDs = new List<int>
            {
                NPCType<RavagerBody>(),
                NPCType<RavagerClawLeft>(),
                NPCType<RavagerClawRight>(),
                NPCType<RavagerLegLeft>(),
                NPCType<RavagerLegRight>(),
                NPCType<RavagerHead>()
            };

            GolemIDs = new List<int>
            {
                NPCID.Golem,
                NPCID.GolemHead,
                NPCID.GolemHeadFree,
                NPCID.GolemFistLeft,
                NPCID.GolemFistRight
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
                NPCID.Harpy,
                NPCID.Salamander,
                NPCID.Salamander2,
                NPCID.Salamander3,
                NPCID.Salamander4,
                NPCID.Salamander5,
                NPCID.Salamander6,
                NPCID.Salamander7,
                NPCID.Salamander8,
                NPCID.Salamander9,
                NPCID.GiantCursedSkull,
                NPCID.FungiBulb,
                NPCID.GiantFungiBulb,
                NPCID.IcyMerman,
                NPCID.AngryNimbus,
                NPCID.SandElemental,
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
                NPCID.BrainScrambler,
                NPCID.GigaZapper,
                NPCID.RayGunner,
                NPCID.ScutlixRider,
                NPCID.MartianWalker,
                NPCID.MartianTurret,
                NPCID.ElfCopter,
                NPCID.ElfArcher,
                NPCID.NebulaBrain,
                NPCID.NebulaSoldier,
                NPCID.StardustCellSmall,
                NPCID.StardustJellyfishBig,
                NPCID.StardustSoldier,
                NPCID.StardustSpiderBig,
                NPCID.VortexHornetQueen,
                NPCID.VortexRifleman,
                NPCID.VortexSoldier,
                NPCID.PirateShipCannon,
                NPCID.MartianSaucer,
                NPCID.MartianSaucerCannon,
                NPCID.MartianSaucerCore,
                NPCID.MartianSaucerTurret,
                NPCID.Probe,
                NPCID.CultistBoss,
                NPCID.GolemHead,
                NPCID.GolemHeadFree,
                NPCID.MoonLordFreeEye,
                NPCID.BloodSquid,
                NPCID.PlanterasHook,
                NPCID.Dandelion,
                NPCID.DD2DarkMageT1,
                NPCID.DD2DarkMageT3,
                NPCID.DD2OgreT2,
                NPCID.DD2OgreT3,
                NPCID.DD2GoblinBomberT1,
                NPCID.DD2GoblinBomberT2,
                NPCID.DD2GoblinBomberT3,
                NPCID.DD2JavelinstT1,
                NPCID.DD2JavelinstT2,
                NPCID.DD2JavelinstT3,
                NPCID.DD2KoboldWalkerT2,
                NPCID.DD2KoboldWalkerT3,
                NPCID.DD2DrakinT2,
                NPCID.DD2DrakinT3,
                NPCID.DD2KoboldFlyerT2,
                NPCID.DD2KoboldFlyerT3,
                NPCID.DD2WitherBeastT2,
                NPCID.DD2WitherBeastT3,
                NPCID.DD2LightningBugT3,
                NPCID.MourningWood,
                NPCID.Pumpking,
                NPCID.Everscream,
                NPCID.IceQueen,
                NPCID.SantaNK1,
                NPCID.DevourerBody,
                NPCID.DevourerTail,
                NPCID.DiggerBody,
                NPCID.DiggerTail,
                NPCID.TombCrawlerBody,
                NPCID.TombCrawlerTail,
                NPCID.DuneSplicerBody,
                NPCID.DuneSplicerTail,
                NPCID.GiantWormBody,
                NPCID.GiantWormTail,
                NPCID.LeechBody,
                NPCID.LeechTail,
                NPCID.StardustWormBody,
                NPCID.StardustWormTail,
                NPCID.SeekerBody,
                NPCID.SeekerTail,
                NPCID.BoneSerpentBody,
                NPCID.BoneSerpentTail,
                NPCID.WyvernBody,
                NPCID.WyvernTail,
                NPCID.WyvernBody2,
                NPCID.WyvernBody3,
                NPCID.WyvernLegs,
                NPCID.CultistDragonBody1,
                NPCID.CultistDragonBody2,
                NPCID.CultistDragonBody3,
                NPCID.CultistDragonBody4,
                NPCID.CultistDragonTail,
                NPCID.BloodEelBody,
                NPCID.BloodEelTail,
                NPCID.AncientDoom
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
                NPCID.DesertGhoul,
                NPCID.DesertGhoulCorruption,
                NPCID.DesertGhoulCrimson,
                NPCID.DesertGhoulHallow,
                NPCID.DuneSplicerHead,
                NPCID.EnchantedSword,
                NPCID.FloatyGross,
                NPCID.GiantBat,
                NPCID.GiantFlyingFox,
                NPCID.FungiSpore,
                NPCID.GiantTortoise,
                NPCID.IceTortoise,
                NPCID.HoppinJack,
                NPCID.Mimic,
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
                NPCID.Wraith,
                NPCID.ChatteringTeethBomb,
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
            BossRushHPChanges = new SortedDictionary<int, int>
            {
                // Tier 1
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
                { NPCType<KingSlimeJewelRuby>(), 21000 },
                { NPCType<KingSlimeJewelSapphire>(), 18000 },
                { NPCType<KingSlimeJewelEmerald>(), 24000 },

                { NPCID.EyeofCthulhu, 450000 }, // 30 seconds
                { NPCID.ServantofCthulhu, 6000 },
                { NPCType<BloodlettingServant>(), 12000 },

                { NPCID.EaterofWorldsHead, 10000 }, // 30 seconds + immunity timer at start
                { NPCID.EaterofWorldsBody, 10000 },
                { NPCID.EaterofWorldsTail, 10000 },

                { NPCID.BrainofCthulhu, 100000 }, // 30 seconds with creepers
                { NPCID.Creeper, 10000 },

                { NPCID.QueenBee, 315000 }, // 30 seconds
                { NPCID.Bee, 3000 },
                { NPCID.BeeSmall, 2000 },
                { NPCID.BigHornetHoney, 10000 },
                { NPCID.HornetHoney, 7500 },
                { NPCID.LittleHornetHoney, 5000 },

                { NPCID.Deerclops, 315000 }, // 30 seconds

                { NPCID.SkeletronHead, 150000 }, // 30 seconds
                { NPCID.SkeletronHand, 60000 },

                { NPCID.WallofFlesh, 450000 }, // 30 seconds
                { NPCID.WallofFleshEye, 450000 },
                { NPCID.TheHungry, 10000 },
                { NPCID.TheHungryII, 5000 },
                { NPCID.LeechHead, 5000 },
                { NPCID.LeechBody, 5000 },
                { NPCID.LeechTail, 5000 },

                // Tier 2
                { NPCID.QueenSlimeBoss, 150000 }, // 30 seconds
                { NPCID.QueenSlimeMinionBlue, 6000 },
                { NPCID.QueenSlimeMinionPink, 6000 },
                { NPCID.QueenSlimeMinionPurple, 5000 },

                { NPCID.Spazmatism, 150000 }, // 30 seconds
                { NPCID.Retinazer, 125000 },

                { NPCID.TheDestroyer, 250000 }, // 30 seconds + immunity timer at start
                { NPCID.TheDestroyerBody, 250000 },
                { NPCID.TheDestroyerTail, 250000 },
                { NPCID.Probe, 5000 },

                { NPCID.SkeletronPrime, 160000 }, // 30 seconds
                { NPCType<SkeletronPrime2>(), 160000 },
                { NPCID.PrimeVice, 54000 },
                { NPCID.PrimeCannon, 45000 },
                { NPCID.PrimeSaw, 45000 },
                { NPCID.PrimeLaser, 38000 },

                { NPCID.Plantera, 160000 }, // 30 seconds
                { NPCID.PlanterasTentacle, 5000 },
                { NPCType<PlanterasFreeTentacle>(), 5000 },

                // Tier 3
                { NPCID.Golem, 50000 }, // 30 seconds
                { NPCID.GolemHead, 30000 },
                { NPCID.GolemHeadFree, 30000 },
                { NPCID.GolemFistLeft, 25000 },
                { NPCID.GolemFistRight, 25000 },

                { NPCID.HallowBoss, 200000 }, // 30 seconds

                { NPCID.DukeFishron, 290000 }, // 30 seconds

                { NPCID.CultistBoss, 220000 }, // 30 seconds
                { NPCID.CultistDragonHead, 60000 },
                { NPCID.CultistDragonBody1, 60000 },
                { NPCID.CultistDragonBody2, 60000 },
                { NPCID.CultistDragonBody3, 60000 },
                { NPCID.CultistDragonBody4, 60000 },
                { NPCID.CultistDragonTail, 60000 },
                { NPCID.AncientCultistSquidhead, 50000 },

                { NPCID.MoonLordCore, 160000 }, // 1 minute
                { NPCID.MoonLordHand, 45000 },
                { NPCID.MoonLordHead, 60000 },
                { NPCID.MoonLordLeechBlob, 800 }

                // 9.5 minutes in total for vanilla Boss Rush bosses
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
                { NPCType<Crabulon>(), 4 },
                { NPCID.EaterofWorldsHead, 5 },
                { NPCID.EaterofWorldsBody, 5 },
                { NPCID.EaterofWorldsTail, 5 },
                { NPCID.BrainofCthulhu, 6 },
                { NPCType<HiveMind>(), 7 },
                { NPCType<PerforatorHive>(), 8 },
                { NPCID.QueenBee, 9 },
                { NPCID.SkeletronHead, 10 },
                { NPCType<SlimeGodCore>(), 11 },
                { NPCType<SplitEbonianPaladin>(), 11 },
                { NPCType<SplitCrimulanPaladin>(), 11 },
                { NPCID.WallofFlesh, 12 },
                { NPCType<Cryogen>(), 13 },
                { NPCID.Retinazer, 14 },
                { NPCID.Spazmatism, 14 },
                { NPCType<AquaticScourgeHead>(), 15 },
                { NPCID.TheDestroyer, 16 },
                { NPCType<BrimstoneElemental>(), 17 },
                { NPCID.SkeletronPrime, 18 },
                { NPCType<CalamitasClone>(), 19 },
                { NPCID.Plantera, 20 },
                { NPCType<Leviathan>(), 21 },
                { NPCType<Anahita>(), 21 },
                { NPCType<AstrumAureus>(), 22 },
                { NPCID.Golem, 23 },
                { NPCType<PlaguebringerGoliath>(), 24 },
                { NPCID.DukeFishron, 25 },
                { NPCType<RavagerBody>(), 26 },
                { NPCID.CultistBoss, 27 },
                { NPCType<AstrumDeusHead>(), 28 },
                { NPCID.MoonLordCore, 29 },
                { NPCType<ProfanedGuardianCommander>(), 30 },
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
                { NPCID.QueenSlimeBoss, 42 },
                { NPCID.HallowBoss, 43 },
                { NPCID.Deerclops, 44 }
            };

            legOverrideList = new List<int>()
            {
                EquipLoader.GetEquipSlot(CalamityMod.Instance, "ProfanedSoulCrystal", EquipType.Legs),
                EquipLoader.GetEquipSlot(CalamityMod.Instance, "AquaticHeart", EquipType.Legs),
                //CalamityMod.Instance.GetEquipSlot("SirenLeg", EquipType.Legs), whate even was SirenLeg vs SirenLegAlt?
                EquipLoader.GetEquipSlot(CalamityMod.Instance, "Popo", EquipType.Legs)
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

            VeneratedLocketBanlist = new List<int>()
            {
                ItemType<PoisonPack>(),
                ItemType<SkyStabber>(),
                ItemType<Nychthemeron>(),
                ItemType<HellsSun>(),
                ItemType<GodsParanoia>(),
                ItemType<SlickCane>(),
                ItemType<Mycoroot>(),
                ItemType<CosmicKunai>()
            };
        }

        public static void UnloadLists()
        {
            donatorList = null;
            projectileDestroyExceptionList = null;
            enemyImmunityList = null;
            confusionEnemyList = null;
            dungeonEnemyBuffList = null;
            dungeonProjectileBuffList = null;
            bossHPScaleList = null;
            friendlyBeeList = null;
            debuffList = null;
            fireDebuffList = null;
            sicknessDebuffList = null;
            alcoholList = null;
            spearAutoreuseList = null;
            pumpkinMoonBuffList = null;
            frostMoonBuffList = null;
            eclipseBuffList = null;
            eventProjectileBuffList = null;
            noRageWormSegmentList = null;
            needsDebuffIconDisplayList = null;
            scopedWeaponList = null;
            highTestFishList = null;
            forceItemList = null;
            livingFireBlockList = null;
            amalgamBuffList = null;
            persistentBuffList = null;
            MagicGunIDs = null;
            MushroomWeaponIDs = null;
            MushroomProjectileIDs = null;

            zombieList = null;
            demonEyeList = null;
            skeletonList = null;
            angryBonesList = null;
            hornetList = null;
            mossHornetList = null;
            minibossList = null;
            pierceResistList = null;
            pierceResistExceptionLeviAureusList = null;
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
            RavagerIDs = null;
            GolemIDs = null;
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

            EncryptedSchematicIDRelationship = null;

            DisabledSummonerNerfItems = null;
            DisabledSummonerNerfMinions = null;

            VeneratedLocketBanlist = null;
        }
    }
}
