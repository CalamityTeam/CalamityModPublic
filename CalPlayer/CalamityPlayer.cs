using CalamityMod.Buffs;
using CalamityMod.Buffs.Cooldowns;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Potions;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Armor;
using CalamityMod.Items.DifficultyItems;
using CalamityMod.Items.Dyes;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Weapons.Typeless;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.GreatSandShark;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.Projectiles.Environment;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.TileEntities;
using CalamityMod.Tiles;
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
using CalamityMod.Projectiles.DraedonsArsenal;

namespace CalamityMod.CalPlayer
{
    public enum GaelSwitchPhase
    {
        LoseRage = 0,
        None = 1
    }

    public enum AnimationType
    {
        Idle,
        Jump,
        Walk
    }

    public enum AndromedaPlayerState
    {
        Inactive,
        SmallRobot,
        LargeRobot,
        SpecialAttack
    }

    public class CalamityPlayer : ModPlayer
    {
        #region Variables

        #region No Category
        public static bool areThereAnyDamnBosses = false;
        public static bool areThereAnyDamnEvents = false;
        public bool drawBossHPBar = true;
        public bool shouldDrawSmallText = true;
        private const int saveVersion = 0;
        public int dashMod;
        public int projTypeJustHitBy;
        public int sCalDeathCount = 0;
        public int sCalKillCount = 0;
        public int deathCount = 0;
        public int actualMaxLife = 0;
        public int deathModeUnderworldTime = 0;
        public int deathModeBlizzardTime = 0;
        public static int chaosStateDuration = 360;
        public static int chaosStateDurationBoss = 600;
        public bool killSpikyBalls = false;
        public Projectile lastProjectileHit;
        public double acidRoundMultiplier = 1D;
        public int waterLeechTarget = -1;
        public float KameiTrailXScale = 0.1f;
        public int KameiBladeUseDelay = 0;
        public Vector2[] KameiOldPositions = new Vector2[4];
		public double trueMeleeDamage = 0D;
		public double contactDamageReduction = 0D;
		public double projectileDamageReduction = 0D;
		public bool brimlashBusterBoost = false;
		public float animusBoost = 1f;
		public int potionTimer = 0;
		#endregion

        public int CurrentlyViewedFactoryX = -1;
        public int CurrentlyViewedFactoryY = -1;
        public TEDraedonFuelFactory CurrentlyViewedFactory;

        public int CurrentlyViewedChargerX = -1;
        public int CurrentlyViewedChargerY = -1;
        public TEDraedonItemCharger CurrentlyViewedCharger;

        public int CurrentlyViewedHologramX = -1;
        public int CurrentlyViewedHologramY = -1;
        public string CurrentlyViewedHologramText;
        #endregion

        #region External variables -- Only set by Mod.Call
        public int externalAbyssLight = 0;
        public bool externalColdImmunity = false;
        public bool externalHeatImmunity = false;
		#endregion

		#region Town NPC Shop Variables
		public bool newMerchantInventory = false;
		public bool newPainterInventory = false;
		public bool newDyeTraderInventory = false;
		public bool newPartyGirlInventory = false;
		public bool newStylistInventory = false;
		public bool newDemolitionistInventory = false;
		public bool newDryadInventory = false;
		public bool newTavernkeepInventory = false;
		public bool newArmsDealerInventory = false;
		public bool newGoblinTinkererInventory = false;
		public bool newWitchDoctorInventory = false;
		public bool newClothierInventory = false;
		public bool newMechanicInventory = false;
		public bool newPirateInventory = false;
		public bool newTruffleInventory = false;
		public bool newWizardInventory = false;
		public bool newSteampunkerInventory = false;
		public bool newCyborgInventory = false;
		public bool newSkeletonMerchantInventory = false;
		public bool newPermafrostInventory = false;
		public bool newCirrusInventory = false;
		public bool newAmidiasInventory = false;
		public bool newBanditInventory = false;
		#endregion

		#region Stat Meter
		public int[] damageStats = new int[6];
        public int[] critStats = new int[4];
		public float actualMeleeDamageStat = 0f;
        public int defenseStat = 0;
        public int DRStat = 0;
        public int meleeSpeedStat = 0;
        public int manaCostStat = 0;
        public int rogueVelocityStat = 0;
        public int minionSlotStat = 0;
        public int lifeRegenStat = 0;
        public int manaRegenStat = 0;
        public int ammoReductionRanged = 0;
        public int ammoReductionRogue = 0;
        public int armorPenetrationStat = 0;
        public float wingFlightTimeStat = 0f;
		public float jumpSpeedStat = 0f;
        public int adrenalineChargeStat = 0;
        public int rageDamageStat = 0;
        public int moveSpeedStat = 0;
        public int abyssLightLevelStat = 0;
        public int abyssBreathLossStat = 0;
        public int abyssBreathLossRateStat = 0;
        public int abyssLifeLostAtZeroBreathStat = 0;
        public int abyssDefenseLossStat = 0;
        public int stealthStat = 0;
        public float standingRegenStat = 0f;
        public float movingRegenStat = 0f;
        public float stealthUIAlpha = 1f;
        #endregion

        #region Timer and Counter
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
        public int navyRodAuraTimer = 0;
        public int brimLoreInfernoTimer = 0;
        public int tarraLifeAuraTimer = 0;
        public int bloodflareHeartTimer = 180;
        public int bloodflareManaTimer = 180;
        public int polarisBoostCounter = 0;
        public int gaelSwipes = 0;
        public float modStealth = 1f;
        public float aquaticBoost = 1f;
        public float shieldInvinc = 5f;
        public GaelSwitchPhase gaelSwitchTimer = 0;
        public int galileoCooldown = 0;
        public int soundCooldown = 0;
        public int planarSpeedBoost = 0;
        public int profanedSoulWeaponUsage = 0;
        public int profanedSoulWeaponType = 0;
        public int hurtSoundTimer = 0;
        public int danceOfLightCharge = 0;
        public int shadowPotCooldown = 0;
        public int dogTextCooldown = 0;
        #endregion

        #region Sound
        public bool playRogueStealthSound = false;
        public bool playFullRageSound = true;
        public bool playFullAdrenalineSound = true;
        #endregion

        #region Proficiency
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
        #endregion

        #region Rogue
        public float rogueStealth = 0f;
        public float rogueStealthMax = 0f;
        public float stealthGenStandstill = 1f;
        public float stealthGenMoving = 1f;
        public const float StealthAccelerationCap = 2f;
        public float stealthAcceleration = 1f;
        public bool stealthStrikeThisFrame = false;
        public bool stealthStrikeHalfCost = false;
        public bool stealthStrikeAlwaysCrits = false;
        public bool wearingRogueArmor = false;
        public float accStealthGenBoost = 0f;

        public float throwingDamage = 1f;
        public float throwingVelocity = 1f;
        public int throwingCrit = 0;
        public bool throwingAmmoCost75 = false;
        public bool throwingAmmoCost66 = false;
        public bool throwingAmmoCost55 = false;
        public bool throwingAmmoCost50 = false;
        #endregion

        #region Mount
        public bool onyxExcavator = false;
        public bool angryDog = false;
        public bool fab = false;
        public bool crysthamyr = false;
        public AndromedaPlayerState andromedaState;
        public int andromedaCripple;
        #endregion

        #region Pet
        public bool thirdSage = false;
        public bool thirdSageH = true; // Third sage healing
        public bool perfmini = false;
        public bool akato = false;
        public bool leviPet = false;
        public bool plaguebringerBab = false;
        public bool rotomPet = false;
        public bool ladShark = false;
        public int ladHearts = 0;
        public bool sparks = false;
        public bool sirenPet = false;
        public bool fox = false;
        public bool chibii = false;
        public bool brimling = false;
        public bool bearPet = false;
        public bool kendra = false;
        public bool trashMan = false;
        public int trashManChest = -1;
        public bool astrophage = false;
        public bool flakPet = false;
        public bool babyGhostBell = false;
        public bool radiator = false;
        public bool scalPet = false;
        public bool bendyPet = false;
        #endregion

        #region Rage
        public float rage = 0f;
        public float rageMax = 10000f;
        public const int RageDuration = 300;
        public const float AbsoluteRageThreshold = 0.98f; // 98% or higher for Absolute Rage
        public bool rageModeActive = false;
        public int gainRageCooldown = 60;
        #endregion

        #region Adrenaline
        public float adrenaline = 0f;
        public float adrenalineMax = 10000f;
        public const int AdrenalineDuration = 300;
        public bool adrenalineModeActive = false;
        #endregion

        #region Permanent Buff
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
        public bool healToFull = false;
        #endregion

        #region Lore
        public bool kingSlimeLore = false;
        public bool desertScourgeLore = false;
        public bool crabulonLore = false;
        public bool eaterOfWorldsLore = false;
        public bool hiveMindLore = false;
        public bool perforatorLore = false;
        public bool queenBeeLore = false;
        public bool skeletronLore = false;
        // This lore boolean is a bit different from the others. It just stops Slime God lore effects from stacking.
        public bool slimeGodLoreProcessed = false;
        public bool wallOfFleshLore = false;
        public bool twinsLore = false;
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
        public bool boomerDukeLore = false;
        public bool ravagerLore = false;
        public bool lunaticCultistLore = false;
        public bool moonLordLore = false;
        public bool providenceLore = false;
        public bool polterghastLore = false;
        public bool DoGLore = false;
        public bool yharonLore = false;
        public bool SCalLore = false;
        public bool oceanLore = false;
        public bool corruptionLore = false;
        public bool crimsonLore = false;
        public bool underworldLore = false;
        #endregion

        #region Accessory
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
        public int theBeeCooldown = 0;
        public bool alluringBait = false;
        public bool enchantedPearl = false;
        public bool fishingStation = false;
        public bool rBrain = false;
        public bool bloodyWormTooth = false;
        public bool afflicted = false;
        public bool affliction = false;
        public bool stressPills = false;
        public bool laudanum = false;
        public bool doubledHorror = false;
        public bool heartOfDarkness = false;
        public bool draedonsHeart = false;
        public bool rampartOfDeities = false;
        public bool vexation = false;
        public bool fBulwark = false;
        public bool dodgeScarf = false;
        public bool evasionScarf = false;
        public bool badgeOfBravery = false;
        public bool badgeOfBraveryRare = false;
        public bool scarfCooldown = false;
        public bool eScarfCooldown = false;
        public bool cryogenSoul = false;
        public bool yInsignia = false;
        public bool eGauntlet = false;
        public bool eTalisman = false;
        public bool statisBeltOfCurses = false;
        public int statisTimer = 0;
        public bool nucleogenesis = false;
        public bool nuclearRod = false;
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
        public bool rOoze = false;
        public bool pAmulet = false;
        public bool fBarrier = false;
        public bool aBrain = false;
        public bool amalgam = false;
        public bool lol = false;
        public bool raiderTalisman = false;
        public int raiderStack = 0;
        public int raiderCooldown = 0;
        public bool gSabaton = false;
        public int gSabatonFall = 0;
        public int gSabatonCooldown = 0;
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
        public bool reducedPlagueDmg = false;
		public bool abaddon = false;
        public bool community = false;
        public bool fleshTotem = false;
        public bool fleshTotemCooldown = false;
        public bool bloodPact = false;
		public bool bloodPactBoost = false;
        public bool bloodflareCore = false;
        public bool coreOfTheBloodGod = false;
        public bool elementalHeart = false;
        public bool crownJewel = false;
        public bool celestialJewel = false;
        public bool astralArcanum = false;
        public bool harpyRing = false;
        public bool harpyWingBoost = false; //harpy wings + harpy ring
        public bool ironBoots = false;
        public bool depthCharm = false;
        public bool anechoicPlating = false;
        public bool jellyfishNecklace = false;
		public bool abyssDivingGear = false;
        public bool abyssalAmulet = false;
        public bool lumenousAmulet = false;
        public bool reaperToothNecklace = false;
        public bool aquaticEmblem = false;
        public bool darkSunRing = false;
        public bool calamityRing = false;
        public bool voidOfExtinction = false;
        public bool eArtifact = false;
        public bool dArtifact = false;
        public bool gArtifact = false;
        public bool pArtifact = false;
        public bool giantPearl = false;
        public bool normalityRelocator = false;
        public bool fabledTortoise = false;
        public bool manaOverloader = false;
        public bool royalGel = false;
        public bool handWarmer = false;
        public bool oldDie = false;
        public bool ursaSergeant = false;
        public bool scuttlersJewel = false;
        public bool thiefsDime = false;
        public bool dynamoStemCells = false;
        public bool etherealExtorter = false;
        public bool blazingCore = false;
        public bool voltaicJelly = false;
        public bool jellyChargedBattery = false;
        public float jellyDmg;
        public bool dukeScales = false;
        public bool sandWaifu = false;
        public bool sandBoobWaifu = false;
        public bool cloudWaifu = false;
        public bool brimstoneWaifu = false;
        public bool sirenWaifu = false;
        public bool fungalClump = false;
        public bool howlsHeart = false;
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
        public int nanoFlareCooldown = 0;
        public bool dragonScales = false;
        public bool gloveOfPrecision = false;
        public bool gloveOfRecklessness = false;
        public bool momentumCapacitor = false;
        public bool vampiricTalisman = false;
        public bool electricianGlove = false;
        public bool bloodyGlove = false;
        public bool filthyGlove = false;
        public bool sandCloak = false;
        public bool sandCloakCooldown = false;
        public bool spectralVeil = false;
        public int spectralVeilImmunity = 0;
        public bool hasJetpack = false;
        public int jetPackCooldown = 0;
        public bool plaguedFuelPack = false;
        public int plaguedFuelPackDash = 0;
        public int plaguedFuelPackDirection = 0;
        public bool blunderBooster = false;
        public int blunderBoosterDash = 0;
        public int blunderBoosterDirection = 0;
        public bool veneratedLocket = false;
        public bool camper = false;
        public bool corrosiveSpine = false;
        public bool miniOldDuke = false;
        public bool starbusterCore = false;
        public bool starTaintedGenerator = false;
        public bool hallowedRune = false;
		public int hallowedRuneCooldown = 0;
		public bool silvaWings = false;
		public int icicleCooldown = 0;
        public bool rustyMedal = false;
        public bool noStupidNaturalARSpawns = false;
        public bool burdenBreakerYeet = false;
		public bool roverDrive = false;
		public int roverDriveTimer = 0;
		public int roverFrameCounter = 0;
		public int roverFrame = 0;
        #endregion

        #region Armor Set
        public bool desertProwler = false;
        public bool snowRuffianSet = false;
        public bool forbiddenCirclet = false;
		public int forbiddenCooldown = 0;
		public int tornadoCooldown = 0;
        public bool eskimoSet = false; //vanilla armor
        public bool meteorSet = false; //vanilla armor, for space gun nerf
        public bool victideSet = false;
        public bool sulfurSet = false;
        public bool sulfurJump = false;
		public bool jumpAgainSulfur = false;
		public int sulphurBubbleCooldown = 0;
        public bool aeroSet = false;
        public bool statigelSet = false;
        public bool statigelJump = false;
		public bool jumpAgainStatigel = false;
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
        public bool hydrothermalSmoke = false;
        public bool daedalusAbsorb = false;
        public bool daedalusShard = false;
        public bool brimflameSet = false;
        public bool brimflameFrenzy = false;
        public bool brimflameFrenzyCooldown = false;
        public bool reaverSpore = false;
        public bool reaverDoubleTap = false;
        public bool flamethrowerBoost = false;
        public bool hoverboardBoost = false; //hoverboard + shroomite visage
        public bool shadeRegen = false;
        public bool shadowSpeed = false;
        public bool dsSetBonus = false;
        public bool auricBoost = false;
        public bool daedalusReflect = false;
        public bool daedalusSplit = false;
        public bool titanHeartSet = false;
        public bool titanHeartMask = false;
        public bool titanHeartMantle = false;
        public bool titanHeartBoots = false;
        public int titanCooldown = 0;
        public bool umbraphileSet = false;
        public bool reaverBlast = false;
        public bool reaverBurst = false;
        public bool fathomSwarmer = false;
        public bool fathomSwarmerVisage = false;
        public bool fathomSwarmerBreastplate = false;
        public bool fathomSwarmerTail = false;
		int tailFrameUp = 0;
		int tailFrame = 0;
        public bool astralStarRain = false;
        public int astralStarRainCooldown = 0;
        public bool plagueReaper = false;
        public int plagueReaperCooldown = 0;
        public bool plaguebringerPatronSet = false;
        public bool plaguebringerCarapace = false;
        public bool plaguebringerPistons = false;
        public int pistonsCounter = 0;
        public float ataxiaDmg;
        public bool ataxiaMage = false;
        public bool ataxiaGeyser = false;
        public float xerocDmg;
        public bool xerocSet = false;
        public bool prismaticSet = false;
        public bool prismaticHelmet = false;
        public bool prismaticRegalia = false;
        public bool prismaticGreaves = false;
		public int prismaticLasers = 0;
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
        public bool fearmongerSet = false;
        public int fearmongerRegenFrames = 0;
        public bool daedalusCrystal = false;
        public bool reaverOrb = false;
        public bool chaosSpirit = false;
        public bool redDevil = false;
        #endregion

        #region Debuff
        public bool alcoholPoisoning = false;
        public bool shadowflame = false;
        public bool wDeath = false;
        public bool lethalLavaBurn = false;
        public bool aCrunch = false;
        public bool absoluteRage = false;
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
        public bool nightwither = false;
        public bool eFreeze = false;
        public bool silvaStun = false;
        public bool wCleave = false;
        public bool eutrophication = false;
        public bool iCantBreathe = false; //Frozen Lungs debuff
        public bool cragsLava = false;
        public bool vaporfied = false;
        public bool energyShellCooldown = false;
        public bool prismaticCooldown = false;
        #endregion

        #region Buff
        public bool trinketOfChiBuff = false;
        public int chiBuffTimer = 0;
        public bool corrEffigy = false;
        public bool crimEffigy = false;
        public bool decayEffigy = false;
        public bool rRage = false;
        public bool tRegen = false;
        public bool xRage = false;
        public bool xWrath = false;
        public bool graxDefense = false;
        public bool encased = false;
        public bool sMeleeBoost = false;
        public bool eScarfBoost = false;
        public bool tFury = false;
        public bool cadence = false;
        public bool omniscience = false;
        public bool zerg = false;
        public bool zen = false;
        public bool bossZen = false;
        public bool yPower = false;
        public bool aWeapon = false;
        public bool tScale = false;
        public int titanBoost = 0;
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
        public bool shadow = false;
        public bool photosynthesis = false;
        public bool astralInjection = false;
        public bool gravityNormalizer = false;
        public bool holyWrath = false;
        public bool profanedRage = false;
        public bool draconicSurge = false;
        public bool draconicSurgeCooldown = false;
        public bool tesla = false;
        public bool teslaFreeze = false;
        public bool sulphurskin = false;
        public bool baguette = false;
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
        public bool hallowedDefense = false;
        public bool hallowedPower = false;
        public bool hallowedRegen = false;
        public bool kamiBoost = false;
        #endregion

        #region Minion
        public bool wDroid = false;
        public bool resButterfly = false;
        public bool glSword = false;
        public bool mWorm = false;
        public bool iClasper = false;
        public bool magicHat = false;
        public bool herring = false;
        public bool blackhawk = false;
        public bool cosmicViper = false;
        public bool calamari = false;
        public bool cEyes = false;
        public bool cSlime = false;
        public bool cSlime2 = false;
        public bool aSlime = false;
        public bool bStar = false;
        public bool aStar = false;
        public bool SP = false;
        public bool dCreeper = false;
        public bool bClot = false;
        public bool eAxe = false;
        public bool endoCooper = false;
        public bool SPG = false;
        public bool sirius = false;
        public bool aChicken = false;
        public bool cLamp = false;
        public bool pGuy = false;
        public bool sandnado = false;
        public bool plantera = false;
        public bool aProbe = false;
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
        public bool apexShark = false;
        public bool gastricBelcher = false;
        public bool squirrel = false;
        public bool hauntedDishes = false;
        public bool stormjaw = false;
        public bool sGod = false;
        public bool vUrchin = false;
        public bool cSpirit = false;
        public bool rOrb = false;
        public bool dCrystal = false;
        public bool endoHydra = false;
        public bool powerfulRaven = false;
        public bool dragonFamily = false;
        public bool providenceStabber = false;
        public bool radiantResolution = false;
        public bool plaguebringerMK2 = false;
        public bool igneousExaltation = false;
        public bool coldDivinity = false;
        public bool youngDuke = false;
        public bool virili = false;
        public bool frostBlossom = false;
        public bool cinderBlossom = false;
        public bool belladonaSpirit = false;
        public bool vileFeeder = false;
        public bool scabRipper = false;
        public bool midnightUFO = false;
        public bool plagueEngine = false;
        public bool brimseeker = false;
        public bool necrosteocytesDudes = false;
        public bool gammaHead = false;
        public List<int> GammaCanisters = new List<int>();
        public bool rustyDrone = false;
        public bool tundraFlameBlossom = false;
        public bool starSwallowerPetFroge = false;
        public bool snakeEyes = false;
        public bool poleWarper = false;
        public bool causticDragon = false;
        public bool plaguebringerPatronSummon = false;
        public bool howlTrio = false;
        public bool mountedScanner = false;
        #endregion

        #region Biome
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
        public float caveDarkness = 0f;
        #endregion

        #region Transformation
        public bool abyssalDivingSuitPrevious;
        public bool abyssalDivingSuit;
        public bool abyssalDivingSuitHide;
        public bool abyssalDivingSuitForce;
        public bool abyssalDivingSuitPower;
        public bool profanedCrystal;
        public bool profanedCrystalPrevious;
        public bool profanedCrystalForce;
        public bool profanedCrystalBuffs;
        public bool profanedCrystalHide;
        public KeyValuePair<int, int> profanedCrystalWingCounter = new KeyValuePair<int, int>(0, 10);
        public KeyValuePair<int, int> profanedCrystalAnimCounter = new KeyValuePair<int, int>(0, 10);
        public bool sirenBoobsPrevious;
        public bool sirenBoobs;
        public bool sirenBoobsHide;
        public bool sirenBoobsForce;
        public bool sirenBoobsPower;
        public bool snowmanPrevious;
        public bool snowman;
        public bool snowmanHide;
        public bool snowmanForce;
        public bool snowmanNoseless;
        public bool snowmanPower;
        public bool meldTransformationPrevious;
        public bool meldTransformation;
        public bool meldTransformationForce;
        public bool meldTransformationPower;
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
			healToFull = false;

			newMerchantInventory = false;
			newPainterInventory = false;
			newDyeTraderInventory = false;
			newPartyGirlInventory = false;
			newStylistInventory = false;
			newDemolitionistInventory = false;
			newDryadInventory = false;
			newTavernkeepInventory = false;
			newArmsDealerInventory = false;
			newGoblinTinkererInventory = false;
			newWitchDoctorInventory = false;
			newClothierInventory = false;
			newMechanicInventory = false;
			newPirateInventory = false;
			newTruffleInventory = false;
			newWizardInventory = false;
			newSteampunkerInventory = false;
			newCyborgInventory = false;
			newSkeletonMerchantInventory = false;
			newPermafrostInventory = false;
			newCirrusInventory = false;
			newAmidiasInventory = false;
			newBanditInventory = false;
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
            boost.AddWithCondition("fullHPRespawn", healToFull);

			boost.AddWithCondition("newMerchantInventory", newMerchantInventory);
			boost.AddWithCondition("newPainterInventory", newPainterInventory);
			boost.AddWithCondition("newDyeTraderInventory", newDyeTraderInventory);
			boost.AddWithCondition("newPartyGirlInventory", newPartyGirlInventory);
			boost.AddWithCondition("newStylistInventory", newStylistInventory);
			boost.AddWithCondition("newDemolitionistInventory", newDemolitionistInventory);
			boost.AddWithCondition("newDryadInventory", newDryadInventory);
			boost.AddWithCondition("newTavernkeepInventory", newTavernkeepInventory);
			boost.AddWithCondition("newArmsDealerInventory", newArmsDealerInventory);
			boost.AddWithCondition("newGoblinTinkererInventory", newGoblinTinkererInventory);
			boost.AddWithCondition("newWitchDoctorInventory", newWitchDoctorInventory);
			boost.AddWithCondition("newClothierInventory", newClothierInventory);
			boost.AddWithCondition("newMechanicInventory", newMechanicInventory);
			boost.AddWithCondition("newPirateInventory", newPirateInventory);
			boost.AddWithCondition("newTruffleInventory", newTruffleInventory);
			boost.AddWithCondition("newWizardInventory", newWizardInventory);
			boost.AddWithCondition("newSteampunkerInventory", newSteampunkerInventory);
			boost.AddWithCondition("newCyborgInventory", newCyborgInventory);
			boost.AddWithCondition("newSkeletonMerchantInventory", newSkeletonMerchantInventory);
			boost.AddWithCondition("newPermafrostInventory", newPermafrostInventory);
			boost.AddWithCondition("newCirrusInventory", newCirrusInventory);
			boost.AddWithCondition("newAmidiasInventory", newAmidiasInventory);
			boost.AddWithCondition("newBanditInventory", newBanditInventory);

			return new TagCompound
            {
                { "boost", boost },
                { "stress", rage },
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
                { "deathModeUnderworldTime", deathModeUnderworldTime },
                { "deathModeBlizzardTime", deathModeBlizzardTime }
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
            healToFull = boost.Contains("fullHPRespawn");

			newMerchantInventory = boost.Contains("newMerchantInventory");
			newPainterInventory = boost.Contains("newPainterInventory");
			newDyeTraderInventory = boost.Contains("newDyeTraderInventory");
			newPartyGirlInventory = boost.Contains("newPartyGirlInventory");
			newStylistInventory = boost.Contains("newStylistInventory");
			newDemolitionistInventory = boost.Contains("newDemolitionistInventory");
			newDryadInventory = boost.Contains("newDryadInventory");
			newTavernkeepInventory = boost.Contains("newTavernkeepInventory");
			newArmsDealerInventory = boost.Contains("newArmsDealerInventory");
			newGoblinTinkererInventory = boost.Contains("newGoblinTinkererInventory");
			newWitchDoctorInventory = boost.Contains("newWitchDoctorInventory");
			newClothierInventory = boost.Contains("newClothierInventory");
			newMechanicInventory = boost.Contains("newMechanicInventory");
			newPirateInventory = boost.Contains("newPirateInventory");
			newTruffleInventory = boost.Contains("newTruffleInventory");
			newWizardInventory = boost.Contains("newWizardInventory");
			newSteampunkerInventory = boost.Contains("newSteampunkerInventory");
			newCyborgInventory = boost.Contains("newCyborgInventory");
			newSkeletonMerchantInventory = boost.Contains("newSkeletonMerchantInventory");
			newPermafrostInventory = boost.Contains("newPermafrostInventory");
			newCirrusInventory = boost.Contains("newCirrusInventory");
			newAmidiasInventory = boost.Contains("newAmidiasInventory");
			newBanditInventory = boost.Contains("newBanditInventory");

			rage = tag.GetAsInt("stress");
            adrenaline = tag.GetAsInt("adrenaline");
            sCalDeathCount = tag.GetInt("sCalDeathCount");
            sCalKillCount = tag.GetInt("sCalKillCount");
            deathCount = tag.GetInt("deathCount");

            // These two variables are no longer used, as the code was moved into CalamityWorld.cs to support multiplayer.
            // As a result, their values are simply fed into a discard.

            _ = tag.GetInt("moneyStolenByBandit");
            _ = tag.GetInt("reforges");

            deathModeUnderworldTime = tag.GetInt("deathModeUnderworldTime");
            deathModeBlizzardTime = tag.GetInt("deathModeBlizzardTime");

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
            rage = reader.ReadInt32();
            adrenaline = reader.ReadInt32();
            sCalDeathCount = reader.ReadInt32();
            sCalKillCount = reader.ReadInt32();
            deathCount = reader.ReadInt32();

            // These two variables are no longer used, as the code was moved into CalamityWorld.cs to support multiplayer.
            // As a result, their values are simply fed into a discard.

            _ = reader.ReadInt32(); // moneyStolenByBandit
            _ = reader.ReadInt32(); // reforges

            deathModeUnderworldTime = reader.ReadInt32();
            deathModeBlizzardTime = reader.ReadInt32();

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
                healToFull = flags3[1];
				newMerchantInventory = flags3[2];
				newPainterInventory = flags3[3];
				newDyeTraderInventory = flags3[4];
				newPartyGirlInventory = flags3[5];
				newStylistInventory = flags3[6];
				newDemolitionistInventory = flags3[7];

				BitsByte flags4 = reader.ReadByte();
				newDryadInventory = flags4[0];
				newTavernkeepInventory = flags4[1];
				newArmsDealerInventory = flags4[2];
				newGoblinTinkererInventory = flags4[3];
				newWitchDoctorInventory = flags4[4];
				newClothierInventory = flags4[5];
				newMechanicInventory = flags4[6];
				newPirateInventory = flags4[7];

				BitsByte flags5 = reader.ReadByte();
				newTruffleInventory = flags5[0];
				newWizardInventory = flags5[1];
				newSteampunkerInventory = flags5[2];
				newCyborgInventory = flags5[3];
				newSkeletonMerchantInventory = flags5[4];
				newPermafrostInventory = flags5[5];
				newCirrusInventory = flags5[6];
				newAmidiasInventory = flags5[7];

				BitsByte flags6 = reader.ReadByte();
				newBanditInventory = flags6[0];
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
            // Max health bonuses
            if (absorber)
                player.statLifeMax2 += 20;
            player.statLifeMax2 +=
                (mFruit ? 25 : 0) +
                (bOrange ? 25 : 0) +
                (eBerry ? 25 : 0) +
                (dFruit ? 25 : 0);
            if (ZoneAbyss && abyssalAmulet)
                player.statLifeMax2 += player.statLifeMax2 / 5 / 20 * (lumenousAmulet ? 25 : 10);
            if (coreOfTheBloodGod)
                player.statLifeMax2 += player.statLifeMax2 / 5 / 20 * 10;
            if (bloodPact)
                player.statLifeMax2 += player.statLifeMax2 / 5 / 20 * 100;
            if (leviathanAndSirenLore)
            {
                if (sirenBoobsPrevious)
                    player.statLifeMax2 += player.statLifeMax2 / 5 / 20 * 5;
            }
            if (absoluteRage)
                player.statLifeMax2 += player.statLifeMax / 5 / 20 * 5;
            if (affliction || afflicted)
                player.statLifeMax2 += player.statLifeMax / 5 / 20 * 10;
            if (cadence)
                player.statLifeMax2 += player.statLifeMax / 5 / 20 * 25;
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
                player.statLifeMax2 += player.statLifeMax / 5 / 20 * integerTypeBoost;
            }

            // Max health reductions
            if (crimEffigy)
                player.statLifeMax2 = (int)(player.statLifeMax2 * 0.8);
            if (badgeOfBraveryRare)
                player.statLifeMax2 = (int)(player.statLifeMax2 * 0.75);
            if (regenator)
                player.statLifeMax2 = (int)(player.statLifeMax2 * 0.5);
            if (skeletronLore)
                player.statLifeMax2 = (int)(player.statLifeMax2 * 0.9);
            if (calamitasLore)
                player.statLifeMax2 = (int)(player.statLifeMax2 * 0.75);
            if (providenceLore)
                player.statLifeMax2 = (int)(player.statLifeMax2 * 0.8);

            // Extra accessory slots
			// This is probably fucked in 1.4
            if (extraAccessoryML)
                player.extraAccessorySlots = 1;
            if (extraAccessoryML && player.extraAccessory && (Main.expertMode || Main.gameMenu))
                player.extraAccessorySlots = 2;
            if (CalamityWorld.bossRushActive)
            {
                if (CalamityConfig.Instance.BossRushAccessoryCurse)
                {
                    player.extraAccessorySlots = 0;
                }
            }

            ResetRogueStealth();

			contactDamageReduction = 0D;
			projectileDamageReduction = 0D;

            throwingDamage = 1f;
            throwingVelocity = 1f;
            throwingCrit = 0;
            throwingAmmoCost75 = false;
            throwingAmmoCost66 = false;
            throwingAmmoCost55 = false;
            throwingAmmoCost50 = false;
			accStealthGenBoost = 0f;

			trueMeleeDamage = 0D;

            dashMod = 0;
            externalAbyssLight = 0;
            externalColdImmunity = externalHeatImmunity = false;
            alcoholPoisonLevel = 0;

            thirdSage = false;
            if (player.immuneTime <= 0)
                thirdSageH = false;

            perfmini = false;
            akato = false;
            leviPet = false;
            plaguebringerBab = false;
            rotomPet = false;
            ladShark = false;
            sparks = false;
            sirenPet = false;
            fox = false;
            chibii = false;
            brimling = false;
            bearPet = false;
            kendra = false;
            trashMan = false;
            astrophage = false;
            flakPet = false;
            babyGhostBell = false;
            radiator = false;
            scalPet = false;
            bendyPet = false;
            onyxExcavator = false;
            angryDog = false;
            fab = false;
            crysthamyr = false;
            miniOldDuke = false;

            abyssalDivingSuitPlates = false;
            abyssalDivingSuitCooldown = false;

            sirenWaterBuff = false;
            sirenIce = false;
            sirenIceCooldown = false;

            draedonsHeart = false;

            afflicted = false;
            affliction = false;

            fasterMeleeLevel = false;
            fasterRangedLevel = false;
            fasterMagicLevel = false;
            fasterSummonLevel = false;
            fasterRogueLevel = false;

            dodgeScarf = false;
			evasionScarf = false;
            scarfCooldown = false;
            eScarfCooldown = false;

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
            fearmongerSet = false;

            ataxiaBolt = false;
            ataxiaGeyser = false;
            ataxiaFire = false;
            ataxiaVolley = false;
            ataxiaBlaze = false;
            ataxiaMage = false;

            shadeRegen = false;

            flamethrowerBoost = false;
            hoverboardBoost = false; //hoverboard + shroomite visage

            shadowSpeed = false;
            dsSetBonus = false;
            wearingRogueArmor = false;

            kingSlimeLore = false;
            desertScourgeLore = false;
            crabulonLore = false;
            eaterOfWorldsLore = false;
            hiveMindLore = false;
            perforatorLore = false;
            queenBeeLore = false;
            skeletronLore = false;
            slimeGodLoreProcessed = false;
            wallOfFleshLore = false;
            twinsLore = false;
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
            boomerDukeLore = false;
            ravagerLore = false;
            lunaticCultistLore = false;
            moonLordLore = false;
            providenceLore = false;
            polterghastLore = false;
            DoGLore = false;
            yharonLore = false;
            SCalLore = false;
			oceanLore = false;
			corruptionLore = false;
			crimsonLore = false;
			underworldLore = false;

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
            rOoze = false;
            pAmulet = false;
            fBarrier = false;
            aBrain = false;
            amalgam = false;
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
            nucleogenesis = false;
            nuclearRod = false;
            heartOfDarkness = false;
            shadowMinions = false;
            tearMinions = false;
            alchFlask = false;
            reducedPlagueDmg = false;
			abaddon = false;
            community = false;
            stressPills = false;
            laudanum = false;
            fleshTotem = false;
            fleshTotemCooldown = false;
            bloodPact = false;
            bloodflareCore = false;
            coreOfTheBloodGod = false;
            elementalHeart = false;
            crownJewel = false;
            celestialJewel = false;
            astralArcanum = false;
            harpyRing = false;
			harpyWingBoost = false; //harpy wings + harpy ring
            darkSunRing = false;
            calamityRing = false;
            voidOfExtinction = false;
            eArtifact = false;
            dArtifact = false;
            gArtifact = false;
            pArtifact = false;
            giantPearl = false;
            normalityRelocator = false;
            fabledTortoise = false;
            manaOverloader = false;
            royalGel = false;
            handWarmer = false;
            lol = false;
            raiderTalisman = false;
            gSabaton = false;
            sGenerator = false;
            sDefense = false;
            sRegen = false;
            sPower = false;
            hallowedRune = false;
            hallowedDefense = false;
            hallowedRegen = false;
            hallowedPower = false;
            kamiBoost = false;
            IBoots = false;
            elysianFire = false;
            sTracers = false;
            eTracers = false;
            cTracers = false;
            oldDie = false;
            ursaSergeant = false;
            scuttlersJewel = false;
            thiefsDime = false;
            dynamoStemCells = false;
            etherealExtorter = false;
            dukeScales = false;
            blazingCore = false;
            voltaicJelly = false;
            jellyChargedBattery = false;
            starbusterCore = false;
            starTaintedGenerator = false;
            camper = false;
			silvaWings = false;
			corrosiveSpine = false;
            rustyMedal = false;
            noStupidNaturalARSpawns = false;
            burdenBreakerYeet = false;
			roverDrive = false;

            daedalusReflect = false;
            daedalusSplit = false;
            daedalusAbsorb = false;
            daedalusShard = false;

            brimflameSet = false;
            brimflameFrenzy = false;
            brimflameFrenzyCooldown = false;

            reaverSpore = false;
            reaverDoubleTap = false;
            reaverBlast = false;
            reaverBurst = false;

            ironBoots = false;
            depthCharm = false;
            anechoicPlating = false;
            jellyfishNecklace = false;
			abyssDivingGear = false;
            abyssalAmulet = false;
            lumenousAmulet = false;
            reaperToothNecklace = false;
            aquaticEmblem = false;

            astralStarRain = false;

            desertProwler = false;

            snowRuffianSet = false;

            forbiddenCirclet = false;

            eskimoSet = false; //vanilla armor
            meteorSet = false; //vanilla armor, for Space Gun nerf

            victideSet = false;

            sulfurSet = false;
			sulfurJump = false;

            aeroSet = false;

            statigelSet = false;
			statigelJump = false;

            titanHeartSet = false;
            titanHeartMask = false;
            titanHeartMantle = false;
            titanHeartBoots = false;
            umbraphileSet = false;
            plagueReaper = false;
			plaguebringerPatronSet = false;
			plaguebringerCarapace = false;
			plaguebringerPistons = false;
            fathomSwarmer = false;
            fathomSwarmerVisage = false;
            fathomSwarmerBreastplate = false;
            fathomSwarmerTail = false;
            prismaticSet = false;
            prismaticHelmet = false;
            prismaticRegalia = false;
            prismaticGreaves = false;

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
            sandCloakCooldown = false;
            spectralVeil = false;
            hasJetpack = false;
            plaguedFuelPack = false;
            blunderBooster = false;
            veneratedLocket = false;

            alcoholPoisoning = false;
            shadowflame = false;
            wDeath = false;
            lethalLavaBurn = false;
            aCrunch = false;
            absoluteRage = false;
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
            nightwither = false;
            eFreeze = false;
            silvaStun = false;
            wCleave = false;
            eutrophication = false;
            iCantBreathe = false;
            cragsLava = false;
            vaporfied = false;
			energyShellCooldown = false;
			prismaticCooldown = false;

            revivify = false;
            trinketOfChiBuff = false;
            corrEffigy = false;
            crimEffigy = false;
            decayEffigy = false;
            rRage = false;
            xRage = false;
            xWrath = false;
            graxDefense = false;
            encased = false;
            sMeleeBoost = false;
            eScarfBoost = false;
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
            shadow = false;
            photosynthesis = false;
            astralInjection = false;
            gravityNormalizer = false;
            holyWrath = false;
            profanedRage = false;
            draconicSurge = false;
            draconicSurgeCooldown = false;
            tesla = false;
            teslaFreeze = false;
            sulphurskin = false;
            baguette = false;
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
			bloodPactBoost = false;

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

            wDroid = false;
            resButterfly = false;
            glSword = false;
            mWorm = false;
            iClasper = false;
            magicHat = false;
            herring = false;
            blackhawk = false;
            cosmicViper = false;
            calamari = false;
            cEyes = false;
            cSlime = false;
            cSlime2 = false;
            aSlime = false;
            bStar = false;
            aStar = false;
            SP = false;
            dCreeper = false;
            bClot = false;
            eAxe = false;
            endoCooper = false;
            apexShark = false;
            gastricBelcher = false;
            squirrel = false;
            hauntedDishes = false;
            stormjaw = false;
            SPG = false;
            sirius = false;
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
            plantera = false;
            aProbe = false;
            vUrchin = false;
            cSpirit = false;
            rOrb = false;
            dCrystal = false;
            youngDuke = false;
            sandWaifu = false;
            sandBoobWaifu = false;
            cloudWaifu = false;
            brimstoneWaifu = false;
            sirenWaifu = false;
            allWaifus = false;
            fungalClump = false;
            howlsHeart = false;
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
            endoHydra = false;
            powerfulRaven = false;
            dragonFamily = false;
            providenceStabber = false;
            plaguebringerMK2 = false;
            igneousExaltation = false;
            coldDivinity = false;
            radiantResolution = false;
            virili = false;
            frostBlossom = false;
            cinderBlossom = false;
            belladonaSpirit = false;
            vileFeeder = false;
            scabRipper = false;
            midnightUFO = false;
            plagueEngine = false;
            brimseeker = false;
            necrosteocytesDudes = false;
            gammaHead = false;
            rustyDrone = false;
            tundraFlameBlossom = false;
            starSwallowerPetFroge = false;
            snakeEyes = false;
            poleWarper = false;
            causticDragon = false;
			plaguebringerPatronSummon = false;
			howlTrio = false;
            mountedScanner = false;

            abyssalDivingSuitPrevious = abyssalDivingSuit;
            abyssalDivingSuit = abyssalDivingSuitHide = abyssalDivingSuitForce = abyssalDivingSuitPower = false;

            sirenBoobsPrevious = sirenBoobs;
            sirenBoobs = sirenBoobsHide = sirenBoobsForce = sirenBoobsPower = false;

            profanedCrystalPrevious = profanedCrystal;
            profanedCrystal = profanedCrystalBuffs = profanedCrystalForce = profanedCrystalHide = false;

            snowmanPrevious = snowman;
            snowman = snowmanHide = snowmanForce = snowmanPower = false;

            meldTransformationPrevious = meldTransformation;
            meldTransformation = meldTransformationForce = meldTransformationPower = false;

            rageModeActive = false;
            adrenalineModeActive = false;

            lastProjectileHit = null;
        }
        #endregion

        #region Screen Position Movements
        public override void ModifyScreenPosition()
        {
            if (CalamityWorld.ScreenShakeSpots.Count > 0)
            {
                // Fail-safe to ensure that spots don't last forever.
                Dictionary<int, ScreenShakeSpot> screenShakeSpots = new Dictionary<int, ScreenShakeSpot>();
                List<int> screenShakeUUIDs = CalamityWorld.ScreenShakeSpots.Keys.ToList();
                for (int i = 0; i < CalamityWorld.ScreenShakeSpots.Count; i++)
                {
                    int uuid = screenShakeUUIDs[i];
                    if (Main.projectile[uuid].active)
                    {
                        screenShakeSpots.Add(uuid, CalamityWorld.ScreenShakeSpots[uuid]);
                    }
                }
                CalamityWorld.ScreenShakeSpots = screenShakeSpots;

                foreach (var spot in CalamityWorld.ScreenShakeSpots)
                {
                    float maxPower = Utils.InverseLerp(1300f, 0f, Vector2.Distance(spot.Value.Position, player.Center), true) * spot.Value.ScreenShakePower;
                    Main.screenPosition += Main.rand.NextVector2Circular(maxPower, maxPower);
                }
            }
        }
        #endregion

        #region UpdateDead
        public override void UpdateDead()
        {
            #region Debuffs
            deathModeBlizzardTime = 0;
            deathModeUnderworldTime = 0;
            gaelRageCooldown = 0;
            gaelSwipes = 0;
            gaelSwitchTimer = 0;
            andromedaState = AndromedaPlayerState.Inactive;
            planarSpeedBoost = 0;
            galileoCooldown = 0;
            soundCooldown = 0;
            shadowPotCooldown = 0;
			dogTextCooldown = 0;
            rage = 0;
            adrenaline = 0;
            raiderStack = 0;
            raiderCooldown = 0;
            gSabatonFall = 0;
            gSabatonCooldown = 0;
            astralStarRainCooldown = 0;
            bloodflareMageCooldown = 0;
            tarraMageHealCooldown = 0;
            bossRushImmunityFrameCurseTimer = 0;
            aBulwarkRareMeleeBoostTimer = 0;
            acidRoundMultiplier = 1D;
            externalAbyssLight = 0;
            externalColdImmunity = externalHeatImmunity = false;
            polarisBoostCounter = 0;
            spectralVeilImmunity = 0;
            jetPackCooldown = 0;
            blunderBoosterDash = 0;
            blunderBoosterDirection = 0;
            plaguedFuelPackDash = 0;
            plaguedFuelPackDirection = 0;
            andromedaCripple = 0;
            theBeeCooldown = 0;
            killSpikyBalls = false;
            moonCrownCooldown = 0;
            featherCrownCooldown = 0;
            nanoFlareCooldown = 0;
            fleshTotemCooldown = false;
            sandCloakCooldown = false;
			icicleCooldown = 0;
			statisTimer = 0;
			hallowedRuneCooldown = 0;
			doubledHorror = false;
			sulphurBubbleCooldown = 0;
			ladHearts = 0;
			prismaticLasers = 0;
			roverDriveTimer = 0;

            alcoholPoisoning = false;
            shadowflame = false;
            wDeath = false;
            lethalLavaBurn = false;
            aCrunch = false;
            absoluteRage = false;
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
            eScarfCooldown = false;
            godSlayerCooldown = false;
            abyssalDivingSuitCooldown = false;
            abyssalDivingSuitPlateHits = 0;
            sirenIceCooldown = false;
            inkBombCooldown = false;
            abyssalMirrorCooldown = false;
            eclipseMirrorCooldown = false;
            sulphurPoison = false;
            nightwither = false;
            eFreeze = false;
            silvaStun = false;
            wCleave = false;
            eutrophication = false;
            iCantBreathe = false;
            cragsLava = false;
            vaporfied = false;
			energyShellCooldown = false;
			prismaticCooldown = false;
			#endregion

			#region Rogue
			// Stealth
			rogueStealth = 0f;
            rogueStealthMax = 0f;
            stealthAcceleration = 1f;

            throwingDamage = 1f;
            throwingVelocity = 1f;
            throwingCrit = 0;
            throwingAmmoCost75 = false;
            throwingAmmoCost66 = false;
            throwingAmmoCost55 = false;
            throwingAmmoCost50 = false;
            #endregion

            #region UI
            if (stealthUIAlpha > 0f)
            {
                stealthUIAlpha -= 0.035f;
                stealthUIAlpha = MathHelper.Clamp(stealthUIAlpha, 0f, 1f);
            }
            #endregion

            #region Buffs
            sDefense = false;
            sRegen = false;
            sPower = false;
            hallowedDefense = false;
            hallowedRegen = false;
            hallowedPower = false;
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
            kamiBoost = false;
            graxDefense = false;
            encased = false;
            sMeleeBoost = false;
            eScarfBoost = false;
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
            shadow = false;
            photosynthesis = false;
            astralInjection = false;
            gravityNormalizer = false;
            holyWrath = false;
            profanedRage = false;
            tesla = false;
            teslaFreeze = false;
            sulphurskin = false;
            baguette = false;
            draconicSurge = false;
            draconicSurgeCooldown = false;
            yPower = false;
            aWeapon = false;
            tScale = false;
			titanBoost = 0;
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
            rageModeActive = false;
            adrenalineModeActive = false;
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
            danceOfLightCharge = 0;
			bloodPactBoost = false;
            #endregion

            #region Armorbonuses
            flamethrowerBoost = false;
            hoverboardBoost = false; //hoverboard + shroomite visage
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
            fearmongerSet = false;
            daedalusReflect = false;
            daedalusSplit = false;
            daedalusAbsorb = false;
            daedalusShard = false;
            brimflameSet = false;
            brimflameFrenzy = false;
            brimflameFrenzyCooldown = false;
            reaverSpore = false;
            reaverDoubleTap = false;
            shadeRegen = false;
            dsSetBonus = false;
            titanHeartSet = false;
            titanHeartMask = false;
            titanHeartMantle = false;
            titanHeartBoots = false;
			titanCooldown = 0;
            umbraphileSet = false;
            reaverBlast = false;
            reaverBurst = false;
            fathomSwarmer = false;
            fathomSwarmerVisage = false;
            fathomSwarmerBreastplate = false;
            fathomSwarmerTail = false;
            prismaticSet = false;
            prismaticHelmet = false;
            prismaticRegalia = false;
            prismaticGreaves = false;
            astralStarRain = false;
            plagueReaper = false;
            plagueReaperCooldown = 0;
			plaguebringerPatronSet = false;
			plaguebringerCarapace = false;
			plaguebringerPistons = false;
            pistonsCounter = 0;
            ataxiaMage = false;
            ataxiaBolt = false;
            ataxiaGeyser = false;
            ataxiaFire = false;
            ataxiaVolley = false;
            ataxiaBlaze = false;
            hydrothermalSmoke = false;
            desertProwler = false;
            snowRuffianSet = false;
            forbiddenCirclet = false;
			forbiddenCooldown = 0;
			tornadoCooldown = 0;
            eskimoSet = false; //vanilla armor
            meteorSet = false; //vanilla armor, for Space Gun nerf
            victideSet = false;
            aeroSet = false;
			sulfurSet = false;
			sulfurJump = false;
			jumpAgainSulfur = false;
            statigelSet = false;
			statigelJump = false;
			jumpAgainStatigel = false;
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
            fearmongerSet = false;
            fearmongerRegenFrames = 0;
            xerocSet = false;
            IBoots = false;
            elysianFire = false;
            elysianAegis = false;
            elysianGuard = false;
            #endregion

            CurrentlyViewedFactoryX = CurrentlyViewedFactoryY = -1;
            CurrentlyViewedFactory = null;

            CurrentlyViewedChargerX = CurrentlyViewedChargerY = -1;
            CurrentlyViewedCharger = null;

            CurrentlyViewedHologramX = CurrentlyViewedHologramY = -1;
            CurrentlyViewedHologramText = string.Empty;

            KameiBladeUseDelay = 0;
            lastProjectileHit = null;
			brimlashBusterBoost = false;
			animusBoost = 1f;
			potionTimer = 0;

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
                    for (int doom = 0; doom < Main.maxNPCs; doom++)
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

            bool useCryogen = NPC.AnyNPCs(ModContent.NPCType<Cryogen>());
            if (SkyManager.Instance["CalamityMod:Cryogen"] != null && useCryogen != SkyManager.Instance["CalamityMod:Cryogen"].IsActive())
            {
                if (useCryogen)
                {
                    SkyManager.Instance.Activate("CalamityMod:Cryogen", player.Center);
                }
                else
                {
                    SkyManager.Instance.Deactivate("CalamityMod:Cryogen");
                }
            }

            Point point = player.Center.ToTileCoordinates();
            bool aboveGround = point.Y > Main.maxTilesY - 320;
            bool overworld = player.ZoneOverworldHeight && (point.X < 380 || point.X > Main.maxTilesX - 380);
            bool useFire = NPC.AnyNPCs(ModContent.NPCType<Yharon>());
            player.ManageSpecialBiomeVisuals("CalamityMod:Yharon", useFire);
            player.ManageSpecialBiomeVisuals("HeatDistortion", Main.UseHeatDistortion && (useFire || trippy ||
                aboveGround || (point.Y < Main.worldSurface && player.ZoneDesert && !overworld && !Main.raining && !Filters.Scene["Sandstorm"].IsActive())));

            bool useWater = NPC.AnyNPCs(ModContent.NPCType<Leviathan>());
            player.ManageSpecialBiomeVisuals("CalamityMod:Leviathan", useWater);

            bool useHoly = NPC.AnyNPCs(ModContent.NPCType<Providence>());
            player.ManageSpecialBiomeVisuals("CalamityMod:Providence", useHoly);

            bool useSBrimstone = NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>());
            player.ManageSpecialBiomeVisuals("CalamityMod:SupremeCalamitas", useSBrimstone);

            bool inAstral = ZoneAstral;
            player.ManageSpecialBiomeVisuals("CalamityMod:Astral", inAstral);

            bool cryogenActive = NPC.AnyNPCs(ModContent.NPCType<Cryogen>());

            if (SkyManager.Instance["CalamityMod:Cryogen"] != null && cryogenActive != SkyManager.Instance["CalamityMod:Cryogen"].IsActive())
            {
                if (cryogenActive)
                {
                    SkyManager.Instance.Activate("CalamityMod:Cryogen");
                }
                else
                {
                    SkyManager.Instance.Deactivate("CalamityMod:Cryogen");
                }
            }
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

            ZoneAbyss = point.Y > (Main.rockLayer - y * 0.05) &&
                !player.lavaWet &&
                !player.honeyWet &&
                abyssPosY &&
                abyssPosX;

            ZoneAbyssLayer1 = ZoneAbyss &&
                point.Y <= (Main.rockLayer + y * 0.03);

            ZoneAbyssLayer2 = ZoneAbyss &&
                point.Y > (Main.rockLayer + y * 0.03) &&
                point.Y <= (Main.rockLayer + y * 0.14);

            ZoneAbyssLayer3 = ZoneAbyss &&
                point.Y > (Main.rockLayer + y * 0.14) &&
                point.Y <= (Main.rockLayer + y * 0.26);

            ZoneAbyssLayer4 = ZoneAbyss &&
                point.Y > (Main.rockLayer + y * 0.26);

            ZoneSulphur = (CalamityWorld.sulphurTiles > 30 || (player.ZoneOverworldHeight && sulphurPosX)) && !ZoneAbyss;

			//Overriding 1.4's ass req boosts
			if (Main.snowTiles > 300)
				player.ZoneSnow = true;

			Mod fargos = ModLoader.GetMod("Fargowiltas");
			if (fargos != null)
			{
				//Fargo's fountain effects
				if (Main.fountainColor == CalamityGlobalTile.WaterStyles.FirstOrDefault((style) => style.Name == "SunkenSeaWater").Type)
				{
					ZoneSunkenSea = true;
				}
				if (Main.fountainColor == CalamityGlobalTile.WaterStyles.FirstOrDefault((style) => style.Name == "SulphuricWater").Type)
				{
					ZoneSulphur = true;
				}
				if (Main.fountainColor == CalamityGlobalTile.WaterStyles.FirstOrDefault((style) => style.Name == "AstralWater").Type)
				{
					ZoneAstral = true;
				}
			}
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
            Item createItem(int type)
            {
                Item i = new Item();
                i.SetDefaults(type);
                return i;
            }

            if (!mediumcoreDeath)
            {
                items.Add(createItem(ModContent.ItemType<StarterBag>()));
                items.Add(createItem(ModContent.ItemType<Revenge>()));
                items.Add(createItem(ModContent.ItemType<IronHeart>()));
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
            if (CalamityMod.MomentumCapacitatorHotkey.JustPressed && momentumCapacitor && Main.myPlayer == player.whoAmI && rogueStealth >= rogueStealthMax * 0.3f &&
                wearingRogueArmor && rogueStealthMax > 0 && CalamityUtils.CountProjectiles(ModContent.ProjectileType<MomentumCapacitorOrb>()) == 0)
            {
                rogueStealth -= rogueStealthMax * 0.3f;
                Vector2 fieldSpawnCenter = new Vector2(Main.mouseX, Main.mouseY) + Main.screenPosition;
                Projectile.NewProjectile(fieldSpawnCenter, Vector2.Zero, ModContent.ProjectileType<MomentumCapacitorOrb>(), 0, 0f, player.whoAmI, 0f, 0f);
            }
            if (CalamityMod.NormalityRelocatorHotKey.JustPressed && normalityRelocator && Main.myPlayer == player.whoAmI)
            {
                if (!player.chaosState)
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
                            NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, (float)player.whoAmI, teleportLocation.X, teleportLocation.Y, 1, 0, 0);

							int duration = chaosStateDuration;
							if (areThereAnyDamnBosses || areThereAnyDamnEvents)
								duration = chaosStateDurationBoss;
							if (eScarfCooldown)
								duration = (int)(duration * 1.5);
							else if (scarfCooldown)
								duration *= 2;
							player.AddBuff(BuffID.ChaosState, duration, true);
                        }
                    }
                }
            }
            if (CalamityMod.SandCloakHotkey.JustPressed && sandCloak && Main.myPlayer == player.whoAmI && rogueStealth >= rogueStealthMax * 0.25f &&
                wearingRogueArmor && rogueStealthMax > 0 && !sandCloakCooldown)
            {
                player.AddBuff(ModContent.BuffType<SandCloakCooldown>(), 1800, false); //30 seconds
                rogueStealth -= rogueStealthMax * 0.25f;
                Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<SandCloakVeil>(), 7, 8, player.whoAmI, 0, 0);
                Main.PlaySound(SoundID.Item, player.position, 45);
            }
            if (CalamityMod.SpectralVeilHotKey.JustPressed && spectralVeil && Main.myPlayer == player.whoAmI && rogueStealth >= rogueStealthMax * 0.25f &&
                wearingRogueArmor && rogueStealthMax > 0)
            {
                if (!player.chaosState)
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
                            rogueStealth -= rogueStealthMax * 0.25f;

                            player.Teleport(teleportLocation, 1, 0);
                            NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, (float)player.whoAmI, teleportLocation.X, teleportLocation.Y, 1, 0, 0);

							int duration = chaosStateDuration;
							if (areThereAnyDamnBosses || areThereAnyDamnEvents)
								duration = chaosStateDurationBoss;
							if (eScarfCooldown)
								duration = (int)(duration * 1.5);
							else if (scarfCooldown)
								duration *= 2;
							player.AddBuff(BuffID.ChaosState, duration, true);

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
            }
            if (CalamityMod.PlaguePackHotKey.JustPressed && hasJetpack && Main.myPlayer == player.whoAmI && rogueStealth >= rogueStealthMax * 0.25f &&
                wearingRogueArmor && rogueStealthMax > 0 && jetPackCooldown == 0 && !player.mount.Active)
            {
				if (blunderBooster)
				{
					jetPackCooldown = 90;
					blunderBoosterDash = 15;
					blunderBoosterDirection = player.direction;
					rogueStealth -= rogueStealthMax * 0.25f;
					Main.PlaySound(SoundID.Item66, player.Center);
					Main.PlaySound(SoundID.Item34, player.Center);
				}
				else if (plaguedFuelPack)
				{
					jetPackCooldown = 90;
					plaguedFuelPackDash = 10;
					plaguedFuelPackDirection = player.direction;
					rogueStealth -= rogueStealthMax * 0.25f;
					Main.PlaySound(SoundID.Item66, player.Center);
					Main.PlaySound(SoundID.Item34, player.Center);
				}
            }
            if (CalamityMod.TarraHotKey.JustPressed)
            {
                if (brimflameSet && !brimflameFrenzyCooldown)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        if (brimflameFrenzy)
                        {
                            brimflameFrenzy = false;
                            player.ClearBuff(ModContent.BuffType<BrimflameFrenzyBuff>());
                        }
                        else
                        {
                            brimflameFrenzy = true;
                            player.AddBuff(ModContent.BuffType<BrimflameFrenzyBuff>(), 10 * 60, true);
                            Main.PlaySound(SoundID.Zombie, (int)player.position.X, (int)player.position.Y, 104);
                            for (int num502 = 0; num502 < 36; num502++)
                            {
                                int dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 16f), player.width, player.height - 16, (int)CalamityDusts.Brimstone, 0f, 0f, 0, default, 1f);
                                Main.dust[dust].velocity *= 3f;
                                Main.dust[dust].scale *= 1.15f;
                            }
                            int num226 = 36;
                            for (int num227 = 0; num227 < num226; num227++)
                            {
                                Vector2 vector6 = Vector2.Normalize(player.velocity) * new Vector2((float)player.width / 2f, (float)player.height) * 0.75f;
                                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * MathHelper.TwoPi / (float)num226), default) + player.Center;
                                Vector2 vector7 = vector6 - player.Center;
                                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, (int)CalamityDusts.Brimstone, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default, 1.4f);
                                Main.dust[num228].noGravity = true;
                                Main.dust[num228].noLight = true;
                                Main.dust[num228].velocity = vector7;
                            }
                        }
                    }
                }
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
                    Main.PlaySound(SoundID.Zombie, (int)player.position.X, (int)player.position.Y, 104);
                    for (int num502 = 0; num502 < 64; num502++)
                    {
                        int dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 16f), player.width, player.height - 16, (int)CalamityDusts.Phantoplasm, 0f, 0f, 0, default, 1f);
                        Main.dust[dust].velocity *= 3f;
                        Main.dust[dust].scale *= 1.15f;
                    }
                    int num226 = 36;
                    for (int num227 = 0; num227 < num226; num227++)
                    {
                        Vector2 vector6 = Vector2.Normalize(player.velocity) * new Vector2((float)player.width / 2f, (float)player.height) * 0.75f;
                        vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + player.Center;
                        Vector2 vector7 = vector6 - player.Center;
                        int num228 = Dust.NewDust(vector6 + vector7, 0, 0, (int)CalamityDusts.Phantoplasm, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default, 1.4f);
                        Main.dust[num228].noGravity = true;
                        Main.dust[num228].noLight = true;
                        Main.dust[num228].velocity = vector7;
                    }
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    int damage = (int)(800 * player.RangedDamage());
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (int i = 0; i < 8; i++)
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
                    Main.PlaySound(SoundID.Zombie, (int)player.position.X, (int)player.position.Y, 104);
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
                    Main.PlaySound(SoundID.Zombie, (int)player.position.X, (int)player.position.Y, 104);
                    for (int num502 = 0; num502 < 36; num502++)
                    {
                        int dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 16f), player.width, player.height - 16, (int)CalamityDusts.Brimstone, 0f, 0f, 0, default, 1f);
                        Main.dust[dust].velocity *= 3f;
                        Main.dust[dust].scale *= 1.15f;
                    }
                    int num226 = 36;
                    for (int num227 = 0; num227 < num226; num227++)
                    {
                        Vector2 vector6 = Vector2.Normalize(player.velocity) * new Vector2((float)player.width / 2f, (float)player.height) * 0.75f;
                        vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * MathHelper.TwoPi / (float)num226), default) + player.Center;
                        Vector2 vector7 = vector6 - player.Center;
                        int num228 = Dust.NewDust(vector6 + vector7, 0, 0, (int)CalamityDusts.Brimstone, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default, 1.4f);
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
                        for (int l = 0; l < Main.maxNPCs; l++)
                        {
                            NPC npc = Main.npc[l];
                            if (npc.active && !npc.friendly && !npc.dontTakeDamage && Vector2.Distance(player.Center, npc.Center) <= 3000f)
                            {
                                npc.AddBuff(ModContent.BuffType<Enraged>(), 600, false);
                            }
                        }
                    }
                }
                if (plagueReaper && plagueReaperCooldown <= 0)
                    plagueReaperCooldown = 1800;
				if (forbiddenCirclet && forbiddenCooldown <= 0)
				{
					forbiddenCooldown = 45;
                    int stormMana = (int)(ForbiddenCirclet.manaCost * player.manaCost);
                    if (player.statMana < stormMana)
                    {
                        if (player.manaFlower)
                        {
                            player.QuickMana();
                        }
                    }
                    if (player.statMana >= stormMana && !player.silence)
                    {
                        player.manaRegenDelay = (int)player.maxRegenDelay;
                        player.statMana -= stormMana;
						float dmgMult = player.RogueDamage() + player.minionDamage - 1f;
						int damage = (int)(ForbiddenCirclet.tornadoBaseDmg * dmgMult);
                        if (player.HasBuff(BuffID.ManaSickness))
                        {
                            int sickPenalty = (int)(damage * (0.05f * ((player.buffTime[player.FindBuffIndex(BuffID.ManaSickness)] + 60) / 60)));
                            damage -= sickPenalty;
                        }
						float kBack = ForbiddenCirclet.tornadoBaseKB + player.minionKB;
						if (player.whoAmI == Main.myPlayer)
						{
							Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<CircletMark>(), damage, kBack, player.whoAmI, 0f, 0f);
						}
					}
				}
				if (prismaticSet && !prismaticCooldown && prismaticLasers <= 0)
					prismaticLasers = CalamityUtils.SecondsToFrames(35f);
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
                        Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 6);
                    }
                    else if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
                    {
                        NetMessage.SendData(MessageID.TeleportationPotion, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
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
                if (gaelRageCooldown == 0 && player.ActiveItem().type == ModContent.ItemType<GaelsGreatsword>() &&
                    rage > 0)
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
                    float rageRatio = rage / rageMax;
                    int damage = (int)(rageRatio * GaelsGreatsword.MaxRageBoost * GaelsGreatsword.BaseDamage * player.MeleeDamage());
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
                    rage = 0;
                }
                if (rage == rageMax && CalamityConfig.Instance.Rippers && !rageModeActive)
                {
                    Main.PlaySound(SoundID.Zombie, (int)player.position.X, (int)player.position.Y, 104);
                    for (int num502 = 0; num502 < 64; num502++)
                    {
                        int dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 16f), player.width, player.height - 16, (int)CalamityDusts.Brimstone, 0f, 0f, 0, default, 1f);
                        Main.dust[dust].velocity *= 3f;
                        Main.dust[dust].scale *= 1.15f;
                    }
                    int num226 = 36;
                    for (int num227 = 0; num227 < num226; num227++)
                    {
                        Vector2 vector6 = Vector2.Normalize(player.velocity) * new Vector2((float)player.width / 2f, (float)player.height) * 0.75f;
                        vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + player.Center;
                        Vector2 vector7 = vector6 - player.Center;
                        int num228 = Dust.NewDust(vector6 + vector7, 0, 0, (int)CalamityDusts.Brimstone, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default, 1.4f);
                        Main.dust[num228].noGravity = true;
                        Main.dust[num228].noLight = true;
                        Main.dust[num228].velocity = vector7;
                    }
                    player.AddBuff(ModContent.BuffType<RageMode>(), RageDuration);
                }
            }
            if (CalamityMod.AdrenalineHotKey.JustPressed && CalamityConfig.Instance.Rippers && CalamityWorld.revenge)
            {
                if (adrenaline == adrenalineMax && !adrenalineModeActive)
                {
                    Main.PlaySound(SoundID.Zombie, (int)player.position.X, (int)player.position.Y, 104);
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
                    player.AddBuff(ModContent.BuffType<AdrenalineMode>(), AdrenalineDuration);
                }
            }

			bool canJump = (!player.doubleJumpCloud || !player.jumpAgainCloud) &&
			(!player.doubleJumpSandstorm || !player.jumpAgainSandstorm) &&
			(!player.doubleJumpBlizzard || !player.jumpAgainBlizzard) &&
			(!player.doubleJumpFart || !player.jumpAgainFart) &&
			(!player.doubleJumpSail || !player.jumpAgainSail) &&
			(!player.doubleJumpUnicorn || !player.jumpAgainUnicorn) &&
			CalamityUtils.CountHookProj() <= 0 && (player.rocketTime == 0 || player.wings > 0);
			if (PlayerInput.Triggers.JustPressed.Jump && player.position.Y != player.oldPosition.Y && canJump)
			{
				if (statigelJump && jumpAgainStatigel)
				{
					jumpAgainStatigel = false;
					int offset = player.height;
					if (player.gravDir == -1f)
						offset = 0;
					Main.PlaySound(SoundID.DoubleJump, (int)player.position.X, (int)player.position.Y, 1, 1f, 0f);
					player.velocity.Y = -Player.jumpSpeed * player.gravDir;
					player.jump = (int)(Player.jumpHeight * 1.25);
					for (int d = 0; d < 30; ++d)
					{
						int goo = Dust.NewDust(new Vector2(player.position.X, player.position.Y + offset), player.width, 12, 4, player.velocity.X * 0.3f, player.velocity.Y * 0.3f, 100, new Color(0, 80, 255, 100), 1.5f);
						if (d % 2 == 0)
							Main.dust[goo].velocity.X += (float)Main.rand.Next(30, 71) * 0.1f;
						else
							Main.dust[goo].velocity.X -= (float)Main.rand.Next(30, 71) * 0.1f;
						Main.dust[goo].velocity.Y += (float)Main.rand.Next(-10, 31) * 0.1f;
						Main.dust[goo].noGravity = true;
						Main.dust[goo].scale += (float)Main.rand.Next(-10, 41) * 0.01f;
						Main.dust[goo].velocity *= Main.dust[goo].scale * 0.7f;
					}
				}
				else if (sulfurJump && jumpAgainSulfur)
				{
					jumpAgainSulfur = false;
					int offset = player.height;
					if (player.gravDir == -1f)
						offset = 0;
					Main.PlaySound(SoundID.DoubleJump, (int)player.position.X, (int)player.position.Y, 1, 1f, 0f);
					player.velocity.Y = -Player.jumpSpeed * player.gravDir;
					player.jump = (int)(Player.jumpHeight * 1.5);
					for (int d = 0; d < 30; ++d)
					{
						int sulfur = Dust.NewDust(new Vector2(player.position.X, player.position.Y + offset), player.width, 12, 31, player.velocity.X * 0.3f, player.velocity.Y * 0.3f, 100, default, 1.5f);
						if (d % 2 == 0)
							Main.dust[sulfur].velocity.X += (float)Main.rand.Next(30, 71) * 0.1f;
						else
							Main.dust[sulfur].velocity.X -= (float)Main.rand.Next(30, 71) * 0.1f;
						Main.dust[sulfur].velocity.Y += (float)Main.rand.Next(-10, 31) * 0.1f;
						Main.dust[sulfur].noGravity = true;
						Main.dust[sulfur].scale += (float)Main.rand.Next(-10, 41) * 0.01f;
						Main.dust[sulfur].velocity *= Main.dust[sulfur].scale * 0.7f;
					}
					if (sulphurBubbleCooldown <= 0)
					{
						int bubble = Projectile.NewProjectile(new Vector2(player.position.X, player.position.Y + (player.gravDir == -1f ? 20 : -20)), Vector2.Zero, ModContent.ProjectileType<SulphuricAcidBubbleFriendly>(), (int)(20f * player.RogueDamage()), 0f, player.whoAmI, 1f, 0f);
						Main.projectile[bubble].Calamity().forceRogue = true;
						sulphurBubbleCooldown = 20;
					}
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
                    if (Main.tile[num2, num3] is null)
                    {
                        Main.tile[num2, num3] = new Tile();
                    }
                    int i = 0;
                    while (i < 100)
                    {
                        if (Main.tile[num2, num3 + i] is null)
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
            int teleportStartX = CalamityWorld.abyssSide ? (int)(Main.maxTilesX * 0.65) : (int)(Main.maxTilesX * 0.2);
            int teleportRangeX = (int)(Main.maxTilesX * 0.15);

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
                    if (Main.tile[num2, num3] is null)
                    {
                        Main.tile[num2, num3] = new Tile();
                    }
                    int i = 0;
                    while (i < 100)
                    {
                        if (Main.tile[num2, num3 + i] is null)
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
            for (int index = 0; index < Main.maxProjectiles; ++index)
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
            Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 6);
            if (Main.netMode != NetmodeID.Server)
            {
                return;
            }
            if (syncData)
            {
                RemoteClient.CheckSection(player.whoAmI, player.position, 1);
                NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, (float)player.whoAmI, pos.X, pos.Y, 3, 0, 0);
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
                else if (item.type == ModContent.ItemType<ProfanedSoulCrystal>())
                {
                    profanedCrystalHide = false;
                    profanedCrystalForce = true;
                }
                else if (item.type == ModContent.ItemType<AbyssalDivingGear>())
                {
                    abyssDivingGear = true;
                }
            }
        }

        public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
        {
            if (CalamityConfig.Instance.BossHealthBar)
            {
                drawBossHPBar = true;
            }
            else
            {
                drawBossHPBar = false;
            }
            if (CalamityConfig.Instance.BossHealthBarExtraInfo)
            {
                shouldDrawSmallText = true;
            }
            else
            {
                shouldDrawSmallText = false;
            }

            if (CalamityConfig.Instance.MiningSpeedBoost)
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
            if (eScarfBoost)
            {
                meleeSpeedMult += 0.15f;
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
			if (bloodyWormTooth)
			{
				if (player.statLife < (int)(player.statLifeMax2 * 0.5))
					meleeSpeedMult += 0.1f;
				else
					meleeSpeedMult += 0.05f;
			}
			if (aquaticScourgeLore)
			{
				if (player.wellFed)
					meleeSpeedMult += 0.025f;
				else
					meleeSpeedMult -= 0.025f;
			}
			if (CalamityConfig.Instance.Proficiency)
            {
                meleeSpeedMult += GetMeleeSpeedBonus();
            }
            player.meleeSpeed += meleeSpeedMult;

			if (player.ActiveItem().type == ModContent.ItemType<AstralBlade>() || player.ActiveItem().type == ModContent.ItemType<MantisClaws>() ||
				player.ActiveItem().type == ModContent.ItemType<Omniblade>() || player.ActiveItem().type == ModContent.ItemType<BladeofEnmity>())
			{
				float newMeleeSpeed = 1f + ((player.meleeSpeed - 1f) * 0.25f);
				player.meleeSpeed = newMeleeSpeed;
			}
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
            if (sirenBoobs && NPC.downedBoss3)
            {
                if (player.whoAmI == Main.myPlayer && !sirenIceCooldown)
                {
                    player.AddBuff(ModContent.BuffType<IceShieldBuff>(), 2);
                }
            }
            if (profanedCrystal)
            {
                player.AddBuff(ModContent.BuffType<ProfanedCrystalBuff>(), 60, true);
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

        #region PostUpdateBuffs
        public override void PostUpdateBuffs()
        {
            if (CalamityWorld.defiled)
                Defiled();

            if (weakPetrification)
                WeakPetrification();

            if (player.mount.Active || player.mount.Cart || (CalamityConfig.Instance.BossRushDashCurse && CalamityWorld.bossRushActive))
				DashExploitFix(true);

            if (silvaCountdown > 0 && hasSilvaEffect && silvaSet)
            {
                if (player.lifeRegen < 0)
                    player.lifeRegen = 0;
            }

			if (boomerDukeLore)
				player.buffImmune[ModContent.BuffType<Irradiated>()] = false;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<GiantIbanRobotOfDoom>()] > 0)
                player.yoraiz0rEye = 0;
        }
        #endregion

        #region PostUpdateEquips
        public override void PostUpdateEquips()
        {
            if (CalamityWorld.defiled)
                Defiled();

            if (weakPetrification)
                WeakPetrification();

			if (player.mount.Active || player.mount.Cart || (CalamityConfig.Instance.BossRushDashCurse && CalamityWorld.bossRushActive))
				DashExploitFix(true);

			if (silvaCountdown > 0 && hasSilvaEffect && silvaSet)
            {
                if (player.lifeRegen < 0)
                    player.lifeRegen = 0;
            }

			if (boomerDukeLore)
				player.buffImmune[ModContent.BuffType<Irradiated>()] = false;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<GiantIbanRobotOfDoom>()] > 0)
                player.yoraiz0rEye = 0;
        }
		#endregion

		#region Dash Exploit Fix
		private void DashExploitFix(bool mount)
		{
			if (player.dashDelay != 0)
			{
				player.velocity *= 0.3f;
				player.dashDelay = 0;
			}

			if (!mount)
				return;

			player.dash = 0;
			dashMod = 0;
		}
		#endregion

		#region PostUpdate

		public override void PostUpdateMiscEffects()
        {
            CalamityPlayerMiscEffects.CalamityPostUpdateMiscEffects(player, mod);

            if (player.ActiveItem().type == ModContent.ItemType<GaelsGreatsword>())
            {
                gaelSwitchTimer = GaelSwitchPhase.LoseRage;
                rage += (int)MathHelper.Min(5, 10000 - rage);
            }
            else if (player.ActiveItem().type != ModContent.ItemType<GaelsGreatsword>() && gaelSwitchTimer == GaelSwitchPhase.LoseRage)
            {
                rage = 0;
                gaelSwitchTimer = GaelSwitchPhase.None;
            }

            // Disable the factory UI if the player is far from the associated factory.
            if (CurrentlyViewedFactory != null)
            {
                Vector2 factoryPosition = new Vector2(CurrentlyViewedFactoryX, CurrentlyViewedFactoryY);
                if (player.Distance(factoryPosition) > 1200f)
                {
                    CurrentlyViewedFactory = null;
                    CurrentlyViewedFactoryX = CurrentlyViewedFactoryY = -1;
                }
            }

            // Disable the charger UI if the player is far from the associated charger.
            if (CurrentlyViewedCharger != null)
            {
                Vector2 chargerPosition = new Vector2(CurrentlyViewedChargerX, CurrentlyViewedChargerY);
                if (player.Distance(chargerPosition) > 1200f)
                {
                    CurrentlyViewedCharger = null;
                    CurrentlyViewedChargerX = CurrentlyViewedChargerY = -1;
                }
            }

            // Disable the hologram UI if the player is far from the associated hologram.
            Vector2 hologramPosition = new Vector2(CurrentlyViewedHologramX, CurrentlyViewedHologramY) * 16f;
            if (player.Distance(hologramPosition) > 120f)
            {
                CurrentlyViewedHologramX = CurrentlyViewedHologramY = -1;
                CurrentlyViewedHologramText = string.Empty;
            }
        }

        #region Dragon Scale Logic
        public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (item.type == ModContent.ItemType<DragonScales>() && !CalamityWorld.dragonScalesBought)
            {
                CalamityWorld.dragonScalesBought = true;
            }
        }
        #endregion

        #region Shop Restrictions

        public override bool CanBuyItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (item.type == ModContent.ItemType<DragonScales>())
            {
                return !CalamityWorld.dragonScalesBought;
            }
            return base.CanBuyItem(vendor, shopInventory, item);
        }

        public override bool CanSellItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (item.type == ModContent.ItemType<ProfanedSoulCrystal>())
                return CalamityWorld.downedSCal; //no easy moneycoins for post doggo/yhar
            return base.CanSellItem(vendor, shopInventory, item);
        }

        #endregion

        public override void PostUpdateRunSpeeds()
        {
            #region SpeedBoosts
            float runAccMult = 1f +
                (shadowSpeed ? 0.5f : 0f) +
                (stressPills ? 0.05f : 0f) +
                (laudanum && horror ? 0.1f : 0f) +
                ((abyssalDivingSuit && player.IsUnderwater()) ? 0.05f : 0f) +
                (sirenWaterBuff ? 0.15f : 0f) +
                ((frostFlare && player.statLife < (int)(player.statLifeMax2 * 0.25)) ? 0.15f : 0f) +
                (auricSet ? 0.1f : 0f) +
                (dragonScales ? 0.1f : 0f) +
                (kamiBoost ? KamiBuff.RunAccelerationBoost : 0f) +
                (cTracers ? 0.1f : 0f) +
                (silvaSet ? 0.05f : 0f) +
                (eTracers ? 0.05f : 0f) +
                (blueCandle ? 0.05f : 0f) +
                (etherealExtorter && player.ZoneBeach ? 0.05f : 0f) +
                (planarSpeedBoost > 0 ? (0.01f * planarSpeedBoost) : 0f) +
                ((deepDiver && player.IsUnderwater()) ? 0.15f : 0f) +
                (rogueStealthMax > 0f ? (rogueStealth >= rogueStealthMax ? rogueStealth * 0.05f : rogueStealth * 0.025f) : 0f);

            float runSpeedMult = 1f +
                (shadowSpeed ? 0.5f : 0f) +
                ((abyssalDivingSuit && player.IsUnderwater()) ? 0.05f : 0f) +
                ((frostFlare && player.statLife < (int)(player.statLifeMax2 * 0.25)) ? 0.15f : 0f) +
                (sirenWaterBuff ? 0.15f : 0f) +
                (auricSet ? 0.1f : 0f) +
                (dragonScales ? 0.1f : 0f) +
                (cTracers ? 0.1f : 0f) +
                (silvaSet ? 0.05f : 0f) +
                (eTracers ? 0.05f : 0f) +
                (kamiBoost ? KamiBuff.RunSpeedBoost : 0f) +
                (etherealExtorter && player.ZoneBeach ? 0.05f : 0f) +
                (stressPills ? 0.05f : 0f) +
                (laudanum && horror ? 0.1f : 0f) +
                (planarSpeedBoost > 0 ? (0.01f * planarSpeedBoost) : 0f) +
                ((deepDiver && player.IsUnderwater()) ? 0.15f : 0f) +
                (rogueStealthMax > 0f ? (rogueStealth >= rogueStealthMax ? rogueStealth * 0.05f : rogueStealth * 0.025f) : 0f);

            if (destroyerLore)
            {
                runAccMult *= 0.95f;
            }
            if (twinsLore)
            {
                if (player.statLife < (int)(player.statLifeMax2 * 0.5))
                    runAccMult *= 0.95f;
            }
            if (skeletronPrimeLore)
            {
                runAccMult *= 0.95f;
            }
            if (abyssalDivingSuit && !player.IsUnderwater())
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
			if (CalamityWorld.revenge)
			{
				if (player.powerrun)
				{
					runSpeedMult *= 0.6666667f;
				}
				if ((player.slippy || player.slippy2) && player.iceSkate)
				{
					runAccMult *= 0.6666667f;
				}
			}
			if (CalamityWorld.death && deathModeBlizzardTime > 0)
            {
                float speedMult = (3600 - deathModeBlizzardTime) / 3600f;
                runAccMult *= speedMult;
                runSpeedMult *= speedMult;
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
                    ModDashMovement();

				if (pAmulet && modStealth < 1f)
                {
                    float num43 = player.maxRunSpeed / 2f * (1f - modStealth);
                    player.maxRunSpeed -= num43;
                    player.accRunSpeed = player.maxRunSpeed;
                }
            }
            #endregion
        }
        #endregion

        #region Rogue Mirrors
        public void AbyssMirrorEvade()
        {
            if (player.whoAmI == Main.myPlayer && abyssalMirror && !abyssalMirrorCooldown && !eclipseMirror)
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

                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SilvaActivation"), player.Center);

                for (int i = 0; i < 10; i++)
                {
                    int lumenyl = Projectile.NewProjectile(player.Center.X, player.Center.Y, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), ModContent.ProjectileType<AbyssalMirrorProjectile>(), (int)(55 * player.RogueDamage()), 0, player.whoAmI);
                    Main.projectile[lumenyl].rotation = Main.rand.NextFloat(0, 360);
                    Main.projectile[lumenyl].frame = Main.rand.Next(0, 4);
                }

                if (player.whoAmI == Main.myPlayer)
                {
                    NetMessage.SendData(MessageID.Dodge, -1, -1, null, player.whoAmI, 1f, 0f, 0f, 0, 0, 0);
                }
            }
        }

        public void EclipseMirrorEvade()
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

                Main.PlaySound(SoundID.Item68, player.Center);
                Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<EclipseMirrorBurst>(), (int)(7000 * player.RogueDamage()), 0, player.whoAmI);

                if (player.whoAmI == Main.myPlayer)
                {
                    NetMessage.SendData(MessageID.Dodge, -1, -1, null, player.whoAmI, 1f, 0f, 0f, 0, 0, 0);
                }
            }
        }
        #endregion

        #region Pre Kill
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            PopupGUIManager.SuspendAll();
            if (player.Calamity().andromedaState == AndromedaPlayerState.LargeRobot)
            {
                if (!Main.dedServ)
                {
                    for (int i = 0; i < 40; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(player.Center + Utils.NextVector2Circular(Main.rand, 60f, 90f), 133);
                        dust.velocity = Utils.NextVector2Circular(Main.rand, 4f, 4f);
                        dust.noGravity = true;
                        dust.scale = Main.rand.NextFloat(1.2f, 1.35f);
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        Utils.PoofOfSmoke(player.Center + Utils.NextVector2Circular(Main.rand, 20f, 30f));
                    }
                }
            }
            if (invincible && player.ActiveItem().type != ModContent.ItemType<ColdheartIcicle>())
            {
                if (player.statLife <= 0)
                {
                    player.statLife = 1;
                }
                return false;
            }
            if (hInferno)
            {
                for (int x = 0; x < Main.maxNPCs; x++)
                {
                    if (Main.npc[x].active && Main.npc[x].type == ModContent.NPCType<Providence>())
                    {
                        Main.npc[x].active = false;
                    }
                }
            }
            if (nCore && Main.rand.NextBool(10))
            {
                Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 67);
                for (int j = 0; j < 25; j++)
                {
                    int num = Dust.NewDust(player.position, player.width, player.height, 173, 0f, 0f, 100, default, 2f);
                    Dust dust = Main.dust[num];
                    dust.position.X += (float)Main.rand.Next(-20, 21);
                    dust.position.Y += (float)Main.rand.Next(-20, 21);
                    dust.velocity *= 0.9f;
                    dust.scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
                    if (Main.rand.NextBool(2))
                    {
                        dust.scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
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
                Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 67);
                for (int j = 0; j < 50; j++)
                {
                    int num = Dust.NewDust(player.position, player.width, player.height, 173, 0f, 0f, 100, default, 2f);
                    Dust dust = Main.dust[num];
                    dust.position.X += (float)Main.rand.Next(-20, 21);
                    dust.position.Y += (float)Main.rand.Next(-20, 21);
                    dust.velocity *= 0.9f;
                    dust.scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
                    if (Main.rand.NextBool(2))
                    {
                        dust.scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                    }
                }
                int heal = draconicSurge && !draconicSurgeCooldown ? (player.statLifeMax2 / 2) : 150;
                player.statLife += heal;
                player.HealEffect(heal);
                if (player.statLife > player.statLifeMax2)
                {
                    player.statLife = player.statLifeMax2;
                }
                if (player.FindBuffIndex(ModContent.BuffType<DraconicSurgeBuff>()) > -1)
                {
                    player.ClearBuff(ModContent.BuffType<DraconicSurgeBuff>());
                    player.AddBuff(ModContent.BuffType<DraconicSurgeCooldown>(), CalamityUtils.SecondsToFrames(60f));

					// Additional potion sickness time
					int additionalTime = 0;
					for (int i = 0; i < Player.MaxBuffs; i++)
					{
						if (player.buffType[i] == BuffID.PotionSickness)
							additionalTime = player.buffTime[i];
					}
					float potionSicknessTime = 30f + (float)Math.Ceiling(additionalTime / 60D);
					player.AddBuff(BuffID.PotionSickness, CalamityUtils.SecondsToFrames(potionSicknessTime));
				}
                player.AddBuff(ModContent.BuffType<GodSlayerCooldown>(), CalamityUtils.SecondsToFrames(45f));
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
                    player.AddBuff(ModContent.BuffType<SilvaRevival>(), auricSet ? 300 : 600);
                    if (draconicSurge && !draconicSurgeCooldown)
                    {
                        player.statLife += player.statLifeMax2 / 2;
                        player.HealEffect(player.statLifeMax2 / 2);
                        if (player.statLife > player.statLifeMax2)
                        {
                            player.statLife = player.statLifeMax2;
                        }
                        if (player.FindBuffIndex(ModContent.BuffType<DraconicSurgeBuff>()) > -1)
                        {
                            player.ClearBuff(ModContent.BuffType<DraconicSurgeBuff>());
                            player.AddBuff(ModContent.BuffType<DraconicSurgeCooldown>(), CalamityUtils.SecondsToFrames(60f));

							// Additional potion sickness time
							int additionalTime = 0;
							for (int i = 0; i < Player.MaxBuffs; i++)
							{
								if (player.buffType[i] == BuffID.PotionSickness)
									additionalTime = player.buffTime[i];
							}
							float potionSicknessTime = 30f + (float)Math.Ceiling(additionalTime / 60D);
							player.AddBuff(BuffID.PotionSickness, CalamityUtils.SecondsToFrames(potionSicknessTime));
						}
                    }
					else if (silvaWings)
					{
                        player.statLife += player.statLifeMax2 / 2;
                        player.HealEffect(player.statLifeMax2 / 2);
                        if (player.statLife > player.statLifeMax2)
                        {
                            player.statLife = player.statLifeMax2;
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
                player.AddBuff(ModContent.BuffType<ConcoctionCooldown>(), CalamityUtils.SecondsToFrames(180f));
                player.AddBuff(ModContent.BuffType<Encased>(), CalamityUtils.SecondsToFrames(3f));
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
            if (nightwither && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + " was incinerated by lunar rays.");
            }
            if (vaporfied && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + " vaporized into thin air.");
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
            if (profanedCrystalBuffs && !profanedCrystalHide)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + " was summoned too soon.");
            }

            if (NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>()))
            {
                if (sCalDeathCount < 51)
                {
                    sCalDeathCount++;
                }
            }

			if (CalamityWorld.ironHeart)
			{
				KillPlayer();
				return false;
			}

			deathCount++;
            if (player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
            {
                DeathPacket(false);
            }

            return true;
        }
        #endregion

        #region On Respawn
        public override void OnRespawn(Player player)
        {
            thirdSageH = true;

            // The player rotation can be off if the player dies at the right time when using Final Dawn.
            player.fullRotation = 0f;
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
                if (player.statLife > (int)(player.statLifeMax2 * 0.5) &&
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

		#region Get Heal Life
		public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
		{
			if (bloodPactBoost)
				healValue = (int)(healValue * 1.5);
			if (CalamityWorld.ironHeart)
				healValue = 0;
		}
		#endregion

		#region Get Weapon Damage And KB
		public override void ModifyWeaponDamage(Item item, ref float add, ref float mult, ref float flat)
        {
            if (item.type == ModContent.ItemType<GaelsGreatsword>())
            {
                add += GaelsGreatsword.BaseDamage / (float)GaelsGreatsword.BaseDamage - 1f;
            }
            if (flamethrowerBoost && item.ranged && (item.useAmmo == 23 || CalamityMod.flamethrowerList.Contains(item.type)))
            {
                add += hoverboardBoost ? 0.35f : 0.25f;
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
            if (eskimoSet && CalamityMod.iceWeaponList.Contains(item.type))
            {
                add += 0.1f;
            }
            if (etherealExtorter && player.ZoneDungeon && item.Calamity().rogue && !item.consumable)
            {
                add += 0.05f;
            }

            if (item.ranged)
            {
                acidRoundMultiplier = item.useTime / 20D;
            }
            else
            {
                acidRoundMultiplier = 1D;
            }
			//Prismatic Breaker is a weird hybrid melee-ranged weapon so include it too.  Why are you using desert prowler post-Yharon? don't ask me
			if (desertProwler && (item.ranged || item.type == ModContent.ItemType<PrismaticBreaker>()) && item.ammo == AmmoID.None)
			{
				flat += 1f;
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
            if (titanHeartMask && item.Calamity().rogue)
            {
                knockback *= 1.05f;
            }
            if (titanHeartMantle && item.Calamity().rogue)
            {
                knockback *= 1.05f;
            }
            if (titanHeartBoots && item.Calamity().rogue)
            {
                knockback *= 1.05f;
            }
            if (titanHeartSet && item.Calamity().rogue)
            {
                knockback *= 1.2f;
            }
            if (titanHeartSet && StealthStrikeAvailable() && item.Calamity().rogue)
            {
                knockback *= 2f;
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

        #region Modify Mana Cost
        public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
        {
            if (item.type == ItemID.SpaceGun && meteorSet)
            {
                mult *= 0.5f;
            }
        }
        #endregion

        #region Melee Effects
        public override void MeleeEffects(Item item, Rectangle hitbox)
        {
            if (!item.melee && !item.noMelee && (!item.noUseGraphic && player.meleeEnchant > 0))
            {
                if (player.meleeEnchant == 7)
                {
                    if (Main.rand.NextBool(20))
                    {
                        int confettiDust = Main.rand.Next(139, 143);
                        int confetti = Dust.NewDust(new Vector2(hitbox.X,hitbox.Y), hitbox.Width, hitbox.Height, confettiDust, player.velocity.X, player.velocity.Y, 0, new Color(), 1.2f);
                        Main.dust[confetti].velocity.X *= (float)(1.0 + Main.rand.Next(-50, 51) * 0.01);
                        Main.dust[confetti].velocity.Y *= (float)(1.0 + Main.rand.Next(-50, 51) * 0.01);
                        Main.dust[confetti].velocity.X += Main.rand.Next(-50, 51) * 0.05f;
                        Main.dust[confetti].velocity.Y += Main.rand.Next(-50, 51) * 0.05f;
                        Main.dust[confetti].scale *= (float)(1.0 + Main.rand.Next(-30, 31) * 0.01);
                    }
                    if (Main.rand.NextBool(40))
                    {
                        int confettiGore = Main.rand.Next(276, 283);
                        int confetti = Gore.NewGore(new Vector2(hitbox.X, hitbox.Y), player.velocity, confettiGore, 1f);
                        Main.gore[confetti].velocity.X *= (float)(1.0 + Main.rand.Next(-50, 51) * 0.01);
                        Main.gore[confetti].velocity.Y *= (float)(1.0 + Main.rand.Next(-50, 51) * 0.01);
                        Main.gore[confetti].scale *= (float)(1.0 + Main.rand.Next(-20, 21) * 0.01);
                        Main.gore[confetti].velocity.X += Main.rand.Next(-50, 51) * 0.05f;
                        Main.gore[confetti].velocity.Y += Main.rand.Next(-50, 51) * 0.05f;
                    }
                }
            }
            if (item.melee)
            {
                if (fungalSymbiote && player.whoAmI == Main.myPlayer)
                {
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.1) ||
                        player.itemAnimation == (int)(player.itemAnimationMax * 0.3) ||
                        player.itemAnimation == (int)(player.itemAnimationMax * 0.5) ||
                        player.itemAnimation == (int)(player.itemAnimationMax * 0.7) ||
                        player.itemAnimation == (int)(player.itemAnimationMax * 0.9))
                    {
                        float yVel = 0f;
                        float xVel = 0f;
                        float yOffset = 0f;
                        float xOffset = 0f;
                        if (player.itemAnimation == (int)(player.itemAnimationMax * 0.9))
                        {
                            yVel = -7f;
                        }
                        if (player.itemAnimation == (int)(player.itemAnimationMax * 0.7))
                        {
                            yVel = -6f;
                            xVel = 2f;
                        }
                        if (player.itemAnimation == (int)(player.itemAnimationMax * 0.5))
                        {
                            yVel = -4f;
                            xVel = 4f;
                        }
                        if (player.itemAnimation == (int)(player.itemAnimationMax * 0.3))
                        {
                            yVel = -2f;
                            xVel = 6f;
                        }
                        if (player.itemAnimation == (int)(player.itemAnimationMax * 0.1))
                        {
                            xVel = 7f;
                        }
                        if (player.itemAnimation == (int)(player.itemAnimationMax * 0.7))
                        {
                            xOffset = 26f;
                        }
                        if (player.itemAnimation == (int)(player.itemAnimationMax * 0.3))
                        {
                            xOffset -= 4f;
                            yOffset -= 20f;
                        }
                        if (player.itemAnimation == (int)(player.itemAnimationMax * 0.1))
                        {
                            yOffset += 6f;
                        }
                        if (player.direction == -1)
                        {
                            if (player.itemAnimation == (int)(player.itemAnimationMax * 0.9))
                            {
                                xOffset -= 8f;
                            }
                            if (player.itemAnimation == (int)(player.itemAnimationMax * 0.7))
                            {
                                xOffset -= 6f;
                            }
                        }
                        yVel *= 1.5f;
                        xVel *= 1.5f;
                        xOffset *= (float)player.direction;
                        yOffset *= player.gravDir;
                        Projectile.NewProjectile((float)(hitbox.X + hitbox.Width / 2) + xOffset, (float)(hitbox.Y + hitbox.Height / 2) + yOffset, (float)player.direction * xVel, yVel * player.gravDir, ProjectileID.Mushroom, (int)(item.damage * 0.25f * player.MeleeDamage()), 0f, player.whoAmI, 0f, 0f);
                    }
                }
                if (aWeapon)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<BrimstoneFlame>(), player.velocity.X * 0.2f + player.direction * 3f, player.velocity.Y * 0.2f, 100, default, 0.75f);
                    }
                }
                if (eGauntlet)
                {
                    if (Main.rand.NextBool(3))
                    {
                        int element = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 66, player.velocity.X * 0.2f + player.direction * 3f, player.velocity.Y * 0.2f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.25f);
                        Main.dust[element].noGravity = true;
                    }
                }
                if (cryogenSoul)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 67, player.velocity.X * 0.2f + player.direction * 3f, player.velocity.Y * 0.2f, 100, default, 0.75f);
                    }
                }
                if (xerocSet)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 58, player.velocity.X * 0.2f + player.direction * 3f, player.velocity.Y * 0.2f, 100, default, 1.25f);
                    }
                }
                if (reaverBlast)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 74, player.velocity.X * 0.2f + player.direction * 3f, player.velocity.Y * 0.2f, 100, default, 0.75f);
                    }
                }
                if (dsSetBonus)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 27, player.velocity.X * 0.2f + player.direction * 3f, player.velocity.Y * 0.2f, 100, default, 2.5f);
                    }
                }
            }
        }
        #endregion

        #region On Hit NPC
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
			if (desertProwler && item.ranged && crit) //for obscure stuff like marnite bayonet
			{
				if (player.ownedProjectileCounts[ModContent.ProjectileType<DesertMark>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<DesertTornado>()] < 1)
				{
					if (Main.rand.NextBool(15))
					{
						if (player.whoAmI == Main.myPlayer)
						{
							Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<DesertMark>(), (int)(item.damage * player.RangedDamage()), knockback, player.whoAmI, 0f, 0f);
						}
					}
				}
			}
            if (!item.melee && player.meleeEnchant == 7)
                Projectile.NewProjectile(target.Center, target.velocity, ProjectileID.ConfettiMelee, 0, 0f, player.whoAmI, 0f, 0f);

            if (omegaBlueChestplate)
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);
            if (sulfurSet)
                target.AddBuff(BuffID.Poisoned, 120);
            switch (item.type)
            {
                case ItemID.IceSickle:
                case ItemID.Frostbrand:
                    target.AddBuff(BuffID.Frostburn, 600);
                    break;

                case ItemID.IceBlade:
                    if (Main.rand.NextBool(5))
                        target.AddBuff(BuffID.Frostburn, 360);
                    else if (Main.rand.NextBool(3))
                        target.AddBuff(BuffID.Frostburn, 120);
                    break;
            }

            if (item.melee) //prevents Deep Sea Dumbell from snagging true melee debuff memes
            {
				titanBoost = 600;
                if (eGauntlet)
                {
					int duration = 90;
                    target.AddBuff(BuffID.CursedInferno, duration / 2, false);
                    target.AddBuff(BuffID.Frostburn, duration, false);
                    target.AddBuff(BuffID.Ichor, duration, false);
                    target.AddBuff(BuffID.Venom, duration, false);
                    target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), duration, false);
                    target.AddBuff(ModContent.BuffType<AbyssalFlames>(), duration, false);
                    target.AddBuff(ModContent.BuffType<HolyFlames>(), duration, false);
                    target.AddBuff(ModContent.BuffType<Plague>(), duration, false);
                    target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), duration, false);
                    if (Main.rand.NextBool(5))
                    {
                        target.AddBuff(ModContent.BuffType<GlacialState>(), duration, false);
                    }
                }
                if (cryogenSoul || frostFlare)
                {
					CalamityUtils.Inflict246DebuffsNPC(target, BuffID.Frostburn);
                }
                if (yInsignia)
                {
					CalamityUtils.Inflict246DebuffsNPC(target, ModContent.BuffType<HolyFlames>());
                }
                if (ataxiaFire)
                {
					CalamityUtils.Inflict246DebuffsNPC(target, BuffID.OnFire, 4f);
                }
				if (aWeapon)
				{
					CalamityUtils.Inflict246DebuffsNPC(target, ModContent.BuffType<AbyssalFlames>());
				}
            }
            if (abyssalAmulet)
            {
				CalamityUtils.Inflict246DebuffsNPC(target, ModContent.BuffType<CrushDepth>());
            }
            if (dsSetBonus)
            {
				CalamityUtils.Inflict246DebuffsNPC(target, ModContent.BuffType<DemonFlames>());
            }
            if (alchFlask)
            {
				CalamityUtils.Inflict246DebuffsNPC(target, ModContent.BuffType<Plague>());
            }
            if (armorCrumbling || armorShattering)
            {
                if (item.melee || item.Calamity().rogue)
                {
					CalamityUtils.Inflict246DebuffsNPC(target, ModContent.BuffType<ArmorCrunch>());
                }
            }
            if (item.Calamity().rogue)
            {
				switch (player.meleeEnchant)
				{
					case 1:
						target.AddBuff(BuffID.Venom, 60 * Main.rand.Next(5, 10), false);
						break;
					case 2:
						target.AddBuff(BuffID.CursedInferno, 60 * Main.rand.Next(3, 7), false);
						break;
					case 3:
						target.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(3, 7), false);
						break;
					case 5:
						target.AddBuff(BuffID.Ichor, 60 * Main.rand.Next(10, 20), false);
						break;
					case 6:
						target.AddBuff(BuffID.Confused, 60 * Main.rand.Next(1, 4), false);
						break;
					case 8:
						target.AddBuff(BuffID.Poisoned, 60 * Main.rand.Next(5, 10), false);
						break;
					case 4:
						target.AddBuff(BuffID.Midas, 120, false);
						break;
				}
				if (titanHeartMask)
				{
					target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 60 * Main.rand.Next(1,6), false); // 1 to 5 seconds
				}
            }
            if (holyWrath)
            {
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 600, false);
            }
            if (vexation)
            {
                if ((player.armor[0].type == ModContent.ItemType<ReaverCap>() || player.armor[0].type == ModContent.ItemType<ReaverHelm>() ||
                    player.armor[0].type == ModContent.ItemType<ReaverHelmet>() || player.armor[0].type == ModContent.ItemType<ReaverMask>() ||
                    player.armor[0].type == ModContent.ItemType<ReaverVisage>()) &&
                    player.armor[1].type == ModContent.ItemType<ReaverScaleMail>() && player.armor[2].type == ModContent.ItemType<ReaverCuisses>())
                {
                    target.AddBuff(BuffID.CursedInferno, 90, false);
                    target.AddBuff(BuffID.Venom, 120, false);
                }
            }
        }
        #endregion

        #region On Hit NPC With Proj
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
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

                case ProjectileID.Blizzard:
                case ProjectileID.NorthPoleSnowflake:
                    target.AddBuff(BuffID.Frostburn, 240);
                    break;

                case ProjectileID.SnowBallFriendly:
                    if (Main.rand.NextBool(10))
                        target.AddBuff(BuffID.Frostburn, 120);
                    else if (Main.rand.NextBool(5))
                        target.AddBuff(BuffID.Frostburn, 60);
                    break;

                case ProjectileID.IceBoomerang:
                case ProjectileID.IceBolt:
                case ProjectileID.FrostDaggerfish:
                    if (Main.rand.NextBool(5))
                        target.AddBuff(BuffID.Frostburn, 240);
                    else if (Main.rand.NextBool(3))
                        target.AddBuff(BuffID.Frostburn, 120);
                    break;
            }

            if (!proj.npcProj && !proj.trap)
            {
				if (desertProwler && proj.ranged && crit)
				{
					if (player.ownedProjectileCounts[ModContent.ProjectileType<DesertMark>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<DesertTornado>()] < 1)
					{
						if (Main.rand.NextBool(15))
						{
							if (player.whoAmI == Main.myPlayer)
							{
								Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<DesertMark>(), proj.damage, proj.knockBack, player.whoAmI, 0f, 0f);
							}
						}
					}
				}

				if (proj.Calamity().trueMelee)
					titanBoost = 600;

                if (sulfurSet && proj.friendly && !target.friendly)
                    target.AddBuff(BuffID.Poisoned, 120);

                if (omegaBlueChestplate && proj.friendly && !target.friendly)
                    target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);

                if (proj.melee && silvaMelee && Main.rand.NextBool(4))
                    target.AddBuff(ModContent.BuffType<SilvaStun>(), 20);

                if (abyssalAmulet)
                {
					CalamityUtils.Inflict246DebuffsNPC(target, ModContent.BuffType<CrushDepth>());
                }
                if (dsSetBonus)
                {
					CalamityUtils.Inflict246DebuffsNPC(target, ModContent.BuffType<DemonFlames>());
                }
                if ((plaguebringerCarapace || uberBees) && CalamityMod.friendlyBeeList.Contains(proj.type))
                {
                    target.AddBuff(ModContent.BuffType<Plague>(), 360);
                }
                else if (alchFlask)
                {
					CalamityUtils.Inflict246DebuffsNPC(target, ModContent.BuffType<Plague>());
                }
                if (proj.melee)
                {
                    if (eGauntlet)
                    {
						int duration = 90;
                        target.AddBuff(BuffID.CursedInferno, duration / 2, false);
                        target.AddBuff(BuffID.Frostburn, duration, false);
                        target.AddBuff(BuffID.Ichor, duration, false);
                        target.AddBuff(BuffID.Venom, duration, false);
                        target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), duration, false);
                        target.AddBuff(ModContent.BuffType<AbyssalFlames>(), duration, false);
                        target.AddBuff(ModContent.BuffType<HolyFlames>(), duration, false);
                        target.AddBuff(ModContent.BuffType<Plague>(), duration, false);
                        target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), duration, false);
                        if (Main.rand.NextBool(5))
                        {
                            target.AddBuff(ModContent.BuffType<GlacialState>(), duration, false);
                        }
                    }
                    if (aWeapon)
                    {
						CalamityUtils.Inflict246DebuffsNPC(target, ModContent.BuffType<AbyssalFlames>());
                    }
                    if (cryogenSoul || frostFlare)
                    {
						CalamityUtils.Inflict246DebuffsNPC(target, BuffID.Frostburn);
                    }
                    if (yInsignia)
                    {
						CalamityUtils.Inflict246DebuffsNPC(target, ModContent.BuffType<HolyFlames>());
                    }
                    if (ataxiaFire)
                    {
						CalamityUtils.Inflict246DebuffsNPC(target, BuffID.OnFire, 4f);
                    }
                }
                if (armorCrumbling || armorShattering)
                {
                    if (proj.melee || proj.Calamity().rogue)
                    {
						CalamityUtils.Inflict246DebuffsNPC(target, ModContent.BuffType<ArmorCrunch>());
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
					switch (player.meleeEnchant)
					{
						case 1:
							target.AddBuff(BuffID.Venom, 60 * Main.rand.Next(5, 10), false);
							break;
						case 2:
							target.AddBuff(BuffID.CursedInferno, 60 * Main.rand.Next(3, 7), false);
							break;
						case 3:
							target.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(3, 7), false);
							break;
						case 5:
							target.AddBuff(BuffID.Ichor, 60 * Main.rand.Next(10, 20), false);
							break;
						case 6:
							target.AddBuff(BuffID.Confused, 60 * Main.rand.Next(1, 4), false);
							break;
						case 8:
							target.AddBuff(BuffID.Poisoned, 60 * Main.rand.Next(5, 10), false);
							break;
						case 4:
							target.AddBuff(BuffID.Midas, 120, false);
							break;
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
                        if (ZoneCalamity && CalamityMod.fireWeaponList.Contains(player.ActiveItem().type))
                        {
                            target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 240, false);
                        }
                    }
					if (titanHeartMask)
					{
						target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 60 * Main.rand.Next(1,6), false); // 1 to 5 seconds
					}
                }
                if (vexation)
                {
                    if ((player.armor[0].type == ModContent.ItemType<ReaverCap>() || player.armor[0].type == ModContent.ItemType<ReaverHelm>() ||
                        player.armor[0].type == ModContent.ItemType<ReaverHelmet>() || player.armor[0].type == ModContent.ItemType<ReaverMask>() ||
                        player.armor[0].type == ModContent.ItemType<ReaverVisage>()) &&
                        player.armor[1].type == ModContent.ItemType<ReaverScaleMail>() && player.armor[2].type == ModContent.ItemType<ReaverCuisses>())
                    {
                        target.AddBuff(BuffID.CursedInferno, 90, false);
                        target.AddBuff(BuffID.Venom, 120, false);
                    }
                }
            }
			proj.Calamity().stealthStrikeHitCount++;
        }
        #endregion

        #region PvP
        public override void OnHitPvp(Item item, Player target, int damage, bool crit)
        {
			if (desertProwler && item.ranged && crit) //for obscure stuff like Marnite Bayonet
			{
				if (player.ownedProjectileCounts[ModContent.ProjectileType<DesertMark>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<DesertTornado>()] < 1)
				{
					if (Main.rand.NextBool(15))
					{
						if (player.whoAmI == Main.myPlayer)
						{
							Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<DesertMark>(), (int)(item.damage * player.RangedDamage()), item.knockBack, player.whoAmI, 0f, 0f);
						}
					}
				}
			}

            if (!item.melee && player.meleeEnchant == 7)
                Projectile.NewProjectile(target.Center, target.velocity, ProjectileID.ConfettiMelee, 0, 0f, player.whoAmI, 0f, 0f);

            if (omegaBlueChestplate)
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);
            if (sulfurSet)
                target.AddBuff(BuffID.Poisoned, 120);
            switch (item.type)
            {
                case ItemID.IceSickle:
                case ItemID.Frostbrand:
                    target.AddBuff(BuffID.Frostburn, 600);
                    break;

                case ItemID.IceBlade:
                    if (Main.rand.NextBool(5))
                        target.AddBuff(BuffID.Frostburn, 360);
                    else if (Main.rand.NextBool(3))
                        target.AddBuff(BuffID.Frostburn, 120);
                    break;
            }

            if (item.melee)
            {
				titanBoost = 600;
                if (eGauntlet)
                {
					int duration = 90;
                    target.AddBuff(BuffID.CursedInferno, duration / 2, false);
                    target.AddBuff(BuffID.Frostburn, duration, false);
                    target.AddBuff(BuffID.Ichor, duration, false);
                    target.AddBuff(BuffID.Venom, duration, false);
                    target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), duration, false);
                    target.AddBuff(ModContent.BuffType<AbyssalFlames>(), duration, false);
                    target.AddBuff(ModContent.BuffType<HolyFlames>(), duration, false);
                    target.AddBuff(ModContent.BuffType<Plague>(), duration, false);
                    target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), duration, false);
                    if (Main.rand.NextBool(5))
                    {
                        target.AddBuff(ModContent.BuffType<GlacialState>(), duration, false);
                    }
                }
                if (aWeapon)
                {
					CalamityUtils.Inflict246DebuffsPvp(target, ModContent.BuffType<AbyssalFlames>());
                }
                if (cryogenSoul || frostFlare)
                {
					CalamityUtils.Inflict246DebuffsPvp(target, BuffID.Frostburn);
                }
                if (yInsignia)
                {
					CalamityUtils.Inflict246DebuffsPvp(target, ModContent.BuffType<HolyFlames>());
                }
                if (ataxiaFire)
                {
					CalamityUtils.Inflict246DebuffsPvp(target, BuffID.OnFire, 4f);
                }
            }
            if (alchFlask)
            {
				CalamityUtils.Inflict246DebuffsPvp(target, ModContent.BuffType<Plague>());
            }
			if (abyssalAmulet)
			{
				CalamityUtils.Inflict246DebuffsPvp(target, ModContent.BuffType<CrushDepth>());
			}
            if (item.Calamity().rogue)
            {
				switch (player.meleeEnchant)
				{
					case 1:
						target.AddBuff(BuffID.Venom, 60 * Main.rand.Next(5, 10), false);
						break;
					case 2:
						target.AddBuff(BuffID.CursedInferno, 60 * Main.rand.Next(3, 7), false);
						break;
					case 3:
						target.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(3, 7), false);
						break;
					case 5:
						target.AddBuff(BuffID.Ichor, 60 * Main.rand.Next(10, 20), false);
						break;
					case 6:
						target.AddBuff(BuffID.Confused, 60 * Main.rand.Next(1, 4), false);
						break;
					case 8:
						target.AddBuff(BuffID.Poisoned, 60 * Main.rand.Next(5, 10), false);
						break;
					/*case 4:
						target.AddBuff(BuffID.Midas, 120, false);
						break;*/
				}
				if (titanHeartMask)
				{
					target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 60 * Main.rand.Next(1,6), false); // 1 to 5 seconds
				}
            }
            if (holyWrath)
            {
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 600, false);
            }
            if (vexation)
            {
                if ((player.armor[0].type == ModContent.ItemType<ReaverCap>() || player.armor[0].type == ModContent.ItemType<ReaverHelm>() ||
                    player.armor[0].type == ModContent.ItemType<ReaverHelmet>() || player.armor[0].type == ModContent.ItemType<ReaverMask>() ||
                    player.armor[0].type == ModContent.ItemType<ReaverVisage>()) &&
                    player.armor[1].type == ModContent.ItemType<ReaverScaleMail>() && player.armor[2].type == ModContent.ItemType<ReaverCuisses>())
                {
                    target.AddBuff(BuffID.CursedInferno, 90, false);
                    target.AddBuff(BuffID.Venom, 120, false);
                }
            }
        }

        public override void OnHitPvpWithProj(Projectile proj, Player target, int damage, bool crit)
        {
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

                case ProjectileID.Blizzard:
                case ProjectileID.NorthPoleSnowflake:
                    target.AddBuff(BuffID.Frostburn, 240);
                    break;

                case ProjectileID.SnowBallFriendly:
                    if (Main.rand.NextBool(10))
                        target.AddBuff(BuffID.Frostburn, 120);
                    else if (Main.rand.NextBool(5))
                        target.AddBuff(BuffID.Frostburn, 60);
                    break;

                case ProjectileID.IceBoomerang:
                case ProjectileID.IceBolt:
                case ProjectileID.FrostDaggerfish:
                    if (Main.rand.NextBool(5))
                        target.AddBuff(BuffID.Frostburn, 240);
                    else if (Main.rand.NextBool(3))
                        target.AddBuff(BuffID.Frostburn, 120);
                    break;
            }

            if (!proj.npcProj && !proj.trap)
            {
				if (desertProwler && proj.ranged && crit)
				{
					if (player.ownedProjectileCounts[ModContent.ProjectileType<DesertMark>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<DesertTornado>()] < 1)
					{
						if (Main.rand.NextBool(15))
						{
							if (player.whoAmI == Main.myPlayer)
							{
								Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<DesertMark>(), proj.damage, proj.knockBack, player.whoAmI, 0f, 0f);
							}
						}
					}
				}

				if (proj.Calamity().trueMelee)
					titanBoost = 600;

                if (sulfurSet && proj.friendly)
                    target.AddBuff(BuffID.Poisoned, 120);

                if (omegaBlueChestplate && proj.friendly)
                    target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);

                if (proj.melee && silvaMelee && Main.rand.NextBool(4))
                    target.AddBuff(ModContent.BuffType<SilvaStun>(), 20);


                if (abyssalAmulet)
                {
					CalamityUtils.Inflict246DebuffsPvp(target, ModContent.BuffType<CrushDepth>());
                }
                if ((plaguebringerCarapace || uberBees) && CalamityMod.friendlyBeeList.Contains(proj.type))
                {
                    target.AddBuff(ModContent.BuffType<Plague>(), 360);
                }
                else if (alchFlask)
                {
					CalamityUtils.Inflict246DebuffsPvp(target, ModContent.BuffType<Plague>());
                }
                if (proj.melee)
                {
                    if (eGauntlet)
                    {
						int duration = 90;
                        target.AddBuff(BuffID.CursedInferno, duration / 2, false);
                        target.AddBuff(BuffID.Frostburn, duration, false);
                        target.AddBuff(BuffID.Ichor, duration, false);
                        target.AddBuff(BuffID.Venom, duration, false);
                        target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), duration, false);
                        target.AddBuff(ModContent.BuffType<AbyssalFlames>(), duration, false);
                        target.AddBuff(ModContent.BuffType<HolyFlames>(), duration, false);
                        target.AddBuff(ModContent.BuffType<Plague>(), duration, false);
                        target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), duration, false);
                        if (Main.rand.NextBool(5))
                        {
                            target.AddBuff(ModContent.BuffType<GlacialState>(), duration, false);
                        }
                    }
                    if (aWeapon)
                    {
						CalamityUtils.Inflict246DebuffsPvp(target, ModContent.BuffType<AbyssalFlames>());
                    }
                    if (cryogenSoul || frostFlare)
                    {
						CalamityUtils.Inflict246DebuffsPvp(target, BuffID.Frostburn);
                    }
                    if (yInsignia)
                    {
						CalamityUtils.Inflict246DebuffsPvp(target, ModContent.BuffType<HolyFlames>());
                    }
                    if (ataxiaFire)
                    {
						CalamityUtils.Inflict246DebuffsPvp(target, BuffID.OnFire, 4f);
                    }
                }
                if (armorCrumbling || armorShattering)
                {
                    if (proj.melee || proj.Calamity().rogue)
                    {
						CalamityUtils.Inflict246DebuffsPvp(target, ModContent.BuffType<ArmorCrunch>());
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
					switch (player.meleeEnchant)
					{
						case 1:
							target.AddBuff(BuffID.Venom, 60 * Main.rand.Next(5, 10), false);
							break;
						case 2:
							target.AddBuff(BuffID.CursedInferno, 60 * Main.rand.Next(3, 7), false);
							break;
						case 3:
							target.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(3, 7), false);
							break;
						case 5:
							target.AddBuff(BuffID.Ichor, 60 * Main.rand.Next(10, 20), false);
							break;
						case 6:
							target.AddBuff(BuffID.Confused, 60 * Main.rand.Next(1, 4), false);
							break;
						case 8:
							target.AddBuff(BuffID.Poisoned, 60 * Main.rand.Next(5, 10), false);
							break;
						/*case 4:
							target.AddBuff(BuffID.Midas, 120, false);
							break;*/
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
                        /*if (Main.moonPhase == 6) //first quarter
                        {
                            target.AddBuff(BuffID.Midas, 120, false);
                        }*/
                        if (ZoneCalamity && CalamityMod.fireWeaponList.Contains(player.ActiveItem().type))
                        {
                            target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 240, false);
                        }
                    }
					if (titanHeartMask)
					{
						target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 60 * Main.rand.Next(1,6), false); // 1 to 5 seconds
					}
                }
                if (vexation)
                {
                    if ((player.armor[0].type == ModContent.ItemType<ReaverCap>() || player.armor[0].type == ModContent.ItemType<ReaverHelm>() ||
                        player.armor[0].type == ModContent.ItemType<ReaverHelmet>() || player.armor[0].type == ModContent.ItemType<ReaverMask>() ||
                        player.armor[0].type == ModContent.ItemType<ReaverVisage>()) &&
                        player.armor[1].type == ModContent.ItemType<ReaverScaleMail>() && player.armor[2].type == ModContent.ItemType<ReaverCuisses>())
                    {
                        target.AddBuff(BuffID.CursedInferno, 90, false);
                        target.AddBuff(BuffID.Venom, 120, false);
                    }
                }
                if (proj.type == ProjectileID.IchorArrow && player.ActiveItem().type == ModContent.ItemType<RaidersGlory>())
                {
                    target.AddBuff(BuffID.Midas, 300, false);
                }
            }
        }
        #endregion

        #region Modify Hit NPC
        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            #region MultiplierBoosts
            double damageMult = 1.0;
            if (silvaMelee && Main.rand.NextBool(4) && item.melee)
            {
                damageMult += 4.0;
            }
			if (item.melee)
			{
                damageMult += trueMeleeDamage;
			}
            if (enraged && !CalamityConfig.Instance.BossRushXerocCurse)
            {
                damageMult += 1.25;
            }
            if (CalamityWorld.revenge && CalamityConfig.Instance.Rippers)
            {
                bool DHorHoD = draedonsHeart || heartOfDarkness;
                if (rageModeActive && adrenalineModeActive)
                {
                    if (item.melee)
                    {
                        damageMult += (DHorHoD ? 2.3 : 2.0);
                    }
                }
                else if (rageModeActive)
                {
                    if (item.melee)
                    {
                        double rageDamageBoost = 0.0 +
                            (rageBoostOne ? 0.15 : 0.0) +
                            (rageBoostTwo ? 0.15 : 0.0) +
                            (rageBoostThree ? 0.15 : 0.0);
                        double rageDamage = (DHorHoD ? 0.65 : 0.5) + rageDamageBoost;
                        damageMult += rageDamage;
                    }
                }
                else if (adrenalineModeActive)
                {
                    if (item.melee)
                    {
                        damageMult += 1.5;
                    }
                }
            }
            damage = (int)(damage * damageMult);

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

            #region AdditiveBoosts
            if (item.melee && badgeOfBravery)
            {
                if ((player.armor[0].type == ModContent.ItemType<TarragonHelmet>() || player.armor[0].type == ModContent.ItemType<TarragonHelm>() ||
                    player.armor[0].type == ModContent.ItemType<TarragonHornedHelm>() || player.armor[0].type == ModContent.ItemType<TarragonMask>() ||
                    player.armor[0].type == ModContent.ItemType<TarragonVisage>()) &&
                    player.armor[1].type == ModContent.ItemType<TarragonBreastplate>() && player.armor[2].type == ModContent.ItemType<TarragonLeggings>())
                {
                    int penetratableDefense = Math.Max(target.defense - player.armorPenetration, 0);
                    int penetratedDefense = Math.Min(penetratableDefense, 10);
                    damage += (int)(0.5f * penetratedDefense);
                }
            }
            #endregion

            if (yharonLore)
                damage = (int)(damage * 0.75);

            if ((target.damage > 5 || target.boss) && player.whoAmI == Main.myPlayer && !target.SpawnedFromStatue)
            {
                if (item.melee && soaring)
                {
                    double useTimeMultiplier = 0.85 + (item.useTime * item.useAnimation / 3600D); //28 * 28 = 784 is average so that equals 784 / 3600 = 0.217777 + 1 = 21.7% boost
                    double wingTimeFraction = player.wingTimeMax / 20D;
                    double meleeStatMultiplier = (double)(player.meleeDamage * (float)(player.meleeCrit / 10D));

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
                            Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<ChaosGeyser>(), (int)(damage * 0.15), 2f, player.whoAmI, 0f, 0f);
                        }
                    }
				}
				if (unstablePrism && crit)
				{
					for (int s = 0; s < 3; s++)
					{
						Vector2 velocity = CalamityUtils.RandomVelocity(50f, 30f, 60f);
						Projectile.NewProjectile(target.Center, velocity, ModContent.ProjectileType<UnstableSpark>(), (int)(damage * 0.15), 0f, player.whoAmI);
					}
				}
                if (astralStarRain && crit && astralStarRainCooldown <= 0)
                {
                    astralStarRainCooldown = 60;
                    for (int n = 0; n < 3; n++)
                    {
						int projectileType = Utils.SelectRandom(Main.rand, new int[]
						{
							ModContent.ProjectileType<AstralStar>(),
							ProjectileID.HallowStar,
							ModContent.ProjectileType<FallenStarProj>()
						});
						CalamityUtils.ProjectileRain(target.Center, 400f, 100f, 500f, 800f, 25f, projectileType, (int)(120 * player.AverageDamage()), 5f, player.whoAmI, 6);
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
                if (CalamityConfig.Instance.Proficiency)
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
                if (CalamityWorld.revenge && CalamityConfig.Instance.Rippers)
                {
                    if (item.melee)
                    {
                        int stressGain = (int)(damage * 0.1);
                        int stressMaxGain = 10;
                        if (stressGain < 1)
                        {
                            stressGain = 1;
                        }
                        if (stressGain > stressMaxGain)
                        {
                            stressGain = stressMaxGain;
                        }
                        rage += stressGain;
                        if (rage >= rageMax)
                        {
                            rage = rageMax;
                        }
                    }
                }
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (proj.npcProj || proj.trap)
                return;

            bool isTrueMelee = proj.Calamity().trueMelee;
            bool isSummon = proj.IsSummon();
            bool hasClassType = proj.melee || proj.ranged || proj.magic || isSummon || proj.Calamity().rogue;

            Item heldItem = player.ActiveItem();

            if (isTrueMelee && soaring)
            {
                double useTimeMultiplier = 0.85 + (heldItem.useTime * heldItem.useAnimation / 3600D); //28 * 28 = 784 is average so that equals 784 / 3600 = 0.217777 + 1 = 21.7% boost
                double wingTimeFraction = player.wingTimeMax / 20D;
                double meleeStatMultiplier = player.meleeDamage * (float)(player.meleeCrit / 10D);

                if (player.wingTime < player.wingTimeMax)
                    player.wingTime += (int)(useTimeMultiplier * (wingTimeFraction + meleeStatMultiplier));

                if (player.wingTime > player.wingTimeMax)
                    player.wingTime = player.wingTimeMax;
            }

            #region MultiplierBoosts
            double damageMult = 1.0;
            if (isSummon)
            {
                if (heldItem.type > ItemID.None)
                {
                    if (heldItem.summon && !heldItem.melee && !heldItem.ranged && !heldItem.magic && !heldItem.Calamity().rogue)
                    {
                        damageMult += 0.1;
                    }
                }
            }
			if (isTrueMelee)
			{
                damageMult += trueMeleeDamage;
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
            if (hallowedPower)
            {
                if (isSummon)
                    damageMult += 0.15;
            }
            if (providenceLore && hasClassType)
            {
                damageMult += 0.1;
            }
            if (silvaMelee && Main.rand.NextBool(4) && isTrueMelee)
            {
                damageMult += 4.0;
            }
            if (enraged && !CalamityConfig.Instance.BossRushXerocCurse)
            {
                damageMult += 1.25;
            }
            if (auricSet)
            {
                if (silvaThrowing && proj.Calamity().rogue &&
                    crit && player.statLife > (int)(player.statLifeMax2 * 0.5))
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
                if (Main.rand.NextBool(randomChance))
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
                    damageMult += 0.1;
            }
            else if (proj.type == ProjectileID.InfernoFriendlyBlast)
            {
                damageMult += 0.33;
            }
            if (brimflameFrenzy && brimflameSet)
            {
                if (proj.magic)
                {
                    damageMult += 0.5;
                }
            }
            if (CalamityWorld.revenge && CalamityConfig.Instance.Rippers)
            {
                bool DHorHoD = draedonsHeart || heartOfDarkness;
                if (rageModeActive && adrenalineModeActive)
                {
                    if (hasClassType)
                    {
                        damageMult += (DHorHoD ? 2.3 : 2.0);
                    }
                }
                else if (rageModeActive)
                {
                    if (hasClassType)
                    {
                        double rageDamageBoost = 0.0 +
                            (rageBoostOne ? 0.15 : 0.0) +
                            (rageBoostTwo ? 0.15 : 0.0) +
                            (rageBoostThree ? 0.15 : 0.0);
                        double rageDamage = (DHorHoD ? 0.65 : 0.5) + rageDamageBoost;
                        damageMult += rageDamage;
                    }
                }
                else if (adrenalineModeActive)
                {
                    if (hasClassType)
                    {
                        damageMult += 1.5;
                    }
                }
            }
            if ((filthyGlove || electricianGlove) && proj.Calamity().stealthStrike && proj.Calamity().rogue)
            {
                if (nanotech)
                    damageMult += 0.05;
                else
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
            damage = (int)(damage * damageMult);

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
            if (proj.type == ModContent.ProjectileType<AcidBulletProj>())
            {
                int defenseAdd = (int)(target.defense * 0.05 * (proj.damage / 50D) * acidRoundMultiplier); //100 defense * 0.05 = 5
                damage += defenseAdd;
            }
            if (uberBees && CalamityMod.friendlyBeeList.Contains(proj.type))
            {
                damage += Main.rand.Next(20, 31);
            }
			if (plaguebringerPatronSummon)
			{
				if (isSummon && proj.active && proj.friendly && !proj.npcProj && !proj.trap && proj.damage > 0)
				{
					if (proj.type != ModContent.ProjectileType<DirectStrike>() && proj.type != ModContent.ProjectileType<PlaguebringerSummon>())
					{
						for (int j = 0; j < Main.maxProjectiles; j++)
						{
							Projectile miniPBG = Main.projectile[j];
							if (miniPBG.type == ModContent.ProjectileType<PlaguebringerSummon>() && Vector2.Distance(proj.Center, miniPBG.Center) <= PlaguebringerSummon.auraRange && miniPBG.owner == proj.owner)
							{
								damage += Main.rand.Next(10, 21);
								break;
							}
						}
					}
				}
			}
			int penetrateAmt = 0;
            if (proj.Calamity().stealthStrike && proj.Calamity().rogue)
            {
				if (nanotech)
					penetrateAmt += 20; //nanotech is weaker
				else if (electricianGlove)
					penetrateAmt += 30;
				else if (filthyGlove || bloodyGlove)
					penetrateAmt += 10;
            }
            if (proj.Calamity().rogue && etherealExtorter)
            {
                if (CalamityMod.boomerangProjList.Contains(proj.type) && player.ZoneCorrupt)
                {
					penetrateAmt += 6;
                }
            }
            if (proj.melee && badgeOfBravery)
            {
                if ((player.armor[0].type == ModContent.ItemType<TarragonHelmet>() || player.armor[0].type == ModContent.ItemType<TarragonHelm>() ||
                    player.armor[0].type == ModContent.ItemType<TarragonHornedHelm>() || player.armor[0].type == ModContent.ItemType<TarragonMask>() ||
                    player.armor[0].type == ModContent.ItemType<TarragonVisage>()) &&
                    player.armor[1].type == ModContent.ItemType<TarragonBreastplate>() && player.armor[2].type == ModContent.ItemType<TarragonLeggings>())
                {
					penetrateAmt += 10;
                }
            }
			int penetratableDefense = Math.Max(target.defense - player.armorPenetration, 0); //if find how much defense we can penetrate
			int penetratedDefense = Math.Min(penetratableDefense, penetrateAmt); //if we have more penetrate than enemy defense, use enemy defense
			damage += (int)(0.5f * penetratedDefense);
            #endregion

            #region MultiplicativeReductions

            // Fearmonger armor reduces the summoner cross-class nerf
			// Forbidden armor reduces said nerf when holding the respective helmet's preferred weapon type
            // Profaned Soul Crystal encourages use of other weapons, nerfing the damage would not make sense.

            bool forbidden = player.head == ArmorIDs.Head.AncientBattleArmor && player.body == ArmorIDs.Body.AncientBattleArmor && player.legs == ArmorIDs.Legs.AncientBattleArmor;
			bool reducedNerf = fearmongerSet || (forbidden && heldItem.magic);

			double summonNerfMult = reducedNerf ? 0.75 : 0.5;
            if (isSummon && !profanedCrystalBuffs)
            {
				if (heldItem.type > ItemID.None)
				{
					if (!heldItem.summon &&
						(heldItem.melee || heldItem.ranged || heldItem.magic || heldItem.Calamity().rogue) &&
						heldItem.hammer == 0 && heldItem.pick == 0 && heldItem.axe == 0 && heldItem.useStyle != 0 && 
						!heldItem.accessory && heldItem.ammo == AmmoID.None)
					{
						damage = (int)(damage * summonNerfMult);
					}
				}
            }

            if (proj.ranged)
            {
				// Nerfed in prehardmode due to bullet damage being a balance meme
				if (heldItem.type == ModContent.ItemType<HalibutCannon>())
				{
					if (!Main.hardMode)
						damage = (int)(damage * 0.5);
					if (proj.type == ProjectileID.IchorBullet || proj.type == ModContent.ProjectileType<AcidBulletProj>())
						damage = (int)(damage * 0.85);
				}

                switch (proj.type)
                {
                    case ProjectileID.CrystalShard:
                        damage = (int)(damage * 0.6);
                        break;
                    case ProjectileID.ChlorophyteBullet:
                        damage = (int)(damage * 0.8);
                        break;
                    case ProjectileID.HallowStar:
                        damage = (int)(damage * 0.7);
                        break;
                }

                if (proj.type == ModContent.ProjectileType<AcidBulletProj>() && heldItem.type == ModContent.ItemType<P90>())
                    damage = (int)(damage * 0.75);
            }

            if (proj.type == ProjectileID.SpectreWrath && player.ghostHurt)
                damage = (int)(damage * 0.7);

            if (yharonLore)
                damage = (int)(damage * 0.75);

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
                if (theBee && player.statLife >= player.statLifeMax2)
                {
                    Main.PlaySound(SoundID.Item110, proj.Center);
                }
                if (unstablePrism && crit)
                {
                    for (int s = 0; s < 3; s++)
                    {
						Vector2 velocity = CalamityUtils.RandomVelocity(50f, 30f, 60f);
                        Projectile.NewProjectile(target.Center, velocity, ModContent.ProjectileType<UnstableSpark>(), (int)(damage * 0.15), 0f, player.whoAmI);
                    }
                }
                if (electricianGlove && proj.Calamity().stealthStrike && proj.Calamity().rogue && proj.Calamity().stealthStrikeHitCount < 5)
                {
                    for (int s = 0; s < 3; s++)
                    {
						Vector2 velocity = CalamityUtils.RandomVelocity(50f, 30f, 60f);
                        int spark = Projectile.NewProjectile(target.Center, velocity, ModContent.ProjectileType<Spark>(), (int)(damage * 0.1), 0f, player.whoAmI);
                        Main.projectile[spark].Calamity().forceRogue = true;
                        Main.projectile[spark].localNPCHitCooldown = -1;
                    }
                }
                if (astralStarRain && crit && astralStarRainCooldown <= 0)
                {
                    astralStarRainCooldown = 60;
                    for (int n = 0; n < 3; n++)
                    {
						int projectileType = Utils.SelectRandom(Main.rand, new int[]
						{
							ModContent.ProjectileType<AstralStar>(),
							ProjectileID.HallowStar,
							ModContent.ProjectileType<FallenStarProj>()
						});
						CalamityUtils.ProjectileRain(target.Center, 400f, 100f, 500f, 800f, 25f, projectileType, (int)(120 * player.AverageDamage()), 5f, player.whoAmI, 6);
                    }
                }
                if (tarraRanged && crit && proj.ranged)
                {
                    int leafAmt = Main.rand.Next(2, 4);
                    for (int l = 0; l < leafAmt; l++)
                    {
						Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                        int FUCKYOU = Projectile.NewProjectile(target.Center, velocity, ProjectileID.Leaf, (int)(damage * 0.25), 0f, player.whoAmI);
                        Main.projectile[FUCKYOU].Calamity().forceTypeless = true;
                        Main.projectile[FUCKYOU].netUpdate = true;
                    }
                }
                if (bloodflareThrowing && proj.Calamity().rogue && crit && Main.rand.NextBool(2))
                {
                    if (target.canGhostHeal)
                    {
                        float projHitMult = 0.03f;
                        projHitMult -= (float)proj.numHits * 0.015f;
                        if (projHitMult < 0f)
                        {
                            projHitMult = 0f;
                        }
                        float cooldownMult = proj.damage * projHitMult;
                        if (cooldownMult < 0f)
                        {
                            cooldownMult = 0f;
                        }
                        if (player.lifeSteal > 0f)
                        {
                            player.statLife += 1;
                            player.HealEffect(1);
                            player.lifeSteal -= cooldownMult * 2f;
                        }
                    }
                }
                if (bloodflareMage && bloodflareMageCooldown <= 0 && crit && proj.magic)
                {
                    bloodflareMageCooldown = 120;
                    for (int i = 0; i < 3; i++)
                    {
						Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                        int fire = Projectile.NewProjectile(target.Center, velocity, ProjectileID.BallofFire, (int)(damage * 0.5), 0f, player.whoAmI);
                        Main.projectile[fire].Calamity().forceTypeless = true;
                        Main.projectile[fire].netUpdate = true;
                    }
                }
                if (umbraphileSet && proj.Calamity().rogue && (Main.rand.NextBool(4) || (proj.Calamity().stealthStrike && proj.Calamity().stealthStrikeHitCount < 5)) && proj.type != ModContent.ProjectileType<UmbraphileBoom>())
                {
                    Projectile.NewProjectile(proj.Center, Vector2.Zero, ModContent.ProjectileType<UmbraphileBoom>(), CalamityUtils.DamageSoftCap(proj.damage * 0.25, 50), 0f, player.whoAmI);
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
                if (CalamityConfig.Instance.Proficiency)
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
                if (raiderTalisman && raiderStack < 150 && proj.Calamity().rogue && crit && raiderCooldown <= 0)
                {
                    raiderStack++;
                    raiderCooldown = 30;
                }
                if (CalamityWorld.revenge && CalamityConfig.Instance.Rippers)
                {
                    if (isTrueMelee)
                    {
                        int stressGain = (int)(damage * 0.1);
                        int stressMaxGain = 10;
                        if (stressGain < 1)
                        {
                            stressGain = 1;
                        }
                        if (stressGain > stressMaxGain)
                        {
                            stressGain = stressMaxGain;
                        }
                        rage += stressGain;
                        if (rage >= rageMax)
                        {
                            rage = rageMax;
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

			if (areThereAnyDamnBosses && CalamityMod.bossVelocityDamageScaleValues.ContainsKey(npc.type))
			{
				CalamityMod.bossVelocityDamageScaleValues.TryGetValue(npc.type, out float velocityScalar);

				if (((npc.type == NPCID.EyeofCthulhu || npc.type == NPCID.Spazmatism) && npc.ai[0] >= 2f) || (npc.type == NPCID.Plantera && npc.life / (float)npc.lifeMax <= 0.5f))
					velocityScalar = CalamityMod.bitingEnemeyVelocityScale;

				if (npc.velocity == Vector2.Zero)
				{
					contactDamageReduction += 1f - velocityScalar;
				}
				else
				{
					float amount = npc.velocity.Length() / (npc.Calamity().maxVelocity * 0.5f);
					if (amount > 1f)
						amount = 1f;

					float damageReduction = MathHelper.Lerp(velocityScalar, 1.1f, amount);
					if (damageReduction < 1f)
						contactDamageReduction += 1f - damageReduction;
					else
						damage = (int)(damage * damageReduction);
				}
			}

            if (triumph)
                contactDamageReduction += 0.15 * (1D - (npc.life / (double)npc.lifeMax));

            if (aSparkRare)
            {
                if (npc.type == NPCID.BlueJellyfish || npc.type == NPCID.PinkJellyfish || npc.type == NPCID.GreenJellyfish ||
                    npc.type == NPCID.FungoFish || npc.type == NPCID.BloodJelly || npc.type == NPCID.AngryNimbus || npc.type == NPCID.GigaZapper ||
                    npc.type == NPCID.MartianTurret || npc.type == ModContent.NPCType<StormlionCharger>() || npc.type == ModContent.NPCType<GhostBell>() || npc.type == ModContent.NPCType<BoxJellyfish>())
                    contactDamageReduction += 0.5;
            }

            if (fleshTotem && !fleshTotemCooldown)
            {
                player.AddBuff(ModContent.BuffType<FleshTotemCooldown>(), 1200, false); //20 seconds
				contactDamageReduction += 0.5;
			}

            if (tarragonCloak && !tarragonCloakCooldown && tarraMelee)
				contactDamageReduction += 0.5;

            if (bloodflareMelee && bloodflareFrenzy && !bloodFrenzyCooldown)
				contactDamageReduction += 0.5;

            if (silvaMelee && silvaCountdown <= 0 && hasSilvaEffect)
				contactDamageReduction += 0.2;

			if (npc.Calamity().tSad > 0)
				contactDamageReduction += 0.5;

			if (npc.Calamity().relicOfResilienceWeakness > 0)
			{
				contactDamageReduction += RelicOfResilience.WeaknessDR;
				npc.Calamity().relicOfResilienceWeakness = 0;
			}

			if (beeResist)
			{
				if (CalamityMod.beeEnemyList.Contains(npc.type))
					contactDamageReduction += 0.25;
			}

			if (eskimoSet)
			{
				if (npc.coldDamage)
					contactDamageReduction += 0.1;
			}

			if (trinketOfChiBuff)
				contactDamageReduction += 0.15;

			// Fearmonger set provides 15% multiplicative DR that ignores caps during the Holiday Moons.
			// To prevent abuse, this effect does not work if there are any bosses alive.
			if (fearmongerSet && !areThereAnyDamnBosses && (Main.pumpkinMoon || Main.snowMoon))
				contactDamageReduction += 0.15;

			if (abyssalDivingSuitPlates)
				contactDamageReduction += 0.15;

			if (sirenIce)
				contactDamageReduction += 0.2;

			if (encased)
				contactDamageReduction += 0.3;

			if (player.ownedProjectileCounts[ModContent.ProjectileType<EnergyShell>()] > 0 && player.ActiveItem().type == ModContent.ItemType<LionHeart>())
				contactDamageReduction += 0.5;

			if (theBee && player.statLife >= player.statLifeMax2 && theBeeCooldown <= 0)
			{
				contactDamageReduction += 0.5;
				theBeeCooldown = 600;
			}

			if (CalamityWorld.revenge)
			{
				if (!CalamityWorld.downedBossAny)
					contactDamageReduction += 0.2;

				if (CalamityConfig.Instance.Rippers)
				{
					if (adrenaline == adrenalineMax && !adrenalineModeActive)
						contactDamageReduction += 0.5;
				}
			}

			if (player.mount.Active && (player.mount.Type == ModContent.MountType<AngryDogMount>() || player.mount.Type == ModContent.MountType<OnyxExcavator>()) && Math.Abs(player.velocity.X) > player.mount.RunSpeed / 2f)
				contactDamageReduction += 0.1;

			if (leviathanAndSirenLore)
			{
				if (!player.IsUnderwater())
					contactDamageReduction -= 0.05;
			}

			if (vHex)
				contactDamageReduction -= 0.3;

			if (irradiated)
				contactDamageReduction -= 0.1;

			if (corrEffigy)
				contactDamageReduction -= 0.2;

			if (calamityRing && !voidOfExtinction)
				contactDamageReduction -= 0.15;

			// 10% is converted to 9%, 25% is converted to 20%, 50% is converted to 33%, 75% is converted to 43%, 100% is converted to 50%
			if (contactDamageReduction > 0D)
			{
				if (marked || reaperToothNecklace)
					contactDamageReduction *= 0.5;

				if (aCrunch)
					contactDamageReduction *= 0.33;

				if (wCleave)
					contactDamageReduction *= 0.75;

				// Scale with base damage reduction
				if (DRStat > 0)
					contactDamageReduction *= 1f - (DRStat * 0.01f);

				contactDamageReduction = 1D / (1D + contactDamageReduction);
				damage = (int)(damage * contactDamageReduction);
			}

			if (Main.hardMode && Main.expertMode)
			{
				bool reduceChaosBallDamage = npc.type == NPCID.ChaosBall && !NPC.AnyNPCs(NPCID.GoblinSummoner);

				if (reduceChaosBallDamage || npc.type == NPCID.BurningSphere || npc.type == NPCID.WaterSphere)
					damage = (int)(damage * 0.6);
			}

			if (CalamityWorld.ironHeart)
			{
				int damageMin = 60 + (player.statLifeMax2 / 10);
				int damageWithDR = (int)(damage * (1f - player.endurance));
				if (damage < damageMin)
				{
					player.endurance = 0f;
					damage = damageMin;
				}
			}

			if (aBulwarkRare)
			{
				aBulwarkRareMeleeBoostTimer += 3 * damage;
				if (aBulwarkRareMeleeBoostTimer > 900)
					aBulwarkRareMeleeBoostTimer = 900;
			}

			if (player.whoAmI == Main.myPlayer && gainRageCooldown <= 0)
            {
                if (CalamityWorld.revenge && CalamityConfig.Instance.Rippers && !npc.SpawnedFromStatue)
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
                    rage += stressGain;
                    if (rage >= rageMax)
                    {
                        rage = rageMax;
                    }
                }
            }
		}
        #endregion

        #region Modify Hit By Proj
        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
			if (CalamityMod.projectileDestroyExceptionList.TrueForAll(x => proj.type != x))
			{
				if (player.ActiveItem().type == ModContent.ItemType<GaelsGreatsword>()
					&& proj.active && proj.hostile && player.altFunctionUse == 2 && Main.rand.NextBool(2))
				{
					for (int j = 0; j < 3; j++)
					{
						int dustIndex = Dust.NewDust(proj.position, proj.width, proj.height, 31, 0f, 0f, 0, default, 1f);
						Main.dust[dustIndex].velocity *= 0.3f;
					}
					int damage2 = (int)(GaelsGreatsword.BaseDamage * player.MeleeDamage());
					proj.hostile = false;
					proj.friendly = true;
					proj.velocity *= -1f;
					proj.damage = damage2;
					proj.penetrate = 1;
					player.immune = true;
					player.immuneNoBlink = true;
					player.immuneTime = 4;
					damage = 0;
					return;
				}
			}

			if (proj.type == ModContent.ProjectileType<BirbAura>())
			{
				damage = 0;
				return;
			}

			// Reduce damage from vanilla traps
			// 350 in normal, 450 in expert
			if (proj.type == ProjectileID.Explosives)
				damage = (int)(damage * (Main.expertMode ? 0.225 : 0.35));
			if (Main.expertMode)
			{
				// 140 in normal, 182 in expert
				if (proj.type == ProjectileID.Boulder)
					damage = (int)(damage * 0.65);
			}

			// Reduce the bullshit damage Ichor Stickers do
			if (proj.type == ProjectileID.GoldenShowerHostile)
				damage = (int)(damage * 0.35);

			if (CalamityWorld.revenge)
			{
				double damageMultiplier = 1D;
				if (CalamityMod.revengeanceProjectileBuffList25Percent.Contains(proj.type))
				{
					damageMultiplier += 0.25;
				}
				else if (CalamityMod.revengeanceProjectileBuffList20Percent.Contains(proj.type))
				{
					damageMultiplier += 0.2;
				}
				else if (CalamityMod.revengeanceProjectileBuffList15Percent.Contains(proj.type))
				{
					damageMultiplier += 0.15;
				}
				else if (CalamityMod.revengeanceProjectileBuffList10Percent.Contains(proj.type))
				{
					damageMultiplier += 0.1;
				}
				else if (CalamityMod.revengeanceProjectileBuffList5Percent.Contains(proj.type))
				{
					damageMultiplier += 0.05;
				}

				if (CalamityWorld.death)
					damageMultiplier += (damageMultiplier - 1D) * 0.6;

				damage = (int)(damage * damageMultiplier);
			}

			int bossRushDamage = (Main.expertMode ? 125 : 150) + (CalamityWorld.bossRushStage / 2);
			if (CalamityWorld.bossRushActive)
			{
				if (damage < bossRushDamage)
					damage = bossRushDamage;
			}

			// Reduce projectile damage based on banner type
			// IMPORTANT NOTE: Rework this in 1.4!
			Point point = player.Center.ToTileCoordinates();
			int buffScanAreaWidth = (Main.maxScreenW + 800) / 16 - 1;
			int buffScanAreaHeight = (Main.maxScreenH + 800) / 16 - 1;
			Rectangle rectangle = CalamityUtils.ClampToWorld(tileRectangle: new Rectangle(point.X - buffScanAreaWidth / 2, point.Y - buffScanAreaHeight / 2, buffScanAreaWidth, buffScanAreaHeight));
			bool[] NPCBannerBuff = new bool[Main.MaxBannerTypes];
			bool hasBanner = false;

			// Scan area around the player for banners
			for (int i = rectangle.Left; i < rectangle.Right; i++)
			{
				for (int j = rectangle.Top; j < rectangle.Bottom; j++)
				{
					if (!rectangle.Contains(i, j))
						continue;

					Tile tile = Main.tile[i, j];
					if (tile is null || !tile.active())
						continue;

					if (tile.type == TileID.Banners && (tile.frameX >= 396 || tile.frameY >= 54))
					{
						int bannerType = tile.frameX / 18 - 21;
						for (int k = tile.frameY; k >= 54; k -= 54)
						{
							bannerType += 90;
							bannerType += 21;
						}

						int bannerItemType = Item.BannerToItem(bannerType);
						if (ItemID.Sets.BannerStrength[bannerItemType].Enabled)
						{
							NPCBannerBuff[bannerType] = true;
							hasBanner = true;
						}
					}
				}
			}

			// Reduce damage
			if (hasBanner)
				BannerProjectileDamageReduction(proj, ref damage, NPCBannerBuff);

            if (projRefRare)
            {
                if (proj.type == projTypeJustHitBy)
                    projectileDamageReduction += 0.15;
            }

            if (aSparkRare)
            {
                if (proj.type == ProjectileID.MartianTurretBolt || proj.type == ProjectileID.GigaZapperSpear || proj.type == ProjectileID.CultistBossLightningOrbArc || proj.type == ModContent.ProjectileType<LightningMark>() || proj.type == ProjectileID.VortexLightning ||
                    proj.type == ProjectileID.BulletSnowman || proj.type == ProjectileID.BulletDeadeye || proj.type == ProjectileID.SniperBullet || proj.type == ProjectileID.VortexLaser)
                    projectileDamageReduction += 0.5;
            }
            

            if (beeResist)
            {
                if (CalamityMod.beeProjectileList.Contains(proj.type))
                    projectileDamageReduction += 0.25;
            }

            if (Main.hardMode && Main.expertMode && !CalamityWorld.spawnedHardBoss && proj.active && !proj.friendly && proj.hostile && damage > 0)
            {
                if (CalamityMod.hardModeNerfList.Contains(proj.type))
                    projectileDamageReduction += 0.25;
            }

			if (trinketOfChiBuff)
				projectileDamageReduction += 0.15;

			// Fearmonger set provides 15% multiplicative DR that ignores caps during the Holiday Moons.
			// To prevent abuse, this effect does not work if there are any bosses alive.
			if (fearmongerSet && !areThereAnyDamnBosses && (Main.pumpkinMoon || Main.snowMoon))
				projectileDamageReduction += 0.15;

			if (abyssalDivingSuitPlates)
				projectileDamageReduction += 0.15;

			if (sirenIce)
				projectileDamageReduction += 0.2;

			if (encased)
				projectileDamageReduction += 0.3;

			if (player.ownedProjectileCounts[ModContent.ProjectileType<EnergyShell>()] > 0 && player.ActiveItem().type == ModContent.ItemType<LionHeart>())
				projectileDamageReduction += 0.5;

			if (theBee && player.statLife >= player.statLifeMax2 && theBeeCooldown <= 0)
			{
				projectileDamageReduction += 0.5;
				theBeeCooldown = 600;
			}

			if (CalamityWorld.revenge)
			{
				if (!CalamityWorld.downedBossAny)
					projectileDamageReduction += 0.2;

				if (CalamityConfig.Instance.Rippers)
				{
					if (adrenaline == adrenalineMax && !adrenalineModeActive)
						projectileDamageReduction += 0.5;
				}
			}

			if (player.mount.Active && (player.mount.Type == ModContent.MountType<AngryDogMount>() || player.mount.Type == ModContent.MountType<OnyxExcavator>()) && Math.Abs(player.velocity.X) > player.mount.RunSpeed / 2f)
				projectileDamageReduction += 0.1;

			if (leviathanAndSirenLore)
			{
				if (!player.IsUnderwater())
					projectileDamageReduction -= 0.05;
			}

			if (vHex)
				projectileDamageReduction -= 0.3;

			if (irradiated)
				projectileDamageReduction -= 0.1;

			if (corrEffigy)
				projectileDamageReduction -= 0.2;

			if (calamityRing && !voidOfExtinction)
				projectileDamageReduction -= 0.15;

			// 10% is converted to 9%, 25% is converted to 20%, 50% is converted to 33%, 75% is converted to 43%, 100% is converted to 50%
			if (projectileDamageReduction > 0D)
			{
				if (marked || reaperToothNecklace)
					projectileDamageReduction *= 0.5;

				if (aCrunch)
					projectileDamageReduction *= 0.33;

				if (wCleave)
					projectileDamageReduction *= 0.75;

				// Scale with base damage reduction
				if (DRStat > 0)
					projectileDamageReduction *= 1f - (DRStat * 0.01f);

				projectileDamageReduction = 1D / (1D + projectileDamageReduction);
				damage = (int)(damage * projectileDamageReduction);
			}

			if (CalamityWorld.ironHeart)
			{
				int damageMin = 60 + (player.statLifeMax2 / 10);
				int damageWithDR = (int)(damage * (1f - player.endurance));
				if (damage < damageMin)
				{
					player.endurance = 0f;
					damage = damageMin;
				}
			}

			if (player.whoAmI == Main.myPlayer && gainRageCooldown <= 0)
            {
                if (CalamityWorld.revenge && CalamityConfig.Instance.Rippers && !CalamityMod.trapProjectileList.Contains(proj.type))
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
                    rage += stressGain;
                    if (rage >= rageMax)
                    {
                        rage = rageMax;
                    }
                }
            }
        }
		#endregion

		#region Banner Projectile Damage Reduction
		private void BannerProjectileDamageReduction(Projectile proj, ref int damage, bool[] NPCBannerBuffs)
		{
			bool? reduceDamage = null;
			double bannerDamageMultiplier = Main.expertMode ? 0.5 : 0.75;

			for (int l = 0; l < Main.MaxBannerTypes; l++)
			{
				int bannerNPCType = Item.BannerToNPC(l);
				if (bannerNPCType != 0 && NPCBannerBuffs[l])
				{
					if (proj.type == ModContent.ProjectileType<BelchingCoralSpike>())
					{
						if (bannerNPCType == ModContent.NPCType<BelchingCoral>())
							reduceDamage = true;
					}
					else if (proj.type == ModContent.ProjectileType<CrabBoulder>())
					{
						if (bannerNPCType == ModContent.NPCType<AnthozoanCrab>())
							reduceDamage = true;
					}
					else if (proj.type == ModContent.ProjectileType<EarthRockBig>() || proj.type == ModContent.ProjectileType<EarthRockSmall>())
					{
						if (bannerNPCType == ModContent.NPCType<Horse>())
							reduceDamage = true;
					}
					else if (proj.type == ModContent.ProjectileType<FlakAcid>())
					{
						if (bannerNPCType == ModContent.NPCType<FlakCrab>())
							reduceDamage = true;
					}
					else if (proj.type == ModContent.ProjectileType<FlameBurstHostile>())
					{
						if (bannerNPCType == ModContent.NPCType<ImpiousImmolator>())
							reduceDamage = true;
					}
					else if (proj.type == ModContent.ProjectileType<GammaAcid>() || proj.type == ModContent.ProjectileType<GammaBeam>())
					{
						if (bannerNPCType == ModContent.NPCType<GammaSlime>())
							reduceDamage = true;
					}
					else if (proj.type == ModContent.ProjectileType<HorsWaterBlast>())
					{
						if (bannerNPCType == ModContent.NPCType<Cnidrion>())
							reduceDamage = true;
					}
					else if (proj.type == ModContent.ProjectileType<InkBombHostile>() || proj.type == ModContent.ProjectileType<InkPoisonCloud>() || proj.type == ModContent.ProjectileType<InkPoisonCloud2>() || proj.type == ModContent.ProjectileType<InkPoisonCloud3>())
					{
						if (bannerNPCType == ModContent.NPCType<ColossalSquid>())
							reduceDamage = true;
					}
					else if (proj.type == ModContent.ProjectileType<MantisRing>())
					{
						if (bannerNPCType == ModContent.NPCType<Mantis>())
							reduceDamage = true;
					}
					else if (proj.type == ModContent.ProjectileType<NuclearToadGoo>())
					{
						if (bannerNPCType == ModContent.NPCType<NuclearToad>())
							reduceDamage = true;
					}
					else if (proj.type == ModContent.ProjectileType<OrthoceraStream>())
					{
						if (bannerNPCType == ModContent.NPCType<Orthocera>())
							reduceDamage = true;
					}
					else if (proj.type == ModContent.ProjectileType<PearlBurst>() || proj.type == ModContent.ProjectileType<PearlRain>())
					{
						if (bannerNPCType == ModContent.NPCType<GiantClam>())
							reduceDamage = true;
					}
					else if (proj.type == ModContent.ProjectileType<PufferExplosion>())
					{
						if (bannerNPCType == ModContent.NPCType<ChaoticPuffer>())
							reduceDamage = true;
					}
					else if (proj.type == ModContent.ProjectileType<StormMarkHostile>() || proj.type == ModContent.ProjectileType<TornadoHostile>())
					{
						if (bannerNPCType == ModContent.NPCType<ThiccWaifu>())
							reduceDamage = true;
					}
					else if (proj.type == ModContent.ProjectileType<SulphuricAcidBubble>() || proj.type == ModContent.ProjectileType<SulphuricAcidMist>())
					{
						if (bannerNPCType == ModContent.NPCType<Mauler>() || (bannerNPCType == ModContent.NPCType<Flounder>() && proj.type == ModContent.ProjectileType<SulphuricAcidMist>()))
							reduceDamage = true;
					}
					else if (proj.type == ModContent.ProjectileType<ToxicMinnowCloud>())
					{
						if (bannerNPCType == ModContent.NPCType<ToxicMinnow>())
							reduceDamage = true;
					}
					else if (proj.type == ModContent.ProjectileType<TrilobiteSpike>())
					{
						if (bannerNPCType == ModContent.NPCType<Trilobite>())
							reduceDamage = true;
					}
					else if (proj.type == ModContent.ProjectileType<BrimstoneLaser>() || proj.type == ModContent.ProjectileType<BrimstoneLaserSplit>())
					{
						if (bannerNPCType == ModContent.NPCType<SoulSlurper>())
							reduceDamage = !NPC.AnyNPCs(ModContent.NPCType<Calamitas>()) && !NPC.AnyNPCs(ModContent.NPCType<CalamitasRun3>());
					}
					else if (proj.type == ModContent.ProjectileType<GreatSandBlast>())
					{
						if (bannerNPCType == ModContent.NPCType<GreatSandShark>())
							reduceDamage = true;
					}
					else if (proj.type == ModContent.ProjectileType<PhantomGhostShot>())
					{
						if (bannerNPCType == ModContent.NPCType<PhantomSpiritL>())
							reduceDamage = !NPC.AnyNPCs(ModContent.NPCType<Polterghast>());
					}
					else if (proj.type == ModContent.ProjectileType<PlagueStingerGoliathV2>())
					{
						if (bannerNPCType == ModContent.NPCType<PlaguedJungleSlime>() || bannerNPCType == ModContent.NPCType<PlaguebringerShade>())
							reduceDamage = !NPC.AnyNPCs(ModContent.NPCType<PlaguebringerGoliath>());
					}
					else if (proj.type == ModContent.ProjectileType<HiveBombGoliath>())
					{
						if (bannerNPCType == ModContent.NPCType<PlaguebringerShade>())
							reduceDamage = !NPC.AnyNPCs(ModContent.NPCType<PlaguebringerGoliath>());
					}
					else if (proj.type == ModContent.ProjectileType<HolyBomb>() || proj.type == ModContent.ProjectileType<HolyFlare>())
					{
						if (bannerNPCType == ModContent.NPCType<ProfanedEnergyBody>())
							reduceDamage = !NPC.AnyNPCs(ModContent.NPCType<Providence>());
					}
					else if (proj.type == ProjectileID.EyeBeam)
					{
						if (bannerNPCType == ModContent.NPCType<Laserfish>())
							reduceDamage = !NPC.AnyNPCs(NPCID.Golem) && !NPC.AnyNPCs(ModContent.NPCType<RavagerBody>());
					}
					else if (proj.type == ProjectileID.CultistBossIceMist || proj.type == ProjectileID.CultistBossLightningOrbArc)
					{
						if (bannerNPCType == ModContent.NPCType<EidolonWyrmHead>() || bannerNPCType == ModContent.NPCType<Eidolist>())
							reduceDamage = !NPC.AnyNPCs(ModContent.NPCType<StormWeaverHead>()) && !NPC.AnyNPCs(ModContent.NPCType<StormWeaverHeadNaked>()) && !NPC.AnyNPCs(NPCID.CultistBoss) && proj.Calamity().lineColor != 1;
					}
					else if (proj.type == ProjectileID.SaucerScrap)
					{
						if (bannerNPCType == ModContent.NPCType<ArmoredDiggerHead>())
							reduceDamage = Main.invasionType != InvasionID.MartianMadness && (!NPC.AnyNPCs(NPCID.TheDestroyer) || !CalamityWorld.revenge);
					}
					else
					{
						switch (proj.type)
						{
							case ProjectileID.Stinger:
								if (CalamityMod.hornetList.Contains(bannerNPCType) || CalamityMod.mossHornetList.Contains(bannerNPCType))
								{
									reduceDamage = !NPC.AnyNPCs(NPCID.QueenBee);
								}
								break;

							case ProjectileID.PinkLaser:
								if (bannerNPCType == NPCID.Gastropod || bannerNPCType == ModContent.NPCType<AstralProbe>())
								{
									reduceDamage = !NPC.AnyNPCs(NPCID.TheDestroyer) && !NPC.AnyNPCs(ModContent.NPCType<LifeSeeker>()) && !NPC.AnyNPCs(ModContent.NPCType<StormWeaverHead>()) && !NPC.AnyNPCs(ModContent.NPCType<StormWeaverHeadNaked>());
								}
								break;

							case ProjectileID.SandBallFalling:
								if (bannerNPCType == NPCID.Antlion)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.DemonSickle:
								if (bannerNPCType == NPCID.Demon || bannerNPCType == NPCID.VoodooDemon)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.HarpyFeather:
								if (bannerNPCType == NPCID.Harpy)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.JavelinHostile:
								if (bannerNPCType == NPCID.GreekSkeleton)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.IceSpike:
								if (bannerNPCType == NPCID.SpikedIceSlime)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.JungleSpike:
								if (bannerNPCType == NPCID.SpikedJungleSlime)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.WebSpit:
								if (bannerNPCType == NPCID.BlackRecluse || bannerNPCType == NPCID.BlackRecluseWall)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.CursedFlameHostile:
								if (bannerNPCType == NPCID.Clinger)
								{
									reduceDamage = !NPC.AnyNPCs(NPCID.Spazmatism) && (!NPC.AnyNPCs(NPCID.EaterofWorldsHead) || !CalamityWorld.revenge);
								}
								break;

							case ProjectileID.DesertDjinnCurse:
								if (bannerNPCType == NPCID.DesertDjinn)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.InfernoHostileBlast:
							case ProjectileID.InfernoHostileBolt:
								if (bannerNPCType == NPCID.DiabolistRed || bannerNPCType == NPCID.DiabolistWhite)
								{
									reduceDamage = !NPC.AnyNPCs(NPCID.Golem) || !CalamityWorld.revenge;
								}
								break;

							case ProjectileID.Shadowflames:
								if (bannerNPCType == NPCID.GiantCursedSkull)
								{
									reduceDamage = !NPC.AnyNPCs(NPCID.SkeletronHead) || !CalamityWorld.revenge;
								}
								break;

							case ProjectileID.FrostBlastHostile:
								if (bannerNPCType == NPCID.IceElemental || bannerNPCType == ModContent.NPCType<IceClasper>())
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.IcewaterSpit:
								if (bannerNPCType == NPCID.IcyMerman)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.GoldenShowerHostile:
								if (bannerNPCType == NPCID.IchorSticker)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.ShadowBeamHostile:
								if (bannerNPCType == NPCID.Necromancer || bannerNPCType == NPCID.NecromancerArmored)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.PaladinsHammerHostile:
								if (bannerNPCType == NPCID.Paladin)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.LostSoulHostile:
								if (bannerNPCType == NPCID.RaggedCaster || bannerNPCType == NPCID.RaggedCasterOpenCoat)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.UnholyTridentHostile:
								if (bannerNPCType == NPCID.RedDevil)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.RuneBlast:
								if (bannerNPCType == NPCID.RuneWizard)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.WoodenArrowHostile:
								if (bannerNPCType == NPCID.GoblinArcher || bannerNPCType == NPCID.CultistArcherBlue || bannerNPCType == NPCID.CultistArcherWhite)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.FlamingArrow:
								if (bannerNPCType == NPCID.SkeletonArcher || bannerNPCType == NPCID.PirateCrossbower || bannerNPCType == NPCID.ElfArcher)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.SalamanderSpit:
								if (bannerNPCType >= NPCID.Salamander && bannerNPCType <= NPCID.Salamander9)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.SkeletonBone:
								if (bannerNPCType == NPCID.Skeleton)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.RocketSkeleton:
								if (bannerNPCType == NPCID.SkeletonCommando)
								{
									reduceDamage = !NPC.AnyNPCs(NPCID.SkeletronPrime) || !CalamityWorld.revenge;
								}
								break;

							case ProjectileID.SniperBullet:
								if (bannerNPCType == NPCID.SkeletonSniper)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.BulletDeadeye:
								if (bannerNPCType == NPCID.TacticalSkeleton || bannerNPCType == NPCID.SnowmanGangsta || bannerNPCType == NPCID.PirateCaptain || bannerNPCType == NPCID.PirateDeadeye || bannerNPCType == NPCID.ElfCopter)
								{
									reduceDamage = !NPC.AnyNPCs(NPCID.SantaNK1);
								}
								break;

							case ProjectileID.RainNimbus:
								if (bannerNPCType == NPCID.AngryNimbus)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.FrostShard:
								if (bannerNPCType == NPCID.AngryNimbus)
								{
									reduceDamage = !NPC.AnyNPCs(NPCID.IceQueen) && CalamityWorld.death;
								}
								break;

							case ProjectileID.FrostBeam:
								if (bannerNPCType == NPCID.IceGolem)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.SandnadoHostile:
							case ProjectileID.SandnadoHostileMark:
								if (bannerNPCType == NPCID.SandElemental)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.SnowBallHostile:
								if (bannerNPCType == NPCID.SnowBalla)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.CannonballHostile:
								if (bannerNPCType == NPCID.PirateCaptain)
								{
									reduceDamage = !NPC.AnyNPCs(NPCID.PirateShip);
								}
								break;

							case ProjectileID.DrManFlyFlask:
								if (bannerNPCType == NPCID.DrManFly)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.EyeLaser:
								if (bannerNPCType == NPCID.Eyezor)
								{
									reduceDamage = !NPC.AnyNPCs(NPCID.Retinazer) && !NPC.AnyNPCs(NPCID.WallofFleshEye);
								}
								break;

							case ProjectileID.Nail:
								if (bannerNPCType == NPCID.Nailhead)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.NebulaSphere:
								if (bannerNPCType == NPCID.NebulaBeast)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.NebulaLaser:
								if (bannerNPCType == NPCID.NebulaBrain)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.NebulaBolt:
								if (bannerNPCType == NPCID.NebulaSoldier)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.StardustJellyfishSmall:
								if (bannerNPCType == NPCID.StardustJellyfishBig)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.StardustSoldierLaser:
								if (bannerNPCType == NPCID.StardustSoldier)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.Twinkle:
								if (bannerNPCType == NPCID.StardustSpiderBig)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.VortexAcid:
								if (bannerNPCType == NPCID.VortexHornetQueen)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.VortexLaser:
								if (bannerNPCType == NPCID.VortexRifleman)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.VortexLightning:
								if (bannerNPCType == NPCID.VortexSoldier)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.RayGunnerLaser:
								if (bannerNPCType == NPCID.RayGunner)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.BrainScramblerBolt:
								if (bannerNPCType == NPCID.BrainScrambler)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.MartianWalkerLaser:
								if (bannerNPCType == NPCID.MartianWalker)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.MartianTurretBolt:
								if ((bannerNPCType == NPCID.MartianTurret && Main.invasionType == InvasionID.MartianMadness) || ((bannerNPCType == ModContent.NPCType<ShockstormShuttle>() || bannerNPCType == ModContent.NPCType<WulfrumDrone>()) && Main.invasionType != InvasionID.MartianMadness))
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.SaucerLaser:
								if (bannerNPCType == ModContent.NPCType<ShockstormShuttle>() && Main.invasionType != InvasionID.MartianMadness)
								{
									reduceDamage = true;
								}
								break;

							case ProjectileID.HappyBomb:
								if (bannerNPCType == NPCID.Clown)
								{
									reduceDamage = true;
								}
								break;
						}
					}

					if (reduceDamage.HasValue)
					{
						if (reduceDamage.Value)
							damage = (int)(damage * bannerDamageMultiplier);

						break;
					}
				}
			}
		}
		#endregion

		#region On Hit
		public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            if (sulfurSet)
                npc.AddBuff(BuffID.Poisoned, 120);
            if (CalamityWorld.revenge)
            {
                if (npc.type == NPCID.ShadowFlameApparition || (npc.type == NPCID.ChaosBall && Main.hardMode))
                {
                    player.AddBuff(ModContent.BuffType<Shadowflame>(), 180);
                }
                else if (npc.type == NPCID.Spazmatism && npc.ai[0] != 1f && npc.ai[0] != 2f && npc.ai[0] != 0f)
                {
                    player.AddBuff(BuffID.Bleeding, 300);
                }
                else if (npc.type == NPCID.Plantera && npc.life < npc.lifeMax / 2)
                {
                    player.AddBuff(BuffID.Poisoned, 180);
					player.AddBuff(BuffID.Venom, 180);
					player.AddBuff(BuffID.Bleeding, 300);
                }
                else if (npc.type == NPCID.PlanterasTentacle)
                {
                    player.AddBuff(BuffID.Poisoned, 120);
					player.AddBuff(BuffID.Venom, 120);
					player.AddBuff(BuffID.Bleeding, 180);
                }
				else if (npc.type == NPCID.AncientDoom)
				{
					player.AddBuff(ModContent.BuffType<Horror>(), 180);
				}
				else if (npc.type == NPCID.AncientLight)
				{
					player.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
				}
			}
        }

        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            if (sulfurSet && !proj.friendly)
            {
                if (Main.player[proj.owner] is null)
                {
                    if (!Main.npc[proj.owner].friendly)
                        Main.npc[proj.owner].AddBuff(BuffID.Poisoned, 120);
                }
                else
                {
                    Player p = Main.player[proj.owner];
                    if (p.hostile && player.hostile && (player.team != p.team || p.team == 0))
                        p.AddBuff(BuffID.Poisoned, 120);
                }
            }

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
                else if (proj.type == ProjectileID.DeathLaser || proj.type == ProjectileID.RocketSkeleton)
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
					player.AddBuff(BuffID.Venom, 120);
				}
                else if (proj.type == ProjectileID.CultistBossIceMist)
                {
                    player.AddBuff(BuffID.Frozen, 60);
                    player.AddBuff(BuffID.Chilled, 120);
                    player.AddBuff(BuffID.Frostburn, 240);
                }
                else if (proj.type == ProjectileID.CultistBossLightningOrbArc)
                {
					int deathModeDuration = NPC.downedMoonlord ? 80 : NPC.downedPlantBoss ? 40 : Main.hardMode ? 20 : 10;
					player.AddBuff(BuffID.Electrified, proj.Calamity().lineColor == 1 ? deathModeDuration : 120);
                    // Scaled duration for DM lightning, 2 seconds for Storm Weaver/Cultist lightning
                }
				else if (proj.type == ProjectileID.AncientDoomProjectile)
				{
					player.AddBuff(ModContent.BuffType<Horror>(), 180);
				}
			}
			if (CalamityMod.projectileDestroyExceptionList.TrueForAll(x => proj.type != x))
			{
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
					if (proj.type == ProjectileID.BulletSnowman || proj.type == ProjectileID.BulletDeadeye || proj.type == ProjectileID.SniperBullet || proj.type == ProjectileID.VortexLaser)
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
        }
        #endregion

        #region Can Hit
        public override bool? CanHitNPC(Item item, NPC target)
        {
            if (camper && !player.StandingStill())
            {
                return false;
            }
            return null;
        }

        public override bool? CanHitNPCWithProj(Projectile proj, NPC target)
        {
            if (camper && !player.StandingStill())
            {
                return false;
            }
            return null;
        }
        #endregion

        #region Fishing
        public override void CatchFish(Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk)
        {
            CalamityPlayerFishing.CalamityCatchFish(player, ref fishingRod, ref bait, ref power, ref liquidType, ref poolSize, ref worldLayer, ref questFish, ref caughtType, ref junk);
        }

        public override void GetFishingLevel(Item fishingRod, Item bait, ref int fishingLevel)
        {
            CalamityPlayerFishing.CalamityGetFishingLevel(player, ref fishingRod, ref bait, ref fishingLevel);
        }
        #endregion

        #region Shoot

        public override bool Shoot(Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (veneratedLocket)
            {
                if (item.Calamity().rogue && item.type != ModContent.ItemType<SylvanSlasher>())
                {
                    float num72 = item.shootSpeed;
                    Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
                    float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
                    float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
                    if (player.gravDir == -1f)
                    {
                        num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
                    }
                    float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
                    if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
                    {
                        num78 = (float)player.direction;
                        num79 = 0f;
                        num80 = num72;
                    }
                    else
                    {
                        num80 = num72 / num80;
                    }

                    vector2 = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                    vector2.X = (vector2.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                    vector2.Y -= 100f;
                    num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
                    num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
                    if (num79 < 0f)
                    {
                        num79 *= -1f;
                    }
                    if (num79 < 20f)
                    {
                        num79 = 20f;
                    }
                    num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
                    num80 = num72 / num80;
                    num78 *= num80;
                    num79 *= num80;
                    float speedX4 = num78 + (float)Main.rand.Next(-30, 31) * 0.02f;
                    float speedY5 = num79 + (float)Main.rand.Next(-30, 31) * 0.02f;
                    int p = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, type, (int)(damage * 0.15f), knockBack * 0.5f, player.whoAmI);
                    Main.projectile[p].Calamity().forceRogue = true; //in case melee/rogue variants bug out
					if (item.type == ModContent.ItemType<FinalDawn>())
					{
						Main.projectile[p].ai[1] = 1f;
					}
                    if (StealthStrikeAvailable())
                    {
                        int knifeCount = 15;
                        int knifeDamage = (int)(150 * player.RogueDamage());
                        float angleStep = MathHelper.TwoPi / knifeCount;
                        float speed = 15f;

                        for (int i = 0; i < knifeCount; i++)
                        {
                            Vector2 velocity = new Vector2(0f, speed);
                            velocity = velocity.RotatedBy(angleStep * i);
                            int knifeCol = Main.rand.Next(0, 2);

                            Projectile.NewProjectile(player.Center, velocity, ModContent.ProjectileType<VeneratedKnife>(), knifeDamage, 0f, player.whoAmI, knifeCol, 0);
                        }
                    }
                }
            }

            if (rustyMedal)
            {
                if (item.ranged)
                {
                    if (Main.rand.NextBool(5))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 startingPosition = Main.MouseWorld - Vector2.UnitY.RotatedByRandom(0.4f) * 1250f;
                            Vector2 directionToMouse = (startingPosition - Main.MouseWorld).SafeNormalize(Vector2.UnitY).RotatedByRandom(0.1f);
                            Projectile.NewProjectileDirect(startingPosition, directionToMouse * 12f, ModContent.ProjectileType<ToxicannonDrop>(), (int)(damage * 0.3f), 0f, player.whoAmI).penetrate = 2;
                        }
                    }
                }
            }

            return true;
        }
        #endregion

		#region Frame Effects
		public override void FrameEffects()
		{
			if (player.Calamity().andromedaState == AndromedaPlayerState.LargeRobot ||
				player.Calamity().andromedaState == AndromedaPlayerState.SpecialAttack)
			{
				player.head = mod.GetEquipSlot("NoHead", EquipType.Head); // To make the head invisible on the map. The map was having a hissy fit because of hitbox changes.
			}
			else if ((profanedCrystal || profanedCrystalForce) && !profanedCrystalHide)
			{
				player.legs = mod.GetEquipSlot("ProviLegs", EquipType.Legs);
				player.body = mod.GetEquipSlot("ProviBody", EquipType.Body);
				player.head = mod.GetEquipSlot("ProviHead", EquipType.Head);
				player.wings = mod.GetEquipSlot("ProviWings", EquipType.Wings);
				player.face = -1;

				bool enrage = !profanedCrystalForce && profanedCrystalBuffs && player.statLife <= (int)(player.statLifeMax2 * 0.5);

				if (profanedCrystalWingCounter.Value == 0)
				{
					int key = profanedCrystalWingCounter.Key;
					profanedCrystalWingCounter = new KeyValuePair<int, int>(key == 3 ? 0 : key + 1, enrage ? 5 : 7);
				}

				player.wingFrame = profanedCrystalWingCounter.Key;
				profanedCrystalWingCounter = new KeyValuePair<int, int>(profanedCrystalWingCounter.Key, profanedCrystalWingCounter.Value - 1);
				player.armorEffectDrawOutlines = true;
				if (profanedCrystalBuffs)
				{
					player.armorEffectDrawShadow = true;
					if (enrage)
					{
						player.armorEffectDrawOutlinesForbidden = true;
					}
				}
			}
			else if ((snowmanPower || snowmanForce) && !snowmanHide)
			{
				player.legs = mod.GetEquipSlot("PopoLeg", EquipType.Legs);
				player.body = mod.GetEquipSlot("PopoBody", EquipType.Body);
				player.head = snowmanNoseless ? mod.GetEquipSlot("PopoNoselessHead", EquipType.Head) : mod.GetEquipSlot("PopoHead", EquipType.Head);
				player.face = -1;
			}
			else if ((abyssalDivingSuitPower || abyssalDivingSuitForce) && !abyssalDivingSuitHide)
			{
				player.legs = mod.GetEquipSlot("AbyssalDivingSuitLeg", EquipType.Legs);
				player.body = mod.GetEquipSlot("AbyssalDivingSuitBody", EquipType.Body);
				player.head = mod.GetEquipSlot("AbyssalDivingSuitHead", EquipType.Head);
				player.face = -1;
			}
			else if ((sirenBoobsPower || sirenBoobsForce) && !sirenBoobsHide)
			{
				player.legs = mod.GetEquipSlot("SirenLeg", EquipType.Legs);
				player.body = mod.GetEquipSlot("SirenBody", EquipType.Body);
				player.head = mod.GetEquipSlot("SirenHead", EquipType.Head);
				player.face = -1;
			}
            else if (meldTransformationPower || meldTransformationForce)
            {
                player.legs = mod.GetEquipSlot("MeldTransformationLegs", EquipType.Legs);
                player.body = mod.GetEquipSlot("MeldTransformationBody", EquipType.Body);
                player.head = mod.GetEquipSlot("MeldTransformationHead", EquipType.Head);
            }
			else
			{
				if (profanedCrystalWingCounter.Key != 1)
					profanedCrystalWingCounter = new KeyValuePair<int, int>(1, 7);
				if (profanedCrystalAnimCounter.Key != 0)
					profanedCrystalAnimCounter = new KeyValuePair<int, int>(0, 10);
			}
			if (snowRuffianSet)
			{
				player.wings = mod.GetEquipSlot("SnowRuffWings", EquipType.Wings);
				bool falling = player.gravDir == -1 ? player.velocity.Y < 0.05f : player.velocity.Y > 0.05f;
				if (player.controlJump && falling)
				{
					player.velocity.Y *= 0.9f;
					player.wingFrame = 3;
					player.noFallDmg = true;
					player.fallStart = (int)(player.position.Y / 16f);
				}
			}
			if (abyssDivingGear && (player.head == -1 || player.head == ArmorIDs.Head.FamiliarWig))
			{
				player.head = mod.GetEquipSlot("AbyssDivingGearHead", EquipType.Head);
				player.face = -1;
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
			player.doubleJumpSail = false;
			player.doubleJumpFart = false;
			statigelJump = false;
			sulfurJump = false;
            player.rocketBoots = 0;
            player.jumpBoost = false;
            player.slowFall = false;
            player.gravControl = false;
            player.gravControl2 = false;
            player.jumpSpeedBoost = 0f;
            player.wingTimeMax = (int)(player.wingTimeMax * 0.5);
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
                    if (CalamityWorld.DoGSecondStageCountdown > 0)
                    {
                        CalamityWorld.DoGSecondStageCountdown = 0;
                        if (Main.netMode == NetmodeID.Server)
                        {
                            var netMessage = mod.GetPacket();
                            netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                            netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                            netMessage.Send();
                        }
                    }
                    KillPlayer();
                }
            }

            if (lol || (invincible && player.ActiveItem().type != ModContent.ItemType<ColdheartIcicle>()))
            {
                return false;
            }
            if (godSlayerReflect && Main.rand.NextBool(50))
            {
                return false;
            }
            if (hurtSoundTimer == 0) //hurtsounds
            {
                if ((profanedCrystal || profanedCrystalForce) && !profanedCrystalHide)
                {
                    playSound = false;
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/ProvidenceHurt"), (int)player.position.X, (int)player.position.Y);
                    hurtSoundTimer = 20;
                }
                else if ((abyssalDivingSuitPower || abyssalDivingSuitForce) && !abyssalDivingSuitHide)
                {
                    playSound = false;
                    Main.PlaySound(SoundID.NPCHit, (int)player.position.X, (int)player.position.Y, 4, 1f, 0f); //metal hit noise
                    hurtSoundTimer = 10;
                }
                else if ((sirenBoobsPower || sirenBoobsForce) && !sirenBoobsHide)
                {
                    playSound = false;
                    Main.PlaySound(SoundID.FemaleHit, (int)player.position.X, (int)player.position.Y, 1, 1f, 0f); //female hit noise
                    hurtSoundTimer = 10;
                }
				else if (titanHeartSet)
				{
					playSound = false;
					Terraria.Audio.LegacySoundStyle atlasHurt = Utils.SelectRandom(Main.rand, new Terraria.Audio.LegacySoundStyle[]
					{
						mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AtlasHurt0"),
						mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AtlasHurt1"),
						mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AtlasHurt2")
					});
					Main.PlaySound(atlasHurt, (int)player.position.X, (int)player.position.Y);
					hurtSoundTimer = 10;
				}
            }

            #region MultiplierBoosts
            double damageMult = 1.0 +
                (dArtifact ? 0.25 : 0.0) +
                (DoGLore ? 0.1 : 0.0) +
                ((player.beetleDefense && player.beetleOrbs > 0) ? (0.05 * player.beetleOrbs) : 0.0) +
                (enraged ? 0.25 : 0.0) +
                ((CalamityWorld.defiled && Main.rand.NextBool(4)) ? 0.5 : 0.0);

			if (bloodPact && Main.rand.NextBool(4))
			{
				player.AddBuff(ModContent.BuffType<BloodyBoost>(), 600);
				damageMult += 1.25;
			}

            // Equivalent to reducing the player's DR by 20% because they have Cursed Inferno.
			if (CalamityWorld.revenge && player.onFire2)
				damageMult += 0.2;

            damage = (int)(damage * damageMult);
            #endregion

            if (CalamityWorld.revenge)
            {
                customDamage = true;

				double defenseMultiplier = /*Main.masterMode ? 1D :*/ 0.75;
                double newDamage = damage - (player.statDefense * defenseMultiplier);
				double bossDamageLimitIncrease = CalamityWorld.death ? 40D : 20D; // dude why would you even do that? super uncool fab, 0/10, you should jump off a bridge for having the audacity to do something like this :(((((
				double newDamageLimit = NPC.downedMoonlord ? 20D : (NPC.downedPlantBoss || CalamityWorld.downedCalamitas) ? 15D : Main.hardMode ? 10D : 5D;
				/*if (areThereAnyDamnBosses && Main.masterMode)
					newDamageLimit += bossDamageLimitIncrease;*/

                if (newDamage < newDamageLimit)
                {
                    newDamage = newDamageLimit;
                }
                damage = (int)newDamage;
            }

			if (CalamityWorld.ironHeart)
			{
				int damageMin = 60 + (player.statLifeMax2 / 10);
				playSound = false;
				hurtSoundTimer = 20;
				if (damage <= damageMin)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/IronHeartHurt"), (int)player.position.X, (int)player.position.Y);
				else
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/IronHeartBigHurt"), (int)player.position.X, (int)player.position.Y);
			}

			if (purpleCandle)
				damage = (int)(damage - (player.statDefense * 0.05));

			if ((godSlayerDamage && damage <= 80) || damage < 1)
                damage = 1;

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
                if (CalamityConfig.Instance.Rippers && CalamityWorld.revenge)
                {
                    if (!adrenalineModeActive && damage > 0) //to prevent paladin's shield ruining adren even with 0 dmg taken
					{
                        adrenaline -= stressPills ? adrenalineMax / 2 : adrenalineMax;
						if (adrenaline < 0)
							adrenaline = 0;
					}
                }
                if (amidiasBlessing)
                {
                    player.ClearBuff(ModContent.BuffType<AmidiasBlessing>());
                    Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 96);
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
                        Main.PlaySound(SoundID.NPCKilled, (int)player.position.X, (int)player.position.Y, 14);
                        player.AddBuff(ModContent.BuffType<AbyssalDivingSuitPlatesBroken>(), 10830);
                        for (int d = 0; d < 20; d++)
                        {
                            int dust = Dust.NewDust(player.position, player.width, player.height, 31, 0f, 0f, 100, default, 2f);
                            Main.dust[dust].velocity *= 3f;
                            if (Main.rand.NextBool(2))
                            {
                                Main.dust[dust].scale = 0.5f;
                                Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                            }
                        }
                        for (int d = 0; d < 35; d++)
                        {
                            int fire = Dust.NewDust(player.position, player.width, player.height, DustID.Fire, 0f, 0f, 100, default, 3f);
                            Main.dust[fire].noGravity = true;
                            Main.dust[fire].velocity *= 5f;
                            fire = Dust.NewDust(player.position, player.width, player.height, DustID.Fire, 0f, 0f, 100, default, 2f);
                            Main.dust[fire].velocity *= 2f;
                        }
						CalamityUtils.ExplosionGores(player.Center, 3);
                    }
                }
                if (sirenIce)
                {
                    Main.PlaySound(SoundID.NPCKilled, (int)player.Center.X, (int)player.Center.Y, 7);
                    player.AddBuff(ModContent.BuffType<IceShieldBrokenBuff>(), 1800);
                    for (int d = 0; d < 10; d++)
                    {
                        int ice = Dust.NewDust(player.position, player.width, player.height, 67, 0f, 0f, 100, default, 2f);
                        Main.dust[ice].velocity *= 3f;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[ice].scale = 0.5f;
                            Main.dust[ice].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int d = 0; d < 15; d++)
                    {
                        int ice = Dust.NewDust(player.position, player.width, player.height, 67, 0f, 0f, 100, default, 3f);
                        Main.dust[ice].noGravity = true;
                        Main.dust[ice].velocity *= 5f;
                        ice = Dust.NewDust(player.position, player.width, player.height, 67, 0f, 0f, 100, default, 2f);
                        Main.dust[ice].velocity *= 2f;
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
                if (fBarrier || (sirenBoobs && NPC.downedBoss3))
                {
                    Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 27);
                    for (int m = 0; m < Main.maxNPCs; m++)
                    {
						NPC npc = Main.npc[m];
						if (!npc.active || npc.friendly || npc.dontTakeDamage)
							continue;
						float npcDist = (npc.Center - player.Center).Length();
						float freezeDist = (float)Main.rand.Next(200 + (int)damage / 2, 301 + (int)damage * 2);
						if (freezeDist > 500f)
						{
							freezeDist = 500f + (freezeDist - 500f) * 0.75f;
						}
						if (freezeDist > 700f)
						{
							freezeDist = 700f + (freezeDist - 700f) * 0.5f;
						}
						if (freezeDist > 900f)
						{
							freezeDist = 900f + (freezeDist - 900f) * 0.25f;
						}
						if (npcDist < freezeDist)
						{
							float duration = (float)Main.rand.Next(90 + (int)damage / 3, 240 + (int)damage / 2);
							npc.AddBuff(ModContent.BuffType<GlacialState>(), (int)duration, false);
						}
                    }
                }
                if (aBrain || amalgam)
                {
                    for (int m = 0; m < Main.maxNPCs; m++)
                    {
						NPC npc = Main.npc[m];
						if (!npc.active || npc.friendly || npc.dontTakeDamage)
							continue;
						float npcDist = (npc.Center - player.Center).Length();
						float range = (float)Main.rand.Next(200 + (int)damage / 2, 301 + (int)damage * 2);
						if (range > 500f)
						{
							range = 500f + (range - 500f) * 0.75f;
						}
						if (range > 700f)
						{
							range = 700f + (range - 700f) * 0.5f;
						}
						if (range > 900f)
						{
							range = 900f + (range - 900f) * 0.25f;
						}
						if (npcDist < range)
						{
							float duration = (float)Main.rand.Next(90 + (int)damage / 3, 300 + (int)damage / 2);
							npc.AddBuff(BuffID.Confused, (int)duration, false);
							if (amalgam)
							{
								npc.AddBuff(ModContent.BuffType<BrimstoneFlames>(), (int)duration, false);
								npc.AddBuff(ModContent.BuffType<GodSlayerInferno>(), (int)duration, false);
								npc.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), (int)duration, false);
								npc.AddBuff(ModContent.BuffType<Irradiated>(), (int)duration, false);
							}
						}
                    }
					//Spawn the harmless brain images that are actually projectiles
                    Projectile.NewProjectile(player.Center.X + (float)Main.rand.Next(-40, 40), player.Center.Y - (float)Main.rand.Next(20, 60), player.velocity.X * 0.3f, player.velocity.Y * 0.3f, ProjectileID.BrainOfConfusion, 0, 0f, player.whoAmI, 0f, 0f);
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
            if (player.ownedProjectileCounts[ModContent.ProjectileType<DrataliornusBow>()] != 0)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<DrataliornusBow>() && Main.projectile[i].owner == player.whoAmI)
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
            if (!profanedCrystal && pArtifact)
            {
                player.AddBuff(ModContent.BuffType<BurntOut>(), 300, true);
            }
            bool hardMode = Main.hardMode;
            int iFramesToAdd = 0;
            if (player.whoAmI == Main.myPlayer)
            {
                if (cTracers && damage > 200)
                {
                    iFramesToAdd += 60;
                }
                if (godSlayerThrowing && damage > 80)
                {
                    iFramesToAdd += 30;
                }
                if (statigelSet && damage > 100)
                {
                    iFramesToAdd += 30;
                }
                if (dAmulet)
                {
                    if (damage == 1.0)
                    {
                        iFramesToAdd += 10;
                    }
                    else
                    {
                        iFramesToAdd += 20;
                    }
                }
                if (fabsolVodka)
                {
                    if (damage == 1.0)
                    {
                        iFramesToAdd += 5;
                    }
                    else
                    {
                        iFramesToAdd += 10;
                    }
                }
                if (CalamityWorld.bossRushActive && CalamityConfig.Instance.BossRushImmunityFrameCurse)
                {
                    bossRushImmunityFrameCurseTimer = 300 + player.immuneTime;
                }
                if (damage > 25)
                {
                    if (aeroSet)
                    {
                        for (int n = 0; n < 4; n++)
                        {
							CalamityUtils.ProjectileRain(player.Center, 400f, 100f, 500f, 800f, 20f, ModContent.ProjectileType<StickyFeatherAero>(), (int)(20 * player.AverageDamage()), 1f, player.whoAmI);
                        }
                    }
                }
                if (aBulwark)
                {
                    if (aBulwarkRare)
                    {
                        Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 74);
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<GodSlayerBlaze>(), (int)(25 * player.AverageDamage()), 5f, player.whoAmI, 0f, 1f);
                    }
                    int starAmt = aBulwarkRare ? 12 : 5;
                    for (int n = 0; n < starAmt; n++)
                    {
						CalamityUtils.ProjectileRain(player.Center, 400f, 100f, 500f, 800f, 29f, ModContent.ProjectileType<AstralStar>(), (int)(320 * player.AverageDamage()), 5f, player.whoAmI);
                    }
                }
                if (dAmulet)
                {
                    for (int n = 0; n < 3; n++)
                    {
						CalamityUtils.ProjectileRain(player.Center, 400f, 100f, 500f, 800f, 29f, ProjectileID.HallowStar, (int)(130 * player.AverageDamage()), 4f, player.whoAmI, 6, 1, 5);
                    }

                    /*int num = 1;
					if (Main.rand.NextBool(3))
						++num;
					if (Main.rand.NextBool(3))
						++num;
					if (player.strongBees && Main.rand.NextBool(3))
						++num;
					for (int index = 0; index < num; ++index)
					{
						int bee = Projectile.NewProjectile(player.position.X, player.position.Y, (float) Main.rand.Next(-35, 36) * 0.02f, (float) Main.rand.Next(-35, 36) * 0.02f, player.beeType(), player.beeDamage(7), player.beeKB(0f), Main.myPlayer, 0f, 0f);
                        Main.projectile[bee].usesLocalNPCImmunity = true;
                        Main.projectile[bee].localNPCHitCooldown = 5;
					}*/
                }
                if (theBee)
                {
                    for (int n = 0; n < 3; n++)
                    {
						CalamityUtils.ProjectileRain(player.Center, 400f, 100f, 500f, 800f, 29f, ProjectileID.HallowStar, (int)(150 * player.AverageDamage()), 4f, player.whoAmI, 6, 1, 5);
                    }
                    int num = 1;
                    if (Main.rand.NextBool(3))
                        ++num;
                    if (Main.rand.NextBool(3))
                        ++num;
                    if (player.strongBees && Main.rand.NextBool(3))
                        ++num;
                    for (int index = 0; index < num; ++index)
                    {
                        int bee = Projectile.NewProjectile(player.position.X, player.position.Y, (float) Main.rand.Next(-35, 36) * 0.02f, (float) Main.rand.Next(-35, 36) * 0.02f, (Main.rand.NextBool(4) ? ModContent.ProjectileType<PlaguenadeBee>() : player.beeType()), player.beeDamage(7), player.beeKB(0f), Main.myPlayer, 0f, 0f);
                        Main.projectile[bee].usesLocalNPCImmunity = true;
                        Main.projectile[bee].localNPCHitCooldown = 5;
                    }
                }
            }
            if (fCarapace)
            {
                if (damage > 0)
                {
                    Main.PlaySound(SoundID.NPCHit, (int)player.position.X, (int)player.position.Y, 45);
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    int fDamage = (int)(56 * player.AverageDamage());
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            float xPos = Main.rand.NextBool(2) ? player.Center.X + 100 : player.Center.X - 100;
                            Vector2 spawnPos = new Vector2(xPos, player.Center.Y + Main.rand.Next(-100, 101));
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            int spore1 = Projectile.NewProjectile(spawnPos.X, spawnPos.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ProjectileID.TruffleSpore, fDamage, 1.25f, player.whoAmI, 0f, 0f);
                            int spore2 = Projectile.NewProjectile(spawnPos.X, spawnPos.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ProjectileID.TruffleSpore, fDamage, 1.25f, player.whoAmI, 0f, 0f);
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
                    Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 93);
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    int sDamage = hardMode ? 36 : 6;
                    if (aSparkRare)
                        sDamage += hardMode ? 12 : 2;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            int spark1 = Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<Spark>(), (int)(sDamage * player.AverageDamage()), 1.25f, player.whoAmI, 0f, 0f);
                            int spark2 = Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<Spark>(), (int)(sDamage * player.AverageDamage()), 1.25f, player.whoAmI, 0f, 0f);
                            Main.projectile[spark1].timeLeft = 120;
                            Main.projectile[spark2].timeLeft = 120;
                            Main.projectile[spark1].Calamity().forceTypeless = true;
                            Main.projectile[spark2].Calamity().forceTypeless = true;
                        }
                    }
                }
            }
            if (inkBomb && !abyssalMirror && !eclipseMirror)
            {
                if (player.whoAmI == Main.myPlayer && !inkBombCooldown)
                {
                    player.AddBuff(ModContent.BuffType<InkBombCooldown>(), 1200);
                    rogueStealth += 0.5f;
                    for (int i = 0; i < 5; i++)
                    {
                        Main.PlaySound(SoundID.Item, (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y, 61);
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-0f, -4f), ModContent.ProjectileType<InkBombProjectile>(), 0, 0, player.whoAmI);
                    }
                }
            }
            if (blazingCore)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<BlazingSun>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<BlazingSun2>()] < 1)
                {
                    for (int i = 0; i < 360; i += 3)
                    {
                        Vector2 BCDSpeed = new Vector2(5f, 5f).RotatedBy(MathHelper.ToRadians(i));
                        Dust.NewDust(player.Center, 1, 1, 244, BCDSpeed.X, BCDSpeed.Y, 0, default, 1.1f);
                    }
                    Main.PlaySound(SoundID.Item14, player.Center);
                    int blazingSun = Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<BlazingSun>(), (int)(1690 * player.AverageDamage()), 0f, player.whoAmI, 0f, 0f);
                    Main.projectile[blazingSun].Center = player.Center;
                    int blazingSun2 = Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<BlazingSun2>(), 0, 0f, player.whoAmI, 0f, 0f);
                    Main.projectile[blazingSun2].Center = player.Center;
                }
            }
            if (ataxiaBlaze && Main.rand.NextBool(5))
            {
                if (damage > 0)
                {
                    Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 74);
                    int eDamage = (int)(100 * player.AverageDamage());
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<ChaosBlaze>(), eDamage, 1f, player.whoAmI, 0f, 0f);
                    }
                }
            }
            else if (daedalusShard)
            {
                if (damage > 0)
                {
                    Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 27);
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    int sDamage = (int)(27 * player.RangedDamage()); //daedalus ranged helm
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            float randomSpeed = (float)Main.rand.Next(1, 7);
                            float randomSpeed2 = (float)Main.rand.Next(1, 7);
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            int shard = Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f) + randomSpeed, ProjectileID.CrystalShard, sDamage, 1f, player.whoAmI, 0f, 0f);
                            int shard2 = Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f) + randomSpeed2, ProjectileID.CrystalShard, sDamage, 1f, player.whoAmI, 0f, 0f);
                            Main.projectile[shard].Calamity().forceTypeless = true;
                            Main.projectile[shard2].Calamity().forceTypeless = true;
                        }
                    }
                }
            }
            else if (reaverSpore)
            {
                if (damage > 0)
                {
                    Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 1);
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    int rDamage = (int)(58 * player.RogueDamage()); //Reaver rogue helm
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            float xPos = Main.rand.NextBool(2) ? player.Center.X + 100 : player.Center.X - 100;
                            Vector2 spawnPos = new Vector2(xPos, player.Center.Y + Main.rand.Next(-100, 101));
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            int rspore1 = Projectile.NewProjectile(spawnPos.X, spawnPos.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<ReaverSpore>(), rDamage, 2f, player.whoAmI, 0f, 0f);
                            Main.projectile[rspore1].usesLocalNPCImmunity = true;
                            Main.projectile[rspore1].localNPCHitCooldown = 60;
                            int rspore2 = Projectile.NewProjectile(spawnPos.X, spawnPos.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<ReaverSpore>(), rDamage, 2f, player.whoAmI, 1f, 0f);
                            Main.projectile[rspore2].usesLocalNPCImmunity = true;
                            Main.projectile[rspore2].localNPCHitCooldown = 60;
                        }
                    }
                }
            }
            else if (godSlayerDamage) //god slayer melee helm
            {
                if (damage > 80)
                {
                    Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 73);
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<GodKiller>(), (int)(900 * player.MeleeDamage()), 5f, player.whoAmI, 0f, 0f);
                            Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<GodKiller>(), (int)(900 * player.MeleeDamage()), 5f, player.whoAmI, 0f, 0f);
                        }
                    }
                }
            }
            else if (godSlayerMage)
            {
                if (damage > 0)
                {
                    Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 74);
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<GodSlayerBlaze>(), (int)((auricSet ? 2400 : 1200) * player.MagicDamage()), 1f, player.whoAmI, 0f, 0f);
                    }
                }
            }
            else if (dsSetBonus)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    for (int l = 0; l < 2; l++)
                    {
						CalamityUtils.ProjectileRain(player.Center, 400f, 100f, 500f, 800f, 22f, ProjectileID.ShadowBeamFriendly, (int)(3000 * player.AverageDamage()), 7f, player.whoAmI, 6, 1);
                    }
                    for (int l = 0; l < 5; l++)
                    {
						CalamityUtils.ProjectileRain(player.Center, 400f, 100f, 500f, 800f, 22f, ProjectileID.DemonScythe, (int)(5000 * player.AverageDamage()), 7f, player.whoAmI, 6, 1);
                    }
                }
            }
            if (lastProjectileHit != null)
            {
                switch (lastProjectileHit.modProjectile.cooldownSlot)
                {
                    case 0:
                    case 1:
                        player.hurtCooldowns[lastProjectileHit.modProjectile.cooldownSlot] += iFramesToAdd;
                        break;
                    case -1:
                    default:
                        player.immuneTime += iFramesToAdd;
                        break;
                }
            }
            else
            {
                player.immuneTime += iFramesToAdd;
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
            bool specialDeath = CalamityWorld.ironHeart;
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
                if (player.difficulty == 0)
                {
                    for (int i = 0; i < 59; i++)
                    {
                        if (player.inventory[i].stack > 0 && ((player.inventory[i].type >= ItemID.LargeAmethyst && player.inventory[i].type <= ItemID.LargeDiamond) || player.inventory[i].type == ItemID.LargeAmber))
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
                                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, num, 0f, 0f, 0f, 0, 0, 0);
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
                Main.PlaySound(SoundID.PlayerKilled, (int)player.position.X, (int)player.position.Y, 1, 1f, 0f);
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
                player.respawnTimer = (int)(player.respawnTimer * 1.5);
            }
            player.immuneAlpha = 0;
            player.palladiumRegen = false;
            player.iceBarrier = false;
            player.crystalLeaf = false;

            PlayerDeathReason damageSource = PlayerDeathReason.ByOther(player.Male ? 14 : 15);
            if (abyssDeath)
            {
                if (Main.rand.NextBool(2))
                {
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + " is food for the Wyrms.");
                }
                else
                {
                    damageSource = PlayerDeathReason.ByCustomReason("Oxygen failed to reach " + player.name + " from the depths of the Abyss.");
                }
            }
            else if (specialDeath)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + " was defeated.");
            }
            else if (CalamityWorld.death && deathModeBlizzardTime > 1980)
            {
                deathModeBlizzardTime = 0;
                damageSource = PlayerDeathReason.ByCustomReason(player.name + " was chilled to the bone by the frigid environment.");
            }
            else if (SCalLore)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.Male ? player.name + " was consumed by his inner hatred." : player.name + " was consumed by her inner hatred.");
            }
            else if (CalamityWorld.armageddon && areThereAnyDamnBosses)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + " failed the challenge at hand.");
            }
            else if (CalamityWorld.bossRushActive && bossRushImmunityFrameCurseTimer > 0)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + " was destroyed by a mysterious force.");
            }
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

            if (player.whoAmI == Main.myPlayer)
            {
                try
                {
                    WorldGen.saveToonWhilePlaying();
                }
                catch
                {
                }
            }
        }
        #endregion

        #region Dash Stuff
        public void ModDashMovement()
        {
            if (dashMod == 6 && player.dashDelay < 0 && player.whoAmI == Main.myPlayer) //cryo lore
            {
                Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
					NPC npc = Main.npc[i];
                    if (npc.active && !npc.dontTakeDamage && !npc.friendly && npc.immune[player.whoAmI] <= 0)
                    {
                        Rectangle rect = npc.getRect();
                        if (rectangle.Intersects(rect) && (npc.noTileCollide || player.CanHit(npc)))
                        {
                            float num = 50f * player.AverageDamage();
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
                                player.ApplyDamageToNPC(npc, (int)num, num2, direction, crit);
                            }
							if (npc.immune[player.whoAmI] < 6)
								npc.immune[player.whoAmI] = 6;
                            npc.AddBuff(ModContent.BuffType<GlacialState>(), 300);
                            player.immune = true;
                            player.immuneNoBlink = true;
							if (player.immuneTime < 4)
								player.immuneTime = 4;
							for (int k = 0; k < player.hurtCooldowns.Length; k++)
							{
								player.hurtCooldowns[k] = player.immuneTime;
							}
                        }
                    }
                }
            }
            if (dashMod == 4 && player.dashDelay < 0 && player.whoAmI == Main.myPlayer) //Asgardian Aegis
            {
                Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
					NPC npc = Main.npc[i];
                    if (npc.active && !npc.dontTakeDamage && !npc.friendly && npc.immune[player.whoAmI] <= 0)
                    {
                        Rectangle rect = npc.getRect();
                        if (rectangle.Intersects(rect) && (npc.noTileCollide || player.CanHit(npc)))
                        {
                            float num = 500f * player.AverageDamage();
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
                                player.ApplyDamageToNPC(npc, (int)num, num2, direction, crit);
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<HolyExplosionSupreme>(), (int)(300 * player.AverageDamage()), 20f, Main.myPlayer, 0f, 0f);
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<HolyEruption>(), (int)(200 * player.AverageDamage()), 5f, Main.myPlayer, 0f, 0f);
                            }
							if (npc.immune[player.whoAmI] < 6)
								npc.immune[player.whoAmI] = 6;
                            npc.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);
                            player.immune = true;
                            player.immuneNoBlink = true;
							if (player.immuneTime < 4)
								player.immuneTime = 4;
							for (int k = 0; k < player.hurtCooldowns.Length; k++)
							{
								player.hurtCooldowns[k] = player.immuneTime;
							}
                        }
                    }
                }
            }
            if (dashMod == 3 && player.dashDelay < 0 && player.whoAmI == Main.myPlayer) //Elysian Aegis
            {
                Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
					NPC npc = Main.npc[i];
                    if (npc.active && !npc.dontTakeDamage && !npc.friendly && npc.immune[player.whoAmI] <= 0)
                    {
                        Rectangle rect = npc.getRect();
                        if (rectangle.Intersects(rect) && (npc.noTileCollide || player.CanHit(npc)))
                        {
                            float num = 350f * player.AverageDamage();
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
                                player.ApplyDamageToNPC(npc, (int)num, num2, direction, crit);
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<HolyExplosionSupreme>(), (int)(210 * player.AverageDamage()), 15f, Main.myPlayer, 0f, 0f);
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<HolyEruption>(), (int)(140 * player.AverageDamage()), 5f, Main.myPlayer, 0f, 0f);
                            }
							if (npc.immune[player.whoAmI] < 6)
								npc.immune[player.whoAmI] = 6;
                            player.immune = true;
                            player.immuneNoBlink = true;
							if (player.immuneTime < 4)
								player.immuneTime = 4;
							for (int k = 0; k < player.hurtCooldowns.Length; k++)
							{
								player.hurtCooldowns[k] = player.immuneTime;
							}
                        }
                    }
                }
            }
            if (dashMod == 2 && player.dashDelay < 0 && player.whoAmI == Main.myPlayer) //Asgard's Valor
            {
                Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
					NPC npc = Main.npc[i];
                    if (npc.active && !npc.dontTakeDamage && !npc.friendly && npc.immune[player.whoAmI] <= 0)
                    {
                        Rectangle rect = npc.getRect();
                        if (rectangle.Intersects(rect) && (npc.noTileCollide || player.CanHit(npc)))
                        {
                            float num = 100f * player.AverageDamage();
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
                                player.ApplyDamageToNPC(npc, (int)num, num2, direction, crit);
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<HolyExplosion>(), (int)(60 * player.AverageDamage()), 15f, Main.myPlayer, 0f, 0f);
                            }
							if (npc.immune[player.whoAmI] < 6)
								npc.immune[player.whoAmI] = 6;
                            player.immune = true;
                            player.immuneNoBlink = true;
							if (player.immuneTime < 4)
								player.immuneTime = 4;
							for (int k = 0; k < player.hurtCooldowns.Length; k++)
							{
								player.hurtCooldowns[k] = player.immuneTime;
							}
                        }
                    }
                }
            }
            if (dashMod == 8 && player.dashDelay < 0 && player.whoAmI == Main.myPlayer) //plaguebringer armor
            {
                Rectangle rectangle = new Rectangle((int)(player.position.X + player.velocity.X * 0.5f - 4f), (int)(player.position.Y + player.velocity.Y * 0.5f - 4f), player.width + 8, player.height + 8);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
					NPC npc = Main.npc[i];
                    if (npc.active && !npc.dontTakeDamage && !npc.friendly && npc.immune[player.whoAmI] <= 0)
                    {
                        Rectangle rect = npc.getRect();
                        if (rectangle.Intersects(rect) && (npc.noTileCollide || player.CanHit(npc)))
                        {
                            float num = 50f * player.MinionDamage();
                            float num2 = 3f;
                            bool crit = false;
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
                                player.ApplyDamageToNPC(npc, (int)num, num2, direction, crit);
                            }
							if (npc.immune[player.whoAmI] < 6)
								npc.immune[player.whoAmI] = 6;
                            npc.AddBuff(ModContent.BuffType<Plague>(), 300);
                            player.immune = true;
                            player.immuneNoBlink = true;
							if (player.immuneTime < 4)
								player.immuneTime = 4;
							for (int k = 0; k < player.hurtCooldowns.Length; k++)
							{
								player.hurtCooldowns[k] = player.immuneTime;
							}
                        }
                    }
                }
            }
            if (dashMod == 1 && player.dashDelay < 0 && player.whoAmI == Main.myPlayer) //Counter Scarf
            {
                Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
					NPC npc = Main.npc[i];
                    if (npc.active && !npc.dontTakeDamage && !npc.friendly && !npc.townNPC && npc.immune[player.whoAmI] <= 0 && npc.damage > 0)
                    {
                        Rectangle rect = npc.getRect();
                        if (rectangle.Intersects(rect) && (npc.noTileCollide || player.CanHit(npc)))
                        {
                            OnDodge();
                            break;
                        }
                    }
                }
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
					Projectile proj = Main.projectile[i];
                    if (proj.active && !proj.friendly && proj.hostile && proj.damage > 0)
                    {
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
                int delay = 20;
				if (dashMod == 1) //Counter Scarf
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
                else if (dashMod == 2) //Asgard's Valor
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
                else if (dashMod == 3) //Elysian Aegis
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
                else if (dashMod == 4) //Asgardian Aegis
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
                else if (dashMod == 5) //Deep Diver
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
                else if (dashMod == 6) //Cryogen Lore
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
					delay = 30;
                }
                else if (dashMod == 7) //Statis' Belt of Curses
                {
					statisTimer++;
                    for (int k = 0; k < 2; k++)
                    {
                        int num12;
                        if (player.velocity.Y == 0f)
                        {
                            num12 = Dust.NewDust(new Vector2(player.position.X, player.position.Y + (float)player.height - 4f), player.width, 8, 70, 0f, 0f, 100, default, 1.4f);
                        }
                        else
                        {
                            num12 = Dust.NewDust(new Vector2(player.position.X, player.position.Y + (float)(player.height / 2) - 8f), player.width, 16, 70, 0f, 0f, 100, default, 1.4f);
                        }
                        Main.dust[num12].velocity *= 0.1f;
                        Main.dust[num12].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                        Main.dust[num12].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    num7 = 14f; //14
					if (statisTimer % 5 == 0)
					{
						int scythe = Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<CosmicScythe>(), (int)(500 * player.AverageDamage()), 5f, player.whoAmI);
						Main.projectile[scythe].Calamity().forceTypeless = true;
						Main.projectile[scythe].usesIDStaticNPCImmunity = true;
						Main.projectile[scythe].idStaticNPCHitCooldown = 10;
					}
                }
                else if (dashMod == 8) //Plaguebringer armor
                {
                    for (int m = 0; m < 24; m++)
                    {
                        int num14 = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 4f), player.width, player.height - 8, 89, 0f, 0f, 100, default, 1f);
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
                    player.dashDelay = delay;
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
                float dashDistance;
                if (dashMod == 1) //Counter and Evasion Scarf
                {
                    dashDistance = evasionScarf ? 16.3f : 14.5f;
                    int direction = 0;
                    bool justDashed = false;
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
                            direction = 1;
                            justDashed = true;
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
                            direction = -1;
                            justDashed = true;
                            dashTimeMod = 0;
                        }
                        else
                        {
                            dashTimeMod = -15;
                        }
                    }
                    if (justDashed)
                    {
                        player.velocity.X = dashDistance * (float)direction; //eoc dash amount (evasion = asgard's)
                        Point point = (player.Center + new Vector2((float)(direction * player.width / 2 + 2), player.gravDir * (float)-(float)player.height / 2f + player.gravDir * 2f)).ToTileCoordinates();
                        Point point2 = (player.Center + new Vector2((float)(direction * player.width / 2 + 2), 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point.X, point.Y) || WorldGen.SolidOrSlopedTile(point2.X, point2.Y))
                        {
                            player.velocity.X = player.velocity.X / 2f;
                        }
                        player.dashDelay = -1;
                        for (int num17 = 0; num17 < 20; num17++)
                        {
                            int num18 = Dust.NewDust(player.position, player.width, player.height, 235, 0f, 0f, 100, default, 2f);
                            Dust dust = Main.dust[num18];
                            dust.position.X += (float)Main.rand.Next(-5, 6);
                            dust.position.Y += (float)Main.rand.Next(-5, 6);
                            dust.velocity *= 0.2f;
                            dust.scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                            dust.shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                        }
                        return;
                    }
                }
                else if (dashMod == 2) //Asgard's Valor
                {
                    dashDistance = 16.9f;
                    int direction = 0;
                    bool justDashed = false;
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
                            direction = 1;
                            justDashed = true;
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
                            direction = -1;
                            justDashed = true;
                            dashTimeMod = 0;
                        }
                        else
                        {
                            dashTimeMod = -15;
                        }
                    }
                    if (justDashed)
                    {
                        player.velocity.X = dashDistance * (float)direction; //tabi dash amount
                        Point point5 = (player.Center + new Vector2((float)(direction * player.width / 2 + 2), player.gravDir * (float)-(float)player.height / 2f + player.gravDir * 2f)).ToTileCoordinates();
                        Point point6 = (player.Center + new Vector2((float)(direction * player.width / 2 + 2), 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point5.X, point5.Y) || WorldGen.SolidOrSlopedTile(point6.X, point6.Y))
                        {
                            player.velocity.X = player.velocity.X / 2f;
                        }
                        player.dashDelay = -1;
                        for (int num24 = 0; num24 < 20; num24++)
                        {
                            int num25 = Dust.NewDust(player.position, player.width, player.height, 246, 0f, 0f, 100, default, 3f);
                            Dust dust = Main.dust[num25];
                            dust.position.X += (float)Main.rand.Next(-5, 6);
                            dust.position.Y += (float)Main.rand.Next(-5, 6);
                            dust.velocity *= 0.2f;
                            dust.scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                            dust.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                            dust.noGravity = true;
                            dust.fadeIn = 0.5f;
                        }
                    }
                }
                else if (dashMod == 3) //Elysian Aegis
                {
                    dashDistance = 21.9f;
                    int direction = 0;
                    bool justDashed = false;
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
                            direction = 1;
                            justDashed = true;
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
                            direction = -1;
                            justDashed = true;
                            dashTimeMod = 0;
                        }
                        else
                        {
                            dashTimeMod = -15;
                        }
                    }
                    if (justDashed)
                    {
                        player.velocity.X = dashDistance * (float)direction; //solar dash amount
                        Point point5 = (player.Center + new Vector2((float)(direction * player.width / 2 + 2), player.gravDir * (float)-(float)player.height / 2f + player.gravDir * 2f)).ToTileCoordinates();
                        Point point6 = (player.Center + new Vector2((float)(direction * player.width / 2 + 2), 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point5.X, point5.Y) || WorldGen.SolidOrSlopedTile(point6.X, point6.Y))
                        {
                            player.velocity.X = player.velocity.X / 2f;
                        }
                        player.dashDelay = -1;
                        for (int num24 = 0; num24 < 40; num24++)
                        {
                            int num25 = Dust.NewDust(player.position, player.width, player.height, 244, 0f, 0f, 100, default, 3f);
                            Dust dust = Main.dust[num25];
                            dust.position.X += (float)Main.rand.Next(-5, 6);
                            dust.position.Y += (float)Main.rand.Next(-5, 6);
                            dust.velocity *= 0.2f;
                            dust.scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                            dust.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                            dust.noGravity = true;
                            dust.fadeIn = 0.5f;
                        }
                    }
                }
                else if (dashMod == 4) //Asgardian Aegis
                {
                    dashDistance = 23.9f;
                    int direction = 0;
                    bool justDashed = false;
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
                            direction = 1;
                            justDashed = true;
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
                            direction = -1;
                            justDashed = true;
                            dashTimeMod = 0;
                        }
                        else
                        {
                            dashTimeMod = -15;
                        }
                    }
                    if (justDashed)
                    {
                        player.velocity.X = dashDistance * (float)direction; //slighty more powerful solar dash
                        Point point5 = (player.Center + new Vector2((float)(direction * player.width / 2 + 2), player.gravDir * (float)-(float)player.height / 2f + player.gravDir * 2f)).ToTileCoordinates();
                        Point point6 = (player.Center + new Vector2((float)(direction * player.width / 2 + 2), 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point5.X, point5.Y) || WorldGen.SolidOrSlopedTile(point6.X, point6.Y))
                        {
                            player.velocity.X = player.velocity.X / 2f;
                        }
                        player.dashDelay = -1;
                        for (int num24 = 0; num24 < 60; num24++)
                        {
                            int num25 = Dust.NewDust(player.position, player.width, player.height, 244, 0f, 0f, 100, default, 3f);
                            Dust dust = Main.dust[num25];
                            dust.position.X += (float)Main.rand.Next(-5, 6);
                            dust.position.Y += (float)Main.rand.Next(-5, 6);
                            dust.velocity *= 0.2f;
                            dust.scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                            dust.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                            dust.noGravity = true;
                            dust.fadeIn = 0.5f;
                        }
                    }
                }
                else if (dashMod == 5) //Deep Diver
                {
                    dashDistance = 25.9f;
                    int direction = 0;
                    bool justDashed = false;
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
                            direction = 1;
                            justDashed = true;
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
                            direction = -1;
                            justDashed = true;
                            dashTimeMod = 0;
                        }
                        else
                        {
                            dashTimeMod = -15;
                        }
                    }
                    if (justDashed)
                    {
                        player.velocity.X = dashDistance * (float)direction;
                        Point point5 = (player.Center + new Vector2((float)(direction * player.width / 2 + 2), player.gravDir * (float)-(float)player.height / 2f + player.gravDir * 2f)).ToTileCoordinates();
                        Point point6 = (player.Center + new Vector2((float)(direction * player.width / 2 + 2), 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point5.X, point5.Y) || WorldGen.SolidOrSlopedTile(point6.X, point6.Y))
                        {
                            player.velocity.X = player.velocity.X / 2f;
                        }
                        player.dashDelay = -1;
                        for (int num24 = 0; num24 < 60; num24++)
                        {
                            int num25 = Dust.NewDust(player.position, player.width, player.height, 33, 0f, 0f, 100, default, 3f);
                            Dust dust = Main.dust[num25];
                            dust.position.X += (float)Main.rand.Next(-5, 6);
                            dust.position.Y += (float)Main.rand.Next(-5, 6);
                            dust.velocity *= 0.2f;
                            dust.scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                            dust.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                            dust.noGravity = true;
                            dust.fadeIn = 0.5f;
                        }
                    }
                }
                else if (dashMod == 6) //Cryogen Lore
                {
                    dashDistance = 15.7f;
                    int direction = 0;
                    bool justDashed = false;
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
                            direction = 1;
                            justDashed = true;
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
                            direction = -1;
                            justDashed = true;
                            dashTimeMod = 0;
                        }
                        else
                        {
                            dashTimeMod = -15;
                        }
                    }
                    if (justDashed)
                    {
                        player.velocity.X = dashDistance * (float)direction;
                        Point point5 = (player.Center + new Vector2((float)(direction * player.width / 2 + 2), player.gravDir * (float)-(float)player.height / 2f + player.gravDir * 2f)).ToTileCoordinates();
                        Point point6 = (player.Center + new Vector2((float)(direction * player.width / 2 + 2), 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point5.X, point5.Y) || WorldGen.SolidOrSlopedTile(point6.X, point6.Y))
                        {
                            player.velocity.X = player.velocity.X / 2f;
                        }
                        player.dashDelay = -1;
                        for (int num24 = 0; num24 < 60; num24++)
                        {
                            int num25 = Dust.NewDust(player.position, player.width, player.height, 67, 0f, 0f, 100, default, 1.25f);
                            Dust dust = Main.dust[num25];
                            dust.position.X += (float)Main.rand.Next(-5, 6);
                            dust.position.Y += (float)Main.rand.Next(-5, 6);
                            dust.velocity *= 0.2f;
                            dust.scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                            dust.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                            dust.noGravity = true;
                            dust.fadeIn = 0.5f;
                        }
                    }
                }
                else if (dashMod == 7) //Statis' Belt of Curses
                {
                    dashDistance = 21.9f;
                    int direction = 0;
                    bool justDashed = false;
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
                            direction = 1;
                            justDashed = true;
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
                            direction = -1;
                            justDashed = true;
                            dashTimeMod = 0;
                        }
                        else
                        {
                            dashTimeMod = -15;
                        }
                    }
                    if (justDashed)
                    {
                        player.velocity.X = dashDistance * (float)direction; //solar dash amount
                        Point point = (player.Center + new Vector2((float)(direction * player.width / 2 + 2), player.gravDir * (float)-(float)player.height / 2f + player.gravDir * 2f)).ToTileCoordinates();
                        Point point2 = (player.Center + new Vector2((float)(direction * player.width / 2 + 2), 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point.X, point.Y) || WorldGen.SolidOrSlopedTile(point2.X, point2.Y))
                        {
                            player.velocity.X = player.velocity.X / 2f;
                        }
                        player.dashDelay = -1;
                        for (int num17 = 0; num17 < 20; num17++)
                        {
                            int num18 = Dust.NewDust(player.position, player.width, player.height, 70, 0f, 0f, 100, default, 2f);
                            Dust dust = Main.dust[num18];
                            dust.position.X += (float)Main.rand.Next(-5, 6);
                            dust.position.Y += (float)Main.rand.Next(-5, 6);
                            dust.velocity *= 0.2f;
                            dust.scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                            dust.shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                        }
                        return;
                    }
                }
                else if (dashMod == 8) //Plaguebringer armor
                {
                    dashDistance = 19f;
                    int direction = 0;
                    bool justDashed = false;
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
                            direction = 1;
                            justDashed = true;
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
                            direction = -1;
                            justDashed = true;
                            dashTimeMod = 0;
                        }
                        else
                        {
                            dashTimeMod = -15;
                        }
                    }
                    if (justDashed)
                    {
                        player.velocity.X = dashDistance * (float)direction;
                        Point point5 = (player.Center + new Vector2((float)(direction * player.width / 2 + 2), player.gravDir * (float)-(float)player.height / 2f + player.gravDir * 2f)).ToTileCoordinates();
                        Point point6 = (player.Center + new Vector2((float)(direction * player.width / 2 + 2), 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point5.X, point5.Y) || WorldGen.SolidOrSlopedTile(point6.X, point6.Y))
                        {
                            player.velocity.X = player.velocity.X / 2f;
                        }
                        player.dashDelay = -1;
                        for (int num24 = 0; num24 < 60; num24++)
                        {
                            int num25 = Dust.NewDust(player.position, player.width, player.height, 89, 0f, 0f, 100, default, 1.25f);
                            Dust dust = Main.dust[num25];
                            dust.position.X += (float)Main.rand.Next(-5, 6);
                            dust.position.Y += (float)Main.rand.Next(-5, 6);
                            dust.velocity *= 0.2f;
                            dust.scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                            dust.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                            dust.noGravity = true;
                            dust.fadeIn = 0.5f;
                        }
                    }
                }
            }
        }

        private void OnDodge()
        {
            if (player.whoAmI == Main.myPlayer && dodgeScarf && !scarfCooldown && !eScarfCooldown)
            {
				if (evasionScarf)
				{
					player.AddBuff(ModContent.BuffType<EvasionScarfBoost>(), CalamityUtils.SecondsToFrames(9f));
					player.AddBuff(ModContent.BuffType<EvasionScarfCooldown>(), player.chaosState ? CalamityUtils.SecondsToFrames(20f) : CalamityUtils.SecondsToFrames(13f));
				}
				else
				{
					player.AddBuff(ModContent.BuffType<ScarfMeleeBoost>(), 540);
					player.AddBuff(ModContent.BuffType<ScarfCooldown>(), player.chaosState ? 1800 : 900);
				}
                player.immune = true;
                player.immuneTime = player.longInvince ? 100 : 60;
                for (int k = 0; k < player.hurtCooldowns.Length; k++)
                {
                    player.hurtCooldowns[k] = player.immuneTime;
                }
                for (int j = 0; j < 100; j++)
                {
                    int num = Dust.NewDust(player.position, player.width, player.height, 235, 0f, 0f, 100, default, 2f);
                    Dust dust = Main.dust[num];
                    dust.position.X += (float)Main.rand.Next(-20, 21);
                    dust.position.Y += (float)Main.rand.Next(-20, 21);
                    dust.velocity *= 0.4f;
                    dust.scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
                    if (Main.rand.NextBool(2))
                    {
                        dust.scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                        dust.noGravity = true;
                    }
                }
                if (player.whoAmI == Main.myPlayer)
                {
                    NetMessage.SendData(MessageID.Dodge, -1, -1, null, player.whoAmI, 1f, 0f, 0f, 0, 0, 0);
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
                        Main.dust[num7].velocity.X *= 0.2f;
                        Main.dust[num7].velocity.Y *= 0.2f;
                        Main.dust[num7].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    else if (dashMod == 2)
                    {
                        int num7 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num3), player.width + 8, 4, 246, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 2.5f);
                        Main.dust[num7].velocity.X *= 0.2f;
                        Main.dust[num7].velocity.Y *= 0.2f;
                        Main.dust[num7].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    else if (dashMod == 3)
                    {
                        int num7 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num3), player.width + 8, 4, 244, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 3f);
                        Main.dust[num7].velocity.X *= 0.2f;
                        Main.dust[num7].velocity.Y *= 0.2f;
                        Main.dust[num7].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    else if (dashMod == 4)
                    {
                        int num7 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num3), player.width + 8, 4, 244, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 3f);
                        Main.dust[num7].velocity.X *= 0.2f;
                        Main.dust[num7].velocity.Y *= 0.2f;
                        Main.dust[num7].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    else if (dashMod == 5)
                    {
                        int num7 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num3), player.width + 8, 4, 33, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 3f);
                        Main.dust[num7].velocity.X *= 0.2f;
                        Main.dust[num7].velocity.Y *= 0.2f;
                        Main.dust[num7].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    else if (dashMod == 6)
                    {
                        int num7 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num3), player.width + 8, 4, 67, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 1.25f);
                        Main.dust[num7].velocity.X *= 0.2f;
                        Main.dust[num7].velocity.Y *= 0.2f;
                        Main.dust[num7].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    else if (dashMod == 7)
                    {
                        int num7 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num3), player.width + 8, 4, 70, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 1.5f);
                        Main.dust[num7].velocity.X *= 0.2f;
                        Main.dust[num7].velocity.Y *= 0.2f;
                        Main.dust[num7].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    else if (dashMod == 8)
                    {
                        int num7 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num3), player.width + 8, 4, 89, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 1.25f);
                        Main.dust[num7].velocity.X *= 0.2f;
                        Main.dust[num7].velocity.Y *= 0.2f;
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
                        Main.dust[num12].velocity.X *= 0.2f;
                        Main.dust[num12].velocity.Y *= 0.2f;
                        Main.dust[num12].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    else if (dashMod == 3)
                    {
                        int num12 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num8), player.width + 8, 4, 244, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 3f);
                        Main.dust[num12].velocity.X *= 0.2f;
                        Main.dust[num12].velocity.Y *= 0.2f;
                        Main.dust[num12].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    else if (dashMod == 4)
                    {
                        int num12 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num8), player.width + 8, 4, 244, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 3f);
                        Main.dust[num12].velocity.X *= 0.2f;
                        Main.dust[num12].velocity.Y *= 0.2f;
                        Main.dust[num12].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
					else if (dashMod == 5)
                    {
                        int num12 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num8), player.width + 8, 4, 33, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 3f);
                        Main.dust[num12].velocity.X *= 0.2f;
                        Main.dust[num12].velocity.Y *= 0.2f;
                        Main.dust[num12].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    else if (dashMod == 6)
                    {
                        int num12 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num8), player.width + 8, 4, 67, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 1.25f);
                        Main.dust[num12].velocity.X *= 0.2f;
                        Main.dust[num12].velocity.Y *= 0.2f;
                        Main.dust[num12].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    else if (dashMod == 7)
                    {
                        int num12 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num8), player.width + 8, 4, 70, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 1.5f);
                        Main.dust[num12].velocity.X *= 0.2f;
                        Main.dust[num12].velocity.Y *= 0.2f;
                        Main.dust[num12].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                    }
                    else if (dashMod == 8)
                    {
                        int num12 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num8), player.width + 8, 4, 89, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 1.25f);
                        Main.dust[num12].velocity.X *= 0.2f;
                        Main.dust[num12].velocity.Y *= 0.2f;
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
                float damage = 800f * player.MinionDamage();
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
                float damage2 = 50f * player.MinionDamage();
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
                float damage2 = 25f * player.MinionDamage();
                float knockback2 = 5f;
                int nPCImmuneTime2 = 30;
                int playerImmuneTime2 = 6;
                ModCollideWithNPCs(rect2, damage2, knockback2, nPCImmuneTime2, playerImmuneTime2);
            }
        }

        private int ModCollideWithNPCs(Rectangle myRect, float Damage, float Knockback, int NPCImmuneTime, int PlayerImmuneTime)
        {
            int num = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
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
            Item item = drawPlayer.ActiveItem();

            // Kamei trail/afterimage effect.
            if (drawPlayer.Calamity().kamiBoost)
            {
                for (int i = drawPlayer.Calamity().KameiOldPositions.Length - 1; i > 0; i--)
                {
                    if (drawPlayer.Calamity().KameiOldPositions[i - 1] == Vector2.Zero)
                        drawPlayer.Calamity().KameiOldPositions[i - 1] = drawPlayer.position;
                    drawPlayer.Calamity().KameiOldPositions[i] = drawPlayer.Calamity().KameiOldPositions[i - 1];
                }
                drawPlayer.Calamity().KameiOldPositions[0] = drawPlayer.position;

                List<DrawData> existingDrawData = Main.playerDrawData;
                for (int i = 0; i < drawPlayer.Calamity().KameiOldPositions.Length; i++)
                {
                    float scale = MathHelper.Lerp(1f, 0.5f, i / (float)drawPlayer.Calamity().KameiOldPositions.Length);
                    float opacity = MathHelper.Lerp(0.25f, 0.08f, i / (float)drawPlayer.Calamity().KameiOldPositions.Length);
                    List<DrawData> afterimage = new List<DrawData>();
                    for (int j = 0; j < existingDrawData.Count; j++)
                    {
                        var drawData = existingDrawData[j];
                        drawData.position = existingDrawData[j].position - drawPlayer.position + drawPlayer.oldPosition;
                        drawData.color = Color.Cyan * opacity;
                        drawData.color.G = (byte)(drawData.color.G * 1.6);
                        drawData.color.B = (byte)(drawData.color.B * 1.2);
                        drawData.scale = new Vector2(scale);
                        afterimage.Add(drawData);
                    }
                    Main.playerDrawData.InsertRange(0, afterimage);
                }
            }

            if (!drawPlayer.frozen &&
                item.type > ItemID.None &&
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
                if ((drawPlayer.itemAnimation > 0 && item.useStyle != 0) || (item.holdStyle > 0 && !drawPlayer.pulley))
                {
                    // Staff
                    if (item.type == ModContent.ItemType<DeathhailStaff>() || item.type == ModContent.ItemType<Vesuvius>() || item.type == ModContent.ItemType<SoulPiercer>() || item.type == ModContent.ItemType<FatesReveal>() || (item.type == ModContent.ItemType<PrismaticBreaker>() && item.useStyle == 5))
                    {
                        Texture2D texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Magic/DeathhailStaffGlow");
                        if (item.type == ModContent.ItemType<Vesuvius>())
                            texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Magic/VesuviusGlow");
                        else if (item.type == ModContent.ItemType<SoulPiercer>())
                            texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Magic/SoulPiercerGlow");
                        else if (item.type == ModContent.ItemType<FatesReveal>())
                            texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Magic/FatesRevealGlow");
                        else if (item.type == ModContent.ItemType<PrismaticBreaker>())
                            texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Melee/PrismaticBreakerGlow");

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
                    item.type == ModContent.ItemType<SubsumingVortex>() || item.type == ModContent.ItemType<AuroraBlazer>())
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
                        else if (item.type == ModContent.ItemType<AuroraBlazer>())
                        {
                            texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Ranged/AuroraBlazerGlow");
                            offsetX = 44;
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
                    item.type == ModContent.ItemType<ElementalExcalibur>() || item.type == ModContent.ItemType<TerrorBlade>() || item.type == ModContent.ItemType<EtherealSubjugator>() || (item.type == ModContent.ItemType<PrismaticBreaker>() && item.useStyle == 1))
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
                        else if (item.type == ModContent.ItemType<PrismaticBreaker>())
                            texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Melee/PrismaticBreakerGlow");

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
            }
        });

        public static readonly PlayerLayer DyeInvisibilityFix = new PlayerLayer("CalamityMod", "DyeInvisibilityFix", PlayerLayer.Arms, (PlayerDrawInfo drawInfo) =>
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            if (!drawPlayer.invis ||
                drawPlayer.itemAnimation > 0)
            {
                return;
            }
            for (int i = 0; i < Main.playerDrawData.Count; i++)
            {
                var copy = Main.playerDrawData[i];
                copy.shader = 0; // There's no other easy solution here to my knowledge since DrawData is a value type.
                Main.playerDrawData[i] = copy;
            }
        });

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return;

            Player drawPlayer = drawInfo.drawPlayer;
            Item item = drawPlayer.ActiveItem();

            if (!drawPlayer.frozen &&
                (item.IsAir || item.type > ItemID.None) &&
                !drawPlayer.dead &&
                (!drawPlayer.wet || !item.noWet))
            {
                if (item.type == ModContent.ItemType<FlurrystormCannon>() || item.type == ModContent.ItemType<MepheticSprayer>() || item.type == ModContent.ItemType<BrimstoneFlameblaster>() || item.type == ModContent.ItemType<BrimstoneFlamesprayer>() || item.type == ModContent.ItemType<SparkSpreader>() || item.type == ModContent.ItemType<HalleysInferno>() || item.type == ModContent.ItemType<CleansingBlaze>() || item.type == ModContent.ItemType<ElementalEruption>() || item.type == ModContent.ItemType<TheEmpyrean>() || item.type == ModContent.ItemType<Meowthrower>() || item.type == ModContent.ItemType<OverloadedBlaster>() || item.type == ModContent.ItemType<TerraFlameburster>() || item.type == ModContent.ItemType<Photoviscerator>() || item.type == ModContent.ItemType<Shadethrower>() || item.type == ModContent.ItemType<BloodBoiler>() || item.type == ModContent.ItemType<PristineFury>() || item.type == ModContent.ItemType<AuroraBlazer>())
                {
                    Color color89 = drawInfo.middleArmorColor = drawPlayer.GetImmuneAlphaPure(Lighting.GetColor((int)(drawPlayer.position.X + drawPlayer.width * 0.5) / 16, (int)(drawPlayer.position.Y + drawPlayer.height * 0.5) / 16, Color.White), drawInfo.shadow);
                    SpriteEffects spriteEffects = player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                    Texture2D thingToDraw = ModContent.GetTexture("CalamityMod/ExtraTextures/Tanks/Backpack_FlurrystormCannon");
                    if (item.type == ModContent.ItemType<MepheticSprayer>())
                        thingToDraw = ModContent.GetTexture("CalamityMod/ExtraTextures/Tanks/Backpack_BlightSpewer");
                    else if (item.type == ModContent.ItemType<BrimstoneFlameblaster>())
                        thingToDraw = ModContent.GetTexture("CalamityMod/ExtraTextures/Tanks/Backpack_BrimstoneFlameblaster");
                    else if (item.type == ModContent.ItemType<BrimstoneFlamesprayer>())
                        thingToDraw = ModContent.GetTexture("CalamityMod/ExtraTextures/Tanks/Backpack_HavocsBreath");
                    else if (item.type == ModContent.ItemType<BloodBoiler>())
                        thingToDraw = ModContent.GetTexture("CalamityMod/ExtraTextures/Tanks/Backpack_BloodBoiler");
                    else if (item.type == ModContent.ItemType<SparkSpreader>())
                        thingToDraw = ModContent.GetTexture("CalamityMod/ExtraTextures/Tanks/Backpack_SparkSpreader");
                    else if (item.type == ModContent.ItemType<HalleysInferno>())
                        thingToDraw = ModContent.GetTexture("CalamityMod/ExtraTextures/Tanks/Backpack_HalleysInferno");
                    else if (item.type == ModContent.ItemType<CleansingBlaze>())
                        thingToDraw = ModContent.GetTexture("CalamityMod/ExtraTextures/Tanks/Backpack_CleansingBlaze");
                    else if (item.type == ModContent.ItemType<ElementalEruption>())
                        thingToDraw = ModContent.GetTexture("CalamityMod/ExtraTextures/Tanks/Backpack_ElementalEruption");
                    else if (item.type == ModContent.ItemType<TheEmpyrean>())
                        thingToDraw = ModContent.GetTexture("CalamityMod/ExtraTextures/Tanks/Backpack_TheEmpyrean");
                    else if (item.type == ModContent.ItemType<Meowthrower>())
                        thingToDraw = ModContent.GetTexture("CalamityMod/ExtraTextures/Tanks/Backpack_Meowthrower");
                    else if (item.type == ModContent.ItemType<OverloadedBlaster>())
                        thingToDraw = ModContent.GetTexture("CalamityMod/ExtraTextures/Tanks/Backpack_OverloadedBlaster");
                    else if (item.type == ModContent.ItemType<TerraFlameburster>())
                        thingToDraw = ModContent.GetTexture("CalamityMod/ExtraTextures/Tanks/Backpack_TerraFlameburster");
                    else if (item.type == ModContent.ItemType<Photoviscerator>())
                        thingToDraw = ModContent.GetTexture("CalamityMod/ExtraTextures/Tanks/Backpack_Photoviscerator");
                    else if (item.type == ModContent.ItemType<Shadethrower>())
                        thingToDraw = ModContent.GetTexture("CalamityMod/ExtraTextures/Tanks/Backpack_Shadethrower");
                    else if (item.type == ModContent.ItemType<PristineFury>())
                        thingToDraw = ModContent.GetTexture("CalamityMod/ExtraTextures/Tanks/Backpack_PristineFury");
                    else if (item.type == ModContent.ItemType<AuroraBlazer>())
                        thingToDraw = ModContent.GetTexture("CalamityMod/ExtraTextures/Tanks/Backpack_AuroraBlazer");

                    float num25 = -4f;
                    float num24 = -8f;
                    DrawData howDoIDrawThings = new DrawData(thingToDraw,
                        new Vector2((int)(drawPlayer.position.X - Main.screenPosition.X + (drawPlayer.width / 2) - (9 * drawPlayer.direction)) + num25 * drawPlayer.direction, (int)(drawPlayer.position.Y - Main.screenPosition.Y + (drawPlayer.height / 2) + 2f * drawPlayer.gravDir + num24 * drawPlayer.gravDir)),
                        new Rectangle(0, 0, thingToDraw.Width, thingToDraw.Height),
                        color89,
                        drawPlayer.bodyRotation,
                        new Vector2(thingToDraw.Width / 2, thingToDraw.Height / 2),
                        1f,
                        spriteEffects,
                        0);
                    howDoIDrawThings.shader = 0;
                    Main.playerDrawData.Add(howDoIDrawThings);
                }
				else if (drawPlayer.Calamity().plaguebringerCarapace)
				{
                    Color color89 = drawInfo.middleArmorColor = drawPlayer.GetImmuneAlphaPure(Lighting.GetColor((int)(drawPlayer.position.X + drawPlayer.width * 0.5) / 16, (int)(drawPlayer.position.Y + drawPlayer.height * 0.5) / 16, Color.White), drawInfo.shadow);
                    SpriteEffects spriteEffects = player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                    Texture2D thingToDraw = ModContent.GetTexture("CalamityMod/Items/Armor/PlaguebringerCarapace_Back");
                    float num25 = -4f;
                    float num24 = -8f;
                    DrawData howDoIDrawThings = new DrawData(thingToDraw,
                        new Vector2((int)(drawPlayer.position.X - Main.screenPosition.X + (drawPlayer.width / 2) - (9 * drawPlayer.direction)) + num25 * drawPlayer.direction, (int)(drawPlayer.position.Y - Main.screenPosition.Y + (drawPlayer.height / 2) + 2f * drawPlayer.gravDir + num24 * drawPlayer.gravDir)),
                        new Rectangle(0, 0, thingToDraw.Width, thingToDraw.Height),
                        color89,
                        drawPlayer.bodyRotation,
                        new Vector2(thingToDraw.Width / 2, thingToDraw.Height / 2),
                        1f,
                        spriteEffects,
                        0);
                    howDoIDrawThings.shader = 0;
                    Main.playerDrawData.Add(howDoIDrawThings);
                }
            }
        }
        public static readonly List<Color> MoonlightDyeDayColors = new List<Color>()
        {
            new Color(255, 163, 56),
            new Color(235, 30, 19),
            new Color(242, 48, 187),
        };
        public static readonly List<Color> MoonlightDyeNightColors = new List<Color>()
        {
            new Color(24, 134, 198),
            new Color(130, 40, 150),
            new Color(40, 64, 150),
        };

        public static void DetermineMoonlightDyeColors(out Color drawColor, Color dayColor, Color nightColor)
        {
            int totalTime = Main.dayTime ? 54000 : 32400;
            float transitionTime = 5400;
            //Color dayColor = new Color(255, 163, 56);
            //Color nightColor = new Color(24, 134, 198);
            float interval = Utils.InverseLerp(0f, transitionTime, (float)Main.time, true) + Utils.InverseLerp(totalTime - transitionTime, totalTime, (float)Main.time, true);
            if (Main.dayTime)
            {
                // Dusk.
                if (Main.time >= totalTime - transitionTime)
                    drawColor = Color.Lerp(dayColor, nightColor, Utils.InverseLerp(totalTime - transitionTime, totalTime, (float)Main.time, true));
                // Dawn.
                else if (Main.time <= transitionTime)
                    drawColor = Color.Lerp(nightColor, dayColor, interval);
                else
                    drawColor = dayColor;
            }
            else
            {
                drawColor = nightColor;
            }
        }
        public static Color GetCurrentMoonlightDyeColor(float angleOffset = 0f)
        {
            float interval = (float)Math.Cos(Main.GlobalTime * 0.6f + angleOffset) * 0.5f + 0.5f;
            interval = MathHelper.Clamp(interval, 0f, 0.995f);
            Color dayColorToUse = CalamityUtils.MulticolorLerp(interval, MoonlightDyeDayColors.ToArray());
            Color nightColorToUse = CalamityUtils.MulticolorLerp(interval, MoonlightDyeNightColors.ToArray());
            DetermineMoonlightDyeColors(out Color drawColor, dayColorToUse, nightColorToUse);
            return drawColor;
        }

        public override void PreUpdate()
        {
            tailFrameUp++;
			if (tailFrameUp == 8)
			{
				tailFrame++;
				if (tailFrame >= 4)
				{
					tailFrame = 0;
				}
				tailFrameUp = 0;
            }

			int frameAmt = 11;
			if (roverFrameCounter >= 7)
			{
				roverFrameCounter = -1;
				roverFrame = roverFrame == frameAmt - 1 ? 0 : roverFrame + 1;
			}
			roverFrameCounter++;

            for (int i = 0; i < player.dye.Length; i++)
            {
                if (player.dye[i].type == ModContent.ItemType<ProfanedMoonlightDye>())
                {
                    GameShaders.Armor.GetSecondaryShader(player.dye[i].dye, player)?.UseColor(GetCurrentMoonlightDyeColor());
                }
            }
        }

		public static readonly PlayerLayer Tail = new PlayerLayer("CalamityMod", "Tail", PlayerLayer.BackAcc, delegate (PlayerDrawInfo drawInfo)
		{
			Player drawPlayer = drawInfo.drawPlayer;
			CalamityPlayer modPlayer = drawPlayer.Calamity();
			if (drawInfo.shadow != 0f || drawPlayer.dead)
			{
				return;
			}
			Rectangle? frame = new Rectangle(0, (int)(modPlayer.tailFrame * 56), 46, 56);
			if (modPlayer.fathomSwarmerTail)
			{
				Texture2D texture = ModContent.GetTexture("CalamityMod/Items/Armor/FathomSwarmerArmor_Tail");

				int frameSize = texture.Height / 4;
				int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X - (3 * drawPlayer.direction));
				int drawY = (int)(drawInfo.position.Y + drawPlayer.height / 2f - Main.screenPosition.Y);
				DrawData data = new DrawData(texture, new Vector2(drawX, drawY), frame,
					Lighting.GetColor((int)((drawInfo.position.X + drawPlayer.width / 2f) / 16f),
						(int)((drawInfo.position.Y + drawPlayer.height / 2f) / 16f)),
					0f, new Vector2(texture.Width / 2f, frameSize / 2f), 1f,
					drawInfo.spriteEffects, 0);
				Main.playerDrawData.Add(data);
			}
		});

		public static readonly PlayerLayer ForbiddenCircletSign = new PlayerLayer("CalamityMod", "ForbiddenSigil", PlayerLayer.BackAcc, delegate (PlayerDrawInfo drawInfo)
		{
			DrawData drawData = new DrawData();
			Player drawPlayer = drawInfo.drawPlayer;
			CalamityPlayer modPlayer = drawPlayer.Calamity();
			if (drawInfo.shadow != 0f || drawPlayer.dead || !modPlayer.forbiddenCirclet)
			{
				return;
			}
			SpriteEffects spriteEffects = SpriteEffects.FlipHorizontally;
			if (drawPlayer.gravDir == 1f)
			{
				if (drawPlayer.direction == 1)
				{
					spriteEffects = SpriteEffects.None;
				}
				else
				{
					spriteEffects = SpriteEffects.FlipHorizontally;
				}
			}
			else
			{
				if (drawPlayer.direction == 1)
				{
					spriteEffects = SpriteEffects.FlipVertically;
				}
				else
				{
					spriteEffects = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
				}
			}
			int dyeShader = 0;
			if (drawPlayer.dye[1] != null)
				dyeShader = (int)drawPlayer.dye[1].dye;
			Microsoft.Xna.Framework.Color color12 = drawPlayer.GetImmuneAlphaPure(Lighting.GetColor((int)((double)drawInfo.position.X + (double)drawPlayer.width * 0.5) / 16, (int)((double)drawInfo.position.Y + (double)drawPlayer.height * 0.5) / 16, Microsoft.Xna.Framework.Color.White), drawInfo.shadow);
			Microsoft.Xna.Framework.Color color19 = Microsoft.Xna.Framework.Color.Lerp(color12, Microsoft.Xna.Framework.Color.White, 0.7f);
			Texture2D texture = Main.extraTexture[ExtrasID.ForbiddenSign];
			Texture2D glowmask = Main.glowMaskTexture[GlowMaskID.ForbiddenSign];
			int num24 = (int)(((float)((double)drawPlayer.miscCounter / 300 * MathHelper.TwoPi)).ToRotationVector2().Y * 6f);
			float num25 = ((float)((double) drawPlayer.miscCounter / 75.0 * MathHelper.TwoPi)).ToRotationVector2().X * 4f;
			Microsoft.Xna.Framework.Color color20 = new Microsoft.Xna.Framework.Color(80, 70, 40, 0) * (float) ((double) num25 / 8.0 + 0.5) * 0.8f;
			Vector2 position = new Vector2((float)((double)drawInfo.position.X - (double)Main.screenPosition.X - (double)(drawPlayer.bodyFrame.Width / 2) + (double)(drawPlayer.width / 2)), (float)((double)drawInfo.position.Y - (double)Main.screenPosition.Y + (double)drawPlayer.height - (double)drawPlayer.bodyFrame.Height + 4.0)) + drawPlayer.bodyPosition + new Vector2((float)(drawPlayer.bodyFrame.Width / 2), (float)(drawPlayer.bodyFrame.Height / 2)) + new Vector2((float)(-drawPlayer.direction * 10), (float)(num24 - 20));
			drawData = new DrawData(texture, position, new Microsoft.Xna.Framework.Rectangle?(), color19, drawPlayer.bodyRotation, texture.Size() / 2f, 1f, spriteEffects, 0);
			drawData.shader = dyeShader;
			Main.playerDrawData.Add(drawData);
			for (float num26 = 0.0f; num26 < 4f; ++num26)
			{
				drawData = new DrawData(glowmask, position + (num26 * MathHelper.PiOver2).ToRotationVector2() * num25, new Microsoft.Xna.Framework.Rectangle?(), color20, drawPlayer.bodyRotation, texture.Size() / 2f, 1f, spriteEffects, 0);
				Main.playerDrawData.Add(drawData);
			}
		});

        public static readonly PlayerLayer Skin = new PlayerLayer("CalamityMod", "Skin", PlayerLayer.Skin, delegate (PlayerDrawInfo drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            if (drawInfo.shadow != 0f || drawPlayer.dead)
            {
                return;
            }
        });

        public static readonly PlayerLayer ColdDivinityOverlay = new PlayerLayer("CalamityMod", "ColdDivinity", PlayerLayer.Skin, (drawInfo) =>
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            if (modPlayer.coldDivinity)
            {
                Texture2D texture = ModContent.GetTexture("CalamityMod/ExtraTextures/ColdDivinityBody");
                int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X);
                int drawY = (int)(drawInfo.position.Y + drawPlayer.height / 2f - Main.screenPosition.Y); //4
                DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, new Color(53, Main.DiscoG, 255) * 0.5f, 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), 1.15f, drawPlayer.direction != -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
                Main.playerDrawData.Add(data);
            }
        });

        public static readonly PlayerLayer RoverDriveShield = new PlayerLayer("CalamityMod", "RoverDrive", PlayerLayer.Skin, (drawInfo) =>
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            if (modPlayer.roverDriveTimer < 616 && modPlayer.roverDrive && !drawPlayer.dead)
            {
                Texture2D texture = ModContent.GetTexture("CalamityMod/ExtraTextures/RoverAccShield");
				int frameAmt = 11;
				int height = texture.Height / frameAmt;
				int frameHeight = height * modPlayer.roverFrame;
				Vector2 drawPos = drawPlayer.Center - Main.screenPosition + new Vector2(0f, drawPlayer.gfxOffY);
				Rectangle frame = new Rectangle(0, frameHeight, texture.Width, height);
				Color color = Color.White * 0.625f;
				Vector2 origin = new Vector2(texture.Width / 2f, height / 2f);
				float scale = 1f + (float)Math.Cos(Main.GlobalTime) * 0.1f;
				SpriteEffects spriteEffects = drawPlayer.direction != -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                DrawData data = new DrawData(texture, drawPos, frame, color, 0f, origin, scale, spriteEffects, 0);
                Main.playerDrawData.Add(data);
            }
        });

        public static readonly PlayerLayer StratusSphereDrawing = new PlayerLayer("CalamityMod", "StratusSphereDrawing", PlayerLayer.HeldProjFront, (drawInfo) =>
        {
            Player drawPlayer = drawInfo.drawPlayer;
            if (drawPlayer.inventory[drawPlayer.selectedItem].type == ModContent.ItemType<StratusSphere>())
            {
                SpriteEffects effect = SpriteEffects.FlipHorizontally;
                if (drawPlayer.gravDir == 1f)
                {
                    if (drawPlayer.direction == 1)
                    {
                        effect = SpriteEffects.None;
                    }
                    else
                    {
                        effect = SpriteEffects.FlipHorizontally;
                    }
                }
                else
                {
                    if (drawPlayer.direction == 1)
                    {
                        effect = SpriteEffects.FlipVertically;
                    }
                    else
                    {
                        effect = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
                    }
                }
                Vector2 itemDrawPosition = drawPlayer.Center;
                Texture2D drawTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/StratusSphereHold");
                Rectangle rectangle = drawTexture.Frame(1, 4, 0, (int)(2 * Math.Sin(drawPlayer.miscCounter / 20f * MathHelper.TwoPi) + 2));
                Vector2 drawOffset = new Vector2(rectangle.Width / 2 * drawPlayer.direction, 0f);
                Vector2 origin = rectangle.Size() / 2f;
                Main.playerDrawData.Add(new DrawData(drawTexture,
                                                     (itemDrawPosition - Main.screenPosition + drawOffset).Floor(),
                                                     new Rectangle?(rectangle),
                                                     Color.White,
                                                     drawPlayer.itemRotation,
                                                     origin,
                                                     drawPlayer.inventory[drawPlayer.selectedItem].scale,
                                                     effect,
                                                     0));
                drawTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/StratusSphereHoldGlow");
                Main.playerDrawData.Add(new DrawData(drawTexture,
                                                     (itemDrawPosition - Main.screenPosition + drawOffset).Floor(),
                                                     new Rectangle?(rectangle),
                                                     Color.White,
                                                     drawPlayer.itemRotation, 
                                                     origin,
                                                     drawPlayer.inventory[drawPlayer.selectedItem].scale,
                                                     effect, 
                                                     0));
            }
        });

        public static readonly PlayerLayer IbanDevRobot = new PlayerLayer("CalamityMod", "IbanDevRobot", PlayerLayer.Body, (drawInfo) =>
        {
            Player drawPlayer = drawInfo.drawPlayer;
            if (drawPlayer.Calamity().andromedaState == AndromedaPlayerState.Inactive)
                return;
            Main.playerDrawData.Clear();
            int robot = -1;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active &&
                    Main.projectile[i].type == ModContent.ProjectileType<GiantIbanRobotOfDoom>() &&
                    Main.projectile[i].owner == drawPlayer.whoAmI)
                {
                    robot = i;
                    break;
                }
            }
            if (robot == -1)
            {
                drawPlayer.Calamity().andromedaState = AndromedaPlayerState.Inactive;
                return;
            }

            GiantIbanRobotOfDoom robotEntityInstance = (GiantIbanRobotOfDoom)Main.projectile[robot].modProjectile;
            if (drawPlayer.Calamity().andromedaState == AndromedaPlayerState.SpecialAttack)
            {
                Texture2D dashTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/AndromedaBolt");
                Rectangle frame = dashTexture.Frame(1, 4, 0, robotEntityInstance.RightIconCooldown / 4 % 4);

                DrawData drawData = new DrawData(dashTexture,
                                 drawPlayer.Center + new Vector2(0f, -8f) - Main.screenPosition,
                                 frame,
                                 Color.White,
                                 Main.projectile[robot].rotation,
                                 drawPlayer.Size / 2,
                                 1f,
                                 Main.projectile[robot].spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                                 1);
                drawData.shader = drawPlayer.cBody;

                Main.playerDrawData.Add(drawData);
            }
            else if (drawPlayer.Calamity().andromedaState == AndromedaPlayerState.LargeRobot)
            {
                Texture2D robotTexture = ModContent.GetTexture(robotEntityInstance.Texture);
                Rectangle frame = new Rectangle(robotEntityInstance.FrameX * robotTexture.Width / 3, robotEntityInstance.FrameY * robotTexture.Height / 7,
                                                robotTexture.Width / 3, robotTexture.Height / 7);

                DrawData drawData = new DrawData(ModContent.GetTexture(Main.projectile[robot].modProjectile.Texture),
                                 Main.projectile[robot].Center + Vector2.UnitY * 6f - Main.screenPosition,
                                 frame,
                                 Color.White,
                                 Main.projectile[robot].rotation,
                                 Main.projectile[robot].Size / 2,
                                 1f,
                                 Main.projectile[robot].spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                                 1);
                drawData.shader = drawPlayer.cBody;

                Main.playerDrawData.Add(drawData);
            }
            else
            {
                Texture2D robotTexture = ModContent.GetTexture("CalamityMod/Projectiles/Summon/AndromedaSmall");
                Rectangle frame = new Rectangle(0, robotEntityInstance.CurrentFrame * 54, robotTexture.Width, robotTexture.Height / 21);
                DrawData drawData = new DrawData(robotTexture,
                                 drawPlayer.Center + new Vector2(drawPlayer.direction == 1 ? -24 : -10, -8f) - Main.screenPosition,
                                 frame,
                                 Color.White,
                                 0f,
                                 drawPlayer.Size / 2,
                                 1f,
                                 Main.projectile[robot].spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                                 1);
                drawData.shader = drawPlayer.cBody;

                Main.playerDrawData.Add(drawData);
            }
        });

        public static readonly PlayerLayer ProfanedMoonlightDyeEffects = new PlayerLayer("CalamityMod", "ProfanedMoonlight", PlayerLayer.Body, (drawInfo) =>
        {
            Player drawPlayer = drawInfo.drawPlayer;
            int totalMoonlightDyes = drawPlayer.dye.Count(dyeItem => dyeItem.type == ModContent.ItemType<ProfanedMoonlightDye>());
            if (totalMoonlightDyes <= 0)
                return;
            float auroraCount = 12 + (int)MathHelper.Clamp(totalMoonlightDyes, 0f, 4f) * 2;
            float opacity = MathHelper.Clamp(totalMoonlightDyes / 3f, 0f, 1f);

            if (Main.dayTime)
                opacity *= 0.4f;
            else
                opacity *= 0.25f;

            float time01 = Main.GlobalTime % 3f / 3f;
            Texture2D auroraTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/AuroraTexture");
            for (int i = 0; i < auroraCount; i++)
            {
                float incrementOffsetAngle = MathHelper.TwoPi * i / auroraCount;
                float timeOffset = (float)Math.Sin(time01 * MathHelper.TwoPi + incrementOffsetAngle * 2f);
                float rotation = (float)Math.Sin(incrementOffsetAngle) * MathHelper.Pi / 12f;
                float yOffset = (float)Math.Sin(time01 * MathHelper.TwoPi + incrementOffsetAngle * 2f + MathHelper.ToRadians(60f)) * 6f;
                Color color = GetCurrentMoonlightDyeColor(incrementOffsetAngle);
                Vector2 offset = new Vector2(20f * timeOffset, yOffset - 14f);
                DrawData drawData = new DrawData(auroraTexture,
                                 drawPlayer.Top + offset - Main.screenPosition,
                                 null,
                                 color * opacity,
                                 rotation + MathHelper.PiOver2,
                                 auroraTexture.Size() * 0.5f,
                                 0.135f,
                                 SpriteEffects.None,
                                 1);

                Main.playerDrawData.Add(drawData);
            }
        });

        public override void ModifyDrawLayers(List<PlayerLayer> list)
        {
            MiscEffectsBack.visible = true;
            list.Insert(0, MiscEffectsBack);
            Skin.visible = true;
            list.Insert(list.IndexOf(PlayerLayer.Skin) + 1, Skin);
            MiscEffects.visible = true;
            list.Add(MiscEffects);

            if (CalamityMod.legOverrideList.Contains(player.legs))
                list[list.IndexOf(PlayerLayer.ShoeAcc)].visible = false;
            if (fab || crysthamyr || onyxExcavator)
            {
                AddPlayerLayer(list, clAfterAll, list[list.Count - 1], false); 
            }

			if (fathomSwarmerTail)
			{
				int legsIndex = list.IndexOf(PlayerLayer.Skin);
				list.Insert(legsIndex - 1, Tail);
			}
			if (forbiddenCirclet)
			{
				int drawTheStupidSign = list.IndexOf(PlayerLayer.Skin);
				list.Insert(drawTheStupidSign, ForbiddenCircletSign);
			}
            list.Add(ColdDivinityOverlay);
            list.Add(RoverDriveShield);
            list.Add(StratusSphereDrawing);
            list.Add(ProfanedMoonlightDyeEffects);
            list.Add(IbanDevRobot);
            list.Add(DyeInvisibilityFix);
        }

        public PlayerLayer clAfterAll = new PlayerLayer("Calamity", "clAfterAll", PlayerLayer.MiscEffectsFront, delegate (PlayerDrawInfo drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            if (drawPlayer.mount != null && (modPlayer.fab || modPlayer.crysthamyr ||
                modPlayer.onyxExcavator))
            {
                drawPlayer.mount.Draw(Main.playerDrawData, 3, drawPlayer, drawInfo.position, drawInfo.mountColor, drawInfo.spriteEffects, drawInfo.shadow);
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
            // Dust modifications while high
            if (trippy)
			{
				if (Main.myPlayer == player.whoAmI)
				{
					Rectangle rectangle = new Rectangle((int)Main.screenPosition.X - 500, (int)Main.screenPosition.Y - 50, Main.screenWidth + 1000, Main.screenHeight + 100);
					int dustDrawn = 0;
					float maxShroomDust = Main.maxDustToDraw / 2;
					for (int i = 0; i < Main.maxDustToDraw; i++)
					{
						Dust dust = Main.dust[i];
						if (dust.active)
						{
							if (new Rectangle((int)dust.position.X, (int)dust.position.Y, 4, 4).Intersects(rectangle))
							{
								dust.color = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0);
								for (int num213 = 0; num213 < 4; num213++)
								{
									Vector2 position9 = dust.position;
									Vector2 dustCenter = new Vector2(position9.X + 4f, position9.Y + 4f);
									float num214 = Math.Abs(dustCenter.X - player.Center.X);
									float num215 = Math.Abs(dustCenter.Y - player.Center.Y);
									if (num213 == 0 || num213 == 2)
									{
										position9.X = player.Center.X + num214;
									}
									else
									{
										position9.X = player.Center.X - num214;
									}
									position9.X -= 4f;
									if (num213 == 0 || num213 == 1)
									{
										position9.Y = player.Center.Y + num215;
									}
									else
									{
										position9.Y = player.Center.Y - num215;
									}
									position9.Y -= 4f;
									Main.spriteBatch.Draw(Main.dustTexture, position9 - Main.screenPosition, new Rectangle?(dust.frame), dust.color, dust.rotation, new Vector2(4f, 4f), dust.scale, SpriteEffects.None, 0f);
									dustDrawn++;
								}

								// Break if too many dust clones have been drawn
								if (dustDrawn > maxShroomDust)
									break;
							}
						}
					}
				}
			}

			bool noRogueStealth = rogueStealth == 0f || player.townNPCs > 2f || !CalamityConfig.Instance.StealthInvisbility;
            if (rogueStealth > 0f && rogueStealthMax > 0f && player.townNPCs < 3f && CalamityConfig.Instance.StealthInvisbility)
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
                if (!player.StandingStill() && !player.mount.Active)
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
                if (!player.StandingStill() && !player.mount.Active)
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
                if (!player.StandingStill() && !player.mount.Active)
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
                if (!player.StandingStill() && !player.mount.Active)
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
            if (bFlames || aFlames || rageModeActive)
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
            if (adrenalineModeActive)
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
            if (nightwither)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 176, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 3f);
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
            if (vaporfied)
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    246,
                    242,
                    229,
                    226,
                    247,
                    187,
                    234
                });
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, dustType, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
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
            if (eFreeze || silvaStun || gState || cDepth || eutrophication)
            {
                if (noRogueStealth)
                {
                    r *= 0f;
                    g *= 0.05f;
                    b *= 0.3f;
                    fullBright = true;
                }
            }
            if (draedonsHeart && !shadeRegen && !cFreeze && player.StandingStill() && player.itemAnimation == 0)
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
			if ((cadence || ladHearts > 0) && !player.loveStruck)
			{
				if (Main.rand.NextBool(5) && drawInfo.shadow == 0f)
				{
					Vector2 vector2_2 = new Vector2((float)Main.rand.Next(-10, 11), (float)Main.rand.Next(-10, 11));
					vector2_2.Normalize();
					vector2_2.X *= 0.66f;
					int heart = Gore.NewGore(drawInfo.position + new Vector2((float)Main.rand.Next(player.width + 1), (float)Main.rand.Next(player.height + 1)), vector2_2 * (float)Main.rand.Next(3, 6) * 0.33f, 331, (float)Main.rand.Next(40, 121) * 0.01f);
					Main.gore[heart].sticky = false;
					Main.gore[heart].velocity *= 0.4f;
					Main.gore[heart].velocity.Y -= 0.6f;
					Main.playerDrawGore.Add(heart);
				}
			}
        }
        #endregion

        #region Nurse Modifications
        public override bool ModifyNurseHeal(NPC nurse, ref int health, ref bool removeDebuffs, ref string chatText)
        {
            if (CalamityWorld.death && areThereAnyDamnBosses)
            {
                chatText = "Now is not the time!";
                return false;
            }

            return true;
        }

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
            // Rogue weapons benefit from throwing crit AND rogue crit, so don't add both.
            // throwingCrit += boost;
        }
        #endregion

        #region Rogue Stealth
        private void ResetRogueStealth()
        {
            // rogueStealth doesn't reset every frame because it's a continuously building resource

            // these other parameters are rebuilt every frame based on the items you have equipped
            rogueStealthMax = 0f;
            stealthGenStandstill = 1f;
            stealthGenMoving = 1f;
            stealthStrikeThisFrame = false;
            stealthStrikeHalfCost = false;
            stealthStrikeAlwaysCrits = false;

            // stealthAcceleration only resets if you don't have either of the accelerator accessories equipped
            if (!darkGodSheath && !eclipseMirror)
                stealthAcceleration = 1f;
        }

        public void UpdateRogueStealth()
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
                // 1f is normal speed, anything higher is faster. Default stealth generation is 3 seconds while standing still.
                float currentStealthGen = UpdateStealthGenStats();
                rogueStealth += rogueStealthMax * (currentStealthGen / 180f); // 180 frames = 3 seconds
                if (rogueStealth > rogueStealthMax)
                    rogueStealth = rogueStealthMax;
            }

            ProvideStealthStatBonuses();

            // If the player is using an item that deals damage and is on their first frame of doing so,
            // consume stealth if a stealth strike wasn't triggered manually by item code.

            // This doesn't trigger stealth strike effects (ConsumeStealthStrike instead of StealthStrike)
            // so non-rogue weapons can't call lasers down from the sky and such.
            // Using any item which deals no damage or is a tool doesn't consume stealth.
            Item it = player.ActiveItem();
            bool hasDamage = it.damage > 0;
            bool hasHitboxes = !it.noMelee || it.shoot > ProjectileID.None;
            bool isPickaxe = it.pick > 0;
            bool isAxe = it.axe > 0;
            bool isHammer = it.hammer > 0;
            bool isPlaced = it.createTile != -1;
            bool isChannelable = it.channel;
            bool hasNonWeaponFunction = isPickaxe || isAxe || isHammer || isPlaced || isChannelable;
            bool playerUsingWeapon = hasDamage && hasHitboxes && !hasNonWeaponFunction;
            if (!stealthStrikeThisFrame && player.itemAnimation == player.itemAnimationMax - 1 && playerUsingWeapon)
                ConsumeStealthByAttacking();
        }

        private void ProvideStealthStatBonuses()
        {
            // At full stealth, you get a higher damage bonus than at any partial level of stealth.
            if (rogueStealth >= rogueStealthMax)
                throwingDamage += rogueStealth * 0.6666666f;
            else
                throwingDamage += rogueStealth * 0.5f;

            // Crit increases based on your stealth value. With certain gear, it's locked at 100% for stealth strikes.
            if (stealthStrikeAlwaysCrits && StealthStrikeAvailable())
                throwingCrit = 100;
            else
                throwingCrit += (int)(rogueStealth * 20f);

            // Stealth slightly increases movement speed and decreases aggro.
            if (wearingRogueArmor && rogueStealthMax > 0)
            {
                player.moveSpeed += rogueStealth * 0.05f;
                player.aggro -= (int)(rogueStealth * 400f);
            }
        }

        private float UpdateStealthGenStats()
        {
			int finalDawnProjCount = player.ownedProjectileCounts[ModContent.ProjectileType<FinalDawnProjectile>()] +
			player.ownedProjectileCounts[ModContent.ProjectileType<FinalDawnFireSlash>()] +
			player.ownedProjectileCounts[ModContent.ProjectileType<FinalDawnHorizontalSlash>()] +
			player.ownedProjectileCounts[ModContent.ProjectileType<FinalDawnThrow>()] +
			player.ownedProjectileCounts[ModContent.ProjectileType<FinalDawnThrow2>()];

            // If you are actively using an item, you cannot gain stealth.
            if (player.itemAnimation > 0 || finalDawnProjCount > 0)
                return 0f;

            // Penumbra Potion provides various boosts to rogue stealth generation
            if (penumbra)
            {
                if (Main.eclipse || umbraphileSet)
                {
                    stealthGenStandstill += 0.2f;
                    stealthGenMoving += 0.2f;
                }
                else if (!Main.dayTime)
                {
                    stealthGenStandstill += 0.15f;
                    stealthGenMoving += 0.15f;
                }
                else // daytime
                    stealthGenMoving += 0.15f;
            }

            if (CalamityMod.daggerList.Contains(player.ActiveItem().type) && player.invis)
            {
                stealthGenStandstill += 0.08f;
                stealthGenMoving += 0.08f;
            }
			if (shadow)
            {
                stealthGenStandstill += 0.1f;
                stealthGenMoving += 0.1f;
            }

            if (etherealExtorter && Main.moonPhase == 3) // 3 = Waning Crescent
                stealthGenStandstill += 0.15f;

			//Accessory modifiers can boost these stats
			stealthGenStandstill += accStealthGenBoost;
			stealthGenMoving += accStealthGenBoost;

            //
            // Other code which affects stealth generation goes here.
            // Increase stealthGenStandstill (default 1.0) to give a % boost to stealth gen while standing still.
            // Increase stealthGenMoving (default 1.0) to give a % boost to stealth gen while moving.
            //

            // Update Dark God's Sheath and Eclipse Mirror's stealth acceleration
            /*
             * T = frame counter
             * DGS  = (100% + 1% * T)
             * EM   = (100% + 1% * T) * 1.0084^T
             * BOTH = (100% + 1.5% * T) * 1.0084^T
             * 
             * DGS alone caps in 100 frames
             * EM alone caps in 41 frames
             * Both together caps in 32 frames
             */
            if (darkGodSheath && eclipseMirror)
            {
                stealthAcceleration += 0.015f;
                stealthAcceleration *= 1.0084f;
            }
            else if (eclipseMirror)
            {
                stealthAcceleration += 0.01f;
                stealthAcceleration *= 1.0084f;
            }
            else if (darkGodSheath)
                stealthAcceleration += 0.01f;
            stealthAcceleration = MathHelper.Clamp(stealthAcceleration, 1f, StealthAccelerationCap);

            // You get 100% stealth regen while standing still and not on a mount. Otherwise, you get your stealth regeneration while moving.
            // Stealth only regenerates at 1/3 speed while moving.
            bool standstill = player.StandingStill(0.1f) && !player.mount.Active;
            return standstill ? stealthGenStandstill : stealthGenMoving * 0.333333f * stealthAcceleration;
        }

        public bool StealthStrikeAvailable()
        {
            if (rogueStealthMax <= 0f)
                return false;
            return rogueStealth >= rogueStealthMax * (stealthStrikeHalfCost ? 0.5f : 1f);
        }

        internal void ConsumeStealthByAttacking()
        {
            stealthStrikeThisFrame = true;
            stealthAcceleration = 1f; // Reset acceleration when you attack

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

        public void StressPacket(bool server)
        {
            ModPacket packet = mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.StressSync);
            packet.Write(player.whoAmI);
            packet.Write(rage);

            if (!server)
                packet.Send();
            else
                packet.Send(-1, player.whoAmI);
        }

        public void AdrenalinePacket(bool server)
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

        public void DeathModeUnderworldTimePacket(bool server)
        {
            ModPacket packet = mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.DeathModeUnderworldTimeSync);
            packet.Write(player.whoAmI);
            packet.Write(deathModeUnderworldTime);

            if (!server)
                packet.Send();
            else
                packet.Send(-1, player.whoAmI);
        }

        public void DeathModeBlizzardTimePacket(bool server)
        {
            ModPacket packet = mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.DeathModeBlizzardTimeSync);
            packet.Write(player.whoAmI);
            packet.Write(deathModeBlizzardTime);

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
            rage = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
                StressPacket(true);
        }

        internal void HandleAdrenaline(BinaryReader reader)
        {
            adrenaline = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
                AdrenalinePacket(true);
        }

        internal void HandleDeathCount(BinaryReader reader)
        {
            deathCount = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
                DeathPacket(true);
        }

        internal void HandleDeathModeUnderworldTime(BinaryReader reader)
        {
            deathModeUnderworldTime = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
                DeathModeUnderworldTimePacket(true);
        }

        internal void HandleDeathModeBlizzardTime(BinaryReader reader)
        {
            deathModeBlizzardTime = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
                DeathModeBlizzardTimePacket(true);
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
                DeathPacket(false);
                DeathModeUnderworldTimePacket(false);
                DeathModeBlizzardTimePacket(false);
            }
        }
        #endregion

        #region Proficiency Stuff
        public void GetExactLevelUp()
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
                                int prof = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworkRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 1f);
                                Main.projectile[prof].ranged = false;
                            }
                            else
                            {
                                int prof = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworksBoxRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 0f);
                                Main.projectile[prof].ranged = false;
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
                                int prof = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworkRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 1f);
                                Main.projectile[prof].ranged = false;
                            }
                            else
                            {
                                int prof = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworksBoxRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 0f);
                                Main.projectile[prof].ranged = false;
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
                                int prof = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworkRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 1f);
                                Main.projectile[prof].ranged = false;
                            }
                            else
                            {
                                int prof = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworksBoxRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 0f);
                                Main.projectile[prof].ranged = false;
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
                                int prof = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworkRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 1f);
                                Main.projectile[prof].ranged = false;
                            }
                            else
                            {
                                int prof = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworksBoxRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 0f);
                                Main.projectile[prof].ranged = false;
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
                                int prof = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworkRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 1f);
                                Main.projectile[prof].ranged = false;
                            }
                            else
                            {
                                int prof = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworksBoxRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 0f);
                                Main.projectile[prof].ranged = false;
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

        public void GetStatBonuses()
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
                throwingDamage += 0.12f;
                throwingVelocity += 0.12f;
                throwingCrit += 6;
            }
            else if (rogueLevel >= 10500)
            {
                throwingDamage += 0.1f;
                throwingVelocity += 0.1f;
                throwingCrit += 5;
            }
            else if (rogueLevel >= 9100)
            {
                throwingDamage += 0.09f;
                throwingVelocity += 0.09f;
                throwingCrit += 5;
            }
            else if (rogueLevel >= 7800)
            {
                throwingDamage += 0.08f;
                throwingVelocity += 0.08f;
                throwingCrit += 4;
            }
            else if (rogueLevel >= 6600)
            {
                throwingDamage += 0.07f;
                throwingVelocity += 0.07f;
                throwingCrit += 4;
            }
            else if (rogueLevel >= 5500)
            {
                throwingDamage += 0.06f;
                throwingVelocity += 0.06f;
                throwingCrit += 3;
            }
            else if (rogueLevel >= 4500)
            {
                throwingDamage += 0.05f;
                throwingVelocity += 0.05f;
                throwingCrit += 3;
            }
            else if (rogueLevel >= 3600)
            {
                throwingDamage += 0.05f;
                throwingVelocity += 0.05f;
                throwingCrit += 2;
            }
            else if (rogueLevel >= 2800)
            {
                throwingDamage += 0.04f;
                throwingVelocity += 0.04f;
                throwingCrit += 2;
            }
            else if (rogueLevel >= 2100)
            {
                throwingDamage += 0.04f;
                throwingVelocity += 0.03f;
                throwingCrit += 1;
            }
            else if (rogueLevel >= 1500)
            {
                throwingDamage += 0.03f;
                throwingVelocity += 0.02f;
                throwingCrit += 1;
            }
            else if (rogueLevel >= 1000)
            {
                throwingDamage += 0.03f;
                throwingVelocity += 0.01f;
                throwingCrit += 1;
            }
            else if (rogueLevel >= 600)
            {
                throwingDamage += 0.02f;
                throwingVelocity += 0.01f;
            }
            else if (rogueLevel >= 300)
                throwingDamage += 0.02f;
            else if (rogueLevel >= 100)
                throwingDamage += 0.01f;
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

        #region Profaned Soul Crystal Stuffs

        internal void rollBabSpears(int randAmt, bool chaseable)
        {
            if (player.whoAmI == Main.myPlayer && !endoCooper && randAmt > 0 && Main.rand.NextBool(randAmt) && chaseable)
            {
                int spearsFired = 0;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (spearsFired == 2)
                        break;
                    if (Main.projectile[i].friendly && Main.projectile[i].owner == player.whoAmI)
                    {
                        bool attack = Main.projectile[i].type == ModContent.ProjectileType<MiniGuardianAttack>() && Main.projectile[i].owner == player.whoAmI;
                        if (attack || (Main.projectile[i].type == ModContent.ProjectileType<MiniGuardianDefense>() && Main.projectile[i].owner == player.whoAmI))
                        {
                            int numSpears = attack ? 12 : 6;
                            int dam = Main.projectile[i].damage;
                            if (!attack)
                                dam = (int)(dam * 0.5f);
                            float angleVariance = MathHelper.TwoPi / (float)numSpears;
                            float spinOffsetAngle = MathHelper.Pi / (2f * numSpears);
                            Vector2 posVec = new Vector2(8f, 0f).RotatedByRandom(MathHelper.TwoPi);
                            for (int x = 0; x < numSpears; x++)
                            {
                                posVec = posVec.RotatedBy(angleVariance);
                                Vector2 velocity = new Vector2(posVec.X, posVec.Y).RotatedBy(spinOffsetAngle);
                                velocity.Normalize();
                                velocity *= 8f;
                                Projectile.NewProjectile(Main.projectile[i].Center + posVec, velocity, ModContent.ProjectileType<MiniGuardianSpear>(), dam, 0f, player.whoAmI, 0f, 0f);
                            }
                            spearsFired++;
                        }
                    }
                }
            }
        }

        private bool IsValidTransitionFrame(AnimationType currentAnim, AnimationType newAnim, int frame, int counter) //this exists so it doesn't loop through the entire walk/idle anim just to find one frame for switching.
        {
            bool result = newAnim != AnimationType.Jump && currentAnim != AnimationType.Jump;
            if (currentAnim == AnimationType.Walk && newAnim == AnimationType.Idle)
            {
                result = counter <= 0 && (frame == 11 || frame == 15 || frame == 19);
            }
            else if (currentAnim == AnimationType.Idle && newAnim == AnimationType.Walk)
            {
                result = counter <= 0 && (frame == 2 || frame == 6);
            }
            return currentAnim != newAnim && result; //swapping to jumps should be instant, no need to check the counter here
        }

        private int HandlePSCAnimationFrames(AnimationType newType)
        {
            int key = profanedCrystalAnimCounter.Key; //0-based indexing 
            int value = profanedCrystalAnimCounter.Value - 1;
            AnimationType currentType = key < 8 ? AnimationType.Idle : key == 8 ? AnimationType.Jump : AnimationType.Walk;

            bool isInvalidTransFrame = !IsValidTransitionFrame(currentType, newType, key, value); //to make the transition between walk and idle frames less jarring and smoother
            AnimationType type = isInvalidTransFrame ? newType : currentType;
            int frameCount = type == AnimationType.Walk || (!profanedCrystalForce && player.statLife <= (int)(player.statLifeMax2 * 0.5)) ? 7 : 10;
            int lowerRange = type == AnimationType.Idle ? 0 : type == AnimationType.Jump ? 8 : 9;
            int upperRange = type == AnimationType.Idle ? 7 : type == AnimationType.Jump ? 8 : 22;
            if (value <= 0 || !isInvalidTransFrame)
            {
                value = frameCount;
                if (key >= lowerRange && key < upperRange)
                    key++;
                else
                    key = lowerRange;
            }
            profanedCrystalAnimCounter = new KeyValuePair<int, int>(key, value);
            return profanedCrystalAnimCounter.Key;
        }

        public override void PostUpdate() //needs to be here else it doesn't work properly, otherwise i'd have stuck it with the wing anim stuffs
        {
            if ((profanedCrystal || profanedCrystalForce) && !profanedCrystalHide && player.legs == mod.GetEquipSlot("ProviLegs", EquipType.Legs))
            {
                bool usingCarpet = player.carpetTime > 0 && player.controlJump; //doesn't make sense for carpet to use jump frame since you have solid ground
                AnimationType animType = AnimationType.Walk;
                if ((player.sliding || player.velocity.Y != 0 || player.mount.Active || player.grappling[0] != -1 || player.GoingDownWithGrapple) && !usingCarpet)
                    animType = AnimationType.Jump;
                else if (player.velocity.X == 0 || usingCarpet)
                    animType = AnimationType.Idle;
                int frame = HandlePSCAnimationFrames(animType);
                player.legFrame.Y = player.legFrame.Height * frame;
            }
            waterLeechTarget = -1;
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

        /// <summary>
        /// Calculates and returns the player's total light strength. This is used for Abyss darkness, among other things.<br/>
        /// The Stat Meter also reports this stat.
        /// </summary>
        /// <returns>The player's total light strength.</returns>
        public int GetTotalLightStrength()
        {
            int light = externalAbyssLight;
            bool underwater = player.IsUnderwater();
            bool miningHelmet = player.head == ArmorIDs.Head.MiningHelmet;

            // The campfire bonus does not apply while in the Abyss.
            if (!ZoneAbyss && (player.HasBuff(BuffID.Campfire) || Main.campfire))
                light += 1;
            if (camper) //inherits Campfire so really +2
                light += 1;
            if (miningHelmet)
                light += 1;
            if (player.lightOrb)
                light += 1;
            if (player.crimsonHeart)
                light += 1;
            if (player.magicLantern)
                light += 1;
            if (giantPearl)
                light += 1;
            if (radiator)
                light += 1;
            if (bendyPet)
                light += 1;
            if (sparks)
                light += 1;
            if (fathomSwarmerVisage)
                light += 1;
            if (sirenBoobs)
                light += 1;
            if (aAmpoule) // sponge inherits this and doesn't stack with ampoule
                light += 1;
            else if (rOoze && !Main.dayTime) // radiant ooze and ampoule/higher don't stack
                light += 1;
            if (aquaticEmblem && underwater)
                light += 1;
            if (player.arcticDivingGear && underwater) //inherited by abyssal diving gear/suit, also gives jellyfish necklace so really +2
                light += 1;
            if (jellyfishNecklace && underwater) //inherited by deific amulet+, jellyfish diving gear+
                light += 1;
            if (lumenousAmulet && underwater)
                light += 2;
            if (shine)
                light += 2;
            if (blazingCore)
                light += 2;
            if (player.redFairy || player.greenFairy || player.blueFairy)
                light += 2;
            if (babyGhostBell)
                light += underwater ? 2 : 1;
            if (player.petFlagDD2Ghost)
                light += 2;
            if (sirenPet)
                light += underwater ? 3 : 1;
            if (player.wisp)
                light += 3;
            if (player.suspiciouslookingTentacle)
                light += 3;
            if (profanedCrystalBuffs && !ZoneAbyss)
                light += Main.dayTime || player.lavaWet ? 2 : 1;
            return light;
        }

        #endregion
    }
}
