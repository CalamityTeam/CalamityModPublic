using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Placeables.FurnitureAbyss;
using CalamityMod.Items.Placeables.FurnitureCosmilite;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Melee.Yoyos;
using CalamityMod.Sounds;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DevourerofGods
{
    [LongDistanceNetSync]
    public class DevourerofGodsHead : ModNPC
    {
        public static int phase1IconIndex;
        public static int phase2IconIndex;

        public static Asset<Texture2D> Texture_Glow;
        public static Asset<Texture2D> Texture_Glow2;
        public static Asset<Texture2D> Phase2Texture;
        public static Asset<Texture2D> Phase2Texture_Glow;
        public static Asset<Texture2D> Phase2Texture_Glow2;

        internal static void LoadHeadIcons()
        {
            string phase1IconPath = "CalamityMod/NPCs/DevourerofGods/DevourerofGodsHead_Head_Boss";
            string phase2IconPath = "CalamityMod/NPCs/DevourerofGods/DevourerofGodsHeadS_Head_Boss";

            CalamityMod.Instance.AddBossHeadTexture(phase1IconPath, -1);
            phase1IconIndex = ModContent.GetModBossHeadSlot(phase1IconPath);

            CalamityMod.Instance.AddBossHeadTexture(phase2IconPath, -1);
            phase2IconIndex = ModContent.GetModBossHeadSlot(phase2IconPath);
        }

        // Laser velocity
        private const float laserVelocity = 14f;

        // Phase 1 variables

        // Enums
        private enum LaserWallType
        {
            DiagonalRight = 0,
            DiagonalLeft = 1,
            DiagonalHorizontal = 2,
            DiagonalCross = 3
        }

        // Laser spread variables
        private const int shotSpacingMax = 1470;
        private int shotSpacing = shotSpacingMax;
        private const int totalShots = 14;
        private const int spacingVar = shotSpacingMax / totalShots * 2;
        private int laserWallType = 0;
        private const float laserWallSpacingOffset = 16f;

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
        private enum LaserWallType_Phase2
        {
            Normal = 0,
            Offset = 1,
            DiagonalHorizontal = 2,
            MultiLayered = 3,
            DiagonalVertical = 4
        }

        // Laser wall variables
        private const int shotSpacingMax_Phase2 = 1470;
        private int[] shotSpacing_Phase2 = new int[4] { shotSpacingMax_Phase2, shotSpacingMax_Phase2, shotSpacingMax_Phase2, shotSpacingMax_Phase2 };
        private const int spacingVar_Phase2 = 105;
        private const int totalShots_Phase2 = 28;
        private const int totalDiagonalShots = 8;
        private const int diagonalSpacingVar = shotSpacingMax_Phase2 / totalDiagonalShots * 2;
        private int laserWallType_Phase2 = 0;
        public int laserWallPhase = 0;

        // Phase variables
        private const int idleCounterMax = 300;
        private int idleCounter = idleCounterMax;
        private int postTeleportTimer = 0;
        private int teleportTimer = -1;
        private const int TimeBeforeTeleport_Death = 120;
        private const int TimeBeforeTeleport_Revengeance = 140;
        private const int TimeBeforeTeleport_Expert = 160;
        private const int TimeBeforeTeleport_Normal = 180;
        private bool spawnedGuardians3 = false;
        private const float alphaGateValue = 669f;
        public const float SkyColorTransitionTime = 90f;

        // Death animation variables
        public bool Dying;
        public int DeathAnimationTimer;
        public int DestroyedSegmentCount;

        // Sounds
        public static readonly SoundStyle SpawnSound = new("CalamityMod/Sounds/Custom/DevourerSpawn");
        public static readonly SoundStyle AttackSound = new("CalamityMod/Sounds/Custom/DevourerAttack");
        public static readonly SoundStyle DeathAnimationSound = new("CalamityMod/Sounds/NPCKilled/DevourerDeath");
        public static readonly SoundStyle DeathExplosionSound = new("CalamityMod/Sounds/NPCKilled/DevourerDeathImpact");
        public static readonly SoundStyle DeathSegmentSound = new("CalamityMod/Sounds/NPCKilled/DevourerSegmentBreak", 4);
        public float extrapitch = 0;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.6f,
                PortraitScale = 0.6f,
                CustomTexturePath = "CalamityMod/ExtraTextures/Bestiary/DevourerofGods_Bestiary",
                PortraitPositionXOverride = 60,
                PortraitPositionYOverride = 40
            };
            value.Position.X += 82f;
            value.Position.Y += 38f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            if (!Main.dedServ)
            {
                Texture_Glow = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
                Texture_Glow2 = ModContent.Request<Texture2D>(Texture + "Glow2", AssetRequestMode.AsyncLoad);
                Phase2Texture = ModContent.Request<Texture2D>(Texture + "S", AssetRequestMode.AsyncLoad);
                Phase2Texture_Glow = ModContent.Request<Texture2D>(Texture + "SGlow", AssetRequestMode.AsyncLoad);
                Phase2Texture_Glow2 = ModContent.Request<Texture2D>(Texture + "SGlow2", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.DevourerofGods")
            });
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.npcSlots = 5f;
            NPC.width = 104;
            NPC.height = 104;
            NPC.defense = 50;
            NPC.LifeMaxNERB(887500, 1065000, 1500000); // Phase 1 is 355000, Phase 2 is 532500
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.takenDamageMultiplier = 1.1f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.value = Item.buyPrice(6, 0, 0, 0);
            NPC.Opacity = 0f;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.netAlways = true;

            if (Main.getGoodWorld)
                NPC.scale *= 1.5f;
        }

        public override void BossHeadSlot(ref int index)
        {
            if (Phase2Started && (NPC.localAI[2] > 60f || AwaitingPhase2Teleport))
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
            writer.Write(shotSpacing);
            writer.Write(laserWallType);
            writer.Write(PortalIndex);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);

            // Phase 2 syncs
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            writer.Write(shotSpacing_Phase2[0]);
            writer.Write(shotSpacing_Phase2[1]);
            writer.Write(shotSpacing_Phase2[2]);
            writer.Write(shotSpacing_Phase2[3]);
            writer.Write(idleCounter);
            writer.Write(laserWallPhase);
            writer.Write(laserWallType_Phase2);
            writer.Write(postTeleportTimer);
            writer.Write(teleportTimer);
            writer.Write(NPC.Opacity);

            // Death animation syncs
            writer.Write(Dying);
            writer.Write(DeathAnimationTimer);
            writer.Write(DestroyedSegmentCount);

            // Frame syncs
            writer.Write(NPC.frame.X);
            writer.Write(NPC.frame.Y);
            writer.Write(NPC.frame.Width);
            writer.Write(NPC.frame.Height);

            // Misc syncs
            writer.Write(extrapitch);

            // Be sure to inform clients of the fact that The Devourer of Gods is dying if only the server recieved this packet.
            if (Main.netMode == NetmodeID.Server && !wasDyingBefore && Dying)
            {
                NPC.netSpam = 0;
                NPC.netUpdate = true;
            }
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
            shotSpacing = reader.ReadInt32();
            laserWallType = reader.ReadInt32();
            PortalIndex = reader.ReadInt32();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();

            // Phase 2 syncs
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            shotSpacing_Phase2[0] = reader.ReadInt32();
            shotSpacing_Phase2[1] = reader.ReadInt32();
            shotSpacing_Phase2[2] = reader.ReadInt32();
            shotSpacing_Phase2[3] = reader.ReadInt32();
            idleCounter = reader.ReadInt32();
            laserWallPhase = reader.ReadInt32();
            laserWallType_Phase2 = reader.ReadInt32();
            postTeleportTimer = reader.ReadInt32();
            teleportTimer = reader.ReadInt32();
            NPC.Opacity = reader.ReadSingle();

            // Death animation syncs
            Dying = reader.ReadBoolean();
            DeathAnimationTimer = reader.ReadInt32();
            DestroyedSegmentCount = reader.ReadInt32();

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
            if (CalamityConfig.Instance.BossesStopWeather)
                CalamityMod.StopRain();

            // Get a target (time is checked in the second check to ensure a new target isn't being set constantly)
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();
            else if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles && Main.time % 60D == 0D)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            // Variables
            bool flyUpDuringLaserWalls = laserWallPhase == (int)LaserWallPhase.FireLaserWalls || (laserWallPhase == (int)LaserWallPhase.End && teleportTimer > 0);
            bool flies = NPC.ai[3] == 0f || flyUpDuringLaserWalls;
            Vector2 destination = flyUpDuringLaserWalls ? (player.Center - Vector2.UnitY * 480f) : player.Center;
            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phase 1 phases
            bool phase2 = lifeRatio < 0.9f;
            bool phase3 = lifeRatio < 0.75f;
            bool bigDaddyPhase2 = lifeRatio < 0.6f;

            // Phase 2 phases
            bool phase4 = lifeRatio < 0.5f;
            bool phase5 = lifeRatio < 0.4f;
            bool phase6 = lifeRatio < 0.2f;
            bool phase7 = lifeRatio < 0.15f;

            // Black sky timer
            if (!death)
            {
                if (phase7)
                {
                    if (NPC.localAI[3] < SkyColorTransitionTime)
                        NPC.localAI[3] += 1f;
                }
                else if (bigDaddyPhase2)
                {
                    if (NPC.localAI[3] > 0f)
                        NPC.localAI[3] -= 1f;
                }
            }

            // Sound pitch
            extrapitch = Main.zenithWorld ? 0.3f : 0f;

            // Velocity variables
            float segmentVelocity = bossRush ? 19f : death ? 17.5f : 16f;
            if (expertMode)
                segmentVelocity += 4f * (1f - lifeRatio);

            float speed = bossRush ? 18f : death ? 16.5f : 15f;
            float turnSpeed = bossRush ? 0.36f : death ? 0.33f : 0.3f;
            float homingSpeed = bossRush ? 36f : death ? 30f : 24f;
            float homingTurnSpeed = bossRush ? 0.48f : death ? 0.405f : 0.33f;

            if (expertMode)
            {
                speed += 3f * (1f - lifeRatio);
                turnSpeed += 0.06f * (1f - lifeRatio);
                homingSpeed += 12f * (1f - lifeRatio);
                homingTurnSpeed += 0.15f * (1f - lifeRatio);
            }

            float groundPhaseTurnSpeed = bossRush ? 0.3f : death ? 0.24f : 0.18f;

            if (expertMode)
                groundPhaseTurnSpeed += 0.1f * (1f - lifeRatio);

            groundPhaseTurnSpeed += Vector2.Distance(destination, NPC.Center) * 0.0002f;

            if (Main.getGoodWorld)
            {
                segmentVelocity *= 1.1f;
                speed *= 1.1f;
                turnSpeed *= 1.1f;
                homingSpeed *= 1.1f;
                homingTurnSpeed *= 1.1f;
                groundPhaseTurnSpeed *= 1.1f;
            }

            // How long it takes before swapping phases
            int phaseLimit = death ? 600 : 900;
            if (expertMode && NPC.ai[3] == 0f)
            {
                phaseLimit /= 1 + (int)((death ? 4f : 5f) * (1f - lifeRatio));
                if (phaseLimit < 180)
                    phaseLimit = 180;
            }

            // Continuously reset certain things.
            AttemptingToEnterPortal = false;

            // Worm variable
            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            // Despawn
            if (player.dead)
            {
                NPC.ai[3] = 0f;
                calamityGlobalNPC.newAI[2] = 0f;

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
                        Main.npc[a].netUpdate = true;

                        // Prevent netUpdate from being blocked by the spam counter.
                        if (Main.npc[a].netSpam >= 10)
                            Main.npc[a].netSpam = 9;
                    }
                }
            }

            float distanceFromTarget = Vector2.Distance(destination, NPC.Center);
            bool increaseSpeed = distanceFromTarget > CalamityGlobalNPC.CatchUpDistance200Tiles;
            bool increaseSpeedMore = distanceFromTarget > CalamityGlobalNPC.CatchUpDistance350Tiles;

            float takeLessDamageDistance = 1600f;
            if (distanceFromTarget > takeLessDamageDistance)
            {
                float damageTakenScalar = MathHelper.Clamp(1f - ((distanceFromTarget - takeLessDamageDistance) / takeLessDamageDistance), 0f, 1f);
                NPC.takenDamageMultiplier = MathHelper.Lerp(1f, 1.1f, damageTakenScalar);
            }
            else
                NPC.takenDamageMultiplier = 1.1f;

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
                Teleport(player, bossRush, death, revenge, expertMode, phase5);

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
                        NPC.netSpam = 0;
                        NPC.netUpdate = true;
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

                    NPC.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    if (NPC.netSpam >= 10)
                        NPC.netSpam = 9;
                }

                // Dialogue the moment the second phase starts
                if (NPC.localAI[2] == 60f && !bossRush)
                {
                    string key = "Mods.CalamityMod.Status.Boss.EdgyBossText5";
                    Color messageColor = Color.Cyan;
                    CalamityUtils.DisplayLocalizedText(key, messageColor);
                }
            }

            // Begin phase 2
            if (Phase2Started)
            {
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

                            NPC.netUpdate = true;

                            // Prevent netUpdate from being blocked by the spam counter.
                            if (NPC.netSpam >= 10)
                                NPC.netSpam = 9;
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

                        NPC.netUpdate = true;

                        // Prevent netUpdate from being blocked by the spam counter.
                        if (NPC.netSpam >= 10)
                            NPC.netSpam = 9;
                    }

                    AttemptingToEnterPortal = true;
                }

                // Phase 2
                else
                {
                    // Immunity after teleport and when dying
                    NPC.dontTakeDamage = postTeleportTimer > 0 || Dying;

                    // Teleport countdown
                    if (teleportTimer > 0)
                    {
                        teleportTimer--;

                        // Teleport
                        if (teleportTimer == 0)
                            Teleport(player, bossRush, death, revenge, expertMode, phase5);
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

                        NPC.netUpdate = true;

                        // Prevent netUpdate from being blocked by the spam counter.
                        if (NPC.netSpam >= 10)
                            NPC.netSpam = 9;

                        return;
                    }

                    // Laser walls
                    if (phase4 && !spawnedGuardians3 && postTeleportTimer <= 0)
                    {
                        if (laserWallPhase == (int)LaserWallPhase.SetUp)
                        {
                            // Enter laser wall phase very quickly when final phase starts
                            if (phase6 && calamityGlobalNPC.newAI[3] < alphaGateValue)
                                calamityGlobalNPC.newAI[3] = alphaGateValue;

                            // Increment next laser wall phase timer
                            calamityGlobalNPC.newAI[3] += 1f;

                            // Set alpha value prior to firing laser walls
                            if (calamityGlobalNPC.newAI[3] > alphaGateValue)
                            {
                                // Disable teleports
                                if (teleportTimer > 0)
                                {
                                    GetRiftLocation(false);
                                    teleportTimer = 0;
                                }

                                NPC.Opacity = 1f - (MathHelper.Clamp((calamityGlobalNPC.newAI[3] - alphaGateValue) * 5f, 0f, 255f) / 255f);
                            }

                            // Fire laser walls every 12 seconds after a laser wall phase ends
                            if (calamityGlobalNPC.newAI[3] >= 720f)
                            {
                                NPC.Opacity = 0f;

                                // Reset laser wall timer to 0
                                calamityGlobalNPC.newAI[1] = 0f;

                                calamityGlobalNPC.newAI[3] = 0f;
                                laserWallPhase = (int)LaserWallPhase.FireLaserWalls;
                            }
                        }
                        else if (laserWallPhase == (int)LaserWallPhase.FireLaserWalls)
                        {
                            // Remain in laser wall firing phase for 4 seconds
                            idleCounter--;
                            if (idleCounter <= 0)
                            {
                                SpawnTeleportLocation(player);
                                laserWallPhase = (int)LaserWallPhase.End;
                                idleCounter = idleCounterMax;
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
                                NPC.Opacity = 1f;
                                laserWallPhase = (int)LaserWallPhase.SetUp;

                                // Enter final phase
                                if (!spawnedGuardians3 && phase6)
                                {
                                    // Reset laser wall timers to 0
                                    calamityGlobalNPC.newAI[1] = 0f;
                                    calamityGlobalNPC.newAI[3] = 0f;

                                    // Anger message
                                    if (!bossRush)
                                    {
                                        string key = "Mods.CalamityMod.Status.Boss.EdgyBossText6";
                                        Color messageColor = Color.Cyan;
                                        CalamityUtils.DisplayLocalizedText(key, messageColor);
                                    }

                                    // Summon Cosmic Guardians
                                    SoundEngine.PlaySound(AttackSound with { Pitch = AttackSound.Pitch + extrapitch }, player.Center);
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        for (int i = 0; i < 3; i++)
                                            NPC.SpawnOnPlayer(NPC.FindClosestPlayer(), ModContent.NPCType<CosmicGuardianHead>());
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
                            postTeleportTimer--;
                            NPC.Opacity = 1f - (postTeleportTimer / 255f);
                        }
                        else
                        {
                            NPC.Opacity += 0.024f;
                            if (NPC.Opacity > 1f)
                                NPC.Opacity = 1f;
                        }
                    }

                    // Fireballs
                    // Check angle and distance to make sure it's realistic that they'd be fired
                    if (NPC.Opacity >= 1f && (distanceFromTarget > 480f || (CalamityWorld.LegendaryMode && CalamityWorld.revenge)) && (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY).ToRotation().AngleTowards(NPC.velocity.ToRotation(), MathHelper.PiOver4) == NPC.velocity.ToRotation())
                    {
                        calamityGlobalNPC.newAI[0] += 1f;
                        if (calamityGlobalNPC.newAI[0] >= ((CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 30f : 150f) && calamityGlobalNPC.newAI[0] % ((CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 30f : phase7 ? 30f : 60f) == 0f)
                        {
                            float fireballSpeed = 8f;
                            Vector2 fireballVelocity = Vector2.Normalize(player.Center - NPC.Center) * fireballSpeed + NPC.velocity * 0.5f;

                            Vector2 dustVelocity = fireballVelocity * 2f;
                            for (int k = 0; k < 50; k++)
                                Dust.NewDust(NPC.Center, 52, 52, (int)CalamityDusts.PurpleCosmilite, dustVelocity.X, dustVelocity.Y);

                            int type = ModContent.ProjectileType<DoGFire>();
                            int damage = NPC.GetProjectileDamage(type);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, fireballVelocity, type, damage, 0f, Main.myPlayer);

                            if (CalamityWorld.LegendaryMode && revenge)
                            {
                                for (int l = 0; l < 8; l++)
                                {
                                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Ichor, 0f, 0f, 100, default, 1f);
                                    float dustVelocityYAdd = Math.Abs(Main.dust[dust].velocity.Y) * 0.5f;
                                    if (Main.dust[dust].velocity.Y < 0f)
                                        Main.dust[dust].velocity.Y = 2f + dustVelocityYAdd;
                                    if (Main.rand.NextBool())
                                    {
                                        Main.dust[dust].scale = 0.25f;
                                        Main.dust[dust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                                    }
                                }

                                int numBlobs = 4;
                                type = ModContent.ProjectileType<IchorBlob>();
                                damage = 60;

                                for (int i = 0; i < numBlobs; i++)
                                {
                                    Vector2 blobVelocity = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                                    blobVelocity.Normalize();
                                    blobVelocity *= Main.rand.Next(400, 801) * (bossRush ? 0.02f : 0.01f);
                                    blobVelocity *= Main.rand.NextFloat() + 1f;

                                    float blobVelocityYAdd = Math.Abs(blobVelocity.Y) * 0.5f;
                                    if (blobVelocity.Y < 2f)
                                        blobVelocity.Y = 2f + blobVelocityYAdd;

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.UnitY * 50f, blobVelocity, type, damage, 0f, Main.myPlayer, 0f, player.Center.Y);
                                }
                            }
                        }
                    }
                    else if (distanceFromTarget < 240f)
                        calamityGlobalNPC.newAI[0] = 0f;

                    // Laser walls
                    if (!spawnedGuardians3 && laserWallPhase == (int)LaserWallPhase.FireLaserWalls)
                    {
                        float spawnOffset = 1200f;
                        float divisor = bossRush ? 100f : 150f;

                        if (calamityGlobalNPC.newAI[1] % divisor == 0f)
                        {
                            SoundEngine.PlaySound(SoundID.Item12, player.Center);

                            // Side walls
                            float targetPosY = player.position.Y;
                            int type = ModContent.ProjectileType<DoGDeath>();
                            int damage = NPC.GetProjectileDamage(type);
                            int halfTotalDiagonalShots = totalDiagonalShots / 2;
                            Vector2 start = default;
                            Vector2 velocity = default;
                            Vector2 aim = expertMode ? player.Center + player.velocity * 20f : Vector2.Zero;

                            switch (laserWallType_Phase2)
                            {
                                case (int)LaserWallType_Phase2.Normal:

                                    for (int x = 0; x < totalShots_Phase2; x++)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + spawnOffset, targetPosY + shotSpacing_Phase2[0], -laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - spawnOffset, targetPosY + shotSpacing_Phase2[0], laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                        }

                                        shotSpacing_Phase2[0] -= spacingVar_Phase2;
                                    }

                                    if (expertMode && Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + spawnOffset, player.Center.Y, -laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - spawnOffset, player.Center.Y, laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                    }

                                    break;

                                case (int)LaserWallType_Phase2.Offset:

                                    targetPosY += 50f;
                                    for (int x = 0; x < totalShots_Phase2; x++)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + spawnOffset, targetPosY + shotSpacing_Phase2[0], -laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - spawnOffset, targetPosY + shotSpacing_Phase2[0], laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                        }

                                        shotSpacing_Phase2[0] -= spacingVar_Phase2;
                                    }

                                    if (expertMode && Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + spawnOffset, player.Center.Y, -laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - spawnOffset, player.Center.Y, laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                    }

                                    break;

                                case (int)LaserWallType_Phase2.DiagonalHorizontal:

                                    for (int x = 0; x < totalDiagonalShots + 1; x++)
                                    {
                                        start = new Vector2(player.position.X + spawnOffset, targetPosY + shotSpacing_Phase2[0]);
                                        aim.Y += laserWallSpacingOffset * (x - halfTotalDiagonalShots);
                                        velocity = Vector2.Normalize(aim - start) * laserVelocity;

                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);

                                        start = new Vector2(player.position.X - spawnOffset, targetPosY + shotSpacing_Phase2[0]);
                                        velocity = Vector2.Normalize(aim - start) * laserVelocity;

                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);

                                        shotSpacing_Phase2[0] -= diagonalSpacingVar;
                                    }

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X, targetPosY + spawnOffset, 0f, -laserVelocity, type, damage, 0f, Main.myPlayer);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X, targetPosY - spawnOffset, 0f, laserVelocity, type, damage, 0f, Main.myPlayer);
                                    }

                                    break;

                                case (int)LaserWallType_Phase2.MultiLayered:

                                    for (int x = 0; x < totalShots_Phase2; x++)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + spawnOffset, targetPosY + shotSpacing_Phase2[0], -laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - spawnOffset, targetPosY + shotSpacing_Phase2[0], laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                        }

                                        shotSpacing_Phase2[0] -= spacingVar_Phase2;
                                    }

                                    int totalBonusLasers = totalShots_Phase2 / 2;
                                    for (int x = 0; x < totalBonusLasers; x++)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + spawnOffset, targetPosY + shotSpacing_Phase2[3], -laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - spawnOffset, targetPosY + shotSpacing_Phase2[3], laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                        }

                                        shotSpacing_Phase2[3] -= Main.rand.NextBool() ? 180 : 200;
                                    }

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + spawnOffset, player.Center.Y, -laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - spawnOffset, player.Center.Y, laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                    }

                                    break;

                                case (int)LaserWallType_Phase2.DiagonalVertical:

                                    for (int x = 0; x < totalDiagonalShots + 1; x++)
                                    {
                                        start = new Vector2(player.position.X + shotSpacing_Phase2[0], targetPosY + spawnOffset);
                                        aim.X += laserWallSpacingOffset * (x - halfTotalDiagonalShots);
                                        velocity = Vector2.Normalize(aim - start) * laserVelocity;

                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);

                                        start = new Vector2(player.position.X + shotSpacing_Phase2[0], targetPosY - spawnOffset);
                                        velocity = Vector2.Normalize(aim - start) * laserVelocity;

                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);

                                        shotSpacing_Phase2[0] -= diagonalSpacingVar;
                                    }

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + spawnOffset, player.Center.Y, -laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - spawnOffset, player.Center.Y, laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                    }

                                    break;
                            }

                            // Pick a random laser wall phase in expert+
                            if (expertMode)
                            {
                                int laserWallPhase;
                                int choices = revenge ? 5 : 3;
                                do laserWallPhase = Main.rand.Next(choices);
                                while (laserWallPhase == laserWallType_Phase2);
                                laserWallType_Phase2 = laserWallPhase;
                            }
                            else
                                laserWallType_Phase2 = laserWallType_Phase2 == (int)LaserWallType_Phase2.Normal ? (int)LaserWallType_Phase2.Offset : (int)LaserWallType_Phase2.Normal;

                            // Lower wall
                            for (int x = 0; x < totalShots_Phase2; x++)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + shotSpacing_Phase2[1], player.position.Y + spawnOffset, 0f, -laserVelocity, type, damage, 0f, Main.myPlayer);

                                shotSpacing_Phase2[1] -= spacingVar_Phase2;
                            }

                            // Upper wall
                            for (int x = 0; x < totalShots_Phase2; x++)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + shotSpacing_Phase2[2], player.position.Y - spawnOffset, 0f, laserVelocity, type, damage, 0f, Main.myPlayer);

                                shotSpacing_Phase2[2] -= spacingVar_Phase2;
                            }

                            for (int i = 0; i < shotSpacing_Phase2.Length; i++)
                                shotSpacing_Phase2[i] = shotSpacingMax_Phase2;
                        }

                        calamityGlobalNPC.newAI[1] += 1f;
                    }

                    // Set flight time to max during laser walls
                    if (!spawnedGuardians3 && laserWallPhase == (int)LaserWallPhase.FireLaserWalls)
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < CalamityGlobalNPC.CatchUpDistance350Tiles)
                            {
                                Main.player[Main.myPlayer].Calamity().infiniteFlight = true;
                            }
                        }
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

                    // Flight
                    if (NPC.ai[3] == 0f)
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < CalamityGlobalNPC.CatchUpDistance350Tiles)
                                Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<Warped>(), 2);
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
                            if (laserWallPhase == (int)LaserWallPhase.SetUp && calamityGlobalNPC.newAI[3] <= alphaGateValue)
                                SpawnTeleportLocation(player);
                            else
                                calamityGlobalNPC.newAI[2] += 10f;
                        }
                        else
                            calamityGlobalNPC.newAI[2] += 2f;

                        float speedCopy = speed;
                        float turnSpeedCopy = turnSpeed;
                        Vector2 npcCenter = NPC.Center;
                        float targetX = destination.X;
                        float targetY = destination.Y;
                        int flyYLevel = -1;
                        int destinationTileX = (int)(destination.X / 16f);
                        int destinationTileY = (int)(destination.Y / 16f);

                        // Charge at target for 1.5 seconds
                        bool flyAtTarget = (!phase4 || spawnedGuardians3) && calamityGlobalNPC.newAI[2] > phaseLimit - 90 && revenge;

                        for (int i = destinationTileX - 2; i <= destinationTileX + 2; i++)
                        {
                            for (int j = destinationTileY; j <= destinationTileY + 15; j++)
                            {
                                if (WorldGen.SolidTile2(i, j))
                                {
                                    flyYLevel = j;
                                    break;
                                }
                            }
                            if (flyYLevel > 0)
                                break;
                        }

                        if (!flyAtTarget && destination == player.Center)
                        {
                            if (flyYLevel > 0)
                            {
                                flyYLevel *= 16;
                                float chaseFlyLevel = flyYLevel - 800;
                                if (player.position.Y > chaseFlyLevel)
                                {
                                    targetY = chaseFlyLevel;
                                    if (Math.Abs(NPC.Center.X - destination.X) < 500f)
                                    {
                                        if (NPC.velocity.X > 0f)
                                            targetX = destination.X + 600f;
                                        else
                                            targetX = destination.X - 600f;
                                    }
                                }
                            }
                        }
                        else
                        {
                            speedCopy = homingSpeed;
                            turnSpeedCopy = homingTurnSpeed;
                        }

                        speedCopy += Vector2.Distance(destination, NPC.Center) * 0.005f;
                        turnSpeedCopy += Vector2.Distance(destination, NPC.Center) * 0.00025f;

                        if (CalamityWorld.LegendaryMode && revenge)
                        {
                            if ((player.Center - NPC.Center).SafeNormalize(Vector2.UnitY).ToRotation().AngleTowards(NPC.velocity.ToRotation(), MathHelper.PiOver4) == NPC.velocity.ToRotation())
                                speedCopy *= 2f;
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

                            NPC.netUpdate = true;

                            // Prevent netUpdate from being blocked by the spam counter.
                            if (NPC.netSpam >= 10)
                                NPC.netSpam = 9;
                        }
                    }

                    // Ground
                    else
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < CalamityGlobalNPC.CatchUpDistance350Tiles)
                                Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<DoGExtremeGravity>(), 2);
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
                            if (laserWallPhase == (int)LaserWallPhase.SetUp && calamityGlobalNPC.newAI[3] <= alphaGateValue)
                                SpawnTeleportLocation(player);
                            else
                                groundPhaseTurnSpeed *= 4f;
                        }
                        else if (increaseSpeed)
                            groundPhaseTurnSpeed *= 2f;

                        if (CalamityWorld.LegendaryMode && revenge)
                        {
                            if ((player.Center - NPC.Center).SafeNormalize(Vector2.UnitY).ToRotation().AngleTowards(NPC.velocity.ToRotation(), MathHelper.PiOver4) == NPC.velocity.ToRotation())
                                segmentVelocity *= 2f;
                        }

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
                            double maximumSpeed1 = bossRush ? 0.52 : death ? 0.46 : 0.4;
                            double maximumSpeed2 = bossRush ? 1.25 : death ? 1.125 : 1D;

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
                            {
                                NPC.netUpdate = true;

                                // Prevent netUpdate from being blocked by the spam counter.
                                if (NPC.netSpam >= 10)
                                    NPC.netSpam = 9;
                            }

                            NPC.localAI[0] = 1f;
                        }
                        else
                        {
                            if (NPC.localAI[0] != 0f)
                            {
                                NPC.netUpdate = true;

                                // Prevent netUpdate from being blocked by the spam counter.
                                if (NPC.netSpam >= 10)
                                    NPC.netSpam = 9;
                            }

                            NPC.localAI[0] = 0f;
                        }

                        if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
                        {
                            NPC.netUpdate = true;

                            // Prevent netUpdate from being blocked by the spam counter.
                            if (NPC.netSpam >= 10)
                                NPC.netSpam = 9;
                        }

                        if (calamityGlobalNPC.newAI[2] > phaseLimit)
                        {
                            calamityGlobalNPC.velocityPriorToPhaseSwap = NPC.velocity.Length();
                            NPC.ai[3] = 0f;
                            calamityGlobalNPC.newAI[2] = 0f;
                            NPC.TargetClosest();

                            NPC.netUpdate = true;

                            // Prevent netUpdate from being blocked by the spam counter.
                            if (NPC.netSpam >= 10)
                                NPC.netSpam = 9;
                        }
                    }
                }
            }
            else
            {
                // Spawn Guardians
                if (phase3)
                {
                    if (!death)
                    {
                        if (NPC.localAI[3] < SkyColorTransitionTime)
                            NPC.localAI[3] += 1f;
                    }

                    if (!spawnedGuardians)
                    {
                        if (revenge)
                            spawnDoGCountdown = 10;

                        if (!bossRush)
                        {
                            string key = "Mods.CalamityMod.Status.Boss.EdgyBossText";
                            Color messageColor = Color.Cyan;
                            CalamityUtils.DisplayLocalizedText(key, messageColor);
                        }

                        NPC.TargetClosest();
                        spawnedGuardians = true;
                    }

                    if (spawnDoGCountdown > 0)
                    {
                        spawnDoGCountdown--;
                        if (spawnDoGCountdown == 0)
                        {
                            SoundEngine.PlaySound(AttackSound with { Pitch = AttackSound.Pitch + extrapitch }, player.Center);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < 2; i++)
                                    NPC.SpawnOnPlayer(NPC.FindClosestPlayer(), ModContent.NPCType<CosmicGuardianHead>());
                            }
                        }
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

                    if (spawnDoGCountdown > 0)
                    {
                        spawnDoGCountdown--;
                        if (spawnDoGCountdown == 0)
                        {
                            SoundEngine.PlaySound(AttackSound with { Pitch = AttackSound.Pitch + extrapitch }, player.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                NPC.SpawnOnPlayer(NPC.FindClosestPlayer(), ModContent.NPCType<CosmicGuardianHead>());
                        }
                    }
                }

                // Laser barrage attack variables
                float laserBarrageGateValue = bossRush ? 780f : death ? 900f : 960f;
                float laserBarrageShootGateValue = bossRush ? 160f : 240f;
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
                    float spawnOffset = 1200f;

                    calamityGlobalNPC.newAI[1] += 1f;
                    if (calamityGlobalNPC.newAI[1] >= laserBarragePhaseGateValue)
                    {
                        if (calamityGlobalNPC.newAI[1] >= laserBarrageGateValue)
                            calamityGlobalNPC.newAI[1] = 0f;

                        if (calamityGlobalNPC.newAI[1] % (laserBarrageShootGateValue * 0.5f) == 0f && calamityGlobalNPC.newAI[1] > 0f)
                        {
                            SoundEngine.PlaySound(SoundID.Item12, player.Center);

                            // Side walls
                            int type = ModContent.ProjectileType<DoGDeath>();
                            int damage = NPC.GetProjectileDamage(type);
                            Vector2 start = default;
                            Vector2 velocity = default;
                            Vector2 aim = expertMode ? player.Center + player.velocity * 20f : Vector2.Zero;
                            Vector2 aimClone = aim;

                            switch (laserWallType)
                            {
                                case (int)LaserWallType.DiagonalRight:

                                    for (int x = 0; x < totalShots + 1; x++)
                                    {
                                        start = new Vector2(player.position.X + spawnOffset, player.position.Y + shotSpacing);
                                        aim.Y += laserWallSpacingOffset * (x - 3);
                                        velocity = Vector2.Normalize(aim - start) * laserVelocity;

                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);

                                        shotSpacing -= spacingVar;
                                    }

                                    if (expertMode && Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + spawnOffset, player.Center.Y, -laserVelocity, 0f, type, damage, 0f, Main.myPlayer);

                                    break;

                                case (int)LaserWallType.DiagonalLeft:

                                    for (int x = 0; x < totalShots + 1; x++)
                                    {
                                        start = new Vector2(player.position.X - spawnOffset, player.position.Y + shotSpacing);
                                        aim.Y += laserWallSpacingOffset * (x - 3);
                                        velocity = Vector2.Normalize(aim - start) * laserVelocity;

                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);

                                        shotSpacing -= spacingVar;
                                    }

                                    if (expertMode && Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - spawnOffset, player.Center.Y, laserVelocity, 0f, type, damage, 0f, Main.myPlayer);

                                    break;

                                case (int)LaserWallType.DiagonalHorizontal:

                                    for (int x = 0; x < totalShots + 1; x++)
                                    {
                                        start = new Vector2(player.position.X + spawnOffset, player.position.Y + shotSpacing);
                                        aim.Y += laserWallSpacingOffset * (x - 3);
                                        velocity = Vector2.Normalize(aim - start) * laserVelocity;

                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);

                                        start = new Vector2(player.position.X - spawnOffset, player.position.Y + shotSpacing);
                                        velocity = Vector2.Normalize(aim - start) * laserVelocity;

                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);

                                        shotSpacing -= spacingVar;
                                    }

                                    if (expertMode && Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + spawnOffset, player.Center.Y, -laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - spawnOffset, player.Center.Y, laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                    }

                                    break;

                                case (int)LaserWallType.DiagonalCross:

                                    int randomLaserGap = Main.rand.Next(3) + 3; // 3, 4, 5, 6
                                    for (int x = 0; x < totalShots + 1; x++)
                                    {
                                        if (x != randomLaserGap && x != randomLaserGap + 1)
                                        {
                                            start = new Vector2(player.position.X + spawnOffset, player.position.Y + shotSpacing);
                                            aim.Y += laserWallSpacingOffset * (x - 3);
                                            velocity = Vector2.Normalize(aim - start) * laserVelocity;

                                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                                Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);

                                            start = new Vector2(player.position.X - spawnOffset, player.position.Y + shotSpacing);
                                            velocity = Vector2.Normalize(aim - start) * laserVelocity;

                                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                                Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);

                                            start = new Vector2(player.position.X + shotSpacing, player.position.Y + spawnOffset);
                                            aimClone.X += laserWallSpacingOffset * (x - 3);
                                            velocity = Vector2.Normalize(aimClone - start) * laserVelocity;

                                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                                Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);

                                            start = new Vector2(player.position.X + shotSpacing, player.position.Y - spawnOffset);
                                            velocity = Vector2.Normalize(aimClone - start) * laserVelocity;

                                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                                Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);
                                        }

                                        shotSpacing -= spacingVar;
                                    }

                                    if (expertMode && Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + spawnOffset, player.Center.Y, -laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - spawnOffset, player.Center.Y, laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                    }

                                    break;
                            }

                            // Pick a random laser wall phase in expert+
                            if (expertMode)
                            {
                                int laserWallPhase;
                                int choices = revenge ? 4 : 3;
                                do laserWallPhase = Main.rand.Next(choices);
                                while (laserWallPhase == laserWallType);
                                laserWallType = laserWallPhase;
                            }
                            else
                                laserWallType = laserWallType == (int)LaserWallType.DiagonalRight ? (int)LaserWallType.DiagonalLeft : (int)LaserWallType.DiagonalRight;

                            shotSpacing = shotSpacingMax;
                        }
                    }
                }

                // Opacity
                if (phase2 && calamityGlobalNPC.newAI[1] >= laserBarragePhaseGateValue)
                {
                    // Adjust opacity upon entering laser barrage phase
                    NPC.Opacity = 1f - (MathHelper.Clamp((calamityGlobalNPC.newAI[1] - laserBarragePhaseGateValue) * 5f, 0f, 255f) / 255f);
                }
                else
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

                // Flight
                if (NPC.ai[3] == 0f)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < CalamityGlobalNPC.CatchUpDistance350Tiles)
                            Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<Warped>(), 2);
                    }

                    // Flying movement
                    NPC.localAI[1] = 0f;

                    calamityGlobalNPC.newAI[2] += 1f;

                    // Go to ground phase sooner
                    if (increaseSpeedMore)
                        calamityGlobalNPC.newAI[2] += 10f;
                    else if (increaseSpeed)
                        calamityGlobalNPC.newAI[2] += 2f;

                    float speedCopy = speed;
                    float turnSpeedCopy = turnSpeed;
                    Vector2 npcCenter = NPC.Center;
                    float targetX = player.position.X + (player.width / 2);
                    float targetY = player.position.Y + (player.height / 2);
                    int flyYLevel = -1;
                    int destinationTileX = (int)(destination.X / 16f);
                    int destinationTileY = (int)(destination.Y / 16f);

                    for (int i = destinationTileX - 2; i <= destinationTileX + 2; i++)
                    {
                        for (int j = destinationTileY; j <= destinationTileY + 15; j++)
                        {
                            if (WorldGen.SolidTile2(i, j))
                            {
                                flyYLevel = j;
                                break;
                            }
                        }
                        if (flyYLevel > 0)
                            break;
                    }

                    if (flyYLevel > 0)
                    {
                        flyYLevel *= 16;
                        float chaseFlyLevel = flyYLevel - 800;
                        if (player.position.Y > chaseFlyLevel)
                        {
                            targetY = chaseFlyLevel;
                            if (Math.Abs(NPC.Center.X - destination.X) < 500f)
                            {
                                if (NPC.velocity.X > 0f)
                                    targetX = destination.X + 600f;
                                else
                                    targetX = destination.X - 600f;
                            }
                        }
                    }
                    else
                    {
                        speedCopy = homingSpeed;
                        turnSpeedCopy = homingTurnSpeed;
                    }

                    if (expertMode)
                    {
                        speedCopy += distanceFromTarget * 0.005f * (1f - lifeRatio);
                        turnSpeedCopy += distanceFromTarget * 0.0001f * (1f - lifeRatio);
                    }

                    if (CalamityWorld.LegendaryMode && revenge)
                    {
                        if ((player.Center - NPC.Center).SafeNormalize(Vector2.UnitY).ToRotation().AngleTowards(NPC.velocity.ToRotation(), MathHelper.PiOver4) == NPC.velocity.ToRotation())
                            speedCopy *= 2f;
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

                        NPC.netUpdate = true;

                        // Prevent netUpdate from being blocked by the spam counter.
                        if (NPC.netSpam >= 10)
                            NPC.netSpam = 9;
                    }
                }

                // Ground
                else
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < CalamityGlobalNPC.CatchUpDistance350Tiles)
                            Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<DoGExtremeGravity>(), 2);
                    }

                    calamityGlobalNPC.newAI[2] += 1f;

                    // Enrage
                    if (increaseSpeedMore)
                        groundPhaseTurnSpeed *= 4f;
                    else if (increaseSpeed)
                        groundPhaseTurnSpeed *= 2f;

                    if (CalamityWorld.LegendaryMode && revenge)
                    {
                        if ((player.Center - NPC.Center).SafeNormalize(Vector2.UnitY).ToRotation().AngleTowards(NPC.velocity.ToRotation(), MathHelper.PiOver4) == NPC.velocity.ToRotation())
                            segmentVelocity *= 2f;
                    }

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
                        double maximumSpeed1 = bossRush ? 0.52 : death ? 0.46 : 0.4;
                        double maximumSpeed2 = bossRush ? 1.25 : death ? 1.125 : 1D;

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
                        {
                            NPC.netUpdate = true;

                            // Prevent netUpdate from being blocked by the spam counter.
                            if (NPC.netSpam >= 10)
                                NPC.netSpam = 9;
                        }

                        NPC.localAI[0] = 1f;
                    }
                    else
                    {
                        if (NPC.localAI[0] != 0f)
                        {
                            NPC.netUpdate = true;

                            // Prevent netUpdate from being blocked by the spam counter.
                            if (NPC.netSpam >= 10)
                                NPC.netSpam = 9;
                        }

                        NPC.localAI[0] = 0f;
                    }

                    if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
                    {
                        NPC.netUpdate = true;

                        // Prevent netUpdate from being blocked by the spam counter.
                        if (NPC.netSpam >= 10)
                            NPC.netSpam = 9;
                    }

                    if (calamityGlobalNPC.newAI[2] > phaseLimit)
                    {
                        calamityGlobalNPC.velocityPriorToPhaseSwap = NPC.velocity.Length();
                        NPC.ai[3] = 0f;
                        calamityGlobalNPC.newAI[2] = 0f;
                        NPC.TargetClosest();
                        NPC.netUpdate = true;
                    }
                }
            }

            // There is no escape...
            if (NPC.Distance(player.Center) > 2400f)
                NPC.velocity += (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * turnSpeed;

            // Calculate contact damage based on velocity
            float minimalContactDamageVelocity = segmentVelocity * 0.25f;
            float minimalDamageVelocity = segmentVelocity * 0.5f;
            if (NPC.velocity.Length() <= minimalContactDamageVelocity)
            {
                NPC.damage = (int)Math.Round(NPC.defDamage * 0.5);
            }
            else
            {
                float velocityDamageScalar = MathHelper.Clamp((NPC.velocity.Length() - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                NPC.damage = (int)MathHelper.Lerp((float)Math.Round(NPC.defDamage * 0.5), NPC.defDamage, velocityDamageScalar);
            }

            if (NPC.life > Main.npc[(int)NPC.ai[0]].life)
                NPC.life = Main.npc[(int)NPC.ai[0]].life;
        }

        private void SpawnTeleportLocation(Player player, bool phase2Transition = false)
        {
            if (teleportTimer > 0 || player.dead || !player.active)
                return;

            if (!phase2Transition)
                teleportTimer = (CalamityWorld.death || BossRushEvent.BossRushActive) ? TimeBeforeTeleport_Death : CalamityWorld.revenge ? TimeBeforeTeleport_Revengeance : Main.expertMode ? TimeBeforeTeleport_Expert : TimeBeforeTeleport_Normal;

            SoundEngine.PlaySound(SoundID.Item109, player.Center);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int randomRange = Main.zenithWorld ? 960 : 48;
                float distance = 500f;
                Vector2 targetVector = player.Center + player.velocity.SafeNormalize(Vector2.UnitX) * distance + new Vector2(Main.rand.Next(-randomRange, randomRange + 1), Main.rand.Next(-randomRange, randomRange + 1));
                Projectile.NewProjectile(NPC.GetSource_FromAI(), targetVector, Vector2.Zero, ModContent.ProjectileType<DoGTeleportRift>(), 0, 0f, Main.myPlayer, NPC.whoAmI);

                if (Main.zenithWorld)
                {
                    // Fake portals galore
                    randomRange = 2000;
                    for (int k = 0; k < 35; k++)
                    {
                        targetVector = player.Center + player.velocity.SafeNormalize(Vector2.UnitX) * distance + new Vector2(Main.rand.Next(-randomRange, randomRange + 1), Main.rand.Next(-randomRange, randomRange + 1));
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), targetVector, Vector2.Zero, ModContent.ProjectileType<DoGTeleportRift>(), 0, 0f, Main.myPlayer, NPC.whoAmI, ai2: 1f);
                    }
                }
            }
        }

        private void Teleport(Player player, bool bossRush, bool death, bool revenge, bool expertMode, bool phase5)
        {
            Vector2 newPosition = GetRiftLocation(true);

            if ((!AwaitingPhase2Teleport && (player.dead || !player.active)) || newPosition == default)
                return;

            if (phase5)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int type = ModContent.ProjectileType<DoGFire>();
                    int damage = NPC.GetProjectileDamage(type);
                    float finalVelocity = 10f;
                    int totalSpreads = revenge ? 6 : 3;
                    float mult = revenge ? 1.5f : 3f;
                    for (int i = 0; i < totalSpreads; i++)
                    {
                        int totalProjectiles = (CalamityWorld.LegendaryMode && revenge) ? 30 : bossRush ? 18 : 12;
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
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), newPosition, vector255, type, damage, 0f, Main.myPlayer, velocityMult, finalVelocity - finalVelocityReduction);
                        }
                    }
                }
            }

            NPC.TargetClosest();
            NPC.position = newPosition;
            float chargeVelocity = bossRush ? 30f : death ? 26f : revenge ? 24f : expertMode ? 22f : 20f;
            float maxChargeDistance = 1600f;
            postTeleportTimer = (int)Math.Round(maxChargeDistance / chargeVelocity);
            AwaitingPhase2Teleport = false;
            NPC.Opacity = 1f - (postTeleportTimer / 255f);
            // Prediction is Death Mode only for now because it's weird without the line telegraph that Shayy spoke about
            Vector2 predictionVector = death ? player.velocity * 40f : Vector2.Zero;
            NPC.velocity = Vector2.Normalize(player.Center + predictionVector - NPC.Center) * chargeVelocity;

            NPC.netUpdate = true;

            // Prevent netUpdate from being blocked by the spam counter.
            if (NPC.netSpam >= 10)
                NPC.netSpam = 9;

            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.type == ModContent.NPCType<DevourerofGodsBody>() || n.type == ModContent.NPCType<DevourerofGodsTail>())
                {
                    n.position = newPosition;

                    if (n.type == ModContent.NPCType<DevourerofGodsTail>())
                        ((DevourerofGodsTail)n.ModNPC).setInvulTime(720);

                    n.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    if (n.netSpam >= 10)
                        n.netSpam = 9;
                }
            }

            SoundEngine.PlaySound(AttackSound with { Pitch = AttackSound.Pitch + extrapitch }, player.Center);
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
                            Dust cosmicBurst = Dust.NewDustPerfect(n.Center + Main.rand.NextVector2Circular(25f, 25f), 234);
                            cosmicBurst.scale = 1.7f;
                            cosmicBurst.velocity = Main.rand.NextVector2Circular(9f, 9f);
                            cosmicBurst.noGravity = true;
                        }

                        n.life = 0;
                        n.HitEffect();
                        n.active = false;

                        n.netUpdate = true;

                        // Prevent netUpdate from being blocked by the spam counter.
                        if (n.netSpam >= 10)
                            n.netSpam = 9;

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

                if (Main.netMode != NetmodeID.Server)
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

                NPC.netUpdate = true;

                // Prevent netUpdate from being blocked by the spam counter.
                if (NPC.netSpam >= 10)
                    NPC.netSpam = 9;
            }
            DeathAnimationTimer++;
        }

        private Vector2 GetRiftLocation(bool spawnDust)
        {
            Vector2 realSpot = default;
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.type == ModContent.ProjectileType<DoGTeleportRift>())
                {
                    if (!spawnDust)
                        proj.ai[0] = -1f;

                    proj.Kill();

                    if (proj.ai[2] == 1f)
                        continue;

                    realSpot = proj.Center;
                }
            }
            return realSpot;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;

            float disintegrationFactor = DeathAnimationTimer / 800f;
            if (disintegrationFactor > 0f)
            {
                spriteBatch.EnterShaderRegion();
                GameShaders.Misc["CalamityMod:DoGDisintegration"].UseOpacity(disintegrationFactor);
                GameShaders.Misc["CalamityMod:DoGDisintegration"].UseSaturation(NPC.whoAmI);
                GameShaders.Misc["CalamityMod:DoGDisintegration"].UseImage0("Images/Misc/Perlin");
                GameShaders.Misc["CalamityMod:DoGDisintegration"].Apply();
            }

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            bool useOtherTextures = (Phase2Started && NPC.localAI[2] <= 60f) || NPC.IsABestiaryIconDummy;
            Texture2D texture2D15 = useOtherTextures ? Phase2Texture.Value : TextureAssets.Npc[NPC.type].Value;
            Vector2 halfSizeTexture = new Vector2(texture2D15.Width / 2, texture2D15.Height / 2);
            if (NPC.IsABestiaryIconDummy)
                NPC.frame = texture2D15.Frame();

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(texture2D15.Width, texture2D15.Height) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            if (!NPC.dontTakeDamage)
            {
                texture2D15 = useOtherTextures ? Phase2Texture_Glow.Value : Texture_Glow.Value;
                Color glowmaskLerp = Color.Lerp(Color.White, Color.Fuchsia, 0.5f);

                spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, glowmaskLerp, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

                texture2D15 = useOtherTextures ? Phase2Texture_Glow2.Value : Texture_Glow2.Value;
                glowmaskLerp = Color.Lerp(Color.White, Color.Cyan, 0.5f);

                spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, glowmaskLerp, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
            }

            if (disintegrationFactor > 0f)
                spriteBatch.ExitShaderRegion();

            return false;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<CosmiliteBrick>();
        }

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            CalamityGlobalNPC.SetNewShopVariable(new int[] { ModContent.NPCType<THIEF>() }, DownedBossSystem.downedDoG);

            // If DoG has not been killed yet, notify players that the holiday moons are buffed
            if (!DownedBossSystem.downedDoG)
            {
                string key = "Mods.CalamityMod.Status.Progression.DoGBossText";
                Color messageColor = Color.Cyan;
                string key2 = "Mods.CalamityMod.Status.Progression.DoGBossText2";
                Color messageColor2 = Color.Orange;
                string key3 = "Mods.CalamityMod.Status.Progression.DargonBossText";
                Color messageColor3 = Color.Yellow;

                CalamityUtils.DisplayLocalizedText(key, messageColor);
                CalamityUtils.DisplayLocalizedText(key2, messageColor2);
                CalamityUtils.DisplayLocalizedText(key3, messageColor3);
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

            // Fabsol Mount
            npcLoot.AddIf((info) => info.player.Calamity().fabsolVodka, ModContent.ItemType<Fabsol>());

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<Excelsus>(),
                    ModContent.ItemType<TheObliterator>(),
                    ModContent.ItemType<Deathwind>(),
                    ModContent.ItemType<DeathhailStaff>(),
                    ModContent.ItemType<StaffoftheMechworm>(),
                    ModContent.ItemType<Eradicator>()
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));
                normalOnly.Add(ModContent.ItemType<CosmicDischarge>(), 10);
                normalOnly.Add(ModContent.ItemType<Norfleet>(), 10);

                // Vanity
                normalOnly.Add(ModContent.ItemType<DevourerofGodsMask>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                // Materials
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<CosmiliteBar>(), 1, 45, 55));
                normalOnly.Add(ModContent.ItemType<CosmiliteBrick>(), 1, 150, 250);

                // Equipment
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<NebulousCore>()));
            }

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<DevourerOfGodsRelic>());

            // GFB torch and Wand drops
            var GFBOnly = npcLoot.DefineConditionalDropSet(DropHelper.GFB);
            {
                GFBOnly.Add(ModContent.ItemType<TheWand>(), hideLootReport: true);

                // this will be disastrous for the torch economy
                int dropRate = 10;
                int dropMin = 1;
                int dropMax = 9999;
                GFBOnly.Add(ItemID.Torch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ItemID.PurpleTorch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ItemID.YellowTorch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ItemID.GreenTorch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ItemID.RedTorch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ItemID.WhiteTorch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ItemID.OrangeTorch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ItemID.PinkTorch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ItemID.RainbowTorch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ItemID.IceTorch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ItemID.BoneTorch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ItemID.UltrabrightTorch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ItemID.DemonTorch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ItemID.CursedTorch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ItemID.IchorTorch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ItemID.DesertTorch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ItemID.CoralTorch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ItemID.CorruptTorch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ItemID.CrimsonTorch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ItemID.HallowedTorch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ItemID.JungleTorch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ItemID.MushroomTorch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ItemID.ShimmerTorch, dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ModContent.ItemType<AbyssTorch>(), dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ModContent.ItemType<AlgalPrismTorch>(), dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ModContent.ItemType<AstralTorch>(), dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ModContent.ItemType<GloomTorch>(), dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ModContent.ItemType<NavyPrismTorch>(), dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ModContent.ItemType<RefractivePrismTorch>(), dropRate, dropMin, dropMax, true);
                GFBOnly.Add(ModContent.ItemType<SulphurousTorch>(), dropRate, dropMin, dropMax, true);
            }

            // Trophy (always directly from boss, never in bag)
            npcLoot.Add(ModContent.ItemType<DevourerofGodsTrophy>(), 10);

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedDoG, ModContent.ItemType<LoreDevourerofGods>(), desc: DropHelper.FirstKillText);
        }

        // Can only hit the target if within certain distance
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
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
                modifiers.SourceDamage *= 40f;
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

            NPC.netUpdate = true;

            // Prevent netUpdate from being blocked by the spam counter.
            if (NPC.netSpam >= 10)
                NPC.netSpam = 9;

            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 8;
                SoundEngine.PlaySound(CommonCalamitySounds.OtherwordlyHitSound with { Pitch = CommonCalamitySounds.OtherwordlyHitSound.Pitch + extrapitch }, NPC.Center);
            }
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
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

            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 200, true);
            target.AddBuff(ModContent.BuffType<WhisperingDeath>(), 600, true);

            if (target.Calamity().dogTextCooldown <= 0 && !BossRushEvent.BossRushActive)
            {
                string text = Utils.SelectRandom(Main.rand, new string[]
                {
                    "Mods.CalamityMod.Status.Boss.EdgyBossText2",
                    "Mods.CalamityMod.Status.Boss.EdgyBossText3",
                    "Mods.CalamityMod.Status.Boss.EdgyBossText4"
                });
                Color messageColor = Color.Cyan;
                Rectangle location = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);
                CombatText.NewText(location, messageColor, Language.GetTextValue(text), true);
                target.Calamity().dogTextCooldown = 60;
            }
        }
    }
}
