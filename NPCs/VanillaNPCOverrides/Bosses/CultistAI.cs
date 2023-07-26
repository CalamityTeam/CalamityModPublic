using CalamityMod.Events;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.VanillaNPCOverrides.Bosses
{
    public static class CultistAI
    {
        public static bool BuffedCultistAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            // Chant sound
            if (npc.ai[0] != -1f && Main.rand.NextBool(1000))
            {
                SoundStyle chantSound = Utils.SelectRandom(Main.rand, new SoundStyle[]
                {
                SoundID.Zombie88,
                SoundID.Zombie89,
                SoundID.Zombie90,
                SoundID.Zombie91
                });

                SoundEngine.PlaySound(chantSound, npc.position);
            }

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Phases
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool phase2 = lifeRatio < 0.85f;
            bool phase3 = lifeRatio < 0.7f;
            bool phase4 = lifeRatio < 0.55f;
            bool phase5 = lifeRatio < 0.4f;
            bool phase6 = lifeRatio < 0.25f;

            // Variables
            bool isCultist = npc.type == NPCID.CultistBoss;
            bool dontTakeDamage = false;

            int iceMistDamage = isCultist ? npc.GetProjectileDamage(ProjectileID.CultistBossIceMist) : 0;
            int fireballDamage = isCultist ? npc.GetProjectileDamage(ProjectileID.CultistBossFireBall) : npc.GetProjectileDamage(ProjectileID.CultistBossFireBallClone);
            int lightningDamage = isCultist ? npc.GetProjectileDamage(ProjectileID.CultistBossLightningOrb) : 0;

            int iceMistFireRate = phase2 ? 50 : 60;
            float iceMistSpeed = (phase6 ? 12f : 10f) + (death ? 2f * (1f - lifeRatio) : 0f);
            int iceMistAmt = phase3 ? 2 : 1;
            int fireballFireRate = phase5 ? 10 : 12;
            float fireballSpeed = (phase6 ? 7.5f : 6f) + (death ? 2f * (1f - lifeRatio) : 0f) - (isCultist ? 0f : 3f);
            int lightningOrbPhaseTime = phase2 ? 90 : 120;
            int ancientLightSpawnRate = phase4 ? 25 : 30;
            int ancientLightAmt = phase4 ? 3 : 2;
            int ancientDoomLimit = 10;
            int idleTime = phase3 ? 35 : 40;
            float timeToFinishRitual = phase5 ? 300f : 360f;

            if (bossRush)
            {
                iceMistFireRate = 40;
                iceMistSpeed = 14f;
                iceMistAmt = 3;
                fireballFireRate = 8;
                fireballSpeed *= 1.2f;
                lightningOrbPhaseTime = 90;
                ancientLightSpawnRate = 20;
                ancientLightAmt = 4;
                idleTime = 30;
            }

            if (Main.getGoodWorld)
            {
                iceMistFireRate = 30;
                iceMistSpeed = 15f;
                fireballFireRate = 6;
                fireballSpeed *= 1.25f;
                lightningOrbPhaseTime = 60;
                ancientLightSpawnRate = 10;
                ancientLightAmt = 5;
                idleTime = 20;
            }

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest(false);

            // Center and target
            Player player = Main.player[npc.target];
            if (npc.target < 0 || npc.target == Main.maxPlayers || player.dead || !player.active)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                npc.netUpdate = true;
            }

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(player.Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest(false);

            // Enrage
            if (!Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height) || CalamityWorld.LegendaryMode)
            {
                calamityGlobalNPC.newAI[0] += 1f;
                if (calamityGlobalNPC.newAI[0] >= 120f)
                {
                    calamityGlobalNPC.newAI[0] = 120f;
                    iceMistSpeed = 16f;
                    iceMistFireRate = 15;
                    lightningOrbPhaseTime = 30;
                    ancientLightSpawnRate = 5;
                    idleTime = 10;
                    timeToFinishRitual = 120f;
                }
            }
            else
            {
                if (calamityGlobalNPC.newAI[0] > 0f)
                    calamityGlobalNPC.newAI[0] -= 1f;
            }

            // Cultist clone AI
            if (!isCultist)
            {
                if (npc.ai[3] < 0f || !Main.npc[(int)npc.ai[3]].active || Main.npc[(int)npc.ai[3]].type != NPCID.CultistBoss)
                {
                    npc.life = 0;
                    npc.HitEffect(0, 10.0);
                    npc.active = false;
                    return false;
                }

                npc.ai[0] = Main.npc[(int)npc.ai[3]].ai[0];
                npc.ai[1] = Main.npc[(int)npc.ai[3]].ai[1];
                dontTakeDamage = true;
            }

            // Stop spawning ritual if hit
            else if (npc.ai[0] == 5f && npc.ai[1] >= 120f && npc.ai[1] < timeToFinishRitual && npc.justHit)
            {
                npc.ai[0] = 0f;
                npc.ai[1] = 0f;
                npc.ai[3] += 1f;
                npc.velocity = Vector2.Zero;
                npc.netUpdate = true;
                Main.projectile[(int)npc.ai[2]].ai[1] = -1f;
                Main.projectile[(int)npc.ai[2]].netUpdate = true;
            }

            // Despawn
            if (player.dead || Vector2.Distance(player.Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance350Tiles)
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.active = false;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);

                for (int j = 0; j < Main.maxNPCs; j++)
                {
                    if (Main.npc[j].active && Main.npc[j].type == NPCID.CultistBossClone && Main.npc[j].ai[3] == npc.whoAmI)
                    {
                        Main.npc[j].life = 0;
                        Main.npc[j].HitEffect(0, 10.0);
                        Main.npc[j].active = false;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                    }
                }
            }

            // Clones set to Cultist phase
            float clonePhase = npc.ai[3];

            // Spawn and play sound
            if (npc.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Zombie89, npc.position);
                npc.localAI[0] = 1f;
                npc.alpha = 255;
                npc.rotation = 0f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ai[0] = -1f;
                    npc.netUpdate = true;
                }
            }

            // Appear and do weird ritual shit with tablet
            if (npc.ai[0] == -1f)
            {
                npc.alpha -= 5;
                if (npc.alpha < 0)
                    npc.alpha = 0;

                npc.ai[1] += 1f;
                if (npc.ai[1] >= 420f)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
                else if (npc.ai[1] > 360f)
                {
                    npc.velocity *= 0.95f;

                    if (npc.localAI[2] != 13f)
                        SoundEngine.PlaySound(SoundID.Zombie105, npc.position);

                    npc.localAI[2] = 13f;
                }
                else if (npc.ai[1] > 300f)
                {
                    npc.velocity = -Vector2.UnitY;
                    npc.localAI[2] = 10f;
                }
                else if (npc.ai[1] > 120f)
                    npc.localAI[2] = 1f;
                else
                    npc.localAI[2] = 0f;

                dontTakeDamage = true;
            }

            // Phase switch
            if (npc.ai[0] == 0f)
            {
                if (npc.ai[1] == 0f)
                    npc.TargetClosest(false);

                npc.localAI[2] = 10f;

                int num14 = Math.Sign(player.Center.X - npc.Center.X);
                if (num14 != 0)
                    npc.direction = npc.spriteDirection = num14;

                npc.ai[1] += 1f;
                if (npc.ai[1] >= idleTime & isCultist)
                {
                    // Phase switch
                    int phase = 0;
                    switch ((int)npc.ai[3])
                    {
                        // Move to target
                        case 0:
                        case 2:
                        case 4:
                        case 6:
                        case 8:
                        case 10:
                        case 12:
                        case 14:
                        case 16:
                        case 18:
                        case 20:
                            phase = 0;
                            break;

                        // Fireball
                        case 1:
                        case 15:
                            phase = 1;
                            break;

                        // Ancient Light
                        case 3:
                        case 11:
                            phase = 5;
                            break;

                        // Lightning Orb
                        case 5:
                        case 13:
                            phase = 3;
                            break;

                        // Ice Mist
                        case 7:
                        case 17:
                            phase = 2;
                            break;

                        // Ancient Doom
                        case 9:
                        case 19:
                            // Pick a different random phase if too many Ancient Dooms are active
                            int[] attackPhases = new int[4] { 1, 2, 3, 5 };
                            phase = NPC.CountNPCS(NPCID.AncientDoom) < ancientDoomLimit ? 6 : attackPhases[Main.rand.Next(attackPhases.Length)];
                            break;

                        // Dragon Summon
                        case 21:
                            phase = 4;
                            npc.ai[3] = -1f;
                            break;

                        default:
                            npc.ai[3] = -1f;
                            break;
                    }

                    // Set AI phase
                    switch (phase)
                    {
                        // Movement
                        case 0:
                            // Set a location to move to
                            float num16 = (float)Math.Ceiling((player.Center + new Vector2(0f, -100f) - npc.Center).Length() / 50f);
                            if (num16 == 0f)
                                num16 = 1f;

                            // Add self and clones to list
                            List<int> list2 = new List<int>();
                            int num17 = 0;
                            list2.Add(npc.whoAmI);
                            for (int k = 0; k < Main.maxNPCs; k++)
                            {
                                if (Main.npc[k].active && Main.npc[k].type == NPCID.CultistBossClone && Main.npc[k].ai[3] == npc.whoAmI)
                                    list2.Add(k);
                            }

                            // Move self and clones to location
                            bool flag5 = list2.Count % 2 == 0;
                            foreach (int current2 in list2)
                            {
                                NPC nPC2 = Main.npc[current2];
                                Vector2 center2 = nPC2.Center;
                                float num18 = (num17 + flag5.ToInt() + 1) / 2 * MathHelper.TwoPi * 0.4f / list2.Count;
                                if (num17 % 2 == 1)
                                {
                                    num18 *= -1f;
                                }
                                if (list2.Count == 1)
                                {
                                    num18 = 0f;
                                }
                                Vector2 value = new Vector2(0f, -1f).RotatedBy(num18) * new Vector2(150f, 200f);
                                Vector2 value2 = player.Center + value - center2;
                                nPC2.ai[0] = 1f;
                                nPC2.ai[1] = num16;
                                nPC2.velocity = value2 / num16 * 2f;
                                if (npc.whoAmI >= nPC2.whoAmI)
                                {
                                    nPC2.position -= nPC2.velocity;
                                }
                                nPC2.netUpdate = true;
                                num17++;
                            }
                            break;

                        // Fireball
                        case 1:
                            npc.ai[0] = 3f;
                            npc.ai[1] = 0f;
                            break;

                        // Ice Mist
                        case 2:
                            npc.ai[0] = 2f;
                            npc.ai[1] = 0f;
                            break;

                        // Lightning Orb
                        case 3:
                            npc.ai[0] = 4f;
                            npc.ai[1] = 0f;
                            break;

                        // Dragon Summon
                        case 4:
                            npc.ai[0] = 5f;
                            npc.ai[1] = 0f;
                            break;

                        // Ancient Light
                        case 5:
                            npc.ai[0] = 7f;
                            npc.ai[1] = 0f;
                            break;

                        // Ancient Doom
                        case 6:
                            npc.ai[0] = 8f;
                            npc.ai[1] = 0f;
                            break;

                        default:
                            break;
                    }

                    npc.netUpdate = true;
                }
            }

            // Movement, then switch to a different attack
            else if (npc.ai[0] == 1f)
            {
                dontTakeDamage = true;

                npc.localAI[2] = 10f;

                if (npc.ai[1] % 2f != 0f && npc.ai[1] != 1f)
                    npc.position -= npc.velocity;

                npc.ai[1] -= 1f;
                if (npc.ai[1] <= 0f)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[3] += 1f;
                    npc.velocity = Vector2.Zero;
                    npc.netUpdate = true;
                }
            }

            // Ice Mist
            else if (npc.ai[0] == 2f)
            {
                npc.localAI[2] = 11f;

                Vector2 vec = Vector2.Normalize(player.Center - npc.Center);
                if (vec.HasNaNs())
                    vec = new Vector2(npc.direction, 0f);

                if ((npc.ai[1] >= 4f & isCultist) && (int)(npc.ai[1] - 4f) % iceMistFireRate == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        List<int> list3 = new List<int>();
                        for (int l = 0; l < Main.maxNPCs; l++)
                        {
                            if (Main.npc[l].active && Main.npc[l].type == NPCID.CultistBossClone && Main.npc[l].ai[3] == npc.whoAmI)
                                list3.Add(l);
                        }

                        foreach (int current3 in list3)
                        {
                            NPC nPC3 = Main.npc[current3];
                            Vector2 center3 = nPC3.Center;

                            int num19 = Math.Sign(player.Center.X - center3.X);
                            if (num19 != 0)
                                nPC3.direction = nPC3.spriteDirection = num19;

                            vec = Vector2.Normalize(player.Center - center3);
                            if (vec.HasNaNs())
                                vec = new Vector2(npc.direction, 0f);

                            Vector2 vector = center3 + new Vector2(npc.direction * 30, 12f);
                            Vector2 vector2 = vec * (fireballSpeed + (float)Main.rand.NextDouble() * 2f);
                            vector2 = vector2.RotatedByRandom(0.52359879016876221);
                            Projectile.NewProjectile(npc.GetSource_FromAI(), vector, vector2, ProjectileID.CultistBossFireBallClone, fireballDamage, 0f, Main.myPlayer);
                        }
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        vec = Vector2.Normalize(player.Center - npc.Center);
                        if (vec.HasNaNs())
                            vec = new Vector2(npc.direction, 0f);

                        Vector2 vector3 = npc.Center + new Vector2(npc.direction * 30, 12f);
                        Vector2 vector4 = vec * iceMistSpeed;
                        Projectile.NewProjectile(npc.GetSource_FromAI(), vector3, vector4, ProjectileID.CultistBossIceMist, iceMistDamage, 0f, Main.myPlayer, 0f, 1f);
                    }
                }

                npc.ai[1] += 1f;
                if (npc.ai[1] >= (4 + iceMistFireRate * iceMistAmt))
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[3] += 1f;
                    npc.velocity = Vector2.Zero;
                    npc.netUpdate = true;
                }
            }

            // Fireballs
            else if (npc.ai[0] == 3f)
            {
                npc.localAI[2] = 11f;

                Vector2 vec2 = Vector2.Normalize(player.Center - npc.Center);
                if (vec2.HasNaNs())
                    vec2 = new Vector2(npc.direction, 0f);

                if ((npc.ai[1] >= 4f & isCultist) && (int)(npc.ai[1] - 4f) % fireballFireRate == 0)
                {
                    if ((int)(npc.ai[1] - 4f) / fireballFireRate == 2)
                    {
                        List<int> list4 = new List<int>();
                        for (int num20 = 0; num20 < Main.maxNPCs; num20++)
                        {
                            if (Main.npc[num20].active && Main.npc[num20].type == NPCID.CultistBossClone && Main.npc[num20].ai[3] == npc.whoAmI)
                                list4.Add(num20);
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            foreach (int current4 in list4)
                            {
                                NPC nPC4 = Main.npc[current4];
                                Vector2 center4 = nPC4.Center;

                                int num21 = Math.Sign(player.Center.X - center4.X);
                                if (num21 != 0)
                                    nPC4.direction = nPC4.spriteDirection = num21;

                                vec2 = Vector2.Normalize(player.Center - center4);
                                if (vec2.HasNaNs())
                                    vec2 = new Vector2(npc.direction, 0f);

                                Vector2 vector5 = center4 + new Vector2(npc.direction * 30, 12f);
                                Vector2 vector6 = vec2 * (fireballSpeed + (float)Main.rand.NextDouble() * 2f);
                                vector6 = vector6.RotatedByRandom(0.52359879016876221);
                                Projectile.NewProjectile(npc.GetSource_FromAI(), vector5, vector6, ProjectileID.CultistBossFireBallClone, fireballDamage, 0f, Main.myPlayer);
                            }
                        }
                    }

                    int num23 = Math.Sign(player.Center.X - npc.Center.X);
                    if (num23 != 0)
                        npc.direction = npc.spriteDirection = num23;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        vec2 = Vector2.Normalize(player.Center - npc.Center);
                        if (vec2.HasNaNs())
                            vec2 = new Vector2(npc.direction, 0f);

                        Vector2 vector7 = npc.Center + new Vector2(npc.direction * 30, 12f);
                        Vector2 vector8 = vec2 * (fireballSpeed + (float)Main.rand.NextDouble() * 4f);
                        vector8 = vector8.RotatedByRandom(0.52359879016876221);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), vector7, vector8, ProjectileID.CultistBossFireBall, fireballDamage, 0f, Main.myPlayer);
                    }
                }

                npc.ai[1] += 1f;
                if (npc.ai[1] >= (4 + fireballFireRate * 4))
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[3] += 1f;
                    npc.velocity = Vector2.Zero;
                    npc.netUpdate = true;
                }
            }

            // Lightning Orb
            else if (npc.ai[0] == 4f)
            {
                if (isCultist)
                    npc.localAI[2] = 12f;
                else
                    npc.localAI[2] = 11f;

                if ((npc.ai[1] == 20f & isCultist) && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    List<int> list5 = new List<int>();
                    for (int num25 = 0; num25 < Main.maxNPCs; num25++)
                    {
                        if (Main.npc[num25].active && Main.npc[num25].type == NPCID.CultistBossClone && Main.npc[num25].ai[3] == npc.whoAmI)
                            list5.Add(num25);
                    }

                    foreach (int current5 in list5)
                    {
                        NPC nPC5 = Main.npc[current5];
                        Vector2 center5 = nPC5.Center;

                        int num26 = Math.Sign(player.Center.X - center5.X);
                        if (num26 != 0)
                            nPC5.direction = nPC5.spriteDirection = num26;

                        Vector2 vec3 = Vector2.Normalize(player.Center - center5);
                        if (vec3.HasNaNs())
                            vec3 = new Vector2(npc.direction, 0f);

                        Vector2 vector9 = center5 + new Vector2(npc.direction * 30, 12f);
                        Vector2 vector10 = vec3 * (fireballSpeed + (float)Main.rand.NextDouble() * 2f);
                        vector10 = vector10.RotatedByRandom(0.52359879016876221);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), vector9.X, vector9.Y, vector10.X, vector10.Y, ProjectileID.CultistBossFireBallClone, fireballDamage, 0f, Main.myPlayer);
                    }

                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y - 100f, 0f, 0f, ProjectileID.CultistBossLightningOrb, lightningDamage, 0f, Main.myPlayer);
                }

                npc.ai[1] += 1f;
                if (npc.ai[1] >= (20 + lightningOrbPhaseTime))
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[3] += 1f;
                    npc.velocity = Vector2.Zero;
                    npc.netUpdate = true;
                }
            }

            // Spawn Clones, and Dragon or Cthulhu head
            else if (npc.ai[0] == 5f)
            {
                npc.localAI[2] = 10f;

                if (Vector2.Normalize(player.Center - npc.Center).HasNaNs())
                    new Vector2(npc.direction, 0f);

                if (npc.ai[1] >= 0f && npc.ai[1] < 30f)
                {
                    dontTakeDamage = true;
                    float num28 = (npc.ai[1] - 0f) / 30f;
                    npc.alpha = (int)(num28 * 255f);
                }
                else if (npc.ai[1] >= 30f && npc.ai[1] < 90f)
                {
                    if ((npc.ai[1] == 30f && Main.netMode != NetmodeID.MultiplayerClient) & isCultist)
                    {
                        npc.localAI[1] += 1f;

                        Vector2 spinningpoint = new Vector2(180f, 0f);

                        List<int> list6 = new List<int>();
                        for (int num29 = 0; num29 < Main.maxNPCs; num29++)
                        {
                            if (Main.npc[num29].active && Main.npc[num29].type == NPCID.CultistBossClone && Main.npc[num29].ai[3] == npc.whoAmI)
                                list6.Add(num29);
                        }

                        int num30 = 6 - list6.Count;
                        if (num30 > 2)
                            num30 = 2;

                        int num31 = list6.Count + num30 + 1;
                        float[] array = new float[num31];
                        for (int num32 = 0; num32 < array.Length; num32++)
                            array[num32] = Vector2.Distance(npc.Center + spinningpoint.RotatedBy(num32 * MathHelper.TwoPi / num31 - MathHelper.PiOver2), player.Center);

                        int num33 = 0;
                        for (int num34 = 1; num34 < array.Length; num34++)
                        {
                            if (array[num33] > array[num34])
                                num33 = num34;
                        }

                        if (num33 < num31 / 2)
                            num33 += num31 / 2;
                        else
                            num33 -= num31 / 2;

                        int num35 = num30;
                        for (int num36 = 0; num36 < array.Length; num36++)
                        {
                            if (num33 != num36)
                            {
                                Vector2 vector11 = npc.Center + spinningpoint.RotatedBy(num36 * MathHelper.TwoPi / num31 - MathHelper.PiOver2);
                                if (num35-- > 0)
                                {
                                    int num37 = NPC.NewNPC(npc.GetSource_FromAI(), (int)vector11.X, (int)vector11.Y + npc.height / 2, NPCID.CultistBossClone, npc.whoAmI);
                                    Main.npc[num37].ai[3] = npc.whoAmI;
                                    Main.npc[num37].netUpdate = true;
                                    Main.npc[num37].localAI[1] = npc.localAI[1];
                                }
                                else
                                {
                                    int num38 = list6[-num35 - 1];
                                    Main.npc[num38].Center = vector11;
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num38, 0f, 0f, 0f, 0, 0, 0);
                                }
                            }
                        }

                        npc.ai[2] = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ProjectileID.CultistRitual, 0, 0f, Main.myPlayer, 0f, npc.whoAmI);
                        npc.Center += spinningpoint.RotatedBy(num33 * MathHelper.TwoPi / num31 - MathHelper.PiOver2);
                        npc.netUpdate = true;
                        list6.Clear();
                    }

                    dontTakeDamage = true;
                    npc.alpha = 255;

                    if (isCultist)
                    {
                        Vector2 vector12 = Main.projectile[(int)npc.ai[2]].Center;
                        vector12 -= npc.Center;
                        if (vector12 == Vector2.Zero)
                            vector12 = -Vector2.UnitY;

                        vector12.Normalize();

                        if (Math.Abs(vector12.Y) < 0.77f)
                            npc.localAI[2] = 11f;
                        else if (vector12.Y < 0f)
                            npc.localAI[2] = 12f;
                        else
                            npc.localAI[2] = 10f;

                        int num39 = Math.Sign(vector12.X);
                        if (num39 != 0)
                            npc.direction = npc.spriteDirection = num39;
                    }
                    else
                    {
                        Vector2 vector13 = Main.projectile[(int)Main.npc[(int)npc.ai[3]].ai[2]].Center;
                        vector13 -= npc.Center;
                        if (vector13 == Vector2.Zero)
                            vector13 = -Vector2.UnitY;

                        vector13.Normalize();

                        if (Math.Abs(vector13.Y) < 0.77f)
                            npc.localAI[2] = 11f;
                        else if (vector13.Y < 0f)
                            npc.localAI[2] = 12f;
                        else
                            npc.localAI[2] = 10f;

                        int num40 = Math.Sign(vector13.X);
                        if (num40 != 0)
                            npc.direction = npc.spriteDirection = num40;
                    }
                }
                else if (npc.ai[1] >= 90f && npc.ai[1] < 120f)
                {
                    dontTakeDamage = true;
                    float num41 = (npc.ai[1] - 90f) / 30f;
                    npc.alpha = 255 - (int)(num41 * 255f);
                }
                else if (npc.ai[1] >= 120f && npc.ai[1] < timeToFinishRitual)
                {
                    npc.alpha = 0;

                    if (isCultist)
                    {
                        Vector2 vector14 = Main.projectile[(int)npc.ai[2]].Center;
                        vector14 -= npc.Center;
                        if (vector14 == Vector2.Zero)
                            vector14 = -Vector2.UnitY;

                        vector14.Normalize();

                        if (Math.Abs(vector14.Y) < 0.77f)
                            npc.localAI[2] = 11f;
                        else if (vector14.Y < 0f)
                            npc.localAI[2] = 12f;
                        else
                            npc.localAI[2] = 10f;

                        int num42 = Math.Sign(vector14.X);
                        if (num42 != 0)
                            npc.direction = npc.spriteDirection = num42;
                    }
                    else
                    {
                        Vector2 vector15 = Main.projectile[(int)Main.npc[(int)npc.ai[3]].ai[2]].Center;
                        vector15 -= npc.Center;
                        if (vector15 == Vector2.Zero)
                            vector15 = -Vector2.UnitY;

                        vector15.Normalize();

                        if (Math.Abs(vector15.Y) < 0.77f)
                            npc.localAI[2] = 11f;
                        else if (vector15.Y < 0f)
                            npc.localAI[2] = 12f;
                        else
                            npc.localAI[2] = 10f;

                        int num43 = Math.Sign(vector15.X);
                        if (num43 != 0)
                            npc.direction = npc.spriteDirection = num43;
                    }
                }

                npc.ai[1] += 1f;
                if (npc.ai[1] >= timeToFinishRitual)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[3] += 1f;
                    npc.velocity = Vector2.Zero;
                    npc.netUpdate = true;
                }
            }

            // Pause
            else if (npc.ai[0] == 6f)
            {
                npc.localAI[2] = 13f;

                npc.ai[1] += 1f;
                if (npc.ai[1] >= (idleTime * 3))
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[3] += 1f;
                    npc.velocity = Vector2.Zero;
                    npc.netUpdate = true;
                }
            }

            // Ancient Light
            else if (npc.ai[0] == 7f)
            {
                npc.localAI[2] = 11f;

                Vector2 vec4 = Vector2.Normalize(player.Center - npc.Center);
                if (vec4.HasNaNs())
                    vec4 = new Vector2(npc.direction, 0f);

                if ((npc.ai[1] >= 4f & isCultist) && (int)(npc.ai[1] - 4f) % ancientLightSpawnRate == 0)
                {
                    if ((int)(npc.ai[1] - 4f) / ancientLightSpawnRate == 2)
                    {
                        List<int> list7 = new List<int>();
                        for (int num44 = 0; num44 < Main.maxNPCs; num44++)
                        {
                            if (Main.npc[num44].active && Main.npc[num44].type == NPCID.CultistBossClone && Main.npc[num44].ai[3] == npc.whoAmI)
                                list7.Add(num44);
                        }

                        foreach (int current6 in list7)
                        {
                            NPC nPC6 = Main.npc[current6];
                            Vector2 center6 = nPC6.Center;

                            int num45 = Math.Sign(player.Center.X - center6.X);
                            if (num45 != 0)
                                nPC6.direction = nPC6.spriteDirection = num45;
                        }
                    }

                    int num47 = Math.Sign(player.Center.X - npc.Center.X);
                    if (num47 != 0)
                        npc.direction = npc.spriteDirection = num47;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        vec4 = Vector2.Normalize(player.Center - npc.Center);
                        if (vec4.HasNaNs())
                            vec4 = new Vector2(npc.direction, 0f);

                        Vector2 vector18 = npc.Center + new Vector2(npc.direction * 30, 12f);
                        float scaleFactor = death ? 6f : 4f;

                        float num48 = MathHelper.ToRadians(15f);
                        int num49 = 0;
                        float totalAncientLights = 5f;
                        while (num49 < totalAncientLights)
                        {
                            Vector2 vector19 = vec4 * scaleFactor;
                            vector19 = vector19.RotatedBy(num48 * num49 - (MathHelper.Pi / totalAncientLights * 2f - num48) / 2f);
                            float ai = (Main.rand.NextFloat() - 0.5f) * 0.3f * MathHelper.TwoPi / 60f;
                            int num50 = NPC.NewNPC(npc.GetSource_FromAI(), (int)vector18.X, (int)vector18.Y + 7, NPCID.AncientLight, 0, 0f, ai, vector19.X, vector19.Y, 255);
                            Main.npc[num50].velocity = vector19;
                            num49++;
                        }
                    }
                }

                npc.ai[1] += 1f;
                if (npc.ai[1] >= (4 + ancientLightSpawnRate * ancientLightAmt))
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[3] += 1f;
                    npc.velocity = Vector2.Zero;
                    npc.netUpdate = true;
                }
            }

            // Ancient Doom
            else if (npc.ai[0] == 8f)
            {
                npc.localAI[2] = 13f;

                if ((npc.ai[1] >= 4f & isCultist) && (int)(npc.ai[1] - 4f) % 20f == 0f)
                {
                    List<int> list8 = new List<int>();
                    for (int num51 = 0; num51 < Main.maxNPCs; num51++)
                    {
                        if (Main.npc[num51].active && Main.npc[num51].type == NPCID.CultistBossClone && Main.npc[num51].ai[3] == npc.whoAmI)
                            list8.Add(num51);
                    }

                    int num52 = list8.Count + 1;
                    if (num52 > 2)
                        num52 = 2;

                    int num53 = Math.Sign(player.Center.X - npc.Center.X);
                    if (num53 != 0)
                        npc.direction = npc.spriteDirection = num53;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (phase4)
                        {
                            // Spawn circle of Ancient Dooms around target
                            for (int i = 0; i < ancientDoomLimit; i++)
                            {
                                float ai2 = i * 120;
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)(player.Center.X + (float)(Math.Sin(i * 120) * 550)), (int)(player.Center.Y + (float)(Math.Cos(i * 120) * 550)),
                                    NPCID.AncientDoom, 0, npc.whoAmI, 0f, ai2, 0f, Main.maxPlayers);
                            }
                        }
                        else
                        {
                            // Spawn Ancient Dooms randomly around the target
                            for (int num54 = 0; num54 < num52; num54++)
                            {
                                Point point = npc.Center.ToTileCoordinates();
                                Point point2 = Main.player[npc.target].Center.ToTileCoordinates();
                                Vector2 vector20 = Main.player[npc.target].Center - npc.Center;

                                int num55 = 20;
                                int num56 = 3;
                                int num57 = 7;
                                int num58 = 2;
                                int num59 = 0;
                                bool flag6 = vector20.Length() > 2800f;
                                while (!flag6 && num59 < 100)
                                {
                                    num59++;
                                    int num60 = Main.rand.Next(point2.X - num55, point2.X + num55 + 1);
                                    int num61 = Main.rand.Next(point2.Y - num55, point2.Y + num55 + 1);
                                    if ((num61 < point2.Y - num57 || num61 > point2.Y + num57 || num60 < point2.X - num57 || num60 > point2.X + num57) && (num61 < point.Y - num56 || num61 > point.Y + num56 || num60 < point.X - num56 || num60 > point.X + num56) && !Main.tile[num60, num61].HasUnactuatedTile)
                                    {
                                        bool flag7 = true;
                                        if (flag7 && Collision.SolidTiles(num60 - num58, num60 + num58, num61 - num58, num61 + num58))
                                            flag7 = false;

                                        if (flag7)
                                        {
                                            NPC.NewNPC(npc.GetSource_FromAI(), num60 * 16 + 8, num61 * 16 + 8, NPCID.AncientDoom, 0, npc.whoAmI);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                npc.ai[1] += 1f;
                if (npc.ai[1] >= 64f)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[3] += 1f;
                    npc.velocity = Vector2.Zero;
                    npc.netUpdate = true;
                }
            }

            // Set Clones to Cultist phase
            if (!isCultist)
                npc.ai[3] = clonePhase;

            // Take damage or not
            npc.dontTakeDamage = dontTakeDamage;
            npc.chaseable = npc.ai[0] != -1f && npc.ai[0] != 5f;

            return false;
        }

        public static bool BuffedAncientLightAI(NPC npc, Mod mod)
        {
            npc.dontTakeDamage = true;

            // Slow and kill code
            if (npc.ai[0] == -1f)
            {
                // Slow down over time
                if (npc.velocity.Length() >= 0.2f)
                {
                    npc.velocity *= 0.96f;
                }
                else
                {
                    npc.velocity = Vector2.Zero;
                    npc.position = npc.oldPosition;

                    // Kill after 2 seconds
                    npc.ai[1] += 1f;
                    if (npc.ai[1] >= 240f)
                    {
                        npc.HitEffect(0, 9999.0);
                        npc.active = false;
                    }
                }

                return false;
            }

            npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;

            // Set velocity and emit dust when spawned
            if (npc.localAI[0] == 0f)
            {
                npc.localAI[0] = 1f;
                npc.velocity.X = npc.ai[2];
                npc.velocity.Y = npc.ai[3];
                for (int num1492 = 0; num1492 < 13; num1492++)
                {
                    int num1493 = Dust.NewDust(npc.position, npc.width, npc.height, 261, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default, 2.5f);
                    Main.dust[num1493].noGravity = true;
                    Main.dust[num1493].fadeIn = 1f;
                    Dust dust = Main.dust[num1493];
                    dust.velocity *= 4f;
                    Main.dust[num1493].noLight = true;
                }
            }

            // Spawn dust
            for (int num1494 = 0; num1494 < 2; num1494++)
            {
                if (Main.rand.Next(10 - (int)Math.Min(7f, npc.velocity.Length())) < 1)
                {
                    int num1495 = Dust.NewDust(npc.position, npc.width, npc.height, 261, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default, 2.5f);
                    Main.dust[num1495].noGravity = true;
                    Dust dust = Main.dust[num1495];
                    dust.velocity *= 0.2f;
                    Main.dust[num1495].fadeIn = 0.4f;
                    if (Main.rand.NextBool(6))
                    {
                        dust = Main.dust[num1495];
                        dust.velocity *= 5f;
                        Main.dust[num1495].noLight = true;
                    }
                    else
                        Main.dust[num1495].velocity = npc.DirectionFrom(Main.dust[num1495].position) * Main.dust[num1495].velocity.Length();
                }
            }

            if (npc.ai[0] >= 0f)
            {
                // Triple damage if the Primordial Wyrm is alive
                if (npc.ai[0] == 0f)
                {
                    if (CalamityGlobalNPC.adultEidolonWyrmHead != -1)
                    {
                        if (Main.npc[CalamityGlobalNPC.adultEidolonWyrmHead].active)
                            npc.damage *= 3;
                    }
                }

                npc.ai[0] += 1f;

                float duration = 120f;

                // Increase velocity for 1 second
                if (npc.ai[0] < duration - 60f)
                {
                    if (npc.velocity.Length() < 20f)
                        npc.velocity *= 1.03f;
                }

                // Intersect velocity paths with other Ancient Lights for 1 second
                if (npc.ai[0] >= duration - 60f)
                    npc.velocity = npc.velocity.RotatedBy(npc.ai[1]);

                // Engage slow code
                if (npc.ai[0] >= duration)
                    npc.ai[0] = -1f;
            }

            return false;
        }

        public static bool BuffedAncientDoomAI(NPC npc, Mod mod)
        {
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            npc.damage = npc.defDamage = 0;
            float duration = 420f;
            float spawnAnimTime = 120f;
            int rateOfChange = 1;
            float splitProjVelocity = death ? 4.8f : 3.2f;

            // Percent life remaining for Cultist or Eidolon Wyrm
            float lifeRatio = Main.npc[(int)npc.ai[0]].life / (float)Main.npc[(int)npc.ai[0]].lifeMax;

            bool phase2 = lifeRatio < 0.7f;
            bool phase3 = lifeRatio < (Main.npc[(int)npc.ai[0]].type == ModContent.NPCType<PrimordialWyrmHead>() ? 0.6f : 0.55f);
            bool phase4 = lifeRatio < 0.4f;

            bool kill = npc.ai[1] < 0f || !Main.npc[(int)npc.ai[0]].active;
            int target = Main.maxPlayers;
            if (Main.npc[(int)npc.ai[0]].type == NPCID.CultistBoss || Main.npc[(int)npc.ai[0]].type == ModContent.NPCType<PrimordialWyrmHead>())
            {
                if (target == Main.maxPlayers)
                    target = Main.npc[(int)npc.ai[0]].target;

                if (phase2 || death)
                    rateOfChange = 2;

                if (phase4 || death)
                    rateOfChange = 3;

                if (Main.npc[(int)npc.ai[0]].type == ModContent.NPCType<PrimordialWyrmHead>())
                    npc.dontTakeDamage = true;
            }
            else
                kill = true;

            npc.ai[1] += rateOfChange;
            float growthRate = npc.ai[1] / spawnAnimTime;
            growthRate = MathHelper.Clamp(growthRate, 0f, 1f);
            npc.position = npc.Center;
            npc.scale = MathHelper.Lerp(0f, 1f, growthRate);
            npc.Center = npc.position;
            npc.alpha = (int)(255f - growthRate * 255f);

            if (phase3)
            {
                if (npc.ai[3] == 0f)
                    npc.ai[3] = npc.ai[2];

                double deg = npc.ai[3];
                double rad = deg * (Math.PI / 180);
                double dist = 550;
                if (Main.npc[(int)npc.ai[0]].type == ModContent.NPCType<PrimordialWyrmHead>())
                {
                    float aiGateValue = Main.npc[(int)npc.ai[0]].Calamity().newAI[2] - 30f;
                    int ancientDoomScale = (int)(aiGateValue / 120f);
                    dist += ancientDoomScale * 45;
                }
                npc.position.X = Main.player[target].Center.X - (int)(Math.Cos(rad) * dist) - npc.width / 2;
                npc.position.Y = Main.player[target].Center.Y - (int)(Math.Sin(rad) * dist) - npc.height / 2;
                float spinVelocity = 8f * (1f - (npc.ai[1] / duration));
                npc.ai[3] += spinVelocity;
            }

            if (Main.rand.NextBool(6))
            {
                Vector2 spinningpoint4 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                Dust dust17 = Main.dust[Dust.NewDust(npc.Center - spinningpoint4 * 20f, 0, 0, 27, 0f, 0f, 0, default, 1f)];
                dust17.noGravity = true;
                dust17.position = npc.Center - spinningpoint4 * Main.rand.Next(10, 21) * npc.scale;
                dust17.velocity = spinningpoint4.RotatedBy(MathHelper.PiOver2) * 4f;
                dust17.scale = 0.5f + Main.rand.NextFloat();
                dust17.fadeIn = 0.5f;
            }
            if (Main.rand.NextBool(6))
            {
                Vector2 spinningpoint5 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                Dust dust18 = Main.dust[Dust.NewDust(npc.Center - spinningpoint5 * 30f, 0, 0, 240, 0f, 0f, 0, default, 1f)];
                dust18.noGravity = true;
                dust18.position = npc.Center - spinningpoint5 * 20f * npc.scale;
                dust18.velocity = spinningpoint5.RotatedBy(-MathHelper.PiOver2) * 2f;
                dust18.scale = 0.5f + Main.rand.NextFloat();
                dust18.fadeIn = 0.5f;
            }
            if (Main.rand.NextBool(6))
            {
                Vector2 vector254 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                Dust dust19 = Main.dust[Dust.NewDust(npc.Center - vector254 * 30f, 0, 0, 240, 0f, 0f, 0, default, 1f)];
                dust19.position = npc.Center - vector254 * 20f * npc.scale;
                dust19.velocity = Vector2.Zero;
                dust19.scale = 0.5f + Main.rand.NextFloat();
                dust19.fadeIn = 0.5f;
                dust19.noLight = true;
            }

            npc.localAI[0] += 0.05235988f;

            npc.localAI[1] = 0.25f + Vector2.UnitY.RotatedBy(npc.ai[1] * MathHelper.TwoPi / 60f).Y * 0.25f;

            if (npc.ai[1] >= duration)
            {
                int type = ProjectileID.AncientDoomProjectile;
                int damage = npc.GetProjectileDamage(type);

                // Triple damage if the Primordial Wyrm is alive
                if (Main.npc[(int)npc.ai[0]].type == ModContent.NPCType<PrimordialWyrmHead>())
                    damage *= 3;

                kill = true;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int totalProjectiles = CalamityWorld.LegendaryMode ? 9 : (Main.npc[(int)npc.ai[0]].type == NPCID.CultistBoss && !phase3) ? 8 : 4;
                    float radians = MathHelper.TwoPi / totalProjectiles;
                    Vector2 spinningPoint = new Vector2(0f, -splitProjVelocity);
                    for (int k = 0; k < totalProjectiles; k++)
                    {
                        Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, vector255, type, damage, 0f, Main.myPlayer);
                    }
                }
            }

            if (kill)
            {
                npc.HitEffect(0, 9999.0);
                npc.active = false;
            }

            return false;
        }
    }
}
