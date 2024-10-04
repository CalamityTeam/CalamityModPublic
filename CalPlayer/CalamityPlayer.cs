using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Balancing;
using CalamityMod.BiomeManagers;
using CalamityMod.Buffs;
using CalamityMod.Buffs.Placeables;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Cooldowns;
using CalamityMod.DataStructures;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.FluidSimulation;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor;
using CalamityMod.Items.Armor.Bloodflare;
using CalamityMod.Items.Armor.Brimflame;
using CalamityMod.Items.Armor.Demonshade;
using CalamityMod.Items.Armor.LunicCorps;
using CalamityMod.Items.Armor.OmegaBlue;
using CalamityMod.Items.Armor.PlagueReaper;
using CalamityMod.Items.Armor.Silva;
using CalamityMod.Items.Dyes;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Mounts.Minecarts;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.TreasureBags.MiscGrabBags;
using CalamityMod.Items.VanillaArmorChanges;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Healing;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Systems;
using CalamityMod.Waters;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Liquid;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities.Terraria.Utilities;

namespace CalamityMod.CalPlayer
{
    public partial class CalamityPlayer : ModPlayer
    {
        #region Variables

        #region No Category
        public static bool areThereAnyDamnBosses = false;
        public static bool areThereAnyDamnEvents = false;
        public bool potionSick = false;
        public int timePotionSick;
        public bool drawBossHPBar = true;
        public float stealthUIAlpha = 1f;
        public float SulphWaterUIOpacity = 1f;
        public bool shouldDrawSmallText = true;
        public int projTypeJustHitBy;
        public int sCalDeathCount = 0;
        public int sCalKillCount = 0;
        public int actualMaxLife = 0;
        public static int chaosStateDuration = 900;
        public static int chaosStateDuration_NR = 1200;
        public bool killSpikyBalls = false;
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
        public bool reducedHolyFlamesDamage = false;
        public bool reducedNightwitherDamage = false;
        public float rangedAmmoCost = 1f;
        public float healingPotionMultiplier = 1f;
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
        public int plagueTaintedSMGDroneCooldown = 0;
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
        public int dragoonDrizzlefishGelBoost = 1;
        public int deadSunCounter = 6;
        public int DragonsBreathAudioCooldown = 0;
        public int DragonsBreathAudioCooldown2 = 0;
        public int PhotoAudioCooldown = 0;
        public int PhotoTimer = 90;
        public bool brittleStarBuffMode = false;
        public bool sBlasterDashActivated = false;
        public int saharaSlicersBolts = 0;
        public int oceanCrestTimer = 0;
        public int Holyhammer = 0;
        public int PHAThammer = 0;
        public int StellarHammer = 0;
        public int GalaxyHammer = 0;
        public int NorfleetCounter = 0;
        public int hideOfDeusMeleeBoostTimer = 0;
        public int alcoholPoisonLevel = 0;
        public int modStealthTimer;
        public int dashTimeMod;
        public int hInfernoBoost = 0;
        public int packetTimer = 0;
        public int navyRodAuraTimer = 0;
        public int brimLoreInfernoTimer = 0;
        public int tarraLifeAuraTimer = 0;
        public int bloodflareHeartTimer = 300;
        public int dragonRageHits = 0;
        public int dragonRageCooldown = 0;
        public float modStealth = 1f;
        public float aquaticBoostMax = 10000f;
        public float aquaticBoost = 0f;
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
        public int necroReviveCounter = -1;
        public int hideOfDeusTimer = 0;
        public int murasamaHitCooldown = 0;
        public int giantShellPostHit = 0;
        public int tortShellPostHit = 0;
        public int spiritOriginBullseyeShootCountdown = 0;
        public int spiritOriginConvertedCrit = 0;
        public int RustyMedallionCooldown = 0;
        public int MiniSwamerCooldown = 0;
        public float SulphWaterPoisoningLevel;
        public float holyInfernoFadeIntensity;
        public NPC unstableSelectedTarget;
        public int zapActivity = 0;
        public bool ragePulse = false;
        public int ragePulseVisualTimer = 0;
        public int ragePulseTimer = 0;

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
        public int fullRageSoundCountdownTimer = 0;
        private const int FullRageSoundDelay = 300; // The "Rage full" sound cannot play for 5 seconds after Rage has filled. This stops it from jittering.
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
        public static readonly SoundStyle LeonDeathNoiseRE4_ForGFB = new("CalamityMod/Sounds/Custom/GFB/LeonDeathNoiseRE4");
        public static readonly SoundStyle BaroclawHit = new("CalamityMod/Sounds/NPCKilled/DevourerSegmentBreak2") { Volume = 0.7f };
        public static readonly SoundStyle AbsorberHit = new("CalamityMod/Sounds/Custom/AbilitySounds/SilvaActivation") { Volume = 0.7f };
        #endregion

        #region Rogue
        public float rogueStealth = 0f;
        public float rogueStealthMax = 0f;
        public float stealthGenStandstill = 1f;
        public float stealthGenMoving = 1f;
        public int flatStealthLossReduction = 0;
        public const float StealthAccelerationCap = 1.5f;
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
        #endregion

        #region Pet
        public bool thirdSage = false;
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
        public bool eidolonSnailPet = false;
        public bool lordePet = false;
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
        public bool AdrenalineTrail = false;
        public float adrenaline = 0f;
        public float adrenalineMax = 100f; // 0 to 100% by default
        public int adrenalinePauseTimer = 0;
        public int AdrenalineDuration = CalamityUtils.SecondsToFrames(5);
        public int AdrenalineChargeTime = CalamityUtils.SecondsToFrames(30);
        public int AdrenalineFadeTime = CalamityUtils.SecondsToFrames(2);
        #endregion

        #region Defense Damage
        // Ratio at which mitigated damage is converted into defense damage.
        // This is a significant rework, so the ratio is much higher than the previous 0.1 / 10%.
        // The net difference between incoming damage and final taken damage is what is multiplied by this ratio.
        //
        // Example: You have 200 defense and 25% DR and get hit for 576, on Expert.
        //
        // Incoming damage = 576
        // Defense reduction = 0.75 * 200 = 150
        // Damage after defense = 426
        // DR reduction = 0.25 * 426 = 106.5
        // Damage after DR = 319.5 (rounds down to 319)
        //
        // Net Difference = 576 - 319 = 257
        // Defense Damage = 257 * 0.3333 = 85.6581 (rounds up to 86)
        //
        // The player then loses 86 defense.
        // DR is lost according to the ratio of defense lost versus total defense.
        // In this case, that ratio is 86 / 200 = 0.43.
        // The player loses 0.43 * 0.25 = 10.75% DR.
        public double defenseDamageRatio = BalancingConstants.DefaultDefenseDamageRatio;

        // Current effect of defense damage, calculated as total defense damage lerped to zero over the recovery time.
        public int CurrentDefenseDamage => (int)(totalDefenseDamage * ((float)defenseDamageRecoveryFrames / totalDefenseDamageRecoveryFrames));

        // Total defense damage inflicted. This number keeps increasing if the player is repeatedly hit during the recovery period.
        internal int totalDefenseDamage = 0;

        // Defense damage from a single hit recovers in 60 frames, no matter how big the hit was.
        // If you get hit AGAIN before you have fully recovered, 60 more frames are added to your recovery timer!
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

        // Temporary bool for whether the current instance of incoming damage to the player is one that inflicts defense damage.
        // Bloodflare Core ignores this and makes every single instance of incoming damage apply defense damage.
        public bool nextHitDealsDefenseDamage = false;
        #endregion

        #region Energy Shields
        public bool HasAnyEnergyShield => roverDrive || lunicCorpsSet || ((pSoulArtifact && !profanedCrystal) || profanedCrystalBuffs) || sponge;
        public bool freeDodgeFromShieldAbsorption = false;
        public bool drawnAnyShieldThisFrame = false;

        // TODO -- Some way to show the player their total shield points.
        public int TotalEnergyShielding => RoverDriveShieldDurability + LunicCorpsShieldDurability + pSoulShieldDurability + SpongeShieldDurability;
        public int TotalMaxShieldDurability => (roverDrive ? RoverDrive.ShieldDurabilityMax : 0) + (lunicCorpsSet ? LunicCorpsHelmet.ShieldDurabilityMax : 0) + (profanedCrystalBuffs ? ProfanedSoulCrystal.ShieldDurabilityMax : ((pSoulArtifact && !profanedCrystal) ? ProfanedSoulArtifact.ShieldDurabilityMax : 0)) + (sponge ? TheSponge.ShieldDurabilityMax : 0);

        public int RoverDriveShieldDurability = 0;
        public int LunicCorpsShieldDurability = 0;
        public int SpongeShieldDurability = 0;

        public bool roverDrive = false;
        public bool roverDriveShieldVisible = false;

        // Lunic Corps shield is controlled by its armor set bool
        // Lunic Corps shield comes from an armor set and its visibility is non optional
        internal float lunicCorpsShieldPartialRechargeProgress = 0f;
        internal bool playedLunicCorpsShieldSound = false;

        //Profaned soul shield applies to psa and psc, with differing max hps for each
        public int pSoulShieldDurability = 0;
        public bool pSoulShieldVisible = false;
        internal bool playedProfanedSoulShieldSound = false;
        internal float pSoulShieldPartialRechargeProgress = 0f;

        public bool sponge = false;
        public bool spongeShieldVisible = false;
        internal float spongeShieldPartialRechargeProgress = 0f;
        internal bool playedSpongeShieldSound = false;
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
        public bool shieldOfTheHighRulerDashVelocityBoosted = false;
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
        public bool chiRegen = false;
        public bool affliction = false;
        public bool stressPills = false;
        public bool laudanum = false;
        public bool heartOfDarkness = false;
        public bool draedonsHeart = false;
        public bool vexation = false;
        public bool dodgeScarf = false;
        public bool evasionScarf = false;
        public bool badgeOfBravery = false;
        public bool warbannerOfTheSun = false;
        public bool cryogenSoul = false;
        public bool ascendantInsignia = false;
        public int ascendantInsigniaBuffTime = 0;
        public int ascendantInsigniaCooldown = 0;
        public bool ascendantTrail = false;
        public bool eGauntlet = false;
        public int gloveLevel = 0; // Used to prevent glove stacking
        public bool alreadyHasFrogLeg = false; // Used to prevent Frog Leg tinker stacking
        public bool eTalisman = false;
        public int statisTimer = 0;
        public bool nucleogenesis = false;
        public bool nuclearFuelRod = false;
        public bool nCore = false;
        public bool deepDiver = false;
        public bool abyssalDivingSuitPlates = false;
        public int abyssalDivingSuitPlateHits = 0;
        public bool aquaticHeartWaterBuff = false;
        public bool aquaticHeartIce = false;
        public bool aSpark = false;
        public bool transformer = false;
        public bool hideOfDeus = false;
        public bool dAmulet = false;
        public bool rampartOfDeities = false;
        public bool gShell = false;
        public bool lAmbergris = false;
        public bool tortShell = false;
        public bool absorber = false;
        public bool alwaysHoneyRegen = false;
        public bool honeyTurboRegen = false;
        public bool honeyDewHalveDebuffs = false;
        public bool livingDewHalveDebuffs = false;
        public int jewelBonusDefense = 0;
        public float pulseCounter = 0; // Toxic Heart
        public float pulseRate = 1; // Toxic Heart
        public bool aAmpoule = false;
        public bool rOoze = false;
        public bool JustWasDebuffed = false;
        public bool fBarrier = false;
        public bool aBrain = false;
        public bool amalgam = false;
        public bool raiderTalisman = false;
        public float raiderCritBonus = RaidersTalisman.RaiderBonus;
        public int raiderSoundCooldown = 0;
        public bool gSabaton = false;
        public int gSabatonHotkeyHoldTime = 0;
        public int gSabatonFall = 0;
        public bool gSabatonFalling = false;
        public int gSabatonTempJumpSpeed = 0;
        public bool sGlyph = false;
        public bool sRegen = false;
        public bool tracersDust = false;
        public bool elysianWingsDust = false;
        public bool tracersCelestial = false;
        public bool tracersElysian = false;
        public bool tracersSeraph = false;
        public bool frostFlare = false;
        public bool uberBees = false;
        public bool evolution = false;
        public int evolutionLifeRegenCounter = 0;
        public bool nanotech = false;
        public bool deadshotBrooch = false;
        public bool shadowMinions = false;
        public bool holyMinions = false;
        public bool alchFlask = false;
        public bool toxicHeart = false;
        public bool abaddon = false;
        public bool aeroStone = false;
        public bool lifejelly = false;
        public bool cleansingjelly = false;
        public bool GrandGelatin = false;
        public int CleansingEffect = 0;
        public bool community = false;
        public bool shatteredCommunity = false;
        public bool fleshTotem = false;
        public bool bloodPact = false;
        public bool bloodPactBoost = false;
        public bool bloodflareCore = false;
        public int bloodflareCoreRemainingHealOverTime = 0;

        public bool chaliceOfTheBloodGod = false;
        public double chaliceBleedoutBuffer = 0D;
        public double chaliceDamagePointPartialProgress = 0D;
        public int chaliceBleedoutToApplyOnHurt = 0;
        public int chaliceHitOriginalDamage = 0;

        public bool elementalHeart = false;
        public bool crownJewel = false;
        public bool infectedJewel = false;
        public bool purity = false;
        public bool eleResist = false;
        public int PurityHealSlowdownFrames = 0;
        public bool harpyRing = false;
        public bool angelTreads = false;
        public bool harpyWingBoost = false; //harpy wings + harpy ring
        public bool fleshKnuckles = false;
        public bool ironBoots = false;
        public bool depthCharm = false;
        public bool anechoicPlating = false;
        public bool jellyfishNecklace = false;
        public bool fairyBoots = false;
        public bool flameWakerBoots = false;
        public int bootLevel = 0; //Used to prevent Flame Waker and Hellfire Treads stacking
        public bool hellfireTreads = false;
        public bool abyssalAmulet = false;
        public bool lumenousAmulet = false;
        public bool oceanCrest = false;
        public bool aquaticEmblem = false;
        public bool spiritOrigin = false;
        public bool spiritOriginVanity = false;
        public bool darkSunRing = false;
        public bool crawCarapace = false;
        public bool baroclaw = false;
        public bool HasReducedDashFirstFrame = false;
        public bool HasIncreasedDashFirstFrame = false;
        public bool voidOfCalamity = false;
        public bool voidOfExtinction = false;
        public bool eArtifact = false;
        public bool dArtifact = false;
        public bool auricSArtifact = false;
        public bool pSoulArtifact = false;
        public bool giantPearl = false;
        public bool normalityRelocator = false;
        public bool flameLickedShell = false;
        public int flameLickedShellParry = 0;
        public bool flameLickedShellEmpoweredParry = false;
        public bool Pauldron = false;
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
        public int blazingCoreParry = 0;
        public int blazingCoreSuccessfulParry = 0;
        public bool blazingCoreEmpoweredParry = false;
        public bool voltaicJelly = false;
        public bool jellyChargedBattery = false;
        public float summonProjCooldown;
        public bool sandWaifu = false;
        public bool sandWaifuVanity = false;
        public bool sandBoobWaifu = false;
        public bool sandBoobWaifuVanity = false;
        public bool cloudWaifu = false;
        public bool cloudWaifuVanity = false;
        public bool brimstoneWaifu = false;
        public bool brimstoneWaifuVanity = false;
        public bool sirenWaifu = false;
        public bool sirenWaifuVanity = false;
        public bool fungalClump = false;
        public bool fungalClumpVanity = false;
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
        public bool miniOldDukeVanity = false;
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
        public bool MiniSwarmers = false;
        public bool noStupidNaturalARSpawns = false;
        public int voidFrameCounter = 0;
        public int voidFrame = 0;
        public bool rottenDogTooth = false;
        public bool angelicAlliance = false;
        public int angelicActivate = -1;
        public bool BloomStoneRegen = false;
        public bool ChaosStone = false;
        public bool CryoStone = false;
        public bool CryoStoneVanity = false;
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
        public bool necroSet = false; //vanilla armor
        public bool frostSet = false; //vanilla armor
        public bool victideSet = false;
        public bool victideSummoner = false;
        public bool sulphurSet = false;
        public bool sulphurJump = false;
        public int sulphurBubbleCooldown = 0;
        public bool aeroSet = false;
        public bool statigelSet = false;
        public bool tarraSet = false;
        public bool tarraMelee = false;
        public bool tarragonCloak = false;
        public int tarraDefenseTime = 600;
        public bool tarraMage = false;
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
        public bool SpeedBlasterDashStarted = false;
        public bool ataxiaBolt = false;
        public bool ataxiaFire = false;
        public bool ataxiaVolley = false;
        public bool ataxiaBlaze = false;
        public bool hydrothermalSmoke = false;
        public bool daedalusAbsorb = false;
        public bool daedalusShard = false;
        public bool brimflameSet = false;
        public bool brimflameFrenzy = false;
        public bool lunicCorpsSet = false;
        public bool lunicCorpsLegs = false;
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
        public int AbaddonCooldown = 0;
        public int AlchFlaskCooldown = 0;
        public bool plagueReaper = false;
        public bool plaguebringerPatronSet = false;
        public bool plaguebringerCarapace = false;
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
        public bool miracleBlight = false;
        public bool aCrunch = false;
        public bool crumble = false;
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
        public bool brainRot = false;
        public bool elementalMix = false;
        public bool icarusFolly = false;
        public bool weakPetrification = false;
        public bool vHex = false;
        public bool DoGExtremeGravity = false;
        public bool warped = false;
        public bool cDepth = false;
        public bool rTide = false;
        public bool fishAlert = false;
        public bool clamity = false;
        public bool NOU = false;
        public bool absorberAffliction = false;
        public bool sulphurPoison = false;
        public bool nightwither = false;
        public bool eutrophication = false;
        public bool iCantBreathe = false; //Frozen Lungs debuff
        public bool cragsLava = false;
        public bool vaporfied = false;
        public bool banishingFire = false;
        public bool wither = false;
        public bool ManaBurn = false;

        public int ImmobilityDebuffImmunityTimer = 0;
        public const int ImmobilityDebuffImmunityTimerMax = 300;

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
        public bool brutalCarnage = false;
        public bool omniscience = false;
        public bool zerg = false;
        public bool zen = false;
        public bool isNearbyBoss = false;
        public bool flaskBrimstone = false;
        public bool fabsolVodka = false;
        public bool mushy = false;
        public bool PinkJellyRegen = false;
        public bool GreenJellyRegen = false;
        public bool AbsorberRegen = false;
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
        public bool galvanicCorrosion = false;
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
        public bool oldFashioned = false;
        public bool starBeamRye = false;
        public bool screwdriver = false;
        public bool moscowMule = false;
        public bool whiteWine = false;
        public bool evergreenGin = false;
        public bool tranquilityCandle = false;
        public bool chaosCandle = false;
        public bool blueCandle = false;
        public bool pinkCandle = false;
        public double pinkCandleHealFraction = 0D;
        public bool yellowCandle = false;
        public bool trippy = false;
        public int trippyLevel = 1;
        public bool amidiasBlessing = false;
        public bool bloodfinBoost = false;
        public int bloodfinTimer = 30;
        public bool hallowedRegen = false;
        public bool hallowedPower = false;
        public bool kamiBoost = false;
        public bool avertorBonus = false;
        public bool divineBless = false;
        public bool infiniteFlight = false;
        public int hasteCounter = 0;
        public int hasteLevel = 0;
        #endregion

        #region Minion
        public bool wDroid = false;
        public bool resButterfly = false;
        public bool mWorm = false;
        public bool IceClasperBool = false;
        public bool magicHat = false;
        public bool herring = false;
        public bool blackhawk = false;
        public bool cosmicViper = false;
        public bool CalamarisLament = false;
        public bool cEyes = false;
        public bool cSlime = false;
        public bool cSlime2 = false;
        public bool aSlime = false;
        public bool brittleStar = false;
        public bool aquaticStar = false;
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
        public bool PlantationSummon = false;
        public bool astralProbe = false;
        public bool pSoulGuardians = false;
        public bool cEnergy = false;
        public int healCounter = 300;
        public bool shellfish = false;
        public bool hCrab = false;
        public bool allWaifus = false;
        public bool allWaifusVanity = false;
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
        public bool saros = false;
        public bool plaguebringerMK2 = false;
        public bool igneousExaltation = false;
        public bool GlacialEmbrace = false;
        public bool voidAura = false;
        public bool voidAuraDamage = false;
        public bool voidConcentrationAura = false;
        public bool MutatedTruffleBool = false;
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
        public bool celestialDragons = false;
        public bool KalandraMirror = false;
        public bool StellarTorus = false;
        public bool LiliesOfFinalityBool = false;
        #endregion

        #region Biome
        public bool ZoneCalamity => Player.InModBiome(ModContent.GetInstance<BrimstoneCragsBiome>());
        public bool ZoneAstral => Player.InModBiome(ModContent.GetInstance<BiomeManagers.AstralInfectionBiome>()) && !ZoneAbyss;
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
        public int profanedCrystalStatePrevious;
        public bool profanedCrystalPrevious;
        public int profanedCrystalAnim;
        public bool profanedCrystalForce;
        public bool profanedCrystalBuffs;
        public bool profanedCrystalHide;
        public KeyValuePair<int, int> profanedCrystalWingCounter = new KeyValuePair<int, int>(0, 10);
        public KeyValuePair<int, int> profanedCrystalAnimCounter = new KeyValuePair<int, int>(0, 10);
        public int pscState;
        public Color pscLerpColor = Color.White;
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
        public bool redBow;
        public bool cocosFeather;
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

        public int monolithBossRushShader = 0;

        public int monolithExoShader = 0;

        public int monolithLeviathanShader = 0;

        public int monolithPlagueShader = 0;

        public int monolithCryogenShader = 0;

        public int monolithAstralShader = 0;

        public int monolithDevourerBShader = 0;

        public int monolithDevourerPShader = 0;

        public int monolithYharonShader = 0;

        // This may seem like a scuffed setup, but a simple bool will have ordering issues when it comes to drawing.
        // Until ModSceneMetrics gets implemented, this works for now.
        public int BrimstoneLavaFountainCounter = 0;
        #endregion Draw Effects

        #region Draedon Summoning
        public bool AbleToSelectExoMech = false;
        public bool HasTalkedAtCodebreaker = false;
        public bool HasCraftedDraedonsForge = false;
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
            boost.AddWithCondition("HasCraftedDraedonsForge", HasCraftedDraedonsForge);

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
            HasCraftedDraedonsForge = boost.Contains("HasCraftedDraedonsForge");

            // Load rage if it's there, which it will be for any players saved with 1.5.
            // Older players have "stress" instead, which will be ignored. This is intentional.
            // Stress ranged from 0 to 10,000. Rage ranges from 0.0 to 100.0.
            rage = tag.ContainsKey("rage") ? tag.GetFloat("rage") : 0f;

            if (tag.ContainsKey("adrenaline"))
            {
                object adrenObj = tag["adrenaline"];

                if (adrenObj is float adrenFloat)
                    adrenaline = adrenFloat;
                else if (adrenObj is int adrenInt)
                    adrenaline = adrenInt;
                else
                    adrenaline = 0f;
            }

            if (tag.ContainsKey("aquaticBoostPower"))
                aquaticBoost = tag.GetFloat("aquaticBoostPower");
            sCalDeathCount = tag.GetInt("sCalDeathCount");
            sCalKillCount = tag.GetInt("sCalKillCount");

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

                // If the cooldown has no registered handler, don't add it. Doing so will cause crashes.
                CooldownInstance instance = new CooldownInstance(Player, id, singleCDTag);
                if (instance.handler is not null)
                    cooldowns.Add(id, instance);
            }
        }
        #endregion

        #region ResetEffects
        public override void ResetEffects()
        {
            if (fleshKnuckles)
                Player.statLifeMax2 += 45;

            int percentMaxLifeIncrease = 0;
            if (ZoneAbyss && (abyssalAmulet || lumenousAmulet))
                percentMaxLifeIncrease += lumenousAmulet ? 25 : 10;

            // Blood Pact and Chalice of the Blood God stack their HP bonuses if you want to equip both
            if (bloodPact)
                percentMaxLifeIncrease += 25;
            if (chaliceOfTheBloodGod)
                percentMaxLifeIncrease += 25;

            if (affliction || afflicted)
                percentMaxLifeIncrease += 10;

            if (community)
                percentMaxLifeIncrease += (int)(TheCommunity.CalculatePower() * TheCommunity.HealthMultiplier);

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

            defenseDamageRatio = BalancingConstants.DefaultDefenseDamageRatio;
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

            // Shields. Has to intentionally be above resetting accessories and armor or the shields would clear instantly
            drawnAnyShieldThisFrame = false;
            if (!roverDrive)
                RoverDriveShieldDurability = 0;
            if (!lunicCorpsSet)
                LunicCorpsShieldDurability = 0;
            if (!sponge)
                SpongeShieldDurability = 0;
            if (!pSoulArtifact)
                pSoulShieldDurability = 0;
            pSoulShieldVisible = false;
            roverDrive = false;
            roverDriveShieldVisible = false;
            sponge = false;
            spongeShieldVisible = false;

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
            eidolonSnailPet = false;
            lordePet = false;

            onyxExcavator = false;
            rimehound = false;
            fab = false;
            crysthamyr = false;
            ExoChair = false;
            miniOldDuke = false;
            miniOldDukeVanity = false;

            abyssalDivingSuitPlates = false;

            aquaticHeartWaterBuff = false;
            aquaticHeartIce = false;

            draedonsHeart = false;

            afflicted = false;
            chiRegen = false;
            affliction = false;

            dodgeScarf = false;
            evasionScarf = false;

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
            transformer = false;
            hideOfDeus = false;
            dAmulet = false;
            rampartOfDeities = false;
            gShell = false;
            lAmbergris = false;
            tortShell = false;
            absorber = false;
            alwaysHoneyRegen = false;
            honeyTurboRegen = false;
            honeyDewHalveDebuffs = false;
            livingDewHalveDebuffs = false;
            aAmpoule = false;
            rOoze = false;
            fBarrier = false;
            aBrain = false;
            amalgam = false;
            frostFlare = false;
            uberBees = false;
            evolution = false;
            nanotech = false;
            deadshotBrooch = false;
            cryogenSoul = false;
            ascendantInsignia = false;
            ascendantTrail = false;
            eGauntlet = false;
            gloveLevel = 0;
            alreadyHasFrogLeg = false;
            eTalisman = false;
            nucleogenesis = false;
            nuclearFuelRod = false;
            heartOfDarkness = false;
            shadowMinions = false;
            holyMinions = false;
            alchFlask = false;
            toxicHeart = false;
            abaddon = false;
            aeroStone = false;
            lifejelly = false;
            GrandGelatin = false;
            cleansingjelly = false;
            community = false;
            shatteredCommunity = false;
            stressPills = false;
            laudanum = false;
            fleshTotem = false;
            bloodPact = false;
            bloodflareCore = false;
            chaliceOfTheBloodGod = false;
            chaliceBleedoutToApplyOnHurt = 0; // Resets every frame so it doesn't improperly carry over between hits
            elementalHeart = false;
            crownJewel = false;
            infectedJewel = false;
            purity = false;
            harpyRing = false;
            angelTreads = false;
            harpyWingBoost = false; //harpy wings + harpy ring
            fleshKnuckles = false;
            darkSunRing = false;
            crawCarapace = false;
            baroclaw = false;
            gShell = false;
            lAmbergris = false;
            tortShell = false;
            voidOfCalamity = false;
            voidOfExtinction = false;
            eArtifact = false;
            dArtifact = false;
            auricSArtifact = false;
            pSoulArtifact = false;
            giantPearl = false;
            normalityRelocator = false;
            flameLickedShell = false;
            Pauldron = false;
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
            tracersDust = false;
            elysianWingsDust = false;
            tracersCelestial = false;
            tracersElysian = false;
            tracersSeraph = false;
            ursaSergeant = false;
            scuttlersJewel = false;
            thiefsDime = false;
            dynamoStemCells = false;
            etherealExtorter = false;
            blazingCore = false;
            voltaicJelly = false;
            jellyChargedBattery = false;
            starbusterCore = false;
            starTaintedGenerator = false;
            camper = false;
            silvaWings = false;
            corrosiveSpine = false;
            RustyMedallionDroplets = false;
            MiniSwarmers = false;
            rottenDogTooth = false;
            angelicAlliance = false;
            BloomStoneRegen = false;
            ChaosStone = false;
            CryoStone = false;
            CryoStoneVanity = false;
            voidField = false;
            copyrightInfringementShield = false;
            reducedHolyFlamesDamage = false;
            reducedNightwitherDamage = false;

            daedalusReflect = false;
            daedalusSplit = false;
            daedalusAbsorb = false;
            daedalusShard = false;

            brimflameSet = false;
            brimflameFrenzy = false;

            lunicCorpsSet = false;
            lunicCorpsLegs = false;

            rangedAmmoCost = 1f;
            healingPotionMultiplier = 1f;

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
            flameWakerBoots = false;
            hellfireTreads = false;
            abyssalAmulet = false;
            lumenousAmulet = false;
            oceanCrest = false;
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
            necroSet = false; //vanilla armor
            frostSet = false; //vanilla armor

            victideSet = false;
            victideSummoner = false;

            sulphurSet = false;

            aeroSet = false;

            statigelSet = false;

            titanHeartSet = false;
            titanHeartMask = false;
            titanHeartMantle = false;
            titanHeartBoots = false;
            umbraphileSet = false;
            plagueReaper = false;
            plaguebringerPatronSet = false;
            plaguebringerCarapace = false;
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
            miracleBlight = false;
            aCrunch = false;
            crumble = false;
            irradiated = false;
            bFlames = false;
            witheredDebuff = false;
            absorberAffliction = false;
            weakBrimstoneFlames = false;
            gsInferno = false;
            astralInfection = false;
            pFlames = false;
            hFlames = false;
            hInferno = false;
            gState = false;
            bBlood = false;
            brainRot = false;
            elementalMix = false;
            icarusFolly = false;
            vHex = false;
            DoGExtremeGravity = false;
            warped = false;
            cDepth = false;
            rTide = false;
            fishAlert = false;
            clamity = false;
            NOU = false;
            enraged = false;
            snowmanNoseless = false;
            sulphurPoison = false;
            nightwither = false;
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
            brutalCarnage = false;
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
            galvanicCorrosion = false;
            sulphurskin = false;
            baguette = false;
            trippy = false;
            amidiasBlessing = false;
            flaskBrimstone = false;
            fabsolVodka = false;
            shine = false;
            anechoicCoating = false;
            mushy = false;
            PinkJellyRegen = false;
            GreenJellyRegen = false;
            AbsorberRegen = false;
            cFreeze = false;
            tRegen = false;
            bloodfinBoost = false;
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
            oldFashioned = false;
            starBeamRye = false;
            screwdriver = false;
            moscowMule = false;
            whiteWine = false;
            evergreenGin = false;

            tranquilityCandle = false;
            chaosCandle = false;
            blueCandle = false;
            pinkCandle = false;
            yellowCandle = false;

            wDroid = false;
            resButterfly = false;
            mWorm = false;
            IceClasperBool = false;
            magicHat = false;
            herring = false;
            blackhawk = false;
            cosmicViper = false;
            CalamarisLament = false;
            cEyes = false;
            cSlime = false;
            cSlime2 = false;
            aSlime = false;
            brittleStar = false;
            aquaticStar = false;
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
            pSoulGuardians = false;
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
            PlantationSummon = false;
            astralProbe = false;
            victideSnail = false;
            cSpirit = false;
            rOrb = false;
            dCrystal = false;
            MutatedTruffleBool = false;
            sandWaifu = false;
            sandWaifuVanity = false;
            sandBoobWaifu = false;
            sandBoobWaifuVanity = false;
            cloudWaifu = false;
            cloudWaifuVanity = false;
            brimstoneWaifu = false;
            brimstoneWaifuVanity = false;
            sirenWaifu = false;
            sirenWaifuVanity = false;
            allWaifus = false;
            allWaifusVanity = false;
            fungalClump = false;
            fungalClumpVanity = false;
            howlsHeart = false;
            howlsHeartVanity = false;
            redDevil = false;
            valkyrie = false;
            slimeGod = false;
            chaosSpirit = false;
            daedalusCrystal = false;
            shellfish = false;
            hCrab = false;
            endoHydra = false;
            powerfulRaven = false;
            dragonFamily = false;
            providenceStabber = false;
            plaguebringerMK2 = false;
            igneousExaltation = false;
            GlacialEmbrace = false;
            voidAura = false;
            voidAuraDamage = false;
            voidConcentrationAura = false;
            saros = false;
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
            celestialDragons = false;
            KalandraMirror = false;
            StellarTorus = false;
            LiliesOfFinalityBool = false;

            /* Spawn blockers from back when they used to work by being favorited and not a toggleable item
            noStupidNaturalARSpawns = false
            disableVoodooSpawns = false;
            disablePerfCystSpawns = false;
            disableHiveCystSpawns = false;
            disableNaturalScourgeSpawns = false;
            disableAnahitaSpawns = false;
            */

            abyssalDivingSuitPrevious = abyssalDivingSuit;
            abyssalDivingSuit = abyssalDivingSuitHide = abyssalDivingSuitForce = abyssalDivingSuitPower = false;

            aquaticHeartPrevious = aquaticHeart;
            aquaticHeart = aquaticHeartHide = aquaticHeartForce = aquaticHeartPower = false;

            profanedCrystalStatePrevious = pscState;
            profanedCrystalPrevious = profanedCrystal;
            profanedCrystal = profanedCrystalBuffs = profanedCrystalForce = profanedCrystalHide = false;
            pscState = 0;
            pscLerpColor = Color.White;

            snowmanPrevious = snowman;
            snowman = snowmanHide = snowmanForce = snowmanPower = false;

            meldTransformationPrevious = meldTransformation;
            meldTransformation = meldTransformationForce = meldTransformationPower = false;

            omegaBlueTransformationPrevious = omegaBlueTransformation;
            omegaBlueTransformation = omegaBlueTransformationForce = omegaBlueTransformationPower = false;

            redBow = false;
            cocosFeather = false;

            rageModeActive = false;
            adrenalineModeActive = false;
            AdrenalineTrail = false;
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

            infiniteFlight = false;

            EnchantHeldItemEffects(Player, Player.Calamity(), Player.ActiveItem());
        }
        #endregion

        #region Modify Max Health and Mana
        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            health = StatModifier.Default;
            health.Base = bOrange.ToInt() * BloodOrange.LifeBoost
                        + mFruit.ToInt() * MiracleFruit.LifeBoost
                        + eBerry.ToInt() * Elderberry.LifeBoost
                        + dFruit.ToInt() * Dragonfruit.LifeBoost;

            mana = StatModifier.Default;
            mana.Base = cShard.ToInt() * CometShard.ManaBoost
                        + eCore.ToInt() * EtherealCore.ManaBoost
                        + pHeart.ToInt() * PhantomHeart.ManaBoost;
        }
        #endregion

        #region Screen Position Movements
        public override void ModifyScreenPosition()
        {
            if (CalamityConfig.Instance.ScreenshakePower == 0)
                return;

            if (GeneralScreenShakePower > 0f)
            {
                Main.screenPosition += Main.rand.NextVector2Circular(GeneralScreenShakePower * CalamityConfig.Instance.ScreenshakePower, GeneralScreenShakePower * CalamityConfig.Instance.ScreenshakePower);
                GeneralScreenShakePower = MathHelper.Clamp(GeneralScreenShakePower - 0.185f, 0f, 20f * CalamityConfig.Instance.ScreenshakePower);
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

            #region Defense Damage
            totalDefenseDamage = 0;
            defenseDamageRecoveryFrames = 0;
            totalDefenseDamageRecoveryFrames = DefenseDamageBaseRecoveryTime;
            defenseDamageDelayFrames = 0;
            nextHitDealsDefenseDamage = false;
            bloodflareCoreRemainingHealOverTime = 0;
            #endregion

            #region Debuffs
            heldGaelsLastFrame = false;
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
            necroReviveCounter = -1;
            hideOfDeusTimer = 0;
            murasamaHitCooldown = 0;
            RustyMedallionCooldown = 0;
            SulphWaterPoisoningLevel = 0f;
            holyInfernoFadeIntensity = 0f;
            spiritOriginConvertedCrit = 0;
            rage = 0f;
            adrenaline = 0f;
            raiderCritBonus = 0f;
            raiderSoundCooldown = 0;
            gSabatonHotkeyHoldTime = 0;
            gSabatonFall = 0;
            gSabatonFalling = false;
            gSabatonTempJumpSpeed = 0;
            astralStarRainCooldown = 0;
            AbaddonCooldown = 0;
            AlchFlaskCooldown = 0;
            silvaMageCooldown = 0;
            bloodflareMageCooldown = 0;
            tarraRangedCooldown = 0;
            hideOfDeusMeleeBoostTimer = 0;
            externalAbyssLight = 0;
            externalColdImmunity = externalHeatImmunity = false;
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
            angelicActivate = -1;
            resetHeightandWidth = false;
            noLifeRegen = false;
            alcoholPoisoning = false;
            shadowflame = false;
            wDeath = false;
            dragonFire = false;
            miracleBlight = false;
            aCrunch = false;
            crumble = false;
            irradiated = false;
            bFlames = false;
            witheredDebuff = false;
            absorberAffliction = false;
            weakBrimstoneFlames = false;
            gsInferno = false;
            astralInfection = false;
            pFlames = false;
            hFlames = false;
            hInferno = false;
            gState = false;
            bBlood = false;
            brainRot = false;
            elementalMix = false;
            icarusFolly = false;
            vHex = false;
            DoGExtremeGravity = false;
            warped = false;
            cDepth = false;
            rTide = false;
            fishAlert = false;
            clamity = false;
            NOU = false;
            snowmanNoseless = false;
            abyssalDivingSuitPlateHits = 0;
            sulphurPoison = false;
            nightwither = false;
            eutrophication = false;
            iCantBreathe = false;
            cragsLava = false;
            vaporfied = false;
            banishingFire = false;
            wither = false;
            PurityHealSlowdownFrames = 0;
            ImmobilityDebuffImmunityTimer = 0;
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
            brutalCarnage = false;
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
            adrenalinePauseTimer = 0;
            photosynthesis = false;
            astralInjection = false;
            gravityNormalizer = false;
            flaskHoly = false;
            tesla = false;
            galvanicCorrosion = false;
            sulphurskin = false;
            baguette = false;
            flaskBrimstone = false;
            fabsolVodka = false;
            shine = false;
            anechoicCoating = false;
            mushy = false;
            PinkJellyRegen = false;
            GreenJellyRegen = false;
            AbsorberRegen = false;
            enraged = false;
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
            oldFashioned = false;
            starBeamRye = false;
            screwdriver = false;
            moscowMule = false;
            whiteWine = false;
            evergreenGin = false;
            tranquilityCandle = false;
            chaosCandle = false;
            blueCandle = false;
            pinkCandle = false;
            pinkCandleHealFraction = 0D;
            yellowCandle = false;
            trippy = false;
            trippyLevel = 1;
            amidiasBlessing = false;
            bloodfinBoost = false;
            bloodfinTimer = 0;
            healCounter = 300;
            danceOfLightCharge = 0;
            rangedAmmoCost = 1f;
            healingPotionMultiplier = 1f;
            avertorBonus = false;
            divineBless = false;
            hasteLevel = 0;
            hasteCounter = 0;
            #endregion

            #region Armor Set Bonuses
            silverMedkit = false;
            silverMedkitTimer = 0;
            goldArmorGoldDrops = false;
            miningSet = false;
            miningSetCooldown = 0;
            shadowSpeed = false;
            godSlayer = false;
            godSlayerDamage = false;
            godSlayerRanged = false;
            godSlayerThrowing = false;
            godSlayerDashHotKeyPressed = false;
            SpeedBlasterDashStarted = false;
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
            lunicCorpsSet = false;
            lunicCorpsLegs = false;
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
            necroSet = false; //vanilla armor
            frostSet = false; //vanilla armor
            victideSet = false;
            aeroSet = false;
            sulphurSet = false;
            statigelSet = false;
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
            tracersDust = false;
            elysianWingsDust = false;
            GemTechState.OnDeathEffects();
            blazingCoreParry = 0;
            blazingCoreEmpoweredParry = false;
            blazingCoreSuccessfulParry = 0;
            flameLickedShellParry = 0;
            flameLickedShellEmpoweredParry = false;
            profanedCrystalAnim = -1;
            #endregion

            #region Shields
            RoverDriveShieldDurability = 0;
            LunicCorpsShieldDurability = 0;
            SpongeShieldDurability = 0;
            pSoulShieldDurability = 0;
            #endregion

            #region UI
            CurrentlyViewedFactoryID = -1;
            CurrentlyViewedChargerID = -1;
            CurrentlyViewedHologramID = -1;
            CurrentlyViewedHologramText = string.Empty;
            #endregion

            KameiBladeUseDelay = 0;
            brimlashBusterBoost = false;
            AdrenalineTrail = false;
            ascendantTrail = false;
            evilSmasherBoost = 0;
            hellbornBoost = 0;
            searedPanCounter = 0;
            searedPanTimer = 0;
            potionTimer = 0;
            persecutedEnchantSummonTimer = 0;
            momentumCapacitorTime = 0;
            momentumCapacitorBoost = 0f;
            LungingDown = false;

            chaliceBleedoutBuffer = 0D;
            chaliceDamagePointPartialProgress = 0D;
            chaliceHitOriginalDamage = 0;

            if (BossRushEvent.BossRushActive)
            {
                // https://github.com/tModLoader/tModLoader/wiki/IEntitySource#detailed-list
                // The boss rush visual failure effect has no meaningful source and passes no meaningful information.
                var source = Player.GetSource_None();
                if (Player.whoAmI == 0 && !CalamityGlobalNPC.AnyLivingPlayers() && CalamityUtils.CountProjectiles(ModContent.ProjectileType<BossRushFailureEffectThing>()) == 0)
                    Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ModContent.ProjectileType<BossRushFailureEffectThing>(), 0, 0f);
            }

            // Respawn the player faster
            // 3 seconds normally and configurable while a boss is alive between 15 and 60 seconds
            int respawnTimerSet = areThereAnyDamnBosses ? (CalamityConfig.Instance.PlayerRespawnTime_BossAlive * 60) : 180;
            if (Player.respawnTimer > respawnTimerSet)
                Player.respawnTimer = respawnTimerSet;
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
            // Why does this otherwise work when you're dead lmao
            if (Player.dead)
                return;

            if (ascendantInsignia && Main.myPlayer == Player.whoAmI && CalamityKeybinds.AscendantInsigniaHotKey.JustPressed && ascendantInsigniaCooldown <= 0)
            {
                var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<AscendantInsignia>()));
                Projectile.NewProjectileDirect(source, Player.Center - Vector2.UnitY * 45f, Vector2.Zero, ModContent.ProjectileType<AscendantAura>(), 0, 0f);
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/AscendantActivate"));
                ascendantInsigniaCooldown = 2400;
                ascendantInsigniaBuffTime = 240; //4 seconds
            }

            //Only increment hotkey holdtime if not on ground, not mounted, not on rope, not hooked, not tongued, otherwise reset hold time to zero
            if (CalamityKeybinds.GravistarSabatonHotkey.Current && gSabaton && Main.myPlayer == Player.whoAmI && (Player.velocity.Y != Player.oldVelocity.Y) && !Player.pulley && !Player.mount.Active && Player.grappling[0] == -1 && !Player.tongued)
            {
                gSabatonHotkeyHoldTime++;
                if (gSabatonHotkeyHoldTime < 60 && gSabatonHotkeyHoldTime % 3f == 0)
                {
                    SpawnGravistarParticle();
                }
            }
            else if (Main.myPlayer == Player.whoAmI)
            {
                gSabatonHotkeyHoldTime = 0;
            }

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
                            NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, (float)Player.whoAmI, teleportLocation.X, teleportLocation.Y, 1, 0, 0);

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
                        angelAmt += 1f;
                }

                var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<AngelicAlliance>()));
                for (int projIndex = 0; projIndex < angelAmt; projIndex++)
                {
                    Projectile proj = Main.projectile[projIndex];
                    float start = 360f / angelAmt;
                    int damage = Player.ApplyArmorAccDamageBonusesTo(proj.damage / 10);

                    Projectile.NewProjectile(source, new Vector2((int)(Player.Center.X + (Math.Sin(projIndex * start) * 300)), (int)(Player.Center.Y + (Math.Cos(projIndex * start) * 300))), Vector2.Zero, ModContent.ProjectileType<AngelicAllianceArchangel>(), damage, proj.knockBack / 10f, Player.whoAmI, Main.rand.Next(180), projIndex * start);
                    Player.statLife += 2;
                    Player.HealEffect(2);
                    if (Player.statLife > Player.statLifeMax2)
                        Player.statLife = Player.statLifeMax2;
                }
            }
            if (CalamityKeybinds.SandCloakHotkey.JustPressed && sandCloak && Main.myPlayer == Player.whoAmI && rogueStealth >= rogueStealthMax * 0.1f &&
                wearingRogueArmor && rogueStealthMax > 0 && !Player.HasCooldown(Cooldowns.SandCloak.ID))
            {
                Player.AddCooldown(Cooldowns.SandCloak.ID, CalamityUtils.SecondsToFrames(30));

                var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<Items.Accessories.SandCloak>()));
                rogueStealth -= rogueStealthMax * 0.1f;
                int damage = Player.ApplyArmorAccDamageBonusesTo(7);

                int veil = Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ModContent.ProjectileType<SandCloakVeil>(), damage, 8, Player.whoAmI);
                Main.projectile[veil].Center = Player.Center;
                SoundEngine.PlaySound(SoundID.Item45, Player.Center);
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
                            NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, (float)Player.whoAmI, teleportLocation.X, teleportLocation.Y, 1, 0, 0);

                            int duration = chaosStateDuration;
                            Player.AddBuff(BuffID.ChaosState, duration, true);
                            Player.AddCooldown(ChaosState.ID, duration, true, "spectralveil");

                            int numDust = 40;
                            Vector2 step = teleportOffset / numDust;
                            for (int i = 0; i < numDust; i++)
                            {
                                int dustIndex = Dust.NewDust(Player.Center - (step * i), 1, 1, DustID.VilePowder, step.X, step.Y);
                                Main.dust[dustIndex].noGravity = true;
                                Main.dust[dustIndex].noLight = true;
                            }

                            spectralVeilImmunity = SpectralVeil.VeilIFrames;
                        }
                    }
                }
            }
            if (CalamityKeybinds.BoosterDashHotKey.JustPressed && hasJetpack && Main.myPlayer == Player.whoAmI && rogueStealth >= rogueStealthMax * 0.25f &&
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
            if (CalamityKeybinds.ArmorSetBonusHotKey.JustPressed)
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
                            for (int i = 0; i < 36; i++)
                            {
                                int brimDust = Dust.NewDust(new Vector2(Player.position.X, Player.position.Y + 16f), Player.width, Player.height - 16, (int)CalamityDusts.Brimstone, 0f, 0f, 0, default, 1f);
                                Main.dust[brimDust].velocity *= 3f;
                                Main.dust[brimDust].scale *= 1.15f;
                            }
                            int dustAmt = 36;
                            for (int j = 0; j < dustAmt; j++)
                            {
                                Vector2 dustRotation = Vector2.Normalize(Player.velocity) * new Vector2((float)Player.width / 2f, (float)Player.height) * 0.75f;
                                dustRotation = dustRotation.RotatedBy((double)((float)(j - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Player.Center;
                                Vector2 dustVelocity = dustRotation - Player.Center;
                                int brimDust2 = Dust.NewDust(dustRotation + dustVelocity, 0, 0, (int)CalamityDusts.Brimstone, dustVelocity.X * 1.5f, dustVelocity.Y * 1.5f, 100, default, 1.4f);
                                Main.dust[brimDust2].noGravity = true;
                                Main.dust[brimDust2].noLight = true;
                                Main.dust[brimDust2].velocity = dustVelocity;
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
                        int dust = Dust.NewDust(new Vector2(Player.position.X, Player.position.Y + 16f), Player.width, Player.height - 16, (int)CalamityDusts.Necroplasm, 0f, 0f, 0, default, 1f);
                        Main.dust[dust].velocity *= 3f;
                        Main.dust[dust].scale *= 1.15f;
                    }
                    int dustAmt = 36;
                    for (int d = 0; d < dustAmt; d++)
                    {
                        Vector2 source = Vector2.Normalize(Player.velocity) * new Vector2((float)Player.width / 2f, (float)Player.height) * 0.75f;
                        source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Player.Center;
                        Vector2 dustVel = source - Player.Center;
                        int phanto = Dust.NewDust(source + dustVel, 0, 0, (int)CalamityDusts.Necroplasm, dustVel.X * 1.5f, dustVel.Y * 1.5f, 100, default, 1.4f);
                        Main.dust[phanto].noGravity = true;
                        Main.dust[phanto].noLight = true;
                        Main.dust[phanto].velocity = dustVel;
                    }
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(Player.velocity.X, Player.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;

                    int damage = (int)(Player.GetTotalDamage<RangedDamageClass>().ApplyTo(300f));
                    damage = Player.ApplyArmorAccDamageBonusesTo(damage);

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
                        int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.PurificationPowder, 0, 0, 100, Color.Transparent, 2.6f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].fadeIn = 1f;
                        Main.dust[d].velocity *= 6.6f;
                    }
                }
                if (dsSetBonus)
                {
                    SoundEngine.PlaySound(DemonshadeHelm.ActivationSound, Player.Center);
                    for (int i = 0; i < 36; i++)
                    {
                        int brimDust = Dust.NewDust(new Vector2(Player.position.X, Player.position.Y + 16f), Player.width, Player.height - 16, (int)CalamityDusts.Brimstone, 0f, 0f, 0, default, 1f);
                        Main.dust[brimDust].velocity *= 3f;
                        Main.dust[brimDust].scale *= 1.15f;
                    }
                    int dustAmt = 36;
                    for (int j = 0; j < dustAmt; j++)
                    {
                        Vector2 dustRotation = Vector2.Normalize(Player.velocity) * new Vector2((float)Player.width / 2f, (float)Player.height) * 0.75f;
                        dustRotation = dustRotation.RotatedBy((double)((float)(j - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Player.Center;
                        Vector2 dustVelocity = dustRotation - Player.Center;
                        int brimDust2 = Dust.NewDust(dustRotation + dustVelocity, 0, 0, (int)CalamityDusts.Brimstone, dustVelocity.X * 1.5f, dustVelocity.Y * 1.5f, 100, default, 1.4f);
                        Main.dust[brimDust2].noGravity = true;
                        Main.dust[brimDust2].noLight = true;
                        Main.dust[brimDust2].velocity = dustVelocity;
                    }
                    if (Player.whoAmI == Main.myPlayer)
                    {
                        Player.AddBuff(ModContent.BuffType<Enraged>(), 600, false);
                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        foreach (NPC npc in Main.ActiveNPCs)
                        {
                            if (!npc.friendly && !npc.dontTakeDamage && Vector2.Distance(Player.Center, npc.Center) <= 3000f)
                                npc.AddBuff(ModContent.BuffType<Enraged>(), 600, false);
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
                        damage = Player.ApplyArmorAccDamageBonusesTo(damage);

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

            if (CalamityKeybinds.AccessoryParryHotKey.JustPressed)
            {
                if (blazingCore && blazingCoreParry == 0 && blazingCoreSuccessfulParry == 0)
                {
                    //minor cheese prevention with standing on a spike with later game gear spamming parry :skull:
                    //because of ordering, if they do not have the cooldown, it will not check the projectile array. Likewise if there are no bosses alive.
                    //Furthermore, Enumerable#Any is lightweight and returns immediately if a single object matches it's predicate
                    if (!Player.HasCooldown(ParryCooldown.ID) || Player.ownedProjectileCounts[ModContent.ProjectileType<BlazingStarHeal>()] == 0)
                    {
                        GeneralScreenShakePower = 3.5f;
                        blazingCoreParry = 30;
                        SoundEngine.PlaySound(BlazingCore.ParryActivateSound, Player.Center);
                        var mySourceIsIMadeItUp = Player.GetSource_FromThis();
                        int blazingSun = Projectile.NewProjectile(mySourceIsIMadeItUp, Player.Center, Vector2.Zero, ModContent.ProjectileType<BlazingSun>(), 0, 0f, Player.whoAmI, 0f, 0f);
                        Main.projectile[blazingSun].Center = Player.Center;
                        int blazingSun2 = Projectile.NewProjectile(mySourceIsIMadeItUp, Player.Center, Vector2.Zero, ModContent.ProjectileType<BlazingSun2>(), 0, 0f, Player.whoAmI, 0f, 0f);
                        Main.projectile[blazingSun2].Center = Player.Center;
                    }
                }
                else if (flameLickedShell && flameLickedShellParry == 0)
                {
                    if (!Player.HasCooldown(ParryCooldown.ID) || Player.ownedProjectileCounts[ModContent.ProjectileType<FlameLickedBarrage>()] == 0)
                    {
                        GeneralScreenShakePower = 2.5f;
                        SoundEngine.PlaySound(ProfanedGuardianDefender.RockShieldSpawnSound, Player.Center);
                        flameLickedShellParry = FlameLickedShell.flameLickedParry;
                    }
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

            //Right click dash on Speed Blaster
            if (sBlasterDashActivated == true)
            {
                if ((Player.controlUp || Player.controlDown || Player.controlLeft || Player.controlRight) && !Player.pulley && Player.grappling[0] == -1 && !Player.tongued && !Player.mount.Active && Player.HasCooldown(Cooldowns.SpeedBlasterBoost.ID) && Player.dashDelay == 0)
                {
                    SpeedBlasterDashStarted = true;
                }
                sBlasterDashActivated = false;
            }

            if (Player.Calamity().SpeedBlasterDashStarted || (Player.dashDelay != 0 && Player.Calamity().LastUsedDashID == SpeedBlasterDash.ID))
            {
                Player.Calamity().DeferredDashID = SpeedBlasterDash.ID;
                Player.dash = 0;
            }

            // Trigger for pressing the Rage hotkey.
            if (CalamityKeybinds.RageHotKey.JustPressed)
            {
                // Gael's Greatsword replaces Rage Mode with an uber skull attack
                if (!(Player.HasCooldown(Cooldowns.GaelsRage.ID)) && Player.ActiveItem().type == ModContent.ItemType<GaelsGreatsword>() && rage > 0f)
                {
                    SoundEngine.PlaySound(SilvaHeadSummon.DispelSound, Player.Center);

                    for (int i = 0; i < 3; i++)
                        Dust.NewDust(Player.position, 120, 120, DustID.Rain_BloodMoon, 0f, 0f, 100, default, 1.5f);
                    for (int i = 0; i < 30; i++)
                    {
                        float angle = MathHelper.TwoPi * i / 30f;
                        int dustIndex = Dust.NewDust(Player.position, 120, 120, DustID.Rain_BloodMoon, 0f, 0f, 0, default, 2f);
                        Main.dust[dustIndex].noGravity = true;
                        Main.dust[dustIndex].velocity *= 4f;
                        dustIndex = Dust.NewDust(Player.position, 120, 120, DustID.Rain_BloodMoon, 0f, 0f, 100, default, 1f);
                        Main.dust[dustIndex].velocity *= 2.25f;
                        Main.dust[dustIndex].noGravity = true;
                        Dust.NewDust(Player.Center + angle.ToRotationVector2() * 160f, 0, 0, DustID.Rain_BloodMoon, 0f, 0f, 100, default, 1f);
                    }

                    // https://github.com/tModLoader/tModLoader/wiki/IEntitySource#detailed-list
                    var source = Player.GetSource_ItemUse(Player.ActiveItem(), GaelsGreatsword.SkullsplosionEntitySourceContext);
                    float rageRatio = rage / rageMax;
                    float baseDamage = rageRatio * GaelsGreatsword.SkullsplosionDamageMultiplier * GaelsGreatsword.BaseDamage;
                    int damage = (int)Player.GetTotalDamage<MeleeDamageClass>().ApplyTo(baseDamage);
                    float skullCount = 14f + (rageBoostOne ? 4f : 0f) + (rageBoostTwo ? 4f : 0f) + (rageBoostThree ? 4f : 0f);
                    float skullSpeed = 12f;
                    for (float i = 0; i < skullCount; i += 1f)
                    {
                        float angle = MathHelper.TwoPi * i / skullCount;
                        Vector2 initialVelocity = angle.ToRotationVector2().RotatedByRandom(MathHelper.ToRadians(12f)) * skullSpeed * new Vector2(0.82f, 1.5f) *
                            Main.rand.NextFloat(0.8f, 1.2f) * (i < skullCount / 2 ? 0.25f : 1f);
                        int projectileIndex = Projectile.NewProjectile(source, Player.Center + initialVelocity * 3f, initialVelocity, ModContent.ProjectileType<GaelSkull2>(), damage, 2f, Player.whoAmI);
                        Main.projectile[projectileIndex].tileCollide = false;
                        Main.projectile[projectileIndex].localAI[1] = (Main.projectile[projectileIndex].velocity.Y < 0f).ToInt();
                        if (projectileIndex.WithinBounds(Main.maxProjectiles))
                            Main.projectile[projectileIndex].DamageType = DamageClass.Generic;
                    }

                    // Remove all rage when the special attack is used, and apply the cooldown.
                    rage = 0f;
                    Player.AddCooldown(Cooldowns.GaelsRage.ID, 1800);
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
                        int dustID = electricity ? (Main.rand.NextBool() ? 132 : 131) : ModContent.DustType<AdrenDust>();

                        float interpolant = i + 0.5f;
                        float spreadSpeed = Main.rand.NextFloat(0.5f, maxDustVelSpread);
                        if (electricity)
                            spreadSpeed *= 4f;

                        Vector2 segmentOnePos = Player.Center + segmentOneStart + segmentOneIncrement * interpolant;
                        Dust d = Dust.NewDustPerfect(segmentOnePos, dustID, Vector2.Zero);
                        if (electricity)
                            d.noGravity = false;
                        d.scale = Main.rand.NextFloat(1.2f, 1.8f);
                        d.velocity = Main.rand.NextVector2Unit() * spreadSpeed;

                        Vector2 segmentTwoPos = Player.Center + segmentTwoStart + segmentTwoIncrement * interpolant;
                        d = Dust.CloneDust(d);
                        d.position = segmentTwoPos;
                        d.scale = Main.rand.NextFloat(1.2f, 1.8f);
                        d.velocity = Main.rand.NextVector2Unit() * spreadSpeed;

                        Vector2 segmentThreePos = Player.Center + segmentThreeStart + segmentThreeIncrement * interpolant;
                        d = Dust.CloneDust(d);
                        d.position = segmentThreePos;
                        d.scale = Main.rand.NextFloat(1.2f, 1.8f);
                        d.velocity = Main.rand.NextVector2Unit() * spreadSpeed;
                    }
                }
            }
        }
        #endregion

        #region TeleportMethods
        // Used for Boss Rush WoF
        public static Vector2? GetUnderworldPosition(Player player)
        {
            bool canSpawn = false;
            int halfWorldXTiles = Main.maxTilesX / 2;
            int largerCheckRadius = 100;
            int smallerCheckRadius = 50;
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

            Vector2 vector = player.CheckForGoodTeleportationSpot(ref canSpawn, halfWorldXTiles - smallerCheckRadius, largerCheckRadius, teleportStartY, teleportRangeY, settings);
            if (!canSpawn)
                vector = player.CheckForGoodTeleportationSpot(ref canSpawn, halfWorldXTiles - largerCheckRadius, smallerCheckRadius, teleportStartY, teleportRangeY, settings);

            if (!canSpawn)
                vector = player.CheckForGoodTeleportationSpot(ref canSpawn, halfWorldXTiles + smallerCheckRadius, smallerCheckRadius, teleportStartY, teleportRangeY, settings);

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
            NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, (float)player.whoAmI, pos.X, pos.Y, style, 0, 0);
            player.velocity = Vector2.Zero;
            player.immune = postImmune;
            player.immuneTime = postImmuneTime;

            // Make some dust
            for (int index = 0; index < 100; ++index)
            {
                Main.dust[Dust.NewDust(player.position, player.width, player.height, DustID.TeleportationPotion, player.velocity.X * 0.2f, player.velocity.Y * 0.2f, 150, Color.Cyan, 1.2f)].velocity *= 0.5f;
            }
            Rectangle rect = player.getRect();
            int dustAmt = rect.Width * rect.Height / 5;
            for (int k = 0; k < dustAmt; k++)
            {
                int idx = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, DustID.TeleportationPotion);
                Main.dust[idx].scale = Main.rand.NextFloat(0.2f, 0.7f);
                if (k < 10)
                    Main.dust[idx].scale += 0.25f;
                if (k < 5)
                    Main.dust[idx].scale += 0.25f;
            }
            for (int k = 0; k < 50; k++)
            {
                int idx = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, DustID.DungeonSpirit);
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
                if (Main.rand.NextBool())
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

            // Putting this in GlobalItem will run multiple times for each slot, which this system already does, creating a slew of problems.
            VanillaArmorChangeManager.ApplyPotentialEffectsTo(Player);

            // If the config is enabled, vastly increase the player's base tile and wall placement speeds
            // This stacks with the Brick Layer and Portable Cement Mixer
            if (CalamityConfig.Instance.FasterTilePlacement)
            {
                Player.tileSpeed += 0.5f;
                Player.wallSpeed += 0.5f;
            }

            // Takes the movement speed bonus and uses it to increase run speed
            float accRunSpeedMin = Player.accRunSpeed * 0.5f;
            Player.accRunSpeed += Player.accRunSpeed * moveSpeedBonus * 0.2f;
            if (Player.accRunSpeed < accRunSpeedMin)
                Player.accRunSpeed = accRunSpeedMin;

            //Life Jelly regen aura spawn when using a healing potion
            if (timePotionSick == 1 && Player.whoAmI == Main.myPlayer && lifejelly && !GrandGelatin)
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<PinkJellyAura>(), 0, 0, Player.whoAmI);

            //Cleansing Jelly cleansing aura spawn when using a healing potion
            if (timePotionSick == 1 && Player.whoAmI == Main.myPlayer && cleansingjelly && !GrandGelatin)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<BlueJellyAura>(), 0, 0, Player.whoAmI);
            }
            //Grand Gellatin regen and cleansing aura spawn when using a healing potion
            if (timePotionSick == 1 && Player.whoAmI == Main.myPlayer && GrandGelatin && !absorber)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<GreenJellyAura>(), 0, 0, Player.whoAmI);
            }
            //Absorber's regen, cleansing, and buffing aura spawn when using a healing potion
            if (timePotionSick == 1 && Player.whoAmI == Main.myPlayer && absorber)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<AbsorberAura>(), 0, 0, Player.whoAmI);
            }

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
            if (gSabaton && Player.whoAmI == Main.myPlayer)
            {
                //While holding hotkey, but before slam, bring Y velocity closer to 0
                if (gSabatonHotkeyHoldTime < 60 && gSabatonHotkeyHoldTime != 0 && !gSabatonFalling)
                {
                    Player.velocity.Y *= (60 - (gSabatonHotkeyHoldTime / 4f)) / 60f;
                }
                //Play sound a bit early so it goes in time with the fall
                if (gSabatonHotkeyHoldTime == 45 && !gSabatonFalling)
                {
                    SoundEngine.PlaySound(new("CalamityMod/Sounds/Custom/GravistarCharge") { Volume = 0.3f });
                }
                //1 second passed, falling time
                if (gSabatonHotkeyHoldTime == 60)
                {
                    gSabatonFalling = true;
                }
                //Cancel fall and don't give 'on ground' effects if on rope, on mount, grappled, or tongued
                if (Player.pulley || Player.mount.Active || Player.grappling[0] != -1 || Player.tongued)
                {
                    gSabatonFall = 0;
                    gSabatonFalling = false;
                }
                if (gSabatonFalling)
                {
                    SpawnGravistarParticle();

                    //Cap time converted to damage at 2 seconds
                    if (gSabatonFall < 120)
                        gSabatonFall++;

                    Player.maxFallSpeed = 40f;
                    Player.gravity = 1.3f;
                    //If the player can fly during the fall, the physics gets a bit funky
                    Player.controlJump = false;

                    //Check if player hit some form of solid resistance (the ground)
                    if (Player.oldVelocity.Y == Player.velocity.Y)
                    {
                        var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<GravistarSabaton>()));
                        //Spawn explosion. ai[0] is used for transferring the recorded falling time

                        int damage = Player.ApplyArmorAccDamageBonusesTo(Player.CalcIntDamage<MeleeDamageClass>(GravistarSabaton.SlamDamage));

                        Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ModContent.ProjectileType<SabatonSlam>(), damage, 4f, Player.whoAmI, gSabatonFall);
                        gSabatonFall = 0;
                        gSabatonFalling = false;
                        //Temporary jump speed is granted for 40 frames
                        gSabatonTempJumpSpeed = 40;
                    }
                }

            }
        }
        #endregion

        #region PreUpdate
        public override void PreUpdate()
        {
            //Infinite flight granted by some boss attacks
            if (infiniteFlight)
                Player.wingTime = Player.wingTimeMax;

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
                    GameShaders.Armor.GetSecondaryShader(Player.dye[i].dye, Player)?.UseColor(GetCurrentMoonlightDyeColor());
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
            if (Player.whoAmI == Main.myPlayer && ExoChair)
            {
                float speed = DraedonGamerChairMount.MovementSpeed;

                if (Player.controlLeft)
                {
                    Player.velocity.X = -speed;
                    Player.ChangeDir(-1);
                }
                else if (Player.controlRight)
                {
                    Player.velocity.X = speed;
                    Player.ChangeDir(1);
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
                if (Player.whoAmI == Main.myPlayer && Player.potionDelay != 0)
                    potionSick = true;
                else
                    potionSick = false;

                if (!potionSick)
                    timePotionSick = 0;
                else
                    timePotionSick++;

                // Add a cooldown display for potion sickness if the player has the vanilla counter ticking
                if (potionSick && !Player.HasCooldown(PotionSickness.ID))
                    Player.AddCooldown(PotionSickness.ID, Player.potionDelay, false);

                if (cooldowns.TryGetValue(PotionSickness.ID, out CooldownInstance cd))
                {
                    if (Player.potionDelay != cd.timeLeft && cd.timeLeft > 0)
                        cd.timeLeft = Player.potionDelay;

                    if (cd.timeLeft > cd.duration)
                        cd.duration = cd.timeLeft; // If the new cooldown is larger than the full duration, update, else keep it the same.
                }

                // Add a cooldown display for chaos state if the player has the vanilla counter ticking
                // This will make the cooldown look like vanilla Rod of Discord, as it wasn't applied by either Normality Relocator or Spectral Veil
                if (Player.chaosState && !Player.HasCooldown(ChaosState.ID))
                {
                    for (int l = 0; l < Player.MaxBuffs; l++)
                    {
                        if (Player.buffType[l] == BuffID.ChaosState)
                        {
                            Player.AddCooldown(ChaosState.ID, Player.buffTime[l], false);
                            break;
                        }
                    }
                }
            }

            // Add a cooldown display for the vanilla lifesteal cooldown, which is active when negative.
            if (Player.whoAmI == Main.myPlayer)
            {
                float baseRecoveryRate = Main.expertMode ? BalancingConstants.LifeStealRecoveryRate_Expert : BalancingConstants.LifeStealRecoveryRate_Classic;

                float lifeStealRecoveryRateReduction =
                    CalamityWorld.death ? BalancingConstants.LifeStealRecoveryRateReduction_Death :
                    CalamityWorld.revenge ? BalancingConstants.LifeStealRecoveryRateReduction_Revengeance :
                    Main.expertMode ? BalancingConstants.LifeStealRecoveryRateReduction_Expert :
                    BalancingConstants.LifeStealRecoveryRateReduction_Classic;

                if (Main.masterMode)
                    lifeStealRecoveryRateReduction += BalancingConstants.LifeStealRecoveryRateReduction_Master;

                float lifeStealRecoveryRate = baseRecoveryRate - lifeStealRecoveryRateReduction;
                if (Player.lifeSteal < -lifeStealRecoveryRate)
                {
                    int duration = (int)Math.Ceiling(Math.Abs(Player.lifeSteal) / lifeStealRecoveryRate);
                    if (!Player.HasCooldown(LifeSteal.ID) || (cooldowns[LifeSteal.ID].duration < duration))
                        Player.AddCooldown(LifeSteal.ID, duration);
                }
            }

            ForceVariousEffects();
        }
        #endregion

        #region PostUpdateEquips
        public override void PostUpdateEquips()
        {
            // True melee damage from various vanilla equipment placed here.

            // Titan Glove and ALL upgrades.
            if (Player.kbGlove)
                Player.GetDamage<TrueMeleeDamageClass>() += 0.1f;

            ForceVariousEffects();
            BaseIdleHoldoutProjectile.CheckForEveryHoldout(Player);

            if (gSabatonTempJumpSpeed > 0)
            {
                gSabatonTempJumpSpeed--;
                // Only give temporary jump speed if Gravistar Sabaton is equipped, but still decrement the time so that you can't store it for later.
                if (gSabaton && Player.whoAmI == Main.myPlayer)
                    Player.jumpSpeedBoost += 2f;
            }
        }
        #endregion

        #region PostUpdate
        #region Shop Restrictions
        public override bool CanSellItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (item.type == ModContent.ItemType<ProfanedSoulCrystal>())
                return DownedBossSystem.downedCalamitas && DownedBossSystem.downedExoMechs; //no easy moneycoins for post doggo/yhar
            return base.CanSellItem(vendor, shopInventory, item);
        }

        #endregion

        public override void PostUpdateRunSpeeds()
        {
            #region SpeedBoosts
            if (!Player.mount.Active)
            {
                float runAccMult = 1f +
                    (lunicCorpsLegs ? 0.1f : 0f) +
                    (shadowSpeed ? 0.5f : 0f) +
                    (stressPills ? 0.05f : 0f) +
                    ((abyssalDivingSuit && Player.IsUnderwater()) ? 0.05f : 0f) +
                    (aquaticHeartWaterBuff ? 0.15f : 0f) +
                    ((frostFlare && Player.statLife <= (int)(Player.statLifeMax2 * 0.5)) ? 0.15f : 0f) +
                    (dragonScales ? 0.1f : 0f) +
                    (kamiBoost ? KamiBuff.RunAccelerationBoost : 0f) +
                    (CobaltSet ? CobaltArmorSetChange.SpeedBoostSetBonusPercentage * 0.01f : 0f) +
                    (silvaSet ? 0.05f : 0f) +
                    (blueCandle ? CirrusBlueCandleBuff.AccelerationBoost : 0f) +
                    (planarSpeedBoost > 0 ? (0.01f * planarSpeedBoost) : 0f) +
                    (hasteLevel * 0.05f);

                float runSpeedMult = 1f +
                    (lunicCorpsLegs ? 0.1f : 0f) +
                    (shadowSpeed ? 0.5f : 0f) +
                    (stressPills ? 0.05f : 0f) +
                    ((abyssalDivingSuit && Player.IsUnderwater()) ? 0.05f : 0f) +
                    (aquaticHeartWaterBuff ? 0.15f : 0f) +
                    ((frostFlare && Player.statLife <= (int)(Player.statLifeMax2 * 0.5)) ? 0.15f : 0f) +
                    (dragonScales ? 0.1f : 0f) +
                    (kamiBoost ? KamiBuff.RunSpeedBoost : 0f) +
                    (CobaltSet ? CobaltArmorSetChange.SpeedBoostSetBonusPercentage * 0.01f : 0f) +
                    (silvaSet ? 0.05f : 0f) +
                    (planarSpeedBoost > 0 ? (0.01f * planarSpeedBoost) : 0f) +
                    (hasteLevel * 0.05f);

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

        #region On Respawn
        public override void OnRespawn()
        {
            healToFull = true;

            // The player rotation can be off if the player dies at the right time when using Final Dawn.
            Player.fullRotation = 0f;
        }
        #endregion

        #region Get Heal Life
        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            healValue = (int)(healValue * healingPotionMultiplier);
        }
        #endregion

        #region Get Weapon Damage And KB
        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            if (fungalSymbiote && CalamityLists.MushroomWeaponIDs.Contains(item.type))
                damage += 0.1f;

            if (item.CountsAsClass<RogueDamageClass>())
            {
                // Apply weapon modifier stealth strike damage bonus
                // 01OCT2023: Ozzatron: This is a multiplicative bonus because it is a prefix.
                // It should be equivalent to x1.15 (or whatever multiplier) on the base damage of the weapon for stealth only.
                if (item.Calamity().StealthStrikePrefixBonus != 0f && StealthStrikeAvailable())
                    damage *= item.Calamity().StealthStrikePrefixBonus; // This number centers on 1f, so 1.15f = 1.15x damage.
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
            if (CalamityLists.MagicGunIDs.Contains(item.type) && meteorSet)
            {
                mult *= 0.33f;
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
                        int confetti = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, confettiDust, Player.velocity.X, Player.velocity.Y, 0, new Color(), 1.2f);
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
                        Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, Main.rand.NextBool(3) ? 114 : ModContent.DustType<BrimstoneFlame>(), Player.velocity.X * 0.2f + Player.direction * 3f, Player.velocity.Y * 0.2f, 100, default, Main.rand.NextFloat(0.3f, 1f));
                    }
                }
                if (flaskCrumbling)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, Main.rand.NextBool() ? 121 : DustID.Stone, Player.velocity.X * 0.2f + Player.direction * 3f, Player.velocity.Y * 0.2f, 100, default, Main.rand.NextFloat(0.2f, 0.7f));
                    }
                }
                if (eGauntlet)
                {
                    if (Main.rand.NextBool(3))
                    {
                        int element = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.RainbowTorch, Player.velocity.X * 0.2f + Player.direction * 3f, Player.velocity.Y * 0.2f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.25f);
                        Main.dust[element].noGravity = true;
                    }
                }
                if (cryogenSoul)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.IceRod, Player.velocity.X * 0.2f + Player.direction * 3f, Player.velocity.Y * 0.2f, 100, default, 0.75f);
                    }
                }
                if (xerocSet)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Enchanted_Pink, Player.velocity.X * 0.2f + Player.direction * 3f, Player.velocity.Y * 0.2f, 100, default, 1.25f);
                    }
                }
                if (dsSetBonus)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Shadowflame, Player.velocity.X * 0.2f + Player.direction * 3f, Player.velocity.Y * 0.2f, 100, default, 2.5f);
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
                    if (!CalamityLists.VeneratedLocketBanlist.Contains(item.type))
                    {
                        float veneratedCloneSpeed = item.shootSpeed;
                        Vector2 realPlayerPos = Player.RotatedRelativePoint(Player.MountedCenter, true);
                        float veneratedCloneXPos = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
                        float veneratedCloneYPos = (float)Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
                        if (Player.gravDir == -1f)
                        {
                            veneratedCloneYPos = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - realPlayerPos.Y;
                        }
                        float veneratedCloneDistance = (float)Math.Sqrt((double)(veneratedCloneXPos * veneratedCloneXPos + veneratedCloneYPos * veneratedCloneYPos));
                        if ((float.IsNaN(veneratedCloneXPos) && float.IsNaN(veneratedCloneYPos)) || (veneratedCloneXPos == 0f && veneratedCloneYPos == 0f))
                        {
                            veneratedCloneXPos = (float)Player.direction;
                            veneratedCloneYPos = 0f;
                            veneratedCloneDistance = veneratedCloneSpeed;
                        }
                        else
                        {
                            veneratedCloneDistance = veneratedCloneSpeed / veneratedCloneDistance;
                        }

                        realPlayerPos = new Vector2(Player.position.X + (float)Player.width * 0.5f + (float)(Main.rand.Next(201) * -(float)Player.direction) + ((float)Main.mouseX + Main.screenPosition.X - Player.position.X), Player.MountedCenter.Y - 600f);
                        realPlayerPos.X = (realPlayerPos.X + Player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                        realPlayerPos.Y -= 100f;
                        veneratedCloneXPos = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
                        veneratedCloneYPos = (float)Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
                        if (veneratedCloneYPos < 0f)
                        {
                            veneratedCloneYPos *= -1f;
                        }
                        if (veneratedCloneYPos < 20f)
                        {
                            veneratedCloneYPos = 20f;
                        }
                        veneratedCloneDistance = (float)Math.Sqrt((double)(veneratedCloneXPos * veneratedCloneXPos + veneratedCloneYPos * veneratedCloneYPos));
                        veneratedCloneDistance = veneratedCloneSpeed / veneratedCloneDistance;
                        veneratedCloneXPos *= veneratedCloneDistance;
                        veneratedCloneYPos *= veneratedCloneDistance;
                        float speedX4 = veneratedCloneXPos + (float)Main.rand.Next(-30, 31) * 0.02f;
                        float speedY5 = veneratedCloneYPos + (float)Main.rand.Next(-30, 31) * 0.02f;

                        // 08DEC2023: Ozzatron: Locket + Old Fashioned may need to be a corner case. We should probably just rework Locket instead.
                        int locketDamage = Player.ApplyArmorAccDamageBonusesTo((int)(damage * 0.07f));

                        int p = Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, speedX4, speedY5, type, locketDamage, knockBack * 0.5f, Player.whoAmI);

                        if (p.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[p].DamageType = DamageClass.Generic; //Makes it not proc shit like nanotech, extorter and other stuff
                            Main.projectile[p].Calamity().LocketClone = true; //To not have clones trigger effects like Sacrifice's Lifesteal and Final Dawn's stealth generation
                        }

                        // Handle AI edge-cases. These are like overlapping projectiles and the projectile not spawning at all
                        if (item.type == ModContent.ItemType<TheFinalDawn>())
                            Main.projectile[p].ai[1] = 1f; //MUST BE 1 OTHERWISE CLONES GENERATE STEALTH AAAAAAAAAAA
                        if (item.type == ModContent.ItemType<TheAtomSplitter>())
                            Main.projectile[p].ai[0] = -1f;
                    }

                    if (StealthStrikeAvailable())
                    {
                        int knifeCount = 12;
                        int knifeDamage = (int)Player.GetTotalDamage<RogueDamageClass>().ApplyTo(55);
                        knifeDamage = Player.ApplyArmorAccDamageBonusesTo(knifeDamage);

                        float angleStep = MathHelper.TwoPi / knifeCount;
                        float speed = 14f;

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
                    int d = (int)Player.GetTotalDamage<RangedDamageClass>().ApplyTo(RustyMedallion.AcidDropBaseDamage);
                    d = Player.ApplyArmorAccDamageBonusesTo(d);

                    Vector2 startingPosition = Main.MouseWorld - Vector2.UnitY.RotatedByRandom(0.3f) * 1250f;
                    Vector2 directionToMouse = (Main.MouseWorld - startingPosition).SafeNormalize(Vector2.UnitX);
                    int drop = Projectile.NewProjectile(source, startingPosition, directionToMouse * 15f, ModContent.ProjectileType<AcidBarrelDrop>(), d, 0f, Player.whoAmI, 3);
                    if (drop.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[drop].penetrate = 2;
                        Main.projectile[drop].DamageType = DamageClass.Generic;
                    }
                    RustyMedallionCooldown = RustyMedallion.AcidCreationCooldown;
                }
            }

            if (MiniSwarmers && MiniSwamerCooldown <= 0)
            {
                if (item.CountsAsClass<RangedDamageClass>() && !item.channel)
                {
                    int newDamage = (int)(damage * (6 - 5 * (item.useTime >= 25 ? 1 : item.useTime / 25)));
                    newDamage = Player.ApplyArmorAccDamageBonusesTo(newDamage);
                    Projectile.NewProjectile(source, position, velocity * 1.25f, ModContent.ProjectileType<MiniatureFolly>(), newDamage, 2f, Player.whoAmI);

                    MiniSwamerCooldown = DynamoStemCells.MiniSwamerCooldown;
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
                Player.legs = EquipLoader.GetEquipSlot(Mod, Main.dayTime ? "ProfanedSoulCrystal" : "PscNightLegs", EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, "ProfanedSoulCrystal", EquipType.Body);
                Player.head = EquipLoader.GetEquipSlot(Mod, Main.dayTime ? "ProfanedSoulCrystal" : "PscNightHead", EquipType.Head);
                Player.wings = EquipLoader.GetEquipSlot(Mod, Main.dayTime ? "ProfanedSoulCrystal" : "PscNightWings", EquipType.Wings);
                Player.face = -1;

                bool enrage = pscState >= (int)ProfanedSoulCrystal.ProfanedSoulCrystalState.Enraged;

                if (profanedCrystalWingCounter.Value == 0)
                {
                    int key = profanedCrystalWingCounter.Key;
                    profanedCrystalWingCounter = new KeyValuePair<int, int>(key == 3 ? 0 : key + 1, enrage ? 5 : 8);
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

            if (redBow)
            {
                Player.legs = EquipLoader.GetEquipSlot(Mod, "RedBow", EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, "RedBow", EquipType.Body);
                Player.head = EquipLoader.GetEquipSlot(Mod, "RedBow", EquipType.Head);
            }
            if (cocosFeather)
            {
                Player.legs = EquipLoader.GetEquipSlot(Mod, "CocosFeather", EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, "CocosFeather", EquipType.Body);
                Player.head = EquipLoader.GetEquipSlot(Mod, "CocosFeather", EquipType.Head);
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
            if (NOU)
            {
                NOULOL();
            }
        }
        #endregion

        #region Limitations
        private void ForceVariousEffects()
        {
            if (blockAllDashes)
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

            // Disable vanilla dashes during god slayer dash
            if (SpeedBlasterDashStarted)
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
                    float unclampedAuroraPower = totalMoonlightDyes / 3f;
                    float timeScalar1 = Main.GlobalTimeWrappedHourly * 0.56f;
                    float timeScalar2 = Main.GlobalTimeWrappedHourly * 0.32f;
                    float timeScalar3 = Main.GlobalTimeWrappedHourly * 0.91f;
                    Vector2 velocityScale = new Vector2(0.15f, 1f);
                    Vector2 playerVelocityOffset = Vector2.UnitX * Player.velocity.X / 9f;
                    Vector2 drawPosition = Main.LocalPlayer.Center - Main.screenPosition;
                    Vector2 auroraOffset = drawPosition - Vector2.UnitY * 15f;
                    int origin = size / 2;
                    float auroraPower = MathHelper.Clamp(unclampedAuroraPower, 0f, 1f);
                    for (int i = 0; i < auroraCount; i++)
                    {
                        float offsetAngle = MathHelper.TwoPi * i / auroraCount + timeScalar1;
                        Color auroraColor = GetCurrentMoonlightDyeColor(offsetAngle) * 0.8f;
                        auroraColor.A = 0;

                        Vector2 auroraVelocity = (offsetAngle / 3f + timeScalar2).ToRotationVector2();
                        auroraVelocity.Y = -Math.Abs(auroraVelocity.Y);
                        auroraVelocity = (auroraVelocity * velocityScale - playerVelocityOffset).SafeNormalize(Vector2.UnitY) * 0.07f;

                        Vector2 auroraSpawnPosition = auroraOffset;
                        auroraSpawnPosition.X += (float)Math.Cos(offsetAngle + timeScalar3) * 75f;

                        int x = (int)((auroraSpawnPosition.X - drawPosition.X) / ProfanedMoonlightAuroraDrawer.Scale);
                        int y = (int)((auroraSpawnPosition.Y - drawPosition.Y) / ProfanedMoonlightAuroraDrawer.Scale);
                        for (int j = -sourceArea; j <= sourceArea; j++)
                        {
                            for (int k = -sourceArea; k <= sourceArea; k++)
                                ProfanedMoonlightAuroraDrawer.CreateSource(x + origin + j, y + origin + k, auroraPower, auroraColor, auroraVelocity);
                        }
                    }
                };
            }

            if (NOU)
                NOULOL();
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

            // All double jumps, vanilla and modded, cannot be used as long as the player has this debuff.
            Player.blockExtraJumps = true;

            Player.rocketBoots = 0;
            Player.wingTimeMax = (int)(Player.wingTimeMax * 0.5);
        }
        #endregion

        #region NOULOL
        private void NOULOL()
        {
            Player.ResetEffects();
            Player.head = -1;
            Player.body = -1;
            Player.legs = -1;
            Player.handon = -1;
            Player.handoff = -1;
            Player.back = -1;
            Player.front = -1;
            Player.shoe = -1;
            Player.waist = -1;
            Player.shield = -1;
            Player.neck = -1;
            Player.face = -1;
            Player.balloon = -1;
            NOU = true;
        }
        #endregion

        #region Anomaly's Nanogun Kill Sound
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (Player.whoAmI == Main.myPlayer && Player.ActiveItem().type == ModContent.ItemType<TheAnomalysNanogun>())
            {
                if (Main.rand.NextBool(20))
                    SoundEngine.PlaySound(IjiDeathSound, Player.Center);
            }
        }
        #endregion

        #region Nurse Modifications
        public override bool ModifyNurseHeal(NPC nurse, ref int health, ref bool removeDebuffs, ref string chatText)
        {
            if (Main.zenithWorld)
            {
                // https://github.com/tModLoader/tModLoader/wiki/IEntitySource#detailed-list
                // The meteor is considered to be spawned from the Nurse herself
                var source = nurse.GetSource_FromThis("Calamity_GetFixedBoiNurseExtinctionMeteor");
                if (Player.whoAmI == Main.myPlayer)
                {
                    int proj = Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ModContent.ProjectileType<LeviathanBomb>(), 9999, 10f, Player.whoAmI);
                    if (Main.projectile[proj].whoAmI.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[proj].timeLeft = 10;
                        Main.projectile[proj].scale = 6f;
                        Main.projectile[proj].friendly = true;
                        Main.projectile[proj].netUpdate = true;
                    }
                }

                return false;
            }

            if ((CalamityWorld.death || BossRushEvent.BossRushActive) && areThereAnyDamnBosses)
            {
                chatText = CalamityUtils.GetTextValue("Vanilla.NurseChat.HealNotAllowed");
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
                else if (NPC.downedPlantBoss || DownedBossSystem.downedCalamitasClone)
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
                SoundEngine.PlaySound(RogueStealthSound, Player.Center);
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
                stealthGenStandstill += 0.15f;
                stealthGenMoving += 0.15f;
            }

            // Accessory modifiers can boost these stats
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
             * DGS  = (100% + 0.5% * T)
             * EM   = (100% + 0.5% * T)
             * BOTH = (100% + 0.75% * T)
             *
             */
            if (darkGodSheath && eclipseMirror)
            {
                stealthAcceleration += 0.075f;
                //stealthAcceleration *= 1.005f;
            }
            else if (eclipseMirror)
            {
                stealthAcceleration += 0.005f;
                //stealthAcceleration *= 1.005f;
            }
            else if (darkGodSheath)
                stealthAcceleration += 0.005f;

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

        public void ConsumeStealthByAttacking()
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

                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (spearsFired == 2)
                        break;
                    if (p.owner == Player.whoAmI && p.friendly)
                    {
                        bool attack = p.type == ModContent.ProjectileType<MiniGuardianAttack>();
                        if (attack)
                        {
                            int numSpears = profanedCrystalBuffs ? 12 : 6;
                            int dam = (int)(p.originalDamage * (profanedCrystalBuffs ? 1f : 0.25f));

                            for (int x = 0; x < numSpears; x++)
                            {
                                float angle = MathHelper.TwoPi / numSpears * x;
                                int proj = Projectile.NewProjectile(source, p.Center, angle.ToRotationVector2().RotatedBy(Math.Atan(-45f)) * 8f, ModContent.ProjectileType<MiniGuardianSpear>(), dam, 0f, Player.whoAmI, pscState, 0f);
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
            bool validEquipSlot = Player.legs == EquipLoader.GetEquipSlot(Mod, "ProfanedSoulCrystal", EquipType.Legs) ||
                                  Player.legs == EquipLoader.GetEquipSlot(Mod, "PscNightLegs", EquipType.Legs);
            if (!profanedCrystalHide && (profanedCrystal || profanedCrystalForce) && validEquipSlot)
            {
                bool usingCarpet = Player.carpetTime > 0 && Player.controlJump; //doesn't make sense for carpet to use jump frame since you have solid ground
                AnimationType animType = AnimationType.Walk;
                if ((Player.sliding || Player.velocity.Y != 0 || Player.mount.Active || (Player.grappling[0] != -1 || !Player.CheckSolidGround()) || Player.GoingDownWithGrapple) && !usingCarpet)
                    animType = AnimationType.Jump;
                else if (Player.velocity.X == 0 || usingCarpet)
                    animType = AnimationType.Idle;
                int frame = HandlePSCAnimationFrames(animType);
                Player.legFrame.Y = Player.legFrame.Height * frame;
            }
        }

        #endregion

        #region Misc Stuff
        private static int startMessageDisplayDelay = -1;

        // Triggers effects that must occur when the player enters the world. This sends a bunch of packets in multiplayer.
        // It also starts the speedrun timer if applicable.
        public override void OnEnterWorld()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                EnterWorldSync();

            // Enabling the config while a player is loaded will show the timer immediately.
            // But it won't start running until you save and quit and re-enter a world.
            if (CalamityConfig.Instance.SpeedrunTimer)
                CalamityMod.SpeedrunTimer.Restart();

            // Set a random delay between 12 and 20 seconds. When this delay hits zero, startup messages display
            if (CalamityConfig.Instance.WikiStatusMessage)
            {
                startMessageDisplayDelay = Main.rand.Next(CalamityUtils.SecondsToFrames(12), CalamityUtils.SecondsToFrames(20) + 1);
            }
        }

        /// <summary>
        /// Returns the range at which an abyss enemy can detect the player
        /// </summary>
        /// <param name="range">The default detection range</param>
        /// <returns></returns>
        public float GetAbyssAggro(float range)
        {
            range *= fishAlert ? 3f : 1f;
            range *= eidolonSnailPet ? 0.85f : 1f;
            range *= anechoicCoating ? 0.5f : 1f;
            range *= anechoicPlating ? 0.5f : 1f;
            range *= abyssalMirror ? 0.65f : 1f;
            range *= eclipseMirror ? 0.3f : 1f;
            range *= reaverExplore ? 0.9f : 1f;
            return range;
        }

        public void SpawnGravistarParticle()
        {
            float height = Player.height;
            if (Player.gravDir == -1)
            {
                height = 0;
            }
            Vector2 position1 = Player.position + new Vector2(Player.width / 14, height);
            Vector2 position2 = Player.position + new Vector2(Player.width * 13 / 14, height);
            SquareParticle square1 = new SquareParticle(position1, Player.velocity * (0.15f + Main.rand.NextFloat(0.1f)), false, 15, 1.7f + Main.rand.NextFloat(0.6f), Color.Cyan * 1.5f);
            SquareParticle square2 = new SquareParticle(position2, Player.velocity * (0.15f + Main.rand.NextFloat(0.1f)), false, 15, 1.7f + Main.rand.NextFloat(0.6f), Color.Cyan * 1.5f);
            GeneralParticleHandler.SpawnParticle(square1);
            GeneralParticleHandler.SpawnParticle(square2);
        }
        #endregion

        #region Mana Consumption Effects
        public override void OnConsumeMana(Item item, int manaConsumed)
        {
            CalamityPlayer modPlayer = Player.Calamity();
            if (Main.rand.NextBool() && modPlayer.lifeManaEnchant)
            {
                if (Main.myPlayer == Player.whoAmI)
                {
                    Player.HealEffect(-5);
                    Player.statLife -= 5;
                    if (Player.statLife <= 0)
                        Player.KillMe(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.ManaConversionEnchant").Format(Player.name)), 1000, -1);
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
    }
}
