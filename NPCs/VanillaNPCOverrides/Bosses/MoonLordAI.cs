using System;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCOverrides.Bosses
{
    // This is the most horrible abomination of code I have ever seen in my life, by a fucking landslide.
    // This took hours of my time to attempt to clean up, and quite frankly I'd be surprised if even half of it is accurate.
    // I do not want to look at this crime against humanity for a very long time. -CIT
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
                            int handsSpawned = 0;
                            int totalSpawns;

                            for (int i = 0; i < 2; i = totalSpawns + 1)
                            {
                                int handSpawn = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X + i * 800 - 400, (int)npc.Center.Y - 100, NPCID.MoonLordHand, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                                Main.npc[handSpawn].ai[2] = i;
                                Main.npc[handSpawn].netUpdate = true;
                                int[] arg_381A6_0 = array5;
                                totalSpawns = handsSpawned;
                                handsSpawned = totalSpawns + 1;
                                arg_381A6_0[totalSpawns] = handSpawn;
                                totalSpawns = i;
                            }

                            int headSpawn = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y - 400, NPCID.MoonLordHead, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                            Main.npc[headSpawn].netUpdate = true;
                            int[] arg_3823F_0 = array5;
                            totalSpawns = handsSpawned;
                            handsSpawned = totalSpawns + 1;
                            arg_3823F_0[totalSpawns] = headSpawn;

                            for (int j = 0; j < 3; j = totalSpawns + 1)
                            {
                                Main.npc[array5[j]].ai[3] = npc.whoAmI;
                                totalSpawns = j;
                            }
                            for (int k = 0; k < 3; k = totalSpawns + 1)
                            {
                                npc.localAI[k] = array5[k];
                                totalSpawns = k;
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
                        int totalSpawns = NPC.NewNPC(npc.GetSource_FromAI(), (int)Main.npc[(int)npc.localAI[2]].Center.X, (int)Main.npc[(int)npc.localAI[2]].Center.Y, NPCID.MoonLordFreeEye);
                        Main.npc[totalSpawns].ai[3] = npc.whoAmI;
                        Main.npc[totalSpawns].netUpdate = true;
                    }
                }

                // Fly near target, don't take damage
                if (npc.ai[0] == 0f)
                {
                    npc.dontTakeDamage = true;
                    npc.TargetClosest(false);

                    Vector2 targetDistance = Main.player[npc.target].Center - npc.Center + new Vector2(0f, 130f);
                    if (targetDistance.Length() > 20f)
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

                        Vector2 desiredVelocity = Vector2.Normalize(targetDistance - npc.velocity) * velocity;
                        Vector2 velocity2 = npc.velocity;
                        npc.SimpleFlyMovement(desiredVelocity, 0.5f);
                        npc.velocity = Vector2.Lerp(npc.velocity, velocity2, 0.5f);
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        // Despawn if other parts aren't there
                        bool shouldDespawn = false;
                        if (npc.localAI[0] < 0f || npc.localAI[1] < 0f || npc.localAI[2] < 0f)
                            shouldDespawn = true;
                        else if (!Main.npc[(int)npc.localAI[0]].active || Main.npc[(int)npc.localAI[0]].type != NPCID.MoonLordHand)
                            shouldDespawn = true;
                        else if (!Main.npc[(int)npc.localAI[1]].active || Main.npc[(int)npc.localAI[1]].type != NPCID.MoonLordHand)
                            shouldDespawn = true;
                        else if (!Main.npc[(int)npc.localAI[2]].active || Main.npc[(int)npc.localAI[2]].type != NPCID.MoonLordHead)
                            shouldDespawn = true;

                        if (shouldDespawn)
                        {
                            npc.life = 0;
                            npc.HitEffect(0, 10.0);
                            npc.active = false;
                        }

                        // Take damage if other parts are down
                        bool coreIsOpen = true;
                        if (Main.npc[(int)npc.localAI[0]].Calamity().newAI[0] != 1f)
                            coreIsOpen = false;
                        if (Main.npc[(int)npc.localAI[1]].Calamity().newAI[0] != 1f)
                            coreIsOpen = false;
                        if (Main.npc[(int)npc.localAI[2]].Calamity().newAI[0] != 1f)
                            coreIsOpen = false;

                        if (coreIsOpen)
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

                    Vector2 targetDistanceVulnerable = Main.player[npc.target].Center - npc.Center + new Vector2(0f, 130f);
                    if (targetDistanceVulnerable.Length() > 20f)
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

                        Vector2 desiredVelocity2 = Vector2.Normalize(targetDistanceVulnerable - npc.velocity) * velocity;
                        Vector2 currentVelocity = npc.velocity;
                        npc.SimpleFlyMovement(desiredVelocity2, 0.5f);
                        npc.velocity = Vector2.Lerp(npc.velocity, currentVelocity, 0.5f);
                    }
                }

                // Death effects
                else if (npc.ai[0] == 2f)
                {
                    npc.dontTakeDamage = true;
                    npc.Calamity().ShouldCloseHPBar = true;
                    Vector2 dyingVelocity = new Vector2(npc.direction, -0.5f);
                    npc.velocity = Vector2.Lerp(npc.velocity, dyingVelocity, 0.98f);

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
                        Vector2 randPositionOffset = Utils.RandomVector2(Main.rand, -1f, 1f);
                        if (randPositionOffset != Vector2.Zero)
                            randPositionOffset.Normalize();

                        randPositionOffset *= 20f + Main.rand.NextFloat() * 400f;
                        Vector2 npcPosition = npc.Center + randPositionOffset;
                        Point npcPositionTileCoords = npcPosition.ToTileCoordinates();

                        bool inOpenSpace = true;
                        if (!WorldGen.InWorld(npcPositionTileCoords.X, npcPositionTileCoords.Y, 0))
                            inOpenSpace = false;
                        if (inOpenSpace && WorldGen.SolidTile(npcPositionTileCoords.X, npcPositionTileCoords.Y))
                            inOpenSpace = false;

                        float randDustAmt = Main.rand.Next(6, 19);
                        float twoPiOverRand = MathHelper.TwoPi * Main.rand.NextFloat();
                        float dustVelocityMult = 1f + Main.rand.NextFloat() * 2f;
                        float dustScale = 1f + Main.rand.NextFloat();
                        float fadeIn = 0.4f + Main.rand.NextFloat();
                        int dustType = Utils.SelectRandom(Main.rand, new int[]
                        {
                            31,
                            229
                        });

                        if (inOpenSpace)
                        {
                            //MoonlordDeathDrama.AddExplosion(npcPosition);
                            for (float j = 0f; j < randDustAmt * 2f; j = ai1 + 1f)
                            {
                                Dust vortex = Main.dust[Dust.NewDust(npcPosition, 0, 0, 229, 0f, 0f, 0, default, 1f)];
                                vortex.noGravity = true;
                                vortex.position = npcPosition;
                                vortex.velocity = Vector2.UnitY.RotatedBy(twoPiOverRand + (MathHelper.TwoPi / randDustAmt) * j) * dustVelocityMult * (Main.rand.NextFloat() * 1.6f + 1.6f);
                                vortex.fadeIn = fadeIn;
                                vortex.scale = dustScale;
                                ai1 = j;
                            }
                        }

                        for (float k = 0f; k < npc.ai[1] / 60f; k = ai1 + 1f)
                        {
                            Vector2 randPosOffset = Utils.RandomVector2(Main.rand, -1f, 1f);
                            if (randPosOffset != Vector2.Zero)
                                randPosOffset.Normalize();

                            randPosOffset *= 20f + Main.rand.NextFloat() * 800f;
                            Vector2 npcPositioning = npc.Center + randPosOffset;
                            Point npcPositioningTileCoords = npcPositioning.ToTileCoordinates();

                            bool isInOpenness = true;
                            if (!WorldGen.InWorld(npcPositioningTileCoords.X, npcPositioningTileCoords.Y, 0))
                                isInOpenness = false;
                            if (isInOpenness && WorldGen.SolidTile(npcPositioningTileCoords.X, npcPositioningTileCoords.Y))
                                isInOpenness = false;

                            if (isInOpenness)
                            {
                                Dust openDust = Main.dust[Dust.NewDust(npcPositioning, 0, 0, dustType, 0f, 0f, 0, default, 1f)];
                                openDust.noGravity = true;
                                openDust.position = npcPositioning;
                                openDust.velocity = -Vector2.UnitY * dustVelocityMult * (Main.rand.NextFloat() * 0.9f + 1.6f);
                                openDust.fadeIn = fadeIn;
                                openDust.scale = dustScale;
                            }

                            ai1 = k;
                        }
                    }

                    if (npc.ai[1] % 15f == 0f && npc.ai[1] < 480f && npc.ai[1] >= 90f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 randomOffset = Utils.RandomVector2(Main.rand, -1f, 1f);
                        if (randomOffset != Vector2.Zero)
                            randomOffset.Normalize();

                        randomOffset *= 20f + Main.rand.NextFloat() * 400f;
                        bool stillInTheOpen = true;
                        Vector2 npcOffsetPos = npc.Center + randomOffset;
                        Point npcOffsetPosTileCoords = npcOffsetPos.ToTileCoordinates();

                        if (!WorldGen.InWorld(npcOffsetPosTileCoords.X, npcOffsetPosTileCoords.Y, 0))
                            stillInTheOpen = false;
                        if (stillInTheOpen && WorldGen.SolidTile(npcOffsetPosTileCoords.X, npcOffsetPosTileCoords.Y))
                            stillInTheOpen = false;

                        if (stillInTheOpen)
                        {
                            float smokeRotation = (Main.rand.Next(4) < 2).ToDirectionInt() * (0.3926991f + MathHelper.PiOver4 * Main.rand.NextFloat());
                            Vector2 smokePosition = new Vector2(0f, -Main.rand.NextFloat() * 0.5f - 0.5f).RotatedBy(smokeRotation) * 6f;
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npcOffsetPos.X, npcOffsetPos.Y, smokePosition.X, smokePosition.Y, ProjectileID.BlowupSmokeMoonlord, 0, 0f, Main.myPlayer, 0f, 0f);
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

                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC nPC5 = Main.npc[i];
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
                    Vector2 despawnVelocityLerp = new Vector2(npc.direction, -0.5f);
                    npc.velocity = Vector2.Lerp(npc.velocity, despawnVelocityLerp, 0.98f);

                    npc.ai[1] += 1f;
                    if (npc.ai[1] < 60f)
                        MoonlordDeathDrama.RequestLight(npc.ai[1] / 40f, npc.Center);

                    if (npc.ai[1] == 40f)
                    {
                        for (int j = 0; j < Main.maxProjectiles; j++)
                        {
                            Projectile projectile2 = Main.projectile[j];
                            if (projectile2.active && (projectile2.type == ProjectileID.MoonLeech || projectile2.type == ProjectileID.PhantasmalBolt ||
                                projectile2.type == ProjectileID.PhantasmalDeathray || projectile2.type == ProjectileID.PhantasmalEye ||
                                projectile2.type == ProjectileID.PhantasmalSphere))
                            {
                                projectile2.active = false;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, j, 0f, 0f, 0f, 0, 0, 0);
                            }
                        }

                        for (int k = 0; k < Main.maxNPCs; k++)
                        {
                            NPC nPC4 = Main.npc[k];
                            if (nPC4.active && nPC4.type == NPCID.MoonLordFreeEye)
                            {
                                nPC4.active = false;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, nPC4.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                            }
                        }

                        for (int l = 0; l < Main.maxGore; l++)
                        {
                            Gore gore2 = Main.gore[l];
                            if (gore2.active && gore2.type >= GoreID.MoonLordHeart1 && gore2.type <= GoreID.MoonLordHeart4)
                                gore2.active = false;
                        }
                    }

                    if (npc.ai[1] >= 60f)
                    {
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC nPC5 = Main.npc[i];
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
                bool preventDespawn = false;
                if (npc.ai[0] == -2f || npc.ai[0] == -1f || npc.ai[0] == -2f || npc.ai[0] == 3f)
                    preventDespawn = true;
                if (Main.player[npc.target].active && !Main.player[npc.target].dead)
                    preventDespawn = true;

                if (!preventDespawn)
                {
                    for (int p = 0; p < Main.maxPlayers; p++)
                    {
                        if (Main.player[p].active && !Main.player[p].dead)
                        {
                            preventDespawn = true;
                            break;
                        }
                    }
                }
                if (!preventDespawn)
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

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC nPC7 = Main.npc[i];
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
                Vector2 boltAndDeathrayVector = new Vector2(27f, 59f);
                float attackTimer = 0f;
                float attackTimerComparison = 0f;
                int headVulnerability = 0;
                int deathrayCheck = 0;

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
                    float ai0CrossCheck = npc.ai[0];
                    npc.ai[1] += 1f;
                    int attackType = (int)Main.npc[(int)npc.ai[3]].ai[2];
                    int fiveFrameCounter = 0;
                    int attackTimerIncrement = 0;

                    while (fiveFrameCounter < 5)
                    {
                        attackTimerComparison = NPC.MoonLordAttacksArray[attackType, 2, 1, fiveFrameCounter];
                        if (attackTimerComparison + attackTimerIncrement > npc.ai[1])
                            break;

                        attackTimerIncrement += (int)attackTimerComparison;
                        int totalSpawns = fiveFrameCounter;
                        fiveFrameCounter = totalSpawns + 1;
                    }

                    if (fiveFrameCounter == 5)
                    {
                        fiveFrameCounter = 0;
                        npc.ai[1] = 0f;
                        attackTimerComparison = NPC.MoonLordAttacksArray[attackType, 2, 1, fiveFrameCounter];
                        attackTimerIncrement = 0;
                    }

                    npc.ai[0] = NPC.MoonLordAttacksArray[attackType, 2, 0, fiveFrameCounter];
                    attackTimer = (int)npc.ai[1] - attackTimerIncrement;

                    if (npc.ai[0] != ai0CrossCheck)
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
                    deathrayCheck = 3;
                    npc.TargetClosest(false);

                    Vector2 v3 = Main.player[npc.target].Center - npc.Center - new Vector2(0f, -22f);
                    float deathrayTravelDist = v3.Length() / 500f;
                    if (deathrayTravelDist > 1f)
                        deathrayTravelDist = 1f;

                    deathrayTravelDist = 1f - deathrayTravelDist;
                    deathrayTravelDist *= 2f;
                    if (deathrayTravelDist > 1f)
                        deathrayTravelDist = 1f;

                    npc.localAI[0] = v3.ToRotation();
                    npc.localAI[1] = deathrayTravelDist;
                    npc.localAI[2] = MathHelper.Lerp(npc.localAI[2], 1f, 0.2f);
                }

                // Deathray
                if (npc.ai[0] == 1f)
                {
                    if (attackTimer < 180f)
                    {
                        npc.localAI[1] -= 0.05f;
                        if (npc.localAI[1] < 0f)
                            npc.localAI[1] = 0f;

                        if (attackTimer >= 60f)
                        {
                            Vector2 center20 = npc.Center;

                            // Hopefully it plays
                            if (attackTimer == 60f)
                                SoundEngine.PlaySound(DeathrayChargeSound, center20);

                            int deathrayAttackDustAmt = 0;
                            if (attackTimer >= 120f)
                                deathrayAttackDustAmt = 1;

                            for (int i = 0; i < 1 + deathrayAttackDustAmt; i++)
                            {
                                float deathrayAttackDustScale = 0.8f;
                                if (i % 2 == 1)
                                    deathrayAttackDustScale = 1.65f;

                                Vector2 deathrayAttackDustRotation = center20 + ((float)Main.rand.NextDouble() * MathHelper.TwoPi).ToRotationVector2() * boltAndDeathrayVector / 2f;
                                int deathrayAttackDust = Dust.NewDust(deathrayAttackDustRotation - Vector2.One * 8f, 16, 16, 229, npc.velocity.X / 2f, npc.velocity.Y / 2f, 0, default, 1f);
                                Main.dust[deathrayAttackDust].velocity = Vector2.Normalize(center20 - deathrayAttackDustRotation) * 3.5f * (10f - deathrayAttackDustAmt * 2f) / 10f;
                                Main.dust[deathrayAttackDust].noGravity = true;
                                Main.dust[deathrayAttackDust].scale = deathrayAttackDustScale;
                                Main.dust[deathrayAttackDust].customData = npc;
                            }
                        }
                    }
                    else if (attackTimer < attackTimerComparison - 15f)
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

                        if (attackTimer == 180f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int projectileType = ProjectileID.PhantasmalDeathray;
                            int damage = npc.GetProjectileDamage(projectileType);

                            npc.TargetClosest(false);
                            Vector2 deathrayRotationSpeed = Main.player[npc.target].Center - npc.Center;
                            deathrayRotationSpeed.Normalize();

                            float deathrayRotationDirection = -1f;
                            if (deathrayRotationSpeed.X < 0f)
                                deathrayRotationDirection = 1f;

                            deathrayRotationSpeed = deathrayRotationSpeed.RotatedBy(-(double)deathrayRotationDirection * MathHelper.TwoPi / 6f);
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y, deathrayRotationSpeed.X, deathrayRotationSpeed.Y, projectileType, damage, 0f, Main.myPlayer, deathrayRotationDirection * MathHelper.TwoPi / calamityGlobalNPC.newAI[1], npc.whoAmI);
                            npc.ai[2] = (deathrayRotationSpeed.ToRotation() + MathHelper.Pi + MathHelper.TwoPi) * deathrayRotationDirection;
                            npc.netUpdate = true;
                        }

                        npc.localAI[1] += 0.05f;
                        if (npc.localAI[1] > 1f)
                            npc.localAI[1] = 1f;

                        float deathrayFaceDirection = (npc.ai[2] >= 0f).ToDirectionInt();
                        float deathrayTimer = npc.ai[2];
                        if (deathrayTimer < 0f)
                            deathrayTimer *= -1f;

                        deathrayTimer += -(MathHelper.Pi + MathHelper.TwoPi);
                        deathrayTimer += deathrayFaceDirection * MathHelper.TwoPi / calamityGlobalNPC.newAI[1];
                        npc.localAI[0] = deathrayTimer;
                        npc.ai[2] = (deathrayTimer + MathHelper.Pi + MathHelper.TwoPi) * deathrayFaceDirection;
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
                                for (int k = 0; k < 30; k++)
                                {
                                    if (!WorldGen.SolidTile((int)(npc.Center.X / 16f), (int)(npc.Center.Y / 16f)))
                                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y, (float)Main.rand.Next(-1599, 1600) * 0.01f, (float)Main.rand.Next(-1599, 1) * 0.01f, ProjectileID.MoonBoulder, 70, 10f);
                                }
                            }
                        }

                        deathrayCheck = 3;
                    }
                }

                // Moon Leech thing
                else if (npc.ai[0] == 2f)
                {
                    headVulnerability = 2;
                    deathrayCheck = 3;
                    Vector2 leechCenterOffset = new Vector2(0f, 216f);

                    if (attackTimer == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 leechNPCCenter = npc.Center + leechCenterOffset;
                        for (int i = 0; i < Main.maxPlayers; i++)
                        {
                            Player player6 = Main.player[i];
                            if (player6.active && !player6.dead && Vector2.Distance(player6.Center, leechNPCCenter) <= 3000f)
                            {
                                Vector2 targetLeechDist = Main.player[npc.target].Center - leechNPCCenter;
                                if (targetLeechDist != Vector2.Zero)
                                    targetLeechDist.Normalize();

                                Projectile.NewProjectile(npc.GetSource_FromAI(), leechNPCCenter.X, leechNPCCenter.Y, targetLeechDist.X, targetLeechDist.Y, ProjectileID.MoonLeech, 0, 0f, Main.myPlayer, npc.whoAmI + 1, i);
                            }
                        }
                    }

                    if ((attackTimer == 120f || attackTimer == 150f || attackTimer == 180f || attackTimer == 210f || attackTimer == 240f) && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int j = 0; j < Main.maxProjectiles; j++)
                        {
                            Projectile projectile6 = Main.projectile[j];
                            if (projectile6.active && projectile6.type == ProjectileID.MoonLeech && Main.player[(int)projectile6.ai[1]].FindBuffIndex(BuffID.MoonLeech) != -1)
                            {
                                Vector2 targetCenter = Main.player[npc.target].Center;
                                int moonLeech = NPC.NewNPC(npc.GetSource_FromAI(), (int)targetCenter.X, (int)targetCenter.Y, NPCID.MoonLordLeechBlob);
                                Main.npc[moonLeech].netUpdate = true;
                                Main.npc[moonLeech].ai[0] = npc.whoAmI + 1;
                                Main.npc[moonLeech].ai[1] = j;
                            }
                        }
                    }
                }

                // Phantasmal Bolts
                else if (npc.ai[0] == 3f)
                {
                    if (attackTimer == 0f)
                    {
                        npc.TargetClosest(false);
                        npc.netUpdate = true;
                    }

                    Vector2 v4 = Main.player[npc.target].Center - npc.Center;
                    bool shootFirstBolt = attackTimer == attackTimerComparison - 14f;
                    bool shootSecondBolt = attackTimer == attackTimerComparison - 7f;
                    bool shootThirdBolt = attackTimer == attackTimerComparison;
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

                    if (attackTimer == attackTimerComparison - 35f)
                        SoundEngine.PlaySound(SoundID.NPCDeath6, npc.position);

                    if ((shootFirstBolt || shootSecondBolt || shootThirdBolt) && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 boltDirection = Utils.Vector2FromElipse(npc.localAI[0].ToRotationVector2(), boltAndDeathrayVector * npc.localAI[1]);

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

                        Vector2 boltVelocity = Vector2.Normalize(v4) * velocity;
                        int type = ProjectileID.PhantasmalBolt;
                        int damage = npc.GetProjectileDamage(type);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + boltDirection.X, npc.Center.Y + boltDirection.Y, boltVelocity.X, boltVelocity.Y, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                }

                // Dictates whether this npc is vulnerable or not
                int headEyeVulnerableCheck = headVulnerability * 7;
                if (headEyeVulnerableCheck > npc.localAI[2])
                    npc.localAI[2] += 1f;
                if (headEyeVulnerableCheck < npc.localAI[2])
                    npc.localAI[2] -= 1f;
                if (npc.localAI[2] < 0f)
                    npc.localAI[2] = 0f;
                if (npc.localAI[2] > 14f)
                    npc.localAI[2] = 14f;

                int headEyeDeathrayCheck = deathrayCheck * 5;
                if (headEyeDeathrayCheck > npc.localAI[3])
                    npc.localAI[3] += 1f;
                if (headEyeDeathrayCheck < npc.localAI[3])
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
                bool isLeftHand = npc.ai[2] == 0f;
                float handFaceDirection = -isLeftHand.ToDirectionInt();
                npc.spriteDirection = (int)handFaceDirection;

                npc.dontTakeDamage = npc.frameCounter >= 21.0;
                if (calamityGlobalNPC.newAI[0] == 1f)
                    npc.dontTakeDamage = true;

                Vector2 handBehaviorVector = new Vector2(30f, 66f);
                float handAttackTimer = 0f;
                float handAttackTimerComparison = 0f;
                int handFrameCheck = 0;

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

                    float handAI0CrossCheck = npc.ai[0];
                    npc.ai[1] += 1f;
                    int handAttackType = (int)Main.npc[(int)npc.ai[3]].ai[2];
                    int handType = isLeftHand ? 0 : 1;
                    int handFiveFrameTimer = 0;
                    int handAttackTimerIncrement = 0;

                    while (handFiveFrameTimer < 5)
                    {
                        handAttackTimerComparison = NPC.MoonLordAttacksArray[handAttackType, handType, 1, handFiveFrameTimer];
                        if (handAttackTimerComparison + handAttackTimerIncrement > npc.ai[1])
                            break;

                        handAttackTimerIncrement += (int)handAttackTimerComparison;
                        int totalSpawns = handFiveFrameTimer;
                        handFiveFrameTimer = totalSpawns + 1;
                    }

                    if (handFiveFrameTimer == 5)
                    {
                        handFiveFrameTimer = 0;
                        npc.ai[1] = 0f;
                        handAttackTimerComparison = NPC.MoonLordAttacksArray[handAttackType, handType, 1, handFiveFrameTimer];
                        handAttackTimerIncrement = 0;
                    }

                    npc.ai[0] = NPC.MoonLordAttacksArray[handAttackType, handType, 0, handFiveFrameTimer];
                    handAttackTimer = (int)npc.ai[1] - handAttackTimerIncrement;
                    if (npc.ai[0] != handAI0CrossCheck)
                        npc.netUpdate = true;
                }

                if (npc.ai[0] == -2f)
                {
                    handFrameCheck = 0;

                    npc.dontTakeDamage = true;

                    npc.velocity = Main.npc[(int)npc.ai[3]].velocity;
                }

                // Move
                else if (npc.ai[0] == 0f)
                {
                    handFrameCheck = 3;
                    npc.localAI[1] -= 0.05f;
                    if (npc.localAI[1] < 0f)
                        npc.localAI[1] = 0f;

                    Vector2 handCenter = Main.npc[(int)npc.ai[3]].Center;
                    Vector2 handMovementVector = handCenter + new Vector2(350f * handFaceDirection, -100f);
                    Vector2 handMovementDirection = handMovementVector - npc.Center;

                    if (handMovementDirection.Length() > 20f)
                    {
                        handMovementDirection.Normalize();

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

                        handMovementDirection *= velocity;
                        Vector2 handVelocity = npc.velocity;

                        if (handMovementDirection != Vector2.Zero)
                            npc.SimpleFlyMovement(handMovementDirection, 0.3f);

                        npc.velocity = Vector2.Lerp(handVelocity, npc.velocity, 0.5f);
                    }
                }

                // Phantasmal Eyes
                else if (npc.ai[0] == 1f)
                {
                    handFrameCheck = 0;
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

                    if (handAttackTimer >= 56)
                    {
                        npc.localAI[1] -= 0.07f;
                        if (npc.localAI[1] < 0f)
                            npc.localAI[1] = 0f;
                    }
                    else if (handAttackTimer >= 28)
                    {
                        npc.localAI[1] += 0.05f;
                        if (npc.localAI[1] > 0.75f)
                            npc.localAI[1] = 0.75f;

                        float handLocalAI = MathHelper.TwoPi * (handAttackTimer % 28) / 28 - MathHelper.PiOver2;
                        npc.localAI[0] = new Vector2((float)Math.Cos(handLocalAI) * handBehaviorVector.X, (float)Math.Sin(handLocalAI) * handBehaviorVector.Y).ToRotation();

                        if (handAttackTimer % divisor == 0f)
                        {
                            Vector2 handAttackVector = new Vector2(1f * -handFaceDirection, 3f);
                            Vector2 handAttackRotation = Utils.Vector2FromElipse(npc.localAI[0].ToRotationVector2(), handBehaviorVector * npc.localAI[1]);
                            Vector2 handAttackMovement = npc.Center + Vector2.Normalize(handAttackRotation) * handBehaviorVector.Length() * 0.4f + handAttackVector;

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

                            Vector2 handAttackDirection = Vector2.Normalize(handAttackRotation) * velocity;
                            float ai = (MathHelper.TwoPi * (float)Main.rand.NextDouble() - MathHelper.Pi) / 30f + 0.0174532924f * handFaceDirection;
                            int type = ProjectileID.PhantasmalEye;
                            int damage = npc.GetProjectileDamage(type);
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), handAttackMovement, handAttackDirection, type, damage, 0f, Main.myPlayer, 0f, ai);
                            Main.projectile[proj].timeLeft = 1200;
                            Main.projectile[proj].Calamity().lineColor = bossRush ? 1 : aggressionLevel;
                        }
                    }
                    else
                    {
                        npc.localAI[1] += 0.02f;
                        if (npc.localAI[1] > 0.75f)
                            npc.localAI[1] = 0.75f;

                        float handPauseDirection = MathHelper.TwoPi * (handAttackTimer % 28) / 28 - MathHelper.PiOver2;
                        npc.localAI[0] = new Vector2((float)Math.Cos(handPauseDirection) * handBehaviorVector.X, (float)Math.Sin(handPauseDirection) * handBehaviorVector.Y).ToRotation();
                    }
                }

                // Phantasmal Spheres
                else if (npc.ai[0] == 2f)
                {
                    npc.localAI[1] -= 0.05f;
                    if (npc.localAI[1] < 0f)
                        npc.localAI[1] = 0f;

                    Vector2 handCenter = Main.npc[(int)npc.ai[3]].Center;
                    Vector2 sphereHandDirection = new Vector2(220f * handFaceDirection, -60f) + handCenter;
                    sphereHandDirection += new Vector2(handFaceDirection * 100f, -50f);
                    Vector2 sphereHandDirectionMaxBound = new Vector2(400f * handFaceDirection, -60f);

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

                    if (handAttackTimer < 30f)
                    {
                        Vector2 sphereHandTravelVelocity = sphereHandDirection - npc.Center;
                        if (sphereHandTravelVelocity != Vector2.Zero)
                        {
                            Vector2 sphereHandTravelDist = sphereHandTravelVelocity;
                            sphereHandTravelDist.Normalize();

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

                            npc.velocity = Vector2.SmoothStep(npc.velocity, sphereHandTravelDist * Math.Min(velocity, sphereHandTravelVelocity.Length()), 0.2f);
                        }
                    }
                    else if (handAttackTimer < 210f)
                    {
                        handFrameCheck = 1;
                        int sphereHandSpeed = (int)handAttackTimer - 30;

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

                        if (sphereHandSpeed % divisor == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 sphereFireDirection = new Vector2(5f * handFaceDirection, -8f);
                            int finalSphereHandSpeed = sphereHandSpeed / 30;
                            sphereFireDirection.X += (finalSphereHandSpeed - 3.5f) * handFaceDirection * 3f;
                            sphereFireDirection.Y += (finalSphereHandSpeed - 4.5f) * 1f;
                            sphereFireDirection *= 1.2f;
                            int type = ProjectileID.PhantasmalSphere;
                            int damage = npc.GetProjectileDamage(type);
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y, sphereFireDirection.X, sphereFireDirection.Y, type, damage, 1f, Main.myPlayer, 0f, npc.whoAmI);
                            Main.projectile[proj].timeLeft = 1200;
                        }

                        Vector2 handSmoothMovement = Vector2.SmoothStep(sphereHandDirection, sphereHandDirection + sphereHandDirectionMaxBound, (handAttackTimer - 30f) / 180f) - npc.Center;
                        if (handSmoothMovement != Vector2.Zero)
                        {
                            Vector2 handSmoothMoveNormalize = handSmoothMovement;
                            handSmoothMoveNormalize.Normalize();

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

                            npc.velocity = Vector2.Lerp(npc.velocity, handSmoothMoveNormalize * Math.Min(velocity, handSmoothMovement.Length()), 0.5f);
                        }
                    }
                    else if (handAttackTimer < 282f)
                    {
                        handFrameCheck = 0;
                        npc.velocity *= velocityMultiplier;
                    }
                    else if (handAttackTimer < 287f)
                    {
                        handFrameCheck = 1;
                        npc.velocity *= velocityMultiplier;
                    }
                    else if (handAttackTimer < 292f)
                    {
                        handFrameCheck = 2;
                        npc.velocity *= velocityMultiplier;
                    }
                    else if (handAttackTimer < 300f)
                    {
                        handFrameCheck = 3;

                        if (handAttackTimer == 292f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int closestPlayer = Player.FindClosest(npc.position, npc.width, npc.height);
                            Vector2 sphereVelocity = Vector2.Normalize(Main.player[closestPlayer].Center - (npc.Center + Vector2.UnitY * -350f));
                            if (float.IsNaN(sphereVelocity.X) || float.IsNaN(sphereVelocity.Y))
                                sphereVelocity = Vector2.UnitY;

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

                            sphereVelocity *= velocity;
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                Projectile currentProjectile = Main.projectile[i];
                                if (currentProjectile.active && currentProjectile.type == ProjectileID.PhantasmalSphere && currentProjectile.ai[1] == npc.whoAmI && currentProjectile.ai[0] != -1f)
                                {
                                    currentProjectile.ai[0] = -1f;
                                    currentProjectile.velocity = sphereVelocity;
                                    currentProjectile.netUpdate = true;
                                }
                            }
                        }

                        Vector2 handPauseSmoothSpeed = Vector2.SmoothStep(sphereHandDirection, sphereHandDirection + sphereHandDirectionMaxBound, 1f - (handAttackTimer - 270f) / 30f) - npc.Center;
                        if (handPauseSmoothSpeed != Vector2.Zero)
                        {
                            Vector2 handPauseDirection = handPauseSmoothSpeed;
                            handPauseDirection.Normalize();

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

                            npc.velocity = Vector2.Lerp(npc.velocity, handPauseDirection * Math.Min(velocity, handPauseSmoothSpeed.Length()), 0.1f);
                        }
                    }
                    else
                    {
                        handFrameCheck = 3;

                        Vector2 handReturnSmoothSpeed = sphereHandDirection - npc.Center;
                        if (handReturnSmoothSpeed != Vector2.Zero)
                        {
                            Vector2 handReturnDirection = handReturnSmoothSpeed;
                            handReturnDirection.Normalize();

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

                            npc.velocity = Vector2.SmoothStep(npc.velocity, handReturnDirection * Math.Min(velocity, handReturnSmoothSpeed.Length()), 0.2f);
                        }
                    }
                }

                // Phantasmal Bolts
                else if (npc.ai[0] == 3f)
                {
                    if (handAttackTimer == 0f)
                    {
                        npc.TargetClosest(false);
                        npc.netUpdate = true;
                    }

                    Vector2 v = Main.player[npc.target].Center - npc.Center;
                    bool shootFirstBolt = handAttackTimer == handAttackTimerComparison - 14f;
                    bool shootSecondBolt = handAttackTimer == handAttackTimerComparison - 7f;
                    bool shootThirdBolt = handAttackTimer == handAttackTimerComparison;
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

                    if (handAttackTimer == handAttackTimerComparison - 35f)
                        SoundEngine.PlaySound(SoundID.NPCDeath6, npc.position);

                    if ((shootFirstBolt || shootSecondBolt || shootThirdBolt) && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 boltShootDirection = Utils.Vector2FromElipse(npc.localAI[0].ToRotationVector2(), handBehaviorVector * npc.localAI[1]);

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

                        Vector2 boltShootSpeed = Vector2.Normalize(v) * velocity;
                        int type = ProjectileID.PhantasmalBolt;
                        int damage = npc.GetProjectileDamage(type);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + boltShootDirection.X, npc.Center.Y + boltShootDirection.Y, boltShootSpeed.X, boltShootSpeed.Y, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                }

                // Center
                Vector2 handCentering = Main.npc[(int)npc.ai[3]].Center;
                Vector2 handDirection = new Vector2(220f * handFaceDirection, -60f) + handCentering;
                Vector2 minHandFaceDirection = handDirection + new Vector2(handFaceDirection * 110f, -150f);
                Vector2 maxHandFaceDirection = minHandFaceDirection + new Vector2(handFaceDirection * 370f, 150f);

                if (minHandFaceDirection.X > maxHandFaceDirection.X)
                    Utils.Swap(ref minHandFaceDirection.X, ref maxHandFaceDirection.X);
                if (minHandFaceDirection.Y > maxHandFaceDirection.Y)
                    Utils.Swap(ref minHandFaceDirection.Y, ref maxHandFaceDirection.Y);

                Vector2 handVelocity2 = Vector2.Clamp(npc.Center + npc.velocity, minHandFaceDirection, maxHandFaceDirection);
                if (handVelocity2 != npc.Center + npc.velocity)
                    npc.Center = handVelocity2 - npc.velocity;

                // Frames
                int handFrameTimer = handFrameCheck * 7;
                if (handFrameTimer > npc.frameCounter)
                {
                    double handFrameControl = npc.frameCounter;
                    npc.frameCounter = handFrameControl + 1.0;
                }
                if (handFrameTimer < npc.frameCounter)
                {
                    double handFrameControl = npc.frameCounter;
                    npc.frameCounter = handFrameControl - 1.0;
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

                Vector2 thirtyVector = new Vector2(30f);

                if (!Main.npc[(int)npc.ai[3]].active || Main.npc[(int)npc.ai[3]].type != NPCID.MoonLordCore)
                {
                    npc.life = 0;
                    npc.HitEffect(0, 10.0);
                    npc.active = false;
                }

                float secondAttackPicker = 0f;
                float ai0Copy = npc.ai[0];

                npc.ai[1] += 1f;

                int secondAttackArrayInc = 0;
                int secondAttackPickerInc = 0;
                while (secondAttackArrayInc < 10)
                {
                    secondAttackPicker = NPC.MoonLordAttacksArray2[1, secondAttackArrayInc];
                    if (secondAttackPicker + secondAttackPickerInc > npc.ai[1])
                        break;

                    secondAttackPickerInc += (int)secondAttackPicker;
                    secondAttackArrayInc += 1;
                }

                if (secondAttackArrayInc == 10)
                {
                    secondAttackArrayInc = 0;
                    npc.ai[1] = 0f;
                    secondAttackPicker = NPC.MoonLordAttacksArray2[1, secondAttackArrayInc];
                    secondAttackPickerInc = 0;
                }

                npc.ai[0] = NPC.MoonLordAttacksArray2[0, secondAttackArrayInc];
                float secondAttackTimer = (int)npc.ai[1] - secondAttackPickerInc;

                if (npc.ai[0] != ai0Copy)
                    npc.netUpdate = true;

                if (npc.ai[0] == -1f)
                {
                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 180f)
                        npc.ai[1] = 0f;

                    float localAI2Lerp;
                    if (npc.ai[1] < 60f)
                    {
                        localAI2Lerp = 0.75f;

                        npc.localAI[0] = 0f;

                        npc.localAI[1] = (float)Math.Sin(npc.ai[1] * MathHelper.TwoPi / 15f) * 0.35f;
                        if (npc.localAI[1] < 0f)
                            npc.localAI[0] = MathHelper.Pi;
                    }
                    else if (npc.ai[1] < 120f)
                    {
                        localAI2Lerp = 1f;

                        if (npc.localAI[1] < 0.5f)
                            npc.localAI[1] += 0.025f;

                        npc.localAI[0] += 0.209439516f;
                    }
                    else
                    {
                        localAI2Lerp = 1.15f;

                        npc.localAI[1] -= 0.05f;
                        if (npc.localAI[1] < 0f)
                            npc.localAI[1] = 0f;
                    }

                    npc.localAI[2] = MathHelper.Lerp(npc.localAI[2], localAI2Lerp, 0.3f);
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
                    Vector2 freeEyeCenter = npc.Center;
                    Vector2 freeEyeTargetCenter = Main.player[npc.target].Center;
                    Vector2 freeEyeTargetDistance = freeEyeTargetCenter - freeEyeCenter;
                    freeEyeTargetDistance = Vector2.Normalize(freeEyeTargetDistance) * velocity;

                    if (Vector2.Distance(freeEyeCenter, freeEyeTargetCenter) > 300f)
                    {
                        npc.velocity.X = (npc.velocity.X * 29 + freeEyeTargetDistance.X) / 30;
                        npc.velocity.Y = (npc.velocity.Y * 29 + freeEyeTargetDistance.Y) / 30;
                    }
                    else
                    {
                        npc.velocity *= 0.8f;
                        if (npc.velocity.Length() < 1f)
                            npc.velocity = Vector2.Zero;
                    }

                    // Fly towards Moon Lord Head and stay away from other True Eyes
                    float freeEyeAccel = 0.5f;
                    for (int j = 0; j < Main.maxNPCs; j++)
                    {
                        if (Main.npc[j].active)
                        {
                            if (j != npc.whoAmI && Main.npc[j].type == npc.type)
                            {
                                if (Vector2.Distance(npc.Center, Main.npc[j].Center) < 150f)
                                {
                                    if (npc.position.X < Main.npc[j].position.X)
                                        npc.velocity.X = npc.velocity.X - freeEyeAccel;
                                    else
                                        npc.velocity.X = npc.velocity.X + freeEyeAccel;

                                    if (npc.position.Y < Main.npc[j].position.Y)
                                        npc.velocity.Y = npc.velocity.Y - freeEyeAccel;
                                    else
                                        npc.velocity.Y = npc.velocity.Y + freeEyeAccel;
                                }
                            }
                        }
                    }
                    return false;
                }

                if (npc.ai[0] == 1f)
                {
                    if (secondAttackTimer == 0f)
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

                    if (secondAttackTimer < 20f)
                        npc.localAI[2] = MathHelper.Lerp(npc.localAI[2], 1.1f, 0.2f);
                    else
                        npc.localAI[2] = MathHelper.Lerp(npc.localAI[2], 0.4f, 0.2f);

                    if (secondAttackTimer == secondAttackPicker - 35f)
                        SoundEngine.PlaySound(SoundID.NPCDeath6, npc.position);

                    if ((secondAttackTimer == secondAttackPicker - 14f || secondAttackTimer == secondAttackPicker - 7f || secondAttackTimer == secondAttackPicker) && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 freeEyeBoltDirection = Utils.Vector2FromElipse(npc.localAI[0].ToRotationVector2(), thirtyVector * npc.localAI[1]);
                        float velocity = death ? 9f : 8f;
                        Vector2 freeEyeBoltVel = Vector2.Normalize(v8) * velocity;
                        int type = ProjectileID.PhantasmalBolt;
                        int damage = npc.GetProjectileDamage(type);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + freeEyeBoltDirection.X, npc.Center.Y + freeEyeBoltDirection.Y, freeEyeBoltVel.X, freeEyeBoltVel.Y, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
                else if (npc.ai[0] == 2f || npc.ai[0] == 4f)
                {
                    int type = ProjectileID.PhantasmalSphere;
                    int damage = npc.GetProjectileDamage(type);

                    if (secondAttackTimer < 15f)
                    {
                        npc.localAI[1] -= 0.07f;
                        if (npc.localAI[1] < 0f)
                            npc.localAI[1] = 0f;

                        npc.localAI[2] = MathHelper.Lerp(npc.localAI[2], 0.4f, 0.2f);

                        npc.velocity *= 0.8f;
                        if (npc.velocity.Length() < 1f)
                            npc.velocity = Vector2.Zero;
                    }
                    else if (secondAttackTimer < 75f)
                    {
                        float freeEyeAttackPattern = (secondAttackTimer - 15f) / 10f;
                        int freeEyeRotateValue = 0;
                        int freeEyeRotateValue2 = 0;
                        switch ((int)freeEyeAttackPattern)
                        {
                            case 0:
                                freeEyeRotateValue = 0;
                                freeEyeRotateValue2 = 2;
                                break;
                            case 1:
                                freeEyeRotateValue = 2;
                                freeEyeRotateValue2 = 5;
                                break;
                            case 2:
                                freeEyeRotateValue = 5;
                                freeEyeRotateValue2 = 3;
                                break;
                            case 3:
                                freeEyeRotateValue = 3;
                                freeEyeRotateValue2 = 1;
                                break;
                            case 4:
                                freeEyeRotateValue = 1;
                                freeEyeRotateValue2 = 4;
                                break;
                            case 5:
                                freeEyeRotateValue = 4;
                                freeEyeRotateValue2 = 0;
                                break;
                        }

                        Vector2 spinningpoint2 = Vector2.UnitY * -30f;
                        Vector2 freeEyeRotateLerpValue = spinningpoint2.RotatedBy(freeEyeRotateValue * MathHelper.TwoPi / 6f);
                        Vector2 freeEyeRotateLerpValue2 = spinningpoint2.RotatedBy(freeEyeRotateValue2 * MathHelper.TwoPi / 6f);
                        Vector2 freeEyeRotation = Vector2.Lerp(freeEyeRotateLerpValue, freeEyeRotateLerpValue2, freeEyeAttackPattern - (int)freeEyeAttackPattern);
                        float freeEyeRotationDist = freeEyeRotation.Length() / 30f;

                        npc.localAI[0] = freeEyeRotation.ToRotation();
                        npc.localAI[1] = MathHelper.Lerp(npc.localAI[1], freeEyeRotationDist, 0.5f);

                        for (int k = 0; k < 2; k++)
                        {
                            int trueEyeDust = Dust.NewDust(npc.Center + freeEyeRotation - Vector2.One * 4f, 0, 0, 229, 0f, 0f, 0, default, 1f);
                            Dust dust = Main.dust[trueEyeDust];
                            dust.velocity += freeEyeRotation / 15f;
                            Main.dust[trueEyeDust].noGravity = true;
                        }

                        if ((secondAttackTimer - 15f) % 10f == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 trueEyeSphereDirection = Vector2.Normalize(freeEyeRotation);
                            if (trueEyeSphereDirection.HasNaNs())
                                trueEyeSphereDirection = Vector2.UnitY * -1f;

                            float spreadVelocity = death ? 4.5f : 4f;
                            trueEyeSphereDirection *= 4f;
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + freeEyeRotation.X, npc.Center.Y + freeEyeRotation.Y, trueEyeSphereDirection.X, trueEyeSphereDirection.Y, type, 0, 0f, Main.myPlayer, 30f, npc.whoAmI);
                            Main.projectile[proj].timeLeft = 1200;

                            if (CalamityWorld.LegendaryMode)
                            {
                                for (int k = 0; k < 3; k++)
                                {
                                    if (!WorldGen.SolidTile((int)(npc.Center.X / 16f), (int)(npc.Center.Y / 16f)))
                                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y, (float)Main.rand.Next(-1599, 1600) * 0.01f, (float)Main.rand.Next(-1599, 1) * 0.01f, ProjectileID.MoonBoulder, 70, 10f);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (secondAttackTimer < 105f)
                        {
                            npc.localAI[0] = npc.localAI[0].AngleLerp(npc.ai[2] - MathHelper.PiOver2, 0.2f);

                            npc.localAI[2] = MathHelper.Lerp(npc.localAI[2], 0.75f, 0.2f);

                            if (secondAttackTimer == 75f)
                            {
                                npc.TargetClosest(false);

                                npc.netUpdate = true;

                                npc.velocity = Vector2.UnitY * -7f;

                                for (int i = 0; i < Main.maxProjectiles; i++)
                                {
                                    Projectile trueEyeSpheres = Main.projectile[i];
                                    if (trueEyeSpheres.active && trueEyeSpheres.type == type && trueEyeSpheres.ai[1] == npc.whoAmI && trueEyeSpheres.ai[0] != -1f)
                                    {
                                        Projectile trueEyeSphereVel = trueEyeSpheres;
                                        trueEyeSphereVel.velocity += npc.velocity;
                                        trueEyeSpheres.netUpdate = true;
                                    }
                                }
                            }

                            npc.velocity.Y = npc.velocity.Y * 0.96f;

                            npc.ai[2] = (Main.player[npc.target].Center - npc.Center).ToRotation() + MathHelper.PiOver2;

                            npc.rotation = npc.rotation.AngleTowards(npc.ai[2], 0.104719758f);

                            return false;
                        }

                        if (secondAttackTimer < 120f)
                        {
                            SoundEngine.PlaySound(SoundID.Zombie102, npc.Center);

                            if (secondAttackTimer == 105f)
                                npc.netUpdate = true;

                            float velocity = death ? 13.25f : 12f;
                            Vector2 trueEyeSphereVelocity = (npc.ai[2] - MathHelper.PiOver2).ToRotationVector2() * velocity;
                            npc.velocity = trueEyeSphereVelocity * 2f;

                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                Projectile trueEyeSphereProj = Main.projectile[i];
                                if (trueEyeSphereProj.active && trueEyeSphereProj.type == type && trueEyeSphereProj.ai[1] == npc.whoAmI && trueEyeSphereProj.ai[0] != -1f)
                                {
                                    trueEyeSphereProj.ai[0] = -1f;
                                    trueEyeSphereProj.damage = damage;
                                    trueEyeSphereProj.velocity = trueEyeSphereVelocity;
                                    trueEyeSphereProj.netUpdate = true;
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
                    if (secondAttackTimer < 15f)
                    {
                        npc.localAI[1] -= 0.07f;
                        if (npc.localAI[1] < 0f)
                            npc.localAI[1] = 0f;

                        npc.localAI[2] = MathHelper.Lerp(npc.localAI[2], 0.4f, 0.2f);

                        npc.velocity *= 0.9f;
                        if (npc.velocity.Length() < 1f)
                            npc.velocity = Vector2.Zero;
                    }
                    else if (secondAttackTimer < 45f)
                    {
                        npc.localAI[0] = 0f;

                        npc.localAI[1] = (float)Math.Sin((secondAttackTimer - 15f) * MathHelper.TwoPi / 15f) * 0.5f;
                        if (npc.localAI[1] < 0f)
                            npc.localAI[0] = MathHelper.Pi;
                    }
                    else
                    {
                        if (secondAttackTimer >= 185f)
                        {
                            npc.velocity *= 0.88f;

                            npc.rotation = npc.rotation.AngleLerp(0f, 0.2f);

                            npc.localAI[1] -= 0.07f;
                            if (npc.localAI[1] < 0f)
                                npc.localAI[1] = 0f;

                            npc.localAI[2] = MathHelper.Lerp(npc.localAI[2], 1f, 0.2f);
                            return false;
                        }

                        if (secondAttackTimer == 45f)
                        {
                            npc.ai[2] = Main.rand.NextBool().ToDirectionInt() * MathHelper.TwoPi / 40f;
                            npc.netUpdate = true;
                        }

                        if ((secondAttackTimer - 15f - 30f) % 40f == 0f)
                            npc.ai[2] *= 0.95f;

                        npc.localAI[0] += npc.ai[2];

                        npc.localAI[1] += 0.05f;
                        if (npc.localAI[1] > 1f)
                            npc.localAI[1] = 1f;

                        Vector2 trueEyeDirection = npc.localAI[0].ToRotationVector2() * thirtyVector * npc.localAI[1];
                        float trueEyeVelScale = MathHelper.Lerp(8f, 20f, (secondAttackTimer - 15f - 30f) / 140f);

                        npc.velocity = Vector2.Normalize(trueEyeDirection) * trueEyeVelScale;
                        npc.rotation = npc.rotation.AngleLerp(npc.velocity.ToRotation() + MathHelper.PiOver2, 0.2f);

                        if ((secondAttackTimer - 15f - 30f) % 10f == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 trueEyeEyeDirection = npc.Center + Vector2.Normalize(trueEyeDirection) * thirtyVector.Length() * 0.4f;
                            float velocity = death ? 6f : 5f;
                            Vector2 trueEyeEyeSpeed = Vector2.Normalize(trueEyeDirection) * velocity;
                            float ai3 = (MathHelper.TwoPi * (float)Main.rand.NextDouble() - MathHelper.Pi) / 30f + 0.0174532924f * npc.ai[2];
                            int type = ProjectileID.PhantasmalEye;
                            int damage = npc.GetProjectileDamage(type);
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), trueEyeEyeDirection, trueEyeEyeSpeed, type, damage, 0f, Main.myPlayer, 0f, ai3);
                            Main.projectile[proj].timeLeft = 1200;
                        }
                    }
                }
                else if (npc.ai[0] == 4f)
                {
                    if (secondAttackTimer == 0f)
                    {
                        npc.TargetClosest(false);
                        npc.netUpdate = true;
                    }

                    if (secondAttackTimer < 180f)
                    {
                        npc.localAI[2] = MathHelper.Lerp(npc.localAI[2], 1f, 0.2f);

                        npc.localAI[1] -= 0.05f;
                        if (npc.localAI[1] < 0f)
                            npc.localAI[1] = 0f;

                        npc.velocity *= 0.95f;
                        if (npc.velocity.Length() < 1f)
                            npc.velocity = Vector2.Zero;

                        if (secondAttackTimer >= 60f)
                        {
                            Vector2 trueEyeCentering = npc.Center;

                            int dustAmt = 0;
                            if (secondAttackTimer >= 120f)
                                dustAmt = 1;

                            for (int j = 0; j < 1 + dustAmt; j++)
                            {
                                float dustScale = 0.8f;
                                if (j % 2 == 1)
                                    dustScale = 1.65f;

                                Vector2 trueEyeDustDirection = trueEyeCentering + ((float)Main.rand.NextDouble() * MathHelper.TwoPi).ToRotationVector2() * thirtyVector / 2f;
                                int trueEyeDust = Dust.NewDust(trueEyeDustDirection - Vector2.One * 8f, 16, 16, 229, npc.velocity.X / 2f, npc.velocity.Y / 2f, 0, default, 1f);
                                Main.dust[trueEyeDust].velocity = Vector2.Normalize(trueEyeCentering - trueEyeDustDirection) * 3.5f * (10f - dustAmt * 2f) / 10f;
                                Main.dust[trueEyeDust].noGravity = true;
                                Main.dust[trueEyeDust].scale = dustScale;
                                Main.dust[trueEyeDust].customData = npc;
                            }
                        }
                    }
                    else
                    {
                        if (secondAttackTimer < secondAttackPicker - 15f)
                        {
                            if (calamityGlobalNPC.newAI[1] == 0f)
                                calamityGlobalNPC.newAI[1] = 600f;

                            if (secondAttackTimer == 180f && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                // If head is in deathray phase
                                if (Main.npc[(int)Main.npc[(int)npc.ai[3]].localAI[2]].ai[0] == 1f)
                                    calamityGlobalNPC.newAI[1] *= 1.5f;

                                npc.TargetClosest(false);

                                Vector2 deathrayTargetDist = Main.player[npc.target].Center - npc.Center;
                                deathrayTargetDist.Normalize();

                                float deathraySweepDirection = -1f;
                                if (deathrayTargetDist.X < 0f)
                                    deathraySweepDirection = 1f;

                                deathrayTargetDist = deathrayTargetDist.RotatedBy(-(double)deathraySweepDirection * MathHelper.TwoPi / 6f);
                                int type = ProjectileID.PhantasmalDeathray;
                                int damage = npc.GetProjectileDamage(type);
                                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y, deathrayTargetDist.X, deathrayTargetDist.Y, type, damage, 0f, Main.myPlayer, deathraySweepDirection * MathHelper.TwoPi / calamityGlobalNPC.newAI[1], npc.whoAmI);
                                npc.ai[2] = (deathrayTargetDist.ToRotation() + MathHelper.Pi + MathHelper.TwoPi) * deathraySweepDirection;
                                npc.netUpdate = true;
                            }

                            npc.localAI[1] += 0.05f;
                            if (npc.localAI[1] > 1f)
                                npc.localAI[1] = 1f;

                            float deathrayRotationDirection = (npc.ai[2] >= 0f).ToDirectionInt();
                            float deathrayRotation = npc.ai[2];
                            if (deathrayRotation < 0f)
                                deathrayRotation *= -1f;

                            deathrayRotation += -(MathHelper.Pi + MathHelper.TwoPi);
                            deathrayRotation += deathrayRotationDirection * MathHelper.TwoPi / calamityGlobalNPC.newAI[1];

                            npc.localAI[0] = deathrayRotation;
                            npc.ai[2] = (deathrayRotation + MathHelper.Pi + MathHelper.TwoPi) * deathrayRotationDirection;

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
                Vector2 mouthMovement = new Vector2(0f, 216f);
                int headNPCType = (int)Math.Abs(npc.ai[0]) - 1;
                int moonLordHead = (int)npc.ai[1];

                // Despawn
                if (!Main.npc[headNPCType].active || Main.npc[headNPCType].type != NPCID.MoonLordHead)
                {
                    npc.life = 0;
                    npc.HitEffect(0, 10.0);
                    npc.active = false;
                    return false;
                }

                // Heal the Moon Lord
                npc.ai[2] += 1f;
                if (npc.ai[2] >= 180f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int k = (int)Main.npc[headNPCType].ai[3];
                        int leftHandHeal = -1;
                        int rightHandHeal = -1;
                        int headHeal = headNPCType;

                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].active && Main.npc[i].ai[3] == k)
                            {
                                if (leftHandHeal == -1 && Main.npc[i].type == NPCID.MoonLordHand && Main.npc[i].ai[2] == 0f)
                                    leftHandHeal = i;
                                if (rightHandHeal == -1 && Main.npc[i].type == NPCID.MoonLordHand && Main.npc[i].ai[2] == 1f)
                                    rightHandHeal = i;
                                if (leftHandHeal != -1 && rightHandHeal != -1 && headHeal != -1)
                                    break;
                            }
                        }

                        // Heal limits
                        int maxHealAmt = death ? 1500 : 1250;
                        int totalHealth = Main.npc[k].lifeMax - Main.npc[k].life;
                        int leftHandHealth = Main.npc[leftHandHeal].lifeMax - Main.npc[leftHandHeal].life;
                        int rightHandHealth = Main.npc[rightHandHeal].lifeMax - Main.npc[rightHandHeal].life;
                        int headHealth = Main.npc[headHeal].lifeMax - Main.npc[headHeal].life;

                        // Healing
                        if (headHealth > 0 && maxHealAmt > 0)
                        {
                            int maxHealthFailsafe = headHealth - maxHealAmt;
                            if (maxHealthFailsafe > 0)
                            {
                                maxHealthFailsafe = 0;
                            }
                            int headHealingAmt = maxHealAmt + maxHealthFailsafe;
                            maxHealAmt -= headHealingAmt;
                            NPC nPC6 = Main.npc[headHeal];
                            nPC6.life += headHealingAmt;
                            NPC.HealEffect(Utils.CenteredRectangle(Main.npc[headHeal].Center, new Vector2(50f)), headHealingAmt, true);
                        }
                        if (totalHealth > 0 && maxHealAmt > 0)
                        {
                            int totalHealthFailsafe = totalHealth - maxHealAmt;
                            if (totalHealthFailsafe > 0)
                            {
                                totalHealthFailsafe = 0;
                            }
                            int totalHealingAmt = maxHealAmt + totalHealthFailsafe;
                            maxHealAmt -= totalHealingAmt;
                            NPC nPC6 = Main.npc[k];
                            nPC6.life += totalHealingAmt;
                            NPC.HealEffect(Utils.CenteredRectangle(Main.npc[k].Center, new Vector2(50f)), totalHealingAmt, true);
                        }
                        if (leftHandHealth > 0 && maxHealAmt > 0)
                        {
                            int leftHandHealthFailsafe = leftHandHealth - maxHealAmt;
                            if (leftHandHealthFailsafe > 0)
                            {
                                leftHandHealthFailsafe = 0;
                            }
                            int leftHandHealingAmt = maxHealAmt + leftHandHealthFailsafe;
                            maxHealAmt -= leftHandHealingAmt;
                            NPC nPC6 = Main.npc[leftHandHeal];
                            nPC6.life += leftHandHealingAmt;
                            NPC.HealEffect(Utils.CenteredRectangle(Main.npc[leftHandHeal].Center, new Vector2(50f)), leftHandHealingAmt, true);
                        }
                        if (rightHandHealth > 0 && maxHealAmt > 0)
                        {
                            int rightHandHealthFailsafe = rightHandHealth - maxHealAmt;
                            if (rightHandHealthFailsafe > 0)
                            {
                                rightHandHealthFailsafe = 0;
                            }
                            int rightHandHealingAmt = maxHealAmt + rightHandHealthFailsafe;
                            NPC nPC6 = Main.npc[rightHandHeal];
                            nPC6.life += rightHandHealingAmt;
                            NPC.HealEffect(Utils.CenteredRectangle(Main.npc[rightHandHeal].Center, new Vector2(50f)), rightHandHealingAmt, true);
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
                npc.Center = Vector2.Lerp(Main.projectile[moonLordHead].Center, Main.npc[(int)Math.Abs(npc.ai[0]) - 1].Center + mouthMovement, npc.ai[2] / 180f);

                // Emit dust
                Vector2 spinningpoint3 = Vector2.UnitY * -npc.height / 2f;
                for (int i = 0; i < 4; i++)
                {
                    int leechDust = Dust.NewDust(npc.Center - Vector2.One * 4f + spinningpoint3.RotatedBy(i * MathHelper.TwoPi / 6f), 0, 0, 229, 0f, 0f, 0, default, 1f);
                    Main.dust[leechDust].velocity = -Vector2.UnitY;
                    Main.dust[leechDust].noGravity = true;
                    Main.dust[leechDust].scale = 0.7f;
                    Main.dust[leechDust].customData = npc;
                }

                spinningpoint3 = Vector2.UnitY * -npc.height / 6f;
                for (int j = 0; j < 2; j++)
                {
                    int leechDust2 = Dust.NewDust(npc.Center - Vector2.One * 4f + spinningpoint3.RotatedBy(j * MathHelper.TwoPi / 6f), 0, 0, 229, 0f, -2f, 0, default, 1f);
                    Main.dust[leechDust2].noGravity = true;
                    Main.dust[leechDust2].scale = 1.5f;
                    Main.dust[leechDust2].customData = npc;
                }
            }
            return false;
        }
    }
}
