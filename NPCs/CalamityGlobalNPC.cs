using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalamityMod.Balancing;
using CalamityMod.Buffs;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Placeables;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.Summon.Whips;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.Graphics.Drawers;
using CalamityMod.Items.Accessories;
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
using CalamityMod.NPCs.Crags;
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
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.VanillaNPCOverrides.Bosses;
using CalamityMod.NPCs.VanillaNPCOverrides.RegularEnemies;
using CalamityMod.Particles;
using CalamityMod.Projectiles;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Systems;
using CalamityMod.UI;
using CalamityMod.Walls.DraedonStructures;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Events;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.Utilities;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.NPCs
{
    public partial class CalamityGlobalNPC : GlobalNPC
    {
        #region Variables
        public float DR { get; set; } = 0f;

        /// <summary>
        /// If this is set to true, the NPC's DR cannot be reduced via any means. This applies regardless of whether customDR is true or false.
        /// </summary>
        public bool unbreakableDR = false;

        /// <summary>
        /// Overrides the normal DR math and uses custom DR reductions for each debuff, registered separately.<br></br>
        /// Used primarily by post-Moon Lord bosses.
        /// </summary>
        public bool customDR = false;
        public Dictionary<int, float> flatDRReductions = new Dictionary<int, float>();
        public Dictionary<int, float> multDRReductions = new Dictionary<int, float>();

        public int KillTime { get; set; } = 0;

        public const int DoGPhase1KillTime = 5400;

        // Debuff Vulnerabilities
        // null = neutral, true = vulnerable, false = resistant
        // neutral = 100% effective, vulnerable = 400% effective DoT and 200% effective non-DoT effects (within reason, some aren't exactly 200% more effective), resistant = 50% effective
        public bool? VulnerableToHeat = null;
        public bool? VulnerableToCold = null;
        public bool? VulnerableToSickness = null;
        public bool? VulnerableToElectricity = null;
        public bool? VulnerableToWater = null;

        public const double BaseDoTDamageMult = 1D;
        public const double VulnerableToDoTDamageMult = 2D;
        public const double VulnerableToDoTDamageMult_Worms_SlimeGod = 1.5;

        // Eskimo Set and Cryo Stone effects
        public bool IncreasedColdEffects_EskimoSet = false;
        public bool IncreasedColdEffects_CryoStone = false;

        // Transformer effect

        public bool IncreasedElectricityEffects_Transformer = false;

        // Fireball, Cinnamon Roll and Hellfire Treads effects
        public bool IncreasedHeatEffects_Fireball = false;
        public bool IncreasedHeatEffects_CinnamonRoll = false;
        public bool IncreasedHeatEffects_HellfireTreads = false;

        // Toxic Heart effect
        public bool IncreasedSicknessEffects_ToxicHeart = false;

        // Evergreen Gin effect
        public bool IncreasedSicknessAndWaterEffects_EvergreenGin = false;

        // Biome enrage timer max
        public const int biomeEnrageTimerMax = 300;

        // Variable for certain worm bosses used to prevent them from moving too fast upon swapping phases while far away from their target
        // Currently only used by DoG
        public float velocityPriorToPhaseSwap = 0f;
        public const float velocityPriorToPhaseSwapIncrement = 0.1f;

        public bool ShouldFallThroughPlatforms;

        /// <summary>
        /// Allows hostile NPCs to deal damage to the player's defense stat, used mostly for hard-hitting bosses.
        /// </summary>
        public bool canBreakPlayerDefense = false;

        // Total defense loss from some true melee hits and other things that reduce defense
        public int miscDefenseLoss = 0;

        // Distance values for when bosses increase velocity to catch up to their target
        public const float CatchUpDistance200Tiles = 3200f;
        public const float CatchUpDistance350Tiles = 5600f;

        // Boss Zen distance
        private const float BossZenDistance = 6400f;

        // Used to nerf desert prehardmode enemies pre-Desert Scourge
        private const double DesertEnemyStatMultiplier = 0.75;

        // Used to increase coin drops in Normal Mode
        private const double NPCValueMultiplier_NormalCalamity = 1.5;

        // Used to decrease coin drops in Expert Mode
        private const double NPCValueMultiplier_ExpertVanilla = 2.5;

        // Used to change the Expert Mode coin drop multiplier
        private const double NPCValueMultiplier_ExpertCalamity = 1.5;

        // Max velocity used in contact damage scaling
        public float maxVelocity = 0f;

        // Dash damage immunity timer
        public const int maxPlayerImmunities = Main.maxPlayers + 1;
        public int[] dashImmunityTime = new int[maxPlayerImmunities];

        // Town NPC shop alert animation variables
        public int shopAlertAnimTimer = 0;
        public int shopAlertAnimFrame = 0;

        // Set to false for this NPC to be unable to generate proximity Rage, regardless of other factors
        public bool ProvidesProximityRage = true;

        // NewAI
        internal const int maxAIMod = 4;
        public float[] newAI = new float[maxAIMod];
        public int AITimer = 0;

        // Town NPC Patreon
        public bool setNewName = true;

        // Stuff used by the Boss Health UI
        public bool SplittingWorm = false;
        public bool CanHaveBossHealthBar = false;
        public bool ShouldCloseHPBar = false;

        // Timer for how long an NPC is immune to certain debuffs
        public const int slowingDebuffResistanceMin = 1800;
        public int debuffResistanceTimer = 0;

        // If a boss is affected by knockback and a timer for how long that boss is immune to knockback after being knocked back
        public bool bossCanBeKnockedBack = false;
        public const int knockbackResistanceMin = 180;
        public int knockbackResistanceTimer = 0;

        // Debuffs
        public int vaporfied = 0;
        public int timeSlow = 0;
        public int gState = 0;
        public int tesla = 0;
        public int tSad = 0;
        public int eutrophication = 0;
        public int webbed = 0;
        public int slowed = 0;
        public int electrified = 0;
        public int pearlAura = 0;
        public int bBlood = 0;
        public int brainRot = 0;
        public int elementalMix = 0;
        public int marked = 0;
        public int absorberAffliction = 0;
        public int irradiated = 0;
        public int bFlames = 0;
        public int hFlames = 0;
        public int pFlames = 0;
        public int aCrunch = 0;
        public int crumble = 0;

        // Soma Prime Shred deals damage with DirectStrikes instead of with direct debuff damage
        // It also stacks, scales with ranged damage, and can crit, meaning it needs to know who applied it most recently
        public int somaShredStacks = 0;
        public int somaShredApplicator = -1;
        // Reduced by the number of active stacks every frame. If it hits zero, one stack disappears.
        public int somaShredFalloff = Shred.StackFalloffFrames;

        public int cDepth = 0;
        public int rTide = 0;
        public int gsInferno = 0;
        public int dragonFire = 0;
        public int miracleBlight = 0;
        public int astralInfection = 0;
        public int wDeath = 0;
        public int nightwither = 0;
        public int shellfishVore = 0;
        public int clamDebuff = 0;
        public int sulphurPoison = 0;
        public int ladHearts = 0;
        public int kamiFlu = 0;
        public int relicOfResilienceCooldown = 0;
        public int relicOfResilienceWeakness = 0;
        public int GaussFluxTimer = 0;
        public int sagePoisonTime = 0;
        public int sagePoisonDamage = 0;
        public int vulnerabilityHex = 0;
        public int banishingFire = 0;
        public int wither = 0;
        public int RancorBurnTime = 0;

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

        // Boss Enrage variable for use with the boss health UI.
        // The logic behind this is as follows:
        // 1 - For special cases with super-enrages (specifically Yharon/SCal with their arenas), go solely based on whether that enrage is active. That information is most important to the player.
        // 2 - Otherwise, check if the demonshade enrage is active. If it is, register this as true. If not, go to step 3.
        // 3 - Check if a specific enrage condition (such as Duke Fishron's Ocean check) is met. If it is, and Boss Rush is not active, set this to true. If not, go to step 4.
        // 4 - Check if Boss Rush isn't active. If so, set this to true.
        public bool CurrentlyEnraged;

        // Increased defense or DR variable for use with the boss health UI.
        // The logic behind this is as follows:
        // 1 - When bosses are transitioning phases they gain a massive DR increase.
        // 2 - When bosses are using certain attacks that make them particularly vulnerable they gain a massive DR or defense increase.
        public bool CurrentlyIncreasingDefenseOrDR;

        // Other Boss Rush stuff
        public bool DoesNotDisappearInBossRush;

        // On-Kill variables
        public bool gladiatorOnKill = true;

        // Variable for if enemy has been recently hit by an ArcZap
        public int arcZapCooldown = 0;
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
            myClone.flatDRReductions = new Dictionary<int, float>();
            foreach (var flatDR in flatDRReductions)
                myClone.flatDRReductions.Add(flatDR.Key, flatDR.Value);
            myClone.multDRReductions = new Dictionary<int, float>();
            foreach (var multDR in multDRReductions)
                myClone.multDRReductions.Add(multDR.Key, multDR.Value);

            myClone.KillTime = KillTime;

            myClone.VulnerableToHeat = VulnerableToHeat;
            myClone.VulnerableToCold = VulnerableToCold;
            myClone.VulnerableToSickness = VulnerableToSickness;
            myClone.VulnerableToElectricity = VulnerableToElectricity;
            myClone.VulnerableToWater = VulnerableToWater;

            myClone.IncreasedColdEffects_EskimoSet = IncreasedColdEffects_EskimoSet;
            myClone.IncreasedColdEffects_CryoStone = IncreasedColdEffects_CryoStone;
            myClone.IncreasedElectricityEffects_Transformer = IncreasedElectricityEffects_Transformer;
            myClone.IncreasedHeatEffects_Fireball = IncreasedHeatEffects_Fireball;
            myClone.IncreasedHeatEffects_CinnamonRoll = IncreasedHeatEffects_CinnamonRoll;
            myClone.IncreasedHeatEffects_HellfireTreads = IncreasedHeatEffects_HellfireTreads;
            myClone.IncreasedSicknessEffects_ToxicHeart = IncreasedSicknessEffects_ToxicHeart;
            myClone.IncreasedSicknessAndWaterEffects_EvergreenGin = IncreasedSicknessAndWaterEffects_EvergreenGin;

            myClone.velocityPriorToPhaseSwap = velocityPriorToPhaseSwap;

            myClone.ShouldFallThroughPlatforms = ShouldFallThroughPlatforms;

            myClone.canBreakPlayerDefense = canBreakPlayerDefense;

            myClone.miscDefenseLoss = miscDefenseLoss;

            myClone.maxVelocity = maxVelocity;

            myClone.dashImmunityTime = new int[maxPlayerImmunities];
            for (int i = 0; i < maxPlayerImmunities; ++i)
                myClone.dashImmunityTime[i] = dashImmunityTime[i];

            myClone.shopAlertAnimTimer = shopAlertAnimTimer;
            myClone.shopAlertAnimFrame = shopAlertAnimFrame;

            myClone.ProvidesProximityRage = ProvidesProximityRage;

            myClone.newAI = new float[maxAIMod];
            for (int i = 0; i < maxAIMod; ++i)
                myClone.newAI[i] = newAI[i];
            myClone.AITimer = AITimer;

            myClone.setNewName = setNewName;

            myClone.SplittingWorm = SplittingWorm;
            myClone.CanHaveBossHealthBar = CanHaveBossHealthBar;
            myClone.ShouldCloseHPBar = ShouldCloseHPBar;

            myClone.debuffResistanceTimer = debuffResistanceTimer;

            myClone.bossCanBeKnockedBack = bossCanBeKnockedBack;
            myClone.knockbackResistanceTimer = knockbackResistanceTimer;

            myClone.vaporfied = vaporfied;
            myClone.timeSlow = timeSlow;
            myClone.gState = gState;
            myClone.tesla = tesla;
            myClone.tSad = tSad;
            myClone.eutrophication = eutrophication;
            myClone.webbed = webbed;
            myClone.slowed = slowed;
            myClone.electrified = electrified;
            myClone.pearlAura = pearlAura;
            myClone.bBlood = bBlood;
            myClone.brainRot = brainRot;
            myClone.elementalMix = elementalMix;
            myClone.marked = marked;
            myClone.absorberAffliction = absorberAffliction;
            myClone.irradiated = irradiated;
            myClone.bFlames = bFlames;
            myClone.hFlames = hFlames;
            myClone.pFlames = pFlames;
            myClone.aCrunch = aCrunch;
            myClone.crumble = crumble;

            myClone.somaShredStacks = somaShredStacks;
            myClone.somaShredApplicator = somaShredApplicator;
            myClone.somaShredFalloff = somaShredFalloff;

            myClone.cDepth = cDepth;
            myClone.rTide = rTide;
            myClone.gsInferno = gsInferno;
            myClone.miracleBlight = miracleBlight;
            myClone.dragonFire = dragonFire;
            myClone.astralInfection = astralInfection;
            myClone.wDeath = wDeath;
            myClone.nightwither = nightwither;
            myClone.shellfishVore = shellfishVore;
            myClone.clamDebuff = clamDebuff;
            myClone.sulphurPoison = sulphurPoison;
            myClone.ladHearts = ladHearts;
            myClone.kamiFlu = kamiFlu;
            myClone.relicOfResilienceCooldown = relicOfResilienceCooldown;
            myClone.relicOfResilienceWeakness = relicOfResilienceWeakness;
            myClone.GaussFluxTimer = GaussFluxTimer;
            myClone.sagePoisonTime = sagePoisonTime;
            myClone.sagePoisonDamage = sagePoisonDamage;
            myClone.vulnerabilityHex = vulnerabilityHex;
            myClone.banishingFire = banishingFire;
            myClone.wither = wither;
            myClone.RancorBurnTime = RancorBurnTime;

            // This gets set up as needed.
            myClone.VulnerabilityHexFireDrawer = null;

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
            ResetSavedIndex(ref slimeGodPurple, NPCType<SlimeGod.EbonianPaladin>(), NPCType<SplitEbonianPaladin>());
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
        }
        #endregion

        #region Life Regen
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (npc.damage > 0 && !npc.boss && !npc.friendly && !npc.dontTakeDamage && BiomeTileCounterSystem.SulphurTiles > 30 &&
                !npc.buffImmune[BuffID.Poisoned] && !npc.buffImmune[BuffType<CrushDepth>()])
            {
                if (npc.wet)
                    npc.AddBuff(BuffID.Poisoned, 2);

                if (Main.raining)
                    npc.AddBuff(BuffType<Irradiated>(), 2);
            }

            // Special venom debuff case for the Lionfish and certain other projectiles.
            if (npc.venom)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                int projectileCount = 0;
                for (int j = 0; j < Main.maxProjectiles; j++)
                {
                    if (Main.projectile[j].active &&
                        (Main.projectile[j].type == ProjectileType<LionfishProj>() || Main.projectile[j].type == ProjectileType<LeviathanTooth>() || Main.projectile[j].type == ProjectileType<JawsProjectile>()) &&
                        Main.projectile[j].ai[0] == 1f && Main.projectile[j].ai[1] == npc.whoAmI)
                    {
                        projectileCount++;
                    }
                }

                if (projectileCount > 0)
                {
                    npc.lifeRegen -= projectileCount * 30;

                    if (damage < projectileCount * 6)
                        damage = projectileCount * 6;
                }
            }

            if (npc.javelined)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                int projectileCount = 0;
                for (int j = 0; j < Main.maxProjectiles; j++)
                {
                    if (Main.projectile[j].active && Main.projectile[j].type == ProjectileType<BonebreakerProjectile>() &&
                        Main.projectile[j].ai[0] == 1f && Main.projectile[j].ai[1] == npc.whoAmI)
                    {
                        projectileCount++;
                    }
                }

                if (projectileCount > 0)
                {
                    npc.lifeRegen -= projectileCount * 20;

                    if (damage < projectileCount * 4)
                        damage = projectileCount * 4;
                }
            }

            if (shellfishVore > 0)
            {
                int projectileCount = 0;
                int owner = 255;
                for (int j = 0; j < Main.maxProjectiles; j++)
                {
                    if (Main.projectile[j].active && Main.projectile[j].type == ProjectileType<Shellfish>() &&
                        Main.projectile[j].ai[0] == 1f && Main.projectile[j].ai[1] == npc.whoAmI)
                    {
                        owner = Main.projectile[j].owner;
                        projectileCount++;
                        if (projectileCount >= 5)
                        {
                            projectileCount = 5;
                            break;
                        }
                    }
                }

                Item heldItem = Main.player[owner].ActiveItem();
                int totalDamage = (int)Main.player[owner].GetTotalDamage<SummonDamageClass>().ApplyTo(150f);
                bool forbidden = Main.player[owner].head == ArmorIDs.Head.AncientBattleArmor && Main.player[owner].body == ArmorIDs.Body.AncientBattleArmor && Main.player[owner].legs == ArmorIDs.Legs.AncientBattleArmor;
                bool reducedNerf = Main.player[owner].Calamity().fearmongerSet || (forbidden && heldItem.CountsAsClass<MagicDamageClass>());

                double summonNerfMult = reducedNerf ? 0.75 : 0.5;
                if (!Main.player[owner].Calamity().profanedCrystalBuffs)
                {
                    if (heldItem.type > ItemID.None)
                    {
                        if (!heldItem.CountsAsClass<SummonDamageClass>() &&
                            (heldItem.CountsAsClass<MeleeDamageClass>() || heldItem.CountsAsClass<RangedDamageClass>() || heldItem.CountsAsClass<MagicDamageClass>() || heldItem.CountsAsClass<ThrowingDamageClass>()) &&
                            heldItem.hammer == 0 && heldItem.pick == 0 && heldItem.axe == 0 && heldItem.useStyle != 0 &&
                            !heldItem.accessory && heldItem.ammo == AmmoID.None)
                        {
                            totalDamage = (int)(totalDamage * summonNerfMult);
                        }
                    }
                }

                int totalDisplayedDamage = totalDamage / 5;
                ApplyDPSDebuff(projectileCount * totalDamage, projectileCount * totalDisplayedDamage, ref npc.lifeRegen, ref damage);
            }

            if (clamDebuff > 0)
            {
                int projectileCount = 0;
                for (int j = 0; j < Main.maxProjectiles; j++)
                {
                    if (Main.projectile[j].active && Main.projectile[j].ai[0] == 1f && Main.projectile[j].ai[1] == npc.whoAmI)
                    {
                        if (Main.projectile[j].type == ProjectileType<SnapClamProj>())
                            projectileCount += 2;
                        if (Main.projectile[j].type == ProjectileType<SnapClamStealth>())
                            projectileCount++;
                    }
                }

                ApplyDPSDebuff(projectileCount * 15, projectileCount * 3, ref npc.lifeRegen, ref damage);
            }

            if (irradiated > 0)
            {
                int projectileCount = 0;
                for (int j = 0; j < Main.maxProjectiles; j++)
                {
                    if (Main.projectile[j].active && Main.projectile[j].type == ProjectileType<WaterLeechProj>() &&
                        Main.projectile[j].ai[0] == 1f && Main.projectile[j].ai[1] == npc.whoAmI)
                    {
                        projectileCount++;
                    }
                }

                if (projectileCount > 0)
                    ApplyDPSDebuff(projectileCount * 20, projectileCount * 4, ref npc.lifeRegen, ref damage);
                else
                    ApplyDPSDebuff(20, 4, ref npc.lifeRegen, ref damage);
            }

            // Glacial State and Temporal Sadness don't work on normal/expert Queen Bee.
            if (debuffResistanceTimer <= 0 || (debuffResistanceTimer > slowingDebuffResistanceMin))
            {
                if (npc.type != NPCID.QueenBee || CalamityWorld.revenge || BossRushEvent.BossRushActive)
                {
                    float baseXVelocityMult = 0.9f;
                    float baseYVelocityIncrease = 0.05f;
                    if (VulnerableToCold.HasValue)
                    {
                        if (VulnerableToCold.Value)
                        {
                            baseXVelocityMult = 0.5f;
                            baseYVelocityIncrease = 0.2f;
                        }
                        else
                        {
                            baseXVelocityMult = 0.98f;
                            baseYVelocityIncrease = 0.01f;
                        }
                    }

                    if (gState > 0)
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            npc.velocity.X *= baseXVelocityMult;
                            npc.velocity.Y += baseYVelocityIncrease * 0.5f;
                            if (npc.velocity.Y > 15f)
                                npc.velocity.Y = 15f;
                        }
                        else
                        {
                            npc.velocity *= baseXVelocityMult;
                        }
                    }
                    else if (tSad > 0)
                        npc.velocity *= 0.5f;
                }
            }

            // Debuff vulnerabilities and resistances.
            // Damage multiplier calcs.
            // Worms that are vulnerable to debuffs and Slime God slimes take reduced damage from vulnerabilities.
            bool wormBoss = CalamityLists.DesertScourgeIDs.Contains(npc.type) || CalamityLists.EaterofWorldsIDs.Contains(npc.type) || CalamityLists.PerforatorIDs.Contains(npc.type) ||
                CalamityLists.AquaticScourgeIDs.Contains(npc.type) || CalamityLists.AstrumDeusIDs.Contains(npc.type) || CalamityLists.StormWeaverIDs.Contains(npc.type);
            bool slimeGod = CalamityLists.SlimeGodIDs.Contains(npc.type);

            bool slimed = npc.drippingSlime || npc.drippingSparkleSlime;
            double heatDamageMult = slimed ? ((wormBoss || slimeGod) ? VulnerableToDoTDamageMult_Worms_SlimeGod : VulnerableToDoTDamageMult) : BaseDoTDamageMult;
            if (VulnerableToHeat.HasValue)
            {
                if (VulnerableToHeat.Value)
                    heatDamageMult *= slimed ? ((wormBoss || slimeGod) ? 1.25 : 1.5) : ((wormBoss || slimeGod) ? VulnerableToDoTDamageMult_Worms_SlimeGod : VulnerableToDoTDamageMult);
                else
                    heatDamageMult *= slimed ? 0.2 : 0.5;
            }

            double coldDamageMult = BaseDoTDamageMult;
            if (VulnerableToCold.HasValue)
            {
                if (VulnerableToCold.Value)
                    coldDamageMult *= wormBoss ? VulnerableToDoTDamageMult_Worms_SlimeGod : VulnerableToDoTDamageMult;
                else
                    coldDamageMult *= 0.5;
            }

            double sicknessDamageMult = irradiated > 0 ? (wormBoss ? VulnerableToDoTDamageMult_Worms_SlimeGod : VulnerableToDoTDamageMult) : BaseDoTDamageMult;
            if (VulnerableToSickness.HasValue)
            {
                if (VulnerableToSickness.Value)
                    sicknessDamageMult *= irradiated > 0 ? (wormBoss ? 1.25 : 1.5) : (wormBoss ? VulnerableToDoTDamageMult_Worms_SlimeGod : VulnerableToDoTDamageMult);
                else
                    sicknessDamageMult *= irradiated > 0 ? 0.2 : 0.5;
            }

            bool increasedElectricityDamage = npc.wet || npc.honeyWet || npc.lavaWet || npc.dripping;
            double electricityDamageMult = increasedElectricityDamage ? (wormBoss ? VulnerableToDoTDamageMult_Worms_SlimeGod : VulnerableToDoTDamageMult) : BaseDoTDamageMult;
            if (VulnerableToElectricity.HasValue)
            {
                if (VulnerableToElectricity.Value)
                    electricityDamageMult *= increasedElectricityDamage ? (wormBoss ? 1.25 : 1.5) : (wormBoss ? VulnerableToDoTDamageMult_Worms_SlimeGod : VulnerableToDoTDamageMult);
                else
                    electricityDamageMult *= increasedElectricityDamage ? 0.2 : 0.5;
            }

            double waterDamageMult = BaseDoTDamageMult;
            if (VulnerableToWater.HasValue)
            {
                if (VulnerableToWater.Value)
                    waterDamageMult *= wormBoss ? VulnerableToDoTDamageMult_Worms_SlimeGod : VulnerableToDoTDamageMult;
                else
                    waterDamageMult *= 0.5;
            }

            if (IncreasedColdEffects_EskimoSet)
                coldDamageMult += 0.25;
            if (IncreasedColdEffects_CryoStone)
                coldDamageMult += 0.5;

            if (IncreasedElectricityEffects_Transformer)
                electricityDamageMult += 0.5;

            if (IncreasedHeatEffects_Fireball)
                heatDamageMult += 0.25;
            if (IncreasedHeatEffects_CinnamonRoll)
                heatDamageMult += 0.5;
            if (IncreasedHeatEffects_HellfireTreads)
                heatDamageMult += 0.5;
            
            if (IncreasedSicknessEffects_ToxicHeart)
                sicknessDamageMult += 0.5;

            if (IncreasedSicknessAndWaterEffects_EvergreenGin)
            {
                sicknessDamageMult += 0.25;
                waterDamageMult += 0.25;
            }

            // Subtract 1 for the vanilla damage multiplier because it's already dealing DoT in the vanilla regen code.
            double vanillaHeatDamageMult = heatDamageMult - BaseDoTDamageMult;
            double vanillaColdDamageMult = coldDamageMult - BaseDoTDamageMult;
            double vanillaSicknessDamageMult = sicknessDamageMult - BaseDoTDamageMult;

            // On Fire
            if (npc.onFire)
            {
                int baseOnFireDoTValue = (int)(12 * vanillaHeatDamageMult);
                npc.lifeRegen -= baseOnFireDoTValue;
                if (damage < baseOnFireDoTValue / 4)
                    damage = baseOnFireDoTValue / 4;
            }

            // Cursed Inferno
            if (npc.onFire2)
            {
                int baseCursedInfernoDoTValue = (int)(48 * vanillaHeatDamageMult);
                npc.lifeRegen -= baseCursedInfernoDoTValue;
                if (damage < baseCursedInfernoDoTValue / 4)
                    damage = baseCursedInfernoDoTValue / 4;
            }

            // Hellfire
            if (npc.onFire3)
            {
                int baseHellfireDoTValue = (int)(30 * vanillaHeatDamageMult);
                npc.lifeRegen -= baseHellfireDoTValue;
                if (damage < baseHellfireDoTValue / 4)
                    damage = baseHellfireDoTValue / 4;
            }

            // Daybroken
            // 18OCT2023: Ozzatron: im not gonna sugarcoat it
            // vanilla debuff damage from Daybreak impales scales linearly up to 8 for 800 DPS
            // instead of allowing this entire 800 DPS to be multiplied by heat weakness + heat DoT bonuses,
            // each Daybreak spear beyond the first is only affected 25% as much by weaknesses or resistances.
            // This also stops Daybreak's DPS from being utterly shafted by heat resistance.
            // As no other weapon can stack Daybroken, this has no effect on other weapons (they count as "1 Daybreak spear")
            if (npc.daybreak)
            {
                int numImpaledSpears = 0;
                for (int k = 0; k < Main.maxProjectiles; k++)
                {
                    if (Main.projectile[k].active && Main.projectile[k].type == ProjectileID.Daybreak && Main.projectile[k].ai[0] == 1f && Main.projectile[k].ai[1] == npc.whoAmI)
                        numImpaledSpears++;
                }

                // If there are no Daybreak impaled spears, Daybroken has 1x potency (it was applied some other way)
                float daybrokenMultiplier = numImpaledSpears <= 1 ? 1f : (1f + 0.25f * (numImpaledSpears - 1));

                int baseDaybreakDoTValue = (int)(daybrokenMultiplier * 2 * 100 * vanillaHeatDamageMult);
                npc.lifeRegen -= baseDaybreakDoTValue;
                if (damage < baseDaybreakDoTValue / 4)
                    damage = baseDaybreakDoTValue / 4;
            }

            // Shadowflame
            if (npc.shadowFlame)
            {
                int baseShadowFlameDoTValue = (int)(60 * vanillaHeatDamageMult);
                npc.lifeRegen -= baseShadowFlameDoTValue;
                if (damage < baseShadowFlameDoTValue / 4)
                    damage = baseShadowFlameDoTValue / 4;
            }

            // Brimstone Flames
            if (bFlames > 0)
            {
                int baseBrimstoneFlamesDoTValue = (int)(60 * heatDamageMult);
                ApplyDPSDebuff(baseBrimstoneFlamesDoTValue, baseBrimstoneFlamesDoTValue / 5, ref npc.lifeRegen, ref damage);
            }

            // Holy Flames
            if (hFlames > 0)
            {
                int baseHolyFlamesDoTValue = (int)(200 * heatDamageMult);
                ApplyDPSDebuff(baseHolyFlamesDoTValue, baseHolyFlamesDoTValue / 5, ref npc.lifeRegen, ref damage);
            }

            // God Slayer Inferno
            if (gsInferno > 0)
            {
                int baseGodSlayerInfernoDoTValue = (int)(250 * heatDamageMult);
                ApplyDPSDebuff(baseGodSlayerInfernoDoTValue, baseGodSlayerInfernoDoTValue / 5, ref npc.lifeRegen, ref damage);
            }

            // Dragonfire
            if (dragonFire > 0)
            {
                int baseDragonFireDoTValue = (int)(360 * heatDamageMult);
                ApplyDPSDebuff(baseDragonFireDoTValue, baseDragonFireDoTValue / 5, ref npc.lifeRegen, ref damage);
            }

            // Banishing Fire
            if (banishingFire > 0)
            {
                int baseBanishingFireDoTValue = (int)((npc.lifeMax >= 1000000 ? npc.lifeMax / 500 : 4000) * heatDamageMult);
                ApplyDPSDebuff(baseBanishingFireDoTValue, baseBanishingFireDoTValue / 5, ref npc.lifeRegen, ref damage);
            }

            // Vulnerability Hex
            if (vulnerabilityHex > 0)
            {
                int baseVulnerabilityHexDoTValue = (int)(VulnerabilityHex.DPS * heatDamageMult);
                ApplyDPSDebuff(baseVulnerabilityHexDoTValue, VulnerabilityHex.TickNumber, ref npc.lifeRegen, ref damage);
            }

            // Frostburn
            if (npc.onFrostBurn)
            {
                int baseFrostBurnDoTValue = (int)(16 * vanillaColdDamageMult);
                npc.lifeRegen -= baseFrostBurnDoTValue;
                if (damage < baseFrostBurnDoTValue / 4)
                    damage = baseFrostBurnDoTValue / 4;
            }

            // Frostbite
            if (npc.onFrostBurn2)
            {
                int baseFrostBiteDoTValue = (int)(50 * vanillaColdDamageMult);
                npc.lifeRegen -= baseFrostBiteDoTValue;
                if (damage < baseFrostBiteDoTValue / 4)
                    damage = baseFrostBiteDoTValue / 4;
            }

            // Oiled
            bool hasColdOil = npc.onFrostBurn || npc.onFrostBurn2;
            bool hasHotOil = npc.onFire || npc.onFire2 || npc.onFire3 || npc.shadowFlame;
            bool hasModHotOil = bFlames > 0 || hFlames > 0 || gsInferno > 0 || dragonFire > 0 || banishingFire > 0 || vulnerabilityHex > 0;
            if (npc.oiled && (hasColdOil || hasHotOil || hasModHotOil))
            {
                double multiplier = 1D;
                if (hasColdOil)
                    multiplier *= vanillaColdDamageMult;
                if (hasHotOil || (hasModHotOil && hasColdOil))
                    multiplier *= vanillaHeatDamageMult;
                if (hasModHotOil && !hasColdOil && !hasHotOil)
                    multiplier *= heatDamageMult;

                int baseOiledDoTValue = (int)(50 * multiplier);
                npc.lifeRegen -= baseOiledDoTValue;
                if (damage < baseOiledDoTValue / 4)
                    damage = baseOiledDoTValue / 4;
            }

            // Nightwither
            if (nightwither > 0)
            {
                int baseNightwitherDoTValue = (int)(200 * coldDamageMult);
                ApplyDPSDebuff(baseNightwitherDoTValue, baseNightwitherDoTValue / 5, ref npc.lifeRegen, ref damage);
            }

            // Plague
            if (pFlames > 0)
            {
                int basePlagueDoTValue = (int)(100 * sicknessDamageMult);
                ApplyDPSDebuff(basePlagueDoTValue, basePlagueDoTValue / 5, ref npc.lifeRegen, ref damage);
            }

            // Astral Infection
            if (astralInfection > 0)
            {
                int baseAstralInfectionDoTValue = (int)(75 * sicknessDamageMult);
                ApplyDPSDebuff(baseAstralInfectionDoTValue, baseAstralInfectionDoTValue / 5, ref npc.lifeRegen, ref damage);
            }

            // Sulphuric Poisoning
            if (sulphurPoison > 0)
            {
                int baseSulphurPoisonDoTValue = (int)(180 * sicknessDamageMult);
                ApplyDPSDebuff(baseSulphurPoisonDoTValue, baseSulphurPoisonDoTValue / 5, ref npc.lifeRegen, ref damage);
            }

            // Sage Poison
            if (sagePoisonTime > 0)
            {
                // npc.Calamity().sagePoisonDamage = 50 * (float)(Math.Pow(totalSageSpirits, 0.73D) + Math.Pow(totalSageSpirits, 1.1D)) * 0.5f
                // See SageNeedle.cs for details
                int baseSagePoisonDoTValue = (int)(npc.Calamity().sagePoisonDamage * sicknessDamageMult);
                ApplyDPSDebuff(baseSagePoisonDoTValue, baseSagePoisonDoTValue / 5, ref npc.lifeRegen, ref damage);
            }

            // Kami Debuff from Yanmei's Knife
            if (kamiFlu > 0)
            {
                int baseKamiFluDoTValue = (int)(250 * sicknessDamageMult);
                ApplyDPSDebuff(baseKamiFluDoTValue, baseKamiFluDoTValue / 10, ref npc.lifeRegen, ref damage);
            }

            //Absorber Affliction
            if (absorberAffliction > 0)
            {
                int baseAbsorberDoTValue = (int)(400 * sicknessDamageMult);
                ApplyDPSDebuff(baseAbsorberDoTValue, baseAbsorberDoTValue / 65, ref npc.lifeRegen, ref damage);
            }

            // Poisoned
            if (npc.poisoned)
            {
                int basePoisonedDoTValue = (int)(4 * vanillaSicknessDamageMult);
                npc.lifeRegen -= basePoisonedDoTValue;
                if (damage < basePoisonedDoTValue / 4)
                    damage = basePoisonedDoTValue / 4;
            }

            // Venom
            if (npc.venom)
            {
                int baseVenomDoTValue = (int)(60 * vanillaSicknessDamageMult);
                npc.lifeRegen -= baseVenomDoTValue;
                if (damage < baseVenomDoTValue / 4)
                    damage = baseVenomDoTValue / 4;
            }

            // Electrified
            if (electrified > 0)
            {
                int baseElectrifiedDoTValue = (int)(5 * (npc.velocity.X == 0 ? 1 : 4) * electricityDamageMult);
                ApplyDPSDebuff(baseElectrifiedDoTValue, baseElectrifiedDoTValue / 5, ref npc.lifeRegen, ref damage);
            }

            // Crush Depth
            if (cDepth > 0)
            {
                int baseCrushDepthDoTValue = (int)(100 * waterDamageMult);
                ApplyDPSDebuff(baseCrushDepthDoTValue, baseCrushDepthDoTValue / 2, ref npc.lifeRegen, ref damage);
            }
            
            //Riptide
            if (rTide > 0)
            {
                int baseRiptideDoTValue = (int)(40 * waterDamageMult);
                ApplyDPSDebuff(baseRiptideDoTValue, baseRiptideDoTValue / 3, ref npc.lifeRegen, ref damage);
            }

            // Debuffs that aren't affected by weaknesses or resistances.
            if (vaporfied > 0)
                ApplyDPSDebuff(30, 6, ref npc.lifeRegen, ref damage);
            if (somaShredStacks > 0)
                Shred.TickDebuff(npc, this);
            if (bBlood > 0)
                ApplyDPSDebuff(50, 10, ref npc.lifeRegen, ref damage);
            if (brainRot > 0)
                ApplyDPSDebuff(50, 10, ref npc.lifeRegen, ref damage);
            if (elementalMix > 0)
                ApplyDPSDebuff(400, 80, ref npc.lifeRegen, ref damage);
            if (miracleBlight > 0)
                ApplyDPSDebuff(2500, 500, ref npc.lifeRegen, ref damage);

            // Reduce DoT on worm bosses by 75%.
            if (wormBoss && npc.lifeRegen < 0)
            {
                npc.lifeRegen /= 4;
                if (npc.lifeRegen > -1)
                    npc.lifeRegen = -1;

                // Every other EoW body segment and the head segments are immune to DoT.
                if (((npc.ai[2] % 2f == 0f && npc.type == NPCID.EaterofWorldsBody) || npc.type == NPCID.EaterofWorldsHead) && (CalamityWorld.death || BossRushEvent.BossRushActive))
                    npc.lifeRegen = 0;
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

        #region Set Defaults
        public override void SetDefaults(NPC npc)
        {
            ShouldFallThroughPlatforms = false;

            for (int i = 0; i < maxPlayerImmunities; i++)
                dashImmunityTime[i] = 0;

            for (int m = 0; m < maxAIMod; m++)
                newAI[m] = 0f;

            // Apply DR to vanilla NPCs.
            // This also applies DR to other mods' NPCs who have set up their NPCs to have DR.
            if (CalamityMod.DRValues.ContainsKey(npc.type))
            {
                CalamityMod.DRValues.TryGetValue(npc.type, out float newDR);
                DR = newDR;
            }

            // Aquatic Scourge sets kill time in AI, not here.
            if (CalamityMod.bossKillTimes.ContainsKey(npc.type) && !CalamityLists.AquaticScourgeIDs.Contains(npc.type))
            {
                CalamityMod.bossKillTimes.TryGetValue(npc.type, out int revKillTime);
                KillTime = revKillTime;
            }

            // Fixing more red mistakes
            if (npc.type == NPCID.WallofFleshEye)
                npc.netAlways = true;

            sagePoisonDamage = 0;
            if (npc.type == NPCID.Golem && (CalamityWorld.revenge || BossRushEvent.BossRushActive))
                npc.noGravity = true;

            DeclareBossHealthUIVariables(npc);

            BaseVanillaBossHPAdjustments(npc);

            if (BossRushEvent.BossRushActive)
                BossRushStatChanges(npc, Mod);

            BossValueChanges(npc);

            if (CalamityWorld.revenge)
                RevDeathStatChanges(npc, Mod);

            OtherStatChanges(npc);

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

            BoundNPCSafety(Mod, npc);
        }
        #endregion

        #region Boss Health UI Variable Setting
        public void DeclareBossHealthUIVariables(NPC npc)
        {
            if (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)
                SplittingWorm = true;
        }
        #endregion

        #region Base Vanilla Boss HP Adjustments
        private void BaseVanillaBossHPAdjustments(NPC npc)
        {
            switch (npc.type)
            {
                case NPCID.MoonLordCore:
                    npc.lifeMax = 92000;
                    break;

                case NPCID.CultistBoss:
                    npc.lifeMax = 80000;
                    break;

                case NPCID.CultistDragonBody1:
                case NPCID.CultistDragonBody2:
                case NPCID.CultistDragonBody3:
                case NPCID.CultistDragonBody4:
                case NPCID.CultistDragonHead:
                case NPCID.CultistDragonTail:
                    npc.lifeMax = 40000;
                    break;

                case NPCID.DukeFishron:
                    npc.lifeMax = 100000;
                    break;

                case NPCID.Golem:
                    npc.lifeMax = 30000;
                    break;

                case NPCID.GolemHead:
                    npc.lifeMax = 20000;
                    break;

                case NPCID.GolemFistRight:
                case NPCID.GolemFistLeft:
                    npc.lifeMax = 7000;
                    break;

                case NPCID.HallowBoss:
                    npc.lifeMax = 100000;
                    break;

                case NPCID.Plantera:
                    npc.lifeMax = 75000;
                    break;

                case NPCID.Retinazer:
                    npc.lifeMax = 22000;
                    break;

                case NPCID.Spazmatism:
                    npc.lifeMax = 26000;
                    break;

                case NPCID.QueenSlimeBoss:
                    npc.lifeMax = 27000;
                    break;

                case NPCID.WallofFlesh:
                case NPCID.WallofFleshEye:
                    npc.lifeMax = 12800;
                    break;

                case NPCID.Deerclops:
                    npc.lifeMax = 10000;
                    break;

                case NPCID.BrainofCthulhu:
                    npc.lifeMax = 1500;
                    break;

                case NPCID.EaterofWorldsBody:
                case NPCID.EaterofWorldsHead:
                case NPCID.EaterofWorldsTail:
                    npc.lifeMax = 175;
                    break;

                case NPCID.EyeofCthulhu:
                    npc.lifeMax = 3000;
                    break;
            }
        }
        #endregion

        #region Boss Rush Stat Changes
        private void BossRushStatChanges(NPC npc, Mod mod)
        {
            foreach (KeyValuePair<int, int> BossRushHPChange in CalamityLists.BossRushHPChanges)
            {
                if (npc.type == BossRushHPChange.Key)
                {
                    npc.lifeMax = BossRushHPChange.Value;
                    break;
                }
            }
        }
        #endregion

        #region Boss Value Changes
        private void BossValueChanges(NPC npc)
        {
            foreach (KeyValuePair<int, int> BossValue in CalamityLists.BossValues)
            {
                if (npc.type == BossValue.Key)
                {
                    npc.value = BossValue.Value;
                    break;
                }
            }
        }
        #endregion

        #region Revengeance and Death Mode Stat Changes
        private void RevDeathStatChanges(NPC npc, Mod mod)
        {
            if (CalamityLists.DeathModeSplittingWormIDs.Contains(npc.type))
            {
                if (CalamityWorld.death)
                    npc.lifeMax = (int)(npc.lifeMax * 0.15);
            }

            if (npc.type == NPCID.Mothron)
            {
                npc.scale *= 1.25f;
            }
            else if (npc.type == NPCID.MoonLordCore || npc.type == NPCID.MoonLordHand || npc.type == NPCID.MoonLordHead || npc.type == NPCID.MoonLordLeechBlob)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.2);

                if (npc.type == NPCID.MoonLordCore)
                    npc.npcSlots = 36f;
            }
            else if (npc.type == NPCID.CultistBoss || (npc.type >= NPCID.CultistDragonHead && npc.type <= NPCID.CultistDragonTail))
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.2);

                if (npc.type == NPCID.CultistBoss)
                    npc.npcSlots = 20f;
            }
            else if (npc.type == NPCID.DukeFishron)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.4);
                npc.npcSlots = 20f;
            }
            else if (npc.type == NPCID.Sharkron || npc.type == NPCID.Sharkron2)
            {
                npc.lifeMax = (int)(npc.lifeMax * 5.0);
            }
            else if (npc.type == NPCID.Golem || npc.type == NPCID.GolemHead)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.2);

                if (npc.type == NPCID.Golem)
                    npc.npcSlots = 64f;
            }
            else if (npc.type == NPCID.GolemHeadFree)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.7);
                npc.width = 88;
                npc.height = 90;
                npc.dontTakeDamage = false;
            }
            else if (npc.type == NPCID.HallowBoss)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.2);
                npc.npcSlots = 32f;
            }
            else if (npc.type == NPCID.Plantera)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.2);
                npc.npcSlots = 32f;
            }
            else if (CalamityLists.DestroyerIDs.Contains(npc.type))
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.25);
                npc.scale *= Main.zenithWorld ? 2.5f : CalamityWorld.death ? 1.7f : 1.5f;
                npc.npcSlots = 10f;
            }
            else if (npc.type == NPCID.Probe)
            {
                if (CalamityWorld.death)
                    npc.lifeMax = (int)(npc.lifeMax * 2.0);

                npc.scale *= Main.zenithWorld ? 2f : CalamityWorld.death ? 1.3f : 1.2f;
            }
            else if (npc.type == NPCID.SkeletronPrime)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.2);
                npc.npcSlots = 12f;
            }
            else if (npc.type <= NPCID.PrimeLaser && npc.type >= NPCID.PrimeCannon)
            {
                npc.lifeMax = (int)(npc.lifeMax * 0.65);
            }
            else if (npc.type == NPCID.Retinazer || npc.type == NPCID.Spazmatism)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.2);
                npc.npcSlots = 10f;
            }
            else if (npc.type == NPCID.QueenSlimeBoss)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.2);
                npc.npcSlots = 32f;
            }
            else if (npc.type == NPCID.WallofFlesh || npc.type == NPCID.WallofFleshEye)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.2);

                if (npc.type == NPCID.WallofFlesh)
                    npc.npcSlots = 20f;
            }
            else if (npc.type == NPCID.Deerclops)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.2);
                npc.npcSlots = 16f;
            }
            else if (npc.type == NPCID.SkeletronHead)
            {
                if (CalamityWorld.death)
                    npc.lifeMax = (int)(npc.lifeMax * 0.5);
                else
                    npc.lifeMax = (int)(npc.lifeMax * 0.75);

                npc.npcSlots = 12f;
            }
            else if (npc.type == NPCID.SkeletronHand)
            {
                if (CalamityWorld.death)
                    npc.lifeMax = (int)(npc.lifeMax * 0.65);
                else
                    npc.lifeMax = (int)(npc.lifeMax * 0.9);
            }
            else if (npc.type == NPCID.QueenBee)
            {
                npc.defense = 14;
                npc.defDefense = npc.defense;
                npc.lifeMax = (int)(npc.lifeMax * 1.2);
                npc.npcSlots = 14f;
            }
            else if ((npc.type == NPCID.Bee || npc.type == NPCID.BeeSmall) && CalamityPlayer.areThereAnyDamnBosses)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.4);
                npc.scale *= 1.25f;
            }
            else if (npc.type == NPCID.Deerclops)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.2);
                npc.npcSlots = 12f;
            }
            else if (npc.type == NPCID.BrainofCthulhu)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.2);
                npc.npcSlots = 12f;
            }
            else if (npc.type == NPCID.Creeper)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.1);
            }
            else if (CalamityLists.EaterofWorldsIDs.Contains(npc.type))
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.2);

                if (npc.type == NPCID.EaterofWorldsHead)
                    npc.npcSlots = 10f;

                if (CalamityWorld.death)
                    npc.scale *= 1.1f;
            }
            else if (npc.type == NPCID.EyeofCthulhu)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.2);
                npc.npcSlots = 10f;
            }
            else if (npc.type == NPCID.KingSlime)
            {
                if (CalamityWorld.death)
                    npc.scale = Main.getGoodWorld ? 6f : 3f;
                else
                    npc.scale = Main.getGoodWorld ? 3f : 1.25f;

                if (Main.getGoodWorld)
                    npc.lifeMax = (int)(npc.lifeMax * 1.5);
            }
            else if ((npc.type == NPCID.Wraith || npc.type == NPCID.Mimic || npc.type == NPCID.Reaper || npc.type == NPCID.PresentMimic || npc.type == NPCID.SandElemental || npc.type == NPCID.Ghost) && CalamityWorld.LegendaryMode)
            {
                npc.knockBackResist = 0f;
            }
            else if (npc.type == NPCID.Spore)
            {
                npc.dontTakeDamage = true;
            }

            if (CalamityLists.revengeanceLifeStealExceptionList.Contains(npc.type))
            {
                npc.canGhostHeal = false;
            }
        }
        #endregion

        #region Vulnerabilities and Resistances
        private void VulnerabilitiesAndResistances(NPC npc)
        {
            // These enemies are categorized in such a way to make them easy to understand.
            // Do not mess with these categories unless you ask me for permission - Fab.
            switch (npc.type)
            {
                // Regular organic desert enemies.
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

                // Scorpions and sand elemental.
                case NPCID.DesertScorpionWalk:
                case NPCID.DesertScorpionWall:
                case NPCID.SandElemental:
                    VulnerableToCold = true;
                    VulnerableToSickness = false;
                    VulnerableToWater = true;
                    break;

                // Desert slimes.
                case NPCID.SandSlime:
                    VulnerableToCold = true;
                    VulnerableToSickness = false;
                    VulnerableToWater = true;
                    VulnerableToHeat = true;
                    break;

                // Organic undead or other enemies that are covered in slime.
                case NPCID.ArmedZombieSlimed:
                case NPCID.BigSlimedZombie:
                case NPCID.BunnySlimed:
                case NPCID.SlimedZombie:
                case NPCID.SmallSlimedZombie:
                    VulnerableToCold = true;
                    VulnerableToHeat = true;
                    break;

                // Slimes that use heat-related attacks.
                case NPCID.LavaSlime:
                    VulnerableToCold = true;
                    VulnerableToSickness = false;
                    VulnerableToHeat = false;
                    VulnerableToWater = true;
                    break;

                // Regular slimes.
                case NPCID.QueenSlimeBoss:
                case NPCID.QueenSlimeMinionBlue:
                case NPCID.QueenSlimeMinionPink:
                case NPCID.QueenSlimeMinionPurple:
                case NPCID.DungeonSlime:
                case NPCID.BabySlime:
                case NPCID.BlackSlime:
                case NPCID.BlueSlime:
                case NPCID.CorruptSlime:
                case NPCID.GreenSlime:
                case NPCID.IlluminantSlime:
                case NPCID.JungleSlime:
                case NPCID.KingSlime:
                case NPCID.MotherSlime:
                case NPCID.PurpleSlime:
                case NPCID.RainbowSlime:
                case NPCID.RedSlime:
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

                // Skeletons and other armored/bone enemies that use heat-related attacks.
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

                // Spore skeleton.
                case NPCID.SporeSkeleton:
                    VulnerableToHeat = true;
                    VulnerableToSickness = false;
                    VulnerableToWater = true;
                    break;

                // Skeletons and other armored/bone enemies that are dead or undead.
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

                // Metal non-robotic enemies
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

                // Robotic enemies.
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

                // Ghostly or ethereal enemies.
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

                // Organic enemies.
                case NPCID.HallowBoss:
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

                // Demons and shit.
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

                // Fire enemies that are also organic.
                case NPCID.FireImp:
                case NPCID.Hellbat:
                case NPCID.Lavabat:
                    VulnerableToCold = true;
                    VulnerableToHeat = false;
                    VulnerableToSickness = true;
                    VulnerableToWater = true;
                    break;

                // Fire enemies that aren't organic.
                case NPCID.MeteorHead:
                    VulnerableToCold = true;
                    VulnerableToHeat = false;
                    VulnerableToSickness = false;
                    VulnerableToWater = true;
                    break;

                // Lightning bug thing.
                case NPCID.DD2LightningBugT3:
                    VulnerableToElectricity = false;
                    VulnerableToCold = true;
                    VulnerableToHeat = true;
                    VulnerableToSickness = true;
                    break;

                // Betsy.
                case NPCID.DD2Betsy:
                    VulnerableToCold = true;
                    VulnerableToHeat = false;
                    VulnerableToSickness = true;
                    break;

                // Nimbus
                case NPCID.AngryNimbus:
                    VulnerableToCold = true;
                    VulnerableToElectricity = false;
                    VulnerableToWater = false;
                    VulnerableToHeat = false;
                    VulnerableToSickness = false;
                    break;

                // Cold-themed enemies
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

                // Cold-themed enemies that aren't organic.
                case NPCID.IceElemental:
                case NPCID.IceSlime:
                case NPCID.SpikedIceSlime:
                case NPCID.IceGolem:
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

                // Water-themed enemies.
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

                // Fucking bees, hornets and poisonous/toxic stuff.
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
            }
        }
        #endregion

        #region Other Stat Changes
        private void OtherStatChanges(NPC npc)
        {
            EditGlobalCoinDrops(npc);

            switch (npc.type)
            {
                case NPCID.KingSlime:
                case NPCID.EyeofCthulhu:
                case NPCID.BrainofCthulhu:
                case NPCID.QueenBee:
                case NPCID.Corruptor:
                case NPCID.Crawdad:
                case NPCID.Crawdad2:
                case NPCID.ManEater:
                case NPCID.AngryTrapper:
                case NPCID.Snatcher:
                case NPCID.SpikeBall:
                case NPCID.DesertBeast:
                case NPCID.BoneLee:
                case NPCID.Paladin:
                case NPCID.BigMimicCorruption:
                case NPCID.BigMimicCrimson:
                case NPCID.BigMimicHallow:
                case NPCID.DiggerHead:
                case NPCID.SeekerHead:
                case NPCID.DuneSplicerHead:
                case NPCID.SolarCrawltipedeHead:
                case NPCID.Mimic:
                case NPCID.SandShark:
                case NPCID.SandsharkCorrupt:
                case NPCID.SandsharkCrimson:
                case NPCID.SandsharkHallow:
                case NPCID.Butcher:
                case NPCID.DeadlySphere:
                case NPCID.Mothron:
                case NPCID.Reaper:
                case NPCID.Psycho:
                case NPCID.PresentMimic:
                case NPCID.Yeti:
                case NPCID.NebulaBeast:
                case NPCID.SolarCorite:
                case NPCID.StardustWormHead:
                case NPCID.EaterofWorldsHead:
                case NPCID.SkeletronHead:
                case NPCID.SkeletronHand:
                case NPCID.WallofFlesh:
                case NPCID.TheHungry:
                case NPCID.TheHungryII:
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
                case NPCID.AncientCultistSquidhead:
                case NPCID.AncientLight:
                case NPCID.DD2OgreT2:
                case NPCID.DD2OgreT3:
                case NPCID.DD2Betsy:
                case NPCID.PumpkingBlade:
                case NPCID.SantaNK1:
                case NPCID.DukeFishron:
                case NPCID.RockGolem:
                case NPCID.BloodEelHead:
                case NPCID.BloodNautilus:
                case NPCID.GoblinShark:
                case NPCID.ZombieMerman:
                case NPCID.HallowBoss:
                case NPCID.QueenSlimeBoss:
                case NPCID.Deerclops:
                    canBreakPlayerDefense = true;
                    break;

                // Enemies that should have coldDamage set to true
                case NPCID.IceMimic:
                    canBreakPlayerDefense = true;
                    npc.coldDamage = true;
                    break;

                case NPCID.IceBat:
                case NPCID.IceTortoise:
                    npc.coldDamage = true;
                    break;

                case NPCID.PirateGhost:
                    npc.lifeMax = (int)(npc.lifeMax * 0.33);
                    break;

                case NPCID.BloodSquid:
                    npc.lifeMax = (int)(npc.lifeMax * 0.25);
                    break;

                case NPCID.ChatteringTeethBomb:
                    npc.damage = 100;
                    canBreakPlayerDefense = true;
                    break;

                case NPCID.LarvaeAntlion:
                    npc.lifeMax = 15;
                    break;

                // Reduce prehardmode desert enemy stats pre-Desert Scourge
                case NPCID.WalkingAntlion:
                case NPCID.GiantWalkingAntlion:
                    npc.lifeMax = (int)(npc.lifeMax * DesertEnemyStatMultiplier);
                    npc.damage = (int)(npc.damage * DesertEnemyStatMultiplier);
                    npc.defDamage = npc.damage;
                    npc.defense /= 2;
                    npc.defDefense = npc.defense;
                    canBreakPlayerDefense = true;
                    break;

                case NPCID.Antlion:
                case NPCID.FlyingAntlion:
                case NPCID.GiantFlyingAntlion:
                    npc.lifeMax = (int)(npc.lifeMax * DesertEnemyStatMultiplier);
                    npc.damage = (int)(npc.damage * DesertEnemyStatMultiplier);
                    npc.defDamage = npc.damage;
                    npc.defense /= 2;
                    npc.defDefense = npc.defense;
                    break;

                // Reduce Dungeon Guardian HP
                case NPCID.DungeonGuardian:
                    npc.lifeMax = (int)(npc.lifeMax * 0.1);
                    canBreakPlayerDefense = true;
                    break;

                // Reduce Tomb Crawler stats
                case NPCID.TombCrawlerHead:
                    npc.lifeMax = (int)(npc.lifeMax * 0.5);
                    npc.damage = (int)(npc.damage * DesertEnemyStatMultiplier);
                    npc.defDamage = npc.damage;
                    // Tomb Crawler Head has 0 defense so there is no need to reduce it
                    canBreakPlayerDefense = true;
                    break;

                case NPCID.TombCrawlerBody:
                case NPCID.TombCrawlerTail:
                    npc.lifeMax = (int)(npc.lifeMax * 0.5);
                    npc.damage = (int)(npc.damage * DesertEnemyStatMultiplier);
                    npc.defDamage = npc.damage;
                    npc.defense /= 2;
                    npc.defDefense = npc.defense;
                    break;

                // Fix Sharkron hitboxes
                case NPCID.Sharkron:
                case NPCID.Sharkron2:
                    npc.width = npc.height = 36;
                    canBreakPlayerDefense = true;
                    break;

                // Make Core hitbox bigger and reduce HP
                case NPCID.MartianSaucerCore:
                    npc.lifeMax = (int)(npc.lifeMax * 0.6);
                    npc.width *= 2;
                    npc.height *= 2;
                    break;

                // Nerf Green Jellyfish because they spawn in prehardmode
                case NPCID.GreenJellyfish:
                    npc.damage = 40;
                    npc.defDamage = npc.damage;
                    npc.defense = 4;
                    npc.defDefense = npc.defense;
                    break;

                default:
                    break;
            }

            // Reduce mech boss HP and damage depending on the new ore progression changes
            if (CalamityConfig.Instance.EarlyHardmodeProgressionRework && !BossRushEvent.BossRushActive)
            {
                if (!NPC.downedMechBossAny)
                {
                    if (CalamityLists.DestroyerIDs.Contains(npc.type) || npc.type == NPCID.Probe || CalamityLists.SkeletronPrimeIDs.Contains(npc.type) || npc.type == NPCID.Spazmatism || npc.type == NPCID.Retinazer)
                    {
                        npc.lifeMax = (int)(npc.lifeMax * 0.8);
                        npc.damage = (int)(npc.damage * 0.8);
                        npc.defDamage = npc.damage;
                    }
                }
                else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
                {
                    if (CalamityLists.DestroyerIDs.Contains(npc.type) || npc.type == NPCID.Probe || CalamityLists.SkeletronPrimeIDs.Contains(npc.type) || npc.type == NPCID.Spazmatism || npc.type == NPCID.Retinazer)
                    {
                        npc.lifeMax = (int)(npc.lifeMax * 0.9);
                        npc.damage = (int)(npc.damage * 0.9);
                        npc.defDamage = npc.damage;
                    }
                }
            }

            if (Main.hardMode && CalamityLists.HardmodeNPCNerfList.Contains(npc.type))
            {
                npc.damage = (int)(npc.damage * 0.75);
                npc.defDamage = npc.damage;
            }

            if (DownedBossSystem.downedDoG)
            {
                if (CalamityLists.pumpkinMoonBuffList.Contains(npc.type))
                {
                    npc.lifeMax = (int)(npc.lifeMax * 3.5);
                    npc.damage += 30;
                    npc.life = npc.lifeMax;
                    npc.defDamage = npc.damage;
                }
                else if (CalamityLists.frostMoonBuffList.Contains(npc.type))
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2.5);
                    npc.damage += 30;
                    npc.life = npc.lifeMax;
                    npc.defDamage = npc.damage;
                }
                else if (CalamityLists.eclipseBuffList.Contains(npc.type))
                {
                    npc.lifeMax = (int)(npc.lifeMax * 5D);
                    npc.damage += 30;
                    npc.life = npc.lifeMax;
                    npc.defDamage = npc.damage;
                }
            }

            if (NPC.downedMoonlord)
            {
                if (CalamityLists.dungeonEnemyBuffList.Contains(npc.type))
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2.5);
                    npc.damage += 30;
                    npc.life = npc.lifeMax;
                    npc.defDamage = npc.damage;
                }
            }

            if (CalamityWorld.revenge)
            {
                double damageMultiplier = 1D;
                bool containsNPC = false;
                if (CalamityLists.revengeanceEnemyBuffList25Percent.Contains(npc.type))
                {
                    damageMultiplier += 0.25;
                    containsNPC = true;
                }
                else if (CalamityLists.revengeanceEnemyBuffList20Percent.Contains(npc.type))
                {
                    damageMultiplier += 0.2;
                    containsNPC = true;
                }
                else if (CalamityLists.revengeanceEnemyBuffList15Percent.Contains(npc.type))
                {
                    damageMultiplier += 0.15;
                    containsNPC = true;
                }
                else if (CalamityLists.revengeanceEnemyBuffList10Percent.Contains(npc.type))
                {
                    damageMultiplier += 0.1;
                    containsNPC = true;
                }

                if (containsNPC)
                {
                    if (CalamityWorld.death)
                        damageMultiplier += (damageMultiplier - 1D) * 0.6;

                    npc.damage = (int)(npc.damage * damageMultiplier);
                    npc.defDamage = npc.damage;
                }
            }

            if (npc.type < NPCID.Count && NPCStats.EnemyStats.ContactDamageValues.ContainsKey(npc.type))
            {
                npc.GetNPCDamage();
                npc.defDamage = npc.damage;
            }

            if ((npc.boss && npc.type != NPCID.MartianSaucerCore && npc.type < NPCID.Count) || CalamityLists.bossHPScaleList.Contains(npc.type))
            {
                double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
                npc.lifeMax += (int)(npc.lifeMax * HPBoost);
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
            npc.value = (int)(npc.value * NPCValueMultiplier_NormalCalamity);

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
            if (texture is null)
                texture = TextureAssets.Npc[npc.type].Value;
            SpriteEffects effects = npc.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (invertedDirection)
                effects = npc.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 screenOffset = npc.IsABestiaryIconDummy ? Vector2.Zero : Main.screenPosition;
            spriteBatch.Draw(texture,
                             npc.Center - screenOffset + offset,
                             npc.frame,
                             npc.GetAlpha(Color.White),
                             npc.rotation,
                             npc.frame.Size() * 0.5f,
                             npc.scale,
                             effects,
                             0f);
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

            // Set the rotation calculation to a predefined value. The null default is solely so that
            if (rotationCalculation is null)
                rotationCalculation = (nPC, afterimageIndex) => nPC.rotation;

            endingColor.A = 0;

            Color drawColor = npc.GetAlpha(startingColor);
            Texture2D npcTexture = texture ?? TextureAssets.Npc[npc.type].Value;
            Vector2 screenOffset = npc.IsABestiaryIconDummy ? Vector2.Zero : Main.screenPosition;
            int afterimageCounter = 1;
            while (afterimageCounter < NPCID.Sets.TrailCacheLength[npc.type] && CalamityConfig.Instance.Afterimages)
            {
                Color colorToDraw = Color.Lerp(drawColor, endingColor, afterimageCounter / (float)NPCID.Sets.TrailCacheLength[npc.type]);
                colorToDraw *= afterimageCounter / (float)NPCID.Sets.TrailCacheLength[npc.type];
                spriteBatch.Draw(npcTexture,
                                 npc.oldPos[afterimageCounter] + npc.Size / 2f - screenOffset + Vector2.UnitY * npc.gfxOffY,
                                 npc.frame,
                                 colorToDraw,
                                 rotationCalculation.Invoke(npc, afterimageCounter),
                                 npc.frame.Size() * 0.5f,
                                 npc.scale,
                                 spriteEffects,
                                 0f);
                afterimageCounter++;
            }
        }
        #endregion

        #region Scale Expert Multiplayer Stats
        public override void ApplyDifficultyAndPlayerScaling(NPC npc, int numPlayers, float balance, float bossAdjustment)
        {
            // Do absolutely nothing in single player, or in multiplayer with only one player connected.
            if (Main.netMode == NetmodeID.SinglePlayer || numPlayers <= 1)
                return;

            bool countsAsBoss = npc.boss || NPCID.Sets.ShouldBeCountedAsBoss[npc.type];
            bool scalesLikeBoss = countsAsBoss || CalamityLists.bossHPScaleList.Contains(npc.type);
            bool isCalamityNPC = npc.ModNPC != null && npc.ModNPC.Mod == CalamityMod.Instance;

            // All bosses, NPCs that are supposed to scale like bosses, and Calamity NPCs follow these rules.
            if (scalesLikeBoss || isCalamityNPC)
            {
                double scalar;
                switch (numPlayers) // Decrease HP in multiplayer before vanilla scaling
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

                npc.lifeMax = (int)(npc.lifeMax * scalar);
            }
        }
        #endregion

        #region Can Hit Player
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (target.Calamity().prismaticHelmet && !CalamityPlayer.areThereAnyDamnBosses)
            {
                if (npc.lifeMax < 500)
                    return false;
            }

            return true;
        }
        #endregion

        #region Strike NPC
        // Incoming defense to this function is already affected by the vanilla debuffs Ichor (-15) and Betsy's Curse (-40), and cannot be below zero.
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            // Apply armor penetration based on Calamity debuffs. The hit system manages the sequencing.
            // Ozzatron 05JAN2023: fixed doubled armor pen, this time for real
            int defenseReduction = (marked > 0 && DR <= 0f ? MarkedforDeath.DefenseReduction : 0) + (wither > 0 ? WitherDebuff.DefenseReduction : 0) + miscDefenseLoss;
            modifiers.ArmorPenetration += defenseReduction;

            // DR applies after vanilla defense.
            ApplyDR(npc, ref modifiers);

            // Damage reduction on spawn for certain worm bosses.
            bool destroyerResist = CalamityLists.DestroyerIDs.Contains(npc.type) && (CalamityWorld.revenge || BossRushEvent.BossRushActive);
            bool eaterofWorldsResist = CalamityLists.EaterofWorldsIDs.Contains(npc.type) && BossRushEvent.BossRushActive;
            if (destroyerResist || eaterofWorldsResist || CalamityLists.AstrumDeusIDs.Contains(npc.type))
            {
                float resistanceGateValue = (CalamityLists.AstrumDeusIDs.Contains(npc.type) && newAI[0] != 0f) ? 300f : 600f;
                if (newAI[1] < resistanceGateValue || (newAI[2] > 0f && CalamityLists.DestroyerIDs.Contains(npc.type)))
                    modifiers.FinalDamage *= 0.01f;
            }

            // Large Deus worm takes reduced damage to last a long enough time.
            // TODO -- WHY DOES DEUS HAVE THIS UNDOCUMENTED MULTIPLIER HERE??
            // this should be in ModifyHitNPC for deus himself
            if (CalamityLists.AstrumDeusIDs.Contains(npc.type) && newAI[0] == 0f)
                modifiers.FinalDamage *= 0.8f;
        }

        // Directly modifies final damage incoming to an NPC based on their DR (damage reduction) stat added by Calamity.
        // This is entirely separate from vanilla's takenDamageMultiplier.
        private void ApplyDR(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (DR <= 0f && KillTime == 0)
                return;
            
            float finalMultiplier = 1f;

            // If the NPC currently has unbreakable DR, it cannot be reduced by any means.
            // If custom DR is enabled, use that instead of normal DR.
            float effectiveDR = unbreakableDR ? DR : (customDR ? CustomDRMath(npc, DR) : DefaultDRMath(npc, DR));

            // DR floor is 0%. Nothing can have negative DR.
            if (effectiveDR <= 0f)
                effectiveDR = 0f;

            // Add Yellow Candle damage if the NPC isn't supposed to be "near invincible"
            // Armor penetration has already been applied as bonus damage.
            // Yellow Candle provides +5% damage which ignores both DR and defense.
            // This means Yellow Candle is buffing armor penetration and technically not ignoring defense,
            // but it's small enough to let it slide.
            if (npc.HasBuff<CirrusYellowCandleBuff>() && DR < 0.99f && npc.takenDamageMultiplier > 0.05f)
                finalMultiplier += 0.05f;

            // Calculate extra DR based on kill time, similar to the Hush boss from The Binding of Isaac
            // Cirrus being active makes the extra DR cease to function
            bool cirrusBossActive = false;
            if (CalamityGlobalNPC.SCal != -1)
            {
                if (Main.npc[CalamityGlobalNPC.SCal].active)
                    cirrusBossActive = Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas.SupremeCalamitas>().cirrus;
            }

            bool nightProvi = npc.type == NPCType<Providence.Providence>() && !Main.dayTime;
            bool dayEmpress = npc.type == NPCID.HallowBoss && NPC.ShouldEmpressBeEnraged();
            if (KillTime > 0 && AITimer < KillTime && !BossRushEvent.BossRushActive && !cirrusBossActive && (nightProvi || dayEmpress))
            {
                // Set the DR scaling factor
                float DRScalar = 10f;

                // The limit for how much extra DR the boss can have
                float extraDRLimit = (1f - DR) * DRScalar;

                // Ranges from 1 to 0
                float currentHPRatio = npc.life / (float)npc.lifeMax;

                // Ranges from 0 to 1
                float killTimeRatio = AITimer / (float)KillTime;

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

        private float DefaultDRMath(NPC npc, float DR)
        {
            float calcDR = DR;
            if (marked > 0)
                calcDR *= 0.5f;
            if (absorberAffliction > 0)
                calcDR *= 0.8f;
            if (npc.betsysCurse)
                calcDR *= 0.66f;
            if (npc.Calamity().kamiFlu > 0)
                calcDR *= KamiFlu.MultiplicativeDamageReduction;
            if (npc.Calamity().aCrunch > 0)
                calcDR *= ArmorCrunch.MultiplicativeDamageReductionEnemy;
            if (npc.Calamity().crumble > 0)
                calcDR *= Crumbling.MultiplicativeDamageReductionEnemy;


            return calcDR;
        }

        private float CustomDRMath(NPC npc, float DR)
        {
            void FlatEditDR(ref float theDR, bool npcHasDebuff, int buffID)
            {
                if (npcHasDebuff && flatDRReductions.TryGetValue(buffID, out float reduction))
                    theDR -= reduction;
            }
            void MultEditDR(ref float theDR, bool npcHasDebuff, int buffID)
            {
                if (npcHasDebuff && multDRReductions.TryGetValue(buffID, out float multiplier))
                    theDR *= multiplier;
            }

            float calcDR = DR;

            // Apply flat reductions first. All vanilla debuffs check their internal booleans.
            FlatEditDR(ref calcDR, npc.poisoned, BuffID.Poisoned);
            FlatEditDR(ref calcDR, npc.onFire, BuffID.OnFire);
            FlatEditDR(ref calcDR, npc.venom, BuffID.Venom);
            FlatEditDR(ref calcDR, npc.onFrostBurn, BuffID.Frostburn);
            FlatEditDR(ref calcDR, npc.shadowFlame, BuffID.ShadowFlame);
            FlatEditDR(ref calcDR, npc.daybreak, BuffID.Daybreak);
            FlatEditDR(ref calcDR, npc.betsysCurse, BuffID.BetsysCurse);
            FlatEditDR(ref calcDR, npc.onFire2, BuffID.CursedInferno);

            // Modded debuffs are handled modularly and use HasBuff.
            foreach (KeyValuePair<int, float> entry in flatDRReductions)
            {
                int buffID = entry.Key;
                if (buffID >= BuffID.Count && npc.HasBuff(buffID))
                    calcDR -= entry.Value;
            }

            // Apply multiplicative reductions second. All vanilla debuffs check their internal booleans.
            MultEditDR(ref calcDR, npc.poisoned, BuffID.Poisoned);
            MultEditDR(ref calcDR, npc.onFire, BuffID.OnFire);
            MultEditDR(ref calcDR, npc.venom, BuffID.Venom);
            MultEditDR(ref calcDR, npc.onFrostBurn, BuffID.Frostburn);
            MultEditDR(ref calcDR, npc.shadowFlame, BuffID.ShadowFlame);
            MultEditDR(ref calcDR, npc.daybreak, BuffID.Daybreak);
            MultEditDR(ref calcDR, npc.betsysCurse, BuffID.BetsysCurse);
            MultEditDR(ref calcDR, npc.onFire2, BuffID.CursedInferno);

            // Modded debuffs are handled modularly and use HasBuff.
            foreach (KeyValuePair<int, float> entry in multDRReductions)
            {
                int buffID = entry.Key;
                if (buffID >= BuffID.Count && npc.HasBuff(buffID))
                    calcDR *= entry.Value;
            }

            return calcDR;
        }
        #endregion

        #region Boss Head Slot
        public override void BossHeadSlot(NPC npc, ref int index)
        {
            if (CalamityWorld.revenge || BossRushEvent.BossRushActive)
            {
                if (npc.type == NPCID.DukeFishron && (CalamityWorld.death || BossRushEvent.BossRushActive))
                {
                    if (npc.life / (float)npc.lifeMax < 0.4f)
                        index = -1;
                }
            }
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

            if (VulnerabilityHexFireDrawer != null)
                VulnerabilityHexFireDrawer.Update();

            SetPatreonTownNPCName(npc, Mod);

            // Decrement each immune timer if it's greater than 0.
            for (int i = 0; i < maxPlayerImmunities; i++)
            {
                if (dashImmunityTime[i] > 0)
                    dashImmunityTime[i]--;
            }

            if (CalamityPlayer.areThereAnyDamnBosses)
            {
                if (npc.velocity.Length() > maxVelocity)
                    maxVelocity = npc.velocity.Length();
            }

            if (KillTime > 0 || npc.type == NPCType<Draedon>())
            {
                // Apply Boss Effects while any boss NPC is active
                if (Main.netMode != NetmodeID.Server)
                {
                    if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, npc.Center) < BossZenDistance)
                        Main.player[Main.myPlayer].AddBuff(BuffType<BossEffects>(), 2);
                }

                if (npc.type != NPCType<Draedon>())
                {
                    if (AITimer < KillTime)
                        AITimer++;
                }
            }

            if (npc.type == NPCID.TargetDummy || npc.type == NPCType<SuperDummyNPC>())
            {
                npc.dontTakeDamage = CalamityPlayer.areThereAnyDamnBosses;

                if (draedon != -1)
                {
                    if (Main.npc[draedon].active)
                        npc.dontTakeDamage = true;
                }
            }

            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            if (CalamityLists.ZeroContactDamageNPCList.Contains(npc.type) && (npc.type != NPCID.RuneWizard || !Main.zenithWorld))
                npc.damage = npc.defDamage = 0;

            // Don't do damage for 42 frames after spawning in
            if (npc.type == NPCID.Sharkron || npc.type == NPCID.Sharkron2)
                npc.damage = npc.alpha > 0 ? 0 : npc.defDamage;

            if (BossRushEvent.BossRushActive && !npc.friendly && !npc.townNPC && !npc.Calamity().DoesNotDisappearInBossRush)
                BossRushForceDespawnOtherNPCs(npc, Mod);

            if (NPC.LunarApocalypseIsUp)
                PillarEventProgressionEdit(npc);

            // Adult Wyrm Ancient Doom
            if (npc.type == NPCID.AncientDoom)
            {
                if (Main.npc[(int)npc.ai[0]].type == NPCType<PrimordialWyrmHead>())
                    return CultistAI.BuffedAncientDoomAI(npc, Mod);
            }

            // Completely override the shitty AI and replace it
            if (npc.type == NPCID.BloodNautilus)
                return DreadnautilusAI.BuffedDreadnautilusAI(npc, Mod);

            // Disable teleports for hardmode dungeon casters if they get hit
            if (npc.type >= NPCID.RaggedCaster && npc.type <= NPCID.DiabolistWhite && npc.justHit && !CalamityWorld.revenge)
            {
                npc.ai[0] = 1f;
            }

            if (npc.type == NPCID.CultistBoss || npc.type == NPCID.CultistBossClone)
            {
                if (npc.type == NPCID.CultistBossClone)
                {
                    if (Main.npc[(int)npc.ai[3]].active)
                    {
                        // Emit light
                        float lifeRatio = Main.npc[(int)npc.ai[3]].life / (float)Main.npc[(int)npc.ai[3]].lifeMax;
                        float colorTransitionAmt = (float)Math.Pow((double)(1f - lifeRatio), 2D);
                        Color lightColor = Color.Lerp(Color.Cyan, Color.Blue, colorTransitionAmt);
                        Lighting.AddLight(npc.Center, lightColor.R / 255f, lightColor.G / 255f, lightColor.B / 255f);
                    }
                }
                else
                {
                    // Emit light
                    float lifeRatio = npc.life / (float)npc.lifeMax;
                    float colorTransitionAmt = (float)Math.Pow((double)(1f - lifeRatio), 2D);
                    Color lightColor = Color.Lerp(Color.Cyan, Color.Blue, colorTransitionAmt);
                    Lighting.AddLight(npc.Center, lightColor.R / 255f, lightColor.G / 255f, lightColor.B / 255f);

                    // Decrement the hit counter for the shield flicker
                    if (newAI[1] > 0f)
                        newAI[1] -= 1f;

                    // Cultist shield hitbox
                    Vector2 hitboxSize = new Vector2(216f / 1.4142f);
                    if (npc.Size != hitboxSize)
                        npc.Size = hitboxSize;
                }
            }

            if (Main.zenithWorld)
            {
                if (npc.type == NPCID.QueenBee)
                    return QueenBeeAI.BuffedQueenBeeAI(npc, Mod);
            }

            if (CalamityWorld.revenge || BossRushEvent.BossRushActive)
            {
                switch (npc.type)
                {
                    case NPCID.KingSlime:
                        return KingSlimeAI.BuffedKingSlimeAI(npc, Mod);

                    case NPCID.EyeofCthulhu:
                        return EyeOfCthulhuAI.BuffedEyeofCthulhuAI(npc, Mod);

                    case NPCID.EaterofWorldsHead:
                    case NPCID.EaterofWorldsBody:
                    case NPCID.EaterofWorldsTail:
                        return EaterOfWorldsAI.BuffedEaterofWorldsAI(npc, Mod);

                    case NPCID.BrainofCthulhu:
                        return BrainOfCthulhuAI.BuffedBrainofCthulhuAI(npc, Mod);
                    case NPCID.Creeper:
                        return BrainOfCthulhuAI.BuffedCreeperAI(npc, Mod);

                    case NPCID.QueenBee:
                        return QueenBeeAI.BuffedQueenBeeAI(npc, Mod);

                    case NPCID.SkeletronHand:
                        return SkeletronAI.BuffedSkeletronHandAI(npc, Mod);
                    case NPCID.SkeletronHead:
                        return SkeletronAI.BuffedSkeletronAI(npc, Mod);

                    case NPCID.Deerclops:
                        return DeerclopsAI.BuffedDeerclopsAI(npc, Mod);

                    case NPCID.WallofFlesh:
                        return WallOfFleshAI.BuffedWallofFleshAI(npc, Mod);
                    case NPCID.WallofFleshEye:
                        return WallOfFleshAI.BuffedWallofFleshEyeAI(npc, Mod);

                    case NPCID.QueenSlimeBoss:
                        return QueenSlimeAI.BuffedQueenSlimeAI(npc, Mod);

                    case NPCID.TheDestroyer:
                    case NPCID.TheDestroyerBody:
                    case NPCID.TheDestroyerTail:
                        return DestroyerAI.BuffedDestroyerAI(npc, Mod);
                    case NPCID.Probe:
                        return DestroyerAI.BuffedProbeAI(npc, Mod);

                    case NPCID.Retinazer:
                        return TwinsAI.BuffedRetinazerAI(npc, Mod);
                    case NPCID.Spazmatism:
                        return TwinsAI.BuffedSpazmatismAI(npc, Mod);

                    case NPCID.SkeletronPrime:
                        return SkeletronPrimeAI.BuffedSkeletronPrimeAI(npc, Mod);
                    case NPCID.PrimeLaser:
                        return SkeletronPrimeAI.BuffedPrimeLaserAI(npc, Mod);
                    case NPCID.PrimeCannon:
                        return SkeletronPrimeAI.BuffedPrimeCannonAI(npc, Mod);
                    case NPCID.PrimeVice:
                        return SkeletronPrimeAI.BuffedPrimeViceAI(npc, Mod);
                    case NPCID.PrimeSaw:
                        return SkeletronPrimeAI.BuffedPrimeSawAI(npc, Mod);

                    case NPCID.Plantera:
                        return PlanteraAI.BuffedPlanteraAI(npc, Mod);
                    case NPCID.PlanterasHook:
                        return PlanteraAI.BuffedPlanterasHookAI(npc, Mod);
                    case NPCID.PlanterasTentacle:
                        return PlanteraAI.BuffedPlanterasTentacleAI(npc, Mod);

                    case NPCID.HallowBoss:
                        return EmpressofLightAI.BuffedEmpressofLightAI(npc, Mod);

                    case NPCID.Golem:
                        return GolemAI.BuffedGolemAI(npc, Mod);
                    case NPCID.GolemFistLeft:
                    case NPCID.GolemFistRight:
                        return GolemAI.BuffedGolemFistAI(npc, Mod);
                    case NPCID.GolemHead:
                        return GolemAI.BuffedGolemHeadAI(npc, Mod);
                    case NPCID.GolemHeadFree:
                        return GolemAI.BuffedGolemHeadFreeAI(npc, Mod);

                    case NPCID.DukeFishron:
                        return DukeFishronAI.BuffedDukeFishronAI(npc, Mod);

                    case NPCID.Pumpking:
                        if (DownedBossSystem.downedDoG)
                        {
                            return CalamityGlobalAI.BuffedPumpkingAI(npc);
                        }

                        break;

                    case NPCID.PumpkingBlade:
                        if (DownedBossSystem.downedDoG)
                        {
                            return CalamityGlobalAI.BuffedPumpkingBladeAI(npc);
                        }

                        break;

                    case NPCID.IceQueen:
                        if (DownedBossSystem.downedDoG)
                        {
                            return CalamityGlobalAI.BuffedIceQueenAI(npc);
                        }

                        break;

                    case NPCID.Mothron:
                        if (DownedBossSystem.downedDoG)
                        {
                            return CalamityGlobalAI.BuffedMothronAI(npc);
                        }

                        break;

                    case NPCID.CultistBoss:
                    case NPCID.CultistBossClone:
                        return CultistAI.BuffedCultistAI(npc, Mod);
                    case NPCID.AncientLight:
                        return CultistAI.BuffedAncientLightAI(npc, Mod);
                    case NPCID.AncientDoom:
                        return CultistAI.BuffedAncientDoomAI(npc, Mod);

                    case NPCID.MoonLordCore:
                    case NPCID.MoonLordHand:
                    case NPCID.MoonLordHead:
                    case NPCID.MoonLordFreeEye:
                    case NPCID.MoonLordLeechBlob:
                        return MoonLordAI.BuffedMoonLordAI(npc, Mod);

                    default:
                        break;
                }
            }
            else if (npc.type == NPCID.Retinazer && !Main.getGoodWorld)
                return TwinsAI.TrueMeleeRetinazerPhase2AI(npc);

            if (CalamityWorld.revenge)
            {
                switch (npc.aiStyle)
                {
                    case NPCAIStyleID.Slime:
                        if (npc.type == NPCType<BloomSlime>() || npc.type == NPCType<InfernalCongealment>() ||
                            npc.type == NPCType<CrimulanBlightSlime>() || npc.type == NPCType<CryoSlime>() ||
                            npc.type == NPCType<EbonianBlightSlime>() || npc.type == NPCType<PerennialSlime>() ||
                            npc.type == NPCType<IrradiatedSlime>() || npc.type == NPCType<AstralSlime>())
                        {
                            return SlimeAI.BuffedSlimeAI(npc, Mod);
                        }
                        else
                        {
                            switch (npc.type)
                            {
                                case NPCID.BlueSlime:
                                case NPCID.MotherSlime:
                                case NPCID.LavaSlime:
                                case NPCID.DungeonSlime:
                                case NPCID.CorruptSlime:
                                case NPCID.IlluminantSlime:
                                case NPCID.ToxicSludge:
                                case NPCID.IceSlime:
                                case NPCID.Crimslime:
                                case NPCID.SpikedIceSlime:
                                case NPCID.SpikedJungleSlime:
                                case NPCID.UmbrellaSlime:
                                case NPCID.RainbowSlime:
                                case NPCID.SlimeMasked:
                                case NPCID.HoppinJack:
                                case NPCID.SlimeRibbonWhite:
                                case NPCID.SlimeRibbonYellow:
                                case NPCID.SlimeRibbonGreen:
                                case NPCID.SlimeRibbonRed:
                                case NPCID.SlimeSpiked:
                                case NPCID.SandSlime:
                                case NPCID.GoldenSlime:
                                    return SlimeAI.BuffedSlimeAI(npc, Mod);
                            }
                        }
                        break;

                    case NPCAIStyleID.DemonEye:
                        if (npc.type == NPCType<CalamityEye>())
                        {
                            return DemonEyeAI.BuffedDemonEyeAI(npc, Mod);
                        }
                        else
                        {
                            switch (npc.type)
                            {
                                case NPCID.DemonEye:
                                case NPCID.TheHungryII:
                                case NPCID.WanderingEye:
                                case NPCID.PigronCorruption:
                                case NPCID.PigronHallow:
                                case NPCID.PigronCrimson:
                                case NPCID.CataractEye:
                                case NPCID.SleepyEye:
                                case NPCID.DialatedEye:
                                case NPCID.GreenEye:
                                case NPCID.PurpleEye:
                                case NPCID.DemonEyeOwl:
                                case NPCID.DemonEyeSpaceship:
                                    return DemonEyeAI.BuffedDemonEyeAI(npc, Mod);
                            }
                        }
                        break;

                    case NPCAIStyleID.Fighter:
                        if (npc.type == NPCType<Stormlion>() ||
                            npc.type == NPCType<AstralachneaGround>() || npc.type == NPCType<RenegadeWarlock>())
                        {
                            return CalamityGlobalAI.BuffedFighterAI(npc, Mod);
                        }
                        else
                        {
                            switch (npc.type)
                            {
                                case NPCID.Zombie:
                                case NPCID.ArmedZombie:
                                case NPCID.ArmedZombieEskimo:
                                case NPCID.ArmedZombiePincussion:
                                case NPCID.ArmedZombieSlimed:
                                case NPCID.ArmedZombieSwamp:
                                case NPCID.ArmedZombieTwiggy:
                                case NPCID.ArmedZombieCenx:
                                case NPCID.Skeleton:
                                case NPCID.SporeSkeleton:
                                case NPCID.AngryBones:
                                case NPCID.UndeadMiner:
                                case NPCID.CorruptBunny:
                                case NPCID.DoctorBones:
                                case NPCID.TheGroom:
                                case NPCID.Crab:
                                case NPCID.GoblinScout:
                                case NPCID.ArmoredSkeleton:
                                case NPCID.Mummy:
                                case NPCID.DarkMummy:
                                case NPCID.LightMummy:
                                case NPCID.Werewolf:
                                case NPCID.Clown:
                                case NPCID.SkeletonArcher:
                                case NPCID.ChaosElemental:
                                case NPCID.BaldZombie:
                                case NPCID.PossessedArmor:
                                case NPCID.ZombieEskimo:
                                case NPCID.BlackRecluse:
                                case NPCID.WallCreeper:
                                case NPCID.UndeadViking:
                                case NPCID.CorruptPenguin:
                                case NPCID.FaceMonster:
                                case NPCID.SnowFlinx:
                                case NPCID.PincushionZombie:
                                case NPCID.SlimedZombie:
                                case NPCID.SwampZombie:
                                case NPCID.TwiggyZombie:
                                case NPCID.Nymph:
                                case NPCID.ArmoredViking:
                                case NPCID.Lihzahrd:
                                case NPCID.LihzahrdCrawler:
                                case NPCID.FemaleZombie:
                                case NPCID.HeadacheSkeleton:
                                case NPCID.MisassembledSkeleton:
                                case NPCID.PantlessSkeleton:
                                case NPCID.IcyMerman:
                                case NPCID.PirateDeckhand:
                                case NPCID.PirateCorsair:
                                case NPCID.PirateDeadeye:
                                case NPCID.PirateCrossbower:
                                case NPCID.PirateCaptain:
                                case NPCID.CochinealBeetle:
                                case NPCID.CyanBeetle:
                                case NPCID.LacBeetle:
                                case NPCID.SeaSnail:
                                case NPCID.ZombieRaincoat:
                                case NPCID.JungleCreeper:
                                case NPCID.BloodCrawler:
                                case NPCID.IceGolem:
                                case NPCID.Eyezor:
                                case NPCID.ZombieMushroom:
                                case NPCID.ZombieMushroomHat:
                                case NPCID.AnomuraFungus:
                                case NPCID.MushiLadybug:
                                case NPCID.RustyArmoredBonesAxe:
                                case NPCID.RustyArmoredBonesFlail:
                                case NPCID.RustyArmoredBonesSword:
                                case NPCID.RustyArmoredBonesSwordNoArmor:
                                case NPCID.BlueArmoredBones:
                                case NPCID.BlueArmoredBonesMace:
                                case NPCID.BlueArmoredBonesNoPants:
                                case NPCID.BlueArmoredBonesSword:
                                case NPCID.HellArmoredBones:
                                case NPCID.HellArmoredBonesSpikeShield:
                                case NPCID.HellArmoredBonesMace:
                                case NPCID.HellArmoredBonesSword:
                                case NPCID.BoneLee:
                                case NPCID.Paladin:
                                case NPCID.SkeletonSniper:
                                case NPCID.TacticalSkeleton:
                                case NPCID.SkeletonCommando:
                                case NPCID.AngryBonesBig:
                                case NPCID.AngryBonesBigMuscle:
                                case NPCID.AngryBonesBigHelmet:
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
                                case NPCID.ZombieDoctor:
                                case NPCID.ZombieSuperman:
                                case NPCID.ZombiePixie:
                                case NPCID.SkeletonTopHat:
                                case NPCID.SkeletonAstonaut:
                                case NPCID.SkeletonAlien:
                                case NPCID.Splinterling:
                                case NPCID.ZombieXmas:
                                case NPCID.ZombieSweater:
                                case NPCID.ZombieElf:
                                case NPCID.ZombieElfBeard:
                                case NPCID.ZombieElfGirl:
                                case NPCID.GingerbreadMan:
                                case NPCID.Yeti:
                                case NPCID.Nutcracker:
                                case NPCID.NutcrackerSpinning:
                                case NPCID.ElfArcher:
                                case NPCID.Krampus:
                                case NPCID.CultistArcherBlue:
                                case NPCID.CultistArcherWhite:
                                case NPCID.BrainScrambler:
                                case NPCID.RayGunner:
                                case NPCID.MartianOfficer:
                                case NPCID.GrayGrunt:
                                case NPCID.MartianEngineer:
                                case NPCID.GigaZapper:
                                case NPCID.Scutlix:
                                case NPCID.BoneThrowingSkeleton:
                                case NPCID.BoneThrowingSkeleton2:
                                case NPCID.BoneThrowingSkeleton3:
                                case NPCID.BoneThrowingSkeleton4:
                                case NPCID.CrimsonBunny:
                                case NPCID.CrimsonPenguin:
                                case NPCID.Medusa:
                                case NPCID.GreekSkeleton:
                                case NPCID.GraniteGolem:
                                case NPCID.BloodZombie:
                                case NPCID.Crawdad:
                                case NPCID.Crawdad2:
                                case NPCID.Salamander:
                                case NPCID.Salamander2:
                                case NPCID.Salamander3:
                                case NPCID.Salamander4:
                                case NPCID.Salamander5:
                                case NPCID.Salamander6:
                                case NPCID.Salamander7:
                                case NPCID.Salamander8:
                                case NPCID.Salamander9:
                                case NPCID.GiantWalkingAntlion:
                                case NPCID.WalkingAntlion:
                                case NPCID.LarvaeAntlion:
                                case NPCID.DesertGhoul:
                                case NPCID.DesertGhoulCorruption:
                                case NPCID.DesertGhoulCrimson:
                                case NPCID.DesertGhoulHallow:
                                case NPCID.DesertLamiaLight:
                                case NPCID.DesertLamiaDark:
                                case NPCID.DesertScorpionWalk:
                                case NPCID.DesertBeast:
                                case NPCID.StardustSoldier:
                                case NPCID.StardustSpiderBig:
                                case NPCID.NebulaSoldier:
                                case NPCID.VortexRifleman:
                                case NPCID.VortexSoldier:
                                case NPCID.VortexLarva:
                                case NPCID.VortexHornet:
                                case NPCID.VortexHornetQueen:
                                case NPCID.SolarDrakomire:
                                case NPCID.SolarSpearman:
                                case NPCID.SolarSolenian:
                                case NPCID.Frankenstein:
                                case NPCID.SwampThing:
                                case NPCID.Vampire:
                                case NPCID.Butcher:
                                case NPCID.CreatureFromTheDeep:
                                case NPCID.Fritz:
                                case NPCID.Nailhead:
                                case NPCID.Psycho:
                                case NPCID.ThePossessed:
                                case NPCID.DrManFly:
                                case NPCID.GoblinPeon:
                                case NPCID.GoblinThief:
                                case NPCID.GoblinWarrior:
                                case NPCID.GoblinArcher:
                                case NPCID.GoblinSummoner:
                                case NPCID.MartianWalker:
                                case NPCID.DemonTaxCollector:
                                case NPCID.TheBride:
                                    return CalamityGlobalAI.BuffedFighterAI(npc, Mod);
                            }
                        }
                        break;

                    case NPCAIStyleID.Flying:
                        switch (npc.type)
                        {
                            case NPCID.ServantofCthulhu:
                            case NPCID.EaterofSouls:
                            case NPCID.MeteorHead:
                            case NPCID.Corruptor:
                            case NPCID.Crimera:
                            case NPCID.Moth:
                            case NPCID.Parrot:
                            case NPCID.Bee:
                            case NPCID.BeeSmall:
                            case NPCID.BloodSquid:
                            case NPCID.Hornet:
                            case NPCID.HornetFatty:
                            case NPCID.HornetHoney:
                            case NPCID.HornetLeafy:
                            case NPCID.HornetSpikey:
                            case NPCID.HornetStingy:
                            case NPCID.MossHornet:
                                return CalamityGlobalAI.BuffedFlyingAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.Worm:
                        switch (npc.type)
                        {
                            case NPCID.DevourerHead:
                            case NPCID.DevourerBody:
                            case NPCID.DevourerTail:
                            case NPCID.GiantWormHead:
                            case NPCID.GiantWormBody:
                            case NPCID.GiantWormTail:
                            case NPCID.BoneSerpentHead:
                            case NPCID.BoneSerpentBody:
                            case NPCID.BoneSerpentTail:
                            case NPCID.WyvernHead:
                            case NPCID.WyvernLegs:
                            case NPCID.WyvernBody:
                            case NPCID.WyvernBody2:
                            case NPCID.WyvernBody3:
                            case NPCID.WyvernTail:
                            case NPCID.LeechHead:
                            case NPCID.LeechBody:
                            case NPCID.LeechTail:
                            case NPCID.TombCrawlerHead:
                            case NPCID.TombCrawlerBody:
                            case NPCID.TombCrawlerTail:
                            case NPCID.StardustWormHead:
                            case NPCID.SolarCrawltipedeHead:
                            case NPCID.SolarCrawltipedeBody:
                            case NPCID.SolarCrawltipedeTail:
                            case NPCID.BloodEelHead:
                            case NPCID.BloodEelBody:
                            case NPCID.BloodEelTail:
                                return CalamityGlobalAI.BuffedWormAI(npc, Mod);

                            // Death Mode splitting worms.
                            case NPCID.DiggerHead:
                            case NPCID.DiggerBody:
                            case NPCID.DiggerTail:
                            case NPCID.SeekerHead:
                            case NPCID.SeekerBody:
                            case NPCID.SeekerTail:
                            case NPCID.DuneSplicerHead:
                            case NPCID.DuneSplicerBody:
                            case NPCID.DuneSplicerTail:
                                if (CalamityWorld.death)
                                    return CalamityGlobalAI.BuffedWormAI(npc, Mod);
                                else
                                    return true;
                        }
                        break;

                    case NPCAIStyleID.Caster:
                        switch (npc.type)
                        {
                            case NPCID.FireImp:
                            case NPCID.DarkCaster:
                            case NPCID.Tim:
                            case NPCID.RuneWizard:
                            case NPCID.RaggedCaster:
                            case NPCID.RaggedCasterOpenCoat:
                            case NPCID.Necromancer:
                            case NPCID.NecromancerArmored:
                            case NPCID.DiabolistRed:
                            case NPCID.DiabolistWhite:
                            case NPCID.DesertDjinn:
                            case NPCID.GoblinSorcerer:
                                return CalamityGlobalAI.BuffedCasterAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.ManEater:
                        switch (npc.type)
                        {
                            case NPCID.ManEater:
                            case NPCID.Snatcher:
                            case NPCID.Clinger:
                            case NPCID.AngryTrapper:
                            case NPCID.FungiBulb:
                            case NPCID.GiantFungiBulb:
                                return CalamityGlobalAI.BuffedPlantAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.Bat:
                        if (npc.type == NPCType<StellarCulex>() || npc.type == NPCType<Melter>() || npc.type == NPCType<AeroSlime>())
                        {
                            return CalamityGlobalAI.BuffedBatAI(npc, Mod);
                        }
                        else
                        {
                            switch (npc.type)
                            {
                                case NPCID.Harpy:
                                case NPCID.CaveBat:
                                case NPCID.JungleBat:
                                case NPCID.Hellbat:
                                case NPCID.Demon:
                                case NPCID.VoodooDemon:
                                case NPCID.GiantBat:
                                case NPCID.Slimer:
                                case NPCID.IlluminantBat:
                                case NPCID.IceBat:
                                case NPCID.Lavabat:
                                case NPCID.GiantFlyingFox:
                                case NPCID.RedDevil:
                                case NPCID.FlyingSnake:
                                case NPCID.VampireBat:
                                case NPCID.SporeBat:
                                    return CalamityGlobalAI.BuffedBatAI(npc, Mod);
                            }
                        }
                        break;

                    case NPCAIStyleID.Piranha:
                        switch (npc.type)
                        {
                            case NPCID.CorruptGoldfish:
                            case NPCID.Piranha:
                            case NPCID.Shark:
                            case NPCID.AnglerFish:
                            case NPCID.Arapaima:
                            case NPCID.BloodFeeder:
                            case NPCID.CrimsonGoldfish:
                                return CalamityGlobalAI.BuffedSwimmingAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.Jellyfish:
                        switch (npc.type)
                        {
                            case NPCID.BlueJellyfish:
                            case NPCID.PinkJellyfish:
                            case NPCID.GreenJellyfish:
                            case NPCID.Squid:
                            case NPCID.BloodJelly:
                            case NPCID.FungoFish:
                                return CalamityGlobalAI.BuffedJellyfishAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.Antlion:
                        switch (npc.type)
                        {
                            case NPCID.Antlion:
                                return CalamityGlobalAI.BuffedAntlionAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.SpikeBall:
                        switch (npc.type)
                        {
                            case NPCID.SpikeBall:
                                return CalamityGlobalAI.BuffedSpikeBallAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.BlazingWheel:
                        switch (npc.type)
                        {
                            case NPCID.BlazingWheel:
                                return CalamityGlobalAI.BuffedBlazingWheelAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.HoveringFighter:
                        switch (npc.type)
                        {
                            case NPCID.Pixie:
                            case NPCID.Wraith:
                            case NPCID.Gastropod:
                            case NPCID.IceElemental:
                            case NPCID.FloatyGross:
                            case NPCID.IchorSticker:
                            case NPCID.Ghost:
                            case NPCID.Poltergeist:
                            case NPCID.Drippler:
                            case NPCID.Reaper:
                                return CalamityGlobalAI.BuffedHoveringAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.EnchantedSword:
                        switch (npc.type)
                        {
                            case NPCID.CursedHammer:
                            case NPCID.EnchantedSword:
                            case NPCID.CrimsonAxe:
                                return CalamityGlobalAI.BuffedFlyingWeaponAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.Mimic:
                        switch (npc.type)
                        {
                            case NPCID.Mimic:
                            case NPCID.PresentMimic:
                            case NPCID.IceMimic:
                                return CalamityGlobalAI.BuffedMimicAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.Unicorn:
                        if (npc.type == NPCType<Rotdog>())
                        {
                            return CalamityGlobalAI.BuffedUnicornAI(npc, Mod);
                        }
                        else
                        {
                            switch (npc.type)
                            {
                                case NPCID.Unicorn:
                                case NPCID.Wolf:
                                case NPCID.HeadlessHorseman:
                                case NPCID.Hellhound:
                                case NPCID.StardustSpiderSmall:
                                case NPCID.NebulaBeast:
                                case NPCID.Tumbleweed:
                                    return CalamityGlobalAI.BuffedUnicornAI(npc, Mod);
                            }
                        }
                        break;

                    case NPCAIStyleID.GiantTortoise:
                        if (npc.type == NPCType<Plagueshell>())
                        {
                            return CalamityGlobalAI.BuffedTortoiseAI(npc, Mod);
                        }
                        else
                        {
                            switch (npc.type)
                            {
                                case NPCID.GiantTortoise:
                                case NPCID.IceTortoise:
                                case NPCID.GiantShelly:
                                case NPCID.GiantShelly2:
                                case NPCID.SolarSroller:
                                    return CalamityGlobalAI.BuffedTortoiseAI(npc, Mod);
                            }
                        }
                        break;

                    case NPCAIStyleID.Spider:
                        switch (npc.type)
                        {
                            case NPCID.BlackRecluseWall:
                            case NPCID.WallCreeperWall:
                            case NPCID.JungleCreeperWall:
                            case NPCID.BloodCrawlerWall:
                            case NPCID.DesertScorpionWall:
                                return CalamityGlobalAI.BuffedSpiderAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.Herpling:
                        if (npc.type == NPCType<Aries>())
                        {
                            return CalamityGlobalAI.BuffedHerplingAI(npc, Mod);
                        }
                        else
                        {
                            switch (npc.type)
                            {
                                case NPCID.Herpling:
                                case NPCID.Derpling:
                                    return CalamityGlobalAI.BuffedHerplingAI(npc, Mod);
                            }
                        }
                        break;

                    case NPCAIStyleID.FlyingFish:
                        switch (npc.type)
                        {
                            case NPCID.FlyingFish:
                            case NPCID.GiantFlyingAntlion:
                            case NPCID.FlyingAntlion:
                            case NPCID.EyeballFlyingFish:
                                return CalamityGlobalAI.BuffedFlyingFishAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.AngryNimbus:
                        switch (npc.type)
                        {
                            case NPCID.AngryNimbus:
                                return CalamityGlobalAI.BuffedAngryNimbusAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.TeslaTurret:
                        switch (npc.type)
                        {
                            case NPCID.MartianTurret:
                                return CalamityGlobalAI.BuffedTeslaTurretAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.Corite:
                        switch (npc.type)
                        {
                            case NPCID.MartianDrone:
                            case NPCID.SolarCorite:
                                return CalamityGlobalAI.BuffedCoriteAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.MartianProbe:
                        switch (npc.type)
                        {
                            case NPCID.MartianProbe:
                                return CalamityGlobalAI.BuffedMartianProbeAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.StarCell:
                        switch (npc.type)
                        {
                            case NPCID.StardustCellBig:
                            case NPCID.NebulaHeadcrab:
                            case NPCID.DeadlySphere:
                                return CalamityGlobalAI.BuffedStarCellAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.AncientVision:
                        switch (npc.type)
                        {
                            case NPCID.ShadowFlameApparition:
                            case NPCID.AncientCultistSquidhead:
                                return CalamityGlobalAI.BuffedAncientVisionAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.BiomeMimic:
                        switch (npc.type)
                        {
                            case NPCID.BigMimicCorruption:
                            case NPCID.BigMimicCrimson:
                            case NPCID.BigMimicHallow:
                            case NPCID.BigMimicJungle:
                                return CalamityGlobalAI.BuffedBigMimicAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.MothronEgg:
                        switch (npc.type)
                        {
                            case NPCID.MothronEgg:
                                return CalamityGlobalAI.BuffedMothronEggAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.GraniteElemental:
                        switch (npc.type)
                        {
                            case NPCID.GraniteFlyer:
                                return CalamityGlobalAI.BuffedGraniteElementalAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.SmallStarCell:
                        switch (npc.type)
                        {
                            case NPCID.StardustCellSmall:
                                return CalamityGlobalAI.BuffedSmallStarCellAI(npc, Mod);
                        }
                        break;

                    case NPCAIStyleID.FlowInvader:
                        switch (npc.type)
                        {
                            case NPCID.StardustJellyfishBig:
                                return CalamityGlobalAI.BuffedFlowInvaderAI(npc, Mod);
                        }
                        break;

                    default:
                        break;
                }
            }

            if (npc.type == NPCID.FungiSpore || npc.type == NPCID.Spore)
                return CalamityGlobalAI.BuffedSporeAI(npc, Mod);

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

                    for (int k = 0; k < Main.maxNPCs; k++)
                    {
                        if (k != npc.whoAmI && Main.npc[k].active && Main.npc[k].aiStyle == NPCAIStyleID.Fairy && Math.Abs(npc.position.X - Main.npc[k].position.X) + Math.Abs(npc.position.Y - Main.npc[k].position.Y) < (float)npc.width * 1.5f)
                        {
                            if (npc.position.Y < Main.npc[k].position.Y)
                                npc.velocity.Y -= 0.05f;
                            else
                                npc.velocity.Y += 0.05f;
                        }
                    }

                    npc.direction = (npc.velocity.X >= 0f) ? 1 : (-1);
                    npc.spriteDirection = -npc.direction;

                    Color dustLerpColor1 = Color.HotPink;
                    Color dustLerpColor2 = Color.LightPink;
                    int dustPosition = 4;
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
                        Dust dust = Dust.NewDustDirect(npc.Center - new Vector2(dustPosition) * 0.5f, dustPosition + 4, dustPosition + 4, 278, 0f, 0f, 200, Color.Lerp(dustLerpColor1, dustLerpColor2, Main.rand.NextFloat()), 0.65f);
                        dust.velocity *= 0f;
                        dust.velocity += npc.velocity * 0.3f;
                        dust.noGravity = true;
                        dust.noLight = true;
                        npc.position -= npc.netOffset;
                    }

                    Lighting.AddLight(npc.Center, dustLerpColor1.ToVector3() * 0.7f);
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Player localPlayer = Main.LocalPlayer;
                        if (!localPlayer.dead && localPlayer.HitboxForBestiaryNearbyCheck.Intersects(npc.Hitbox))
                            AchievementsHelper.HandleSpecialEvent(localPlayer, 22);
                    }

                    return false;
                }
            }

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

        #region Pillar Event Progression Edit
        private void PillarEventProgressionEdit(NPC npc)
        {
            // Make pillars a bit more fun by forcing more difficult enemies based on progression.
            int solarTowerShieldStrength = (int)Math.Ceiling(NPC.ShieldStrengthTowerSolar / 25D);
            switch (solarTowerShieldStrength)
            {
                case 4:
                    // Possible spawns: Drakanian, Drakomire, Drakomire Rider, Sroller
                    switch (npc.type)
                    {
                        case NPCID.SolarCrawltipedeHead:
                        case NPCID.SolarCrawltipedeBody:
                        case NPCID.SolarCrawltipedeTail:
                        case NPCID.SolarSolenian:
                        case NPCID.SolarCorite:
                            npc.active = false;
                            npc.netUpdate = true;
                            break;
                        default:
                            break;
                    }
                    break;
                case 3:
                    // Possible spawns: Drakanian, Drakomire Rider, Sroller
                    switch (npc.type)
                    {
                        case NPCID.SolarCrawltipedeHead:
                        case NPCID.SolarCrawltipedeBody:
                        case NPCID.SolarCrawltipedeTail:
                        case NPCID.SolarDrakomire:
                        case NPCID.SolarSolenian:
                        case NPCID.SolarCorite:
                            npc.active = false;
                            npc.netUpdate = true;
                            break;
                        default:
                            break;
                    }
                    break;
                case 2:
                    // Possible spawns: Drakanian, Selenian, Sroller
                    switch (npc.type)
                    {
                        case NPCID.SolarDrakomire:
                        case NPCID.SolarCrawltipedeHead:
                        case NPCID.SolarCrawltipedeBody:
                        case NPCID.SolarCrawltipedeTail:
                        case NPCID.SolarCorite:
                        case NPCID.SolarDrakomireRider:
                            npc.active = false;
                            npc.netUpdate = true;
                            break;
                        default:
                            break;
                    }
                    break;
                case 1:
                    // Possible spawns: Corite, Selenian, Sroller, Crawltipede
                    switch (npc.type)
                    {
                        case NPCID.SolarDrakomire:
                        case NPCID.SolarSpearman:
                        case NPCID.SolarDrakomireRider:
                            npc.active = false;
                            npc.netUpdate = true;
                            break;
                        default:
                            break;
                    }
                    break;
                case 0:
                    // Possible spawns: Corite, Crawltipede, Selenian
                    switch (npc.type)
                    {
                        case NPCID.SolarDrakomire:
                        case NPCID.SolarSpearman:
                        case NPCID.SolarDrakomireRider:
                        case NPCID.SolarSroller:
                            npc.active = false;
                            npc.netUpdate = true;
                            break;
                        default:
                            break;
                    }
                    break;
            }

            int vortexTowerShieldStrength = (int)Math.Ceiling(NPC.ShieldStrengthTowerVortex / 25D);
            switch (vortexTowerShieldStrength)
            {
                case 4:
                    // Possible spawns: Alien Larva, Alien Hornet, Alien Queen
                    switch (npc.type)
                    {
                        case NPCID.VortexSoldier:
                        case NPCID.VortexRifleman:
                            npc.active = false;
                            npc.netUpdate = true;
                            break;
                        default:
                            break;
                    }
                    break;
                case 3:
                    // Possible spawns: Alien Larva, Alien Hornet, Alien Queen, Vortexian
                    if (npc.type == NPCID.VortexRifleman)
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }
                    break;
                case 2:
                    // Possible spawns: Alien Larva, Alien Hornet, Alien Queen, Storm Diver
                    if (npc.type == NPCID.VortexSoldier)
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }
                    break;
                case 1:
                case 0:
                    // Possible spawns: Alien Larva, Alien Hornet, Alien Queen, Vortexian, Storm Diver
                    break;
            }

            int nebulaTowerShieldStrength = (int)Math.Ceiling(NPC.ShieldStrengthTowerNebula / 25D);
            switch (nebulaTowerShieldStrength)
            {
                case 4:
                    // Possible spawns: Brain Suckler
                    switch (npc.type)
                    {
                        case NPCID.NebulaBeast:
                        case NPCID.NebulaBrain:
                        case NPCID.NebulaSoldier:
                            npc.active = false;
                            npc.netUpdate = true;
                            break;
                        default:
                            break;
                    }
                    break;
                case 3:
                    // Possible spawns: Brain Suckler, Predictor
                    switch (npc.type)
                    {
                        case NPCID.NebulaBeast:
                        case NPCID.NebulaBrain:
                            npc.active = false;
                            npc.netUpdate = true;
                            break;
                        default:
                            break;
                    }
                    break;
                case 2:
                    // Possible spawns: Brain Suckler, Predictor, Evolution Beast
                    if (npc.type == NPCID.NebulaBrain)
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }
                    break;
                case 1:
                case 0:
                    // Possible spawns: Predictor, Evolution Beast, Nebula Floater
                    if (npc.type == NPCID.NebulaHeadcrab)
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                    }
                    break;
            }

            int stardustTowerShieldStrength = (int)Math.Ceiling(NPC.ShieldStrengthTowerStardust / 25D);
            switch (stardustTowerShieldStrength)
            {
                case 4:
                    // Possible spawns: Milkyway Weaver, Star Cell
                    switch (npc.type)
                    {
                        case NPCID.StardustSpiderBig:
                        case NPCID.StardustSoldier:
                        case NPCID.StardustJellyfishBig:
                            npc.active = false;
                            npc.netUpdate = true;
                            break;
                        default:
                            break;
                    }
                    break;
                case 3:
                    // Possible spawns: Milkyway Weaver, Stargazer, Twinkle Popper
                    switch (npc.type)
                    {
                        case NPCID.StardustCellBig:
                        case NPCID.StardustJellyfishBig:
                            npc.active = false;
                            npc.netUpdate = true;
                            break;
                        default:
                            break;
                    }
                    break;
                case 2:
                    // Possible spawns: Stargazer, Twinkle Popper, Flow Invader
                    switch (npc.type)
                    {
                        case NPCID.StardustCellBig:
                        case NPCID.StardustWormHead:
                        case NPCID.StardustWormBody:
                        case NPCID.StardustWormTail:
                            npc.active = false;
                            npc.netUpdate = true;
                            break;
                        default:
                            break;
                    }
                    break;
                case 1:
                case 0:
                    // Possible spawns: Twinkle Popper, Flow Invader
                    switch (npc.type)
                    {
                        case NPCID.StardustCellBig:
                        case NPCID.StardustWormHead:
                        case NPCID.StardustWormBody:
                        case NPCID.StardustWormTail:
                        case NPCID.StardustSoldier:
                            npc.active = false;
                            npc.netUpdate = true;
                            break;
                        default:
                            break;
                    }
                    break;
            }
        }
        #endregion

        #region AI
        public override void AI(NPC npc)
        {
            if (CalamityWorld.revenge && npc.type == NPCID.DungeonGuardian)
                SkeletronAI.RevengeanceDungeonGuardianAI(npc);
        }
        #endregion

        #region Post AI
        public override void PostAI(NPC npc)
        {
            // Worm heads emit dust when close enough to the player and digging through tiles
            if (npc.type == NPCID.GiantWormHead || npc.type == NPCID.DiggerHead || npc.type == NPCID.DevourerHead ||
                npc.type == NPCID.SeekerHead || npc.type == NPCID.TombCrawlerHead || npc.type == NPCID.BoneSerpentHead ||
                npc.type == NPCID.DuneSplicerHead)
            {
                Point point = npc.Center.ToTileCoordinates();
                Tile tileSafely = Framing.GetTileSafely(point);
                bool createDust = tileSafely.HasUnactuatedTile && npc.Distance(Main.player[npc.target].Center) < 800f;
                if (createDust)
                {
                    if (Main.rand.NextBool())
                    {
                        Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, 204, 0f, 0f, 150, default, 0.3f);
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
                        Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, 44, 0f, 0f, 250, default, 0.4f);
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
                        Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, 75, 0f, 0f, 100, default, 1.5f);
                        dust.noGravity = true;
                    }
                }
            }

            // Debuff decrements
            if (debuffResistanceTimer > 0)
                debuffResistanceTimer--;
            if (knockbackResistanceTimer > 0)
                knockbackResistanceTimer--;

            if (timeSlow > 0)
                timeSlow--;
            if (tesla > 0)
                tesla--;
            if (gState > 0)
                gState--;
            if (tSad > 0)
                tSad--;
            if (eutrophication > 0)
                eutrophication--;
            if (webbed > 0)
                webbed--;
            if (slowed > 0)
                slowed--;
            if (kamiFlu > 0)
                kamiFlu--;
            if (vaporfied > 0)
                vaporfied--;

            if (electrified > 0)
                electrified--;
            if (pearlAura > 0)
                pearlAura--;
            if (bBlood > 0)
                bBlood--;
            if (brainRot > 0)
                brainRot--;
            if (elementalMix > 0)
                elementalMix--;
            if (vulnerabilityHex > 0)
                vulnerabilityHex--;
            if (marked > 0)
                marked--;
            if (absorberAffliction > 0)
                absorberAffliction--;
            if (irradiated > 0)
                irradiated--;
            if (bFlames > 0)
                bFlames--;
            if (hFlames > 0)
                hFlames--;
            if (pFlames > 0)
                pFlames--;
            // Soma Prime's Shred stacks have a unique falloff mechanic in the debuff's own file.
            if (aCrunch > 0)
                aCrunch--;
            if (crumble > 0)
                crumble--;
            if (cDepth > 0)
                cDepth--;
            if (rTide > 0)
                rTide--;
            if (gsInferno > 0)
                gsInferno--;
            if (dragonFire > 0)
                dragonFire--;
            if (miracleBlight > 0)
                miracleBlight--;
            if (astralInfection > 0)
                astralInfection--;
            if (wDeath > 0)
                wDeath--;
            if (nightwither > 0)
                nightwither--;
            if (shellfishVore > 0)
                shellfishVore--;
            if (clamDebuff > 0)
                clamDebuff--;
            if (sulphurPoison > 0)
                sulphurPoison--;
            if (sagePoisonTime > 0)
                sagePoisonTime--;
            if (kamiFlu > 0)
                kamiFlu--;
            if (relicOfResilienceCooldown > 0)
                relicOfResilienceCooldown--;
            if (relicOfResilienceWeakness > 0)
                relicOfResilienceWeakness--;
            if (GaussFluxTimer > 0)
                GaussFluxTimer--;
            if (ladHearts > 0)
                ladHearts--;
            if (vulnerabilityHex > 0)
                vulnerabilityHex--;
            if (banishingFire > 0)
                banishingFire--;
            if (wither > 0)
                wither--;
            if (RancorBurnTime > 0)
                RancorBurnTime--;

            // Queen Bee is completely immune to having her movement impaired if not in a high difficulty mode.
            if (npc.type == NPCID.QueenBee && !CalamityWorld.revenge && !BossRushEvent.BossRushActive)
                return;

            if (debuffResistanceTimer <= 0 || (debuffResistanceTimer > slowingDebuffResistanceMin))
            {
                if (gState <= 0 && tSad <= 0)
                {
                    if (eutrophication > 0)
                    {
                        float velocityMult = 0.95f;
                        if (VulnerableToWater.HasValue)
                        {
                            if (VulnerableToWater.Value)
                                velocityMult = 0.6f;
                            else
                                velocityMult = 0.99f;
                        }
                        npc.velocity *= velocityMult;
                    }
                    else if (timeSlow > 0 || webbed > 0)
                    {
                        npc.velocity *= 0.85f;
                    }
                    else if (slowed > 0 || tesla > 0 || vaporfied > 0)
                    {
                        float velocityMult = 0.95f;
                        if (tesla > 0)
                        {
                            if (VulnerableToElectricity.HasValue)
                            {
                                if (VulnerableToElectricity.Value)
                                    velocityMult = 0.6f;
                                else
                                    velocityMult = 0.99f;
                            }
                        }
                        npc.velocity *= velocityMult;
                    }
                    else if (vulnerabilityHex > 0)
                    {
                        npc.velocity = Vector2.Clamp(npc.velocity, new Vector2(-Calamity.MaxNPCSpeed), new Vector2(Calamity.MaxNPCSpeed, 10f));
                    }
                    else if (kamiFlu > 420)
                    {
                        npc.velocity = Vector2.Clamp(npc.velocity, new Vector2(-KamiFlu.MaxNPCSpeed), new Vector2(KamiFlu.MaxNPCSpeed));
                    }
                }
            }

            if (!CalamityPlayer.areThereAnyDamnBosses && !CalamityLists.enemyImmunityList.Contains(npc.type))
            {
                if (pearlAura > 0)
                    npc.velocity *= 0.9f;
            }
        }
        #endregion

        #region On Hit Player
        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage <= 0)
                return;

            if (target.Calamity().sulfurSet)
                npc.AddBuff(BuffID.Poisoned, 120);

            if (target.Calamity().snowman)
            {
                if (npc.type == NPCID.Demon || npc.type == NPCID.VoodooDemon || npc.type == NPCID.RedDevil)
                    target.AddBuff(BuffType<PopoNoselessBuff>(), 36000);
            }

            switch (npc.type)
            {
                case NPCID.ShadowFlameApparition:
                    target.AddBuff(BuffType<Shadowflame>(), 180);
                    break;
                case NPCID.ChaosBall:
                    if (Main.hardMode || CalamityPlayer.areThereAnyDamnBosses)
                        target.AddBuff(BuffType<Shadowflame>(), 180);
                    break;

                case NPCID.Spazmatism:
                    if (npc.ai[0] != 1f && npc.ai[0] != 2f && npc.ai[0] != 0f)
                        target.AddBuff(BuffID.Bleeding, 600);
                    break;

                case NPCID.Plantera:
                    if (npc.life < npc.lifeMax / 2)
                        target.AddBuff(BuffID.Poisoned, 600);
                    break;
                case NPCID.PlanterasTentacle:
                    target.AddBuff(BuffID.Poisoned, 300);
                    break;

                case NPCID.Golem:
                    target.AddBuff(BuffType<ArmorCrunch>(), 480);
                    break;

                case NPCID.GolemHead:
                case NPCID.GolemHeadFree:
                case NPCID.GolemFistRight:
                case NPCID.GolemFistLeft:
                    target.AddBuff(BuffType<ArmorCrunch>(), 240);
                    break;

                case NPCID.AncientDoom:
                    target.AddBuff(BuffType<Shadowflame>(), 180);
                    break;
                case NPCID.AncientLight:
                    target.AddBuff(BuffType<HolyFlames>(), 120);
                    break;

                case NPCID.HallowBoss:
                    target.AddBuff(BuffType<HolyFlames>(), 320);
                    break;

                case NPCID.BloodNautilus:
                    target.AddBuff(BuffType<BurningBlood>(), 480);
                    break;

                case NPCID.GoblinShark:
                case NPCID.BloodEelHead:
                    target.AddBuff(BuffType<BurningBlood>(), 300);
                    break;
                case NPCID.BloodEelBody:
                    target.AddBuff(BuffType<BurningBlood>(), 180);
                    break;
                case NPCID.BloodEelTail:
                    target.AddBuff(BuffType<BurningBlood>(), 120);
                    break;

                case NPCID.Lavabat:
                    target.AddBuff(BuffID.OnFire, 180);
                    break;

                case NPCID.RuneWizard:
                    if (Main.zenithWorld)
                        target.AddBuff(BuffType<MiracleBlight>(), 600);
                    break;

                default:
                    break;
            }

            if (Main.hardMode)
            {
                switch (npc.type)
                {
                    case NPCID.BigBoned:
                    case NPCID.ShortBones:
                    case NPCID.AngryBones:
                    case NPCID.AngryBonesBig:
                    case NPCID.AngryBonesBigMuscle:
                    case NPCID.AngryBonesBigHelmet:
                        target.AddBuff(BuffType<ArmorCrunch>(), 60);
                        break;

                    default:
                        break;
                }

                if (NPC.downedPlantBoss)
                {
                    switch (npc.type)
                    {
                        case NPCID.RustyArmoredBonesAxe:
                        case NPCID.RustyArmoredBonesFlail:
                        case NPCID.RustyArmoredBonesSword:
                        case NPCID.RustyArmoredBonesSwordNoArmor:
                        case NPCID.BlueArmoredBones:
                        case NPCID.BlueArmoredBonesMace:
                        case NPCID.BlueArmoredBonesNoPants:
                        case NPCID.BlueArmoredBonesSword:
                        case NPCID.HellArmoredBones:
                        case NPCID.HellArmoredBonesSpikeShield:
                        case NPCID.HellArmoredBonesMace:
                        case NPCID.HellArmoredBonesSword:
                            target.AddBuff(BuffType<ArmorCrunch>(), 120);
                            break;

                        default:
                            break;
                    }
                }
            }

            if (Main.expertMode)
            {
                switch (npc.type)
                {
                    case NPCID.Hellbat:
                        target.AddBuff(BuffID.OnFire, 120);
                        break;

                    default:
                        break;
                }
            }
        }
        #endregion

        #region Modify Hit
        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.camper && !player.StandingStill())
                modifiers.SourceDamage *= 0.1f;

            // True melee resists
            if (CalamityLists.DesertScourgeIDs.Contains(npc.type) || CalamityLists.EaterofWorldsIDs.Contains(npc.type) || npc.type == NPCID.Creeper ||
                CalamityLists.PerforatorIDs.Contains(npc.type) || CalamityLists.AquaticScourgeIDs.Contains(npc.type) || CalamityLists.DestroyerIDs.Contains(npc.type) ||
                CalamityLists.AstrumDeusIDs.Contains(npc.type) || CalamityLists.StormWeaverIDs.Contains(npc.type) || CalamityLists.ThanatosIDs.Contains(npc.type) ||
                npc.type == NPCType<DarkEnergy>() || npc.type == NPCType<RavagerBody>() || CalamityLists.AresIDs.Contains(npc.type) || npc.type == NPCType<Crabulon.Crabulon>() ||
                npc.type == NPCType<ProfanedRocks>())
            {
                float damageMult = CalamityLists.ThanatosIDs.Contains(npc.type) ? 0.35f : 0.5f;
                if (item.CountsAsClass<MeleeDamageClass>() && item.type != ItemType<UltimusCleaver>() && item.type != ItemType<InfernaCutter>())
                    modifiers.SourceDamage *= damageMult;
            }
        }
        #endregion

        #region Modify Hit By Projectile
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            MakeTownNPCsTakeMoreDamage(npc, projectile, Mod, ref modifiers);

            // Block natural falling stars from killing boss spawners randomly
            if ((projectile.type == ProjectileID.FallingStar && projectile.damage >= 1000) && (npc.type == NPCType<PerforatorCyst>() || npc.type == NPCType<HiveTumor>() || npc.type == NPCType<LeviathanStart>()))
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

                    // Apply supercrit damage. This projectile is already guaranteed to be a crit, which will double its damage at the end.
                    // As such, only 50% damage is added per supercrit layer here.
                    modifiers.SourceDamage *= 1f + 0.5f * supercritLayers;
                }
            }

            //
            // DAAWNLIGHT SPIRIT ORIGIN AIM IMPLEMENTATION
            //
            if (modPlayer.spiritOrigin && projectile.CountsAsClass<RangedDamageClass>())
            {
                int bullseyeType = ProjectileType<SpiritOriginBullseye>();
                Projectile bullseye = null;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].type != bullseyeType || !Main.projectile[i].active || Main.projectile[i].owner != player.whoAmI)
                        continue;

                    // Only choose a bullseye if it is attached to the NPC that is being hit.
                    if (npc.whoAmI == (int)Main.projectile[i].ai[0])
                    {
                        bullseye = Main.projectile[i];
                        break;
                    }
                }

                // Don't allow large hitbox projectiles or explosions to "snipe" enemies.
                // Hitbox criteria were changed to allow long one dimensional projectiles so that Condemnation would work.
                bool hitBullseye = false;
                bool acceptableVelocity = projectile.velocity != Vector2.Zero;
                bool acceptableHitbox = (projectile.width <= 36) || (projectile.height <= 36);
                if (bullseye != null && acceptableVelocity && acceptableHitbox)
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
                    if (willStrikeBullseye && bullseye.ai[1] == 0f)
                    {
                        modifiers.SetCrit();
                        hitBullseye = true;
                        bullseye.ai[1] = 1f; // Make the bullseye disappear immediately.
                        bullseye.netUpdate = true;
                    }
                }

                // The bonus provided by Daawnlight Spirit Origin can be computed as a complete replacement to regular crits.
                // As such, it is subtracted by the base critical strike damage boost of 200%
                float bonus = DaawnlightSpiritOrigin.GetDamageMultiplier(player, modPlayer, hitBullseye, cgp.forcedCrit) - 2f;
                modifiers.CritDamage += bonus;
            }

            // Plague Reaper deals 1.1x damage to Plagued enemies
            if (!projectile.npcProj && !projectile.trap)
            {
                if (projectile.CountsAsClass<RangedDamageClass>() && modPlayer.plagueReaper && pFlames > 0)
                    modifiers.SourceDamage *= 1.1f;
            }

            // Any weapons that shoot projectiles from anywhere other than the player's center aren't affected by point-blank shot damage boost.
            if (!Main.player[projectile.owner].ActiveItem().IsAir && Main.player[projectile.owner].ActiveItem().Calamity().canFirePointBlankShots && projectile.CountsAsClass<RangedDamageClass>())
            {
                if (projectile.Calamity().pointBlankShotDuration > 0 && projectile.Calamity().pointBlankShotDistanceTravelled < CalamityGlobalProjectile.PointBlankShotDistanceLimit)
                {
                    float pointBlankShotDurationRatio = projectile.Calamity().pointBlankShotDuration / (float)CalamityGlobalProjectile.DefaultPointBlankDuration;
                    float pointBlankShotDistanceRatio = 1f - projectile.Calamity().pointBlankShotDistanceTravelled / CalamityGlobalProjectile.PointBlankShotDistanceLimit;
                    float pointBlankDamageRatio = (pointBlankShotDurationRatio < pointBlankShotDistanceRatio ? pointBlankShotDurationRatio : pointBlankShotDistanceRatio) * 0.5f;
                    float pointBlankShotDamageMultiplier = 1f + pointBlankDamageRatio;
                    modifiers.SourceDamage *= pointBlankShotDamageMultiplier;
                    projectile.Calamity().pointBlankShotDuration = 0;
                }
            }

            // Apply balancing resists/vulnerabilities.
            BalancingChangesManager.ApplyFromProjectile(npc, ref modifiers, projectile);

            if (CalamityLists.GrenadeResistIDs.Contains(projectile.type))
            {
                // Eater of Worlds has a vanilla resist in Expert+, this gives it to him in Normal mode
                bool hasResist = CalamityLists.EaterofWorldsIDs.Contains(npc.type) && !Main.expertMode;
                // Add a resist for BoC's creepers and Prehardmode worm bosses
                if (npc.type == NPCID.Creeper || CalamityLists.DesertScourgeIDs.Contains(npc.type) || CalamityLists.PerforatorIDs.Contains(npc.type))
                    hasResist = true;
                if (hasResist)
                    modifiers.SourceDamage *= 0.2f;
            }

            if (CalamityLists.pierceResistList.Contains(npc.type))
                PierceResistGlobal(projectile, npc, ref modifiers);

            if (modPlayer.camper && !player.StandingStill())
                modifiers.SourceDamage *= 0.1f;

            if (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type] || projectile.sentry || ProjectileID.Sets.SentryShot[projectile.type])
                EditWhipTagDamage(projectile, npc, ref modifiers);
        }

        // Generalized pierce resistance that stacks with all other resistances for some specific bosses defined in a list.
        // The actual resistance formula isn't really a problem, but the implementation of this desperately needs refactoring.
        private void PierceResistGlobal(Projectile projectile, NPC npc, ref NPC.HitModifiers modifiers)
        {
            // Thanatos segments do not trigger pierce resistance if they are closed
            if (CalamityLists.ThanatosIDs.Contains(npc.type) && unbreakableDR)
                return;

            float damageReduction = projectile.Calamity().timesPierced * CalamityGlobalProjectile.PierceResistHarshness;
            if (damageReduction > CalamityGlobalProjectile.PierceResistCap)
                damageReduction = CalamityGlobalProjectile.PierceResistCap;

            modifiers.FinalDamage *= 1f - damageReduction;

            if ((projectile.penetrate > 1 || projectile.penetrate == -1) && !CalamityLists.pierceResistExceptionList.Contains(projectile.type) && !projectile.CountsAsClass<SummonDamageClass>() && projectile.aiStyle != 15 && projectile.aiStyle != 39 && projectile.aiStyle != 99)
                projectile.Calamity().timesPierced++;
        }

        // Make whip tags multiplicative, by effectively reversing the process done to it
        private void EditWhipTagDamage(Projectile proj, NPC npc, ref NPC.HitModifiers modifiers)
        {
            // Don't make it run through the index if it's a trap
            if (proj.npcProj || proj.trap)
                return;

            float TagDamageMult = ProjectileID.Sets.SummonTagDamageMultiplier[proj.type];
            for (int i = 0; i < NPC.maxBuffs; i++)
            {
                if (npc.buffTime[i] >= 1)
                {
                    switch (npc.buffType[i])
                    {
                        case BuffID.BlandWhipEnemyDebuff: // Leather Whip
                            modifiers.FlatBonusDamage += -4f * TagDamageMult;
                            modifiers.ScalingBonusDamage += (BalancingConstants.DurendalTagDamageMultiplier - 1f) * TagDamageMult;
                            break;
                        case BuffID.ThornWhipNPCDebuff: // Snapthorn
                            modifiers.FlatBonusDamage += -6f * TagDamageMult;
                            modifiers.ScalingBonusDamage += (BalancingConstants.SnapthornTagDamageMultiplier - 1f) * TagDamageMult;
                            break;
                        case BuffID.BoneWhipNPCDebuff: // Spinal Tap
                            modifiers.FlatBonusDamage += -7f * TagDamageMult;
                            modifiers.ScalingBonusDamage += (BalancingConstants.SpinalTapTagDamageMultiplier - 1f) * TagDamageMult;
                            break;
                        case BuffID.FlameWhipEnemyDebuff: // Firecracker
                            modifiers.ScalingBonusDamage += (BalancingConstants.FirecrackerExplosionDamageMultiplier - 2.75f) * TagDamageMult;
                            break;
                        case BuffID.CoolWhipNPCDebuff: // Cool Whip
                            modifiers.FlatBonusDamage += -6f * TagDamageMult;
                            modifiers.ScalingBonusDamage += (BalancingConstants.CoolWhipTagDamageMultiplier - 1f) * TagDamageMult;
                            break;
                        case BuffID.SwordWhipNPCDebuff: // Durendal
                            modifiers.FlatBonusDamage += -9f * TagDamageMult;
                            modifiers.ScalingBonusDamage += (BalancingConstants.DurendalTagDamageMultiplier - 1f) * TagDamageMult;
                            break;
                        case BuffID.ScytheWhipEnemyDebuff: // Dark Harvest
                            modifiers.FlatBonusDamage += -10f * TagDamageMult;
                            break;
                        case BuffID.MaceWhipNPCDebuff: // Morning Star
                            modifiers.FlatBonusDamage += -8f * TagDamageMult;
                            modifiers.ScalingBonusDamage += (BalancingConstants.MorningStarTagDamageMultiplier - 1f) * TagDamageMult;
                            break;
                        case BuffID.RainbowWhipNPCDebuff: // Kaleidoscope
                            modifiers.FlatBonusDamage += -20f * TagDamageMult;
                            modifiers.ScalingBonusDamage += (BalancingConstants.KaleidoscopeTagDamageMultiplier - 1f) * TagDamageMult;
                            break;
                    }
                }
            }
            //BuffType cannot be used in switch case, so that has to be handled outside of it
            //Verify that the owner of the proj has psc state higher or equal to psc buffs
            if (npc.HasBuff<ProfanedCrystalWhipDebuff>() && Main.player[proj.owner].Calamity().pscState >= (int)ProfanedSoulCrystal.ProfanedSoulCrystalState.Buffs)
            {
                var empowered = Main.player[proj.owner].Calamity().pscState == (int)ProfanedSoulCrystal.ProfanedSoulCrystalState.Empowered;
                //20% is balanced for non empowered, while 40% helps ensure psc remains balanced at empowered tier
                //Some PSC projectiles receive a reduced amount of benefit from this, for balancing purposes
                modifiers.ScalingBonusDamage += (empowered ? 0.4f : 0.2f) * TagDamageMult;
                if (Main.netMode != NetmodeID.Server)
                {
                    var color = ProvUtils.GetProjectileColor((int)(Main.dayTime ? Providence.Providence.BossMode.Day : Providence.Providence.BossMode.Night), 0);
                    float power = Math.Min(npc.height / 100f, 3f);
                    var position = new Vector2(Main.rand.NextFloat(npc.Left.X, npc.Right.X), Main.rand.NextFloat(npc.Top.Y, npc.Bottom.Y));
                    var particle = new FlameParticle(position, 50, 0.25f, power, color * (Main.dayTime ? 1f : 1.25f), color * (Main.dayTime ? 1.25f : 1f));
                    GeneralParticleHandler.SpawnParticle(particle);
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
                npc.value > 0f && npc.HasPlayerTarget &&
                NPC.downedMoonlord &&
                Main.player[npc.target].ZoneDungeon)
            {
                int maxValue = Main.expertMode ? 4 : 6;

                if (Main.rand.NextBool(maxValue) && Main.wallDungeon[Main.tile[(int)npc.Center.X / 16, (int)npc.Center.Y / 16].WallType])
                {
                    int randomType = Utils.SelectRandom(Main.rand, new int[]
                    {
                        NPCType<PhantomSpirit>(),
                        NPCType<PhantomSpiritS>(),
                        NPCType<PhantomSpiritM>(),
                        NPCType<PhantomSpiritL>()
                    });

                    NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, randomType, 0, 0f, 0f, 0f, 0f, 255);
                }
            }

            return true;
        }
        #endregion

        #region Hit Effect
        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            if (npc.life <= 0 && npc.Organic() && RancorBurnTime > 0)
                DeathAshParticle.CreateAshesFromNPC(npc);

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
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (float)(npc.width / 2)), (int)(npc.position.Y + (float)npc.height), ModContent.NPCType<PlanterasFreeTentacle>());
                        }
                        break;

                    case NPCID.MotherSlime:
                        if (npc.life <= 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int slimeAmt = Main.getGoodWorld ? Main.rand.Next(6) + 10 : Main.rand.Next(2) + 2; // 2 to 3 extra
                                for (int s = 0; s < slimeAmt; s++)
                                {
                                    int slime = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)(npc.position.Y + npc.height), NPCID.BlueSlime, 0, 0f, 0f, 0f, 0f, 255);
                                    NPC npc2 = Main.npc[slime];
                                    npc2.SetDefaults(NPCID.BabySlime);
                                    npc2.velocity.X = npc.velocity.X * 2f;
                                    npc2.velocity.Y = npc.velocity.Y;
                                    npc2.velocity.X += Main.rand.Next(-20, 20) * (Main.getGoodWorld ? 0.5f : 0.1f) + s * npc.direction * (Main.getGoodWorld ? 0.5f : 0.3f);
                                    npc2.velocity.Y -= Main.rand.Next(0, 10) * (Main.getGoodWorld ? 0.5f : 0.1f) + s;
                                    npc2.ai[0] = -1000 * Main.rand.Next(3);

                                    if (Main.netMode == NetmodeID.Server && slime < Main.maxNPCs)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, slime, 0f, 0f, 0f, 0, 0, 0);
                                }
                            }
                        }
                        break;

                    case NPCID.CursedHammer:
                    case NPCID.EnchantedSword:
                    case NPCID.CrimsonAxe:
                        if (Main.getGoodWorld)
                            npc.justHit = false;

                        break;

                    case NPCID.Clinger:
                    case NPCID.Gastropod:
                    case NPCID.GiantTortoise:
                    case NPCID.IceTortoise:
                    case NPCID.BlackRecluse:
                    case NPCID.BlackRecluseWall:
                        if (Main.getGoodWorld)
                            npc.justHit = false;

                        break;

                    case NPCID.Paladin:
                        if (Main.getGoodWorld)
                            npc.justHit = false;

                        break;
                }

                if (npc.type == NPCType<Plagueshell>())
                {
                    if (Main.getGoodWorld)
                        npc.justHit = false;
                }
            }

            if (pFlames > 0 && npc.life <= 0)
            {
                Rectangle hitbox = npc.Hitbox;
                for (int i = 0; i < 20; i++)
                {
                    int idx = Dust.NewDust(hitbox.TopLeft(), npc.width, npc.height, 89, 0f, -2.5f);
                    Dust dust = Main.dust[idx];
                    dust.alpha = 200;
                    dust.velocity *= 1.4f;
                    dust.scale += Main.rand.NextFloat();
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int j = 0; j < Main.maxNPCs; j++)
                    {
                        NPC nPC = Main.npc[j];
                        if (nPC.active && !nPC.buffImmune[BuffType<Plague>()] && npc.Distance(nPC.Center) < 100f && !nPC.dontTakeDamage && nPC.lifeMax > 5 && !nPC.friendly && !nPC.townNPC)
                            nPC.AddBuff(BuffType<Plague>(), 300);
                    }
                }
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

            if (CalamityWorld.death && Main.bloodMoon)
            {
                spawnRate = (int)(spawnRate * 0.25);
                maxSpawns = (int)(maxSpawns * 10f);
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

            if (Main.SceneMetrics.WaterCandleCount > 0)
            {
                spawnRate = (int)(spawnRate * 0.9);
                maxSpawns = (int)(maxSpawns * 1.1f);
            }
            if (player.enemySpawns)
            {
                spawnRate = (int)(spawnRate * 0.8);
                maxSpawns = (int)(maxSpawns * 1.2f);
            }
            if (player.Calamity().chaosCandle)
            {
                spawnRate = (int)(spawnRate * 0.6);
                maxSpawns = (int)(maxSpawns * 2.5f);
            }
            if (player.Calamity().zerg)
            {
                spawnRate = (int)(spawnRate * 0.2);
                maxSpawns = (int)(maxSpawns * 5f);
            }

            // This is horribly unoptimized, I'm leaving it commented out. - Fab
            /*if (NPC.AnyNPCs(NPCType<WulfrumAmplifier>()))
            {
                int otherWulfrumEnemies = NPC.CountNPCS(NPCType<WulfrumDrone>()) + NPC.CountNPCS(NPCType<WulfrumGyrator>()) + NPC.CountNPCS(NPCType<WulfrumHovercraft>()) + NPC.CountNPCS(NPCType<WulfrumRover>());
                if (otherWulfrumEnemies < 4)
                {
                    spawnRate = (int)(spawnRate * 0.8);
                    maxSpawns = (int)(maxSpawns * 1.2f);
                }
            }*/

            // Reductions
            if (Main.SceneMetrics.PeaceCandleCount > 0)
            {
                spawnRate = (int)(spawnRate * 1.1);
                maxSpawns = (int)(maxSpawns * 0.9f);
            }
            if (player.calmed)
            {
                spawnRate = (int)(spawnRate * 1.2);
                maxSpawns = (int)(maxSpawns * 0.8f);
            }
            if (player.Calamity().tranquilityCandle)
            {
                spawnRate = (int)(spawnRate * 1.4);
                maxSpawns = (int)(maxSpawns * 0.4f);
            }
            if (player.Calamity().zen || (CalamityConfig.Instance.ForceTownSafety && player.townNPCs > 1f && Main.expertMode))
            {
                spawnRate = (int)(spawnRate * 2.5);
                maxSpawns = (int)(maxSpawns * 0.3f);
            }
            if (player.Calamity().isNearbyBoss && CalamityConfig.Instance.BossZen)
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

        internal static readonly FieldInfo MaxSpawnsField = typeof(NPC).GetField("maxSpawns", BindingFlags.NonPublic | BindingFlags.Static);

        public static void AttemptToSpawnLabCritters(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int spawnRate = 400;
            int maxSpawnCount = (int)MaxSpawnsField.GetValue(null);
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
                pool.Add(NPCType<Androomba>(), 0.001f);
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
                    if (Main.netMode == NetmodeID.Server && spawnedNPC < Main.maxNPCs)
                    {
                        Main.npc[spawnedNPC].position.Y -= 8f;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, spawnedNPC);
                        return;
                    }
                }
            }
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            bool calamityBiomeZone = spawnInfo.Player.Calamity().ZoneAbyss ||
                spawnInfo.Player.Calamity().ZoneCalamity ||
                spawnInfo.Player.Calamity().ZoneSulphur ||
                spawnInfo.Player.Calamity().ZoneSunkenSea ||
                (spawnInfo.Player.Calamity().ZoneAstral && !spawnInfo.Player.PillarZone());

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

            // 12JUL2023: Ozzatron: what does this do
            // 27SEP2023: Fabsol: disables vanilla spawns "a pool of [0] indicates vanilla spawning"
            if (calamityBiomeZone)
            {
                pool[0] = 0f;
            }

            // Add Enchanted Nightcrawlers as a critter to the Astral Infection
            if (!AnyEvents(spawnInfo.Player) && spawnInfo.Player.InAstral())
            {
                pool[NPCID.EnchantedNightcrawler] = SpawnCondition.TownCritter.Chance;
            }

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
                    {
                        pool.Remove(NPCType<NuclearToad>());
                    }
                }
            }

            if (spawnInfo.PlayerSafe)
                return;

            // Voodoo Demon changes (including partial Voodoo Demon Voodoo Doll implementation)
            bool voodooDemonDollActive = spawnInfo.Player.Calamity().disableVoodooSpawns;

            // If the doll is active, Voodoo Demons cannot spawn (via modded means).
            if (voodooDemonDollActive)
                pool.Remove(NPCID.VoodooDemon);
            // Otherwise, if it's pre-Hardmode, provide a modded spawn entry that makes them much more common.
            else if (!Main.hardMode && spawnInfo.Player.ZoneUnderworldHeight && !calamityBiomeZone)
            {
                pool[NPCID.VoodooDemon] = SpawnCondition.Underworld.Chance * 0.15f;
            }
        }
        #endregion

        #region On Spawn
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
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
            if (CalamityWorld.revenge || BossRushEvent.BossRushActive)
            {
                if (npc.type == NPCID.SkeletronPrime)
                    npc.frameCounter = 0D;
            }
        }

        // Debuff visuals. Alphabetical order as per usual, please
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (absorberAffliction > 0)
                AbsorberAffliction.DrawEffects(npc, ref drawColor);

            if (astralInfection > 0)
                AstralInfectionDebuff.DrawEffects(npc, ref drawColor);

            // Brimstone Flames and Demonshade Enrage set bonus share the same visual effects
            // TODO -- change this when Demonshade is reworked
            if (bFlames > 0 || npc.HasBuff<Enraged>())
                BrimstoneFlames.DrawEffects(npc, ref drawColor);

            if (bBlood > 0)
                BurningBlood.DrawEffects(npc, ref drawColor);

            if (brainRot > 0)
                BrainRot.DrawEffects(npc, ref drawColor);

            if (cDepth > 0)
                CrushDepth.DrawEffects(npc, ref drawColor);

            if (dragonFire > 0)
                Dragonfire.DrawEffects(npc, ref drawColor);

            if (elementalMix > 0)
                ElementalMix.DrawEffects(npc, ref drawColor);

            // Eutrophication and Temporal Sadness share the same visual effects
            if (eutrophication > 0 || tSad > 0)
                Eutrophication.DrawEffects(npc, ref drawColor);

            if (gsInferno > 0)
                GodSlayerInferno.DrawEffects(npc, ref drawColor);

            // Holy Flames and Banishing Fire share the same visual effects
            if (hFlames > 0 || banishingFire > 0)
                HolyFlames.DrawEffects(npc, ref drawColor);

            // These draw effects do not include Miracle Blight's shader
            if (miracleBlight > 0)
                MiracleBlight.DrawEffects(npc, ref drawColor);

            if (nightwither > 0)
                Nightwither.DrawEffects(npc, ref drawColor);

            if (pFlames > 0) // Plague debuff
                Plague.DrawEffects(npc, ref drawColor);

            if (rTide > 0)
                RiptideDebuff.DrawEffects(npc, ref drawColor);

            if (somaShredStacks > 0 && Main.netMode != NetmodeID.Server)
                Shred.DrawEffects(npc, this, ref drawColor);

            if (sulphurPoison > 0)
                SulphuricPoisoning.DrawEffects(npc, ref drawColor);

            if (vaporfied > 0)
                Vaporfied.DrawEffects(npc, ref drawColor);

            // TODO -- These debuff visuals cannot be moved because they correspond to vanilla debuffs
            if (electrified > 0)
            {
                if (Main.rand.NextBool())
                {
                    Dust.NewDustDirect(npc.position, npc.width, npc.height, 226, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 0, default, 0.35f);
                }
            }
            if (slowed > 0)
            {
                if (Main.rand.Next(5) < 4)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 191, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 225, default, 3f);
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
            if (webbed > 0)
            {
                if (Main.rand.Next(5) < 4)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 30, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 1.5f);
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

            // Some extraneous and probably undocumented visual effect caused by the heart lad pet thing
            if (ladHearts > 0 && !npc.loveStruck && Main.netMode != NetmodeID.Server)
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
            if (gState > 0)
                drawColor = Color.Cyan;

            else if (electrified > 0)
                drawColor = Main.rand.NextBool(5) ? Color.White : Color.SlateGray;

            else if (absorberAffliction > 0)
                drawColor = Color.DarkSeaGreen;

            else if (marked > 0 || vaporfied > 0)
                drawColor = Color.Fuchsia;

            else if (pearlAura > 0)
                drawColor = Color.White;

            else if (timeSlow > 0 || tesla > 0)
                drawColor = Color.Aquamarine;
        }

        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            // Don't make this affect the bestiary, that's goofy
            if (npc.IsABestiaryIconDummy)
                return null;

            if (Main.LocalPlayer.Calamity().trippy || (npc.type == NPCID.KingSlime && CalamityWorld.LegendaryMode && CalamityWorld.revenge))
                return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, npc.alpha);

            if (npc.type == NPCID.QueenBee && Main.zenithWorld)
            {
                if (npc.life / (float)npc.lifeMax < 0.5f)
                    return new Color(0, 255, 0, npc.alpha);
                else
                    return new Color(255, 0, 0, npc.alpha);
            }

            if (npc.HasBuff<Enraged>())
                return new Color(200, 50, 50, npc.alpha);

            if (npc.Calamity().kamiFlu > 0 && !CalamityLists.kamiDebuffColorImmuneList.Contains(npc.type))
                return new Color(51, 197, 108, npc.alpha);

            if (npc.type == NPCID.VileSpit || npc.type == NPCID.VileSpitEaterOfWorlds)
                return new Color(150, 200, 0, npc.alpha);

            if (npc.type == NPCID.AncientDoom || npc.type == NPCID.QueenSlimeMinionBlue || npc.type == NPCID.QueenSlimeMinionPink || npc.type == NPCID.QueenSlimeMinionPurple)
                return new Color(255, 255, 255, npc.alpha);

            return null;
        }

        public static List<(string, Predicate<NPC>)> moddedDebuffTextureList = new List<(string, Predicate<NPC>)>
        {
            // All Calamity DoTs in alphabetical order
            ("CalamityMod/Buffs/DamageOverTime/AstralInfectionDebuff", NPC => NPC.Calamity().astralInfection > 0),
            ("CalamityMod/Buffs/DamageOverTime/BanishingFire", NPC => NPC.Calamity().banishingFire > 0),
            ("CalamityMod/Buffs/DamageOverTime/BrainRot", NPC => NPC.Calamity().brainRot > 0),
            ("CalamityMod/Buffs/DamageOverTime/BrimstoneFlames", NPC => NPC.Calamity().bFlames > 0),
            ("CalamityMod/Buffs/DamageOverTime/BurningBlood", NPC => NPC.Calamity().bBlood > 0),
            ("CalamityMod/Buffs/DamageOverTime/CrushDepth", NPC => NPC.Calamity().cDepth > 0),
            ("CalamityMod/Buffs/DamageOverTime/Dragonfire", NPC => NPC.Calamity().dragonFire > 0),
            ("CalamityMod/Buffs/DamageOverTime/ElementalMix", NPC => NPC.Calamity().elementalMix > 0),
            ("CalamityMod/Buffs/DamageOverTime/GodSlayerInferno", NPC => NPC.Calamity().gsInferno > 0),
            ("CalamityMod/Buffs/DamageOverTime/HolyFlames", NPC => NPC.Calamity().hFlames > 0),
            ("CalamityMod/Buffs/DamageOverTime/MiracleBlight", NPC => NPC.Calamity().miracleBlight > 0),
            ("CalamityMod/Buffs/DamageOverTime/Nightwither", NPC => NPC.Calamity().nightwither > 0),
            ("CalamityMod/Buffs/DamageOverTime/Plague", NPC => NPC.Calamity().pFlames > 0),
            ("CalamityMod/Buffs/DamageOverTime/RancorBurn", NPC => NPC.Calamity().RancorBurnTime > 0),
            ("CalamityMod/Buffs/DamageOverTime/RiptideDebuff", NPC => NPC.Calamity().rTide > 0),
            ("CalamityMod/Buffs/DamageOverTime/SagePoison", NPC => NPC.Calamity().sagePoisonTime > 0),
            ("CalamityMod/Buffs/DamageOverTime/ShellfishClaps", NPC => NPC.Calamity().shellfishVore > 0),
            ("CalamityMod/Buffs/DamageOverTime/Shred", NPC => NPC.Calamity().somaShredStacks > 0),
            ("CalamityMod/Buffs/DamageOverTime/SnapClamDebuff", NPC => NPC.Calamity().clamDebuff > 0),
            ("CalamityMod/Buffs/DamageOverTime/SulphuricPoisoning", NPC => NPC.Calamity().sulphurPoison > 0),
            ("CalamityMod/Buffs/DamageOverTime/Vaporfied", NPC => NPC.Calamity().vaporfied > 0),
            ("CalamityMod/Buffs/DamageOverTime/VulnerabilityHex", NPC => NPC.Calamity().vulnerabilityHex > 0),

            // All other important Calamity debuffs, in alphabetical order
            ("CalamityMod/Buffs/StatDebuffs/AbsorberAffliction", NPC => NPC.Calamity().absorberAffliction > 0),
            ("CalamityMod/Buffs/StatDebuffs/ArmorCrunch", NPC => NPC.Calamity().aCrunch > 0),
            ("CalamityMod/Buffs/StatDebuffs/Crumbling", NPC => NPC.Calamity().crumble > 0),
            ("CalamityMod/Buffs/StatDebuffs/Eutrophication", NPC => NPC.Calamity().eutrophication > 0),
            ("CalamityMod/Buffs/StatDebuffs/GalvanicCorrosion", NPC => NPC.Calamity().tesla > 0),
            ("CalamityMod/Buffs/StatDebuffs/GlacialState", NPC => NPC.Calamity().gState > 0),
            ("CalamityMod/Buffs/StatDebuffs/Irradiated", NPC => NPC.Calamity().irradiated > 0),
            ("CalamityMod/Buffs/StatDebuffs/KamiFlu", NPC => NPC.Calamity().kamiFlu > 0),
            ("CalamityMod/Buffs/StatDebuffs/MarkedforDeath", NPC => NPC.Calamity().marked > 0),
            ("CalamityMod/Buffs/StatDebuffs/PearlAura", NPC => NPC.Calamity().pearlAura > 0),
            ("CalamityMod/Buffs/StatDebuffs/ProfanedWeakness", NPC => NPC.Calamity().relicOfResilienceWeakness > 0),
            ("CalamityMod/Buffs/StatDebuffs/TemporalSadness", NPC => NPC.Calamity().tSad > 0),
            ("CalamityMod/Buffs/StatDebuffs/TimeDistortion", NPC => NPC.Calamity().timeSlow > 0),
            ("CalamityMod/Buffs/StatDebuffs/WhisperingDeath", NPC => NPC.Calamity().wDeath > 0),
            ("CalamityMod/Buffs/StatDebuffs/WitherDebuff", NPC => NPC.Calamity().wither > 0),
        };

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.type != NPCID.BrainofCthulhu && (npc.type != NPCID.DukeFishron || npc.ai[0] <= 9f) && npc.active)
            {
                if (CalamityConfig.Instance.DebuffDisplay && (npc.boss || BossHealthBarManager.MinibossHPBarList.Contains(npc.type) || BossHealthBarManager.OneToMany.ContainsKey(npc.type) || CalamityLists.needsDebuffIconDisplayList.Contains(npc.type)))
                {
                    List<Texture2D> currentDebuffs = new List<Texture2D>() { };

                    for (int b = 0; b < moddedDebuffTextureList.Count(); b++)
                    {
                        if (moddedDebuffTextureList[b].Item2.Invoke(npc))
                        {
                            currentDebuffs.Add(Request<Texture2D>(moddedDebuffTextureList[b].Item1).Value);
                        }
                    }

                    // Vanilla damage over time debuffs
                    if (electrified > 0)
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
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Oiled].Value);
                    if (npc.javelined)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.BoneJavelin].Value);
                    if (npc.daybreak)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Daybreak].Value);
                    if (npc.celled)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.StardustMinionBleed].Value);
                    if (npc.dryadBane)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.DryadsWardDebuff].Value);
                    if (npc.dryadWard)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.DryadsWard].Value);
                    if (npc.soulDrain && npc.realLife == -1)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.SoulDrain].Value);
                    if (npc.onFire3) // Hellfire
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.OnFire3].Value);
                    if (npc.onFrostBurn2) // Frostbite
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Frostburn2].Value);
                    if (npc.tentacleSpiked)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.TentacleSpike].Value);

                    // Vanilla stat debuffs
                    if (npc.confused)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Confused].Value);
                    if (npc.ichor)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Ichor].Value);
                    if (slowed > 0)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Slow].Value);
                    if (webbed > 0)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Webbed].Value);
                    if (npc.midas)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Midas].Value);
                    if (npc.loveStruck)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Lovestruck].Value);
                    if (npc.stinky)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Stinky].Value);
                    if (npc.betsysCurse)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.BetsysCurse].Value);
                    if (npc.dripping)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Wet].Value);
                    if (npc.drippingSlime)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.Slimed].Value);
                    if (npc.drippingSparkleSlime)
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.GelBalloonBuff].Value);
                    if (npc.markedByScytheWhip) // Dark Harvest whip, the only Whip debuff that has an NPC bool
                        currentDebuffs.Add(TextureAssets.Buff[BuffID.ScytheWhipEnemyDebuff].Value);

                    // Total amount of elements in the buff list
                    int buffTextureListLength = currentDebuffs.Count();

                    // Total length of a single row in the buff display
                    int totalLength = buffTextureListLength * 14;

                    // Max amount of buffs per row
                    int buffDisplayRowLimit = 5;

                    // The maximum length of a single row in the buff display
                    // Limited to 80 units, because every buff drawn here is half the size of a normal buff, 16 x 16, 16 * 5 = 80 units
                    float drawPosX = totalLength >= 80f ? 40f : (float)(totalLength / 2);

                    // The height of a single frame of the npc
                    float npcHeight = (float)(TextureAssets.Npc[npc.type].Value.Height / Main.npcFrameCount[npc.type] / 2) * npc.scale;

                    // Offset the debuff display based on the npc's graphical offset, and 16 units, to create some space between the sprite and the display
                    float drawPosY = npcHeight + npc.gfxOffY + 16f;

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

                        // TODO -- Show number of Shred stacks (how?)
                    }
                }
            }

            TownNPCAlertSystem(npc, Mod, spriteBatch);

            if (CalamityWorld.revenge || BossRushEvent.BossRushActive)
            {
                if (npc.type == NPCID.SkeletronPrime || CalamityLists.DestroyerIDs.Contains(npc.type))
                    return false;
            }

            if (npc.type == NPCID.GolemHeadFree)
            {
                // Draw the head as usual.
                Texture2D golemHeadTexture = TextureAssets.Npc[npc.type].Value;
                Vector2 headDrawPosition = npc.Center - screenPos;
                spriteBatch.Draw(golemHeadTexture, headDrawPosition, npc.frame, npc.GetAlpha(drawColor), 0f, npc.frame.Size() * 0.5f, npc.scale, SpriteEffects.None, 0f);

                // Draw the eyes. The way vanilla handles this is hardcoded bullshit that cannot handle different hitboxes and thus requires rewriting.
                Color eyeColor = new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, 0);
                Vector2 eyesDrawPosition = headDrawPosition - npc.scale * new Vector2(1f, 12f);
                Rectangle eyesFrame = new Rectangle(0, 0, TextureAssets.Golem[1].Value.Width, TextureAssets.Golem[1].Value.Height / 2);
                spriteBatch.Draw(TextureAssets.Golem[1].Value, eyesDrawPosition, eyesFrame, eyeColor, 0f, eyesFrame.Size() * 0.5f, npc.scale, SpriteEffects.None, 0f);
                return false;
            }

            if (Main.LocalPlayer.Calamity().trippy && !npc.IsABestiaryIconDummy)
            {
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (npc.spriteDirection == 1)
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }

                Vector2 halfSizeTexture = new Vector2(TextureAssets.Npc[npc.type].Value.Width / 2, TextureAssets.Npc[npc.type].Value.Height / Main.npcFrameCount[npc.type] / 2);
                Color rainbow = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0);
                Color alphaColor = npc.GetAlpha(rainbow);
                float RGBMult = 0.99f;
                alphaColor.R = (byte)(alphaColor.R * RGBMult);
                alphaColor.G = (byte)(alphaColor.G * RGBMult);
                alphaColor.B = (byte)(alphaColor.B * RGBMult);
                alphaColor.A = (byte)(alphaColor.A * RGBMult);
                float xOffset = screenPos.X + npc.width / 2 - TextureAssets.Npc[npc.type].Value.Width * npc.scale / 2f + halfSizeTexture.X * npc.scale;
                float yOffset = screenPos.Y + npc.height - TextureAssets.Npc[npc.type].Value.Height * npc.scale / Main.npcFrameCount[npc.type] + 4f + halfSizeTexture.Y * npc.scale + npc.gfxOffY;
                for (int i = 0; i < 4; i++)
                {
                    Vector2 position9 = npc.position;
                    float num214 = Math.Abs(npc.Center.X - Main.LocalPlayer.Center.X);
                    float num215 = Math.Abs(npc.Center.Y - Main.LocalPlayer.Center.Y);

                    if (i == 0 || i == 2)
                        position9.X = Main.LocalPlayer.Center.X + num214;
                    else
                        position9.X = Main.LocalPlayer.Center.X - num214;

                    position9.X -= npc.width / 2;

                    if (i == 0 || i == 1)
                        position9.Y = Main.LocalPlayer.Center.Y + num215;
                    else
                        position9.Y = Main.LocalPlayer.Center.Y - num215;

                    position9.Y -= npc.height / 2;

                    Main.spriteBatch.Draw(TextureAssets.Npc[npc.type].Value, new Vector2(position9.X - xOffset, position9.Y - yOffset), new Microsoft.Xna.Framework.Rectangle?(npc.frame), alphaColor, npc.rotation, halfSizeTexture, npc.scale, spriteEffects, 0f);
                }
            }
            else
            {
                // VHex and Miracle Blight visuals do not appear if Odd Mushroom is in use for sanity reasons
                if (npc.Calamity().vulnerabilityHex > 0)
                {
                    float compactness = npc.width * 0.6f;
                    if (compactness < 10f)
                        compactness = 10f;
                    float power = npc.height / 100f;
                    if (power > 2.75f)
                        power = 2.75f;
                    if (VulnerabilityHexFireDrawer is null || VulnerabilityHexFireDrawer.LocalTimer >= VulnerabilityHexFireDrawer.SetLifetime)
                        VulnerabilityHexFireDrawer = new FireParticleSet(npc.Calamity().vulnerabilityHex, 1, Color.Red * 1.25f, Color.Red, compactness, power);
                    else
                        VulnerabilityHexFireDrawer.DrawSet(npc.Bottom - Vector2.UnitY * (12f - npc.gfxOffY));
                }
                else
                    VulnerabilityHexFireDrawer = null;

                // Only draw the NPC if told to by the miracle blight drawer.
                if (MiracleBlightDrawer.ValidToDraw(npc))
                    return MiracleBlightDrawer.ActuallyDoPreDraw;
            }

            // Draw a pillar of light and fade the background as an animation when skipping things in the DD2 event.
            if (npc.type == NPCID.DD2EterniaCrystal)
            {
                float animationTime = 120f - npc.ai[3];
                animationTime /= 120f;

                if (!Main.dedServ)
                {
                    if (!Filters.Scene["CrystalDestructionColor"].IsActive())
                        Filters.Scene.Activate("CrystalDestructionColor");

                    Filters.Scene["CrystalDestructionColor"].GetShader().UseIntensity((float)Math.Sin(animationTime * MathHelper.Pi) * 0.4f);
                }

                Vector2 drawPosition = npc.Center - screenPos + Vector2.UnitY * 60f;
                for (int i = 0; i < 4; i++)
                {
                    float intensity = MathHelper.Clamp(animationTime * 2f - i / 3f, 0f, 1f);
                    Vector2 origin = new Vector2(TextureAssets.MagicPixel.Value.Width / 2f, TextureAssets.MagicPixel.Value.Height);
                    Vector2 scale = new Vector2((float)Math.Sqrt(intensity) * 50f, intensity * 4f);
                    Color beamColor = new Color(0.4f, 0.17f, 0.4f, 0f) * (intensity * (1f - MathHelper.Clamp((animationTime - 0.8f) / 0.2f, 0f, 1f))) * 0.5f;
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value, drawPosition, null, beamColor, 0f, origin, scale, SpriteEffects.None, 0f);
                }
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

            return true;
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Energy shield
            if (npc.type == NPCID.CultistBoss || npc.type == NPCID.CultistBossClone)
            {
                spriteBatch.EnterShaderRegion();

                float intensity = newAI[1] / 35f;

                float lifeRatio = npc.type == NPCID.CultistBoss ? (npc.life / (float)npc.lifeMax) : (Main.npc[(int)npc.ai[3]].life / (float)Main.npc[(int)npc.ai[3]].lifeMax);

                float flickerPower = 0f;
                if (lifeRatio < 0.85f)
                    flickerPower += 0.1f;
                if (lifeRatio < 0.7f)
                    flickerPower += 0.1f;
                if (lifeRatio < 0.55f)
                    flickerPower += 0.1f;
                if (lifeRatio < 0.4f)
                    flickerPower += 0.1f;
                if (lifeRatio < 0.25f)
                    flickerPower += 0.1f;
                if (lifeRatio < 0.1f)
                    flickerPower += 0.1f;

                float opacity = 1f;
                opacity *= MathHelper.Lerp(MathHelper.Max(1f - flickerPower, 0.56f), 1f, (float)Math.Pow(Math.Cos(Main.GlobalTimeWrappedHourly * MathHelper.Lerp(3f, 5f, flickerPower)) * 0.5 + 0.5, 24D));

                // Dampen the opacity and intensity slightly, to allow Cultist to be more easily visible inside of the forcefield.
                // Dampen the opacity and intensity a bit more for the Clones.
                float intensityAndOpacityMult = npc.type == NPCID.CultistBossClone ? 0.9f : 1f;
                intensity *= intensityAndOpacityMult;
                opacity *= intensityAndOpacityMult;

                Texture2D forcefieldTexture = Request<Texture2D>("CalamityMod/NPCs/SupremeCalamitas/ForcefieldTexture").Value;

                if (npc.type == NPCID.CultistBoss)
                    GameShaders.Misc["CalamityMod:SupremeShield"].SetShaderTexture(Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/EternityStreak"));
                else
                    GameShaders.Misc["CalamityMod:SupremeShield"].UseImage1("Images/Misc/noise");

                float colorTransitionAmt = (float)Math.Pow((double)(1f - lifeRatio), 2D);
                Color forcefieldColor = Color.Lerp(Color.MediumSpringGreen, Color.Black, colorTransitionAmt);
                Color secondaryForcefieldColor = Color.Lerp(Color.Cyan, Color.Blue, colorTransitionAmt);

                forcefieldColor *= opacity;
                secondaryForcefieldColor *= opacity;

                GameShaders.Misc["CalamityMod:SupremeShield"].UseSecondaryColor(secondaryForcefieldColor);
                GameShaders.Misc["CalamityMod:SupremeShield"].UseColor(forcefieldColor);
                GameShaders.Misc["CalamityMod:SupremeShield"].UseSaturation(intensity);
                GameShaders.Misc["CalamityMod:SupremeShield"].UseOpacity(opacity);
                GameShaders.Misc["CalamityMod:SupremeShield"].Apply();

                // Actual Cultist has a bigger shield than the Clones.
                float shieldScale = npc.type == NPCID.CultistBossClone ? 1.65f : MathHelper.Lerp(1.65f, 3f, (float)Math.Pow((double)lifeRatio, 2D));
                spriteBatch.Draw(forcefieldTexture, npc.Center - Main.screenPosition, null, Color.White * opacity, 0f, forcefieldTexture.Size() * 0.5f, shieldScale, SpriteEffects.None, 0f);

                spriteBatch.ExitShaderRegion();
            }

            if (CalamityWorld.revenge || BossRushEvent.BossRushActive)
            {
                // His afterimages I can't get to work, so fuck it
                if (npc.type == NPCID.SkeletronPrime)
                {
                    Texture2D npcTexture = TextureAssets.Npc[npc.type].Value;
                    int frameHeight = npcTexture.Height / Main.npcFrameCount[npc.type];

                    npc.frame.Y = (int)newAI[3];

                    // Mechdusa drawing
                    if (NPC.IsMechQueenUp)
                    {
                        if (npc.ai[1] == 0f || npc.ai[1] == 4f)
                        {
                            newAI[2] += 1f;
                            if (newAI[2] >= 12f)
                            {
                                newAI[2] = 0f;
                                newAI[3] += frameHeight;
                                if (newAI[3] / frameHeight >= 5)
                                    newAI[3] = frameHeight * 3;
                            }
                        }
                        else
                        {
                            newAI[2] = 0f;
                            newAI[3] = frameHeight * 5;
                        }
                    }

                    // Floating phase
                    else if (npc.ai[1] == 0f || npc.ai[1] == 4f)
                    {
                        newAI[2] += 1f;
                        if (newAI[2] >= 12f)
                        {
                            newAI[2] = 0f;
                            newAI[3] += frameHeight;

                            if (newAI[3] / frameHeight >= 2f)
                                newAI[3] = 0f;
                        }
                    }

                    // Spinning probe spawn or fly over phase
                    else if (npc.ai[1] == 5f || npc.ai[1] == 6f)
                    {
                        newAI[2] = 0f;
                        newAI[3] = frameHeight;
                    }

                    // Spinning phase
                    else
                    {
                        newAI[2] = 0f;
                        newAI[3] = frameHeight * 2;
                    }

                    npc.frame.Y = (int)newAI[3];

                    SpriteEffects spriteEffects = SpriteEffects.None;
                    if (npc.spriteDirection == 1)
                        spriteEffects = SpriteEffects.FlipHorizontally;

                    spriteBatch.Draw(npcTexture, npc.Center - screenPos + new Vector2(0, npc.gfxOffY), npc.frame, npc.GetAlpha(drawColor), npc.rotation, npc.frame.Size() / 2, npc.scale, spriteEffects, 0f);

                    spriteBatch.Draw(TextureAssets.BoneEyes.Value, npc.Center - screenPos + new Vector2(0, npc.gfxOffY),
                        npc.frame, new Color(200, 200, 200, 0), npc.rotation, npc.frame.Size() / 2, npc.scale, spriteEffects, 0f);
                }
                else if (CalamityLists.DestroyerIDs.Contains(npc.type))
                {
                    Texture2D npcTexture = TextureAssets.Npc[npc.type].Value;
                    int frameHeight = npcTexture.Height / Main.npcFrameCount[npc.type];

                    Vector2 halfSize = npc.frame.Size() / 2;
                    SpriteEffects spriteEffects = SpriteEffects.None;
                    if (npc.spriteDirection == 1)
                        spriteEffects = SpriteEffects.FlipHorizontally;

                    if (npc.type == NPCID.TheDestroyerBody)
                    {
                        if (npc.ai[2] == 0f)
                            npc.frame.Y = 0;
                        else
                            npc.frame.Y = frameHeight;
                    }

                    Color segmentDrawColor = npc.GetAlpha(drawColor);

                    // Check if Destroyer is behind tiles and if so, how much of the segment is behind tiles and adjust color accordingly
                    int x = (int)((npc.position.X - 8f) / 16f);
                    int x2 = (int)((npc.position.X + npc.width + 8f) / 16f);
                    int y = (int)((npc.position.Y - 8f) / 16f);
                    int y2 = (int)((npc.position.Y + npc.height + 8f) / 16f);
                    for (int l = x; l <= x2; l++)
                    {
                        for (int m = y; m <= y2; m++)
                        {
                            if (Lighting.Brightness(l, m) == 0f)
                                segmentDrawColor = Color.Black;
                        }
                    }
                    
                    // Draw segments
                    spriteBatch.Draw(npcTexture, npc.Center - screenPos + new Vector2(0, npc.gfxOffY),
                        npc.frame, segmentDrawColor, npc.rotation, halfSize, npc.scale, spriteEffects, 0f);

                    // Draw lights
                    if (npc.ai[2] == 0f && segmentDrawColor != Color.Black)
                    {
                        float destroyerLifeRatio = 1f;
                        if (npc.realLife >= 0)
                            destroyerLifeRatio = Main.npc[npc.realLife].life / (float)Main.npc[npc.realLife].lifeMax;

                        float shootProjectile = (CalamityWorld.death || BossRushEvent.BossRushActive) ? 180 : 300;
                        float timer = npc.ai[0] * 30f;
                        float shootProjectileGateValue = timer + shootProjectile;
                        float glowDuration = 120f;
                        float startGlowingGateValue = shootProjectileGateValue - glowDuration;

                        if ((newAI[3] >= 900f && destroyerLifeRatio < 0.5f) || (newAI[1] < 600f && newAI[1] > 60f))
                        {
                            Color drawColor2 = new Color(50, 50, 255, 0) * (1f - npc.alpha / 255f);
                            for (int i = 0; i < 3; i++)
                            {
                                spriteBatch.Draw(TextureAssets.Dest[npc.type - NPCID.TheDestroyer].Value, npc.Center - screenPos + new Vector2(0, npc.gfxOffY), npc.frame,
                                    drawColor2, npc.rotation, halfSize, npc.scale, spriteEffects, 0f);
                            }

                            // Glow telegraph for lasers
                            /*if (newAI[0] > startGlowingGateValue)
                            {
                                Texture2D bloomTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
                                Color bloom = drawColor2;
                                bloom = Main.hslToRgb((Main.rgbToHsl(bloom).X + 0.02f) % 1, Main.rgbToHsl(bloom).Y, Main.rgbToHsl(bloom).Z);
                                float scalingFactor = (newAI[0] - startGlowingGateValue) / glowDuration;
                                float opacity = (float)Math.Sin(MathHelper.PiOver2 + scalingFactor * MathHelper.PiOver2);
                                float properBloomSize = (float)npc.height / (float)bloomTexture.Height;
                                float scale = scalingFactor;
                                spriteBatch.Draw(bloomTexture, npc.Center - Main.screenPosition, null, bloom * opacity * 0.5f, 0, bloomTexture.Size() / 2f, scale * 1f * properBloomSize, SpriteEffects.None, 0);
                            }*/
                        }
                        else
                        {
                            Color drawColor2 = new Color(255, 255, 255, 0) * (1f - npc.alpha / 255f);
                            spriteBatch.Draw(TextureAssets.Dest[npc.type - NPCID.TheDestroyer].Value, npc.Center - screenPos + new Vector2(0, npc.gfxOffY), npc.frame,
                                drawColor2, npc.rotation, halfSize, npc.scale, spriteEffects, 0f);

                            // Glow telegraph for lasers
                            /*if (newAI[0] > startGlowingGateValue)
                            {
                                Texture2D bloomTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
                                Color bloom = drawColor2;
                                bloom = Main.hslToRgb((Main.rgbToHsl(bloom).X + 0.02f) % 1, Main.rgbToHsl(bloom).Y, Main.rgbToHsl(bloom).Z);
                                float scalingFactor = (newAI[0] - startGlowingGateValue) / glowDuration;
                                float opacity = (float)Math.Sin(MathHelper.PiOver2 + scalingFactor * MathHelper.PiOver2);
                                float properBloomSize = (float)npc.height / (float)bloomTexture.Height;
                                float scale = scalingFactor;
                                spriteBatch.Draw(bloomTexture, npc.Center - Main.screenPosition, null, bloom * opacity * 0.5f, 0, bloomTexture.Size() / 2f, scale * 1f * properBloomSize, SpriteEffects.None, 0);
                            }*/
                        }
                    }
                }
            }
        }

        public override bool? DrawHealthBar(NPC npc, byte hbPosition, ref float scale, ref Vector2 position)
        {
            if (CalamityWorld.death)
            {
                switch (npc.type)
                {
                    case NPCID.DiggerHead:
                    case NPCID.DiggerBody:
                    case NPCID.DiggerTail:
                    case NPCID.SeekerHead:
                    case NPCID.SeekerBody:
                    case NPCID.SeekerTail:
                    case NPCID.DuneSplicerHead:
                    case NPCID.DuneSplicerBody:
                    case NPCID.DuneSplicerTail:
                        return true;
                    default:
                        break;
                }
            }
            return null;
        }

        public static Color buffColor(Color newColor, float R, float G, float B, float A)
        {
            newColor.R = (byte)((float)newColor.R * R);
            newColor.G = (byte)((float)newColor.G * G);
            newColor.B = (byte)((float)newColor.B * B);
            newColor.A = (byte)((float)newColor.A * A);
            return newColor;
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
            {
                return DownedBossSystem.downedDesertScourge;
            }
            else if (type == NPCType<Crabulon.Crabulon>())
            {
                return DownedBossSystem.downedCrabulon;
            }
            else if (type == NPCType<HiveMind.HiveMind>())
            {
                return DownedBossSystem.downedHiveMind;
            }
            else if (type == NPCType<PerforatorHive>())
            {
                return DownedBossSystem.downedPerforator;
            }
            else if (type == NPCType<SlimeGodCore>())
            {
                return DownedBossSystem.downedSlimeGod;
            }
            else if (type == NPCType<Cryogen.Cryogen>())
            {
                return DownedBossSystem.downedCryogen;
            }
            else if (type == NPCType<AquaticScourgeHead>() || type == NPCType<AquaticScourgeBody>() || type == NPCType<AquaticScourgeBodyAlt>() || type == NPCType<AquaticScourgeTail>())
            {
                return DownedBossSystem.downedAquaticScourge;
            }
            else if (type == NPCType<BrimstoneElemental.BrimstoneElemental>())
            {
                return DownedBossSystem.downedBrimstoneElemental;
            }
            else if (type == NPCType<CalamitasClone>())
            {
                return DownedBossSystem.downedCalamitasClone;
            }
            else if (type == NPCType<Leviathan.Leviathan>() || type == NPCType<Anahita>())
            {
                return DownedBossSystem.downedLeviathan;
            }
            else if (type == NPCType<AstrumAureus.AstrumAureus>())
            {
                return DownedBossSystem.downedAstrumAureus;
            }
            else if (type == NPCType<AstrumDeusHead>() || type == NPCType<AstrumDeusBody>() || type == NPCType<AstrumDeusTail>())
            {
                return DownedBossSystem.downedAstrumDeus;
            }
            else if (type == NPCType<PlaguebringerGoliath.PlaguebringerGoliath>())
            {
                return DownedBossSystem.downedPlaguebringer;
            }
            else if (type == NPCType<RavagerBody>())
            {
                return DownedBossSystem.downedRavager;
            }
            else if (type == NPCType<ProfanedGuardianCommander>())
            {
                return DownedBossSystem.downedGuardians;
            }
            else if (type == NPCType<Bumblefuck>())
            {
                return DownedBossSystem.downedDragonfolly;
            }
            else if (type == NPCType<Providence.Providence>())
            {
                return DownedBossSystem.downedProvidence;
            }
            else if (type == NPCType<CeaselessVoid.CeaselessVoid>() || type == NPCType<DarkEnergy>())
            {
                return DownedBossSystem.downedCeaselessVoid;
            }
            else if (type == NPCType<StormWeaverHead>() || type == NPCType<StormWeaverBody>() || type == NPCType<StormWeaverTail>())
            {
                return DownedBossSystem.downedStormWeaver;
            }
            else if (type == NPCType<Signus.Signus>())
            {
                return DownedBossSystem.downedSignus;
            }
            else if (type == NPCType<Polterghast.Polterghast>())
            {
                return DownedBossSystem.downedPolterghast;
            }
            else if (type == NPCType<OldDuke.OldDuke>())
            {
                return DownedBossSystem.downedBoomerDuke;
            }
            else if (type == NPCType<DevourerofGodsHead>() || type == NPCType<DevourerofGodsBody>() || type == NPCType<DevourerofGodsTail>())
            {
                return DownedBossSystem.downedDoG;
            }
            else if (type == NPCType<Yharon.Yharon>())
            {
                return DownedBossSystem.downedYharon;
            }
            else if (type == NPCType<Artemis>() || type == NPCType<Apollo>() || type == NPCType<AresBody>() || type == NPCType<AresGaussNuke>() || type == NPCType<AresLaserCannon>() || type == NPCType<AresPlasmaFlamethrower>() || type == NPCType<AresTeslaCannon>() || type == NPCType<ThanatosHead>() || type == NPCType<ThanatosBody1>() || type == NPCType<ThanatosBody2>() || type == NPCType<ThanatosTail>())
            {
                return DownedBossSystem.downedExoMechs;
            }
            else if (type == NPCType<SupremeCalamitas.SupremeCalamitas>())
            {
                return DownedBossSystem.downedCalamitas;
            }
            else if (type == NPCType<PrimordialWyrmHead>())
            {
                return DownedBossSystem.downedPrimordialWyrm;
            }

            return true;
        }
        #endregion

        #region Speedrun Display
        public static void SetNewBossJustDowned(NPC npc)
        {
            if (!GetDownedBossVariable(npc.type))
            {
                CalamityLists.bossTypes.TryGetValue(npc.type, out int newBossTypeJustDowned);

                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (!player.active)
                        continue;

                    CalamityPlayer mp = player.Calamity();
                    mp.lastSplitType = newBossTypeJustDowned;
                    mp.lastSplit = mp.previousSessionTotal.Add(CalamityMod.SpeedrunTimer.Elapsed);
                }
            }
        }
        #endregion

        #region Player Counts
        public static bool AnyLivingPlayers()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i] != null && Main.player[i].active && !Main.player[i].dead && !Main.player[i].ghost)
                {
                    return true;
                }
            }
            return false;
        }

        public static int GetActivePlayerCount()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                return 1;
            }

            int players = 0;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i] != null && Main.player[i].active)
                {
                    players++;
                }
            }
            return players;
        }
        #endregion

        #region Should Affect NPC
        public static bool ShouldAffectNPC(NPC target)
        {
            if (CalamityLists.EaterofWorldsIDs.Contains(target.type) || CalamityLists.DestroyerIDs.Contains(target.type))
                return false;

            if (target.damage > 0 && !target.boss && !target.friendly && !target.dontTakeDamage && target.type != NPCID.Creeper && target.type != NPCType<RavagerClawLeft>() &&
                target.type != NPCID.MourningWood && target.type != NPCID.Everscream && target.type != NPCID.SantaNK1 && target.type != NPCType<RavagerClawRight>() &&
                target.type != NPCType<ReaperShark>() && target.type != NPCType<Mauler>() && target.type != NPCType<EidolonWyrmHead>() && target.type != NPCID.GolemFistLeft && target.type != NPCID.GolemFistRight &&
                target.type != NPCType<PrimordialWyrmHead>() && target.type != NPCType<ColossalSquid>() && target.type != NPCID.DD2Betsy && !CalamityLists.enemyImmunityList.Contains(target.type) && !AcidRainEvent.AllMinibosses.Contains(target.type))
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
                                    player.inventory[item].SetDefaults(0, false);
                                }
                                break;
                            }
                        }

                        projectile.ai[0] = 2f;
                        projectile.netUpdate = true;

                        // The vanilla game uses a special packet for Duke Fishron spawning.
                        // However, this packet doesn't work on modded NPC types, so we must create
                        // a custom one.
                        // Also, you can't use Netmode != NetmodeID.MultiplayerClient in a projectile context that
                        // has an owner, hence the MyPlayer check.
                        if (Main.myPlayer == projectile.owner)
                        {
                            if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                if (!player.active || player.dead)
                                    return;

                                Projectile proj = null;
                                for (int i = 0; i < Main.maxProjectiles; i++)
                                {
                                    proj = Main.projectile[i];
                                    if (Main.projectile[i].active && Main.projectile[i].bobber && Main.projectile[i].owner == player.whoAmI)
                                    {
                                        proj = Main.projectile[i];
                                        break;
                                    }
                                }

                                if (proj is null)
                                    return;

                                int oldDuke = NPC.NewNPC(NPC.GetBossSpawnSource(player.whoAmI), (int)proj.Center.X, (int)proj.Center.Y + 100, NPCType<OldDuke.OldDuke>());
                                CalamityUtils.BossAwakenMessage(oldDuke);
                            }
                            else
                            {
                                var netMessage = CalamityMod.Instance.GetPacket();
                                netMessage.Write((byte)CalamityModMessageType.ServersideSpawnOldDuke);
                                netMessage.Write((byte)player.whoAmI);
                                netMessage.Send();
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

        #region Astral things
        public static void DoHitDust(NPC npc, int hitDirection, int dustType = 5, float xSpeedMult = 1f, int numHitDust = 5, int numDeathDust = 20)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, dustType, hitDirection * xSpeedMult, -1f);
            }

            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, dustType, hitDirection * xSpeedMult, -1f);
                }
            }
        }

        public static void DoFlyingAI(NPC npc, float maxSpeed, float acceleration, float circleTime, float minDistanceTarget = 150f, bool shouldAttackTarget = true)
        {
            //Pick a new target.
            if (npc.target < 0 || npc.target >= Main.maxPlayers || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
            }

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

            //Circular motion
            npc.ai[0]++;

            //y motion
            if (npc.ai[0] > circleTime * 0.5f)
            {
                npc.velocity.Y += acceleration;
            }
            else
            {
                npc.velocity.Y -= acceleration;
            }

            //x motion
            if (npc.ai[0] < circleTime * 0.25f || npc.ai[0] > circleTime * 0.75f)
            {
                npc.velocity.X += acceleration;
            }
            else
            {
                npc.velocity.X -= acceleration;
            }

            //reset
            if (npc.ai[0] > circleTime)
            {
                npc.ai[0] = 0f;
            }

            //if close enough
            if (shouldAttackTarget && distanceToTarget < minDistanceTarget)
            {
                npc.velocity += maxVelocity * 0.007f;
            }

            if (myTarget.dead)
            {
                maxVelocity.X = npc.direction * maxSpeed / 2f;
                maxVelocity.Y = -maxSpeed / 2f;
            }

            //maximise velocity
            if (npc.velocity.X < maxVelocity.X)
            {
                npc.velocity.X += acceleration;
            }

            if (npc.velocity.X > maxVelocity.X)
            {
                npc.velocity.X -= acceleration;
            }

            if (npc.velocity.Y < maxVelocity.Y)
            {
                npc.velocity.Y += acceleration;
            }

            if (npc.velocity.Y > maxVelocity.Y)
            {
                npc.velocity.Y -= acceleration;
            }

            //rotate towards player if alive
            if (!myTarget.dead)
            {
                npc.rotation = toTarget.ToRotation();
            }
            else //don't, do velocity instead
            {
                npc.rotation = npc.velocity.ToRotation();
            }

            npc.rotation += MathHelper.Pi;

            //tile collision
            float collisionDamp = 0.7f;
            if (npc.collideX)
            {
                npc.netUpdate = true;
                npc.velocity.X = npc.oldVelocity.X * -collisionDamp;

                if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                {
                    npc.velocity.X = 2f;
                }

                if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                {
                    npc.velocity.X = -2f;
                }
            }
            if (npc.collideY)
            {
                npc.netUpdate = true;
                npc.velocity.Y = npc.oldVelocity.Y * -collisionDamp;

                if (npc.velocity.Y > 0f && npc.velocity.Y < 1.5f)
                {
                    npc.velocity.Y = 1.5f;
                }

                if (npc.velocity.Y < 0f && npc.velocity.Y > -1.5f)
                {
                    npc.velocity.Y = -1.5f;
                }
            }

            //water collision
            if (npc.wet)
            {
                if (npc.velocity.Y > 0f)
                {
                    npc.velocity.Y *= 0.95f;
                }

                npc.velocity.Y -= 0.3f;

                if (npc.velocity.Y < -2f)
                {
                    npc.velocity.Y = -2f;
                }
            }

            //Taken from source. Important for net?
            if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
            {
                npc.netUpdate = true;
            }
        }

        public static void DoSpiderWallAI(NPC npc, int transformType, float chaseMaxSpeed = 2f, float chaseAcceleration = 0.08f)
        {
            //GET NEW TARGET
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead)
            {
                npc.TargetClosest();
            }

            Vector2 between = Main.player[npc.target].Center - npc.Center;
            float distance = between.Length();

            //modify vector depending on distance and speed.
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

            //update if target dead.
            if (Main.player[npc.target].dead)
            {
                between.X = npc.direction * chaseMaxSpeed / 2f;
                between.Y = -chaseMaxSpeed / 2f;
            }
            npc.spriteDirection = -1;

            //If spider can't see target, circle around to attempt to find the target.
            if (!Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
            {
                //CIRCULAR MOTION, SIMILAR TO FLYING AI (Eater of Souls etc.)
                npc.ai[0]++;

                if (npc.ai[0] > 0f)
                {
                    npc.velocity.Y += 0.023f;
                }
                else
                {
                    npc.velocity.Y -= 0.023f;
                }

                if (npc.ai[0] < -100f || npc.ai[0] > 100f)
                {
                    npc.velocity.X += 0.023f;
                }
                else
                {
                    npc.velocity.X -= 0.023f;
                }

                if (npc.ai[0] > 200f)
                {
                    npc.ai[0] = -200f;
                }

                npc.velocity.X += between.X * 0.007f;
                npc.velocity.Y += between.Y * 0.007f;
                npc.rotation = npc.velocity.ToRotation();

                if (npc.velocity.X > 1.5f)
                {
                    npc.velocity.X *= 0.9f;
                }

                if (npc.velocity.X < -1.5f)
                {
                    npc.velocity.X *= 0.9f;
                }

                if (npc.velocity.Y > 1.5f)
                {
                    npc.velocity.Y *= 0.9f;
                }

                if (npc.velocity.Y < -1.5f)
                {
                    npc.velocity.Y *= 0.9f;
                }

                npc.velocity.X = MathHelper.Clamp(npc.velocity.X, -3f, 3f);
                npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y, -3f, 3f);
            }
            else //CHASE TARGET
            {
                if (npc.velocity.X < between.X)
                {
                    npc.velocity.X += chaseAcceleration;

                    if (npc.velocity.X < 0f && between.X > 0f)
                    {
                        npc.velocity.X += chaseAcceleration;
                    }
                }
                else if (npc.velocity.X > between.X)
                {
                    npc.velocity.X -= chaseAcceleration;

                    if (npc.velocity.X > 0f && between.X < 0f)
                    {
                        npc.velocity.X -= chaseAcceleration;
                    }
                }
                if (npc.velocity.Y < between.Y)
                {
                    npc.velocity.Y += chaseAcceleration;

                    if (npc.velocity.Y < 0f && between.Y > 0f)
                    {
                        npc.velocity.Y += chaseAcceleration;
                    }
                }
                else if (npc.velocity.Y > between.Y)
                {
                    npc.velocity.Y -= chaseAcceleration;

                    if (npc.velocity.Y > 0f && between.Y < 0f)
                    {
                        npc.velocity.Y -= chaseAcceleration;
                    }
                }
                npc.rotation = between.ToRotation();
            }

            //DAMP COLLISIONS OFF OF WALLS
            float collisionDamp = 0.5f;
            if (npc.collideX)
            {
                npc.netUpdate = true;
                npc.velocity.X = npc.oldVelocity.X * -collisionDamp;

                if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                {
                    npc.velocity.X = 2f;
                }

                if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                {
                    npc.velocity.X = -2f;
                }
            }
            if (npc.collideY)
            {
                npc.netUpdate = true;
                npc.velocity.Y = npc.oldVelocity.Y * -collisionDamp;

                if (npc.velocity.Y > 0f && npc.velocity.Y < 1.5f)
                {
                    npc.velocity.Y = 2f;
                }

                if (npc.velocity.Y < 0f && npc.velocity.Y > -1.5f)
                {
                    npc.velocity.Y = -2f;
                }
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
                        if (Main.tile[i, j].WallType > 0)
                        {
                            flag = true;
                        }
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
                    {
                        npc.velocity.X = 2f;
                    }

                    if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                    {
                        npc.velocity.X = -2f;
                    }
                }

                if (npc.collideY)
                {
                    npc.velocity.Y = npc.oldVelocity.Y * -0.5f;

                    if (npc.velocity.Y > 0f && npc.velocity.Y < 1f)
                    {
                        npc.velocity.Y = 1f;
                    }

                    if (npc.velocity.Y < 0f && npc.velocity.Y > -1f)
                    {
                        npc.velocity.Y = -1f;
                    }
                }

                if (npc.direction == -1 && npc.velocity.X > -maxSpeed)
                {
                    npc.velocity.X -= acceleration;

                    if (npc.velocity.X > maxSpeed)
                    {
                        npc.velocity.X -= acceleration;
                    }
                    else if (npc.velocity.X > 0f)
                    {
                        npc.velocity.X -= acceleration * 0.5f;
                    }

                    if (npc.velocity.X < -maxSpeed)
                    {
                        npc.velocity.X = -maxSpeed;
                    }
                }
                else if (npc.direction == 1 && npc.velocity.X < maxSpeed)
                {
                    npc.velocity.X += acceleration;

                    if (npc.velocity.X < -maxSpeed)
                    {
                        npc.velocity.X += acceleration;
                    }
                    else if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X += acceleration * 0.5f;
                    }

                    if (npc.velocity.X > maxSpeed)
                    {
                        npc.velocity.X = maxSpeed;
                    }
                }

                float xDistance = Math.Abs(npc.Center.X - Main.player[npc.target].Center.X);
                float yLimiter = Main.player[npc.target].position.Y - (npc.height / 2f);
                if (xDistance > 50f)
                {
                    yLimiter -= 100f;
                }

                if (npc.position.Y < yLimiter)
                {
                    npc.velocity.Y += acceleration * 0.5f;

                    if (npc.velocity.Y < 0f)
                    {
                        npc.velocity.Y += acceleration * 0.1f;
                    }
                }
                else
                {
                    npc.velocity.Y -= acceleration * 0.5f;

                    if (npc.velocity.Y > 0f)
                    {
                        npc.velocity.Y -= acceleration * 0.1f;
                    }
                }

                if (npc.velocity.Y < -maxSpeed)
                {
                    npc.velocity.Y = -maxSpeed;
                }

                if (npc.velocity.Y > maxSpeed)
                {
                    npc.velocity.Y = maxSpeed;
                }
            }
            //Change velocity if wet.
            if (npc.wet)
            {
                if (npc.velocity.Y > 0f)
                {
                    npc.velocity.Y *= 0.95f;
                }

                npc.velocity.Y -= 0.5f;

                if (npc.velocity.Y < -4f)
                {
                    npc.velocity.Y = -4f;
                }
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

            //"flip" the rectangle's position x-wise.
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
    }
}
