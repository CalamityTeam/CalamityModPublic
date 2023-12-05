using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.VanillaNPCOverrides.Bosses
{
    public static class BrainOfCthulhuAI
    {
        public static bool BuffedBrainofCthulhuAI(NPC npc, Mod mod)
        {
            // whoAmI variable
            NPC.crimsonBoss = npc.whoAmI;

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            float enrageScale = bossRush ? 1f : 0f;
            if ((npc.position.Y / 16f) < Main.worldSurface || bossRush)
            {
                npc.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 1f;
            }
            if (!Main.player[npc.target].ZoneCrimson || bossRush)
            {
                npc.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 2f;
            }

            // Extra distance for teleports if enraged
            int teleportDistanceIncrease = (int)(enrageScale * 3);

            // Spawn Creepers
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] == 0f)
            {
                npc.localAI[0] = 1f;
                int brainOfCthuluCreepersCount = GetBrainOfCthuluCreepersCountRevDeath();
                for (int i = 0; i < brainOfCthuluCreepersCount; i++)
                {
                    float brainX = npc.Center.X;
                    float brainY = npc.Center.Y;
                    brainX += Main.rand.Next(-npc.width, npc.width);
                    brainY += Main.rand.Next(-npc.height, npc.height);

                    int creeperSpawn = NPC.NewNPC(npc.GetSource_FromAI(), (int)brainX, (int)brainY, NPCID.Creeper, 0, 0f, 0f, 0f, 0f, 255);
                    Main.npc[creeperSpawn].velocity = new Vector2(Main.rand.Next(-30, 31) * 0.1f, Main.rand.Next(-30, 31) * 0.1f);
                    Main.npc[creeperSpawn].netUpdate = true;
                }
            }

            // Despawn
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 6000f)
                {
                    npc.active = false;
                    npc.life = 0;

                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                }
            }

            // Phase 2
            if (npc.ai[0] < 0f)
            {
                if (Main.getGoodWorld)
                    NPC.brainOfGravity = npc.whoAmI;

                // Spawn gore
                if (npc.localAI[2] == 0f)
                {
                    SoundEngine.PlaySound(SoundID.NPCHit1, npc.position);

                    npc.localAI[2] = 1f;

                    if (Main.netMode != NetmodeID.Server)
                    {
                        Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 392, 1f);
                        Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 393, 1f);
                        Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 394, 1f);
                        Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 395, 1f);
                    }

                    for (int j = 0; j < 20; j++)
                        Dust.NewDust(npc.position, npc.width, npc.height, 5, Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);

                    SoundEngine.PlaySound(SoundID.Roar, npc.position);
                }

                // Percent life remaining
                float lifeRatio = npc.life / (float)npc.lifeMax;

                // Phases based on HP
                bool phase3 = lifeRatio < 0.6f;
                bool spinning = npc.ai[0] == -4f;

                // KnockBack
                float baseKnockBackResist = death ? 0.4f : 0.45f;

                // Gain defense while spinning
                npc.defense = npc.defDefense + (spinning ? 7 : 0);

                // Take damage
                npc.dontTakeDamage = false;

                // Deal no damage while spinning
                npc.damage = spinning ? 0 : npc.defDamage;

                // Target distance X
                float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;

                // Charge
                if (!spinning)
                {
                    // Not charging
                    if (npc.ai[0] != -6f)
                    {
                        // Rubber band movement
                        Vector2 brainCenter = new Vector2(npc.Center.X, npc.Center.Y);
                        float targetXDist = Main.player[npc.target].Center.X - brainCenter.X;
                        float targetYDist = Main.player[npc.target].Center.Y - brainCenter.Y;
                        float targetDistance = (float)Math.Sqrt(targetXDist * targetXDist + targetYDist * targetYDist);
                        float velocityScale = death ? 4f : 2f;
                        float velocityBoost = velocityScale * (1f - lifeRatio);
                        float nonChargeSpeed = 8f + velocityBoost + 3f * enrageScale;
                        if (Main.getGoodWorld)
                            nonChargeSpeed *= 1.15f;

                        targetDistance = nonChargeSpeed / targetDistance;
                        targetXDist *= targetDistance;
                        targetYDist *= targetDistance;
                        npc.velocity.X = (npc.velocity.X * 50f + targetXDist) / 51f;
                        npc.velocity.Y = (npc.velocity.Y * 50f + targetYDist) / 51f;
                    }

                    // Charge, -6
                    else
                    {
                        npc.ai[1] += 1f;

                        // Teleport
                        float timeGateValue = death ? 90f : 115f;
                        if (npc.ai[1] >= timeGateValue)
                        {
                            if (npc.knockBackResist == 0f)
                                npc.knockBackResist = GetCrimsonBossKnockBack(npc, CalamityGlobalNPC.GetActivePlayerCount(), lifeRatio, baseKnockBackResist);

                            npc.ai[0] = -7f;
                            npc.ai[1] = 0f;
                            npc.localAI[1] = 120f;
                            npc.netUpdate = true;
                        }

                        // Charge sound and velocity
                        else if (npc.ai[1] == 10f)
                        {
                            // Sound
                            SoundEngine.PlaySound(SoundID.ForceRoar, npc.position);

                            // Velocity
                            npc.velocity = Vector2.Normalize(Main.player[npc.target].Center + (bossRush ? Main.player[npc.target].velocity * 20f : Vector2.Zero) - npc.Center) * ((death ? 20f : 16f) + 4f * enrageScale);
                            if (Main.getGoodWorld)
                                npc.velocity *= 1.15f;
                        }
                    }

                    // Rubber band movement, -5
                    if (npc.ai[0] == -5f)
                    {
                        // Spin or teleport
                        npc.ai[2] += 1f;
                        if (npc.ai[2] >= 180f)
                        {
                            // Velocity and knockback
                            npc.knockBackResist = 0f;
                            npc.velocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * 24f;
                            npc.ai[0] = -4f;
                            npc.ai[1] = playerLocation < 0 ? 1f : -1f;
                            npc.ai[2] = 0f;
                            npc.ai[3] = Main.rand.Next(61);
                            npc.localAI[1] = 0f;
                            npc.netUpdate = true;
                        }
                    }
                }

                // Circle around, -4
                if (spinning)
                {
                    // Charge sound
                    if (npc.ai[2] == 0f)
                    {
                        SoundEngine.PlaySound(SoundID.Roar, npc.position);

                        if (CalamityWorld.LegendaryMode)
                        {
                            if (Main.netMode != NetmodeID.Server)
                            {
                                if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, npc.Center) < CalamityGlobalNPC.CatchUpDistance350Tiles)
                                    Main.player[Main.myPlayer].AddBuff(BuffID.Confused, 90);
                            }
                        }
                    }

                    // Velocity
                    float velocity = MathHelper.TwoPi / (120 * 0.75f);
                    npc.velocity = npc.velocity.RotatedBy(-(double)velocity * npc.ai[1]);

                    npc.ai[2] += 1f;

                    float timer = (death ? 0f : 30f) + npc.ai[3];

                    if (npc.ai[2] >= timer - 5f)
                    {
                        if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) < 400f) // 25 tile distance
                        {
                            npc.ai[2] -= 1f;
                            npc.velocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * ((death ? -10f : -8f) - 2f * enrageScale);
                            if (Main.getGoodWorld)
                                npc.velocity *= 1.15f;
                        }
                    }

                    // Charge at target, -6 is straight line movement, -5 is rubber band movement
                    if (npc.ai[2] >= timer)
                    {
                        // Complete stop
                        npc.velocity *= 0f;

                        // Adjust knockback
                        npc.knockBackResist = 0f;

                        npc.ai[0] = -6f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.TargetClosest();
                        npc.netUpdate = true;
                    }
                }

                // Pick teleport location
                else if (npc.ai[0] == -1f || npc.ai[0] == -7f)
                {
                    // Adjust knockback
                    if (npc.ai[0] == -1f)
                    {
                        if (npc.knockBackResist == 0f)
                            npc.knockBackResist = GetCrimsonBossKnockBack(npc, CalamityGlobalNPC.GetActivePlayerCount(), lifeRatio, baseKnockBackResist);
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        // Go to phase 3
                        if (phase3 && npc.ai[0] == -1f)
                        {
                            npc.ai[0] = -5f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                            npc.ai[3] = 0f;
                            npc.localAI[1] = 0f;
                            npc.alpha = 0;
                            npc.netUpdate = true;
                        }

                        npc.localAI[1] += 1f;

                        int randomReduction = death ? 4 : (int)(5f * (1f - lifeRatio));
                        int random = 5 - randomReduction;
                        if (npc.justHit)
                            npc.localAI[1] -= Main.rand.Next(random);

                        if (npc.localAI[1] >= 60f)
                        {
                            npc.localAI[1] = 0f;
                            npc.TargetClosest();
                            int numTeleportTries = 0;
                            int teleportTileX;
                            int teleportTileY;
                            while (true)
                            {
                                numTeleportTries++;
                                teleportTileX = (int)Main.player[npc.target].Center.X / 16;
                                teleportTileY = (int)Main.player[npc.target].Center.Y / 16;

                                int min = 14;
                                int max = 17;

                                min += teleportDistanceIncrease;
                                max += teleportDistanceIncrease;

                                if (Main.rand.NextBool())
                                    teleportTileX += Main.rand.Next(min, max);
                                else
                                    teleportTileX -= Main.rand.Next(min, max);

                                if (Main.rand.NextBool())
                                    teleportTileY += Main.rand.Next(min, max);
                                else
                                    teleportTileY -= Main.rand.Next(min, max);

                                if (!WorldGen.SolidTile(teleportTileX, teleportTileY))
                                    break;

                                if (numTeleportTries > 100)
                                    goto Block_2784;
                            }
                            npc.ai[3] = 0f;
                            npc.ai[0] = npc.ai[0] == -7f ? -8f : -2f;
                            npc.ai[1] = teleportTileX;
                            npc.ai[2] = teleportTileY;
                            npc.netUpdate = true;
                            npc.netSpam = 0;
                            Block_2784:
                            ;
                        }
                    }
                }

                // Teleport and turn invisible
                else if (npc.ai[0] == -2f || npc.ai[0] == -8f)
                {
                    // Avoid cheap bullshit
                    npc.damage = 0;

                    npc.velocity *= 0.9f;

                    if (Main.netMode != NetmodeID.SinglePlayer)
                        npc.ai[3] += 15f;
                    else
                        npc.ai[3] += 25f;

                    if (npc.ai[3] >= 255f)
                    {
                        npc.ai[3] = 255f;
                        npc.position.X = npc.ai[1] * 16f - (npc.width / 2);
                        npc.position.Y = npc.ai[2] * 16f - (npc.height / 2);
                        SoundEngine.PlaySound(SoundID.Item8, npc.Center);
                        npc.ai[0] = npc.ai[0] == -8f ? -9f : -3f;
                        npc.netUpdate = true;
                        npc.netSpam = 0;
                    }

                    npc.alpha = (int)npc.ai[3];
                }

                // Become visible
                else if (npc.ai[0] == -3f || npc.ai[0] == -9f)
                {
                    // Avoid cheap bullshit
                    npc.damage = 0;

                    if (Main.netMode != NetmodeID.SinglePlayer)
                        npc.ai[3] -= 15f;
                    else
                        npc.ai[3] -= 25f;

                    if (npc.ai[3] <= 0f)
                    {
                        if (npc.ai[0] == -9f)
                        {
                            // Adjust knockback
                            npc.knockBackResist = 0f;

                            npc.velocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * 24f;

                            npc.ai[0] = -4f;
                            npc.ai[1] = playerLocation < 0 ? 1f : -1f;
                            npc.ai[2] = 0f;
                            npc.ai[3] = Main.rand.Next(61);
                        }
                        else
                        {
                            npc.ai[3] = 0f;
                            npc.ai[2] = 0f;
                            npc.ai[1] = 0f;
                            npc.ai[0] = -1f;
                        }
                        npc.netUpdate = true;
                        npc.netSpam = 0;
                    }

                    npc.alpha = (int)npc.ai[3];
                }
            }

            // Phase 1
            else
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                // Creeper count
                int creeperCount = NPC.CountNPCS(NPCID.Creeper);
                int creeperScale = GetBrainOfCthuluCreepersCountRevDeath() + 1 - creeperCount;
                creeperScale *= (int)enrageScale;
                bool phase2 = creeperCount <= 0;

                // Go to phase 2
                if (phase2)
                {
                    npc.ai[0] = -1f;
                    npc.localAI[1] = 0f;
                    npc.alpha = 0;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                    return false;
                }

                // Move towards target
                Vector2 brainCenterPhase1 = new Vector2(npc.Center.X, npc.Center.Y);
                float targetXDistPhase1 = Main.player[npc.target].Center.X - brainCenterPhase1.X;
                float targetYDistPhase1 = Main.player[npc.target].Center.Y - brainCenterPhase1.Y;
                float targetDistancePhase1 = (float)Math.Sqrt(targetXDistPhase1 * targetXDistPhase1 + targetYDistPhase1 * targetYDistPhase1);
                float maxMoveVelocity = (death ? 2f : 1f) + (creeperScale * 0.1f);
                if (Main.getGoodWorld)
                    maxMoveVelocity *= 2f;

                if (targetDistancePhase1 < maxMoveVelocity)
                {
                    npc.velocity.X = targetXDistPhase1;
                    npc.velocity.Y = targetYDistPhase1;
                }
                else
                {
                    targetDistancePhase1 = maxMoveVelocity / targetDistancePhase1;
                    npc.velocity.X = targetXDistPhase1 * targetDistancePhase1;
                    npc.velocity.Y = targetYDistPhase1 * targetDistancePhase1;
                }

                // Pick a teleport location
                if (npc.ai[0] == 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        // Teleport location
                        npc.localAI[1] += (death ? 2f : 1f) + (creeperScale * 0.1f);
                        if (npc.localAI[1] >= 300f)
                        {
                            npc.localAI[1] = 0f;
                            npc.TargetClosest();
                            int phase1TeleportTries = 0;
                            int phase1TeleportTileX;
                            int phase1TeleportTileY;
                            while (true)
                            {
                                phase1TeleportTries++;
                                phase1TeleportTileX = (int)Main.player[npc.target].Center.X / 16;
                                phase1TeleportTileY = (int)Main.player[npc.target].Center.Y / 16;

                                int min = 18 + teleportDistanceIncrease;
                                int max = 26 + teleportDistanceIncrease;

                                if (Main.rand.NextBool())
                                    phase1TeleportTileX += Main.rand.Next(min, max);
                                else
                                    phase1TeleportTileX -= Main.rand.Next(min, max);

                                if (Main.rand.NextBool())
                                    phase1TeleportTileY += Main.rand.Next(min, max);
                                else
                                    phase1TeleportTileY -= Main.rand.Next(min, max);

                                if (!WorldGen.SolidTile(phase1TeleportTileX, phase1TeleportTileY) && Collision.CanHit(new Vector2(phase1TeleportTileX * 16, phase1TeleportTileY * 16), 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                                {
                                    break;
                                }
                                if (phase1TeleportTries > 100)
                                {
                                    goto Block_2801;
                                }
                            }
                            npc.ai[0] = 1f;
                            npc.ai[1] = phase1TeleportTileX;
                            npc.ai[2] = phase1TeleportTileY;
                            npc.netUpdate = true;
                            Block_2801:
                            ;
                        }
                    }
                }

                // Turn invisible and teleport
                else if (npc.ai[0] == 1f)
                {
                    // Avoid cheap bullshit
                    npc.damage = 0;

                    npc.alpha += 25;
                    if (npc.alpha >= 255)
                    {
                        SoundEngine.PlaySound(SoundID.Item8, npc.Center);
                        npc.alpha = 255;
                        npc.position.X = npc.ai[1] * 16f - (npc.width / 2);
                        npc.position.Y = npc.ai[2] * 16f - (npc.height / 2);
                        npc.ai[0] = 2f;
                    }
                }

                // Become visible
                else if (npc.ai[0] == 2f)
                {
                    // Avoid cheap bullshit
                    npc.damage = 0;

                    npc.alpha -= 25;
                    if (npc.alpha <= 0)
                    {
                        npc.alpha = 0;
                        npc.ai[0] = 0f;
                    }
                }
            }

            // Despawn
            if (Main.player[npc.target].dead && !bossRush)
            {
                if (npc.localAI[3] < 120f)
                    npc.localAI[3] += 1f;

                if (npc.localAI[3] > 60f)
                    npc.velocity.Y += (npc.localAI[3] - 60f) * 0.25f;

                npc.ai[0] = 2f;
                npc.alpha = 10;
                return false;
            }
            if (npc.localAI[3] > 0f)
                npc.localAI[3] -= 1f;

            return false;
        }

        public static bool BuffedCreeperAI(NPC npc, Mod mod)
        {
            // Despawn if Brain is gone
            if (NPC.crimsonBoss < 0)
            {
                npc.active = false;
                npc.netUpdate = true;
                return false;
            }

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            float enrageScale = bossRush ? 1f : 0f;
            if ((npc.position.Y / 16f) < Main.worldSurface || bossRush)
                enrageScale += 1f;
            if (!Main.player[npc.target].ZoneCrimson || bossRush)
                enrageScale += 2f;

            // Creeper count, 0 to 20
            int creeperCount = NPC.CountNPCS(NPCID.Creeper);

            // Stay near Brain
            if (npc.ai[0] == 0f)
            {
                Vector2 creeperCenter = new Vector2(npc.Center.X, npc.Center.Y);
                float brainXDist = Main.npc[NPC.crimsonBoss].Center.X - creeperCenter.X;
                float brainYDist = Main.npc[NPC.crimsonBoss].Center.Y - creeperCenter.Y;
                float brainDistance = (float)Math.Sqrt(brainXDist * brainXDist + brainYDist * brainYDist);
                float velocity = death ? 10f : 7f;
                velocity += 4f * enrageScale;

                // Max distance from Brain
                if (brainDistance > 90f)
                {
                    brainDistance = velocity / brainDistance;
                    brainXDist *= brainDistance;
                    brainYDist *= brainDistance;
                    npc.velocity.X = (npc.velocity.X * 15f + brainXDist) / 16f;
                    npc.velocity.Y = (npc.velocity.Y * 15f + brainYDist) / 16f;
                    return false;
                }

                // Increase speed
                if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < velocity)
                {
                    npc.velocity *= 1.05f;
                }

                // Charge at target
                int creeperScale = GetBrainOfCthuluCreepersCountRevDeath() + 1 - creeperCount;
                creeperScale *= (int)enrageScale;
                npc.ai[1] += (death ? 1f : 0.5f) + (creeperScale * 0.25f);
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[1] >= 150f)
                {
                    npc.ai[1] = 0f;
                    npc.TargetClosest();
                    creeperCenter = new Vector2(npc.Center.X, npc.Center.Y);
                    brainXDist = Main.player[npc.target].Center.X - creeperCenter.X;
                    brainYDist = Main.player[npc.target].Center.Y - creeperCenter.Y;
                    brainDistance = (float)Math.Sqrt(brainXDist * brainXDist + brainYDist * brainYDist);
                    brainDistance = velocity / brainDistance;
                    npc.velocity.X = brainXDist * brainDistance;
                    npc.velocity.Y = brainYDist * brainDistance;
                    npc.ai[0] = 1f;
                    npc.netUpdate = true;
                }
            }

            // Charge
            else
            {
                float chargeVelocity = death ? 10f : 7f;
                chargeVelocity += 4f * enrageScale;
                Vector2 targetDirection = Main.player[npc.target].Center - npc.Center;
                targetDirection.Normalize();
                if (Main.getGoodWorld)
                {
                    targetDirection *= 12f;
                    npc.velocity = (npc.velocity * 49f + targetDirection) / 50f;
                }
                else
                {
                    targetDirection *= chargeVelocity;
                    npc.velocity = (npc.velocity * 99f + targetDirection) / 100f;
                }

                Vector2 creeperCenterCharge = new Vector2(npc.Center.X, npc.Center.Y);
                float brainXDistCharge = Main.npc[NPC.crimsonBoss].Center.X - creeperCenterCharge.X;
                float brainYDistCharge = Main.npc[NPC.crimsonBoss].Center.Y - creeperCenterCharge.Y;
                float brainDistanceCharge = (float)Math.Sqrt(brainXDistCharge * brainXDistCharge + brainYDistCharge * brainYDistCharge);

                // Return to Brain
                if (brainDistanceCharge > 700f || npc.justHit)
                    npc.ai[0] = 0f;
            }

            // Push away from each other in death mode
            if (death)
            {
                float pushVelocity = 0.5f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active)
                    {
                        if (i != npc.whoAmI && Main.npc[i].type == npc.type)
                        {
                            if (Vector2.Distance(npc.Center, Main.npc[i].Center) < 48f)
                            {
                                if (npc.position.X < Main.npc[i].position.X)
                                    npc.velocity.X -= pushVelocity;
                                else
                                    npc.velocity.X += pushVelocity;

                                if (npc.position.Y < Main.npc[i].position.Y)
                                    npc.velocity.Y -= pushVelocity;
                                else
                                    npc.velocity.Y += pushVelocity;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static int GetBrainOfCthuluCreepersCountRevDeath()
        {
            return CalamityWorld.LegendaryMode ? 50 : Main.getGoodWorld ? 40 : (CalamityWorld.death || BossRushEvent.BossRushActive) ? 30 : 25;
        }

        private static float GetCrimsonBossKnockBack(NPC npc, int numPlayers, float lifeScale, float baseKnockBackResist)
        {
            float balance = 1f;
            float boost = 0.35f;

            for (int i = 1; i < numPlayers; i++)
            {
                balance += boost;
                boost += (1f - boost) / 3f;
            }

            if (balance > 8f)
                balance = (balance * 2f + 8f) / 3f;
            if (balance > 1000f)
                balance = 1000f;

            float KBResist = baseKnockBackResist * lifeScale;
            float KBResistMultiplier = 1f - baseKnockBackResist * 0.4f;
            for (float num = 1f; num < balance; num += 0.34f)
            {
                if (KBResist < 0.05)
                {
                    KBResist = 0f;
                    break;
                }
                KBResist *= KBResistMultiplier;
            }

            return KBResist;
        }
    }
}
