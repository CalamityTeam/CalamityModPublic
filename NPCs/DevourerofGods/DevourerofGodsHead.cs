using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Placeables.FurnitureCosmilite;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Melee.Yoyos;
using CalamityMod.UI.DialogueDisplay;
using CalamityMod.UI.DialogueDisplay.DisplayEffects;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DevourerofGods
{
    [LongDistanceNetSync]
    public class DevourerofGodsHead : ModNPC
    {
        public static Color SpecialMoveColor => !CalamityClientConfig.Instance.TextEffects ? Color.Cyan : Color.Lerp(Color.Fuchsia, Color.Cyan, MathHelper.SmoothStep(0, 1, (MathF.Sin(Main.GlobalTimeWrappedHourly * 2) + 1) * 0.5f));

        public static int phase1IconIndex;
        public static int phase2IconIndex;

        public static Asset<Texture2D> Texture_Glow_Purple;
        public static Asset<Texture2D> Texture_Glow_Cyan;
        public static Asset<Texture2D> TextureP2;
        public static Asset<Texture2D> TextureP2_Glow_Purple;
        public static Asset<Texture2D> TextureP2_Glow_Cyan;

        public static Asset<Texture2D> JawTexture;
        public static Asset<Texture2D> JawTexture_Glow;
        public static Asset<Texture2D> JawTextureP2;
        public static Asset<Texture2D> JawTextureP2_Glow;
        public static Asset<Texture2D> GodSlayerDashJawTexture;

        public static Asset<Texture2D> TextureP2_Full;

        public override void Load()
        {
            string phase1IconPath = "CalamityMod/NPCs/DevourerofGods/DevourerofGodsHead_Head_Boss";
            string phase2IconPath = "CalamityMod/NPCs/DevourerofGods/DevourerofGodsHead_P2_Head_Boss";

            phase1IconIndex = CalamityMod.Instance.AddBossHeadTexture(phase1IconPath, -1);
            phase2IconIndex = CalamityMod.Instance.AddBossHeadTexture(phase2IconPath, -1);
        }

        // Laser velocity
        private const float laserVelocity = 14f;

        // Phase 1 variables

        // Laser spread variables
        private const int totalShots = 10;

        // Continuously reset variables
        public bool AttemptingToEnterPortal = false;
        public int PortalIndex = -1;

        // Spawn variables
        private bool tail = false;
        private int minLength = 100;
        private int maxLength = 101;

        // Phase variables
        private bool spawnedGuardians = false;
        private bool spawnedGuardians2 = false;
        private int spawnDoGCountdown = 0;
        private bool hasCreatedPhase1Portal = false;
        public bool Phase2Started = false;
        public bool AwaitingPhase2Teleport = true;

        // Phase 2 variables

        // Enums
        private enum LaserWallPhase
        {
            SetUp = 0,
            FireLaserWalls = 1,
            End = 2
        }

        // Laser wall variables
        public int laserWallPhase = 0;

        // Phase variables
        private const int idleCounterMax = 300;
        private int idleCounter = idleCounterMax;
        public const float LaserWallCooldown = 1800f;
        public int postTeleportTimer = 0;
        public int teleportTimer = -1;
        private const int TimeBeforeTeleport_Death = 150;
        private const int TimeBeforeTeleport_Revengeance = 150;
        private const int TimeBeforeTeleport_Expert = 160;
        private const int TimeBeforeTeleport_Normal = 180;
        private bool spawnedGuardians3 = false;
        private const float AlphaGateValue = 1660f;
        public const float SkyColorTransitionTime = 90f;
        public Vector2 PortalEntryLocation = Vector2.Zero;
        public bool doTpFX = true;
        public int dashes = 0;

        // Death animation variables
        public bool Dying;
        public int DeathAnimationTimer;
        public int DestroyedSegmentCount;

        // Jaw related variables.
        public float JawRotation;
        public float JawChompDownProgress;
        public float GodSlayerDashJawFadeProgress;
        public float GodSlayerDashJawTimer;
        public bool ShouldSpawnChompVFX;

        // Sounds
        public static readonly SoundStyle SpawnSound = new("CalamityMod/Sounds/Custom/DevourerSpawn");
        public static readonly SoundStyle AttackSound = new("CalamityMod/Sounds/Custom/DevourerAttack");
        public static readonly SoundStyle RiftOpenSound = new("CalamityMod/Sounds/Custom/DevourerRiftOpen");
        public static readonly SoundStyle RiftBuildingSound = new("CalamityMod/Sounds/Custom/DevourerRiftBuilding");
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/NPCHit/OtherworldlyHit");
        public static readonly SoundStyle DeathAnimationSound = new("CalamityMod/Sounds/NPCKilled/DevourerDeath");
        public static readonly SoundStyle DeathExplosionSound = new("CalamityMod/Sounds/NPCKilled/DevourerDeathImpact");
        public static readonly SoundStyle DeathSegmentSound = new("CalamityMod/Sounds/NPCKilled/DevourerSegmentBreak", 4);
        public float extrapitch = 0;

        public bool isInPassiveState
        {
            get
            {
                if (Phase2Started && NPC.ai[3] == 1)
                    return true;
                if (!Phase2Started && NPC.ai[3] == 0)
                    return true;
                return false;
            }
        }

        public bool isInAgressiveState
        {
            get
            {
                if (Phase2Started && NPC.ai[3] == 0)
                    return true;
                if (!Phase2Started && NPC.ai[3] == 1)
                    return true;
                return false;
            }
        }

        public bool isInLaserWallState
        {
            get
            {
                if (NPC.ai[3] == 2)
                    return true;
                return false;
            }
        }

        public bool isInPostWallState
        {
            get
            {
                if (NPC.ai[3] > 2)
                    return true;
                return false;
            }
        }
        /// <summary>
        /// This gets the player's center, adjusted for predictiveness on FTW
        /// </summary>
        /// <returns></returns>
        Vector2 AdjustedPlayerCenter(float predictiveness = 0)
        {
            Player player = Main.player[NPC.target];
            if (!(Main.getGoodWorld))
                return player.Center;
            return player.Center + player.velocity * predictiveness;
        }
        public override void SetStaticDefaults()
        {
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.6f,
                PortraitScale = 0.6f,
                PortraitPositionXOverride = 60,
                PortraitPositionYOverride = 40
            };
            value.Position.X += 82f;
            value.Position.Y += 38f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            if (!Main.dedServ)
            {
                Texture_Glow_Purple = ModContent.Request<Texture2D>(Texture + "_Glow_Purple", AssetRequestMode.AsyncLoad);
                Texture_Glow_Cyan = ModContent.Request<Texture2D>(Texture + "_Glow_Cyan", AssetRequestMode.AsyncLoad);
                TextureP2 = ModContent.Request<Texture2D>(Texture + "_Jawless_P2", AssetRequestMode.AsyncLoad);
                TextureP2_Glow_Purple = ModContent.Request<Texture2D>(Texture + "_Jawless_P2_Glow_Purple", AssetRequestMode.AsyncLoad);
                TextureP2_Glow_Cyan = ModContent.Request<Texture2D>(Texture + "_Jawless_P2_Glow_Cyan", AssetRequestMode.AsyncLoad);

                JawTexture = ModContent.Request<Texture2D>(Texture + "_Jaw", AssetRequestMode.AsyncLoad);
                JawTexture_Glow = ModContent.Request<Texture2D>(Texture + "_Jaw_Glow", AssetRequestMode.AsyncLoad);
                JawTextureP2 = ModContent.Request<Texture2D>(Texture + "_Jaw_P2", AssetRequestMode.AsyncLoad);
                JawTextureP2_Glow = ModContent.Request<Texture2D>(Texture + "_Jaw_P2_Glow", AssetRequestMode.AsyncLoad);
                GodSlayerDashJawTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/DevourerofGods/GodSlayerDashJaw", AssetRequestMode.AsyncLoad);

                // For bestiary.
                TextureP2_Full = ModContent.Request<Texture2D>(Texture + "_P2", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.DevourerofGods")
            });
        }

        public static int LaserWallDamage = 75; // 300
        public static int LaserWallMiddleBeamDamage = 85; // 340
        public static int FireballDamage = 55; // 220

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 235; // 470
            NPC.npcSlots = 5f;
            NPC.width = 104;
            NPC.height = 104;
            NPC.defense = 50;
            NPC.LifeMaxNERB(750000, 900000, 1500000);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.value = Item.buyPrice(platinum: 1, gold: 50);
            NPC.Opacity = 0f;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.ai[3] = 1;
            NPC.netAlways = true;
            if (Main.rand.NextBool())
                NPC.velocity = new Vector2(-60, 0);
            else
                NPC.velocity = new Vector2(60, 0);
            if (Main.zenithWorld)
                NPC.scale *= 1.5f;
            if (Main.zenithWorld)
                NPC.takenDamageMultiplier = 2;


            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToSickness = false;

            NPC.life = NPC.lifeMax;
        }

        public override void BossHeadSlot(ref int index)
        {
            if ((Phase2Started && (NPC.localAI[2] > 60f || AwaitingPhase2Teleport)) || NPC.Opacity < 0.1f)
                index = -1;
            else if (Phase2Started && !AwaitingPhase2Teleport)
                index = phase2IconIndex;
            else
                index = phase1IconIndex;
        }

        public override void BossHeadRotation(ref float rotation)
        {
            if (Phase2Started && NPC.localAI[2] <= 60f)
                rotation = NPC.rotation;
        }

        public override void OnSpawn(IEntitySource source)
        {
            string key = "Mods.CalamityMod.Status.Boss.DoGSpawn";
            Color messageColor = Color.Cyan;
            CalamityUtils.BroadcastLocalizedText(key, messageColor);

            DialogueDisplaySystem.StartDialogue("Mods.CalamityMod.DevourerOfGods.Phases", NPC, 0, 120, false, new BossText());
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            bool wasDyingBefore = Dying;

            // Velocity sync
            writer.Write(NPC.Calamity().velocityPriorToPhaseSwap);

            // Phase 1 syncs
            writer.Write(NPC.dontTakeDamage);
            writer.Write(spawnedGuardians);
            writer.Write(spawnedGuardians2);
            writer.Write(spawnedGuardians3);
            writer.Write(Phase2Started);
            writer.Write(hasCreatedPhase1Portal);
            writer.Write(AwaitingPhase2Teleport);
            writer.Write(spawnDoGCountdown);
            writer.Write(PortalIndex);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);

            // Phase 2 syncs
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            writer.Write(idleCounter);
            writer.Write(laserWallPhase);
            writer.Write(postTeleportTimer);
            writer.Write(teleportTimer);
            writer.Write(NPC.Opacity);

            // Death animation syncs
            writer.Write(Dying);
            writer.Write(DeathAnimationTimer);
            writer.Write(DestroyedSegmentCount);

            // Jaw animation syncs
            writer.Write(JawRotation);
            writer.Write(JawChompDownProgress);
            writer.Write(GodSlayerDashJawFadeProgress);
            writer.Write(GodSlayerDashJawTimer);
            writer.Write(ShouldSpawnChompVFX);

            // Frame syncs
            writer.Write(NPC.frame.X);
            writer.Write(NPC.frame.Y);
            writer.Write(NPC.frame.Width);
            writer.Write(NPC.frame.Height);

            // Misc syncs
            writer.Write(extrapitch);

            // Be sure to inform clients of the fact that The Devourer of Gods is dying if only the server recieved this packet.
            if (Main.dedServ && !wasDyingBefore && Dying)
                NPC.ForceNetUpdate();
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            // Velocity sync
            NPC.Calamity().velocityPriorToPhaseSwap = reader.ReadSingle();

            // Phase 1 syncs
            NPC.dontTakeDamage = reader.ReadBoolean();
            spawnedGuardians = reader.ReadBoolean();
            spawnedGuardians2 = reader.ReadBoolean();
            spawnedGuardians3 = reader.ReadBoolean();
            Phase2Started = reader.ReadBoolean();
            hasCreatedPhase1Portal = reader.ReadBoolean();
            AwaitingPhase2Teleport = reader.ReadBoolean();
            spawnDoGCountdown = reader.ReadInt32();
            PortalIndex = reader.ReadInt32();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();

            // Phase 2 syncs
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            idleCounter = reader.ReadInt32();
            laserWallPhase = reader.ReadInt32();
            postTeleportTimer = reader.ReadInt32();
            teleportTimer = reader.ReadInt32();
            NPC.Opacity = reader.ReadSingle();

            // Death animation syncs
            Dying = reader.ReadBoolean();
            DeathAnimationTimer = reader.ReadInt32();
            DestroyedSegmentCount = reader.ReadInt32();

            // Jaw animation syncs
            JawRotation = reader.ReadSingle();
            JawChompDownProgress = reader.ReadSingle();
            GodSlayerDashJawFadeProgress = reader.ReadSingle();
            GodSlayerDashJawTimer = reader.ReadSingle();
            ShouldSpawnChompVFX = reader.ReadBoolean();

            // Misc syncs
            extrapitch = reader.ReadSingle();

            // Frame syncs
            Rectangle frame = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
            if (frame.Width > 0 && frame.Height > 0)
                NPC.frame = frame;
        }

        public override void AI()
        {

            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            // whoAmI variable
            CalamityGlobalNPC.DoGHead = NPC.whoAmI;
            CalamityGlobalNPC.DoGP2 = -1;

            // Stop rain
            if (CalamityServerConfig.Instance.BossesStopWeather)
                CalamityWorld.StopRain();

            // Get a target (time is checked in the second check to ensure a new target isn't being set constantly)
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();
            else if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles && Main.time % 60D == 0D)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            // Use the shimmer background effect if fancy background visuals are disabled.
            if (DeathAnimationTimer < 435 && !CalamityClientConfig.Instance.FancyBackgroundVisuals)
            {
                player.shimmerMonolithShader = true;
                if (Main.shimmerDarken > 0.75f)
                    Main.shimmerDarken = 0.75f;
            }

            // Variables
            bool flies = NPC.ai[3] == 0f;

            if (Main.getGoodWorld)
                flies = true;
            Vector2 destination = AdjustedPlayerCenter(2);
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phase 1 phases
            bool phase2 = lifeRatio < 1f;
            bool phase3 = lifeRatio < 0.75f;
            bool bigDaddyPhase2 = lifeRatio < 0.65f;

            // Phase 2 phases
            bool phase4 = lifeRatio < 0.65f;
            bool phase5 = lifeRatio < 0.5f;
            bool phase6 = lifeRatio < 0.25f;

            // Sound pitch
            extrapitch = Main.zenithWorld ? 0.3f : 0f;

            // Velocity variables
            float segmentVelocity = death ? 17.5f : 16f;
            if (expertMode)
                segmentVelocity += 4f * (1f - (lifeRatio * 0.75f + 0.25f));

            float speed = death ? 16.5f : 15f;
            float turnSpeed = death ? 0.33f : 0.3f;
            float homingSpeed = death ? 30f : 24f;
            float homingTurnSpeed = 0.405f;

            if (expertMode)
            {
                speed += 3f * (1f - (lifeRatio * 0.75f + 0.25f));
                turnSpeed += 0.06f * (1f - (lifeRatio * 0.75f + 0.25f));
                homingSpeed += 12f * (1f - (lifeRatio * 0.75f + 0.25f));
                homingTurnSpeed += 0.15f * (1f - (lifeRatio * 0.75f + 0.25f));
            }

            float groundPhaseTurnSpeed = death ? 0.24f : 0.21f;

            if (expertMode)
                groundPhaseTurnSpeed += 0.1f * (1f - (lifeRatio * 0.75f + 0.25f));

            groundPhaseTurnSpeed += Vector2.Distance(destination, NPC.Center) * 0.0002f;

            if (Vector2.Distance(destination, NPC.Center) < 320)
                groundPhaseTurnSpeed *= 0.75f;

            // How long it takes before swapping phases
            int phaseLimit = 900;
            if (spawnedGuardians3)
                phaseLimit -= 180;

            if (Main.getGoodWorld)
            {
                homingSpeed *= 3f;
                homingTurnSpeed *= NPC.Distance(destination) / 500f;
                if (NPC.Distance(destination) > 1400)
                    NPC.ai[1] = 1;
                if (NPC.ai[1] == -1)
                {
                    NPC.ai[1] = 0;
                }
                else
                if (NPC.ai[1] == 1)
                {
                    homingTurnSpeed *= 5;
                    float ftwDotProduct = Vector2.Dot(NPC.DirectionTo(destination), NPC.velocity.SafeNormalize(Vector2.Zero));
                    if (ftwDotProduct > 0.95f)
                    {
                        NPC.ai[1] = -1;

                    }
                }
                else
                    speed *= 5;
                if (NPC.ai[3] == 0)
                    phaseLimit += 600;
                else
                    phaseLimit -= 600;

                if (lifeRatio < 0.1f && NPC.ai[3] <= 1f)
                {
                    SpawnTeleportLocation(player);
                }
            }

            // Continuously reset certain things.
            AttemptingToEnterPortal = false;


            // Despawn
            if (player.dead)
            {
                NPC.ai[3] = 3f;
                calamityGlobalNPC.newAI[2] = -1f;

                NPC.velocity.Y -= 4f;

                int bodyType = ModContent.NPCType<DevourerofGodsBody>();
                int tailType = ModContent.NPCType<DevourerofGodsTail>();
                if ((double)NPC.position.Y < Main.topWorld + 16f)
                {
                    for (int a = 0; a < Main.maxNPCs; a++)
                    {
                        if (Main.npc[a].type != NPC.type && Main.npc[a].type != bodyType && Main.npc[a].type != tailType)
                            continue;

                        Main.npc[a].active = false;
                        Main.npc[a].ForceNetUpdate(false);
                    }
                }
            }

            float distanceFromTarget = Vector2.Distance(destination, NPC.Center);
            bool increaseSpeed = distanceFromTarget > CalamityGlobalNPC.CatchUpDistance200Tiles;
            bool increaseSpeedMore = distanceFromTarget > CalamityGlobalNPC.CatchUpDistance350Tiles * 2;

            // Close DoG's HP bar during P2 transition and decrement the countdown.
            if (NPC.localAI[2] > 0f)
            {
                NPC.localAI[2] -= 1f;
                NPC.Calamity().ShouldCloseHPBar = true;
            }

            // Teleport after the Phase 2 animation.
            float timeWhenDoGShouldTeleportDuringPhase2Countdown = 61f;
            if (NPC.localAI[2] == timeWhenDoGShouldTeleportDuringPhase2Countdown + (death ? TimeBeforeTeleport_Death : CalamityWorld.revenge ? TimeBeforeTeleport_Revengeance : Main.expertMode ? TimeBeforeTeleport_Expert : TimeBeforeTeleport_Normal))
                SpawnTeleportLocation(player, true);

            if (NPC.localAI[2] == timeWhenDoGShouldTeleportDuringPhase2Countdown)
                Teleport(player, death, revenge, expertMode, phase5);

            // Just in case the projectile cap is reached and the teleport rift doesn't spawn.
            if (AwaitingPhase2Teleport && NPC.localAI[2] == 0f)
                AwaitingPhase2Teleport = false;

            // Be invincibile until the phase 2 teleport happens.
            // This is done to prevent DoG from suddenly and weirdly re-appearing after entering the phase 1 portal.
            // Once the teleport happens he will be in position and this effect stops.
            if (Phase2Started && AwaitingPhase2Teleport && NPC.localAI[2] < 60f)
            {
                NPC.Opacity = 0f;
                NPC.dontTakeDamage = true;
            }
            // Start phase 2, only run things that have to happen once in here
            if (bigDaddyPhase2)
            {
                if (!Phase2Started)
                {
                    Phase2Started = true;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        // Reset important shit
                        NPC.ai[3] = 0f;
                        calamityGlobalNPC.newAI[1] = 0f;
                        calamityGlobalNPC.newAI[2] = 0f;
                        NPC.ForceNetUpdate();
                    }

                    // Phase 2 countdown
                    NPC.localAI[2] = 705f;
                }

                // Play music after the transiton BS
                if (NPC.localAI[2] <= 635f)
                    CalamityGlobalNPC.DoGP2 = NPC.whoAmI;

                // Once before DoG spawns, set new size and become visible again.
                if (NPC.localAI[2] == 60f)
                {
                    NPC.position = NPC.Center;
                    NPC.width = (int)(186 * NPC.scale);
                    NPC.height = (int)(186 * NPC.scale);
                    NPC.position -= NPC.Size * 0.5f;
                    NPC.frame = new Rectangle(0, 0, 134, 196);
                    NPC.ForceNetUpdate(false);
                }

                // Dialogue the moment the second phase starts
                if (NPC.localAI[2] == 60f && !BossRushEvent.BossRushActive)
                {
                    string key = "Mods.CalamityMod.Status.Boss.DoGPhase2";
                    Color messageColor = Color.Cyan;
                    CalamityUtils.BroadcastLocalizedText(key, messageColor);
                    /*                    
                    var ctid = CombatText.NewText(NPC.Hitbox, messageColor, Language.GetTextValue(key), true);
                    if (ctid < Main.maxCombatText)
                        player.Calamity().subtitletext = Main.combatText[ctid];
                    player.Calamity().subtitleColors = new Color[] { Color.Cyan, Color.Fuchsia };
                    */
                    DialogueDisplaySystem.StartDialogue("Mods.CalamityMod.DevourerOfGods.Phases", NPC, 2, 120, false, new BossText());
                }
            }

            // Begin phase 2
            #region Phase 2
            if (Phase2Started)
            {
                #region Transition
                // Go immune and invisible
                if (NPC.localAI[2] > 5f)
                {
                    // Don't take damage
                    NPC.dontTakeDamage = true;

                    // Adjust movement speed. Direction is unaltered unless DoG is close to the top of the world, in which case he moves horizontally.
                    // A portal will be created ahead of where DoG is moving that he will enter before Phase 2 begins.
                    float idealFlySpeed = 28f;

                    float oldVelocity = NPC.velocity.Length();
                    float horizontalInterpolant = Utils.GetLerpValue(1200f, 600f, NPC.Center.Y, true);
                    Vector2 idealDirection = NPC.velocity.SafeNormalize(-Vector2.UnitY);
                    idealDirection = Vector2.Lerp(idealDirection, Vector2.UnitX * Math.Sign(idealDirection.X), horizontalInterpolant);
                    NPC.velocity = idealDirection * MathHelper.Lerp(oldVelocity, idealFlySpeed, 0.1f);
                    NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

                    if (PortalIndex != -1)
                    {
                        Projectile portal = Main.projectile[PortalIndex];
                        float newOpacity = 1f - Utils.GetLerpValue(200f, 130f, NPC.Distance(portal.Center), true);
                        if (Main.netMode != NetmodeID.MultiplayerClient && newOpacity > 0f && NPC.Opacity > newOpacity)
                        {
                            NPC.Opacity = newOpacity;
                            NPC.ForceNetUpdate(false);
                        }

                        if (NPC.Opacity < 0.2f)
                            NPC.Opacity = 0f;

                        // Ensure the portal is pointing in the direction of the head at first, to prevent direction offsets.
                        if (NPC.localAI[2] > 360f)
                            Main.projectile[PortalIndex].Center = NPC.Center + NPC.SafeDirectionTo(Main.projectile[PortalIndex].Center) * NPC.Distance(Main.projectile[PortalIndex].Center);
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient && !hasCreatedPhase1Portal)
                    {
                        Vector2 portalSpawnPosition = NPC.Center + NPC.velocity.SafeNormalize(-Vector2.UnitY) * 1000f;
                        PortalIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), portalSpawnPosition, Vector2.Zero, ModContent.ProjectileType<DoGP1EndPortal>(), 0, 0f);

                        hasCreatedPhase1Portal = true;
                        NPC.ForceNetUpdate(false);
                    }

                    AttemptingToEnterPortal = true;
                }
                #endregion

                // Phase 2
                else
                {
                    #region Misc Stuff I Want To Be Able To Collapse
                    // Immunity after teleport and when dying
                    NPC.dontTakeDamage = postTeleportTimer > 0 || Dying;

                    // Teleport countdown
                    if (teleportTimer > 0)
                    {
                        teleportTimer--;
                        // Teleport
                        if (teleportTimer == 0)
                            Teleport(player, death, revenge, expertMode, phase5);
                    }

                    // Do the death animation once killed.
                    if (Dying)
                    {
                        teleportTimer = 0;
                        DoDeathAnimation();
                        return;
                    }
                    // Trigger the death animation
                    else if (NPC.life == 1)
                    {
                        Dying = true;
                        NPC.dontTakeDamage = true;
                        NPC.ForceNetUpdate(false);

                        return;
                    }
                    #endregion

                    // Laser walls
                    #region Laser Walls
                    float adjustedAlphaGateValue = AlphaGateValue;
                    if (phase4 && postTeleportTimer <= 0)
                    {
                        if (laserWallPhase == (int)LaserWallPhase.SetUp)
                        {
                            // Enter laser wall phase very quickly when final phase starts
                            if (phase6 && !spawnedGuardians3 && calamityGlobalNPC.newAI[3] < adjustedAlphaGateValue && NPC.ai[3] < 2)
                            {
                                NPC.ai[3] = 2;
                                calamityGlobalNPC.newAI[3] = adjustedAlphaGateValue;
                            }

                            // Increment next laser wall phase timer
                            if (NPC.ai[3] == 2 || (!spawnedGuardians3 && phase6 && NPC.ai[3] < 2))
                                calamityGlobalNPC.newAI[3] += 1f;

                            // Set alpha value prior to firing laser walls
                            if (calamityGlobalNPC.newAI[3] > adjustedAlphaGateValue)
                            {
                                // Disable teleports
                                if (teleportTimer > 0)
                                {
                                    GetRiftLocation(true);
                                    teleportTimer = 0;
                                }
                            }

                            // Fire laser walls every X seconds after a laser wall phase ends
                            float laserWallGateValue = LaserWallCooldown;
                            if (calamityGlobalNPC.newAI[3] >= laserWallGateValue)
                            {

                                // Reset laser wall timer to 0
                                calamityGlobalNPC.newAI[1] = 0f;

                                calamityGlobalNPC.newAI[3] = 0f;
                                laserWallPhase = (int)LaserWallPhase.FireLaserWalls;
                            }
                        }
                        else if (laserWallPhase == (int)LaserWallPhase.FireLaserWalls)
                        {
                            // Remain in laser wall firing phase for X seconds
                            idleCounter--;
                            if (idleCounter <= 0)
                            {
                                laserWallPhase = (int)LaserWallPhase.End;
                                idleCounter = idleCounterMax;
                                NPC.ai[3] = 3;
                                calamityGlobalNPC.newAI[2] = 0;
                            }
                        }
                        else if (laserWallPhase == (int)LaserWallPhase.End)
                        {
                            // End laser wall phase after 4.1667 seconds
                            float totalTimeBeforeFullOpacity = 250f;
                            float timeBeforeTeleportHappens = death ? TimeBeforeTeleport_Death : CalamityWorld.revenge ? TimeBeforeTeleport_Revengeance : Main.expertMode ? TimeBeforeTeleport_Expert : TimeBeforeTeleport_Normal;
                            float opacityIncrement = 1f / (totalTimeBeforeFullOpacity - timeBeforeTeleportHappens);
                            if (teleportTimer == 0)
                                NPC.Opacity += opacityIncrement;

                            if (NPC.Opacity >= 1f)
                            {
                                // Don't do contact damage while flying away after laser walls
                                NPC.damage = 0;
                                NPC.Opacity = 1f;
                                laserWallPhase = (int)LaserWallPhase.SetUp;

                                // Enter final phase
                                if (!spawnedGuardians3 && phase6)
                                {
                                    // Reset laser wall timers to 0
                                    calamityGlobalNPC.newAI[1] = 0f;
                                    calamityGlobalNPC.newAI[3] = 0f;

                                    // Anger message
                                    if (!BossRushEvent.BossRushActive)
                                    {
                                        string key = "Mods.CalamityMod.Status.Boss.DoGPhase3";
                                        Color messageColor = Color.Cyan;
                                        CalamityUtils.BroadcastLocalizedText(key, messageColor);
                                        Vector2 start = GetRiftLocation();

                                        DialogueDisplaySystem.StartDialogue("Mods.CalamityMod.DevourerOfGods.Phases", start, 3, 120, false, new BossText());
                                    }

                                    spawnedGuardians3 = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        // Set opacity after teleport
                        if (postTeleportTimer > 0)
                        {
                            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;

                            postTeleportTimer--;
                            NPC.Opacity = 1f - (postTeleportTimer / 255f);
                            if (NPC.ai[3] < 2)
                                calamityGlobalNPC.newAI[2] = 0;
                        }
                        else
                        {
                            NPC.Opacity += 0.024f;
                            if (NPC.Opacity > 1f)
                                NPC.Opacity = 1f;
                        }
                    }

                    // Fireballs
                    if (isInPassiveState)
                    {
                        ShootFireballs(player, distanceFromTarget, revenge);
                    }
                    else
                    {
                        calamityGlobalNPC.newAI[0] = 0f;
                    }

                    // Laser walls
                    if (laserWallPhase == (int)LaserWallPhase.FireLaserWalls && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (death && phase6)
                        {
                            float spacing = 320;
                            float miniInterval = 12;
                            float megaInterval = 120;
                            float time = 0.35f;
                            for (var i = 0; i < 3; i++)
                                if ((int)(calamityGlobalNPC.newAI[1] - miniInterval * i) % megaInterval == 0f)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center + Main.rand.NextVector2CircularEdge(600, 600), Vector2.Zero, ModContent.ProjectileType<DoGLaserWalls>(), LaserWallDamage, 0, Main.myPlayer, time, spacing, 2 - i);
                                    if (i == 2)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<DoGLaserWallsBigBeam>(), LaserWallMiddleBeamDamage, 0, Main.myPlayer, time, 0, i);
                                    else if (Main.zenithWorld)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center + Main.rand.NextVector2CircularEdge(600, 600), Vector2.Zero, ModContent.ProjectileType<DoGLaserWalls>(), LaserWallDamage, 0, Main.myPlayer, time, 240, 6);
                                }
                            calamityGlobalNPC.newAI[1] += 1f;
                        }
                        else
                        {
                            float divisor = death ? 100f : 120f;
                            if (phase6)
                                divisor -= 15;

                            if (calamityGlobalNPC.newAI[1] % divisor == 0f)
                            {
                                float spacing = death ? 144 : 160;
                                if (divisor == 0)
                                    spacing += 64;
                                if (phase6)
                                    spacing += death ? 64 : 32;
                                int bType = Main.rand.Next(0, 5 + 1);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center + Main.rand.NextVector2CircularEdge(600, 600), Vector2.Zero, ModContent.ProjectileType<DoGLaserWalls>(), LaserWallDamage, 0, Main.myPlayer, 0.5f, spacing, bType);
                                if (phase6 || death)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<DoGLaserWallsBigBeam>(), LaserWallMiddleBeamDamage, 0, Main.myPlayer, 0.5f, 0, bType);

                                if (Main.zenithWorld)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center + Main.rand.NextVector2CircularEdge(600, 600), Vector2.Zero, ModContent.ProjectileType<DoGLaserWalls>(), LaserWallDamage, 0, Main.myPlayer, 0.5f, 120, 6);
                            }
                            calamityGlobalNPC.newAI[1] += 1f;
                        }
                    }

                    // Set flight time to max during laser walls
                    if (laserWallPhase == (int)LaserWallPhase.FireLaserWalls)
                    {
                        if (!Main.dedServ)
                        {
                            if (!Main.LocalPlayer.dead && Main.LocalPlayer.active && Vector2.Distance(Main.LocalPlayer.Center, NPC.Center) < CalamityGlobalNPC.CatchUpDistance350Tiles)
                                Main.LocalPlayer.Calamity().infiniteFlight = true;
                        }
                    }
                    #endregion

                    // Movement
                    int tilePositionX = (int)(NPC.position.X / 16f) - 1;
                    int tileWidthPosX = (int)((NPC.position.X + NPC.width) / 16f) + 2;
                    int tilePositionY = (int)(NPC.position.Y / 16f) - 1;
                    int tileWidthPosY = (int)((NPC.position.Y + NPC.height) / 16f) + 2;

                    if (tilePositionX < 0)
                        tilePositionX = 0;
                    if (tileWidthPosX > Main.maxTilesX)
                        tileWidthPosX = Main.maxTilesX;
                    if (tilePositionY < 0)
                        tilePositionY = 0;
                    if (tileWidthPosY > Main.maxTilesY)
                        tileWidthPosY = Main.maxTilesY;

                    if (NPC.velocity.X < 0f)
                        NPC.spriteDirection = -1;
                    else if (NPC.velocity.X > 0f)
                        NPC.spriteDirection = 1;

                    var VelocityRotation = NPC.velocity.ToRotation();
                    void TurnTowards(Vector2 goal, float offset = 0, float maxSpeed = 1)
                    {
                        float goal2 = (goal - NPC.Center).ToRotation() + offset;
                        maxSpeed *= (float)Math.PI / 180f;
                        var dif = MathF.Atan2(MathF.Sin(goal2 - VelocityRotation), MathF.Cos(goal2 - VelocityRotation));
                        if (dif < 0)
                        {
                            if (-dif > maxSpeed)
                                VelocityRotation -= maxSpeed;
                            else
                                VelocityRotation += dif;
                        }
                        else
                        {
                            if (dif > maxSpeed)
                                VelocityRotation += maxSpeed;
                            else
                                VelocityRotation += dif;
                        }
                    }

                    // Flight
                    #region Air AI
                    if (NPC.ai[3] == 0f)
                    {
                        if (!Main.dedServ)
                        {
                            if (!Main.LocalPlayer.dead && Main.LocalPlayer.active && Vector2.Distance(Main.LocalPlayer.Center, NPC.Center) < CalamityGlobalNPC.CatchUpDistance350Tiles)
                                Main.LocalPlayer.AddBuff(ModContent.BuffType<DoGExtremeGravity>(), 2);
                        }

                        // Charge in a direction for a second until the timer is back at 0
                        if (postTeleportTimer > 0)
                        {
                            NPC.damage = NPC.defDamage;

                            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;
                            return;
                        }

                        calamityGlobalNPC.newAI[2] += 1f;

                        NPC.localAI[1] = 0f;

                        // Go to ground phase sooner
                        if (increaseSpeedMore)
                        {
                            if (laserWallPhase == (int)LaserWallPhase.SetUp && calamityGlobalNPC.newAI[3] <= adjustedAlphaGateValue)
                                SpawnTeleportLocation(player);
                        }

                        float speedCopy = speed;
                        float turnSpeedCopy = turnSpeed;
                        Vector2 npcCenter = NPC.Center;
                        float targetX = destination.X;
                        float targetY = destination.Y;
                        int destinationTileX = (int)(destination.X / 16f);
                        int destinationTileY = (int)(destination.Y / 16f);


                        speedCopy = homingSpeed;
                        turnSpeedCopy = homingTurnSpeed;


                        speedCopy += Vector2.Distance(destination, NPC.Center) * 0.005f;
                        turnSpeedCopy += Vector2.Distance(destination, NPC.Center) * 0.00025f;

                        float fasterSpeedMult = speedCopy * 1.3f;
                        float slowerSpeedMult = speedCopy * 0.7f;
                        float npcSpeed = NPC.velocity.Length();
                        if (npcSpeed > 0f)
                        {
                            if (npcSpeed > fasterSpeedMult)
                            {
                                NPC.velocity.Normalize();
                                NPC.velocity *= fasterSpeedMult;
                            }
                            else if (npcSpeed < slowerSpeedMult)
                            {
                                NPC.velocity.Normalize();
                                NPC.velocity *= slowerSpeedMult;
                            }
                        }
                        targetX = (int)(targetX / 16f) * 16;
                        targetY = (int)(targetY / 16f) * 16;
                        npcCenter.X = (int)(npcCenter.X / 16f) * 16;
                        npcCenter.Y = (int)(npcCenter.Y / 16f) * 16;
                        targetX -= npcCenter.X;
                        targetY -= npcCenter.Y;
                        float targetDistance = (float)Math.Sqrt(targetX * targetX + targetY * targetY);
                        float absoluteTargetX = Math.Abs(targetX);
                        float absoluteTargetY = Math.Abs(targetY);
                        float timeToReachTarget = speedCopy / targetDistance;
                        targetX *= timeToReachTarget;
                        targetY *= timeToReachTarget;

                        turnSpeedCopy *= NPC.Distance(destination) / (death ? 800 : 1000f);
                        if ((NPC.velocity.X > 0f && targetX > 0f) || (NPC.velocity.X < 0f && targetX < 0f) || (NPC.velocity.Y > 0f && targetY > 0f) || (NPC.velocity.Y < 0f && targetY < 0f))
                        {
                            if (NPC.velocity.X < targetX)
                                NPC.velocity.X += turnSpeedCopy;
                            else
                            {
                                if (NPC.velocity.X > targetX)
                                    NPC.velocity.X -= turnSpeedCopy;
                            }

                            if (NPC.velocity.Y < targetY)
                                NPC.velocity.Y += turnSpeedCopy;
                            else
                            {
                                if (NPC.velocity.Y > targetY)
                                    NPC.velocity.Y -= turnSpeedCopy;
                            }

                            if (Math.Abs(targetY) < speedCopy * 0.2 && ((NPC.velocity.X > 0f && targetX < 0f) || (NPC.velocity.X < 0f && targetX > 0f)))
                            {
                                if (NPC.velocity.Y > 0f)
                                    NPC.velocity.Y += turnSpeedCopy * 2f;
                                else
                                    NPC.velocity.Y -= turnSpeedCopy * 2f;
                            }

                            if (Math.Abs(targetX) < speedCopy * 0.2 && ((NPC.velocity.Y > 0f && targetY < 0f) || (NPC.velocity.Y < 0f && targetY > 0f)))
                            {
                                if (NPC.velocity.X > 0f)
                                    NPC.velocity.X += turnSpeedCopy * 2f;
                                else
                                    NPC.velocity.X -= turnSpeedCopy * 2f;
                            }
                        }
                        else
                        {
                            if (absoluteTargetX > absoluteTargetY)
                            {
                                if (NPC.velocity.X < targetX)
                                    NPC.velocity.X += turnSpeedCopy * 1.1f;
                                else if (NPC.velocity.X > targetX)
                                    NPC.velocity.X -= turnSpeedCopy * 1.1f;

                                if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < speedCopy * 0.5)
                                {
                                    if (NPC.velocity.Y > 0f)
                                        NPC.velocity.Y += turnSpeedCopy;
                                    else
                                        NPC.velocity.Y -= turnSpeedCopy;
                                }
                            }
                            else
                            {
                                if (NPC.velocity.Y < targetY)
                                    NPC.velocity.Y += turnSpeedCopy * 1.1f;
                                else if (NPC.velocity.Y > targetY)
                                    NPC.velocity.Y -= turnSpeedCopy * 1.1f;

                                if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < speedCopy * 0.5)
                                {
                                    if (NPC.velocity.X > 0f)
                                        NPC.velocity.X += turnSpeedCopy;
                                    else
                                        NPC.velocity.X -= turnSpeedCopy;
                                }
                            }
                        }

                        // Set velocity so that DoG cannot speed burst instantly at the start of a phase swap
                        if (calamityGlobalNPC.velocityPriorToPhaseSwap > 0f)
                        {
                            if (NPC.velocity.Length() > calamityGlobalNPC.velocityPriorToPhaseSwap)
                            {
                                NPC.velocity.Normalize();
                                NPC.velocity *= calamityGlobalNPC.velocityPriorToPhaseSwap;
                                calamityGlobalNPC.velocityPriorToPhaseSwap += CalamityGlobalNPC.velocityPriorToPhaseSwapIncrement;
                            }
                        }

                        NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;

                        if (calamityGlobalNPC.newAI[2] > phaseLimit)
                        {
                            calamityGlobalNPC.velocityPriorToPhaseSwap = NPC.velocity.Length();
                            NPC.ai[3] = 1f;
                            calamityGlobalNPC.newAI[2] = 0f;
                            NPC.TargetClosest();
                            NPC.ForceNetUpdate(false);
                        }
                    }
                    #endregion
                    // Ground
                    #region Ground AI
                    else if (NPC.ai[3] == 1f)
                    {
                        if (!Main.dedServ)
                        {
                            if (!Main.LocalPlayer.dead && Main.LocalPlayer.active && Vector2.Distance(Main.LocalPlayer.Center, NPC.Center) < CalamityGlobalNPC.CatchUpDistance350Tiles)
                                Main.LocalPlayer.AddBuff(ModContent.BuffType<Warped>(), 2);
                        }

                        // Charge in a direction for a second until the timer is back at 0
                        if (postTeleportTimer > 0)
                        {
                            NPC.damage = NPC.defDamage;

                            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;
                            return;
                        }

                        calamityGlobalNPC.newAI[2] += 1f;

                        // Enrage
                        if (increaseSpeedMore)
                        {
                            if (laserWallPhase == (int)LaserWallPhase.SetUp && calamityGlobalNPC.newAI[3] <= adjustedAlphaGateValue)
                                SpawnTeleportLocation(player);
                            else
                                groundPhaseTurnSpeed *= 4f;
                        }
                        else if (increaseSpeed)
                            groundPhaseTurnSpeed *= 2f;

                        #region Digging AI
                        if (!flies)
                        {
                            for (int r = tilePositionX; r < tileWidthPosX; r++)
                            {
                                for (int s = tilePositionY; s < tileWidthPosY; s++)
                                {
                                    if (Main.tile[r, s] != null && ((Main.tile[r, s].HasUnactuatedTile && (Main.tileSolid[Main.tile[r, s].TileType] || (Main.tileSolidTop[Main.tile[r, s].TileType] && Main.tile[r, s].TileFrameY == 0))) || Main.tile[r, s].LiquidAmount > 64))
                                    {
                                        Vector2 positionCheck;
                                        positionCheck.X = r * 16;
                                        positionCheck.Y = s * 16;
                                        if (NPC.position.X + NPC.width > positionCheck.X && NPC.position.X < positionCheck.X + 16f && NPC.position.Y + NPC.height > positionCheck.Y && NPC.position.Y < positionCheck.Y + 16f)
                                        {
                                            flies = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (!flies)
                        {
                            NPC.localAI[1] = 1f;

                            Rectangle rectangle12 = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);

                            int directChargeRange = death ? 1125 : 1200;

                            if (expertMode)
                                directChargeRange -= (int)(150f * (1f - lifeRatio));

                            if (directChargeRange < 1050)
                                directChargeRange = 1050;

                            bool canDirectlyCharge = true;
                            if (NPC.position.Y > player.position.Y)
                            {
                                for (int k = 0; k < Main.maxPlayers; k++)
                                {
                                    if (Main.player[k].active)
                                    {
                                        Rectangle rectangle13 = new Rectangle((int)Main.player[k].position.X - 1000, (int)Main.player[k].position.Y - 1000, 2000, directChargeRange);
                                        if (rectangle12.Intersects(rectangle13))
                                        {
                                            canDirectlyCharge = false;
                                            break;
                                        }
                                    }
                                }
                                if (canDirectlyCharge)
                                    flies = true;
                            }
                        }
                        else
                            NPC.localAI[1] = 0f;

                        float turnSpeedCopy = groundPhaseTurnSpeed;
                        Vector2 npcCenter = NPC.Center;
                        float targetX = destination.X;
                        float targetY = destination.Y;
                        targetX = (int)(targetX / 16f) * 16;
                        targetY = (int)(targetY / 16f) * 16;
                        npcCenter.X = (int)(npcCenter.X / 16f) * 16;
                        npcCenter.Y = (int)(npcCenter.Y / 16f) * 16;
                        targetX -= npcCenter.X;
                        targetY -= npcCenter.Y;

                        if (!flies)
                        {
                            NPC.velocity.Y += groundPhaseTurnSpeed;
                            if (NPC.velocity.Y > segmentVelocity)
                                NPC.velocity.Y = segmentVelocity;

                            // This bool exists to stop the strange wiggle behavior when worms are falling down
                            bool slowXVelocity = Math.Abs(NPC.velocity.X) > turnSpeedCopy;
                            if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < segmentVelocity * 2.2)
                            {
                                if (NPC.velocity.X < 0f)
                                    NPC.velocity.X -= turnSpeedCopy * 1.1f;
                                else
                                    NPC.velocity.X += turnSpeedCopy * 1.1f;
                            }
                            else if (NPC.velocity.Y == segmentVelocity)
                            {
                                if (slowXVelocity)
                                {
                                    if (NPC.velocity.X < targetX)
                                        NPC.velocity.X += turnSpeedCopy;
                                    else if (NPC.velocity.X > targetX)
                                        NPC.velocity.X -= turnSpeedCopy;
                                }
                                else
                                    NPC.velocity.X = 0f;
                            }
                            else if (NPC.velocity.Y > 4f)
                            {
                                if (slowXVelocity)
                                {
                                    if (NPC.velocity.X < 0f)
                                        NPC.velocity.X += turnSpeedCopy * 0.9f;
                                    else
                                        NPC.velocity.X -= turnSpeedCopy * 0.9f;
                                }
                                else
                                    NPC.velocity.X = 0f;
                            }
                        }
                        #endregion

                        #region Flying AI
                        else
                        {
                            double maximumSpeed1 = death ? 0.46 : 0.4;
                            double maximumSpeed2 = death ? 1.125 : 1D;

                            if (expertMode)
                            {
                                maximumSpeed1 += 0.1f * (1f - lifeRatio);
                                maximumSpeed2 += 0.2f * (1f - lifeRatio);
                            }

                            float targetDistance = (float)Math.Sqrt(targetX * targetX + targetY * targetY);
                            float absoluteTargetX2 = Math.Abs(targetX);
                            float absoluteTargetY2 = Math.Abs(targetY);
                            float timeToReachTarget2 = segmentVelocity / targetDistance;
                            targetX *= timeToReachTarget2;
                            targetY *= timeToReachTarget2;

                            if (((NPC.velocity.X > 0f && targetX > 0f) || (NPC.velocity.X < 0f && targetX < 0f)) && ((NPC.velocity.Y > 0f && targetY > 0f) || (NPC.velocity.Y < 0f && targetY < 0f)))
                            {
                                if (NPC.velocity.X < targetX)
                                    NPC.velocity.X += groundPhaseTurnSpeed * 1.5f;
                                else if (NPC.velocity.X > targetX)
                                    NPC.velocity.X -= groundPhaseTurnSpeed * 1.5f;

                                if (NPC.velocity.Y < targetY)
                                    NPC.velocity.Y += groundPhaseTurnSpeed * 1.5f;
                                else if (NPC.velocity.Y > targetY)
                                    NPC.velocity.Y -= groundPhaseTurnSpeed * 1.5f;
                            }

                            if ((NPC.velocity.X > 0f && targetX > 0f) || (NPC.velocity.X < 0f && targetX < 0f) || (NPC.velocity.Y > 0f && targetY > 0f) || (NPC.velocity.Y < 0f && targetY < 0f))
                            {
                                if (NPC.velocity.X < targetX)
                                    NPC.velocity.X += groundPhaseTurnSpeed;
                                else if (NPC.velocity.X > targetX)
                                    NPC.velocity.X -= groundPhaseTurnSpeed;

                                if (NPC.velocity.Y < targetY)
                                    NPC.velocity.Y += groundPhaseTurnSpeed;
                                else if (NPC.velocity.Y > targetY)
                                    NPC.velocity.Y -= groundPhaseTurnSpeed;

                                if (Math.Abs(targetY) < segmentVelocity * maximumSpeed1 && ((NPC.velocity.X > 0f && targetX < 0f) || (NPC.velocity.X < 0f && targetX > 0f)))
                                {
                                    if (NPC.velocity.Y > 0f)
                                        NPC.velocity.Y += groundPhaseTurnSpeed * 2f;
                                    else
                                        NPC.velocity.Y -= groundPhaseTurnSpeed * 2f;
                                }

                                if (Math.Abs(targetX) < segmentVelocity * maximumSpeed1 && ((NPC.velocity.Y > 0f && targetY < 0f) || (NPC.velocity.Y < 0f && targetY > 0f)))
                                {
                                    if (NPC.velocity.X > 0f)
                                        NPC.velocity.X += groundPhaseTurnSpeed * 2f;
                                    else
                                        NPC.velocity.X -= groundPhaseTurnSpeed * 2f;
                                }
                            }
                            else if (absoluteTargetX2 > absoluteTargetY2)
                            {
                                if (NPC.velocity.X < targetX)
                                    NPC.velocity.X += groundPhaseTurnSpeed * 1.1f;
                                else if (NPC.velocity.X > targetX)
                                    NPC.velocity.X -= groundPhaseTurnSpeed * 1.1f;

                                if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < segmentVelocity * maximumSpeed2)
                                {
                                    if (NPC.velocity.Y > 0f)
                                        NPC.velocity.Y += groundPhaseTurnSpeed;
                                    else
                                        NPC.velocity.Y -= groundPhaseTurnSpeed;
                                }
                            }
                            else
                            {
                                if (NPC.velocity.Y < targetY)
                                    NPC.velocity.Y += groundPhaseTurnSpeed * 1.1f;
                                else if (NPC.velocity.Y > targetY)
                                    NPC.velocity.Y -= groundPhaseTurnSpeed * 1.1f;

                                if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < segmentVelocity * maximumSpeed2)
                                {
                                    if (NPC.velocity.X > 0f)
                                        NPC.velocity.X += groundPhaseTurnSpeed;
                                    else
                                        NPC.velocity.X -= groundPhaseTurnSpeed;
                                }
                            }
                        }
                        #endregion
                        // Set velocity so that DoG cannot speed burst instantly at the start of a phase swap
                        if (calamityGlobalNPC.velocityPriorToPhaseSwap > 0f)
                        {
                            if (NPC.velocity.Length() > calamityGlobalNPC.velocityPriorToPhaseSwap)
                            {
                                NPC.velocity.Normalize();
                                NPC.velocity *= calamityGlobalNPC.velocityPriorToPhaseSwap;
                                calamityGlobalNPC.velocityPriorToPhaseSwap += CalamityGlobalNPC.velocityPriorToPhaseSwapIncrement;
                            }
                        }

                        NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;

                        if (flies)
                        {
                            if (NPC.localAI[0] != 1f)
                                NPC.ForceNetUpdate(false);

                            NPC.localAI[0] = 1f;
                        }
                        else
                        {
                            if (NPC.localAI[0] != 0f)
                                NPC.ForceNetUpdate(false);

                            NPC.localAI[0] = 0f;
                        }

                        if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
                            NPC.ForceNetUpdate(false);

                        if (calamityGlobalNPC.newAI[2] > phaseLimit)
                        {


                            calamityGlobalNPC.velocityPriorToPhaseSwap = NPC.velocity.Length();
                            NPC.ai[3] = 0f;
                            calamityGlobalNPC.newAI[2] = 0f;

                            if ((phase4))
                            {
                                calamityGlobalNPC.newAI[3] = AlphaGateValue - 120;
                                NPC.ai[3] = 2f;

                            }
                            NPC.TargetClosest();
                            NPC.ForceNetUpdate(false);
                        }
                    }
                    #endregion

                    #region Laser Wall AI
                    else if (NPC.ai[3] == 2)
                    {
                        calamityGlobalNPC.newAI[2]++;
                        if (laserWallPhase != (int)LaserWallPhase.End)
                        {
                            var dogRotation = player.DirectionTo(NPC.Center).ToRotation();
                            var DOGDIR = 1;

                            var goalpos = player.Center + new Vector2(1000, 0).RotatedBy(dogRotation + 0.05f * DOGDIR);

                            var currentVelLength = NPC.velocity.Length();
                            var goalVel = NPC.DirectionTo(goalpos) * currentVelLength;

                            TurnTowards(goalpos, maxSpeed: 6f * calamityGlobalNPC.newAI[2] / 120f);
                            NPC.velocity = VelocityRotation.ToRotationVector2() * (currentVelLength < 40 ? currentVelLength + 0.2f : currentVelLength > 42 ? currentVelLength - 0.2f : currentVelLength);

                            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;
                        }
                        else
                        {
                            NPC.dontTakeDamage = true;
                        }
                    }
                    #endregion
                    #region Portal AI
                    else
                    {
                        if (calamityGlobalNPC.newAI[2] == 0)
                        {
                            SpawnTeleportLocation(player);
                        }
                        NPC.velocity *= 1.02f;
                        calamityGlobalNPC.newAI[2]++;

                        NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;
                    }
                    #endregion
                }
            }
            #endregion
            #region Phase 1
            else
            {
                // Spawn fireballs
                if (phase3)
                {
                    // Fireballs
                    if (isInPassiveState)
                    {
                        ShootFireballs(player, distanceFromTarget, revenge);
                    }
                    else
                    {
                        calamityGlobalNPC.newAI[0] = 0f;
                    }

                    if (!spawnedGuardians)
                    {
                        if (revenge)
                            spawnDoGCountdown = 10;

                        if (!BossRushEvent.BossRushActive)
                        {
                            string key = "Mods.CalamityMod.Status.Boss.DoGSubphase1";
                            Color messageColor = Color.Cyan;
                            CalamityUtils.BroadcastLocalizedText(key, messageColor);

                            /*
                            int ctid = CombatText.NewText(NPC.Hitbox, messageColor, Language.GetTextValue(key), true);
                            if (ctid < Main.maxCombatText)
                                player.Calamity().subtitletext = Main.combatText[ctid];
                            player.Calamity().subtitleColors = new Color[] { Color.Cyan, Color.Fuchsia };
                            */
                            DialogueDisplaySystem.StartDialogue("Mods.CalamityMod.DevourerOfGods.Phases", NPC, 1, 120, false, new BossText());
                        }

                        NPC.TargetClosest();
                        spawnedGuardians = true;
                    }
                }
                else if (phase2)
                {
                    if (!spawnedGuardians2)
                    {
                        if (revenge)
                            spawnDoGCountdown = 10;

                        spawnedGuardians2 = true;
                    }

                }

                // Laser barrage attack variables
                float laserBarrageGateValue = 1440f;
                float laserBarrageShootGateValue = 240f;
                float laserBarragePhaseGateValue = laserBarrageGateValue - laserBarrageShootGateValue;

                // Spawn segments
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (!tail && NPC.ai[0] == 0f)
                    {
                        int Previous = NPC.whoAmI;
                        if (Main.zenithWorld)
                        {
                            maxLength = 2;
                            minLength = 1;
                        }
                        for (int segmentSpawn = 0; segmentSpawn < maxLength; segmentSpawn++)
                        {
                            int segment;
                            if (segmentSpawn >= 0 && segmentSpawn < minLength)
                            {
                                segment = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<DevourerofGodsBody>(), NPC.whoAmI);
                                Main.npc[segment].ModNPC<DevourerofGodsBody>().SegmentIndex = maxLength - segmentSpawn;
                            }
                            else
                                segment = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<DevourerofGodsTail>(), NPC.whoAmI);

                            Main.npc[segment].realLife = NPC.whoAmI;
                            Main.npc[segment].ai[2] = NPC.whoAmI;
                            Main.npc[segment].ai[1] = Previous;
                            Main.npc[Previous].ai[0] = segment;
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, segment, 0f, 0f, 0f, 0);
                            Previous = segment;
                        }
                        tail = true;
                    }
                }

                if (phase2)
                {
                    if (NPC.ai[3] == 2)
                        calamityGlobalNPC.newAI[1] += 1f;
                    if (calamityGlobalNPC.newAI[1] >= laserBarragePhaseGateValue)
                    {
                        if (!Main.dedServ)
                        {
                            if (!Main.LocalPlayer.dead && Main.LocalPlayer.active && Vector2.Distance(Main.LocalPlayer.Center, NPC.Center) < CalamityGlobalNPC.CatchUpDistance350Tiles)
                                Main.LocalPlayer.Calamity().infiniteFlight = true;
                        }
                        if (calamityGlobalNPC.newAI[1] >= laserBarrageGateValue)
                        {
                            calamityGlobalNPC.newAI[1] = 0f;
                            NPC.ai[3] = 3;
                            calamityGlobalNPC.newAI[2] = 0;
                            NPC.velocity += player.DirectionTo(NPC.Center) * 15;
                        }

                        if (calamityGlobalNPC.newAI[1] % (int)(laserBarrageShootGateValue * (death ? 0.33f : 0.5f)) == 0f && calamityGlobalNPC.newAI[1] > 0f)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int bType = Main.rand.Next(0, 5 + 1);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center + Main.rand.NextVector2CircularEdge(600, 600), Vector2.Zero, ModContent.ProjectileType<DoGLaserWalls>(), LaserWallDamage, 0, Main.myPlayer, 0.45f, 170, bType);

                                if (Main.zenithWorld)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center + Main.rand.NextVector2CircularEdge(600, 600), Vector2.Zero, ModContent.ProjectileType<DoGLaserWalls>(), LaserWallDamage, 0, Main.myPlayer, 0.45f, 120, 6);
                            }
                        }
                    }
                }

                // Opacity
                if (!(phase2 && calamityGlobalNPC.newAI[1] >= laserBarragePhaseGateValue) && !bigDaddyPhase2)
                {
                    // 2 seconds to become fully visible again
                    NPC.Opacity += 0.0083f;
                    if (NPC.Opacity > 1f)
                        NPC.Opacity = 1f;
                }

                // Movement
                int tilePositionX = (int)(NPC.position.X / 16f) - 1;
                int tileWidthPosX = (int)((NPC.position.X + NPC.width) / 16f) + 2;
                int tilePositionY = (int)(NPC.position.Y / 16f) - 1;
                int tileWidthPosY = (int)((NPC.position.Y + NPC.height) / 16f) + 2;

                if (tilePositionX < 0)
                    tilePositionX = 0;
                if (tileWidthPosX > Main.maxTilesX)
                    tileWidthPosX = Main.maxTilesX;
                if (tilePositionY < 0)
                    tilePositionY = 0;
                if (tileWidthPosY > Main.maxTilesY)
                    tileWidthPosY = Main.maxTilesY;

                if (NPC.velocity.X < 0f)
                    NPC.spriteDirection = -1;
                else if (NPC.velocity.X > 0f)
                    NPC.spriteDirection = 1;

                var VelocityRotation = NPC.velocity.ToRotation();
                void TurnTowards(Vector2 goal, float offset = 0, float maxSpeed = 1)
                {
                    float goal2 = (goal - NPC.Center).ToRotation() + offset;
                    maxSpeed *= (float)Math.PI / 180f;
                    var dif = MathF.Atan2(MathF.Sin(goal2 - VelocityRotation), MathF.Cos(goal2 - VelocityRotation));
                    if (dif < 0)
                    {
                        if (-dif > maxSpeed)
                            VelocityRotation -= maxSpeed;
                        else
                            VelocityRotation += dif;
                    }
                    else
                    {
                        if (dif > maxSpeed)
                            VelocityRotation += maxSpeed;
                        else
                            VelocityRotation += dif;
                    }
                }

                // Flight
                #region Phase 1 Air AI
                if (NPC.ai[3] == 1f)
                {
                    if (!Main.dedServ)
                    {
                        if (!Main.LocalPlayer.dead && Main.LocalPlayer.active && Vector2.Distance(Main.LocalPlayer.Center, NPC.Center) < CalamityGlobalNPC.CatchUpDistance350Tiles)
                            Main.LocalPlayer.AddBuff(ModContent.BuffType<DoGExtremeGravity>(), 2);
                    }

                    // Flying movement
                    NPC.localAI[1] = 0f;

                    calamityGlobalNPC.newAI[2] += 1f;

                    float speedCopy = speed;
                    float turnSpeedCopy = turnSpeed;
                    Vector2 npcCenter = NPC.Center;
                    float targetX = player.position.X + (player.width / 2);
                    float targetY = player.position.Y + (player.height / 2);

                    speedCopy = homingSpeed;
                    turnSpeedCopy = homingTurnSpeed;

                    if (expertMode)
                    {
                        speedCopy += distanceFromTarget * 0.005f * (1f - lifeRatio);
                        turnSpeedCopy += distanceFromTarget * 0.0001f * (1f - lifeRatio);
                    }
                    float fasterSpeedMult = speedCopy * 1.3f;
                    float slowerSpeedMult = speedCopy * 0.7f;
                    float npcSpeed = NPC.velocity.Length();
                    if (npcSpeed > 0f)
                    {
                        if (npcSpeed > fasterSpeedMult)
                        {
                            NPC.velocity.Normalize();
                            NPC.velocity *= fasterSpeedMult;
                        }
                        else if (npcSpeed < slowerSpeedMult)
                        {
                            NPC.velocity.Normalize();
                            NPC.velocity *= slowerSpeedMult;
                        }
                    }

                    targetX = (int)(targetX / 16f) * 16;
                    targetY = (int)(targetY / 16f) * 16;
                    npcCenter.X = (int)(npcCenter.X / 16f) * 16;
                    npcCenter.Y = (int)(npcCenter.Y / 16f) * 16;
                    targetX -= npcCenter.X;
                    targetY -= npcCenter.Y;
                    float targetDistance = (float)Math.Sqrt(targetX * targetX + targetY * targetY);
                    float absoluteTargetX = Math.Abs(targetX);
                    float absoluteTargetY = Math.Abs(targetY);
                    float timeToReachTarget = speedCopy / targetDistance;
                    targetX *= timeToReachTarget;
                    targetY *= timeToReachTarget;

                    turnSpeedCopy *= NPC.Distance(destination) / (death ? 800 : 1000);
                    if ((NPC.velocity.X > 0f && targetX > 0f) || (NPC.velocity.X < 0f && targetX < 0f) || (NPC.velocity.Y > 0f && targetY > 0f) || (NPC.velocity.Y < 0f && targetY < 0f))
                    {
                        if (NPC.velocity.X < targetX)
                            NPC.velocity.X += turnSpeedCopy;
                        else
                        {
                            if (NPC.velocity.X > targetX)
                                NPC.velocity.X -= turnSpeedCopy;
                        }

                        if (NPC.velocity.Y < targetY)
                            NPC.velocity.Y += turnSpeedCopy;
                        else
                        {
                            if (NPC.velocity.Y > targetY)
                                NPC.velocity.Y -= turnSpeedCopy;
                        }

                        if (Math.Abs(targetY) < speedCopy * 0.2 && ((NPC.velocity.X > 0f && targetX < 0f) || (NPC.velocity.X < 0f && targetX > 0f)))
                        {
                            if (NPC.velocity.Y > 0f)
                                NPC.velocity.Y += turnSpeedCopy * 2f;
                            else
                                NPC.velocity.Y -= turnSpeedCopy * 2f;
                        }

                        if (Math.Abs(targetX) < speedCopy * 0.2 && ((NPC.velocity.Y > 0f && targetY < 0f) || (NPC.velocity.Y < 0f && targetY > 0f)))
                        {
                            if (NPC.velocity.X > 0f)
                                NPC.velocity.X += turnSpeedCopy * 2f;
                            else
                                NPC.velocity.X -= turnSpeedCopy * 2f;
                        }
                    }
                    else
                    {
                        if (absoluteTargetX > absoluteTargetY)
                        {
                            if (NPC.velocity.X < targetX)
                                NPC.velocity.X += turnSpeedCopy * 1.1f;
                            else if (NPC.velocity.X > targetX)
                                NPC.velocity.X -= turnSpeedCopy * 1.1f;

                            if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < speedCopy * 0.5)
                            {
                                if (NPC.velocity.Y > 0f)
                                    NPC.velocity.Y += turnSpeedCopy;
                                else
                                    NPC.velocity.Y -= turnSpeedCopy;
                            }
                        }
                        else
                        {
                            if (NPC.velocity.Y < targetY)
                                NPC.velocity.Y += turnSpeedCopy * 1.1f;
                            else if (NPC.velocity.Y > targetY)
                                NPC.velocity.Y -= turnSpeedCopy * 1.1f;

                            if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < speedCopy * 0.5)
                            {
                                if (NPC.velocity.X > 0f)
                                    NPC.velocity.X += turnSpeedCopy;
                                else
                                    NPC.velocity.X -= turnSpeedCopy;
                            }
                        }
                    }

                    // Set velocity so that DoG cannot speed burst instantly at the start of a phase swap
                    if (calamityGlobalNPC.velocityPriorToPhaseSwap > 0f)
                    {
                        if (NPC.velocity.Length() > calamityGlobalNPC.velocityPriorToPhaseSwap)
                        {
                            NPC.velocity.Normalize();
                            NPC.velocity *= calamityGlobalNPC.velocityPriorToPhaseSwap;
                            calamityGlobalNPC.velocityPriorToPhaseSwap += CalamityGlobalNPC.velocityPriorToPhaseSwapIncrement;
                        }
                    }

                    NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;

                    if (calamityGlobalNPC.newAI[2] > phaseLimit)
                    {
                        calamityGlobalNPC.velocityPriorToPhaseSwap = NPC.velocity.Length();
                        NPC.ai[3] = 0f;
                        calamityGlobalNPC.newAI[2] = 0f;
                        if (phase2)
                        {
                            NPC.ai[3] = 2;
                            calamityGlobalNPC.newAI[1] = laserBarragePhaseGateValue - 120;
                        }
                        NPC.TargetClosest();
                        NPC.ForceNetUpdate(false);
                    }
                }
                #endregion
                // Ground
                #region Phase 1 Ground AI
                else if (NPC.ai[3] == 0)
                {
                    if (!Main.dedServ)
                    {
                        if (!Main.LocalPlayer.dead && Main.LocalPlayer.active && Vector2.Distance(Main.LocalPlayer.Center, NPC.Center) < CalamityGlobalNPC.CatchUpDistance350Tiles)
                            Main.LocalPlayer.AddBuff(ModContent.BuffType<Warped>(), 2);
                    }

                    calamityGlobalNPC.newAI[2] += 1f;

                    // Enrage
                    if (increaseSpeedMore)
                        groundPhaseTurnSpeed *= 4f;
                    else if (increaseSpeed)
                        groundPhaseTurnSpeed *= 2f;

                    if (!flies)
                    {
                        for (int r = tilePositionX; r < tileWidthPosX; r++)
                        {
                            for (int s = tilePositionY; s < tileWidthPosY; s++)
                            {
                                if (Main.tile[r, s] != null && ((Main.tile[r, s].HasUnactuatedTile && (Main.tileSolid[Main.tile[r, s].TileType] || (Main.tileSolidTop[Main.tile[r, s].TileType] && Main.tile[r, s].TileFrameY == 0))) || Main.tile[r, s].LiquidAmount > 64))
                                {
                                    Vector2 positionCheck;
                                    positionCheck.X = r * 16;
                                    positionCheck.Y = s * 16;
                                    if (NPC.position.X + NPC.width > positionCheck.X && NPC.position.X < positionCheck.X + 16f && NPC.position.Y + NPC.height > positionCheck.Y && NPC.position.Y < positionCheck.Y + 16f)
                                    {
                                        flies = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (!flies)
                    {
                        NPC.localAI[1] = 1f;

                        Rectangle rectangle12 = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);

                        int directChargeRange = death ? 1125 : 1200;

                        if (expertMode)
                            directChargeRange -= (int)(150f * (1f - lifeRatio));

                        if (directChargeRange < 1050)
                            directChargeRange = 1050;

                        bool canDirectlyCharge = true;
                        if (NPC.position.Y > player.position.Y)
                        {
                            for (int k = 0; k < Main.maxPlayers; k++)
                            {
                                if (Main.player[k].active)
                                {
                                    Rectangle rectangle13 = new Rectangle((int)Main.player[k].position.X - 1000, (int)Main.player[k].position.Y - 1000, 2000, directChargeRange);
                                    if (rectangle12.Intersects(rectangle13))
                                    {
                                        canDirectlyCharge = false;
                                        break;
                                    }
                                }
                            }
                            if (canDirectlyCharge)
                                flies = true;
                        }
                    }
                    else
                        NPC.localAI[1] = 0f;

                    float turnSpeedCopy = groundPhaseTurnSpeed;
                    Vector2 npcCenter = NPC.Center;
                    float targetX = destination.X;
                    float targetY = destination.Y;
                    targetX = (int)(targetX / 16f) * 16;
                    targetY = (int)(targetY / 16f) * 16;
                    npcCenter.X = (int)(npcCenter.X / 16f) * 16;
                    npcCenter.Y = (int)(npcCenter.Y / 16f) * 16;
                    targetX -= npcCenter.X;
                    targetY -= npcCenter.Y;

                    if (!flies)
                    {
                        NPC.velocity.Y += groundPhaseTurnSpeed;
                        if (NPC.velocity.Y > segmentVelocity)
                            NPC.velocity.Y = segmentVelocity;

                        // This bool exists to stop the strange wiggle behavior when worms are falling down
                        bool slowXVelocity = Math.Abs(NPC.velocity.X) > turnSpeedCopy;
                        if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < segmentVelocity * 2.2)
                        {
                            if (NPC.velocity.X < 0f)
                                NPC.velocity.X -= turnSpeedCopy * 1.1f;
                            else
                                NPC.velocity.X += turnSpeedCopy * 1.1f;
                        }
                        else if (NPC.velocity.Y == segmentVelocity)
                        {
                            if (slowXVelocity)
                            {
                                if (NPC.velocity.X < targetX)
                                    NPC.velocity.X += turnSpeedCopy;
                                else if (NPC.velocity.X > targetX)
                                    NPC.velocity.X -= turnSpeedCopy;
                            }
                            else
                                NPC.velocity.X = 0f;
                        }
                        else if (NPC.velocity.Y > 4f)
                        {
                            if (slowXVelocity)
                            {
                                if (NPC.velocity.X < 0f)
                                    NPC.velocity.X += turnSpeedCopy * 0.9f;
                                else
                                    NPC.velocity.X -= turnSpeedCopy * 0.9f;
                            }
                            else
                                NPC.velocity.X = 0f;
                        }
                    }
                    else
                    {
                        double maximumSpeed1 = death ? 0.46 : 0.4;
                        double maximumSpeed2 = death ? 1.125 : 1D;

                        if (expertMode)
                        {
                            maximumSpeed1 += 0.1f * (1f - lifeRatio);
                            maximumSpeed2 += 0.2f * (1f - lifeRatio);
                        }

                        float targetDistance = (float)Math.Sqrt(targetX * targetX + targetY * targetY);
                        float absoluteTargetX2 = Math.Abs(targetX);
                        float absoluteTargetY2 = Math.Abs(targetY);
                        float timeToReachTarget2 = segmentVelocity / targetDistance;
                        targetX *= timeToReachTarget2;
                        targetY *= timeToReachTarget2;

                        if (((NPC.velocity.X > 0f && targetX > 0f) || (NPC.velocity.X < 0f && targetX < 0f)) && ((NPC.velocity.Y > 0f && targetY > 0f) || (NPC.velocity.Y < 0f && targetY < 0f)))
                        {
                            if (NPC.velocity.X < targetX)
                                NPC.velocity.X += groundPhaseTurnSpeed * 1.5f;
                            else if (NPC.velocity.X > targetX)
                                NPC.velocity.X -= groundPhaseTurnSpeed * 1.5f;

                            if (NPC.velocity.Y < targetY)
                                NPC.velocity.Y += groundPhaseTurnSpeed * 1.5f;
                            else if (NPC.velocity.Y > targetY)
                                NPC.velocity.Y -= groundPhaseTurnSpeed * 1.5f;
                        }

                        if ((NPC.velocity.X > 0f && targetX > 0f) || (NPC.velocity.X < 0f && targetX < 0f) || (NPC.velocity.Y > 0f && targetY > 0f) || (NPC.velocity.Y < 0f && targetY < 0f))
                        {
                            if (NPC.velocity.X < targetX)
                                NPC.velocity.X += groundPhaseTurnSpeed;
                            else if (NPC.velocity.X > targetX)
                                NPC.velocity.X -= groundPhaseTurnSpeed;

                            if (NPC.velocity.Y < targetY)
                                NPC.velocity.Y += groundPhaseTurnSpeed;
                            else if (NPC.velocity.Y > targetY)
                                NPC.velocity.Y -= groundPhaseTurnSpeed;

                            if (Math.Abs(targetY) < segmentVelocity * maximumSpeed1 && ((NPC.velocity.X > 0f && targetX < 0f) || (NPC.velocity.X < 0f && targetX > 0f)))
                            {
                                if (NPC.velocity.Y > 0f)
                                    NPC.velocity.Y += groundPhaseTurnSpeed * 2f;
                                else
                                    NPC.velocity.Y -= groundPhaseTurnSpeed * 2f;
                            }

                            if (Math.Abs(targetX) < segmentVelocity * maximumSpeed1 && ((NPC.velocity.Y > 0f && targetY < 0f) || (NPC.velocity.Y < 0f && targetY > 0f)))
                            {
                                if (NPC.velocity.X > 0f)
                                    NPC.velocity.X += groundPhaseTurnSpeed * 2f;
                                else
                                    NPC.velocity.X -= groundPhaseTurnSpeed * 2f;
                            }
                        }
                        else if (absoluteTargetX2 > absoluteTargetY2)
                        {
                            if (NPC.velocity.X < targetX)
                                NPC.velocity.X += groundPhaseTurnSpeed * 1.1f;
                            else if (NPC.velocity.X > targetX)
                                NPC.velocity.X -= groundPhaseTurnSpeed * 1.1f;

                            if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < segmentVelocity * maximumSpeed2)
                            {
                                if (NPC.velocity.Y > 0f)
                                    NPC.velocity.Y += groundPhaseTurnSpeed;
                                else
                                    NPC.velocity.Y -= groundPhaseTurnSpeed;
                            }
                        }
                        else
                        {
                            if (NPC.velocity.Y < targetY)
                                NPC.velocity.Y += groundPhaseTurnSpeed * 1.1f;
                            else if (NPC.velocity.Y > targetY)
                                NPC.velocity.Y -= groundPhaseTurnSpeed * 1.1f;

                            if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < segmentVelocity * maximumSpeed2)
                            {
                                if (NPC.velocity.X > 0f)
                                    NPC.velocity.X += groundPhaseTurnSpeed;
                                else
                                    NPC.velocity.X -= groundPhaseTurnSpeed;
                            }
                        }
                    }

                    // Set velocity so that DoG cannot speed burst instantly at the start of a phase swap
                    if (calamityGlobalNPC.velocityPriorToPhaseSwap > 0f)
                    {
                        if (NPC.velocity.Length() > calamityGlobalNPC.velocityPriorToPhaseSwap)
                        {
                            NPC.velocity.Normalize();
                            NPC.velocity *= calamityGlobalNPC.velocityPriorToPhaseSwap;
                            calamityGlobalNPC.velocityPriorToPhaseSwap += CalamityGlobalNPC.velocityPriorToPhaseSwapIncrement;
                        }
                    }

                    NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;

                    if (flies)
                    {
                        if (NPC.localAI[0] != 1f)
                            NPC.ForceNetUpdate(false);

                        NPC.localAI[0] = 1f;
                    }
                    else
                    {
                        if (NPC.localAI[0] != 0f)
                            NPC.ForceNetUpdate(false);

                        NPC.localAI[0] = 0f;
                    }

                    if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
                        NPC.ForceNetUpdate(false);

                    if (calamityGlobalNPC.newAI[2] > phaseLimit)
                    {
                        calamityGlobalNPC.velocityPriorToPhaseSwap = NPC.velocity.Length();
                        NPC.ai[3] = 1f;
                        calamityGlobalNPC.newAI[2] = 0f;
                        NPC.TargetClosest();
                        NPC.netUpdate = true;
                    }
                }
                #endregion

                #region Laser Wall AI
                else if (NPC.ai[3] == 2)
                {
                    calamityGlobalNPC.newAI[2]++;
                    if (laserWallPhase != (int)LaserWallPhase.End)
                    {
                        var dogRotation = player.DirectionTo(NPC.Center).ToRotation();
                        var DOGDIR = 1;

                        var goalpos = player.Center + new Vector2(1000, 0).RotatedBy(dogRotation + 0.05f * DOGDIR);

                        var currentVelLength = NPC.velocity.Length();
                        var goalVel = NPC.DirectionTo(goalpos) * currentVelLength;

                        TurnTowards(goalpos, maxSpeed: 6f * calamityGlobalNPC.newAI[2] / 120f);
                        NPC.velocity = VelocityRotation.ToRotationVector2() * (currentVelLength < 40 ? currentVelLength + 0.2f : currentVelLength);

                        NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;
                    }
                }
                #endregion

                #region Decoil AI
                else
                {
                    NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;
                    calamityGlobalNPC.newAI[2]++;
                    if (calamityGlobalNPC.newAI[2] >= 45)
                    {
                        NPC.ai[3] = 0;
                        calamityGlobalNPC.newAI[2] = 0;
                    }
                }
                #endregion
            }
            #endregion

            // There is no escape...
            if (NPC.Distance(destination) > 2400f)
                NPC.velocity += (destination - NPC.Center).SafeNormalize(Vector2.UnitY) * turnSpeed;

            // Jaw movement.
            float targetRotationReset = ((Phase2Started || phase3) && isInPassiveState) ? MathHelper.ToRadians(10f) : 0f;
            float targetRotationOpenJaw = MathHelper.ToRadians(Phase2Started ? 32f : 22f);
            float targetRotationChompDown = MathHelper.ToRadians(Phase2Started ? -25f : -15f);
            float dotProduct = Vector2.Dot(NPC.DirectionTo(destination), NPC.velocity.SafeNormalize(Vector2.Zero));
            bool rotatedTowardsPlayer = dotProduct > 0.8f;

            // Reset slowly at all times.
            JawRotation = MathHelper.Lerp(JawRotation, targetRotationReset, 0.08f);

            // Chomp down when close enough to the player.
            if (distanceFromTarget < 110f)
            {
                JawRotation = MathHelper.Lerp(JawRotation, targetRotationChompDown, JawChompDownProgress);
                JawChompDownProgress = MathHelper.Clamp(JawChompDownProgress + 0.28f, 0f, 1f);

                // Spawn some particle effects when chomping down
                if (JawChompDownProgress >= 0.94f && ShouldSpawnChompVFX)
                {
                    Vector2 jawChompPosition = (NPC.Center - Vector2.UnitY.RotatedBy(NPC.rotation) * 36f) + (NPC.velocity * 1.45f);
                    Color jawParticleColor = isInPassiveState ? Color.Cyan : Color.Purple;
                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 sparkVelocity = NPC.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.ToRadians(20f)) * Main.rand.NextFloat(26f, 32f);
                        int sparkLifetime = Main.rand.Next(20, 30);
                        float sparkScale = Main.rand.NextFloat(1.4f, 1.8f);
                        Color sparkColor = Color.Lerp(jawParticleColor, Color.White, Main.rand.NextFloat(0f, 0.3f));

                        SparkParticle chompSpark = new(jawChompPosition, sparkVelocity, false, sparkLifetime, sparkScale, sparkColor);
                        GeneralParticleHandler.SpawnParticle(chompSpark);
                    }

                    // Play a sound and add some screenshake with it if you're close enough too.
                    float shakeStrength = Phase2Started ? 14f : 8f;
                    CalamityUtils.AddScreenshakeAt(jawChompPosition, shakeStrength, 100f);
                    SoundEngine.PlaySound(HitSound with { Pitch = -0.45f }, jawChompPosition);
                    ShouldSpawnChompVFX = false;
                    NPC.netUpdate = true;
                }
            }
            // Only open up when DoG is facing the player.
            else if (distanceFromTarget < 480f && rotatedTowardsPlayer)
            {
                JawRotation = MathHelper.Lerp(JawRotation, targetRotationOpenJaw, 0.28f);
                if (!ShouldSpawnChompVFX)
                    ShouldSpawnChompVFX = true;
            }

            // Fade out the special jaw texture during rift dashes.
            GodSlayerDashJawTimer--;
            if (GodSlayerDashJawTimer <= 0f)
            {
                GodSlayerDashJawFadeProgress = MathHelper.Lerp(GodSlayerDashJawFadeProgress, 0f, 0.15f);
                if (GodSlayerDashJawTimer < 0f)
                    GodSlayerDashJawTimer = 0f;
            }
            if (NPC.life > Main.npc[(int)NPC.ai[0]].life)
                NPC.life = Main.npc[(int)NPC.ai[0]].life;
        }

        private void ShootFireballs(Player player, float distanceFromTarget, bool revenge)
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();
            Vector2 mouthPosition = NPC.Center - Vector2.UnitY.RotatedBy(NPC.rotation) * (Phase2Started ? 28f : -16f);

            calamityGlobalNPC.newAI[0] += 1f;
            if (NPC.Opacity >= 1f && (distanceFromTarget > (revenge ? 320f : 480f)))
            {
                float dotProduct = Vector2.Dot(NPC.DirectionTo(AdjustedPlayerCenter()), NPC.velocity.SafeNormalize(Vector2.Zero));
                if (dotProduct > 0.8f)
                {
                    if (calamityGlobalNPC.newAI[0] > 75f)
                    {
                        // Flame and cinder particles from the mouth.
                        for (int i = 0; i < 18; i++)
                        {
                            Vector2 flameVelocity = NPC.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(32f, 44f);
                            int flameLifetime = Main.rand.Next(45, 60);
                            float flameScale = Main.rand.NextFloat(0.6f, 0.9f);
                            float flameOpacity = Main.rand.NextFloat(0.7f, 0.9f);
                            Color flameColor = Color.Lerp(Color.Cyan, Color.White, Main.rand.NextFloat(0f, 0.4f));

                            HeavySmokeParticle fireballFlames = new(mouthPosition, flameVelocity, flameColor, flameLifetime, flameScale, flameOpacity, 0.01f, true);
                            GeneralParticleHandler.SpawnParticle(fireballFlames);
                        }

                        for (int i = 0; i < 10; i++)
                        {
                            Vector2 cinderVelocity = NPC.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(36f, 48f);
                            float cinderScale = Main.rand.NextFloat(0.3f, 0.5f);
                            Color cinderColor = Color.Lerp(Color.Cyan, Color.White, Main.rand.NextFloat(0.2f, 0.6f));
                            int cinderLifetime = Main.rand.Next(45, 60);

                            SquishyLightParticle cinder = new(mouthPosition, cinderVelocity, cinderScale, cinderColor, cinderLifetime);
                            GeneralParticleHandler.SpawnParticle(cinder);
                        }

                        // Open DoG's mouth a bit everytime a fireball is fired.
                        JawRotation = MathHelper.ToRadians(22f);

                        float fireballSpeed = 8f;
                        Vector2 fireballVelocity = Vector2.Normalize(AdjustedPlayerCenter() - NPC.Center) * (fireballSpeed + NPC.velocity.Length() * 0.5f);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), mouthPosition, fireballVelocity, ModContent.ProjectileType<DoGFire>(), FireballDamage, 0f, Main.myPlayer, 2f);

                        calamityGlobalNPC.newAI[0] = 0f;
                        NPC.ForceNetUpdate(false);
                    }
                }
            }

        }

        private void SpawnTeleportLocation(Player player, bool phase2Transition = false)
        {
            if (teleportTimer > 0 || player.dead || !player.active)
                return;

            if (NPC.ai[3] < 2)
                NPC.ai[3] = 3;

            int baseTeleportTime = ((CalamityWorld.death || BossRushEvent.BossRushActive) ? TimeBeforeTeleport_Death : CalamityWorld.revenge ? TimeBeforeTeleport_Revengeance : Main.expertMode ? TimeBeforeTeleport_Expert : TimeBeforeTeleport_Normal);
            if ((CalamityWorld.death || BossRushEvent.BossRushActive) && NPC.life / (float)NPC.lifeMax < 0.25f && NPC.ai[3] < 8 && NPC.ai[3] > 3)
                baseTeleportTime -= 45;

            if (!phase2Transition)
                teleportTimer = baseTeleportTime;

            SoundEngine.PlaySound(RiftOpenSound with { Volume = 1.5f }, player.Center);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int randomRange = Main.zenithWorld ? 960 : 48;
                float distance = 500f;
                if ((CalamityWorld.death || BossRushEvent.BossRushActive) && NPC.life / (float)NPC.lifeMax < 0.25f && NPC.ai[3] < 8)
                {
                    distance += 450; //More breathing room for rapid dash
                }
                Vector2 targetVector = player.Center + player.velocity.SafeNormalize(Vector2.UnitX) * distance + new Vector2(Main.rand.Next(-randomRange, randomRange + 1), Main.rand.Next(-randomRange, randomRange + 1));
                int rift = Projectile.NewProjectile(NPC.GetSource_FromAI(), targetVector, Vector2.Zero, ModContent.ProjectileType<DoGTeleportRift>(), 0, 0f, Main.myPlayer, NPC.whoAmI);
                if (Main.projectile.IndexInRange(rift))
                    Main.projectile[rift].ModProjectile<DoGTeleportRift>().RiftLifetime = baseTeleportTime;

                // GFB, spawn a ton of fake rifts alongside the real one.
                if (Main.zenithWorld)
                {
                    randomRange = 2000;
                    for (int k = 0; k < 35; k++)
                    {
                        targetVector = player.Center + player.velocity.SafeNormalize(Vector2.UnitX) * distance + new Vector2(Main.rand.Next(-randomRange, randomRange + 1), Main.rand.Next(-randomRange, randomRange + 1));
                        int faker = Projectile.NewProjectile(NPC.GetSource_FromAI(), targetVector, Vector2.Zero, ModContent.ProjectileType<DoGTeleportRift>(), 0, 0f, Main.myPlayer, NPC.whoAmI);
                        if (Main.projectile.IndexInRange(faker))
                        {
                            Main.projectile[faker].ModProjectile<DoGTeleportRift>().FakeRift = true;
                            Main.projectile[faker].ModProjectile<DoGTeleportRift>().RiftLifetime = baseTeleportTime;
                        }
                    }
                }
            }
        }

        private void Teleport(Player player, bool death, bool revenge, bool expertMode, bool phase5)
        {
            Vector2 newPosition = GetRiftLocation(true);

            if ((!AwaitingPhase2Teleport && (player.dead || !player.active)) || newPosition == default)
                return;
            bool phase6 = NPC.life / (float)NPC.lifeMax < 0.25f;

            if (Main.netMode != NetmodeID.MultiplayerClient && !(death && phase6 && NPC.ai[3] < 7)) // 4 dashes on Death phase 3 without fireballs
            {
                float finalVelocity = death ? 12f : 10f;
                int totalSpreads = revenge ? 6 : 3;
                float mult = revenge ? 1.5f : 3f;
                for (int i = 0; i < totalSpreads; i++)
                {
                    if (!death && i % 3 == 2)
                        continue;
                    int totalProjectiles = 12;
                    float radians = MathHelper.TwoPi / totalProjectiles;
                    float newVelocity = finalVelocity - i * mult;
                    float velocityMult = 1f + ((finalVelocity - newVelocity) / (newVelocity * 2f) / 100f);
                    double angleA = radians * 0.5;
                    double angleB = MathHelper.ToRadians(90f) - angleA;
                    float velocityX = (float)(newVelocity * Math.Sin(angleA) / Math.Sin(angleB));
                    Vector2 spinningPoint = i < 3 ? new Vector2(0f, -newVelocity) : new Vector2(-velocityX, -newVelocity);
                    float finalVelocityReduction = (float)Math.Pow(1.25, i) - 1f;
                    for (int k = 0; k < totalProjectiles; k++)
                    {
                        Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), newPosition, vector255, ModContent.ProjectileType<DoGFire>(), FireballDamage, 0f, Main.myPlayer, velocityMult, finalVelocity - finalVelocityReduction);
                    }
                }
            }

            NPC.TargetClosest();
            NPC.position = newPosition;
            float chargeVelocity = death ? 26f : revenge ? 24f : expertMode ? 22f : 20f;
            chargeVelocity *= (death) ? (!(phase6 && NPC.ai[3] < 7)) ? 2.5f : 3 : 2.25f;
            float maxChargeDistance = 1800f;
            postTeleportTimer = (int)Math.Round(maxChargeDistance / chargeVelocity);
            int phase6dashcount = death ? 5 : 3;
            if (phase6 && NPC.ai[3] < 2 + phase6dashcount)
            {
                if (NPC.ai[3] < 3)
                    NPC.ai[3] = 3;
                NPC.ai[3]++;
                // On FTW the final 10% is infinite portal dashing
                if (Main.getGoodWorld && NPC.life / (float)NPC.lifeMax < 0.1f)
                {
                    NPC.ai[3] = 4;
                    NPC.SimpleStrikeNPC(10000, 1);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int bType = Main.rand.Next(0, 2);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center + Main.rand.NextVector2CircularEdge(600, 600), Vector2.Zero, ModContent.ProjectileType<DoGLaserWalls>(), LaserWallDamage, 0, Main.myPlayer, 0.45f, 300, bType);
                    }
                }
            }
            else
            {
                NPC.ai[3] = 0;
            }
            NPC.Calamity().newAI[2] = 0;
            AwaitingPhase2Teleport = false;
            NPC.Opacity = 1f - (postTeleportTimer / 255f);
            NPC.velocity = Vector2.Normalize(AdjustedPlayerCenter(10) - NPC.Center) * chargeVelocity;
            NPC.damage = NPC.defDamage;
            NPC.ForceNetUpdate(false);
            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.type == ModContent.NPCType<DevourerofGodsBody>() || n.type == ModContent.NPCType<DevourerofGodsTail>())
                {
                    n.position = newPosition;

                    if (n.type == ModContent.NPCType<DevourerofGodsTail>())
                        ((DevourerofGodsTail)n.ModNPC).setInvulTime(720);

                    n.ForceNetUpdate(false);
                }
            }

            GodSlayerDashJawFadeProgress = 1f;
            GodSlayerDashJawTimer = 60f;
            SoundEngine.PlaySound(AttackSound with { Pitch = AttackSound.Pitch + extrapitch }, player.Center);
            if (Main.getGoodWorld)
                NPC.Calamity().velocityPriorToPhaseSwap = 20;
        }

        public void DoDeathAnimation()
        {
            // Play a sound at the start.
            if (DeathAnimationTimer == 1f)
            {
                SoundEngine.PlaySound(DeathExplosionSound, NPC.Center);
            }

            // Close the health bar, fade in, and stop doing contact damage.
            NPC.Calamity().CanHaveBossHealthBar = false;
            NPC.Opacity = MathHelper.Clamp(NPC.Opacity + 0.1f, 0f, 1f);
            NPC.dontTakeDamage = true;
            NPC.damage = 0;

            void destroySegment(int index, ref int destroyedSegments)
            {
                if (Main.rand.NextBool(5))
                    SoundEngine.PlaySound(DeathSegmentSound, NPC.Center);

                List<int> segments = new List<int>()
                {
                    ModContent.NPCType<DevourerofGodsBody>(),
                    ModContent.NPCType<DevourerofGodsTail>()
                };
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (segments.Contains(n.type) && n.active &&
                        (n.type == segments[1] || n.ModNPC<DevourerofGodsBody>().SegmentIndex == index))
                    {
                        for (int j = 0; j < 20; j++)
                        {
                            Dust cosmicBurst = Dust.NewDustPerfect(n.Center + Main.rand.NextVector2Circular(25f, 25f), DustID.BoneTorch);
                            cosmicBurst.scale = 1.7f;
                            cosmicBurst.velocity = Main.rand.NextVector2Circular(9f, 9f);
                            cosmicBurst.noGravity = true;
                        }

                        n.life = 0;
                        n.HitEffect();
                        n.active = false;
                        n.ForceNetUpdate(false);

                        destroyedSegments++;
                        break;
                    }
                }
            }

            // Slow down but maintain a specific direction.
            float idealSpeed = MathHelper.Lerp(8.4f, 4f, Utils.GetLerpValue(15f, 210f, DeathAnimationTimer, true));
            if (NPC.velocity.Length() != idealSpeed)
                NPC.velocity = NPC.velocity.SafeNormalize(Vector2.UnitY) * MathHelper.Lerp(NPC.velocity.Length(), idealSpeed, 0.08f);

            // Stay within the world.
            if (NPC.Center.X < 300f || NPC.Center.X > Main.maxTilesX * 16f - 300f)
                NPC.velocity.X *= -1f;
            if (NPC.Center.Y < 300f || NPC.Center.Y > Main.maxTilesY * 16f - 300f)
                NPC.velocity.Y *= -1f;

            if (DeathAnimationTimer >= 120f && DeathAnimationTimer < 370f && DeathAnimationTimer % 3f == 0f)
            {
                int segmentToDestroy = (int)(Utils.GetLerpValue(120f, 370f, DeathAnimationTimer, true) * 60f);
                destroySegment(segmentToDestroy, ref DestroyedSegmentCount);
            }

            if (DeathAnimationTimer == 452f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<DoGDeathBoom>(), 0, 0f);

                if (!Main.dedServ)
                {
                    SoundEngine.PlaySound(DeathAnimationSound, NPC.Center);

                    for (int i = 0; i < 3; i++)
                    {
                        SoundEngine.PlaySound(DeathExplosionSound, NPC.Center);
                    }
                }
            }

            if (DeathAnimationTimer >= 410f && DeathAnimationTimer < 470f && DeathAnimationTimer % 2f == 0f)
            {
                int segmentToDestroy = (int)(Utils.GetLerpValue(410f, 470f, DeathAnimationTimer, true) * 10f) + 60;
                destroySegment(segmentToDestroy, ref DeathAnimationTimer);
            }

            float light = Utils.GetLerpValue(430f, 465f, DeathAnimationTimer, true);
            MoonlordDeathDrama.RequestLight(light, Main.LocalPlayer.Center);

            if (DeathAnimationTimer >= 485f)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.NPCLoot();
                NPC.active = false;
                NPC.ForceNetUpdate(false);
            }
            DeathAnimationTimer++;
        }

        public Vector2 GetRiftLocation(bool activateRift = false)
        {
            Vector2 realSpot = Vector2.Zero;
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.type == ModContent.ProjectileType<DoGTeleportRift>())
                {
                    if (proj.ModProjectile is DoGTeleportRift rift && !rift.FakeRift)
                    {
                        realSpot = proj.Center;

                        // Safeguard for if the rift doesn't activate at its designated time.
                        if (activateRift)
                            proj.ModProjectile<DoGTeleportRift>().SwitchAIStates();
                    }
                }
            }
            return realSpot;
        }

        Vector2 noiseOffset = Vector2.Zero;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            bool actuallyInPhaseTwo = (Phase2Started && NPC.localAI[2] <= 60f);
            Texture2D mainTexture = actuallyInPhaseTwo ? TextureP2.Value : TextureAssets.Npc[Type].Value;
            Texture2D mainGlowTexture = actuallyInPhaseTwo ? TextureP2_Glow_Purple.Value : Texture_Glow_Purple.Value;
            Texture2D mainJawTexture = actuallyInPhaseTwo ? JawTextureP2.Value : JawTexture.Value;
            Texture2D mainJawGlowTexture = actuallyInPhaseTwo ? JawTextureP2_Glow.Value : JawTexture_Glow.Value;

            // Draw the custom worm bestiary sprite separately.
            if (NPC.IsABestiaryIconDummy)
            {
                NPC.Opacity = 1f;
                return CalamityUtils.DrawAnimatedBestiaryWorm(spriteBatch, NPC, drawColor, TextureP2_Full.Value, DevourerofGodsBody.TextureP2.Value, 4, 26, 0.5f, new Vector2(30, 10), 2, 20);
            }

            bool shouldUseShader = CalamityDrawParameterNPC.DoGDeathAnimationTimer != 0;
            SpriteBatchSnapshot snap = new(spriteBatch);

            if (shouldUseShader)
            {
                if (noiseOffset == Vector2.zeroVector)
                    noiseOffset = NPC.Center;

                Main.spriteBatch.End(out snap);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                MiscShaderData dissolveShader = GameShaders.Misc["CalamityMod:Dissolve"];
                Texture2D dissolveTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/HarshNoise").Value;

                dissolveShader.Shader.Parameters["noiseScale"].SetValue(0.5f);
                dissolveShader.Shader.Parameters["dissolveIntensity"].SetValue(CalamityDrawParameterNPC.DoGDeathAnimationTimer / 600f);
                dissolveShader.Shader.Parameters["sampleOffset"].SetValue(noiseOffset * 0.5f);
                dissolveShader.Shader.Parameters["transitionColor"].SetValue(SpecialMoveColor.ToVector4());
                dissolveShader.Shader.Parameters["transitionOffset"].SetValue(0.05f);

                Main.instance.GraphicsDevice.Textures[1] = dissolveTexture;
                Main.instance.GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;

                dissolveShader.Apply();
            }

            SpriteEffects spriteEffects = (NPC.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 halfSizeTexture = mainTexture.Size() * 0.5f;

            Vector2 drawPosition = NPC.Center - screenPos;
            drawPosition -= new Vector2(mainTexture.Width, mainTexture.Height) * NPC.scale / 2f;
            drawPosition += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);

            // Draw DoG's mouth and its glow masks.
            Vector2 jawOrigin = mainJawTexture.Size() * 0.5f;
            for (int i = -1; i <= 1; i += 2)
            {
                float jawBaseOffset = actuallyInPhaseTwo ? 42f : 28f;
                SpriteEffects jawSpriteEffect = (i == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                Vector2 jawDrawPosition = drawPosition;
                jawDrawPosition += Vector2.UnitX.RotatedBy(NPC.rotation + JawRotation * i) * i * (jawBaseOffset + MathF.Sin(JawRotation) * 24f);
                jawDrawPosition -= Vector2.UnitY.RotatedBy(NPC.rotation) * ((actuallyInPhaseTwo ? 44f : 8f) + MathF.Sin(JawRotation) * (actuallyInPhaseTwo ? 30f : -6f));

                spriteBatch.Draw(mainJawTexture, jawDrawPosition, null, NPC.GetAlpha(drawColor), NPC.rotation + JawRotation * i, jawOrigin, NPC.scale, jawSpriteEffect, 0f);
                spriteBatch.Draw(mainJawGlowTexture, jawDrawPosition, null, NPC.GetAlpha(Color.White), NPC.rotation + JawRotation * i, jawOrigin, NPC.scale, jawSpriteEffect, 0f);

                // Draw the additional special jaw textures above these for the Rift Dash attack.
                if (GodSlayerDashJawFadeProgress > 0.02f && CalamityDrawParameterNPC.DoGDeathAnimationTimer <= 0)
                {
                    using (spriteBatch.Scope())
                    {
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                        Vector2 godSlayerJawOrigin = GodSlayerDashJawTexture.Size() * 0.5f;
                        float godSlayerJawOpacity = GodSlayerDashJawFadeProgress;

                        spriteBatch.Draw(GodSlayerDashJawTexture.Value, jawDrawPosition, null, NPC.GetAlpha(Color.Fuchsia) * godSlayerJawOpacity, NPC.rotation + JawRotation * i, godSlayerJawOrigin, NPC.scale * 1.6f, jawSpriteEffect, 0f);
                        spriteBatch.Draw(GodSlayerDashJawTexture.Value, jawDrawPosition, null, NPC.GetAlpha(Color.Cyan) * godSlayerJawOpacity, NPC.rotation + JawRotation * i, godSlayerJawOrigin, NPC.scale * 1.3f, jawSpriteEffect, 0f);
                        spriteBatch.End();
                    }
                }
            }

            // Draw the main head.
            spriteBatch.Draw(mainTexture, drawPosition, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            // Draw the head glow masks. There are two glow mask textures for each head which are split between purple and cyan highlights.
            // Each color is exaggerated by a similar color slightly when drawn to make them pop out more.
            Color glowmaskColor = Color.Lerp(Color.White, Color.Fuchsia, 0.5f);
            spriteBatch.Draw(mainGlowTexture, drawPosition, NPC.frame, NPC.GetAlpha(glowmaskColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            mainGlowTexture = actuallyInPhaseTwo ? TextureP2_Glow_Cyan.Value : Texture_Glow_Cyan.Value;
            glowmaskColor = Color.Lerp(Color.White, Color.Cyan, 0.5f);
            spriteBatch.Draw(mainGlowTexture, drawPosition, NPC.frame, NPC.GetAlpha(glowmaskColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            if (shouldUseShader)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(snap);
            }

            return false;
        }

        public override void BossLoot(ref int potionType)
        {
            potionType = ModContent.ItemType<CosmiliteBrick>();
        }

        public override void OnKill()
        {
            // Don't bother running any of this in Boss Rush.
            if (BossRushEvent.BossRushActive)
                return;

            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            CalamityGlobalTownNPC.SetNewShopVariable(new int[] { ModContent.NPCType<Bandit>() }, DownedBossSystem.downedDoG);

            // If DoG has not been killed yet, notify players that the holiday moons are buffed
            if (!DownedBossSystem.downedDoG)
            {
                string key = "Mods.CalamityMod.Status.Progression.DoGBossText";
                Color messageColor = Color.Cyan;
                string key2 = "Mods.CalamityMod.Status.Progression.DoGBossText2";
                Color messageColor2 = Color.Orange;
                string key3 = "Mods.CalamityMod.Status.Progression.DargonBossText";
                Color messageColor3 = Color.Yellow;

                CalamityUtils.BroadcastLocalizedText(key, messageColor);
                CalamityUtils.BroadcastLocalizedText(key2, messageColor2);
                CalamityUtils.BroadcastLocalizedText(key3, messageColor3);
            }

            // Mark DoG as dead
            DownedBossSystem.downedDoG = true;
            CalamityNetcode.SyncWorld();
        }

        public override bool SpecialOnKill()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(NPC,
                ModContent.NPCType<DevourerofGodsHead>(),
                ModContent.NPCType<DevourerofGodsBody>(),
                ModContent.NPCType<DevourerofGodsTail>());
            NPC.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Boss bag
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<DevourerofGodsBag>()));

            // Extraneous potions
            npcLoot.DefineConditionalDropSet(() => true).Add(DropHelper.PerPlayer(ModContent.ItemType<OmegaHealingPotion>(), 1, 5, 15), hideLootReport: true); // Healing Potions don't show up in the Bestiary

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<MawOfInfinity>(),
                    ModContent.ItemType<TheObliterator>(),
                    ModContent.ItemType<ThreadOfEradication>(),
                    ModContent.ItemType<HyperdeathRiftScepter>(),
                    ModContent.ItemType<VoidEaterMarionette>(),
                    ModContent.ItemType<DimensionTearingDisk>()
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));
                normalOnly.Add(ModContent.ItemType<CosmicDischarge>(), 10);

                // Vanity
                normalOnly.Add(ModContent.ItemType<DevourerofGodsMask>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                // Materials
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<CosmiliteBar>(), 1, 65, 80));
                normalOnly.Add(ModContent.ItemType<CosmiliteBrick>(), 1, 150, 250);

                // Equipment
                // 16NOV2025: Ozzatron: item has been chosen as the "Expert gatekept" item for this Calamity boss
                // normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<NebulousCore>()));
            }

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<DevourerOfGodsRelic>());

            // GFB torch and Wand drops
            var GFBOnly = npcLoot.DefineConditionalDropSet(DropHelper.GFB);
            {
                GFBOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<TheWand>()), hideLootReport: true);

                // this will be disastrous for the torch economy
                int dropRate = 10;
                int dropMin = 1;
                int dropMax = 9999;
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.Torch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.PurpleTorch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.YellowTorch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.GreenTorch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.RedTorch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.WhiteTorch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.OrangeTorch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.PinkTorch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.RainbowTorch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.IceTorch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.BoneTorch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.UltrabrightTorch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.DemonTorch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.CursedTorch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.IchorTorch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.DesertTorch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.CoralTorch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.CorruptTorch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.CrimsonTorch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.HallowedTorch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.JungleTorch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.MushroomTorch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.ShimmerTorch, dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<CausticTorch>(), dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<KelpTorch>(), dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<ThermalTorch>(), dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<VoidTorch>(), dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<AlgalPrismTorch>(), dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<AstralTorch>(), dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<GloomTorch>(), dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<NavyPrismTorch>(), dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<RefractivePrismTorch>(), dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<SulphurousTorch>(), dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<ThermalTorch>(), dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<CausticTorch>(), dropRate, dropMin, dropMax), true);
                GFBOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<KelpTorch>(), dropRate, dropMin, dropMax), true);
            }

            // Trophy (always directly from boss, never in bag)
            npcLoot.Add(ModContent.ItemType<DevourerofGodsTrophy>(), 10);

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedDoG, ModContent.ItemType<LoreDevourerofGods>(), desc: DropHelper.FirstKillText);
        }

        // Can only hit the target if within certain distance
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if (Phase2Started && NPC.localAI[2] > 60f)
                return false;
            cooldownSlot = ImmunityCooldownID.Bosses;

            Rectangle targetHitbox = target.Hitbox;

            float hitboxTopLeft = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float hitboxTopRight = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float hitboxBotLeft = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float hitboxBotRight = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = hitboxTopLeft;
            if (hitboxTopRight < minDist)
                minDist = hitboxTopRight;
            if (hitboxBotLeft < minDist)
                minDist = hitboxBotLeft;
            if (hitboxBotRight < minDist)
                minDist = hitboxBotRight;

            return minDist <= (Phase2Started ? 80f : 55f) * NPC.scale && (NPC.Opacity >= 1f || postTeleportTimer > 0);
        }

        // This will always put the boss to 1 health before dying, which makes external checks work.
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers) => modifiers.SetMaxDamage(NPC.life - 1);

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // viable???, done here since it's conditional
            if (Main.zenithWorld && projectile.type == ModContent.ProjectileType<LaceratorYoyo>())
            {
                modifiers.SourceDamage *= 10f;
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 2f;
            return null;
        }

        public override bool CheckActive()
        {
            return false;
        }

        // This can be ran multiple times per death, goofy mode
        public override bool CheckDead()
        {
            NPC.life = 1;
            Dying = true;
            NPC.dontTakeDamage = true;
            NPC.active = true;
            NPC.ForceNetUpdate(false);

            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 8;
                SoundEngine.PlaySound(HitSound with { Pitch = HitSound.Pitch + extrapitch }, NPC.Center);
            }
            if (NPC.life <= 0)
            {
                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("DoGS").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("DoGS2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("DoGS5").Type, NPC.scale);
                }
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = (int)(100 * NPC.scale);
                NPC.height = (int)(100 * NPC.scale);
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int i = 0; i < 15; i++)
                {
                    int cosmiliteDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[cosmiliteDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[cosmiliteDust].scale = 0.5f;
                        Main.dust[cosmiliteDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 30; j++)
                {
                    int cosmiliteDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                    Main.dust[cosmiliteDust2].noGravity = true;
                    Main.dust[cosmiliteDust2].velocity *= 5f;
                    cosmiliteDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[cosmiliteDust2].velocity *= 2f;
                }
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage <= 0)
                return;

            if (!DialogueDisplaySystem.ContainsDialogueKey("Mods.CalamityMod.DevourerOfGods.Phases") && target.Calamity().dogTextCooldown <= 0 && !BossRushEvent.BossRushActive)
            {
                var counter = target.Calamity().DoGHeadHitCounter;
                string DialogueGroup;
                int DialogueIndex;
                if (target.statLife - hurtInfo.Damage <= 0)
                {
                    DialogueGroup = "Mods.CalamityMod.DevourerOfGods.Death";
                    if (counter == 0)
                        if (NPC.GetLifePercent() <= 0.25f)
                            DialogueIndex = 4; //All that running just to die to a single touch?
                        else
                            DialogueIndex = 0;// text = "Mods.CalamityMod.Status.Boss.DoGHeadDeath1"; //Tasteless slop.
                    else if (NPC.GetLifePercent() <= 0.25f || counter >= 10)
                        DialogueIndex = 3; //WEAK.
                    else if (Phase2Started)
                        DialogueIndex = 2; //And STAY dead!
                    else if (!spawnedGuardians)
                        DialogueIndex = 0; //Tasteless slop.
                    else
                        DialogueIndex = 1; //A feast worthy of a god!
                }
                else
                {
                    if (counter == 0 && Phase2Started)
                    {
                        DialogueGroup = "Mods.CalamityMod.DevourerOfGods.Running";
                        if (NPC.GetLifePercent() <= 0.25f)
                            DialogueIndex = 1; //WHAT'S THE PROBLEM?! CAN'T RUN ANYMORE?!
                        else
                            DialogueIndex = 0; //You can't run forever!
                    }
                    else
                    {
                        DialogueGroup = "Mods.CalamityMod.DevourerOfGods.Head";
                        if (counter > 9)
                            DialogueIndex = Main.rand.Next(10, 13); //DogHead11-13
                        else if (counter == 9)
                            DialogueIndex = 10;//text = headHitKeys[9]; //DogHead10
                        else if (counter > 4)
                            DialogueIndex = Main.rand.Next(4, 9); //DogHead5-9
                        else
                            DialogueIndex = Main.rand.Next(0, 4); //DogHead1-4
                    }
                }

                DialogueDisplaySystem.StartDialogueOnClient(DialogueGroup, NPC, DialogueIndex, 60, false, new BossText());
                target.Calamity().dogTextCooldown = 90;

            }
            target.Calamity().DoGHeadHitCounter++;
        }
    }
}
