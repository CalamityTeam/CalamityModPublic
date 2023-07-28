using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.VanillaNPCOverrides.Bosses
{
    public static class MoonLordAI
    {
        public static readonly SoundStyle DeathrayChargeSound = new("CalamityMod/Sounds/Custom/MoonLordLaserCharge");

        public static bool BuffedMoonLordAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;

            int aggressionLevel = 4;
            if (npc.type == NPCID.MoonLordCore || npc.type == NPCID.MoonLordHand || npc.type == NPCID.MoonLordHead)
            {
                switch (NPC.CountNPCS(NPCID.MoonLordFreeEye))
                {
                    case 0:
                        break;
                    case 1:
                        aggressionLevel = 3;
                        break;
                    case 2:
                        aggressionLevel = 2;
                        break;
                    case 3:
                        aggressionLevel = 1;
                        break;
                    default:
                        break;
                }
            }

            if (bossRush)
                aggressionLevel = 5;

            if (Main.getGoodWorld)
                aggressionLevel = 6;

            if (npc.type == NPCID.MoonLordCore)
            {
                // Play a random Moon Lord sound
                if (npc.ai[0] != -1f && npc.ai[0] != 2f && Main.rand.NextBool(200))
                {
                    SoundStyle voiceSound = Utils.SelectRandom(Main.rand, new SoundStyle[]
                    {
                        SoundID.Zombie93,
                        SoundID.Zombie94,
                        SoundID.Zombie95,
                        SoundID.Zombie96,
                        SoundID.Zombie97,
                        SoundID.Zombie98,
                        SoundID.Zombie99
                    });
                    SoundEngine.PlaySound(voiceSound, npc.Center);
                }

                // Start the AI
                if (npc.localAI[3] == 0f)
                {
                    npc.netUpdate = true;
                    npc.localAI[3] = 1f;
                    npc.ai[0] = -1f;
                }

                if (npc.ai[0] == -2f)
                {
                    npc.dontTakeDamage = true;

                    npc.ai[1] += 1f;
                    if (npc.ai[1] == 30f)
                        SoundEngine.PlaySound(SoundID.Zombie92, npc.Center);

                    if (npc.ai[1] < 60f)
                        MoonlordDeathDrama.RequestLight(npc.ai[1] / 30f, npc.Center);

                    if (npc.ai[1] == 60f)
                    {
                        npc.ai[1] = 0f;
                        npc.ai[0] = 0f;
                        if (Main.netMode != NetmodeID.MultiplayerClient && npc.type == NPCID.MoonLordCore)
                        {
                            npc.ai[2] = Main.rand.Next(3);
                            npc.ai[2] = 0f;
                            npc.netUpdate = true;
                        }
                    }
                }

                // Spawn head and hands
                if (npc.ai[0] == -1f)
                {
                    npc.dontTakeDamage = true;

                    npc.ai[1] += 1f;
                    if (npc.ai[1] == 30f)
                        SoundEngine.PlaySound(SoundID.Zombie92, npc.Center);

                    if (npc.ai[1] < 60f)
                        MoonlordDeathDrama.RequestLight(npc.ai[1] / 30f, npc.Center);

                    if (npc.ai[1] == 60f)
                    {
                        npc.ai[1] = 0f;
                        npc.ai[0] = 0f;

                        if (Main.netMode != NetmodeID.MultiplayerClient && npc.type == NPCID.MoonLordCore)
                        {
                            npc.ai[2] = Main.rand.Next(3);
                            npc.ai[2] = 0f;

                            npc.netUpdate = true;
                            int[] array5 = new int[3];
                            int num1155 = 0;
                            int num;

                            for (int num1156 = 0; num1156 < 2; num1156 = num + 1)
                            {
                                int num1157 = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X + num1156 * 800 - 400, (int)npc.Center.Y - 100, NPCID.MoonLordHand, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                                Main.npc[num1157].ai[2] = num1156;
                                Main.npc[num1157].netUpdate = true;
                                int[] arg_381A6_0 = array5;
                                num = num1155;
                                num1155 = num + 1;
                                arg_381A6_0[num] = num1157;
                                num = num1156;
                            }

                            int num1158 = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y - 400, NPCID.MoonLordHead, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                            Main.npc[num1158].netUpdate = true;
                            int[] arg_3823F_0 = array5;
                            num = num1155;
                            num1155 = num + 1;
                            arg_3823F_0[num] = num1158;

                            for (int num1159 = 0; num1159 < 3; num1159 = num + 1)
                            {
                                Main.npc[array5[num1159]].ai[3] = npc.whoAmI;
                                num = num1159;
                            }
                            for (int num1160 = 0; num1160 < 3; num1160 = num + 1)
                            {
                                npc.localAI[num1160] = array5[num1160];
                                num = num1160;
                            }
                        }
                    }
                }

                int trueEyesThatShouldBeActive = 0;
                if (Main.npc[(int)npc.localAI[0]].Calamity().newAI[0] == 1f)
                    trueEyesThatShouldBeActive++;
                if (Main.npc[(int)npc.localAI[1]].Calamity().newAI[0] == 1f)
                    trueEyesThatShouldBeActive++;
                if (Main.npc[(int)npc.localAI[2]].Calamity().newAI[0] == 1f)
                    trueEyesThatShouldBeActive++;

                if (NPC.CountNPCS(NPCID.MoonLordFreeEye) < trueEyesThatShouldBeActive)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int num = NPC.NewNPC(npc.GetSource_FromAI(), (int)Main.npc[(int)npc.localAI[2]].Center.X, (int)Main.npc[(int)npc.localAI[2]].Center.Y, NPCID.MoonLordFreeEye);
                        Main.npc[num].ai[3] = npc.whoAmI;
                        Main.npc[num].netUpdate = true;
                    }
                }

                // Fly near target, don't take damage
                if (npc.ai[0] == 0f)
                {
                    npc.dontTakeDamage = true;
                    npc.TargetClosest(false);

                    Vector2 value4 = Main.player[npc.target].Center - npc.Center + new Vector2(0f, 130f);
                    if (value4.Length() > 20f)
                    {
                        float velocity = death ? 9.5f : 9.25f;
                        switch (aggressionLevel)
                        {
                            case 6:
                                velocity += 4f;
                                break;
                            case 5:
                                velocity += 2f;
                                break;
                            case 4:
                                break;
                            case 3:
                                velocity -= 0.25f;
                                break;
                            case 2:
                                velocity -= 0.5f;
                                break;
                            case 1:
                                velocity -= 0.75f;
                                break;
                            default:
                                break;
                        }
                        if (Main.npc[(int)npc.localAI[2]].ai[0] == 1f)
                            velocity -= 2f;

                        Vector2 desiredVelocity = Vector2.Normalize(value4 - npc.velocity) * velocity;
                        Vector2 velocity2 = npc.velocity;
                        npc.SimpleFlyMovement(desiredVelocity, 0.5f);
                        npc.velocity = Vector2.Lerp(npc.velocity, velocity2, 0.5f);
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        // Despawn if other parts aren't there
                        bool flag84 = false;
                        if (npc.localAI[0] < 0f || npc.localAI[1] < 0f || npc.localAI[2] < 0f)
                            flag84 = true;
                        else if (!Main.npc[(int)npc.localAI[0]].active || Main.npc[(int)npc.localAI[0]].type != NPCID.MoonLordHand)
                            flag84 = true;
                        else if (!Main.npc[(int)npc.localAI[1]].active || Main.npc[(int)npc.localAI[1]].type != NPCID.MoonLordHand)
                            flag84 = true;
                        else if (!Main.npc[(int)npc.localAI[2]].active || Main.npc[(int)npc.localAI[2]].type != NPCID.MoonLordHead)
                            flag84 = true;

                        if (flag84)
                        {
                            npc.life = 0;
                            npc.HitEffect(0, 10.0);
                            npc.active = false;
                        }

                        // Take damage if other parts are down
                        bool flag85 = true;
                        if (Main.npc[(int)npc.localAI[0]].Calamity().newAI[0] != 1f)
                            flag85 = false;
                        if (Main.npc[(int)npc.localAI[1]].Calamity().newAI[0] != 1f)
                            flag85 = false;
                        if (Main.npc[(int)npc.localAI[2]].Calamity().newAI[0] != 1f)
                            flag85 = false;

                        if (flag85)
                        {
                            npc.ai[0] = 1f;
                            npc.dontTakeDamage = false;
                            npc.netUpdate = true;
                        }
                    }
                }

                // Fly near target, take damage
                else if (npc.ai[0] == 1f)
                {
                    npc.dontTakeDamage = false;
                    npc.TargetClosest(false);

                    Vector2 value5 = Main.player[npc.target].Center - npc.Center + new Vector2(0f, 130f);
                    if (value5.Length() > 20f)
                    {
                        float velocity = death ? 9.5f : 9.25f;
                        switch (aggressionLevel)
                        {
                            case 6:
                                velocity += 4f;
                                break;
                            case 5:
                                velocity += 2f;
                                break;
                            case 4:
                                break;
                            case 3:
                                velocity -= 0.25f;
                                break;
                            case 2:
                                velocity -= 0.5f;
                                break;
                            case 1:
                                velocity -= 0.75f;
                                break;
                            default:
                                break;
                        }
                        if (Main.npc[(int)npc.localAI[2]].ai[0] == 1f)
                            velocity -= 2f;

                        Vector2 desiredVelocity2 = Vector2.Normalize(value5 - npc.velocity) * velocity;
                        Vector2 velocity3 = npc.velocity;
                        npc.SimpleFlyMovement(desiredVelocity2, 0.5f);
                        npc.velocity = Vector2.Lerp(npc.velocity, velocity3, 0.5f);
                    }
                }

                // Death effects
                else if (npc.ai[0] == 2f)
                {
                    npc.dontTakeDamage = true;
                    npc.Calamity().ShouldCloseHPBar = true;
                    Vector2 value6 = new Vector2(npc.direction, -0.5f);
                    npc.velocity = Vector2.Lerp(npc.velocity, value6, 0.98f);

                    npc.ai[1] += 1f;
                    float ai1 = npc.ai[1];
                    if (npc.ai[1] < 60f)
                        MoonlordDeathDrama.RequestLight(npc.ai[1] / 60f, npc.Center);

                    if (npc.ai[1] == 60f)
                    {
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile projectile = Main.projectile[i];
                            if (projectile.active && (projectile.type == ProjectileID.MoonLeech || projectile.type == ProjectileID.PhantasmalBolt ||
                                projectile.type == ProjectileID.PhantasmalDeathray || projectile.type == ProjectileID.PhantasmalEye ||
                                projectile.type == ProjectileID.PhantasmalSphere))
                                projectile.Kill();
                        }

                        for (int j = 0; j < Main.maxNPCs; j++)
                        {
                            NPC nPC3 = Main.npc[j];
                            if (nPC3.active && nPC3.type == NPCID.MoonLordFreeEye)
                            {
                                nPC3.HitEffect(0, 9999.0);
                                nPC3.active = false;
                            }
                        }
                    }

                    if (npc.ai[1] % 3f == 0f && npc.ai[1] < 580f && npc.ai[1] > 60f)
                    {
                        Vector2 vector158 = Utils.RandomVector2(Main.rand, -1f, 1f);
                        if (vector158 != Vector2.Zero)
                            vector158.Normalize();

                        vector158 *= 20f + Main.rand.NextFloat() * 400f;
                        Vector2 vector159 = npc.Center + vector158;
                        Point point5 = vector159.ToTileCoordinates();

                        bool flag86 = true;
                        if (!WorldGen.InWorld(point5.X, point5.Y, 0))
                            flag86 = false;
                        if (flag86 && WorldGen.SolidTile(point5.X, point5.Y))
                            flag86 = false;

                        float num1163 = Main.rand.Next(6, 19);
                        float num1164 = MathHelper.TwoPi / num1163;
                        float num1165 = MathHelper.TwoPi * Main.rand.NextFloat();
                        float scaleFactor8 = 1f + Main.rand.NextFloat() * 2f;
                        float num1166 = 1f + Main.rand.NextFloat();
                        float fadeIn = 0.4f + Main.rand.NextFloat();
                        int num1167 = Utils.SelectRandom(Main.rand, new int[]
                        {
                            31,
                            229
                        });

                        if (flag86)
                        {
                            //MoonlordDeathDrama.AddExplosion(vector159);
                            for (float num1168 = 0f; num1168 < num1163 * 2f; num1168 = ai1 + 1f)
                            {
                                Dust dust2 = Main.dust[Dust.NewDust(vector159, 0, 0, 229, 0f, 0f, 0, default, 1f)];
                                dust2.noGravity = true;
                                dust2.position = vector159;
                                dust2.velocity = Vector2.UnitY.RotatedBy(num1165 + num1164 * num1168) * scaleFactor8 * (Main.rand.NextFloat() * 1.6f + 1.6f);
                                dust2.fadeIn = fadeIn;
                                dust2.scale = num1166;
                                ai1 = num1168;
                            }
                        }

                        for (float num1169 = 0f; num1169 < npc.ai[1] / 60f; num1169 = ai1 + 1f)
                        {
                            Vector2 vector160 = Utils.RandomVector2(Main.rand, -1f, 1f);
                            if (vector160 != Vector2.Zero)
                                vector160.Normalize();

                            vector160 *= 20f + Main.rand.NextFloat() * 800f;
                            Vector2 vector161 = npc.Center + vector160;
                            Point point6 = vector161.ToTileCoordinates();

                            bool flag87 = true;
                            if (!WorldGen.InWorld(point6.X, point6.Y, 0))
                                flag87 = false;
                            if (flag87 && WorldGen.SolidTile(point6.X, point6.Y))
                                flag87 = false;

                            if (flag87)
                            {
                                Dust dust3 = Main.dust[Dust.NewDust(vector161, 0, 0, num1167, 0f, 0f, 0, default, 1f)];
                                dust3.noGravity = true;
                                dust3.position = vector161;
                                dust3.velocity = -Vector2.UnitY * scaleFactor8 * (Main.rand.NextFloat() * 0.9f + 1.6f);
                                dust3.fadeIn = fadeIn;
                                dust3.scale = num1166;
                            }

                            ai1 = num1169;
                        }
                    }

                    if (npc.ai[1] % 15f == 0f && npc.ai[1] < 480f && npc.ai[1] >= 90f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 vector162 = Utils.RandomVector2(Main.rand, -1f, 1f);
                        if (vector162 != Vector2.Zero)
                            vector162.Normalize();

                        vector162 *= 20f + Main.rand.NextFloat() * 400f;
                        bool flag88 = true;
                        Vector2 vector163 = npc.Center + vector162;
                        Point point7 = vector163.ToTileCoordinates();

                        if (!WorldGen.InWorld(point7.X, point7.Y, 0))
                            flag88 = false;
                        if (flag88 && WorldGen.SolidTile(point7.X, point7.Y))
                            flag88 = false;

                        if (flag88)
                        {
                            float num1170 = (Main.rand.Next(4) < 2).ToDirectionInt() * (0.3926991f + MathHelper.PiOver4 * Main.rand.NextFloat());
                            Vector2 vector164 = new Vector2(0f, -Main.rand.NextFloat() * 0.5f - 0.5f).RotatedBy(num1170) * 6f;
                            Projectile.NewProjectile(npc.GetSource_FromAI(), vector163.X, vector163.Y, vector164.X, vector164.Y, ProjectileID.BlowupSmokeMoonlord, 0, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }

                    if (npc.ai[1] == 1f)
                        SoundEngine.PlaySound(SoundID.NPCDeath61, npc.Center);

                    if (npc.ai[1] >= 480f)
                        MoonlordDeathDrama.RequestLight((npc.ai[1] - 480f) / 120f, npc.Center);

                    if (npc.ai[1] >= 600f)
                    {
                        npc.life = 0;
                        npc.HitEffect(0, 1337.0);
                        npc.checkDead();

                        for (int num1174 = 0; num1174 < Main.maxNPCs; num1174++)
                        {
                            NPC nPC5 = Main.npc[num1174];
                            if (nPC5.active && (nPC5.type == NPCID.MoonLordHand || nPC5.type == NPCID.MoonLordHead))
                            {
                                nPC5.active = false;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, nPC5.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                            }
                        }

                        npc.active = false;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);

                        return false;
                    }
                }

                // Despawn effects
                else if (npc.ai[0] == 3f)
                {
                    npc.dontTakeDamage = true;
                    Vector2 value7 = new Vector2(npc.direction, -0.5f);
                    npc.velocity = Vector2.Lerp(npc.velocity, value7, 0.98f);

                    npc.ai[1] += 1f;
                    if (npc.ai[1] < 60f)
                        MoonlordDeathDrama.RequestLight(npc.ai[1] / 40f, npc.Center);

                    if (npc.ai[1] == 40f)
                    {
                        for (int num1171 = 0; num1171 < Main.maxProjectiles; num1171++)
                        {
                            Projectile projectile2 = Main.projectile[num1171];
                            if (projectile2.active && (projectile2.type == ProjectileID.MoonLeech || projectile2.type == ProjectileID.PhantasmalBolt ||
                                projectile2.type == ProjectileID.PhantasmalDeathray || projectile2.type == ProjectileID.PhantasmalEye ||
                                projectile2.type == ProjectileID.PhantasmalSphere))
                            {
                                projectile2.active = false;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, num1171, 0f, 0f, 0f, 0, 0, 0);
                            }
                        }

                        for (int num1172 = 0; num1172 < Main.maxNPCs; num1172++)
                        {
                            NPC nPC4 = Main.npc[num1172];
                            if (nPC4.active && nPC4.type == NPCID.MoonLordFreeEye)
                            {
                                nPC4.active = false;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, nPC4.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                            }
                        }

                        for (int num1173 = 0; num1173 < Main.maxGore; num1173++)
                        {
                            Gore gore2 = Main.gore[num1173];
                            if (gore2.active && gore2.type >= GoreID.MoonLordHeart1 && gore2.type <= GoreID.MoonLordHeart4)
                                gore2.active = false;
                        }
                    }

                    if (npc.ai[1] >= 60f)
                    {
                        for (int num1174 = 0; num1174 < Main.maxNPCs; num1174++)
                        {
                            NPC nPC5 = Main.npc[num1174];
                            if (nPC5.active && (nPC5.type == NPCID.MoonLordFreeEye || nPC5.type == NPCID.MoonLordHand || nPC5.type == NPCID.MoonLordHead))
                            {
                                nPC5.active = false;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, nPC5.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                            }
                        }

                        npc.active = false;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);

                        NPC.LunarApocalypseIsUp = false;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);

                        return false;
                    }
                }

                // Despawn
                bool flag89 = false;
                if (npc.ai[0] == -2f || npc.ai[0] == -1f || npc.ai[0] == -2f || npc.ai[0] == 3f)
                    flag89 = true;
                if (Main.player[npc.target].active && !Main.player[npc.target].dead)
                    flag89 = true;

                if (!flag89)
                {
                    for (int num1175 = 0; num1175 < Main.maxPlayers; num1175++)
                    {
                        if (Main.player[num1175].active && !Main.player[num1175].dead)
                        {
                            flag89 = true;
                            break;
                        }
                    }
                }
                if (!flag89)
                {
                    npc.ai[0] = 3f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }

                // Teleport
                if (npc.ai[0] >= 0f && npc.ai[0] < 2f && Main.netMode != NetmodeID.MultiplayerClient && npc.Distance(Main.player[npc.target].Center) > 1800f)
                {
                    npc.ai[0] = -2f;
                    npc.netUpdate = true;
                    Vector2 value8 = Main.player[npc.target].Center - Vector2.UnitY * 150f - npc.Center;
                    npc.position += value8;

                    if (Main.npc[(int)npc.localAI[0]].active)
                    {
                        NPC nPC6 = Main.npc[(int)npc.localAI[0]];
                        nPC6.position += value8;
                        Main.npc[(int)npc.localAI[0]].netUpdate = true;
                    }
                    if (Main.npc[(int)npc.localAI[1]].active)
                    {
                        NPC nPC6 = Main.npc[(int)npc.localAI[1]];
                        nPC6.position += value8;
                        Main.npc[(int)npc.localAI[1]].netUpdate = true;
                    }
                    if (Main.npc[(int)npc.localAI[2]].active)
                    {
                        NPC nPC6 = Main.npc[(int)npc.localAI[2]];
                        nPC6.position += value8;
                        Main.npc[(int)npc.localAI[2]].netUpdate = true;
                    }

                    for (int num1176 = 0; num1176 < Main.maxNPCs; num1176++)
                    {
                        NPC nPC7 = Main.npc[num1176];
                        if (nPC7.active && nPC7.type == NPCID.MoonLordFreeEye)
                        {
                            NPC nPC6 = nPC7;
                            nPC6.position += value8;
                            nPC7.netUpdate = true;
                        }
                    }
                }
            }
            else if (npc.type == NPCID.MoonLordHead)
            {
                // Despawn
                if (!Main.npc[(int)npc.ai[3]].active || Main.npc[(int)npc.ai[3]].type != NPCID.MoonLordCore)
                {
                    npc.life = 0;
                    npc.HitEffect(0, 10.0);
                    npc.active = false;
                }

                // Variables
                npc.dontTakeDamage = npc.localAI[3] >= 15f;
                if (calamityGlobalNPC.newAI[0] == 1f)
                    npc.dontTakeDamage = true;

                npc.velocity = Vector2.Zero;
                npc.Center = Main.npc[(int)npc.ai[3]].Center + new Vector2(0f, -400f);
                Vector2 value19 = new Vector2(27f, 59f);
                float num1207 = 0f;
                float num1208 = 0f;
                int num1209 = 0;
                int num1210 = 0;

                // Invulnerable
                if (npc.ai[0] >= 0f || npc.ai[0] == -2f)
                {
                    if (npc.ai[0] == -2f)
                    {
                        if (calamityGlobalNPC.newAI[0] != 1f)
                            calamityGlobalNPC.newAI[0] = 1f;

                        npc.life = npc.lifeMax;
                        npc.netUpdate = true;
                        npc.dontTakeDamage = true;
                    }

                    // Go to die
                    if (Main.npc[(int)npc.ai[3]].ai[0] == 2f)
                    {
                        npc.ai[0] = -3f;
                        return false;
                    }

                    // Set up attacks
                    float num1211 = npc.ai[0];
                    npc.ai[1] += 1f;
                    int num1212 = (int)Main.npc[(int)npc.ai[3]].ai[2];
                    int num1213 = 2;
                    int num1214 = 0;
                    int num1215 = 0;

                    while (num1214 < 5)
                    {
                        num1208 = NPC.MoonLordAttacksArray[num1212, num1213, 1, num1214];
                        if (num1208 + num1215 > npc.ai[1])
                            break;

                        num1215 += (int)num1208;
                        int num = num1214;
                        num1214 = num + 1;
                    }

                    if (num1214 == 5)
                    {
                        num1214 = 0;
                        npc.ai[1] = 0f;
                        num1208 = NPC.MoonLordAttacksArray[num1212, num1213, 1, num1214];
                        num1215 = 0;
                    }

                    npc.ai[0] = NPC.MoonLordAttacksArray[num1212, num1213, 0, num1214];
                    num1207 = (int)npc.ai[1] - num1215;

                    if (npc.ai[0] != num1211)
                        npc.netUpdate = true;
                }

                // Die
                if (npc.ai[0] == -3f)
                {
                    npc.dontTakeDamage = true;
                    npc.rotation = MathHelper.Lerp(npc.rotation, 0.2617994f, 0.07f);

                    npc.ai[1] += 1f;
                    if (npc.ai[1] >= 32f)
                        npc.ai[1] = 0f;
                    if (npc.ai[1] < 0f)
                        npc.ai[1] = 0f;

                    if (npc.localAI[2] < 14f)
                        npc.localAI[2] += 1f;
                }

                // Set variables for deathray
                else if (npc.ai[0] == 0f)
                {
                    num1210 = 3;
                    npc.TargetClosest(false);

                    Vector2 v3 = Main.player[npc.target].Center - npc.Center - new Vector2(0f, -22f);
                    float num1219 = v3.Length() / 500f;
                    if (num1219 > 1f)
                        num1219 = 1f;

                    num1219 = 1f - num1219;
                    num1219 *= 2f;
                    if (num1219 > 1f)
                        num1219 = 1f;

                    npc.localAI[0] = v3.ToRotation();
                    npc.localAI[1] = num1219;
                    npc.localAI[2] = MathHelper.Lerp(npc.localAI[2], 1f, 0.2f);
                }

                // Deathray
                if (npc.ai[0] == 1f)
                {
                    if (num1207 < 180f)
                    {
                        npc.localAI[1] -= 0.05f;
                        if (npc.localAI[1] < 0f)
                            npc.localAI[1] = 0f;

                        if (num1207 >= 60f)
                        {
                            Vector2 center20 = npc.Center;

                            // Hopefully it plays
                            if (num1207 == 60f)
                                SoundEngine.PlaySound(DeathrayChargeSound, center20);

                            int num1220 = 0;
                            if (num1207 >= 120f)
                                num1220 = 1;

                            for (int num1221 = 0; num1221 < 1 + num1220; num1221++)
                            {
                                int num1222 = 229;
                                float num1223 = 0.8f;
                                if (num1221 % 2 == 1)
                                {
                                    num1222 = 229;
                                    num1223 = 1.65f;
                                }

                                Vector2 vector199 = center20 + ((float)Main.rand.NextDouble() * MathHelper.TwoPi).ToRotationVector2() * value19 / 2f;
                                int num1224 = Dust.NewDust(vector199 - Vector2.One * 8f, 16, 16, num1222, npc.velocity.X / 2f, npc.velocity.Y / 2f, 0, default, 1f);
                                Main.dust[num1224].velocity = Vector2.Normalize(center20 - vector199) * 3.5f * (10f - num1220 * 2f) / 10f;
                                Main.dust[num1224].noGravity = true;
                                Main.dust[num1224].scale = num1223;
                                Main.dust[num1224].customData = npc;
                            }
                        }
                    }
                    else if (num1207 < num1208 - 15f)
                    {
                        if (calamityGlobalNPC.newAI[1] == 0f)
                        {
                            calamityGlobalNPC.newAI[1] = 420f;
                            if (death)
                                calamityGlobalNPC.newAI[1] -= 60f;

                            switch (aggressionLevel)
                            {
                                case 6:
                                    calamityGlobalNPC.newAI[1] -= 180f;
                                    break;
                                case 5:
                                    calamityGlobalNPC.newAI[1] -= 90f;
                                    break;
                                case 4:
                                    break;
                                case 3:
                                    calamityGlobalNPC.newAI[1] += 120f;
                                    break;
                                case 2:
                                    calamityGlobalNPC.newAI[1] += 240f;
                                    break;
                                case 1:
                                    calamityGlobalNPC.newAI[1] += 360f;
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (num1207 == 180f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int projectileType = ProjectileID.PhantasmalDeathray;
                            int damage = npc.GetProjectileDamage(projectileType);

                            npc.TargetClosest(false);
                            Vector2 vector200 = Main.player[npc.target].Center - npc.Center;
                            vector200.Normalize();

                            float num1225 = -1f;
                            if (vector200.X < 0f)
                                num1225 = 1f;

                            vector200 = vector200.RotatedBy(-(double)num1225 * MathHelper.TwoPi / 6f);
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y, vector200.X, vector200.Y, projectileType, damage, 0f, Main.myPlayer, num1225 * MathHelper.TwoPi / calamityGlobalNPC.newAI[1], npc.whoAmI);
                            npc.ai[2] = (vector200.ToRotation() + MathHelper.Pi + MathHelper.TwoPi) * num1225;
                            npc.netUpdate = true;
                        }

                        npc.localAI[1] += 0.05f;
                        if (npc.localAI[1] > 1f)
                            npc.localAI[1] = 1f;

                        float num1226 = (npc.ai[2] >= 0f).ToDirectionInt();
                        float num1227 = npc.ai[2];
                        if (num1227 < 0f)
                            num1227 *= -1f;

                        num1227 += -(MathHelper.Pi + MathHelper.TwoPi);
                        num1227 += num1226 * MathHelper.TwoPi / calamityGlobalNPC.newAI[1];
                        npc.localAI[0] = num1227;
                        npc.ai[2] = (num1227 + MathHelper.Pi + MathHelper.TwoPi) * num1226;
                    }
                    else
                    {
                        calamityGlobalNPC.newAI[1] = 0f;

                        npc.localAI[1] -= 0.07f;
                        if (npc.localAI[1] < 0f)
                        {
                            npc.localAI[1] = 0f;
                            if (Main.netMode != NetmodeID.MultiplayerClient && Main.getGoodWorld && Main.remixWorld)
                            {
                                for (int num1268 = 0; num1268 < 30; num1268++)
                                {
                                    if (!WorldGen.SolidTile((int)(npc.Center.X / 16f), (int)(npc.Center.Y / 16f)))
                                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y, (float)Main.rand.Next(-1599, 1600) * 0.01f, (float)Main.rand.Next(-1599, 1) * 0.01f, ProjectileID.MoonBoulder, 70, 10f);
                                }
                            }
                        }

                        num1210 = 3;
                    }
                }

                // Moon Leech thing
                else if (npc.ai[0] == 2f)
                {
                    num1209 = 2;
                    num1210 = 3;
                    Vector2 value21 = new Vector2(0f, 216f);

                    if (num1207 == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 vector201 = npc.Center + value21;
                        for (int num1228 = 0; num1228 < Main.maxPlayers; num1228++)
                        {
                            Player player6 = Main.player[num1228];
                            if (player6.active && !player6.dead && Vector2.Distance(player6.Center, vector201) <= 3000f)
                            {
                                Vector2 vector202 = Main.player[npc.target].Center - vector201;
                                if (vector202 != Vector2.Zero)
                                    vector202.Normalize();

                                Projectile.NewProjectile(npc.GetSource_FromAI(), vector201.X, vector201.Y, vector202.X, vector202.Y, ProjectileID.MoonLeech, 0, 0f, Main.myPlayer, npc.whoAmI + 1, num1228);
                            }
                        }
                    }

                    if ((num1207 == 120f || num1207 == 150f || num1207 == 180f || num1207 == 210f || num1207 == 240f) && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int num1229 = 0; num1229 < Main.maxProjectiles; num1229++)
                        {
                            Projectile projectile6 = Main.projectile[num1229];
                            if (projectile6.active && projectile6.type == ProjectileID.MoonLeech && Main.player[(int)projectile6.ai[1]].FindBuffIndex(BuffID.MoonLeech) != -1)
                            {
                                Vector2 center21 = Main.player[npc.target].Center;
                                int num1230 = NPC.NewNPC(npc.GetSource_FromAI(), (int)center21.X, (int)center21.Y, NPCID.MoonLordLeechBlob);
                                Main.npc[num1230].netUpdate = true;
                                Main.npc[num1230].ai[0] = npc.whoAmI + 1;
                                Main.npc[num1230].ai[1] = num1229;
                            }
                        }
                    }
                }

                // Phantasmal Bolts
                else if (npc.ai[0] == 3f)
                {
                    if (num1207 == 0f)
                    {
                        npc.TargetClosest(false);
                        npc.netUpdate = true;
                    }

                    Vector2 v4 = Main.player[npc.target].Center - npc.Center;
                    bool shootFirstBolt = num1207 == num1208 - 14f;
                    bool shootSecondBolt = num1207 == num1208 - 7f;
                    bool shootThirdBolt = num1207 == num1208;
                    switch (aggressionLevel)
                    {
                        case 6:
                            v4 = Main.player[npc.target].Center + Main.player[npc.target].velocity * 30f - npc.Center;
                            break;
                        case 5:
                            v4 = Main.player[npc.target].Center + Main.player[npc.target].velocity * 20f - npc.Center;
                            break;
                        case 4:
                            break;
                        case 3:
                        case 2:
                            shootSecondBolt = false;
                            break;
                        case 1:
                            shootSecondBolt = false;
                            shootThirdBolt = false;
                            break;
                        default:
                            break;
                    }

                    npc.localAI[0] = npc.localAI[0].AngleLerp(v4.ToRotation(), 0.5f);
                    npc.localAI[1] += 0.05f;
                    if (npc.localAI[1] > 1f)
                        npc.localAI[1] = 1f;

                    if (num1207 == num1208 - 35f)
                        SoundEngine.PlaySound(SoundID.NPCDeath6, npc.position);

                    if ((shootFirstBolt || shootSecondBolt || shootThirdBolt) && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 vector203 = Utils.Vector2FromElipse(npc.localAI[0].ToRotationVector2(), value19 * npc.localAI[1]);

                        float velocity = death ? 6.75f : 6.25f;
                        switch (aggressionLevel)
                        {
                            case 6:
                            case 5:
                            case 4:
                                break;
                            case 3:
                                velocity -= 0.25f;
                                break;
                            case 2:
                                velocity -= 0.5f;
                                break;
                            case 1:
                                velocity -= 0.75f;
                                break;
                            default:
                                break;
                        }

                        Vector2 vector204 = Vector2.Normalize(v4) * velocity;
                        int type = ProjectileID.PhantasmalBolt;
                        int damage = npc.GetProjectileDamage(type);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + vector203.X, npc.Center.Y + vector203.Y, vector204.X, vector204.Y, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                }

                // Dictates whether this npc is vulnerable or not
                int num1231 = num1209 * 7;
                if (num1231 > npc.localAI[2])
                    npc.localAI[2] += 1f;
                if (num1231 < npc.localAI[2])
                    npc.localAI[2] -= 1f;
                if (npc.localAI[2] < 0f)
                    npc.localAI[2] = 0f;
                if (npc.localAI[2] > 14f)
                    npc.localAI[2] = 14f;

                int num1232 = num1210 * 5;
                if (num1232 > npc.localAI[3])
                    npc.localAI[3] += 1f;
                if (num1232 < npc.localAI[3])
                    npc.localAI[3] -= 1f;
                if (npc.localAI[3] < 0f)
                    npc.localAI[2] = 0f;
                if (npc.localAI[3] > 15f)
                    npc.localAI[2] = 15f;
            }
            else if (npc.type == NPCID.MoonLordHand)
            {
                // Start attack array
                NPC.InitializeMoonLordAttacks();

                // Despawn
                if (!Main.npc[(int)npc.ai[3]].active || Main.npc[(int)npc.ai[3]].type != NPCID.MoonLordCore)
                {
                    npc.life = 0;
                    npc.HitEffect(0, 10.0);
                    npc.active = false;
                }

                // Variables
                bool flag90 = npc.ai[2] == 0f;
                float num1177 = -flag90.ToDirectionInt();
                npc.spriteDirection = (int)num1177;

                npc.dontTakeDamage = npc.frameCounter >= 21.0;
                if (calamityGlobalNPC.newAI[0] == 1f)
                    npc.dontTakeDamage = true;

                Vector2 vector165 = new Vector2(30f, 66f);
                float num1178 = 0f;
                float num1179 = 0f;
                int num1180 = 0;

                // Go to die
                if (Main.npc[(int)npc.ai[3]].ai[0] == 2f)
                    npc.ai[0] = -2f;

                // Choose attacks
                if (npc.ai[0] != -2f || (npc.ai[0] == -2f && Main.npc[(int)npc.ai[3]].ai[0] != 2f))
                {
                    if (npc.ai[0] == -2f && Main.npc[(int)npc.ai[3]].ai[0] != 2f)
                    {
                        if (calamityGlobalNPC.newAI[0] != 1f)
                            calamityGlobalNPC.newAI[0] = 1f;

                        npc.life = npc.lifeMax;
                        npc.netUpdate = true;
                        npc.dontTakeDamage = true;
                    }

                    float num1181 = npc.ai[0];
                    npc.ai[1] += 1f;
                    int num1182 = (int)Main.npc[(int)npc.ai[3]].ai[2];
                    int num1183 = flag90 ? 0 : 1;
                    int num1184 = 0;
                    int num1185 = 0;

                    while (num1184 < 5)
                    {
                        num1179 = NPC.MoonLordAttacksArray[num1182, num1183, 1, num1184];
                        if (num1179 + num1185 > npc.ai[1])
                            break;

                        num1185 += (int)num1179;
                        int num = num1184;
                        num1184 = num + 1;
                    }

                    if (num1184 == 5)
                    {
                        num1184 = 0;
                        npc.ai[1] = 0f;
                        num1179 = NPC.MoonLordAttacksArray[num1182, num1183, 1, num1184];
                        num1185 = 0;
                    }

                    npc.ai[0] = NPC.MoonLordAttacksArray[num1182, num1183, 0, num1184];
                    num1178 = (int)npc.ai[1] - num1185;
                    if (npc.ai[0] != num1181)
                        npc.netUpdate = true;
                }

                if (npc.ai[0] == -2f)
                {
                    num1180 = 0;

                    npc.dontTakeDamage = true;

                    npc.velocity = Main.npc[(int)npc.ai[3]].velocity;
                }

                // Move
                else if (npc.ai[0] == 0f)
                {
                    num1180 = 3;
                    npc.localAI[1] -= 0.05f;
                    if (npc.localAI[1] < 0f)
                        npc.localAI[1] = 0f;

                    Vector2 center15 = Main.npc[(int)npc.ai[3]].Center;
                    Vector2 value10 = center15 + new Vector2(350f * num1177, -100f);
                    Vector2 vector167 = value10 - npc.Center;

                    if (vector167.Length() > 20f)
                    {
                        vector167.Normalize();

                        float velocity = death ? 7.75f : 7.5f;
                        switch (aggressionLevel)
                        {
                            case 6:
                                velocity += 3f;
                                break;
                            case 5:
                                velocity += 1.5f;
                                break;
                            case 4:
                                break;
                            case 3:
                                velocity -= 0.4f;
                                break;
                            case 2:
                                velocity -= 0.8f;
                                break;
                            case 1:
                                velocity -= 1.2f;
                                break;
                            default:
                                break;
                        }

                        vector167 *= velocity;
                        Vector2 velocity5 = npc.velocity;

                        if (vector167 != Vector2.Zero)
                            npc.SimpleFlyMovement(vector167, 0.3f);

                        npc.velocity = Vector2.Lerp(velocity5, npc.velocity, 0.5f);
                    }
                }

                // Phantasmal Eyes
                else if (npc.ai[0] == 1f)
                {
                    num1180 = 0;
                    int num1186 = 7;
                    int num1187 = 4;

                    float divisor = 6f;
                    switch (aggressionLevel)
                    {
                        case 6:
                            divisor = 3f;
                            break;
                        case 5:
                            divisor = 4f;
                            break;
                        case 4:
                            break;
                        case 3:
                            divisor = 8f;
                            break;
                        case 2:
                            divisor = 10f;
                            break;
                        case 1:
                            divisor = 12f;
                            break;
                        default:
                            break;
                    }

                    if (num1178 >= (num1186 * num1187 * 2))
                    {
                        npc.localAI[1] -= 0.07f;
                        if (npc.localAI[1] < 0f)
                            npc.localAI[1] = 0f;
                    }
                    else if (num1178 >= (num1186 * num1187))
                    {
                        npc.localAI[1] += 0.05f;
                        if (npc.localAI[1] > 0.75f)
                            npc.localAI[1] = 0.75f;

                        float num1188 = MathHelper.TwoPi * (num1178 % (num1186 * num1187)) / (num1186 * num1187) - MathHelper.PiOver2;
                        npc.localAI[0] = new Vector2((float)Math.Cos(num1188) * vector165.X, (float)Math.Sin(num1188) * vector165.Y).ToRotation();

                        if (num1178 % divisor == 0f)
                        {
                            Vector2 value11 = new Vector2(1f * -num1177, 3f);
                            Vector2 vector168 = Utils.Vector2FromElipse(npc.localAI[0].ToRotationVector2(), vector165 * npc.localAI[1]);
                            Vector2 vector169 = npc.Center + Vector2.Normalize(vector168) * vector165.Length() * 0.4f + value11;

                            float velocity = death ? 3.5f : 3f;
                            switch (aggressionLevel)
                            {
                                case 6:
                                case 5:
                                case 4:
                                    break;
                                case 3:
                                    velocity += 0.5f;
                                    break;
                                case 2:
                                    velocity += 1f;
                                    break;
                                case 1:
                                    velocity += 1.5f;
                                    break;
                                default:
                                    break;
                            }

                            Vector2 vector170 = Vector2.Normalize(vector168) * velocity;
                            float ai = (MathHelper.TwoPi * (float)Main.rand.NextDouble() - MathHelper.Pi) / 30f + 0.0174532924f * num1177;
                            int type = ProjectileID.PhantasmalEye;
                            int damage = npc.GetProjectileDamage(type);
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), vector169, vector170, type, damage, 0f, Main.myPlayer, 0f, ai);
                            Main.projectile[proj].timeLeft = 1200;
                            Main.projectile[proj].Calamity().lineColor = bossRush ? 1 : aggressionLevel;
                        }
                    }
                    else
                    {
                        npc.localAI[1] += 0.02f;
                        if (npc.localAI[1] > 0.75f)
                            npc.localAI[1] = 0.75f;

                        float num1189 = MathHelper.TwoPi * (num1178 % (num1186 * num1187)) / (num1186 * num1187) - MathHelper.PiOver2;
                        npc.localAI[0] = new Vector2((float)Math.Cos(num1189) * vector165.X, (float)Math.Sin(num1189) * vector165.Y).ToRotation();
                    }
                }

                // Phantasmal Spheres
                else if (npc.ai[0] == 2f)
                {
                    npc.localAI[1] -= 0.05f;
                    if (npc.localAI[1] < 0f)
                        npc.localAI[1] = 0f;

                    Vector2 center16 = Main.npc[(int)npc.ai[3]].Center;
                    Vector2 value12 = new Vector2(220f * num1177, -60f) + center16;
                    value12 += new Vector2(num1177 * 100f, -50f);
                    Vector2 value13 = new Vector2(400f * num1177, -60f);

                    float velocityMultiplier = death ? 0.88f : 0.885f;
                    switch (aggressionLevel)
                    {
                        case 6:
                            velocityMultiplier -= 0.04f;
                            break;
                        case 5:
                            velocityMultiplier -= 0.02f;
                            break;
                        case 4:
                            break;
                        case 3:
                            velocityMultiplier += 0.004f;
                            break;
                        case 2:
                            velocityMultiplier += 0.008f;
                            break;
                        case 1:
                            velocityMultiplier += 0.012f;
                            break;
                        default:
                            break;
                    }

                    if (num1178 < 30f)
                    {
                        Vector2 vector171 = value12 - npc.Center;
                        if (vector171 != Vector2.Zero)
                        {
                            Vector2 vector172 = vector171;
                            vector172.Normalize();

                            float velocity = death ? 11f : 10f;
                            switch (aggressionLevel)
                            {
                                case 6:
                                    velocity += 4f;
                                    break;
                                case 5:
                                    velocity += 2f;
                                    break;
                                case 4:
                                    break;
                                case 3:
                                    velocity -= 0.5f;
                                    break;
                                case 2:
                                    velocity -= 1f;
                                    break;
                                case 1:
                                    velocity -= 1.5f;
                                    break;
                                default:
                                    break;
                            }

                            npc.velocity = Vector2.SmoothStep(npc.velocity, vector172 * Math.Min(velocity, vector171.Length()), 0.2f);
                        }
                    }
                    else if (num1178 < 210f)
                    {
                        num1180 = 1;
                        int num1190 = (int)num1178 - 30;

                        int divisor = 30;
                        switch (aggressionLevel)
                        {
                            case 6:
                                divisor = 15;
                                break;
                            case 5:
                                divisor = 20;
                                break;
                            case 4:
                                break;
                            case 3:
                                divisor = 45;
                                break;
                            case 2:
                                divisor = 60;
                                break;
                            case 1:
                                divisor = 90;
                                break;
                            default:
                                break;
                        }

                        if (num1190 % divisor == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 vector173 = new Vector2(5f * num1177, -8f);
                            int num1191 = num1190 / 30;
                            vector173.X += (num1191 - 3.5f) * num1177 * 3f;
                            vector173.Y += (num1191 - 4.5f) * 1f;
                            vector173 *= 1.2f;
                            int type = ProjectileID.PhantasmalSphere;
                            int damage = npc.GetProjectileDamage(type);
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y, vector173.X, vector173.Y, type, damage, 1f, Main.myPlayer, 0f, npc.whoAmI);
                            Main.projectile[proj].timeLeft = 1200;
                        }

                        Vector2 vector174 = Vector2.SmoothStep(value12, value12 + value13, (num1178 - 30f) / 180f) - npc.Center;
                        if (vector174 != Vector2.Zero)
                        {
                            Vector2 vector175 = vector174;
                            vector175.Normalize();

                            float velocity = death ? 26.5f : 24f;
                            switch (aggressionLevel)
                            {
                                case 6:
                                    velocity += 4f;
                                    break;
                                case 5:
                                    velocity += 2f;
                                    break;
                                case 4:
                                    break;
                                case 3:
                                    velocity -= 1f;
                                    break;
                                case 2:
                                    velocity -= 2f;
                                    break;
                                case 1:
                                    velocity -= 3f;
                                    break;
                                default:
                                    break;
                            }

                            npc.velocity = Vector2.Lerp(npc.velocity, vector175 * Math.Min(velocity, vector174.Length()), 0.5f);
                        }
                    }
                    else if (num1178 < 282f)
                    {
                        num1180 = 0;
                        npc.velocity *= velocityMultiplier;
                    }
                    else if (num1178 < 287f)
                    {
                        num1180 = 1;
                        npc.velocity *= velocityMultiplier;
                    }
                    else if (num1178 < 292f)
                    {
                        num1180 = 2;
                        npc.velocity *= velocityMultiplier;
                    }
                    else if (num1178 < 300f)
                    {
                        num1180 = 3;

                        if (num1178 == 292f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int num1193 = Player.FindClosest(npc.position, npc.width, npc.height);
                            Vector2 vector176 = Vector2.Normalize(Main.player[num1193].Center - (npc.Center + Vector2.UnitY * -350f));
                            if (float.IsNaN(vector176.X) || float.IsNaN(vector176.Y))
                                vector176 = Vector2.UnitY;

                            float velocity = death ? 2.2f : 2f;
                            switch (aggressionLevel)
                            {
                                case 6:
                                    velocity += 3f;
                                    break;
                                case 5:
                                    velocity += 1.5f;
                                    break;
                                case 4:
                                    break;
                                case 3:
                                    velocity -= 0.25f;
                                    break;
                                case 2:
                                    velocity -= 0.5f;
                                    break;
                                case 1:
                                    velocity -= 0.75f;
                                    break;
                                default:
                                    break;
                            }

                            vector176 *= velocity;
                            for (int num1194 = 0; num1194 < Main.maxProjectiles; num1194++)
                            {
                                Projectile projectile3 = Main.projectile[num1194];
                                if (projectile3.active && projectile3.type == ProjectileID.PhantasmalSphere && projectile3.ai[1] == npc.whoAmI && projectile3.ai[0] != -1f)
                                {
                                    projectile3.ai[0] = -1f;
                                    projectile3.velocity = vector176;
                                    projectile3.netUpdate = true;
                                }
                            }
                        }

                        Vector2 vector177 = Vector2.SmoothStep(value12, value12 + value13, 1f - (num1178 - 270f) / 30f) - npc.Center;
                        if (vector177 != Vector2.Zero)
                        {
                            Vector2 vector178 = vector177;
                            vector178.Normalize();

                            float velocity = death ? 19.75f : 17.5f;
                            switch (aggressionLevel)
                            {
                                case 6:
                                    velocity += 4f;
                                    break;
                                case 5:
                                    velocity += 2f;
                                    break;
                                case 4:
                                    break;
                                case 3:
                                    velocity -= 1f;
                                    break;
                                case 2:
                                    velocity -= 2f;
                                    break;
                                case 1:
                                    velocity -= 3f;
                                    break;
                                default:
                                    break;
                            }

                            npc.velocity = Vector2.Lerp(npc.velocity, vector178 * Math.Min(velocity, vector177.Length()), 0.1f);
                        }
                    }
                    else
                    {
                        num1180 = 3;

                        Vector2 vector179 = value12 - npc.Center;
                        if (vector179 != Vector2.Zero)
                        {
                            Vector2 vector180 = vector179;
                            vector180.Normalize();

                            float velocity = death ? 11f : 10f;
                            switch (aggressionLevel)
                            {
                                case 6:
                                    velocity += 4f;
                                    break;
                                case 5:
                                    velocity += 2f;
                                    break;
                                case 4:
                                    break;
                                case 3:
                                    velocity -= 0.5f;
                                    break;
                                case 2:
                                    velocity -= 1f;
                                    break;
                                case 1:
                                    velocity -= 1.5f;
                                    break;
                                default:
                                    break;
                            }

                            npc.velocity = Vector2.SmoothStep(npc.velocity, vector180 * Math.Min(velocity, vector179.Length()), 0.2f);
                        }
                    }
                }

                // Phantasmal Bolts
                else if (npc.ai[0] == 3f)
                {
                    if (num1178 == 0f)
                    {
                        npc.TargetClosest(false);
                        npc.netUpdate = true;
                    }

                    Vector2 v = Main.player[npc.target].Center - npc.Center;
                    bool shootFirstBolt = num1178 == num1179 - 14f;
                    bool shootSecondBolt = num1178 == num1179 - 7f;
                    bool shootThirdBolt = num1178 == num1179;
                    switch (aggressionLevel)
                    {
                        case 6:
                            v = Main.player[npc.target].Center + Main.player[npc.target].velocity * 30f - npc.Center;
                            break;
                        case 5:
                            v = Main.player[npc.target].Center + Main.player[npc.target].velocity * 20f - npc.Center;
                            break;
                        case 4:
                            break;
                        case 3:
                        case 2:
                            shootSecondBolt = false;
                            break;
                        case 1:
                            shootSecondBolt = false;
                            shootThirdBolt = false;
                            break;
                        default:
                            break;
                    }

                    npc.localAI[0] = npc.localAI[0].AngleLerp(v.ToRotation(), 0.5f);

                    npc.localAI[1] += 0.05f;
                    if (npc.localAI[1] > 1f)
                        npc.localAI[1] = 1f;

                    if (num1178 == num1179 - 35f)
                        SoundEngine.PlaySound(SoundID.NPCDeath6, npc.position);

                    if ((shootFirstBolt || shootSecondBolt || shootThirdBolt) && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 vector181 = Utils.Vector2FromElipse(npc.localAI[0].ToRotationVector2(), vector165 * npc.localAI[1]);

                        float velocity = death ? 6.75f : 6.25f;
                        switch (aggressionLevel)
                        {
                            case 6:
                            case 5:
                            case 4:
                                break;
                            case 3:
                                velocity -= 0.25f;
                                break;
                            case 2:
                                velocity -= 0.5f;
                                break;
                            case 1:
                                velocity -= 0.75f;
                                break;
                            default:
                                break;
                        }

                        Vector2 vector182 = Vector2.Normalize(v) * velocity;
                        int type = ProjectileID.PhantasmalBolt;
                        int damage = npc.GetProjectileDamage(type);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + vector181.X, npc.Center.Y + vector181.Y, vector182.X, vector182.Y, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                }

                // Center
                Vector2 center17 = Main.npc[(int)npc.ai[3]].Center;
                Vector2 value14 = new Vector2(220f * num1177, -60f) + center17;
                Vector2 vector183 = value14 + new Vector2(num1177 * 110f, -150f);
                Vector2 vector184 = vector183 + new Vector2(num1177 * 370f, 150f);

                if (vector183.X > vector184.X)
                    Utils.Swap(ref vector183.X, ref vector184.X);
                if (vector183.Y > vector184.Y)
                    Utils.Swap(ref vector183.Y, ref vector184.Y);

                Vector2 value15 = Vector2.Clamp(npc.Center + npc.velocity, vector183, vector184);
                if (value15 != npc.Center + npc.velocity)
                    npc.Center = value15 - npc.velocity;

                // Frames
                int num1195 = num1180 * 7;
                if (num1195 > npc.frameCounter)
                {
                    double num1196 = npc.frameCounter;
                    npc.frameCounter = num1196 + 1.0;
                }
                if (num1195 < npc.frameCounter)
                {
                    double num1196 = npc.frameCounter;
                    npc.frameCounter = num1196 - 1.0;
                }

                if (npc.frameCounter < 0.0)
                    npc.frameCounter = 0.0;
                if (npc.frameCounter > 21.0)
                    npc.frameCounter = 21.0;
            }
            else if (npc.type == NPCID.MoonLordFreeEye)
            {
                if (Main.npc[(int)npc.ai[3]].ai[0] == 2f)
                {
                    npc.HitEffect(0, 9999.0);
                    npc.active = false;
                }

                if (calamityGlobalNPC.newAI[0] == 0f)
                {
                    int eyeCount = NPC.CountNPCS(npc.type);
                    if (eyeCount > 1)
                    {
                        int eyesSynced = 1;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].active)
                            {
                                if (i != npc.whoAmI && Main.npc[i].type == npc.type)
                                {
                                    Main.npc[i].ai[0] = 0f;
                                    Main.npc[i].ai[1] = 0f;
                                    Main.npc[i].ai[2] = 0f;
                                    Main.npc[i].localAI[0] = 0f;
                                    Main.npc[i].localAI[1] = 0f;
                                    Main.npc[i].localAI[2] = 0f;
                                    calamityGlobalNPC.newAI[0] = 1f;
                                    calamityGlobalNPC.newAI[1] = 0f;
                                    npc.netUpdate = true;

                                    eyesSynced++;
                                    if (eyesSynced >= eyeCount)
                                        break;
                                }
                            }
                        }
                    }
                    else
                        calamityGlobalNPC.newAI[0] = 1f;
                }

                if (Main.rand.NextBool(420))
                    SoundEngine.PlaySound(Main.rand.NextBool() ? SoundID.Zombie100 : SoundID.Zombie101, npc.Center);

                Vector2 value22 = new Vector2(30f);

                if (!Main.npc[(int)npc.ai[3]].active || Main.npc[(int)npc.ai[3]].type != NPCID.MoonLordCore)
                {
                    npc.life = 0;
                    npc.HitEffect(0, 10.0);
                    npc.active = false;
                }

                float num1241 = 0f;
                float num1242 = npc.ai[0];

                npc.ai[1] += 1f;

                int num1243 = 0;
                int num1244 = 0;
                while (num1243 < 10)
                {
                    num1241 = NPC.MoonLordAttacksArray2[1, num1243];
                    if (num1241 + num1244 > npc.ai[1])
                        break;

                    num1244 += (int)num1241;
                    num1243 += 1;
                }

                if (num1243 == 10)
                {
                    num1243 = 0;
                    npc.ai[1] = 0f;
                    num1241 = NPC.MoonLordAttacksArray2[1, num1243];
                    num1244 = 0;
                }

                npc.ai[0] = NPC.MoonLordAttacksArray2[0, num1243];
                float num1245 = (int)npc.ai[1] - num1244;

                if (npc.ai[0] != num1242)
                    npc.netUpdate = true;

                if (npc.ai[0] == -1f)
                {
                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 180f)
                        npc.ai[1] = 0f;

                    float value23;
                    if (npc.ai[1] < 60f)
                    {
                        value23 = 0.75f;

                        npc.localAI[0] = 0f;

                        npc.localAI[1] = (float)Math.Sin(npc.ai[1] * MathHelper.TwoPi / 15f) * 0.35f;
                        if (npc.localAI[1] < 0f)
                            npc.localAI[0] = MathHelper.Pi;
                    }
                    else if (npc.ai[1] < 120f)
                    {
                        value23 = 1f;

                        if (npc.localAI[1] < 0.5f)
                            npc.localAI[1] += 0.025f;

                        npc.localAI[0] += 0.209439516f;
                    }
                    else
                    {
                        value23 = 1.15f;

                        npc.localAI[1] -= 0.05f;
                        if (npc.localAI[1] < 0f)
                            npc.localAI[1] = 0f;
                    }

                    npc.localAI[2] = MathHelper.Lerp(npc.localAI[2], value23, 0.3f);
                }

                if (npc.ai[0] == 0f)
                {
                    npc.TargetClosest(false);

                    Vector2 v7 = Main.player[npc.target].Center - npc.Center;

                    npc.localAI[0] = npc.localAI[0].AngleLerp(v7.ToRotation(), 0.5f);

                    npc.localAI[1] += 0.05f;
                    if (npc.localAI[1] > 0.7f)
                        npc.localAI[1] = 0.7f;

                    npc.localAI[2] = MathHelper.Lerp(npc.localAI[2], 1f, 0.2f);

                    float velocity = death ? 38f : 36f;
                    Vector2 center23 = npc.Center;
                    Vector2 center24 = Main.player[npc.target].Center;
                    Vector2 value24 = center24 - center23;
                    value24 = Vector2.Normalize(value24) * velocity;

                    if (Vector2.Distance(center23, center24) > 300f)
                    {
                        int num1246 = 30;
                        npc.velocity.X = (npc.velocity.X * (num1246 - 1) + value24.X) / num1246;
                        npc.velocity.Y = (npc.velocity.Y * (num1246 - 1) + value24.Y) / num1246;
                    }
                    else
                    {
                        npc.velocity *= 0.8f;
                        if (npc.velocity.Length() < 1f)
                            npc.velocity = Vector2.Zero;
                    }

                    // Fly towards Moon Lord Head and stay away from other True Eyes
                    float num1247 = 0.5f;
                    for (int num1248 = 0; num1248 < Main.maxNPCs; num1248++)
                    {
                        if (Main.npc[num1248].active)
                        {
                            if (num1248 != npc.whoAmI && Main.npc[num1248].type == npc.type)
                            {
                                if (Vector2.Distance(npc.Center, Main.npc[num1248].Center) < 150f)
                                {
                                    if (npc.position.X < Main.npc[num1248].position.X)
                                        npc.velocity.X = npc.velocity.X - num1247;
                                    else
                                        npc.velocity.X = npc.velocity.X + num1247;

                                    if (npc.position.Y < Main.npc[num1248].position.Y)
                                        npc.velocity.Y = npc.velocity.Y - num1247;
                                    else
                                        npc.velocity.Y = npc.velocity.Y + num1247;
                                }
                            }
                        }
                    }
                    return false;
                }

                if (npc.ai[0] == 1f)
                {
                    if (num1245 == 0f)
                    {
                        npc.TargetClosest(false);
                        npc.netUpdate = true;
                    }

                    npc.velocity *= 0.95f;
                    if (npc.velocity.Length() < 1f)
                        npc.velocity = Vector2.Zero;

                    Vector2 v8 = Main.player[npc.target].Center - npc.Center;

                    npc.localAI[0] = npc.localAI[0].AngleLerp(v8.ToRotation(), 0.5f);

                    npc.localAI[1] += 0.05f;
                    if (npc.localAI[1] > 1f)
                        npc.localAI[1] = 1f;

                    if (num1245 < 20f)
                        npc.localAI[2] = MathHelper.Lerp(npc.localAI[2], 1.1f, 0.2f);
                    else
                        npc.localAI[2] = MathHelper.Lerp(npc.localAI[2], 0.4f, 0.2f);

                    if (num1245 == num1241 - 35f)
                        SoundEngine.PlaySound(SoundID.NPCDeath6, npc.position);

                    if ((num1245 == num1241 - 14f || num1245 == num1241 - 7f || num1245 == num1241) && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 vector214 = Utils.Vector2FromElipse(npc.localAI[0].ToRotationVector2(), value22 * npc.localAI[1]);
                        float velocity = death ? 9f : 8f;
                        Vector2 vector215 = Vector2.Normalize(v8) * velocity;
                        int type = ProjectileID.PhantasmalBolt;
                        int damage = npc.GetProjectileDamage(type);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + vector214.X, npc.Center.Y + vector214.Y, vector215.X, vector215.Y, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
                else if (npc.ai[0] == 2f || npc.ai[0] == 4f)
                {
                    int type = ProjectileID.PhantasmalSphere;
                    int damage = npc.GetProjectileDamage(type);

                    if (num1245 < 15f)
                    {
                        npc.localAI[1] -= 0.07f;
                        if (npc.localAI[1] < 0f)
                            npc.localAI[1] = 0f;

                        npc.localAI[2] = MathHelper.Lerp(npc.localAI[2], 0.4f, 0.2f);

                        npc.velocity *= 0.8f;
                        if (npc.velocity.Length() < 1f)
                            npc.velocity = Vector2.Zero;
                    }
                    else if (num1245 < 75f)
                    {
                        float num1249 = (num1245 - 15f) / 10f;
                        int num1250 = 0;
                        int num1251 = 0;
                        switch ((int)num1249)
                        {
                            case 0:
                                num1250 = 0;
                                num1251 = 2;
                                break;
                            case 1:
                                num1250 = 2;
                                num1251 = 5;
                                break;
                            case 2:
                                num1250 = 5;
                                num1251 = 3;
                                break;
                            case 3:
                                num1250 = 3;
                                num1251 = 1;
                                break;
                            case 4:
                                num1250 = 1;
                                num1251 = 4;
                                break;
                            case 5:
                                num1250 = 4;
                                num1251 = 0;
                                break;
                        }

                        Vector2 spinningpoint2 = Vector2.UnitY * -30f;
                        Vector2 value25 = spinningpoint2.RotatedBy(num1250 * MathHelper.TwoPi / 6f);
                        Vector2 value26 = spinningpoint2.RotatedBy(num1251 * MathHelper.TwoPi / 6f);
                        Vector2 vector216 = Vector2.Lerp(value25, value26, num1249 - (int)num1249);
                        float value27 = vector216.Length() / 30f;

                        npc.localAI[0] = vector216.ToRotation();
                        npc.localAI[1] = MathHelper.Lerp(npc.localAI[1], value27, 0.5f);

                        for (int num1252 = 0; num1252 < 2; num1252++)
                        {
                            int num1253 = Dust.NewDust(npc.Center + vector216 - Vector2.One * 4f, 0, 0, 229, 0f, 0f, 0, default, 1f);
                            Dust dust = Main.dust[num1253];
                            dust.velocity += vector216 / 15f;
                            Main.dust[num1253].noGravity = true;
                        }

                        if ((num1245 - 15f) % 10f == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 vector217 = Vector2.Normalize(vector216);
                            if (vector217.HasNaNs())
                                vector217 = Vector2.UnitY * -1f;

                            float spreadVelocity = death ? 4.5f : 4f;
                            vector217 *= 4f;
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + vector216.X, npc.Center.Y + vector216.Y, vector217.X, vector217.Y, type, 0, 0f, Main.myPlayer, 30f, npc.whoAmI);
                            Main.projectile[proj].timeLeft = 1200;

                            if (CalamityWorld.LegendaryMode)
                            {
                                for (int num1268 = 0; num1268 < 3; num1268++)
                                {
                                    if (!WorldGen.SolidTile((int)(npc.Center.X / 16f), (int)(npc.Center.Y / 16f)))
                                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y, (float)Main.rand.Next(-1599, 1600) * 0.01f, (float)Main.rand.Next(-1599, 1) * 0.01f, ProjectileID.MoonBoulder, 70, 10f);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (num1245 < 105f)
                        {
                            npc.localAI[0] = npc.localAI[0].AngleLerp(npc.ai[2] - MathHelper.PiOver2, 0.2f);

                            npc.localAI[2] = MathHelper.Lerp(npc.localAI[2], 0.75f, 0.2f);

                            if (num1245 == 75f)
                            {
                                npc.TargetClosest(false);

                                npc.netUpdate = true;

                                npc.velocity = Vector2.UnitY * -7f;

                                for (int num1255 = 0; num1255 < Main.maxProjectiles; num1255++)
                                {
                                    Projectile projectile7 = Main.projectile[num1255];
                                    if (projectile7.active && projectile7.type == type && projectile7.ai[1] == npc.whoAmI && projectile7.ai[0] != -1f)
                                    {
                                        Projectile projectile8 = projectile7;
                                        projectile8.velocity += npc.velocity;
                                        projectile7.netUpdate = true;
                                    }
                                }
                            }

                            npc.velocity.Y = npc.velocity.Y * 0.96f;

                            npc.ai[2] = (Main.player[npc.target].Center - npc.Center).ToRotation() + MathHelper.PiOver2;

                            npc.rotation = npc.rotation.AngleTowards(npc.ai[2], 0.104719758f);

                            return false;
                        }

                        if (num1245 < 120f)
                        {
                            SoundEngine.PlaySound(SoundID.Zombie102, npc.Center);

                            if (num1245 == 105f)
                                npc.netUpdate = true;

                            float velocity = death ? 13.25f : 12f;
                            Vector2 velocity6 = (npc.ai[2] - MathHelper.PiOver2).ToRotationVector2() * velocity;
                            npc.velocity = velocity6 * 2f;

                            for (int num1256 = 0; num1256 < Main.maxProjectiles; num1256++)
                            {
                                Projectile projectile9 = Main.projectile[num1256];
                                if (projectile9.active && projectile9.type == type && projectile9.ai[1] == npc.whoAmI && projectile9.ai[0] != -1f)
                                {
                                    projectile9.ai[0] = -1f;
                                    projectile9.damage = damage;
                                    projectile9.velocity = velocity6;
                                    projectile9.netUpdate = true;
                                }
                            }

                            return false;
                        }

                        npc.velocity *= 0.92f;
                        npc.rotation = npc.rotation.AngleLerp(0f, 0.2f);
                    }
                }
                else if (npc.ai[0] == 3f)
                {
                    if (num1245 < 15f)
                    {
                        npc.localAI[1] -= 0.07f;
                        if (npc.localAI[1] < 0f)
                            npc.localAI[1] = 0f;

                        npc.localAI[2] = MathHelper.Lerp(npc.localAI[2], 0.4f, 0.2f);

                        npc.velocity *= 0.9f;
                        if (npc.velocity.Length() < 1f)
                            npc.velocity = Vector2.Zero;
                    }
                    else if (num1245 < 45f)
                    {
                        npc.localAI[0] = 0f;

                        npc.localAI[1] = (float)Math.Sin((num1245 - 15f) * MathHelper.TwoPi / 15f) * 0.5f;
                        if (npc.localAI[1] < 0f)
                            npc.localAI[0] = MathHelper.Pi;
                    }
                    else
                    {
                        if (num1245 >= 185f)
                        {
                            npc.velocity *= 0.88f;

                            npc.rotation = npc.rotation.AngleLerp(0f, 0.2f);

                            npc.localAI[1] -= 0.07f;
                            if (npc.localAI[1] < 0f)
                                npc.localAI[1] = 0f;

                            npc.localAI[2] = MathHelper.Lerp(npc.localAI[2], 1f, 0.2f);
                            return false;
                        }

                        if (num1245 == 45f)
                        {
                            npc.ai[2] = Main.rand.NextBool().ToDirectionInt() * MathHelper.TwoPi / 40f;
                            npc.netUpdate = true;
                        }

                        if ((num1245 - 15f - 30f) % 40f == 0f)
                            npc.ai[2] *= 0.95f;

                        npc.localAI[0] += npc.ai[2];

                        npc.localAI[1] += 0.05f;
                        if (npc.localAI[1] > 1f)
                            npc.localAI[1] = 1f;

                        Vector2 vector218 = npc.localAI[0].ToRotationVector2() * value22 * npc.localAI[1];
                        float scaleFactor10 = MathHelper.Lerp(8f, 20f, (num1245 - 15f - 30f) / 140f);

                        npc.velocity = Vector2.Normalize(vector218) * scaleFactor10;
                        npc.rotation = npc.rotation.AngleLerp(npc.velocity.ToRotation() + MathHelper.PiOver2, 0.2f);

                        if ((num1245 - 15f - 30f) % 10f == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 vector219 = npc.Center + Vector2.Normalize(vector218) * value22.Length() * 0.4f;
                            float velocity = death ? 6f : 5f;
                            Vector2 vector220 = Vector2.Normalize(vector218) * velocity;
                            float ai3 = (MathHelper.TwoPi * (float)Main.rand.NextDouble() - MathHelper.Pi) / 30f + 0.0174532924f * npc.ai[2];
                            int type = ProjectileID.PhantasmalEye;
                            int damage = npc.GetProjectileDamage(type);
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), vector219, vector220, type, damage, 0f, Main.myPlayer, 0f, ai3);
                            Main.projectile[proj].timeLeft = 1200;
                        }
                    }
                }
                else if (npc.ai[0] == 4f)
                {
                    if (num1245 == 0f)
                    {
                        npc.TargetClosest(false);
                        npc.netUpdate = true;
                    }

                    if (num1245 < 180f)
                    {
                        npc.localAI[2] = MathHelper.Lerp(npc.localAI[2], 1f, 0.2f);

                        npc.localAI[1] -= 0.05f;
                        if (npc.localAI[1] < 0f)
                            npc.localAI[1] = 0f;

                        npc.velocity *= 0.95f;
                        if (npc.velocity.Length() < 1f)
                            npc.velocity = Vector2.Zero;

                        if (num1245 >= 60f)
                        {
                            Vector2 center25 = npc.Center;

                            int num1257 = 0;
                            if (num1245 >= 120f)
                                num1257 = 1;

                            for (int num1258 = 0; num1258 < 1 + num1257; num1258++)
                            {
                                int num1259 = 229;
                                float num1260 = 0.8f;
                                if (num1258 % 2 == 1)
                                {
                                    num1259 = 229;
                                    num1260 = 1.65f;
                                }

                                Vector2 vector221 = center25 + ((float)Main.rand.NextDouble() * MathHelper.TwoPi).ToRotationVector2() * value22 / 2f;
                                int num1261 = Dust.NewDust(vector221 - Vector2.One * 8f, 16, 16, num1259, npc.velocity.X / 2f, npc.velocity.Y / 2f, 0, default, 1f);
                                Main.dust[num1261].velocity = Vector2.Normalize(center25 - vector221) * 3.5f * (10f - num1257 * 2f) / 10f;
                                Main.dust[num1261].noGravity = true;
                                Main.dust[num1261].scale = num1260;
                                Main.dust[num1261].customData = npc;
                            }
                        }
                    }
                    else
                    {
                        if (num1245 < num1241 - 15f)
                        {
                            if (calamityGlobalNPC.newAI[1] == 0f)
                                calamityGlobalNPC.newAI[1] = 600f;

                            if (num1245 == 180f && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                // If head is in deathray phase
                                if (Main.npc[(int)Main.npc[(int)npc.ai[3]].localAI[2]].ai[0] == 1f)
                                    calamityGlobalNPC.newAI[1] *= 1.5f;

                                npc.TargetClosest(false);

                                Vector2 vector222 = Main.player[npc.target].Center - npc.Center;
                                vector222.Normalize();

                                float num1262 = -1f;
                                if (vector222.X < 0f)
                                    num1262 = 1f;

                                vector222 = vector222.RotatedBy(-(double)num1262 * MathHelper.TwoPi / 6f);
                                int type = ProjectileID.PhantasmalDeathray;
                                int damage = npc.GetProjectileDamage(type);
                                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y, vector222.X, vector222.Y, type, damage, 0f, Main.myPlayer, num1262 * MathHelper.TwoPi / calamityGlobalNPC.newAI[1], npc.whoAmI);
                                npc.ai[2] = (vector222.ToRotation() + MathHelper.Pi + MathHelper.TwoPi) * num1262;
                                npc.netUpdate = true;
                            }

                            npc.localAI[1] += 0.05f;
                            if (npc.localAI[1] > 1f)
                                npc.localAI[1] = 1f;

                            float num1263 = (npc.ai[2] >= 0f).ToDirectionInt();
                            float num1264 = npc.ai[2];
                            if (num1264 < 0f)
                                num1264 *= -1f;

                            num1264 += -(MathHelper.Pi + MathHelper.TwoPi);
                            num1264 += num1263 * MathHelper.TwoPi / calamityGlobalNPC.newAI[1];

                            npc.localAI[0] = num1264;
                            npc.ai[2] = (num1264 + MathHelper.Pi + MathHelper.TwoPi) * num1263;

                            return false;
                        }

                        calamityGlobalNPC.newAI[1] = 0f;

                        npc.localAI[1] -= 0.07f;
                        if (npc.localAI[1] < 0f)
                            npc.localAI[1] = 0f;
                    }
                }
            }
            else if (npc.type == NPCID.MoonLordLeechBlob)
            {
                // Variables
                float num1265 = 180f;
                Vector2 value28 = new Vector2(0f, 216f);
                int num1266 = (int)Math.Abs(npc.ai[0]) - 1;
                int num1267 = (int)npc.ai[1];

                // Despawn
                if (!Main.npc[num1266].active || Main.npc[num1266].type != NPCID.MoonLordHead)
                {
                    npc.life = 0;
                    npc.HitEffect(0, 10.0);
                    npc.active = false;
                    return false;
                }

                // Heal the Moon Lord
                npc.ai[2] += 1f;
                if (npc.ai[2] >= num1265)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int num1268 = (int)Main.npc[num1266].ai[3];
                        int num1269 = -1;
                        int num1270 = -1;
                        int num1271 = num1266;

                        for (int num1272 = 0; num1272 < Main.maxNPCs; num1272++)
                        {
                            if (Main.npc[num1272].active && Main.npc[num1272].ai[3] == num1268)
                            {
                                if (num1269 == -1 && Main.npc[num1272].type == NPCID.MoonLordHand && Main.npc[num1272].ai[2] == 0f)
                                    num1269 = num1272;
                                if (num1270 == -1 && Main.npc[num1272].type == NPCID.MoonLordHand && Main.npc[num1272].ai[2] == 1f)
                                    num1270 = num1272;
                                if (num1269 != -1 && num1270 != -1 && num1271 != -1)
                                    break;
                            }
                        }

                        // Heal limits
                        int num1273 = death ? 1500 : 1250;
                        int num1274 = Main.npc[num1268].lifeMax - Main.npc[num1268].life;
                        int num1275 = Main.npc[num1269].lifeMax - Main.npc[num1269].life;
                        int num1276 = Main.npc[num1270].lifeMax - Main.npc[num1270].life;
                        int num1277 = Main.npc[num1271].lifeMax - Main.npc[num1271].life;

                        // Healing
                        if (num1277 > 0 && num1273 > 0)
                        {
                            int num1278 = num1277 - num1273;
                            if (num1278 > 0)
                            {
                                num1278 = 0;
                            }
                            int num1279 = num1273 + num1278;
                            num1273 -= num1279;
                            NPC nPC6 = Main.npc[num1271];
                            nPC6.life += num1279;
                            NPC.HealEffect(Utils.CenteredRectangle(Main.npc[num1271].Center, new Vector2(50f)), num1279, true);
                        }
                        if (num1274 > 0 && num1273 > 0)
                        {
                            int num1280 = num1274 - num1273;
                            if (num1280 > 0)
                            {
                                num1280 = 0;
                            }
                            int num1281 = num1273 + num1280;
                            num1273 -= num1281;
                            NPC nPC6 = Main.npc[num1268];
                            nPC6.life += num1281;
                            NPC.HealEffect(Utils.CenteredRectangle(Main.npc[num1268].Center, new Vector2(50f)), num1281, true);
                        }
                        if (num1275 > 0 && num1273 > 0)
                        {
                            int num1282 = num1275 - num1273;
                            if (num1282 > 0)
                            {
                                num1282 = 0;
                            }
                            int num1283 = num1273 + num1282;
                            num1273 -= num1283;
                            NPC nPC6 = Main.npc[num1269];
                            nPC6.life += num1283;
                            NPC.HealEffect(Utils.CenteredRectangle(Main.npc[num1269].Center, new Vector2(50f)), num1283, true);
                        }
                        if (num1276 > 0 && num1273 > 0)
                        {
                            int num1284 = num1276 - num1273;
                            if (num1284 > 0)
                            {
                                num1284 = 0;
                            }
                            int num1285 = num1273 + num1284;
                            NPC nPC6 = Main.npc[num1270];
                            nPC6.life += num1285;
                            NPC.HealEffect(Utils.CenteredRectangle(Main.npc[num1270].Center, new Vector2(50f)), num1285, true);
                        }
                    }

                    // Die
                    npc.life = 0;
                    npc.HitEffect(0, 10.0);
                    npc.active = false;
                    return false;
                }

                // Move towards the Moon Lord mouth
                npc.velocity = Vector2.Zero;
                npc.Center = Vector2.Lerp(Main.projectile[num1267].Center, Main.npc[(int)Math.Abs(npc.ai[0]) - 1].Center + value28, npc.ai[2] / num1265);

                // Emit dust
                Vector2 spinningpoint3 = Vector2.UnitY * -npc.height / 2f;
                for (int num1286 = 0; num1286 < 4; num1286++)
                {
                    int num1287 = Dust.NewDust(npc.Center - Vector2.One * 4f + spinningpoint3.RotatedBy(num1286 * MathHelper.TwoPi / 6f), 0, 0, 229, 0f, 0f, 0, default, 1f);
                    Main.dust[num1287].velocity = -Vector2.UnitY;
                    Main.dust[num1287].noGravity = true;
                    Main.dust[num1287].scale = 0.7f;
                    Main.dust[num1287].customData = npc;
                }

                spinningpoint3 = Vector2.UnitY * -npc.height / 6f;
                for (int num1288 = 0; num1288 < 2; num1288++)
                {
                    int num1289 = Dust.NewDust(npc.Center - Vector2.One * 4f + spinningpoint3.RotatedBy(num1288 * MathHelper.TwoPi / 6f), 0, 0, 229, 0f, -2f, 0, default, 1f);
                    Main.dust[num1289].noGravity = true;
                    Main.dust[num1289].scale = 1.5f;
                    Main.dust[num1289].customData = npc;
                }
            }
            return false;
        }
    }
}
