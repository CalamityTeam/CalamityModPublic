using CalamityMod.Buffs;
using CalamityMod.Buffs.Cooldowns;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Pets;
using CalamityMod.Buffs.Potions;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.Summon;
using CalamityMod.DataStructures;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Armor;
using CalamityMod.Items.DifficultyItems;
using CalamityMod.Items.Dyes;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Mounts.Minecarts;
using CalamityMod.Items.Tools;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.VanillaArmorChanges;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Typeless;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.AdultEidolonWyrm;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.GreatSandShark;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Other;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.Projectiles.Environment;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Skies;
using CalamityMod.Tiles;
using CalamityMod.UI;
using CalamityMod.UI.CooldownIndicators;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
	public partial class CalamityPlayer : ModPlayer
    {
        #region Variables

        #region No Category
        public static bool areThereAnyDamnBosses = false;
        public static bool areThereAnyDamnEvents = false;
        public bool drawBossHPBar = true;
        public bool shouldDrawSmallText = true;
        public int dashMod;
        public int projTypeJustHitBy;
        public int sCalDeathCount = 0;
        public int sCalKillCount = 0;
        public int deathCount = 0;
        public int actualMaxLife = 0;
        public static int chaosStateDuration = 360;
        public static int chaosStateDurationBoss = 600;
        public bool killSpikyBalls = false;
        public Projectile lastProjectileHit;
        public double acidRoundMultiplier = 1D;
        public int waterLeechTarget = -1;
        public float KameiTrailXScale = 0.1f;
        public int KameiBladeUseDelay = 0;
        public Vector2[] OldPositions = new Vector2[4];
        public double trueMeleeDamage = 0D;
        public double contactDamageReduction = 0D;
        public double projectileDamageReduction = 0D;
        public bool brimlashBusterBoost = false;
		public int evilSmasherBoost = 0;
		public int hellbornBoost = 0;
        public float animusBoost = 1f;
		public int searedPanCounter = 0;
		public int searedPanTimer = 0;
        public int potionTimer = 0;
		public bool cirrusDress = false;
        public bool blockAllDashes = false;
        public bool resetHeightandWidth = false;
        public bool noLifeRegen = false;
        public int itemTypeLastReforged = 0;
        public int reforgeTierSafety = 0;
		public bool finalTierAccessoryReforge = false;
        public float rangedAmmoCost = 1f;
        public bool heldGaelsLastFrame = false;
        public bool disableVoodooSpawns = false;
        public bool disablePerfCystSpawns = false;
        public bool disableHiveCystSpawns = false;
        public bool disableNaturalScourgeSpawns = false;
        public bool disableAnahitaSpawns = false;
        public bool blazingCursorDamage = false;
        public bool blazingCursorVisuals = false;
        public float blazingMouseAuraFade = 0f;
        public float GeneralScreenShakePower = 0f;
        public bool GivenBrimstoneLocus = false;
        public DoGCartSegment[] DoGCartSegments = new DoGCartSegment[DoGCartMount.SegmentCount];
        public float SmoothenedMinecartRotation;
        public bool LungingDown = false;
        #endregion

        #region IL Editing Constants
        // These values are referenced by IL edits but don't fit into any other category.
        public const float BalloonJumpSpeedBoost = 0.75f;

        // Shield slam stats
        public const int ShieldOfCthulhuIFrames = 6;
        public const int ShieldOfCthulhuBonkNoCollideFrames = 6;
        public const int SolarFlareIFrames = 12;
        public const float SolarFlareBaseDamage = 400f;

        // Dodge stats
        public const int BeltDodgeCooldown = 5400;
        public const int MirrorDodgeCooldown = 5400;
        public const int DaedalusReflectCooldown = 5400;
        public const int ArcanumReflectCooldown = 5400;
        public const int EvolutionReflectCooldown = 7200;
        #endregion

        #region Speedrun Timer
        // The Calamity Speedrun Timer uses the highest precision timing available to .NET and thus to the system hardware.
        // Current session time is maintained by CalamityMod.SpeedrunTimer, which is a C# Stopwatch running constantly while a player is loaded.
        // Total time is calculated on demand by adding the current stopwatch time to the previous session total.
        // This allows time to be tracked accurately through multiple save and quits.
        internal TimeSpan previousSessionTotal;
		internal int lastSplitType = -1;
        internal TimeSpan lastSplit;
		#endregion

		#region Tile Entity Trackers
		public int CurrentlyViewedFactoryID = -1;
        public int CurrentlyViewedChargerID = -1;
        public int CurrentlyViewedHologramID = -1;
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
        public bool newCalamitasInventory = false;
        #endregion

        #region Stat Meter
        public int[] damageStats = new int[6];
        public int[] critStats = new int[4];
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
        public int rageDamageStat = 0;
        public int adrenalineDamageStat = 0;
        public int adrenalineDRStat = 0;
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
        public int gaelSwipes = 0;
        public int gaelRageAttackCooldown = 0;
        public int bossRushImmunityFrameCurseTimer = 0;
        public int aBulwarkRareMeleeBoostTimer = 0;
        public int nebulaManaNerfCounter = 0;
        public int alcoholPoisonLevel = 0;
        public int modStealthTimer;
        public int dashTimeMod;
        public int hInfernoBoost = 0;
        public int pissWaterBoost = 0;
        public int packetTimer = 0;
        public int navyRodAuraTimer = 0;
        public int brimLoreInfernoTimer = 0;
        public int tarraLifeAuraTimer = 0;
        public int bloodflareHeartTimer = 300;
        public int polarisBoostCounter = 0;
        public int dragonRageHits = 0;
        public float modStealth = 1f;
        public float aquaticBoostMax = 10000f;
        public float aquaticBoost = 0f;
        public float shieldInvinc = 5f;
        public int galileoCooldown = 0;
        public int soundCooldown = 0;
        public int planarSpeedBoost = 0;
        public int profanedSoulWeaponUsage = 0;
        public int profanedSoulWeaponType = 0;
        public int hurtSoundTimer = 0;
        public int danceOfLightCharge = 0;
        public int shadowPotCooldown = 0;
        public int dogTextCooldown = 0;
        public float auralisStealthCounter = 0f;
        public int auralisAuroraCounter = 0;
        public int auralisAuroraCooldown = 0;
        public int auralisAurora = 0;
        public int fungalSymbioteTimer = 0;
        public int aBulwarkRareTimer = 0;
        public int spiritOriginBullseyeShootCountdown = 0;
        public int spiritOriginConvertedCrit = 0;

        private const int DashDisableCooldown = 12;
        public int dodgeCooldownTimer = 0;
        public bool disableAllDodges = false;

        public List<CooldownIndicator> Cooldowns = new List<CooldownIndicator>();

        public bool canFireAtaxiaRangedProjectile = false;
        public bool canFireAtaxiaRogueProjectile = false;
        public bool canFireGodSlayerRangedProjectile = false;
        public bool canFireBloodflareMageProjectile = false;
        public bool canFireBloodflareRangedProjectile = false;
        #endregion

        #region Sound
        public bool playRogueStealthSound = false;
        public bool playFullRageSound = true;
        public bool playFullAdrenalineSound = true;
        #endregion

        #region Proficiency
        private const int levelTier1 = 1500;
        private const int levelTier2 = 5500;
        private const int levelTier3 = 12500;
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
        // If stealth is too weak, increase this number. If stealth is too strong, decrease this number.
        public static double StealthDamageConstant = 0.5;

        public float rogueStealth = 0f;
        public float rogueStealthMax = 0f;
        public float stealthGenStandstill = 1f;
        public float stealthGenMoving = 1f;
        public int flatStealthLossReduction = 0;
        public const float StealthAccelerationCap = 2f;
        public float stealthAcceleration = 1f;
        public bool stealthStrikeThisFrame = false;
        public bool stealthStrikeHalfCost = false;
        public bool stealthStrike75Cost = false;
        public bool stealthStrikeAlwaysCrits = false;
        public bool wearingRogueArmor = false;
        public float accStealthGenBoost = 0f;

        public float throwingDamage = 1f;
        public float stealthDamage = 0f; // This is extra Rogue Damage that is only added for stealth strikes.
        public float RogueDamageWithStealth => throwingDamage + stealthDamage;
        public float throwingVelocity = 1f;
        public int throwingCrit = 0;
        public float throwingAmmoCost = 1f;
        public float rogueUseSpeedFactor = 0f;
        #endregion

        #region Mount
        public bool onyxExcavator = false;
        public bool angryDog = false;
        public bool fab = false;
        public bool crysthamyr = false;
        public bool ExoChair = false;
        public AndromedaPlayerState andromedaState;
        public int andromedaCripple;
        public const float UnicornSpeedNerfPower = 0.8f;
		public const float MechanicalCartSpeedNerfPower = 0.7f;
		#endregion

		#region Pet
		public bool thirdSage = false;
        public bool thirdSageH = true;
        public bool perfmini = false;
        public bool akato = false;
        public bool yharonPet = false;
        public bool leviPet = false;
        public bool plaguebringerBab = false;
        public bool rotomPet = false;
        public bool ladShark = false;
        public int ladHearts = 0;
        public bool sparks = false;
        public bool sirenPet = false;
        public bool spiritOriginPet = false;
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
        public bool hiveMindPet = false;
        public bool bendyPet = false;
        public bool littleLightPet = false;
		public bool pineapplePet = false;
        #endregion

        #region Rage
        public bool rageModeActive = false;
        public float rage = 0f;
        public float rageMax = 100f; // 0 to 100% by default
        public static readonly int DefaultRageDuration = CalamityUtils.SecondsToFrames(9); // Rage lasts 9 seconds by default.
        public static readonly int RageDurationPerBooster = CalamityUtils.SecondsToFrames(1); // Each booster is +1 second: 10, 11, 12.
        public int RageDuration = DefaultRageDuration;
        public int rageGainCooldown = 0;
        public static readonly int DefaultRageGainCooldown = 10; // It is pretty hard to have less than 10 iframes for any reason
        public int rageCombatFrames = 0;
        public static readonly int RageCombatDelayTime = CalamityUtils.SecondsToFrames(10);
        public static readonly int RageFadeTime = CalamityUtils.SecondsToFrames(30);
        public static readonly double DefaultRageDamageBoost = 0.35D; // +35%
        public double RageDamageBoost = DefaultRageDamageBoost;
        #endregion

        #region Adrenaline
        public bool adrenalineModeActive = false;
        public float adrenaline = 0f;
        public float adrenalineMax = 100f; // 0 to 100% by default
        public int AdrenalineDuration = CalamityUtils.SecondsToFrames(5);
        public int AdrenalineChargeTime = CalamityUtils.SecondsToFrames(30);
        public int AdrenalineFadeTime = CalamityUtils.SecondsToFrames(2);
        public static readonly double AdrenalineDamageBoost = 2.0D; // +200%
        public static readonly double AdrenalineDamagePerBooster = 0.15D; // +15%
        #endregion

        #region Defense Damage
        // Ratio at which incoming damage (after mitigation) is converted into defense damage.
        // Used to be 5% normal, 10% expert, 12% rev, 15% death, 20% malice
        // It is now 15% on all difficulties because you already take less damage on lower difficulties.
        public const double DefenseDamageRatio = 0.15;
        public int CurrentDefenseDamage => (int)(totalDefenseDamage * ((float)defenseDamageRecoveryFrames / totalDefenseDamageRecoveryFrames));
        internal int totalDefenseDamage = 0;
        // Defense damage from a single hit recovers in 50 frames, no matter how big the hit was.
        // If you get hit AGAIN before you have fully recovered, 50 more frames are added to your recovery timer!
        internal const int DefenseDamageBaseRecoveryTime = 60;
        // The maximum possible recovery time is 15 seconds. This is to prevent annoyance where godmode defense damage never goes away.
        internal const int DefenseDamageMaxRecoveryTime = 900;
        // How many frames the player will continue to be recovering from defense damage.
        internal int defenseDamageRecoveryFrames = 0;
        // The total timer of defense damage recovery that the player is currently suffering from.
        internal int totalDefenseDamageRecoveryFrames = DefenseDamageBaseRecoveryTime;
        // Defense damage does not start recovering for a certain number of frames after iframes end.
        internal const int DefenseDamageRecoveryDelay = 10;
        // The current timer for how long the player must wait before defense damage begins recovering.
        internal int defenseDamageDelayFrames = 0;
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
        public bool heartOfDarkness = false;
        public bool draedonsHeart = false;
        public bool rampartOfDeities = false;
        public bool vexation = false;
        public bool fBulwark = false;
        public bool dodgeScarf = false;
        public bool evasionScarf = false;
        public bool badgeOfBravery = false;
        public bool badgeOfBraveryRare = false;
		public float warBannerBonus = 0f;
		private const float maxWarBannerBonus = 0.2f;
		private const float maxWarBannerDistance = 480f;
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
		public int nCoreCooldown = 0;
        public bool deepDiver = false;
        public bool abyssalDivingSuitPlates = false;
        public bool abyssalDivingSuitCooldown = false;
        public int abyssalDivingSuitPlateHits = 0;
        public bool sirenWaterBuff = false;
        public bool sirenIce = false;
        public bool sirenIceCooldown = false;
        public bool aSpark = false;
        public bool aSparkRare = false;
        public bool aBulwarkRare = false;
        public bool dAmulet = false;
        public bool fCarapace = false;
        public bool gShell = false;
        public bool seaShell = false;
        public bool absorber = false;
        public bool aAmpoule = false;
		public bool sponge = false;
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
        public bool sRegen = false;
        public bool IBoots = false;
        public bool elysianFire = false;
        public bool sTracers = false;
        public bool eTracers = false;
        public bool cTracers = false;
        public bool frostFlare = false;
        public bool beeResist = false;
        public bool uberBees = false;
        public bool projRefRare = false;
        public int projRefRareLifeRegenCounter = 0;
        public bool nanotech = false;
        public bool eQuiver = false;
        public bool shadowMinions = false;
        public bool holyMinions = false;
        public bool alchFlask = false;
        public bool reducedPlagueDmg = false;
        public bool abaddon = false;
		public bool aeroStone = false;
        public bool community = false;
        public bool shatteredCommunity = false;
        public bool fleshTotem = false;
        public bool fleshTotemCooldown = false;
        public bool bloodPact = false;
        public bool bloodPactBoost = false;
        public bool bloodflareCore = false;
        public int bloodflareCoreLostDefense = 0;
        public bool coreOfTheBloodGod = false;
        public bool elementalHeart = false;
        public bool crownJewel = false;
        public bool celestialJewel = false;
        public bool astralArcanum = false;
        public bool harpyRing = false;
		public bool angelTreads = false;
        public bool harpyWingBoost = false; //harpy wings + harpy ring
		public bool fleshKnuckles = false;
        public bool ironBoots = false;
        public bool depthCharm = false;
        public bool anechoicPlating = false;
        public bool jellyfishNecklace = false;
        public bool abyssDivingGear = false;
        public bool abyssalAmulet = false;
        public bool lumenousAmulet = false;
        public bool aquaticEmblem = false;
        public bool spiritOrigin = false;
        public bool spiritOriginVanity = false;
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
        public bool howlsHeartVanity = false;
        public bool darkGodSheath = false;
        public bool inkBomb = false;
        public bool inkBombCooldown = false;
        public bool abyssalMirror = false;
        public bool abyssalMirrorCooldown = false;
        public bool eclipseMirror = false;
        public bool eclipseMirrorCooldown = false;
        public bool featherCrown = false;
        public bool featherCrownDraw = false;
        public bool moonCrown = false;
        public bool moonCrownDraw = false;
        public int rogueCrownCooldown = 0;
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
        public bool blunderBooster = false;
        public int jetPackDash = 0;
        public int jetPackDirection = 0;
        public bool veneratedLocket = false;
        public bool camper = false;
        public bool corrosiveSpine = false;
        public bool miniOldDuke = false;
        public bool starbusterCore = false;
        public bool starTaintedGenerator = false;
        public bool hallowedRune = false;
        public int hallowedRuneCooldown = 0;
        public bool phantomicArtifact = false;
        public int phantomicBulwarkCooldown = 0;
        public int phantomicHeartRegen = 0; // 0 = can spawn, 720 = regen applied, 600 = regen stops and 10 sec cd before it can spawn again
        public bool silvaWings = false;
        public int icicleCooldown = 0;
        public bool rustyMedal = false;
        public bool noStupidNaturalARSpawns = false;
		public bool roverDrive = false;
		public int roverDriveTimer = 0;
		public int roverFrameCounter = 0;
		public int roverFrame = 0;
        public int voidFrameCounter = 0;
        public int voidFrame = 0;
        public bool rottenDogTooth = false;
		public bool angelicAlliance = false;
		public int angelicActivate = -1;
        public bool BloomStoneRegen = false;
        public bool ChaosStone = false;
        public bool CryoStone = false;
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
		public int tarraRangedCooldown = 0;
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
        public int bloodflareSoulTimer = 0;
        public bool bloodflareThrowing = false;
        public bool bloodflareMage = false;
        public int bloodflareMageCooldown = 0;
        public bool bloodflareSummon = false;
        public int bloodflareSummonTimer = 0;
        public bool godSlayer = false;
        public bool godSlayerDamage = false;
        public bool godSlayerRanged = false;
        public bool godSlayerThrowing = false;
        public bool godSlayerCooldown = false;
		public bool godSlayerDashHotKeyPressed = false;
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
        public int brimflameFrenzyTimer = 0;
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
        public bool reaverSpeed = false;
        public bool reaverRegen = false;
        public int reaverRegenCooldown = 0;
        public bool reaverDefense = false;
        public bool reaverExplore = false;
        public bool fathomSwarmer = false;
        public bool fathomSwarmerVisage = false;
        public bool fathomSwarmerBreastplate = false;
        public bool fathomSwarmerTail = false;
        public int tailFrameUp = 0;
        public int tailFrame = 0;
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
        public bool silvaMage = false;
		public int silvaMageCooldown = 0;
		public bool silvaSummon = false;
        public bool hasSilvaEffect = false;
        public static int silvaReviveDuration = 480;
        public int silvaCountdown = silvaReviveDuration;
        public int silvaReviveCooldown = 0;
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
        public bool chaosSpirit = false;
        public bool redDevil = false;
        public bool GemTechSet = false;
        public bool CobaltSet = false;
        public bool MythrilSet = false;
        public int MythrilFlareSpawnCountdown = 0;
        public bool AdamantiteSet = false;
        public int AdamantiteSetDecayDelay = 0;

        private float adamantiteSetDefenseBoostInterpolant;
        public int AdamantiteSetDefenseBoost
		{
            get => (int)(MathHelper.Clamp(adamantiteSetDefenseBoostInterpolant, 0f, 1f) * AdamantiteArmorSetChange.DefenseBoostMax);
			set
			{
                // Clamp the boost within a respected bound.
                adamantiteSetDefenseBoostInterpolant = MathHelper.Clamp(value / (float)AdamantiteArmorSetChange.DefenseBoostMax, 0f, 1f);
            }
		}

        private GemTechArmorState gemTechState;
        public GemTechArmorState GemTechState
		{
			get
			{
                if (gemTechState is null || gemTechState.HasInvalidOwner)
                    gemTechState = new GemTechArmorState(player.whoAmI);
                return gemTechState;
            }
            set => gemTechState = value;
        }
        #endregion

        #region Debuff
        public bool alcoholPoisoning = false;
        public bool shadowflame = false;
        public bool wDeath = false;
        public bool lethalLavaBurn = false;
        public bool aCrunch = false;
        public bool irradiated = false;
        public bool bFlames = false;
        public bool weakBrimstoneFlames = false;
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
        public bool cDepth = false;
        public bool fishAlert = false;
        public bool bOut = false;
        public bool clamity = false;
        public bool sulphurPoison = false;
        public bool nightwither = false;
        public bool eFreeze = false;
        public bool wCleave = false;
        public bool eutrophication = false;
        public bool iCantBreathe = false; //Frozen Lungs debuff
        public bool cragsLava = false;
        public bool vaporfied = false;
        public bool energyShellCooldown = false;
        public bool prismaticCooldown = false;
        public bool waterLeechBleeding = false;
		public bool divineBlessCooldown = false;
		public bool banishingFire = false;
		public bool wither = false;
        public bool ManaBurn = false;
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
        public bool hallowedRegen = false;
		public bool hallowedPower = false;
        public bool kamiBoost = false;
		public bool avertorBonus = false;
		public bool divineBless = false;
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
        public bool voidAura = false;
        public bool voidAuraDamage = false;
		public bool voidConcentrationAura = false;
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
        public bool rustyDrone = false;
        public bool tundraFlameBlossom = false;
        public bool starSwallowerPetFroge = false;
        public bool snakeEyes = false;
        public bool poleWarper = false;
        public bool causticDragon = false;
        public bool plaguebringerPatronSummon = false;
        public bool howlTrio = false;
        public bool mountedScanner = false;
        public bool sepulcher = false;
        public bool daedalusGolem = false;
        public bool deathstareEyeball = false;
        public bool witherBlossom = false;
        public bool flowersOfMortality = false;
        public bool viridVanguard = false;
        public bool sageSpirit = false;
        public bool fleshBall = false;
        public bool eyeOfNight = false;
        public bool soulSeeker = false;
        public bool perditionBeacon = false;

        public List<DeadMinionProperties> PendingProjectilesToRespawn = new List<DeadMinionProperties>();

        // Due to the way vanilla summons work, the buff must be applied manually for it to properly register, since
        // the buff is typically created via the minion's item usage, not its idle existence.
        public static Dictionary<int, int> VanillaMinionBuffRelationship = new Dictionary<int, int>()
        {
            [ProjectileID.BabySlime] = BuffID.BabySlime,
            [ProjectileID.BabyHornet] = BuffID.BabyHornet,
            [ProjectileID.FlyingImp] = BuffID.ImpMinion,
            [ProjectileID.VenomSpider] = BuffID.SpiderMinion,
            [ProjectileID.JumperSpider] = BuffID.SpiderMinion,
            [ProjectileID.DangerousSpider] = BuffID.SpiderMinion,
            [ProjectileID.Spazmamini] = BuffID.TwinEyesMinion,
            [ProjectileID.Retanimini] = BuffID.TwinEyesMinion,
            [ProjectileID.Raven] = BuffID.Ravens,
            [ProjectileID.DeadlySphere] = BuffID.DeadlySphere,
            [ProjectileID.Tempest] = BuffID.SharknadoMinion,
            [ProjectileID.UFOMinion] = BuffID.UFOMinion,
            [ProjectileID.StardustCellMinion] = BuffID.StardustMinion,
            [ProjectileID.StardustDragon1] = BuffID.StardustDragonMinion,
        };

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
        public bool omegaBlueTransformationPrevious;
        public bool omegaBlueTransformation;
        public bool omegaBlueTransformationForce;
        public bool omegaBlueTransformationPower;
        #endregion

        #region Calamitas Enchant Effects
        public bool cursedSummonsEnchant = false;
        public bool flamingItemEnchant = false;
        public bool lifeManaEnchant = false;
        public bool farProximityRewardEnchant = false;
        public bool closeProximityRewardEnchant = false;
        public bool dischargingItemEnchant = false;
        public bool explosiveMinionsEnchant = false;
        public bool bladeArmEnchant = false;
        public bool manaMonsterEnchant = false;

        public bool witheringWeaponEnchant = false;
        public bool witheredDebuff = false;
        public int witheredWeaponHoldTime = 0;
        public int witheringDamageDone = 0;

        public bool persecutedEnchant = false;
        public int persecutedEnchantSummonTimer = 0;

        public bool lecherousOrbEnchant = false;
        public bool awaitingLecherousOrbSpawn = false;
        #endregion Calamitas Enchant Effects

        #region Draw Effects
        public FireParticleSet ProvidenceBurnEffectDrawer = new FireParticleSet(-1, int.MaxValue, Color.Yellow, Color.Red * 1.2f, 10f, 0.65f);
        #endregion Draw Effects

        #region Draedon Summoning
        public bool AbleToSelectExoMech = false;
        #endregion Draedon Summoning

        #region Mouse Controls Syncing
        public bool mouseRight = false;
        private bool oldMouseRight = false;
        public Vector2 mouseWorld;
        private Vector2 oldMouseWorld;

        /// <summary>
        /// Set this to true if you need to recieve updates on right clicks from players and sync them in mp.
        /// Automatically resets itself after sending an update
        /// <\summary>
        public bool rightClickListener = false;
        /// <summary>
        /// Set this to true if you need to recieve updates on the position of the player's mouse and sync them in mp.
        /// Automatically resets itself after sending an update
        /// <\summary>
        public bool mouseWorldListener = false;
        /// <summary>
        /// Set this to true if you need to recieve updates on the rotation of the mouse to the player. This sends updates less frequently than the more tight tolerance mouseWorldListener
        /// Automatically resets itself after sending an update
        /// <\summary>
        public bool mouseRotationListener = false;

        public bool syncMouseControls = false;
        #endregion

        #endregion

        #region Saving And Loading
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
            newCalamitasInventory = false;
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
			boost.AddWithCondition("finalTierAccessoryReforge", finalTierAccessoryReforge);

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
            boost.AddWithCondition("newCalamitasInventory", newCalamitasInventory);
            boost.AddWithCondition("GivenBrimstoneLocus", GivenBrimstoneLocus);

            // Calculate the new total time of all sessions at the instant of this player save.
            TimeSpan newSessionTotal;
            long totalTicks = 0L;
            if (previousSessionTotal != null)
            {
                newSessionTotal = previousSessionTotal.Add(CalamityMod.SpeedrunTimer.Elapsed);
                totalTicks = newSessionTotal.Ticks;
            }

            return new TagCompound
            {
                { "boost", boost },
                { "rage", rage },
                { "stress", rage * (10000 / 100f) }, // Backwards compatibility -- save new rage as old stress.
                { "adrenaline", adrenaline },
                { "aquaticBoostPower", aquaticBoost },
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
                { "itemTypeLastReforged", itemTypeLastReforged },
                { "reforgeTierSafety", reforgeTierSafety },
                { "moveSpeedStat", moveSpeedStat },
                { "defenseDamage", totalDefenseDamage },
                { "defenseDamageRecoveryFrames", defenseDamageRecoveryFrames },
                { "totalDefenseDamageRecoveryFrames", totalDefenseDamageRecoveryFrames },
                { "disableAllDodges", disableAllDodges },
                { "totalSpeedrunTicks", totalTicks },
				{ "lastSplitType", lastSplitType },
                { "lastSplitTicks", lastSplit.Ticks },
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
			finalTierAccessoryReforge = boost.Contains("finalTierAccessoryReforge");

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
            newCalamitasInventory = boost.Contains("newCalamitasInventory");
            GivenBrimstoneLocus = boost.Contains("GivenBrimstoneLocus");

            // Load rage if it's there, which it will be for any players saved with 1.5.
            // Older players have "stress" instead, which will be ignored. This is intentional.
            // Stress ranged from 0 to 10,000. Rage ranges from 0.0 to 100.0.
            rage = tag.ContainsKey("rage") ? tag.GetFloat("rage") : 0f;

            if (tag.ContainsKey("adrenaline"))
            {
                bool failedToLoadFloatAdrenaline = false;
                try
                {
                    adrenaline = tag.GetFloat("adrenaline");
                }
                catch (Exception _) { failedToLoadFloatAdrenaline = true; }

                if (failedToLoadFloatAdrenaline)
                    adrenaline = tag.GetInt("adrenaline") / (10000 / 100f);
            }

            if (tag.ContainsKey("aquaticBoostPower"))
                aquaticBoost = tag.GetFloat("aquaticBoostPower");
            sCalDeathCount = tag.GetInt("sCalDeathCount");
            sCalKillCount = tag.GetInt("sCalKillCount");
            deathCount = tag.GetInt("deathCount");

            // These two variables are no longer used, as the code was moved into CalamityWorld.cs to support multiplayer.
            // As a result, their values are simply fed into a discard.

            _ = tag.GetInt("moneyStolenByBandit");
            _ = tag.GetInt("reforges");

            itemTypeLastReforged = tag.GetInt("itemTypeLastReforged");
            reforgeTierSafety = tag.GetInt("reforgeTierSafety");

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

            moveSpeedStat = tag.GetInt("moveSpeedStat");
            totalDefenseDamage = tag.GetInt("defenseDamage");
            defenseDamageRecoveryFrames = tag.GetInt("defenseDamageRecoveryFrames");
            if (defenseDamageRecoveryFrames < 0)
                defenseDamageRecoveryFrames = 0;
            totalDefenseDamageRecoveryFrames = tag.GetInt("totalDefenseDamageRecoveryFrames");
            if (totalDefenseDamageRecoveryFrames <= 0)
                totalDefenseDamageRecoveryFrames = DefenseDamageBaseRecoveryTime;
            disableAllDodges = tag.GetBool("disableAllDodges");

            // Load the previous total elapsed time to know where to start the timer when it starts.
            long ticks = tag.GetLong("totalSpeedrunTicks");
            previousSessionTotal = new TimeSpan(ticks);
            // Also load the last split, so it will show up.
			lastSplitType = tag.GetInt("lastSplitType");
            ticks = tag.GetLong("lastSplitTicks");
            lastSplit = new TimeSpan(ticks);
		}
        #endregion

        #region ResetEffects
        public override void ResetEffects()
        {
            // Max health bonuses
            if (absorber)
                player.statLifeMax2 += sponge ? 30 : 20;
            player.statLifeMax2 +=
                (mFruit ? 25 : 0) +
                (bOrange ? 25 : 0) +
                (eBerry ? 25 : 0) +
                (dFruit ? 25 : 0);
			if (fleshKnuckles)
				player.statLifeMax2 += 45;
			if (ZoneAbyss && abyssalAmulet)
                player.statLifeMax2 += player.statLifeMax2 / 5 / 20 * (lumenousAmulet ? 25 : 10);
            if (coreOfTheBloodGod)
                player.statLifeMax2 += player.statLifeMax2 / 5 / 20 * 10;
            if (bloodPact)
                player.statLifeMax2 += player.statLifeMax2 / 5 / 20 * 100;
            if (affliction || afflicted)
                player.statLifeMax2 += player.statLifeMax / 5 / 20 * 10;
            if (cadence)
                player.statLifeMax2 += player.statLifeMax / 5 / 20 * 25;
            if (community)
            {
                float floatTypeBoost = 0.05f +
                    (NPC.downedSlimeKing ? 0.01f : 0f) +
                    (NPC.downedBoss1 ? 0.01f : 0f) +
                    (NPC.downedBoss2 ? 0.01f : 0f) +
                    (NPC.downedQueenBee ? 0.01f : 0f) +
                    (NPC.downedBoss3 ? 0.01f : 0f) + // 0.1
                    (Main.hardMode ? 0.01f : 0f) +
                    (NPC.downedMechBossAny ? 0.01f : 0f) +
                    (NPC.downedPlantBoss ? 0.01f : 0f) +
                    (NPC.downedGolemBoss ? 0.01f : 0f) +
                    (NPC.downedFishron ? 0.01f : 0f) + // 0.15
                    (NPC.downedAncientCultist ? 0.01f : 0f) +
                    (NPC.downedMoonlord ? 0.01f : 0f) +
                    (CalamityWorld.downedProvidence ? 0.01f : 0f) +
                    (CalamityWorld.downedDoG ? 0.01f : 0f) +
                    (CalamityWorld.downedYharon ? 0.01f : 0f); // 0.2
                int integerTypeBoost = (int)(floatTypeBoost * 50f);
                player.statLifeMax2 += player.statLifeMax / 5 / 20 * integerTypeBoost;
            }
            // Shattered Community gives the same max health boost as normal full-power Community (10%)
            if (shatteredCommunity)
                player.statLifeMax2 += (player.statLifeMax / 5 / 10) * 5;

            // Max health reductions
            if (crimEffigy)
                player.statLifeMax2 = (int)(player.statLifeMax2 * 0.9);
            if (regenator)
                player.statLifeMax2 = (int)(player.statLifeMax2 * 0.5);

            // Extra accessory slots
            // This is probably fucked in 1.4
            if (extraAccessoryML)
                player.extraAccessorySlots = 1;
            if (extraAccessoryML && player.extraAccessory && (Main.expertMode || Main.gameMenu))
                player.extraAccessorySlots = 2;
            if (BossRushEvent.BossRushActive)
            {
                if (CalamityConfig.Instance.BossRushAccessoryCurse)
                    player.extraAccessorySlots = 0;
            }

            ResetRogueStealth();

            contactDamageReduction = 0D;
            projectileDamageReduction = 0D;

            throwingDamage = 1f;
            throwingVelocity = 1f;
            throwingCrit = 0;
            throwingAmmoCost = 1f;
            accStealthGenBoost = 0f;
			rogueUseSpeedFactor = 0f;

            trueMeleeDamage = 0D;
			warBannerBonus = 0f;

            dashMod = 0;
            externalAbyssLight = 0;
            externalColdImmunity = externalHeatImmunity = false;
            alcoholPoisonLevel = 0;
            noLifeRegen = false;

            thirdSage = false;
            perfmini = false;
            akato = false;
            yharonPet = false;
            leviPet = false;
            plaguebringerBab = false;
            rotomPet = false;
            ladShark = false;
            sparks = false;
            sirenPet = false;
            spiritOriginPet = false;
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
            hiveMindPet = false;
            bendyPet = false;
            littleLightPet = false;
			pineapplePet = false;

            onyxExcavator = false;
            angryDog = false;
            fab = false;
            crysthamyr = false;
            ExoChair = false;
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
            godSlayerRanged = false;
            godSlayerThrowing = false;
            godSlayerCooldown = false;

            silvaSet = false;
            silvaMage = false;
            silvaSummon = false;

            auricSet = false;
            auricBoost = false;

            GemTechSet = false;

            CobaltSet = false;
            MythrilSet = false;
            AdamantiteSet = false;

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
			cirrusDress = false;

            blockAllDashes = false;
            blazingCursorDamage = false;
            blazingCursorVisuals = false;

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
            aBulwarkRare = false;
            dAmulet = false;
            fCarapace = false;
            gShell = false;
            seaShell = false;
            absorber = false;
            aAmpoule = false;
			sponge = false;
            rOoze = false;
            pAmulet = false;
            fBarrier = false;
            aBrain = false;
            amalgam = false;
            frostFlare = false;
            beeResist = false;
            uberBees = false;
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
            holyMinions = false;
            alchFlask = false;
            reducedPlagueDmg = false;
            abaddon = false;
			aeroStone = false;
            community = false;
            shatteredCommunity = false;
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
			angelTreads = false;
            harpyWingBoost = false; //harpy wings + harpy ring
			fleshKnuckles = false;
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
            sRegen = false;
            hallowedRune = false;
            phantomicArtifact = false;
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
			roverDrive = false;
            rottenDogTooth = false;
			angelicAlliance = false;
            BloomStoneRegen = false;
            ChaosStone = false;
            CryoStone = false;

            daedalusReflect = false;
            daedalusSplit = false;
            daedalusAbsorb = false;
            daedalusShard = false;

            brimflameSet = false;
            brimflameFrenzy = false;
            brimflameFrenzyCooldown = false;

            rangedAmmoCost = 1f;

            avertorBonus = false;

            reaverRegen = false;
            reaverSpeed = false;
            reaverDefense = false;
            reaverExplore = false;

            ironBoots = false;
            depthCharm = false;
            anechoicPlating = false;
            jellyfishNecklace = false;
            abyssDivingGear = false;
            abyssalAmulet = false;
            lumenousAmulet = false;
            aquaticEmblem = false;
            spiritOrigin = false;
            spiritOriginVanity = false;
            spiritOriginConvertedCrit = 0;

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
			featherCrownDraw = false;
            moonCrown = false;
			moonCrownDraw = false;
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
            irradiated = false;
            bFlames = false;
            witheredDebuff = false;
            weakBrimstoneFlames = false;
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
            cDepth = false;
            fishAlert = false;
            bOut = false;
            clamity = false;
            enraged = false;
            snowmanNoseless = false;
            sulphurPoison = false;
            nightwither = false;
            eFreeze = false;
            wCleave = false;
            eutrophication = false;
            iCantBreathe = false;
            cragsLava = false;
            vaporfied = false;
            energyShellCooldown = false;
            prismaticCooldown = false;
            waterLeechBleeding = false;
			divineBlessCooldown = false;
			banishingFire = false;
			wither = false;
            ManaBurn = false;

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
			divineBless = false;

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
            howlsHeartVanity = false;
            redDevil = false;
            valkyrie = false;
            slimeGod = false;
            urchin = false;
            chaosSpirit = false;
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
            voidAura = false;
            voidAuraDamage = false;
			voidConcentrationAura = false;
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
            sepulcher = false;
            daedalusGolem = false;
            deathstareEyeball = false;
            witherBlossom = false;
            flowersOfMortality = false;
            viridVanguard = false;
            sageSpirit = false;
            fleshBall = false;
            eyeOfNight = false;
            soulSeeker = false;
            perditionBeacon = false;

            disableVoodooSpawns = false;
            disablePerfCystSpawns = false;
            disableHiveCystSpawns = false;
            disableNaturalScourgeSpawns = false;
            disableAnahitaSpawns = false;

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

            omegaBlueTransformationPrevious = omegaBlueTransformation;
            omegaBlueTransformation = omegaBlueTransformationForce = omegaBlueTransformationPower = false;

            rageModeActive = false;
            adrenalineModeActive = false;
            RageDuration = DefaultRageDuration;
            RageDamageBoost = DefaultRageDamageBoost;

            cursedSummonsEnchant = false;
            flamingItemEnchant = false;
            lifeManaEnchant = false;
            farProximityRewardEnchant = false;
            closeProximityRewardEnchant = false;
            dischargingItemEnchant = false;
            explosiveMinionsEnchant = false;
            bladeArmEnchant = false;
            manaMonsterEnchant = false;
            witheringWeaponEnchant = false;
            persecutedEnchant = false;
            lecherousOrbEnchant = false;
            flatStealthLossReduction = 0;

            lastProjectileHit = null;

            AbleToSelectExoMech = false;

            EnchantHeldItemEffects(player, player.Calamity(), player.ActiveItem());
        }
        #endregion

        #region Screen Position Movements
        public override void ModifyScreenPosition()
        {
            if (CalamityConfig.Instance.DisableScreenShakes)
                return;

            if (GeneralScreenShakePower > 0f)
            {
                Main.screenPosition += Main.rand.NextVector2Circular(GeneralScreenShakePower, GeneralScreenShakePower);
                GeneralScreenShakePower = MathHelper.Clamp(GeneralScreenShakePower - 0.185f, 0f, 20f);
            }
        }
        #endregion

        #region UpdateDead
        public override void UpdateDead()
        {
			#region Debuffs
			dodgeCooldownTimer = 0;
			totalDefenseDamage = 0;
            defenseDamageRecoveryFrames = 0;
            totalDefenseDamageRecoveryFrames = DefenseDamageBaseRecoveryTime;
            defenseDamageDelayFrames = 0;
            heldGaelsLastFrame = false;
            gaelRageAttackCooldown = 0;
            gaelSwipes = 0;
            andromedaState = AndromedaPlayerState.Inactive;
            planarSpeedBoost = 0;
            galileoCooldown = 0;
            soundCooldown = 0;
            shadowPotCooldown = 0;
            dogTextCooldown = 0;
            auralisStealthCounter = 0f;
            auralisAuroraCounter = 0;
            auralisAuroraCooldown = 0;
            auralisAurora = 0;
            fungalSymbioteTimer = 0;
            aBulwarkRareTimer = 0;
            spiritOriginConvertedCrit = 0;
            rage = 0f;
            adrenaline = 0f;
            raiderStack = 0;
            raiderCooldown = 0;
            gSabatonFall = 0;
            gSabatonCooldown = 0;
            astralStarRainCooldown = 0;
			silvaMageCooldown = 0;
			bloodflareMageCooldown = 0;
			tarraRangedCooldown = 0;
			tarraMageHealCooldown = 0;
            bossRushImmunityFrameCurseTimer = 0;
            aBulwarkRareMeleeBoostTimer = 0;
            acidRoundMultiplier = 1D;
            externalAbyssLight = 0;
            externalColdImmunity = externalHeatImmunity = false;
            polarisBoostCounter = 0;
            dragonRageHits = 0;
            spectralVeilImmunity = 0;
            jetPackCooldown = 0;
            jetPackDash = 0;
            jetPackDirection = 0;
            andromedaCripple = 0;
            theBeeCooldown = 0;
			nCoreCooldown = 0;
            killSpikyBalls = false;
            rogueCrownCooldown = 0;
            fleshTotemCooldown = false;
            sandCloakCooldown = false;
			icicleCooldown = 0;
			statisTimer = 0;
			hallowedRuneCooldown = 0;
			sulphurBubbleCooldown = 0;
			ladHearts = 0;
			prismaticLasers = 0;
			roverDriveTimer = 0;
			angelicActivate = -1;
			resetHeightandWidth = false;
			noLifeRegen = false;
            alcoholPoisoning = false;
            shadowflame = false;
            wDeath = false;
            lethalLavaBurn = false;
            aCrunch = false;
            irradiated = false;
            bFlames = false;
            witheredDebuff = false;
            weakBrimstoneFlames = false;
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
            wCleave = false;
            eutrophication = false;
            iCantBreathe = false;
            cragsLava = false;
            vaporfied = false;
            energyShellCooldown = false;
            prismaticCooldown = false;
            waterLeechBleeding = false;
			divineBlessCooldown = false;
			banishingFire = false;
			wither = false;
            #endregion

            #region Rogue
            // Stealth
            rogueStealth = 0f;
            rogueStealthMax = 0f;
            stealthAcceleration = 1f;

            throwingDamage = 1f;
            stealthDamage = 0f;
            throwingVelocity = 1f;
            throwingCrit = 0;
            throwingAmmoCost = 1f;
			rogueUseSpeedFactor = 0f;
            #endregion

            #region UI
            if (stealthUIAlpha > 0f)
            {
                stealthUIAlpha -= 0.035f;
                stealthUIAlpha = MathHelper.Clamp(stealthUIAlpha, 0f, 1f);
            }
            #endregion

            #region Buffs
            sRegen = false;
            hallowedRegen = false;
			hallowedPower = false;
            onyxExcavator = false;
            angryDog = false;
            fab = false;
            crysthamyr = false;
            ExoChair = false;
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
			rangedAmmoCost = 1f;
			avertorBonus = false;
			divineBless = false;
			#endregion

            #region Armorbonuses
            flamethrowerBoost = false;
            hoverboardBoost = false; //hoverboard + shroomite visage
            shadowSpeed = false;
            godSlayer = false;
            godSlayerDamage = false;
            godSlayerRanged = false;
            godSlayerThrowing = false;
			godSlayerDashHotKeyPressed = false;
            auricBoost = false;
            silvaSet = false;
            silvaMage = false;
            silvaSummon = false;
            hasSilvaEffect = false;
            silvaCountdown = silvaReviveDuration;
            auricSet = false;
            GemTechSet = false;
            CobaltSet = false;
            MythrilSet = false;
            MythrilFlareSpawnCountdown = 0;
            AdamantiteSet = false;
            AdamantiteSetDecayDelay = 0;
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
            brimflameFrenzyTimer = 0;
            reaverSpeed = false;
            reaverRegen = false;
            reaverRegenCooldown = 0;
            reaverDefense = false;
            reaverExplore = false;
            shadeRegen = false;
            dsSetBonus = false;
            titanHeartSet = false;
            titanHeartMask = false;
            titanHeartMantle = false;
            titanHeartBoots = false;
            titanCooldown = 0;
            umbraphileSet = false;
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
            bloodflareSoulTimer = 0;
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
            GemTechState.OnDeathEffects();
            #endregion

            CurrentlyViewedFactoryID = -1;
            CurrentlyViewedChargerID = -1;
            CurrentlyViewedHologramID = -1;
            CurrentlyViewedHologramText = string.Empty;

            KameiBladeUseDelay = 0;
            lastProjectileHit = null;
            brimlashBusterBoost = false;
			evilSmasherBoost = 0;
			hellbornBoost = 0;
            animusBoost = 1f;
			searedPanCounter = 0;
			searedPanTimer = 0;
            potionTimer = 0;
            bloodflareCoreLostDefense = 0;
            persecutedEnchantSummonTimer = 0;
            LungingDown = false;

            if (BossRushEvent.BossRushActive)
            {
                if (player.whoAmI == 0 && !CalamityGlobalNPC.AnyLivingPlayers() && CalamityUtils.CountProjectiles(ModContent.ProjectileType<BossRushFailureEffectThing>()) == 0)
                    Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<BossRushFailureEffectThing>(), 0, 0f);
            }

			if (player.respawnTimer > 300)
			{
				if (CalamityWorld.armageddon && !areThereAnyDamnBosses)
					player.respawnTimer -= 5;
				else if (Main.expertMode) // 600 normal 900 expert
					player.respawnTimer--;
			}
        }
        #endregion

        #region BiomeStuff
        internal static readonly FieldInfo EffectsField = typeof(SkyManager).GetField("_effects", BindingFlags.NonPublic | BindingFlags.Instance);

        public override void UpdateBiomeVisuals()
        {
            if (BossRushSky.DetermineDrawEligibility())
            {
                // Clear all other skies, including the vanilla ones.
                Dictionary<string, CustomSky> skies = EffectsField.GetValue(SkyManager.Instance) as Dictionary<string, CustomSky>;
                bool updateRequired = false;
                foreach (string skyName in skies.Keys)
				{
                    if (skies[skyName].IsActive() && skyName != "CalamityMod:BossRush")
                    {
                        skies[skyName].Opacity = 0f;
                        skies[skyName].Deactivate();
                        updateRequired = true;
                    }
                }

                if (updateRequired)
                    SkyManager.Instance.Update(new GameTime());

                return;
            }

            bool useNebula = NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsHead>());
            player.ManageSpecialBiomeVisuals("CalamityMod:DevourerofGodsHead", useNebula);

            bool useFlash = NPC.AnyNPCs(ModContent.NPCType<StormWeaverHead>());
            if (SkyManager.Instance["CalamityMod:StormWeaverFlash"] != null && useFlash != SkyManager.Instance["CalamityMod:StormWeaverFlash"].IsActive())
            {
                if (useFlash)
                    SkyManager.Instance.Activate("CalamityMod:StormWeaverFlash", player.Center);
                else
                    SkyManager.Instance.Deactivate("CalamityMod:StormWeaverFlash");
            }

            bool useBrimstone = NPC.AnyNPCs(ModContent.NPCType<CalamitasRun3>());
            player.ManageSpecialBiomeVisuals("CalamityMod:CalamitasRun3", useBrimstone);

            bool usePlague = NPC.AnyNPCs(ModContent.NPCType<PlaguebringerGoliath>());
            player.ManageSpecialBiomeVisuals("CalamityMod:PlaguebringerGoliath", usePlague);

            bool useCryogen = NPC.AnyNPCs(ModContent.NPCType<Cryogen>());
            if (SkyManager.Instance["CalamityMod:Cryogen"] != null && useCryogen != SkyManager.Instance["CalamityMod:Cryogen"].IsActive())
            {
                if (useCryogen)
                    SkyManager.Instance.Activate("CalamityMod:Cryogen", player.Center);
                else
                    SkyManager.Instance.Deactivate("CalamityMod:Cryogen");
            }

            bool useExoMechs = ExoMechsSky.CanSkyBeActive;
            player.ManageSpecialBiomeVisuals("CalamityMod:ExoMechs", useExoMechs);
            if (useExoMechs)
                SkyManager.Instance.Activate("CalamityMod:ExoMechs", player.Center);
            else
                SkyManager.Instance.Deactivate("CalamityMod:ExoMechs");

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

            bool useSBrimstone = NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>()) || SCalSky.OverridingIntensity > 0f;
            player.ManageSpecialBiomeVisuals("CalamityMod:SupremeCalamitas", useSBrimstone);

			bool useWyrmWater = NPC.AnyNPCs(ModContent.NPCType<EidolonWyrmHeadHuge>());
			player.ManageSpecialBiomeVisuals("CalamityMod:AdultEidolonWyrm", useWyrmWater);

			bool inAstral = ZoneAstral;
            player.ManageSpecialBiomeVisuals("CalamityMod:Astral", inAstral);

            CryogenSky.UpdateDrawEligibility();

            if (SkyManager.Instance["CalamityMod:StormWeaverFlash"] != null && useFlash != SkyManager.Instance["CalamityMod:StormWeaverFlash"].IsActive())
            {
                if (useFlash)
                    SkyManager.Instance.Activate("CalamityMod:StormWeaverFlash", player.Center);
                else
                    SkyManager.Instance.Deactivate("CalamityMod:StormWeaverFlash");
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
            
            ZoneSulphur = (CalamityWorld.sulphurTiles >= 300 || (player.ZoneOverworldHeight && sulphurPosX)) && !ZoneAbyss;

            //Overriding 1.4's ass req boosts
            if (Main.snowTiles > 300)
                player.ZoneSnow = true;
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
            }
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
                if (!player.CCed && !player.chaosState)
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
                                duration = (int)(chaosStateDurationBoss * 1.5);
                            if (eScarfCooldown)
                                duration = (int)(duration * 1.5);
                            else if (scarfCooldown)
                                duration *= 2;
                            player.AddBuff(BuffID.ChaosState, duration, true);
                        }
                    }
                }
            }
            if (CalamityMod.AngelicAllianceHotKey.JustPressed && angelicAlliance && Main.myPlayer == player.whoAmI && !divineBless && !divineBlessCooldown)
            {
				int seconds = CalamityUtils.SecondsToFrames(15f);
                player.AddBuff(ModContent.BuffType<DivineBless>(), seconds, false);
				Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AngelicAllianceActivation"), player.Center);

				// Spawn an archangel for every minion you have
				float angelAmt = 0f;
				for (int projIndex = 0; projIndex < Main.maxProjectiles; projIndex++)
				{
					Projectile proj = Main.projectile[projIndex];
					if (proj.minionSlots <= 0f || !proj.IsSummon())
						continue;
					if (proj.active && proj.owner == player.whoAmI)
					{
						angelAmt += 1f;
					}
				}
				for (int projIndex = 0; projIndex < angelAmt; projIndex++)
				{
					Projectile proj = Main.projectile[projIndex];
					float start = 360f / angelAmt;
					Projectile.NewProjectile(new Vector2((int)(player.Center.X + (Math.Sin(projIndex * start) * 300)), (int)(player.Center.Y + (Math.Cos(projIndex * start) * 300))), Vector2.Zero, ModContent.ProjectileType<AngelicAllianceArchangel>(), proj.damage / 4, proj.knockBack / 4f, player.whoAmI, Main.rand.Next(120), projIndex * start);
					player.statLife += 2;
					player.HealEffect(2);
				}
            }
            if (CalamityMod.SandCloakHotkey.JustPressed && sandCloak && Main.myPlayer == player.whoAmI && rogueStealth >= rogueStealthMax * 0.25f &&
                wearingRogueArmor && rogueStealthMax > 0 && !sandCloakCooldown)
            {
                player.AddBuff(ModContent.BuffType<SandCloakCooldown>(), 1800, false); //30 seconds
                rogueStealth -= rogueStealthMax * 0.25f;
                Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<SandCloakVeil>(), 7, 8, player.whoAmI);
                Main.PlaySound(SoundID.Item, player.position, 45);
            }
            if (CalamityMod.SpectralVeilHotKey.JustPressed && spectralVeil && Main.myPlayer == player.whoAmI && rogueStealth >= rogueStealthMax * 0.25f &&
                wearingRogueArmor && rogueStealthMax > 0)
            {
                if (!player.chaosState)
                {
                    Vector2 teleportLocation;
                    teleportLocation.X = Main.mouseX + Main.screenPosition.X;
                    if (player.gravDir == 1f)
                        teleportLocation.Y = Main.mouseY + Main.screenPosition.Y - player.height;
                    else
                        teleportLocation.Y = Main.screenPosition.Y + Main.screenHeight - Main.mouseY;

                    teleportLocation.X -= player.width * 0.5f;
                    Vector2 teleportOffset = teleportLocation - player.position;
                    if (teleportOffset.Length() > SpectralVeil.TeleportRange)
                    {
                        teleportOffset = teleportOffset.SafeNormalize(Vector2.Zero) * SpectralVeil.TeleportRange;
                        teleportLocation = player.position + teleportOffset;
                    }
                    if (teleportLocation.X > 50f && teleportLocation.X < (float)(Main.maxTilesX * 16 - 50) && teleportLocation.Y > 50f && teleportLocation.Y < (float)(Main.maxTilesY * 16 - 50))
                    {
                        if (!Collision.SolidCollision(teleportLocation, player.width, player.height))
                        {
                            rogueStealth -= rogueStealthMax * 0.25f;

                            player.Teleport(teleportLocation, 1);
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
                            Vector2 step = teleportOffset / numDust;
                            for (int i = 0; i < numDust; i++)
                            {
                                int dustIndex = Dust.NewDust(player.Center - (step * i), 1, 1, 21, step.X, step.Y);
                                Main.dust[dustIndex].noGravity = true;
                                Main.dust[dustIndex].noLight = true;
                            }

                            int iframes = 150;
                            player.GiveIFrames(iframes, true);
                            spectralVeilImmunity = iframes;
                        }
                    }
                }
            }
            if (CalamityMod.PlaguePackHotKey.JustPressed && hasJetpack && Main.myPlayer == player.whoAmI && rogueStealth >= rogueStealthMax * 0.25f &&
                wearingRogueArmor && rogueStealthMax > 0 && jetPackCooldown == 0 && !player.mount.Active)
            {
                jetPackDash = blunderBooster ? 15 : 10;
                jetPackDirection = player.direction;
                jetPackCooldown = 60;
                rogueStealth -= rogueStealthMax * 0.25f;
                Main.PlaySound(SoundID.Item66, player.Center);
                Main.PlaySound(SoundID.Item34, player.Center);
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
                            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/BrimflameAbility"), player.Center);
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
                        bloodflareSoulTimer = 1800;
                    }
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/BloodflareRangerActivation"), player.Center);
                    for (int d = 0; d < 64; d++)
                    {
                        int dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 16f), player.width, player.height - 16, (int)CalamityDusts.Phantoplasm, 0f, 0f, 0, default, 1f);
                        Main.dust[dust].velocity *= 3f;
                        Main.dust[dust].scale *= 1.15f;
                    }
                    int dustAmt = 36;
                    for (int d = 0; d < dustAmt; d++)
                    {
                        Vector2 source = Vector2.Normalize(player.velocity) * new Vector2((float)player.width / 2f, (float)player.height) * 0.75f;
                        source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + player.Center;
                        Vector2 dustVel = source - player.Center;
                        int phanto = Dust.NewDust(source + dustVel, 0, 0, (int)CalamityDusts.Phantoplasm, dustVel.X * 1.5f, dustVel.Y * 1.5f, 100, default, 1.4f);
                        Main.dust[phanto].noGravity = true;
                        Main.dust[phanto].noLight = true;
                        Main.dust[phanto].velocity = dustVel;
                    }
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    int damage = (int)(300 * player.RangedDamage());
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            float ai1 = Main.rand.NextFloat() + 0.5f;
                            float randomSpeed = (float)Main.rand.Next(1, 7);
                            float randomSpeed2 = (float)Main.rand.Next(1, 7);
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            int soul = Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f) + randomSpeed, ModContent.ProjectileType<BloodflareSoul>(), damage, 0f, player.whoAmI, 0f, ai1);
							if (soul.WithinBounds(Main.maxProjectiles))
								Main.projectile[soul].Calamity().forceTypeless = true;
							int soul2 = Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f) + randomSpeed2, ModContent.ProjectileType<BloodflareSoul>(), damage, 0f, player.whoAmI, 0f, ai1);
							if (soul2.WithinBounds(Main.maxProjectiles))
								Main.projectile[soul2].Calamity().forceTypeless = true;
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
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OmegaBlueAbility"), player.Center);
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
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/DemonshadeEnrage"), player.Center);
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
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/PlagueReaperAbility"), player.Center);
                    plagueReaperCooldown = 1800;
                }
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
                            int mark = Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<CircletMark>(), damage, kBack, player.whoAmI);
							if (mark.WithinBounds(Main.maxProjectiles))
								Main.projectile[mark].Calamity().forceTypeless = true;
						}
                    }
                }
                if (prismaticSet && !prismaticCooldown && prismaticLasers <= 0)
                    prismaticLasers = CalamityUtils.SecondsToFrames(35f);
            }
            if (CalamityMod.AstralArcanumUIHotkey.JustPressed && astralArcanum && !areThereAnyDamnBosses)
            {
                AstralArcanumUI.Toggle();
            }
            if (CalamityMod.AstralTeleportHotKey.JustPressed)
            {
                if (celestialJewel && !areThereAnyDamnBosses)
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

			// Trigger for pressing the God Slayer dash key
			if (CalamityMod.GodSlayerDashHotKey.JustPressed && (player.controlUp || player.controlDown || player.controlLeft || player.controlRight) && !player.pulley && player.grappling[0] == -1 && !player.tongued && !player.mount.Active && !godSlayerCooldown && player.dashDelay == 0)
				godSlayerDashHotKeyPressed = true;

            // Trigger for pressing the Rage hotkey.
            if (CalamityMod.RageHotKey.JustPressed)
            {
                // Gael's Greatsword replaces Rage Mode with an uber skull attack
                if (gaelRageAttackCooldown == 0 && player.ActiveItem().type == ModContent.ItemType<GaelsGreatsword>() && rage > 0f)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SilvaDispel"), player.Center);

                    for (int i = 0; i < 3; i++)
                        Dust.NewDust(player.position, 120, 120, 218, 0f, 0f, 100, default, 1.5f);
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

                    float rageRatio = rage / rageMax;
                    int damage = (int)(rageRatio * GaelsGreatsword.SkullsplosionDamageMultiplier * GaelsGreatsword.BaseDamage * player.MeleeDamage());
                    float skullCount = 20f;
                    float skullSpeed = 12f;
                    for (float i = 0; i < skullCount; i += 1f)
                    {
                        float angle = MathHelper.TwoPi * i / skullCount;
                        Vector2 initialVelocity = angle.ToRotationVector2().RotatedByRandom(MathHelper.ToRadians(12f)) * skullSpeed * new Vector2(0.82f, 1.5f) *
                            Main.rand.NextFloat(0.8f, 1.2f) * (i < skullCount / 2  ? 0.25f : 1f);
                        int projectileIndex = Projectile.NewProjectile(player.Center + initialVelocity * 3f, initialVelocity, ModContent.ProjectileType<GaelSkull2>(), damage, 2f, player.whoAmI);
                        Main.projectile[projectileIndex].tileCollide = false;
                        Main.projectile[projectileIndex].localAI[1] = (Main.projectile[projectileIndex].velocity.Y < 0f).ToInt();
						if (projectileIndex.WithinBounds(Main.maxProjectiles))
							Main.projectile[projectileIndex].Calamity().forceTypeless = true;
					}

                    // Remove all rage when the special attack is used, and apply the cooldown.
                    rage = 0f;
                    gaelRageAttackCooldown = CalamityUtils.SecondsToFrames(GaelsGreatsword.SkullsplosionCooldownSeconds);
                }
                
                // Activating Rage Mode
                if (rage >= rageMax && !rageModeActive)
                {
                    // Rage duration isn't calculated here because the buff keeps itself alive automatically as long as the player has Rage left.
                    player.AddBuff(ModContent.BuffType<RageMode>(), 2);

					// Play Rage Activation sound
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/RageActivate"), player.position);

                    // TODO -- improve Rage activation visuals
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
                }
            }

            // Trigger for pressing the Adrenaline hotkey.
            if (CalamityMod.AdrenalineHotKey.JustPressed && CalamityWorld.revenge)
            {
                if (adrenaline == adrenalineMax && !adrenalineModeActive)
                {
                    player.AddBuff(ModContent.BuffType<AdrenalineMode>(), AdrenalineDuration);

					// Play Adrenaline Activation sound
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AdrenalineActivate"), player.position);

                    // TODO -- improve Adrenaline activation visuals
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
                }
            }


            bool mountCheck = true;
            if (player.mount != null && player.mount.Active)
                mountCheck = !player.mount.BlockExtraJumps;
            bool canJump = (!player.doubleJumpCloud || !player.jumpAgainCloud) &&
            (!player.doubleJumpSandstorm || !player.jumpAgainSandstorm) &&
            (!player.doubleJumpBlizzard || !player.jumpAgainBlizzard) &&
            (!player.doubleJumpFart || !player.jumpAgainFart) &&
            (!player.doubleJumpSail || !player.jumpAgainSail) &&
            (!player.doubleJumpUnicorn || !player.jumpAgainUnicorn) &&
            CalamityUtils.CountHookProj() <= 0 && (player.rocketTime == 0 || player.wings > 0) && mountCheck;
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
                        if (bubble.WithinBounds(Main.maxProjectiles))
                            Main.projectile[bubble].Calamity().forceTypeless = true;
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
            int postImmuneTime = player.immuneTime;
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
            player.immuneTime = postImmuneTime;
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

				// Whichever is lowest in your vanity slots should take priority
                if (item.type == ModContent.ItemType<AbyssalDivingGear>())
                {
					featherCrownDraw = false;
					moonCrownDraw = false;
                    abyssDivingGear = true;
                }
                if (item.type == ModContent.ItemType<FeatherCrown>())
                {
					abyssDivingGear = false;
					moonCrownDraw = false;
                    featherCrownDraw = true;
                }
                if (item.type == ModContent.ItemType<MoonstoneCrown>())
                {
					abyssDivingGear = false;
					featherCrownDraw = false;
                    moonCrownDraw = true;
                }

				// Summon anime girl if it's in vanity slot as the pet is purely vanity
				// It's possible for other "pet" items like Fungal Clump or HotE to summon a passive version of their "pets" with some tweaks though
                if (item.type == ModContent.ItemType<DaawnlightSpiritOrigin>())
                {
					spiritOriginVanity = true;
					if (player.whoAmI == Main.myPlayer)
					{
						if (player.FindBuffIndex(ModContent.BuffType<DaawnlightSpiritOriginBuff>()) == -1)
							player.AddBuff(ModContent.BuffType<DaawnlightSpiritOriginBuff>(), 18000, true);
					}
                }
                if (item.type == ModContent.ItemType<HowlsHeart>())
                {
					howlsHeartVanity = true;
					if (player.whoAmI == Main.myPlayer)
					{
						if (player.FindBuffIndex(ModContent.BuffType<HowlTrio>()) == -1)
						{
							player.AddBuff(ModContent.BuffType<HowlTrio>(), 3600, true);
						}
						if (player.ownedProjectileCounts[ModContent.ProjectileType<HowlsHeartHowl>()] < 1)
						{
							Projectile.NewProjectile(player.Center, -Vector2.UnitY, ModContent.ProjectileType<HowlsHeartHowl>(), (int)(HowlsHeart.HowlDamage * player.MinionDamage()), 1f, player.whoAmI, 0f, 1f);
						}
						if (player.ownedProjectileCounts[ModContent.ProjectileType<HowlsHeartCalcifer>()] < 1)
						{
							Projectile.NewProjectile(player.Center, -Vector2.UnitY, ModContent.ProjectileType<HowlsHeartCalcifer>(), 0, 0f, player.whoAmI, 0f, 0f);
						}
						if (player.ownedProjectileCounts[ModContent.ProjectileType<HowlsHeartTurnipHead>()] < 1)
						{
							Projectile.NewProjectile(player.Center, -Vector2.UnitY, ModContent.ProjectileType<HowlsHeartTurnipHead>(), 0, 0f, player.whoAmI, 0f, 0f);
						}
					}
                }
            }
        }

        public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
        {
            // Ankh Shield Mighty Wind immunity.
            for (int i = 0; i < 8 + player.extraAccessorySlots; i++)
            {
                if (player.armor[i].type == ItemID.AnkhShield)
                    player.buffImmune[BuffID.WindPushed] = true;
            }

            if (CalamityConfig.Instance.BossHealthBar)
            {
                drawBossHPBar = true;
            }
            else
            {
                drawBossHPBar = false;
            }

            CalamityConfig.Instance.BossHealthBarExtraInfo = shouldDrawSmallText;

			// Increase tile placement speed to speed up early game a bit and make building more fun
			player.tileSpeed += 0.5f;

			// Increase wall placement speed to speed up early game a bit and make building more fun
			player.wallSpeed += 0.5f;

			// Takes the % move speed boost and reduces it to a quarter to get the actual speed increase
			// 400% move speed boost = 80% run speed boost, so an 8 run speed would become 14.4 with a 400% move speed stat
			float accRunSpeedMin = player.accRunSpeed * 0.5f;
            player.accRunSpeed += player.accRunSpeed * moveSpeedStat * 0.002f;

            if (player.accRunSpeed < accRunSpeedMin)
                player.accRunSpeed = accRunSpeedMin;

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
				int closestNPC = -1;
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC nPC = Main.npc[i];
					if (nPC.active && !nPC.friendly && (nPC.damage > 0 || nPC.boss) && !nPC.dontTakeDamage)
					{
						closestNPC = i;
						break;
					}
				}
				float distance = -1f;
				for (int j = 0; j < Main.maxNPCs; j++)
				{
					NPC nPC = Main.npc[j];
					if (nPC.active && !nPC.friendly && (nPC.damage > 0 || nPC.boss) && !nPC.dontTakeDamage)
					{
						float distance2 = Math.Abs(nPC.position.X + (float)(nPC.width / 2) - (player.position.X + (float)(player.width / 2))) + Math.Abs(nPC.position.Y + (float)(nPC.height / 2) - (player.position.Y + (float)(player.height / 2)));
						if (distance == -1f || distance2 < distance)
						{
							distance = distance2;
							closestNPC = j;
						}
					}
				}

				if (closestNPC != -1)
				{
					NPC actualClosestNPC = Main.npc[closestNPC];

					float generousHitboxWidth = Math.Max(actualClosestNPC.Hitbox.Width / 2f, actualClosestNPC.Hitbox.Height / 2f);
					float hitboxEdgeDist = actualClosestNPC.Distance(player.Center) - generousHitboxWidth;

					if (hitboxEdgeDist < 0)
						hitboxEdgeDist = 0;

					if (hitboxEdgeDist < maxWarBannerDistance)
					{
						warBannerBonus = MathHelper.Lerp(0f, maxWarBannerBonus, 1f - (hitboxEdgeDist / maxWarBannerDistance));

						if (warBannerBonus > maxWarBannerBonus)
							warBannerBonus = maxWarBannerBonus;
					}

					meleeSpeedMult += warBannerBonus;
				}
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
                    meleeSpeedMult += 0.15f;
            }
            if (community)
            {
                float floatTypeBoost = 0.05f +
                    (NPC.downedSlimeKing ? 0.01f : 0f) +
                    (NPC.downedBoss1 ? 0.01f : 0f) +
                    (NPC.downedBoss2 ? 0.01f : 0f) +
                    (NPC.downedQueenBee ? 0.01f : 0f) +
                    (NPC.downedBoss3 ? 0.01f : 0f) + // 0.1
                    (Main.hardMode ? 0.01f : 0f) +
                    (NPC.downedMechBossAny ? 0.01f : 0f) +
                    (NPC.downedPlantBoss ? 0.01f : 0f) +
                    (NPC.downedGolemBoss ? 0.01f : 0f) +
                    (NPC.downedFishron ? 0.01f : 0f) + // 0.15
                    (NPC.downedAncientCultist ? 0.01f : 0f) +
                    (NPC.downedMoonlord ? 0.01f : 0f) +
                    (CalamityWorld.downedProvidence ? 0.01f : 0f) +
                    (CalamityWorld.downedDoG ? 0.01f : 0f) +
                    (CalamityWorld.downedYharon ? 0.01f : 0f); // 0.2
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
			if (player.beetleOffense && player.beetleOrbs > 0)
			{
				meleeSpeedMult -= 0.1f * player.beetleOrbs;
			}
            if (CalamityConfig.Instance.Proficiency)
            {
                meleeSpeedMult += GetMeleeSpeedBonus();
            }
            if (GemTechState.IsYellowGemActive)
                meleeSpeedMult += GemTechHeadgear.MeleeSpeedBoost;

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
                    player.AddBuff(ModContent.BuffType<PopoBuff>(), 60, true);
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

        #region PreUpdate
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

            int frames = 4;
            if (voidFrameCounter >= 6)
            {
                voidFrameCounter = 0;
                voidFrame = voidFrame == frames - 1 ? 0 : voidFrame + 1;
            }
            voidFrameCounter++;

            for (int i = 0; i < player.dye.Length; i++)
            {
                if (player.dye[i].type == ModContent.ItemType<ProfanedMoonlightDye>())
                {
                    GameShaders.Armor.GetSecondaryShader(player.dye[i].dye, player)?.UseColor(GetCurrentMoonlightDyeColor());
                }
            }
            //Syncing mouse controls
            if (Main.myPlayer == player.whoAmI)
            {
                mouseRight = PlayerInput.Triggers.Current.MouseRight;
                mouseWorld = Main.MouseWorld;

                if (rightClickListener && mouseRight != oldMouseRight)
                {
                    oldMouseRight = mouseRight;
                    syncMouseControls = true;
                    rightClickListener = false;
                }
                if (mouseWorldListener && Vector2.Distance(mouseWorld, oldMouseWorld) > 5f)
                {
                    oldMouseWorld = mouseWorld;
                    syncMouseControls = true;
                    mouseWorldListener = false;
                }
                if (mouseRotationListener && Math.Abs((mouseWorld - player.MountedCenter).ToRotation() - (oldMouseWorld - player.MountedCenter).ToRotation()) > 0.15f)
                {
                    oldMouseWorld = mouseWorld;
                    syncMouseControls = true;
                    mouseRotationListener = false;
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

        #region PreUpdateMovement
        public override void PreUpdateMovement()
        {
            // Remove acceleration when using the exo chair.
            if (ExoChair)
            {
                float speed = DraedonGamerChairMount.MovementSpeed;
                if (CalamityMod.ExoChairSpeedupHotkey.Current)
                    speed *= 2f;

                if (player.controlLeft)
                {
                    player.velocity.X = -speed;
                    player.direction = -1;
                }
                else if (player.controlRight)
                {
                    player.velocity.X = speed;
                    player.direction = 1;
                }
                else
                    player.velocity.X = 0f;

                if (player.controlUp || player.controlJump)
                    player.velocity.Y = -speed;

                else if (player.controlDown)
                {
                    player.velocity.Y = speed;
                    if (Collision.TileCollision(player.position, player.velocity, player.width, player.height, true, false, (int)player.gravDir).Y == 0f)
                        player.velocity.Y = 0.5f;
                }
                else
                    player.velocity.Y = 0f;

                if (CalamityMod.ExoChairSlowdownHotkey.Current)
                    player.velocity *= 0.5f;
            }
        }
        #endregion

        #region PostUpdateBuffs
        public override void PostUpdateBuffs() => ForceVariousEffects();
        #endregion

        #region PostUpdateEquips
        public override void PostUpdateEquips() => ForceVariousEffects();
        #endregion

        #region PostUpdate
        #region Shop Restrictions
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
			if (!player.mount.Active)
			{
				float runAccMult = 1f +
					(shadowSpeed ? 0.5f : 0f) +
					(stressPills ? 0.05f : 0f) +
					((abyssalDivingSuit && player.IsUnderwater()) ? 0.05f : 0f) +
					(sirenWaterBuff ? 0.15f : 0f) +
					((frostFlare && player.statLife < (int)(player.statLifeMax2 * 0.25)) ? 0.15f : 0f) +
					(dragonScales ? 0.1f : 0f) +
					(kamiBoost ? KamiBuff.RunAccelerationBoost : 0f) +
                    (CobaltSet ? CobaltArmorSetChange.SpeedBoostSetBonusPercentage * 0.01f : 0f) +
                    (silvaSet ? 0.05f : 0f) +
					(blueCandle ? 0.05f : 0f) +
					(planarSpeedBoost > 0 ? (0.01f * planarSpeedBoost) : 0f) +
					((deepDiver && player.IsUnderwater()) ? 0.15f : 0f) +
					(rogueStealthMax > 0f ? (rogueStealth >= rogueStealthMax ? rogueStealth * 0.05f : rogueStealth * 0.025f) : 0f);

				float runSpeedMult = 1f +
					(shadowSpeed ? 0.5f : 0f) +
					(stressPills ? 0.05f : 0f) +
					((abyssalDivingSuit && player.IsUnderwater()) ? 0.05f : 0f) +
					(sirenWaterBuff ? 0.15f : 0f) +
					((frostFlare && player.statLife < (int)(player.statLifeMax2 * 0.25)) ? 0.15f : 0f) +
					(dragonScales ? 0.1f : 0f) +
					(kamiBoost ? KamiBuff.RunSpeedBoost : 0f) +
                    (CobaltSet ? CobaltArmorSetChange.SpeedBoostSetBonusPercentage * 0.01f : 0f) +
                    (silvaSet ? 0.05f : 0f) +
					(planarSpeedBoost > 0 ? (0.01f * planarSpeedBoost) : 0f) +
					((deepDiver && player.IsUnderwater()) ? 0.15f : 0f) +
					(rogueStealthMax > 0f ? (rogueStealth >= rogueStealthMax ? rogueStealth * 0.05f : rogueStealth * 0.025f) : 0f);

				if (abyssalDivingSuit && !player.IsUnderwater())
				{
					float multiplier = 0.4f + abyssalDivingSuitPlateHits * 0.2f;
					runAccMult *= multiplier;
					runSpeedMult *= multiplier;
				}
				if (fabledTortoise)
				{
					float multiplier = shellBoost ? 0.8f : 0.5f;
					runAccMult *= multiplier;
					runSpeedMult *= multiplier;
				}
				if (ursaSergeant)
				{
					runAccMult *= 0.85f;
					runSpeedMult *= 0.85f;
				}
				if (elysianGuard)
				{
					runAccMult *= 0.5f;
					runSpeedMult *= 0.5f;
				}
				if ((player.slippy || player.slippy2) && player.iceSkate)
				{
					runAccMult *= 0.6f;
				}

				player.runAcceleration *= runAccMult;
				player.maxRunSpeed *= runSpeedMult;
			}
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

        #region Dodges
        private bool HandleDodges()
        {
            if (player.whoAmI != Main.myPlayer || disableAllDodges)
                return false;

            if (spectralVeil && spectralVeilImmunity > 0)
            {
                SpectralVeilDodge();
                return true;
            }

            bool playerDashing = player.pulley || (player.grappling[0] == -1 && !player.tongued);
			if (playerDashing && dashMod == 9 && player.dashDelay < 0)
			{
				GodSlayerDodge();
				return true;
			}
            // Neither scarf can be used if either is on cooldown
            if (playerDashing && dashMod == 1 && player.dashDelay < 0 && dodgeScarf && !scarfCooldown && !eScarfCooldown)
            {
                CounterScarfDodge();
                return true;
            }

            // Neither mirror can be used if either is on cooldown
            if (dodgeCooldownTimer == 0 && !eclipseMirrorCooldown && !abyssalMirrorCooldown)
            {
                if (eclipseMirror)
                {
                    EclipseMirrorEvade();
                    return true;
                }
                else if (abyssalMirror)
                {
                    AbyssMirrorEvade();
                    return true;
                }
            }
            return false;
        }

        private void SpectralVeilDodge()
        {
            player.GiveIFrames(spectralVeilImmunity, true); //Set immunity before setting this variable to 0
            rogueStealth = rogueStealthMax;
            spectralVeilImmunity = 0;

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

            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SilvaDispel"), player.Center);

            NetMessage.SendData(MessageID.Dodge, -1, -1, null, player.whoAmI, 1f, 0f, 0f, 0, 0, 0);
        }

		private void GodSlayerDodge()
		{
            player.GiveIFrames(player.longInvince ? 100 : 60, true);
			Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 67);

			for (int j = 0; j < 30; j++)
			{
				int num = Dust.NewDust(player.position, player.width, player.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
				Dust dust = Main.dust[num];
				dust.position.X += Main.rand.Next(-20, 21);
				dust.position.Y += Main.rand.Next(-20, 21);
				dust.velocity *= 0.4f;
				dust.scale *= 1f + Main.rand.Next(40) * 0.01f;
				dust.shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
				if (Main.rand.NextBool(2))
				{
					dust.scale *= 1f + Main.rand.Next(40) * 0.01f;
					dust.noGravity = true;
				}
			}

			NetMessage.SendData(MessageID.Dodge, -1, -1, null, player.whoAmI, 1f, 0f, 0f, 0, 0, 0);
		}

        private void CounterScarfDodge()
        {
            if (evasionScarf)
                player.AddBuff(ModContent.BuffType<EvasionScarfCooldown>(), player.chaosState ? CalamityUtils.SecondsToFrames(20f) : CalamityUtils.SecondsToFrames(13f));
            else
                player.AddBuff(ModContent.BuffType<ScarfCooldown>(), player.chaosState ? 1800 : 900);

            player.GiveIFrames(player.longInvince ? 100 : 60, true);

            for (int j = 0; j < 100; j++)
            {
                int num = Dust.NewDust(player.position, player.width, player.height, 235, 0f, 0f, 100, default, 2f);
                Dust dust = Main.dust[num];
                dust.position.X += Main.rand.Next(-20, 21);
                dust.position.Y += Main.rand.Next(-20, 21);
                dust.velocity *= 0.4f;
                dust.scale *= 1f + Main.rand.Next(40) * 0.01f;
                dust.shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
                if (Main.rand.NextBool(2))
                {
                    dust.scale *= 1f + Main.rand.Next(40) * 0.01f;
                    dust.noGravity = true;
                }
            }

            NetMessage.SendData(MessageID.Dodge, -1, -1, null, player.whoAmI, 1f, 0f, 0f, 0, 0, 0);
        }

        public void AbyssMirrorEvade()
        {
            if (player.whoAmI == Main.myPlayer && abyssalMirror && !abyssalMirrorCooldown && !eclipseMirror)
            {
                dodgeCooldownTimer = MirrorDodgeCooldown;
				player.AddBuff(ModContent.BuffType<AbyssalMirrorCooldown>(), dodgeCooldownTimer);

                // TODO -- why is this here?
                player.noKnockback = true;

                player.GiveIFrames(player.longInvince ? 100 : 60, true);
                rogueStealth += 0.5f;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SilvaActivation"), player.Center);

                for (int i = 0; i < 10; i++)
                {
                    int lumenyl = Projectile.NewProjectile(player.Center.X, player.Center.Y, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), ModContent.ProjectileType<AbyssalMirrorProjectile>(), (int)(55 * player.RogueDamage()), 0, player.whoAmI);
                    Main.projectile[lumenyl].rotation = Main.rand.NextFloat(0, 360);
                    Main.projectile[lumenyl].frame = Main.rand.Next(0, 4);
					if (lumenyl.WithinBounds(Main.maxProjectiles))
						Main.projectile[lumenyl].Calamity().forceTypeless = true;
				}

                // TODO -- Calamity dodges should probably not send a vanilla dodge packet considering that causes Tabi dust
                if (player.whoAmI == Main.myPlayer)
                {
                    NetMessage.SendData(MessageID.Dodge, -1, -1, null, player.whoAmI, 1f, 0f, 0f, 0, 0, 0);
                }

                // Send a Calamity dodge cooldown packet.
                if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
                    SyncDodgeCooldown(false);
            }
        }

        public void EclipseMirrorEvade()
        {
            if (player.whoAmI == Main.myPlayer && eclipseMirror && !eclipseMirrorCooldown)
            {
                dodgeCooldownTimer = MirrorDodgeCooldown;
				player.AddBuff(ModContent.BuffType<EclipseMirrorCooldown>(), dodgeCooldownTimer);

                // TODO -- why is this here?
                player.noKnockback = true;

                player.GiveIFrames(player.longInvince ? 100 : 60, true);
                rogueStealth = rogueStealthMax;
                Main.PlaySound(SoundID.Item68, player.Center);

                int eclipse = Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<EclipseMirrorBurst>(), (int)(2750 * player.RogueDamage()), 0, player.whoAmI);
				if (eclipse.WithinBounds(Main.maxProjectiles))
					Main.projectile[eclipse].Calamity().forceTypeless = true;

				// TODO -- Calamity dodges should probably not send a vanilla dodge packet considering that causes Tabi dust
				if (player.whoAmI == Main.myPlayer)
                {
                    NetMessage.SendData(MessageID.Dodge, -1, -1, null, player.whoAmI, 1f, 0f, 0f, 0, 0, 0);
                }

                // Send a Calamity dodge cooldown packet.
                if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
                    SyncDodgeCooldown(false);
            }
        }
        #endregion

        #region Pre Kill
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            PopupGUIManager.SuspendAll();

            // Determine which minions need to be respawned.
            if (Main.myPlayer == player.whoAmI)
            {
                int endoHydraHeadCount = 0;
                int endoCooperType = ModContent.ProjectileType<EndoCooperBody>();
                int endoHydraHeadType = ModContent.ProjectileType<EndoHydraHead>();
                int endoHydraBodyType = ModContent.ProjectileType<EndoHydraBody>();
                int mechwormHeadType = ModContent.ProjectileType<MechwormHead>();

                // Claim data to cache before respawning as necessary.
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile projectile = Main.projectile[i];
                    if (projectile.type != endoHydraHeadType || projectile.owner != player.whoAmI || !projectile.active)
                        continue;
                    endoHydraHeadCount++;
                }

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile projectile = Main.projectile[i];
                    if ((projectile.minionSlots <= 0f && !CalamityLists.ZeroMinionSlotExceptionList.Contains(projectile.type)) || !projectile.minion || 
                        projectile.owner != player.whoAmI || !projectile.active || CalamityLists.MinionsToNotResurrectList.Contains(projectile.type))
                        continue;

                    DeadMinionProperties deadMinionProperties;

                    // Handle unique edge-cases in terms of summoning logic.
                    if (projectile.type == endoHydraBodyType)
                        deadMinionProperties = new DeadEndoHydraProperties(endoHydraHeadCount, projectile.damage, projectile.knockBack);
                    else if (projectile.type == endoCooperType)
                        deadMinionProperties = new DeadEndoCooperProperties((int)projectile.ai[0], projectile.minionSlots, projectile.damage, projectile.knockBack);
                    else
                    {
                        float[] aiToCopy = projectile.ai;

                        // If blacklisted from copying AI state values, zero out the AI values to feed to the copy.
                        if (CalamityLists.DontCopyOriginalMinionAIList.Contains(projectile.type))
                            aiToCopy = new float[aiToCopy.Length];
                        deadMinionProperties = new DeadMinionProperties(projectile.type, projectile.minionSlots, projectile.damage, projectile.knockBack, aiToCopy);
                    }

                    // Refuse to add duplicate entries of a certain type if an entry already exists and
                    // the minion properties signify that it should be unique.
                    if (deadMinionProperties.DisallowMultipleEntries && PendingProjectilesToRespawn.ContainsType(deadMinionProperties.GetType()))
                        continue;

                    // Otherwise, cache the minion's data for when the player respawns.
                    PendingProjectilesToRespawn.Add(deadMinionProperties);
                }
            }

            if (andromedaState == AndromedaPlayerState.LargeRobot)
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
                        Utils.PoofOfSmoke(player.Center + Utils.NextVector2Circular(Main.rand, 20f, 30f));
                }
            }

            if (invincible && player.ActiveItem().type != ModContent.ItemType<ColdheartIcicle>())
            {
                if (player.statLife <= 0)
                    player.statLife = 1;

                return false;
            }

            if (hInferno)
            {
                for (int x = 0; x < Main.maxNPCs; x++)
                {
                    if (Main.npc[x].active && Main.npc[x].type == ModContent.NPCType<Providence>())
                        Main.npc[x].active = false;
                }
            }

            if (nCore && nCoreCooldown <= 0)
            {
				Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 67);

				for (int j = 0; j < 50; j++)
				{
					int num = Dust.NewDust(player.position, player.width, player.height, 173, 0f, 0f, 100, default, 2f);
					Dust dust = Main.dust[num];
					dust.position.X += Main.rand.Next(-20, 21);
					dust.position.Y += Main.rand.Next(-20, 21);
					dust.velocity *= 0.9f;
					dust.scale *= 1f + Main.rand.Next(40) * 0.01f;
					dust.shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
					if (Main.rand.NextBool(2))
						dust.scale *= 1f + Main.rand.Next(40) * 0.01f;
				}

				player.statLife += 100;
				player.HealEffect(100);

				if (player.statLife > player.statLifeMax2)
					player.statLife = player.statLifeMax2;

				nCoreCooldown = CalamityUtils.SecondsToFrames(90f);

				return false;
			}

			if (dashMod == 9 && player.dashDelay < 0)
			{
				if (player.statLife < 1)
					player.statLife = 1;

				return false;
			}

            if (silvaSet && silvaCountdown > 0)
            {
                if (silvaCountdown == silvaReviveDuration && !hasSilvaEffect)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SilvaActivation"), (int)player.position.X, (int)player.position.Y);

                    player.AddBuff(ModContent.BuffType<SilvaRevival>(), silvaReviveDuration);

                    if (draconicSurge && !draconicSurgeCooldown)
                    {
                        player.statLife += player.statLifeMax2 / 2;
                        player.HealEffect(player.statLifeMax2 / 2);

                        if (player.statLife > player.statLifeMax2)
                            player.statLife = player.statLifeMax2;

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
                            player.statLife = player.statLifeMax2;
                    }
                }

                hasSilvaEffect = true;

                if (player.statLife < 1)
                    player.statLife = 1;

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

            // Custom Death Messages

            if (damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                if (alcoholPoisoning)
                {
                    if (Main.rand.NextBool())
                        damageSource = PlayerDeathReason.ByCustomReason(player.name + " downed too many shots.");
                    else
                        damageSource = PlayerDeathReason.ByCustomReason(player.name + "'s liver failed.");
                }
                if (vHex)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + " was charred by the brimstone inferno.");
                }
                if (ZoneCalamity && player.lavaWet)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + "'s soul was released by the lava.");
                }
                if (gsInferno)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + "'s soul was extinguished.");
                }
                if (sulphurPoison)
                {
                    if (Main.rand.NextBool(2))
                        damageSource = PlayerDeathReason.ByCustomReason(player.name + " was melted by the toxic waste.");
                    else
                        damageSource = PlayerDeathReason.ByOther(9);
                }
                if (lethalLavaBurn)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + " disintegrated into ashes.");
                }
                if (hInferno)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + " was turned to ashes by the Profaned Goddess.");
                }
                if (hFlames || banishingFire)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + " fell prey to their sins.");
                }
                if (waterLeechBleeding)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + " lost too much blood.");
                }
                if (shadowflame)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + "'s spirit was turned to ash.");
                }
                if (bBlood)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + " became a blood geyser.");
                }
                if (cDepth)
                {
                    if (Main.rand.NextBool())
                        damageSource = PlayerDeathReason.ByCustomReason(player.name + " was crushed by the pressure.");
                    else
                        damageSource = PlayerDeathReason.ByCustomReason(player.name + "'s lungs collapsed.");
                }
                if (bFlames || aFlames || weakBrimstoneFlames)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + " was consumed by the black flames.");
                }
                if (pFlames)
                {
                    if (Main.rand.NextBool())
                        damageSource = PlayerDeathReason.ByCustomReason(player.name + "'s flesh was melted by the plague.");
                    else
                        damageSource = PlayerDeathReason.ByCustomReason(player.name + " didn't vaccinate.");
                }
                if (astralInfection)
                {
                    if (Main.rand.NextBool())
                        damageSource = PlayerDeathReason.ByCustomReason(player.name + "'s infection spread too far.");
                    else
                        damageSource = PlayerDeathReason.ByCustomReason(player.name + "'s skin was replaced by the astral virus.");
                }
                if (nightwither)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + " was incinerated by lunar rays.");
                }
                if (vaporfied)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + " vaporized into thin air.");
                }
                if (manaOverloader || ManaBurn)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + "'s life was completely converted into mana.");
                }
                if (bloodyMary || everclear || evergreenGin || fireball || margarita || moonshine || moscowMule || redWine || screwdriver || starBeamRye || tequila || tequilaSunrise || vodka || whiteWine)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + " succumbed to alcohol sickness.");
                }
                if (witheredDebuff)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(player.name + " withered away.");
                }
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

            deathCount++;
            if (player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
            {
                SyncDeathCount(false);
            }

            return true;
        }
        #endregion

        #region On Respawn
        public override void OnRespawn(Player player)
        {
            thirdSageH = true;

            // Order the list such that less expensive minions are at the top.
            // This way cheaper minions will be spawned first, and at the end, the most expensive
            // ones can be ignored if the player ultimately has insufficient slots.
            PendingProjectilesToRespawn = PendingProjectilesToRespawn.OrderBy(proj => proj.RequiredMinionSlots).ToList();

            // Resurrect all pending minions as necessary.
            if (Main.myPlayer == player.whoAmI)
            {
                float remainingSlots = player.maxMinions;
                for (int i = 0; i < PendingProjectilesToRespawn.Count; i++)
                {
                    // Stop checking if the player has exhausted all of their base minion slots.
                    if (remainingSlots - PendingProjectilesToRespawn[i].RequiredMinionSlots < 0f)
                        break;

                    PendingProjectilesToRespawn[i].SummonCopy(player.whoAmI);

                    
                    // Apply vanilla buffs as usual to the player.
                    if (VanillaMinionBuffRelationship.ContainsKey(PendingProjectilesToRespawn[i].Type))
                        player.AddBuff(VanillaMinionBuffRelationship[PendingProjectilesToRespawn[i].Type], 3600);

                    remainingSlots -= PendingProjectilesToRespawn[i].RequiredMinionSlots;
                }
                PendingProjectilesToRespawn.Clear();
            }

            // The player rotation can be off if the player dies at the right time when using Final Dawn.
            player.fullRotation = 0f;
        }
        #endregion

        #region Get Heal Life
        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            double healMult = 1D +
                    (coreOfTheBloodGod ? 0.15 : 0) +
                    (bloodPactBoost ? 0.5 : 0);
            healValue = (int)(healValue * healMult);
        }
        #endregion

        #region Get Weapon Damage And KB
        public override void ModifyWeaponDamage(Item item, ref float add, ref float mult, ref float flat)
        {
            if (item.type == ModContent.ItemType<GaelsGreatsword>())
				mult += GaelsGreatsword.BaseDamage / (float)GaelsGreatsword.BaseDamage - 1f;

            if (flamethrowerBoost && item.ranged && (item.useAmmo == AmmoID.Gel || CalamityLists.flamethrowerList.Contains(item.type)))
				mult += hoverboardBoost ? 0.35f : 0.25f;

            if (item.ranged)
                acidRoundMultiplier = item.useTime / 20D;
            else
                acidRoundMultiplier = 1D;

            // Prismatic Breaker is a weird hybrid melee-ranged weapon so include it too.  Why are you using desert prowler post-Yharon? don't ask me
            if (desertProwler && (item.ranged || item.type == ModContent.ItemType<PrismaticBreaker>()) && item.ammo == AmmoID.None)
                flat += 1f;
        }

        public override void GetWeaponKnockback(Item item, ref float knockback)
        {
            if (auricBoost)
                knockback *= 1f + (1f - modStealth) * 0.5f;

            if (whiskey)
                knockback *= 1.04f;

            if (tequila && Main.dayTime)
                knockback *= 1.03f;

            if (tequilaSunrise && Main.dayTime)
                knockback *= 1.07f;

            if (moscowMule)
                knockback *= 1.09f;

            if (titanHeartMask && item.Calamity().rogue)
                knockback *= 1.05f;

            if (titanHeartMantle && item.Calamity().rogue)
                knockback *= 1.05f;

            if (titanHeartBoots && item.Calamity().rogue)
                knockback *= 1.05f;

            if (titanHeartSet && item.Calamity().rogue)
                knockback *= 1.2f;

            if (titanHeartSet && StealthStrikeAvailable() && item.Calamity().rogue)
                knockback *= 2f;
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
                if (fungalSymbiote && player.whoAmI == Main.myPlayer && fungalSymbioteTimer == 0)
                {
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.1) ||
                        player.itemAnimation == (int)(player.itemAnimationMax * 0.3) ||
                        player.itemAnimation == (int)(player.itemAnimationMax * 0.5) ||
                        player.itemAnimation == (int)(player.itemAnimationMax * 0.7) ||
                        player.itemAnimation == (int)(player.itemAnimationMax * 0.9))
                    {
                        fungalSymbioteTimer = 3;
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
                        Projectile.NewProjectile((float)(hitbox.X + hitbox.Width / 2) + xOffset, (float)(hitbox.Y + hitbox.Height / 2) + yOffset, (float)player.direction * xVel, yVel * player.gravDir, ProjectileID.Mushroom, (int)(item.damage * 0.15 * player.MeleeDamage()), 0f, player.whoAmI, 0f, 0f);
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

        #region Modify Hit NPC
        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            #region MultiplierBoosts
            double damageMult = 1.0;
            bool isTrueMelee = item.melee && item.type != ModContent.ItemType<UltimusCleaver>() && item.type != ModContent.ItemType<InfernaCutter>();
            if (isTrueMelee)
            {
                damageMult += trueMeleeDamage;
            }
            if (enraged)
            {
                damageMult += 1.25;
            }

            if (witheredDebuff && witheringWeaponEnchant)
                damageMult += 0.6;

            if (CalamityWorld.revenge)
            {
                CalamityUtils.ApplyRippersToDamage(this, isTrueMelee, ref damageMult);
            }
            damage = (int)(damage * damageMult);

            if (oldDie)
            {
                float diceMult = Main.rand.NextFloat(0.9215f, 1.1205f);
                if (item.Calamity().rogue || wearingRogueArmor)
                {
                    float roll2 = Main.rand.NextFloat(0.9215f, 1.1205f);
                    diceMult = roll2 > diceMult ? roll2 : diceMult;
                }
                damage = (int)(damage * diceMult);
            }
            #endregion

            #region AdditiveBoosts
            if (item.type == ItemID.Excalibur || item.type == ItemID.TrueExcalibur)
            {
                if (target.life > (int)(target.lifeMax * 0.75))
                    damage *= 2;
            }
            if (item.type == ItemID.TitaniumSword)
            {
                int knockbackAdd = (int)(damage * 0.15 * (1f - target.knockBackResist));
                damage += knockbackAdd;
            }
            if (item.type == ItemID.AntlionClaw || item.type == ItemID.BoneSword || item.type == ItemID.BreakerBlade)
            {
                int defenseAdd = (int)(target.defense * 0.25);
                damage += defenseAdd;
            }
            if (item.type == ItemID.StylistKilLaKillScissorsIWish || (item.type >= ItemID.BluePhaseblade && item.type <= ItemID.YellowPhaseblade) || (item.type >= ItemID.BluePhasesaber && item.type <= ItemID.YellowPhasesaber))
            {
                int defenseAdd = (int)(target.defense * 0.5);
                damage += defenseAdd;
            }
			if (item.melee && badgeOfBravery)
			{
				int penetratableDefense = Math.Max(target.defense - player.armorPenetration, 0);
				int penetratedDefense = Math.Min(penetratableDefense, 5);
				damage += (int)(0.5f * penetratedDefense);
			}
            #endregion

            if (draedonsHeart)
            {
                if (player.StandingStill() && player.itemAnimation == 0)
                    damage = (int)(damage * 0.5);
            }

            if ((target.damage > 0 || target.boss) && !target.SpawnedFromStatue && player.whoAmI == Main.myPlayer)
            {
                if (CalamityConfig.Instance.Proficiency)
                {
                    if (gainLevelCooldown <= 0)
                    {
                        gainLevelCooldown = 120;
                        if (meleeLevel <= 12500)
                        {
                            if (!ReduceCooldown((int)ClassType.Melee))
                            {
                                if (!Main.hardMode && meleeLevel >= 1500)
                                    gainLevelCooldown = 1200; //20 seconds
                                if (!NPC.downedMoonlord && meleeLevel >= 5500)
                                    gainLevelCooldown = 2400; //40 seconds
                            }
                            else
                                gainLevelCooldown /= 2;

                            if (fasterMeleeLevel)
                                gainLevelCooldown /= 2;

                            meleeLevel++;
                            shootFireworksLevelUpMelee = true;

                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                SyncLevel(false, (int)ClassType.Melee);
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
            Item heldItem = player.ActiveItem();

            #region MultiplierBoosts
            double damageMult = 1.0;
            if (isSummon)
            {
                if (heldItem.type > ItemID.None)
                {
                    if (heldItem.summon && !heldItem.melee && !heldItem.ranged && !heldItem.magic && !heldItem.Calamity().rogue)
                        damageMult += 0.1;
                }
            }

            if (isTrueMelee)
            {
				// Add more true melee damage to the true melee projectile that scales with melee speed
				// This is done because melee speed does nothing to melee weapons that are purely projectiles
                damageMult += trueMeleeDamage + ((1f - player.meleeSpeed) * (100f / player.meleeSpeed) / 100f * 0.25f);
            }

            if (screwdriver)
            {
                if (proj.penetrate > 1 || proj.penetrate == -1)
                    damageMult += 0.05;
            }

			if (enraged)
                damageMult += 1.25;

            // Calamity buffs Inferno Fork by 33%.
			// However, because the weapon is coded like spaghetti, you have to multiply the explosion's damage too.
			if (proj.type == ProjectileID.InfernoFriendlyBlast)
                damageMult += 0.33;

            if (brimflameFrenzy && brimflameSet)
            {
                if (proj.magic)
                    damageMult += 0.3;
            }

            if (witheredDebuff)
                damageMult += 0.6;

            if (CalamityWorld.revenge)
                CalamityUtils.ApplyRippersToDamage(this, isTrueMelee, ref damageMult);

            if (filthyGlove && proj.Calamity().stealthStrike && proj.Calamity().rogue)
            {
                if (nanotech)
                    damageMult += 0.05;
                else
                    damageMult += 0.1;
            }

			// Adjust damage based on the damage multiplier
            damage = (int)(damage * damageMult);

			// Old Die damage multiplier, rolls again and takes the higher roll if it's a rogue projectile or the player is wearing rogue armor
            if (oldDie)
            {
                float diceMult = Main.rand.NextFloat(0.9215f, 1.1205f);
                if (proj.Calamity().rogue || wearingRogueArmor)
                {
                    float roll2 = Main.rand.NextFloat(0.9215f, 1.1205f);
                    diceMult = roll2 > diceMult ? roll2 : diceMult;
                }
                damage = (int)(damage * diceMult);
            }
            #endregion

            #region AdditiveBoosts
            if (proj.type == ProjectileID.Gungnir)
            {
                if (target.life > (int)(target.lifeMax * 0.75))
                    damage *= 2;
            }
            if (proj.type == ProjectileID.TitaniumTrident)
            {
                int knockbackAdd = (int)(damage * 0.15 * (1f - target.knockBackResist));
                damage += knockbackAdd;
            }
            if (proj.type == ModContent.ProjectileType<AcidBulletProj>())
            {
                int defenseAdd = (int)(target.defense * 0.05 * (proj.damage / 50D) * acidRoundMultiplier); //100 defense * 0.05 = 5
                damage += defenseAdd;
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
				// Nanotech is a total of 20 as it has all three bools
                if (nanotech)
                    penetrateAmt += 10;
                if (filthyGlove || bloodyGlove)
                    penetrateAmt += 10;
            }
			if (proj.melee && badgeOfBravery)
			{
				penetrateAmt += 5;
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
            bool reducedNerf = fearmongerSet || (forbidden && heldItem.magic) || (GemTechSet && GemTechState.IsBlueGemActive);

            double summonNerfMult = reducedNerf ? 0.75 : 0.5;
            if (isSummon && heldItem.type > ItemID.None && !profanedCrystalBuffs)
            {
                bool classCheck = !heldItem.summon && (heldItem.melee || heldItem.ranged || heldItem.magic || heldItem.thrown || heldItem.Calamity().rogue);
                bool toolCheck = heldItem.pick == 0 && heldItem.axe == 0 && heldItem.hammer == 0;
                bool itemCanBeUsed = heldItem.useStyle != 0;
                bool notAccessoryOrAmmo = !heldItem.accessory && heldItem.ammo == AmmoID.None;
                if (classCheck && itemCanBeUsed && toolCheck && notAccessoryOrAmmo)
                    damage = (int)(damage * summonNerfMult);
            }

            if (proj.ranged)
            {
                switch (proj.type)
                {
                    case ProjectileID.CrystalShard:
                        damage = (int)(damage * 0.6);
                        break;
                    case ProjectileID.HallowStar:
                        damage = (int)(damage * 0.5);
                        break;
                }

                if (proj.type == ModContent.ProjectileType<AcidBulletProj>() && heldItem.type == ModContent.ItemType<P90>())
                    damage = (int)(damage * 0.75);
            }

            if (proj.type == ProjectileID.SpectreWrath && player.ghostHurt)
                damage = (int)(damage * 0.7);

            if (draedonsHeart)
            {
                if (player.StandingStill() && player.itemAnimation == 0)
                    damage = (int)(damage * 0.5);
            }

            #endregion

            // Handle on-hit ranged effects for the gem tech armor set.
            if (proj.ranged && proj.type != ModContent.ProjectileType<GemTechGreenFlechette>())
                GemTechState.RangedOnHitEffects(target, damage);

            if ((target.damage > 0 || target.boss) && !target.SpawnedFromStatue && CalamityConfig.Instance.Proficiency)
            {
                if (gainLevelCooldown <= 0) //max is 12501 to avoid setting off fireworks forever
                {
                    gainLevelCooldown = 120; //2 seconds
                    if (proj.melee && meleeLevel <= 12500)
                    {
                        if (!ReduceCooldown((int)ClassType.Melee))
                        {
                            if (!Main.hardMode && meleeLevel >= 1500)
                                gainLevelCooldown = 1200; //20 seconds
                            if (!NPC.downedMoonlord && meleeLevel >= 5500)
                                gainLevelCooldown = 2400; //40 seconds
                        }
                        else
                            gainLevelCooldown /= 2;

                        if (fasterMeleeLevel)
                            gainLevelCooldown /= 2;

                        meleeLevel++;
                        shootFireworksLevelUpMelee = true;

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            SyncLevel(false, (int)ClassType.Melee);
                    }
                    else if (proj.ranged && rangedLevel <= 12500)
                    {
                        if (!ReduceCooldown((int)ClassType.Ranged))
                        {
                            if (!Main.hardMode && rangedLevel >= 1500)
                                gainLevelCooldown = 1200; //20 seconds
                            if (!NPC.downedMoonlord && rangedLevel >= 5500)
                                gainLevelCooldown = 2400; //40 seconds
                        }
                        else
                            gainLevelCooldown /= 2;

                        if (fasterRangedLevel)
                            gainLevelCooldown /= 2;

                        rangedLevel++;
                        shootFireworksLevelUpRanged = true;

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            SyncLevel(false, (int)ClassType.Ranged);
                    }
                    else if (proj.magic && magicLevel <= 12500)
                    {
                        if (!ReduceCooldown((int)ClassType.Magic))
                        {
                            if (!Main.hardMode && magicLevel >= 1500)
                                gainLevelCooldown = 1200; //20 seconds
                            if (!NPC.downedMoonlord && magicLevel >= 5500)
                                gainLevelCooldown = 2400; //40 seconds
                        }
                        else
                            gainLevelCooldown /= 2;

                        if (fasterMagicLevel)
                            gainLevelCooldown /= 2;

                        magicLevel++;
                        shootFireworksLevelUpMagic = true;

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            SyncLevel(false, (int)ClassType.Magic);
                    }
                    else if (proj.IsSummon() && summonLevel <= 12500)
                    {
                        if (!ReduceCooldown((int)ClassType.Summon))
                        {
                            if (!Main.hardMode && summonLevel >= 1500)
                                gainLevelCooldown = 1200; //20 seconds
                            if (!NPC.downedMoonlord && summonLevel >= 5500)
                                gainLevelCooldown = 2400; //40 seconds
                        }
                        else
                            gainLevelCooldown /= 2;

                        if (fasterSummonLevel)
                            gainLevelCooldown /= 2;

                        summonLevel++;
                        shootFireworksLevelUpSummon = true;

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            SyncLevel(false, (int)ClassType.Summon);
                    }
                    else if (proj.Calamity().rogue && rogueLevel <= 12500)
                    {
                        if (!ReduceCooldown((int)ClassType.Rogue))
                        {
                            if (!Main.hardMode && rogueLevel >= 1500)
                                gainLevelCooldown = 1200; //20 seconds
                            if (!NPC.downedMoonlord && rogueLevel >= 5500)
                                gainLevelCooldown = 2400; //40 seconds
                        }
                        else
                            gainLevelCooldown /= 2;

                        if (fasterRogueLevel)
                            gainLevelCooldown /= 2;

                        rogueLevel++;
                        shootFireworksLevelUpRogue = true;

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            SyncLevel(false, (int)ClassType.Rogue);
                    }
                }
            }
        }
        #endregion

        #region Modify Hit By NPC
        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            int bossRushDamage = (Main.expertMode ? 400 : 240) + (BossRushEvent.BossRushStage * 2);
            if (BossRushEvent.BossRushActive)
            {
                if (damage < bossRushDamage)
                    damage = bossRushDamage;
            }

			// Enemies deal less contact damage while sick, due to being weakened.
			if (npc.poisoned)
			{
				int damageReductionFromPoison = (int)(damage * (npc.Calamity().irradiated > 0 ? 0.075 : 0.05));
				if (npc.Calamity().VulnerableToSickness.HasValue)
				{
					if (npc.Calamity().VulnerableToSickness.Value)
						damageReductionFromPoison *= 2;
					else
						damageReductionFromPoison /= 2;
				}

				damage -= damageReductionFromPoison;
			}

			if (npc.venom)
			{
				int damageReductionFromVenom = (int)(damage * (npc.Calamity().irradiated > 0 ? 0.075 : 0.05));
				if (npc.Calamity().VulnerableToSickness.HasValue)
				{
					if (npc.Calamity().VulnerableToSickness.Value)
						damageReductionFromVenom *= 2;
					else
						damageReductionFromVenom /= 2;
				}

				damage -= damageReductionFromVenom;
			}

			if (npc.Calamity().astralInfection > 0)
			{
				int damageReductionFromAstralInfection = (int)(damage * (npc.Calamity().irradiated > 0 ? 0.075 : 0.05));
				if (npc.Calamity().VulnerableToSickness.HasValue)
				{
					if (npc.Calamity().VulnerableToSickness.Value)
						damageReductionFromAstralInfection *= 2;
					else
						damageReductionFromAstralInfection /= 2;
				}

				damage -= damageReductionFromAstralInfection;
			}

			if (npc.Calamity().pFlames > 0)
			{
				int damageReductionFromPlague = (int)(damage * (npc.Calamity().irradiated > 0 ? 0.075 : 0.05));
				if (npc.Calamity().VulnerableToSickness.HasValue)
				{
					if (npc.Calamity().VulnerableToSickness.Value)
						damageReductionFromPlague *= 2;
					else
						damageReductionFromPlague /= 2;
				}

				damage -= damageReductionFromPlague;
			}

			if (npc.Calamity().wDeath > 0)
			{
				int damageReductionFromWhisperingDeath = (int)(damage * (npc.Calamity().irradiated > 0 ? 0.15 : 0.1));
				if (npc.Calamity().VulnerableToSickness.HasValue)
				{
					if (npc.Calamity().VulnerableToSickness.Value)
						damageReductionFromWhisperingDeath *= 2;
					else
						damageReductionFromWhisperingDeath /= 2;
				}

				damage -= damageReductionFromWhisperingDeath;
			}

			// Check if the player has iframes for the sake of avoiding defense damage.
			bool hasIFrames = false;
            for (int i = 0; i < player.hurtCooldowns.Length; i++)
                if (player.hurtCooldowns[i] > 0)
                    hasIFrames = true;

            // If this NPC deals defense damage with contact damage, then apply defense damage.
            // Defense damage is not applied if the player has iframes, is otherwise invincible, or has Lul equipped.
            if (npc.Calamity().canBreakPlayerDefense && !hasIFrames && !invincible && !lol)
                DealDefenseDamage(damage);

            if (areThereAnyDamnBosses && CalamityMod.bossVelocityDamageScaleValues.ContainsKey(npc.type))
            {
                CalamityMod.bossVelocityDamageScaleValues.TryGetValue(npc.type, out float velocityScalar);

                if (((npc.type == NPCID.EyeofCthulhu || npc.type == NPCID.Spazmatism) && npc.ai[0] >= 2f) || (npc.type == NPCID.Plantera && npc.life / (float)npc.lifeMax <= 0.5f) ||
					(npc.type == ModContent.NPCType<Apollo>() && npc.life / (float)npc.lifeMax < 0.6f))
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

            if (npc.Calamity().tSad > 0)
                contactDamageReduction += 0.5;

            if (npc.Calamity().relicOfResilienceWeakness > 0)
            {
                contactDamageReduction += RelicOfResilience.WeaknessDR;
                npc.Calamity().relicOfResilienceWeakness = 0;
            }

            if (beeResist)
            {
                if (CalamityLists.beeEnemyList.Contains(npc.type))
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
                contactDamageReduction += 0.15 - abyssalDivingSuitPlateHits * 0.03;

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
				if (adrenaline == adrenalineMax && !adrenalineModeActive)
				{
					double adrenalineDRBoost = 0D +
						(adrenalineBoostOne ? 0.05 : 0D) +
						(adrenalineBoostTwo ? 0.05 : 0D) +
						(adrenalineBoostThree ? 0.05 : 0D);
					contactDamageReduction += 0.5 + adrenalineDRBoost;
				}
            }

            if (player.mount.Active && (player.mount.Type == ModContent.MountType<AngryDogMount>() || player.mount.Type == ModContent.MountType<OnyxExcavator>()) && Math.Abs(player.velocity.X) > player.mount.RunSpeed / 2f)
                contactDamageReduction += 0.1;

            if (vHex)
                contactDamageReduction -= 0.1;

            if (irradiated)
                contactDamageReduction -= 0.1;

            if (corrEffigy)
                contactDamageReduction -= 0.05;

            if (calamityRing && !voidOfExtinction)
                contactDamageReduction -= 0.15;

            // 10% is converted to 9%, 25% is converted to 20%, 50% is converted to 33%, 75% is converted to 43%, 100% is converted to 50%
            if (contactDamageReduction > 0D)
            {
                if (aCrunch)
                    contactDamageReduction *= 0.33;

                if (wCleave)
                    contactDamageReduction *= 0.75;

                // Contact damage reduction is reduced by DR Damage, which itself is proportional to defense damage
                if (totalDefenseDamage > 0 && defenseStat > 0)
                {
					double drDamageRatio = CurrentDefenseDamage / (double)defenseStat;
					if (drDamageRatio > 1D)
						drDamageRatio = 1D;

                    contactDamageReduction *= 1D - drDamageRatio;
                    if (contactDamageReduction < 0D)
                        contactDamageReduction = 0D;
				}

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
                {
                    damage = (int)(damage * 0.6);

                    if (damage < 1)
                        damage = 1;
                }
            }

            if (aBulwarkRare)
            {
                aBulwarkRareMeleeBoostTimer += 3 * damage;
                if (aBulwarkRareMeleeBoostTimer > 900)
                    aBulwarkRareMeleeBoostTimer = 900;
            }
        }
        #endregion

        #region Modify Hit By Proj
        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
			if (CalamityLists.projectileDestroyExceptionList.TrueForAll(x => proj.type != x) && proj.active && !proj.friendly && proj.hostile && damage > 0)
			{
				// Reflects count as dodges. They share the timer and can be disabled by Armageddon right click.
                if (dodgeCooldownTimer == 0 && !disableAllDodges)
				{
					// The Evolution
                    if (projRefRare)
					{
						proj.hostile = false;
						proj.friendly = true;
						proj.velocity *= -2f;
						proj.extraUpdates += 1;
						proj.penetrate = 1;
                        player.GiveIFrames(20, false);

                        damage = 0;
						projRefRareLifeRegenCounter = 300;
						projTypeJustHitBy = proj.type;

                        dodgeCooldownTimer = EvolutionReflectCooldown;
                        // Send a Calamity dodge cooldown packet.
                        if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
                            SyncDodgeCooldown(false);
                        return;
					}
				}
			}

            if (phantomicArtifact && player.ownedProjectileCounts[ModContent.ProjectileType<PhantomicShield>()] != 0)
            {
                Projectile pro = Main.projectile.AsEnumerable().Where(projectile => projectile.friendly && projectile.owner == player.whoAmI && projectile.type == ModContent.ProjectileType<PhantomicShield>()).First();
                phantomicBulwarkCooldown = 1800; // 30 second cooldown
                pro.Kill();
				projectileDamageReduction += 0.2;
            }

            if (auralisAuroraCounter >= 300)
            {
                damage -= 100;
                if (damage < 1)
                    damage = 1;
                auralisAuroraCounter = 0;
                auralisAuroraCooldown = CalamityUtils.SecondsToFrames(30f);
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

            if (CalamityWorld.revenge)
            {
                double damageMultiplier = 1D;
                bool containsProjectile = false;
                if (CalamityLists.revengeanceProjectileBuffList25Percent.Contains(proj.type))
                {
                    damageMultiplier += 0.25;
                    containsProjectile = true;
                }
                else if (CalamityLists.revengeanceProjectileBuffList20Percent.Contains(proj.type))
                {
                    damageMultiplier += 0.2;
                    containsProjectile = true;
                }
                else if (CalamityLists.revengeanceProjectileBuffList15Percent.Contains(proj.type))
                {
                    damageMultiplier += 0.15;
                    containsProjectile = true;
                }

                if (containsProjectile)
                {
                    if (CalamityWorld.death)
                        damageMultiplier += (damageMultiplier - 1D) * 0.6;

                    damage = (int)(damage * damageMultiplier);
                }
            }

            int bossRushDamage = (Main.expertMode ? 90 : 110) + (BossRushEvent.BossRushStage / 2);
            if (BossRushEvent.BossRushActive)
            {
                if (damage < bossRushDamage)
                    damage = bossRushDamage;
            }

            // Check if the player has iframes for the sake of avoiding defense damage.
            bool hasIFrames = false;
            for (int i = 0; i < player.hurtCooldowns.Length; i++)
                if (player.hurtCooldowns[i] > 0)
                    hasIFrames = true;

            // If this projectile is capable of dealing defense damage, then apply defense damage.
            // Defense damage is not applied if the player has iframes, is otherwise invincible, or has Lul equipped.
            if (proj.Calamity().canBreakPlayerDefense && !hasIFrames && !invincible && !lol)
                DealDefenseDamage(damage);

            // Reduce projectile damage based on banner type
            // IMPORTANT NOTE: Rework this in 1.4!
            // TODO -- 1.4 has a banner projectile source system. Rework this.
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
                    if (tile.type == ModContent.TileType<MonsterBanner>())
                    {
                        int bannerType = tile.frameX / 18;

                        int bannerItemType = CalamityUtils.GetBannerItem(bannerType);
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
                if (proj.type == ProjectileID.MartianTurretBolt || proj.type == ProjectileID.GigaZapperSpear || proj.type == ProjectileID.CultistBossLightningOrbArc || proj.type == ModContent.ProjectileType<LightningMark>() || proj.type == ProjectileID.VortexLightning || proj.type == ModContent.ProjectileType<DestroyerElectricLaser>() ||
                    proj.type == ProjectileID.BulletSnowman || proj.type == ProjectileID.BulletDeadeye || proj.type == ProjectileID.SniperBullet || proj.type == ProjectileID.VortexLaser)
                    projectileDamageReduction += 0.5;
            }
			if (CalamityLists.projectileDestroyExceptionList.TrueForAll(x => proj.type != x) && proj.active && !proj.friendly && proj.hostile && damage > 0)
			{
                if (dodgeCooldownTimer == 0 && !disableAllDodges && daedalusReflect && !projRefRare)
				{
					projectileDamageReduction += 0.5;
				}
			}
            

            if (beeResist)
            {
                if (CalamityLists.beeProjectileList.Contains(proj.type))
                    projectileDamageReduction += 0.25;
            }

            if (trinketOfChiBuff)
                projectileDamageReduction += 0.15;

            // Fearmonger set provides 15% multiplicative DR that ignores caps during the Holiday Moons.
            // To prevent abuse, this effect does not work if there are any bosses alive.
            if (fearmongerSet && !areThereAnyDamnBosses && (Main.pumpkinMoon || Main.snowMoon))
                projectileDamageReduction += 0.15;

            if (abyssalDivingSuitPlates)
                projectileDamageReduction += 0.15 - abyssalDivingSuitPlateHits * 0.03;

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
				if (adrenaline == adrenalineMax && !adrenalineModeActive)
				{
					double adrenalineDRBoost = 0D +
						(adrenalineBoostOne ? 0.05 : 0D) +
						(adrenalineBoostTwo ? 0.05 : 0D) +
						(adrenalineBoostThree ? 0.05 : 0D);
					projectileDamageReduction += 0.5 + adrenalineDRBoost;
				}
            }

            if (player.mount.Active && (player.mount.Type == ModContent.MountType<AngryDogMount>() || player.mount.Type == ModContent.MountType<OnyxExcavator>()) && Math.Abs(player.velocity.X) > player.mount.RunSpeed / 2f)
                projectileDamageReduction += 0.1;

            if (vHex)
                projectileDamageReduction -= 0.1;

            if (irradiated)
                projectileDamageReduction -= 0.1;

            if (corrEffigy)
                projectileDamageReduction -= 0.05;

            if (calamityRing && !voidOfExtinction)
                projectileDamageReduction -= 0.15;

            // 10% is converted to 9%, 25% is converted to 20%, 50% is converted to 33%, 75% is converted to 43%, 100% is converted to 50%
            if (projectileDamageReduction > 0D)
            {
                if (aCrunch)
                    projectileDamageReduction *= 0.33;

                if (wCleave)
                    projectileDamageReduction *= 0.75;

                // Projectile damage reduction is reduced by DR Damage, which itself is proportional to defense damage
                if (totalDefenseDamage > 0 && defenseStat > 0)
                {
					double drDamageRatio = CurrentDefenseDamage / (double)defenseStat;
					if (drDamageRatio > 1D)
						drDamageRatio = 1D;

					projectileDamageReduction *= 1D - drDamageRatio;

					if (projectileDamageReduction < 0D)
						projectileDamageReduction = 0D;
				}

                // Scale with base damage reduction
                if (DRStat > 0)
                    projectileDamageReduction *= 1f - (DRStat * 0.01f);

                projectileDamageReduction = 1D / (1D + projectileDamageReduction);
                damage = (int)(damage * projectileDamageReduction);
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
                    else if (proj.type == ModContent.ProjectileType<BrimstoneBarrage>())
                    {
                        if (bannerNPCType == ModContent.NPCType<SoulSlurper>())
                            reduceDamage = !NPC.AnyNPCs(ModContent.NPCType<CalamitasRun3>()) && !NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>()) && !NPC.AnyNPCs(ModContent.NPCType<BrimstoneElemental>());
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
                            reduceDamage = !NPC.AnyNPCs(ModContent.NPCType<StormWeaverHead>()) && !NPC.AnyNPCs(NPCID.CultistBoss) && proj.Calamity().lineColor != 1;
                    }
                    else if (proj.type == ProjectileID.SaucerScrap)
                    {
                        if (bannerNPCType == ModContent.NPCType<ArmoredDiggerHead>())
                            reduceDamage = Main.invasionType != InvasionID.MartianMadness;
                    }
                    else
                    {
                        switch (proj.type)
                        {
                            case ProjectileID.Stinger:
                                if (CalamityLists.hornetList.Contains(bannerNPCType) || CalamityLists.mossHornetList.Contains(bannerNPCType))
                                {
                                    reduceDamage = !NPC.AnyNPCs(NPCID.QueenBee);
                                }
                                break;

                            case ProjectileID.PinkLaser:
                                if (bannerNPCType == NPCID.Gastropod || bannerNPCType == ModContent.NPCType<AstralProbe>())
                                {
                                    reduceDamage = !NPC.AnyNPCs(NPCID.TheDestroyer);
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

			if (npc.type == NPCID.ShadowFlameApparition || (npc.type == NPCID.ChaosBall && (Main.hardMode || areThereAnyDamnBosses)))
			{
				player.AddBuff(ModContent.BuffType<Shadowflame>(), 180);
			}
			else if (npc.type == NPCID.Spazmatism && npc.ai[0] != 1f && npc.ai[0] != 2f && npc.ai[0] != 0f)
			{
				player.AddBuff(BuffID.Bleeding, 300);
			}
			else if (npc.type == NPCID.Plantera && npc.life < npc.lifeMax / 2)
			{
				player.AddBuff(BuffID.Poisoned, 300);
			}
			else if (npc.type == NPCID.PlanterasTentacle)
			{
				player.AddBuff(BuffID.Poisoned, 180);
			}
			else if (npc.type == NPCID.AncientDoom)
			{
				player.AddBuff(ModContent.BuffType<Shadowflame>(), 120);
			}
			else if (npc.type == NPCID.AncientLight)
			{
				player.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
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

            if (proj.hostile)
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
                }
                else if (proj.type == ProjectileID.CultistBossIceMist)
                {
                    player.AddBuff(BuffID.Frozen, 90);
                    player.AddBuff(BuffID.Chilled, 180);
                }
                else if (proj.type == ProjectileID.CultistBossLightningOrbArc)
                {
                    int deathModeDuration = NPC.downedMoonlord ? 80 : NPC.downedPlantBoss ? 40 : Main.hardMode ? 20 : 10;
                    player.AddBuff(BuffID.Electrified, proj.Calamity().lineColor == 1 ? deathModeDuration : 120);
                    // Scaled duration for DM lightning, 2 seconds for Storm Weaver/Cultist lightning
                }
                else if (proj.type == ProjectileID.AncientDoomProjectile)
                {
                    player.AddBuff(ModContent.BuffType<Shadowflame>(), 120);
                }
                else if (proj.type == ProjectileID.CultistBossFireBallClone)
                {
                    player.AddBuff(ModContent.BuffType<Shadowflame>(), 120);
                }
                else if (proj.type == ProjectileID.PhantasmalBolt || proj.type == ProjectileID.PhantasmalEye)
                {
                    player.AddBuff(ModContent.BuffType<Nightwither>(), 180);
                }
                else if (proj.type == ProjectileID.PhantasmalSphere)
                {
                    player.AddBuff(ModContent.BuffType<Nightwither>(), 360);
                }
                else if (proj.type == ProjectileID.PhantasmalDeathray)
                {
                    player.AddBuff(ModContent.BuffType<Nightwither>(), 600);
                }
            }

			// As these reflects do not cancel damage, they need to be in OnHit rather than ModifyHit
			if (CalamityLists.projectileDestroyExceptionList.TrueForAll(x => proj.type != x) && proj.active && !proj.friendly && proj.hostile && damage > 0)
			{
				// The Transformer can reflect bullets
                if (aSparkRare)
				{
					if (proj.type == ProjectileID.BulletSnowman || proj.type == ProjectileID.BulletDeadeye || proj.type == ProjectileID.SniperBullet || proj.type == ProjectileID.VortexLaser)
					{
						proj.hostile = false;
						proj.friendly = true;
						proj.velocity *= -1f;
						proj.damage = (int)(proj.damage * 8 * player.AverageDamage());
                        proj.penetrate = 1;
                        player.GiveIFrames(20, false);
					}
				}

				// Reflects count as dodges. They share the timer and can be disabled by Armageddon right click.
                if (dodgeCooldownTimer == 0 && !disableAllDodges)
				{
					if (daedalusReflect && !projRefRare)
					{
						proj.hostile = false;
						proj.friendly = true;
						proj.velocity *= -1f;
						proj.penetrate = 1;
                        player.GiveIFrames(20, false);

                        damage /= 2;

						dodgeCooldownTimer = DaedalusReflectCooldown;
                        // Send a Calamity dodge cooldown packet.
                        if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
                            SyncDodgeCooldown(false);

                        // No return because the projectile hit isn't canceled -- it only does half damage.
                    }
				}
			}
        }
        #endregion

        #region Shoot
        public override bool Shoot(Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (bladeArmEnchant)
                return false;

            if (rottenDogTooth && item.Calamity().rogue && item.type != ModContent.ItemType<SylvanSlasher>())
                damage = (int)(damage * (1f + RottenDogtooth.StealthStrikeDamageMultiplier));

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
                    int p = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, type, (int)(damage * 0.065), knockBack * 0.5f, player.whoAmI);

                    if (p.WithinBounds(Main.maxProjectiles))
                        Main.projectile[p].Calamity().forceTypeless = true; //in case melee/rogue variants bug out

                    // Handle AI edge-cases.
                    if (item.type == ModContent.ItemType<FinalDawn>())
                        Main.projectile[p].ai[1] = 1f;
                    if (item.type == ModContent.ItemType<TheAtomSplitter>())
                        Main.projectile[p].ai[0] = -1f;

                    if (StealthStrikeAvailable())
                    {
                        int knifeCount = 15;
                        int knifeDamage = (int)(35 * player.RogueDamage());
                        float angleStep = MathHelper.TwoPi / knifeCount;
                        float speed = 15f;

                        for (int i = 0; i < knifeCount; i++)
                        {
                            Vector2 velocity = new Vector2(0f, speed);
                            velocity = velocity.RotatedBy(angleStep * i);
                            int knifeCol = Main.rand.Next(0, 2);

                            int knife = Projectile.NewProjectile(player.Center, velocity, ModContent.ProjectileType<VeneratedKnife>(), knifeDamage, 0f, player.whoAmI, knifeCol, 0);
							if (knife.WithinBounds(Main.maxProjectiles))
								Main.projectile[knife].Calamity().forceTypeless = true;
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
                            int drop = Projectile.NewProjectileDirect(startingPosition, directionToMouse * 12f, ModContent.ProjectileType<ToxicannonDrop>(), (int)(damage * 0.3), 0f, player.whoAmI).penetrate = 2;
							if (drop.WithinBounds(Main.maxProjectiles))
								Main.projectile[drop].Calamity().forceTypeless = true;
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
            else if ((omegaBlueTransformationPower || omegaBlueTransformationForce) && omegaBlueCooldown > 1500)
            {
                player.head = mod.GetEquipSlot("OmegaBlueTransformationHead", EquipType.Head);
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
					if (!player.mount.Active)
					{
						player.velocity.Y *= 0.9f;
						player.wingFrame = 3;
					}
                    player.noFallDmg = true;
                    player.fallStart = (int)(player.position.Y / 16f);
                }
            }
            if (abyssDivingGear && (player.head == -1 || player.head == ArmorIDs.Head.FamiliarWig))
            {
                player.head = mod.GetEquipSlot("AbyssDivingGearHead", EquipType.Head);
                player.face = -1;
            }
            if (featherCrownDraw && (player.head == -1 || player.head == ArmorIDs.Head.FamiliarWig))
            {
                player.head = mod.GetEquipSlot("FeatherCrownHead", EquipType.Head);
                player.face = -1;
            }
            if (moonCrownDraw && (player.head == -1 || player.head == ArmorIDs.Head.FamiliarWig))
            {
                player.head = mod.GetEquipSlot("MoonstoneCrownHead", EquipType.Head);
                player.face = -1;
            }
        }
        #endregion

        #region Limitations
        private void ForceVariousEffects()
        {
            if (blockAllDashes || (CalamityConfig.Instance.BossRushDashCurse && BossRushEvent.BossRushActive))
                DisableDashes();
            if (weakPetrification)
                WeakPetrification();

			// Disable vanilla dashes during god slayer dash
			if (godSlayerDashHotKeyPressed)
			{
				// Set the player to have no registered vanilla dashes.
				player.dash = 0;

				// Prevent the possibility of Shield of Cthulhu invulnerability exploits.
				player.eocHit = -1;
				if (player.eocDash != 0)
					player.eocDash = 0;
			}

            if (lol || (silvaCountdown > 0 && hasSilvaEffect && silvaSet) || (dashMod == 9 && player.dashDelay < 0))
            {
                if (player.lifeRegen < 0)
                    player.lifeRegen = 0;
            }

            if (player.ownedProjectileCounts[ModContent.ProjectileType<GiantIbanRobotOfDoom>()] > 0)
                player.yoraiz0rEye = 0;
        }

        private void DisableDashes()
        {
            // Set the player to have no registered dashes.
            player.dash = 0;
            dashMod = 0;

            // Put the player in a permanent state of dash cooldown. This is removed 1/5 of a second after disabling the effect.
            // This is necessary so that arbitrary dashes from other mods are also blocked by Calamity.
            if (player.dashDelay >= 0 && player.dashDelay < DashDisableCooldown)
                player.dashDelay = DashDisableCooldown;

            // Prevent the possibility of Shield of Cthulhu invulnerability exploits.
            player.eocHit = -1;
            if (player.eocDash != 0)
                player.eocDash = 0;
        }

        private void WeakPetrification()
        {
            weakPetrification = true;
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
        }
        #endregion

        #region Pre Hurt
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
			#region Ignore Incoming Hits
			// If any dodges are active which could dodge this hit, the hurting event is canceled (and the dodge is used).
			if (HandleDodges())
				return false;

			// Lul makes the player completely invincible.
			if (lol)
                return false;

            // Unless holding Coldheart Icicle, the Purified Jam makes you completely invincible.
            if (invincible && player.ActiveItem().type != ModContent.ItemType<ColdheartIcicle>())
                return false;

            // If Armageddon is active or the Boss Rush Immunity Curse is triggered, instantly kill the player.
            if (CalamityWorld.armageddon || (BossRushEvent.BossRushActive && bossRushImmunityFrameCurseTimer > 0))
            {
                if (areThereAnyDamnBosses || (BossRushEvent.BossRushActive && bossRushImmunityFrameCurseTimer > 0))
                {
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
            #endregion

            //
            // At this point, the player is guaranteed to be hit.
            // The amount of damage that will be dealt is yet to be determined.
            //

            #region Custom Hurt Sounds
            // TODO -- Shouldn't these all not occur in favor of the Iron Heart hurt noise if Iron Heart is on?
            if (hurtSoundTimer == 0)
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
            #endregion

            #region Player Incoming Damage Multiplier (Increases)
            double damageMult = 1D;
            if (dArtifact) // Dimensional Soul Artifact increases incoming damage by 15%.
                damageMult += 0.15;
            if (enraged) // Demonshade Enrage increases incoming damage by 25%.
                damageMult += 0.25;

            // Add 5% damage multiplier for each Beetle Shell beetle that is active, thus reducing the DR from 10% to 5% per stack.
            if (player.beetleDefense && player.beetleOrbs > 0)
                damageMult += 0.05 * player.beetleOrbs;

            // If inflicted with Cursed Inferno, take 20% more damage.
            // This is the equivalent to reducing DR by 20%, except it works on you even when you have less than 20% DR.
            if (player.onFire2)
                damageMult += 0.2;

            // Blood Pact gives you a 1/4 chance to be crit, increasing the incoming damage by 25%.
            if (bloodPact && Main.rand.NextBool(4))
            {
                player.AddBuff(ModContent.BuffType<BloodyBoost>(), 600);
                damageMult += 1.25;
            }

            damage = (int)(damage * damageMult);
            #endregion

            //
            // At this point, the true, final incoming damage to the player has been calculated.
            // It has not yet been mitigated by any means.
            //

            // God Slayer Damage Resistance makes you ignore hits that came in as less than 80.
            if ((godSlayerDamage && damage <= 80) || damage < 1)
                damage = 1;

            // Shattered Community makes the player gain rage based on the amount of damage taken.
            // Also set the Rage gain cooldown to prevent bizarre abuse cases.
            if (CalamityWorld.revenge && shatteredCommunity && rageGainCooldown == 0)
            {
                float HPRatio = (float)damage / player.statLifeMax2;
                float rageConversionRatio = 0.8f;

                // Damage to rage conversion is half as effective while Rage Mode is active.
                if (rageModeActive)
                    rageConversionRatio *= 0.5f;
                // If Rage is over 100%, damage to rage conversion scales down asymptotically based on how full Rage is.
                if (rage >= rageMax)
                    rageConversionRatio *= 3f / (3f + rage / rageMax);

                rage += rageMax * HPRatio * rageConversionRatio;
                rageGainCooldown = DefaultRageGainCooldown;
                // Rage capping is handled in MiscEffects
            }

            // Resilient Candle makes defense 5% more effective, aka 5% of defense is subtracted from all incoming damage.
            if (purpleCandle)
                damage = (int)(damage - (player.statDefense * 0.05));

            return true;
        }
        #endregion

        #region Hurt
        public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            modStealth = 1f;

            // Give Rage combat frames because being hurt counts as combat.
            if (CalamityWorld.revenge)
                rageCombatFrames = RageCombatDelayTime;

            if (player.whoAmI == Main.myPlayer)
            {
                // Summon a portal if needed.
                if (player.Calamity().persecutedEnchant && NPC.CountNPCS(ModContent.NPCType<DemonPortal>()) < 2)
                {
                    int tries = 0;
                    Vector2 spawnPosition;
                    do
                    {
                        spawnPosition = player.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(270f, 420f);
                        tries++;
                    }
                    while (Collision.SolidCollision(spawnPosition - Vector2.One * 24f, 48, 24) && tries < 100);
                    CalamityNetcode.NewNPC_ClientSide(spawnPosition, ModContent.NPCType<DemonPortal>(), player);
                }

				if (revivify)
				{
					int healAmt = (int)(damage / 15D);
					player.statLife += healAmt;
					player.HealEffect(healAmt);
				}

				if (daedalusAbsorb && Main.rand.NextBool(10))
				{
					int healAmt = (int)(damage / 2D);
					player.statLife += healAmt;
					player.HealEffect(healAmt);
				}

				if (absorber)
				{
					int healAmt = (int)(damage / (sponge ? 16D : 20D));
					player.statLife += healAmt;
					player.HealEffect(healAmt);
				}

                if (witheringDamageDone > 0)
                {
                    double healCompenstationRatio = Math.Log(witheringDamageDone) * Math.Pow(witheringDamageDone, 2D / 3D) / 177000D;
                    if (healCompenstationRatio > 1D)
                        healCompenstationRatio = 1D;
                    int healCompensation = (int)(healCompenstationRatio * damage);
                    player.statLife += (int)(healCompenstationRatio * damage);
                    player.HealEffect(healCompensation);
                    player.AddBuff(ModContent.BuffType<Withered>(), 1080);
                    witheringDamageDone = 0;
                }

                if (CalamityWorld.revenge)
                {
                    if (!adrenalineModeActive && damage > 0) // To prevent paladin's shield ruining adren even with 0 dmg taken
                    {
                        adrenaline -= stressPills ? adrenalineMax / 2 : adrenalineMax;
                        if (adrenaline < 0)
                            adrenaline = 0;
                    }
                }

				if (evilSmasherBoost > 0)
					evilSmasherBoost -= 1;

				hellbornBoost = 0;

                if (amidiasBlessing)
                {
                    player.ClearBuff(ModContent.BuffType<AmidiasBlessing>());
                    Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 96);
                }

                if ((gShell || fabledTortoise) && !player.panic)
                    player.AddBuff(ModContent.BuffType<ShellBoost>(), 180);

                if (abyssalDivingSuitPlates && damage > 50)
                {
                    if (abyssalDivingSuitPlateHits < 3)
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
                                Main.dust[dust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
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
                        player.AddBuff(ModContent.BuffType<TarraLifeRegen>(), 120);
                }
                else if (xerocSet)
                {
                    player.AddBuff(ModContent.BuffType<XerocRage>(), 240);
                    player.AddBuff(ModContent.BuffType<XerocWrath>(), 240);
                }
                else if (reaverDefense)
                {
                    if (Main.rand.NextBool(4))
                        player.AddBuff(ModContent.BuffType<ReaverRage>(), 180);
                }

                if ((fBarrier || (sirenBoobs && NPC.downedBoss3)) && !areThereAnyDamnBosses)
                {
                    Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 27);
                    for (int m = 0; m < Main.maxNPCs; m++)
                    {
                        NPC npc = Main.npc[m];
                        if (!npc.active || npc.friendly || npc.dontTakeDamage)
                            continue;

                        float npcDist = (npc.Center - player.Center).Length();
                        float freezeDist = Main.rand.Next(200 + (int)damage / 2, 301 + (int)damage * 2);
                        if (freezeDist > 500f)
                            freezeDist = 500f + (freezeDist - 500f) * 0.75f;
                        if (freezeDist > 700f)
                            freezeDist = 700f + (freezeDist - 700f) * 0.5f;
                        if (freezeDist > 900f)
                            freezeDist = 900f + (freezeDist - 900f) * 0.25f;

                        if (npcDist < freezeDist)
                        {
                            float duration = Main.rand.Next(10 + (int)damage / 4, 20 + (int)damage / 3);
							if (duration > 120)
								duration = 120;

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
                        float range = Main.rand.Next(200 + (int)damage / 2, 301 + (int)damage * 2);
                        if (range > 500f)
                            range = 500f + (range - 500f) * 0.75f;
                        if (range > 700f)
                            range = 700f + (range - 700f) * 0.5f;
                        if (range > 900f)
                            range = 900f + (range - 900f) * 0.25f;

                        if (npcDist < range)
                        {
                            float duration = Main.rand.Next(300 + (int)damage / 3, 480 + (int)damage / 2);
                            npc.AddBuff(BuffID.Confused, (int)duration, false);
                            if (amalgam)
                            {
                                npc.AddBuff(ModContent.BuffType<BrimstoneFlames>(), (int)duration);
                                npc.AddBuff(ModContent.BuffType<GodSlayerInferno>(), (int)duration);
                                npc.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), (int)duration);
                                npc.AddBuff(ModContent.BuffType<Irradiated>(), (int)duration);
                            }
                        }
                    }

                    // Spawn the harmless brain images that are actually projectiles
                    Projectile.NewProjectile(player.Center.X + Main.rand.Next(-40, 40), player.Center.Y - Main.rand.Next(20, 60), player.velocity.X * 0.3f, player.velocity.Y * 0.3f, ProjectileID.BrainOfConfusion, 0, 0f, player.whoAmI);
                }

                if (polarisBoost)
                {
                    polarisBoostCounter = 0;
                    polarisBoost = false;
                    polarisBoostTwo = false;
                    polarisBoostThree = false;
                    if (player.FindBuffIndex(ModContent.BuffType<PolarisBuff>()) > -1)
						player.ClearBuff(ModContent.BuffType<PolarisBuff>());
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
                player.AddBuff(ModContent.BuffType<BurntOut>(), 300, true);

            // Bloodflare Core defense shattering
            if (bloodflareCore)
            {
                // Shattered defense has a hard cap equal to half of total defense.
                // It also has a soft cap determined by a formula so it isn't too powerful at excessively high defense.
                int shatterDefenseCap = (int)(1.5D * Math.Pow(player.statDefense, 0.91D) - 0.5D * player.statDefense);
                if (shatterDefenseCap > player.statDefense / 2)
                    shatterDefenseCap = player.statDefense / 2;

                // Every hit adds its damage as shattered defense.
                int newLostDefense = Math.Min(bloodflareCoreLostDefense + (int)damage, shatterDefenseCap);

                // Suddenly reducing your base defense stat does not let you suddenly reduce your shattered defense cap.
                // In other words, you can't ever reduce your lost defense by taking another hit.
                if (bloodflareCoreLostDefense < newLostDefense)
                    bloodflareCoreLostDefense = newLostDefense;

                // Play a sound and make dust to signify that defense has been shattered
                Main.PlaySound(SoundID.DD2_MonkStaffGroundImpact, player.Center);
                for (int i = 0; i < 36; ++i)
                {
                    float speed = Main.rand.NextFloat(1.8f, 8f);
                    Vector2 dustVel = new Vector2(speed, speed);
                    Dust d = Dust.NewDustDirect(player.position, player.width, player.height, 90);
                    d.velocity = dustVel;
                    d.noGravity = true;
                    d.scale *= Main.rand.NextFloat(1.1f, 1.4f);
                    Dust.CloneDust(d).velocity = dustVel.RotatedBy(MathHelper.PiOver2);
                    Dust.CloneDust(d).velocity = dustVel.RotatedBy(MathHelper.Pi);
                    Dust.CloneDust(d).velocity = dustVel.RotatedBy(MathHelper.Pi * 1.5f);
                }
            }

            // Handle hit effects from the gem tech armor set.
            player.Calamity().GemTechState.PlayerOnHitEffects((int)damage);

            bool hardMode = Main.hardMode;
            if (player.whoAmI == Main.myPlayer)
            {
                int iFramesToAdd = 0;
                if (cTracers && damage > 200)
                    iFramesToAdd += 30;
                if (godSlayerThrowing && damage > 80)
                    iFramesToAdd += 30;
                if (statigelSet && damage > 100)
                    iFramesToAdd += 30;

                if (dAmulet)
                {
                    if (damage == 1)
                        iFramesToAdd += 5;
                    else
                        iFramesToAdd += 10;
                }

                if (fabsolVodka)
                {
                    if (damage == 1)
                        iFramesToAdd += 5;
                    else
                        iFramesToAdd += 10;
                }

                // TODO -- good god what the fuck is this system
                // Projectiles should be providing an index to a temporary variable in Player.OnHitByProjectile, not hardcoding it in their own OnHitPlayer
                //
                // To my best understanding the point of this system is to avoid giving the player type-0 iframes if they are hit by a type-1 projectile.
                // Why did vanilla Moon Lord have to hate Slime Mount cheese so much to create a second type of iframes?
                // WHY DID THEY NOT JUST FIX SLIME MOUNT?!
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
                            player.GiveIFrames(player.immuneTime + iFramesToAdd, true);
                            break;
                    }
                }

                // In the case that no projectile that hit the player was defined, just give them iframes normally
                else
                    player.GiveIFrames(player.immuneTime + iFramesToAdd, true);

                if (BossRushEvent.BossRushActive && CalamityConfig.Instance.BossRushImmunityFrameCurse)
                    bossRushImmunityFrameCurseTimer = 180 + player.immuneTime;

                if (aeroSet && damage > 25)
                {
                    for (int n = 0; n < 4; n++)
                    {
                        CalamityUtils.ProjectileRain(player.Center, 400f, 100f, 500f, 800f, 20f, ModContent.ProjectileType<StickyFeatherAero>(), (int)(20 * player.AverageDamage()), 1f, player.whoAmI);
                    }
                }
                if (aBulwarkRare)
                {
                    Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 74);
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<GodSlayerBlaze>(), (int)(25 * player.AverageDamage()), 5f, player.whoAmI, 0f, 1f);
                    for (int n = 0; n < 12; n++)
                    {
                        CalamityUtils.ProjectileRain(player.Center, 400f, 100f, 500f, 800f, 29f, ModContent.ProjectileType<AstralStar>(), (int)(320 * player.AverageDamage()), 5f, player.whoAmI);
                    }
                }
                if (dAmulet)
                {
                    for (int n = 0; n < 3; n++)
                    {
                        Projectile star = CalamityUtils.ProjectileRain(player.Center, 400f, 100f, 500f, 800f, 29f, ProjectileID.HallowStar, (int)(130 * player.AverageDamage()), 4f, player.whoAmI);
                        if (star.whoAmI.WithinBounds(Main.maxProjectiles))
                        {
                            star.Calamity().forceTypeless = true;
                            star.usesLocalNPCImmunity = true;
                            star.localNPCHitCooldown = 5;
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
                        int fDamage = (int)(70 * player.AverageDamage());
                        if (player.whoAmI == Main.myPlayer)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                float xPos = Main.rand.NextBool(2) ? player.Center.X + 100 : player.Center.X - 100;
                                Vector2 spawnPos = new Vector2(xPos, player.Center.Y + Main.rand.Next(-100, 101));
                                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                                int spore1 = Projectile.NewProjectile(spawnPos.X, spawnPos.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ProjectileID.TruffleSpore, fDamage, 1.25f, player.whoAmI, 0f, 0f);
                                int spore2 = Projectile.NewProjectile(spawnPos.X, spawnPos.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ProjectileID.TruffleSpore, fDamage, 1.25f, player.whoAmI, 0f, 0f);
                                Main.projectile[spore1].timeLeft = 300;
                                Main.projectile[spore2].timeLeft = 300;
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
                        int sDamage = 6;
                        if (aSparkRare)
                            sDamage += 42;
                        if (player.whoAmI == Main.myPlayer)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                                int spark1 = Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<Spark>(), (int)(sDamage * player.AverageDamage()), 1.25f, player.whoAmI, 0f, 0f);
                                int spark2 = Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<Spark>(), (int)(sDamage * player.AverageDamage()), 1.25f, player.whoAmI, 0f, 0f);
                                if (spark1.WithinBounds(Main.maxProjectiles))
                                {
                                    Main.projectile[spark1].timeLeft = 120;
                                    Main.projectile[spark1].Calamity().forceTypeless = true;
                                }
                                if (spark2.WithinBounds(Main.maxProjectiles))
                                {
                                    Main.projectile[spark2].timeLeft = 120;
                                    Main.projectile[spark2].Calamity().forceTypeless = true;
                                }
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
                            int ink = Projectile.NewProjectile(player.Center.X, player.Center.Y, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-0f, -4f), ModContent.ProjectileType<InkBombProjectile>(), 0, 0, player.whoAmI);
                            if (ink.WithinBounds(Main.maxProjectiles))
                                Main.projectile[ink].Calamity().forceTypeless = true;
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
                        int blazingSun = Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<BlazingSun>(), (int)(1270 * player.AverageDamage()), 0f, player.whoAmI, 0f, 0f);
                        Main.projectile[blazingSun].Center = player.Center;
                        int blazingSun2 = Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<BlazingSun2>(), 0, 0f, player.whoAmI, 0f, 0f);
                        Main.projectile[blazingSun2].Center = player.Center;
                    }
                }
                if (ataxiaBlaze)
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
                                float randomSpeed = Main.rand.Next(1, 7);
                                float randomSpeed2 = Main.rand.Next(1, 7);
                                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                                int shard = Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f) + randomSpeed, ProjectileID.CrystalShard, sDamage, 1f, player.whoAmI, 0f, 0f);
                                int shard2 = Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f) + randomSpeed2, ProjectileID.CrystalShard, sDamage, 1f, player.whoAmI, 0f, 0f);
                                if (shard.WithinBounds(Main.maxProjectiles))
                                    Main.projectile[shard].Calamity().forceTypeless = true;
                                if (shard2.WithinBounds(Main.maxProjectiles))
                                    Main.projectile[shard2].Calamity().forceTypeless = true;
                            }
                        }
                    }
                }
                else if (reaverDefense) //Defense and DR Helm
                {
                    if (damage > 0)
                    {
                        int rDamage = (int)(80 * player.AverageDamage());
                        if (player.whoAmI == Main.myPlayer)
                        {
                            Projectile.NewProjectile(player.Center.X, player.position.Y + 36f, 0f, -18f, ModContent.ProjectileType<ReaverThornBase>(), rDamage, 0f, player.whoAmI, 0f, 0f);
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
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<GodKiller>(), (int)(675 * player.MeleeDamage()), 5f, player.whoAmI, 0f, 0f);
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<GodKiller>(), (int)(675 * player.MeleeDamage()), 5f, player.whoAmI, 0f, 0f);
                            }
                        }
                    }
                }
                else if (dsSetBonus)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (int l = 0; l < 2; l++)
                        {
                            Projectile beam = CalamityUtils.ProjectileRain(player.Center, 400f, 100f, 500f, 800f, 22f, ProjectileID.ShadowBeamFriendly, (int)(3000 * player.AverageDamage()), 7f, player.whoAmI);
                            if (beam.whoAmI.WithinBounds(Main.maxProjectiles))
                            {
                                beam.Calamity().forceTypeless = true;
                                beam.usesLocalNPCImmunity = true;
                                beam.localNPCHitCooldown = 10;
                            }
                        }
                        for (int l = 0; l < 5; l++)
                        {
                            Projectile scythe = CalamityUtils.ProjectileRain(player.Center, 400f, 100f, 500f, 800f, 22f, ProjectileID.DemonScythe, (int)(5000 * player.AverageDamage()), 7f, player.whoAmI);
                            if (scythe.whoAmI.WithinBounds(Main.maxProjectiles))
                            {
                                scythe.Calamity().forceTypeless = true;
                                scythe.usesLocalNPCImmunity = true;
                                scythe.localNPCHitCooldown = 10;
                            }
                        }
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
                SyncDeathCount(false);
            }
            player.lastDeathPostion = player.Center;
            player.lastDeathTime = DateTime.Now;
            player.showLastDeath = true;
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
            Main.PlaySound(SoundID.PlayerKilled, (int)player.position.X, (int)player.position.Y, 1, 1f, 0f);
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
                Dust.NewDust(player.position, player.width, player.height, 235, (float)(2 * 0), -2f, 0, default, 1f);
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
            else if (CalamityWorld.armageddon && areThereAnyDamnBosses)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + " failed the challenge at hand.");
            }
            else if (BossRushEvent.BossRushActive && bossRushImmunityFrameCurseTimer > 0)
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
			if (dashMod == 4 && player.dashDelay < 0 && player.whoAmI == Main.myPlayer) // Asgardian Aegis
            {
                Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && !npc.dontTakeDamage && !npc.friendly && npc.Calamity().dashImmunityTime[player.whoAmI] <= 0)
                    {
                        Rectangle rect = npc.getRect();
						if (rectangle.Intersects(rect) && (npc.noTileCollide || player.CanHit(npc)))
						{
							float num = 300f * player.AverageDamage();
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
							player.ApplyDamageToNPC(npc, (int)num, num2, direction, crit);
							Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<HolyExplosionSupreme>(), (int)(135 * player.AverageDamage()), 20f, Main.myPlayer, 0f, 0f);
							Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<HolyEruption>(), (int)(90 * player.AverageDamage()), 5f, Main.myPlayer, 0f, 0f);
							if (npc.Calamity().dashImmunityTime[player.whoAmI] < 6)
								npc.Calamity().dashImmunityTime[player.whoAmI] = 6;
							npc.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);

                            player.GiveIFrames(AsgardianAegis.ShieldSlamIFrames, false);
						}
                    }
                }
            }
            if (dashMod == 3 && player.dashDelay < 0 && player.whoAmI == Main.myPlayer) // Elysian Aegis
            {
                Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && !npc.dontTakeDamage && !npc.friendly && npc.Calamity().dashImmunityTime[player.whoAmI] <= 0)
                    {
                        Rectangle rect = npc.getRect();
						if (rectangle.Intersects(rect) && (npc.noTileCollide || player.CanHit(npc)))
						{
							float num = 250f * player.AverageDamage();
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
							player.ApplyDamageToNPC(npc, (int)num, num2, direction, crit);
							Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<HolyExplosionSupreme>(), (int)(120 * player.AverageDamage()), 15f, Main.myPlayer, 0f, 0f);
							Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<HolyEruption>(), (int)(80 * player.AverageDamage()), 5f, Main.myPlayer, 0f, 0f);
							if (npc.Calamity().dashImmunityTime[player.whoAmI] < 6)
								npc.Calamity().dashImmunityTime[player.whoAmI] = 6;

                            player.GiveIFrames(ElysianAegis.ShieldSlamIFrames, false);
                        }
                    }
                }
            }
            if (dashMod == 2 && player.dashDelay < 0 && player.whoAmI == Main.myPlayer) // Asgard's Valor
            {
                Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && !npc.dontTakeDamage && !npc.friendly && npc.Calamity().dashImmunityTime[player.whoAmI] <= 0)
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
							player.ApplyDamageToNPC(npc, (int)num, num2, direction, crit);
							Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<HolyExplosion>(), (int)(60 * player.AverageDamage()), 15f, Main.myPlayer, 0f, 0f);
							if (npc.Calamity().dashImmunityTime[player.whoAmI] < 6)
								npc.Calamity().dashImmunityTime[player.whoAmI] = 6;

                            player.GiveIFrames(AsgardsValor.ShieldSlamIFrames, false);
                        }
                    }
                }
            }
			if (dashMod == 6 && player.dashDelay < 0 && player.whoAmI == Main.myPlayer) // Ornate Shield
			{
				Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];
					if (npc.active && !npc.dontTakeDamage && !npc.friendly && npc.Calamity().dashImmunityTime[player.whoAmI] <= 0)
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
							player.ApplyDamageToNPC(npc, (int)num, num2, direction, crit);
							if (npc.Calamity().dashImmunityTime[player.whoAmI] < 6)
								npc.Calamity().dashImmunityTime[player.whoAmI] = 6;
							npc.AddBuff(BuffID.Frostburn, 300);

                            player.GiveIFrames(OrnateShield.ShieldSlamIFrames, false);
                        }
					}
				}
			}
			if (dashMod == 8 && player.dashDelay < 0 && player.whoAmI == Main.myPlayer) // Plaguebringer Armor
            {
                Rectangle rectangle = new Rectangle((int)(player.position.X + player.velocity.X * 0.5f - 4f), (int)(player.position.Y + player.velocity.Y * 0.5f - 4f), player.width + 8, player.height + 8);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && !npc.dontTakeDamage && !npc.friendly && npc.Calamity().dashImmunityTime[player.whoAmI] <= 0)
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
                            player.ApplyDamageToNPC(npc, (int)num, num2, direction, crit);
                            if (npc.Calamity().dashImmunityTime[player.whoAmI] < 6)
								npc.Calamity().dashImmunityTime[player.whoAmI] = 6;
                            npc.AddBuff(ModContent.BuffType<Plague>(), 300);

                            player.GiveIFrames(PlaguebringerVisor.PlagueDashIFrames, false);
                        }
                    }
                }
            }
			if (dashMod == 9 && player.dashDelay < 0 && player.whoAmI == Main.myPlayer) // God Slayer Armor
			{
				Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];
					if (npc.active && !npc.dontTakeDamage && !npc.friendly && npc.Calamity().dashImmunityTime[player.whoAmI] <= 0)
					{
						Rectangle rect = npc.getRect();
						if (rectangle.Intersects(rect) && (npc.noTileCollide || player.CanHit(npc)))
						{
							Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 67);

							for (int j = 0; j < 30; j++)
							{
								int dust = Dust.NewDust(player.position, player.width, player.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
								Dust dust2 = Main.dust[dust];
								dust2.position.X += Main.rand.Next(-20, 21);
								dust2.position.Y += Main.rand.Next(-20, 21);
								dust2.velocity *= 0.9f;
								dust2.scale *= 1f + Main.rand.Next(40) * 0.01f;
								dust2.shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
								if (Main.rand.NextBool(2))
									dust2.scale *= 1f + Main.rand.Next(40) * 0.01f;
							}

							float num = 3000f * player.AverageDamage();
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
							player.ApplyDamageToNPC(npc, (int)num, num2, direction, crit);
							if (npc.Calamity().dashImmunityTime[player.whoAmI] < 6)
								npc.Calamity().dashImmunityTime[player.whoAmI] = 6;
							npc.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);

                            player.GiveIFrames(GodSlayerChestplate.DashIFrames, false);
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
                if (dashMod == 1) // Counter Scarf
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
                else if (dashMod == 2) // Asgard's Valor
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
                else if (dashMod == 3) // Elysian Aegis
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
                    num7 = 14f;
                }
                else if (dashMod == 4) // Asgardian Aegis
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
                    num7 = 16f;
                }
                else if (dashMod == 5) // Deep Diver
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
                    num7 = 18f;
                }
				else if (dashMod == 6) // Ornate Shield
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
					num7 = 12.5f;
				}
				else if (dashMod == 7) // Statis' Belt of Curses
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
                    num7 = 14f;
                    if (statisTimer % 5 == 0)
                    {
                        int scythe = Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<CosmicScythe>(), (int)(250 * player.AverageDamage()), 5f, player.whoAmI);
                        if (scythe.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[scythe].Calamity().forceTypeless = true;
                            Main.projectile[scythe].usesIDStaticNPCImmunity = true;
                            Main.projectile[scythe].idStaticNPCHitCooldown = 10;
                        }
                    }
                }
                else if (dashMod == 8) // Plaguebringer Armor
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
                    num7 = 12.5f;
                }
				else if (dashMod == 9) // God Slayer Armor
				{
					player.maxFallSpeed = 50f;
					for (int m = 0; m < 24; m++)
					{
						int num14 = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 4f), player.width, player.height - 8, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2.75f);
						Main.dust[num14].velocity *= 0.1f;
						Main.dust[num14].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
						Main.dust[num14].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
						Main.dust[num14].noGravity = true;
						if (Main.rand.NextBool(2))
						{
							Main.dust[num14].fadeIn = 0.5f;
						}
					}
					num7 = 40f; // 46 frames before hitting this gate value
					num10 = 0.8f;
				}
				if (dashMod > 0)
                {
                    player.vortexStealthActive = false;

					if (dashMod == 9)
					{
						if (player.velocity.Length() > num7)
						{
							player.velocity *= num8;
							return;
						}
						if (player.velocity.Length() > num9)
						{
							player.velocity *= num10;
							return;
						}
					}
					else
					{
						if (player.velocity.X > num7 || player.velocity.X < -num7)
						{
							player.velocity.X *= num8;
							return;
						}
						if (player.velocity.X > num9 || player.velocity.X < -num9)
						{
							player.velocity.X *= num10;
							return;
						}
					}

                    player.dashDelay = delay;

					// Cooldown for God Slayer Armor dash
					if (dashMod == 9)
					{
						player.AddBuff(ModContent.BuffType<GodSlayerCooldown>(), CalamityUtils.SecondsToFrames(35f));
						godSlayerDashHotKeyPressed = false;
					}

					if (dashMod == 9)
					{
						if (player.velocity.Length() < 0f)
						{
							player.velocity.Normalize();
							player.velocity *= -num9;
							return;
						}
						if (player.velocity.Length() > 0f)
						{
							player.velocity.Normalize();
							player.velocity *= num9;
							return;
						}
					}
					else
					{
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
            }
            else if (dashMod > 0 && !player.mount.Active)
            {
                if (dashMod == 1) // Counter and Evasion Scarf
                {
                    if (DoADash(evasionScarf ? 16.3f : 15f))
                    {
                        for (int d = 0; d < 20; d++)
                        {
                            int idx = Dust.NewDust(player.position, player.width, player.height, 235, 0f, 0f, 100, default, 2f);
                            Dust dust = Main.dust[idx];
                            dust.position.X += Main.rand.NextFloat(-5f, 5f);
                            dust.position.Y += Main.rand.NextFloat(-5f, 5f);
                            dust.velocity *= 0.2f;
                            dust.scale *= Main.rand.NextFloat(1f, 1.2f);
                            dust.shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                        }
                    }
                }
                else if (dashMod == 2) // Asgard's Valor
                {
                    if (DoADash(16.9f))
                    {
                        for (int d = 0; d < 20; d++)
                        {
                            int idx = Dust.NewDust(player.position, player.width, player.height, 246, 0f, 0f, 100, default, 3f);
                            Dust dust = Main.dust[idx];
                            dust.position.X += Main.rand.NextFloat(-5f, 5f);
                            dust.position.Y += Main.rand.NextFloat(-5f, 5f);
                            dust.velocity *= 0.2f;
                            dust.scale *= Main.rand.NextFloat(1f, 1.2f);
                            dust.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                            dust.noGravity = true;
                            dust.fadeIn = 0.5f;
                        }
                    }
                }
                else if (dashMod == 3) // Elysian Aegis
                {
                    if (DoADash(21.5f))
                    {
                        for (int d = 0; d < 40; d++)
                        {
                            int idx = Dust.NewDust(player.position, player.width, player.height, 244, 0f, 0f, 100, default, 3f);
                            Dust dust = Main.dust[idx];
                            dust.position.X += Main.rand.NextFloat(-5f, 5f);
                            dust.position.Y += Main.rand.NextFloat(-5f, 5f);
                            dust.velocity *= 0.2f;
                            dust.scale *= Main.rand.NextFloat(1f, 1.2f);
                            dust.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                            dust.noGravity = true;
                            dust.fadeIn = 0.5f;
                        }
                    }
                }
                else if (dashMod == 4) // Asgardian Aegis
                {
                    if (DoADash(23.3f))
                    {
                        for (int d = 0; d < 60; d++)
                        {
                            int idx = Dust.NewDust(player.position, player.width, player.height, 244, 0f, 0f, 100, default, 3f);
                            Dust dust = Main.dust[idx];
                            dust.position.X += Main.rand.NextFloat(-5f, 5f);
                            dust.position.Y += Main.rand.NextFloat(-5f, 5f);
                            dust.velocity *= 0.2f;
                            dust.scale *= Main.rand.NextFloat(1f, 1.2f);
                            dust.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                            dust.noGravity = true;
                            dust.fadeIn = 0.5f;
                        }
                    }
                }
                else if (dashMod == 5) // Deep Diver
                {
                    if (DoADash(25.9f))
                    {
                        for (int d = 0; d < 60; d++)
                        {
                            int idx = Dust.NewDust(player.position, player.width, player.height, 33, 0f, 0f, 100, default, 3f);
                            Dust dust = Main.dust[idx];
                            dust.position.X += Main.rand.NextFloat(-5f, 5f);
                            dust.position.Y += Main.rand.NextFloat(-5f, 5f);
                            dust.velocity *= 0.2f;
                            dust.scale *= Main.rand.NextFloat(1f, 1.2f);
                            dust.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                            dust.noGravity = true;
                            dust.fadeIn = 0.5f;
                        }
                    }
                }
				else if (dashMod == 6) // Ornate Shield
				{
					if (DoADash(16.9f))
					{
						for (int d = 0; d < 60; d++)
						{
							int idx = Dust.NewDust(player.position, player.width, player.height, 67, 0f, 0f, 100, default, 1.25f);
							Dust dust = Main.dust[idx];
							dust.position.X += Main.rand.NextFloat(-5f, 5f);
							dust.position.Y += Main.rand.NextFloat(-5f, 5f);
							dust.velocity *= 0.2f;
							dust.scale *= Main.rand.NextFloat(1f, 1.2f);
							dust.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
							dust.noGravity = true;
							dust.fadeIn = 0.5f;
						}
					}
				}
				else if (dashMod == 7) // Statis' Belt of Curses
                {
                    if (DoADash(25.4f))
                    {
                        for (int d = 0; d < 20; d++)
                        {
                            int idx = Dust.NewDust(player.position, player.width, player.height, 70, 0f, 0f, 100, default, 2f);
                            Dust dust = Main.dust[idx];
                            dust.position.X += Main.rand.NextFloat(-5f, 5f);
                            dust.position.Y += Main.rand.NextFloat(-5f, 5f);
                            dust.velocity *= 0.2f;
                            dust.scale *= Main.rand.NextFloat(1f, 1.2f);
                            dust.shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                        }
                    }
                }
                else if (dashMod == 8) // Plaguebringer armor
                {
                    if (DoADash(19f))
                    {
                        for (int d = 0; d < 60; d++)
                        {
                            int idx = Dust.NewDust(player.position, player.width, player.height, 89, 0f, 0f, 100, default, 1.25f);
                            Dust dust = Main.dust[idx];
                            dust.position.X += Main.rand.NextFloat(-5f, 5f);
                            dust.position.Y += Main.rand.NextFloat(-5f, 5f);
                            dust.velocity *= 0.2f;
                            dust.scale *= Main.rand.NextFloat(1f, 1.2f);
                            dust.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                            dust.noGravity = true;
                            dust.fadeIn = 0.5f;
                        }
                    }
                }
				else if (dashMod == 9) // God Slayer Armor
				{
					if (DoADash(80f))
					{
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/DevourerAttack"), (int)player.position.X, (int)player.position.Y);

						for (int d = 0; d < 60; d++)
						{
							int idx = Dust.NewDust(player.position, player.width, player.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
							Dust dust = Main.dust[idx];
							dust.position.X += Main.rand.NextFloat(-5f, 5f);
							dust.position.Y += Main.rand.NextFloat(-5f, 5f);
							dust.velocity *= 0.2f;
							dust.scale *= Main.rand.NextFloat(1f, 1.2f);
							dust.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
							dust.noGravity = true;
							dust.fadeIn = 0.5f;
						}
					}
				}
			}
        }

        private bool DoADash(float dashDistance)
        {
			bool godSlayerDash = dashMod == 9;
            bool justDashed = false;
            int direction = 0;

            if (dashTimeMod > 0)
                dashTimeMod--;
            if (dashTimeMod < 0)
                dashTimeMod++;

			if (godSlayerDash)
			{
				if (player.controlUp && player.controlLeft)
				{
					if (dashTimeMod < 0)
					{
						direction = -4;
						justDashed = true;
						dashTimeMod = 0;
					}
					else
						dashTimeMod = -15;
				}
				else if (player.controlUp && player.controlRight)
				{
					if (dashTimeMod > 0)
					{
						direction = 4;
						justDashed = true;
						dashTimeMod = 0;
					}
					else
						dashTimeMod = 15;
				}
				else if (player.controlDown && player.controlLeft)
				{
					if (dashTimeMod < 0)
					{
						direction = -3;
						justDashed = true;
						dashTimeMod = 0;
						player.maxFallSpeed = 50f;
					}
					else
						dashTimeMod = -15;
				}
				else if (player.controlDown && player.controlRight)
				{
					if (dashTimeMod > 0)
					{
						direction = 3;
						justDashed = true;
						dashTimeMod = 0;
						player.maxFallSpeed = 50f;
					}
					else
						dashTimeMod = 15;
				}
				else if (player.controlUp)
				{
					if (dashTimeMod < 0)
					{
						direction = -2;
						justDashed = true;
						dashTimeMod = 0;
					}
					else
						dashTimeMod = -15;
				}
				else if (player.controlDown)
				{
					if (dashTimeMod > 0)
					{
						direction = 2;
						justDashed = true;
						dashTimeMod = 0;
						player.maxFallSpeed = 50f;
					}
					else
						dashTimeMod = 15;
				}
				else if (player.controlLeft)
				{
					if (dashTimeMod < 0)
					{
						direction = -1;
						justDashed = true;
						dashTimeMod = 0;
					}
					else
						dashTimeMod = -15;
				}
				else if (player.controlRight)
				{
					if (dashTimeMod > 0)
					{
						direction = 1;
						justDashed = true;
						dashTimeMod = 0;
					}
					else
						dashTimeMod = 15;
				}
			}
			else
			{
				if (player.controlRight && player.releaseRight)
				{
					if (dashTimeMod > 0)
					{
						direction = 1;
						justDashed = true;
						dashTimeMod = 0;
					}
					else
						dashTimeMod = 15;
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
						dashTimeMod = -15;
				}
			}

            if (justDashed)
            {
				int totalDirections = 8;
				float radians = MathHelper.TwoPi / totalDirections;
				Vector2 spinningPoint = new Vector2(0f, -dashDistance);
				Vector2[] possibleVelocities = new Vector2[totalDirections];
				for (int k = 0; k < totalDirections; k++)
					possibleVelocities[k] = spinningPoint.RotatedBy(radians * k);

				switch (direction)
				{
					// Up Left
					case -4:
						player.velocity = possibleVelocities[7];
						break;

					// Down Left
					case -3:
						player.velocity = possibleVelocities[5];
						break;

					// Up
					case -2:
						player.velocity = possibleVelocities[0];
						break;

					// Left
					case -1:
						player.velocity = godSlayerDash ? possibleVelocities[6] : new Vector2(possibleVelocities[6].X, player.velocity.Y);
						break;

					// Nothing
					case 0:
						break;

					// Right
					case 1:
						player.velocity = godSlayerDash ? possibleVelocities[2] : new Vector2(possibleVelocities[2].X, player.velocity.Y);
						break;

					// Down
					case 2:
						player.velocity = possibleVelocities[4];
						break;

					// Down Right
					case 3:
						player.velocity = possibleVelocities[3];
						break;

					// Up Right
					case 4:
						player.velocity = possibleVelocities[1];
						break;
				}

				Point point5 = (player.Center + new Vector2(MathHelper.Clamp(direction, -1f, 1f) * player.width / 2 + 2, player.gravDir * -player.height / 2f + player.gravDir * 2f)).ToTileCoordinates();
				Point point6 = (player.Center + new Vector2(MathHelper.Clamp(direction, -1f, 1f) * player.width / 2 + 2, 0f)).ToTileCoordinates();
				if (WorldGen.SolidOrSlopedTile(point5.X, point5.Y) || WorldGen.SolidOrSlopedTile(point6.X, point6.Y))
					player.velocity.X /= 2f;

                player.dashDelay = -1;
            }

            return justDashed;
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
					else if (dashMod == 9)
					{
						int num7 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num3), player.width + 8, 4, (int)CalamityDusts.PurpleCosmilite, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 3f);
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
					else if (dashMod == 9)
					{
						int num12 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num8), player.width + 8, 4, (int)CalamityDusts.PurpleCosmilite, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default, 3f);
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
                int NPCImmuneTime = 30;
                int playerImmuneTime = 6;
                ModCollideWithNPCs(rect, damage, knockback, NPCImmuneTime, playerImmuneTime);
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
                int NPCImmuneTime = 30;
                int playerImmuneTime = 6;
                ModCollideWithNPCs(rect2, damage2, knockback2, NPCImmuneTime, playerImmuneTime);
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
                int NPCImmuneTime = 30;
                int playerImmuneTime = 6;
                ModCollideWithNPCs(rect2, damage2, knockback2, NPCImmuneTime, playerImmuneTime);
            }
        }

        private int ModCollideWithNPCs(Rectangle myRect, float Damage, float Knockback, int NPCImmuneTime, int PlayerImmuneTime)
        {
            int num = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC nPC = Main.npc[i];
                if (nPC.active && !nPC.dontTakeDamage && !nPC.friendly && nPC.Calamity().dashImmunityTime[player.whoAmI] <= 0)
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
						nPC.Calamity().dashImmunityTime[player.whoAmI] = NPCImmuneTime;
                        player.GiveIFrames(PlayerImmuneTime, false);
                        num++;
                        break;
                    }
                }
            }
            return num;
        }
        #endregion

        #region Nurse Modifications
        public override bool ModifyNurseHeal(NPC nurse, ref int health, ref bool removeDebuffs, ref string chatText)
        {
            if ((CalamityWorld.death || BossRushEvent.BossRushActive) && areThereAnyDamnBosses)
            {
                chatText = "Now is not the time!";
                return false;
            }

            return true;
        }

        public override void ModifyNursePrice(NPC nurse, int health, bool removeDebuffs, ref int price)
        {
            // Nurse costs scale as the game progresses.
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

            if (price > 0)
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
            stealthDamage = 0f;
            rogueStealthMax = 0f;
            stealthGenStandstill = 1f;
            stealthGenMoving = 1f;
            stealthStrikeThisFrame = false;
            stealthStrikeHalfCost = false;
            stealthStrike75Cost = false;
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
            // 1f is normal speed, anything higher is faster. Default stealth generation is 3 seconds while standing still.
            float currentStealthGen = UpdateStealthGenStats();
            rogueStealth += rogueStealthMax * (currentStealthGen / 180f); // 180 frames = 3 seconds
            if (rogueStealth > rogueStealthMax)
                rogueStealth = rogueStealthMax;

            ProvideStealthStatBonuses();

            // If the player is using an item that deals damage and is on their first frame of a use of that item,
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

            // The Gem Tech armor's rogue crystal ensures that stealth is not consumed by non-rogue items.
            if ((it.IsAir || !it.Calamity().rogue) && GemTechSet && GemTechState.IsRedGemActive)
                playerUsingWeapon = false;

            // Animation check depends on whether the item is "clockwork", like Clockwork Assault Rifle.
            // "Clockwork" weapons can chain-fire multiple stealth strikes (really only 2 max) until you run out of stealth.
            bool animationCheck = it.useAnimation == it.useTime
                ? player.itemAnimation == player.itemAnimationMax - 1 // Standard weapon (first frame of use animation)
                : player.itemTime == it.useTime; // Clockwork weapon (first frame of any individual use event)

            if (!stealthStrikeThisFrame && animationCheck && playerUsingWeapon)
            {
                bool canStealthStrike = StealthStrikeAvailable();

                // If you can stealth strike, you do.
                if (canStealthStrike)
                    ConsumeStealthByAttacking();
                // Otherwise you get a "partial stealth strike" (stealth damage is still added to the weapon) and return to normally attacking.
                else
                    rogueStealth = 0f;
            }
        }

        private void ProvideStealthStatBonuses()
        {
            if (!wearingRogueArmor || rogueStealthMax <= 0)
                return;

            // Hovering over an item will adjust the stealth bonus dynamically so that you see the correct damage for an item you put your cursor on.
            Item it = !Main.HoverItem.IsAir ? Main.HoverItem : player.ActiveItem();

            // The potential damage bonus from stealth is a complex equation based on the item's use time,
            // the player's averaged-together stealth generation stats, and max stealth.
            // Lower stealth generation rate (especially while moving) enables higher maximum stealth damage.
            // This enables stealth to be conditionally useful -- even powerful -- even without a dedicated stealth build.
            double averagedStealthGen = 0.8 * stealthGenMoving + 0.2 * stealthGenStandstill;
            double fakeStealthTime = 9D / averagedStealthGen;

            // Use time  3 = 162% damage ratio
            // Use time  8 = 200% damage ratio
            // Use time 13 = 221% damage ratio
            // Use time 17 = 234% damage ratio
            // Use time 20 = 242% damage ratio
            // Use time 30 = 263% damage ratio
            // Use time 59 = 297% damage ratio
            int realUseTime = Math.Max(it.useTime, it.useAnimation);
            double useTimeFactor = 0.75 + 0.75 * Math.Log(realUseTime + 2D, 4D);

            // 9.00 second stealth charge = 433% damage ratio
            // 6.00 second stealth charge = 330% damage ratio
            // 4.00 second stealth charge = 252% damage ratio
            // 2.50 second stealth charge = 184% damage ratio
            double stealthGenFactor = Math.Max(Math.Pow(fakeStealthTime, 2D / 3D), 1.5);

            double stealthAddedDamage = rogueStealth * StealthDamageConstant * useTimeFactor * stealthGenFactor;
            stealthDamage += (float)stealthAddedDamage;

            // Show 100% crit chance if your stealth strikes always crit.
            // In practice, this is only for visuals because Terraria determines crit status on hit.
            if (stealthStrikeAlwaysCrits && StealthStrikeAvailable())
                throwingCrit = 100;

            // Stealth slightly decreases aggro.
            player.aggro -= (int)(rogueStealth * 300f);
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

            if (CalamityLists.daggerList.Contains(player.ActiveItem().type) && player.invis)
            {
                stealthGenStandstill += 0.08f;
                stealthGenMoving += 0.08f;
            }
            if (shadow)
            {
                stealthGenStandstill += 0.1f;
                stealthGenMoving += 0.1f;
            }

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
            float consumptionMult = 1f;
            if (stealthStrikeHalfCost)
                consumptionMult = 0.5f;
            else if (stealthStrike75Cost)
                consumptionMult = 0.75f;
            return rogueStealth >= rogueStealthMax * consumptionMult;
        }

        internal void ConsumeStealthByAttacking()
        {
            stealthStrikeThisFrame = true;
            stealthAcceleration = 1f; // Reset acceleration when you attack

            float lossReductionRatio = flatStealthLossReduction / (rogueStealthMax * 100f);
			float remainingStealth = rogueStealthMax * lossReductionRatio;
			float stealthToLose = rogueStealthMax - remainingStealth;
			// You cannot lose less than one stealth point.
			if (stealthToLose < 0.01f)
				stealthToLose = 0.01f;
            if (stealthStrikeHalfCost)
            {
                rogueStealth -= 0.5f * stealthToLose;
                if (rogueStealth <= 0f)
                    rogueStealth = 0f;
            }
            else if (stealthStrike75Cost)
			{
                rogueStealth -= 0.75f * stealthToLose;
                if (rogueStealth <= 0f)
                    rogueStealth = 0f;
            }
			else
                rogueStealth = remainingStealth;
        }
        #endregion

        #region Proficiency Stuff
        private bool ReduceCooldown(int classType)
        {
            switch (classType)
            {
                case (int)ClassType.Melee:

                    if (meleeLevel < levelTier3 && (rangedLevel >= levelTier3 || magicLevel >= levelTier3 || summonLevel >= levelTier3 || rogueLevel >= levelTier3))
                        return true;
                    if (meleeLevel < levelTier2 && (rangedLevel >= levelTier2 || magicLevel >= levelTier2 || summonLevel >= levelTier2 || rogueLevel >= levelTier2))
                        return true;
                    if (meleeLevel < levelTier1 && (rangedLevel >= levelTier1 || magicLevel >= levelTier1 || summonLevel >= levelTier1 || rogueLevel >= levelTier1))
                        return true;

                    break;

                case (int)ClassType.Ranged:

                    if (rangedLevel < levelTier3 && (meleeLevel >= levelTier3 || magicLevel >= levelTier3 || summonLevel >= levelTier3 || rogueLevel >= levelTier3))
                        return true;
                    if (rangedLevel < levelTier2 && (meleeLevel >= levelTier2 || magicLevel >= levelTier2 || summonLevel >= levelTier2 || rogueLevel >= levelTier2))
                        return true;
                    if (rangedLevel < levelTier1 && (meleeLevel >= levelTier1 || magicLevel >= levelTier1 || summonLevel >= levelTier1 || rogueLevel >= levelTier1))
                        return true;

                    break;

                case (int)ClassType.Magic:

                    if (magicLevel < levelTier3 && (rangedLevel >= levelTier3 || meleeLevel >= levelTier3 || summonLevel >= levelTier3 || rogueLevel >= levelTier3))
                        return true;
                    if (magicLevel < levelTier2 && (rangedLevel >= levelTier2 || meleeLevel >= levelTier2 || summonLevel >= levelTier2 || rogueLevel >= levelTier2))
                        return true;
                    if (magicLevel < levelTier1 && (rangedLevel >= levelTier1 || meleeLevel >= levelTier1 || summonLevel >= levelTier1 || rogueLevel >= levelTier1))
                        return true;

                    break;

                case (int)ClassType.Summon:

                    if (summonLevel < levelTier3 && (rangedLevel >= levelTier3 || magicLevel >= levelTier3 || meleeLevel >= levelTier3 || rogueLevel >= levelTier3))
                        return true;
                    if (summonLevel < levelTier2 && (rangedLevel >= levelTier2 || magicLevel >= levelTier2 || meleeLevel >= levelTier2 || rogueLevel >= levelTier2))
                        return true;
                    if (summonLevel < levelTier1 && (rangedLevel >= levelTier1 || magicLevel >= levelTier1 || meleeLevel >= levelTier1 || rogueLevel >= levelTier1))
                        return true;

                    break;

                case (int)ClassType.Rogue:

                    if (rogueLevel < levelTier3 && (rangedLevel >= levelTier3 || magicLevel >= levelTier3 || summonLevel >= levelTier3 || meleeLevel >= levelTier3))
                        return true;
                    if (rogueLevel < levelTier2 && (rangedLevel >= levelTier2 || magicLevel >= levelTier2 || summonLevel >= levelTier2 || meleeLevel >= levelTier2))
                        return true;
                    if (rogueLevel < levelTier1 && (rangedLevel >= levelTier1 || magicLevel >= levelTier1 || summonLevel >= levelTier1 || meleeLevel >= levelTier1))
                        return true;

                    break;
            }
            return false;
        }

        public void GetExactLevelUp()
        {
            if (gainLevelCooldown > 0)
                gainLevelCooldown--;

            #region MeleeLevels
            switch (meleeLevel)
            {
                case 100:
                    ExactLevelUp(0, 1, false);
                    break;
                case 300:
                    ExactLevelUp(0, 2, false);
                    break;
                case 600:
                    ExactLevelUp(0, 3, false);
                    break;
                case 1000:
                    ExactLevelUp(0, 4, false);
                    break;
                case 1500:
                    ExactLevelUp(0, 5, false);
                    break;
                case 2100:
                    ExactLevelUp(0, 6, false);
                    break;
                case 2800:
                    ExactLevelUp(0, 7, false);
                    break;
                case 3600:
                    ExactLevelUp(0, 8, false);
                    break;
                case 4500:
                    ExactLevelUp(0, 9, false);
                    break;
                case 5500:
                    ExactLevelUp(0, 10, false);
                    break;
                case 6600:
                    ExactLevelUp(0, 11, false);
                    break;
                case 7800:
                    ExactLevelUp(0, 12, false);
                    break;
                case 9100:
                    ExactLevelUp(0, 13, false);
                    break;
                case 10500:
                    ExactLevelUp(0, 14, false);
                    break;
                case 12500: //celebration or some shit for final level, yay
                    ExactLevelUp(0, 15, true);
                    break;
                default:
                    break;
            }
            #endregion

            #region RangedLevels
            switch (rangedLevel)
            {
                case 100:
                    ExactLevelUp(1, 1, false);
                    break;
                case 300:
                    ExactLevelUp(1, 2, false);
                    break;
                case 600:
                    ExactLevelUp(1, 3, false);
                    break;
                case 1000:
                    ExactLevelUp(1, 4, false);
                    break;
                case 1500:
                    ExactLevelUp(1, 5, false);
                    break;
                case 2100:
                    ExactLevelUp(1, 6, false);
                    break;
                case 2800:
                    ExactLevelUp(1, 7, false);
                    break;
                case 3600:
                    ExactLevelUp(1, 8, false);
                    break;
                case 4500:
                    ExactLevelUp(1, 9, false);
                    break;
                case 5500:
                    ExactLevelUp(1, 10, false);
                    break;
                case 6600:
                    ExactLevelUp(1, 11, false);
                    break;
                case 7800:
                    ExactLevelUp(1, 12, false);
                    break;
                case 9100:
                    ExactLevelUp(1, 13, false);
                    break;
                case 10500:
                    ExactLevelUp(1, 14, false);
                    break;
                case 12500: //celebration or some shit for final level, yay
                    ExactLevelUp(1, 15, true);
                    break;
                default:
                    break;
            }
            #endregion

            #region MagicLevels
            switch (magicLevel)
            {
                case 100:
                    ExactLevelUp(2, 1, false);
                    break;
                case 300:
                    ExactLevelUp(2, 2, false);
                    break;
                case 600:
                    ExactLevelUp(2, 3, false);
                    break;
                case 1000:
                    ExactLevelUp(2, 4, false);
                    break;
                case 1500:
                    ExactLevelUp(2, 5, false);
                    break;
                case 2100:
                    ExactLevelUp(2, 6, false);
                    break;
                case 2800:
                    ExactLevelUp(2, 7, false);
                    break;
                case 3600:
                    ExactLevelUp(2, 8, false);
                    break;
                case 4500:
                    ExactLevelUp(2, 9, false);
                    break;
                case 5500:
                    ExactLevelUp(2, 10, false);
                    break;
                case 6600:
                    ExactLevelUp(2, 11, false);
                    break;
                case 7800:
                    ExactLevelUp(2, 12, false);
                    break;
                case 9100:
                    ExactLevelUp(2, 13, false);
                    break;
                case 10500:
                    ExactLevelUp(2, 14, false);
                    break;
                case 12500: //celebration or some shit for final level, yay
                    ExactLevelUp(2, 15, true);
                    break;
                default:
                    break;
            }
            #endregion

            #region SummonLevels
            switch (summonLevel)
            {
                case 100:
                    ExactLevelUp(3, 1, false);
                    break;
                case 300:
                    ExactLevelUp(3, 2, false);
                    break;
                case 600:
                    ExactLevelUp(3, 3, false);
                    break;
                case 1000:
                    ExactLevelUp(3, 4, false);
                    break;
                case 1500:
                    ExactLevelUp(3, 5, false);
                    break;
                case 2100:
                    ExactLevelUp(3, 6, false);
                    break;
                case 2800:
                    ExactLevelUp(3, 7, false);
                    break;
                case 3600:
                    ExactLevelUp(3, 8, false);
                    break;
                case 4500:
                    ExactLevelUp(3, 9, false);
                    break;
                case 5500:
                    ExactLevelUp(3, 10, false);
                    break;
                case 6600:
                    ExactLevelUp(3, 11, false);
                    break;
                case 7800:
                    ExactLevelUp(3, 12, false);
                    break;
                case 9100:
                    ExactLevelUp(3, 13, false);
                    break;
                case 10500:
                    ExactLevelUp(3, 14, false);
                    break;
                case 12500: //celebration or some shit for final level, yay
                    ExactLevelUp(3, 15, true);
                    break;
                default:
                    break;
            }
            #endregion

            #region RogueLevels
            switch (rogueLevel)
            {
                case 100:
                    ExactLevelUp(4, 1, false);
                    break;
                case 300:
                    ExactLevelUp(4, 2, false);
                    break;
                case 600:
                    ExactLevelUp(4, 3, false);
                    break;
                case 1000:
                    ExactLevelUp(4, 4, false);
                    break;
                case 1500:
                    ExactLevelUp(4, 5, false);
                    break;
                case 2100:
                    ExactLevelUp(4, 6, false);
                    break;
                case 2800:
                    ExactLevelUp(4, 7, false);
                    break;
                case 3600:
                    ExactLevelUp(4, 8, false);
                    break;
                case 4500:
                    ExactLevelUp(4, 9, false);
                    break;
                case 5500:
                    ExactLevelUp(4, 10, false);
                    break;
                case 6600:
                    ExactLevelUp(4, 11, false);
                    break;
                case 7800:
                    ExactLevelUp(4, 12, false);
                    break;
                case 9100:
                    ExactLevelUp(4, 13, false);
                    break;
                case 10500:
                    ExactLevelUp(4, 14, false);
                    break;
                case 12500: //celebration or some shit for final level, yay
                    ExactLevelUp(4, 15, true);
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
                SyncExactLevel(false, levelUpType);
            }
        }

        public void GetStatBonuses()
        {
            #region MeleeLevelBoosts
            if (meleeLevel >= 12500)
            {
                player.meleeDamage += 0.06f;
                player.meleeCrit += 3;
            }
            else if (meleeLevel >= 10500)
            {
                player.meleeDamage += 0.06f;
                player.meleeCrit += 3;
            }
            else if (meleeLevel >= 9100)
            {
                player.meleeDamage += 0.06f;
                player.meleeCrit += 3;
            }
            else if (meleeLevel >= 7800)
            {
                player.meleeDamage += 0.05f;
                player.meleeCrit += 3;
            }
            else if (meleeLevel >= 6600)
            {
                player.meleeDamage += 0.05f;
                player.meleeCrit += 3;
            }
            else if (meleeLevel >= 5500) //hm limit
            {
                player.meleeDamage += 0.05f;
                player.meleeCrit += 2;
            }
            else if (meleeLevel >= 4500)
            {
                player.meleeDamage += 0.05f;
                player.meleeCrit += 2;
            }
            else if (meleeLevel >= 3600)
            {
                player.meleeDamage += 0.04f;
                player.meleeCrit += 2;
            }
            else if (meleeLevel >= 2800)
            {
                player.meleeDamage += 0.04f;
                player.meleeCrit += 1;
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
                player.meleeDamage += 0.02f;
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
                player.rangedDamage += 0.06f;
                player.moveSpeed += 0.06f;
                player.rangedCrit += 3;
            }
            else if (rangedLevel >= 10500)
            {
                player.rangedDamage += 0.05f;
                player.moveSpeed += 0.06f;
                player.rangedCrit += 3;
            }
            else if (rangedLevel >= 9100)
            {
                player.rangedDamage += 0.05f;
                player.moveSpeed += 0.05f;
                player.rangedCrit += 3;
            }
            else if (rangedLevel >= 7800)
            {
                player.rangedDamage += 0.04f;
                player.moveSpeed += 0.05f;
                player.rangedCrit += 3;
            }
            else if (rangedLevel >= 6600)
            {
                player.rangedDamage += 0.04f;
                player.moveSpeed += 0.04f;
                player.rangedCrit += 3;
            }
            else if (rangedLevel >= 5500)
            {
                player.rangedDamage += 0.04f;
                player.moveSpeed += 0.04f;
                player.rangedCrit += 2;
            }
            else if (rangedLevel >= 4500)
            {
                player.rangedDamage += 0.03f;
                player.moveSpeed += 0.04f;
                player.rangedCrit += 2;
            }
            else if (rangedLevel >= 3600)
            {
                player.rangedDamage += 0.03f;
                player.moveSpeed += 0.03f;
                player.rangedCrit += 2;
            }
            else if (rangedLevel >= 2800)
            {
                player.rangedDamage += 0.02f;
                player.moveSpeed += 0.03f;
                player.rangedCrit += 2;
            }
            else if (rangedLevel >= 2100)
            {
                player.rangedDamage += 0.02f;
                player.moveSpeed += 0.03f;
                player.rangedCrit += 1;
            }
            else if (rangedLevel >= 1500)
            {
                player.rangedDamage += 0.02f;
                player.moveSpeed += 0.02f;
                player.rangedCrit += 1;
            }
            else if (rangedLevel >= 1000)
            {
                player.rangedDamage += 0.02f;
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
                player.magicDamage += 0.06f;
                player.manaCost *= 0.94f;
                player.magicCrit += 3;
            }
            else if (magicLevel >= 10500)
            {
                player.magicDamage += 0.05f;
                player.manaCost *= 0.94f;
                player.magicCrit += 3;
            }
            else if (magicLevel >= 9100)
            {
                player.magicDamage += 0.05f;
                player.manaCost *= 0.95f;
                player.magicCrit += 3;
            }
            else if (magicLevel >= 7800)
            {
                player.magicDamage += 0.04f;
                player.manaCost *= 0.95f;
                player.magicCrit += 3;
            }
            else if (magicLevel >= 6600)
            {
                player.magicDamage += 0.04f;
                player.manaCost *= 0.96f;
                player.magicCrit += 3;
            }
            else if (magicLevel >= 5500)
            {
                player.magicDamage += 0.04f;
                player.manaCost *= 0.96f;
                player.magicCrit += 2;
            }
            else if (magicLevel >= 4500)
            {
                player.magicDamage += 0.04f;
                player.manaCost *= 0.97f;
                player.magicCrit += 2;
            }
            else if (magicLevel >= 3600)
            {
                player.magicDamage += 0.03f;
                player.manaCost *= 0.97f;
                player.magicCrit += 2;
            }
            else if (magicLevel >= 2800)
            {
                player.magicDamage += 0.03f;
                player.manaCost *= 0.98f;
                player.magicCrit += 2;
            }
            else if (magicLevel >= 2100)
            {
                player.magicDamage += 0.03f;
                player.manaCost *= 0.98f;
                player.magicCrit += 1;
            }
            else if (magicLevel >= 1500)
            {
                player.magicDamage += 0.02f;
                player.manaCost *= 0.98f;
                player.magicCrit += 1;
            }
            else if (magicLevel >= 1000)
            {
                player.magicDamage += 0.02f;
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
            }
            else if (summonLevel >= 10500)
            {
                player.minionDamage += 0.1f;
                player.minionKB += 3.0f;
            }
            else if (summonLevel >= 9100)
            {
                player.minionDamage += 0.09f;
                player.minionKB += 2.7f;
            }
            else if (summonLevel >= 7800)
            {
                player.minionDamage += 0.08f;
                player.minionKB += 2.4f;
            }
            else if (summonLevel >= 6600)
            {
                player.minionDamage += 0.07f;
                player.minionKB += 2.1f;
            }
            else if (summonLevel >= 5500)
            {
                player.minionDamage += 0.07f;
                player.minionKB += 1.8f;
            }
            else if (summonLevel >= 4500)
            {
                player.minionDamage += 0.06f;
                player.minionKB += 1.8f;
            }
            else if (summonLevel >= 3600)
            {
                player.minionDamage += 0.05f;
                player.minionKB += 1.5f;
            }
            else if (summonLevel >= 2800)
            {
                player.minionDamage += 0.04f;
                player.minionKB += 1.2f;
            }
            else if (summonLevel >= 2100)
            {
                player.minionDamage += 0.04f;
                player.minionKB += 0.9f;
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
                throwingDamage += 0.06f;
                throwingVelocity += 0.06f;
                throwingCrit += 3;
            }
            else if (rogueLevel >= 10500)
            {
                throwingDamage += 0.05f;
                throwingVelocity += 0.06f;
                throwingCrit += 3;
            }
            else if (rogueLevel >= 9100)
            {
                throwingDamage += 0.05f;
                throwingVelocity += 0.05f;
                throwingCrit += 3;
            }
            else if (rogueLevel >= 7800)
            {
                throwingDamage += 0.04f;
                throwingVelocity += 0.05f;
                throwingCrit += 3;
            }
            else if (rogueLevel >= 6600)
            {
                throwingDamage += 0.04f;
                throwingVelocity += 0.04f;
                throwingCrit += 3;
            }
            else if (rogueLevel >= 5500)
            {
                throwingDamage += 0.04f;
                throwingVelocity += 0.04f;
                throwingCrit += 2;
            }
            else if (rogueLevel >= 4500)
            {
                throwingDamage += 0.03f;
                throwingVelocity += 0.04f;
                throwingCrit += 2;
            }
            else if (rogueLevel >= 3600)
            {
                throwingDamage += 0.03f;
                throwingVelocity += 0.03f;
                throwingCrit += 2;
            }
            else if (rogueLevel >= 2800)
            {
                throwingDamage += 0.03f;
                throwingVelocity += 0.03f;
                throwingCrit += 1;
            }
            else if (rogueLevel >= 2100)
            {
                throwingDamage += 0.02f;
                throwingVelocity += 0.03f;
                throwingCrit += 1;
            }
            else if (rogueLevel >= 1500)
            {
                throwingDamage += 0.02f;
                throwingVelocity += 0.02f;
                throwingCrit += 1;
            }
            else if (rogueLevel >= 1000)
            {
                throwingDamage += 0.02f;
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
                meleeSpeedBonus += 0.06f;
            }
            else if (meleeLevel >= 10500)
            {
                meleeSpeedBonus += 0.05f;
            }
            else if (meleeLevel >= 9100)
            {
                meleeSpeedBonus += 0.04f;
            }
            else if (meleeLevel >= 7800)
            {
                meleeSpeedBonus += 0.04f;
            }
            else if (meleeLevel >= 6600)
            {
                meleeSpeedBonus += 0.03f;
            }
            else if (meleeLevel >= 5500) //hm limit
            {
                meleeSpeedBonus += 0.03f;
            }
            else if (meleeLevel >= 4500)
            {
                meleeSpeedBonus += 0.02f;
            }
            else if (meleeLevel >= 3600)
            {
                meleeSpeedBonus += 0.02f;
            }
            else if (meleeLevel >= 2800)
            {
                meleeSpeedBonus += 0.02f;
            }
            else if (meleeLevel >= 2100)
            {
                meleeSpeedBonus += 0.01f;
            }
            else if (meleeLevel >= 1500) //prehm limit
            {
                meleeSpeedBonus += 0.01f;
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

        // Triggers effects that must occur when the player enters the world. This sends a bunch of packets in multiplayer.
        // It also starts the speedrun timer if applicable.
        public override void OnEnterWorld(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                EnterWorldSync();

            // Enabling the config while a player is loaded will show the timer immediately.
            // But it won't start running until you save and quit and re-enter a world.
            if (CalamityConfig.Instance.SpeedrunTimer)
                CalamityMod.SpeedrunTimer.Restart();
        }

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
            range *= reaverExplore ? 0.9f : 1f;
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
            if (aAmpoule)
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
            if (littleLightPet)
                light += 3;
            if (profanedCrystalBuffs && !ZoneAbyss)
                light += Main.dayTime || player.lavaWet ? 2 : 1;
            return light;
        }

        #endregion

        #region Mana Consumption Effects
        public override void OnConsumeMana(Item item, int manaConsumed)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (Main.rand.NextBool(2) && modPlayer.lifeManaEnchant)
            {
                if (Main.myPlayer == player.whoAmI)
                {
                    player.HealEffect(-5);
                    player.statLife -= 5;
                    if (player.statLife <= 0)
                        player.KillMe(PlayerDeathReason.ByCustomReason($"{player.name} converted all of their life to mana."), 1000, -1);
                }

                for (int i = 0; i < 8; i++)
                {
                    Dust life = Dust.NewDustPerfect(player.Top + Main.rand.NextVector2Circular(player.width * 0.5f, 6f), 267);
                    life.color = Color.Red;
                    life.velocity = -Vector2.UnitY.RotatedByRandom(0.48f) * Main.rand.NextFloat(3f, 4.4f);
                    life.scale = Main.rand.NextFloat(1.5f, 1.72f);
                    life.fadeIn = 0.7f;
                    life.noGravity = true;
                }
            }
        }
        #endregion

        #region Defense Damage Function
        private void DealDefenseDamage(int damage)
        {
            double ratioToUse = DefenseDamageRatio;
            if (draedonsHeart)
                ratioToUse *= 0.5;

            // Calculate the defense damage taken from this hit.
            int defenseDamageTaken = (int)(damage * ratioToUse);

            // There is a floor on defense damage based on difficulty; i.e. there is a minimum amount of defense damage from any hit that can deal defense damage.
            // This floor is only applied if bosses are alive
            if (areThereAnyDamnBosses)
            {
                int defenseDamageFloor = ((CalamityWorld.malice || BossRushEvent.BossRushActive) ? 5 : CalamityWorld.death ? 4 : CalamityWorld.revenge ? 3 : Main.expertMode ? 2 : 1) * (NPC.downedMoonlord ? 3 : Main.hardMode ? 2 : 1);
                if (defenseDamageTaken < defenseDamageFloor)
                    defenseDamageTaken = defenseDamageFloor;
            }

            // There is also a cap on defense damage: 25% of the player's original defense.
            int cap = player.statDefense / 4;
            if (defenseDamageTaken > cap)
                defenseDamageTaken = cap;

            // Apply defense damage to the adamantite armor set boost.
            AdamantiteSetDefenseBoost -= defenseDamageTaken;

            // Apply that defense damage on top of whatever defense damage the player currently has.
            int previousDefenseDamage = CurrentDefenseDamage;
            totalDefenseDamage = previousDefenseDamage + defenseDamageTaken;

            // Safety check to prevent illegal recovery time
            if (defenseDamageRecoveryFrames < 0)
                defenseDamageRecoveryFrames = 0;

            // DIRECTLY ADD the base defense damage recovery time to whatever recovery time the player already has.
            totalDefenseDamageRecoveryFrames = defenseDamageRecoveryFrames + DefenseDamageBaseRecoveryTime;
            if (totalDefenseDamageRecoveryFrames > DefenseDamageMaxRecoveryTime)
                totalDefenseDamageRecoveryFrames = DefenseDamageMaxRecoveryTime;
            // Reset any recovery progress they may have already made.
            // They start the new recovery timer from the beginning.
            defenseDamageRecoveryFrames = totalDefenseDamageRecoveryFrames;

            // Reset the delay between iframes and being able to recover from defense damage.
            defenseDamageDelayFrames = DefenseDamageRecoveryDelay;

            // Play a sound from taking defense damage.
            if (hurtSoundTimer == 0)
            {
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/DefenseDamage"), (int)player.position.X, (int)player.position.Y);
                hurtSoundTimer = 30;
            }

            // Display text indicating that defense damage was taken.
            string text = (-defenseDamageTaken).ToString();
            Color messageColor = Color.LightGray;
            Rectangle location = new Rectangle((int)player.position.X, (int)player.position.Y - 16, player.width, player.height);
            CombatText.NewText(location, messageColor, Language.GetTextValue(text));
        }
        #endregion
    }
}
