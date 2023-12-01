using System;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCOverrides.Bosses
{
    public static class SkeletronAI
    {
        public static bool BuffedSkeletronAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Phases
            bool respawnHands = lifeRatio < 0.33f;
            bool phase2 = respawnHands || death;

            // Set defense
            npc.defense = npc.defDefense;

            npc.reflectsProjectiles = false;

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
                    SoundEngine.PlaySound(SoundID.Roar with { Pitch = SoundID.Roar.Pitch - 0.25f }, npc.position);
                    SpawnHands();

                    npc.netUpdate = true;
                    npc.SyncExtraAI();
                }

                void SpawnHands()
                {
                    int skeletronHand = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.SkeletronHand, npc.whoAmI);
                    Main.npc[skeletronHand].ai[0] = -1f;
                    Main.npc[skeletronHand].ai[1] = npc.whoAmI;
                    Main.npc[skeletronHand].target = npc.target;
                    Main.npc[skeletronHand].netUpdate = true;

                    skeletronHand = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.SkeletronHand, npc.whoAmI);
                    Main.npc[skeletronHand].ai[0] = 1f;
                    Main.npc[skeletronHand].ai[1] = npc.whoAmI;
                    Main.npc[skeletronHand].ai[3] = 150f;
                    Main.npc[skeletronHand].target = npc.target;
                    Main.npc[skeletronHand].netUpdate = true;

                    // Spawn two additional hands with different attack timings
                    if (death)
                    {
                        skeletronHand = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.SkeletronHand, npc.whoAmI);
                        Main.npc[skeletronHand].ai[0] = -1f;
                        Main.npc[skeletronHand].Calamity().newAI[0] = -1f;
                        Main.npc[skeletronHand].ai[1] = npc.whoAmI;
                        Main.npc[skeletronHand].ai[3] = respawnHands ? -75f : 0f;
                        Main.npc[skeletronHand].target = npc.target;
                        Main.npc[skeletronHand].netUpdate = true;

                        skeletronHand = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.SkeletronHand, npc.whoAmI);
                        Main.npc[skeletronHand].ai[0] = 1f;
                        Main.npc[skeletronHand].Calamity().newAI[0] = -1f;
                        Main.npc[skeletronHand].ai[1] = npc.whoAmI;
                        Main.npc[skeletronHand].ai[3] = respawnHands ? 75f : 150f;
                        Main.npc[skeletronHand].target = npc.target;
                        Main.npc[skeletronHand].netUpdate = true;
                    }
                }
            }

            // Distance from target
            float distance = Vector2.Distance(Main.player[npc.target].Center, npc.Center);

            // Despawn
            if (Main.player[npc.target].dead || distance > (bossRush ? CalamityGlobalNPC.CatchUpDistance350Tiles : CalamityGlobalNPC.CatchUpDistance200Tiles))
            {
                npc.TargetClosest();
                if (Main.player[npc.target].dead || distance > (bossRush ? CalamityGlobalNPC.CatchUpDistance350Tiles : CalamityGlobalNPC.CatchUpDistance200Tiles))
                    npc.ai[1] = 3f;
            }

            // Daytime enrage
            if (Main.dayTime && !bossRush && npc.ai[1] != 3f && npc.ai[1] != 2f)
            {
                npc.ai[1] = 2f;
                SoundEngine.PlaySound(SoundID.Roar, npc.position);
            }

            // Hand immunity
            int numHandsAlive = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == NPCID.SkeletronHand)
                    numHandsAlive++;
            }
            bool handsDead = numHandsAlive == 0;
            npc.chaseable = handsDead;
            calamityGlobalNPC.DR = numHandsAlive > 0 ? 0.9999f : 0.05f;
            calamityGlobalNPC.unbreakableDR = numHandsAlive > 0;
            calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = numHandsAlive > 0;

            // Teleport
            if (handsDead || phase2)
            {
                // Post-teleport
                if (npc.ai[3] == -60f)
                {
                    npc.ai[3] = 0f;

                    SoundEngine.PlaySound(SoundID.Item66, npc.position);

                    // Fire magic bolt after teleport
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float magicBoltSpeed = 2f + (distance * 0.005f);
                        if (magicBoltSpeed > 6f)
                            magicBoltSpeed = 6f;

                        int type = ProjectileID.Skull;
                        int damage = npc.GetProjectileDamage(type);
                        int numProj = Main.getGoodWorld ? 22 : death ? 5 : 3;

                        float boltTargetXDist = Main.player[npc.target].Center.X - npc.Center.X;
                        float boltTargetYDist = Main.player[npc.target].Center.Y - npc.Center.Y;
                        float boltTargetDistance = (float)Math.Sqrt(boltTargetXDist * boltTargetXDist + boltTargetYDist * boltTargetYDist);

                        boltTargetDistance = magicBoltSpeed / boltTargetDistance;
                        boltTargetXDist *= boltTargetDistance;
                        boltTargetYDist *= boltTargetDistance;
                        Vector2 center = npc.Center;
                        center.X += boltTargetXDist * 5f;
                        center.Y += boltTargetYDist * 5f;

                        float rotation = MathHelper.ToRadians(Main.getGoodWorld ? 180 : 60);
                        float baseSpeed = (float)Math.Sqrt(boltTargetXDist * boltTargetXDist + boltTargetYDist * boltTargetYDist);
                        double startAngle = Math.Atan2(boltTargetXDist, boltTargetYDist) - rotation / 2;
                        double deltaAngle = rotation / numProj;
                        double offsetAngle;

                        for (int i = 0; i < numProj; i++)
                        {
                            offsetAngle = startAngle + deltaAngle * i;
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), center.X, center.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), type, damage, 0f, Main.myPlayer, -2f, 0f);
                            Main.projectile[proj].timeLeft = 300;
                        }

                        npc.netUpdate = true;
                    }

                    // Teleport dust
                    for (int m = 0; m < 30; m++)
                    {
                        int teleportDust = Dust.NewDust(npc.position, npc.width, npc.height, 27, 0f, 0f, 200, default, 3f);
                        Main.dust[teleportDust].noGravity = true;
                        Main.dust[teleportDust].velocity.X = Main.dust[teleportDust].velocity.X * 2f;
                    }
                }

                // Teleport after a certain time
                // If hands are dead: 7 seconds
                // If hands are not dead: 14 seconds
                // If hands are dead in phase 2: 4.7 seconds
                npc.ai[3] += 1f + (((phase2 && handsDead) || bossRush) ? 0.5f : 0f) - ((handsDead || bossRush) ? 0f : 0.5f);

                // Dust to show teleport
                int ai3 = (int)npc.ai[3]; // 0 to 30, and -60
                bool emitDust = false;

                if (ai3 >= 300 && calamityGlobalNPC.newAI[2] == 0f && calamityGlobalNPC.newAI[3] == 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 skullFaceDirection = npc.Center + new Vector2(npc.direction * 20, 6f);
                        Vector2 skullTargetDirection = Main.player[npc.target].Center - skullFaceDirection;
                        Point skullTileCoords = npc.Center.ToTileCoordinates();
                        Point targetTileCoords = Main.player[npc.target].Center.ToTileCoordinates();
                        int randomTeleportOffset = 26;
                        int skullPositionOffset = 4;
                        int targetPositionOffset = 22;
                        int teleportTries = 0;

                        bool targetTooFar = false;
                        if (skullTargetDirection.Length() > 2000f)
                            targetTooFar = true;

                        while (!targetTooFar && teleportTries < 100)
                        {
                            teleportTries++;
                            int teleportTileX = Main.rand.Next(targetTileCoords.X - randomTeleportOffset, targetTileCoords.X + randomTeleportOffset + 1);
                            int teleportTileY = Main.rand.Next(targetTileCoords.Y - randomTeleportOffset, targetTileCoords.Y + randomTeleportOffset + 1);
                            if ((teleportTileY < targetTileCoords.Y - targetPositionOffset || teleportTileY > targetTileCoords.Y + targetPositionOffset || teleportTileX < targetTileCoords.X - targetPositionOffset || teleportTileX > targetTileCoords.X + targetPositionOffset) && (teleportTileY < skullTileCoords.Y - skullPositionOffset || teleportTileY > skullTileCoords.Y + skullPositionOffset || teleportTileX < skullTileCoords.X - skullPositionOffset || teleportTileX > skullTileCoords.X + skullPositionOffset) && !Main.tile[teleportTileX, teleportTileY].HasUnactuatedTile)
                            {
                                // New location params
                                calamityGlobalNPC.newAI[2] = teleportTileX * 16 - npc.width / 2;
                                calamityGlobalNPC.newAI[3] = teleportTileY * 16 - npc.height;
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
                        int teleportDust = Dust.NewDust(position, npc.width, npc.height, 27, 0f, 0f, 200, default, 2f);
                        Main.dust[teleportDust].noGravity = true;
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
                        int teleportDust = Dust.NewDust(npc.position, npc.width, npc.height, 27, 0f, 0f, 200, default, 3f);
                        Main.dust[teleportDust].noGravity = true;
                        Main.dust[teleportDust].velocity.X = Main.dust[teleportDust].velocity.X * 2f;
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
                float skullProjFrequency = bossRush ? 10f : phase2 ? (60f - (death ? 30f * (1f - lifeRatio) : 0f)) : 75f;
                if (Main.getGoodWorld)
                    skullProjFrequency *= 0.8f;
                skullProjFrequency = (float)Math.Ceiling(skullProjFrequency);

                if (Main.netMode != NetmodeID.MultiplayerClient && calamityGlobalNPC.newAI[1] % skullProjFrequency == 0f && calamityGlobalNPC.newAI[1] > 45f)
                {
                    Vector2 skullFiringPos = npc.Center;
                    float skullProjTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - skullFiringPos.X;
                    float skullProjTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - skullFiringPos.Y;
                    // Ozzatron: noticed this is unused. Leaving as-is for minimal touch.
                    float skullProjTargetDist = (float)Math.Sqrt(skullProjTargetX * skullProjTargetX + skullProjTargetY * skullProjTargetY);
                    if (Collision.CanHit(skullFiringPos, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        float skullProjSpeed = phase2 ? (5f + (death ? 3f * (1f - lifeRatio) : 0f)) : 4f;
                        skullProjTargetX = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - skullFiringPos.X;
                        skullProjTargetY = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - skullFiringPos.Y;
                        skullProjTargetDist = (float)Math.Sqrt(skullProjTargetX * skullProjTargetX + skullProjTargetY * skullProjTargetY);
                        skullProjTargetDist = skullProjSpeed / skullProjTargetDist;
                        skullProjTargetX *= skullProjTargetDist;
                        skullProjTargetY *= skullProjTargetDist;
                        int spread = bossRush ? 100 : 50;
                        Vector2 skullProjDirection = new Vector2(skullProjTargetX + Main.rand.Next(-spread, spread + 1) * 0.01f, skullProjTargetY + Main.rand.Next(-spread, spread + 1) * 0.01f);
                        skullProjDirection.Normalize();
                        skullProjDirection *= skullProjSpeed;
                        skullProjDirection += npc.velocity;
                        skullProjTargetX = skullProjDirection.X;
                        skullProjTargetY = skullProjDirection.Y;
                        int type = ProjectileID.Skull;
                        int damage = npc.GetProjectileDamage(type);
                        skullFiringPos += skullProjDirection * 5f;
                        int skullProjectile = Projectile.NewProjectile(npc.GetSource_FromAI(), skullFiringPos.X, skullFiringPos.Y, skullProjTargetX, skullProjTargetY, type, damage, 0f, Main.myPlayer, -1f, 0f);
                        Main.projectile[skullProjectile].timeLeft = 300;

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

                float headYAcceleration = 0.04f + (death ? 0.04f * (1f - lifeRatio) : 0f);
                float headYTopSpeed = 3.5f - (death ? 1f - lifeRatio : 0f);
                float headXAcceleration = 0.08f + (death ? 0.04f * (1f - lifeRatio) : 0f);
                float headXTopSpeed = 8.5f - (death ? 2f * (1f - lifeRatio) : 0f);
                if (Main.getGoodWorld)
                {
                    headYAcceleration += 0.01f;
                    headYTopSpeed += 1f;
                    headXAcceleration += 0.05f;
                    headXTopSpeed += 2f;
                }
                if (bossRush)
                {
                    headYAcceleration *= 1.25f;
                    headYTopSpeed *= 0.75f;
                    headXAcceleration *= 1.25f;
                    headXTopSpeed *= 0.75f;
                }

                if (npc.position.Y > Main.player[npc.target].position.Y - 250f)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= 0.98f;
                    npc.velocity.Y -= headYAcceleration;
                    if (npc.velocity.Y > headYTopSpeed)
                        npc.velocity.Y = headYTopSpeed;
                }
                else if (npc.position.Y < Main.player[npc.target].position.Y - 250f)
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= 0.98f;
                    npc.velocity.Y += headYAcceleration;
                    if (npc.velocity.Y < -headYTopSpeed)
                        npc.velocity.Y = -headYTopSpeed;
                }

                if (npc.position.X + (npc.width / 2) > Main.player[npc.target].position.X + (Main.player[npc.target].width / 2))
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= 0.98f;
                    npc.velocity.X -= headXAcceleration;
                    if (npc.velocity.X > headXTopSpeed)
                        npc.velocity.X = headXTopSpeed;
                }

                if (npc.position.X + (npc.width / 2) < Main.player[npc.target].position.X + (Main.player[npc.target].width / 2))
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= 0.98f;
                    npc.velocity.X += headXAcceleration;
                    if (npc.velocity.X < -headXTopSpeed)
                        npc.velocity.X = -headXTopSpeed;
                }
            }

            // Spin charge
            else if (npc.ai[1] == 1f)
            {
                if (Main.getGoodWorld)
                {
                    npc.reflectsProjectiles = true;
                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == 0f)
                    {
                        if (NPC.CountNPCS(NPCID.DarkCaster) < 6)
                        {
                            for (int i = 0; i < 1000; i++)
                            {
                                int headYAcceleration = (int)(npc.Center.X / 16f) + Main.rand.Next(-50, 51);
                                int headYTopSpeed;
                                for (headYTopSpeed = (int)(npc.Center.Y / 16f) + Main.rand.Next(-50, 51); headYTopSpeed < Main.maxTilesY - 10 && !WorldGen.SolidTile(headYAcceleration, headYTopSpeed); headYTopSpeed++)
                                {
                                }

                                headYTopSpeed--;
                                if (!WorldGen.SolidTile(headYAcceleration, headYTopSpeed))
                                {
                                    int headXAcceleration = NPC.NewNPC(npc.GetSource_FromAI(), headYAcceleration * 16 + 8, headYTopSpeed * 16, NPCID.DarkCaster);
                                    if (Main.netMode == NetmodeID.Server && headXAcceleration < Main.maxNPCs)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, headXAcceleration);

                                    break;
                                }
                            }
                        }

                        if (CalamityWorld.LegendaryMode)
                        {
                            if (!NPC.AnyNPCs(NPCID.DiabolistWhite))
                            {
                                for (int i = 0; i < 1000; i++)
                                {
                                    int headYAcceleration = (int)(npc.Center.X / 16f) + Main.rand.Next(-50, 51);
                                    int headYTopSpeed;
                                    for (headYTopSpeed = (int)(npc.Center.Y / 16f) + Main.rand.Next(-50, 51); headYTopSpeed < Main.maxTilesY - 10 && !WorldGen.SolidTile(headYAcceleration, headYTopSpeed); headYTopSpeed++)
                                    {
                                    }

                                    headYTopSpeed--;
                                    if (!WorldGen.SolidTile(headYAcceleration, headYTopSpeed))
                                    {
                                        int headXAcceleration = NPC.NewNPC(npc.GetSource_FromAI(), headYAcceleration * 16 + 8, headYTopSpeed * 16, NPCID.DiabolistWhite);
                                        if (Main.netMode == NetmodeID.Server && headXAcceleration < Main.maxNPCs)
                                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, headXAcceleration);

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                npc.defense -= 10;

                float phaseChangeRateBoost = 0.5f * (1f - lifeRatio);
                npc.ai[2] += 1f + phaseChangeRateBoost;

                calamityGlobalNPC.newAI[1] += 1f;
                if (calamityGlobalNPC.newAI[1] == 2f)
                    SoundEngine.PlaySound(SoundID.Roar, npc.position);

                if (npc.ai[2] >= 300f)
                {
                    if (Main.getGoodWorld)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient && NPC.CountNPCS(NPCID.DarkCaster) < 6)
                        {
                            for (int j = 0; j < 1000; j++)
                            {
                                int headYAcceleration = (int)(npc.Center.X / 16f) + Main.rand.Next(-50, 51);
                                int headYTopSpeed;
                                for (headYTopSpeed = (int)(npc.Center.Y / 16f) + Main.rand.Next(-50, 51); headYTopSpeed < Main.maxTilesY - 10 && !WorldGen.SolidTile(headYAcceleration, headYTopSpeed); headYTopSpeed++)
                                {
                                }

                                headYTopSpeed--;
                                if (!WorldGen.SolidTile(headYAcceleration, headYTopSpeed))
                                {
                                    int headXAcceleration = NPC.NewNPC(npc.GetSource_FromAI(), headYAcceleration * 16 + 8, headYTopSpeed * 16, NPCID.DarkCaster);
                                    if (Main.netMode == NetmodeID.Server && headXAcceleration < Main.maxNPCs)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, headXAcceleration);

                                    break;
                                }
                            }
                        }
                    }

                    npc.ai[2] = 0f;
                    npc.ai[1] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;

                    npc.TargetClosest();
                    npc.SyncExtraAI();
                    npc.netUpdate = true;
                }

                npc.rotation += npc.direction * 0.3f;
                Vector2 headSpinPos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float headSpinTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - headSpinPos.X;
                float headSpinTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - headSpinPos.Y;
                float headSpinTargetDist = (float)Math.Sqrt(headSpinTargetX * headSpinTargetX + headSpinTargetY * headSpinTargetY);

                // Increase speed while charging
                npc.damage = (int)(npc.defDamage * 1.3);
                float headSpinVelocityMult = bossRush ? 9f : 4.5f;
                float velocityBoost = death ? 1f : 1f - lifeRatio;
                if (handsDead || bossRush)
                    headSpinVelocityMult += velocityBoost;

                if (headSpinTargetDist > 150f)
                {
                    float baseDistanceVelocityMult = 1f + MathHelper.Clamp((headSpinTargetDist - 150f) * 0.0015f, 0.05f, 1.5f);
                    headSpinVelocityMult *= baseDistanceVelocityMult;
                }

                if (Main.getGoodWorld)
                    headSpinVelocityMult *= 1.3f;

                headSpinTargetDist = headSpinVelocityMult / headSpinTargetDist;
                npc.velocity.X = headSpinTargetX * headSpinTargetDist;
                npc.velocity.Y = headSpinTargetY * headSpinTargetDist;
            }

            // Daytime enrage
            else if (npc.ai[1] == 2f)
            {
                npc.damage = 1000;
                calamityGlobalNPC.DR = 0.9999f;
                calamityGlobalNPC.unbreakableDR = true;
                npc.rotation += npc.direction * 0.3f;
                Vector2 enrageSpinPos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float enrageSpinTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - enrageSpinPos.X;
                float enrageSpinTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - enrageSpinPos.Y;
                float enrageSpinTargetDist = (float)Math.Sqrt(enrageSpinTargetX * enrageSpinTargetX + enrageSpinTargetY * enrageSpinTargetY);
                enrageSpinTargetDist = 8f / enrageSpinTargetDist;
                npc.velocity.X = enrageSpinTargetX * enrageSpinTargetDist;
                npc.velocity.Y = enrageSpinTargetY * enrageSpinTargetDist;
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
            if (npc.ai[1] != 2f && npc.ai[1] != 3f && numHandsAlive != 0)
            {
                int idleDust = Dust.NewDust(new Vector2(npc.position.X + (npc.width / 2) - 15f - npc.velocity.X * 5f, npc.position.Y + npc.height - 2f), 30, 10, 5, -npc.velocity.X * 0.2f, 3f, 0, default, 2f);
                Main.dust[idleDust].noGravity = true;
                Main.dust[idleDust].velocity.X = Main.dust[idleDust].velocity.X * 1.3f;
                Main.dust[idleDust].velocity.X = Main.dust[idleDust].velocity.X + npc.velocity.X * 0.4f;
                Main.dust[idleDust].velocity.Y = Main.dust[idleDust].velocity.Y + (2f + npc.velocity.Y);
                for (int j = 0; j < 2; j++)
                {
                    idleDust = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + 120f), npc.width, 60, 5, npc.velocity.X, npc.velocity.Y, 0, default, 2f);
                    Main.dust[idleDust].noGravity = true;
                    Main.dust[idleDust].velocity -= npc.velocity;
                    Main.dust[idleDust].velocity.Y = Main.dust[idleDust].velocity.Y + 5f;
                }
            }

            return false;
        }

        public static bool BuffedSkeletronHandAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;

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
                        int teleportDust = Dust.NewDust(npc.position, npc.width, npc.height, 27, 0f, 0f, 200, default, 3f);
                        Main.dust[teleportDust].noGravity = true;
                        Main.dust[teleportDust].velocity.X = Main.dust[teleportDust].velocity.X * 2f;
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
                    float maxX = bossRush ? 4f : death ? 6f : 7f;
                    float maxY = bossRush ? 3f : death ? 4.5f : 5f;

                    if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y - 100f * yMultiplier)
                    {
                        if (npc.velocity.Y > 0f)
                            npc.velocity.Y *= 0.94f;
                        npc.velocity.Y -= 0.2f;
                        if (npc.velocity.Y > maxY)
                            npc.velocity.Y = maxY;
                    }
                    else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 100f * yMultiplier)
                    {
                        if (npc.velocity.Y < 0f)
                            npc.velocity.Y *= 0.94f;
                        npc.velocity.Y += 0.2f;
                        if (npc.velocity.Y < -maxY)
                            npc.velocity.Y = -maxY;
                    }

                    if (npc.position.X + (npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 120f * npc.ai[0])
                    {
                        if (npc.velocity.X > 0f)
                            npc.velocity.X *= 0.94f;
                        npc.velocity.X -= 0.2f;
                        if (npc.velocity.X > maxX)
                            npc.velocity.X = maxX;
                    }

                    if (npc.position.X + (npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 120f * npc.ai[0])
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X *= 0.94f;
                        npc.velocity.X += 0.2f;
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

                    float maxX = bossRush ? 4f : death ? 6f : 7f;
                    float maxY = bossRush ? 1f : death ? 2f : 2.5f;

                    if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y + 230f * yMultiplier)
                    {
                        if (npc.velocity.Y > 0f)
                            npc.velocity.Y *= 0.92f;
                        npc.velocity.Y -= 0.2f;
                        if (npc.velocity.Y > maxY)
                            npc.velocity.Y = maxY;
                    }
                    else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y + 230f * yMultiplier)
                    {
                        if (npc.velocity.Y < 0f)
                            npc.velocity.Y *= 0.92f;
                        npc.velocity.Y += 0.2f;
                        if (npc.velocity.Y < -maxY)
                            npc.velocity.Y = -maxY;
                    }

                    if (npc.position.X + (npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0])
                    {
                        if (npc.velocity.X > 0f)
                            npc.velocity.X *= 0.92f;
                        npc.velocity.X -= 0.2f;
                        if (npc.velocity.X > maxX)
                            npc.velocity.X = maxX;
                    }

                    if (npc.position.X + (npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0])
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X *= 0.92f;
                        npc.velocity.X += 0.2f;
                        if (npc.velocity.X < -maxX)
                            npc.velocity.X = -maxX;
                    }
                }

                Vector2 handCurrentPos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float handIdleXPos = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - handCurrentPos.X;
                float handIdleYPos = Main.npc[(int)npc.ai[1]].position.Y + 230f - handCurrentPos.Y;
                float handIdleDist = (float)Math.Sqrt(handIdleXPos * handIdleXPos + handIdleYPos * handIdleYPos);
                npc.rotation = (float)Math.Atan2(handIdleYPos, handIdleXPos) + MathHelper.PiOver2;
                return false;
            }
            if (npc.ai[2] == 1f)
            {
                Vector2 handCurrentPosition = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float handDrawbackXPos = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - handCurrentPosition.X;
                float handDrawbackYPos = Main.npc[(int)npc.ai[1]].position.Y + 230f - handCurrentPosition.Y;
                float handDrawbackDist = (float)Math.Sqrt(handDrawbackXPos * handDrawbackXPos + handDrawbackYPos * handDrawbackYPos);
                npc.rotation = (float)Math.Atan2(handDrawbackYPos, handDrawbackXPos) + MathHelper.PiOver2;
                npc.velocity.X *= 0.95f;
                npc.velocity.Y -= 0.2f;

                if (npc.velocity.Y < -13f)
                    npc.velocity.Y = -13f;

                if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 200f)
                {
                    npc.TargetClosest();
                    npc.ai[2] = 2f;
                    handCurrentPosition = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    handDrawbackXPos = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - handCurrentPosition.X;
                    handDrawbackYPos = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - handCurrentPosition.Y;
                    handDrawbackDist = (float)Math.Sqrt(handDrawbackXPos * handDrawbackXPos + handDrawbackYPos * handDrawbackYPos);
                    handDrawbackDist = 22f / handDrawbackDist;
                    npc.velocity.X = handDrawbackXPos * handDrawbackDist;
                    npc.velocity.Y = handDrawbackYPos * handDrawbackDist;
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
                Vector2 handStrikeCurrentPos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float handStrikeXPos = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - handStrikeCurrentPos.X;
                float handStrikeYPos = Main.npc[(int)npc.ai[1]].position.Y + 230f - handStrikeCurrentPos.Y;
                float handStrikeDist = (float)Math.Sqrt(handStrikeXPos * handStrikeXPos + handStrikeYPos * handStrikeYPos);
                npc.rotation = (float)Math.Atan2(handStrikeYPos, handStrikeXPos) + MathHelper.PiOver2;
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
                    handStrikeCurrentPos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    handStrikeXPos = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - handStrikeCurrentPos.X;
                    handStrikeYPos = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - handStrikeCurrentPos.Y;
                    handStrikeDist = (float)Math.Sqrt(handStrikeXPos * handStrikeXPos + handStrikeYPos * handStrikeYPos);
                    handStrikeDist = 23f / handStrikeDist;
                    npc.velocity.X = handStrikeXPos * handStrikeDist;
                    npc.velocity.Y = handStrikeYPos * handStrikeDist;
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
                            int skull = Projectile.NewProjectile(npc.GetSource_FromAI(), source, velocity, projType, damage, 0f, Main.myPlayer, -1f, 0f);
                            Main.projectile[skull].timeLeft = 300;
                        }
                    }
                }
            }
        }
    }
}
