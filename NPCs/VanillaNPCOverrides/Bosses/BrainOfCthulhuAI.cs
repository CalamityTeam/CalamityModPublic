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
                for (int num789 = 0; num789 < brainOfCthuluCreepersCount; num789++)
                {
                    float num790 = npc.Center.X;
                    float num791 = npc.Center.Y;
                    num790 += Main.rand.Next(-npc.width, npc.width);
                    num791 += Main.rand.Next(-npc.height, npc.height);

                    int num792 = NPC.NewNPC(npc.GetSource_FromAI(), (int)num790, (int)num791, NPCID.Creeper, 0, 0f, 0f, 0f, 0f, 255);
                    Main.npc[num792].velocity = new Vector2(Main.rand.Next(-30, 31) * 0.1f, Main.rand.Next(-30, 31) * 0.1f);
                    Main.npc[num792].netUpdate = true;
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

                    for (int num794 = 0; num794 < 20; num794++)
                        Dust.NewDust(npc.position, npc.width, npc.height, 5, Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);

                    SoundEngine.PlaySound(SoundID.Roar, npc.position);
                }

                // Percent life remaining
                float lifeRatio = npc.life / (float)npc.lifeMax;

                // Phases based on HP
                bool phase2 = lifeRatio < 0.8f;
                bool phase3 = lifeRatio < 0.65f;
                bool phase4 = lifeRatio < 0.5f;
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
                        Vector2 vector98 = new Vector2(npc.Center.X, npc.Center.Y);
                        float num795 = Main.player[npc.target].Center.X - vector98.X;
                        float num796 = Main.player[npc.target].Center.Y - vector98.Y;
                        float num797 = (float)Math.Sqrt(num795 * num795 + num796 * num796);
                        float velocityScale = death ? 4f : 2f;
                        float velocityBoost = velocityScale * (1f - lifeRatio);
                        float num798 = 8f + velocityBoost + 3f * enrageScale;
                        if (Main.getGoodWorld)
                            num798 *= 1.15f;

                        num797 = num798 / num797;
                        num795 *= num797;
                        num796 *= num797;
                        npc.velocity.X = (npc.velocity.X * 50f + num795) / 51f;
                        npc.velocity.Y = (npc.velocity.Y * 50f + num796) / 51f;
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
                            bool spin = phase3 ? Main.rand.Next(4) > 0 : Main.rand.NextBool();
                            if (phase4)
                                spin = true;

                            // Velocity and knockback
                            if (spin)
                            {
                                npc.knockBackResist = 0f;
                                npc.velocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * 24f;
                            }
                            else
                            {
                                if (npc.knockBackResist == 0f)
                                    npc.knockBackResist = GetCrimsonBossKnockBack(npc, CalamityGlobalNPC.GetActivePlayerCount(), lifeRatio, baseKnockBackResist);
                            }

                            npc.ai[0] = !spin ? -7f : -4f;
                            npc.ai[1] = !spin ? 0f : (playerLocation < 0 ? 1f : -1f);
                            npc.ai[2] = 0f;
                            npc.ai[3] = !spin ? 0f : Main.rand.Next(61);
                            npc.localAI[1] = !spin ? 120f : 0f;
                            npc.netUpdate = true;
                        }
                    }
                }

                // Circle around, -4
                if (spinning)
                {
                    // Charge sound
                    if (npc.ai[2] == 0f)
                        SoundEngine.PlaySound(SoundID.Roar, npc.position);

                    // Velocity
                    int var = 120;
                    float velocity = MathHelper.TwoPi / (var * 0.75f);
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

                        bool charge = phase3 ? Main.rand.Next(4) > 0 : Main.rand.NextBool();
                        if (phase4)
                            charge = true;

                        // Adjust knockback
                        if (charge)
                            npc.knockBackResist = 0f;
                        else
                        {
                            if (npc.knockBackResist == 0f)
                                npc.knockBackResist = GetCrimsonBossKnockBack(npc, CalamityGlobalNPC.GetActivePlayerCount(), lifeRatio, baseKnockBackResist);
                        }

                        npc.ai[0] = charge ? -6f : -5f;
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
                        if (phase2 && npc.ai[0] == -1f)
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

                        float num799 = phase2 ? 60f : 90f;
                        if (npc.localAI[1] >= num799)
                        {
                            npc.localAI[1] = 0f;
                            npc.TargetClosest();
                            int num800 = 0;
                            int num801;
                            int num802;
                            while (true)
                            {
                                num800++;
                                num801 = (int)Main.player[npc.target].Center.X / 16;
                                num802 = (int)Main.player[npc.target].Center.Y / 16;

                                int min = 14;
                                int max = 17;

                                min += teleportDistanceIncrease;
                                max += teleportDistanceIncrease;

                                if (Main.rand.NextBool(2))
                                    num801 += Main.rand.Next(min, max);
                                else
                                    num801 -= Main.rand.Next(min, max);

                                if (Main.rand.NextBool(2))
                                    num802 += Main.rand.Next(min, max);
                                else
                                    num802 -= Main.rand.Next(min, max);

                                if (!WorldGen.SolidTile(num801, num802))
                                    break;

                                if (num800 > 100)
                                    goto Block_2784;
                            }
                            npc.ai[3] = 0f;
                            npc.ai[0] = npc.ai[0] == -7f ? -8f : -2f;
                            npc.ai[1] = num801;
                            npc.ai[2] = num802;
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
                        bool spin = phase3 ? (Main.rand.NextBool() && npc.ai[0] == -9f) : false;
                        if (phase4 && npc.ai[0] == -9f)
                            spin = true;

                        if (spin)
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
                            npc.ai[0] = npc.ai[0] == -9f ? -5f : -1f;
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
                // Set damage
                npc.damage = npc.defDamage;

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
                Vector2 vector99 = new Vector2(npc.Center.X, npc.Center.Y);
                float num803 = Main.player[npc.target].Center.X - vector99.X;
                float num804 = Main.player[npc.target].Center.Y - vector99.Y;
                float num805 = (float)Math.Sqrt(num803 * num803 + num804 * num804);
                float num806 = (death ? 2f : 1f) + (creeperScale * 0.1f);
                if (Main.getGoodWorld)
                    num806 *= 3f;

                if (num805 < num806)
                {
                    npc.velocity.X = num803;
                    npc.velocity.Y = num804;
                }
                else
                {
                    num805 = num806 / num805;
                    npc.velocity.X = num803 * num805;
                    npc.velocity.Y = num804 * num805;
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
                            int num809 = 0;
                            int num810;
                            int num811;
                            while (true)
                            {
                                num809++;
                                num810 = (int)Main.player[npc.target].Center.X / 16;
                                num811 = (int)Main.player[npc.target].Center.Y / 16;

                                int min = 18 + teleportDistanceIncrease;
                                int max = 26 + teleportDistanceIncrease;

                                if (Main.rand.NextBool(2))
                                    num810 += Main.rand.Next(min, max);
                                else
                                    num810 -= Main.rand.Next(min, max);

                                if (Main.rand.NextBool(2))
                                    num811 += Main.rand.Next(min, max);
                                else
                                    num811 -= Main.rand.Next(min, max);

                                if (!WorldGen.SolidTile(num810, num811) && Collision.CanHit(new Vector2(num810 * 16, num811 * 16), 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                                {
                                    break;
                                }
                                if (num809 > 100)
                                {
                                    goto Block_2801;
                                }
                            }
                            npc.ai[0] = 1f;
                            npc.ai[1] = num810;
                            npc.ai[2] = num811;
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
            bool phase2 = creeperCount <= (death ? 12 : 6);

            // Adjust knockback
            if (phase2)
                npc.knockBackResist = 0f;

            // Stay near Brain
            if (npc.ai[0] == 0f)
            {
                Vector2 vector100 = new Vector2(npc.Center.X, npc.Center.Y);
                float num812 = Main.npc[NPC.crimsonBoss].Center.X - vector100.X;
                float num813 = Main.npc[NPC.crimsonBoss].Center.Y - vector100.Y;
                float num814 = (float)Math.Sqrt(num812 * num812 + num813 * num813);
                float velocity = death ? 10f : 7f;
                velocity += 4f * enrageScale;

                // Max distance from Brain
                if (num814 > 90f)
                {
                    num814 = velocity / num814;
                    num812 *= num814;
                    num813 *= num814;
                    npc.velocity.X = (npc.velocity.X * 15f + num812) / 16f;
                    npc.velocity.Y = (npc.velocity.Y * 15f + num813) / 16f;
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
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[1] >= 240f)
                {
                    npc.ai[1] = 0f;
                    npc.TargetClosest();
                    vector100 = new Vector2(npc.Center.X, npc.Center.Y);
                    num812 = Main.player[npc.target].Center.X - vector100.X;
                    num813 = Main.player[npc.target].Center.Y - vector100.Y;
                    num814 = (float)Math.Sqrt(num812 * num812 + num813 * num813);
                    num814 = velocity / num814;
                    npc.velocity.X = num812 * num814;
                    npc.velocity.Y = num813 * num814;
                    npc.ai[0] = 1f;
                    npc.netUpdate = true;
                }
            }

            // Charge
            else
            {
                float chargeVelocity = death ? 10f : 7f;
                chargeVelocity += 4f * enrageScale;
                Vector2 value2 = Main.player[npc.target].Center - npc.Center;
                value2.Normalize();
                if (Main.getGoodWorld)
                {
                    value2 *= 12f;
                    npc.velocity = (npc.velocity * 49f + value2) / 50f;
                }
                else
                {
                    value2 *= chargeVelocity;
                    npc.velocity = (npc.velocity * 99f + value2) / 100f;
                }

                Vector2 vector101 = new Vector2(npc.Center.X, npc.Center.Y);
                float num815 = Main.npc[NPC.crimsonBoss].Center.X - vector101.X;
                float num816 = Main.npc[NPC.crimsonBoss].Center.Y - vector101.Y;
                float num817 = (float)Math.Sqrt(num815 * num815 + num816 * num816);

                // Return to Brain
                if (num817 > 700f || (npc.justHit && !phase2))
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
            return Main.getGoodWorld ? 40 : (CalamityWorld.death || BossRushEvent.BossRushActive) ? 30 : 25;
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
