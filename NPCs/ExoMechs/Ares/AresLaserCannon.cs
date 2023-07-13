using CalamityMod.Events;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Sounds;
using CalamityMod.Items.Tools;
using ReLogic.Utilities;

namespace CalamityMod.NPCs.ExoMechs.Ares
{
    public class AresLaserCannon : ModNPC
    {
        public enum Phase
        {
            Nothing = 0,
            Deathray = 1
        }

        public float AIState
        {
            get => NPC.Calamity().newAI[0];
            set => NPC.Calamity().newAI[0] = value;
        }

        public ThanatosSmokeParticleSet SmokeDrawer = new ThanatosSmokeParticleSet(-1, 3, 0f, 16f, 1.5f);
        public AresCannonChargeParticleSet EnergyDrawer = new AresCannonChargeParticleSet(-1, 15, 40f, Color.OrangeRed);

        public Vector2 CoreSpritePosition => NPC.Center + NPC.spriteDirection * NPC.rotation.ToRotationVector2() * 30f + (NPC.rotation + MathHelper.PiOver2).ToRotationVector2() * 15f;

        // Number of frames on the X and Y axis
        public const int maxFramesX = 6;
        public const int maxFramesY = 8;

        // Counters for frames on the X and Y axis
        public int frameX = 0;
        public int frameY = 0;

        // Frame limit per animation, these are the specific frames where each animation ends
        public const int normalFrameLimit = 11;
        public const int firstStageDeathrayChargeFrameLimit = 23;
        public const int secondStageDeathrayChargeFrameLimit = 35;
        public const int finalStageDeathrayChargeFrameLimit = 47;

        // Default life ratio for the other mechs
        public const float defaultLifeRatio = 5f;

        // Total duration of the deathray telegraph
        public const float deathrayTelegraphDuration = 144f;

        // Total duration of the deathray
        public const float deathrayDuration = 60f;

        // This stores the sound slot of the telegraph sound it makes, so it may be properly updated in terms of position.
        public SlotId TelegraphSoundSlot;

        // This stores the sound slot of the deathray sound it makes, so it may be properly updated in terms of position.
        public SlotId DeathraySoundSlot;

        // Telegraph sound.
        public static readonly SoundStyle TelSound = new("CalamityMod/Sounds/Custom/ExoMechs/AresLaserArmCharge") { Volume = 1.1f };

        public static readonly SoundStyle LaserbeamShootSound = new("CalamityMod/Sounds/Custom/ExoMechs/AresLaserArmShoot") { Volume = 1.1f };

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            NPCID.Sets.TrailingMode[NPC.type] = 3;
            NPCID.Sets.TrailCacheLength[NPC.type] = NPC.oldPos.Length;
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 5f;
            NPC.damage = 100;
            NPC.width = 154;
            NPC.height = 90;
            NPC.defense = 80;
            NPC.DR_NERD(0.35f);
            NPC.LifeMaxNERB(1250000, 1495000, 650000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.Opacity = 0f;
            NPC.knockBackResist = 0f;
            NPC.canGhostHeal = false;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.hide = true;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(frameX);
            writer.Write(frameY);
            writer.Write(NPC.dontTakeDamage);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            frameX = reader.ReadInt32();
            frameY = reader.ReadInt32();
            NPC.dontTakeDamage = reader.ReadBoolean();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            NPC.frame = new Rectangle(NPC.width * frameX, NPC.height * frameY, NPC.width, NPC.height);

            if (CalamityGlobalNPC.draedonExoMechPrime < 0 || !Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.checkDead();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            // Difficulty modes
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            // Percent life remaining
            float lifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechPrime].lifeMax;

            // Check if the other exo mechs are alive
            int otherExoMechsAlive = 0;
            bool exoWormAlive = false;
            bool exoTwinsAlive = false;
            if (CalamityGlobalNPC.draedonExoMechWorm != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedonExoMechWorm].active)
                {
                    otherExoMechsAlive++;
                    exoWormAlive = true;
                }
            }
            if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].active)
                {
                    otherExoMechsAlive++;
                    exoTwinsAlive = true;
                }
            }

            // Used to nerf Ares if fighting alongside Artemis and Apollo, because otherwise it's too difficult
            bool nerfedAttacks = false;
            if (exoTwinsAlive)
                nerfedAttacks = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].Calamity().newAI[1] != (float)Apollo.Apollo.SecondaryPhase.PassiveAndImmune;

            // Phases
            bool berserk = lifeRatio < 0.4f || (otherExoMechsAlive == 0 && lifeRatio < 0.7f);
            bool lastMechAlive = berserk && otherExoMechsAlive == 0;

            // These are 5 by default to avoid triggering passive phases after the other mechs are dead
            float exoWormLifeRatio = defaultLifeRatio;
            float exoTwinsLifeRatio = defaultLifeRatio;
            if (exoWormAlive)
                exoWormLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechWorm].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechWorm].lifeMax;
            if (exoTwinsAlive)
                exoTwinsLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].lifeMax;

            // If Ares doesn't go berserk
            bool otherMechIsBerserk = exoWormLifeRatio < 0.4f || exoTwinsLifeRatio < 0.4f;

            // Whether Ares should be buffed while in berserk phase
            bool shouldGetBuffedByBerserkPhase = berserk && !otherMechIsBerserk;

            // Target variable
            Player player = Main.player[Main.npc[CalamityGlobalNPC.draedonExoMechPrime].target];

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            if (NPC.life > Main.npc[(int)NPC.ai[1]].life)
                NPC.life = Main.npc[(int)NPC.ai[1]].life;

            AresBody aresBody = Main.npc[(int)NPC.ai[2]].ModNPC<AresBody>();
            CalamityGlobalNPC calamityGlobalNPC_Body = Main.npc[(int)NPC.ai[2]].Calamity();

            // Passive phase check
            bool passivePhase = calamityGlobalNPC_Body.newAI[1] == (float)AresBody.SecondaryPhase.Passive;

            // Enrage check
            bool enraged = Main.npc[(int)NPC.ai[2]].localAI[1] == (float)AresBody.Enraged.Yes;

            // Adjust opacity
            bool invisiblePhase = calamityGlobalNPC_Body.newAI[1] == (float)AresBody.SecondaryPhase.PassiveAndImmune;
            NPC.dontTakeDamage = invisiblePhase || Main.npc[(int)NPC.ai[2]].dontTakeDamage;
            if (!invisiblePhase)
            {
                NPC.Opacity += 0.2f;
                if (NPC.Opacity > 1f)
                    NPC.Opacity = 1f;
            }
            else
            {
                NPC.Opacity -= 0.05f;
                if (NPC.Opacity < 0f)
                    NPC.Opacity = 0f;
            }

            // Variable to fire normal lasers
            bool fireNormalLasers = calamityGlobalNPC_Body.newAI[0] == (float)AresBody.Phase.Deathrays;

            // Default vector to fly to
            float offsetX = -560f;
            float offsetY = 0f;
            float offsetX2 = -540f;
            float offsetY2 = -540f;
            bool flyLeft = true;
            switch ((int)Main.npc[CalamityGlobalNPC.draedonExoMechPrime].ai[3])
            {
                case 0:
                case 3:
                case 4:
                    break;

                case 1:
                case 2:
                case 5:
                    offsetX *= -1f;
                    offsetX2 *= -1f;
                    offsetY2 *= -1f;
                    flyLeft = false;
                    break;
            }
            Vector2 destination = fireNormalLasers ? new Vector2(Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.X + offsetX2, Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.Y + offsetY2) : new Vector2(Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.X + offsetX, Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.Y + offsetY);

            // Rotate the cannon to look at the target while not firing the beam
            // Rotate the cannon to look in the direction it will fire only while it's charging or while it's firing
            // Rotation
            bool horizontalLaserSweep = calamityGlobalNPC.newAI[3] == 0f;
            float rateOfRotation = AIState == (int)Phase.Deathray ? 0.08f : 0.04f;
            float lookAtX = flyLeft ? 1000f : -1000f;
            Vector2 lookAt = AIState == (int)Phase.Deathray && !fireNormalLasers ? (horizontalLaserSweep ? new Vector2(NPC.Center.X, NPC.Center.Y + 1000f) : new Vector2(NPC.Center.X + lookAtX, NPC.Center.Y)) : player.Center;

            float rotation = (float)Math.Atan2(lookAt.Y - NPC.Center.Y, lookAt.X - NPC.Center.X);
            if (NPC.spriteDirection == 1)
                rotation += MathHelper.Pi;
            if (rotation < 0f)
                rotation += MathHelper.TwoPi;
            if (rotation > MathHelper.TwoPi)
                rotation -= MathHelper.TwoPi;

            NPC.rotation = NPC.rotation.AngleTowards(rotation, rateOfRotation);

            // Direction
            int direction = Math.Sign(((AIState == (int)Phase.Deathray && !horizontalLaserSweep) || fireNormalLasers ? lookAt.X : player.Center.X) - NPC.Center.X);
            if (direction != 0)
            {
                NPC.direction = direction;

                if (NPC.spriteDirection != -NPC.direction)
                    NPC.rotation += MathHelper.Pi;

                NPC.spriteDirection = -NPC.direction;
            }

            // Despawn if target is dead
            if (player.dead)
            {
                player = Main.player[Main.npc[CalamityGlobalNPC.draedonExoMechPrime].target];
                if (player.dead)
                {
                    AIState = (float)Phase.Nothing;
                    calamityGlobalNPC.newAI[1] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    NPC.dontTakeDamage = true;

                    NPC.velocity.Y -= 1f;
                    if ((double)NPC.position.Y < Main.topWorld + 16f)
                        NPC.velocity.Y -= 1f;

                    if ((double)NPC.position.Y < Main.topWorld + 16f)
                    {
                        for (int a = 0; a < Main.maxNPCs; a++)
                        {
                            if (Main.npc[a].type == NPC.type || Main.npc[a].type == ModContent.NPCType<Artemis.Artemis>() || Main.npc[a].type == ModContent.NPCType<AresBody>() ||
                                Main.npc[a].type == ModContent.NPCType<Apollo.Apollo>() || Main.npc[a].type == ModContent.NPCType<AresPlasmaFlamethrower>() ||
                                Main.npc[a].type == ModContent.NPCType<AresTeslaCannon>() || Main.npc[a].type == ModContent.NPCType<AresGaussNuke>() ||
                                Main.npc[a].type == ModContent.NPCType<ThanatosHead>() || Main.npc[a].type == ModContent.NPCType<ThanatosBody1>() ||
                                Main.npc[a].type == ModContent.NPCType<ThanatosBody2>() || Main.npc[a].type == ModContent.NPCType<ThanatosTail>())
                                Main.npc[a].active = false;
                        }
                    }

                    return;
                }
            }

            // Velocity and acceleration values
            float baseVelocityMult = (shouldGetBuffedByBerserkPhase ? 0.25f : 0f) + (bossRush ? 1.15f : death ? 1.1f : revenge ? 1.075f : expertMode ? 1.05f : 1f);
            float baseVelocity = (enraged ? 38f : 30f) * baseVelocityMult;
            baseVelocity *= 1f + Main.npc[(int)NPC.ai[2]].localAI[2];
            float baseAcceleration = shouldGetBuffedByBerserkPhase ? 1.25f : 1f;

            Vector2 distanceFromDestination = destination - NPC.Center;
            Vector2 desiredVelocity = Vector2.Normalize(distanceFromDestination) * baseVelocity;

            // Whether Ares Laser Arm should move to its spot or not
            float movementDistanceGateValue = 50f;
            bool moveToLocation = Vector2.Distance(NPC.Center, destination) > movementDistanceGateValue;

            // Gate values
            float deathrayPhaseGateValue = 450f;
            if (enraged)
                deathrayPhaseGateValue *= 0.1f;
            else if (lastMechAlive)
                deathrayPhaseGateValue *= 0.3f;
            else if (shouldGetBuffedByBerserkPhase)
                deathrayPhaseGateValue *= 0.5f;

            float deathrayPhaseVelocity = (nerfedAttacks ? 6f : passivePhase ? 9f : 12f) * baseVelocityMult;
            if (lastMechAlive)
                deathrayPhaseVelocity *= 1.2f;
            else if (shouldGetBuffedByBerserkPhase)
                deathrayPhaseVelocity *= 1.1f;

            // If Laser Cannon can fire normal lasers, cannot fire if too close to the target and in deathray spiral phase
            bool canFire = Vector2.Distance(NPC.Center, player.Center) > 320f || !fireNormalLasers;

            // Telegraph duration for deathray spiral
            float deathraySpiralTelegraphDuration = bossRush ? AresBody.deathrayTelegraphDuration_BossRush : death ? AresBody.deathrayTelegraphDuration_Death :
                revenge ? AresBody.deathrayTelegraphDuration_Rev : expertMode ? AresBody.deathrayTelegraphDuration_Expert : AresBody.deathrayTelegraphDuration_Normal;

            // Variable to disable deathray firing
            bool doNotFire = calamityGlobalNPC_Body.newAI[1] == (float)AresBody.SecondaryPhase.PassiveAndImmune ||
                (calamityGlobalNPC_Body.newAI[2] >= deathraySpiralTelegraphDuration + AresBody.deathrayDuration - 10 && fireNormalLasers) ||
                (calamityGlobalNPC_Body.newAI[3] == 0f && fireNormalLasers);

            if (doNotFire)
            {
                AIState = (float)Phase.Nothing;
                calamityGlobalNPC.newAI[1] = 0f;
                calamityGlobalNPC.newAI[2] = 0f;
            }

            // Emit steam while enraged
            SmokeDrawer.ParticleSpawnRate = 9999999;
            if (enraged)
            {
                SmokeDrawer.ParticleSpawnRate = AresBody.ventCloudSpawnRate;
                SmokeDrawer.BaseMoveRotation = NPC.rotation + MathHelper.Pi;
                SmokeDrawer.SpawnAreaCompactness = 40f;

                // Increase DR during enrage
                NPC.Calamity().DR = 0.85f;
            }
            else
                NPC.Calamity().DR = 0.35f;

            SmokeDrawer.Update();

            EnergyDrawer.ParticleSpawnRate = 9999999;

            // Attacking phases
            switch ((int)AIState)
            {
                // Do nothing, rotate to aim at the target and fly in place
                case (int)Phase.Nothing:

                    // Smooth movement towards the location Ares Laser Cannon is meant to be at
                    CalamityUtils.SmoothMovement(NPC, movementDistanceGateValue, distanceFromDestination, baseVelocity, 0f, false);

                    calamityGlobalNPC.newAI[1] += 1f;
                    if (calamityGlobalNPC.newAI[1] >= deathrayPhaseGateValue)
                    {
                        AIState = (float)Phase.Deathray;
                        calamityGlobalNPC.newAI[1] = 0f;
                    }

                    break;

                // Move close to target, reduce velocity when close enough, create telegraph beams, fire deathrays
                case (int)Phase.Deathray:

                    calamityGlobalNPC.newAI[2] += 1f;
                    if (calamityGlobalNPC.newAI[2] < deathrayTelegraphDuration)
                    {
                        // Play a charge up sound so that the player knows when it's about to fire the deathray
                        if (calamityGlobalNPC.newAI[2] == 1 && !fireNormalLasers)
                            TelegraphSoundSlot = SoundEngine.PlaySound(TelSound, NPC.Center);

                        // Smooth movement towards the location Ares Laser Cannon is meant to be at
                        CalamityUtils.SmoothMovement(NPC, movementDistanceGateValue, distanceFromDestination, baseVelocity, 0f, false);

                        // Set frames to deathray charge up frames, which begin on frame 12
                        if (calamityGlobalNPC.newAI[2] == 1f)
                        {
                            // Reset the frame counter
                            NPC.frameCounter = 0D;

                            // X = 1 sets to frame 8
                            frameX = 1;

                            // Y = 4 sets to frame 12
                            frameY = 4;
                        }

                        EnergyDrawer.ParticleSpawnRate = AresBody.telegraphParticlesSpawnRate;
                        EnergyDrawer.SpawnAreaCompactness = 100f;
                        EnergyDrawer.chargeProgress = calamityGlobalNPC.newAI[2] / deathrayTelegraphDuration;
                    }
                    else
                    {
                        // Fire regular Thanatos lasers if Ares is in deathray phase, otherwise, fire deathray
                        if (fireNormalLasers)
                        {
                            // Smooth movement towards the location Ares Laser Cannon is meant to be at
                            CalamityUtils.SmoothMovement(NPC, movementDistanceGateValue, distanceFromDestination, baseVelocity, 0f, false);

                            // Fire Thanatos lasers
                            int numLasers = enraged ? 9 : lastMechAlive ? 3 : 2;
                            float divisor = deathrayDuration / numLasers;

                            if (calamityGlobalNPC.newAI[2] % divisor == 0f && canFire)
                            {
                                SoundEngine.PlaySound(CommonCalamitySounds.ExoLaserShootSound, NPC.Center);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int type = ModContent.ProjectileType<ThanatosLaser>();
                                    int damage = NPC.GetProjectileDamage(type);
                                    Vector2 laserVelocity = Vector2.Normalize(player.Center - NPC.Center);
                                    Vector2 offset = laserVelocity * 70f + Vector2.UnitY * 16f;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset, player.Center, type, damage, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                                }
                            }
                        }
                        else
                        {
                            // Two possible variants: 1 - Horizontal, 2 - Vertical

                            // Movement while firing deathray
                            if (horizontalLaserSweep)
                                desiredVelocity.X = 0f;
                            else
                                desiredVelocity.Y = 0f;

                            NPC.SimpleFlyMovement(desiredVelocity, baseAcceleration);
                            float velocityX = flyLeft ? deathrayPhaseVelocity : -deathrayPhaseVelocity;
                            NPC.velocity = horizontalLaserSweep ? new Vector2(velocityX, NPC.velocity.Y) : new Vector2(NPC.velocity.X, deathrayPhaseVelocity * 0.75f);

                            NPC.netUpdate = true;
                            NPC.netSpam -= 5;

                            // Fire deathray
                            if (calamityGlobalNPC.newAI[2] == deathrayTelegraphDuration)
                            {
                                // Play the deathray sound.
                                DeathraySoundSlot = SoundEngine.PlaySound(LaserbeamShootSound, NPC.Center);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int type = ModContent.ProjectileType<AresLaserBeamStart>();
                                    int damage = NPC.GetProjectileDamage(type);
                                    float offset = 84f;
                                    float offset2 = 16f;
                                    Vector2 source = horizontalLaserSweep ? new Vector2(NPC.Center.X - offset2 * NPC.direction, NPC.Center.Y + offset) : new Vector2(NPC.Center.X + offset * NPC.direction, NPC.Center.Y + offset2);
                                    Vector2 laserVelocity = Vector2.Normalize(lookAt - source);
                                    if (laserVelocity.HasNaNs())
                                        laserVelocity = -Vector2.UnitY;

                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), source, laserVelocity, type, damage, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                                }
                            }
                        }
                    }

                    if (calamityGlobalNPC.newAI[2] % (float)Math.Floor(deathrayTelegraphDuration / 5f) == (float)Math.Floor(deathrayTelegraphDuration / 5f) - 1 && calamityGlobalNPC.newAI[2] <= deathrayTelegraphDuration)
                    {
                        float pulseCounter = (float)Math.Floor(calamityGlobalNPC.newAI[2] / (deathrayTelegraphDuration / 5f)) + 1;
                        EnergyDrawer.AddPulse(pulseCounter);
                    }

                    if (calamityGlobalNPC.newAI[2] >= deathrayTelegraphDuration + deathrayDuration)
                    {
                        // Reset
                        AIState = (float)Phase.Nothing;
                        calamityGlobalNPC.newAI[2] = 0f;

                        // Change deathray sweep type for next deathray phase
                        calamityGlobalNPC.newAI[3] += 1f;
                        if (calamityGlobalNPC.newAI[3] > 1f)
                            calamityGlobalNPC.newAI[3] = 0f;
                    }

                    break;
            }

            EnergyDrawer.Update();

            // Update the deathray and telegraph sound if they're being played.
            if (SoundEngine.TryGetActiveSound(DeathraySoundSlot, out var deathraySound) && deathraySound.IsPlaying)
                deathraySound.Position = NPC.Center;
            
            // Immediately stop the telegraph sound if Ares just begun transitioning to his laserbeam attack, since that automatically resets all impending cannon shots.
            if (SoundEngine.TryGetActiveSound(TelegraphSoundSlot, out var telSound) && telSound.IsPlaying)
            {
                telSound.Position = NPC.Center;
                if (aresBody.AIState == (int)AresBody.Phase.Deathrays && calamityGlobalNPC_Body.newAI[2] <= 10f)
                    telSound.Stop();
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override void FindFrame(int frameHeight)
        {
            // Use telegraph frames when using deathrays
            NPC.frameCounter += 1D;
            if (AIState == (float)Phase.Nothing)
            {
                if (NPC.frameCounter >= 6D)
                {
                    // Reset frame counter
                    NPC.frameCounter = 0D;

                    // Increment the Y frame
                    frameY++;

                    // Reset the Y frame if greater than 8
                    if (frameY == maxFramesY)
                    {
                        frameX++;
                        frameY = 0;
                    }

                    // Reset the frames
                    if ((frameX * maxFramesY) + frameY > normalFrameLimit)
                        frameX = frameY = 0;
                }
            }
            else
            {
                if (NPC.frameCounter >= 6D)
                {
                    // Reset frame counter
                    NPC.frameCounter = 0D;

                    // Increment the Y frame
                    frameY++;

                    // Reset the Y frame if greater than 8
                    if (frameY == maxFramesY)
                    {
                        frameX++;
                        frameY = 0;
                    }

                    // Reset the frames to frame 36, the start of the deathray firing animation loop
                    if ((frameX * maxFramesY) + frameY > finalStageDeathrayChargeFrameLimit)
                        frameX = frameY = 4;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Draw the enrage smoke behind Ares
            SmokeDrawer.DrawSet(NPC.Center);

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Rectangle frame = new Rectangle(NPC.width * frameX, NPC.height * frameY, NPC.width, NPC.height);
            Vector2 vector = new Vector2(NPC.width / 2, NPC.height / 2);
            Color afterimageBaseColor = Main.npc[(int)NPC.ai[2]].localAI[1] == (float)AresBody.Enraged.Yes ? Color.Red : Color.White;
            int numAfterimages = 5;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 1; i < numAfterimages; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, afterimageBaseColor, 0.5f);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (numAfterimages - i) / 15f;
                    Vector2 afterimageCenter = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    afterimageCenter -= new Vector2(texture.Width, texture.Height) / new Vector2(maxFramesX, maxFramesY) * NPC.scale / 2f;
                    afterimageCenter += vector * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture, afterimageCenter, NPC.frame, afterimageColor, NPC.oldRot[i], vector, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 center = NPC.Center - screenPos;

            // Draw an outline to the arm when it charges up
            if ((NPC.Calamity().newAI[2] < deathrayTelegraphDuration) && AIState == (float)Phase.Deathray)
            {
                CalamityUtils.EnterShaderRegion(spriteBatch);
                Color outlineColor = Color.Lerp(Color.OrangeRed, Color.White, NPC.Calamity().newAI[2] / deathrayTelegraphDuration);
                Vector3 outlineHSL = Main.rgbToHsl(outlineColor); // BasicTint uses the opposite hue i guess? Or something is fucked with the way shaders get their colors. Anyways, we invert it
                float outlineThickness = MathHelper.Clamp(NPC.Calamity().newAI[2] / deathrayTelegraphDuration * 4f, 0f, 3f);

                GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(1f);
                GameShaders.Misc["CalamityMod:BasicTint"].UseColor(Main.hslToRgb(1 - outlineHSL.X, outlineHSL.Y, outlineHSL.Z));
                GameShaders.Misc["CalamityMod:BasicTint"].Apply();

                for (float i = 0; i < 1; i += 0.125f)
                    spriteBatch.Draw(texture, center + (i * MathHelper.TwoPi + NPC.rotation).ToRotationVector2() * outlineThickness, frame, outlineColor, NPC.rotation, vector, NPC.scale, spriteEffects, 0f);

                CalamityUtils.ExitShaderRegion(spriteBatch);
            }

            spriteBatch.Draw(texture, center, frame, NPC.GetAlpha(drawColor), NPC.rotation, vector, NPC.scale, spriteEffects, 0f);

            Texture2D glowTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresLaserCannonGlow").Value;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 1; i < numAfterimages; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, afterimageBaseColor, 0.5f);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (numAfterimages - i) / 15f;
                    Vector2 afterimageCenter = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    afterimageCenter -= new Vector2(glowTexture.Width, glowTexture.Height) / new Vector2(maxFramesX, maxFramesY) * NPC.scale / 2f;
                    afterimageCenter += vector * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(glowTexture, afterimageCenter, NPC.frame, afterimageColor, NPC.oldRot[i], vector, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(glowTexture, center, frame, afterimageBaseColor * NPC.Opacity, NPC.rotation, vector, NPC.scale, spriteEffects, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            // Update the parameters
            // Draw a pulsing version of the cannon above the real one
            if ((NPC.Calamity().newAI[2] < deathrayTelegraphDuration) && AIState == (float)Phase.Deathray)
            {
                // Also draw a telegraph line
                Texture2D lineTex = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomLine").Value;
                Color outlineColor = Color.Lerp(Color.OrangeRed, Color.White, NPC.Calamity().newAI[2] / deathrayTelegraphDuration);
                spriteBatch.Draw(lineTex, CoreSpritePosition - NPC.rotation.ToRotationVector2() * NPC.spriteDirection * 104 - screenPos, null, outlineColor, NPC.rotation - MathHelper.PiOver2 * NPC.spriteDirection, new Vector2(lineTex.Width / 2f, lineTex.Height), new Vector2(1f * NPC.Calamity().newAI[2] / deathrayTelegraphDuration, 4200f), spriteEffects, 0f);

                float pulseRatio = (NPC.Calamity().newAI[2] % (deathrayTelegraphDuration / 5f)) / (deathrayTelegraphDuration / 5f);
                float pulseSize = MathHelper.Lerp(0.1f, 0.6f, (float)Math.Floor(NPC.Calamity().newAI[2] / (deathrayTelegraphDuration / 5f)) / 4f);
                float pulseOpacity = MathHelper.Clamp((float)Math.Floor(NPC.Calamity().newAI[2] / (deathrayTelegraphDuration / 5f)) * 0.3f, 1f, 2f);
                spriteBatch.Draw(texture, center, frame, Color.OrangeRed * MathHelper.Lerp(1f, 0f, pulseRatio) * pulseOpacity, NPC.rotation, vector, NPC.scale + pulseRatio * pulseSize, spriteEffects, 0f);

                // Draw the bloom
                EnergyDrawer.DrawBloom(CoreSpritePosition);
            }

            EnergyDrawer.DrawPulses(CoreSpritePosition);
            EnergyDrawer.DrawSet(CoreSpritePosition);

            // Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCProjectiles.Add(index);
        }

        public override void ModifyTypeName(ref string typeName)
        {
            int index = CalamityGlobalNPC.draedonExoMechPrime;

            if (index < 0 || index >= Main.maxNPCs || Main.npc[index] is null)
                return;

            if (Main.npc[index].ModNPC<AresBody>().exoMechdusa)
            {
                typeName = this.GetLocalizedValue("HekateName");
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1f);

            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 3;
                SoundEngine.PlaySound(CommonCalamitySounds.ExoHitSound, NPC.Center);
            }

            if (NPC.life <= 0)
            {
                for (int num193 = 0; num193 < 2; num193++)
                {
                    Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                }
                for (int num194 = 0; num194 < 20; num194++)
                {
                    int num195 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 107, 0f, 0f, 0, new Color(0, 255, 255), 2.5f);
                    Main.dust[num195].noGravity = true;
                    Main.dust[num195].velocity *= 3f;
                    num195 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                    Main.dust[num195].velocity *= 2f;
                    Main.dust[num195].noGravity = true;
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AresLaserCannon1").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AresLaserCannon2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AresHandBase1").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AresHandBase2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AresHandBase3").Type, 1f);
                }
            }
        }

        public override bool CheckActive() => false;

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * 0.8f);
        }
    }
}
