using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Balancing;
using CalamityMod.BiomeManagers;
using CalamityMod.Buffs;
using CalamityMod.Buffs.Cooldowns;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Potions;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Cooldowns;
using CalamityMod.DataStructures;
using CalamityMod.Dusts;
using CalamityMod.EntitySources;
using CalamityMod.Events;
using CalamityMod.FluidSimulation;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Armor;
using CalamityMod.Items.Armor.Bloodflare;
using CalamityMod.Items.Armor.Brimflame;
using CalamityMod.Items.Armor.Demonshade;
using CalamityMod.Items.Armor.GemTech;
using CalamityMod.Items.Armor.OmegaBlue;
using CalamityMod.Items.Armor.PlagueReaper;
using CalamityMod.Items.Armor.Silva;
using CalamityMod.Items.Armor.Wulfrum;
using CalamityMod.Items.Dyes;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Mounts.Minecarts;
using CalamityMod.Items.TreasureBags.MiscGrabBags;
using CalamityMod.Items.VanillaArmorChanges;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Other;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.UI;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameInput;
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
        public float stealthUIAlpha = 1f;
        public float SulphWaterUIOpacity = 1f;
        public bool shouldDrawSmallText = true;
        public int projTypeJustHitBy;
        public int sCalDeathCount = 0;
        public int sCalKillCount = 0;
        public int deathCount = 0;
        public int actualMaxLife = 0;
        public static int chaosStateDuration = 900;
        public static int chaosStateDuration_NR = 1200;
        public bool killSpikyBalls = false;
        public double acidRoundMultiplier = 1D;
        public float KameiTrailXScale = 0.1f;
        public int KameiBladeUseDelay = 0;
        public Vector2[] OldPositions = new Vector2[4];
        public double contactDamageReduction = 0D;
        public double projectileDamageReduction = 0D;
        public const float projectileMeleeWeaponMeleeSpeedMultiplier = 0f;
        public bool brimlashBusterBoost = false;
        public int evilSmasherBoost = 0;
        public int hellbornBoost = 0;
        public int searedPanCounter = 0;
        public int searedPanTimer = 0;
        public int potionTimer = 0;
        public bool cirrusDress = false;
        public bool blockAllDashes = false;
        public bool resetHeightandWidth = false;
        public bool noLifeRegen = false;
        public float rangedAmmoCost = 1f;
        public float healingPotBonus = 1f;
        public bool heldGaelsLastFrame = false;
        internal bool hadNanomachinesLastFrame = false;
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
		public Vector2? BossRushReturnPosition = null;

        public float moveSpeedBonus = 0f;
        public int momentumCapacitorTime = 0;
        public float momentumCapacitorBoost = 0f;
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
        // NOTE -- With the Armageddon item removed from Calamity, this bool can only be set by other mods
        public bool disableAllDodges = false;
        #endregion

        #region Town NPC Shop Variables
        public bool newMerchantInventory = false;
        public bool newPainterInventory = false;
        public bool newGolferInventory = false;
        public bool newZoologistInventory = false;
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
        public bool newPrincessInventory = false;
        public bool newSkeletonMerchantInventory = false;
        public bool newPermafrostInventory = false;
        public bool newCirrusInventory = false;
        public bool newAmidiasInventory = false;
        public bool newBanditInventory = false;
        public bool newCalamitasInventory = false;
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
        public int packetTimer = 0;
        public int navyRodAuraTimer = 0;
        public int brimLoreInfernoTimer = 0;
        public int tarraLifeAuraTimer = 0;
        public int bloodflareHeartTimer = 300;
        public int polarisBoostCounter = 0;
        public int dragonRageHits = 0;
        public int dragonRageCooldown = 0;
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
        public int aBulwarkRareTimer = 0;
        public int spiritOriginBullseyeShootCountdown = 0;
        public int spiritOriginConvertedCrit = 0;
        public int RustyMedallionCooldown = 0;
        public float SulphWaterPoisoningLevel;
        public NPC unstableSelectedTarget;
        public int zapActivity = 0;

        private const int DashDisableCooldown = 12;

        public Dictionary<string, CooldownInstance> cooldowns;

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

        public static readonly SoundStyle RageFilledSound = new("CalamityMod/Sounds/Custom/AbilitySounds/FullRage");
        public static readonly SoundStyle RageActivationSound = new("CalamityMod/Sounds/Custom/AbilitySounds/RageActivate");
        public static readonly SoundStyle RageEndSound = new("CalamityMod/Sounds/Custom/AbilitySounds/RageEnd");

        public static readonly SoundStyle AdrenalineFilledSound = new("CalamityMod/Sounds/Custom/AbilitySounds/FullAdrenaline");
        public static readonly SoundStyle AdrenalineActivationSound = new("CalamityMod/Sounds/Custom/AbilitySounds/AdrenalineActivate");
        public static readonly SoundStyle AdrenalineHurtSound = new("CalamityMod/Sounds/Custom/AdrenalineMajorLoss");
        public static readonly SoundStyle NanomachinesActivationSound = new("CalamityMod/Sounds/Custom/AbilitySounds/NanomachinesActivate");

        public static readonly SoundStyle RogueStealthSound = new("CalamityMod/Sounds/Custom/RogueStealth");
        public static readonly SoundStyle DefenseDamageSound = new("CalamityMod/Sounds/Custom/DefenseDamage");
        public static readonly SoundStyle BloodCritSound = new("CalamityMod/Sounds/Custom/BloodPactCrit");

        public static readonly SoundStyle IjiDeathSound = new("CalamityMod/Sounds/Custom/IjiDies");
        public static readonly SoundStyle DrownSound = new("CalamityMod/Sounds/Custom/AbyssDrown");
        #endregion

        #region Rogue
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
        public bool stealthStrike85Cost = false;
        public bool wearingRogueArmor = false;
        public float accStealthGenBoost = 0f;

        // TODO -- Stealth needs to be its own damage class so that stealth bonuses only apply to stealth strikes
        public float stealthDamage = 0f; // This is extra Rogue Damage that is only added for stealth strikes.
        public double bonusStealthDamage = 0;
        public float rogueVelocity = 1f;
        public float rogueAmmoCost = 1f;
        #endregion

        #region Mount
        public bool onyxExcavator = false;
        public bool rimehound = false;
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
        public bool thirdSageH = false;
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
        public bool RageEnabled => CalamityWorld.revenge || shatteredCommunity;
        public bool rageModeActive = false;
        public float rage = 0f;
        public float rageMax = 100f; // 0 to 100% by default
        public int RageDuration = BalancingConstants.DefaultRageDuration;
        public int rageGainCooldown = 0;
        public int rageCombatFrames = 0;
        public float RageDamageBoost = BalancingConstants.DefaultRageDamageBoost;
        #endregion

        #region Adrenaline
        public bool AdrenalineEnabled => CalamityWorld.revenge || draedonsHeart;
        public bool adrenalineModeActive = false;
        public float adrenaline = 0f;
        public float adrenalineMax = 100f; // 0 to 100% by default
        public int AdrenalineDuration = CalamityUtils.SecondsToFrames(5);
        public int AdrenalineChargeTime = CalamityUtils.SecondsToFrames(30);
        public int AdrenalineFadeTime = CalamityUtils.SecondsToFrames(2);
        #endregion

        #region Defense Damage
        // Ratio at which incoming damage (after mitigation) is converted into defense damage.
        // Used to be 5% normal, 10% expert, 12% rev, 15% death, 20% boss rush
        // It is now 10% on all difficulties because you already take less damage on lower difficulties.
        public const double DefenseDamageRatio = 0.1;
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
        public bool justHitByDefenseDamage = false;
        public int defenseDamageToTake = 0;
        #endregion

        #region Abyss
        public float abyssBreathLossStat = 0;
        public float abyssBreathLossRateStat = 0;
        public int abyssLifeLostAtZeroBreathStat = 0;
        public int abyssDefenseLossStat = 0;
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
        public bool luxorsGift = false;
        public bool fungalSymbiote = false;
        public bool trinketOfChi = false;
        public bool gladiatorSword = false;
        public bool unstableGraniteCore = false;
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
        public int nanomachinesLockoutTimer = 0;
        public bool vexation = false;
        public bool dodgeScarf = false;
        public bool evasionScarf = false;
        public bool badgeOfBravery = false;
        public bool warbannerOfTheSun = false;
        public float warBannerBonus = 0f;
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
        public int abyssalDivingSuitPlateHits = 0;
        public bool aquaticHeartWaterBuff = false;
        public bool aquaticHeartIce = false;
        public bool aSpark = false;
        public bool aSparkRare = false;
        public bool aBulwarkRare = false;
        public bool dAmulet = false;
        public bool fCarapace = false;
        public bool gShell = false;
        public bool absorber = false;
        public bool aAmpoule = false;
        public bool sponge = false;
        public bool rOoze = false;
        public bool fBarrier = false;
        public bool aBrain = false;
        public bool amalgam = false;
        public bool raiderTalisman = false;
        public float raiderCritBonus = RaidersTalisman.RaiderBonus;
        public int raiderSoundCooldown = 0;
        public bool gSabaton = false;
        public int gSabatonFall = 0;
        public int gSabatonCooldown = 0;
        public bool sGlyph = false;
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
        public bool deadshotBrooch = false;
        public bool shadowMinions = false;
        public bool holyMinions = false;
        public bool alchFlask = false;
        public bool abaddon = false;
        public bool aeroStone = false;
        public bool community = false;
        public bool shatteredCommunity = false;
        public bool fleshTotem = false;
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
        public bool fairyBoots = false;
        public bool hellfireTreads = false;
        public bool abyssalAmulet = false;
        public bool lumenousAmulet = false;
        public bool aquaticEmblem = false;
        public bool spiritOrigin = false;
        public bool spiritOriginVanity = false;
        public bool darkSunRing = false;
        public bool voidOfCalamity = false;
        public bool voidOfExtinction = false;
        public bool eArtifact = false;
        public bool dArtifact = false;
        public bool gArtifact = false;
        public bool pArtifact = false;
        public bool giantPearl = false;
        public bool normalityRelocator = false;
        public bool flameLickedShell = false;
        public bool manaOverloader = false;
        public bool royalGel = false;
        public bool handWarmer = false;
        public bool ursaSergeant = false;
        public bool scuttlersJewel = false;
        public int scuttlerCooldown = 0;
        public bool thiefsDime = false;
        public bool dynamoStemCells = false;
        public bool etherealExtorter = false;
        public bool blazingCore = false;
        public bool voltaicJelly = false;
        public bool jellyChargedBattery = false;
        public float jellyDmg;
        public bool oldDukeScales = false;
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
        public bool abyssalMirror = false;
        public bool eclipseMirror = false;
        public bool featherCrown = false;
        public bool moonCrown = false;
        public int rogueCrownCooldown = 0;
        public bool dragonScales = false;
        public bool gloveOfPrecision = false;
        public bool gloveOfRecklessness = false;
        public bool vampiricTalisman = false;
        public bool electricianGlove = false;
        public bool bloodyGlove = false;
        public bool filthyGlove = false;
        public bool sandCloak = false;
        public bool spectralVeil = false;
        public int spectralVeilImmunity = 0;
        public bool hasJetpack = false;
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
        public bool RustyMedallionDroplets = false;
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
        public bool voidField = false;
        public bool copyrightInfringementShield = false;
        #endregion

        #region Armor Set
        public bool silverMedkit = false;
        public int silverMedkitTimer = 0;
        public bool goldArmorGoldDrops = false;
        public bool miningSet = false;
        public int miningSetCooldown = 0;
        public bool desertProwler = false;
        public bool snowRuffianSet = false;
        public bool forbiddenCirclet = false;
        public int forbiddenCooldown = 0;
        public int tornadoCooldown = 0;
        public bool eskimoSet = false; //vanilla armor
        public bool meteorSet = false; //vanilla armor, for space gun nerf
        public bool victideSet = false;
        public bool victideSummoner = false;
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
        public int tarraDefenseTime = 600;
        public bool tarraMage = false;
        public int tarraMageHealCooldown = 0;
        public int tarraCrits = 0;
        public bool tarraRanged = false;
        public int tarraRangedCooldown = 0;
        public bool tarraThrowing = false;
        public bool tarragonImmunity = false;
        public int tarraThrowingCrits = 0;
        public bool tarraSummon = false;
        public bool bloodflareSet = false;
        public bool bloodflareMelee = false;
        public bool bloodflareFrenzy = false;
        public int bloodflareMeleeHits = 0;
        public bool bloodflareRanged = false;
        public bool bloodflareThrowing = false;
        public bool bloodflareMage = false;
        public int bloodflareMageCooldown = 0;
        public bool bloodflareSummon = false;
        public int bloodflareSummonTimer = 0;
        public bool godSlayer = false;
        public bool godSlayerDamage = false;
        public bool godSlayerRanged = false;
        public bool godSlayerThrowing = false;
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
        public bool auricSet = false;
        public bool omegaBlueChestplate = false;
        public bool omegaBlueSet = false;
        public bool omegaBlueHentai = false;
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
        public int ChlorophyteHealDelay = 0;
        public bool WearingPostMLSummonerSet = false;

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
                    gemTechState = new GemTechArmorState(Player.whoAmI);
                return gemTechState;
            }
            set => gemTechState = value;
        }
        #endregion

        #region Debuff
        public bool alcoholPoisoning = false;
        public bool shadowflame = false;
        public bool wDeath = false;
        public bool dragonFire = false;
        public bool aCrunch = false;
        public bool irradiated = false;
        public bool bFlames = false;
        public bool weakBrimstoneFlames = false;
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
        public bool clamity = false;
        public bool sulphurPoison = false;
        public bool nightwither = false;
        public bool eFreeze = false;
        public bool eutrophication = false;
        public bool iCantBreathe = false; //Frozen Lungs debuff
        public bool cragsLava = false;
        public bool vaporfied = false;
        public bool banishingFire = false;
        public bool wither = false;
        public bool ManaBurn = false;

        public const int SulphSeaWaterSafetyTime = 720;
        public const int SulphSeaWaterRecoveryTime = 150;
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
        public bool omniscience = false;
        public bool zerg = false;
        public bool zen = false;
        public bool isNearbyBoss = false;
        public bool flaskBrimstone = false;
        public bool fabsolVodka = false;
        public bool mushy = false;
        public bool molten = false;
        public bool shellBoost = false;
        public bool cFreeze = false;
        public bool shine = false;
        public bool anechoicCoating = false;
        public bool enraged = false;
        public bool permafrostsConcoction = false;
        public bool flaskCrumbling = false;
        public bool ceaselessHunger = false;
        public bool calcium = false;
        public bool soaring = false;
        public bool bounding = false;
        public bool shadow = false;
        public bool photosynthesis = false;
        public bool astralInjection = false;
        public bool gravityNormalizer = false;
        public bool flaskHoly = false;
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
        public bool brittleStar = false;
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
        public bool victideSnail = false;
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
        public bool puffWarrior = false;
        public bool vileFeeder = false;
        public bool scabRipper = false;
        public bool midnightUFO = false;
        public bool plagueEngine = false;
        public bool brimseeker = false;
        public bool necrosteocytesDudes = false;
        public bool gammaHead = false;
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
        public bool ViridVanguard = false;
        public bool sageSpirit = false;
        public bool fleshBall = false;
        public bool eyeOfNight = false;
        public bool soulSeeker = false;
        public bool perditionBeacon = false;
        public bool MoonFist = false;
        public bool AresCannons = false;

        public List<DeadMinionProperties> PendingProjectilesToRespawn = new List<DeadMinionProperties>();

        // Due to the way vanilla summons work, the buff must be applied manually for it to properly register, since
        // the buff is typically created via the minion's item usage, not its idle existence.
        public static Dictionary<int, int> VanillaMinionBuffRelationship = new Dictionary<int, int>()
        {
            [ProjectileID.BabyBird] = BuffID.BabyBird,
            [ProjectileID.BabySlime] = BuffID.BabySlime,
            [ProjectileID.BabyHornet] = BuffID.BabyHornet,
            [ProjectileID.FlinxMinion] = BuffID.FlinxMinion,
            [ProjectileID.Hornet] = BuffID.HornetMinion,
            [ProjectileID.FlyingImp] = BuffID.ImpMinion,
            [ProjectileID.VampireFrog] = BuffID.VampireFrog,
            [ProjectileID.VenomSpider] = BuffID.SpiderMinion,
            [ProjectileID.JumperSpider] = BuffID.SpiderMinion,
            [ProjectileID.DangerousSpider] = BuffID.SpiderMinion,
            [ProjectileID.BatOfLight] = BuffID.BatOfLight,
            [ProjectileID.Smolstar] = BuffID.Smolstar,
            [ProjectileID.Spazmamini] = BuffID.TwinEyesMinion,
            [ProjectileID.Retanimini] = BuffID.TwinEyesMinion,
            [ProjectileID.StormTigerTier1] = BuffID.StormTiger,
            [ProjectileID.StormTigerTier2] = BuffID.StormTiger,
            [ProjectileID.StormTigerTier3] = BuffID.StormTiger,
            [ProjectileID.Raven] = BuffID.Ravens,
            [ProjectileID.DeadlySphere] = BuffID.DeadlySphere,
            [ProjectileID.Tempest] = BuffID.SharknadoMinion,
            [ProjectileID.EmpressBlade] = BuffID.EmpressBlade,
            [ProjectileID.UFOMinion] = BuffID.UFOMinion,
            [ProjectileID.StardustCellMinion] = BuffID.StardustMinion,
            [ProjectileID.StardustDragon1] = BuffID.StardustDragonMinion,
        };

        #endregion

        #region Biome
        public bool ZoneCalamity => Player.InModBiome(ModContent.GetInstance<BrimstoneCragsBiome>());
        public bool ZoneAstral => (Player.InModBiome(ModContent.GetInstance<AbovegroundAstralBiome>()) ||
            Player.InModBiome(ModContent.GetInstance<AbovegroundAstralSnowBiome>()) ||
            Player.InModBiome(ModContent.GetInstance<AbovegroundAstralDesertBiome>()) ||
            Player.InModBiome(ModContent.GetInstance<UndergroundAstralBiome>())) && !ZoneAbyss;
        public bool ZoneSunkenSea => Player.InModBiome(ModContent.GetInstance<SunkenSeaBiome>());
        public bool ZoneSulphur => Player.InModBiome(ModContent.GetInstance<SulphurousSeaBiome>());
        public bool ZoneAbyss => ZoneAbyssLayer1 || ZoneAbyssLayer2 || ZoneAbyssLayer3 || ZoneAbyssLayer4;
        public bool ZoneAbyssLayer1 => Player.InModBiome(ModContent.GetInstance<AbyssLayer1Biome>());
        public bool ZoneAbyssLayer2 => Player.InModBiome(ModContent.GetInstance<AbyssLayer2Biome>());
        public bool ZoneAbyssLayer3 => Player.InModBiome(ModContent.GetInstance<AbyssLayer3Biome>());
        public bool ZoneAbyssLayer4 => Player.InModBiome(ModContent.GetInstance<AbyssLayer4Biome>());
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
        public bool aquaticHeartPrevious;
        public bool aquaticHeart;
        public bool aquaticHeartHide;
        public bool aquaticHeartForce;
        public bool aquaticHeartPower;
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

        public FluidField CalamityFireDrawer;

        public FluidField ProfanedMoonlightAuroraDrawer;

        public ArmorShaderData CalamityFireDyeShader = null;

        public Vector2 FireDrawerPosition;

        public int monolithAccursedShader = 0;
        #endregion Draw Effects

        #region Draedon Summoning
        public bool AbleToSelectExoMech = false;
        public bool HasTalkedAtCodebreaker = false;
        public List<ulong> SeenDraedonDialogs = new();
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
            newGolferInventory = false;
            newZoologistInventory = false;
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
            newPrincessInventory = false;
            newSkeletonMerchantInventory = false;
            newPermafrostInventory = false;
            newCirrusInventory = false;
            newAmidiasInventory = false;
            newBanditInventory = false;
            newCalamitasInventory = false;

            cooldowns = new Dictionary<string, CooldownInstance>(16);
        }

        public override void SaveData(TagCompound tag)
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
            boost.AddWithCondition("newGolferInventory", newGolferInventory);
            boost.AddWithCondition("newZoologistInventory", newZoologistInventory);
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
            boost.AddWithCondition("newPrincessInventory", newPrincessInventory);
            boost.AddWithCondition("newSkeletonMerchantInventory", newSkeletonMerchantInventory);
            boost.AddWithCondition("newPermafrostInventory", newPermafrostInventory);
            boost.AddWithCondition("newCirrusInventory", newCirrusInventory);
            boost.AddWithCondition("newAmidiasInventory", newAmidiasInventory);
            boost.AddWithCondition("newBanditInventory", newBanditInventory);
            boost.AddWithCondition("newCalamitasInventory", newCalamitasInventory);
            boost.AddWithCondition("GivenBrimstoneLocus", GivenBrimstoneLocus);
            boost.AddWithCondition("HasTalkedAtCodebreaker", HasTalkedAtCodebreaker);

            // Calculate the new total time of all sessions at the instant of this player save.
            TimeSpan newSessionTotal = previousSessionTotal.Add(CalamityMod.SpeedrunTimer.Elapsed);
            long totalTicks = newSessionTotal.Ticks;

            // Save all cooldowns which are marked as persisting through save/load.
            TagCompound cooldownsTag = new TagCompound();
            var cdIterator = cooldowns.GetEnumerator();
            while (cdIterator.MoveNext())
            {
                KeyValuePair<string, CooldownInstance> kv = cdIterator.Current;
                string id = kv.Key;
                CooldownInstance instance = kv.Value;

                // If the cooldown isn't supposed to persist, skip it.
                if (!instance.handler.SavedWithPlayer)
                    continue;

                // Add this cooldown to the overall cooldowns tag compound using its ID as the string key.
                TagCompound singleCDTag = instance.Save();
                cooldownsTag.Add(id, singleCDTag);
            }

            tag["boost"] = boost;
            tag["rage"] = rage;
            tag["adrenaline"] = adrenaline;
            tag["aquaticBoostPower"] = aquaticBoost;
            tag["sCalDeathCount"] = sCalDeathCount;
            tag["sCalKillCount"] = sCalKillCount;
            tag["moveSpeedBonus"] = moveSpeedBonus;
            tag["defenseDamage"] = totalDefenseDamage;
            tag["defenseDamageRecoveryFrames"] = defenseDamageRecoveryFrames;
            tag["totalSpeedrunTicks"] = totalTicks;
            tag["lastSplitType"] = lastSplitType;
            tag["lastSplitTicks"] = lastSplit.Ticks;
            tag["cooldowns"] = cooldownsTag;
            tag["SeenDraedonDialogs"] = SeenDraedonDialogs;
        }

        public override void LoadData(TagCompound tag)
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
            newGolferInventory = boost.Contains("newGolferInventory");
            newZoologistInventory = boost.Contains("newZoologistInventory");
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
            newPrincessInventory = boost.Contains("newPrincessInventory");
            newSkeletonMerchantInventory = boost.Contains("newSkeletonMerchantInventory");
            newPermafrostInventory = boost.Contains("newPermafrostInventory");
            newCirrusInventory = boost.Contains("newCirrusInventory");
            newAmidiasInventory = boost.Contains("newAmidiasInventory");
            newBanditInventory = boost.Contains("newBanditInventory");
            newCalamitasInventory = boost.Contains("newCalamitasInventory");
            GivenBrimstoneLocus = boost.Contains("GivenBrimstoneLocus");
            HasTalkedAtCodebreaker = boost.Contains("HasTalkedAtCodebreaker");

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
                catch (Exception) { failedToLoadFloatAdrenaline = true; }

                if (failedToLoadFloatAdrenaline)
                    adrenaline = tag.GetInt("adrenaline") / (10000 / 100f);
            }

            if (tag.ContainsKey("aquaticBoostPower"))
                aquaticBoost = tag.GetFloat("aquaticBoostPower");
            sCalDeathCount = tag.GetInt("sCalDeathCount");
            sCalKillCount = tag.GetInt("sCalKillCount");
            deathCount = tag.GetInt("deathCount");

            if (tag.ContainsKey("moveSpeedBonus"))
                moveSpeedBonus = tag.GetFloat("moveSpeedBonus");
            totalDefenseDamage = tag.GetInt("defenseDamage");
            defenseDamageRecoveryFrames = tag.GetInt("defenseDamageRecoveryFrames");
            if (defenseDamageRecoveryFrames < 0)
                defenseDamageRecoveryFrames = 0;
            totalDefenseDamageRecoveryFrames = tag.GetInt("totalDefenseDamageRecoveryFrames");
            if (totalDefenseDamageRecoveryFrames <= 0)
                totalDefenseDamageRecoveryFrames = DefenseDamageBaseRecoveryTime;

            // Load the previous total elapsed time to know where to start the timer when it starts.
            long ticks = tag.GetLong("totalSpeedrunTicks");
            previousSessionTotal = new TimeSpan(ticks);
            // Also load the last split, so it will show up.
            lastSplitType = tag.GetInt("lastSplitType");
            ticks = tag.GetLong("lastSplitTicks");
            lastSplit = new TimeSpan(ticks);
            SeenDraedonDialogs = tag.GetList<ulong>("SeenDraedonDialogs").ToList();

            // Clear the player's cooldowns in preparation for loading.
            cooldowns.Clear();
            if (!tag.ContainsKey("cooldowns"))
                return;

            // Load cooldowns and add them to the player's cooldown list.
            TagCompound cooldownsTag = tag.GetCompound("cooldowns");
            var tagIterator = cooldownsTag.GetEnumerator();
            while (tagIterator.MoveNext())
            {
                KeyValuePair<string, object> kv = tagIterator.Current;
                string id = kv.Key;
                TagCompound singleCDTag = cooldownsTag.GetCompound(id);
                CooldownInstance instance = new CooldownInstance(Player, id, singleCDTag);
                cooldowns.Add(id, instance);
            }
        }
        #endregion

        #region ResetEffects
        public override void ResetEffects()
        {
            // Max health bonuses
            if (absorber)
                Player.statLifeMax2 += sponge ? 30 : 20;
            Player.statLifeMax2 +=
                (mFruit ? 25 : 0) +
                (bOrange ? 25 : 0) +
                (eBerry ? 25 : 0) +
                (dFruit ? 25 : 0);
            if (fleshKnuckles)
                Player.statLifeMax2 += 45;

            int percentMaxLifeIncrease = 0;
            if (ZoneAbyss && abyssalAmulet)
                percentMaxLifeIncrease += lumenousAmulet ? 25 : 10;
            if (coreOfTheBloodGod)
                percentMaxLifeIncrease += 15;
            if (bloodPact)
                percentMaxLifeIncrease += 100;
            if (affliction || afflicted)
                percentMaxLifeIncrease += 10;

            if (community)
            {
                float BoostAtZeroBosses = 0.05f;
                float BoostPostYharon = 0.2f;
                float floatTypeBoost = MathHelper.Lerp(BoostAtZeroBosses, BoostPostYharon, TheCommunity.CalculatePower());
                int integerTypeBoost = (int)(floatTypeBoost * 50f);
                percentMaxLifeIncrease += integerTypeBoost;
            }

            // Shattered Community gives the same max health boost as normal full-power Community (10%)
            if (shatteredCommunity)
                percentMaxLifeIncrease += 10;

            Player.statLifeMax2 += Player.statLifeMax / 5 / 20 * percentMaxLifeIncrease;

            // Max health reductions
            if (crimEffigy)
                Player.statLifeMax2 = (int)(Player.statLifeMax2 * 0.9);

            ResetRogueStealth();

            // Reset adrenaline duration to default. If Draedon's Heart is equipped, it'll change itself every frame.
            AdrenalineDuration = CalamityUtils.SecondsToFrames(5);

            contactDamageReduction = 0D;
            projectileDamageReduction = 0D;
            rogueVelocity = 1f;
            rogueAmmoCost = 1f;
            accStealthGenBoost = 0f;

            DashID = string.Empty;
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
            rimehound = false;
            fab = false;
            crysthamyr = false;
            ExoChair = false;
            miniOldDuke = false;

            abyssalDivingSuitPlates = false;

            aquaticHeartWaterBuff = false;
            aquaticHeartIce = false;

            draedonsHeart = false;

            afflicted = false;
            affliction = false;

            dodgeScarf = false;
            evasionScarf = false;

            elysianAegis = false;

            nCore = false;

            godSlayer = false;
            godSlayerDamage = false;
            godSlayerRanged = false;
            godSlayerThrowing = false;

            silvaSet = false;
            silvaMage = false;
            silvaSummon = false;

            auricSet = false;
            auricBoost = false;

            GemTechSet = false;

            CobaltSet = false;
            MythrilSet = false;
            AdamantiteSet = false;

            WearingPostMLSummonerSet = false;

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
            unstableGraniteCore = false;
            regenator = false;
            deepDiver = false;
            theBee = false;
            alluringBait = false;
            enchantedPearl = false;
            fishingStation = false;
            rBrain = false;
            bloodyWormTooth = false;
            vexation = false;
            badgeOfBravery = false;
            warbannerOfTheSun = false;
            aSpark = false;
            aSparkRare = false;
            aBulwarkRare = false;
            dAmulet = false;
            fCarapace = false;
            gShell = false;
            absorber = false;
            aAmpoule = false;
            sponge = false;
            rOoze = false;
            fBarrier = false;
            aBrain = false;
            amalgam = false;
            frostFlare = false;
            beeResist = false;
            uberBees = false;
            projRefRare = false;
            nanotech = false;
            deadshotBrooch = false;
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
            abaddon = false;
            aeroStone = false;
            community = false;
            shatteredCommunity = false;
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
            angelTreads = false;
            harpyWingBoost = false; //harpy wings + harpy ring
            fleshKnuckles = false;
            darkSunRing = false;
            voidOfCalamity = false;
            voidOfExtinction = false;
            eArtifact = false;
            dArtifact = false;
            gArtifact = false;
            pArtifact = false;
            giantPearl = false;
            normalityRelocator = false;
            flameLickedShell = false;
            manaOverloader = false;
            royalGel = false;
            handWarmer = false;
            raiderTalisman = false;
            gSabaton = false;
            sGlyph = false;
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
            ursaSergeant = false;
            scuttlersJewel = false;
            thiefsDime = false;
            dynamoStemCells = false;
            etherealExtorter = false;
            oldDukeScales = false;
            blazingCore = false;
            voltaicJelly = false;
            jellyChargedBattery = false;
            starbusterCore = false;
            starTaintedGenerator = false;
            camper = false;
            silvaWings = false;
            corrosiveSpine = false;
            RustyMedallionDroplets = false;
            noStupidNaturalARSpawns = false;
            roverDrive = false;
            rottenDogTooth = false;
            angelicAlliance = false;
            BloomStoneRegen = false;
            ChaosStone = false;
            CryoStone = false;
            voidField = false;
            copyrightInfringementShield = false;

            daedalusReflect = false;
            daedalusSplit = false;
            daedalusAbsorb = false;
            daedalusShard = false;

            brimflameSet = false;
            brimflameFrenzy = false;

            rangedAmmoCost = 1f;
            healingPotBonus = 1f;

            avertorBonus = false;

            reaverRegen = false;
            reaverSpeed = false;
            reaverDefense = false;
            reaverExplore = false;

            ironBoots = false;
            depthCharm = false;
            anechoicPlating = false;
            jellyfishNecklace = false;
            fairyBoots = false;
            hellfireTreads = false;
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

            silverMedkit = false;
            goldArmorGoldDrops = false;

            miningSet = false;
            miningSetCooldown = 0;

            eskimoSet = false; //vanilla armor
            meteorSet = false; //vanilla armor, for Space Gun nerf

            victideSet = false;
            victideSummoner = false;

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
            tarraMage = false;
            tarraRanged = false;
            tarraThrowing = false;
            tarragonImmunity = false;
            tarraSummon = false;

            bloodflareSet = false;
            bloodflareMelee = false;
            bloodflareFrenzy = false;
            bloodflareRanged = false;
            bloodflareThrowing = false;
            bloodflareMage = false;
            bloodflareSummon = false;

            xerocSet = false;

            weakPetrification = false;

            inkBomb = false;
            darkGodSheath = false;
            abyssalMirror = false;
            eclipseMirror = false;
            featherCrown = false;
            moonCrown = false;
            dragonScales = false;
            gloveOfPrecision = false;
            gloveOfRecklessness = false;
            vampiricTalisman = false;
            electricianGlove = false;
            bloodyGlove = false;
            filthyGlove = false;
            sandCloak = false;
            spectralVeil = false;
            hasJetpack = false;
            plaguedFuelPack = false;
            blunderBooster = false;
            veneratedLocket = false;

            alcoholPoisoning = false;
            shadowflame = false;
            wDeath = false;
            dragonFire = false;
            aCrunch = false;
            irradiated = false;
            bFlames = false;
            witheredDebuff = false;
            weakBrimstoneFlames = false;
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
            clamity = false;
            enraged = false;
            snowmanNoseless = false;
            sulphurPoison = false;
            nightwither = false;
            eFreeze = false;
            eutrophication = false;
            iCantBreathe = false;
            cragsLava = false;
            vaporfied = false;
            banishingFire = false;
            wither = false;
            ManaBurn = false;

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
            omniscience = false;
            zerg = false;
            zen = false;
            isNearbyBoss = false;
            permafrostsConcoction = false;
            flaskCrumbling = false;
            ceaselessHunger = false;
            calcium = false;
            soaring = false;
            bounding = false;
            shadow = false;
            photosynthesis = false;
            astralInjection = false;
            gravityNormalizer = false;
            flaskHoly = false;
            tesla = false;
            teslaFreeze = false;
            sulphurskin = false;
            baguette = false;
            trippy = false;
            amidiasBlessing = false;
            flaskBrimstone = false;
            fabsolVodka = false;
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
            brittleStar = false;
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
            victideSnail = false;
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
            puffWarrior = false;
            vileFeeder = false;
            scabRipper = false;
            midnightUFO = false;
            plagueEngine = false;
            brimseeker = false;
            necrosteocytesDudes = false;
            gammaHead = false;
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
            ViridVanguard = false;
            sageSpirit = false;
            fleshBall = false;
            eyeOfNight = false;
            soulSeeker = false;
            perditionBeacon = false;
            MoonFist = false;
            AresCannons = false;

            disableVoodooSpawns = false;
            disablePerfCystSpawns = false;
            disableHiveCystSpawns = false;
            disableNaturalScourgeSpawns = false;
            disableAnahitaSpawns = false;

            abyssalDivingSuitPrevious = abyssalDivingSuit;
            abyssalDivingSuit = abyssalDivingSuitHide = abyssalDivingSuitForce = abyssalDivingSuitPower = false;

            aquaticHeartPrevious = aquaticHeart;
            aquaticHeart = aquaticHeartHide = aquaticHeartForce = aquaticHeartPower = false;

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
            RageDuration = BalancingConstants.DefaultRageDuration;
            RageDamageBoost = BalancingConstants.DefaultRageDamageBoost;

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

            AbleToSelectExoMech = false;

            EnchantHeldItemEffects(Player, Player.Calamity(), Player.ActiveItem());
        }
        #endregion

        #region Screen Position Movements
        public override void ModifyScreenPosition()
        {
            if (!CalamityConfig.Instance.Screenshake)
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
            // This function runs every frame the player is dead, so if the player does not have any cooldowns, don't try to remove any.
            if (cooldowns.Count > 0)
            {
                // Iterate through all cooldowns and find those which do not persist through death.
                IList<string> removedCooldowns = new List<string>(16);
                var cdIterator = cooldowns.GetEnumerator();
                while (cdIterator.MoveNext())
                {
                    KeyValuePair<string, CooldownInstance> kv = cdIterator.Current;
                    string id = kv.Key;
                    CooldownInstance instance = kv.Value;
                    CooldownHandler handler = instance.handler;
                    if (!handler.PersistsThroughDeath)
                        removedCooldowns.Add(id);
                }
                cdIterator.Dispose();

                // Actually remove all cooldowns which do not persist through death.
                // If any cooldowns were removed, net sync the remaining cooldown dictionary.
                if (removedCooldowns.Count > 0)
                {
                    foreach (string cdID in removedCooldowns)
                        cooldowns.Remove(cdID);
                    SyncCooldownDictionary(Main.netMode == NetmodeID.Server);
                }
            }

            #region Debuffs
            totalDefenseDamage = 0;
            defenseDamageRecoveryFrames = 0;
            totalDefenseDamageRecoveryFrames = DefenseDamageBaseRecoveryTime;
            defenseDamageDelayFrames = 0;
            justHitByDefenseDamage = false;
            defenseDamageToTake = 0;
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
            aBulwarkRareTimer = 0;
            RustyMedallionCooldown = 0;
            SulphWaterPoisoningLevel = 0f;
            spiritOriginConvertedCrit = 0;
            rage = 0f;
            adrenaline = 0f;
            raiderCritBonus = 0f;
            raiderSoundCooldown = 0;
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
            dragonRageCooldown = 0;
            spectralVeilImmunity = 0;
            jetPackDash = 0;
            jetPackDirection = 0;
            andromedaCripple = 0;
            theBeeCooldown = 0;
            killSpikyBalls = false;
            scuttlerCooldown = 0;
            rogueCrownCooldown = 0;
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
            dragonFire = false;
            aCrunch = false;
            irradiated = false;
            bFlames = false;
            witheredDebuff = false;
            weakBrimstoneFlames = false;
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
            clamity = false;
            snowmanNoseless = false;
            abyssalDivingSuitPlateHits = 0;
            sulphurPoison = false;
            nightwither = false;
            eFreeze = false;
            eutrophication = false;
            iCantBreathe = false;
            cragsLava = false;
            vaporfied = false;
            banishingFire = false;
            wither = false;
            #endregion

            #region Rogue
            // Stealth
            rogueStealth = 0f;
            rogueStealthMax = 0f;
            stealthAcceleration = 1f;

            stealthDamage = 0f;
            bonusStealthDamage = 0;
            rogueVelocity = 1f;
            rogueAmmoCost = 1f;
            #endregion

            #region UI
            if (stealthUIAlpha > 0f)
            {
                stealthUIAlpha -= 0.035f;
                stealthUIAlpha = MathHelper.Clamp(stealthUIAlpha, 0f, 1f);
            }
            if (SulphWaterUIOpacity > 0f)
                SulphWaterUIOpacity = MathHelper.Clamp(SulphWaterUIOpacity - 0.035f, 0f, 1f);
            #endregion

            #region Buffs
            sRegen = false;
            hallowedRegen = false;
            hallowedPower = false;
            onyxExcavator = false;
            rimehound = false;
            fab = false;
            crysthamyr = false;
            ExoChair = false;
            abyssalDivingSuitPlates = false;
            aquaticHeartWaterBuff = false;
            aquaticHeartIce = false;
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
            omniscience = false;
            zerg = false;
            zen = false;
            isNearbyBoss = false;
            permafrostsConcoction = false;
            flaskCrumbling = false;
            ceaselessHunger = false;
            calcium = false;
            soaring = false;
            bounding = false;
            shadow = false;
            nanomachinesLockoutTimer = 0;
            photosynthesis = false;
            astralInjection = false;
            gravityNormalizer = false;
            flaskHoly = false;
            tesla = false;
            teslaFreeze = false;
            sulphurskin = false;
            baguette = false;
            flaskBrimstone = false;
            fabsolVodka = false;
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
            healCounter = 300;
            danceOfLightCharge = 0;
            bloodPactBoost = false;
            rangedAmmoCost = 1f;
            healingPotBonus = 1f;
            avertorBonus = false;
            divineBless = false;
            #endregion

            #region Armorbonuses
            silverMedkit = false;
            silverMedkitTimer = 0;
            goldArmorGoldDrops = false;
            miningSet = false;
            miningSetCooldown = 0;
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
            WearingPostMLSummonerSet = false;
            AdamantiteSetDecayDelay = 0;
            ChlorophyteHealDelay = 0;
            omegaBlueChestplate = false;
            omegaBlueSet = false;
            molluskSet = false;
            fearmongerSet = false;
            daedalusReflect = false;
            daedalusSplit = false;
            daedalusAbsorb = false;
            daedalusShard = false;
            brimflameSet = false;
            brimflameFrenzy = false;
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
            tarraDefenseTime = 600;
            tarraMage = false;
            tarraRanged = false;
            tarraThrowing = false;
            tarragonImmunity = false;
            tarraThrowingCrits = 0;
            tarraSummon = false;
            bloodflareSet = false;
            bloodflareMelee = false;
            bloodflareFrenzy = false;
            bloodflareMeleeHits = 0;
            bloodflareRanged = false;
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
            brimlashBusterBoost = false;
            evilSmasherBoost = 0;
            hellbornBoost = 0;
            searedPanCounter = 0;
            searedPanTimer = 0;
            potionTimer = 0;
            bloodflareCoreLostDefense = 0;
            persecutedEnchantSummonTimer = 0;
            momentumCapacitorTime = 0;
            momentumCapacitorBoost = 0f;
            LungingDown = false;

            if (BossRushEvent.BossRushActive)
            {
                var source = new ProjectileSource_Death(Player);
                if (Player.whoAmI == 0 && !CalamityGlobalNPC.AnyLivingPlayers() && CalamityUtils.CountProjectiles(ModContent.ProjectileType<BossRushFailureEffectThing>()) == 0)
                    Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ModContent.ProjectileType<BossRushFailureEffectThing>(), 0, 0f);
            }

            // Respawn the player faster if no bosses are alive
            if (!areThereAnyDamnBosses)
            {
                int respawnTimerSet = areThereAnyDamnEvents ? 360 : 180;
                if (Player.respawnTimer > respawnTimerSet)
                    Player.respawnTimer = respawnTimerSet;
            }
        }
        #endregion

        #region InventoryStartup
        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            static Item createItem(int type)
            {
                Item i = new Item();
                i.SetDefaults(type);
                return i;
            }

            if (!mediumCoreDeath)
                yield return createItem(ModContent.ItemType<StarterBag>());
        }
        #endregion

        #region Keybinds

        public Item FindAccessory(int itemID)
        {
            for (int i = 0; i < 10; i++)
            {
                if (Player.armor[i].type == itemID)
                    return Player.armor[i];
            }
            return new Item();
        }

        

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (CalamityKeybinds.NormalityRelocatorHotKey.JustPressed && normalityRelocator && Main.myPlayer == Player.whoAmI)
            {
                if (!Player.CCed && !Player.chaosState)
                {
                    Vector2 teleportLocation;
                    teleportLocation.X = (float)Main.mouseX + Main.screenPosition.X;
                    if (Player.gravDir == 1f)
                    {
                        teleportLocation.Y = (float)Main.mouseY + Main.screenPosition.Y - (float)Player.height;
                    }
                    else
                    {
                        teleportLocation.Y = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY;
                    }
                    teleportLocation.X -= (float)(Player.width / 2);
                    if (teleportLocation.X > 50f && teleportLocation.X < (float)(Main.maxTilesX * 16 - 50) && teleportLocation.Y > 50f && teleportLocation.Y < (float)(Main.maxTilesY * 16 - 50))
                    {
                        if (!Collision.SolidCollision(teleportLocation, Player.width, Player.height))
                        {
                            Player.Teleport(teleportLocation, 4, 0);
                            NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, (float)Player.whoAmI, teleportLocation.X, teleportLocation.Y, 1, 0, 0);

                            int duration = areThereAnyDamnBosses ? chaosStateDuration_NR : 360;
                            Player.AddBuff(BuffID.ChaosState, duration, true);
                            // Add a cooldown here so it can have the custom NR icon
                            Player.AddCooldown(ChaosState.ID, duration, true, "normalityrelocator");
                        }
                    }
                }
            }
            if (CalamityKeybinds.AngelicAllianceHotKey.JustPressed && angelicAlliance && Main.myPlayer == Player.whoAmI && !divineBless && !Player.HasCooldown(Cooldowns.DivineBless.ID))
            {
                int seconds = CalamityUtils.SecondsToFrames(15f);
                Player.AddBuff(ModContent.BuffType<Buffs.StatBuffs.DivineBless>(), seconds, false);
                SoundEngine.PlaySound(AngelicAlliance.ActivationSound, Player.Center);

                // Spawn an archangel for every minion you have
                float angelAmt = 0f;
                for (int projIndex = 0; projIndex < Main.maxProjectiles; projIndex++)
                {
                    Projectile proj = Main.projectile[projIndex];
                    if (proj.minionSlots <= 0f || !proj.CountsAsClass<SummonDamageClass>())
                        continue;
                    if (proj.active && proj.owner == Player.whoAmI)
                    {
                        angelAmt += 1f;
                    }
                }

                var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<AngelicAlliance>()));
                for (int projIndex = 0; projIndex < angelAmt; projIndex++)
                {
                    Projectile proj = Main.projectile[projIndex];
                    float start = 360f / angelAmt;
                    Projectile.NewProjectile(source, new Vector2((int)(Player.Center.X + (Math.Sin(projIndex * start) * 300)), (int)(Player.Center.Y + (Math.Cos(projIndex * start) * 300))), Vector2.Zero, ModContent.ProjectileType<AngelicAllianceArchangel>(), proj.damage / 10, proj.knockBack / 10f, Player.whoAmI, Main.rand.Next(180), projIndex * start);
                    Player.statLife += 2;
                    Player.HealEffect(2);
                }
            }
            if (CalamityKeybinds.SandCloakHotkey.JustPressed && sandCloak && Main.myPlayer == Player.whoAmI && rogueStealth >= rogueStealthMax * 0.1f &&
                wearingRogueArmor && rogueStealthMax > 0 && !Player.HasCooldown(Cooldowns.SandCloak.ID))
            {
                Player.AddCooldown(Cooldowns.SandCloak.ID, CalamityUtils.SecondsToFrames(30));

                var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<Items.Accessories.SandCloak>()));
                rogueStealth -= rogueStealthMax * 0.1f;
                int veil = Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ModContent.ProjectileType<SandCloakVeil>(), 7, 8, Player.whoAmI);
                Main.projectile[veil].Center = Player.Center;
                SoundEngine.PlaySound(SoundID.Item45, Player.position);
            }
            if (CalamityKeybinds.SpectralVeilHotKey.JustPressed && spectralVeil && Main.myPlayer == Player.whoAmI && rogueStealth >= rogueStealthMax * 0.25f &&
                wearingRogueArmor && rogueStealthMax > 0)
            {
                if (!Player.chaosState)
                {
                    Vector2 teleportLocation;
                    teleportLocation.X = Main.mouseX + Main.screenPosition.X;
                    if (Player.gravDir == 1f)
                        teleportLocation.Y = Main.mouseY + Main.screenPosition.Y - Player.height;
                    else
                        teleportLocation.Y = Main.screenPosition.Y + Main.screenHeight - Main.mouseY;

                    teleportLocation.X -= Player.width * 0.5f;
                    Vector2 teleportOffset = teleportLocation - Player.position;
                    if (teleportOffset.Length() > SpectralVeil.TeleportRange)
                    {
                        teleportOffset = teleportOffset.SafeNormalize(Vector2.Zero) * SpectralVeil.TeleportRange;
                        teleportLocation = Player.position + teleportOffset;
                    }
                    if (teleportLocation.X > 50f && teleportLocation.X < (float)(Main.maxTilesX * 16 - 50) && teleportLocation.Y > 50f && teleportLocation.Y < (float)(Main.maxTilesY * 16 - 50))
                    {
                        if (!Collision.SolidCollision(teleportLocation, Player.width, Player.height))
                        {
                            rogueStealth -= rogueStealthMax * 0.25f;

                            Player.Teleport(teleportLocation, 1);
                            NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, (float)Player.whoAmI, teleportLocation.X, teleportLocation.Y, 1, 0, 0);

                            int duration = chaosStateDuration;
                            Player.AddBuff(BuffID.ChaosState, duration, true);
                            Player.AddCooldown(ChaosState.ID, duration, true, "spectralveil");

                            int numDust = 40;
                            Vector2 step = teleportOffset / numDust;
                            for (int i = 0; i < numDust; i++)
                            {
                                int dustIndex = Dust.NewDust(Player.Center - (step * i), 1, 1, 21, step.X, step.Y);
                                Main.dust[dustIndex].noGravity = true;
                                Main.dust[dustIndex].noLight = true;
                            }

                            spectralVeilImmunity = 60;
                        }
                    }
                }
            }
            if (CalamityKeybinds.PlaguePackHotKey.JustPressed && hasJetpack && Main.myPlayer == Player.whoAmI && rogueStealth >= rogueStealthMax * 0.25f &&
                wearingRogueArmor && rogueStealthMax > 0 && !Player.HasCooldown(RogueBooster.ID) && !Player.mount.Active)
            {
                jetPackDash = blunderBooster ? 15 : 10;
                jetPackDirection = Player.direction;
                Player.AddCooldown(RogueBooster.ID, 60, true, blunderBooster ? "birb" : "default");
                rogueStealth -= rogueStealthMax * 0.25f;
                SoundEngine.PlaySound(SoundID.Item66, Player.Center);
                SoundEngine.PlaySound(SoundID.Item34, Player.Center);
            }

            // TODO -- It would be nice if triggerable set bonuses used interfaces instead of having to go through this large if chain.
            if (CalamityKeybinds.SetBonusHotKey.JustPressed)
            {
                if (brimflameSet && !Player.HasCooldown(BrimflameFrenzy.ID))
                {
                    if (Player.whoAmI == Main.myPlayer)
                    {
                        if (brimflameFrenzy)
                        {
                            brimflameFrenzy = false;
                            Player.ClearBuff(ModContent.BuffType<BrimflameFrenzyBuff>());
                        }
                        else
                        {
                            brimflameFrenzy = true;
                            Player.AddBuff(ModContent.BuffType<BrimflameFrenzyBuff>(), 10 * 60, true);
                            SoundEngine.PlaySound(BrimflameScowl.ActivationSound, Player.Center);
                            for (int num502 = 0; num502 < 36; num502++)
                            {
                                int dust = Dust.NewDust(new Vector2(Player.position.X, Player.position.Y + 16f), Player.width, Player.height - 16, (int)CalamityDusts.Brimstone, 0f, 0f, 0, default, 1f);
                                Main.dust[dust].velocity *= 3f;
                                Main.dust[dust].scale *= 1.15f;
                            }
                            int num226 = 36;
                            for (int num227 = 0; num227 < num226; num227++)
                            {
                                Vector2 vector6 = Vector2.Normalize(Player.velocity) * new Vector2((float)Player.width / 2f, (float)Player.height) * 0.75f;
                                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * MathHelper.TwoPi / (float)num226), default) + Player.Center;
                                Vector2 vector7 = vector6 - Player.Center;
                                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, (int)CalamityDusts.Brimstone, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default, 1.4f);
                                Main.dust[num228].noGravity = true;
                                Main.dust[num228].noLight = true;
                                Main.dust[num228].velocity = vector7;
                            }
                        }
                    }
                }
                if (tarraMelee && !Player.HasCooldown(Cooldowns.TarragonCloak.ID) && !tarragonCloak)
                {
                    if (Player.whoAmI == Main.myPlayer)
                    {
                        Player.AddBuff(ModContent.BuffType<Buffs.StatBuffs.TarragonCloak>(), 602, false);
                    }
                }
                if (bloodflareRanged && !Player.HasCooldown(BloodflareRangedSet.ID))
                {
                    if (Player.whoAmI == Main.myPlayer)
                        Player.AddCooldown(BloodflareRangedSet.ID, 1800);

                    SoundEngine.PlaySound(BloodflareHeadRanged.ActivationSound, Player.Center);
                    for (int d = 0; d < 64; d++)
                    {
                        int dust = Dust.NewDust(new Vector2(Player.position.X, Player.position.Y + 16f), Player.width, Player.height - 16, (int)CalamityDusts.Phantoplasm, 0f, 0f, 0, default, 1f);
                        Main.dust[dust].velocity *= 3f;
                        Main.dust[dust].scale *= 1.15f;
                    }
                    int dustAmt = 36;
                    for (int d = 0; d < dustAmt; d++)
                    {
                        Vector2 source = Vector2.Normalize(Player.velocity) * new Vector2((float)Player.width / 2f, (float)Player.height) * 0.75f;
                        source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Player.Center;
                        Vector2 dustVel = source - Player.Center;
                        int phanto = Dust.NewDust(source + dustVel, 0, 0, (int)CalamityDusts.Phantoplasm, dustVel.X * 1.5f, dustVel.Y * 1.5f, 100, default, 1.4f);
                        Main.dust[phanto].noGravity = true;
                        Main.dust[phanto].noLight = true;
                        Main.dust[phanto].velocity = dustVel;
                    }
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(Player.velocity.X, Player.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;

                    int damage = (int)(Player.GetTotalDamage<RangedDamageClass>().ApplyTo(300f));
                    if (Player.whoAmI == Main.myPlayer)
                    {
                        var source = Player.GetSource_Misc("1");
                        for (int i = 0; i < 8; i++)
                        {
                            float ai1 = Main.rand.NextFloat() + 0.5f;
                            float randomSpeed = (float)Main.rand.Next(1, 7);
                            float randomSpeed2 = (float)Main.rand.Next(1, 7);
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            int soul = Projectile.NewProjectile(source, Player.Center.X, Player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f) + randomSpeed, ModContent.ProjectileType<BloodflareSoul>(), damage, 0f, Player.whoAmI, 0f, ai1);
                            if (soul.WithinBounds(Main.maxProjectiles))
                                Main.projectile[soul].DamageType = DamageClass.Generic;
                            int soul2 = Projectile.NewProjectile(source, Player.Center.X, Player.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f) + randomSpeed2, ModContent.ProjectileType<BloodflareSoul>(), damage, 0f, Player.whoAmI, 0f, ai1);
                            if (soul2.WithinBounds(Main.maxProjectiles))
                                Main.projectile[soul2].DamageType = DamageClass.Generic;
                        }
                    }
                }
                if (omegaBlueSet && !Player.HasCooldown(OmegaBlue.ID))
                {
                    if (Player.whoAmI == Main.myPlayer)
                    {
                        Player.AddBuff(ModContent.BuffType<AbyssalMadness>(), 300, false);
                    }
                    Player.AddCooldown(OmegaBlue.ID, 1800);
                    SoundEngine.PlaySound(OmegaBlueHelmet.ActivationSound, Player.Center);
                    for (int i = 0; i < 66; i++)
                    {
                        int d = Dust.NewDust(Player.position, Player.width, Player.height, 20, 0, 0, 100, Color.Transparent, 2.6f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].fadeIn = 1f;
                        Main.dust[d].velocity *= 6.6f;
                    }
                }
                if (dsSetBonus)
                {
                    SoundEngine.PlaySound(DemonshadeHelm.ActivationSound, Player.Center);
                    for (int num502 = 0; num502 < 36; num502++)
                    {
                        int dust = Dust.NewDust(new Vector2(Player.position.X, Player.position.Y + 16f), Player.width, Player.height - 16, (int)CalamityDusts.Brimstone, 0f, 0f, 0, default, 1f);
                        Main.dust[dust].velocity *= 3f;
                        Main.dust[dust].scale *= 1.15f;
                    }
                    int num226 = 36;
                    for (int num227 = 0; num227 < num226; num227++)
                    {
                        Vector2 vector6 = Vector2.Normalize(Player.velocity) * new Vector2((float)Player.width / 2f, (float)Player.height) * 0.75f;
                        vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * MathHelper.TwoPi / (float)num226), default) + Player.Center;
                        Vector2 vector7 = vector6 - Player.Center;
                        int num228 = Dust.NewDust(vector6 + vector7, 0, 0, (int)CalamityDusts.Brimstone, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default, 1.4f);
                        Main.dust[num228].noGravity = true;
                        Main.dust[num228].noLight = true;
                        Main.dust[num228].velocity = vector7;
                    }
                    if (Player.whoAmI == Main.myPlayer)
                    {
                        Player.AddBuff(ModContent.BuffType<Enraged>(), 600, false);
                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int l = 0; l < Main.maxNPCs; l++)
                        {
                            NPC npc = Main.npc[l];
                            if (npc.active && !npc.friendly && !npc.dontTakeDamage && Vector2.Distance(Player.Center, npc.Center) <= 3000f)
                            {
                                npc.AddBuff(ModContent.BuffType<Enraged>(), 600, false);
                            }
                        }
                    }
                }
                if (plagueReaper && !Player.HasCooldown(PlagueBlackout.ID))
                {
                    SoundEngine.PlaySound(PlagueReaperMask.ActivationSound, Player.Center);
                    Player.AddCooldown(PlagueBlackout.ID, 1800);
                }
                if (forbiddenCirclet && forbiddenCooldown <= 0)
                {
                    forbiddenCooldown = 45;
                    int stormMana = (int)(ForbiddenCirclet.manaCost * Player.manaCost);
                    if (Player.statMana < stormMana)
                    {
                        if (Player.manaFlower)
                        {
                            Player.QuickMana();
                        }
                    }
                    if (Player.statMana >= stormMana && !Player.silence)
                    {
                        var source = Player.GetSource_Misc("1");
                        Player.manaRegenDelay = (int)Player.maxRegenDelay;
                        Player.statMana -= stormMana;

                        // To compute Forbidden Circlet tornado damage, create a fake stat modifier on the spot which combines both classes.
                        StatModifier forbidden = Player.GetTotalDamage<SummonDamageClass>().CombineWith(Player.GetDamage<RogueDamageClass>());
                        int damage = (int)forbidden.ApplyTo(ForbiddenCirclet.tornadoBaseDmg);
                        float kBack = Player.GetTotalKnockback<SummonDamageClass>().ApplyTo(ForbiddenCirclet.tornadoBaseKB);

                        if (Player.whoAmI == Main.myPlayer)
                        {
                            int mark = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<CircletMark>(), damage, kBack, Player.whoAmI);
                            if (mark.WithinBounds(Main.maxProjectiles))
                                Main.projectile[mark].DamageType = DamageClass.Generic;
                        }
                    }
                }
                if (prismaticSet && !Player.HasCooldown(PrismaticLaser.ID) && prismaticLasers <= 0)
                    prismaticLasers = CalamityUtils.SecondsToFrames(35f);
            }
            if (CalamityKeybinds.AstralArcanumUIHotkey.JustPressed && astralArcanum && !areThereAnyDamnBosses)
            {
                AstralArcanumUI.Toggle();
            }
            if (CalamityKeybinds.AstralTeleportHotKey.JustPressed)
            {
                if (celestialJewel && !areThereAnyDamnBosses)
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Player.TeleportationPotion();
                        SoundEngine.PlaySound(SoundID.Item6, Player.position);
                    }
                    else if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI == Main.myPlayer)
                    {
                        NetMessage.SendData(MessageID.RequestTeleportationByServer, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
            }
            if (CalamityKeybinds.AegisHotKey.JustPressed)
            {
                if (elysianAegis && !Player.mount.Active)
                {
                    elysianGuard = !elysianGuard;
                }
            }

            // Trigger for pressing the God Slayer dash key
            if (CalamityKeybinds.GodSlayerDashHotKey.JustPressed)
            {
                if (godSlayer && (Player.controlUp || Player.controlDown || Player.controlLeft || Player.controlRight) && !Player.pulley && Player.grappling[0] == -1 && !Player.tongued && !Player.mount.Active && !Player.HasCooldown(GodSlayerDash.ID) && Player.dashDelay == 0)
                {
                    godSlayerDashHotKeyPressed = true;
                }
            }

            // Trigger for pressing the Rage hotkey.
            if (CalamityKeybinds.RageHotKey.JustPressed)
            {
                // Gael's Greatsword replaces Rage Mode with an uber skull attack
                if (gaelRageAttackCooldown == 0 && Player.ActiveItem().type == ModContent.ItemType<GaelsGreatsword>() && rage > 0f)
                {
                    SoundEngine.PlaySound(SilvaHeadSummon.DispelSound, Player.Center);

                    for (int i = 0; i < 3; i++)
                        Dust.NewDust(Player.position, 120, 120, 218, 0f, 0f, 100, default, 1.5f);
                    for (int i = 0; i < 30; i++)
                    {
                        float angle = MathHelper.TwoPi * i / 30f;
                        int dustIndex = Dust.NewDust(Player.position, 120, 120, 218, 0f, 0f, 0, default, 2f);
                        Main.dust[dustIndex].noGravity = true;
                        Main.dust[dustIndex].velocity *= 4f;
                        dustIndex = Dust.NewDust(Player.position, 120, 120, 218, 0f, 0f, 100, default, 1f);
                        Main.dust[dustIndex].velocity *= 2.25f;
                        Main.dust[dustIndex].noGravity = true;
                        Dust.NewDust(Player.Center + angle.ToRotationVector2() * 160f, 0, 0, 218, 0f, 0f, 100, default, 1f);
                    }

                    var source = new ProjectileSource_GaelsGreatswordRage(Player);
                    float rageRatio = rage / rageMax;
                    float baseDamage = rageRatio * GaelsGreatsword.SkullsplosionDamageMultiplier * GaelsGreatsword.BaseDamage;
                    int damage = (int)Player.GetTotalDamage<MeleeDamageClass>().ApplyTo(baseDamage);
                    float skullCount = 20f;
                    float skullSpeed = 12f;
                    for (float i = 0; i < skullCount; i += 1f)
                    {
                        float angle = MathHelper.TwoPi * i / skullCount;
                        Vector2 initialVelocity = angle.ToRotationVector2().RotatedByRandom(MathHelper.ToRadians(12f)) * skullSpeed * new Vector2(0.82f, 1.5f) *
                            Main.rand.NextFloat(0.8f, 1.2f) * (i < skullCount / 2  ? 0.25f : 1f);
                        int projectileIndex = Projectile.NewProjectile(source, Player.Center + initialVelocity * 3f, initialVelocity, ModContent.ProjectileType<GaelSkull2>(), damage, 2f, Player.whoAmI);
                        Main.projectile[projectileIndex].tileCollide = false;
                        Main.projectile[projectileIndex].localAI[1] = (Main.projectile[projectileIndex].velocity.Y < 0f).ToInt();
                        if (projectileIndex.WithinBounds(Main.maxProjectiles))
                            Main.projectile[projectileIndex].DamageType = DamageClass.Generic;
                    }

                    // Remove all rage when the special attack is used, and apply the cooldown.
                    rage = 0f;
                    gaelRageAttackCooldown = CalamityUtils.SecondsToFrames(GaelsGreatsword.SkullsplosionCooldownSeconds);
                }

                // Activating Rage Mode
                if (rage >= rageMax && !rageModeActive)
                {
                    // Rage duration isn't calculated here because the buff keeps itself alive automatically as long as the player has Rage left.
                    Player.AddBuff(ModContent.BuffType<RageMode>(), 2);

                    // Play Rage Activation sound
                    if (Player.whoAmI == Main.myPlayer)
                        SoundEngine.PlaySound(RageActivationSound);

                    // TODO -- Rage should provide glowy red afterimages to the player for the duration.
                    // If Shattered Community is equipped, the afterimages are magenta instead.
                    int rageDustID = 235;
                    int dustCount = 132;
                    float minSpeed = 4f;
                    float maxSpeed = 11f;
                    for (int i = 0; i < dustCount; ++i)
                    {
                        float speed = (float)Math.Sqrt(Main.rand.NextFloat(minSpeed * minSpeed, maxSpeed * maxSpeed));
                        Vector2 dustVel = Main.rand.NextVector2Unit() * speed;
                        Dust d = Dust.NewDustPerfect(Player.Center, rageDustID, dustVel);
                        d.noGravity = !Main.rand.NextBool(4); // 25% of dust has gravity
                        d.noLight = false;
                        d.scale = Main.rand.NextFloat(0.9f, 2.1f);
                    }
                }
            }

            // Trigger for pressing the Adrenaline hotkey.
            if (CalamityKeybinds.AdrenalineHotKey.JustPressed && AdrenalineEnabled)
            {
                if (adrenaline == adrenalineMax && !adrenalineModeActive)
                {
                    Player.AddBuff(ModContent.BuffType<AdrenalineMode>(), AdrenalineDuration);

                    SoundStyle ActivationSound = draedonsHeart ? NanomachinesActivationSound : AdrenalineActivationSound;

                    // Play Adrenaline Activation sound
                    if (Player.whoAmI == Main.myPlayer)
                        SoundEngine.PlaySound(ActivationSound);

                    // TODO -- Adrenaline should provide bright green vibrating afterimages on the player for the duration.
                    int dustPerSegment = 96;

                    // Parametric segment 1: y = 3x + 120
                    Vector2 segmentOneStart = new Vector2(0f, -120f);
                    Vector2 segmentOneEnd = new Vector2(-48f, 24f);
                    Vector2 segmentOneIncrement = (segmentOneEnd - segmentOneStart) / dustPerSegment;

                    // Parametric segment 2: y = 0.5x
                    Vector2 segmentTwoStart = segmentOneEnd;
                    Vector2 segmentTwoEnd = new Vector2(48f, -24f);
                    Vector2 segmentTwoIncrement = (segmentTwoEnd - segmentTwoStart) / dustPerSegment;

                    // Parametric segment 3: y = 3x - 120
                    Vector2 segmentThreeStart = segmentTwoEnd;
                    Vector2 segmentThreeEnd = new Vector2(0f, 120f);
                    Vector2 segmentThreeIncrement = (segmentThreeEnd - segmentThreeStart) / dustPerSegment;

                    float maxDustVelSpread = 1.2f;
                    for (int i = 0; i < dustPerSegment; ++i)
                    {
                        bool electricity = Main.rand.NextBool(4);
                        int dustID = electricity ? 132 : DustID.TerraBlade;

                        float interpolant = i + 0.5f;
                        float spreadSpeed = Main.rand.NextFloat(0.5f, maxDustVelSpread);
                        if (electricity)
                            spreadSpeed *= 4f;

                        Vector2 segmentOnePos = Player.Center + segmentOneStart + segmentOneIncrement * interpolant;
                        Dust d = Dust.NewDustPerfect(segmentOnePos, dustID, Vector2.Zero);
                        if (electricity)
                            d.noGravity = false;
                        d.scale = Main.rand.NextFloat(0.8f, 1.4f);
                        d.velocity = Main.rand.NextVector2Unit() * spreadSpeed;

                        Vector2 segmentTwoPos = Player.Center + segmentTwoStart + segmentTwoIncrement * interpolant;
                        d = Dust.CloneDust(d);
                        d.position = segmentTwoPos;
                        d.scale = Main.rand.NextFloat(0.8f, 1.4f);
                        d.velocity = Main.rand.NextVector2Unit() * spreadSpeed;

                        Vector2 segmentThreePos = Player.Center + segmentThreeStart + segmentThreeIncrement * interpolant;
                        d = Dust.CloneDust(d);
                        d.position = segmentThreePos;
                        d.scale = Main.rand.NextFloat(0.8f, 1.4f);
                        d.velocity = Main.rand.NextVector2Unit() * spreadSpeed;
                    }
                }
            }


            bool mountCheck = true;
            if (Player.mount != null && Player.mount.Active)
                mountCheck = !Player.mount.BlockExtraJumps;
            bool canJump = (!Player.hasJumpOption_Cloud || !Player.canJumpAgain_Cloud) &&
            (!Player.hasJumpOption_Sandstorm || !Player.canJumpAgain_Sandstorm) &&
            (!Player.hasJumpOption_Blizzard || !Player.canJumpAgain_Blizzard) &&
            (!Player.hasJumpOption_Fart || !Player.canJumpAgain_Fart) &&
            (!Player.hasJumpOption_Sail || !Player.canJumpAgain_Sail) &&
            (!Player.hasJumpOption_Unicorn || !Player.canJumpAgain_Unicorn) &&
            CalamityUtils.CountHookProj() <= 0 && (Player.rocketTime == 0 || Player.wings > 0) && mountCheck;
            if (PlayerInput.Triggers.JustPressed.Jump && Player.position.Y != Player.oldPosition.Y && canJump)
            {
                if (statigelJump && jumpAgainStatigel)
                {
                    jumpAgainStatigel = false;
                    int offset = Player.height;
                    if (Player.gravDir == -1f)
                        offset = 0;
                    SoundEngine.PlaySound(SoundID.DoubleJump, Player.position);
                    Player.velocity.Y = -Player.jumpSpeed * Player.gravDir;
                    Player.jump = (int)(Player.jumpHeight * 1.25);
                    for (int d = 0; d < 30; ++d)
                    {
                        int goo = Dust.NewDust(new Vector2(Player.position.X, Player.position.Y + offset), Player.width, 12, 4, Player.velocity.X * 0.3f, Player.velocity.Y * 0.3f, 100, new Color(0, 80, 255, 100), 1.5f);
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
                    int offset = Player.height;
                    if (Player.gravDir == -1f)
                        offset = 0;
                    SoundEngine.PlaySound(SoundID.DoubleJump, Player.position);
                    Player.velocity.Y = -Player.jumpSpeed * Player.gravDir;
                    Player.jump = (int)(Player.jumpHeight * 1.5);
                    for (int d = 0; d < 30; ++d)
                    {
                        int sulfur = Dust.NewDust(new Vector2(Player.position.X, Player.position.Y + offset), Player.width, 12, 31, Player.velocity.X * 0.3f, Player.velocity.Y * 0.3f, 100, default, 1.5f);
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
                        var source = Player.GetSource_Misc("0");
                        int damage = (int)Player.GetTotalDamage<RogueDamageClass>().ApplyTo(20);
                        int bubble = Projectile.NewProjectile(source, new Vector2(Player.position.X, Player.position.Y + (Player.gravDir == -1f ? 20 : -20)), Vector2.Zero, ModContent.ProjectileType<SulphuricAcidBubbleFriendly>(), damage, 0f, Player.whoAmI, 1f, 0f);
                        if (bubble.WithinBounds(Main.maxProjectiles))
                            Main.projectile[bubble].DamageType = DamageClass.Generic;
                        sulphurBubbleCooldown = 20;
                    }
                }
            }
        }
        #endregion

        #region TeleportMethods
        public static Vector2? GetJunglePosition(Player player)
        {
			bool canSpawn = false;
            int teleportStartX = Abyss.AtLeftSideOfWorld ? (int)(Main.maxTilesX * 0.65) : (int)(Main.maxTilesX * 0.2);
            int teleportRangeX = (int)(Main.maxTilesX * 0.15);
            int teleportStartY = (int)Main.worldSurface - 75;
            int teleportRangeY = 50;

			Player.RandomTeleportationAttemptSettings settings = new Player.RandomTeleportationAttemptSettings
			{
				mostlySolidFloor = true,
				avoidAnyLiquid = true,
				avoidLava = true,
				avoidHurtTiles = true,
				avoidWalls = true,
				attemptsBeforeGivingUp = 1000,
				maximumFallDistanceFromOrignalPoint = 30
			};

			Vector2 vector = player.CheckForGoodTeleportationSpot(ref canSpawn, teleportStartX, teleportRangeX, teleportStartY, teleportRangeY, settings);

			if (canSpawn)
			{
				return (Vector2?)vector;
			}
            return null;
        }

        public static Vector2? GetUnderworldPosition(Player player)
        {
			bool canSpawn = false;
			int num = Main.maxTilesX / 2;
			int num2 = 100;
			int num3 = num2 / 2;
			int teleportStartY = Main.UnderworldLayer + 20;
			int teleportRangeY = 80;
			Player.RandomTeleportationAttemptSettings settings = new Player.RandomTeleportationAttemptSettings
			{
				mostlySolidFloor = true,
				avoidAnyLiquid = true,
				avoidLava = true,
				avoidHurtTiles = true,
				avoidWalls = true,
				attemptsBeforeGivingUp = 1000,
				maximumFallDistanceFromOrignalPoint = 30
			};

			Vector2 vector = player.CheckForGoodTeleportationSpot(ref canSpawn, num - num3, num2, teleportStartY, teleportRangeY, settings);
			if (!canSpawn)
				vector = player.CheckForGoodTeleportationSpot(ref canSpawn, num - num2, num3, teleportStartY, teleportRangeY, settings);

			if (!canSpawn)
				vector = player.CheckForGoodTeleportationSpot(ref canSpawn, num + num3, num3, teleportStartY, teleportRangeY, settings);

			if (canSpawn)
			{
				return (Vector2?)vector;
			}
            return null;
        }

        public static void ModTeleport(Player player, Vector2 pos, bool playSound = true, int style = TeleportationStyleID.RecallPotion)
        {
            bool postImmune = player.immune;
            int postImmuneTime = player.immuneTime;
			player.StopVanityActions(false);
            player.RemoveAllGrapplingHooks();
            player.Teleport(pos, style);
			if (Main.netMode == NetmodeID.Server)
				RemoteClient.CheckSection(player.whoAmI, player.Center);
			NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, (float)player.whoAmI, pos.X, pos.Y, style, 0, 0);
            player.velocity = Vector2.Zero;
            player.immune = postImmune;
            player.immuneTime = postImmuneTime;

            // Make some dust
            for (int index = 0; index < 100; ++index)
            {
                Main.dust[Dust.NewDust(player.position, player.width, player.height, 164, player.velocity.X * 0.2f, player.velocity.Y * 0.2f, 150, Color.Cyan, 1.2f)].velocity *= 0.5f;
            }
			Rectangle rect = player.getRect();
			int dustAmt = rect.Width * rect.Height / 5;
			for (int k = 0; k < dustAmt; k++)
			{
				int idx = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, 164);
				Main.dust[idx].scale = Main.rand.NextFloat(0.2f, 0.7f);
				if (k < 10)
					Main.dust[idx].scale += 0.25f;
				if (k < 5)
					Main.dust[idx].scale += 0.25f;
			}
			for (int k = 0; k < 50; k++)
			{
				int idx = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, 180);
				Main.dust[idx].noGravity = true;
				for (int i = 0; i < 5; i++)
				{
					if (Main.rand.NextBool(3))
						Main.dust[idx].velocity *= 0.75f;
				}
				if (Main.rand.NextBool(3))
				{
					Main.dust[idx].velocity *= 2f;
					Main.dust[idx].scale *= 1.2f;
				}
				if (Main.rand.NextBool(3))
				{
					Main.dust[idx].velocity *= 2f;
					Main.dust[idx].scale *= 1.2f;
				}
				if (Main.rand.NextBool(2))
				{
					Main.dust[idx].fadeIn = Main.rand.NextFloat(0.75f, 1f);
					Main.dust[idx].scale = Main.rand.NextFloat(0.25f, 0.75f);
				}
				Main.dust[idx].scale *= 0.8f;
			}

			if (playSound)
				SoundEngine.PlaySound(SoundID.Item6, player.Center);
        }
        #endregion

        #region UpdateEquips
        public override void UpdateEquips()
        {
            // TODO -- why is boss health bar code in Player.UpdateEquips and not a ModSystem
            CalamityConfig.Instance.BossHealthBarExtraInfo = shouldDrawSmallText;

            if (CalamityConfig.Instance.FasterTilePlacement)
            {
                // Increase tile placement speed to speed up early game a bit and make building more fun
                Player.tileSpeed += 0.5f;

                // Increase wall placement speed to speed up early game a bit and make building more fun
                Player.wallSpeed += 0.5f;
            }

            // Takes the movement speed bonus and uses it to increase run speed
            float accRunSpeedMin = Player.accRunSpeed * 0.5f;
            Player.accRunSpeed += Player.accRunSpeed * moveSpeedBonus * 0.2f;
            if (Player.accRunSpeed < accRunSpeedMin)
                Player.accRunSpeed = accRunSpeedMin;

            #region Melee Speed for Projectile Melee Weapons
            float meleeSpeedMult = 0f;
            if (community)
            {
                float BoostAtZeroBosses = 0.05f;
                float BoostPostYharon = 0.2f;
                float floatTypeBoost = MathHelper.Lerp(BoostAtZeroBosses, BoostPostYharon, TheCommunity.CalculatePower());
                meleeSpeedMult += floatTypeBoost * 0.25f;
            }

            // Nerfs the effectiveness of Beetle Scale Mail.
            if (Player.beetleOffense && Player.beetleOrbs > 0)
                meleeSpeedMult -= 0.1f * Player.beetleOrbs;

            if (GemTechSet && GemTechState.IsYellowGemActive)
                meleeSpeedMult += GemTechHeadgear.MeleeSpeedBoost;

            Player.GetAttackSpeed<MeleeDamageClass>() += meleeSpeedMult;
            #endregion

            if (snowman)
            {
                if (Player.whoAmI == Main.myPlayer && !snowmanNoseless)
                    Player.AddBuff(ModContent.BuffType<PopoBuff>(), 60, true);
            }
            if (abyssalDivingSuit)
            {
                Player.AddBuff(ModContent.BuffType<AbyssalDivingSuitBuff>(), 60, true);
                if (Player.whoAmI == Main.myPlayer && !Player.HasCooldown(DivingPlatesBroken.ID))
                {
                    Player.AddBuff(ModContent.BuffType<AbyssalDivingSuitPlates>(), 2);
                }

                if (Player.whoAmI == Main.myPlayer && Player.active && abyssalDivingSuitPlateHits < 3)
                {
                    if (!Player.HasCooldown(DivingPlatesBreaking.ID))
                    {
                        CooldownInstance plates = Player.AddCooldown(DivingPlatesBreaking.ID, 3);
                        plates.timeLeft = abyssalDivingSuitPlateHits;
                    }
                    else
                    {
                        CooldownInstance plates = cooldowns[DivingPlatesBreaking.ID];
                        plates.timeLeft = abyssalDivingSuitPlateHits;
                    }
                }
            }

            if (aquaticHeart)
            {
                Player.AddBuff(ModContent.BuffType<AquaticHeartBuff>(), 60, true);
            }
            if (aquaticHeart && NPC.downedBoss3)
            {
                if (Player.whoAmI == Main.myPlayer && !Player.HasCooldown(AquaticHeartIceShield.ID))
                {
                    Player.AddBuff(ModContent.BuffType<IceShieldBuff>(), 2);
                }
            }
            if (profanedCrystal)
            {
                Player.AddBuff(ModContent.BuffType<ProfanedCrystalBuff>(), 60, true);
            }
        }
        #endregion

        #region PreUpdate
        public override void PreUpdate()
        {
            // Reset the Calamity shader.
            CalamityFireDyeShader = null;

            if (HasCustomDash && UsedDash.IsOmnidirectional)
                Player.maxFallSpeed = 50f;

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

            for (int i = 0; i < Player.dye.Length; i++)
            {
                if (Player.dye[i].type == ModContent.ItemType<ProfanedMoonlightDye>())
                {
                    GameShaders.Armor.GetSecondaryShader(Player.dye[i].dye, Player)?.UseColor(GetCurrentMoonlightDyeColor());
                }
            }

            // Syncing mouse controls
            if (Main.myPlayer == Player.whoAmI)
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
                if (mouseRotationListener && Math.Abs((mouseWorld - Player.MountedCenter).ToRotation() - (oldMouseWorld - Player.MountedCenter).ToRotation()) > 0.15f)
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
            if (Player.ZoneDesert && (ZoneAstral || areThereAnyDamnBosses) && Player.HasBuff(BuffID.WindPushed))
            {
                Player.ClearBuff(BuffID.WindPushed);
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

                if (Player.controlLeft)
                {
                    Player.velocity.X = -speed;
                    Player.direction = -1;
                }
                else if (Player.controlRight)
                {
                    Player.velocity.X = speed;
                    Player.direction = 1;
                }
                else
                    Player.velocity.X = 0f;

                if (Player.controlUp || Player.controlJump)
                    Player.velocity.Y = -speed;

                else if (Player.controlDown)
                {
                    Player.velocity.Y = speed;
                    if (Collision.TileCollision(Player.position, Player.velocity, Player.width, Player.height, true, false, (int)Player.gravDir).Y == 0f)
                        Player.velocity.Y = 0.5f;
                }
                else
                    Player.velocity.Y = 0f;

                if (CalamityKeybinds.ExoChairSlowdownHotkey.Current)
                    Player.velocity *= 0.5f;
            }
        }
        #endregion

        #region PostUpdateBuffs
        public override void PostUpdateBuffs()
        {
            if (Player.whoAmI == Main.myPlayer && CalamityConfig.Instance.VanillaCooldownDisplay)
            {
                // Add a cooldown display for potion sickness if the player has the vanilla counter ticking
                if (Player.potionDelay > 0 && !Player.HasCooldown(PotionSickness.ID))
                    Player.AddCooldown(PotionSickness.ID, Player.potionDelay, false);

                // Add a cooldown display for chaos state if the player has the vanilla counter ticking
                // This will make the cooldown look like vanilla Rod of Discord, as it wasn't applied by either Normality Relocator or Spectral Veil
                if (Player.chaosState && !Player.HasCooldown(ChaosState.ID))
                {
                    for (int l = 0; l < Player.MaxBuffs; l++)
                        if (Player.buffType[l] == BuffID.ChaosState)
                        {
                            Player.AddCooldown(ChaosState.ID, Player.buffTime[l], false);
                            break;
                        }
                }
            }

            // Add a cooldown display for the vanilla lifesteal cooldown, which is active when negative
            if (Player.whoAmI == Main.myPlayer && Player.lifeSteal < 0f)
            {
                float duration = Player.lifeSteal;
                float baseCooldown = Main.expertMode ? 0.5f : 0.6f;
                float lifeStealNerf = BossRushEvent.BossRushActive ? 0.3f : CalamityWorld.death ? 0.25f : CalamityWorld.revenge ? 0.2f : Main.expertMode ? 0.15f : 0.1f;
                duration /= baseCooldown - lifeStealNerf;
                duration *= -1f;
                if (!Player.HasCooldown(LifeSteal.ID) || (cooldowns[LifeSteal.ID].duration < (int)duration))
                    Player.AddCooldown(LifeSteal.ID, (int)duration);
            }

            ForceVariousEffects();
        }
        #endregion

        #region PostUpdateEquips
        public override void PostUpdateEquips()
        {
            // True melee damage from various vanilla equipment placed here.

            // Titan Glove and ALL upgrades
            if (Player.kbGlove)
                Player.GetDamage<TrueMeleeDamageClass>() += 0.1f;

            ForceVariousEffects();
            BaseIdleHoldoutProjectile.CheckForEveryHoldout(Player);
        }
        #endregion

        #region PostUpdate
        #region Shop Restrictions
        public override bool CanSellItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (item.type == ModContent.ItemType<ProfanedSoulCrystal>())
                return DownedBossSystem.downedSCal; //no easy moneycoins for post doggo/yhar
            return base.CanSellItem(vendor, shopInventory, item);
        }

        #endregion

        public override void PostUpdateRunSpeeds()
        {
            #region SpeedBoosts
            if (!Player.mount.Active)
            {
                float runAccMult = 1f +
                    (shadowSpeed ? 0.5f : 0f) +
                    (stressPills ? 0.05f : 0f) +
                    ((abyssalDivingSuit && Player.IsUnderwater()) ? 0.05f : 0f) +
                    (aquaticHeartWaterBuff ? 0.15f : 0f) +
                    ((frostFlare && Player.statLife < (int)(Player.statLifeMax2 * 0.25)) ? 0.15f : 0f) +
                    (dragonScales ? 0.1f : 0f) +
                    (kamiBoost ? KamiBuff.RunAccelerationBoost : 0f) +
                    (CobaltSet ? CobaltArmorSetChange.SpeedBoostSetBonusPercentage * 0.01f : 0f) +
                    (silvaSet ? 0.05f : 0f) +
                    (blueCandle ? 0.05f : 0f) +
                    (planarSpeedBoost > 0 ? (0.01f * planarSpeedBoost) : 0f) +
                    ((deepDiver && Player.IsUnderwater()) ? 0.15f : 0f);

                float runSpeedMult = 1f +
                    (shadowSpeed ? 0.5f : 0f) +
                    (stressPills ? 0.05f : 0f) +
                    ((abyssalDivingSuit && Player.IsUnderwater()) ? 0.05f : 0f) +
                    (aquaticHeartWaterBuff ? 0.15f : 0f) +
                    ((frostFlare && Player.statLife < (int)(Player.statLifeMax2 * 0.25)) ? 0.15f : 0f) +
                    (dragonScales ? 0.1f : 0f) +
                    (kamiBoost ? KamiBuff.RunSpeedBoost : 0f) +
                    (CobaltSet ? CobaltArmorSetChange.SpeedBoostSetBonusPercentage * 0.01f : 0f) +
                    (silvaSet ? 0.05f : 0f) +
                    (planarSpeedBoost > 0 ? (0.01f * planarSpeedBoost) : 0f) +
                    ((deepDiver && Player.IsUnderwater()) ? 0.15f : 0f);

                if ((Player.slippy || Player.slippy2) && Player.iceSkate)
                    runAccMult *= 0.6f;

                if (momentumCapacitorTime > 0)
                {
                    runAccMult += momentumCapacitorBoost * 0.25f;
                    runSpeedMult += momentumCapacitorBoost;

                    // Sputters out chaotically when you let go of the button
                    if (momentumCapacitorTime < MomentumCapacitor.TotalFadeTime - 3)
                        momentumCapacitorBoost *= Main.rand.NextFloat(0.955f, 0.99f);
                }
                // If the timer has hit zero, or you aren't using Momentum Capacitor, you get nothing.
                else
                    momentumCapacitorBoost = 0f;

                Player.runAcceleration *= runAccMult;
                Player.maxRunSpeed *= runSpeedMult;
            }
            #endregion

            #region DashEffects
            if (!string.IsNullOrEmpty(DeferredDashID))
            {
                DashID = DeferredDashID;
                DeferredDashID = string.Empty;
            }

            if (Player.pulley && HasCustomDash)
            {
                ModDashMovement();
            }
            else if (Player.grappling[0] == -1 && !Player.tongued)
            {
                ModHorizontalMovement();

                if (HasCustomDash)
                    ModDashMovement();
            }
            #endregion
        }
        #endregion

        #region Dodges
        private bool HandleDodges()
        {
            if (Player.whoAmI != Main.myPlayer || disableAllDodges)
                return false;

            if (spectralVeil && spectralVeilImmunity > 0)
            {
                SpectralVeilDodge();
                return true;
            }

            if (HandleDashDodges())
                return true;

            // Mirror evades do not work if the global dodge cooldown is active. This cooldown can be triggered by either mirror.
            if (!Player.HasCooldown(GlobalDodge.ID))
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
            Player.GiveIFrames(spectralVeilImmunity, true); //Set immunity before setting this variable to 0
            rogueStealth = rogueStealthMax;
            spectralVeilImmunity = 0;

            Vector2 sVeilDustDir = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
            sVeilDustDir.Normalize();
            sVeilDustDir *= 0.5f;

            for (int j = 0; j < 20; j++)
            {
                int sVeilDustIndex1 = Dust.NewDust(Player.Center, 1, 1, 21, sVeilDustDir.X * j, sVeilDustDir.Y * j);
                int sVeilDustIndex2 = Dust.NewDust(Player.Center, 1, 1, 21, -sVeilDustDir.X * j, -sVeilDustDir.Y * j);
                Main.dust[sVeilDustIndex1].noGravity = false;
                Main.dust[sVeilDustIndex1].noLight = false;
                Main.dust[sVeilDustIndex2].noGravity = false;
                Main.dust[sVeilDustIndex2].noLight = false;
            }

            SoundEngine.PlaySound(SilvaHeadSummon.DispelSound, Player.Center);

            NetMessage.SendData(MessageID.Dodge, -1, -1, null, Player.whoAmI, 1f, 0f, 0f, 0, 0, 0);
        }

        private void GodSlayerDodge()
        {
            Player.GiveIFrames(Player.longInvince ? 100 : 60, true);
            SoundEngine.PlaySound(SoundID.Item67, Player.position);

            for (int j = 0; j < 30; j++)
            {
                int num = Dust.NewDust(Player.position, Player.width, Player.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                Dust dust = Main.dust[num];
                dust.position.X += Main.rand.Next(-20, 21);
                dust.position.Y += Main.rand.Next(-20, 21);
                dust.velocity *= 0.4f;
                dust.scale *= 1f + Main.rand.Next(40) * 0.01f;
                dust.shader = GameShaders.Armor.GetSecondaryShader(Player.cWaist, Player);
                if (Main.rand.NextBool(2))
                {
                    dust.scale *= 1f + Main.rand.Next(40) * 0.01f;
                    dust.noGravity = true;
                }
            }

            NetMessage.SendData(MessageID.Dodge, -1, -1, null, Player.whoAmI, 1f, 0f, 0f, 0, 0, 0);
        }

        private void CounterScarfDodge()
        {
            if (evasionScarf)
            {
                int duration = CalamityUtils.SecondsToFrames(30);
                Player.AddCooldown(Cooldowns.EvasionScarf.ID, duration);
            }
            else
            {
                int duration = CalamityUtils.SecondsToFrames(30);
                Player.AddCooldown(Cooldowns.CounterScarf.ID, duration);
            }

            Player.GiveIFrames(Player.longInvince ? 100 : 60, true);

            for (int j = 0; j < 100; j++)
            {
                int num = Dust.NewDust(Player.position, Player.width, Player.height, 235, 0f, 0f, 100, default, 2f);
                Dust dust = Main.dust[num];
                dust.position.X += Main.rand.Next(-20, 21);
                dust.position.Y += Main.rand.Next(-20, 21);
                dust.velocity *= 0.4f;
                dust.scale *= 1f + Main.rand.Next(40) * 0.01f;
                dust.shader = GameShaders.Armor.GetSecondaryShader(Player.cWaist, Player);
                if (Main.rand.NextBool(2))
                {
                    dust.scale *= 1f + Main.rand.Next(40) * 0.01f;
                    dust.noGravity = true;
                }
            }

            NetMessage.SendData(MessageID.Dodge, -1, -1, null, Player.whoAmI, 1f, 0f, 0f, 0, 0, 0);
        }

        public void AbyssMirrorEvade()
        {
            if (Player.whoAmI == Main.myPlayer && abyssalMirror && !eclipseMirror)
            {
                Player.AddCooldown(GlobalDodge.ID, BalancingConstants.MirrorDodgeCooldown, true, "abyssmirror");

                // TODO -- why is this here?
                Player.noKnockback = true;

                Player.GiveIFrames(Player.longInvince ? 100 : 60, true);
                rogueStealth += 0.5f;
                SoundEngine.PlaySound(SilvaHeadSummon.ActivationSound, Player.Center);

                var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<AbyssalMirror>()));
                for (int i = 0; i < 10; i++)
                {
                    int damage = (int)Player.GetTotalDamage<RogueDamageClass>().ApplyTo(55);
                    int lumenyl = Projectile.NewProjectile(source, Player.Center.X, Player.Center.Y, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), ModContent.ProjectileType<AbyssalMirrorProjectile>(), damage, 0, Player.whoAmI);
                    Main.projectile[lumenyl].rotation = Main.rand.NextFloat(0, 360);
                    Main.projectile[lumenyl].frame = Main.rand.Next(0, 4);
                    if (lumenyl.WithinBounds(Main.maxProjectiles))
                        Main.projectile[lumenyl].DamageType = DamageClass.Generic;
                }

                // TODO -- Calamity dodges should probably not send a vanilla dodge packet considering that causes Tabi dust
                if (Player.whoAmI == Main.myPlayer)
                {
                    NetMessage.SendData(MessageID.Dodge, -1, -1, null, Player.whoAmI, 1f, 0f, 0f, 0, 0, 0);
                }
            }
        }

        public void EclipseMirrorEvade()
        {
            if (Player.whoAmI == Main.myPlayer && eclipseMirror)
            {
                Player.AddCooldown(GlobalDodge.ID, BalancingConstants.MirrorDodgeCooldown, true, "eclipsemirror");

                // TODO -- why is this here?
                Player.noKnockback = true;

                Player.GiveIFrames(Player.longInvince ? 100 : 60, true);
                rogueStealth = rogueStealthMax;
                SoundEngine.PlaySound(SoundID.Item68, Player.Center);

                var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<EclipseMirror>()));
                int damage = (int)Player.GetTotalDamage<RogueDamageClass>().ApplyTo(2750);
                int eclipse = Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ModContent.ProjectileType<EclipseMirrorBurst>(), damage, 0, Player.whoAmI);
                if (eclipse.WithinBounds(Main.maxProjectiles))
                    Main.projectile[eclipse].DamageType = DamageClass.Generic;

                // TODO -- Calamity dodges should probably not send a vanilla dodge packet considering that causes Tabi dust
                if (Player.whoAmI == Main.myPlayer)
                {
                    NetMessage.SendData(MessageID.Dodge, -1, -1, null, Player.whoAmI, 1f, 0f, 0f, 0, 0, 0);
                }
            }
        }
        #endregion

        #region Pre Kill
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            PopupGUIManager.SuspendAll();

            // Determine which minions need to be respawned.
            if (Main.myPlayer == Player.whoAmI)
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
                    if (projectile.type != endoHydraHeadType || projectile.owner != Player.whoAmI || !projectile.active)
                        continue;
                    endoHydraHeadCount++;
                }

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile projectile = Main.projectile[i];
                    if ((projectile.minionSlots <= 0f && !CalamityLists.ZeroMinionSlotExceptionList.Contains(projectile.type)) || !projectile.minion ||
                        projectile.owner != Player.whoAmI || !projectile.active || CalamityLists.MinionsToNotResurrectList.Contains(projectile.type))
                        continue;

                    DeadMinionProperties deadMinionProperties;

                    // Handle unique edge-cases in terms of summoning logic.
                    if (projectile.type == endoHydraBodyType)
                        deadMinionProperties = new DeadEndoHydraProperties(endoHydraHeadCount, projectile.originalDamage, projectile.damage, projectile.knockBack);
                    else if (projectile.type == endoCooperType)
                        deadMinionProperties = new DeadEndoCooperProperties((int)projectile.ai[0], projectile.minionSlots, projectile.originalDamage, projectile.damage, projectile.knockBack);
                    else
                    {
                        float[] aiToCopy = projectile.ai;

                        // If blacklisted from copying AI state values, zero out the AI values to feed to the copy.
                        if (CalamityLists.DontCopyOriginalMinionAIList.Contains(projectile.type))
                            aiToCopy = new float[aiToCopy.Length];
                        deadMinionProperties = new DeadMinionProperties(projectile.type, projectile.minionSlots, projectile.originalDamage, projectile.damage, projectile.knockBack, aiToCopy);
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
                        Dust dust = Dust.NewDustPerfect(Player.Center + Utils.NextVector2Circular(Main.rand, 60f, 90f), 133);
                        dust.velocity = Utils.NextVector2Circular(Main.rand, 4f, 4f);
                        dust.noGravity = true;
                        dust.scale = Main.rand.NextFloat(1.2f, 1.35f);
                    }

                    for (int i = 0; i < 3; i++)
                        Utils.PoofOfSmoke(Player.Center + Utils.NextVector2Circular(Main.rand, 20f, 30f));
                }
            }

            if (hInferno)
            {
                for (int x = 0; x < Main.maxNPCs; x++)
                {
                    if (Main.npc[x].active && Main.npc[x].type == ModContent.NPCType<Providence>())
                        Main.npc[x].active = false;
                }
            }

            if (nCore && !Player.HasCooldown(Cooldowns.NebulousCore.ID))
            {
                SoundEngine.PlaySound(SoundID.Item67, Player.position);

                for (int j = 0; j < 50; j++)
                {
                    int num = Dust.NewDust(Player.position, Player.width, Player.height, 173, 0f, 0f, 100, default, 2f);
                    Dust dust = Main.dust[num];
                    dust.position.X += Main.rand.Next(-20, 21);
                    dust.position.Y += Main.rand.Next(-20, 21);
                    dust.velocity *= 0.9f;
                    dust.scale *= 1f + Main.rand.Next(40) * 0.01f;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(Player.cWaist, Player);
                    if (Main.rand.NextBool(2))
                        dust.scale *= 1f + Main.rand.Next(40) * 0.01f;
                }

                Player.statLife += 100;
                Player.HealEffect(100);

                if (Player.statLife > Player.statLifeMax2)
                    Player.statLife = Player.statLifeMax2;

                Player.AddCooldown(Cooldowns.NebulousCore.ID, CalamityUtils.SecondsToFrames(90));
                return false;
            }

            if (DashID == GodslayerArmorDash.ID && Player.dashDelay < 0)
            {
                if (Player.statLife < 1)
                    Player.statLife = 1;

                return false;
            }

            if (silvaSet && silvaCountdown > 0)
            {
                if (silvaCountdown == silvaReviveDuration && !hasSilvaEffect)
                {
                    SoundEngine.PlaySound(SilvaHeadSummon.ActivationSound, Player.position);

                    Player.AddBuff(ModContent.BuffType<SilvaRevival>(), silvaReviveDuration);

                    if (silvaWings)
                    {
                        Player.statLife += Player.statLifeMax2 / 2;
                        Player.HealEffect(Player.statLifeMax2 / 2);

                        if (Player.statLife > Player.statLifeMax2)
                            Player.statLife = Player.statLifeMax2;
                    }
                }

                hasSilvaEffect = true;

                if (Player.statLife < 1)
                    Player.statLife = 1;

                return false;
            }

            if (permafrostsConcoction && !Player.HasCooldown(PermafrostConcoction.ID))
            {
                Player.AddCooldown(PermafrostConcoction.ID, CalamityUtils.SecondsToFrames(180));
                Player.AddBuff(ModContent.BuffType<Encased>(), CalamityUtils.SecondsToFrames(3f));

                Player.statLife = Player.statLifeMax2 * 3 / 10;

                SoundEngine.PlaySound(SoundID.Item92, Player.position);

                for (int i = 0; i < 60; i++)
                {
                    int d = Dust.NewDust(Player.position, Player.width, Player.height, 88, 0f, 0f, 0, default, 2.5f);
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
                        damageSource = PlayerDeathReason.ByCustomReason(Player.name + " downed too many shots.");
                    else
                        damageSource = PlayerDeathReason.ByCustomReason(Player.name + "'s liver failed.");
                }
                if (vHex)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + " was charred by the brimstone inferno.");
                }
                if (ZoneCalamity && Player.lavaWet)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + "'s soul was released by the lava.");
                }
                if (gsInferno)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + "'s soul was extinguished.");
                }
                if (sulphurPoison)
                {
                    if (Main.rand.NextBool(2))
                        damageSource = PlayerDeathReason.ByCustomReason(Player.name + " was melted by the toxic waste.");
                    else
                        damageSource = PlayerDeathReason.ByOther(9);
                }
                if (dragonFire)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + "'s ashes scatter in the wind.");
                }
                if (hInferno)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + " was turned to ashes by the Profaned Goddess.");
                }
                if (hFlames || banishingFire)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + " fell prey to their sins.");
                }
                if (shadowflame)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + "'s spirit was turned to ash.");
                }
                if (bBlood)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + " became a blood geyser.");
                }
                if (cDepth)
                {
                    if (Main.rand.NextBool())
                        damageSource = PlayerDeathReason.ByCustomReason(Player.name + " was crushed by the pressure.");
                    else
                        damageSource = PlayerDeathReason.ByCustomReason(Player.name + "'s lungs collapsed.");
                }
                if (bFlames || weakBrimstoneFlames)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + " was consumed by the black flames.");
                }
                if (pFlames)
                {
                    if (Main.rand.NextBool())
                        damageSource = PlayerDeathReason.ByCustomReason(Player.name + "'s flesh was melted by the plague.");
                    else
                        damageSource = PlayerDeathReason.ByCustomReason(Player.name + " didn't vaccinate.");
                }
                if (astralInfection)
                {
                    if (Main.rand.NextBool())
                        damageSource = PlayerDeathReason.ByCustomReason(Player.name + "'s infection spread too far.");
                    else
                        damageSource = PlayerDeathReason.ByCustomReason(Player.name + "'s skin was replaced by the astral virus.");
                }
                if (nightwither)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + " was incinerated by lunar rays.");
                }
                if (vaporfied)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + " vaporized into thin air.");
                }
                if (manaOverloader || ManaBurn)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + "'s life was completely converted into mana.");
                }
                if (bloodyMary || everclear || evergreenGin || fireball || margarita || moonshine || moscowMule || redWine || screwdriver || starBeamRye || tequila || tequilaSunrise || vodka || whiteWine || Player.tipsy)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + " succumbed to alcohol sickness.");
                }
                if (witheredDebuff)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + " withered away.");
                }
            }
            if (profanedCrystalBuffs && !profanedCrystalHide)
            {
                damageSource = PlayerDeathReason.ByCustomReason(Player.name + " was summoned too soon.");
            }

            if (NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>()))
            {
                if (sCalDeathCount < 51)
                {
                    sCalDeathCount++;
                }
            }

            deathCount++;
            if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
            {
                SyncDeathCount(false);
            }

            return true;
        }
        #endregion

        #region On Respawn
        public override void OnRespawn(Player player)
        {
            if (healToFull)
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
            healValue = (int)(healValue * healingPotBonus);
        }
        #endregion

        #region Get Weapon Damage And KB
        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            if (flamethrowerBoost && item.CountsAsClass<RangedDamageClass>() && (item.useAmmo == AmmoID.Gel || CalamityLists.flamethrowerList.Contains(item.type)))
                damage += hoverboardBoost ? 0.35f : 0.25f;

            if (fungalSymbiote && CalamityLists.MushroomWeaponIDs.Contains(item.type))
                damage += 0.1f;

            if (item.CountsAsClass<RangedDamageClass>())
                acidRoundMultiplier = item.useTime / 20D;
            else
                acidRoundMultiplier = 1D;

            if (item.CountsAsClass<RogueDamageClass>())
            {
                // Apply weapon modifier stealth strike damage bonus
                if (item.Calamity().StealthStrikePrefixBonus != 0f && StealthStrikeAvailable())
                    damage += 1f - item.Calamity().StealthStrikePrefixBonus;
            }
        }

        public override void ModifyWeaponKnockback(Item item, ref StatModifier knockback)
        {
            bool rogue = item.CountsAsClass<RogueDamageClass>();
            if (auricBoost)
                knockback.Flat += item.knockBack * ((1f - modStealth) * 0.5f);

            if (whiskey)
                knockback.Flat += item.knockBack * 0.2f;

            if (tequila && Main.dayTime)
                knockback += item.knockBack * 0.1f;

            if (tequilaSunrise && Main.dayTime)
                knockback += item.knockBack * 0.2f;

            if (moscowMule)
                knockback += item.knockBack * 0.5f;

            if (titanHeartMask && rogue)
                knockback += item.knockBack * 0.05f;

            if (titanHeartMantle && rogue)
                knockback += item.knockBack * 0.05f;

            if (titanHeartBoots && rogue)
                knockback += item.knockBack * 0.05f;

            if (titanHeartSet && rogue)
                knockback += item.knockBack * 0.2f;

            if (titanHeartSet && StealthStrikeAvailable() && rogue)
                knockback += item.knockBack;
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
            if (!item.CountsAsClass<MeleeDamageClass>() && !item.noMelee && (!item.noUseGraphic && Player.meleeEnchant > 0))
            {
                if (Player.meleeEnchant == 7)
                {
                    if (Main.rand.NextBool(20))
                    {
                        int confettiDust = Main.rand.Next(139, 143);
                        int confetti = Dust.NewDust(new Vector2(hitbox.X,hitbox.Y), hitbox.Width, hitbox.Height, confettiDust, Player.velocity.X, Player.velocity.Y, 0, new Color(), 1.2f);
                        Main.dust[confetti].velocity.X *= (float)(1.0 + Main.rand.Next(-50, 51) * 0.01);
                        Main.dust[confetti].velocity.Y *= (float)(1.0 + Main.rand.Next(-50, 51) * 0.01);
                        Main.dust[confetti].velocity.X += Main.rand.Next(-50, 51) * 0.05f;
                        Main.dust[confetti].velocity.Y += Main.rand.Next(-50, 51) * 0.05f;
                        Main.dust[confetti].scale *= (float)(1.0 + Main.rand.Next(-30, 31) * 0.01);
                    }
                    if (Main.rand.NextBool(40) && Main.netMode != NetmodeID.Server)
                    {
                        int confettiGore = Main.rand.Next(276, 283);
                        int confetti = Gore.NewGore(Player.GetSource_ItemUse(item), new Vector2(hitbox.X, hitbox.Y), Player.velocity, confettiGore, 1f);
                        Main.gore[confetti].velocity.X *= (float)(1.0 + Main.rand.Next(-50, 51) * 0.01);
                        Main.gore[confetti].velocity.Y *= (float)(1.0 + Main.rand.Next(-50, 51) * 0.01);
                        Main.gore[confetti].scale *= (float)(1.0 + Main.rand.Next(-20, 21) * 0.01);
                        Main.gore[confetti].velocity.X += Main.rand.Next(-50, 51) * 0.05f;
                        Main.gore[confetti].velocity.Y += Main.rand.Next(-50, 51) * 0.05f;
                    }
                }
            }
            if (item.CountsAsClass<MeleeDamageClass>())
            {
                var source = Player.GetSource_ItemUse(item);
                if (fungalSymbiote && CalamityLists.MushroomWeaponIDs.Contains(item.type) && Player.whoAmI == Main.myPlayer)
                {
                    if (Player.itemAnimation == (int)(Player.itemAnimationMax * 0.1) ||
                        Player.itemAnimation == (int)(Player.itemAnimationMax * 0.3) ||
                        Player.itemAnimation == (int)(Player.itemAnimationMax * 0.5) ||
                        Player.itemAnimation == (int)(Player.itemAnimationMax * 0.7) ||
                        Player.itemAnimation == (int)(Player.itemAnimationMax * 0.9))
                    {
                        float yVel = 0f;
                        float xVel = 0f;
                        float yOffset = 0f;
                        float xOffset = 0f;
                        if (Player.itemAnimation == (int)(Player.itemAnimationMax * 0.9))
                        {
                            yVel = -7f;
                        }
                        if (Player.itemAnimation == (int)(Player.itemAnimationMax * 0.7))
                        {
                            yVel = -6f;
                            xVel = 2f;
                        }
                        if (Player.itemAnimation == (int)(Player.itemAnimationMax * 0.5))
                        {
                            yVel = -4f;
                            xVel = 4f;
                        }
                        if (Player.itemAnimation == (int)(Player.itemAnimationMax * 0.3))
                        {
                            yVel = -2f;
                            xVel = 6f;
                        }
                        if (Player.itemAnimation == (int)(Player.itemAnimationMax * 0.1))
                        {
                            xVel = 7f;
                        }
                        if (Player.itemAnimation == (int)(Player.itemAnimationMax * 0.7))
                        {
                            xOffset = 26f;
                        }
                        if (Player.itemAnimation == (int)(Player.itemAnimationMax * 0.3))
                        {
                            xOffset -= 4f;
                            yOffset -= 20f;
                        }
                        if (Player.itemAnimation == (int)(Player.itemAnimationMax * 0.1))
                        {
                            yOffset += 6f;
                        }
                        if (Player.direction == -1)
                        {
                            if (Player.itemAnimation == (int)(Player.itemAnimationMax * 0.9))
                            {
                                xOffset -= 8f;
                            }
                            if (Player.itemAnimation == (int)(Player.itemAnimationMax * 0.7))
                            {
                                xOffset -= 6f;
                            }
                        }
                        yVel *= 1.5f;
                        xVel *= 1.5f;
                        xOffset *= (float)Player.direction;
                        yOffset *= Player.gravDir;
                        Projectile.NewProjectile(source, (float)(hitbox.X + hitbox.Width / 2) + xOffset, (float)(hitbox.Y + hitbox.Height / 2) + yOffset, (float)Player.direction * xVel, yVel * Player.gravDir, ProjectileID.Mushroom, 0, 0f, Player.whoAmI);
                    }
                }
                if (flaskHoly)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, (int)CalamityDusts.ProfanedFire, Player.velocity.X * 0.2f + Player.direction * 3f, Player.velocity.Y * 0.2f, 100, default, 1f);
                    }
                }
                if (flaskBrimstone)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<BrimstoneFlame>(), Player.velocity.X * 0.2f + Player.direction * 3f, Player.velocity.Y * 0.2f, 100, default, 1f);
                    }
                }
                if (flaskCrumbling)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Stone, Player.velocity.X * 0.2f + Player.direction * 3f, Player.velocity.Y * 0.2f, 100, default, 0.75f);
                    }
                }
                if (eGauntlet)
                {
                    if (Main.rand.NextBool(3))
                    {
                        int element = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 66, Player.velocity.X * 0.2f + Player.direction * 3f, Player.velocity.Y * 0.2f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.25f);
                        Main.dust[element].noGravity = true;
                    }
                }
                if (cryogenSoul)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 67, Player.velocity.X * 0.2f + Player.direction * 3f, Player.velocity.Y * 0.2f, 100, default, 0.75f);
                    }
                }
                if (xerocSet)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 58, Player.velocity.X * 0.2f + Player.direction * 3f, Player.velocity.Y * 0.2f, 100, default, 1.25f);
                    }
                }
                if (dsSetBonus)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 27, Player.velocity.X * 0.2f + Player.direction * 3f, Player.velocity.Y * 0.2f, 100, default, 2.5f);
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

            if (enraged)
                damageMult += 1.25;

            if (witheredDebuff && witheringWeaponEnchant)
                damageMult += 0.6;

            // Rippers are always checked for application, because there are ways to get rippers outside of Rev now
            CalamityUtils.ApplyRippersToDamage(this, item.IsTrueMelee(), ref damageMult);

            damage = (int)(damage * damageMult);
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
            if (item.type == ItemID.StylistKilLaKillScissorsIWish || (item.type >= ItemID.BluePhaseblade && item.type <= ItemID.YellowPhaseblade) || (item.type >= ItemID.BluePhasesaber && item.type <= ItemID.YellowPhasesaber) || item.type == ItemID.OrangePhaseblade || item.type == ItemID.OrangePhasesaber)
            {
                int defenseAdd = (int)(target.defense * 0.5);
                damage += defenseAdd;
            }
            #endregion
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (proj.npcProj || proj.trap)
                return;

            bool isSummon = proj.CountsAsClass<SummonDamageClass>();
            Item heldItem = Player.ActiveItem();

            #region MultiplierBoosts
            double damageMult = 1D;

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
                if (proj.CountsAsClass<MagicDamageClass>())
                    damageMult += 0.3;
            }

            if (witheredDebuff)
                damageMult += 0.6;

            // Rippers are always checked for application, because there are ways to get rippers outside of Rev now
            CalamityUtils.ApplyRippersToDamage(this, proj.IsTrueMelee(), ref damageMult);

            if (proj.Calamity().stealthStrike && proj.CountsAsClass<RogueDamageClass>())
            {
                damageMult += bonusStealthDamage;
            }

            if (proj.type == ProjectileID.Gungnir)
            {
                if (target.life > (int)(target.lifeMax * 0.75))
                    damageMult += 1D;
            }

            // Adjust damage based on the damage multiplier
            damage = (int)(damage * damageMult);
            #endregion

            #region AdditiveBoosts
            if (proj.type == ProjectileID.TitaniumTrident)
            {
                int knockbackAdd = (int)(damage * 0.15 * (1f - target.knockBackResist));
                damage += knockbackAdd;
            }
            if (proj.type == ModContent.ProjectileType<AcidRoundProj>())
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
            #endregion

            #region MultiplicativeReductions

            // Fearmonger armor reduces the summoner cross-class nerf
            // Forbidden armor reduces said nerf when holding the respective helmet's preferred weapon type
            // Profaned Soul Crystal encourages use of other weapons, nerfing the damage would not make sense.

            bool forbidden = Player.head == ArmorIDs.Head.AncientBattleArmor && Player.body == ArmorIDs.Body.AncientBattleArmor && Player.legs == ArmorIDs.Legs.AncientBattleArmor;
            bool reducedNerf = fearmongerSet || (forbidden && heldItem.CountsAsClass<MagicDamageClass>()) || (GemTechSet && GemTechState.IsBlueGemActive);

            double summonNerfMult = reducedNerf ? 0.75 : 0.5;
            if (isSummon && heldItem.type > ItemID.None && !profanedCrystalBuffs)
            {
                bool classCheck = !heldItem.CountsAsClass<SummonDamageClass>() && 
                    (heldItem.CountsAsClass<MeleeDamageClass>() || heldItem.CountsAsClass<RangedDamageClass>() || heldItem.CountsAsClass<MagicDamageClass>() || 
                    heldItem.CountsAsClass<ThrowingDamageClass>());
                bool toolCheck = heldItem.pick == 0 && heldItem.axe == 0 && heldItem.hammer == 0;
                bool itemCanBeUsed = heldItem.useStyle != ItemUseStyleID.None;
                bool notAccessoryOrAmmo = !heldItem.accessory && heldItem.ammo == AmmoID.None;
                bool nerfNotDisabledByCalls = !CalamityLists.DisabledSummonerNerfItems.Contains(heldItem.type) && !CalamityLists.DisabledSummonerNerfMinions.Contains(proj.type);
                if (classCheck && itemCanBeUsed && toolCheck && notAccessoryOrAmmo && nerfNotDisabledByCalls)
                    damage = (int)(damage * summonNerfMult);
            }

            if (proj.CountsAsClass<RangedDamageClass>())
            {
                switch (proj.type)
                {
                    case ProjectileID.CrystalShard:
                        damage = (int)(damage * 0.6);
                        break;
                    case ProjectileID.HallowStar:
                        damage = (int)(damage * 0.7);
                        break;
                }
            }
            #endregion

            // Handle on-hit ranged effects for the gem tech armor set.
            if (proj.CountsAsClass<RangedDamageClass>() && proj.type != ModContent.ProjectileType<GemTechGreenFlechette>())
                GemTechState.RangedOnHitEffects(target, damage);
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
            for (int i = 0; i < Player.hurtCooldowns.Length; i++)
                if (Player.hurtCooldowns[i] > 0)
                    hasIFrames = true;

            // If this NPC deals defense damage with contact damage, then mark the player to take defense damage.
            // Defense damage is not applied if the player has iframes.
            if (!hasIFrames && !Player.creativeGodMode)
            {
                justHitByDefenseDamage = npc.Calamity().canBreakPlayerDefense;
                defenseDamageToTake = npc.Calamity().canBreakPlayerDefense ? damage : 0;
            }

            //
            // At this point, the player is guaranteed to be hit if there is no dodge.
            // The amount of damage that will be dealt is yet to be determined.
            //

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

                    float damageReduction = MathHelper.Lerp(velocityScalar, 1f, amount);
                    if (damageReduction < 1f)
                        contactDamageReduction += 1f - damageReduction;
                }
            }

            if (aSparkRare)
            {
                if (npc.type == NPCID.BlueJellyfish || npc.type == NPCID.PinkJellyfish || npc.type == NPCID.GreenJellyfish ||
                    npc.type == NPCID.FungoFish || npc.type == NPCID.BloodJelly || npc.type == NPCID.AngryNimbus || npc.type == NPCID.GigaZapper ||
                    npc.type == NPCID.MartianTurret || npc.type == ModContent.NPCType<Stormlion>() || npc.type == ModContent.NPCType<GhostBell>() || npc.type == ModContent.NPCType<BoxJellyfish>())
                    contactDamageReduction += 0.5;
            }

            if (fleshTotem && !Player.HasCooldown(Cooldowns.FleshTotem.ID))
            {
                Player.AddCooldown(Cooldowns.FleshTotem.ID, CalamityUtils.SecondsToFrames(20), true, coreOfTheBloodGod ? "bloodgod" : "default");
                contactDamageReduction += 0.5;
            }

            if (tarragonCloak && tarraMelee && !Player.HasCooldown(Cooldowns.TarragonCloak.ID))
                contactDamageReduction += 0.5;

            if (bloodflareMelee && bloodflareFrenzy && !Player.HasCooldown(BloodflareFrenzy.ID))
                contactDamageReduction += 0.5;

            if (npc.Calamity().tSad > 0)
                contactDamageReduction += 0.5;

            if (npc.Calamity().relicOfResilienceWeakness > 0)
            {
                contactDamageReduction += Items.Weapons.Typeless.RelicOfResilience.WeaknessDR;
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
                contactDamageReduction += 0.1;

            // Fearmonger set provides 15% multiplicative DR that ignores caps during the Holiday Moons.
            // To prevent abuse, this effect does not work if there are any bosses alive.
            if (fearmongerSet && !areThereAnyDamnBosses && (Main.pumpkinMoon || Main.snowMoon))
                contactDamageReduction += 0.15;

            if (abyssalDivingSuitPlates)
                contactDamageReduction += 0.15 - abyssalDivingSuitPlateHits * 0.03;

            if (aquaticHeartIce)
                contactDamageReduction += 0.2;

            if (encased)
                contactDamageReduction += 0.3;

            if (Player.ownedProjectileCounts[ModContent.ProjectileType<EnergyShell>()] > 0 && Player.ActiveItem().type == ModContent.ItemType<LionHeart>())
                contactDamageReduction += 0.5;

            if (theBee && Player.statLife >= Player.statLifeMax2 && theBeeCooldown <= 0)
            {
                contactDamageReduction += 0.5;
                theBeeCooldown = 600;
            }

            // Apply Adrenaline DR if available
            if (AdrenalineEnabled)
            {
                bool fullAdrenWithoutDH = !draedonsHeart && (adrenaline == adrenalineMax) && !adrenalineModeActive;
                bool usingNanomachinesWithDH = draedonsHeart && adrenalineModeActive;
                if (fullAdrenWithoutDH || usingNanomachinesWithDH)
                    contactDamageReduction += this.GetAdrenalineDR();
            }

            if (Player.mount.Active && (Player.mount.Type == ModContent.MountType<RimehoundMount>() || Player.mount.Type == ModContent.MountType<OnyxExcavator>()) && Math.Abs(Player.velocity.X) > Player.mount.RunSpeed / 2f)
                contactDamageReduction += 0.1;

            if (vHex)
                contactDamageReduction -= 0.1;

            if (irradiated)
                contactDamageReduction -= 0.1;

            if (corrEffigy)
                contactDamageReduction -= 0.05;

            // 10% is converted to 9%, 25% is converted to 20%, 50% is converted to 33%, 75% is converted to 43%, 100% is converted to 50%
            if (contactDamageReduction > 0D)
            {
                if (aCrunch)
                    contactDamageReduction *= (double)ArmorCrunch.MultiplicativeDamageReductionPlayer;

                // Contact damage reduction is reduced by DR Damage, which itself is proportional to defense damage
                int currentDefense = Player.GetCurrentDefense(false);
                if (totalDefenseDamage > 0 && currentDefense > 0)
                {
                    double drDamageRatio = CurrentDefenseDamage / (double)currentDefense;
                    if (drDamageRatio > 1D)
                        drDamageRatio = 1D;

                    contactDamageReduction *= 1D - drDamageRatio;
                    if (contactDamageReduction < 0D)
                        contactDamageReduction = 0D;
                }

                // Scale with base damage reduction
                if (Player.endurance > 0)
                    contactDamageReduction *= 1f - (Player.endurance * 0.01f);

                contactDamageReduction = 1D / (1D + contactDamageReduction);
                damage = (int)(damage * contactDamageReduction);
            }

            if (Main.hardMode && Main.expertMode)
            {
                bool reduceChaosBallDamage = npc.type == NPCID.ChaosBall && !NPC.AnyNPCs(NPCID.GoblinSummoner);

                if (reduceChaosBallDamage || npc.type == NPCID.ChaosBallTim || npc.type == NPCID.BurningSphere || npc.type == NPCID.WaterSphere)
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
                if (!disableAllDodges && !Player.HasCooldown(GlobalDodge.ID))
                {
                    // The Evolution
                    if (projRefRare)
                    {
                        proj.hostile = false;
                        proj.friendly = true;
                        proj.velocity *= -2f;
                        proj.extraUpdates += 1;
                        proj.penetrate = 1;
                        Player.GiveIFrames(20, false);

                        damage = 0;
                        projRefRareLifeRegenCounter = 300;
                        projTypeJustHitBy = proj.type;

                        Player.AddCooldown(GlobalDodge.ID, BalancingConstants.EvolutionReflectCooldown);
                        return;
                    }
                }
            }

            if (phantomicArtifact && Player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Summon.PhantomicShield>()] != 0)
            {
                Projectile pro = Main.projectile.AsEnumerable().Where(projectile => projectile.friendly && projectile.owner == Player.whoAmI && projectile.type == ModContent.ProjectileType<Projectiles.Summon.PhantomicShield>()).First();
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

            // Torch God does 1 damage but inflicts a random fire debuff
            if (proj.type == ProjectileID.TorchGod)
                damage = 1;

            // Reduce damage from vanilla traps

            // Explosives
            // 350 in normal, 450 in expert
            if (proj.type == ProjectileID.Explosives)
                damage = (int)(damage * (Main.expertMode ? 0.225 : 0.35));

            // Rolling Cacti
            // 45 in normal, 65 in expert for cactus
            // 30 in normal, 36 in expert for spikes
            else if (proj.type == ProjectileID.RollingCactus || proj.type == ProjectileID.RollingCactusSpike)
                damage = (int)(damage * (Main.expertMode ? 0.3 : 0.5));

            // Boulders
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

            // Reduce damage dealt by rainbow trails depending on how faded they are.
            if (proj.type == ProjectileID.HallowBossLastingRainbow)
            {
                // Find the oldPos of the projectile that is intersecting the player hitbox.
                Rectangle hitbox = proj.Hitbox;
                int trailLength = 80;
                int startOfDamageFalloff = 20;
                for (int k = 0; k < trailLength; k += 2)
                {
                    Vector2 trailHitbox = proj.oldPos[k];
                    if (!(trailHitbox == Vector2.Zero))
                    {
                        hitbox.X = (int)trailHitbox.X;
                        hitbox.Y = (int)trailHitbox.Y;

                        // Adjust damage based on what part of the trail intersected the player hitbox.
                        if (hitbox.Intersects(Player.Hitbox))
                        {
                            if (k > startOfDamageFalloff)
                                damage = (int)(damage * MathHelper.Lerp(0.4f, 1f, 1f - (k - startOfDamageFalloff) / (float)(trailLength - startOfDamageFalloff)));

                            break;
                        }
                    }
                }
            }

            // Check if the player has iframes for the sake of avoiding defense damage.
            bool hasIFrames = false;
            for (int i = 0; i < Player.hurtCooldowns.Length; i++)
                if (Player.hurtCooldowns[i] > 0)
                    hasIFrames = true;

            // If this projectile is capable of dealing defense damage, then apply defense damage.
            // Defense damage is not applied if the player has iframes.
            if (!hasIFrames && !Player.creativeGodMode)
            {
                justHitByDefenseDamage = proj.Calamity().DealsDefenseDamage;
                defenseDamageToTake = proj.Calamity().DealsDefenseDamage ? damage : 0;
            }

            //
            // At this point, the player is guaranteed to be hit if there is no dodge.
            // The amount of damage that will be dealt is yet to be determined.
            //

            if (projRefRare)
            {
                if (proj.type == projTypeJustHitBy)
                    projectileDamageReduction += 0.15;
            }

            if (aSparkRare)
            {
                if (proj.type == ProjectileID.MartianTurretBolt || proj.type == ProjectileID.GigaZapperSpear || proj.type == ProjectileID.CultistBossLightningOrbArc || proj.type == ProjectileID.VortexLightning || proj.type == ModContent.ProjectileType<DestroyerElectricLaser>() ||
                    proj.type == ProjectileID.BulletSnowman || proj.type == ProjectileID.BulletDeadeye || proj.type == ProjectileID.SniperBullet || proj.type == ProjectileID.VortexLaser)
                    projectileDamageReduction += 0.5;
            }

            if (CalamityLists.projectileDestroyExceptionList.TrueForAll(x => proj.type != x) && proj.active && !proj.friendly && proj.hostile && damage > 0)
            {
                // Daedalus Reflect counts as a reflect but doesn't actually stop you from taking damage
                if (daedalusReflect && !disableAllDodges && !projRefRare && !Player.HasCooldown(GlobalDodge.ID))
                    projectileDamageReduction += 0.5;
            }

            if (beeResist)
            {
                if (CalamityLists.beeProjectileList.Contains(proj.type))
                    projectileDamageReduction += 0.25;
            }

            if (trinketOfChiBuff)
                projectileDamageReduction += 0.1;

            // Fearmonger set provides 15% multiplicative DR that ignores caps during the Holiday Moons.
            // To prevent abuse, this effect does not work if there are any bosses alive.
            if (fearmongerSet && !areThereAnyDamnBosses && (Main.pumpkinMoon || Main.snowMoon))
                projectileDamageReduction += 0.15;

            if (abyssalDivingSuitPlates)
                projectileDamageReduction += 0.15 - abyssalDivingSuitPlateHits * 0.03;

            if (aquaticHeartIce)
                projectileDamageReduction += 0.2;

            if (encased)
                projectileDamageReduction += 0.3;

            if (Player.ownedProjectileCounts[ModContent.ProjectileType<EnergyShell>()] > 0 && Player.ActiveItem().type == ModContent.ItemType<LionHeart>())
                projectileDamageReduction += 0.5;

            if (theBee && Player.statLife >= Player.statLifeMax2 && theBeeCooldown <= 0)
            {
                projectileDamageReduction += 0.5;
                theBeeCooldown = 600;
            }

            // Apply Adrenaline DR if available
            if (AdrenalineEnabled)
            {
                bool fullAdrenWithoutDH = !draedonsHeart && (adrenaline == adrenalineMax) && !adrenalineModeActive;
                bool usingNanomachinesWithDH = draedonsHeart && adrenalineModeActive;
                if (fullAdrenWithoutDH || usingNanomachinesWithDH)
                    projectileDamageReduction += this.GetAdrenalineDR();
            }

            if (Player.mount.Active && (Player.mount.Type == ModContent.MountType<RimehoundMount>() || Player.mount.Type == ModContent.MountType<OnyxExcavator>()) && Math.Abs(Player.velocity.X) > Player.mount.RunSpeed / 2f)
                projectileDamageReduction += 0.1;

            // Damage reduction from Shield of the High Ruler if facing the projectile that just hit.
            // If the projectile is in the exact center of the player on the X axis YOU GET NOTHING, GOOD DAY, SIR!
            if (copyrightInfringementShield)
            {
                bool projectileRight = (Player.Center.X - proj.Center.X) < 0f;
                bool projectileLeft = (Player.Center.X - proj.Center.X) > 0f;
                if (Player.direction == 1)
                {
                    if (projectileRight)
                        projectileDamageReduction += 0.15;
                }
                else
                {
                    if (projectileLeft)
                        projectileDamageReduction += 0.15;
                }
            }

            if (vHex)
                projectileDamageReduction -= 0.1;

            if (irradiated)
                projectileDamageReduction -= 0.1;

            if (corrEffigy)
                projectileDamageReduction -= 0.05;

            // 10% is converted to 9%, 25% is converted to 20%, 50% is converted to 33%, 75% is converted to 43%, 100% is converted to 50%
            if (projectileDamageReduction > 0D)
            {
                if (aCrunch)
                    projectileDamageReduction *= (double)ArmorCrunch.MultiplicativeDamageReductionPlayer;

                // Projectile damage reduction is reduced by DR Damage, which itself is proportional to defense damage
                int currentDefense = Player.GetCurrentDefense(false);
                if (totalDefenseDamage > 0 && currentDefense > 0)
                {
                    double drDamageRatio = CurrentDefenseDamage / (double)currentDefense;
                    if (drDamageRatio > 1D)
                        drDamageRatio = 1D;

                    projectileDamageReduction *= 1D - drDamageRatio;

                    if (projectileDamageReduction < 0D)
                        projectileDamageReduction = 0D;
                }

                // Scale with base damage reduction
                if (Player.endurance > 0)
                    projectileDamageReduction *= 1f - (Player.endurance * 0.01f);

                projectileDamageReduction = 1D / (1D + projectileDamageReduction);
                damage = (int)(damage * projectileDamageReduction);
            }
        }
        #endregion

        #region On Hit
        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            if (sulfurSet && !proj.friendly && damage > 0)
            {
                if (Main.player[proj.owner] is null)
                {
                    if (!Main.npc[proj.owner].friendly)
                        Main.npc[proj.owner].AddBuff(BuffID.Poisoned, 120);
                }
                else
                {
                    Player p = Main.player[proj.owner];
                    if (p.hostile && Player.hostile && (Player.team != p.team || p.team == 0))
                        p.AddBuff(BuffID.Poisoned, 120);
                }
            }

            if (proj.hostile && damage > 0)
            {
                if (proj.type == ProjectileID.TorchGod)
                {
                    int fireDebuffTypes = CalamityWorld.death ? 9 : CalamityWorld.revenge ? 7 : Main.expertMode ? 5 : 3;
                    switch (Main.rand.Next(fireDebuffTypes))
                    {
                        case 0:
                            Player.AddBuff(BuffID.OnFire, 600);
                            break;

                        case 1:
                            Player.AddBuff(BuffID.Frostburn, 300);
                            break;

                        case 2:
                            Player.AddBuff(BuffID.CursedInferno, 300);
                            break;

                        case 3:
                            Player.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
                            break;

                        case 4:
                            Player.AddBuff(ModContent.BuffType<Shadowflame>(), 150);
                            break;

                        case 5:
                            Player.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 150);
                            break;

                        case 6:
                            Player.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
                            break;

                        case 7:
                            Player.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 300);
                            break;

                        case 8:
                            Player.AddBuff(ModContent.BuffType<Dragonfire>(), 300);
                            break;
                    }
                }
                else if (proj.type == ProjectileID.Explosives)
                {
                    Player.AddBuff(BuffID.OnFire, 600);
                }
                else if (proj.type == ProjectileID.Boulder)
                {
                    Player.AddBuff(BuffID.BrokenArmor, 600);
                }
                else if (proj.type == ProjectileID.FrostBeam && !Player.frozen && !gState)
                {
                    Player.AddBuff(ModContent.BuffType<GlacialState>(), 120);
                }
                else if (proj.type == ProjectileID.DeathLaser || proj.type == ProjectileID.RocketSkeleton)
                {
                    Player.AddBuff(BuffID.OnFire, 240);
                }
                else if (proj.type == ProjectileID.Skull)
                {
                    Player.AddBuff(BuffID.Weak, 300);
                }
                else if (proj.type == ProjectileID.ThornBall)
                {
                    Player.AddBuff(BuffID.Poisoned, 480);
                }
                else if (proj.type == ProjectileID.CultistBossIceMist)
                {
                    Player.AddBuff(BuffID.Frozen, 90);
                    Player.AddBuff(BuffID.Chilled, 180);
                }
                else if (proj.type == ProjectileID.CultistBossLightningOrbArc)
                {
                    Player.AddBuff(BuffID.Electrified, 120);
                }
                else if (proj.type == ProjectileID.AncientDoomProjectile)
                {
                    Player.AddBuff(ModContent.BuffType<Shadowflame>(), 120);
                }
                else if (proj.type == ProjectileID.CultistBossFireBallClone)
                {
                    Player.AddBuff(ModContent.BuffType<Shadowflame>(), 120);
                }
                else if (proj.type == ProjectileID.PhantasmalBolt || proj.type == ProjectileID.PhantasmalEye)
                {
                    Player.AddBuff(ModContent.BuffType<Nightwither>(), 180);
                }
                else if (proj.type == ProjectileID.PhantasmalSphere)
                {
                    Player.AddBuff(ModContent.BuffType<Nightwither>(), 360);
                }
                else if (proj.type == ProjectileID.PhantasmalDeathray)
                {
                    Player.AddBuff(ModContent.BuffType<Nightwither>(), 600);
                }
                else if (proj.type == ProjectileID.FairyQueenLance || proj.type == ProjectileID.HallowBossRainbowStreak || proj.type == ProjectileID.HallowBossSplitShotCore)
                {
                    Player.AddBuff(Main.dayTime ? ModContent.BuffType<HolyFlames>() : ModContent.BuffType<Nightwither>(), 180);
                }
                else if (proj.type == ProjectileID.HallowBossLastingRainbow)
                {
                    Player.AddBuff(Main.dayTime ? ModContent.BuffType<HolyFlames>() : ModContent.BuffType<Nightwither>(), 240);
                }
                else if (proj.type == ProjectileID.FairyQueenSunDance)
                {
                    Player.AddBuff(Main.dayTime ? ModContent.BuffType<HolyFlames>() : ModContent.BuffType<Nightwither>(), 300);
                }
                else if (proj.type == ProjectileID.BloodNautilusShot)
                {
                    Player.AddBuff(ModContent.BuffType<BurningBlood>(), 240);
                }
                else if (proj.type == ProjectileID.BloodShot)
                {
                    Player.AddBuff(ModContent.BuffType<BurningBlood>(), 180);
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
                        proj.damage = (int)Player.GetBestClassDamage().ApplyTo(proj.damage * 8);
                        proj.penetrate = 1;
                        Player.GiveIFrames(20, false);
                    }
                }

                // Reflects count as dodges. They share the timer and can be disabled by global dodge disabling effects.
                if (!disableAllDodges && !Player.HasCooldown(GlobalDodge.ID))
                {
                    if (daedalusReflect && !projRefRare)
                    {
                        proj.hostile = false;
                        proj.friendly = true;
                        proj.velocity *= -1f;
                        proj.penetrate = 1;
                        Player.GiveIFrames(20, false);

                        damage /= 2;

                        Player.AddCooldown(GlobalDodge.ID, BalancingConstants.DaedalusReflectCooldown);
                        // No return because the projectile hit isn't canceled -- it only does half damage.
                    }
                }
            }
        }
        #endregion
        
        #region Shoot
        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
        {
            if (bladeArmEnchant)
                return false;

            if (veneratedLocket)
            {
                if (item.CountsAsClass<RogueDamageClass>())
                {
                    float num72 = item.shootSpeed;
                    Vector2 vector2 = Player.RotatedRelativePoint(Player.MountedCenter, true);
                    float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
                    float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
                    if (Player.gravDir == -1f)
                    {
                        num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
                    }
                    float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
                    if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
                    {
                        num78 = (float)Player.direction;
                        num79 = 0f;
                        num80 = num72;
                    }
                    else
                    {
                        num80 = num72 / num80;
                    }

                    vector2 = new Vector2(Player.position.X + (float)Player.width * 0.5f + (float)(Main.rand.Next(201) * -(float)Player.direction) + ((float)Main.mouseX + Main.screenPosition.X - Player.position.X), Player.MountedCenter.Y - 600f);
                    vector2.X = (vector2.X + Player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
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
                    int p = Projectile.NewProjectile(source, vector2.X, vector2.Y, speedX4, speedY5, type, (int)(damage * 0.065), knockBack * 0.5f, Player.whoAmI);

                    if (p.WithinBounds(Main.maxProjectiles))
                        Main.projectile[p].DamageType = DamageClass.Generic; //in case melee/rogue variants bug out

                    // Handle AI edge-cases.
                    if (item.type == ModContent.ItemType<FinalDawn>())
                        Main.projectile[p].ai[1] = 1f;
                    if (item.type == ModContent.ItemType<TheAtomSplitter>())
                        Main.projectile[p].ai[0] = -1f;

                    if (StealthStrikeAvailable())
                    {
                        int knifeCount = 15;
                        int knifeDamage = (int)Player.GetTotalDamage<RogueDamageClass>().ApplyTo(35);
                        float angleStep = MathHelper.TwoPi / knifeCount;
                        float speed = 15f;

                        for (int i = 0; i < knifeCount; i++)
                        {
                            Vector2 velocity2 = new Vector2(0f, speed);
                            velocity2 = velocity2.RotatedBy(angleStep * i);
                            int knifeCol = Main.rand.Next(0, 2);

                            int knife = Projectile.NewProjectile(source, Player.Center, velocity2, ModContent.ProjectileType<VeneratedKnife>(), knifeDamage, 0f, Player.whoAmI, knifeCol, 0);
                            if (knife.WithinBounds(Main.maxProjectiles))
                                Main.projectile[knife].DamageType = DamageClass.Generic;
                        }
                    }
                }
            }

            if (RustyMedallionDroplets && RustyMedallionCooldown <= 0)
            {
                if (item.CountsAsClass<RangedDamageClass>())
                {
                    int d = (int)Player.GetTotalDamage<RangedDamageClass>().ApplyTo(Items.Accessories.RustyMedallion.AcidDropBaseDamage);
                    Vector2 startingPosition = Main.MouseWorld - Vector2.UnitY.RotatedByRandom(0.4f) * 1250f;
                    Vector2 directionToMouse = (Main.MouseWorld - startingPosition).SafeNormalize(Vector2.UnitY).RotatedByRandom(0.1f);
                    int drop = Projectile.NewProjectile(source, startingPosition, directionToMouse * 15f, ModContent.ProjectileType<ToxicannonDrop>(), d, 0f, Player.whoAmI);
                    if (drop.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[drop].penetrate = 2;
                        Main.projectile[drop].DamageType = DamageClass.Generic;
                    }
                    RustyMedallionCooldown = RustyMedallion.AcidCreationCooldown;
                }
            }

            return true;
        }
        #endregion

        #region Frame Effects
        public override void FrameEffects()
        {
            if (Player.Calamity().andromedaState == AndromedaPlayerState.LargeRobot ||
                Player.Calamity().andromedaState == AndromedaPlayerState.SpecialAttack)
            {
                Player.head = EquipLoader.GetEquipSlot(Mod, "HeadlessEquipTexture", EquipType.Head); // To make the head invisible on the map. The map was having a hissy fit because of hitbox changes.
            }
            else if ((profanedCrystal || profanedCrystalForce) && !profanedCrystalHide)
            {
                Player.legs = EquipLoader.GetEquipSlot(Mod, "ProfanedSoulCrystal", EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, "ProfanedSoulCrystal", EquipType.Body);
                Player.head = EquipLoader.GetEquipSlot(Mod, "ProfanedSoulCrystal", EquipType.Head);
                Player.wings = EquipLoader.GetEquipSlot(Mod, "ProfanedSoulCrystal", EquipType.Wings);
                Player.face = -1;

                bool enrage = !profanedCrystalForce && profanedCrystalBuffs && Player.statLife <= (int)(Player.statLifeMax2 * 0.5);

                if (profanedCrystalWingCounter.Value == 0)
                {
                    int key = profanedCrystalWingCounter.Key;
                    profanedCrystalWingCounter = new KeyValuePair<int, int>(key == 3 ? 0 : key + 1, enrage ? 5 : 7);
                }

                Player.wingFrame = profanedCrystalWingCounter.Key;
                profanedCrystalWingCounter = new KeyValuePair<int, int>(profanedCrystalWingCounter.Key, profanedCrystalWingCounter.Value - 1);
                Player.armorEffectDrawOutlines = true;
                if (profanedCrystalBuffs)
                {
                    Player.armorEffectDrawShadow = true;
                    if (enrage)
                    {
                        Player.armorEffectDrawOutlinesForbidden = true;
                    }
                }
            }
            else if ((snowmanPower || snowmanForce) && !snowmanHide)
            {
                Player.legs = EquipLoader.GetEquipSlot(Mod, "Popo", EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, "Popo", EquipType.Body);
                Player.head = EquipLoader.GetEquipSlot(Mod, snowmanNoseless ? "PopoNoseless" : "Popo", EquipType.Head);
                Player.face = -1;
            }
            else if (AresExoskeleton.ArmExists(Player))
                Player.body = EquipLoader.GetEquipSlot(Mod, "AresExoskeleton", EquipType.Body);
            
            else if ((abyssalDivingSuitPower || abyssalDivingSuitForce) && !abyssalDivingSuitHide)
            {
                Player.legs = EquipLoader.GetEquipSlot(Mod, "AbyssalDivingSuit", EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, "AbyssalDivingSuit", EquipType.Body);
                Player.head = EquipLoader.GetEquipSlot(Mod, "AbyssalDivingSuit", EquipType.Head);
                Player.face = -1;
            }
            else if ((aquaticHeartPower || aquaticHeartForce) && !aquaticHeartHide)
            {
                Player.legs = EquipLoader.GetEquipSlot(Mod, "AquaticHeart", EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, "AquaticHeart", EquipType.Body);
                Player.head = EquipLoader.GetEquipSlot(Mod, "AquaticHeart", EquipType.Head);
                Player.face = -1;
            }
            else if (meldTransformationPower || meldTransformationForce)
            {
                Player.legs = EquipLoader.GetEquipSlot(Mod, "MeldTransformation", EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, "MeldTransformation", EquipType.Body);
                Player.neck = (sbyte)EquipLoader.GetEquipSlot(Mod, "MeldTransformation", EquipType.Neck);
                Player.head = EquipLoader.GetEquipSlot(Mod, "MeldTransformation", EquipType.Head);
                Player.face = -1;
            }
            else if (omegaBlueTransformationPower || omegaBlueTransformationForce)
            {
                bool hasOmegaBlueCooldown = cooldowns.TryGetValue(OmegaBlue.ID, out CooldownInstance cd);
                if (hasOmegaBlueCooldown && cd.timeLeft > 1500)
                    Player.head = EquipLoader.GetEquipSlot(Mod, "OmegaBlueTransformation", EquipType.Head);
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
                Player.wings = EquipLoader.GetEquipSlot(Mod, "SnowRuffianMask", EquipType.Wings);
                bool falling = Player.gravDir == -1 ? Player.velocity.Y < 0.05f : Player.velocity.Y > 0.05f;
                if (Player.controlJump && falling)
                {
                    if (!Player.mount.Active)
                    {
                        Player.velocity.Y *= 0.9f;
                        Player.wingFrame = 3;
                    }
                    Player.noFallDmg = true;
                    Player.fallStart = (int)(Player.position.Y / 16f);
                }
            }
            if (Player.body == EquipLoader.GetEquipSlot(Mod, "AuricTeslaBodyArmor", EquipType.Body))
            {
                Player.back = (sbyte)EquipLoader.GetEquipSlot(Mod, "AuricTeslaBodyArmor", EquipType.Back);
            }

            if (Player.body == EquipLoader.GetEquipSlot(Mod, "SnowRuffianChestplate", EquipType.Body))
            {
                Player.back = (sbyte)EquipLoader.GetEquipSlot(Mod, "SnowRuffianChestplate", EquipType.Back);
                Player.neck = (sbyte)EquipLoader.GetEquipSlot(Mod, "SnowRuffianChestplate", EquipType.Neck);
            }

            if (Player.body == EquipLoader.GetEquipSlot(Mod, "EmpyreanCloak", EquipType.Body) && !meldTransformationPower && !meldTransformationForce)
            {
                Player.back = (sbyte)EquipLoader.GetEquipSlot(Mod, "EmpyreanCloak", EquipType.Back);
                Player.neck = (sbyte)EquipLoader.GetEquipSlot(Mod, "EmpyreanCloak", EquipType.Neck);
            }

            if (Player.body == EquipLoader.GetEquipSlot(Mod, "DaedalusBreastplate", EquipType.Body))
            {
                //Put the faulds on the chestplate
                Player.waist = (sbyte)EquipLoader.GetEquipSlot(Mod, "DaedalusBreastplate", EquipType.Waist);
            }

            bool victideBreastplateVisible = Player.body == EquipLoader.GetEquipSlot(Mod, "VictideBreastplate", EquipType.Body);
            //Give the player faulds if either the body armor or the leggings are equipped
            if (victideBreastplateVisible || Player.legs == EquipLoader.GetEquipSlot(Mod, "VictideGreaves", EquipType.Legs))
            {
                Player.waist = (sbyte)EquipLoader.GetEquipSlot(Mod, "VictideFaulds", EquipType.Waist);

                //Also prevent the player from having any front drawing accs which would be wildly offset because of the different proportions.
                if (victideBreastplateVisible)
                {
                    Player.front = -1;
                    Player.handoff = -1;
                    Player.handon = -1;
                }
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
                Player.dashType = 0;

                // Prevent the possibility of Shield of Cthulhu invulnerability exploits.
                Player.eocHit = -1;
                if (Player.eocDash != 0)
                    Player.eocDash = 0;
            }

            if ((silvaCountdown > 0 && hasSilvaEffect && silvaSet) || (DashID == GodslayerArmorDash.ID && Player.dashDelay < 0))
            {
                if (Player.lifeRegen < 0)
                    Player.lifeRegen = 0;
            }

            if (meteorSet)
                Player.spaceGun = false;

            if (Player.ownedProjectileCounts[ModContent.ProjectileType<GiantIbanRobotOfDoom>()] > 0)
                Player.yoraiz0rEye = 0;

            if (Player.HeldItem != null && Player.HeldItem.type == ItemID.FalconBlade)
                Player.moveSpeed += 0.2f;

            int totalMoonlightDyes = Player.dye.Count(dyeItem => dyeItem.type == ModContent.ItemType<ProfanedMoonlightDye>());
            if (totalMoonlightDyes > 0)
            {
                // Initialize the aurora drawer.
                int size = 340;
                FluidFieldManager.AdjustSizeRelativeToGraphicsQuality(ref size);

                float scale = MathHelper.Max(Main.screenWidth, Main.screenHeight) / size * 0.4f;
                if (ProfanedMoonlightAuroraDrawer is null || ProfanedMoonlightAuroraDrawer.Size != size)
                    ProfanedMoonlightAuroraDrawer = FluidFieldManager.CreateField(size, scale, 0.1f, 50f, 0.992f);

                int sourceArea = (int)Math.Ceiling(6f / ProfanedMoonlightAuroraDrawer.Scale) + 1;
                ProfanedMoonlightAuroraDrawer.ShouldUpdate = Player.miscCounter % 2 == 0;
                ProfanedMoonlightAuroraDrawer.UpdateAction = () =>
                {
                    // Aurora Count does not scale to save on resources if you have a lot of dyes
                    int auroraCount = 5;
                    for (int i = 0; i < auroraCount; i++)
                    {
                        float auroraPower = MathHelper.Clamp(totalMoonlightDyes / 3f, 0f, 1f);
                        float offsetAngle = MathHelper.TwoPi * i / auroraCount + Main.GlobalTimeWrappedHourly * 0.56f;
                        Color auroraColor = GetCurrentMoonlightDyeColor(offsetAngle) * 0.8f;
                        auroraColor.A = 0;

                        Vector2 auroraVelocity = (offsetAngle / 3f + Main.GlobalTimeWrappedHourly * 0.32f).ToRotationVector2();
                        auroraVelocity.Y = -Math.Abs(auroraVelocity.Y);
                        auroraVelocity = (auroraVelocity * new Vector2(0.15f, 1f) - Vector2.UnitX * Player.velocity.X / 9f).SafeNormalize(Vector2.UnitY) * 0.07f;

                        Vector2 drawPosition = Main.LocalPlayer.Center - Main.screenPosition;
                        Vector2 auroraSpawnPosition = drawPosition - Vector2.UnitY * 15f;
                        auroraSpawnPosition.X += (float)Math.Cos(offsetAngle + Main.GlobalTimeWrappedHourly * 0.91f) * 75f;

                        int x = (int)((auroraSpawnPosition.X - drawPosition.X) / ProfanedMoonlightAuroraDrawer.Scale);
                        int y = (int)((auroraSpawnPosition.Y - drawPosition.Y) / ProfanedMoonlightAuroraDrawer.Scale);
                        for (int j = -sourceArea; j <= sourceArea; j++)
                        {
                            for (int k = -sourceArea; k <= sourceArea; k++)
                                ProfanedMoonlightAuroraDrawer.CreateSource(x + size / 2 + j, y + size / 2 + k, auroraPower, auroraColor, auroraVelocity);
                        }
                    }
                };
            }
        }

        private void DisableDashes()
        {
            // Set the player to have no registered dashes.
            Player.dashType = 0;
            DashID = string.Empty;

            // Put the player in a permanent state of dash cooldown. This is removed 1/5 of a second after disabling the effect.
            // This is necessary so that arbitrary dashes from other mods are also blocked by Calamity.
            if (Player.dashDelay >= 0 && Player.dashDelay < DashDisableCooldown)
                Player.dashDelay = DashDisableCooldown;

            // Prevent the possibility of Shield of Cthulhu invulnerability exploits.
            Player.eocHit = -1;
            if (Player.eocDash != 0)
                Player.eocDash = 0;
        }

        private void WeakPetrification()
        {
            weakPetrification = true;
            Player.hasJumpOption_Cloud = false;
            Player.hasJumpOption_Sandstorm = false;
            Player.hasJumpOption_Blizzard = false;
            Player.hasJumpOption_Sail = false;
            Player.hasJumpOption_Fart = false;
            statigelJump = false;
            sulfurJump = false;
            Player.rocketBoots = 0;
            Player.jumpBoost = false;
            Player.slowFall = false;
            Player.gravControl = false;
            Player.gravControl2 = false;
            Player.jumpSpeedBoost = 0f;
            Player.wingTimeMax = (int)(Player.wingTimeMax * 0.5);
            Player.balloon = -1;
        }
        #endregion

        #region Pre Hurt
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            #region Ignore Incoming Hits
            // If any dodges are active which could dodge this hit, the hurting event is canceled (and the dodge is used).
            if (HandleDodges())
            {
                justHitByDefenseDamage = false;
                defenseDamageToTake = 0;
                return false;
            }

            // If Armageddon is active or the Boss Rush Immunity Curse is triggered, instantly kill the player.
            if (CalamityWorld.armageddon || (BossRushEvent.BossRushActive && bossRushImmunityFrameCurseTimer > 0))
            {
                if (areThereAnyDamnBosses || (BossRushEvent.BossRushActive && bossRushImmunityFrameCurseTimer > 0))
                    KillPlayer();
            }
            #endregion

            //Todo - At some point it'd be nice to have a "TransformationPlayer" that has all the transformation sfx and visuals so their priorities can be more easily managed.
            #region Custom Hurt Sounds
            if (hurtSoundTimer == 0)
            {
                if (Player.GetModPlayer<RoverDrivePlayer>().ProtectionMatrixDurability > 0)
                {
                    playSound = false;
                    SoundEngine.PlaySound(RoverDrive.ShieldHurtSound, Player.position);
                    hurtSoundTimer = 20;
                }
                else if ((profanedCrystal || profanedCrystalForce) && !profanedCrystalHide)
                {
                    playSound = false;
                    SoundEngine.PlaySound(Providence.HurtSound, Player.position);
                    hurtSoundTimer = 20;
                }
                else if ((abyssalDivingSuitPower || abyssalDivingSuitForce) && !abyssalDivingSuitHide)
                {
                    playSound = false;
                    SoundEngine.PlaySound(SoundID.NPCHit4, Player.position); //metal hit noise
                    hurtSoundTimer = 10;
                }
                else if ((aquaticHeartPower || aquaticHeartForce) && !aquaticHeartHide)
                {
                    playSound = false;
                    SoundEngine.PlaySound(SoundID.FemaleHit, Player.position); //female hit noise
                    hurtSoundTimer = 10;
                }
                else if (titanHeartSet)
                {
                    playSound = false;
                    SoundEngine.PlaySound(NPCs.Astral.Atlas.HurtSound, Player.position);
                    hurtSoundTimer = 10;
                }
                else if (Player.GetModPlayer<WulfrumTransformationPlayer>().transformationActive)
                {
                    playSound = false;
                    SoundEngine.PlaySound(SoundID.NPCHit4, Player.position);
                    hurtSoundTimer = 10;
                }
                else if (Player.GetModPlayer<WulfrumArmorPlayer>().wulfrumSet && (Player.name.ToLower() == "wagstaff" || Player.name.ToLower() == "john wulfrum"))
                {
                    playSound = false;
                    SoundEngine.PlaySound(SoundID.DSTMaleHurt, Player.position);
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
            if (Player.beetleDefense && Player.beetleOrbs > 0)
                damageMult += 0.05 * Player.beetleOrbs;

            // Blood Pact gives you a 1/4 chance to be crit, increasing the incoming damage by 25%.
            if (bloodPact && Main.rand.NextBool(4))
            {
                Player.AddBuff(ModContent.BuffType<BloodyBoost>(), 600);
                SoundEngine.PlaySound(BloodCritSound, Player.Center);
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
            if (shatteredCommunity && rageGainCooldown == 0)
            {
                float HPRatio = (float)damage / Player.statLifeMax2;
                float rageConversionRatio = 0.8f;

                // Damage to rage conversion is half as effective while Rage Mode is active.
                if (rageModeActive)
                    rageConversionRatio *= 0.5f;
                // If Rage is over 100%, damage to rage conversion scales down asymptotically based on how full Rage is.
                if (rage >= rageMax)
                    rageConversionRatio *= 3f / (3f + rage / rageMax);

                rage += rageMax * HPRatio * rageConversionRatio;
                rageGainCooldown = ShatteredCommunity.RageGainCooldown;
                // Rage capping is handled in MiscEffects
            }

            // Resilient Candle makes defense 5% more effective, aka 5% of defense is subtracted from all incoming damage.
            if (purpleCandle)
                damage = (int)(damage - (Player.statDefense * 0.05));

            return true;
        }
        #endregion

        #region Hurt

        public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldownCounter)
        {
            #region Defense Damage
            // Check if the player has iframes for the sake of avoiding defense damage.
            bool hasIFrames = false;
            for (int i = 0; i < Player.hurtCooldowns.Length; i++)
                if (Player.hurtCooldowns[i] > 0)
                    hasIFrames = true;

            // If the player was just hit by something capable of dealing defense damage, then apply defense damage.
            // Defense damage is not applied if the player has iframes.
            if (justHitByDefenseDamage && !hasIFrames && !Player.creativeGodMode)
            {
                DealDefenseDamage(defenseDamageToTake, damage);
            }
            justHitByDefenseDamage = false;
            defenseDamageToTake = 0;
            #endregion

            modStealth = 1f;

            // Give Rage combat frames because being hurt counts as combat.
            if (RageEnabled)
                rageCombatFrames = BalancingConstants.RageCombatDelayTime;

            if (Player.whoAmI == Main.myPlayer)
            {
                // Summon a portal if needed.
                if (Player.Calamity().persecutedEnchant && NPC.CountNPCS(ModContent.NPCType<DemonPortal>()) < 2)
                {
                    int tries = 0;
                    Vector2 spawnPosition;
                    do
                    {
                        spawnPosition = Player.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(270f, 420f);
                        tries++;
                    }
                    while (Collision.SolidCollision(spawnPosition - Vector2.One * 24f, 48, 24) && tries < 100);
                    CalamityNetcode.NewNPC_ClientSide(spawnPosition, ModContent.NPCType<DemonPortal>(), Player);
                }

                if (daedalusAbsorb && Main.rand.NextBool(10))
                {
                    int healAmt = (int)(damage / 2D);
                    Player.statLife += healAmt;
                    Player.HealEffect(healAmt);
                }

                if (absorber)
                {
                    int healAmt = (int)(damage / (sponge ? 16D : 20D));
                    Player.statLife += healAmt;
                    Player.HealEffect(healAmt);
                }

                if (witheringDamageDone > 0)
                {
                    double healCompenstationRatio = Math.Log(witheringDamageDone) * Math.Pow(witheringDamageDone, 2D / 3D) / 177000D;
                    if (healCompenstationRatio > 1D)
                        healCompenstationRatio = 1D;
                    int healCompensation = (int)(healCompenstationRatio * damage);
                    Player.statLife += (int)(healCompenstationRatio * damage);
                    Player.HealEffect(healCompensation);
                    Player.AddBuff(ModContent.BuffType<Withered>(), 1080);
                    witheringDamageDone = 0;
                }

                // Lose adrenaline on hit, unless using Draedon's Heart.
                if (AdrenalineEnabled)
                {
                    // Being hit for zero from Paladin's Shield damage share does not cancel Adrenaline.
                    // Adrenaline is not lost when hit if using Draedon's Heart.
                    if (!draedonsHeart && !adrenalineModeActive && damage > 0)
                    {
                        if (adrenaline >= adrenalineMax)
                        {
                            SoundEngine.PlaySound(AdrenalineHurtSound, Player.Center);
                        }
                        adrenaline = 0f;
                    }

                    // If using Draedon's Heart and not actively healing with Nanomachines, pause generation briefly.
                    if (draedonsHeart && !adrenalineModeActive)
                        nanomachinesLockoutTimer = DraedonsHeart.NanomachinePauseAfterDamage;
                }

                if (evilSmasherBoost > 0)
                    evilSmasherBoost -= 1;

                hellbornBoost = 0;

                if (trinketOfChi)
                    chiBuffTimer = 0;

                if (amidiasBlessing && damage > 50)
                {
                    Player.ClearBuff(ModContent.BuffType<AmidiasBlessing>());
                    SoundEngine.PlaySound(SoundID.Item96, Player.position);
                }

                if ((gShell || flameLickedShell) && !Player.panic)
                    Player.AddBuff(ModContent.BuffType<ShellBoost>(), 180);

                if (abyssalDivingSuitPlates && damage > 50)
                {
                    if (abyssalDivingSuitPlateHits < 3)
                        abyssalDivingSuitPlateHits++;

                    bool plateCDExists = cooldowns.TryGetValue(DivingPlatesBreaking.ID, out CooldownInstance plateDurability);
                    if (plateCDExists)
                        plateDurability.timeLeft = abyssalDivingSuitPlateHits;

                    if (abyssalDivingSuitPlateHits >= 3)
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath14, Player.position);
                        if (plateCDExists)
                            cooldowns.Remove(DivingPlatesBreaking.ID);
                        Player.AddCooldown(DivingPlatesBroken.ID, 10830);
                        for (int d = 0; d < 20; d++)
                        {
                            int dust = Dust.NewDust(Player.position, Player.width, Player.height, 31, 0f, 0f, 100, default, 2f);
                            Main.dust[dust].velocity *= 3f;
                            if (Main.rand.NextBool(2))
                            {
                                Main.dust[dust].scale = 0.5f;
                                Main.dust[dust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                        }
                        for (int d = 0; d < 35; d++)
                        {
                            int fire = Dust.NewDust(Player.position, Player.width, Player.height, 6, 0f, 0f, 100, default, 3f);
                            Main.dust[fire].noGravity = true;
                            Main.dust[fire].velocity *= 5f;
                            fire = Dust.NewDust(Player.position, Player.width, Player.height, 6, 0f, 0f, 100, default, 2f);
                            Main.dust[fire].velocity *= 2f;
                        }
                    }
                }

                if (aquaticHeartIce)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath7, Player.Center);
                    Player.AddCooldown(AquaticHeartIceShield.ID, CalamityUtils.SecondsToFrames(30));

                    for (int d = 0; d < 10; d++)
                    {
                        int ice = Dust.NewDust(Player.position, Player.width, Player.height, 67, 0f, 0f, 100, default, 2f);
                        Main.dust[ice].velocity *= 3f;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[ice].scale = 0.5f;
                            Main.dust[ice].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int d = 0; d < 15; d++)
                    {
                        int ice = Dust.NewDust(Player.position, Player.width, Player.height, 67, 0f, 0f, 100, default, 3f);
                        Main.dust[ice].noGravity = true;
                        Main.dust[ice].velocity *= 5f;
                        ice = Dust.NewDust(Player.position, Player.width, Player.height, 67, 0f, 0f, 100, default, 2f);
                        Main.dust[ice].velocity *= 2f;
                    }
                }

                if (tarraMelee)
                {
                    if (Main.rand.NextBool(4))
                        Player.AddBuff(ModContent.BuffType<TarraLifeRegen>(), 120);
                }
                else if (xerocSet)
                {
                    Player.AddBuff(ModContent.BuffType<EmpyreanRage>(), 240);
                    Player.AddBuff(ModContent.BuffType<EmpyreanWrath>(), 240);
                }
                else if (reaverDefense)
                {
                    if (Main.rand.NextBool(4))
                        Player.AddBuff(ModContent.BuffType<ReaverRage>(), 180);
                }

                if ((fBarrier || (aquaticHeart && NPC.downedBoss3)) && !areThereAnyDamnBosses)
                {
                    SoundEngine.PlaySound(SoundID.Item27, Player.position);
                    for (int m = 0; m < Main.maxNPCs; m++)
                    {
                        NPC npc = Main.npc[m];
                        if (!npc.active || npc.friendly || npc.dontTakeDamage)
                            continue;

                        float npcDist = (npc.Center - Player.Center).Length();
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

                // By setting brainOfConfusionItem, these accessories have this code already,
                // but doing it again allows for increased duration + The Amalgam's other buffs,
                // and also doesn't have random chance (why does Brain of Confusion not guarantee confusion on hit)
                if (aBrain || amalgam)
                {
                    for (int m = 0; m < Main.maxNPCs; m++)
                    {
                        NPC npc = Main.npc[m];
                        if (!npc.active || npc.friendly || npc.dontTakeDamage)
                            continue;

                        float npcDist = (npc.Center - Player.Center).Length();
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
                                npc.AddBuff(BuffID.Venom, (int)duration);
                                npc.AddBuff(ModContent.BuffType<Plague>(), (int)duration);
                                npc.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), (int)duration);
                                npc.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), (int)duration);
                                npc.AddBuff(ModContent.BuffType<Irradiated>(), (int)duration);
                            }
                        }
                    }

                    // Spawn the harmless brain images that are actually projectiles
                    var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<TheAmalgam>()));
                    Projectile.NewProjectile(source, Player.Center.X + Main.rand.Next(-40, 40), Player.Center.Y - Main.rand.Next(20, 60), Player.velocity.X * 0.3f, Player.velocity.Y * 0.3f, ProjectileID.BrainOfConfusion, 0, 0f, Player.whoAmI);
                }

                if (polarisBoost)
                {
                    polarisBoostCounter -= 10;
                    if (polarisBoostCounter < 0)
                        polarisBoostCounter = 0;

                    if (polarisBoostCounter >= 20)
                    {
                        polarisBoostTwo = false;
                        polarisBoostThree = true;
                    }
                    else if (polarisBoostCounter >= 10)
                    {
                        polarisBoostTwo = true;
                        polarisBoostThree = false;
                    }
                    else
                    {
                        polarisBoostThree = false;
                        polarisBoostTwo = false;
                    }
                }
            }

            if (Player.ownedProjectileCounts[ModContent.ProjectileType<DrataliornusBow>()] != 0)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<DrataliornusBow>() && Main.projectile[i].owner == Player.whoAmI)
                    {
                        Main.projectile[i].Kill();
                        break;
                    }
                }

                if (Player.wingTime > Player.wingTimeMax / 2)
                    Player.wingTime = Player.wingTimeMax / 2;
            }
        }
        #endregion

        #region Post Hurt

        public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldownCounter)
        {
            if (pArtifact && !profanedCrystal)
                Player.AddCooldown(Cooldowns.ProfanedSoulArtifact.ID, CalamityUtils.SecondsToFrames(5));

            // Silver Armor medkit timer
            if (silverMedkit && damage >= SilverArmorSetChange.SetBonusMinimumDamageToHeal)
                silverMedkitTimer = SilverArmorSetChange.SetBonusHealTime;

            // Bloodflare Core defense shattering
            if (bloodflareCore)
            {
                // Shattered defense has a hard cap equal to half of total defense.
                // It also has a soft cap determined by a formula so it isn't too powerful at excessively high defense.
                int shatterDefenseCap = (int)(1.5D * Math.Pow(Player.statDefense, 0.91D) - 0.5D * Player.statDefense);
                if (shatterDefenseCap > Player.statDefense / 2)
                    shatterDefenseCap = Player.statDefense / 2;

                // Every hit adds its damage as shattered defense.
                int newLostDefense = Math.Min(bloodflareCoreLostDefense + (int)damage, shatterDefenseCap);

                // Suddenly reducing your base defense stat does not let you suddenly reduce your shattered defense cap.
                // In other words, you can't ever reduce your lost defense by taking another hit.
                if (bloodflareCoreLostDefense < newLostDefense)
                    bloodflareCoreLostDefense = newLostDefense;

                // Play a sound and make dust to signify that defense has been shattered
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Player.Center);
                for (int i = 0; i < 36; ++i)
                {
                    float speed = Main.rand.NextFloat(1.8f, 8f);
                    Vector2 dustVel = new Vector2(speed, speed);
                    Dust d = Dust.NewDustDirect(Player.position, Player.width, Player.height, 90);
                    d.velocity = dustVel;
                    d.noGravity = true;
                    d.scale *= Main.rand.NextFloat(1.1f, 1.4f);
                    Dust.CloneDust(d).velocity = dustVel.RotatedBy(MathHelper.PiOver2);
                    Dust.CloneDust(d).velocity = dustVel.RotatedBy(MathHelper.Pi);
                    Dust.CloneDust(d).velocity = dustVel.RotatedBy(MathHelper.Pi * 1.5f);
                }
            }

            // Handle hit effects from the gem tech armor set.
            Player.Calamity().GemTechState.PlayerOnHitEffects((int)damage);

            if (Player.whoAmI == Main.myPlayer)
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

                // Give bonus immunity frames based on the type of damage dealt
                if (cooldownCounter != -1)
                    Player.hurtCooldowns[cooldownCounter] += iFramesToAdd;
                else
                    Player.immuneTime += iFramesToAdd;

                if (BossRushEvent.BossRushActive && CalamityConfig.Instance.BossRushIFrameCurse)
                    bossRushImmunityFrameCurseTimer = 180 + Player.immuneTime;

                if (aeroSet && damage > 25)
                {
                    var source = new ProjectileSource_AerospecSetFeathers(Player);
                    for (int n = 0; n < 4; n++)
                    {
                        int featherDamage = (int)Player.GetBestClassDamage().ApplyTo(20);
                        CalamityUtils.ProjectileRain(source, Player.Center, 400f, 100f, 500f, 800f, 20f, ModContent.ProjectileType<StickyFeatherAero>(), featherDamage, 1f, Player.whoAmI);
                    }
                }
                if (aBulwarkRare)
                {
                    var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<HideofAstrumDeus>()));
                    SoundEngine.PlaySound(SoundID.Item74, Player.position);
                    int blazeDamage = (int)Player.GetBestClassDamage().ApplyTo(25);
                    int astralStarDamage = (int)Player.GetBestClassDamage().ApplyTo(320);
                    Projectile.NewProjectile(source, Player.Center.X, Player.Center.Y, 0f, 0f, ModContent.ProjectileType<GodSlayerBlaze>(), blazeDamage, 5f, Player.whoAmI, 0f, 1f);
                    for (int n = 0; n < 12; n++)
                    {
                        CalamityUtils.ProjectileRain(source, Player.Center, 400f, 100f, 500f, 800f, 29f, ModContent.ProjectileType<AstralStar>(), astralStarDamage, 5f, Player.whoAmI);
                    }
                }
                if (dAmulet)
                {
                    var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<DeificAmulet>()));
                    for (int n = 0; n < 3; n++)
                    {
                        int deificStarDamage = (int)Player.GetBestClassDamage().ApplyTo(130);
                        Projectile star = CalamityUtils.ProjectileRain(source, Player.Center, 400f, 100f, 500f, 800f, 29f, ProjectileID.StarVeilStar, deificStarDamage, 4f, Player.whoAmI);
                        if (star.whoAmI.WithinBounds(Main.maxProjectiles))
                        {
                            star.DamageType = DamageClass.Generic;
                            star.usesLocalNPCImmunity = true;
                            star.localNPCHitCooldown = 5;
                        }
                    }
                }
                if (fCarapace)
                {
                    var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<FungalCarapace>()));
                    if (damage > 0)
                    {
                        SoundEngine.PlaySound(SoundID.NPCHit45, Player.position);
                        float spread = 45f * 0.0174f;
                        double startAngle = Math.Atan2(Player.velocity.X, Player.velocity.Y) - spread / 2;
                        double deltaAngle = spread / 8f;
                        double offsetAngle;
                        int fDamage = (int)Player.GetBestClassDamage().ApplyTo(70);
                        if (Player.whoAmI == Main.myPlayer)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                float xPos = Main.rand.NextBool(2) ? Player.Center.X + 100 : Player.Center.X - 100;
                                Vector2 spawnPos = new Vector2(xPos, Player.Center.Y + Main.rand.Next(-100, 101));
                                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                                int spore1 = Projectile.NewProjectile(source, spawnPos.X, spawnPos.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ProjectileID.TruffleSpore, fDamage, 1.25f, Player.whoAmI, 0f, 0f);
                                int spore2 = Projectile.NewProjectile(source, spawnPos.X, spawnPos.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ProjectileID.TruffleSpore, fDamage, 1.25f, Player.whoAmI, 0f, 0f);
                                Main.projectile[spore1].timeLeft = 300;
                                Main.projectile[spore2].timeLeft = 300;
                            }
                        }
                    }
                }
                if (aSpark)
                {
                    var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<HideofAstrumDeus>()));
                    if (damage > 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item93, Player.position);
                        float spread = 45f * 0.0174f;
                        double startAngle = Math.Atan2(Player.velocity.X, Player.velocity.Y) - spread / 2;
                        double deltaAngle = spread / 8f;
                        double offsetAngle;
                        
                        // Start with base damage, then apply the best damage class you can
                        int sDamage = 6;
                        if (aSparkRare)
                            sDamage += 42;
                        sDamage = (int)Player.GetBestClassDamage().ApplyTo(sDamage);
                        if (Player.whoAmI == Main.myPlayer)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                                int spark1 = Projectile.NewProjectile(source, Player.Center.X, Player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<Spark>(), sDamage, 1.25f, Player.whoAmI, 0f, 0f);
                                int spark2 = Projectile.NewProjectile(source, Player.Center.X, Player.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<Spark>(), sDamage, 1.25f, Player.whoAmI, 0f, 0f);
                                if (spark1.WithinBounds(Main.maxProjectiles))
                                {
                                    Main.projectile[spark1].timeLeft = 120;
                                    Main.projectile[spark1].DamageType = DamageClass.Generic;
                                }
                                if (spark2.WithinBounds(Main.maxProjectiles))
                                {
                                    Main.projectile[spark2].timeLeft = 120;
                                    Main.projectile[spark2].DamageType = DamageClass.Generic;
                                }
                            }
                        }
                    }
                }
                if (inkBomb && !abyssalMirror && !eclipseMirror)
                {
                    var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<Items.Accessories.InkBomb>()));
                    if (Player.whoAmI == Main.myPlayer && !Player.HasCooldown(Cooldowns.InkBomb.ID))
                    {
                        Player.AddCooldown(Cooldowns.InkBomb.ID, CalamityUtils.SecondsToFrames(20));
                        rogueStealth += 0.5f;
                        for (int i = 0; i < 3; i++)
                        {
                            SoundEngine.PlaySound(SoundID.Item61, Player.position);
                            int ink = Projectile.NewProjectile(source, Player.Center.X, Player.Center.Y, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-0f, -4f), ModContent.ProjectileType<InkBombProjectile>(), 0, 0, Player.whoAmI);
                            if (ink.WithinBounds(Main.maxProjectiles))
                                Main.projectile[ink].DamageType = DamageClass.Generic;
                        }
                    }
                }
                if (blazingCore)
                {
                    var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<BlazingCore>()));
                    if (Player.ownedProjectileCounts[ModContent.ProjectileType<BlazingSun>()] < 1 && Player.ownedProjectileCounts[ModContent.ProjectileType<BlazingSun2>()] < 1)
                    {
                        for (int i = 0; i < 360; i += 3)
                        {
                            Vector2 BCDSpeed = new Vector2(5f, 5f).RotatedBy(MathHelper.ToRadians(i));
                            Dust.NewDust(Player.Center, 1, 1, 244, BCDSpeed.X, BCDSpeed.Y, 0, default, 1.1f);
                        }
                        SoundEngine.PlaySound(SoundID.Item14, Player.Center);
                        int sunDamage = (int)Player.GetBestClassDamage().ApplyTo(1270);
                        int blazingSun = Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ModContent.ProjectileType<BlazingSun>(), sunDamage, 0f, Player.whoAmI, 0f, 0f);
                        Main.projectile[blazingSun].Center = Player.Center;
                        int blazingSun2 = Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ModContent.ProjectileType<BlazingSun2>(), 0, 0f, Player.whoAmI, 0f, 0f);
                        Main.projectile[blazingSun2].Center = Player.Center;
                    }
                }
                if (ataxiaBlaze)
                {
                    var fuckYouBitch = Player.GetSource_Misc("21");
                    if (damage > 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item74, Player.position);
                        int eDamage = (int)Player.GetBestClassDamage().ApplyTo(100);
                        if (Player.whoAmI == Main.myPlayer)
                            Projectile.NewProjectile(fuckYouBitch, Player.Center, Vector2.Zero, ModContent.ProjectileType<DeepseaBlaze>(), eDamage, 1f, Player.whoAmI, 0f, 0f);
                    }
                }
                else if (daedalusShard) // Daedalus Ranged helm
                {
                    var source = Player.GetSource_Misc("22");
                    if (damage > 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item27, Player.position);
                        float spread = 45f * 0.0174f;
                        double startAngle = Math.Atan2(Player.velocity.X, Player.velocity.Y) - spread / 2;
                        double deltaAngle = spread / 8f;
                        double offsetAngle;
                        int sDamage = (int)Player.GetTotalDamage<RangedDamageClass>().ApplyTo(27);
                        if (Player.whoAmI == Main.myPlayer)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                float randomSpeed = Main.rand.Next(1, 7);
                                float randomSpeed2 = Main.rand.Next(1, 7);
                                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                                int shard = Projectile.NewProjectile(source, Player.Center.X, Player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f) + randomSpeed, ProjectileID.CrystalShard, sDamage, 1f, Player.whoAmI, 0f, 0f);
                                int shard2 = Projectile.NewProjectile(source, Player.Center.X, Player.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f) + randomSpeed2, ProjectileID.CrystalShard, sDamage, 1f, Player.whoAmI, 0f, 0f);
                                if (shard.WithinBounds(Main.maxProjectiles))
                                    Main.projectile[shard].DamageType = DamageClass.Generic;
                                if (shard2.WithinBounds(Main.maxProjectiles))
                                    Main.projectile[shard2].DamageType = DamageClass.Generic;
                            }
                        }
                    }
                }
                else if (reaverDefense) //Defense and DR Helm
                {
                    var source = Player.GetSource_Misc("23");
                    if (damage > 0)
                    {
                        int rDamage = (int)Player.GetBestClassDamage().ApplyTo(80);
                        if (Player.whoAmI == Main.myPlayer)
                            Projectile.NewProjectile(source, Player.Center.X, Player.position.Y + 36f, 0f, -18f, ModContent.ProjectileType<ReaverThornBase>(), rDamage, 0f, Player.whoAmI, 0f, 0f);
                    }
                }
                else if (godSlayerDamage) //god slayer melee helm
                {
                    var source = Player.GetSource_Misc("24");
                    if (damage > 80)
                    {
                        SoundEngine.PlaySound(SoundID.Item73, Player.position);
                        float spread = 45f * 0.0174f;
                        double startAngle = Math.Atan2(Player.velocity.X, Player.velocity.Y) - spread / 2;
                        double deltaAngle = spread / 8f;
                        double offsetAngle;
                        int baseDamage = 675;
                        int shrapnelFinalDamage = (int)Player.GetTotalDamage<MeleeDamageClass>().ApplyTo(baseDamage);
                        if (Player.whoAmI == Main.myPlayer)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                                Projectile.NewProjectile(source, Player.Center.X, Player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<GodKiller>(), shrapnelFinalDamage, 5f, Player.whoAmI, 0f, 0f);
                                Projectile.NewProjectile(source, Player.Center.X, Player.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<GodKiller>(), shrapnelFinalDamage, 5f, Player.whoAmI, 0f, 0f);
                            }
                        }
                    }
                }
                else if (dsSetBonus)
                {
                    if (Player.whoAmI == Main.myPlayer)
                    {
                        var source = new ProjectileSource_DemonshadeSet(Player);
                        for (int l = 0; l < 2; l++)
                        {
                            int shadowbeamDamage = (int)Player.GetBestClassDamage().ApplyTo(3000);
                            Projectile beam = CalamityUtils.ProjectileRain(source, Player.Center, 400f, 100f, 500f, 800f, 22f, ProjectileID.ShadowBeamFriendly, shadowbeamDamage, 7f, Player.whoAmI);
                            if (beam.whoAmI.WithinBounds(Main.maxProjectiles))
                            {
                                beam.DamageType = DamageClass.Generic;
                                beam.usesLocalNPCImmunity = true;
                                beam.localNPCHitCooldown = 10;
                            }
                        }
                        for (int l = 0; l < 5; l++)
                        {
                            int scytheDamage = (int)Player.GetBestClassDamage().ApplyTo(5000);
                            Projectile scythe = CalamityUtils.ProjectileRain(source, Player.Center, 400f, 100f, 500f, 800f, 22f, ProjectileID.DemonScythe, scytheDamage, 7f, Player.whoAmI);
                            if (scythe.whoAmI.WithinBounds(Main.maxProjectiles))
                            {
                                scythe.DamageType = DamageClass.Generic;
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
            if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
            {
                SyncDeathCount(false);
            }
            var source = Player.GetSource_Death();
            Player.lastDeathPostion = Player.Center;
            Player.lastDeathTime = DateTime.Now;
            Player.showLastDeath = true;
            int coinsOwned = (int)Utils.CoinsCount(out bool flag, Player.inventory, new int[0]);
            if (Main.myPlayer == Player.whoAmI)
            {
                Player.lostCoins = coinsOwned;
                Player.lostCoinString = Main.ValueToCoins(Player.lostCoins);
            }
            if (Main.myPlayer == Player.whoAmI)
            {
                Main.mapFullscreen = false;
            }
            if (Main.myPlayer == Player.whoAmI)
            {
                Player.trashItem.SetDefaults(0, false);
                if (Player.difficulty == PlayerDifficultyID.SoftCore || Player.difficulty == PlayerDifficultyID.Creative)
                {
                    for (int i = 0; i < 59; i++)
                    {
                        if (Player.inventory[i].stack > 0 && ((Player.inventory[i].type >= ItemID.LargeAmethyst && Player.inventory[i].type <= ItemID.LargeDiamond) || Player.inventory[i].type == ItemID.LargeAmber))
                        {
                            int num = Item.NewItem(source, (int)Player.position.X, (int)Player.position.Y, Player.width, Player.height, Player.inventory[i].type, 1, false, 0, false, false);
                            Main.item[num].netDefaults(Player.inventory[i].netID);
                            Main.item[num].Prefix((int)Player.inventory[i].prefix);
                            Main.item[num].stack = Player.inventory[i].stack;
                            Main.item[num].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
                            Main.item[num].velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
                            Main.item[num].noGrabDelay = 100;
                            Main.item[num].favorited = false;
                            Main.item[num].newAndShiny = false;
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                            {
                                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, num, 0f, 0f, 0f, 0, 0, 0);
                            }
                            Player.inventory[i].SetDefaults(0, false);
                        }
                    }
                }
                else if (Player.difficulty == PlayerDifficultyID.MediumCore)
                {
                    Player.DropItems();
                }
                else if (Player.difficulty == PlayerDifficultyID.Hardcore)
                {
                    Player.DropItems();
                    Player.KillMeForGood();
                }
            }
            SoundEngine.PlaySound(SoundID.PlayerKilled, Player.position);
            Player.headVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
            Player.bodyVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
            Player.legVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
            Player.headVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * 0);
            Player.bodyVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * 0);
            Player.legVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * 0);
            if (Player.stoned)
            {
                Player.headPosition = Vector2.Zero;
                Player.bodyPosition = Vector2.Zero;
                Player.legPosition = Vector2.Zero;
            }
            for (int j = 0; j < 100; j++)
            {
                Dust.NewDust(Player.position, Player.width, Player.height, 235, (float)(2 * 0), -2f, 0, default, 1f);
            }
            Player.mount.Dismount(Player);
            Player.dead = true;
            Player.respawnTimer = 600;
            if (Main.expertMode)
            {
                Player.respawnTimer = (int)(Player.respawnTimer * 1.5);
            }
            Player.immuneAlpha = 0;
            Player.palladiumRegen = false;
            Player.iceBarrier = false;
            Player.crystalLeaf = false;

            PlayerDeathReason damageSource = PlayerDeathReason.ByOther(Player.Male ? 14 : 15);
            if (abyssDeath)
            {
                SoundEngine.PlaySound(DrownSound, Player.position);

                if (Main.rand.NextBool(2))
                {
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + " is food for the Wyrms.");
                }
                else
                {
                    damageSource = PlayerDeathReason.ByCustomReason("Oxygen failed to reach " + Player.name + " from the depths of the Abyss.");
                }
            }
            else if (CalamityWorld.armageddon && areThereAnyDamnBosses)
            {
                damageSource = PlayerDeathReason.ByCustomReason(Player.name + " failed the challenge at hand.");
            }
            else if (BossRushEvent.BossRushActive && bossRushImmunityFrameCurseTimer > 0)
            {
                damageSource = PlayerDeathReason.ByCustomReason(Player.name + " was destroyed by a mysterious force.");
            }
            NetworkText deathText = damageSource.GetDeathText(Player.name);
            if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI == Main.myPlayer)
            {
                NetMessage.SendPlayerDeath(Player.whoAmI, damageSource, (int)1000.0, 0, false, -1, -1);
            }
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(deathText, new Color(225, 25, 25));
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(deathText.ToString(), 225, 25, 25);
            }

            if (Player.whoAmI == Main.myPlayer && (Player.difficulty == PlayerDifficultyID.SoftCore || Player.difficulty == PlayerDifficultyID.Creative))
            {
                Player.DropCoins();
            }
            Player.DropTombstone(coinsOwned, deathText, 0);

            if (Player.whoAmI == Main.myPlayer)
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

                if (DownedBossSystem.downedYharon)
                    price += Item.buyPrice(0, 9, 0, 0);
                else if (DownedBossSystem.downedDoG)
                    price += Item.buyPrice(0, 6, 0, 0);
                else if (DownedBossSystem.downedProvidence)
                    price += Item.buyPrice(0, 3, 20, 0);
                else if (NPC.downedMoonlord)
                    price += Item.buyPrice(0, 2, 0, 0);
                else if (NPC.downedFishron || DownedBossSystem.downedPlaguebringer || DownedBossSystem.downedRavager)
                    price += Item.buyPrice(0, 1, 20, 0);
                else if (NPC.downedGolemBoss)
                    price += Item.buyPrice(0, 0, 90, 0);
                else if (NPC.downedPlantBoss || DownedBossSystem.downedCalamitas)
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

        #region Rogue Stealth
        private void ResetRogueStealth()
        {
            // rogueStealth doesn't reset every frame because it's a continuously building resource

            // these other parameters are rebuilt every frame based on the items you have equipped
            stealthDamage = 0f;
            bonusStealthDamage = 0;
            rogueStealthMax = 0f;
            stealthGenStandstill = 1f;
            stealthGenMoving = 1f;
            stealthStrikeThisFrame = false;
            stealthStrikeHalfCost = false;
            stealthStrike75Cost = false;
            stealthStrike85Cost = false;

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
            if (playRogueStealthSound && rogueStealth >= rogueStealthMax && Player.whoAmI == Main.myPlayer)
            {
                playRogueStealthSound = false;
                SoundEngine.PlaySound(RogueStealthSound, Player.position);
            }

            // If the player isn't at full stealth, reset the sound so it'll play again when they hit full stealth.
            else if (rogueStealth < rogueStealthMax)
                playRogueStealthSound = true;

            // Calculate stealth generation and gain stealth accordingly
            // 1f is normal speed, anything higher is faster. Default stealth generation is 2 seconds while standing still.
            float currentStealthGen = UpdateStealthGenStats();
            rogueStealth += rogueStealthMax * (currentStealthGen / 120f); // 120 frames = 2 seconds
            if (rogueStealth > rogueStealthMax)
                rogueStealth = rogueStealthMax;

            ProvideStealthStatBonuses();

            // If the player is using an item that deals damage and is on their first frame of a use of that item,
            // consume stealth if a stealth strike wasn't triggered manually by item code.

            // This doesn't trigger stealth strike effects (ConsumeStealthStrike instead of StealthStrike)
            // so non-rogue weapons can't call lasers down from the sky and such.
            // Using any item which deals no damage or is a tool doesn't consume stealth.
            Item it = Player.ActiveItem();
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
            if ((it.IsAir || !it.CountsAsClass<RogueDamageClass>()) && GemTechSet && GemTechState.IsRedGemActive)
                playerUsingWeapon = false;

            // Animation check depends on whether the item is "clockwork", like Clockwork Assault Rifle.
            // "Clockwork" weapons can chain-fire multiple stealth strikes (really only 2 max) until you run out of stealth.
            bool animationCheck = it.useAnimation == it.useTime
                ? Player.itemAnimation == Player.itemAnimationMax - 1 // Standard weapon (first frame of use animation)
                : Player.itemTime == (int)(it.useTime / Player.GetAttackSpeed<RogueDamageClass>()); // Clockwork weapon (first frame of any individual use event)

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
            Item it = !Main.HoverItem.IsAir ? Main.HoverItem : Player.ActiveItem();

            // The potential damage bonus from stealth is a complex equation based on the item's use time,
            // the player's averaged-together stealth generation stats, and max stealth.
            // Lower stealth generation rate (especially while moving) enables higher maximum stealth damage.
            // This enables stealth to be conditionally useful -- even powerful -- even without a dedicated stealth build.
            double averagedStealthGen = 0.8 * stealthGenMoving + 0.2 * stealthGenStandstill;
            double fakeStealthTime = BalancingConstants.BaseStealthGenTime / averagedStealthGen;

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

            double stealthAddedDamage = rogueStealth * BalancingConstants.UniversalStealthStrikeDamageFactor * useTimeFactor * stealthGenFactor;
            stealthDamage += (float)stealthAddedDamage;

            // Stealth slightly decreases aggro.
            Player.aggro -= (int)(rogueStealth * 300f);
        }

        private float UpdateStealthGenStats()
        {
            int finalDawnProjCount = Player.ownedProjectileCounts[ModContent.ProjectileType<FinalDawnProjectile>()] +
            Player.ownedProjectileCounts[ModContent.ProjectileType<FinalDawnFireSlash>()] +
            Player.ownedProjectileCounts[ModContent.ProjectileType<FinalDawnHorizontalSlash>()] +
            Player.ownedProjectileCounts[ModContent.ProjectileType<FinalDawnThrow>()] +
            Player.ownedProjectileCounts[ModContent.ProjectileType<FinalDawnThrow2>()];

            // If you are actively using an item, you cannot gain stealth.
            if (Player.itemAnimation > 0 || finalDawnProjCount > 0)
                return 0f;

            if (shadow)
            {
                stealthGenStandstill += 0.08f;
                stealthGenMoving += 0.08f;
            }

            if (eArtifact)
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
             * DGS  = (100% + 0.7% * T)
             * EM   = (100% + 0.7% * T) * 1.006^T
             * BOTH = (100% + 1% * T) * 1.006^T
             *
             * DGS alone caps in 143 frames
             * EM alone caps in 59 frames
             * Both together caps in 50 frames
             */
            if (darkGodSheath && eclipseMirror)
            {
                stealthAcceleration += 0.01f;
                stealthAcceleration *= 1.006f;
            }
            else if (eclipseMirror)
            {
                stealthAcceleration += 0.007f;
                stealthAcceleration *= 1.006f;
            }
            else if (darkGodSheath)
                stealthAcceleration += 0.007f;

            stealthAcceleration = MathHelper.Clamp(stealthAcceleration, 1f, StealthAccelerationCap);

            // You get 100% stealth regen while standing still and not on a mount. Otherwise, you get your stealth regeneration while moving.
            // Stealth only regenerates at 1/3 speed while moving.
            bool standstill = Player.StandingStill(0.1f) && !Player.mount.Active;
            return standstill ? stealthGenStandstill : stealthGenMoving * BalancingConstants.MovingStealthGenRatio * stealthAcceleration;
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
            else if (stealthStrike85Cost)
                consumptionMult = 0.85f;
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
            else if (stealthStrike85Cost)
            {
                rogueStealth -= 0.9f * stealthToLose;
                if (rogueStealth <= 0f)
                    rogueStealth = 0f;
            }
            else
                rogueStealth = remainingStealth;
        }
        #endregion

        #region Profaned Soul Crystal Stuffs

        internal void rollBabSpears(int randAmt, bool chaseable)
        {
            var source = Player.GetSource_ItemUse(Player.ActiveItem());
            if (Player.whoAmI == Main.myPlayer && !endoCooper && randAmt > 0 && Main.rand.NextBool(randAmt) && chaseable)
            {
                int spearsFired = 0;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (spearsFired == 2)
                        break;
                    if (Main.projectile[i].friendly && Main.projectile[i].owner == Player.whoAmI)
                    {
                        bool attack = Main.projectile[i].type == ModContent.ProjectileType<MiniGuardianAttack>() && Main.projectile[i].owner == Player.whoAmI;
                        if (attack || (Main.projectile[i].type == ModContent.ProjectileType<MiniGuardianDefense>() && Main.projectile[i].owner == Player.whoAmI))
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
                                int proj = Projectile.NewProjectile(source, Main.projectile[i].Center + posVec, velocity, ModContent.ProjectileType<MiniGuardianSpear>(), dam, 0f, Player.whoAmI, 0f, 0f);
                                Main.projectile[proj].originalDamage = dam;
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
            int frameCount = type == AnimationType.Walk || (!profanedCrystalForce && Player.statLife <= (int)(Player.statLifeMax2 * 0.5)) ? 7 : 10;
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
            ProfanedSoulCrystal.DetermineTransformationEligibility(Player);
            if ((profanedCrystal || profanedCrystalForce) && !profanedCrystalHide && Player.legs == EquipLoader.GetEquipSlot(Mod, "ProfanedSoulCrystal", EquipType.Legs))
            {
                bool usingCarpet = Player.carpetTime > 0 && Player.controlJump; //doesn't make sense for carpet to use jump frame since you have solid ground
                AnimationType animType = AnimationType.Walk;
                if ((Player.sliding || Player.velocity.Y != 0 || Player.mount.Active || Player.grappling[0] != -1 || Player.GoingDownWithGrapple) && !usingCarpet)
                    animType = AnimationType.Jump;
                else if (Player.velocity.X == 0 || usingCarpet)
                    animType = AnimationType.Idle;
                int frame = HandlePSCAnimationFrames(animType);
                Player.legFrame.Y = Player.legFrame.Height * frame;
            }
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

            if (CalamityConfig.Instance.WikiStatusMessage)
                Main.NewText($"[i:{ItemID.Book}]" + " [c/EE4939:Be sure to check out the Official Calamity Mod Wiki at ][c/3989FF:calamitymod.wiki.gg][c/EE4939:!] " + $"[i:{ItemID.Book}]");
        }

        /// <summary>
        /// Returns the range at which an abyss enemy can detect the player
        /// </summary>
        /// <param name="range">The default detection range</param>
        /// <returns></returns>
        public float GetAbyssAggro(float range)
        {
            range *= fishAlert ? 3f : 1f;
            range *= anechoicCoating ? 0.5f : 1f;
            range *= anechoicPlating ? 0.5f : 1f;
            range *= abyssalMirror ? 0.65f : 1f;
            range *= eclipseMirror ? 0.3f : 1f;
            range *= reaverExplore ? 0.9f : 1f;
            return range;
        }
        #endregion

        #region Mana Consumption Effects
        public override void OnConsumeMana(Item item, int manaConsumed)
        {
            CalamityPlayer modPlayer = Player.Calamity();
            if (Main.rand.NextBool(2) && modPlayer.lifeManaEnchant)
            {
                if (Main.myPlayer == Player.whoAmI)
                {
                    Player.HealEffect(-5);
                    Player.statLife -= 5;
                    if (Player.statLife <= 0)
                        Player.KillMe(PlayerDeathReason.ByCustomReason($"{Player.name} converted all of their life to mana."), 1000, -1);
                }

                for (int i = 0; i < 8; i++)
                {
                    Dust life = Dust.NewDustPerfect(Player.Top + Main.rand.NextVector2Circular(Player.width * 0.5f, 6f), 267);
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
        private void DealDefenseDamage(int damage, double realDamage)
        {
            if (realDamage <= 0)
                return;

            double ratioToUse = DefenseDamageRatio;
            if (draedonsHeart)
                ratioToUse *= 0.5;

            // Calculate the defense damage taken from this hit.
            int defenseDamageTaken = (int)(damage * ratioToUse);

            // There is a floor on defense damage based on difficulty; i.e. there is a minimum amount of defense damage from any hit that can deal defense damage.
            // This floor is only applied if bosses are alive
            if (areThereAnyDamnBosses)
            {
                int defenseDamageFloor = (BossRushEvent.BossRushActive ? 5 : CalamityWorld.death ? 4 : CalamityWorld.revenge ? 3 : Main.expertMode ? 2 : 1) * (NPC.downedMoonlord ? 3 : Main.hardMode ? 2 : 1);
                if (defenseDamageTaken < defenseDamageFloor)
                    defenseDamageTaken = defenseDamageFloor;
            }

            // There is also a cap on defense damage: 25% of the player's original defense.
            int cap = Player.statDefense / 4;
            if (defenseDamageTaken > cap)
                defenseDamageTaken = cap;

            // Apply defense damage to the adamantite armor set boost.
            if (AdamantiteSetDefenseBoost > 0)
            {
                int defenseDamageToAdamantite = Math.Min(AdamantiteSetDefenseBoost, defenseDamageTaken);
                AdamantiteSetDefenseBoost -= defenseDamageToAdamantite;
                defenseDamageTaken -= defenseDamageToAdamantite;

                // If Adamantite Armor's set bonus entirely absorbed the defense damage, then display the number and play the sound,
                // but don't actually reduce defense or trigger the defense damage recovery cooldown.
                if (defenseDamageTaken <= 0)
                {
                    ShowDefenseDamageEffects(defenseDamageToAdamantite);
                    return;
                }
            }

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

            // Audiovisual effects
            ShowDefenseDamageEffects(defenseDamageTaken);
        }

        private void ShowDefenseDamageEffects(int defDamage)
        {
            // Play a sound from taking defense damage.
            if (hurtSoundTimer == 0 && Main.myPlayer == Player.whoAmI)
            {
                SoundEngine.PlaySound(DefenseDamageSound with { Volume = DefenseDamageSound.Volume * 0.75f}, Player.position);
                hurtSoundTimer = 30;
            }

            // Display text indicating that defense damage was taken.
            string text = (-defDamage).ToString();
            Color messageColor = Color.LightGray;
            Rectangle location = new Rectangle((int)Player.position.X, (int)Player.position.Y - 16, Player.width, Player.height);
            CombatText.NewText(location, messageColor, Language.GetTextValue(text));
        }
        #endregion

        #region Kill
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (Player.whoAmI == Main.myPlayer && Player.ActiveItem().type == ModContent.ItemType<TheAnomalysNanogun>())
            {
                if (Main.rand.NextBool(20))
                    SoundEngine.PlaySound(IjiDeathSound, Player.Center);
            }
        }
        #endregion
    }
}
