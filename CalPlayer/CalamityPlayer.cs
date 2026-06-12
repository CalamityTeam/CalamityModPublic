using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Balancing;
using CalamityMod.BiomeManagers;
using CalamityMod.Buffs;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Cooldowns;
using CalamityMod.DataStructures;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.FluidSimulation;
using CalamityMod.Graphics;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Armor;
using CalamityMod.Items.Armor.Auric;
using CalamityMod.Items.Armor.Bloodflare;
using CalamityMod.Items.Armor.Brimflame;
using CalamityMod.Items.Armor.Daedalus;
using CalamityMod.Items.Armor.Demonshade;
using CalamityMod.Items.Armor.DesertProwler;
using CalamityMod.Items.Armor.Empyrean;
using CalamityMod.Items.Armor.LunicCorps;
using CalamityMod.Items.Armor.OmegaBlue;
using CalamityMod.Items.Armor.PlagueReaper;
using CalamityMod.Items.Armor.Prismatic;
using CalamityMod.Items.Armor.Silva;
using CalamityMod.Items.Armor.SnowRuffian;
using CalamityMod.Items.Armor.Tarragon;
using CalamityMod.Items.Armor.TitanHeart;
using CalamityMod.Items.Armor.Victide;
using CalamityMod.Items.Armor.Wulfrum;
using CalamityMod.Items.Dyes;
using CalamityMod.Items.Fishing;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Mounts.Minecarts;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.Potions.Food;
using CalamityMod.Items.Tools;
using CalamityMod.Items.Tools.ClimateChange;
using CalamityMod.Items.TreasureBags.MiscGrabBags;
using CalamityMod.Items.VanillaArmorChanges;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Systems;
using CalamityMod.Systems.Collections;
using CalamityMod.Systems.Mechanic;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Utilities;
using CalamityMod.Walls.UnsafeWalls;
using CalamityMod.Waters;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.GameContent.NetModules;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Net;
using static Terraria.Main;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.CalPlayer
{
    public partial class CalamityPlayer : ModPlayer
    {
        #region Variables

        #region No Category
        /// <summary>
        /// Increments by 1 for every frame this player has existed.
        /// </summary>
        internal ulong universalFrameTimer = 0;

        /// <summary> If true, there is a boss NPC active in the world. Primary bool for checking effects that only occur if a boss is alive. </summary>
        public static bool areThereAnyDamnBosses = false;
        /// <summary> If true, there is an event actively occuring near the player. Solely used for preventing Silva Revive's cooldown from decreasing. </summary>
        public static bool areThereAnyDamnEvents = false;
        public float calamityBonusLuck = 0f;
        public bool drawBossHPBar = true;
        public float stealthUIAlpha = 1f;
        public float SulphWaterUIOpacity = 1f;
        /// <summary> Used to determine whether or not extra information should be displayed on the Boss Health Bar. </summary>
        public bool shouldDrawSmallText = true;
        /// <summary> Used by The Evolution to store the type of the projectile that hit the player when its reflect is triggered. </summary>
        public int projTypeJustHitBy;
        /// <summary> Unused variable, formerly used for displaying special SCal dialogue. </summary>
        public int sCalDeathCount = 0;
        /// <summary> <inheritdoc cref="sCalDeathCount"/> </summary>
        public int sCalKillCount = 0;
        public int actualMaxLife = 0;
        /// <summary> Constant variable used for how long Chaos State is inflicted by Rod of Discord while a boss is alive. </summary>
        public static int chaosStateDuration = 900;
        /// <summary> Constant variable used for how long Chaos State is inflicted by Normality Relocator while a boss is alive. </summary>
        public static int chaosStateDuration_NR = 1200;
        public double contactDamageReduction = 0D;
        public double projectileDamageReduction = 0D;
        public int hellbornShots = 0;
        public int garandShots = 0;
        /// <summary> If set to true, prevents all player dashes. Used by Ball and Chain, and Stygian Shield. </summary>
        public bool blockAllDashes = false;
        /// <summary> Used by Flamsteed Ring to reset the player's hitbox size after dismounting. </summary>
        public bool resetHeightandWidth = false;
        /// <summary> If set to true, completely disables ALL life regeneration effects. Used by Omega Blue armor. </summary>
        public bool noLifeRegen = false;
        public float ammoCost = 1f;
        public float partialLifeRegen = 0f;
        public float healingPotionMultiplier = 1f;
        /// <summary>
        /// Tracks whether or not the player is currently holding Gael's Greatsword.<br/>
        /// Used to toggle the ability to use its unique Rage attack.
        /// </summary>
        public bool heldGaelsLastFrame = false;
        /// <summary>
        /// Tracks whether or not the player is currently holding Elephant Killer.<br/>
        /// Used to reset stealth when swapping to this item.
        /// </summary>
        public bool heldElephantKillerLastFrame = false;
        public float elephantKillerJoke = 0;
        public bool drawingElephantKillerJoke = false;
        /// <summary>
        /// Tracks whether or not the player currently has Draedon's Heart equipped.<br/>
        /// Used to reset Adrenaline when (un)equipped to prevent exploits.
        /// </summary>
        internal bool hadNanomachinesLastFrame = false;
        public bool combHair;
        public bool disableVoodooSpawns = false;
        public bool disablePerfCystSpawns = false;
        public bool disableHiveCystSpawns = false;
        public bool disableNaturalScourgeSpawns = false;
        public bool disableAnahitaSpawns = false;
        public int whitewaterHeal = 0;
        /// <summary> Used for toggling Calamity's blazing cursor effect. </summary>
        public bool blazingCursorDamage = false;
        /// <summary>
        /// <inheritdoc cref="blazingCursorDamage"/><br/>
        /// This variable is enabled if the accessory is in vanity, granting only the visuals.
        /// </summary>
        public bool blazingCursorVisuals = false;
        public float blazingMouseAuraFade = 0f;
        /// <summary>
        /// General variable used for controlling the strength of screenshake this player is experiencing. Measured in pixels of offset that can be applied to the screen.<br/>
        /// When setting this, be sure to only set it if its current value is less than the value to set, to prevent overriding an ongoing stronger screenshake with a weaker one.<br/>
        /// A helper method exists which can automatically do this for you, <see cref="CalamityUtils.SetScreenshake"/>.
        /// </summary>
        public float GeneralScreenShakePower = 0f;
        /// <summary> Set to true when this player receives the Brimstone Locus from speaking to the Brimstone Witch for the first time. </summary>
        public bool GivenBrimstoneLocus = false;
        public DoGCartSegment[] DoGCartSegments = new DoGCartSegment[DoGCartMount.SegmentCount];
        public float SmoothenedMinecartRotation;
        /// <summary>
        /// Set to true when a weapon performs a lunge attack, such as Biome Blade's Pure Clarity attunement.<br/>
        /// Massively increases the player's max fall speed, and prevents fall damage.
        /// </summary>
        public bool LungingDown = false;
        /// <summary>
        /// Variable set before the player is teleported to the Underworld for Wall of Flesh in Boss Rush.<br/>
        /// Used to teleport them back to their previous position after Wall of Flesh is defeated.
        /// </summary>
        public Vector2? BossRushReturnPosition = null;

        public float moveSpeedBonus = 0f;
        public int momentumCapacitorTime = 0;
        /// <summary> A multiplier on the player's movement speed applied while using Momentum Capacitor. </summary>
        public float momentumCapacitorBoost = 0f;
        public enum FishingMinigames
        {
            None,
            WulfrumBobber,
            SunkenSinker,
            AcrobaticBobber,
            FeralBobber,
            VolcanicSinker,
            DevourerofCods //Not a fishing minigame but uses the same code as the rest
        }
        public FishingMinigames SelectedFishingMinigame = FishingMinigames.None;
        public bool countsAsAnyWet => (Player.armor[0].type == ItemID.FishBowl || (Main.IsItRaining && Player.Center.Y < Main.worldSurface * 16.0) || Player.dripping || Player.wetCount > 0 || Player.wet || Player.honeyWet || Player.lavaWet);

        /// <summary>
        /// How many Starbursts the player has
        /// </summary>
        public int StratusStarburst = 0;
        public int AvaliableStarburst = 0;
        public static int MaxStratusStarburst = 100;
        /// <summary>
        /// Used for Halley's Inferno to generate starbursts
        /// </summary>
        public float HalleyAccuracyCounter = 0;
        public float StarburstSpawnFrameCounter = 0;
        /// <summary>
        /// A cooldown for losing Starbursts over time, reset by holding Stratus items or having Sirius spawned
        /// </summary>
        public int StratusStarburstResetTimer = 0;
        /// <summary>
        /// Duration of Stratus Sphere's Starshield
        /// </summary>
        public int Starshield = 0;

        public List<StarburstEntity> StarburstEntities = new();

        public CombatText subtitletext = null;
        public Color[] subtitleColors = new Color[] { Color.White, Color.White };
        public int DoGHeadHitCounter = 0;

        public StatModifier TypelessDebuffMultiplier = new();
        public StatModifier HeatDebuffMultiplier = new();
        public StatModifier ColdDebuffMultiplier = new();
        public StatModifier SicknessDebuffMultiplier = new();
        public StatModifier WaterDebuffMultiplier = new();
        public StatModifier ElectricDebuffMultiplier = new();

        public int TimeHoldingMelee = 0;
        public int TimeHoldingRanged = 0;
        public int TimeHoldingMagic = 0;
        public int TimeHoldingSummon = 0;
        public int TimeHoldingRogue = 0;
        public int TimeHoldingClassless = 0;

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
        public int CurrentlyViewedCanvasID = -1;
        public int CurrentlyViewedCanvasType = -1;
        public string CurrentlyViewedHologramText;
        #endregion

        #region External variables -- Not used by Calamity, only via Mod.Call or reflection
        [Obsolete("No longer does anything in the new abyss light system")]
        public int externalAbyssLight = 0;

        public float externalBreathTickBoost = 0f;
        public float externalFlightTimeMultBoost = 0f;

        // 25FEB2025: Ozzatron: per request, there are now three-state controls for enabling or disabling rippers
        public bool? externalRageEnabled = null;
        public bool? externalAdrenalineEnabled = null;

        public bool externalColdImmunity = false;
        public bool externalHeatImmunity = false;

        // 25FEB2025: Ozzatron: per request, there is now an external bool for Auric Rejection immunity
        public bool externalAuricRejectionImmunity = false;

        // 27AUG2024: Ozzatron: per request, there is now an external bool for per-player defense damage immunity
        public bool externalDefenseDamageImmunity = false;

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
        public bool newAmidiasInventory = false;
        public bool newBanditInventory = false;
        public bool newCalamitasInventory = false;
        #endregion

        #region Timer and Counter
        public float partialLifeRegenCounter = 0f;
        public int gaelSwipes = 0;
        public int arsenalCooldown = 0;
        public int killModeCooldown = 0;
        public bool demonSwordKillMode = false;
        public bool exaltedKillMode => (demonSwordKillMode && Player.HeldItem.type == ItemType<ExaltedOathblade>());
        public bool devilsDevastationKillMode => (demonSwordKillMode && Player.HeldItem.type == ItemType<DevilsDevastation>());
        /// <summary>
        /// Tracks Dragoon Drizzlefish's "gel feed" mechanic in Get fixed boi.<br/>
        /// Consuming Gel adds 1 to this counter, up to a maximum of 6, and using the weapon has a random chance to decrement the counter.<br/>
        /// The weapon's damage is multiplied by the value in this counter.
        /// </summary>
        public int dragoonDrizzlefishGelBoost = 1;
        public int deadSunCounter = 6;
        public int DragonsBreathAudioCooldown = 0;
        public int DragonsBreathAudioCooldown2 = 0;
        public float unstableCastersGauntletVis = 0;
        public int unstableCastersGauntletVisTimer = 0;
        public int PhotoAudioCooldown = 0;
        public int PhotoTimer = 90;
        /// <summary> Cooldown variable used to add a delay between Anahita's Arpeggio uses. </summary>
        public int arpeggioCooldown = 0;
        /// <summary> Cooldown variable which prevents using Burning Sea during its "burn-out" mechanic. </summary>
        public int burningSeaBurnOut = 0;
        /// <summary> Cooldown variable for spawning Plague Tainted SMG's drones from left-click bullets. </summary>
        public int plagueTaintedSMGDroneCooldown = 0;
        /// <summary> Cooldown variable which prevents using Firestorm Cannon or Spectralstorm Cannon during their overheat periods. </summary>
        public int flareGunOverheat = 0;
        /// <summary>
        /// If true, this player's Brittle Star Staff minions are in their orbiting mode.<br/>
        /// While in this mode, they orbit around the player, do not break on hits, and increase defense.
        /// </summary>
        public bool brittleStarBuffMode = false;
        /// <summary> If set to true, initiates the dash ability of Speed Blaster or Superradiant Slaughterer. </summary>
        public bool sBlasterDashActivated = false;
        public int saharaSlicersBolts = 0;
        public int oceanCrestTimer = 0;
        /// <summary> Pwnagehammer's hit counter, used to track when to spawn its empowered hammer. </summary>
        public int Holyhammer = 0;
        /// <summary> Fallen Paladin's Hammer's hit counter, used to track when to spawn its empowered hammer. </summary>
        public int PHAThammer = 0;
        /// <summary> Stellar Contempt's hit counter, used to track when to spawn its empowered hammer. </summary>
        public int StellarHammer = 0;
        /// <summary> Galaxy Smasher's hit counter, used to track when to spawn its empowered hammer. </summary>
        public int GalaxyHammer = 0;
        /// <summary> Variable used to apply Ontological Despoiler's nerfs when continuously using a single firing mode. </summary>
        public bool despoilerNerf = false;
        /// <summary> Variable used to trigger Molten Amputator's stealth effect on right-click. </summary>
        public int amputatorBuff = 0;
        /// <summary> Variable used to track the current fuel amount for Pristine Fury's right-click. </summary>
        public int furyFuel = 1800;
        /// <summary> Variable to track the healing over time gained from Grand Dad. </summary>
        public int grandDadHealPool = 0;
        public int grandDadHealTimer = 0;
        public const int FuryFuelMax = 1800;
        public float furyRefuelTimer = 0;
        /// <summary> Variable used to track if Auger can do its big slash attack. </summary>
        public bool buffedAuger = false;

        public int rOfResilienceCooldown = 0;
        public int rOfResilienceEffect = 0;
        public int rOfResilienceOrbitOffset = 0;
        /// <summary> Variable which tracks how many shots Norfleet has fired. Used to determine when to recharge. </summary>
        public int NorfleetCounter = 0;
        public int hideOfDeusMeleeBoostTimer = 0;
        /// <summary>
        /// The player's alcohol level. Increased by 1 for each alcohol the player has drank, 2 for Everclear.<br/>
        /// If this value is greater than the max (normally 3), the player is inflicted with Alcohol Poisoning.
        /// </summary>
        public int alcoholPoisonLevel = 0;
        public static int alcoholPoisonMaxBase = 3;
        /// <summary> The maximum alcohol level the player can reach before getting Alcohol Poisoning. </summary>
        public int alcoholPoisonMax = alcoholPoisonMaxBase;
        public int dashTimeMod;
        /// <summary>
        /// Timer variable which tracks how long the player has spent outside of Providence's border radius, in frames.<br/>
        /// Used to scale the damage of the Holy Inferno debuff.
        /// </summary>
        public int hInfernoBoost = 0;
        public int packetTimer = 0;
        public int navyRodAuraTimer = 0;
        /// <summary> Timer variable used to time when Hydrothermic armor's set bonus inferno ring deals damage to targets. </summary>
        public int hydrothermicInfernoTimer = 0;
        /// <summary> Timer variable used to time when Tarragon armor's set bonus life aura deals damage to targets. </summary>
        public int tarraLifeAuraTimer = 0;
        public int bloodflareHeartTimer = 300;
        /// <summary> Counter variable used to determine when to spawn Dragon Rage's fireballs. Fireballs are spawned after 10 hits. </summary>
        public int dragonRageHits = 0;
        /// <summary> Cooldown variable for Dragon Rage's fireball spawning to prevent spamming projectiles when hitting multiple enemies simultaneously. </summary>
        public int dragonRageCooldown = 0;
        /// <summary> Counter variable which controls Aquatic Emblem's stat boosts while underwater. </summary>
        public float aquaticBoost = 0;
        public int galileoCooldown = 0;
        /// <summary> Used to track Prideful Hunter's Planar Ripper's movement speed boost, along with its visual effects. </summary>
        public int planarSpeedBoost = 0;
        public int profanedSoulWeaponUsage = 0;
        public int profanedSoulWeaponType = 0;
        /// <summary> Counter variable used to track how many hits have been landed with The Dance of Light, for the purposes of triggering its blinding flash attack. </summary>
        public int danceOfLightCharge = 0;
        /// <summary> Cooldown variable used to prevent DoG from spamming combat text messages when hitting the player. </summary>
        public int dogTextCooldown = 0;
        public float auralisStealthCounter = 0f;
        public int auralisAuroraCounter = 0;
        public int auralisAuroraCooldown = 0;
        public int auralisAurora = 0;
        /// <summary>
        /// Counter variable used to track Necro armor's set bonus temporary revive.<br/>
        /// When the player receives fatal damage, this value is set to 0, and then increments on each frame.
        /// </summary>
        public int necroReviveCounter = -1;
        public int hideOfDeusTimer = 0;
        public int murasamaHitCooldown = 0;
        public int giantShellPostHit = 0;
        public int tortShellPostHit = 0;
        public int sharkGunDamageScaling = 0;
        public int MiniSwarmerCooldown = 0;
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

        public int consecutiveCaughtFish = 0;
        public float WeakTimeFreezeUseTimer = 0;
        public bool WeakTimeFreezeInUse = false;

        public int DemonAltarDialogueCounter = 0;
        public int DemonAltarDialogueCooldown = 0;
        #endregion

        #region Sound
        /// <summary> General sound cooldown variable. Used by The Microwave and Gastric Belcher Staff. </summary>
        public int soundCooldown = 0;
        public int hurtSoundTimer = 0;
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
        public static readonly SoundStyle AdrenalineHurtGFB = new("CalamityMod/Sounds/Custom/AdrenalineMajorLossGFB");
        public static readonly SoundStyle NanomachinesActivationSound = new("CalamityMod/Sounds/Custom/AbilitySounds/NanomachinesActivate");

        public static readonly SoundStyle RogueStealthSound = new("CalamityMod/Sounds/Custom/RogueStealth");
        public static readonly SoundStyle DefenseDamageSound = new("CalamityMod/Sounds/Custom/DefenseDamage");

        public static readonly SoundStyle IjiDeathSound = new("CalamityMod/Sounds/Custom/IjiDies");
        public static readonly SoundStyle DrownSound = new("CalamityMod/Sounds/Custom/AbyssDrown");
        public static readonly SoundStyle LeonDeathNoiseRE4_ForGFB = new("CalamityMod/Sounds/Custom/GFB/LeonDeathNoiseRE4");
        public static readonly SoundStyle BaroclawHit = new("CalamityMod/Sounds/NPCKilled/DevourerSegmentBreak2") { Volume = 0.7f };
        public static readonly SoundStyle AbsorberHit = new("CalamityMod/Sounds/Custom/AbilitySounds/SilvaActivation") { Volume = 0.7f };
        #endregion

        #region Rogue
        /// <summary> The player's current rogue stealth value.<br/>
        /// Note that the player's displayed stealth value is 100x the internal value. For example, a value of 0.1f means having 10 stealth.
        /// </summary>
        public float rogueStealth = 0f;
        /// <summary>
        /// The player's maximum rogue stealth value, from armor and accessories.<br/>
        /// Note that the player's displayed stealth value is 100x the internal value. For example, a value of 0.1f means having 10 max stealth.
        /// </summary>
        public float rogueStealthMax = 0f;
        /// <summary>
        /// Temporarily provides stealth even without Rogue armor.
        /// Max stealth is set to temporaryStealthMax if it's 0.
        /// </summary>
        public int temporaryStealthTimer = 0;
        /// <summary>
        /// Amount of temporary max stealth provided by gear
        /// 1f = 100 max steatlh
        /// Duration is temporaryStealthTimer
        /// Resets to 0.1f whenever timer ends
        /// </summary>
        public float temporaryStealthMax = 0.1f;
        /// <summary> A multiplier to the player's stealth generation when standing still. </summary>
        public float stealthGenStandstill = 1f;
        /// <summary> A multiplier to the player's stealth generation when moving. </summary>
        public float stealthGenMoving = 1f;
        public int flatStealthLossReduction = 0;
        public const float StealthAccelerationCap = 1.5f;
        public float stealthAcceleration = 1f;
        public bool stealthStrikeThisFrame = false;
        /// <summary>
        /// If true, stealth strikes only require 50% of the player's max stealth to perform.<br/>
        /// Used by Dark Matter Sheath and Eclipse Mirror.
        /// </summary>
        public bool stealthStrikeHalfCost = false;
        /// <summary>
        /// If true, stealth strikes only require 75% of the player's max stealth to perform.<br/>
        /// Used by Ruin Medallion.
        /// </summary>
        public bool stealthStrike75Cost = false;
        /// <summary>
        /// If true, stealth strikes only require 90% of the player's max stealth to perform.<br/>
        /// Used by Coin of Deceit.
        /// </summary>
        public bool stealthStrike90Cost = false;
        /// <summary> If true, this player is wearing a rogue or all-class armor set. This bool is required in order to use rogue stealth. </summary>
        public bool wearingRogueArmor = false;
        /// <summary> The sum of the player's stealth generation boosts from accessory modifiers. </summary>
        public float accStealthGenBoost = 0f;

        // TODO -- Stealth needs to be its own damage class so that stealth bonuses only apply to stealth strikes
        /// <summary>
        /// The extra damage boost for rogue stealth strikes.<br/>
        /// This is obtained from a formula derived from the player's current stealth, the weapon's use time, and the player's stealth generation boosts.
        /// </summary>
        public float stealthDamage = 0f;
        /// <summary> An additional damage multiplier applied to rogue stealth strikes. Used by Filthy Glove, Rotten Dogtooth and their upgrades. </summary>
        public double bonusStealthDamage = 0;
        public float rogueVelocity = 1f;

        public int focusFlurryAttackCount = 0;
        #endregion

        #region Mount
        public bool onyxExcavator = false;
        public bool rimehound = false;
        public bool crysthamyr = false;
        public bool ExoChair = false;
        public AndromedaPlayerState andromedaState;
        /// <summary>
        /// Andromeda Cripple is inflicted on the player if they dismount Flamsteed Ring while a boss is alive.<br/>
        /// This severely slows movement and prevents the weapon from being used for its duration.
        /// </summary>
        public int andromedaCripple;
        #endregion

        #region Pet
        public bool burrowerPet = false;
        public bool thirdSage = false;
        public bool perfmini = false;
        public bool akato = false;
        public bool yharonPet = false;
        public bool leviPet = false;
        public bool plaguebringerBab = false;
        public bool rotomPet = false;
        public bool ladShark = false;
        /// <summary> If greater than 0, causes this player to constantly spawn heart gores. </summary>
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
        public bool frostyBat = false;
        public bool toastyBat = false;
        public bool beldum = false;
        public bool starSwallowerPetFroge = false;
        #endregion

        #region Rage
        /// <summary>
        /// If true, enables the Rage mechanic. This is normally Revengeance-exclusive, but can also be enabled by Shattered Community.<br/>
        /// Rage is gained from staying close to enemies, or passively by using certain accessories.<br/>
        /// When the bar is filled, Rage can be activated to provide a small damage boost over a longer duration.
        /// </summary>
        public bool RageEnabled
        {
            get
            {
                if (externalRageEnabled.HasValue)
                    return externalRageEnabled.Value;
                return CalamityWorld.revenge || shatteredCommunity;
            }
        }
        public bool rageModeActive = false;
        /// <summary> The player's current Rage level. Expressed as a percentage of maximum Rage. </summary>
        public float rage = 0f;
        public float rageMax = 100f; // 0 to 100% by default
        /// <summary>
        /// The duration which Rage Mode lasts.<br/>
        /// Starts at 9 seconds, and is increased by 1 second for each upgrade, up to a maximum of 12 seconds.
        /// </summary>
        public int RageDuration = BalancingConstants.DefaultRageDuration;
        /// <summary> Used by Shattered Community as a short cooldown for gaining Rage when taking damage. </summary>
        public int rageGainCooldown = 0;
        /// <summary>
        /// Counter variable used as a delay before the player starts losing Rage.<br/>
        /// The player will start losing Rage if they do not generate Rage, hit an enemy, or take damage for 10 seconds.
        /// </summary>
        public int rageCombatFrames = 0;
        public float RageDamageBoost = BalancingConstants.DefaultRageDamageBoost;
        #endregion

        #region Adrenaline
        /// <summary>
        /// If true, enables the Adrenaline mechanic. This is normally Revengeance-exclusive, but can also be enabled by Draedon's Heart.<br/>
        /// Adrenaline is gained by avoiding taking damage while a boss is alive.<br/>
        /// When the bar is filled, Adrenaline can be activated to provide a large damage boost over a short duration.
        /// </summary>
        public bool AdrenalineEnabled
        {
            get
            {
                if (externalAdrenalineEnabled.HasValue)
                    return externalAdrenalineEnabled.Value;
                return CalamityWorld.revenge || draedonsHeart;
            }
        }
        public bool adrenalineModeActive = false;
        /// <summary> The player's current Adrenaline level. Expressed as a percentage of maximum Adrenaline. </summary>
        public float adrenaline = 0f;
        public float adrenalineMax = 100f; // 0 to 100% by default
        /// <summary> Used as a short cooldown when the player takes damage before Adrenaline can begin charging again. </summary>
        public int adrenalinePauseTimer = 0;
        /// <summary> Constant variable representing the duration which Adrenaline Mode lasts. </summary>
        public int AdrenalineDuration = CalamityUtils.SecondsToFrames(5);
        /// <summary> Constant variable representing the duration it takes to fully charge Adrenaline. </summary>
        public int AdrenalineChargeTime = CalamityUtils.SecondsToFrames(30);
        /// <summary> Constant variable representing the duration it takes for Adrenaline to empty if no boss is alive. </summary>
        public int AdrenalineFadeTime = CalamityUtils.SecondsToFrames(2);
        #endregion

        #region Defense Damage
        /// <summary>
        /// Ratio at which mitigated damage is converted into defense damage.<br/>
        /// The net difference between incoming damage and final taken damage is what is multiplied by this ratio.<br/>
        ///
        /// For example, if the player has 200 defense and 25% DR and gets hit for 576 damage, on Expert:
        ///
        /// <para>Incoming damage = 576</para>
        /// <para>Defense reduction = 0.75 * 200 = 150</para>
        /// <para>Damage after defense = 426</para>
        /// <para>DR reduction = 0.25 * 426 = 106.5</para>
        /// <para>Damage after DR = 319.5 (rounds down to 319)</para>
        ///
        /// <para>Net Difference = 576 - 319 = 257</para>
        /// <para>Defense Damage = 257 * 0.3333 = <b>85.6581</b> (rounds up to 86)</para>
        ///
        /// The player then loses 86 defense.<br/>
        /// DR is lost according to the ratio of defense lost versus total defense.<br/>
        /// In this case, that ratio is 86 / 200 = 0.43. The player loses 0.43 * 0.25 = <b>10.75% DR</b>.
        /// </summary>
        public double defenseDamageRatio = BalancingConstants.DefaultDefenseDamageRatio;

        /// <summary> Current effect of defense damage, calculated as total defense damage lerped to zero over the recovery time. </summary>
        public int CurrentDefenseDamage => (int)(totalDefenseDamage * ((float)defenseDamageRecoveryFrames / totalDefenseDamageRecoveryFrames));

        /// <summary> Total defense damage inflicted. This number keeps increasing if the player is repeatedly hit during the recovery period. </summary>
        internal int totalDefenseDamage = 0;

        /// <summary>
        /// Defense damage from a single hit recovers in 60 frames, no matter how big the hit was.<br/>
        /// If you get hit AGAIN before you have fully recovered, 60 more frames are added to your recovery timer.
        /// </summary>
        internal const int DefenseDamageBaseRecoveryTime = 60;
        /// <summary>
        /// The maximum possible recovery time is 15 seconds.<br/>
        /// This is to prevent annoyance where God Mode defense damage never goes away.
        /// </summary>
        internal const int DefenseDamageMaxRecoveryTime = 900;
        /// <summary> How many frames the player will continue to be recovering from defense damage. </summary>
        internal int defenseDamageRecoveryFrames = 0;

        /// <summary> The total timer of defense damage recovery that the player is currently suffering from. </summary>
        internal int totalDefenseDamageRecoveryFrames = DefenseDamageBaseRecoveryTime;

        /// <summary> The number of frames after immunity frames end before defense damage can start recovering. </summary>
        internal const int DefenseDamageRecoveryDelay = 10;
        /// <summary> The current timer for how long the player must wait before defense damage begins recovering. </summary>
        internal int defenseDamageDelayFrames = 0;

        /// <summary>
        /// Temporary bool for whether the current instance of incoming damage to the player is one that inflicts defense damage.<br/>
        /// Bloodflare Core ignores this and makes every single instance of incoming damage apply defense damage.
        /// </summary>
        public bool nextHitDealsDefenseDamage = false;
        #endregion

        #region Energy Shields
        public bool HasAnyEnergyShield => roverDrive || lunicCorpsSet || ((pSoulArtifact && !profanedCrystal) || profanedCrystalBuffs) || sponge || Starshield > 0;
        public bool freeDodgeFromShieldAbsorption = false;
        public bool drawnAnyShieldThisFrame = false;

        // TODO -- Some way to show the player their total shield points.
        public int TotalEnergyShielding => RoverDriveShieldDurability + LunicCorpsShieldDurability + pSoulShieldDurability + SpongeShieldDurability + (Starshield > 0 ? StratusStarburst : 0);
        public int TotalMaxShieldDurability => (roverDrive ? RoverDrive.ShieldDurabilityMax : 0) + (lunicCorpsSet ? LunicCorpsHelmet.ShieldDurabilityMax : 0) + (profanedCrystalBuffs ? ProfanedSoulCrystal.ShieldDurabilityMax : ((pSoulArtifact && !profanedCrystal) ? ProfanedSoulArtifact.ShieldDurabilityMax : 0)) + (sponge ? TheSponge.ShieldDurabilityMax : 0) + (Starshield > 0 ? 100 : 0);

        public int RoverDriveShieldDurability = 0;
        public int LunicCorpsShieldDurability = 0;
        public int SpongeShieldDurability = 0;

        public bool roverDrive = false;
        public bool roverDriveShieldVisible = false;
        internal float roverDriveShieldPartialRechargeProgress = 0f;
        internal bool playedRoverDriveShieldSound = false;

        // Lunic Corps shield is controlled by its armor set bool
        // Lunic Corps shield comes from an armor set and its visibility is non optional
        internal float lunicCorpsShieldPartialRechargeProgress = 0f;
        internal bool playedLunicCorpsShieldSound = false;

        // Profaned soul shield applies to psa and psc, with differing max hps for each
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
        /// <summary> The rate at which breath is lost while in the Abyss. </summary>
        public float abyssBreathLossRateStat = 0;
        /// <summary> The rate at which health is decreased after running out of breath while in the Abyss. </summary>
        public int abyssLifeLostAtZeroBreathStat = 0;
        /// <summary> The amount which defense is reduced while in the Abyss. </summary>
        public int abyssDefenseLossStat = 0;

        //These stats are for the abyss darkness system.
        public float abyssDarkness = 0f;
        public float abyssPlayerGlowMultiplier = 1f;
        public float abyssFlashlightWidthMultiplier = 1f;

        #endregion

        #region Light System
        public float darknessIntensity = 0;

        public Vector2? lastDeerclopsPosition;
        #endregion

        #region Permanent Buff
        /// <summary> If true, the player has consumed Celestial Onion. </summary>
        public bool extraAccessoryML = false;
        /// <summary> If true, the player has consumed Comet Shard. </summary>
        public bool cShard = false;
        /// <summary> If true, the player has consumed Ethereal Core. </summary>
        public bool eCore = false;
        /// <summary> If true, the player has consumed Phantom Heart. </summary>
        public bool pHeart = false;
        /// <summary> If true, the player has consumed Sanguine Tangerine. </summary>
        public bool sTangerine = false;
        /// <summary> If true, the player has consumed Miracle Fruit. </summary>
        public bool mFruit = false;
        /// <summary> If true, the player has consumed Tainted Cloudberry. </summary>
        public bool tCloudberry = false;
        /// <summary> If true, the player has consumed Sacred Strawberry. </summary>
        public bool sStrawberry = false;
        public bool revJamDrop = false;
        /// <summary> If true, the player has consumed Mushroom Plasma Root. </summary>
        public bool rageBoostOne = false;
        /// <summary> If true, the player has consumed Infernal Blood. </summary>
        public bool rageBoostTwo = false;
        /// <summary> If true, the player has consumed Red Lightning Container. </summary>
        public bool rageBoostThree = false;
        /// <summary> If true, the player has consumed Electrolyte Gel Pack. </summary>
        public bool adrenalineBoostOne = false;
        /// <summary> If true, the player has consumed Starlight Fuel Cell. </summary>
        public bool adrenalineBoostTwo = false;
        /// <summary> If true, the player has consumed Ectoheart. </summary>
        public bool adrenalineBoostThree = false;
        /// <summary> Used to heal the player to full health on respawn. </summary>
        public bool healToFull = false;
        #endregion

        #region Accessory
        public int friendlyMinions = 0;
        public bool shieldOfTheHighRulerDashVelocityBoosted = false;
        public bool yharimsGift = false;
        public bool luxorsGift = false;
        public bool luxorHit = false;
        public bool luxorsGiftVanity = false;
        public bool fungalSymbiote = false;
        public bool trinketOfChi = false;
        public bool gladiatorSword = false;
        public int gladiatorTimer = 0;
        public bool unstableGraniteCore = false;
        public bool regenerator = false;
        public float regeneratorDamage = 0;
        public bool theBee = false;
        public bool arcFlashRing = false;
        public bool arcFlashRingVisual = false;
        public int generalBandCooldown = 0;
        public bool bGlassBand = false; // Obsidian band
        public bool bGlassBandVisual = false;
        public bool batholithBangle = false; // Granite band
        public bool batholithBangleVisual = false;
        public bool protolithBangle = false; // Marble band
        public bool protolithBangleVisual = false;
        /// <summary> Used to prevent dodges from triggering The Bee's full health damage reduction cooldown. </summary>
        public bool shouldTriggerBeeCooldown = false;
        public int theBeeCooldown = 0;
        public bool aFossil = false;
        public bool aPowder = false;
        public bool fallingBlockProtection = false;
        public bool trapProtection = false;
        public bool alluringBait = false;
        public bool enchantedPearl = false;
        public bool fishingStation = false;
        public bool rBrain = false;
        public bool bloodyWormTooth = false;
        public bool ivDrip = false;
        /// <summary> NOT the primary Affliction variable, used instead for the Afflicted buff which is given to teammates. </summary>
        public bool afflicted = false;
        public bool chiRegen = false;
        public bool affliction = false;
        public bool stressPills = false;
        public bool laudanum = false;
        public bool heartOfDarkness = false;
        /// <summary> Used to buff the Profaned Guardian Relic drops while using Profaned Soul Crystal. </summary>
        public bool profanedSoulRelicBuff = false;
        public bool draedonsHeart = false;
        public bool vexation = false;
        public bool dodgeScarf = false;
        public bool evasionScarf = false;
        public bool badgeOfBravery = false;
        public bool WarbanneroftheRighteous = false;
        public bool warbannerGlow = false;
        public float warbannerDamageMult = 0;
        public bool tesla = false;
        public bool teslaVisuals = true;
        public bool cryogenSoul = false;
        public bool ascendantInsignia = false;
        public int ascendantInsigniaBuffTime = 0;
        public int ascendantInsigniaCooldown = 0;
        public bool fishStocks = false;
        public float fishStockVisual = 0;
        public float fishStockPower = 0;
        public float fishStockSlidingPower = 0;
        public (float, float, float, float, float) fishStockOldPower = (0, 0, 0, 0, 0);
        /// <summary> Used to toggle dust spawned while swinging, through accessory visibility. </summary>
        public bool magmaStoneVisuals = true;
        public bool eGauntlet = false;
        /// <summary> <inheritdoc cref="magmaStoneVisuals"/> </summary>
        public bool eGauntletVisuals = true;
        /// <summary>
        /// Used to prevent melee speed stacking with Feral Claws and its upgrades.<br/>
        /// Feral Claws = 1, Power Glove = 2, Mechanical Glove = 3, Fire Gauntlet = 4, Elemental Gauntlet = 5
        /// </summary>
        public int gloveLevel = 0;
        public bool alreadyHasFrogLeg = false; // Unused, intended to prevent Frog Leg tinker stacking
        public bool eTalisman = false;
        public bool lastDashWasTabi = false;
        public bool statisNinjaBelt = false;
        public bool voidSashVisuals = true;
        public bool statisVoidSash = false;
        public bool nucleogenesis = false;
        public bool nuclearFuelRod = false;
        public bool nebulousCore = false;
        public bool deepDiver = false;
        public bool aquaticHeartWaterBuff = false;
        public bool aquaticHeartIce = false;
        public bool ilSpark = false;
        public bool transformer = false;
        public bool transformerVisual = false;
        public int transformerCooldown = 0;
        public int transformerDelay = 0;
        public int transformerStoredKills = 0;
        public int hookPullVisuals = 0;
        public bool bloomStone = false;
        public bool bloomStoneHookVisuals = false;
        public int bloomStoneHealPool = 0;
        public int bloomStoneTotalHeal = 0;
        public float bloomStoneHealTimer = 0;
        public float bloomStoneHealRate = 0;
        public int bloomStoneBuffedHealRateTimer = 0;
        public bool hideOfDeus = false;
        public bool dAmulet = false;
        public bool rampartOfDeities = false;
        public bool gShell = false;
        public bool lAmbergris = false;
        public bool lAmbergrisVisual = false;
        public bool tortShell = false;
        public bool absorber = false;
        /// <summary>
        /// Used for detetcing life regen hindering debuffs
        /// </summary>
        bool _hasNoNaturalRegen = false;
        public bool hadLifeRegenHinderingDebuff = false;
        public bool alwaysHoneyRegen = false;
        public float alwaysHoneyRegenAmount = 0;
        public bool honeyTurboRegen = false;
        public bool honeyDew = false;
        public bool livingDew = false;
        public int jewelBonusDefense = 0;
        /// <summary>
        /// Counter variable for spawning Toxic Heart's pulses.<br/>
        /// Incremented on every frame by <see cref="pulseRate"/>, and when it reaches 420, it is reset and a pulse is spawned.
        /// </summary>
        public float pulseCounter = 0;
        /// <summary>
        /// The rate at which <see cref="pulseCounter"/> is incremented, and thus how often Toxic Heart's pulses are spawned.<br/>
        /// This value scales with how low the player's life regeneration is.
        /// </summary>
        public float pulseRate = 1;
        public bool aAmpoule = false;
        public bool rOoze = false;
        public bool fBarrier = false;
        public bool aBrain = false;
        public bool amalgam = false;
        public bool raiderTalisman = false;
        public int raiderCritLifespan = 0;
        public int raiderSoundCooldown = 0;
        public bool gSabaton = false;
        public int gSabatonHotkeyFallWindup = -1;
        public int gSabatonFall = 0;
        public bool gSabatonFalling = false;
        public int gSabatonTempJumpSpeed = 0;
        public bool rOfDelivarenceRam = false;
        public bool sGlyph = false;
        public bool sRegen = false;
        public bool tracersDust = false;
        public bool moonWalkers = false;
        public bool voidStriders = false;
        public bool seraphTracers = false;
        public bool frostFlare = false;
        public bool evolution = false;
        public bool v8Engine = false;
        public bool v8000Engine = false;
        public bool v8EngineFXPlayed = false;
        public bool procDodgeEffects = false;
        public bool nanotech = false;
        public int nanotechHitCooldown = 0;
        public bool deadshotBrooch = false;
        public bool shadowMinions = false;
        public bool holyMinions = false;
        public bool alchFlask = false;
        public bool toxicHeart = false;
        public bool toxicHeartVisuals = false;
        public bool abaddon = false;
        public int abaddonCooldown = 0;
        public bool abaddonEffectVisual = false;
        public float playerBaneDebuffDamage = 0;

        public bool aeroStone = false;
        public bool lifejelly = false;
        public bool cleansingjelly = false;
        public bool GrandGelatin = false;
        public int CleansingEffect = 0;
        public bool spawnedJellyAura = false;
        public bool community = false;
        public bool shatteredCommunity = false;
        public bool fleshTotem = false;
        public bool fleshTotemMinion = false;
        public bool fleshTotemVisual = false;
        public int fleshTotemManaStorage = 0;
        public bool bloodPact = false;
        public bool bloodflareCore = false;
        public int bloodflareCoreRemainingHealOverTime = 0;

        public bool chaliceOfTheBloodGod = false;
        public double chaliceBleedoutBuffer = 0D;
        public double chaliceDamagePointPartialProgress = 0D;
        public int chaliceBleedoutToApplyOnHurt = 0;
        public int chaliceHitOriginalDamage = 0;
        public bool chaliceHeartStyle = false;

        public bool elementalHeart = false;
        public bool crownJewel = false;
        public bool infectedJewel = false;
        public bool purity = false;
        /// <summary> If true, reduces the damage of electricity debuffs by 50%. </summary>
        public bool eleResist = false;
        public bool harpyRing = false;
        public bool angelTreads = false;
        /// <summary> Makes Flesh Knuckles and its upgrades increase the player's max health. </summary>
        public bool fleshKnuckles = false;
        public bool ironBoots = false;
        public bool depthCharm = false;
        public bool anechoicPlating = false;
        /// <summary> Used for increasing light level in the Abyss. </summary>
        public bool jellyfishNecklace = false;
        /// <summary> Calamity's Fairy Boots effect; makes fairies spawn around the player which give stats. </summary>
        public bool fairyBoots = false;
        /// <summary> Calamity's Flame Waker Boots effect; multiplies heat debuff damage and makes attacks inflict On Fire. </summary>
        public bool flameWakerBoots = false;
        /// <summary> Calamity's Hellfire Treads effect; multiplies heat debuff damage and makes attacks inflict Hellfire. </summary>
        public bool hellfireTreads = false;
        /// <summary>
        /// Used to prevent heat debuff damage stacking with Flame Waker Boots and Hellfire Treads.<br/>
        /// Flame Waker Boots = 1, Hellfire Treads = 2
        /// </summary>
        public int bootLevel = 0;
        public bool sSpiritAmulet = false;
        public int sSpiritAmuletTimer = 0;
        public bool sSpiritAmuletVisual = false;
        public bool dOfTheDeep = false;
        public int dOfTheDeepTimer = 0;
        public bool dOfTheDeepVisual = false;
        public int dOfTheDeepDefenseBuffMax = 420;
        public int dOfTheDeepDefenseBuffTimer = 0;
        public bool oceanCrest = false;
        public bool aquaticEmblem = false;
        public bool spiritOrigin = false;
        public bool spiritOriginVanity = false;
        public int spiritOriginCritBoost = 0;
        /// <summary>
        /// The amount of bonus crit damage the player has.
        /// At 0f, the player has regular crit damage. At 1f, the player has +100% crit damage.
        /// </summary>
        public float critDamage = 0;
        /// <summary>
        /// Multiplies the amount of coins the player gets from enemies.
        /// </summary>
        public float coinDropMult = 1f;
        public bool darkSunRing = false;
        public bool crawCarapace = false;
        public bool baroclaw = false;
        public bool IsFirstDashFrame = true;
        public int fallingBootVelCheckTimer = 0;
        public bool voidOfCalamity = false;
        public bool apollyon = false;
        public bool fragmentsOfAnotherWorld = false;
        public bool crushingEgo = false;
        public bool fadedIdolatry = false;
        public bool pSoulArtifact = false;
        public bool giantPearl = false;
        public bool normalityRelocator = false;
        public bool flameLickedShell = false;
        public int flameLickedShellParry = 0;
        public bool flameLickedShellEmpoweredParry = false;
        public bool sPauldron = false;
        public bool sPauldronVisual = false;
        public bool XykVisualsBlue = false;
        public bool XykVisualsOrange = false;
        public Color XykFXColor = Color.Black;
        public int XykWingTimer = 0;
        public Color lightRGB = Color.Black;
        public bool manaOverloader = false;
        /// <summary> Used for allowing Calamity slimes to be affected by Royal Gel. </summary>
        public bool royalGel = false;
        /// <summary> Used for implementing its synergy with Snow armor. </summary>
        public bool handWarmer = false;
        public bool ursaSergeant = false;
        public bool ursaSergeantVisual = false;
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
        /// <summary> General cooldown for accessories which spawn projectiles on minion hits. </summary>
        public float summonProjCooldown;
        public bool sandElemental = false;
        public bool sandElementalVanity = false;
        public bool oasisElemental = false;
        public bool oasisElementalVanity = false;
        public bool cloudElemental = false;
        public bool cloudElementalVanity = false;
        public bool brimElemental = false;
        public bool brimElementalVanity = false;
        public bool waterElemental = false;
        public bool waterElementalVanity = false;
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
        public bool mageCrownVisibility = false;
        public int mageCrownTimer = 0;
        public int mageCrownCount = 0;
        public bool dragonScales = false;
        public bool gloveOfPrecision = false;
        public bool gloveOfRecklessness = false;
        public bool vampiricTalisman = false;
        public bool electricianGlove = false;
        public bool bloodyGlove = false;
        public bool filthyGlove = false;
        public bool sandCloak = false;
        /// <summary> Solely used for granting the acceleration boost while within Sand Cloak's veil. Other stat boosts are directly given by the projectile. </summary>
        public bool getSandCloakAccelBoost = false;
        public bool spectralVeil = false;
        public int spectralVeilImmunity = 0;
        /// <summary> Check for if the player has Plagued Fuel Pack OR Blunder Booster equipped. </summary>
        public bool hasJetpack = false;
        public bool hasEngineDash = false;
        public bool plaguedFuelPack = false;
        public bool blunderBooster = false;
        public bool blunderBoosterVisibility = true;
        public int jetPackDash = 0;
        public int jetPackDirection = 0;
        public bool veneratedLocket = false;
        public bool camper = false;
        public bool corrosiveSpine = false;
        public bool scionsCurio = false;
        public bool scionsCurioGotHit = false;
        public bool scionsCurioVisuals = false;
        public float scionsCurioDebuffDamage = 0;
        public bool frozenCube = false;
        public bool frozenCubeVanity = false;
        public bool frozenCubeVisuals = false;
        public float frozenCubeDebuffBoost = 0;
        public float frozenCubeElumphantBoost = 0;
        public int frozenCubeUsedDefense = 0;
        public bool miniOldDuke = false;
        public bool miniOldDukeVanity = false;
        public bool starbusterCore = false;
        public bool starTaintedGenerator = false;
        public bool hallowedRune = false;
        public int hallowedRuneCooldown = 0;
        public bool phantomicArtifact = false;
        public int phantomicBulwarkCooldown = 0;
        /// <summary>
        /// Controls the state of Phantomic Artifact's Phantomic Regen boost.<br/>
        /// When the heart is spawned, this variable is set to 1000. When the heart is touched, it is set to 720 and decrements every frame.<br/>
        /// Life regeneration is increased while between 600 and 720, with a 10 second cooldown afterwards represented by being below 600.
        /// </summary>
        public int phantomicHeartRegen = 0;
        /// <summary> General cooldown variable for spawning projectiles from wing bonus effects. Used by Soul of Cryogen, Tattered Fairy Wings, and Festive Wings. </summary>
        public int wingProjectileCooldown = 0;
        public bool noStupidNaturalARSpawns = false;
        /// <summary> Used for animating Void Concentration Staff's draw layer. </summary>
        public int voidFrameCounter = 0;
        /// <summary> <inheritdoc cref="voidFrameCounter"/> </summary>
        public int voidFrame = 0;
        public bool rottenDogTooth = false;
        public bool angelicAlliance = false;
        public int angelicActivate = -1;
        public bool ChaosStone = false;
        public bool CryoStone = false;
        public bool CryoStoneVanity = false;
        /// <summary> Used for spawning Quiver of Nihility's void fields. </summary>
        public bool voidField = false;
        public bool copyrightInfringementShield = false;

        /// <summary>
        /// This is the maximum cooldown of general Dodge effects, in frames. Can be modified by equipment.
        /// Defaults to 90 seconds, or 5400 frames.
        /// </summary>
        public int ConsumableDodgeCooldown = 0;
        /// <summary>
        /// A list of on dodge effects for every dodge the player has equipped.
        /// Also used to determine the amount of dodges for cooldowns.
        /// Returns the string for the dodge cooldown icon to use.
        /// </summary>
        public List<Func<Player, Player.HurtInfo,string?>> DodgeEffects = [];
        /// <summary>
        /// Used internally to disable Player.shadowDodge so we can use custom dodge ordering.
        /// </summary>
        bool storedShadowDodge = false;
        #endregion

        #region Armor Set
        public int ArmorSetBonusKeyHeldTimer;
        /// <summary> Calamity's Silver armor set bonus; taking over 20 damage heals 10 health if the player avoids damage for 2 seconds. </summary>
        public bool silverMedkit = false;
        public int silverMedkitTimer = 0;
        /// <summary> Calamity's Tungsten armor set bonus; makes grappling hooks fly and retract faster. </summary>
        public bool tungstenArmorHookBoost = false;
        /// <summary> Calamity's Gold armor set bonus; makes enemies drop Gold Coins. </summary>
        public bool goldArmorGoldDrops = false;
        /// <summary> Calamity's Mining armor set bonus; gives a chance for extra items to drop when mining ores. </summary>
        public bool miningSet = false;
        public int miningSetCooldown = 0;
        public bool desertProwler = false;
        public bool snowRuffianSet = false;
        public bool forbiddenCirclet = false;
        public int forbiddenCooldown = 0;
        public int tornadoCooldown = 0;
        /// <summary> Calamity's Snow armor set bonus; reduces cold enemy damage and increases cold debuff damage. </summary>
        public bool eskimoSet = false;
        /// <summary> Calamity's Rain armor set bonus; increases jump speed and makes jumps create a damaging splash. </summary>
        public bool rainSet = false;
        /// <summary> Calamity's Meteor armor set bonus; makes all magic guns cost 33% mana instead of Space Gun costing 0 mana. </summary>
        public bool meteorSet = false;
        /// <summary> Calamity's Necro armor set bonus; gives a temporary 10 second revive when the player is killed before actually dying. </summary>
        public bool necroSet = false;
        /// <summary> Calamity's Frost armor set bonus; gives a combined 20% damage boost split between melee and ranged based on distance from the closest enemy. </summary>
        public bool frostSet = false;
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
        public bool ataxiaVolley = false;
        public bool ataxiaBlaze = false;
        public bool hydrothermalSmoke = false;
        public bool daedalusAbsorb = false;
        public bool daedalusShard = false;
        public bool brimflameSet = false;
        public bool brimflameFrenzy = false;
        public bool lunicCorpsSet = false;
        public bool lunicCorpsLegs = false;
        /// <summary> Demonshade Breastplate's life regeneration boost. </summary>
        public bool shadeRegen = false;
        /// <summary> Demonshade Greaves' movement speed boost. </summary>
        public bool shadowSpeed = false;
        public bool dsSetBonus = false;
        public bool auricSetMelee = false;
        public bool daedalusReflect = false;
        public bool daedalusSplit = false;
        public bool titanHeartSet = false;
        public bool titanHeartMask = false;
        public bool titanHeartMantle = false;
        public int titanCooldown = 0;
        public bool umbraphileSet = false;
        public bool reaverSpeed = false;
        public bool reaverDefense = false;
        public bool reaverExplore = false;
        public bool fathomSwarmer = false;
        public bool fathomSwarmerVisage = false;
        public bool fathomSwarmerBreastplate = false;
        public bool fathomSwarmerTail = false;
        /// <summary> Used for animating Fathom Swarmer armor's tail layer. </summary>
        public int tailFrameUp = 0;
        /// <summary> <inheritdoc cref="tailFrameUp"/> </summary>
        public int tailFrame = 0;
        public bool astralStarRain = false;
        public int astralStarRainCooldown = 0;
        public int VoidCooldown = 0;
        public int ursaSergeantCooldown = 0;
        public int AlchFlaskCooldown = 0;
        public bool plagueReaper = false;
        public bool plaguebringerPatronSet = false;
        public bool plaguebringerCarapace = false;
        public float ataxiaDmg;
        public float hydroHealTimer = 0;
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
        public int silvaCountdown = SilvaArmor.ReviveDuration;
        public bool auricSet = false;
        public bool omegaBlueChestplate = false;
        public bool omegaBlueSet = false;
        public bool omegaBlueAbyssalMadness = false;
        /// <summary> Aerospec armor's summoner set bonus minion. </summary>
        public bool valkyrie = false;
        /// <summary> Statigel armor's summoner set bonus minion. </summary>
        public bool slimeGod = false;
        public bool molluskHelmet = false;
        public bool molluskChest = false;
        public bool molluskLegs = false;
        public bool fearmongerSet = false;
        public int fearmongerRegenFrames = 0;
        public bool daedalusCrystal = false;
        /// <summary> Hydrothermic armor's summoner set bonus minion. </summary>
        public bool chaosSpirit = false;
        public bool GemTechSet = false;
        /// <summary> Calamity's Cobalt armor set bonus; increases damage and crit chance based on how fast the player is moving. </summary>
        public bool CobaltSet = false;
        /// <summary> Calamity's Mythril armor set bonus; hits spawn additional homing mythril flares. </summary>
        public bool MythrilSet = false;
        public int MythrilFlareSpawnCountdown = 0;
        /// <summary> Calamity's Adamantite armor set bonus; adds half of DR to crit chance, and makes landing hits give a stacking defense boost. </summary>
        public bool AdamantiteSet = false;
        public int AdamantiteSetDecayDelay = 0;
        public int ChlorophyteHealDelay = 0;
        /// <summary>
        /// If true, the player is wearing a post-Moon Lord summoner armor set.<br/>
        /// Currently unused.
        /// </summary>
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
        public bool daybroken = false;
        public bool whisperingDeath = false;
        public bool dragonFire = false;
        public bool vermillionFlux = false;
        public bool auricRebuke = false;
        public bool staticDischarge = false;
        public bool miracleBlight = false;
        public bool armorCrunch = false;
        public bool irradiated = false;
        public bool bane = false;
        public bool brimstoneFlames = false;
        public bool weakBrimstoneFlames = false;
        public bool demonicFlames = false;
        public bool godSlayerInferno = false;
        public bool astralInfection = false;
        public bool plague = false;
        public bool holyFlames = false;
        public bool holyInferno = false;
        public bool burningBlood = false;
        public bool brainRot = false;
        public bool heavybleeding = false;
        public bool laceration = false;
        public bool elementalMix = false;
        public bool icarusFolly = false;
        public bool weakPetrification = false;
        public bool vHex = false;
        public bool trueVHex = false;
        public bool DoGExtremeGravity = false;
        public bool warped = false;
        public bool crushDepth = false;
        public bool riptide = false;
        public bool hadopelagicPressure = false;
        public bool fishAlert = false;
        public bool malnourished = false;
        public bool clamity = false;
        public bool NOU = false;
        public bool absorberAffliction = false;
        public bool sulphurPoison = false;
        public bool nightwither = false;
        public bool voidfrost = false;
        public bool eutrophication = false;
        public bool frozenLungs = false;
        public bool searingLava = false;
        public bool vaporfied = false;
        public bool banishingFire = false;
        public bool wither = false;
        public bool windChilled = false;
        public bool ManaBurn = false;

        /// <summary> Counter variable used to prevent the player from being inflicted with another immobilizing debuff for a short time after being inflicted with one. </summary>
        public int ImmobilityDebuffImmunityTimer = 0;
        /// <summary> Constant variable representing the time in which players cannot be inflicted with another immobilizing debuff, in frames. </summary>
        public const int ImmobilityDebuffImmunityTimerMax = 300;

        public const int SulphSeaWaterSafetyTime = 720;
        public const int SulphSeaWaterRecoveryTime = 150;
        #endregion

        #region Buff
        public bool sandsWindBuff = false;
        public bool aeolianEarthBuff = false;
        public int chiBuffTimer = 0;
        public bool corrEffigy = false;
        public bool crimEffigy = false;
        public bool decayEffigy = false;
        /// <summary> Reaver Rage buff. </summary>
        public bool rRage = false;
        /// <summary> Tarra Life buff. </summary>
        public bool tRegen = false;
        /// <summary> Empyrean Wrath buff. </summary>
        public bool xWrath = false;
        public bool graxDefense = false;
        public bool encased = false;
        public bool omniscience = false;
        public bool zerg = false;
        public bool zen = false;
        public bool isNearbyBoss = false;
        public bool flaskBrimstone = false;
        public bool purpleHaze = false;
        public int purpleHazeStealthTimer = 0;
        public bool mushy = false;
        public bool PinkJellyRegen = false;
        public bool GreenJellyRegen = false;
        public bool AbsorberRegen = false;
        public bool cFreeze = false;
        /// <summary> Used for increasing light level in the Abyss. </summary>
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
        public bool galvanicCorrosion = false;
        public bool sulphurskin = false;
        public bool baguette = false;
        public bool vodka = false;
        public bool redWine = false;
        public float redWineStoredY = 0;
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
        public float whiteWineTimer = 0;
        public bool evergreenGin = false;
        public bool baconOil = false;
        public int baconOilSoundCooldown = 0;
        public bool tranquilityCandle = false;
        public bool chaosCandle = false;
        public bool blueCandle = false;
        public bool pinkCandle = false;
        public double pinkCandleHealFraction = 0D;
        public bool yellowCandle = false;
        /// <summary> If true, the player has consumed Odd Mushroom. Controls drawing its rainbow fake clones of NPCs and projectiles. </summary>
        public bool trippy = false;
        public bool amidiasBlessing = false;
        public bool bloodfinBoost = false;
        public int bloodfinTimer = 30;
        public bool hallowedRegen = false;
        public bool hallowedPower = false;
        public bool avertorBonus = false;
        public bool divineBless = false;
        public bool infiniteFlight = false;
        /// <summary>
        /// Counter variable for natural decay of Haste buffs from Chronomancer's Scythe.<br/>
        /// If the player does not collect a clock powerup for 5 seconds, they will lose a stack of Haste.
        /// </summary>
        public int hasteCounter = 0;
        /// <summary> How many stacks of the Haste buff the player has, from Chronomancer's Scythe. </summary>
        public int hasteLevel = 0;
        #endregion

        #region Minion
        public bool wDroid = false;
        public bool resButterfly = false;
        public bool hasVoidEaterMarionette = false;
        public bool IceClasperBool = false;
        public bool magicHat = false;
        public bool herring = false;
        public bool blackhawk = false;
        public bool cosmicViper = false;
        public bool CalamarisLament = false;
        /// <summary> Entropy's Vigil. </summary>
        public bool cEyes = false;
        /// <summary> Corroslime Staff. </summary>
        public bool cSlime = false;
        /// <summary> Crimslime Staff. </summary>
        public bool cSlime2 = false;
        /// <summary> Abandoned Slime Staff. </summary>
        public bool aSlime = false;
        public bool brittleStar = false;
        public bool aquaticStar = false;
        /// <summary> Sun Spirit Staff. </summary>
        public bool sunSpirit = false;
        public bool dCreeper = false;
        public bool eAxe = false;
        public bool endoCooper = false;
        /// <summary> Vengeful Sun Staff. </summary>
        public bool vengefulSunMinion = false;
        public bool sirius = false;
        /// <summary> Yharon's Kindle Staff. </summary>
        public bool aChicken = false;
        public bool cLamp = false;
        /// <summary> Ethereal Subjugator. </summary>
        public bool pGuy = false;
        public bool sandnado = false;
        public bool PlantationSummon = false;
        public bool astralProbe = false;
        /// <summary> Profaned Soul Artifact/Crystal guardians. </summary>
        public bool pSoulGuardians = false;
        /// <summary> Counter variable used for healing the player by Profaned Soul Artifact's healer guardian. </summary>
        public int healCounter = 300;
        /// <summary> Cosmic Immaterializer. </summary>
        public bool cEnergy = false;
        public bool shellfish = false;
        /// <summary> Enchanted Conch. </summary>
        public bool hCrab = false;
        /// <summary> Heart of the Elements. </summary>
        public bool allElementals = false;
        /// <summary> Heart of the Elements; however, the minions will not attack. </summary>
        public bool allElementalsVanity = false;
        /// <summary> Silva armor's Silva Crystal. </summary>
        public bool sCrystal = false;
        /// <summary> Elemental in a Bottle. </summary>
        public bool sandEleBuff = false;
        /// <summary> Rare Elemental in a Bottle. </summary>
        public bool oasisEleBuff = false;
        /// <summary> Eye of the Storm. </summary>
        public bool cloudEleBuff = false;
        /// <summary> Rose Stone. </summary>
        public bool brimEleBuff = false;
        /// <summary> Pearl of Enthrallment. </summary>
        public bool waterEleBuff = false;
        public bool fClump = false;
        /// <summary> Aerospec armor's Valkyrie. </summary>
        public bool aValkyrie = false;
        public bool apexShark = false;
        public bool gastricBelcher = false;
        public bool hauntedDishes = false;
        public bool stormjaw = false;
        /// <summary> Statigel armor's Baby Paladin. </summary>
        public bool sGod = false;
        public bool victideSnail = false;
        /// <summary> Hydrothermic armor's Vent. </summary>
        public bool cSpirit = false;
        /// <summary> Daedalus armor's Daedalus Crystal. </summary>
        public bool dCrystal = false;
        public bool endoHydra = false;
        /// <summary> Corvid Harbinger Staff. </summary>
        public bool powerfulRaven = false;
        /// <summary> Dragonblood Disgorger. </summary>
        public bool dragonFamily = false;
        public bool providenceStabber = false;
        public bool seashineSwordBuff = false;
        public bool saros = false;
        public int sarosEclipseBeamUsage = 0;
        /// <summary> Fuel Cell Bundle. </summary>
        public bool plaguebringerMK2 = false;
        public bool igneousExaltation = false;
        public bool GlacialEmbrace = false;
        public bool VoidConcentrationStaff = false;
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
        public bool snakeEyes = false;
        public bool poleWarper = false;
        public bool aqueousHunterDrone = false;
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
        public bool ViridVanguardActiveAttackerThisFrame = false;
        public float ViridVanguardRotation = 0;
        public float ViridVanguardActiveCooldown = 0;
        public float ViridVanguardRotationToAdd = 0;
        public bool InvertExaltationLineRotationDirections = false;
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
        public bool EnchantedKnifeStaffBool = false;
        public bool AmphibiansGuitarBool = false;
        #endregion

        #region Whip
        public bool forceSummonTagCrit = false; //Forces whip multiplicative effects to function as crit chance
        public bool forceSummonTagMultiplicative = false; //Forces whip multiplicative effects to function as multiplicative damage

        //Allows providing flat, crit, and multiplicative tag values on gear. These will apply on *any* minion or sentry hit, not just if the target is tagged.
        public int bonusFlatTag = 0;
        public float bonusCritTag = 0;
        public float bonusMultTag = 0;
        #endregion

        #region Biome
        public bool ZoneSunkenSea => ZoneTimelessShores || ZoneRadiantReefs || ZonePolypForest || ZoneGleamingBurrows || ZoneClamDen || ZoneBasaltGully;
        public bool ZoneTimelessShores => Player.InModBiome<TimelessShoresBiome>();
        public bool ZonePolypForest => Player.InModBiome<PolypForestBiome>();
        public bool ZoneRadiantReefs => Player.InModBiome<RadiantReefsBiome>();
        public bool ZoneGleamingBurrows => Player.InModBiome<GleamingBurrowsBiome>();
        public bool ZoneClamDen => Player.InModBiome<ClamDenBiome>();
        public bool ZoneBasaltGully => Player.InModBiome<BasaltGullyBiome>();

        public bool ZoneSulphur => Player.InModBiome<SulphurousSeaBiome>();
        public bool ZoneAbyss => ZoneAbyssLayer1 || ZoneAbyssLayer2 || ZoneAbyssLayer3 || ZoneAbyssLayer4;
        public bool ZoneAbyssLayer1 => Player.InModBiome<AbyssLayer1Biome>();
        public bool ZoneAbyssLayer2 => Player.InModBiome<AbyssLayer2Biome>();
        public bool ZoneAbyssLayer3 => Player.InModBiome<AbyssLayer3Biome>();
        public bool ZoneAbyssLayer4 => Player.InModBiome<AbyssLayer4Biome>();

        public bool ZoneCalamity => Player.InModBiome<BrimstoneCragsBiome>();

        public bool ZoneAstral => Player.InModBiome<AstralInfectionBiome>() && !ZoneAbyss;

        public bool InAnyCalamityBiome => ZoneAbyss || ZoneCalamity || ZoneSulphur || ZoneSunkenSea || ZoneAstral;

        public bool abyssDeath = false;
        public int abyssBreathCD;
        public float caveDarkness = 0f;
        #endregion

        #region Transformation
        public bool abyssalDivingSuit;
        public bool abyssalDivingSuitPrevious;
        public bool profanedCrystal;
        public int profanedCrystalStatePrevious;
        public bool profanedCrystalPrevious;
        public int profanedCrystalAnim;
        public bool profanedCrystalBuffs;

        public int pscState;
        public Color pscLerpColor = Color.White;
        public bool aquaticHeartPrevious;
        public bool aquaticHeart;
        public bool snowmanNoseless;
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
        /// <summary> If true, the player is holding an item with the Indignant enchantment. </summary>
        public bool cursedSummonsEnchant = false;
        /// <summary> If true, the player is holding an item with the Aflame enchantment. </summary>
        public bool flamingItemEnchant = false;
        /// <summary> If true, the player is holding an item with the Oblatory enchantment. </summary>
        public bool lifeManaEnchant = false;
        /// <summary> If true, the player is holding an item with the Resentful enchantment. </summary>
        public bool farProximityRewardEnchant = false;
        /// <summary> If true, the player is holding an item with the Bloodthirsty enchantment. </summary>
        public bool closeProximityRewardEnchant = false;
        /// <summary> If true, the player is holding an item with the Ephemeral enchantment. </summary>
        public bool dischargingItemEnchant = false;
        /// <summary> If true, the player is holding an item with the Hellbound enchantment. </summary>
        public bool explosiveMinionsEnchant = false;
        /// <summary> If true, the player is holding an item with the Tainted enchantment. </summary>
        public bool bladeArmEnchant = false;
        /// <summary> If true, the player is holding an item with the Traitorous enchantment. </summary>
        public bool manaMonsterEnchant = false;

        /// <summary> If true, the player is holding an item with the Withering enchantment. </summary>
        public bool witheringWeaponEnchant = false;
        public bool witheredDebuff = false;
        /// <summary>
        /// Counter variable which controls negative life regeneration from the Withered debuff.<br/>
        /// Increments every frame a weapon with the Withering enchantment is held, and decrements vice versa.
        /// </summary>
        public int witheredWeaponHoldTime = 0;
        /// <summary>
        /// How much damage was dealt by a weapon with the Withering enchantment.<br/>
        /// Used to determine what percentage of the hit to heal back.
        /// </summary>
        public int witheringDamageDone = 0;

        /// <summary> If true, the player is holding an item with the Persecuted enchantment. </summary>
        public bool persecutedEnchant = false;
        public int persecutedEnchantSummonTimer = 0;

        /// <summary> If true, the player is holding an item with the Lecherous enchantment. </summary>
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
        public FireParticleSet ManaBurnFireDrawer = null;
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

        public float oldGravDir = 1;
        public float tempGravDir = 1;
        public bool justChangedGravity = false;

        public Vector2 mouseWorld => Player.MountedCenter + mouseWorldDeltaFromPlayer;
        public Vector2 mouseNormalFromPlayer => mouseRotationFromPlayer.ToRotationVector2();
        public Vector2 mouseWorldDeltaFromPlayer;
        public float mouseRotationFromPlayer;

        private Vector2 oldMouseWorldDeltaFromPlayer;
        private int mouseWorldPacketTimer = 0;
        private const int MouseWorldPacketInterval = 2;

        /// <summary>
        /// Set this to true if you need to receive updates on right clicks from players and sync them in multiplayer.<br/>
        /// Automatically resets itself after sending an update.
        /// </summary>
        public bool rightClickListener = false;
        /// <summary>
        /// Set this to true if you need to receive updates on the position of the player's mouse and sync them in multiplayer.<br/>
        /// Automatically resets itself after sending an update.<br/>
        /// This also update the rotation.
        /// </summary>
        public bool mouseWorldListener = false;
        /// <summary>
        /// Set this to true if you need to receive updates on the rotation of the mouse to the player. This sends updates less frequently the tighter the tolerance of mouseWorldListener.<br/>
        /// Automatically resets itself after sending an update.<br/>
        /// This does NOT update the position.
        /// </summary>
        public bool mouseRotationListener = false;

        public bool syncMousePosition = false;
        public bool syncMouseRotation = false;
        public bool syncMouseRightClick = false;
        #endregion

        #endregion

        #region Saving And Loading
        public override void Initialize()
        {
            extraAccessoryML = false;
            eCore = false;
            mFruit = false;
            sTangerine = false;
            tCloudberry = false;
            sStrawberry = false;
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
            boost.AddWithCondition("bloodOrange", sTangerine);
            boost.AddWithCondition("elderBerry", tCloudberry);
            boost.AddWithCondition("dragonFruit", sStrawberry);
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
            boost.AddWithCondition("newAmidiasInventory", newAmidiasInventory);
            boost.AddWithCondition("newBanditInventory", newBanditInventory);
            boost.AddWithCondition("newCalamitasInventory", newCalamitasInventory);
            boost.AddWithCondition("GivenBrimstoneLocus", GivenBrimstoneLocus);
            boost.AddWithCondition("HasTalkedAtCodebreaker", HasTalkedAtCodebreaker);
            boost.AddWithCondition("HasCraftedDraedonsForge", HasCraftedDraedonsForge);

            // Calculate the new total time of all sessions at the instant of this player save.
            TimeSpan newSessionTotal = previousSessionTotal.Add(SpeedrunTimerSystem.Elapsed);
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
            sTangerine = boost.Contains("bloodOrange");
            tCloudberry = boost.Contains("elderBerry");
            sStrawberry = boost.Contains("dragonFruit");
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
            if (!areThereAnyDamnBosses)
            {
                DoGHeadHitCounter = 0;
            }
            ViridVanguardActiveAttackerThisFrame = Main.projectile.Any(x => x.active && x.type == ModContent.ProjectileType<ViridVanguardBlade>() && x.owner == Player.whoAmI && x.ModProjectile<ViridVanguardBlade>().CurrentState == ViridVanguardBlade.ViridVanguardAIState.PhotonRipperZenithSlashes);
            ViridVanguardRotation = MathHelper.WrapAngle(ViridVanguardRotation + ViridVanguardRotationToAdd);
            ViridVanguardRotationToAdd = ViridVanguard.IdleCirclingSpeed;

            if (Player.HeldItem.type == ModContent.ItemType<ViridVanguard>())
            {
                if (ViridVanguardActiveCooldown > 0)
                    ViridVanguardActiveCooldown--;
            }
            else if (ViridVanguardActiveCooldown >= 0 && ViridVanguardActiveCooldown < ViridVanguard.ActiveAttackCooldown)
                ViridVanguardActiveCooldown++;
            if (fleshKnuckles)
                Player.statLifeMax2 += 25;

                int percentMaxLifeIncrease = 0;
            // Blood Pact and Chalice of the Blood God stack their HP bonuses if you want to equip both
            if (bloodPact)
                percentMaxLifeIncrease += 25;
            if (chaliceOfTheBloodGod)
                percentMaxLifeIncrease += 25;

            if (affliction || afflicted)
                percentMaxLifeIncrease += Affliction.MaxLifeBoostPercent;

            if (community)
                percentMaxLifeIncrease += (int)(TheCommunity.CalculatePower() * TheCommunity.HealthMultiplier);

            // Shattered Community gives the same max health boost as normal full-power Community (10%)
            if (shatteredCommunity)
                percentMaxLifeIncrease += 10;

            Player.statLifeMax2 += Player.statLifeMax / 5 / 20 * percentMaxLifeIncrease;

            // Max health reductions
            if (crimEffigy)
                Player.statLifeMax2 = (int)(Player.statLifeMax2 * (1f - CrimsonEffigy.MaxHealthLossPercent));

            ResetRogueStealth();

            calamityBonusLuck = 0f;
            combHair = false;

            // Reset adrenaline duration to default. If Draedon's Heart is equipped, it'll change itself every frame.
            AdrenalineDuration = CalamityUtils.SecondsToFrames(5);

            defenseDamageRatio = BalancingConstants.DefaultDefenseDamageRatio;
            contactDamageReduction = 0D;
            projectileDamageReduction = 0D;
            rogueVelocity = 1f;
            accStealthGenBoost = 0f;

            DashID = string.Empty;

            externalBreathTickBoost = 0f;
            externalFlightTimeMultBoost = 0f;
            externalRageEnabled = externalAdrenalineEnabled = null;
            externalColdImmunity = externalHeatImmunity = false;
            externalDefenseDamageImmunity = false;
            externalAuricRejectionImmunity = false;

            alcoholPoisonLevel = 0;
            alcoholPoisonMax = alcoholPoisonMaxBase;
            noLifeRegen = false;

            //Stratus Starburst amount management

            if (HalleyAccuracyCounter > HalleysInferno.MaxAccuracy)
                HalleyAccuracyCounter = HalleysInferno.MaxAccuracy;
            if (HalleyAccuracyCounter < 0)
                HalleyAccuracyCounter = 0;
            while (StarburstSpawnFrameCounter >= 1)
            {
                StratusStarburst++;
                StarburstSpawnFrameCounter--;
            }
            if (Starshield > 0)
            {
                Starshield--;
                StratusStarburstResetTimer = (int)MathHelper.Max(300,StratusStarburstResetTimer);
            }
            if (StratusStarburstResetTimer > 0)
                StratusStarburstResetTimer--;
            else if (StratusStarburst > 0)
                StratusStarburst--;
            if (StratusStarburst > MaxStratusStarburst)
                StratusStarburst = MaxStratusStarburst;
            var starpower = 0;
            var avaliableStarpower = 0;
            var oneCount = 0;
            Vector2 starSpawnPos = Player.Center;
            for (var i = 0; i < StarburstEntities.Count(); i++)
            {
                starpower += StarburstEntities[i].value;
                if (StarburstEntities[i].AICooldown <= 0)
                    avaliableStarpower += StarburstEntities[i].value;
                if (starpower > StratusStarburst)
                {
                    var deadStar = StarburstEntities[i];
                    starpower -= deadStar.value;
                    StarburstEntities.RemoveAt(i);
                    starSpawnPos = deadStar.Center;
                    i--;
                    continue;
                }
                if (StarburstEntities[i].value == 1)
                    oneCount++;
            }
            while (starpower < StratusStarburst)
            {
                StarburstEntities.Add(new(starSpawnPos));
                oneCount++;
                starpower++;
                avaliableStarpower++;
            }
            while (oneCount >= 10)
            {
                var bigStar = StarburstEntities.First(x => x.value == 1);
                bigStar.value = 10;
                for (var i = 0; i < 9; i++)
                {
                    var star = StarburstEntities.Last(x => x.value == 1);
                    bigStar.MergeChildren.Add(star);
                    star.MergeTarget = bigStar;
                    StarburstEntities.Remove(star);
                }
                oneCount -= 10;
            }
            AvaliableStarburst = avaliableStarpower;
            if (StratusStarburstResetTimer > 0)
            {
                if (cooldowns.TryGetValue(Starburst.ID, out var cooldown))
                {
                    cooldown.timeLeft = MaxStratusStarburst - StratusStarburst;
                }
                else
                {
                    Player.AddCooldown(Starburst.ID, MaxStratusStarburst);
                }
            }
            
            if (Player.whoAmI == Main.myPlayer)
            {
                if (!roverDrive)
                    RoverDriveShieldDurability = 0;
                if (!lunicCorpsSet)
                    LunicCorpsShieldDurability = 0;
                if (!sponge)
                    SpongeShieldDurability = 0;
                if (!pSoulArtifact)
                    pSoulShieldDurability = 0;
            }
            friendlyMinions = 0;
            pSoulShieldVisible = false;
            roverDrive = false;
            roverDriveShieldVisible = false;
            sponge = false;
            spongeShieldVisible = false;

            thirdSage = false;
            perfmini = false;
            akato = false;
            yharonPet = false;
            burrowerPet = false;
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
            frostyBat = false;
            toastyBat = false;
            beldum = false;
            starSwallowerPetFroge = false;

            onyxExcavator = false;
            rimehound = false;
            crysthamyr = false;
            ExoChair = false;
            miniOldDuke = false;
            miniOldDukeVanity = false;

            aquaticHeartWaterBuff = false;
            aquaticHeartIce = false;

            draedonsHeart = false;

            afflicted = false;
            chiRegen = false;
            affliction = false;

            dodgeScarf = false;
            evasionScarf = false;

            nebulousCore = false;

            godSlayer = false;
            godSlayerDamage = false;
            godSlayerRanged = false;
            godSlayerThrowing = false;

            silvaSet = false;
            silvaMage = false;
            silvaSummon = false;

            auricSet = false;
            auricSetMelee = false;

            GemTechSet = false;

            CobaltSet = false;
            MythrilSet = false;
            AdamantiteSet = false;

            WearingPostMLSummonerSet = false;

            omegaBlueChestplate = false;
            omegaBlueSet = false;
            omegaBlueAbyssalMadness = false;

            molluskHelmet = false;
            molluskChest = false;
            molluskLegs = false;
            fearmongerSet = false;

            ataxiaBolt = false;
            ataxiaGeyser = false;
            ataxiaVolley = false;
            ataxiaBlaze = false;
            ataxiaMage = false;

            shadeRegen = false;

            shadowSpeed = false;
            dsSetBonus = false;
            wearingRogueArmor = false;

            blockAllDashes = false;
            blazingCursorDamage = false;
            blazingCursorVisuals = false;

            yharimsGift = false;
            luxorsGift = false;
            luxorsGiftVanity = false;
            fungalSymbiote = false;
            trinketOfChi = false;
            gladiatorSword = false;
            unstableGraniteCore = false;
            regenerator = false;
            deepDiver = false;
            theBee = false;
            arcFlashRing = false;
            arcFlashRingVisual = false;
            bGlassBand = false;
            bGlassBandVisual = false;
            batholithBangle = false; // Granite band
            batholithBangleVisual = false;
            protolithBangle = false; // Marble band
            protolithBangleVisual = false;
            aFossil = false;
            aPowder = false;
            fallingBlockProtection = false;
            trapProtection = false;
            alluringBait = false;
            enchantedPearl = false;
            fishingStation = false;
            rBrain = false;
            bloodyWormTooth = false;
            ivDrip = false;
            vexation = false;
            badgeOfBravery = false;
            // Clear the Warbanner "cooldown" if not wearing Warbanner. This has absolutely zero effect for a casual player, but is useful for resetting the cooldown's duration.
            if (!WarbanneroftheRighteous)
                cooldowns.Remove(WarbanneroftheRighteousBuff.ID);
            WarbanneroftheRighteous = false;
            warbannerGlow = false;
            ilSpark = false;
            transformer = false;
            transformerVisual = false;
            bloomStone = false;
            bloomStoneHookVisuals = false;
            hideOfDeus = false;
            dAmulet = false;
            rampartOfDeities = false;
            gShell = false;
            lAmbergris = false;
            tortShell = false;
            absorber = false;
            honeyDew = false;
            livingDew = false;
            aAmpoule = false;
            rOoze = false;
            fBarrier = false;
            aBrain = false;
            amalgam = false;
            frostFlare = false;
            evolution = false;
            v8Engine = false;
            v8000Engine = false;
            nanotech = false;
            nanotechHitCooldown = 0;
            deadshotBrooch = false;
            tesla = false;
            teslaVisuals = true;
            cryogenSoul = false;
            ascendantInsignia = false;
            fishStocks = false;
            magmaStoneVisuals = true;
            eGauntlet = false;
            eGauntletVisuals = true;
            gloveLevel = 0;
            if (Player.dashDelay != -1)
                statisNinjaBelt = false;
            if (Player.dashDelay != -1)
                statisVoidSash = false;
            alreadyHasFrogLeg = false;
            eTalisman = false;
            nucleogenesis = false;
            nuclearFuelRod = false;
            heartOfDarkness = false;
            profanedSoulRelicBuff = false;
            shadowMinions = false;
            holyMinions = false;
            alchFlask = false;
            toxicHeart = false;
            toxicHeartVisuals = false;
            abaddon = false;
            aeroStone = false;
            lifejelly = false;
            GrandGelatin = false;
            cleansingjelly = false;
            spawnedJellyAura = false;
            community = false;
            shatteredCommunity = false;
            stressPills = false;
            laudanum = false;
            fleshTotem = false;
            fleshTotemVisual = false;
            fleshTotemMinion = false;
            bloodPact = false;
            bloodflareCore = false;
            chaliceOfTheBloodGod = false;
            chaliceHeartStyle = false;
            chaliceBleedoutToApplyOnHurt = 0; // Resets every frame so it doesn't improperly carry over between hits
            elementalHeart = false;
            crownJewel = false;
            infectedJewel = false;
            purity = false;
            harpyRing = false;
            angelTreads = false;
            fleshKnuckles = false;
            darkSunRing = false;
            crawCarapace = false;
            baroclaw = false;
            voidOfCalamity = false;
            apollyon = false;
            fragmentsOfAnotherWorld = false;
            crushingEgo = false;
            fadedIdolatry = false;
            pSoulArtifact = false;
            giantPearl = false;
            normalityRelocator = false;
            flameLickedShell = false;
            sPauldron = false;
            XykVisualsBlue = false;
            XykVisualsOrange = false;
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
            tracersDust = false;
            moonWalkers = false;
            voidStriders = false;
            seraphTracers = false;
            ursaSergeant = false;
            ursaSergeantVisual = false;
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
            corrosiveSpine = false;
            scionsCurio = false;
            frozenCube = false; frozenCubeVanity = false; frozenCubeDebuffBoost = 0; frozenCubeElumphantBoost = 0; frozenCubeUsedDefense = 0;
            rottenDogTooth = false;
            angelicAlliance = false;
            ChaosStone = false;
            CryoStone = false;
            CryoStoneVanity = false;
            voidField = false;
            copyrightInfringementShield = false;

            ConsumableDodgeCooldown = BalancingConstants.DodgeCooldownMax;
            DodgeEffects = [];

        daedalusReflect = false;
            daedalusSplit = false;
            daedalusAbsorb = false;
            daedalusShard = false;

            brimflameSet = false;
            brimflameFrenzy = false;

            lunicCorpsSet = false;
            lunicCorpsLegs = false;

            ammoCost = 1f;
            partialLifeRegen = 0f;
            healingPotionMultiplier = 1f;

            avertorBonus = false;

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
            bootLevel = 0;
            sSpiritAmulet = false;
            dOfTheDeep = false;
            oceanCrest = false;
            aquaticEmblem = false;
            if (!spiritOrigin)
                spiritOriginCritBoost = 0;
            spiritOrigin = false;
            spiritOriginVanity = false;
            critDamage = 0;
            if (abaddonCooldown > 0)
                abaddonCooldown--;


            astralStarRain = false;

            desertProwler = false;

            snowRuffianSet = false;

            forbiddenCirclet = false;

            silverMedkit = false;
            tungstenArmorHookBoost = false;
            goldArmorGoldDrops = false;

            miningSet = false;

            eskimoSet = false;
            rainSet = false;
            meteorSet = false;
            necroSet = false;
            frostSet = false;

            victideSet = false;
            victideSummoner = false;

            sulphurSet = false;

            aeroSet = false;

            statigelSet = false;

            titanHeartSet = false;
            titanHeartMask = false;
            titanHeartMantle = false;
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
            mageCrownVisibility = false;
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
            hasEngineDash = false;
            plaguedFuelPack = false;
            blunderBooster = false;
            blunderBoosterVisibility = true;
            veneratedLocket = false;

            if (DemonAltarDialogueCooldown > 0)
                DemonAltarDialogueCooldown--;

            alcoholPoisoning = false;
            shadowflame = false;
            daybroken = false;
            whisperingDeath = false;
            dragonFire = false;
            vermillionFlux = false;
            auricRebuke = false;
            staticDischarge = false;
            miracleBlight = false;
            armorCrunch = false;
            irradiated = false;
            bane = false;
            brimstoneFlames = false;
            witheredDebuff = false;
            absorberAffliction = false;
            weakBrimstoneFlames = false;
            demonicFlames = false;
            godSlayerInferno = false;
            astralInfection = false;
            plague = false;
            holyFlames = false;
            holyInferno = false;
            burningBlood = false;
            brainRot = false;
            heavybleeding = false;
            laceration = false;
            elementalMix = false;
            icarusFolly = false;
            vHex = false;
            trueVHex = false;
            DoGExtremeGravity = false;
            warped = false;
            crushDepth = false;
            riptide = false;
            hadopelagicPressure = false;
            fishAlert = false;
            malnourished = false;
            clamity = false;
            NOU = false;
            enraged = false;
            snowmanNoseless = false;
            sulphurPoison = false;
            nightwither = false;
            voidfrost = false;
            eutrophication = false;
            frozenLungs = false;
            searingLava = false;
            vaporfied = false;
            banishingFire = false;
            wither = false;
            windChilled = false;
            ManaBurn = false;

            TypelessDebuffMultiplier = new();
            HeatDebuffMultiplier = new();
            ColdDebuffMultiplier = new();
            SicknessDebuffMultiplier = new();
            WaterDebuffMultiplier = new();
            ElectricDebuffMultiplier = new();

            sandsWindBuff = false;
            aeolianEarthBuff = false;
            corrEffigy = false;
            crimEffigy = false;
            decayEffigy = false;
            rRage = false;
            xWrath = false;
            graxDefense = false;
            encased = false;
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
            galvanicCorrosion = false;
            sulphurskin = false;
            baguette = false;
            trippy = false;
            amidiasBlessing = false;
            flaskBrimstone = false;
            purpleHaze = false;
            if (purpleHazeStealthTimer > 0)
                purpleHazeStealthTimer--;
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
            baconOil = false;

            tranquilityCandle = false;
            chaosCandle = false;
            blueCandle = false;
            pinkCandle = false;
            yellowCandle = false;

            
            //Disable Lockon when not on gamepad to prevent it being left on forever by The Pointer
            if (!Main.gamePad)
                LockOnHelper.ForceUsability = false;

            SelectedFishingMinigame = FishingMinigames.None;

            #region Minion Reset Effects
            wDroid = false;
            resButterfly = false;
            hasVoidEaterMarionette = false;
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
            sunSpirit = false;
            dCreeper = false;
            eAxe = false;
            endoCooper = false;
            apexShark = false;
            gastricBelcher = false;
            hauntedDishes = false;
            stormjaw = false;
            vengefulSunMinion = false;
            sirius = false;
            aChicken = false;
            cLamp = false;
            pGuy = false;
            cEnergy = false;
            pSoulGuardians = false;
            sandEleBuff = false;
            oasisEleBuff = false;
            cloudEleBuff = false;
            brimEleBuff = false;
            waterEleBuff = false;
            fClump = false;
            aValkyrie = false;
            sCrystal = false;
            sGod = false;
            sandnado = false;
            PlantationSummon = false;
            astralProbe = false;
            victideSnail = false;
            cSpirit = false;
            dCrystal = false;
            MutatedTruffleBool = false;
            sandElemental = false;
            sandElementalVanity = false;
            oasisElemental = false;
            oasisElementalVanity = false;
            cloudElemental = false;
            cloudElementalVanity = false;
            brimElemental = false;
            brimElementalVanity = false;
            waterElemental = false;
            waterElementalVanity = false;
            allElementals = false;
            allElementalsVanity = false;
            fungalClump = false;
            fungalClumpVanity = false;
            howlsHeart = false;
            howlsHeartVanity = false;
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
            seashineSwordBuff = false;
            plaguebringerMK2 = false;
            igneousExaltation = false;
            GlacialEmbrace = false;
            VoidConcentrationStaff = false;
            saros = false;
            if (sarosEclipseBeamUsage > 300)
                sarosEclipseBeamUsage = 300;
            if (sarosEclipseBeamUsage > 0 && Player.ownedProjectileCounts[ModContent.ProjectileType<SarosEclipseBeam>()] <= 0)
                sarosEclipseBeamUsage--;
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
            snakeEyes = false;
            poleWarper = false;
            aqueousHunterDrone = false;
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
            MoonFist = false;
            AresCannons = false;
            celestialDragons = false;
            KalandraMirror = false;
            StellarTorus = false;
            LiliesOfFinalityBool = false;
            EnchantedKnifeStaffBool = false;
            AmphibiansGuitarBool = false;
            #endregion

            //On GFB both are enabled to cause the game to pick between multiplicative and crit at random
            forceSummonTagMultiplicative = (Main.zenithWorld ? true : false);
            forceSummonTagCrit = (Main.zenithWorld ? true : false);
            bonusFlatTag = 0;
            bonusCritTag = 0;
            bonusMultTag = 0;

            /* Spawn blockers from back when they used to work by being favorited and not a toggleable item
            noStupidNaturalARSpawns = false
            disableVoodooSpawns = false;
            disablePerfCystSpawns = false;
            disableHiveCystSpawns = false;
            disableNaturalScourgeSpawns = false;
            disableAnahitaSpawns = false;
            */

            abyssalDivingSuitPrevious = abyssalDivingSuit;
            abyssalDivingSuit = false;

            aquaticHeartPrevious = aquaticHeart;
            aquaticHeart = false;

            profanedCrystalStatePrevious = pscState;
            profanedCrystalPrevious = profanedCrystal;
            profanedCrystal = profanedCrystalBuffs = false;
            pscState = 0;
            pscLerpColor = Color.White;

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

            infiniteFlight = false;

            noStupidNaturalARSpawns = false;
            disableAnahitaSpawns = false;
            disableHiveCystSpawns = false;
            disableNaturalScourgeSpawns = false;
            disablePerfCystSpawns = false;
            disableVoodooSpawns = false;


            #region Abyss
            abyssDarkness = 0;
            abyssPlayerGlowMultiplier = 1;
            abyssFlashlightWidthMultiplier = 1;
            darknessIntensity = MathHelper.Max(darknessIntensity-0.05f,0);
            #endregion

                EnchantHeldItemEffects(Player, Player.Calamity(), Player.HeldItem);
        }
        #endregion

        #region Modify Max Health and Mana
        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            health = StatModifier.Default;
            health.Base = sTangerine.ToInt() * SanguineTangerine.LifeBoost
                        + mFruit.ToInt() * MiracleFruit.LifeBoost
                        + tCloudberry.ToInt() * TaintedCloudberry.LifeBoost
                        + sStrawberry.ToInt() * SacredStrawberry.LifeBoost;

            mana = StatModifier.Default;
            mana.Base = cShard.ToInt() * CometShard.ManaBoost
                        + eCore.ToInt() * EtherealCore.ManaBoost
                        + pHeart.ToInt() * PhantomHeart.ManaBoost;
        }
        #endregion

        #region Screen Position Movements
        public override void ModifyScreenPosition()
        {
            // CIT 08FEB2025: Photosensitivity config also disables screenshake
            bool allowScreenshake = CalamityClientConfig.Instance.ScreenshakePower > 0 && !CalamityClientConfig.Instance.Photosensitivity;

            if (GeneralScreenShakePower > 0f && allowScreenshake)
                Main.screenPosition += Main.rand.NextVector2Circular(GeneralScreenShakePower * CalamityClientConfig.Instance.ScreenshakePower, GeneralScreenShakePower * CalamityClientConfig.Instance.ScreenshakePower);

            GeneralScreenShakePower = MathHelper.Clamp(GeneralScreenShakePower - 0.185f, 0f, 20f * CalamityClientConfig.Instance.ScreenshakePower);
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

                    SyncCooldownDictionary(Main.dedServ);
                }
            }

            calamityBonusLuck = 0f;

            #region Defense Damage
            totalDefenseDamage = 0;
            defenseDamageRecoveryFrames = 0;
            totalDefenseDamageRecoveryFrames = DefenseDamageBaseRecoveryTime;
            defenseDamageDelayFrames = 0;
            nextHitDealsDefenseDamage = false;
            bloodflareCoreRemainingHealOverTime = 0;
            #endregion

            #region Buffs, Debuffs, Counters, and Nonsense
            if (Player.HeldItem.IsAir || Player.HeldItem.fishingPole == 0)
                consecutiveCaughtFish = 0;
            heldGaelsLastFrame = false;
            heldElephantKillerLastFrame = false;
            partialLifeRegenCounter = 0f;
            gaelSwipes = 0;
            whitewaterHeal = 0;
            luxorHit = false;
            arsenalCooldown = 0;
            andromedaState = AndromedaPlayerState.Inactive;
            planarSpeedBoost = 0;
            galileoCooldown = 0;
            soundCooldown = 0;
            dogTextCooldown = 0;
            auralisStealthCounter = 0f;
            auralisAuroraCounter = 0;
            auralisAuroraCooldown = 0;
            auralisAurora = 0;
            necroReviveCounter = -1;
            hideOfDeusTimer = 0;
            bloomStoneHealPool = 0;
            bloomStoneTotalHeal = 0;
            bloomStoneHealTimer = 0;
            bloomStoneHealRate = 0;
            murasamaHitCooldown = 0;
            SulphWaterPoisoningLevel = 0f;
            holyInfernoFadeIntensity = 0f;
            spiritOriginCritBoost = 0;
            critDamage = 0f;
            coinDropMult = 1f;
            rage = 0f;
            adrenaline = 0f;
            raiderCritLifespan = 0;
            raiderSoundCooldown = 0;
            gSabatonHotkeyFallWindup = -1;
            gSabatonFall = 0;
            gSabatonFalling = false;
            gSabatonTempJumpSpeed = 0;
            rOfDelivarenceRam = false;
            astralStarRainCooldown = 0;
            VoidCooldown = 0;
            AlchFlaskCooldown = 0;
            ascendantInsigniaCooldown = 0;
            transformerCooldown = 0;
            transformerDelay = 0;
            transformerStoredKills = 0;
            silvaMageCooldown = 0;
            bloodflareMageCooldown = 0;
            tarraRangedCooldown = 0;
            hideOfDeusMeleeBoostTimer = 0;
            rOfResilienceCooldown = 0;
            rOfResilienceEffect = 0;
            demonSwordKillMode = false;

            externalBreathTickBoost = 0f;
            externalFlightTimeMultBoost = 0f;
            externalColdImmunity = externalHeatImmunity = false;
            externalDefenseDamageImmunity = false;

            dragonRageHits = 0;
            dragonRageCooldown = 0;
            spectralVeilImmunity = 0;
            jetPackDash = 0;
            jetPackDirection = 0;
            andromedaCripple = 0;
            theBeeCooldown = 0;
            scuttlerCooldown = 0;
            mageCrownTimer = 0;
            wingProjectileCooldown = 0;
            hallowedRuneCooldown = 0;
            sulphurBubbleCooldown = 0;
            ladHearts = 0;
            prismaticLasers = 0;
            angelicActivate = -1;
            resetHeightandWidth = false;
            noLifeRegen = false;
            alcoholPoisoning = false;
            shadowflame = false;
            daybroken = false;
            whisperingDeath = false;
            dragonFire = false;
            vermillionFlux = false;
            auricRebuke = false;
            staticDischarge = false;
            miracleBlight = false;
            armorCrunch = false;
            irradiated = false;
            bane = false;
            brimstoneFlames = false;
            witheredDebuff = false;
            absorberAffliction = false;
            weakBrimstoneFlames = false;
            demonicFlames = false;
            godSlayerInferno = false;
            astralInfection = false;
            plague = false;
            holyFlames = false;
            holyInferno = false;
            burningBlood = false;
            brainRot = false;
            heavybleeding = false;
            laceration = false;
            elementalMix = false;
            icarusFolly = false;
            vHex = false;
            trueVHex = false;
            DoGExtremeGravity = false;
            warped = false;
            crushDepth = false;
            riptide = false;
            hadopelagicPressure = false;
            fishAlert = false;
            malnourished = false;
            clamity = false;
            NOU = false;
            snowmanNoseless = false;
            sulphurPoison = false;
            nightwither = false;
            voidfrost = false;
            eutrophication = false;
            frozenLungs = false;
            searingLava = false;
            vaporfied = false;
            banishingFire = false;
            wither = false;
            windChilled = false;
            ImmobilityDebuffImmunityTimer = 0;
            TypelessDebuffMultiplier = new();
            HeatDebuffMultiplier = new();
            ColdDebuffMultiplier = new();
            SicknessDebuffMultiplier = new();
            WaterDebuffMultiplier = new();
            ElectricDebuffMultiplier = new();
            #endregion

            #region Rogue
            // Stealth
            rogueStealth = 0f;
            rogueStealthMax = 0f;
            stealthAcceleration = 1f;

            stealthDamage = 0f;
            bonusStealthDamage = 0;
            rogueVelocity = 1f;
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
            crysthamyr = false;
            ExoChair = false;
            aquaticHeartWaterBuff = false;
            aquaticHeartIce = false;
            sandsWindBuff = false;
            aeolianEarthBuff = false;
            chiBuffTimer = 0;
            corrEffigy = false;
            crimEffigy = false;
            rRage = false;
            xWrath = false;
            graxDefense = false;
            encased = false;
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
            galvanicCorrosion = false;
            sulphurskin = false;
            baguette = false;
            flaskBrimstone = false;
            purpleHaze = false;
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
            baconOil = false;
            tranquilityCandle = false;
            chaosCandle = false;
            blueCandle = false;
            pinkCandle = false;
            pinkCandleHealFraction = 0D;
            yellowCandle = false;
            trippy = false;
            amidiasBlessing = false;
            bloodfinBoost = false;
            bloodfinTimer = 0;
            healCounter = 300;
            danceOfLightCharge = 0;
            ammoCost = 1f;
            partialLifeRegen = 0f;
            healingPotionMultiplier = 1f;
            avertorBonus = false;
            divineBless = false;
            hasteLevel = 0;
            hasteCounter = 0;
            #endregion

            #region Armor Set Bonuses
            silverMedkit = false;
            silverMedkitTimer = 0;
            tungstenArmorHookBoost = false;
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
            auricSetMelee = false;
            silvaSet = false;
            silvaMage = false;
            silvaSummon = false;
            hasSilvaEffect = false;
            silvaCountdown = SilvaArmor.ReviveDuration;
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
            molluskHelmet = false;
            molluskChest = false;
            molluskLegs = false;
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
            reaverDefense = false;
            reaverExplore = false;
            shadeRegen = false;
            dsSetBonus = false;
            titanHeartSet = false;
            titanHeartMask = false;
            titanHeartMantle = false;
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
            ataxiaVolley = false;
            ataxiaBlaze = false;
            hydrothermalSmoke = false;
            desertProwler = false;
            snowRuffianSet = false;
            forbiddenCirclet = false;
            forbiddenCooldown = 0;
            tornadoCooldown = 0;
            eskimoSet = false;
            rainSet = false;
            meteorSet = false;
            necroSet = false;
            frostSet = false;
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
            CurrentlyViewedCanvasID = -1;
            CurrentlyViewedHologramText = string.Empty;
            #endregion

            burningSeaBurnOut = 0;
            flareGunOverheat = 0;
            hellbornShots = 0;
            garandShots = 0;
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
                if (Player.whoAmI == 0 && !CalamityGlobalNPC.AnyLivingPlayers() && CalamityUtils.CountProjectiles(ProjectileType<BossRushFailureEffectThing>()) == 0)
                    Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ProjectileType<BossRushFailureEffectThing>(), 0, 0f);
            }

            // Respawn the player faster
            // 3 seconds normally and configurable while a boss is alive between 15 and 60 seconds
            int respawnTimerSet = areThereAnyDamnBosses ? (CalamityServerConfig.Instance.PlayerRespawnTime_BossAlive * 60) : 180;
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
                yield return createItem(ItemType<StarterBag>());
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
            return ContentSamples.ItemsByType[itemID];
        }
        public Item FindAccessory<T>() where T : ModItem => FindAccessory(ItemType<T>());

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            // Why does this otherwise work when you're dead lmao
            if (Player.dead)
                return;

            if (ascendantInsignia && Main.myPlayer == Player.whoAmI && CalamityKeybinds.AscendantInsigniaHotKey.JustPressed && ascendantInsigniaCooldown <= 0)
            {
                var source = Player.GetSource_Accessory(FindAccessory<AscendantInsignia>());
                Projectile.NewProjectile(source, Player.Center - Vector2.UnitY * 45f, Vector2.Zero, ProjectileType<AscendantAura>(), 0, 0f);
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/AscendantActivate"));
                ascendantInsigniaCooldown = AscendantInsignia.AbilityCooldown;
                ascendantInsigniaBuffTime = AscendantInsignia.AbilityDuration;
            }

            int numOfBlobs = Player.ownedProjectileCounts[ProjectileType<TransformerBlob>()];
            if (transformer && numOfBlobs > 0 && Main.myPlayer == Player.whoAmI && CalamityKeybinds.TransformerHotKey.JustPressed && transformerCooldown <= 0 && true) // Add check if projectiles are active
            {
                // Go fire all the blobs
                int cooldownTime = 300;
                transformerCooldown = cooldownTime;
                Player.AddCooldown(TransformerCooldown.ID, cooldownTime);

                for (int x = 0; x < Main.maxProjectiles; x++)
                {
                    Projectile projectile = Main.projectile[x];
                    if (projectile.active && projectile.type == ProjectileType<TransformerBlob>())
                    {
                        projectile.localAI[0] = 5;
                    }
                }
                if (transformerVisual)
                {
                    SoundStyle activate = new("CalamityMod/Sounds/Item/NullShot");
                    for (int i = 0; i < 3; i++)
                        SoundEngine.PlaySound(activate with { Volume = 0.3f, Pitch = 0.2f + i * 0.3f, MaxInstances = -1 }, Player.Center);
                    Particle orb2 = new CustomPulse(Player.Center, Vector2.Zero, Color.DodgerBlue, "CalamityMod/Particles/BloomRingThinLarge", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0, 0.2f, 20);
                    GeneralParticleHandler.SpawnParticle(orb2);
                }

            }

            //Only increment the slam if not on ground, not mounted, not on rope, not hooked, not tongued, otherwise reset slam time to zero
            if (CalamityKeybinds.GravistarSabatonHotkey.JustPressed && gSabatonHotkeyFallWindup < 0)
                gSabatonHotkeyFallWindup = 0;
            if (gSabaton && gSabatonHotkeyFallWindup >= 0 && Main.myPlayer == Player.whoAmI && (Player.velocity.Y != 0) && !Player.pulley && !Player.mount.Active && Player.grappling[0] == -1 && !Player.tongued)
            {
                gSabatonHotkeyFallWindup++;
                if (gSabatonHotkeyFallWindup < 20 && gSabatonHotkeyFallWindup % 2f == 0)
                {
                    SpawnGravistarParticle();
                }
            }
            else if (Main.myPlayer == Player.whoAmI)
            {
                gSabatonHotkeyFallWindup = -1;
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
                            SoundEngine.PlaySound(NormalityRelocator.TeleportSound, Player.Center);

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
                Player.AddBuff(BuffType<Buffs.StatBuffs.DivineBless>(), AngelicAlliance.DivineBlessDuration, false);
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

                var source = Player.GetSource_Accessory(FindAccessory<AngelicAlliance>());
                for (int projIndex = 0; projIndex < angelAmt; projIndex++)
                {
                    Projectile proj = Main.projectile[projIndex];
                    float start = 360f / angelAmt;

                    Projectile.NewProjectile(source, new Vector2((int)(Player.Center.X + (Math.Sin(projIndex * start) * 300)), (int)(Player.Center.Y + (Math.Cos(projIndex * start) * 300))), Vector2.Zero, ProjectileType<AngelicAllianceArchangel>(), proj.damage / 10, proj.knockBack / 10f, Player.whoAmI, Main.rand.Next(180), projIndex * start);
                    Player.HealPlayer(AngelicAlliance.HealPerAngelSpawned);
                }
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

                            int duration = areThereAnyDamnBosses ? chaosStateDuration : 360;
                            Player.AddBuff(BuffID.ChaosState, duration, true);
                            Player.AddCooldown(ChaosState.ID, duration, true, "spectralveil");

                            int numDust = 40;
                            Vector2 step = teleportOffset / numDust;
                            for (int i = 0; i < numDust; i++)
                            {
                                Dust dust = Dust.NewDustDirect(Player.Center - (step * i), 1, 1, DustID.VilePowder, step.X, step.Y);
                                dust.noGravity = true;
                                dust.noLight = true;
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

            if (CalamityKeybinds.AmmoCycleHotkey.JustPressed && deadshotBrooch)
            {
                SoundEngine.PlaySound(SoundID.Item149, Player.Center);

                // I could've made this so simple on myself, but no, I felt the need to make things convenient for the users...
                // First we need to determine the bounds for what we're swapping.
                int ammoType = Player.HeldItem.useAmmo;
                int lastSlot, firstSlot;
                for (lastSlot = 57; lastSlot >= 54; lastSlot--)
                {
                    if (!Player.inventory[lastSlot].IsAir && Player.inventory[lastSlot].ammo == ammoType)
                        break;
                }
                for (firstSlot = 54; firstSlot <= 57; firstSlot++)
                {
                    if (!Player.inventory[firstSlot].IsAir && Player.inventory[firstSlot].ammo == ammoType)
                        break;
                }
                // If firstSlot = lastSlot, then there is only one ammo in ammo slots, so don't do anything.
                if (firstSlot != lastSlot)
                {
                    int tempType = Player.inventory[lastSlot].type;
                    int tempStack = Player.inventory[lastSlot].stack;
                    // This list tracks the favorited status of the ammo slots. Since swapping the slots around fucks with the favorite status it must be manually set.
                    List<bool> favorited = [];
                    for (int z = 54; z <= 57; z++)
                        favorited.Add(!Player.inventory[z].IsAir && Player.inventory[z].favorited);

                    // It is impossible for lastSlot to equal 54 and firstSlot to not equal 54.
                    for (int i = lastSlot; i >= 55; i--)
                    {
                        if (Player.inventory[i].IsAir || Player.inventory[i].ammo != ammoType)
                            continue;

                        // If the slot we're interested in is the lower bound of our ammos, set it to the temp values and don't run anything else.
                        if (i == firstSlot)
                        {
                            Player.inventory[i].SetDefaults(tempType);
                            Player.inventory[i].stack = tempStack;
                            Player.inventory[i].favorited = favorited[lastSlot - 54];
                            continue;
                        }

                        // Find the next slot that has ammo in it.
                        int nextSlot;
                        for (nextSlot = i - 1; nextSlot >= 54; nextSlot--)
                        {
                            if (!Player.inventory[nextSlot].IsAir && Player.inventory[nextSlot].ammo == ammoType)
                                break;
                        }

                        // Set this slot to what is in that slot.
                        Player.inventory[i].SetDefaults(Player.inventory[nextSlot].type);
                        Player.inventory[i].stack = Player.inventory[nextSlot].stack;
                        Player.inventory[i].favorited = favorited[nextSlot - 54];
                    }

                    // Handles swapping the ammo that is in the first slot.
                    if (firstSlot == 54)
                    {
                        Player.inventory[54].SetDefaults(tempType);
                        Player.inventory[54].stack = tempStack;
                        Player.inventory[54].favorited = favorited[lastSlot - 54];
                    }

                    // Produce a visual effect showing the top ammo that you swapped to.
                    int visualType = Player.inventory[firstSlot].type;
                    Texture2D ammoTex = TextureAssets.Item[visualType].Value;
                    int frameAmt = Main.itemAnimations[visualType] == null ? 1 : ammoTex.Height / Main.itemAnimations[visualType].GetFrame(ammoTex).Height;
                    CustomSprite ammoVisual = new(Player.Center - Vector2.UnitY * 20f, -Vector2.UnitY * 7f, 30, ammoTex, 1f, Color.White, 0f, false, false, frameAmt);
                    GeneralParticleHandler.SpawnParticle(ammoVisual);
                }
            }

            if (CalamityKeybinds.ArmorSetBonusHotKey.JustPressed)
            {
                PlayerLoader.ArmorSetBonusActivated(Player);

                // Activate vanilla set bonuses
                if (Player.setVortex && !Player.mount.Active)
                    Player.vortexStealthActive = !Player.vortexStealthActive;

                if (Player.setForbidden)
                {
                    Player.MinionRestTargetAim();

                    if (!Player.setForbiddenCooldownLocked)
                        Player.CommandForbiddenStorm();
                }
            }

            if (CalamityKeybinds.ArmorSetBonusHotKey.Current)
            {
                ArmorSetBonusKeyHeldTimer++;
                PlayerLoader.ArmorSetBonusHeld(Player, ArmorSetBonusKeyHeldTimer);
            }
            else
                ArmorSetBonusKeyHeldTimer = 0;

            if (CalamityKeybinds.AccessoryParryHotKey.JustPressed)
            {
                if (blazingCore && blazingCoreParry == 0 && blazingCoreSuccessfulParry == 0)
                {
                    //minor cheese prevention with standing on a spike with later game gear spamming parry :skull:
                    //because of ordering, if they do not have the cooldown, it will not check the projectile array. Likewise if there are no bosses alive.
                    //Furthermore, Enumerable#Any is lightweight and returns immediately if a single object matches it's predicate
                    if (!Player.HasCooldown(ParryCooldown.ID) || Player.ownedProjectileCounts[ProjectileType<BlazingStarHeal>()] == 0)
                    {
                        Player.SetScreenshake(3.5f);
                        blazingCoreParry = 30;
                        SoundEngine.PlaySound(BlazingCore.ParryActivateSound, Player.Center);
                        var mySourceIsIMadeItUp = Player.GetSource_FromThis();
                        int blazingSun = Projectile.NewProjectile(mySourceIsIMadeItUp, Player.Center, Vector2.Zero, ProjectileType<BlazingSun>(), 0, 0f, Player.whoAmI, 0f, 0f);
                        Main.projectile[blazingSun].Center = Player.Center;
                        int blazingSun2 = Projectile.NewProjectile(mySourceIsIMadeItUp, Player.Center, Vector2.Zero, ProjectileType<BlazingSun2>(), 0, 0f, Player.whoAmI, 0f, 0f);
                        Main.projectile[blazingSun2].Center = Player.Center;
                    }
                }
                else if (flameLickedShell && flameLickedShellParry == 0)
                {
                    if (!Player.HasCooldown(ParryCooldown.ID) || Player.ownedProjectileCounts[ProjectileType<FlameLickedBarrage>()] == 0)
                    {
                        Player.SetScreenshake(2.5f);
                        SoundEngine.PlaySound(ProfanedGuardianDefender.RockShieldSpawnSound, Player.Center);
                        flameLickedShellParry = FlameLickedShell.flameLickedParry;
                    }
                }
            }

            // Trigger for pressing the God Slayer dash key
            if (CalamityKeybinds.GodSlayerDashHotKey.JustPressed)
            {
                if (godSlayer && !Player.pulley && Player.grappling[0] == -1 && !Player.tongued && !Player.mount.Active && !Player.HasCooldown(GodSlayerDash.ID) && Player.dashDelay == 0)
                {
                    godSlayerDashHotKeyPressed = true;
                    foreach (Projectile p in Main.ActiveProjectiles)
                    {
                        if (p.type == ProjectileType<RelicOfDeliveranceSpear>() && Player.whoAmI == p.owner)
                        {
                            (p.ModProjectile as RelicOfDeliveranceSpear).KillProj();
                            return;
                        }
                    }
                }
            }

            //Right click dash on Speed Blaster
            if (sBlasterDashActivated == true)
            {
                if ((Player.controlUp || Player.controlDown || Player.controlLeft || Player.controlRight) && !Player.pulley && Player.grappling[0] == -1 && !Player.tongued && !Player.mount.Active && (Player.HasCooldown(SpeedBlasterBoost.ID) || Player.HasCooldown(SuperradiantSawBoost.ID)) && Player.dashDelay == 0)
                {
                    SpeedBlasterDashStarted = true;
                }
                sBlasterDashActivated = false;
            }

            if (Player.Calamity().SpeedBlasterDashStarted || (Player.dashDelay != 0 && (Player.Calamity().LastUsedDashID == SuperradiantSawDash.ID || Player.Calamity().LastUsedDashID == SpeedBlasterDash.ID)))
            {
                Player.Calamity().DeferredDashID = Player.HeldItem.type == ItemType<SuperradiantSlaughterer>() ? SuperradiantSawDash.ID : SpeedBlasterDash.ID;
                Player.dash = 0;
            }

            // Trigger for pressing the Rage hotkey.
            if (CalamityKeybinds.RageHotKey.JustPressed)
            {
                // Gael's Greatsword replaces Rage Mode with an uber skull attack
                if (!(Player.HasCooldown(Cooldowns.GaelsRage.ID)) && Player.HeldItem.type == ItemType<GaelsGreatsword>() && rage > 0f)
                {
                    SoundEngine.PlaySound(SilvaArmor.DispelSound, Player.Center);

                    for (int i = 0; i < 3; i++)
                        Dust.NewDust(Player.position, 120, 120, DustID.Rain_BloodMoon, 0f, 0f, 100, default, 1.5f);
                    for (int i = 0; i < 30; i++)
                    {
                        float angle = MathHelper.TwoPi * i / 30f;
                        Dust dust = Dust.NewDustDirect(Player.position, 120, 120, DustID.Rain_BloodMoon, 0f, 0f, 0, default, 2f);
                        dust.noGravity = true;
                        dust.velocity *= 4f;
                        dust = Dust.NewDustDirect(Player.position, 120, 120, DustID.Rain_BloodMoon, 0f, 0f, 100, default, 1f);
                        dust.velocity *= 2.25f;
                        dust.noGravity = true;
                        Dust.NewDust(Player.Center + angle.ToRotationVector2() * 160f, 0, 0, DustID.Rain_BloodMoon, 0f, 0f, 100, default, 1f);
                    }

                    // https://github.com/tModLoader/tModLoader/wiki/IEntitySource#detailed-list
                    var source = Player.GetSource_ItemUse(Player.HeldItem, GaelsGreatsword.SkullsplosionEntitySourceContext);
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
                        int projectileIndex = Projectile.NewProjectile(source, Player.Center + initialVelocity * 3f, initialVelocity, ProjectileType<GaelSkull2>(), damage, 2f, Player.whoAmI);
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
                    Player.AddBuff(BuffType<RageMode>(), 2);

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
                    Player.AddBuff(BuffType<AdrenalineMode>(), AdrenalineDuration);

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
                        int dustID = electricity ? (Main.rand.NextBool() ? 132 : 131) : DustType<AdrenDust>();

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
                        d = Dust.BetterCloneDust(d);
                        d.position = segmentTwoPos;
                        d.scale = Main.rand.NextFloat(1.2f, 1.8f);
                        d.velocity = Main.rand.NextVector2Unit() * spreadSpeed;

                        Vector2 segmentThreePos = Player.Center + segmentThreeStart + segmentThreeIncrement * interpolant;
                        d = Dust.BetterCloneDust(d);
                        d.position = segmentThreePos;
                        d.scale = Main.rand.NextFloat(1.2f, 1.8f);
                        d.velocity = Main.rand.NextVector2Unit() * spreadSpeed;
                    }
                }
            }
        }

        public override void ArmorSetBonusActivated()
        {
            if (brimflameSet && !Player.HasCooldown(BrimflameFrenzy.ID))
            {
                if (Player.whoAmI == Main.myPlayer)
                {
                    if (brimflameFrenzy)
                    {
                        Player.ClearBuff(BuffType<BrimflameFrenzyBuff>());
                        Player.AddCooldown(BrimflameFrenzy.ID, BrimflameCowl.FrenzyCooldown);
                    }
                    else
                    {
                        Player.AddBuff(BuffType<BrimflameFrenzyBuff>(), BrimflameCowl.FrenzyDuration, true);
                        SoundEngine.PlaySound(BrimflameCowl.ActivationSound, Player.Center);
                        for (int i = 0; i < 36; i++)
                        {
                            Dust brimDust = Dust.NewDustDirect(new Vector2(Player.position.X, Player.position.Y + 16f), Player.width, Player.height - 16, (int)CalamityDusts.Brimstone, 0f, 0f, 0, default, 1f);
                            brimDust.velocity *= 3f;
                            brimDust.scale *= 1.15f;
                        }
                        int dustAmt = 36;
                        for (int j = 0; j < dustAmt; j++)
                        {
                            Vector2 dustRotation = Vector2.Normalize(Player.velocity) * new Vector2((float)Player.width / 2f, (float)Player.height) * 0.75f;
                            dustRotation = dustRotation.RotatedBy((double)((float)(j - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Player.Center;
                            Vector2 dustVelocity = dustRotation - Player.Center;
                            Dust brimDust2 = Dust.NewDustDirect(dustRotation + dustVelocity, 0, 0, (int)CalamityDusts.Brimstone, dustVelocity.X * 1.5f, dustVelocity.Y * 1.5f, 100, default, 1.4f);
                            brimDust2.noGravity = true;
                            brimDust2.noLight = true;
                            brimDust2.velocity = dustVelocity;
                        }
                    }
                }
            }
            if (tarraMelee && !Player.HasCooldown(Cooldowns.TarragonCloak.ID) && !tarragonCloak)
            {
                if (Player.whoAmI == Main.myPlayer)
                {
                    Player.AddBuff(BuffType<Buffs.StatBuffs.TarragonCloak>(), TarragonHeadMelee.CloakDuration, false);
                }
            }
            if (bloodflareRanged && !Player.HasCooldown(BloodflareRangedSet.ID))
            {
                if (Player.whoAmI == Main.myPlayer)
                    Player.AddCooldown(BloodflareRangedSet.ID, BloodflareHeadRanged.SoulCooldown);

                SoundEngine.PlaySound(BloodflareHeadRanged.ActivationSound, Player.Center);
                for (int d = 0; d < 64; d++)
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(Player.position.X, Player.position.Y + 16f), Player.width, Player.height - 16, (int)CalamityDusts.Necroplasm, 0f, 0f, 0, default, 1f);
                    dust.velocity *= 3f;
                    dust.scale *= 1.15f;
                }
                int dustAmt = 36;
                for (int d = 0; d < dustAmt; d++)
                {
                    Vector2 source = Vector2.Normalize(Player.velocity) * new Vector2((float)Player.width / 2f, (float)Player.height) * 0.75f;
                    source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Player.Center;
                    Vector2 dustVel = source - Player.Center;
                    Dust phanto = Dust.NewDustDirect(source + dustVel, 0, 0, (int)CalamityDusts.Necroplasm, dustVel.X * 1.5f, dustVel.Y * 1.5f, 100, default, 1.4f);
                    phanto.noGravity = true;
                    phanto.noLight = true;
                    phanto.velocity = dustVel;
                }

                if (Player.whoAmI == Main.myPlayer)
                {
                    var source = Player.GetSource_Misc("1");
                    int damage = (int)(Player.GetTotalDamage<RangedDamageClass>().ApplyTo(BloodflareHeadRanged.SoulDamage));
                    for (int i = 0; i < BloodflareHeadRanged.SoulAmount; i++)
                    {
                        float ai1 = Main.rand.NextFloat() + 0.5f;
                        Vector2 circleVel = (MathHelper.TwoPi * i / 16f).ToRotationVector2() * Main.rand.NextFloat(5f, 8f);
                        int soul = Projectile.NewProjectile(source, Player.Center, circleVel, ProjectileType<BloodflareSoul>(), damage, 0f, Player.whoAmI, 0f, ai1);
                        if (soul.WithinBounds(Main.maxProjectiles))
                            Main.projectile[soul].DamageType = DamageClass.Generic;
                    }
                }
            }
            if (omegaBlueSet && !Player.HasCooldown(OmegaBlue.ID))
            {
                if (Player.whoAmI == Main.myPlayer)
                {
                    Player.AddBuff(BuffType<AbyssalMadness>(), OmegaBlueHelmet.MadnessDuration, false);
                }
                Player.AddCooldown(OmegaBlue.ID, OmegaBlueHelmet.MadnessDuration + OmegaBlueHelmet.MadnessCooldown);
                SoundEngine.PlaySound(OmegaBlueHelmet.ActivationSound, Player.Center);
                for (int i = 0; i < 66; i++)
                {
                    Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.PurificationPowder, 0, 0, 100, Color.Transparent, 2.6f);
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.fadeIn = 1f;
                    dust.velocity *= 6.6f;
                }
            }
            if (dsSetBonus)
            {
                SoundEngine.PlaySound(DemonshadeHelm.ActivationSound, Player.Center);
                for (int i = 0; i < 36; i++)
                {
                    Dust brimDust = Dust.NewDustDirect(new Vector2(Player.position.X, Player.position.Y + 16f), Player.width, Player.height - 16, (int)CalamityDusts.Brimstone, 0f, 0f, 0, default, 1f);
                    brimDust.velocity *= 3f;
                    brimDust.scale *= 1.15f;
                }
                int dustAmt = 36;
                for (int j = 0; j < dustAmt; j++)
                {
                    Vector2 dustRotation = Vector2.Normalize(Player.velocity) * new Vector2((float)Player.width / 2f, (float)Player.height) * 0.75f;
                    dustRotation = dustRotation.RotatedBy((double)((float)(j - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Player.Center;
                    Vector2 dustVelocity = dustRotation - Player.Center;
                    Dust brimDust2 = Dust.NewDustDirect(dustRotation + dustVelocity, 0, 0, (int)CalamityDusts.Brimstone, dustVelocity.X * 1.5f, dustVelocity.Y * 1.5f, 100, default, 1.4f);
                    brimDust2.noGravity = true;
                    brimDust2.noLight = true;
                    brimDust2.velocity = dustVelocity;
                }
                if (Player.whoAmI == Main.myPlayer)
                {
                    Player.AddBuff(BuffType<Enraged>(), DemonshadeHelm.EnrageDuration, false);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (!npc.friendly && !npc.dontTakeDamage && Vector2.Distance(Player.Center, npc.Center) <= 3000f)
                            npc.AddBuff(BuffType<Enraged>(), DemonshadeHelm.EnrageDuration, false);
                    }
                }
            }
            if (plagueReaper && !Player.HasCooldown(PlagueBlackout.ID))
            {
                SoundEngine.PlaySound(PlagueReaperMask.ActivationSound, Player.Center);
                Player.AddCooldown(PlagueBlackout.ID, PlagueReaperMask.BlackoutDuration + PlagueReaperMask.BlackoutCooldown);
            }
            if (forbiddenCirclet && Player.ownedProjectileCounts[ProjectileType<CircletTornado>()] < 2)
            {
                forbiddenCooldown = ForbiddenCirclet.StormCooldown;
                int stormMana = (int)(ForbiddenCirclet.StormManaCost * Player.manaCost);
                if (Player.statMana < stormMana)
                {
                    if (Player.manaFlower)
                    {
                        Player.QuickMana();
                    }
                }
                if (Player.statMana >= stormMana && !Player.silence)
                {
                    var source = Player.GetSource_ItemUse(FindAccessory<ForbiddenCirclet>());
                    Player.manaRegenDelay = (int)Player.maxRegenDelay;
                    Player.statMana -= stormMana;

                    // To compute Forbidden Circlet tornado damage, create a fake stat modifier on the spot which combines both classes.
                    StatModifier forbidden = Player.GetTotalDamage<SummonDamageClass>().CombineWith(Player.GetDamage<RogueDamageClass>());
                    int damage = (int)forbidden.ApplyTo(ForbiddenCirclet.StormDamage);

                    float kBack = Player.GetTotalKnockback<SummonDamageClass>().ApplyTo(ForbiddenCirclet.StormKB);

                    if (Player.whoAmI == Main.myPlayer)
                    {
                        if (Player.ownedProjectileCounts[ProjectileType<CircletTornado>()] > 0)
                        {
                            foreach (var proj in Main.ActiveProjectiles)
                            {
                                if (proj.owner != Player.whoAmI || proj.type != ProjectileType<CircletTornado>())
                                    continue;
                                proj.ai[0] = CircletTornado.Lifetime - CircletTornado.Fadetime + proj.ai[0] % 60f;
                                proj.netUpdate = true;
                            }
                        }
                        Vector2 tornadoPos = Player.ClampedMouseWorld();
                        Projectile.NewProjectile(source, tornadoPos, Vector2.Zero, ProjectileType<CircletTornado>(), damage, kBack, Player.whoAmI);

                        Vector2 diff = tornadoPos - Player.Center;
                        float distance = diff.Length();
                        if (distance > 0f)
                        {
                            for (float i = 0f; i < distance; i += 15f)
                            {
                                Vector2 dustPos = Player.Center + diff * i / distance;
                                Dust trail = Dust.NewDustDirect(dustPos, 0, 0, DustID.Sandnado);
                                trail.position = dustPos;
                                trail.fadeIn = 0.5f;
                                trail.scale = 0.7f;
                                trail.velocity *= 0.4f;
                                trail.noLight = true;
                            }
                        }
                        for (int j = 0; j < 30; j++)
                        {
                            Dust cloud = Dust.NewDustDirect(tornadoPos, 0, 0, DustID.Sandnado);
                            cloud.position = tornadoPos;
                            cloud.fadeIn = 1f;
                            cloud.scale = 0.3f;
                            cloud.noLight = true;
                        }
                    }
                }
            }
            if (prismaticSet && !Player.HasCooldown(PrismaticLaser.ID) && prismaticLasers <= 0)
                prismaticLasers = PrismaticHelmet.LaserDuration + PrismaticHelmet.LaserCooldown;
            if (WulfrumHat.HasArmorSet(Player))
            {
                //Only activate if no cooldown & available scrap.
                if (cooldowns.TryGetValue(WulfrumBastion.ID, out CooldownInstance cd))
                {
                    // Quick dismount if activated again
                    if (cd.timeLeft > WulfrumHat.BastionCooldown && cd.timeLeft < WulfrumHat.BastionCooldown + WulfrumHat.BastionTime - 60 * 3)
                    {
                        cd.timeLeft = WulfrumHat.BastionCooldown + 1;
                        SyncCooldownDictionary(false);
                    }
                }

                else if (Player.HasItem(ItemType<WulfrumMetalScrap>()))
                {
                    Player.ConsumeItem(ItemType<WulfrumMetalScrap>());
                    //I Thiiiinnnk there's no need to add mp syncing packets since cooldowns get auto synced right
                    Player.AddCooldown(WulfrumBastion.ID, WulfrumHat.BastionCooldown + WulfrumHat.BastionTime);
                    //Though do I need to sync that or is the player inventory auto synced?
                    WulfrumHat.DummyCannon.SetDefaults(ItemType<WulfrumFusionCannon>());
                }
            }
            if (DesertProwlerHat.HasArmorSet(Player) && !Player.HasCooldown(SandsmokeBomb.ID))
                Player.AddCooldown(SandsmokeBomb.ID, DesertProwlerHat.SmokeCooldown + DesertProwlerHat.SmokeDuration);
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
                return (Vector2?)vector;

            return null;
        }

        /// <summary>
        /// Returns a random position in the Lihzhard Temple to teleport the player to. <br></br>
        /// Used primarily for <see cref="Items.Potions.TheElixir"/>.
        /// </summary>
        public static Vector2? GetTempleTeleportPosition(Player player)
        {
            int foundX = -1;
            int foundY = -1;
            bool templeOnLeftSide = Main.dungeonX > Main.spawnTileX;
            int xSearchEnd = templeOnLeftSide ? 20 : Main.maxTilesX - 20;

            if (templeOnLeftSide)
            {
                for (int i = Main.spawnTileX; i >= xSearchEnd; i -= 2)
                {
                    for (int j = (int)Main.worldSurface; j < Main.maxTilesY - 210; j++)
                    {
                        Tile tile = Framing.GetTileSafely(i, j);
                        if (tile.WallType == WallID.LihzahrdBrickUnsafe)
                        {
                            foundX = i;
                            foundY = j;
                            break;
                        }
                    }
                    if (foundX != -1)
                        break;
                }
            }
            else
            {
                for (int i = Main.spawnTileX; i <= xSearchEnd; i += 2)
                {
                    for (int j = (int)Main.worldSurface; j < Main.maxTilesY - 210; j++)
                    {
                        Tile tile = Framing.GetTileSafely(i, j);
                        if (tile.WallType == WallID.LihzahrdBrickUnsafe)
                        {
                            foundX = i;
                            foundY = j;
                            break;
                        }
                    }

                    if (foundX != -1)
                        break;
                }
            }

            if (foundX == -1)
                return null;

            int attempts = 25;
            int rangeX = 100;
            int rangeY = 100;
            int randomFluff = 400;
            Point finalTeleportPoint = Point.Zero;
            for (int i = 0; i < attempts; i++)
            {
                for (int x = 0; x <= rangeX; x++)
                {
                    for (int y = 0; y <= rangeY; y++)
                    {
                        int teleportPosX = foundX + (templeOnLeftSide ? x : -x) + Main.rand.Next(-randomFluff, randomFluff);
                        int teleportPosY = foundY + y + Main.rand.Next(-randomFluff, randomFluff);
                        Tile tile = Framing.GetTileSafely(teleportPosX, teleportPosY);
                        Tile tileAbove = Framing.GetTileSafely(teleportPosX, teleportPosY - 1);
                        Tile tileBelow = Framing.GetTileSafely(teleportPosX, teleportPosY + 1);

                        if (!tileAbove.HasTile && WorldGen.ActiveAndWalkableTile(tileBelow.X(), tileBelow.Y()) && tile.WallType == WallID.LihzahrdBrickUnsafe)
                        {
                            finalTeleportPoint = new(tile.X(), tile.Y());
                            if (!Collision.SolidCollision(finalTeleportPoint.ToWorldCoordinates(), player.width, player.height))
                                break;
                        }
                    }
                }
            }

            return finalTeleportPoint.ToWorldCoordinates() - new Vector2(player.width, player.height);
        }

        /// <summary>
        /// Returns a random position in the Void (Abyss Layer 4) to teleport the player to. <br></br>
        /// Used primarily for <see cref="Items.Potions.TheElixir"/>.
        /// </summary>
        public static Vector2? GetAbyssVoidTeleportPosition(Player player)
        {
            int halfWorldWidth = Main.maxTilesX / 2;
            int abyssStartX = Abyss.AtLeftSideOfWorld ? halfWorldWidth - (halfWorldWidth - 135) + 35 : halfWorldWidth + (halfWorldWidth - 135) - 35;
            int abyssEndX = Abyss.AtLeftSideOfWorld ? 0 : Main.maxTilesX;

            int foundX = -1;
            int foundY = -1;

            if (Abyss.AtLeftSideOfWorld)
            {
                for (int y = Main.maxTilesY; y >= (int)Main.worldSurface; y--)
                {
                    for (int x = abyssStartX; x >= abyssEndX; x -= 2)
                    {
                        Tile tile = Framing.GetTileSafely(x, y);
                        if (tile.TileType == ModContent.TileType<Voidstone>())
                        {
                            foundX = x;
                            foundY = y;
                            break;
                        }
                    }

                    if (foundY != -1)
                        break;
                }
            }
            else
            {
                for (int y = Main.maxTilesY; y >= (int)Main.worldSurface; y--)
                {
                    for (int x = abyssStartX; x <= abyssEndX; x += 2)
                    {
                        Tile tile = Framing.GetTileSafely(x, y);
                        if (tile.TileType == ModContent.TileType<Voidstone>())
                        {
                            foundX = x;
                            foundY = y;
                            break;
                        }
                    }

                    if (foundY != -1)
                        break;
                }
            }

            if (foundY == -1)
                return null;

            int attempts = 25;
            int rangeX = 100;
            int rangeY = 200;
            int randomFluff = 200;
            Point finalTeleportPoint = new(foundX, foundY);
            for (int i = 0; i < attempts; i++)
            {
                for (int x = 0; x <= rangeX; x++)
                {
                    for (int y = 0; y <= rangeY; y++)
                    {
                        int teleportPosX = foundX + (Abyss.AtLeftSideOfWorld ? x : -x) + Main.rand.Next(-randomFluff, randomFluff);
                        int teleportPosY = foundY - y + Main.rand.Next(-randomFluff, randomFluff);
                        Tile tile = Framing.GetTileSafely(teleportPosX, teleportPosY);
                        Tile tileAbove = Framing.GetTileSafely(teleportPosX, teleportPosY - 1);
                        Tile tileBelow = Framing.GetTileSafely(teleportPosX, teleportPosY + 1);

                        if (!tileAbove.HasTile && WorldGen.ActiveAndWalkableTile(tileBelow.X(), tileBelow.Y()) && tile.WallType == ModContent.WallType<UnsafeVoidstoneWall>())
                        {
                            finalTeleportPoint = new(tile.X(), tile.Y());
                            if (!Collision.SolidCollision(finalTeleportPoint.ToWorldCoordinates(), player.width, player.height))
                                break;
                        }
                    }
                }
            }

            return finalTeleportPoint.ToWorldCoordinates() - new Vector2(player.width, player.height);
        }

        /// <summary>
        /// Returns a random position in the Forsaken Archives to teleport the player to. <br></br>
        /// Used primarily for <see cref="Items.Potions.TheElixir"/>.
        /// </summary>
        public static Vector2? GetDungeonArchivesTeleportPosition(Player player)
        {
            // Define an area around the center to search for a valid spot to tp
            int teleportStartX = WorldgenManagementSystem.DungeonArchivePos.X;
            int teleportStartY = WorldgenManagementSystem.DungeonArchivePos.Y;
            bool dungeonOnRightSide = Main.dungeonX > Main.spawnTileX;

            // Manually search for the Archives from the bottom of the world if the Archive position isn't saved.
            if (WorldgenManagementSystem.DungeonArchivePos == Point.Zero)
            {
                int foundX = -1;
                int foundY = -1;
                int xSearchStart = dungeonOnRightSide ? Main.maxTilesX - 20 : 20;

                if (dungeonOnRightSide)
                {
                    for (int y = Main.maxTilesY; y >= (int)Main.worldSurface; y--)
                    {
                        for (int x = xSearchStart; x >= Main.spawnTileX; x -= 2)
                        {
                            Tile tile = Framing.GetTileSafely(x, y);
                            if (Main.wallDungeon[tile.WallType] && Main.tileDungeon[tile.TileType])
                            {
                                foundX = x;
                                foundY = y;
                                break;
                            }
                        }

                        if (foundY != -1)
                            break;
                    }
                }
                else
                {
                    for (int y = Main.maxTilesY; y >= (int)Main.worldSurface; y--)
                    {
                        for (int x = xSearchStart; x <= Main.spawnTileX; x += 2)
                        {
                            Tile tile = Framing.GetTileSafely(x, y);
                            if (Main.wallDungeon[tile.WallType] && Main.tileDungeon[tile.TileType])
                            {
                                foundX = x;
                                foundY = y;
                                break;
                            }
                        }

                        if (foundY != -1)
                            break;
                    }
                }

                if (foundY == -1)
                    return null;

                teleportStartX = foundX;
                teleportStartY = foundY;
            }


            int attempts = 25;
            int rangeX = 50;
            int rangeY = 50;
            int randomFluff = 20;
            Point finalTeleportPoint = Point.Zero;
            for (int i = 0; i < attempts; i++)
            {
                for (int x = 0; x <= rangeX; x++)
                {
                    for (int y = 0; y <= rangeY; y++)
                    {
                        int teleportPosX = teleportStartX + (dungeonOnRightSide ? -x : x) + Main.rand.Next(-randomFluff, randomFluff);
                        int teleportPosY = teleportStartY - y + Main.rand.Next(-randomFluff, randomFluff);
                        Tile tile = Framing.GetTileSafely(teleportPosX, teleportPosY);
                        Tile tileAbove = Framing.GetTileSafely(teleportPosX, teleportPosY - 1);
                        Tile tileBelow = Framing.GetTileSafely(teleportPosX, teleportPosY + 1);

                        if (!tileAbove.HasTile && WorldGen.ActiveAndWalkableTile(tileBelow.X(), tileBelow.Y()) && Main.tileDungeon[tile.TileType] && Main.wallDungeon[tile.WallType])
                        {
                            finalTeleportPoint = new(tile.X(), tile.Y());
                            if (!Collision.SolidCollision(finalTeleportPoint.ToWorldCoordinates(), player.width, player.height))
                                break;
                        }
                    }
                }
            }

            return finalTeleportPoint.ToWorldCoordinates() - new Vector2(player.width, player.height);
        }

        public static void ModTeleport(Player player, Vector2 pos, bool playSound = true, int style = TeleportationStyleID.RecallPotion)
        {
            bool postImmune = player.immune;
            int postImmuneTime = player.immuneTime;
            player.StopVanityActions(false);
            player.RemoveAllGrapplingHooks();
            player.Teleport(pos, style);
            if (Main.dedServ)
                RemoteClient.CheckSection(player.whoAmI, player.Center);
            NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, (float)player.whoAmI, pos.X, pos.Y, style, 0, 0);
            player.velocity = Vector2.Zero;
            player.immune = postImmune;
            player.immuneTime = postImmuneTime;

            // Make some dust
            for (int index = 0; index < 100; ++index)
            {
                Dust.NewDust(player.position, player.width, player.height, DustID.TeleportationPotion, player.velocity.X * 0.1f, player.velocity.Y * 0.1f, 150, Color.Cyan, 1.2f);
            }
            Rectangle rect = player.getRect();
            int dustAmt = rect.Width * rect.Height / 5;
            for (int k = 0; k < dustAmt; k++)
            {
                Dust dust = Dust.NewDustDirect(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, DustID.TeleportationPotion);
                dust.scale = Main.rand.NextFloat(0.2f, 0.7f);
                if (k < 10)
                    dust.scale += 0.25f;
                if (k < 5)
                    dust.scale += 0.25f;
            }
            for (int k = 0; k < 50; k++)
            {
                Dust dust = Dust.NewDustDirect(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, DustID.DungeonSpirit);
                dust.noGravity = true;
                for (int i = 0; i < 5; i++)
                {
                    if (Main.rand.NextBool(3))
                        dust.velocity *= 0.75f;
                }
                if (Main.rand.NextBool(3))
                {
                    dust.velocity *= 2f;
                    dust.scale *= 1.2f;
                }
                if (Main.rand.NextBool(3))
                {
                    dust.velocity *= 2f;
                    dust.scale *= 1.2f;
                }
                if (Main.rand.NextBool())
                {
                    dust.fadeIn = Main.rand.NextFloat(0.75f, 1f);
                    dust.scale = Main.rand.NextFloat(0.25f, 0.75f);
                }
                dust.scale *= 0.8f;
            }

            if (playSound)
                SoundEngine.PlaySound(SoundID.Item6, player.Center);
        }
        #endregion

        #region UpdateEquips
        public override void UpdateEquips()
        {
            // TODO -- why is boss health bar code in Player.UpdateEquips and not a ModSystem
            CalamityClientConfig.Instance.BossHealthBarExtraInfo = shouldDrawSmallText;

            // Putting this in GlobalItem will run multiple times for each slot, which this system already does, creating a slew of problems.
            VanillaArmorChangeManager.ApplyPotentialEffectsTo(Player);

            // Nerf to the proc rate of Spectre Mask's set bonus souls
            // Vanilla subtracts 6.6666665 from this counter per frame, this reduces it to 4
            if (Player.ghostDmg > 0)
                Player.ghostDmg += 2.6666665f;

            // If the config is enabled, vastly increase the player's base tile and wall placement speeds
            // This stacks with the Brick Layer and Portable Cement Mixer
            if (CalamityServerConfig.Instance.FasterTilePlacement)
            {
                Player.tileSpeed += 0.5f;
                Player.wallSpeed += 0.5f;
            }

            // Takes the movement speed bonus and uses it to increase run speed
            float accRunSpeedMin = Player.accRunSpeed * 0.5f;
            Player.accRunSpeed += Player.accRunSpeed * moveSpeedBonus * 0.16f;
            if (Player.accRunSpeed < accRunSpeedMin)
                Player.accRunSpeed = accRunSpeedMin;

            if (Player.blackBelt) 
                DodgeEffects.Add((Player, hit) => {
                    Player.NinjaDodge();
                    return null;
                    }); 
            if (Player.brainOfConfusionItem != null && !Player.brainOfConfusionItem.IsAir)
                DodgeEffects.Add((Player, hit) => {
                    Player.BrainOfConfusionDodge();
                    return null;
                    });

            if (Player.Transformation().Type == ItemType<Popo>())
            {
                if (Player.whoAmI == Main.myPlayer && !snowmanNoseless)
                    Player.AddBuff(BuffType<PopoBuff>(), 60, true);
            }

            if (abyssalDivingSuit)
                Player.AddBuff(BuffType<AbyssalDivingSuitBuff>(), 60, true);

            if (aquaticHeart)
                Player.AddBuff(BuffType<AquaticHeartBuff>(), 60, true);

            if (aquaticHeart && NPC.downedBoss3)
            {
                if (Player.whoAmI == Main.myPlayer && !Player.HasCooldown(AquaticHeartIceShield.ID))
                    Player.AddBuff(BuffType<IceShieldBuff>(), 2);
            }

            if (profanedCrystal)
                Player.AddBuff(BuffType<ProfanedCrystalBuff>(), 60, true);

            if (gSabaton)
            {
                if (Player.whoAmI == Main.myPlayer)
                {
                    // While preparing slam, bring Y velocity closer to 0
                    if (gSabatonHotkeyFallWindup < 20 && gSabatonHotkeyFallWindup != 0 && !gSabatonFalling)
                        Player.velocity.Y *= (60 - (gSabatonHotkeyFallWindup * 0.75f)) / 60f;

                    // Play sound a bit early so it goes in time with the fall
                    if (gSabatonHotkeyFallWindup == 5 && !gSabatonFalling)
                        SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/GravistarCharge") { Volume = 0.3f });

                    // 0.5 seconds passed, falling time
                    if (gSabatonHotkeyFallWindup == 20)
                    {
                        gSabatonFalling = true;
                        Player.velocity.Y = 0.01f;
                    }

                    // Cancel fall and don't give 'on ground' effects if on rope, on mount, grappled, or tongued
                    // Also cancel fall if the player has upwards Y velocity (Goodbye Inner Tube cheese)
                    if ((Player.gravDir == 1 && Player.velocity.Y < 0f) || (Player.gravDir == -1 && Player.velocity.Y > 1f) || Player.pulley || Player.mount.Active || Player.grappling[0] != -1 || Player.tongued)
                    {
                        gSabatonFall = 0;
                        gSabatonFalling = false;
                        gSabatonHotkeyFallWindup = -1;
                    }

                    if (gSabatonFalling)
                    {
                        SpawnGravistarParticle();

                        // Cap time converted to damage at 2 seconds
                        if (gSabatonFall < 120)
                            gSabatonFall++;

                        Player.maxFallSpeed = 40f;
                        Player.gravity = 1.3f;

                        // If the player can fly during the fall, the physics gets a bit funky
                        Player.controlJump = false;

                        // Check if player hit some form of solid resistance (the ground)
                        if (0 == Player.velocity.Y)
                        {
                            var source = Player.GetSource_Accessory(FindAccessory<InterstellarStompers>());
                            // Spawn explosion. ai[0] is used for transferring the recorded falling time

                            int damage = Player.CalcIntDamage<MeleeDamageClass>(InterstellarStompers.SlamDamage);

                            Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ProjectileType<StomperSlam>(), damage, 4f, Player.whoAmI, gSabatonFall);
                            gSabatonFall = 0;
                            gSabatonFalling = false;
                            gSabatonHotkeyFallWindup = -1;

                            // Temporary jump speed is granted for 40 frames
                            gSabatonTempJumpSpeed = 40;
                        }
                    }
                }
            }
            else // Reset slam effect if the accessory is unequipped
            {
                gSabatonFall = 0;
                gSabatonFalling = false;
                gSabatonHotkeyFallWindup = -1;
            }

            // Reset The Evolution's same projectile DR if unequipped or the cooldown ends
            if (!evolution || !Player.HasCooldown(GlobalDodge.ID))
                projTypeJustHitBy = -1;
            }
        #endregion

        #region PreUpdate
        public override void PreUpdate()
        {
            // Increment the universal frame timer.
            ++universalFrameTimer;
            
            //Infinite flight granted by some boss attacks
            if (infiniteFlight)
                Player.wingTime = Player.wingTimeMax;

            // Reset the Calamity shader.
            CalamityFireDyeShader = null;

            if (HasCustomDash && UsedDash.IsOmnidirectional)
                Player.maxFallSpeed = 50f;

            // If the player isn't moving yet is still dashing, disable the dash (prevents weird hook jank)
            if (Player.velocity == Vector2.Zero && Player.dashDelay == -1)
                Player.dashDelay = 0;

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
                if (Player.dye[i].type == ItemType<ProfanedMoonlightDye>())
                    GameShaders.Armor.GetSecondaryShader(Player.dye[i].dye, Player)?.UseColor(GetCurrentMoonlightDyeColor());
            }

            // Syncing mouse controls
            if (Main.myPlayer == Player.whoAmI)
            {
                mouseRight = PlayerInput.Triggers.Current.MouseRight;
                var worldPos = LockOnHelper.Enabled ? LockOnHelper.PredictedPosition : Main.MouseWorld;
                mouseWorldDeltaFromPlayer = worldPos - Player.MountedCenter;
                mouseRotationFromPlayer = mouseWorldDeltaFromPlayer.ToRotation();

                if (rightClickListener && mouseRight != oldMouseRight)
                {
                    oldMouseRight = mouseRight;
                    syncMouseRightClick = true;
                    rightClickListener = false;
                }

                if (mouseWorldListener && Vector2.Distance(mouseWorldDeltaFromPlayer, oldMouseWorldDeltaFromPlayer) > 5f)
                {
                    oldMouseWorldDeltaFromPlayer = mouseWorldDeltaFromPlayer;
                    syncMousePosition = true;
                    mouseWorldListener = false;
                }

                if (mouseRotationListener && Math.Abs((mouseWorldDeltaFromPlayer).ToRotation() - (oldMouseWorldDeltaFromPlayer).ToRotation()) > 0.15f)
                {
                    oldMouseWorldDeltaFromPlayer = mouseWorldDeltaFromPlayer;
                    syncMouseRotation = true;
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
            if (Player.statMana < 0 && Player.Calamity().ChaosStone)
            {
                Player.AddBuff(BuffType<ManaBurn>(), 10);
            }
            else if (Player.HasBuff(BuffType<ManaBurn>()))
            {
                Player.ClearBuff(BuffType<ManaBurn>());
            }
        }
        #endregion

        #region OnExtraJumpStarted
        public override void OnExtraJumpStarted(ExtraJump jump, ref bool playSound)
        {
            if (rainSet && Player.whoAmI == Main.myPlayer)
            {
                RainArmorSetChange.SpawnRainArmorJump(Player);
            }
        }
        #endregion

        #region PreUpdateMovement
        public override void PreUpdateMovement()
        {

            // Rain armor set effects
            if (rainSet)
            {
                if (Player.justJumped && Player.whoAmI == Main.myPlayer)
                {
                    RainArmorSetChange.SpawnRainArmorJump(Player);
                }
            }

            //Multiply vertical speed
            if (redWine || Player.GetModPlayer<IVDripPlayer>().HasAlcohol(AlcoholType.RedWine))
            {
                if (Player.velocity.Y > 0.2f || Player.velocity.Y < -0.2f)
                {
                    redWineStoredY = Player.velocity.Y;
                    Player.velocity.Y *= 1 + (redWine ? RedWine.VerticalSpeedBoost : 0) + (Player.GetModPlayer<IVDripPlayer>().HasAlcohol(AlcoholType.RedWine) ? RedWine.VerticalSpeedBoost : 0);
                }
                else
                {
                    redWineStoredY = 0;
                }
            }
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

            foreach (var npc in Main.ActiveNPCs)
            {
                //This is the ravager rock pillar collision logic. It should only apply to rock pillars that are mostly opaque to ensure players don't get stopped by invisible pillars right when they spawn.
                //This could also potentially be limited to only run with the pillar is stationary if desired.
                if (npc.type == ModContent.NPCType<RockPillar>() && npc.Opacity > 0.9f && npc.damage < 1)
                {
                    bool LandedOnPlayer = false;
                    float origX = Player.velocity.X;
                    float origY = Player.velocity.Y;
                    npc.position += npc.velocity;
                    bool intersectPreVel = false;
                    Player.position += Player.velocity;

                    if (Player.Hitbox.Intersects(npc.Hitbox))
                        intersectPreVel = true;
                    Player.position -= Player.velocity;
                    if (intersectPreVel)
                    {
                        Player.position += npc.velocity;
                        if (Collision.SolidCollision(Player.position, Player.Hitbox.Width, Player.Hitbox.Height) || Main.npc.Any(x => x.active && x.type == ModContent.NPCType<RockPillar>() && x.Opacity > 0.9f && x.whoAmI != npc.whoAmI && x.Hitbox.Intersects(Player.Hitbox)))
                        {
                            npc.damage = npc.defDamage;
                            npc.ai[0] = 1;
                            LandedOnPlayer = true;
                            Player.position -= npc.velocity;
                        }
                    }
                    npc.position -= npc.velocity;

                    if (!LandedOnPlayer)
                    {
                        //Yes, testing intercepts on th X, Y, and X+Y individually has a purpose. It makes it far more stable and consistent.
                        bool intersectX = false;
                        Player.position.X += Player.velocity.X;
                        if (Player.Hitbox.Intersects(npc.Hitbox))
                            intersectX = true;
                        Player.position.X -= Player.velocity.X;
                        bool intersectY = false;
                        Player.position.Y += Player.velocity.Y;
                        if (Player.Hitbox.Intersects(npc.Hitbox))
                            intersectY = true;
                        Player.position.Y -= Player.velocity.Y;
                        bool intersect = false;
                        //We only bother checking the X+Y together if individually they return false.
                        //This accounts for moving diagonal onto a corner.
                        if (!intersectX && !intersectY)
                        {
                            Player.position += Player.velocity;
                            if (Player.Hitbox.Intersects(npc.Hitbox))
                                intersect = true;
                            Player.position -= Player.velocity;
                        }
                        if (intersectX || intersect)
                        {
                            //This checks if the player is to the left of the pillar
                            if (Player.Center.X < npc.Center.X)
                            {
                                float playerEdge = Player.Hitbox.X + Player.Hitbox.Width;
                                float pillarEdge = npc.Hitbox.X;
                                float goalEdge = playerEdge + Player.velocity.X;
                                if (goalEdge > pillarEdge)
                                {
                                    Player.velocity.X += pillarEdge - goalEdge;
                                    if (Player.velocity.X < 0)
                                        if (!Collision.SolidCollision(Player.position + new Vector2(Player.velocity.X, 0), Player.Hitbox.Width, Player.Hitbox.Height))
                                        {
                                            Player.position.X += Player.velocity.X;
                                            Player.velocity.X = 0;
                                        }
                                        else
                                        {
                                            Player.velocity.X = origX;
                                        }
                                }
                            }
                            //This is for if the player is to the right of the pillar
                            else
                            {
                                float playerEdge = Player.Hitbox.X;
                                float pillarEdge = npc.Hitbox.X + npc.Hitbox.Width;
                                float goalEdge = playerEdge + Player.velocity.X;
                                if (goalEdge < pillarEdge)
                                {
                                    Player.velocity.X += pillarEdge - goalEdge;
                                    if (Player.velocity.X > 0)
                                        if (!Collision.SolidCollision(Player.position + new Vector2(Player.velocity.X, 0), Player.Hitbox.Width, Player.Hitbox.Height))
                                        {
                                            Player.position.X += Player.velocity.X;
                                            Player.velocity.X = 0;
                                        }
                                        else
                                        {
                                            Player.velocity.X = origX;
                                        }
                                }
                            }
                        }
                        //For the vertical axis, we don't distinguish between top and bottom, and always move players to the top
                        //This is so being placed inside a pillar teleports a player to the top, which applies primarily to teleporting or pillar spawning
                        //Additionally, this is set to only work when the pillar is stationary vertically to prevent janky interations when pillars are moving.
                        if ((intersectY || intersect))
                        {
                            if (Player.Center.Y < npc.Center.Y)
                            {
                                float playerEdge = Player.Hitbox.Y + Player.Hitbox.Height;
                                float pillarEdge = npc.Hitbox.Y;
                                float goalEdge = playerEdge + Player.velocity.Y;
                                if (goalEdge > pillarEdge)
                                {
                                    Player.velocity.Y += pillarEdge - goalEdge;
                                    if (Player.velocity.Y < 0)
                                        if (!Collision.SolidCollision(Player.position + new Vector2(0, Player.velocity.Y), Player.Hitbox.Width, Player.Hitbox.Height))
                                        {
                                            Player.position.Y += Player.velocity.Y;
                                            Player.velocity.Y = 0;
                                        }
                                        else
                                        {
                                            Player.velocity.Y = origY;
                                        }
                                }
                            }
                            else
                            {
                                float playerEdge = Player.Hitbox.Y;
                                float pillarEdge = npc.Hitbox.Y + npc.Hitbox.Height;
                                float goalEdge = playerEdge + Player.velocity.Y;
                                if (goalEdge < pillarEdge)
                                {
                                    Player.velocity.Y += pillarEdge - goalEdge;
                                    if (Player.velocity.Y > 0)
                                        if (!Collision.SolidCollision(Player.position + new Vector2(0, Player.velocity.Y), Player.Hitbox.Width, Player.Hitbox.Height))
                                        {
                                            Player.position.Y += Player.velocity.Y;
                                            Player.velocity.Y = 0;
                                        }
                                        else
                                        {
                                            Player.velocity.Y = origY;
                                        }
                                }
                            }
                        }
                        //This is to guarantee we can stand on top of rock pillars instead of looking like we're "floating" on top of them.
                        if (intersectY && Player.velocity.Y < 0.25f && Player.velocity.Y > -0.25f)
                        {
                            Player.velocity.Y = 0;
                        }
                    }
                }
            }
        }
        #endregion

        #region PostUpdateBuffs
        public override void PostUpdateBuffs()
        {
            if (Player.whoAmI == Main.myPlayer && CalamityClientConfig.Instance.VanillaCooldownDisplay)
            {
                if (cooldowns.TryGetValue(PotionSickness.ID, out CooldownInstance cd))
                {
                    if (Player.potionDelay != cd.timeLeft && cd.timeLeft > 0)
                        cd.timeLeft = Player.potionDelay;

                    if (cd.timeLeft > cd.duration)
                        cd.duration = cd.timeLeft; // If the new cooldown is larger than the full duration, update, else keep it the same.
                }
                else if (Player.whoAmI == Main.myPlayer && Player.potionDelay > 0)
                    Player.AddCooldown(PotionSickness.ID, Player.potionDelay, false);

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
                float lifeStealRecoveryRateReduction = Main.expertMode ? BalancingConstants.LifeStealRecoveryRateReduction_Expert : BalancingConstants.LifeStealRecoveryRateReduction_Classic;
                float lifeStealCap = Main.expertMode ? BalancingConstants.LifeStealCap_Expert: BalancingConstants.LifeStealCap_Classic;

                float lifeStealRecoveryRate = baseRecoveryRate - lifeStealRecoveryRateReduction;
                
                if (Player.lifeSteal < lifeStealCap)
                {

                    if (Player.Calamity().cooldowns.TryGetValue(LifeSteal.ID, out var cooldown))
                    {
                        cooldown.timeLeft = (int)Math.Max(0,(lifeStealCap-Math.Abs(Player.lifeSteal)));
                    }
                    else
                    {
                        Player.AddCooldown(LifeSteal.ID, (int)lifeStealCap)
                            .timeLeft = (int)Math.Max(0, (lifeStealCap - Math.Abs(Player.lifeSteal)));
                    }
                }
            }

            if (malnourished) // Has to be here because alcoholPoisonLevel isn't updated until now
            {
                alcoholPoisonMax += TheSandwich.alcoholCapBoost;
                // Gives inverted buffs from well fed
                Player.statDefense -= 2;
                Player.GetCritChance<GenericDamageClass>() -= 2;
                Player.minionKB -= 0.5f;
                Player.moveSpeed -= 0.05f;
                // damage nerf/boost happens in PostUpdate
                Player.pickSpeed += 0.05f;


                for (int l = 0; l < Player.MaxBuffs; ++l)
                {
                    int buffID = Player.buffType[l];
                    if ((CalamityBuffSets.DebuffDataset[buffID] != null && CalamityBuffSets.DebuffDataset[buffID].AlcoholLevel > 0) || buffID == BuffID.Tipsy)
                    {
                        Player.buffTime[l]++;
                    }
                }

                if (Player.wellFed)
                    Player.ClearBuff(ModContent.BuffType<Malnourished>());
            }

            if (moonshine)
                Player.statLifeMax2 = (int)(Player.statLifeMax2 * (1 + Moonshine.MaxLifePercentBoost));
            ForceVariousEffects();
        }
        #endregion

        #region PreItemChecks
        public override bool PreItemCheck()
        {
            //For use when the player runs out of mana. Currently only used for Feather and Moonstone Crown
            if (Player.HeldItem.mana > 0 && Player.controlUseItem && Player.itemAnimation == 0)
            {
                if (!Player.CheckMana(Player.HeldItem, -1, false, true))
                {
                    if (moonCrown || featherCrown)
                    {
                        if (mageCrownCount > 0)
                        {
                            mageCrownCount = 0;
                            mageCrownTimer = 300;
                        }
                        if (mageCrownCount < 0) mageCrownCount = 0;
                    }
                }
            }
            return true;
        }
        #endregion

        #region PostUpdateEquips
        public override void PostUpdateEquips()
        {
            var dripPlayer = Player.GetModPlayer<IVDripPlayer>();
            var item = Player.HeldItem;
            if (!(item.IsAir || item.damage <= 0))
            {
                if (item.DamageType.CountsAsClass(DamageClass.Melee))
                {
                    TimeHoldingMelee++;
                    if (TimeHoldingMelee > Whiskey.TimeToDischarge)
                        TimeHoldingMelee = (int)Whiskey.TimeToDischarge;

                    if (dripPlayer.HasAlcohol(AlcoholType.Whiskey) && whiskey)
                        Player.GetDamage(DamageClass.Melee) += MathHelper.Lerp(Whiskey.CombinedDamageCeiling, Whiskey.CombinedDamageFloor, TimeHoldingMelee / Whiskey.TimeToDischarge);
                    else if (whiskey || dripPlayer.HasAlcohol(AlcoholType.Whiskey))
                        Player.GetDamage(DamageClass.Melee) += MathHelper.Lerp(Whiskey.StandardDamageCeiling, Whiskey.StandardDamageFloor, TimeHoldingMelee / Whiskey.TimeToDischarge);
                } 
                else
                {
                    TimeHoldingMelee -= 2;
                    if (TimeHoldingMelee < 0)
                        TimeHoldingMelee = 0;
                }
                if (item.DamageType.CountsAsClass(DamageClass.Ranged))
                {
                    TimeHoldingRanged++;
                    if (TimeHoldingRanged > Whiskey.TimeToDischarge)
                        TimeHoldingRanged = (int)Whiskey.TimeToDischarge;

                    if (dripPlayer.HasAlcohol(AlcoholType.Whiskey) && whiskey)
                        Player.GetDamage(DamageClass.Ranged) += MathHelper.Lerp(Whiskey.CombinedDamageCeiling, Whiskey.CombinedDamageFloor, TimeHoldingRanged / Whiskey.TimeToDischarge);
                    else if (whiskey || dripPlayer.HasAlcohol(AlcoholType.Whiskey))
                        Player.GetDamage(DamageClass.Ranged) += MathHelper.Lerp(Whiskey.StandardDamageCeiling, Whiskey.StandardDamageFloor, TimeHoldingRanged / Whiskey.TimeToDischarge);
                }
                else
                {
                    TimeHoldingRanged -= 2;
                    if (TimeHoldingRanged < 0)
                        TimeHoldingRanged = 0;
                }
                if (item.DamageType.CountsAsClass(DamageClass.Magic))
                {
                    TimeHoldingMagic++;
                    if (TimeHoldingMagic > Whiskey.TimeToDischarge)
                        TimeHoldingMagic = (int)Whiskey.TimeToDischarge;

                    if (dripPlayer.HasAlcohol(AlcoholType.Whiskey) && whiskey)
                        Player.GetDamage(DamageClass.Magic) += MathHelper.Lerp(Whiskey.CombinedDamageCeiling, Whiskey.CombinedDamageFloor, TimeHoldingMagic / Whiskey.TimeToDischarge);
                    else if (whiskey || dripPlayer.HasAlcohol(AlcoholType.Whiskey))
                        Player.GetDamage(DamageClass.Magic) += MathHelper.Lerp(Whiskey.StandardDamageCeiling, Whiskey.StandardDamageFloor, TimeHoldingMagic / Whiskey.TimeToDischarge);
                }
                else
                {
                    TimeHoldingMagic -= 2;
                    if (TimeHoldingMagic < 0)
                        TimeHoldingMagic = 0;
                }
                if (item.DamageType.CountsAsClass(DamageClass.Summon))
                {
                    TimeHoldingSummon++;
                    if (TimeHoldingSummon > Whiskey.TimeToDischarge)
                        TimeHoldingSummon = (int)Whiskey.TimeToDischarge;

                    if (dripPlayer.HasAlcohol(AlcoholType.Whiskey) && whiskey)
                        Player.GetDamage(DamageClass.Summon) += MathHelper.Lerp(Whiskey.CombinedDamageCeiling, Whiskey.CombinedDamageFloor, TimeHoldingSummon / Whiskey.TimeToDischarge);
                    else if (whiskey || dripPlayer.HasAlcohol(AlcoholType.Whiskey))
                        Player.GetDamage(DamageClass.Summon) += MathHelper.Lerp(Whiskey.StandardDamageCeiling, Whiskey.StandardDamageFloor, TimeHoldingSummon / Whiskey.TimeToDischarge);
                }
                else
                {
                    TimeHoldingSummon -= 2;
                    if (TimeHoldingSummon < 0)
                        TimeHoldingSummon = 0;
                }
                if (item.DamageType.CountsAsClass(RogueDamageClass.Instance))
                {
                    TimeHoldingRogue++;
                    if (TimeHoldingRogue > Whiskey.TimeToDischarge)
                        TimeHoldingRogue = (int)Whiskey.TimeToDischarge;

                    if (dripPlayer.HasAlcohol(AlcoholType.Whiskey) && whiskey)
                        Player.GetDamage(RogueDamageClass.Instance) += MathHelper.Lerp(Whiskey.CombinedDamageCeiling, Whiskey.CombinedDamageFloor, TimeHoldingRogue / Whiskey.TimeToDischarge);
                    else if (whiskey || dripPlayer.HasAlcohol(AlcoholType.Whiskey))
                        Player.GetDamage(RogueDamageClass.Instance) += MathHelper.Lerp(Whiskey.StandardDamageCeiling, Whiskey.StandardDamageFloor, TimeHoldingRogue / Whiskey.TimeToDischarge);
                    
                }
                else
                {
                    TimeHoldingRogue -= 2;
                    if (TimeHoldingRogue < 0)
                        TimeHoldingRogue = 0;
                }
            } else
            {
                if (TimeHoldingMelee-- < 0)
                    TimeHoldingMelee = 0;
                if (TimeHoldingRanged-- < 0)
                    TimeHoldingRanged = 0;
                if (TimeHoldingMagic-- < 0)
                    TimeHoldingMagic = 0;
                if (TimeHoldingSummon-- < 0)
                    TimeHoldingSummon = 0;
                if (TimeHoldingRogue-- < 0)
                    TimeHoldingRogue = 0;
            }
            if (dripPlayer.HasAlcohol(AlcoholType.Moonshine))
            {
                Player.statLifeMax2 = (int)(Player.statLifeMax2 * (1 + Moonshine.MaxLifePercentBoost));
            }


            // PostUpdateMiscEffects runs after the cap has been applied. Do NOT put mining speed stuff there.
            // Ancient Chisel nerf (also affects Hand of Creation)
            if (Player.chiselSpeed)
                Player.pickSpeed += 0.1f;

            if (oceanCrest)
            {
                bool surface = Player.Center.Y < Main.worldSurface * 16.0;
                bool GetEffects = ((Main.raining && surface) || Player.dripping || (Player.wet && !Player.lavaWet && !Player.honeyWet));
                if (GetEffects)
                {
                    if (oceanCrestTimer < 300)
                        oceanCrestTimer += 5;
                    if (Player.StandingStill(0.1f) && !ZoneAbyss && Player.breath < 201 && Player.miscCounter % 2 == 0)
                        Player.breath += 1;
                }
                else
                    if (oceanCrestTimer > 0)
                    oceanCrestTimer--;

                if (oceanCrestTimer > 0 || GetEffects)
                    Player.pickSpeed -= 0.15f; // 15% mining speed

                Vector3 Light = new Vector3(0.090f, 0.180f, 0.200f);
                Lighting.AddLight(Player.Center, Light * (0.55f + (oceanCrestTimer * 0.0035f)));
            }

            if (rampartOfDeities)
            {
                // Ice Barrier buff inherited from Frozen Turtle Shell
                if (Player.statLife <= Player.statLifeMax2 * 0.5)
                    Player.AddBuff(BuffID.IceBarrier, 5);

                // Paladin's Shield application
                if (Player.statLife > Player.statLifeMax2 * 0.25f)
                {
                    Player.hasPaladinShield = true;
                    if (Player.whoAmI != Main.myPlayer && Player.miscCounter % 10 == 0)
                    {
                        var localPlayer = Main.LocalPlayer;
                        if (localPlayer.team == Player.team && Player.team != 0)
                        {
                            float teamPlayerXDist = Player.position.X - localPlayer.position.X;
                            float teamPlayerYDist = Player.position.Y - localPlayer.position.Y;
                            if ((float)Math.Sqrt(teamPlayerXDist * teamPlayerXDist + teamPlayerYDist * teamPlayerYDist) < 800f)
                                localPlayer.AddBuff(BuffID.PaladinsShield, 20);
                        }
                    }
                }
            }

            if (scionsCurio)
            {
                scionsCurioDebuffDamage = Player.GetTotalDamage(DamageClass.Ranged).ApplyTo(Irradiated.debuffData.EnemyLostRegen * (1 + Player.GetTotalCritChance(DamageClass.Ranged) * 0.01f));
            }
            else
                scionsCurioDebuffDamage = 0;

            if (Player.Calamity().abaddon || Player.Calamity().apollyon)
            {
                Player.Calamity().playerBaneDebuffDamage = (1 + Player.GetTotalCritChance(Player.GetBestClass()) * 0.01f * (Player.Calamity().apollyon ? Apollyon.critScaling : Abaddon.critScaling));
            }
            else
                Player.Calamity().playerBaneDebuffDamage = 0;

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

            //Update life regen that needs to happen before calculating debuffs
            GeneralLifeRegen();
        }
        #endregion

        #region PostUpdate
        #region Shop Restrictions
        public override bool CanSellItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (item.type == ItemType<ProfanedSoulCrystal>())
                return DownedBossSystem.downedCalamitas && DownedBossSystem.downedExoMechs; //no easy moneycoins for post doggo/yhar
            return base.CanSellItem(vendor, shopInventory, item);
        }

        #endregion

        public override void PostUpdate()
        {
            if (Player.Calamity().XykVisualsBlue || Player.Calamity().XykVisualsOrange)
                Player.wings = 0;

            if (ZoneAbyss && Main.netMode != NetmodeID.Server)
            {
                //Main aura
                EnhancedDarknessSystem.lights.Add(new(center: Player.Center, scale: 4 * abyssPlayerGlowMultiplier));

                //Flashlight
                //Due to being in postUpdate we need to adjust the mousepos for the player's movement that will have happened
                mouseWorldListener = true;
                var mouseworld = mouseWorld + Player.position - Player.oldPosition;
                EnhancedDarknessSystem.lights.Add(new(center: Player.Center + Player.DirectionTo(mouseworld) * 750f, rotation: Player.DirectionTo(mouseworld).ToRotation() - MathHelper.PiOver2, vectorScale: new Vector2(0.85f * abyssFlashlightWidthMultiplier, 0.75f), texture: Request<Texture2D>("CalamityMod/Particles/BloomLineFade").Value));
            }

            if (lastDeerclopsPosition.HasValue)
            {
                //While deer is alive and the player is nearby, make it dark
                if (Main.npc.Any(x => x.active && x.type == NPCID.Deerclops) && Player.DistanceSQ(lastDeerclopsPosition.Value) < 10240000) //3200^2, or 00 tiles
                {
                    darknessIntensity = MathHelper.Min(Main.LocalPlayer.Calamity().darknessIntensity + 0.06f, 1f); ;
                }

                //Add the main light circle for the arena
                DeerclopsAI.ArenaTex ??= ModContent.Request<Texture2D>(DeerclopsAI.ArenaTexPath);
                EnhancedDarknessSystem.lights.Add(new(lastDeerclopsPosition.Value, scale: DeerclopsAI.borderScale, texture: DeerclopsAI.ArenaTex.Value));

                //we draw light around the player when far away so they have some visibility, although very small. This is especially nice in multiplayer. we scale opacity with distance bc it looks better
                EnhancedDarknessSystem.lights.Add(new EnhancedDarknessSystem.LightSource(scale: 0.75f, opacity: MathHelper.Clamp(Main.LocalPlayer.DistanceSQ(lastDeerclopsPosition.Value) / (409600 /*640^2*/), 0, 1)));

                if (darknessIntensity == 0)
                {
                    lastDeerclopsPosition = null;
                }
            }

            //reset the stored Y value for red wine's increased vertical speed
            if ((redWine || Player.GetModPlayer<IVDripPlayer>().HasAlcohol(AlcoholType.RedWine)) && (redWineStoredY > 0.2f || redWineStoredY < -0.2f) && (Player.velocity.Y > 0.2f || Player.velocity.Y < -0.2f))
            {
                Player.velocity.Y = redWineStoredY;
            }


            if (subtitletext != null)
            {
                if (!subtitletext.active)
                {
                    subtitletext = null;
                }
                else
                {
                    subtitletext.position = Player.Center + new Vector2(-FontAssets.CombatText[subtitletext.crit ? 1 : 0].Value.MeasureString(subtitletext.text).X * 0.5f, 64);
                    subtitletext.color = Color.Lerp(subtitleColors[1], subtitleColors[0], subtitletext.lifeTime / 120f);
                }
            }

            // Frozen Cube defense cut
            if (Player.Calamity().frozenCube)
            {
                int usedDefense = (Player.statDefense / FrozenCube.usedDefenseDivide);
                Player.Calamity().frozenCubeUsedDefense = usedDefense;
                Player.statDefense -= usedDefense;
            }

            if (malnourished) // Has to be here because alcoholPoisonLevel isn't updated until now
                Player.GetDamage<GenericDamageClass>() += -0.05f + TheSandwich.statBoost * alcoholPoisonLevel;

            // Relic of Convergence defense cut
            if (Player.ownedProjectileCounts[ModContent.ProjectileType<RelicOfConvergenceCrystal>()] > 0 && Player.HeldItem.type == ModContent.ItemType<RelicOfConvergence>())
                Player.statDefense *= RelicOfConvergence.DefenseMultiplier;

            #region Managing time control
            if (Main.netMode != NetmodeID.Server && Player == Main.LocalPlayer)
            {
                var power = CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>();
                double dayrate = CreativePowerManager.Instance.GetPower<CreativePowers.ModifyTimeRate>().TargetTimeRate;
                if (CurrentFrameFlags.SleepingPlayersCount == CurrentFrameFlags.ActivePlayersCount && CurrentFrameFlags.SleepingPlayersCount > 0)
                    dayrate *= 5;
                if (Main.IsFastForwardingTime())
                    dayrate = 60;
                double tileUpdate = 1;
                double eventUpdate = 1;
                SystemLoader.ModifyTimeRate(ref dayrate, ref tileUpdate, ref eventUpdate);
                if (WeakTimeFreezeInUse)
                {
                    if (!power.Enabled)
                        WeakTimeFreezeInUse = false;
                    else
                    {
                        if (WeakTimeFreezeUseTimer >= Bakidon.FreezeTime / Bakidon.RechargeMultiplier || Main.bloodMoon || Main.eclipse || Main.pumpkinMoon || Main.snowMoon)
                        {

                            NetPacket packet = NetCreativePowersModule.PreparePacket(power.PowerId, 1);
                            packet.Writer.Write(false);
                            NetManager.Instance.SendToServerOrLoopback(packet);
                            WeakTimeFreezeInUse = false;
                        }
                        WeakTimeFreezeUseTimer += 1 * (float)dayrate;
                    }
                }
                else
                {
                    if (WeakTimeFreezeUseTimer > 0)
                        WeakTimeFreezeUseTimer -= 1 * (float)dayrate;
                    else
                        WeakTimeFreezeUseTimer = 0;
                }
            }
            #endregion
        }
        public override void PostUpdateRunSpeeds()
        {
            #region SpeedBoosts
            if (!Player.mount.Active)
            {
                float runAccMult = 1f +
                    (lunicCorpsLegs ? LunicCorpsBoots.MoveSpeedAccelerationBoost : 0f) +
                    (shadowSpeed ? DemonshadeGreaves.AccelerationBoost : 0f) +
                    (stressPills ? 0.05f : 0f) +
                    ((abyssalDivingSuit && Player.IsUnderwater()) ? 0.05f : 0f) +
                    (aquaticHeartWaterBuff ? AquaticHeart.WaterSpeedBoost : 0f) +
                    (laudanum && Player.HasBuff(BuffID.VortexDebuff) ? 0.15f : 0f) +
                    ((frostFlare && Player.statLife <= (int)(Player.statLifeMax2 * FrostFlare.HealthRatioThreshold)) ? FrostFlare.AccelerationBoost : 0f) +
                    (dragonScales ? 0.1f : 0f) +
                    (CobaltSet ? CobaltArmorSetChange.SpeedBoostSetBonusPercentage * 0.01f : 0f) +
                    (silvaSet ? SilvaArmor.AccelerationBoost : 0f) +
                    (getSandCloakAccelBoost ? Items.Accessories.SandCloak.SandVeilAccelerationBoost : 0f) +
                    (ascendantInsignia ? (AscendantInsignia.AccelerationBoost - 0.25f) : 0f) + // Subtracted 0.25 provided by Soaring Insignia
                    (statisNinjaBelt ? 0.6f : 0f) +
                    (statisVoidSash ? 0.85f : 0f) +
                    (blueCandle ? WeightlessCandle.AccelerationBoost : 0f) +
                    (planarSpeedBoost > 0 ? (0.01f * planarSpeedBoost) : 0f) +
                    //(exaltedKillMode ? 7f : devilsDevastationKillMode ? 11f : 0) +
                    (hasteLevel * 0.05f);

                float runSpeedMult = 1f +
                    (lunicCorpsLegs ? LunicCorpsBoots.MoveSpeedAccelerationBoost : 0f) +
                    (shadowSpeed ? DemonshadeGreaves.AccelerationBoost : 0f) +
                    (stressPills ? 0.05f : 0f) +
                    ((abyssalDivingSuit && Player.IsUnderwater()) ? 0.05f : 0f) +
                    (aquaticHeartWaterBuff ? AquaticHeart.WaterSpeedBoost : 0f) +
                    ((frostFlare && Player.statLife <= (int)(Player.statLifeMax2 * FrostFlare.HealthRatioThreshold)) ? FrostFlare.AccelerationBoost : 0f) +
                    (dragonScales ? 0.1f : 0f) +
                    (CobaltSet ? CobaltArmorSetChange.SpeedBoostSetBonusPercentage * 0.01f : 0f) +
                    (silvaSet ? SilvaArmor.AccelerationBoost : 0f) +
                    (planarSpeedBoost > 0 ? (0.01f * planarSpeedBoost) : 0f) +
                    //(exaltedKillMode ? 0.4f : devilsDevastationKillMode ? 0.7f : 0) +
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

            if ((Player.Calamity().baconOil || Player.GetModPlayer<IVDripPlayer>().HasAlcohol(AlcoholType.BaconOil)) && !Player.mount.Active)
            {
                Player.noFallDmg = true;

                float stackScale = (Player.Calamity().baconOil && Player.GetModPlayer<IVDripPlayer>().HasAlcohol(AlcoholType.BaconOil)) ? 1.33f : 1;

                float velX = Math.Abs(Player.velocity.X);
                float velY = Math.Abs(Player.velocity.Y);

                float speedMult = Player.GetBestClassDamage().ApplyTo(1);
                float maxMoveSpeed = 10.5f * speedMult * stackScale;
                float minMoveSpeed = 3 * stackScale; // Minimum move speed to be at if the player can bounce
                float hitboxSizeMult = 0.45f;

                bool onPlatform = false;
                if (Player.velocity.Y > 0 && !Player.controlDown)
                {
                    for (int i = 4; i < 8; i++)
                    {
                        Point bottom = (Player.Bottom + Vector2.UnitY * i).ToTileCoordinates();
                        if (TileID.Sets.Platforms[CalamityUtils.ParanoidTileRetrieval(bottom.X, bottom.Y).TileType])
                        {
                            onPlatform = true;
                        }
                    }
                }

                bool hitDown =  velY > minMoveSpeed && (Collision.SolidCollision(Player.Bottom + Vector2.UnitY * Player.velocity.Y, (int)(Player.width * hitboxSizeMult), 1) || onPlatform);
                bool hitUp =  velY > minMoveSpeed && Collision.SolidCollision(Player.Top + Vector2.UnitY * Player.velocity.Y, (int)(Player.width * hitboxSizeMult), 1);
                bool hitSide = velX > minMoveSpeed && Collision.SolidCollision(Vector2.Lerp(Player.TopLeft, Player.Left, 0.15f) + Vector2.UnitX * Player.velocity.X, 1, (int)(Player.height * hitboxSizeMult)) || Collision.SolidCollision(Vector2.Lerp(Player.TopRight, Player.Right, 0.15f) + Vector2.UnitX * Player.velocity.X, 1, (int)(Player.height * hitboxSizeMult));
                float acceleration = 0.05f * stackScale;
                float decceleration = -0.025f / speedMult;
                float riseSpeed = 0.8f * stackScale; // The speed the player rises when in water or rain
                float slipPower = 0.85f; //How strong the slippery effect is

                float bounceReduction = 0.9f;
                bool makeSound = false;
                if (hitSide)
                {
                    Player.velocity.X = -Player.velocity.X * bounceReduction;
                    Player.oldVelocity.X = -Player.oldVelocity.X * bounceReduction;
                    makeSound = true;
                }
                if ((hitDown || hitUp))
                {
                    if (Player.controlUp)
                        bounceReduction = 1.3f;
                    Player.velocity.Y = -Player.velocity.Y * bounceReduction;
                    Player.oldVelocity.Y = -Player.oldVelocity.Y * bounceReduction;
                    if (hitDown)
                    {
                        Player.RefreshMovementAbilities(true);
                    }
                    makeSound = true;
                }
                int soundCDMax = 150;
                if (makeSound && baconOilSoundCooldown < soundCDMax)
                {
                    if (baconOilSoundCooldown < soundCDMax)
                        baconOilSoundCooldown += 30;
                    float pitch = (0.55f * stackScale) * Utils.GetLerpValue(0, soundCDMax, baconOilSoundCooldown, true);
                    SoundStyle boyoyoing = new("CalamityMod/Sounds/Item/BaconOilBounce");
                    SoundEngine.PlaySound(boyoyoing with { Volume = 0.45f, Pitch = hitSide ? pitch - 0.3f : pitch, MaxInstances = 5 }, Player.Center);
                }

                bool playerMoving = Player.controlLeft || Player.controlRight;
                bool playerMovingInDirection = (Player.controlLeft && Player.oldVelocity.X < 0) || (Player.controlRight && Player.oldVelocity.X > 0);
                if (((Player.oldVelocity.X > 0 && Player.velocity.X > 0) || (Player.oldVelocity.X < 0 && Player.velocity.X < 0)))
                    Player.velocity.X = MathHelper.Clamp(Player.velocity.X + maxMoveSpeed * MathF.Sign(Player.oldVelocity.X) * 
                        ((playerMovingInDirection || !playerMoving) ? Player.dashDelay == -1 ? decceleration : // If dashing, deccelerate to prevent infinite dash
                        // If not moving or holding in move direction, accelerate to max speed, otherwise deccelerate
                        acceleration : decceleration), -maxMoveSpeed, maxMoveSpeed);

                bool raining = Player.Center.Y < Main.worldSurface * 16.0 && Main.raining;
                if ((Player.wet || raining) && Player.velocity.Y > -maxMoveSpeed)
                {
                    Player.velocity.Y -= riseSpeed; // Cover yourself in oil (go up in liquid)
                }


                Vector2 lerpedVel = Vector2.Lerp(Player.velocity, Player.oldVelocity, slipPower);
                Player.velocity = new Vector2(lerpedVel.X, Player.velocity.Y);
            }
            if (baconOilSoundCooldown > 0)
                baconOilSoundCooldown = (int)MathHelper.Lerp(baconOilSoundCooldown, 0, 0.027f);

            Player.oldVelocity = Player.velocity; // Apparently this value is not updated on its own, so we do it
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
            if (bloomStone)
                healValue = 0;
        }
        #endregion

        #region Get Weapon Damage And KB
        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            if (item.CountsAsClass<RogueDamageClass>() && item.TryGetGlobalItem<RogueGlobalItem>(out var rogueItem))
            {
                // Apply weapon modifier stealth strike damage bonus
                // 01OCT2023: Ozzatron: This is a multiplicative bonus because it is a prefix.
                // It should be equivalent to x1.15 (or whatever multiplier) on the base damage of the weapon for stealth only.
                bool dontProvideStealthDamage = Player.HeldItem.type == ModContent.ItemType<ElephantKiller>();
                if (rogueItem.StealthStrikePrefixBonus != 0f && StealthStrikeAvailable() && !dontProvideStealthDamage)
                    damage *= rogueItem.StealthStrikePrefixBonus; // This number centers on 1f, so 1.15f = 1.15x damage.
            }
        }

        public override void ModifyWeaponKnockback(Item item, ref StatModifier knockback)
        {
            // Adding to StatModifier adds to the additive multiplier
            bool rogue = item.CountsAsClass<RogueDamageClass>();

            if (moscowMule)
                knockback += MoscowMule.KnockbackBoost;
            if (Player.GetModPlayer<IVDripPlayer>().HasAlcohol(AlcoholType.MoscowMule))
                knockback += MoscowMule.KnockbackBoost;

            if (titanHeartMantle && rogue)
                knockback += TitanHeartMantle.RogueKnockbackBoost;

            if (titanHeartSet && StealthStrikeAvailable() && rogue)
                knockback *= TitanHeartMask.StealthStrikeKnockbackMult;
        }
        #endregion

        #region Modify Luck
        public override void ModifyLuck(ref float luck)
        {
            luck += calamityBonusLuck;
        }
        #endregion

        #region Modify Mana Cost
        public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
        {
            if (CalamityItemSets.MagicGun[item.type] && meteorSet)
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
                        Dust confetti = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, confettiDust, Player.velocity.X, Player.velocity.Y, 0, new Color(), 1.2f);
                        confetti.velocity.X *= (float)(1.0 + Main.rand.Next(-50, 51) * 0.01);
                        confetti.velocity.Y *= (float)(1.0 + Main.rand.Next(-50, 51) * 0.01);
                        confetti.velocity.X += Main.rand.Next(-50, 51) * 0.05f;
                        confetti.velocity.Y += Main.rand.Next(-50, 51) * 0.05f;
                        confetti.scale *= (float)(1.0 + Main.rand.Next(-30, 31) * 0.01);
                    }
                    if (Main.rand.NextBool(40) && !Main.dedServ)
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
                if (fungalSymbiote && Player.HasBuff(BuffType<Mushy>()) && Player.whoAmI == Main.myPlayer)
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
                        Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, Main.rand.NextBool(3) ? 114 : DustType<BrimstoneFlame>(), Player.velocity.X * 0.2f + Player.direction * 3f, Player.velocity.Y * 0.2f, 100, default, Main.rand.NextFloat(0.3f, 1f));
                    }
                }
                if (flaskCrumbling)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, Main.rand.NextBool() ? 121 : DustID.Stone, Player.velocity.X * 0.2f + Player.direction * 3f, Player.velocity.Y * 0.2f, 100, default, Main.rand.NextFloat(0.2f, 0.7f));
                    }
                }
                if (eGauntlet && eGauntletVisuals)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust rainbow = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.RainbowTorch, Player.velocity.X * 0.2f + Player.direction * 3f, Player.velocity.Y * 0.2f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.25f);
                        rainbow.noGravity = true;
                    }
                }
                if (dsSetBonus)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Shadowflame, Player.velocity.X * 0.2f + Player.direction * 3f, Player.velocity.Y * 0.2f, 100, default, 0.7f);
                    }
                }
            }
        }
        #endregion

        #region Shoot
        // Brimflame Frenzy disables mana potion mana recovery
        // This also covers all Quick Hotkeys
        public override bool CanUseItem(Item item)
        {
            if (item.healMana > 0 && brimflameFrenzy)
                return false;
            return base.CanUseItem(item);
        }

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
        {
            if (bladeArmEnchant)
                return false;
            if (purpleHaze || Player.GetModPlayer<IVDripPlayer>().HasAlcohol(AlcoholType.PurpleHaze))
            {
                if (item.CountsAsClass<RogueDamageClass>() && StealthStrikeAvailable())
                {
                    purpleHazeStealthTimer = 300;
                }
            }
            if (veneratedLocket)
            {
                var LocketSource = Player.GetSource_Accessory(FindAccessory<VeneratedLocket>());
                if (item.CountsAsClass<RogueDamageClass>())
                {
                    if (!CalamityItemSets.DisablesVeneratedLocketEffect[item.type])
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
                        int locketDamage = (int)(damage * 0.07f);
                        int p = Projectile.NewProjectile(LocketSource, realPlayerPos.X, realPlayerPos.Y, speedX4, speedY5, type, locketDamage, knockBack * 0.5f, Player.whoAmI);

                        if (p.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[p].DamageType = DamageClass.Generic; //Makes it not proc shit like nanotech, extorter and other stuff
                            Main.projectile[p].Calamity().LocketClone = true; //To not have clones trigger effects like Sacrifice's Lifesteal and Final Dawn's stealth generation

                            Projectile.NewProjectile(LocketSource, Main.projectile[p].Center, Vector2.Zero, ModContent.ProjectileType<DoGWeaponTeleportRift>(), 0, 0, Player.whoAmI); // Spawn a little portal!
                        }

                        // Handle AI edge-cases. These are like overlapping projectiles and the projectile not spawning at all
                        if (item.type == ItemType<TheFinalDawn>())
                            Main.projectile[p].ai[1] = 1f; //MUST BE 1 OTHERWISE CLONES GENERATE STEALTH AAAAAAAAAAA
                        if (item.type == ItemType<TheAtomSplitter>())
                            Main.projectile[p].ai[0] = -1f;
                    }

                    if (StealthStrikeAvailable())
                    {
                        int knifeCount = 12;
                        int knifeDamage = (int)Player.GetTotalDamage<RogueDamageClass>().ApplyTo(55);

                        float angleStep = MathHelper.TwoPi / knifeCount;
                        float speed = 14f;

                        for (int i = 0; i < knifeCount; i++)
                        {
                            Vector2 velocity2 = new Vector2(0f, speed);
                            velocity2 = velocity2.RotatedBy(angleStep * i);
                            int knifeCol = Main.rand.Next(0, 2);

                            int knife = Projectile.NewProjectile(LocketSource, Player.Center, velocity2, ProjectileType<VeneratedKnife>(), knifeDamage, 0f, Player.whoAmI, knifeCol, 0);
                            if (knife.WithinBounds(Main.maxProjectiles))
                                Main.projectile[knife].DamageType = DamageClass.Generic;
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
            // Mannequin frame effects
            // These "players" never load Calamity's equippable bools so they need to be manually loaded
            if (Player.isDisplayDollOrInanimate)
            {
                // Body
                if (Player.armor[1].type == ItemType<AuricTeslaBodyArmor>())
                    Player.body = EquipLoader.GetEquipSlot(Mod, "AuricTeslaBodyArmor", EquipType.Body);
                else if (Player.armor[1].type == ItemType<DaedalusBreastplate>())
                    Player.body = EquipLoader.GetEquipSlot(Mod, "DaedalusBreastplate", EquipType.Body);
                else if (Player.armor[1].type == ItemType<EmpyreanCloak>())
                    Player.body = EquipLoader.GetEquipSlot(Mod, "EmpyreanCloak", EquipType.Body);
                else if (Player.armor[1].type == ItemType<SnowRuffianChestplate>())
                    Player.body = EquipLoader.GetEquipSlot(Mod, "SnowRuffianChestplate", EquipType.Body);
                else if (Player.armor[1].type == ItemType<VictideBreastplate>())
                    Player.body = EquipLoader.GetEquipSlot(Mod, "VictideBreastplate", EquipType.Body);

                // Legs
                if (Player.armor[2].type == ItemType<VictideGreaves>())
                    Player.legs = EquipLoader.GetEquipSlot(Mod, "VictideGreaves", EquipType.Legs);

                // Set Bonus
                if (Player.armor[0].type == ItemType<SnowRuffianMask>()
                && Player.armor[1].type == ItemType<SnowRuffianChestplate>()
                && Player.armor[2].type == ItemType<SnowRuffianGreaves>())
                    snowRuffianSet = true;
            }

            if (Player.Calamity().andromedaState == AndromedaPlayerState.LargeRobot ||
                Player.Calamity().andromedaState == AndromedaPlayerState.SpecialAttack)
            {
                Player.head = EquipLoader.GetEquipSlot(Mod, "HeadlessEquipTexture", EquipType.Head); // To make the head invisible on the map. The map was having a hissy fit because of hitbox changes.
            }
            else if (AresExoskeleton.ArmExists(Player))
                Player.body = EquipLoader.GetEquipSlot(Mod, "AresExoskeleton", EquipType.Body);
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


            if (snowRuffianSet)
            {
                Player.wings = EquipLoader.GetEquipSlot(Mod, "SnowRuffianMask", EquipType.Wings);
                bool falling = Player.gravDir == -1 ? Player.velocity.Y < 0.05f : Player.velocity.Y > 0.05f;
                if (Player.controlJump && falling)
                {
                    if (!Player.mount.Active)
                    {
                        Player.velocity.Y *= SnowRuffianMask.GlideFallSpeedMult;
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

            // Disable vanilla dashes during God Slayer or Speed Blaster dashes
            if (godSlayerDashHotKeyPressed || SpeedBlasterDashStarted)
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

            if (Player.ownedProjectileCounts[ProjectileType<GiantIbanRobotOfDoom>()] > 0)
                Player.yoraiz0rEye = 0;

            int totalMoonlightDyes = Player.dye.Count(dyeItem => dyeItem.type == ItemType<ProfanedMoonlightDye>());
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
            for (int j = 0; j < 1000; j++)
            {
                if (!Main.projectile[j].active || Main.projectile[j].owner != Player.whoAmI)
                    continue;

                Player.ownedProjectileCounts[Main.projectile[j].type]++;
            }
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
            if (Player.whoAmI == Main.myPlayer && Player.HeldItem.type == ItemType<TheAnomalysNanogun>())
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
                    int proj = Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ProjectileType<LeviathanBomb>(), 9999, 10f, Player.whoAmI);
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
            return true;
        }

        public override void ModifyNursePrice(NPC nurse, int health, bool removeDebuffs, ref int price)
        {
            // Seemlessly apply progression scaling on top of vanilla's scaling logic
            // In order to do this, we need to cancel out vanilla's currently active multiplier to account for possible non-linearity
            // Golem (vanilla): 200x    2 gold (per 100 HP or 1 debuff)
            // Moon Lord:       250x    2 gold 50 silver
            // Providence:      300x    3 gold
            // DoG:             400x    4 gold
            // Yharon:          500x    5 gold
            // Exo Mechs/SCal:  600x    6 gold

            if (price > 0)
            {
                if (CalamityWorld.death)
                    price *= 2;
                if (areThereAnyDamnBosses)
                    price *= 5;

                if (DownedBossSystem.downedExoMechs || DownedBossSystem.downedCalamitas)
                    price *= 600;
                else if (DownedBossSystem.downedYharon)
                    price *= 500;
                else if (DownedBossSystem.downedDoG)
                    price *= 400;
                else if (DownedBossSystem.downedProvidence)
                    price *= 300;
                else if (NPC.downedMoonlord)
                    price *= 250;
                else // If none of Calamity's scaling logic applies, do not do any calculations to cancel off vanilla's price multiplier
                    return;

                int vanillaPriceMult = 1;
                if (NPC.downedGolemBoss)
                    vanillaPriceMult = 200;
                else if (NPC.downedPlantBoss)
                    vanillaPriceMult = 150;
                else if (NPC.downedMechBossAny)
                    vanillaPriceMult = 100;
                else if (Main.hardMode)
                    vanillaPriceMult = 60;
                else if (NPC.downedBoss3 || NPC.downedQueenBee)
                    vanillaPriceMult = 25;
                else if (NPC.downedBoss2)
                    vanillaPriceMult = 10;
                else if (NPC.downedBoss1)
                    vanillaPriceMult = 3;
                price /= vanillaPriceMult;
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
            stealthStrike90Cost = false;

            // stealthAcceleration only resets if you don't have either of the accelerator accessories equipped
            if (!darkGodSheath && !eclipseMirror)
                stealthAcceleration = 1f;

            if (temporaryStealthTimer > 0)
                temporaryStealthTimer--;
            else
                temporaryStealthMax = 0.1f;
        }

        public void UpdateRogueStealth()
        {
            if (temporaryStealthTimer > 0)
            {
                if (rogueStealthMax < temporaryStealthMax)
                    rogueStealthMax = temporaryStealthMax;
                wearingRogueArmor = true;
            }

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
            Item it = Player.HeldItem;
            bool hasDamage = it.damage > 0;
            bool hasHitboxes = !it.noMelee || it.shoot > ProjectileID.None;
            bool isPickaxe = it.pick > 0;
            bool isAxe = it.axe > 0;
            bool isHammer = it.hammer > 0;
            bool isPlaced = it.createTile != -1;
            bool isChannelable = it.channel;
            bool hasNonWeaponFunction = isPickaxe || isAxe || isHammer || isPlaced || isChannelable;
            bool playerUsingWeapon = hasDamage && hasHitboxes && !hasNonWeaponFunction;

            // The Gem Tech armor's rogue crystal ensures that stealth is not consumed by non-rogue items. Forbidden Circlet does this for summon weapons
            if ((it.IsAir || (!it.CountsAsClass<RogueDamageClass>()) && GemTechSet && GemTechState.IsRedGemActive) || (it.CountsAsClass<SummonDamageClass>() && forbiddenCirclet))
                playerUsingWeapon = false;

            // Molten Amputator consumes stealth in a special way
            if (it.type == ItemType<MoltenAmputator>())
                playerUsingWeapon = false;

            // Shock Grenade consumes stealth in a special way
            if (it.type == ItemType<DoomsdayDevice>())
                playerUsingWeapon = false;

            // Elephant Killer consumes stealth in a special wa- hey maybe this is a sign this class needs some work huh
            if (it.type == ItemType<ElephantKiller>())
                playerUsingWeapon = false;

            if (it.type == ItemType<Spadefish>())
                playerUsingWeapon = false;

            // Animation check depends on whether the item is "clockwork", like Clockwork Assault Rifle.
            // "Clockwork" weapons can chain-fire multiple stealth strikes (really only 2 max) until you run out of stealth.
            bool animationCheck = it.useAnimation == it.useTime
                ? Player.itemAnimation == Player.itemAnimationMax - 1 // Standard weapon (first frame of use animation)
                : Player.itemTime == (int)(it.useTime / Player.GetTotalAttackSpeed<RogueDamageClass>()); // Clockwork weapon (first frame of any individual use event)

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
            Item it = !Main.HoverItem.IsAir ? Main.HoverItem : Player.HeldItem;

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
            int finalDawnProjCount = Player.ownedProjectileCounts[ProjectileType<FinalDawnProjectile>()] +
            Player.ownedProjectileCounts[ProjectileType<FinalDawnFireSlash>()] +
            Player.ownedProjectileCounts[ProjectileType<FinalDawnHorizontalSlash>()] +
            Player.ownedProjectileCounts[ProjectileType<FinalDawnThrow>()] +
            Player.ownedProjectileCounts[ProjectileType<FinalDawnThrow2>()];

            // If you are actively using an item, you cannot gain stealth.
            if (Player.itemAnimation > 0 || finalDawnProjCount > 0 || (Player.ownedProjectileCounts[ProjectileType<ElephantKillerThrown>()] > 0 || Player.HeldItem.type == ModContent.ItemType<ElephantKiller>()))
                return 0f;

            if (shadow)
            {
                stealthGenStandstill += ShadowPotion.StealthRegenBoost;
                stealthGenMoving += ShadowPotion.StealthRegenBoost;
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
            else if (stealthStrike90Cost)
                consumptionMult = 0.9f;
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
            else if (stealthStrike90Cost)
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
            var source = Player.GetSource_ItemUse(Player.HeldItem);
            if (Player.whoAmI == Main.myPlayer && !endoCooper && randAmt > 0 && Main.rand.NextBool(randAmt) && chaseable)
            {
                int spearsFired = 0;

                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (spearsFired == 2)
                        break;
                    if (p.owner == Player.whoAmI && p.friendly)
                    {
                        bool attack = p.type == ProjectileType<MiniGuardianAttack>();
                        if (attack)
                        {
                            int numSpears = profanedCrystalBuffs ? 12 : 6;
                            int dam = (int)(p.originalDamage * (profanedCrystalBuffs ? 1f : 0.25f));

                            for (int x = 0; x < numSpears; x++)
                            {
                                float angle = MathHelper.TwoPi / numSpears * x;
                                int proj = Projectile.NewProjectile(source, p.Center, angle.ToRotationVector2().RotatedBy(Math.Atan(-45f)) * 8f, ProjectileType<MiniGuardianSpear>(), dam, 0f, Player.whoAmI, pscState, 0f);
                                Main.projectile[proj].originalDamage = dam;
                            }
                            spearsFired++;
                        }
                    }
                }
            }
        }
        #endregion

        #region Misc Stuff
        private static int startMessageDisplayDelay = -1;

        // Triggers effects that must occur when the player enters the world. This sends a bunch of packets in multiplayer.
        // It also starts the speedrun timer if applicable.
        public override void OnEnterWorld()
        {
            if (CalamityClientConfig.Instance.StutterFix)
                WorldGen.SectionTileFrameWithCheck(0, 0, Main.maxTilesX, Main.maxTilesY);

            if (Main.netMode == NetmodeID.MultiplayerClient)
                EnterWorldSync();

            // Enabling the config while a player is loaded will show the timer immediately.
            // But it won't start running until you save and quit and re-enter a world.
            if (CalamityClientConfig.Instance.SpeedrunTimer)
                SpeedrunTimerSystem.Restart();

            bool showWikiMessage = CalamityClientConfig.Instance.WikiStatusMessage;
            bool showVCMMMessage = CalamityClientConfig.Instance.VCMMStatusMessage && !ExternalMods.VCMMAvailable;
            bool showStartupMessages = showWikiMessage || showVCMMMessage;

            // Set a random delay between 12 and 20 seconds. When this delay hits zero, startup messages display
            if (showStartupMessages)
                startMessageDisplayDelay = Main.rand.Next(CalamityUtils.SecondsToFrames(12), CalamityUtils.SecondsToFrames(20) + 1);
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
                        Player.KillMe(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.ManaConversionEnchant").ToNetworkText(Player.name)), 1000, -1);
                }

                for (int i = 0; i < 8; i++)
                {
                    Dust life = Dust.NewDustPerfect(Player.Top + Main.rand.NextVector2Circular(Player.width * 0.5f, 6f), DustID.RainbowMk2);
                    life.color = Color.Red;
                    life.velocity = -Vector2.UnitY.RotatedByRandom(0.48f) * Main.rand.NextFloat(3f, 4.4f);
                    life.scale = Main.rand.NextFloat(1.5f, 1.72f);
                    life.fadeIn = 0.7f;
                    life.noGravity = true;
                }
            }

            if (modPlayer.fleshTotem)
            {
                if (Main.myPlayer == Player.whoAmI)
                {
                    fleshTotemManaStorage += manaConsumed;
                    if (fleshTotemManaStorage >= FleshTotem.manaStorageMax)
                    {
                        fleshTotemManaStorage = FleshTotem.manaStorageMax;
                    }
                }
            }
        }
        #endregion

        #region Controls
        // These are used to entirely disable player directional inputs while being able to read them for other features
        public bool ShouldHideControls = false;
        public bool pressedRight = false;
        public bool pressedLeft = false;
        public bool pressedUp = false;
        public bool pressedDown = false;
        public override void SetControls()
        {
            pressedRight = Player.controlRight;
            pressedLeft = Player.controlLeft;
            pressedUp = Player.controlUp;
            pressedDown = Player.controlDown;
            if (ShouldHideControls)
            {
                Player.controlLeft = false;
                Player.controlUp = false;
                Player.controlDown = false;
                Player.controlRight = false;
            }
            ShouldHideControls = false;
        }
        #endregion
    }
}
