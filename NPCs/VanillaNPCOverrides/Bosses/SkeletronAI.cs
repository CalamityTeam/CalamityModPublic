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
    public static class SkeletronAI
    {
        // Master Mode changes
        // 1 - Arms are immune to damage and Skeletron no longer has increased defense while the arms are alive,
        // 2 - Moves far more aggressively
        public static bool BuffedSkeletronAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive && malice;

            Vector2 vectorCenter = npc.Center;

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Phases
            bool respawnHands = lifeRatio < 0.33f;
            bool phase2 = respawnHands || death;

            // Set defense
            npc.defense = npc.defDefense;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            // Spawn hands
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.ai[0] == 0f)
                {
                    npc.ai[0] = 1f;
                    SpawnHands();
                    npc.netUpdate = true;
                }

                // Respawn hands
                if (respawnHands && calamityGlobalNPC.newAI[0] == 0f && Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 160f)
                {
                    calamityGlobalNPC.newAI[0] = 1f;
                    SoundEngine.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0, 1f, -0.25f);
                    SpawnHands();

                    npc.netUpdate = true;
                    npc.SyncExtraAI();
                }

                void SpawnHands()
                {
                    int num155 = NPC.NewNPC((int)(npc.position.X + (npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.SkeletronHand, npc.whoAmI);
                    Main.npc[num155].ai[0] = -1f;
                    Main.npc[num155].ai[1] = npc.whoAmI;
                    Main.npc[num155].target = npc.target;
                    Main.npc[num155].netUpdate = true;

                    num155 = NPC.NewNPC((int)(npc.position.X + (npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.SkeletronHand, npc.whoAmI);
                    Main.npc[num155].ai[0] = 1f;
                    Main.npc[num155].ai[1] = npc.whoAmI;
                    Main.npc[num155].ai[3] = 150f;
                    Main.npc[num155].target = npc.target;
                    Main.npc[num155].netUpdate = true;

                    // Spawn two additional hands with different attack timings
                    if (death)
                    {
                        num155 = NPC.NewNPC((int)(npc.position.X + (npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.SkeletronHand, npc.whoAmI);
                        Main.npc[num155].ai[0] = -1f;
                        Main.npc[num155].Calamity().newAI[0] = -1f;
                        Main.npc[num155].ai[1] = npc.whoAmI;
                        Main.npc[num155].ai[3] = respawnHands ? -75f : 0f;
                        Main.npc[num155].target = npc.target;
                        Main.npc[num155].netUpdate = true;

                        num155 = NPC.NewNPC((int)(npc.position.X + (npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.SkeletronHand, npc.whoAmI);
                        Main.npc[num155].ai[0] = 1f;
                        Main.npc[num155].Calamity().newAI[0] = -1f;
                        Main.npc[num155].ai[1] = npc.whoAmI;
                        Main.npc[num155].ai[3] = respawnHands ? 75f : 150f;
                        Main.npc[num155].target = npc.target;
                        Main.npc[num155].netUpdate = true;
                    }
                }
            }

            // Distance from target
            float distance = Vector2.Distance(Main.player[npc.target].Center, npc.Center);

            // Despawn
            if (Main.player[npc.target].dead || distance > (BossRushEvent.BossRushActive ? 6000f : 4000f))
            {
                npc.TargetClosest();
                if (Main.player[npc.target].dead || distance > (BossRushEvent.BossRushActive ? 6000f : 4000f))
                    npc.ai[1] = 3f;
            }

            // Daytime enrage
            if (Main.dayTime && !BossRushEvent.BossRushActive && npc.ai[1] != 3f && npc.ai[1] != 2f)
            {
                npc.ai[1] = 2f;
                SoundEngine.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
            }

            // Hand immunity
            int num156 = 0;
            for (int num157 = 0; num157 < Main.maxNPCs; num157++)
            {
                if (Main.npc[num157].active && Main.npc[num157].type == NPCID.SkeletronHand)
                    num156++;
            }
            bool handsDead = num156 == 0;
            npc.chaseable = handsDead;
            calamityGlobalNPC.DR = num156 > 0 ? 0.9999f : 0.05f;
            calamityGlobalNPC.unbreakableDR = num156 > 0;
            calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = num156 > 0;

            // Teleport
            if (handsDead || phase2)
            {
                // Post-teleport
                if (npc.ai[3] == -60f)
                {
                    npc.ai[3] = 0f;

                    SoundEngine.PlaySound(SoundID.Item66, npc.position);

                    Vector2 vector10 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);

                    // Fire magic bolt after teleport
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float num151 = 2f + (distance * 0.005f);
                        if (num151 > 5f)
                            num151 = 5f;

                        int type = ProjectileID.Shadowflames;
                        int damage = npc.GetProjectileDamage(type);
                        int numProj = death ? 5 : 3;

                        float num743 = Main.player[npc.target].Center.X - vectorCenter.X;
                        float num744 = Main.player[npc.target].Center.Y - vectorCenter.Y;
                        float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);

                        num745 = num151 / num745;
                        num743 *= num745;
                        num744 *= num745;
                        vectorCenter.X += num743 * 3f;
                        vectorCenter.Y += num744 * 3f;

                        float rotation = MathHelper.ToRadians(60);
                        float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
                        double startAngle = Math.Atan2(num743, num744) - rotation / 2;
                        double deltaAngle = rotation / numProj;
                        double offsetAngle;

                        for (int i = 0; i < numProj; i++)
                        {
                            offsetAngle = startAngle + deltaAngle * i;
                            int proj = Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), type, damage, 0f, Main.myPlayer, 0f, 1f);
                            Main.projectile[proj].timeLeft = 600;
                        }
                        npc.netUpdate = true;
                    }

                    // Teleport dust
                    for (int m = 0; m < 30; m++)
                    {
                        int num39 = Dust.NewDust(npc.position, npc.width, npc.height, 27, 0f, 0f, 200, default, 3f);
                        Main.dust[num39].noGravity = true;
                        Main.dust[num39].velocity.X = Main.dust[num39].velocity.X * 2f;
                    }
                }

                // Teleport after a certain time
                // If hands are dead: 7 seconds
                // If hands are not dead: 14 seconds
                // If hands are dead in phase 2: 4.7 seconds
                npc.ai[3] += 1f + (((phase2 && handsDead) || malice) ? 0.5f : 0f) - ((handsDead || malice) ? 0f : 0.5f);

                // Dust to show teleport
                int ai3 = (int)npc.ai[3]; // 0 to 30, and -60
                bool emitDust = false;

                if (ai3 >= 300 && calamityGlobalNPC.newAI[2] == 0f && calamityGlobalNPC.newAI[3] == 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 value53 = npc.Center + new Vector2(npc.direction * 20, 6f);
                        Vector2 vector251 = Main.player[npc.target].Center - value53;
                        Point point12 = npc.Center.ToTileCoordinates();
                        Point point13 = Main.player[npc.target].Center.ToTileCoordinates();
                        int num1453 = 26;
                        int num1454 = 4;
                        int num1455 = 22;
                        int num1457 = 0;

                        bool flag106 = false;
                        if (vector251.Length() > 2000f)
                            flag106 = true;

                        while (!flag106 && num1457 < 100)
                        {
                            num1457++;
                            int num1458 = Main.rand.Next(point13.X - num1453, point13.X + num1453 + 1);
                            int num1459 = Main.rand.Next(point13.Y - num1453, point13.Y + num1453 + 1);
                            if ((num1459 < point13.Y - num1455 || num1459 > point13.Y + num1455 || num1458 < point13.X - num1455 || num1458 > point13.X + num1455) && (num1459 < point12.Y - num1454 || num1459 > point12.Y + num1454 || num1458 < point12.X - num1454 || num1458 > point12.X + num1454) && !Main.tile[num1458, num1459].nactive())
                            {
                                // New location params
                                calamityGlobalNPC.newAI[2] = num1458 * 16 - npc.width / 2;
                                calamityGlobalNPC.newAI[3] = num1459 * 16 - npc.height;
                                npc.SyncExtraAI();
                                break;
                            }
                        }
                    }
                }

                if (calamityGlobalNPC.newAI[2] != 0f && calamityGlobalNPC.newAI[3] != 0f)
                {
                    for (int m = 0; m < 5; m++)
                    {
                        Vector2 position = new Vector2(calamityGlobalNPC.newAI[2], calamityGlobalNPC.newAI[3]);
                        int num39 = Dust.NewDust(position, npc.width, npc.height, 27, 0f, 0f, 200, default, 2f);
                        Main.dust[num39].noGravity = true;
                    }
                }

                if (ai3 >= 390)
                    emitDust = true;
                else if (ai3 >= 330)
                {
                    if (Main.rand.Next(310, ai3 + 1) >= 325)
                        emitDust = true;
                }
                if (emitDust)
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, 27, 0f, 0f, 200, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                }

                // Teleport
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[3] >= 420f)
                {
                    // Teleport dust
                    for (int m = 0; m < 30; m++)
                    {
                        int num39 = Dust.NewDust(npc.position, npc.width, npc.height, 27, 0f, 0f, 200, default, 3f);
                        Main.dust[num39].noGravity = true;
                        Main.dust[num39].velocity.X = Main.dust[num39].velocity.X * 2f;
                    }

                    // New location
                    npc.Center = new Vector2(calamityGlobalNPC.newAI[2], calamityGlobalNPC.newAI[3]);
                    npc.velocity = Vector2.Zero;
                    npc.ai[3] = -60f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    calamityGlobalNPC.newAI[3] = 0f;
                    npc.SyncExtraAI();
                    npc.netUpdate = true;
                }
            }
            else
                npc.ai[3] = 0f;

            // Skull shooting
            if (handsDead && npc.ai[1] == 0f)
            {
                float num158 = BossRushEvent.BossRushActive ? 10f : malice ? 15f : phase2 ? (60f - (death ? 30f * (1f - lifeRatio) : 0f)) : 75f;
                if (Main.netMode != NetmodeID.MultiplayerClient && calamityGlobalNPC.newAI[1] >= num158)
                {
                    calamityGlobalNPC.newAI[1] = 0f;
                    Vector2 vector18 = npc.Center;
                    float num159 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector18.X;
                    float num160 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector18.Y;
                    float num161 = (float)Math.Sqrt(num159 * num159 + num160 * num160);
                    if (Collision.CanHit(vector18, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        float num162 = phase2 ? (5f + (death ? 3f * (1f - lifeRatio) : 0f)) : 4f;
                        float num163 = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - vector18.X + Main.rand.Next(-20, 21);
                        float num164 = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - vector18.Y + Main.rand.Next(-20, 21);
                        float num165 = (float)Math.Sqrt(num163 * num163 + num164 * num164);
                        num165 = num162 / num165;
                        num163 *= num165;
                        num164 *= num165;
                        int spread = malice ? 100 : 50;
                        Vector2 vector19 = new Vector2(num163 + Main.rand.Next(-spread, spread + 1) * 0.01f, num164 + Main.rand.Next(-spread, spread + 1) * 0.01f);
                        vector19.Normalize();
                        vector19 *= num162;
                        vector19 += npc.velocity;
                        num163 = vector19.X;
                        num164 = vector19.Y;
                        int type = ProjectileID.Skull;
                        int damage = npc.GetProjectileDamage(type);
                        vector18 += vector19 * 5f;
                        int num168 = Projectile.NewProjectile(vector18.X, vector18.Y, num163, num164, type, damage, 0f, Main.myPlayer, -1f, 0f);
                        Main.projectile[num168].timeLeft = 300;

                        npc.netUpdate = true;
                    }
                }
            }

            // Float above target
            if (npc.ai[1] == 0f)
            {
                npc.damage = lifeRatio > 0.99f ? 0 : npc.defDamage;

                calamityGlobalNPC.newAI[1] += 1f;
                float phaseChangeRateBoost = 3f * (1f - lifeRatio);
                npc.ai[2] += 1f + phaseChangeRateBoost;
                if (npc.ai[2] >= 600f)
                {
                    npc.ai[2] = 0f;
                    npc.ai[1] = 1f;
                    calamityGlobalNPC.newAI[1] = 0f;

                    npc.TargetClosest();
                    npc.SyncExtraAI();
                    npc.netUpdate = true;
                }

                npc.rotation = npc.velocity.X / 15f;

                float num169 = 0.04f + (death ? 0.04f * (1f - lifeRatio) : 0f);
                float num170 = 3.5f - (death ? 1f - lifeRatio : 0f);
                float num171 = 0.08f + (death ? 0.04f * (1f - lifeRatio) : 0f);
                float num172 = 8.5f - (death ? 2f * (1f - lifeRatio) : 0f);
                if (malice)
                {
                    num169 *= 1.25f;
                    num170 *= 0.75f;
                    num171 *= 1.25f;
                    num172 *= 0.75f;
                }

                if (npc.position.Y > Main.player[npc.target].position.Y - 250f)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= 0.98f;
                    npc.velocity.Y -= num169;
                    if (npc.velocity.Y > num170)
                        npc.velocity.Y = num170;
                }
                else if (npc.position.Y < Main.player[npc.target].position.Y - 250f)
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= 0.98f;
                    npc.velocity.Y += num169;
                    if (npc.velocity.Y < -num170)
                        npc.velocity.Y = -num170;
                }

                if (npc.position.X + (npc.width / 2) > Main.player[npc.target].position.X + (Main.player[npc.target].width / 2))
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= 0.98f;
                    npc.velocity.X -= num171;
                    if (npc.velocity.X > num172)
                        npc.velocity.X = num172;
                }

                if (npc.position.X + (npc.width / 2) < Main.player[npc.target].position.X + (Main.player[npc.target].width / 2))
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= 0.98f;
                    npc.velocity.X += num171;
                    if (npc.velocity.X < -num172)
                        npc.velocity.X = -num172;
                }
            }

            // Spin charge
            else if (npc.ai[1] == 1f)
            {
                npc.defense -= 10;

                float phaseChangeRateBoost = 0.5f * (1f - lifeRatio);
                npc.ai[2] += 1f + phaseChangeRateBoost;

                calamityGlobalNPC.newAI[1] += 1f;
                if (calamityGlobalNPC.newAI[1] == 2f)
                    SoundEngine.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);

                if (npc.ai[2] >= 300f)
                {
                    npc.ai[2] = 0f;
                    npc.ai[1] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;

                    npc.TargetClosest();
                    npc.SyncExtraAI();
                    npc.netUpdate = true;
                }

                npc.rotation += npc.direction * 0.3f;
                Vector2 vector20 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float num173 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector20.X;
                float num174 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector20.Y;
                float num175 = (float)Math.Sqrt(num173 * num173 + num174 * num174);

                // Increase speed while charging
                npc.damage = (int)(npc.defDamage * 1.3);
                float num176 = BossRushEvent.BossRushActive ? 9f : malice ? 6f : 4.5f;
                float velocityBoost = death ? 1f : 1f - lifeRatio;
                if (handsDead || malice)
                    num176 += velocityBoost;

                if (num175 > 150f)
                {
                    float baseDistanceVelocityMult = 1f + MathHelper.Clamp((num175 - 150f) * 0.0015f, 0.05f, 1.5f);
                    num176 *= baseDistanceVelocityMult;
                }

                num175 = num176 / num175;
                npc.velocity.X = num173 * num175;
                npc.velocity.Y = num174 * num175;
            }

            // Daytime enrage
            else if (npc.ai[1] == 2f)
            {
                npc.damage = 1000;
                calamityGlobalNPC.DR = 0.9999f;
                calamityGlobalNPC.unbreakableDR = true;
                npc.rotation += npc.direction * 0.3f;
                Vector2 vector21 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float num177 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector21.X;
                float num178 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector21.Y;
                float num179 = (float)Math.Sqrt(num177 * num177 + num178 * num178);
                num179 = 8f / num179;
                npc.velocity.X = num177 * num179;
                npc.velocity.Y = num178 * num179;
            }

            // Despawn
            else if (npc.ai[1] == 3f)
            {
                npc.velocity.Y += 0.1f;
                if (npc.velocity.Y < 0f)
                    npc.velocity.Y *= 0.95f;
                npc.velocity.X *= 0.95f;
                if (npc.timeLeft > 50)
                    npc.timeLeft = 50;
            }

            // Emit dust
            if (npc.ai[1] != 2f && npc.ai[1] != 3f && num156 != 0)
            {
                int num180 = Dust.NewDust(new Vector2(npc.position.X + (npc.width / 2) - 15f - npc.velocity.X * 5f, npc.position.Y + npc.height - 2f), 30, 10, 5, -npc.velocity.X * 0.2f, 3f, 0, default, 2f);
                Main.dust[num180].noGravity = true;
                Main.dust[num180].velocity.X = Main.dust[num180].velocity.X * 1.3f;
                Main.dust[num180].velocity.X = Main.dust[num180].velocity.X + npc.velocity.X * 0.4f;
                Main.dust[num180].velocity.Y = Main.dust[num180].velocity.Y + (2f + npc.velocity.Y);
                for (int num181 = 0; num181 < 2; num181++)
                {
                    num180 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + 120f), npc.width, 60, 5, npc.velocity.X, npc.velocity.Y, 0, default, 2f);
                    Main.dust[num180].noGravity = true;
                    Main.dust[num180].velocity -= npc.velocity;
                    Main.dust[num180].velocity.Y = Main.dust[num180].velocity.Y + 5f;
                }
            }

            return false;
        }

        public static bool BuffedSkeletronHandAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            float yMultiplier = 1f;
            if (calamityGlobalNPC.newAI[0] != 0f)
                yMultiplier = calamityGlobalNPC.newAI[0];

            // Inflict 0 damage for 3 seconds after spawning
            if (calamityGlobalNPC.newAI[1] < 180f)
            {
                calamityGlobalNPC.newAI[1] += 1f;
                if (calamityGlobalNPC.newAI[1] % 15f == 0f)
                    npc.SyncExtraAI();
                npc.damage = 0;
            }
            else
                npc.damage = npc.defDamage;

            npc.spriteDirection = -(int)npc.ai[0];

            if (Main.npc[(int)npc.ai[1]].ai[3] == -60f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Teleport dust
                    for (int m = 0; m < 10; m++)
                    {
                        int num39 = Dust.NewDust(npc.position, npc.width, npc.height, 27, 0f, 0f, 200, default, 3f);
                        Main.dust[num39].noGravity = true;
                        Main.dust[num39].velocity.X = Main.dust[num39].velocity.X * 2f;
                    }

                    // New location
                    npc.Center = Main.npc[(int)npc.ai[1]].Center;
                    npc.velocity = Vector2.Zero;
                    npc.netUpdate = true;
                }
            }

            if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != 11)
            {
                npc.ai[2] += 10f;
                if (npc.ai[2] > 50f || Main.netMode != NetmodeID.Server)
                {
                    npc.life = -1;
                    npc.HitEffect(0, 10.0);
                    npc.active = false;
                }
            }
            if (npc.ai[2] == 0f || npc.ai[2] == 3f)
            {
                if (Main.npc[(int)npc.ai[1]].ai[1] == 3f && npc.timeLeft > 10)
                    npc.timeLeft = 10;

                if (Main.npc[(int)npc.ai[1]].ai[1] != 0f)
                {
                    float maxX = malice ? 4f : death ? 6f : 7f;
                    float maxY = malice ? 3f : death ? 4.5f : 5f;

                    if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y - 100f * yMultiplier)
                    {
                        if (npc.velocity.Y > 0f)
                            npc.velocity.Y *= 0.94f;
                        npc.velocity.Y -= 0.08f;
                        if (npc.velocity.Y > maxY)
                            npc.velocity.Y = maxY;
                    }
                    else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 100f * yMultiplier)
                    {
                        if (npc.velocity.Y < 0f)
                            npc.velocity.Y *= 0.94f;
                        npc.velocity.Y += 0.08f;
                        if (npc.velocity.Y < -maxY)
                            npc.velocity.Y = -maxY;
                    }

                    if (npc.position.X + (npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 120f * npc.ai[0])
                    {
                        if (npc.velocity.X > 0f)
                            npc.velocity.X *= 0.94f;
                        npc.velocity.X -= 0.12f;
                        if (npc.velocity.X > maxX)
                            npc.velocity.X = maxX;
                    }

                    if (npc.position.X + (npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 120f * npc.ai[0])
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X *= 0.94f;
                        npc.velocity.X += 0.12f;
                        if (npc.velocity.X < -maxX)
                            npc.velocity.X = -maxX;
                    }
                }
                else
                {
                    npc.ai[3] += 1f;
                    if (npc.ai[3] >= 200f)
                    {
                        npc.ai[2] += 1f;
                        npc.ai[3] = 0f;
                        npc.netUpdate = true;
                    }

                    float maxX = malice ? 4f : death ? 6f : 7f;
                    float maxY = malice ? 1f : death ? 2f : 2.5f;

                    if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y + 230f * yMultiplier)
                    {
                        if (npc.velocity.Y > 0f)
                            npc.velocity.Y *= 0.92f;
                        npc.velocity.Y -= 0.1f;
                        if (npc.velocity.Y > maxY)
                            npc.velocity.Y = maxY;
                    }
                    else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y + 230f * yMultiplier)
                    {
                        if (npc.velocity.Y < 0f)
                            npc.velocity.Y *= 0.92f;
                        npc.velocity.Y += 0.1f;
                        if (npc.velocity.Y < -maxY)
                            npc.velocity.Y = -maxY;
                    }

                    if (npc.position.X + (npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0])
                    {
                        if (npc.velocity.X > 0f)
                            npc.velocity.X *= 0.92f;
                        npc.velocity.X -= 0.18f;
                        if (npc.velocity.X > maxX)
                            npc.velocity.X = maxX;
                    }

                    if (npc.position.X + (npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0])
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X *= 0.92f;
                        npc.velocity.X += 0.18f;
                        if (npc.velocity.X < -maxX)
                            npc.velocity.X = -maxX;
                    }
                }

                Vector2 vector22 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float num182 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector22.X;
                float num183 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector22.Y;
                float num184 = (float)Math.Sqrt(num182 * num182 + num183 * num183);
                npc.rotation = (float)Math.Atan2(num183, num182) + MathHelper.PiOver2;
                return false;
            }
            if (npc.ai[2] == 1f)
            {
                Vector2 vector23 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float num185 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector23.X;
                float num186 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector23.Y;
                float num187 = (float)Math.Sqrt(num185 * num185 + num186 * num186);
                npc.rotation = (float)Math.Atan2(num186, num185) + MathHelper.PiOver2;
                npc.velocity.X *= 0.95f;
                npc.velocity.Y -= 0.2f;

                if (npc.velocity.Y < -13f)
                    npc.velocity.Y = -13f;

                if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 200f)
                {
                    npc.TargetClosest();
                    npc.ai[2] = 2f;
                    vector23 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    num185 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector23.X;
                    num186 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector23.Y;
                    num187 = (float)Math.Sqrt(num185 * num185 + num186 * num186);
                    num187 = 22f / num187;
                    npc.velocity.X = num185 * num187;
                    npc.velocity.Y = num186 * num187;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[2] == 2f)
            {
                if (npc.position.Y > Main.player[npc.target].position.Y || npc.velocity.Y < 0f || npc.velocity == Vector2.Zero)
                {
                    npc.ai[2] = 3f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[2] == 4f)
            {
                Vector2 vector24 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float num188 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector24.X;
                float num189 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector24.Y;
                float num190 = (float)Math.Sqrt(num188 * num188 + num189 * num189);
                npc.rotation = (float)Math.Atan2(num189, num188) + MathHelper.PiOver2;
                npc.velocity.Y *= 0.95f;
                npc.velocity.X += 0.2f * -npc.ai[0];

                if (npc.velocity.X < -12f)
                    npc.velocity.X = -12f;
                else if (npc.velocity.X > 12f)
                    npc.velocity.X = 12f;

                if (npc.position.X + (npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 500f || npc.position.X + (npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) + 500f)
                {
                    npc.TargetClosest();
                    npc.ai[2] = 5f;
                    vector24 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    num188 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector24.X;
                    num189 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector24.Y;
                    num190 = (float)Math.Sqrt(num188 * num188 + num189 * num189);
                    num190 = 23f / num190;
                    npc.velocity.X = num188 * num190;
                    npc.velocity.Y = num189 * num190;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[2] == 5f && ((npc.velocity.X > 0f && npc.position.X + (npc.width / 2) > Main.player[npc.target].position.X + (Main.player[npc.target].width / 2)) || (npc.velocity.X < 0f && npc.position.X + (npc.width / 2) < Main.player[npc.target].position.X + (Main.player[npc.target].width / 2)) || npc.velocity == Vector2.Zero))
            {
                npc.ai[2] = 0f;
                npc.netUpdate = true;
            }

            return false;
        }

        public static void RevengeanceDungeonGuardianAI(NPC npc)
        {
            Player target = Main.player[npc.target];
            if (npc.ai[1] != 3f)
            {
                Vector2 targetVector = target.Center - npc.Center;
                float targetDist = targetVector.Length();
                targetDist = 12f / targetDist;
                npc.velocity.X = targetVector.X * targetDist;
                npc.velocity.Y = targetVector.Y * targetDist;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (npc.localAI[1]++ % 60f == 59f)
                    {
                        Vector2 source = npc.Center;
                        if (Collision.CanHit(source, 1, 1, target.Center, target.width, target.height))
                        {
                            float speed = 5f;
                            float xDist = target.Center.X - source.X + Main.rand.Next(-20, 21);
                            float yDist = target.Center.Y - source.Y + Main.rand.Next(-20, 21);
                            Vector2 velocity = new Vector2(xDist, yDist);
                            float distTarget = velocity.Length();
                            distTarget = speed / distTarget;
                            velocity.X *= distTarget;
                            velocity.Y *= distTarget;
                            Vector2 offset = new Vector2(velocity.X * 1f + Main.rand.Next(-50, 51) * 0.01f, velocity.Y * 1f + Main.rand.Next(-50, 51) * 0.01f);
                            offset.Normalize();
                            offset *= speed;
                            offset += npc.velocity;
                            velocity.X = offset.X;
                            velocity.Y = offset.Y;
                            int damage = 2500;
                            int projType = ProjectileID.Skull;
                            source += offset * 5f;
                            int skull = Projectile.NewProjectile(source, velocity, projType, damage, 0f, Main.myPlayer, -1f, 0f);
                            Main.projectile[skull].timeLeft = 300;
                        }
                    }
                }
            }
        }
    }
}
