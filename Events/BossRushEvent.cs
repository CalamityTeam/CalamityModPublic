using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
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
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;

namespace CalamityMod.Events
{
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

        internal static IEntitySource Source => new EntitySource_WorldEvent();

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
        public static readonly Color XerocTextColor = Color.LightCoral;
        public const int StartEffectTotalTime = 120;
        public const int EndVisualEffectTime = 340;
        public static int ClosestPlayerToWorldCenter => Player.FindClosest(new Vector2(Main.maxTilesX, Main.maxTilesY) * 16f * 0.5f, 1, 1);
        public static int CurrentlyFoughtBoss => Bosses[BossRushStage].EntityID;
        public static int NextBossToFight => Bosses[BossRushStage + 1].EntityID;

        public static readonly SoundStyle EndSound = new("CalamityMod/Sounds/Custom/BossRushEnd");

        #region Loading and Unloading
        public static void Load()
        {
            BossIDsAfterDeath = new Dictionary<int, int[]>();

            // TODO -- Multiple different lists might be ideal for this at some point instead of a god-struct? This is a lot of parameters.
            Bosses = new List<Boss>()
            {
                new Boss(NPCID.QueenBee),

                new Boss(NPCID.BrainofCthulhu, permittedNPCs: NPCID.Creeper),

                new Boss(NPCID.KingSlime, permittedNPCs: new int[] { NPCID.BlueSlime, NPCID.YellowSlime, NPCID.PurpleSlime, NPCID.RedSlime, NPCID.GreenSlime, NPCID.RedSlime,
                    NPCID.IceSlime, NPCID.UmbrellaSlime, NPCID.Pinky, NPCID.SlimeSpiked, NPCID.RainbowSlime, ModContent.NPCType<KingSlimeJewel>() }),

                new Boss(NPCID.EyeofCthulhu, TimeChangeContext.Night, permittedNPCs: NPCID.ServantofCthulhu),

                new Boss(NPCID.SkeletronPrime, TimeChangeContext.Night, permittedNPCs: new int[] { NPCID.PrimeCannon, NPCID.PrimeSaw, NPCID.PrimeVice, NPCID.PrimeLaser, NPCID.Probe }),

                new Boss(NPCID.Golem, TimeChangeContext.Day, type =>
                {
                    int shittyStatueBoss = NPC.NewNPC(Source, (int)(Main.player[ClosestPlayerToWorldCenter].position.X + Main.rand.Next(-100, 101)), (int)(Main.player[ClosestPlayerToWorldCenter].position.Y - 400f), type, 1);
                    Main.npc[shittyStatueBoss].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(shittyStatueBoss);
                }, permittedNPCs: new int[] { NPCID.GolemFistLeft, NPCID.GolemFistRight, NPCID.GolemHead, NPCID.GolemHeadFree }),

                new Boss(ModContent.NPCType<ProfanedGuardianCommander>(), TimeChangeContext.Day, permittedNPCs: new int[] { ModContent.NPCType<ProfanedGuardianDefender>(), ModContent.NPCType<ProfanedGuardianHealer>() }),

                new Boss(NPCID.EaterofWorldsHead, permittedNPCs: new int[] { NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail, NPCID.VileSpit }),

                new Boss(ModContent.NPCType<AstrumAureus>(), TimeChangeContext.Night, permittedNPCs: ModContent.NPCType<AureusSpawn>()),

                new Boss(NPCID.TheDestroyer, TimeChangeContext.Night, specialSpawnCountdown: 300, permittedNPCs: new int[] { NPCID.TheDestroyerBody, NPCID.TheDestroyerTail, NPCID.Probe }),

                new Boss(NPCID.Spazmatism, TimeChangeContext.Night, type =>
                {
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, NPCID.Spazmatism);
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, NPCID.Retinazer);
                }, permittedNPCs: NPCID.Retinazer),

                new Boss(ModContent.NPCType<Bumblefuck>(), TimeChangeContext.Day, permittedNPCs: new int[] { ModContent.NPCType<Bumblefuck2>(), NPCID.Spazmatism, NPCID.Retinazer }),

                new Boss(NPCID.WallofFlesh, spawnContext: type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];
                    NPC.SpawnWOF(player.position);
                }, permittedNPCs: new int[] { NPCID.WallofFleshEye, NPCID.LeechHead, NPCID.LeechBody, NPCID.LeechTail, NPCID.TheHungry, NPCID.TheHungryII }),

                new Boss(ModContent.NPCType<HiveMind>(), permittedNPCs: new int[] { ModContent.NPCType<DankCreeper>(), ModContent.NPCType<DarkHeart>(), ModContent.NPCType<HiveBlob>(), ModContent.NPCType<HiveBlob2>() }),

                new Boss(NPCID.SkeletronHead, TimeChangeContext.Night, type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];
                    int sans = NPC.NewNPC(Source, (int)(player.position.X + Main.rand.Next(-100, 101)), (int)(player.position.Y - 400f), type, 1);
                    Main.npc[sans].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(sans);
                }, permittedNPCs: NPCID.SkeletronHand),

                new Boss(ModContent.NPCType<StormWeaverHead>(), TimeChangeContext.Day, permittedNPCs: new int[] { ModContent.NPCType<StormWeaverBody>(), ModContent.NPCType<StormWeaverTail>(),  }),

                new Boss(ModContent.NPCType<AquaticScourgeHead>(), permittedNPCs: new int[] { ModContent.NPCType<AquaticScourgeBody>(), ModContent.NPCType<AquaticScourgeBodyAlt>(), ModContent.NPCType<AquaticScourgeTail>() }),

                new Boss(ModContent.NPCType<DesertScourgeHead>(), spawnContext: type =>
                {
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, ModContent.NPCType<DesertScourgeHead>());
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, ModContent.NPCType<DesertNuisanceHead>());
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, ModContent.NPCType<DesertNuisanceHead>());
                }, permittedNPCs: new int[] { ModContent.NPCType<DesertScourgeBody>(), ModContent.NPCType<DesertScourgeTail>(), ModContent.NPCType<DesertNuisanceHead>(),
                    ModContent.NPCType<DesertNuisanceBody>(), ModContent.NPCType<DesertNuisanceTail>() }),

                new Boss(NPCID.CultistBoss, spawnContext: type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];
                    int doctorLooneyTunes = NPC.NewNPC(Source, (int)player.Center.X, (int)player.Center.Y - 400, type, 1);
                    Main.npc[doctorLooneyTunes].direction = Main.npc[doctorLooneyTunes].spriteDirection = Math.Sign(player.Center.X - player.Center.X - 90f);
                    Main.npc[doctorLooneyTunes].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(doctorLooneyTunes);
                }, permittedNPCs: new int[] { NPCID.CultistBossClone, NPCID.CultistDragonHead, NPCID.CultistDragonBody1, NPCID.CultistDragonBody2, NPCID.CultistDragonBody3, NPCID.CultistDragonBody4,
                    NPCID.CultistDragonTail, NPCID.AncientCultistSquidhead, NPCID.AncientLight, NPCID.AncientDoom }),

                new Boss(ModContent.NPCType<Crabulon>(), spawnContext: type =>
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
                    int thePefectOne = NPC.NewNPC(Source, (int)(player.position.X + Main.rand.Next(-100, 101)), (int)(player.position.Y - 400f), type, 1);
                    Main.npc[thePefectOne].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(thePefectOne);
                }, specialSpawnCountdown: 300, permittedNPCs: ModContent.NPCType<CrabShroom>()),

                new Boss(NPCID.Plantera, permittedNPCs: new int[] { NPCID.PlanterasTentacle, NPCID.PlanterasHook, NPCID.Spore }),

                new Boss(ModContent.NPCType<CeaselessVoid>(), permittedNPCs: ModContent.NPCType<DarkEnergy>()),

                new Boss(ModContent.NPCType<PerforatorHive>(), permittedNPCs: new int[] { ModContent.NPCType<PerforatorHeadLarge>(), ModContent.NPCType<PerforatorBodyLarge>(), ModContent.NPCType<PerforatorTailLarge>(),
                    ModContent.NPCType<PerforatorHeadMedium>(), ModContent.NPCType<PerforatorBodyMedium>(), ModContent.NPCType<PerforatorTailMedium>(), ModContent.NPCType<PerforatorHeadSmall>(),
                    ModContent.NPCType<PerforatorBodySmall>() ,ModContent.NPCType<PerforatorTailSmall>() }),

                new Boss(ModContent.NPCType<Cryogen>(), permittedNPCs: ModContent.NPCType<CryogenShield>()),

                new Boss(ModContent.NPCType<BrimstoneElemental>(), permittedNPCs: ModContent.NPCType<Brimling>()),

                new Boss(ModContent.NPCType<Signus>(), specialSpawnCountdown: 360, permittedNPCs: new int[] { ModContent.NPCType<CosmicLantern>(), ModContent.NPCType<CosmicMine>() }),

                new Boss(ModContent.NPCType<RavagerBody>(), spawnContext: type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];

                    SoundEngine.PlaySound(SoundID.ScaryScream, player.position);
                    int ravager = NPC.NewNPC(Source, (int)(player.position.X + Main.rand.Next(-100, 101)), (int)(player.position.Y - 400f), type, 1);
                    Main.npc[ravager].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(ravager);
                }, usesSpecialSound: true, permittedNPCs: new int[] { ModContent.NPCType<FlamePillar>(), ModContent.NPCType<RockPillar>(), ModContent.NPCType<RavagerLegLeft>(), ModContent.NPCType<RavagerLegRight>(),
                   ModContent.NPCType<RavagerClawLeft>(), ModContent.NPCType<RavagerClawRight>() }),

                new Boss(NPCID.DukeFishron, spawnContext: type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];
                    int dukeFishron = NPC.NewNPC(Source, (int)(player.position.X + Main.rand.Next(-100, 101)), (int)(player.position.Y - 400f), type, 1);
                    Main.npc[dukeFishron].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(dukeFishron);
                }, permittedNPCs: new int[] { NPCID.DetonatingBubble, NPCID.Sharkron, NPCID.Sharkron2 }),

                new Boss(NPCID.MoonLordCore, spawnContext: type =>
                {
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.BossRushTierThreeEndText2", XerocTextColor);
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                }, permittedNPCs: new int[] { NPCID.MoonLordLeechBlob, NPCID.MoonLordHand, NPCID.MoonLordHead, NPCID.MoonLordFreeEye }),

                new Boss(ModContent.NPCType<AstrumDeusHead>(), TimeChangeContext.Night, type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];

                    SoundEngine.PlaySound(AstrumDeusHead.SpawnSound, player.Center);
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                }, usesSpecialSound: true, permittedNPCs: new int[] { ModContent.NPCType<AstrumDeusBody>(), ModContent.NPCType<AstrumDeusTail>() }),

                new Boss(ModContent.NPCType<Polterghast>(), TimeChangeContext.Day, permittedNPCs: new int[] { ModContent.NPCType<PhantomFuckYou>(), ModContent.NPCType<PolterghastHook>(), ModContent.NPCType<PolterPhantom>() }),

                new Boss(ModContent.NPCType<PlaguebringerGoliath>(), permittedNPCs: new int[] { ModContent.NPCType<PlagueHomingMissile>(), ModContent.NPCType<PlagueMine>() }),

                new Boss(ModContent.NPCType<CalamitasClone>(), TimeChangeContext.Night, specialSpawnCountdown: 420, dimnessFactor: 0.6f, permittedNPCs: new int[] { ModContent.NPCType<Cataclysm>(), ModContent.NPCType<Catastrophe>(),
                    ModContent.NPCType<SoulSeeker>() }),

                new Boss(ModContent.NPCType<Anahita>(), TimeChangeContext.Day, permittedNPCs: new int[] { ModContent.NPCType<Leviathan>(), ModContent.NPCType<AquaticAberration>(),
                    ModContent.NPCType<AnahitasIceShield>(), NPCID.DetonatingBubble}),

                new Boss(ModContent.NPCType<OldDuke>(), spawnContext: type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];
                    int boomerDuke = NPC.NewNPC(Source, (int)(player.position.X + Main.rand.Next(-100, 101)), (int)(player.position.Y - 400f), type, 1);
                    Main.npc[boomerDuke].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(boomerDuke);
                }, permittedNPCs: new int[] { ModContent.NPCType<OldDukeToothBall>(), ModContent.NPCType<SulphurousSharkron>() }),

                new Boss(ModContent.NPCType<SlimeGodCore>(), permittedNPCs: new int[] { ModContent.NPCType<EbonianSlimeGod>(), ModContent.NPCType<CrimulanSlimeGod>(), ModContent.NPCType<SplitEbonianSlimeGod>(), ModContent.NPCType<SplitCrimulanSlimeGod>(),
                    ModContent.NPCType<CorruptSlimeSpawn>(), ModContent.NPCType<CorruptSlimeSpawn2>(), ModContent.NPCType<CrimsonSlimeSpawn>(), ModContent.NPCType<CrimsonSlimeSpawn2>() }),

                new Boss(ModContent.NPCType<Providence>(), TimeChangeContext.Day, type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];

                    SoundEngine.PlaySound(Providence.SpawnSound, player.Center);
                    int prov = NPC.NewNPC(Source, (int)(player.position.X + Main.rand.Next(-500, 501)), (int)(player.position.Y - 250f), type, 1);
                    Main.npc[prov].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(prov);
                }, usesSpecialSound: true, permittedNPCs: new int[] { ModContent.NPCType<ProvSpawnOffense>(), ModContent.NPCType<ProvSpawnHealer>(), ModContent.NPCType<ProvSpawnDefense>() }),

                new Boss(ModContent.NPCType<SupremeCalamitas>(), spawnContext: type =>
                {
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.BossRushTierFourEndText2", XerocTextColor);
                    for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
                    {
                        if (Main.player[playerIndex].active)
                        {
                            Player player = Main.player[playerIndex];
                            if (player.FindBuffIndex(ModContent.BuffType<ExtremeGravity>()) > -1)
                                player.ClearBuff(ModContent.BuffType<ExtremeGravity>());
                        }
                    }
                    SoundEngine.PlaySound(SupremeCalamitas.SpawnSound, Main.player[ClosestPlayerToWorldCenter].Center);
                    CalamityUtils.SpawnBossBetter(Main.player[ClosestPlayerToWorldCenter].Top - new Vector2(42f, 84f), type);
                }, dimnessFactor: 0.6f, permittedNPCs: new int[] { ModContent.NPCType<SCalWormArm>(), ModContent.NPCType<SCalWormHead>(), ModContent.NPCType<SCalWormBody>(), ModContent.NPCType<SCalWormBodyWeak>(), ModContent.NPCType<SCalWormTail>(),
                    ModContent.NPCType<SoulSeekerSupreme>(), ModContent.NPCType<BrimstoneHeart>(), ModContent.NPCType<SupremeCataclysm>(), ModContent.NPCType<SupremeCatastrophe>() }),

                new Boss(ModContent.NPCType<Yharon>(), TimeChangeContext.Day),

                new Boss(ModContent.NPCType<DevourerofGodsHead>(), TimeChangeContext.Day, type =>
                {
                    Player player = Main.player[ClosestPlayerToWorldCenter];

                    SoundEngine.PlaySound(DevourerofGodsHead.SpawnSound, player.Center);
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                }, usesSpecialSound: true, permittedNPCs: new int[] { ModContent.NPCType<DevourerofGodsBody>(), ModContent.NPCType<DevourerofGodsTail>() })
            };

            BossDeathEffects = new Dictionary<int, Action<NPC>>()
            {
                [NPCID.TheDestroyer] = npc =>
                {
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.BossRushTierOneEndText", XerocTextColor);
                },
                [NPCID.CultistBoss] = npc =>
                {
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.BossRushTierTwoEndText", XerocTextColor);
                },
                [NPCID.DukeFishron] = npc =>
                {
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.BossRushTierThreeEndText", XerocTextColor);
                },
                [ModContent.NPCType<Providence>()] = npc =>
                {
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.BossRushTierFourEndText", XerocTextColor);
                },
                [ModContent.NPCType<DevourerofGodsHead>()] = npc =>
                {
                    CalamityUtils.KillAllHostileProjectiles();
                    HostileProjectileKillCounter = 3;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(Source, npc.Center, Vector2.Zero, ModContent.ProjectileType<BossRushEndEffectThing>(), 0, 0f, Main.myPlayer);
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
                if (BossRushStage > Bosses.FindIndex(boss => boss.EntityID == NPCID.CultistBoss))
                    return 3;
                if (BossRushStage > Bosses.FindIndex(boss => boss.EntityID == NPCID.TheDestroyer))
                    return 2;
                return 1;
            }
        }

        public static int MusicToPlay
        {
            get
            {
                if (CalamityMod.Instance.musicMod != null)
                    return CalamityMod.Instance.GetMusicFromMusicMod($"BossRushTier{CurrentTier}") ?? 0;

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

                    // Increase cooldown post-Fishron.
                    if (BossRushStage >= Bosses.FindIndex(boss => boss.EntityID == NPCID.DukeFishron))
                        BossRushSpawnCountdown += 300;

                    // Override the spawn countdown if specified.
                    if (BossRushStage < Bosses.Count - 1 && Bosses[BossRushStage + 1].SpecialSpawnCountdown != -1)
                        BossRushSpawnCountdown = Bosses[BossRushStage + 1].SpecialSpawnCountdown;

                    // Change time as necessary.
                    if (Bosses[BossRushStage].ToChangeTimeTo != TimeChangeContext.None)
                        CalamityUtils.ChangeTime(Bosses[BossRushStage].ToChangeTimeTo == TimeChangeContext.Day);

                    // Play the typical boss roar sound.
                    if (!Bosses[BossRushStage].UsesSpecialSound)
                        SoundEngine.PlaySound(SoundID.Roar, Main.player[ClosestPlayerToWorldCenter].position);

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
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                EndEffects();
                return;
            }

            var netMessage = CalamityMod.Instance.GetPacket();
            netMessage.Write((byte)CalamityModMessageType.EndBossRush);
            netMessage.Send();
        }

        internal static void EndEffects()
        {
            for (int doom = 0; doom < Main.maxNPCs; doom++)
            {
                NPC n = Main.npc[doom];
                if (!n.active)
                    continue;

                // will also correctly despawn EoW because none of his segments are boss flagged
                bool shouldDespawn = n.boss || n.type == NPCID.EaterofWorldsHead || n.type == NPCID.EaterofWorldsBody || n.type == NPCID.EaterofWorldsTail;
                if (shouldDespawn)
                {
                    n.life = 0;
                    n.HitEffect();
                    n.checkDead();
                    n.active = false;
                }
            }

            BossRushActive = false;
            BossRushStage = 0;
            StartTimer = 0;
            EndTimer = 0;
            CalamityUtils.KillAllHostileProjectiles();

            // Send the EndBossRush packet again if this is called serverside to ensure that the changes are recieved by clients.
            if (Main.netMode == NetmodeID.Server)
                End();
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
            else if (npc.type == ModContent.NPCType<SlimeGodCore>() || npc.type == ModContent.NPCType<SplitEbonianSlimeGod>() || npc.type == ModContent.NPCType<SplitCrimulanSlimeGod>())
            {
                if (npc.type == ModContent.NPCType<SlimeGodCore>() && !NPC.AnyNPCs(ModContent.NPCType<SplitEbonianSlimeGod>()) && !NPC.AnyNPCs(ModContent.NPCType<SplitCrimulanSlimeGod>()) &&
                    !NPC.AnyNPCs(ModContent.NPCType<EbonianSlimeGod>()) && !NPC.AnyNPCs(ModContent.NPCType<CrimulanSlimeGod>()))
                {
                    BossRushStage++;
                    CalamityUtils.KillAllHostileProjectiles();
                    HostileProjectileKillCounter = 3;
                }
                else if (npc.type == ModContent.NPCType<SplitEbonianSlimeGod>() && !NPC.AnyNPCs(ModContent.NPCType<SlimeGodCore>()) && !NPC.AnyNPCs(ModContent.NPCType<SplitCrimulanSlimeGod>()) &&
                    NPC.CountNPCS(ModContent.NPCType<SplitEbonianSlimeGod>()) < 2 && !NPC.AnyNPCs(ModContent.NPCType<CrimulanSlimeGod>()))
                {
                    BossRushStage++;
                    CalamityUtils.KillAllHostileProjectiles();
                    HostileProjectileKillCounter = 3;
                }
                else if (npc.type == ModContent.NPCType<SplitCrimulanSlimeGod>() && !NPC.AnyNPCs(ModContent.NPCType<SlimeGodCore>()) && !NPC.AnyNPCs(ModContent.NPCType<SplitEbonianSlimeGod>()) &&
                    NPC.CountNPCS(ModContent.NPCType<SplitCrimulanSlimeGod>()) < 2 && !NPC.AnyNPCs(ModContent.NPCType<EbonianSlimeGod>()))
                {
                    BossRushStage++;
                    CalamityUtils.KillAllHostileProjectiles();
                    HostileProjectileKillCounter = 3;
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
                    BossDeathEffects[npc.type].Invoke(npc);
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
