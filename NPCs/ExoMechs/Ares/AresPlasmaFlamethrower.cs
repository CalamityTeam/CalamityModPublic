using CalamityMod.Events;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Sounds;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.ExoMechs.Ares
{
    public class AresPlasmaFlamethrower : ModNPC
    {
        public enum Phase
        {
            Nothing = 0,
            PlasmaBolts = 1
        }

        public float AIState
        {
            get => NPC.Calamity().newAI[0];
            set => NPC.Calamity().newAI[0] = value;
        }

        public ThanatosSmokeParticleSet SmokeDrawer = new ThanatosSmokeParticleSet(-1, 3, 0f, 16f, 1.5f);
        public AresCannonChargeParticleSet EnergyDrawer = new AresCannonChargeParticleSet(-1, 15, 40f, Color.GreenYellow);

        public Vector2 CoreSpritePosition => NPC.Center + NPC.spriteDirection * NPC.rotation.ToRotationVector2() * 30f + (NPC.rotation + MathHelper.PiOver2).ToRotationVector2() * 15f;

        // Number of frames on the X and Y axis
        public const int maxFramesX = 6;
        public const int maxFramesY = 8;

        // Counters for frames on the X and Y axis
        public int frameX = 0;
        public int frameY = 0;

        // Frame limit per animation, these are the specific frames where each animation ends
        public const int normalFrameLimit = 11;
        public const int firstStagePlasmaBoltChargeFrameLimit = 23;
        public const int secondStagePlasmaBoltChargeFrameLimit = 35;
        public const int finalStagePlasmaBoltChargeFrameLimit = 47;

        // Default life ratio for the other mechs
        public const float defaultLifeRatio = 5f;

        // Total duration of the plasma bolt telegraph
        public const float plasmaBoltTelegraphDuration = 144f;

        // Total duration of the plasma bolt firing phase
        public const float plasmaBoltDuration = 120f;

        // This stores the sound slot of the telegraph sound it makes, so it may be properly updated in terms of position.
        public SlotId TelegraphSoundSlot;

        // Telegraph sound.
        public static readonly SoundStyle TelSound = new("CalamityMod/Sounds/Custom/ExoMechs/AresPlasmaArmCharge");

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
            NPC.width = 152;
            NPC.height = 90;
            NPC.defense = 100;
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
            NPC.DeathSound = CommonCalamitySounds.ExoDeathSound;
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

            CalamityGlobalNPC.draedonExoMechPrimePlasmaCannon = NPC.whoAmI;

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

            // Predictiveness
            float predictionAmt = bossRush ? 20f : death ? 15f : revenge ? 13.75f : expertMode ? 12.5f : 10f;
            if (nerfedAttacks)
                predictionAmt *= 0.5f;
            if (passivePhase)
                predictionAmt *= 0.5f;

            Vector2 predictionVector = player.velocity * predictionAmt;
            Vector2 rotationVector = player.Center + predictionVector - NPC.Center;

            float projectileVelocity = passivePhase ? 9.6f : 12f;
            if (lastMechAlive)
                projectileVelocity *= 1.2f;
            else if (shouldGetBuffedByBerserkPhase)
                projectileVelocity *= 1.1f;

            float rateOfRotation = AIState == (int)Phase.PlasmaBolts ? 0.08f : 0.04f;
            Vector2 lookAt = Vector2.Normalize(rotationVector) * projectileVelocity;

            float rotation = (float)Math.Atan2(lookAt.Y, lookAt.X);
            if (NPC.spriteDirection == 1)
                rotation += MathHelper.Pi;
            if (rotation < 0f)
                rotation += MathHelper.TwoPi;
            if (rotation > MathHelper.TwoPi)
                rotation -= MathHelper.TwoPi;

            NPC.rotation = NPC.rotation.AngleTowards(rotation, rateOfRotation);

            // Direction
            int direction = Math.Sign(player.Center.X - NPC.Center.X);
            if (direction != 0)
            {
                NPC.direction = direction;

                if (NPC.spriteDirection != -NPC.direction)
                    NPC.rotation += MathHelper.Pi;

                NPC.spriteDirection = -NPC.direction;
            }

            // Gate values
            bool fireMoreBolts = calamityGlobalNPC_Body.newAI[0] == (float)AresBody.Phase.Deathrays;
            float plasmaBoltPhaseGateValue = fireMoreBolts ? 120f : 270f;
            if (enraged)
                plasmaBoltPhaseGateValue *= 0.1f;
            else if (lastMechAlive)
                plasmaBoltPhaseGateValue *= 0.4f;
            else if (shouldGetBuffedByBerserkPhase)
                plasmaBoltPhaseGateValue *= 0.7f;

            // Set attack timer to this when despawning or when Ares is coming out of deathray phase
            float setTimerTo = (int)(plasmaBoltPhaseGateValue * 0.95f) - 1;

            // Despawn if target is dead
            if (player.dead)
            {
                player = Main.player[Main.npc[CalamityGlobalNPC.draedonExoMechPrime].target];
                if (player.dead)
                {
                    AIState = (float)Phase.Nothing;
                    calamityGlobalNPC.newAI[1] = setTimerTo;
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
                                Main.npc[a].type == ModContent.NPCType<AresLaserCannon>() || Main.npc[a].type == ModContent.NPCType<Apollo.Apollo>() ||
                                Main.npc[a].type == ModContent.NPCType<AresTeslaCannon>() || Main.npc[a].type == ModContent.NPCType<AresGaussNuke>() ||
                                Main.npc[a].type == ModContent.NPCType<ThanatosHead>() || Main.npc[a].type == ModContent.NPCType<ThanatosBody1>() ||
                                Main.npc[a].type == ModContent.NPCType<ThanatosBody2>() || Main.npc[a].type == ModContent.NPCType<ThanatosTail>())
                                Main.npc[a].active = false;
                        }
                    }

                    return;
                }
            }

            // Default vector to fly to
            float offsetX = 375f;
            float offsetY = 160f;
            float offsetX2 = 540f;
            float offsetY2 = -540f;
            switch ((int)Main.npc[CalamityGlobalNPC.draedonExoMechPrime].ai[3])
            {
                case 0:
                case 1:
                case 4:
                case 5:
                    break;

                case 2:
                case 3:
                    offsetX *= -1f;
                    offsetX2 *= -1f;
                    offsetY2 *= -1f;
                    break;
            }
            Vector2 destination = calamityGlobalNPC_Body.newAI[0] == (float)AresBody.Phase.Deathrays ? new Vector2(Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.X + offsetX2, Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.Y + offsetY2) : new Vector2(Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.X + offsetX, Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.Y + offsetY);

            // Velocity and acceleration values
            float baseVelocityMult = (shouldGetBuffedByBerserkPhase ? 0.25f : 0f) + (bossRush ? 1.15f : death ? 1.1f : revenge ? 1.075f : expertMode ? 1.05f : 1f);
            float baseVelocity = (enraged ? 38f : 30f) * baseVelocityMult;
            baseVelocity *= 1f + Main.npc[(int)NPC.ai[2]].localAI[2];

            Vector2 distanceFromDestination = destination - NPC.Center;

            // Distance where Ares Plasma Arm stops moving
            float movementDistanceGateValue = 50f;

            // Make plasma bolts split less if in deathray spiral phase and not the last mech alive, but fire more if in deathray spiral phase
            bool boltsSplitLess = fireMoreBolts && !lastMechAlive;

            // If Plasma Cannon can fire projectiles, cannot fire if too close to the target and in deathray spiral phase
            bool canFire = Vector2.Distance(NPC.Center, player.Center) > 320f || calamityGlobalNPC_Body.newAI[0] != (float)AresBody.Phase.Deathrays;

            // Telegraph duration for deathray spiral
            float deathrayTelegraphDuration = bossRush ? AresBody.deathrayTelegraphDuration_BossRush : death ? AresBody.deathrayTelegraphDuration_Death :
                revenge ? AresBody.deathrayTelegraphDuration_Rev : expertMode ? AresBody.deathrayTelegraphDuration_Expert : AresBody.deathrayTelegraphDuration_Normal;

            // Variable to cancel plasma bolt firing
            bool doNotFire = calamityGlobalNPC_Body.newAI[1] == (float)AresBody.SecondaryPhase.PassiveAndImmune ||
                (calamityGlobalNPC_Body.newAI[2] >= deathrayTelegraphDuration + AresBody.deathrayDuration - 10 && calamityGlobalNPC_Body.newAI[0] == (float)AresBody.Phase.Deathrays) ||
                (calamityGlobalNPC_Body.newAI[3] == 0f && calamityGlobalNPC_Body.newAI[0] == (float)AresBody.Phase.Deathrays);

            if (doNotFire)
            {
                AIState = (float)Phase.Nothing;
                calamityGlobalNPC.newAI[1] = setTimerTo;
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
                // Do nothing and fly in place
                case (int)Phase.Nothing:

                    calamityGlobalNPC.newAI[1] += 1f;
                    if (calamityGlobalNPC.newAI[1] >= plasmaBoltPhaseGateValue)
                    {
                        AIState = (float)Phase.PlasmaBolts;
                        calamityGlobalNPC.newAI[1] = 0f;
                    }

                    break;

                // Fire plasma bolts in a double burst that burst into a halo of smaller bolts
                case (int)Phase.PlasmaBolts:

                    calamityGlobalNPC.newAI[2] += 1f;
                    if (calamityGlobalNPC.newAI[2] < plasmaBoltTelegraphDuration)
                    {
                        // Play a charge up sound so that the player knows when it's about to fire fireballs
                        if (calamityGlobalNPC.newAI[2] == 1)
                            TelegraphSoundSlot = SoundEngine.PlaySound(TelSound, NPC.Center);

                        // Set frames to plasma orb charge up frames, which begin on frame 12
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
                        EnergyDrawer.chargeProgress = calamityGlobalNPC.newAI[2] / plasmaBoltTelegraphDuration;
                    }
                    else if (calamityGlobalNPC.newAI[2] < plasmaBoltTelegraphDuration + plasmaBoltDuration)
                    {
                        // Fire plasma bolts
                        int numPlasmaBolts = lastMechAlive ? 3 : 2;
                        float divisor = plasmaBoltDuration / numPlasmaBolts;

                        if ((calamityGlobalNPC.newAI[2] - plasmaBoltTelegraphDuration) % divisor == 0f && canFire)
                        {
                            NPC.ai[3] += 1f;
                            SoundEngine.PlaySound(CommonCalamitySounds.ExoPlasmaShootSound, NPC.Center);
                            Vector2 plasmaBoltVelocity = Vector2.Normalize(rotationVector) * projectileVelocity;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = ModContent.ProjectileType<AresPlasmaFireball>();
                                int damage = NPC.GetProjectileDamage(type);
                                Vector2 offset = Vector2.Normalize(plasmaBoltVelocity) * 40f + Vector2.UnitY * 16f;

                                if (boltsSplitLess)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset, plasmaBoltVelocity, type, damage, 0f, Main.myPlayer, -1f);
                                else
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset, plasmaBoltVelocity, type, damage, 0f, Main.myPlayer, player.Center.X, player.Center.Y);
                            }

                            // Recoil
                            NPC.velocity -= plasmaBoltVelocity;
                        }
                    }

                    if (calamityGlobalNPC.newAI[2] % (float)Math.Floor(plasmaBoltTelegraphDuration / 5f) == (float)Math.Floor(plasmaBoltTelegraphDuration / 5f) - 1 && calamityGlobalNPC.newAI[2] <= plasmaBoltTelegraphDuration)
                    {
                        float pulseCounter = (float)Math.Floor(calamityGlobalNPC.newAI[2] / (plasmaBoltTelegraphDuration / 5f)) + 1;
                        EnergyDrawer.AddPulse(pulseCounter);
                    }

                    if (calamityGlobalNPC.newAI[2] >= plasmaBoltTelegraphDuration + plasmaBoltDuration)
                    {
                        AIState = (float)Phase.Nothing;
                        calamityGlobalNPC.newAI[2] = 0f;
                    }

                    break;
            }

            EnergyDrawer.Update();

            // Smooth movement towards the location Ares Plasma Flamethrower is meant to be at
            CalamityUtils.SmoothMovement(NPC, movementDistanceGateValue, distanceFromDestination, baseVelocity, 0f, false);

            // Update the telegraph sound if it's being played. Immediately stop it if Ares just begun transitioning to his laserbeam attack, since that automatically resets all impending cannon shots.
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
            // Use telegraph frames when using plasma bolts
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

                    // Reset the frames to frame 36, the start of the plasma bolt firing animation loop
                    if ((frameX * maxFramesY) + frameY > finalStagePlasmaBoltChargeFrameLimit)
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

            //Draw an outline to the arm when it charges up
            if ((NPC.Calamity().newAI[2] < plasmaBoltTelegraphDuration) && AIState == (float)Phase.PlasmaBolts)
            {
                CalamityUtils.EnterShaderRegion(spriteBatch);
                Color outlineColor = Color.Lerp(Color.GreenYellow, Color.White, NPC.Calamity().newAI[2] / plasmaBoltTelegraphDuration);
                Vector3 outlineHSL = Main.rgbToHsl(outlineColor); //BasicTint uses the opposite hue i guess? or smth is fucked with the way shaders get their colors. anyways, we invert it
                float outlineThickness = MathHelper.Clamp(NPC.Calamity().newAI[2] / plasmaBoltTelegraphDuration * 4f, 0f, 3f);

                GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(1f);
                GameShaders.Misc["CalamityMod:BasicTint"].UseColor(Main.hslToRgb(1 - outlineHSL.X, outlineHSL.Y, outlineHSL.Z));
                GameShaders.Misc["CalamityMod:BasicTint"].Apply();

                for (float i = 0; i < 1; i += 0.125f)
                {
                    spriteBatch.Draw(texture, center + (i * MathHelper.TwoPi + NPC.rotation).ToRotationVector2() * outlineThickness, frame, outlineColor, NPC.rotation, vector, NPC.scale, spriteEffects, 0f);
                }
                CalamityUtils.ExitShaderRegion(spriteBatch);
            }

            spriteBatch.Draw(texture, center, frame, NPC.GetAlpha(drawColor), NPC.rotation, vector, NPC.scale, spriteEffects, 0f);

            Texture2D glowTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresPlasmaFlamethrowerGlow").Value;

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
            //Update the parameters

            //Draw a pulsing version of the cannon above the real one
            if ((NPC.Calamity().newAI[2] < plasmaBoltTelegraphDuration) && AIState == (float)Phase.PlasmaBolts)
            {

                float pulseRatio = (NPC.Calamity().newAI[2] % (plasmaBoltTelegraphDuration / 5f)) / (plasmaBoltTelegraphDuration / 5f);
                float pulseSize = MathHelper.Lerp(0.1f, 0.6f, (float)Math.Floor(NPC.Calamity().newAI[2] / (plasmaBoltTelegraphDuration / 5f)) / 4f);
                float pulseOpacity = MathHelper.Clamp((float)Math.Floor(NPC.Calamity().newAI[2] / (plasmaBoltTelegraphDuration / 5f)) * 0.3f, 1f, 2f);
                spriteBatch.Draw(texture, center, frame, Color.GreenYellow * MathHelper.Lerp(1f, 0f, pulseRatio) * pulseOpacity, NPC.rotation, vector, NPC.scale + pulseRatio * pulseSize, spriteEffects, 0f);

                //Draw the bloom
                EnergyDrawer.DrawBloom(CoreSpritePosition);
            }

            EnergyDrawer.DrawPulses(CoreSpritePosition);
            EnergyDrawer.DrawSet(CoreSpritePosition);

            //Back to normal
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
                for (int i = 0; i < 2; i++)
                {
                    Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                }
                for (int j = 0; j < 20; j++)
                {
                    int plasmaDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 107, 0f, 0f, 0, new Color(0, 255, 255), 2.5f);
                    Main.dust[plasmaDust].noGravity = true;
                    Main.dust[plasmaDust].velocity *= 3f;
                    plasmaDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                    Main.dust[plasmaDust].velocity *= 2f;
                    Main.dust[plasmaDust].noGravity = true;
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AresPlasmaFlamethrower1").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AresPlasmaFlamethrower2").Type, 1f);
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
