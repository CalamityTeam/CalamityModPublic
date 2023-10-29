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
    public static class GolemAI
    {
        public static bool BuffedGolemAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            // whoAmI variable
            NPC.golemBoss = npc.whoAmI;

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Phases
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool phase2 = lifeRatio < 0.75f;
            bool phase3 = lifeRatio < 0.5f;
            bool phase4 = lifeRatio < 0.25f;

            // Spawn parts
            if (npc.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[0] = 1f;
                NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X - 84, (int)npc.Center.Y - 9, NPCID.GolemFistLeft);
                NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X + 78, (int)npc.Center.Y - 9, NPCID.GolemFistRight);
                NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X - 3, (int)npc.Center.Y - 57, NPCID.GolemHead);
            }

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            // Despawn
            if (npc.target >= 0 && Main.player[npc.target].dead)
            {
                npc.TargetClosest();
                if (Main.player[npc.target].dead)
                    npc.noTileCollide = true;
            }

            // Enrage if the target isn't inside the temple
            // Turbo enrage if target isn't inside the temple and it's Boss Rush or For the Worthy
            bool enrage = true;
            bool turboEnrage = false;
            if (Main.player[npc.target].Center.Y > Main.worldSurface * 16.0)
            {
                int targetTilePosX = (int)Main.player[npc.target].Center.X / 16;
                int targetTilePosY = (int)Main.player[npc.target].Center.Y / 16;

                Tile tile = Framing.GetTileSafely(targetTilePosX, targetTilePosY);
                if (tile.WallType == WallID.LihzahrdBrickUnsafe)
                    enrage = false;
                else
                    turboEnrage = bossRush || Main.getGoodWorld;
            }
            else
                turboEnrage = bossRush || Main.getGoodWorld;

            if (bossRush || Main.getGoodWorld)
                enrage = true;

            npc.Calamity().CurrentlyEnraged = !bossRush && (enrage || turboEnrage);

            bool reduceFallSpeed = npc.velocity.Y > 0f && Collision.SolidCollision(npc.position + Vector2.UnitY * 1.1f * npc.velocity.Y, npc.width, npc.height);

            // Alpha
            if (npc.alpha > 0)
            {
                npc.alpha -= 10;
                if (npc.alpha < 0)
                    npc.alpha = 0;

                npc.ai[1] = 0f;
            }

            // Check for body parts
            bool headAlive = NPC.AnyNPCs(NPCID.GolemHead);
            bool leftFistAlive = NPC.AnyNPCs(NPCID.GolemFistLeft);
            bool rightFistAlive = NPC.AnyNPCs(NPCID.GolemFistRight);
            npc.dontTakeDamage = (headAlive || leftFistAlive || rightFistAlive) && !CalamityWorld.LegendaryMode;

            // Phase 2, check for free head
            bool freedHeadAlive = NPC.AnyNPCs(NPCID.GolemHeadFree);

            // Deactivate torches
            if (Main.netMode != NetmodeID.MultiplayerClient && Main.getGoodWorld && npc.velocity.Y > 0f)
            {
                for (int j = (int)(npc.position.X / 16f); (float)j < (npc.position.X + (float)npc.width) / 16f; j++)
                {
                    for (int k = (int)(npc.position.Y / 16f); (float)k < (npc.position.Y + (float)npc.width) / 16f; k++)
                    {
                        if (Main.tile[j, k].TileType == TileID.Torches)
                        {
                            Main.tile[j, k].Get<TileWallWireStateData>().HasTile = false;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendTileSquare(-1, j, k);
                        }
                    }
                }
            }

            // Spawn arm dust
            if (!Main.getGoodWorld)
            {
                if (!leftFistAlive)
                {
                    int lostLeftFistDust = Dust.NewDust(new Vector2(npc.Center.X - 80f, npc.Center.Y - 9f), 8, 8, 31, 0f, 0f, 100, default, 1f);
                    Dust dust = Main.dust[lostLeftFistDust];
                    dust.alpha += Main.rand.Next(100);
                    dust.velocity *= 0.2f;
                    dust.velocity.Y -= 0.5f + Main.rand.Next(10) * 0.1f;
                    dust.fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;

                    if (Main.rand.NextBool(10))
                    {
                        lostLeftFistDust = Dust.NewDust(new Vector2(npc.Center.X - 80f, npc.Center.Y - 9f), 8, 8, 6, 0f, 0f, 0, default, 1f);
                        if (Main.rand.Next(20) != 0)
                        {
                            Main.dust[lostLeftFistDust].noGravity = true;
                            dust = Main.dust[lostLeftFistDust];
                            dust.scale *= 1f + Main.rand.Next(10) * 0.1f;
                            dust.velocity.Y -= 1f;
                        }
                    }
                }
                if (!rightFistAlive)
                {
                    int lostRightFistDust = Dust.NewDust(new Vector2(npc.Center.X + 62f, npc.Center.Y - 9f), 8, 8, 31, 0f, 0f, 100, default, 1f);
                    Dust dust = Main.dust[lostRightFistDust];
                    dust.alpha += Main.rand.Next(100);
                    dust.velocity *= 0.2f;
                    dust.velocity.Y -= 0.5f + Main.rand.Next(10) * 0.1f;
                    dust.fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;

                    if (Main.rand.NextBool(10))
                    {
                        lostRightFistDust = Dust.NewDust(new Vector2(npc.Center.X + 62f, npc.Center.Y - 9f), 8, 8, 6, 0f, 0f, 0, default, 1f);
                        if (Main.rand.Next(20) != 0)
                        {
                            Main.dust[lostRightFistDust].noGravity = true;
                            dust = Main.dust[lostRightFistDust];
                            dust.scale *= 1f + Main.rand.Next(10) * 0.1f;
                            dust.velocity.Y -= 1f;
                        }
                    }
                }
            }

            if (npc.noTileCollide && !Main.player[npc.target].dead)
            {
                if (npc.velocity.Y > 0f && npc.Bottom.Y > Main.player[npc.target].Top.Y)
                    npc.noTileCollide = false;
                else if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].Center, 1, 1) && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                    npc.noTileCollide = false;
            }

            // Jump
            if (npc.ai[0] == 0f)
            {
                if (npc.velocity.Y == 0f)
                {
                    // Laser fire when head is dead
                    if (Main.netMode != NetmodeID.MultiplayerClient && (!headAlive || turboEnrage || CalamityWorld.LegendaryMode))
                    {
                        npc.localAI[1] += 1f;

                        float divisor = 15f -
                            (phase2 ? 4f : 0f) -
                            (phase3 ? 3f : 0f) -
                            (phase4 ? 2f : 0f);

                        if (enrage)
                            divisor = 5f;

                        if (turboEnrage && Main.getGoodWorld)
                            divisor = 2f;

                        Vector2 projectileFirePos = new Vector2(npc.Center.X, npc.Center.Y - 60f);
                        if (npc.localAI[1] % divisor == 0f && (Vector2.Distance(Main.player[npc.target].Center, projectileFirePos) > 160f || !freedHeadAlive))
                        {
                            float laserSpeed = turboEnrage ? 12f : enrage ? 9f : 6f;
                            float laserTargetXDist = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - projectileFirePos.X;
                            float laserTargetYDist = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - projectileFirePos.Y;
                            float laserTargetDistance = (float)Math.Sqrt(laserTargetXDist * laserTargetXDist + laserTargetYDist * laserTargetYDist);

                            laserTargetDistance = laserSpeed / laserTargetDistance;
                            laserTargetXDist *= laserTargetDistance;
                            laserTargetYDist *= laserTargetDistance;
                            projectileFirePos.X += laserTargetXDist * 3f;
                            projectileFirePos.Y += laserTargetYDist * 3f;

                            int type = ProjectileID.EyeBeam;
                            int damage = npc.GetProjectileDamage(type);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int bodyLaser = Projectile.NewProjectile(npc.GetSource_FromAI(), projectileFirePos.X, projectileFirePos.Y, laserTargetXDist, laserTargetYDist, type, damage, 0f, Main.myPlayer, 0f, 0f);
                                Main.projectile[bodyLaser].timeLeft = 480;
                                if (turboEnrage && Main.getGoodWorld)
                                    Main.projectile[bodyLaser].extraUpdates += 1;
                            }
                        }

                        if (npc.localAI[1] >= 15f)
                            npc.localAI[1] = 0f;
                    }

                    // Slow down
                    npc.velocity.X *= 0.8f;

                    // Delay before jumping
                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 0f)
                    {
                        npc.ai[1] += 1f;
                        if (Main.getGoodWorld)
                            npc.ai[1] += 100f;
                        if (enrage || death)
                            npc.ai[1] += 18f;
                        else
                        {
                            if (!leftFistAlive)
                                npc.ai[1] += 6f;
                            if (!rightFistAlive)
                                npc.ai[1] += 6f;
                            if (!headAlive)
                                npc.ai[1] += 6f;
                        }
                    }
                    if (npc.ai[1] >= 300f)
                    {
                        npc.ai[1] = -20f;
                        npc.frameCounter = 0.0;
                    }
                    else if (npc.ai[1] == -1f)
                    {
                        // Set jump velocity
                        npc.TargetClosest();

                        float velocityBoost = death ? 12f * (1f - lifeRatio) : 8f * (1f - lifeRatio);
                        float velocityX = 4f + velocityBoost;
                        if (enrage)
                            velocityX *= 1.5f;

                        float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
                        npc.direction = playerLocation < 0 ? 1 : -1;
                        calamityGlobalNPC.newAI[1] = npc.direction;

                        npc.velocity.X = velocityX * npc.direction;

                        float distanceBelowTarget = npc.position.Y - (Main.player[npc.target].position.Y + 80f);
                        float speedMult = 1f;

                        float multiplier = turboEnrage ? 0.0025f : enrage ? 0.002f : 0.0015f;
                        if (distanceBelowTarget > 0f && ((!leftFistAlive && !rightFistAlive) || turboEnrage || CalamityWorld.LegendaryMode))
                            speedMult += distanceBelowTarget * multiplier;

                        float speedMultLimit = turboEnrage ? 3.5f : enrage ? 3f : 2.5f;
                        if (speedMult > speedMultLimit)
                            speedMult = speedMultLimit;

                        if (Main.player[npc.target].position.Y < npc.Bottom.Y)
                            npc.velocity.Y = ((((!freedHeadAlive && !headAlive) || turboEnrage || CalamityWorld.LegendaryMode) ? -15.1f : -12.1f) + (enrage ? -4f : 0f)) * speedMult;
                        else
                            npc.velocity.Y = 1f;

                        npc.noTileCollide = true;

                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;

                        npc.netUpdate = true;
                        npc.SyncExtraAI();
                    }
                }

                // Don't run custom gravity when starting a jump
                if (npc.ai[0] != 1f)
                    CustomGravity();
            }

            // Fall down
            else if (npc.ai[0] == 1f)
            {
                if (npc.velocity.Y == 0f)
                {
                    npc.TargetClosest();

                    // Play sound
                    SoundEngine.PlaySound(SoundID.Item14, npc.position);

                    npc.ai[0] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;
                    npc.SyncExtraAI();

                    // Dust and gore
                    for (int i = (int)npc.position.X - 20; i < (int)npc.position.X + npc.width + 40; i += 20)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            int fallDust = Dust.NewDust(new Vector2(npc.position.X - 20f, npc.position.Y + npc.height), npc.width + 20, 4, 31, 0f, 0f, 100, default, 1.5f);
                            Dust dust = Main.dust[fallDust];
                            dust.velocity *= 0.2f;
                        }
                        if (Main.netMode != NetmodeID.Server)
                        {
                            int fallGore = Gore.NewGore(npc.GetSource_FromAI(), new Vector2(i - 20, npc.position.Y + npc.height - 8f), default, Main.rand.Next(61, 64), 1f);
                            Gore gore = Main.gore[fallGore];
                            gore.velocity *= 0.4f;
                        }
                    }

                    // Fireball explosion when head is dead
                    if (Main.netMode != NetmodeID.MultiplayerClient && (!headAlive || turboEnrage || CalamityWorld.LegendaryMode))
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            int fiery = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                            Main.dust[fiery].velocity.Y *= 6f;
                            Main.dust[fiery].velocity.X *= 3f;
                            if (Main.rand.NextBool())
                            {
                                Main.dust[fiery].scale = 0.5f;
                                Main.dust[fiery].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                        }
                        for (int j = 0; j < 20; j++)
                        {
                            int fiery2 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                            Main.dust[fiery2].noGravity = true;
                            Main.dust[fiery2].velocity.Y *= 10f;
                            fiery2 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                            Main.dust[fiery2].velocity.X *= 2f;
                        }

                        int totalFireballs = 5;
                        if (turboEnrage && Main.getGoodWorld)
                            totalFireballs *= 2;

                        int spawnX = npc.width / 2;
                        for (int i = 0; i < totalFireballs; i++)
                        {
                            Vector2 spawnVector = new Vector2(npc.Center.X + Main.rand.Next(-spawnX, spawnX), npc.Center.Y + npc.height / 2 * 0.8f);
                            Vector2 velocity = new Vector2(Main.rand.NextBool() ? Main.rand.Next(4, 6) : Main.rand.Next(-5, -3), Main.rand.Next(-1, 2));

                            if (death)
                                velocity *= 1.5f;

                            if (enrage)
                                velocity *= 1.25f;

                            if (turboEnrage)
                                velocity *= 1.25f;

                            int type = ProjectileID.Fireball;
                            int damage = npc.GetProjectileDamage(type);
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), spawnVector, velocity, type, damage, 0f, Main.myPlayer);
                            Main.projectile[proj].timeLeft = 240;
                            if (turboEnrage && Main.getGoodWorld)
                                Main.projectile[proj].extraUpdates += 1;
                        }

                        npc.netUpdate = true;
                    }
                }
                else
                {
                    // Velocity when falling
                    if (npc.position.X < Main.player[npc.target].position.X && npc.position.X + npc.width > Main.player[npc.target].position.X + Main.player[npc.target].width)
                    {
                        npc.velocity.X *= 0.8f;

                        if (npc.Bottom.Y < Main.player[npc.target].position.Y)
                        {
                            float fallSpeedBoost = death ? 1.2f * (1f - lifeRatio) : 0.8f * (1f - lifeRatio);
                            float fallSpeed = 0.2f + fallSpeedBoost;
                            if (enrage)
                                fallSpeed *= 1.5f;

                            npc.velocity.Y += fallSpeed;
                        }
                    }
                    else
                    {
                        float velocityXChange = death ? 0.3f : 0.2f;
                        if (npc.direction < 0)
                            npc.velocity.X -= velocityXChange;
                        else if (npc.direction > 0)
                            npc.velocity.X += velocityXChange;

                        float velocityBoost = death ? 9f * (1f - lifeRatio) : 6f * (1f - lifeRatio);
                        float velocityXCap = 3f + velocityBoost;
                        if (enrage)
                            velocityXCap *= 1.5f;

                        float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
                        int directionRelativeToTarget = playerLocation < 0 ? 1 : -1;
                        bool slowDown = directionRelativeToTarget != calamityGlobalNPC.newAI[1];

                        if (slowDown)
                            velocityXCap *= 0.5f;

                        if (npc.velocity.X < -velocityXCap)
                            npc.velocity.X = -velocityXCap;
                        if (npc.velocity.X > velocityXCap)
                            npc.velocity.X = velocityXCap;
                    }

                    CustomGravity();
                }
            }

            void CustomGravity()
            {
                float gravity = turboEnrage ? (Main.getGoodWorld ? 1.2f : 0.9f) : enrage ? 0.6f : (!leftFistAlive && !rightFistAlive) ? 0.45f : 0.3f;
                float maxFallSpeed = reduceFallSpeed ? 12f : turboEnrage ? (Main.getGoodWorld ? 40f : 30f) : enrage ? 20f : (!leftFistAlive && !rightFistAlive) ? 15f : 10f;

                npc.velocity.Y += gravity;
                if (npc.velocity.Y > maxFallSpeed)
                    npc.velocity.Y = maxFallSpeed;
            }

            // Despawn
            int despawnDistance = turboEnrage ? 7500 : enrage ? 6000 : 4500;
            if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) + Math.Abs(npc.Center.Y - Main.player[npc.target].Center.Y) > despawnDistance)
            {
                npc.TargetClosest();

                if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) + Math.Abs(npc.Center.Y - Main.player[npc.target].Center.Y) > despawnDistance)
                {
                    npc.active = false;
                    npc.netUpdate = true;
                }
            }

            return false;
        }

        public static bool BuffedGolemFistAI(NPC npc, Mod mod)
        {
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;

            // Enrage if the target isn't inside the temple
            // Turbo enrage if target isn't inside the temple and it's Boss Rush or For the Worthy
            bool enrage = true;
            bool turboEnrage = false;
            if (Main.player[npc.target].Center.Y > Main.worldSurface * 16.0)
            {
                int targetTilePosX = (int)Main.player[npc.target].Center.X / 16;
                int targetTilePosY = (int)Main.player[npc.target].Center.Y / 16;

                Tile tile = Framing.GetTileSafely(targetTilePosX, targetTilePosY);
                if (tile.WallType == WallID.LihzahrdBrickUnsafe)
                    enrage = false;
                else
                    turboEnrage = bossRush || Main.getGoodWorld;
            }
            else
                turboEnrage = bossRush || Main.getGoodWorld;

            if (bossRush || Main.getGoodWorld)
                enrage = true;

            float aggression = turboEnrage ? (Main.getGoodWorld ? 4f : 3f) : enrage ? 2f : death ? 1.5f : 1f;

            if (NPC.golemBoss < 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    npc.StrikeInstantKill();

                return false;
            }

            if (npc.alpha > 0)
            {
                npc.alpha -= 10;
                if (npc.alpha < 0)
                    npc.alpha = 0;

                npc.ai[1] = 0f;
            }

            if (npc.ai[0] == 0f)
            {
                npc.noTileCollide = true;

                float fistSpeed = 25f;
                fistSpeed *= (aggression + 3f) / 4f;

                Vector2 fistCenter = new Vector2(npc.Center.X, npc.Center.Y);
                float fistXPos = Main.npc[NPC.golemBoss].Center.X - fistCenter.X;
                float fistYPos = Main.npc[NPC.golemBoss].Center.Y - fistCenter.Y;
                fistYPos -= 9f;
                fistXPos = (npc.type != NPCID.GolemFistLeft) ? (fistXPos + 78f) : (fistXPos - 84f);
                if (Main.getGoodWorld)
                    fistXPos = (npc.type != NPCID.GolemFistLeft) ? (fistXPos - 40f) : (fistXPos + 40f);

                float fistRestDistance = (float)Math.Sqrt(fistXPos * fistXPos + fistYPos * fistYPos);
                if (fistRestDistance < 12f + fistSpeed)
                {
                    npc.rotation = 0f;
                    npc.velocity.X = fistXPos;
                    npc.velocity.Y = fistYPos;

                    float fistShootSpeed = aggression;
                    npc.ai[1] += fistShootSpeed;
                    if (npc.life < npc.lifeMax / 2)
                        npc.ai[1] += fistShootSpeed;
                    if (npc.life < npc.lifeMax / 4)
                        npc.ai[1] += fistShootSpeed;

                    if (npc.ai[1] >= 40f)
                    {
                        npc.TargetClosest();

                        if ((npc.type == NPCID.GolemFistLeft && npc.Center.X + 100f > Main.player[npc.target].Center.X) || (npc.type == NPCID.GolemFistRight && npc.Center.X - 100f < Main.player[npc.target].Center.X))
                        {
                            npc.ai[1] = 0f;
                            npc.ai[0] = 1f;
                        }
                        else
                            npc.ai[1] = 0f;
                    }
                }
                else
                {
                    fistRestDistance = fistSpeed / fistRestDistance;
                    npc.velocity.X = fistXPos * fistRestDistance;
                    npc.velocity.Y = fistYPos * fistRestDistance;

                    npc.rotation = (float)Math.Atan2(0f - npc.velocity.Y, 0f - npc.velocity.X);
                    if (npc.type == NPCID.GolemFistLeft)
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);
                }
            }
            else if (npc.ai[0] == 1f)
            {
                Vector2 fistCenter = new Vector2(npc.Center.X, npc.Center.Y);
                float fistXPos = Main.npc[NPC.golemBoss].Center.X - fistCenter.X;
                float fistYPos = Main.npc[NPC.golemBoss].Center.Y - fistCenter.Y;
                fistYPos -= 9f;
                fistXPos = (npc.type != NPCID.GolemFistLeft) ? (fistXPos + 78f) : (fistXPos - 84f);
                if (Main.getGoodWorld)
                    fistXPos = (npc.type != NPCID.GolemFistLeft) ? (fistXPos - 40f) : (fistXPos + 40f);

                npc.ai[1] += 1f;
                npc.Center = new Vector2(fistXPos, fistYPos);
                npc.rotation = 0f;
                npc.velocity = Vector2.Zero;
                if (npc.ai[1] <= 15f)
                {
                    for (int i = 0; i < 1; i++)
                    {
                        Vector2 largeRandDustRadius = Main.rand.NextVector2Circular(80f, 80f);
                        Vector2 largeRandDustRecoil = largeRandDustRadius * -1f * 0.05f;
                        Vector2 smallRandDustRadius = Main.rand.NextVector2Circular(20f, 20f);
                        Dust dust = Dust.NewDustPerfect(npc.Center + largeRandDustRecoil + largeRandDustRadius + smallRandDustRadius, 228, largeRandDustRecoil);
                        dust.fadeIn = 1.5f;
                        dust.scale = 0.5f;
                        if (Main.getGoodWorld)
                            dust.noLight = true;

                        dust.noGravity = true;
                    }
                }

                if (npc.ai[1] >= 30f)
                {
                    npc.noTileCollide = true;
                    npc.collideX = false;
                    npc.collideY = false;

                    float fistReturnSpeed = 20f;
                    fistReturnSpeed *= (aggression + 3f) / 4f;
                    if (fistReturnSpeed > 48f)
                        fistReturnSpeed = 48f;

                    Vector2 fistCent = new Vector2(npc.Center.X, npc.Center.Y);
                    float fistTargetXDist = Main.player[npc.target].Center.X - fistCent.X;
                    float fistTargetYDist = Main.player[npc.target].Center.Y - fistCent.Y;
                    float fistTargetDistance = (float)Math.Sqrt(fistTargetXDist * fistTargetXDist + fistTargetYDist * fistTargetYDist);
                    fistTargetDistance = fistReturnSpeed / fistTargetDistance;
                    npc.velocity.X = fistTargetXDist * fistTargetDistance;
                    npc.velocity.Y = fistTargetYDist * fistTargetDistance;
                    npc.ai[0] = 2f;
                    npc.ai[1] = 0f;

                    npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);
                    if (npc.type == NPCID.GolemFistLeft)
                        npc.rotation = (float)Math.Atan2(0f - npc.velocity.Y, 0f - npc.velocity.X);
                }
            }
            else if (npc.ai[0] == 2f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && Main.getGoodWorld)
                {
                    for (int j = (int)(npc.position.X / 16f) - 1; (float)j < (npc.position.X + (float)npc.width) / 16f + 1f; j++)
                    {
                        for (int k = (int)(npc.position.Y / 16f) - 1; (float)k < (npc.position.Y + (float)npc.width) / 16f + 1f; k++)
                        {
                            if (Main.tile[j, k].TileType == TileID.Torches)
                            {
                                Main.tile[j, k].Get<TileWallWireStateData>().HasTile = false;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendTileSquare(-1, j, k);
                            }
                        }
                    }
                }

                npc.ai[1] += 1f;
                if (npc.ai[1] == 1f)
                    SoundEngine.PlaySound(SoundID.Item14, npc.Center);

                if (Main.rand.NextBool())
                {
                    Vector2 halfVelocityDust = npc.velocity * 0.5f;
                    Vector2 randDustRadius = Main.rand.NextVector2Circular(20f, 20f);
                    Dust.NewDustPerfect(npc.Center + halfVelocityDust + randDustRadius, 306, halfVelocityDust, 0, Main.OurFavoriteColor).scale = 2f;
                }

                if (Math.Abs(npc.velocity.X) > Math.Abs(npc.velocity.Y))
                {
                    if (npc.velocity.X > 0f && npc.Center.X > Main.player[npc.target].Center.X)
                        npc.noTileCollide = false;

                    if (npc.velocity.X < 0f && npc.Center.X < Main.player[npc.target].Center.X)
                        npc.noTileCollide = false;
                }
                else
                {
                    if (npc.velocity.Y > 0f && npc.Center.Y > Main.player[npc.target].Center.Y)
                        npc.noTileCollide = false;

                    if (npc.velocity.Y < 0f && npc.Center.Y < Main.player[npc.target].Center.Y)
                        npc.noTileCollide = false;
                }

                Vector2 projectileFirePos = new Vector2(npc.Center.X, npc.Center.Y);
                float fistGolemDistanceX = Main.npc[NPC.golemBoss].Center.X - projectileFirePos.X;
                float fistGolemDistanceY = Main.npc[NPC.golemBoss].Center.Y - projectileFirePos.Y;
                fistGolemDistanceX += Main.npc[NPC.golemBoss].velocity.X;
                fistGolemDistanceY += Main.npc[NPC.golemBoss].velocity.Y;
                fistGolemDistanceY -= 9f;
                fistGolemDistanceX = (npc.type != NPCID.GolemFistLeft) ? (fistGolemDistanceX + 78f) : (fistGolemDistanceX - 84f);
                float fistGolemDistance = (float)Math.Sqrt(fistGolemDistanceX * fistGolemDistanceX + fistGolemDistanceY * fistGolemDistanceY);

                if (npc.life < npc.lifeMax / 4)
                {
                    npc.knockBackResist = 0f;

                    if (fistGolemDistance > 700f || npc.collideX || npc.collideY)
                    {
                        npc.noTileCollide = true;
                        npc.ai[0] = 0f;
                    }

                    return false;
                }

                bool leftFistAlive = npc.justHit;
                if (leftFistAlive)
                {
                    if (npc.life < npc.lifeMax / 2)
                    {
                        if (npc.knockBackResist == 0f)
                            leftFistAlive = false;

                        npc.knockBackResist = 0f;
                    }
                }

                if ((fistGolemDistance > 600f || npc.collideX || npc.collideY) | leftFistAlive)
                {
                    npc.noTileCollide = true;
                    npc.ai[0] = 0f;
                }
            }
            else
            {
                if (npc.ai[0] != 3f)
                    return false;

                npc.noTileCollide = true;
                float fistAcceleration = 0.4f;
                Vector2 returningFistCenter = new Vector2(npc.Center.X, npc.Center.Y);
                float returningTargetX = Main.player[npc.target].Center.X - returningFistCenter.X;
                float returningTargetY = Main.player[npc.target].Center.Y - returningFistCenter.Y;
                float returningTargetDist = (float)Math.Sqrt(returningTargetX * returningTargetX + returningTargetY * returningTargetY);
                returningTargetDist = 12f / returningTargetDist;
                returningTargetX *= returningTargetDist;
                returningTargetY *= returningTargetDist;

                if (npc.velocity.X < returningTargetX)
                {
                    npc.velocity.X += fistAcceleration;
                    if (npc.velocity.X < 0f && returningTargetX > 0f)
                        npc.velocity.X += fistAcceleration * 2f;
                }
                else if (npc.velocity.X > returningTargetX)
                {
                    npc.velocity.X -= fistAcceleration;
                    if (npc.velocity.X > 0f && returningTargetX < 0f)
                        npc.velocity.X -= fistAcceleration * 2f;
                }

                if (npc.velocity.Y < returningTargetY)
                {
                    npc.velocity.Y += fistAcceleration;
                    if (npc.velocity.Y < 0f && returningTargetY > 0f)
                        npc.velocity.Y += fistAcceleration * 2f;
                }
                else if (npc.velocity.Y > returningTargetY)
                {
                    npc.velocity.Y -= fistAcceleration;
                    if (npc.velocity.Y > 0f && returningTargetY < 0f)
                        npc.velocity.Y -= fistAcceleration * 2f;
                }

                npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);
                if (npc.type == NPCID.GolemFistLeft)
                    npc.rotation = (float)Math.Atan2(0f - npc.velocity.Y, 0f - npc.velocity.X);
            }

            return false;
        }


        public static bool BuffedGolemHeadAI(NPC npc, Mod mod)
        {
            // Don't collide
            npc.noTileCollide = true;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            // Die if body is gone
            if (NPC.golemBoss < 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    npc.StrikeInstantKill();

                return false;
            }

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;

            // Count body parts
            bool leftFistAlive = NPC.AnyNPCs(NPCID.GolemFistLeft);
            bool rightFistAlive = NPC.AnyNPCs(NPCID.GolemFistRight);
            npc.dontTakeDamage = (leftFistAlive || rightFistAlive) && !CalamityWorld.LegendaryMode;

            // Stay in position on top of body
            npc.Center = Main.npc[NPC.golemBoss].Center - new Vector2(Main.getGoodWorld ? 2f : 3f, Main.getGoodWorld ? 37f : 57f);

            // Enrage if the target isn't inside the temple
            bool enrage = true;
            bool turboEnrage = false;
            if (Main.player[npc.target].Center.Y > Main.worldSurface * 16.0)
            {
                int targetTilePosX = (int)Main.player[npc.target].Center.X / 16;
                int targetTilePosY = (int)Main.player[npc.target].Center.Y / 16;

                Tile tile = Framing.GetTileSafely(targetTilePosX, targetTilePosY);
                if (tile.WallType == WallID.LihzahrdBrickUnsafe)
                    enrage = false;
                else
                    turboEnrage = bossRush || Main.getGoodWorld;
            }
            else
                turboEnrage = bossRush || Main.getGoodWorld;

            if (bossRush || Main.getGoodWorld)
                enrage = true;

            // Alpha
            if (npc.alpha > 0)
            {
                npc.alpha -= 10;
                if (npc.alpha < 0)
                    npc.alpha = 0;

                npc.ai[1] = 30f;
            }

            // Spit fireballs if arms are alive
            if (npc.ai[0] == 0f)
            {
                npc.ai[1] += 1f;
                if (npc.ai[1] < 20f || npc.ai[1] > 130)
                    npc.localAI[0] = 1f;
                else
                    npc.localAI[0] = 0f;

                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[1] >= 150)
                {
                    npc.TargetClosest();

                    npc.ai[1] = 0f;

                    Vector2 headCent = new Vector2(npc.Center.X, npc.Center.Y + 10f);
                    float headFireballSpeed = turboEnrage ? 12f : enrage ? 10f : 8f;
                    float headFireballTargetX = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - headCent.X;
                    float headFireballTargetY = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - headCent.Y;
                    float headFireballTargetDist = (float)Math.Sqrt(headFireballTargetX * headFireballTargetX + headFireballTargetY * headFireballTargetY);

                    headFireballTargetDist = headFireballSpeed / headFireballTargetDist;
                    headFireballTargetX *= headFireballTargetDist;
                    headFireballTargetY *= headFireballTargetDist;

                    int type = ProjectileID.Fireball;
                    int damage = npc.GetProjectileDamage(type);
                    Projectile.NewProjectile(npc.GetSource_FromAI(), headCent.X, headCent.Y, headFireballTargetX, headFireballTargetY, type, damage, 0f, Main.myPlayer, 0f, 0f);

                    npc.netUpdate = true;
                }
            }

            // Shoot lasers and fireballs if arms are dead
            else if (npc.ai[0] == 1f)
            {
                // Fire projectiles from eye positions
                Vector2 projectileFirePos = new Vector2(npc.Center.X, npc.Center.Y + 10f);
                if (Main.player[npc.target].Center.X < npc.Center.X - npc.width)
                {
                    npc.localAI[1] = -1f;
                    projectileFirePos.X -= 40f;
                }
                else if (Main.player[npc.target].Center.X > npc.Center.X + npc.width)
                {
                    npc.localAI[1] = 1f;
                    projectileFirePos.X += 40f;
                }
                else
                    npc.localAI[1] = 0f;

                // Fireballs
                float shootBoost = death ? 3f * (1f - lifeRatio) : 2f * (1f - lifeRatio);
                npc.ai[1] += 1f + shootBoost;

                if (npc.ai[1] < 20f || npc.ai[1] > 220)
                    npc.localAI[0] = 1f;
                else
                    npc.localAI[0] = 0f;

                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[1] >= 240)
                {
                    npc.TargetClosest();

                    npc.ai[1] = 0f;

                    float fireballSpeedFistsDed = turboEnrage ? 16f : enrage ? 14f : 12f;
                    float fireballFistsDedTargetX = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - projectileFirePos.X;
                    float fireballFistsDedTargetY = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - projectileFirePos.Y;
                    float fireballFistsDedTargetDist = (float)Math.Sqrt(fireballFistsDedTargetX * fireballFistsDedTargetX + fireballFistsDedTargetY * fireballFistsDedTargetY);

                    fireballFistsDedTargetDist = fireballSpeedFistsDed / fireballFistsDedTargetDist;
                    fireballFistsDedTargetX *= fireballFistsDedTargetDist;
                    fireballFistsDedTargetY *= fireballFistsDedTargetDist;

                    int type = ProjectileID.Fireball;
                    int damage = npc.GetProjectileDamage(type);
                    Projectile.NewProjectile(npc.GetSource_FromAI(), projectileFirePos.X, projectileFirePos.Y, fireballFistsDedTargetX, fireballFistsDedTargetY, type, damage, 0f, Main.myPlayer, 0f, 0f);

                    npc.netUpdate = true;
                }

                // Lasers
                float shootBoost2 = death ? 5f * (1f - lifeRatio) : 3f * (1f - lifeRatio);
                npc.ai[2] += 1f + shootBoost2;
                if (enrage)
                    npc.ai[2] += 4f;
                if (!Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                    npc.ai[2] += 8f;

                if (npc.ai[2] >= 300f)
                {
                    npc.TargetClosest();

                    npc.ai[2] = 0f;

                    int projType = ProjectileID.EyeBeam;
                    int dmg = npc.GetProjectileDamage(projType);

                    if (npc.localAI[1] == 0f)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            projectileFirePos = new Vector2(npc.Center.X, npc.Center.Y - 22f);
                            if (i == 0)
                                projectileFirePos.X -= 18f;
                            else
                                projectileFirePos.X += 18f;

                            float laserSpeed = 9f;
                            if (!Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                                laserSpeed = 14f;

                            float laserTargetXDist = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - projectileFirePos.X;
                            float laserTargetYDist = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - projectileFirePos.Y;
                            float laserTargetDistance = (float)Math.Sqrt(laserTargetXDist * laserTargetXDist + laserTargetYDist * laserTargetYDist);

                            laserTargetDistance = laserSpeed / laserTargetDistance;
                            laserTargetXDist *= laserTargetDistance;
                            laserTargetYDist *= laserTargetDistance;
                            projectileFirePos.X += laserTargetXDist * 3f;
                            projectileFirePos.Y += laserTargetYDist * 3f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int bodyLaser = Projectile.NewProjectile(npc.GetSource_FromAI(), projectileFirePos.X, projectileFirePos.Y, laserTargetXDist, laserTargetYDist, projType, dmg, 0f, Main.myPlayer, 0f, 0f);
                                Main.projectile[bodyLaser].timeLeft = enrage ? 480 : 300;
                                if (turboEnrage && Main.getGoodWorld)
                                    Main.projectile[bodyLaser].extraUpdates += 1;

                                npc.netUpdate = true;
                            }
                        }
                    }
                    else if (npc.localAI[1] != 0f)
                    {
                        projectileFirePos = new Vector2(npc.Center.X, npc.Center.Y - 22f);
                        if (npc.localAI[1] == -1f)
                            projectileFirePos.X -= 30f;
                        else if (npc.localAI[1] == 1f)
                            projectileFirePos.X += 30f;

                        float extraLaserSpeed = 9f;
                        if (!Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                            extraLaserSpeed = 14f;

                        float extraLaserTargetX = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - projectileFirePos.X;
                        float extraLaserTargetY = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - projectileFirePos.Y;
                        float extraLaserTargetDist = (float)Math.Sqrt(extraLaserTargetX * extraLaserTargetX + extraLaserTargetY * extraLaserTargetY);

                        extraLaserTargetDist = extraLaserSpeed / extraLaserTargetDist;
                        extraLaserTargetX *= extraLaserTargetDist;
                        extraLaserTargetY *= extraLaserTargetDist;
                        projectileFirePos.X += extraLaserTargetX * 3f;
                        projectileFirePos.Y += extraLaserTargetY * 3f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int extraLasers = Projectile.NewProjectile(npc.GetSource_FromAI(), projectileFirePos.X, projectileFirePos.Y, extraLaserTargetX, extraLaserTargetY, projType, dmg, 0f, Main.myPlayer, 0f, 0f);
                            Main.projectile[extraLasers].timeLeft = enrage ? 480 : 300;
                            if (turboEnrage && Main.getGoodWorld)
                                Main.projectile[extraLasers].extraUpdates += 1;

                            npc.netUpdate = true;
                        }
                    }
                }
            }

            // Laser fire if arms are dead
            if ((!leftFistAlive && !rightFistAlive) || death || CalamityWorld.LegendaryMode)
            {
                npc.ai[0] = 1f;
                return false;
            }
            npc.ai[0] = 0f;

            return false;
        }

        public static bool BuffedGolemHeadFreeAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            // Die if body is gone
            if (NPC.golemBoss < 0)
            {
                calamityGlobalNPC.DR = 0.25f;
                calamityGlobalNPC.unbreakableDR = false;
                calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = false;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    npc.StrikeInstantKill();

                return false;
            }

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;
            float golemLifeRatio = Main.npc[NPC.golemBoss].life / (float)Main.npc[NPC.golemBoss].lifeMax;

            // Phases
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool phase2 = lifeRatio < 0.7f || golemLifeRatio < 0.85f;
            bool phase3 = lifeRatio < 0.55f || golemLifeRatio < 0.7f;
            bool phase4 = lifeRatio < 0.4f || golemLifeRatio < 0.55f;

            // Enrage if the target isn't inside the temple
            bool enrage = true;
            bool turboEnrage = false;
            if (Main.player[npc.target].Center.Y > Main.worldSurface * 16.0)
            {
                int targetTilePosX = (int)Main.player[npc.target].Center.X / 16;
                int targetTilePosY = (int)Main.player[npc.target].Center.Y / 16;

                Tile tile = Framing.GetTileSafely(targetTilePosX, targetTilePosY);
                if (tile.WallType == WallID.LihzahrdBrickUnsafe)
                    enrage = false;
                else
                    turboEnrage = bossRush || Main.getGoodWorld;
            }
            else
                turboEnrage = bossRush || Main.getGoodWorld;

            if (bossRush || Main.getGoodWorld)
                enrage = true;

            if (turboEnrage)
            {
                calamityGlobalNPC.DR = 0.9999f;
                calamityGlobalNPC.unbreakableDR = true;
                calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = true;
            }

            // Float through tiles or not
            bool canPassThroughTiles = false;
            if (!Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1) || phase3 || turboEnrage)
            {
                npc.noTileCollide = true;
                canPassThroughTiles = true;
            }
            else
                npc.noTileCollide = false;

            // Move to new location
            if (npc.ai[3] <= 0f)
            {
                npc.ai[3] = 300f;

                float maxDistance = 300f;

                // Four corners around target
                if (phase3 || turboEnrage)
                {
                    if (calamityGlobalNPC.newAI[1] == -maxDistance)
                    {
                        switch ((int)calamityGlobalNPC.newAI[0])
                        {
                            case 0:
                            case 300:
                                calamityGlobalNPC.newAI[0] = -maxDistance;
                                break;
                            case -300:
                                calamityGlobalNPC.newAI[1] = maxDistance;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch ((int)calamityGlobalNPC.newAI[0])
                        {
                            case 0:
                            case -300:
                                calamityGlobalNPC.newAI[0] = maxDistance;
                                break;
                            case 300:
                                calamityGlobalNPC.newAI[1] = -maxDistance;
                                break;
                            default:
                                break;
                        }
                    }
                }

                // Above target
                else if (phase2)
                {
                    switch ((int)calamityGlobalNPC.newAI[0])
                    {
                        case 0:
                            calamityGlobalNPC.newAI[0] = maxDistance;
                            break;
                        case 300:
                            calamityGlobalNPC.newAI[0] = -maxDistance;
                            break;
                        case -300:
                            calamityGlobalNPC.newAI[0] = 0f;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    calamityGlobalNPC.newAI[0] = 0f;
                    calamityGlobalNPC.newAI[1] = -maxDistance;
                }

                npc.netSpam = 5;
                npc.SyncExtraAI();
                npc.netUpdate = true;
            }

            npc.ai[3] -= 1f +
                ((phase2 || turboEnrage) ? 1f : 0f) +
                ((phase3 || turboEnrage) ? 1f : 0f) +
                ((phase4 || turboEnrage) ? 2f : 0f);

            float offsetX = calamityGlobalNPC.newAI[0];
            float offsetY = calamityGlobalNPC.newAI[1];
            Vector2 destination = Main.player[npc.target].Center + new Vector2(offsetX, offsetY);

            // Velocity and acceleration
            float velocity = 7f +
                ((phase2 || turboEnrage) ? 4f : 0f) +
                ((phase3 || turboEnrage) ? 4f : 0f);

            if (enrage)
                velocity = (phase3 || turboEnrage) ? 25f : 20f;

            float acceleration = enrage ? 0.4f : phase3 ? 0.2f : phase2 ? 0.15f : 0.1f;

            // How far  is from where it's supposed to be
            Vector2 distanceFromDestination = destination - npc.Center;

            CalamityUtils.SmoothMovement(npc, 0f, distanceFromDestination, velocity, acceleration, true);

            if (death && calamityGlobalNPC.newAI[2] < 120f)
            {
                calamityGlobalNPC.newAI[2] += 1f;

                if (calamityGlobalNPC.newAI[2] % 15f == 0f)
                {
                    npc.netUpdate = true;
                    npc.SyncExtraAI();
                }

                return false;
            }

            // Fireballs
            float shootBoost = death ? 3f * (2f - (lifeRatio + golemLifeRatio)) : 2f * (2f - (lifeRatio + golemLifeRatio));
            npc.ai[1] += 1f + shootBoost;

            if (npc.ai[1] < 20f || npc.ai[1] > 340)
                npc.localAI[0] = 1f;
            else
                npc.localAI[0] = 0f;

            if (canPassThroughTiles && !phase3)
                npc.ai[1] = 20f;

            if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[1] >= 360 && Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 160f)
            {
                npc.TargetClosest();

                npc.ai[1] = 0f;

                Vector2 freeHeadCenter = new Vector2(npc.Center.X, npc.Center.Y - 10f);
                float freeHeadSpeed = turboEnrage ? 8f : enrage ? 6.5f : 5f;
                float freeHeadTargetX = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - freeHeadCenter.X;
                float freeHeadTargetY = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - freeHeadCenter.Y;
                float freeHeadTargetDist = (float)Math.Sqrt(freeHeadTargetX * freeHeadTargetX + freeHeadTargetY * freeHeadTargetY);

                freeHeadTargetDist = freeHeadSpeed / freeHeadTargetDist;
                freeHeadTargetX *= freeHeadTargetDist;
                freeHeadTargetY *= freeHeadTargetDist;

                int projectileType = phase3 ? ProjectileID.InfernoHostileBolt : ProjectileID.Fireball;
                int damage = npc.GetProjectileDamage(projectileType);
                int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), freeHeadCenter.X, freeHeadCenter.Y, freeHeadTargetX, freeHeadTargetY, projectileType, damage, 0f, Main.myPlayer, 0f, 0f);
                if (projectileType == ProjectileID.InfernoHostileBolt)
                {
                    Main.projectile[proj].timeLeft = 300;
                    Main.projectile[proj].ai[0] = Main.player[npc.target].Center.X;
                    Main.projectile[proj].ai[1] = Main.player[npc.target].Center.Y;
                    Main.projectile[proj].netUpdate = true;
                }

                npc.netUpdate = true;
            }

            // Lasers
            npc.ai[2] += 1f + shootBoost;
            if (!Collision.CanHit(Main.npc[NPC.golemBoss].Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                npc.ai[2] += 8f;

            if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] >= 300f && Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 160f)
            {
                npc.TargetClosest();

                npc.ai[2] = 0f;

                for (int i = 0; i < 2; i++)
                {
                    Vector2 freeHeadProjSpawn = new Vector2(npc.Center.X, npc.Center.Y - 50f);
                    if (Main.getGoodWorld)
                        freeHeadProjSpawn.Y += 30f;
                    if (i == 0)
                        freeHeadProjSpawn.X -= 14f;
                    else if (i == 1)
                        freeHeadProjSpawn.X += 14f;

                    float freeHeadProjSpeed = 5f + shootBoost;
                    float freeHeadProjTargetX = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - freeHeadProjSpawn.X;
                    float freeHeadProjTargetY = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - freeHeadProjSpawn.Y;
                    float freeHeadProjTargetDist = (float)Math.Sqrt(freeHeadProjTargetX * freeHeadProjTargetX + freeHeadProjTargetY * freeHeadProjTargetY);

                    freeHeadProjTargetDist = freeHeadProjSpeed / freeHeadProjTargetDist;
                    freeHeadProjTargetX *= freeHeadProjTargetDist;
                    freeHeadProjTargetY *= freeHeadProjTargetDist;
                    freeHeadProjSpawn.X += freeHeadProjTargetX * 3f;
                    freeHeadProjSpawn.Y += freeHeadProjTargetY * 3f;

                    int type = ProjectileID.EyeBeam;
                    int damage = npc.GetProjectileDamage(type);
                    int freeHeadLaser = Projectile.NewProjectile(npc.GetSource_FromAI(), freeHeadProjSpawn.X, freeHeadProjSpawn.Y, freeHeadProjTargetX, freeHeadProjTargetY, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    Main.projectile[freeHeadLaser].timeLeft = enrage ? 480 : 300;
                    if (turboEnrage && Main.getGoodWorld)
                        Main.projectile[freeHeadLaser].extraUpdates += 1;
                }
            }

            if (!Main.getGoodWorld)
            {
                npc.position += npc.netOffset;
                int randDustOffset = Main.rand.Next(2) * 2 - 1;
                Vector2 randDustPos = npc.Bottom + new Vector2((float)(randDustOffset * 22) * npc.scale, -22f * npc.scale);
                Dust getGoodDust = Dust.NewDustPerfect(randDustPos, 228, ((float)Math.PI / 2f + -(float)Math.PI / 2f * (float)randDustOffset + Main.rand.NextFloatDirection() * ((float)Math.PI / 4f)).ToRotationVector2() * (2f + Main.rand.NextFloat()));
                Dust dust = getGoodDust;
                dust.velocity += npc.velocity;
                getGoodDust.noGravity = true;
                getGoodDust = Dust.NewDustPerfect(npc.Bottom + new Vector2(Main.rand.NextFloatDirection() * 6f * npc.scale, (Main.rand.NextFloat() * -4f - 8f) * npc.scale), 228, Vector2.UnitY * (2f + Main.rand.NextFloat()));
                getGoodDust.fadeIn = 0f;
                getGoodDust.scale = 0.7f + Main.rand.NextFloat() * 0.5f;
                getGoodDust.noGravity = true;
                dust = getGoodDust;
                dust.velocity += npc.velocity;
                npc.position -= npc.netOffset;
            }

            return false;
        }
    }
}
