using System;
using System.IO;
using CalamityMod.Events;
using CalamityMod.Items.Potions;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Skies;
using CalamityMod.Sounds;
using CalamityMod.UI.VanillaBossBars;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.ExoMechs.Thanatos
{
    [LongDistanceNetSync]
    public class ThanatosHead : ModNPC
    {
        public static int normalIconIndex;
        public static int vulnerableIconIndex;

        public static readonly SoundStyle VentSound = new("CalamityMod/Sounds/Custom/ExoMechs/ThanatosVent");

        public static readonly SoundStyle LaserSound = new("CalamityMod/Sounds/Custom/ExoMechs/THanosLaser");

        public static readonly SoundStyle ThanatosHitSoundOpen = new("CalamityMod/Sounds/NPCHit/ThanatosHitOpen", 2) { Volume = 0.5f };

        public static readonly SoundStyle ThanatosHitSoundClosed = new("CalamityMod/Sounds/NPCHit/ThanatosHitClosed", 3) { Volume = 0.4f };

        public SlotId LaserSoundSlot;

        public static Asset<Texture2D> GlowTexture;
        public static Asset<Texture2D> AuraTexture;
        public static Asset<Texture2D> ReticleLeftTexture;
        public static Asset<Texture2D> ReticleRightTexture;
        public static Asset<Texture2D> ReticleProngLeftTexture;
        public static Asset<Texture2D> ReticleProngRightTexture;
        public static Asset<Texture2D> ReticleTopTexture;
        public static Asset<Texture2D> ReticleBottomTexture;

        internal static void LoadHeadIcons()
        {
            string normalIconPath = "CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosNormalHead";
            string vulnerableIconPath = "CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosVulnerableHead";

            CalamityMod.Instance.AddBossHeadTexture(normalIconPath, -1);
            normalIconIndex = ModContent.GetModBossHeadSlot(normalIconPath);

            CalamityMod.Instance.AddBossHeadTexture(vulnerableIconPath, -1);
            vulnerableIconIndex = ModContent.GetModBossHeadSlot(vulnerableIconPath);
        }

        public enum Phase
        {
            Charge = 0,
            UndergroundLaserBarrage = 1,
            Deathray = 2
        }

        public float AIState
        {
            get => NPC.Calamity().newAI[0];
            set => NPC.Calamity().newAI[0] = value;
        }

        public enum SecondaryPhase
        {
            Nothing = 0,
            Passive = 1,
            PassiveAndImmune = 2
        }

        public float SecondaryAIState
        {
            get => NPC.Calamity().newAI[1];
            set => NPC.Calamity().newAI[1] = value;
        }

        public ThanatosSmokeParticleSet SmokeDrawer = new ThanatosSmokeParticleSet(-1, 3, 0f, 16f, 1.5f);

        // Timer to prevent Thanatos from dealing contact damage for a bit
        private int noContactDamageTimer = 0;

        // Invincibility time for the first 10 seconds
        public const float immunityTime = 600f;

        // Whether the head is venting heat or not, it is vulnerable to damage during venting
        private bool vulnerable = false;

        // Mark Thanatos as a component of the Exo Mechdusa
        public bool exoMechdusa = false;

        // Max time in vent phase
        public const float ventDuration = 180f;

        // Spawn rate for vent clouds
        public const int ventCloudSpawnRate = 10;

        // Default life ratio for the other mechs
        private const float defaultLifeRatio = 5f;

        // Base distance from the target for most attacks
        private const float baseDistance = 800f;

        // Base distance from target location in order to continue turning
        private const float baseTurnDistance = 160f;

        // Max distance from the target before they are unable to hear sound telegraphs
        private const float soundDistance = 2800f;

        // Length variables
        public const int minLength = 100;
        private const int maxLength = 101;

        // Variable used to stop the segment spawning loop
        private bool tailSpawned = false;

        // Used in the lerp to smoothly scale velocity up and down
        private float chargeVelocityScalar = 0f;

        // Total duration of the deathray telegraph
        private const float deathrayTelegraphDuration = 180f;

        // Total duration of the deathray
        private const float deathrayDuration = 180f;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;

            // Ensure that the reticle is not culled due to the player being very far from Thanatos.
            NPCID.Sets.MustAlwaysDraw[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.65f,
                PortraitScale = 0.6f,
                CustomTexturePath = "CalamityMod/ExtraTextures/Bestiary/Thanatos_Bestiary",
                PortraitPositionXOverride = 40
            };
            value.Position.X += 52f;
            value.Position.Y += 16f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
                ReticleLeftTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosReticleLeft", AssetRequestMode.AsyncLoad);
                ReticleRightTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosReticleRight", AssetRequestMode.AsyncLoad);
                ReticleTopTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosReticleTop", AssetRequestMode.AsyncLoad);
                ReticleBottomTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosReticleHead", AssetRequestMode.AsyncLoad);
                ReticleProngRightTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosReticleProngLeft", AssetRequestMode.AsyncLoad);
                ReticleProngLeftTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosReticleProngRight", AssetRequestMode.AsyncLoad);
                AuraTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Thanatos/THanosAura", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 5f;
            NPC.GetNPCDamage();
            NPC.width = 164;
            NPC.height = 164;
            NPC.defense = 100;
            NPC.DR_NERD(0.9999f);
            NPC.Calamity().unbreakableDR = true;
            NPC.LifeMaxNERB(960000, 1150000, 600000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.Opacity = 0f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(15, 0, 0, 0);
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.DeathSound = CommonCalamitySounds.ExoDeathSound;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.BossBar = ModContent.GetInstance<ExoMechsBossBar>();
            NPC.chaseable = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Thanatos")
            });
        }

        public override void BossHeadSlot(ref int index)
        {
            if (SecondaryAIState == (float)SecondaryPhase.PassiveAndImmune)
                index = -1;
            else if (vulnerable)
                index = vulnerableIconIndex;
            else
                index = normalIconIndex;
        }

        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.chaseable);
            writer.Write(NPC.dontTakeDamage);
            writer.Write(noContactDamageTimer);
            writer.Write(chargeVelocityScalar);
            writer.Write(vulnerable);
            writer.Write(exoMechdusa);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.chaseable = reader.ReadBoolean();
            NPC.dontTakeDamage = reader.ReadBoolean();
            noContactDamageTimer = reader.ReadInt32();
            chargeVelocityScalar = reader.ReadSingle();
            vulnerable = reader.ReadBoolean();
            exoMechdusa = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public float GetSlowdownAreaEdgeRadius(bool lastMechAlive) =>
            (BossRushEvent.BossRushActive ? 400f : CalamityWorld.death ? 600f : CalamityWorld.revenge ? 700f : Main.expertMode ? 800f : 1000f) * (lastMechAlive ? 0.6f : 1f) * (Main.zenithWorld && !exoMechdusa ? 2 : Main.getGoodWorld ? 0.5f : 1f);

        public int CheckForOtherMechs(ref int targetIndex, out bool exoPrimeAlive, out bool exoTwinsAlive)
        {
            exoPrimeAlive = false;
            exoTwinsAlive = false;
            int otherExoMechsAlive = 0;
            if (CalamityGlobalNPC.draedonExoMechPrime != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active)
                {
                    // Set target to Ares' target if Ares is alive.
                    targetIndex = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].target;

                    otherExoMechsAlive++;
                    exoPrimeAlive = true;
                }
            }

            // There is no need in checking for the other twin because they have linked HP.
            if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].active)
                {
                    otherExoMechsAlive++;
                    exoTwinsAlive = true;
                }
            }
            return otherExoMechsAlive;
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            CalamityGlobalNPC.draedonExoMechWorm = NPC.whoAmI;

            // Difficulty modes
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            // Target variable
            int targetIndex = NPC.target;

            // Check if the other exo mechs are alive
            int otherExoMechsAlive = CheckForOtherMechs(ref targetIndex, out bool exoPrimeAlive, out bool exoTwinsAlive);

            // These are 5 by default to avoid triggering passive phases after the other mechs are dead
            float exoPrimeLifeRatio = defaultLifeRatio;
            float exoTwinsLifeRatio = defaultLifeRatio;
            if (exoPrimeAlive)
                exoPrimeLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechPrime].lifeMax;
            if (exoTwinsAlive)
                exoTwinsLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].lifeMax;
            float totalOtherExoMechLifeRatio = exoPrimeLifeRatio + exoTwinsLifeRatio;

            // Check if any of the other mechs are passive
            bool exoPrimePassive = false;
            bool exoTwinsPassive = false;
            if (exoPrimeAlive)
                exoPrimePassive = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Calamity().newAI[1] == (float)AresBody.SecondaryPhase.Passive;
            if (exoTwinsAlive)
                exoTwinsPassive = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].Calamity().newAI[1] == (float)Apollo.Apollo.SecondaryPhase.Passive;
            bool anyOtherExoMechPassive = exoPrimePassive || exoTwinsPassive;

            // Check if any of the other mechs were spawned first
            bool exoPrimeWasFirst = false;
            bool exoTwinsWereFirst = false;
            if (exoPrimeAlive)
                exoPrimeWasFirst = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].ai[3] == 1f;
            if (exoTwinsAlive)
                exoTwinsWereFirst = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].ai[3] == 1f;
            bool otherExoMechWasFirst = exoPrimeWasFirst || exoTwinsWereFirst;

            // Check for Draedon
            bool draedonAlive = false;
            if (CalamityGlobalNPC.draedon != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedon].active)
                    draedonAlive = true;
            }

            // Prevent mechs from being respawned
            if (otherExoMechWasFirst)
            {
                if (NPC.ai[3] < 1f)
                    NPC.ai[3] = 1f;
            }

            // Phases
            bool spawnOtherExoMechs = lifeRatio < 0.7f && NPC.ai[3] == 0f;
            bool berserk = lifeRatio < 0.4f || (otherExoMechsAlive == 0 && lifeRatio < 0.7f);
            bool lastMechAlive = berserk && otherExoMechsAlive == 0;

            // Set vulnerable to false by default
            vulnerable = false;

            // If Thanatos doesn't go berserk
            bool otherMechIsBerserk = exoPrimeLifeRatio < 0.4f || exoTwinsLifeRatio < 0.4f;

            // Whether Thanatos should be buffed while in berserk phase
            bool shouldGetBuffedByBerserkPhase = berserk && !otherMechIsBerserk;

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            // Spawn segments
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!tailSpawned && NPC.ai[0] == 0f)
                {
                    int Previous = NPC.whoAmI;
                    for (int num36 = 0; num36 < maxLength; num36++)
                    {
                        int lol;
                        if (num36 >= 0 && num36 < minLength)
                        {
                            if (num36 % 2 == 0)
                                lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<ThanatosBody1>(), NPC.whoAmI);
                            else
                                lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<ThanatosBody2>(), NPC.whoAmI);
                        }
                        else
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<ThanatosTail>(), NPC.whoAmI);

                        Main.npc[lol].realLife = NPC.whoAmI;
                        Main.npc[lol].ai[2] = NPC.whoAmI;
                        Main.npc[lol].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = lol;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
                        Previous = lol;
                    }
                    tailSpawned = true;
                }
            }

            if (NPC.life > Main.npc[(int)NPC.ai[0]].life)
                NPC.life = Main.npc[(int)NPC.ai[0]].life;

            // General AI pattern
            // 0 - Fly towards the target for 7 seconds, gradually speeding up for the first 5 seconds and slowing down for the last 2 seconds, fire lasers from segments that are venting
            // 1 - Fly underneath the target and fire barrages of lasers
            // 2 - Fire deathray from mouth with a telegraph similar to the railgun from enter the gungeon, turn speed is very low during this to avoid cheap hits
            // 3 - Go passive and fly underneath the target while firing lasers
            // 4 - Go passive, immune and invisible; fly far underneath the target and do nothing until next phase

            // Attack patterns
            // If spawned first
            // Phase 1 - 0, 1, 0, 2
            // Phase 2 - 4
            // Phase 3 - 3

            // If berserk, this is the last phase of thanatos
            // Phase 4 - 0, 1, 0, 2

            // If not berserk
            // Phase 4 - 4
            // Phase 5 - 0, 1, 0, 2

            // If berserk, this is the last phase of thanatos
            // Phase 6 - 0, 1, 0, 2

            // If not berserk
            // Phase 6 - 4

            // Berserk, final phase of Thanatos
            // Phase 7 - 0, 1, 0, 2

            // Phase gate values
            float velocityAdjustTime = 20f;
            float speedUpTime = lastMechAlive ? 180f : shouldGetBuffedByBerserkPhase ? 220f : 300f;
            float slowDownTime = lastMechAlive ? 30f : shouldGetBuffedByBerserkPhase ? 40f : 50f;
            float chargePhaseGateValue = speedUpTime + slowDownTime;
            float laserBarrageDuration = lastMechAlive ? 270f : shouldGetBuffedByBerserkPhase ? 300f : 360f;

            // Despawn if target is dead
            bool targetDead = false;
            if (Main.player[targetIndex].dead)
            {
                targetDead = true;
                AIState = (float)Phase.Charge;
                NPC.localAI[0] = 0f;
                NPC.localAI[2] = 0f;
                calamityGlobalNPC.newAI[2] = 0f;
                calamityGlobalNPC.newAI[3] = 0f;
                chargeVelocityScalar = 0f;
                NPC.dontTakeDamage = true;

                NPC.velocity.Y -= 1f;
                if ((double)NPC.position.Y < Main.topWorld + 16f)
                    NPC.velocity.Y -= 1f;

                if ((double)NPC.position.Y < Main.topWorld + 16f)
                {
                    for (int a = 0; a < Main.maxNPCs; a++)
                    {
                        if (Main.npc[a].type == NPC.type || Main.npc[a].type == ModContent.NPCType<Artemis.Artemis>() || Main.npc[a].type == ModContent.NPCType<AresBody>() ||
                            Main.npc[a].type == ModContent.NPCType<AresLaserCannon>() || Main.npc[a].type == ModContent.NPCType<AresPlasmaFlamethrower>() ||
                            Main.npc[a].type == ModContent.NPCType<AresTeslaCannon>() || Main.npc[a].type == ModContent.NPCType<AresGaussNuke>() ||
                            Main.npc[a].type == ModContent.NPCType<Apollo.Apollo>() || Main.npc[a].type == ModContent.NPCType<ThanatosBody1>() ||
                            Main.npc[a].type == ModContent.NPCType<ThanatosBody2>() || Main.npc[a].type == ModContent.NPCType<ThanatosTail>())
                            Main.npc[a].active = false;
                    }
                }
            }

            // Rotation and direction
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            int direction = NPC.direction;
            NPC.direction = NPC.spriteDirection = (NPC.velocity.X > 0f) ? 1 : (-1);
            if (direction != NPC.direction)
                NPC.netUpdate = true;

            // Default vector to fly to
            Vector2 destination = Main.player[targetIndex].Center;

            // Move destination to somewhere far below the target for the first 3 seconds so that Thanatos can fully uncoil quickly
            bool speedUp = false;
            if (NPC.localAI[3] < 180f)
            {
                speedUp = true;
                destination += new Vector2(0f, 2400f);
            }

            // Distance from target
            float distanceFromTarget = Vector2.Distance(NPC.Center, destination);

            // Increase speed if too far from target
            float increaseSpeedMult = 1f;
            float increaseSpeedGateValue = 600f;
            if (distanceFromTarget > increaseSpeedGateValue)
            {
                float distanceAmount = MathHelper.Clamp((distanceFromTarget - increaseSpeedGateValue) / (CalamityGlobalNPC.CatchUpDistance350Tiles - increaseSpeedGateValue), 0f, 1f);
                increaseSpeedMult = MathHelper.Lerp(1f, 3.5f, distanceAmount);
            }

            // Charge variables
            float turnDistance = baseTurnDistance;
            float chargeLocationDistance = turnDistance * 0.2f;

            // Laser Barrage variables
            float laserBarrageLocationBaseDistance = SecondaryAIState == (int)SecondaryPhase.PassiveAndImmune ? baseDistance * 2f : baseDistance;
            Vector2 laserBarrageLocation = new Vector2(0f, NPC.ai[1] % 2f == 0f ? laserBarrageLocationBaseDistance : -laserBarrageLocationBaseDistance);
            float laserBarrageLocationDistance = turnDistance * 3f;

            // Velocity and turn speed values
            float baseVelocityMult = (shouldGetBuffedByBerserkPhase ? 0.15f : 0f) + (bossRush ? 1.25f : death ? 1.2f : revenge ? 1.175f : expertMode ? 1.15f : 1.1f);
            float baseVelocity = 10f * baseVelocityMult;

            // Increase top velocity if target is dead or if Thanatos is uncoiling
            if (targetDead || speedUp)
                baseVelocity *= 4f;
            else
                baseVelocity *= increaseSpeedMult;

            if (Main.getGoodWorld)
                baseVelocity *= 1.15f;

            float turnDegrees = baseVelocity * 0.1f * (shouldGetBuffedByBerserkPhase ? 1.25f : 1.1f);

            float turnSpeed = MathHelper.ToRadians(turnDegrees);
            float chargeVelocityMult = MathHelper.Lerp(1f, 1.5f, chargeVelocityScalar);
            float chargeTurnSpeedMult = MathHelper.Lerp(1f, 1.5f, chargeVelocityScalar);
            float laserBarragePhaseVelocityMult = MathHelper.Lerp(1f, 1.5f, chargeVelocityScalar);
            float laserBarragePhaseTurnSpeedMult = MathHelper.Lerp(1f, 3f, chargeVelocityScalar);
            float deathrayVelocityMult = MathHelper.Lerp(0.5f, 3f, chargeVelocityScalar);
            float deathrayTurnSpeedMult = MathHelper.Lerp(0.5f, 3f, chargeVelocityScalar);

            // Base scale on total time spent in phase
            float chargeVelocityScalarIncrement = 1f / speedUpTime;
            float chargeVelocityScalarDecrement = 1f / slowDownTime;

            // Scalar to use during deathray phase
            float deathrayVelocityScalarIncrement = 1f / deathrayDuration;

            // Scalar to use during laser barrage, passive and immune phases
            float laserBarrageVelocityScalarIncrement = lastMechAlive ? 0.025f : shouldGetBuffedByBerserkPhase ? 0.0225f : 0.02f;
            float laserBarrageVelocityScalarDecrement = 1f / velocityAdjustTime;

            // Passive and Immune phases
            switch ((int)SecondaryAIState)
            {
                case (int)SecondaryPhase.Nothing:

                    // Spawn the other mechs if Thanatos is first
                    if (otherExoMechsAlive == 0 && !exoMechdusa)
                    {
                        if (spawnOtherExoMechs)
                        {
                            // Reset everything
                            if (NPC.ai[3] < 1f)
                                NPC.ai[3] = 1f;

                            SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
                            NPC.localAI[0] = 0f;
                            NPC.localAI[2] = 0f;
                            calamityGlobalNPC.newAI[2] = 0f;
                            calamityGlobalNPC.newAI[3] = 0f;
                            chargeVelocityScalar = 0f;
                            NPC.TargetClosest();

                            // Draedon text for the start of phase 2
                            if (draedonAlive)
                            {
                                Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 1f;
                                Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                // Spawn the fuckers
                                NPC.SpawnOnPlayer(Main.player[targetIndex].whoAmI, ModContent.NPCType<AresBody>());
                                NPC.SpawnOnPlayer(Main.player[targetIndex].whoAmI, ModContent.NPCType<Artemis.Artemis>());
                                NPC.SpawnOnPlayer(Main.player[targetIndex].whoAmI, ModContent.NPCType<Apollo.Apollo>());
                            }
                        }
                    }
                    else
                    {
                        // If not spawned first, go to passive state if any other mech is passive or if Thanatos is under 70% life
                        // Do not run this if berserk
                        // Do not run this if any exo mech is dead
                        if ((anyOtherExoMechPassive || lifeRatio < 0.7f) && !berserk && totalOtherExoMechLifeRatio < 5f)
                        {
                            // Tells Thanatos to return to the battle in passive state and reset everything
                            SecondaryAIState = (float)SecondaryPhase.Passive;
                            NPC.localAI[0] = 0f;
                            NPC.localAI[2] = 0f;
                            calamityGlobalNPC.newAI[2] = 0f;
                            calamityGlobalNPC.newAI[3] = 0f;
                            chargeVelocityScalar = 0f;
                            NPC.TargetClosest();
                        }

                        // Go passive and immune if one of the other mechs is berserk
                        // This is only called if two exo mechs are alive in ideal scenarios
                        // This is not called if Thanatos and another one or two mechs are berserk
                        if (otherMechIsBerserk && !berserk && !exoMechdusa)
                        {
                            // Reset everything
                            SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
                            NPC.localAI[0] = 0f;
                            NPC.localAI[2] = 0f;
                            calamityGlobalNPC.newAI[2] = 0f;
                            calamityGlobalNPC.newAI[3] = 0f;
                            chargeVelocityScalar = 0f;
                            NPC.TargetClosest();

                            // Phase 6, when 1 mech goes berserk and the other one leaves
                            if (draedonAlive)
                            {
                                Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 5f;
                                Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
                            }
                        }
                    }

                    break;

                // Fly underneath target and fire lasers, this happens when all 3 mechs are present and attacking
                case (int)SecondaryPhase.Passive:

                    // Fire lasers while passive
                    AIState = (float)Phase.UndergroundLaserBarrage;

                    // Enter passive and invincible phase if one of the other exo mechs is berserk
                    if (otherMechIsBerserk && !exoMechdusa)
                    {
                        // Reset everything
                        SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
                        NPC.localAI[0] = 0f;
                        NPC.localAI[2] = 0f;
                        calamityGlobalNPC.newAI[2] = 0f;
                        calamityGlobalNPC.newAI[3] = 0f;
                        chargeVelocityScalar = 0f;
                        NPC.TargetClosest();
                    }

                    // If Thanatos is the first mech to go berserk
                    if (berserk)
                    {
                        // Reset everything
                        AIState = (float)Phase.Charge;
                        NPC.localAI[0] = 0f;
                        NPC.localAI[2] = 0f;
                        calamityGlobalNPC.newAI[2] = 0f;
                        calamityGlobalNPC.newAI[3] = 0f;
                        chargeVelocityScalar = 0f;
                        NPC.TargetClosest();

                        // Never be passive if berserk
                        SecondaryAIState = (float)SecondaryPhase.Nothing;

                        // Phase 4, when 1 mech goes berserk and the other 2 leave
                        if (exoTwinsAlive && exoPrimeAlive)
                        {
                            if (draedonAlive)
                            {
                                Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 3f;
                                Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
                            }
                        }
                    }

                    break;

                // Fly underneath target and become immune
                case (int)SecondaryPhase.PassiveAndImmune:

                    // Do nothing while immune
                    AIState = (float)Phase.UndergroundLaserBarrage;

                    // Enter the fight again if any of the other exo mechs is below 70% and other mechs aren't berserk
                    if ((exoPrimeLifeRatio < 0.7f || exoTwinsLifeRatio < 0.7f) && !otherMechIsBerserk)
                    {
                        // Tells Thanatos to return to the battle in passive state and reset everything
                        // Return to normal phases if one or more mechs have been downed
                        SecondaryAIState = totalOtherExoMechLifeRatio > 5f ? (float)SecondaryPhase.Nothing : (float)SecondaryPhase.Passive;
                        NPC.localAI[0] = 0f;
                        NPC.localAI[2] = 0f;
                        calamityGlobalNPC.newAI[2] = 0f;
                        calamityGlobalNPC.newAI[3] = 0f;
                        chargeVelocityScalar = 0f;
                        NPC.TargetClosest();

                        // Phase 3, when all 3 mechs attack at the same time
                        if (exoPrimeAlive && exoTwinsAlive)
                        {
                            if (draedonAlive)
                            {
                                Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 2f;
                                Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
                            }
                        }
                    }

                    // This is here just in case
                    if (berserk)
                    {
                        // Reset everything
                        AIState = (float)Phase.Charge;
                        NPC.localAI[0] = 0f;
                        NPC.localAI[2] = 0f;
                        calamityGlobalNPC.newAI[2] = 0f;
                        calamityGlobalNPC.newAI[3] = 0f;
                        chargeVelocityScalar = 0f;
                        NPC.TargetClosest();

                        // Never be passive if berserk
                        SecondaryAIState = (float)SecondaryPhase.Nothing;
                    }

                    break;
            }

            // Adjust opacity
            bool invisiblePhase = SecondaryAIState == (float)SecondaryPhase.PassiveAndImmune;
            NPC.dontTakeDamage = invisiblePhase;
            if (!invisiblePhase)
            {
                if (noContactDamageTimer > 0)
                    noContactDamageTimer--;

                NPC.Opacity += 0.2f;
                if (NPC.Opacity > 1f)
                    NPC.Opacity = 1f;
            }
            else
            {
                // Deal no contact damage for 3 seconds after becoming visible
                noContactDamageTimer = 185;

                NPC.Opacity -= 0.05f;
                if (NPC.Opacity < 0f)
                    NPC.Opacity = 0f;
            }

            // Attacking phases
            switch ((int)AIState)
            {
                // Fly towards target and gain velocity over time
                case (int)Phase.Charge:

                    // Use a lerp to smoothly scale up velocity and turn speed
                    if (calamityGlobalNPC.newAI[3] == 0f)
                    {
                        chargeVelocityScalar += chargeVelocityScalarIncrement;
                        if (chargeVelocityScalar >= 1f)
                        {
                            chargeVelocityScalar = 1f;
                            calamityGlobalNPC.newAI[3] = 1f;
                        }
                    }
                    else
                    {
                        chargeVelocityScalar -= chargeVelocityScalarDecrement;
                        if (chargeVelocityScalar < 0f)
                            chargeVelocityScalar = 0f;
                    }

                    baseVelocity *= chargeVelocityMult;
                    turnSpeed *= chargeTurnSpeedMult;
                    turnDistance = chargeLocationDistance;

                    // Gradually turn slower if within 20 tiles of the target
                    float turnSlowerDistanceGateValue = lastMechAlive ? 160f : 240f;
                    if (distanceFromTarget < turnSlowerDistanceGateValue)
                        turnSpeed *= distanceFromTarget / turnSlowerDistanceGateValue;

                    calamityGlobalNPC.newAI[2] += 1f;
                    if (calamityGlobalNPC.newAI[2] >= chargePhaseGateValue)
                    {
                        AIState = NPC.localAI[0] == 1f ? (float)Phase.Deathray : (float)Phase.UndergroundLaserBarrage;
                        calamityGlobalNPC.newAI[2] = 0f;
                        calamityGlobalNPC.newAI[3] = 0f;
                        chargeVelocityScalar = 0f;
                        NPC.TargetClosest();
                    }

                    break;

                // Fly below and summon barrages of lasers
                case (int)Phase.UndergroundLaserBarrage:

                    // Fly down
                    destination += laserBarrageLocation;
                    turnDistance = laserBarrageLocationDistance;

                    // Use a lerp to smoothly scale up velocity and turn speed
                    if (calamityGlobalNPC.newAI[3] == 0f)
                    {
                        chargeVelocityScalar += laserBarrageVelocityScalarIncrement;
                        if (chargeVelocityScalar > 1f)
                            chargeVelocityScalar = 1f;
                    }

                    baseVelocity *= laserBarragePhaseVelocityMult;
                    turnSpeed *= laserBarragePhaseTurnSpeedMult;

                    if ((destination - NPC.Center).Length() < laserBarrageLocationDistance || calamityGlobalNPC.newAI[2] > 0f)
                    {
                        calamityGlobalNPC.newAI[2] += 1f;

                        if (SecondaryAIState != (float)SecondaryPhase.Passive && SecondaryAIState != (float)SecondaryPhase.PassiveAndImmune)
                        {
                            if (calamityGlobalNPC.newAI[2] >= laserBarrageDuration)
                            {
                                // Use a lerp to smoothly scale down velocity and turn speed
                                chargeVelocityScalar -= laserBarrageVelocityScalarDecrement;
                                if (chargeVelocityScalar < 0f)
                                    chargeVelocityScalar = 0f;

                                calamityGlobalNPC.newAI[3] += 1f;
                                if (calamityGlobalNPC.newAI[3] >= velocityAdjustTime)
                                {
                                    NPC.ai[1] += (shouldGetBuffedByBerserkPhase && revenge) ? 1f : 0f;
                                    NPC.localAI[0] = shouldGetBuffedByBerserkPhase ? 1f : 0f;
                                    AIState = (float)Phase.Charge;
                                    calamityGlobalNPC.newAI[2] = 0f;
                                    calamityGlobalNPC.newAI[3] = 0f;
                                    chargeVelocityScalar = 0f;
                                    NPC.TargetClosest();
                                }
                            }
                        }
                    }

                    break;

                // Move close to target, reduce velocity and turn speed when close enough, create telegraph beams, reduce turn speed, fire deathray
                case (int)Phase.Deathray:

                    // Head is vulnerable while charging and firing deathray
                    vulnerable = true;

                    // If close enough to the target, prepare to fire deathray
                    float slowDownDistance = GetSlowdownAreaEdgeRadius(lastMechAlive);
                    bool readyToFireDeathray = distanceFromTarget < slowDownDistance;
                    if (readyToFireDeathray)
                        NPC.localAI[2] = 1f;

                    // Use a lerp to smoothly scale up velocity and turn speed
                    if (calamityGlobalNPC.newAI[3] == 0f)
                    {
                        chargeVelocityScalar += deathrayVelocityScalarIncrement;
                        if (chargeVelocityScalar >= 1f)
                        {
                            chargeVelocityScalar = 1f;

                            // If ready to fire deathray, start reducing the velocity scalar
                            if (NPC.localAI[2] == 1f)
                                calamityGlobalNPC.newAI[3] = 1f;
                        }
                    }
                    else
                    {
                        // Reduce velocity scalar very quickly
                        chargeVelocityScalar -= deathrayVelocityScalarIncrement * 5f;
                        if (chargeVelocityScalar < 0f)
                            chargeVelocityScalar = 0f;
                    }

                    baseVelocity *= deathrayVelocityMult;
                    turnSpeed *= deathrayTurnSpeedMult;
                    turnDistance = chargeLocationDistance;

                    // Gradually turn and move slower if within 50 tiles of the target
                    if (NPC.localAI[2] == 1f)
                    {
                        // Exponentially scale down velocity if close to the target
                        float velocityScale = distanceFromTarget / slowDownDistance;
                        if (velocityScale < 1f)
                            velocityScale *= velocityScale;

                        baseVelocity *= velocityScale;
                        turnSpeed *= velocityScale;

                        calamityGlobalNPC.newAI[2] += 1f;
                        if (calamityGlobalNPC.newAI[2] < deathrayTelegraphDuration)
                        {
                            // Fire deathray telegraph beams
                            if (calamityGlobalNPC.newAI[2] == 1f)
                            {
                                //Commented out in case we decide its better for it to simply play from anywhere in range
                                //if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < soundDistance)
                                LaserSoundSlot = SoundEngine.PlaySound(LaserSound, NPC.Center);

                                // Create a bunch of lightning bolts in the sky
                                ExoMechsSky.CreateLightningBolt(12);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int type = ModContent.ProjectileType<ThanatosBeamTelegraph>();
                                    for (int b = 0; b < 6; b++)
                                    {
                                        int beam = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, type, 0, 0f, 255, NPC.whoAmI);

                                        // Determine the initial offset angle of telegraph. It will be smoothened to give a "stretch" effect.
                                        if (Main.projectile.IndexInRange(beam))
                                        {
                                            float squishedRatio = (float)Math.Pow((float)Math.Sin(MathHelper.Pi * b / 6f), 2D);
                                            float smoothenedRatio = MathHelper.SmoothStep(0f, 1f, squishedRatio);
                                            Main.projectile[beam].ai[0] = NPC.whoAmI;
                                            Main.projectile[beam].ai[1] = MathHelper.Lerp(-0.74f, 0.74f, smoothenedRatio);
                                        }
                                    }
                                    int beam2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, type, 0, 0f, 255, NPC.whoAmI);
                                    if (Main.projectile.IndexInRange(beam2))
                                        Main.projectile[beam2].ai[0] = NPC.whoAmI;
                                }
                            }
                        }
                        else
                        {
                            // Fire deathray
                            if (calamityGlobalNPC.newAI[2] == deathrayTelegraphDuration)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int type = ModContent.ProjectileType<ThanatosBeamStart>();
                                    int damage = NPC.GetProjectileDamage(type);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, type, damage, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                                }
                            }
                        }

                        if (calamityGlobalNPC.newAI[2] >= deathrayTelegraphDuration + deathrayDuration)
                        {
                            if (Main.zenithWorld && !exoMechdusa)
                            {
                                AIState = (float)Phase.Deathray;
                            }
                            else
                            {
                                AIState = (float)Phase.Charge;
                            }
                            NPC.localAI[0] = 0f;
                            NPC.localAI[2] = 0f;
                            calamityGlobalNPC.newAI[2] = 0f;
                            calamityGlobalNPC.newAI[3] = 0f;
                            chargeVelocityScalar = 0f;
                            NPC.TargetClosest();
                        }
                    }

                    break;
            }

            // Do not deal contact damage for 5 seconds after spawning
            if (NPC.localAI[3] == 0f)
                noContactDamageTimer = 300;

            if (NPC.localAI[3] < immunityTime)
                NPC.localAI[3] += 1f;

            // Homing only works if vulnerable is true
            NPC.chaseable = vulnerable;

            // Adjust DR based on vulnerable
            NPC.Calamity().DR = vulnerable ? 0f : 0.9999f;
            NPC.Calamity().unbreakableDR = !vulnerable;

            // Vent noise and steam
            SmokeDrawer.ParticleSpawnRate = 9999999;
            if (vulnerable)
            {
                // Noise
                if (NPC.localAI[1] == 0f)
                    SoundEngine.PlaySound(VentSound, NPC.Center);

                // Steam
                NPC.localAI[1] += 1f;
                if (NPC.localAI[1] < ventDuration)
                {
                    SmokeDrawer.BaseMoveRotation = NPC.rotation - MathHelper.PiOver2;
                    SmokeDrawer.ParticleSpawnRate = ventCloudSpawnRate;
                }
            }
            else
                NPC.localAI[1] = 0f;

            SmokeDrawer.Update();

            if (!targetDead)
            {
                // Increase velocity if velocity is ever zero
                if (NPC.velocity == Vector2.Zero)
                    NPC.velocity = Vector2.Normalize(Main.player[targetIndex].Center - NPC.Center).SafeNormalize(Vector2.Zero) * baseVelocity;

                // Acceleration
                if (!((destination - NPC.Center).Length() < turnDistance))
                {
                    float targetAngle = NPC.AngleTo(destination);
                    float f = NPC.velocity.ToRotation().AngleTowards(targetAngle, turnSpeed);
                    NPC.velocity = f.ToRotationVector2() * baseVelocity;
                }
            }

            // Velocity upper limit
            if (NPC.velocity.Length() > baseVelocity)
                NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero) * baseVelocity;

            //Update the laser sound if it's being done.
            if (SoundEngine.TryGetActiveSound(LaserSoundSlot, out var laserSound) && laserSound.IsPlaying)
            {
                laserSound.Position = NPC.Center;
            }

            if (exoMechdusa)
            {
                if (CalamityGlobalNPC.draedonExoMechPrime != -1)
                {
                    if (Main.npc[CalamityGlobalNPC.draedonExoMechPrime].ModNPC<AresBody>().exoMechdusa)
                    {
                        NPC.rotation = 0;
                        NPC aresin = Main.npc[CalamityGlobalNPC.draedonExoMechPrime];
                        if (NPC.Calamity().newAI[0] != (float)Phase.Deathray)
                        {
                            Vector2 pos = new Vector2(aresin.Center.X - 80, aresin.Center.Y - 89);
                            NPC.position = pos;
                            NPC.Calamity().newAI[2]++;
                        }
                    }
                }
            }

            // Calculate contact damage based on velocity
            float minimalContactDamageVelocity = baseVelocity * 0.25f;
            float minimalDamageVelocity = baseVelocity * 0.5f;
            if (NPC.velocity.Length() <= minimalContactDamageVelocity)
            {
                NPC.damage = (int)Math.Round(NPC.defDamage * 0.5);
            }
            else
            {
                float velocityDamageScalar = MathHelper.Clamp((NPC.velocity.Length() - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                NPC.damage = (int)MathHelper.Lerp((float)Math.Round(NPC.defDamage * 0.5), NPC.defDamage, velocityDamageScalar);
            }
        }

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

            return minDist <= 50f && NPC.Opacity == 1f && noContactDamageTimer <= 0;
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (NPC.localAI[3] < immunityTime)
                modifiers.SourceDamage *= 0.01f;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return null;
        }

        public override void FindFrame(int frameHeight) // 5 total frames
        {
            // Swap between venting and non-venting frames
            NPC.frameCounter += 1D;
            if (AIState == (float)Phase.Charge || AIState == (float)Phase.UndergroundLaserBarrage)
            {
                if (NPC.frameCounter >= 6D)
                {
                    NPC.frame.Y -= frameHeight;
                    NPC.frameCounter = 0D;
                }
                if (NPC.frame.Y < 0)
                    NPC.frame.Y = 0;
            }
            else
            {
                if (NPC.frameCounter >= 6D)
                {
                    NPC.frame.Y += frameHeight;
                    NPC.frameCounter = 0D;
                }
                int finalFrame = Main.npcFrameCount[NPC.type] - 1;
                if (NPC.frame.Y >= frameHeight * finalFrame)
                    NPC.frame.Y = frameHeight * finalFrame;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Vector2 vector = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);

            Vector2 center = NPC.Center - screenPos;
            center -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            center += vector * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture, center, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector, NPC.scale, spriteEffects, 0f);

            texture = GlowTexture.Value;
            Color glowmaskColor = vulnerable ? ((AIState == (int)Phase.Deathray && SecondaryAIState != (int)SecondaryPhase.PassiveAndImmune) ? Color.Lerp(new Color(255, 64, 64), Color.CornflowerBlue, NPC.Calamity().newAI[2] / deathrayTelegraphDuration) : new Color(255, 64, 64)) : Color.White;

            spriteBatch.Draw(texture, center, NPC.frame, glowmaskColor * NPC.Opacity, NPC.rotation, vector, NPC.scale, spriteEffects, 0f);

            SmokeDrawer.DrawSet(NPC.Center);

            // Draw a white indicator aura and reticle before and while a death ray is being fired.
            // This is done to give a visual indicator that players should move near the head (which results in slower movement and thus easier dodging capabilities).
            if (AIState == (int)Phase.Deathray && SecondaryAIState != (int)SecondaryPhase.PassiveAndImmune)
            {
                spriteBatch.SetBlendState(BlendState.Additive);

                // A large, faded circle. Is rescaled to fit the radius of the slowdown area.
                Texture2D auraTexture = AuraTexture.Value;

                float lifeRatio = NPC.life / (float)NPC.lifeMax;

                // Yes, this is bizarre as fuck.
                int completelyUseless = 0;
                int otherExoMechsAlive = CheckForOtherMechs(ref completelyUseless, out _, out _);

                // A general factor for the aura.
                // Used to cause fade-ins/outs as the attack starts/stops.
                float auraGeneralPower = Utils.GetLerpValue(0f, deathrayTelegraphDuration * 0.333f, NPC.Calamity().newAI[2], true);
                auraGeneralPower *= Utils.GetLerpValue(deathrayTelegraphDuration + deathrayDuration, deathrayTelegraphDuration + deathrayDuration, NPC.Calamity().newAI[2], true);

                // Determine the characteristics of the aura. This requires intermediate computations to determine if Thanatos is in its final phase as well as for pulsing.
                bool berserk = lifeRatio < 0.4f || (otherExoMechsAlive == 0 && lifeRatio < 0.7f);
                bool lastMechAlive = berserk && otherExoMechsAlive == 0;
                float pulse = Main.GlobalTimeWrappedHourly * 0.72f % 1f;
                float auraRadius = GetSlowdownAreaEdgeRadius(lastMechAlive) * auraGeneralPower * 1.25f;
                Vector2 outerAuraScale = Vector2.One * auraRadius / auraTexture.Size();
                Vector2 innerAuraScale = outerAuraScale * pulse * 1.2f;
                Color outerAuraColor = Color.White * auraGeneralPower * 0.6f;
                Color innerAuraColor = outerAuraColor * (float)Math.Sqrt(1f - pulse);

                // Draw the aura.
                spriteBatch.Draw(auraTexture, center, null, outerAuraColor, 0f, auraTexture.Size() * 0.5f, outerAuraScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(auraTexture, center, null, innerAuraColor, 0f, auraTexture.Size() * 0.5f, innerAuraScale, SpriteEffects.None, 0f);

                spriteBatch.SetBlendState(BlendState.AlphaBlend);

                // Draw the reticle depending on how close the player is. This drawcode terminates early if Thanatos has no target.
                if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                    return false;

                Player target = Main.player[NPC.target];

                Texture2D leftReticleTexture = ReticleLeftTexture.Value;
                Texture2D rightReticleTexture = ReticleRightTexture.Value;
                Texture2D topReticleTexture = ReticleTopTexture.Value;
                Texture2D bottomReticleTexture = ReticleBottomTexture.Value;
                Texture2D leftReticleProngTexture = ReticleProngLeftTexture.Value;
                Texture2D rightReticleProngTexture = ReticleProngRightTexture.Value;

                // The reticle fades away and moves farther away from the target the closer they are to the aura.
                // Once far away, the reticle will flash between red and white as an indicator.
                float targetHeadDistance = NPC.Distance(target.Center);
                float reticleOpacity = Utils.GetLerpValue(auraRadius * 0.5f - 40f, auraRadius * 0.5f + 100f, targetHeadDistance, true);
                float reticleOffsetDistance = MathHelper.SmoothStep(300f, 0f, reticleOpacity);
                float reticleFadeToWhite = ((float)Math.Cos(Main.GlobalTimeWrappedHourly * 6.8f) * 0.5f + 0.5f) * reticleOpacity * 0.67f;
                Color reticleBaseColor = new Color(255, 0, 0, 127) * reticleOpacity;
                Color reticleFlashBaseColor = Color.Lerp(reticleBaseColor, new Color(255, 255, 255, 0), reticleFadeToWhite) * reticleOpacity;
                Vector2 origin = leftReticleTexture.Size() * 0.5f;

                Vector2 playerDrawPosition = target.Center - screenPos;
                spriteBatch.Draw(leftReticleTexture, playerDrawPosition - Vector2.UnitX * reticleOffsetDistance, null, reticleBaseColor, 0f, origin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(rightReticleTexture, playerDrawPosition + Vector2.UnitX * reticleOffsetDistance, null, reticleBaseColor, 0f, origin, 1f, SpriteEffects.None, 0f);

                for (int i = 0; i < 3; i++)
                {
                    float scale = 1f + i * 0.125f;
                    spriteBatch.Draw(leftReticleProngTexture, playerDrawPosition - Vector2.UnitX * reticleOffsetDistance, null, reticleFlashBaseColor, 0f, origin, scale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(rightReticleProngTexture, playerDrawPosition + Vector2.UnitX * reticleOffsetDistance, null, reticleFlashBaseColor, 0f, origin, scale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(bottomReticleTexture, playerDrawPosition + Vector2.UnitY * reticleOffsetDistance, null, reticleFlashBaseColor, 0f, origin, scale, SpriteEffects.None, 0f);
                }
                spriteBatch.Draw(topReticleTexture, playerDrawPosition - Vector2.UnitY * reticleOffsetDistance, null, reticleBaseColor, 0f, origin, 1f, SpriteEffects.None, 0f);
            }

            return false;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<OmegaHealingPotion>();
        }

        public override bool SpecialOnKill()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(NPC,
                ModContent.NPCType<ThanatosHead>(),
                ModContent.NPCType<ThanatosBody1>(),
                ModContent.NPCType<ThanatosBody2>(),
                ModContent.NPCType<ThanatosTail>());
            NPC.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void OnKill()
        {
            // Check if the other exo mechs are alive
            bool exoTwinsAlive = false;
            bool exoPrimeAlive = false;
            if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].active)
                    exoTwinsAlive = true;
            }
            if (CalamityGlobalNPC.draedonExoMechPrime != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active)
                    exoPrimeAlive = true;
            }

            // Check for Draedon
            bool draedonAlive = false;
            if (CalamityGlobalNPC.draedon != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedon].active)
                    draedonAlive = true;
            }

            // Phase 5, when 1 mech dies and the other 2 return to fight
            if (exoTwinsAlive && exoPrimeAlive)
            {
                if (draedonAlive)
                {
                    Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 4f;
                    Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
                }
            }

            // Phase 7, when 1 mech dies and the final one returns to the fight
            else if (exoTwinsAlive || exoPrimeAlive)
            {
                if (draedonAlive)
                {
                    Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 6f;
                    Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
                }
            }
            else
                AresBody.DoMiscDeathEffects(NPC, AresBody.MechType.Thanatos);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => AresBody.DefineExoMechLoot(NPC, npcLoot, (int)AresBody.MechType.Thanatos);

        public override void ModifyTypeName(ref string typeName)
        {
            if (exoMechdusa)
            {
                typeName = this.GetLocalizedValue("HekateName");
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.soundDelay == 0)
            {
                if (vulnerable)
                {
                    NPC.soundDelay = 8;
                    SoundEngine.PlaySound(ThanatosHitSoundOpen, NPC.Center);
                }
                else
                {
                    NPC.soundDelay = 3;
                    SoundEngine.PlaySound(ThanatosHitSoundClosed, NPC.Center);
                }
            }

            for (int k = 0; k < 3; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TerraBlade, 0f, 0f, 100, new Color(0, 255, 255), 1f);

            if (NPC.life <= 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TerraBlade, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                }
                for (int j = 0; j < 20; j++)
                {
                    int plasmaDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TerraBlade, 0f, 0f, 0, new Color(0, 255, 255), 2.5f);
                    Main.dust[plasmaDust].noGravity = true;
                    Main.dust[plasmaDust].velocity *= 3f;
                    plasmaDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TerraBlade, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                    Main.dust[plasmaDust].velocity *= 2f;
                    Main.dust[plasmaDust].noGravity = true;
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ThanatosHead").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ThanatosHead2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ThanatosHead3").Type, 1f);
                }
            }
        }

        public override bool CheckActive() => false;

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }
    }
}
