using CalamityMod.Buffs;
using CalamityMod.Buffs.Alcohol;
using CalamityMod.Buffs.Cooldowns;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Pets;
using CalamityMod.Buffs.Potions;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.Summon;
using CalamityMod.Dusts;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.DifficultyItems;
using CalamityMod.Items.Fishing;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Fishing.SulphurCatches;
using CalamityMod.Items.Fishing.SunkenSeaCatches;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.UI;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.CalPlayer
{
    public enum GaelSwitchPhase
    {
        LoseRage = 0,
        None = 1
    }
    public class CalamityPlayer : ModPlayer
    {
        #region Variables
        // No Category
        public static bool areThereAnyDamnBosses = false;
        public bool drawBossHPBar = true;
        public bool shouldDrawSmallText = true;
        private const int saveVersion = 0;
        //public int distanceFromBoss = -1;
        public int dashMod;
        public int projTypeJustHitBy;
        public int sCalDeathCount = 0;
        public int sCalKillCount = 0;
        public int deathCount = 0;
		public int actualMaxLife = 0;
		public bool killSpikyBalls = false;

		// Stat Meter
		public int[] damageStats = new int[5];
		public int[] critStats = new int[4];
		public int defenseStat = 0;
		public int DRStat = 0;
		public int meleeSpeedStat = 0;
		public int manaCostStat = 0;
		public int rogueVelocityStat = 0;
		public int minionSlotStat = 0;
		public int lifeRegenStat = 0;
		public int manaRegenStat = 0;
		public int armorPenetrationStat = 0;
		public int wingFlightTimeStat = 0;
		public int adrenalineChargeStat = 0;
		public int rageDamageStat = 0;
		public int moveSpeedStat = 0;
		public int abyssLightLevelStat = 0;
		public int[] abyssBreathLossStats = new int[4];
		public int abyssBreathLossRateStat = 0;
		public int[] abyssLifeLostAtZeroBreathStats = new int[4];

        // Timer and Counter
        public int bossRushImmunityFrameCurseTimer = 0;
        public int aBulwarkRareMeleeBoostTimer = 0;
        public int nebulaManaNerfCounter = 0;
        public int alcoholPoisonLevel = 0;
        public int modStealthTimer;
        public int dashTimeMod;
        public int hInfernoBoost = 0;
        public int pissWaterBoost = 0;
        public int gaelRageCooldown = 0;
        public int packetTimer = 0;
        public int bloodflareHeartTimer = 180;
        public int bloodflareManaTimer = 180;
        public int moneyStolenByBandit = 0;
        public int polarisBoostCounter = 0;
        public int reforges = 0;
        public int gaelSwipes = 0;
        public float modStealth = 1f;
        public float aquaticBoost = 1f;
        public float shieldInvinc = 5f;
        public GaelSwitchPhase gaelSwitchTimer = 0;

        // Sound
        public bool playRogueStealthSound = false;
        public bool playFullRageSound = true;
        public bool playFullAdrenalineSound = true;
        public bool playAdrenalineBurnoutSound = true;
        public bool playFullAdrenalineBurnoutSound = true;

        // Proficiency
        public int meleeLevel = 0;
        public int rangedLevel = 0;
        public int magicLevel = 0;
        public int rogueLevel = 0;
        public int summonLevel = 0;
        public bool shootFireworksLevelUpMelee = true;
        public bool shootFireworksLevelUpRanged = true;
        public bool shootFireworksLevelUpMagic = true;
        public bool shootFireworksLevelUpSummon = true;
        public bool shootFireworksLevelUpRogue = true;
        public int exactMeleeLevel = 0;
        public int exactRangedLevel = 0;
        public int exactMagicLevel = 0;
        public int exactSummonLevel = 0;
        public int exactRogueLevel = 0;
        public int gainLevelCooldown = 120;

        //Rogue
        public float rogueStealth = 0f;
        public float rogueStealthMax = 0f;
        public float stealthGenStandstill = 1f;
        public float stealthGenMoving = 0f;
        public float stealthGenMultiplier = 1f;
        private float stealthGenAcceleration = 1f;
        private bool stealthStrikeThisFrame = false;
        public bool stealthStrikeHalfCost = false;
        public bool stealthStrikeAlwaysCrits = false;
        public bool wearingRogueArmor = false;

        public float throwingDamage = 1f;
        public float throwingVelocity = 1f;
        public int throwingCrit = 4;
        public bool throwingAmmoCost75 = false;
        public bool throwingAmmoCost66 = false;
        public bool throwingAmmoCost50 = false;

        // Mount
        public bool onyxExcavator = false;
        public bool angryDog = false;
        public bool fab = false;
        public bool crysthamyr = false;

        // Pet
        public bool thirdSage = false;
        public bool thirdSageH = true; // Third sage healing
        public bool perfmini = false;
        public bool akato = false;
        public bool leviPet = false;
        public bool sirenPet = false;
        public bool fox = false;
        public bool chibii = false;
        public bool brimling = false;
        public bool bearPet = false;
        public bool kendra = false;
        public bool trashMan = false;
        public int trashManChest = -1;
        public bool astrophage = false;
        public bool babyGhostBell = false;
        public bool radiator = false;

        // Rage
        public const int stressMax = 10000;
        public int stress;
        public int stressCD;
        public bool stressLevel500 = false;
        public bool rageMode = false;
        public int gainRageCooldown = 60;

        // Adrenaline
        public const int adrenalineMax = 10000;
        public int adrenalineMaxTimer = 300;
        public int adrenalineDmgDown = 600;
        public float adrenalineDmgMult = 1f;
        public int adrenaline;
        public int adrenalineCD;
        public bool adrenalineMode = false;

        // Permanent Buff
        public bool extraAccessoryML = false;
        public bool eCore = false;
        public bool pHeart = false;
        public bool cShard = false;
        public bool mFruit = false;
        public bool bOrange = false;
        public bool eBerry = false;
        public bool dFruit = false;
        public bool revJamDrop = false;
        public bool rageBoostOne = false;
        public bool rageBoostTwo = false;
        public bool rageBoostThree = false;
        public bool adrenalineBoostOne = false;
        public bool adrenalineBoostTwo = false;
        public bool adrenalineBoostThree = false;

        // Lore
        public bool desertScourgeLore = false;
        public bool eaterOfWorldsLore = false;
        public bool hiveMindLore = false;
        public bool perforatorLore = false;
        public bool queenBeeLore = false;
        public bool skeletronLore = false;
        public bool wallOfFleshLore = false;
        public bool destroyerLore = false;
        public bool aquaticScourgeLore = false;
        public bool skeletronPrimeLore = false;
        public bool brimstoneElementalLore = false;
        public bool calamitasLore = false;
        public bool planteraLore = false;
        public bool leviathanAndSirenLore = false;
        public bool astrumAureusLore = false;
        public bool astrumDeusLore = false;
        public bool golemLore = false;
        public bool plaguebringerGoliathLore = false;
        public bool dukeFishronLore = false;
        public bool ravagerLore = false;
        public bool lunaticCultistLore = false;
        public bool moonLordLore = false;
        public bool providenceLore = false;
        public bool polterghastLore = false;
        public bool DoGLore = false;
        public bool yharonLore = false;
        public bool SCalLore = false;

        // Accessory
        public bool fasterMeleeLevel = false;
        public bool fasterRangedLevel = false;
        public bool fasterMagicLevel = false;
        public bool fasterSummonLevel = false;
        public bool fasterRogueLevel = false;
        public bool luxorsGift = false;
        public bool fungalSymbiote = false;
        public bool trinketOfChi = false;
        public bool gladiatorSword = false;
        public bool unstablePrism = false;
        public bool regenator = false;
        public bool theBee = false;
        public int theBeeDamage = 0;
		public bool alluringBait = false;
		public bool enchantedPearl = false;
		public bool fishingStation = false;
        public bool rBrain = false;
        public bool bloodyWormTooth = false;
        public bool afflicted = false;
        public bool affliction = false;
        public bool stressPills = false;
        public bool laudanum = false;
        public bool heartOfDarkness = false;
        public bool draedonsHeart = false;
        public bool draedonsStressGain = false;
        public bool rampartOfDeities = false;
        public bool vexation = false;
        public bool fBulwark = false;
        public bool dodgeScarf = false;
        public bool badgeOfBravery = false;
        public bool badgeOfBraveryRare = false;
        public bool scarfCooldown = false;
        public bool cryogenSoul = false;
        public bool yInsignia = false;
        public bool eGauntlet = false;
        public bool eTalisman = false;
        public bool statisBeltOfCurses = false;
        public bool elysianAegis = false;
        public bool elysianGuard = false;
        public bool nCore = false;
        public bool deepDiver = false;
        public bool abyssalDivingSuitPlates = false;
        public bool abyssalDivingSuitCooldown = false;
        public int abyssalDivingSuitPlateHits = 0;
        public bool sirenWaterBuff = false;
        public bool sirenIce = false;
        public bool sirenIceCooldown = false;
        public bool aSpark = false;
        public bool aSparkRare = false;
        public bool aBulwark = false;
        public bool aBulwarkRare = false;
        public bool dAmulet = false;
        public bool fCarapace = false;
        public bool gShell = false;
        public bool seaShell = false;
        public bool absorber = false;
        public bool aAmpoule = false;
        public bool pAmulet = false;
        public bool fBarrier = false;
        public bool aBrain = false;
        public bool lol = false;
        public bool raiderTalisman = false;
        public int raiderStack = 0;
        public bool sGenerator = false;
        public bool sDefense = false;
        public bool sPower = false;
        public bool sRegen = false;
        public bool IBoots = false;
        public bool elysianFire = false;
        public bool sTracers = false;
        public bool eTracers = false;
        public bool cTracers = false;
        public bool frostFlare = false;
        public bool beeResist = false;
        public bool uberBees = false;
        public bool projRef = false;
        public bool projRefRare = false;
        public int projRefRareLifeRegenCounter = 0;
        public bool nanotech = false;
        public bool eQuiver = false;
        public bool shadowMinions = false;
        public bool tearMinions = false;
        public bool alchFlask = false;
        public bool community = false;
        public bool fleshTotem = false;
        public int fleshTotemCooldown = 0;
        public bool bloodPact = false;
        public bool bloodflareCore = false;
        public bool coreOfTheBloodGod = false;
        public bool elementalHeart = false;
        public bool crownJewel = false;
        public bool celestialJewel = false;
        public bool astralArcanum = false;
        public bool harpyRing = false;
        public bool ironBoots = false;
        public bool depthCharm = false;
        public bool anechoicPlating = false;
        public bool jellyfishNecklace = false;
        public bool abyssalAmulet = false;
        public bool lumenousAmulet = false;
        public bool reaperToothNecklace = false;
        public bool aquaticEmblem = false;
        public bool darkSunRing = false;
        public bool calamityRing = false;
        public bool eArtifact = false;
        public bool dArtifact = false;
        public bool gArtifact = false;
        public bool pArtifact = false;
        public bool giantPearl = false;
        public bool normalityRelocator = false;
        public bool fabledTortoise = false;
        public bool manaOverloader = false;
        public bool royalGel = false;
        public bool oldDie = false;
        public bool ursaSergeant = false;
        public bool thiefsDime = false;
        public bool dynamoStemCells = false;
        public bool etherealExtorter = false;
        //public bool dukeScales = false;
        public bool sandWaifu = false;
        public bool sandBoobWaifu = false;
        public bool cloudWaifu = false;
        public bool brimstoneWaifu = false;
        public bool sirenWaifu = false;
        public bool fungalClump = false;
        public bool darkGodSheath = false;
        public bool inkBomb = false;
        public bool inkBombCooldown = false;
        public bool abyssalMirror = false;
        public bool abyssalMirrorCooldown = false;
        public bool eclipseMirror = false;
        public bool eclipseMirrorCooldown = false;
        public bool featherCrown = false;
        public bool moonCrown = false;
        public int featherCrownCooldown = 0;
        public int moonCrownCooldown = 0;
        public bool dragonScales = false;
        public bool gloveOfPrecision = false;
        public bool gloveOfRecklessness = false;
        public bool momentumCapacitor = false;
        public bool vampiricTalisman = false;
        public bool electricianGlove = false;
        public bool bloodyGlove = false;
        public bool filthyGlove = false;
        public bool sandCloak = false;
        public int sandCloakCooldown = 0;
        public bool spectralVeil = false;
        public int spectralVeilImmunity = 0;
        public bool plaguedFuelPack = false;
        public int plaguedFuelPackCooldown = 0;
        public int plaguedFuelPackDash = 0;
        public int plaguedFuelPackDirection = 0;

        // Armor Set
        public bool victideSet = false;
        public bool aeroSet = false;
        public bool statigelSet = false;
        public bool tarraSet = false;
        public bool tarraMelee = false;
        public bool tarragonCloak = false;
        public bool tarragonCloakCooldown = false;
		public int tarraDefenseTime = 600;
        public bool tarraMage = false;
        public int tarraMageHealCooldown = 0;
        public int tarraCrits = 0;
        public bool tarraRanged = false;
        public bool tarraThrowing = false;
		public bool tarragonImmunityCooldown = false;
		public bool tarragonImmunity = false;
		public int tarraThrowingCrits = 0;
        public bool tarraSummon = false;
        public bool bloodflareSet = false;
        public bool bloodflareMelee = false;
		public bool bloodflareFrenzy = false;
		public bool bloodFrenzyCooldown = false;
		public int bloodflareMeleeHits = 0;
        public bool bloodflareRanged = false;
		public bool bloodflareSoulCooldown = false;
        public bool bloodflareThrowing = false;
        public bool bloodflareMage = false;
        public int bloodflareMageCooldown = 0;
        public bool bloodflareSummon = false;
        public int bloodflareSummonTimer = 0;
        public bool godSlayer = false;
        public bool godSlayerDamage = false;
        public bool godSlayerMage = false;
        public bool godSlayerRanged = false;
        public bool godSlayerThrowing = false;
        public bool godSlayerSummon = false;
        public float godSlayerDmg;
        public bool godSlayerReflect = false;
        public bool godSlayerCooldown = false;
        public bool ataxiaBolt = false;
        public bool ataxiaFire = false;
        public bool ataxiaVolley = false;
        public bool ataxiaBlaze = false;
        public bool daedalusAbsorb = false;
        public bool daedalusShard = false;
        public bool reaverSpore = false;
        public bool reaverDoubleTap = false;
        public bool flamethrowerBoost = false;
        public bool shadeRegen = false;
        public bool shadowSpeed = false;
        public bool dsSetBonus = false;
        public bool auricBoost = false;
        public bool daedalusReflect = false;
        public bool daedalusSplit = false;
        public bool umbraphileSet = false;
        public bool reaverBlast = false;
        public bool reaverBurst = false;
        public bool astralStarRain = false;
        public int astralStarRainCooldown = 0;
        public float ataxiaDmg;
        public bool ataxiaMage = false;
        public bool ataxiaGeyser = false;
        public float xerocDmg;
        public bool xerocSet = false;
        public bool silvaSet = false;
        public bool silvaMelee = false;
        public bool silvaRanged = false;
        public bool silvaThrowing = false;
        public bool silvaMage = false;
        public bool silvaSummon = false;
        public bool hasSilvaEffect = false;
        public int silvaCountdown = 600;
        public int silvaHitCounter = 0;
        public bool auricSet = false;
        public bool omegaBlueChestplate = false;
        public bool omegaBlueSet = false;
        public bool omegaBlueHentai = false;
        public int omegaBlueCooldown = 0;
        public bool urchin = false;
        public bool valkyrie = false;
        public bool slimeGod = false;
        public bool molluskSet = false;
        public bool daedalusCrystal = false;
        public bool reaverOrb = false;
        public bool chaosSpirit = false;
        public bool redDevil = false;

		// Debuff
		public bool alcoholPoisoning = false;
        public bool shadowflame = false;
        public bool wDeath = false;
        public bool lethalLavaBurn = false;
        public bool aCrunch = false;
        public bool hAttack = false;
        public bool horror = false;
        public bool irradiated = false;
        public bool bFlames = false;
        public bool aFlames = false;
        public bool gsInferno = false;
        public bool astralInfection = false;
        public bool pFlames = false;
        public bool hFlames = false;
        public bool hInferno = false;
        public bool gState = false;
        public bool bBlood = false;
        public bool eGravity = false;
        public bool weakPetrification = false;
        public bool vHex = false;
        public bool eGrav = false;
        public bool warped = false;
        public bool marked = false;
        public bool cDepth = false;
        public bool fishAlert = false;
        public bool bOut = false;
        public bool clamity = false;
        public bool sulphurPoison = false;

        // Buff
        public bool trinketOfChiBuff = false;
        public int chiBuffTimer = 0;
        public bool corrEffigy = false;
        public bool crimEffigy = false;
        public bool rRage = false;
        public bool tRegen = false;
        public bool xRage = false;
        public bool xWrath = false;
        public bool graxDefense = false;
        public bool sMeleeBoost = false;
        public bool tFury = false;
        public bool cadence = false;
        public bool omniscience = false;
        public bool zerg = false;
        public bool zen = false;
		public bool bossZen = false;
        public bool yPower = false;
        public bool aWeapon = false;
        public bool tScale = false;
        public bool fabsolVodka = false;
        public bool mushy = false;
        public bool molten = false;
        public bool shellBoost = false;
        public bool cFreeze = false;
        public bool invincible = false;
        public bool shine = false;
        public bool anechoicCoating = false;
        public bool enraged = false;
        public bool revivify = false;
        public bool permafrostsConcoction = false;
        public bool armorCrumbling = false;
        public bool armorShattering = false;
        public bool ceaselessHunger = false;
        public bool calcium = false;
        public bool soaring = false;
        public bool bounding = false;
        public bool triumph = false;
        public bool penumbra = false;
        public bool photosynthesis = false;
        public bool astralInjection = false;
        public bool gravityNormalizer = false;
        public bool holyWrath = false;
        public bool profanedRage = false;
        public bool draconicSurge = false;
        public int draconicSurgeCooldown = 0;
        public bool vodka = false;
        public bool redWine = false;
        public bool grapeBeer = false;
        public bool moonshine = false;
        public bool rum = false;
        public bool whiskey = false;
        public bool fireball = false;
        public bool everclear = false;
        public bool bloodyMary = false;
        public bool tequila = false;
        public bool caribbeanRum = false;
        public bool cinnamonRoll = false;
        public bool tequilaSunrise = false;
        public bool margarita = false;
        public bool starBeamRye = false;
        public bool screwdriver = false;
        public bool moscowMule = false;
        public bool whiteWine = false;
        public bool evergreenGin = false;
        public bool tranquilityCandle = false;
        public bool chaosCandle = false;
        public bool purpleCandle = false;
        public bool blueCandle = false;
        public bool pinkCandle = false;
        public double pinkCandleHealFraction = 0D;
        public bool yellowCandle = false;
        public bool trippy = false;
        public bool amidiasBlessing = false;
        public bool polarisBoost = false;
        public bool polarisBoostTwo = false;
        public bool polarisBoostThree = false;
        public bool bloodfinBoost = false;
        public int bloodfinTimer = 30;

        // Minion
        public bool resButterfly = false;
        public bool glSword = false;
        public bool mWorm = false;
        public bool iClasper = false;
        public bool herring = false;
        public bool blackhawk = false;
        public bool calamari = false;
        public bool cEyes = false;
        public bool cSlime = false;
        public bool cSlime2 = false;
        public bool bStar = false;
        public bool aStar = false;
        public bool SP = false;
        public bool dCreeper = false;
        public bool bClot = false;
        public bool eAxe = false;
        public bool SPG = false;
        public bool aChicken = false;
        public bool cLamp = false;
        public bool pGuy = false;
        public bool sandnado = false;
        public bool gDefense = false;
        public bool gOffense = false;
        public bool gHealer = false;
        public bool cEnergy = false;
        public int healCounter = 300;
        public bool shellfish = false;
        public bool hCrab = false;
        public bool tDime = false;
        public bool allWaifus = false;
        public bool sCrystal = false;
        public bool sWaifu = false;
        public bool dWaifu = false;
        public bool cWaifu = false;
        public bool bWaifu = false;
        public bool slWaifu = false;
        public bool fClump = false;
        public bool rDevil = false;
        public bool aValkyrie = false;
        public bool sGod = false;
        public bool vUrchin = false;
        public bool cSpirit = false;
        public bool rOrb = false;
        public bool dCrystal = false;

        // Biome
        public bool ZoneCalamity = false;
        public bool ZoneAstral = false;
        public bool ZoneSunkenSea = false;
        public bool ZoneSulphur = false;
        public bool ZoneAbyss = false;
        public bool ZoneAbyssLayer1 = false;
        public bool ZoneAbyssLayer2 = false;
        public bool ZoneAbyssLayer3 = false;
        public bool ZoneAbyssLayer4 = false;
        public bool abyssDeath = false;
        public int abyssBreathCD;

        // Transformation
        public bool abyssalDivingSuitPrevious;
        public bool abyssalDivingSuit;
        public bool abyssalDivingSuitHide;
        public bool abyssalDivingSuitForce;
        public bool abyssalDivingSuitPower;
        public bool sirenBoobsPrevious;
        public bool sirenBoobs;
        public bool sirenBoobsHide;
        public bool sirenBoobsForce;
        public bool sirenBoobsPower;
        public bool sirenBoobsAltPrevious;
        public bool sirenBoobsAlt;
        public bool sirenBoobsAltHide;
        public bool sirenBoobsAltForce;
        public bool sirenBoobsAltPower;
        public bool snowmanPrevious;
        public bool snowman;
        public bool snowmanHide;
        public bool snowmanForce;
        public bool snowmanNoseless;
        public bool snowmanPower;

        #endregion

        #region SavingAndLoading
        public override void Initialize()
        {
            extraAccessoryML = false;
            eCore = false;
            mFruit = false;
            bOrange = false;
            eBerry = false;
            dFruit = false;
            pHeart = false;
            cShard = false;
            revJamDrop = false;
            rageBoostOne = false;
            rageBoostTwo = false;
            rageBoostThree = false;
            adrenalineBoostOne = false;
            adrenalineBoostTwo = false;
            adrenalineBoostThree = false;
            drawBossHPBar = true;
            shouldDrawSmallText = true;
        }

        public override TagCompound Save()
        {
            var boost = new List<string>();
            boost.AddWithCondition("extraAccessoryML", extraAccessoryML);
            boost.AddWithCondition("etherealCore", eCore);
            boost.AddWithCondition("miracleFruit", mFruit);
            boost.AddWithCondition("bloodOrange", bOrange);
            boost.AddWithCondition("elderBerry", eBerry);
            boost.AddWithCondition("dragonFruit", dFruit);
            boost.AddWithCondition("phantomHeart", pHeart);
            boost.AddWithCondition("cometShard", cShard);
            boost.AddWithCondition("revJam", revJamDrop);
            boost.AddWithCondition("rageOne", rageBoostOne);
            boost.AddWithCondition("rageTwo", rageBoostTwo);
            boost.AddWithCondition("rageThree", rageBoostThree);
            boost.AddWithCondition("adrenalineOne", adrenalineBoostOne);
            boost.AddWithCondition("adrenalineTwo", adrenalineBoostTwo);
            boost.AddWithCondition("adrenalineThree", adrenalineBoostThree);
            boost.AddWithCondition("bossHPBar", drawBossHPBar);
            boost.AddWithCondition("drawSmallText", shouldDrawSmallText);

            return new TagCompound
            {
                { "boost", boost },
                { "stress", stress },
                { "adrenaline", adrenaline },
                { "sCalDeathCount", sCalDeathCount },
                { "sCalKillCount", sCalKillCount },
                { "meleeLevel", meleeLevel },
                { "exactMeleeLevel", exactMeleeLevel },
                { "rangedLevel", rangedLevel },
                { "exactRangedLevel", exactRangedLevel },
                { "magicLevel", magicLevel },
                { "exactMagicLevel", exactMagicLevel },
                { "summonLevel", summonLevel },
                { "exactSummonLevel", exactSummonLevel },
                { "rogueLevel", rogueLevel },
                { "exactRogueLevel", exactRogueLevel },
                { "deathCount", deathCount },
                { "moneyStolenByBandit", moneyStolenByBandit },
                { "reforges", reforges }
            };
        }

        public override void Load(TagCompound tag)
        {
            var boost = tag.GetList<string>("boost");
            extraAccessoryML = boost.Contains("extraAccessoryML");
            eCore = boost.Contains("etherealCore");
            mFruit = boost.Contains("miracleFruit");
            bOrange = boost.Contains("bloodOrange");
            eBerry = boost.Contains("elderBerry");
            dFruit = boost.Contains("dragonFruit");
            pHeart = boost.Contains("phantomHeart");
            cShard = boost.Contains("cometShard");
            revJamDrop = boost.Contains("revJam");
            rageBoostOne = boost.Contains("rageOne");
            rageBoostTwo = boost.Contains("rageTwo");
            rageBoostThree = boost.Contains("rageThree");
            adrenalineBoostOne = boost.Contains("adrenalineOne");
            adrenalineBoostTwo = boost.Contains("adrenalineTwo");
            adrenalineBoostThree = boost.Contains("adrenalineThree");
            drawBossHPBar = boost.Contains("bossHPBar");
            shouldDrawSmallText = boost.Contains("drawSmallText");

            stress = tag.GetInt("stress");
            adrenaline = tag.GetInt("adrenaline");
            sCalDeathCount = tag.GetInt("sCalDeathCount");
            sCalKillCount = tag.GetInt("sCalKillCount");
            deathCount = tag.GetInt("deathCount");
            moneyStolenByBandit = tag.GetInt("moneyStolenByBandit");
            reforges = tag.GetInt("reforges");

            meleeLevel = tag.GetInt("meleeLevel");
            rangedLevel = tag.GetInt("rangedLevel");
            magicLevel = tag.GetInt("magicLevel");
            summonLevel = tag.GetInt("summonLevel");
            rogueLevel = tag.GetInt("rogueLevel");
            exactMeleeLevel = tag.GetInt("exactMeleeLevel");
            exactRangedLevel = tag.GetInt("exactRangedLevel");
            exactMagicLevel = tag.GetInt("exactMagicLevel");
            exactSummonLevel = tag.GetInt("exactSummonLevel");
            exactRogueLevel = tag.GetInt("exactRogueLevel");
        }

        public override void LoadLegacy(BinaryReader reader)
        {
            int loadVersion = reader.ReadInt32();
            stress = reader.ReadInt32();
            adrenaline = reader.ReadInt32();
            sCalDeathCount = reader.ReadInt32();
            sCalKillCount = reader.ReadInt32();
            deathCount = reader.ReadInt32();
            moneyStolenByBandit = reader.ReadInt32();
            reforges = reader.ReadInt32();

            meleeLevel = reader.ReadInt32();
            rangedLevel = reader.ReadInt32();
            magicLevel = reader.ReadInt32();
            summonLevel = reader.ReadInt32();
            rogueLevel = reader.ReadInt32();
            exactMeleeLevel = reader.ReadInt32();
            exactRangedLevel = reader.ReadInt32();
            exactMagicLevel = reader.ReadInt32();
            exactSummonLevel = reader.ReadInt32();
            exactRogueLevel = reader.ReadInt32();

            if (loadVersion == 0)
            {
                BitsByte flags = reader.ReadByte();
                extraAccessoryML = flags[0];
                eCore = flags[1];
                mFruit = flags[2];
                bOrange = flags[3];
                eBerry = flags[4];
                dFruit = flags[5];
                pHeart = flags[6];
                cShard = flags[7];

                BitsByte flags2 = reader.ReadByte();
                revJamDrop = flags2[0];
                rageBoostOne = flags2[1];
                rageBoostTwo = flags2[2];
                rageBoostThree = flags2[3];
                adrenalineBoostOne = flags2[4];
                adrenalineBoostTwo = flags2[5];
                adrenalineBoostThree = flags2[6];
                drawBossHPBar = flags2[7];

                BitsByte flags3 = reader.ReadByte();
                shouldDrawSmallText = flags3[0];
            }
            else
            {
                ModContent.GetInstance<CalamityMod>().Logger.Error("Unknown loadVersion: " + loadVersion);
            }
        }
        #endregion

        #region ResetEffects
        public override void ResetEffects()
        {
            if (extraAccessoryML)
            {
                player.extraAccessorySlots = 1;
            }
            if (extraAccessoryML && player.extraAccessory && (Main.expertMode || Main.gameMenu))
            {
                player.extraAccessorySlots = 2;
            }
            if (CalamityWorld.bossRushActive)
            {
                if (Config.BossRushAccessoryCurse)
                {
                    player.extraAccessorySlots = 0;
                }
            }

            ResetRogueStealth();

            throwingDamage = 1f;
            throwingVelocity = 1f;
            throwingCrit = 4;
            throwingAmmoCost75 = false;
            throwingAmmoCost66 = false;
            throwingAmmoCost50 = false;

            dashMod = 0;
            alcoholPoisonLevel = 0;

            thirdSage = false;
            if (player.immuneTime == 0)
                thirdSageH = false;

            perfmini = false;
            akato = false;
            leviPet = false;
            sirenPet = false;
            fox = false;
            chibii = false;
            brimling = false;
            bearPet = false;
            kendra = false;
            trashMan = false;
            astrophage = false;
            babyGhostBell = false;
            radiator = false;
            onyxExcavator = false;
            angryDog = false;
            fab = false;
            crysthamyr = false;

            abyssalDivingSuitPlates = false;
            abyssalDivingSuitCooldown = false;

            sirenWaterBuff = false;
            sirenIce = false;
            sirenIceCooldown = false;

            draedonsHeart = false;
            draedonsStressGain = false;

            afflicted = false;
            affliction = false;

            fasterMeleeLevel = false;
            fasterRangedLevel = false;
            fasterMagicLevel = false;
            fasterSummonLevel = false;
            fasterRogueLevel = false;

            dodgeScarf = false;
            scarfCooldown = false;

            elysianAegis = false;

            nCore = false;

            godSlayer = false;
            godSlayerDamage = false;
            godSlayerMage = false;
            godSlayerRanged = false;
            godSlayerThrowing = false;
            godSlayerSummon = false;
            godSlayerReflect = false;
            godSlayerCooldown = false;

            silvaSet = false;
            silvaMelee = false;
            silvaRanged = false;
            silvaThrowing = false;
            silvaMage = false;
            silvaSummon = false;

            auricSet = false;
            auricBoost = false;

            omegaBlueChestplate = false;
            omegaBlueSet = false;
            omegaBlueHentai = false;

            molluskSet = false;

            ataxiaBolt = false;
            ataxiaGeyser = false;
            ataxiaFire = false;
            ataxiaVolley = false;
            ataxiaBlaze = false;
            ataxiaMage = false;

            shadeRegen = false;

            flamethrowerBoost = false;

            shadowSpeed = false;
            dsSetBonus = false;
            wearingRogueArmor = false;

            desertScourgeLore = false;
            eaterOfWorldsLore = false;
            hiveMindLore = false;
            perforatorLore = false;
            queenBeeLore = false;
            skeletronLore = false;
            wallOfFleshLore = false;
            destroyerLore = false;
            aquaticScourgeLore = false;
            skeletronPrimeLore = false;
            brimstoneElementalLore = false;
            calamitasLore = false;
            planteraLore = false;
            leviathanAndSirenLore = false;
            astrumAureusLore = false;
            astrumDeusLore = false;
            golemLore = false;
            plaguebringerGoliathLore = false;
            dukeFishronLore = false;
            ravagerLore = false;
            lunaticCultistLore = false;
            moonLordLore = false;
            providenceLore = false;
            polterghastLore = false;
            DoGLore = false;
            yharonLore = false;
            SCalLore = false;

            luxorsGift = false;
            fungalSymbiote = false;
            trinketOfChi = false;
            gladiatorSword = false;
            unstablePrism = false;
            regenator = false;
            deepDiver = false;
            theBee = false;
			alluringBait = false;
			enchantedPearl = false;
			fishingStation = false;
            rBrain = false;
            bloodyWormTooth = false;
            rampartOfDeities = false;
            vexation = false;
            fBulwark = false;
            badgeOfBravery = false;
            badgeOfBraveryRare = false;
            aSpark = false;
            aSparkRare = false;
            aBulwark = false;
            aBulwarkRare = false;
            dAmulet = false;
            fCarapace = false;
            gShell = false;
            seaShell = false;
            absorber = false;
            aAmpoule = false;
            pAmulet = false;
            fBarrier = false;
            aBrain = false;
            frostFlare = false;
            beeResist = false;
            uberBees = false;
            projRef = false;
            projRefRare = false;
            nanotech = false;
            eQuiver = false;
            cryogenSoul = false;
            yInsignia = false;
            eGauntlet = false;
            eTalisman = false;
            statisBeltOfCurses = false;
            heartOfDarkness = false;
            shadowMinions = false;
            tearMinions = false;
            alchFlask = false;
            community = false;
            stressPills = false;
            laudanum = false;
            fleshTotem = false;
            bloodPact = false;
            bloodflareCore = false;
            coreOfTheBloodGod = false;
            elementalHeart = false;
            crownJewel = false;
            celestialJewel = false;
            astralArcanum = false;
            harpyRing = false;
            darkSunRing = false;
            calamityRing = false;
            eArtifact = false;
            dArtifact = false;
            gArtifact = false;
            pArtifact = false;
            giantPearl = false;
            normalityRelocator = false;
            fabledTortoise = false;
            manaOverloader = false;
            royalGel = false;
            lol = false;
            raiderTalisman = false;
            sGenerator = false;
            sDefense = false;
            sRegen = false;
            sPower = false;
            IBoots = false;
            elysianFire = false;
            sTracers = false;
            eTracers = false;
            cTracers = false;
            oldDie = false;
            ursaSergeant = false;
            thiefsDime = false;
            dynamoStemCells = false;
            etherealExtorter = false;
            //dukeScales = false;

            daedalusReflect = false;
            daedalusSplit = false;
            daedalusAbsorb = false;
            daedalusShard = false;

            reaverSpore = false;
            reaverDoubleTap = false;
            reaverBlast = false;
            reaverBurst = false;

            ironBoots = false;
            depthCharm = false;
            anechoicPlating = false;
            jellyfishNecklace = false;
            abyssalAmulet = false;
            lumenousAmulet = false;
            reaperToothNecklace = false;
            aquaticEmblem = false;

            astralStarRain = false;

            victideSet = false;

            aeroSet = false;

            statigelSet = false;

            umbraphileSet = false;

            tarraSet = false;
            tarraMelee = false;
			tarragonCloak = false;
			tarragonCloakCooldown = false;
            tarraMage = false;
            tarraRanged = false;
            tarraThrowing = false;
			tarragonImmunity = false;
			tarragonImmunityCooldown = false;
            tarraSummon = false;

            bloodflareSet = false;
            bloodflareMelee = false;
			bloodflareFrenzy = false;
			bloodFrenzyCooldown = false;
			bloodflareRanged = false;
			bloodflareSoulCooldown = false;
			bloodflareThrowing = false;
            bloodflareMage = false;
            bloodflareSummon = false;

            xerocSet = false;

            weakPetrification = false;

            inkBomb = false;
            inkBombCooldown = false;
            darkGodSheath = false;
            abyssalMirror = false;
            abyssalMirrorCooldown = false;
            eclipseMirror = false;
            eclipseMirrorCooldown = false;
            featherCrown = false;
            moonCrown = false;
            dragonScales = false;
            gloveOfPrecision = false;
            gloveOfRecklessness = false;
            momentumCapacitor = false;
            vampiricTalisman = false;
            electricianGlove = false;
            bloodyGlove = false;
            filthyGlove = false;
            sandCloak = false;
            spectralVeil = false;
            plaguedFuelPack = false;

			alcoholPoisoning = false;
            shadowflame = false;
            wDeath = false;
            lethalLavaBurn = false;
            aCrunch = false;
            hAttack = false;
            horror = false;
            irradiated = false;
            bFlames = false;
            aFlames = false;
            gsInferno = false;
            astralInfection = false;
            pFlames = false;
            hFlames = false;
            hInferno = false;
            gState = false;
            bBlood = false;
            eGravity = false;
            vHex = false;
            eGrav = false;
            warped = false;
            marked = false;
            cDepth = false;
            fishAlert = false;
            bOut = false;
            clamity = false;
            enraged = false;
            snowmanNoseless = false;
            sulphurPoison = false;

			revivify = false;
            trinketOfChiBuff = false;
            corrEffigy = false;
            crimEffigy = false;
            rRage = false;
            xRage = false;
            xWrath = false;
            graxDefense = false;
            sMeleeBoost = false;
            tFury = false;
            cadence = false;
            omniscience = false;
            zerg = false;
            zen = false;
			bossZen = false;
            permafrostsConcoction = false;
            armorCrumbling = false;
            armorShattering = false;
            ceaselessHunger = false;
            calcium = false;
            soaring = false;
            bounding = false;
            triumph = false;
            penumbra = false;
            photosynthesis = false;
            astralInjection = false;
            gravityNormalizer = false;
            holyWrath = false;
            profanedRage = false;
            draconicSurge = false;
            trippy = false;
            amidiasBlessing = false;
            yPower = false;
            aWeapon = false;
            tScale = false;
            fabsolVodka = false;
            invincible = false;
            shine = false;
            anechoicCoating = false;
            mushy = false;
            molten = false;
            shellBoost = false;
            cFreeze = false;
            tRegen = false;
            polarisBoost = false;
            polarisBoostTwo = false;
            polarisBoostThree = false;
            bloodfinBoost = false;

            killSpikyBalls = false;

            vodka = false;
            redWine = false;
            grapeBeer = false;
            moonshine = false;
            rum = false;
            whiskey = false;
            fireball = false;
            everclear = false;
            bloodyMary = false;
            tequila = false;
            caribbeanRum = false;
            cinnamonRoll = false;
            tequilaSunrise = false;
            margarita = false;
            starBeamRye = false;
            screwdriver = false;
            moscowMule = false;
            whiteWine = false;
            evergreenGin = false;

            tranquilityCandle = false;
            chaosCandle = false;
            purpleCandle = false;
            blueCandle = false;
            pinkCandle = false;
            yellowCandle = false;

            resButterfly = false;
            glSword = false;
            mWorm = false;
            iClasper = false;
            herring = false;
            blackhawk = false;
            calamari = false;
            cEyes = false;
            cSlime = false;
            cSlime2 = false;
            bStar = false;
            aStar = false;
            SP = false;
            dCreeper = false;
            bClot = false;
            eAxe = false;
            SPG = false;
            aChicken = false;
            cLamp = false;
            pGuy = false;
            cEnergy = false;
            gDefense = false;
            gOffense = false;
            gHealer = false;
            sWaifu = false;
            dWaifu = false;
            cWaifu = false;
            bWaifu = false;
            slWaifu = false;
            fClump = false;
            rDevil = false;
            aValkyrie = false;
            sCrystal = false;
            sGod = false;
            sandnado = false;
            vUrchin = false;
            cSpirit = false;
            rOrb = false;
            dCrystal = false;
            sandWaifu = false;
            sandBoobWaifu = false;
            cloudWaifu = false;
            brimstoneWaifu = false;
            sirenWaifu = false;
            allWaifus = false;
            fungalClump = false;
            redDevil = false;
            valkyrie = false;
            slimeGod = false;
            urchin = false;
            chaosSpirit = false;
            reaverOrb = false;
            daedalusCrystal = false;
            shellfish = false;
            hCrab = false;
            tDime = false;

            abyssalDivingSuitPrevious = abyssalDivingSuit;
            abyssalDivingSuit = abyssalDivingSuitHide = abyssalDivingSuitForce = abyssalDivingSuitPower = false;

            sirenBoobsPrevious = sirenBoobs;
            sirenBoobs = sirenBoobsHide = sirenBoobsForce = sirenBoobsPower = false;
            sirenBoobsAltPrevious = sirenBoobsAlt;
            sirenBoobsAlt = sirenBoobsAltHide = sirenBoobsAltForce = sirenBoobsAltPower = false;

            snowmanPrevious = snowman;
            snowman = snowmanHide = snowmanForce = snowmanPower = false;

            rageMode = false;
            adrenalineMode = false;
        }
        #endregion

        #region UpdateDead
        public override void UpdateDead()
        {
            #region Debuffs
            //distanceFromBoss = -1;
            gaelRageCooldown = 0;
            gaelSwipes = 0;
            gaelSwitchTimer = (GaelSwitchPhase)0;
            stress = 0;
            adrenaline = 0;
            adrenalineMaxTimer = 300;
            adrenalineDmgDown = 600;
            adrenalineDmgMult = 1f;
            raiderStack = 0;
            fleshTotemCooldown = 0;
            astralStarRainCooldown = 0;
            bloodflareMageCooldown = 0;
            tarraMageHealCooldown = 0;
            bossRushImmunityFrameCurseTimer = 0;
            aBulwarkRareMeleeBoostTimer = 0;
            theBeeDamage = 0;
            reforges = 0;
            polarisBoostCounter = 0;
			killSpikyBalls = false;

			alcoholPoisoning = false;
            shadowflame = false;
            wDeath = false;
            lethalLavaBurn = false;
            aCrunch = false;
            hAttack = false;
            horror = false;
            irradiated = false;
            bFlames = false;
            aFlames = false;
            gsInferno = false;
            astralInfection = false;
            pFlames = false;
            hFlames = false;
            hInferno = false;
            gState = false;
            bBlood = false;
            eGravity = false;
            vHex = false;
            eGrav = false;
            warped = false;
            marked = false;
            cDepth = false;
            fishAlert = false;
            bOut = false;
            clamity = false;
            snowmanNoseless = false;
            scarfCooldown = false;
            godSlayerCooldown = false;
            abyssalDivingSuitCooldown = false;
            abyssalDivingSuitPlateHits = 0;
            sirenIceCooldown = false;
            inkBombCooldown = false;
            abyssalMirrorCooldown = false;
            eclipseMirrorCooldown = false;
            moonCrownCooldown = 0;
            featherCrownCooldown = 0;
            sulphurPoison = false;
            sandCloakCooldown = 0;
            spectralVeilImmunity = 0;
            plaguedFuelPackCooldown = 0;
            plaguedFuelPackDash = 0;
            plaguedFuelPackDirection = 0;
            #endregion

            #region Rogue
            // Stealth
            rogueStealth = 0f;
            rogueStealthMax = 0f;

            throwingDamage = 1f;
            throwingVelocity = 1f;
            throwingCrit = 4;
            throwingAmmoCost75 = false;
            throwingAmmoCost66 = false;
            throwingAmmoCost50 = false;
            #endregion

            #region Buffs
            sDefense = false;
            sRegen = false;
            sPower = false;
            onyxExcavator = false;
            angryDog = false;
            fab = false;
            crysthamyr = false;
            abyssalDivingSuitPlates = false;
            sirenWaterBuff = false;
            sirenIce = false;
            trinketOfChiBuff = false;
            chiBuffTimer = 0;
            corrEffigy = false;
            crimEffigy = false;
            rRage = false;
            xRage = false;
            xWrath = false;
            graxDefense = false;
            sMeleeBoost = false;
            tFury = false;
            cadence = false;
            omniscience = false;
            zerg = false;
            zen = false;
			bossZen = false;
            permafrostsConcoction = false;
            armorCrumbling = false;
            armorShattering = false;
            ceaselessHunger = false;
            calcium = false;
            soaring = false;
            bounding = false;
            triumph = false;
            penumbra = false;
            photosynthesis = false;
            astralInjection = false;
            gravityNormalizer = false;
            holyWrath = false;
            profanedRage = false;
            draconicSurge = false;
            draconicSurgeCooldown = 0;
            yPower = false;
            aWeapon = false;
            tScale = false;
            fabsolVodka = false;
            invincible = false;
            shine = false;
            anechoicCoating = false;
            mushy = false;
            molten = false;
            enraged = false;
            shellBoost = false;
            cFreeze = false;
            tRegen = false;
            rageMode = false;
            adrenalineMode = false;
            vodka = false;
            redWine = false;
            grapeBeer = false;
            moonshine = false;
            rum = false;
            whiskey = false;
            fireball = false;
            everclear = false;
            bloodyMary = false;
            tequila = false;
            caribbeanRum = false;
            cinnamonRoll = false;
            tequilaSunrise = false;
            margarita = false;
            starBeamRye = false;
            screwdriver = false;
            moscowMule = false;
            whiteWine = false;
            evergreenGin = false;
            tranquilityCandle = false;
            chaosCandle = false;
            purpleCandle = false;
            blueCandle = false;
            pinkCandle = false;
            pinkCandleHealFraction = 0D;
            yellowCandle = false;
            trippy = false;
            amidiasBlessing = false;
            polarisBoost = false;
            polarisBoostTwo = false;
            polarisBoostThree = false;
            bloodfinBoost = false;
            bloodfinTimer = 0;
            revivify = false;
            healCounter = 300;
            #endregion

            #region Armorbonuses
            flamethrowerBoost = false;
            shadowSpeed = false;
            godSlayer = false;
            godSlayerDamage = false;
            godSlayerMage = false;
            godSlayerRanged = false;
            godSlayerThrowing = false;
            godSlayerSummon = false;
            godSlayerReflect = false;
            auricBoost = false;
            silvaSet = false;
            silvaMelee = false;
            silvaRanged = false;
            silvaThrowing = false;
            silvaMage = false;
            silvaSummon = false;
            hasSilvaEffect = false;
            silvaCountdown = 600;
            silvaHitCounter = 0;
            auricSet = false;
            omegaBlueChestplate = false;
            omegaBlueSet = false;
            omegaBlueCooldown = 0;
            molluskSet = false;
            daedalusReflect = false;
            daedalusSplit = false;
            daedalusAbsorb = false;
            daedalusShard = false;
            reaverSpore = false;
            reaverDoubleTap = false;
            shadeRegen = false;
            dsSetBonus = false;
            umbraphileSet = false;
            reaverBlast = false;
            reaverBurst = false;
            astralStarRain = false;
            ataxiaMage = false;
            ataxiaBolt = false;
            ataxiaGeyser = false;
            ataxiaFire = false;
            ataxiaVolley = false;
            ataxiaBlaze = false;
            victideSet = false;
            aeroSet = false;
            statigelSet = false;
            tarraSet = false;
            tarraMelee = false;
			tarragonCloak = false;
			tarragonCloakCooldown = false;
			tarraDefenseTime = 600;
            tarraMage = false;
            tarraRanged = false;
            tarraThrowing = false;
			tarragonImmunity = false;
			tarragonImmunityCooldown = false;
            tarraThrowingCrits = 0;
            tarraSummon = false;
            bloodflareSet = false;
            bloodflareMelee = false;
			bloodflareFrenzy = false;
			bloodFrenzyCooldown = false;
			bloodflareMeleeHits = 0;
            bloodflareRanged = false;
			bloodflareSoulCooldown = false;
            bloodflareThrowing = false;
            bloodflareMage = false;
            bloodflareSummon = false;
            bloodflareSummonTimer = 0;
            xerocSet = false;
            IBoots = false;
            elysianFire = false;
            elysianAegis = false;
            elysianGuard = false;
            #endregion


            if (CalamityWorld.bossRushActive)
            {
                if (!CalamityGlobalNPC.AnyLivingPlayers())
                {
                    CalamityWorld.bossRushActive = false;
                    CalamityWorld.bossRushStage = 0;
                    CalamityMod.UpdateServerBoolean();
                    if (Main.netMode == NetmodeID.Server)
                    {
                        var netMessage = mod.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.BossRushStage);
                        netMessage.Write(CalamityWorld.bossRushStage);
                        netMessage.Send();
                    }
                    for (int doom = 0; doom < 200; doom++)
                    {
                        if (Main.npc[doom].active && Main.npc[doom].boss)
                        {
                            Main.npc[doom].active = false;
                            Main.npc[doom].netUpdate = true;
                        }
                    }
                }
            }
            if (CalamityWorld.armageddon && !areThereAnyDamnBosses)
            {
                player.respawnTimer -= 5;
            }
            else if (player.respawnTimer > 300 && Main.expertMode) //600 normal 900 expert
            {
                player.respawnTimer--;
            }
        }
        #endregion

        #region BiomeStuff
        public override void UpdateBiomeVisuals()
        {
            bool useNebula = NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsHead>());
            player.ManageSpecialBiomeVisuals("CalamityMod:DevourerofGodsHead", useNebula);

            bool useNebulaS = NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsHeadS>());
            player.ManageSpecialBiomeVisuals("CalamityMod:DevourerofGodsHeadS", useNebulaS);

            bool useBrimstone = NPC.AnyNPCs(ModContent.NPCType<CalamitasRun3>());
            player.ManageSpecialBiomeVisuals("CalamityMod:CalamitasRun3", useBrimstone);

            bool usePlague = NPC.AnyNPCs(ModContent.NPCType<PlaguebringerGoliath>());
            player.ManageSpecialBiomeVisuals("CalamityMod:PlaguebringerGoliath", usePlague);

            Point point = player.Center.ToTileCoordinates();
            bool aboveGround = point.Y > Main.maxTilesY - 320;
            bool overworld = player.ZoneOverworldHeight && (point.X < 380 || point.X > Main.maxTilesX - 380);
            bool useFire = NPC.AnyNPCs(ModContent.NPCType<Yharon>());
            player.ManageSpecialBiomeVisuals("CalamityMod:Yharon", useFire);
            player.ManageSpecialBiomeVisuals("HeatDistortion", Main.UseHeatDistortion && (useFire || trippy ||
                aboveGround || ((double)point.Y < Main.worldSurface && player.ZoneDesert && !overworld && !Main.raining && !Filters.Scene["Sandstorm"].IsActive())));

            bool useWater = NPC.AnyNPCs(ModContent.NPCType<Leviathan>());
            player.ManageSpecialBiomeVisuals("CalamityMod:Leviathan", useWater);

            bool useHoly = NPC.AnyNPCs(ModContent.NPCType<Providence>());
            player.ManageSpecialBiomeVisuals("CalamityMod:Providence", useHoly);

            bool useSBrimstone = NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>());
            player.ManageSpecialBiomeVisuals("CalamityMod:SupremeCalamitas", useSBrimstone);

            bool inAstral = ZoneAstral;
            player.ManageSpecialBiomeVisuals("CalamityMod:Astral", inAstral);
        }

        public override void UpdateBiomes()
        {
            Point point = player.Center.ToTileCoordinates();
            ZoneCalamity = CalamityWorld.calamityTiles > 50;
            ZoneAstral = !player.ZoneDungeon && (CalamityWorld.astralTiles > 950 || (player.ZoneSnow && CalamityWorld.astralTiles > 300));
            ZoneSunkenSea = CalamityWorld.sunkenSeaTiles > 150;

            int x = Main.maxTilesX;
            int y = Main.maxTilesY;
            int genLimit = x / 2;
            int abyssChasmY = y - 250;
            int abyssChasmX = CalamityWorld.abyssSide ? genLimit - (genLimit - 135) : genLimit + (genLimit - 135);

            bool abyssPosX = false;
            bool sulphurPosX = false;
            bool abyssPosY = point.Y <= abyssChasmY;
            if (CalamityWorld.abyssSide)
            {
                if (point.X < 380)
                {
                    sulphurPosX = true;
                }
                if (point.X < abyssChasmX + 80)
                {
                    abyssPosX = true;
                }
            }
            else
            {
                if (point.X > Main.maxTilesX - 380)
                {
                    sulphurPosX = true;
                }
                if (point.X > abyssChasmX - 80)
                {
                    abyssPosX = true;
                }
            }

            ZoneAbyss = ((double)point.Y > (Main.rockLayer - (double)y * 0.05)) &&
                !player.lavaWet &&
                !player.honeyWet &&
                abyssPosY &&
                abyssPosX;

            ZoneAbyssLayer1 = ZoneAbyss &&
                (double)point.Y <= (Main.rockLayer + (double)y * 0.03);

            ZoneAbyssLayer2 = ZoneAbyss &&
                (double)point.Y > (Main.rockLayer + (double)y * 0.03) &&
                (double)point.Y <= (Main.rockLayer + (double)y * 0.14);

            ZoneAbyssLayer3 = ZoneAbyss &&
                (double)point.Y > (Main.rockLayer + (double)y * 0.14) &&
                (double)point.Y <= (Main.rockLayer + (double)y * 0.26);

            ZoneAbyssLayer4 = ZoneAbyss &&
                (double)point.Y > (Main.rockLayer + (double)y * 0.26);

            ZoneSulphur = (CalamityWorld.sulphurTiles > 30 || (player.ZoneOverworldHeight && sulphurPosX)) && !ZoneAbyss;
        }

        public override bool CustomBiomesMatch(Player other)
        {
            CalamityPlayer modOther = other.Calamity();

            // least common biomes checked first so it short circuits as rapidly as possible
            return ZoneAbyssLayer4 == modOther.ZoneAbyssLayer4 &&
                ZoneAbyssLayer3 == modOther.ZoneAbyssLayer3 &&
                ZoneAbyssLayer2 == modOther.ZoneAbyssLayer2 &&
                ZoneAbyssLayer1 == modOther.ZoneAbyssLayer1 &&
                ZoneCalamity == modOther.ZoneCalamity &&
                ZoneSulphur == modOther.ZoneSulphur &&
                ZoneSunkenSea == modOther.ZoneSunkenSea &&
                ZoneAbyss == modOther.ZoneAbyss &&
                ZoneAstral == modOther.ZoneAstral;
        }

        public override void CopyCustomBiomesTo(Player other)
        {
            CalamityPlayer modOther = other.Calamity();
            modOther.ZoneCalamity = ZoneCalamity;
            modOther.ZoneAstral = ZoneAstral;
            modOther.ZoneSulphur = ZoneSulphur;
            modOther.ZoneSunkenSea = ZoneSunkenSea;
            modOther.ZoneAbyss = ZoneAbyss;
            modOther.ZoneAbyssLayer1 = ZoneAbyssLayer1;
            modOther.ZoneAbyssLayer2 = ZoneAbyssLayer2;
            modOther.ZoneAbyssLayer3 = ZoneAbyssLayer3;
            modOther.ZoneAbyssLayer4 = ZoneAbyssLayer4;
        }

        public override void SendCustomBiomes(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = ZoneCalamity;
            flags[1] = ZoneAstral;
            flags[2] = ZoneAbyss;
            flags[3] = ZoneAbyssLayer1;
            flags[4] = ZoneAbyssLayer2;
            flags[5] = ZoneAbyssLayer3;
            flags[6] = ZoneAbyssLayer4;
            flags[7] = ZoneSulphur;

            BitsByte flags2 = new BitsByte();
            flags2[0] = ZoneSunkenSea;

            writer.Write(flags);
            writer.Write(flags2);
        }

        public override void ReceiveCustomBiomes(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            ZoneCalamity = flags[0];
            ZoneAstral = flags[1];
            ZoneAbyss = flags[2];
            ZoneAbyssLayer1 = flags[3];
            ZoneAbyssLayer2 = flags[4];
            ZoneAbyssLayer3 = flags[5];
            ZoneAbyssLayer4 = flags[6];
            ZoneSulphur = flags[7];

            BitsByte flags2 = reader.ReadByte();
            ZoneSunkenSea = flags2[0];
        }

        public override Texture2D GetMapBackgroundImage()
        {
            if (ZoneSulphur)
            {
                return ModContent.GetTexture("CalamityMod/Backgrounds/MapBackgrounds/SulphurBG");
            }
            if (ZoneAstral)
            {
                return ModContent.GetTexture("CalamityMod/Backgrounds/MapBackgrounds/AstralBG");
            }
            return null;
        }
        #endregion

        #region InventoryStartup
        public override void SetupStartInventory(IList<Item> items, bool mediumcoreDeath)
        {
            if (!mediumcoreDeath)
            {
                player.inventory[9].SetDefaults(ModContent.ItemType<Revenge>());
                player.inventory[8].SetDefaults(ModContent.ItemType<IronHeart>());
                player.inventory[7].SetDefaults(ModContent.ItemType<StarterBag>());
            }
        }
        #endregion

        #region Life Regen
        public override void UpdateBadLifeRegen()
        {
            CalamityPlayerLifeRegen.CalamityUpdateBadLifeRegen(player, mod);
        }

        public override void UpdateLifeRegen()
        {
            CalamityPlayerLifeRegen.CalamityUpdateLifeRegen(player, mod);
        }
        #endregion

        #region HotKeys
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (CalamityMod.MomentumCapacitatorHotkey.JustPressed && momentumCapacitor && Main.myPlayer == player.whoAmI && player.Calamity().rogueStealth >= player.Calamity().rogueStealthMax * 0.3f &&
                wearingRogueArmor && player.Calamity().rogueStealthMax > 0 && CalamityUtils.CountProjectiles(ModContent.ProjectileType<MomentumCapacitorOrb>()) == 0)
            {
                player.Calamity().rogueStealth -= player.Calamity().rogueStealthMax * 0.3f;
                Vector2 fieldSpawnCenter = new Vector2(Main.mouseX, Main.mouseY) + Main.screenPosition;
                Projectile.NewProjectile(fieldSpawnCenter, Vector2.Zero, ModContent.ProjectileType<MomentumCapacitorOrb>(), 0, 0f, player.whoAmI, 0f, 0f);
            }
            if (CalamityMod.NormalityRelocatorHotKey.JustPressed && normalityRelocator && Main.myPlayer == player.whoAmI)
            {
                Vector2 teleportLocation;
                teleportLocation.X = (float)Main.mouseX + Main.screenPosition.X;
                if (player.gravDir == 1f)
                {
                    teleportLocation.Y = (float)Main.mouseY + Main.screenPosition.Y - (float)player.height;
                }
                else
                {
                    teleportLocation.Y = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY;
                }
                teleportLocation.X -= (float)(player.width / 2);
                if (teleportLocation.X > 50f && teleportLocation.X < (float)(Main.maxTilesX * 16 - 50) && teleportLocation.Y > 50f && teleportLocation.Y < (float)(Main.maxTilesY * 16 - 50))
                {
                    int x = (int)(teleportLocation.X / 16f);
                    int y = (int)(teleportLocation.Y / 16f);
                    if (!Collision.SolidCollision(teleportLocation, player.width, player.height))
                    {
                        player.Teleport(teleportLocation, 4, 0);
                        NetMessage.SendData(65, -1, -1, null, 0, (float)player.whoAmI, teleportLocation.X, teleportLocation.Y, 1, 0, 0);
                        if (player.chaosState)
                        {
                            player.statLife -= player.statLifeMax2 / 4;
                            PlayerDeathReason damageSource = PlayerDeathReason.ByOther(13);
                            if (Main.rand.NextBool(2))
                            {
                                damageSource = PlayerDeathReason.ByOther(player.Male ? 14 : 15);
                            }
                            if (player.statLife <= 0)
                            {
                                player.KillMe(damageSource, 1.0, 0, false);
                            }
                            player.lifeRegenCount = 0;
                            player.lifeRegenTime = 0;
                        }
                        player.AddBuff(BuffID.ChaosState, 360, true);
                    }
                }
            }
            if (CalamityMod.SandCloakHotkey.JustPressed && sandCloak && Main.myPlayer == player.whoAmI && player.Calamity().rogueStealth >= player.Calamity().rogueStealthMax * 0.25f &&
                wearingRogueArmor && player.Calamity().rogueStealthMax > 0 && sandCloakCooldown == 0)
            {
                sandCloakCooldown = 900;
                player.Calamity().rogueStealth -= player.Calamity().rogueStealthMax * 0.25f;
                Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<SandCloakVeil>(), 7, 8, player.whoAmI, 0, 0);
                Main.PlaySound(2, player.position, 45);
            }
            if (CalamityMod.SpectralVeilHotKey.JustPressed && spectralVeil && Main.myPlayer == player.whoAmI && player.Calamity().rogueStealth >= player.Calamity().rogueStealthMax * 0.25f &&
                wearingRogueArmor && player.Calamity().rogueStealthMax > 0)
            {
                float teleportRange = 320f;
                Vector2 teleportLocation;
                teleportLocation.X = (float)Main.mouseX + Main.screenPosition.X;
                if (player.gravDir == 1f)
                {
                    teleportLocation.Y = (float)Main.mouseY + Main.screenPosition.Y - (float)player.height;
                }
                else
                {
                    teleportLocation.Y = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY;
                }
                teleportLocation.X -= (float)(player.width / 2);
                Vector2 playerToTeleport = teleportLocation - player.position;
                if (playerToTeleport.Length() > teleportRange)
                {
                    playerToTeleport.Normalize();
                    playerToTeleport *= teleportRange;
                    teleportLocation = player.position + playerToTeleport;
                }
                if (teleportLocation.X > 50f && teleportLocation.X < (float)(Main.maxTilesX * 16 - 50) && teleportLocation.Y > 50f && teleportLocation.Y < (float)(Main.maxTilesY * 16 - 50))
                {
                    int x = (int)(teleportLocation.X / 16f);
                    int y = (int)(teleportLocation.Y / 16f);
                    if (!Collision.SolidCollision(teleportLocation, player.width, player.height))
                    {
                        player.Calamity().rogueStealth -= player.Calamity().rogueStealthMax * 0.25f;

                        player.Teleport(teleportLocation, 1, 0);
                        NetMessage.SendData(65, -1, -1, null, 0, (float)player.whoAmI, teleportLocation.X, teleportLocation.Y, 1, 0, 0);
                        if (player.chaosState)
                        {
                            player.statLife -= player.statLifeMax2 / 7;
                            PlayerDeathReason damageSource = PlayerDeathReason.ByOther(13);
                            if (player.statLife <= 0)
                            {
                                player.KillMe(damageSource, 1.0, 0, false);
                            }
                        }
                        player.AddBuff(BuffID.ChaosState, 360, true);

                        int numDust = 40;
                        Vector2 step = playerToTeleport / numDust;
                        for (int i = 0; i < numDust; i++)
                        {
                            int dustIndex = Dust.NewDust(player.Center - (step * i), 1, 1, 21, step.X, step.Y);
                            Main.dust[dustIndex].noGravity = true;
                            Main.dust[dustIndex].noLight = true;
                        }

                        player.immune = true;
                        player.immuneTime = 120;
                        spectralVeilImmunity = 120;
                        for (int k = 0; k < player.hurtCooldowns.Length; k++)
                        {
                            player.hurtCooldowns[k] = player.immuneTime;
                        }
                    }
                }

            }
            if (CalamityMod.PlaguePackHotKey.JustPressed && plaguedFuelPack && Main.myPlayer == player.whoAmI && player.Calamity().rogueStealth >= player.Calamity().rogueStealthMax * 0.25f &&
                wearingRogueArmor && player.Calamity().rogueStealthMax > 0 && plaguedFuelPackCooldown == 0 && !player.mount.Active)
            {
                plaguedFuelPackCooldown = 90;
                plaguedFuelPackDash = 10;
                plaguedFuelPackDirection = player.direction;
                player.Calamity().rogueStealth -= player.Calamity().rogueStealthMax * 0.25f;
                Main.PlaySound(2, player.position, 66);
                Main.PlaySound(2, player.position, 34);
            }
            if (CalamityMod.BossBarToggleHotKey.JustPressed)
            {
                if (drawBossHPBar)
                {
                    drawBossHPBar = false;
                }
                else
                {
                    drawBossHPBar = true;
                }
            }
            if (CalamityMod.BossBarToggleSmallTextHotKey.JustPressed)
            {
                if (shouldDrawSmallText)
                {
                    shouldDrawSmallText = false;
                }
                else
                {
                    shouldDrawSmallText = true;
                }
            }
            if (CalamityMod.TarraHotKey.JustPressed)
            {
                if (tarraMelee && !tarragonCloakCooldown && !tarragonCloak)
                {
					if (player.whoAmI == Main.myPlayer)
					{
						player.AddBuff(ModContent.BuffType<TarragonCloak>(), 602, false);
					}
				}
                if (bloodflareRanged && !bloodflareSoulCooldown)
                {
					if (player.whoAmI == Main.myPlayer)
					{
						player.AddBuff(ModContent.BuffType<BloodflareSoulCooldown>(), 1800, false);
					}
					Main.PlaySound(29, (int)player.position.X, (int)player.position.Y, 104);
                    for (int num502 = 0; num502 < 64; num502++)
                    {
                        int dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 16f), player.width, player.height - 16, 60, 0f, 0f, 0, default, 1f);
                        Main.dust[dust].velocity *= 3f;
                        Main.dust[dust].scale *= 1.15f;
                    }
                    int num226 = 36;
                    for (int num227 = 0; num227 < num226; num227++)
                    {
                        Vector2 vector6 = Vector2.Normalize(player.velocity) * new Vector2((float)player.width / 2f, (float)player.height) * 0.75f;
                        vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + player.Center;
                        Vector2 vector7 = vector6 - player.Center;
                        int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 60, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default, 1.4f);
                        Main.dust[num228].noGravity = true;
                        Main.dust[num228].noLight = true;
                        Main.dust[num228].velocity = vector7;
                    }
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    int i;
                    int damage = 800;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (i = 0; i < 8; i++)
                        {
                            float ai1 = Main.rand.NextFloat() + 0.5f;
                            float randomSpeed = (float)Main.rand.Next(1, 7);
                            float randomSpeed2 = (float)Main.rand.Next(1, 7);
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f) + randomSpeed, ModContent.ProjectileType<BloodflareSoul>(), damage, 0f, player.whoAmI, 0f, ai1);
                            Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f) + randomSpeed2, ModContent.ProjectileType<BloodflareSoul>(), damage, 0f, player.whoAmI, 0f, ai1);
                        }
                    }
                }
                if (omegaBlueSet && omegaBlueCooldown <= 0)
                {
					if (player.whoAmI == Main.myPlayer)
					{
						player.AddBuff(ModContent.BuffType<AbyssalMadness>(), 300, false);
					}
                    omegaBlueCooldown = 1800;
                    Main.PlaySound(29, (int)player.position.X, (int)player.position.Y, 104);
                    for (int i = 0; i < 66; i++)
                    {
                        int d = Dust.NewDust(player.position, player.width, player.height, 20, 0, 0, 100, Color.Transparent, 2.6f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].fadeIn = 1f;
                        Main.dust[d].velocity *= 6.6f;
                    }
                }
                if (dsSetBonus)
                {
                    Main.PlaySound(29, (int)player.position.X, (int)player.position.Y, 104);
                    for (int num502 = 0; num502 < 36; num502++)
                    {
                        int dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 16f), player.width, player.height - 16, 235, 0f, 0f, 0, default, 1f);
                        Main.dust[dust].velocity *= 3f;
                        Main.dust[dust].scale *= 1.15f;
                    }
                    int num226 = 36;
                    for (int num227 = 0; num227 < num226; num227++)
                    {
                        Vector2 vector6 = Vector2.Normalize(player.velocity) * new Vector2((float)player.width / 2f, (float)player.height) * 0.75f;
                        vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + player.Center;
                        Vector2 vector7 = vector6 - player.Center;
                        int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 235, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default, 1.4f);
                        Main.dust[num228].noGravity = true;
                        Main.dust[num228].noLight = true;
                        Main.dust[num228].velocity = vector7;
                    }
                    if (player.whoAmI == Main.myPlayer)
                    {
                        player.AddBuff(ModContent.BuffType<Enraged>(), 600, false);
                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int l = 0; l < 200; l++)
                        {
                            NPC nPC = Main.npc[l];
                            if (nPC.active && !nPC.friendly && !nPC.dontTakeDamage && Vector2.Distance(player.Center, nPC.Center) <= 3000f)
                            {
                                nPC.AddBuff(ModContent.BuffType<Enraged>(), 600, false);
                            }
                        }
                    }
                }
            }
            if (CalamityMod.AstralArcanumUIHotkey.JustPressed && astralArcanum)
            {
                AstralArcanumUI.Toggle();
            }
            if (CalamityMod.AstralTeleportHotKey.JustPressed)
            {
                if (celestialJewel)
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        player.TeleportationPotion();
                        Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 6);
                    }
                    else if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
                    {
                        NetMessage.SendData(73, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
            }
            if (CalamityMod.AegisHotKey.JustPressed)
            {
                if (elysianAegis && !player.mount.Active)
                {
                    elysianGuard = !elysianGuard;
                }
            }
            if (CalamityMod.RageHotKey.JustPressed)
            {
                if (gaelRageCooldown == 0 && player.HeldItem.type == ModContent.ItemType<GaelsGreatsword>() &&
                    stress > 0)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SilvaDispel"), (int)player.position.X, (int)player.position.Y);
                    for (int i = 0; i < 3; i++)
                    {
                        Dust.NewDust(player.position, 120, 120, 218, 0f, 0f, 100, default, 1.5f);
                    }
                    for (int i = 0; i < 30; i++)
                    {
                        float angle = MathHelper.TwoPi * i / 30f;
                        int dustIndex = Dust.NewDust(player.position, 120, 120, 218, 0f, 0f, 0, default, 2f);
                        Main.dust[dustIndex].noGravity = true;
                        Main.dust[dustIndex].velocity *= 4f;
                        dustIndex = Dust.NewDust(player.position, 120, 120, 218, 0f, 0f, 100, default, 1f);
                        Main.dust[dustIndex].velocity *= 2.25f;
                        Main.dust[dustIndex].noGravity = true;
                        Dust.NewDust(player.Center + angle.ToRotationVector2() * 160f, 0, 0, 218, 0f, 0f, 100, default, 1f);
                    }
                    gaelRageCooldown = 60 * GaelsGreatsword.SkullsplosionCooldownSeconds;
                    float rageRatio = (float)stress / stressMax;
                    int damage = (int)(rageRatio * GaelsGreatsword.MaxRageBoost * GaelsGreatsword.BaseDamage);
                    float skullCount = 5f;
                    float skullSpeed = 5f;
                    if (CalamityWorld.downedYharon)
                    {
                        skullCount = 20f;
                        skullSpeed = 12f;
                    }
                    else if (NPC.downedMoonlord)
                    {
                        skullCount = 13f;
                        skullSpeed = 10f;
                    }
                    else if (Main.hardMode)
                    {
                        skullCount = 9f;
                        skullSpeed = 6.8f;
                    }
                    for (float i = 0; i < skullCount; i += 1f)
                    {
                        float angle = MathHelper.TwoPi * i / skullCount;
                        Vector2 initialVelocity = angle.ToRotationVector2().RotatedByRandom(MathHelper.ToRadians(12f)) * skullSpeed * new Vector2(0.82f, 1.5f) *
                            Main.rand.NextFloat(0.8f, 1.2f) * (i < skullCount / 2  ? 0.25f : 1f);
                        int projectileIndex = Projectile.NewProjectile(player.Center + initialVelocity * 3f, initialVelocity, ModContent.ProjectileType<GaelSkull2>(), damage, 2f, player.whoAmI);
                        Main.projectile[projectileIndex].tileCollide = false;
                        Main.projectile[projectileIndex].localAI[1] = (Main.projectile[projectileIndex].velocity.Y < 0f).ToInt();
                    }
                    stress = 0;
                }
                if (stress == stressMax && !rageMode)
                {
                    Main.PlaySound(29, (int)player.position.X, (int)player.position.Y, 104);
                    for (int num502 = 0; num502 < 64; num502++)
                    {
                        int dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 16f), player.width, player.height - 16, 235, 0f, 0f, 0, default, 1f);
                        Main.dust[dust].velocity *= 3f;
                        Main.dust[dust].scale *= 1.15f;
                    }
                    int num226 = 36;
                    for (int num227 = 0; num227 < num226; num227++)
                    {
                        Vector2 vector6 = Vector2.Normalize(player.velocity) * new Vector2((float)player.width / 2f, (float)player.height) * 0.75f;
                        vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + player.Center;
                        Vector2 vector7 = vector6 - player.Center;
                        int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 235, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default, 1.4f);
                        Main.dust[num228].noGravity = true;
                        Main.dust[num228].noLight = true;
                        Main.dust[num228].velocity = vector7;
                    }
                    player.AddBuff(ModContent.BuffType<RageMode>(), 300);
                }
            }
            if (CalamityMod.AdrenalineHotKey.JustPressed)
            {
                if (adrenaline == adrenalineMax && !adrenalineMode)
                {
                    Main.PlaySound(29, (int)player.position.X, (int)player.position.Y, 104);
                    for (int num502 = 0; num502 < 64; num502++)
                    {
                        int dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 16f), player.width, player.height - 16, 206, 0f, 0f, 0, default, 1f);
                        Main.dust[dust].velocity *= 3f;
                        Main.dust[dust].scale *= 2f;
                    }
                    int num226 = 36;
                    for (int num227 = 0; num227 < num226; num227++)
                    {
                        Vector2 vector6 = Vector2.Normalize(player.velocity) * new Vector2((float)player.width / 2f, (float)player.height) * 0.75f;
                        vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + player.Center;
                        Vector2 vector7 = vector6 - player.Center;
                        int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 206, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default, 1.4f);
                        Main.dust[num228].noGravity = true;
                        Main.dust[num228].noLight = true;
                        Main.dust[num228].velocity = vector7;
                    }
                    player.AddBuff(ModContent.BuffType<AdrenalineMode>(), 300);
                }
            }
        }
        #endregion

        #region TeleportMethods
        public void HandleTeleport(int teleportType, bool forceHandle = false, int whoAmI = 0)
        {
            bool syncData = forceHandle || Main.netMode == NetmodeID.SinglePlayer;
            if (syncData)
            {
                TeleportPlayer(teleportType, forceHandle, whoAmI);
            }
            else
            {
                SyncTeleport(teleportType);
            }
        }

        public static void TeleportPlayer(int teleportType, bool syncData = false, int whoAmI = 0)
        {
            Player player;
            if (!syncData)
            {
                player = Main.LocalPlayer;
            }
            else
            {
                player = Main.player[whoAmI];
            }
            switch (teleportType)
            {
                case 0:
                    UnderworldTeleport(player, syncData);
                    break;
                case 1:
                    DungeonTeleport(player, syncData);
                    break;
                case 2:
                    JungleTeleport(player, syncData);
                    break;
                default:
                    break;
            }
        }

        public void SyncTeleport(int teleportType)
        {
            ModPacket netMessage = mod.GetPacket();
            netMessage.Write((byte)CalamityModMessageType.TeleportPlayer);
            netMessage.Write(teleportType);
            netMessage.Send();
        }

        public static void UnderworldTeleport(Player player, bool syncData = false)
        {
            int teleportStartX = 100;
            int teleportRangeX = Main.maxTilesX - 200;
            int teleportStartY = Main.maxTilesY - 200;
            int teleportRangeY = 50;
            bool flag = false;
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            int width = player.width;
            Vector2 vector = new Vector2((float)num2, (float)num3) * 16f + new Vector2((float)(-(float)width / 2 + 8), (float)-(float)player.height);
            while (!flag && num < 1000)
            {
                num++;
                num2 = teleportStartX + Main.rand.Next(teleportRangeX);
                num3 = teleportStartY + Main.rand.Next(teleportRangeY);
                vector = new Vector2((float)num2, (float)num3) * 16f + new Vector2((float)(-(float)width / 2 + 8), (float)-(float)player.height);
                if (!Collision.SolidCollision(vector, width, player.height))
                {
                    if (Main.tile[num2, num3] == null)
                    {
                        Main.tile[num2, num3] = new Tile();
                    }
                    int i = 0;
                    while (i < 100)
                    {
                        if (Main.tile[num2, num3 + i] == null)
                        {
                            Main.tile[num2, num3 + i] = new Tile();
                        }
                        Tile tile = Main.tile[num2, num3 + i];
                        vector = new Vector2((float)num2, (float)(num3 + i)) * 16f + new Vector2((float)(-(float)width / 2 + 8), (float)-(float)player.height);
                        Vector4 vector2 = Collision.SlopeCollision(vector, player.velocity, width, player.height, player.gravDir, false);
                        bool arg_1FF_0 = !Collision.SolidCollision(vector, width, player.height);
                        if (vector2.Z == player.velocity.X && vector2.Y == player.velocity.Y && vector2.X == vector.X)
                        {
                            bool arg_1FE_0 = vector2.Y == vector.Y;
                        }
                        if (arg_1FF_0)
                        {
                            i++;
                        }
                        else
                        {
                            if (tile.active() && !tile.inActive() && Main.tileSolid[(int)tile.type])
                            {
                                break;
                            }
                            i++;
                        }
                    }
                    if (!Collision.LavaCollision(vector, width, player.height) && Collision.HurtTiles(vector, player.velocity, width, player.height, false).Y <= 0f)
                    {
                        Collision.SlopeCollision(vector, player.velocity, width, player.height, player.gravDir, false);
                        if (Collision.SolidCollision(vector, width, player.height) && i < 99)
                        {
                            Vector2 vector3 = Vector2.UnitX * 16f;
                            if (!(Collision.TileCollision(vector - vector3, vector3, player.width, player.height, false, false, (int)player.gravDir) != vector3))
                            {
                                vector3 = -Vector2.UnitX * 16f;
                                if (!(Collision.TileCollision(vector - vector3, vector3, player.width, player.height, false, false, (int)player.gravDir) != vector3))
                                {
                                    vector3 = Vector2.UnitY * 16f;
                                    if (!(Collision.TileCollision(vector - vector3, vector3, player.width, player.height, false, false, (int)player.gravDir) != vector3))
                                    {
                                        vector3 = -Vector2.UnitY * 16f;
                                        if (!(Collision.TileCollision(vector - vector3, vector3, player.width, player.height, false, false, (int)player.gravDir) != vector3))
                                        {
                                            flag = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (!flag)
            {
                return;
            }
            ModTeleport(player, vector, syncData, false);
        }

        public static void DungeonTeleport(Player player, bool syncData = false)
        {
            ModTeleport(player, new Vector2(Main.dungeonX, Main.dungeonY), syncData, true);
        }

        public static void JungleTeleport(Player player, bool syncData = false)
        {
            int teleportStartX = CalamityWorld.abyssSide ? (int)((double)Main.maxTilesX * 0.65) : (int)((double)Main.maxTilesX * 0.2);
            int teleportRangeX = (int)((double)Main.maxTilesX * 0.15);

            int teleportStartY = (int)Main.worldSurface - 75;
            int teleportRangeY = 50;

            bool flag = false;
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            int width = player.width;
            Vector2 vector = new Vector2((float)num2, (float)num3) * 16f + new Vector2((float)(-(float)width / 2 + 8), (float)-(float)player.height);
            while (!flag && num < 1000)
            {
                num++;
                num2 = teleportStartX + Main.rand.Next(teleportRangeX);
                num3 = teleportStartY + Main.rand.Next(teleportRangeY);
                vector = new Vector2((float)num2, (float)num3) * 16f + new Vector2((float)(-(float)width / 2 + 8), (float)-(float)player.height);
                if (!Collision.SolidCollision(vector, width, player.height))
                {
                    if (Main.tile[num2, num3] == null)
                    {
                        Main.tile[num2, num3] = new Tile();
                    }
                    int i = 0;
                    while (i < 100)
                    {
                        if (Main.tile[num2, num3 + i] == null)
                        {
                            Main.tile[num2, num3 + i] = new Tile();
                        }
                        Tile tile = Main.tile[num2, num3 + i];
                        vector = new Vector2((float)num2, (float)(num3 + i)) * 16f + new Vector2((float)(-(float)width / 2 + 8), (float)-(float)player.height);
                        Vector4 vector2 = Collision.SlopeCollision(vector, player.velocity, width, player.height, player.gravDir, false);
                        bool arg_1FF_0 = !Collision.SolidCollision(vector, width, player.height);
                        if (vector2.Z == player.velocity.X && vector2.Y == player.velocity.Y && vector2.X == vector.X)
                        {
                            bool arg_1FE_0 = vector2.Y == vector.Y;
                        }
                        if (arg_1FF_0)
                        {
                            i++;
                        }
                        else
                        {
                            if (tile.active() && !tile.inActive() && Main.tileSolid[(int)tile.type])
                            {
                                break;
                            }
                            i++;
                        }
                    }
                    if (!Collision.LavaCollision(vector, width, player.height) && Collision.HurtTiles(vector, player.velocity, width, player.height, false).Y <= 0f)
                    {
                        Collision.SlopeCollision(vector, player.velocity, width, player.height, player.gravDir, false);
                        if (Collision.SolidCollision(vector, width, player.height) && i < 99)
                        {
                            Vector2 vector3 = Vector2.UnitX * 16f;
                            if (!(Collision.TileCollision(vector - vector3, vector3, player.width, player.height, false, false, (int)player.gravDir) != vector3))
                            {
                                vector3 = -Vector2.UnitX * 16f;
                                if (!(Collision.TileCollision(vector - vector3, vector3, player.width, player.height, false, false, (int)player.gravDir) != vector3))
                                {
                                    vector3 = Vector2.UnitY * 16f;
                                    if (!(Collision.TileCollision(vector - vector3, vector3, player.width, player.height, false, false, (int)player.gravDir) != vector3))
                                    {
                                        vector3 = -Vector2.UnitY * 16f;
                                        if (!(Collision.TileCollision(vector - vector3, vector3, player.width, player.height, false, false, (int)player.gravDir) != vector3))
                                        {
                                            flag = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (!flag)
            {
                return;
            }

            ModTeleport(player, vector, syncData, false);
        }

        public static void ModTeleport(Player player, Vector2 pos, bool syncData = false, bool convertFromTiles = false)
        {
            bool postImmune = player.immune;
            int postImmunteTime = player.immuneTime;
            if (convertFromTiles)
            {
                pos = new Vector2(pos.X * 16 + 8 - player.width / 2, pos.Y * 16 - player.height);
            }
            player.grappling[0] = -1;
            player.grapCount = 0;
            for (int index = 0; index < 1000; ++index)
            {
                if (Main.projectile[index].active && Main.projectile[index].owner == player.whoAmI && Main.projectile[index].aiStyle == 7)
                {
                    Main.projectile[index].Kill();
                }
            }
            player.Teleport(pos, 2, 0);
            player.velocity = Vector2.Zero;
            player.immune = postImmune;
            player.immuneTime = postImmunteTime;
            for (int index = 0; index < 100; ++index)
            {
                Main.dust[Dust.NewDust(player.position, player.width, player.height, 164, player.velocity.X * 0.2f, player.velocity.Y * 0.2f, 150, Color.Cyan, 1.2f)].velocity *= 0.5f;
            }
            Main.TeleportEffect(player.getRect(), 1);
            Main.TeleportEffect(player.getRect(), 3);
            Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 6);
            if (Main.netMode != NetmodeID.Server)
            {
                return;
            }
            if (syncData)
            {
                RemoteClient.CheckSection(player.whoAmI, player.position, 1);
                NetMessage.SendData(65, -1, -1, null, 0, (float)player.whoAmI, pos.X, pos.Y, 3, 0, 0);
            }
        }
        #endregion

        #region UpdateEquips
        public override void UpdateVanityAccessories()
        {
            for (int n = 13; n < 18 + player.extraAccessorySlots; n++)
            {
                Item item = player.armor[n];
                if (item.type == ModContent.ItemType<Popo>())
                {
                    snowmanHide = false;
                    snowmanForce = true;
                }
                else if (item.type == ModContent.ItemType<AbyssalDivingSuit>())
                {
                    abyssalDivingSuitHide = false;
                    abyssalDivingSuitForce = true;
                }
                else if (item.type == ModContent.ItemType<SirensHeart>())
                {
                    sirenBoobsHide = false;
                    sirenBoobsForce = true;
                }
                else if (item.type == ModContent.ItemType<SirensHeartAlt>())
                {
                    sirenBoobsAltHide = false;
                    sirenBoobsAltForce = true;
                }
            }
        }

        public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
        {
            if (Config.MiningSpeedBoost)
            {
                player.pickSpeed *= 0.75f;
            }

            #region MeleeSpeed
            float meleeSpeedMult = 0f;
            if (bBlood)
            {
                meleeSpeedMult += 0.025f;
            }
            if (rRage)
            {
                meleeSpeedMult += 0.05f;
            }
            if (graxDefense)
            {
                meleeSpeedMult += 0.1f;
            }
            if (sMeleeBoost)
            {
                meleeSpeedMult += 0.05f;
            }
            if (yPower)
            {
                meleeSpeedMult += 0.05f;
            }
            if (darkSunRing)
            {
                meleeSpeedMult += 0.12f;
            }
            if (badgeOfBravery)
            {
                meleeSpeedMult += 0.15f;
            }
            if (badgeOfBraveryRare)
            {
                meleeSpeedMult += 0.2f;
            }
            if (eGauntlet)
            {
                meleeSpeedMult += 0.15f;
            }
            if (yInsignia)
            {
                meleeSpeedMult += 0.1f;
            }
            if (bloodyMary)
            {
                if (Main.bloodMoon)
                {
                    meleeSpeedMult += 0.15f;
                }
            }
            if (community)
            {
                float floatTypeBoost = 0.01f +
                    (NPC.downedSlimeKing ? 0.01f : 0f) +
                    (NPC.downedBoss1 ? 0.01f : 0f) +
                    (NPC.downedBoss2 ? 0.01f : 0f) +
                    (NPC.downedQueenBee ? 0.01f : 0f) + //0.05
                    (NPC.downedBoss3 ? 0.01f : 0f) +
                    (Main.hardMode ? 0.01f : 0f) +
                    (NPC.downedMechBossAny ? 0.01f : 0f) +
                    (NPC.downedPlantBoss ? 0.01f : 0f) +
                    (NPC.downedGolemBoss ? 0.01f : 0f) + //0.1
                    (NPC.downedFishron ? 0.01f : 0f) +
                    (NPC.downedAncientCultist ? 0.01f : 0f) +
                    (NPC.downedMoonlord ? 0.01f : 0f) +
                    (CalamityWorld.downedProvidence ? 0.02f : 0f) + //0.15
                    (CalamityWorld.downedDoG ? 0.02f : 0f) + //0.17
                    (CalamityWorld.downedYharon ? 0.03f : 0f); //0.2
                meleeSpeedMult += floatTypeBoost * 0.25f;
            }
            if (eArtifact)
            {
                meleeSpeedMult += 0.1f;
            }
            if (Config.ProficiencyEnabled)
            {
                meleeSpeedMult += GetMeleeSpeedBonus();
            }
            player.meleeSpeed += meleeSpeedMult;
            #endregion

            if (snowman)
            {
                if (player.whoAmI == Main.myPlayer && !snowmanNoseless)
                {
                    player.AddBuff(ModContent.BuffType<PopoBuff>(), 60, true);
                }
            }
            if (abyssalDivingSuit)
            {
                player.AddBuff(ModContent.BuffType<AbyssalDivingSuitBuff>(), 60, true);
                if (player.whoAmI == Main.myPlayer)
                {
                    if (abyssalDivingSuitCooldown)
                    {
                        for (int l = 0; l < Player.MaxBuffs; l++)
                        {
                            int hasBuff = player.buffType[l];
                            if (player.buffTime[l] < 30 && hasBuff == ModContent.BuffType<AbyssalDivingSuitPlatesBroken>())
                            {
                                abyssalDivingSuitPlateHits = 0;
                                player.DelBuff(l);
                                l = -1;
                            }
                        }
                    }
                    else
                    {
                        player.AddBuff(ModContent.BuffType<AbyssalDivingSuitPlates>(), 2);
                    }
                }
            }
            if (sirenBoobs)
            {
                player.AddBuff(ModContent.BuffType<SirenBobs>(), 60, true);
            }
            else if (sirenBoobsAlt)
            {
                player.AddBuff(ModContent.BuffType<SirenBobsAlt>(), 60, true);
            }
            if ((sirenBoobs || sirenBoobsAlt) && NPC.downedBoss3)
            {
                if (player.whoAmI == Main.myPlayer && !sirenIceCooldown)
                {
                    player.AddBuff(ModContent.BuffType<IceShieldBuff>(), 2);
                }
            }
        }
        #endregion

        #region PreUpdateBuffs
        public override void PreUpdateBuffs()
        {
            // Remove the mighty wind buff if the player is in the astral desert or if a boss is alive.
            if (player.ZoneDesert && (ZoneAstral || areThereAnyDamnBosses) && player.HasBuff(BuffID.WindPushed))
            {
                player.ClearBuff(BuffID.WindPushed);
            }
        }
        #endregion

        #region PostUpdate

        #region PostUpdateBuffs
        public override void PostUpdateBuffs()
        {
            if (CalamityWorld.defiled)
                Defiled();

            if (weakPetrification)
                WeakPetrification();

            if (player.mount.Active || player.mount.Cart || (Config.BossRushDashCurse && CalamityWorld.bossRushActive))
            {
                player.dashDelay = 60;
                dashMod = 0;
            }

            if (silvaCountdown > 0 && hasSilvaEffect && silvaSet)
            {
                if (player.lifeRegen < 0)
                    player.lifeRegen = 0;
            }
        }
        #endregion

        #region PostUpdateEquips
        public override void PostUpdateEquips()
        {
            if (CalamityWorld.defiled)
                Defiled();

            if (weakPetrification)
                WeakPetrification();

            if (player.mount.Active || player.mount.Cart || (Config.BossRushDashCurse && CalamityWorld.bossRushActive))
            {
                player.dashDelay = 60;
                dashMod = 0;
            }

            if (silvaCountdown > 0 && hasSilvaEffect && silvaSet)
            {
                if (player.lifeRegen < 0)
                    player.lifeRegen = 0;
            }
        }
        #endregion

        public override void PostUpdateMiscEffects()
        {
            if (Config.ExpertDebuffDurationReduction)
                Main.expertDebuffTime = 1f;

            areThereAnyDamnBosses = CalamityGlobalNPC.AnyBossNPCS();

			if (areThereAnyDamnBosses)
			{
				if (player.whoAmI == Main.myPlayer)
					player.AddBuff(ModContent.BuffType<BossZen>(), 2, false);
			}

            #region RevengeanceEffects
            /*if (!areThereAnyDamnBosses)
            {
                distanceFromBoss = -1;
            }*/
            if (CalamityWorld.revenge)
            {
                if (player.lifeSteal > (CalamityWorld.death ? 50f : 60f))
                {
                    player.lifeSteal = CalamityWorld.death ? 50f : 60f;
                }
                if (player.whoAmI == Main.myPlayer)
                {
                    if (player.onHitDodge)
                    {
                        for (int l = 0; l < Player.MaxBuffs; l++)
                        {
                            int hasBuff = player.buffType[l];
                            if (player.buffTime[l] > 360 && hasBuff == BuffID.ShadowDodge)
                            {
                                player.buffTime[l] = 360;
                            }
                        }
                    }
                    if (player.immuneTime > 120)
                    {
                        player.immuneTime = 120;
                    }
                    if (Config.AdrenalineAndRage)
                    {
                        int stressGain = 0;
                        if (rageMode)
                        {
                            stressGain = -2000;
                        }
                        else
                        {
                            if (draedonsHeart)
                            {
                                if (draedonsStressGain)
                                {
                                    stressGain += 60;
                                }
                            }
                            else if (heartOfDarkness)
                            {
                                stressGain += 30;
                            }
                        }
                        stressCD++;
                        if (stressCD >= 60)
                        {
                            stressCD = 0;
                            stress += stressGain;
                            if (stress < 0)
                            {
                                stress = 0;
                            }
                            if (stress >= stressMax)
                            {
                                if (playFullRageSound)
                                {
                                    playFullRageSound = false;
                                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/FullRage"), (int)player.position.X, (int)player.position.Y);
                                }
                                stress = stressMax;
                            }
                            else
                            {
                                playFullRageSound = true;
                            }
                        }
                        stressLevel500 = stress >= 9800;
                        if (stressLevel500 && !hAttack)
                        {
                            int heartAttackChance = (draedonsHeart || heartOfDarkness) ? 2000 : 10000;
                            if (Main.rand.Next(heartAttackChance) == 0)
                            {
                                player.AddBuff(ModContent.BuffType<HeartAttack>(), 18000);
                            }
                        }
                        if (adrenaline >= adrenalineMax)
                        {
                            adrenalineMaxTimer--;
                            if (adrenalineMaxTimer <= 0)
                            {
                                if (playAdrenalineBurnoutSound)
                                {
                                    playAdrenalineBurnoutSound = false;
                                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AdrenalineBurnout1"), (int)player.position.X, (int)player.position.Y);
                                }
                                adrenalineDmgDown--;
                                if (adrenalineDmgDown < 0)
                                {
                                    if (playFullAdrenalineBurnoutSound)
                                    {
                                        playFullAdrenalineBurnoutSound = false;
                                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AdrenalineBurnout2"), (int)player.position.X, (int)player.position.Y);
                                    }
                                    adrenalineDmgDown = 0;
                                }
                                adrenalineMaxTimer = 0;
                            }
                        }
                        else if (!adrenalineMode && adrenaline <= 0)
                        {
                            playAdrenalineBurnoutSound = true;
                            playFullAdrenalineBurnoutSound = true;
                            adrenalineDmgDown = 600;
                            adrenalineMaxTimer = 300;
                            adrenalineDmgMult = 1f;
                        }
                        adrenalineDmgMult = 0.1f * (float)(adrenalineDmgDown / 60);
                        if (adrenalineDmgMult < 0.33f)
                            adrenalineDmgMult = 0.33f;
                        int adrenalineGain = 0;
                        bool SCalAlive = NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>());
                        if (adrenalineMode)
                        {
                            adrenalineGain = SCalAlive ? -10000 : -2000;
                        }
                        else
                        {
                            if (Main.wof >= 0 && player.position.Y < (float)((Main.maxTilesY - 200) * 16)) // >
                            {
                                adrenaline = 0;
                            }
                            else if (areThereAnyDamnBosses || CalamityWorld.DoGSecondStageCountdown > 0)
                            {
                                int adrenalineTickBoost = 0 +
                                    (adrenalineBoostOne ? 63 : 0) + //286
                                    (adrenalineBoostTwo ? 115 : 0) + //401
                                    (adrenalineBoostThree ? 100 : 0); //501
                                adrenalineGain = 223 + adrenalineTickBoost; //pre-slime god = 45, pre-astrum deus = 35, pre-polterghast = 25, post-polter = 20
                                                                            /*if (distanceFromBoss == -1 || distanceFromBoss > 5600)
                                                                            {
                                                                                adrenalineGain /= 4;
                                                                            }*/
                            }
                            else
                            {
                                adrenaline = 0;
                            }
                        }
                        adrenalineCD++;
                        if (adrenalineCD >= (SCalAlive ? 135 : 60))
                        {
                            adrenalineCD = 0;
                            adrenaline += adrenalineGain;
                            if (adrenaline < 0)
                            {
                                adrenaline = 0;
                            }
                            if (adrenaline >= adrenalineMax)
                            {
                                if (playFullAdrenalineSound)
                                {
                                    playFullAdrenalineSound = false;
                                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/FullAdrenaline"), (int)player.position.X, (int)player.position.Y);
                                }
                                adrenaline = adrenalineMax;
                            }
                            else
                            {
                                playFullAdrenalineSound = true;
                            }
                        }
                    }
                }
            }
            else
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    stressCD++;
                    if (stressCD >= 60)
                    {
                        stressCD = 0;
                        stress += -30;
                        if (stress < 0)
                        {
                            stress = 0;
                        }
                    }
                    adrenalineCD++;
                    if (adrenalineCD >= 60)
                    {
                        adrenalineCD = 0;
                        adrenaline += -30;
                        if (adrenaline < 0)
                        {
                            adrenaline = 0;
                        }
                    }
                }
            }
            if (player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
            {
                packetTimer++;
                if (packetTimer == 60)
                {
                    packetTimer = 0;
                    StressPacket(false);
                    AdrenalinePacket(false);
                    /*if (areThereAnyDamnBosses)
                        DistanceFromBossPacket(false);*/
                }
            }
            #endregion

            #region STRESSNOLONGEREXISTSYOUSEXUALDINOSAUR
            if (stressPills)
            {
                player.statDefense += 8;
                player.allDamage += 0.08f;
            }
            if (laudanum)
            {
                player.statDefense += 6;
                player.allDamage += 0.06f;
            }
            if (draedonsHeart)
            {
                player.allDamage += 0.1f;
            }
            if (!stressLevel500 && player.FindBuffIndex(ModContent.BuffType<HeartAttack>()) > -1)
            { player.ClearBuff(ModContent.BuffType<HeartAttack>()); }
            if (draedonsHeart && (double)Math.Abs(player.velocity.X) < 0.05 && (double)Math.Abs(player.velocity.Y) < 0.05 && player.itemAnimation == 0)
            {
                player.statDefense += 25;
            }
            if (hAttack)
            {
                if (heartOfDarkness || draedonsHeart)
                {
                    player.allDamage += 0.1f;
                }
                player.statLifeMax2 += player.statLifeMax / 5 / 20 * 5;
            }
            if (affliction || afflicted)
            {
                player.endurance += 0.07f;
                player.statDefense += 20;
                player.allDamage += 0.12f;
                player.statLifeMax2 += player.statLifeMax / 5 / 20 * 10;
            }
            #endregion

            #region MaxLifeAndManaBoosts
            player.statLifeMax2 +=
                (mFruit ? 25 : 0) +
                (bOrange ? 25 : 0) +
                (eBerry ? 25 : 0) +
                (dFruit ? 25 : 0);
            if (silvaHitCounter > 0)
            {
                player.statLifeMax2 -= silvaHitCounter * 100;
                if (player.statLifeMax2 <= 400)
                {
                    player.statLifeMax2 = 400;
                    if (silvaCountdown > 0)
                    {
                        if (player.FindBuffIndex(ModContent.BuffType<SilvaRevival>()) > -1)
                        { player.ClearBuff(ModContent.BuffType<SilvaRevival>()); }
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SilvaDispel"), (int)player.position.X, (int)player.position.Y);
                    }
                    silvaCountdown = 0;
                }
            }
            player.statManaMax2 +=
                (pHeart ? 50 : 0) +
                (eCore ? 50 : 0) +
                (cShard ? 50 : 0) +
                (starBeamRye ? 50 : 0);
			if (Main.netMode != NetmodeID.Server && player.whoAmI == Main.myPlayer)
            {
                Texture2D rain3 = ModContent.GetTexture("CalamityMod/ExtraTextures/Rain3");
                Texture2D rainOriginal = ModContent.GetTexture("CalamityMod/ExtraTextures/RainOriginal");
                Texture2D mana2 = ModContent.GetTexture("CalamityMod/ExtraTextures/Mana2");
                Texture2D mana3 = ModContent.GetTexture("CalamityMod/ExtraTextures/Mana3");
                Texture2D mana4 = ModContent.GetTexture("CalamityMod/ExtraTextures/Mana4");
                Texture2D manaOriginal = ModContent.GetTexture("CalamityMod/ExtraTextures/ManaOriginal");
                Texture2D carpetAuric = ModContent.GetTexture("CalamityMod/ExtraTextures/AuricCarpet");
                Texture2D carpetOriginal = ModContent.GetTexture("CalamityMod/ExtraTextures/Carpet");
                int totalManaBoost =
                    (pHeart ? 1 : 0) +
                    (eCore ? 1 : 0) +
                    (cShard ? 1 : 0);
                switch (totalManaBoost)
                {
                    default:
                        Main.manaTexture = manaOriginal;
                        break;
                    case 3:
                        Main.manaTexture = mana4;
                        break;
                    case 2:
                        Main.manaTexture = mana3;
                        break;
                    case 1:
                        Main.manaTexture = mana2;
                        break;
                }
                if (Main.bloodMoon)
                { Main.rainTexture = rainOriginal; }
                else if (Main.raining && ZoneSulphur)
                { Main.rainTexture = rain3; }
                else
                { Main.rainTexture = rainOriginal; }
                if (auricSet)
                { Main.flyingCarpetTexture = carpetAuric; }
                else
                { Main.flyingCarpetTexture = carpetOriginal; }
            }
            #endregion

            #region MiscEffects
            if (player.HeldItem.type == ModContent.ItemType<GaelsGreatsword>())
            {
                gaelSwitchTimer = GaelSwitchPhase.LoseRage;
                stress += (int)MathHelper.Min(5, 10000 - stress);
            }
            else if (player.HeldItem.type != ModContent.ItemType<GaelsGreatsword>() && gaelSwitchTimer == GaelSwitchPhase.LoseRage)
            {
                stress = 0;
                gaelSwitchTimer = GaelSwitchPhase.None;
            }
            if (Config.ProficiencyEnabled)
            {
                GetExactLevelUp();
            }

            if (gainRageCooldown > 0)
                gainRageCooldown--;

            if (player.nebulaLevelMana > 0 && player.statMana < player.statManaMax2)
            {
                int num = 12;
                nebulaManaNerfCounter += player.nebulaLevelMana;
                if (nebulaManaNerfCounter >= num)
                {
                    nebulaManaNerfCounter -= num;
                    player.statMana--;
                    if (player.statMana < 0)
                    {
                        player.statMana = 0;
                    }
                }
            }
            else
            {
                nebulaManaNerfCounter = 0;
            }
            if (Main.myPlayer == player.whoAmI)
            {
                BossHealthBarManager.SHOULD_DRAW_SMALLTEXT_HEALTH = shouldDrawSmallText;
                /*if (player.chest != -1)
                {
                    if (player.chest != -2)
                    {
                        trashManChest = -1;
                    }
                    if (trashManChest >= 0)
                    {
                        if (!Main.projectile[trashManChest].active || Main.projectile[trashManChest].type != ModContent.ProjectileType<DannyDevito>())
                        {
                            Main.PlaySound(SoundID.Item11, -1, -1);
                            player.chest = -1;
                            Recipe.FindRecipes();
                        }
                        else
                        {
                            int num16 = (int)(((double)player.position.X + (double)player.width * 0.5) / 16.0);
                            int num17 = (int)(((double)player.position.Y + (double)player.height * 0.5) / 16.0);
                            player.chestX = (int)Main.projectile[trashManChest].Center.X / 16;
                            player.chestY = (int)Main.projectile[trashManChest].Center.Y / 16;
                            if (num16 < player.chestX - Player.tileRangeX || num16 > player.chestX + Player.tileRangeX + 1 || num17 < player.chestY - Player.tileRangeY || num17 > player.chestY + Player.tileRangeY + 1)
                            {
                                if (player.chest != -1)
                                {
                                    Main.PlaySound(SoundID.Item11, -1, -1);
                                }
                                player.chest = -1;
                                Recipe.FindRecipes();
                            }
                        }
                    }
                }
                else
                {
                    trashManChest = -1;
                }*/
            }
            if (silvaSet || invincible || margarita)
            {
                foreach (int debuff in CalamityMod.debuffList)
                    player.buffImmune[debuff] = true;
            }
            if (aSparkRare)
            {
                player.buffImmune[BuffID.Electrified] = true;
            }
            if (Config.ExpertChilledWaterRemoval)
            {
                if (Main.expertMode && player.ZoneSnow && player.wet && !player.lavaWet && !player.honeyWet && !player.arcticDivingGear)
                {
                    player.buffImmune[BuffID.Chilled] = true;
                    if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
                    {
                        if (Main.myPlayer == player.whoAmI && !player.gills && !player.merman)
                        {
                            if (player.breath > 0)
                                player.breath--;
                        }
                    }
                }
            }

			int lightStrength = 0 +
				((player.lightOrb || player.crimsonHeart || player.magicLantern || radiator) ? 1 : 0) + //1
				(aquaticEmblem ? 1 : 0) + //2
				(player.arcticDivingGear ? 1 : 0) + //3
				(jellyfishNecklace ? 1 : 0) + //4
				((player.blueFairy || player.greenFairy || player.redFairy || player.petFlagDD2Ghost || babyGhostBell) ? 2 : 0) + //6
				((shine || lumenousAmulet) ? 2 : 0) + //8
				((player.wisp || player.suspiciouslookingTentacle || sirenPet) ? 3 : 0); //11

			double breathLossMult = 1.0 -
				(player.gills ? 0.2 : 0.0) - //0.8
				(player.accDivingHelm ? 0.25 : 0.0) - //0.75
				(player.arcticDivingGear ? 0.25 : 0.0) - //0.75
				(aquaticEmblem ? 0.25 : 0.0) - //0.75
				(player.accMerman ? 0.3 : 0.0) - //0.7
				(victideSet ? 0.2 : 0.0) - //0.85
				(((sirenBoobs || sirenBoobsAlt) && NPC.downedBoss3) ? 0.3 : 0.0) - //0.7
				(abyssalDivingSuit ? 0.3 : 0.0); //0.7

			if (breathLossMult < 0.05)
				breathLossMult = 0.05;

			double tickMult = 1.0 +
				(player.gills ? 4.0 : 0.0) + //5
				(player.ignoreWater ? 5.0 : 0.0) + //10
				(player.accDivingHelm ? 10.0 : 0.0) + //20
				(player.arcticDivingGear ? 10.0 : 0.0) + //30
				(aquaticEmblem ? 10.0 : 0.0) + //40
				(player.accMerman ? 15.0 : 0.0) + //55
				(victideSet ? 5.0 : 0.0) + //60
				(((sirenBoobs || sirenBoobsAlt) && NPC.downedBoss3) ? 15.0 : 0.0) + //75
				(abyssalDivingSuit ? 15.0 : 0.0); //90

			if (tickMult > 50.0)
				tickMult = 50.0;

			int lifeLossAtZeroBreathResist = 0;
			if (depthCharm)
			{
				lifeLossAtZeroBreathResist += 3;
				if (abyssalDivingSuit)
					lifeLossAtZeroBreathResist += 6;
			}

			abyssLightLevelStat = lightStrength;
			abyssBreathLossStats[0] = (int)(2D * breathLossMult);
			abyssBreathLossStats[1] = (int)(6D * breathLossMult);
			abyssBreathLossStats[2] = (int)(18D * breathLossMult);
			abyssBreathLossStats[3] = (int)(54D * breathLossMult);
			abyssBreathLossRateStat = (int)(6D * tickMult);
			abyssLifeLostAtZeroBreathStats[0] = 3 - lifeLossAtZeroBreathResist;
			abyssLifeLostAtZeroBreathStats[1] = 6 - lifeLossAtZeroBreathResist;
			abyssLifeLostAtZeroBreathStats[2] = 12 - lifeLossAtZeroBreathResist;
			abyssLifeLostAtZeroBreathStats[3] = 24 - lifeLossAtZeroBreathResist;

			if (abyssLifeLostAtZeroBreathStats[0] < 0)
				abyssLifeLostAtZeroBreathStats[0] = 0;
			if (abyssLifeLostAtZeroBreathStats[1] < 0)
				abyssLifeLostAtZeroBreathStats[1] = 0;

			if (ZoneAbyss)
            {
                if (abyssalAmulet)
                    player.statLifeMax2 += player.statLifeMax2 / 5 / 20 * (lumenousAmulet ? 25 : 10);

                if (Main.myPlayer == player.whoAmI) //4200 total tiles small world
                {
					int breathLoss = 2;
					int lifeLossAtZeroBreath = 3;
					int tick = 6;

					bool lightLevelOne = lightStrength > 0; //1+
                    bool lightLevelTwo = lightStrength > 2; //3+
                    bool lightLevelThree = lightStrength > 4; //5+
                    bool lightLevelFour = lightStrength > 6; //7+

                    if (ZoneAbyssLayer4) //3200 and below
                    {
                        breathLoss = 54;
                        if (!lightLevelFour)
                            player.blind = true;
                        if (!lightLevelThree)
                            player.headcovered = true;
                        player.bleed = true;
                        lifeLossAtZeroBreath = 24;
                        player.statDefense -= anechoicPlating ? 40 : 120;
                    }
                    else if (ZoneAbyssLayer3) //2700 to 3200
                    {
                        breathLoss = 18;
                        if (!lightLevelThree)
                            player.blind = true;
                        if (!lightLevelTwo)
                            player.headcovered = true;
                        if (!abyssalDivingSuit)
                            player.bleed = true;
                        lifeLossAtZeroBreath = 12;
                        player.statDefense -= anechoicPlating ? 20 : 60;
                    }
                    else if (ZoneAbyssLayer2) //2100 to 2700
                    {
                        breathLoss = 6;
                        if (!lightLevelTwo)
                            player.blind = true;
                        if (!depthCharm)
                            player.bleed = true;
                        lifeLossAtZeroBreath = 6;
                        player.statDefense -= anechoicPlating ? 10 : 30;
                    }
                    else if (ZoneAbyssLayer1) //1500 to 2100
                    {
                        if (!lightLevelOne)
                            player.blind = true;
                        player.statDefense -= anechoicPlating ? 5 : 15;
                    }

                    breathLoss = (int)((double)breathLoss * breathLossMult);
                    tick = (int)((double)tick * tickMult);

                    if (player.gills || player.merman)
                    {
                        if (player.breath > 0)
                            player.breath -= 3;
                    }

					abyssBreathCD++;
					if (abyssBreathCD >= tick)
                    {
                        abyssBreathCD = 0;

                        if (player.breath > 0)
                            player.breath -= breathLoss;

                        if (cDepth)
                        {
                            if (player.breath > 0)
                                player.breath--;
                        }

                        if (player.breath <= 0)
                        {
							lifeLossAtZeroBreath -= lifeLossAtZeroBreathResist;

                            if (lifeLossAtZeroBreath < 0)
                                lifeLossAtZeroBreath = 0;

                            player.statLife -= lifeLossAtZeroBreath;

                            if (player.statLife <= 0)
                            {
                                abyssDeath = true;
                                KillPlayer();
                            }
                        }
                    }
                }
            }
            else
            {
                abyssBreathCD = 0;
                abyssDeath = false;
            }

            if (!player.mount.Active)
            {
                if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir) && ironBoots)
                {
                    player.maxFallSpeed = 9f;
                }
                if (aeroSet && !player.wet)
                {
                    player.maxFallSpeed = 15f;
                }
                if (normalityRelocator)
                {
                    player.maxFallSpeed *= 1.1f;
                }
                if (etherealExtorter && player.ZoneSkyHeight)
                {
                    player.maxFallSpeed *= 1.25f;
                }
            }
            if (normalityRelocator)
            {
                player.moveSpeed += 0.1f;
            }
            if (player.ZoneSkyHeight)
            {
                if (astrumDeusLore)
                {
                    player.moveSpeed += 0.2f;
                }
                if (astrumAureusLore)
                {
                    player.jumpSpeedBoost += 0.5f;
                }
            }
            if (omegaBlueSet) //should apply after rev caps, actually those are gone so AAAAA
            {
                //add tentacles
                if (player.ownedProjectileCounts[ModContent.ProjectileType<OmegaBlueTentacle>()] < 6)
                {
                    bool[] tentaclesPresent = new bool[6];
                    for (int i = 0; i < 1000; i++)
                    {
                        Projectile projectile = Main.projectile[i];
                        if (projectile.active && projectile.type == ModContent.ProjectileType<OmegaBlueTentacle>() && projectile.owner == Main.myPlayer && projectile.ai[1] >= 0f && projectile.ai[1] < 6f)
                        {
                            tentaclesPresent[(int)projectile.ai[1]] = true;
                        }
                    }
                    for (int i = 0; i < 6; i++)
                    {
                        if (!tentaclesPresent[i])
                        {
                            float modifier = player.meleeDamage + player.magicDamage + player.rangedDamage +
                                player.Calamity().throwingDamage + player.minionDamage;
                            modifier /= 5f;
                            int damage = (int)(666 * modifier);
                            Vector2 vel = new Vector2(Main.rand.Next(-13, 14), Main.rand.Next(-13, 14)) * 0.25f;
                            Projectile.NewProjectile(player.Center, vel, ModContent.ProjectileType<OmegaBlueTentacle>(), damage, 8f, Main.myPlayer, Main.rand.Next(120), i);
                        }
                    }
                }
                float damageUp = 0.1f;
                int critUp = 10;
                if (omegaBlueHentai)
                {
                    damageUp *= 2f;
                    critUp *= 2;
                }
                player.allDamage += damageUp;
                AllCritBoost(critUp);
            }
            if (!bOut)
            {
                if (gHealer)
                {
                    if (healCounter > 0)
                    {
                        healCounter--;
                    }
                    if (healCounter <= 0)
                    {
                        healCounter = 300;
                        if (player.whoAmI == Main.myPlayer)
                        {
                            int healAmount = 5 +
                                (gDefense ? 5 : 0) +
                                (gOffense ? 5 : 0);
                            player.statLife += healAmount;
                            player.HealEffect(healAmount);
                        }
                    }
                }
                if (gDefense)
                {
                    player.moveSpeed += 0.1f +
                        (gOffense ? 0.1f : 0f);
                    player.endurance += 0.025f +
                        (gOffense ? 0.025f : 0f);
                }
                if (gOffense)
                {
                    player.minionDamage += 0.1f +
                        (gDefense ? 0.05f : 0f);
                }
            }

            // You always get the max minions, even during the effect of the burnout debuff
            if (gOffense)
                player.maxMinions++;

            if (draconicSurgeCooldown > 0)
                draconicSurgeCooldown--;
            if (fleshTotemCooldown > 0)
                fleshTotemCooldown--;
            if (astralStarRainCooldown > 0)
                astralStarRainCooldown--;
            if (bloodflareMageCooldown > 0)
                bloodflareMageCooldown--;
            if (tarraMageHealCooldown > 0)
                tarraMageHealCooldown--;
            if (featherCrownCooldown > 0)
                featherCrownCooldown--;
            if (moonCrownCooldown > 0)
                moonCrownCooldown--;
            if (sandCloakCooldown > 0)
                sandCloakCooldown--;
            if (spectralVeilImmunity > 0)
                spectralVeilImmunity--;
            if (plaguedFuelPackCooldown > 0)
                plaguedFuelPackCooldown--;
            if (plaguedFuelPackDash > 0)
                plaguedFuelPackDash--;
            if (ataxiaDmg > 0f)
                ataxiaDmg -= 1.5f;
            if (ataxiaDmg < 0f)
                ataxiaDmg = 0f;
            if (xerocDmg > 0f)
                xerocDmg -= 2f;
            if (xerocDmg < 0f)
                xerocDmg = 0f;
            if (godSlayerDmg > 0f)
                godSlayerDmg -= 2.5f;
            if (godSlayerDmg < 0f)
                godSlayerDmg = 0f;
            if (aBulwarkRareMeleeBoostTimer > 0)
                aBulwarkRareMeleeBoostTimer--;
            if (bossRushImmunityFrameCurseTimer > 0)
                bossRushImmunityFrameCurseTimer--;
            if (gaelRageCooldown > 0)
                gaelRageCooldown--;

            if (silvaCountdown > 0 && hasSilvaEffect && silvaSet)
            {
                player.buffImmune[ModContent.BuffType<VulnerabilityHex>()] = true;
                silvaCountdown--;
                if (silvaCountdown <= 0)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SilvaDispel"), (int)player.position.X, (int)player.position.Y);
                }
                for (int j = 0; j < 2; j++)
                {
                    int num = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 157, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 2f);
                    Dust expr_A4_cp_0 = Main.dust[num];
                    expr_A4_cp_0.position.X += (float)Main.rand.Next(-20, 21);
                    Dust expr_CB_cp_0 = Main.dust[num];
                    expr_CB_cp_0.position.Y += (float)Main.rand.Next(-20, 21);
                    Main.dust[num].velocity *= 0.9f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                    Main.dust[num].shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                    }
                }
            }

			if (tarragonCloak)
			{
				tarraDefenseTime--;
				if (tarraDefenseTime <= 0)
				{
					tarraDefenseTime = 600;
					if (player.whoAmI == Main.myPlayer)
					{
						player.AddBuff(ModContent.BuffType<TarragonCloakCooldown>(), 1800, false);
					}
				}
				for (int j = 0; j < 2; j++)
				{
					int num = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 157, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 2f);
					Dust expr_A4_cp_0 = Main.dust[num];
					expr_A4_cp_0.position.X += (float)Main.rand.Next(-20, 21);
					Dust expr_CB_cp_0 = Main.dust[num];
					expr_CB_cp_0.position.Y += (float)Main.rand.Next(-20, 21);
					Main.dust[num].velocity *= 0.9f;
					Main.dust[num].noGravity = true;
					Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
					Main.dust[num].shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
					if (Main.rand.NextBool(2))
					{
						Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
					}
				}
			}

            if (tarraThrowing)
            {
				if (tarragonImmunity)
				{
					player.immune = true;
					player.immuneTime = 2;
				}

                if (tarraThrowingCrits >= 25)
                {
                    tarraThrowingCrits = 0;
					if (player.whoAmI == Main.myPlayer)
					{
						player.AddBuff(ModContent.BuffType<TarragonImmunity>(), 300, false);
					}
                }

				for (int l = 0; l < Player.MaxBuffs; l++)
                {
                    int hasBuff = player.buffType[l];
					if (player.buffTime[l] <= 2 && hasBuff == ModContent.BuffType<TarragonImmunity>())
					{
						if (player.whoAmI == Main.myPlayer)
							player.AddBuff(ModContent.BuffType<TarragonImmunityCooldown>(), 1500, false);
					}

					bool shouldAffect = CalamityMod.debuffList.Contains(hasBuff);
                    if (shouldAffect)
                    {
                        player.Calamity().throwingDamage += 0.1f;
                    }
                }
            }

            if (bloodflareSet)
            {
                if (bloodflareHeartTimer > 0)
                    bloodflareHeartTimer--;
                if (bloodflareManaTimer > 0)
                    bloodflareManaTimer--;
            }

            if (bloodflareMelee)
            {
                if (bloodflareMeleeHits >= 15)
                {
                    bloodflareMeleeHits = 0;
					if (player.whoAmI == Main.myPlayer)
					{
						player.AddBuff(ModContent.BuffType<BloodflareBloodFrenzy>(), 302, false);
					}
				}

                if (bloodflareFrenzy)
                {
					for (int l = 0; l < Player.MaxBuffs; l++)
					{
						int hasBuff = player.buffType[l];
						if (player.buffTime[l] <= 2 && hasBuff == ModContent.BuffType<BloodflareBloodFrenzy>())
						{
							if (player.whoAmI == Main.myPlayer)
								player.AddBuff(ModContent.BuffType<BloodflareBloodFrenzyCooldown>(), 1800, false);
						}
					}
                    player.meleeCrit += 25;
                    player.meleeDamage += 0.25f;
                    for (int j = 0; j < 2; j++)
                    {
                        int num = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 5, 0f, 0f, 100, default, 2f);
                        Dust expr_A4_cp_0 = Main.dust[num];
                        expr_A4_cp_0.position.X += (float)Main.rand.Next(-20, 21);
                        Dust expr_CB_cp_0 = Main.dust[num];
                        expr_CB_cp_0.position.Y += (float)Main.rand.Next(-20, 21);
                        Main.dust[num].velocity *= 0.9f;
                        Main.dust[num].noGravity = true;
                        Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                        Main.dust[num].shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                        }
                    }
                }
            }

            if (Main.raining && ZoneSulphur)
            {
                if (player.ZoneOverworldHeight || player.ZoneSkyHeight)
                    player.AddBuff(ModContent.BuffType<Irradiated>(), 2);
            }
            if (raiderTalisman)
            {
                player.Calamity().throwingDamage += (float)raiderStack / 250f * 0.25f;
            }
            if (silvaCountdown <= 0 && hasSilvaEffect && silvaSummon)
            {
                player.maxMinions += 2;
            }
            if (sDefense)
            {
                player.statDefense += 5;
                player.endurance += 0.05f;
            }
            if (absorber)
            {
                player.moveSpeed += 0.12f;
                player.jumpSpeedBoost += 1.2f;
                player.statLifeMax2 += 20;
                player.thorns = 0.5f;
                player.endurance += 0.05f;
                if ((double)Math.Abs(player.velocity.X) < 0.05 && (double)Math.Abs(player.velocity.Y) < 0.05 && player.itemAnimation == 0)
                {
                    player.manaRegenBonus += 2;
                }
                if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
                {
                    player.statDefense += 2;
                    player.moveSpeed += 0.05f;
                }
            }
            if (seaShell)
            {
                if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
                {
                    player.statDefense += 3;
                    player.endurance += 0.05f;
                    player.moveSpeed += 0.15f;
                    player.ignoreWater = true;
                }
            }
            if (coreOfTheBloodGod)
            {
                player.statLifeMax2 += player.statLifeMax2 / 5 / 20 * 10;
            }
            if (bloodPact)
            {
                player.statLifeMax2 += player.statLifeMax2 / 5 / 20 * 100;
            }
            if (aAmpoule)
            {
                Lighting.AddLight((int)(player.position.X + (float)(player.width / 2)) / 16, (int)(player.position.Y + (float)(player.height / 2)) / 16, 1f, 1f, 0.6f);
                player.endurance += 0.05f;
                player.pickSpeed -= 0.25f;
                player.buffImmune[70] = true;
                player.buffImmune[47] = true;
                player.buffImmune[46] = true;
                player.buffImmune[44] = true;
                player.buffImmune[20] = true;
            }
            else if (cFreeze)
            {
                Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), 0.3f, (float)Main.DiscoG / 400f, 0.5f);
            }
            else if (sirenIce)
            {
                Lighting.AddLight((int)(player.position.X + (float)(player.width / 2)) / 16, (int)(player.position.Y + (float)(player.height / 2)) / 16, 0.35f, 1f, 1.25f);
            }
            else if (sirenBoobs || sirenBoobsAlt)
            {
                Lighting.AddLight((int)(player.position.X + (float)(player.width / 2)) / 16, (int)(player.position.Y + (float)(player.height / 2)) / 16, 1.5f, 1f, 0.1f);
            }
            else if (tarraSummon)
            {
                Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), 0f, 3f, 0f);
            }
            if (cFreeze)
            {
                int num = ModContent.BuffType<GlacialState>();
                float num2 = 200f;
                int random = Main.rand.Next(5);
                if (player.whoAmI == Main.myPlayer)
                {
                    if (random == 0)
                    {
                        for (int l = 0; l < 200; l++)
                        {
                            NPC nPC = Main.npc[l];
                            if (nPC.active && !nPC.friendly && nPC.damage > 0 && !nPC.dontTakeDamage && !nPC.buffImmune[num] && Vector2.Distance(player.Center, nPC.Center) <= num2)
                            {
                                if (nPC.FindBuffIndex(num) == -1)
                                {
                                    nPC.AddBuff(num, 120, false);
                                }
                            }
                        }
                    }
                }
            }
            if (invincible || lol)
            {
                player.thorns = 0f;
                player.turtleThorns = false;
            }
            if (player.vortexStealthActive)
            {
                player.rangedDamage -= (1f - player.stealth) * 0.4f; //change 80 to 40
                player.rangedCrit -= (int)((1f - player.stealth) * 5f); //change 20 to 15
            }
            if (polarisBoost)
            {
                player.endurance += 0.01f;
                player.statDefense += 2;
            }
            if (!polarisBoost || player.inventory[player.selectedItem].type != ModContent.ItemType<PolarisParrotfish>())
            {
                polarisBoost = false;
                if (player.FindBuffIndex(ModContent.BuffType<PolarisBuff>()) > -1)
                { player.ClearBuff(ModContent.BuffType<PolarisBuff>()); }
                polarisBoostCounter = 0;
                polarisBoostTwo = false;
                polarisBoostThree = false;
            }
            if (polarisBoostCounter >= 20)
            {
                polarisBoostTwo = false;
                polarisBoostThree = true;
            }
            else if (polarisBoostCounter >= 10)
            {
                polarisBoostTwo = true;
            }
            if (projRefRareLifeRegenCounter > 0)
            {
                projRefRareLifeRegenCounter--;
            }
            if (desertScourgeLore)
            {
                player.statDefense += 5;
            }
            if (skeletronPrimeLore)
            {
                player.armorPenetration += 5;
            }
            if (destroyerLore)
            {
                player.pickSpeed -= 0.05f;
            }
            if (golemLore)
            {
                if ((double)Math.Abs(player.velocity.X) < 0.05 && (double)Math.Abs(player.velocity.Y) < 0.05 && player.itemAnimation == 0)
                {
                    player.statDefense += 10;
                }
            }
            if (aquaticScourgeLore && player.wellFed)
            {
                player.statDefense += 1;
                player.allDamage += 0.025f;
                AllCritBoost(1);
                player.meleeSpeed += 0.025f;
                player.minionKB += 0.25f;
                player.moveSpeed += 0.1f;
            }
            if (eaterOfWorldsLore)
            {
                int damage = 10;
                float knockBack = 1f;
                if (Main.rand.NextBool(15))
                {
                    int num = 0;
                    for (int i = 0; i < 1000; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<TheDeadlyMicrobeProjectile>())
                        {
                            num++;
                        }
                    }
                    if (Main.rand.Next(15) >= num && num < 6)
                    {
                        int num2 = 50;
                        int num3 = 24;
                        int num4 = 90;
                        for (int j = 0; j < num2; j++)
                        {
                            int num5 = Main.rand.Next(200 - j * 2, 400 + j * 2);
                            Vector2 center = player.Center;
                            center.X += (float)Main.rand.Next(-num5, num5 + 1);
                            center.Y += (float)Main.rand.Next(-num5, num5 + 1);
                            if (!Collision.SolidCollision(center, num3, num3) && !Collision.WetCollision(center, num3, num3))
                            {
                                center.X += (float)(num3 / 2);
                                center.Y += (float)(num3 / 2);
                                if (Collision.CanHit(new Vector2(player.Center.X, player.position.Y), 1, 1, center, 1, 1) || Collision.CanHit(new Vector2(player.Center.X, player.position.Y - 50f), 1, 1, center, 1, 1))
                                {
                                    int num6 = (int)center.X / 16;
                                    int num7 = (int)center.Y / 16;
                                    bool flag = false;
                                    if (Main.rand.NextBool(3) && Main.tile[num6, num7] != null && Main.tile[num6, num7].wall > 0)
                                    {
                                        flag = true;
                                    }
                                    else
                                    {
                                        center.X -= (float)(num4 / 2);
                                        center.Y -= (float)(num4 / 2);
                                        if (Collision.SolidCollision(center, num4, num4))
                                        {
                                            center.X += (float)(num4 / 2);
                                            center.Y += (float)(num4 / 2);
                                            flag = true;
                                        }
                                    }
                                    if (flag)
                                    {
                                        for (int k = 0; k < 1000; k++)
                                        {
                                            if (Main.projectile[k].active && Main.projectile[k].owner == player.whoAmI && Main.projectile[k].type == ModContent.ProjectileType<TheDeadlyMicrobeProjectile>() && (center - Main.projectile[k].Center).Length() < 48f)
                                            {
                                                flag = false;
                                                break;
                                            }
                                        }
                                        if (flag && Main.myPlayer == player.whoAmI)
                                        {
                                            Projectile.NewProjectile(center.X, center.Y, 0f, 0f, ModContent.ProjectileType<TheDeadlyMicrobeProjectile>(), damage, knockBack, player.whoAmI, 0f, 0f);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (calcium)
            {
                player.noFallDmg = true;
            }
            if (skeletronLore)
            {
                player.allDamage += 0.05f;
                AllCritBoost(5);
                player.statDefense += 5;
            }
            if (ceaselessHunger)
            {
                for (int j = 0; j < 400; j++)
                {
                    if (Main.item[j].active && Main.item[j].noGrabDelay == 0 && Main.item[j].owner == player.whoAmI)
                    {
                        Main.item[j].beingGrabbed = true;
                        if ((double)player.position.X + (double)player.width * 0.5 > (double)Main.item[j].position.X + (double)Main.item[j].width * 0.5)
                        {
                            if (Main.item[j].velocity.X < 90f + player.velocity.X)
                            {
                                Item item = Main.item[j];
                                item.velocity.X += 9f;
                            }
                            if (Main.item[j].velocity.X < 0f)
                            {
                                Item item = Main.item[j];
                                item.velocity.X += 9f * 0.75f;
                            }
                        }
                        else
                        {
                            if (Main.item[j].velocity.X > -90f + player.velocity.X)
                            {
                                Item item = Main.item[j];
                                item.velocity.X -= 9f;
                            }
                            if (Main.item[j].velocity.X > 0f)
                            {
                                Item item = Main.item[j];
                                item.velocity.X -= 9f * 0.75f;
                            }
                        }
                        if ((double)player.position.Y + (double)player.height * 0.5 > (double)Main.item[j].position.Y + (double)Main.item[j].height * 0.5)
                        {
                            if (Main.item[j].velocity.Y < 90f)
                            {
                                Item item = Main.item[j];
                                item.velocity.Y += 9f;
                            }
                            if (Main.item[j].velocity.Y < 0f)
                            {
                                Item item = Main.item[j];
                                item.velocity.Y += 9f * 0.75f;
                            }
                        }
                        else
                        {
                            if (Main.item[j].velocity.Y > -90f)
                            {
                                Item item = Main.item[j];
                                item.velocity.Y -= 9f;
                            }
                            if (Main.item[j].velocity.Y > 0f)
                            {
                                Item item = Main.item[j];
                                item.velocity.Y -= 9f * 0.75f;
                            }
                        }
                    }
                }
            }
            if (dukeFishronLore)
            {
                player.allDamage += 0.05f;
                AllCritBoost(5);
                player.moveSpeed += 0.1f;
            }
            if (lunaticCultistLore)
            {
                player.endurance += 0.04f;
                player.statDefense += 4;
                player.allDamage += 0.04f;
                AllCritBoost(4);
                player.minionKB += 0.5f;
                player.moveSpeed += 0.1f;
            }
            if (moonLordLore)
            {
                if (player.gravDir == -1f && player.gravControl2)
                {
                    player.endurance += 0.05f;
                    player.statDefense += 10;
                    player.allDamage += 0.1f;
                    AllCritBoost(10);
                    player.minionKB += 1.5f;
                    player.moveSpeed += 0.15f;
                }
            }
            if (leviathanAndSirenLore)
            {
                if (sirenBoobsPrevious || sirenBoobsAltPrevious)
                {
                    player.statLifeMax2 += player.statLifeMax2 / 5 / 20 * 5;
                }
                if (sirenPet)
                {
                    player.spelunkerTimer += 1;
                    if (player.spelunkerTimer >= 10)
                    {
                        player.spelunkerTimer = 0;
                        int num65 = 30;
                        int num66 = (int)player.Center.X / 16;
                        int num67 = (int)player.Center.Y / 16;
                        for (int num68 = num66 - num65; num68 <= num66 + num65; num68++)
                        {
                            for (int num69 = num67 - num65; num69 <= num67 + num65; num69++)
                            {
                                if (Main.rand.NextBool(4))
                                {
                                    Vector2 vector = new Vector2((float)(num66 - num68), (float)(num67 - num69));
                                    if (vector.Length() < (float)num65 && num68 > 0 && num68 < Main.maxTilesX - 1 && num69 > 0 && num69 < Main.maxTilesY - 1 && Main.tile[num68, num69] != null && Main.tile[num68, num69].active())
                                    {
                                        bool flag7 = false;
                                        if (Main.tile[num68, num69].type == 185 && Main.tile[num68, num69].frameY == 18)
                                        {
                                            if (Main.tile[num68, num69].frameX >= 576 && Main.tile[num68, num69].frameX <= 882)
                                            {
                                                flag7 = true;
                                            }
                                        }
                                        else if (Main.tile[num68, num69].type == 186 && Main.tile[num68, num69].frameX >= 864 && Main.tile[num68, num69].frameX <= 1170)
                                        {
                                            flag7 = true;
                                        }
                                        if (flag7 || Main.tileSpelunker[(int)Main.tile[num68, num69].type] || (Main.tileAlch[(int)Main.tile[num68, num69].type] && Main.tile[num68, num69].type != 82))
                                        {
                                            int num70 = Dust.NewDust(new Vector2((float)(num68 * 16), (float)(num69 * 16)), 16, 16, 204, 0f, 0f, 150, default, 0.3f);
                                            Main.dust[num70].fadeIn = 0.75f;
                                            Main.dust[num70].velocity *= 0.1f;
                                            Main.dust[num70].noLight = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (spectralVeil && spectralVeilImmunity > 0)
            {
                Rectangle sVeilRectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && !Main.npc[i].friendly && !Main.npc[i].townNPC && Main.npc[i].immune[player.whoAmI] <= 0 && Main.npc[i].damage > 0)
                    {
                        NPC nPC = Main.npc[i];
                        Rectangle rect = nPC.getRect();
                        if (sVeilRectangle.Intersects(rect) && (nPC.noTileCollide || player.CanHit(nPC)))
                        {
                            if (player.whoAmI == Main.myPlayer)
                            {
                                player.noKnockback = true;
                                rogueStealth = rogueStealthMax;
                                spectralVeilImmunity = 0;

                                for (int k = 0; k < player.hurtCooldowns.Length; k++)
                                {
                                    player.hurtCooldowns[k] = player.immuneTime;
                                }

                                Vector2 sVeilDustDir = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                                sVeilDustDir.Normalize();
                                sVeilDustDir *= 0.5f;
                                for (int j = 0; j < 20; j++)
                                {
                                    int sVeilDustIndex1 = Dust.NewDust(player.Center, 1, 1, 21, sVeilDustDir.X * j, sVeilDustDir.Y * j);
                                    int sVeilDustIndex2 = Dust.NewDust(player.Center, 1, 1, 21, -sVeilDustDir.X * j, -sVeilDustDir.Y * j);
                                    Main.dust[sVeilDustIndex1].noGravity = false;
                                    Main.dust[sVeilDustIndex1].noLight = false;
                                    Main.dust[sVeilDustIndex2].noGravity = false;
                                    Main.dust[sVeilDustIndex2].noLight = false;
                                }

                                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SilvaDispel"), (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);
                            }
                            break;
                        }
                    }
                }
                for (int i = 0; i < 1000; i++)
                {
                    if (Main.projectile[i].active && !Main.projectile[i].friendly && Main.projectile[i].hostile && Main.projectile[i].damage > 0)
                    {
                        Projectile proj = Main.projectile[i];
                        Rectangle rect = proj.getRect();
                        if (sVeilRectangle.Intersects(rect))
                        {
                            if (player.whoAmI == Main.myPlayer)
                            {
                                player.noKnockback = true;
                                rogueStealth = rogueStealthMax;
                                spectralVeilImmunity = 0;

                                for (int k = 0; k < player.hurtCooldowns.Length; k++)
                                {
                                    player.hurtCooldowns[k] = player.immuneTime;
                                }

                                Vector2 sVeilDustDir = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                                sVeilDustDir.Normalize();
                                sVeilDustDir *= 0.5f;
                                for (int j = 0; j < 20; j++)
                                {
                                    int sVeilDustIndex1 = Dust.NewDust(player.Center, 1, 1, 21, sVeilDustDir.X * j, sVeilDustDir.Y * j);
                                    int sVeilDustIndex2 = Dust.NewDust(player.Center, 1, 1, 21, -sVeilDustDir.X * j, -sVeilDustDir.Y * j);
                                    Main.dust[sVeilDustIndex1].noGravity = false;
                                    Main.dust[sVeilDustIndex1].noLight = false;
                                    Main.dust[sVeilDustIndex2].noGravity = false;
                                    Main.dust[sVeilDustIndex2].noLight = false;
                                }

                                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SilvaDispel"), (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);
                            }
                            break;
                        }
                    }
                }
            }
            if (plaguedFuelPack && plaguedFuelPackDash > 0)
            {
                if (plaguedFuelPackDash > 1)
                    player.velocity = new Vector2(plaguedFuelPackDirection, -1) * 25;
                else
                    player.velocity = new Vector2(plaguedFuelPackDirection, -1) * 5;

                int numClouds = Main.rand.Next(2, 10);
                for (int i = 0; i < numClouds; i++)
                {
                    Vector2 cloudVelocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                    cloudVelocity.Normalize();
                    cloudVelocity *= Main.rand.NextFloat(0f, 1f);
                    int projectile = Projectile.NewProjectile(player.Center, cloudVelocity, ModContent.ProjectileType<PlaguedFuelPackCloud>(), 20, 0, player.whoAmI, 0, 0);
                    Main.projectile[projectile].timeLeft = Main.rand.Next(75, 125);
                }
                for (int i = 0; i < 3; i++)
                {
                    int dust = Dust.NewDust(player.Center, 1, 1, 89, player.velocity.X * -0.1f, player.velocity.Y * -0.1f, 100, default, 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity.Y -= 0.15f;
                }
            }
            #endregion

            #region StandingStillEffects
            UpdateRogueStealth();

            if (trinketOfChi)
            {
                if (trinketOfChiBuff)
                {
                    player.allDamage += 0.5f;
                    if (player.itemAnimation > 0)
                        chiBuffTimer = 0;
                }
                if ((double)Math.Abs(player.velocity.X) < 0.1 && (double)Math.Abs(player.velocity.Y) < 0.1 && !player.mount.Active)
                {
                    if (chiBuffTimer < 120)
                        chiBuffTimer++;
                    else
                        player.AddBuff(ModContent.BuffType<ChiBuff>(), 6);
                }
                else
                    chiBuffTimer--;
            }
            else
                chiBuffTimer = 0;

            if (aquaticEmblem)
            {
                if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir) && player.wet && !player.lavaWet && !player.honeyWet &&
                    !player.mount.Active)
                {
                    if (aquaticBoost > 0f)
                    {
                        aquaticBoost -= 0.0002f; //0.015
                        if ((double)aquaticBoost <= 0.0)
                        {
                            aquaticBoost = 0f;
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                NetMessage.SendData(84, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                        }
                    }
                }
                else
                {
                    aquaticBoost += 0.0002f;
                    if (aquaticBoost > 1f)
                        aquaticBoost = 1f;
                    if (player.mount.Active)
                        aquaticBoost = 1f;
                }
                player.statDefense += (int)((1f - aquaticBoost) * 30f);
                player.moveSpeed -= (1f - aquaticBoost) * 0.1f;
            }
            else
                aquaticBoost = 1f;

            if (auricBoost)
            {
                if (player.itemAnimation > 0)
                {
                    modStealthTimer = 5;
                }
                if ((double)Math.Abs(player.velocity.X) < 0.1 && (double)Math.Abs(player.velocity.Y) < 0.1 && !player.mount.Active)
                {
                    if (modStealthTimer == 0 && modStealth > 0f)
                    {
                        modStealth -= 0.015f;
                        if ((double)modStealth <= 0.0)
                        {
                            modStealth = 0f;
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                NetMessage.SendData(84, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                        }
                    }
                }
                else
                {
                    float num27 = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
                    modStealth += num27 * 0.0075f;
                    if (modStealth > 1f)
                        modStealth = 1f;
                    if (player.mount.Active)
                        modStealth = 1f;
                }
                float damageBoost = (1f - modStealth) * 0.2f;
                player.allDamage += damageBoost;
                int critBoost = (int)((1f - modStealth) * 10f);
                AllCritBoost(critBoost);
                if (modStealthTimer > 0)
                    modStealthTimer--;
            }
            else if (pAmulet)
            {
                if (player.itemAnimation > 0)
                {
                    modStealthTimer = 5;
                }
                if ((double)Math.Abs(player.velocity.X) < 0.1 && (double)Math.Abs(player.velocity.Y) < 0.1 && !player.mount.Active)
                {
                    if (modStealthTimer == 0 && modStealth > 0f)
                    {
                        modStealth -= 0.015f;
                        if ((double)modStealth <= 0.0)
                        {
                            modStealth = 0f;
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                NetMessage.SendData(84, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                        }
                    }
                }
                else
                {
                    float num27 = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
                    modStealth += num27 * 0.0075f;
                    if (modStealth > 1f)
                        modStealth = 1f;
                    if (player.mount.Active)
                        modStealth = 1f;
                }
                player.Calamity().throwingDamage += (1f - modStealth) * 0.2f;
                player.Calamity().throwingCrit += (int)((1f - modStealth) * 10f);
                player.aggro -= (int)((1f - modStealth) * 750f);
                if (modStealthTimer > 0)
                    modStealthTimer--;
            }
            else
                modStealth = 1f;
            #endregion

            #region ElysianAegis
            if (elysianAegis)
            {
                bool flag14 = false;
                if (elysianGuard)
                {
					if (player.whoAmI == Main.myPlayer)
					{
						player.AddBuff(ModContent.BuffType<ElysianGuard>(), 2, false);
					}
					float num29 = shieldInvinc;
                    shieldInvinc -= 0.08f;
                    if (shieldInvinc < 0f)
                    {
                        shieldInvinc = 0f;
                    }
                    else
                    {
                        flag14 = true;
                    }
                    if (shieldInvinc == 0f && num29 != shieldInvinc && Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(84, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                    }
                    float damageBoost = (5f - shieldInvinc) * 0.03f;
                    player.allDamage += damageBoost;
                    int critBoost = (int)((5f - shieldInvinc) * 2f);
                    AllCritBoost(critBoost);
                    player.aggro += (int)((5f - shieldInvinc) * 220f);
                    player.statDefense += (int)((5f - shieldInvinc) * 4f);
                    player.moveSpeed *= 0.85f;
                    if (player.mount.Active)
                    {
                        elysianGuard = false;
                    }
                }
                else
                {
                    float num30 = shieldInvinc;
                    shieldInvinc += 0.08f;
                    if (shieldInvinc > 5f)
                    {
                        shieldInvinc = 5f;
                    }
                    else
                    {
                        flag14 = true;
                    }
                    if (shieldInvinc == 5f && num30 != shieldInvinc && Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(84, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
                if (flag14)
                {
                    if (Main.rand.NextBool(2))
                    {
                        Vector2 vector = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
                        Dust dust = Main.dust[Dust.NewDust(player.Center - vector * 30f, 0, 0, 244, 0f, 0f, 0, default, 1f)];
                        dust.noGravity = true;
                        dust.position = player.Center - vector * (float)Main.rand.Next(5, 11);
                        dust.velocity = vector.RotatedBy(1.5707963705062866, default) * 4f;
                        dust.scale = 0.5f + Main.rand.NextFloat();
                        dust.fadeIn = 0.5f;
                    }
                    if (Main.rand.NextBool(2))
                    {
                        Vector2 vector2 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
                        Dust dust2 = Main.dust[Dust.NewDust(player.Center - vector2 * 30f, 0, 0, 246, 0f, 0f, 0, default, 1f)];
                        dust2.noGravity = true;
                        dust2.position = player.Center - vector2 * 12f;
                        dust2.velocity = vector2.RotatedBy(-1.5707963705062866, default) * 2f;
                        dust2.scale = 0.5f + Main.rand.NextFloat();
                        dust2.fadeIn = 0.5f;
                    }
                }
            }
            else
            {
                elysianGuard = false;
            }
            #endregion

            #region OtherBuffs
            if (gravityNormalizer)
            {
                player.buffImmune[BuffID.VortexDebuff] = true;
                float x = (float)(Main.maxTilesX / 4200);
                x *= x;
                float spaceGravityMult = (float)((double)(player.position.Y / 16f - (60f + 10f * x)) / (Main.worldSurface / 6.0));
                if (spaceGravityMult < 1f)
                {
                    player.gravity = Player.defaultGravity;
                    if (player.wet)
                    {
                        if (player.honeyWet)
                            player.gravity = 0.1f;
                        else if (player.merman)
                            player.gravity = 0.3f;
                        else
                            player.gravity = 0.2f;
                    }
                }
            }
            if (astralInjection)
            {
                if (player.statMana < player.statManaMax2)
                {
                    player.statMana += 3;
                }
                if (player.statMana > player.statManaMax2)
                {
                    player.statMana = player.statManaMax2;
                }
            }
            if (armorCrumbling)
            {
                player.Calamity().throwingCrit += 5;
                player.meleeCrit += 5;
            }
            if (armorShattering)
            {
                if (player.FindBuffIndex(ModContent.BuffType<ArmorCrumbling>()) > -1)
                { player.ClearBuff(ModContent.BuffType<ArmorCrumbling>()); }
                player.Calamity().throwingDamage += 0.08f;
                player.meleeDamage += 0.08f;
                player.Calamity().throwingCrit += 8;
                player.meleeCrit += 8;
            }
            if (holyWrath)
            {
                if (player.FindBuffIndex(BuffID.Wrath) > -1)
                { player.ClearBuff(BuffID.Wrath); }
                player.allDamage += 0.12f;
                player.moveSpeed += 0.05f;
            }
            if (profanedRage)
            {
                if (player.FindBuffIndex(BuffID.Rage) > -1)
                { player.ClearBuff(BuffID.Rage); }
                AllCritBoost(12);
                player.moveSpeed += 0.05f;
            }
            if (irradiated)
            {
                player.statDefense -= 10;
                player.allDamage += 0.05f;
                player.minionKB += 0.5f;
                player.moveSpeed += 0.05f;
            }
            if (rRage)
            {
                player.allDamage += 0.05f;
                player.moveSpeed += 0.05f;
            }
            if (xRage)
            {
                player.allDamage += 0.1f;
            }
            if (xWrath)
            {
                AllCritBoost(5);
            }
            if (godSlayerCooldown)
            {
                player.allDamage += 0.1f;
            }
            if (graxDefense)
            {
                player.statDefense += 30;
                player.endurance += 0.1f;
                player.meleeDamage += 0.2f;
            }
            if (sMeleeBoost)
            {
                player.allDamage += 0.1f;
                AllCritBoost(5);
            }
            if (tFury)
            {
                player.meleeDamage += 0.3f;
                player.meleeCrit += 10;
            }
            if (yPower)
            {
                player.endurance += 0.05f;
                player.statDefense += 5;
                player.allDamage += 0.06f;
                AllCritBoost(2);
                player.minionKB += 1f;
                player.moveSpeed += 0.15f;
            }
            if (tScale)
            {
                player.endurance += 0.05f;
                player.statDefense += 5;
                player.kbBuff = true;
            }
            if (darkSunRing)
            {
                player.maxMinions += 2;
                player.allDamage += 0.12f;
                player.minionKB += 1.2f;
                player.pickSpeed -= 0.15f;
                if (!Main.dayTime)
                {
                    player.statDefense += 30;
                }
            }
            if (eGauntlet)
            {
                player.longInvince = true;
                player.kbGlove = true;
                player.meleeDamage += 0.15f;
                player.meleeCrit += 5;
                player.lavaMax += 240;
            }
            if (fabsolVodka)
            {
                player.allDamage += 0.08f;
                player.statDefense -= 20;
            }
            if (vodka)
            {
                player.statDefense -= 4;
                player.allDamage += 0.06f;
                AllCritBoost(2);
            }
            if (grapeBeer)
            {
                player.statDefense -= 2;
                player.moveSpeed -= 0.05f;
            }
            if (moonshine)
            {
                player.statDefense += 10;
                player.endurance += 0.05f;
            }
            if (rum)
            {
                player.moveSpeed += 0.1f;
                player.statDefense -= 8;
            }
            if (whiskey)
            {
                player.statDefense -= 8;
                player.allDamage += 0.04f;
                AllCritBoost(2);
            }
            if (everclear)
            {
                player.statDefense -= 40;
                player.allDamage += 0.25f;
            }
            if (bloodyMary)
            {
                if (Main.bloodMoon)
                {
                    player.statDefense -= 6;
                    player.allDamage += 0.15f;
                    AllCritBoost(7);
                    player.moveSpeed += 0.15f;
                }
            }
            if (tequila)
            {
                if (Main.dayTime)
                {
                    player.statDefense += 5;
                    player.allDamage += 0.03f;
                    AllCritBoost(2);
                    player.endurance += 0.03f;
                }
            }
            if (tequilaSunrise)
            {
                if (Main.dayTime)
                {
                    player.statDefense += 15;
                    player.allDamage += 0.07f;
                    AllCritBoost(3);
                    player.endurance += 0.07f;
                }
            }
            if (caribbeanRum)
            {
                player.moveSpeed += 0.2f;
                player.statDefense -= 12;
            }
            if (cinnamonRoll)
            {
                player.statDefense -= 12;
                player.manaRegenDelay--;
                player.manaRegenBonus += 10;
            }
            if (margarita)
            {
                player.statDefense -= 6;
            }
            if (starBeamRye)
            {
                player.statDefense -= 6;
                player.magicDamage += 0.08f;
                player.manaCost *= 0.9f;
            }
            if (moscowMule)
            {
                player.allDamage += 0.09f;
                AllCritBoost(3);
            }
            if (whiteWine)
            {
                player.statDefense -= 6;
                player.magicDamage += 0.1f;
            }
            if (evergreenGin)
            {
                player.endurance += 0.05f;
            }
            if (giantPearl)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int m = 0; m < 200; m++)
                    {
                        if (Main.npc[m].active && !Main.npc[m].friendly)
                        {
                            float distance = (Main.npc[m].Center - player.Center).Length();
                            if (distance < 120f)
                                Main.npc[m].AddBuff(ModContent.BuffType<PearlAura>(), 20, false);
                        }
                    }
                }
            }
            if (CalamityMod.scopedWeaponList.Contains(player.inventory[player.selectedItem].type))
            {
                player.scope = true;
            }
            if (CalamityMod.highTestFishList.Contains(player.inventory[player.selectedItem].type))
            {
                player.accFishingLine = true;
            }
			if (CalamityMod.boomerangList.Contains(player.inventory[player.selectedItem].type) && player.invis)
            {
				player.Calamity().throwingDamage += 0.1f;
            }
            if (CalamityMod.javelinList.Contains(player.inventory[player.selectedItem].type) && player.invis)
            {
                player.armorPenetration += 5;
            }
            if (CalamityMod.flaskBombList.Contains(player.inventory[player.selectedItem].type) && player.invis)
            {
				player.Calamity().throwingVelocity += 0.1f;
            }
            if (CalamityMod.spikyBallList.Contains(player.inventory[player.selectedItem].type) && player.invis)
            {
				player.Calamity().throwingCrit += 10;
            }
			if (etherealExtorter)
			{
				bool ZoneForest = !ZoneAbyss && !ZoneSulphur && !ZoneAstral && !ZoneCalamity && !ZoneSunkenSea && !player.ZoneSnow && !player.ZoneCorrupt && !player.ZoneCrimson && !player.ZoneHoly && !player.ZoneDesert && !player.ZoneUndergroundDesert && !player.ZoneGlowshroom && !player.ZoneDungeon && !player.ZoneBeach && !player.ZoneMeteor;
				if (player.ZoneUnderworldHeight && !ZoneCalamity && CalamityMod.fireWeaponList.Contains(player.inventory[player.selectedItem].type))
				{
                    player.endurance += 0.03f;
				}
				if ((player.ZoneDesert || player.ZoneUndergroundDesert) && CalamityMod.daggerList.Contains(player.inventory[player.selectedItem].type))
				{
					player.scope = true;
				}
				if (ZoneSunkenSea)
				{
					player.gills = true;
					player.ignoreWater = true;
				}
				if (player.ZoneSnow && CalamityMod.iceWeaponList.Contains(player.inventory[player.selectedItem].type))
				{
					player.statDefense += 5;
				}
				if (ZoneAstral)
				{
					if (player.wingTimeMax > 0)
						player.wingTimeMax = (int)((double)player.wingTimeMax * 1.05);
				}
				if (player.ZoneJungle && CalamityMod.natureWeaponList.Contains(player.inventory[player.selectedItem].type))
				{
                    player.AddBuff(165, 5, true); //Dryad's Blessing
				}
				if (ZoneAbyss)
				{
                    player.blind = true;
                    player.headcovered = true;
                    player.blackout = true;
					if (player.FindBuffIndex(BuffID.Shine) > -1)
					{ player.ClearBuff(BuffID.Shine); }
					if (player.FindBuffIndex(BuffID.NightOwl) > -1)
					{ player.ClearBuff(BuffID.NightOwl); }
					player.nightVision = false;
					shine = false;
					player.allDamage += 0.2f;
					AllCritBoost(5);
					player.statDefense += 8;
					player.endurance += 0.05f;
				}
				if (player.ZoneRockLayerHeight && ZoneForest && CalamityMod.flaskBombList.Contains(player.inventory[player.selectedItem].type))
				{
					player.blackBelt = true;
				}
				if (player.ZoneHoly)
				{
					player.maxMinions += 1;
					player.manaCost *= 0.9f;
					player.ammoCost75 = true; //25% chance to not use ranged ammo
					throwingAmmoCost75 = true; //25% chance to not consume rogue consumables
				}
				if (player.ZoneBeach)
				{
					player.moveSpeed += 0.05f;
				}
				if (player.ZoneGlowshroom)
				{
					player.statDefense += 3;
				}
				if (player.ZoneMeteor)
				{
					gravityNormalizer = true;
					player.slowFall = true;
				}
				if (Main.moonPhase == 0) //full moon
				{
					player.fishingSkill += 30;
				}
				if (Main.moonPhase == 6) //first quarter
				{
					player.discount = true;
				}
			}
            if (harpyRing)
            {
                if (player.wingTimeMax > 0)
                    player.wingTimeMax = (int)((double)player.wingTimeMax * 1.25);
                player.moveSpeed += 0.2f;
            }
            if (blueCandle)
            {
                if (player.wingTimeMax > 0)
                    player.wingTimeMax = (int)((double)player.wingTimeMax * 1.1);
                player.moveSpeed += 0.15f;
            }
            if (plaguebringerGoliathLore)
            {
                if (player.wingTimeMax > 0)
                    player.wingTimeMax = (int)((double)player.wingTimeMax * 1.25);
            }
            if (soaring)
            {
                if (player.wingTimeMax > 0)
                    player.wingTimeMax = (int)((double)player.wingTimeMax * 1.1);
            }
            if (draconicSurge)
            {
                if (player.wingTimeMax > 0)
                    player.wingTimeMax = (int)((double)player.wingTimeMax * 1.35);
            }
            if (bounding)
            {
                player.jumpSpeedBoost += 0.5f;
                Player.jumpHeight += 10;
                player.extraFall += 25;
            }

			if (mushy)
				player.statDefense += 5;

			if (omniscience)
			{
				player.detectCreature = true;
				player.dangerSense = true;
				player.findTreasure = true;
			}

			if (aWeapon)
				player.moveSpeed += 0.15f;

			if (molten)
				player.resistCold = true;

			if (shellBoost)
				player.moveSpeed += 0.9f;

			if (tarraSet)
			{
				player.calmed = tarraMelee ? false : true;
				player.lifeMagnet = true;
			}

			if (aChicken)
			{
				player.statDefense += 5;
				player.moveSpeed += 0.1f;
			}

			if (cadence)
			{
				if (player.FindBuffIndex(BuffID.Regeneration) > -1)
				{ player.ClearBuff(BuffID.Regeneration); }
				if (player.FindBuffIndex(BuffID.Lifeforce) > -1)
				{ player.ClearBuff(BuffID.Lifeforce); }
				player.discount = true;
				player.lifeMagnet = true;
				player.calmed = true;
				player.loveStruck = true;
				player.statLifeMax2 += player.statLifeMax / 5 / 20 * 25;
			}

			if (community)
            {
                float floatTypeBoost = 0.01f +
                    (NPC.downedSlimeKing ? 0.01f : 0f) +
                    (NPC.downedBoss1 ? 0.01f : 0f) +
                    (NPC.downedBoss2 ? 0.01f : 0f) +
                    (NPC.downedQueenBee ? 0.01f : 0f) + //0.05
                    (NPC.downedBoss3 ? 0.01f : 0f) +
                    (Main.hardMode ? 0.01f : 0f) +
                    (NPC.downedMechBossAny ? 0.01f : 0f) +
                    (NPC.downedPlantBoss ? 0.01f : 0f) +
                    (NPC.downedGolemBoss ? 0.01f : 0f) + //0.1
                    (NPC.downedFishron ? 0.01f : 0f) +
                    (NPC.downedAncientCultist ? 0.01f : 0f) +
                    (NPC.downedMoonlord ? 0.01f : 0f) +
                    (CalamityWorld.downedProvidence ? 0.02f : 0f) + //0.15
                    (CalamityWorld.downedDoG ? 0.02f : 0f) + //0.17
                    (CalamityWorld.downedYharon ? 0.03f : 0f); //0.2
                int integerTypeBoost = (int)(floatTypeBoost * 50f);
                int critBoost = integerTypeBoost / 2;
                float damageBoost = floatTypeBoost * 0.5f;
                player.endurance += floatTypeBoost * 0.25f;
                player.statDefense += integerTypeBoost;
                player.allDamage += damageBoost;
                AllCritBoost(critBoost);
                player.minionKB += floatTypeBoost;
                player.moveSpeed += floatTypeBoost;
                player.statLifeMax2 += player.statLifeMax / 5 / 20 * integerTypeBoost;
                if (player.wingTimeMax > 0)
                    player.wingTimeMax = (int)((double)player.wingTimeMax * 1.15);
            }

            if (ravagerLore)
            {
                if (player.wingTimeMax > 0)
                    player.wingTimeMax = (int)((double)player.wingTimeMax * 0.5);
                player.allDamage += 0.1f;
            }

			if (wDeath)
			{
				player.statDefense -= WhisperingDeath.DefenseReduction;
				player.allDamage -= 0.1f;
			}

			if (aFlames)
				player.statDefense -= AbyssalFlames.DefenseReduction;

			if (gsInferno)
				player.statDefense -= GodSlayerInferno.DefenseReduction;

			if (astralInfection)
				player.statDefense -= AstralInfectionDebuff.DefenseReduction;

			if (pFlames)
			{
				player.blind = true;
				player.statDefense -= Plague.DefenseReduction;
				player.moveSpeed -= 0.15f;
			}

			if (bBlood)
			{
				player.blind = true;
				player.statDefense -= 3;
				player.moveSpeed += 0.2f;
				player.meleeDamage += 0.05f;
				player.rangedDamage -= 0.1f;
				player.magicDamage -= 0.1f;
			}

			if (horror)
			{
				player.blind = true;
				player.statDefense -= 15;
				player.moveSpeed -= 0.15f;
			}

			if (aCrunch)
			{
				player.statDefense -= ArmorCrunch.DefenseReduction;
				player.endurance *= 0.33f;
			}

			if (vHex)
			{
				player.blind = true;
				player.statDefense -= 30;
				player.moveSpeed -= 0.1f;

				if (player.wingTimeMax <= 0)
					player.wingTimeMax = 0;

				player.wingTimeMax /= 2;
			}

			if (gState)
			{
				player.statDefense -= GlacialState.DefenseReduction;
				player.velocity.Y = 0f;
				player.velocity.X = 0f;
			}

			if (eGravity)
			{
				if (player.wingTimeMax < 0)
					player.wingTimeMax = 0;

				if (player.wingTimeMax > 400)
					player.wingTimeMax = 400;

				player.wingTimeMax /= 4;
			}

			if (eGrav)
			{
				if (player.wingTimeMax < 0)
					player.wingTimeMax = 0;

				if (player.wingTimeMax > 400)
					player.wingTimeMax = 400;

				player.wingTimeMax /= 2;
			}

			if (molluskSet)
				player.velocity.X *= 0.985f;

			if ((warped || caribbeanRum) && !player.slowFall && !player.mount.Active)
				player.velocity.Y *= 1.01f;

			if (corrEffigy)
            {
                player.moveSpeed += 0.15f;
                AllCritBoost(10);
            }

            if (crimEffigy)
            {
                player.allDamage += 0.15f;
                player.statDefense += 10;
                player.statLifeMax2 = (int)((double)player.statLifeMax2 * 0.8);
            }

            if (badgeOfBraveryRare)
            {
                player.meleeDamage += 0.2f;
                player.statLifeMax2 = (int)((double)player.statLifeMax2 * 0.75);
            }

            if (regenator)
                player.statLifeMax2 = (int)((double)player.statLifeMax2 * 0.5);

            if (calamitasLore)
            {
                player.statLifeMax2 = (int)((double)player.statLifeMax2 * 0.75);
                player.maxMinions += 2;
            }

			// The player's true max life value with Calamity adjustments
			actualMaxLife = player.statLifeMax2;

            if (thirdSageH && !player.dead && player.HasBuff(ModContent.BuffType<ThirdSageBuff>()))
            {
                player.statLife = player.statLifeMax2;
            }

            if (pinkCandle)
            {
                // every frame, add up 1/60th of the healing value (0.4% max HP per second)
                pinkCandleHealFraction += player.statLifeMax2 * 0.004 / 60;
                if (pinkCandleHealFraction >= 1D)
                {
                    pinkCandleHealFraction = 0D;
                    if (player.statLife < player.statLifeMax2)
                        player.statLife++;
                }
            }
            else
                pinkCandleHealFraction = 0D;

            if (manaOverloader)
            {
                player.magicDamage += 0.06f;
                if (player.statMana < (int)((double)player.statManaMax2 * 0.1))
                    player.ghostHeal = true;
            }

            if (rBrain)
            {
                if (player.statLife <= (int)((double)player.statLifeMax2 * 0.75))
                    player.allDamage += 0.1f;
                if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
                    player.moveSpeed -= 0.05f;
            }

            if (bloodyWormTooth)
            {
                if (player.statLife < (int)((double)player.statLifeMax2 * 0.5))
                {
                    player.meleeDamage += 0.1f;
                    player.meleeSpeed += 0.1f;
                    player.endurance += 0.1f;
                }
                else
                {
                    player.meleeDamage += 0.05f;
                    player.meleeSpeed += 0.05f;
                    player.endurance += 0.05f;
                }
            }

            if (dAmulet)
            {
                player.panic = true;
                player.pStone = true;
                player.armorPenetration += 10;
                if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
                {
                    Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 1.35f, 0.3f, 0.9f);
                }
            }

            if (rampartOfDeities)
            {
                player.armorPenetration += 10;
                player.noKnockback = true;
                if (player.statLife > (int)((double)player.statLifeMax2 * 0.25))
                {
                    player.hasPaladinShield = true;
                    if (player.whoAmI != Main.myPlayer && player.miscCounter % 10 == 0)
                    {
                        int myPlayer = Main.myPlayer;
                        if (Main.player[myPlayer].team == player.team && player.team != 0)
                        {
                            float arg = player.position.X - Main.player[myPlayer].position.X;
                            float num3 = player.position.Y - Main.player[myPlayer].position.Y;
                            if ((float)Math.Sqrt((double)(arg * arg + num3 * num3)) < 800f)
                            {
                                Main.player[myPlayer].AddBuff(43, 20, true);
                            }
                        }
                    }
                }
                if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
                {
                    player.AddBuff(62, 5, true);
                }
                if (player.statLife <= (int)((double)player.statLifeMax2 * 0.15))
                {
                    player.endurance += 0.05f;
                }
            }
            else if (fBulwark)
            {
                player.noKnockback = true;
                if (player.statLife > (int)((double)player.statLifeMax2 * 0.25))
                {
                    player.hasPaladinShield = true;
                    if (player.whoAmI != Main.myPlayer && player.miscCounter % 10 == 0)
                    {
                        int myPlayer = Main.myPlayer;
                        if (Main.player[myPlayer].team == player.team && player.team != 0)
                        {
                            float arg = player.position.X - Main.player[myPlayer].position.X;
                            float num3 = player.position.Y - Main.player[myPlayer].position.Y;
                            if ((float)Math.Sqrt((double)(arg * arg + num3 * num3)) < 800f)
                            {
                                Main.player[myPlayer].AddBuff(43, 20, true);
                            }
                        }
                    }
                }
                if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
                {
                    player.AddBuff(62, 5, true);
                }
                if (player.statLife <= (int)((double)player.statLifeMax2 * 0.15))
                {
                    player.endurance += 0.05f;
                }
            }

            if (frostFlare)
            {
                player.resistCold = true;
                player.buffImmune[44] = true;
                player.buffImmune[46] = true;
                player.buffImmune[47] = true;
                if (player.statLife > (int)((double)player.statLifeMax2 * 0.75))
                {
                    player.allDamage += 0.1f;
                }
                if (player.statLife < (int)((double)player.statLifeMax2 * 0.25))
                {
                    player.statDefense += 10;
                }
            }

            if (vexation)
            {
                if (player.statLife < (int)((double)player.statLifeMax2 * 0.5))
                {
                    player.allDamage += 0.15f;
                }
            }

            if (ataxiaBlaze)
            {
                if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
                {
                    player.AddBuff(BuffID.Inferno, 2);
                }
            }

            if (bloodflareThrowing)
            {
                if (player.statLife > (int)((double)player.statLifeMax2 * 0.8))
                {
                    player.Calamity().throwingCrit += 5;
                    player.statDefense += 30;
                }
                else
                {
                    player.Calamity().throwingDamage += 0.1f;
                }
            }

            if (bloodflareSummon)
            {
                if (player.statLife >= (int)((double)player.statLifeMax2 * 0.9))
                {
                    player.minionDamage += 0.1f;
                }
                else if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
                {
                    player.statDefense += 20;
                }
                if (bloodflareSummonTimer > 0)
                { bloodflareSummonTimer--; }
                if (player.whoAmI == Main.myPlayer && bloodflareSummonTimer <= 0)
                {
                    bloodflareSummonTimer = 900;
                    for (int I = 0; I < 3; I++)
                    {
                        float ai1 = (float)(I * 120);
                        Projectile.NewProjectile(player.Center.X + (float)(Math.Sin(I * 120) * 550), player.Center.Y + (float)(Math.Cos(I * 120) * 550), 0f, 0f,
                            ModContent.ProjectileType<GhostlyMine>(), (int)((auricSet ? 15000f : 5000f) * player.minionDamage), 1f, player.whoAmI, ai1, 0f);
                    }
                }
            }

            if (yInsignia)
            {
                player.longInvince = true;
                player.kbGlove = true;
                player.meleeDamage += 0.05f;
                player.lavaMax += 240;
                if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
                {
                    player.allDamage += 0.1f;
                }
            }

            if (reaperToothNecklace)
            {
                player.allDamage += 0.25f;
                player.statDefense /= 2;
            }

            if (deepDiver)
            {
                player.allDamage += 0.15f;
                player.statDefense += (int)((double)player.statDefense * 0.15);
                player.moveSpeed += 0.15f;
            }

            if (coreOfTheBloodGod)
            {
                player.endurance += 0.05f;
                player.allDamage += 0.07f;
                if (player.statDefense < 100)
                {
                    player.allDamage += 0.15f;
                }
            }
            else if (bloodflareCore)
            {
                if (player.statDefense < 100)
                {
                    player.allDamage += 0.15f;
                }
                if (player.statLife <= (int)((double)player.statLifeMax2 * 0.15))
                {
                    player.endurance += 0.1f;
                    player.allDamage += 0.2f;
                }
                else if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
                {
                    player.endurance += 0.05f;
                    player.allDamage += 0.1f;
                }
            }

            if (godSlayerThrowing)
            {
                if (player.statLife >= player.statLifeMax2)
                {
                    player.Calamity().throwingCrit += 10;
                    player.Calamity().throwingDamage += 0.1f;
                    player.Calamity().throwingVelocity += 0.1f;
                }
            }

            if (tarraSummon)
            {
                int lifeCounter = 0;
                float num2 = 300f;
                bool flag = lifeCounter % 60 == 0;
                int num3 = 200;
                if (player.whoAmI == Main.myPlayer)
                {
                    for (int l = 0; l < 200; l++)
                    {
                        NPC nPC = Main.npc[l];
                        if (nPC.active && !nPC.friendly && nPC.damage > 0 && !nPC.dontTakeDamage && Vector2.Distance(player.Center, nPC.Center) <= num2)
                        {
                            if (flag)
                            {
                                nPC.StrikeNPC(num3, 0f, 0, false, false, false);
                                if (Main.netMode != NetmodeID.SinglePlayer)
                                {
                                    NetMessage.SendData(28, -1, -1, null, l, (float)num3, 0f, 0f, 0, 0, 0);
                                }
                            }
                        }
                    }
                }
                lifeCounter++;
                if (lifeCounter >= 180)
                {
                }
            }

            if (player.inventory[player.selectedItem].type == ModContent.ItemType<NavyFishingRod>() && player.ownedProjectileCounts[ModContent.ProjectileType<NavyBobber>()] != 0)
            {
				int auraCounter = 0;
				float num2 = 200f;
				bool flag = auraCounter % 60 == 0;
				int num3 = 10;
				int random = Main.rand.Next(15);
				if (player.whoAmI == Main.myPlayer)
				{
					if (random == 0)
					{
						for (int l = 0; l < 200; l++)
						{
							NPC nPC = Main.npc[l];
							if (nPC.active && !nPC.friendly && nPC.damage > 0 && !nPC.dontTakeDamage && Vector2.Distance(player.Center, nPC.Center) <= num2)
							{
								if (flag)
								{
									nPC.StrikeNPC(num3, 0f, 0, false, false, false);
									if (Main.netMode != NetmodeID.SinglePlayer)
									{
										NetMessage.SendData(28, -1, -1, null, l, (float)num3, 0f, 0f, 0, 0, 0);
									}
									Vector2 value15 = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
									while (value15.X == 0f && value15.Y == 0f)
									{
										value15 = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
									}
									value15.Normalize();
									value15 *= (float)Main.rand.Next(30, 61) * 0.1f;
									int num17 = Projectile.NewProjectile(nPC.Center.X, nPC.Center.Y, value15.X, value15.Y, ModContent.ProjectileType<EutrophicSpark>(), 5, 0f, player.whoAmI, 0f, 0f);
									Main.projectile[num17].melee = false;
									Main.projectile[num17].localNPCHitCooldown = -1;
								}
							}
						}
					}
				}
				auraCounter++;
				if (auraCounter >= 180)
				{
				}
			}

            if (brimstoneElementalLore && player.inferno)
            {
                int num = ModContent.BuffType<BrimstoneFlames>();
                float num2 = 300f;
                bool flag = player.infernoCounter % 30 == 0;
                int damage = 50;
                if (player.whoAmI == Main.myPlayer)
                {
                    for (int l = 0; l < 200; l++)
                    {
                        NPC nPC = Main.npc[l];
                        if (nPC.active && !nPC.friendly && nPC.damage > 0 && !nPC.dontTakeDamage && Vector2.Distance(player.Center, nPC.Center) <= num2)
                        {
                            if (nPC.FindBuffIndex(num) == -1 && !nPC.buffImmune[num])
                            {
                                nPC.AddBuff(num, 120, false);
                            }
                            if (flag)
                            {
                                player.ApplyDamageToNPC(nPC, damage, 0f, 0, false);
                            }
                        }
                    }
                }
            }

            if (royalGel)
            {
                player.npcTypeNoAggro[ModContent.NPCType<AeroSlime>()] = true;
                player.npcTypeNoAggro[ModContent.NPCType<BloomSlime>()] = true;
                player.npcTypeNoAggro[ModContent.NPCType<CharredSlime>()] = true;
                player.npcTypeNoAggro[ModContent.NPCType<CrimulanBlightSlime>()] = true;
                player.npcTypeNoAggro[ModContent.NPCType<CryoSlime>()] = true;
                player.npcTypeNoAggro[ModContent.NPCType<EbonianBlightSlime>()] = true;
                player.npcTypeNoAggro[ModContent.NPCType<IrradiatedSlime>()] = true;
                player.npcTypeNoAggro[ModContent.NPCType<PerennialSlime>()] = true;
                player.npcTypeNoAggro[ModContent.NPCType<PlaguedJungleSlime>()] = true;
                player.npcTypeNoAggro[ModContent.NPCType<AstralSlime>()] = true;
                // LATER -- When Wulfrum Slimes start being definitely robots, remove this immunity.
                player.npcTypeNoAggro[ModContent.NPCType<WulfrumSlime>()] = true;
            }

            /*if (dukeScales)
            {
				player.buffImmune[ModContent.BuffType<SulphuricPoisoning>()] = true;
				player.buffImmune[BuffID.Poisoned] = true;
				player.buffImmune[BuffID.Venom] = true;
                if (player.statLife <= (int)((double)player.statLifeMax2 * 0.75))
                {
                    player.allDamage += 0.03f;
					AllCritBoost(3);
                }
                if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
                {
                    player.allDamage += 0.05f;
					AllCritBoost(5);
                }
				if (player.lifeRegen < 0)
                {
                    player.allDamage += 0.1f;
					AllCritBoost(5);
                }
            }*/
            #endregion

            #region LimitsAndOtherShit
            if (player.meleeSpeed < 0.5f)
            {
                player.meleeSpeed = 0.5f;
            }
            if (auricSet && silvaMelee)
            {
                double multiplier = (double)player.statLife / (double)player.statLifeMax2;
                player.meleeDamage += (float)(multiplier * 0.2); //ranges from 1.2 times to 1 times
            }
            // 10% is converted to 9%, 25% is converted to 20%, 50% is converted to 33%, 75% is converted to 43%, 100% is converted to 50%
            player.endurance = 1f - (1f / (1f + player.endurance));
            if (vHex)
            {
                player.endurance -= 0.3f;
            }
            if (irradiated)
            {
                player.endurance -= 0.1f;
            }
            if (corrEffigy)
            {
                player.endurance -= 0.2f;
            }
            if (Config.ProficiencyEnabled)
            {
                GetStatBonuses();
            }
            if (dArtifact)
            {
                player.allDamage += 0.25f;
            }
            if (trippy)
            {
                player.allDamage += 0.5f;
            }
            if (eArtifact)
            {
                player.manaCost *= 0.85f;
                player.Calamity().throwingDamage += 0.15f;
                player.maxMinions += 2;
            }
            if (gArtifact)
            {
                player.maxMinions += 8;
                if (player.whoAmI == Main.myPlayer)
                {
                    if (player.FindBuffIndex(ModContent.BuffType<YharonKindleBuff>()) == -1)
                    {
                        player.AddBuff(ModContent.BuffType<YharonKindleBuff>(), 3600, true);
                    }
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<SonOfYharon>()] < 2)
                    {
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<SonOfYharon>(), (int)(232f * player.minionDamage), 2f, Main.myPlayer, 0f, 0f);
                    }
                }
            }
            if (pArtifact)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    if (player.FindBuffIndex(ModContent.BuffType<GuardianHealer>()) == -1)
                    {
                        player.AddBuff(ModContent.BuffType<GuardianHealer>(), 3600, true);
                    }
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<MiniGuardianHealer>()] < 1)
                    {
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -6f, ModContent.ProjectileType<MiniGuardianHealer>(), 0, 0f, Main.myPlayer, 0f, 0f);
                    }
                    float baseDamage = 100f +
                        (CalamityWorld.downedDoG ? 100f : 0f) +
                        (CalamityWorld.downedYharon ? 100f : 0f);
                    if (player.maxMinions >= 8)
                    {
                        if (player.FindBuffIndex(ModContent.BuffType<GuardianDefense>()) == -1)
                        {
                            player.AddBuff(ModContent.BuffType<GuardianDefense>(), 3600, true);
                        }
                        if (player.ownedProjectileCounts[ModContent.ProjectileType<MiniGuardianDefense>()] < 1)
                        {
                            Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -3f, ModContent.ProjectileType<MiniGuardianDefense>(), (int)(baseDamage * player.minionDamage), 1f, Main.myPlayer, 0f, 0f);
                        }
                    }
                    if (tarraSummon || bloodflareSummon || godSlayerSummon || silvaSummon || dsSetBonus || omegaBlueSet)
                    {
                        if (player.FindBuffIndex(ModContent.BuffType<GuardianOffense>()) == -1)
                        {
                            player.AddBuff(ModContent.BuffType<GuardianOffense>(), 3600, true);
                        }
                        if (player.ownedProjectileCounts[ModContent.ProjectileType<MiniGuardianAttack>()] < 1)
                        {
                            Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<MiniGuardianAttack>(), (int)(baseDamage * player.minionDamage), 1f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }
            }
            if (player.ownedProjectileCounts[ModContent.ProjectileType<BrimstoneElementalMinion>()] > 1 || player.ownedProjectileCounts[ModContent.ProjectileType<WaterElementalMinion>()] > 1 ||
                player.ownedProjectileCounts[ModContent.ProjectileType<SandElementalHealer>()] > 1 || player.ownedProjectileCounts[ModContent.ProjectileType<SandElementalMinion>()] > 1 ||
                player.ownedProjectileCounts[ModContent.ProjectileType<CloudElementalMinion>()] > 1 || player.ownedProjectileCounts[ModContent.ProjectileType<FungalClumpMinion>()] > 1)
            {
                for (int projIndex = 0; projIndex < 1000; projIndex++)
                {
                    if (Main.projectile[projIndex].active && Main.projectile[projIndex].owner == player.whoAmI)
                    {
                        if (Main.projectile[projIndex].type == ModContent.ProjectileType<BrimstoneElementalMinion>() || Main.projectile[projIndex].type == ModContent.ProjectileType<WaterElementalMinion>() ||
                            Main.projectile[projIndex].type == ModContent.ProjectileType<SandElementalHealer>() || Main.projectile[projIndex].type == ModContent.ProjectileType<SandElementalMinion>() ||
                            Main.projectile[projIndex].type == ModContent.ProjectileType<CloudElementalMinion>() || Main.projectile[projIndex].type == ModContent.ProjectileType<FungalClumpMinion>())
                        {
                            Main.projectile[projIndex].Kill();
                        }
                    }
                }
            }
            if (marked || reaperToothNecklace)
            {
                if (player.endurance > 0f)
                    player.endurance *= 0.5f;
            }
            if (yharonLore && !CalamityWorld.defiled)
            {
                if (player.wingTimeMax < 50000)
                    player.wingTimeMax = 50000;
            }

			// For the stat meter
			float allDamageStat = player.allDamage - 1f;
			damageStats[0] = (int)((player.meleeDamage + allDamageStat - 1f) * 100f);
			damageStats[1] = (int)((player.rangedDamage + allDamageStat - 1f) * 100f);
			damageStats[2] = (int)((player.magicDamage + allDamageStat - 1f) * 100f);
			damageStats[3] = (int)((player.minionDamage + allDamageStat - 1f) * 100f);
			damageStats[4] = (int)((player.Calamity().throwingDamage + allDamageStat - 1f) * 100f);
			critStats[0] = player.meleeCrit;
			critStats[1] = player.rangedCrit;
			critStats[2] = player.magicCrit;
			critStats[3] = player.Calamity().throwingCrit;
			defenseStat = player.statDefense;
			DRStat = (int)(player.endurance * 100f);
			meleeSpeedStat = (int)((1f - player.meleeSpeed) * (100f / player.meleeSpeed));
			manaCostStat = (int)(player.manaCost * 100f);
			rogueVelocityStat = (int)((player.Calamity().throwingVelocity - 1f) * 100f);
			minionSlotStat = player.maxMinions;
			manaRegenStat = player.manaRegen;
			armorPenetrationStat = player.armorPenetration;
			moveSpeedStat = (int)((player.moveSpeed - 1f) * 100f);
			wingFlightTimeStat = player.wingTimeMax;
			adrenalineChargeStat = 45 -
				(adrenalineBoostOne ? 10 : 0) -
				(adrenalineBoostTwo ? 10 : 0) -
				(adrenalineBoostThree ? 5 : 0);
			bool DHorHoD = draedonsHeart || heartOfDarkness;
			int rageDamageBoost = 0 +
				(rageBoostOne ? (CalamityWorld.death ? 50 : 15) : 0) +
				(rageBoostTwo ? (CalamityWorld.death ? 50 : 15) : 0) +
				(rageBoostThree ? (CalamityWorld.death ? 50 : 15) : 0);
			rageDamageStat = (CalamityWorld.death ? (DHorHoD ? 200 : 170) : (DHorHoD ? 65 : 50)) + rageDamageBoost; // Death Mode values: 2.3 and 2.0, rev: 0.65 and 0.5

			#endregion

			#region Rogue Mirrors
			Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && !Main.npc[i].friendly && !Main.npc[i].townNPC && Main.npc[i].immune[player.whoAmI] <= 0 && Main.npc[i].damage > 0)
                {
                    NPC nPC = Main.npc[i];
                    Rectangle rect = nPC.getRect();
                    if (rectangle.Intersects(rect) && (nPC.noTileCollide || player.CanHit(nPC)))
                    {
                        if (Main.rand.Next(0, 10) == 0 && player.immuneTime <= 0)
                        {
                            AbyssMirrorEvade();
                            EclipseMirrorEvade();
                        }
                        break;
                    }
                }
            }
            for (int i = 0; i < 1000; i++)
            {
                if (Main.projectile[i].active && !Main.projectile[i].friendly && Main.projectile[i].hostile && Main.projectile[i].damage > 0)
                {
                    Projectile proj = Main.projectile[i];
                    Rectangle rect = proj.getRect();
                    if (rectangle.Intersects(rect))
                    {
                        if (Main.rand.Next(0, 10) == 0)
                        {
                            AbyssMirrorEvade();
                            EclipseMirrorEvade();
                        }
                        break;
                    }
                }
            }
            #endregion
        }

        #region Dragon Scale Logic
        public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (item.type == ModContent.ItemType<DragonScales>() && !CalamityWorld.dragonScalesBought)
            {
                CalamityWorld.dragonScalesBought = true;
            }
        }

        public override bool CanBuyItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (item.type == ModContent.ItemType<DragonScales>())
            {
                return !CalamityWorld.dragonScalesBought;
            }
            return base.CanBuyItem(vendor, shopInventory, item);
        }
        #endregion

        public override void PostUpdateRunSpeeds()
        {
            #region SpeedBoosts
            float runAccMult = 1f +
                (shadowSpeed ? 0.5f : 0f) +
                ((stressPills || laudanum || draedonsHeart) ? 0.05f : 0f) +
                ((abyssalDivingSuit && Collision.DrownCollision(player.position, player.width, player.height, player.gravDir)) ? 0.05f : 0f) +
                (sirenWaterBuff ? 0.15f : 0f) +
                ((frostFlare && player.statLife < (int)((double)player.statLifeMax2 * 0.25)) ? 0.15f : 0f) +
                (auricSet ? 0.1f : 0f) +
                (dragonScales ? 0.1f : 0f) +
                (cTracers ? 0.1f : 0f) +
                (silvaSet ? 0.05f : 0f) +
                (eTracers ? 0.05f : 0f) +
                (blueCandle ? 0.05f : 0f) +
                (etherealExtorter && player.ZoneBeach ? 0.05f : 0f) +
                ((deepDiver && Collision.DrownCollision(player.position, player.width, player.height, player.gravDir)) ? 0.15f : 0f) +
                (rogueStealthMax > 0f ? (rogueStealth >= rogueStealthMax ? rogueStealth * 0.05f : rogueStealth * 0.025f) : 0f);

            float runSpeedMult = 1f +
                (shadowSpeed ? 0.5f : 0f) +
                ((abyssalDivingSuit && Collision.DrownCollision(player.position, player.width, player.height, player.gravDir)) ? 0.05f : 0f) +
                ((frostFlare && player.statLife < (int)((double)player.statLifeMax2 * 0.25)) ? 0.15f : 0f) +
                (sirenWaterBuff ? 0.15f : 0f) +
                (auricSet ? 0.1f : 0f) +
                (dragonScales ? 0.1f : 0f) +
                (cTracers ? 0.1f : 0f) +
                (silvaSet ? 0.05f : 0f) +
                (eTracers ? 0.05f : 0f) +
                (etherealExtorter && player.ZoneBeach ? 0.05f : 0f) +
                ((stressPills || laudanum || draedonsHeart) ? 0.05f : 0f) +
                ((deepDiver && Collision.DrownCollision(player.position, player.width, player.height, player.gravDir)) ? 0.15f : 0f) +
                (rogueStealthMax > 0f ? (rogueStealth >= rogueStealthMax ? rogueStealth * 0.05f : rogueStealth * 0.025f) : 0f);

            if (abyssalDivingSuit && !Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                runAccMult *= 0.4f;
                runSpeedMult *= 0.4f;
            }
            if (horror)
            {
                runAccMult *= 0.85f;
                runSpeedMult *= 0.85f;
            }
            if (fabledTortoise)
            {
                runAccMult *= 0.5f;
                runSpeedMult *= 0.5f;
            }
            if (ursaSergeant)
            {
                runAccMult *= 0.65f;
                runSpeedMult *= 0.65f;
            }
            if (elysianGuard)
            {
                runAccMult *= 0.5f;
                runSpeedMult *= 0.5f;
            }
            if (player.powerrun && CalamityWorld.revenge)
            {
                runSpeedMult *= 0.6666667f;
            }

            player.runAcceleration *= runAccMult;
            player.maxRunSpeed *= runSpeedMult;
            #endregion

            #region DashEffects
            if (player.pulley && dashMod > 0)
            {
                ModDashMovement();
            }
            else if (player.grappling[0] == -1 && !player.tongued)
            {
                ModHorizontalMovement();
                if (dashMod > 0)
                {
                    ModDashMovement();
                }
                if (pAmulet && modStealth < 1f)
                {
                    float num43 = player.maxRunSpeed / 2f * (1f - modStealth);
                    player.maxRunSpeed -= num43;
                    player.accRunSpeed = player.maxRunSpeed;
                }
            }
            #endregion
        }

        #region Rogue Mirrors
        private void AbyssMirrorEvade()
        {
            if (player.whoAmI == Main.myPlayer && abyssalMirror && !abyssalMirrorCooldown)
            {
                player.AddBuff(ModContent.BuffType<AbyssalMirrorCooldown>(), 1200);
                player.immune = true;
                player.immuneTime = player.longInvince ? 100 : 60;
                player.noKnockback = true;
                rogueStealth += 0.5f;

                for (int k = 0; k < player.hurtCooldowns.Length; k++)
                {
                    player.hurtCooldowns[k] = player.immuneTime;
                }

                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SilvaActivation"), (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);

                for (int i = 0; i < 50; i++)
                {
                    int lumenyl = Projectile.NewProjectile(player.Center.X, player.Center.Y, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), ModContent.ProjectileType<AbyssalMirrorProjectile>(), 55, 0, player.whoAmI);
                    Main.projectile[lumenyl].rotation = Main.rand.NextFloat(0, 360);
                    Main.projectile[lumenyl].frame = Main.rand.Next(0, 4);
                }
            }
        }

        private void EclipseMirrorEvade()
        {
            if (player.whoAmI == Main.myPlayer && eclipseMirror && !eclipseMirrorCooldown)
            {
                player.AddBuff(ModContent.BuffType<EclipseMirrorCooldown>(), 1200);
                player.immune = true;
                player.immuneTime = player.longInvince ? 100 : 60;
                player.noKnockback = true;
                rogueStealth = rogueStealthMax;

                for (int k = 0; k < player.hurtCooldowns.Length; k++)
                {
                    player.hurtCooldowns[k] = player.immuneTime;
                }

                Main.PlaySound(2, (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y, 68);
                int eclipseBurst = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<EclipseMirrorBurst>(), 8000, 0, player.whoAmI);
            }
        }
        #endregion

        #endregion

        #region Pre Kill
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (invincible && player.inventory[player.selectedItem].type != ModContent.ItemType<ColdheartIcicle>())
            {
                if (player.statLife <= 0)
                {
                    player.statLife = 1;
                }
                return false;
            }
            if (hInferno)
            {
                for (int x = 0; x < 200; x++)
                {
                    if (Main.npc[x].active && Main.npc[x].type == ModContent.NPCType<Providence>())
                    {
                        Main.npc[x].active = false;
                    }
                }
            }
            if (nCore && Main.rand.NextBool(10))
            {
                Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 67);
                for (int j = 0; j < 25; j++)
                {
                    int num = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 173, 0f, 0f, 100, default, 2f);
                    Dust expr_A4_cp_0 = Main.dust[num];
                    expr_A4_cp_0.position.X += (float)Main.rand.Next(-20, 21);
                    Dust expr_CB_cp_0 = Main.dust[num];
                    expr_CB_cp_0.position.Y += (float)Main.rand.Next(-20, 21);
                    Main.dust[num].velocity *= 0.9f;
                    Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                    Main.dust[num].shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                    }
                }
                player.statLife += 100;
                player.HealEffect(100);
                if (player.statLife > player.statLifeMax2)
                {
                    player.statLife = player.statLifeMax2;
                }
                return false;
            }
            if (godSlayer && !godSlayerCooldown)
            {
                Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 67);
                for (int j = 0; j < 50; j++)
                {
                    int num = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 173, 0f, 0f, 100, default, 2f);
                    Dust expr_A4_cp_0 = Main.dust[num];
                    expr_A4_cp_0.position.X += (float)Main.rand.Next(-20, 21);
                    Dust expr_CB_cp_0 = Main.dust[num];
                    expr_CB_cp_0.position.Y += (float)Main.rand.Next(-20, 21);
                    Main.dust[num].velocity *= 0.9f;
                    Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                    Main.dust[num].shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                    }
                }
                int heal = draconicSurge ? player.statLifeMax2 : 150;
                player.statLife += heal;
                player.HealEffect(heal);
                if (player.statLife > player.statLifeMax2)
                {
                    player.statLife = player.statLifeMax2;
                }
                if (player.FindBuffIndex(ModContent.BuffType<DraconicSurgeBuff>()) > -1)
                {
                    player.ClearBuff(ModContent.BuffType<DraconicSurgeBuff>());
                    draconicSurgeCooldown = 1800;
                }
                player.AddBuff(ModContent.BuffType<GodSlayerCooldown>(), 2700);
                return false;
            }
            if (silvaSet && silvaCountdown > 0)
            {
                if (hasSilvaEffect)
                {
                    silvaHitCounter++;
                }
                if (player.FindBuffIndex(ModContent.BuffType<SilvaRevival>()) == -1)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SilvaActivation"), (int)player.position.X, (int)player.position.Y);
                    player.AddBuff(ModContent.BuffType<SilvaRevival>(), 600);
                    if (draconicSurge)
                    {
                        player.statLife += player.statLifeMax2;
                        player.HealEffect(player.statLifeMax2);
                        if (player.statLife > player.statLifeMax2)
                        {
                            player.statLife = player.statLifeMax2;
                        }
                        if (player.FindBuffIndex(ModContent.BuffType<DraconicSurgeBuff>()) > -1)
                        {
                            player.ClearBuff(ModContent.BuffType<DraconicSurgeBuff>());
                            draconicSurgeCooldown = 1800;
                        }
                    }
                }
                hasSilvaEffect = true;
                if (player.statLife < 1)
                {
                    player.statLife = 1;
                }
                return false;
            }
            if (permafrostsConcoction && player.FindBuffIndex(ModContent.BuffType<ConcoctionCooldown>()) == -1)
            {
                player.AddBuff(ModContent.BuffType<ConcoctionCooldown>(), 10800);
                player.AddBuff(ModContent.BuffType<Encased>(), 300);
                player.statLife = player.statLifeMax2 * 3 / 10;
                Main.PlaySound(SoundID.Item92, player.position);
                for (int i = 0; i < 60; i++)
                {
                    int d = Dust.NewDust(player.position, player.width, player.height, 88, 0f, 0f, 0, default, 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 5f;
                }
                return false;
            }

            //Custom Death Messages
            if (alcoholPoisoning && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                if (Main.rand.Next(2) == 0)
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + " downed too many shots.");
                else
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + "'s liver failed.");
            }
            if (vHex && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + " was charred by the brimstone inferno.");
            }
            if ((ZoneCalamity && player.lavaWet) && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + "'s soul was released by the lava.");
            }
            if (gsInferno && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + "'s soul was extinguished.");
            }
            if (sulphurPoison && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                if (Main.rand.NextBool(2))
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + " was melted by the toxic waste.");
                else
                    damageSource = PlayerDeathReason.ByOther(9);
            }
            if (lethalLavaBurn && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + " disintegrated into ashes.");
            }
            if (hInferno && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + " was turned to ashes by the Profaned Goddess.");
            }
            if (hFlames && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + " fell prey to their sins.");
            }
            if (shadowflame && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + "'s spirit was turned to ash.");
            }
            if (bBlood && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + " became a blood geyser.");
            }
            if (cDepth && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                if (Main.rand.NextBool(2))
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + " was crushed by the pressure.");
                else
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + "'s lungs collapsed.");
            }
            if ((bFlames || aFlames) && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + " was consumed by the black flames.");
            }
            if (pFlames && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                if (Main.rand.NextBool(2))
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + "'s flesh was melted by the plague.");
                else
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + " didn't vaccinate.");
            }
            if (astralInfection && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                if (Main.rand.NextBool(2))
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + "'s infection spread too far.");
                else
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + "'s skin was replaced by the astral virus.");
            }
            if (manaOverloader && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + "'s life was completely converted into mana.");
            }
            if ((bloodyMary || everclear || evergreenGin || fireball || margarita || moonshine || moscowMule || redWine || screwdriver || starBeamRye || tequila || tequilaSunrise || vodka || whiteWine)
                && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + " succumbed to alcohol sickness.");
            }

            if (NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>()))
            {
                if (sCalDeathCount < 51)
                {
                    sCalDeathCount++;
                }
            }
            deathCount++;
            if (player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
            {
                DeathPacket(false);
            }
            if (CalamityWorld.ironHeart && areThereAnyDamnBosses)
            {
                KillPlayer();
                return false;
            }
            return true;
        }
        #endregion

        #region On Respawn
        public override void OnRespawn(Player player)
        {
            thirdSageH = true;
        }
        #endregion

        #region Use Time Mult
        public override float UseTimeMultiplier(Item item)
        {
            if (silvaRanged)
            {
                if (item.ranged && item.useTime > 3)
                    return auricSet ? 1.2f : 1.1f;
            }
            if (silvaThrowing)
            {
                if (player.statLife > (int)((double)player.statLifeMax2 * 0.5) &&
                    item.Calamity().rogue && item.useTime > 3)
                    return 1.1f;
            }
            if (etherealExtorter)
            {
                if (Main.moonPhase == 1 && item.Calamity().rogue && item.useTime > 3) //Waning gibbous
                    return 1.1f;
            }
            return 1f;
        }
        #endregion

        #region Get Weapon Damage And KB
        public override void ModifyWeaponDamage(Item item, ref float add, ref float mult, ref float flat)
        {
            bool isTrueMelee = item.melee && (item.shoot == 0 || (item.noMelee && item.noUseGraphic && item.useStyle == 5 && !CalamityMod.trueMeleeBoostExceptionList.Contains(item.type) && ItemID.Sets.Yoyo[item.type] != true));
            if (isTrueMelee)
            {
				if (tScale)
				{
					player.statDefense += 25;
					player.endurance += 0.1f;
				}

                float damageAdd = (dodgeScarf ? 0.2f : 0f) +
                    ((aBulwarkRare && aBulwarkRareMeleeBoostTimer > 0) ? 2f : 0f) +
                    (DoGLore ? 0.5f : 0f) +
                    (fungalSymbiote ? 0.25f : 0f);

                add += damageAdd;
            }
            if (item.type == ModContent.ItemType<GaelsGreatsword>())
            {
                add += GaelsGreatsword.BaseDamage / (float)GaelsGreatsword.BaseDamage - 1f;
            }
            if (flamethrowerBoost && item.ranged && (item.useAmmo == 23 || item.type == ModContent.ItemType<DragoonDrizzlefish>()))
            {
                add += 0.25f;
            }
            if (cinnamonRoll && CalamityMod.fireWeaponList.Contains(item.type))
            {
                add += 0.15f;
            }
            if (evergreenGin && CalamityMod.natureWeaponList.Contains(item.type))
            {
                add += 0.15f;
            }
            if (fireball && CalamityMod.fireWeaponList.Contains(item.type))
            {
                add += 0.1f;
            }
			if (etherealExtorter && player.ZoneDungeon && item.Calamity().rogue && !item.consumable)
			{
				add += 0.05f;
			}

            if (theBee && player.statLife >= player.statLifeMax2)
            {
                if (item.melee || item.ranged || item.magic || item.Calamity().rogue)
                {
                    double useTimeBeeMultiplier = (double)(item.useTime * item.useAnimation) / 3600D; //28 * 28 = 784 is average so that equals 784 / 3600 = 0.217777 = 21.7% boost
                    if (item.type == ModContent.ItemType<ScarletDevil>())
                    {
                        if (useTimeBeeMultiplier > 0.1)
                            useTimeBeeMultiplier = 0.1;
                    }
                    else
                    {
                        if (useTimeBeeMultiplier > 0.35)
                            useTimeBeeMultiplier = 0.35;
                    }
                    theBeeDamage = (int)((double)item.damage * useTimeBeeMultiplier);
                }
            }
            else
            {
                theBeeDamage = 0;
            }
            if (!theBee)
            {
                theBeeDamage = 0;
            }
        }
        public override void GetWeaponKnockback(Item item, ref float knockback)
        {
            if (auricBoost)
            {
                knockback *= 1f + (1f - modStealth) * 0.5f;
            }
            if (whiskey)
            {
                knockback *= 1.04f;
            }
            if (tequila && Main.dayTime)
            {
                knockback *= 1.03f;
            }
            if (tequilaSunrise && Main.dayTime)
            {
                knockback *= 1.07f;
            }
            if (moscowMule)
            {
                knockback *= 1.09f;
            }
			bool ZoneForest = !ZoneAbyss && !ZoneSulphur && !ZoneAstral && !ZoneCalamity && !ZoneSunkenSea && !player.ZoneSnow && !player.ZoneCorrupt && !player.ZoneCrimson && !player.ZoneHoly && !player.ZoneDesert && !player.ZoneUndergroundDesert && !player.ZoneGlowshroom && !player.ZoneDungeon && !player.ZoneBeach && !player.ZoneMeteor;
            if (etherealExtorter)
            {
				if (player.ZoneOverworldHeight && ZoneForest)
				{
					knockback *= 1.15f;
				}
            }
        }
        #endregion

        #region Melee Effects
        public override void MeleeEffects(Item item, Rectangle hitbox)
        {
            bool isTrueMelee = item.melee && (item.shoot == 0 || (item.noMelee && item.noUseGraphic && item.useStyle == 5 && !CalamityMod.trueMeleeBoostExceptionList.Contains(item.type) && ItemID.Sets.Yoyo[item.type] != true));
            if (isTrueMelee)
            {
                if (fungalSymbiote && player.whoAmI == Main.myPlayer)
                {
                    if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.1) ||
                        player.itemAnimation == (int)((double)player.itemAnimationMax * 0.3) ||
                        player.itemAnimation == (int)((double)player.itemAnimationMax * 0.5) ||
                        player.itemAnimation == (int)((double)player.itemAnimationMax * 0.7) ||
                        player.itemAnimation == (int)((double)player.itemAnimationMax * 0.9))
                    {
                        float num339 = 0f;
                        float num340 = 0f;
                        float num341 = 0f;
                        float num342 = 0f;
                        if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.9))
                        {
                            num339 = -7f;
                        }
                        if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.7))
                        {
                            num339 = -6f;
                            num340 = 2f;
                        }
                        if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.5))
                        {
                            num339 = -4f;
                            num340 = 4f;
                        }
                        if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.3))
                        {
                            num339 = -2f;
                            num340 = 6f;
                        }
                        if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.1))
                        {
                            num340 = 7f;
                        }
                        if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.7))
                        {
                            num342 = 26f;
                        }
                        if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.3))
                        {
                            num342 -= 4f;
                            num341 -= 20f;
                        }
                        if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.1))
                        {
                            num341 += 6f;
                        }
                        if (player.direction == -1)
                        {
                            if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.9))
                            {
                                num342 -= 8f;
                            }
                            if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.7))
                            {
                                num342 -= 6f;
                            }
                        }
                        num339 *= 1.5f;
                        num340 *= 1.5f;
                        num342 *= (float)player.direction;
                        num341 *= player.gravDir;
                        Projectile.NewProjectile((float)(hitbox.X + hitbox.Width / 2) + num342, (float)(hitbox.Y + hitbox.Height / 2) + num341, (float)player.direction * num340, num339 * player.gravDir, ProjectileID.Mushroom, (int)((float)item.damage * 0.25f * player.meleeDamage), 0f, player.whoAmI, 0f, 0f);
                    }
                }
                if (aWeapon)
                {
                    if (Main.rand.NextBool(3))
                    {
                        int num280 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<BrimstoneFlame>(), player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default, 0.75f);
                    }
                }
                if (aChicken)
                {
                    if (Main.rand.NextBool(3))
                    {
                        int num280 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 244, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default, 0.75f);
                    }
                }
                if (eGauntlet)
                {
                    if (Main.rand.NextBool(3))
                    {
                        int num280 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 66, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.25f);
                        Main.dust[num280].noGravity = true;
                    }
                }
                if (cryogenSoul)
                {
                    if (Main.rand.NextBool(3))
                    {
                        int num280 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 67, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default, 0.75f);
                    }
                }
                if (xerocSet)
                {
                    if (Main.rand.NextBool(3))
                    {
                        int num280 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 58, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default, 1.25f);
                    }
                }
                if (reaverBlast)
                {
                    if (Main.rand.NextBool(3))
                    {
                        int num280 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 74, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default, 0.75f);
                    }
                }
                if (dsSetBonus)
                {
                    if (Main.rand.NextBool(3))
                    {
                        int num280 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 27, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default, 2.5f);
                    }
                }
            }
        }
        #endregion

        #region On Hit NPC
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (omegaBlueChestplate)
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);

			switch (item.type)
			{
				case ItemID.IceSickle:
				case ItemID.Frostbrand:
					target.AddBuff(BuffID.Frostburn, 600);
					break;

				case ItemID.IceBlade:
					target.AddBuff(BuffID.Frostburn, 360);
					break;
			}

			if (eGauntlet)
            {
                target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 120, false);
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 120, false);
                target.AddBuff(ModContent.BuffType<Plague>(), 120, false);
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120, false);
                if (Main.rand.NextBool(5))
                {
                    target.AddBuff(ModContent.BuffType<GlacialState>(), 120, false);
                }
                target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120, false);
                target.AddBuff(BuffID.Poisoned, 120, false);
                target.AddBuff(BuffID.OnFire, 120, false);
                target.AddBuff(BuffID.CursedInferno, 120, false);
                target.AddBuff(BuffID.Frostburn, 120, false);
                target.AddBuff(BuffID.Ichor, 120, false);
                target.AddBuff(BuffID.Venom, 120, false);
            }
            if (aWeapon)
            {
                if (Main.rand.NextBool(4))
                {
                    target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 360, false);
                }
                else if (Main.rand.NextBool(2))
                {
                    target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 240, false);
                }
                else
                {
                    target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 120, false);
                }
            }
            if (abyssalAmulet)
            {
                if (Main.rand.NextBool(4))
                {
                    target.AddBuff(ModContent.BuffType<CrushDepth>(), 360, false);
                }
                else if (Main.rand.NextBool(2))
                {
                    target.AddBuff(ModContent.BuffType<CrushDepth>(), 240, false);
                }
                else
                {
                    target.AddBuff(ModContent.BuffType<CrushDepth>(), 120, false);
                }
            }
            if (dsSetBonus)
            {
                if (Main.rand.NextBool(4))
                {
                    target.AddBuff(ModContent.BuffType<DemonFlames>(), 360, false);
                }
                else if (Main.rand.NextBool(2))
                {
                    target.AddBuff(ModContent.BuffType<DemonFlames>(), 240, false);
                }
                else
                {
                    target.AddBuff(ModContent.BuffType<DemonFlames>(), 120, false);
                }
            }
            if (cryogenSoul || frostFlare)
            {
                if (Main.rand.NextBool(4))
                {
                    target.AddBuff(44, 360, false);
                }
                else if (Main.rand.NextBool(2))
                {
                    target.AddBuff(44, 240, false);
                }
                else
                {
                    target.AddBuff(44, 120, false);
                }
            }
            if (yInsignia)
            {
                if (Main.rand.NextBool(4))
                {
                    target.AddBuff(ModContent.BuffType<HolyFlames>(), 360, false);
                }
                else if (Main.rand.NextBool(2))
                {
                    target.AddBuff(ModContent.BuffType<HolyFlames>(), 240, false);
                }
                else
                {
                    target.AddBuff(ModContent.BuffType<HolyFlames>(), 120, false);
                }
            }
            if (ataxiaFire)
            {
                if (Main.rand.NextBool(4))
                {
                    target.AddBuff(BuffID.OnFire, 720, false);
                }
                else if (Main.rand.NextBool(2))
                {
                    target.AddBuff(BuffID.OnFire, 480, false);
                }
                else
                {
                    target.AddBuff(BuffID.OnFire, 240, false);
                }
            }
            if (alchFlask)
            {
                if (Main.rand.NextBool(4))
                {
                    target.AddBuff(ModContent.BuffType<Plague>(), 360, false);
                }
                else if (Main.rand.NextBool(2))
                {
                    target.AddBuff(ModContent.BuffType<Plague>(), 240, false);
                }
                else
                {
                    target.AddBuff(ModContent.BuffType<Plague>(), 120, false);
                }
            }
            if (armorCrumbling || armorShattering)
            {
                if (item.melee || item.Calamity().rogue)
                {
                    if (Main.rand.NextBool(4))
                    {
                        target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 360, false);
                    }
                    else if (Main.rand.NextBool(2))
                    {
                        target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 240, false);
                    }
                    else
                    {
                        target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 120, false);
                    }
                }
            }
            if (holyWrath)
            {
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 600, false);
            }
        }
        #endregion

        #region On Hit NPC With Proj
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (omegaBlueChestplate && proj.friendly && !target.friendly)
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);

            if (proj.melee && silvaMelee && Main.rand.NextBool(4))
                target.AddBuff(ModContent.BuffType<SilvaStun>(), 20);

			switch (proj.type)
			{
				case ProjectileID.BoneArrow:
					target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 300);
					break;

				case ProjectileID.FrostBlastFriendly:
				case ProjectileID.NorthPoleWeapon:
					target.AddBuff(BuffID.Frostburn, 600);
					break;

				case ProjectileID.FrostBoltStaff:
				case ProjectileID.IceSickle:
				case ProjectileID.FrostBoltSword:
				case ProjectileID.FrostArrow:
				case ProjectileID.NorthPoleSpear:
					target.AddBuff(BuffID.Frostburn, 480);
					break;

				case ProjectileID.IceBoomerang:
				case ProjectileID.IceBolt:
				case ProjectileID.Blizzard:
				case ProjectileID.NorthPoleSnowflake:
					target.AddBuff(BuffID.Frostburn, 240);
					break;

				case ProjectileID.FrostDaggerfish:
					target.AddBuff(BuffID.Frostburn, 180);
					break;

				case ProjectileID.SnowBallFriendly:
					target.AddBuff(BuffID.Frostburn, 120);
					break;
			}

            if (abyssalAmulet)
            {
                if (Main.rand.NextBool(4))
                {
                    target.AddBuff(ModContent.BuffType<CrushDepth>(), 360, false);
                }
                else if (Main.rand.NextBool(2))
                {
                    target.AddBuff(ModContent.BuffType<CrushDepth>(), 240, false);
                }
                else
                {
                    target.AddBuff(ModContent.BuffType<CrushDepth>(), 120, false);
                }
            }
            if (dsSetBonus)
            {
                if (Main.rand.NextBool(4))
                {
                    target.AddBuff(ModContent.BuffType<DemonFlames>(), 360, false);
                }
                else if (Main.rand.NextBool(2))
                {
                    target.AddBuff(ModContent.BuffType<DemonFlames>(), 240, false);
                }
                else
                {
                    target.AddBuff(ModContent.BuffType<DemonFlames>(), 120, false);
                }
            }
            if (uberBees && (proj.type == 566 || proj.type == 181 || proj.type == 189))
            {
                target.AddBuff(ModContent.BuffType<Plague>(), 360);
            }
            else if (alchFlask)
            {
                if (Main.rand.NextBool(4))
                {
                    target.AddBuff(ModContent.BuffType<Plague>(), 360, false);
                }
                else if (Main.rand.NextBool(2))
                {
                    target.AddBuff(ModContent.BuffType<Plague>(), 240, false);
                }
                else
                {
                    target.AddBuff(ModContent.BuffType<Plague>(), 120, false);
                }
            }
            if (proj.melee)
            {
                if (eGauntlet)
                {
                    target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 120, false);
                    target.AddBuff(ModContent.BuffType<HolyFlames>(), 120, false);
                    target.AddBuff(ModContent.BuffType<Plague>(), 120, false);
                    target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120, false);
                    if (Main.rand.NextBool(5))
                    {
                        target.AddBuff(ModContent.BuffType<GlacialState>(), 120, false);
                    }
                    target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120, false);
                    target.AddBuff(BuffID.Poisoned, 120, false);
                    target.AddBuff(BuffID.OnFire, 120, false);
                    target.AddBuff(BuffID.CursedInferno, 120, false);
                    target.AddBuff(BuffID.Frostburn, 120, false);
                    target.AddBuff(BuffID.Ichor, 120, false);
                    target.AddBuff(BuffID.Venom, 120, false);
                }
                if (aWeapon)
                {
                    if (Main.rand.NextBool(4))
                    {
                        target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 360, false);
                    }
                    else if (Main.rand.NextBool(2))
                    {
                        target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 240, false);
                    }
                    else
                    {
                        target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 120, false);
                    }
                }
                if (cryogenSoul || frostFlare)
                {
                    if (Main.rand.NextBool(4))
                    {
                        target.AddBuff(44, 360, false);
                    }
                    else if (Main.rand.NextBool(2))
                    {
                        target.AddBuff(44, 240, false);
                    }
                    else
                    {
                        target.AddBuff(44, 120, false);
                    }
                }
                if (yInsignia)
                {
                    if (Main.rand.NextBool(4))
                    {
                        target.AddBuff(ModContent.BuffType<HolyFlames>(), 360, false);
                    }
                    else if (Main.rand.NextBool(2))
                    {
                        target.AddBuff(ModContent.BuffType<HolyFlames>(), 240, false);
                    }
                    else
                    {
                        target.AddBuff(ModContent.BuffType<HolyFlames>(), 120, false);
                    }
                }
                if (ataxiaFire)
                {
                    if (Main.rand.NextBool(4))
                    {
                        target.AddBuff(BuffID.OnFire, 720, false);
                    }
                    else if (Main.rand.NextBool(2))
                    {
                        target.AddBuff(BuffID.OnFire, 480, false);
                    }
                    else
                    {
                        target.AddBuff(BuffID.OnFire, 240, false);
                    }
                }
            }
            if (armorCrumbling || armorShattering)
            {
                if (proj.melee || proj.Calamity().rogue)
                {
                    if (Main.rand.NextBool(4))
                    {
                        target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 360, false);
                    }
                    else if (Main.rand.NextBool(2))
                    {
                        target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 240, false);
                    }
                    else
                    {
                        target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 120, false);
                    }
                }
            }
            if (perforatorLore)
            {
                target.AddBuff(BuffID.Ichor, 90);
            }
            if (hiveMindLore)
            {
                target.AddBuff(BuffID.CursedInferno, 90);
            }
            if (holyWrath)
            {
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 600, false);
            }
            else if (providenceLore)
            {
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 420, false);
            }
            if (proj.Calamity().rogue)
            {
                if (player.meleeEnchant > 0)
                {
                    if (player.meleeEnchant == 1)
                    {
                        target.AddBuff(BuffID.Venom, 60 * Main.rand.Next(5, 10), false);
                    }
                    if (player.meleeEnchant == 2)
                    {
                        target.AddBuff(BuffID.CursedInferno, 60 * Main.rand.Next(3, 7), false);
                    }
                    if (player.meleeEnchant == 3)
                    {
                        target.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(3, 7), false);
                    }
                    if (player.meleeEnchant == 5)
                    {
                        target.AddBuff(BuffID.Ichor, 60 * Main.rand.Next(10, 20), false);
                    }
                    if (player.meleeEnchant == 6)
                    {
                        target.AddBuff(BuffID.Confused, 60 * Main.rand.Next(1, 4), false);
                    }
                    if (player.meleeEnchant == 8)
                    {
                        target.AddBuff(BuffID.Poisoned, 60 * Main.rand.Next(5, 10), false);
                    }
                    if (player.meleeEnchant == 4)
                    {
                        target.AddBuff(BuffID.Midas, 120, false);
                    }
                }
				if (etherealExtorter)
				{
					if (ZoneSunkenSea)
					{
						target.AddBuff(ModContent.BuffType<TemporalSadness>(), 60, false);
					}
					if (ZoneSulphur)
					{
						target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 120, false);
						target.AddBuff(ModContent.BuffType<Irradiated>(), 300, false);
					}
					if (Main.moonPhase == 6) //first quarter
					{
						target.AddBuff(BuffID.Midas, 120, false);
					}
					if (ZoneCalamity && CalamityMod.fireWeaponList.Contains(player.inventory[player.selectedItem].type))
					{
						target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 240, false);
					}
				}					
            }
        }
        #endregion

        #region PvP
        public override void OnHitPvp(Item item, Player target, int damage, bool crit)
        {
            if (omegaBlueChestplate)
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);

            if (eGauntlet)
            {
                target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 120, false);
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 120, false);
                target.AddBuff(ModContent.BuffType<Plague>(), 120, false);
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120, false);
                if (Main.rand.NextBool(5))
                {
                    target.AddBuff(ModContent.BuffType<GlacialState>(), 120, false);
                }
                target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120, false);
                target.AddBuff(BuffID.Poisoned, 120, false);
                target.AddBuff(BuffID.OnFire, 120, false);
                target.AddBuff(BuffID.CursedInferno, 120, false);
                target.AddBuff(BuffID.Frostburn, 120, false);
                target.AddBuff(BuffID.Ichor, 120, false);
                target.AddBuff(BuffID.Venom, 120, false);
            }
            if (aWeapon)
            {
                if (Main.rand.NextBool(4))
                {
                    target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 360, false);
                }
                else if (Main.rand.NextBool(2))
                {
                    target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 240, false);
                }
                else
                {
                    target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 120, false);
                }
            }
            if (abyssalAmulet)
            {
                if (Main.rand.NextBool(4))
                {
                    target.AddBuff(ModContent.BuffType<CrushDepth>(), 360, false);
                }
                else if (Main.rand.NextBool(2))
                {
                    target.AddBuff(ModContent.BuffType<CrushDepth>(), 240, false);
                }
                else
                {
                    target.AddBuff(ModContent.BuffType<CrushDepth>(), 120, false);
                }
            }
            if (cryogenSoul || frostFlare)
            {
                if (Main.rand.NextBool(4))
                {
                    target.AddBuff(44, 360, false);
                }
                else if (Main.rand.NextBool(2))
                {
                    target.AddBuff(44, 240, false);
                }
                else
                {
                    target.AddBuff(44, 120, false);
                }
            }
            if (yInsignia)
            {
                if (Main.rand.NextBool(4))
                {
                    target.AddBuff(ModContent.BuffType<HolyFlames>(), 360, false);
                }
                else if (Main.rand.NextBool(2))
                {
                    target.AddBuff(ModContent.BuffType<HolyFlames>(), 240, false);
                }
                else
                {
                    target.AddBuff(ModContent.BuffType<HolyFlames>(), 120, false);
                }
            }
            if (ataxiaFire)
            {
                if (Main.rand.NextBool(4))
                {
                    target.AddBuff(BuffID.OnFire, 720, false);
                }
                else if (Main.rand.NextBool(2))
                {
                    target.AddBuff(BuffID.OnFire, 480, false);
                }
                else
                {
                    target.AddBuff(BuffID.OnFire, 240, false);
                }
            }
            if (alchFlask)
            {
                if (Main.rand.NextBool(4))
                {
                    target.AddBuff(ModContent.BuffType<Plague>(), 360, false);
                }
                else if (Main.rand.NextBool(2))
                {
                    target.AddBuff(ModContent.BuffType<Plague>(), 240, false);
                }
                else
                {
                    target.AddBuff(ModContent.BuffType<Plague>(), 120, false);
                }
            }
            if (holyWrath)
            {
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 600, false);
            }
        }

        public override void OnHitPvpWithProj(Projectile proj, Player target, int damage, bool crit)
        {
            if (omegaBlueChestplate)
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);

            if (proj.melee && silvaMelee && Main.rand.NextBool(4))
                target.AddBuff(ModContent.BuffType<SilvaStun>(), 20);

            if (abyssalAmulet)
            {
                if (Main.rand.NextBool(4))
                {
                    target.AddBuff(ModContent.BuffType<CrushDepth>(), 360, false);
                }
                else if (Main.rand.NextBool(2))
                {
                    target.AddBuff(ModContent.BuffType<CrushDepth>(), 240, false);
                }
                else
                {
                    target.AddBuff(ModContent.BuffType<CrushDepth>(), 120, false);
                }
            }
            if (uberBees && (proj.type == 566 || proj.type == 181 || proj.type == 189))
            {
                target.AddBuff(ModContent.BuffType<Plague>(), 360);
            }
            else if (alchFlask)
            {
                if (Main.rand.NextBool(4))
                {
                    target.AddBuff(ModContent.BuffType<Plague>(), 360, false);
                }
                else if (Main.rand.NextBool(2))
                {
                    target.AddBuff(ModContent.BuffType<Plague>(), 240, false);
                }
                else
                {
                    target.AddBuff(ModContent.BuffType<Plague>(), 120, false);
                }
            }
            if (proj.melee)
            {
                if (eGauntlet)
                {
                    target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 120, false);
                    target.AddBuff(ModContent.BuffType<HolyFlames>(), 120, false);
                    target.AddBuff(ModContent.BuffType<Plague>(), 120, false);
                    target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120, false);
                    if (Main.rand.NextBool(5))
                    {
                        target.AddBuff(ModContent.BuffType<GlacialState>(), 120, false);
                    }
                    target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120, false);
                    target.AddBuff(BuffID.Poisoned, 120, false);
                    target.AddBuff(BuffID.OnFire, 120, false);
                    target.AddBuff(BuffID.CursedInferno, 120, false);
                    target.AddBuff(BuffID.Frostburn, 120, false);
                    target.AddBuff(BuffID.Ichor, 120, false);
                    target.AddBuff(BuffID.Venom, 120, false);
                }
                if (aWeapon)
                {
                    if (Main.rand.NextBool(4))
                    {
                        target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 360, false);
                    }
                    else if (Main.rand.NextBool(2))
                    {
                        target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 240, false);
                    }
                    else
                    {
                        target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 120, false);
                    }
                }
                if (cryogenSoul || frostFlare)
                {
                    if (Main.rand.NextBool(4))
                    {
                        target.AddBuff(44, 360, false);
                    }
                    else if (Main.rand.NextBool(2))
                    {
                        target.AddBuff(44, 240, false);
                    }
                    else
                    {
                        target.AddBuff(44, 120, false);
                    }
                }
                if (yInsignia)
                {
                    if (Main.rand.NextBool(4))
                    {
                        target.AddBuff(ModContent.BuffType<HolyFlames>(), 360, false);
                    }
                    else if (Main.rand.NextBool(2))
                    {
                        target.AddBuff(ModContent.BuffType<HolyFlames>(), 240, false);
                    }
                    else
                    {
                        target.AddBuff(ModContent.BuffType<HolyFlames>(), 120, false);
                    }
                }
                if (ataxiaFire)
                {
                    if (Main.rand.NextBool(4))
                    {
                        target.AddBuff(BuffID.OnFire, 720, false);
                    }
                    else if (Main.rand.NextBool(2))
                    {
                        target.AddBuff(BuffID.OnFire, 480, false);
                    }
                    else
                    {
                        target.AddBuff(BuffID.OnFire, 240, false);
                    }
                }
            }
            if (armorCrumbling || armorShattering)
            {
                if (proj.melee || proj.Calamity().rogue)
                {
                    if (Main.rand.NextBool(4))
                    {
                        target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 360, false);
                    }
                    else if (Main.rand.NextBool(2))
                    {
                        target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 240, false);
                    }
                    else
                    {
                        target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 120, false);
                    }
                }
            }
            if (perforatorLore)
            {
                target.AddBuff(BuffID.Ichor, 90);
            }
            if (hiveMindLore)
            {
                target.AddBuff(BuffID.CursedInferno, 90);
            }
            if (holyWrath)
            {
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 600, false);
            }
            else if (providenceLore)
            {
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 420, false);
            }
            if (proj.Calamity().rogue)
            {
                if (player.meleeEnchant > 0)
                {
                    if (player.meleeEnchant == 1)
                    {
                        target.AddBuff(BuffID.Venom, 60 * Main.rand.Next(5, 10), false);
                    }
                    if (player.meleeEnchant == 2)
                    {
                        target.AddBuff(BuffID.CursedInferno, 60 * Main.rand.Next(3, 7), false);
                    }
                    if (player.meleeEnchant == 3)
                    {
                        target.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(3, 7), false);
                    }
                    if (player.meleeEnchant == 5)
                    {
                        target.AddBuff(BuffID.Ichor, 60 * Main.rand.Next(10, 20), false);
                    }
                    if (player.meleeEnchant == 6)
                    {
                        target.AddBuff(BuffID.Confused, 60 * Main.rand.Next(1, 4), false);
                    }
                    if (player.meleeEnchant == 8)
                    {
                        target.AddBuff(BuffID.Poisoned, 60 * Main.rand.Next(5, 10), false);
                    }
                    if (player.meleeEnchant == 4)
                    {
                        target.AddBuff(BuffID.Midas, 120, false);
                    }
                }
            }
        }
        #endregion

        #region Modify Hit NPC
        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
			bool isTrueMelee = item.melee && item.shoot == 0;

			#region MultiplierBoosts
			double damageMult = 1.0;
            if (silvaMelee && Main.rand.NextBool(4) && item.melee)
            {
                damageMult += 4.0;
            }
            if (enraged && !Config.BossRushXerocCurse)
            {
                damageMult += 1.25;
            }
            if (CalamityWorld.revenge && Config.AdrenalineAndRage)
            {
                bool DHorHoD = draedonsHeart || heartOfDarkness;
                if (rageMode && adrenalineMode)
                {
                    if (item.melee)
                    {
                        damageMult += CalamityWorld.death ? (DHorHoD ? 7.6 : 6.8) : (DHorHoD ? 2.3 : 2.0); // Death Mode values: 8.9 and 8.0, rev: 2.3 and 2.0
                    }
                }
                else if (rageMode)
                {
                    if (item.melee)
                    {
                        double rageDamageBoost = 0.0 +
                            (rageBoostOne ? (CalamityWorld.death ? 0.5 : 0.15) : 0.0) + // Death Mode values: 0.6, rev: 0.15
                            (rageBoostTwo ? (CalamityWorld.death ? 0.5 : 0.15) : 0.0) + // Death Mode values: 0.6, rev: 0.15
                            (rageBoostThree ? (CalamityWorld.death ? 0.5 : 0.15) : 0.0); // Death Mode values: 0.6, rev: 0.15
                        double rageDamage = (CalamityWorld.death ? (DHorHoD ? 2.0 : 1.7) : (DHorHoD ? 0.65 : 0.5)) + rageDamageBoost; // Death Mode values: 2.3 and 2.0, rev: 0.65 and 0.5
                        damageMult += rageDamage;
                    }
                }
                else if (adrenalineMode)
                {
                    if (item.melee)
                    {
                        double damageMultAdr = (CalamityWorld.death ? 5.0 : 1.5) * (double)adrenalineDmgMult; // Death Mode values: 6, rev: 1.5
                        damageMult += damageMultAdr;
                    }
                }
            }
            damage = (int)((double)damage * damageMult);

            if (oldDie)
            {
                float diceMult = Main.rand.NextFloat(0.8824f, 1.1565f);
                if (item.Calamity().rogue || wearingRogueArmor)
                {
                    float roll2 = Main.rand.NextFloat(0.8824f, 1.1565f);
                    diceMult = roll2 > diceMult ? roll2 : diceMult;
                }
                damage = (int)(damage * diceMult);
            }
            #endregion

            if (yharonLore)
                damage = (int)((double)damage * 0.75);

            if ((target.damage > 5 || target.boss) && player.whoAmI == Main.myPlayer && !target.SpawnedFromStatue)
            {
				if (isTrueMelee && soaring)
				{
					double useTimeMultiplier = 0.85 + ((double)(item.useTime * item.useAnimation) / 3600D); //28 * 28 = 784 is average so that equals 784 / 3600 = 0.217777 + 1 = 21.7% boost
					double wingTimeFraction = (double)player.wingTimeMax / 20D;
					double meleeStatMultiplier = (double)(player.meleeDamage * (float)((double)player.meleeCrit / 10D));

					if (player.wingTime < player.wingTimeMax)
						player.wingTime += (int)(useTimeMultiplier * (wingTimeFraction + meleeStatMultiplier));

					if (player.wingTime > player.wingTimeMax)
						player.wingTime = player.wingTimeMax;
				}
				if (item.melee && !item.noMelee && !item.noUseGraphic)
                {
					if (ataxiaGeyser)
                    {
                        if (player.ownedProjectileCounts[ModContent.ProjectileType<ChaosGeyser>()] < 3)
                        {
                            Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<ChaosGeyser>(), (int)((double)damage * 0.15), 2f, player.whoAmI, 0f, 0f);
                        }
                    }
                    if (unstablePrism && crit)
                    {
                        for (int num252 = 0; num252 < 3; num252++)
                        {
                            Vector2 value15 = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
                            while (value15.X == 0f && value15.Y == 0f)
                            {
                                value15 = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
                            }
                            value15.Normalize();
                            value15 *= (float)Main.rand.Next(30, 61) * 0.1f;
                            Projectile.NewProjectile(target.Center.X, target.Center.Y, value15.X, value15.Y, ModContent.ProjectileType<UnstableSpark>(), (int)((double)damage * 0.15), 0f, player.whoAmI, 0f, 0f);
                        }
                    }
                }
                if (astralStarRain && crit && astralStarRainCooldown <= 0)
                {
                    astralStarRainCooldown = 60;
                    for (int n = 0; n < 3; n++)
                    {
                        float x = target.position.X + (float)Main.rand.Next(-400, 400);
                        float y = target.position.Y - (float)Main.rand.Next(500, 800);
                        Vector2 vector = new Vector2(x, y);
                        float num13 = target.position.X + (float)(target.width / 2) - vector.X;
                        float num14 = target.position.Y + (float)(target.height / 2) - vector.Y;
                        num13 += (float)Main.rand.Next(-100, 101);
                        float num15 = 25f;
                        int projectileType = Main.rand.Next(3);
                        if (projectileType == 0)
                        {
                            projectileType = ModContent.ProjectileType<AstralStar>();
                        }
                        else if (projectileType == 1)
                        {
                            projectileType = 92;
                        }
                        else
                        {
                            projectileType = 12;
                        }
                        float num16 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
                        num16 = num15 / num16;
                        num13 *= num16;
                        num14 *= num16;
                        int num17 = Projectile.NewProjectile(x, y, num13, num14, projectileType, 120, 5f, player.whoAmI, 0f, 0f);
                        Main.projectile[num17].ranged = false;
                    }
                }
                if (bloodflareMelee && item.melee)
                {
                    if (bloodflareMeleeHits < 15 && !bloodflareFrenzy && !bloodFrenzyCooldown)
                    {
                        bloodflareMeleeHits++;
                    }
                    if (player.whoAmI == Main.myPlayer && target.canGhostHeal)
                    {
                        int healAmount = Main.rand.Next(3) + 1;
                        player.statLife += healAmount;
                        player.HealEffect(healAmount);
                    }
                }
                if (Config.ProficiencyEnabled)
                {
                    if (gainLevelCooldown <= 0)
                    {
                        gainLevelCooldown = 120;
                        if (item.melee && meleeLevel <= 12500)
                        {
                            if (!Main.hardMode && meleeLevel >= 1500)
                            {
                                gainLevelCooldown = 1200; //20 seconds
                            }
                            if (!NPC.downedMoonlord && meleeLevel >= 5500)
                            {
                                gainLevelCooldown = 2400; //40 seconds
                            }
                            meleeLevel++;
                            if (fasterMeleeLevel && meleeLevel % 100 != 0 && Main.rand.NextBool(10)) //add only to non-multiples of 100
                                meleeLevel++;
                            shootFireworksLevelUpMelee = true;
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                            {
                                LevelPacket(false, 0);
                            }
                        }
                    }
                }
                if (CalamityWorld.revenge && Config.AdrenalineAndRage)
                {
                    if (item.melee)
                    {
                        int stressGain = (int)((double)damage * 0.1);
                        int stressMaxGain = 10;
                        if (stressGain < 1)
                        {
                            stressGain = 1;
                        }
                        if (stressGain > stressMaxGain)
                        {
                            stressGain = stressMaxGain;
                        }
                        stress += stressGain;
                        if (stress >= stressMax)
                        {
                            stress = stressMax;
                        }
                    }
                }
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            bool isTrueMelee = proj.Calamity().trueMelee;
            bool isSummon = proj.minion || proj.sentry || CalamityMod.projectileMinionList.Contains(proj.type);
            bool hasClassType = proj.melee || proj.ranged || proj.magic || isSummon || proj.Calamity().rogue;

			Item heldItem = player.inventory[player.selectedItem];

			if (isTrueMelee && soaring)
			{
				double useTimeMultiplier = 0.85 + ((double)(heldItem.useTime * heldItem.useAnimation) / 3600D); //28 * 28 = 784 is average so that equals 784 / 3600 = 0.217777 + 1 = 21.7% boost
				double wingTimeFraction = (double)player.wingTimeMax / 20D;
				double meleeStatMultiplier = (double)(player.meleeDamage * (float)((double)player.meleeCrit / 10D));

				if (player.wingTime < player.wingTimeMax)
					player.wingTime += (int)(useTimeMultiplier * (wingTimeFraction + meleeStatMultiplier));

				if (player.wingTime > player.wingTimeMax)
					player.wingTime = player.wingTimeMax;
			}

            if (proj.Calamity().rogue)
            {
                crit = Main.rand.Next(1, 101) < player.Calamity().throwingCrit;
            }

            #region MultiplierBoosts
            double damageMult = 1.0;
            if (isSummon)
            {
                if (heldItem.type > 0)
                {
                    if ((heldItem.summon && !heldItem.melee && !heldItem.ranged && !heldItem.magic && !heldItem.Calamity().rogue) ||
						heldItem.hammer > 0 || heldItem.pick > 0 || heldItem.axe > 0)
                    {
                        damageMult += 0.1;
                    }
                }
            }
            if (screwdriver)
            {
                if (proj.penetrate > 1 || proj.penetrate == -1)
                    damageMult += 0.1;
            }
            if (sPower)
            {
                if (isSummon)
                    damageMult += 0.1;
            }
            if (providenceLore && hasClassType)
            {
                damageMult += 0.05;
            }
            if (silvaMelee && Main.rand.NextBool(4) && isTrueMelee)
            {
                damageMult += 4.0;
            }
            if (enraged && !Config.BossRushXerocCurse)
            {
                damageMult += 1.25;
            }
            if (auricSet)
            {
                if (silvaThrowing && proj.Calamity().rogue &&
                    crit && player.statLife > (int)((double)player.statLifeMax2 * 0.5))
                {
                    damageMult += 0.25;
                }
                if (silvaMelee && proj.melee)
                {
                    double multiplier = (double)player.statLife / (double)player.statLifeMax2;
                    damageMult += multiplier * 0.2;
                }
            }
            if (godSlayerRanged && crit && proj.ranged)
            {
                int randomChance = 100 - player.rangedCrit; //100 min to 15 max with cap

                if (randomChance < 15)
                    randomChance = 15;
                if (Main.rand.Next(randomChance) == 0)
                    damageMult += 1.0;
            }
            if (silvaCountdown > 0 && hasSilvaEffect && silvaRanged && proj.ranged)
            {
                damageMult += 0.4;
            }
            if (silvaCountdown <= 0 && hasSilvaEffect && silvaThrowing && proj.Calamity().rogue)
            {
                damageMult += 0.1;
            }
            if (silvaCountdown <= 0 && hasSilvaEffect && silvaMage && proj.magic)
            {
                damageMult += 0.1;
            }
            if (silvaCountdown <= 0 && hasSilvaEffect && silvaSummon && isSummon)
            {
                damageMult += 0.1;
            }
            if (proj.type == ModContent.ProjectileType<FrostsparkBulletProj>())
            {
                if (target.buffImmune[ModContent.BuffType<GlacialState>()])
                    damageMult += 0.2;
            }
            else if (proj.type == ProjectileID.InfernoFriendlyBlast)
            {
                damageMult += 0.33;
            }
            if (CalamityWorld.revenge)
            {
                bool DHorHoD = draedonsHeart || heartOfDarkness;
                if (rageMode && adrenalineMode)
                {
                    if (hasClassType)
                    {
                        damageMult += CalamityWorld.death ? (DHorHoD ? 7.6 : 6.8) : (DHorHoD ? 2.3 : 2.0); // Death Mode values: 8.9 and 8.0, rev: 2.3 and 2.0
                    }
                }
                else if (rageMode)
                {
                    if (hasClassType)
                    {
                        double rageDamageBoost = 0.0 +
                            (rageBoostOne ? (CalamityWorld.death ? 0.5 : 0.15) : 0.0) + // Death Mode values: 0.6, rev: 0.15
                            (rageBoostTwo ? (CalamityWorld.death ? 0.5 : 0.15) : 0.0) + // Death Mode values: 0.6, rev: 0.15
                            (rageBoostThree ? (CalamityWorld.death ? 0.5 : 0.15) : 0.0); // Death Mode values: 0.6, rev: 0.15
                        double rageDamage = (CalamityWorld.death ? (DHorHoD ? 2.0 : 1.7) : (DHorHoD ? 0.65 : 0.5)) + rageDamageBoost; // Death Mode values: 2.3 and 2.0, rev: 0.65 and 0.5
                        damageMult += rageDamage;
                    }
                }
                else if (adrenalineMode)
                {
                    if (hasClassType)
                    {
                        double damageMultAdr = (CalamityWorld.death ? 5.0 : 1.5) * (double)adrenalineDmgMult; // Death Mode values: 6, rev: 1.5
                        damageMult += damageMultAdr;
                    }
                }
            }
            if ((filthyGlove || electricianGlove) && proj.Calamity().stealthStrike && proj.Calamity().rogue)
            {
                damageMult += 0.1;
            }
            if (etherealExtorter && proj.Calamity().rogue)
            {
				bool ZoneForest = !ZoneAbyss && !ZoneSulphur && !ZoneAstral && !ZoneCalamity && !ZoneSunkenSea && !player.ZoneSnow && !player.ZoneCorrupt && !player.ZoneCrimson && !player.ZoneHoly && !player.ZoneDesert && !player.ZoneUndergroundDesert && !player.ZoneGlowshroom && !player.ZoneDungeon && !player.ZoneBeach && !player.ZoneMeteor;
                if (Main.moonPhase == 7 && crit) //Waxing Gibbous
				{
					damageMult += 0.05;
				}
				if (Main.moonPhase == 5) //Waxing Cresent
				{
					if (proj.penetrate == -1)
						damageMult += 0.1;
					else if (proj.penetrate >= 5)
						damageMult += 0.08;
					else if (proj.penetrate == 4)
						damageMult += 0.06;
					else if (proj.penetrate == 3)
						damageMult += 0.03;
					else if (proj.penetrate == 2)
						damageMult += 0.02;
				}
				if (player.ZoneDirtLayerHeight && ZoneForest)
				{
					if (Main.rand.NextBool(20) && !crit) //5% chance to minicrit
						damageMult += 0.5;
				}
            }
            damage = (int)((double)damage * damageMult);

            if (oldDie)
            {
                float diceMult = Main.rand.NextFloat(0.8824f, 1.1565f);
                if (proj.Calamity().rogue || wearingRogueArmor)
                {
                    float roll2 = Main.rand.NextFloat(0.8824f, 1.1565f);
                    diceMult = roll2 > diceMult ? roll2 : diceMult;
                }
                damage = (int)(damage * diceMult);
            }
            #endregion

            #region AdditiveBoosts
            if (theBee && !isSummon)
            {
                if (hasClassType)
                    damage += theBeeDamage;
            }
            if (proj.type == ModContent.ProjectileType<AcidBulletProj>())
            {
                int defenseAdd = (int)((double)target.defense * 0.1 * ((double)damage / 50.0)); //100 defense * 0.1 = 10
                damage += defenseAdd;
            }
            if (uberBees && (proj.type == ProjectileID.GiantBee || proj.type == ProjectileID.Bee || proj.type == ProjectileID.Wasp || proj.type == ModContent.ProjectileType<PlaguenadeBee>()))
            {
                damage += Main.rand.Next(20, 31);
            }
            if (proj.Calamity().stealthStrike && proj.Calamity().rogue && electricianGlove)
            {
				//Ozzatron insists on counting for edge-cases
				int penetratableDefense = Math.Max(target.defense - player.armorPenetration, 0);
				int penetratedDefense = Math.Min(penetratableDefense, 30);
				damage += (int)(0.5f * penetratedDefense);
            }
            else if (proj.Calamity().stealthStrike && proj.Calamity().rogue && (filthyGlove || bloodyGlove))
            {
				int penetratableDefense = Math.Max(target.defense - player.armorPenetration, 0);
				int penetratedDefense = Math.Min(penetratableDefense, 10);
				damage += (int)(0.5f * penetratedDefense);
            }
            if (proj.Calamity().rogue && etherealExtorter)
            {
                if (CalamityMod.boomerangProjList.Contains(proj.type) && player.ZoneCorrupt)
				{
					int penetratableDefense = Math.Max(target.defense - player.armorPenetration, 0);
					int penetratedDefense = Math.Min(penetratableDefense, 6);
					damage += (int)(0.5f * penetratedDefense);
				}
            }
            #endregion

            #region MultiplicativeReductions
            if (isSummon)
            {
                if (heldItem.type > 0)
                {
                    if (!heldItem.summon &&
						(heldItem.melee || heldItem.ranged || heldItem.magic || heldItem.Calamity().rogue) &&
						heldItem.hammer == 0 && heldItem.pick == 0 && heldItem.axe == 0 && heldItem.useStyle != 0)
                    {
                        damage = (int)((double)damage * 0.75);
                    }
                }
            }
            if (proj.ranged)
            {
                switch (proj.type)
                {
                    case ProjectileID.CrystalShard:
                        damage = (int)((double)damage * 0.6);
                        break;
                    case ProjectileID.ChlorophyteBullet:
                        damage = (int)((double)damage * 0.8);
                        break;
                    case ProjectileID.HallowStar:
                        damage = (int)((double)damage * 0.7);
                        break;
                }
                if (proj.type == ModContent.ProjectileType<VeriumBulletProj>())
                    damage = (int)((double)damage * 0.8);
            }
            if (proj.type == ProjectileID.SpectreWrath && player.ghostHurt)
                damage = (int)((double)damage * 0.7);
            if (yharonLore)
                damage = (int)((double)damage * 0.75);
            #endregion

            if (tarraMage && crit && proj.magic)
            {
                tarraCrits++;
            }
            if (tarraThrowing && !tarragonImmunity && !tarragonImmunityCooldown && tarraThrowingCrits < 25 && crit && proj.Calamity().rogue)
            {
                tarraThrowingCrits++;
            }

            if ((target.damage > 5 || target.boss) && player.whoAmI == Main.myPlayer && !target.SpawnedFromStatue)
            {
                if (theBeeDamage > 0 && (proj.melee || proj.ranged || proj.magic || proj.Calamity().rogue))
                {
                    Main.PlaySound(2, (int)proj.position.X, (int)proj.position.Y, 110);
                }
                if (unstablePrism && crit)
                {
                    for (int num252 = 0; num252 < 3; num252++)
                    {
                        Vector2 value15 = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
                        while (value15.X == 0f && value15.Y == 0f)
                        {
                            value15 = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
                        }
                        value15.Normalize();
                        value15 *= (float)Main.rand.Next(30, 61) * 0.1f;
                        Projectile.NewProjectile(proj.oldPosition.X + (float)(proj.width / 2), proj.oldPosition.Y + (float)(proj.height / 2), value15.X, value15.Y, ModContent.ProjectileType<UnstableSpark>(), (int)((double)damage * 0.15), 0f, player.whoAmI, 0f, 0f);
                    }
                }
                if (electricianGlove && proj.Calamity().stealthStrike && proj.Calamity().rogue)
                {
                    for (int num252 = 0; num252 < 3; num252++)
                    {
                        Vector2 value15 = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
                        while (value15.X == 0f && value15.Y == 0f)
                        {
                            value15 = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
                        }
                        value15.Normalize();
                        value15 *= (float)Main.rand.Next(30, 61) * 0.1f;
                        int num17 = Projectile.NewProjectile(proj.oldPosition.X + (float)(proj.width / 2), proj.oldPosition.Y + (float)(proj.height / 2), value15.X, value15.Y, ModContent.ProjectileType<Spark>(), (int)((double)damage * 0.1), 0f, player.whoAmI, 0f, 0f);
						Main.projectile[num17].Calamity().forceRogue = true;
						Main.projectile[num17].localNPCHitCooldown = -1;
                    }
                }
                if (astralStarRain && crit && astralStarRainCooldown <= 0)
                {
                    astralStarRainCooldown = 60;
                    for (int n = 0; n < 3; n++)
                    {
                        float x = target.position.X + (float)Main.rand.Next(-400, 400);
                        float y = target.position.Y - (float)Main.rand.Next(500, 800);
                        Vector2 vector = new Vector2(x, y);
                        float num13 = target.position.X + (float)(target.width / 2) - vector.X;
                        float num14 = target.position.Y + (float)(target.height / 2) - vector.Y;
                        num13 += (float)Main.rand.Next(-100, 101);
                        float num15 = 25f;
                        int projectileType = Main.rand.Next(3);
                        if (projectileType == 0)
                        {
                            projectileType = ModContent.ProjectileType<AstralStar>();
                        }
                        else if (projectileType == 1)
                        {
                            projectileType = 92;
                        }
                        else
                        {
                            projectileType = 12;
                        }
                        float num16 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
                        num16 = num15 / num16;
                        num13 *= num16;
                        num14 *= num16;
                        int num17 = Projectile.NewProjectile(x, y, num13, num14, projectileType, 120, 5f, player.whoAmI, 0f, 0f);
                        Main.projectile[num17].ranged = false;
                    }
                }
                if (tarraRanged && crit && proj.ranged)
                {
                    int num251 = Main.rand.Next(2, 4);
                    for (int num252 = 0; num252 < num251; num252++)
                    {
                        Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                        while (value15.X == 0f && value15.Y == 0f)
                        {
                            value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                        }
                        value15.Normalize();
                        value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
                        int FUCKYOU = Projectile.NewProjectile(target.position.X + (float)(target.width / 2), target.position.Y + (float)(target.height / 2),
                            value15.X, value15.Y, 206, (int)((double)damage * 0.25), 0f, player.whoAmI, 0f, 0f);
                        Main.projectile[FUCKYOU].magic = false;
                        Main.projectile[FUCKYOU].netUpdate = true;
                    }
                }
                if (bloodflareThrowing && proj.Calamity().rogue && crit && Main.rand.NextBool(2))
                {
                    if (target.canGhostHeal)
                    {
                        float num11 = 0.03f;
                        num11 -= (float)proj.numHits * 0.015f;
                        if (num11 < 0f)
                        {
                            num11 = 0f;
                        }
                        float num12 = (float)proj.damage * num11;
                        if (num12 < 0f)
                        {
                            num12 = 0f;
                        }
                        if (player.lifeSteal > 0f)
                        {
                            player.statLife += 1;
                            player.HealEffect(1);
                            player.lifeSteal -= num12 * 2f;
                        }
                    }
                }
                if (bloodflareMage && bloodflareMageCooldown <= 0 && crit && proj.magic)
                {
                    bloodflareMageCooldown = 120;
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                        while (value15.X == 0f && value15.Y == 0f)
                        {
                            value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                        }
                        value15.Normalize();
                        value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
                        int fire = Projectile.NewProjectile(target.position.X + (float)(target.width / 2), target.position.Y + (float)(target.height / 2),
                            value15.X, value15.Y, 15, (int)((double)damage * 0.5), 0f, player.whoAmI, 0f, 0f);
                        Main.projectile[fire].magic = false;
                        Main.projectile[fire].netUpdate = true;
                    }
                }
                if (umbraphileSet && proj.Calamity().rogue && (Main.rand.NextBool(4) || proj.Calamity().stealthStrike))
                {
                    int newDamage = (int)((double)proj.damage * 0.25);
                    if (newDamage > 50)
                    {
                        newDamage = 50;
                    }
                    Projectile.NewProjectile(proj.Center.X, proj.Center.Y, 0f, 0f, ModContent.ProjectileType<UmbraphileBoom>(), newDamage, 0f, player.whoAmI, 0f, 0f);
                }
                if (bloodflareMelee && isTrueMelee)
                {
                    if (bloodflareMeleeHits < 15 && !bloodflareFrenzy && !bloodFrenzyCooldown)
                    {
                        bloodflareMeleeHits++;
                    }
                    if (player.whoAmI == Main.myPlayer && target.canGhostHeal)
                    {
                        int healAmount = Main.rand.Next(3) + 1;
                        player.statLife += healAmount;
                        player.HealEffect(healAmount);
                    }
                }
                if (proj.type == ModContent.ProjectileType<PolarStar>())
                {
                    polarisBoostCounter += 1;
                }
                if (Config.ProficiencyEnabled)
                {
                    if (gainLevelCooldown <= 0) //max is 12501 to avoid setting off fireworks forever
                    {
                        gainLevelCooldown = 120; //2 seconds
                        if (proj.melee && meleeLevel <= 12500)
                        {
                            if (!Main.hardMode && meleeLevel >= 1500)
                            {
                                gainLevelCooldown = 1200; //20 seconds
                            }
                            if (!NPC.downedMoonlord && meleeLevel >= 5500)
                            {
                                gainLevelCooldown = 2400; //40 seconds
                            }
                            meleeLevel++;
                            if (fasterMeleeLevel && meleeLevel % 100 != 0 && Main.rand.NextBool(10)) //add only to non-multiples of 100
                                meleeLevel++;
                            shootFireworksLevelUpMelee = true;
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                            {
                                LevelPacket(false, 0);
                            }
                        }
                        else if (proj.ranged && rangedLevel <= 12500)
                        {
                            if (!Main.hardMode && rangedLevel >= 1500)
                            {
                                gainLevelCooldown = 1200; //20 seconds
                            }
                            if (!NPC.downedMoonlord && rangedLevel >= 5500)
                            {
                                gainLevelCooldown = 2400; //40 seconds
                            }
                            rangedLevel++;
                            if (fasterRangedLevel && rangedLevel % 100 != 0 && Main.rand.NextBool(10)) //add only to non-multiples of 100
                                rangedLevel++;
                            shootFireworksLevelUpRanged = true;
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                            {
                                LevelPacket(false, 1);
                            }
                        }
                        else if (proj.magic && magicLevel <= 12500)
                        {
                            if (!Main.hardMode && magicLevel >= 1500)
                            {
                                gainLevelCooldown = 1200; //20 seconds
                            }
                            if (!NPC.downedMoonlord && magicLevel >= 5500)
                            {
                                gainLevelCooldown = 2400; //40 seconds
                            }
                            magicLevel++;
                            if (fasterMagicLevel && magicLevel % 100 != 0 && Main.rand.NextBool(10)) //add only to non-multiples of 100
                                magicLevel++;
                            shootFireworksLevelUpMagic = true;
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                            {
                                LevelPacket(false, 2);
                            }
                        }
                        else if (isSummon && summonLevel <= 12500)
                        {
                            if (!Main.hardMode && summonLevel >= 1500)
                            {
                                gainLevelCooldown = 1200; //20 seconds
                            }
                            if (!NPC.downedMoonlord && summonLevel >= 5500)
                            {
                                gainLevelCooldown = 2400; //40 seconds
                            }
                            summonLevel++;
                            if (fasterSummonLevel && summonLevel % 100 != 0 && Main.rand.NextBool(10)) //add only to non-multiples of 100
                                summonLevel++;
                            shootFireworksLevelUpSummon = true;
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                            {
                                LevelPacket(false, 3);
                            }
                        }
                        else if (proj.Calamity().rogue && rogueLevel <= 12500)
                        {
                            if (!Main.hardMode && rogueLevel >= 1500)
                            {
                                gainLevelCooldown = 1200; //20 seconds
                            }
                            if (!NPC.downedMoonlord && rogueLevel >= 5500)
                            {
                                gainLevelCooldown = 2400; //40 seconds
                            }
                            rogueLevel++;
                            if (fasterRogueLevel && rogueLevel % 100 != 0 && Main.rand.NextBool(10)) //add only to non-multiples of 100
                                rogueLevel++;
                            shootFireworksLevelUpRogue = true;
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                            {
                                LevelPacket(false, 4);
                            }
                        }
                    }
                }
                if (raiderTalisman && raiderStack < 250 && proj.Calamity().rogue && crit)
                {
                    raiderStack++;
                }
                if (CalamityWorld.revenge && Config.AdrenalineAndRage)
                {
                    if (isTrueMelee)
                    {
                        int stressGain = (int)((double)damage * 0.1);
                        int stressMaxGain = 10;
                        if (stressGain < 1)
                        {
                            stressGain = 1;
                        }
                        if (stressGain > stressMaxGain)
                        {
                            stressGain = stressMaxGain;
                        }
                        stress += stressGain;
                        if (stress >= stressMax)
                        {
                            stress = stressMax;
                        }
                    }
                }
            }
        }
        #endregion

        #region Modify Hit By NPC
        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            int bossRushDamage = (Main.expertMode ? 500 : 300) + (CalamityWorld.bossRushStage * 2);
            if (CalamityWorld.bossRushActive)
            {
                if (damage < bossRushDamage)
                    damage = bossRushDamage;
            }
            if (triumph)
            {
                double HPMultiplier = 0.15 * (1.0 - ((double)npc.life / (double)npc.lifeMax));
                int damageReduction = (int)((double)damage * HPMultiplier);
                damage -= damageReduction;
            }
            if (aSparkRare)
            {
                if (npc.type == NPCID.BlueJellyfish || npc.type == NPCID.PinkJellyfish || npc.type == NPCID.GreenJellyfish ||
                    npc.type == NPCID.FungoFish || npc.type == NPCID.BloodJelly || npc.type == NPCID.AngryNimbus || npc.type == NPCID.GigaZapper ||
                    npc.type == NPCID.MartianTurret || npc.type == ModContent.NPCType<StormlionCharger>())
                    damage /= 2;
            }
            if (fleshTotem && fleshTotemCooldown <= 0)
            {
                fleshTotemCooldown = 1200; //20 seconds
                damage /= 2;
            }
            if (tarragonCloak && !tarragonCloakCooldown && tarraMelee)
            {
                damage /= 2;
            }
            if (bloodflareMelee && bloodflareFrenzy && !bloodFrenzyCooldown)
            {
                damage /= 2;
            }
            if (silvaMelee && silvaCountdown <= 0 && hasSilvaEffect)
            {
                damage = (int)((double)damage * 0.8);
            }
            if (aBulwarkRare)
            {
                aBulwarkRareMeleeBoostTimer += 3 * damage;
                if (aBulwarkRareMeleeBoostTimer > 900)
                    aBulwarkRareMeleeBoostTimer = 900;
            }
            if (player.whoAmI == Main.myPlayer && gainRageCooldown <= 0)
            {
                if (CalamityWorld.revenge && Config.AdrenalineAndRage && !npc.SpawnedFromStatue)
                {
                    gainRageCooldown = 60;
                    int stressGain = damage * (profanedRage ? 3 : 2);
                    int stressMaxGain = 2500;
                    if (stressGain < 1)
                    {
                        stressGain = 1;
                    }
                    if (stressGain > stressMaxGain)
                    {
                        stressGain = stressMaxGain;
                    }
                    stress += stressGain;
                    if (stress >= stressMax)
                    {
                        stress = stressMax;
                    }
                }
            }
        }
        #endregion

        #region Modify Hit By Proj
        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            if (player.HeldItem.type == ModContent.ItemType<GaelsGreatsword>()
                && proj.active && proj.hostile && player.altFunctionUse == 2 && Main.rand.NextBool(2))
            {
                for (int j = 0; j < 3; j++)
                {
                    int dustIndex = Dust.NewDust(proj.position, proj.width, proj.height, 31, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dustIndex].velocity *= 0.3f;
                }
                int damage2 = GaelsGreatsword.BaseDamage;
                proj.hostile = false;
                proj.friendly = true;
                proj.velocity *= -1f;
                proj.damage = damage2;
                proj.penetrate = 1;
                player.immune = true;
                player.immuneNoBlink = true;
                player.immuneTime = 4;
                damage = 0;
            }
            int bossRushDamage = (Main.expertMode ? 125 : 150) + (CalamityWorld.bossRushStage / 2);
            if (CalamityWorld.bossRushActive)
            {
                if (damage < bossRushDamage)
                    damage = bossRushDamage;
            }
            if (projRefRare)
            {
                if (proj.type == projTypeJustHitBy)
                    damage = (int)((double)damage * 0.85);
            }
            if (aSparkRare)
            {
                if (proj.type == ProjectileID.MartianTurretBolt || proj.type == ProjectileID.GigaZapperSpear || proj.type == ProjectileID.CultistBossLightningOrbArc ||
                    proj.type == ProjectileID.BulletSnowman || proj.type == ProjectileID.BulletDeadeye || proj.type == ProjectileID.SniperBullet)
                    damage /= 2;
            }
            if (proj.type == ProjectileID.Nail)
            {
                damage = (int)((double)damage * 0.75);
            }
            if (beeResist)
            {
                if (CalamityMod.beeProjectileList.Contains(proj.type))
                    damage = (int)((double)damage * 0.75);
            }
            if (Main.hardMode && Main.expertMode && !CalamityWorld.spawnedHardBoss && proj.active && !proj.friendly && proj.hostile && damage > 0)
            {
                if (CalamityMod.hardModeNerfList.Contains(proj.type))
                    damage = (int)((double)damage * 0.75);
            }
            if (CalamityWorld.revenge)
            {
                if (CalamityMod.revengeanceProjectileBuffList.Contains(proj.type))
                    damage = (int)((double)damage * 1.25);
            }

			// Reduce damage from vanilla traps
			// 350 in normal, 450 in expert
			if (proj.type == ProjectileID.Explosives)
				damage = (int)((double)damage * (Main.expertMode ? 0.225 : 0.35));
			if (Main.expertMode)
			{
				// 140 in normal, 182 in expert
				if (proj.type == ProjectileID.Boulder)
					damage = (int)((double)damage * 0.65);
			}

			if (player.whoAmI == Main.myPlayer && gainRageCooldown <= 0)
            {
                if (CalamityWorld.revenge && Config.AdrenalineAndRage && !CalamityMod.trapProjectileList.Contains(proj.type))
                {
                    gainRageCooldown = 60;
                    int stressGain = damage * (profanedRage ? 3 : 2);
                    int stressMaxGain = 2500;
                    if (stressGain < 1)
                    {
                        stressGain = 1;
                    }
                    if (stressGain > stressMaxGain)
                    {
                        stressGain = stressMaxGain;
                    }
                    stress += stressGain;
                    if (stress >= stressMax)
                    {
                        stress = stressMax;
                    }
                }
            }
        }
        #endregion

        #region On Hit
        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            if (CalamityWorld.revenge)
            {
                if (npc.type == NPCID.ShadowFlameApparition || (npc.type == NPCID.ChaosBall && Main.hardMode))
                {
                    player.AddBuff(ModContent.BuffType<Shadowflame>(), 180);
                }
                else if (npc.type == NPCID.SkeletronPrime || npc.type == NPCID.PrimeVice || npc.type == NPCID.PrimeSaw)
                {
                    player.AddBuff(BuffID.Bleeding, 300);
                }
                else if (npc.type == NPCID.Spazmatism && npc.ai[0] != 1f && npc.ai[0] != 2f && npc.ai[0] != 0f)
                {
                    player.AddBuff(BuffID.Bleeding, 300);
                }
                else if (npc.type == NPCID.Plantera && npc.life < npc.lifeMax / 2)
                {
                    player.AddBuff(BuffID.Poisoned, 180);
                    player.AddBuff(BuffID.Bleeding, 300);
                }
                else if (npc.type == NPCID.PlanterasTentacle)
                {
                    player.AddBuff(BuffID.Poisoned, 120);
                    player.AddBuff(BuffID.Bleeding, 180);
                }
                else if (npc.type == NPCID.Golem || npc.type == NPCID.GolemFistLeft || npc.type == NPCID.GolemFistRight ||
                    npc.type == NPCID.GolemHead || npc.type == NPCID.GolemHeadFree)
                {
                    player.AddBuff(BuffID.BrokenArmor, 180);
                }
            }
        }

        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            if (CalamityWorld.revenge && proj.hostile)
            {
				if (proj.type == ProjectileID.Explosives)
				{
					player.AddBuff(BuffID.OnFire, 600);
				}
				else if (proj.type == ProjectileID.Boulder)
				{
					player.AddBuff(BuffID.BrokenArmor, 600);
				}
				else if (proj.type == ProjectileID.FrostBeam && !player.frozen && !gState)
                {
                    player.AddBuff(ModContent.BuffType<GlacialState>(), 120);
                }
                else if (proj.type == ProjectileID.DeathLaser)
                {
                    player.AddBuff(BuffID.OnFire, 240);
                }
                else if (proj.type == ProjectileID.Skull)
                {
                    player.AddBuff(BuffID.Weak, 180);
                }
                else if (proj.type == ProjectileID.ThornBall)
                {
                    player.AddBuff(BuffID.Poisoned, 240);
                }
                else if (proj.type == ProjectileID.CultistBossIceMist)
                {
                    player.AddBuff(BuffID.Frozen, 60);
                    player.AddBuff(BuffID.Chilled, 120);
                    player.AddBuff(BuffID.Frostburn, 240);
                }
                else if (proj.type == ProjectileID.CultistBossLightningOrbArc)
                {
                    player.AddBuff(BuffID.Electrified, 120);
                }
            }
            if (projRef && proj.active && !proj.friendly && proj.hostile && damage > 0 && Main.rand.NextBool(20))
            {
                player.statLife += damage;
                player.HealEffect(damage);
                proj.hostile = false;
                proj.friendly = true;
                proj.velocity.X = -proj.velocity.X;
                proj.velocity.Y = -proj.velocity.Y;
            }
            if (projRefRare && proj.active && !proj.friendly && proj.hostile && damage > 0 && Main.rand.NextBool(2))
            {
                proj.hostile = false;
                proj.friendly = true;
                proj.velocity.X = -proj.velocity.X * 2f;
                proj.velocity.Y = -proj.velocity.Y * 2f;
                proj.damage *= 10;
                projRefRareLifeRegenCounter = 120;
                projTypeJustHitBy = proj.type;
            }
            if (aSparkRare && proj.active && !proj.friendly && proj.hostile && damage > 0)
            {
                if (proj.type == ProjectileID.BulletSnowman || proj.type == ProjectileID.BulletDeadeye || proj.type == ProjectileID.SniperBullet)
                {
                    proj.hostile = false;
                    proj.friendly = true;
                    proj.velocity.X = -proj.velocity.X;
                    proj.velocity.Y = -proj.velocity.Y;
                    proj.damage *= 8;
                }
            }
            if (daedalusReflect && proj.active && !proj.friendly && proj.hostile && damage > 0 && Main.rand.NextBool(3))
            {
                int healAmt = damage / 5;
                player.statLife += healAmt;
                player.HealEffect(healAmt);
                proj.hostile = false;
                proj.friendly = true;
                proj.velocity.X = -proj.velocity.X;
                proj.velocity.Y = -proj.velocity.Y;
            }
        }
        #endregion

        #region Fishing
        public override void CatchFish(Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk)
        {
			bool water = liquidType == 0;
			bool lava = liquidType == 1;
			bool honey = liquidType == 2;

			Point point = player.Center.ToTileCoordinates();
			bool abyssPosX = false;
			if (CalamityWorld.abyssSide)
			{
				if (point.X < 380)
				{
					abyssPosX = true;
				}
			}
			else
			{
				if (point.X > Main.maxTilesX - 380)
				{
					abyssPosX = true;
				}
			}
			if (ZoneAbyss || ZoneSulphur)
			{
				abyssPosX = true;
			}

			if (alluringBait)
			{
				int chanceForPotionFish = 1000 / power;

				if (chanceForPotionFish < 3)
					chanceForPotionFish = 3;

				if (Main.rand.NextBool(chanceForPotionFish))
				{
					List<int> fishList = new List<int>();

					if (lava)
					{
						if (!ZoneCalamity)
						{
							fishList.Add(ItemID.FlarefinKoi);
							fishList.Add(ItemID.Obsidifish);
						}
						if (ZoneCalamity)
						{
							fishList.Add(ModContent.ItemType<CoastalDemonfish>());
						}
					}
					else if (water)
					{
						if (player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight)
						{
							fishList.Add(ItemID.ArmoredCavefish);

							if (player.ZoneHoly)
							{
								fishList.Add(ItemID.ChaosFish);
							}
							if (player.ZoneJungle)
							{
								fishList.Add(ItemID.VariegatedLardfish);
							}
						}
						if (player.ZoneSnow)
						{
							fishList.Add(ItemID.FrostMinnow);
						}
						if (player.ZoneCorrupt)
						{
							fishList.Add(ItemID.Ebonkoi);
						}
						if (player.ZoneCrimson)
						{
							fishList.Add(ItemID.CrimsonTigerfish);
							fishList.Add(ItemID.Hemopiranha);
						}
						if (player.ZoneHoly)
						{
							fishList.Add(ItemID.PrincessFish);
							fishList.Add(ItemID.Prismite);
						}
						if (player.ZoneSkyHeight)
						{
							fishList.Add(ItemID.Damselfish);
						}
						if (player.ZoneJungle)
						{
							if (player.ZoneOverworldHeight || player.ZoneSkyHeight)
							{
								fishList.Add(ItemID.DoubleCod);
							}
						}
						if (ZoneAstral)
						{
							fishList.Add(ModContent.ItemType<AldebaranAlewife>());
						}
						if (ZoneSunkenSea)
						{
							fishList.Add(ModContent.ItemType<CoralskinFoolfish>());
							fishList.Add(ModContent.ItemType<SunkenSailfish>());
							fishList.Add(ModContent.ItemType<ScarredAngelfish>());
						}
					}

					if (fishList.Any())
					{
						int fishAmt = fishList.Count;
						int caughtFish = fishList[Main.rand.Next(fishAmt)];
						caughtType = caughtFish;
					}
				}
			}

			if (enchantedPearl || fishingStation)
			{
				int chanceForCrates = (enchantedPearl ? 10 : 0) +
					(fishingStation ? 10 : 0);

				int poolSizeAmt = poolSize / 10;
				if (poolSizeAmt > 100)
					poolSizeAmt = 100;

				int fishingPowerDivisor = power + poolSizeAmt;

				int chanceForIronCrate = 1000 / fishingPowerDivisor;
				int chanceForBiomeCrate = 2000 / fishingPowerDivisor;
				int chanceForGoldCrate = 3000 / fishingPowerDivisor;
				int chanceForRareItems = 4000 / fishingPowerDivisor;

				if (chanceForIronCrate < 3)
					chanceForIronCrate = 3;

				if (chanceForBiomeCrate < 4)
					chanceForBiomeCrate = 4;

				if (chanceForGoldCrate < 5)
					chanceForGoldCrate = 5;

				if (chanceForRareItems < 6)
					chanceForRareItems = 6;

				if (lava)
				{
					if (Main.rand.Next(100) < chanceForCrates)
					{
						if (Main.rand.NextBool(chanceForRareItems) && enchantedPearl && fishingStation && player.cratePotion)
						{
							if (ZoneCalamity)
							{
								caughtType = ModContent.ItemType<BrimstoneCrate>();
							}
						}
					}
				}

				if (water)
				{
					if (Main.rand.Next(100) < chanceForCrates)
					{
						if (Main.rand.NextBool(chanceForRareItems) && enchantedPearl && fishingStation && player.cratePotion)
						{
							List<int> rareItemList = new List<int>();

							if (abyssPosX)
							{
								switch (Main.rand.Next(4))
								{
									case 0:
										rareItemList.Add(ModContent.ItemType<IronBoots>());
										break;
									case 1:
										rareItemList.Add(ModContent.ItemType<DepthCharm>());
										break;
									case 2:
										rareItemList.Add(ModContent.ItemType<AnechoicPlating>());
										break;
									case 3:
										rareItemList.Add(ModContent.ItemType<StrangeOrb>());
										break;
								}
							}
							if (ZoneAstral)
							{
								switch (Main.rand.Next(3))
								{
									case 0:
										rareItemList.Add(ModContent.ItemType<GacruxianMollusk>());
										break;
									case 1:
										rareItemList.Add(ModContent.ItemType<PolarisParrotfish>());
										break;
									case 2:
										rareItemList.Add(ModContent.ItemType<UrsaSergeant>());
										break;
								}
							}
							if (ZoneSunkenSea)
							{
								switch (Main.rand.Next(2))
								{
									case 0:
										rareItemList.Add(ModContent.ItemType<SerpentsBite>());
										break;
									case 1:
										rareItemList.Add(ModContent.ItemType<RustedJingleBell>());
										break;
								}
							}
							if (player.ZoneSnow && player.ZoneRockLayerHeight && (player.ZoneCorrupt || player.ZoneCrimson || player.ZoneHoly))
							{
								rareItemList.Add(ItemID.ScalyTruffle);
							}
							if (player.ZoneCorrupt)
							{
								rareItemList.Add(ItemID.Toxikarp);
							}
							if (player.ZoneCrimson)
							{
								rareItemList.Add(ItemID.Bladetongue);
							}
							if (player.ZoneHoly)
							{
								rareItemList.Add(ItemID.CrystalSerpent);
							}

							if (rareItemList.Any())
							{
								int rareItemAmt = rareItemList.Count;
								int caughtRareItem = rareItemList[Main.rand.Next(rareItemAmt)];
								caughtType = caughtRareItem;
							}
						}
						else if (Main.rand.NextBool(chanceForGoldCrate))
						{
							caughtType = ItemID.GoldenCrate;
						}
						else if (Main.rand.NextBool(chanceForBiomeCrate))
						{
							List<int> biomeCrateList = new List<int>();

							if (ZoneAstral)
							{
								biomeCrateList.Add(ModContent.ItemType<AstralCrate>());
							}
							if (ZoneSunkenSea)
							{
								biomeCrateList.Add(ModContent.ItemType<SunkenCrate>());
							}
							if (abyssPosX)
							{
								biomeCrateList.Add(ModContent.ItemType<AbyssalCrate>());
							}
							if (player.ZoneCorrupt)
							{
								biomeCrateList.Add(ItemID.CorruptFishingCrate);
							}
							if (player.ZoneCrimson)
							{
								biomeCrateList.Add(ItemID.CrimsonFishingCrate);
							}
							if (player.ZoneHoly)
							{
								biomeCrateList.Add(ItemID.HallowedFishingCrate);
							}
							if (player.ZoneDungeon)
							{
								biomeCrateList.Add(ItemID.DungeonFishingCrate);
							}
							if (player.ZoneJungle)
							{
								biomeCrateList.Add(ItemID.JungleFishingCrate);
							}
							if (player.ZoneSkyHeight)
							{
								biomeCrateList.Add(ItemID.FloatingIslandFishingCrate);
							}


							if (biomeCrateList.Any())
							{
								int biomeCrateAmt = biomeCrateList.Count;
								int caughtBiomeCrate = biomeCrateList[Main.rand.Next(biomeCrateAmt)];
								caughtType = caughtBiomeCrate;
							}
						}
						else if (Main.rand.NextBool(chanceForIronCrate))
						{
							caughtType = ItemID.IronCrate;
						}
						else
						{
							caughtType = ItemID.WoodenCrate;
						}
						return;
					}
				}
			}

			if (water)
			{
				if (ZoneAstral) //Astral Infection, fishing in water
				{
					int astralFish = Main.rand.Next(100);
					if (caughtType == ItemID.WoodenCrate)
					{
						caughtType = ItemID.WoodenCrate;
					}
					else if (caughtType == ItemID.IronCrate)
					{
						caughtType = ItemID.IronCrate;
					}
					else if (caughtType == ItemID.GoldenCrate)
					{
						caughtType = ItemID.GoldenCrate;
					}
					else if (caughtType == ItemID.FrogLeg)
					{
						caughtType = ItemID.FrogLeg;
					}
					else if (caughtType == ItemID.BalloonPufferfish)
					{
						caughtType = ItemID.BalloonPufferfish;
					}
					else if (caughtType == ItemID.ZephyrFish)
					{
						caughtType = ItemID.ZephyrFish;
					}
					else if (astralFish >= 85) //15%
					{
						caughtType = ModContent.ItemType<ProcyonidPrawn>();
					}
					else if (astralFish <= 84 && astralFish >= 70) //15%
					{
						caughtType = ModContent.ItemType<ArcturusAstroidean>();
					}
					else if (astralFish <= 69 && astralFish >= 55) //15%
					{
						caughtType = ModContent.ItemType<AldebaranAlewife>();
					}
					else if (player.cratePotion && astralFish <= 28 && astralFish >= 9) //20%
					{
						caughtType = ModContent.ItemType<AstralCrate>();
					}
					else if (!player.cratePotion && astralFish <= 18 && astralFish >= 9) //10%
					{
						caughtType = ModContent.ItemType<AstralCrate>();
					}
					else if (astralFish <= 8 && astralFish >= 6) //3%
					{
						caughtType = ModContent.ItemType<UrsaSergeant>();
					}
					else if (astralFish <= 5 && astralFish >= 3) //3%
					{
						caughtType = ModContent.ItemType<GacruxianMollusk>();
					}
					else if (astralFish <= 2 && astralFish >= 0) //3%
					{
						caughtType = ModContent.ItemType<PolarisParrotfish>();
					}
					else //31% w/o crate pot, 21% w/ crate pot
					{
						caughtType = ModContent.ItemType<TwinklingPollox>();
					}
				}

				if (ZoneSunkenSea) //Sunken Sea, fishing in water
				{
					int sunkenFish = Main.rand.Next(100);
					if (caughtType == ItemID.WoodenCrate)
					{
						caughtType = ItemID.WoodenCrate;
					}
					else if (caughtType == ItemID.IronCrate)
					{
						caughtType = ItemID.IronCrate;
					}
					else if (caughtType == ItemID.GoldenCrate)
					{
						caughtType = ItemID.GoldenCrate;
					}
					else if (caughtType == ItemID.FrogLeg)
					{
						caughtType = ItemID.FrogLeg;
					}
					else if (caughtType == ItemID.BalloonPufferfish)
					{
						caughtType = ItemID.BalloonPufferfish;
					}
					else if (caughtType == ItemID.ZephyrFish)
					{
						caughtType = ItemID.ZephyrFish;
					}
					else if (sunkenFish >= 85 && Main.hardMode) //15%
					{
						caughtType = ModContent.ItemType<ScarredAngelfish>();
					}
					else if (sunkenFish <= 84 && sunkenFish >= 70) //15%
					{
						caughtType = ModContent.ItemType<SunkenSailfish>();
					}
					else if (sunkenFish <= 69 && sunkenFish >= 55) //15%
					{
						caughtType = ModContent.ItemType<CoralskinFoolfish>();
					}
					else if (player.cratePotion && sunkenFish <= 28 && sunkenFish >= 9) //20%
					{
						caughtType = ModContent.ItemType<SunkenCrate>();
					}
					else if (!player.cratePotion && sunkenFish <= 18 && sunkenFish >= 9) //10%
					{
						caughtType = ModContent.ItemType<SunkenCrate>();
					}
					else if (sunkenFish <= 31 && sunkenFish >= 29) //3%
					{
						caughtType = ModContent.ItemType<GreenwaveLoach>();
					}
					else if (sunkenFish <= 8 && sunkenFish >= 6 && Main.hardMode) //3%
					{
						caughtType = ModContent.ItemType<SerpentsBite>();
					}
					else if (sunkenFish <= 5 && sunkenFish >= 3) //3%
					{
						caughtType = ModContent.ItemType<RustedJingleBell>();
					}
					else if (sunkenFish <= 2 && sunkenFish >= 0) //3%
					{
						caughtType = ModContent.ItemType<SparklingEmpress>();
					}
					else //33% w/o crate pot, 23% w/ crate pot + 18% if prehardmode
					{
						caughtType = ModContent.ItemType<PrismaticGuppy>();
					}
				}

				if ((player.ZoneCrimson || player.ZoneCorrupt) && player.ZoneRockLayerHeight && Main.hardMode)
				{
					if (Main.rand.NextBool(15))
					{
						caughtType = ModContent.ItemType<FishofNight>();
					}
				}

				if (player.ZoneHoly && player.ZoneRockLayerHeight && Main.hardMode)
				{
					if (Main.rand.NextBool(15))
					{
						caughtType = ModContent.ItemType<FishofLight>();
					}
				}

				if (player.ZoneSkyHeight && Main.hardMode)
				{
					if (Main.rand.NextBool(15))
					{
						caughtType = ModContent.ItemType<FishofFlight>();
					}
				}

				if (player.ZoneOverworldHeight && !Main.dayTime)
				{
					if (Main.rand.NextBool(10))
					{
						caughtType = ModContent.ItemType<EnchantedStarfish>();
					}
				}

				if (player.ZoneOverworldHeight && Main.dayTime)
				{
					if (Main.rand.NextBool(15))
					{
						caughtType = ModContent.ItemType<StuffedFish>();
					}
				}

				if (player.ZoneRockLayerHeight)
				{
					if (Main.rand.NextBool(15))
					{
						caughtType = ModContent.ItemType<GlimmeringGemfish>();
					}
				}

				if (player.ZoneDirtLayerHeight)
				{
					if (Main.rand.NextBool(40))
					{
						caughtType = ModContent.ItemType<Spadefish>();
					}
				}

				if (power >= 60 && player.FindBuffIndex(BuffID.Gills) > -1 && NPC.downedPlantBoss && Main.rand.NextBool(25) && power < 160)
				{
					caughtType = ModContent.ItemType<Floodtide>();
				}

				if (junk)
				{
					if (abyssPosX && power < 40)
					{
						caughtType = ModContent.ItemType<PlantyMush>();
					}
					return;
				}

				/*if (abyssPosX && (bait.type == ItemID.GoldWorm || bait.type == ItemID.GoldGrasshopper || bait.type == ItemID.GoldButterfly) && power > 150)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						CalamityGlobalNPC.OldDukeSpawn(player.whoAmI, ModContent.NPCType<OldDuke>());
					}
					else
					{
						NetMessage.SendData(61, -1, -1, null, player.whoAmI, (float)ModContent.NPCType<OldDuke>(), 0f, 0f, 0, 0, 0);
					}
					switch (Main.rand.Next(4))
					{
						case 0: caughtType = ModContent.ItemType<IronBoots>(); break; //movement acc
						case 1: caughtType = ModContent.ItemType<DepthCharm>(); break; //regen acc
						case 2: caughtType = ModContent.ItemType<AnechoicPlating>(); break; //defense acc
						case 3: caughtType = ModContent.ItemType<StrangeOrb>(); break; //light pet
					}
					return;
				}*/

				if (abyssPosX)
				{
					if (caughtType == ItemID.WoodenCrate)
					{
						caughtType = ItemID.WoodenCrate;
					}
					else if (caughtType == ItemID.IronCrate)
					{
						caughtType = ItemID.IronCrate;
					}
					else if (caughtType == ItemID.GoldenCrate)
					{
						caughtType = ItemID.GoldenCrate;
					}
					else if (caughtType == ItemID.FrogLeg)
					{
						caughtType = ItemID.FrogLeg;
					}
					else if (caughtType == ItemID.BalloonPufferfish)
					{
						caughtType = ItemID.BalloonPufferfish;
					}
					else if (caughtType == ItemID.ZephyrFish)
					{
						caughtType = ItemID.ZephyrFish;
					}
					else if (power >= 40)
					{
						if (Main.rand.NextBool(15) && power < 80)
						{
							caughtType = ModContent.ItemType<PlantyMush>();
						}
						if (Main.rand.NextBool(25) && power < 160)
						{
							caughtType = ModContent.ItemType<AlluringBait>();
						}
						if (power >= 110)
						{
							if (abyssPosX && Main.rand.NextBool(25) && power < 240)
							{
								caughtType = ModContent.ItemType<AbyssalAmulet>();
							}
						}
					}
					else if (player.cratePotion && Main.rand.NextBool(5)) //20%
					{
						caughtType = ModContent.ItemType<AbyssalCrate>();
					}
					else if (!player.cratePotion && Main.rand.NextBool(10)) //10%
					{
						caughtType = ModContent.ItemType<AbyssalCrate>();
					}
				}
			}

			if (lava)
			{
				if (ZoneCalamity) //Brimstone Crags, fishing in lava
				{
					int cragFish = Main.rand.Next(100);
					if (cragFish >= 92) //8%
					{
						caughtType = ModContent.ItemType<CoastalDemonfish>();
					}
					else if (cragFish <= 91 && cragFish >= 84) //8%
					{
						caughtType = ModContent.ItemType<CragBullhead>();
					}
					else if (cragFish <= 83 && cragFish >= 76) //8%
					{
						caughtType = ModContent.ItemType<Shadowfish>();
					}
					else if (cragFish <= 75 && cragFish >= 68 && Main.hardMode) //8%
					{
						caughtType = ModContent.ItemType<ChaoticFish>();
					}
					else if (player.cratePotion && cragFish <= 40 && cragFish >= 21) //20%
					{
						caughtType = ModContent.ItemType<BrimstoneCrate>();
					}
					else if (!player.cratePotion && cragFish <= 30 && cragFish >= 21) //10%
					{
						caughtType = ModContent.ItemType<BrimstoneCrate>();
					}
					else if (cragFish <= 20 && cragFish >= 11 && CalamityWorld.downedProvidence) //10%
					{
						caughtType = ModContent.ItemType<Bloodfin>();
					}
					else if (cragFish <= 10 && cragFish >= 5) //5%
					{
						caughtType = ModContent.ItemType<DragoonDrizzlefish>();
					}
					else if (cragFish <= 2 && cragFish >= 0 && Main.hardMode) //3%
					{
						caughtType = ModContent.ItemType<CharredLasher>();
					}
					else //40% w/o crate pot, 30% w/ crate pot, add 10% pre-Prov, add another 11% prehardmode
					{
						caughtType = ModContent.ItemType<BrimstoneFish>();
					}
				}
			}

			//Quest Fish
			if (ZoneSunkenSea && questFish == ModContent.ItemType<EutrophicSandfish>() && Main.rand.NextBool(10))
			{
				caughtType = ModContent.ItemType<EutrophicSandfish>();
			}
			if (ZoneSunkenSea && questFish == ModContent.ItemType<SurfClam>() && Main.rand.NextBool(10))
			{
				caughtType = ModContent.ItemType<SurfClam>();
			}
			if (ZoneSunkenSea && questFish == ModContent.ItemType<Serpentuna>() && Main.rand.NextBool(10))
			{
				caughtType = ModContent.ItemType<Serpentuna>();
			}
			if (ZoneCalamity && questFish == ModContent.ItemType<Brimlish>() && Main.rand.NextBool(10))
			{
				caughtType = ModContent.ItemType<Brimlish>();
			}
			if (ZoneCalamity && questFish == ModContent.ItemType<Slurpfish>() && Main.rand.NextBool(10))
			{
				caughtType = ModContent.ItemType<Slurpfish>();
			}
        }

        public override void GetFishingLevel(Item fishingRod, Item bait, ref int fishingLevel)
        {
            if ((ZoneAstral || ZoneAbyss || ZoneSulphur) && bait.type == ModContent.ItemType<ArcturusAstroidean>())
                fishingLevel = (int)(fishingLevel * 1.1f);
            if (Main.player[Main.myPlayer].ZoneSnow && fishingRod.type == ModContent.ItemType<VerstaltiteFishingRod>())
                fishingLevel = (int)(fishingLevel * 1.1f);
            if (Main.player[Main.myPlayer].ZoneSkyHeight && fishingRod.type == ModContent.ItemType<HeronRod>())
                fishingLevel = (int)(fishingLevel * 1.1f);
        }
        #endregion

        #region Frame Effects
        public override void FrameEffects()
        {
            if ((snowmanPower || snowmanForce) && !snowmanHide)
            {
                player.legs = mod.GetEquipSlot("PopoLeg", EquipType.Legs);
                player.body = mod.GetEquipSlot("PopoBody", EquipType.Body);
                player.head = snowmanNoseless ? mod.GetEquipSlot("PopoNoselessHead", EquipType.Head) : mod.GetEquipSlot("PopoHead", EquipType.Head);
            }
            else if ((abyssalDivingSuitPower || abyssalDivingSuitForce) && !abyssalDivingSuitHide)
            {
                player.legs = mod.GetEquipSlot("AbyssalDivingSuitLeg", EquipType.Legs);
                player.body = mod.GetEquipSlot("AbyssalDivingSuitBody", EquipType.Body);
                player.head = mod.GetEquipSlot("AbyssalDivingSuitHead", EquipType.Head);
            }
            else if ((sirenBoobsPower || sirenBoobsForce) && !sirenBoobsHide)
            {
                player.legs = mod.GetEquipSlot("SirenLeg", EquipType.Legs);
                player.body = mod.GetEquipSlot("SirenBody", EquipType.Body);
                player.head = mod.GetEquipSlot("SirenHead", EquipType.Head);
            }
            else if ((sirenBoobsAltPower || sirenBoobsAltForce) && !sirenBoobsAltHide)
            {
                player.legs = mod.GetEquipSlot("SirenLegAlt", EquipType.Legs);
                player.body = mod.GetEquipSlot("SirenBodyAlt", EquipType.Body);
                player.head = mod.GetEquipSlot("SirenHeadAlt", EquipType.Head);
            }

            if (CalamityWorld.defiled)
                Defiled();

            if (weakPetrification)
                WeakPetrification();
        }
        #endregion

        #region Limitations
        private void WeakPetrification()
        {
            player.doubleJumpCloud = false;
            player.doubleJumpSandstorm = false;
            player.doubleJumpBlizzard = false;
            player.rocketBoots = 0;
            player.jumpBoost = false;
            player.slowFall = false;
            player.gravControl = false;
            player.gravControl2 = false;
            player.jumpSpeedBoost = 0f;
            player.wingTimeMax = (int)((double)player.wingTimeMax * 0.5);
            player.balloon = -1;
            weakPetrification = true;
        }

        private void Defiled()
        {
            player.wingTimeMax = 0;
        }
        #endregion

        #region Pre Hurt
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (CalamityWorld.armageddon || SCalLore || (CalamityWorld.bossRushActive && bossRushImmunityFrameCurseTimer > 0))
            {
                if (areThereAnyDamnBosses || SCalLore || (CalamityWorld.bossRushActive && bossRushImmunityFrameCurseTimer > 0))
                {
                    if (SCalLore)
                    {
                        string key = "Mods.CalamityMod.SupremeBossText2";
                        Color messageColor = Color.Orange;
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                        else if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                        }
                    }
                    KillPlayer();
                }
            }

            if (lol || (invincible && player.inventory[player.selectedItem].type != ModContent.ItemType<ColdheartIcicle>()))
            {
                return false;
            }
            if (godSlayerReflect && Main.rand.NextBool(50))
            {
                return false;
            }

            if ((abyssalDivingSuitPower || abyssalDivingSuitForce) && !abyssalDivingSuitHide)
            {
                playSound = false;
                Main.PlaySound(3, (int)player.position.X, (int)player.position.Y, 4, 1f, 0f); //metal hit noise
            }
            else if (((sirenBoobsPower || sirenBoobsForce) && !sirenBoobsHide) || ((sirenBoobsAltPower || sirenBoobsAltForce) && !sirenBoobsAltHide))
            {
                playSound = false;
                Main.PlaySound(20, (int)player.position.X, (int)player.position.Y, 1, 1f, 0f); //female hit noise
            }

            #region MultiplierBoosts
            double damageMult = 1.0 +
                (dArtifact ? 0.25 : 0.0) +
                ((player.beetleDefense && player.beetleOrbs > 0) ? (0.05 * (double)player.beetleOrbs) : 0.0) +
                (enraged ? 0.25 : 0.0) +
                ((CalamityWorld.defiled && Main.rand.NextBool(4)) ? 0.5 : 0.0) +
                ((bloodPact && Main.rand.NextBool(4)) ? 1.5 : 0.0);

            if (CalamityWorld.revenge)
            {
                if (player.chaosState)
                    damageMult += 0.25;
                if (CalamityWorld.death)
                    damageMult += 0.15;
                if (player.ichor)
                    damageMult += 0.25;
                else if (player.onFire2)
                    damageMult += 0.2;
            }

            damage = (int)((double)damage * damageMult);
            #endregion

            if (CalamityWorld.revenge)
            {
                customDamage = true;
                double newDamage = (double)damage - ((double)player.statDefense * 0.75);
                double newDamageLimit = 5.0 + (Main.hardMode ? 5.0 : 0.0) + (NPC.downedPlantBoss ? 5.0 : 0.0) + (NPC.downedMoonlord ? 5.0 : 0.0); //5, 10, 15, 20
                if (newDamage < newDamageLimit)
                {
                    newDamage = newDamageLimit;
                }
                damage = (int)newDamage;
            }

            #region MultiplicativeReductions
            if (trinketOfChiBuff)
            {
                damage = (int)((double)damage * 0.85);
            }
            if (purpleCandle)
            {
                damage = (int)((double)damage - ((double)player.statDefense * 0.05));
            }
            if (abyssalDivingSuitPlates)
            {
                damage = (int)((double)damage * 0.85);
            }
            if (sirenIce)
            {
                damage = (int)((double)damage * 0.85);
            }
            if (CalamityWorld.revenge)
            {
                if (!CalamityWorld.downedBossAny)
                    damage = (int)((double)damage * 0.8);
            }
            if (player.mount.Active && (player.mount.Type == ModContent.MountType<AngryDogMount>() || player.mount.Type == ModContent.MountType<OnyxExcavator>()) && Math.Abs(player.velocity.X) > player.mount.RunSpeed / 2f)
            {
                damage = (int)((double)damage * 0.9);
            }
            #endregion

            if ((godSlayerDamage && damage <= 80) || damage < 1)
            {
                damage = 1;
            }

            #region HealingEffects
            if (revivify)
            {
                int healAmt = damage / 15;
                player.statLife += healAmt;
                player.HealEffect(healAmt);
            }
            if (daedalusAbsorb && Main.rand.NextBool(10))
            {
                int healAmt = damage / 2;
                player.statLife += healAmt;
                player.HealEffect(healAmt);
            }
            if (absorber)
            {
                int healAmt = damage / 20;
                player.statLife += healAmt;
                player.HealEffect(healAmt);
            }
            #endregion

            return true;
        }
        #endregion

        #region Hurt
        public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            modStealth = 1f;
            if (player.whoAmI == Main.myPlayer)
            {
                if (rageMode)
                {
                    stress = 0;
                    if (player.FindBuffIndex(ModContent.BuffType<RageMode>()) > -1)
						player.ClearBuff(ModContent.BuffType<RageMode>());
                }
                if (amidiasBlessing)
                {
                    player.ClearBuff(ModContent.BuffType<AmidiasBlessing>());
                    Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 96);
                }
                if (!adrenalineMode && adrenaline != adrenalineMax)
                {
                    adrenaline = 0;
                }
                if ((gShell || fabledTortoise) && !player.panic)
                {
                    player.AddBuff(ModContent.BuffType<ShellBoost>(), 300);
                }
                if (abyssalDivingSuitPlates && damage > 50)
                {
                    abyssalDivingSuitPlateHits++;
                    if (abyssalDivingSuitPlateHits >= 3)
                    {
                        Main.PlaySound(4, (int)player.position.X, (int)player.position.Y, 14);
                        player.AddBuff(ModContent.BuffType<AbyssalDivingSuitPlatesBroken>(), 10830);
                        for (int num621 = 0; num621 < 20; num621++)
                        {
                            int num622 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 31, 0f, 0f, 100, default, 2f);
                            Main.dust[num622].velocity *= 3f;
                            if (Main.rand.NextBool(2))
                            {
                                Main.dust[num622].scale = 0.5f;
                                Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                            }
                        }
                        for (int num623 = 0; num623 < 35; num623++)
                        {
                            int num624 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 6, 0f, 0f, 100, default, 3f);
                            Main.dust[num624].noGravity = true;
                            Main.dust[num624].velocity *= 5f;
                            num624 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 6, 0f, 0f, 100, default, 2f);
                            Main.dust[num624].velocity *= 2f;
                        }
                        for (int num625 = 0; num625 < 3; num625++)
                        {
                            float scaleFactor10 = 0.33f;
                            if (num625 == 1)
                            {
                                scaleFactor10 = 0.66f;
                            }
                            if (num625 == 2)
                            {
                                scaleFactor10 = 1f;
                            }
                            int num626 = Gore.NewGore(new Vector2(player.position.X + (float)(player.width / 2) - 24f, player.position.Y + (float)(player.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                            Main.gore[num626].velocity *= scaleFactor10;
                            Gore expr_13AB6_cp_0 = Main.gore[num626];
                            expr_13AB6_cp_0.velocity.X += 1f;
                            Gore expr_13AD6_cp_0 = Main.gore[num626];
                            expr_13AD6_cp_0.velocity.Y += 1f;
                            num626 = Gore.NewGore(new Vector2(player.position.X + (float)(player.width / 2) - 24f, player.position.Y + (float)(player.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                            Main.gore[num626].velocity *= scaleFactor10;
                            Gore expr_13B79_cp_0 = Main.gore[num626];
                            expr_13B79_cp_0.velocity.X -= 1f;
                            Gore expr_13B99_cp_0 = Main.gore[num626];
                            expr_13B99_cp_0.velocity.Y += 1f;
                            num626 = Gore.NewGore(new Vector2(player.position.X + (float)(player.width / 2) - 24f, player.position.Y + (float)(player.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                            Main.gore[num626].velocity *= scaleFactor10;
                            Gore expr_13C3C_cp_0 = Main.gore[num626];
                            expr_13C3C_cp_0.velocity.X += 1f;
                            Gore expr_13C5C_cp_0 = Main.gore[num626];
                            expr_13C5C_cp_0.velocity.Y -= 1f;
                            num626 = Gore.NewGore(new Vector2(player.position.X + (float)(player.width / 2) - 24f, player.position.Y + (float)(player.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                            Main.gore[num626].velocity *= scaleFactor10;
                            Gore expr_13CFF_cp_0 = Main.gore[num626];
                            expr_13CFF_cp_0.velocity.X -= 1f;
                            Gore expr_13D1F_cp_0 = Main.gore[num626];
                            expr_13D1F_cp_0.velocity.Y -= 1f;
                        }
                    }
                }
                if (sirenIce)
                {
                    Main.PlaySound(4, (int)player.position.X, (int)player.position.Y, 7);
                    player.AddBuff(ModContent.BuffType<IceShieldBrokenBuff>(), 1800);
                    for (int num621 = 0; num621 < 10; num621++)
                    {
                        int num622 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 67, 0f, 0f, 100, default, 2f);
                        Main.dust[num622].velocity *= 3f;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num622].scale = 0.5f;
                            Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int num623 = 0; num623 < 15; num623++)
                    {
                        int num624 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 67, 0f, 0f, 100, default, 3f);
                        Main.dust[num624].noGravity = true;
                        Main.dust[num624].velocity *= 5f;
                        num624 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 67, 0f, 0f, 100, default, 2f);
                        Main.dust[num624].velocity *= 2f;
                    }
                }
                if (tarraMelee)
                {
                    if (Main.rand.NextBool(4))
                    {
                        player.AddBuff(ModContent.BuffType<TarraLifeRegen>(), 120);
                    }
                }
                else if (xerocSet)
                {
                    player.AddBuff(ModContent.BuffType<XerocRage>(), 240);
                    player.AddBuff(ModContent.BuffType<XerocWrath>(), 240);
                }
                else if (reaverBlast)
                {
                    player.AddBuff(ModContent.BuffType<ReaverRage>(), 180);
                }
                if (fBarrier || ((sirenBoobs || sirenBoobsAlt) && NPC.downedBoss3))
                {
                    Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 27);
                    for (int m = 0; m < 200; m++)
                    {
                        if (Main.npc[m].active && !Main.npc[m].friendly)
                        {
                            float distance = (Main.npc[m].Center - player.Center).Length();
                            float num10 = (float)Main.rand.Next(200 + (int)damage / 2, 301 + (int)damage * 2);
                            if (num10 > 500f)
                            {
                                num10 = 500f + (num10 - 500f) * 0.75f;
                            }
                            if (num10 > 700f)
                            {
                                num10 = 700f + (num10 - 700f) * 0.5f;
                            }
                            if (num10 > 900f)
                            {
                                num10 = 900f + (num10 - 900f) * 0.25f;
                            }
                            if (distance < num10)
                            {
                                float num11 = (float)Main.rand.Next(90 + (int)damage / 3, 240 + (int)damage / 2);
                                Main.npc[m].AddBuff(ModContent.BuffType<GlacialState>(), (int)num11, false);
                            }
                        }
                    }
                }
                if (aBrain)
                {
                    for (int m = 0; m < 200; m++)
                    {
                        if (Main.npc[m].active && !Main.npc[m].friendly)
                        {
                            float arg_67A_0 = (Main.npc[m].Center - player.Center).Length();
                            float num10 = (float)Main.rand.Next(200 + (int)damage / 2, 301 + (int)damage * 2);
                            if (num10 > 500f)
                            {
                                num10 = 500f + (num10 - 500f) * 0.75f;
                            }
                            if (num10 > 700f)
                            {
                                num10 = 700f + (num10 - 700f) * 0.5f;
                            }
                            if (num10 > 900f)
                            {
                                num10 = 900f + (num10 - 900f) * 0.25f;
                            }
                            if (arg_67A_0 < num10)
                            {
                                float num11 = (float)Main.rand.Next(90 + (int)damage / 3, 300 + (int)damage / 2);
                                Main.npc[m].AddBuff(31, (int)num11, false);
                            }
                        }
                    }
                    Projectile.NewProjectile(player.Center.X + (float)Main.rand.Next(-40, 40), player.Center.Y - (float)Main.rand.Next(20, 60), player.velocity.X * 0.3f, player.velocity.Y * 0.3f, 565, 0, 0f, player.whoAmI, 0f, 0f);
                }
                if (polarisBoost)
                {
                    polarisBoostCounter = 0;
                    polarisBoost = false;
                    polarisBoostTwo = false;
                    polarisBoostThree = false;
                    if (player.FindBuffIndex(ModContent.BuffType<PolarisBuff>()) > -1)
                    { player.ClearBuff(ModContent.BuffType<PolarisBuff>()); }
                }
            }
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Ranged.DrataliornusBow>()] != 0)
            {
                for (int i = 0; i < 1000; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<Projectiles.Ranged.DrataliornusBow>() && Main.projectile[i].owner == player.whoAmI)
                    {
                        Main.projectile[i].Kill();
                        break;
                    }
                }
                if (player.wingTime > player.wingTimeMax / 2)
                    player.wingTime = player.wingTimeMax / 2;
            }
        }
        #endregion

        #region Post Hurt
        public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            if (pArtifact)
            {
                player.AddBuff(ModContent.BuffType<BurntOut>(), 300, true);
            }
            bool hardMode = Main.hardMode;
            if (player.whoAmI == Main.myPlayer)
            {
                if (cTracers && damage > 200)
                {
                    player.immuneTime += 60;
                }
                if (godSlayerThrowing && damage > 80)
                {
                    player.immuneTime += 30;
                }
                if (statigelSet && damage > 100)
                {
                    player.immuneTime += 30;
                }
                if (dAmulet)
                {
                    if (damage == 1.0)
                    {
                        player.immuneTime += 10;
                    }
                    else
                    {
                        player.immuneTime += 20;
                    }
                }
                if (fabsolVodka)
                {
                    if (damage == 1.0)
                    {
                        player.immuneTime += 5;
                    }
                    else
                    {
                        player.immuneTime += 10;
                    }
                }
                if (CalamityWorld.bossRushActive && Config.BossRushImmunityFrameCurse)
                {
                    bossRushImmunityFrameCurseTimer = 300 + player.immuneTime;
                }
                if (damage > 25)
                {
                    if (aeroSet)
                    {
                        for (int n = 0; n < 4; n++)
                        {
                            float x = player.position.X + (float)Main.rand.Next(-400, 400);
                            float y = player.position.Y - (float)Main.rand.Next(500, 800);
                            Vector2 vector = new Vector2(x, y);
                            float num13 = player.position.X + (float)(player.width / 2) - vector.X;
                            float num14 = player.position.Y + (float)(player.height / 2) - vector.Y;
                            num13 += (float)Main.rand.Next(-100, 101);
                            int num15 = 20;
                            float num16 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
                            num16 = (float)num15 / num16;
                            num13 *= num16;
                            num14 *= num16;
                            int num17 = Projectile.NewProjectile(x, y, num13, num14, ModContent.ProjectileType<StickyFeatherAero>(), 20, 1f, player.whoAmI, 0f, 0f);
                        }
                    }
                }
                if (aBulwark)
                {
                    if (aBulwarkRare)
                    {
                        Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 74);
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<GodSlayerBlaze>(), 25, 5f, player.whoAmI, 0f, 1f);
                    }
                    int starAmt = aBulwarkRare ? 12 : 5;
                    for (int n = 0; n < starAmt; n++)
                    {
                        float x = player.position.X + (float)Main.rand.Next(-400, 400);
                        float y = player.position.Y - (float)Main.rand.Next(500, 800);
                        Vector2 vector = new Vector2(x, y);
                        float num13 = player.position.X + (float)(player.width / 2) - vector.X;
                        float num14 = player.position.Y + (float)(player.height / 2) - vector.Y;
                        num13 += (float)Main.rand.Next(-100, 101);
                        int num15 = 29;
                        float num16 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
                        num16 = (float)num15 / num16;
                        num13 *= num16;
                        num14 *= num16;
                        int num17 = Projectile.NewProjectile(x, y, num13, num14, ModContent.ProjectileType<AstralStar>(), 320, 5f, player.whoAmI, 0f, 0f);
                    }
                }
                if (dAmulet)
                {
                    for (int n = 0; n < 3; n++)
                    {
                        float x = player.position.X + (float)Main.rand.Next(-400, 400);
                        float y = player.position.Y - (float)Main.rand.Next(500, 800);
                        Vector2 vector = new Vector2(x, y);
                        float num13 = player.position.X + (float)(player.width / 2) - vector.X;
                        float num14 = player.position.Y + (float)(player.height / 2) - vector.Y;
                        num13 += (float)Main.rand.Next(-100, 101);
                        int num15 = 29;
                        float num16 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
                        num16 = (float)num15 / num16;
                        num13 *= num16;
                        num14 *= num16;
                        int num17 = Projectile.NewProjectile(x, y, num13, num14, 92, 130, 4f, player.whoAmI, 0f, 0f);
                        Main.projectile[num17].usesLocalNPCImmunity = true;
                        Main.projectile[num17].localNPCHitCooldown = 5;
                        Main.projectile[num17].ranged = false;
                    }
                }
            }
            if (fCarapace)
            {
                if (damage > 0)
                {
                    Main.PlaySound(3, (int)player.position.X, (int)player.position.Y, 45);
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    int i;
                    int fDamage = 56;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (i = 0; i < 4; i++)
                        {
                            float xPos = Main.rand.NextBool(2) ? player.Center.X + 100 : player.Center.X - 100;
                            Vector2 vector2 = new Vector2(xPos, player.Center.Y + Main.rand.Next(-100, 101));
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            int spore1 = Projectile.NewProjectile(vector2.X, vector2.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), 590, fDamage, 1.25f, player.whoAmI, 0f, 0f);
                            int spore2 = Projectile.NewProjectile(vector2.X, vector2.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), 590, fDamage, 1.25f, player.whoAmI, 0f, 0f);
                            Main.projectile[spore1].timeLeft = 120;
                            Main.projectile[spore2].timeLeft = 120;
                        }
                    }
                }
            }
            if (aSpark)
            {
                if (damage > 0)
                {
                    Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 93);
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    int i;
                    int sDamage = hardMode ? 36 : 6;
                    if (aSparkRare)
                        sDamage += hardMode ? 12 : 2;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (i = 0; i < 4; i++)
                        {
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            int spark1 = Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<Spark>(), sDamage, 1.25f, player.whoAmI, 0f, 0f);
                            int spark2 = Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<Spark>(), sDamage, 1.25f, player.whoAmI, 0f, 0f);
                            Main.projectile[spark1].timeLeft = 120;
                            Main.projectile[spark2].timeLeft = 120;
                        }
                    }
                }
            }
            if (inkBomb)
            {
                if (player.whoAmI == Main.myPlayer && !inkBombCooldown)
                {
                    player.AddBuff(ModContent.BuffType<InkBombCooldown>(), 1200);
                    rogueStealth += 0.5f;
                    for (int i = 0; i < 5; i++)
                    {
                        Main.PlaySound(2, (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y, 61);
                        int inkBomb = Projectile.NewProjectile(player.Center.X, player.Center.Y, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-0f, -4f), ModContent.ProjectileType<InkBombProjectile>(), 0, 0, player.whoAmI);
                    }
                }
            }
            if (ataxiaBlaze && Main.rand.NextBool(5))
            {
                if (damage > 0)
                {
                    Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 74);
                    int eDamage = 100;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<ChaosBlaze>(), eDamage, 1f, player.whoAmI, 0f, 0f);
                    }
                }
            }
            else if (daedalusShard)
            {
                if (damage > 0)
                {
                    Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 27);
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    int i;
                    int sDamage = 27;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (i = 0; i < 8; i++)
                        {
                            float randomSpeed = (float)Main.rand.Next(1, 7);
                            float randomSpeed2 = (float)Main.rand.Next(1, 7);
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            int shard = Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f) + randomSpeed, 90, sDamage, 1f, player.whoAmI, 0f, 0f);
                            int shard2 = Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f) + randomSpeed2, 90, sDamage, 1f, player.whoAmI, 0f, 0f);
                            Main.projectile[shard].ranged = false;
                            Main.projectile[shard2].ranged = false;
                        }
                    }
                }
            }
            else if (reaverSpore)
            {
                if (damage > 0)
                {
                    Main.PlaySound(3, (int)player.position.X, (int)player.position.Y, 1);
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    int i;
                    int rDamage = 58;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (i = 0; i < 4; i++)
                        {
                            float xPos = Main.rand.NextBool(2) ? player.Center.X + 100 : player.Center.X - 100;
                            Vector2 vector2 = new Vector2(xPos, player.Center.Y + Main.rand.Next(-100, 101));
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            int rspore1 = Projectile.NewProjectile(vector2.X, vector2.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), 567, rDamage, 2f, player.whoAmI, 0f, 0f);
                            Main.projectile[rspore1].usesLocalNPCImmunity = true;
                            Main.projectile[rspore1].localNPCHitCooldown = 60;
                            int rspore2 = Projectile.NewProjectile(vector2.X, vector2.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), 568, rDamage, 2f, player.whoAmI, 0f, 0f);
                            Main.projectile[rspore2].usesLocalNPCImmunity = true;
                            Main.projectile[rspore2].localNPCHitCooldown = 60;
                        }
                    }
                }
            }
            else if (godSlayerDamage)
            {
                if (damage > 80)
                {
                    Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 73);
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    int i;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (i = 0; i < 4; i++)
                        {
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<GodKiller>(), 900, 5f, player.whoAmI, 0f, 0f);
                            Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<GodKiller>(), 900, 5f, player.whoAmI, 0f, 0f);
                        }
                    }
                }
            }
            else if (godSlayerMage)
            {
                if (damage > 0)
                {
                    Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 74);
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<GodSlayerBlaze>(), auricSet ? 2400 : 1200, 1f, player.whoAmI, 0f, 0f);
                    }
                }
            }
            else if (dsSetBonus)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    for (int l = 0; l < 2; l++)
                    {
                        float x = player.position.X + (float)Main.rand.Next(-400, 400);
                        float y = player.position.Y - (float)Main.rand.Next(500, 800);
                        Vector2 vector = new Vector2(x, y);
                        float num15 = player.position.X + (float)(player.width / 2) - vector.X;
                        float num16 = player.position.Y + (float)(player.height / 2) - vector.Y;
                        num15 += (float)Main.rand.Next(-100, 101);
                        int num17 = 22;
                        float num18 = (float)Math.Sqrt((double)(num15 * num15 + num16 * num16));
                        num18 = (float)num17 / num18;
                        num15 *= num18;
                        num16 *= num18;
                        int num19 = Projectile.NewProjectile(x, y, num15, num16, 294, 3000, 7f, player.whoAmI, 0f, 0f);
                        Main.projectile[num19].ai[1] = player.position.Y;
                    }
                    for (int l = 0; l < 5; l++)
                    {
                        float x = player.position.X + (float)Main.rand.Next(-400, 400);
                        float y = player.position.Y - (float)Main.rand.Next(500, 800);
                        Vector2 vector = new Vector2(x, y);
                        float num15 = player.position.X + (float)(player.width / 2) - vector.X;
                        float num16 = player.position.Y + (float)(player.height / 2) - vector.Y;
                        num15 += (float)Main.rand.Next(-100, 101);
                        int num17 = 22;
                        float num18 = (float)Math.Sqrt((double)(num15 * num15 + num16 * num16));
                        num18 = (float)num17 / num18;
                        num15 *= num18;
                        num16 *= num18;
                        int num19 = Projectile.NewProjectile(x, y, num15, num16, 45, 5000, 7f, player.whoAmI, 0f, 0f);
                        Main.projectile[num19].ai[1] = player.position.Y;
                    }
                }
            }
        }
        #endregion

        #region Kill Player
        public void KillPlayer()
        {
            deathCount++;
            if (player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
            {
                DeathPacket(false);
            }
            player.lastDeathPostion = player.Center;
            player.lastDeathTime = DateTime.Now;
            player.showLastDeath = true;
            bool specialDeath = CalamityWorld.ironHeart && areThereAnyDamnBosses;
            int coinsOwned = (int)Utils.CoinsCount(out bool flag, player.inventory, new int[0]);
            if (Main.myPlayer == player.whoAmI)
            {
                player.lostCoins = coinsOwned;
                player.lostCoinString = Main.ValueToCoins(player.lostCoins);
            }
            if (Main.myPlayer == player.whoAmI)
            {
                Main.mapFullscreen = false;
            }
            if (Main.myPlayer == player.whoAmI)
            {
                player.trashItem.SetDefaults(0, false);
                if (specialDeath)
                {
                    player.difficulty = 2;
                    player.DropItems();
                    player.KillMeForGood();
                }
                else if (player.difficulty == 0)
                {
                    for (int i = 0; i < 59; i++)
                    {
                        if (player.inventory[i].stack > 0 && ((player.inventory[i].type >= 1522 && player.inventory[i].type <= 1527) || player.inventory[i].type == 3643))
                        {
                            int num = Item.NewItem((int)player.position.X, (int)player.position.Y, player.width, player.height, player.inventory[i].type, 1, false, 0, false, false);
                            Main.item[num].netDefaults(player.inventory[i].netID);
                            Main.item[num].Prefix((int)player.inventory[i].prefix);
                            Main.item[num].stack = player.inventory[i].stack;
                            Main.item[num].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
                            Main.item[num].velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
                            Main.item[num].noGrabDelay = 100;
                            Main.item[num].favorited = false;
                            Main.item[num].newAndShiny = false;
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                            {
                                NetMessage.SendData(21, -1, -1, null, num, 0f, 0f, 0f, 0, 0, 0);
                            }
                            player.inventory[i].SetDefaults(0, false);
                        }
                    }
                }
                else if (player.difficulty == 1)
                {
                    player.DropItems();
                }
                else if (player.difficulty == 2)
                {
                    player.DropItems();
                    player.KillMeForGood();
                }
            }
            if (specialDeath)
            {
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/IronHeartDeath"), (int)player.position.X, (int)player.position.Y);
            }
            else
            {
                Main.PlaySound(5, (int)player.position.X, (int)player.position.Y, 1, 1f, 0f);
            }
            player.headVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
            player.bodyVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
            player.legVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
            player.headVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * 0);
            player.bodyVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * 0);
            player.legVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * 0);
            if (player.stoned)
            {
                player.headPosition = Vector2.Zero;
                player.bodyPosition = Vector2.Zero;
                player.legPosition = Vector2.Zero;
            }
            for (int j = 0; j < 100; j++)
            {
                Dust.NewDust(player.position, player.width, player.height, specialDeath ? 91 : 235, (float)(2 * 0), -2f, 0, default, 1f);
            }
            player.mount.Dismount(player);
            player.dead = true;
            player.respawnTimer = 600;
            if (Main.expertMode)
            {
                player.respawnTimer = (int)((double)player.respawnTimer * 1.5);
            }
            player.immuneAlpha = 0;
            player.palladiumRegen = false;
            player.iceBarrier = false;
            player.crystalLeaf = false;
            if (abyssDeath)
            {
                if (Main.rand.NextBool(2))
                {
                    PlayerDeathReason damageSource = PlayerDeathReason.ByCustomReason(player.name + " is food for the Wyrms.");
                    NetworkText deathText = damageSource.GetDeathText(player.name);
                    if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
                    {
                        NetMessage.SendPlayerDeath(player.whoAmI, damageSource, (int)1000.0, 0, false, -1, -1);
                    }
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(deathText, new Color(225, 25, 25), -1);
                    }
                    else if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(deathText.ToString(), 225, 25, 25, false);
                    }
                    if (player.whoAmI == Main.myPlayer && player.difficulty == 0)
                    {
                        player.DropCoins();
                    }
                    player.DropTombstone(coinsOwned, deathText, 0);
                }
                else
                {
                    PlayerDeathReason damageSource = PlayerDeathReason.ByCustomReason("Oxygen failed to reach " + player.name + " from the depths of the Abyss.");
                    NetworkText deathText = damageSource.GetDeathText(player.name);
                    if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
                    {
                        NetMessage.SendPlayerDeath(player.whoAmI, damageSource, (int)1000.0, 0, false, -1, -1);
                    }
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(deathText, new Color(225, 25, 25), -1);
                    }
                    else if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(deathText.ToString(), 225, 25, 25, false);
                    }
                    if (player.whoAmI == Main.myPlayer && player.difficulty == 0)
                    {
                        player.DropCoins();
                    }
                    player.DropTombstone(coinsOwned, deathText, 0);
                }
            }
            else if (specialDeath)
            {
                PlayerDeathReason damageSource = PlayerDeathReason.ByCustomReason(player.name + " was defeated.");
                NetworkText deathText = damageSource.GetDeathText(player.name);
                if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
                {
                    NetMessage.SendPlayerDeath(player.whoAmI, damageSource, (int)1000.0, 0, false, -1, -1);
                }
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(deathText, new Color(225, 25, 25), -1);
                }
                else if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(deathText.ToString(), 225, 25, 25, false);
                }
                if (player.whoAmI == Main.myPlayer && player.difficulty == 0)
                {
                    player.DropCoins();
                }
                player.DropTombstone(coinsOwned, deathText, 0);
            }
            else if (SCalLore)
            {
                PlayerDeathReason damageSource = PlayerDeathReason.ByCustomReason(player.Male ? player.name + " was consumed by his inner hatred." : player.name + " was consumed by her inner hatred.");
                NetworkText deathText = damageSource.GetDeathText(player.name);
                if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
                {
                    NetMessage.SendPlayerDeath(player.whoAmI, damageSource, (int)1000.0, 0, false, -1, -1);
                }
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(deathText, new Color(225, 25, 25), -1);
                }
                else if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(deathText.ToString(), 225, 25, 25, false);
                }
                if (player.whoAmI == Main.myPlayer && player.difficulty == 0)
                {
                    player.DropCoins();
                }
                player.DropTombstone(coinsOwned, deathText, 0);
            }
            else if (CalamityWorld.armageddon && areThereAnyDamnBosses)
            {
                PlayerDeathReason damageSource = PlayerDeathReason.ByCustomReason(player.name + " failed the challenge at hand.");
                NetworkText deathText = damageSource.GetDeathText(player.name);
                if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
                {
                    NetMessage.SendPlayerDeath(player.whoAmI, damageSource, (int)1000.0, 0, false, -1, -1);
                }
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(deathText, new Color(225, 25, 25), -1);
                }
                else if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(deathText.ToString(), 225, 25, 25, false);
                }
                if (player.whoAmI == Main.myPlayer && player.difficulty == 0)
                {
                    player.DropCoins();
                }
                player.DropTombstone(coinsOwned, deathText, 0);
            }
            else if (CalamityWorld.bossRushActive && bossRushImmunityFrameCurseTimer > 0)
            {
                PlayerDeathReason damageSource = PlayerDeathReason.ByCustomReason(player.name + " was destroyed by a mysterious force.");
                NetworkText deathText = damageSource.GetDeathText(player.name);
                if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
                {
                    NetMessage.SendPlayerDeath(player.whoAmI, damageSource, (int)1000.0, 0, false, -1, -1);
                }
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(deathText, new Color(225, 25, 25), -1);
                }
                else if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(deathText.ToString(), 225, 25, 25, false);
                }
                if (player.whoAmI == Main.myPlayer && player.difficulty == 0)
                {
                    player.DropCoins();
                }
                player.DropTombstone(coinsOwned, deathText, 0);
            }
            else
            {
                PlayerDeathReason damageSource = PlayerDeathReason.ByOther(player.Male ? 14 : 15);
                NetworkText deathText = damageSource.GetDeathText(player.name);
                if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
                {
                    NetMessage.SendPlayerDeath(player.whoAmI, damageSource, (int)1000.0, 0, false, -1, -1);
                }
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(deathText, new Color(225, 25, 25), -1);
                }
                else if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(deathText.ToString(), 225, 25, 25, false);
                }
                if (player.whoAmI == Main.myPlayer && player.difficulty == 0)
                {
                    player.DropCoins();
                }
                player.DropTombstone(coinsOwned, deathText, 0);
            }
            if (player.whoAmI == Main.myPlayer)
            {
                try
                {
                    WorldGen.saveToonWhilePlaying();
                } catch
                {
                }
            }
        }
        #endregion

        #region Dash Stuff
        public void ModDashMovement()
        {
            if (dashMod == 6 && player.dashDelay < 0 && player.whoAmI == Main.myPlayer)
            {
                Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && !Main.npc[i].friendly && Main.npc[i].immune[player.whoAmI] <= 0)
                    {
                        NPC nPC = Main.npc[i];
                        Rectangle rect = nPC.getRect();
                        if (rectangle.Intersects(rect) && (nPC.noTileCollide || player.CanHit(nPC)))
                        {
                            float num = 50f * player.meleeDamage;
                            float num2 = 3f;
                            bool crit = false;
                            if (player.kbGlove)
                            {
                                num2 *= 2f;
                            }
                            if (player.kbBuff)
                            {
                                num2 *= 1.5f;
                            }
                            if (Main.rand.Next(100) < player.meleeCrit)
                            {
                                crit = true;
                            }
                            int direction = player.direction;
                            if (player.velocity.X < 0f)
                            {
                                direction = -1;
                            }
                            if (player.velocity.X > 0f)
                            {
                                direction = 1;
                            }
                            if (player.whoAmI == Main.myPlayer)
                            {
                                player.ApplyDamageToNPC(nPC, (int)num, num2, direction, crit);
                            }
                            nPC.immune[player.whoAmI] = 6;
                            nPC.AddBuff(ModContent.BuffType<GlacialState>(), 300);
                            player.immune = true;
                            player.immuneNoBlink = true;
                            player.immuneTime = 4;
                        }
                    }
                }
            }
            if (dashMod == 4 && player.dashDelay < 0 && player.whoAmI == Main.myPlayer)
            {
                Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && !Main.npc[i].friendly && Main.npc[i].immune[player.whoAmI] <= 0)
                    {
                        NPC nPC = Main.npc[i];
                        Rectangle rect = nPC.getRect();
                        if (rectangle.Intersects(rect) && (nPC.noTileCollide || player.CanHit(nPC)))
                        {
                            float num = 1500f * player.meleeDamage;
                            float num2 = 15f;
                            bool crit = false;
                            if (player.kbGlove)
                            {
                                num2 *= 2f;
                            }
                            if (player.kbBuff)
                            {
                                num2 *= 1.5f;
                            }
                            if (Main.rand.Next(100) < player.meleeCrit)
                            {
                                crit = true;
                            }
                            int direction = player.direction;
                            if (player.velocity.X < 0f)
                            {
                                direction = -1;
                            }
                            if (player.velocity.X > 0f)
                            {
                                direction = 1;
                            }
                            if (player.whoAmI == Main.myPlayer)
                            {
                                player.ApplyDamageToNPC(nPC, (int)num, num2, direction, crit);
                                int num6 = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<HolyExplosionSupreme>(), 1000, 20f, Main.myPlayer, 0f, 0f);
                                Main.projectile[num6].Kill();
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<HolyEruption>(), 780, 5f, Main.myPlayer, 0f, 0f);
                            }
                            nPC.immune[player.whoAmI] = 6;
                            nPC.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);
                            player.immune = true;
                            player.immuneNoBlink = true;
                            player.immuneTime = 4;
                        }
                    }
                }
            }
            if (dashMod == 3 && player.dashDelay < 0 && player.whoAmI == Main.myPlayer)
            {
                Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && !Main.npc[i].friendly && Main.npc[i].immune[player.whoAmI] <= 0)
                    {
                        NPC nPC = Main.npc[i];
                        Rectangle rect = nPC.getRect();
                        if (rectangle.Intersects(rect) && (nPC.noTileCollide || player.CanHit(nPC)))
                        {
                            float num = 500f * player.meleeDamage;
                            float num2 = 12f;
                            bool crit = false;
                            if (player.kbGlove)
                            {
                                num2 *= 2f;
                            }
                            if (player.kbBuff)
                            {
                                num2 *= 1.5f;
                            }
                            if (Main.rand.Next(100) < player.meleeCrit)
                            {
                                crit = true;
                            }
                            int direction = player.direction;
                            if (player.velocity.X < 0f)
                            {
                                direction = -1;
                            }
                            if (player.velocity.X > 0f)
                            {
                                direction = 1;
                            }
                            if (player.whoAmI == Main.myPlayer)
                            {
                                player.ApplyDamageToNPC(nPC, (int)num, num2, direction, crit);
                                int num6 = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<HolyExplosionSupreme>(), 500, 15f, Main.myPlayer, 0f, 0f);
                                Main.projectile[num6].Kill();
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<HolyEruption>(), 380, 5f, Main.myPlayer, 0f, 0f);
                            }
                            nPC.immune[player.whoAmI] = 6;
                            player.immune = true;
                            player.immuneNoBlink = true;
                            player.immuneTime = 4;
                        }
                    }
                }
            }
            if (dashMod == 2 && player.dashDelay < 0 && player.whoAmI == Main.myPlayer)
            {
                Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && !Main.npc[i].friendly && Main.npc[i].immune[player.whoAmI] <= 0)
                    {
                        NPC nPC = Main.npc[i];
                        Rectangle rect = nPC.getRect();
                        if (rectangle.Intersects(rect) && (nPC.noTileCollide || player.CanHit(nPC)))
                        {
                            float num = 100f * player.meleeDamage;
                            float num2 = 9f;
                            bool crit = false;
                            if (player.kbGlove)
                            {
                                num2 *= 2f;
                            }
                            if (player.kbBuff)
                            {
                                num2 *= 1.5f;
                            }
                            if (Main.rand.Next(100) < player.meleeCrit)
                            {
                                crit = true;
                            }
                            int direction = player.direction;
                            if (player.velocity.X < 0f)
                            {
                                direction = -1;
                            }
                            if (player.velocity.X > 0f)
                            {
                                direction = 1;
                            }
                            if (player.whoAmI == Main.myPlayer)
                            {
                                player.ApplyDamageToNPC(nPC, (int)num, num2, direction, crit);
                                int num6 = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<HolyExplosion>(), 100, 15f, Main.myPlayer, 0f, 0f);
                                Main.projectile[num6].Kill();
                            }
                            nPC.immune[player.whoAmI] = 6;
                            player.immune = true;
                            player.immuneNoBlink = true;
                            player.immuneTime = 4;
                        }
                    }
                }
            }
            if (dashMod == 1 && player.dashDelay < 0 && player.whoAmI == Main.myPlayer)
            {
                Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && !Main.npc[i].friendly && !Main.npc[i].townNPC && Main.npc[i].immune[player.whoAmI] <= 0 && Main.npc[i].damage > 0)
                    {
                        NPC nPC = Main.npc[i];
                        Rectangle rect = nPC.getRect();
                        if (rectangle.Intersects(rect) && (nPC.noTileCollide || player.CanHit(nPC)))
                        {
                            OnDodge();
                            break;
                        }
                    }
                }
                for (int i = 0; i < 1000; i++)
                {
                    if (Main.projectile[i].active && !Main.projectile[i].friendly && Main.projectile[i].hostile && Main.projectile[i].damage > 0)
                    {
                        Projectile proj = Main.projectile[i];
                        Rectangle rect = proj.getRect();
                        if (rectangle.Intersects(rect))
                        {
                            OnDodge();
                            break;
                        }
                    }
                }
            }
            if (player.dashDelay > 0)
            {
                return;
            }
            if (player.dashDelay < 0)
            {
                float num7 = 12f;
                float num8 = 0.985f;
                float num9 = Math.Max(player.accRunSpeed, player.maxRunSpeed);
                float num10 = 0.94f;
                int num11 = 20;
                if (dashMod == 1)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        int num12;
                        if (player.velocity.Y == 0f)
                        {
                            num12 = Dust.NewDust(new Vector2(player.position.X, player.position.Y + (float)player.height - 4f), player.width, 8, 235, 0f, 0f, 100, default, 1.4f);
                        }
                        else
                        {
                            num12 = Dust.NewDust(new Vector2(player.position.X, player.position.Y + (float)(player.height / 2) - 8f), player.width, 16, 235, 0f, 0f, 100, default, 1.4f);
                        }
                        Main.dust[num12].velocity *= 0.1f;
                        Main.dust[num12].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                        Main.dust[num12].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                }
                else if (dashMod == 2)
                {
                    for (int m = 0; m < 4; m++)
                    {
                        int num14 = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 4f), player.width, player.height - 8, 246, 0f, 0f, 100, default, 2.75f);
                        Main.dust[num14].velocity *= 0.1f;
                        Main.dust[num14].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                        Main.dust[num14].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                        Main.dust[num14].noGravity = true;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num14].fadeIn = 0.5f;
                        }
                    }
                }
                else if (dashMod == 3)
                {
                    for (int m = 0; m < 12; m++)
                    {
                        int num14 = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 4f), player.width, player.height - 8, 244, 0f, 0f, 100, default, 2.75f);
                        Main.dust[num14].velocity *= 0.1f;
                        Main.dust[num14].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                        Main.dust[num14].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                        Main.dust[num14].noGravity = true;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num14].fadeIn = 0.5f;
                        }
                    }
                    num7 = 14f; //14
                }
                else if (dashMod == 4)
                {
                    for (int m = 0; m < 24; m++)
                    {
                        int num14 = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 4f), player.width, player.height - 8, 244, 0f, 0f, 100, default, 2.75f);
                        Main.dust[num14].velocity *= 0.1f;
                        Main.dust[num14].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                        Main.dust[num14].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                        Main.dust[num14].noGravity = true;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num14].fadeIn = 0.5f;
                        }
                    }
                    num7 = 16f; //14
                }
                else if (dashMod == 5)
                {
                    for (int m = 0; m < 24; m++)
                    {
                        int num14 = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 4f), player.width, player.height - 8, 33, 0f, 0f, 100, default, 2.75f);
                        Main.dust[num14].velocity *= 0.1f;
                        Main.dust[num14].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                        Main.dust[num14].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                        Main.dust[num14].noGravity = true;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num14].fadeIn = 0.5f;
                        }
                    }
                    num7 = 18f; //14
                }
                else if (dashMod == 6)
                {
                    for (int m = 0; m < 24; m++)
                    {
                        int num14 = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 4f), player.width, player.height - 8, 67, 0f, 0f, 100, default, 1f);
                        Main.dust[num14].velocity *= 0.1f;
                        Main.dust[num14].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                        Main.dust[num14].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                        Main.dust[num14].noGravity = true;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num14].fadeIn = 0.5f;
                        }
                    }
                    num7 = 12.5f; //14
                }
                if (dashMod > 0)
                {
                    player.vortexStealthActive = false;
                    if (player.velocity.X > num7 || player.velocity.X < -num7)
                    {
                        player.velocity.X = player.velocity.X * num8;
                        return;
                    }
                    if (player.velocity.X > num9 || player.velocity.X < -num9)
                    {
                        player.velocity.X = player.velocity.X * num10;
                        return;
                    }
                    player.dashDelay = num11;
                    if (player.velocity.X < 0f)
                    {
                        player.velocity.X = -num9;
                        return;
                    }
                    if (player.velocity.X > 0f)
                    {
                        player.velocity.X = num9;
                        return;
                    }
                }
            }
            else if (dashMod > 0 && !player.mount.Active)
            {
                if (dashMod == 1)
                {
                    int num16 = 0;
                    bool flag = false;
                    if (dashTimeMod > 0)
                    {
                        dashTimeMod--;
                    }
                    if (dashTimeMod < 0)
                    {
                        dashTimeMod++;
                    }
                    if (player.controlRight && player.releaseRight)
                    {
                        if (dashTimeMod > 0)
                        {
                            num16 = 1;
                            flag = true;
                            dashTimeMod = 0;
                        }
                        else
                        {
                            dashTimeMod = 15;
                        }
                    }
                    else if (player.controlLeft && player.releaseLeft)
                    {
                        if (dashTimeMod < 0)
                        {
                            num16 = -1;
                            flag = true;
                            dashTimeMod = 0;
                        }
                        else
                        {
                            dashTimeMod = -15;
                        }
                    }
                    if (flag)
                    {
                        player.velocity.X = 14.5f * (float)num16; //eoc dash amount
                        Point point = (player.Center + new Vector2((float)(num16 * player.width / 2 + 2), player.gravDir * (float)-(float)player.height / 2f + player.gravDir * 2f)).ToTileCoordinates();
                        Point point2 = (player.Center + new Vector2((float)(num16 * player.width / 2 + 2), 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point.X, point.Y) || WorldGen.SolidOrSlopedTile(point2.X, point2.Y))
                        {
                            player.velocity.X = player.velocity.X / 2f;
                        }
                        player.dashDelay = -1;
                        for (int num17 = 0; num17 < 20; num17++)
                        {
                            int num18 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 235, 0f, 0f, 100, default, 2f);
                            Dust expr_CDB_cp_0 = Main.dust[num18];
                            expr_CDB_cp_0.position.X += (float)Main.rand.Next(-5, 6);
                            Dust expr_D02_cp_0 = Main.dust[num18];
                            expr_D02_cp_0.position.Y += (float)Main.rand.Next(-5, 6);
                            Main.dust[num18].velocity *= 0.2f;
                            Main.dust[num18].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                            Main.dust[num18].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                        }
                        return;
                    }
                }
                else if (dashMod == 2)
                {
                    int num23 = 0;
                    bool flag3 = false;
                    if (dashTimeMod > 0)
                    {
                        dashTimeMod--;
                    }
                    if (dashTimeMod < 0)
                    {
                        dashTimeMod++;
                    }
                    if (player.controlRight && player.releaseRight)
                    {
                        if (dashTimeMod > 0)
                        {
                            num23 = 1;
                            flag3 = true;
                            dashTimeMod = 0;
                        }
                        else
                        {
                            dashTimeMod = 15;
                        }
                    }
                    else if (player.controlLeft && player.releaseLeft)
                    {
                        if (dashTimeMod < 0)
                        {
                            num23 = -1;
                            flag3 = true;
                            dashTimeMod = 0;
                        }
                        else
                        {
                            dashTimeMod = -15;
                        }
                    }
                    if (flag3)
                    {
                        player.velocity.X = 16.9f * (float)num23; //tabi dash amount
                        Point point5 = (player.Center + new Vector2((float)(num23 * player.width / 2 + 2), player.gravDir * (float)-(float)player.height / 2f + player.gravDir * 2f)).ToTileCoordinates();
                        Point point6 = (player.Center + new Vector2((float)(num23 * player.width / 2 + 2), 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point5.X, point5.Y) || WorldGen.SolidOrSlopedTile(point6.X, point6.Y))
                        {
                            player.velocity.X = player.velocity.X / 2f;
                        }
                        player.dashDelay = -1;
                        for (int num24 = 0; num24 < 20; num24++)
                        {
                            int num25 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 246, 0f, 0f, 100, default, 3f);
                            Dust expr_13AF_cp_0 = Main.dust[num25];
                            expr_13AF_cp_0.position.X += (float)Main.rand.Next(-5, 6);
                            Dust expr_13D6_cp_0 = Main.dust[num25];
                            expr_13D6_cp_0.position.Y += (float)Main.rand.Next(-5, 6);
                            Main.dust[num25].velocity *= 0.2f;
                            Main.dust[num25].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                            Main.dust[num25].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                            Main.dust[num25].noGravity = true;
                            Main.dust[num25].fadeIn = 0.5f;
                        }
                    }
                }
                else if (dashMod == 3)
                {
                    int num23 = 0;
                    bool flag3 = false;
                    if (dashTimeMod > 0)
                    {
                        dashTimeMod--;
                    }
                    if (dashTimeMod < 0)
                    {
                        dashTimeMod++;
                    }
                    if (player.controlRight && player.releaseRight)
                    {
                        if (dashTimeMod > 0)
                        {
                            num23 = 1;
                            flag3 = true;
                            dashTimeMod = 0;
                        }
                        else
                        {
                            dashTimeMod = 15;
                        }
                    }
                    else if (player.controlLeft && player.releaseLeft)
                    {
                        if (dashTimeMod < 0)
                        {
                            num23 = -1;
                            flag3 = true;
                            dashTimeMod = 0;
                        }
                        else
                        {
                            dashTimeMod = -15;
                        }
                    }
                    if (flag3)
                    {
                        player.velocity.X = 21.9f * (float)num23; //solar dash amount
                        Point point5 = (player.Center + new Vector2((float)(num23 * player.width / 2 + 2), player.gravDir * (float)-(float)player.height / 2f + player.gravDir * 2f)).ToTileCoordinates();
                        Point point6 = (player.Center + new Vector2((float)(num23 * player.width / 2 + 2), 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point5.X, point5.Y) || WorldGen.SolidOrSlopedTile(point6.X, point6.Y))
                        {
                            player.velocity.X = player.velocity.X / 2f;
                        }
                        player.dashDelay = -1;
                        for (int num24 = 0; num24 < 40; num24++)
                        {
                            int num25 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 244, 0f, 0f, 100, default, 3f);
                            Dust expr_13AF_cp_0 = Main.dust[num25];
                            expr_13AF_cp_0.position.X += (float)Main.rand.Next(-5, 6);
                            Dust expr_13D6_cp_0 = Main.dust[num25];
                            expr_13D6_cp_0.position.Y += (float)Main.rand.Next(-5, 6);
                            Main.dust[num25].velocity *= 0.2f;
                            Main.dust[num25].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                            Main.dust[num25].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                            Main.dust[num25].noGravity = true;
                            Main.dust[num25].fadeIn = 0.5f;
                        }
                    }
                }
                else if (dashMod == 4)
                {
                    int num23 = 0;
                    bool flag3 = false;
                    if (dashTimeMod > 0)
                    {
                        dashTimeMod--;
                    }
                    if (dashTimeMod < 0)
                    {
                        dashTimeMod++;
                    }
                    if (player.controlRight && player.releaseRight)
                    {
                        if (dashTimeMod > 0)
                        {
                            num23 = 1;
                            flag3 = true;
                            dashTimeMod = 0;
                        }
                        else
                        {
                            dashTimeMod = 15;
                        }
                    }
                    else if (player.controlLeft && player.releaseLeft)
                    {
                        if (dashTimeMod < 0)
                        {
                            num23 = -1;
                            flag3 = true;
                            dashTimeMod = 0;
                        }
                        else
                        {
                            dashTimeMod = -15;
                        }
                    }
                    if (flag3)
                    {
                        player.velocity.X = 23.9f * (float)num23; //slighty more powerful solar dash
                        Point point5 = (player.Center + new Vector2((float)(num23 * player.width / 2 + 2), player.gravDir * (float)-(float)player.height / 2f + player.gravDir * 2f)).ToTileCoordinates();
                        Point point6 = (player.Center + new Vector2((float)(num23 * player.width / 2 + 2), 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point5.X, point5.Y) || WorldGen.SolidOrSlopedTile(point6.X, point6.Y))
                        {
                            player.velocity.X = player.velocity.X / 2f;
                        }
                        player.dashDelay = -1;
                        for (int num24 = 0; num24 < 60; num24++)
                        {
                            int num25 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 244, 0f, 0f, 100, default, 3f);
                            Dust expr_13AF_cp_0 = Main.dust[num25];
                            expr_13AF_cp_0.position.X += (float)Main.rand.Next(-5, 6);
                            Dust expr_13D6_cp_0 = Main.dust[num25];
                            expr_13D6_cp_0.position.Y += (float)Main.rand.Next(-5, 6);
                            Main.dust[num25].velocity *= 0.2f;
                            Main.dust[num25].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                            Main.dust[num25].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                            Main.dust[num25].noGravity = true;
                            Main.dust[num25].fadeIn = 0.5f;
                        }
                    }
                }
                else if (dashMod == 5)
                {
                    int num23 = 0;
                    bool flag3 = false;
                    if (dashTimeMod > 0)
                    {
                        dashTimeMod--;
                    }
                    if (dashTimeMod < 0)
                    {
                        dashTimeMod++;
                    }
                    if (player.controlRight && player.releaseRight)
                    {
                        if (dashTimeMod > 0)
                        {
                            num23 = 1;
                            flag3 = true;
                            dashTimeMod = 0;
                        }
                        else
                        {
                            dashTimeMod = 15;
                        }
                    }
                    else if (player.controlLeft && player.releaseLeft)
                    {
                        if (dashTimeMod < 0)
                        {
                            num23 = -1;
                            flag3 = true;
                            dashTimeMod = 0;
                        }
                        else
                        {
                            dashTimeMod = -15;
                        }
                    }
                    if (flag3)
                    {
                        player.velocity.X = 25.9f * (float)num23;
                        Point point5 = (player.Center + new Vector2((float)(num23 * player.width / 2 + 2), player.gravDir * (float)-(float)player.height / 2f + player.gravDir * 2f)).ToTileCoordinates();
                        Point point6 = (player.Center + new Vector2((float)(num23 * player.width / 2 + 2), 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point5.X, point5.Y) || WorldGen.SolidOrSlopedTile(point6.X, point6.Y))
                        {
                            player.velocity.X = player.velocity.X / 2f;
                        }
                        player.dashDelay = -1;
                        for (int num24 = 0; num24 < 60; num24++)
                        {
                            int num25 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 33, 0f, 0f, 100, default, 3f);
                            Dust expr_13AF_cp_0 = Main.dust[num25];
                            expr_13AF_cp_0.position.X += (float)Main.rand.Next(-5, 6);
                            Dust expr_13D6_cp_0 = Main.dust[num25];
                            expr_13D6_cp_0.position.Y += (float)Main.rand.Next(-5, 6);
                            Main.dust[num25].velocity *= 0.2f;
                            Main.dust[num25].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                            Main.dust[num25].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                            Main.dust[num25].noGravity = true;
                            Main.dust[num25].fadeIn = 0.5f;
                        }
                    }
                }
                else if (dashMod == 6)
                {
                    int num23 = 0;
                    bool flag3 = false;
                    if (dashTimeMod > 0)
                    {
                        dashTimeMod--;
                    }
                    if (dashTimeMod < 0)
                    {
                        dashTimeMod++;
                    }
                    if (player.controlRight && player.releaseRight)
                    {
                        if (dashTimeMod > 0)
                        {
                            num23 = 1;
                            flag3 = true;
                            dashTimeMod = 0;
                        }
                        else
                        {
                            dashTimeMod = 15;
                        }
                    }
                    else if (player.controlLeft && player.releaseLeft)
                    {
                        if (dashTimeMod < 0)
                        {
                            num23 = -1;
                            flag3 = true;
                            dashTimeMod = 0;
                        }
                        else
                        {
                            dashTimeMod = -15;
                        }
                    }
                    if (flag3)
                    {
                        player.velocity.X = 15.7f * (float)num23;
                        Point point5 = (player.Center + new Vector2((float)(num23 * player.width / 2 + 2), player.gravDir * (float)-(float)player.height / 2f + player.gravDir * 2f)).ToTileCoordinates();
                        Point point6 = (player.Center + new Vector2((float)(num23 * player.width / 2 + 2), 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point5.X, point5.Y) || WorldGen.SolidOrSlopedTile(point6.X, point6.Y))
                        {
                            player.velocity.X = player.velocity.X / 2f;
                        }
                        player.dashDelay = -1;
                        for (int num24 = 0; num24 < 60; num24++)
                        {
                            int num25 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 67, 0f, 0f, 100, default, 1.25f);
                            Dust expr_13AF_cp_0 = Main.dust[num25];
                            expr_13AF_cp_0.position.X += (float)Main.rand.Next(-5, 6);
                            Dust expr_13D6_cp_0 = Main.dust[num25];
                            expr_13D6_cp_0.position.Y += (float)Main.rand.Next(-5, 6);
                            Main.dust[num25].velocity *= 0.2f;
                            Main.dust[num25].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                            Main.dust[num25].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                            Main.dust[num25].noGravity = true;
                            Main.dust[num25].fadeIn = 0.5f;
                        }
                    }
                }
            }
        }

        private void OnDodge()
        {
            if (player.whoAmI == Main.myPlayer && dodgeScarf && !scarfCooldown)
            {
                player.AddBuff(ModContent.BuffType<ScarfMeleeBoost>(), 540);
                player.AddBuff(ModContent.BuffType<ScarfCooldown>(), player.chaosState ? 1800 : 900);
                player.immune = true;
                player.immuneTime = 60;
                if (player.longInvince)
                {
                    player.immuneTime += 40;
                }
                for (int k = 0; k < player.hurtCooldowns.Length; k++)
                {
                    player.hurtCooldowns[k] = player.immuneTime;
                }
                for (int j = 0; j < 100; j++)
                {
                    int num = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 235, 0f, 0f, 100, default, 2f);
                    Dust expr_A4_cp_0 = Main.dust[num];
                    expr_A4_cp_0.position.X += (float)Main.rand.Next(-20, 21);
                    Dust expr_CB_cp_0 = Main.dust[num];
                    expr_CB_cp_0.position.Y += (float)Main.rand.Next(-20, 21);
                    Main.dust[num].velocity *= 0.4f;
                    Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                    Main.dust[num].shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                        Main.dust[num].noGravity = true;
                    }
                }
                if (player.whoAmI == Main.myPlayer)
                {
                    NetMessage.SendData(62, -1, -1, null, player.whoAmI, 1f, 0f, 0f, 0, 0, 0);
                }
            }
        }

        public void ModHorizontalMovement()
        {
            float num = (player.accRunSpeed + player.maxRunSpeed) / 2f;
            if (player.controlLeft && player.velocity.X > -player.accRunSpeed && player.dashDelay >= 0)
            {
                if (player.velocity.X < -num && player.velocity.Y == 0f && !player.mount.Active)
                {
                    int num3 = 0;
                    if (player.gravDir == -1f)
                    {
                        num3 -= player.height;
                    }
                    if (dashMod == 1)
                    {
                        int num7 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num3), player.width + 8, 4, 235, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 1.5f);
                        Main.dust[num7].velocity.X = Main.dust[num7].velocity.X * 0.2f;
                        Main.dust[num7].velocity.Y = Main.dust[num7].velocity.Y * 0.2f;
                        Main.dust[num7].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    else if (dashMod == 2)
                    {
                        int num7 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num3), player.width + 8, 4, 246, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 2.5f);
                        Main.dust[num7].velocity.X = Main.dust[num7].velocity.X * 0.2f;
                        Main.dust[num7].velocity.Y = Main.dust[num7].velocity.Y * 0.2f;
                        Main.dust[num7].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    else if (dashMod == 3)
                    {
                        int num7 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num3), player.width + 8, 4, 244, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 3f);
                        Main.dust[num7].velocity.X = Main.dust[num7].velocity.X * 0.2f;
                        Main.dust[num7].velocity.Y = Main.dust[num7].velocity.Y * 0.2f;
                        Main.dust[num7].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    else if (dashMod == 4)
                    {
                        int num7 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num3), player.width + 8, 4, 244, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 3f);
                        Main.dust[num7].velocity.X = Main.dust[num7].velocity.X * 0.2f;
                        Main.dust[num7].velocity.Y = Main.dust[num7].velocity.Y * 0.2f;
                        Main.dust[num7].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    else if (dashMod == 5)
                    {
                        int num7 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num3), player.width + 8, 4, 33, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 3f);
                        Main.dust[num7].velocity.X = Main.dust[num7].velocity.X * 0.2f;
                        Main.dust[num7].velocity.Y = Main.dust[num7].velocity.Y * 0.2f;
                        Main.dust[num7].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    else if (dashMod == 6)
                    {
                        int num7 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num3), player.width + 8, 4, 67, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 1.25f);
                        Main.dust[num7].velocity.X = Main.dust[num7].velocity.X * 0.2f;
                        Main.dust[num7].velocity.Y = Main.dust[num7].velocity.Y * 0.2f;
                        Main.dust[num7].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                }
            }
            else if (player.controlRight && player.velocity.X < player.accRunSpeed && player.dashDelay >= 0)
            {
                if (player.velocity.X > num && player.velocity.Y == 0f && !player.mount.Active)
                {
                    int num8 = 0;
                    if (player.gravDir == -1f)
                    {
                        num8 -= player.height;
                    }
                    if (dashMod == 1)
                    {
                        int num12 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num8), player.width + 8, 4, 235, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 1.5f);
                        Main.dust[num12].velocity.X = Main.dust[num12].velocity.X * 0.2f;
                        Main.dust[num12].velocity.Y = Main.dust[num12].velocity.Y * 0.2f;
                        Main.dust[num12].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    else if (dashMod == 2)
                    {
                        int num12 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num8), player.width + 8, 4, 246, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 2.5f);
                        Main.dust[num12].velocity.X = Main.dust[num12].velocity.X * 0.2f;
                        Main.dust[num12].velocity.Y = Main.dust[num12].velocity.Y * 0.2f;
                        Main.dust[num12].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    else if (dashMod == 3)
                    {
                        int num12 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num8), player.width + 8, 4, 244, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 3f);
                        Main.dust[num12].velocity.X = Main.dust[num12].velocity.X * 0.2f;
                        Main.dust[num12].velocity.Y = Main.dust[num12].velocity.Y * 0.2f;
                        Main.dust[num12].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    else if (dashMod == 4)
                    {
                        int num12 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num8), player.width + 8, 4, 244, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 3f);
                        Main.dust[num12].velocity.X = Main.dust[num12].velocity.X * 0.2f;
                        Main.dust[num12].velocity.Y = Main.dust[num12].velocity.Y * 0.2f;
                        Main.dust[num12].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    else if (dashMod == 5)
                    {
                        int num12 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num8), player.width + 8, 4, 33, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 3f);
                        Main.dust[num12].velocity.X = Main.dust[num12].velocity.X * 0.2f;
                        Main.dust[num12].velocity.Y = Main.dust[num12].velocity.Y * 0.2f;
                        Main.dust[num12].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    else if (dashMod == 6)
                    {
                        int num12 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num8), player.width + 8, 4, 67, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 1.25f);
                        Main.dust[num12].velocity.X = Main.dust[num12].velocity.X * 0.2f;
                        Main.dust[num12].velocity.Y = Main.dust[num12].velocity.Y * 0.2f;
                        Main.dust[num12].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                }
            }

            if (player.mount.Active && player.mount.Type == ModContent.MountType<AlicornMount>() && Math.Abs(player.velocity.X) > player.mount.DashSpeed - player.mount.RunSpeed / 2f)
            {
                Rectangle rect = player.getRect();

                if (player.direction == 1)
                    rect.Offset(player.width - 1, 0);

                rect.Width = 2;
                rect.Inflate(6, 12);
                float damage = 800f * player.minionDamage;
                float knockback = 10f;
                int nPCImmuneTime = 30;
                int playerImmuneTime = 6;
                ModCollideWithNPCs(rect, damage, knockback, nPCImmuneTime, playerImmuneTime);
            }

            if (player.mount.Active && player.mount.Type == ModContent.MountType<AngryDogMount>() && Math.Abs(player.velocity.X) > player.mount.RunSpeed / 2f)
            {
                Rectangle rect2 = player.getRect();

                if (player.direction == 1)
                    rect2.Offset(player.width - 1, 0);

                rect2.Width = 2;
                rect2.Inflate(6, 12);
                float damage2 = 50f * player.minionDamage;
                float knockback2 = 8f;
                int nPCImmuneTime2 = 30;
                int playerImmuneTime2 = 6;
                ModCollideWithNPCs(rect2, damage2, knockback2, nPCImmuneTime2, playerImmuneTime2);
            }

            if (player.mount.Active && player.mount.Type == ModContent.MountType<OnyxExcavator>() && Math.Abs(player.velocity.X) > player.mount.RunSpeed / 2f)
            {
                Rectangle rect2 = player.getRect();

                if (player.direction == 1)
                    rect2.Offset(player.width - 1, 0);

                rect2.Width = 2;
                rect2.Inflate(6, 12);
                float damage2 = 25f * player.minionDamage;
                float knockback2 = 5f;
                int nPCImmuneTime2 = 30;
                int playerImmuneTime2 = 6;
                ModCollideWithNPCs(rect2, damage2, knockback2, nPCImmuneTime2, playerImmuneTime2);
            }
        }

        private int ModCollideWithNPCs(Rectangle myRect, float Damage, float Knockback, int NPCImmuneTime, int PlayerImmuneTime)
        {
            int num = 0;
            for (int i = 0; i < 200; i++)
            {
                NPC nPC = Main.npc[i];
                if (nPC.active && !nPC.dontTakeDamage && !nPC.friendly && nPC.immune[player.whoAmI] == 0)
                {
                    Rectangle rect = nPC.getRect();
                    if (myRect.Intersects(rect) && (nPC.noTileCollide || Collision.CanHit(player.position, player.width, player.height, nPC.position, nPC.width, nPC.height)))
                    {
                        int direction = player.direction;
                        if (player.velocity.X < 0f)
                        {
                            direction = -1;
                        }
                        if (player.velocity.X > 0f)
                        {
                            direction = 1;
                        }
                        if (player.whoAmI == Main.myPlayer)
                        {
                            player.ApplyDamageToNPC(nPC, (int)Damage, Knockback, direction, false);
                        }
                        nPC.immune[player.whoAmI] = NPCImmuneTime;
                        player.immune = true;
                        player.immuneNoBlink = true;
                        player.immuneTime = PlayerImmuneTime;
                        num++;
                        break;
                    }
                }
            }
            return num;
        }
        #endregion

        #region Drawing
        public static readonly PlayerLayer MiscEffectsBack = new PlayerLayer("CalamityMod", "MiscEffectsBack", PlayerLayer.MiscEffectsBack, delegate (PlayerDrawInfo drawInfo)
        {
            if (drawInfo.shadow != 0f)
            {
                return;
            }
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            if (modPlayer.sirenIce)
            {
                Texture2D texture = ModContent.GetTexture("CalamityMod/ExtraTextures/IceShield");
                int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X);
                int drawY = (int)(drawInfo.position.Y + drawPlayer.height / 2f - Main.screenPosition.Y); //4
                DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Color.Cyan, 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, SpriteEffects.None, 0);
                Main.playerDrawData.Add(data);
            }
            if (modPlayer.amidiasBlessing)
            {
                Texture2D texture = ModContent.GetTexture("CalamityMod/ExtraTextures/AmidiasBubble");
                int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X);
                int drawY = (int)(drawInfo.position.Y + drawPlayer.height / 2f - Main.screenPosition.Y); //4
                DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Color.White, 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, SpriteEffects.None, 0);
                Main.playerDrawData.Add(data);
            }
        });

        public static readonly PlayerLayer MiscEffects = new PlayerLayer("CalamityMod", "MiscEffects", PlayerLayer.MiscEffectsFront, delegate (PlayerDrawInfo drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return;

            Player drawPlayer = drawInfo.drawPlayer;
            Item item = drawPlayer.inventory[drawPlayer.selectedItem];

            if (!drawPlayer.frozen &&
                ((drawPlayer.itemAnimation > 0 && item.useStyle != 0) || (item.holdStyle > 0 && !drawPlayer.pulley)) &&
                item.type > 0 &&
                !drawPlayer.dead &&
                !item.noUseGraphic &&
                (!drawPlayer.wet || !item.noWet))
            {
                Vector2 vector = drawPlayer.position + (drawPlayer.itemLocation - drawPlayer.position);
                SpriteEffects effect = SpriteEffects.FlipHorizontally;

                if (drawPlayer.gravDir == 1f)
                {
                    if (drawPlayer.direction == 1)
                        effect = SpriteEffects.None;
                    else
                        effect = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    if (drawPlayer.direction == 1)
                        effect = SpriteEffects.FlipVertically;
                    else
                        effect = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
                }

				// Staff
                if (item.type == ModContent.ItemType<DeathhailStaff>() || item.type == ModContent.ItemType<Vesuvius>() || item.type == ModContent.ItemType<SoulPiercer>() ||
				item.type == ModContent.ItemType<FatesReveal>())
                {
                    Texture2D texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Magic/DeathhailStaffGlow");
					if (item.type == ModContent.ItemType<Vesuvius>())
						texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Magic/VesuviusGlow");
					else if (item.type == ModContent.ItemType<SoulPiercer>())
						texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Magic/SoulPiercerGlow");
					else if (item.type == ModContent.ItemType<FatesReveal>())
						texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Magic/FatesRevealGlow");

                    float num104 = drawPlayer.itemRotation + 0.785f * (float)drawPlayer.direction;
                    int num105 = 0;
                    Vector2 zero3 = new Vector2(0f, (float)Main.itemTexture[item.type].Height);

                    if (drawPlayer.gravDir == -1f)
                    {
                        if (drawPlayer.direction == -1)
                        {
                            num104 += 1.57f;
                            zero3 = new Vector2((float)Main.itemTexture[item.type].Width, 0f);
                            num105 -= Main.itemTexture[item.type].Width;
                        }
                        else
                        {
                            num104 -= 1.57f;
                            zero3 = Vector2.Zero;
                        }
                    }
                    else if (drawPlayer.direction == -1)
                    {
                        zero3 = new Vector2((float)Main.itemTexture[item.type].Width, (float)Main.itemTexture[item.type].Height);
                        num105 -= Main.itemTexture[item.type].Width;
                    }

                    DrawData data = new DrawData(texture,
                        new Vector2((float)(int)(vector.X - Main.screenPosition.X + zero3.X + (float)num105), (float)(int)(vector.Y - Main.screenPosition.Y + 0f)),
                        new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, Main.itemTexture[item.type].Width, Main.itemTexture[item.type].Height)),
                        Color.White,
                        num104,
                        zero3,
                        item.scale,
                        effect,
                        0);

                    Main.playerDrawData.Add(data);
                }

				// Bow and Book
                else if (item.type == ModContent.ItemType<Deathwind>() || item.type == ModContent.ItemType<Apotheosis>() || item.type == ModContent.ItemType<CleansingBlaze>() ||
                item.type == ModContent.ItemType<SubsumingVortex>())
                {
                    Texture2D texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Ranged/DeathwindGlow");
                    int offsetX = 10;
                    if (item.type == ModContent.ItemType<Apotheosis>())
                    {
                        texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Magic/ApotheosisGlow");
                        offsetX = 6;
                    }
                    else if (item.type == ModContent.ItemType<CleansingBlaze>())
                    {
                        texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Ranged/CleansingBlazeGlow");
                        offsetX = 37;
                    }
                    else if (item.type == ModContent.ItemType<SubsumingVortex>())
                    {
                        texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Magic/SubsumingVortexGlow");
                        offsetX = 9;
                    }

					Vector2 vector13 = new Vector2((float)(Main.itemTexture[item.type].Width / 2), (float)(Main.itemTexture[item.type].Height / 2));
                    Vector2 vector14 = vector13;
                    int num107 = (int)vector14.X - offsetX;
                    vector13.Y = vector14.Y;

                    Vector2 origin4 = new Vector2(-(float)num107, (float)(Main.itemTexture[item.type].Height / 2));
                    if (drawPlayer.direction == -1)
                        origin4 = new Vector2((float)(Main.itemTexture[item.type].Width + num107), (float)(Main.itemTexture[item.type].Height / 2));

                    DrawData data = new DrawData(texture,
                        new Vector2((float)(int)(vector.X - Main.screenPosition.X + vector13.X), (float)(int)(vector.Y - Main.screenPosition.Y + vector13.Y)),
                        new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, Main.itemTexture[item.type].Width, Main.itemTexture[item.type].Height)),
                        Color.White,
                        drawPlayer.itemRotation,
                        origin4,
                        item.scale,
                        effect,
                        0);

                    Main.playerDrawData.Add(data);
                }

				// Sword
                else if (item.type == ModContent.ItemType<Excelsus>() || item.type == ModContent.ItemType<EssenceFlayer>() || item.type == ModContent.ItemType<TheEnforcer>() ||
                item.type == ModContent.ItemType<ElementalExcalibur>() || item.type == ModContent.ItemType<TerrorBlade>() || item.type == ModContent.ItemType<EtherealSubjugator>())
                {
                    Texture2D texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Melee/ExcelsusGlow");
                    if (item.type == ModContent.ItemType<EssenceFlayer>())
                        texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Melee/EssenceFlayerGlow");
                    else if (item.type == ModContent.ItemType<TheEnforcer>())
                        texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Melee/TheEnforcerGlow");
                    else if (item.type == ModContent.ItemType<ElementalExcalibur>())
                        texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Melee/ElementalExcaliburGlow");
					else if (item.type == ModContent.ItemType<TerrorBlade>())
						texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Melee/TerrorBladeGlow");
					else if (item.type == ModContent.ItemType<EtherealSubjugator>())
						texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Summon/EtherealSubjugatorGlow");

					float yOffset = drawPlayer.gravDir == -1f ? 0f : (float)Main.itemTexture[item.type].Height;

                    DrawData data = new DrawData(texture,
                        new Vector2((float)(int)(vector.X - Main.screenPosition.X), (float)(int)(vector.Y - Main.screenPosition.Y)),
                        new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, Main.itemTexture[item.type].Width, Main.itemTexture[item.type].Height)),
                        Color.White,
                        drawPlayer.itemRotation,
                        new Vector2((float)Main.itemTexture[item.type].Width * 0.5f - (float)Main.itemTexture[item.type].Width * 0.5f * (float)drawPlayer.direction, yOffset) + Vector2.Zero,
                        item.scale,
                        effect,
                        0);

                    Main.playerDrawData.Add(data);
                }
            }
        });

        public override void ModifyDrawLayers(List<PlayerLayer> list)
        {
            MiscEffectsBack.visible = true;
            list.Insert(0, MiscEffectsBack);
            MiscEffects.visible = true;
            list.Add(MiscEffects);
            if (fab || crysthamyr || onyxExcavator)
            { AddPlayerLayer(list, clAfterAll, list[list.Count - 1], false); }
        }

        public PlayerLayer clAfterAll = new PlayerLayer("Calamity", "clAfterAll", PlayerLayer.MiscEffectsFront, delegate (PlayerDrawInfo edi)
        {
            Player drawPlayer = edi.drawPlayer;
            if (drawPlayer.mount != null && (drawPlayer.Calamity().fab || drawPlayer.Calamity().crysthamyr ||
                drawPlayer.Calamity().onyxExcavator))
            {
                drawPlayer.mount.Draw(Main.playerDrawData, 3, drawPlayer, edi.position, edi.mountColor, edi.spriteEffects, edi.shadow);
            }
        });

        public static void AddPlayerLayer(List<PlayerLayer> list, PlayerLayer layer, PlayerLayer parent, bool first)
        {
            int insertAt = -1;
            for (int m = 0; m < list.Count; m++)
            {
                PlayerLayer dl = list[m];
                if (dl.Name.Equals(parent.Name))
                { insertAt = m; break; }
            }
            if (insertAt == -1)
                list.Add(layer);
            else
                list.Insert(first ? insertAt : insertAt + 1, layer);
        }

        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            bool noRogueStealth = rogueStealth == 0f || player.townNPCs > 2f;
            if (rogueStealth > 0f && rogueStealthMax > 0f && player.townNPCs < 3f)
            {
                //A translucent orchid color, the rogue class color
                float colorValue = rogueStealth / rogueStealthMax * 0.9f; //0 to 0.9
                r *= 1f - (colorValue * 0.89f); //255 to 50
                g *= 1f - colorValue; //255 to 25
                b *= 1f - (colorValue * 0.89f); //255 to 50
                a *= 1f - colorValue; //255 to 25
                player.armorEffectDrawOutlines = false;
                player.armorEffectDrawShadow = false;
                player.armorEffectDrawShadowSubtle = false;
            }

            if (CalamityWorld.ironHeart && !Main.gameMenu)
            {
                Texture2D ironHeart = ModContent.GetTexture("CalamityMod/ExtraTextures/IronHeart");
                Main.heartTexture = Main.heart2Texture = ironHeart;
            }
            else
            {
                Texture2D heart3 = ModContent.GetTexture("CalamityMod/ExtraTextures/Heart3");
                Texture2D heart4 = ModContent.GetTexture("CalamityMod/ExtraTextures/Heart4");
                Texture2D heart5 = ModContent.GetTexture("CalamityMod/ExtraTextures/Heart5");
                Texture2D heart6 = ModContent.GetTexture("CalamityMod/ExtraTextures/Heart6");
				Texture2D heartOriginal = ModContent.GetTexture("CalamityMod/ExtraTextures/HeartOriginal"); //Life fruit
				Texture2D heartOriginal2 = ModContent.GetTexture("CalamityMod/ExtraTextures/HeartOriginal2"); //Life crystal

				int totalFruit =
                    (mFruit ? 1 : 0) +
                    (bOrange ? 1 : 0) +
                    (eBerry ? 1 : 0) +
                    (dFruit ? 1 : 0);

                switch (totalFruit)
                {
                    default:
						Main.heart2Texture = heartOriginal;
                        break;
                    case 4:
                        Main.heart2Texture = heart6;
                        break;
                    case 3:
                        Main.heart2Texture = heart5;
                        break;
                    case 2:
                        Main.heart2Texture = heart4;
                        break;
                    case 1:
                        Main.heart2Texture = heart3;
                        break;
                }

                Main.heartTexture = heartOriginal2;
            }
            if (revivify)
            {
                if (Main.rand.NextBool(2) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 91, player.velocity.X * 0.2f, player.velocity.Y * 0.2f, 100, default, 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
            }
            if (tRegen)
            {
                if (Main.rand.NextBool(10) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 107, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.75f;
                    Main.dust[dust].velocity.Y -= 0.35f;
                    Main.playerDrawDust.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.025f;
                    g *= 0.15f;
                    b *= 0.035f;
                    fullBright = true;
                }
            }
            if (IBoots)
            {
                if (((double)Math.Abs(player.velocity.X) > 0.05 || (double)Math.Abs(player.velocity.Y) > 0.05) && !player.mount.Active)
                {
                    if (Main.rand.NextBool(2) && drawInfo.shadow == 0f)
                    {
                        int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 229, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 1f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0.5f;
                        Main.playerDrawDust.Add(dust);
                    }
                    if (noRogueStealth)
                    {
                        r *= 0.05f;
                        g *= 0.05f;
                        b *= 0.05f;
                        fullBright = true;
                    }
                }
            }
            if (elysianFire)
            {
                if (((double)Math.Abs(player.velocity.X) > 0.05 || (double)Math.Abs(player.velocity.Y) > 0.05) && !player.mount.Active)
                {
                    if (Main.rand.NextBool(2) && drawInfo.shadow == 0f)
                    {
                        int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 246, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 1f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0.5f;
                        Main.playerDrawDust.Add(dust);
                    }
                    if (noRogueStealth)
                    {
                        r *= 0.75f;
                        g *= 0.55f;
                        b *= 0f;
                        fullBright = true;
                    }
                }
            }
            if (dsSetBonus)
            {
                if (((double)Math.Abs(player.velocity.X) > 0.05 || (double)Math.Abs(player.velocity.Y) > 0.05) && !player.mount.Active)
                {
                    if (Main.rand.NextBool(2) && drawInfo.shadow == 0f)
                    {
                        int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 27, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 1.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0.5f;
                        Main.playerDrawDust.Add(dust);
                    }
                    if (noRogueStealth)
                    {
                        r *= 0.15f;
                        g *= 0.025f;
                        b *= 0.1f;
                        fullBright = true;
                    }
                }
            }
            if (auricSet)
            {
                if (((double)Math.Abs(player.velocity.X) > 0.05 || (double)Math.Abs(player.velocity.Y) > 0.05) && !player.mount.Active)
                {
                    if (drawInfo.shadow == 0f)
                    {
                        int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, Main.rand.NextBool(2) ? 57 : 244, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 1.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0.5f;
                        Main.playerDrawDust.Add(dust);
                    }
                    if (noRogueStealth)
                    {
                        r *= 0.15f;
                        g *= 0.025f;
                        b *= 0.1f;
                        fullBright = true;
                    }
                }
            }
            if (bFlames || aFlames || rageMode)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, ModContent.DustType<BrimstoneFlame>(), player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.25f;
                    g *= 0.01f;
                    b *= 0.01f;
                    fullBright = true;
                }
            }
            if (shadowflame)
            {
                if (Main.rand.Next(5) < 4 && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 27, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 1.95f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.75f;
                    Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.75f;
                    Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
            }
            if (sulphurPoison)
            {
                if (Main.rand.Next(5) < 4 && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 46, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 1.95f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.75f;
                    Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.75f;
                    Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
                if (noRogueStealth)
                {
                    r *= 0.65f;
                    b *= 0.75f;
                    fullBright = true;
                }
            }
            if (adrenalineMode)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 206, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.01f;
                    g *= 0.15f;
                    b *= 0.1f;
                    fullBright = true;
                }
            }
            if (gsInferno)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 173, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.25f;
                    g *= 0.01f;
                    b *= 0.01f;
                    fullBright = true;
                }
            }
            if (astralInfection)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dustType = Main.rand.NextBool(2) ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>();
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, dustType, player.velocity.X * 0.2f, player.velocity.Y * 0.2f, 100, default, 0.7f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.dust[dust].color = new Color(255, 255, 255, 0);
                    Main.playerDrawDust.Add(dust);
                }
            }
            if (hFlames || hInferno)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, ModContent.DustType<HolyFireDust>(), player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.25f;
                    g *= 0.25f;
                    b *= 0.1f;
                    fullBright = true;
                }
            }
            else if (eGravity)
            {
                if (Main.rand.NextBool(12) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 244, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
            }
            if (pFlames)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 89, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity.Y -= 0.15f;
                    Main.playerDrawDust.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.07f;
                    g *= 0.15f;
                    b *= 0.01f;
                    fullBright = true;
                }
            }
            if (gState || cDepth)
            {
                if (noRogueStealth)
                {
                    r *= 0f;
                    g *= 0.05f;
                    b *= 0.3f;
                    fullBright = true;
                }
            }
            if (draedonsHeart && !shadeRegen && !cFreeze && (double)Math.Abs(player.velocity.X) < 0.05 && (double)Math.Abs(player.velocity.Y) < 0.05 && player.itemAnimation == 0)
            {
                if (noRogueStealth)
                {
                    r *= 0f;
                    g *= 0.5f;
                    b *= 0f;
                    fullBright = true;
                }
            }
            if (bBlood)
            {
                if (Main.rand.NextBool(6) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 5, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.15f;
                    g *= 0.01f;
                    b *= 0.01f;
                    fullBright = true;
                }
            }
            if (mushy || (etherealExtorter && player.ZoneGlowshroom))
            {
                if (Main.rand.NextBool(6) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 56, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 2f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.5f;
                    Main.dust[dust].velocity.Y -= 0.1f;
                    Main.playerDrawDust.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.15f;
                    g *= 0.01f;
                    b *= 0.01f;
                    fullBright = true;
                }
            }
            if (bloodfinBoost)
            {
                if (Main.rand.NextBool(6) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 5, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.5f;
                    g *= 0f;
                    b *= 0f;
                    fullBright = true;
                }
            }
        }
        #endregion

        #region Nurse Modifications
        public override void ModifyNursePrice(NPC nurse, int health, bool removeDebuffs, ref int price)
        {
            // In Rev+, nurse costs scale as the game progresses.
            // Base:            300     3 silver
            // EoC:             900     9 silver
            // Skeletron:       1200    12 silver
            // Hardmode:        2400    24 silver
            // Any Mech Boss:   4000    40 silver
            // Plantera/Cal:    6000    60 silver
            // Golem:           9000    90 silver
            // Fish/PBG/Rav:    12000   1 gold 20 silver
            // Moon Lord:       20000   2 gold
            // Providence:      32000   3 gold 20 silver
            // DoG:             60000   6 gold
            // Yharon:          90000   9 gold

            if (CalamityWorld.revenge && price > 0)
            {
                // start with a vanilla cost of zero instead of 3 silver
                price -= Item.buyPrice(0, 0, 3, 0);

                if (CalamityWorld.downedYharon)
                    price += Item.buyPrice(0, 9, 0, 0);
                else if (CalamityWorld.downedDoG)
                    price += Item.buyPrice(0, 6, 0, 0);
                else if (CalamityWorld.downedProvidence)
                    price += Item.buyPrice(0, 3, 20, 0);
                else if (NPC.downedMoonlord)
                    price += Item.buyPrice(0, 2, 0, 0);
                else if (NPC.downedFishron || CalamityWorld.downedPlaguebringer || CalamityWorld.downedScavenger)
                    price += Item.buyPrice(0, 1, 20, 0);
                else if (NPC.downedGolemBoss)
                    price += Item.buyPrice(0, 0, 90, 0);
                else if (NPC.downedPlantBoss || CalamityWorld.downedCalamitas)
                    price += Item.buyPrice(0, 0, 60, 0);
                else if (NPC.downedMechBossAny)
                    price += Item.buyPrice(0, 0, 40, 0);
                else if (Main.hardMode)
                    price += Item.buyPrice(0, 0, 24, 0);
                else if (NPC.downedBoss3)
                    price += Item.buyPrice(0, 0, 12, 0);
                else if (NPC.downedBoss1)
                    price += Item.buyPrice(0, 0, 6, 0);
                else
                    price += Item.buyPrice(0, 0, 3, 0);

                if (areThereAnyDamnBosses)
                    price *= 5;
            }
        }
        #endregion

        #region All-Class Crit Boost
        public void AllCritBoost(int boost)
        {
            player.meleeCrit += boost;
            player.rangedCrit += boost;
            player.magicCrit += boost;
            player.thrownCrit += boost;
            player.Calamity().throwingCrit += boost;
        }
        #endregion

        #region Rogue Stealth
        private void ResetRogueStealth()
        {
            // rogueStealth doesn't reset every frame because it's a continuously building resource

            // these other parameters are rebuilt every frame based on the items you have equipped
            rogueStealthMax = 0f;
            stealthGenStandstill = 1f;
            stealthGenMoving = 0f;
            stealthGenMultiplier = 1f;
            stealthStrikeThisFrame = false;
            stealthStrikeHalfCost = false;
            stealthStrikeAlwaysCrits = false;
        }

        private void UpdateRogueStealth()
        {
            // If the player un-equips rogue armor, then reset the sound so it'll play again when they re-equip it
            if (!wearingRogueArmor)
            {
                rogueStealth = 0f;
                playRogueStealthSound = false;
                return;
            }

            // Sound plays upon hitting full stealth, not upon having stealth strike available (this can occur at lower than 100% stealth)
            if (playRogueStealthSound && rogueStealth >= rogueStealthMax)
            {
                playRogueStealthSound = false;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/RogueStealth"), (int)player.position.X, (int)player.position.Y);
            }

            // If the player isn't at full stealth, reset the sound so it'll play again when they hit full stealth.
            else if (rogueStealth < rogueStealthMax)
                playRogueStealthSound = true;

            // Calculate stealth generation and gain stealth accordingly
            if (wearingRogueArmor)
            {
                float currentStealthGen = UpdateStealthGenStats();
                rogueStealth += rogueStealthMax * 0.006f * currentStealthGen;
                if (rogueStealth > rogueStealthMax)
                    rogueStealth = rogueStealthMax;
            }

            ProvideStealthStatBonuses();

            // If the player is using an item that deals damage and is on their first frame of doing so,
            // consume stealth if a stealth strike wasn't triggered manually by item code.

            // This doesn't trigger stealth strike effects (ConsumeStealthStrike instead of StealthStrike)
            // so non-rogue weapons can't call lasers down from the sky and such.
            // Using any item which deals no damage or is a tool doesn't consume stealth.
            bool playerUsingWeapon = player.HeldItem.damage > 0 && player.HeldItem.pick <= 0 && player.HeldItem.hammer <= 0 && player.HeldItem.axe <= 0;
            if (!stealthStrikeThisFrame && player.itemAnimation == player.itemAnimationMax - 1 && playerUsingWeapon)
                ConsumeStealthByAttacking();
        }

        private void ProvideStealthStatBonuses()
        {
            CalamityPlayer roguePlayer = player.Calamity();

            // At full stealth, you get 100% of the max possible bonus. Partial stealth only gives you 75% of the partial bonus you have.
            if (rogueStealth >= rogueStealthMax)
                roguePlayer.throwingDamage += rogueStealth * 1.0f;
            else
                roguePlayer.throwingDamage += rogueStealth * 0.75f;

            // Crit increases massively based on your stealth value. With certain gear, it's locked at 100% for stealth strikes.
            if (stealthStrikeAlwaysCrits && StealthStrikeAvailable())
                roguePlayer.throwingCrit = 100;
            else
                roguePlayer.throwingCrit += (int)(rogueStealth * 30f);

            // Stealth increases movement speed and significantly decreases aggro.
            if (wearingRogueArmor && rogueStealthMax > 0)
            {
                player.moveSpeed += rogueStealth * 0.05f;
                player.aggro -= (int)(rogueStealth / rogueStealthMax * 900f);
            }
        }

        private float UpdateStealthGenStats()
        {
            // If you are actively using an item, you cannot gain stealth even while moving.
            if (player.itemAnimation > 0)
                return 0f;

            // Penumbra Potion provides 10% stealth regen while moving, 20% at night and 30% during an eclipse
            if (penumbra)
            {
                if (Main.eclipse || umbraphileSet)
                    stealthGenMoving += 0.3f;
                else if (!Main.dayTime)
                    stealthGenMoving += 0.2f;
                else
                    stealthGenMoving += 0.1f;
            }

			if (CalamityMod.daggerList.Contains(player.inventory[player.selectedItem].type) && player.invis)
			{
				stealthGenMoving += 0.2f;
			}

			if (etherealExtorter && Main.moonPhase == 3) //Waning Crescent
			{
				stealthGenMoving += 0.1f;
			}

            bool standstill = Math.Abs(player.velocity.X) < 0.1f && Math.Abs(player.velocity.Y) < 0.1f && !player.mount.Active;

            // Stealth gen acceleration
            float stealthScaleCap = eclipseMirror ? 40f : 25f;
            if (standstill && (eclipseMirror || darkGodSheath) && player.itemAnimation == 0)
            {
                if (eclipseMirror)
                    stealthGenAcceleration = stealthGenAcceleration < stealthScaleCap ? stealthGenAcceleration * 1.2f : stealthScaleCap;
                if (darkGodSheath)
                    stealthGenAcceleration = stealthGenAcceleration < stealthScaleCap ? stealthGenAcceleration + 0.15f : stealthScaleCap;
            }
            else
            {
                stealthGenAcceleration = 1f;
            }

            //
            // Other code which affects stealth generation goes here.
            // Increase stealthGenStandstill (default 1.0) to increase basic "stand still" stealth generation.
            // Incrase stealthGenMoving (default 0.0) to increase stealth generation while moving.
            // Increase stealthGenMultiplier (default 1.0) to provide a global % boost to all stealth generation.
            //

            // You get 100% stealth regen while standing still and not on a mount. Otherwise, you get your stealth regeneration while moving.
            if (standstill)
            {
                if (eclipseMirror || darkGodSheath)
                    return stealthGenStandstill * stealthGenMultiplier * stealthGenAcceleration;
                else
                    return stealthGenStandstill * stealthGenMultiplier;
            }
            else
            {
                return stealthGenMoving * stealthGenMultiplier;
            }
        }

        public bool StealthStrikeAvailable()
        {
            if (rogueStealthMax <= 0f)
                return false;
            return rogueStealth >= rogueStealthMax * (stealthStrikeHalfCost ? 0.5f : 1f);
        }

        public void StealthStrike()
        {
            //
            // Stuff that happens on stealth strike goes here. Call this from item code.
            //

            ConsumeStealthByAttacking();
        }

        private void ConsumeStealthByAttacking()
        {
            stealthStrikeThisFrame = true;

            if (stealthStrikeHalfCost)
            {
                rogueStealth -= 0.5f * rogueStealthMax;
                if (rogueStealth <= 0f)
                    rogueStealth = 0f;
            }
            else
                rogueStealth = 0f;
        }
        #endregion

        #region Packet Stuff
        private void ExactLevelPacket(bool server, int levelType)
        {
            ModPacket packet = mod.GetPacket(256);
            switch (levelType)
            {
                case 0:
                    packet.Write((byte)CalamityModMessageType.ExactMeleeLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(exactMeleeLevel);
                    break;
                case 1:
                    packet.Write((byte)CalamityModMessageType.ExactRangedLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(exactRangedLevel);
                    break;
                case 2:
                    packet.Write((byte)CalamityModMessageType.ExactMagicLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(exactMagicLevel);
                    break;
                case 3:
                    packet.Write((byte)CalamityModMessageType.ExactSummonLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(exactSummonLevel);
                    break;
                case 4:
                    packet.Write((byte)CalamityModMessageType.ExactRogueLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(exactRogueLevel);
                    break;
            }

            if (!server)
                packet.Send();
            else
                packet.Send(-1, player.whoAmI);
        }

        private void LevelPacket(bool server, int levelType)
        {
            ModPacket packet = mod.GetPacket(256);
            switch (levelType)
            {
                case 0:
                    packet.Write((byte)CalamityModMessageType.MeleeLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(meleeLevel);
                    break;
                case 1:
                    packet.Write((byte)CalamityModMessageType.RangedLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(rangedLevel);
                    break;
                case 2:
                    packet.Write((byte)CalamityModMessageType.MagicLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(magicLevel);
                    break;
                case 3:
                    packet.Write((byte)CalamityModMessageType.SummonLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(summonLevel);
                    break;
                case 4:
                    packet.Write((byte)CalamityModMessageType.RogueLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(rogueLevel);
                    break;
            }

            if (!server)
                packet.Send();
            else
                packet.Send(-1, player.whoAmI);
        }

        private void StressPacket(bool server)
        {
            ModPacket packet = mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.StressSync);
            packet.Write(player.whoAmI);
            packet.Write(stress);

            if (!server)
                packet.Send();
            else
                packet.Send(-1, player.whoAmI);
        }

        private void AdrenalinePacket(bool server)
        {
            ModPacket packet = mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.AdrenalineSync);
            packet.Write(player.whoAmI);
            packet.Write(adrenaline);

            if (!server)
                packet.Send();
            else
                packet.Send(-1, player.whoAmI);
        }

        /*private void DistanceFromBossPacket(bool server)
        {
            ModPacket packet = mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.DistanceFromBossSync);
            packet.Write(player.whoAmI);
            packet.Write(distanceFromBoss);
            if (!server)
            {
                packet.Send();
            }
            else
            {
                packet.Send(-1, player.whoAmI);
            }
        }*/

        private void DeathPacket(bool server)
        {
            ModPacket packet = mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.DeathCountSync);
            packet.Write(player.whoAmI);
            packet.Write(deathCount);

            if (!server)
                packet.Send();
            else
                packet.Send(-1, player.whoAmI);
        }

        internal void HandleExactLevels(BinaryReader reader, int levelType)
        {
            switch (levelType)
            {
                case 0:
                    exactMeleeLevel = reader.ReadInt32();
                    break;
                case 1:
                    exactRangedLevel = reader.ReadInt32();
                    break;
                case 2:
                    exactMagicLevel = reader.ReadInt32();
                    break;
                case 3:
                    exactSummonLevel = reader.ReadInt32();
                    break;
                case 4:
                    exactRogueLevel = reader.ReadInt32();
                    break;
            }

            if (Main.netMode == NetmodeID.Server)
                ExactLevelPacket(true, levelType);
        }

        internal void HandleLevels(BinaryReader reader, int levelType)
        {
            switch (levelType)
            {
                case 0:
                    meleeLevel = reader.ReadInt32();
                    break;
                case 1:
                    rangedLevel = reader.ReadInt32();
                    break;
                case 2:
                    magicLevel = reader.ReadInt32();
                    break;
                case 3:
                    summonLevel = reader.ReadInt32();
                    break;
                case 4:
                    rogueLevel = reader.ReadInt32();
                    break;
            }

            if (Main.netMode == NetmodeID.Server)
                LevelPacket(true, levelType);
        }

        internal void HandleStress(BinaryReader reader)
        {
            stress = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
                StressPacket(true);
        }

        internal void HandleAdrenaline(BinaryReader reader)
        {
            adrenaline = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
                AdrenalinePacket(true);
        }

        /*internal void HandleDistanceFromBoss(BinaryReader reader)
        {
            distanceFromBoss = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
            {
                DistanceFromBossPacket(true);
            }
        }*/

        internal void HandleDeathCount(BinaryReader reader)
        {
            deathCount = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
                DeathPacket(true);
        }

        public override void OnEnterWorld(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ExactLevelPacket(false, 0);
                ExactLevelPacket(false, 1);
                ExactLevelPacket(false, 2);
                ExactLevelPacket(false, 3);
                ExactLevelPacket(false, 4);
                LevelPacket(false, 0);
                LevelPacket(false, 1);
                LevelPacket(false, 2);
                LevelPacket(false, 3);
                LevelPacket(false, 4);
                StressPacket(false);
                AdrenalinePacket(false);
                //DistanceFromBossPacket(false);
                DeathPacket(false);
            }
        }
        #endregion

        #region Proficiency Stuff
        private void GetExactLevelUp()
        {
            if (gainLevelCooldown > 0)
                gainLevelCooldown--;

            #region MeleeLevels
            switch (meleeLevel)
            {
                case 100:
                    this.ExactLevelUp(0, 1, false);
                    break;
                case 300:
                    this.ExactLevelUp(0, 2, false);
                    break;
                case 600:
                    this.ExactLevelUp(0, 3, false);
                    break;
                case 1000:
                    this.ExactLevelUp(0, 4, false);
                    break;
                case 1500:
                    this.ExactLevelUp(0, 5, false);
                    break;
                case 2100:
                    this.ExactLevelUp(0, 6, false);
                    break;
                case 2800:
                    this.ExactLevelUp(0, 7, false);
                    break;
                case 3600:
                    this.ExactLevelUp(0, 8, false);
                    break;
                case 4500:
                    this.ExactLevelUp(0, 9, false);
                    break;
                case 5500:
                    this.ExactLevelUp(0, 10, false);
                    break;
                case 6600:
                    this.ExactLevelUp(0, 11, false);
                    break;
                case 7800:
                    this.ExactLevelUp(0, 12, false);
                    break;
                case 9100:
                    this.ExactLevelUp(0, 13, false);
                    break;
                case 10500:
                    this.ExactLevelUp(0, 14, false);
                    break;
                case 12500: //celebration or some shit for final level, yay
                    this.ExactLevelUp(0, 15, true);
                    break;
                default:
                    break;
            }
            #endregion

            #region RangedLevels
            switch (rangedLevel)
            {
                case 100:
                    this.ExactLevelUp(1, 1, false);
                    break;
                case 300:
                    this.ExactLevelUp(1, 2, false);
                    break;
                case 600:
                    this.ExactLevelUp(1, 3, false);
                    break;
                case 1000:
                    this.ExactLevelUp(1, 4, false);
                    break;
                case 1500:
                    this.ExactLevelUp(1, 5, false);
                    break;
                case 2100:
                    this.ExactLevelUp(1, 6, false);
                    break;
                case 2800:
                    this.ExactLevelUp(1, 7, false);
                    break;
                case 3600:
                    this.ExactLevelUp(1, 8, false);
                    break;
                case 4500:
                    this.ExactLevelUp(1, 9, false);
                    break;
                case 5500:
                    this.ExactLevelUp(1, 10, false);
                    break;
                case 6600:
                    this.ExactLevelUp(1, 11, false);
                    break;
                case 7800:
                    this.ExactLevelUp(1, 12, false);
                    break;
                case 9100:
                    this.ExactLevelUp(1, 13, false);
                    break;
                case 10500:
                    this.ExactLevelUp(1, 14, false);
                    break;
                case 12500: //celebration or some shit for final level, yay
                    this.ExactLevelUp(1, 15, true);
                    break;
                default:
                    break;
            }
            #endregion

            #region MagicLevels
            switch (magicLevel)
            {
                case 100:
                    this.ExactLevelUp(2, 1, false);
                    break;
                case 300:
                    this.ExactLevelUp(2, 2, false);
                    break;
                case 600:
                    this.ExactLevelUp(2, 3, false);
                    break;
                case 1000:
                    this.ExactLevelUp(2, 4, false);
                    break;
                case 1500:
                    this.ExactLevelUp(2, 5, false);
                    break;
                case 2100:
                    this.ExactLevelUp(2, 6, false);
                    break;
                case 2800:
                    this.ExactLevelUp(2, 7, false);
                    break;
                case 3600:
                    this.ExactLevelUp(2, 8, false);
                    break;
                case 4500:
                    this.ExactLevelUp(2, 9, false);
                    break;
                case 5500:
                    this.ExactLevelUp(2, 10, false);
                    break;
                case 6600:
                    this.ExactLevelUp(2, 11, false);
                    break;
                case 7800:
                    this.ExactLevelUp(2, 12, false);
                    break;
                case 9100:
                    this.ExactLevelUp(2, 13, false);
                    break;
                case 10500:
                    this.ExactLevelUp(2, 14, false);
                    break;
                case 12500: //celebration or some shit for final level, yay
                    this.ExactLevelUp(2, 15, true);
                    break;
                default:
                    break;
            }
            #endregion

            #region SummonLevels
            switch (summonLevel)
            {
                case 100:
                    this.ExactLevelUp(3, 1, false);
                    break;
                case 300:
                    this.ExactLevelUp(3, 2, false);
                    break;
                case 600:
                    this.ExactLevelUp(3, 3, false);
                    break;
                case 1000:
                    this.ExactLevelUp(3, 4, false);
                    break;
                case 1500:
                    this.ExactLevelUp(3, 5, false);
                    break;
                case 2100:
                    this.ExactLevelUp(3, 6, false);
                    break;
                case 2800:
                    this.ExactLevelUp(3, 7, false);
                    break;
                case 3600:
                    this.ExactLevelUp(3, 8, false);
                    break;
                case 4500:
                    this.ExactLevelUp(3, 9, false);
                    break;
                case 5500:
                    this.ExactLevelUp(3, 10, false);
                    break;
                case 6600:
                    this.ExactLevelUp(3, 11, false);
                    break;
                case 7800:
                    this.ExactLevelUp(3, 12, false);
                    break;
                case 9100:
                    this.ExactLevelUp(3, 13, false);
                    break;
                case 10500:
                    this.ExactLevelUp(3, 14, false);
                    break;
                case 12500: //celebration or some shit for final level, yay
                    this.ExactLevelUp(3, 15, true);
                    break;
                default:
                    break;
            }
            #endregion

            #region RogueLevels
            switch (rogueLevel)
            {
                case 100:
                    this.ExactLevelUp(4, 1, false);
                    break;
                case 300:
                    this.ExactLevelUp(4, 2, false);
                    break;
                case 600:
                    this.ExactLevelUp(4, 3, false);
                    break;
                case 1000:
                    this.ExactLevelUp(4, 4, false);
                    break;
                case 1500:
                    this.ExactLevelUp(4, 5, false);
                    break;
                case 2100:
                    this.ExactLevelUp(4, 6, false);
                    break;
                case 2800:
                    this.ExactLevelUp(4, 7, false);
                    break;
                case 3600:
                    this.ExactLevelUp(4, 8, false);
                    break;
                case 4500:
                    this.ExactLevelUp(4, 9, false);
                    break;
                case 5500:
                    this.ExactLevelUp(4, 10, false);
                    break;
                case 6600:
                    this.ExactLevelUp(4, 11, false);
                    break;
                case 7800:
                    this.ExactLevelUp(4, 12, false);
                    break;
                case 9100:
                    this.ExactLevelUp(4, 13, false);
                    break;
                case 10500:
                    this.ExactLevelUp(4, 14, false);
                    break;
                case 12500: //celebration or some shit for final level, yay
                    this.ExactLevelUp(4, 15, true);
                    break;
                default:
                    break;
            }
            #endregion
        }

        private void ExactLevelUp(int levelUpType, int level, bool final)
        {
            Color messageColor = Color.Orange;
            switch (levelUpType)
            {
                case 0:
                    exactMeleeLevel = level;
                    if (shootFireworksLevelUpMelee)
                    {
                        string key = final ? "Mods.CalamityMod.MeleeLevelUpFinal" : "Mods.CalamityMod.MeleeLevelUp";
                        shootFireworksLevelUpMelee = false;
                        if (player.whoAmI == Main.myPlayer)
                        {
                            if (final)
                            {
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworkRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 1f);
                            }
                            else
                            {
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworksBoxRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 0f);
                            }
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                    }
                    break;
                case 1:
                    exactRangedLevel = level;
                    if (shootFireworksLevelUpRanged)
                    {
                        string key = final ? "Mods.CalamityMod.RangedLevelUpFinal" : "Mods.CalamityMod.RangedLevelUp";
                        messageColor = Color.GreenYellow;
                        shootFireworksLevelUpRanged = false;
                        if (player.whoAmI == Main.myPlayer)
                        {
                            if (final)
                            {
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworkRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 1f);
                            }
                            else
                            {
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworksBoxRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 0f);
                            }
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                    }
                    break;
                case 2:
                    exactMagicLevel = level;
                    if (shootFireworksLevelUpMagic)
                    {
                        string key = final ? "Mods.CalamityMod.MagicLevelUpFinal" : "Mods.CalamityMod.MagicLevelUp";
                        messageColor = Color.DodgerBlue;
                        shootFireworksLevelUpMagic = false;
                        if (player.whoAmI == Main.myPlayer)
                        {
                            if (final)
                            {
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworkRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 1f);
                            }
                            else
                            {
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworksBoxRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 0f);
                            }
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                    }
                    break;
                case 3:
                    exactSummonLevel = level;
                    if (shootFireworksLevelUpSummon)
                    {
                        string key = final ? "Mods.CalamityMod.SummonLevelUpFinal" : "Mods.CalamityMod.SummonLevelUp";
                        messageColor = Color.Aquamarine;
                        shootFireworksLevelUpSummon = false;
                        if (player.whoAmI == Main.myPlayer)
                        {
                            if (final)
                            {
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworkRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 1f);
                            }
                            else
                            {
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworksBoxRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 0f);
                            }
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                    }
                    break;
                case 4:
                    exactRogueLevel = level;
                    if (shootFireworksLevelUpRogue)
                    {
                        string key = final ? "Mods.CalamityMod.RogueLevelUpFinal" : "Mods.CalamityMod.RogueLevelUp";
                        messageColor = Color.Orchid;
                        shootFireworksLevelUpRogue = false;
                        if (player.whoAmI == Main.myPlayer)
                        {
                            if (final)
                            {
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworkRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 1f);
                            }
                            else
                            {
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworksBoxRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 0f);
                            }
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                    }
                    break;
            }
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ExactLevelPacket(false, levelUpType);
            }
        }

        private void GetStatBonuses()
        {
            #region MeleeLevelBoosts
            if (meleeLevel >= 12500)
            {
                player.meleeDamage += 0.12f;
                player.meleeCrit += 6;
            }
            else if (meleeLevel >= 10500)
            {
                player.meleeDamage += 0.1f;
                player.meleeCrit += 5;
            }
            else if (meleeLevel >= 9100)
            {
                player.meleeDamage += 0.09f;
                player.meleeCrit += 5;
            }
            else if (meleeLevel >= 7800)
            {
                player.meleeDamage += 0.08f;
                player.meleeCrit += 4;
            }
            else if (meleeLevel >= 6600)
            {
                player.meleeDamage += 0.07f;
                player.meleeCrit += 4;
            }
            else if (meleeLevel >= 5500) //hm limit
            {
                player.meleeDamage += 0.06f;
                player.meleeCrit += 3;
            }
            else if (meleeLevel >= 4500)
            {
                player.meleeDamage += 0.05f;
                player.meleeCrit += 3;
            }
            else if (meleeLevel >= 3600)
            {
                player.meleeDamage += 0.05f;
                player.meleeCrit += 2;
            }
            else if (meleeLevel >= 2800)
            {
                player.meleeDamage += 0.04f;
                player.meleeCrit += 2;
            }
            else if (meleeLevel >= 2100)
            {
                player.meleeDamage += 0.04f;
                player.meleeCrit += 1;
            }
            else if (meleeLevel >= 1500) //prehm limit
            {
                player.meleeDamage += 0.03f;
                player.meleeCrit += 1;
            }
            else if (meleeLevel >= 1000)
            {
                player.meleeDamage += 0.03f;
                player.meleeCrit += 1;
            }
            else if (meleeLevel >= 600)
            {
                player.meleeDamage += 0.02f;
            }
            else if (meleeLevel >= 300)
                player.meleeDamage += 0.02f;
            else if (meleeLevel >= 100)
                player.meleeDamage += 0.01f;
            #endregion

            #region RangedLevelBoosts
            if (rangedLevel >= 12500)
            {
                player.rangedDamage += 0.12f;
                player.moveSpeed += 0.12f;
                player.rangedCrit += 6;
            }
            else if (rangedLevel >= 10500)
            {
                player.rangedDamage += 0.1f;
                player.moveSpeed += 0.1f;
                player.rangedCrit += 5;
            }
            else if (rangedLevel >= 9100)
            {
                player.rangedDamage += 0.09f;
                player.moveSpeed += 0.09f;
                player.rangedCrit += 5;
            }
            else if (rangedLevel >= 7800)
            {
                player.rangedDamage += 0.08f;
                player.moveSpeed += 0.08f;
                player.rangedCrit += 4;
            }
            else if (rangedLevel >= 6600)
            {
                player.rangedDamage += 0.07f;
                player.moveSpeed += 0.07f;
                player.rangedCrit += 4;
            }
            else if (rangedLevel >= 5500)
            {
                player.rangedDamage += 0.06f;
                player.moveSpeed += 0.06f;
                player.rangedCrit += 3;
            }
            else if (rangedLevel >= 4500)
            {
                player.rangedDamage += 0.05f;
                player.moveSpeed += 0.05f;
                player.rangedCrit += 3;
            }
            else if (rangedLevel >= 3600)
            {
                player.rangedDamage += 0.05f;
                player.moveSpeed += 0.05f;
                player.rangedCrit += 2;
            }
            else if (rangedLevel >= 2800)
            {
                player.rangedDamage += 0.04f;
                player.moveSpeed += 0.04f;
                player.rangedCrit += 2;
            }
            else if (rangedLevel >= 2100)
            {
                player.rangedDamage += 0.04f;
                player.moveSpeed += 0.03f;
                player.rangedCrit += 1;
            }
            else if (rangedLevel >= 1500)
            {
                player.rangedDamage += 0.03f;
                player.moveSpeed += 0.02f;
                player.rangedCrit += 1;
            }
            else if (rangedLevel >= 1000)
            {
                player.rangedDamage += 0.03f;
                player.moveSpeed += 0.01f;
                player.rangedCrit += 1;
            }
            else if (rangedLevel >= 600)
            {
                player.rangedDamage += 0.02f;
                player.rangedCrit += 1;
            }
            else if (rangedLevel >= 300)
                player.rangedDamage += 0.02f;
            else if (rangedLevel >= 100)
                player.rangedDamage += 0.01f;
            #endregion

            #region MagicLevelBoosts
            if (magicLevel >= 12500)
            {
                player.magicDamage += 0.12f;
                player.manaCost *= 0.88f;
                player.magicCrit += 6;
            }
            else if (magicLevel >= 10500)
            {
                player.magicDamage += 0.1f;
                player.manaCost *= 0.9f;
                player.magicCrit += 5;
            }
            else if (magicLevel >= 9100)
            {
                player.magicDamage += 0.09f;
                player.manaCost *= 0.91f;
                player.magicCrit += 5;
            }
            else if (magicLevel >= 7800)
            {
                player.magicDamage += 0.08f;
                player.manaCost *= 0.92f;
                player.magicCrit += 4;
            }
            else if (magicLevel >= 6600)
            {
                player.magicDamage += 0.07f;
                player.manaCost *= 0.93f;
                player.magicCrit += 4;
            }
            else if (magicLevel >= 5500)
            {
                player.magicDamage += 0.06f;
                player.manaCost *= 0.94f;
                player.magicCrit += 3;
            }
            else if (magicLevel >= 4500)
            {
                player.magicDamage += 0.05f;
                player.manaCost *= 0.95f;
                player.magicCrit += 3;
            }
            else if (magicLevel >= 3600)
            {
                player.magicDamage += 0.05f;
                player.manaCost *= 0.95f;
                player.magicCrit += 2;
            }
            else if (magicLevel >= 2800)
            {
                player.magicDamage += 0.04f;
                player.manaCost *= 0.96f;
                player.magicCrit += 2;
            }
            else if (magicLevel >= 2100)
            {
                player.magicDamage += 0.04f;
                player.manaCost *= 0.97f;
                player.magicCrit += 1;
            }
            else if (magicLevel >= 1500)
            {
                player.magicDamage += 0.03f;
                player.manaCost *= 0.98f;
                player.magicCrit += 1;
            }
            else if (magicLevel >= 1000)
            {
                player.magicDamage += 0.03f;
                player.manaCost *= 0.99f;
                player.magicCrit += 1;
            }
            else if (magicLevel >= 600)
            {
                player.magicDamage += 0.02f;
                player.manaCost *= 0.99f;
            }
            else if (magicLevel >= 300)
                player.magicDamage += 0.02f;
            else if (magicLevel >= 100)
                player.magicDamage += 0.01f;
            #endregion

            #region SummonLevelBoosts
            if (summonLevel >= 12500)
            {
                player.minionDamage += 0.12f;
                player.minionKB += 3.0f;
                player.maxMinions += 3;
            }
            else if (summonLevel >= 10500)
            {
                player.minionDamage += 0.1f;
                player.minionKB += 3.0f;
                player.maxMinions += 2;
            }
            else if (summonLevel >= 9100)
            {
                player.minionDamage += 0.09f;
                player.minionKB += 2.7f;
                player.maxMinions += 2;
            }
            else if (summonLevel >= 7800)
            {
                player.minionDamage += 0.08f;
                player.minionKB += 2.4f;
                player.maxMinions += 2;
            }
            else if (summonLevel >= 6600)
            {
                player.minionDamage += 0.07f;
                player.minionKB += 2.1f;
                player.maxMinions += 2;
            }
            else if (summonLevel >= 5500)
            {
                player.minionDamage += 0.06f;
                player.minionKB += 1.8f;
                player.maxMinions += 2;
            }
            else if (summonLevel >= 4500)
            {
                player.minionDamage += 0.06f;
                player.minionKB += 1.8f;
                player.maxMinions++;
            }
            else if (summonLevel >= 3600)
            {
                player.minionDamage += 0.05f;
                player.minionKB += 1.5f;
                player.maxMinions++;
            }
            else if (summonLevel >= 2800)
            {
                player.minionDamage += 0.04f;
                player.minionKB += 1.2f;
                player.maxMinions++;
            }
            else if (summonLevel >= 2100)
            {
                player.minionDamage += 0.04f;
                player.minionKB += 0.9f;
                player.maxMinions++;
            }
            else if (summonLevel >= 1500)
            {
                player.minionDamage += 0.03f;
                player.minionKB += 0.6f;
            }
            else if (summonLevel >= 1000)
            {
                player.minionDamage += 0.03f;
                player.minionKB += 0.3f;
            }
            else if (summonLevel >= 600)
            {
                player.minionDamage += 0.02f;
                player.minionKB += 0.3f;
            }
            else if (summonLevel >= 300)
                player.minionDamage += 0.02f;
            else if (summonLevel >= 100)
                player.minionDamage += 0.01f;
            #endregion

            #region RogueLevelBoosts
            if (rogueLevel >= 12500)
            {
                player.Calamity().throwingDamage += 0.12f;
                player.Calamity().throwingVelocity += 0.12f;
                player.Calamity().throwingCrit += 6;
            }
            else if (rogueLevel >= 10500)
            {
                player.Calamity().throwingDamage += 0.1f;
                player.Calamity().throwingVelocity += 0.1f;
                player.Calamity().throwingCrit += 5;
            }
            else if (rogueLevel >= 9100)
            {
                player.Calamity().throwingDamage += 0.09f;
                player.Calamity().throwingVelocity += 0.09f;
                player.Calamity().throwingCrit += 5;
            }
            else if (rogueLevel >= 7800)
            {
                player.Calamity().throwingDamage += 0.08f;
                player.Calamity().throwingVelocity += 0.08f;
                player.Calamity().throwingCrit += 4;
            }
            else if (rogueLevel >= 6600)
            {
                player.Calamity().throwingDamage += 0.07f;
                player.Calamity().throwingVelocity += 0.07f;
                player.Calamity().throwingCrit += 4;
            }
            else if (rogueLevel >= 5500)
            {
                player.Calamity().throwingDamage += 0.06f;
                player.Calamity().throwingVelocity += 0.06f;
                player.Calamity().throwingCrit += 3;
            }
            else if (rogueLevel >= 4500)
            {
                player.Calamity().throwingDamage += 0.05f;
                player.Calamity().throwingVelocity += 0.05f;
                player.Calamity().throwingCrit += 3;
            }
            else if (rogueLevel >= 3600)
            {
                player.Calamity().throwingDamage += 0.05f;
                player.Calamity().throwingVelocity += 0.05f;
                player.Calamity().throwingCrit += 2;
            }
            else if (rogueLevel >= 2800)
            {
                player.Calamity().throwingDamage += 0.04f;
                player.Calamity().throwingVelocity += 0.04f;
                player.Calamity().throwingCrit += 2;
            }
            else if (rogueLevel >= 2100)
            {
                player.Calamity().throwingDamage += 0.04f;
                player.Calamity().throwingVelocity += 0.03f;
                player.Calamity().throwingCrit += 1;
            }
            else if (rogueLevel >= 1500)
            {
                player.Calamity().throwingDamage += 0.03f;
                player.Calamity().throwingVelocity += 0.02f;
                player.Calamity().throwingCrit += 1;
            }
            else if (rogueLevel >= 1000)
            {
                player.Calamity().throwingDamage += 0.03f;
                player.Calamity().throwingVelocity += 0.01f;
                player.Calamity().throwingCrit += 1;
            }
            else if (rogueLevel >= 600)
            {
                player.Calamity().throwingDamage += 0.02f;
                player.Calamity().throwingVelocity += 0.01f;
            }
            else if (rogueLevel >= 300)
                player.Calamity().throwingDamage += 0.02f;
            else if (rogueLevel >= 100)
                player.Calamity().throwingDamage += 0.01f;
            #endregion
        }

        private float GetMeleeSpeedBonus()
        {
            float meleeSpeedBonus = 0f;
            if (meleeLevel >= 12500)
            {
                meleeSpeedBonus += 0.12f;
            }
            else if (meleeLevel >= 10500)
            {
                meleeSpeedBonus += 0.1f;
            }
            else if (meleeLevel >= 9100)
            {
                meleeSpeedBonus += 0.09f;
            }
            else if (meleeLevel >= 7800)
            {
                meleeSpeedBonus += 0.08f;
            }
            else if (meleeLevel >= 6600)
            {
                meleeSpeedBonus += 0.07f;
            }
            else if (meleeLevel >= 5500) //hm limit
            {
                meleeSpeedBonus += 0.06f;
            }
            else if (meleeLevel >= 4500)
            {
                meleeSpeedBonus += 0.05f;
            }
            else if (meleeLevel >= 3600)
            {
                meleeSpeedBonus += 0.05f;
            }
            else if (meleeLevel >= 2800)
            {
                meleeSpeedBonus += 0.04f;
            }
            else if (meleeLevel >= 2100)
            {
                meleeSpeedBonus += 0.03f;
            }
            else if (meleeLevel >= 1500) //prehm limit
            {
                meleeSpeedBonus += 0.02f;
            }
            else if (meleeLevel >= 1000)
            {
                meleeSpeedBonus += 0.01f;
            }
            else if (meleeLevel >= 600)
            {
                meleeSpeedBonus += 0.01f;
            }
            return meleeSpeedBonus;
        }
        #endregion

        #region Misc Stuff

        /// <summary>
        /// Returns the range at which an abyss enemy can detect the player
        /// </summary>
        /// <param name="defaultRange">The default detection range</param>
        /// <param name="anechoicRange">The detection range set by the player having anechoic plating/coating</param>
        /// <returns></returns>
        public float GetAbyssAggro(float defaultRange, float anechoicRange)
        {
            /* ((Main.player[npc.target].GetCalamityPlayer().anechoicPlating ||
                    Main.player[npc.target].GetCalamityPlayer().anechoicCoating) ? 200f : 600f) *
                    (Main.player[npc.target].GetCalamityPlayer().fishAlert ? 3f : 1f) *
                    (Main.player[npc.target].GetCalamityPlayer().abyssalMirror ? 0.7f : 1f) *
                    (Main.player[npc.target].GetCalamityPlayer().eclipseMirror ? 0.3f : 1f) */
            float range = anechoicPlating || anechoicCoating ? anechoicRange : defaultRange;
            range *= fishAlert ? 3f : 1f;
            range *= abyssalMirror ? 0.65f : 1f;
            range *= eclipseMirror ? 0.3f : 1f;
            return range;
        }

        #endregion
    }
}
