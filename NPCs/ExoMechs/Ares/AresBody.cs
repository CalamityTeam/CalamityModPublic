using CalamityMod.Events;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Skies;
using CalamityMod.Sounds;
using CalamityMod.UI.VanillaBossBars;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.ExoMechs.Ares
{
    [AutoloadBossHead]
    public class AresBody : ModNPC
    {
        // Used for loot
        public enum MechType
        {
            Ares = 0,
            Thanatos = 1,
            ArtemisAndApollo = 2
        }

        public enum Phase
        {
            Normal = 0,
            Deathrays = 1
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

        public enum Enraged
        {
            No = 0,
            Yes = 1
        }

        public float EnragedState
        {
            get => NPC.localAI[1];
            set => NPC.localAI[1] = value;
        }

        public float VelocityBoostMult
        {
            get => NPC.localAI[2];
            set => NPC.localAI[2] = value;
        }

        public ThanatosSmokeParticleSet SmokeDrawer = new ThanatosSmokeParticleSet(-1, 3, 0f, 16f, 1.5f);

        // This stores the sound slot of the deathray sound Ares makes, so it may be properly updated in terms of position and looped.
        public SlotId DeathraySoundSlot;

        // Spawn rate for enrage steam
        public const int ventCloudSpawnRate = 3;

        // Spawn rate for telegraph particles
        public const int telegraphParticlesSpawnRate = 5;

        // Number of frames on the X and Y axis
        private const int maxFramesX = 6;
        private const int maxFramesY = 8;

        // Counters for frames on the X and Y axis
        private int frameX = 0;
        private int frameY = 0;

        // Frame limit per animation, these are the specific frames where each animation ends
        private const int normalFrameLimit = 11;
        private const int firstStageDeathrayChargeFrameLimit = 23;
        private const int secondStageDeathrayChargeFrameLimit = 35;
        private const int finalStageDeathrayChargeFrameLimit = 47;

        // Default life ratio for the other mechs
        private const float defaultLifeRatio = 5f;

        // Variable used to stop the arm spawning loop
        private bool armsSpawned = false;

        // Exo Mechdusa stuff
        public bool exoMechdusa = false;
        public int neurontimer = 0;

        // Total duration of the deathray telegraph
        public const float deathrayTelegraphDuration_Normal = 150f;
        public const float deathrayTelegraphDuration_Expert = 120f;
        public const float deathrayTelegraphDuration_Rev = 105f;
        public const float deathrayTelegraphDuration_Death = 90f;
        public const float deathrayTelegraphDuration_BossRush = 60f;

        // Total duration of the deathrays
        public const float deathrayDuration = 600f;

        // Max distance from the target before they are unable to hear sound telegraphs
        private const float soundDistance = 4800f;

        // Distance the player has to be away from Ares in order to trigger the Deathray Spiral enrage
        private const float DeathrayEnrageDistance = 2480f;

        // Timers for the Tesla and Plasma Arms so that they fire at the proper times when they spawn and enter new phases
        public const float plasmaArmStartTimer = 260f;
        public const float teslaArmStartTimer = 80f;

        public static readonly SoundStyle EnragedSound = new("CalamityMod/Sounds/Custom/ExoMechs/AresEnraged");

        public static readonly SoundStyle LaserStartSound = new("CalamityMod/Sounds/Custom/ExoMechs/AresCircleLaserStart");

        public static readonly SoundStyle LaserLoopSound = new SoundStyle("CalamityMod/Sounds/Custom/ExoMechs/AresCircleLaserLoop") with { IsLooped = true };

        public static readonly SoundStyle LaserEndSound = new("CalamityMod/Sounds/Custom/ExoMechs/AresCircleLaserEnd");
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[NPC.type] = 3;
            NPCID.Sets.TrailCacheLength[NPC.type] = NPC.oldPos.Length;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                PortraitScale = 0.54f,
                Scale = 0.4f
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 5f;
            NPC.damage = 100;
            NPC.width = 220;
            NPC.height = 252;
            NPC.defense = 100;
            NPC.DR_NERD(0.35f);
            NPC.LifeMaxNERB(1250000, 1495000, 650000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.Opacity = 0f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(15, 0, 0, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.DeathSound = CommonCalamitySounds.ExoDeathSound;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.BossBar = ModContent.GetInstance<ExoMechsBossBar>();
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Ares")
            });
        }

        public override void BossHeadSlot(ref int index)
        {
            if (SecondaryAIState == (float)SecondaryPhase.PassiveAndImmune)
                index = -1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(frameX);
            writer.Write(frameY);
            writer.Write(armsSpawned);
            writer.Write(exoMechdusa);
            writer.Write(NPC.dontTakeDamage);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
            writer.Write(neurontimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            frameX = reader.ReadInt32();
            frameY = reader.ReadInt32();
            armsSpawned = reader.ReadBoolean();
            exoMechdusa = reader.ReadBoolean();
            NPC.dontTakeDamage = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
            neurontimer = reader.ReadInt32();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            CalamityGlobalNPC.draedonExoMechPrime = NPC.whoAmI;

            NPC.frame = new Rectangle(NPC.width * frameX, NPC.height * frameY, NPC.width, NPC.height);

            // Difficulty modes
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            // Spawn arms
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!armsSpawned && NPC.ai[0] == 0f)
                {
                    int totalArms = 4;
                    int Previous = NPC.whoAmI;
                    for (int i = 0; i < totalArms; i++)
                    {
                        int lol = 0;
                        switch (i)
                        {
                            case 0:
                                lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<AresLaserCannon>(), NPC.whoAmI);
                                break;
                            case 1:
                                lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<AresPlasmaFlamethrower>(), NPC.whoAmI);
                                Main.npc[lol].Calamity().newAI[1] = plasmaArmStartTimer;
                                break;
                            case 2:
                                lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<AresTeslaCannon>(), NPC.whoAmI);
                                Main.npc[lol].Calamity().newAI[1] = teslaArmStartTimer;
                                break;
                            case 3:
                                lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<AresGaussNuke>(), NPC.whoAmI);
                                break;
                            default:
                                break;
                        }

                        Main.npc[lol].realLife = NPC.whoAmI;
                        Main.npc[lol].ai[2] = NPC.whoAmI;
                        Main.npc[lol].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = lol;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
                        Previous = lol;
                    }
                    if (exoMechdusa)
                    {
                        NPC apolloNPC = CalamityUtils.SpawnBossBetter(NPC.Center, ModContent.NPCType<Apollo.Apollo>());
                        apolloNPC.ModNPC<Apollo.Apollo>().exoMechdusa = true;
                        NPC artemisNPC = CalamityUtils.SpawnBossBetter(NPC.Center, ModContent.NPCType<Artemis.Artemis>());
                        artemisNPC.ModNPC<Artemis.Artemis>().exoMechdusa = true;
                        NPC thanosNPC = CalamityUtils.SpawnBossBetter(NPC.Center, ModContent.NPCType<ThanatosHead>());
                        thanosNPC.ModNPC<ThanatosHead>().exoMechdusa = true;
                    }
                    armsSpawned = true;
                }
            }

            if (exoMechdusa)
            {
                int yoffset = 180;
                int xoffset = 180;
                Vector2 NeuronRight = new Vector2(NPC.Center.X + xoffset, NPC.Center.Y + yoffset);
                Vector2 NeuronLeft = new Vector2(NPC.Center.X - xoffset, NPC.Center.Y + yoffset);
                NPC.alpha = 0;
                NPC.dontTakeDamage = true;
                neurontimer++; 
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (neurontimer >= 180)
                    {
                        float variance = MathHelper.TwoPi / 6;
                        for (int i = 0; i < 6; i++)
                        {
                            Vector2 velocity = new Vector2(0f, 6f);
                            velocity = velocity.RotatedBy(variance * i);
                            velocity.Normalize();

                            Vector2 betweenR = NeuronRight + velocity * 650;
                            Vector2 betweenL = NeuronLeft + velocity * 650;
                        
                            Terraria.Audio.SoundEngine.PlaySound(CommonCalamitySounds.LaserCannonSound with { Volume = CommonCalamitySounds.LaserCannonSound.Volume - 0.2f, Pitch = CommonCalamitySounds.LaserCannonSound.Pitch + 0.2f }, NeuronRight);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), betweenL, betweenL + velocity, ModContent.ProjectileType<ArtemisLaser>(), 111, 0f, Main.myPlayer, 7, NPC.whoAmI);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), betweenR, betweenR + velocity, ModContent.ProjectileType<ArtemisLaser>(), 111, 0f, Main.myPlayer, 7, NPC.whoAmI);

                        }
                        neurontimer = 0;
                    }
                }
                if (NPC.CountNPCS(ModContent.NPCType<Artemis.Artemis>()) > 0 || NPC.CountNPCS(ModContent.NPCType<ThanatosHead>()) > 0)
                {
                    NPC.TargetClosest();
                    Vector2 where2 = new Vector2(Main.player[NPC.target].Center.X + 600, Main.player[NPC.target].Center.Y - 200);
                    if (CalamityGlobalNPC.draedonExoMechWorm != -1)
                    {
                        if (Main.npc[CalamityGlobalNPC.draedonExoMechWorm].Calamity().newAI[0] == (float)ThanatosHead.Phase.Deathray)
                        {
                            where2 = new Vector2(Main.npc[CalamityGlobalNPC.draedonExoMechWorm].position.X, Main.npc[CalamityGlobalNPC.draedonExoMechWorm].position.Y - 40);
                            NPC.position = where2;
                        }
                        else
                        {
                            CalamityUtils.SmoothMovement(NPC, 100, where2 - NPC.Center, 8, 1.4f, true);
                        }
                    }
                    else if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1)
                    {
                        if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[0] == (float)Artemis.Artemis.Phase.Deathray)
                        {
                            where2 = new Vector2(Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].position.X, Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].position.Y);
                            NPC.position = where2;
                        }
                        else if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].Calamity().newAI[0] == (float)Apollo.Apollo.Phase.ChargeCombo || Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].Calamity().newAI[0] == (float)Apollo.Apollo.Phase.LineUpChargeCombo)
                        {
                            where2 = new Vector2(Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].position.X, Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].position.Y);
                            NPC.position = where2;
                        }
                        else
                        {
                            CalamityUtils.SmoothMovement(NPC, 100, where2 - NPC.Center, 8, 1.4f, true);
                        }
                    }
                    else
                    {
                        CalamityUtils.SmoothMovement(NPC, 100, where2 - NPC.Center, 8, 1.4f, true);
                    }
                    return;
                }
            }

            if (NPC.life > Main.npc[(int)NPC.ai[0]].life)
                NPC.life = Main.npc[(int)NPC.ai[0]].life;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

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

            // There is no point in checking for the other twin because they have linked HP
            if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].active)
                {
                    otherExoMechsAlive++;
                    exoTwinsAlive = true;
                }
            }

            // These are 5 by default to avoid triggering passive phases after the other mechs are dead
            float exoWormLifeRatio = defaultLifeRatio;
            float exoTwinsLifeRatio = defaultLifeRatio;
            if (exoWormAlive)
                exoWormLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechWorm].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechWorm].lifeMax;
            if (exoTwinsAlive)
                exoTwinsLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].lifeMax;
            float totalOtherExoMechLifeRatio = exoWormLifeRatio + exoTwinsLifeRatio;

            // Check if any of the other mechs are passive
            bool exoWormPassive = false;
            bool exoTwinsPassive = false;
            if (exoWormAlive)
                exoWormPassive = Main.npc[CalamityGlobalNPC.draedonExoMechWorm].Calamity().newAI[1] == (float)ThanatosHead.SecondaryPhase.Passive;
            if (exoTwinsAlive)
                exoTwinsPassive = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].Calamity().newAI[1] == (float)Apollo.Apollo.SecondaryPhase.Passive;
            bool anyOtherExoMechPassive = exoWormPassive || exoTwinsPassive;

            // Check if any of the other mechs were spawned first
            bool exoWormWasFirst = false;
            bool exoTwinsWereFirst = false;
            if (exoWormAlive)
                exoWormWasFirst = Main.npc[CalamityGlobalNPC.draedonExoMechWorm].ai[3] == 1f;
            if (exoTwinsAlive)
                exoTwinsWereFirst = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].ai[3] == 1f;
            bool otherExoMechWasFirst = exoWormWasFirst || exoTwinsWereFirst;

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

            // If Ares doesn't go berserk
            bool otherMechIsBerserk = exoWormLifeRatio < 0.4f || exoTwinsLifeRatio < 0.4f;

            // Whether Ares should be buffed while in berserk phase
            bool shouldGetBuffedByBerserkPhase = berserk && !otherMechIsBerserk;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            // Target variable
            Player player = Main.player[NPC.target];

            // General AI pattern
            // 0 - Fly above target
            // 1 - Fly towards the target, slow down when close enough
            // 2 - Fire deathrays from telegraph locations to avoid cheap hits and rotate them around for 10 seconds while the plasma and tesla arms fire projectiles to make dodging difficult
            // 3 - Go passive and fly above the target while firing less projectiles
            // 4 - Go passive, immune and invisible; fly above the target and do nothing until next phase

            // Attack patterns
            // If spawned first
            // Phase 1 - 0
            // Phase 2 - 4
            // Phase 3 - 3

            // If berserk, this is the last phase of Ares
            // Phase 4 - 1, 2

            // If not berserk
            // Phase 4 - 4
            // Phase 5 - 0

            // If berserk, this is the last phase of Ares
            // Phase 6 - 1, 2

            // If not berserk
            // Phase 6 - 4

            // Berserk, final phase of Ares
            // Phase 7 - 1, 2

            // Rotation
            NPC.rotation = NPC.velocity.X * 0.003f;

            // Enrage check
            if (EnragedState == (float)Enraged.Yes)
                NPC.Calamity().CurrentlyEnraged = true;

            // Despawn if target is dead
            if (player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (player.dead)
                {
                    AIState = (float)Phase.Normal;
                    calamityGlobalNPC.newAI[2] = 0f;
                    calamityGlobalNPC.newAI[3] = 0f;
                    NPC.dontTakeDamage = true;

                    NPC.velocity.Y -= 1f;
                    if ((double)NPC.position.Y < Main.topWorld + 16f)
                        NPC.velocity.Y -= 1f;

                    if ((double)NPC.position.Y < Main.topWorld + 16f)
                    {
                        for (int a = 0; a < Main.maxNPCs; a++)
                        {
                            if (Main.npc[a].type == NPC.type || Main.npc[a].type == ModContent.NPCType<Artemis.Artemis>() || Main.npc[a].type == ModContent.NPCType<Apollo.Apollo>() ||
                                Main.npc[a].type == ModContent.NPCType<AresLaserCannon>() || Main.npc[a].type == ModContent.NPCType<AresPlasmaFlamethrower>() ||
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
            Vector2 destination = SecondaryAIState == (float)SecondaryPhase.PassiveAndImmune ? new Vector2(player.Center.X, player.Center.Y - 800f) : AIState != (float)Phase.Deathrays ? new Vector2(player.Center.X, player.Center.Y - 425f) : player.Center;

            // Velocity and acceleration values
            float baseVelocityMult = (shouldGetBuffedByBerserkPhase ? 0.25f : 0f) + (bossRush ? 1.15f : death ? 1.1f : revenge ? 1.075f : expertMode ? 1.05f : 1f);
            float baseVelocity = (EnragedState == (float)Enraged.Yes ? 28f : 20f) * baseVelocityMult;
            float baseAcceleration = shouldGetBuffedByBerserkPhase ? 1.25f : 1f;
            float decelerationVelocityMult = 0.85f;

            // Distance where Ares stops moving
            float movementDistanceGateValue = 50f;

            // Scale up velocity over time if too far from destination
            Vector2 distanceFromDestination = destination - NPC.Center;
            if (distanceFromDestination.Length() > movementDistanceGateValue && AIState != (float)Phase.Deathrays)
            {
                if (VelocityBoostMult < 1f)
                    VelocityBoostMult += 0.004f;
            }
            else
            {
                if (VelocityBoostMult > 0f)
                    VelocityBoostMult -= 0.004f;
            }
            baseVelocity *= 1f + VelocityBoostMult;

            // Distance from target
            float distanceFromTarget = Vector2.Distance(NPC.Center, player.Center);

            // Gate values
            float deathrayPhaseGateValue = lastMechAlive ? 420f : 600f;
            float deathrayDistanceGateValue = 480f;

            // Enter deathray phase again more quickly if enraged
            if (EnragedState == (float)Enraged.Yes)
                deathrayPhaseGateValue *= 0.75f;

            // Emit steam while enraged
            SmokeDrawer.ParticleSpawnRate = 9999999;
            if (EnragedState == (float)Enraged.Yes)
            {
                SmokeDrawer.ParticleSpawnRate = ventCloudSpawnRate;
                SmokeDrawer.BaseMoveRotation = NPC.rotation + MathHelper.PiOver2;
                SmokeDrawer.SpawnAreaCompactness = 80f;

                // Increase DR during enrage
                NPC.Calamity().DR = 0.85f;
            }
            else
                NPC.Calamity().DR = 0.35f;

            calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = EnragedState == (float)Enraged.Yes;

            SmokeDrawer.Update();

            // Passive and Immune phases
            switch ((int)SecondaryAIState)
            {
                case (int)SecondaryPhase.Nothing:

                    // Spawn the other mechs if Ares is first and not Exo Mechdusa
                    if (otherExoMechsAlive == 0 && !exoMechdusa)
                    {
                        if (spawnOtherExoMechs)
                        {
                            // Reset everything
                            if (NPC.ai[3] < 1f)
                                NPC.ai[3] = 1f;

                            SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
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
                                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<ThanatosHead>());
                                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Artemis.Artemis>());
                                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Apollo.Apollo>());
                            }
                        }
                    }
                    else
                    {
                        // If not spawned first, go to passive state if any other mech is passive or if Ares is under 70% life
                        // Do not run this if berserk
                        // Do not run this if any exo mech is dead
                        if ((anyOtherExoMechPassive || lifeRatio < 0.7f) && !berserk && totalOtherExoMechLifeRatio < 5f)
                        {
                            // Tells Ares to return to the battle in passive state and reset everything
                            SecondaryAIState = (float)SecondaryPhase.Passive;
                            NPC.TargetClosest();
                        }

                        // Go passive and immune if one of the other mechs is berserk
                        // This is only called if two exo mechs are alive in ideal scenarios
                        // This is not called if Ares and another one or two mechs are berserk
                        if (otherMechIsBerserk && !berserk)
                        {
                            // Reset everything
                            if (NPC.ai[3] < 2f)
                                NPC.ai[3] = 2f;

                            SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
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

                // Fire projectiles less often, this happens when all 3 mechs are present and attacking
                case (int)SecondaryPhase.Passive:

                    // Enter passive and invincible phase if one of the other exo mechs is berserk
                    if (otherMechIsBerserk)
                    {
                        // Reset everything
                        if (NPC.ai[3] < 2f)
                            NPC.ai[3] = 2f;

                        SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
                        NPC.TargetClosest();
                    }

                    // If Ares is the first mech to go berserk
                    if (berserk)
                    {
                        // Reset everything
                        NPC.TargetClosest();

                        // Never be passive if berserk
                        SecondaryAIState = (float)SecondaryPhase.Nothing;

                        // Phase 4, when 1 mech goes berserk and the other 2 leave
                        if (exoWormAlive && exoTwinsAlive)
                        {
                            if (draedonAlive)
                            {
                                Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 3f;
                                Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
                            }
                        }
                    }

                    break;

                // Fly above target and become immune
                case (int)SecondaryPhase.PassiveAndImmune:

                    // Enter the fight again if any of the other exo mechs is below 70% and other mechs aren't berserk
                    if ((exoWormLifeRatio < 0.7f || exoTwinsLifeRatio < 0.7f) && !otherMechIsBerserk)
                    {
                        // Tells Ares to return to the battle in passive state and reset everything
                        // Return to normal phases if one or more mechs have been downed
                        SecondaryAIState = totalOtherExoMechLifeRatio > 5f ? (float)SecondaryPhase.Nothing : (float)SecondaryPhase.Passive;
                        NPC.TargetClosest();

                        // Phase 3, when all 3 mechs attack at the same time
                        if (exoWormAlive && exoTwinsAlive)
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

            // Attacking phases
            switch ((int)AIState)
            {
                // Fly above the target
                case (int)Phase.Normal:

                    // Smooth movement towards the location Ares is meant to be at
                    CalamityUtils.SmoothMovement(NPC, movementDistanceGateValue, distanceFromDestination, baseVelocity, 0f, false);

                    if (shouldGetBuffedByBerserkPhase)
                    {
                        calamityGlobalNPC.newAI[2] += 1f;
                        if (calamityGlobalNPC.newAI[2] > deathrayPhaseGateValue)
                        {
                            // Despawn stupid fucking dog shit to avoid screaming the word "cunt"
                            for (int x = 0; x < Main.maxProjectiles; x++)
                            {
                                Projectile projectile = Main.projectile[x];
                                if (projectile.active)
                                {
                                    if (projectile.type == ModContent.ProjectileType<AresTeslaOrb>() || projectile.type == ModContent.ProjectileType<AresPlasmaFireball>() ||
                                        projectile.type == ModContent.ProjectileType<AresPlasmaBolt>() || projectile.type == ModContent.ProjectileType<AresGaussNukeProjectile>() ||
                                        projectile.type == ModContent.ProjectileType<AresGaussNukeProjectileSpark>())
                                    {
                                        if (projectile.timeLeft > 15)
                                            projectile.timeLeft = 15;

                                        if (projectile.type == ModContent.ProjectileType<AresPlasmaFireball>())
                                        {
                                            projectile.ai[0] = -1f;
                                            projectile.ai[1] = -1f;
                                        }
                                        else if (projectile.type == ModContent.ProjectileType<AresGaussNukeProjectile>())
                                            projectile.ai[0] = -1f;
                                    }
                                    else if (projectile.type == ModContent.ProjectileType<AresGaussNukeProjectileBoom>())
                                        projectile.Kill();
                                }
                            }

                            calamityGlobalNPC.newAI[2] = 0f;
                            AIState = (float)Phase.Deathrays;

                            // Cancel enrage state if Ares is enraged
                            if (EnragedState == (float)Enraged.Yes)
                                EnragedState = (float)Enraged.No;
                        }
                    }

                    break;

                // Move close to target, reduce velocity when close enough, create telegraph beams, fire deathrays
                case (int)Phase.Deathrays:

                    // Set flight time to max during Deathray Spiral
                    if (Main.netMode != NetmodeID.Server)
                    {
                        if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < DeathrayEnrageDistance)
                        {
                            Main.player[Main.myPlayer].Calamity().infiniteFlight = true;
                        }
                    }

                    if (distanceFromTarget > deathrayDistanceGateValue && calamityGlobalNPC.newAI[3] == 0f)
                    {
                        Vector2 desiredVelocity2 = Vector2.Normalize(distanceFromDestination) * baseVelocity;
                        NPC.SimpleFlyMovement(desiredVelocity2, baseAcceleration);
                    }
                    else
                    {
                        // Enrage if the target is more than the deathray length away
                        if ((distanceFromTarget > DeathrayEnrageDistance || (CalamityWorld.LegendaryMode && revenge)) && EnragedState == (float)Enraged.No)
                        {
                            // Play enrage sound
                            if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < soundDistance)
                            {
                                SoundEngine.PlaySound(EnragedSound, Main.player[Main.myPlayer].Center);
                            }

                            // Draedon comments on how foolish it is to run
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonAresEnrageText", Draedon.TextColor);

                            // Enrage
                            EnragedState = (float)Enraged.Yes;
                        }

                        calamityGlobalNPC.newAI[3] = 1f;
                        NPC.velocity *= decelerationVelocityMult;

                        int totalProjectiles = bossRush ? 12 : death ? 10 : revenge ? 9 : expertMode ? 8 : 6;
                        if (Main.getGoodWorld)
                            totalProjectiles += 4;

                        float radians = MathHelper.TwoPi / totalProjectiles;
                        bool normalLaserRotation = NPC.localAI[0] % 2f == 0f;
                        float velocity = 6f;
                        double angleA = radians * 0.5;
                        double angleB = MathHelper.ToRadians(90f) - angleA;
                        float velocityX2 = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                        Vector2 spinningPoint = normalLaserRotation ? new Vector2(0f, -velocity) : new Vector2(-velocityX2, -velocity);
                        spinningPoint.Normalize();

                        float deathrayTelegraphDuration = bossRush ? deathrayTelegraphDuration_BossRush : death ? deathrayTelegraphDuration_Death :
                            revenge ? deathrayTelegraphDuration_Rev : expertMode ? deathrayTelegraphDuration_Expert : deathrayTelegraphDuration_Normal;

                        calamityGlobalNPC.newAI[2] += (EnragedState == (float)Enraged.Yes && calamityGlobalNPC.newAI[2] % 2f == 0f) ? 2f : 1f;
                        if (calamityGlobalNPC.newAI[2] < deathrayTelegraphDuration)
                        {
                            // Fire deathray telegraph beams
                            if (calamityGlobalNPC.newAI[2] == 1f)
                            {
                                // Despawn stupid fucking dog shit to avoid screaming the word "cunt", again
                                for (int x = 0; x < Main.maxProjectiles; x++)
                                {
                                    Projectile projectile = Main.projectile[x];
                                    if (projectile.active)
                                    {
                                        if (projectile.type == ModContent.ProjectileType<AresTeslaOrb>() || projectile.type == ModContent.ProjectileType<AresPlasmaFireball>() ||
                                            projectile.type == ModContent.ProjectileType<AresPlasmaBolt>() || projectile.type == ModContent.ProjectileType<AresGaussNukeProjectile>() ||
                                            projectile.type == ModContent.ProjectileType<AresGaussNukeProjectileSpark>())
                                        {
                                            if (projectile.timeLeft > 15)
                                                projectile.timeLeft = 15;

                                            if (projectile.type == ModContent.ProjectileType<AresPlasmaFireball>())
                                            {
                                                projectile.ai[0] = -1f;
                                                projectile.ai[1] = -1f;
                                            }
                                            else if (projectile.type == ModContent.ProjectileType<AresGaussNukeProjectile>())
                                                projectile.ai[0] = -1f;
                                        }
                                        else if (projectile.type == ModContent.ProjectileType<AresGaussNukeProjectileBoom>())
                                            projectile.Kill();
                                    }
                                }

                                // Set frames to deathray charge up frames, which begin on frame 12
                                // Reset the frame counter
                                NPC.frameCounter = 0D;

                                // X = 1 sets to frame 8
                                frameX = 1;

                                // Y = 4 sets to frame 12
                                frameY = 4;

                                // Create a bunch of lightning bolts in the sky
                                ExoMechsSky.CreateLightningBolt(12);

                                SoundEngine.PlaySound(CommonCalamitySounds.LaserCannonSound, NPC.Center);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int type = ModContent.ProjectileType<AresDeathBeamTelegraph>();
                                    Vector2 spawnPoint = NPC.Center + new Vector2(-1f, 23f);
                                    for (int k = 0; k < totalProjectiles; k++)
                                    {
                                        Vector2 laserVelocity = spinningPoint.RotatedBy(radians * k);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPoint + Vector2.Normalize(laserVelocity) * 17f, laserVelocity, type, 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Fire deathrays
                            if (calamityGlobalNPC.newAI[2] == deathrayTelegraphDuration)
                            {
                                DeathraySoundSlot = SoundEngine.PlaySound(LaserStartSound, NPC.Center);
                                
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int type = ModContent.ProjectileType<AresDeathBeamStart>();
                                    int damage = NPC.GetProjectileDamage(type);
                                    Vector2 spawnPoint = NPC.Center + new Vector2(-1f, 23f);
                                    for (int k = 0; k < totalProjectiles; k++)
                                    {
                                        Vector2 laserVelocity = spinningPoint.RotatedBy(radians * k);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPoint + Vector2.Normalize(laserVelocity) * 35f, laserVelocity, type, damage, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                                    }
                                }
                            }
                        }

                        // Update the deathray sound if it's being played.
                        if (SoundEngine.TryGetActiveSound(DeathraySoundSlot, out var deathraySound) && deathraySound.IsPlaying)
                            deathraySound.Position = NPC.Center;
                        if (calamityGlobalNPC.newAI[2] >= deathrayTelegraphDuration)
                        {
                            // Start the loop sound if the start sound finished.
                            if (deathraySound is null || !deathraySound.IsPlaying || calamityGlobalNPC.newAI[2] == deathrayTelegraphDuration + 180f)
                            {
                                if (deathraySound is null || deathraySound.Style == LaserStartSound)
                                {
                                    deathraySound?.Stop();
                                    DeathraySoundSlot = SoundEngine.PlaySound(LaserLoopSound, NPC.Center);
                                }
                                else if (deathraySound is not null)
                                    deathraySound.Resume();
                            }
                        }

                        if (calamityGlobalNPC.newAI[2] >= deathrayTelegraphDuration + deathrayDuration)
                        {
                            if (!Main.zenithWorld || exoMechdusa)
                            {
                                AIState = (float)Phase.Normal;
                                calamityGlobalNPC.newAI[2] = 0f;
                                calamityGlobalNPC.newAI[3] = 0f;

                                /* Normal positions: Laser = 0, Tesla = 1, Plasma = 2, Gauss = 3
                                 * 0 = Laser = 0, Tesla = 1, Plasma = 2, Gauss = 3
                                 * 1 = Laser = 3, Tesla = 1, Plasma = 2, Gauss = 0
                                 * 2 = Laser = 3, Tesla = 2, Plasma = 1, Gauss = 0
                                 * 3 = Laser = 0, Tesla = 2, Plasma = 1, Gauss = 3
                                 * 4 = Laser = 0, Tesla = 1, Plasma = 2, Gauss = 3
                                 * 5 = Laser = 3, Tesla = 1, Plasma = 2, Gauss = 0
                                 */
                                if (revenge)
                                {
                                    NPC.ai[3] += 1f + Main.rand.Next(2);
                                    if (NPC.ai[3] > 5f)
                                        NPC.ai[3] -= 4f;
                                }
                                else if (expertMode)
                                {
                                    NPC.ai[3] += Main.rand.Next(2);
                                    if (NPC.ai[3] > 3f)
                                        NPC.ai[3] -= 2f;
                                }
                            }
                            else
                            {
                                // Despawn stupid fucking dog shit to avoid screaming the word "cunt"
                                for (int x = 0; x < Main.maxProjectiles; x++)
                                {
                                    Projectile projectile = Main.projectile[x];
                                    if (projectile.active)
                                    {
                                        if (projectile.type == ModContent.ProjectileType<AresTeslaOrb>() || projectile.type == ModContent.ProjectileType<AresPlasmaFireball>() ||
                                            projectile.type == ModContent.ProjectileType<AresPlasmaBolt>() || projectile.type == ModContent.ProjectileType<AresGaussNukeProjectile>() ||
                                            projectile.type == ModContent.ProjectileType<AresGaussNukeProjectileSpark>())
                                        {
                                            if (projectile.timeLeft > 15)
                                                projectile.timeLeft = 15;

                                            if (projectile.type == ModContent.ProjectileType<AresPlasmaFireball>())
                                            {
                                                projectile.ai[0] = -1f;
                                                projectile.ai[1] = -1f;
                                            }
                                            else if (projectile.type == ModContent.ProjectileType<AresGaussNukeProjectile>())
                                                projectile.ai[0] = -1f;
                                        }
                                        else if (projectile.type == ModContent.ProjectileType<AresGaussNukeProjectileBoom>())
                                            projectile.Kill();
                                    }
                                }

                                calamityGlobalNPC.newAI[2] = 0f;
                                calamityGlobalNPC.newAI[3] = 0f;

                                // Cancel enrage state if Ares is enraged
                                if (EnragedState == (float)Enraged.Yes)
                                    EnragedState = (float)Enraged.No;
                            }

                            // Stop the laser loop and play the end sound.
                            deathraySound?.Stop();
                            SoundEngine.PlaySound(LaserEndSound, NPC.Center);

                            NPC.localAI[0] += 1f;
                            NPC.TargetClosest();
                            NPC.netUpdate = true;
                        }
                    }

                    break;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 2f;
            return null;
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;

            // Use telegraph frames when using deathrays
            NPC.frameCounter += 1D;
            if ((AIState == (float)Phase.Normal || NPC.Calamity().newAI[3] == 0f) && !NPC.IsABestiaryIconDummy)
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

                    // Reset the frames to frame 0
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
            NPC.frame = new Rectangle(NPC.width * frameX, NPC.height * frameY, NPC.width, NPC.height);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Draw the enrage smoke behind Ares
            SmokeDrawer.DrawSet(NPC.Center);

            // Draw arms.
            int laserArm = NPC.FindFirstNPC(ModContent.NPCType<AresLaserCannon>());
            int gaussArm = NPC.FindFirstNPC(ModContent.NPCType<AresGaussNuke>());
            int teslaArm = NPC.FindFirstNPC(ModContent.NPCType<AresTeslaCannon>());
            int plasmaArm = NPC.FindFirstNPC(ModContent.NPCType<AresPlasmaFlamethrower>());
            Color afterimageBaseColor = EnragedState == (float)Enraged.Yes ? Color.Red : Color.White;
            Color armGlowmaskColor = afterimageBaseColor;
            armGlowmaskColor.A = 184;

            (int, bool)[] armProperties = new (int, bool)[]
            {
                // Laser arm.
                (-1, true),

                // Gauss arm.
                (1, true),

                // Telsa arm.
                (-1, false),

                // Plasma arm.
                (1, false),
            };

            // Swap out arm positions as necessary.
            // Normal Position: Laser, Tesla, Plasma, Laser
            switch ((int)NPC.ai[3])
            {
                case 0:
                    if (AIState == (int)Phase.Deathrays)
                    {
                        CalamityUtils.SwapArrayIndices(ref armProperties, 1, 3);
                        CalamityUtils.SwapArrayIndices(ref armProperties, 0, 1);
                    }
                    break;
                case 1:
                    CalamityUtils.SwapArrayIndices(ref armProperties, 0, 1);
                    if (AIState == (int)Phase.Deathrays)
                        CalamityUtils.SwapArrayIndices(ref armProperties, 0, 3);
                    break;
                case 2:
                    if (AIState != (int)Phase.Deathrays)
                    {
                        CalamityUtils.SwapArrayIndices(ref armProperties, 0, 1);
                        CalamityUtils.SwapArrayIndices(ref armProperties, 2, 3);
                    }
                    else
                    {
                        CalamityUtils.SwapArrayIndices(ref armProperties, 0, 1);
                        CalamityUtils.SwapArrayIndices(ref armProperties, 2, 3);
                        CalamityUtils.SwapArrayIndices(ref armProperties, 0, 2);
                    }
                    break;
                case 3:
                    CalamityUtils.SwapArrayIndices(ref armProperties, 2, 3);
                    break;
                case 4:
                    CalamityUtils.SwapArrayIndices(ref armProperties, 1, 3);
                    break;
                case 5:
                    if (AIState != (int)Phase.Deathrays)
                        CalamityUtils.SwapArrayIndices(ref armProperties, 0, 1);
                    else
                    {
                        CalamityUtils.SwapArrayIndices(ref armProperties, 0, 3);
                        CalamityUtils.SwapArrayIndices(ref armProperties, 1, 3);
                    }
                    break;
            }

            // Create hulking arms that attach to all cannons.
            if (laserArm != -1)
                DrawArm(spriteBatch, Main.npc[laserArm].Center, screenPos, armGlowmaskColor, armProperties[0].Item1, armProperties[0].Item2);
            if (gaussArm != -1)
                DrawArm(spriteBatch, Main.npc[gaussArm].Center, screenPos, armGlowmaskColor, armProperties[1].Item1, armProperties[1].Item2);
            if (teslaArm != -1)
                DrawArm(spriteBatch, Main.npc[teslaArm].Center, screenPos, armGlowmaskColor, armProperties[2].Item1, armProperties[2].Item2);
            if (plasmaArm != -1)
                DrawArm(spriteBatch, Main.npc[plasmaArm].Center, screenPos, armGlowmaskColor, armProperties[3].Item1, armProperties[3].Item2);

            // Draw fake arms if Ares is being drawn as a bestiary icon.
            if (NPC.IsABestiaryIconDummy)
            {
                DrawArm(spriteBatch, NPC.Center + NPC.scale * new Vector2(-300f, 200f), screenPos, armGlowmaskColor, -1, true);
                DrawArm(spriteBatch, NPC.Center + NPC.scale * new Vector2(-400f, 300f), screenPos, armGlowmaskColor, -1, false);

                DrawArm(spriteBatch, NPC.Center + NPC.scale * new Vector2(300f, 200f), screenPos, armGlowmaskColor, 1, true);
                DrawArm(spriteBatch, NPC.Center + NPC.scale * new Vector2(400f, 300f), screenPos, armGlowmaskColor, 1, false);
            }

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Rectangle frame = new Rectangle(NPC.width * frameX, NPC.height * frameY, NPC.width, NPC.height);
            Vector2 vector = new Vector2(NPC.width / 2, NPC.height / 2);
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
                    spriteBatch.Draw(texture, afterimageCenter, NPC.frame, afterimageColor, NPC.oldRot[i], vector, NPC.scale, SpriteEffects.None, 0f);
                }
            }

            Vector2 center = NPC.Center - screenPos;
            spriteBatch.Draw(texture, center, frame, NPC.GetAlpha(drawColor), NPC.rotation, vector, NPC.scale, SpriteEffects.None, 0f);

            texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresBodyGlow").Value;

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
                    spriteBatch.Draw(texture, afterimageCenter, NPC.frame, afterimageColor, NPC.oldRot[i], vector, NPC.scale, SpriteEffects.None, 0f);
                }
            }

            spriteBatch.Draw(texture, center, frame, afterimageBaseColor * NPC.Opacity, NPC.rotation, vector, NPC.scale, SpriteEffects.None, 0f);

            // Draw Aergia Neuron boobs if Exo Mechdusa
            if (exoMechdusa)
            {
                Texture2D neurontexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/AergiaNeuron").Value;
                Texture2D glowtexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/AergiaNeuron_Glow").Value;
                Vector2 NeuronRight = new Vector2(NPC.Center.X + 40, NPC.Center.Y + 50);
                Vector2 NeuronLeft = new Vector2(NPC.Center.X - 40, NPC.Center.Y + 50);
                Vector2 origin = new Vector2((float)(neurontexture.Width / 2), (float)(neurontexture.Height / 2));
                spriteBatch.Draw(neurontexture, NeuronRight - Main.screenPosition, null, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(neurontexture, NeuronLeft - Main.screenPosition, null, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(glowtexture, NeuronRight - Main.screenPosition, null, Color.White, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(glowtexture, NeuronLeft - Main.screenPosition, null, Color.White, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            }

            return false;
        }
        internal float WidthFunction(float completionRatio)
        {
            return MathHelper.Lerp(0.5f, 1.3f, (float)Math.Sin(MathHelper.Pi * completionRatio)) * NPC.scale;
        }

        internal Color ColorFunction(float completionRatio)
        {
            Color baseColor1 = EnragedState == (float)Enraged.Yes ? Color.Red : Color.Cyan;
            Color baseColor2 = EnragedState == (float)Enraged.Yes ? Color.IndianRed : Color.Cyan;

            float fadeToWhite = MathHelper.Lerp(0f, 0.65f, (float)Math.Sin(MathHelper.TwoPi * completionRatio + Main.GlobalTimeWrappedHourly * 4f) * 0.5f + 0.5f);
            Color baseColor = Color.Lerp(baseColor1, Color.White, fadeToWhite);
            Color color = Color.Lerp(baseColor, baseColor2, ((float)Math.Sin(MathHelper.Pi * completionRatio + Main.GlobalTimeWrappedHourly * 4f) * 0.5f + 0.5f) * 0.8f) * 0.65f;
            color.A = 84;
            if (NPC.Opacity <= 0f)
                return Color.Transparent;
            return color;
        }

        internal float BackgroundWidthFunction(float completionRatio) => WidthFunction(completionRatio) * 4f;

        public Color BackgroundColorFunction(float completionRatio)
        {
            Color backgroundColor = EnragedState == (float)Enraged.Yes ? Color.Crimson : Color.CornflowerBlue;
            Color color = backgroundColor * NPC.Opacity * 0.4f;
            return color;
        }

        public void DrawArm(SpriteBatch spriteBatch, Vector2 handPosition, Vector2 screenOffset, Color glowmaskColor, int direction, bool backArm)
        {
            SpriteEffects spriteDirection = direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float distanceFromHand = NPC.Distance(handPosition);
            float frameTime = Main.GlobalTimeWrappedHourly * 0.9f % 1f;

            // Draw back arms.
            if (backArm)
            {
                Texture2D shoulderTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresArmTopShoulder").Value;
                Texture2D armTexture1 = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresArmTopPart1").Value;
                Texture2D armSegmentTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresArmTopSegment").Value;
                Texture2D armTexture2 = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresArmTopPart2").Value;

                Texture2D shoulderGlowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresArmTopShoulderGlow").Value;
                Texture2D armSegmentGlowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresArmTopSegmentGlow").Value;
                Texture2D armGlowmask2 = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresArmTopPart2Glow").Value;

                Vector2 shoulderDrawPosition = NPC.Center + NPC.scale * new Vector2(direction * 176f, -100f);
                Vector2 arm1DrawPosition = shoulderDrawPosition + NPC.scale * new Vector2(direction * (shoulderTexture.Width + 16f), 10f);
                Vector2 armSegmentDrawPosition = arm1DrawPosition;

                // Determine frames.
                Rectangle shoulderFrame = shoulderTexture.Frame(1, 9, 0, (int)(frameTime * 9f));
                Rectangle armSegmentFrame = armSegmentTexture.Frame(1, 9, 0, (int)(frameTime * 9f));
                Rectangle arm2Frame = armTexture2.Frame(1, 9, 0, (int)(frameTime * 9f));

                Vector2 arm1Origin = armTexture1.Size() * new Vector2((direction == 1).ToInt(), 0.5f);
                Vector2 arm2Origin = arm2Frame.Size() * new Vector2((direction == 1).ToInt(), 0.5f);

                float arm1Rotation = MathHelper.Clamp(distanceFromHand * direction / 1200f, -0.12f, 0.12f);
                float arm2Rotation = (handPosition - armSegmentDrawPosition - Vector2.UnitY * 12f).ToRotation();
                if (direction == 1)
                    arm2Rotation += MathHelper.Pi;
                float armSegmentRotation = arm2Rotation;

                // Handle offsets for points.
                armSegmentDrawPosition += arm1Rotation.ToRotationVector2() * NPC.scale * direction * -14f;
                armSegmentDrawPosition -= arm2Rotation.ToRotationVector2() * NPC.scale * direction * 20f;
                Vector2 arm2DrawPosition = armSegmentDrawPosition;
                arm2DrawPosition -= arm2Rotation.ToRotationVector2() * direction * NPC.scale * 40f;
                arm2DrawPosition += (arm2Rotation - MathHelper.PiOver2).ToRotationVector2() * NPC.scale * 14f;

                // Calculate colors.
                Color shoulderLightColor = NPC.GetAlpha(Lighting.GetColor((int)shoulderDrawPosition.X / 16, (int)shoulderDrawPosition.Y / 16));
                Color arm1LightColor = NPC.GetAlpha(Lighting.GetColor((int)arm1DrawPosition.X / 16, (int)arm1DrawPosition.Y / 16));
                Color armSegmentLightColor = NPC.GetAlpha(Lighting.GetColor((int)armSegmentDrawPosition.X / 16, (int)armSegmentDrawPosition.Y / 16));
                Color arm2LightColor = NPC.GetAlpha(Lighting.GetColor((int)arm2DrawPosition.X / 16, (int)arm2DrawPosition.Y / 16));
                Color glowmaskAlphaColor = NPC.GetAlpha(glowmaskColor);

                // Draw electricity between arms.
                if (NPC.Opacity > 0f && !NPC.IsABestiaryIconDummy)
                {
                    List<Vector2> arm2ElectricArcPoints = AresTeslaOrb.DetermineElectricArcPoints(armSegmentDrawPosition, arm2DrawPosition + arm2Rotation.ToRotationVector2() * -direction * 20f, 250290787);
                    PrimitiveSet.Prepare(arm2ElectricArcPoints, new(BackgroundWidthFunction, BackgroundColorFunction, smoothen: false), 90);
                    PrimitiveSet.Prepare(arm2ElectricArcPoints, new(WidthFunction, ColorFunction, smoothen: false), 90);

                    // Draw electricity between the final arm and the hand.
                    List<Vector2> handElectricArcPoints = AresTeslaOrb.DetermineElectricArcPoints(arm2DrawPosition - arm2Rotation.ToRotationVector2() * direction * 100f, handPosition, 27182);
                    PrimitiveSet.Prepare(handElectricArcPoints, new(BackgroundWidthFunction, BackgroundColorFunction, smoothen: false), 90);
                    PrimitiveSet.Prepare(handElectricArcPoints, new(WidthFunction, ColorFunction, smoothen: false), 90);
                }

                shoulderDrawPosition += Vector2.UnitY * NPC.gfxOffY - screenOffset;
                arm1DrawPosition += Vector2.UnitY * NPC.gfxOffY - screenOffset;
                armSegmentDrawPosition += Vector2.UnitY * NPC.gfxOffY - screenOffset;
                arm2DrawPosition += Vector2.UnitY * NPC.gfxOffY - screenOffset;

                spriteBatch.Draw(armTexture1, arm1DrawPosition, null, arm1LightColor, arm1Rotation, arm1Origin, NPC.scale, spriteDirection ^ SpriteEffects.FlipHorizontally, 0f);
                spriteBatch.Draw(shoulderTexture, shoulderDrawPosition, shoulderFrame, shoulderLightColor, 0f, shoulderFrame.Size() * 0.5f, NPC.scale, spriteDirection, 0f);
                spriteBatch.Draw(shoulderGlowmask, shoulderDrawPosition, shoulderFrame, glowmaskAlphaColor, 0f, shoulderFrame.Size() * 0.5f, NPC.scale, spriteDirection, 0f);
                spriteBatch.Draw(armSegmentTexture, armSegmentDrawPosition, armSegmentFrame, armSegmentLightColor, armSegmentRotation, armSegmentFrame.Size() * 0.5f, NPC.scale, spriteDirection ^ SpriteEffects.FlipHorizontally, 0f);
                spriteBatch.Draw(armSegmentGlowmask, armSegmentDrawPosition, armSegmentFrame, glowmaskAlphaColor, armSegmentRotation, armSegmentFrame.Size() * 0.5f, NPC.scale, spriteDirection ^ SpriteEffects.FlipHorizontally, 0f);
                spriteBatch.Draw(armTexture2, arm2DrawPosition, arm2Frame, arm2LightColor, arm2Rotation, arm2Origin, NPC.scale, spriteDirection ^ SpriteEffects.FlipVertically, 0f);
                spriteBatch.Draw(armGlowmask2, arm2DrawPosition, arm2Frame, glowmaskAlphaColor, arm2Rotation, arm2Origin, NPC.scale, spriteDirection ^ SpriteEffects.FlipVertically, 0f);
            }
            else
            {
                Texture2D shoulderTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresBottomArmShoulder").Value;
                Texture2D connectorTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresBottomArmConnector").Value;
                Texture2D armTexture1 = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresBottomArmPart1").Value;
                Texture2D armTexture2 = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresBottomArmPart2").Value;

                Texture2D shoulderGlowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresBottomArmShoulderGlow").Value;
                Texture2D armTexture1Glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresBottomArmPart1Glow").Value;
                Texture2D armTexture2Glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Ares/AresBottomArmPart2Glow").Value;

                Vector2 shoulderDrawPosition = NPC.Center + NPC.scale * new Vector2(direction * 110f, -54f);
                Vector2 connectorDrawPosition = shoulderDrawPosition + NPC.scale * new Vector2(direction * 20f, 32f);
                Vector2 arm1DrawPosition = shoulderDrawPosition + NPC.scale * Vector2.UnitX * direction * 20f;

                // Determine frames.
                Rectangle arm1Frame = armTexture1.Frame(1, 9, 0, (int)(frameTime * 9f));
                Rectangle shoulderFrame = shoulderTexture.Frame(1, 9, 0, (int)(frameTime * 9f));
                Rectangle arm2Frame = armTexture2.Frame(1, 9, 0, (int)(frameTime * 9f));

                Vector2 arm1Origin = arm1Frame.Size() * new Vector2((direction == 1).ToInt(), 0.5f);
                Vector2 arm2Origin = arm2Frame.Size() * new Vector2((direction == 1).ToInt(), 0.5f);

                float arm1Rotation = CalamityUtils.WrapAngle90Degrees((handPosition - shoulderDrawPosition).ToRotation()) * 0.5f;
                connectorDrawPosition += arm1Rotation.ToRotationVector2() * NPC.scale * direction * -26f;
                arm1DrawPosition += arm1Rotation.ToRotationVector2() * NPC.scale * direction * (armTexture1.Width - 14f);
                float arm2Rotation = CalamityUtils.WrapAngle90Degrees((handPosition - arm1DrawPosition).ToRotation());

                Vector2 arm2DrawPosition = arm1DrawPosition + arm2Rotation.ToRotationVector2() * NPC.scale * direction * (armTexture2.Width + 16f) - Vector2.UnitY * 16f;

                // Calculate colors.
                Color shoulderLightColor = NPC.GetAlpha(Lighting.GetColor((int)shoulderDrawPosition.X / 16, (int)shoulderDrawPosition.Y / 16));
                Color arm1LightColor = NPC.GetAlpha(Lighting.GetColor((int)arm1DrawPosition.X / 16, (int)arm1DrawPosition.Y / 16));
                Color arm2LightColor = NPC.GetAlpha(Lighting.GetColor((int)arm2DrawPosition.X / 16, (int)arm2DrawPosition.Y / 16));
                Color glowmaskAlphaColor = NPC.GetAlpha(glowmaskColor);

                // Draw electricity between arms.
                if (NPC.Opacity > 0f && !NPC.IsABestiaryIconDummy)
                {
                    List<Vector2> arm2ElectricArcPoints = AresTeslaOrb.DetermineElectricArcPoints(arm1DrawPosition - arm2Rotation.ToRotationVector2() * direction * 10f, arm1DrawPosition + arm2Rotation.ToRotationVector2() * direction * 20f, 31416);
                    PrimitiveSet.Prepare(arm2ElectricArcPoints, new(BackgroundWidthFunction, BackgroundColorFunction, smoothen: false), 90);
                    PrimitiveSet.Prepare(arm2ElectricArcPoints, new(WidthFunction, ColorFunction, smoothen: false), 90);

                    // Draw electricity between the final arm and the hand.
                    List<Vector2> handElectricArcPoints = AresTeslaOrb.DetermineElectricArcPoints(arm2DrawPosition - arm2Rotation.ToRotationVector2() * direction * 20f, handPosition, 27182);
                    PrimitiveSet.Prepare(handElectricArcPoints, new(BackgroundWidthFunction, BackgroundColorFunction, smoothen: false), 90);
                    PrimitiveSet.Prepare(handElectricArcPoints, new(WidthFunction, ColorFunction, smoothen: false), 90);
                }

                shoulderDrawPosition += Vector2.UnitY * NPC.gfxOffY - screenOffset;
                connectorDrawPosition += Vector2.UnitY * NPC.gfxOffY - screenOffset;
                arm1DrawPosition += Vector2.UnitY * NPC.gfxOffY - screenOffset;
                arm2DrawPosition += Vector2.UnitY * NPC.gfxOffY - screenOffset;

                spriteBatch.Draw(shoulderTexture, shoulderDrawPosition, shoulderFrame, shoulderLightColor, arm1Rotation, shoulderFrame.Size() * 0.5f, NPC.scale, spriteDirection ^ SpriteEffects.FlipHorizontally, 0f);
                spriteBatch.Draw(shoulderGlowmask, shoulderDrawPosition, shoulderFrame, glowmaskAlphaColor, arm1Rotation, shoulderFrame.Size() * 0.5f, NPC.scale, spriteDirection ^ SpriteEffects.FlipHorizontally, 0f);
                spriteBatch.Draw(connectorTexture, connectorDrawPosition, null, shoulderLightColor, 0f, connectorTexture.Size() * 0.5f, NPC.scale, spriteDirection ^ SpriteEffects.FlipHorizontally, 0f);
                spriteBatch.Draw(armTexture1, arm1DrawPosition, arm1Frame, arm1LightColor, arm1Rotation, arm1Origin, NPC.scale, spriteDirection ^ SpriteEffects.FlipHorizontally, 0f);
                spriteBatch.Draw(armTexture1Glowmask, arm1DrawPosition, arm1Frame, glowmaskAlphaColor, arm1Rotation, arm1Origin, NPC.scale, spriteDirection ^ SpriteEffects.FlipHorizontally, 0f);
                spriteBatch.Draw(armTexture2, arm2DrawPosition, arm2Frame, arm2LightColor, arm2Rotation, arm2Origin, NPC.scale, spriteDirection ^ SpriteEffects.FlipHorizontally, 0f);
                spriteBatch.Draw(armTexture2Glowmask, arm2DrawPosition, arm2Frame, glowmaskAlphaColor, arm2Rotation, arm2Origin, NPC.scale, spriteDirection ^ SpriteEffects.FlipHorizontally, 0f);
            }
        }
        public override void ModifyTypeName(ref string typeName)
        {
            if (exoMechdusa)
            {
                typeName = this.GetLocalizedValue("HekateName");
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<OmegaHealingPotion>();
        }

        public override void OnKill()
        {
            // Check if the other exo mechs are alive
            bool exoWormAlive = false;
            bool exoTwinsAlive = false;
            if (SoundEngine.TryGetActiveSound(DeathraySoundSlot, out var deathraySound) && deathraySound.IsPlaying)
                deathraySound?.Stop();
            if (CalamityGlobalNPC.draedonExoMechWorm != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedonExoMechWorm].active)
                    exoWormAlive = true;
            }
            if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].active)
                    exoTwinsAlive = true;
            }

            // Check for Draedon
            bool draedonAlive = false;
            if (CalamityGlobalNPC.draedon != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedon].active)
                    draedonAlive = true;
            }

            // Phase 5, when 1 mech dies and the other 2 return to fight
            if (exoWormAlive && exoTwinsAlive)
            {
                if (draedonAlive)
                {
                    Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 4f;
                    Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
                }
            }

            // Phase 7, when 1 mech dies and the final one returns to the fight
            else if (exoWormAlive || exoTwinsAlive)
            {
                if (draedonAlive)
                {
                    Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 6f;
                    Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
                }
            }
            else
                AresBody.DoMiscDeathEffects(NPC, MechType.Ares);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => DefineExoMechLoot(NPC, npcLoot, (int)MechType.Ares);

        public static bool CanDropLoot()
        {
            return NPC.CountNPCS(ModContent.NPCType<ThanatosHead>()) +
                NPC.CountNPCS(ModContent.NPCType<AresBody>()) +
                NPC.CountNPCS(ModContent.NPCType<Apollo.Apollo>()) <= 1;
        }

        public static void DoMiscDeathEffects(NPC npc, MechType mechType)
        {
            CalamityGlobalNPC.SetNewBossJustDowned(npc);

            switch (mechType)
            {
                case MechType.Thanatos:
                    DownedBossSystem.downedThanatos = true;
                    DownedBossSystem.downedExoMechs = true;
                    break;
                case MechType.Ares:
                    DownedBossSystem.downedAres = true;
                    DownedBossSystem.downedExoMechs = true;
                    break;
                case MechType.ArtemisAndApollo:
                    DownedBossSystem.downedArtemisAndApollo = true;
                    DownedBossSystem.downedExoMechs = true;
                    break;
            }
            CalamityNetcode.SyncWorld();
        }

        public static void DefineExoMechLoot(NPC npc, NPCLoot npcLoot, int mechType)
        {
            var mainDrops = npcLoot.DefineConditionalDropSet(CanDropLoot);
            LeadingConditionRule normalOnly = new LeadingConditionRule(new Conditions.NotExpert());
            mainDrops.Add(normalOnly);

            bool ThanatosLoot(DropAttemptInfo info) => info.npc.type == ModContent.NPCType<ThanatosHead>() || DownedBossSystem.downedThanatos;
            bool AresLoot(DropAttemptInfo info) => info.npc.type == ModContent.NPCType<AresBody>() || DownedBossSystem.downedAres;
            bool ApolloLoot(DropAttemptInfo info) => info.npc.type == ModContent.NPCType<Apollo.Apollo>() || DownedBossSystem.downedArtemisAndApollo;

            // Trophies
            mainDrops.Add(ItemDropRule.ByCondition(DropHelper.If(info => info.npc.type == ModContent.NPCType<ThanatosHead>()), ModContent.ItemType<ThanatosTrophy>()));
            mainDrops.Add(ItemDropRule.ByCondition(DropHelper.If(info => info.npc.type == ModContent.NPCType<AresBody>()), ModContent.ItemType<AresTrophy>()));
            mainDrops.Add(ItemDropRule.ByCondition(DropHelper.If(info => info.npc.type == ModContent.NPCType<Apollo.Apollo>()), ModContent.ItemType<ArtemisTrophy>()));
            mainDrops.Add(ItemDropRule.ByCondition(DropHelper.If(info => info.npc.type == ModContent.NPCType<Apollo.Apollo>()), ModContent.ItemType<ApolloTrophy>()));

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).AddIf(CanDropLoot, ModContent.ItemType<DraedonRelic>());

            // GFB Broken Water Filter
            var GFBOnly = npcLoot.DefineConditionalDropSet(DropHelper.GFB);
            {
                GFBOnly.Add(ModContent.ItemType<BrokenWaterFilter>(), hideLootReport: true);
            }

            // Lore item
            mainDrops.Add(ItemDropRule.ByCondition(DropHelper.If(() => !DownedBossSystem.downedExoMechs, desc: DropHelper.FirstKillText), ModContent.ItemType<LoreExoMechs>()));

            // Cynosure: If SCal has been defeated and this is the first kill of the Exo Mechs, drop the special lore item
            mainDrops.Add(ItemDropRule.ByCondition(
                DropHelper.If(
                    () => !DownedBossSystem.downedExoMechs && DownedBossSystem.downedCalamitas,
                    desc: DropHelper.CynosureText),
                ModContent.ItemType<LoreCynosure>()
            ));

            // Treasure bag
            npcLoot.Add(ItemDropRule.BossBagByCondition(DropHelper.If(CanDropLoot), ModContent.ItemType<DraedonBag>()));

            // Legendary seed soup
            mainDrops.Add(ItemDropRule.ByCondition(DropHelper.If(info => info.npc.type == ModContent.NPCType<AresBody>() && info.npc.ModNPC<Ares.AresBody>().exoMechdusa), ModContent.ItemType<Fabsoup>()), hideLootReport: true);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                // Materials
                normalOnly.Add(ModContent.ItemType<ExoPrism>(), 1, 25, 30);

                // Weapons
                // Higher chance due to how the drops work

                // Thanatos weapons
                normalOnly.Add(ItemDropRule.ByCondition(DropHelper.If(ThanatosLoot), ModContent.ItemType<SpineOfThanatos>()));
                normalOnly.Add(ItemDropRule.ByCondition(DropHelper.If(ThanatosLoot), ModContent.ItemType<RefractionRotor>()));
                normalOnly.Add(ItemDropRule.ByCondition(DropHelper.If(ThanatosLoot), ModContent.ItemType<AtlasMunitionsBeacon>()));

                // Ares weapons
                normalOnly.Add(ItemDropRule.ByCondition(DropHelper.If(AresLoot), ModContent.ItemType<PhotonRipper>()));
                normalOnly.Add(ItemDropRule.ByCondition(DropHelper.If(AresLoot), ModContent.ItemType<TheJailor>()));
                normalOnly.Add(ItemDropRule.ByCondition(DropHelper.If(AresLoot), ModContent.ItemType<AresExoskeleton>()));

                // Twins weapons
                normalOnly.Add(ItemDropRule.ByCondition(DropHelper.If(ApolloLoot), ModContent.ItemType<TheAtomSplitter>()));
                normalOnly.Add(ItemDropRule.ByCondition(DropHelper.If(ApolloLoot), ModContent.ItemType<SurgeDriver>()));

                // Equipment
                normalOnly.Add(ModContent.ItemType<ExoThrone>());
                normalOnly.Add(ModContent.ItemType<DraedonsHeart>());

                // Vanity
                // Higher chance due to how the drops work
                normalOnly.Add(ModContent.ItemType<DraedonMask>(), 3);
                normalOnly.Add(ItemDropRule.ByCondition(DropHelper.If(ThanatosLoot), ModContent.ItemType<ThanatosMask>(), 7, chanceNumerator: 2));
                normalOnly.Add(ItemDropRule.ByCondition(DropHelper.If(AresLoot), ModContent.ItemType<AresMask>(), 7, chanceNumerator: 2));
                normalOnly.Add(ItemDropRule.ByCondition(DropHelper.If(ApolloLoot), ModContent.ItemType<ArtemisMask>(), 7, chanceNumerator: 2));
                normalOnly.Add(ItemDropRule.ByCondition(DropHelper.If(ApolloLoot), ModContent.ItemType<ApolloMask>(), 7, chanceNumerator: 2));
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
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
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AresBody1").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AresBody2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AresBody3").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AresBody4").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AresBody5").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AresBody6").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AresBody7").Type, 1f);
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
