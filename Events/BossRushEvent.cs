using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Enums;
using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Skies;
using CalamityMod.Systems;
using CalamityMod.UI.DraedonSummoning;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using ApolloBoss = CalamityMod.NPCs.ExoMechs.Apollo.Apollo;
using ArtemisBoss = CalamityMod.NPCs.ExoMechs.Artemis.Artemis;

namespace CalamityMod.Events
{
    // TODO -- This can be made into a ModSystem with simple OnModLoad and Unload hooks.
    public class BossRushEvent
    {
        public enum TimeChangeContext
        {
            None = 0,
            Day = 1,
            Night = -1
        }

        public struct Boss
        {
            public int EntityID;
            public int SpecialSpawnCountdown;
            public float DimnessFactor;
            public bool UsesSpecialSound;
            public TimeChangeContext ToChangeTimeTo;
            public OnSpawnContext SpawnContext;
            public List<int> HostileNPCsToNotDelete;

            public delegate void OnSpawnContext(int type);

            public Boss(int id, TimeChangeContext toChangeTimeTo = TimeChangeContext.None, OnSpawnContext spawnContext = null, int specialSpawnCountdown = -1, bool usesSpecialSound = false, float dimnessFactor = 0f, params int[] permittedNPCs)
            {
                // Default to a typical SpawnOnPlayer call for boss summoning if nothing else is inputted.
                if (spawnContext is null)
                    spawnContext = (type) => NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);

                EntityID = id;
                SpecialSpawnCountdown = specialSpawnCountdown;
                UsesSpecialSound = usesSpecialSound;
                ToChangeTimeTo = toChangeTimeTo;
                SpawnContext = spawnContext;
                DimnessFactor = dimnessFactor;
                HostileNPCsToNotDelete = permittedNPCs.ToList();

                // Add the NPC type to delete blacklist list by default.
                if (!HostileNPCsToNotDelete.Contains(id))
                    HostileNPCsToNotDelete.Add(id);
                if (BossIDsAfterDeath.TryGetValue(id, out int[] deathThings))
                    HostileNPCsToNotDelete.AddRange(deathThings);
            }
        }

        internal static IEntitySource Source => new EntitySource_WorldEvent("CalamityMod_BossRush");

        public static int HostileProjectileKillCounter;
        public static bool BossRushActive = false; // Whether Boss Rush is active or not.
        public static bool DeactivateStupidFuckingBullshit = false; // Force Boss Rush to inactive.
        public static int BossRushStage = 0; // Boss Rush Stage.
        public static int BossRushSpawnCountdown = 180; // Delay before another Boss Rush boss can spawn.
        public static List<Boss> Bosses = new List<Boss>();
        public static Dictionary<int, int[]> BossIDsAfterDeath = new Dictionary<int, int[]>();
        public static Dictionary<int, Action<NPC>> BossDeathEffects = new Dictionary<int, Action<NPC>>();
        public static int StartTimer;
        public static int EndTimer;
        public static float WhiteDimness;
        public static readonly Color XerocTextColor = new(250, 213, 77); // #FAD54D
        public const int StartEffectTotalTime = 120;
        public const int EndVisualEffectTime = 340;
        public static int ClosestPlayerToWorldCenter => Player.FindClosest(new Vector2(Main.maxTilesX, Main.maxTilesY) * 16f * 0.5f, 1, 1);
        public static int CurrentlyFoughtBoss => Bosses[BossRushStage].EntityID;
        public static int NextBossToFight => Bosses[BossRushStage + 1].EntityID;

        public static readonly SoundStyle BossSummonSound = new("CalamityMod/Sounds/Custom/BossRush/BossRushSummon", 2);

        public static readonly SoundStyle TeleportSound = new("CalamityMod/Sounds/Custom/BossRush/BossRushTeleport");

        public static readonly SoundStyle TerminusActivationSound = new("CalamityMod/Sounds/Custom/BossRush/BossRushTerminusActivate");

        public static readonly SoundStyle StartBuildupSound = new("CalamityMod/Sounds/Custom/BossRush/BossRushTerminusCharge");

        public static readonly SoundStyle TerminusDeactivationSound = new("CalamityMod/Sounds/Custom/BossRush/BossRushTerminusDeactivate");

        public static readonly SoundStyle Tier2TransitionSound = new("CalamityMod/Sounds/Custom/BossRush/BossRushTier2Transition");

        public static readonly SoundStyle Tier3TransitionSound = new("CalamityMod/Sounds/Custom/BossRush/BossRushTier3Transition");

        public static readonly SoundStyle Tier4TransitionSound = new("CalamityMod/Sounds/Custom/BossRush/BossRushTier4Transition");

        public static readonly SoundStyle Tier5TransitionSound = new("CalamityMod/Sounds/Custom/BossRush/BossRushTier5Transition");

        public static readonly SoundStyle VictorySound = new("CalamityMod/Sounds/Custom/BossRush/BossRushVictory");

        #region Loading and Unloading
        public static void Load()
        {
            BossIDsAfterDeath = new Dictionary<int, int[]>();

            // TODO -- Multiple different lists might be ideal for this at some point instead of a god-struct? This is a lot of parameters.
            Bosses = new List<Boss>()
            {
                new Boss(NPCID.KingSlime, spawnContext: type => {
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);

                    // When King Slime spawns, Boss Rush is considered to be started at least once.
                    // Xeroc will no longer give his full start monologue anymore for this world.
                    DownedBossSystem.startedBossRushAtLeastOnce = true;
                },
                permittedNPCs: new int[] { NPCID.BlueSlime, NPCID.YellowSlime, NPCID.PurpleSlime, NPCID.RedSlime, NPCID.GreenSlime, NPCID.RedSlime,
                    NPCID.IceSlime, NPCID.UmbrellaSlime, NPCID.Pinky, NPCID.SlimeSpiked, NPCID.RainbowSlime, ModContent.NPCType<KingSlimeJewel>() }),

                new Boss(ModContent.NPCType<DesertScourgeHead>(), spawnContext: type =>
                {
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, ModContent.NPCType<DesertScourgeHead>());
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, ModContent.NPCType<DesertNuisanceHead>());
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, ModContent.NPCType<DesertNuisanceHead>());
                }, permittedNPCs: new int[] { ModContent.NPCType<DesertScourgeBody>(), ModContent.NPCType<DesertScourgeTail>(), ModContent.NPCType<DesertNuisanceHead>(),
                    ModContent.NPCType<DesertNuisanceBody>(), ModContent.NPCType<DesertNuisanceTail>() }),

                new Boss(NPCID.EyeofCthulhu, TimeChangeContext.Night, permittedNPCs: NPCID.ServantofCthulhu),

                new Boss(ModContent.NPCType<Crabulon>(), TimeChangeContext.Day, type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];
                    int thePefectOne = NPC.NewNPC(Source, (int)(player.position.X + Main.rand.Next(-100, 101)), (int)(player.position.Y - 400f), type, 1);
                    Main.npc[thePefectOne].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(thePefectOne);
                }, permittedNPCs: ModContent.NPCType<CrabShroom>()),

                new Boss(NPCID.EaterofWorldsHead, permittedNPCs: new int[] { NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail, NPCID.VileSpitEaterOfWorlds }),

                new Boss(NPCID.BrainofCthulhu, permittedNPCs: NPCID.Creeper),

                new Boss(ModContent.NPCType<HiveMind>(), permittedNPCs: new int[] { ModContent.NPCType<DankCreeper>(), ModContent.NPCType<DarkHeart>(), ModContent.NPCType<HiveBlob>(), ModContent.NPCType<HiveBlob2>() }),

                new Boss(ModContent.NPCType<PerforatorHive>(), permittedNPCs: new int[] { ModContent.NPCType<PerforatorHeadLarge>(), ModContent.NPCType<PerforatorBodyLarge>(), ModContent.NPCType<PerforatorTailLarge>(),
                    ModContent.NPCType<PerforatorHeadMedium>(), ModContent.NPCType<PerforatorBodyMedium>(), ModContent.NPCType<PerforatorTailMedium>(), ModContent.NPCType<PerforatorHeadSmall>(),
                    ModContent.NPCType<PerforatorBodySmall>() ,ModContent.NPCType<PerforatorTailSmall>() }),

                new Boss(NPCID.QueenBee, permittedNPCs: new int[] { NPCID.Bee, NPCID.BeeSmall }),

                new Boss(NPCID.Deerclops),

                new Boss(NPCID.SkeletronHead, TimeChangeContext.Night, type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];
                    int sans = NPC.NewNPC(Source, (int)(player.position.X + Main.rand.Next(-100, 101)), (int)(player.position.Y - 400f), type, 1);
                    Main.npc[sans].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(sans);
                }, permittedNPCs: NPCID.SkeletronHand),

                new Boss(ModContent.NPCType<SlimeGodCore>(), TimeChangeContext.Day, permittedNPCs: new int[] { ModContent.NPCType<EbonianPaladin>(), ModContent.NPCType<CrimulanPaladin>(), ModContent.NPCType<SplitEbonianPaladin>(), ModContent.NPCType<SplitCrimulanPaladin>(),
                    ModContent.NPCType<CorruptSlimeSpawn>(), ModContent.NPCType<CorruptSlimeSpawn2>(), ModContent.NPCType<CrimsonSlimeSpawn>(), ModContent.NPCType<CrimsonSlimeSpawn2>() }),

                new Boss(NPCID.WallofFlesh, spawnContext: type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];
                    NPC.SpawnWOF(player.position);
                }, permittedNPCs: new int[] { NPCID.WallofFleshEye, NPCID.LeechHead, NPCID.LeechBody, NPCID.LeechTail, NPCID.TheHungry, NPCID.TheHungryII }),

                new Boss(NPCID.QueenSlimeBoss, specialSpawnCountdown: 120, permittedNPCs: new int[] { NPCID.QueenSlimeMinionBlue, NPCID.QueenSlimeMinionPink, NPCID.QueenSlimeMinionPurple }),

                new Boss(ModContent.NPCType<Cryogen>(), permittedNPCs: ModContent.NPCType<CryogenShield>()),

                new Boss(NPCID.Spazmatism, TimeChangeContext.Night, type =>
                {
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, NPCID.Spazmatism);
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, NPCID.Retinazer);
                }, permittedNPCs: NPCID.Retinazer),

                new Boss(ModContent.NPCType<AquaticScourgeHead>(), TimeChangeContext.Day, permittedNPCs: new int[] { ModContent.NPCType<AquaticScourgeBody>(), ModContent.NPCType<AquaticScourgeBodyAlt>(), ModContent.NPCType<AquaticScourgeTail>() }),

                new Boss(NPCID.TheDestroyer, TimeChangeContext.Night, permittedNPCs: new int[] { NPCID.TheDestroyerBody, NPCID.TheDestroyerTail, NPCID.Probe }),

                new Boss(ModContent.NPCType<BrimstoneElemental>(), TimeChangeContext.Day, permittedNPCs: ModContent.NPCType<Brimling>()),

                new Boss(NPCID.SkeletronPrime, TimeChangeContext.Night, permittedNPCs: new int[] { NPCID.PrimeCannon, NPCID.PrimeSaw, NPCID.PrimeVice, NPCID.PrimeLaser, NPCID.Probe }),

                new Boss(ModContent.NPCType<CalamitasClone>(), TimeChangeContext.Night, dimnessFactor: 0.6f, permittedNPCs: new int[] { ModContent.NPCType<Cataclysm>(), ModContent.NPCType<Catastrophe>(),
                    ModContent.NPCType<SoulSeeker>() }),

                new Boss(NPCID.Plantera, TimeChangeContext.Day, permittedNPCs: new int[] { NPCID.PlanterasTentacle, ModContent.NPCType<PlanterasFreeTentacle>(), NPCID.PlanterasHook, NPCID.Spore }),

                new Boss(ModContent.NPCType<Anahita>(), specialSpawnCountdown: 120, permittedNPCs: new int[] { ModContent.NPCType<Leviathan>(), ModContent.NPCType<AquaticAberration>(),
                    ModContent.NPCType<AnahitasIceShield>(), NPCID.DetonatingBubble}),

                new Boss(ModContent.NPCType<AstrumAureus>(), TimeChangeContext.Night, permittedNPCs: ModContent.NPCType<AureusSpawn>()),

                new Boss(NPCID.Golem, TimeChangeContext.Day, type =>
                {
                    int shittyStatueBoss = NPC.NewNPC(Source, (int)(Main.player[ClosestPlayerToWorldCenter].position.X + Main.rand.Next(-100, 101)), (int)(Main.player[ClosestPlayerToWorldCenter].position.Y - 600f), type, 1);
                    Main.npc[shittyStatueBoss].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(shittyStatueBoss);
                }, permittedNPCs: new int[] { NPCID.GolemFistLeft, NPCID.GolemFistRight, NPCID.GolemHead, NPCID.GolemHeadFree }),

                new Boss(ModContent.NPCType<PlaguebringerGoliath>(), spawnContext: type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];

                    SoundEngine.PlaySound(Abombination.UseSound, player.Center);
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                }, usesSpecialSound: true, permittedNPCs: new int[] { ModContent.NPCType<PlagueHomingMissile>(), ModContent.NPCType<PlagueMine>() }),

                new Boss(NPCID.HallowBoss, TimeChangeContext.Night),

                new Boss(NPCID.DukeFishron, TimeChangeContext.Day, type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];
                    int dukeFishron = NPC.NewNPC(Source, (int)(player.position.X + Main.rand.Next(-100, 101)), (int)(player.position.Y - 400f), type, 1);
                    Main.npc[dukeFishron].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(dukeFishron);
                }, permittedNPCs: new int[] { NPCID.DetonatingBubble, NPCID.Sharkron, NPCID.Sharkron2 }),

                new Boss(ModContent.NPCType<RavagerBody>(), spawnContext: type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];

                    SoundEngine.PlaySound(SoundID.ScaryScream, player.Center);
                    int ravager = NPC.NewNPC(Source, (int)(player.position.X + Main.rand.Next(-100, 101)), (int)(player.position.Y - 600f), type, 1);
                    Main.npc[ravager].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(ravager);
                }, usesSpecialSound: true, permittedNPCs: new int[] { ModContent.NPCType<FlamePillar>(), ModContent.NPCType<RockPillar>(), ModContent.NPCType<RavagerLegLeft>(), ModContent.NPCType<RavagerLegRight>(),
                   ModContent.NPCType<RavagerClawLeft>(), ModContent.NPCType<RavagerClawRight>(), ModContent.NPCType<RavagerHead>(), ModContent.NPCType<RavagerHead2>() }),

                new Boss(NPCID.CultistBoss, spawnContext: type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];
                    int doctorLooneyTunes = NPC.NewNPC(Source, (int)player.Center.X, (int)player.Center.Y - 400, type, 1);
                    Main.npc[doctorLooneyTunes].direction = Main.npc[doctorLooneyTunes].spriteDirection = Math.Sign(player.Center.X - player.Center.X - 90f);
                    Main.npc[doctorLooneyTunes].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(doctorLooneyTunes);
                }, permittedNPCs: new int[] { NPCID.CultistBossClone, NPCID.CultistDragonHead, NPCID.CultistDragonBody1, NPCID.CultistDragonBody2, NPCID.CultistDragonBody3, NPCID.CultistDragonBody4,
                    NPCID.CultistDragonTail, NPCID.AncientCultistSquidhead, NPCID.AncientLight, NPCID.AncientDoom }),

                new Boss(ModContent.NPCType<AstrumDeusHead>(), TimeChangeContext.Night, type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];

                    // Delete the pillars spawned by the cultist.
                    for (int doom = 0; doom < Main.maxNPCs; doom++)
                    {
                        bool isPillar = Main.npc[doom].type == NPCID.LunarTowerStardust || Main.npc[doom].type == NPCID.LunarTowerVortex || Main.npc[doom].type == NPCID.LunarTowerNebula || Main.npc[doom].type == NPCID.LunarTowerSolar;
                        if (Main.npc[doom].active && isPillar)
                        {
                            Main.npc[doom].active = false;
                            Main.npc[doom].netUpdate = true;
                        }
                    }

                    SoundEngine.PlaySound(AstrumDeusHead.SpawnSound, player.Center);
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                }, usesSpecialSound: true, permittedNPCs: new int[] { ModContent.NPCType<AstrumDeusBody>(), ModContent.NPCType<AstrumDeusTail>() }),

                new Boss(NPCID.MoonLordCore, spawnContext: type =>
                {
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                }, permittedNPCs: new int[] { NPCID.MoonLordLeechBlob, NPCID.MoonLordHand, NPCID.MoonLordHead, NPCID.MoonLordFreeEye }),

                new Boss(ModContent.NPCType<ProfanedGuardianCommander>(), TimeChangeContext.Day, type =>
                {
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                }, permittedNPCs: new int[] { ModContent.NPCType<ProfanedGuardianDefender>(), ModContent.NPCType<ProfanedGuardianHealer>(), ModContent.NPCType<ProfanedRocks>() }),

                new Boss(ModContent.NPCType<Bumblefuck>(), permittedNPCs: new int[] { ModContent.NPCType<Bumblefuck2>() }),

                new Boss(ModContent.NPCType<Providence>(), TimeChangeContext.Day, type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];

                    SoundEngine.PlaySound(Providence.SpawnSound, player.Center);
                    int prov = NPC.NewNPC(Source, (int)(player.position.X + Main.rand.Next(-500, 501)), (int)(player.position.Y - 250f), type, 1);
                    Main.npc[prov].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(prov);
                }, usesSpecialSound: true, permittedNPCs: new int[] { ModContent.NPCType<ProvSpawnOffense>(), ModContent.NPCType<ProvSpawnHealer>(), ModContent.NPCType<ProvSpawnDefense>() }),

                new Boss(ModContent.NPCType<CeaselessVoid>(), spawnContext: type =>
                {
                    for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
                    {
                        Player p = Main.player[playerIndex];
                        if (p is not null && p.active)
                        {
                            if (p.FindBuffIndex(ModContent.BuffType<IcarusFolly>()) > -1)
                                p.ClearBuff(ModContent.BuffType<IcarusFolly>());
                        }
                    }

                    Player player = Main.player[ClosestPlayerToWorldCenter];

                    SoundEngine.PlaySound(RuneofKos.CVSound, player.Center);
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                }, usesSpecialSound: true, permittedNPCs: ModContent.NPCType<DarkEnergy>()),

                new Boss(ModContent.NPCType<StormWeaverHead>(), spawnContext: type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];

                    SoundEngine.PlaySound(RuneofKos.StormSound, player.Center);
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                }, usesSpecialSound: true, permittedNPCs: new int[] { ModContent.NPCType<StormWeaverBody>(), ModContent.NPCType<StormWeaverTail>(), }),

                new Boss(ModContent.NPCType<Signus>(), spawnContext: type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];

                    SoundEngine.PlaySound(RuneofKos.SignutSound, player.Center);
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                }, usesSpecialSound: true, permittedNPCs: new int[] { ModContent.NPCType<CosmicLantern>(), ModContent.NPCType<CosmicMine>() }),

                new Boss(ModContent.NPCType<Polterghast>(), spawnContext: type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];

                    SoundEngine.PlaySound(Polterghast.SpawnSound, player.Center);
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);

                }, usesSpecialSound: true, permittedNPCs: new int[] { ModContent.NPCType<PhantomFuckYou>(), ModContent.NPCType<PolterghastHook>(), ModContent.NPCType<PolterPhantom>() }),

                new Boss(ModContent.NPCType<OldDuke>(), spawnContext: type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];
                    int boomerDuke = NPC.NewNPC(Source, (int)(player.position.X + Main.rand.Next(-100, 101)), (int)(player.position.Y - 400f), type, 1);
                    Main.npc[boomerDuke].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(boomerDuke);
                }, permittedNPCs: new int[] { ModContent.NPCType<OldDukeToothBall>(), ModContent.NPCType<SulphurousSharkron>() }),

                new Boss(ModContent.NPCType<DevourerofGodsHead>(), spawnContext: type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];

                    SoundEngine.PlaySound(DevourerofGodsHead.SpawnSound, player.Center);
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                }, usesSpecialSound: true, permittedNPCs: new int[] { ModContent.NPCType<DevourerofGodsBody>(), ModContent.NPCType<DevourerofGodsTail>(), ModContent.NPCType<CosmicGuardianHead>(), ModContent.NPCType<CosmicGuardianBody>(), ModContent.NPCType<CosmicGuardianTail>() }),

                new Boss(ModContent.NPCType<Yharon>(), spawnContext: type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];

                    SoundEngine.PlaySound(Yharon.FireSound, player.Center);
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                }, usesSpecialSound: true),

                new Boss(ModContent.NPCType<Draedon>(), spawnContext: type =>
                {
                    if (!NPC.AnyNPCs(ModContent.NPCType<Draedon>()))
                    {
                        Player player = Main.player[ClosestPlayerToWorldCenter];

                        SoundEngine.PlaySound(CodebreakerUI.SummonSound, player.Center);
                        Vector2 spawnPos = player.Center + new Vector2(-8f, -100f);
                        int draedon = NPC.NewNPC(Source, (int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<Draedon>());
                        Main.npc[draedon].timeLeft *= 20;
                    }
                }, usesSpecialSound: true, permittedNPCs: new int[] { ModContent.NPCType<ApolloBoss>(), ModContent.NPCType<AresBody>(), ModContent.NPCType<AresGaussNuke>(), ModContent.NPCType<AresLaserCannon>(), ModContent.NPCType<AresPlasmaFlamethrower>(), ModContent.NPCType<AresTeslaCannon>(), ModContent.NPCType<ArtemisBoss>(), ModContent.NPCType<ThanatosBody1>(), ModContent.NPCType<ThanatosBody2>(), ModContent.NPCType<ThanatosHead>(), ModContent.NPCType<ThanatosTail>() }),

                new Boss(ModContent.NPCType<SupremeCalamitas>(), spawnContext: type =>
                {
                    SoundEngine.PlaySound(SupremeCalamitas.SpawnSound, Main.player[ClosestPlayerToWorldCenter].Center);
                    CalamityUtils.SpawnBossBetter(Main.player[ClosestPlayerToWorldCenter].Top - new Vector2(42f, 84f), type);
                }, specialSpawnCountdown: 840, dimnessFactor: 0.6f, permittedNPCs: new int[] { ModContent.NPCType<SepulcherArm>(), ModContent.NPCType<SepulcherHead>(), ModContent.NPCType<SepulcherBody>(), ModContent.NPCType<SepulcherBodyEnergyBall>(), ModContent.NPCType<SepulcherTail>(),
                    ModContent.NPCType<SoulSeekerSupreme>(), ModContent.NPCType<BrimstoneHeart>(), ModContent.NPCType<SupremeCataclysm>(), ModContent.NPCType<SupremeCatastrophe>() }),
            };

            BossDeathEffects = new Dictionary<int, Action<NPC>>()
            {
                // Wall of Flesh: End of Tier 1
                [NPCID.WallofFlesh] = npc =>
                {
                    CreateTierAnimation(2);
                    BossRushDialogueSystem.StartDialogue(BossRushDialoguePhase.TierOneComplete);

                    // Teleport players to where they came from
                    for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
                    {
                        Player p = Main.player[playerIndex];
                        if (p is not null && p.active)
                        {
                            if (p.Calamity().BossRushReturnPosition.HasValue)
                            {
                                CalamityPlayer.ModTeleport(p, p.Calamity().BossRushReturnPosition.Value, false, TeleportationStyleID.TeleportationPotion);
                                p.Calamity().BossRushReturnPosition = null;
                            }
                            p.Calamity().BossRushReturnPosition = null;
                            SoundEngine.PlaySound(TeleportSound with { Volume = 1.6f }, p.Center);
                        }
                    }
                },
                // Plantera: End of Tier 2
                [NPCID.Plantera] = npc =>
                {
                    CreateTierAnimation(3);
                    BossRushDialogueSystem.StartDialogue(BossRushDialoguePhase.TierTwoComplete);
                },
                // Moon Lord: End of Tier 3
                [NPCID.MoonLordCore] = npc =>
                {
                    CreateTierAnimation(4);
                    BossRushDialogueSystem.StartDialogue(BossRushDialoguePhase.TierThreeComplete);
                },
                // Devourer of Gods: End of Tier 4
                [ModContent.NPCType<DevourerofGodsHead>()] = npc =>
                {
                    CreateTierAnimation(5);
                    BossRushDialogueSystem.StartDialogue(BossRushDialoguePhase.TierFourComplete);
                },
                // Supreme Calamitas: Ends Boss Rush (also delete all hostile projectiles to be safe)
                [ModContent.NPCType<SupremeCalamitas>()] = npc =>
                {
                    CalamityUtils.KillAllHostileProjectiles();
                    HostileProjectileKillCounter = 3;

                    // Display short dialogue if BR has been beaten before.
                    BossRushDialogueSystem.StartDialogue(DownedBossSystem.downedBossRush ? BossRushDialoguePhase.EndRepeat : BossRushDialoguePhase.End);
                }
            };
        }

        public static void Unload()
        {
            Bosses = null;
            BossIDsAfterDeath = null;
            BossDeathEffects = null;
        }
        #endregion

        #region Properties
        public static int CurrentTier
        {
            get
            {
                if (BossRushStage > Bosses.FindIndex(boss => boss.EntityID == ModContent.NPCType<DevourerofGodsHead>()))
                    return 5;
                if (BossRushStage > Bosses.FindIndex(boss => boss.EntityID == NPCID.MoonLordCore))
                    return 4;
                if (BossRushStage > Bosses.FindIndex(boss => boss.EntityID == NPCID.Plantera))
                    return 3;
                if (BossRushStage > Bosses.FindIndex(boss => boss.EntityID == NPCID.WallofFlesh))
                    return 2;
                return 1;
            }
        }

        public static int MusicToPlay
        {
            get
            {
                int tier = CurrentTier;
                if (CalamityMod.Instance.musicMod != null)
                {
                    // Boss Rush music for tiers 4 and 5 don't exist
                    if (tier > 3)
                        tier = 3;
                    return CalamityMod.Instance.GetMusicFromMusicMod($"BossRushTier{tier}") ?? 0;
                }

                switch (CurrentTier)
                {
                    case 1:
                        return MusicID.Boss1;
                    case 2:
                        return MusicID.Boss4;
                    case 3:
                        return MusicID.Boss2;
                    case 4:
                        return MusicID.Boss3;
                    case 5:
                        return MusicID.LunarBoss;
                    default:
                        break;
                }
                return 0;
            }
        }
        #endregion Properties

        #region Updates
        internal static void MiscUpdateEffects()
        {
            if (!BossRushActive)
                return;

            // Handle dialogue as appropriate.
            BossRushDialogueSystem.Tick();

            // Disable the stupid credits sequence.
            if (CreditsRollEvent.IsEventOngoing)
                CreditsRollEvent.SetRemainingTimeDirect(1);

            // Prevent Moon Lord from spawning naturally
            if (NPC.MoonLordCountdown > 0)
                NPC.MoonLordCountdown = 0;

            // Handle projectile clearing.
            if (HostileProjectileKillCounter > 0)
            {
                HostileProjectileKillCounter--;
                if (HostileProjectileKillCounter == 1)
                    CalamityUtils.KillAllHostileProjectiles();

                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = CalamityMod.Instance.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.BRHostileProjKillSync);
                    netMessage.Write(HostileProjectileKillCounter);
                    netMessage.Send();
                }
            }
        }

        internal static void Update()
        {
            if (!BossRushActive)
            {
                BossRushSpawnCountdown = 180;
                BossRushSky.CurrentInterestMin = 0f;
                if (BossRushStage != 0)
                {
                    BossRushStage = 0;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        var netMessage = CalamityMod.Instance.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.BossRushStage);
                        netMessage.Write(BossRushStage);
                        netMessage.Send();
                    }
                }
                return;
            }

            // Projectile deletion, preventing Credits and ML from spawning naturally, and dialogue.
            MiscUpdateEffects();

            // Do boss rush countdown and shit if no boss is alive.
            if (!CalamityPlayer.areThereAnyDamnBosses)
            {
                if (BossRushSpawnCountdown > 0)
                    BossRushSpawnCountdown--;

                // Cooldown and boss spawn.
                if (BossRushSpawnCountdown <= 0 && BossRushStage < Bosses.Count)
                {
                    // Cooldown before next boss spawns.
                    BossRushSpawnCountdown = 60;

                    // Increase cooldown post-Moon Lord.
                    if (BossRushStage >= Bosses.FindIndex(boss => boss.EntityID == NPCID.MoonLordCore))
                        BossRushSpawnCountdown += 300;

                    // Override the spawn countdown if specified.
                    if (BossRushStage < Bosses.Count - 1 && Bosses[BossRushStage + 1].SpecialSpawnCountdown != -1)
                        BossRushSpawnCountdown = Bosses[BossRushStage + 1].SpecialSpawnCountdown;

                    // Change time as necessary.
                    if (Bosses[BossRushStage].ToChangeTimeTo != TimeChangeContext.None)
                        CalamityUtils.ChangeTime(Bosses[BossRushStage].ToChangeTimeTo == TimeChangeContext.Day);

                    // Play a special boss roar sound by default.
                    if (!Bosses[BossRushStage].UsesSpecialSound)
                        SoundEngine.PlaySound(BossSummonSound, Main.player[ClosestPlayerToWorldCenter].Center);

                    // And spawn the boss.
                    Bosses[BossRushStage].SpawnContext.Invoke(CurrentlyFoughtBoss);
                }
            }

            // Change dimness.
            if (BossRushStage >= 0 && BossRushStage < Bosses.Count)
            {
                WhiteDimness = MathHelper.Lerp(WhiteDimness, Bosses[BossRushStage].DimnessFactor, 0.1f);
                if (MathHelper.Distance(WhiteDimness, Bosses[BossRushStage].DimnessFactor) < 0.004f)
                    WhiteDimness = Bosses[BossRushStage].DimnessFactor;
            }

            if (EndTimer > 0)
                BossRushSky.CurrentInterest = MathHelper.Lerp(0.5f, 0.75f, Utils.GetLerpValue(5f, 145f, EndTimer, true));
            BossRushSky.CurrentInterestMin = MathHelper.Lerp(0f, 0.5f, (float)Math.Pow(BossRushStage / (float)Bosses.Count, 5D));
        }

        public static void End()
        {
            // Reset BossRushReturnPosition
            for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
            {
                Player p = Main.player[playerIndex];
                if (p is not null && p.active)
                {
                    p.Calamity().BossRushReturnPosition = null;
                }
            }

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                EndEffects();
            }
            else
            {
                var netMessage = CalamityMod.Instance.GetPacket();
                netMessage.Write((byte)CalamityModMessageType.EndBossRush);
                netMessage.Send();
            }
        }

        internal static void EndEffects()
        {
            for (int doom = 0; doom < Main.maxNPCs; doom++)
            {
                NPC n = Main.npc[doom];
                if (!n.active)
                    continue;

                // will also correctly despawn EoW because none of his segments are boss flagged. Draedon isn't a boss either
                bool shouldDespawn = n.boss || n.type == NPCID.EaterofWorldsHead || n.type == NPCID.EaterofWorldsBody || n.type == NPCID.EaterofWorldsTail || n.type == ModContent.NPCType<Draedon>();
                if (shouldDespawn)
                {
                    n.active = false;
                    n.netUpdate = true;
                }
            }

            BossRushActive = false;
            BossRushStage = 0;
            StartTimer = 0;
            EndTimer = 0;
            CalamityUtils.KillAllHostileProjectiles();

            CalamityNetcode.SyncWorld();
            if (Main.netMode == NetmodeID.Server)
            {
                var netMessage = CalamityMod.Instance.GetPacket();
                netMessage.Write((byte)CalamityModMessageType.BossRushStage);
                netMessage.Write(BossRushStage);
                netMessage.Send();
                var netMessage2 = CalamityMod.Instance.GetPacket();
                netMessage2.Write((byte)CalamityModMessageType.BossRushStartTimer);
                netMessage2.Write(StartTimer);
                netMessage2.Send();
                var netMessage3 = CalamityMod.Instance.GetPacket();
                netMessage3.Write((byte)CalamityModMessageType.BossRushEndTimer);
                netMessage3.Write(EndTimer);
                netMessage3.Send();
            }
        }

        #endregion

        #region On Boss Kill
        internal static void OnBossKill(NPC npc, Mod mod)
        {
            // Eater of Worlds splits in Boss Rush now, so you have to kill every single segment to progress.
            // Vanilla sets npc.boss to true for the last Eater of Worlds segment to die in NPC.checkDead.
            // This means we do not need to manually check for other segments ourselves.
            if (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)
            {
                if (npc.boss)
                {
                    BossRushStage++;
                    CalamityUtils.KillAllHostileProjectiles();
                    HostileProjectileKillCounter = 3;
                }
            }

            // Anahita and Leviathan manually check for each other (this probably isn't necessary).
            else if (npc.type == ModContent.NPCType<Anahita>() || npc.type == ModContent.NPCType<Leviathan>())
            {
                int bossType = (npc.type == ModContent.NPCType<Anahita>()) ? ModContent.NPCType<Leviathan>() : ModContent.NPCType<Anahita>();
                if (!NPC.AnyNPCs(bossType))
                {
                    BossRushStage++;
                    CalamityUtils.KillAllHostileProjectiles();
                    HostileProjectileKillCounter = 3;
                }
            }

            // Killing any split Deus head ends the fight instantly. You don't need to kill both.
            else if (npc.type == ModContent.NPCType<AstrumDeusHead>() && npc.Calamity().newAI[0] != 0f)
            {
                BossRushStage++;
                CalamityUtils.KillAllHostileProjectiles();
                HostileProjectileKillCounter = 3;
            }

            // All Slime God entities must be killed to progress to the next stage.
            else if (npc.type == ModContent.NPCType<SlimeGodCore>())
            {
                BossRushStage++;
                CalamityUtils.KillAllHostileProjectiles();
                HostileProjectileKillCounter = 3;

                for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
                {
                    Player p = Main.player[playerIndex];
                    if (p is not null && p.active)
                    {
                        p.Calamity().BossRushReturnPosition = p.Center;
                        Vector2? underworld = CalamityPlayer.GetUnderworldPosition(p);
                        if (!underworld.HasValue)
                            break;
                        CalamityPlayer.ModTeleport(p, underworld.Value, false, TeleportationStyleID.TeleportationPotion);
                        SoundEngine.PlaySound(TeleportSound with { Volume = 1.6f }, p.Center);
                    }
                }
            }

            // This is the generic form of "Are there any remaining NPCs on the boss list for this boss rush stage?" check.
            else if ((Bosses.Any(boss => boss.EntityID == npc.type) && !BossIDsAfterDeath.ContainsKey(npc.type)) ||
                     BossIDsAfterDeath.Values.Any(killList => killList.Contains(npc.type)))
            {
                BossRushStage++;
                CalamityUtils.KillAllHostileProjectiles();
                HostileProjectileKillCounter = 3;

                if (BossDeathEffects.ContainsKey(npc.type))
                {
                    BossDeathEffects[npc.type].Invoke(npc);
                }

                if (npc.type == Bosses[Bosses.Count -1].EntityID)
                {
                    // Mark Boss Rush as complete
                    DownedBossSystem.downedBossRush = true;
                    CalamityNetcode.SyncWorld();

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(Source, npc.Center, Vector2.Zero, ModContent.ProjectileType<BossRushEndEffectThing>(), 0, 0f, Main.myPlayer);
                }
            }

            // Sync the stage and progress of Boss Rush whenever a relevant boss dies.
            if (Main.netMode == NetmodeID.Server)
            {
                var netMessage = mod.GetPacket();
                netMessage.Write((byte)CalamityModMessageType.BossRushStage);
                netMessage.Write(BossRushStage);
                netMessage.Send();
                var netMessage2 = mod.GetPacket();
                netMessage2.Write((byte)CalamityModMessageType.BRHostileProjKillSync);
                netMessage2.Write(HostileProjectileKillCounter);
                netMessage2.Send();
            }

            BossRushSky.CurrentInterest = 0.85f;
        }

        public static void CreateTierAnimation(int tier)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (!Main.player[i].active || Main.player[i].dead)
                        continue;

                    int animation = Projectile.NewProjectile(new EntitySource_WorldEvent(), Main.player[i].Center, Vector2.Zero, ModContent.ProjectileType<BossRushTierAnimation>(), 0, 0f, i);
                    if (Main.projectile.IndexInRange(animation))
                        Main.projectile[animation].ai[0] = tier;
                }
            }
        }
        #endregion

        #region Netcode

        public static void SyncStartTimer(int time)
        {
            StartTimer = time;
            if (Main.netMode != NetmodeID.Server)
                return;

            var netMessage = CalamityMod.Instance.GetPacket();
            netMessage.Write((byte)CalamityModMessageType.BossRushStartTimer);
            netMessage.Write(StartTimer);
            netMessage.Send();
        }

        public static void SyncEndTimer(int time)
        {
            EndTimer = time;
            if (Main.netMode != NetmodeID.Server)
                return;

            var netMessage = CalamityMod.Instance.GetPacket();
            netMessage.Write((byte)CalamityModMessageType.BossRushEndTimer);
            netMessage.Write(EndTimer);
            netMessage.Send();
        }
        #endregion
    }
}
