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
    public static class WallOfFleshAI
    {
        // Master Mode changes
        // 1 - Hungries spawn detached and have more health,
        // 2 - Moves quicker overall,
        // 3 - Mouth vomits a tight spread of 3 demon scythes at the same time as its leech vomit,
        // 4 - Eyes become immune to damage and stop firing when the wall drops below 15% health
        public static bool BuffedWallofFleshAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive && malice;

            // Despawn
            if (npc.position.X < 160f || npc.position.X > ((Main.maxTilesX - 10) * 16))
                npc.active = false;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Set Wall of Flesh variables
            if (npc.localAI[0] == 0f)
            {
                npc.localAI[0] = 1f;
                Main.wofB = -1;
                Main.wofT = -1;
            }

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Clamp life ratio to prevent bad velocity math.
            lifeRatio = MathHelper.Clamp(lifeRatio, 0f, 1f);

            // Phases based on HP
            bool phase2 = lifeRatio < 0.66f;
            bool phase3 = lifeRatio < 0.33f;

            // Start leech spawning based on HP
            npc.ai[1] += 1f;
            if (npc.ai[2] == 0f)
            {
                if (phase2)
                    npc.ai[1] += 1f;
                if (phase3)
                    npc.ai[1] += 1f;
                if (malice)
                    npc.ai[1] += 3f;

                if (npc.ai[1] > 2700f)
                    npc.ai[2] = 1f;
            }

            // Leech spawn
            if (npc.ai[2] > 0f && npc.ai[1] > 60f)
            {
                int num330 = phase3 ? 3 : 2;

                npc.ai[2] += 1f;
                npc.ai[1] = 0f;
                if (npc.ai[2] > num330)
                    npc.ai[2] = 0f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int num331 = NPC.NewNPC((int)(npc.position.X + (npc.width / 2)), (int)(npc.position.Y + (npc.height / 2) + 20f), NPCID.LeechHead, 1);
                    Main.npc[num331].velocity.X = npc.direction * 9;

                    if (phase2)
                    {
                        // Get target vector
                        Vector2 projectileVelocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * npc.velocity.Length();
                        Vector2 projectileSpawn = npc.Center + projectileVelocity * 5f;

                        int damage = npc.GetProjectileDamage(ProjectileID.DemonSickle);
                        int proj = Projectile.NewProjectile(npc.GetSpawnSource_ForProjectile(), projectileSpawn, projectileVelocity, ProjectileID.DemonSickle, damage, 0f, Main.myPlayer, 0f, projectileVelocity.Length() * 3f);
                        Main.projectile[proj].timeLeft = 600;
                        Main.projectile[proj].tileCollide = false;
                    }
                }
            }

            // Play sound
            npc.localAI[3] += 1f;
            if (npc.localAI[3] >= (600 + Main.rand.Next(1000)))
            {
                npc.localAI[3] = -Main.rand.Next(200);
                SoundEngine.PlaySound(SoundID.NPCDeath10, (int)npc.position.X, (int)npc.position.Y);
            }

            // Set whoAmI variable
            Main.wof = npc.whoAmI;

            // Set eye positions
            int num332 = (int)(npc.position.X / 16f);
            int num333 = (int)((npc.position.X + npc.width) / 16f);
            int num334 = (int)((npc.position.Y + (npc.height / 2)) / 16f);
            int num335 = 0;
            int num336 = num334 + 7;
            while (num335 < 15 && num336 > Main.maxTilesY - 200)
            {
                num336++;
                int num;
                for (int num337 = num332; num337 <= num333; num337 = num + 1)
                {
                    try
                    {
                        if (WorldGen.SolidTile(num337, num336) || Main.tile[num337, num336].LiquidAmount > 0)
                            num335++;
                    }
                    catch
                    { num335 += 15; }

                    num = num337;
                }
            }
            num336 += 4;
            if (Main.wofB == -1)
                Main.wofB = num336 * 16;
            else if (Main.wofB > num336 * 16)
            {
                Main.wofB--;
                if (Main.wofB < num336 * 16)
                    Main.wofB = num336 * 16;
            }
            else if (Main.wofB < num336 * 16)
            {
                Main.wofB++;
                if (Main.wofB > num336 * 16)
                    Main.wofB = num336 * 16;
            }

            num335 = 0;
            num336 = num334 - 7;
            while (num335 < 15 && num336 < Main.maxTilesY - 10)
            {
                num336--;
                int num;
                for (int num338 = num332; num338 <= num333; num338 = num + 1)
                {
                    try
                    {
                        if (WorldGen.SolidTile(num338, num336) || Main.tile[num338, num336].LiquidAmount > 0)
                            num335++;
                    }
                    catch
                    { num335 += 15; }

                    num = num338;
                }
            }
            num336 -= 4;
            if (Main.wofT == -1)
                Main.wofT = num336 * 16;
            else if (Main.wofT > num336 * 16)
            {
                Main.wofT--;
                if (Main.wofT < num336 * 16)
                    Main.wofT = num336 * 16;
            }
            else if (Main.wofT < num336 * 16)
            {
                Main.wofT++;
                if (Main.wofT > num336 * 16)
                    Main.wofT = num336 * 16;
            }

            // Set Y velocity and position
            float num339 = (Main.wofB + Main.wofT) / 2 - npc.height / 2;

            if (npc.position.Y > num339 + 1f)
                npc.velocity.Y = -1f;
            else if (npc.position.Y < num339 - 1f)
                npc.velocity.Y = 1f;
            npc.velocity.Y = 0f;

            int num340 = (Main.maxTilesY - 180) * 16;
            if (num339 < num340)
                num339 = num340;
            npc.position.Y = num339;

            float targetPosition = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2);
            float npcPosition = npc.position.X + (npc.width / 2);

            // Speed up if target is too far or if they're hiding behind tiles, slow down if too close
            float distanceFromTarget;
            if (npc.velocity.X < 0f)
                distanceFromTarget = npcPosition - targetPosition;
            else
                distanceFromTarget = targetPosition - npcPosition;

            float halfAverageScreenWidth = 960f;
            float distanceBeforeSlowingDown = 400f;
            float timeBeforeEnrage = 600f - (death ? 390f * (1f - lifeRatio) : 0f);
            float speedMult = 1f;

            if (malice)
                timeBeforeEnrage *= 0.25f;

            if (calamityGlobalNPC.newAI[0] < timeBeforeEnrage)
            {
                if (distanceFromTarget > halfAverageScreenWidth ||
                    !Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    speedMult += (distanceFromTarget - halfAverageScreenWidth) * 0.001f;
                    calamityGlobalNPC.newAI[0] += 1f;

                    // Enrage after 10 seconds of target being off screen
                    if (calamityGlobalNPC.newAI[0] >= timeBeforeEnrage)
                    {
                        calamityGlobalNPC.newAI[1] = 1f;

                        // Tell eyes to fire different lasers
                        npc.ai[3] = 1f;

                        // Play roar sound on players nearby
                        if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, npc.Center) < 2800f)
                            SoundEngine.PlaySound(SoundID.NPCKilled, (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y, 10, 1f, -0.25f);
                    }
                }
                else if (distanceFromTarget < distanceBeforeSlowingDown)
                    speedMult += (distanceFromTarget - distanceBeforeSlowingDown) * 0.002f;

                if (distanceFromTarget < halfAverageScreenWidth &&
                    Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    if (calamityGlobalNPC.newAI[0] > 0f)
                        calamityGlobalNPC.newAI[0] -= 1f;
                }

                speedMult = MathHelper.Clamp(speedMult, 0.75f, 2f);
            }

            // Enrage if target is off screen for too long
            if (calamityGlobalNPC.newAI[1] == 1f)
            {
                // Triple speed
                speedMult = 3.25f;

                // Return to normal if very close to target
                if (distanceFromTarget < distanceBeforeSlowingDown)
                {
                    npc.TargetClosest();
                    calamityGlobalNPC.newAI[0] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;
                    npc.ai[3] = 0f;
                }
            }

            if (malice)
                speedMult += 0.2f;

            // NOTE: Max velocity is 8 in expert mode

            float velocityBoost = 4f * (1f - lifeRatio);
            float velocityX = (BossRushEvent.BossRushActive ? 7f : death ? 3.5f : 2f) + velocityBoost;
            velocityX *= speedMult;

            // NOTE: Values below are based on Rev Mode only!
            // Max velocity without enrage is 12
            // Min velocity is 1.5
            // Max velocity with enrage is 18

            // Set X velocity
            if (npc.velocity.X == 0f)
                npc.velocity.X = npc.direction;

            if (npc.velocity.X < 0f)
            {
                npc.velocity.X = -velocityX;
                npc.direction = -1;
            }
            else
            {
                npc.velocity.X = velocityX;
                npc.direction = 1;
            }

            // Direction
            npc.spriteDirection = npc.direction;
            Vector2 vector37 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
            float num342 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector37.X;
            float num343 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector37.Y;
            float num344 = (float)Math.Sqrt(num342 * num342 + num343 * num343);
            num342 *= num344;
            num343 *= num344;

            // Rotation based on direction
            if (npc.direction > 0)
            {
                if (Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) > npc.position.X + (npc.width / 2))
                    npc.rotation = (float)Math.Atan2(-num343, -num342) + MathHelper.Pi;
                else
                    npc.rotation = 0f;
            }
            else if (Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) < npc.position.X + (npc.width / 2))
                npc.rotation = (float)Math.Atan2(num343, num342) + MathHelper.Pi;
            else
                npc.rotation = 0f;

            // Expert hungry respawn over time
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // Range of 2 to 11
                float spawnBoost = death ? 1f : (float)Math.Ceiling(lifeRatio * 10f);
                int chance = (int)(1f + spawnBoost);

                // Range of 4 to 121
                chance *= chance;

                // Range of 23 to 134
                chance = (chance * 19 + 400) / 20;

                // Range of 32 to 59
                if (chance < 60)
                    chance = (chance * 3 + 60) / 4;

                // Range of 64 to 268
                chance *= 2;

                if (malice)
                    chance /= 4;

                if (Main.rand.NextBool(chance))
                {
                    int num346 = 0;
                    float[] array = new float[10];
                    for (int num347 = 0; num347 < Main.maxNPCs; num347++)
                    {
                        if (num346 < 10 && Main.npc[num347].active && Main.npc[num347].type == NPCID.TheHungry)
                        {
                            array[num346] = Main.npc[num347].ai[0];
                            num346++;
                        }
                    }

                    int maxValue = 1 + num346 * 2;
                    if (num346 < 10 && Main.rand.Next(maxValue) <= 1)
                    {
                        int num348 = -1;
                        for (int num349 = 0; num349 < 1000; num349++)
                        {
                            int num350 = Main.rand.Next(10);
                            float num351 = num350 * 0.1f - 0.05f;
                            bool flag29 = true;
                            for (int num352 = 0; num352 < num346; num352++)
                            {
                                if (num351 == array[num352])
                                {
                                    flag29 = false;
                                    break;
                                }
                            }
                            if (flag29)
                            {
                                num348 = num350;
                                break;
                            }
                        }
                        if (num348 >= 0)
                        {
                            int num353 = NPC.NewNPC((int)npc.position.X, (int)num339, NPCID.TheHungry, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                            Main.npc[num353].ai[0] = num348 * 0.1f - 0.05f;
                        }
                    }
                }
            }

            // Spawn eyes and hungries
            if (npc.localAI[0] == 1f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[0] = 2f;

                num339 = (Main.wofB + Main.wofT) / 2;
                num339 = (num339 + Main.wofT) / 2f;
                int num354 = NPC.NewNPC((int)npc.position.X, (int)num339, NPCID.WallofFleshEye, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                Main.npc[num354].ai[0] = 1f;

                num339 = (Main.wofB + Main.wofT) / 2;
                num339 = (num339 + Main.wofB) / 2f;
                num354 = NPC.NewNPC((int)npc.position.X, (int)num339, NPCID.WallofFleshEye, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                Main.npc[num354].ai[0] = -1f;

                num339 = (Main.wofB + Main.wofT) / 2;
                num339 = (num339 + Main.wofB) / 2f;

                int num;
                for (int num355 = 0; num355 < 11; num355 = num + 1)
                {
                    num354 = NPC.NewNPC((int)npc.position.X, (int)num339, NPCID.TheHungry, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                    Main.npc[num354].ai[0] = num355 * 0.1f - 0.05f;
                    num = num355;
                }
            }

            return false;
        }

        public static bool BuffedWallofFleshEyeAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Despawn
            if (Main.wof < 0)
            {
                npc.active = false;
                return false;
            }

            npc.realLife = Main.wof;

            if (Main.npc[Main.wof].life > 0)
                npc.life = Main.npc[Main.wof].life;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Velocity, direction, and position
            npc.position.X = Main.npc[Main.wof].position.X;
            npc.direction = Main.npc[Main.wof].direction;
            npc.spriteDirection = npc.direction;

            float num356 = (Main.wofB + Main.wofT) / 2;
            if (npc.ai[0] > 0f)
                num356 = (num356 + Main.wofT) / 2f;
            else
                num356 = (num356 + Main.wofB) / 2f;
            num356 -= npc.height / 2;

            if (npc.position.Y > num356 + 1f)
                npc.velocity.Y = -1f;
            else if (npc.position.Y < num356 - 1f)
                npc.velocity.Y = 1f;
            else
            {
                npc.velocity.Y = 0f;
                npc.position.Y = num356;
            }

            if (npc.velocity.Y > 5f)
                npc.velocity.Y = 5f;
            if (npc.velocity.Y < -5f)
                npc.velocity.Y = -5f;

            Vector2 vector38 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
            float num357 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector38.X;
            float num358 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector38.Y;
            float num359 = (float)Math.Sqrt(num357 * num357 + num358 * num358);
            num357 *= num359;
            num358 *= num359;

            // Rotation based on direction and whether to fire lasers or not
            bool flag30 = true;
            if (npc.direction > 0)
            {
                if (Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) > npc.position.X + (npc.width / 2))
                    npc.rotation = (float)Math.Atan2(-num358, -num357) + MathHelper.Pi;
                else
                {
                    npc.rotation = 0f;
                    flag30 = false;
                }
            }
            else if (Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) < npc.position.X + (npc.width / 2))
                npc.rotation = (float)Math.Atan2(num358, num357) + MathHelper.Pi;
            else
            {
                npc.rotation = 0f;
                flag30 = false;
            }

            // Fire lasers
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // Percent life remaining
                float lifeRatio = Main.npc[Main.wof].life / (float)Main.npc[Main.wof].lifeMax;

                bool charging = Main.npc[Main.wof].ai[3] == 1f;

                // Set up enraged laser firing timer
                float enragedLaserTimer = 300f;
                if (charging)
                    npc.localAI[3] = enragedLaserTimer;

                bool fireAcceleratingLasers = npc.localAI[3] > 0f && npc.localAI[3] < enragedLaserTimer;

                // Decrement the enraged laser timer
                if (npc.localAI[3] > 0f)
                {
                    npc.localAI[3] -= 1f;

                    // Stop firing normal lasers when enrage ends
                    if (npc.localAI[3] == 0f)
                        npc.localAI[1] = 0f;
                }

                float shootBoost = fireAcceleratingLasers ? (death ? 1.5f : 1.5f * (1f - lifeRatio)) : (death ? 3f : 4f * (1f - lifeRatio));
                npc.localAI[1] += 1f + shootBoost;

                bool canHit = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);

                if (npc.localAI[2] == 0f)
                {
                    if (npc.localAI[1] > 400f || fireAcceleratingLasers)
                    {
                        npc.localAI[2] = 1f;
                        npc.localAI[1] = 0f;
                        npc.TargetClosest();
                    }
                }
                else if (npc.localAI[1] > 45f && (canHit || fireAcceleratingLasers) && !charging)
                {
                    npc.localAI[1] = 0f;
                    npc.localAI[2] += 1f;
                    if (npc.localAI[2] >= 4f)
                        npc.localAI[2] = 0f;

                    if (flag30)
                    {
                        bool phase2 = lifeRatio < 0.5 || malice;
                        float velocity = (fireAcceleratingLasers ? 3f : 9f) + shootBoost;

                        int projectileType = phase2 ? ProjectileID.DeathLaser : ProjectileID.EyeLaser;
                        int damage = npc.GetProjectileDamage(projectileType);

                        float laserSpawnDistance = fireAcceleratingLasers ? 30f : 10f;
                        Vector2 projectileVelocity = Vector2.Normalize(Main.player[npc.target].Center + (fireAcceleratingLasers ? Main.player[npc.target].velocity * 40f : Vector2.Zero) - npc.Center) * velocity;
                        Vector2 projectileSpawn = npc.Center + projectileVelocity * laserSpawnDistance;

                        int proj = Projectile.NewProjectile(npc.GetSpawnSource_ForProjectile(), projectileSpawn, projectileVelocity, projectileType, damage, 0f, Main.myPlayer, fireAcceleratingLasers ? 1f : 0f, 0f);
                        Main.projectile[proj].timeLeft = 900;

                        if (!canHit)
                            Main.projectile[proj].tileCollide = false;

                    }
                }
            }

            return false;
        }
    }
}
