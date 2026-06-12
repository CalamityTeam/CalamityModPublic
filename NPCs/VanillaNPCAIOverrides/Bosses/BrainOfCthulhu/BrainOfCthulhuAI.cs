using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.DataStructures;
using CalamityMod.Enums;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss.BrainOfCthulhu;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.World;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses.BrainOfCthulhu;

public class BrainOfCthulhuAI : VanillaAIOverride
{
    private static SoundStyle StunnedHit = new SoundStyle("CalamityMod/Sounds/Custom/BrainOfCthulhu/BoC_Rev_Stun_Hit", 3);
    private static SoundStyle ShieldDown = new SoundStyle("CalamityMod/Sounds/Custom/BrainOfCthulhu/BoC_Rev_Shield_Down") with { PauseBehavior = PauseBehavior.PauseWithGame };
    private static SoundStyle ShieldUp = new SoundStyle("CalamityMod/Sounds/Custom/BrainOfCthulhu/BoC_Rev_Shield_Up") with { PauseBehavior = PauseBehavior.PauseWithGame };
    private static SoundStyle IntroRoar = new SoundStyle("CalamityMod/Sounds/Custom/BrainOfCthulhu/BoC_Rev_Roar") with { PauseBehavior = PauseBehavior.PauseWithGame };
    private static SoundStyle Roar = new SoundStyle("CalamityMod/Sounds/Custom/BrainOfCthulhu/BoC_Rev_Short_Roar") with { PauseBehavior =  PauseBehavior.PauseWithGame};
    public static SoundStyle Laugh = new SoundStyle("CalamityMod/Sounds/Custom/BrainOfCthulhu/BoC_Rev_Laugh") with { PauseBehavior = PauseBehavior.PauseWithGame, MaxInstances = 5 };
    private static SoundStyle Growl = new SoundStyle("CalamityMod/Sounds/Custom/BrainOfCthulhu/BoC_Rev_Growl", 2) with { PauseBehavior = PauseBehavior.PauseWithGame };
    private static SoundStyle Death = new SoundStyle("CalamityMod/Sounds/Custom/BrainOfCthulhu/BoC_Rev_Death_Roar") with { PauseBehavior = PauseBehavior.PauseWithGame };
    private static SoundStyle BloodShot = new SoundStyle("CalamityMod/Sounds/Custom/BrainOfCthulhu/BoC_Rev_BloodShot");
    private static SoundStyle BloodBomb = new SoundStyle("CalamityMod/Sounds/Custom/BrainOfCthulhu/BoC_Rev_BloodBomb");
    private static SoundStyle BloodExplosion = new SoundStyle("CalamityMod/Sounds/Custom/BrainOfCthulhu/BoC_Rev_Explosion", 2);


    internal static bool SummonedViaItem = false;
    internal List<Particle> BoCAfterImages = [];
    internal float ShieldOpacity = 1f;
    internal float ShieldScale = 1f;
    private Vector2 BoCDrawOffset = Vector2.Zero;
    private Rectangle BoCFrame = new(0, 0, 198, 180);

    #region Balancing Values

    internal static int BrainIllusionDamage => 15;

    #region Projectile Damage Values
    internal static int BloodShotDamage => 12; // 48
    internal static int BloodScytheDamage => 12; // 48
    internal static int IchorShotDamage => 12; // 48
    internal static int CrimsonEyeDamage => 12; // 48
    #endregion

    internal static float Phase1DefenseMultiplier => 1.5f; //Multiplies BoC's default defense value by this amount when in Phase 1.

    #region Health Gates
    internal static float DesperateOnslaughtCreeperHealthGate => 0.1f; //When the cumulative health % of all creepers falls below this value, BoC will begin its pre-stun attack upon entering its idle phase.
    internal static float Phase2HealthGate => 0.5f; //When BoC's health % falls below this value, it will begin entering Phase 2
    #endregion

    internal static float DespawnRangeSQ => 36000000f;
    internal static float DespawnRange => 6000f;

    #region Phase 1 Attack Values

    #region Idle Period
    internal static int IdlePeriodDuration => 180;
    internal static int CreeperChargeDelayMin => 70;
    internal static int CreeperChargeDelayMax => 100;
    internal static int CreeperChargePositioningTime => 60;
    internal static int CreeperChargeWindUpTime => 22;
    #endregion

    internal static int StunDuration => 480;

    #region Creeper Swipes
    internal static int SwipesStartupDuration => 120;
    internal int SwipeDuration => 60 + SwipeDelay;
    internal int SwipeDelay => AttackFlag ? 30 : CalamityWorld.death ? 40 : 50;
    internal static int SwipeAmount => 4;
    internal int SwipeIchorDelay => 30 + SwipeDelay;
    #endregion

    #region Creeper Crush        
    internal static int LightSwipeDelay => 60;
    internal static int LightSwipeAmount => CalamityWorld.death ? 8 : 6;
    internal static int LightSwipeTravelTime => 30;
    internal static int LightSwipeAttackDelay => 10;
    internal static int LightSwipeDuration => CalamityWorld.death ? 45 : 60;
    internal static int StrongSwipeDelay => 90;
    internal static int StrongSwipeAmount => CalamityWorld.death ? 7 : 5;
    internal static int StrongSwipeTravelTime => 45;
    internal static int StrongSwipeAttackDelay => 15;
    internal static int StrongSwipeDuration => CalamityWorld.death ? 60 : 80;
    #endregion

    #region Creeper Orbit
    internal static int OrbitSetupDuration => 60;
    internal static int OrbitDuration => 720;
    internal static int OrbitAttackInterval => 120;
    internal static int OrbitAttackParticipantCount => CalamityWorld.death ? 4 : 3;
    internal static float OrbitStandardRadius => 320f;
    internal static float OrbitTelegraphRadius => 480f;
    internal static float BaseRotationSpeed => 0.0175f;
    #endregion

    #region Creeper Spiral
    internal static int SpiralDuration => 720;
    internal static int SpiralSetupTime => 90;
    internal static int TendrilCount => 3;
    internal static float TendrilLength => 512;
    internal static float TendrilStartDistance => 64;
    internal static float MaxCreeperSway => 64;
    internal static int StartingTimePerRevolutionMax => CalamityWorld.death ? 270 : 300;
    internal static int StartingTimePerRevolutionMin => CalamityWorld.death ? 180 : 210;
    internal static int EndingTimePerRevolutionMax => CalamityWorld.death ? 210 : 240;
    internal static int EndingTimePerRevolutionMin => CalamityWorld.death ? 120 : 150;
    internal static int SpeedUpDelayTime => 120;
    internal static int SpeedUpExtensionTime => 120;
    internal static float TurnAroundRatio => 0.6f; //In the second creeper phase, the creeper spiral will turn around at this completion percentage of the attack;
    internal static float TurnAroundDurationRatio => CalamityWorld.death ? 0.1f : 0.125f; //The amount of time that it'll take for the creeper spiral to turn around

    #endregion

    #endregion

    #region Phase 2 Attack Values

    internal static float DefaultTeleportDistance => 360f;

    #region Idle Period
    internal static int ChaseTime => 160;
    internal static int ChaseAmount => 2;
    internal static int IdleTeleportDuration => CalamityWorld.death ? 36 : 44;
    internal static float ChaseMinSpeed => 3;
    internal static float ChaseMaxSpeed => CalamityWorld.death ? 18 : 15;


    #endregion

    #region Bloodletting
    internal static int BloodlettingDuration => 675;
    internal static Vector2 HoverDistance => new (420f, 300f);
    internal static float HoverEndHeight => 300f;
    internal static int IchorRate => CalamityWorld.death ? 10 : 12;
    internal static float IchorSpread => 1.5f;
    internal static float IchorVelocity => 3f;
    internal static int BloodshotRate => 90;
    internal static float BloodshotVelocity => 10f;
    internal static int DashPrepTime => 90;
    internal static int DashReelbackTime => 20;
    internal static int DashDuration => 30;
    internal static float DashVelocity => 32f;
    internal static int DashScytheRate => CalamityWorld.death ? 5 : 6;
    #endregion

    #region Sanguine Scythes
    internal static int SanguineTeleportCount => 5;
    internal static int SanguineScytheCount => CalamityWorld.death ? 12 : 10;
    internal static int SanguineTeleportDuration => 30;
    internal static float SanguineTeleportDistance => 440f;
    internal static Vector2 SanguineFinalTeleportOffset => new(720, 300);
    internal static int SanguineAttackEndDelay => 30;
    internal static int SanguineAttackEndDuration => 100;
    internal static int SanguineAttackEndIchorRate => 10;

    #endregion

    #region Crimson Eyes
    internal static int CrimsonEyeAttackIdleDuration => 210;
    internal static int CrimsonEyeAttackSetUpDuration => 30;
    internal static int CrimsonEyeAttackBuildUpDuration => 120;

    internal static int CrimsonEyeRate => 60;
    internal static int CrimsonEyeCap => 40;

    internal static int CrimsonEyeAttackDuration => 960;
    internal static int CrimsonEyeAttackEndDuration => 210;
    internal static float TurnAccelerationMultiplier => 0.01f; //Base multiplier for the boss' course correction rotation
    internal static float TurnAccelerationDistanceBuffer => 160f; //Determines the minimum distance it must be at before it can begin redirecting.
    internal static float TurnAccelerationDistanceDivisor => 72f; //Determines how much the distance affects its turn amount. Larger number means it must get further from the player in order to correct its course
    #endregion

    #region Illusion Dash
    internal static float IllusionDashTeleportDistance => 300f;
    internal static int IllusionDashTeleportDuration => 30;
    internal static float IllusionDashCloseInDistance => 280f;
    internal static float IllusionDashStartingSpinSpeed => 0.125f;
    internal static int IllusionDashSpinDuration => 100;
    internal static int IllusionDashFakeoutTeleportDuration => 16;
    internal static float IllusionDashVelocity => 30f;

    #endregion

    #region Illusion Trick
    internal static int IllusionTrickAngleGroups => CalamityWorld.death ? 8 : 6;
    internal static int IllusionTrickGroupSize => CalamityWorld.death ? 5 : 4;
    internal static int IllusionTrickStunDuration => 120;
    internal static int IllusionTrickTimeLimit => 960;

    #endregion

    #endregion

    #endregion

    internal enum BrainAIState : byte
    {
        //Spawn Animation
        UndergroundSpawnAnimation,
        SurfaceSpawnAnimation,
        //Phase 1
        Phase1Idle,
        CreeperSwipes,
        CreeperSwings,
        CreeperOrbit,
        CreeperSpiral,
        TelekineticOnslaught,
        Stunned,
        //Phase Transition
        Phase2TransitionClosed,
        Phase2TransitionOpen,
        //Phase 2
        Phase2Idle,
        CrimsonEyes,
        SanguineScythes,
        Bloodletting,
        IllusionDash,
        IllusionTrick,
        //Defeat
        DeathAnimation
    }

    internal BrainAIState AIState { get => (BrainAIState)NPC.ai[0]; set => NPC.ai[0] = (float)value; }
    internal BrainAIState PreviousAttack = BrainAIState.Phase1Idle;
    internal ref float Time => ref NPC.ai[1];
    internal ref float DespawnTime => ref NPC.ai[2];
    internal ref float CachedRatio => ref NPC.ai[3];
    internal float TeleportTime = 0;
    internal float TeleportDuration = 0;
    internal float SpawnTime = 0;
    internal int SpawnDelay = 0;
    internal bool OnSecondCreeperPhase = false;

    private bool isNegative = false;
    internal int AttackSign { get => isNegative ? -1 : 1; set => isNegative = value == -1; }
    internal float AttackRotation = 0;
    internal float AttackTime = 0;
    internal int AttackCounter = 0;
    internal bool AttackFlag = false;
    internal Vector2 AttackPosition = Vector2.Zero;
    internal List<BrainAIState> availableAttacks = [];
    internal List<byte> AttackList = [];
    internal HashSet<int> TargetsSet = [];

    private Player Target => Main.player[NPC.target];

    private static float CreeperHPRatio { get
        {
            float ratio = 0f;
            foreach (NPC creeper in Main.npc.Where(n => n.active && n.type == NPCID.Creeper))
                ratio += creeper.life / (float)creeper.lifeMax;
            if (ratio != 0f)
                ratio /= GetBrainOfCthuluCreepersCountRevDeath();
            return ratio;
        }
    }

    private static float CreeperAmountRatio => NPC.CountNPCS(NPCID.Creeper) / (float)GetBrainOfCthuluCreepersCountRevDeath();

    public override bool EnableMultiplayerSmoothingAheadOfAI => true;

    public override void SetDefaults(Mod mod)
    {
        NPC.damage = NPC.defDamage = 36; // 64 (1.8x expert scaling)
        BoCDrawOffset = Vector2.Zero;
        ShieldOpacity = 1f;
        ShieldScale = 1f;
        BrainOfCthulhuSystem.ScreenBlurStrength = 0f;

        if (!Main.dedServ)
        {
            int brainOfCthuluCreepersCount = GetBrainOfCthuluCreepersCountRevDeath();
            BrainOfCthulhuSystem.VerletTendrils = new (int creeper, List<VerletSimulatedSegment> tendril, int reelInTimer)[brainOfCthuluCreepersCount];

            for (int i = 0; i < brainOfCthuluCreepersCount; i++)
            {
                List<VerletSimulatedSegment> tendril = [];
                for (int j = 0; j < 28; j++)
                    tendril.Add(new(NPC.Center));

                BrainOfCthulhuSystem.VerletTendrils[i].tendril = tendril;
            }
        }
    }

    public override void OnSpawn(Mod mod)
    {
        if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
        {
            // Ignore tank players, target low HP players, Brain is smart
            CalamityTargetingParameters options = CalamityTargetingParameters.BossDefaults;
            options.aggroRatio = -1f;
            options.finishThemOff = true;
            CalamityUtils.CalamityTargeting(NPC, options);
        }
        Player target = Main.player[NPC.target];
        bool onSurface = target.Center.Y / 16 < Main.worldSurface;

        NPC.Center = target.Center + Vector2.UnitY * (onSurface ? 900 : -900);
        DisableMultiplayerSmoothing = true;
        NPC.dontTakeDamage = true;

        AIState = onSurface ? BrainAIState.SurfaceSpawnAnimation : BrainAIState.UndergroundSpawnAnimation;
        PreviousAttack = BrainAIState.Phase1Idle;
        SpawnDelay = (SummonedViaItem || BossRushEvent.BossRushActive) ? 2 : 60;
        if (SummonedViaItem || BossRushEvent.BossRushActive)
            SpawnTime = -1;

        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            int brainOfCthuluCreepersCount = GetBrainOfCthuluCreepersCountRevDeath();
            for (int i = 0; i < brainOfCthuluCreepersCount; i++)
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.Creeper, NPC.whoAmI, i, ai2: -1);
        }

        NPC.netUpdate = true;
    }

    public override bool AI(Mod mod)
    {
        // whoAmI variable
        NPC.crimsonBoss = NPC.whoAmI;
        bool phase2 = AIState >= BrainAIState.Phase2TransitionClosed;

        //Takes more damage in Phase 1 to account for invulnerability phases
        if (phase2)
        {
            NPC.knockBackResist = 0f;
            NPC.defense = NPC.defDefense;

            NPC.chaseable = (AIState != BrainAIState.IllusionDash && AIState != BrainAIState.IllusionTrick);
        }
        else
            NPC.defense = (int)(NPC.defDefense * Phase1DefenseMultiplier);

        NPC.extraValue = 0;

        #region Targeting
        if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
        {
            // Ignore tank players, target low HP players, Brain is smart
            CalamityTargetingParameters options = CalamityTargetingParameters.BossDefaults;
            options.aggroRatio = -1f;
            options.finishThemOff = true;
            options.maxSearchRange = DespawnRange;
            CalamityUtils.CalamityTargeting(NPC, options);
        }
        #endregion

        #region Despawn
        // Despawn check
        if (!BossRushEvent.BossRushActive && AIState != BrainAIState.DeathAnimation)
        {
            bool despawn = (Target.dead || !Target.ZoneCrimson);
            if (despawn)
            {
                var v = GetAllValidTargets(NPC.Center);
                if (v.Count > 0)
                {
                    despawn = false;
                    NPC.target = v[0];
                }
            }

            // Despawn
            if (despawn)
            {
                if (DespawnTime < 90)
                    DespawnTime += 1f;

                if (DespawnTime == 90)
                    NPC.velocity.Y += 0.1f;
            }
            else if (DespawnTime > 0f)
                DespawnTime -= 1f;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Target.DistanceSQ(NPC.Center) > DespawnRangeSQ)
                {
                    NPC.active = false;
                    NPC.life = 0;

                    if (Main.dedServ)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                }
            }

            if (DespawnTime > 60)
                return false;
        }
        #endregion

        #region Hit Sounds
        if (AIState == BrainAIState.Stunned)
            NPC.HitSound = StunnedHit;
        else
            NPC.HitSound = SoundID.NPCHit9;
        #endregion

        #region Forced State Changes
        if (!phase2 && CreeperHPRatio == 0f && AIState != BrainAIState.Stunned && AIState >= BrainAIState.Phase1Idle)
        {
            AIState = BrainAIState.Stunned;
            Time = 0;
            ResetAttackValues();
            foreach(Projectile p in Main.ActiveProjectiles)
            {
                if (p.type != ModContent.ProjectileType<TelekineticEnemyGrab>())
                    continue;

                p.ai[1] = 0;
            }
        }

        if (AIState == BrainAIState.Stunned && (NPC.life / (float)NPC.lifeMax) < Phase2HealthGate)
        {
            AIState = BrainAIState.Phase2TransitionClosed;
            Time = 0;
            TeleportTime = 0;
        }
        #endregion

        switch (AIState)
        {
            #region Spawn Animations
            case BrainAIState.UndergroundSpawnAnimation:
            case BrainAIState.SurfaceSpawnAnimation:
                SpawnAnimation();
                break;
            #endregion
            
            #region Phase 1
            case BrainAIState.Phase1Idle:
                Phase1Idle();
                break;
            case BrainAIState.TelekineticOnslaught:
                TelekineticOnslaught();
                break;
            case BrainAIState.Stunned:
                Stunned();
                break;
            case BrainAIState.CreeperSwipes:
                CreeperSwipes();
                break;
            case BrainAIState.CreeperSwings:
                CreeperSwings();
                break;
            case BrainAIState.CreeperOrbit:
                CreeperOrbit();
                break;
            case BrainAIState.CreeperSpiral:
                CreeperSpiral();
                break;
            #endregion

            #region Phase Transition
            case BrainAIState.Phase2TransitionClosed:
            case BrainAIState.Phase2TransitionOpen:
                PhaseTransition();
                break;
            #endregion

            #region Phase 2
            case BrainAIState.Phase2Idle:
                Phase2Idle();
                break;
            case BrainAIState.Bloodletting:
                Bloodletting();
                break;
            case BrainAIState.SanguineScythes:
                SanguineScythes();
                break;
            case BrainAIState.CrimsonEyes:
                CrimsonEyes();
                break;
            case BrainAIState.IllusionDash:
                IllusionDash();
                break;
            case BrainAIState.IllusionTrick:
                IllusionTrick();
                break;
            #endregion

            case BrainAIState.DeathAnimation:
                DeathAnimation();
                break;
        }

        #region Projectile Altering
        foreach (Projectile p in Main.ActiveProjectiles)
        {
            if (p.type != ProjectileID.BloodNautilusShot || p.ai[0] == 0)
                continue;

            int startUpTime = 20;
            float speedUpTime = 30;
            float slowDownMult = 0.96f;
            float speedUpMult = 1.025f;
            if(AIState == BrainAIState.IllusionDash)
            {
                startUpTime = 20;
                speedUpTime = 30;
                slowDownMult = 0.96f;
                speedUpMult = 1.025f;
            }

            if (p.ai[2] <= startUpTime)
                p.velocity *= slowDownMult;
            else
            {
                p.velocity *= speedUpMult;
                if (p.ai[2] <= startUpTime + speedUpTime)
                {
                    float newAngle = p.ai[1].AngleLerp(p.ai[0] - MathHelper.TwoPi, (p.ai[2] - startUpTime) / speedUpTime);

                    p.velocity = newAngle.ToRotationVector2() * p.velocity.Length();
                }
            }
            p.ai[2]++;
        }
        #endregion

        NPC.oldVelocity = NPC.velocity;
        Time++;

        if(AIState != BrainAIState.DeathAnimation && NPC.lifeRegen < 0 && Math.Abs(NPC.lifeRegen) >= NPC.life)
            TriggerDeathAnimation();

        return false;
    }

    #region Attacks + Animations
    
    private void SpawnAnimation()
    {
        NPC.damage = 0;

        foreach (Player p in Main.ActivePlayers)
            p.Calamity().adrenaline = 0;

        if (SpawnTime != 0) //BoC should begin appearing
        {
            float d = Main.LocalPlayer.DistanceSQ(NPC.Center);
            float distanceScaleFactor = 1;
            if (d > 592900) //770^2
                distanceScaleFactor = 1 / (1 + (((float)Math.Sqrt(d) - 770) / 32f));

            // Time spent in the spawn animation is shortened when summoned via item
            float spawnAnimTimeReduction = (SummonedViaItem || BossRushEvent.BossRushActive) ? 120 : 0;
            float spawnCounter = Time - Math.Abs(SpawnTime) + spawnAnimTimeReduction;

            if (spawnCounter < 180)
            {
                float shakeIntensity = CalamityUtils.CircOutEasing(spawnCounter / 180f, 1) * 3f * distanceScaleFactor;
                Main.LocalPlayer.SetScreenshake(shakeIntensity);
                for (int i = 0; i < shakeIntensity; i++)
                {
                    Point start = Target.Center.ToTileCoordinates() + new Point(Main.rand.Next(-64, 65), 48);
                    for (int j = 0; j < 96; j++)
                    {
                        Point current = start - new Point(0, j);
                        if (!Main.tile[current].IsTileSolid() && Main.tile[current - new Point(0, 1)].TileType == TileID.Crimstone)
                            Dust.NewDust(current.ToWorldCoordinates(0, -16), 16, 16, DustID.Crimstone, 0, 3);
                    }
                }
            }
            if (spawnCounter == 180)
                NPC.velocity = Vector2.UnitY * (AIState == BrainAIState.UndergroundSpawnAnimation ? 32 : -50);
            else if (spawnCounter > 180)
            {
                NPC.velocity *= 0.955f;

                if (spawnCounter == 240)
                {
                    SoundEngine.PlaySound(IntroRoar, NPC.Center);

                    if (Main.netMode == NetmodeID.SinglePlayer)
                        Main.NewText(Language.GetTextValue("Announcement.HasAwoken", NPC.TypeName), 175, 75);
                    else if (Main.dedServ)
                        ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", NPC.TypeName), new Color(175, 75, 255));
                }

                if (spawnCounter > 240 && spawnCounter < 390)
                {
                    BrainOfCthulhuSystem.ScreenBlurStrength = 0.5f;// 0.5f;
                    if (spawnCounter < 250)
                        BrainOfCthulhuSystem.ScreenBlurStrength = MathHelper.Lerp(0, 0.5f, (spawnCounter - 240) / 10f);

                    NPC.frameCounter += 1f;

                    BoCDrawOffset = Main.rand.NextVector2Circular(4, 4);

                    for (int i = 0; i < 3; i++)
                    {
                        Point start = Target.Center.ToTileCoordinates() + new Point(Main.rand.Next(-64, 65), 48);
                        for (int j = 0; j < 96; j++)
                        {
                            Point current = start - new Point(0, j);
                            if (!Main.tile[current].IsTileSolid() && Main.tile[current - new Point(0, 1)].TileType == TileID.Crimstone)
                                Dust.NewDust(current.ToWorldCoordinates(0, -16), 16, 16, DustID.Crimstone, 0, 3);
                        }
                    }

                    Main.LocalPlayer.SetScreenshake(6 * BrainOfCthulhuSystem.ScreenBlurStrength * distanceScaleFactor);

                    if (spawnCounter % 15 == 0)
                    {
                        BossRoar pulse = new(NPC.Center, Color.Black, Main.rand.NextFloatDirection(), 0.1f, 3f, 30);
                        GeneralParticleHandler.SpawnParticle(pulse);
                    }
                }
                else if (spawnCounter >= 390 && spawnCounter <= 420)
                {
                    BrainOfCthulhuSystem.ScreenBlurStrength = MathHelper.Lerp(0.5f, 0f, (spawnCounter - 390) / 30f);
                    BoCDrawOffset *= 0.75f;
                }
                else if (spawnCounter > 420)
                {
                    BrainOfCthulhuSystem.ScreenBlurStrength = 0f;
                    BoCDrawOffset = Vector2.Zero;
                    AIState = BrainAIState.Phase1Idle;
                    NPC.damage = NPC.defDamage;
                    ResetAttackValues();
                    Time = -1;
                    SpawnTime = -1;
                    Main.musicFade[Main.curMusic] = 1f;
                    return;
                }
            }
        }
        if (AttackCounter < GetBrainOfCthuluCreepersCountRevDeath())
        {
            if (SpawnTime == 0)
            {
                NPC.Center = Target.Center + Vector2.UnitY * (AIState == BrainAIState.UndergroundSpawnAnimation ? -900 : 900);
                DisableMultiplayerSmoothing = true;
            }

            if(SpawnDelay == 1 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                bool targetLeft = AttackCounter % 2 == 0;
                List<NPC> creepers = Main.npc.Where(n => n.active && n.type == NPCID.Creeper && n.AIOverride<CreeperAI>().Time == -1 && n.AIOverride<CreeperAI>().CreeperID % 2 == (targetLeft ? 0 : 1)).ToList();

                if (creepers.Count > 0)
                {
                    AttackTime = creepers[Main.rand.Next(creepers.Count)].whoAmI;
                }
                else
                {
                    creepers = Main.npc.Where(n => n.active && n.type == NPCID.Creeper && n.AIOverride<CreeperAI>().Time == -1).ToList();
                    AttackTime = creepers.Count == 0 ? -1 : creepers[Main.rand.Next(creepers.Count)].whoAmI;
                }

                NPC.netUpdate = true;
            }

            if (SpawnDelay <= 0)
            {
                if (AttackTime != -1)
                {
                    NPC creeper = Main.npc[(int)AttackTime];
                    creeper.AIOverride<CreeperAI>().Time = 1;
                    creeper.netUpdate = true;
                    AttackCounter++;

                    if (SummonedViaItem || BossRushEvent.BossRushActive)
                        SpawnDelay = 1;
                    else
                        SpawnDelay = AttackCounter switch
                        {
                            1 => 90,
                            2 or 3 or 4 => 24,
                            5 => 60,
                            6 or 7 or 8 => 24,
                            9 => 60,
                            _ => 4,
                        };
                }
                else
                    SpawnTime = Time;
            }
            else
                SpawnDelay--;
        }
        else if (SpawnTime == 0)
            SpawnTime = Time;
    }

    #region Phase 1

    private void Phase1Idle()
    {
        BrainOfCthulhuSystem.ScreenBlurStrength = 0f;

        #region Attack Selection
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            if (CreeperHPRatio <= DesperateOnslaughtCreeperHealthGate)
            {
                Time = -1;
                AIState = BrainAIState.TelekineticOnslaught;
                AttackSign = Main.rand.NextBool() ? -1 : 1;
                NPC.netUpdate = true;
            }
            else if (Time > IdlePeriodDuration)
            {
                Time = -1;

                ResetAttackValues();

                if (availableAttacks.Count == 0)
                {
                    availableAttacks = [BrainAIState.CreeperSwipes, BrainAIState.CreeperSwings, BrainAIState.CreeperOrbit, BrainAIState.CreeperSpiral];
                    if (PreviousAttack != BrainAIState.Phase1Idle)
                        availableAttacks.Remove(PreviousAttack);
                }

                int pick = Main.rand.Next(availableAttacks.Count);
                AIState = availableAttacks[pick];
                availableAttacks.RemoveAt(pick);
                PreviousAttack = AIState;

                foreach (NPC creep in Main.npc.Where(n => n.active && n.type == NPCID.Creeper))
                    creep.AIOverride<CreeperAI>().Time = -1;

                SoundEngine.PlaySound(Growl, NPC.Center);

                NPC.netUpdate = true;
            }
        }
        #endregion

        /*
        #region Creeper Attacks
        if (AttackCounter > 0)
            AttackCounter--;
        else if (Time < IdlePeriodDuration - 120)
        {
            if (Time >= 30)
            {
                bool targetLeft = target.Center.X < NPC.Center.X;
                List<NPC> creepers = Main.npc.Where(n => n.active && n.type == NPCID.Creeper && n.AIOverride<CreeperAI>().Time == -1 && n.AIOverride<CreeperAI>().CreeperID % 2 == (targetLeft ? 0 : 1)).ToList();
                if (creepers.Count == 0)
                    creepers = Main.npc.Where(n => n.active && n.type == NPCID.Creeper && n.AIOverride<CreeperAI>().Time == -1).ToList();

                if (creepers.Count > 0)
                {
                    int rand = Main.rand.Next(creepers.Count);

                    NPC creeper = creepers[rand];
                    creeper.AIOverride<CreeperAI>().Time = 0;
                }
            }
            AttackCounter = (int)MathHelper.Lerp(CreeperChargeDelayMax, CreeperChargeDelayMin, 1 - CreeperAmountRatio);
        }
        #endregion
        */

        #region Movement
        if (Time == 0)
        {
            AttackSign = Main.rand.NextBool() ? -1 : 1;
            NPC.netUpdate = true;
            foreach (NPC creeper in Main.ActiveNPCs)
            {
                if (creeper.type != NPCID.Creeper)
                    continue;
                creeper.netUpdate = true;
            }
        }

        float rotateDir = AttackSign;
        Vector2 fromTarget = NPC.DirectionFrom(Target.Center);
        Vector2 dir = fromTarget.RotatedBy(Math.Sin(Time / 60f) * rotateDir) * new Vector2(2, 1);
        float rayDist = CalamityUtils.PreciseDistanceToTileCollisionHit(Target.Center, dir.ToRotation(), 360, 1);
        Vector2 offset = dir * (rayDist - NPC.width);
        Vector2 goalPos = Target.Center + offset;
        float distSQ = NPC.DistanceSQ(goalPos);
        if (distSQ > 129600)
        {
            NPC.velocity = NPC.DirectionTo(goalPos) * (4 + (NPC.Distance(goalPos) - 360) / 64f);
        }
        else if (distSQ <= 2048)
        {
            NPC.velocity *= 0.9f;
        }
        else if (NPC.velocity.LengthSquared() < 16f)
        {
            NPC.velocity += NPC.DirectionTo(goalPos).SafeNormalize(Vector2.Zero) * 0.15f;
        }
        else
        {
            NPC.velocity = NPC.DirectionTo(goalPos).SafeNormalize(Vector2.Zero) * 6f;
        }
        #endregion
    }

    private void TelekineticOnslaught()
    {
        #region Movement
        if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
        {
            NPC.velocity = NPC.DirectionTo(Target.Center) * 4f;
            Time = -1;
        }
        else
        {
            float distSQ = NPC.DistanceSQ(Target.Center);
            if (distSQ > 230400) //480^2
                NPC.velocity = NPC.DirectionTo(Target.Center) * (MathF.Sqrt(distSQ) - 480) / 128f;
            else
                NPC.velocity *= 0.9f;
        }
        #endregion

        float wrappedCounter = Time % 90;

        if (Time <= 60)
        {
            if (Time == 0)
            {
                SoundEngine.PlaySound(Roar, NPC.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    foreach (NPC creeper in Main.ActiveNPCs)
                    {
                        if (creeper.type != NPCID.Creeper)
                            continue;

                        CreeperAI ai = creeper.AIOverride<CreeperAI>();
                        int dirSign = ai.evenID ? -1 : 1;
                        Vector2 dir = Vector2.UnitX.RotatedBy(Main.rand.NextFloat(-MathHelper.TwoPi / 3f, MathHelper.TwoPi / 3f)) * dirSign;
                        if (Target.Center.Y / 16 < Main.worldSurface) //Makes creepers not go as high up when on the surface
                        {
                            float dirY = dir.Y;
                            dir.Y -= dirY * 0.5f;
                            dir.X += dirY * 0.5f * Math.Sign(dir.X);
                            dir.SafeNormalize(Vector2.unitXVector * dirSign);
                        }
                        ai.AttackAngle = dir.ToRotation();
                        float rayDist = CalamityUtils.PreciseDistanceToTileCollisionHit(NPC.Center, ai.AttackAngle, 800, 4);
                        ai.AttackPosition = NPC.Center + (dir * (rayDist - 64));
                        creeper.netUpdate = true;
                    }

                    NPC.netUpdate = true;
                }
            }
            if (Time < 30)
            {
                BrainOfCthulhuSystem.ScreenBlurStrength = 0.5f;

                NPC.frameCounter += 1f;

                for (int i = 0; i < 3; i++)
                {
                    Point start = Target.Center.ToTileCoordinates() + new Point(Main.rand.Next(-64, 65), 48);
                    for (int j = 0; j < 96; j++)
                    {
                        Point current = start - new Point(0, j);
                        if (!Main.tile[current].IsTileSolid() && Main.tile[current - new Point(0, 1)].TileType == TileID.Crimstone)
                            Dust.NewDust(current.ToWorldCoordinates(0, -16), 16, 16, DustID.Crimstone, 0, 3);
                    }
                }

                float d = Main.LocalPlayer.DistanceSQ(NPC.Center);
                float distanceScaleFactor = 1;
                if (d > 592900) //770^2
                    distanceScaleFactor = 1 / (1 + (((float)Math.Sqrt(d) - 770) / 32f));

                Main.LocalPlayer.SetScreenshake(4 * BrainOfCthulhuSystem.ScreenBlurStrength * distanceScaleFactor);

                if (Time % 15 == 0)
                {
                    BossRoar pulse = new(NPC.Center, Color.Black, Main.rand.NextFloatDirection(), 0.1f, 3f, 30);
                    GeneralParticleHandler.SpawnParticle(pulse);
                }
            }
            else
                BrainOfCthulhuSystem.ScreenBlurStrength = MathHelper.Lerp(0.5f, 0, CalamityUtils.CircOutEasing((Time - 30) / 30f, 1));
        }
        else
        {
            BrainOfCthulhuSystem.ScreenBlurStrength = 0f;
            if (wrappedCounter == 65)
            {
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    SelectNewTarget();
                    NPC.netUpdate = true;
                }

                int checkCount = 8;
                float wallDist = CalamityUtils.PreciseDistanceToTileCollisionHit(NPC.Center, AttackSign == -1 ? MathHelper.Pi : 0, 480 + NPC.width) - NPC.width;
                Vector2[] starts = new Vector2[checkCount];
                for (int i = 0; i < checkCount; i++)
                {
                    float completion = (i + 1) / (float)(checkCount + 1);
                    starts[i] = NPC.Center + (Vector2.UnitX * ((wallDist * completion) + NPC.width) * AttackSign);
                }

                Vector2[] ends = new Vector2[checkCount];
                List<Vector2> goodEnds = [];
                List<Vector2> farEnds = [];
                List<Vector2> closeEnds = [];

                for (int i = 0; i < checkCount; i++)
                {
                    float maxDist = 960;
                    float floorDist = CalamityUtils.PreciseDistanceToTileCollisionHit(NPC.Center, Vector2.UnitY.ToRotation(), maxDist);
                    ends[i] = starts[i] + (Vector2.UnitY * (floorDist + 48));
                    if (floorDist >= 600)
                        farEnds.Add(ends[i]);
                    else if (floorDist > 240)
                        goodEnds.Add(ends[i]);
                    else
                        closeEnds.Add(ends[i]);
                }

                Vector2 chosenEnd;

                if (goodEnds.Count > 0)
                    chosenEnd = goodEnds[Main.rand.Next(goodEnds.Count)];
                else if (closeEnds.Count > 0)
                    chosenEnd = closeEnds[Main.rand.Next(closeEnds.Count)];
                else
                    chosenEnd = farEnds[Main.rand.Next(farEnds.Count)];

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), chosenEnd, Vector2.Zero, ModContent.ProjectileType<TelekineticEnemyGrab>(), 10, 0.5f);

                AttackSign *= -1;
            }
        }
    }

    private void Stunned()
    {
        #region Movement
        NPC.velocity = NPC.velocity.ClampMagnitude(0f, 6f);
        if (Time == 0)
        {
            NPC.velocity = (NPC.Center - Target.Center).SafeNormalize(Vector2.UnitX) * 4f;
            SoundEngine.PlaySound(ShieldDown, NPC.Center);
        }

        if (NPC.velocity != Vector2.Zero)
        {
            if (Time >= StunDuration)
                NPC.velocity *= 0.8f;
            else
                NPC.velocity *= 0.93f;
        }

        if (Time < StunDuration)
            NPC.position.Y += (float)Math.Sin(Time / 8f) * 2 * (1 - MathHelper.Clamp((Time - (StunDuration - 30)) / 30f, 0f, 1f));

        if (Time <= StunDuration - 30)
        {
            if (AttackTime > 0)
            {
                float kbCounter = 30 - AttackTime;
                if (kbCounter < 10)
                {
                    float lerp = CalamityUtils.SineOutEasing(kbCounter / 10f, 1);
                    NPC.rotation = AttackRotation.AngleLerp(-AttackRotation, lerp);
                }
                else
                {
                    float lerp = CalamityUtils.SineInOutEasing((kbCounter - 10) / 20f, 1);
                    NPC.rotation = (-AttackRotation).AngleLerp(0, lerp);
                }

            }
            else if (Math.Abs(NPC.oldVelocity.X) < Math.Abs(NPC.velocity.X) || Time <= 0)
            {
                AttackRotation = NPC.rotation;
                TeleportTime = 0;
            }
            else
            {
                TeleportTime++;
                NPC.rotation = MathHelper.Lerp(AttackRotation, MathHelper.Pi / 24f * NPC.oldVelocity.X, CalamityUtils.CircOutEasing(MathHelper.Clamp(TeleportTime / 30f, 0f, 1f), 1));
            }
        }
        #endregion

        #region Tile Collision
        if (Time < StunDuration)
        {
            if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
            {
                NPC.velocity = NPC.DirectionTo(Target.Center) * 4f;
            }
            else if (Collision.SolidCollision(NPC.position + NPC.velocity, NPC.width, NPC.height))
            {
                if (NPC.velocity.X != NPC.oldVelocity.X)
                    NPC.velocity.X = -NPC.oldVelocity.X;
                if (NPC.velocity.Y != NPC.oldVelocity.Y)
                    NPC.velocity.Y = -NPC.oldVelocity.Y;
                //NPC.velocity *= 2f;
                NPC.velocity = NPC.velocity.ClampMagnitude(0f, 8f);
                AttackTime = 30;
                AttackRotation = NPC.rotation;
            }

            if (AttackTime > 0)
            {
                NPC.knockBackResist = 0f;
                AttackTime--;
                if (AttackTime == 0)
                {
                    NPC.velocity = Vector2.Zero;
                    AttackRotation = 0;
                }
            }
            else
                NPC.knockBackResist = 1f;
        }
        #endregion

        BrainOfCthulhuSystem.ScreenBlurStrength = 0f;

        NPC.dontTakeDamage = false;
        NPC.damage = 0;

        if (Time <= 15)
        {
            float lerp = Time / 15f;
            ShieldOpacity = 1 - CalamityUtils.CircOutEasing(lerp, 1);
            ShieldScale = MathHelper.Lerp(1f, 1.5f, lerp);
        }
        if(Time > 15 && Time < StunDuration)
        {
            ShieldOpacity = 0f;
            ShieldScale = 1.5f;
        }

        #region Recovery
        if (OnSecondCreeperPhase && Time == StunDuration - 30)
        {
            AIState = BrainAIState.Phase2TransitionClosed;
            Time = -1;
            TeleportTime = 0;
            return;
        }

        if (Time > StunDuration - 30)
        {
            NPC.rotation = NPC.rotation.AngleLerp(0f, CalamityUtils.SineInOutEasing((Time - (StunDuration - 30)) / 30f, 1));
            if (Time == StunDuration - 15)
                SoundEngine.PlaySound(ShieldUp, NPC.Center);
        }

        if (Time >= StunDuration)
        {
            if (NPC.velocity.X < 0.001f && NPC.velocity.X < 0.001f)
                NPC.velocity = Vector2.Zero;

            int creeperRate = 5;
            float wrappedCounter = (Time - StunDuration) % creeperRate;
            int spawnTime = GetBrainOfCthuluCreepersCountRevDeath() / 2 * creeperRate;

            if (Time == StunDuration)
            {
                AttackCounter = GetBrainOfCthuluCreepersCountRevDeath() - 1;
                SoundEngine.PlaySound(Roar, NPC.Center);
            }

            NPC.knockBackResist = 0f;
            NPC.dontTakeDamage = true;
            NPC.rotation = 0f;

            float shieldAppearTime = 15f;

            float lerp = MathHelper.Clamp((Time - StunDuration) / shieldAppearTime, 0f, 1f);
            if (lerp >= 1)
            {
                ShieldOpacity = 1f;
                ShieldScale = 1f;
            }
            else
            {
                ShieldOpacity = CalamityUtils.CircOutEasing(lerp, 1);
                ShieldScale = MathHelper.Lerp(1.5f, 1f, CalamityUtils.SineOutEasing(lerp, 1));
            }

            if (AttackCounter == -1 && Time > StunDuration + spawnTime + 30)
            {
                OnSecondCreeperPhase = true;
                AIState = BrainAIState.Phase1Idle;
                Time = -1;
                NPC.damage = NPC.defDamage;
            }
            else if (AttackCounter > -1 && wrappedCounter == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 dir = Vector2.UnitY.RotatedBy((AttackCounter % 2 == 0 ? 1 : -1) * (MathHelper.Pi / 16f + ((MathHelper.Pi - MathHelper.Pi / 8f) * (AttackCounter / 2f / (GetBrainOfCthuluCreepersCountRevDeath() / 2f)))));
                    Vector2 spawnPos = NPC.Center + (dir * 72f);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC creeper = NPC.NewNPCDirect(NPC.GetSource_FromAI(), spawnPos, NPCID.Creeper, NPC.whoAmI, AttackCounter, ai2: -1, ai3: 1);
                        creeper.velocity = dir * 24f;
                    }

                    for (int j = 0; j < 3; j++)
                    {
                        BloodParticle p = new(spawnPos, dir.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 6f, MathHelper.Pi / 6f)) * Main.rand.NextFloat(8f, 12f), 32, 1f, Color.Red);
                        GeneralParticleHandler.SpawnParticle(p);
                    }
                    BloodParticle2 p2 = new(spawnPos, dir * 10f, 16, 0.5f, Color.Red);
                    GeneralParticleHandler.SpawnParticle(p2);           

                    AttackCounter--;
                    if (AttackCounter <= -1)
                        return;
                }

                SoundEngine.PlaySound(SoundID.NPCHit9, NPC.Center);
            }
        }
        #endregion
    }

    private void CreeperSwipes()
    {
        //Hand Size check to determine if 1 hand variant should be used
        if (Time == 0 && Main.netMode != NetmodeID.MultiplayerClient)
        {
            int leftAmt = 0;
            int rightAmt = 0;
            foreach (NPC creeper in Main.ActiveNPCs)
            {
                if (creeper.type != NPCID.Creeper)
                    continue;

                if (creeper.AIOverride<CreeperAI>().CreeperID % 2 == 0)
                    leftAmt++;
                else
                    rightAmt++;
            }

            if (leftAmt > 5 && rightAmt > 5)
            {
                AttackSign = Main.rand.NextBool() ? -1 : 1;
                AttackFlag = false;
            }
            else
                AttackFlag = true;

            NPC.damage = 0;

            NPC.netUpdate = true;
            AttackList.Clear();
            TargetsSet.Clear();
        }

        float wrappedCount = Time % (SwipeDuration + SwipeDelay);

        if (Time >= SwipesStartupDuration)
        {
            NPC.damage = NPC.defDamage;
            if (wrappedCount == 0)
            {
                bool useEven = Main.rand.NextBool();

                bool anyActivated = false;

                foreach (NPC Npc in Main.ActiveNPCs)
                {
                    if (Npc.type != NPCID.Creeper)
                        continue;

                    if (AttackFlag || Npc.AIOverride<CreeperAI>().CreeperID % 2 == 0! ^ useEven)
                    {
                        AttackList.Add((byte)Npc.whoAmI);
                        //Npc.AIOverride<CreeperAI>().Time = 0;
                        anyActivated = true;
                    }
                }

                if (!anyActivated)
                    foreach (NPC Npc in Main.ActiveNPCs)
                    {
                        if (Npc.type != NPCID.Creeper)
                            continue;

                        AttackList.Add((byte)Npc.whoAmI);
                        //Npc.AIOverride<CreeperAI>().Time = 0;
                    }

                if (Main.netMode != NetmodeID.SinglePlayer)
                    SelectNewTarget();

                NPC.netUpdate = true;
            }
            else if (wrappedCount == SwipeDuration && Main.netMode != NetmodeID.MultiplayerClient)
            {
                AttackSign *= -1;
                AttackList.Clear();
                NPC.netUpdate = true;
            }

            if (wrappedCount > 1 && wrappedCount <= SwipeIchorDelay && Time % 3 == 0) // Telegraphs ichor rain w/ dripping particles
            {
                Vector2 spawnPosition = NPC.Center;
                spawnPosition.Y += Main.rand.NextFloat(38, 50);
                spawnPosition.X += Main.rand.NextFloat(-56, 56);

                BloodParticle blood = new BloodParticle(spawnPosition, Main.rand.NextVector2Unit() * Main.rand.NextFloat(1.5f, 2.5f), Main.rand.Next(30, 40), Main.rand.NextFloat(0.6f, 1f), Color.Gold);
                GeneralParticleHandler.SpawnParticle(blood);
            }

            if (wrappedCount < 60f) // Vibrates during telegraph
            {
                Vector2 vibrationVector = Main.rand.NextVector2CircularEdge(1, 1) * MathHelper.Lerp(0f, 12f, CalamityUtils.CircInEasing(wrappedCount / 80f, 1));

                BoCDrawOffset = vibrationVector;
            }
            else if (wrappedCount > 60f && wrappedCount < 80f) // Droops down when starting to fire ichor shots
            {
                float progress = (wrappedCount - 60f) / 20f;
                BoCDrawOffset = new Vector2(0, MathHelper.Lerp(10, 0, 1f - (float)Math.Pow(1f - progress, 3f)));
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
                if (wrappedCount > SwipeIchorDelay && wrappedCount <= SwipeDuration && Time % 2 == 0)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-72, 72), 56), Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 4f, MathHelper.Pi / 4f)) * 4f, ProjectileID.GoldenShowerHostile, IchorShotDamage, 0.5f);

            if (Time >= SwipesStartupDuration + ((SwipeDuration + SwipeDelay) * SwipeAmount) && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Time = 0;
                AIState = BrainAIState.Phase1Idle;
                NPC.netUpdate = true;
                NPC.damage = NPC.defDamage;
                AttackList.Clear();
            }
        }
        else
            NPC.damage = 0;

        #region Movement
        
        Vector2 goalPos = Target.Center + (Vector2.UnitY * -270);
        float distSQ = NPC.DistanceSQ(goalPos);
        if ((Time > SwipesStartupDuration && wrappedCount > 30 && wrappedCount <= SwipeDuration) || NPC.DistanceSQ(goalPos) <= 2048)
            NPC.velocity *= 0.9f;
        else if (distSQ > 14400)
            NPC.velocity = NPC.DirectionTo(goalPos) * (8 + (NPC.Distance(goalPos) - 120) / 16f);
        else
            NPC.velocity = NPC.DirectionTo(goalPos).SafeNormalize(Vector2.UnitX * -Target.direction) * (8f * distSQ / 14400f);
        #endregion
    }

    private void CreeperSwings()
    {
        NPC.damage = 0;

        #region Movement
        Vector2 fromTarget;
        if (Main.netMode == NetmodeID.SinglePlayer)
            fromTarget = NPC.Center - Target.Center;          
        else
        {
            Vector2 averagePlayerPos = Vector2.zeroVector;
            List<int> targets = GetAllValidTargets(NPC.Center);
            foreach (var p in targets)
                averagePlayerPos += Main.player[p].Center;
            averagePlayerPos /= targets.Count;

            fromTarget = NPC.Center - averagePlayerPos;
        }

        Vector2 goalDir;
        if (Math.Abs(fromTarget.X) > Math.Abs(fromTarget.Y))
            goalDir = Vector2.UnitX * Math.Sign(fromTarget.X);
        else
            goalDir = Vector2.UnitY * Math.Sign(fromTarget.Y);

        Vector2 goalPos = Target.Center + (goalDir * 360) - (Vector2.UnitY * 32);
        if (NPC.DistanceSQ(goalPos) <= 2048)
            NPC.velocity *= 0.9f;
        else if (NPC.velocity.LengthSquared() <= 56.25f) //7.5^2
            NPC.velocity += NPC.DirectionTo(goalPos).SafeNormalize(Vector2.UnitX * -Target.direction) * 0.5f;
        else
            NPC.velocity = NPC.DirectionTo(goalPos).SafeNormalize(Vector2.UnitX * -Target.direction) * 8f;
        #endregion

        int delay = OnSecondCreeperPhase ? StrongSwipeDelay : LightSwipeDelay;
        if (Time == 0)
            AttackList.Clear();

        if (Time > delay)
        {
            foreach (NPC creeper in Main.npc.Where(n => n.active && n.type == NPCID.Creeper && n.AIOverride<CreeperAI>().Time == -1 && !AttackList.Contains((byte)n.whoAmI)))
                creeper.position += NPC.velocity;

            int crushCount;
            int attackDelay;
            if (!OnSecondCreeperPhase)
            {
                crushCount = LightSwipeAmount;
                attackDelay = LightSwipeDuration;
            }
            else
            {
                crushCount = StrongSwipeAmount;
                attackDelay = StrongSwipeDuration;
            }
            int attackDur = delay + ((attackDelay + 1) * crushCount);

            if (Time < attackDur && Time % attackDelay == 0)
            {
                List<NPC> creepers = Main.npc.Where(n => n.active && n.type == NPCID.Creeper && n.AIOverride<CreeperAI>().Time == -1 && !AttackList.Contains((byte)n.whoAmI)).ToList();
                if (creepers.Count > 1 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float rotation;
                    if (!OnSecondCreeperPhase)
                    {
                        rotation = Target.velocity.ToRotation();
                        if (Target.velocity == Vector2.Zero)
                            rotation = Target.direction == 1 ? 0 : MathHelper.Pi;
                        rotation += Main.rand.NextFloat(-MathHelper.PiOver4 / 2f, MathHelper.PiOver4 / 2f);
                    }
                    else
                        rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);

                    int rand = Main.rand.Next(creepers.Count);
                    NPC first = creepers[rand];
                    first.netUpdate = true;
                    CreeperAI creeper1 = first.AIOverride<CreeperAI>();
                    AttackList.Add((byte)first.whoAmI);
                    //creeper1.Time = 0;
                    creeper1.AttackAngle = rotation;
                    creepers.RemoveAt(rand);

                    NPC second = creepers[Main.rand.Next(creepers.Count)];
                    second.netUpdate = true;
                    CreeperAI creeper2 = second.AIOverride<CreeperAI>();
                    AttackList.Add((byte)second.whoAmI);
                    //creeper2.Time = 0;
                    creeper2.AttackAngle = rotation + MathHelper.Pi;

                    creeper1.PartnerIndex = second.whoAmI;
                    creeper2.PartnerIndex = first.whoAmI;

                    if (Main.netMode != NetmodeID.SinglePlayer)
                        SelectNewTarget();

                    NPC.netUpdate = true;
                }
            }

            if (Time > attackDur + attackDelay)
            {
                Time = 0;
                NPC.damage = NPC.defDamage;
                AIState = BrainAIState.Phase1Idle;
                AttackList.Clear();
            }
        }
    }

    private void CreeperOrbit()
    {
        #region Movement
        Vector2 fromTarget = (NPC.Center - Target.Center).SafeNormalize(Vector2.UnitX);
        Vector2 goalPos = Target.Center + (fromTarget * 440) - (Vector2.UnitY * 32);
        float distSQ = NPC.DistanceSQ(goalPos);
        if (distSQ > 14400)
            NPC.velocity = NPC.DirectionTo(goalPos) * (4 + (NPC.Distance(goalPos) - 120) / 16f);
        else
            NPC.velocity = NPC.DirectionTo(goalPos).SafeNormalize(Vector2.Zero) * (2 + (2f * (distSQ / 14400)));
        #endregion

        if (Time == 0 && Main.netMode != NetmodeID.MultiplayerClient)
        {
            AttackSign = Main.rand.NextBool() ? -1 : 1;
            AttackPosition = Target.Center;
            AttackList.Clear();
            NPC.netUpdate = true;

            NPC.damage = 0;

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                List<int> extraTargets = GetAllValidTargets(NPC.Center);
                extraTargets.Remove(NPC.target);

                int targetCount = extraTargets.Count;
                int creepersPerExtraPlayer = CalamityWorld.death ? 3 : 2;
                int creepersDesired = targetCount * creepersPerExtraPlayer;
                int creepersAlive = NPC.CountNPCS(NPCID.Creeper);
                int creepersToSpare = creepersAlive - 4;
                for (int i = 0; i < creepersPerExtraPlayer; i++)
                {
                    if (creepersToSpare >= creepersDesired || creepersPerExtraPlayer == 1)
                        break;

                    if (creepersToSpare < creepersDesired && creepersPerExtraPlayer > 1)
                    {
                        creepersPerExtraPlayer--;
                        creepersDesired = targetCount * creepersPerExtraPlayer;
                    }
                }
                int creepersSparedForTarget = 0;

                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (n.type != NPCID.Creeper)
                        continue;

                    n.TryGetAIOverride<CreeperAI>(out var creeper);
                    if (creepersDesired > 0 && creepersToSpare > 0)
                    {
                        creeper.CachedValue2 = extraTargets[0];
                        creepersToSpare--;
                        creepersDesired--;

                        creeper.Time -= 30 * creepersSparedForTarget;

                        if (++creepersSparedForTarget >= creepersPerExtraPlayer)
                        {
                            extraTargets.RemoveAt(0);
                            creepersSparedForTarget = 0;
                        }
                        AttackCounter++;
                    }
                    else
                        creeper.CachedValue2 = -1;
                }
            }
            else
            {
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (n.type != NPCID.Creeper)
                        continue;

                    n.TryGetAIOverride<CreeperAI>(out var creeper);
                    creeper.CachedValue2 = -1;
                }
            }
        }

        if (Time < OrbitSetupDuration)
        {
            AttackPosition = Target.Center;
        }
        else
        {
            float prox = Target.DistanceSQ(AttackPosition);
            if (prox > 65536) //256^2
                AttackPosition += Target.DirectionFrom(AttackPosition) * (((float)Math.Sqrt(prox) - 256) / 16f);
        }

        if (Time >= OrbitDuration + 30)
        {
            Time = -1;
            AIState = BrainAIState.Phase1Idle;
            AttackList.Clear();
            NPC.damage = NPC.defDamage;
            foreach (NPC creep in Main.ActiveNPCs)
            {
                if (creep.type == NPCID.Creeper)
                    creep.AIOverride<CreeperAI>().Time = -1;
            }
        }
        else if (Time >= OrbitAttackInterval && Time < OrbitDuration && Time % OrbitAttackInterval == 0 && Main.netMode != NetmodeID.MultiplayerClient)
        {
            List<NPC> mainOrbitMembers = Main.npc.Where(n => n.active && n.type == NPCID.Creeper && n.TryGetAIOverride<CreeperAI>(out var ai) && ai.CachedValue2 == -1).ToList();
            if (mainOrbitMembers.Count > 0)
            {
                int rand = Main.rand.Next(mainOrbitMembers.Count);
                for (int i = 0; i < OrbitAttackParticipantCount; i++)
                {
                    if (rand >= mainOrbitMembers.Count)
                        rand -= mainOrbitMembers.Count;
                    NPC creeper = mainOrbitMembers[rand];
                    AttackList.Add((byte)creeper.whoAmI);
                    //creeper.AIOverride<CreeperAI>().Time = 0;
                    rand += (int)Math.Round(mainOrbitMembers.Count / (float)OrbitAttackParticipantCount);
                }

                NPC.netUpdate = true;
            }
        }
    }

    private void CreeperSpiral()
    {
        if (Time == 0 && Main.netMode != NetmodeID.MultiplayerClient)
        {
            AttackSign = Main.rand.NextBool() ? -1 : 1;
            AttackRotation = 0;
            NPC.netUpdate = true;
        }
        float startTimePerRev = MathHelper.Lerp(StartingTimePerRevolutionMax, StartingTimePerRevolutionMin, 1 - CreeperAmountRatio);
        float endTimePerRev = MathHelper.Lerp(EndingTimePerRevolutionMax, EndingTimePerRevolutionMin, 1 - CreeperAmountRatio);
        float spinSpeedCompletion = MathHelper.Clamp((Time - SpeedUpDelayTime) / (SpiralDuration - SpeedUpDelayTime - SpeedUpExtensionTime), 0f, 1f);

        float timePerRev = MathHelper.Lerp(startTimePerRev, endTimePerRev, spinSpeedCompletion);
        if (Time > SpiralDuration - 30)
            timePerRev *= MathHelper.Lerp(1f, 10f, CalamityUtils.CircOutEasing(MathHelper.Clamp((Time - (SpiralDuration - 30)) / 30f, 0f, 1f), 1));
        else if (Time < SpiralSetupTime)
            timePerRev *= MathHelper.Lerp(1f, 10f, CalamityUtils.CircInEasing(MathHelper.Clamp(Time / SpiralSetupTime, 0f, 1f), 1));

        float rotToAdd = MathHelper.TwoPi / timePerRev * AttackSign;
        if (OnSecondCreeperPhase)
        {
            float attackComplationRatio = Time / SpiralDuration;
            float lerp = Utils.GetLerpValue(TurnAroundRatio - (TurnAroundDurationRatio / 2f), TurnAroundRatio + (TurnAroundDurationRatio / 2f), attackComplationRatio, true);
            rotToAdd *= MathHelper.Lerp(1, -1, lerp);
        }
        AttackRotation += rotToAdd;

        if (NPC.DistanceSQ(Target.Center) > 57600)
            NPC.velocity = NPC.DirectionTo(Target.Center) * (NPC.Distance(Target.Center) - 240) / 32f;
        else
            NPC.velocity *= 0.9f;

        if (Time > SpiralDuration)
        {
            Time = -1;
            AIState = BrainAIState.Phase1Idle;
            foreach (NPC creep in Main.ActiveNPCs)
            {
                if (creep.type == NPCID.Creeper)
                    creep.AIOverride<CreeperAI>().Time = -1;
            }
        }
    }
    
    #endregion

    private void PhaseTransition()
    {
        NPC.dontTakeDamage = true;
        NPC.rotation *= 0.9f;
        TeleportTime = 0;
        NPC.damage = 0;

        float animCounter = Time - 60;
        if (animCounter >= 0)
        {
            if (animCounter == 0)
                NPC.velocity = Vector2.UnitY * 2f;
            else if (animCounter < 60)
                NPC.velocity *= 0.99f;
            else if (animCounter == 60)
                NPC.velocity = Vector2.UnitY * -8f;
            else if (animCounter == 65)
            {
                AIState = BrainAIState.Phase2TransitionOpen;
                PreviousAttack = BrainAIState.Phase1Idle;
                availableAttacks.Clear();
                NPC.netUpdate = true;

                SoundEngine.PlaySound(SoundID.NPCHit1, NPC.Center);

                if (!Main.dedServ)
                {
                    //Spawns all of BoC's Phase Transition Gores (GoreIDs 392 -> 395)
                    for (int i = 392; i <= 395; i++)
                        Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, Main.rand.NextVector2Circular(6f, 6f), i);
                }

                for (int j = 0; j < 20; j++)
                {
                    Vector2 edgeBloodDir = Main.rand.NextVector2CircularEdge(1, 1);
                    BloodParticle b = new(NPC.Center - (Vector2.unitYVector * 32) + (edgeBloodDir * new Vector2(16, 24)), edgeBloodDir.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 10f, MathHelper.Pi / 10f)) * Main.rand.NextFloat(4f, 8f), 24, 0.75f, Color.Red);
                    GeneralParticleHandler.SpawnParticle(b);

                    Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(NPC.Hitbox), DustID.Blood, Main.rand.NextVector2Circular(6f, 6f));
                }

                for (int i = 1; i <= 3; i++)
                {
                    Color color = i switch
                    {
                        1 => Color.Yellow,
                        2 => Color.Orange,
                        _ => Color.Red,
                    };
                    PulseRing ring = new(NPC.Center, NPC.velocity * 0.5f, color, 0f, 1f + i * 0.5f, 24);
                    GeneralParticleHandler.SpawnParticle(ring);
                }
                
                BoCAfterImages = [];

                SoundEngine.PlaySound(Roar, NPC.Center);
            }
            else
                NPC.velocity *= 0.9f;

            if (animCounter < 60f)
                BoCDrawOffset = Main.rand.NextVector2CircularEdge(1, 1) * MathHelper.Lerp(0f, 16f, CalamityUtils.CircInEasing(animCounter / 60f, 1));
            else if (animCounter < 70f)
                BoCDrawOffset = Main.rand.NextVector2CircularEdge(1, 1) * MathHelper.Lerp(16f, 0f, CalamityUtils.CircOutEasing((animCounter - 60) / 10f, 1));

            if (animCounter >= 120f)
            {
                Time = 0;
                ResetAttackValues();
                AIState = BrainAIState.Phase2Idle;
                NPC.dontTakeDamage = false;
                NPC.damage = NPC.defDamage;
            }
        }
        else
        {
            NPC.velocity *= 0.8f;

            #region Tile Collision
            if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
            {
                NPC.velocity = NPC.DirectionTo(Target.Center) * 4f;
            }
            else if (Collision.SolidCollision(NPC.position + NPC.velocity, NPC.width, NPC.height))
            {
                if (NPC.velocity.X != NPC.oldVelocity.X)
                    NPC.velocity.X = -NPC.oldVelocity.X;
                if (NPC.velocity.Y != NPC.oldVelocity.Y)
                    NPC.velocity.Y = -NPC.oldVelocity.Y;
                NPC.velocity *= 2f;
                NPC.velocity = NPC.velocity.ClampMagnitude(0f, 8f);
            }
            #endregion
        }
    }

    #region Phase 2

    private void Phase2Idle()
    {
        NPC.rotation = NPC.velocity.X / 6f * MathHelper.Pi / 8f;

        if (Time == ChaseTime - 5)
            AttackCounter++;

        if (Time <= ChaseTime)
        {
            NPC.damage = NPC.defDamage;
            float speedUp = MathHelper.Clamp((Time - 10) / 10f, 0f, 1f);
            float slowDown = 1 - MathHelper.Clamp((Time - (ChaseTime - 15)) / 15f, 0f, 1f);
            float angleChange = MathHelper.Lerp(MathHelper.Pi / 24f, 0f, MathHelper.Clamp(Time / (ChaseTime * 0.666f), 0f, 1f));
            NPC.velocity = NPC.velocity.RotateDirectionTowards(NPC.DirectionTo(Target.Center).ToRotation(), angleChange) * (MathHelper.Lerp(ChaseMinSpeed, ChaseMaxSpeed, Time / ChaseTime) * speedUp * slowDown);

            if (Time == ChaseTime)
            {
                if (Main.netMode != NetmodeID.SinglePlayer)
                    SelectNewTarget();
                Vector2 direction = Target.velocity.SafeNormalize(Vector2.UnitX * Target.direction).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 3f, MathHelper.Pi / 3f));
                AttackPosition = Target.Center + (direction * DefaultTeleportDistance);
                BoCAfterImages = [];
                NPC.damage = 0;
                NPC.netUpdate = true;
            }
            else
            {
                TeleportTime = 0;
                NPC.Opacity = 1f;
            }
        }
        else
        {
            TeleportDuration = IdleTeleportDuration;
            NPC.damage = 0;

            Vector2 endPoint = AttackPosition;

            NPC.velocity = Vector2.Zero;

            if (Time < ChaseTime + (TeleportDuration / 2f))
            {
                if (Time % 4 == 0)
                {
                    Vector2 startPoint = NPC.Center;

                    Vector2 direction = endPoint - startPoint;
                    float curveIntensity = Main.rand.NextFloat(-0.2f, 0.2f);
                    Vector2 perpindicular = direction.RotatedBy(MathHelper.PiOver2);

                    Vector2 controlPoint1 = startPoint + (direction * 0.25f) + (perpindicular * curveIntensity);
                    Vector2 controlPoint2 = startPoint + (direction * 0.75f) + (perpindicular * curveIntensity);

                    BezierCurve path = new BezierCurve(startPoint, controlPoint1, controlPoint2, endPoint);

                    BrainOfCthulhuAfterImage afterimage = new(path, NPC.rotation, Vector2.One, (int)(ChaseTime + (TeleportDuration * 0.75f) - Time), BoCFrame);
                    BoCAfterImages.Add(afterimage);
                    GeneralParticleHandler.SpawnParticle(afterimage);
                }
                TeleportTime++;
            }
            else if (Time == ChaseTime + (TeleportDuration / 2f) && !AttackFlag)
            {
                NPC.Center = endPoint;
                NPC.damage = NPC.defDamage;
                DisableMultiplayerSmoothing = true;
                AttackFlag = true;
                NPC.netUpdate = true;
            }
            else
            {
                TeleportTime--;
                if (TeleportTime < 0)
                {
                    TeleportTime = 0;
                    Time = -1;
                    AttackFlag = false;

                    #region Attack Selection
                    if (AttackCounter >= ChaseAmount) //Pick attack
                    {
                        NPC.rotation = 0;
                        ResetAttackValues();

                        if (availableAttacks.Count == 0)
                        {
                            bool quickChoice = Main.rand.NextBool();
                            availableAttacks = [
                                BrainAIState.Bloodletting,
                                        quickChoice ? BrainAIState.SanguineScythes : BrainAIState.IllusionDash,
                                        Main.rand.NextBool() ? BrainAIState.Phase2Idle : BrainAIState.Bloodletting,
                                        quickChoice ? BrainAIState.IllusionDash : BrainAIState.SanguineScythes,
                                        BrainAIState.IllusionTrick
                            ];
                        }

                        AIState = availableAttacks[0];
                        availableAttacks.RemoveAt(0);
                        PreviousAttack = AIState;

                        if (AIState == BrainAIState.SanguineScythes)
                        {
                            Time = -31;
                            BoCAfterImages = [];
                        }

                        NPC.netUpdate = true;
                    }
                    #endregion
                }
            }

            NPC.Opacity = 1 - (TeleportTime / (TeleportDuration / 2f));
        }
    }

    private void Bloodletting()
    {
        float endTime = Time - BloodlettingDuration;

        #region Movement

        if (endTime < 0)
        {
            if (Time == 0)
            {
                AttackPosition = NPC.Center;
                if (NPC.Center.X < Target.Center.X)
                    AttackSign = -1;
                else
                    AttackSign = 1;

                CachedRatio = float.MaxValue;
                AttackTime = float.MaxValue;

                NPC.netUpdate = true;
            }

            if (Time == 30)
                AttackPosition = Vector2.zeroVector;

            //Make sure we have an accurate view on who the furthest players
            if (Main.netMode != NetmodeID.SinglePlayer && Time % 2 == 0)
            {
                Player furthestLeft = null;
                Player furthestRight = null;
                Player highestUp = null;
                foreach (int who in GetAllValidTargets(NPC.Center))
                {
                    Player p = Main.player[who];

                    if (furthestLeft == null || p.Center.X < furthestLeft.Center.X)
                        furthestLeft = p;

                    if (furthestRight == null || p.Center.X > furthestRight.Center.X)
                        furthestRight = p;

                    if (highestUp == null || p.Center.Y < highestUp.Center.Y)
                        highestUp = p;
                }

                AttackList.Clear();
                AttackList.Add((byte)furthestLeft.whoAmI);
                AttackList.Add((byte)furthestRight.whoAmI);
                AttackList.Add((byte)highestUp.whoAmI);
            }

            float waveValue = Time * MathHelper.Pi / BloodshotRate;
            Vector2 goalPos;
            if (Main.netMode == NetmodeID.SinglePlayer)
                goalPos = Target.Center + new Vector2((float)Math.Cos(waveValue) * HoverDistance.X * AttackSign, (float)(-0.5f * Math.Cos(2 * waveValue) + 0.5f) * -HoverDistance.Y);
            else
            {
                Player furthestLeft = Main.player[AttackList[0]];
                Player furthestRight = Main.player[AttackList[1]];
                Player highestUp = Main.player[AttackList[2]];

                Vector2 hoverCenter = (furthestLeft.Center + furthestRight.Center) / 2f;
                float xDist = furthestRight.Center.X - furthestLeft.Center.X;
                float yDist = hoverCenter.Y - highestUp.Center.Y;

                if (AttackPosition == Vector2.zeroVector)
                    AttackPosition = hoverCenter;
                else
                    AttackPosition += (hoverCenter - AttackPosition) / 30f;

                if (CachedRatio == float.MaxValue || Math.Abs(xDist - CachedRatio) < 0.01f)
                    CachedRatio = xDist;
                else
                    CachedRatio += (xDist - CachedRatio) / 30f;

                if (AttackTime == float.MaxValue || Math.Abs(yDist - AttackTime) < 0.01f)
                    AttackTime = yDist;
                    AttackTime += (yDist - AttackTime) / 30f;

                float xMag = (HoverDistance.X + CachedRatio);
                float yMag = (HoverDistance.Y + AttackTime);
                Vector2 destination = AttackPosition + new Vector2((float)Math.Cos(waveValue) * xMag * AttackSign, (float)(-0.5f * Math.Cos(2 * waveValue) + 0.5f) * -yMag);

                goalPos = destination;
            }

            NPC.velocity = Vector2.Zero;
            DisableMultiplayerSmoothing = true;
            if (Time < 30f)
                NPC.Center = Vector2.Lerp(AttackPosition, goalPos, CalamityUtils.SineOutEasing(Time / 30f, 1));
            else
                NPC.Center = goalPos;
        }
        else if (endTime < DashPrepTime)
        {
            if (endTime == 0)
                NPC.velocity = Vector2.UnitX * AttackSign * 10f;
            else
            {
                Vector2 goalPos = Target.Center - Vector2.UnitY * HoverEndHeight;
                Vector2 accel = new Vector2(0.5f, 1.5f);

                NPC.velocity += NPC.DirectionTo(goalPos).SafeNormalize(Vector2.Zero) * accel;
                NPC.velocity = NPC.velocity.ClampMagnitude(0f, 8f);
            }
        }
        #endregion

        #region Main Attack
        if (endTime < 0)
        {
            NPC.rotation = (float)Math.Sin(Time / 8f) * MathHelper.Pi / 8f;
            BoCDrawOffset = Vector2.Zero;

            NPC.damage = 0;

            if (Time > BloodshotRate) //Doesnt fire first bloodshot
            {
                if (Time % IchorRate == 0)
                {
                    if (Time > BloodshotRate + 30 && Time % (IchorRate * 10) == 0)
                    {
                        SoundEngine.PlaySound(BloodBomb, NPC.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BloodBomb>(), NPC.whoAmI);
                    }
                    else
                    {
                        if (Time % (IchorRate * 2) == 0)
                            SoundEngine.PlaySound(SoundID.Item17, NPC.Center);

                        if (Main.netMode == NetmodeID.SinglePlayer)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.velocity + Main.rand.NextVector2Circular(72, 72), new Vector2(Main.rand.NextFloat(-IchorSpread, IchorSpread), -IchorVelocity), ModContent.ProjectileType<IchorShower>(), IchorShotDamage, 0.5f);
                        else if (Main.dedServ)
                        {
                            Player furthestLeft = Main.player[AttackList[0]];
                            Player furthestRight = Main.player[AttackList[1]];
                            float xDist = furthestRight.Center.X - furthestLeft.Center.X;
                            int projCount = 1 + (int)(xDist / 360);

                            for (int i = 0; i < projCount; i++)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.velocity + Main.rand.NextVector2Circular(72, 72), new Vector2(Main.rand.NextFloat(-IchorSpread, IchorSpread), -IchorVelocity), ModContent.ProjectileType<IchorShower>(), IchorShotDamage, 0.5f);
                        }
                    }
                }

                if (Time % BloodshotRate == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 dir = NPC.DirectionTo(Target.Center);

                        for (int i = -2; i <= 2; i++)
                        {
                            Vector2 initialDir = dir.RotatedBy(i * MathHelper.Pi / 4f);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, initialDir * BloodshotVelocity, ProjectileID.BloodNautilusShot, BloodShotDamage, 0.5f, ai0: dir.ToRotation() + MathHelper.TwoPi, ai1: initialDir.ToRotation());
                        }

                        if (CalamityWorld.death)
                        {
                            Vector2 initialDir = dir.RotatedBy(MathHelper.Pi / 6f);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, initialDir * BloodshotVelocity / 2.1f, ProjectileID.BloodNautilusShot, BloodShotDamage, 0.5f, ai0: dir.ToRotation() + MathHelper.TwoPi, ai1: initialDir.ToRotation());
                            initialDir = dir.RotatedBy(-MathHelper.Pi / 6f);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, initialDir * BloodshotVelocity / 2.1f, ProjectileID.BloodNautilusShot, BloodShotDamage, 0.5f, ai0: dir.ToRotation() + MathHelper.TwoPi, ai1: initialDir.ToRotation());
                        }
                    }
                    SoundEngine.PlaySound(BloodShot with { PitchVariance = 0.5f }, NPC.Center);
                }

            }
        }
        #endregion

        #region Attack End
        else
        {
            NPC.rotation *= 0.9f;

            NPC.damage = NPC.defDamage;

            if (endTime == 0)
                SoundEngine.PlaySound(Roar, NPC.Center);

            if (endTime >= DashPrepTime)
            {
                if (endTime < DashPrepTime + DashReelbackTime)
                {
                    float reelBackSpeedExponent = 2.6f;
                    float reelBackCompletion = Utils.GetLerpValue(0f, 30, endTime - DashPrepTime, true);
                    float reelBackSpeed = MathHelper.Lerp(4f, 16f, MathF.Pow(reelBackCompletion, reelBackSpeedExponent));
                    Vector2 reelBackVelocity = Vector2.UnitY * -reelBackSpeed;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, reelBackVelocity, 0.25f);
                }
                else if (endTime == DashPrepTime + 20)
                    NPC.velocity = Vector2.UnitY * DashVelocity;

                if (endTime >= DashPrepTime + DashReelbackTime && Time % DashScytheRate == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX * 16f, ModContent.ProjectileType<BloodScythe>(), BloodScytheDamage, 0.5f);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX * -16f, ModContent.ProjectileType<BloodScythe>(), BloodScytheDamage, 0.5f);
                    }
                }

                if (endTime > DashPrepTime + DashReelbackTime + DashDuration)
                {
                    NPC.rotation = 0;
                    SetupForNextAttack();
                    return;
                }
            }
        }
        #endregion
    }

    private void SanguineScythes()
    {
        NPC.damage = 0;

        #region Attack Ending
        if (AttackCounter > SanguineTeleportCount)
        {
            if (Time == SanguineAttackEndDelay)
            {
                SoundEngine.PlaySound(Roar, NPC.Center);
                bool left = Target.Center.X > NPC.Center.X;
                NPC.velocity = Vector2.UnitX * (left ? 18 : -18);
                NPC.rotation = MathHelper.Pi / 8f * (left ? 1 : -1);
            }
            if (Time > SanguineAttackEndDelay + SanguineAttackEndDuration)
            {
                NPC.velocity *= 0.8f;
                NPC.rotation *= 0.8f;
            }
            else if (Time >= SanguineAttackEndDelay && Time % SanguineAttackEndIchorRate == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Math.Sign(NPC.velocity.X), -2f), ModContent.ProjectileType<IchorShower>(), IchorShotDamage, 0.5f);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Math.Sign(NPC.velocity.X), -6f), ModContent.ProjectileType<IchorShower>(), IchorShotDamage, 0.5f);
                }
            }

            if (Time > SanguineAttackEndDelay + SanguineAttackEndDuration + (CalamityWorld.death ? 20 : 40))
                SetupForNextAttack();
        }
        #endregion
        #region Attack Start
        else if (Time < 0)
        {
            if (Time == -25)
                NPC.netUpdate = true;

            if (Time == -24)
            {
                SoundEngine.PlaySound(Roar, NPC.Center);

                for (int i = 1; i <= 3; i++)
                {
                    Color color = i switch
                    {
                        1 => Color.Yellow,
                        2 => Color.Orange,
                        _ => Color.Red,
                    };
                    PulseRing ring = new(NPC.Center, NPC.velocity * 0.5f, color, 0f, 1f + i * 0.5f, 24);
                    GeneralParticleHandler.SpawnParticle(ring);
                }
            }
            if (Time == -1)
            {
                Vector2 direction = Target.velocity.SafeNormalize(Vector2.UnitX * Target.direction).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 3f, MathHelper.Pi / 3f));
                float distance = SanguineTeleportDistance;
                AttackPosition = Target.Center + (direction * distance);
                NPC.netUpdate = true;
            }
        }
        #endregion
        #region Teleport
        else
        {
            TeleportDuration = SanguineTeleportDuration;

            Vector2 endPoint = AttackPosition;

            if (Time < (TeleportDuration / 2f))
            {
                if (Time % 4 == 0)
                {
                    Vector2 startPoint = NPC.Center;

                    Vector2 direction = endPoint - startPoint;
                    float curveIntensity = Main.rand.NextFloat(-0.2f, 0.2f);
                    Vector2 perpindicular = direction.RotatedBy(MathHelper.PiOver2);

                    Vector2 controlPoint1 = startPoint + (direction * 0.25f) + (perpindicular * curveIntensity);
                    Vector2 controlPoint2 = startPoint + (direction * 0.75f) + (perpindicular * curveIntensity);

                    BezierCurve path = new BezierCurve(startPoint, controlPoint1, controlPoint2, endPoint);

                    BrainOfCthulhuAfterImage afterimage = new(path, NPC.rotation, Vector2.One, (int)((TeleportDuration * 0.75f) - Time), BoCFrame);
                    BoCAfterImages.Add(afterimage);
                    GeneralParticleHandler.SpawnParticle(afterimage);
                }
                TeleportTime++;
            }
            else if (Time == (TeleportDuration / 2f) && !AttackFlag)
            {
                Vector2 start = NPC.Center;
                NPC.Center = endPoint;
                DisableMultiplayerSmoothing = true;
                AttackFlag = true;
                NPC.netUpdate = true;
            }
            else
            {
                TeleportTime--;
                if (TeleportTime < 0)
                {
                    TeleportTime = 0;
                    Time = -1;
                    AttackFlag = false;
                    AttackCounter++;

                    if (AttackCounter <= SanguineTeleportCount)
                    {
                        SoundEngine.PlaySound(BloodExplosion, NPC.Center);

                        for (int i = 0; i < SanguineScytheCount; i++)
                        {
                            float initalSpeed = 16f;
                            if (CalamityWorld.death && i % 2 == 0)
                                initalSpeed /= 2f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(MathHelper.TwoPi / SanguineScytheCount * i) * initalSpeed, ModContent.ProjectileType<BloodScythe>(), BloodScytheDamage, 0.5f);
                        }
                    }

                    Vector2 direction;
                    if (AttackCounter < SanguineTeleportCount)
                    {
                        direction = Main.rand.NextFloat(0f, MathHelper.TwoPi).ToRotationVector2();
                        AttackPosition = Target.Center + (direction * SanguineTeleportDistance);
                    }
                    else
                    {
                        direction = Vector2.UnitX * (Main.rand.NextBool() ? -1 : 1);
                        AttackPosition = Target.Center + (direction * SanguineFinalTeleportOffset.X) + (Vector2.UnitY * -SanguineFinalTeleportOffset.Y);
                    }

                    NPC.netUpdate = true;

                    BoCAfterImages = [];
                    NPC.Opacity = 1f;
                }
            }

            NPC.Opacity = 1 - (TeleportTime / (TeleportDuration / 2f));
        }
        #endregion
    }

    //Unused
    private void CrimsonEyes()
    {
        #region Attack Start
        if (Time <= 30)
        {
            if (Time >= 6 && Time <= 12)
            {
                if (Time == 6)
                {
                    SoundEngine.PlaySound(Roar, NPC.Center);

                    for (int i = 1; i <= 3; i++)
                    {
                        Color color = i switch
                        {
                            1 => Color.Yellow,
                            2 => Color.Orange,
                            _ => Color.Red,
                        };
                        PulseRing ring = new(NPC.Center, NPC.velocity * 0.5f, color, 0f, 1f + i * 0.5f, 24);
                        GeneralParticleHandler.SpawnParticle(ring);
                    }

                    CalamityUtils.AddScreenshakeAt(NPC.Center, 10f);
                }

                for (int i = 0; i < 12; i++)
                {
                    Point start = Target.Center.ToTileCoordinates() + new Point(Main.rand.Next(-64, 65), 48);
                    for (int j = 0; j < 96; j++)
                    {
                        Point current = start - new Point(0, j);
                        if (!Main.tile[current].IsTileSolid() && Main.tile[current - new Point(0, 1)].IsTileSolid())
                            Dust.NewDust(current.ToWorldCoordinates(0, 0), 16, 16, DustID.Crimstone, 0, 3);
                    }
                }
            }
        }
        #endregion
        #region Attack
        else
        {
            #region Eye Spawning
            if (Time < CrimsonEyeAttackDuration && CalamityUtils.CountProjectiles(ModContent.ProjectileType<CrimsonEye>()) < CrimsonEyeCap && Time % CrimsonEyeRate == 0)
            {
                Vector2 spawnPos = Target.Center;
                int i = 0;
                for (i = 0; i <= 32; i++)
                {
                    spawnPos = Target.Center + Main.rand.NextVector2Circular(256, 256);

                    if (Collision.IsWorldPointSolid(spawnPos))
                        continue;

                    bool alreadyFilled = false;
                    foreach (Projectile p in Main.ActiveProjectiles)
                    {
                        if (p.type != ModContent.ProjectileType<CrimsonEye>())
                            continue;

                        Rectangle hitbox = new((int)spawnPos.X - 50, (int)spawnPos.Y - 18, 100, 36);

                        if (p.Hitbox.Intersects(hitbox))
                        {
                            alreadyFilled = true;
                            break;
                        }
                    }

                    if (alreadyFilled)
                        continue;

                    if (Collision.CanHitLine(spawnPos, 1, 1, Target.position, Target.width, Target.height))
                        break;
                }

                if (i == 32)
                    spawnPos = Target.Center;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, Vector2.Zero, ModContent.ProjectileType<CrimsonEye>(), CrimsonEyeDamage, 0f);
            }
            #endregion

            #region Early Movement
            if (Time < 180)
            {
                float dist = 210;

                Vector2 fromTarget = (NPC.Center - Target.Center).SafeNormalize(Vector2.UnitX);
                Vector2 goalPos = Target.Center + (fromTarget * dist) - (Vector2.UnitY * 32);
                if (NPC.DistanceSQ(goalPos) <= 2048)
                    NPC.velocity *= 0.9f;
                else if (NPC.velocity.LengthSquared() < 16f) //7.5^2
                    NPC.velocity += NPC.DirectionTo(goalPos).SafeNormalize(Vector2.Zero) * 0.1f;
                else
                    NPC.velocity = NPC.DirectionTo(goalPos).SafeNormalize(Vector2.Zero) * 4f;
            }
            #endregion

            #region Scythe Movement + Attack
            else
            {
                if (Time < CrimsonEyeAttackIdleDuration)
                    NPC.velocity *= 0.9f;
                if (Time >= CrimsonEyeAttackIdleDuration + CrimsonEyeAttackSetUpDuration && Time < CrimsonEyeAttackDuration)
                {
                    if (Time == CrimsonEyeAttackIdleDuration + CrimsonEyeAttackSetUpDuration)
                        NPC.velocity = NPC.DirectionTo(Target.Center);

                    bool onSurface = Target.Center.Y / 16f < Main.worldSurface;
                    float speed = onSurface ? 10f : 8f;
                    if (Time < CrimsonEyeAttackIdleDuration + CrimsonEyeAttackSetUpDuration + CrimsonEyeAttackBuildUpDuration)
                    {
                        float lerp = CalamityUtils.SineInEasing((Time - (CrimsonEyeAttackIdleDuration + CrimsonEyeAttackSetUpDuration)) / CrimsonEyeAttackBuildUpDuration, 1);
                        speed = MathHelper.Lerp(0f, speed, lerp);
                    }

                    float turnAmt = TurnAccelerationMultiplier * ((NPC.Distance(Target.Center) - TurnAccelerationDistanceBuffer) / TurnAccelerationDistanceDivisor);

                    NPC.velocity = NPC.velocity.RotateDirectionTowards(NPC.AngleTo(Target.Center), turnAmt) * speed;
                }
                else
                {
                    NPC.velocity *= 0.9f;

                    if (Time == CrimsonEyeAttackDuration)
                    {
                        foreach (Projectile p in Main.ActiveProjectiles)
                        {
                            if (p.type != ModContent.ProjectileType<CrimsonEye>())
                                continue;

                            p.timeLeft = 60;
                        }
                    }

                    if (Time > CrimsonEyeAttackDuration + CrimsonEyeAttackEndDuration)
                        SetupForNextAttack();
                }

                if (Time == CrimsonEyeAttackIdleDuration)
                {
                    SoundEngine.PlaySound(BloodExplosion, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float projCount = 10;
                        for (int i = 0; i < projCount; i++)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CirclingBloodScythe>(), BloodScytheDamage, 0.5f, -1, MathHelper.TwoPi / projCount * i);
                    }
                }
            }
            #endregion
        }
        #endregion
    }

    private void IllusionDash()
    {
        NPC.damage = 0;

        #region Attack Start
        if (Time < IllusionDashTeleportDuration)
        {
            if (Time == 0)
            {
                AttackPosition = Target.Center;
                AttackRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                NPC.netUpdate = true;
            }

            TeleportDuration = IllusionDashTeleportDuration;

            if (Time < (TeleportDuration / 2f))
            {
                if (Time % 4 == 0)
                {
                    Vector2 startPoint = NPC.Center;

                    for (int i = 0; i < (CalamityWorld.death ? 8 : 4); i++)
                    {
                        Vector2 myEndPoint = AttackPosition + ((AttackRotation + (CalamityWorld.death ? MathHelper.PiOver4 : MathHelper.PiOver2) * i).ToRotationVector2() * IllusionDashTeleportDistance);
                        Vector2 direction = myEndPoint - startPoint;

                        float curveIntensity = Main.rand.NextFloat(-0.2f, 0.2f);
                        Vector2 perpindicular = direction.RotatedBy(MathHelper.PiOver2);

                        Vector2 controlPoint1 = startPoint + (direction * 0.25f) + (perpindicular * curveIntensity);
                        Vector2 controlPoint2 = startPoint + (direction * 0.75f) + (perpindicular * curveIntensity);

                        BezierCurve path = new BezierCurve(startPoint, controlPoint1, controlPoint2, myEndPoint);

                        BrainOfCthulhuAfterImage afterimage = new(path, NPC.rotation, Vector2.One, (int)((TeleportDuration * 0.75f) - Time), BoCFrame);
                        BoCAfterImages.Add(afterimage);
                        GeneralParticleHandler.SpawnParticle(afterimage);
                    }
                }
                TeleportTime++;
            }
            else if (Time == (TeleportDuration / 2f) && !AttackFlag)
            {
                AttackFlag = true;

                for (int i = 0; i < (CalamityWorld.death ? 8 : 4); i++)
                {
                    float rot = AttackRotation + (CalamityWorld.death ? MathHelper.PiOver4 : MathHelper.PiOver2) * i;
                    Vector2 spawnPos = AttackPosition + (rot.ToRotationVector2() * IllusionDashTeleportDistance);
                    if (i == 0)
                    {
                        NPC.Center = spawnPos;
                        DisableMultiplayerSmoothing = true;
                    }
                    else if (Main.netMode != NetmodeID.MultiplayerClient)
                        NPC.NewNPCDirect(NPC.GetSource_FromThis(), spawnPos, ModContent.NPCType<BrainIllusion>(), 0, 15, 30, rot).target = NPC.target;
                }

                if(Main.netMode != NetmodeID.SinglePlayer)
                {
                    List<int> targets = GetAllValidTargets(NPC.Center);
                    targets.Remove(Target.whoAmI);

                    foreach (int who in targets)
                    {
                        Player p = Main.player[who];
                        float baseRot = Main.rand.NextFloat(0, MathHelper.TwoPi);
                        for (int i = 0; i < (CalamityWorld.death ? 4 : 2); i++)
                        {
                            float rot = baseRot + (CalamityWorld.death ? MathHelper.PiOver2 : MathHelper.Pi) * i;
                            Vector2 spawnPos = p.Center + (rot.ToRotationVector2() * IllusionDashTeleportDistance);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                NPC.NewNPCDirect(NPC.GetSource_FromThis(), spawnPos, ModContent.NPCType<BrainIllusion>(), 0, 15, 30, rot).target = p.whoAmI;
                        }
                    }
                }

                NPC.netUpdate = true;
            }
            else
                TeleportTime--;

            NPC.Opacity = 1 - (TeleportTime / (TeleportDuration / 2f));
        }
        #endregion
        else
        {
            float startTime = Time - IllusionDashTeleportDuration;

            if (startTime == 0)
            {
                    AttackPosition = NPC.Center;
                    TeleportTime = 0;
                    AttackFlag = false;

                BoCAfterImages = [];
                NPC.Opacity = 1f;

                GenericSparkle sparkle = new(NPC.Center + new Vector2(16, -8), Vector2.Zero, Color.Yellow, Color.Orange, 2f, 16, needed: true);
                GeneralParticleHandler.SpawnParticle(sparkle);

                NPC.netUpdate = true;

                foreach (NPC n in Main.ActiveNPCs)
                    if (n.type == ModContent.NPCType<BrainIllusion>())
                        n.netUpdate = true;
            }
            if (startTime < 30)
            {
                float lerp = startTime / 30f;
                float circleDist = MathHelper.Lerp(IllusionDashTeleportDistance, IllusionDashCloseInDistance, CalamityUtils.SineOutEasing(lerp, 1));
                NPC.Center = Vector2.Lerp(AttackPosition, Target.Center + (AttackRotation.ToRotationVector2() * circleDist), lerp);
                AttackRotation += MathHelper.Lerp(0f, IllusionDashStartingSpinSpeed, CalamityUtils.SineInEasing(lerp, 1));
                DisableMultiplayerSmoothing = true;
            }
            else if (startTime <= 30 + IllusionDashSpinDuration)
            {
                NPC.Center = Target.Center + AttackRotation.ToRotationVector2() * IllusionDashCloseInDistance;
                DisableMultiplayerSmoothing = true;
                AttackRotation += MathHelper.Lerp(IllusionDashStartingSpinSpeed, 0f, CalamityUtils.SineOutEasing((startTime - 30) / (float)IllusionDashSpinDuration, 1));
            }
            else if (startTime < 30 + IllusionDashSpinDuration + 30)
            {
                if (startTime < 30 + IllusionDashSpinDuration + 15)
                {
                    float reelBackSpeedExponent = 2.6f;
                    float reelBackCompletion = Utils.GetLerpValue(0f, 30, startTime - 130, true);
                    float reelBackSpeed = MathHelper.Lerp(2.5f, 16f, MathF.Pow(reelBackCompletion, reelBackSpeedExponent));
                    Vector2 reelBackVelocity = (AttackRotation + MathHelper.Pi).ToRotationVector2() * -reelBackSpeed;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, reelBackVelocity, 0.25f);
                }
                else if (startTime == 30 + IllusionDashSpinDuration + 15)
                    NPC.velocity = (AttackRotation + MathHelper.Pi).ToRotationVector2() * -32;
                else
                    NPC.velocity *= 0.9f;
            }
            else if (startTime <= 30 + IllusionDashSpinDuration + 30 + IllusionDashFakeoutTeleportDuration) //176
            {
                NPC.velocity = Vector2.Zero;

                if (startTime == 30 + IllusionDashSpinDuration + 30)
                {
                    AttackPosition = Target.Center;
                    AttackRotation = AttackRotation + MathHelper.Pi + Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
                    NPC.netUpdate = true;
                }

                float wrappedCounter = startTime - (30 + IllusionDashSpinDuration + 30);

                TeleportDuration = IllusionDashFakeoutTeleportDuration;

                Vector2 endPoint = AttackPosition + (AttackRotation.ToRotationVector2() * 270);

                if (wrappedCounter < (TeleportDuration / 2f))
                {
                    if (wrappedCounter % 2 == 0)
                    {
                        Vector2 startPoint = NPC.Center;

                        Vector2 direction = endPoint - startPoint;

                        float curveIntensity = Main.rand.NextFloat(-0.2f, 0.2f);
                        Vector2 perpindicular = direction.RotatedBy(MathHelper.PiOver2);

                        Vector2 controlPoint1 = startPoint + (direction * 0.25f) + (perpindicular * curveIntensity);
                        Vector2 controlPoint2 = startPoint + (direction * 0.75f) + (perpindicular * curveIntensity);

                        BezierCurve path = new BezierCurve(startPoint, controlPoint1, controlPoint2, endPoint);

                        BrainOfCthulhuAfterImage afterimage = new(path, NPC.rotation, Vector2.One, (int)((TeleportDuration * 0.75f) - wrappedCounter), BoCFrame);
                        BoCAfterImages.Add(afterimage);
                        GeneralParticleHandler.SpawnParticle(afterimage);
                    }
                    TeleportTime++;
                }
                else if (wrappedCounter == (int)(TeleportDuration / 2f) && !AttackFlag)
                {
                    AttackFlag = true;
                    NPC.Center = endPoint;
                    DisableMultiplayerSmoothing = true;
                }
                else
                {
                    TeleportTime--;
                    if (TeleportTime <= 0)
                    {
                        TeleportTime = 0;
                        BoCAfterImages = [];
                        ResetAttackValues();
                        NPC.Opacity = 1f;
                        NPC.netUpdate = true;
                        AttackRotation = NPC.AngleTo(Target.Center);
                    }
                }

                NPC.Opacity = 1 - (TeleportTime / (TeleportDuration / 2f));
            }
            else if (startTime <= 150 + IllusionDashSpinDuration + 30 + IllusionDashFakeoutTeleportDuration + 30)
            {
                NPC.velocity *= 0.9f;
                if (startTime % 15 == 5 && startTime > 60 + IllusionDashSpinDuration + 30 + IllusionDashFakeoutTeleportDuration)
                {
                    Vector2 dir = NPC.DirectionTo(Target.Center);

                    for (int i = 0; i < (CalamityWorld.death ? 2 : 1); i++)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 initialDir = dir.RotatedBy(MathHelper.Pi + Main.rand.NextFloat(-0.01f, 0.01f));
                            Vector2 spawnPos = NPC.Center + (dir.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-NPC.width, NPC.width) - (dir * 48));
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, initialDir * Main.rand.NextFloat(6f, 8f), ProjectileID.BloodNautilusShot, BloodShotDamage, 0.5f, ai0: dir.ToRotation() + MathHelper.TwoPi, ai1: initialDir.ToRotation());
                        }
                        NPC.velocity -= dir;
                    }
                }
            }
            else if (startTime >= 180 + IllusionDashSpinDuration + 30 + IllusionDashFakeoutTeleportDuration + 30)
            {
                foreach (NPC n in Main.ActiveNPCs)
                    if (n.type == ModContent.NPCType<BrainIllusion>())
                        n.active = false;

                SetupForNextAttack();
            }
        }
    }

    private void IllusionTrick()
    {
        if (Time >= 90)
        {
            if (Time == 90)
            {
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (!p.friendly)
                        continue;

                    p.Calamity().IgnoreBoCIllusions = true;
                }

                int brainAngleSlot = Main.rand.Next(0, IllusionTrickAngleGroups);
                int brainDistSlot = Main.rand.Next(0, IllusionTrickGroupSize);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    for (int a = 0; a < IllusionTrickAngleGroups; a++)
                        for (int d = 0; d < IllusionTrickGroupSize; d++)
                        {
                            if (a != brainAngleSlot || d != brainDistSlot)
                                NPC.NewNPC(NPC.GetSource_FromThis(), 0, 0, ModContent.NPCType<FalseBrain>(), 0, MathHelper.TwoPi / IllusionTrickAngleGroups * a, FalseBrain.TimeDivisor / IllusionTrickGroupSize * d);
                        }

                AttackTime = (int)(FalseBrain.TimeDivisor / IllusionTrickGroupSize * brainDistSlot);
                AttackRotation = MathHelper.TwoPi / IllusionTrickAngleGroups * brainAngleSlot;
                AttackFlag = false;

                if(Main.netMode == NetmodeID.SinglePlayer)
                    AttackPosition = Target.Center;
                else
                {
                    List<int> nearbyPlayers = GetAllValidTargets(NPC.Center);
                    Vector2 averagePosition = Vector2.zeroVector;
                    foreach(int p in nearbyPlayers)
                    {
                        TargetsSet.Add(p);
                        averagePosition += Main.player[p].Center;
                    }

                    averagePosition /= (float)nearbyPlayers.Count;

                    AttackPosition = averagePosition;
                }

                NPC.ShowNameOnHover = false;
                NPC.netUpdate = true;
            }
            else
            {
                Vector2 goalPos;
                if (Main.netMode == NetmodeID.SinglePlayer)
                    goalPos = Target.Center;
                else
                {
                    Vector2 averagePosition = Vector2.zeroVector;
                    foreach (int who in TargetsSet)
                        averagePosition += Main.player[who].Center;

                    averagePosition /= (float)TargetsSet.Count;

                    goalPos = averagePosition;
                }

                float distSq = AttackPosition.DistanceSQ(goalPos);

                if (distSq > 90000) //300^2
                {
                    float dist = MathF.Sqrt(distSq);
                    Vector2 dir = (goalPos - AttackPosition).SafeNormalize(Vector2.zeroVector);
                    AttackPosition += dir * (dist - 300) / 60f;
                }
            }

            NPC.damage = 0;
            NPC.dontTakeDamage = false;

            if (AttackFlag)
            {
                if (AttackCounter == 0)
                {
                    NPC.ShowNameOnHover = true;
                    NPC.velocity = NPC.DirectionFrom(Target.Center) * 4f;
                }
                else
                    NPC.velocity *= 0.95f;

                if (AttackCounter >= IllusionTrickStunDuration)
                {
                    SetupForNextAttack();
                    NPC.Opacity = 1f;
                    TeleportTime = 0;
                    return;
                }

                AttackCounter++;
            }
            else if (Time >= IllusionTrickTimeLimit) //Players have failed to find the real BoC within the time limit
            {
                if (Time == IllusionTrickTimeLimit)
                {
                    var targets = GetAllValidTargets(NPC.Center);

                    AttackList.Clear();
                    foreach (int p in targets)
                        AttackList.Add((byte)p);

                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        if (n.type != ModContent.NPCType<FalseBrain>())
                            continue;

                        n.ModNPC<FalseBrain>().BeenHit = true;
                        n.netUpdate = true;
                    }

                    for (int i = 1; i <= 3; i++)
                    {
                        Color color = i switch
                        {
                            1 => Color.Yellow,
                            2 => Color.Orange,
                            _ => Color.Red,
                        };
                        PulseRing ring = new(NPC.Center, NPC.velocity * 0.5f, color, 0f, 1f + i * 0.5f, 24);
                        GeneralParticleHandler.SpawnParticle(ring);
                    }

                    NPC.netUpdate = true;
                }
                else if (Time % 30 == 0)
                {
                    if (AttackList.Count > 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TelekineticBlast>(), 0, 0f, -1, AttackList[0], 0, NPC.whoAmI);

                        AttackList.RemoveAt(0);
                    }
                    else
                    {
                        Vector2 direction = Target.velocity.SafeNormalize(Vector2.UnitX * Target.direction).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 3f, MathHelper.Pi / 3f));
                        float distance = DefaultTeleportDistance;
                        NPC.damage = NPC.defDamage;
                        AttackPosition = Target.Center + (direction * distance);
                        BoCAfterImages = [];
                        NPC.Opacity = 1f;
                        TeleportTime = 0;
                        Time = ChaseTime - 1;
                        NPC.netUpdate = true;
                        ResetAttackValues();

                        AIState = BrainAIState.Phase2Idle;
                    }
                }
            }
            else
            {
                CachedRatio = (float)Math.Cos(AttackTime * MathHelper.TwoPi / FalseBrain.TimeDivisor);

                float lerp = CalamityUtils.SineInOutEasing(MathHelper.Clamp((Time - 150) / 30f, 0f, 1f), 1);
                float baseDist = 240;
                float circleDist = 480;
                if (Time - 150 < 30f)
                {
                    baseDist = MathHelper.Lerp(480, 240, lerp);
                    circleDist = MathHelper.Lerp(240, 480, lerp);
                }
                NPC.Center = AttackPosition + Vector2.UnitX.RotatedBy(AttackRotation) * (baseDist + (circleDist * ((float)Math.Sin(-AttackTime * MathHelper.TwoPi / FalseBrain.TimeDivisor) / 2f + 0.5f)));
                NPC.Center += Vector2.UnitX.RotatedBy(AttackRotation + MathHelper.PiOver2) * (90 * (float)Math.Cos(AttackTime * MathHelper.TwoPi / FalseBrain.TimeDivisor));
                NPC.Opacity = 1f;
                DisableMultiplayerSmoothing = true;
                AttackCounter = 0;
            }
        }
        else if (Time <= 60)
        {
            TeleportDuration = 60;
            TeleportTime++;
            NPC.Opacity = 1 - (Time / 60f);

            if (Time < 50)
            {
                Vector2 startPoint = NPC.Center;
                Vector2 endPoint = NPC.Center + Vector2.UnitX.RotatedBy(Main.rand.NextFloat(0, MathHelper.TwoPi)) * Main.rand.NextFloat(240, 480);

                Vector2 direction = endPoint - startPoint;
                float curveIntensity = Main.rand.NextFloat(-0.2f, 0.2f);
                Vector2 perpindicular = direction.RotatedBy(MathHelper.PiOver2);

                Vector2 controlPoint1 = startPoint + (direction * 0.25f) + (perpindicular * curveIntensity);
                Vector2 controlPoint2 = startPoint + (direction * 0.75f) + (perpindicular * curveIntensity);

                BezierCurve path = new BezierCurve(startPoint, controlPoint1, controlPoint2, endPoint);

                BrainOfCthulhuAfterImage afterimage = new(path, NPC.rotation, Vector2.One, (int)(60 - Time), BoCFrame);
                BoCAfterImages.Add(afterimage);
                GeneralParticleHandler.SpawnParticle(afterimage);
            }
        }
        else
        {
            NPC.Opacity = 0f;
            BoCAfterImages = [];
            NPC.damage = 0;
            NPC.dontTakeDamage = true;
        }

        if (Time >= 180)
            AttackTime++;
        else if (Time >= 150)
            AttackTime += (Time - 150) / 30f;
    }

    #endregion

    private void DeathAnimation()
    {
        if (Time == 0)
            NPC.velocity = NPC.DirectionFrom(Target.Center) * 6f;
        else
            NPC.velocity *= 0.95f;

        NPC.damage = 0;

        NPC.rotation = MathHelper.Pi / 24f * NPC.oldVelocity.X;
        TeleportTime *= 0.6f;
        if (TeleportTime < 0.005f)
            TeleportTime = 0;
        BoCDrawOffset *= 0.6f;
        NPC.Opacity = BringOpacityTo(NPC.Opacity, 1, 0.1f);

        (float angle, Vector2 offset, int time)[] bloodGushingData = [
            (MathHelper.Pi / 6f, new(18, 12), 60),
            (MathHelper.Pi, new(-30, -10), 150),
            (-MathHelper.Pi / 4f, new(20, -40), 210),
            (MathHelper.Pi + MathHelper.Pi / 4f, new(-20, -40), 240),
            (MathHelper.Pi / 1.75f, new(-5, 22), 260)
        ];
        float EndTime = 270f;

        for (int i = 0; i < bloodGushingData.Length; i++)
        {
            if (Time >= bloodGushingData[i].time)
            {
                if (Time == bloodGushingData[i].time)
                {
                    if(i == 3)
                        SoundEngine.PlaySound(Death, NPC.Center);

                    Vector2 bloodDir = bloodGushingData[i].angle.ToRotationVector2();
                    BloodParticle2 p2 = new(NPC.Center + bloodGushingData[i].offset.RotatedBy(NPC.rotation), bloodDir * 7.5f, 16, 0.5f, Color.Red);
                    GeneralParticleHandler.SpawnParticle(p2);
                    NPC.velocity = bloodDir * -4f;

                    SoundStyle bleed = BloodShot with
                    {
                        Pitch = i / (float)(bloodGushingData.Length - 1)
                    };
                    SoundEngine.PlaySound(bleed, NPC.Center);

                    Main.LocalPlayer.SetScreenshake(1f);
                }

                for (int j = 0; j < 2; j++)
                {
                    BloodParticle p = new(NPC.Center + bloodGushingData[i].offset.RotatedBy(NPC.rotation), (bloodGushingData[i].angle + Main.rand.NextFloat(-MathHelper.Pi / 10f, MathHelper.Pi / 10f)).ToRotationVector2() * Main.rand.NextFloat(5f, 10f), 32, 1f, Color.Red);
                    GeneralParticleHandler.SpawnParticle(p);
                }
            }

        }

        if (Time >= EndTime)
        {
            SoundEngine.PlaySound(BloodExplosion, NPC.Center);
            SoundEngine.PlaySound(BloodShot, NPC.Center);

            Main.LocalPlayer.SetScreenshake(2f);

            int pCount = 10;
            for (int i = 0; i < pCount; i++)
            {
                float initalSpeed = 24f;
                Vector2 pVelo = Vector2.UnitX.RotatedBy(MathHelper.TwoPi / pCount * i) * initalSpeed;

                for (int j = 0; j < 2; j++)
                {
                    BloodParticle p = new(NPC.Center, pVelo.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 6f, MathHelper.Pi / 6f)) * Main.rand.NextFloat(0.5f, 1f), 32, 1f, Color.Red);
                    GeneralParticleHandler.SpawnParticle(p);
                }
                BloodParticle2 p2 = new(NPC.Center, pVelo * 0.75f, 16, 0.5f, Color.Red);
                GeneralParticleHandler.SpawnParticle(p2);
            }

            NPC.dontTakeDamage = false;
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.StrikeInstantKill();
        }

        float animationCompletion = Time / EndTime;
        NPC.frameCounter += 2 * animationCompletion;
        BoCDrawOffset = Main.rand.NextVector2Circular(4, 4) * animationCompletion;

        if (Main.rand.NextFloat(0.5f, 1f) < animationCompletion)
        {
            Vector2 edgeBloodDir = Main.rand.NextVector2CircularEdge(1, 1);
            BloodParticle b = new(NPC.Center + (edgeBloodDir * NPC.Size * 0.75f), edgeBloodDir.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 10f, MathHelper.Pi / 10f)) * Main.rand.NextFloat(2f, 4f), 16, 0.75f, Color.Red);
            GeneralParticleHandler.SpawnParticle(b);
        }
    }

    #endregion

    public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        binaryWriter.Write((byte)PreviousAttack);

        if (AIState == BrainAIState.Stunned || AIState >= BrainAIState.Phase2TransitionClosed)
        {
            binaryWriter.Write(TeleportTime);
            binaryWriter.Write(TeleportDuration);
        }

        if (AIState <= BrainAIState.SurfaceSpawnAnimation)
        {
            binaryWriter.Write(SpawnTime);
            binaryWriter.Write(SpawnDelay);
        }

        binaryWriter.WriteFlags(OnSecondCreeperPhase, isNegative, AttackFlag);

        binaryWriter.Write(AttackRotation);
        binaryWriter.Write(AttackTime);
        binaryWriter.Write(AttackCounter);

        binaryWriter.WritePackedWorldPosition(AttackPosition);

        binaryWriter.Write((byte)availableAttacks.Count);

        binaryWriter.Write(availableAttacks.Select(e => (byte)e).ToArray());

        binaryWriter.Write((byte)AttackList.Count);
        binaryWriter.Write(AttackList.ToArray());
    }

    public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
    {
        PreviousAttack = (BrainAIState)binaryReader.ReadByte();

        if (AIState == BrainAIState.Stunned || AIState >= BrainAIState.Phase2TransitionClosed)
        {
            TeleportTime = binaryReader.ReadSingle();
            TeleportDuration = binaryReader.ReadSingle();
        }

        if (AIState <= BrainAIState.SurfaceSpawnAnimation)
        {
            SpawnTime = binaryReader.ReadSingle();
            SpawnDelay = binaryReader.ReadInt32();
        }

        binaryReader.ReadFlags(out OnSecondCreeperPhase, out isNegative, out AttackFlag);

        AttackRotation = binaryReader.ReadSingle();
        AttackTime = binaryReader.ReadSingle();
        AttackCounter = binaryReader.ReadInt32();

        AttackPosition = binaryReader.ReadPackedWorldPosition();

        int availableLength = binaryReader.ReadByte();
        availableAttacks = binaryReader.ReadBytes(availableLength).Select(e => (BrainAIState)e).ToList();

        byte attackLength = binaryReader.ReadByte();
        AttackList = binaryReader.ReadBytes(attackLength).ToList();
    }

    public override bool? CanBeHitByProjectile(Mod mod, Projectile projectile)
    {
        if (AIState == BrainAIState.IllusionTrick && !AttackFlag && projectile.Calamity().IgnoreBoCIllusions)
            return false;
        return base.CanBeHitByProjectile(mod, projectile);
    }

    public override void ModifyHitByItem(Mod mod, Player player, Item item, ref NPC.HitModifiers modifiers)
    {
        if (AIState != BrainAIState.DeathAnimation)
            modifiers.SetMaxDamage(NPC.life - 1);
    }

    public override void ModifyHitByProjectile(Mod mod, Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        if (AIState != BrainAIState.DeathAnimation)
            modifiers.SetMaxDamage(NPC.life - 1);
    }

    public override void HitEffect(Mod mod, NPC.HitInfo hit)
    {
        if (AIState != BrainAIState.DeathAnimation && (NPC.life + 1) <= hit.Damage)
        {
            TriggerDeathAnimation();
            return;
        }

        if (AIState == BrainAIState.IllusionTrick && Time < 960)
        {
            AttackFlag = true;
            NPC.netUpdate = true;
        }
    }

    private void TriggerDeathAnimation()
    {
        NPC.life = 1;
        NPC.lifeRegen = 0;
        NPC.BossBar = null;
        NPC.dontTakeDamage = true;

        if (AIState == BrainAIState.Stunned || AIState == BrainAIState.IllusionTrick)
            TeleportTime = 0;

        foreach (NPC n in Main.ActiveNPCs)
        {
            if (n.type != ModContent.NPCType<FalseBrain>())
                continue;

            n.ModNPC<FalseBrain>().BeenHit = true;
        }

        AIState = BrainAIState.DeathAnimation;
        ResetAttackValues();
        Time = 0;
        NPC.netUpdate = true;
    }

    public override void FindFrame(Mod mod, int frameHeight)
    {
        if (BoCFrame == Rectangle.Empty)
            BoCFrame = TextureAssets.Npc[NPCID.BrainofCthulhu].Frame(verticalFrames: 8);

        if (NPC.frameCounter == 0)
            BoCFrame.Y += frameHeight;

        if (AIState <= BrainAIState.Phase2TransitionClosed)
        {
            if (BoCFrame.Y > frameHeight * 3)
                BoCFrame.Y = 0;
            return;
        }
        if (BoCFrame.Y < frameHeight * 4)
            BoCFrame.Y = frameHeight * 4;

        if (BoCFrame.Y > frameHeight * 7)
            BoCFrame.Y = frameHeight * 4;
    }

    public override bool PreDraw(Mod mod, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        bool phase1 = AIState <= BrainAIState.Phase2TransitionOpen;
        bool drawBrain = true;

        if (phase1)
        {
            List<NPC> creepers = Main.npc.Where(n => n.active && n.type == NPCID.Creeper).ToList();
            creepers.Sort((a, b) => b.DistanceSQ(NPC.Center).CompareTo(a.DistanceSQ(NPC.Center)));

            var tendrils = BrainOfCthulhuSystem.VerletTendrils.ToList();
            tendrils.Sort((a, b) => {
                int bIndex = creepers.IndexOf(Main.npc[b.creeper]);
                if (bIndex == -1)
                    bIndex = int.MaxValue;
                int aIndex = creepers.IndexOf(Main.npc[a.creeper]);
                if (aIndex == -1)
                    aIndex = int.MaxValue;
                return bIndex.CompareTo(aIndex);
            });

            foreach (var v in tendrils)
            {
                List<VerletSimulatedSegment> curvePoints = v.tendril;
                if (curvePoints is null)
                    continue;

                NPC creeper = Main.npc[v.creeper];
                if (creeper == null || !creeper.active || creeper.type != NPCID.Creeper)
                    creeper = null;

                float glowIntensity = creeper == null ? 0 : creeper.AIOverride<CreeperAI>().ConnectionOpacity;
                Color ichorLess = Color.Lerp(Color.Transparent, Color.OrangeRed * 0.333f, glowIntensity);
                Color ichorful = Color.Lerp(Color.OrangeRed * 0.25f, Color.Orange * 0.666f, glowIntensity);

                for (int i = 0; i < curvePoints.Count; i++)
                {
                    Vector2 start = curvePoints[i].position;
                    Vector2 end = i == curvePoints.Count - 1 ? (creeper == null ? curvePoints[i].position : creeper.Center) : curvePoints[i + 1].position;
                    Vector2 center = (end + start) / 2f;
                    start -= Main.screenPosition;
                    end -= Main.screenPosition;

                    float rotation = (end - start).ToRotation() - MathHelper.PiOver2;

                    float ichorRatio = 0;

                    if (creeper != null)
                    {
                        float flowTime = creeper.AIOverride<CreeperAI>().FlowTime;
                        float flowAmt = creeper.AIOverride<CreeperAI>().FlowAmount;
                        ichorRatio = CalamityUtils.ExpInEasing((float)Math.Sin((flowTime + (i * flowAmt)) * 2f) / 2f + 0.5f, 1);
                    }

                    Color glowColor = Color.Lerp(ichorLess, ichorful, ichorRatio);

                    float dist = Vector2.Distance(start, end);
                    Vector2 tendrilScale = new(1f + (0.5f * (ichorRatio * (1 + (glowIntensity * 0.5f)))), dist / BrainOfCthulhuSystem.tendril.Height());

                    spriteBatch.Draw(BrainOfCthulhuSystem.tendril.Value, start, null, Lighting.GetColor(center.ToTileCoordinates()) * NPC.Opacity, rotation, BrainOfCthulhuSystem.tendril.Size() * Vector2.UnitX * 0.5f, tendrilScale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(BrainOfCthulhuSystem.GetTendrilGlow(), start, null, glowColor * NPC.Opacity, rotation, BrainOfCthulhuSystem.GetTendrilGlow().Size() * Vector2.UnitX * 0.5f, tendrilScale, SpriteEffects.None, 0f);
                }
            }

            foreach (NPC creeper in creepers)
            {
                float glowIntensity = creeper.AIOverride<CreeperAI>().ConnectionOpacity;

                spriteBatch.Draw(BrainOfCthulhuSystem.GetCreeperGlow(), creeper.Center - Main.screenPosition, null, Color.Orange * glowIntensity, creeper.rotation, TextureAssets.Npc[NPCID.Creeper].Size() * 0.5f, creeper.scale * 1.15f, 0, 0);

                spriteBatch.Draw(TextureAssets.Npc[NPCID.Creeper].Value, creeper.Center - Main.screenPosition, null, Lighting.GetColor(creeper.Center.ToTileCoordinates()).MultiplyRGB(Color.Lerp(Color.White, new Color(255, 180, 180), glowIntensity)), creeper.rotation, TextureAssets.Npc[NPCID.Creeper].Size() * 0.5f, creeper.scale, 0, 0);
            }
        }
        else
        {
            List<NPC> falseBrains = Main.npc.Where(n => n.active && n.type == ModContent.NPCType<FalseBrain>()).ToList();
            if (falseBrains.Count > 0)
            {
                drawBrain = false;

                falseBrains.Add(NPC);
                falseBrains.Sort((Comparison<NPC>)((a, b) =>
                {
                    float aValue;
                    float bValue;

                    if (a.ModNPC is FalseBrain falseA)
                        aValue = falseA.DrawPriority;
                    else
                        aValue = a.AIOverride<BrainOfCthulhuAI>().CachedRatio;

                    if (b.ModNPC is FalseBrain falseB)
                        bValue = falseB.DrawPriority;
                    else
                        bValue = b.AIOverride<BrainOfCthulhuAI>().CachedRatio;

                    return aValue.CompareTo(bValue);
                }));

                foreach (NPC n in falseBrains)
                {
                    if (n.ModNPC is FalseBrain falseBrain)
                        falseBrain.DrawSelf(spriteBatch, screenPos, Lighting.GetColor(n.Center.ToTileCoordinates()));
                    else if (AIState == BrainAIState.DeathAnimation)
                        drawBrain = true;
                    else
                        DrawBrainLikeFakes(spriteBatch, n);
                }
            }
        }

        if (drawBrain)
            DrawBrain(spriteBatch, NPC);

        return false;
    }

    public static int GetBrainOfCthuluCreepersCountRevDeath() => CalamityWorld.death ? 27 : 18;

    public static List<int> GetAllValidTargets(Vector2 brainPosition)
    {
        List<int> validTargets = [];

        foreach(Player p in Main.ActivePlayers)
        {
            if(ValidateTarget(p, brainPosition))
                validTargets.Add(p.whoAmI);
        }

        return validTargets;
    }

    public static bool ValidateTarget(Player p, Vector2 brainPosition) => !p.dead && p.ZoneCrimson && p.Center.DistanceSQ(brainPosition) <= DespawnRangeSQ;

    private void SelectNewTarget()
    {
        CalamityTargetingParameters options = CalamityTargetingParameters.BossDefaults;
        options.aggroRatio = -1f;
        options.finishThemOff = true;
        options.maxSearchRange = DespawnRange;
        options.targetType = NPCTargetType.ForceSwitch;

        var available = GetAllValidTargets(NPC.Center);
        if (available.Any(p => !TargetsSet.Contains(p)))
            options.excludedPlayers = TargetsSet;
        else
        {
            TargetsSet.Clear();
            options.excludedPlayers = [NPC.target];
        }

        NPC.CalamityTargeting(options);

        TargetsSet.Add(NPC.target);
    }

    internal static float BringOpacityTo(float currentOpacity, float goalOpacity, float changeAmount = 0.025f)
    {
        if (currentOpacity == goalOpacity)
            return goalOpacity;

        if (currentOpacity < goalOpacity)
        {
            currentOpacity += changeAmount;
            if (currentOpacity >= goalOpacity)
                return goalOpacity;
            else
                return currentOpacity;
        }
        else
        {
            currentOpacity -= changeAmount;
            if (currentOpacity <= goalOpacity)
                return goalOpacity;
            else
                return currentOpacity;
        }
    }

    private static void DrawBrain(SpriteBatch spriteBatch, NPC brain)
    {
        BrainOfCthulhuAI ai = brain.AIOverride<BrainOfCthulhuAI>();
        bool phase1 = ai.AIState < BrainAIState.Phase2TransitionClosed;

        Vector2 drawPos = brain.Center + ai.BoCDrawOffset + (Vector2.UnitY * 16) - Main.screenPosition;
        Vector2 scale = Vector2.One;

        if (!phase1 && ai.TeleportTime != 0)
        {
            ai.BoCAfterImages.RemoveAll(p => p.Time > p.Lifetime);
            foreach (Particle p in ai.BoCAfterImages)
                p.CustomDraw(spriteBatch);

            //Color glowColor = Color.White * (teleportCounter / 30f);
            scale = Vector2.Lerp(Vector2.One, new Vector2(0.5f + ((float)Math.Cos(ai.Time / (ai.TeleportDuration / 2f) * MathHelper.TwoPi) / 2f + 0.5f), 0.5f + ((float)Math.Sin(ai.Time / (ai.TeleportDuration / 2f) * MathHelper.TwoPi) / 2f + 0.5f)), CalamityUtils.SineInOutEasing(ai.TeleportTime / (ai.TeleportDuration / 2f), 1));

            spriteBatch.Draw(TextureAssets.Npc[NPCID.BrainofCthulhu].Value, drawPos, ai.BoCFrame, Lighting.GetColor(brain.Center.ToTileCoordinates()) * brain.Opacity, brain.rotation, ai.BoCFrame.Size() * 0.5f, scale * brain.scale, 0, 0);
            //spriteBatch.Draw(GetBrainGlow(), drawPos, NPC.frame, glowColor, NPC.rotation, NPC.frame.Size() * 0.5f, scale, 0, 0);
        }
        else
            spriteBatch.Draw(TextureAssets.Npc[NPCID.BrainofCthulhu].Value, drawPos, ai.BoCFrame, Lighting.GetColor(brain.Center.ToTileCoordinates()) * brain.Opacity, brain.rotation, ai.BoCFrame.Size() * 0.5f, scale * brain.scale, 0, 0);
    }

    private static void DrawBrainLikeFakes(SpriteBatch spriteBatch, NPC brain)
    {
        BrainOfCthulhuAI ai = brain.AIOverride<BrainOfCthulhuAI>();

        Vector2 scaleDistort = new Vector2((float)Math.Cos(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi * 2) / 2f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi * 2) / 2f);

        int spawnTime = 150 - ((int)ai.Time);
        float startLerp = spawnTime / 60f;

        Color drawColor = Lighting.GetColor(brain.Center.ToTileCoordinates());

        if (spawnTime > 0)
        {
            drawColor *= (1 - startLerp);
            scaleDistort *= startLerp;
        }
        else
            scaleDistort = Vector2.Zero;

        spriteBatch.Draw(TextureAssets.Npc[NPCID.BrainofCthulhu].Value, brain.Center + (Vector2.UnitY * 16) - Main.screenPosition, ai.BoCFrame, drawColor, brain.rotation, ai.BoCFrame.Size() * 0.5f, (Vector2.One + scaleDistort) * brain.scale, 0, 0);
    }

    private void SetupForNextAttack()
    {
        Player target = Main.player[NPC.target];

        Vector2 direction = target.velocity.SafeNormalize(Vector2.UnitX * target.direction).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 3f, MathHelper.Pi / 3f));
        float distance = DefaultTeleportDistance;
        AttackPosition = target.Center + (direction * distance);
        BoCAfterImages = [];
        Time = ChaseTime - 1;
        ResetAttackValues();
        NPC.netUpdate = true;

        if (availableAttacks.Count != 0)
        {
            if (availableAttacks[0] != BrainAIState.Phase2Idle)
                AttackCounter = 4;
            else
                availableAttacks.RemoveAt(0);
        }

        AIState = BrainAIState.Phase2Idle;
    }

    private void ResetAttackValues()
    {
        isNegative = false;
        AttackRotation = 0;
        AttackTime = 0;
        AttackFlag = false;
        AttackPosition = Vector2.Zero;
        AttackCounter = 0;
    }
}
