using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Balancing;
using CalamityMod.BiomeManagers;
using CalamityMod.Buffs;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.DataStructures;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Graphics.Metaballs;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Armor.PlagueReaper;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.Tools;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Deconstructors;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.DraedonLabThings;
using CalamityMod.NPCs.ExoMechs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses;
using CalamityMod.NPCs.VanillaNPCAIOverrides.RegularEnemies;
using CalamityMod.Packets;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Systems;
using CalamityMod.Systems.Collections;
using CalamityMod.Tiles.FurnitureAuric;
using CalamityMod.Tiles.Ores;
using CalamityMod.UI;
using CalamityMod.UI.DebuffSystem;
using CalamityMod.UI.VanillaBossBars;
using CalamityMod.Walls.DraedonStructures;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Utils;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.UI.Chat;
using Terraria.Utilities;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.NPCs
{
    public partial class CalamityGlobalNPC : GlobalNPC
    {
        #region Variables

        /// <summary> Data structure used for storing the damage reduction values of NPCs. </summary>
        public static SortedDictionary<int, float> DRValues { get; set; }

        /// <summary> Damage Reduction Value </summary>
        public float DR { get; set; } = 0f;

        /// <summary> If set to true, the NPC's damage reduction cannot be reduced via any means. </summary>
        public bool unbreakableDR = false;

        public int KillTime { get; set; } = 0;

        /// <summary>
        /// Controls the effectiveness of heat debuffs against this NPC.<br/>
        /// If true, they are vulnerable and debuffs are 200% effective. If false, they are resistant and debuffs are 50% effective. If null, they are neutral and suffer standard effects.
        /// </summary>
        public bool? VulnerableToHeat = null;
        /// <summary>
        /// Controls the effectiveness of cold debuffs against this NPC.<br/>
        /// If true, they are vulnerable and debuffs are 200% effective. If false, they are resistant and debuffs are 50% effective. If null, they are neutral and suffer standard effects.
        /// </summary>
        public bool? VulnerableToCold = null;
        /// <summary>
        /// Controls the effectiveness of sickness debuffs against this NPC.<br/>
        /// If true, they are vulnerable and debuffs are 200% effective. If false, they are resistant and debuffs are 50% effective. If null, they are neutral and suffer standard effects.
        /// </summary>
        public bool? VulnerableToSickness = null;
        /// <summary>
        /// Controls the effectiveness of electricity debuffs against this NPC.<br/>
        /// If true, they are vulnerable and debuffs are 200% effective. If false, they are resistant and debuffs are 50% effective. If null, they are neutral and suffer standard effects.
        /// </summary>
        public bool? VulnerableToElectricity = null;
        /// <summary>
        /// Controls the effectiveness of water debuffs against this NPC.<br/>
        /// If true, they are vulnerable and debuffs are 200% effective. If false, they are resistant and debuffs are 50% effective. If null, they are neutral and suffer standard effects.
        /// </summary>
        public bool? VulnerableToWater = null;

        public const float BaseDoTDamageMult = 1f;
        public const float VulnerableToDoTDamageMult = 2f;
        public const float VulnerableToDoTDamageMult_Worms_SlimeGod = 1.5f;
        public const float ResistantToDoTDamageMult = 0.5f;

        public StatModifier TypelessDebuffMultiplier = new StatModifier();
        public StatModifier HeatDebuffMultiplier = new StatModifier();
        public StatModifier ColdDebuffMultiplier = new StatModifier();
        public StatModifier SicknessDebuffMultiplier = new StatModifier();
        public StatModifier WaterDebuffMultiplier = new StatModifier();
        public StatModifier ElectricDebuffMultiplier = new StatModifier();

        // These are all recalculated constantly, while the regular ones are recalulated only on hit
        public StatModifier ActiveTypelessDebuffMultiplier = new StatModifier();
        public StatModifier ActiveHeatDebuffMultiplier = new StatModifier();
        public StatModifier ActiveColdDebuffMultiplier = new StatModifier();
        public StatModifier ActiveSicknessDebuffMultiplier = new StatModifier();
        public StatModifier ActiveWaterDebuffMultiplier = new StatModifier();
        public StatModifier ActiveElectricDebuffMultiplier = new StatModifier();

        // Cold debuff effects
        public bool IncreasedColdEffects_EskimoSet = false;
        public bool IncreasedColdEffects_CryoStone = false;
        public bool IncreasedColdEffects_FrozenCube = false;

        // Electric effects
        public bool IncreasedElectricityEffects_Unused = false;

        // Heat debuff effects
        public bool IncreasedHeatEffects_Fireball = false;
        public int IncreasedHeatEffects_FireBoots = 0;

        // Toxic Heart effect
        public bool IncreasedSicknessEffects_ToxicHeart = false;

        // Amulets effects
        public bool IncreasedWaterEffects_Amulet1 = false;
        public bool IncreasedWaterEffects_Amulet2 = false;

        // Sickness and Water debuff effects
        public bool IncreasedSicknessAndWaterEffects_EvergreenGin = false;
        public bool IncreasedSicknessAndWaterEffects_CorrosiveSpine = false;

        // Universal debuff effects
        public bool IncreasedDebuffEffects_Amalgam = false;

        /// <summary> Constant variable representing the grace period, in frames, in which a boss can remain outside of its native biome before enraging. </summary>
        public const int biomeEnrageTimerMax = 300;

        /// <summary>
        /// Variable for worm bosses used to prevent them from moving too fast upon swapping phases while far away from their target.<br/>
        /// Currently only used by DoG.
        /// </summary>
        public float velocityPriorToPhaseSwap = 0f;
        public const float velocityPriorToPhaseSwapIncrement = 0.1f;

        /// <summary> Allows hostile NPCs to deal defense damage to the player, used mostly for hard-hitting bosses. </summary>
        public bool canBreakPlayerDefense = false;

        /// <summary> Set this value to reduce target defense by a flat amount. </summary>
        public int miscDefenseLoss = 0;

        /// <summary> If true, enemy will not drop any items. </summary>
        public bool preventDrops = false;

        /// <summary>
        /// Constant representing a distance of 200 tiles in pixel measurement.<br/>
        /// Used by bosses to increase their velocity in order to catch up to their target.
        /// </summary>
        public const float CatchUpDistance200Tiles = 3200f;
        /// <summary>
        /// Constant representing a distance of 350 tiles in pixel measurement.<br/>
        /// Used by bosses to increase their velocity in order to catch up to their target.
        /// </summary>
        public const float CatchUpDistance350Tiles = 5600f;
        /// <summary>
        /// Constant representing a distance of 400 tiles in pixel measurement.<br/>
        /// Used as a cap on the distance away from a boss a player can be inflicted with Boss Effects.
        /// </summary>
        private const float BossZenDistance = 6400f;

        /// <summary> Constant multiplier used to decrease the health and/or damage of pre-Hardmode Desert enemies. </summary>
        private const double DesertEnemyStatMultiplier = 0.75;

        /// <summary> Constant multiplier used for decreasing the health and damage of mechanical bosses if the Early Hardmode Progression Rework config is enabled. </summary>
        public const double EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Classic = 0.8;
        /// <summary> <inheritdoc cref="EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Classic" /> </summary>
        public const double EarlyHardmodeProgressionReworkSecondMechStatMultiplier_Classic = 0.9;
        /// <summary> <inheritdoc cref="EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Classic" /> </summary>
        public const double EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Expert = 0.9;
        /// <summary> <inheritdoc cref="EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Classic" /> </summary>
        public const double EarlyHardmodeProgressionReworkSecondMechStatMultiplier_Expert = 0.95;

        /// <summary> Constant multiplier used to increase coin drops in Classic Mode. </summary>
        private const double NPCValueMultiplier_ClassicCalamity = 1.5;
        /// <summary> Constant multiplier used to decrease coin drops in Expert Mode. </summary>
        private const double NPCValueMultiplier_ExpertVanilla = 2.5;
        /// <summary> <inheritdoc cref="NPCValueMultiplier_ExpertVanilla"/> </summary>
        private const double NPCValueMultiplier_ExpertCalamity = 1.5;

        // Dash damage immunity timer
        public const int maxPlayerImmunities = Main.maxPlayers + 1;
        public int[] dashImmunityTime = new int[maxPlayerImmunities];

        /// <summary>
        /// If set to false, prevents this NPC from allowing Rage to be generated by nearby players, regardless of other factors.<br/>
        /// Defaults to true.
        /// </summary>
        public bool ProvidesProximityRage = true;

        // NewAI
        // TODO: This should be deprecated at some point.
        internal const int maxAIMod = 4;
        public float[] newAI = new float[maxAIMod];
        public int killTimeTimer = 0;

        /// <summary> If set to true, the Boss Health Bar for this NPC will count the total health of all individual segments using worm segment logic. </summary>
        public bool SplittingWorm = false;
        /// <summary> If set to true, allows this NPC to draw a Boss Health Bar, regardless of other factors. </summary>
        public bool CanHaveBossHealthBar = false;
        /// <summary> If set to true, allows for manually disabling this NPC's Boss Health Bar, even if they are still active. </summary>
        public bool ShouldCloseHPBar = false;

        #region Debuffs
        public bool vaporfied = false;
        public bool timeDistortion = false;
        public bool frozen = false;
        public bool galvanicCorrosion = false;
        public bool temporalSadness = false;
        public bool eutrophication = false;
        public bool webbed = false;
        public bool electrified = false;
        public bool pearlAura = false;

        public float manaBurn = 0f;
        public float manaBurnPeak = 0f;
        public float playerManaBurnIntensity = 0f;
        public bool burningBlood = false;
        public bool brainRot = false;
        public bool heavyBleeding = false;
        public bool laceration = false;
        public bool elementalMix = false;
        public bool markedForDeath = false;
        public bool absorberAffliction = false;
        public bool irradiated = false;
        public double irradiatedContactBoost = 1.5;
        public bool bane = false;
        public float baneVisual = 0;
        public bool brimstoneFlames = false;
        public bool demonicFlames = false;
        public int demonicFlamesBonusDamage = 0;
        public int demonicFlamesClearTimer = 0;
        public bool holyFlames = false;
        public bool plague = false;
        public bool armorCrunch = false;
        public bool crumble = false;

        public int antlionCloudDebuffTimer = 0;
        public bool scionsCurioEffected = false;
        public bool abaddonEffected = false;
        public bool apollyonEffected = false;
        public int warbannerBurnTime = 0; // Determines the rate that the enemy is damaged
        public int warbannerBurnTimer = 0; // The duration of the debuff
        public int warbannerBurnStacks = 0; // The stacks increase how fast the debuff hits
        public int warbannerBurnDamage = 0; // Damage of the hits based on player's damage
        public Vector2 warbannerBurnDirection;
        public float warbannerBurnIntensity = 0;
        public bool warbannerBurnMarked = false;
        public bool warbannerBurnHideEffects = false;
        /// <summary> Constant variable representing the delay, in frames, before Verium Bolt's extra damage applies. </summary>
        public const int veriumDoomTime = 90;
        public int veriumDoomTimer = 0;
        public int veriumDoomStacks = 0;
        public bool veriumDoomMarked = false;

        public bool laserBurnMarked = false;
        /// <summary>
        /// The type of laser burn that this NPC is inflicted with.<br/>
        /// When set to 1, applies all accrued damage in a single hit. When set to 2, deals constant flat damage + extra flat damage from stacks.
        /// </summary>
        public int laserBurnType = 0;
        public int laserBurnDamage = 0; // Only used if laser burn type is 1
        public const int laserBurnTime = 300;
        public int laserBurnTimer = 0;
        public int laserBurnStacks = 0;

        public bool hyperiusMarked = false;
        public int hyperiusDamage = 0;
        public static int hyperiusOverflowTime = 100;
        public int hyperiusOverflowTimer = hyperiusOverflowTime;
        /// <summary> Constant variable representing the % of max health Hyperius Bullet's damage stacks must reach before they start to bleed. </summary>
        public const float HyperiusLifePercentThreshold = 0.10f;
        public int hyperiusFxTimer = 0;

        public int glaiveShredTimer = 0;
        public int blazingStarShredTimer = 0;

        /// <summary>
        /// Tracks the strength of Calamity's cursor effect; increments by 2 on every frame.<br/>
        /// If this value reaches <see cref="cursorFocusMax"/>, the enemy is afflicted with True Vulnerability Hex.
        /// </summary>
        public int cursorFocus = 0;
        public const int cursorFocusMax = 300;
        public int demonSwordImpales = 0;
        public int impalePacketTimer = 0;

        /// <summary>
        /// If set to true, prevents this NPC from dealing contact damage.<br/>
        /// Used by Septic Skewer's execution attack.
        /// </summary>
        public bool pacified = false;

        public float coinDropMult = 1;

        // Soma Prime Shred deals damage with DirectStrikes instead of with direct debuff damage
        // It also stacks, scales with ranged damage, and can crit, meaning it needs to know who applied it most recently
        /// <summary> Tracks how many stacks of the Shred debuff this NPC is inflicted with. </summary>
        public int somaShredStacks = 0;
        /// <summary> Tracks the index of the player that inflicted this NPC with Shred, for the purpose of scaling damage. </summary>
        public int somaShredApplicator = -1;
        /// <summary> Counter used for removing stacks of Shred. The number of stacks is subtracted every frame, and when it hits zero, it is reset and one stack is removed. </summary>
        public int somaShredFalloff = Shred.StackFalloffFrames;

        public bool crushDepth = false;
        public bool riptide = false;
        public bool hadopelagicPressure = false;
        public bool godSlayerInferno = false;
        public bool dragonFire = false;
        public bool vermillionFlux = false;
        public bool auricRebuke = false;
        public bool staticDischarge = false;
        public bool miracleBlight = false;
        public bool astralInfection = false;
        public bool whisperingDeath = false;
        public bool nightwither = false;
        /// <summary> If greater than 0, this NPC has been "shocked" by Ilmeris' Spark's on hurt effect. </summary>
        public int shocked = 0;
        public bool voidfrost = false;
        public bool shellfishStaffDebuff = false;
        public bool snapClamDebuff = false;
        public bool sulphurPoison = false;
        /// <summary> If greater than 0, makes this NPC constantly spawn heart gores. </summary>
        public int ladHearts = 0;
        public bool relicOfResilienceWeakness = false;
        public bool sagePoison = false;
        public bool vulnerabilityHex = false;
        public bool trueVulnerabilityHex = false;
        public bool banishingFire = false;
        public bool wither = false;
        public bool windChilled = false;
        public float windChilledMult = 1;
        /// <summary>
        /// If greater than 0, this enemy will appear to disintegrate into ash when killed.<br/>
        /// Used by Rancor's laser beam.
        /// </summary>
        public int ashesOnDeath = 0;
        #endregion

        // whoAmI Variables
        public static int[] bobbitWormBottom = new int[5];
        public static int hiveMind = -1;
        public static int perfHive = -1;
        public static int slimeGodPurple = -1;
        public static int slimeGodRed = -1;
        public static int slimeGod = -1;
        public static int laserEye = -1;
        public static int fireEye = -1;
        public static int primeLaser = -1;
        public static int primeCannon = -1;
        public static int primeVice = -1;
        public static int primeSaw = -1;
        public static int aquaticScourge = -1;
        public static int brimstoneElemental = -1;
        public static int cataclysm = -1;
        public static int catastrophe = -1;
        public static int calamitas = -1;
        public static int LeviAndAna = -1;
        public static int leviathan = -1;
        public static int siren = -1;
        public static int astrumAureus = -1;
        public static int scavenger = -1;
        public static int energyFlame = -1;
        public static int doughnutBoss = -1;
        public static int doughnutBossDefender = -1;
        public static int doughnutBossHealer = -1;
        public static int holyBossAttacker = -1;
        public static int holyBossDefender = -1;
        public static int holyBossHealer = -1;
        public static int holyBoss = -1;
        public static int voidBoss = -1;
        public static int signus = -1;
        public static int ghostBossClone = -1;
        public static int ghostBoss = -1;
        public static int DoGHead = -1;
        public static int DoGP2 = -1;
        public static int yharon = -1;
        public static int yharonP2 = -1;
        public static int SCalCataclysm = -1;
        public static int SCalCatastrophe = -1;
        public static int SCal = -1;
        public static int SCalWorm = -1;
        public static int SCalGrief = -1;
        public static int SCalLament = -1;
        public static int SCalEpiphany = -1;
        public static int SCalAcceptance = -1;
        public static int draedon = -1;
        public static int draedonAmbience = -1;
        public static int draedonExoMechWorm = -1;
        public static int draedonExoMechTwinRed = -1;
        public static int draedonExoMechTwinGreen = -1;
        public static int draedonExoMechPrime = -1;
        public static int draedonExoMechPrimePlasmaCannon = -1;
        public static int adultEidolonWyrmHead = -1;

        // Drawing variables.
        public FireParticleSet VulnerabilityHexFireDrawer = null;
        public FireParticleSet ManaBurnFireDrawer = null;

        /// <summary>
        /// Boss Enrage variable for use with the boss health UI.<br/>
        /// The logic behind this is as follows:
        /// <para>1 - For special cases with super-enrages (specifically Yharon/SCal with their arenas), go solely based on whether that enrage is active. That information is most important to the player.</para>
        /// <para>2 - Check if the Demonshade enrage is active. If it is, register this as true. If not, go to step 3.</para>
        /// <para>3 - Check if a specific enrage condition (such as Duke Fishron's Ocean check) is met. If it is, and Boss Rush is not active, set this to true. If not, go to step 4.</para>
        /// <para>4 - Check if Boss Rush isn't active. If so, set this to true.</para>
        /// </summary>
        public bool CurrentlyEnraged;

        /// <summary>
        /// Increased defense or DR variable for use with the boss health UI.<br/>
        /// The logic behind this is as follows:
        /// <para>1 - When bosses are transitioning phases they gain a massive DR increase.</para>
        /// <para>2 - When bosses are using certain attacks that make them particularly vulnerable they gain a massive DR or defense increase.</para>
        /// While either of these are occuring, this variable should be set to true.
        /// </summary>
        public bool CurrentlyIncreasingDefenseOrDR;

        /// <summary> If set to true, this NPC will be ignored by Boss Rush's whitelist and will always be allowed to exist. </summary>
        public bool DoesNotDisappearInBossRush;

        /// <summary> Variable used for Gladiator's Locket's on-kill effect to ensure it only triggers once per kill. </summary>
        public bool gladiatorOnKill = true;
        /// <summary> Cooldown variable for Unstable Granite Core's arc zap effect. </summary>
        public int arcZapCooldown = 0;

        /// <summary> Timer for animating worm enemies in the bestiary. </summary>
        public float bestiaryWormTimer = 0;
        #endregion

        #region Instance Per Entity and TML 1.4 Cloning
        public override bool InstancePerEntity => true;

        // Ozzatron 25APR2022: This function was required by TML 1.4's new clone behavior,
        // which broke every custom NPC in the game simultaneously when it was introduced.
        // It manually copies everything because I don't trust the base clone behavior after seeing the insane bugs.
        // Considering the continuing revisions to Entity cloning, it's possible that this is no longer needed.
        // Don't risk it and don't remove this code unless it's clear that it is causing problems.
        //
        // ANY TIME YOU ADD A VARIABLE TO CalamityGlobalNPC, IT MUST BE COPIED IN THIS FUNCTION.
        public override GlobalNPC Clone(NPC npc, NPC npcClone)
        {
            CalamityGlobalNPC myClone = (CalamityGlobalNPC)base.Clone(npc, npcClone);

            myClone.DR = DR;
            myClone.unbreakableDR = unbreakableDR;
            myClone.KillTime = KillTime;

            myClone.VulnerableToHeat = VulnerableToHeat;
            myClone.VulnerableToCold = VulnerableToCold;
            myClone.VulnerableToSickness = VulnerableToSickness;
            myClone.VulnerableToElectricity = VulnerableToElectricity;
            myClone.VulnerableToWater = VulnerableToWater;

            myClone.IncreasedColdEffects_EskimoSet = IncreasedColdEffects_EskimoSet;
            myClone.IncreasedColdEffects_CryoStone = IncreasedColdEffects_CryoStone;
            myClone.IncreasedColdEffects_FrozenCube = IncreasedColdEffects_FrozenCube;
            myClone.IncreasedElectricityEffects_Unused = IncreasedElectricityEffects_Unused;
            myClone.IncreasedHeatEffects_Fireball = IncreasedHeatEffects_Fireball;
            myClone.IncreasedHeatEffects_FireBoots = IncreasedHeatEffects_FireBoots;
            myClone.IncreasedSicknessEffects_ToxicHeart = IncreasedSicknessEffects_ToxicHeart;
            myClone.IncreasedWaterEffects_Amulet1 = IncreasedWaterEffects_Amulet1;
            myClone.IncreasedWaterEffects_Amulet2 = IncreasedWaterEffects_Amulet2;
            myClone.IncreasedSicknessAndWaterEffects_CorrosiveSpine = IncreasedSicknessAndWaterEffects_CorrosiveSpine;
            myClone.IncreasedSicknessAndWaterEffects_EvergreenGin = IncreasedSicknessAndWaterEffects_EvergreenGin;
            myClone.IncreasedDebuffEffects_Amalgam = IncreasedDebuffEffects_Amalgam;

            myClone.velocityPriorToPhaseSwap = velocityPriorToPhaseSwap;

            myClone.canBreakPlayerDefense = canBreakPlayerDefense;

            myClone.miscDefenseLoss = miscDefenseLoss;

            myClone.preventDrops = preventDrops;

            myClone.dashImmunityTime = new int[maxPlayerImmunities];
            for (int i = 0; i < maxPlayerImmunities; ++i)
                myClone.dashImmunityTime[i] = dashImmunityTime[i];

            myClone.ProvidesProximityRage = ProvidesProximityRage;

            myClone.newAI = new float[maxAIMod];
            for (int i = 0; i < maxAIMod; ++i)
                myClone.newAI[i] = newAI[i];
            myClone.killTimeTimer = killTimeTimer;

            myClone.SplittingWorm = SplittingWorm;
            myClone.CanHaveBossHealthBar = CanHaveBossHealthBar;
            myClone.ShouldCloseHPBar = ShouldCloseHPBar;

            myClone.vaporfied = vaporfied;
            myClone.timeDistortion = timeDistortion;
            myClone.frozen = frozen;
            myClone.galvanicCorrosion = galvanicCorrosion;
            myClone.temporalSadness = temporalSadness;
            myClone.eutrophication = eutrophication;
            myClone.webbed = webbed;
            myClone.electrified = electrified;
            myClone.pearlAura = pearlAura;
            myClone.burningBlood = burningBlood;
            myClone.brainRot = brainRot;
            myClone.heavyBleeding = heavyBleeding;
            myClone.laceration = laceration;
            myClone.elementalMix = elementalMix;
            myClone.markedForDeath = markedForDeath;
            myClone.absorberAffliction = absorberAffliction;
            myClone.irradiated = irradiated;
            myClone.irradiatedContactBoost = irradiatedContactBoost;
            myClone.bane = bane;
            myClone.baneVisual = baneVisual;
            myClone.brimstoneFlames = brimstoneFlames;
            myClone.demonicFlames = demonicFlames;
            myClone.demonicFlamesBonusDamage = demonicFlamesBonusDamage;
            myClone.demonicFlamesClearTimer = demonicFlamesClearTimer;
            myClone.holyFlames = holyFlames;
            myClone.plague = plague;
            myClone.armorCrunch = armorCrunch;
            myClone.crumble = crumble;

            myClone.antlionCloudDebuffTimer = antlionCloudDebuffTimer;
            myClone.scionsCurioEffected = scionsCurioEffected;
            myClone.abaddonEffected = abaddonEffected;
            myClone.apollyonEffected = apollyonEffected;
            myClone.warbannerBurnTime = warbannerBurnTime;
            myClone.warbannerBurnTimer = warbannerBurnTimer;
            myClone.warbannerBurnStacks = warbannerBurnStacks;
            myClone.warbannerBurnDamage = warbannerBurnDamage;
            myClone.warbannerBurnDirection = warbannerBurnDirection;
            myClone.warbannerBurnIntensity = warbannerBurnIntensity;
            myClone.warbannerBurnMarked = warbannerBurnMarked;
            myClone.warbannerBurnHideEffects = warbannerBurnHideEffects;
            myClone.veriumDoomTimer = veriumDoomTimer;
            myClone.veriumDoomStacks = veriumDoomStacks;
            myClone.veriumDoomMarked = veriumDoomMarked;
            myClone.laserBurnDamage = laserBurnDamage;
            myClone.laserBurnMarked = laserBurnMarked;
            myClone.laserBurnStacks = laserBurnStacks;
            myClone.laserBurnTimer = laserBurnTimer;
            myClone.laserBurnType = laserBurnType;
            myClone.hyperiusDamage = hyperiusDamage;
            myClone.hyperiusMarked = hyperiusMarked;
            myClone.hyperiusOverflowTimer = hyperiusOverflowTimer;
            myClone.hyperiusFxTimer = hyperiusFxTimer;
            myClone.cursorFocus = cursorFocus;
            myClone.demonSwordImpales = demonSwordImpales;
            myClone.impalePacketTimer = impalePacketTimer;

            myClone.pacified = pacified;

            myClone.coinDropMult = coinDropMult;

            myClone.somaShredStacks = somaShredStacks;
            myClone.somaShredApplicator = somaShredApplicator;
            myClone.somaShredFalloff = somaShredFalloff;

            myClone.crushDepth = crushDepth;
            myClone.riptide = riptide;
            myClone.hadopelagicPressure = hadopelagicPressure;
            myClone.godSlayerInferno = godSlayerInferno;
            myClone.miracleBlight = miracleBlight;
            myClone.dragonFire = dragonFire;
            myClone.vermillionFlux = vermillionFlux;
            myClone.auricRebuke = auricRebuke;
            myClone.staticDischarge = staticDischarge;
            myClone.astralInfection = astralInfection;
            myClone.whisperingDeath = whisperingDeath;
            myClone.nightwither = nightwither;
            myClone.shocked = shocked;
            myClone.voidfrost = voidfrost;
            myClone.shellfishStaffDebuff = shellfishStaffDebuff;
            myClone.snapClamDebuff = snapClamDebuff;
            myClone.sulphurPoison = sulphurPoison;
            myClone.ladHearts = ladHearts;
            myClone.relicOfResilienceWeakness = relicOfResilienceWeakness;
            myClone.sagePoison = sagePoison;
            myClone.vulnerabilityHex = vulnerabilityHex;
            myClone.trueVulnerabilityHex = trueVulnerabilityHex;
            myClone.banishingFire = banishingFire;
            myClone.wither = wither;
            myClone.windChilled = windChilled;
            myClone.ashesOnDeath = ashesOnDeath;

            // This gets set up as needed.
            myClone.VulnerabilityHexFireDrawer = null;
            myClone.ManaBurnFireDrawer = null;

            myClone.CurrentlyEnraged = CurrentlyEnraged;

            myClone.CurrentlyIncreasingDefenseOrDR = CurrentlyIncreasingDefenseOrDR;

            myClone.DoesNotDisappearInBossRush = DoesNotDisappearInBossRush;

            return myClone;
        }
        #endregion

        #region Reset Effects
        public override void ResetEffects(NPC npc)
        {
            void ResetSavedIndex(ref int type, int type1, int type2 = -1)
            {
                if (type >= 0)
                {
                    if (!Main.npc[type].active)
                    {
                        type = -1;
                    }
                    else if (type2 == -1)
                    {
                        if (Main.npc[type].type != type1)
                            type = -1;
                    }
                    else
                    {
                        if (Main.npc[type].type != type1 && Main.npc[type].type != type2)
                            type = -1;
                    }
                }
            }

            for (int i = 0; i < bobbitWormBottom.Length; i++)
                ResetSavedIndex(ref bobbitWormBottom[i], NPCType<BobbitWormSegment>());

            ResetSavedIndex(ref hiveMind, NPCType<HiveMind.HiveMind>());
            ResetSavedIndex(ref perfHive, NPCType<PerforatorHive>());
            ResetSavedIndex(ref slimeGodPurple, NPCType<EbonianPaladin>(), NPCType<SplitEbonianPaladin>());
            ResetSavedIndex(ref slimeGodRed, NPCType<CrimulanPaladin>(), NPCType<SplitCrimulanPaladin>());
            ResetSavedIndex(ref slimeGod, NPCType<SlimeGodCore>());
            ResetSavedIndex(ref laserEye, NPCID.Retinazer);
            ResetSavedIndex(ref fireEye, NPCID.Spazmatism);
            ResetSavedIndex(ref primeLaser, NPCID.PrimeLaser);
            ResetSavedIndex(ref primeCannon, NPCID.PrimeCannon);
            ResetSavedIndex(ref primeVice, NPCID.PrimeVice);
            ResetSavedIndex(ref primeSaw, NPCID.PrimeSaw);
            ResetSavedIndex(ref aquaticScourge, NPCType<AquaticScourgeHead>());
            ResetSavedIndex(ref brimstoneElemental, NPCType<BrimstoneElemental.BrimstoneElemental>());
            ResetSavedIndex(ref cataclysm, NPCType<Cataclysm>());
            ResetSavedIndex(ref catastrophe, NPCType<Catastrophe>());
            ResetSavedIndex(ref calamitas, NPCType<CalamitasClone>());
            ResetSavedIndex(ref LeviAndAna, NPCType<Leviathan.Leviathan>(), NPCType<Anahita>());
            ResetSavedIndex(ref leviathan, NPCType<Leviathan.Leviathan>());
            ResetSavedIndex(ref siren, NPCType<Anahita>());
            ResetSavedIndex(ref astrumAureus, NPCType<AstrumAureus.AstrumAureus>());
            ResetSavedIndex(ref scavenger, NPCType<RavagerBody>());
            ResetSavedIndex(ref energyFlame, NPCType<ProfanedEnergyBody>());
            ResetSavedIndex(ref doughnutBoss, NPCType<ProfanedGuardianCommander>());
            ResetSavedIndex(ref doughnutBossDefender, NPCType<ProfanedGuardianDefender>());
            ResetSavedIndex(ref doughnutBossHealer, NPCType<ProfanedGuardianHealer>());
            ResetSavedIndex(ref holyBossAttacker, NPCType<ProvSpawnOffense>());
            ResetSavedIndex(ref holyBossDefender, NPCType<ProvSpawnDefense>());
            ResetSavedIndex(ref holyBossHealer, NPCType<ProvSpawnHealer>());
            ResetSavedIndex(ref holyBoss, NPCType<Providence.Providence>());
            ResetSavedIndex(ref voidBoss, NPCType<CeaselessVoid.CeaselessVoid>());
            ResetSavedIndex(ref signus, NPCType<Signus.Signus>());
            ResetSavedIndex(ref ghostBossClone, NPCType<PolterPhantom>());
            ResetSavedIndex(ref ghostBoss, NPCType<Polterghast.Polterghast>());
            ResetSavedIndex(ref DoGHead, NPCType<DevourerofGodsHead>());
            ResetSavedIndex(ref DoGP2, NPCType<DevourerofGodsHead>());
            ResetSavedIndex(ref yharon, NPCType<Yharon.Yharon>());
            ResetSavedIndex(ref yharonP2, NPCType<Yharon.Yharon>());
            ResetSavedIndex(ref SCalCataclysm, NPCType<SupremeCataclysm>());
            ResetSavedIndex(ref SCalCatastrophe, NPCType<SupremeCatastrophe>());
            ResetSavedIndex(ref SCal, NPCType<SupremeCalamitas.SupremeCalamitas>());
            ResetSavedIndex(ref SCalGrief, NPCType<SupremeCalamitas.SupremeCalamitas>());
            ResetSavedIndex(ref SCalLament, NPCType<SupremeCalamitas.SupremeCalamitas>());
            ResetSavedIndex(ref SCalEpiphany, NPCType<SupremeCalamitas.SupremeCalamitas>());
            ResetSavedIndex(ref SCalAcceptance, NPCType<SupremeCalamitas.SupremeCalamitas>());
            ResetSavedIndex(ref SCalWorm, NPCType<SepulcherHead>());

            ResetSavedIndex(ref draedon, NPCType<Draedon>());
            ResetSavedIndex(ref draedonAmbience, NPCType<Draedon>());
            ResetSavedIndex(ref draedonExoMechWorm, NPCType<ThanatosHead>());
            ResetSavedIndex(ref draedonExoMechTwinRed, NPCType<Artemis>());
            ResetSavedIndex(ref draedonExoMechTwinGreen, NPCType<Apollo>());
            ResetSavedIndex(ref draedonExoMechPrime, NPCType<AresBody>());
            ResetSavedIndex(ref draedonExoMechPrimePlasmaCannon, NPCType<AresPlasmaFlamethrower>());

            ResetSavedIndex(ref adultEidolonWyrmHead, NPCType<PrimordialWyrmHead>());

            // Reset the enraged state every frame. The expectation is that bosses will continuously set it back to true if necessary.
            CurrentlyEnraged = false;
            CurrentlyIncreasingDefenseOrDR = false;
            CanHaveBossHealthBar = false;
            ShouldCloseHPBar = false;
            if (arcZapCooldown > 0) { arcZapCooldown--; }

            //Debuff Bool clearing.
            // Doze 2jun2025 - Moved here from PostAI so drawing can read the bools.
            timeDistortion = false;
            galvanicCorrosion = false;
            frozen = false;
            temporalSadness = false;
            eutrophication = false;
            webbed = false;
            vaporfied = false;
            electrified = false;
            pearlAura = false;
            burningBlood = false;
            brainRot = false;
            heavyBleeding = false;
            laceration = false;
            elementalMix = false;
            if (!trueVulnerabilityHex && !vulnerabilityHex)
            {
                cursorFocus = 0;
            }
            trueVulnerabilityHex = false;
            vulnerabilityHex = false;
            markedForDeath = false;
            absorberAffliction = false;
            irradiated = false;
            if (scionsCurioEffected)
                irradiatedContactBoost = 2f;
            bane = false;
            brimstoneFlames = false;
            demonicFlames = false;
            holyFlames = false;
            plague = false;
            // Soma Prime's Shred stacks have a unique falloff mechanic in the debuff's own file.
            armorCrunch = false;
            crumble = false;
            crushDepth = false;
            hadopelagicPressure = false;
            riptide = false;
            godSlayerInferno = false;
            dragonFire = false;
            vermillionFlux = false;
            auricRebuke = false;
            staticDischarge = false;
            miracleBlight = false;
            astralInfection = false;
            whisperingDeath = false;
            nightwither = false;
            if (shocked > 0)
                shocked--;
            voidfrost = false;
            shellfishStaffDebuff = false;
            snapClamDebuff = false;
            sulphurPoison = false;
            sagePoison = false;
            if (ladHearts > 0)
                ladHearts--;
            banishingFire = false;
            wither = false;
            windChilled = false;
            if (ashesOnDeath > 0)
                ashesOnDeath--;

            if (antlionCloudDebuffTimer > 0)
                antlionCloudDebuffTimer--;
            if (cursorFocus > 0 && cursorFocus < cursorFocusMax)
                cursorFocus--;
            relicOfResilienceWeakness = false;
        }
        #endregion

        #region Life Regen
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (npc.defDamage > 0 && !npc.boss && !npc.friendly && !npc.dontTakeDamage && BiomeTileCounterSystem.SulphurTiles > 30 &&
                !npc.buffImmune[BuffID.Poisoned] && !npc.buffImmune[BuffType<CrushDepth>()])
            {
                if (npc.wet)
                    npc.AddBuff(BuffID.Poisoned, 2);

                if (Main.raining)
                    npc.AddBuff(BuffType<Irradiated>(), 2);
            }

            // Lionfish and Jaws of Oblivion debuff stacking
            if (npc.venom)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                int projectileCount = 0;
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if ((p.type == ProjectileType<LionfishProj>() || p.type == ProjectileType<JawsProjectile>()) &&
                        p.ai[0] == 1f && p.ai[1] == npc.whoAmI)
                    {
                        projectileCount++;
                    }
                }

                if (projectileCount > 0)
                {
                    npc.lifeRegen -= projectileCount * (int)(DebuffData.AcidVenom.EnemyLostRegen / 2);

                    if (damage < projectileCount * 6)
                        damage = projectileCount * 6;
                }
            }

            // Debuff vulnerabilities and resistances.
            // Damage multiplier calcs.
            // Worms that are vulnerable to debuffs and Slime God slimes take reduced damage from vulnerabilities.
            #region Debuff System Multiplier Calculations
            bool wormBoss = CalamityNPCTypeSets.DesertScourge.Contains(npc.type) || CalamityNPCTypeSets.EaterOfWorlds.Contains(npc.type) || CalamityNPCTypeSets.Perforators.Contains(npc.type) ||
                CalamityNPCTypeSets.AquaticScourge.Contains(npc.type) || CalamityNPCTypeSets.AstrumDeus.Contains(npc.type) || CalamityNPCTypeSets.StormWeaver.Contains(npc.type);
            bool slimeGod = CalamityNPCTypeSets.SlimeGod.Contains(npc.type);

            ActiveHeatDebuffMultiplier = HeatDebuffMultiplier;
            ActiveColdDebuffMultiplier = ColdDebuffMultiplier;
            ActiveSicknessDebuffMultiplier = SicknessDebuffMultiplier;
            ActiveElectricDebuffMultiplier = ElectricDebuffMultiplier;
            ActiveWaterDebuffMultiplier = WaterDebuffMultiplier;
            ActiveTypelessDebuffMultiplier = TypelessDebuffMultiplier;

            if (irradiated)
            {
                float irradiatedBoost = scionsCurioEffected ? 1.75f : 1f;
                ActiveSicknessDebuffMultiplier += irradiatedBoost;
            }

            if (npc.drippingSlime || npc.drippingSparkleSlime)
            {
                ActiveHeatDebuffMultiplier += 1;
            }

            if (npc.wet || npc.honeyWet || npc.lavaWet || npc.dripping)
            {
                ActiveElectricDebuffMultiplier += 1;
            }

            if (npc.wet || npc.honeyWet || npc.dripping)
            {
                ActiveColdDebuffMultiplier += 1;
                ActiveHeatDebuffMultiplier -= 0.5f;
            }
            if (npc.HasBuff(ModContent.BuffType<WindChilled>()))
            {
                ActiveWaterDebuffMultiplier += 0.5f;
            }
            if (npc.buffType.Any(i => CalamityBuffSets.DebuffDataset[i] is not null && CalamityBuffSets.DebuffDataset[i].WaterDebuffScaling > 0) || npc.wet || npc.honeyWet || npc.dripping)
            {
                windChilledMult = 1.5f;
            }
            if (VulnerableToHeat.HasValue)
            {
                if (VulnerableToHeat.Value)
                    ActiveHeatDebuffMultiplier *= wormBoss ? VulnerableToDoTDamageMult_Worms_SlimeGod : VulnerableToDoTDamageMult;
                else
                    ActiveHeatDebuffMultiplier *= ResistantToDoTDamageMult;
            }

            if (VulnerableToCold.HasValue)
            {
                if (VulnerableToCold.Value)
                    ActiveColdDebuffMultiplier *= wormBoss ? VulnerableToDoTDamageMult_Worms_SlimeGod : VulnerableToDoTDamageMult;
                else
                    ActiveColdDebuffMultiplier *= ResistantToDoTDamageMult;
            }

            if (VulnerableToSickness.HasValue)
            {
                if (VulnerableToSickness.Value)
                    ActiveSicknessDebuffMultiplier *= wormBoss ? VulnerableToDoTDamageMult_Worms_SlimeGod : VulnerableToDoTDamageMult;
                else
                    ActiveSicknessDebuffMultiplier *= ResistantToDoTDamageMult;
            }
            if (VulnerableToElectricity.HasValue)
            {
                if (VulnerableToElectricity.Value)
                    ActiveElectricDebuffMultiplier *= wormBoss ? VulnerableToDoTDamageMult_Worms_SlimeGod : VulnerableToDoTDamageMult;
                else
                    ActiveElectricDebuffMultiplier *= ResistantToDoTDamageMult;
            }
            if (VulnerableToWater.HasValue)
            {
                if (VulnerableToWater.Value)
                    ActiveWaterDebuffMultiplier *= wormBoss ? VulnerableToDoTDamageMult_Worms_SlimeGod : VulnerableToDoTDamageMult;
                else
                    ActiveWaterDebuffMultiplier *= ResistantToDoTDamageMult;
            }
            #endregion

            //Apply DoT Debuffs
            for (var index = 0; index < npc.buffType.Length; index++)
            {
                var type = npc.buffType[index];
                var debuffData = CalamityBuffSets.DebuffDataset[type];
                if (debuffData == null || debuffData == DebuffData.Oiled) //Oiled is done after
                    continue;
                debuffData.NPCLifeRegenMethod(npc, type, ref index, ref damage);
            }
            //Oiled comes after so that we can detect if they have a heat debuff in the above loop
            bool hasVanillaOil = npc.onFrostBurn || npc.onFrostBurn2 || npc.onFire || npc.onFire2 || npc.onFire3 || npc.shadowFlame;
            if (npc.oiled)
            {
                var oil = DebuffData.Oiled;
                int index = npc.FindBuffIndex(BuffID.Oiled);
                if (hasVanillaOil)
                    npc.lifeRegen -= oil.EnemyVanillaRegenToCancelOut;
                oil.NPCLifeRegenMethod(npc, BuffID.Oiled, ref index, ref damage);
            }

            // Debuffs that aren't affected by weaknesses or resistances.
            if (somaShredStacks > 0)
                Shred.TickDebuff(npc, this);

            // Reduce DoT on worm bosses and Creepers by 75%.
            if ((wormBoss || npc.type == NPCID.Creeper) && npc.lifeRegen < 0)
            {
                npc.lifeRegen /= 4;
                if (npc.lifeRegen > -1)
                    npc.lifeRegen = -1;

                // Every other EoW body segment and the head segments are immune to DoT in Death Mode.
                if (((npc.ai[2] % 2f == 0f && npc.type == NPCID.EaterofWorldsBody) || npc.type == NPCID.EaterofWorldsHead) && (CalamityWorld.death || BossRushEvent.BossRushActive))
                    npc.lifeRegen = 0;
            }

            // Mana Burn
            // This is at the end to leave it full effect on worms, and to force the DOT numbers to match mana burn
            if (manaBurn > 0)
            {
                if (manaBurnPeak >= 0.1f)
                    manaBurnPeak *= 0.999f;

                manaBurnPeak = Math.Max(manaBurnPeak, manaBurn);
                int burnPerSecond = (int)MathF.Ceiling(manaBurn * 0.5f);
                manaBurn -= burnPerSecond / 60f;

                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= burnPerSecond * 2;
                damage += (int)(burnPerSecond * 0.5f);
            }
            else
            {
                manaBurnPeak = 0;
                playerManaBurnIntensity = 0;
            }

            // This is at the end to make sure the dmg check for particle creations runs properly
            if (glaiveShredTimer > 0 || blazingStarShredTimer > 0)
            {
                int dmg = 0;
                if (glaiveShredTimer > 0)
                {
                    dmg += 200; // 100 DPS
                    glaiveShredTimer--;
                }
                if (blazingStarShredTimer > 0)
                {
                    dmg += 480; // 240 DPS
                    blazingStarShredTimer--;
                }
                dmg = (int)ActiveTypelessDebuffMultiplier.ApplyTo(dmg);
                npc.lifeRegenCount -= dmg;
                if (damage < dmg / 12) // 1/6th of the DPS dealt by Glaive Shred shows up as the indicator, unless another debuff does more per tick
                    damage = dmg / 12;

                if (-120 * damage >= npc.lifeRegenCount)
                {
                    GeneralParticleHandler.SpawnParticle(new CustomSpark(npc.Center, new Vector2(1, 0).RotatedByRandom(7), "CalamityMod/Particles/TrientCircularSmear", false, 15, 0.4f + 0.1f * npc.width / 16f, Color.White, new Vector2(0.5f, 1f)));
                }
            }
        }

        public void ApplyDPSDebuff(int lifeRegenValue, int damageValue, ref int lifeRegen, ref int damage)
        {
            if (lifeRegen > 0)
                lifeRegen = 0;

            lifeRegen -= lifeRegenValue;

            if (damage < damageValue)
                damage = damageValue;
        }
        #endregion

        #region Load/Unload
        public override void Load()
        {
            #region Setup Vanilla DR Values
            DRValues = new SortedDictionary<int, float> {
                { NPCID.CultistBoss, 0.15f },
                { NPCID.DukeFishron, 0.15f },
                { NPCID.Golem, 0.15f },
                { NPCID.GolemFistLeft, 0.15f },
                { NPCID.GolemFistRight, 0.15f },
                { NPCID.GolemHead, 0.15f },
                { NPCID.MoonLordCore, 0.15f },
                { NPCID.MoonLordHand, 0.15f },
                { NPCID.MoonLordHead, 0.15f },
                { NPCID.Plantera, 0.15f },
                { NPCID.HallowBoss, 0.15f },
                { NPCID.PrimeCannon, 0.2f },
                { NPCID.PrimeLaser, 0.2f },
                { NPCID.PrimeSaw, 0.2f },
                { NPCID.PrimeVice, 0.2f },
                { NPCID.Retinazer, 0.2f },
                { NPCID.SkeletronPrime, 0.2f },
                { NPCID.Spazmatism, 0.2f },
                { NPCID.TheDestroyer, 0.1f },
                { NPCID.TheDestroyerBody, 0.2f },
                { NPCID.TheDestroyerTail, 0.35f },
                { NPCID.WallofFlesh, 0.15f },
            };
            #endregion
        }

        public override void Unload()
        {
            DRValues?.Clear();
            DRValues = null;
        }
        #endregion

        #region Set Defaults
        public override void SetStaticDefaults()
        {
            // Set Plantera to be able to update oldPos[x]
            // This is only used for her Rev+ AI charge attacks
            NPCID.Sets.TrailingMode[NPCID.Plantera] = 1;

            // Allow Moon Lord to directly be summoned in Multiplayer.
            // This is used for the modified Celestial Sigil without Impending Doom.
            NPCID.Sets.MPAllowedEnemies[NPCID.MoonLordCore] = true;
        }

        public override void SetDefaults(NPC npc)
        {
            for (int i = 0; i < maxPlayerImmunities; i++)
                dashImmunityTime[i] = 0;

            for (int m = 0; m < maxAIMod; m++)
                newAI[m] = 0f;

            // Apply DR to vanilla NPCs.
            // This also applies DR to other mods' NPCs who have set up their NPCs to have DR.
            if (DRValues.ContainsKey(npc.type))
            {
                DRValues.TryGetValue(npc.type, out float newDR);
                DR = newDR;
            }

            // Aquatic Scourge sets kill time in AI, not here.
            if (!CalamityNPCTypeSets.AquaticScourge.Contains(npc.type))
                KillTime = CalamityNPCSets.BossKillTimes[npc.type];

            // Fixing more red mistakes
            if (npc.type == NPCID.WallofFleshEye)
                npc.netAlways = true;

            if (npc.type == NPCID.Golem && (CalamityWorld.revenge || BossRushEvent.BossRushActive))
                npc.noGravity = true;

            DeclareBossHealthUIVariables(npc);

            if (BossRushEvent.BossRushActive)
                BossRushStatChanges(npc, Mod);

            if (CalamityWorld.revenge)
                RevDeathStatChanges(npc, Mod);

            OtherStatChanges(npc);

            // Change Queen Slime's fart sound on death to something more serious. Except GFB though because naturally
            if (npc.type == NPCID.QueenSlimeBoss)
                npc.DeathSound = Main.zenithWorld ? new SoundStyle("CalamityMod/Sounds/Item/GFBScreams/Scream", 8) : SoundID.NPCDeath1;

            // Function lives in NPCDebuffs.cs
            // This applies to ALL NPCs, vanilla AND Calamity.
            // Calamity NPC debuff immunity definitions live here.
            // Changes to vanilla debuff immunities are applied holistically in the function.
            // Sweeping debuff vulnerabilities for special effects are also applied in this function.
            //
            // NO CALAMITY NPC DEFINES THEIR DEBUFF VULNERABILITIES IN THEIR OWN FILE.
            // THEY ALL RELY ON THIS SINGLE DATABASE.
            npc.SetDebuffImmunities();

            VulnerabilitiesAndResistances(npc);

            // Gives Brain of Cthulhu a unique boss bar in Rev+ where Creepers contribute to a Shield rather than additional Health
            if (npc.type == NPCID.BrainofCthulhu && CalamityWorld.revenge)
                npc.BossBar = GetInstance<RevBrainOfCthulhuBossBar>();
            // Replaces Moon Lord's boss bar in Rev+ to fix a health counting bug with the eyes after being killed
            bool hasBar = Main.BigBossProgressBar.TryGetSpecialVanillaBossBar(npc.type, out IBigProgressBar bar);
            if (hasBar && bar is MoonLordProgressBar && CalamityWorld.revenge)
                npc.BossBar = GetInstance<RevMoonLordBossBar>();
        }

        public override bool? CanFallThroughPlatforms(NPC npc)
        {
            // Allow the free Golem Head to pass through platforms in Rev+
            if (npc.type == NPCID.GolemHeadFree && (CalamityWorld.revenge || BossRushEvent.BossRushActive))
                return true;
            return base.CanFallThroughPlatforms(npc);
        }
        #endregion

        #region Boss Health UI Variable Setting
        public void DeclareBossHealthUIVariables(NPC npc)
        {
            if (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)
                SplittingWorm = true;
        }
        #endregion

        #region Boss Rush Stat Changes
        private void BossRushStatChanges(NPC npc, Mod mod)
        {
            if (CalamityNPCSets.BossRushHealth.TryGetValue(npc.type, out var newHP))
            {
                npc.lifeMax = newHP;
            }
        }
        #endregion

        #region Revengeance and Death Mode Stat Changes
        private void RevDeathStatChanges(NPC npc, Mod mod)
        {
            // Health changes (disabled in Boss Rush)
            if (!BossRushEvent.BossRushActive)
            {
                switch (npc.type)
                {
                    case NPCID.KingSlime:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.5);
                        break;
                    case NPCID.EyeofCthulhu:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.5);
                        break;
                    case NPCID.ServantofCthulhu:
                        npc.lifeMax *= 4;
                        break;
                    case NPCID.BrainofCthulhu:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.95);
                        break;
                    case NPCID.Creeper:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.1);
                        break;
                    case NPCID.QueenBee:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.8);
                        break;
                    case NPCID.Bee:
                    case NPCID.BeeSmall:
                        if (CalamityPlayer.areThereAnyDamnBosses)
                            npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.5);
                        break;
                    case NPCID.Deerclops:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.4);
                        break;
                    case NPCID.SkeletronHand:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * (CalamityWorld.death ? 0.5 : 0.75));
                        break;
                    case NPCID.WallofFlesh:
                    case NPCID.WallofFleshEye:
                        npc.lifeMax *= 2;
                        break;
                    case NPCID.QueenSlimeBoss:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.8);
                        break;
                    case NPCID.Retinazer:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.4);
                        break;
                    case NPCID.Spazmatism:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.3);
                        break;
                    case NPCID.Probe:
                        if (CalamityWorld.death)
                            npc.lifeMax *= 2;
                        break;
                    case NPCID.SkeletronPrime:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.2);
                        break;
                    case NPCID.PrimeCannon:
                    case NPCID.PrimeSaw:
                    case NPCID.PrimeVice:
                    case NPCID.PrimeLaser:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * 0.65);
                        break;
                    case NPCID.Plantera:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * 2.85);
                        break;
                    case NPCID.PlanterasTentacle:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * 0.5);
                        break;
                    case NPCID.Golem:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * 3.2);
                        break;
                    case NPCID.GolemHead:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.28);
                        break;
                    case NPCID.GolemFistLeft:
                    case NPCID.GolemFistRight:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * 0.75);
                        break;
                    case NPCID.DukeFishron:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * 2.3);
                        break;
                    case NPCID.Sharkron:
                    case NPCID.Sharkron2:
                        npc.lifeMax *= 5;
                        break;
                    case NPCID.HallowBoss:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.7);
                        break;
                    case NPCID.CultistBoss:
                        npc.lifeMax *= 3;
                        break;
                    case NPCID.AncientCultistSquidhead:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.8);
                        break;
                    case NPCID.MoonLordCore:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * 2.2);
                        break;
                    case NPCID.MoonLordHand:
                    case NPCID.MoonLordHead:
                    case NPCID.MoonLordLeechBlob:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.2);
                        break;
                }

                if (npc.type >= NPCID.CultistDragonHead && npc.type <= NPCID.CultistDragonTail)
                {
                    npc.lifeMax = (int)Math.Round(npc.lifeMax * 2.4);
                }
                else if (CalamityNPCTypeSets.Destroyer.Contains(npc.type))
                {
                    npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.25);
                }
                else if (CalamityNPCTypeSets.EaterOfWorlds.Contains(npc.type))
                {
                    npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.4);
                }
            }

            // Other stat changes
            switch (npc.type)
            {
                case NPCID.KingSlime:
                    npc.scale = CalamityWorld.death ? (Main.getGoodWorld ? 6f : 2.5f) : (Main.getGoodWorld ? 3f : 1.5f);
                    break;
                case NPCID.QueenBee:
                    npc.defense = 14;
                    npc.defDefense = npc.defense;
                    break;
                case NPCID.Bee:
                case NPCID.BeeSmall:
                    if (CalamityPlayer.areThereAnyDamnBosses)
                        npc.scale *= 1.25f;
                    break;
                case NPCID.Probe:
                    npc.scale *= Main.zenithWorld ? 2f : 1.2f;
                    break;
                case NPCID.PrimeCannon:
                case NPCID.PrimeSaw:
                case NPCID.PrimeVice:
                case NPCID.PrimeLaser:
                    npc.scale *= 1.15f;
                    break;
                case NPCID.GolemFistLeft:
                case NPCID.GolemFistRight:
                    npc.scale *= 1.15f;
                    break;
                case NPCID.Mothron:
                    npc.scale *= 1.25f;
                    break;
                case NPCID.Ghost:
                case NPCID.IceMimic:
                case NPCID.Mimic:
                case NPCID.PresentMimic:
                case NPCID.Reaper:
                case NPCID.SandElemental:
                case NPCID.Wraith:
                    if (Main.getGoodWorld)
                        npc.knockBackResist = 0f;
                    break;
            }

            if (CalamityNPCTypeSets.Destroyer.Contains(npc.type))
            {
                npc.scale *= Main.zenithWorld ? 2f : 1.2f;
            }
            else if (CalamityNPCTypeSets.EaterOfWorlds.Contains(npc.type))
            {
                if (CalamityWorld.death)
                    npc.scale *= 1.1f;
            }
        }
        #endregion

        #region Vulnerabilities and Resistances
        private void VulnerabilitiesAndResistances(NPC npc)
        {
            // These enemies are categorized in such a way to make them easy to understand
            // Regroup these if necessary, reminder to keep it comprehensive
            switch (npc.type)
            {
                // Organic desert enemies: +Cold, +Sickness, +Water
                #region Organic Desert
                case NPCID.Antlion:
                case NPCID.GiantWalkingAntlion:
                case NPCID.FlyingAntlion:
                case NPCID.GiantFlyingAntlion:
                case NPCID.LarvaeAntlion:
                case NPCID.WalkingAntlion:
                case NPCID.TombCrawlerHead:
                case NPCID.TombCrawlerBody:
                case NPCID.TombCrawlerTail:
                case NPCID.DesertBeast:
                case NPCID.DuneSplicerHead:
                case NPCID.DuneSplicerBody:
                case NPCID.DuneSplicerTail:
                case NPCID.DesertLamiaDark:
                case NPCID.DesertLamiaLight:
                case NPCID.DesertGhoul:
                case NPCID.DesertGhoulCorruption:
                case NPCID.DesertGhoulCrimson:
                case NPCID.DesertGhoulHallow:
                case NPCID.Mummy:
                case NPCID.DarkMummy:
                case NPCID.LightMummy:
                case NPCID.BloodMummy:
                case NPCID.Tumbleweed:
                case NPCID.SandShark:
                case NPCID.SandsharkCorrupt:
                case NPCID.SandsharkCrimson:
                case NPCID.SandsharkHallow:
                    VulnerableToCold = true;
                    VulnerableToSickness = true;
                    VulnerableToWater = true;
                    break;
                #endregion

                // Sand Poacher and Sand Elemental: +Cold, +Water, -Sickness
                case NPCID.DesertScorpionWalk:
                case NPCID.DesertScorpionWall:
                case NPCID.SandElemental:
                    VulnerableToCold = true;
                    VulnerableToSickness = false;
                    VulnerableToWater = true;
                    break;

                // Sand Slime: +Cold, +Water, +Heat, -Sickness
                case NPCID.SandSlime:
                    VulnerableToCold = true;
                    VulnerableToSickness = false;
                    VulnerableToWater = true;
                    VulnerableToHeat = true;
                    break;

                // Organic undead enemies covered in slime: +Cold, +Heat
                case NPCID.ArmedZombieSlimed:
                case NPCID.BigSlimedZombie:
                case NPCID.SlimedZombie:
                case NPCID.SmallSlimedZombie:
                    VulnerableToCold = true;
                    VulnerableToHeat = true;
                    break;

                // Lava Slime: +Cold, +Water, -Sickness, -Heat
                case NPCID.LavaSlime:
                    VulnerableToCold = true;
                    VulnerableToSickness = false;
                    VulnerableToHeat = false;
                    VulnerableToWater = true;
                    break;

                // Regular slimes: +Heat, -Sickness
                #region Slimes
                case NPCID.QueenSlimeBoss:
                case NPCID.QueenSlimeMinionBlue:
                case NPCID.QueenSlimeMinionPink:
                case NPCID.QueenSlimeMinionPurple:
                case NPCID.DungeonSlime:
                case NPCID.BabySlime:
                case NPCID.BlackSlime:
                case NPCID.BlueSlime:
                case NPCID.CorruptSlime:
                case NPCID.GoldenSlime:
                case NPCID.GreenSlime:
                case NPCID.IlluminantSlime:
                case NPCID.JungleSlime:
                case NPCID.KingSlime:
                case NPCID.MotherSlime:
                case NPCID.PurpleSlime:
                case NPCID.RainbowSlime:
                case NPCID.RedSlime:
                case NPCID.ShimmerSlime:
                case NPCID.Slimeling:
                case NPCID.SlimeMasked:
                case NPCID.Slimer:
                case NPCID.Slimer2:
                case NPCID.SlimeRibbonGreen:
                case NPCID.SlimeRibbonRed:
                case NPCID.SlimeRibbonWhite:
                case NPCID.SlimeRibbonYellow:
                case NPCID.SlimeSpiked:
                case NPCID.SpikedJungleSlime:
                case NPCID.UmbrellaSlime:
                case NPCID.YellowSlime:
                case NPCID.ToxicSludge:
                case NPCID.Crimslime:
                case NPCID.BigCrimslime:
                case NPCID.LittleCrimslime:
                case NPCID.Gastropod:
                case NPCID.Pinky:
                    VulnerableToSickness = false;
                    VulnerableToHeat = true;
                    break;
                #endregion

                // Skeleton enemies that are heat-related: +Cold, +Water, -Heat, -Sickness
                case NPCID.HellArmoredBones:
                case NPCID.HellArmoredBonesMace:
                case NPCID.HellArmoredBonesSpikeShield:
                case NPCID.HellArmoredBonesSword:
                case NPCID.DiabolistRed:
                case NPCID.DiabolistWhite:
                    VulnerableToHeat = false;
                    VulnerableToCold = true;
                    VulnerableToSickness = false;
                    VulnerableToWater = true;
                    break;

                // Spore Skeleton: +Heat, +Water, -Sickness
                case NPCID.SporeSkeleton:
                    VulnerableToHeat = true;
                    VulnerableToSickness = false;
                    VulnerableToWater = true;
                    break;

                // Skeleton and rock enemies: +Water, -Sickness
                #region Skeletons And Rock Enemies
                case NPCID.SkeletronHand:
                case NPCID.SkeletronHead:
                case NPCID.AngryBones:
                case NPCID.AngryBonesBig:
                case NPCID.AngryBonesBigHelmet:
                case NPCID.AngryBonesBigMuscle:
                case NPCID.DarkCaster:
                case NPCID.CursedSkull:
                case NPCID.GiantCursedSkull:
                case NPCID.DungeonGuardian:
                case NPCID.BigBoned:
                case NPCID.BlueArmoredBones:
                case NPCID.BlueArmoredBonesMace:
                case NPCID.BlueArmoredBonesNoPants:
                case NPCID.BlueArmoredBonesSword:
                case NPCID.BoneLee:
                case NPCID.BoneSerpentBody:
                case NPCID.BoneSerpentHead:
                case NPCID.BoneSerpentTail:
                case NPCID.BoneThrowingSkeleton:
                case NPCID.BoneThrowingSkeleton2:
                case NPCID.BoneThrowingSkeleton3:
                case NPCID.BoneThrowingSkeleton4:
                case NPCID.RustyArmoredBonesAxe:
                case NPCID.RustyArmoredBonesFlail:
                case NPCID.RustyArmoredBonesSword:
                case NPCID.RustyArmoredBonesSwordNoArmor:
                case NPCID.ShortBones:
                case NPCID.Necromancer:
                case NPCID.NecromancerArmored:
                case NPCID.RaggedCaster:
                case NPCID.RaggedCasterOpenCoat:
                case NPCID.SkeletonCommando:
                case NPCID.ArmoredSkeleton:
                case NPCID.BigHeadacheSkeleton:
                case NPCID.BigMisassembledSkeleton:
                case NPCID.BigPantlessSkeleton:
                case NPCID.BigSkeleton:
                case NPCID.DD2SkeletonT1:
                case NPCID.DD2SkeletonT3:
                case NPCID.GreekSkeleton:
                case NPCID.HeadacheSkeleton:
                case NPCID.HeavySkeleton:
                case NPCID.MisassembledSkeleton:
                case NPCID.PantlessSkeleton:
                case NPCID.Skeleton:
                case NPCID.SkeletonAlien:
                case NPCID.SkeletonArcher:
                case NPCID.SkeletonAstonaut:
                case NPCID.SkeletonSniper:
                case NPCID.SkeletonTopHat:
                case NPCID.SmallHeadacheSkeleton:
                case NPCID.SmallMisassembledSkeleton:
                case NPCID.SmallPantlessSkeleton:
                case NPCID.SmallSkeleton:
                case NPCID.TacticalSkeleton:
                case NPCID.Tim:
                case NPCID.UndeadMiner:
                case NPCID.UndeadViking:
                case NPCID.ArmoredViking:
                case NPCID.GraniteFlyer:
                case NPCID.GraniteGolem:
                case NPCID.RuneWizard:
                case NPCID.Golem:
                case NPCID.GolemFistLeft:
                case NPCID.GolemFistRight:
                case NPCID.GolemHead:
                case NPCID.GolemHeadFree:
                case NPCID.RockGolem:
                    VulnerableToSickness = false;
                    VulnerableToWater = true;
                    break;
                #endregion

                // Metal non-robotic enemies: -Sickness
                case NPCID.BigMimicCorruption:
                case NPCID.BigMimicCrimson:
                case NPCID.BigMimicHallow:
                case NPCID.BigMimicJungle:
                case NPCID.Paladin:
                case NPCID.Mimic:
                case NPCID.PresentMimic:
                case NPCID.PirateShipCannon:
                case NPCID.PossessedArmor:
                    VulnerableToSickness = false;
                    break;

                // Robotic enemies: +Electricity, -Sickness
                #region Robots
                case NPCID.Probe:
                case NPCID.MartianProbe:
                case NPCID.DeadlySphere:
                case NPCID.MartianDrone:
                case NPCID.MartianWalker:
                case NPCID.MartianTurret:
                case NPCID.ElfCopter:
                case NPCID.SkeletronPrime:
                case NPCID.PrimeCannon:
                case NPCID.PrimeLaser:
                case NPCID.PrimeSaw:
                case NPCID.PrimeVice:
                case NPCID.TheDestroyer:
                case NPCID.TheDestroyerBody:
                case NPCID.TheDestroyerTail:
                case NPCID.SantaNK1:
                case NPCID.MartianSaucer:
                case NPCID.MartianSaucerCannon:
                case NPCID.MartianSaucerCore:
                case NPCID.MartianSaucerTurret:
                case NPCID.ChatteringTeethBomb:
                    VulnerableToElectricity = true;
                    VulnerableToSickness = false;
                    break;
                #endregion

                // Ghostly or ethereal enemies: -Sickness
                #region Ghosts
                case NPCID.DungeonSpirit:
                case NPCID.AncientCultistSquidhead:
                case NPCID.CultistDragonBody1:
                case NPCID.CultistDragonBody2:
                case NPCID.CultistDragonBody3:
                case NPCID.CultistDragonBody4:
                case NPCID.CultistDragonHead:
                case NPCID.CultistDragonTail:
                case NPCID.Ghost:
                case NPCID.ChaosElemental:
                case NPCID.CrimsonAxe:
                case NPCID.EnchantedSword:
                case NPCID.CursedHammer:
                case NPCID.DesertDjinn:
                case NPCID.Wraith:
                case NPCID.ShadowFlameApparition:
                case NPCID.Reaper:
                case NPCID.Poltergeist:
                case NPCID.Pixie:
                case NPCID.PirateGhost:
                    VulnerableToSickness = false;
                    break;
                #endregion

                // Organic enemies: +Cold, +Heat, +Sickness
                #region Organic Enemies
                case NPCID.HallowBoss:
                case NPCID.Dandelion:
                case NPCID.Gnome:
                case NPCID.BloodEelHead:
                case NPCID.BloodEelBody:
                case NPCID.BloodEelTail:
                case NPCID.BloodSquid:
                case NPCID.BloodNautilus:
                case NPCID.GoblinShark:
                case NPCID.EyeballFlyingFish:
                case NPCID.ZombieMerman:
                case NPCID.CultistArcherBlue:
                case NPCID.CultistArcherWhite:
                case NPCID.CultistBoss:
                case NPCID.CultistDevote:
                case NPCID.BloodCrawler:
                case NPCID.BloodCrawlerWall:
                case NPCID.CaveBat:
                case NPCID.GiantBat:
                case NPCID.CochinealBeetle:
                case NPCID.CyanBeetle:
                case NPCID.LacBeetle:
                case NPCID.AnomuraFungus:
                case NPCID.GiantFungiBulb:
                case NPCID.FungiBulb:
                case NPCID.MushiLadybug:
                case NPCID.SporeBat:
                case NPCID.ZombieMushroom:
                case NPCID.ZombieMushroomHat:
                case NPCID.ManEater:
                case NPCID.Snatcher:
                case NPCID.AngryTrapper:
                case NPCID.HoppinJack:
                case NPCID.Splinterling:
                case NPCID.MourningWood:
                case NPCID.Pumpking:
                case NPCID.Everscream:
                case NPCID.Crimera:
                case NPCID.BigCrimera:
                case NPCID.LittleCrimera:
                case NPCID.DemonEye:
                case NPCID.DemonEye2:
                case NPCID.DemonEyeOwl:
                case NPCID.DemonEyeSpaceship:
                case NPCID.DevourerBody:
                case NPCID.DevourerHead:
                case NPCID.DevourerTail:
                case NPCID.DoctorBones:
                case NPCID.EaterofSouls:
                case NPCID.BigEater:
                case NPCID.LittleEater:
                case NPCID.EaterofWorldsBody:
                case NPCID.EaterofWorldsHead:
                case NPCID.EaterofWorldsTail:
                case NPCID.FaceMonster:
                case NPCID.GiantShelly:
                case NPCID.GiantShelly2:
                case NPCID.GiantWormBody:
                case NPCID.GiantWormHead:
                case NPCID.GiantWormTail:
                case NPCID.GoblinScout:
                case NPCID.Harpy:
                case NPCID.JungleBat:
                case NPCID.Nymph:
                case NPCID.Raven:
                case NPCID.Salamander:
                case NPCID.Salamander2:
                case NPCID.Salamander3:
                case NPCID.Salamander4:
                case NPCID.Salamander5:
                case NPCID.Salamander6:
                case NPCID.Salamander7:
                case NPCID.Salamander8:
                case NPCID.Salamander9:
                case NPCID.Vulture:
                case NPCID.WallCreeper:
                case NPCID.WallCreeperWall:
                case NPCID.ArmedZombie:
                case NPCID.ArmedZombieCenx:
                case NPCID.ArmedZombiePincussion:
                case NPCID.ArmedZombieSwamp:
                case NPCID.ArmedZombieTwiggy:
                case NPCID.ArmedTorchZombie:
                case NPCID.BaldZombie:
                case NPCID.BigBaldZombie:
                case NPCID.BigFemaleZombie:
                case NPCID.BigPincushionZombie:
                case NPCID.BigRainZombie:
                case NPCID.BigSwampZombie:
                case NPCID.BigTwiggyZombie:
                case NPCID.BigZombie:
                case NPCID.MaggotZombie:
                case NPCID.BloodZombie:
                case NPCID.FemaleZombie:
                case NPCID.PincushionZombie:
                case NPCID.SmallBaldZombie:
                case NPCID.SmallFemaleZombie:
                case NPCID.SmallPincushionZombie:
                case NPCID.SmallRainZombie:
                case NPCID.SmallSwampZombie:
                case NPCID.SmallTwiggyZombie:
                case NPCID.SmallZombie:
                case NPCID.SwampZombie:
                case NPCID.TorchZombie:
                case NPCID.TwiggyZombie:
                case NPCID.Zombie:
                case NPCID.ZombieDoctor:
                case NPCID.ZombiePixie:
                case NPCID.ZombieRaincoat:
                case NPCID.ZombieSuperman:
                case NPCID.ZombieSweater:
                case NPCID.ZombieXmas:
                case NPCID.Clinger:
                case NPCID.Corruptor:
                case NPCID.Derpling:
                case NPCID.Herpling:
                case NPCID.DiggerBody:
                case NPCID.DiggerHead:
                case NPCID.DiggerTail:
                case NPCID.FloatyGross:
                case NPCID.FlyingSnake:
                case NPCID.Lihzahrd:
                case NPCID.LihzahrdCrawler:
                case NPCID.GiantFlyingFox:
                case NPCID.GiantTortoise:
                case NPCID.IchorSticker:
                case NPCID.IlluminantBat:
                case NPCID.Medusa:
                case NPCID.Moth:
                case NPCID.Unicorn:
                case NPCID.WanderingEye:
                case NPCID.Werewolf:
                case NPCID.SeekerBody:
                case NPCID.SeekerHead:
                case NPCID.SeekerTail:
                case NPCID.WyvernBody:
                case NPCID.WyvernBody2:
                case NPCID.WyvernBody3:
                case NPCID.WyvernHead:
                case NPCID.WyvernLegs:
                case NPCID.WyvernTail:
                case NPCID.Clown:
                case NPCID.CorruptBunny:
                case NPCID.CrimsonBunny:
                case NPCID.Drippler:
                case NPCID.TheGroom:
                case NPCID.TheBride:
                case NPCID.GoblinArcher:
                case NPCID.GoblinPeon:
                case NPCID.GoblinSorcerer:
                case NPCID.GoblinSummoner:
                case NPCID.GoblinThief:
                case NPCID.GoblinWarrior:
                case NPCID.DD2DarkMageT1:
                case NPCID.DD2DarkMageT3:
                case NPCID.DD2DrakinT2:
                case NPCID.DD2DrakinT3:
                case NPCID.DD2GoblinBomberT1:
                case NPCID.DD2GoblinBomberT2:
                case NPCID.DD2GoblinBomberT3:
                case NPCID.DD2GoblinT1:
                case NPCID.DD2GoblinT2:
                case NPCID.DD2GoblinT3:
                case NPCID.DD2JavelinstT1:
                case NPCID.DD2JavelinstT2:
                case NPCID.DD2JavelinstT3:
                case NPCID.DD2KoboldFlyerT2:
                case NPCID.DD2KoboldFlyerT3:
                case NPCID.DD2KoboldWalkerT2:
                case NPCID.DD2KoboldWalkerT3:
                case NPCID.DD2OgreT2:
                case NPCID.DD2OgreT3:
                case NPCID.DD2WitherBeastT2:
                case NPCID.DD2WitherBeastT3:
                case NPCID.DD2WyvernT1:
                case NPCID.DD2WyvernT2:
                case NPCID.DD2WyvernT3:
                case NPCID.Parrot:
                case NPCID.PirateCaptain:
                case NPCID.PirateCorsair:
                case NPCID.PirateCrossbower:
                case NPCID.PirateDeadeye:
                case NPCID.PirateDeckhand:
                case NPCID.Mothron:
                case NPCID.MothronEgg:
                case NPCID.MothronSpawn:
                case NPCID.Butcher:
                case NPCID.DrManFly:
                case NPCID.Eyezor:
                case NPCID.Frankenstein:
                case NPCID.Fritz:
                case NPCID.Nailhead:
                case NPCID.Psycho:
                case NPCID.SwampThing:
                case NPCID.ThePossessed:
                case NPCID.Vampire:
                case NPCID.VampireBat:
                case NPCID.BrainScrambler:
                case NPCID.GigaZapper:
                case NPCID.GrayGrunt:
                case NPCID.MartianEngineer:
                case NPCID.MartianOfficer:
                case NPCID.RayGunner:
                case NPCID.Scutlix:
                case NPCID.ScutlixRider:
                case NPCID.HeadlessHorseman:
                case NPCID.Hellhound:
                case NPCID.Scarecrow1:
                case NPCID.Scarecrow2:
                case NPCID.Scarecrow3:
                case NPCID.Scarecrow4:
                case NPCID.Scarecrow5:
                case NPCID.Scarecrow6:
                case NPCID.Scarecrow7:
                case NPCID.Scarecrow8:
                case NPCID.Scarecrow9:
                case NPCID.Scarecrow10:
                case NPCID.NebulaHeadcrab:
                case NPCID.LunarTowerNebula:
                case NPCID.NebulaBeast:
                case NPCID.NebulaBrain:
                case NPCID.NebulaSoldier:
                case NPCID.LunarTowerSolar:
                case NPCID.SolarCorite:
                case NPCID.SolarCrawltipedeTail:
                case NPCID.SolarDrakomire:
                case NPCID.SolarDrakomireRider:
                case NPCID.SolarSolenian:
                case NPCID.SolarSpearman:
                case NPCID.SolarSroller:
                case NPCID.LunarTowerStardust:
                case NPCID.StardustCellBig:
                case NPCID.StardustCellSmall:
                case NPCID.StardustJellyfishBig:
                case NPCID.StardustSoldier:
                case NPCID.StardustSpiderBig:
                case NPCID.StardustSpiderSmall:
                case NPCID.StardustWormHead:
                case NPCID.LunarTowerVortex:
                case NPCID.VortexHornet:
                case NPCID.VortexHornetQueen:
                case NPCID.VortexLarva:
                case NPCID.VortexRifleman:
                case NPCID.VortexSoldier:
                case NPCID.BrainofCthulhu:
                case NPCID.Creeper:
                case NPCID.EyeofCthulhu:
                case NPCID.ServantofCthulhu:
                case NPCID.MoonLordCore:
                case NPCID.MoonLordHand:
                case NPCID.MoonLordHead:
                case NPCID.Spazmatism: // Changes to robotic in phase 2
                case NPCID.Retinazer: // Changes to robotic in phase 2
                    VulnerableToCold = true;
                    VulnerableToHeat = true;
                    VulnerableToSickness = true;
                    break;
                #endregion

                // Demons: +Cold, +Sickness, -Heat
                #region Demons
                case NPCID.WallofFlesh:
                case NPCID.WallofFleshEye:
                case NPCID.TheHungry:
                case NPCID.TheHungryII:
                case NPCID.LeechBody:
                case NPCID.LeechHead:
                case NPCID.LeechTail:
                case NPCID.Demon:
                case NPCID.VoodooDemon:
                case NPCID.RedDevil:
                case NPCID.DemonTaxCollector:
                    VulnerableToCold = true;
                    VulnerableToHeat = false;
                    VulnerableToSickness = true;
                    break;
                #endregion

                // Fiery organic enemies: +Cold, +Sickness, +Water, -Heat
                case NPCID.FireImp:
                case NPCID.Hellbat:
                case NPCID.Lavabat:
                    VulnerableToCold = true;
                    VulnerableToHeat = false;
                    VulnerableToSickness = true;
                    VulnerableToWater = true;
                    break;

                // Meteor Head: +Cold, +Water, -Heat, -Sickness
                case NPCID.MeteorHead:
                    VulnerableToCold = true;
                    VulnerableToHeat = false;
                    VulnerableToSickness = false;
                    VulnerableToWater = true;
                    break;

                // Lightning Bug: +Cold, +Heat, +Sickness, -Electricity
                case NPCID.DD2LightningBugT3:
                    VulnerableToElectricity = false;
                    VulnerableToCold = true;
                    VulnerableToHeat = true;
                    VulnerableToSickness = true;
                    break;

                // Betsy: +Cold, +Sickness, -Heat
                case NPCID.DD2Betsy:
                    VulnerableToCold = true;
                    VulnerableToHeat = false;
                    VulnerableToSickness = true;
                    break;

                // Angry Nimbus: +Cold, -Heat, -Sickness, -Electricity, -Water
                case NPCID.AngryNimbus:
                    VulnerableToCold = true;
                    VulnerableToElectricity = false;
                    VulnerableToWater = false;
                    VulnerableToHeat = false;
                    VulnerableToSickness = false;
                    break;

                // Cold organic enemies: +Heat, +Sickness, -Cold
                #region Cold Organic
                case NPCID.ArmedZombieEskimo:
                case NPCID.ZombieEskimo:
                case NPCID.IceBat:
                case NPCID.SnowFlinx:
                case NPCID.IceTortoise:
                case NPCID.IcyMerman:
                case NPCID.PigronCorruption:
                case NPCID.PigronCrimson:
                case NPCID.PigronHallow:
                case NPCID.Wolf:
                case NPCID.CorruptPenguin:
                case NPCID.CrimsonPenguin:
                case NPCID.ElfArcher:
                case NPCID.Krampus:
                case NPCID.Yeti:
                case NPCID.Nutcracker:
                case NPCID.NutcrackerSpinning:
                case NPCID.ZombieElf:
                case NPCID.ZombieElfBeard:
                case NPCID.ZombieElfGirl:
                case NPCID.Deerclops:
                    VulnerableToHeat = true;
                    VulnerableToCold = false;
                    VulnerableToSickness = true;
                    break;
                #endregion

                // Cold non-organic enemies: +Heat, -Cold, -Sickness
                #region Cold Non-Organic
                case NPCID.IceElemental:
                case NPCID.IceSlime:
                case NPCID.SpikedIceSlime:
                case NPCID.IceGolem:
                case NPCID.IceMimic:
                case NPCID.MisterStabby:
                case NPCID.SnowBalla:
                case NPCID.SnowmanGangsta:
                case NPCID.Flocko:
                case NPCID.IceQueen:
                case NPCID.GingerbreadMan:
                    VulnerableToCold = false;
                    VulnerableToHeat = true;
                    VulnerableToSickness = false;
                    break;
                #endregion

                // Fish and aquatic enemies: +Sickness, +Electricity, -Water, -Heat
                #region Aquatic Enemies
                case NPCID.Crawdad:
                case NPCID.Crawdad2:
                case NPCID.BlueJellyfish:
                case NPCID.GreenJellyfish:
                case NPCID.PinkJellyfish:
                case NPCID.BloodJelly:
                case NPCID.FungoFish:
                case NPCID.Crab:
                case NPCID.Piranha:
                case NPCID.SeaSnail:
                case NPCID.Squid:
                case NPCID.Shark:
                case NPCID.AnglerFish:
                case NPCID.Arapaima:
                case NPCID.BloodFeeder:
                case NPCID.CorruptGoldfish:
                case NPCID.CrimsonGoldfish:
                case NPCID.FlyingFish:
                case NPCID.CreatureFromTheDeep:
                case NPCID.DukeFishron:
                case NPCID.Sharkron:
                case NPCID.Sharkron2:
                    VulnerableToHeat = false;
                    VulnerableToSickness = true;
                    VulnerableToElectricity = true;
                    VulnerableToWater = false;
                    break;
                #endregion

                // Bees, Hornets, and poisonous enemies: +Cold, +Heat, -Sickness
                #region Bees and Poisonous
                case NPCID.Bee:
                case NPCID.BeeSmall:
                case NPCID.QueenBee:
                case NPCID.BigHornetFatty:
                case NPCID.BigHornetHoney:
                case NPCID.BigHornetLeafy:
                case NPCID.BigHornetSpikey:
                case NPCID.BigHornetStingy:
                case NPCID.BigMossHornet:
                case NPCID.GiantMossHornet:
                case NPCID.Hornet:
                case NPCID.HornetFatty:
                case NPCID.HornetHoney:
                case NPCID.HornetLeafy:
                case NPCID.HornetSpikey:
                case NPCID.HornetStingy:
                case NPCID.LittleHornetFatty:
                case NPCID.LittleHornetHoney:
                case NPCID.LittleHornetLeafy:
                case NPCID.LittleHornetSpikey:
                case NPCID.LittleHornetStingy:
                case NPCID.LittleMossHornet:
                case NPCID.MossHornet:
                case NPCID.TinyMossHornet:
                case NPCID.JungleCreeper:
                case NPCID.JungleCreeperWall:
                case NPCID.BlackRecluse:
                case NPCID.BlackRecluseWall:
                case NPCID.Plantera:
                case NPCID.PlanterasTentacle:
                    VulnerableToCold = true;
                    VulnerableToHeat = true;
                    VulnerableToSickness = false;
                    break;
                #endregion

                // Town NPCs. Mostly irrelevant, but it displays in the Bestiary
                #region Town NPCs
                case NPCID.Merchant:
                case NPCID.Nurse:
                case NPCID.ArmsDealer:
                case NPCID.Dryad:
                case NPCID.Guide:
                case NPCID.OldMan:
                case NPCID.Demolitionist:
                case NPCID.Clothier:
                case NPCID.BoundGoblin:
                case NPCID.BoundWizard:
                case NPCID.GoblinTinkerer:
                case NPCID.Wizard:
                case NPCID.BoundMechanic:
                case NPCID.Mechanic:
                case NPCID.Truffle:
                case NPCID.Steampunker:
                case NPCID.DyeTrader:
                case NPCID.PartyGirl:
                case NPCID.Painter:
                case NPCID.WitchDoctor:
                case NPCID.Pirate:
                case NPCID.Stylist:
                case NPCID.WebbedStylist:
                case NPCID.TravellingMerchant:
                case NPCID.Angler:
                case NPCID.SleepingAngler:
                case NPCID.DD2Bartender:
                case NPCID.BartenderUnconscious:
                case NPCID.Golfer:
                case NPCID.GolferRescue:
                case NPCID.BestiaryGirl:
                case NPCID.Princess:
                case NPCID.TownCat:
                case NPCID.TownDog:
                case NPCID.TownBunny:
                    VulnerableToCold = true;
                    VulnerableToHeat = true;
                    VulnerableToSickness = true;
                    break;
                case NPCID.SantaClaus:
                    VulnerableToCold = false;
                    VulnerableToHeat = true;
                    VulnerableToSickness = true;
                    break;
                case NPCID.TaxCollector:
                    VulnerableToCold = true;
                    VulnerableToHeat = false;
                    VulnerableToSickness = true;
                    break;
                // Non-organic Town NPCs.
                case NPCID.Cyborg:
                case NPCID.BoundTownSlimeOld:
                    VulnerableToSickness = false;
                    break;
                // Town Slimes.
                case NPCID.TownSlimeBlue:
                case NPCID.TownSlimeGreen:
                case NPCID.TownSlimeOld:
                case NPCID.TownSlimePurple:
                case NPCID.TownSlimeRainbow:
                case NPCID.TownSlimeRed:
                case NPCID.TownSlimeYellow:
                case NPCID.TownSlimeCopper:
                case NPCID.BoundTownSlimePurple:
                    VulnerableToSickness = false;
                    VulnerableToHeat = true;
                    break;
                #endregion

                // Critters
                #region Critters
                case NPCID.Bunny:
                case NPCID.Bird:
                case NPCID.BirdBlue:
                case NPCID.BirdRed:
                case NPCID.Squirrel:
                case NPCID.Mouse:
                case NPCID.BunnySlimed:
                case NPCID.BunnyXmas:
                case NPCID.Firefly:
                case NPCID.Butterfly:
                case NPCID.Worm:
                case NPCID.LightningBug:
                case NPCID.Snail:
                case NPCID.GlowingSnail:
                case NPCID.Frog:
                case NPCID.Duck:
                case NPCID.Duck2:
                case NPCID.DuckWhite:
                case NPCID.DuckWhite2:
                case NPCID.ScorpionBlack:
                case NPCID.Scorpion:
                case NPCID.TruffleWorm:
                case NPCID.TruffleWormDigger:
                case NPCID.Grasshopper:
                case NPCID.GoldBird:
                case NPCID.GoldBunny:
                case NPCID.GoldButterfly:
                case NPCID.GoldFrog:
                case NPCID.GoldGrasshopper:
                case NPCID.GoldMouse:
                case NPCID.GoldWorm:
                case NPCID.EnchantedNightcrawler:
                case NPCID.Grubby:
                case NPCID.Sluggy:
                case NPCID.Buggy:
                case NPCID.SquirrelRed:
                case NPCID.SquirrelGold:
                case NPCID.PartyBunny:
                case NPCID.BlackDragonfly:
                case NPCID.BlueDragonfly:
                case NPCID.GreenDragonfly:
                case NPCID.OrangeDragonfly:
                case NPCID.RedDragonfly:
                case NPCID.YellowDragonfly:
                case NPCID.GoldDragonfly:
                case NPCID.Seagull:
                case NPCID.Seagull2:
                case NPCID.LadyBug:
                case NPCID.GoldLadyBug:
                case NPCID.Maggot:
                case NPCID.Grebe:
                case NPCID.Grebe2:
                case NPCID.Rat:
                case NPCID.Owl:
                case NPCID.WaterStrider:
                case NPCID.GoldWaterStrider:
                case NPCID.ExplosiveBunny:
                case NPCID.EmpressButterfly:
                case NPCID.Stinkbug:
                case NPCID.ScarletMacaw:
                case NPCID.BlueMacaw:
                case NPCID.Toucan:
                case NPCID.YellowCockatiel:
                case NPCID.GrayCockatiel:
                case NPCID.Shimmerfly:
                case NPCID.BoundTownSlimeYellow:
                    VulnerableToCold = true;
                    VulnerableToHeat = true;
                    VulnerableToSickness = true;
                    break;
                // Water Critters
                case NPCID.Goldfish:
                case NPCID.GoldfishWalker:
                case NPCID.GoldGoldfish:
                case NPCID.GoldGoldfishWalker:
                case NPCID.Pupfish:
                case NPCID.Dolphin:
                case NPCID.Turtle:
                case NPCID.TurtleJungle:
                case NPCID.SeaTurtle:
                case NPCID.Seahorse:
                case NPCID.GoldSeahorse:
                    VulnerableToHeat = false;
                    VulnerableToSickness = true;
                    VulnerableToElectricity = true;
                    VulnerableToWater = false;
                    break;
                // Penguins
                case NPCID.Penguin:
                case NPCID.PenguinBlack:
                    VulnerableToCold = false;
                    VulnerableToHeat = true;
                    VulnerableToSickness = true;
                    break;
                // Fairies
                case NPCID.FairyCritterPink:
                case NPCID.FairyCritterGreen:
                case NPCID.FairyCritterBlue:
                    VulnerableToSickness = false;
                    break;
                // Gem Critters
                case NPCID.GemSquirrelAmethyst:
                case NPCID.GemSquirrelTopaz:
                case NPCID.GemSquirrelSapphire:
                case NPCID.GemSquirrelEmerald:
                case NPCID.GemSquirrelRuby:
                case NPCID.GemSquirrelDiamond:
                case NPCID.GemSquirrelAmber:
                case NPCID.GemBunnyAmethyst:
                case NPCID.GemBunnyTopaz:
                case NPCID.GemBunnySapphire:
                case NPCID.GemBunnyEmerald:
                case NPCID.GemBunnyDiamond:
                case NPCID.GemBunnyAmber:
                    VulnerableToCold = true;
                    VulnerableToSickness = true;
                    VulnerableToWater = true;
                    break;
                // Underworld Critters
                case NPCID.HellButterfly:
                case NPCID.Lavafly:
                case NPCID.MagmaSnail:
                    VulnerableToCold = true;
                    VulnerableToHeat = false;
                    VulnerableToSickness = true;
                    VulnerableToWater = true;
                    break;
                    #endregion
            }
        }
        #endregion

        #region Other Stat Changes
        private void OtherStatChanges(NPC npc)
        {
            EditGlobalCoinDrops(npc);

            if ((npc.boss && npc.type != NPCID.MartianSaucerCore) || CalamityNPCSets.ScalesHealthLikeBoss[npc.type])
            {
                double HPBoost = CalamityServerConfig.Instance.BossHealthBoost * 0.01;
                npc.lifeMax += (int)Math.Round(npc.lifeMax * HPBoost);
            }

            switch (npc.type)
            {
                case NPCID.KingSlime:
                case NPCID.EyeofCthulhu:
                case NPCID.BrainofCthulhu:
                case NPCID.QueenBee:
                case NPCID.Paladin:
                case NPCID.BigMimicCorruption:
                case NPCID.BigMimicCrimson:
                case NPCID.BigMimicHallow:
                case NPCID.Mothron:
                case NPCID.EaterofWorldsHead:
                case NPCID.SkeletronHead:
                case NPCID.DungeonGuardian:
                case NPCID.WallofFlesh:
                case NPCID.Spazmatism:
                case NPCID.Retinazer:
                case NPCID.TheDestroyer:
                case NPCID.TheDestroyerBody:
                case NPCID.TheDestroyerTail:
                case NPCID.SkeletronPrime:
                case NPCID.PrimeVice:
                case NPCID.PrimeSaw:
                case NPCID.Plantera:
                case NPCID.PlanterasTentacle:
                case NPCID.Golem:
                case NPCID.GolemFistLeft:
                case NPCID.GolemFistRight:
                case NPCID.CultistDragonHead:
                case NPCID.DD2OgreT2:
                case NPCID.DD2OgreT3:
                case NPCID.DD2Betsy:
                case NPCID.PumpkingBlade:
                case NPCID.SantaNK1:
                case NPCID.DukeFishron:
                case NPCID.BloodNautilus:
                case NPCID.HallowBoss:
                case NPCID.QueenSlimeBoss:
                case NPCID.Deerclops:
                    canBreakPlayerDefense = true;
                    break;

                // These go through walls and are very annoying with the new tombstone breaking spawning them mechanic in 1.4
                case NPCID.Ghost:
                    npc.lifeMax = (int)Math.Round(npc.lifeMax * 0.5);
                    break;

                case NPCID.BloodSquid:
                    npc.lifeMax = (int)Math.Round(npc.lifeMax * 0.25);
                    break;

                case NPCID.LarvaeAntlion:
                    npc.lifeMax = (int)Math.Round(npc.lifeMax * 0.5);
                    break;

                // Reduce prehardmode desert enemy stats
                case NPCID.WalkingAntlion:
                case NPCID.GiantWalkingAntlion:
                    npc.lifeMax = (int)Math.Round(npc.lifeMax * DesertEnemyStatMultiplier);
                    npc.damage = (int)Math.Round(npc.damage * DesertEnemyStatMultiplier);
                    npc.defDamage = npc.damage;
                    npc.defense /= 2;
                    npc.defDefense = npc.defense;
                    break;

                case NPCID.Antlion:
                case NPCID.FlyingAntlion:
                case NPCID.GiantFlyingAntlion:
                    npc.lifeMax = (int)Math.Round(npc.lifeMax * DesertEnemyStatMultiplier);
                    npc.damage = (int)Math.Round(npc.damage * DesertEnemyStatMultiplier);
                    npc.defDamage = npc.damage;
                    npc.defense /= 2;
                    npc.defDefense = npc.defense;
                    break;

                // Reduce Tomb Crawler stats
                case NPCID.TombCrawlerHead:
                    npc.lifeMax = (int)Math.Round(npc.lifeMax * 0.5);
                    npc.damage = (int)Math.Round(npc.damage * DesertEnemyStatMultiplier);
                    npc.defDamage = npc.damage;
                    break;

                case NPCID.TombCrawlerBody:
                case NPCID.TombCrawlerTail:
                    npc.lifeMax = (int)Math.Round(npc.lifeMax * 0.5);
                    npc.damage = (int)Math.Round(npc.damage * DesertEnemyStatMultiplier);
                    npc.defDamage = npc.damage;
                    npc.defense /= 2;
                    npc.defDefense = npc.defense;
                    break;

                // Fix Sharkron hitboxes
                case NPCID.Sharkron:
                case NPCID.Sharkron2:
                    npc.width = npc.height = 36;
                    npc.chaseable = false;
                    break;

                // Make Core hitbox bigger
                case NPCID.MartianSaucerCore:
                    npc.width *= 2;
                    npc.height *= 2;
                    break;

                // Nerf Green Jellyfish stats in pre-Hardmode
                case NPCID.GreenJellyfish:
                    if (!Main.hardMode)
                    {
                        npc.damage = 40;
                        npc.defDamage = npc.damage;
                        npc.defense = 4;
                        npc.defDefense = npc.defense;
                    }
                    break;

                // Make Plantera's Spores immune to damage because otherwise they're pointless
                case NPCID.Spore:
                    npc.dontTakeDamage = true;
                    break;

                // Make Fishron and Anahita Bubbles have actual health in Death Mode
                case NPCID.DetonatingBubble:
                    if (CalamityWorld.death)
                        npc.lifeMax = 300;
                    break;

                default:
                    break;
            }

            // Reduce mech boss HP and damage depending on the new ore progression changes
            if (CalamityServerConfig.Instance.EarlyHardmodeProgressionRework && !BossRushEvent.BossRushActive)
            {
                if (!NPC.downedMechBossAny)
                {
                    if (CalamityNPCTypeSets.Destroyer.Contains(npc.type) || npc.type == NPCID.Probe || CalamityNPCTypeSets.SkeletronPrime.Contains(npc.type) || npc.type == NPCID.Spazmatism || npc.type == NPCID.Retinazer)
                    {
                        double multiplier = Main.expertMode ? EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Expert : EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Classic;
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * multiplier);
                        npc.damage = (int)Math.Round(npc.damage * multiplier);
                        npc.defDamage = npc.damage;
                    }
                }
                else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
                {
                    if (CalamityNPCTypeSets.Destroyer.Contains(npc.type) || npc.type == NPCID.Probe || CalamityNPCTypeSets.SkeletronPrime.Contains(npc.type) || npc.type == NPCID.Spazmatism || npc.type == NPCID.Retinazer)
                    {
                        double multiplier = Main.expertMode ? EarlyHardmodeProgressionReworkSecondMechStatMultiplier_Expert : EarlyHardmodeProgressionReworkSecondMechStatMultiplier_Classic;
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * multiplier);
                        npc.damage = (int)Math.Round(npc.damage * multiplier);
                        npc.defDamage = npc.damage;
                    }
                }
            }

            // Prehardmode mushroom enemy nerfs
            if (!Main.hardMode)
            {
                if (npc.type == NPCID.ZombieMushroom || npc.type == NPCID.ZombieMushroomHat || npc.type == NPCID.AnomuraFungus || npc.type == NPCID.FungiBulb || npc.type == NPCID.MushiLadybug)
                {
                    npc.lifeMax = (int)Math.Round(npc.lifeMax * 0.5);
                    npc.damage = (int)Math.Round(npc.damage * 0.5);
                    npc.defDamage = npc.damage;
                }

                if (npc.type == NPCID.FungiSpore)
                {
                    npc.damage = (int)Math.Round(npc.damage * 0.5);
                    npc.defDamage = npc.damage;
                }
            }

            if (Main.hardMode && CalamityNPCSets.NerfDamageInHardmode[npc.type])
            {
                npc.damage = (int)Math.Round(npc.damage * 0.75);
                npc.defDamage = npc.damage;
            }

            if (DownedBossSystem.downedDoG)
            {
                if (CalamityNPCSets.IsBuffedPumpkinMoonEnemy[npc.type])
                {
                    npc.lifeMax = (int)Math.Round(npc.lifeMax * 3.5);
                    npc.damage += 30;
                    npc.life = npc.lifeMax;
                    npc.defDamage = npc.damage;
                }
                else if (CalamityNPCSets.IsBuffedFrostMoonEnemy[npc.type])
                {
                    npc.lifeMax = (int)Math.Round(npc.lifeMax * 2.5);
                    npc.damage += 30;
                    npc.life = npc.lifeMax;
                    npc.defDamage = npc.damage;
                }
                else if (CalamityNPCSets.IsBuffedSolarEclipseEnemy[npc.type])
                {
                    npc.lifeMax = (int)Math.Round(npc.lifeMax * 5D);
                    npc.damage += 30;
                    npc.life = npc.lifeMax;
                    npc.defDamage = npc.damage;
                }
            }

            if (NPC.downedMoonlord)
            {
                if (CalamityNPCSets.IsBuffedDungeonEnemy[npc.type])
                {
                    npc.lifeMax = (int)Math.Round(npc.lifeMax * 2.5);
                    npc.damage += 30;
                    npc.life = npc.lifeMax;
                    npc.defDamage = npc.damage;
                }
            }
        }
        #endregion

        #region Edit Coin Drops
        private void EditGlobalCoinDrops(NPC npc)
        {
            // Old Rev coin drop math: Normal = 10 Gold, Expert = 25 Gold, Rev = 37 Gold 50 Silver.
            // New Rev coin drop math: Normal = 15 Gold, Expert AND Rev = 22 Gold 50 Silver.
            // Rebalance coin drops so that Normal Mode enemies and bosses drop an adequate amount of coins.

            // Increase Normal Mode coin drops by 1.5x.
            npc.value = (int)(npc.value * NPCValueMultiplier_ClassicCalamity);

            // Change the Expert Mode coin drop multiplier.
            if (Main.expertMode)
            {
                // Undo the Expert Mode coin drop multiplier.
                npc.value = (int)(npc.value / NPCValueMultiplier_ExpertVanilla);

                // Change the Expert Mode coin drop multiplier to the new Calamity amount.
                npc.value = (int)(npc.value * NPCValueMultiplier_ExpertCalamity);
            }
        }
        #endregion

        #region Special Drawing
        public static void DrawGlowmask(NPC npc, SpriteBatch spriteBatch, Texture2D texture = null, bool invertedDirection = false, Vector2 offset = default)
        {
            texture ??= TextureAssets.Npc[npc.type].Value;
            SpriteEffects effects = npc.spriteDirection == 1 ? (invertedDirection ? SpriteEffects.FlipHorizontally : SpriteEffects.None) : (invertedDirection ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            Vector2 screenOffset = npc.IsABestiaryIconDummy ? Vector2.Zero : Main.screenPosition;
            spriteBatch.Draw(texture, npc.Center - screenOffset + offset, npc.frame, npc.GetAlpha(Color.White), npc.rotation, npc.frame.Size() * 0.5f, npc.scale, effects, 0f);
        }

        public static void DrawAfterimage(NPC npc, SpriteBatch spriteBatch, Color startingColor, Color endingColor, Texture2D texture = null, Func<NPC, int, float> rotationCalculation = null, bool directioning = false, bool invertedDirection = false)
        {
            if (NPCID.Sets.TrailingMode[npc.type] != 1)
                return;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == -1 && directioning)
                spriteEffects = SpriteEffects.FlipHorizontally;
            if (invertedDirection)
                spriteEffects ^= SpriteEffects.FlipHorizontally; // Same as x XOR 1, or x XOR TRUE, which inverts the bit. In this case, this reverses the horizontal flip

            // Set the rotation calculation to a predefined value.
            rotationCalculation ??= (nPC, afterimageIndex) => nPC.rotation;

            endingColor.A = 0;
            Color drawColor = npc.GetAlpha(startingColor);
            Texture2D npcTexture = texture ?? TextureAssets.Npc[npc.type].Value;
            Vector2 screenOffset = npc.IsABestiaryIconDummy ? Vector2.Zero : Main.screenPosition;
            int afterimageCounter = 1;
            while (afterimageCounter < NPCID.Sets.TrailCacheLength[npc.type] && CalamityClientConfig.Instance.Afterimages)
            {
                Color colorToDraw = Color.Lerp(drawColor, endingColor, afterimageCounter / (float)NPCID.Sets.TrailCacheLength[npc.type]);
                colorToDraw *= afterimageCounter / (float)NPCID.Sets.TrailCacheLength[npc.type];
                Vector2 imagePosition = npc.oldPos[afterimageCounter] + npc.Size / 2f - screenOffset + Vector2.UnitY * npc.gfxOffY;

                spriteBatch.Draw(npcTexture, imagePosition, npc.frame, colorToDraw, rotationCalculation.Invoke(npc, afterimageCounter), npc.frame.Size() * 0.5f, npc.scale, spriteEffects, 0f);
                afterimageCounter++;
            }
        }
        #endregion

        #region Scale Expert Multiplayer Stats
        private const float VanillaScalingFactor_2Players = 1.35f;
        private const float VanillaScalingFactor_3Players = 1.9166666666666666f;

        /// <summary>
        /// Applies Calamity's adjustments to difficulty-based player count stat scaling for NPCs. Calamity only adjusts the health of NPCs and does not touch any other stats.
        /// </summary>
        /// <param name="npc">The NPC which is having its stats adjusted.</param>
        /// <param name="numPlayers">The number of players considered active for the purposes of stat scaling.</param>
        /// <param name="balance">The vanilla Expert+ multiplayer health scalar value.</param>
        /// <param name="bossAdjustment">An arbitrary float to make Master Mode easier. On Master Mode, it is 0.85, otherwise it is 1.0.</param>
        public override void ApplyDifficultyAndPlayerScaling(NPC npc, int numPlayers, float balance, float bossAdjustment)
        {
            // Do absolutely nothing in single player, or in multiplayer with only one player connected.
            if (Main.netMode == NetmodeID.SinglePlayer || numPlayers <= 1)
                return;

            bool countsAsBoss = npc.boss || NPCID.Sets.ShouldBeCountedAsBoss[npc.type];
            bool scalesLikeBoss = countsAsBoss || CalamityNPCSets.ScalesHealthLikeBoss[npc.type];
            bool isCalamityNPC = npc.ModNPC != null && npc.ModNPC.Mod == CalamityMod.Instance;

            // 14APR2025: Ozzatron: Reworked how Calamity changes the health of Expert+ multiplayer bosses
            // Non-boss enemies that receive scaling in Expert+ are still reduced via the old formula
            //
            // TL;DR:
            // - 2 players goes from 135% health to 175% health
            // - 3 players goes from 191.6% health to 225% health
            // - 4 players and beyond are unedited (4 players is 262.8% for reference)

            // This case applies to all bosses: vanilla, Calamity, and other mods, and anything that is supposed to scale like a boss.
            if (countsAsBoss || scalesLikeBoss)
            {
                double adjustmentFactor = 1.0;

                // The 2-player boss case is too easy; 1.35x health does not even come close to justify being able to respawn.
                if (numPlayers == 2)
                    adjustmentFactor = BalancingConstants.ExpertHealthScalingOverride_2Players / VanillaScalingFactor_2Players;

                // Similarly, the 3-player boss case is too easy, given the considerably higher damage output available.
                else if (numPlayers == 3)
                    adjustmentFactor = BalancingConstants.ExpertHealthScalingOverride_3Players / VanillaScalingFactor_3Players;

                // Cases beyond 3 players are already sufficiently scaled by vanilla and continue to scale harder with more players.

                // Apply the adjustment factor, if any. No other changes are made to bosses or boss-like NPCs.
                npc.life = (int)Math.Round(npc.life * adjustmentFactor);
                return;
            }

            // Do not touch non-boss NPCs from vanilla or other mods.
            if (!isCalamityNPC)
                return;

            // Reduction to multiplayer HP scaling for non-boss Calamity enemies in Expert+
            double scalar;
            switch (numPlayers)
            {
                case 1:
                    scalar = 1.0;
                    break;

                case 2:
                    scalar = 0.9; // 1.8
                    break;

                case 3:
                    scalar = 0.82; // 2.46
                    break;

                case 4:
                    scalar = 0.76; // 3.04
                    break;

                case 5:
                    scalar = 0.71; // 3.55
                    break;

                case 6:
                    scalar = 0.67; // 4.02
                    break;

                default:
                    scalar = 0.64; // 4.48 + 0.64 per player beyond 7
                    break;
            }

            npc.lifeMax = (int)Math.Round(npc.lifeMax * scalar);
        }
        #endregion

        #region Can Hit Player
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (pacified)
                return false;

            if (target.Calamity().prismaticHelmet && !CalamityPlayer.areThereAnyDamnBosses)
            {
                if (npc.lifeMax < 500)
                    return false;
            }

            return true;
        }
        #endregion

        #region Strike NPC
        // Incoming defense to this function is already affected by the vanilla debuffs Ichor (-10) and Betsy's Curse (-40), and cannot be below zero.
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            // Reduce ichor debuff defense reduction from -15 to -10.
            if (npc.ichor)
                modifiers.Defense.Flat += 5;

            // Apply armor penetration based on Calamity debuffs. The hit system manages the sequencing.
            // Ozzatron 05JAN2023: fixed doubled armor pen, this time for real
            int defenseReduction = (wither ? RemsRevenge.WitherDefenseReduction : 0) + miscDefenseLoss;
            modifiers.ArmorPenetration += defenseReduction;

            // DR applies after vanilla defense.
            ApplyDR(npc, ref modifiers);

            // Damage reduction on spawn for certain worm bosses.
            if (CalamityWorld.revenge)
            {
                if (CalamityNPCTypeSets.EaterOfWorlds.Contains(npc.type) && newAI[1] < EaterOfWorldsAI.DRIncreaseTime)
                    modifiers.FinalDamage *= 1f - (float)Math.Sqrt(MathHelper.Lerp(0f, 0.99f, MathHelper.Clamp(1f - newAI[1] / EaterOfWorldsAI.DRIncreaseTime, 0f, 1f)));
                if (CalamityNPCTypeSets.Destroyer.Contains(npc.type) && newAI[1] < DestroyerAI.DRIncreaseTime)
                    modifiers.FinalDamage *= 1f - (float)Math.Sqrt(MathHelper.Lerp(0f, 0.99f, MathHelper.Clamp(1f - newAI[1] / DestroyerAI.DRIncreaseTime, 0f, 1f)));
            }
            if (CalamityNPCTypeSets.AstrumDeus.Contains(npc.type))
            {
                float drTime = newAI[0] != 0f ? 300f : 600f;
                if (newAI[1] < drTime)
                    modifiers.FinalDamage *= 1f - (float)Math.Sqrt(MathHelper.Lerp(0f, 0.99f, MathHelper.Clamp(1f - newAI[1] / drTime, 0f, 1f)));
            }
        }

        // Directly modifies final damage incoming to an NPC based on their DR (damage reduction) stat added by Calamity.
        // This is entirely separate from vanilla's takenDamageMultiplier.
        private void ApplyDR(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (DR <= 0f && KillTime == 0)
                return;

            float finalMultiplier = 1f;

            // If the NPC currently has unbreakable DR, it cannot be reduced by any means.
            float effectiveDR = unbreakableDR ? DR : ApplyDRReduction(npc, DR);

            // DR floor is 0%. Nothing can have negative DR.
            if (effectiveDR <= 0f)
                effectiveDR = 0f;

            // Calculate extra DR based on kill time, similar to the Hush boss from The Binding of Isaac
            bool enragedProvi = npc.type == NPCType<Providence.Providence>() && !ProvUtils.StandardAI();
            if (KillTime > 0 && killTimeTimer < KillTime && !BossRushEvent.BossRushActive && enragedProvi)
            {
                // Set the DR scaling factor
                float DRScalar = 10f;

                // The limit for how much extra DR the boss can have
                float extraDRLimit = (1f - DR) * DRScalar;

                // Ranges from 1 to 0
                float currentHPRatio = npc.life / (float)npc.lifeMax;

                // Ranges from 0 to 1
                float killTimeRatio = killTimeTimer / (float)KillTime;

                // If the player is damaging the boss too quickly
                float extraDRScalar = currentHPRatio + killTimeRatio;
                if (extraDRScalar < 1f)
                {
                    // Ranges from 0 to (extraDRLimit / 2)
                    effectiveDR += extraDRLimit - (extraDRLimit / (1f + (1f - extraDRScalar)));
                }
            }

            // Final DR calculation
            finalMultiplier -= effectiveDR;

            modifiers.FinalDamage *= finalMultiplier;
        }

        //TODO:
        //This will need to be adjusted to use DebuffData in the future.
        //However, we still need to decide what to actually do with these due to the general flattening of DR amounts and removal from most enemies
        //This means that this will be handled in the future
        private float ApplyDRReduction(NPC npc, float DR)
        {
            float calcDR = DR;
            if (absorberAffliction)
                calcDR *= 0.8f;
            if (npc.Calamity().armorCrunch)
                calcDR *= ArmorCrunch.MultiplicativeDamageReductionEnemy;
            if (npc.Calamity().crumble)
                calcDR *= Crumbling.MultiplicativeDamageReductionEnemy;
            if (relicOfResilienceWeakness)
                calcDR *= 0.5f;

            return calcDR;
        }

        public bool IsArmored()
        {
            return unbreakableDR && DR > 0.9f;
        }
        #endregion

        #region Pre AI
        public override bool PreAI(NPC npc)
        {
            // Change Spaz and Ret weaknesses and resistances when phase 2 starts.
            if (npc.type == NPCID.Spazmatism || npc.type == NPCID.Retinazer)
            {
                if (npc.ai[0] >= 2f)
                {
                    VulnerableToCold = null;
                    VulnerableToHeat = null;
                    VulnerableToSickness = false;
                    VulnerableToElectricity = true;
                }
            }

            VulnerabilityHexFireDrawer?.Update();

            if (ManaBurnFireDrawer != null)
            {
                ManaBurnFireDrawer.LocalTimer = 0;
                float power = npc.height / 100f;
                if (power > 2.75f)
                    power = 2.75f;
                ManaBurnFireDrawer.RelativePower = power * MathHelper.Lerp(0.5f, 1.5f, MathHelper.Clamp(manaBurn / manaBurnPeak, 0, 1)) * playerManaBurnIntensity;
                ManaBurnFireDrawer.Update();
            }

            // Decrement each immune timer if it's greater than 0.
            for (int i = 0; i < maxPlayerImmunities; i++)
            {
                if (dashImmunityTime[i] > 0)
                    dashImmunityTime[i]--;
            }

            if (KillTime > 0 || npc.type == NPCType<Draedon>())
            {
                // Apply Boss Effects while any boss NPC is active
                if (!Main.dedServ)
                {
                    if (!Main.LocalPlayer.dead && Main.LocalPlayer.active && Vector2.Distance(Main.LocalPlayer.Center, npc.Center) < BossZenDistance)
                        Main.LocalPlayer.AddBuff(BuffType<BossEffects>(), 2);
                }

                if (npc.type != NPCType<Draedon>())
                {
                    if (killTimeTimer < KillTime)
                        killTimeTimer++;
                }
            }

            if (npc.type == NPCID.TargetDummy || npc.type == NPCType<SuperDummyNPC>())
                npc.dontTakeDamage = CalamityPlayer.areThereAnyDamnBosses || (draedon != -1 && Main.npc[draedon].active);

            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            if (CalamityNPCSets.DealsZeroContactDamage[npc.type] && !(npc.type == NPCID.RuneWizard && Main.zenithWorld))
                npc.damage = 0;

            if (BossRushEvent.BossRushActive && !npc.friendly && !npc.townNPC && !DoesNotDisappearInBossRush)
                BossRushForceDespawnOtherNPCs(npc, Mod);

            #region Fairies Edit
            // Fairies don't run away and are immune to damage while wearing Fairy Boots.
            if (npc.type >= NPCID.FairyCritterPink && npc.type <= NPCID.FairyCritterBlue && (npc.ai[2] < 2f || npc.ai[2] == 7f))
            {
                npc.TargetClosest();
                if (Main.player[npc.target].Calamity().fairyBoots)
                {
                    NPCAimedTarget targetData = npc.GetTargetData();
                    if (targetData.Type == NPCTargetType.Player)
                    {
                        if (Main.player[npc.target].dead)
                            return true;
                    }

                    // Set this to 7 so that they run away when the player takes off their Fairy Boots.
                    npc.ai[2] = 7f;

                    npc.lavaImmune = true;
                    npc.dontTakeDamage = true;
                    npc.noTileCollide = true;
                    npc.rarity = 0;

                    // Teleport to the player if far enough away.
                    if (Vector2.Distance(npc.Center, targetData.Center) > 1000f)
                    {
                        npc.Center = targetData.Center;
                    }

                    // Move towards the player if far enough away.
                    else if (Vector2.Distance(npc.Center, targetData.Center) > 80f)
                    {
                        Rectangle r = Utils.CenteredRectangle(targetData.Center, new Vector2(targetData.Width + 60, targetData.Height / 2));
                        Vector2 closestTargetPoint = r.ClosestPointInRect(npc.Center);
                        Vector2 targetPointDir = npc.DirectionTo(closestTargetPoint) * ((targetData.Velocity.Length() * 0.5f) + 2f);
                        float targetPointDist = npc.Distance(closestTargetPoint);
                        if (targetPointDist > 225f)
                            targetPointDir *= 2f;
                        else if (targetPointDist > 120f)
                            targetPointDir *= 1.5f;

                        npc.velocity = Vector2.Lerp(npc.velocity, targetPointDir, 0.07f);
                    }

                    foreach (NPC k in Main.ActiveNPCs)
                    {
                        if (k != npc && k.aiStyle == NPCAIStyleID.Fairy && Math.Abs(npc.position.X - k.position.X) + Math.Abs(npc.position.Y - k.position.Y) < npc.width * 1.5f)
                        {
                            if (npc.position.Y < k.position.Y)
                                npc.velocity.Y -= 0.05f;
                            else
                                npc.velocity.Y += 0.05f;
                        }
                    }

                    npc.direction = npc.velocity.X >= 0f ? 1 : -1;
                    npc.spriteDirection = -npc.direction;

                    Color dustLerpColor1 = Color.HotPink;
                    Color dustLerpColor2 = Color.LightPink;
                    if (npc.type == NPCID.FairyCritterGreen)
                    {
                        dustLerpColor1 = Color.LimeGreen;
                        dustLerpColor2 = Color.LightSeaGreen;
                    }
                    if (npc.type == NPCID.FairyCritterBlue)
                    {
                        dustLerpColor1 = Color.RoyalBlue;
                        dustLerpColor2 = Color.LightBlue;
                    }

                    if ((int)Main.timeForVisualEffects % 2 == 0)
                    {
                        npc.position += npc.netOffset;
                        Dust dust = Dust.NewDustDirect(npc.Center - new Vector2(2f), 8, 8, DustID.FireworksRGB, 0f, 0f, 200, Color.Lerp(dustLerpColor1, dustLerpColor2, Main.rand.NextFloat()), 0.65f);
                        dust.velocity *= 0f;
                        dust.velocity += npc.velocity * 0.3f;
                        dust.noGravity = true;
                        dust.noLight = true;
                        npc.position -= npc.netOffset;
                    }

                    Lighting.AddLight(npc.Center, dustLerpColor1.ToVector3() * 0.7f);
                    if (!Main.dedServ)
                    {
                        Player localPlayer = Main.LocalPlayer;
                        if (!localPlayer.dead && localPlayer.HitboxForBestiaryNearbyCheck.Intersects(npc.Hitbox))
                            AchievementsHelper.HandleSpecialEvent(localPlayer, 22);
                    }

                    return false;
                }
            }
            #endregion

            return true;
        }
        #endregion

        #region Boss Rush Force Despawn Other NPCs
        private void BossRushForceDespawnOtherNPCs(NPC npc, Mod mod)
        {
            if (BossRushEvent.BossRushStage >= BossRushEvent.Bosses.Count)
                return;

            if (!BossRushEvent.Bosses[BossRushEvent.BossRushStage].HostileNPCsToNotDelete.Contains(npc.type))
            {
                npc.active = false;
                npc.netUpdate = true;
            }
        }
        #endregion

        #region Post AI
        public override void PostAI(NPC npc)
        {
            // Worm heads emit dust when close enough to the player and digging through tiles
            if (npc.type == NPCID.GiantWormHead || npc.type == NPCID.DiggerHead || npc.type == NPCID.DevourerHead ||
                npc.type == NPCID.SeekerHead || npc.type == NPCID.TombCrawlerHead || npc.type == NPCID.BoneSerpentHead ||
                npc.type == NPCID.DuneSplicerHead || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.TheDestroyer)
            {
                Point point = npc.Center.ToTileCoordinates();
                Tile tileSafely = Framing.GetTileSafely(point);
                bool createDust = tileSafely.HasUnactuatedTile && npc.Distance(Main.player[npc.target].Center) < 800f;
                if (createDust)
                {
                    if (Main.rand.NextBool())
                    {
                        Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.TreasureSparkle, 0f, 0f, 150, default, 0.3f);
                        dust.fadeIn = 0.75f;
                        dust.velocity *= 0.1f;
                        dust.noLight = true;
                    }
                }
            }

            // Plants that go through tiles emit spores while inside tiles
            else if (npc.type == NPCID.ManEater || npc.type == NPCID.Snatcher || npc.type == NPCID.AngryTrapper)
            {
                Point point = npc.Center.ToTileCoordinates();
                Tile tileSafely = Framing.GetTileSafely(point);
                bool createDust = tileSafely.HasUnactuatedTile && npc.Distance(Main.player[npc.target].Center) < 800f;
                if (createDust)
                {
                    if (Main.rand.NextBool(10))
                    {
                        Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.JungleSpore, 0f, 0f, 250, default, 0.4f);
                        dust.fadeIn = 0.7f;
                    }
                }
            }

            // Clingers emit cursed fire while inside tiles
            else if (npc.type == NPCID.Clinger)
            {
                Point point = npc.Center.ToTileCoordinates();
                Tile tileSafely = Framing.GetTileSafely(point);
                bool createDust = tileSafely.HasUnactuatedTile && npc.Distance(Main.player[npc.target].Center) < 800f;
                if (createDust)
                {
                    if (Main.rand.NextBool(5))
                    {
                        Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.CursedTorch, 0f, 0f, 100, default, 1.5f);
                        dust.noGravity = true;
                    }
                }

                // Emit cursed flame dust from mouth when about to fire
                else if (npc.localAI[0] > (CalamityWorld.revenge ? RevengeanceAndDeathAI.ClingerShootGateValue_Rev : RevengeanceAndDeathAI.ClingerShootGateValue) - RevengeanceAndDeathAI.ClingerTelegraphTime)
                {
                    Vector2 dustCenter = npc.Center + npc.SafeDirectionTo(Main.player[npc.target].Center, -Vector2.UnitY) * 20f + Main.rand.NextVector2CircularEdge(5f, 5f);
                    Dust dust = Dust.NewDustDirect(dustCenter, 1, 1, DustID.CursedTorch, 0f, 0f, 100, default, 3f);
                    dust.noGravity = true;
                    dust.velocity *= 0f;
                }

                // Reset shoot counter if inside tiles or cannot see the target
                if (Collision.SolidCollision(npc.position, npc.width, npc.height) || !Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    npc.localAI[0] = 0f;
            }

            else if (npc.type == NPCID.IchorSticker)
            {
                // Emit ichor dust from mouth when about to fire
                if (npc.ai[3] > (CalamityWorld.death ? RevengeanceAndDeathAI.IchorStickerShootGateValue_Death : CalamityWorld.revenge ? RevengeanceAndDeathAI.IchorStickerShootGateValue_Rev : RevengeanceAndDeathAI.IchorStickerShootGateValue) - RevengeanceAndDeathAI.IchorStickerTelegraphTime)
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(npc.Center.X - 4f, npc.position.Y + npc.height * 0.7f) + Main.rand.NextVector2CircularEdge(2f, 2f), 1, 1, DustID.Ichor, 0f, 0f, 100, default, 1.5f);
                    dust.noGravity = true;
                    dust.velocity *= 0f;
                }

                // Reset shoot counter if cannot see the target
                if (!Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].head))
                    npc.ai[3] = 0f;
            }
            if (demonicFlames)
                demonicFlamesClearTimer = 0;
            else
                demonicFlamesClearTimer++;
            if (demonicFlamesClearTimer >= 2)
                demonicFlamesBonusDamage = 0;

            if (warbannerBurnTimer > 0)
                warbannerBurnTimer--;
            if (warbannerBurnTimer == 0 && warbannerBurnMarked)
            {
                warbannerBurnTime = 0;
                warbannerBurnDamage = 0;
                warbannerBurnMarked = false;
                warbannerBurnStacks = 0;
            }
            if (warbannerBurnTimer <= 60)
            {
                warbannerBurnStacks = (int)(warbannerBurnStacks * 0.9f);
            }
            if (warbannerBurnMarked)
            {
                int maxStacks = 300; // Time in frames needed to reach max power
                int fastestBurnRate = 2;
                int slowestBurnRate = 15;
                float burnPower = Utils.Remap(warbannerBurnStacks, 0, maxStacks, slowestBurnRate, fastestBurnRate, true);

                float sizeBonus = (1 + Utils.GetLerpValue(0, 170, Math.Max(npc.Hitbox.Width / 2f, npc.Hitbox.Height / 2f)));

                if (!warbannerBurnHideEffects)
                {
                    Lighting.AddLight(npc.Center, Color.Gold.ToVector3() * 0.3f * warbannerBurnIntensity);
                }
                if (warbannerBurnStacks == maxStacks && !warbannerBurnHideEffects)
                {
                    // Sound and visual for hitting max stacks
                    for (int i = 0; i < 15; i++)
                    {
                        Particle spark = new SparkParticle(npc.Center, new Vector2(13, 13).RotatedByRandom(100) * Main.rand.NextFloat(0.4f, 1f), true, 45, 0.85f, Main.rand.NextBool() ? Color.Goldenrod : Color.Orange);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                    SoundStyle fullPower = new("CalamityMod/Sounds/Custom/Providence/ProvidenceBurn");
                    SoundEngine.PlaySound(fullPower with { Volume = 0.7f, Pitch = 0.7f }, npc.Center);
                    warbannerBurnStacks++;
                }
                if (warbannerBurnIntensity > 2.5f && npc.CanBeMoved())
                {
                    npc.velocity *= 1f - 0.25f * Utils.GetLerpValue(2.5f, 3, warbannerBurnIntensity);
                    if (npc.velocity.Length() > 5 && warbannerBurnIntensity > 2.85f) // Repel leaping enemies
                        npc.velocity = -npc.velocity * 0.7f;
                }
                if (warbannerBurnTime == 0)
                {
                    if (!warbannerBurnHideEffects)
                    {
                        int particleLevel = (int)(MathHelper.Clamp((slowestBurnRate - burnPower) * 0.15f, 1, 2) * warbannerBurnIntensity);
                        for (int d = 0; d < particleLevel; d++)
                        {
                            Color color = Main.rand.NextBool() ? Color.Goldenrod : Color.Lerp(Color.OrangeRed, Color.Orange, Main.rand.NextFloat(0, 1)); ;
                            Vector2 sparkPos = npc.Center - warbannerBurnDirection * 220 * Utils.GetLerpValue(0, 200, Math.Max(npc.Hitbox.Width / 2f, npc.Hitbox.Height / 2f));
                            float velAdjust = Main.rand.NextFloat(2, 7) * warbannerBurnIntensity * sizeBonus;
                            Vector2 endVel = warbannerBurnDirection * velAdjust;
                            Vector2 startVel = (warbannerBurnDirection * velAdjust).RotatedByRandom(0.6f * warbannerBurnIntensity);
                            Particle sparks = new VelChangingSpark(sparkPos, startVel, endVel, "CalamityMod/Particles/SmallBloom", Main.rand.Next(18, 22 + 1), Main.rand.NextFloat(0.1f, 0.25f) * sizeBonus, color * 0.75f, new Vector2(0.7f, 1), true, false, 0, false, 0.45f, 0.1f);
                            GeneralParticleHandler.SpawnParticle(sparks);
                            if (Main.rand.NextBool())
                            {
                                Dust lust2 = Dust.NewDustPerfect(sparkPos, DustType<LightDust>(), startVel, Scale: Main.rand.NextFloat(0.5f, 0.9f) * Math.Min(sizeBonus, 1.3f));
                                lust2.noGravity = true;
                                lust2.color = color;
                                lust2.noLightEmittence = true;
                            }
                        }
                    }
                    var player = Main.LocalPlayer;
                    Projectile burnHit = Projectile.NewProjectileDirect(player.GetSource_FromThis(), npc.Center, Vector2.Zero, ProjectileType<WarbannerDamage>(), (int)(warbannerBurnDamage * warbannerBurnIntensity), 0, Main.myPlayer, npc.whoAmI);
                    burnHit.ArmorPenetration = 50;
                    warbannerBurnTime = (int)(burnPower + (3 - warbannerBurnIntensity) * 4);
                }
                warbannerBurnTime--;
            }
            float fadeSpeed = 0.15f;
            if (bane)
                baneVisual = MathHelper.Lerp(baneVisual, 1, fadeSpeed);
            else if (baneVisual > 0)
                baneVisual = MathHelper.Lerp(baneVisual, 0, fadeSpeed);

            if (veriumDoomTimer > 0)
                veriumDoomTimer--;
            if (veriumDoomTimer == 0 && veriumDoomMarked)
            {
                for (int d = 0; d < 14 + veriumDoomStacks; d++)
                {
                    Particle sparks = new LineParticle(npc.Center, new Vector2(Main.rand.NextFloat(-9f, 9f), Main.rand.NextFloat(-9f, 9f)), false, 45, 0.9f, Main.rand.NextBool() ? Color.Cyan : Color.SkyBlue);
                    GeneralParticleHandler.SpawnParticle(sparks);
                }

                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/NPCHit/CryogenHit", 3) { Volume = 0.6f }, npc.Center);
                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ProjectileType<DirectStrike>(), 100 + (15 * veriumDoomStacks), 0, Main.myPlayer, npc.whoAmI);

                veriumDoomMarked = false;
                veriumDoomStacks = 0;
            }

            // Amidias' Spark spark spawning
            if (shocked > 0)
            {
                var player = Main.LocalPlayer;
                int frequency = 15;

                // Spawn sparks from the enemy
                if (player.miscCounter % frequency == 0)
                {
                    int sDamage = 10;
                    Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.Pi) * 5f;
                    Projectile spark = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), npc.Center, velocity, ProjectileType<GenericElectricSpark>(), sDamage, 0f, player.whoAmI, 0f, 1f);
                    spark.timeLeft = 120;
                    spark.penetrate = 3;
                }
            }
            if (hyperiusMarked)
            {
                if (hyperiusFxTimer < 20)
                    hyperiusFxTimer++;
                else if (hyperiusFxTimer > 20)
                    hyperiusFxTimer = (int)Utils.Lerp(hyperiusFxTimer, 20, 0.2f);

                float threshold = hyperiusDamage / (float)(npc.lifeMax);
                int overflowSpeed = (int)Utils.Remap(threshold, HyperiusLifePercentThreshold, 0.35f, 1, 34);
                if (threshold > HyperiusLifePercentThreshold) // If the stored damage is greater than the life % cap of the target's max health, rapily deal a % of the stored damage to the enemy to drain it
                {
                    bool enemyIsNotArmored = npc.defense < 1000 && !unbreakableDR && DR <= 0.9f && !npc.dontTakeDamage && !npc.immortal;
                    if (enemyIsNotArmored)
                        hyperiusOverflowTimer -= overflowSpeed;
                    if (hyperiusOverflowTimer <= 0)
                    {
                        hyperiusOverflowTimer = hyperiusOverflowTime;

                        float damagePercent = 0.07f; // The % of stacks drained when you're over the cap
                        int damage = Math.Max((int)(hyperiusDamage * damagePercent), 1);
                        hyperiusDamage -= damage;

                        // Spawn "bleed" hit
                        // Uses a seperate projectile so that the hit takes defense and DR into account
                        Projectile overflow = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ProjectileType<HyperiusBleed>(), damage, 0, -1, npc.whoAmI);
                        overflow.DamageType = DamageClass.Ranged;
                        if (hyperiusFxTimer >= 20)
                            hyperiusFxTimer = 35;

                        if (hyperiusDamage <= 0)
                        {
                            hyperiusDamage = 0;
                            hyperiusMarked = false;
                        }
                    }
                }
            }
            else if (hyperiusFxTimer > 0)
                hyperiusFxTimer--;

            if (laserBurnTimer > 0)
                laserBurnTimer--;
            if ((laserBurnTimer <= 0 || laserBurnDamage >= npc.life * 1.5f) && laserBurnMarked && laserBurnType > 0)
            {
                if (laserBurnType == 1) // Applied damage
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ProjectileType<DirectStrike>(), laserBurnDamage, 0, Main.myPlayer, npc.whoAmI);
                if (laserBurnType == 2) // Flat damage + stacks
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ProjectileType<DirectStrike>(), 70 + (20 * laserBurnStacks), 0, Main.myPlayer, npc.whoAmI);

                for (int d = 0; d < (int)(7 + laserBurnStacks * 0.4f); d++)
                {
                    Vector2 partVel = (new Vector2(10) * (laserBurnStacks * 0.025f)).RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat(0.2f, 1f);
                    Particle spark2 = new CustomSpark(npc.Center, partVel, "CalamityMod/Particles/BloomLineSoftEdge", false, 12, Main.rand.NextFloat(0.02f, 0.03f), Effects.ArsenalEffects.ArsenalLaserColor * 0.8f, new Vector2(1, 1), true, false, 0, false, false, 1f);
                    GeneralParticleHandler.SpawnParticle(spark2);

                    Vector2 dustVel = (Vector2.UnitX * 5f * (laserBurnStacks * 0.05f)).RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat(0.85f, 1f);
                    Dust dust = Dust.NewDustPerfect(npc.Center, Effects.ArsenalEffects.ArsenalLaserDust, dustVel, 0, Color.Red, Main.rand.NextFloat(0.65f, 0.8f));
                    dust.noGravity = true;
                }

                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/LaserBurn") { Volume = 0.6f, Pitch = Main.rand.NextFloat(-0.15f, 0.15f) }, npc.Center);
                laserBurnMarked = false;
                laserBurnStacks = 0;
                laserBurnTimer = 0;
                laserBurnDamage = 0;
            }

            if (demonSwordImpales > 0 && npc.CanBeMoved())
            {
                npc.velocity *= Utils.Remap(demonSwordImpales, 1, 5, 0.95f, 0.3f, true);
                if (impalePacketTimer > 30) // There's probably a better solution, but this is the best for now
                {
                    npc.SyncMotionToServer();
                    impalePacketTimer = 0;
                }
            }
            impalePacketTimer++;

            // Apply slowing debuff effects, will not apply to bosses
            if (!(CalamityNPCSets.ImmuneToSlowsAndOtherSpecialEffects[npc.type] || npc.boss))
            {
                // Slowing debuffs which set a velocity hard cap take priority first.
                if (vulnerabilityHex)
                    npc.velocity = Vector2.Clamp(npc.velocity, new Vector2(-Calamity.MaxNPCSpeed), new Vector2(Calamity.MaxNPCSpeed, 10f));

                // Then debuffs which apply a multiplier to velocity.
                // These multipliers can stack with each other, even if you'll rarely see this on a boss.
                float velocitySlownessFactor = 1f;

                if (temporalSadness)
                    velocitySlownessFactor += 0.2f;

                if (timeDistortion)
                    velocitySlownessFactor += 0.15f;

                if (webbed)
                    velocitySlownessFactor += 0.15f;

                if (frozen)
                {
                    float baseSlownessFactor = 0.1f;
                    if (VulnerableToCold.HasValue)
                    {
                        if (VulnerableToCold.Value)
                            baseSlownessFactor = 0.4f;
                        else
                            baseSlownessFactor = 0.025f;
                    }
                    velocitySlownessFactor += baseSlownessFactor;
                }

                if (pearlAura)
                    velocitySlownessFactor += 0.1f;

                if (eutrophication)
                {
                    float baseSlownessFactor = 0.05f;
                    if (VulnerableToWater.HasValue)
                    {
                        if (VulnerableToWater.Value)
                            baseSlownessFactor = 0.2f;
                        else
                            baseSlownessFactor = 0.0125f;
                    }
                    velocitySlownessFactor += baseSlownessFactor;
                }

                if (galvanicCorrosion)
                {
                    float baseSlownessFactor = 0.05f;
                    if (VulnerableToElectricity.HasValue)
                    {
                        if (VulnerableToElectricity.Value)
                            baseSlownessFactor = 0.2f;
                        else
                            baseSlownessFactor = 0.0125f;
                    }
                    velocitySlownessFactor += baseSlownessFactor;
                }

                if (vaporfied)
                    velocitySlownessFactor += 0.05f;

                // Divide 1 by the slowness factor to get the amount to slow by.
                // This scales with diminishing returns, though getting slowed every frame means they quickly slow down either way.
                velocitySlownessFactor = 1f / velocitySlownessFactor;
                npc.velocity *= velocitySlownessFactor;
            }

            // Auric Ore/Repulsers reject Town NPCs and dummies
            if ((NPCID.Sets.ActsLikeTownNPC[npc.type] || npc.townNPC) && !npc.dontTakeDamage || npc.type == NPCType<SuperDummyNPC>())
            {
                int auricOreID = TileType<AuricOre>();
                int auricRepulserID = TileType<AuricRepulserPanelTile>();

                // Get a list of tiles near the npc
                // This is just Collision.GetEntityTiles but with a larger detection square because the sheer speed from auric boosts causes the detection to fail at higher speeds
                List<Point> EdgeTiles = [];
                int extraDist = (int)(8 * npc.velocity.Length() / 6) + 1;
                int left = (int)npc.position.X - extraDist;
                int up = (int)npc.position.Y - extraDist;
                int right = (int)npc.Right.X + extraDist;
                int down = (int)npc.Bottom.Y + extraDist;
                if (left % 16 == 0)
                    left--;
                if (up % 16 == 0)
                    up--;
                if (right % 16 == 0)
                    right++;
                if (down % 16 == 0)
                    down++;

                int width = right / 16 - left / 16;
                int height = down / 16 - up / 16;
                left /= 16;
                up /= 16;
                for (int i = left; i <= left + width; i++)
                {
                    EdgeTiles.Add(new Point(i, up));
                    EdgeTiles.Add(new Point(i, up + height));
                }

                for (int j = up; j < up + height; j++)
                {
                    EdgeTiles.Add(new Point(left, j));
                    EdgeTiles.Add(new Point(left + width, j));
                }
                foreach (Point touchedTile in EdgeTiles)
                {
                    Tile tile = Framing.GetTileSafely(touchedTile);
                    if (!tile.HasTile || !tile.HasUnactuatedTile)
                        continue;

                    if (tile.TileType != auricOreID && tile.TileType != auricRepulserID)
                        continue;

                    // Force Auric Ore to animate with its crackling electricity
                    if (tile.TileType == auricOreID)
                    {
                        AuricOre.Animate = true;
                    }

                    var yeetVec = Vector2.Normalize(npc.Center - touchedTile.ToWorldCoordinates());
                    npc.velocity += yeetVec * 20f;
                    // Speed must be clamped or they start clipping through tiles very easily
                    float clampedSpeed = MathHelper.Clamp(npc.velocity.Length(), -40, 40);
                    npc.velocity = npc.velocity.SafeNormalize(Vector2.Zero) * clampedSpeed;
                    if (tile.TileType == auricOreID)
                    {
                        npc.SimpleStrikeNPC((int)(npc.lifeMax * 0.2f), 0);
                        npc.AddBuff(BuffType<AuricRebuke>(), 120);
                    }
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/ExoMechs/TeslaShoot1"), npc.Center);
                    break;
                }
            }
        }
        #endregion

        #region On Hit Player
        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage <= 0)
                return;

            if (target.Calamity().sulphurSet)
                npc.AddBuff(BuffID.Poisoned, 60);

            if (target.Transformation().Type == ItemType<Popo>())
            {
                if (npc.type == NPCID.Demon || npc.type == NPCID.VoodooDemon || npc.type == NPCID.RedDevil)
                    target.AddBuff(BuffType<PopoNoselessBuff>(), 36000);
            }

            switch (npc.type)
            {
                case NPCID.DevourerHead:
                case NPCID.FaceMonster:
                    target.AddBuff(BuffID.Weak, 180);
                    break;

                case NPCID.ArmoredViking:
                    if (Main.rand.NextBool(6))
                        target.AddBuff(BuffID.BrokenArmor, 1800);
                    break;

                case NPCID.IlluminantBat:
                    if (Main.rand.NextBool(14))
                        target.AddBuff(BuffID.Confused, 180);
                    break;

                case NPCID.Piranha:
                    target.AddBuff(BuffID.Bleeding, 180);
                    break;

                case NPCID.Arapaima:
                case NPCID.BloodFeeder:
                    target.AddBuff(BuffID.Bleeding, 300);
                    break;

                case NPCID.ShadowFlameApparition:
                    target.AddBuff(BuffType<Shadowflame>(), 120);
                    break;

                case NPCID.ChaosBall:
                    if (Main.hardMode || CalamityPlayer.areThereAnyDamnBosses)
                        target.AddBuff(BuffType<Shadowflame>(), 120);
                    break;

                case NPCID.Golem:
                    if (CalamityWorld.revenge)
                        target.AddBuff(BuffType<ArmorCrunch>(), 480);
                    break;

                case NPCID.GolemFistRight:
                case NPCID.GolemFistLeft:
                    if (CalamityWorld.revenge)
                        target.AddBuff(BuffType<ArmorCrunch>(), 240);
                    break;

                case NPCID.BloodNautilus:
                    target.AddBuff(BuffType<BurningBlood>(), 300);
                    break;

                case NPCID.GoblinShark:
                case NPCID.BloodEelHead:
                    target.AddBuff(BuffType<BurningBlood>(), 180);
                    break;

                case NPCID.Hellbat:
                    if (Main.expertMode)
                        target.AddBuff(BuffID.OnFire, 120);
                    break;

                case NPCID.Lavabat:
                    target.AddBuff(BuffID.OnFire, 300);
                    break;

                case NPCID.RuneWizard:
                    if (Main.zenithWorld)
                        target.AddBuff(BuffType<MiracleBlight>(), 600);
                    break;

                default:
                    break;
            }

            // GFB Brain and its Creepers can inflict literally any buff in the game
            // Yes this includes pets, light pets, mounts, whip tags, endgame debuffs, anything!
            if ((npc.type == NPCID.BrainofCthulhu || npc.type == NPCID.Creeper) && Main.zenithWorld)
            {
                int buffType = Main.rand.Next(BuffLoader.BuffCount);
                target.AddBuff(buffType, Main.rand.Next(300, 601));
            }
        }
        #endregion

        #region On Hit NPC
        public override void OnHitNPC(NPC npc, NPC target, NPC.HitInfo hit)
        {
            if (target.ModNPC is SunkenSeaNPC ssnpc)
                ssnpc.OnHitByNPC(npc);
        }
        #endregion

        #region Modify Hit
        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            List<int> SharkIDs =
            [
                NPCID.Shark,
                NPCID.DukeFishron,
                NPCID.Sharkron,
                NPCID.Sharkron2,
                NPCID.SandShark,
                NPCID.SandsharkCorrupt,
                NPCID.SandsharkCrimson,
                NPCID.SandsharkHallow,
                NPCID.GoblinShark,
                NPCType<FusionFeeder>(),
                NPCType<GreatSandShark.GreatSandShark>(),
                NPCType<Mauler>(),
                NPCType<OldDuke.OldDuke>(),
                NPCType<SulphurousSharkron>(),
                NPCType<ReaperShark>()
            ];

            // Kaguya hair boom GIF
            if (SharkIDs.Contains(npc.type) && target.name == "Rebecca" && Main.zenithWorld)
            {
                SoundEngine.PlaySound(AresGaussNuke.NukeExplosionSound, target.Center);
                Main.LocalPlayer.SetScreenshake(12f);

                target.KillMe(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.Rebecca").ToNetworkText(target.name)), 1000.0, 0);
                modifiers.SourceDamage *= target.statLifeMax2 * Main.rand.NextFloat(3f, 6f);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile explosion = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), target.Center, Vector2.Zero, ProjectileType<ScorpioLargeRocket>(), 9999, 0f, Main.myPlayer, ItemID.MiniNukeII, 0.01f);
                    explosion.friendly = false;
                    explosion.hostile = true;
                    explosion.timeLeft = 5;
                }
            }
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (markedForDeath)
                modifiers.SourceDamage *= 1.1f;

            if (modPlayer.camper && !player.StandingStill())
                modifiers.SourceDamage *= 0.5f;

            // Hide combat text so we can draw our own for armored NPCs
            if (IsArmored())
                modifiers.HideCombatText();

            // True melee resists
            // TODO: This should probably be moved into BalancingChangesManager
            if (item.CountsAsClass<MeleeDamageClass>() && item.type != ItemType<InfernaCutter>())
            {
                float damageMult = 1f;
                if (npc.type == NPCType<Crabulon.Crabulon>())
                    damageMult = 0.8f;
                else if (CalamityNPCTypeSets.EaterOfWorlds.Contains(npc.type) || npc.type == NPCID.Creeper || npc.type == NPCType<AstrumAureus.AstrumAureus>())
                    damageMult = 0.75f;
                else if (CalamityNPCTypeSets.Perforators.Contains(npc.type) || CalamityNPCTypeSets.AquaticScourge.Contains(npc.type) || CalamityNPCTypeSets.Destroyer.Contains(npc.type) ||
                    CalamityNPCTypeSets.Ravager.Contains(npc.type) || CalamityNPCTypeSets.AstrumDeus.Contains(npc.type) || CalamityNPCTypeSets.StormWeaver.Contains(npc.type) ||
                    npc.type == NPCType<ProfanedRocks>() || npc.type == NPCType<DarkEnergy>())
                    damageMult = 0.5f;
                else if (CalamityNPCTypeSets.Thanatos.Contains(npc.type))
                    damageMult = 0.35f;

                modifiers.SourceDamage *= damageMult;
            }
        }
        #endregion

        #region Modify Hit By Projectile
        public static bool DisableMultWhipTag = false;
        // This bool does nothing on the main branch, it's just here so that CalTestHelpers doesn't crash searching for it
        // If you want to mess with this to test whips, please do so in the summoner branch - Shade
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            // Hide combat text so we can draw our own for armored NPCs
            if (IsArmored())
                modifiers.HideCombatText();

            // Block natural falling stars from killing boss spawners randomly
            if (projectile.type == ProjectileID.FallingStar && projectile.damage >= 1000 && (npc.type == NPCType<PerforatorCyst>() || npc.type == NPCType<HiveTumor>() || npc.type == NPCType<LeviathanStart>()))
                modifiers.SourceDamage *= 0f;

            // Supercrits
            var cgp = projectile.Calamity();
            if (cgp.supercritHits != 0)
            {
                cgp.supercritHits--;
                float critOver100 = (projectile.ContinuouslyUpdateDamageStats ? player.GetCritChance(projectile.DamageType) : projectile.CritChance) - 100f;

                // Supercrits can "supercrit" over and over for each extra 100% critical strike chance.
                // For example if you have 716% critical strike chance, you are guaranteed +700% damage and then have a 16% chance for +800% damage instead.
                if (critOver100 > 0f)
                {
                    int supercritLayers = (int)(critOver100 / 100f);
                    float lastLayerCritChance = critOver100 % 100f;
                    // Roll for the remaining crit chance
                    if (Main.rand.NextFloat(100f) <= lastLayerCritChance)
                        ++supercritLayers;

                    // 08MAR2025: Ozzatron: changed supercrit implementation to actually increase crit multiplier instead of multiplying source damage
                    // This means supercrits don't affect on-hits, just like regular crits don't
                    //
                    // Apply supercrit damage as a direct increase to the critical strike damage multiplier, which starts at 2.0 (aka 200%).
                    modifiers.CritDamage += supercritLayers;
                }
            }

            // 08MAR2025: Simplistic crit damage increase. Doesn't force a crit, though you can do that separately.
            modifiers.CritDamage += cgp.bonusCritDamage;

            //
            // DAAWNLIGHT SPIRIT ORIGIN AIM IMPLEMENTATION
            //
            if (modPlayer.spiritOrigin && projectile.CountsAsClass<RangedDamageClass>())
            {
                int bullseyeType = ProjectileType<SpiritOriginBullseye>();
                Projectile bullseye = null;
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (p.type != bullseyeType || p.owner != player.whoAmI)
                        continue;

                    // Only choose a bullseye if it is attached to the NPC that is being hit.
                    if (npc.whoAmI == (int)p.ai[0])
                    {
                        bullseye = p;
                        break;
                    }
                }

                // Don't allow large hitbox projectiles or explosions to "snipe" enemies.
                // Hitbox criteria were changed to allow long one dimensional projectiles so that Condemnation would work.
                bool acceptableVelocity = projectile.velocity != Vector2.Zero;
                bool acceptableHitbox = (projectile.width <= 36) || (projectile.height <= 36);
                
                if (bullseye != null && acceptableVelocity && acceptableHitbox && !CalamityProjectileSets.DaawnlightBlacklist[projectile.type])
                {
                    // Bullseyes are visually different on bosses and thus have larger hitboxes.
                    float bullseyeRadius = npc.IsABoss() ? DaawnlightSpiritOrigin.BossBullseyeRadius : DaawnlightSpiritOrigin.RegularEnemyBullseyeRadius;

                    // Do some geometry + trig to determine if the projectile WOULD hit the bullseye, even if it's about to be deleted on-hit.
                    // This is the equivalent of drawing a laser sight from the projectile along its velocity vector and seeing if it crosses the bullseye's hitbox.
                    // To do this more reliably, we back the projectile up quite a distance.
                    Vector2 normVelocity = projectile.velocity.SafeNormalize(Vector2.UnitY);
                    Vector2 backedUpPosition = projectile.Center - 160f * normVelocity;
                    Vector2 directionToBullseyeCenter = (bullseye.Center - backedUpPosition).SafeNormalize(Vector2.UnitY);
                    Vector2 perp = directionToBullseyeCenter.RotatedBy(MathHelper.PiOver2);
                    // Double the radius is given so that the cosine break-even point is right at the edge of the hitbox.
                    Vector2 comparisonPointOne = bullseye.Center + perp * 2f * bullseyeRadius;
                    Vector2 comparisonPointTwo = bullseye.Center - perp * 2f * bullseyeRadius;
                    Vector2 dirToPointOne = (comparisonPointOne - backedUpPosition).SafeNormalize(-Vector2.UnitX);
                    Vector2 dirToPointTwo = (comparisonPointTwo - backedUpPosition).SafeNormalize(Vector2.UnitX);

                    // Law of cosines: (A dot B) = |A| * |B| * cos(theta)
                    // where theta is the angle between the two vectors A and B.
                    // cos(theta) approaches one as the angle approaches zero, so an angle is smaller if the cos is bigger.
                    // If the angle to the bullseye's center is smaller than the angle to both the comparison points, it's a hit.
                    float dotCenter = Vector2.Dot(normVelocity, directionToBullseyeCenter);
                    float dotOne = Vector2.Dot(normVelocity, dirToPointOne);
                    float dotTwo = Vector2.Dot(normVelocity, dirToPointTwo);
                    bool willStrikeBullseye = dotCenter > dotOne && dotCenter > dotTwo;

                    // If a bullseye is triggered, set it as hit.
                    if (willStrikeBullseye)
                    {
                        // 08OCT2024: Ozzatron: this can be abused by firing a ton of shots then hotswapping to AMR while they are in flight
                        // we will need IEntitySource item use time provenance to fix this, and even that is unreliable with holdouts
                        modPlayer.spiritOriginCritBoost += player.HeldItem.useTime;

                        if (bullseye.ai[2] == 0f)
                        {
                            bullseye.timeLeft = DaawnlightSpiritOrigin.BullseyeHitLifetime;
                            bullseye.ai[2] = 1f;
                        }

                        if (Main.rand.NextBool(5))
                        {
                            int randomStarAmount = Main.rand.Next(3, 6);
                            float randomCircleRotation = Main.rand.NextFloat(MathHelper.TwoPi);
                            for (int i = 0; i < randomStarAmount; i++)
                            {
                                Particle fancyStars = new FancyStars(
                                bullseye.Center,
                                Main.rand.NextFloat(MathHelper.TwoPi) * Main.rand.NextBool().ToDirectionInt(),
                                Main.rand.NextFloat(0.42f, 0.63f),
                                (MathHelper.TwoPi / randomStarAmount * i).ToRotationVector2().RotatedBy(randomCircleRotation).RotatedByRandom(MathHelper.ToRadians(30f)) * Main.rand.NextFloat(7f, 12f),
                                Main.rand.NextFloat(0.1f, 0.5f),
                                55,
                                new Color(Main.rand.Next(256), Main.rand.Next(256), Main.rand.Next(256)) * 1.2f);
                                GeneralParticleHandler.SpawnParticle(fancyStars);
                            }
                        }

                        bullseye.netUpdate = true;
                    }
                }
            }

            if (!projectile.npcProj && !projectile.trap)
            {
                // Plague Reaper deals extra damage to Plagued enemies
                if (projectile.CountsAsClass<RangedDamageClass>() && modPlayer.plagueReaper && plague)
                    modifiers.SourceDamage *= PlagueReaperMask.SetBonusPlaguedRangedDamageMult;

                // True Vulnerability Hex causes enemies to take 1.15x damage, 2.5x from Calamity itself
                if (trueVulnerabilityHex)
                    modifiers.SourceDamage *= (projectile.type == ProjectileType<DirectStrike>() && projectile.ai[1] == 255f) ? 2.5f : 1.15f;
            }

            // Apply balancing resists/vulnerabilities.
            BalancingChangesManager.ApplyFromProjectile(npc, ref modifiers, projectile);

            if (CalamityProjectileSets.ResistedExplosiveProjectile[projectile.type])
            {
                // Eater of Worlds has a vanilla resist in Expert+, this gives it to him in Normal mode
                // Note that Calamity reduces the vanilla resist from 80% to 67%
                bool hasResist = CalamityNPCTypeSets.EaterOfWorlds.Contains(npc.type) && !Main.expertMode;
                // Add a resist for BoC's creepers and Prehardmode worm bosses
                if (npc.type == NPCID.Creeper || CalamityNPCTypeSets.DesertScourge.Contains(npc.type) || CalamityNPCTypeSets.Perforators.Contains(npc.type))
                    hasResist = true;
                if (hasResist)
                    modifiers.SourceDamage *= 0.33f;
            }

            if (markedForDeath)
                modifiers.SourceDamage *= 1.1f;

            if (modPlayer.camper && !player.StandingStill())
                modifiers.SourceDamage *= 0.5f;

            // Reduce damage of summon weapons while using Guardians relics
            if ((projectile.minion || ProjectileID.Sets.MinionShot[projectile.type] || projectile.sentry || ProjectileID.Sets.SentryShot[projectile.type]) && (player.ownedProjectileCounts[ProjectileType<RelicOfDeliveranceSpear>()] > 0 || player.ownedProjectileCounts[ProjectileType<RelicOfConvergenceCrystal>()] > 0 || (player.Calamity().rOfResilienceCooldown == 0 && player.HeldItem.type == ItemType<RelicOfResilience>())))
                modifiers.SourceDamage *= 0.1f;

            // Doze apr-6-2025: With the summon tag system we now have this is unnecessary and very likely causes issues on MP, so I'm commenting it out for the time being. Once further testing is done, delete it entirely.
            // Delete Ardor Blossom sparks and buff if hit by something that isn't a minion or sentry while not having Ardor Blossom Star in hand.
            /*if (npc.HasBuff<ArdorBlossomSpark>() && player.HeldItem.type != ModContent.ItemType<ArdorBlossomStar>() && !projectile.minion && !ProjectileID.Sets.MinionShot[projectile.type] && !projectile.sentry)
            {
                npc.RequestBuffRemoval(ModContent.BuffType<ArdorBlossomSpark>());
                //Remove all embers from this enemy
                for (int k = 0; k < Main.maxProjectiles; k++)
                {
                    if (Main.projectile[k].active && Main.projectile[k].type == ModContent.ProjectileType<ArdorBlossomStarSpark>() && Main.projectile[k].ai[0] == 1f && Main.projectile[k].ai[1] == npc.whoAmI && Main.projectile[k].owner == player.whoAmI)
                        Main.projectile[k].Kill();
                }
            }*/
            // Handle summon tag effects
            if (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type] || projectile.sentry || ProjectileID.Sets.SentryShot[projectile.type])
                EditSummonTagDamage(projectile, npc, ref modifiers);
        }
        #endregion

        #region OnHitBy overrides
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damagedone)
        {
            if (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type] || projectile.sentry || ProjectileID.Sets.SentryShot[projectile.type])
                SummonTagOnHitEffects(npc, projectile, hit, damagedone);

            if (IsArmored())
                CombatText.NewText(npc.Hitbox, Color.Gray, damagedone, hit.Crit);

            if (projectile.type == ProjectileType<HyperiusDamage>() || projectile.type == ProjectileType<HyperiusBleed>())
            {
                float rate = Main.GlobalTimeWrappedHourly * 3f;
                List<Color> eColors =
                [
                    Color.Yellow,
                    Color.Magenta,
                    Color.Red,
                    Color.Cyan,
                    Color.Lime
                ];
                int colorIndex = (int)(rate / 2 % eColors.Count);
                Color currentColor = eColors[colorIndex];
                Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
                Color usedColor = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : rate % 1f);

                CombatText.NewText(npc.Hitbox, usedColor, damagedone, hit.Crit, true);
            }
        }
        #endregion

        #region Summon Tag
        //doze 03-15-2025: A full refactor of the summon tag system to make it easier to use and more flexible. Ping me with any questions.
        private void EditSummonTagDamage(Projectile proj, NPC npc, ref NPC.HitModifiers modifiers)
        {
            // Don't run on non-player-owned projectiles.
            if (proj.npcProj || proj.trap || proj.owner == -1)
                return;

            var player = Main.player[proj.owner];
            var modPlayer = player.Calamity();

            float critChance = modPlayer.bonusCritTag;
            float TagDamageMult = ProjectileID.Sets.SummonTagDamageMultiplier[proj.type];

            TagDamageMult += modPlayer.bonusMultTag;
            modifiers.FlatBonusDamage += modPlayer.bonusFlatTag;

            for (int i = 0; i < NPC.maxBuffs; i++)
            {
                if (npc.buffTime[i] >= 1)
                {
                    int type = npc.buffType[i];
                    var tag = CalamityBuffSets.SummonTagDebuff[type];
                    if (tag is not null)
                        tag.TagModifyHitEffects(proj, npc, ref modifiers, ref TagDamageMult, ref critChance);
                }
            }

            // Used to convert all multiplicative tag into crit chance and vice-versa. If both force tag crit and multiplicative are applied, chooses one at random.
            if (modPlayer.forceSummonTagCrit && !(modPlayer.forceSummonTagMultiplicative && Main.rand.NextBool()))
            {
                critChance += modifiers.ScalingBonusDamage.Value;
                modifiers.ScalingBonusDamage += -modifiers.ScalingBonusDamage.Value;

            }
            else if (modPlayer.forceSummonTagMultiplicative)
            {
                modifiers.ScalingBonusDamage += critChance;
                critChance = 0;
            }

            // Currently doesn't support more than 100% crit chance, todo if something does more than +100% tag damage
            if (Main.rand.NextFloat() < critChance)
                modifiers.SetCrit();
        }

        // This is for whip tag effects that run on hit and don't modify the damage of the hit.
        private void SummonTagOnHitEffects(NPC npc, Projectile projectile, NPC.HitInfo hit, int damagedone)
        {
            // Don't run on non-player-owned projectiles.
            if (projectile.npcProj || projectile.trap || projectile.owner == -1)
                return;

            Player player = Main.player[projectile.owner];

            for (int i = 0; i < NPC.maxBuffs; i++)
            {
                if (npc.buffTime[i] >= 1)
                {
                    int type = npc.buffType[i];
                    var tag = CalamityBuffSets.SummonTagDebuff[type];
                    if (tag is not null)
                        tag.TagOnHit(npc, projectile, hit, damagedone);
                }
            }
        }
        #endregion

        #region Check Dead
        public override bool CheckDead(NPC npc)
        {
            if (npc.lifeMax > 1000 && npc.type != NPCID.DungeonSpirit &&
                npc.type != NPCType<PhantomSpirit>() &&
                npc.type != NPCType<PhantomSpiritS>() &&
                npc.type != NPCType<PhantomSpiritM>() &&
                npc.type != NPCType<PhantomSpiritL>() &&
                npc.value > 0f && !npc.boss && npc.HasPlayerTarget &&
                NPC.downedMoonlord &&
                Main.player[npc.target].ZoneDungeon)
            {
                // This value can change by (on average) 0.75x or 1.25x depending on your having positive or negative luck
                int baseValue = Main.expertMode ? 4 : 6;

                if (Main.player[npc.target].RollLuck(baseValue) == 0 && Main.wallDungeon[Main.tile[(int)npc.Center.X / 16, (int)npc.Center.Y / 16].WallType])
                {
                    int randomType = Utils.SelectRandom(Main.rand,
                    [
                        NPCType<PhantomSpirit>(),
                        NPCType<PhantomSpiritS>(),
                        NPCType<PhantomSpiritM>(),
                        NPCType<PhantomSpiritL>()
                    ]);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, randomType);
                }
            }
            return true;
        }
        #endregion

        #region Hit Effect
        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            if (npc.life <= 0 && npc.Organic() && ashesOnDeath > 0)
                DeathAshParticle.CreateAshesFromNPC(npc, Vector2.Zero);

            // Cultist shield flicker
            if (npc.type == NPCID.CultistBoss)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    newAI[1] = 35f;
                    npc.netUpdate = true;
                }
            }

            if (CalamityWorld.revenge)
            {
                switch (npc.type)
                {
                    case NPCID.PlanterasTentacle:
                        if (npc.life <= 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + npc.width / 2), (int)(npc.position.Y + npc.height), NPCType<PlanterasFreeTentacle>());
                        }
                        break;

                    case NPCID.MotherSlime:
                        if (npc.life <= 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int slimeAmt = Main.rand.Next(2) + 2; // 2 to 3 extra
                                for (int s = 0; s < slimeAmt; s++)
                                {
                                    int slime = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)(npc.position.Y + npc.height), NPCID.BlueSlime, 0, 0f, 0f, 0f, 0f, 255);
                                    NPC npc2 = Main.npc[slime];
                                    npc2.SetDefaults(NPCID.BabySlime);
                                    npc2.velocity.X = npc.velocity.X * 2f;
                                    npc2.velocity.Y = npc.velocity.Y;
                                    npc2.velocity.X += Main.rand.Next(-20, 20) * 0.1f + s * npc.direction * 0.3f;
                                    npc2.velocity.Y -= Main.rand.Next(10) * 0.1f + s;
                                    npc2.ai[0] = -1000 * Main.rand.Next(3);

                                    if (Main.dedServ && slime < Main.maxNPCs)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, slime, 0f, 0f, 0f, 0, 0, 0);
                                }
                            }
                        }
                        break;

                    case NPCID.CursedHammer:
                    case NPCID.EnchantedSword:
                    case NPCID.CrimsonAxe:
                    case NPCID.Clinger:
                    case NPCID.Gastropod:
                    case NPCID.GiantTortoise:
                    case NPCID.IceTortoise:
                    case NPCID.BlackRecluse:
                    case NPCID.BlackRecluseWall:
                    case NPCID.Paladin:
                        if (Main.getGoodWorld)
                            npc.justHit = false;
                        break;
                }

                if (npc.type == NPCType<Plagueshell>() && Main.getGoodWorld)
                    npc.justHit = false;
            }

            // Plague debuff on kill effect
            if (plague && npc.life <= 0 && npc.realLife == -1)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        int DustID = 220;
                        Dust dust2 = Dust.NewDustDirect(npc.Center, npc.width, npc.height, DustID);
                        dust2.scale = Main.rand.NextFloat(0.6f, 0.75f);
                        dust2.velocity = new Vector2(12, 12).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 0.8f);
                        dust2.noGravity = true;
                    }

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC target = Main.npc[i];

                        if (target != null && target.IsAnEnemy() && !target.buffImmune[BuffType<Plague>()] && Vector2.Distance(target.Center, npc.Center) < 400)
                        {
                            if (!target.HasBuff<Plague>() && target.life > 0)
                            {
                                DirectionalPulseRing pulse = new DirectionalPulseRing(target.Center, Vector2.Zero, Main.rand.NextBool(3) ? Color.LimeGreen : Color.Green, Vector2.One, 0, Main.rand.NextFloat(0.07f, 0.18f) * 3, 0f, 15);
                                GeneralParticleHandler.SpawnParticle(pulse);
                            }
                            target.AddBuff(BuffType<Plague>(), 300);
                        }
                    }
                }
            }

            if (scionsCurioEffected && npc.life <= 0 && npc.realLife == -1)
            {
                for (int g = 0; g < 17; g++)
                {
                    Vector2 dustVel = new Vector2(9).RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat(0.4f, 0.9f) + Vector2.UnitY * -10;
                    Dust dust = Dust.NewDustPerfect(npc.Center, DustType<SquashDust>(), dustVel, 0, Main.rand.NextBool() ? Color.Green : Color.Chartreuse, Main.rand.NextFloat(1.1f, 1.35f));
                    dust.noGravity = false;
                    dust.fadeIn = Main.rand.NextFloat(0.2f, 2f);
                }
                Particle blastvfx = new CustomPulse(npc.Center, Vector2.Zero, Color.Chartreuse * 0.9f, "CalamityMod/Particles/ShineExplosion1", Vector2.One, Main.rand.NextFloat(-10, 10), 0.05f, 0.15f, 10, true);
                GeneralParticleHandler.SpawnParticle(blastvfx);
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { Volume = 0.5f, Pitch = Main.rand.NextFloat(0.5f, 0.7f), MaxInstances = 6 }, npc.Center);

                int explosionDamage = 12;
                float highestDamage = 0;
                Player Owner = null;
                foreach (Player p in Main.ActivePlayers)
                {
                    float playerRangedDamage = p.GetTotalDamage(DamageClass.Ranged).ApplyTo(explosionDamage);
                    if (playerRangedDamage > highestDamage && p.Calamity().scionsCurio)
                    {
                        highestDamage = playerRangedDamage;
                        Owner = p;
                    }
                }

                // Create Blast
                float blastSize = 115;
                float minMultiplier = 0.5f;
                int hitsToMinMult = 5;
                int debuff = BuffType<Irradiated>();
                int debuffTime = 300;
                Projectile blast = Projectile.NewProjectileDirect(Owner != null ? Owner.GetSource_FromThis() : npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ProjectileType<BasicBurst>(), (int)highestDamage, 7, Owner != null ? Owner.whoAmI : -1, blastSize, minMultiplier, hitsToMinMult);
                blast.localAI[0] = debuff;
                blast.localAI[1] = debuffTime;
                blast.timeLeft = 15;
                blast.DamageType = DamageClass.Ranged;
            }
        }
        #endregion

        #region Edit Spawn Rate
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            // Biomes
            if (player.Calamity().ZoneSulphur)
            {
                spawnRate = (int)(spawnRate * 1.1);
                maxSpawns = (int)(maxSpawns * 0.8f);
                if (Main.raining)
                {
                    spawnRate = (int)(spawnRate * 0.7);
                    maxSpawns = (int)(maxSpawns * 1.2f);

                    if (!player.Calamity().ZoneAbyss && AcidRainEvent.AcidRainEventIsOngoing)
                    {
                        if (AcidRainEvent.AnyRainMinibosses)
                        {
                            maxSpawns = 5;
                            spawnRate *= 2;
                        }
                        else
                        {
                            spawnRate = Main.hardMode ? 36 : 33;
                            maxSpawns = Main.hardMode ? 15 : 12;
                        }
                    }
                }
            }
            else if (player.Calamity().ZoneAbyss)
            {
                spawnRate = (int)(spawnRate * 0.7);
                maxSpawns = (int)(maxSpawns * 1.1f);
            }
            else if (player.Calamity().ZoneCalamity)
            {
                spawnRate = (int)(spawnRate * 0.9);
                maxSpawns = (int)(maxSpawns * 1.1f);
            }
            else if (player.Calamity().ZoneAstral)
            {
                spawnRate = (int)(spawnRate * 0.6);
                maxSpawns = (int)(maxSpawns * 1.2f);
            }
            else if (player.Calamity().ZoneSunkenSea)
            {
                spawnRate = (int)(spawnRate * 0.9);
                maxSpawns = (int)(maxSpawns * 1.1f);
            }

            // Boosts
            if (DownedBossSystem.downedDoG && (Main.pumpkinMoon || Main.snowMoon || Main.eclipse))
            {
                spawnRate = (int)(spawnRate * 0.75);
                maxSpawns = (int)(maxSpawns * 3f);
            }

            if (player.Calamity().clamity)
            {
                spawnRate = (int)(spawnRate * 0.02);
                maxSpawns = (int)(maxSpawns * 1.5f);
            }

            if (CalamityWorld.death && Main.bloodMoon && player.position.Y < Main.worldSurface * 16.0)
            {
                spawnRate = (int)(spawnRate * 0.25);
                maxSpawns = (int)(maxSpawns * 5f);
            }

            if (CalamityWorld.death && player.ZoneGraveyard)
            {
                spawnRate = (int)(spawnRate * 0.6667);
                maxSpawns = (int)(maxSpawns * 1.5f);
            }

            if (NPC.LunarApocalypseIsUp)
            {
                if ((player.ZoneTowerNebula && NPC.ShieldStrengthTowerNebula == 0) || (player.ZoneTowerStardust && NPC.ShieldStrengthTowerStardust == 0) ||
                    (player.ZoneTowerVortex && NPC.ShieldStrengthTowerVortex == 0) || (player.ZoneTowerSolar && NPC.ShieldStrengthTowerSolar == 0))
                {
                    spawnRate = (int)(spawnRate * 0.85);
                    maxSpawns = (int)(maxSpawns * 1.25f);
                }
            }

            if (CalamityWorld.revenge)
                spawnRate = (int)(spawnRate * 0.85);

            if (player.Calamity().chaosCandle)
            {
                spawnRate = (int)(spawnRate * 0.5); // 2x spawn rate
                maxSpawns = (int)(maxSpawns * 2f);
            }
            if (player.Calamity().zerg)
            {
                spawnRate = (int)(spawnRate * 0.25); // 4x spawn rate
                maxSpawns = (int)(maxSpawns * 4f);
            }
            if (player.Calamity().bloodyMary || player.GetModPlayer<IVDripPlayer>().HasAlcohol(AlcoholType.BloodyMary))
            {
                spawnRate = (int)(spawnRate * BloodyMary.SpawnRateGateMultiplier); // ~7x spawn rate
                maxSpawns = (int)(maxSpawns * BloodyMary.SpawnLimitMultiplier); // 5x spawn rate cap
            }
            // Only when BOTH effects are active, also applies the previous spawn rate and limit boosts multiplicatively
            if (player.Calamity().bloodyMary && player.GetModPlayer<IVDripPlayer>().HasAlcohol(AlcoholType.BloodyMary))
            {
                spawnRate = (int)(spawnRate * BloodyMary.IVDripAdditionalSpawnRateGateMultiplier); // 1.429x spawn rate, total of 10x 
                maxSpawns = (int)(maxSpawns * BloodyMary.IVDripAdditionalSpawnLimitMultiplier); // 1.5x spawn rate cap
            }

            // Reductions
            if (player.Calamity().tranquilityCandle)
            {
                spawnRate = (int)(spawnRate * 1.6666); // 0.6x spawn rate
                maxSpawns = (int)(maxSpawns * 0.6f);
            }
            if (player.Calamity().zen || (CalamityServerConfig.Instance.ForceTownSafety && player.townNPCs > 1f && Main.expertMode))
            {
                spawnRate = (int)(spawnRate * 2.5); // 0.4x spawn rate
                maxSpawns = (int)(maxSpawns * 0.4f);
            }
            if (player.Calamity().isNearbyBoss && CalamityServerConfig.Instance.BossZen)
            {
                spawnRate *= 5;
                maxSpawns = (int)(maxSpawns * 0.001f);
            }
        }
        #endregion

        #region Edit Spawn Range
        public override void EditSpawnRange(Player player, ref int spawnRangeX, ref int spawnRangeY, ref int safeRangeX, ref int safeRangeY)
        {
            if (player.Calamity().ZoneAbyss)
            {
                spawnRangeX = (int)(1920 / 16 * 0.5); //0.7
                safeRangeX = (int)(1920 / 16 * 0.32); //0.52
            }
        }
        #endregion

        #region Edit Spawn Pool
        public static void AttemptToSpawnLabCritters(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int spawnRate = 400;
            int maxSpawnCount = NPC.maxSpawns;
            NPCLoader.EditSpawnRate(player, ref spawnRate, ref maxSpawnCount);

            // Enforce a limit on the amount of enemies that can appear.
            if (player.nearbyActiveNPCs >= maxSpawnCount)
                return;

            float playerCenterX = player.Center.X / 16f;
            float playerCenterY = player.Center.Y / 16f;
            Vector2 sunkenSeaLabCenter = CalamityWorld.SunkenSeaLabCenter / 16f;
            Vector2 planetoidLabCenter = CalamityWorld.PlanetoidLabCenter / 16f;
            Vector2 jungleLabCenter = CalamityWorld.JungleLabCenter / 16f;
            Vector2 hellLabCenter = CalamityWorld.HellLabCenter / 16f;
            Vector2 iceLabCenter = CalamityWorld.IceLabCenter / 16f;
            for (int i = 0; i < 8; i++)
            {
                int checkPositionX = (int)(playerCenterX + Main.rand.Next(30, 54) * Main.rand.NextBool().ToDirectionInt());
                int checkPositionY = (int)(playerCenterY + Main.rand.Next(24, 45) * Main.rand.NextBool().ToDirectionInt());
                Vector2 checkPosition = new Vector2(checkPositionX, checkPositionY);

                Tile aboveSpawnTile = CalamityUtils.ParanoidTileRetrieval(checkPositionX, checkPositionY - 1);
                bool nearLab = CalamityUtils.ManhattanDistance(checkPosition, sunkenSeaLabCenter) < 180f;
                nearLab |= CalamityUtils.ManhattanDistance(checkPosition, planetoidLabCenter) < 180f;
                nearLab |= CalamityUtils.ManhattanDistance(checkPosition, jungleLabCenter) < 180f;
                nearLab |= CalamityUtils.ManhattanDistance(checkPosition, hellLabCenter) < 180f;
                nearLab |= CalamityUtils.ManhattanDistance(checkPosition, iceLabCenter) < 180f;
                bool nearPlagueLab = CalamityUtils.ManhattanDistance(checkPosition, jungleLabCenter) < 180f;

                bool isLabWall = aboveSpawnTile.WallType == WallType<HazardChevronWall>() || aboveSpawnTile.WallType == WallType<LaboratoryPanelWall>() || aboveSpawnTile.WallType == WallType<LaboratoryPlateBeam>();
                isLabWall |= aboveSpawnTile.WallType == WallType<LaboratoryPlatePillar>() || aboveSpawnTile.WallType == WallType<LaboratoryPlatingWall>() || aboveSpawnTile.WallType == WallType<RustedPlateBeam>();
                if (!isLabWall || !nearLab || Collision.SolidCollision((checkPosition - new Vector2(2f, 2f)).ToWorldCoordinates(), 4, 4) || player.nearbyActiveNPCs >= maxSpawnCount || !Main.rand.NextBool(spawnRate))
                    continue;

                WeightedRandom<int> pool = new WeightedRandom<int>();
                pool.Add(NPCID.None, 0f);
                pool.Add(NPCType<RepairUnitCritter>(), 0.025f);
                pool.Add(NPCType<Androomba>(), 0.01f);
                // Normal droids are replaced with plague droids in the Jungle Lab.
                if (nearPlagueLab)
                {
                    pool.Add(NPCType<NanodroidPlagueGreen>(), 0.025f);
                    pool.Add(NPCType<NanodroidPlagueRed>(), 0.025f);
                    pool.Add(NPCType<NanodroidDysfunctional>(), 0.02f);
                }
                else
                {
                    pool.Add(NPCType<Nanodroid>(), 0.05f);
                    pool.Add(NPCType<NanodroidDysfunctional>(), 0.05f);
                }

                int typeToSpawn = pool.Get();
                if (typeToSpawn != NPCID.None)
                {
                    int spawnedNPC = NPCLoader.SpawnNPC(typeToSpawn, checkPositionX, checkPositionY - 1);
                    if (Main.dedServ && spawnedNPC < Main.maxNPCs)
                    {
                        Main.npc[spawnedNPC].position.Y -= 8f;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, spawnedNPC);
                        return;
                    }
                }
            }
        }

        /*public static void AttemptToSpawnLavaNPCs(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            // For now, we only need this for the Basalt Gully, but this may be used for the crags in the future
            if (!player.Calamity().ZoneBasaltGully)
                return;

            int spawnRate = 400;
            int maxSpawnCount = NPC.maxSpawns;
            NPCLoader.EditSpawnRate(player, ref spawnRate, ref maxSpawnCount);

            // Enforce a limit on the amount of enemies that can appear.
            if (player.nearbyActiveNPCs >= maxSpawnCount)
                return;

            float playerCenterX = player.Center.X / 16f;
            float playerCenterY = player.Center.Y / 16f;
            for (int i = 0; i < 8; i++)
            {
                int checkPositionX = (int)(playerCenterX + Main.rand.Next(30, 54) * Main.rand.NextBool().ToDirectionInt());
                int checkPositionY = (int)(playerCenterY + Main.rand.Next(24, 45) * Main.rand.NextBool().ToDirectionInt());
                Vector2 checkPosition = new Vector2(checkPositionX, checkPositionY);

                Tile aboveSpawnTile = CalamityUtils.ParanoidTileRetrieval(checkPositionX, checkPositionY - 1);

                if (aboveSpawnTile.LiquidAmount < 255 || aboveSpawnTile.LiquidType != LiquidID.Lava || Collision.SolidCollision((checkPosition - new Vector2(2f, 2f)).ToWorldCoordinates(), 4, 4) || player.nearbyActiveNPCs >= maxSpawnCount || !Main.rand.NextBool(spawnRate))
                    continue;

                WeightedRandom<int> pool = new WeightedRandom<int>();
                pool.Add(NPCID.None, 1f);

                int typeToSpawn = pool.Get();
                if (typeToSpawn != NPCID.None)
                {
                    int spawnedNPC = NPCLoader.SpawnNPC(typeToSpawn, checkPositionX, checkPositionY - 1);
                    if (Main.dedServ && spawnedNPC < Main.maxNPCs)
                    {
                        Main.npc[spawnedNPC].position.Y -= 8f;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, spawnedNPC);
                        return;
                    }
                }
            }
        }*/

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            bool calamityBiomeZone = spawnInfo.Player.Calamity().ZoneAbyss ||
                spawnInfo.Player.Calamity().ZoneCalamity ||
                spawnInfo.Player.Calamity().ZoneSulphur ||
                spawnInfo.Player.Calamity().ZoneSunkenSea ||
                (spawnInfo.Player.Calamity().ZoneAstral && !spawnInfo.Player.PillarZone());

            // Increase Goblin and Wizard spawn rates
            if (!spawnInfo.Water && spawnInfo.Player.ZoneRockLayerHeight)
            {
                if (NPC.downedGoblins && !NPC.savedGoblin)
                {
                    if (!NPC.AnyNPCs(NPCID.BoundGoblin))
                        pool[NPCID.BoundGoblin] = SpawnCondition.BoundCaveNPC.Chance * 2f;
                }

                if (Main.hardMode && !NPC.savedWizard)
                {
                    if (!NPC.AnyNPCs(NPCID.BoundWizard))
                        pool[NPCID.BoundWizard] = SpawnCondition.BoundCaveNPC.Chance * 2f;
                }
            }

            // Overrides the vanilla spawn of Chaos Elementals so they can be AFK farmed once more
            if (Main.hardMode && spawnInfo.Player.ZoneRockLayerHeight && !calamityBiomeZone)
            {
                // Added more tiles for them to spawn on
                bool isChaosElementalSpawnTile =
                    spawnInfo.SpawnTileType == TileID.Pearlstone ||
                    spawnInfo.SpawnTileType == TileID.Pearlsand ||
                    spawnInfo.SpawnTileType == TileID.HallowedIce ||
                    spawnInfo.SpawnTileType == TileID.HallowedGrass ||
                    spawnInfo.SpawnTileType == TileID.HallowHardenedSand ||
                    spawnInfo.SpawnTileType == TileID.HallowSandstone;

                if (isChaosElementalSpawnTile)
                    pool[NPCID.ChaosElemental] = SpawnCondition.Cavern.Chance * 0.125f;
            }

            // Spawn Green Jellyfish in prehm and Blue Jellyfish in hardmode
            if (spawnInfo.Player.ZoneRockLayerHeight && spawnInfo.Water && !calamityBiomeZone)
            {
                if (!Main.hardMode)
                    pool[NPCID.GreenJellyfish] = SpawnCondition.CaveJellyfish.Chance * 0.5f;
                else
                    pool[NPCID.BlueJellyfish] = SpawnCondition.CaveJellyfish.Chance;
            }

            // Add Truffle Worm spawns to surface mushroom biome
            if (spawnInfo.Player.ZoneGlowshroom && Main.hardMode && (spawnInfo.Player.ZoneOverworldHeight || spawnInfo.Player.ZoneSkyHeight))
            {
                if (NPC.CountNPCS(NPCID.TruffleWorm) < 2)
                    pool[NPCID.TruffleWorm] = SpawnCondition.OverworldMushroom.Chance * 0.5f;
            }

            // Add Prismatic Lacewing spawns to surface hallow from dusk to midnight
            if (!Main.dayTime && Main.time < 16200D && Main.hardMode && (spawnInfo.Player.ZoneOverworldHeight || spawnInfo.Player.ZoneSkyHeight))
            {
                if (!NPC.AnyNPCs(NPCID.EmpressButterfly))
                    pool[NPCID.EmpressButterfly] = SpawnCondition.OverworldHallow.Chance * 0.1f;
            }

            // Increase fairy spawn rates while wearing Fairy Boots
            if (spawnInfo.Player.Calamity().fairyBoots)
            {
                int maxFairies = 5;
                if ((NPC.CountNPCS(NPCID.FairyCritterBlue) + NPC.CountNPCS(NPCID.FairyCritterGreen) + NPC.CountNPCS(NPCID.FairyCritterPink)) < maxFairies)
                {
                    if (!NPC.AnyNPCs(NPCID.FairyCritterBlue))
                        pool[NPCID.FairyCritterBlue] = SpawnCondition.Overworld.Chance * 5f;
                    if (!NPC.AnyNPCs(NPCID.FairyCritterGreen))
                        pool[NPCID.FairyCritterGreen] = SpawnCondition.Overworld.Chance * 5f;
                    if (!NPC.AnyNPCs(NPCID.FairyCritterPink))
                        pool[NPCID.FairyCritterPink] = SpawnCondition.Overworld.Chance * 5f;
                }
            }

            // Increased Maggot Zombie,the Groom, and the Bride spawn rates in a Graveyard
            if (spawnInfo.Player.ZoneGraveyard)
            {
                pool[NPCID.MaggotZombie] = SpawnCondition.OverworldNightMonster.Chance * 0.2f;
                pool[NPCID.TheGroom] = SpawnCondition.OverworldNightMonster.Chance * 0.035f;
                pool[NPCID.TheBride] = SpawnCondition.OverworldNightMonster.Chance * 0.035f;
            }

            // Disable vanilla spawns while in the Brimstone Crag
            if (calamityBiomeZone)
                pool[0] = 0f;

            // Add Enchanted Nightcrawlers as a critter to the Astral Infection
            if (!AnyEvents(spawnInfo.Player) && spawnInfo.Player.InAstral())
                pool[NPCID.EnchantedNightcrawler] = SpawnCondition.TownCritter.Chance;

            if (spawnInfo.Player.Calamity().ZoneSulphur && !spawnInfo.Player.Calamity().ZoneAbyss && AcidRainEvent.AcidRainEventIsOngoing)
            {
                pool.Clear();

                if (!(DownedBossSystem.downedPolterghast && AcidRainEvent.AccumulatedKillPoints == 1))
                {
                    Dictionary<int, AcidRainSpawnData> PossibleEnemies = AcidRainEvent.PossibleEnemiesPreHM;
                    Dictionary<int, AcidRainSpawnData> PossibleMinibosses = new Dictionary<int, AcidRainSpawnData>();
                    if (DownedBossSystem.downedAquaticScourge)
                    {
                        PossibleEnemies = AcidRainEvent.PossibleEnemiesAS;
                        PossibleMinibosses = AcidRainEvent.PossibleMinibossesAS;
                        if (!PossibleEnemies.ContainsKey(NPCType<IrradiatedSlime>()))
                        {
                            PossibleEnemies.Add(NPCType<IrradiatedSlime>(), new AcidRainSpawnData(1, 0f, AcidRainSpawnRequirement.Anywhere));
                        }
                    }
                    if (DownedBossSystem.downedPolterghast)
                    {
                        PossibleEnemies = AcidRainEvent.PossibleEnemiesPolter;
                        PossibleMinibosses = AcidRainEvent.PossibleMinibossesPolter;
                    }
                    foreach (int enemy in PossibleEnemies.Select(enemyType => enemyType.Key))
                    {
                        bool canSpawn = true;
                        switch (PossibleEnemies[enemy].SpawnRequirement)
                        {
                            case AcidRainSpawnRequirement.Anywhere:
                                break;
                            case AcidRainSpawnRequirement.Land:
                                canSpawn = !spawnInfo.Water;
                                break;
                            case AcidRainSpawnRequirement.Water:
                                canSpawn = spawnInfo.Water;
                                break;
                        }
                        if (canSpawn)
                        {
                            if (!pool.ContainsKey(enemy))
                            {
                                pool.Add(enemy, PossibleEnemies[enemy].SpawnRate);
                            }
                        }
                    }
                    if (PossibleMinibosses.Count > 0)
                    {
                        foreach (int miniboss in PossibleMinibosses.Select(miniboss => miniboss.Key).ToList())
                        {
                            bool canSpawn = true;
                            switch (PossibleMinibosses[miniboss].SpawnRequirement)
                            {
                                case AcidRainSpawnRequirement.Anywhere:
                                    break;
                                case AcidRainSpawnRequirement.Land:
                                    canSpawn = !spawnInfo.Water;
                                    break;
                                case AcidRainSpawnRequirement.Water:
                                    canSpawn = spawnInfo.Water;
                                    break;
                            }
                            if (canSpawn)
                            {
                                pool.Add(miniboss, PossibleMinibosses[miniboss].SpawnRate);
                            }
                        }
                    }
                    if (NPC.CountNPCS(NPCType<NuclearToad>()) >= AcidRainEvent.MaxNuclearToadCount)
                        pool.Remove(NPCType<NuclearToad>());
                }
            }

            if (spawnInfo.PlayerSafe)
                return;

            // Voodoo Demon changes (including partial Voodoo Demon Voodoo Doll implementation)
            bool voodooDemonDollActive = spawnInfo.Player.Calamity().disableVoodooSpawns;
            // If the doll is active, Voodoo Demons cannot spawn (via modded means).
            if (voodooDemonDollActive)
                pool.Remove(NPCID.VoodooDemon);
        }
        #endregion

        #region On Spawn
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (npc.type == NPCID.Deerclops)
            {
                DeerclopsAI.hasTargetBeenInRange = false;
            }

            // Despawn Blazing Wheels and Spike Balls when a boss spawns so they're not annoying and stay in the arena
            if (npc.boss)
            {
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (n.type == NPCID.BlazingWheel || n.type == NPCID.SpikeBall)
                    {
                        n.active = false;
                        n.netUpdate = true;
                    }
                }
            }

            if (npc.type != NPCID.VoodooDemon)
                return;

            // This entity source does not provide a player. So we have to find out if anyone close enough has a doll.
            if (source is EntitySource_SpawnNPC)
            {
                bool voodooDemonDollActive = false;
                Vector2 v = npc.Center;
                for (int i = 0; i < Main.maxPlayers; ++i)
                {
                    Player p = Main.player[i];
                    if (p is null || !p.active)
                        continue;
                    if (p.DistanceSQ(v) < 4000000f && p.Calamity().disableVoodooSpawns) // 2000 pixel radius
                    {
                        voodooDemonDollActive = true;
                        break;
                    }
                }
                if (!voodooDemonDollActive)
                    return;

                npc.Transform(NPCID.Demon);
                npc.netUpdate = true;
            }
        }
        #endregion

        #region Drawing
        public override void FindFrame(NPC npc, int frameHeight)
        {
            // Increment the bestiary worm timer when hovering over the NPC or having their entry open. Pauses otherwise
            if (npc.IsABestiaryIconDummy)
            {
                bestiaryWormTimer += 0.02f;
                // Resets after an hour. No sane human being is looking at a bestiary entry for an hour straight
                if (bestiaryWormTimer > 4320)
                    bestiaryWormTimer = 0;
            }
        }

        // Debuff visuals. Alphabetical order as per usual, please
        // TODO - Merge these into DebuffData
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (!npc.canDisplayBuffs)
                return;

            if (absorberAffliction)
                AbsorberAffliction.DrawEffects(npc, ref drawColor);

            // Rancor's burn effect
            if (ashesOnDeath > 0)
            {
                if (Main.rand.NextBool(4))
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Main.rand.NextVector2Circular(2.75f, 6.5f), ProjectileType<RancorFog>(), 0, 0f, Main.myPlayer, 0f, 0.475f);

                if (Main.rand.NextBool(6))
                {
                    Vector2 randomPosition = new(npc.position.X + Main.rand.NextFloat(-10f, npc.width + 10f), npc.position.Y + Main.rand.NextFloat(-10f, npc.height + 10f));
                    RancorLavaMetaball.SpawnParticle(randomPosition, Main.rand.NextFloat(30f, 37f));
                }
            }

            if (astralInfection)
                AstralInfectionDebuff.DrawEffects(npc, ref drawColor);

            if (bane || baneVisual > 0.05f)
                Bane.DrawEffects(npc, ref drawColor);

            // Brimstone Flames and Demonshade Enrage set bonus share the same visual effects
            // TODO -- change this when Demonshade is reworked
            if (brimstoneFlames || npc.HasBuff<Enraged>())
                BrimstoneFlames.DrawEffects(npc, ref drawColor);

            if (demonicFlames)
                DemonicFlames.DrawEffects(npc, ref drawColor);

            if (burningBlood)
                BurningBlood.DrawEffects(npc, ref drawColor);

            if (brainRot)
                BrainRot.DrawEffects(npc, ref drawColor);

            if (crushDepth)
                CrushDepth.DrawEffects(npc, ref drawColor);

            if (hadopelagicPressure)
                HadopelagicPressure.DrawEffects(npc, ref drawColor);

            if (dragonFire)
                Dragonfire.DrawEffects(npc, ref drawColor);

            if (vermillionFlux)
                VermillionFlux.DrawEffects(npc, ref drawColor);

            if (auricRebuke)
                AuricRebuke.DrawEffects(npc, ref drawColor);

            if (staticDischarge)
                StaticDischarge.DrawEffects(npc, ref drawColor);

            if (elementalMix)
                ElementalMix.DrawEffects(npc, ref drawColor);

            // Eutrophication and Temporal Sadness share the same visual effects
            if (eutrophication || temporalSadness)
                Eutrophication.DrawEffects(npc, ref drawColor);

            if (godSlayerInferno)
                GodSlayerInferno.DrawEffects(npc, ref drawColor);

            // Holy Flames and Banishing Fire share the same visual effects
            if (holyFlames || banishingFire)
                HolyFlames.DrawEffects(npc, ref drawColor);

            if (heavyBleeding)
                HeavyBleeding.DrawEffects(npc, ref drawColor);

            if (hyperiusFxTimer > 0)
            {
                float rate = (Main.GlobalTimeWrappedHourly * 5);
                List<Color> eColors =
                [
                    Color.Yellow,
                    Color.Magenta,
                    Color.Red,
                    Color.Cyan,
                    Color.Lime
                ];
                int colorIndex = (int)(rate / 2 % eColors.Count);
                Color currentColor = eColors[colorIndex];
                Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
                Color usedColor = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : rate % 1f);

                Texture2D tex2 = Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
                Texture2D sparkle = Request<Texture2D>("CalamityMod/Particles/BloomLineSoftEdge").Value;
                Vector2 drawPosition = npc.Center - Main.screenPosition;

                float power = (float)(Math.Pow(Utils.GetLerpValue(0, 20, hyperiusFxTimer, true), 3)) * MathHelper.Lerp(Math.Max(npc.height, npc.width) / 100, 1.4f, 0.5f);
                for (int i = 0; i < 4; i++)
                {
                    float iMult = 1 + 0.25f * i;
                    Main.EntitySpriteDraw(tex2, drawPosition, null, Color.Lerp(usedColor, Color.White, i * 0.1f) with { A = 0 } * 0.6f, Main.rand.NextFloat(-5f, 5f), tex2.Size() * 0.5f, new Vector2(1f, 0.8f) * 0.35f * Main.rand.NextFloat(0.9f, 1.1f) * iMult * power * (Utils.GetLerpValue(0, 20, hyperiusFxTimer)), SpriteEffects.None);

                    for (int b = -1; b <= 1; b += 2)
                    {
                        float uncappedSine = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 8f / MathHelper.Pi);
                        float sine = MathHelper.Lerp(Math.Abs(uncappedSine), 0.75f, 0.75f);
                        Vector2 scale = new Vector2((0.25f / iMult) + (0.7f * (1 - sine)), 1.1f * sine * iMult) * power * 0.05f;
                        float rotation = MathHelper.PiOver4 * b * uncappedSine;
                        Main.EntitySpriteDraw(sparkle, drawPosition, null, Color.Lerp(usedColor, Color.White, i * 0.1f) with { A = 0 }, rotation, sparkle.Size() * 0.5f, scale, SpriteEffects.None);
                    }
                }
            }

            if (laceration)
                Laceration.DrawEffects(npc, ref drawColor);

            if (laserBurnTimer > 0)
            {
                int particleChance = Math.Max(3, 10 - (laserBurnStacks / 3));
                if (laserBurnTimer % particleChance == 0)
                {
                    Vector2 randPosition = new Vector2(npc.position.X + Main.rand.Next(0, npc.width), npc.position.Y + Main.rand.Next(0, npc.height));

                    Dust dust = Dust.NewDustPerfect(randPosition, Effects.ArsenalEffects.ArsenalLaserDust);
                    dust.velocity = ((Vector2.UnitX * 3 * (laserBurnStacks * 0.03f)).RotatedByRandom(100) * Main.rand.NextFloat(0.85f, 1f)) + npc.velocity * 0.5f;
                    dust.scale = Main.rand.NextFloat(0.55f, 0.7f) + laserBurnStacks * 0.01f;
                    dust.noGravity = true;
                    dust.color = Color.Red;
                    dust.fadeIn = laserBurnStacks * 0.3f;
                }
                // If for some reason neither burn type is set
                if (laserBurnType == 0)
                {
                    laserBurnMarked = false;
                    laserBurnTimer = 0;
                }
            }

            // These draw effects do not include Miracle Blight's shader
            if (miracleBlight)
                MiracleBlight.DrawEffects(npc, ref drawColor);

            if (nightwither)
                Nightwither.DrawEffects(npc, ref drawColor);

            if (pearlAura)
                PearlAura.DrawEffects(npc, ref drawColor);

            if (plague)
                Plague.DrawEffects(npc, ref drawColor);

            if (relicOfResilienceWeakness)
                ProfanedWeakness.DrawEffects(npc, ref drawColor);

            if (riptide)
                RiptideDebuff.DrawEffects(npc, ref drawColor);

            if (somaShredStacks > 0 && !Main.dedServ)
                Shred.DrawEffects(npc, this, ref drawColor);

            if (sulphurPoison)
                SulphuricPoisoning.DrawEffects(npc, ref drawColor);

            if (trueVulnerabilityHex)
                TrueVulnerabilityHex.DrawEffects(npc, ref drawColor);

            if (vaporfied)
                Vaporfied.DrawEffects(npc, ref drawColor);

            if (veriumDoomTimer > 0)
            {
                int sparkleChance = Math.Max(2, 8 - (veriumDoomStacks / 2));
                if (veriumDoomTimer % sparkleChance == 0)
                {
                    float veriumRatio = veriumDoomTimer / (float)veriumDoomTime;
                    Vector2 randPosition = new Vector2(npc.position.X + Main.rand.Next(0, npc.width), npc.position.Y + Main.rand.Next(0, npc.height));
                    Particle markedSparkle = new CustomPulse(randPosition, Vector2.Zero, Color.Lerp(new Color(103, 230, 240), new Color(255, 110, 220), 1 - veriumRatio), "CalamityMod/Particles/Sparkle", Vector2.One, Main.rand.NextFloat(-0.75f, 0.75f), 0.9f, 1.1f, 35);
                    GeneralParticleHandler.SpawnParticle(markedSparkle);
                }
            }

            if (voidfrost)
                Voidfrost.DrawEffects(npc, ref drawColor);

            if (windChilled)
                WindChilled.DrawEffects(npc, ref drawColor);

            // TODO -- These debuff visuals cannot be moved because they correspond to vanilla debuffs
            if (electrified)
            {
                if (Main.rand.NextBool())
                    Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Electric, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 0, default, 0.35f);
            }
            if (webbed)
            {
                if (Main.rand.Next(5) < 4)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.Web, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.1f;
                    Main.dust[dust].velocity.Y += 0.25f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
            }

            // Lad pet spawns hearts from NPCs
            if (ladHearts > 0 && !npc.loveStruck && !Main.dedServ)
            {
                if (Main.rand.NextBool(5))
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(10f, 1f, 1f, 0.66f);
                    int heart = Gore.NewGore(npc.GetSource_FromThis(), npc.position + new Vector2(Main.rand.Next(npc.width + 1), Main.rand.Next(npc.height + 1)), velocity * Main.rand.Next(3, 6) * 0.33f, 331, Main.rand.Next(40, 121) * 0.01f);
                    Main.gore[heart].sticky = false;
                    Main.gore[heart].velocity *= 0.4f;
                    Main.gore[heart].velocity.Y -= 0.6f;
                }
            }

            // Vanilla debuff coloring effects + Hunter Potion. This allows GetAlpha (often used in PreDraw) to get vanilla debuff colors
            drawColor = npc.GetNPCColorTintedByBuffs(drawColor);

            // Calamity debuff coloring effects
            // These are in order of precedence because they override each other.
            if (frozen)
                drawColor = Color.Cyan;

            else if (auricRebuke)
            {
                int scaleFactor = (int)(Utils.Remap(npc.width, 30, 400, 5, 15, true));
                drawColor = Main.rand.NextBool(scaleFactor) ? Color.Lerp(Color.DarkBlue, Color.White, Utils.Remap(npc.width, 30, 400, 0.4f, 0.7f, true)) : Color.White;
            }
            else if (vermillionFlux)
            {
                int scaleFactor = (int)(Utils.Remap(npc.width, 30, 400, 5, 15, true));
                drawColor = Main.rand.NextBool(scaleFactor) ? Color.Lerp(Color.DarkRed, Color.White, Utils.Remap(npc.width, 30, 400, 0, 0.7f, true)) : Color.White;
            }
            else if (electrified)
            {
                int scaleFactor = (int)(Utils.Remap(npc.width, 30, 400, 5, 15, true));
                drawColor = Main.rand.NextBool(scaleFactor) ? Color.Lerp(Color.SlateGray, Color.White, Utils.Remap(npc.width, 30, 400, 0, 0.7f, true)) : Color.White;
            }

            else if (absorberAffliction)
                drawColor = Color.DarkSeaGreen;

            else if (markedForDeath || vaporfied)
                drawColor = Color.Fuchsia;

            else if (pearlAura)
                drawColor = new Color(185, 185, 255);

            else if (timeDistortion || galvanicCorrosion)
                drawColor = Color.Aquamarine;
        }

        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            // Don't make this affect the bestiary, that's goofy
            if (npc.IsABestiaryIconDummy)
                return null;

            if (Main.LocalPlayer.Calamity().trippy || (npc.type == NPCID.KingSlime && Main.zenithWorld))
                return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, Main.DiscoR);

            if (npc.type == NPCID.QueenBee && Main.zenithWorld)
            {
                if (npc.life / (float)npc.lifeMax < 0.5f)
                    return new Color(0, 255, 0, 255 - npc.alpha);
                else
                    return new Color(255, 0, 0, 255 - npc.alpha);
            }

            if (npc.HasBuff<Enraged>())
                return new Color(200, 50, 50, 255 - npc.alpha);

            if (npc.type == NPCID.VileSpit || npc.type == NPCID.VileSpitEaterOfWorlds)
                return new Color(150, 200, 0, npc.alpha);

            if (npc.type == NPCID.AncientDoom || npc.type == NPCID.QueenSlimeMinionBlue || npc.type == NPCID.QueenSlimeMinionPink || npc.type == NPCID.QueenSlimeMinionPurple)
                return new Color(255, 255, 255, npc.alpha);

            return null;
        }

        //TODO - Make this a part of DebuffData
        public static List<(string, Predicate<NPC>)> moddedDebuffTextureList = new List<(string, Predicate<NPC>)>
        {
            // All Calamity DoTs in alphabetical order
            ("CalamityMod/Buffs/DamageOverTime/AstralInfectionDebuff", NPC => NPC.Calamity().astralInfection),
            ("CalamityMod/Buffs/DamageOverTime/AuricRebuke", NPC => NPC.Calamity().auricRebuke),
            ("CalamityMod/Buffs/DamageOverTime/Bane", NPC => NPC.Calamity().bane),
            ("CalamityMod/Buffs/DamageOverTime/BanishingFire", NPC => NPC.Calamity().banishingFire),
            ("CalamityMod/Buffs/DamageOverTime/BrainRot", NPC => NPC.Calamity().brainRot),
            ("CalamityMod/Buffs/DamageOverTime/BrimstoneFlames", NPC => NPC.Calamity().brimstoneFlames),
            ("CalamityMod/Buffs/DamageOverTime/DemonicFlames", NPC => NPC.Calamity().demonicFlames),
            ("CalamityMod/Buffs/DamageOverTime/BurningBlood", NPC => NPC.Calamity().burningBlood),
            ("CalamityMod/Buffs/DamageOverTime/CrushDepth", NPC => NPC.Calamity().crushDepth),
            ("CalamityMod/Buffs/DamageOverTime/Dragonfire", NPC => NPC.Calamity().dragonFire),
            ("CalamityMod/Buffs/DamageOverTime/ElementalMix", NPC => NPC.Calamity().elementalMix),
            ("CalamityMod/Buffs/DamageOverTime/GodSlayerInferno", NPC => NPC.Calamity().godSlayerInferno),
            ("CalamityMod/Buffs/DamageOverTime/HadopelagicPressure", NPC => NPC.Calamity().hadopelagicPressure),
            ("CalamityMod/Buffs/DamageOverTime/HolyFlames", NPC => NPC.Calamity().holyFlames),
            ("CalamityMod/Buffs/DamageOverTime/Laceration", NPC => NPC.Calamity().laceration),
            ("CalamityMod/Buffs/DamageOverTime/HeavyBleeding", NPC => NPC.Calamity().heavyBleeding),
            ("CalamityMod/Buffs/DamageOverTime/ManaBurn", NPC => NPC.Calamity().manaBurn > 0),
            ("CalamityMod/Buffs/DamageOverTime/MiracleBlight", NPC => NPC.Calamity().miracleBlight),
            ("CalamityMod/Buffs/DamageOverTime/Nightwither", NPC => NPC.Calamity().nightwither),
            ("CalamityMod/Buffs/DamageOverTime/Plague", NPC => NPC.Calamity().plague),
            ("CalamityMod/Buffs/DamageOverTime/RiptideDebuff", NPC => NPC.Calamity().riptide),
            ("CalamityMod/Buffs/DamageOverTime/SagePoison", NPC => NPC.Calamity().sagePoison),
            ("CalamityMod/Buffs/DamageOverTime/SearingLava", NPC => NPC.HasBuff<SearingLava>()),
            ("CalamityMod/Buffs/DamageOverTime/ShellfishClaps", NPC => NPC.Calamity().shellfishStaffDebuff),
            ("CalamityMod/Buffs/DamageOverTime/Shred", NPC => NPC.Calamity().somaShredStacks > 0),
            ("CalamityMod/Buffs/DamageOverTime/SnapClamDebuff", NPC => NPC.Calamity().snapClamDebuff),
            ("CalamityMod/Buffs/DamageOverTime/StaticDischarge", NPC => NPC.Calamity().staticDischarge),
            ("CalamityMod/Buffs/DamageOverTime/SulphuricPoisoning", NPC => NPC.Calamity().sulphurPoison),
            ("CalamityMod/Buffs/DamageOverTime/TrueVulnerabilityHex", NPC => NPC.Calamity().trueVulnerabilityHex),
            ("CalamityMod/Buffs/DamageOverTime/Vaporfied", NPC => NPC.Calamity().vaporfied),
            ("CalamityMod/Buffs/DamageOverTime/VermillionFlux", NPC => NPC.Calamity().vermillionFlux),
            ("CalamityMod/Buffs/DamageOverTime/Voidfrost", NPC => NPC.Calamity().voidfrost),
            ("CalamityMod/Buffs/DamageOverTime/VulnerabilityHex", NPC => NPC.Calamity().vulnerabilityHex),
            ("CalamityMod/Buffs/DamageOverTime/WindChilled", NPC => NPC.Calamity().windChilled),

            // All other important Calamity debuffs, in alphabetical order
            ("CalamityMod/Buffs/StatDebuffs/AbsorberAffliction", NPC => NPC.Calamity().absorberAffliction),
            ("CalamityMod/Buffs/StatDebuffs/ArmorCrunch", NPC => NPC.Calamity().armorCrunch),
            ("CalamityMod/Buffs/StatDebuffs/Crumbling", NPC => NPC.Calamity().crumble),
            ("CalamityMod/Buffs/StatDebuffs/Eutrophication", NPC => NPC.Calamity().eutrophication),
            ("CalamityMod/Buffs/StatDebuffs/GalvanicCorrosion", NPC => NPC.Calamity().galvanicCorrosion),
            ("CalamityMod/Buffs/StatDebuffs/Irradiated", NPC => NPC.Calamity().irradiated),
            ("CalamityMod/Buffs/StatDebuffs/MarkedforDeath", NPC => NPC.Calamity().markedForDeath),
            ("CalamityMod/Buffs/StatDebuffs/PearlAura", NPC => NPC.Calamity().pearlAura),
            ("CalamityMod/Buffs/StatDebuffs/ProfanedWeakness", NPC => NPC.Calamity().relicOfResilienceWeakness),
            ("CalamityMod/Buffs/StatBuffs/SmashedEvil", NPC => NPC.HasBuff<SmashedEvil>()),
            ("CalamityMod/Buffs/StatDebuffs/TemporalSadness", NPC => NPC.Calamity().temporalSadness),
            ("CalamityMod/Buffs/StatDebuffs/TimeDistortion", NPC => NPC.Calamity().timeDistortion),
            ("CalamityMod/Buffs/StatDebuffs/WhisperingDeath", NPC => NPC.Calamity().whisperingDeath),
            ("CalamityMod/Buffs/StatDebuffs/WitherDebuff", NPC => NPC.Calamity().wither),
        };

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // This is used so that NPCs with specific PreDraws can still draw everything in this hook.
            bool shouldDrawBool = true;

            if (npc.IsABestiaryIconDummy)
            {
                switch (npc.netID)
                {
                    case NPCID.DiggerHead:
                    case NPCID.GiantWormHead:
                    case NPCID.EaterofWorldsHead:
                    case NPCID.WyvernHead:
                    case NPCID.StardustWormHead:
                    case NPCID.SolarCrawltipedeHead:
                    case NPCID.CultistDragonHead:
                    case NPCID.TheDestroyer:
                    case NPCID.LeechHead:
                    case NPCID.DevourerHead:
                    case NPCID.TombCrawlerHead:
                    case NPCID.DuneSplicerHead:
                    case NPCID.BloodEelHead:
                    case NPCID.BoneSerpentHead:
                    case NPCID.SeekerHead:
                        return DrawVanillaBestiaryWorms(spriteBatch, npc, drawColor);
                }
            }
            if (npc.type != NPCID.BrainofCthulhu && (npc.type != NPCID.DukeFishron || npc.ai[0] <= 9f) && npc.active)
            {
                if (CalamityClientConfig.Instance.DebuffDisplay && (npc.boss || BossHealthBarManager.MinibossHPBarList.Contains(npc.type) || BossHealthBarManager.OneToMany.ContainsKey(npc.type) || CalamityNPCSets.ForceDrawDebuffDisplay[npc.type]))
                {
                    List<Texture2D> currentDebuffs = new List<Texture2D>() { };

                    for (int b = 0; b < moddedDebuffTextureList.Count(); b++)
                    {
                        if (moddedDebuffTextureList[b].Item2.Invoke(npc))
                            currentDebuffs.Add(Request<Texture2D>(moddedDebuffTextureList[b].Item1).Value);
                    }
                    // Vanilla damage over time debuffs
                    if (electrified)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Electrified].Value);
                    if (npc.onFire)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.OnFire].Value);
                    if (npc.poisoned)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Poisoned].Value);
                    if (npc.onFire2)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.CursedInferno].Value);
                    if (npc.onFrostBurn)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Frostburn].Value);
                    if (npc.venom)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Venom].Value);
                    if (npc.shadowFlame)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.ShadowFlame].Value);
                    if (npc.oiled)
                        currentDebuffs.Add(Request<Texture2D>("CalamityMod/ExtraTextures/VanillaBuffs/Oiled").Value);
                    if (npc.javelined)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.BoneJavelin].Value);
                    if (npc.daybreak)
                        currentDebuffs.Add(Request<Texture2D>("CalamityMod/Buffs/DamageOverTime/Daybroken").Value);
                    if (npc.celled)
                        currentDebuffs.Add(Request<Texture2D>("CalamityMod/ExtraTextures/VanillaBuffs/Celled").Value);
                    if (npc.dryadBane)
                        currentDebuffs.Add(Request<Texture2D>("CalamityMod/ExtraTextures/VanillaBuffs/DryadsBane").Value);
                    if (npc.dryadWard)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.DryadsWard].Value);
                    if (npc.soulDrain && npc.realLife == -1)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.SoulDrain].Value);
                    if (npc.onFire3) // Hellfire
                        currentDebuffs.Add(Request<Texture2D>("CalamityMod/ExtraTextures/VanillaBuffs/Hellfire").Value);
                    if (npc.onFrostBurn2) // Frostbite
                        currentDebuffs.Add(Request<Texture2D>("CalamityMod/ExtraTextures/VanillaBuffs/Frostbite").Value);
                    if (npc.tentacleSpiked)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.TentacleSpike].Value);

                    // Vanilla stat debuffs
                    if (npc.confused)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Confused].Value);
                    if (npc.ichor)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Ichor].Value);
                    if (frozen)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Frozen].Value);
                    if (webbed)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Webbed].Value);
                    if (npc.midas)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Midas].Value);
                    if (npc.loveStruck)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Lovestruck].Value);
                    if (npc.stinky)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Stinky].Value);
                    if (npc.betsysCurse)
                        currentDebuffs.Add(Request<Texture2D>("CalamityMod/ExtraTextures/VanillaBuffs/BetsysCurse").Value);
                    if (npc.dripping)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Wet].Value);
                    if (npc.drippingSlime)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Slimed].Value);
                    if (npc.drippingSparkleSlime)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.GelBalloonBuff].Value);

                    // Total amount of elements in the buff list
                    int buffTextureListLength = currentDebuffs.Count;
                    // Total length of a single row in the buff display
                    int totalLength = buffTextureListLength * 14;
                    // Max amount of buffs per row
                    int buffDisplayRowLimit = 5;
                    // The maximum length of a single row in the buff display
                    // Limited to 80 units, because every buff drawn here is half the size of a normal buff, 16 x 16, 16 * 5 = 80 units
                    float drawPosX = totalLength >= 80f ? 40f : (float)(totalLength / 2);
                    // The height of a single frame of the npc
                    float npcHeight = (npc.height * npc.scale) / 2;
                    // Offset the debuff display based on the npc's graphical offset, and 16 units, to create some space between the sprite and the display
                    float drawPosY = npcHeight + npc.gfxOffY + 32f;

                    // Iterate through the buff texture list
                    for (int i = 0; i < currentDebuffs.Count; i++)
                    {
                        // Reset the X position of the display every 5th and non-zero iteration, otherwise decrease the X draw position by 16 units
                        if (i != 0)
                        {
                            if (i % buffDisplayRowLimit == 0)
                                drawPosX = 40f;
                            else
                                drawPosX -= 14f;
                        }

                        // Offset the Y position every row after 5 iterations to limit each displayed row to 5 debuffs
                        float additionalYOffset = 14f * (float)Math.Floor(i * 0.2);

                        // Draw the display
                        var tex = currentDebuffs[i];
                        spriteBatch.Draw(tex, npc.Center - screenPos - new Vector2(drawPosX, drawPosY + additionalYOffset), null, Color.White, 0f, default, 0.5f, SpriteEffects.None, 0f);

                        // Shred stack display
                        if (currentDebuffs[i] == TextureAssets.Buff[BuffType<Shred>()].Value)
                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, somaShredStacks.ToString(), npc.Center - screenPos - new Vector2(drawPosX, drawPosY + additionalYOffset) + Vector2.One * 4f, Color.Gold, 0f, Vector2.Zero, Vector2.One * Main.UIScale * 0.8f);
                    }

                    int yOffset = 0;
                    for (int i = NPC.maxBuffs - 1; i >= 0; i--)
                    {
                        if (npc.buffTime[i] > 0)
                        {
                            var tag = CalamityBuffSets.SummonTagDebuff[npc.buffType[i]];
                            if (tag is not null)
                            {
                                // Fetch the item and its frames
                                var tex = TextureAssets.Item[tag.TagItem].Value;
                                Rectangle frame = (Main.itemAnimations[tag.TagItem] == null) ? tex.Frame() : Main.itemAnimations[tag.TagItem].GetFrame(tex);
                                if (tag.TagTexture != null)
                                {
                                    tex = tag.TagTexture.Value;
                                    frame = tex.Frame();
                                }

                                // Draw it accordingly
                                // This is drawn below the NPC as opposed to above to differentiate from regular debuffs
                                Vector2 drawPos = npc.Center - screenPos + Vector2.UnitY * (drawPosY + frame.Height * 0.5f + yOffset);
                                spriteBatch.Draw(tex, drawPos, frame, Color.White, 0f, frame.Size() * 0.5f, 0.75f, SpriteEffects.None, 0f);
                                yOffset += frame.Height + 4;
                            }
                        }
                    }
                }
            }

            // VHex, Mana Burn and Miracle Blight visuals do not appear if Odd Mushroom is in use for sanity reasons
            if (!Main.LocalPlayer.Calamity().trippy)
            {
                if (npc.Calamity().vulnerabilityHex || npc.Calamity().trueVulnerabilityHex)
                {
                    float compactness = npc.width * 0.6f;
                    if (compactness < 10f)
                        compactness = 10f;
                    float power = npc.height / 100f;
                    if (power > 2.75f)
                        power = 2.75f;
                    if (VulnerabilityHexFireDrawer is null || VulnerabilityHexFireDrawer.LocalTimer >= VulnerabilityHexFireDrawer.SetLifetime)
                        VulnerabilityHexFireDrawer = new FireParticleSet(npc.Calamity().trueVulnerabilityHex ? npc.buffTime[npc.FindBuffIndex(BuffType<TrueVulnerabilityHex>())] : npc.buffTime[npc.FindBuffIndex(BuffType<VulnerabilityHex>())], 1, Color.Red * 1.25f, Color.Red, compactness, power);
                    else
                        VulnerabilityHexFireDrawer.DrawSet(npc.Bottom - Vector2.UnitY * (12f - npc.gfxOffY));
                }
                else
                    VulnerabilityHexFireDrawer = null;

                // Mana Burn effect is just vhex but blue
                if (npc.Calamity().manaBurn > 0)
                {
                    float compactness = npc.width * 0.6f;
                    if (compactness < 10f)
                        compactness = 10f;
                    float power = npc.height / 100f;
                    if (power > 2.75f)
                        power = 2.75f;
                    var color = Color.Blue;
                    if (ManaBurnFireDrawer is null || ManaBurnFireDrawer.LocalTimer >= ManaBurnFireDrawer.SetLifetime)
                        ManaBurnFireDrawer = new FireParticleSet(60, 1, color * 1.25f, color, compactness, power);
                    else
                        ManaBurnFireDrawer.DrawSet(npc.Bottom - Vector2.UnitY * (12f - npc.gfxOffY));
                }
                else
                    ManaBurnFireDrawer = null;
            }

            if (Main.zenithWorld)
            {
                if (NPC.AnyNPCs(NPCType<CeaselessVoid.CeaselessVoid>()))
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                    var midnightShader = GameShaders.Armor.GetShaderFromItemId(ItemID.MidnightRainbowDye);
                    midnightShader.Apply();
                }
            }

            return shouldDrawBool;
        }

        public static Color buffColor(Color newColor, float R, float G, float B, float A)
        {
            newColor.R = (byte)((float)newColor.R * R);
            newColor.G = (byte)((float)newColor.G * G);
            newColor.B = (byte)((float)newColor.B * B);
            newColor.A = (byte)((float)newColor.A * A);
            return newColor;
        }

        public static bool DrawVanillaBestiaryWorms(SpriteBatch spriteBatch, NPC npc, Color drawColor)
        {
            npc.Opacity = 1;
            int segments = 6;
            int spacing = 20;
            int bashLength = 0;
            float bashSpeed = 0f;
            int speed = 3;
            float rotation = 0.6f;
            Texture2D wyvernArm = TextureAssets.Npc[NPCID.WyvernLegs].Value;
            Texture2D wyvernBody = TextureAssets.Npc[NPCID.WyvernBody].Value;
            switch (npc.netID)
            {
                case NPCID.DiggerHead:
                    return CalamityUtils.DrawAnimatedBestiaryWorm(spriteBatch, npc, drawColor, TextureAssets.Npc[npc.type].Value, TextureAssets.Npc[npc.type + 1].Value, segments, 24, 0.4f, Vector2.Zero, speed, 10, 10, 0.2f);
                case NPCID.GiantWormHead:
                    return CalamityUtils.DrawAnimatedBestiaryWorm(spriteBatch, npc, drawColor, TextureAssets.Npc[npc.type].Value, TextureAssets.Npc[npc.type + 1].Value, 8, 14, 0.6f, new Vector2(20, 0), 4, 10, 6, 0.18f);
                case NPCID.EaterofWorldsHead:
                    return CalamityUtils.DrawAnimatedBestiaryWorm(spriteBatch, npc, drawColor, TextureAssets.Npc[npc.type].Value, TextureAssets.Npc[npc.type + 1].Value, segments, 34, 0.2f, new Vector2(30, 0), speed, 10, 16, 0.24f);
                case NPCID.WyvernHead:
                    return CalamityUtils.DrawAnimatedBestiaryWorm(spriteBatch, npc, drawColor, TextureAssets.Npc[npc.type].Value, [wyvernArm, wyvernBody, wyvernBody, wyvernBody], 4, 28, 0.1f, new Vector2(36, 0), speed, 6, 50, 0.3f, true);
                case NPCID.StardustWormHead:
                    return CalamityUtils.DrawAnimatedBestiaryWorm(spriteBatch, npc, drawColor, TextureAssets.Npc[npc.type].Value, TextureAssets.Npc[npc.type + 1].Value, 8, 14, rotation, new Vector2(0, 10), 4, 10, 6, 0.18f);
                case NPCID.SolarCrawltipedeHead:
                    return CalamityUtils.DrawAnimatedBestiaryWorm(spriteBatch, npc, drawColor, TextureAssets.Npc[npc.type].Value, TextureAssets.Npc[npc.type + 1].Value, segments, spacing, rotation, Vector2.Zero, 6, 10, 16, 0.22f);
                case NPCID.CultistDragonHead:
                    return DrawSpecialBestiaryWorm(spriteBatch, npc, drawColor);
                case NPCID.TheDestroyer:
                    return DrawSpecialBestiaryWorm(spriteBatch, npc, drawColor);
                case NPCID.LeechHead:
                    return CalamityUtils.DrawAnimatedBestiaryWorm(spriteBatch, npc, drawColor, TextureAssets.Npc[npc.type].Value, TextureAssets.Npc[npc.type + 1].Value, 8, 14, 0.6f, new Vector2(20, 0), 4, 10, 6, 0.18f);
                case NPCID.DevourerHead:
                    return CalamityUtils.DrawAnimatedBestiaryWorm(spriteBatch, npc, drawColor, TextureAssets.Npc[npc.type].Value, TextureAssets.Npc[npc.type + 1].Value, segments, spacing, rotation, Vector2.Zero, speed, 20, 10, 0.2f);
                case NPCID.TombCrawlerHead:
                    return CalamityUtils.DrawAnimatedBestiaryWorm(spriteBatch, npc, drawColor, TextureAssets.Npc[npc.type].Value, TextureAssets.Npc[npc.type + 1].Value, 9, 14, rotation, Vector2.Zero, speed, 20, 6, 0.14f);
                case NPCID.DuneSplicerHead:
                    return CalamityUtils.DrawAnimatedBestiaryWorm(spriteBatch, npc, drawColor, TextureAssets.Npc[npc.type].Value, TextureAssets.Npc[npc.type + 1].Value, segments, 28, 0.4f, Vector2.Zero, speed, 10, bashLength, bashSpeed);
                case NPCID.BloodEelHead:
                    return CalamityUtils.DrawAnimatedBestiaryWorm(spriteBatch, npc, drawColor, TextureAssets.Npc[npc.type].Value, TextureAssets.Npc[npc.type + 1].Value, 6, 22, 0.1f, Vector2.Zero, speed, 6, 20, 0.2f, true);
                case NPCID.BoneSerpentHead:
                    return CalamityUtils.DrawAnimatedBestiaryWorm(spriteBatch, npc, drawColor, TextureAssets.Npc[npc.type].Value, TextureAssets.Npc[npc.type + 1].Value, 9, 16, rotation, Vector2.Zero, speed, 10, 30, 0.4f);
                case NPCID.SeekerHead:
                    return CalamityUtils.DrawAnimatedBestiaryWorm(spriteBatch, npc, drawColor, TextureAssets.Npc[npc.type].Value, TextureAssets.Npc[npc.type + 1].Value, segments, spacing, rotation, Vector2.Zero, speed, 20, 10, 0.2f);
            }
            return true;
        }

        public static bool DrawSpecialBestiaryWorm(SpriteBatch spriteBatch, NPC npc, Color drawColor)
        {
            // This is solely for The Destroyer and the Phantasm Dragon due to having more than 1 frame each but only for specific segments
            bool dragon = npc.type == NPCID.CultistDragonHead;
            Texture2D headTexture = TextureAssets.Npc[npc.type].Value;
            float wormTimer = npc.Calamity().bestiaryWormTimer;
            // Dragon head has 3 frames, Destroyer has 1
            int frameAmt = dragon ? 3 : 1;
            npc.frame = TextureAssets.Npc[npc.type].Frame(1, frameAmt, 0, 0);
            Vector2 baseOffset = new Vector2(dragon ? 0 : 20, dragon ? 0 : 20);
            // Buffers the segment position and rotations
            float offset = -0.2f;
            float startX = baseOffset.X;
            float startY = baseOffset.Y;
            int segmentSpacing = dragon ? 32 : 38;
            int animationSpeed = 3;
            int range = 10;
            int headOffset = dragon ? 40 : 20;
            float headSpeedOffset = dragon ? 0.2f : 0.16f;
            float rotationStrength = 0.2f;
            // Draw the body segments
            for (int i = 4; i > 0; i--)
            {
                // The first segment is slightly closer to keep up with the head
                float bodyOffset = i == 1 ? i * segmentSpacing * 0.4f : i * segmentSpacing - segmentSpacing * 0.5f;

                // Second dragon segment uses the arm, rest use the normal body
                Texture2D toUse = i == 2 ? TextureAssets.Npc[NPCID.CultistDragonBody1].Value : TextureAssets.Npc[NPCID.CultistDragonBody2].Value;
                // If it's The Destroyer instead use his texture and increase the frame count to two
                if (!dragon)
                    toUse = TextureAssets.Npc[NPCID.TheDestroyerBody].Value;
                int bodyFrameAmt = dragon ? 1 : 2;
                spriteBatch.Draw(toUse, npc.position + new Vector2(startX + bodyOffset, MathF.Sin((wormTimer + offset * i) * animationSpeed) * range + startY), toUse.Frame(1, bodyFrameAmt, 0, 0), npc.GetAlpha(drawColor), npc.rotation - MathHelper.PiOver2 - MathF.Cos((wormTimer + offset * i) * animationSpeed) * MathHelper.PiOver4 * rotationStrength, new Vector2(toUse.Width * 0.5f, toUse.Height * 0.5f / bodyFrameAmt), npc.scale, SpriteEffects.FlipHorizontally, 0f);
            }
            // Draw the head
            spriteBatch.Draw(headTexture, npc.position + new Vector2(startX + headOffset, MathF.Sin((wormTimer - headSpeedOffset) * animationSpeed) * range + startY), npc.frame, npc.GetAlpha(drawColor), npc.rotation - MathHelper.PiOver2 - MathF.Cos((wormTimer - headSpeedOffset) * animationSpeed) * MathHelper.PiOver4 * rotationStrength, new Vector2(headTexture.Width * 0.5f, headTexture.Height / (float)frameAmt), npc.scale, SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
        #endregion

        #region Any Events
        public static bool AnyEvents(Player player, bool checkBloodMoon = false)
        {
            if (Main.invasionType > InvasionID.None && Main.invasionProgressNearInvasion)
                return true;
            if (player.PillarZone())
                return true;
            if (DD2Event.Ongoing && player.ZoneOldOneArmy)
                return true;
            if ((player.ZoneOverworldHeight || player.ZoneSkyHeight) && (Main.eclipse || Main.pumpkinMoon || Main.snowMoon))
                return true;
            if (AcidRainEvent.AcidRainEventIsOngoing && player.InSulphur())
                return true;
            if ((player.ZoneOverworldHeight || player.ZoneSkyHeight) && Main.bloodMoon && checkBloodMoon)
                return true;
            return false;
        }
        #endregion

        #region Get Downed Boss Variable
        public static bool GetDownedBossVariable(int type)
        {
            switch (type)
            {
                case NPCID.KingSlime:
                    return NPC.downedSlimeKing;
                case NPCID.EyeofCthulhu:
                    return NPC.downedBoss1;
                case NPCID.EaterofWorldsHead:
                case NPCID.EaterofWorldsBody:
                case NPCID.EaterofWorldsTail:
                case NPCID.BrainofCthulhu:
                case NPCID.Creeper:
                    return NPC.downedBoss2;
                case NPCID.QueenBee:
                    return NPC.downedQueenBee;
                case NPCID.SkeletronHead:
                    return NPC.downedBoss3;
                case NPCID.Deerclops:
                    return NPC.downedDeerclops;
                case NPCID.WallofFlesh:
                case NPCID.WallofFleshEye:
                    return Main.hardMode;
                case NPCID.QueenSlimeBoss:
                    return NPC.downedQueenSlime;
                case NPCID.TheDestroyer:
                case NPCID.TheDestroyerBody:
                case NPCID.TheDestroyerTail:
                    return NPC.downedMechBoss1;
                case NPCID.Spazmatism:
                case NPCID.Retinazer:
                    return NPC.downedMechBoss2;
                case NPCID.SkeletronPrime:
                    return NPC.downedMechBoss3;
                case NPCID.Plantera:
                    return NPC.downedPlantBoss;
                case NPCID.HallowBoss:
                    return NPC.downedEmpressOfLight;
                case NPCID.Golem:
                case NPCID.GolemHead:
                    return NPC.downedGolemBoss;
                case NPCID.DukeFishron:
                    return NPC.downedFishron;
                case NPCID.CultistBoss:
                    return NPC.downedAncientCultist;
                case NPCID.MoonLordCore:
                case NPCID.MoonLordHand:
                case NPCID.MoonLordHead:
                    return NPC.downedMoonlord;
            }

            if (type == NPCType<DesertScourgeHead>() || type == NPCType<DesertScourgeBody>() || type == NPCType<DesertScourgeTail>())
                return DownedBossSystem.downedDesertScourge;
            else if (type == NPCType<Crabulon.Crabulon>())
                return DownedBossSystem.downedCrabulon;
            else if (type == NPCType<HiveMind.HiveMind>())
                return DownedBossSystem.downedHiveMind;
            else if (type == NPCType<PerforatorHive>())
                return DownedBossSystem.downedPerforator;
            else if (type == NPCType<SlimeGodCore>())
                return DownedBossSystem.downedSlimeGod;
            else if (type == NPCType<Cryogen.Cryogen>())
                return DownedBossSystem.downedCryogen;
            else if (type == NPCType<AquaticScourgeHead>() || type == NPCType<AquaticScourgeBody>() || type == NPCType<AquaticScourgeBodyAlt>() || type == NPCType<AquaticScourgeTail>())
                return DownedBossSystem.downedAquaticScourge;
            else if (type == NPCType<BrimstoneElemental.BrimstoneElemental>())
                return DownedBossSystem.downedBrimstoneElemental;
            else if (type == NPCType<CalamitasClone>())
                return DownedBossSystem.downedCalamitasClone;
            else if (type == NPCType<Leviathan.Leviathan>() || type == NPCType<Anahita>())
                return DownedBossSystem.downedLeviathan;
            else if (type == NPCType<AstrumAureus.AstrumAureus>())
                return DownedBossSystem.downedAstrumAureus;
            else if (type == NPCType<AstrumDeusHead>() || type == NPCType<AstrumDeusBody>() || type == NPCType<AstrumDeusTail>())
                return DownedBossSystem.downedAstrumDeus;
            else if (type == NPCType<PlaguebringerGoliath.PlaguebringerGoliath>())
                return DownedBossSystem.downedPlaguebringer;
            else if (type == NPCType<RavagerBody>())
                return DownedBossSystem.downedRavager;
            else if (type == NPCType<ProfanedGuardianCommander>())
                return DownedBossSystem.downedGuardians;
            else if (type == NPCType<Dragonfolly>())
                return DownedBossSystem.downedDragonfolly;
            else if (type == NPCType<Providence.Providence>())
                return DownedBossSystem.downedProvidence;
            else if (type == NPCType<CeaselessVoid.CeaselessVoid>() || type == NPCType<DarkEnergy>())
                return DownedBossSystem.downedCeaselessVoid;
            else if (type == NPCType<StormWeaverHead>() || type == NPCType<StormWeaverBody>() || type == NPCType<StormWeaverTail>())
                return DownedBossSystem.downedStormWeaver;
            else if (type == NPCType<Signus.Signus>())
                return DownedBossSystem.downedSignus;
            else if (type == NPCType<Polterghast.Polterghast>())
                return DownedBossSystem.downedPolterghast;
            else if (type == NPCType<OldDuke.OldDuke>())
                return DownedBossSystem.downedBoomerDuke;
            else if (type == NPCType<DevourerofGodsHead>() || type == NPCType<DevourerofGodsBody>() || type == NPCType<DevourerofGodsTail>())
                return DownedBossSystem.downedDoG;
            else if (type == NPCType<Yharon.Yharon>())
                return DownedBossSystem.downedYharon;
            else if (type == NPCType<Artemis>() || type == NPCType<Apollo>() || type == NPCType<AresBody>() || type == NPCType<AresGaussNuke>() || type == NPCType<AresLaserCannon>() || type == NPCType<AresPlasmaFlamethrower>() || type == NPCType<AresTeslaCannon>() || type == NPCType<ThanatosHead>() || type == NPCType<ThanatosBody1>() || type == NPCType<ThanatosBody2>() || type == NPCType<ThanatosTail>())
                return DownedBossSystem.downedExoMechs;
            else if (type == NPCType<SupremeCalamitas.SupremeCalamitas>())
                return DownedBossSystem.downedCalamitas;
            else if (type == NPCType<PrimordialWyrmHead>())
                return DownedBossSystem.downedPrimordialWyrm;

            return true;
        }
        #endregion

        #region Speedrun Display
        public static void SetNewBossJustDowned(NPC npc)
        {
            if (!GetDownedBossVariable(npc.type))
            {
                CalamityNPCSets.BossSpeedrunTimerID.TryGetValue(npc.type, out int newBossTypeJustDowned);
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (!player.active)
                        continue;

                    CalamityPlayer mp = player.Calamity();
                    mp.lastSplitType = newBossTypeJustDowned;
                    mp.lastSplit = mp.previousSessionTotal.Add(SpeedrunTimerSystem.Elapsed);
                }
            }
        }
        #endregion

        #region Player Counts
        public static bool AnyLivingPlayers()
        {
            foreach (Player player in Main.ActivePlayers)
            {
                if (!player.dead && !player.ghost)
                    return true;
            }
            return false;
        }

        public static int GetActivePlayerCount()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return 1;

            return Main.CurrentFrameFlags.ActivePlayersCount;
        }
        #endregion

        #region Should Affect NPC
        public static bool ShouldAffectNPC(NPC target)
        {
            if (CalamityNPCTypeSets.EaterOfWorlds.Contains(target.type) || CalamityNPCTypeSets.Destroyer.Contains(target.type))
                return false;

            if (target.damage > 0 && !target.boss && !target.friendly && !target.dontTakeDamage && target.type != NPCID.Creeper && target.type != NPCType<RavagerClawLeft>() &&
                target.type != NPCID.MourningWood && target.type != NPCID.Everscream && target.type != NPCID.SantaNK1 && target.type != NPCType<RavagerClawRight>() &&
                target.type != NPCType<ReaperShark>() && target.type != NPCType<Mauler>() && target.type != NPCType<EidolonWyrmHead>() && target.type != NPCID.GolemFistLeft && target.type != NPCID.GolemFistRight &&
                target.type != NPCType<PrimordialWyrmHead>() && target.type != NPCType<ColossalSquid>() && target.type != NPCID.DD2Betsy && !CalamityNPCSets.ImmuneToSlowsAndOtherSpecialEffects[target.type] && !AcidRainEvent.AllMinibosses.Contains(target.type))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Old Duke Spawn
        public static void OldDukeSpawn(int plr, int type, int baitType)
        {
            Player player = Main.player[plr];
            if (!player.active || player.dead)
                return;

            int m = 0;
            while (m < Main.maxProjectiles)
            {
                Projectile projectile = Main.projectile[m];
                if (projectile.active && projectile.bobber && projectile.owner == plr)
                {
                    if (plr == Main.myPlayer && projectile.ai[0] == 0f)
                    {
                        for (int item = 0; item < Main.InventorySlotsTotal; item++)
                        {
                            if (player.inventory[item].type == baitType)
                            {
                                player.inventory[item].stack--;
                                if (player.inventory[item].stack <= 0)
                                {
                                    player.inventory[item].SetDefaults(ItemID.None, false);
                                }
                                break;
                            }
                        }

                        projectile.ai[0] = 2f;
                        projectile.netUpdate = true;

                        // The vanilla game uses a special packet for Duke Fishron spawning.
                        // However, this packet doesn't work on modded NPC types, so we must create a custom one.
                        // Also, you can't use Netmode != NetmodeID.MultiplayerClient in a projectile context that has an owner, hence the MyPlayer check.
                        if (Main.myPlayer == projectile.owner)
                        {
                            if (!player.active || player.dead)
                                return;

                            Projectile proj = null;
                            foreach (Projectile p in Main.ActiveProjectiles)
                            {
                                proj = p;
                                if (p.bobber && p.owner == player.whoAmI)
                                {
                                    break;
                                }
                            }

                            if (proj is null)
                                return;

                            var spawnPosX = (int)proj.Center.X;
                            var spawnPosY = (int)proj.Center.Y + 100;
                            if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                int oldDuke = NPC.NewNPC(NPC.GetBossSpawnSource(player.whoAmI), spawnPosX, spawnPosY, NPCType<OldDuke.OldDuke>());
                                CalamityUtils.BossAwakenMessage(oldDuke);
                            }
                            else if (Main.netMode == NetmodeID.MultiplayerClient)
                            {
                                SpawnBossOnPositionPacket.Send(spawnPosX, spawnPosY, NPCType<OldDuke.OldDuke>(), player);
                            }
                        }
                    }
                    break;
                }
                else
                {
                    m++;
                }
            }
        }
        #endregion

        #region Astral Things
        public static void DoHitDust(NPC npc, int hitDirection, int dustType = 5, float xSpeedMult = 1f, int numHitDust = 5, int numDeathDust = 20)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, dustType, hitDirection * xSpeedMult, -1f);
            }

            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(npc.position, npc.width, npc.height, dustType, hitDirection * xSpeedMult, -1f);
            }
        }

        public static void DoFlyingAI(NPC npc, float maxSpeed, float acceleration, float circleTime, float minDistanceTarget = 150f, bool shouldAttackTarget = true)
        {
            // Pick a new target
            if (npc.target < 0 || npc.target >= Main.maxPlayers || Main.player[npc.target].dead)
                npc.TargetClosest(true);

            Player myTarget = Main.player[npc.target];
            Vector2 toTarget = myTarget.Center - npc.Center;
            float distanceToTarget = toTarget.Length();
            Vector2 maxVelocity = toTarget;

            if (distanceToTarget < 3f)
            {
                maxVelocity = npc.velocity;
            }
            else
            {
                float magnitude = maxSpeed / distanceToTarget;
                maxVelocity *= magnitude;
            }

            // Circular motion
            npc.ai[0]++;

            // Y motion
            if (npc.ai[0] > circleTime * 0.5f)
                npc.velocity.Y += acceleration;
            else
                npc.velocity.Y -= acceleration;
            // X motion
            if (npc.ai[0] < circleTime * 0.25f || npc.ai[0] > circleTime * 0.75f)
                npc.velocity.X += acceleration;
            else
                npc.velocity.X -= acceleration;
            // Reset
            if (npc.ai[0] > circleTime)
                npc.ai[0] = 0f;

            // If close enough
            if (shouldAttackTarget && distanceToTarget < minDistanceTarget)
                npc.velocity += maxVelocity * 0.007f;

            if (myTarget.dead)
            {
                maxVelocity.X = npc.direction * maxSpeed / 2f;
                maxVelocity.Y = -maxSpeed / 2f;
            }

            // Maximise velocity
            if (npc.velocity.X < maxVelocity.X)
                npc.velocity.X += acceleration;
            if (npc.velocity.X > maxVelocity.X)
                npc.velocity.X -= acceleration;
            if (npc.velocity.Y < maxVelocity.Y)
                npc.velocity.Y += acceleration;
            if (npc.velocity.Y > maxVelocity.Y)
                npc.velocity.Y -= acceleration;

            // Rotate towards player if alive
            if (!myTarget.dead)
                npc.rotation = toTarget.ToRotation();
            else // Don't, do velocity instead
                npc.rotation = npc.velocity.ToRotation();

            npc.rotation += MathHelper.Pi;

            // Tile collision
            float collisionDamp = 0.7f;
            if (npc.collideX)
            {
                npc.netUpdate = true;
                npc.velocity.X = npc.oldVelocity.X * -collisionDamp;

                if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                    npc.velocity.X = 2f;
                if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                    npc.velocity.X = -2f;
            }
            if (npc.collideY)
            {
                npc.netUpdate = true;
                npc.velocity.Y = npc.oldVelocity.Y * -collisionDamp;

                if (npc.velocity.Y > 0f && npc.velocity.Y < 1.5f)
                    npc.velocity.Y = 1.5f;
                if (npc.velocity.Y < 0f && npc.velocity.Y > -1.5f)
                    npc.velocity.Y = -1.5f;
            }

            // Water collision
            if (npc.wet)
            {
                if (npc.velocity.Y > 0f)
                    npc.velocity.Y *= 0.95f;
                npc.velocity.Y -= 0.3f;
                if (npc.velocity.Y < -2f)
                    npc.velocity.Y = -2f;
            }

            // Taken from source. Important for net?
            if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
            {
                npc.netUpdate = true;
            }
        }

        public static void DoSpiderWallAI(NPC npc, int transformType, float chaseMaxSpeed = 2f, float chaseAcceleration = 0.08f)
        {
            // GET NEW TARGET
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead)
                npc.TargetClosest();

            Vector2 between = Main.player[npc.target].Center - npc.Center;
            float distance = between.Length();

            // Modify vector depending on distance and speed.
            if (distance == 0f)
            {
                between.X = npc.velocity.X;
                between.Y = npc.velocity.Y;
            }
            else
            {
                distance = chaseMaxSpeed / distance;
                between.X *= distance;
                between.Y *= distance;
            }

            // Update if target dead.
            if (Main.player[npc.target].dead)
            {
                between.X = npc.direction * chaseMaxSpeed / 2f;
                between.Y = -chaseMaxSpeed / 2f;
            }
            npc.spriteDirection = -1;

            // If spider can't see target, circle around to attempt to find the target.
            if (!Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
            {
                // CIRCULAR MOTION, SIMILAR TO FLYING AI (Eater of Souls etc.)
                npc.ai[0]++;

                if (npc.ai[0] > 0f)
                    npc.velocity.Y += 0.023f;
                else
                    npc.velocity.Y -= 0.023f;
                if (npc.ai[0] < -100f || npc.ai[0] > 100f)
                    npc.velocity.X += 0.023f;
                else
                    npc.velocity.X -= 0.023f;
                if (npc.ai[0] > 200f)
                    npc.ai[0] = -200f;

                npc.velocity.X += between.X * 0.007f;
                npc.velocity.Y += between.Y * 0.007f;
                npc.rotation = npc.velocity.ToRotation();

                if (npc.velocity.X > 1.5f)
                    npc.velocity.X *= 0.9f;
                if (npc.velocity.X < -1.5f)
                    npc.velocity.X *= 0.9f;
                if (npc.velocity.Y > 1.5f)
                    npc.velocity.Y *= 0.9f;
                if (npc.velocity.Y < -1.5f)
                    npc.velocity.Y *= 0.9f;

                npc.velocity.X = MathHelper.Clamp(npc.velocity.X, -3f, 3f);
                npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y, -3f, 3f);
            }
            else //CHASE TARGET
            {
                if (npc.velocity.X < between.X)
                {
                    npc.velocity.X += chaseAcceleration;
                    if (npc.velocity.X < 0f && between.X > 0f)
                        npc.velocity.X += chaseAcceleration;
                }
                else if (npc.velocity.X > between.X)
                {
                    npc.velocity.X -= chaseAcceleration;
                    if (npc.velocity.X > 0f && between.X < 0f)
                        npc.velocity.X -= chaseAcceleration;
                }
                if (npc.velocity.Y < between.Y)
                {
                    npc.velocity.Y += chaseAcceleration;
                    if (npc.velocity.Y < 0f && between.Y > 0f)
                        npc.velocity.Y += chaseAcceleration;
                }
                else if (npc.velocity.Y > between.Y)
                {
                    npc.velocity.Y -= chaseAcceleration;
                    if (npc.velocity.Y > 0f && between.Y < 0f)
                        npc.velocity.Y -= chaseAcceleration;
                }
                npc.rotation = between.ToRotation();
            }

            // DAMP COLLISIONS OFF OF WALLS
            float collisionDamp = 0.5f;
            if (npc.collideX)
            {
                npc.netUpdate = true;
                npc.velocity.X = npc.oldVelocity.X * -collisionDamp;

                if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                    npc.velocity.X = 2f;
                if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                    npc.velocity.X = -2f;
            }
            if (npc.collideY)
            {
                npc.netUpdate = true;
                npc.velocity.Y = npc.oldVelocity.Y * -collisionDamp;

                if (npc.velocity.Y > 0f && npc.velocity.Y < 1.5f)
                    npc.velocity.Y = 2f;
                if (npc.velocity.Y < 0f && npc.velocity.Y > -1.5f)
                    npc.velocity.Y = -2f;
            }

            if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
            {
                npc.netUpdate = true;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int x = (int)npc.Center.X / 16;
                int y = (int)npc.Center.Y / 16;
                bool flag = false;

                for (int i = x - 1; i <= x + 1; i++)
                {
                    for (int j = y - 1; j <= y + 1; j++)
                    {
                        if (Main.tile[i, j].WallType > WallID.None)
                            flag = true;
                    }
                }
                if (!flag)
                {
                    npc.Transform(transformType);
                    return;
                }
            }
        }

        public static void DoVultureAI(NPC npc, float acceleration = 0.1f, float maxSpeed = 3f, int sitWidth = 30, int flyWidth = 50, int rangeX = 100, int rangeY = 100)
        {
            npc.localAI[0]++;
            npc.noGravity = true;
            npc.TargetClosest(true);

            if (npc.ai[0] == 0f)
            {
                npc.width = sitWidth;
                npc.noGravity = false;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (npc.velocity.X != 0f || npc.velocity.Y < 0f || npc.velocity.Y > 0.3)
                    {
                        npc.ai[0] = 1f;
                        npc.netUpdate = true;
                    }
                    else
                    {
                        Rectangle playerRect = Main.player[npc.target].getRect();
                        Rectangle rangeRect = new Rectangle((int)npc.Center.X - rangeX, (int)npc.Center.Y - rangeY, rangeX * 2, rangeY * 2);
                        if (npc.localAI[0] > 20f && (rangeRect.Intersects(playerRect) || npc.life < npc.lifeMax))
                        {
                            npc.ai[0] = 1f;
                            npc.velocity.Y -= 6f;
                            npc.netUpdate = true;
                        }
                    }
                }
            }
            else if (!Main.player[npc.target].dead)
            {
                npc.width = flyWidth;

                //Collision damping
                if (npc.collideX)
                {
                    npc.velocity.X = npc.oldVelocity.X * -0.5f;

                    if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                        npc.velocity.X = 2f;
                    if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                        npc.velocity.X = -2f;
                }

                if (npc.collideY)
                {
                    npc.velocity.Y = npc.oldVelocity.Y * -0.5f;

                    if (npc.velocity.Y > 0f && npc.velocity.Y < 1f)
                        npc.velocity.Y = 1f;
                    if (npc.velocity.Y < 0f && npc.velocity.Y > -1f)
                        npc.velocity.Y = -1f;
                }

                if (npc.direction == -1 && npc.velocity.X > -maxSpeed)
                {
                    npc.velocity.X -= acceleration;

                    if (npc.velocity.X > maxSpeed)
                        npc.velocity.X -= acceleration;
                    else if (npc.velocity.X > 0f)
                        npc.velocity.X -= acceleration * 0.5f;
                    if (npc.velocity.X < -maxSpeed)
                        npc.velocity.X = -maxSpeed;
                }
                else if (npc.direction == 1 && npc.velocity.X < maxSpeed)
                {
                    npc.velocity.X += acceleration;

                    if (npc.velocity.X < -maxSpeed)
                        npc.velocity.X += acceleration;
                    else if (npc.velocity.X < 0f)
                        npc.velocity.X += acceleration * 0.5f;
                    if (npc.velocity.X > maxSpeed)
                        npc.velocity.X = maxSpeed;
                }

                float xDistance = Math.Abs(npc.Center.X - Main.player[npc.target].Center.X);
                float yLimiter = Main.player[npc.target].position.Y - (npc.height / 2f);
                if (xDistance > 50f)
                    yLimiter -= 100f;

                if (npc.position.Y < yLimiter)
                {
                    npc.velocity.Y += acceleration * 0.5f;

                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y += acceleration * 0.1f;
                }
                else
                {
                    npc.velocity.Y -= acceleration * 0.5f;

                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y -= acceleration * 0.1f;
                }

                if (npc.velocity.Y < -maxSpeed)
                    npc.velocity.Y = -maxSpeed;
                if (npc.velocity.Y > maxSpeed)
                    npc.velocity.Y = maxSpeed;
            }
            // Change velocity if wet.
            if (npc.wet)
            {
                if (npc.velocity.Y > 0f)
                    npc.velocity.Y *= 0.95f;
                npc.velocity.Y -= 0.5f;
                if (npc.velocity.Y < -4f)
                    npc.velocity.Y = -4f;
            }
        }

        /// <summary>
        /// Allows you to spawn dust on the NPC in a certain place. Uses the npc.position value as the base point for the rectangle.
        /// Takes direction and rotation into account.
        /// </summary>
        /// <param name="frameWidth">The width of the sheet for the NPC.</param>
        /// <param name="rect">The place to put a dust.</param>
        /// <param name="chance">The chance to spawn a dust (0.3 = 30%)</param>
        public static Dust SpawnDustOnNPC(NPC npc, int frameWidth, int frameHeight, int dustType, Rectangle rect, Vector2 velocity = default, float chance = 0.5f, bool useSpriteDirection = false)
        {
            Vector2 half = new Vector2(frameWidth / 2f, frameHeight / 2f);

            // "Flip" the rectangle's position x-wise.
            if ((!useSpriteDirection && npc.direction == 1) || (useSpriteDirection && npc.spriteDirection == 1))
            {
                rect.X = frameWidth - rect.Right;
            }

            if (Main.rand.NextFloat(1f) < chance)
            {
                Vector2 offset = npc.Center - half + new Vector2(Main.rand.NextFloat(rect.Left, rect.Right), Main.rand.NextFloat(rect.Top, rect.Bottom)) - npc.Center;
                offset = offset.RotatedBy(npc.rotation);
                Dust d = Dust.NewDustPerfect(npc.Center + offset, dustType, velocity);
                return d;
            }
            return null;
        }
        #endregion

        #region Bestiary
        public override void SetBestiary(NPC npc, BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // Replace vanilla bestiary flavor text for certain NPCs
            // These are ordered by their order in the bestiary, if you're wondering why it seems so arbitrary lmao
            switch (npc.netID)
            {
                case NPCID.Guide:
                case NPCID.Dryad:
                case NPCID.Mechanic:
                case NPCID.EmpressButterfly:
                case NPCID.DemonEye:
                case NPCID.CataractEye:
                case NPCID.DialatedEye:
                case NPCID.SleepyEye:
                case NPCID.GreenEye:
                case NPCID.PurpleEye:
                case NPCID.Wraith:
                case NPCID.BloodNautilus:
                case NPCID.DiggerHead:
                case NPCID.UndeadMiner:
                case NPCID.GraniteGolem:
                case NPCID.GraniteFlyer:
                case NPCID.GreekSkeleton:
                case NPCID.UndeadViking:
                case NPCID.IcyMerman:
                case NPCID.IceElemental:
                case NPCID.DesertBeast:
                case NPCID.DuneSplicerHead:
                case NPCID.SandElemental:
                case NPCID.SandShark:
                case NPCID.SandsharkCorrupt:
                case NPCID.SandsharkCrimson:
                case NPCID.SandsharkHallow:
                case NPCID.MeteorHead:
                case NPCID.AngryBones:
                case NPCID.AngryBonesBig:
                case NPCID.AngryBonesBigMuscle:
                case NPCID.AngryBonesBigHelmet:
                case NPCID.BlueArmoredBones:
                case NPCID.BlueArmoredBonesMace:
                case NPCID.BlueArmoredBonesNoPants:
                case NPCID.BlueArmoredBonesSword:
                case NPCID.HellArmoredBones:
                case NPCID.HellArmoredBonesSpikeShield:
                case NPCID.HellArmoredBonesMace:
                case NPCID.HellArmoredBonesSword:
                case NPCID.RustyArmoredBonesAxe:
                case NPCID.RustyArmoredBonesFlail:
                case NPCID.RustyArmoredBonesSword:
                case NPCID.RustyArmoredBonesSwordNoArmor:
                case NPCID.SkeletonSniper:
                case NPCID.TacticalSkeleton:
                case NPCID.SkeletonCommando:
                case NPCID.BoneLee:
                case NPCID.Paladin:
                case NPCID.DiabolistRed:
                case NPCID.DiabolistWhite:
                case NPCID.Necromancer:
                case NPCID.NecromancerArmored:
                case NPCID.RaggedCaster:
                case NPCID.RaggedCasterOpenCoat:
                case NPCID.DungeonGuardian:
                case NPCID.BoneSerpentHead:
                case NPCID.Demon:
                case NPCID.VoodooDemon:
                case NPCID.RedDevil:
                case NPCID.WyvernHead:
                case NPCID.Harpy:
                case NPCID.MartianProbe:
                case NPCID.SeekerHead:
                case NPCID.DesertDjinn:
                case NPCID.ChaosElemental:
                case NPCID.GoblinThief:
                case NPCID.GoblinSummoner:
                case NPCID.GoblinSorcerer:
                case NPCID.PirateCaptain:
                case NPCID.MartianSaucerCore:
                case NPCID.TorchGod:
                case NPCID.EyeofCthulhu:
                case NPCID.BrainofCthulhu:
                case NPCID.SkeletronHead:
                case NPCID.WallofFlesh:
                case NPCID.QueenSlimeBoss:
                case NPCID.Retinazer:
                case NPCID.Spazmatism:
                case NPCID.TheDestroyer:
                case NPCID.SkeletronPrime:
                case NPCID.Plantera:
                case NPCID.HallowBoss:
                case NPCID.Golem:
                case NPCID.DukeFishron:
                case NPCID.CultistBoss:
                case NPCID.CultistDevote:
                case NPCID.LunarTowerNebula:
                case NPCID.LunarTowerSolar:
                case NPCID.LunarTowerVortex:
                case NPCID.LunarTowerStardust:
                case NPCID.MoonLordCore:
                    FlavorTextBestiaryInfoElement f = new("Hi CS0120");
                    bestiaryEntry.Info.RemoveAll(i => i.GetType() == f.GetType());
                    bestiaryEntry.Info.Add(new FlavorTextBestiaryInfoElement(CalamityUtils.GetTextValue($"Bestiary.Vanilla.{Lang.GetNPCName(npc.netID).Key}")));
                    break;
                default:
                    break;

            }

            // Create a string array containing all an NPC's debuff resistances
            string[] elements =
            [
                NPCDebuffResistText(npc.Calamity().VulnerableToCold, CalamityUtils.GetTextValue("UI.DebuffSystem.Cold")),
                NPCDebuffResistText(npc.Calamity().VulnerableToElectricity, CalamityUtils.GetTextValue("UI.DebuffSystem.Electricity")),
                NPCDebuffResistText(npc.Calamity().VulnerableToHeat, CalamityUtils.GetTextValue("UI.DebuffSystem.Heat")),
                NPCDebuffResistText(npc.Calamity().VulnerableToSickness, CalamityUtils.GetTextValue("UI.DebuffSystem.Sickness")),
                NPCDebuffResistText(npc.Calamity().VulnerableToWater, CalamityUtils.GetTextValue("UI.DebuffSystem.Water"))
            ];

            // Insert the debuff info into the NPC's bestiary entry
            bool force = npc.type == ModContent.NPCType<Burrower>(); //Force Burrower to always show the debuff section
            bestiaryEntry.Info.Insert(0, new BestiaryDebuffInfo(elements,force));

            // Add the Astral Infection to the Enchanted Nightcrawler's entry as it spawns there now
            if (npc.type == NPCID.EnchantedNightcrawler)
                bestiaryEntry.AddTags(GetInstance<AstralInfectionBiome>().ModBiomeBestiaryInfoElement);

            // Add the Surface Mushroom biome to the Truffle Worm's entry as it spawns there now
            if (npc.type == NPCID.TruffleWorm)
                bestiaryEntry.AddTags(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.SurfaceMushroom);

            // Remove the static portraits from vanilla worms so that Calamity's worm movement can be added in PreDraw
            switch (npc.netID)
            {
                case NPCID.DiggerHead:
                case NPCID.GiantWormHead:
                case NPCID.EaterofWorldsHead:
                case NPCID.WyvernHead:
                case NPCID.StardustWormHead:
                case NPCID.SolarCrawltipedeHead:
                case NPCID.CultistDragonHead:
                case NPCID.TheDestroyer:
                case NPCID.LeechHead:
                case NPCID.DevourerHead:
                case NPCID.TombCrawlerHead:
                case NPCID.DuneSplicerHead:
                case NPCID.BloodEelHead:
                case NPCID.BoneSerpentHead:
                case NPCID.SeekerHead:
                    NPCID.Sets.NPCBestiaryDrawOffset[npc.type] = NPCID.Sets.NPCBestiaryDrawOffset[npc.type] with { CustomTexturePath = null };
                    break;
            }
        }

        public static string NPCDebuffResistText(bool? effectiveness, string name)
        {
            string result = CalamityUtils.GetTextValue("UI.DebuffSystem.Neutral");
            if (effectiveness == true)
                result = CalamityUtils.GetTextValue("UI.DebuffSystem.Weak");
            else if (effectiveness == false)
                result = CalamityUtils.GetTextValue("UI.DebuffSystem.Resistant");

            result += " " + CalamityUtils.GetTextValue("UI.DebuffSystem.To") + " " + name;
            return result;
        }
        #endregion
    }
}
