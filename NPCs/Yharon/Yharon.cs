using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture.Trophies;
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
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Tiles.Ores;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Yharon
{
    [AutoloadBossHead]
    public class Yharon : ModNPC
    {
        private Rectangle safeBox = default;
        private Vector2 flareDustBulletHellSpawn = default;

        private bool enraged = false;
        private bool protectionBoost = false;
        private bool moveCloser = false;
        private bool useTornado = true;
        private int secondPhasePhase = 1;
        private int teleportLocation = 0;
        private bool startSecondAI = false;
        private bool spawnArena = false;
        private int invincibilityCounter = 0;
        private int fastChargeTelegraphTime = 120;

        private const float ai2GateValue = 0.55f;
        public const int Phase2InvincibilityTime = 300;

        public static float normalDR = 0.22f;
        public static float EnragedDR = 0.9f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Jungle Dragon, Yharon");
            Main.npcFrameCount[npc.type] = 7;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.Calamity().canBreakPlayerDefense = true;
            npc.npcSlots = 50f;
            npc.GetNPCDamage();
            npc.width = 200;
            npc.height = 200;
            npc.defense = 90;
            npc.LifeMaxNERB(1085000, 1300000, 370000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Item.buyPrice(1, 0, 0, 0);
            npc.boss = true;
            npc.DR_NERD(normalDR);

            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.netAlways = true;

            music = CalamityMod.Instance.GetMusicFromMusicMod("YharonP1") ?? MusicID.Boss3;

            npc.HitSound = SoundID.NPCHit56;
            npc.DeathSound = SoundID.NPCDeath60;
            bossBag = ModContent.ItemType<YharonBag>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToSickness = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            BitsByte bb = new BitsByte();
            bb[0] = enraged;
            bb[1] = protectionBoost;
            bb[2] = moveCloser;
            bb[3] = useTornado;
            bb[4] = startSecondAI;
            bb[5] = npc.dontTakeDamage;
            writer.Write(bb);
            writer.Write(secondPhasePhase);
            writer.Write(teleportLocation);
            writer.Write(invincibilityCounter);
            writer.Write(fastChargeTelegraphTime);
            writer.WriteVector2(flareDustBulletHellSpawn);
            writer.Write(safeBox.X);
            writer.Write(safeBox.Y);
            writer.Write(safeBox.Width);
            writer.Write(safeBox.Height);
            writer.Write(npc.localAI[0]);
            writer.Write(npc.localAI[1]);
            writer.Write(npc.localAI[2]);
            writer.Write(npc.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            BitsByte bb = reader.ReadByte();
            enraged = bb[0];
            protectionBoost = bb[1];
            moveCloser = bb[2];
            useTornado = bb[3];
            startSecondAI = bb[4];
            npc.dontTakeDamage = bb[5];
            secondPhasePhase = reader.ReadInt32();
            teleportLocation = reader.ReadInt32();
            invincibilityCounter = reader.ReadInt32();
            fastChargeTelegraphTime = reader.ReadInt32();
            flareDustBulletHellSpawn = reader.ReadVector2();
            safeBox.X = reader.ReadInt32();
            safeBox.Y = reader.ReadInt32();
            safeBox.Width = reader.ReadInt32();
            safeBox.Height = reader.ReadInt32();
            npc.localAI[0] = reader.ReadSingle();
            npc.localAI[1] = reader.ReadSingle();
            npc.localAI[2] = reader.ReadSingle();
            npc.localAI[3] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Stop rain
            CalamityMod.StopRain();

            // Variables
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            float pie = (float)Math.PI;

            Vector2 vectorCenter = npc.Center;

            // Start phase 2 or not
            if (startSecondAI)
            {
                Yharon_AI2(expertMode, revenge, death, malice, pie, lifeRatio, vectorCenter, calamityGlobalNPC);
                return;
            }

            // Phase booleans
            float phase2GateValue = revenge ? 0.9f : expertMode ? 0.85f : 0.75f;
            bool phase2Check = death || lifeRatio <= phase2GateValue;
            bool phase3Check = lifeRatio <= (death ? 0.8f : revenge ? 0.75f : expertMode ? 0.7f : 0.625f);
            bool phase4Check = lifeRatio <= ai2GateValue;
            bool phase1Change = npc.ai[0] > -1f;
            bool phase2Change = npc.ai[0] > 5f;
            bool phase3Change = npc.ai[0] > 12f;

            // Timer, velocity and acceleration for idle phase before phase switch
            int phaseSwitchTimer = expertMode ? 36 : 40;
            float acceleration = expertMode ? 0.75f : 0.7f;
            float velocity = expertMode ? 12f : 11f;

            // Damage immunity
            if (phase3Change)
                npc.dontTakeDamage = phase4Check;
            else if (phase2Change)
                npc.dontTakeDamage = phase3Check;
            else if (phase1Change)
                npc.dontTakeDamage = phase2Check;

            if (phase3Change)
            {
                acceleration = expertMode ? 0.85f : 0.8f;
                velocity = expertMode ? 14f : 13f;
                phaseSwitchTimer = expertMode ? 25 : 28;
                fastChargeTelegraphTime = 100;
            }
            else if (phase2Change)
            {
                acceleration = expertMode ? 0.8f : 0.75f;
                velocity = expertMode ? 13f : 12f;
                phaseSwitchTimer = expertMode ? 32 : 36;
                fastChargeTelegraphTime = 110;
            }
            else
                phaseSwitchTimer = 25;

            // Timers and velocity for charging
            float reduceSpeedChargeDistance = 540f;
            int chargeTime = expertMode ? 40 : 45;
            float chargeSpeed = expertMode ? 28f : 26f;
            float fastChargeVelocityMultiplier = malice ? 2f : 1.5f;
            bool playFastChargeRoarSound = npc.localAI[1] == fastChargeTelegraphTime * 0.5f;
            bool doFastCharge = npc.localAI[1] > fastChargeTelegraphTime;

            if (phase3Change)
            {
                chargeTime = 35;
                chargeSpeed = 30f;
            }
            else if (phase2Change)
            {
                chargeTime = expertMode ? 38 : 43;

                if (expertMode)
                    chargeSpeed = 28.5f;
            }

            if (revenge)
            {
                int chargeTimeDecrease = malice ? 6 : death ? 4 : 2;
                float velocityMult = malice ? 1.15f : death ? 1.1f : 1.05f;
                phaseSwitchTimer -= chargeTimeDecrease;
                acceleration *= velocityMult;
                velocity *= velocityMult;
                chargeTime -= chargeTimeDecrease;
                chargeSpeed *= velocityMult;
            }

            float reduceSpeedFlareBombDistance = 570f;
            int flareBombPhaseTimer = malice ? 30 : death ? 40 : 60;
            int flareBombSpawnDivisor = flareBombPhaseTimer / 20;
            float flareBombPhaseAcceleration = malice ? 1f : death ? 0.92f : 0.8f;
            float flareBombPhaseVelocity = malice ? 16f : death ? 14f : 12f;

            int fireTornadoPhaseTimer = 90;

            int newPhaseTimer = 180;

            float flareDustPhaseScalar = death ? 48f : 60f;
            phase2GateValue -= ai2GateValue; // 0.35, 70% HP = 0.7 - 0.55 = 0.15, 0.35 - 0.15 = 0.2 / 0.35 = 0.57, 60% HP = 0.6 - 0.55 = 0.05, 0.35 - 0.05 = 0.3 / 0.35 = 0.85, 80% HP = 0.8 - 0.55 = 0.25, 0.35 - 0.25 = 0.1 / 0.35 = 0.28
            int flareDustPhaseTimerReduction = revenge ? (int)(flareDustPhaseScalar * ((phase2GateValue - (lifeRatio - ai2GateValue)) / phase2GateValue)) : 0;
            int flareDustPhaseTimer = (malice ? 210 : death ? 240 : 300) - flareDustPhaseTimerReduction;
            int flareDustPhaseTimer2 = (malice ? 105 : death ? 120 : 150) - (flareDustPhaseTimerReduction / 2);

            float spinTime = flareDustPhaseTimer / 2;

            int flareDustSpawnDivisor = flareDustPhaseTimer / 10;
            int flareDustSpawnDivisor2 = flareDustPhaseTimer2 / 30;
            int flareDustSpawnDivisor3 = flareDustPhaseTimer / 25;

            float spinPhaseVelocity = 25f;
            float spinPhaseRotation = MathHelper.TwoPi * 3 / spinTime;

            float increasedIdleTimeAfterBulletHell = -120f;

            float teleportPhaseTimer = 30f;

            int spawnPhaseTimer = 75;

            // Target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest();
                npc.netUpdate = true;
            }

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, vectorCenter) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            Player player = Main.player[npc.target];

            // Despawn
            if (player.dead || !player.active)
            {
                npc.TargetClosest();
                player = Main.player[npc.target];
                if (player.dead || !player.active)
                {
                    npc.velocity.Y -= 0.4f;

                    if (npc.timeLeft > 60)
                        npc.timeLeft = 60;

                    if (npc.ai[0] > 12f)
                        npc.ai[0] = 13f;
                    else if (npc.ai[0] > 5f)
                        npc.ai[0] = 6f;
                    else
                        npc.ai[0] = 0f;

                    npc.ai[2] = 0f;
                }
            }
            else if (npc.timeLeft < 1800)
                npc.timeLeft = 1800;

            int xPos = 60 * npc.direction;
            Vector2 vector = Vector2.Normalize(player.Center - vectorCenter) * (npc.width + 20) / 2f + vectorCenter;
            Vector2 fromMouth = new Vector2((int)vector.X + xPos, (int)vector.Y - 15);

            // Create the arena, but not as a multiplayer client.
            // In single player, the arena gets created and never gets synced because it's single player.
            // In multiplayer, only the server/host creates the arena, and everyone else receives it on the next frame via SendExtraAI.
            // Everyone however sets spawnArena to true to confirm that the fight has started.
            if (!spawnArena)
            {
                spawnArena = true;
                enraged = false;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    safeBox.X = (int)(player.Center.X - (malice ? 2000f : revenge ? 3000f : 3500f));
                    safeBox.Y = (int)(player.Center.Y - 10500f);
                    safeBox.Width = malice ? 4000 : revenge ? 6000 : 7000;
                    safeBox.Height = 21000;
                    Projectile.NewProjectile(player.Center.X + (malice ? 2000f : revenge ? 3000f : 3500f), player.Center.Y + 100f, 0f, 0f, ModContent.ProjectileType<SkyFlareRevenge>(), 0, 0f, Main.myPlayer);
                    Projectile.NewProjectile(player.Center.X - (malice ? 2000f : revenge ? 3000f : 3500f), player.Center.Y + 100f, 0f, 0f, ModContent.ProjectileType<SkyFlareRevenge>(), 0, 0f, Main.myPlayer);
                }

                // Force Yharon to send a sync packet so that the arena gets sent immediately
                npc.netUpdate = true;
            }
            // Enrage code doesn't run on frame 1 so that Yharon won't be enraged for 1 frame in multiplayer
            else
            {
                enraged = !player.Hitbox.Intersects(safeBox);
                npc.Calamity().CurrentlyEnraged = enraged;
                if (enraged)
                {
                    phaseSwitchTimer = 15;
                    protectionBoost = true;
                    npc.damage = npc.defDamage * 5;
                    chargeSpeed += 25f;
                }
                else
                {
                    npc.damage = npc.defDamage;
                    protectionBoost = false;
                }
            }

            // Set DR based on protection boost (aka enrage)
            bool chargeTelegraph = (npc.ai[0] == 0f || npc.ai[0] == 6f || npc.ai[0] == 13f) && npc.localAI[1] > 0f;
            bool bulletHell = npc.ai[0] == 8f || npc.ai[0] == 15f;
            calamityGlobalNPC.DR = protectionBoost ? EnragedDR : normalDR;

            if (bulletHell)
                npc.damage = 0;

            // Trigger spawn effects
            if (npc.localAI[0] == 0f)
            {
                npc.localAI[0] = 1f;
                npc.alpha = 255;
                npc.rotation = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ai[0] = -1f;
                    npc.netUpdate = true;
                }
            }

            // Rotation
            float npcRotation = (float)Math.Atan2(player.Center.Y - vectorCenter.Y, player.Center.X - vectorCenter.X);
            if (npc.spriteDirection == 1)
                npcRotation += pie;
            if (npcRotation < 0f)
                npcRotation += MathHelper.TwoPi;
            if (npcRotation > MathHelper.TwoPi)
                npcRotation -= MathHelper.TwoPi;
            if (npc.ai[0] == -1f || npc.ai[0] == 3f || npc.ai[0] == 4f || npc.ai[0] == 9f || npc.ai[0] == 10f || npc.ai[0] == 16f)
                npcRotation = 0f;

            float npcRotationSpeed = 0.04f;
            if (npc.ai[0] == 1f || npc.ai[0] == 5f || npc.ai[0] == 7f || npc.ai[0] == 8f || npc.ai[0] == 11f || npc.ai[0] == 12f ||
                npc.ai[0] == 14f || npc.ai[0] == 15f || npc.ai[0] == 18f || npc.ai[0] == 19f)
                npcRotationSpeed = 0f;
            if (npc.ai[0] == 3f || npc.ai[0] == 4f || npc.ai[0] == 9f || npc.ai[0] == 16f)
                npcRotationSpeed = 0.01f;

            if (npcRotationSpeed != 0f)
                npc.rotation = npc.rotation.AngleTowards(npcRotation, npcRotationSpeed);

            // Alpha effects
            if (npc.ai[0] != -1f && ((npc.ai[0] != 6f && npc.ai[0] != 13f) || npc.ai[2] <= phaseSwitchTimer))
            {
                bool colliding = Collision.SolidCollision(npc.position, npc.width, npc.height);

                if (colliding)
                    npc.alpha += 15;
                else
                    npc.alpha -= 15;

                if (npc.alpha < 0)
                    npc.alpha = 0;

                if (npc.alpha > 150)
                    npc.alpha = 150;
            }

            // Spawn effects
            if (npc.ai[0] == -1f)
            {
                npc.velocity *= 0.98f;

                int num1467 = Math.Sign(player.Center.X - vectorCenter.X);
                if (num1467 != 0)
                {
                    npc.direction = num1467;
                    npc.spriteDirection = -npc.direction;
                }

                if (npc.ai[2] > 20f)
                {
                    npc.velocity.Y = -2f;
                    npc.alpha -= 5;

                    bool colliding = Collision.SolidCollision(npc.position, npc.width, npc.height);

                    if (colliding)
                        npc.alpha += 15;

                    if (npc.alpha < 0)
                        npc.alpha = 0;

                    if (npc.alpha > 150)
                        npc.alpha = 150;
                }

                if (npc.ai[2] == fireTornadoPhaseTimer - 30)
                {
                    int num1468 = 72;
                    for (int num1469 = 0; num1469 < num1468; num1469++)
                    {
                        Vector2 vector169 = Vector2.Normalize(npc.velocity) * new Vector2(npc.width / 2f, npc.height) * 0.75f * 0.5f;
                        vector169 = vector169.RotatedBy((num1469 - (num1468 / 2 - 1)) * MathHelper.TwoPi / num1468) + vectorCenter;
                        Vector2 value16 = vector169 - vectorCenter;
                        int num1470 = Dust.NewDust(vector169 + value16, 0, 0, 244, value16.X * 2f, value16.Y * 2f, 100, default, 1.4f);
                        Main.dust[num1470].noGravity = true;
                        Main.dust[num1470].noLight = true;
                        Main.dust[num1470].velocity = Vector2.Normalize(value16) * 3f;
                    }

                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= spawnPhaseTimer)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }
            }

            #region Phase1
            // Phase switch
            else if (npc.ai[0] == 0f && !player.dead)
            {
                if (npc.ai[1] == 0f)
                    npc.ai[1] = Math.Sign((vectorCenter - player.Center).X);

                Vector2 destination = player.Center + new Vector2(npc.ai[1], 0);
                Vector2 distanceFromDestination = destination - vectorCenter;
                Vector2 desiredVelocity = Vector2.Normalize(distanceFromDestination - npc.velocity) * velocity;

                if (Vector2.Distance(vectorCenter, destination) > reduceSpeedChargeDistance)
                    npc.SimpleFlyMovement(desiredVelocity, acceleration);
                else
                    npc.velocity *= 0.98f;

                int num1471 = Math.Sign(player.Center.X - vectorCenter.X);
                if (num1471 != 0)
                {
                    if (npc.ai[2] == 0f && num1471 != npc.direction)
                        npc.rotation += pie;

                    npc.direction = num1471;

                    if (npc.spriteDirection != -npc.direction)
                        npc.rotation += pie;

                    npc.spriteDirection = -npc.direction;
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= phaseSwitchTimer)
                {
                    int aiState = 0;
                    switch ((int)npc.ai[3])
                    {
                        case 0:
                        case 1:
                        case 2:
                            aiState = 1;
                            break;
                        case 3:
                            aiState = 5;
                            break;
                        case 4:
                            npc.ai[3] = 1f;
                            aiState = 2;
                            break;
                        case 5:
                            npc.ai[3] = 0f;
                            aiState = 3;
                            break;
                    }

                    if (phase2Check)
                        aiState = 4;

                    if (aiState == 1)
                    {
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;

                        npc.velocity = Vector2.Normalize(player.Center - vectorCenter) * chargeSpeed;
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                        if (num1471 != 0)
                        {
                            npc.direction = num1471;

                            if (npc.spriteDirection == 1)
                                npc.rotation += pie;

                            npc.spriteDirection = -npc.direction;
                        }
                    }
                    else if (aiState == 2)
                    {
                        npc.ai[0] = 2f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else if (aiState == 3)
                    {
                        npc.ai[0] = 3f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else if (aiState == 4)
                    {
                        npc.ai[0] = 4f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else if (aiState == 5)
                    {
                        if (playFastChargeRoarSound)
                            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);

                        if (doFastCharge)
                        {
                            npc.ai[0] = 5f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                            npc.localAI[1] = 0f;

                            npc.velocity = Vector2.Normalize(player.Center - vectorCenter) * chargeSpeed * fastChargeVelocityMultiplier;
                            npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                            if (num1471 != 0)
                            {
                                npc.direction = num1471;

                                if (npc.spriteDirection == 1)
                                    npc.rotation += pie;

                                npc.spriteDirection = -npc.direction;
                            }
                        }
                        else
                            npc.localAI[1] += 1f;
                    }

                    npc.netUpdate = true;
                }
            }

            // Charge
            else if (npc.ai[0] == 1f)
            {
                ChargeDust(7, pie);

                npc.ai[2] += 1f;
                if (npc.ai[2] >= chargeTime)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 2f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Fireball breath
            else if (npc.ai[0] == 2f)
            {
                if (npc.ai[1] == 0f)
                    npc.ai[1] = Math.Sign((vectorCenter - player.Center).X);

                Vector2 destination = player.Center + new Vector2(npc.ai[1], 0);
                Vector2 value17 = destination - vectorCenter;
                Vector2 vector170 = Vector2.Normalize(value17 - npc.velocity) * flareBombPhaseVelocity;

                if (Vector2.Distance(vectorCenter, destination) > reduceSpeedFlareBombDistance)
                    npc.SimpleFlyMovement(vector170, flareBombPhaseAcceleration);
                else
                    npc.velocity *= 0.98f;

                if (npc.ai[2] == 0f)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);

                if (npc.ai[2] % flareBombSpawnDivisor == 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int type = ModContent.ProjectileType<FlareBomb>();
                        int damage = npc.GetProjectileDamage(type);
                        Projectile.NewProjectile(fromMouth, Vector2.Zero, type, damage, 0f, Main.myPlayer, npc.target, 1f);
                    }
                }

                int num1476 = Math.Sign(player.Center.X - vectorCenter.X);
                if (num1476 != 0)
                {
                    npc.direction = num1476;

                    if (npc.spriteDirection != -npc.direction)
                        npc.rotation += pie;

                    npc.spriteDirection = -npc.direction;
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= flareBombPhaseTimer)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Fire tornadoes
            else if (npc.ai[0] == 3f)
            {
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

                if (npc.ai[2] == fireTornadoPhaseTimer - 30)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);

                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == fireTornadoPhaseTimer - 30)
                {
                    Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, npc.direction * 4, 8f, ModContent.ProjectileType<Flare>(), 0, 0f, Main.myPlayer, 0f, 0f);
                    Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, -(float)npc.direction * 4, 8f, ModContent.ProjectileType<Flare>(), 0, 0f, Main.myPlayer, 0f, 0f);
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= fireTornadoPhaseTimer)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Enter new phase
            else if (npc.ai[0] == 4f)
            {
                npc.velocity *= 0.9f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

                if (npc.ai[2] == newPhaseTimer - 60)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);

                npc.ai[2] += 1f;
                if (npc.ai[2] >= newPhaseTimer)
                {
                    npc.ai[0] = 6f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.localAI[1] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Fast charge
            else if (npc.ai[0] == 5f)
            {
                ChargeDust(14, pie);

                npc.ai[2] += 1f;
                if (npc.ai[2] >= chargeTime)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 2f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }
            #endregion

            #region Phase2
            // Phase switch
            else if (npc.ai[0] == 6f && !player.dead)
            {
                if (npc.ai[1] == 0f)
                    npc.ai[1] = Math.Sign((vectorCenter - player.Center).X);

                Vector2 destination = player.Center + new Vector2(npc.ai[1], 0);
                Vector2 distanceFromDestination = destination - vectorCenter;
                Vector2 desiredVelocity = Vector2.Normalize(distanceFromDestination - npc.velocity) * velocity;

                if (Vector2.Distance(vectorCenter, destination) > reduceSpeedChargeDistance)
                    npc.SimpleFlyMovement(desiredVelocity, acceleration);
                else
                    npc.velocity *= 0.98f;

                int num1477 = Math.Sign(player.Center.X - vectorCenter.X);
                if (num1477 != 0)
                {
                    if (npc.ai[2] == 0f && num1477 != npc.direction)
                        npc.rotation += pie;

                    npc.direction = num1477;

                    if (npc.spriteDirection != -npc.direction)
                        npc.rotation += pie;

                    npc.spriteDirection = -npc.direction;
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= phaseSwitchTimer)
                {
                    int aiState = 0;
                    switch ((int)npc.ai[3])
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                            aiState = 1;
                            break;
                        case 4:
                            aiState = 5;
                            break;
                        case 5:
                            aiState = 6;
                            break;
                        case 6:
                            aiState = 2;
                            break;
                        case 7:
                            npc.ai[3] = 0f;
                            aiState = 3;
                            break;
                    }

                    if (phase3Check)
                        aiState = 4;

                    if (aiState == 1)
                    {
                        npc.ai[0] = 7f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;

                        npc.velocity = Vector2.Normalize(player.Center - vectorCenter) * chargeSpeed;
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                        if (num1477 != 0)
                        {
                            npc.direction = num1477;

                            if (npc.spriteDirection == 1)
                                npc.rotation += pie;

                            npc.spriteDirection = -npc.direction;
                        }
                    }
                    else if (aiState == 2)
                    {
                        Vector2 npcCenter = vectorCenter;

                        if (npc.alpha < 255)
                        {
                            npc.alpha += 17;
                            if (npc.alpha > 255)
                                npc.alpha = 255;
                        }

                        if (npc.ai[2] == phaseSwitchTimer + 15f)
                            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);

                        if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == phaseSwitchTimer + 15f)
                        {
                            Vector2 center = player.Center + new Vector2(0f, -540f);
                            npcCenter = npc.Center = center;
                        }

                        if (npc.ai[2] < phaseSwitchTimer + teleportPhaseTimer)
                            return;

                        npc.velocity = Vector2.Normalize(player.Center - vectorCenter) * spinPhaseVelocity;
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                        if (num1477 != 0)
                        {
                            npc.direction = num1477;

                            if (npc.spriteDirection == 1)
                                npc.rotation += pie;

                            npc.spriteDirection = -npc.direction;
                        }

                        npc.ai[0] = 8f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 1f;
                    }
                    else if (aiState == 3)
                    {
                        npc.ai[0] = 9f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else if (aiState == 4)
                    {
                        npc.ai[0] = 10f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else if (aiState == 5)
                    {
                        if (playFastChargeRoarSound)
                            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);

                        if (doFastCharge)
                        {
                            npc.ai[0] = 11f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                            npc.localAI[1] = 0f;

                            npc.velocity = Vector2.Normalize(player.Center - vectorCenter) * chargeSpeed * fastChargeVelocityMultiplier;
                            npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                            if (num1477 != 0)
                            {
                                npc.direction = num1477;

                                if (npc.spriteDirection == 1)
                                    npc.rotation += pie;

                                npc.spriteDirection = -npc.direction;
                            }
                        }
                        else
                            npc.localAI[1] += 1f;
                    }
                    else if (aiState == 6)
                    {
                        npc.velocity = Vector2.Normalize(player.Center - vectorCenter) * spinPhaseVelocity;
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                        if (num1477 != 0)
                        {
                            npc.direction = num1477;

                            if (npc.spriteDirection == 1)
                                npc.rotation += pie;

                            npc.spriteDirection = -npc.direction;
                        }

                        npc.ai[0] = 12f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }

                    npc.netUpdate = true;
                }
            }

            // Charge
            else if (npc.ai[0] == 7f)
            {
                ChargeDust(7, pie);

                npc.ai[2] += 1f;
                if (npc.ai[2] >= chargeTime)
                {
                    npc.ai[0] = 6f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 2f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Flare Dust bullet hell
            else if (npc.ai[0] == 8f)
            {
                if (npc.ai[2] == 0f)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);
                    flareDustBulletHellSpawn = vectorCenter + npc.velocity.RotatedBy(MathHelper.PiOver2 * -npc.direction) * spinTime / (MathHelper.TwoPi * 3f);
                }

                npc.ai[2] += 1f;

                if (npc.ai[2] % flareDustSpawnDivisor == 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int ringReduction = (int)MathHelper.Lerp(0f, 14f, npc.ai[2] / flareDustPhaseTimer);
                        int totalProjectiles = 38 - ringReduction; // 36 for first ring, 22 for last ring
                        DoFlareDustBulletHell(0, flareDustSpawnDivisor, npc.GetProjectileDamage(ModContent.ProjectileType<FlareDust>()), totalProjectiles, 0f, 0f, false);

                        // Fire a flame towards every player, with a limit of 5
                        if (expertMode)
                        {
                            List<int> targets = new List<int>();
                            for (int p = 0; p < Main.maxPlayers; p++)
                            {
                                if (Main.player[p].active && !Main.player[p].dead)
                                    targets.Add(p);

                                if (targets.Count > 4)
                                    break;
                            }
                            foreach (int t in targets)
                            {
                                Vector2 velocity2 = Vector2.Normalize(Main.player[t].Center + Main.player[t].velocity * 40f - flareDustBulletHellSpawn) * 8f;
                                int type = ModContent.ProjectileType<FlareDust>();
                                int proj = Projectile.NewProjectile(flareDustBulletHellSpawn, velocity2, type, npc.GetProjectileDamage(ModContent.ProjectileType<FlareDust>()), 0f, Main.myPlayer, 2f, 0f);
                                Main.projectile[proj].extraUpdates += 1;
                            }
                        }
                    }
                }

                npc.velocity = npc.velocity.RotatedBy(-(double)spinPhaseRotation * (float)npc.direction);
                npc.rotation -= spinPhaseRotation * npc.direction;

                if (npc.ai[2] >= flareDustPhaseTimer)
                {
                    npc.ai[0] = 6f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = increasedIdleTimeAfterBulletHell;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Infernado
            else if (npc.ai[0] == 9f)
            {
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

                if (npc.ai[2] == fireTornadoPhaseTimer - 30)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);

                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == fireTornadoPhaseTimer - 30)
                    Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, 0f, 0f, ModContent.ProjectileType<BigFlare>(), 0, 0f, Main.myPlayer, 1f, npc.target + 1);

                npc.ai[2] += 1f;
                if (npc.ai[2] >= fireTornadoPhaseTimer)
                {
                    npc.ai[0] = 6f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Enter new phase
            else if (npc.ai[0] == 10f)
            {
                npc.velocity *= 0.9f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

                if (npc.ai[2] == newPhaseTimer - 60)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);

                npc.ai[2] += 1f;
                if (npc.ai[2] >= newPhaseTimer)
                {
                    npc.ai[0] = 13f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.localAI[1] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Fast charge
            else if (npc.ai[0] == 11f)
            {
                ChargeDust(14, pie);

                npc.ai[2] += 1f;
                if (npc.ai[2] >= chargeTime)
                {
                    npc.ai[0] = 6f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 2f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Flare Dust that speeds up and whips around in a wave
            else if (npc.ai[0] == 12f)
            {
                if (npc.ai[2] == 0f)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);

                npc.ai[2] += 1f;

                if (npc.ai[2] % flareDustSpawnDivisor2 == 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 projectileVelocity = player.Center - fromMouth;
                        projectileVelocity.Normalize();
                        projectileVelocity *= 0.1f;
                        int type = ModContent.ProjectileType<FlareDust2>();
                        int damage = npc.GetProjectileDamage(type);
                        Projectile.NewProjectile(fromMouth, projectileVelocity, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                }

                npc.velocity = npc.velocity.RotatedBy(-(double)spinPhaseRotation * (float)npc.direction);
                npc.rotation -= spinPhaseRotation * npc.direction;

                if (npc.ai[2] >= flareDustPhaseTimer2)
                {
                    npc.ai[0] = 6f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 2f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }
            #endregion

            #region Phase3
            // Phase switch
            else if (npc.ai[0] == 13f && !player.dead)
            {
                if (npc.ai[1] == 0f)
                    npc.ai[1] = Math.Sign((vectorCenter - player.Center).X);

                Vector2 destination = player.Center + new Vector2(npc.ai[1], 0);
                Vector2 distanceFromDestination = destination - vectorCenter;
                Vector2 desiredVelocity = Vector2.Normalize(distanceFromDestination - npc.velocity) * velocity;

                if (Vector2.Distance(vectorCenter, destination) > reduceSpeedChargeDistance)
                    npc.SimpleFlyMovement(desiredVelocity, acceleration);
                else
                    npc.velocity *= 0.98f;

                int num1477 = Math.Sign(player.Center.X - vectorCenter.X);
                if (num1477 != 0)
                {
                    if (npc.ai[2] == 0f && num1477 != npc.direction)
                        npc.rotation += pie;

                    npc.direction = num1477;

                    if (npc.spriteDirection != -npc.direction)
                        npc.rotation += pie;

                    npc.spriteDirection = -npc.direction;
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= phaseSwitchTimer)
                {
                    int aiState = 0;
                    switch ((int)npc.ai[3])
                    {
                        case 0:
                        case 1:
                            aiState = 1;
                            break;
                        case 2:
                        case 3:
                        case 4:
                            aiState = 5;
                            break;
                        case 5:
                            aiState = 3;
                            break;
                        case 6:
                            aiState = 6;
                            break;
                        case 7:
                            npc.ai[3] = 1f;
                            aiState = 7;
                            break;
                        case 8:
                            aiState = 2;
                            break;
                    }

                    if (phase4Check)
                        aiState = 4;

                    if (aiState == 1)
                    {
                        npc.ai[0] = 14f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;

                        npc.velocity = Vector2.Normalize(player.Center - vectorCenter) * chargeSpeed;
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                        if (num1477 != 0)
                        {
                            npc.direction = num1477;

                            if (npc.spriteDirection == 1)
                                npc.rotation += pie;

                            npc.spriteDirection = -npc.direction;
                        }
                    }
                    else if (aiState == 2)
                    {
                        Vector2 npcCenter = vectorCenter;

                        if (npc.alpha < 255)
                        {
                            npc.alpha += 17;
                            if (npc.alpha > 255)
                                npc.alpha = 255;
                        }

                        if (npc.ai[2] == phaseSwitchTimer + 15f)
                            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);

                        if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == phaseSwitchTimer + 15f)
                        {
                            Vector2 center = player.Center + new Vector2(0f, -540f);
                            npcCenter = npc.Center = center;
                        }

                        if (npc.ai[2] < phaseSwitchTimer + teleportPhaseTimer)
                            return;

                        npc.velocity = Vector2.Normalize(player.Center - vectorCenter) * spinPhaseVelocity;
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                        if (num1477 != 0)
                        {
                            npc.direction = num1477;

                            if (npc.spriteDirection == 1)
                                npc.rotation += pie;

                            npc.spriteDirection = -npc.direction;
                        }

                        npc.ai[0] = 15f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                    }
                    else if (aiState == 3)
                    {
                        npc.ai[0] = 16f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else if (aiState == 4)
                    {
                        npc.ai[0] = 17f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else if (aiState == 5)
                    {
                        if (playFastChargeRoarSound)
                            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);

                        if (doFastCharge)
                        {
                            npc.ai[0] = 18f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                            npc.localAI[1] = 0f;

                            npc.velocity = Vector2.Normalize(player.Center - vectorCenter) * chargeSpeed * fastChargeVelocityMultiplier;
                            npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                            if (num1477 != 0)
                            {
                                npc.direction = num1477;

                                if (npc.spriteDirection == 1)
                                    npc.rotation += pie;

                                npc.spriteDirection = -npc.direction;
                            }
                        }
                        else
                            npc.localAI[1] += 1f;
                    }
                    else if (aiState == 6)
                    {
                        npc.velocity = Vector2.Normalize(player.Center - vectorCenter) * spinPhaseVelocity;
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                        if (num1477 != 0)
                        {
                            npc.direction = num1477;

                            if (npc.spriteDirection == 1)
                                npc.rotation += pie;

                            npc.spriteDirection = -npc.direction;
                        }

                        npc.ai[0] = 19f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else if (aiState == 7)
                    {
                        npc.ai[0] = 20f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }

                    npc.netUpdate = true;
                }
            }

            // Charge
            else if (npc.ai[0] == 14f)
            {
                ChargeDust(7, pie);

                npc.ai[2] += 1f;
                if (npc.ai[2] >= chargeTime)
                {
                    npc.ai[0] = 13f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 2f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Flare Dust bullet hell
            else if (npc.ai[0] == 15f)
            {
                if (npc.ai[2] == 0f)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);
                    flareDustBulletHellSpawn = vectorCenter + npc.velocity.RotatedBy(MathHelper.PiOver2 * -npc.direction) * spinTime / (MathHelper.TwoPi * 3f);
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] % flareDustSpawnDivisor3 == 0f)
                {
                    // Rotate spiral by 7.2 * (300 / 12) = +90 degrees and then back -90 degrees

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        DoFlareDustBulletHell(1, flareDustPhaseTimer, npc.GetProjectileDamage(ModContent.ProjectileType<FlareDust>()), 8, 12f, 3.6f, false);

                        // Fire a flame towards every player, with a limit of 5
                        if (expertMode && npc.ai[2] % (flareDustSpawnDivisor3 * 2) == 0f)
                        {
                            List<int> targets = new List<int>();
                            for (int p = 0; p < Main.maxPlayers; p++)
                            {
                                if (Main.player[p].active && !Main.player[p].dead)
                                    targets.Add(p);

                                if (targets.Count > 4)
                                    break;
                            }
                            foreach (int t in targets)
                            {
                                Vector2 velocity2 = Vector2.Normalize(Main.player[t].Center + Main.player[t].velocity * 40f - flareDustBulletHellSpawn) * 8f;
                                int type = ModContent.ProjectileType<FlareDust>();
                                int proj = Projectile.NewProjectile(flareDustBulletHellSpawn, velocity2, type, npc.GetProjectileDamage(ModContent.ProjectileType<FlareDust>()), 0f, Main.myPlayer, 2f, 0f);
                                Main.projectile[proj].extraUpdates += 1;
                            }
                        }
                    }
                }

                npc.velocity = npc.velocity.RotatedBy(-(double)spinPhaseRotation * (float)npc.direction);
                npc.rotation -= spinPhaseRotation * npc.direction;

                if (npc.ai[2] >= flareDustPhaseTimer)
                {
                    npc.ai[0] = 13f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.localAI[2] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Infernado
            else if (npc.ai[0] == 16f)
            {
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

                if (npc.ai[2] == fireTornadoPhaseTimer - 30)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);

                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == fireTornadoPhaseTimer - 30)
                    Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, 0f, 0f, ModContent.ProjectileType<BigFlare>(), 0, 0f, Main.myPlayer, 1f, npc.target + 1);

                npc.ai[2] += 1f;
                if (npc.ai[2] >= fireTornadoPhaseTimer)
                {
                    npc.ai[0] = 13f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 3f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Enter new phase
            else if (npc.ai[0] == 17f)
            {
                npc.velocity *= 0.9f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

                if (npc.ai[2] == newPhaseTimer - 60)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);

                npc.ai[2] += 1f;
                if (npc.ai[2] >= newPhaseTimer)
                {
                    startSecondAI = true;
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.localAI[1] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Fast charge
            else if (npc.ai[0] == 18f)
            {
                ChargeDust(14, pie);

                npc.ai[2] += 1f;
                if (npc.ai[2] >= chargeTime)
                {
                    npc.ai[0] = 13f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 2f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Fireball ring
            else if (npc.ai[0] == 19f)
            {
                if (npc.ai[2] == 0f)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);

                npc.ai[2] += 1f;

                if (npc.ai[2] % flareDustSpawnDivisor2 == 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 projectileVelocity = player.Center - fromMouth;
                        projectileVelocity.Normalize();
                        projectileVelocity *= 0.1f;
                        int type = ModContent.ProjectileType<FlareDust2>();
                        int damage = npc.GetProjectileDamage(type);
                        Projectile.NewProjectile(fromMouth, projectileVelocity, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                }

                npc.velocity = npc.velocity.RotatedBy(-(double)spinPhaseRotation * (float)npc.direction);
                npc.rotation -= spinPhaseRotation * npc.direction;

                if (npc.ai[2] >= flareDustPhaseTimer2 - 50)
                {
                    npc.ai[0] = 13f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 1f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Fireball breath
            else if (npc.ai[0] == 20f)
            {
                if (npc.ai[1] == 0f)
                    npc.ai[1] = Math.Sign((vectorCenter - player.Center).X);

                Vector2 destination = player.Center + new Vector2(npc.ai[1], 0);
                Vector2 value17 = destination - vectorCenter;
                Vector2 vector170 = Vector2.Normalize(value17 - npc.velocity) * flareBombPhaseVelocity;

                if (Vector2.Distance(vectorCenter, destination) > reduceSpeedFlareBombDistance)
                    npc.SimpleFlyMovement(vector170, flareBombPhaseAcceleration);
                else
                    npc.velocity *= 0.98f;

                if (npc.ai[2] == 0f)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);

                if (npc.ai[2] % flareBombSpawnDivisor == 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int type = ModContent.ProjectileType<FlareBomb>();
                        int damage = npc.GetProjectileDamage(type);
                        Projectile.NewProjectile(fromMouth, Vector2.Zero, type, damage, 0f, Main.myPlayer, npc.target, 1f);
                    }
                }

                int num1476 = Math.Sign(player.Center.X - vectorCenter.X);
                if (num1476 != 0)
                {
                    npc.direction = num1476;

                    if (npc.spriteDirection != -npc.direction)
                        npc.rotation += pie;

                    npc.spriteDirection = -npc.direction;
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= flareBombPhaseTimer - 15)
                {
                    npc.ai[0] = 13f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }
            #endregion
        }

        #region AI2
        public void Yharon_AI2(bool expertMode, bool revenge, bool death, bool malice, float pie, float lifeRatio, Vector2 vectorCenter, CalamityGlobalNPC calamityGlobalNPC)
        {
            float phase2GateValue = revenge ? 0.44f : expertMode ? 0.385f : 0.275f;
            bool phase2 = death || lifeRatio <= phase2GateValue;
            float phase3GateValue = death ? 0.358f : revenge ? 0.275f : expertMode ? 0.22f : 0.138f;
            bool phase3 = lifeRatio <= phase3GateValue;
            float phase4GateValue = death ? 0.165f : 0.11f;
            bool phase4 = lifeRatio <= phase4GateValue && revenge;

            if (npc.ai[0] != 8f)
            {
                npc.alpha -= 25;
                if (npc.alpha < 0)
                    npc.alpha = 0;
            }

            if (!moveCloser)
            {
                // When Yharon begins Phase 2, switch music to Roar of the Jungle Dragon.
                music = CalamityMod.Instance.GetMusicFromMusicMod("YharonP2") ?? MusicID.LunarBoss;

                moveCloser = true;

                string key = "Mods.CalamityMod.FlameText";
                Color messageColor = Color.Orange;

                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }

            if (invincibilityCounter < Phase2InvincibilityTime)
            {
                phase2 = phase3 = phase4 = false;
                invincibilityCounter += 1;
            }
            else
            {
                // Damage immunity
                switch (secondPhasePhase)
                {
                    case 1:
                        npc.dontTakeDamage = phase2;
                        break;
                    case 2:
                        npc.dontTakeDamage = phase3;
                        break;
                    case 3:
                        npc.dontTakeDamage = phase4;
                        break;
                    case 4:
                        npc.dontTakeDamage = false;
                        break;
                }

                if (!npc.dontTakeDamage)
                    npc.dontTakeDamage = npc.ai[0] == 9f;
            }

            // Acquire target and determine enrage state
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest();
                npc.netUpdate = true;
            }

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, vectorCenter) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            Player targetData = Main.player[npc.target];

            // Despawn
            bool targetDead = false;
            if (targetData.dead || !targetData.active)
            {
                npc.TargetClosest();
                targetData = Main.player[npc.target];
                if (targetData.dead || !targetData.active)
                {
                    targetDead = true;

                    npc.velocity.Y -= 0.4f;

                    if (npc.timeLeft > 60)
                        npc.timeLeft = 60;

                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                }
            }
            else if (npc.timeLeft < 1800)
                npc.timeLeft = 1800;

            enraged = !targetData.Hitbox.Intersects(safeBox);
            if (enraged)
            {
                protectionBoost = true;
                npc.damage = npc.defDamage * 5;
            }
            else
            {
                protectionBoost = false;
                npc.damage = npc.defDamage;
            }

            // Set DR based on protection boost (aka enrage)
            bool bulletHell = npc.ai[0] == 5f;
            calamityGlobalNPC.DR = protectionBoost ? EnragedDR : normalDR;

            if (bulletHell)
                npc.damage = 0;

            float reduceSpeedChargeDistance = 500f;
            float phaseSwitchTimer = malice ? 28f : expertMode ? 30f : 32f;
            float acceleration = expertMode ? 0.92f : 0.9f;
            float velocity = expertMode ? 14.5f : 14f;
            float chargeTime = expertMode ? 32f : 35f;
            float chargeSpeed = expertMode ? 32f : 30f;

            float fastChargeVelocityMultiplier = malice ? 2f : 1.5f;
            fastChargeTelegraphTime = protectionBoost ? 60 : (100 - secondPhasePhase * 10);
            bool playFastChargeRoarSound = npc.localAI[1] == fastChargeTelegraphTime * 0.5f;
            bool doFastChargeTelegraph = npc.localAI[1] <= fastChargeTelegraphTime;

            float fireballBreathTimer = 60f;
            float fireballBreathPhaseTimer = fireballBreathTimer + 120f;
            float fireballBreathPhaseVelocity = 22f;

            float splittingFireballBreathTimer = 40f;
            float splittingFireballBreathPhaseVelocity = 22f;
            int splittingFireballBreathDivisor = 10;
            int splittingFireballs = 10;
            int splittingFireballBreathTimer2 = splittingFireballs * splittingFireballBreathDivisor;
            float splittingFireballBreathYVelocityTimer = 40f;
            float splittingFireballBreathPhaseTimer = splittingFireballBreathTimer + splittingFireballBreathTimer2 + splittingFireballBreathYVelocityTimer;

            float flareDustPhaseScalar = secondPhasePhase == 4 ? (death ? 54f : 60f) : (death ? 67f : 80f);
            int spinPhaseTimerReduction = revenge ? (secondPhasePhase == 4 ? (int)(flareDustPhaseScalar * ((phase4GateValue - lifeRatio) / phase4GateValue)) : (int)(flareDustPhaseScalar * (ai2GateValue - lifeRatio))) : 0;
            int spinPhaseTimer = (secondPhasePhase == 4 ? (malice ? 150 : death ? 160 : 180) : (malice ? 180 : death ? 200 : 240)) - spinPhaseTimerReduction;
            float spinTime = spinPhaseTimer / 2;
            float spinRotation = MathHelper.TwoPi * 3 / spinTime;
            float spinPhaseVelocity = 25f;
            int flareDustSpawnDivisor = spinPhaseTimer / 10;
            int flareDustSpawnDivisor2 = spinPhaseTimer / 20 + (secondPhasePhase == 4 ? spinPhaseTimer / 60 : 0);
            float increasedIdleTimeAfterBulletHell = -120f;

            float flareSpawnDecelerationTimer = malice ? 60f : death ? 75f : 90f;
            int flareSpawnPhaseTimerReduction = revenge ? (int)(flareSpawnDecelerationTimer * (ai2GateValue - lifeRatio)) : 0;
            float flareSpawnPhaseTimer = (malice ? 120f : death ? 150f : 180f) - flareSpawnPhaseTimerReduction;

            float teleportPhaseTimer = 45f;

            if (revenge)
            {
                float chargeTimeDecrease = malice ? 6f : death ? 4f : 2f;
                float velocityMult = malice ? 1.15f : death ? 1.1f : 1.05f;
                acceleration *= velocityMult;
                velocity *= velocityMult;
                chargeTime -= chargeTimeDecrease;
                chargeSpeed *= velocityMult;
            }

            if (npc.ai[0] == 0f)
            {
                npc.ai[1] += 1f;
                if (npc.ai[1] >= 10f)
                {
                    npc.ai[1] = 0f;
                    npc.ai[0] = 1f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 1f)
            {
                if (npc.ai[2] == 0f)
                    npc.ai[2] = (vectorCenter.X < targetData.Center.X) ? 1 : -1;

                Vector2 destination = targetData.Center + new Vector2(-npc.ai[2], 0f);
                Vector2 desiredVelocity = npc.SafeDirectionTo(destination) * velocity;

                if (!targetDead)
                {
                    if (Vector2.Distance(vectorCenter, destination) > reduceSpeedChargeDistance)
                    npc.SimpleFlyMovement(desiredVelocity, acceleration);
                    else
                        npc.velocity *= 0.98f;
                }

                int num27 = (vectorCenter.X < targetData.Center.X) ? 1 : -1;
                npc.direction = npc.spriteDirection = num27;

                npc.ai[1] += 1f;
                if (npc.ai[1] >= phaseSwitchTimer)
                {
                    int num28 = 1;
                    if (phase4)
                    {
                        switch ((int)npc.ai[3])
                        {
                            case 0:
                                num28 = 8; //teleport
                                break;
                            case 1:
                            case 2:
                                num28 = 7; //fast charge
                                break;
                            case 3:
                                num28 = 5; //fire circle + tornado (only once) + fireballs
                                break;
                        }
                    }
                    else if (phase3)
                    {
                        switch ((int)npc.ai[3])
                        {
                            case 0:
                                num28 = 6; //tornado
                                break;
                            case 1:
                                num28 = 7; //fast charge
                                break;
                            case 2:
                                num28 = 8; //teleport
                                break;
                            case 3:
                                num28 = 7; //fast charge
                                break;
                            case 4:
                                num28 = 5; //fire circle
                                break;
                            case 5:
                                num28 = 4; //fireballs
                                break;
                            case 6:
                                num28 = 7; //fast charge
                                break;
                            case 7:
                                num28 = 8; //teleport
                                break;
                            case 8:
                                num28 = 7; //fast charge
                                break;
                            case 9:
                                num28 = 3; //fireballs
                                break;
                            case 10:
                                num28 = 6; //tornado
                                break;
                            case 11:
                                num28 = 7; //fast charge
                                break;
                            case 12:
                                num28 = 8; //teleport
                                break;
                            case 13:
                                num28 = 7; //fast charge
                                break;
                            case 14:
                                num28 = 5; //fire circle
                                break;
                            case 15:
                                num28 = 4; //fireballs
                                break;
                        }
                    }
                    else if (phase2)
                    {
                        switch ((int)npc.ai[3])
                        {
                            case 0:
                                num28 = 6; //tornado
                                break;
                            case 1:
                                num28 = 7; //fast charge
                                break;
                            case 2:
                                num28 = 2; //charge
                                break;
                            case 3:
                                num28 = 5; //fire circle
                                break;
                            case 4:
                                num28 = 4; //fireballs
                                break;
                            case 5:
                                num28 = 7; //fast charge
                                break;
                            case 6:
                                num28 = 2; //charge
                                break;
                            case 7:
                                num28 = 3; //fireballs
                                break;
                            case 8:
                                num28 = 7; //fast charge
                                break;
                            case 9:
                                num28 = 2; //charge
                                break;
                            case 10:
                                num28 = 5; //fire circle
                                break;
                        }
                    }
                    else
                    {
                        switch ((int)npc.ai[3])
                        {
                            case 0:
                                num28 = 6; //tornado
                                break;
                            case 1:
                            case 2:
                                num28 = 2; //charge
                                break;
                            case 3:
                                num28 = 3; //fireballs
                                break;
                            case 4:
                            case 5:
                                num28 = 7; //fast charge
                                break;
                            case 6:
                                num28 = 4; //fireballs
                                break;
                            case 7:
                            case 8:
                                num28 = 2; //charge
                                break;
                            case 9:
                                num28 = 5; //fire circle
                                break;
                        }
                    }

                    if (num28 == 5 && npc.ai[1] < phaseSwitchTimer + teleportPhaseTimer)
                    {
                        float newRotation = npc.AngleTo(targetData.Center);
                        float amount = 0.04f;

                        if (npc.spriteDirection == -1)
                            newRotation += pie;

                        if (amount != 0f)
                            npc.rotation = npc.rotation.AngleTowards(newRotation, amount);

                        Vector2 npcCenter = vectorCenter;

                        if (npc.alpha < 255)
                        {
                            npc.alpha += 17;
                            if (npc.alpha > 255)
                                npc.alpha = 255;
                        }

                        float timeBeforeTeleport = teleportPhaseTimer - 15f;
                        if (npc.ai[1] == phaseSwitchTimer + timeBeforeTeleport)
                            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);

                        if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[1] == phaseSwitchTimer + timeBeforeTeleport)
                        {
                            Vector2 center = targetData.Center + new Vector2(0f, -540f);
                            npcCenter = npc.Center = center;
                        }

                        return;
                    }

                    if (num28 == 7 && doFastChargeTelegraph)
                    {
                        float newRotation = npc.AngleTo(targetData.Center);
                        float amount = 0.04f;

                        if (npc.spriteDirection == -1)
                            newRotation += pie;

                        if (amount != 0f)
                            npc.rotation = npc.rotation.AngleTowards(newRotation, amount);

                        if (playFastChargeRoarSound)
                            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);

                        npc.localAI[1] += 1f;

                        return;
                    }

                    npc.ai[0] = num28;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 1f;
                    npc.localAI[1] = 0f;

                    switch (secondPhasePhase)
                    {
                        case 1:
                            if (phase2)
                            {
                                secondPhasePhase = 2;
                                npc.ai[0] = 9f;
                                npc.ai[1] = 0f;
                                npc.ai[2] = 0f;
                                npc.ai[3] = 0f;
                            }
                            break;
                        case 2:
                            if (phase3)
                            {
                                secondPhasePhase = 3;
                                npc.ai[0] = 9f;
                                npc.ai[1] = 0f;
                                npc.ai[2] = 0f;
                                npc.ai[3] = 0f;
                            }
                            break;
                        case 3:
                            if (phase4)
                            {
                                secondPhasePhase = 4;
                                npc.ai[0] = 9f;
                                npc.ai[1] = 0f;
                                npc.ai[2] = 0f;
                                npc.ai[3] = 0f;
                            }
                            break;
                    }

                    npc.netUpdate = true;

                    float aiLimit = 10f;
                    if (phase4)
                        aiLimit = 4f;
                    else if (phase3)
                        aiLimit = 16f;
                    else if (phase2)
                        aiLimit = 11f;

                    if (npc.ai[3] >= aiLimit)
                        npc.ai[3] = 0f;

                    switch (num28)
                    {
                        case 2: //charge
                        {
                            Vector2 vector = npc.SafeDirectionTo(targetData.Center, Vector2.UnitX * npc.spriteDirection);
                            npc.spriteDirection = (vector.X > 0f) ? 1 : -1;
                            npc.rotation = vector.ToRotation();

                            if (npc.spriteDirection == -1)
                                npc.rotation += pie;

                            npc.velocity = vector * chargeSpeed;

                            break;
                        }
                        case 3: //fireballs
                        {
                            Vector2 vector2 = new Vector2((targetData.Center.X > vectorCenter.X) ? 1 : -1, 0f);
                            npc.spriteDirection = (vector2.X > 0f) ? 1 : -1;
                            npc.velocity = vector2 * -2f;

                            break;
                        }
                        case 5: //spin move
                        {
                            Vector2 vector3 = npc.SafeDirectionTo(targetData.Center, Vector2.UnitX * npc.spriteDirection);
                            npc.spriteDirection = (vector3.X > 0f) ? 1 : -1;
                            npc.rotation = vector3.ToRotation();

                            if (npc.spriteDirection == -1)
                                npc.rotation += pie;

                            npc.velocity = vector3 * spinPhaseVelocity;
                            
                            npc.localAI[3] = Main.rand.Next(2);
                            
                            break;
                        }
                        case 7: //fast charge
                        {
                            Vector2 vector = npc.SafeDirectionTo(targetData.Center, Vector2.UnitX * npc.spriteDirection);
                            npc.spriteDirection = (vector.X > 0f) ? 1 : -1;
                            npc.rotation = vector.ToRotation();

                            if (npc.spriteDirection == -1)
                                npc.rotation += pie;

                            npc.velocity = vector * chargeSpeed * fastChargeVelocityMultiplier;

                            break;
                        }
                    }
                }
            }

            // Charge
            else if (npc.ai[0] == 2f)
            {
                if (npc.ai[1] == 1f)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);

                ChargeDust(7, pie);

                npc.ai[1] += 1f;
                if (npc.ai[1] >= chargeTime)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.TargetClosest();
                }
            }

            // Fireball spit
            else if (npc.ai[0] == 3f)
            {
                int num29 = (vectorCenter.X < targetData.Center.X) ? 1 : -1;
                npc.ai[2] = num29;

                npc.ai[1] += 1f;
                if (npc.ai[1] < fireballBreathTimer)
                {
                    Vector2 vector4 = targetData.Center + new Vector2(num29 * -750f, -300f);
                    Vector2 value = npc.SafeDirectionTo(vector4) * 16f;

                    if (npc.Distance(vector4) < 16f)
                        npc.Center = vector4;
                    else
                        npc.position += value;

                    if (Vector2.Distance(vector4, npc.Center) < 32f)
                        npc.ai[1] = fireballBreathTimer - 1f;
                }

                if (npc.ai[1] == fireballBreathTimer)
                {
                    int direction = (targetData.Center.X > vectorCenter.X) ? 1 : -1;
                    npc.velocity = new Vector2(direction, 0f) * fireballBreathPhaseVelocity;
                    npc.direction = npc.spriteDirection = direction;
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonFire"), npc.Center);
                }

                if (npc.ai[1] >= fireballBreathTimer)
                {
                    if (npc.ai[1] % 10 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float xOffset = 30f;
                        Vector2 position = vectorCenter + new Vector2((110f + xOffset) * npc.direction, -20f).RotatedBy(npc.rotation);
                        Vector2 projectileVelocity = targetData.Center - position;
                        projectileVelocity.Normalize();
                        projectileVelocity *= 0.1f;
                        int type = ModContent.ProjectileType<FlareDust2>();
                        int damage = npc.GetProjectileDamage(type);
                        Projectile.NewProjectile(position, projectileVelocity, type, damage, 0f, Main.myPlayer, 1f, 0f);
                    }

                    if (Math.Abs(targetData.Center.X - vectorCenter.X) > 700f && Math.Abs(npc.velocity.X) < chargeSpeed)
                        npc.velocity.X += Math.Sign(npc.velocity.X) * 0.5f;
                }

                if (npc.ai[1] >= fireballBreathPhaseTimer)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.TargetClosest();
                }
            }

            // Splitting fireball breath
            else if (npc.ai[0] == 4f)
            {
                int num31 = (vectorCenter.X < targetData.Center.X) ? 1 : -1;
                npc.ai[2] = num31;

                if (npc.ai[1] < splittingFireballBreathTimer)
                {
                    Vector2 vector5 = targetData.Center + new Vector2(num31 * -750f, -300f);
                    Vector2 value2 = npc.SafeDirectionTo(vector5) * splittingFireballBreathPhaseVelocity;

                    npc.velocity = Vector2.Lerp(npc.velocity, value2, 0.0333333351f);

                    int direction = (vectorCenter.X < targetData.Center.X) ? 1 : -1;
                    npc.direction = npc.spriteDirection = direction;

                    if (Vector2.Distance(vector5, vectorCenter) < 32f)
                        npc.ai[1] = splittingFireballBreathTimer - 1f;
                }
                else if (npc.ai[1] == splittingFireballBreathTimer)
                {
                    Vector2 vector6 = npc.SafeDirectionTo(targetData.Center, Vector2.UnitX * npc.spriteDirection);
                    vector6.Y *= 0.15f;
                    vector6 = vector6.SafeNormalize(Vector2.UnitX * npc.direction);

                    npc.spriteDirection = (vector6.X > 0f) ? 1 : -1;
                    npc.rotation = vector6.ToRotation();

                    if (npc.spriteDirection == -1)
                        npc.rotation += pie;

                    npc.velocity = vector6 * splittingFireballBreathPhaseVelocity;
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonFire"), npc.Center);
                }
                else
                {
                    npc.position.X += npc.SafeDirectionTo(targetData.Center).X * 7f;
                    npc.position.Y += npc.SafeDirectionTo(targetData.Center + new Vector2(0f, -400f)).Y * 6f;

                    float xOffset = 30f;
                    Vector2 position = vectorCenter + new Vector2((110f + xOffset) * npc.direction, -20f).RotatedBy(npc.rotation);
                    int num34 = (int)(npc.ai[1] - splittingFireballBreathTimer + 1f);

                    int type = ModContent.ProjectileType<YharonFireball>();
                    int damage = npc.GetProjectileDamage(type);
                    if (num34 <= splittingFireballBreathTimer2 && num34 % splittingFireballBreathDivisor == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(position, npc.velocity, type, damage, 0f, Main.myPlayer, 0f, 0f);
                }

                if (npc.ai[1] > splittingFireballBreathPhaseTimer - splittingFireballBreathYVelocityTimer)
                    npc.velocity.Y -= 0.1f;

                npc.ai[1] += 1f;
                if (npc.ai[1] >= splittingFireballBreathPhaseTimer)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.TargetClosest();
                }
            }

            // Fireball spin
            else if (npc.ai[0] == 5f)
            {
                npc.velocity = npc.velocity.RotatedBy(-(double)spinRotation * (float)npc.direction);
                npc.rotation -= spinRotation * npc.direction;

                if (npc.ai[1] == 1f)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);
                    flareDustBulletHellSpawn = npc.Center + npc.velocity.RotatedBy(MathHelper.PiOver2 * -npc.direction) * spinTime / (MathHelper.TwoPi * 3f);
                }

                npc.ai[1] += 1f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (secondPhasePhase >= 3)
                    {
                        // Rotate spiral by 9 * (240 / 12) = +90 degrees and then back -90 degrees

                        // For phase 4: Rotate spiral by 18 * (240 / 16) = +135 degrees and then back -135 degrees

                        if (npc.ai[1] % flareDustSpawnDivisor2 == 0f)
                        {
                            int totalProjectiles = secondPhasePhase == 4 ? 12 : 10;
                            float projectileVelocity = secondPhasePhase == 4 ? 16f : 12f;
                            float radialOffset = secondPhasePhase == 4 ? 2.8f : 3.2f;
                            if (npc.localAI[3] == 0f)
                            {
                                DoFlareDustBulletHell(1, spinPhaseTimer, npc.GetProjectileDamage(ModContent.ProjectileType<FlareDust>()), totalProjectiles, projectileVelocity, radialOffset, true);
                            }
                            else
                            {
                                int ringReduction = (int)MathHelper.Lerp(0f, 12f, npc.ai[1] / spinPhaseTimer);
                                int totalProjectiles2 = 38 - ringReduction; // 36 for first ring, 24 for last ring
                                DoFlareDustBulletHell(0, flareDustSpawnDivisor2, npc.GetProjectileDamage(ModContent.ProjectileType<FlareDust>()), totalProjectiles2, 0f, 0f, true);
                            }

                            // Fire a flame towards every player, with a limit of 10
                            if (expertMode && npc.ai[2] % (flareDustSpawnDivisor2 * 2) == 0f)
                            {
                                List<int> targets = new List<int>();
                                for (int p = 0; p < Main.maxPlayers; p++)
                                {
                                    if (Main.player[p].active && !Main.player[p].dead)
                                        targets.Add(p);

                                    if (targets.Count > 4)
                                        break;
                                }
                                foreach (int t in targets)
                                {
                                    Vector2 velocity2 = Vector2.Normalize(Main.player[t].Center + Main.player[t].velocity * 40f - flareDustBulletHellSpawn) * (projectileVelocity * 0.7f);
                                    int type = ModContent.ProjectileType<FlareDust>();
                                    int proj = Projectile.NewProjectile(flareDustBulletHellSpawn, velocity2, type, npc.GetProjectileDamage(ModContent.ProjectileType<FlareDust>()), 0f, Main.myPlayer, 2f, 0f);
                                    Main.projectile[proj].extraUpdates += 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (npc.ai[1] % flareDustSpawnDivisor == 0f)
                        {
                            int ringReduction = (int)MathHelper.Lerp(0f, 12f, npc.ai[1] / spinPhaseTimer);
                            int totalProjectiles = 38 - ringReduction; // 36 for first ring, 24 for last ring
                            DoFlareDustBulletHell(0, flareDustSpawnDivisor, npc.GetProjectileDamage(ModContent.ProjectileType<FlareDust>()), totalProjectiles, 0f, 0f, true);

                            // Fire a flame towards every player, with a limit of 10
                            if (expertMode)
                            {
                                List<int> targets = new List<int>();
                                for (int p = 0; p < Main.maxPlayers; p++)
                                {
                                    if (Main.player[p].active && !Main.player[p].dead)
                                        targets.Add(p);

                                    if (targets.Count > 4)
                                        break;
                                }
                                foreach (int t in targets)
                                {
                                    Vector2 velocity2 = Vector2.Normalize(Main.player[t].Center + Main.player[t].velocity * 40f - flareDustBulletHellSpawn) * 8f;
                                    int type = ModContent.ProjectileType<FlareDust>();
                                    int proj = Projectile.NewProjectile(flareDustBulletHellSpawn, velocity2, type, npc.GetProjectileDamage(ModContent.ProjectileType<FlareDust>()), 0f, Main.myPlayer, 2f, 0f);
                                    Main.projectile[proj].extraUpdates += 1;
                                }
                            }
                        }
                    }

                    if (npc.ai[1] == 210f && secondPhasePhase == 4 && useTornado)
                    {
                        useTornado = false;
                        Projectile.NewProjectile(vectorCenter, Vector2.Zero, ModContent.ProjectileType<BigFlare2>(), 0, 0f, Main.myPlayer, 1f, npc.target + 1);
                    }
                }

                if (npc.ai[1] >= spinPhaseTimer)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = increasedIdleTimeAfterBulletHell;
                    npc.ai[2] = 0f;
                    npc.localAI[2] = 0f;
                    npc.TargetClosest();
                    npc.velocity /= 2f;
                }
            }

            // Fire ring
            else if (npc.ai[0] == 6f)
            {
                if (npc.ai[1] == 0f)
                {
                    Vector2 destination2 = targetData.Center + new Vector2(0f, -200f);
                    Vector2 desiredVelocity2 = npc.SafeDirectionTo(destination2) * velocity * 1.5f;
                    npc.SimpleFlyMovement(desiredVelocity2, acceleration * 1.5f);

                    int num35 = (vectorCenter.X < targetData.Center.X) ? 1 : -1;
                    npc.direction = npc.spriteDirection = num35;

                    npc.ai[2] += 1f;
                    if (npc.Distance(targetData.Center) < 600f || npc.ai[2] >= 180f)
                    {
                        npc.ai[1] = 1f;
                        npc.netUpdate = true;
                    }
                }
                else
                {
                    if (npc.ai[1] < flareSpawnDecelerationTimer)
                        npc.velocity *= 0.95f;
                    else
                        npc.velocity *= 0.98f;

                    if (npc.ai[1] == flareSpawnDecelerationTimer)
                    {
                        if (npc.velocity.Y > 0f)
                            npc.velocity.Y /= 3f;

                        npc.velocity.Y -= 3f;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (npc.ai[1] == 20f || npc.ai[1] == 80f || npc.ai[1] == 140f)
                        {
                            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);

                            DoFireRing(expertMode ? 300 : 180, npc.GetProjectileDamage(ModContent.ProjectileType<FlareBomb>()), npc.target, 1f);

                            Vector2 vector7 = vectorCenter + (MathHelper.TwoPi * Main.rand.NextFloat()).ToRotationVector2() * new Vector2(2f, 1f) * 100f * (0.6f + Main.rand.NextFloat() * 0.4f);
                        }
                    }

                    npc.ai[1] += 1f;
                }

                if (npc.ai[1] >= flareSpawnPhaseTimer)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(vectorCenter, Vector2.Zero, ModContent.ProjectileType<BigFlare2>(), 0, 0f, Main.myPlayer, 1f, npc.target + 1);

                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.TargetClosest();
                }
            }

            // Fast charge
            else if (npc.ai[0] == 7f)
            {
                if (npc.ai[1] == 1f)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);

                ChargeDust(14, pie);

                npc.ai[1] += 1f;
                if (npc.ai[1] >= chargeTime)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.TargetClosest();
                }
            }

            // Teleport
            else if (npc.ai[0] == 8f)
            {
                Vector2 npcCenter = vectorCenter;

                if (npc.alpha < 255)
                {
                    npc.alpha += 17;
                    if (npc.alpha > 255)
                        npc.alpha = 255;
                }

                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

                if (npc.ai[2] == 15f)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);

                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == 15f)
                {
                    if (npc.ai[1] == 0f)
                        npc.ai[1] = 450 * Math.Sign((npcCenter - targetData.Center).X);

                    teleportLocation = Main.rand.NextBool(2) ? (revenge ? 500 : 600) : (revenge ? -500 : -600);
                    Vector2 center = targetData.Center + new Vector2(-npc.ai[1], teleportLocation);
                    npcCenter = npc.Center = center;
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= teleportPhaseTimer)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.localAI[1] = fastChargeTelegraphTime + 1f;
                    npc.netUpdate = true;
                }
            }

            // Enter new phase
            else if (npc.ai[0] == 9f)
            {
                npc.velocity *= 0.9f;

                Vector2 vector = npc.SafeDirectionTo(targetData.Center, -Vector2.UnitY);
                npc.spriteDirection = (vector.X > 0f) ? 1 : -1;
                npc.rotation = vector.ToRotation();

                if (npc.spriteDirection == -1)
                    npc.rotation += pie;

                if (npc.ai[2] == 120f)
                {
                    if (secondPhasePhase == 4)
                    {
                        for (int x = 0; x < Main.maxProjectiles; x++)
                        {
                            Projectile projectile = Main.projectile[x];
                            if (projectile.active)
                            {
                                if (projectile.type == ModContent.ProjectileType<Infernado2>())
                                {
                                    if (projectile.timeLeft >= 300)
                                        projectile.active = false;
                                    else if (projectile.timeLeft > 5)
                                        projectile.timeLeft = (int)(5f * projectile.ai[1]);
                                }
                                else if (projectile.type == ModContent.ProjectileType<BigFlare2>())
                                    projectile.active = false;
                            }
                        }
                    }

                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= 180f)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            float num42 = npc.AngleTo(targetData.Center);
            float num43 = 0.04f;

            switch ((int)npc.ai[0])
            {
                case 2:
                case 5:
                case 7:
                case 8:
                case 9:
                    num43 = 0f;
                    break;
                case 3:
                    num43 = 0.01f;
                    num42 = 0f;

                    if (npc.spriteDirection == -1)
                        num42 -= pie;

                    if (npc.ai[1] >= fireballBreathTimer)
                    {
                        num42 += npc.spriteDirection * pie / 12f;
                        num43 = 0.05f;
                    }

                    break;
                case 4:
                    num43 = 0.01f;
                    num42 = pie;

                    if (npc.spriteDirection == 1)
                        num42 += pie;

                    break;
                case 6:
                    num43 = 0.02f;
                    num42 = 0f;

                    if (npc.spriteDirection == -1)
                        num42 -= pie;

                    break;
            }

            if (npc.spriteDirection == -1)
                num42 += pie;

            if (num43 != 0f)
                npc.rotation = npc.rotation.AngleTowards(num42, num43);
        }
        #endregion

        #region Charge Dust
        private void ChargeDust(int dustAmt, float pie)
        {
            for (int num1474 = 0; num1474 < dustAmt; num1474++)
            {
                Vector2 vector171 = Vector2.Normalize(npc.velocity) * new Vector2((npc.width + 50) / 2f, npc.height) * 0.75f;
                vector171 = vector171.RotatedBy((num1474 - (dustAmt / 2 - 1)) * (double)pie / (float)dustAmt) + npc.Center;
                Vector2 value18 = ((float)(Main.rand.NextDouble() * pie) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                int num1475 = Dust.NewDust(vector171 + value18, 0, 0, 244, value18.X * 2f, value18.Y * 2f, 100, default, 1.4f);
                Main.dust[num1475].noGravity = true;
                Main.dust[num1475].noLight = true;
                Main.dust[num1475].velocity /= 4f;
                Main.dust[num1475].velocity -= npc.velocity;
            }
        }
        #endregion

        #region Flare Dust Bullet Hell
        private void DoFlareDustBulletHell(int attackType, int timer, int projectileDamage, int totalProjectiles, float projectileVelocity, float radialOffset, bool phase2)
        {
            Main.PlaySound(SoundID.Item20, flareDustBulletHellSpawn);
            float aiVariableUsed = phase2 ? npc.ai[1] : npc.ai[2];
            switch (attackType)
            {
                case 0:
                    float offsetAngle = 360 / totalProjectiles;
                    int totalSpaces = totalProjectiles / 5;
                    int spaceStart = Main.rand.Next(totalProjectiles - totalSpaces);
                    float ai0 = aiVariableUsed % (timer * 2) == 0f ? 1f : 0f;

                    int spacesMade = 0;
                    for (int i = 0; i < totalProjectiles; i++)
                    {
                        if (i >= spaceStart && spacesMade < totalSpaces)
                            spacesMade++;
                        else
                            Projectile.NewProjectile(flareDustBulletHellSpawn, Vector2.Zero, ModContent.ProjectileType<FlareDust>(), projectileDamage, 0f, Main.myPlayer, ai0, i * offsetAngle);
                    }
                    break;

                case 1:
                    double radians = MathHelper.TwoPi / totalProjectiles;
                    Vector2 spinningPoint = Vector2.Normalize(new Vector2(-npc.localAI[2], -projectileVelocity));

                    for (int i = 0; i < totalProjectiles; i++)
                    {
                        Vector2 vector2 = spinningPoint.RotatedBy(radians * i) * projectileVelocity;
                        Projectile.NewProjectile(flareDustBulletHellSpawn, vector2, ModContent.ProjectileType<FlareDust>(), projectileDamage, 0f, Main.myPlayer, 2f, 0f);
                    }

                    float newRadialOffset = (int)aiVariableUsed / (timer / 4) % 2f == 0f ? radialOffset : -radialOffset;
                    npc.localAI[2] += newRadialOffset;
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region Fire Ring
        public void DoFireRing(int timeLeft, int damage, float ai0, float ai1)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                float velocity = ai1 == 0f ? 10f : 5f;
                int totalProjectiles = 50;
                float radians = MathHelper.TwoPi / totalProjectiles;
                for (int i = 0; i < totalProjectiles; i++)
                {
                    Vector2 vector255 = new Vector2(0f, -velocity).RotatedBy(radians * i);
                    int proj = Projectile.NewProjectile(npc.Center, vector255, ModContent.ProjectileType<FlareBomb>(), damage, 0f, Main.myPlayer, ai0, ai1);
                    Main.projectile[proj].timeLeft = timeLeft;
                }
            }
        }
        #endregion

        #region Drawing
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            bool idlePhases = (!startSecondAI && (npc.ai[0] == 0f || npc.ai[0] == 6f || npc.ai[0] == 13f)) || (startSecondAI && (npc.ai[0] == 5f || npc.ai[0] < 2f));

            bool chargingOrSpawnPhases = (!startSecondAI && (npc.ai[0] == 1f || npc.ai[0] == 5f || npc.ai[0] == 7f || npc.ai[0] == 11f || npc.ai[0] == 14f || npc.ai[0] == 18f)) ||
                (startSecondAI && (npc.ai[0] == 6f || npc.ai[0] == 2f || npc.ai[0] == 7f));

            bool projectileOrCirclePhases = (!startSecondAI && (npc.ai[0] == 2f || npc.ai[0] == 8f || npc.ai[0] == 12f || npc.ai[0] == 15f || npc.ai[0] == 19f || npc.ai[0] == 20f)) ||
                (startSecondAI && (npc.ai[0] == 4f || npc.ai[0] == 3f));

            bool tornadoPhase = !startSecondAI && (npc.ai[0] == 3f || npc.ai[0] == 9f || npc.ai[0] == -1f || npc.ai[0] == 16f);

            bool newPhasePhase = (!startSecondAI && (npc.ai[0] == 4f || npc.ai[0] == 10f || npc.ai[0] == 17f)) || (startSecondAI && npc.ai[0] == 9f);

            bool pauseAfterTeleportPhase = startSecondAI && npc.ai[0] == 8f;

            bool ai2 = startSecondAI;

            SpriteEffects spriteEffects = ai2 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (npc.spriteDirection == 1)
                spriteEffects = ai2 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Texture2D texture = Main.npcTexture[npc.type];
            Vector2 vector11 = new Vector2(texture.Width / 2, texture.Height / Main.npcFrameCount[npc.type] / 2);
            Color color = lightColor;
            Color invincibleColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0);
            Color color36 = Color.White;

            float amount9 = 0f;
            bool invincible = ai2 && invincibilityCounter < Phase2InvincibilityTime;
            bool flag8 = npc.ai[0] > 5f;
            bool flag9 = npc.ai[0] > 12f;
            bool flag10 = startSecondAI;
            int num150 = 120;
            int num151 = 60;

            if (flag10)
                color = CalamityGlobalNPC.buffColor(color, 0.9f, 0.7f, 0.3f, 1f);
            else if (flag9)
                color = CalamityGlobalNPC.buffColor(color, 0.8f, 0.7f, 0.4f, 1f);
            else if (flag8)
                color = CalamityGlobalNPC.buffColor(color, 0.7f, 0.7f, 0.5f, 1f);
            else if (npc.ai[0] == 4f && npc.ai[2] > num150)
            {
                float num152 = npc.ai[2] - num150;
                num152 /= num151;
                color = CalamityGlobalNPC.buffColor(color, 1f - 0.3f * num152, 1f - 0.3f * num152, 1f - 0.5f * num152, 1f);
            }

            int num153 = 10;
            int num154 = 2;
            if (npc.ai[0] == -1f)
                num153 = 0;
            if (idlePhases)
                num153 = 7;

            if (invincible)
                color36 = invincibleColor;
            else if (chargingOrSpawnPhases)
            {
                color36 = Color.Red;
                amount9 = 0.5f;
            }
            else
                color = lightColor;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += num154)
                {
                    Color color38 = color;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = npc.GetAlpha(color38);
                    color38 *= (num153 - num155) / 15f;
                    Vector2 vector41 = npc.oldPos[num155] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
                    vector41 -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
                    vector41 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
                    spriteBatch.Draw(texture, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
                }
            }

            int num156 = 0;
            float num157 = 0f;
            float scaleFactor9 = 0f;

            if (npc.ai[0] == -1f)
                num156 = 0;

            if (tornadoPhase)
            {
                int num158 = 60;
                int num159 = 30;
                if (npc.ai[2] > num158)
                {
                    num156 = 6;
                    num157 = 1f - (float)Math.Cos((npc.ai[2] - num158) / num159 * MathHelper.TwoPi);
                    num157 /= 3f;
                    scaleFactor9 = 40f;
                }
            }

            if (newPhasePhase && npc.ai[2] > num150)
            {
                num156 = 6;
                num157 = 1f - (float)Math.Cos((npc.ai[2] - num150) / num151 * MathHelper.TwoPi);
                num157 /= 3f;
                scaleFactor9 = 60f;
            }

            if (pauseAfterTeleportPhase)
            {
                num156 = 6;
                num157 = 1f - (float)Math.Cos(npc.ai[2] / 30f * MathHelper.TwoPi);
                num157 /= 3f;
                scaleFactor9 = 20f;
            }

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num160 = 0; num160 < num156; num160++)
                {
                    Color color39 = lightColor;
                    color39 = Color.Lerp(color39, color36, amount9);
                    color39 = npc.GetAlpha(color39);
                    color39 *= 1f - num157;
                    Vector2 vector42 = npc.Center + (num160 / (float)num156 * MathHelper.TwoPi + npc.rotation).ToRotationVector2() * scaleFactor9 * num157 - Main.screenPosition;
                    vector42 -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
                    vector42 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
                    spriteBatch.Draw(texture, vector42, npc.frame, color39, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = npc.Center - Main.screenPosition;
            vector43 -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
            vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
            spriteBatch.Draw(texture, vector43, npc.frame, invincible ? invincibleColor : npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

            if (flag8 || npc.ai[0] == 4f || startSecondAI)
            {
                texture = ModContent.GetTexture("CalamityMod/NPCs/Yharon/YharonGlowOrange");
                Color color40 = Color.Lerp(Color.White, invincible ? invincibleColor : Color.Orange, 0.5f);
                color36 = invincible ? invincibleColor : Color.Orange;

                Texture2D texture2 = ModContent.GetTexture("CalamityMod/NPCs/Yharon/YharonGlowGreen");
                Color color43 = Color.Lerp(Color.White, invincible ? invincibleColor : Color.Chartreuse, 0.5f);
                Color color44 = invincible ? invincibleColor : Color.Chartreuse;

                Texture2D texture3 = ModContent.GetTexture("CalamityMod/NPCs/Yharon/YharonGlowPurple");
                Color color45 = Color.Lerp(Color.White, invincible ? invincibleColor : Color.BlueViolet, 0.5f);
                Color color46 = invincible ? invincibleColor : Color.BlueViolet;

                amount9 = 1f;
                num157 = 0.5f;
                scaleFactor9 = 10f;
                num154 = 1;

                if (newPhasePhase)
                {
                    float num161 = npc.ai[2] - num150;
                    num161 /= num151;
                    color36 *= num161;
                    color40 *= num161;

                    if (flag9 || npc.ai[0] == 10f || startSecondAI)
                    {
                        color43 *= num161;
                        color44 *= num161;
                    }

                    if (flag10 || npc.ai[0] == 17f)
                    {
                        color45 *= num161;
                        color46 *= num161;
                    }
                }

                if (pauseAfterTeleportPhase)
                {
                    float num162 = npc.ai[2];
                    num162 /= 30f;

                    if (num162 > 0.5f)
                        num162 = 1f - num162;

                    num162 *= 2f;
                    num162 = 1f - num162;
                    color36 *= num162;
                    color40 *= num162;
                    color43 *= num162;
                    color44 *= num162;
                    color45 *= num162;
                    color46 *= num162;
                }

                if (CalamityConfig.Instance.Afterimages)
                {
                    for (int num163 = 1; num163 < num153; num163 += num154)
                    {
                        Color color41 = color40;
                        color41 = Color.Lerp(color41, color36, amount9);
                        color41 *= (num153 - num163) / 15f;
                        Vector2 vector44 = npc.oldPos[num163] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
                        vector44 -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
                        vector44 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
                        spriteBatch.Draw(texture, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

                        if (flag9 || npc.ai[0] == 10f || startSecondAI)
                        {
                            Color color47 = color43;
                            color47 = Color.Lerp(color47, color44, amount9);
                            color47 *= (num153 - num163) / 15f;
                            spriteBatch.Draw(texture2, vector44, npc.frame, color47, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
                        }

                        if (flag10 || npc.ai[0] == 17f)
                        {
                            Color color48 = color45;
                            color48 = Color.Lerp(color48, color46, amount9);
                            color48 *= (num153 - num163) / 15f;
                            spriteBatch.Draw(texture3, vector44, npc.frame, color48, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
                        }
                    }

                    for (int num164 = 1; num164 < num156; num164++)
                    {
                        Color color42 = color40;
                        color42 = Color.Lerp(color42, color36, amount9);
                        color42 = npc.GetAlpha(color42);
                        color42 *= 1f - num157;
                        Vector2 vector45 = npc.Center + (num164 / (float)num156 * MathHelper.TwoPi + npc.rotation).ToRotationVector2() * scaleFactor9 * num157 - Main.screenPosition;
                        vector45 -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
                        vector45 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
                        spriteBatch.Draw(texture, vector45, npc.frame, color42, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

                        if (flag9 || npc.ai[0] == 10f || startSecondAI)
                        {
                            Color color49 = color43;
                            color49 = Color.Lerp(color49, color44, amount9);
                            color49 = npc.GetAlpha(color49);
                            color49 *= 1f - num157;
                            spriteBatch.Draw(texture2, vector45, npc.frame, color49, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
                        }

                        if (flag10 || npc.ai[0] == 17f)
                        {
                            Color color50 = color45;
                            color50 = Color.Lerp(color50, color46, amount9);
                            color50 = npc.GetAlpha(color50);
                            color50 *= 1f - num157;
                            spriteBatch.Draw(texture3, vector45, npc.frame, color50, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
                        }
                    }
                }

                spriteBatch.Draw(texture, vector43, npc.frame, color40, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

                if (flag9 || npc.ai[0] == 10f || startSecondAI)
                    spriteBatch.Draw(texture2, vector43, npc.frame, color43, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

                if (flag10 || npc.ai[0] == 17f)
                    spriteBatch.Draw(texture3, vector43, npc.frame, color45, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
            }

            return false;
        }
        #endregion

        #region Loot
        public override void NPCLoot()
        {
            CalamityGlobalNPC.SetNewShopVariable(new int[] { ModContent.NPCType<THIEF>() }, CalamityWorld.downedYharon);

            CalamityGlobalNPC.SetNewBossJustDowned(npc);

            // Bags occur in either phase 1 or 2, as they don't contain phase 2 only drops
            DropHelper.DropBags(npc);

            // Normal drops: Everything that would otherwise be in the bag
            if (!Main.expertMode)
            {
                // Weapons
                float w = DropHelper.NormalWeaponDropRateFloat;
                DropHelper.DropEntireWeightedSet(npc,
                    DropHelper.WeightStack<DragonRage>(w),
                    DropHelper.WeightStack<TheBurningSky>(w),
                    DropHelper.WeightStack<DragonsBreath>(w),
                    DropHelper.WeightStack<ChickenCannon>(w),
                    DropHelper.WeightStack<PhoenixFlameBarrage>(w),
                    DropHelper.WeightStack<AngryChickenStaff>(w), // Yharon Kindle Staff
                    DropHelper.WeightStack<ProfanedTrident>(w), // Infernal Spear
                    DropHelper.WeightStack<FinalDawn>(w),
                    DropHelper.WeightStack<YharimsCrystal>(w)
                );

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<YharonMask>(), 7);
                DropHelper.DropItemChance(npc, ModContent.ItemType<ForgottenDragonEgg>(), 10);
                DropHelper.DropItemChance(npc, ModContent.ItemType<McNuggets>(), 10);

                // Materials
                int soulFragMin = 15;
                int soulFragMax = 22;
                DropHelper.DropItem(npc, ModContent.ItemType<HellcasterFragment>(), true, soulFragMin, soulFragMax);

                // Equipment
                DropHelper.DropItem(npc, ModContent.ItemType<YharimsGift>(), true);
                DropHelper.DropItem(npc, ModContent.ItemType<DrewsWings>(), true);
            }

            // Vanity
            DropHelper.DropItemChance(npc, ModContent.ItemType<YharonTrophy>(), 10);

            // Other
            // DropHelper.DropItem(npc, ModContent.ItemType<BossRush>());
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeYharon>(), true, !CalamityWorld.downedYharon);

            // If Yharon has not been killed yet, notify players of Auric Ore
            if (!CalamityWorld.downedYharon)
            {
                CalamityUtils.SpawnOre(ModContent.TileType<AuricOre>(), 2E-05, 0.6f, 0.8f, 10, 20);

                string key = "Mods.CalamityMod.AuricOreText";
                Color messageColor = Color.Gold;
                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }

            // Mark Yharon as dead
            CalamityWorld.downedYharon = true;
            CalamityNetcode.SyncWorld();
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<OmegaHealingPotion>();
        }
        #endregion

        #region On Hit Player
        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<LethalLavaBurn>(), 420, true);
        }
        #endregion

        #region Projectile Resists
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.type == ModContent.ProjectileType<TimeBoltKnife>())
                damage = (int)(damage * 0.85);
            if (projectile.type == ModContent.ProjectileType<ReaperProjectile>())
                damage = (int)(damage * 0.9);
            if (projectile.type == ModContent.ProjectileType<PhantasmalSoul>() || projectile.type == ModContent.ProjectileType<PhantasmalRuinProj>() || projectile.type == ModContent.ProjectileType<PhantasmalRuinGhost>())
                damage = (int)(damage * 0.95);
        }
        #endregion

        #region HP Bar Cooldown Slot and Stats
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 2f;
            return null;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
        }
        #endregion

        #region Find Frame
        public override void FindFrame(int frameHeight)
        {
            bool idlePhases = (!startSecondAI && (npc.ai[0] == 0f || npc.ai[0] == 6f || npc.ai[0] == 13f)) || (startSecondAI && (npc.ai[0] == 5f || npc.ai[0] < 2f));

            bool chargingOrSpawnPhases = (!startSecondAI && (npc.ai[0] == 1f || npc.ai[0] == 5f || npc.ai[0] == 7f || npc.ai[0] == 11f || npc.ai[0] == 14f || npc.ai[0] == 18f)) ||
                (startSecondAI && (npc.ai[0] == 6f || npc.ai[0] == 2f || npc.ai[0] == 7f));

            bool projectileOrCirclePhases = (!startSecondAI && (npc.ai[0] == 2f || npc.ai[0] == 8f || npc.ai[0] == 12f || npc.ai[0] == 15f || npc.ai[0] == 19f || npc.ai[0] == 20f)) ||
                (startSecondAI && (npc.ai[0] == 4f || npc.ai[0] == 3f || npc.ai[0] == 8f));

            bool tornadoPhase = !startSecondAI && (npc.ai[0] == 3f || npc.ai[0] == 9f || npc.ai[0] == -1f || npc.ai[0] == 16f);

            bool newPhasePhase = (!startSecondAI && (npc.ai[0] == 4f || npc.ai[0] == 10f || npc.ai[0] == 17f)) || (startSecondAI && npc.ai[0] == 9f);

            bool chargeTelegraph = (!startSecondAI && (npc.ai[0] == 0f || npc.ai[0] == 6f || npc.ai[0] == 13f) && npc.localAI[1] > 0f) ||
                (startSecondAI && npc.ai[0] < 2f && npc.localAI[1] > 0f);

            if (chargeTelegraph)
            {
                // Percent life remaining
                float lifeRatio = npc.life / (float)npc.lifeMax;

                bool doTelegraphFlightAnimation = npc.localAI[1] < fastChargeTelegraphTime * 0.5f || npc.localAI[1] > fastChargeTelegraphTime - (fastChargeTelegraphTime / 6f);
                bool doTelegraphRoarAnimation = npc.localAI[1] > fastChargeTelegraphTime - fastChargeTelegraphTime * 0.4f && npc.localAI[1] < fastChargeTelegraphTime - fastChargeTelegraphTime * 0.2f;
                bool phase4 = startSecondAI && lifeRatio <= ((CalamityWorld.death || BossRushEvent.BossRushActive) ? 0.165f : 0.11f) && (CalamityWorld.revenge || BossRushEvent.BossRushActive);
                if (doTelegraphFlightAnimation)
                {
                    npc.frameCounter += phase4 ? 2D : 1D;
                    if (npc.frameCounter > 5D)
                    {
                        npc.frameCounter = 0D;
                        npc.frame.Y += frameHeight;
                    }
                    if (npc.frame.Y >= frameHeight * 5)
                        npc.frame.Y = 0;
                    }
                else
                {
                    npc.frame.Y = frameHeight * 5;
                    if (doTelegraphRoarAnimation)
                        npc.frame.Y = frameHeight * 6;
                    }
                return;
            }

            if (idlePhases)
            {
                int num84 = 5;
                if (!startSecondAI && (npc.ai[0] == 6f || npc.ai[0] == 13f))
                {
                    num84 = 4;
                }
                npc.frameCounter += 1D;
                if (npc.frameCounter > num84)
                {
                    npc.frameCounter = 0D;
                    npc.frame.Y += frameHeight;
                }
                if (npc.frame.Y >= frameHeight * 5)
                {
                    npc.frame.Y = 0;
                }
            }

            if (chargingOrSpawnPhases)
                npc.frame.Y = frameHeight * 5;

            if (projectileOrCirclePhases)
                npc.frame.Y = frameHeight * 5;

            if (tornadoPhase)
            {
                int num85 = 90;
                if (npc.ai[2] < num85 - 30 || npc.ai[2] > num85 - 10)
                {
                    npc.frameCounter += 1D;
                    if (npc.frameCounter > 5D)
                    {
                        npc.frameCounter = 0D;
                        npc.frame.Y += frameHeight;
                    }
                    if (npc.frame.Y >= frameHeight * 5)
                    {
                        npc.frame.Y = 0;
                    }
                }
                else
                {
                    npc.frame.Y = frameHeight * 5;
                    if (npc.ai[2] > num85 - 20 && npc.ai[2] < num85 - 15)
                    {
                        npc.frame.Y = frameHeight * 6;
                    }
                }
            }

            if (newPhasePhase)
            {
                int num86 = 180;
                if (npc.ai[2] < num86 - 60 || npc.ai[2] > num86 - 20)
                {
                    npc.frameCounter += 1D;
                    if (npc.frameCounter > 5D)
                    {
                        npc.frameCounter = 0D;
                        npc.frame.Y += frameHeight;
                    }
                    if (npc.frame.Y >= frameHeight * 5)
                    {
                        npc.frame.Y = 0;
                    }
                }
                else
                {
                    npc.frame.Y = frameHeight * 5;
                    if (npc.ai[2] > num86 - 50 && npc.ai[2] < num86 - 25)
                    {
                        npc.frame.Y = frameHeight * 6;
                    }
                }
            }
        }
        #endregion

        #region Hit Effect
        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                DoFireRing(300, (Main.expertMode || BossRushEvent.BossRushActive) ? 125 : 150, -1f, 0f);
                npc.position.X = npc.position.X + (npc.width / 2);
                npc.position.Y = npc.position.Y + (npc.height / 2);
                npc.width = 300;
                npc.height = 280;
                npc.position.X = npc.position.X - (npc.width / 2);
                npc.position.Y = npc.position.Y - (npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 244, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 244, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 244, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }

                // Turn into dust on death.
                if (npc.life <= 0)
                    DeathAshParticle.CreateAshesFromNPC(npc);
            }
        }
        #endregion
    }
}
