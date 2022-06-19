using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Placeables.FurnitureCosmilite;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace CalamityMod.NPCs.DevourerofGods
{
    public class DevourerofGodsHead : ModNPC
    {
        public static int phase1IconIndex;
        public static int phase2IconIndex;

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
        private const float laserVelocity = 5f;

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
        private const int shotSpacingMax = 1050;
        private int shotSpacing = shotSpacingMax;
        private const int totalShots = 10;
        private const int spacingVar = shotSpacingMax / totalShots * 2;
        private int laserWallType = 0;
        private const float laserWallSpacingOffset = 16f;

        // Continuously reset variables
        public bool AttemptingToEnterPortal = false;
        public int PortalIndex = -1;

        // Spawn variables
        private bool tail = false;
        private const int minLength = 100;
        private const int maxLength = 101;

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
        private const int shotSpacingMax_Phase2 = 1050;
        private int[] shotSpacing_Phase2 = new int[4] { shotSpacingMax_Phase2, shotSpacingMax_Phase2, shotSpacingMax_Phase2, shotSpacingMax_Phase2 };
        private const int spacingVar_Phase2 = 105;
        private const int totalShots_Phase2 = 20;
        private const int totalDiagonalShots = 6;
        private const int diagonalSpacingVar = shotSpacingMax_Phase2 / totalDiagonalShots * 2;
        private int laserWallType_Phase2 = 0;
        public int laserWallPhase = 0;

        // Phase variables
        private const int idleCounterMax = 240;
        private int idleCounter = idleCounterMax;
        private int postTeleportTimer = 0;
        private int teleportTimer = -1;
        private const int TimeBeforeTeleport_Death = 120;
        private const int TimeBeforeTeleport_Revengeance = 140;
        private const int TimeBeforeTeleport_Expert = 160;
        private const int TimeBeforeTeleport_Normal = 180;
        private bool spawnedGuardians3 = false;
        private const float alphaGateValue = 669f;

        // Death animation variables
        public bool Dying;
        public int DeathAnimationTimer;
        public int DestroyedSegmentCount;

        //Sounds
        public static readonly SoundStyle SpawnSound = new("CalamityMod/Sounds/Custom/DevourerSpawn");
        public static readonly SoundStyle AttackSound = new("CalamityMod/Sounds/Custom/DevourerAttack");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Devourer of Gods");
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
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
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                //Custom background probably?,

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("Its otherworldly ego is known as well as its overwhelming power across the land, as in battle it boasts constantly. Admittedly it is one of the few able to back up its claims.")
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
            NPC.LifeMaxNERB(888750, 1066500, 1500000); // Phase 1 is 371250, Phase 2 is 517500
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
            Music = CalamityMod.Instance.GetMusicFromMusicMod("DevourerOfGodsP1") ?? MusicID.Boss3;
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

            // Stop rain
            CalamityMod.StopRain();

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            // Variables
            Vector2 vector = NPC.Center;
            bool flies = NPC.ai[3] == 0f;
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            NPC.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive && malice;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phase 1 phases
            bool phase2 = lifeRatio < 0.9f;
            bool phase3 = lifeRatio < 0.75f;
            bool summonSentinels = lifeRatio < 0.6f;

            // Phase 2 phases
            bool phase4 = lifeRatio < 0.5f;
            bool phase5 = lifeRatio < 0.4f;
            bool phase6 = lifeRatio < 0.2f;
            bool phase7 = lifeRatio < 0.15f;

            // Velocity variables
            float fallSpeed = malice ? 19f : death ? 17.5f : 16f;
            if (expertMode)
                fallSpeed += 4f * (1f - lifeRatio);

            float speed = malice ? 18f : death ? 16.5f : 15f;
            float turnSpeed = malice ? 0.36f : death ? 0.33f : 0.3f;
            float homingSpeed = malice ? 36f : death ? 30f : 24f;
            float homingTurnSpeed = malice ? 0.48f : death ? 0.405f : 0.33f;

            if (expertMode)
            {
                speed += 3f * (1f - lifeRatio);
                turnSpeed += 0.06f * (1f - lifeRatio);
                homingSpeed += 12f * (1f - lifeRatio);
                homingTurnSpeed += 0.15f * (1f - lifeRatio);
            }

            float groundPhaseTurnSpeed = malice ? 0.3f : death ? 0.24f : 0.18f;

            if (expertMode)
                groundPhaseTurnSpeed += 0.1f * (1f - lifeRatio);

            groundPhaseTurnSpeed += Vector2.Distance(player.Center, NPC.Center) * 0.0002f;

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

            // Light
            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);

            // Worm variable
            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            // Despawn
            if (player.dead)
            {
                NPC.ai[3] = 0f;
                calamityGlobalNPC.newAI[2] = 0f;

                NPC.velocity.Y -= 3f;
                if ((double)NPC.position.Y < Main.topWorld + 16f)
                    NPC.velocity.Y -= 3f;

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
                    }
                }
            }

            float distanceFromTarget = Vector2.Distance(player.Center, vector);
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

            // Close DoG's HP bar if busy with sentinels or a P2 transition and decrement the countdown.
            if (NPC.localAI[2] > 0f)
            {
                NPC.localAI[2] -= 1f;
                NPC.Calamity().ShouldCloseHPBar = true;
            }

            // Teleport after the Phase 2 animation.
            float timeWhenDoGShouldTeleportDuringPhase2Countdown = 61f;
            if (NPC.localAI[2] == timeWhenDoGShouldTeleportDuringPhase2Countdown + ((CalamityWorld.death || BossRushEvent.BossRushActive) ? TimeBeforeTeleport_Death : CalamityWorld.revenge ? TimeBeforeTeleport_Revengeance : Main.expertMode ? TimeBeforeTeleport_Expert : TimeBeforeTeleport_Normal))
                SpawnTeleportLocation(player, true);
            if (NPC.localAI[2] == timeWhenDoGShouldTeleportDuringPhase2Countdown)
                Teleport(player, malice, death, revenge, expertMode, phase5);

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

            // Start sentinel phases, only run things that have to happen once in here
            if (summonSentinels)
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
                    NPC.localAI[2] = 600f;
                }

                // Play music after the transiton BS
                if (NPC.localAI[2] == 530f)
                    Music = CalamityMod.Instance.GetMusicFromMusicMod("DevourerOfGodsP2") ?? MusicID.LunarBoss;

                // Once before DoG spawns, set new size and become visible again.
                if (NPC.localAI[2] == 60f)
                {
                    NPC.position = NPC.Center;
                    NPC.width = 186;
                    NPC.height = 186;
                    NPC.position -= NPC.Size * 0.5f;
                    NPC.frame = new Rectangle(0, 0, 134, 196);
                    NPC.netUpdate = true;
                }

                // Dialogue the moment the second phase starts
                if (NPC.localAI[2] == 60f)
                {
                    string key = "Mods.CalamityMod.EdgyBossText10";
                    Color messageColor = Color.Cyan;
                    CalamityUtils.DisplayLocalizedText(key, messageColor);
                }
            }

            // Begin phase 2 once all sentinels are down
            if (Phase2Started)
            {
                // Go immune and invisible
                if (NPC.localAI[2] > 5f)
                {
                    // Don't take damage
                    NPC.dontTakeDamage = true;

                    // Adjust movement speed. Direction is unaltered unless DoG is close to the top of the world, in which case he moves horizontally.
                    // A portal will be created ahead of where DoG is moving that he will enter before Phase 2 begins.
                    float idealFlySpeed = 14f;

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
                            Teleport(player, malice, death, revenge, expertMode, phase5);
                    }

                    // Do the death animation once killed.
                    if (Dying)
                    {
                        teleportTimer = 0;
                        DoDeathAnimation();
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
                            // End laser wall phase after 4.25 seconds
                            NPC.Opacity += 0.004f;
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
                                    string key = "Mods.CalamityMod.EdgyBossText11";
                                    Color messageColor = Color.Cyan;
                                    CalamityUtils.DisplayLocalizedText(key, messageColor);

                                    // Summon Cosmic Guardians
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        SoundEngine.PlaySound(AttackSound, player.position);

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

                    // Spawn segments and fire projectiles
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        // Fireballs
                        if (NPC.Opacity >= 1f && distanceFromTarget > 500f)
                        {
                            calamityGlobalNPC.newAI[0] += 1f;
                            if (calamityGlobalNPC.newAI[0] >= 150f && calamityGlobalNPC.newAI[0] % (phase7 ? 30f : 60f) == 0f)
                            {
                                Vector2 vector44 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
                                float num427 = player.position.X + (player.width / 2) - vector44.X;
                                float num428 = player.position.Y + (player.height / 2) - vector44.Y;
                                float num430 = 8f;
                                float num429 = (float)Math.Sqrt(num427 * num427 + num428 * num428);
                                num429 = num430 / num429;
                                num427 *= num429;
                                num428 *= num429;
                                num428 += NPC.velocity.Y * 0.5f;
                                num427 += NPC.velocity.X * 0.5f;
                                vector44.X -= num427;
                                vector44.Y -= num428;

                                int type = ModContent.ProjectileType<DoGFire>();
                                int damage = NPC.GetProjectileDamage(type);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), vector44.X, vector44.Y, num427, num428, type, damage, 0f, Main.myPlayer);
                            }
                        }
                        else if (distanceFromTarget < 250f)
                            calamityGlobalNPC.newAI[0] = 0f;

                        // Laser walls
                        if (!spawnedGuardians3 && laserWallPhase == (int)LaserWallPhase.FireLaserWalls)
                        {
                            float spawnOffset = 1500f;
                            float divisor = malice ? 80f : 120f;

                            if (calamityGlobalNPC.newAI[1] % divisor == 0f)
                            {
                                SoundEngine.PlaySound(SoundID.Item12, player.position);

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
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + spawnOffset, targetPosY + shotSpacing_Phase2[0], -laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - spawnOffset, targetPosY + shotSpacing_Phase2[0], laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                            shotSpacing_Phase2[0] -= spacingVar_Phase2;
                                        }

                                        if (expertMode)
                                        {
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + spawnOffset, player.Center.Y, -laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - spawnOffset, player.Center.Y, laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                        }

                                        break;

                                    case (int)LaserWallType_Phase2.Offset:

                                        targetPosY += 50f;
                                        for (int x = 0; x < totalShots_Phase2; x++)
                                        {
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + spawnOffset, targetPosY + shotSpacing_Phase2[0], -laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - spawnOffset, targetPosY + shotSpacing_Phase2[0], laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                            shotSpacing_Phase2[0] -= spacingVar_Phase2;
                                        }

                                        if (expertMode)
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
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);

                                            start = new Vector2(player.position.X - spawnOffset, targetPosY + shotSpacing_Phase2[0]);
                                            velocity = Vector2.Normalize(aim - start) * laserVelocity;
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);

                                            shotSpacing_Phase2[0] -= diagonalSpacingVar;
                                        }

                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X, targetPosY + spawnOffset, 0f, -laserVelocity, type, damage, 0f, Main.myPlayer);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X, targetPosY - spawnOffset, 0f, laserVelocity, type, damage, 0f, Main.myPlayer);

                                        break;

                                    case (int)LaserWallType_Phase2.MultiLayered:

                                        for (int x = 0; x < totalShots_Phase2; x++)
                                        {
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + spawnOffset, targetPosY + shotSpacing_Phase2[0], -laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - spawnOffset, targetPosY + shotSpacing_Phase2[0], laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                            shotSpacing_Phase2[0] -= spacingVar_Phase2;
                                        }

                                        int totalBonusLasers = totalShots_Phase2 / 2;
                                        for (int x = 0; x < totalBonusLasers; x++)
                                        {
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + spawnOffset, targetPosY + shotSpacing_Phase2[3], -laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - spawnOffset, targetPosY + shotSpacing_Phase2[3], laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                            shotSpacing_Phase2[3] -= Main.rand.NextBool(2) ? 180 : 200;
                                        }

                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + spawnOffset, player.Center.Y, -laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - spawnOffset, player.Center.Y, laserVelocity, 0f, type, damage, 0f, Main.myPlayer);

                                        break;

                                    case (int)LaserWallType_Phase2.DiagonalVertical:

                                        for (int x = 0; x < totalDiagonalShots + 1; x++)
                                        {
                                            start = new Vector2(player.position.X + shotSpacing_Phase2[0], targetPosY + spawnOffset);
                                            aim.X += laserWallSpacingOffset * (x - halfTotalDiagonalShots);
                                            velocity = Vector2.Normalize(aim - start) * laserVelocity;
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);

                                            start = new Vector2(player.position.X + shotSpacing_Phase2[0], targetPosY - spawnOffset);
                                            velocity = Vector2.Normalize(aim - start) * laserVelocity;
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);

                                            shotSpacing_Phase2[0] -= diagonalSpacingVar;
                                        }

                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + spawnOffset, player.Center.Y, -laserVelocity, 0f, type, damage, 0f, Main.myPlayer);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - spawnOffset, player.Center.Y, laserVelocity, 0f, type, damage, 0f, Main.myPlayer);

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
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + shotSpacing_Phase2[1], player.position.Y + spawnOffset, 0f, -laserVelocity, type, damage, 0f, Main.myPlayer);
                                    shotSpacing_Phase2[1] -= spacingVar_Phase2;
                                }

                                // Upper wall
                                for (int x = 0; x < totalShots_Phase2; x++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + shotSpacing_Phase2[2], player.position.Y - spawnOffset, 0f, laserVelocity, type, damage, 0f, Main.myPlayer);
                                    shotSpacing_Phase2[2] -= spacingVar_Phase2;
                                }

                                for (int i = 0; i < shotSpacing_Phase2.Length; i++)
                                    shotSpacing_Phase2[i] = shotSpacingMax_Phase2;
                            }

                            calamityGlobalNPC.newAI[1] += 1f;
                        }
                    }

                    // Set flight time to max during laser walls
                    if (!spawnedGuardians3 && laserWallPhase == (int)LaserWallPhase.FireLaserWalls)
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, vector) < CalamityGlobalNPC.CatchUpDistance350Tiles)
                            {
                                if (Main.player[Main.myPlayer].wingTime < Main.player[Main.myPlayer].wingTimeMax)
                                    Main.player[Main.myPlayer].wingTime = Main.player[Main.myPlayer].wingTimeMax;
                            }
                        }
                    }

                    // Movement
                    int num180 = (int)(NPC.position.X / 16f) - 1;
                    int num181 = (int)((NPC.position.X + NPC.width) / 16f) + 2;
                    int num182 = (int)(NPC.position.Y / 16f) - 1;
                    int num183 = (int)((NPC.position.Y + NPC.height) / 16f) + 2;

                    if (num180 < 0)
                        num180 = 0;
                    if (num181 > Main.maxTilesX)
                        num181 = Main.maxTilesX;
                    if (num182 < 0)
                        num182 = 0;
                    if (num183 > Main.maxTilesY)
                        num183 = Main.maxTilesY;

                    if (NPC.velocity.X < 0f)
                        NPC.spriteDirection = -1;
                    else if (NPC.velocity.X > 0f)
                        NPC.spriteDirection = 1;

                    // Flight
                    if (NPC.ai[3] == 0f)
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, vector) < CalamityGlobalNPC.CatchUpDistance350Tiles)
                                Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<Warped>(), 2);
                        }

                        // Charge in a direction for a second until the timer is back at 0
                        if (postTeleportTimer > 0)
                        {
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

                        float num188 = speed;
                        float num189 = turnSpeed;
                        Vector2 vector18 = NPC.Center;
                        float num191 = player.Center.X;
                        float num192 = player.Center.Y;
                        int num42 = -1;
                        int num43 = (int)(player.Center.X / 16f);
                        int num44 = (int)(player.Center.Y / 16f);

                        // Charge at target for 1.5 seconds
                        bool flyAtTarget = (!phase4 || spawnedGuardians3) && calamityGlobalNPC.newAI[2] > phaseLimit - 90 && revenge;

                        for (int num45 = num43 - 2; num45 <= num43 + 2; num45++)
                        {
                            for (int num46 = num44; num46 <= num44 + 15; num46++)
                            {
                                if (WorldGen.SolidTile2(num45, num46))
                                {
                                    num42 = num46;
                                    break;
                                }
                            }
                            if (num42 > 0)
                                break;
                        }

                        if (!flyAtTarget)
                        {
                            if (num42 > 0)
                            {
                                num42 *= 16;
                                float num47 = num42 - 800;
                                if (player.position.Y > num47)
                                {
                                    num192 = num47;
                                    if (Math.Abs(NPC.Center.X - player.Center.X) < 500f)
                                    {
                                        if (NPC.velocity.X > 0f)
                                            num191 = player.Center.X + 600f;
                                        else
                                            num191 = player.Center.X - 600f;
                                    }
                                }
                            }
                        }
                        else
                        {
                            num188 = homingSpeed;
                            num189 = homingTurnSpeed;
                        }

                        num188 += Vector2.Distance(player.Center, NPC.Center) * 0.005f;
                        num189 += Vector2.Distance(player.Center, NPC.Center) * 0.00025f;

                        float num48 = num188 * 1.3f;
                        float num49 = num188 * 0.7f;
                        float num50 = NPC.velocity.Length();
                        if (num50 > 0f)
                        {
                            if (num50 > num48)
                            {
                                NPC.velocity.Normalize();
                                NPC.velocity *= num48;
                            }
                            else if (num50 < num49)
                            {
                                NPC.velocity.Normalize();
                                NPC.velocity *= num49;
                            }
                        }

                        num191 = (int)(num191 / 16f) * 16;
                        num192 = (int)(num192 / 16f) * 16;
                        vector18.X = (int)(vector18.X / 16f) * 16;
                        vector18.Y = (int)(vector18.Y / 16f) * 16;
                        num191 -= vector18.X;
                        num192 -= vector18.Y;
                        float num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
                        float num196 = Math.Abs(num191);
                        float num197 = Math.Abs(num192);
                        float num198 = num188 / num193;
                        num191 *= num198;
                        num192 *= num198;

                        if ((NPC.velocity.X > 0f && num191 > 0f) || (NPC.velocity.X < 0f && num191 < 0f) || (NPC.velocity.Y > 0f && num192 > 0f) || (NPC.velocity.Y < 0f && num192 < 0f))
                        {
                            if (NPC.velocity.X < num191)
                                NPC.velocity.X += num189;
                            else
                            {
                                if (NPC.velocity.X > num191)
                                    NPC.velocity.X -= num189;
                            }

                            if (NPC.velocity.Y < num192)
                                NPC.velocity.Y += num189;
                            else
                            {
                                if (NPC.velocity.Y > num192)
                                    NPC.velocity.Y -= num189;
                            }

                            if (Math.Abs(num192) < num188 * 0.2 && ((NPC.velocity.X > 0f && num191 < 0f) || (NPC.velocity.X < 0f && num191 > 0f)))
                            {
                                if (NPC.velocity.Y > 0f)
                                    NPC.velocity.Y += num189 * 2f;
                                else
                                    NPC.velocity.Y -= num189 * 2f;
                            }

                            if (Math.Abs(num191) < num188 * 0.2 && ((NPC.velocity.Y > 0f && num192 < 0f) || (NPC.velocity.Y < 0f && num192 > 0f)))
                            {
                                if (NPC.velocity.X > 0f)
                                    NPC.velocity.X += num189 * 2f;
                                else
                                    NPC.velocity.X -= num189 * 2f;
                            }
                        }
                        else
                        {
                            if (num196 > num197)
                            {
                                if (NPC.velocity.X < num191)
                                    NPC.velocity.X += num189 * 1.1f;
                                else if (NPC.velocity.X > num191)
                                    NPC.velocity.X -= num189 * 1.1f;

                                if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < num188 * 0.5)
                                {
                                    if (NPC.velocity.Y > 0f)
                                        NPC.velocity.Y += num189;
                                    else
                                        NPC.velocity.Y -= num189;
                                }
                            }
                            else
                            {
                                if (NPC.velocity.Y < num192)
                                    NPC.velocity.Y += num189 * 1.1f;
                                else if (NPC.velocity.Y > num192)
                                    NPC.velocity.Y -= num189 * 1.1f;

                                if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < num188 * 0.5)
                                {
                                    if (NPC.velocity.X > 0f)
                                        NPC.velocity.X += num189;
                                    else
                                        NPC.velocity.X -= num189;
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
                        }
                    }

                    // Ground
                    else
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, vector) < CalamityGlobalNPC.CatchUpDistance350Tiles)
                                Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<DoGExtremeGravity>(), 2);
                        }

                        // Charge in a direction for a second until the timer is back at 0
                        if (postTeleportTimer > 0)
                        {
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

                        if (!flies)
                        {
                            for (int num952 = num180; num952 < num181; num952++)
                            {
                                for (int num953 = num182; num953 < num183; num953++)
                                {
                                    if (Main.tile[num952, num953] != null && ((Main.tile[num952, num953].HasUnactuatedTile && (Main.tileSolid[Main.tile[num952, num953].TileType] || (Main.tileSolidTop[Main.tile[num952, num953].TileType] && Main.tile[num952, num953].TileFrameY == 0))) || Main.tile[num952, num953].LiquidAmount > 64))
                                    {
                                        Vector2 vector105;
                                        vector105.X = num952 * 16;
                                        vector105.Y = num953 * 16;
                                        if (NPC.position.X + NPC.width > vector105.X && NPC.position.X < vector105.X + 16f && NPC.position.Y + NPC.height > vector105.Y && NPC.position.Y < vector105.Y + 16f)
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

                            int num954 = death ? 1125 : 1200;

                            if (expertMode)
                                num954 -= (int)(150f * (1f - lifeRatio));

                            if (num954 < 1050)
                                num954 = 1050;

                            bool flag95 = true;
                            if (NPC.position.Y > player.position.Y)
                            {
                                for (int num955 = 0; num955 < Main.maxPlayers; num955++)
                                {
                                    if (Main.player[num955].active)
                                    {
                                        Rectangle rectangle13 = new Rectangle((int)Main.player[num955].position.X - 1000, (int)Main.player[num955].position.Y - 1000, 2000, num954);
                                        if (rectangle12.Intersects(rectangle13))
                                        {
                                            flag95 = false;
                                            break;
                                        }
                                    }
                                }
                                if (flag95)
                                    flies = true;
                            }
                        }
                        else
                            NPC.localAI[1] = 0f;

                        float num189 = groundPhaseTurnSpeed;
                        Vector2 vector18 = NPC.Center;
                        float num191 = player.Center.X;
                        float num192 = player.Center.Y;
                        num191 = (int)(num191 / 16f) * 16;
                        num192 = (int)(num192 / 16f) * 16;
                        vector18.X = (int)(vector18.X / 16f) * 16;
                        vector18.Y = (int)(vector18.Y / 16f) * 16;
                        num191 -= vector18.X;
                        num192 -= vector18.Y;

                        if (!flies)
                        {
                            NPC.velocity.Y += groundPhaseTurnSpeed;
                            if (NPC.velocity.Y > fallSpeed)
                                NPC.velocity.Y = fallSpeed;

                            if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < fallSpeed * 2.2)
                            {
                                if (NPC.velocity.X < 0f)
                                    NPC.velocity.X -= num189 * 1.1f;
                                else
                                    NPC.velocity.X += num189 * 1.1f;
                            }
                            else if (NPC.velocity.Y == fallSpeed)
                            {
                                if (NPC.velocity.X < num191)
                                    NPC.velocity.X += num189;
                                else if (NPC.velocity.X > num191)
                                    NPC.velocity.X -= num189;
                            }
                            else if (NPC.velocity.Y > 4f)
                            {
                                if (NPC.velocity.X < 0f)
                                    NPC.velocity.X += num189 * 0.9f;
                                else
                                    NPC.velocity.X -= num189 * 0.9f;
                            }
                        }
                        else
                        {
                            double maximumSpeed1 = malice ? 0.52 : death ? 0.46 : 0.4;
                            double maximumSpeed2 = malice ? 1.25 : death ? 1.125 : 1D;

                            if (expertMode)
                            {
                                maximumSpeed1 += 0.1f * (1f - lifeRatio);
                                maximumSpeed2 += 0.2f * (1f - lifeRatio);
                            }

                            float num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
                            float num25 = Math.Abs(num191);
                            float num26 = Math.Abs(num192);
                            float num27 = fallSpeed / num193;
                            num191 *= num27;
                            num192 *= num27;

                            if (((NPC.velocity.X > 0f && num191 > 0f) || (NPC.velocity.X < 0f && num191 < 0f)) && ((NPC.velocity.Y > 0f && num192 > 0f) || (NPC.velocity.Y < 0f && num192 < 0f)))
                            {
                                if (NPC.velocity.X < num191)
                                    NPC.velocity.X += groundPhaseTurnSpeed * 1.5f;
                                else if (NPC.velocity.X > num191)
                                    NPC.velocity.X -= groundPhaseTurnSpeed * 1.5f;

                                if (NPC.velocity.Y < num192)
                                    NPC.velocity.Y += groundPhaseTurnSpeed * 1.5f;
                                else if (NPC.velocity.Y > num192)
                                    NPC.velocity.Y -= groundPhaseTurnSpeed * 1.5f;
                            }

                            if ((NPC.velocity.X > 0f && num191 > 0f) || (NPC.velocity.X < 0f && num191 < 0f) || (NPC.velocity.Y > 0f && num192 > 0f) || (NPC.velocity.Y < 0f && num192 < 0f))
                            {
                                if (NPC.velocity.X < num191)
                                    NPC.velocity.X += groundPhaseTurnSpeed;
                                else if (NPC.velocity.X > num191)
                                    NPC.velocity.X -= groundPhaseTurnSpeed;

                                if (NPC.velocity.Y < num192)
                                    NPC.velocity.Y += groundPhaseTurnSpeed;
                                else if (NPC.velocity.Y > num192)
                                    NPC.velocity.Y -= groundPhaseTurnSpeed;

                                if (Math.Abs(num192) < fallSpeed * maximumSpeed1 && ((NPC.velocity.X > 0f && num191 < 0f) || (NPC.velocity.X < 0f && num191 > 0f)))
                                {
                                    if (NPC.velocity.Y > 0f)
                                        NPC.velocity.Y += groundPhaseTurnSpeed * 2f;
                                    else
                                        NPC.velocity.Y -= groundPhaseTurnSpeed * 2f;
                                }

                                if (Math.Abs(num191) < fallSpeed * maximumSpeed1 && ((NPC.velocity.Y > 0f && num192 < 0f) || (NPC.velocity.Y < 0f && num192 > 0f)))
                                {
                                    if (NPC.velocity.X > 0f)
                                        NPC.velocity.X += groundPhaseTurnSpeed * 2f;
                                    else
                                        NPC.velocity.X -= groundPhaseTurnSpeed * 2f;
                                }
                            }
                            else if (num25 > num26)
                            {
                                if (NPC.velocity.X < num191)
                                    NPC.velocity.X += groundPhaseTurnSpeed * 1.1f;
                                else if (NPC.velocity.X > num191)
                                    NPC.velocity.X -= groundPhaseTurnSpeed * 1.1f;

                                if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < fallSpeed * maximumSpeed2)
                                {
                                    if (NPC.velocity.Y > 0f)
                                        NPC.velocity.Y += groundPhaseTurnSpeed;
                                    else
                                        NPC.velocity.Y -= groundPhaseTurnSpeed;
                                }
                            }
                            else
                            {
                                if (NPC.velocity.Y < num192)
                                    NPC.velocity.Y += groundPhaseTurnSpeed * 1.1f;
                                else if (NPC.velocity.Y > num192)
                                    NPC.velocity.Y -= groundPhaseTurnSpeed * 1.1f;

                                if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < fallSpeed * maximumSpeed2)
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
                                NPC.netUpdate = true;

                            NPC.localAI[0] = 1f;
                        }
                        else
                        {
                            if (NPC.localAI[0] != 0f)
                                NPC.netUpdate = true;

                            NPC.localAI[0] = 0f;
                        }

                        if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
                            NPC.netUpdate = true;

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
            }
            else
            {
                // Spawn Guardians
                if (phase3)
                {
                    if (!spawnedGuardians)
                    {
                        if (revenge)
                            spawnDoGCountdown = 10;

                        string key = "Mods.CalamityMod.EdgyBossText";
                        Color messageColor = Color.Cyan;
                        CalamityUtils.DisplayLocalizedText(key, messageColor);

                        NPC.TargetClosest();
                        spawnedGuardians = true;
                    }

                    if (spawnDoGCountdown > 0)
                    {
                        spawnDoGCountdown--;
                        if (spawnDoGCountdown == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            SoundEngine.PlaySound(AttackSound, player.position);

                            for (int i = 0; i < 2; i++)
                                NPC.SpawnOnPlayer(NPC.FindClosestPlayer(), ModContent.NPCType<CosmicGuardianHead>());
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
                        if (spawnDoGCountdown == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            SoundEngine.PlaySound(AttackSound, player.position);

                            NPC.SpawnOnPlayer(NPC.FindClosestPlayer(), ModContent.NPCType<CosmicGuardianHead>());
                        }
                    }
                }

                // Laser barrage attack variables
                float laserBarrageGateValue = malice ? 780f : death ? 900f : 960f;
                float laserBarrageShootGateValue = malice ? 160f : 240f;
                float laserBarragePhaseGateValue = laserBarrageGateValue - laserBarrageShootGateValue;

                // Spawn segments
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (!tail && NPC.ai[0] == 0f)
                    {
                        int Previous = NPC.whoAmI;
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

                    if (phase2)
                    {
                        float spawnOffset = 1500f;

                        calamityGlobalNPC.newAI[1] += 1f;
                        if (calamityGlobalNPC.newAI[1] >= laserBarragePhaseGateValue)
                        {
                            if (calamityGlobalNPC.newAI[1] >= laserBarrageGateValue)
                                calamityGlobalNPC.newAI[1] = 0f;

                            if (calamityGlobalNPC.newAI[1] % (laserBarrageShootGateValue * 0.5f) == 0f && calamityGlobalNPC.newAI[1] > 0f)
                            {
                                SoundEngine.PlaySound(SoundID.Item12, player.position);

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
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);

                                            shotSpacing -= spacingVar;
                                        }

                                        if (expertMode)
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + spawnOffset, player.Center.Y, -laserVelocity, 0f, type, damage, 0f, Main.myPlayer);

                                        break;

                                    case (int)LaserWallType.DiagonalLeft:

                                        for (int x = 0; x < totalShots + 1; x++)
                                        {
                                            start = new Vector2(player.position.X - spawnOffset, player.position.Y + shotSpacing);
                                            aim.Y += laserWallSpacingOffset * (x - 3);
                                            velocity = Vector2.Normalize(aim - start) * laserVelocity;
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);

                                            shotSpacing -= spacingVar;
                                        }

                                        if (expertMode)
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - spawnOffset, player.Center.Y, laserVelocity, 0f, type, damage, 0f, Main.myPlayer);

                                        break;

                                    case (int)LaserWallType.DiagonalHorizontal:

                                        for (int x = 0; x < totalShots + 1; x++)
                                        {
                                            start = new Vector2(player.position.X + spawnOffset, player.position.Y + shotSpacing);
                                            aim.Y += laserWallSpacingOffset * (x - 3);
                                            velocity = Vector2.Normalize(aim - start) * laserVelocity;
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);

                                            start = new Vector2(player.position.X - spawnOffset, player.position.Y + shotSpacing);
                                            velocity = Vector2.Normalize(aim - start) * laserVelocity;
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);

                                            shotSpacing -= spacingVar;
                                        }

                                        if (expertMode)
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
                                                Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);

                                                start = new Vector2(player.position.X - spawnOffset, player.position.Y + shotSpacing);
                                                velocity = Vector2.Normalize(aim - start) * laserVelocity;
                                                Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);

                                                start = new Vector2(player.position.X + shotSpacing, player.position.Y + spawnOffset);
                                                aimClone.X += laserWallSpacingOffset * (x - 3);
                                                velocity = Vector2.Normalize(aimClone - start) * laserVelocity;
                                                Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);

                                                start = new Vector2(player.position.X + shotSpacing, player.position.Y - spawnOffset);
                                                velocity = Vector2.Normalize(aimClone - start) * laserVelocity;
                                                Projectile.NewProjectile(NPC.GetSource_FromAI(), start, velocity, type, damage, 0f, Main.myPlayer);
                                            }

                                            shotSpacing -= spacingVar;
                                        }

                                        if (expertMode)
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
                }

                // Opacity
                if (phase2 && calamityGlobalNPC.newAI[1] >= laserBarragePhaseGateValue)
                {
                    // Adjust opacity upon entering laser barrage phase
                    NPC.Opacity = 1f - (MathHelper.Clamp((calamityGlobalNPC.newAI[1] - laserBarragePhaseGateValue) * 5f, 0f, 255f) / 255f);
                }
                else
                {
                    NPC.Opacity += 0.047f;
                    if (NPC.Opacity > 1f)
                        NPC.Opacity = 1f;
                }

                // Movement
                int num180 = (int)(NPC.position.X / 16f) - 1;
                int num181 = (int)((NPC.position.X + NPC.width) / 16f) + 2;
                int num182 = (int)(NPC.position.Y / 16f) - 1;
                int num183 = (int)((NPC.position.Y + NPC.height) / 16f) + 2;

                if (num180 < 0)
                    num180 = 0;
                if (num181 > Main.maxTilesX)
                    num181 = Main.maxTilesX;
                if (num182 < 0)
                    num182 = 0;
                if (num183 > Main.maxTilesY)
                    num183 = Main.maxTilesY;

                if (NPC.velocity.X < 0f)
                    NPC.spriteDirection = -1;
                else if (NPC.velocity.X > 0f)
                    NPC.spriteDirection = 1;

                // Flight
                if (NPC.ai[3] == 0f)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, vector) < CalamityGlobalNPC.CatchUpDistance350Tiles)
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

                    float num188 = speed;
                    float num189 = turnSpeed;
                    Vector2 vector18 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
                    float num191 = player.position.X + (player.width / 2);
                    float num192 = player.position.Y + (player.height / 2);
                    int num42 = -1;
                    int num43 = (int)(player.Center.X / 16f);
                    int num44 = (int)(player.Center.Y / 16f);

                    for (int num45 = num43 - 2; num45 <= num43 + 2; num45++)
                    {
                        for (int num46 = num44; num46 <= num44 + 15; num46++)
                        {
                            if (WorldGen.SolidTile2(num45, num46))
                            {
                                num42 = num46;
                                break;
                            }
                        }
                        if (num42 > 0)
                            break;
                    }

                    if (num42 > 0)
                    {
                        num42 *= 16;
                        float num47 = num42 - 800;
                        if (player.position.Y > num47)
                        {
                            num192 = num47;
                            if (Math.Abs(NPC.Center.X - player.Center.X) < 500f)
                            {
                                if (NPC.velocity.X > 0f)
                                    num191 = player.Center.X + 600f;
                                else
                                    num191 = player.Center.X - 600f;
                            }
                        }
                    }
                    else
                    {
                        num188 = homingSpeed;
                        num189 = homingTurnSpeed;
                    }

                    if (expertMode)
                    {
                        num188 += distanceFromTarget * 0.005f * (1f - lifeRatio);
                        num189 += distanceFromTarget * 0.0001f * (1f - lifeRatio);
                    }

                    float num48 = num188 * 1.3f;
                    float num49 = num188 * 0.7f;
                    float num50 = NPC.velocity.Length();
                    if (num50 > 0f)
                    {
                        if (num50 > num48)
                        {
                            NPC.velocity.Normalize();
                            NPC.velocity *= num48;
                        }
                        else if (num50 < num49)
                        {
                            NPC.velocity.Normalize();
                            NPC.velocity *= num49;
                        }
                    }

                    num191 = (int)(num191 / 16f) * 16;
                    num192 = (int)(num192 / 16f) * 16;
                    vector18.X = (int)(vector18.X / 16f) * 16;
                    vector18.Y = (int)(vector18.Y / 16f) * 16;
                    num191 -= vector18.X;
                    num192 -= vector18.Y;
                    float num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
                    float num196 = Math.Abs(num191);
                    float num197 = Math.Abs(num192);
                    float num198 = num188 / num193;
                    num191 *= num198;
                    num192 *= num198;

                    if ((NPC.velocity.X > 0f && num191 > 0f) || (NPC.velocity.X < 0f && num191 < 0f) || (NPC.velocity.Y > 0f && num192 > 0f) || (NPC.velocity.Y < 0f && num192 < 0f))
                    {
                        if (NPC.velocity.X < num191)
                            NPC.velocity.X += num189;
                        else
                        {
                            if (NPC.velocity.X > num191)
                                NPC.velocity.X -= num189;
                        }

                        if (NPC.velocity.Y < num192)
                            NPC.velocity.Y += num189;
                        else
                        {
                            if (NPC.velocity.Y > num192)
                                NPC.velocity.Y -= num189;
                        }

                        if (Math.Abs(num192) < num188 * 0.2 && ((NPC.velocity.X > 0f && num191 < 0f) || (NPC.velocity.X < 0f && num191 > 0f)))
                        {
                            if (NPC.velocity.Y > 0f)
                                NPC.velocity.Y += num189 * 2f;
                            else
                                NPC.velocity.Y -= num189 * 2f;
                        }

                        if (Math.Abs(num191) < num188 * 0.2 && ((NPC.velocity.Y > 0f && num192 < 0f) || (NPC.velocity.Y < 0f && num192 > 0f)))
                        {
                            if (NPC.velocity.X > 0f)
                                NPC.velocity.X += num189 * 2f;
                            else
                                NPC.velocity.X -= num189 * 2f;
                        }
                    }
                    else
                    {
                        if (num196 > num197)
                        {
                            if (NPC.velocity.X < num191)
                                NPC.velocity.X += num189 * 1.1f;
                            else if (NPC.velocity.X > num191)
                                NPC.velocity.X -= num189 * 1.1f;

                            if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < num188 * 0.5)
                            {
                                if (NPC.velocity.Y > 0f)
                                    NPC.velocity.Y += num189;
                                else
                                    NPC.velocity.Y -= num189;
                            }
                        }
                        else
                        {
                            if (NPC.velocity.Y < num192)
                                NPC.velocity.Y += num189 * 1.1f;
                            else if (NPC.velocity.Y > num192)
                                NPC.velocity.Y -= num189 * 1.1f;

                            if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < num188 * 0.5)
                            {
                                if (NPC.velocity.X > 0f)
                                    NPC.velocity.X += num189;
                                else
                                    NPC.velocity.X -= num189;
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
                    }
                }

                // Ground
                else
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, vector) < CalamityGlobalNPC.CatchUpDistance350Tiles)
                            Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<DoGExtremeGravity>(), 2);
                    }

                    calamityGlobalNPC.newAI[2] += 1f;

                    // Enrage
                    if (increaseSpeedMore)
                        groundPhaseTurnSpeed *= 4f;
                    else if (increaseSpeed)
                        groundPhaseTurnSpeed *= 2f;

                    if (!flies)
                    {
                        for (int num952 = num180; num952 < num181; num952++)
                        {
                            for (int num953 = num182; num953 < num183; num953++)
                            {
                                if (Main.tile[num952, num953] != null && ((Main.tile[num952, num953].HasUnactuatedTile && (Main.tileSolid[Main.tile[num952, num953].TileType] || (Main.tileSolidTop[Main.tile[num952, num953].TileType] && Main.tile[num952, num953].TileFrameY == 0))) || Main.tile[num952, num953].LiquidAmount > 64))
                                {
                                    Vector2 vector105;
                                    vector105.X = num952 * 16;
                                    vector105.Y = num953 * 16;
                                    if (NPC.position.X + NPC.width > vector105.X && NPC.position.X < vector105.X + 16f && NPC.position.Y + NPC.height > vector105.Y && NPC.position.Y < vector105.Y + 16f)
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

                        int num954 = death ? 1125 : 1200;

                        if (expertMode)
                            num954 -= (int)(150f * (1f - lifeRatio));

                        if (num954 < 1050)
                            num954 = 1050;

                        bool flag95 = true;
                        if (NPC.position.Y > player.position.Y)
                        {
                            for (int num955 = 0; num955 < 255; num955++)
                            {
                                if (Main.player[num955].active)
                                {
                                    Rectangle rectangle13 = new Rectangle((int)Main.player[num955].position.X - 1000, (int)Main.player[num955].position.Y - 1000, 2000, num954);
                                    if (rectangle12.Intersects(rectangle13))
                                    {
                                        flag95 = false;
                                        break;
                                    }
                                }
                            }
                            if (flag95)
                                flies = true;
                        }
                    }
                    else
                        NPC.localAI[1] = 0f;

                    float num189 = groundPhaseTurnSpeed;
                    Vector2 vector18 = NPC.Center;
                    float num191 = player.Center.X;
                    float num192 = player.Center.Y;
                    num191 = (int)(num191 / 16f) * 16;
                    num192 = (int)(num192 / 16f) * 16;
                    vector18.X = (int)(vector18.X / 16f) * 16;
                    vector18.Y = (int)(vector18.Y / 16f) * 16;
                    num191 -= vector18.X;
                    num192 -= vector18.Y;

                    if (!flies)
                    {
                        NPC.velocity.Y += groundPhaseTurnSpeed;
                        if (NPC.velocity.Y > fallSpeed)
                            NPC.velocity.Y = fallSpeed;

                        if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < fallSpeed * 2.2)
                        {
                            if (NPC.velocity.X < 0f)
                                NPC.velocity.X -= num189 * 1.1f;
                            else
                                NPC.velocity.X += num189 * 1.1f;
                        }
                        else if (NPC.velocity.Y == fallSpeed)
                        {
                            if (NPC.velocity.X < num191)
                                NPC.velocity.X += num189;
                            else if (NPC.velocity.X > num191)
                                NPC.velocity.X -= num189;
                        }
                        else if (NPC.velocity.Y > 4f)
                        {
                            if (NPC.velocity.X < 0f)
                                NPC.velocity.X += num189 * 0.9f;
                            else
                                NPC.velocity.X -= num189 * 0.9f;
                        }
                    }
                    else
                    {
                        double maximumSpeed1 = malice ? 0.52 : death ? 0.46 : 0.4;
                        double maximumSpeed2 = malice ? 1.25 : death ? 1.125 : 1D;

                        if (expertMode)
                        {
                            maximumSpeed1 += 0.1f * (1f - lifeRatio);
                            maximumSpeed2 += 0.2f * (1f - lifeRatio);
                        }

                        float num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
                        float num25 = Math.Abs(num191);
                        float num26 = Math.Abs(num192);
                        float num27 = fallSpeed / num193;
                        num191 *= num27;
                        num192 *= num27;

                        if (((NPC.velocity.X > 0f && num191 > 0f) || (NPC.velocity.X < 0f && num191 < 0f)) && ((NPC.velocity.Y > 0f && num192 > 0f) || (NPC.velocity.Y < 0f && num192 < 0f)))
                        {
                            if (NPC.velocity.X < num191)
                                NPC.velocity.X += groundPhaseTurnSpeed * 1.5f;
                            else if (NPC.velocity.X > num191)
                                NPC.velocity.X -= groundPhaseTurnSpeed * 1.5f;

                            if (NPC.velocity.Y < num192)
                                NPC.velocity.Y += groundPhaseTurnSpeed * 1.5f;
                            else if (NPC.velocity.Y > num192)
                                NPC.velocity.Y -= groundPhaseTurnSpeed * 1.5f;
                        }

                        if ((NPC.velocity.X > 0f && num191 > 0f) || (NPC.velocity.X < 0f && num191 < 0f) || (NPC.velocity.Y > 0f && num192 > 0f) || (NPC.velocity.Y < 0f && num192 < 0f))
                        {
                            if (NPC.velocity.X < num191)
                                NPC.velocity.X += groundPhaseTurnSpeed;
                            else if (NPC.velocity.X > num191)
                                NPC.velocity.X -= groundPhaseTurnSpeed;

                            if (NPC.velocity.Y < num192)
                                NPC.velocity.Y += groundPhaseTurnSpeed;
                            else if (NPC.velocity.Y > num192)
                                NPC.velocity.Y -= groundPhaseTurnSpeed;

                            if (Math.Abs(num192) < fallSpeed * maximumSpeed1 && ((NPC.velocity.X > 0f && num191 < 0f) || (NPC.velocity.X < 0f && num191 > 0f)))
                            {
                                if (NPC.velocity.Y > 0f)
                                    NPC.velocity.Y += groundPhaseTurnSpeed * 2f;
                                else
                                    NPC.velocity.Y -= groundPhaseTurnSpeed * 2f;
                            }

                            if (Math.Abs(num191) < fallSpeed * maximumSpeed1 && ((NPC.velocity.Y > 0f && num192 < 0f) || (NPC.velocity.Y < 0f && num192 > 0f)))
                            {
                                if (NPC.velocity.X > 0f)
                                    NPC.velocity.X += groundPhaseTurnSpeed * 2f;
                                else
                                    NPC.velocity.X -= groundPhaseTurnSpeed * 2f;
                            }
                        }
                        else if (num25 > num26)
                        {
                            if (NPC.velocity.X < num191)
                                NPC.velocity.X += groundPhaseTurnSpeed * 1.1f;
                            else if (NPC.velocity.X > num191)
                                NPC.velocity.X -= groundPhaseTurnSpeed * 1.1f;

                            if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < fallSpeed * maximumSpeed2)
                            {
                                if (NPC.velocity.Y > 0f)
                                    NPC.velocity.Y += groundPhaseTurnSpeed;
                                else
                                    NPC.velocity.Y -= groundPhaseTurnSpeed;
                            }
                        }
                        else
                        {
                            if (NPC.velocity.Y < num192)
                                NPC.velocity.Y += groundPhaseTurnSpeed * 1.1f;
                            else if (NPC.velocity.Y > num192)
                                NPC.velocity.Y -= groundPhaseTurnSpeed * 1.1f;

                            if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < fallSpeed * maximumSpeed2)
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
                            NPC.netUpdate = true;

                        NPC.localAI[0] = 1f;
                    }
                    else
                    {
                        if (NPC.localAI[0] != 0f)
                            NPC.netUpdate = true;

                        NPC.localAI[0] = 0f;
                    }

                    if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
                        NPC.netUpdate = true;

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

            if (NPC.life > Main.npc[(int)NPC.ai[0]].life)
                NPC.life = Main.npc[(int)NPC.ai[0]].life;
        }

        private void SpawnTeleportLocation(Player player, bool phase2Transition = false)
        {
            if (teleportTimer > 0 || player.dead || !player.active)
                return;

            if (!phase2Transition)
                teleportTimer = (CalamityWorld.death || BossRushEvent.BossRushActive) ? TimeBeforeTeleport_Death : CalamityWorld.revenge ? TimeBeforeTeleport_Revengeance : Main.expertMode ? TimeBeforeTeleport_Expert : TimeBeforeTeleport_Normal;
            
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int randomRange = 48;
                float distance = 500f;
                Vector2 targetVector = player.Center + player.velocity.SafeNormalize(Vector2.UnitX) * distance + new Vector2(Main.rand.Next(-randomRange, randomRange + 1), Main.rand.Next(-randomRange, randomRange + 1));
                SoundEngine.PlaySound(SoundID.Item109, player.Center);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), targetVector, Vector2.Zero, ModContent.ProjectileType<DoGTeleportRift>(), 0, 0f, Main.myPlayer, NPC.whoAmI);
            }
        }

        private void Teleport(Player player, bool malice, bool death, bool revenge, bool expertMode, bool phase5)
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
                        int totalProjectiles = malice ? 18 : 12;
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
            float chargeVelocity = malice ? 30f : death ? 26f : revenge ? 24f : expertMode ? 22f : 20f;
            float maxChargeDistance = 1600f;
            postTeleportTimer = (int)Math.Round(maxChargeDistance / chargeVelocity);
            AwaitingPhase2Teleport = false;
            NPC.Opacity = 1f - (postTeleportTimer / 255f);
            NPC.velocity = Vector2.Normalize(player.Center + player.velocity * 40f - NPC.Center) * chargeVelocity;
            NPC.netUpdate = true;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && (Main.npc[i].type == ModContent.NPCType<DevourerofGodsBody>() || Main.npc[i].type == ModContent.NPCType<DevourerofGodsTail>()))
                {
                    Main.npc[i].position = newPosition;

                    if (Main.npc[i].type == ModContent.NPCType<DevourerofGodsTail>())
                        ((DevourerofGodsTail)Main.npc[i].ModNPC).setInvulTime(720);

                    Main.npc[i].netUpdate = true;
                }
            }

            SoundEngine.PlaySound(AttackSound, player.Center);
        }

        public void DoDeathAnimation()
        {
            // Play a sound at the start.
            if (DeathAnimationTimer == 1f)
            {
                SoundEngine.PlaySound(AttackSound with { Volume = AttackSound.Volume * 1.6f}, NPC.Center);
            }

            // Close the health bar, fade in, and stop doing contact damage.
            NPC.Calamity().CanHaveBossHealthBar = false;
            NPC.Opacity = MathHelper.Clamp(NPC.Opacity + 0.1f, 0f, 1f);
            NPC.dontTakeDamage = true;
            NPC.damage = 0;

            void destroySegment(int index, ref int destroyedSegments)
            {
                if (Main.rand.NextBool(5))
                    SoundEngine.PlaySound(SoundID.Item94, NPC.Center);

                List<int> segments = new List<int>()
                {
                    ModContent.NPCType<DevourerofGodsBody>(),
                    ModContent.NPCType<DevourerofGodsTail>()
                };
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (segments.Contains(Main.npc[i].type) && Main.npc[i].active &&
                        (Main.npc[i].type == segments[1] || Main.npc[i].ModNPC<DevourerofGodsBody>().SegmentIndex == index))
                    {
                        for (int j = 0; j < 20; j++)
                        {
                            Dust cosmicBurst = Dust.NewDustPerfect(Main.npc[i].Center + Main.rand.NextVector2Circular(25f, 25f), 234);
                            cosmicBurst.scale = 1.7f;
                            cosmicBurst.velocity = Main.rand.NextVector2Circular(9f, 9f);
                            cosmicBurst.noGravity = true;
                        }

                        Main.npc[i].life = 0;
                        Main.npc[i].HitEffect();
                        Main.npc[i].active = false;
                        Main.npc[i].netUpdate = true;
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
                    SoundEngine.PlaySound(AttackSound with { Volume = AttackSound.Volume * 1.6f}, NPC.Center);

                    for (int i = 0; i < 3; i++)
                    {
                        SoundEngine.PlaySound(TeslaCannon.FireSound with { Volume = TeslaCannon.FireSound.Volume * 1.4f, Pitch = TeslaCannon.FireSound.Pitch -MathHelper.Lerp(0.1f, 0.4f, i / 3f) }, NPC.Center);
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
            }
            DeathAnimationTimer++;
        }

        private Vector2 GetRiftLocation(bool spawnDust)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].type == ModContent.ProjectileType<DoGTeleportRift>())
                {
                    if (!spawnDust)
                        Main.projectile[i].ai[0] = -1f;

                    Main.projectile[i].Kill();
                    return Main.projectile[i].Center;
                }
            }
            return default;
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
            Texture2D texture2D15 = useOtherTextures ? ModContent.Request<Texture2D>("CalamityMod/NPCs/DevourerofGods/DevourerofGodsHeadS").Value : TextureAssets.Npc[NPC.type].Value;
            Vector2 vector11 = new Vector2(texture2D15.Width / 2, texture2D15.Height / 2);
            if (NPC.IsABestiaryIconDummy)
                NPC.frame = texture2D15.Frame();

            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            if (!NPC.dontTakeDamage)
            {
                texture2D15 = useOtherTextures ? ModContent.Request<Texture2D>("CalamityMod/NPCs/DevourerofGods/DevourerofGodsHeadSGlow").Value : ModContent.Request<Texture2D>("CalamityMod/NPCs/DevourerofGods/DevourerofGodsHeadGlow").Value;
                Color color37 = Color.Lerp(Color.White, Color.Fuchsia, 0.5f);

                spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

                texture2D15 = useOtherTextures ? ModContent.Request<Texture2D>("CalamityMod/NPCs/DevourerofGods/DevourerofGodsHeadSGlow2").Value : ModContent.Request<Texture2D>("CalamityMod/NPCs/DevourerofGods/DevourerofGodsHeadGlow2").Value;
                color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);

                spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
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
                string key = "Mods.CalamityMod.DoGBossText";
                Color messageColor = Color.Cyan;
                string key2 = "Mods.CalamityMod.DoGBossText2";
                Color messageColor2 = Color.Orange;
                string key3 = "Mods.CalamityMod.DargonBossText";

                CalamityUtils.DisplayLocalizedText(key, messageColor);
                CalamityUtils.DisplayLocalizedText(key2, messageColor2);
                CalamityUtils.DisplayLocalizedText(key3, messageColor2);
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
            npcLoot.Add(DropHelper.PerPlayer(ModContent.ItemType<OmegaHealingPotion>(), 1, 5, 15));

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

                // Materials
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<CosmiliteBar>(), 1, 25, 35));
                normalOnly.Add(ModContent.ItemType<CosmiliteBrick>(), 1, 150, 250);

                // Equipment
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<NebulousCore>()));
            }

            // Trophy (always directly from boss, never in bag)
            npcLoot.Add(ModContent.ItemType<DevourerofGodsTrophy>(), 10);

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedDoG, ModContent.ItemType<KnowledgeDevourerofGods>());
        }

        // Can only hit the target if within certain distance
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;

            Rectangle targetHitbox = target.Hitbox;

            float dist1 = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= (Phase2Started ? 80f : 55f) && (NPC.Opacity >= 1f || postTeleportTimer > 0);
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (!Dying && (damage * (crit ? 2D : 1D)) >= NPC.life)
            {
                damage = 0D;
                CheckDead();
                return false;
            }

            return true;
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

        public override bool CheckDead()
        {
            if (!Dying)
            {
                Dying = true;
                NPC.life = 1;
                NPC.dontTakeDamage = true;
                NPC.active = true;
                NPC.netUpdate = true;
            }
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 8;
                SoundEngine.PlaySound(CommonCalamitySounds.OtherwordlyHitSound, NPC.Center);
            }
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("DoGS").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("DoGS2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("DoGS5").Type, 1f);
                }
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 50;
                NPC.height = 50;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int num621 = 0; num621 < 15; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 30; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossLifeScale);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300, true);
            player.AddBuff(ModContent.BuffType<WhisperingDeath>(), 600, true);

            if (player.Calamity().dogTextCooldown <= 0)
            {
                string text = Utils.SelectRandom(Main.rand, new string[]
                {
                    "Mods.CalamityMod.EdgyBossText3",
                    "Mods.CalamityMod.EdgyBossText4",
                    "Mods.CalamityMod.EdgyBossText5",
                    "Mods.CalamityMod.EdgyBossText6",
                    "Mods.CalamityMod.EdgyBossText7"
                });
                Color messageColor = Color.Cyan;
                Rectangle location = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);
                CombatText.NewText(location, messageColor, Language.GetTextValue(text), true);
                player.Calamity().dogTextCooldown = 60;
            }
        }
    }
}
